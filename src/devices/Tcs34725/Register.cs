// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Iot.Device.Tcs34725
{
    /// <summary>
    /// Register addresses of Tcs34725
    /// </summary>
    public enum Register : byte
    {
        ENABLE = 0x00,
        /// <summary>
        /// RGBC Interrupt Enable
        /// </summary>
        ENABLE_AIEN = 0x10,
        /// <summary>
        /// Wait enable - Writing 1 activates the wait timer
        /// </summary>
        ENABLE_WEN = 0x08,
        /// <summary>
        /// RGBC Enable - Writing 1 actives the ADC, 0 disables it
        /// </summary>
        ENABLE_AEN = 0x02,
        /// <summary>
        /// Power on - Writing 1 activates the internal oscillator, 0 disables it
        /// Enable register PON bit in datasheet
        /// </summary>
        ENABLE_POWER_ON = 0x01,
        /// <summary>
        /// Integration time
        /// ATIME in datasheet
        /// </summary>
        ATIME = 0x01,
        /// <summary>
        /// Wait time (if TCS34725_ENABLE_WEN is asserted).
        /// WTIME in datasheet
        /// </summary>
        WAIT_TIME = 0x03,
        /// <summary>
        /// Clear channel lower interrupt threshold
        /// </summary>
        AILTL = 0x04,
        /// <summary>
        /// RGBC clear channel low threshold upper byte
        /// </summary>
        AILTH = 0x05,
        /// <summary>
        /// Clear channel upper interrupt threshold
        /// </summary>
        AIHTL = 0x06,
        /// <summary>
        /// RGBC clear channel high threshold upper byte
        /// </summary>
        AIHTH = 0x07,
        /// <summary>
        /// Persistence register - basic SW filtering mechanism for interrupts
        /// </summary>
        PERS = 0x0C,
        /// <summary>
        /// The configuration register sets the wait long time.
    /// </summary>
        CONFIG = 0x0D,
        /// <summary>
        ///  Choose between short and long (12x) wait times via TCS34725_WTIME 
        /// </summary>
        CONFIG_WLONG = 0x02,
        /// <summary>
        /// Set the gain level for the sensor
        /// </summary>
        CONTROL_ANALOG_GAIN = 0x0F,
        /// <summary>
        /// 0x44 = Tcs34721/Tcs34725, 0x4D = Tcs34723/Tcs34727
        /// </summary>
        ID = 0x12,
        STATUS = 0x13,
        /// <summary>
        ///  RGBC Clear channel interrupt 
        /// </summary>
        STATUS_AINT = 0x10,
        /// <summary>
        /// Indicates that the RGBC channels have completed an integration cycle
        /// </summary>
        STATUS_AVALID = 0x01,
        /// <summary>
        /// Clear channel data
        /// </summary>
        CDATAL = 0x14,
        CDATAH = 0x15,
        /// <summary>
        ///  Red channel data
        /// </summary>
        RDATAL = 0x16,
        RDATAH = 0x17,
        /// <summary>
        /// Green channel data 
        /// </summary>
        GDATAL = 0x18,
        GDATAH = 0x19,
        /// <summary>
        /// Blue channel data 
        /// </summary>
        BDATAL = 0x1A,
        BDATAH = 0x1B,
    }
}
