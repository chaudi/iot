// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Device.Spi;
using System.Device.Spi.Drivers;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Bmp280;

namespace Iot.Device.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Bmp280!");
            await SpiLoop();
        }

        static async Task SpiLoop()
        {
            var spiSettings = new SpiConnectionSettings(0, 1);
            var spiDevice = new UnixSpiDevice(spiSettings);

            var spiBmp280 = new Bmp280.Bmp280(spiDevice);

            await Loop(spiBmp280);
        }

        static async Task I2cLoop()
        {
            //0x77 is the address for BMP280
            const int bmp280Address = 0x77;
            //bus id on the raspberry pi 3
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, bmp280Address);
            var i2cDevice = new UnixI2cDevice(i2cSettings);
            var i2CBmp280 = new Bmp280.Bmp280(i2cDevice);

            await Loop(i2CBmp280);
        }

        static async Task Loop(Bmp280.Bmp280 bmp280Device)
        {
            //set this to the current sea level pressure in the area for correct altitude readings
            const double defaultSeaLevelPressure = 1033.00;

            using (bmp280Device)
            {
                while (true)
                {
                    ////set mode forced so device sleeps after read
                    //bmp280Device.SetPowerMode(PowerMode.Forced);

                    ////set samplings
                    //bmp280Device.SetTemperatureSampling(Sampling.UltraLowPower);
                    //bmp280Device.SetPressureSampling(Sampling.UltraLowPower);

                    var result = bmp280Device.ReadPowerMode();
                    
                    Console.WriteLine($"Powermode {Enum.GetName(typeof(PowerMode), result)}");

                    //read values
                    double tempValue = await bmp280Device.ReadTemperatureAsync();
                    Console.WriteLine($"Temperature {tempValue}");
                    double preValue = await bmp280Device.ReadPressureAsync();
                    Console.WriteLine($"Pressure {preValue}");
                    double altValue = await bmp280Device.ReadAltitudeAsync(defaultSeaLevelPressure);
                    Console.WriteLine($"Altitude: {altValue}");
                    Thread.Sleep(1000);

                    ////set higher sampling
                    //bmp280Device.SetTemperatureSampling(Sampling.LowPower);
                    //Console.WriteLine(bmp280Device.ReadTemperatureSampling());
                    //bmp280Device.SetPressureSampling(Sampling.UltraHighResolution);
                    //Console.WriteLine(bmp280Device.ReadPressureSampling());

                    ////set mode forced and read again
                    //bmp280Device.SetPowerMode(PowerMode.Forced);

                    //read values
                    tempValue = await bmp280Device.ReadTemperatureAsync();
                    Console.WriteLine($"Temperature {tempValue}");
                    preValue = await bmp280Device.ReadPressureAsync();
                    Console.WriteLine($"Pressure {preValue}");
                    altValue = await bmp280Device.ReadAltitudeAsync(defaultSeaLevelPressure);
                    Console.WriteLine($"Altitude: {altValue}");
                    //Thread.Sleep(5000);

                    ////set sampling to higher
                    //bmp280Device.SetTemperatureSampling(Sampling.UltraHighResolution);
                    //Console.WriteLine(bmp280Device.ReadTemperatureSampling());
                    //bmp280Device.SetPressureSampling(Sampling.UltraLowPower);
                    //Console.WriteLine(bmp280Device.ReadPressureSampling());
                }
            }
        }
    }
}
