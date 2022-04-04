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

        public static void DrawCheckboxSingle(string config, string checkBoxName, string checkboxDescription, float itemWidth = 150, Vector4 descriptionColor = new Vector4())
        {
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.TankBlue;
            var output = Service.Configuration.GetCustomBoolValue(config);
            ImGui.PushItemWidth(itemWidth);

            if (ImGui.Checkbox(checkBoxName, ref output))
            {
                Service.Configuration.SetCustomBoolValue(config, output);
                Service.Configuration.Save();

            }
            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            ImGui.TextWrapped(checkboxDescription);
            ImGui.PopStyleColor();

            ImGui.Spacing();
        }

        public static void DrawJobGrid(string config)
        {
            
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
