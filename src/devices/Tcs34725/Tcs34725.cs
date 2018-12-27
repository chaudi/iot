// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Datasheet https://ams.com/documents/20143/36005/TCS3472_DS000390_2-00.pdf
//Code converted from https://github.com/adafruit/Adafruit_TCS34725

using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Threading.Tasks;

namespace Iot.Device.Tcs34725
{
    public class Tcs34725 : IDisposable
    {
        private const byte Signature = 0x58;
        private const byte Id34725 = 0x44;
        private const byte Id34727 = 0x4D;

        private I2cDevice _i2cDevice;
        private readonly CommunicationProtocol _communicationProtocol;
        private bool _initialized = false;
        private readonly byte _commandBit = 0x80;
        private Gain _gain;
        private IntegrationTime _integrationTime;

        private enum CommunicationProtocol
        {
            I2c
        }

        public Tcs34725(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
            _communicationProtocol = CommunicationProtocol.I2c;
        }

        /// <summary>
        ///  Initializes I2C and configures the sensor (call this function before doing anything else) 
        /// </summary>
        public void Initialize()
        {
            // Make sure we're actually connected 
            byte x = Read8BitsFromRegister((byte)Register.Id);
            if ((x != Id34725) && (x != 0x10))
            {
                return;
            }
            _initialized = true;

            // Note: by default, the i2cDevice is in power down mode on bootup 
            Enable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTime"></param>
        public void SetWait(WaitTime waitTime)
        {
            Write((byte)Register.WaitTime, (byte)waitTime);
        }

        /// <summary>
        /// Wait long. If set the wait cycles are increased by a factor of 12.
        /// </summary>
        /// <param name="value"></param>
        public void SetWaitLong(bool value)
        {
            if (true)
            {
                Write((byte)Register.Configuration, 0b0010);
            }
            Write((byte)Register.Configuration, 0b0000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gain"></param>
        public void SetGain(Gain gain)
        {
            if (!_initialized) Initialize();
            Write((byte)Register.ControlAnalogGain, (byte)gain);
            _gain = gain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integrationTime"></param>
        public void SetIntegrationTime(IntegrationTime integrationTime)
        {
            if (!_initialized) Initialize();
            Write((byte)Register.RgbcTiming, (byte)integrationTime);
            _integrationTime = integrationTime;
        }

        /// <summary>
        /// Reads the raw red, green, blue and clear channel values 
        /// </summary>
        /// <returns></returns>
        private async Task<Tcs34725Color> GetRawData()
        {
            if (!_initialized) Initialize();

            ushort clear = Read16BitsFromRegister((byte)Register.ClearDataLow);
            ushort red = Read16BitsFromRegister((byte)Register.RedDataLow);
            ushort green = Read16BitsFromRegister((byte)Register.GreenDataLow);
            ushort blue = Read16BitsFromRegister((byte)Register.BlueDataLow);

            // Set a delay for the integration time 
            switch (_integrationTime)
            {
                case IntegrationTime.Ms2Point4:
                    await Task.Delay(3);
                    break;
                case IntegrationTime.Ms24:
                    await Task.Delay(24);
                    break;
                case IntegrationTime.Ms50:
                    await Task.Delay(50);
                    break;
                case IntegrationTime.Ms101:
                    await Task.Delay(101);
                    break;
                case IntegrationTime.Ms154:
                    await Task.Delay(154);
                    break;
                case IntegrationTime.Ms700:
                    await Task.Delay(700);
                    break;
            }
            return new Tcs34725Color(red, green, blue, clear);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Tcs34725Color> GetRawDataOneShot()
        {
            if (!_initialized) Initialize();

            Enable();
            var result = await GetRawData();
            Disable();
            return result;
        }

        /// <summary>
        /// Enables the i2cDevice 
        /// </summary>
        private async void Enable()
        {
            Write((byte)Register.Enable, (byte)EnableRegisterBit.PowerOn);
            await Task.Delay(3);
            Write((byte)Register.Enable, (byte)EnableRegisterBit.PowerOn | (byte)EnableRegisterBit.Enable);
        }

        /// <summary>
        ///  Disables the i2cDevice (putting it in lower power sleep mode) 
        /// </summary>
        private void Disable()
        {
            // Turn the i2cDevice off to save power 
            byte currentRegisterValue;
            currentRegisterValue = Read8BitsFromRegister((byte)Register.Enable);
            int value = ~((byte)EnableRegisterBit.PowerOn | (byte)EnableRegisterBit.Enable);
            Write((byte)Register.Enable, (byte)(currentRegisterValue & value));
        }

        /// <summary>
        /// Converts the raw R/G/B values to color temperature in degrees Kelvin
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public ushort CalculateColorTemperature(ushort red, ushort green, ushort blue)
        {
            //RGB to XYZ correlation
            double x, y, z;
            //Chromaticity co-ordinates
            double xChromatic, yChromatic;
            //McCamy's formula  
            double cct;

            // 1. Map RGB values to their XYZ counterparts.    
            // Based on 6500K fluorescent, 3000K fluorescent   
            // and 60W incandescent values for a wide range.  
            // Note: Y = Illuminance or lux                    
            x = (-0.14282F * red) + (1.54924F * green) + (-0.95641F * blue);
            y = (-0.32466F * red) + (1.57837F * green) + (-0.73191F * blue);
            z = (-0.68202F * red) + (0.77073F * green) + (0.56332F * blue);

            // 2. Calculate the chromaticity co-ordinates      
            xChromatic = (x) / (x + y + z);
            yChromatic = (y) / (x + y + z);

            // 3. Use McCamy's formula to determine the CCT    
            double n = (xChromatic - 0.3320F) / (0.1858F - yChromatic);

            // Calculate the final CCT 
            cct = (449.0F * Math.Pow(n, 3)) + (3525.0F * Math.Pow(n, 2)) + (6823.3F * n) + 5520.33F;

            // Return the results in degrees Kelvin 
            return (ushort)cct;
        }

        /// <summary>
        /// Converts the raw R/G/B values to color temperature in degrees
        /// Kelvin using the algorithm described in DN40 from Taos(now AMS).
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        public ushort CalculateColorTemperature_dn40(ushort red, ushort green, ushort blue, ushort clear)
        {
            //RGB values minus IR component
            ushort red2, green2, blue2;
            //Results of the initial lux conversion
            int gl;
            //Gain multiplier as a normal integer
            ushort gain;
            //Digital saturation level
            ushort saturation;
            //Inferred IR content
            ushort ir;

            //Enable/Digital saturation:
            //(a) As light becomes brighter, the clear channel will tend to
            //    saturate first since R+G+B is approximately equal to C.
            //(blue) The TCS34725 accumulates 1024 counts per 2.4ms of integration
            //    time, up to a maximum values of 65535. This means analog
            //    saturation can occur up to an integration time of 153.6ms
            //   (64*2.4ms=153.6ms).
            //(clear) If the integration time is > 153.6ms, digital saturation will
            //    occur before analog saturation. Digital saturation occurs when
            //    the count reaches 65535.
            if ((256 - (byte)_integrationTime) > 63)
            {
                // Track digital saturation
                saturation = 65535;
            }
            else
            {
                // Track analog saturation
                saturation = (ushort)(1024 * (256 - (byte)_integrationTime));
            }

            //Ripple rejection:
            // (a) An integration time of 50ms or multiples of 50ms are required to
            //     reject both 50Hz and 60Hz ripple.
            // (blue) If an integration time faster than 50ms is required, you may need
            //     to average a number of samples over a 50ms period to reject ripple
            //     from fluorescent and incandescent light sources.

            // Ripple saturation notes:
            // (a) If there is ripple in the received signal, the value read from C
            //     will be less than the max, but still have some effects of being
            //     saturated. This means that you can be below the 'sat' value, but
            //     still be saturating. At integration times >150ms this can be
            //     ignored, but <= 150ms you should calculate the 75% saturation
            //     level to avoid this problem.

            if ((256 - (byte)_integrationTime) <= 63)
            {
                // Adjust sat to 75% to avoid analog saturation if atime < 153.6ms 
                saturation -= (ushort)(saturation / 4);
            }

            // Check for saturation and mark the sample as invalid if true 
            if (clear >= saturation)
            {
                return 0;
            }

            // AMS RGB sensors have no IR channel, so the IR content must be
            // calculated indirectly. 
            ir = (red + green + blue > clear) ? (ushort)((red + green + blue - clear) / 2) : (ushort)0;

            // Remove the IR component from the raw RGB values 
            red2 = (ushort)(red - ir);
            green2 = (ushort)(green - ir);
            blue2 = (ushort)(blue - ir);

            // Convert gain to a usable integer value
            switch (_gain)
            {
                case Gain.X4:
                    gain = 4;
                    break;
                case Gain.X16:
                    gain = 16;
                    break;
                case Gain.X60:
                    gain = 60;
                    break;
                case Gain.X1:
                default:
                    gain = 1;
                    break;
            }

            //Calculate the counts per lux (CPL), taking into account the optional
            // arguments for Glass Attenuation (GA) and Device Factor (DF).

            // GA = 1/T where T is glass transmissivity, meaning if glass is 50%
            // transmissive, the GA is 2 (1/0.5=2), and if the glass attenuates light
            // 95% the GA is 20 (1/0.05). A GA of 1.0 assumes perfect transmission.

            // NOTE: It is recommended to have a CPL > 5 to have a lux accuracy
            //       < +/- 0.5 lux, where the digitization error can be calculated via:
            //       'DER = (+/-2) / CPL'.
            float cpl = (((256 - (byte)_integrationTime) * 2.4f) * gain) / (1.0f * 310.0f);

            // Determine lux accuracy (+/- lux)
            float der = 2.0f / cpl;

            // Determine the maximum lux value 
            double max_lux = 65535.0 / (cpl * 3);

            //Lux is a function of the IR-compensated RGB channels and the associated
            // color coefficients, with G having a particularly heavy influence to
            // match the nature of the human eye.

            // NOTE: The green value should be > 10 to ensure the accuracy of the lux
            //       conversions. If it is below 10, the gain should be increased, but
            //       the clear<100 check earlier should cover this edge case.
            gl = (int)(0.136f * red2 +                   /** Red coefficient. */
                  1.000f * green2 +                   /** Green coefficient. */
                 -0.444f * blue2);                    /** Blue coefficient. */

            float lux = gl / cpl;

            // A simple method of measuring color temp is to use the ratio of blue 
            // to red light, taking IR cancellation into account. 
            ushort cct = (ushort)((3810 * (uint)blue2) /      /** Color temp coefficient. */
                           (uint)red2 + 1391);         /** Color temp offset. */

            return cct;
        }

        /// <summary>
        /// Converts the raw R/G/B values to lux 
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public ushort CalculateLux(ushort red, ushort green, ushort blue)
        {
            // This only uses RGB ... how can we integrate clear or calculate lux 
            // based exclusively on clear since this might be more reliable?      
            var illuminance = (-0.32466f * red) + (1.57837f * green) + (-0.73191f * blue);

            return (ushort)illuminance;
        }

        /// <summary>
        /// RGBC interrupt enable. When asserted, permits RGBC interrupts to be generated
        /// </summary>
        /// <param name="interrupt"></param>
        public void SetInterrupt(bool interrupt)
        {
            byte data = Read8BitsFromRegister((byte)Register.Enable);
            if (interrupt)
            {
                //enable
                data |= (byte)EnableRegisterBit.Interrupt;
            }
            else
            {
                //disable
                data &= (byte)(~EnableRegisterBit.Interrupt);
            }
            Write((byte)Register.Enable, data);
        }

        public void ClearInterrupt()
        {
            throw new NotImplementedException();

            //            Wire.beginTransmission(TCS34725_ADDRESS);
            //#if ARDUINO >= 100
            //  Wire.write(TCS34725_COMMAND_BIT | 0x66);
            //#else
            //            Wire.send(TCS34725_COMMAND_BIT | 0x66);
            //#endif
            //            Wire.endTransmission();
        }

        /// <summary>
        /// Writes the data to the given register
        /// </summary>
        /// <param name="register">Register to write to</param>
        /// <param name="data">Data to write to the register</param>
        private void Write(byte register, byte data)
        {
            Span<byte> writeBuffer = stackalloc byte[2] { register, data };
            _i2cDevice.Write(writeBuffer);
        }

        /// <summary>
        ///  Reads an 8 bit value from a register
        /// </summary>
        /// <param name="register">
        ///  Register to read from
        /// </param>
        /// <returns>
        ///  Value from register
        /// </returns>
        private byte Read8BitsFromRegister(byte register)
        {
            if (_communicationProtocol == CommunicationProtocol.I2c)
            {
                _i2cDevice.WriteByte(register);
                byte value = _i2cDevice.ReadByte();
                return value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///  Reads a 16 bit value over I2C 
        /// </summary>
        /// <param name="register">
        ///  Register to read from
        /// </param>
        /// <returns>
        ///  Value from register
        /// </returns>
        private ushort Read16BitsFromRegister(byte register)
        {
            if (_communicationProtocol == CommunicationProtocol.I2c)
            {
                Span<byte> bytes = stackalloc byte[2];

                _i2cDevice.WriteByte(register);
                _i2cDevice.Read(bytes);

                return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///  Reads a 24 bit value over I2C 
        /// </summary>
        /// <param name="register">
        ///  Register to read from
        /// </param>
        /// <returns>
        ///  Value from register
        /// </returns>
        private uint Read24BitsFromRegister(byte register)
        {
            if (_communicationProtocol == CommunicationProtocol.I2c)
            {
                Span<byte> bytes = stackalloc byte[4];

                _i2cDevice.WriteByte(register);
                _i2cDevice.Read(bytes.Slice(1));

                return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            _i2cDevice = null;
        }
    }
}
