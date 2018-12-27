﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    public enum Gain : byte
    {
        /// <summary>
        /// 1x gain
        /// </summary>
        X1 = 0b0000,
        /// <summary>
        /// 4x gain
        /// </summary>
        X4 = 0b0001,
        /// <summary>
        /// 16x gain
        /// </summary>
        X16 = 0b0010,
        /// <summary>
        /// 60x gain
        /// </summary>
        X60 = 0b0011
    }
}
