using System;
using XIVSlothCombo.Core;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        public static int GetOptionValue(string SliderID) => PluginConfiguration.GetCustomIntValue(SliderID);

        public static bool GetIntOptionAsBool(string SliderID) => Convert.ToBoolean(GetOptionValue(SliderID));

        public static bool GetOptionBool(string SliderID) => PluginConfiguration.GetCustomBoolValue(SliderID);
    }
}
