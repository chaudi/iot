// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    /// <summary>
    /// Wait time is set 2.4 ms increments unless the WLONG bit is 
    /// asserted, in which case the wait times are 12× longer. WTIME is 
    /// programmed as a 2’s complement number.
    /// Used to set valid values in <see cref="Register.WaitTime"/>
    /// </summary>
    public enum WaitTime
    {
        /// <summary>
        /// Wait time 1
        /// 2.4 milliseconds
        /// If <see cref="Tcs34725.SetWaitLong"/> is set then the wait time is 0.029 seconds
        /// </summary>
        Ms24 = 0xFF,
        /// <summary>
        /// Wait time 85
        /// 204 milliseconds
        /// If <see cref="Tcs34725.SetWaitLong"/> is set then the wait time is 2.45 seconds
        /// </summary>
        Ms204 = 0xAB,
        /// <summary>
        /// Wait time 256
        /// 614 milliseconds
        /// If <see cref="Tcs34725.SetWaitLong"/> is set then the wait time is 7.4 seconds
        /// </summary>
        Ms614 = 0x00,
    }
}
