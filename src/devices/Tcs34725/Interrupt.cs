// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    /// <summary>
    /// Interrupt persistence. Controls rate of interrupt to the host processor
    /// Apers field
    /// </summary>
    public enum Interrupt : byte
    {
        /// <summary>
        /// Every RGBC cycle generates an interrupt 
        /// </summary>
        Every = 0b0000, 
        /// <summary>
        /// 1 clear channel value outside of threshold range
        /// </summary>
        OneCycle = 0b0001, 
        /// <summary>
        /// 2 clear channel value outside of threshold range
        /// </summary>
        TwoCycles = 0b0010,
        /// <summary>
        /// 3 clear channel value outside of threshold range
        /// </summary>
        ThreeCycles = 0b0011,
        /// <summary>
        /// 5 clear channel consecutive values out of range
        /// </summary>
        FiveCycles = 0b0100,
        /// <summary>
        /// 10 clear channel consecutive values out of range
        /// </summary>
        TenCycles = 0b0101,
        /// <summary>
        /// 15 clear channel consecutive values out of range
        /// </summary>
        FifteenCycles = 0b0110,
        /// <summary>
        /// 20 clear channel consecutive values out of range
        /// </summary>
        TwentyCycles = 0b0111,
        /// <summary>
        /// 25 clear channel consecutive values out of range 
        /// </summary>
        TwentyfiveCycles = 0b1000,
        /// <summary>
        /// 30 clear channel consecutive values out of range
        /// </summary>
        ThirtyCycles = 0b1001,
        /// <summary>
        /// 35 clear channel consecutive values out of range
        /// </summary>
        ThirtyfiveCycles = 0b1010,
        /// <summary>
        /// 40 clear channel consecutive values out of range
        /// </summary>
        FourtyCycles = 0b1011,
        /// <summary>
        /// 45 clear channel consecutive values out of range
        /// </summary>
        FourtyfiveCycles = 0b1100,
        /// <summary>
        /// 50 clear channel consecutive values out of range
        /// </summary>
        FiftyCycles = 0b1101,
        /// <summary>
        /// 55 clear channel consecutive values out of range
        /// </summary>
        FiftyfiveCycles = 0b1110,
        /// <summary>
        /// 60 clear channel consecutive values out of range
        /// </summary>
        SixtyCycles = 0b1111, 
    }
}
