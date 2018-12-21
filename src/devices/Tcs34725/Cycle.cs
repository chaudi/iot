// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    public enum Cycle : byte
    {
        /// <summary>
        /// Every RGBC cycle generates an interrupt 
        /// </summary>
        PERS_NONE = 0,         //(0b0000),  /*                                */
        /// <summary>
        /// 1 clean channel value outside threshold range generates an interrupt
        /// </summary>
        PERS_1_CYCLE = 1,      //(0b0001),  /*    */
        /// <summary>
        /// 2 clean channel values outside threshold range generates an interrupt 
        /// </summary>
        PERS_2_CYCLE = 2,      //(0b0010),  /*  */
        /// <summary>
        /// 3 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_3_CYCLE = 3,      //(0b0011),  /*   */
        /// <summary>
        /// 5 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_5_CYCLE = 4,      //(0b0100),  /*   */
        /// <summary>
        /// 10 clean channel values outside threshold range generates an interrupt 
        /// </summary>
        PERS_10_CYCLE = 5,     //(0b0101),  /* */
        /// <summary>
        /// 15 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_15_CYCLE = 6,     //(0b0110),  /*  */
        /// <summary>
        /// 20 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_20_CYCLE = 7,     //(0b0111),  /*  */
        /// <summary>
        /// 25 clean channel values outside threshold range generates an interrupt 
        /// </summary>
        PERS_25_CYCLE = 8,     //(0b1000),  /* */
        /// <summary>
        /// 30 clean channel values outside threshold range generates an interrupt 
        /// </summary>
        PERS_30_CYCLE = 9,     //(0b1001),  /* */
        /// <summary>
        /// 35 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_35_CYCLE = 10,    //(0b1010),  /*  */
        /// <summary>
        /// 40 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_40_CYCLE = 11,    //(0b1011),  /* */
        /// <summary>
        /// 45 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_45_CYCLE = 12,    //(0b1100),  /* */
        /// <summary>
        /// 50 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_50_CYCLE = 13,    //(0b1101),  /*  */
        /// <summary>
        /// 55 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_55_CYCLE = 14,    //(0b1110),  /* */
        /// <summary>
        /// 60 clean channel values outside threshold range generates an interrupt
        /// </summary>
        PERS_60_CYCLE = 15     //(0b1111),  /*  */
    }
}
