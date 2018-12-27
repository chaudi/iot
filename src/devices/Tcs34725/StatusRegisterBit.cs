// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    public enum StatusRegisterBit : byte
    {
        /// <summary>
        /// RGBC Clear channel interrupt
        /// AINT
        /// </summary>
        Aint = 0x10,
        /// <summary>
        /// Indicates that the RGBC channels have completed an integration cycle
        /// AVAILID
        /// </summary>
        AvailId = 0x01,
    }
}
