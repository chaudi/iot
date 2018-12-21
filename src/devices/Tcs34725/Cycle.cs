// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    public enum Cycle : byte
    {
        TCS34725_PERS_NONE = 0,         //(0b0000),  /* Every RGBC cycle generates an interrupt                                */
        TCS34725_PERS_1_CYCLE = 1,      //(0b0001),  /* 1 clean channel value outside threshold range generates an interrupt   */
        TCS34725_PERS_2_CYCLE = 2,      //(0b0010),  /* 2 clean channel values outside threshold range generates an interrupt  */
        TCS34725_PERS_3_CYCLE = 3,      //(0b0011),  /* 3 clean channel values outside threshold range generates an interrupt  */
        TCS34725_PERS_5_CYCLE = 4,      //(0b0100),  /* 5 clean channel values outside threshold range generates an interrupt  */
        TCS34725_PERS_10_CYCLE = 5,     //(0b0101),  /* 10 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_15_CYCLE = 6,     //(0b0110),  /* 15 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_20_CYCLE = 7,     //(0b0111),  /* 20 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_25_CYCLE = 8,     //(0b1000),  /* 25 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_30_CYCLE = 9,     //(0b1001),  /* 30 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_35_CYCLE = 10,    //(0b1010),  /* 35 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_40_CYCLE = 11,    //(0b1011),  /* 40 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_45_CYCLE = 12,    //(0b1100),  /* 45 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_50_CYCLE = 13,    //(0b1101),  /* 50 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_55_CYCLE = 14,    //(0b1110),  /* 55 clean channel values outside threshold range generates an interrupt */
        TCS34725_PERS_60_CYCLE = 15     //(0b1111),  /* 60 clean channel values outside threshold range generates an interrupt */
    }
}
