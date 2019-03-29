using System;
using System.ComponentModel;

namespace SettingsManagement.Tests.Models
{
    public interface IMySettings //: ISettingsManager
    {
        [DefaultValue(5), Description("This is a description")]
        int MyFirstProperty { get; set; }
        bool MySecondProperty { get; set; }

        [DefaultValue("Test"), Description("This is another description")]
        string MyThirdProperty { get; set; }

        [DefaultValue("00:20")]
        TimeSpan MyFourthProperty { get; set; }
    }
}
