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
            if (!directImgUrl.IsNullOrEmpty())
            {
                byte[]? imageData = Service.Configuration.GetImageInCache(directImgUrl);
                if (imageData == null)
                {
                    Log.Debug("IsNull");
                    if (await ImageHandler.TryGetImage(directImgUrl))
                    {
                        imageData = Service.Configuration.GetImageInCache(directImgUrl);
                    }
                }

                if (imageData != null)
                {
                    ImGui.Image(Service.Interface.UiBuilder.LoadImage(imageData).ImGuiHandle, new Vector2(width, height));
                }

            }
        }

        public static void DrawJobGridMultiChoice(string config)
        {
            var values = Service.Configuration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, null, false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 20);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);

            if (ImGui.Checkbox($"Paladin###{config}0", ref values[0]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Warrior###{config}1", ref values[1]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dark Knight###{config}2", ref values[2]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Gunbreaker###{config}3", ref values[3]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            if (ImGui.Checkbox($"White Mage###{config}", ref values[4]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Scholar###{config}5", ref values[5]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Astrologian###{config}6", ref values[6]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Sage###{config}7", ref values[7]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);
            if (ImGui.Checkbox($"Monk###{config}8", ref values[8]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dragoon###{config}9", ref values[9]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Ninja###{config}10", ref values[10]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Samurai###{config}11", ref values[11]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Reaper###{config}12", ref values[12]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
            ImGui.NextColumn();

            if (ImGui.Checkbox($"Bard###{config}13", ref values[13]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Machinist###{config}14", ref values[14]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dancer###{config}15", ref values[15]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.NextColumn();


            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);
            if (ImGui.Checkbox($"Black Mage###{config}16", ref values[16]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();

            if (ImGui.Checkbox($"Summoner###{config}17", ref values[17]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();

            if (ImGui.Checkbox($"Red Mage###{config}18", ref values[18]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.NextColumn();
            if (ImGui.Checkbox($"Blue Mage###{config}19", ref values[19]))
            {
                Service.Configuration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            ImGui.PopStyleColor();
            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.Spacing();

        }

        public static void DrawJobGridSingleChoice(string config)
        {
            var value = Service.Configuration.GetCustomIntValue(config);
            bool[] values = new bool[20];
            
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
