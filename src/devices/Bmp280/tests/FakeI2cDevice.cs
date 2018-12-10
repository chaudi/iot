using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

namespace Iot.Device.Tests
{
    internal class FakeI2cDevice : I2cDevice
    {
        public override I2cConnectionSettings ConnectionSettings => throw new NotImplementedException();

        private byte LastWrittenByte = 0;

        public override void Read(Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public override void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte data)
        {
            throw new NotImplementedException();
        }
    }
}
