using CommandLine;
using DeviceApiTester.Infrastructure;
using System;
using System.Device.I2c;

namespace DeviceApiTester.Commands.I2c
{
    [Verb("i2c-bme280-read-temp", HelpText = "Reads temperature from Bme280 sensor connected on I2C bus")]
    public class ReadTempFromBme280 : I2cCommand, ICommandVerb
    {
        /// <summary>Executes the command.</summary>
        /// <returns>The command's exit code.</returns>
        /// <remarks>
        ///     NOTE: This test app uses the base class's <see cref="CreateI2cDevice"/> method to create a device.<br/>
        ///     Real-world usage would simply create an instance of an <see cref="I2cDevice"/> implementation:
        ///     <code>using (var i2c = new Windows10I2cDevice(connectionSettings))</code>
        /// </remarks>
        public int Execute()
        {
            const int bme280_device_address = 0x77;
            var connectionSettings = new I2cConnectionSettings(BusId, bme280_device_address);
            using (var i2c = CreateI2cDevice(connectionSettings))
            {
                const byte temperatureCommand = 0xE3;
                var buffer = new byte[2];
                // Send temperature command, read back two bytes
                i2c.WriteByte(temperatureCommand);
                i2c.Read(buffer.AsSpan());
                // Calculate temperature
                var temp_code = buffer[0] << 8 | buffer[1];
                var temp_celcius = (((175.72 * temp_code) / (float)65536) - 46.85);
                var temp_fahrenheit = (temp_celcius * (9 / 5)) + 32;
                Console.WriteLine($"Temperature in fahrenheit: {temp_fahrenheit}");
                Console.WriteLine($"Temperature in celcius: {temp_celcius}");
            }
            return 0;
        }
    }
}
