// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    internal enum EnableRegisterBit : byte
    {
        /// <summary>
        /// RGBC interrupt enable. When asserted, permits RGBC interrupts to be generated.
        /// </summary>
        AIEN = 0b0001_0000,
        /// <summary>
        /// Wait enable. This bit activates the wait feature. Writing a 1 activates the wait timer. Writing a 0 disables the wait timer
        /// </summary>
        WEN = 0b0000_1000,
        /// <summary>
        /// RGBC enable. This bit actives the two-channel ADC. Writing a 1 activates the RGBC. Writing a 0 disables the RGBC
        /// </summary>
        AEN = 0b0000_0010,
        /// <summary>
        /// Power ON. This bit activates the internal oscillator to permit the timers and ADC channels to operate.
        /// Writing a 1  activates the oscillator. Writing a 0 disables the oscillator.
        /// PON
        /// </summary>
        PowerOn = 0b0000_0001,
    }
}
