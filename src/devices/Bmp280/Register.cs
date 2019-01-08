// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Bmp280
{
    /// <summary>
    ///  Register
    /// </summary>
    internal enum Register : byte
    {
        DigT1 = 0x88,
        DigT2 = 0x8A,
        DigT3 = 0x8C,

        DigP1 = 0x8E,
        DigP2 = 0x90,
        DigP3 = 0x92,
        DigP4 = 0x94,
        DigP5 = 0x96,
        DigP6 = 0x98,
        DigP7 = 0x9A,
        DigP8 = 0x9C,
        DigP9 = 0x9E,

        ChipId = 0xD0,
        Version = 0xD1,
        SoftReset = 0xE0,

        Cal26 = 0xE1,  // R calibration stored in 0xE1-0xF0

        Status = 0xF3,
        Control = 0xF4,
        Config = 0xF5,

        PressureDataMsb = 0xF7,
        PressureDataLsb = 0xF8,
        PressureDataXlsb = 0xF9, // bits <7:4>

        TemperatureDataMsb = 0xFA,
        TemperatureDataLsb = 0xFB,
        TemperatureDataXlsb = 0xFC, // bits <7:4>=
    }
}