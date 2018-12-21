// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Iot.Device.Tcs34725
{
    public class Tcs34725Color
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Clear { get; set; }

        public Tcs34725Color(byte red, byte green, byte blue, byte clear)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Clear = clear;
        }
    }
}
