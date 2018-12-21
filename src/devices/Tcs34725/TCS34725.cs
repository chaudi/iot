// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Iot.Device.Tcs34725
{
    //https://github.com/adafruit/Adafruit_TCS34725
    public class Tcs34725 : IDisposable
    {
        private const byte Signature = 0x58;

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

            //    I2CDevice = await I2cDevice.FromIdAsync(dic[0].Id, settings);

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

        /**************************************************************************/
        /*! 
            Initializes I2C and configures the sensor (call this function before 
            doing anything else) 
        */
        /**************************************************************************/
        public void Begin()
        {
            Debug.WriteLine("Tcs34725 BEGIN");

            /* Make sure we're actually connected */
            byte x = Read8BitsFromRegister((byte)Register.ID);
            if ((x != 0x44) && (x != 0x10))
            {
                return;
            }
            _initialized = true;

            /* Note: by default, the device is in power down mode on bootup */
            Enable();
        }

        public void SetGain(Gain gain)
        {
            _gain = gain;
        }

        public void SetIntegrationTime(IntegrationTime integrationTime)
        {
            _integrationTime = integrationTime;
        }

        public void Write(byte register, byte data)
        {
            byte[] writeBuffer = new byte[] { register, data };
            _i2cDevice.Write(writeBuffer);
        }

        /**************************************************************************/
        /*! 
            @brief  Reads the raw red, green, blue and clear channel values 
        */
        /**************************************************************************/
        private async Task<Tcs34725Color> GetRawData()
        {
            if (!_initialized) Begin();

            byte c = Read8BitsFromRegister((byte)Register.CDATAL);
            byte r = Read8BitsFromRegister((byte)Register.RDATAL);
            byte g = Read8BitsFromRegister((byte)Register.GDATAL);
            byte b = Read8BitsFromRegister((byte)Register.BDATAL);

            /* Set a delay for the integration time */
            switch (_integrationTime)
            {
                case IntegrationTime.T2_4MS:
                    //delay(3);
                    await Task.Delay(3);
                    break;
                case IntegrationTime.T24MS:
                    //delay(24);
                    await Task.Delay(24);
                    break;
                case IntegrationTime.T50MS:
                    //delay(50);
                    await Task.Delay(50);
                    break;
                case IntegrationTime.T101MS:
                    //delay(101);
                    await Task.Delay(101);
                    break;
                case IntegrationTime.T154MS:
                    //delay(154);
                    await Task.Delay(154);
                    break;
                case IntegrationTime.T700MS:
                    //delay(700);
                    await Task.Delay(700);
                    break;
            }
            return new Tcs34725Color(r, g, b, c);
        }

        /**************************************************************************/
        /*! 
            Enables the device 
        */
        /**************************************************************************/
        private async void Enable()
        {
            Write((byte)Register.ENABLE, (byte)Register.ENABLE_PON);
            await Task.Delay(3);
            Write((byte)Register.ENABLE, (byte)Register.ENABLE_PON | (byte)Register.ENABLE_AEN);
        }

        /**************************************************************************/
        /*! 
            Disables the device (putting it in lower power sleep mode) 
        */
        /**************************************************************************/
        private void Disable()
        {
            /* Turn the device off to save power */
            byte reg = 0;
            reg = Read8BitsFromRegister((byte)Register.ENABLE);
            int value = ~(((byte)Register.ENABLE_PON | (byte)Register.ENABLE_AEN));
            Write((byte)Register.ENABLE, (byte)(reg & value));
        }

        /**************************************************************************/
        /*! 
            @brief  Converts the raw R/G/B values to color temperature in degrees 
                    Kelvin 
        */
        /**************************************************************************/
        public double CalculateColorTemperature(short r, short g, short b)
        {
            double X, Y, Z;      /* RGB to XYZ correlation      */
            double xc, yc;       /* Chromaticity co-ordinates   */
            double n;            /* McCamy's formula            */
            double cct;

            /* 1. Map RGB values to their XYZ counterparts.    */
            /* Based on 6500K fluorescent, 3000K fluorescent   */
            /* and 60W incandescent values for a wide range.   */
            /* Note: Y = Illuminance or lux                    */
            X = (-0.14282F * r) + (1.54924F * g) + (-0.95641F * b);
            Y = (-0.32466F * r) + (1.57837F * g) + (-0.73191F * b);
            Z = (-0.68202F * r) + (0.77073F * g) + (0.56332F * b);

            /* 2. Calculate the chromaticity co-ordinates      */
            xc = (X) / (X + Y + Z);
            yc = (Y) / (X + Y + Z);

            /* 3. Use McCamy's formula to determine the CCT    */
            n = (xc - 0.3320F) / (0.1858F - yc);

            /* Calculate the final CCT */
            cct = (449.0F * Math.Pow(n, 3)) + (3525.0F * Math.Pow(n, 2)) + (6823.3F * n) + 5520.33F;

            /* Return the results in degrees Kelvin */
            return cct;
        }

        /**************************************************************************/
        /*! 
            @brief  Converts the raw R/G/B values to lux 
        */
        /**************************************************************************/
        public double CalculateLux(short r, short g, short b)
        {
            float illuminance;

            /* This only uses RGB ... how can we integrate clear or calculate lux */
            /* based exclusively on clear since this might be more reliable?      */
            illuminance = (-0.32466f * r) + (1.57837f * g) + (-0.73191f * b);

            return illuminance;
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
            throw new NotImplementedException();
        }
    }
}
