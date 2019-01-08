﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Bmp280
{
    internal class CalibrationData
    {
        public ushort DigT1 { get; set; }
        public short DigT2 { get; set; }
        public short DigT3 { get; set; }

        public ushort DigP1 { get; set; }
        public short DigP2 { get; set; }
        public short DigP3 { get; set; }
        public short DigP4 { get; set; }
        public short DigP5 { get; set; }
        public short DigP6 { get; set; }
        public short DigP7 { get; set; }
        public short DigP8 { get; set; }
        public short DigP9 { get; set; }

        internal void ReadFromDevice(Bmp280 bmp280)
        {
            // Read temperature calibration data
            DigT1 = bmp280.Read16BitsFromRegister((byte)Register.DigT1);
            DigT2 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigT2);
            DigT3 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigT3);

            // Read pressure calibration data
            DigP1 = bmp280.Read16BitsFromRegister((byte)Register.DigP1);
            DigP2 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP2);
            DigP3 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP3);
            DigP4 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP4);
            DigP5 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP5);
            DigP6 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP6);
            DigP7 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP7);
            DigP8 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP8);
            DigP9 = (short)bmp280.Read16BitsFromRegister((byte)Register.DigP9);
        }
    }
}