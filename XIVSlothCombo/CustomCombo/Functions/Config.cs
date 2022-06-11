using System;
using XIVSlothCombo.Core;

namespace XIVSlothCombo.CustomComboNS
{
    internal abstract partial class CustomCombo
    {
        public static int GetOptionValue(string SliderID) => PluginConfiguration.GetCustomIntValue(SliderID);

        public static bool GetOptionBool(string SliderID) => Convert.ToBoolean(GetOptionValue(SliderID));
    }
}
