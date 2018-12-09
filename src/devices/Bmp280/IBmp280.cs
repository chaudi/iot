using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device
{
    public interface IBmp280
    {
        double ReadTemperature();

        double ReadPreasure();

        double ReadAltitude(double seaLevel);

        byte Read8(byte register);

        ushort Read16(byte register);

        uint Read24(byte register);
    }
}
