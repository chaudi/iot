using System;
using System.Device.I2c;

namespace Iot.Device
{
    public class Bmp280 : IDisposable
    {
        private I2cDevice _i2cDevice;
        private CommunicationProtocol protocol;
        private bool initialised = false;
        private Bmp280CalibrationData CalibrationData;
        protected readonly byte Signature = 0x58;
        private int TFine;

        private enum CommunicationProtocol
        {
            Gpio,
            Spi,
            I2c
        }

        public Bmp280(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
            CalibrationData = new Bmp280CalibrationData();
            protocol = CommunicationProtocol.I2c;
        }

        private void Begin()
        {
            byte[] readBuffer = new byte[] { 0xFF };

            _i2cDevice.WriteByte((byte)Registers.REGISTER_CHIPID);
            _i2cDevice.Read(readBuffer);

            if (readBuffer[0] != Signature)
            {
                return;
            }
            initialised = true;

            //Read the coefficients table
            ReadCoefficients();

            WriteControlReguster();
        }

        /// <summary>
        ///  Reads the factory-set coefficients 
        /// </summary>
        private void ReadCoefficients()
        {
            // Read temperature calibration data
            CalibrationData.DigT1 = Read16((byte)Registers.REGISTER_DIG_T1);
            CalibrationData.DigT2 = (short)Read16((byte)Registers.REGISTER_DIG_T2);
            CalibrationData.DigT3 = (short)Read16((byte)Registers.REGISTER_DIG_T3);

            // Read presure calibration data
            CalibrationData.DigP1 = Read16((byte)Registers.REGISTER_DIG_P1);
            CalibrationData.DigP2 = (short)Read16((byte)Registers.REGISTER_DIG_P2);
            CalibrationData.DigP3 = (short)Read16((byte)Registers.REGISTER_DIG_P3);
            CalibrationData.DigP4 = (short)Read16((byte)Registers.REGISTER_DIG_P4);
            CalibrationData.DigP5 = (short)Read16((byte)Registers.REGISTER_DIG_P5);
            CalibrationData.DigP6 = (short)Read16((byte)Registers.REGISTER_DIG_P6);
            CalibrationData.DigP7 = (short)Read16((byte)Registers.REGISTER_DIG_P7);
            CalibrationData.DigP8 = (short)Read16((byte)Registers.REGISTER_DIG_P8);
            CalibrationData.DigP9 = (short)Read16((byte)Registers.REGISTER_DIG_P9);

        }

        private void WriteControlReguster()
        {
            byte[] writebuffer = new byte[] { (byte)Registers.REGISTER_CONTROL, 0x3F };
            _i2cDevice.Write(writebuffer);
        }

        /// <summary>
        ///  Reads the temperature from the sensor
        /// </summary>
        /// <returns>
        ///  Temperature in degrees celsius
        /// </returns>
        public double ReadTemperature()
        {
            //Make sure the I2C device is initialized
            if (!initialised) Begin();

            //Read the MSB, LSB and bits 7:4 (XLSB) of the temperature from the BMP280 registers
            byte tmsb = Read8((byte)Registers.REGISTER_TEMPDATA_MSB);
            byte tlsb = Read8((byte)Registers.REGISTER_TEMPDATA_LSB);
            byte txlsb = Read8((byte)Registers.REGISTER_TEMPDATA_XLSB); // bits 7:4

            //Combine the values into a 32-bit integer
            int t = (tmsb << 12) + (tlsb << 4) + (txlsb >> 4);

            //Convert the raw value to the temperature in degC
            double temp = BMP280_compensate_T_double(t);

            //Return the temperature as a float value
            return temp;
        }

        /// <summary>
        ///  Reads the pressure from the sensor
        /// </summary>
        /// <returns>
        ///  Atmospheric pressure in hPa
        /// </returns>
        public double ReadPressure()
        {
            //Make sure the I2C device is initialized
            if (!initialised) Begin();

            //Read the temperature first to load the t_fine value for compensation
            if (TFine == int.MinValue)
            {
                ReadTemperature();
            }

            //Read the MSB, LSB and bits 7:4 (XLSB) of the pressure from the BMP280 registers
            byte tmsb = Read8((byte)Registers.REGISTER_PRESSUREDATA_MSB);
            byte tlsb = Read8((byte)Registers.REGISTER_PRESSUREDATA_LSB);
            byte txlsb = Read8((byte)Registers.REGISTER_PRESSUREDATA_XLSB); // bits 7:4

            //Combine the values into a 32-bit integer
            int t = (tmsb << 12) + (tlsb << 4) + (txlsb >> 4);

            //Convert the raw value to the pressure in Pa
            long pres = BMP280_compensate_P_Int64(t);

            //Return the temperature as a float value
            return (pres) / 256;
        }

        /// <summary>
        ///  Calculates the altitude in meters from the specified sea-level pressure(in hPa).
        /// </summary>
        /// <param name="seaLevelPressure" > 
        ///  Sea-level pressure in hPa
        /// </param>
        /// <returns>
        ///  Height in meters from the sensor
        /// </returns>
        public double ReadAltitude(double seaLevelPressure)
        {
            //Make sure the I2C device is initialized
            if (!initialised) Begin();

            //Read the pressure first
            double pressure = ReadPressure();
            //Convert the pressure to Hectopascals(hPa)
            pressure /= 100;

            //Calculate and return the altitude using the international barometric formula
            return 44330.0 * (1.0 - Math.Pow((pressure / seaLevelPressure), 0.1903));
        }

        /// <summary>
        ///  Returns the temperature in DegC. Resolution is 0.01 DegC. Output value of “5123” equals 51.23 DegC.
        /// </summary>
        /// <param name="adc_T"></param>
        /// <returns>
        ///  Degrees celsius
        /// </returns>
        private double BMP280_compensate_T_double(int adc_T)
        {
            double var1, var2, T;

            //The temperature is calculated using the compensation formula in the BMP280 datasheet
            var1 = ((adc_T / 16384.0) - (CalibrationData.DigT1 / 1024.0)) * CalibrationData.DigT2;
            var2 = ((adc_T / 131072.0) - (CalibrationData.DigT1 / 8192.0)) * CalibrationData.DigT3;

            TFine = (int)(var1 + var2);

            T = (var1 + var2) / 5120.0;
            return T;
        }

        /// <summary>
        ///  Returns the pressure in Pa, in Q24.8 format (24 integer bits and 8 fractional bits).
        ///  Output value of “24674867” represents 24674867/256 = 96386.2 Pa = 963.862 hPa
        /// </summary>
        /// <param name="adc_P"></param>
        /// <returns>
        ///  Pressure in hPa
        /// </returns>
        private long BMP280_compensate_P_Int64(int adc_P)
        {
            long var1, var2, p;

            //The pressure is calculated using the compensation formula in the BMP280 datasheet
            var1 = TFine - 128000;
            var2 = var1 * var1 * (long)CalibrationData.DigP6;
            var2 = var2 + ((var1 * (long)CalibrationData.DigP5) << 17);
            var2 = var2 + ((long)CalibrationData.DigP4 << 35);
            var1 = ((var1 * var1 * (long)CalibrationData.DigP3) >> 8) + ((var1 * (long)CalibrationData.DigP2) << 12);
            var1 = (((((long)1 << 47) + var1)) * (long)CalibrationData.DigP1) >> 33;
            if (var1 == 0)
            {
                return 0; //Avoid exception caused by division by zero
            }
            //Perform calibration operations as per datasheet: http://www.adafruit.com/datasheets/BST-BMP280-DS001-11.pdf
            p = 1048576 - adc_P;
            p = (((p << 31) - var2) * 3125) / var1;
            var1 = ((long)CalibrationData.DigP9 * (p >> 13) * (p >> 13)) >> 25;
            var2 = ((long)CalibrationData.DigP8 * p) >> 19;
            p = ((p + var1 + var2) >> 8) + ((long)CalibrationData.DigP7 << 4);
            return p;
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
        public byte Read8(byte register)
        {
            if (protocol == CommunicationProtocol.I2c)
            {
                byte value = 0;

                _i2cDevice.WriteByte(register);
                value = _i2cDevice.ReadByte();
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
        public ushort Read16(byte register)
        {
            if (protocol == CommunicationProtocol.I2c)
            {
                ushort value;

                byte[] readBuffer = new byte[] { 0x00, 0x00 };

                _i2cDevice.WriteByte(register);
                _i2cDevice.Read(readBuffer);
                int h = readBuffer[1] << 8;
                int l = readBuffer[0];
                value = (ushort)(h + l);

                return value;
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
        public uint Read24(byte register)
        {
            if (protocol == CommunicationProtocol.I2c)
            {
                uint value;

                byte[] readBuffer = new byte[] { 0x00, 0x00, 0x00 };

                _i2cDevice.WriteByte(register);
                _i2cDevice.Read(readBuffer);
                value = readBuffer[2];
                value <<= 8;
                value = readBuffer[1];
                value <<= 8;
                value = readBuffer[0];

                return value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            if (_i2cDevice != null)
            {
                _i2cDevice.Dispose();
                _i2cDevice = null;
            }
        }

        /// <summary>
        ///  Registers
        /// </summary>
        private enum Registers : byte
        {
            REGISTER_DIG_T1 = 0x88,
            REGISTER_DIG_T2 = 0x8A,
            REGISTER_DIG_T3 = 0x8C,

            REGISTER_DIG_P1 = 0x8E,
            REGISTER_DIG_P2 = 0x90,
            REGISTER_DIG_P3 = 0x92,
            REGISTER_DIG_P4 = 0x94,
            REGISTER_DIG_P5 = 0x96,
            REGISTER_DIG_P6 = 0x98,
            REGISTER_DIG_P7 = 0x9A,
            REGISTER_DIG_P8 = 0x9C,
            REGISTER_DIG_P9 = 0x9E,

            REGISTER_CHIPID = 0xD0,
            REGISTER_VERSION = 0xD1,
            REGISTER_SOFTRESET = 0xE0,

            REGISTER_CAL26 = 0xE1,  // R calibration stored in 0xE1-0xF0
            
            REGISTER_CONTROL = 0xF4,
            REGISTER_CONFIG = 0xF5,

            REGISTER_PRESSUREDATA_MSB = 0xF7,
            REGISTER_PRESSUREDATA_LSB = 0xF8,
            REGISTER_PRESSUREDATA_XLSB = 0xF9, // bits <7:4>

            REGISTER_TEMPDATA_MSB = 0xFA,
            REGISTER_TEMPDATA_LSB = 0xFB,
            REGISTER_TEMPDATA_XLSB = 0xFC, // bits <7:4>=
        };
    }
}
