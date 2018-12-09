// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
