using System;
using System.ComponentModel.DataAnnotations;
using System.Device.I2c;
using System.Device.I2c.Drivers;

namespace Tcs34725.sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var i2cSettings = new I2cConnectionSettings(1, 0x29);

            var device = new UnixI2cDevice(i2cSettings);
            Console.WriteLine("Hello World!");
        }
    }
}
