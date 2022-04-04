using Dalamud.Interface.Colors;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVSlothComboPlugin;

namespace XIVSlothComboPlugin.ConfigFunctions
{
    public static class ConfigWindowFunctions
    {
        public static void DrawSliderInt(int minValue, int maxValue, string config, string sliderDescription, float itemWidth = 150, uint sliderIncrement = SliderIncrements.Ones)
        {
            var output = Service.Configuration.GetCustomIntValue(config, minValue);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            inputChanged |= ImGui.SliderInt(sliderDescription, ref output, minValue, maxValue);
            

            if (inputChanged)
            {
                if (output % sliderIncrement != 0)
                {
                    output = output.RoundOff(sliderIncrement);
                    if (output < minValue) output = minValue;
                    if (output > maxValue) output = maxValue;

                }
                Service.Configuration.SetCustomIntValue(config, output);
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        public static void DrawSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = Service.Configuration.GetCustomConfigValue(config, minValue);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            inputChanged |= ImGui.SliderFloat(sliderDescription, ref output, minValue, maxValue);

            if (inputChanged)
            {
                Service.Configuration.SetCustomConfigValue(config, output);
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        public static void DrawCheckboxSingleChoice(string config, string checkBoxName, string checkboxDescription, int outputValue, float itemWidth = 150, Vector4 descriptionColor = new Vector4())
        {
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudYellow;
            var output = Service.Configuration.GetCustomIntValue(config);
            ImGui.PushItemWidth(itemWidth);
            var enabled = output == outputValue ? true : false;

            if (ImGui.Checkbox(checkBoxName, ref enabled))
            {
                Service.Configuration.SetCustomIntValue(config, output);
                Service.Configuration.Save();

            }
            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            ImGui.TextWrapped(checkboxDescription);
            ImGui.PopStyleColor();

            ImGui.Spacing();
        }

        public static void DrawJobGrid(string config)
        {
            var values = Service.Configuration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, null, false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 20);

            if (ImGui.Checkbox("Paladin", ref values[0]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Warrior", ref values[1]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Dark Knight", ref values[2]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Gunbreaker", ref values[3]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            if (ImGui.Checkbox("White Mage", ref values[4]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Scholar", ref values[5]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Astrologian", ref values[6]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Sage", ref values[7]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            if (ImGui.Checkbox("Monk", ref values[8]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Dragoon", ref values[9]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Ninja", ref values[10]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Samurai", ref values[11]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Reaper", ref values[12]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }


            ImGui.NextColumn();

            if (ImGui.Checkbox("Bard", ref values[13]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Machinist", ref values[14]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox("Dancer", ref values[15]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.NextColumn();

            if (ImGui.Checkbox("Black Mage", ref values[16]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();

            if (ImGui.Checkbox("Summoner", ref values[17]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();

            if (ImGui.Checkbox("Red Mage", ref values[18]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();
            if (ImGui.Checkbox("Blue Mage", ref values[19]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.Spacing();
        }

        public static int RoundOff(this int i, uint sliderIncrement)
        {
            double sliderAsDouble = Convert.ToDouble(sliderIncrement);
            return ((int)Math.Round(i / sliderAsDouble)) * (int)sliderIncrement;
        }

        //public static float RoundOff(this float i, uint sliderIncrement)
        //{
        //    double sliderAsFloat = Convert.ToDouble(sliderIncrement);
        //    double iAsFloat = Convert.ToDouble(i);

        //    return Convert.ToSingle(Math.Round(iAsFloat / sliderAsFloat) * sliderIncrement);
        //}
    }

    public static class SliderIncrements
    {
        public const uint
            Ones = 1,
            Tens = 10,
            Hundreds = 100,
            Thousands = 1000;
    }
}
