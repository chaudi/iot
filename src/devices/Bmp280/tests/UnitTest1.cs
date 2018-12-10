using System;
using Xunit;

namespace Iot.Device.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var fakeDevice = new FakeI2cDevice();
            var i2CBmp280 = new Bmp280(fakeDevice);

            using (i2CBmp280)
            {
                while (true)
                {
                    double tempValue = i2CBmp280.ReadTemperature();
                    Assert.Equal(24.23424, tempValue);
                }
            }
        }

        [Fact]
        public void Test2()
        {
            var fakeDevice = new FakeI2cDevice();
            var i2CBmp280 = new Bmp280(fakeDevice);

            using (i2CBmp280)
            {
                while (true)
                {
                    double tempValue = i2CBmp280.ReadPressure();
                    Assert.Equal(24.23424, tempValue);
                }
            }
        }

        [Fact]
        public void Test3()
        {
            var fakeDevice = new FakeI2cDevice();
            var i2CBmp280 = new Bmp280(fakeDevice);

            using (i2CBmp280)
            {
                while (true)
                {
                    double tempValue = i2CBmp280.ReadAltitude(1013.25);
                    Assert.Equal(24.23424, tempValue);
                }
            }
        }
    }
}
