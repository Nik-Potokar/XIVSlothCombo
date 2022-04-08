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
        /// <summary>
        /// Draws a slider that lets the user set a given value for their feature.
        /// </summary>
        /// <param name="minValue">The absolute minimum value you'll let the user pick.</param>
        /// <param name="maxValue">The absolute maximum value you'll let the user pick.</param>
        /// <param name="config">The config ID.</param>
        /// <param name="sliderDescription">Description of the slider. Appends to the right of the slider.</param>
        /// <param name="itemWidth">How long the slider should be.</param>
        /// <param name="sliderIncrement">How much you want the user to increment the slider by. Uses SliderIncrements as a preset.</param>
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

                inputChanged = false;
            }

            
            
            ImGui.Spacing();
        }

        /// <summary>
        /// Draws a slider that lets the user set a given value for their feature.
        /// </summary>
        /// <param name="minValue">The absolute minimum value you'll let the user pick.</param>
        /// <param name="maxValue">The absolute maximum value you'll let the user pick.</param>
        /// <param name="config">The config ID.</param>
        /// <param name="sliderDescription">Description of the slider. Appends to the right of the slider.</param>
        /// <param name="itemWidth">How long the slider should be.</param>
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

        /// <summary>
        /// Draws a checkbox intended to be linked to other checkboxes sharing the same config value.
        /// </summary>
        /// <param name="config">The config ID.</param>
        /// <param name="checkBoxName">The name of the feature.</param>
        /// <param name="checkboxDescription">The description of the feature.</param>
        /// <param name="outputValue">If the user ticks this box, this is the value the config will be set to.</param>
        /// <param name="itemWidth"></param>
        /// <param name="descriptionColor"></param>
        public static void DrawRadioButton(string config, string checkBoxName, string checkboxDescription, int outputValue, float itemWidth = 150, Vector4 descriptionColor = new Vector4())
        {
            ImGui.Indent();
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudYellow;
            var output = Service.Configuration.GetCustomIntValue(config);
            Dalamud.Logging.PluginLog.Debug(output.ToString());
            ImGui.PushItemWidth(itemWidth);
            var enabled = output == outputValue ? true : false;

            if (ImGui.Checkbox(checkBoxName, ref enabled))
            {
                Service.Configuration.SetCustomIntValue(config, outputValue);
                Service.Configuration.Save();

            }
            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            ImGui.TextWrapped(checkboxDescription);
            ImGui.PopStyleColor();
            ImGui.Unindent();
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
