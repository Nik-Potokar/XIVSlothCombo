using System;
using System.Linq;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        public static int GetOptionValue(string SliderID) => PluginConfiguration.GetCustomIntValue(SliderID);

        public static bool GetIntOptionAsBool(string SliderID) => Convert.ToBoolean(GetOptionValue(SliderID));

        public static bool GetOptionBool(string SliderID) => PluginConfiguration.GetCustomBoolValue(SliderID);

        public static float GetOptionFloat(string SliderID) => PluginConfiguration.GetCustomFloatValue(SliderID);
    }

    internal class UserData
    {
        protected string pName;
        public UserData(string v) => pName = v;
        public static implicit operator string(UserData o) => (o.pName);
    }

    internal class UserFloat : UserData
    {
        public UserFloat(string v) : base(v) { }
        public static implicit operator float(UserFloat o) => PluginConfiguration.GetCustomFloatValue(o.pName);
    }

    internal class UserInt : UserData
    {
        public UserInt(string v) : base(v) { }
        public static implicit operator int(UserInt o) => PluginConfiguration.GetCustomIntValue(o.pName);
    }

    internal class UserBool : UserData
    {
        public UserBool(string v) : base(v) { }
        public static implicit operator bool(UserBool o) => PluginConfiguration.GetCustomBoolValue(o.pName);
    }

    internal class UserIntArray : UserData
    {
        public UserIntArray(string v) : base(v) { }
        public string Name => pName;
        public int Count => PluginConfiguration.GetCustomIntArrayValue(this.pName).Length;
        public bool Any(Func<int, bool> func) => PluginConfiguration.GetCustomIntArrayValue(this.pName).Any(func);
        public int[] Items => PluginConfiguration.GetCustomIntArrayValue(this.pName);
        public int IndexOf(int item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Items[i] == item)
                    return i;
            }
            return -1;
        }

        public void Clear(int maxValues)
        {
            var array = PluginConfiguration.GetCustomIntArrayValue(this.pName);
            Array.Resize<int>(ref array, maxValues);
            PluginConfiguration.SetCustomIntArrayValue(this.pName, array);
            Service.Configuration.Save();
        }

        public static implicit operator int[](UserIntArray o) => PluginConfiguration.GetCustomIntArrayValue(o.pName);

        public int this[int index]
        {
            get
            {
                if (index >= this.Count)
                {
                    var array = PluginConfiguration.GetCustomIntArrayValue(this.pName);
                    Array.Resize(ref array, index + 1);
                    array[index] = 0;
                    PluginConfiguration.SetCustomIntArrayValue(this.pName, array);
                    Service.Configuration.Save();
                }
                return PluginConfiguration.GetCustomIntArrayValue(this.pName)[index];
            }
            set
            {
                if (index < this.Count)
                {
                    var array = PluginConfiguration.GetCustomIntArrayValue(this.pName);
                    array[index] = value;
                    Service.Configuration.Save();
                }
            }
        }
    }

    internal class UserBoolArray : UserData
    {
        public UserBoolArray(string v) : base(v) { }
        public int Count => PluginConfiguration.GetCustomBoolArrayValue(this.pName).Length;
        public static implicit operator bool[](UserBoolArray o) => PluginConfiguration.GetCustomBoolArrayValue(o.pName);
        public bool this[int index]
        {
            get
            {
                if (index >= this.Count)
                {
                    var array = PluginConfiguration.GetCustomBoolArrayValue(this.pName);
                    Array.Resize(ref array, index + 1);
                    array[index] = false;
                    PluginConfiguration.SetCustomBoolArrayValue(this.pName, array);
                    Service.Configuration.Save();
                }
                return PluginConfiguration.GetCustomBoolArrayValue(this.pName)[index];
            }
        }

        public bool All(Func<bool, bool> predicate)
        {
            var array = PluginConfiguration.GetCustomBoolArrayValue(this.pName);
            return array.All(predicate);

        }
    }

}
