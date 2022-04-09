using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using ImGuiScene;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Log = Dalamud.Logging.PluginLog;

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
        /// <summary>
        /// Draws an image located inside the plugins images folder
        /// </summary>
        /// <param name="imageName">Name of the image. Must be locted inside the images folder.</param>
        /// <param name="height">The output height of the image.</param>
        /// <param name="width">The output width of the image.</param>
        public static void DrawImage(string imageName, int height, int width)
        {
            var path = Path.Combine(Service.PluginFolder, "images", imageName);

            if (File.Exists(path))
            {
                var slothImage = Service.Interface.UiBuilder.LoadImage(path);
                if (slothImage != null)
                {
                    ImGui.Image(slothImage.ImGuiHandle, new Vector2(width, height));
                }
            }
        }

        public async static Task DrawDownloadedImage(string directImgUrl, int height, int width)
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
