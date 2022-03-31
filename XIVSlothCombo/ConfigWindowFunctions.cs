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
        public static void DrawSliderInt(int minValue, int maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = Service.Configuration.GetCustomIntValue(config);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            inputChanged |= ImGui.SliderInt(sliderDescription, ref output, minValue, maxValue);

            if (inputChanged)
            {
                Service.Configuration.SetCustomIntValue(config, output);
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        public static void DrawSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = Service.Configuration.GetCustomConfigValue(config);
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
    }
}
