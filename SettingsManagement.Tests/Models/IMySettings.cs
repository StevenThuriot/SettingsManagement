﻿using System;
using System.ComponentModel;

namespace SettingsManagement.Tests.Models
{
    public interface IMySettings //: ISettingsManager
    {
        [DefaultValue(5L), Description("This is a description")]
        long MyFirstProperty { get; set; }

        bool MySecondProperty { get; set; }

        [DefaultValue("Test"), Description("This is another description")]
        string MyThirdProperty { get; set; }

        [DefaultValue("00:20"), SettingsConverter(typeof(TimeSpanConverter))]
        TimeSpan MyFourthProperty { get; set; }

        [DefaultValue("Ja"), SettingsConverter(typeof(JaNeeConverter))]
        bool MyFifthProperty { get; set; }
    }
}
