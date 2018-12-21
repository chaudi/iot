// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Datasheet https://ams.com/documents/20143/36005/TCS3472_DS000390_2-00.pdf
//Code converted from https://github.com/adafruit/Adafruit_TCS34725

using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Diagnostics;
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
        private readonly byte CommandBit = 0x80;
        private Gain _gain;
        private IntegrationTime _integrationTime;

        private enum CommunicationProtocol
        {
            I2c
        }

        public Tcs34725(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
        }

        public Tcs34725(IntegrationTime time = IntegrationTime.T2_4MS, Gain gain = Gain.GAIN_1X, byte address = 0x29, byte commandbit = 0x80)
        {
            _integrationTime = time;
            _gain = gain;
            CommandBit = commandbit;
        }

        public async Task Initialize()
        {
            //Debug.WriteLine("Tcs34725 initialized");
            //try
            //{
            //    I2cConnectionSettings settings = new I2cConnectionSettings(Address);

            //    settings.BusSpeed = I2cBusSpeed.FastMode;

            //    String aqs = I2cDevice.GetDeviceSelector(I2CControllerName);

            //    DeviceInformationCollection dic = await DeviceInformation.FindAllAsync(aqs);

            //    I2CDevice = await I2cDevice.FromIdAsync(dic[0].Id34725, settings);

            //    if (I2CDevice == null)
            //    {
            //        Debug.WriteLine("Device not found");
            //    }
            //    initialised = true;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
            //    throw;
            //}
        }

        /// <summary>
        ///  Initializes I2C and configures the sensor (call this function before doing anything else) 
        /// </summary>
        public void Begin()
        {
            Debug.WriteLine("Tcs34725 BEGIN");

            // Make sure we're actually connected 
            byte x = Read8BitsFromRegister((byte)Register.ID);
            if ((x != 0x44) && (x != 0x10))
            {
                return;
            }
            _initialized = true;

            // Note: by default, the i2cDevice is in power down mode on bootup 
            Enable();
        }

        public void SetGain(Gain gain)
        {
            if (!_initialized) Begin();
            Write((byte)Register.CONTROL_ANALOG_GAIN, (byte)gain);
            _gain = gain;
        }

        public void SetIntegrationTime(IntegrationTime integrationTime)
        {
            if (!_initialized) Begin();
            Write((byte)Register.ATIME, (byte)integrationTime);
            _integrationTime = integrationTime;
        }

        public void Write(byte register, byte data)
        {
            byte[] writeBuffer = new byte[] { register, data };
            _i2cDevice.Write(writeBuffer);
        }

        /// <summary>
        /// Reads the raw red, green, blue and clear channel values 
        /// </summary>
        /// <returns></returns>
        private async Task<Tcs34725Color> GetRawData()
        {
            if (!_initialized) Begin();

            ushort clear = Read16BitsFromRegister((byte)Register.CDATAL);
            ushort red = Read16BitsFromRegister((byte)Register.RDATAL);
            ushort green = Read16BitsFromRegister((byte)Register.GDATAL);
            ushort blue = Read16BitsFromRegister((byte)Register.BDATAL);

            // Set a delay for the integration time 
            switch (_integrationTime)
            {
                case IntegrationTime.T2_4MS:
                    await Task.Delay(3);
                    break;
                case IntegrationTime.T24MS:
                    await Task.Delay(24);
                    break;
                case IntegrationTime.T50MS:
                    await Task.Delay(50);
                    break;
                case IntegrationTime.T101MS:
                    await Task.Delay(101);
                    break;
                case IntegrationTime.T154MS:
                    await Task.Delay(154);
                    break;
                case IntegrationTime.T700MS:
                    await Task.Delay(700);
                    break;
            }
            return new Tcs34725Color(red, green, blue, clear);
        }

        async Task<Tcs34725Color> GetRawDataOneShot()
        {
            if (!_initialized) Begin();

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
            Write((byte)Register.ENABLE, (byte)Register.ENABLE_POWER_ON);
            await Task.Delay(3);
            Write((byte)Register.ENABLE, (byte)Register.ENABLE_POWER_ON | (byte)Register.ENABLE_AEN);
        }

        /// <summary>
        ///  Disables the i2cDevice (putting it in lower power sleep mode) 
        /// </summary>
        private void Disable()
        {
            // Turn the i2cDevice off to save power 
            byte reg = 0;
            reg = Read8BitsFromRegister((byte)Register.ENABLE);
            int value = ~(((byte)Register.ENABLE_POWER_ON | (byte)Register.ENABLE_AEN));
            Write((byte)Register.ENABLE, (byte)(reg & value));
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
            double X, Y, Z;
            //Chromaticity co-ordinates
            double xc, yc;
            //McCamy's formula  
            double n;
            double cct;

            // 1. Map RGB values to their XYZ counterparts.    
            // Based on 6500K fluorescent, 3000K fluorescent   
            // and 60W incandescent values for a wide range.  
            // Note: Y = Illuminance or lux                    
            X = (-0.14282F * red) + (1.54924F * green) + (-0.95641F * blue);
            Y = (-0.32466F * red) + (1.57837F * green) + (-0.73191F * blue);
            Z = (-0.68202F * red) + (0.77073F * green) + (0.56332F * blue);

            // 2. Calculate the chromaticity co-ordinates      
            xc = (X) / (X + Y + Z);
            yc = (Y) / (X + Y + Z);

            // 3. Use McCamy's formula to determine the CCT    
            n = (xc - 0.3320F) / (0.1858F - yc);

            // Calculate the final CCT 
            cct = (449.0F * Math.Pow(n, 3)) + (3525.0F * Math.Pow(n, 2)) + (6823.3F * n) + 5520.33F;

            // Return the results in degrees Kelvin 
            return (ushort)cct;
        }

        /**************************************************************************/
        /*!
            @brief  Converts the raw R/G/B values to color temperature in degrees
                    Kelvin using the algorithm described in DN40 from Taos (now AMS).
        */
        /**************************************************************************/
        ushort CalculateColorTemperature_dn40(ushort r, ushort g, ushort b, ushort c)
        {
            int rc;                     /* Error return code */
            ushort r2, g2, b2;        /* RGB values minus IR component */
            int gl;                     /* Results of the initial lux conversion */
            ushort gain_int;           /* Gain multiplier as a normal integer */
            ushort sat;               /* Digital saturation level */
            ushort ir;                /* Inferred IR content */

            /* Analog/Digital saturation:
             *
             * (a) As light becomes brighter, the clear channel will tend to
             *     saturate first since R+G+B is approximately equal to C.
             * (blue) The TCS34725 accumulates 1024 counts per 2.4ms of integration
             *     time, up to a maximum values of 65535. This means analog
             *     saturation can occur up to an integration time of 153.6ms
             *     (64*2.4ms=153.6ms).
             * (c) If the integration time is > 153.6ms, digital saturation will
             *     occur before analog saturation. Digital saturation occurs when
             *     the count reaches 65535.
             */
            if ((256 - (byte)_integrationTime) > 63)
            {
                /* Track digital saturation */
                sat = 65535;
            }
            else
            {
                /* Track analog saturation */
                sat = (ushort)(1024 * (256 - (byte)_integrationTime));
            }

            /* Ripple rejection:
             *
             * (a) An integration time of 50ms or multiples of 50ms are required to
             *     reject both 50Hz and 60Hz ripple.
             * (blue) If an integration time faster than 50ms is required, you may need
             *     to average a number of samples over a 50ms period to reject ripple
             *     from fluorescent and incandescent light sources.
             *
             * Ripple saturation notes:
             *
             * (a) If there is ripple in the received signal, the value read from C
             *     will be less than the max, but still have some effects of being
             *     saturated. This means that you can be below the 'sat' value, but
             *     still be saturating. At integration times >150ms this can be
             *     ignored, but <= 150ms you should calculate the 75% saturation
             *     level to avoid this problem.
             */
            if ((256 - (byte)_integrationTime) <= 63)
            {
                /* Adjust sat to 75% to avoid analog saturation if atime < 153.6ms */
                sat -= (ushort)(sat / 4);
            }

            /* Check for saturation and mark the sample as invalid if true */
            if (c >= sat)
            {
                return 0;
            }

            /* AMS RGB sensors have no IR channel, so the IR content must be */
            /* calculated indirectly. */
            ir = (r + g + b > c) ? (ushort)((r + g + b - c) / 2) : (ushort)0;

            /* Remove the IR component from the raw RGB values */
            r2 = (ushort)(r - ir);
            g2 = (ushort)(g - ir);
            b2 = (ushort)(b - ir);

            /* Convert gain to a usable integer value */
            switch (_gain)
            {
                case Gain.GAIN_4X: /* GAIN 4X */
                    gain_int = 4;
                    break;
                case Gain.GAIN_16X: /* GAIN 16X */
                    gain_int = 16;
                    break;
                case Gain.GAIN_60X: /* GAIN 60X */
                    gain_int = 60;
                    break;
                case Gain.GAIN_1X: /* GAIN 1X */
                default:
                    gain_int = 1;
                    break;
            }

            /* Calculate the counts per lux (CPL), taking into account the optional
             * arguments for Glass Attenuation (GA) and Device Factor (DF).
             *
             * GA = 1/T where T is glass transmissivity, meaning if glass is 50%
             * transmissive, the GA is 2 (1/0.5=2), and if the glass attenuates light
             * 95% the GA is 20 (1/0.05). A GA of 1.0 assumes perfect transmission.
             *
             * NOTE: It is recommended to have a CPL > 5 to have a lux accuracy
             *       < +/- 0.5 lux, where the digitization error can be calculated via:
             *       'DER = (+/-2) / CPL'.
             */
            float cpl = (((256 - (byte)_integrationTime) * 2.4f) * gain_int) / (1.0f * 310.0f);

            /* Determine lux accuracy (+/- lux) */
            float der = 2.0f / cpl;

            /* Determine the maximum lux value */
            double max_lux = 65535.0 / (cpl * 3);

            /* Lux is a function of the IR-compensated RGB channels and the associated
             * color coefficients, with G having a particularly heavy influence to
             * match the nature of the human eye.
             *
             * NOTE: The green value should be > 10 to ensure the accuracy of the lux
             *       conversions. If it is below 10, the gain should be increased, but
             *       the clear<100 check earlier should cover this edge case.
             */
            gl = (int)(0.136f * r2 +                   /** Red coefficient. */
                  1.000f * g2 +                   /** Green coefficient. */
                 -0.444f * b2);                    /** Blue coefficient. */

            float lux = gl / cpl;

            /* A simple method of measuring color temp is to use the ratio of blue */
            /* to red light, taking IR cancellation into account. */
            ushort cct = (ushort)((3810 * (uint)b2) /      /** Color temp coefficient. */
                           (uint)r2 + 1391);         /** Color temp offset. */

            return cct;
        }

        /// <summary>
        /// Converts the raw R/G/B values to lux 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public ushort CalculateLux(ushort r, ushort g, ushort b)
        {
            float illuminance;

            // This only uses RGB ... how can we integrate clear or calculate lux 
            // based exclusively on clear since this might be more reliable?      
            illuminance = (-0.32466f * r) + (1.57837f * g) + (-0.73191f * b);

            return (ushort)illuminance;
        }

        void SetInterrupt(bool i)
        {
            byte r = Read8BitsFromRegister((byte)Register.ENABLE);
            if (i)
            {
                r |= (byte)Register.ENABLE_AIEN;
            }
            else
            {
                r &= (byte)(~Register.ENABLE_AIEN);
            }
            Write((byte)Register.ENABLE, r);
        }

        void ClearInterrupt()
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
        internal ushort Read16BitsFromRegister(byte register)
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
