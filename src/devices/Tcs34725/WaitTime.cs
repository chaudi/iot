// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    /// <summary>
    /// Wait time is set 2.4 ms increments unless the WLONG bit is 
    /// asserted, in which case the wait times are 12× longer.WTIME is 
    /// programmed as a 2’s complement number.
    /// Used to set valid values in <see cref="Register.WAIT_TIME"/>
    /// </summary>
    public enum WaitTime
    {
        /// <summary>
        /// WLONG0 = 2.4ms   WLONG1 = 0.029s. 1
        /// </summary>
        WTIME_2_4MS = 0xFF,
        /// <summary>
        ///  WLONG0 = 204ms   WLONG1 = 2.45s. 85
        /// </summary>
        WTIME_204MS = 0xAB,
        /// <summary>
        /// WLONG0 = 614ms   WLONG1 = 7.4s. 256
        /// </summary>
        WTIME_614MS = 0x00,
    }
}
