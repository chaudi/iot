using Iot.Device;
using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Threading;

namespace Iot.Devices.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Bmp280!");

            var i2cSettings = new I2cConnectionSettings(1, 0x77);
            var unixDevice = new UnixI2cDevice(i2cSettings);
            var i2CBmp280 = new Bmp280(unixDevice);

            using (i2CBmp280)
            {
                while (true)
                {
                    double tempValue = i2CBmp280.ReadTemperature();
                    Console.WriteLine($"Temperature {tempValue}");
                    double preValue = i2CBmp280.ReadPreasure();
                    Console.WriteLine($"Preasure {preValue}");
                    double altValue = i2CBmp280.ReadAltitude(1013.25);
                    Console.WriteLine($"Altitude: {altValue}");
                    Thread.Sleep(1000);
                }
            }

        }
    }
}
