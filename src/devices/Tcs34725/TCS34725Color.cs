// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    public class Tcs34725Color
    {
        public ushort Red { get; set; }
        public ushort Green { get; set; }
        public ushort Blue { get; set; }
        public ushort Clear { get; set; }

        public Tcs34725Color(ushort red, ushort green, ushort blue, ushort clear)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Clear = clear;
        }
    }
}
