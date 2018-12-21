// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Iot.Device.Tcs34725
{
    public enum Gain : byte
    {
        /// <summary>
        /// No Gain
        /// </summary>
        GAIN_1X = 0x00,
        /// <summary>
        /// 4x Gain
        /// </summary>
        GAIN_4X = 0x01,
        /// <summary>
        /// 16x Gain
        /// </summary>
        GAIN_16X = 0x02,
        /// <summary>
        /// 60x Gain
        /// </summary>
        GAIN_60X = 0x03
    }
}
