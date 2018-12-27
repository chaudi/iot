using Xunit;
using System.Diagnostics;
namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Sampling result = ByteToSampling(0);
            Assert.Equal(Sampling.Skipped, result);

            result = ByteToSampling(1);
            Assert.Equal(Sampling.UltraLowPower, result);

            result = ByteToSampling(2);
            Assert.Equal(Sampling.LowPower, result);

            result = ByteToSampling(3);
            Assert.Equal(Sampling.Standard, result);

            result = ByteToSampling(4);
            Assert.Equal(Sampling.HighResolution, result);

            result = ByteToSampling(5);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(6);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(7);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(8);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(9);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(10);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(11);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(12);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(13);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(14);
            Assert.Equal(Sampling.UltraHighResolution, result);

            result = ByteToSampling(15);
            Assert.Equal(Sampling.UltraHighResolution, result);

        }

        private Sampling ByteToSampling(byte value)
        {
            //Values >=5 equals UltraHighResolution
            //if (value >= 5)
            //{
            //    return Sampling.UltraHighResolution;
            //}

            return (Sampling) value;            
        }

        /// <summary>
        /// Oversampling settings. Maximum of x2 is recommended for temperature
        /// </summary>
        public enum Sampling : byte
        {
            /// <summary>
            /// Skipped (output set to 0x80000)
            /// </summary>
            Skipped = 0b000,
            /// <summary>
            /// oversampling x1
            /// </summary>
            UltraLowPower = 0b001,
            /// <summary>
            /// oversampling x2
            /// </summary>
            LowPower = 0b010,
            /// <summary>
            /// oversampling x4
            /// </summary>
            Standard = 0b011,
            /// <summary>
            /// oversampling x8
            /// </summary>
            HighResolution = 0b100,
            /// <summary>
            /// oversampling x16
            /// </summary>
            UltraHighResolution = 0b101,
        }
    }
}
