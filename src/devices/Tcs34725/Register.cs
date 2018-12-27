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
        /// <summary>
        /// The ENABLE register is used primarily to power the TCS3472 
        /// device ON and OFF, and enable functions and interrupts as shown below. 
        /// </summary>
        Enable = 0x00,
        /// <summary>
        /// The RGBC timing register controls the internal integration time of the RGBC clear and IR channel ADCs in 2.4-ms increments.
        /// Max RGBC Count = (256 − <see cref="RgbcTiming"/>) × 1024 up to a maximum of 65535
        /// <see cref="IntegrationTime"/> for valid values
        /// ATIME
        /// </summary>
        RgbcTiming = 0x01,
        /// <summary>
        /// Wait time is set 2.4 ms increments unless the <see cref="Configuration"/> bit is asserted, in which case the wait times are 12x longer. <see cref="WaitTime"/> is programmed as a 2’s complement number.
        /// <see cref="WaitTime"/> for valid values
        /// WTIME
        /// </summary>
        WaitTime = 0x03,
        /// <summary>
        /// Clear channel lower interrupt threshold
        /// AILTL
        /// </summary>
        AILTL = 0x04,
        /// <summary>
        /// RGBC clear channel low threshold upper byte
        /// AILTH
        /// </summary>
        AILTH = 0x05,
        /// <summary>
        /// Clear channel upper interrupt threshold
        /// AIHTL
        /// </summary>
        AIHTL = 0x06,
        /// <summary>
        /// RGBC clear channel high threshold upper byte
        /// AIHTH
        /// </summary>
        AIHTH = 0x07,
        /// <summary>
        /// Controls the filtering interrupt capabilities of the device. Configurable filtering is provided toallow interrupts to be generated after each integration cycle or 
        /// if the integration has produced a result that is outside of the values specified by the threshold register for some specified
        /// amount of time.
        /// <see cref="Interrupt"/> for valid values
        /// </summary>
        Persistence = 0x0C,
        /// <summary>
        /// The configuration register sets the wait long time.
        /// Wait Long. When asserted, the wait cycles are increased by a factor 12x from that programmed in the WTIME register.
        /// </summary>
        Configuration = 0x0D,
        // <summary>
        //  Choose between short and long (12x) wait times via TCS34725_WTIME 
        // </summary>
        //CONFIG_WLONG = 0x02,
        /// <summary>
        /// Set the gain level for the sensor, <see cref="Gain"/> for valid values.
        /// </summary>
        ControlAnalogGain = 0x0F,
        /// <summary>
        /// The Id Register provides the value for the part number. The Id register is a read-only register.
        /// 0x44 = Tcs34721/Tcs34725, 0x4D = Tcs34723/Tcs34727
        /// </summary>
        Id = 0x12,
        /// <summary>
        /// The Status Register provides the internal status of the device. This register is read only.
        /// </summary>
        Status = 0x13,
        /// <summary>
        /// Clear channel data low byte
        /// </summary>
        ClearDataLow = 0x14,
        /// <summary>
        /// Clear channel data high byte
        /// </summary>
        ClearDataHigh = 0x15,
        /// <summary>
        ///  Red channel data low byte
        /// </summary>
        RedDataLow = 0x16,
        /// <summary>
        ///  Red channel data high byte
        /// </summary>
        RedDataHigh = 0x17,
        /// <summary>
        /// Green channel data low byte
        /// </summary>
        GreenDataLow = 0x18,
        /// <summary>
        /// Green channel data high byte
        /// </summary>
        GreenDataHigh = 0x19,
        /// <summary>
        /// Blue channel data low byte
        /// </summary>
        BlueDataLow = 0x1A,
        /// <summary>
        /// Blue channel data high byte
        /// </summary>
        BlueDataHigh = 0x1B
    }
}
