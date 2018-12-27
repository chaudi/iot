// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Threading.Tasks;
using Iot.Device.Tcs34725;

namespace Iot.Device.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            byte raspberryPiI2cBusId = 1;
            byte tcs34725Address = 0x29;

            var i2cSettings = new I2cConnectionSettings(raspberryPiI2cBusId, tcs34725Address);

            var device = new UnixI2cDevice(i2cSettings);
            var tcs34725 = new Tcs34725.Tcs34725(device);

            while (true)
            {
                var color = await tcs34725.GetRawDataOneShot();
                var temperature = tcs34725.CalculateColorTemperature(color.Red, color.Green, color.Blue);
                var lux = tcs34725.CalculateLux(color.Red, color.Green, color.Blue);
                var temperatureDn40 = tcs34725.CalculateColorTemperature_dn40(color.Red, color.Green, color.Blue, color.Clear);

                Console.WriteLine($"Raw color RGBC: {color.Red},{color.Green},{color.Blue},{color.Clear}");
                Console.WriteLine($"Lux {lux}");
                Console.WriteLine($"Temperature {temperature} Kelvin");
                Console.WriteLine($"Temperature (DN40 formula) {temperatureDn40} Kelvin");
                await Task.Delay(1000);

                tcs34725.SetGain(Gain.X16);
                tcs34725.SetIntegrationTime(IntegrationTime.Ms101);
                color = await tcs34725.GetRawDataOneShot();
                temperature = tcs34725.CalculateColorTemperature(color.Red, color.Green, color.Blue);
                lux = tcs34725.CalculateLux(color.Red, color.Green, color.Blue);
                temperatureDn40 = tcs34725.CalculateColorTemperature_dn40(color.Red, color.Green, color.Blue, color.Clear);

                Console.WriteLine($"Raw color RGBC: {color.Red},{color.Green},{color.Blue},{color.Clear}");
                Console.WriteLine($"Lux {lux}");
                Console.WriteLine($"Temperature {temperature} Kelvin");
                Console.WriteLine($"Temperature (DN40 formula) {temperatureDn40} Kelvin");
                await Task.Delay(1000);
            }
        }
    }
}
