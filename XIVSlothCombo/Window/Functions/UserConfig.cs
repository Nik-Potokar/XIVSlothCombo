using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Combos.PvP;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Functions
{
    public static class UserConfig
    {
        /// <summary> Draws a slider that lets the user set a given value for their feature. </summary>
        /// <param name="minValue"> The absolute minimum value you'll let the user pick. </param>
        /// <param name="maxValue"> The absolute maximum value you'll let the user pick. </param>
        /// <param name="config"> The config ID. </param>
        /// <param name="sliderDescription"> Description of the slider. Appends to the right of the slider. </param>
        /// <param name="itemWidth"> How long the slider should be. </param>
        /// <param name="sliderIncrement"> How much you want the user to increment the slider by. Uses SliderIncrements as a preset. </param>
        /// <param name="hasAdditionalChoice">True if this config can trigger additional configs depending on value.</param>
        /// <param name="additonalChoiceCondition">What the condition is to convey to the user what triggers it.</param>
        public static void DrawSliderInt(int minValue, int maxValue, string config, string sliderDescription, float itemWidth = 150, uint sliderIncrement = SliderIncrements.Ones, bool hasAdditionalChoice = false, string additonalChoiceCondition = "")
        {
            ImGui.Indent();
            int output = PluginConfiguration.GetCustomIntValue(config, minValue);
            if (output < minValue)
            {
                output = minValue;
                PluginConfiguration.SetCustomIntValue(config, output);
                Service.Configuration.Save();
            }

            sliderDescription = sliderDescription.Replace("%", "%%");
            float contentRegionMin = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y;
            float wrapPos = ImGui.GetContentRegionMax().X - 35f;

            InfoBox box = new()
            {
                Color = Colors.White,
                BorderThickness = 1f,
                CurveRadius = 3f,
                AutoResize = true,
                HasMaxWidth = true,
                IsSubBox = true,
                ContentsAction = () =>
                {
                    bool inputChanged = false;
                    Vector2 currentPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(currentPos.X + itemWidth);
                    ImGui.PushTextWrapPos(wrapPos);
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite);
                    ImGui.Text($"{sliderDescription}");
                    Vector2 height = ImGui.GetItemRectSize();
                    float lines = height.Y / ImGui.GetFontSize();
                    Vector2 textLength = ImGui.CalcTextSize(sliderDescription);
                    string newLines = "";
                    for (int i = 1; i < lines; i++)
                    {
                        if (i % 2 == 0)
                        {
                            newLines += "\n";
                        }
                        else
                        {
                            newLines += "\n\n";
                        }

                    }

                    if (hasAdditionalChoice)
                    {
                        ImGui.SameLine();
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Dummy(new Vector2(5, 0));
                        ImGui.SameLine();
                        ImGui.TextWrapped($"{FontAwesomeIcon.Search.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.PopStyleColor();

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"This setting has additional options depending on its value.{(string.IsNullOrEmpty(additonalChoiceCondition) ? "" : $"\nCondition: {additonalChoiceCondition}")}");
                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopTextWrapPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(currentPos.X);
                    ImGui.PushItemWidth(itemWidth);
                    inputChanged |= ImGui.SliderInt($"{newLines}###{config}", ref output, minValue, maxValue);

                    if (inputChanged)
                    {
                        if (output % sliderIncrement != 0)
                        {
                            output = output.RoundOff(sliderIncrement);
                            if (output < minValue) output = minValue;
                            if (output > maxValue) output = maxValue;
                        }

                        PluginConfiguration.SetCustomIntValue(config, output);
                        Service.Configuration.Save();
                    }
                }
            };

            box.Draw();
            ImGui.Spacing();
            ImGui.Unindent();
        }

        /// <summary> Draws a slider that lets the user set a given value for their feature. </summary>
        /// <param name="minValue"> The absolute minimum value you'll let the user pick. </param>
        /// <param name="maxValue"> The absolute maximum value you'll let the user pick. </param>
        /// <param name="config"> The config ID. </param>
        /// <param name="sliderDescription"> Description of the slider. Appends to the right of the slider. </param>
        /// <param name="itemWidth"> How long the slider should be. </param>
        /// <param name="hasAdditionalChoice"></param>
        /// <param name="additonalChoiceCondition"></param>
        public static void DrawSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150, bool hasAdditionalChoice = false, string additonalChoiceCondition = "")
        {
            float output = PluginConfiguration.GetCustomFloatValue(config, minValue);
            if (output < minValue)
            {
                output = minValue;
                PluginConfiguration.SetCustomFloatValue(config, output);
                Service.Configuration.Save();
            }

            sliderDescription = sliderDescription.Replace("%", "%%");
            float contentRegionMin = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y;
            float wrapPos = ImGui.GetContentRegionMax().X - 35f;


            InfoBox box = new()
            {
                Color = Colors.White,
                BorderThickness = 1f,
                CurveRadius = 3f,
                AutoResize = true,
                HasMaxWidth = true,
                IsSubBox = true,
                ContentsAction = () =>
                {
                    bool inputChanged = false;
                    Vector2 currentPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(currentPos.X + itemWidth);
                    ImGui.PushTextWrapPos(wrapPos);
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite);
                    ImGui.Text($"{sliderDescription}");
                    Vector2 height = ImGui.GetItemRectSize();
                    float lines = height.Y / ImGui.GetFontSize();
                    Vector2 textLength = ImGui.CalcTextSize(sliderDescription);
                    string newLines = "";
                    for (int i = 1; i < lines; i++)
                    {
                        if (i % 2 == 0)
                        {
                            newLines += "\n";
                        }
                        else
                        {
                            newLines += "\n\n";
                        }

                    }

                    if (hasAdditionalChoice)
                    {
                        ImGui.SameLine();
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Dummy(new Vector2(5, 0));
                        ImGui.SameLine();
                        ImGui.TextWrapped($"{FontAwesomeIcon.Search.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.PopStyleColor();

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"This setting has additional options depending on its value.{(string.IsNullOrEmpty(additonalChoiceCondition) ? "" : $"\nCondition: {additonalChoiceCondition}")}");
                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopTextWrapPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(currentPos.X);
                    ImGui.PushItemWidth(itemWidth);
                    inputChanged |= ImGui.SliderFloat($"{newLines}###{config}", ref output, minValue, maxValue);

                    if (inputChanged)
                    {
                        PluginConfiguration.SetCustomFloatValue(config, output);
                        Service.Configuration.Save();
                    }
                }
            };

            box.Draw();
            ImGui.Spacing();
        }

        /// <summary> Draws a slider that lets the user set a given value for their feature. </summary>
        /// <param name="minValue"> The absolute minimum value you'll let the user pick. </param>
        /// <param name="maxValue"> The absolute maximum value you'll let the user pick. </param>
        /// <param name="config"> The config ID. </param>
        /// <param name="sliderDescription"> Description of the slider. Appends to the right of the slider. </param>
        /// <param name="itemWidth"> How long the slider should be. </param>
        /// <param name="hasAdditionalChoice"></param>
        /// <param name="additonalChoiceCondition"></param>
        /// <param name="digits"></param>
        public static void DrawRoundedSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150, bool hasAdditionalChoice = false, string additonalChoiceCondition = "", int digits = 1)
        {
            float output = PluginConfiguration.GetCustomFloatValue(config, minValue);
            if (output < minValue)
            {
                output = minValue;
                PluginConfiguration.SetCustomFloatValue(config, output);
                Service.Configuration.Save();
            }

            sliderDescription = sliderDescription.Replace("%", "%%");
            float contentRegionMin = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y;
            float wrapPos = ImGui.GetContentRegionMax().X - 35f;


            InfoBox box = new()
            {
                Color = Colors.White,
                BorderThickness = 1f,
                CurveRadius = 3f,
                AutoResize = true,
                HasMaxWidth = true,
                IsSubBox = true,
                ContentsAction = () =>
                {
                    bool inputChanged = false;
                    Vector2 currentPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(currentPos.X + itemWidth);
                    ImGui.PushTextWrapPos(wrapPos);
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite);
                    ImGui.Text($"{sliderDescription}");
                    Vector2 height = ImGui.GetItemRectSize();
                    float lines = height.Y / ImGui.GetFontSize();
                    Vector2 textLength = ImGui.CalcTextSize(sliderDescription);
                    string newLines = "";
                    for (int i = 1; i < lines; i++)
                    {
                        if (i % 2 == 0)
                        {
                            newLines += "\n";
                        }
                        else
                        {
                            newLines += "\n\n";
                        }

                    }

                    if (hasAdditionalChoice)
                    {
                        ImGui.SameLine();
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Dummy(new Vector2(5, 0));
                        ImGui.SameLine();
                        ImGui.TextWrapped($"{FontAwesomeIcon.Search.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.PopStyleColor();

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"This setting has additional options depending on its value.{(string.IsNullOrEmpty(additonalChoiceCondition) ? "" : $"\nCondition: {additonalChoiceCondition}")}");
                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopTextWrapPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(currentPos.X);
                    ImGui.PushItemWidth(itemWidth);
                    inputChanged |= ImGui.SliderFloat($"{newLines}###{config}", ref output, minValue, maxValue, $"%.{digits}f");

                    if (inputChanged)
                    {
                        PluginConfiguration.SetCustomFloatValue(config, output);
                        Service.Configuration.Save();
                    }
                }
            };

            box.Draw();
            ImGui.Spacing();
        }

        /// <summary> Draws a checkbox intended to be linked to other checkboxes sharing the same config value. </summary>
        /// <param name="config"> The config ID. </param>
        /// <param name="checkBoxName"> The name of the feature. </param>
        /// <param name="checkboxDescription"> The description of the feature. </param>
        /// <param name="outputValue"> If the user ticks this box, this is the value the config will be set to. </param>
        /// <param name="itemWidth"></param>
        /// <param name="descriptionColor"></param>
        public static void DrawRadioButton(string config, string checkBoxName, string checkboxDescription, int outputValue, float itemWidth = 150, Vector4 descriptionColor = new Vector4())
        {
            ImGui.Indent();
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudYellow;
            int output = PluginConfiguration.GetCustomIntValue(config, outputValue);
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            bool enabled = output == outputValue;

            if (ImGui.RadioButton($"{checkBoxName}###{config}{outputValue}", enabled))
            {
                PluginConfiguration.SetCustomIntValue(config, outputValue);
                Service.Configuration.Save();
            }

            if (!checkboxDescription.IsNullOrEmpty())
            {
                ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
                ImGui.TextWrapped(checkboxDescription);
                ImGui.PopStyleColor();
            }

            ImGui.Unindent();
            ImGui.Spacing();
        }

        /// <summary> Draws a checkbox in a horizontal configuration intended to be linked to other checkboxes sharing the same config value. </summary>
        /// <param name="config"> The config ID. </param>
        /// <param name="checkBoxName"> The name of the feature. </param>
        /// <param name="checkboxDescription"> The description of the feature. </param>
        /// <param name="outputValue"> If the user ticks this box, this is the value the config will be set to. </param>
        /// <param name="itemWidth"></param>
        /// <param name="descriptionColor"></param>
        public static void DrawHorizontalRadioButton(string config, string checkBoxName, string checkboxDescription, int outputValue, float itemWidth = 150, Vector4 descriptionColor = new Vector4())
        {
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudYellow;
            int output = PluginConfiguration.GetCustomIntValue(config);
            ImGui.SameLine();
            ImGui.PushItemWidth(itemWidth);
            bool enabled = output == outputValue;

            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            if (ImGui.RadioButton($"{checkBoxName}###{config}{outputValue}", enabled))
            {
                PluginConfiguration.SetCustomIntValue(config, outputValue);
                Service.Configuration.Save();
            }

            if (!checkboxDescription.IsNullOrEmpty() && ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(checkboxDescription);
                ImGui.EndTooltip();
            }
            ImGui.PopStyleColor();

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(16f, 0));
        }

        /// <summary>A true or false configuration. Similar to presets except can be used as part of a condition on another config.</summary>
        /// <param name="config">The config ID.</param>
        /// <param name="checkBoxName">The name of the feature.</param>
        /// <param name="checkboxDescription">The description of the feature</param>
        /// <param name="itemWidth"></param>
        /// <param name="isConditionalChoice"></param>
        public static void DrawAdditionalBoolChoice(string config, string checkBoxName, string checkboxDescription, float itemWidth = 150, bool isConditionalChoice = false)
        {
            bool output = PluginConfiguration.GetCustomBoolValue(config);
            ImGui.PushItemWidth(itemWidth);
            if (!isConditionalChoice)
                ImGui.Indent();
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.AlignTextToFramePadding();
                ImGui.TextWrapped($"{FontAwesomeIcon.Plus.ToIconString()}");
                ImGui.PopFont();
                ImGui.PopStyleColor();
                ImGui.SameLine();
                ImGui.Dummy(new Vector2(3));
                ImGui.SameLine();
                if (isConditionalChoice) ImGui.Indent(); //Align checkbox after the + symbol
            }
            if (ImGui.Checkbox($"{checkBoxName}###{config}", ref output))
            {
                PluginConfiguration.SetCustomBoolValue(config, output);
                Service.Configuration.Save();
            }

            if (!checkboxDescription.IsNullOrEmpty())
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                ImGui.TextWrapped(checkboxDescription);
                ImGui.PopStyleColor();
            }

            //!isConditionalChoice
            ImGui.Unindent();
            ImGui.Spacing();
        }

        /// <summary> Draws multi choice checkboxes in a horizontal configuration. </summary>
        /// <param name="config"> The config ID. </param>
        /// <param name="checkBoxName"> The name of the feature. </param>
        /// <param name="checkboxDescription"> The description of the feature. </param>
        /// <param name="totalChoices"> The total number of options for the feature </param>
        /// /// <param name="choice"> If the user ticks this box, this is the value the config will be set to. </param>
        /// <param name="descriptionColor"></param>
        public static void DrawHorizontalMultiChoice(string config, string checkBoxName, string checkboxDescription, int totalChoices, int choice, Vector4 descriptionColor = new Vector4())
        {
            ImGui.Indent();
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudWhite;
            bool[]? values = PluginConfiguration.GetCustomBoolArrayValue(config);

            //If new saved options or amount of choices changed, resize and save
            if (values.Length == 0 || values.Length != totalChoices)
            {
                Array.Resize(ref values, totalChoices);
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            if (choice > 0)
            {
                ImGui.SameLine();
                ImGui.Dummy(new Vector2(12f, 0));
                ImGui.SameLine();
            }

            if (ImGui.Checkbox($"{checkBoxName}###{config}{choice}", ref values[choice]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }
            if (!checkboxDescription.IsNullOrEmpty() && ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(checkboxDescription);
                ImGui.EndTooltip();
            }


            ImGui.PopStyleColor();
            ImGui.Unindent();
        }

        public static void DrawGridMultiChoice(string config, byte columns, string[,] nameAndDesc, Vector4 descriptionColor = new Vector4())
        {
            int totalChoices = nameAndDesc.GetLength(0);
            if (totalChoices > 0)
            {
                ImGui.Indent();
                if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudWhite;
                //ImGui.PushItemWidth(itemWidth);
                //ImGui.SameLine();
                //ImGui.Dummy(new Vector2(21, 0));
                //ImGui.SameLine();
                bool[]? values = PluginConfiguration.GetCustomBoolArrayValue(config);

                //If new saved options or amount of choices changed, resize and save
                if (values.Length == 0 || values.Length != totalChoices)
                {
                    Array.Resize(ref values, totalChoices);
                    PluginConfiguration.SetCustomBoolArrayValue(config, values);
                    Service.Configuration.Save();
                }

                ImGui.BeginTable($"Grid###{config}", columns);
                ImGui.TableNextRow();
                //Convert the 2D array of names and descriptions into radio buttons
                for (int idx = 0; idx < totalChoices; idx++)
                {
                    ImGui.TableNextColumn();
                    string checkBoxName = nameAndDesc[idx, 0];
                    string checkboxDescription = nameAndDesc[idx, 1];

                    ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
                    if (ImGui.Checkbox($"{checkBoxName}###{config}{idx}", ref values[idx]))
                    {
                        PluginConfiguration.SetCustomBoolArrayValue(config, values);
                        Service.Configuration.Save();
                    }

                    if (!checkboxDescription.IsNullOrEmpty() && ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(checkboxDescription);
                        ImGui.EndTooltip();
                    }

                    ImGui.PopStyleColor();

                    if (((idx + 1) % columns) == 0)
                        ImGui.TableNextRow();
                }
                ImGui.EndTable();
                ImGui.Unindent();
            }
        }

        public static void DrawPvPStatusMultiChoice(string config)
        {
            bool[]? values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(4, $"{config}", false);

            if (values.Length == 0) Array.Resize(ref values, 7);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPink);

            if (ImGui.Checkbox($"Stun###{config}0", ref values[0]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Deep Freeze###{config}1", ref values[1]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Half Asleep###{config}2", ref values[2]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Sleep###{config}3", ref values[3]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Bind###{config}4", ref values[4]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Heavy###{config}5", ref values[5]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Silence###{config}6", ref values[6]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.Columns(1);
            ImGui.PopStyleColor();
            ImGui.Spacing();
        }

        public static void DrawRoleGridMultiChoice(string config)
        {
            bool[]? values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize(ref values, 5);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);

            if (ImGui.Checkbox($"Tanks###{config}0", ref values[0]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            if (ImGui.Checkbox($"Healers###{config}1", ref values[1]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);

            if (ImGui.Checkbox($"Melee###{config}2", ref values[2]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);

            if (ImGui.Checkbox($"Ranged###{config}3", ref values[3]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);

            if (ImGui.Checkbox($"Casters###{config}4", ref values[4]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.Columns(1);
            ImGui.PopStyleColor();
            ImGui.Spacing();
        }

        public static void DrawRoleGridSingleChoice(string config)
        {
            int value = PluginConfiguration.GetCustomIntValue(config);
            bool[] values = new bool[20];

            for (int i = 0; i <= 4; i++)
            {
                if (value == i) values[i] = true;
                else
                    values[i] = false;
            }

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize(ref values, 5);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);

            if (ImGui.Checkbox($"Tanks###{config}0", ref values[0]))
            {
                PluginConfiguration.SetCustomIntValue(config, 0);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            if (ImGui.Checkbox($"Healers###{config}1", ref values[1]))
            {
                PluginConfiguration.SetCustomIntValue(config, 1);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);

            if (ImGui.Checkbox($"Melee###{config}2", ref values[2]))
            {
                PluginConfiguration.SetCustomIntValue(config, 2);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);

            if (ImGui.Checkbox($"Ranged###{config}3", ref values[3]))
            {
                PluginConfiguration.SetCustomIntValue(config, 3);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);

            if (ImGui.Checkbox($"Casters###{config}4", ref values[4]))
            {
                PluginConfiguration.SetCustomIntValue(config, 4);
                Service.Configuration.Save();
            }

            ImGui.Columns(1);
            ImGui.PopStyleColor();
            ImGui.Spacing();
        }

        public static void DrawJobGridMultiChoice(string config)
        {
            bool[]? values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize(ref values, 20);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);

            if (ImGui.Checkbox($"Paladin###{config}0", ref values[0]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Warrior###{config}1", ref values[1]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dark Knight###{config}2", ref values[2]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Gunbreaker###{config}3", ref values[3]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            if (ImGui.Checkbox($"White Mage###{config}", ref values[4]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Scholar###{config}5", ref values[5]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Astrologian###{config}6", ref values[6]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Sage###{config}7", ref values[7]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);

            if (ImGui.Checkbox($"Monk###{config}8", ref values[8]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dragoon###{config}9", ref values[9]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Ninja###{config}10", ref values[10]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Samurai###{config}11", ref values[11]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Reaper###{config}12", ref values[12]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
            ImGui.NextColumn();

            if (ImGui.Checkbox($"Bard###{config}13", ref values[13]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Machinist###{config}14", ref values[14]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dancer###{config}15", ref values[15]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);

            if (ImGui.Checkbox($"Black Mage###{config}16", ref values[16]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Summoner###{config}17", ref values[17]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Red Mage###{config}18", ref values[18]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Blue Mage###{config}19", ref values[19]))
            {
                PluginConfiguration.SetCustomBoolArrayValue(config, values);
                Service.Configuration.Save();
            }

            ImGui.PopStyleColor();
            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.Spacing();
        }

        public static void DrawJobGridSingleChoice(string config)
        {
            int value = PluginConfiguration.GetCustomIntValue(config);
            bool[] values = new bool[20];

            for (int i = 0; i <= 19; i++)
            {
                if (value == i) values[i] = true;
                else
                    values[i] = false;
            }

            ImGui.Columns(5, null, false);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);

            if (ImGui.Checkbox($"Paladin###{config}0", ref values[0]))
            {
                PluginConfiguration.SetCustomIntValue(config, 0);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Warrior###{config}1", ref values[1]))
            {
                PluginConfiguration.SetCustomIntValue(config, 1);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dark Knight###{config}2", ref values[2]))
            {
                PluginConfiguration.SetCustomIntValue(config, 2);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Gunbreaker###{config}3", ref values[3]))
            {
                PluginConfiguration.SetCustomIntValue(config, 3);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            if (ImGui.Checkbox($"White Mage###{config}4", ref values[4]))
            {
                PluginConfiguration.SetCustomIntValue(config, 4);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Scholar###{config}5", ref values[5]))
            {
                PluginConfiguration.SetCustomIntValue(config, 5);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Astrologian###{config}6", ref values[6]))
            {
                PluginConfiguration.SetCustomIntValue(config, 6);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Sage###{config}7", ref values[7]))
            {
                PluginConfiguration.SetCustomIntValue(config, 7);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);

            if (ImGui.Checkbox($"Monk###{config}8", ref values[8]))
            {
                PluginConfiguration.SetCustomIntValue(config, 8);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dragoon###{config}9", ref values[9]))
            {
                PluginConfiguration.SetCustomIntValue(config, 9);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Ninja###{config}10", ref values[10]))
            {
                PluginConfiguration.SetCustomIntValue(config, 10);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Samurai###{config}11", ref values[11]))
            {
                PluginConfiguration.SetCustomIntValue(config, 11);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Reaper###{config}12", ref values[12]))
            {
                PluginConfiguration.SetCustomIntValue(config, 12);
                Service.Configuration.Save();
            }

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
            ImGui.NextColumn();

            if (ImGui.Checkbox($"Bard###{config}13", ref values[13]))
            {
                PluginConfiguration.SetCustomIntValue(config, 13);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Machinist###{config}14", ref values[14]))
            {
                PluginConfiguration.SetCustomIntValue(config, 14);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Dancer###{config}15", ref values[15]))
            {
                PluginConfiguration.SetCustomIntValue(config, 15);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.NextColumn();

            ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);

            if (ImGui.Checkbox($"Black Mage###{config}16", ref values[16]))
            {
                PluginConfiguration.SetCustomIntValue(config, 16);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Summoner###{config}17", ref values[17]))
            {
                PluginConfiguration.SetCustomIntValue(config, 17);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Red Mage###{config}18", ref values[18]))
            {
                PluginConfiguration.SetCustomIntValue(config, 18);
                Service.Configuration.Save();
            }

            ImGui.NextColumn();

            if (ImGui.Checkbox($"Blue Mage###{config}19", ref values[19]))
            {
                PluginConfiguration.SetCustomIntValue(config, 19);
                Service.Configuration.Save();
            }

            ImGui.PopStyleColor();
            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.Spacing();
        }

        internal static void DrawPriorityInput(UserIntArray config, int maxValues, int currentItem, string customLabel = "")
        {
            if (config.Count != maxValues || config.Any(x => x == 0))
            {
                config.Clear(maxValues);
                for (int i = 1; i <= maxValues; i++)
                {
                    config[i - 1] = i;
                }
            }

            int curVal = config[currentItem];
            int oldVal = config[currentItem];

            InfoBox box = new()
            {
                Color = Colors.Blue,
                BorderThickness = 1f,
                CurveRadius = 3f,
                AutoResize = true,
                HasMaxWidth = true,
                IsSubBox = true,
                ContentsAction = () =>
                {
                    if (customLabel.IsNullOrEmpty())
                    {
                        ImGui.TextUnformatted($"Priority: ");
                    }
                    else
                    {
                        ImGui.TextUnformatted(customLabel);
                    }
                    ImGui.SameLine();
                    ImGui.PushItemWidth(100f);

                    if (ImGui.InputInt($"###Priority{config.Name}{currentItem}", ref curVal))
                    {
                        for (int i = 0; i < maxValues; i++)
                        {
                            if (i == currentItem)
                                continue;

                            if (config[i] == curVal)
                            {
                                config[i] = oldVal;
                                config[currentItem] = curVal;
                                break;
                            }
                        }
                    }
                }
            };

            ImGui.Indent();
            box.Draw();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Smaller Number = Higher Priority");
                ImGui.EndTooltip();
            }
            ImGui.Unindent();
            ImGui.Spacing();
        }

        public static int RoundOff(this int i, uint sliderIncrement)
        {
            double sliderAsDouble = Convert.ToDouble(sliderIncrement);
            return ((int)Math.Round(i / sliderAsDouble)) * (int)sliderIncrement;
        }
    }

    public static class UserConfigItems
    {
        /// <summary> Draws the User Configurable settings. </summary>
        /// <param name="preset"> The preset it's attached to. </param>
        /// <param name="enabled"> If it's enabled or not. </param>
        internal static void Draw(CustomComboPreset preset, bool enabled)
        {
            if (!enabled) return;

            // ====================================================================================
            #region Misc

            #endregion
            // ====================================================================================
            #region ADV

            #endregion
            // ====================================================================================
            #region ASTROLOGIAN

            if (preset is CustomComboPreset.AST_ST_DPS)
            {
                UserConfig.DrawRadioButton(AST.Config.AST_DPS_AltMode, "On Malefic", "", 0);
                UserConfig.DrawRadioButton(AST.Config.AST_DPS_AltMode, "On Combust", "Alternative DPS Mode. Leaves Malefic alone for pure DPS, becomes Malefic when features are on cooldown", 1);
            }

            if (preset is CustomComboPreset.AST_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, AST.Config.AST_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.AST_ST_DPS_CombustUptime)
            {
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_ST_DPS_CombustUptime_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (PluginConfiguration.GetCustomBoolValue(AST.Config.AST_ST_DPS_CombustUptime_Adv))
                {
                    ImGui.Indent();
                    UserConfig.DrawRoundedSliderFloat(0, 4, AST.Config.AST_ST_DPS_CombustUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                    ImGui.Unindent();
                }

            }

            if (preset is CustomComboPreset.AST_DPS_Divination)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

            if (preset is CustomComboPreset.AST_DPS_LightSpeed)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");


            if (preset is CustomComboPreset.AST_ST_SimpleHeals)
            {
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_ST_SimpleHeals_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (AST.Config.AST_ST_SimpleHeals_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_ST_SimpleHeals_UIMouseOver,
                        "Party UI Mouseover Checking",
                        "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                        "To be used in conjunction with Redirect/Reaction/etc");
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_EssentialDignity, "Set percentage value");

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_Spire)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_Spire, "Set percentage value");

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_Ewer)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_Ewer, "Set percentage value");

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_Esuna)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_ST_SimpleHeals_Esuna, "Stop using when below HP %. Set to Zero to disable this check");


            if (preset is CustomComboPreset.AST_AoE_SimpleHeals_LazyLady)
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_AoE_SimpleHeals_WeaveLady, "Only Weave", "Will only weave this action.");

            if (preset is CustomComboPreset.AST_AoE_SimpleHeals_Horoscope)
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_AoE_SimpleHeals_Horoscope, "Only Weave", "Will only weave this action.");

            if (preset is CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition)
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_AoE_SimpleHeals_Opposition, "Only Weave", "Will only weave this action.");

            if (preset is CustomComboPreset.AST_Cards_QuickTargetCards)
            {
                UserConfig.DrawRadioButton(AST.Config.AST_QuickTarget_Override, "No Override", "", 0);
                UserConfig.DrawRadioButton(AST.Config.AST_QuickTarget_Override, "Hard Target Override", "Overrides selection with hard target if you have one", 1);
                UserConfig.DrawRadioButton(AST.Config.AST_QuickTarget_Override, "UI Mousover Override", "Overrides selection with UI mouseover target if you have one", 2);

                ImGui.Spacing();
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_QuickTarget_SkipDamageDown, $"Skip targets with a {ActionWatching.GetStatusName(62)} debuff", "");
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_QuickTarget_SkipRezWeakness, $"Skip targets with a {ActionWatching.GetStatusName(43)} or {ActionWatching.GetStatusName(44)} debuff", "");
            }

            if (preset is CustomComboPreset.AST_DPS_AutoPlay)
            {
                UserConfig.DrawRadioButton(AST.Config.AST_ST_DPS_Play_SpeedSetting, "Fast (1 DPS GCD minimum delay)", "", 1);
                UserConfig.DrawRadioButton(AST.Config.AST_ST_DPS_Play_SpeedSetting, "Medium (2 DPS GCD minimum delay)", "", 2);
                UserConfig.DrawRadioButton(AST.Config.AST_ST_DPS_Play_SpeedSetting, "Slow (3 DPS GCD minimum delay)", "", 3);

            }

            if (preset is CustomComboPreset.AST_DPS_AutoDraw)
            {
                UserConfig.DrawAdditionalBoolChoice(AST.Config.AST_ST_DPS_OverwriteCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
            }

            #endregion
            // ====================================================================================
            #region BLACK MAGE

            if (preset is CustomComboPreset.BLM_ST_AdvancedMode)
            {
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_InitialCast, "Fire 3 Initial Cast", "", 0);
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_InitialCast, "Blizzard 3 Initial Cast", "", 1);
                ImGui.Indent();
                UserConfig.DrawRoundedSliderFloat(3.0f, 8.0f, BLM.Config.BLM_AstralFire_Refresh, "Seconds before refreshing Astral Fire");
                ImGui.Unindent();
            }

            if (preset is CustomComboPreset.BLM_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, BLM.Config.BLM_VariantCure, "HP% to be at or under", 200);

            if (preset is CustomComboPreset.BLM_Adv_Opener)
            {
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Advanced_OpenerSelection, "Standard Opener", "Uses Standard Opener.", 0);
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Advanced_OpenerSelection, "Double Transpose Opener", "Uses Fire III opener - Double Transpose variation.", 1);
            }

            if (preset is CustomComboPreset.BLM_Adv_Rotation)
            {
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_Rotation_Options, "Standard Rotation", "Uses Standard Rotation.", 0);
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_Rotation_Options, "Double Transpose Rotation", "Uses Double Transpose rotation.\nOnly works at Lv.90.", 1);

                if (BLM.Config.BLM_Adv_Rotation_Options == 0)
                {
                    ImGui.Indent();
                    UserConfig.DrawAdditionalBoolChoice(BLM.Config.BLM_Adv_Xeno_Burst, "Use Xenoglossy for burst", "Will save Xenoglossy for every minute burst window.");
                    ImGui.Unindent();
                    ImGui.Spacing();
                }

            }

            if (preset is CustomComboPreset.BLM_Adv_Cooldowns)
            {
                UserConfig.DrawGridMultiChoice(BLM.Config.BLM_Adv_Cooldowns_Choice, 4, new string[,]{
                    {"Manafont", "Add Manafont to the rotation." },
                    {"Sharpcast", "Add Sharpcast to the rotation." },
                    {"Amplifier", "Add Amplifier to the rotation." },
                    {"Ley Lines", "Add Ley Lines to the rotation." },
                });
            }

            if (preset is CustomComboPreset.BLM_AoE_Adv_Cooldowns)
            {
                UserConfig.DrawGridMultiChoice(BLM.Config.BLM_AoE_Adv_Cooldowns_Choice, 5, new string[,]{
                    {$"Manafont", "Add Manafont to the rotation." },
                    {"Sharpcast", "Add Sharpcast to the rotation." },
                    {"Amplifier", "Add Amplifier to the rotation." },
                    {"Ley Lines", "Add Ley Lines to the rotation." },
                    {"Triplecast", "Add Triplecast to the rotation" }
                });
            }

            if (preset is CustomComboPreset.BLM_Adv_Movement)
            {
                UserConfig.DrawGridMultiChoice(BLM.Config.BLM_Adv_Movement_Choice, 4, new string[,]{
                    {"Sharpcast", "Add Sharpcast." },
                    {"Thunder", "Add Thunder I/Thunder III." },
                    {"Firestarter", "Add Firestarter when in Astral Fire." },
                    {"Paradox", "Add Paradox when in Umbral Ice." },
                    {"Xenoglossy", "Add Xenoglossy.\nOne charge will be held for rotation." },
                    {"Swiftcast", "Add Swiftcast." },
                    {"Triplecast", "Add (pooled) Triplecast." },
                    {"Scathe", "Add Scathe." }
                });
            }

            if (preset is CustomComboPreset.BLM_ST_Adv_Thunder)
                UserConfig.DrawSliderInt(0, 5, BLM.Config.BLM_Adv_Thunder, "Seconds remaining before refreshing Thunder");

            if (preset is CustomComboPreset.BLM_AoE_Adv_ThunderUptime)
                UserConfig.DrawSliderInt(0, 5, BLM.Config.BLM_AoE_Adv_ThunderUptime, "Seconds remaining before refreshing Thunder");

            if (preset is CustomComboPreset.BLM_ST_Adv_Thunder)
                UserConfig.DrawSliderInt(0, 5, BLM.Config.BLM_ST_Adv_ThunderHP, "Target HP% to stop using Thunder");

            if (preset is CustomComboPreset.BLM_AoE_Adv_ThunderUptime)
                UserConfig.DrawSliderInt(0, 5, BLM.Config.BLM_AoE_Adv_ThunderHP, "Target HP% to stop using Thunder");

            if (preset is CustomComboPreset.BLM_ST_Adv_Thunder_ThunderCloud)
            {
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_ThunderCloud, "Only after quicker casts (weave window)", "", 0);
                UserConfig.DrawHorizontalRadioButton(BLM.Config.BLM_Adv_ThunderCloud, "Use as soon as possible", "", 1);
            }

            #endregion
            // ====================================================================================
            #region BLUE MAGE

            #endregion
            // ====================================================================================
            #region BARD

            if (preset == CustomComboPreset.BRD_Simple_RagingJaws)
                UserConfig.DrawSliderInt(3, 5, BRD.Config.BRD_RagingJawsRenewTime, "Remaining time (In seconds)");

            if (preset == CustomComboPreset.BRD_Simple_NoWaste)
                UserConfig.DrawSliderInt(1, 10, BRD.Config.BRD_NoWasteHPPercentage, "Remaining target HP percentage");

            if (preset == CustomComboPreset.BRD_AoE_Simple_NoWaste)
                UserConfig.DrawSliderInt(1, 10, BRD.Config.BRD_AoENoWasteHPPercentage, "Remaining target HP percentage");

            if (preset == CustomComboPreset.BRD_ST_SecondWind)
                UserConfig.DrawSliderInt(0, 100, BRD.Config.BRD_STSecondWindThreshold, "HP percent threshold to use Second Wind below.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.BRD_AoE_SecondWind)
                UserConfig.DrawSliderInt(0, 100, BRD.Config.BRD_AoESecondWindThreshold, "HP percent threshold to use Second Wind below.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.BRD_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, BRD.Config.BRD_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region DANCER

            if (preset == CustomComboPreset.DNC_DanceComboReplacer)
            {
                //int[]? actions = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                int[]? actions = Service.Configuration.DancerDanceCompatActionIDs.Select(x => (int)x).ToArray();


                bool inputChanged = false;

                inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
                inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
                inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
                inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

                if (inputChanged)
                {
                    //Service.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                    Service.Configuration.DancerDanceCompatActionIDs = actions.Select(x => (uint)x).ToArray();
                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }

            if (preset == CustomComboPreset.DNC_ST_EspritOvercap)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_ST, "Esprit", 150, SliderIncrements.Fives);

            if (preset == CustomComboPreset.DNC_AoE_EspritOvercap)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_AoE, "Esprit", 150, SliderIncrements.Fives);

            if (preset == CustomComboPreset.DNC_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, DNC.Config.DNCVariantCurePercent, "HP% to be at or under", 200);

            #region Simple ST Sliders

            if (preset == CustomComboPreset.DNC_ST_Simple_SS)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleSSBurstPercent, "Target HP% to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_TS)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleTSBurstPercent, "Target HP% to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_Feathers)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleFeatherBurstPercent, "Target HP% to dump all pooled feathers below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_SaberDance)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCSimpleSaberThreshold, "Esprit", 150, SliderIncrements.Fives);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWaltzPercent, "Curing Waltz HP%", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWindPercent, "Second Wind HP%", 200, SliderIncrements.Ones);

            #endregion

            #region Simple AoE Sliders

            if (preset == CustomComboPreset.DNC_AoE_Simple_SS)
                UserConfig.DrawSliderInt(0, 10, DNC.Config.DNCSimpleSSAoEBurstPercent, "Target HP% to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_TS)
                UserConfig.DrawSliderInt(0, 10, DNC.Config.DNCSimpleTSAoEBurstPercent, "Target HP% to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_SaberDance)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCSimpleAoESaberThreshold, "Esprit", 150, SliderIncrements.Fives);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWaltzPercent, "Curing Waltz HP%", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWindPercent, "Second Wind HP%", 200, SliderIncrements.Ones);

            #endregion

            #region PvP Sliders

            if (preset == CustomComboPreset.DNCPvP_BurstMode_CuringWaltz)
                UserConfig.DrawSliderInt(0, 90, DNCPvP.Config.DNCPvP_WaltzThreshold, "Curing Waltz HP% - caps at 90 to prevent waste.", 150, SliderIncrements.Ones);

            #endregion

            #endregion
            // ====================================================================================
            #region DARK KNIGHT

            if (preset == CustomComboPreset.DRK_ST_ManaSpenderPooling && enabled)
                UserConfig.DrawSliderInt(0, 3000, DRK.Config.DRK_ST_ManaSpenderPooling, "How much MP to save (0 = Use All)", 150, SliderIncrements.Thousands);

            if (preset == CustomComboPreset.DRK_ST_CDs_LivingShadow && enabled)
                UserConfig.DrawSliderInt(0, 100, DRK.Config.DRK_ST_LivingDeadThreshold, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.DRK_AoE_CDs_LivingShadow && enabled)
                UserConfig.DrawSliderInt(0, 100, DRK.Config.DRK_AoE_LivingDeadThreshold, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.DRKPvP_Burst)
                UserConfig.DrawSliderInt(1, 100, DRKPvP.Config.ShadowbringerThreshold, "HP% to be at or above to use Shadowbringer");

            if (preset == CustomComboPreset.DRK_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, DRK.Config.DRK_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region DRAGOON
            if (preset == CustomComboPreset.DRG_ST_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_ST_SecondWind_Threshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_ST_Bloodbath_Threshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.DRG_AoE_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_AoE_SecondWind_Threshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_AoE_Bloodbath_Threshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.DRG_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, DRG.Config.DRG_Variant_Cure, "HP% to be at or under", 200);

            if (preset == CustomComboPreset.DRG_ST_Litany)
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_ST_LitanyHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.DRG_ST_Lance)
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_ST_LanceChargeHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.DRG_AoE_Litany)
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_AoE_LitanyHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.DRG_AoE_Lance)
                UserConfig.DrawSliderInt(0, 100, DRG.Config.DRG_AoE_LanceChargeHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");


            #region Dragoon PvP
            if (preset is CustomComboPreset.DRGPvP_Nastrond)
                UserConfig.DrawSliderInt(0, 100, DRGPvP.Config.DRGPvP_LOTD_HPValue, "Ends Life of the Dragon if HP falls below the set percentage", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.DRGPvP_Nastrond)
                UserConfig.DrawSliderInt(2, 8, DRGPvP.Config.DRGPvP_LOTD_Duration, "Seconds remaining of Life of the Dragon buff before using Nastrond if you are still above the set HP percentage.", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.DRGPvP_ChaoticSpringSustain)
                UserConfig.DrawSliderInt(0, 101, DRGPvP.Config.DRGPvP_CS_HP_Threshold, "Chaos Spring HP percentage threshold", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.DRGPvP_WyrmwindThrust)
                UserConfig.DrawSliderInt(0, 20, DRGPvP.Config.DRGPvP_Distance_Threshold, "Distance Treshold for Wyrmwind Thrust", 150, SliderIncrements.Ones);
            #endregion

            #endregion
            // ====================================================================================
            #region GUNBREAKER

            if (preset == CustomComboPreset.GNB_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, GNB.Config.GNB_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region MACHINIST

            if (preset == CustomComboPreset.MCH_ST_Adv_Reassemble)
                UserConfig.DrawSliderInt(0, 1, MCH.Config.MCH_ST_ReassemblePool, "Number of Charges to Save for Manual Use");

            if (preset == CustomComboPreset.MCH_AoE_Adv_Reassemble)
                UserConfig.DrawSliderInt(0, 1, MCH.Config.MCH_AoE_ReassemblePool, "Number of Charges to Save for Manual Use");

            if (preset is CustomComboPreset.MCH_ST_Adv_Reassemble)
            {
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.Excavator)}", "", 5, 0);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.Chainsaw)}", "", 5, 1);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.AirAnchor)}", "", 5, 2);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.Drill)}", "", 5, 3);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.CleanShot)}", "", 5, 4);
            }

            if (preset is CustomComboPreset.MCH_AoE_Adv_Reassemble)
            {
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.SpreadShot)}/{ActionWatching.GetActionName(MCH.Scattergun)}", "", 4, 0);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.AutoCrossbow)}", "", 4, 1);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.Chainsaw)}", "", 4, 2);
                UserConfig.DrawHorizontalMultiChoice(MCH.Config.MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(MCH.Excavator)}", "", 4, 3);
            }

            if (preset == CustomComboPreset.MCH_ST_Adv_SecondWind)
                UserConfig.DrawSliderInt(0, 100, MCH.Config.MCH_ST_SecondWindThreshold, $"{ActionWatching.GetActionName(All.SecondWind)} HP percentage threshold", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.MCH_AoE_Adv_SecondWind)
                UserConfig.DrawSliderInt(0, 100, MCH.Config.MCH_AoE_SecondWindThreshold, $"{ActionWatching.GetActionName(All.SecondWind)} HP percentage threshold", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.MCH_AoE_Adv_Queen)
                UserConfig.DrawSliderInt(50, 100, MCH.Config.MCH_AoE_TurretUsage, "Battery threshold", sliderIncrement: 5);

            if (preset == CustomComboPreset.MCH_AoE_Adv_GaussRicochet)
                UserConfig.DrawAdditionalBoolChoice(MCH.Config.MCH_AoE_Hypercharge, $"Use Outwith {ActionWatching.GetActionName(MCH.Hypercharge)}", "");

            if (preset == CustomComboPreset.MCH_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, MCH.Config.MCH_VariantCure, "HP% to be at or under", 200);

            if (preset == CustomComboPreset.MCH_ST_Adv_QueenOverdrive)
                UserConfig.DrawSliderInt(1, 10, MCH.Config.MCH_ST_QueenOverDrive, "HP% for the target to be at or under");

            if (preset == CustomComboPreset.MCH_ST_Adv_WildFire)
                UserConfig.DrawSliderInt(0, 15, MCH.Config.MCH_ST_WildfireHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            if (preset == CustomComboPreset.MCH_ST_Adv_Hypercharge)
                UserConfig.DrawSliderInt(0, 15, MCH.Config.MCH_ST_HyperchargeHP, "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

            #endregion
            // ====================================================================================
            #region MONK

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                UserConfig.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_Demolish_Apply, "Seconds remaining before refreshing Demolish.");

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                UserConfig.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_DisciplinedFist_Apply, "Seconds remaining before refreshing Disciplined Fist.");

            if (preset == CustomComboPreset.MNK_ST_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, MNK.Config.MNK_STSecondWindThreshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, MNK.Config.MNK_STBloodbathThreshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.MNK_AoE_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, MNK.Config.MNK_AoESecondWindThreshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, MNK.Config.MNK_AoEBloodbathThreshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.MNK_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, MNK.Config.MNK_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region NINJA

            if (preset == CustomComboPreset.NIN_Simple_Mudras)
            {
                UserConfig.DrawRadioButton(NIN.Config.NIN_SimpleMudra_Choice, "Mudra Path Set 1", $"1. Ten Mudras -> Fuma Shuriken, Raiton/Hyosho Ranryu, Suiton (Doton under Kassatsu).\nChi Mudras -> Fuma Shuriken, Hyoton, Huton.\nJin Mudras -> Fuma Shuriken, Katon/Goka Mekkyaku, Doton", 1);
                UserConfig.DrawRadioButton(NIN.Config.NIN_SimpleMudra_Choice, "Mudra Path Set 2", $"2. Ten Mudras -> Fuma Shuriken, Hyoton/Hyosho Ranryu, Doton.\nChi Mudras -> Fuma Shuriken, Katon, Suiton.\nJin Mudras -> Fuma Shuriken, Raiton/Goka Mekkyaku, Huton (Doton under Kassatsu).", 2);
            }

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Huraijin)
                UserConfig.DrawSliderInt(0, 60, NIN.Config.Huton_RemainingHuraijinST, "Set the amount of time remaining on Huton the feature should wait before using Huraijin");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_ArmorCrush)
            {
                UserConfig.DrawSliderInt(0, 30, NIN.Config.Huton_RemainingArmorCrush, "Set the amount of time remaining on Huton the feature should wait before using Armor Crush", hasAdditionalChoice: true, additonalChoiceCondition: "Value set to 12 or less.");

                if (PluginConfiguration.GetCustomIntValue(NIN.Config.Huton_RemainingArmorCrush) <= 12)
                    UserConfig.DrawAdditionalBoolChoice(NIN.Config.Advanced_DoubleArmorCrush, "Double Armor Crush Feature", "Uses the Armor Crush ender twice before switching back to Aeolian Edge.", isConditionalChoice: true);
            }

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BhavaPooling, "Set the minimal amount of Ninki required to have before spending on Bhavacakra.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack)
                UserConfig.DrawSliderInt(0, 21, NIN.Config.Trick_CooldownRemaining, "Set the amount of time remaining on Trick Attack cooldown before trying to set up with Suiton.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Bunshin)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPoolingST, "Set the amount of Ninki required to have before spending on Bunshin.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPoolingAoE, "Set the amount of Ninki required to have before spending on Bunshin.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Cooldowns)
                UserConfig.DrawSliderInt(0, 21, NIN.Config.Advanced_Trick_Cooldown, "Set the amount of time remaining on Trick Attack cooldown to start saving cooldowns.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_SecondWind)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.SecondWindThresholdST, "Set a HP% threshold for when Second Wind will be used.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.ShadeShiftThresholdST, "Set a HP% threshold for when Shade Shift will be used.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.BloodbathThresholdST, "Set a HP% threshold for when Bloodbath will be used.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_SecondWind)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.SecondWindThresholdAoE, "Set a HP% threshold for when Second Wind will be used.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_ShadeShift)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.ShadeShiftThresholdAoE, "Set a HP% threshold for when Shade Shift will be used.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_Bloodbath)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.BloodbathThresholdAoE, "Set a HP% threshold for when Bloodbath will be used.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_HellfrogPooling, "Set the amount of Ninki required to have before spending on Hellfrog Medium.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton)
            {
                UserConfig.DrawSliderInt(0, 18, NIN.Config.Advanced_DotonTimer, "Sets the amount of time remaining on Doton before casting again.");
                UserConfig.DrawSliderInt(0, 100, NIN.Config.Advanced_DotonHP, "Sets the max remaining HP percentage of the current target to cast Doton.");
            }

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_TCJ)
            {
                UserConfig.DrawRadioButton(NIN.Config.Advanced_TCJEnderAoE, "Ten Chi Jin Ender 1", "Ends Ten Chi Jin with Suiton.", 0);
                UserConfig.DrawRadioButton(NIN.Config.Advanced_TCJEnderAoE, $"Ten Chi Jin Ender 2", "Ends Ten Chi Jin with Doton.\nIf you have Doton enabled, Ten Chi Jin will be delayed according to the settings in that feature.", 1);
            }

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton)
            {
                UserConfig.DrawAdditionalBoolChoice(NIN.Config.Advanced_ChargePool, "Pool Charges", "Waits until at least 2 seconds before your 2nd charge or if Trick Attack debuff is on your target before spending.");
            }

            if (preset == CustomComboPreset.NIN_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, NIN.Config.NIN_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region PICTOMANCER
            if (preset == CustomComboPreset.CombinedAetherhues)
            {
                UserConfig.DrawRadioButton(PCT.Config.CombinedAetherhueChoices, "Both Single Target & AoE", $"Replaces both {PCT.FireInRed.ActionName()} & {PCT.FireIIinRed.ActionName()}", 0);
                UserConfig.DrawRadioButton(PCT.Config.CombinedAetherhueChoices, "Single Target Only", $"Replace only {PCT.FireInRed.ActionName()}", 1);
                UserConfig.DrawRadioButton(PCT.Config.CombinedAetherhueChoices, "AoE Only", $"Replace only {PCT.FireIIinRed.ActionName()}", 2);
            }

            if (preset == CustomComboPreset.CombinedMotifs)
            {
                UserConfig.DrawAdditionalBoolChoice(PCT.Config.CombinedMotifsMog, $"{PCT.MogoftheAges.ActionName()} Feature", $"Add {PCT.MogoftheAges.ActionName()} when fully drawn and off cooldown.");
                UserConfig.DrawAdditionalBoolChoice(PCT.Config.CombinedMotifsMadeen, $"{PCT.RetributionoftheMadeen.ActionName()} Feature", $"Add {PCT.RetributionoftheMadeen.ActionName()} when fully drawn and off cooldown.");
                UserConfig.DrawAdditionalBoolChoice(PCT.Config.CombinedMotifsWeapon, $"{PCT.HammerStamp.ActionName()} Feature", $"Add {PCT.HammerStamp.ActionName()} when under the effect of {PCT.Buffs.HammerTime.StatusName()}.");
            }

            #endregion
            // ====================================================================================
            #region PALADIN

            if (preset == CustomComboPreset.PLD_Requiescat_Options)
            {
                UserConfig.DrawRadioButton(PLD.Config.PLD_RequiescatOption, "Confiteor", "", 1);
                UserConfig.DrawRadioButton(PLD.Config.PLD_RequiescatOption, "Blade of Faith/Truth/Valor", "", 2);
                UserConfig.DrawRadioButton(PLD.Config.PLD_RequiescatOption, "Confiteor & Blade of Faith/Truth/Valor", "", 3);
                UserConfig.DrawRadioButton(PLD.Config.PLD_RequiescatOption, "Holy Spirit", "", 4);
                UserConfig.DrawRadioButton(PLD.Config.PLD_RequiescatOption, "Holy Circle", "", 5);
            }

            // Removed Requiescat weaving options pending further assessment - Kaeris
            //if (preset is CustomComboPreset.PLD_ST_AdvancedMode_Requiescat)
            //{
            //    UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_ST_RequiescatWeave, "Standard Weave", "Weaves Requiescat immediately after Fight or Flight for a standard burst window.", 0, 150, ImGuiColors.ParsedGreen);
            //    UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_ST_RequiescatWeave, "Late Weave", "Late-weaves Requiescat after Fight or Flight.\nEnsures Requiescat's damage falls under the Fight or Flight buff, but may lead to misalignment in longer fights.", 1, 150, ImGuiColors.DalamudYellow);
            //    ImGui.Spacing();
            //}

            //if (preset is CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat)
            //{
            //    UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_AoE_RequiescatWeave, "Standard Weave", "Weaves Requiescat immediately after Fight or Flight for a standard burst window.", 0, 150, ImGuiColors.ParsedGreen);
            //    UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_AoE_RequiescatWeave, "Late Weave", "Late-weaves Requiescat after Fight or Flight.\nEnsures Requiescat's damage falls under the Fight or Flight buff, but may lead to misalignment in longer fights.", 1, 150, ImGuiColors.DalamudYellow);
            //    ImGui.Spacing();
            //}

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve)
            {
                UserConfig.DrawSliderInt(1000, 10000, PLD.Config.PLD_ST_MP_Reserve, "Minimum MP gauge required.", sliderIncrement: 100);
            }

            if (preset == CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve)
            {
                UserConfig.DrawSliderInt(1000, 10000, PLD.Config.PLD_AoE_MP_Reserve, "Minimum MP gauge required.", sliderIncrement: 100);
            }


            if (preset == CustomComboPreset.PLD_SpiritsWithin)
            {
                UserConfig.DrawRadioButton(PLD.Config.PLD_SpiritsWithinOption, "Prioritize Circle of Scorn", "", 1);
                UserConfig.DrawRadioButton(PLD.Config.PLD_SpiritsWithinOption, "Prioritize Spirits Within / Expiacion", "", 2);
            }

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_FoF)
                UserConfig.DrawSliderInt(0, 50, PLD.Config.PLD_ST_FoF_Trigger, "Target HP%", 200);

            if (preset == CustomComboPreset.PLD_AoE_AdvancedMode_FoF)
                UserConfig.DrawSliderInt(0, 50, PLD.Config.PLD_AoE_FoF_Trigger, "Target HP%", 200);

            //if (preset == CustomComboPreset.PLD_ST_AdvancedMode_Sheltron)
            //    UserConfig.DrawSliderInt(0, 100, PLD.Config.PLD_ST_SheltronHP, "Player HP%", 200);

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_Sheltron)
                UserConfig.DrawSliderInt(50, 100, PLD.Config.PLD_ST_SheltronOption, "Oath Gauge", 200, 5);

            //if (preset == CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron)
            //    UserConfig.DrawSliderInt(0, 100, PLD.Config.PLD_AoE_SheltronHP, "Player HP%", 200);

            if (preset == CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron)
                UserConfig.DrawSliderInt(50, 100, PLD.Config.PLD_AoE_SheltronOption, "Oath Gauge", 200, 5);

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_Intervene)
                UserConfig.DrawSliderInt(0, 1, PLD.Config.PLD_Intervene_HoldCharges, "Charges", 200);

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_Intervene)
            {
                UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_Intervene_MeleeOnly, "Melee Range", "Uses Intervene while within melee range.\nMay result in minor movement.", 1);
                UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_Intervene_MeleeOnly, "No Movement", "Only uses Intervene when it would not result in movement (zero distance).", 2);
            }

            if (preset == CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob)
            {
                UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_ShieldLob_SubOption, "Shield Lob Only", "Uses only Shield Lob.", 1);
                UserConfig.DrawHorizontalRadioButton(PLD.Config.PLD_ShieldLob_SubOption, "Hardcast Holy Spirit", "Attempts to hardcast Holy Spirit when not moving.\nOtherwise uses Shield Lob.", 2);
            }

            if (preset == CustomComboPreset.PLD_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, PLD.Config.PLD_VariantCure, "Player HP%", 200);

            // New logic handles early/late spend for Atonement and Holy Spirit automatically - Kaeris
            //if (preset == CustomComboPreset.PLD_ST_AdvancedMode_Atonement)
            //{
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_AtonementTiming, "Early Spend", "Uses Atonement before restarting the basic combo.", 1);
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_AtonementTiming, "Late Spend", "Uses Atonement before the end of the basic combo.", 2);
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_AtonementTiming, "Alternate Spend", "Switches between early and and late depending on how often Flight or Fight is used.", 3);
            //}

            //if (preset == CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit)
            //{
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_DivineMightTiming, "Early Spend", "When under the effect of Divine Might, use Holy Spirit before restarting the basic combo.", 1);
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_DivineMightTiming, "Late Spend", "When under the effect of Divine Might, uses Holy Spirit before the end of the basic combo.", 2);
            //    UserConfig.DrawRadioButton(PLD.Config.PLD_ST_DivineMightTiming, "Alternate Spend", "When under the effect of Divine Might, switches between early and late depending on how often Flight or Fight is used.", 3);
            //}

            #endregion
            // ====================================================================================
            #region REAPER

            if (preset == CustomComboPreset.RPRPvP_Burst_ImmortalPooling && enabled)
                UserConfig.DrawSliderInt(0, 8, RPRPvP.Config.RPRPvP_ImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPRPvP_Burst_ArcaneCircle && enabled)
                UserConfig.DrawSliderInt(5, 90, RPRPvP.Config.RPRPvP_ArcaneCircleThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPR_ST_AdvancedMode && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_Positional, "Rear First", "First positional: Gallows.", 0);
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_Positional, "Flank First", "First positional: Gibbet.", 1);
            }

            if (preset == CustomComboPreset.RPRPvP_Burst_ImmortalPooling && enabled)
                UserConfig.DrawSliderInt(0, 8, RPRPvP.Config.RPRPvP_ImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPRPvP_Burst_ArcaneCircle && enabled)
                UserConfig.DrawSliderInt(5, 90, RPRPvP.Config.RPRPvP_ArcaneCircleThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPR_ST_SoD && enabled)
            {
                UserConfig.DrawSliderInt(0, 6, RPR.Config.RPR_SoDRefreshRange, "Seconds remaining before refreshing Death's Design.", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 5, RPR.Config.RPR_SoDThreshold, "Set a HP% Threshold for when SoD will not be automatically applied to the target.", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.RPR_AoE_WoD && enabled)
            {
                UserConfig.DrawSliderInt(0, 5, RPR.Config.RPR_WoDThreshold, "Set a HP% Threshold for when WoD will not be automatically applied to the target.", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.RPR_ST_ComboHeals && enabled)
            {
                UserConfig.DrawSliderInt(0, 100, RPR.Config.RPR_STSecondWindThreshold, "HP percent threshold to use Second Wind below (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, RPR.Config.RPR_STBloodbathThreshold, "HP percent threshold to use Bloodbath (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.RPR_AoE_ComboHeals && enabled)
            {
                UserConfig.DrawSliderInt(0, 100, RPR.Config.RPR_AoESecondWindThreshold, "HP percent threshold to use Second Wind below (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, RPR.Config.RPR_AoEBloodbathThreshold, "HP percent threshold to use Bloodbath below (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.RPR_Soulsow && enabled)
            {
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Harpe", "Adds Soulsow to Harpe.", 5, 0);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Slice", "Adds Soulsow to Slice.", 5, 1);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Spinning Scythe", "Adds Soulsow to Spinning Scythe", 5, 2);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Shadow of Death", "Adds Soulsow to Shadow of Death.", 5, 3);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Blood Stalk", "Adds Soulsow to Blood Stalk.", 5, 4);
            }

            if (preset == CustomComboPreset.RPR_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, RPR.Config.RPR_VariantCure, "HP% to be at or under", 200);


            #endregion
            // ====================================================================================
            #region RED MAGE

            if (preset is CustomComboPreset.RDM_ST_oGCD)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_OnAction_Adv, "Advanced Action Options.", "Changes which action this option will replace.", isConditionalChoice: true);
                if (RDM.Config.RDM_ST_oGCD_OnAction_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_oGCD_OnAction, "Jolts", "", 4, 0, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_oGCD_OnAction, "Fleche", "", 4, 1, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_oGCD_OnAction, "Riposte", "", 4, 2, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_oGCD_OnAction, "Reprise", "", 4, 3, descriptionColor: ImGuiColors.DalamudYellow);
                    ImGui.Unindent();
                }

                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_Fleche, "Fleche", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_ContraSixte, "Contra Sixte", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                if (RDM.Config.RDM_ST_oGCD_Engagement)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                    ImGui.Unindent();
                }
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                if (RDM.Config.RDM_ST_oGCD_CorpACorps)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                    ImGui.Unindent();
                }
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_ViceOfThorns, "Vice of Thorns", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_oGCD_Prefulgence, "Prefulgence", "");
            }

            if (preset is CustomComboPreset.RDM_ST_MeleeCombo)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                if (RDM.Config.RDM_ST_MeleeCombo_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_MeleeCombo_OnAction, "Jolts", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_MeleeCombo_OnAction, "Riposte", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.RDM_ST_MeleeFinisher)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                if (RDM.Config.RDM_ST_MeleeFinisher_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_MeleeFinisher_OnAction, "Jolts", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_MeleeFinisher_OnAction, "Riposte", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_ST_MeleeFinisher_OnAction, "VerAero & VerThunder", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.RDM_ST_Lucid)
                UserConfig.DrawSliderInt(0, 10000, RDM.Config.RDM_ST_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

            // AoE
            if (preset is CustomComboPreset.RDM_AoE_oGCD)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_Fleche, "Fleche", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_ContraSixte, "Contra Sixte", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                if (RDM.Config.RDM_AoE_oGCD_Engagement)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                    ImGui.Unindent();
                }
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                if (RDM.Config.RDM_AoE_oGCD_CorpACorps)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                    ImGui.Unindent();
                }
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_ViceOfThorns, "Vice of Thorns", "");
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_oGCD_Prefulgence, "Prefulgence", "");
            }

            if (preset is CustomComboPreset.RDM_AoE_MeleeCombo)
            {
                UserConfig.DrawSliderInt(3, 8, RDM.Config.RDM_AoE_MoulinetRange, "Range to use first Moulinet; no range restrictions after first Moulinet", sliderIncrement: SliderIncrements.Ones);
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                if (RDM.Config.RDM_AoE_MeleeCombo_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_AoE_MeleeCombo_OnAction, "Scatter/Impact", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_AoE_MeleeCombo_OnAction, "Moulinet", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.RDM_AoE_MeleeFinisher)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_AoE_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                if (RDM.Config.RDM_AoE_MeleeFinisher_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_AoE_MeleeFinisher_OnAction, "Scatter/Impact", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_AoE_MeleeFinisher_OnAction, "Moulinet", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                    UserConfig.DrawHorizontalMultiChoice(RDM.Config.RDM_AoE_MeleeFinisher_OnAction, "VerAero II & VerThunder II", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.RDM_AoE_Lucid)
                UserConfig.DrawSliderInt(0, 10000, RDM.Config.RDM_AoE_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.RDM_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, RDM.Config.RDM_VariantCure, "HP% to be at or under", 200);

            if (preset is CustomComboPreset.RDM_ST_MeleeCombo)
            {
                UserConfig.DrawAdditionalBoolChoice(RDM.Config.RDM_ST_MeleeEnforced, "Enforced Melee Check", "Once the melee combo has started, don't switch away even if target is out of range.");
            }

            #endregion
            // ====================================================================================
            #region SAGE

            if (preset is CustomComboPreset.SGE_ST_DPS)
                UserConfig.DrawAdditionalBoolChoice(SGE.Config.SGE_ST_DPS_Adv, $"Apply all selected options to {SGE.Dosis2.ActionName()}", $"{SGE.Dosis.ActionName()} & {SGE.Dosis3.ActionName()} will behave normally.");

            if (preset is CustomComboPreset.SGE_ST_DPS_EDosis)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_DPS_EDosisHPPer, "Stop using at Enemy HP %. Set to Zero to disable this check");

                UserConfig.DrawAdditionalBoolChoice(SGE.Config.SGE_ST_DPS_EDosis_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (SGE.Config.SGE_ST_DPS_EDosis_Adv)
                {
                    ImGui.Indent();
                    UserConfig.DrawRoundedSliderFloat(0, 4, SGE.Config.SGE_ST_DPS_EDosisThreshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SGE_ST_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SGE.Config.SGE_ST_DPS_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);


            if (preset is CustomComboPreset.SGE_ST_DPS_Rhizo)
                UserConfig.DrawSliderInt(0, 1, SGE.Config.SGE_ST_DPS_Rhizo, "Addersgall Threshold", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.SGE_ST_DPS_AddersgallProtect)
                UserConfig.DrawSliderInt(1, 3, SGE.Config.SGE_ST_DPS_AddersgallProtect, "Addersgall Threshold", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.SGE_ST_DPS_Movement)
            {
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_DPS_Movement, SGE.Toxikon.ActionName(), $"Use {SGE.Toxikon.ActionName()} when Addersting is available.", 4, 0);
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_DPS_Movement, SGE.Dyskrasia.ActionName(), $"Use {SGE.Dyskrasia.ActionName()} when in range of a selected enemy target.", 4, 1);
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_DPS_Movement, SGE.Eukrasia.ActionName(), $"Use {SGE.Eukrasia.ActionName()}.", 4, 2);
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_DPS_Movement, SGE.Psyche.ActionName(), $"Use {SGE.Psyche.ActionName()}.", 4, 3);
            }

            if (preset is CustomComboPreset.SGE_AoE_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SGE.Config.SGE_AoE_DPS_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_AoE_DPS_Rhizo)
                UserConfig.DrawSliderInt(0, 1, SGE.Config.SGE_AoE_DPS_Rhizo, "Addersgall Threshold", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.SGE_AoE_DPS_AddersgallProtect)
                UserConfig.DrawSliderInt(1, 3, SGE.Config.SGE_AoE_DPS_AddersgallProtect, "Addersgall Threshold", 150, SliderIncrements.Ones);

            if (preset is CustomComboPreset.SGE_ST_Heal)
            {
                UserConfig.DrawAdditionalBoolChoice(SGE.Config.SGE_ST_Heal_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (SGE.Config.SGE_ST_Heal_Adv)
                {
                    ImGui.Indent();
                    UserConfig.DrawAdditionalBoolChoice(SGE.Config.SGE_ST_Heal_UIMouseOver,
                        "Party UI Mouseover Checking",
                        "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                        "To be used in conjunction with Redirect/Reaction/etc");
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Esuna)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Esuna, "Stop using when below HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SGE_ST_Heal_Soteria)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Soteria, $"Use {SGE.Soteria.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 0, $"{SGE.Soteria.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Zoe)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Zoe, $"Use {SGE.Zoe.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 1, $"{SGE.Zoe.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Pepsis)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Pepsis, $"Use {SGE.Pepsis.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 2, $"{SGE.Pepsis.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Taurochole)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Taurochole, $"Use {SGE.Taurochole.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 3, $"{SGE.Taurochole.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Haima)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Haima, $"Use {SGE.Haima.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 4, $"{SGE.Haima.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Krasis)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Krasis, $"Use {SGE.Krasis.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 5, $"{SGE.Krasis.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_Druochole)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Druochole, $"Use {SGE.Druochole.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawPriorityInput(SGE.Config.SGE_ST_Heals_Priority, 7, 6, $"{SGE.Druochole.ActionName()} Priority: ");
            }

            if (preset is CustomComboPreset.SGE_ST_Heal_EDiagnosis)
            {
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_EDiagnosisHP, $"Use {SGE.EukrasianDiagnosis.ActionName()} when Target HP is at or below set percentage");
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_Heal_EDiagnosisOpts, "Ignore Shield Check", $"Warning, will force the use of {SGE.EukrasianDiagnosis.ActionName()}, and normal {SGE.Diagnosis.ActionName()} will be unavailable.", 2, 0);
                UserConfig.DrawHorizontalMultiChoice(SGE.Config.SGE_ST_Heal_EDiagnosisOpts, "Check for Scholar Galvenize", "Enable to not override an existing Scholar's shield.", 2, 1);
            }

            if (preset is CustomComboPreset.SGE_AoE_Heal_Kerachole)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 0, $"{SGE.Kerachole.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Ixochole)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 1, $"{SGE.Ixochole.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Physis)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 2, $"{SGE.Physis.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Holos)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 3, $"{SGE.Holos.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Panhaima)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 4, $"{SGE.Panhaima.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Pepsis)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 5, $"{SGE.Pepsis.ActionName()} Priority: ");

            if (preset is CustomComboPreset.SGE_AoE_Heal_Philosophia)
                UserConfig.DrawPriorityInput(SGE.Config.SGE_AoE_Heals_Priority, 7, 6, $"{SGE.Philosophia.ActionName()} Priority: ");


            if (preset is CustomComboPreset.SGE_AoE_Heal_Kerachole)
                UserConfig.DrawAdditionalBoolChoice(SGE.Config.SGE_AoE_Heal_KeracholeTrait,
                    "Check for Enhanced Kerachole Trait (Heal over Time)",
                    $"Enabling this will prevent {SGE.Kerachole.ActionName()} from being used when the Heal over Time trait is unavailable.");

            if (preset is CustomComboPreset.SGE_Eukrasia)
            {
                UserConfig.DrawRadioButton(SGE.Config.SGE_Eukrasia_Mode, $"{SGE.EukrasianDosis.ActionName()}", "", 0);
                UserConfig.DrawRadioButton(SGE.Config.SGE_Eukrasia_Mode, $"{SGE.EukrasianDiagnosis.ActionName()}", "", 1);
                UserConfig.DrawRadioButton(SGE.Config.SGE_Eukrasia_Mode, $"{SGE.EukrasianPrognosis.ActionName()}", "", 2);
                UserConfig.DrawRadioButton(SGE.Config.SGE_Eukrasia_Mode, $"{SGE.EukrasianDyskrasia.ActionName()}", "", 3);
            }

            #endregion
            // ====================================================================================
            #region SAMURAI

            if (preset == CustomComboPreset.SAM_ST_Overcap && enabled)
                UserConfig.DrawSliderInt(0, 85, SAM.Config.SAM_ST_KenkiOvercapAmount, "Set the Kenki overcap amount for ST combos.");

            if (preset == CustomComboPreset.SAM_AoE_Overcap && enabled)
                UserConfig.DrawSliderInt(0, 85, SAM.Config.SAM_AoE_KenkiOvercapAmount, "Set the Kenki overcap amount for AOE combos.");

            if (preset == CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_MeikyoChoice, "Use after Hakaze/Sen Applier", "Uses Meikyo Shisui after Hakaze, Gekko, Yukikaze, or Kasha.", 1);
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_MeikyoChoice, "Use outside of combo chain", "Uses Meikyo Shisui outside of a combo chain.", 2);
            }

            //PvP
            if (preset == CustomComboPreset.SAMPvP_BurstMode && enabled)
                UserConfig.DrawSliderInt(0, 2, SAMPvP.Config.SAMPvP_SotenCharges, "How many charges of Soten to keep ready? (0 = Use All).");

            if (preset == CustomComboPreset.SAMPvP_KashaFeatures_GapCloser && enabled)
                UserConfig.DrawSliderInt(0, 100, SAMPvP.Config.SAMPvP_SotenHP, "Use Soten on enemies below selected HP.");

            //Fillers
            if (preset == CustomComboPreset.SAM_ST_GekkoCombo_FillerCombos)
            {
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "2.14+", "2 Filler GCDs", 1);
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "2.06 - 2.08", "3 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range.\nHakaze will be delayed by half a GCD after Enpi.", 2);
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "1.99 - 2.01", "4 Filler GCDs. \nUses double Yukikaze loop.", 3);
            }

            if (preset == CustomComboPreset.SAM_ST_GekkoCombo_CDs_Iaijutsu)
            {
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_ST_Higanbana_Threshold, "Stop using Higanbana on targets below this HP % (0% = always use).", 150, SliderIncrements.Ones);
            }
            if (preset == CustomComboPreset.SAM_ST_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_STSecondWindThreshold, "HP percent threshold to use Second Wind below (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_STBloodbathThreshold, "HP percent threshold to use Bloodbath (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.SAM_AoE_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_AoESecondWindThreshold, "HP percent threshold to use Second Wind below (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_AoEBloodbathThreshold, "HP percent threshold to use Bloodbath below (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.SAM_ST_Execute)
            {
                UserConfig.DrawSliderInt(0, 100, SAM.Config.SAM_ST_ExecuteThreshold, "HP percent threshold to use Shinten below", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.SAM_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, SAM.Config.SAM_VariantCure, "HP% to be at or under", 200);

            #endregion
            // ====================================================================================
            #region SCHOLAR

            if (preset is CustomComboPreset.SCH_DPS)
            {
                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_ST_DPS_Adv, "Advanced Action Options", "Change how actions are handled", isConditionalChoice: true);

                if (SCH.Config.SCH_ST_DPS_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(SCH.Config.SCH_ST_DPS_Adv_Actions, "On Ruin/Broils", "Apply options to Ruin and all Broils.", 3, 0);
                    UserConfig.DrawHorizontalMultiChoice(SCH.Config.SCH_ST_DPS_Adv_Actions, "On Bio/Bio II/Biolysis", "Apply options to Bio and Biolysis.", 3, 1);
                    UserConfig.DrawHorizontalMultiChoice(SCH.Config.SCH_ST_DPS_Adv_Actions, "On Ruin II", "Apply options to Ruin II.", 3, 2);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SCH.Config.SCH_ST_DPS_LucidOption, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_DPS_Bio)
            {
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_BioOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");

                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_ST_DPS_Bio_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (PluginConfiguration.GetCustomBoolValue(SCH.Config.SCH_ST_DPS_Bio_Adv))
                {
                    ImGui.Indent();
                    UserConfig.DrawRoundedSliderFloat(0, 4, SCH.Config.SCH_ST_DPS_Bio_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_DPS_ChainStrat)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_ChainStratagemOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");

            if (preset is CustomComboPreset.SCH_DPS_EnergyDrain)
            {
                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_ST_DPS_EnergyDrain_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (SCH.Config.SCH_ST_DPS_EnergyDrain_Adv)
                {
                    ImGui.Indent();
                    UserConfig.DrawRoundedSliderFloat(0, 60, SCH.Config.SCH_ST_DPS_EnergyDrain, "Aetherflow remaining cooldown:", digits: 1);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_AoE_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SCH.Config.SCH_AoE_LucidOption, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_ST_Heal)
            {
                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_ST_Heal_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (SCH.Config.SCH_ST_Heal_Adv)
                {
                    ImGui.Indent();
                    UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_ST_Heal_UIMouseOver,
                        "Party UI Mouseover Checking",
                        "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                        "To be used in conjunction with Redirect/Reaction/etc");
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_ST_Heal_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SCH.Config.SCH_ST_Heal_LucidOption, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_ST_Heal_Adloquium)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_Heal_AdloquiumOption, "Use Adloquium on targets at or below HP % even if they have Galvanize\n0 = Only ever use Adloquium on targets without Galvanize\n100 = Always use Adloquium");

            if (preset is CustomComboPreset.SCH_ST_Heal_Lustrate)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_Heal_LustrateOption, "Start using when below HP %. Set to 100 to disable this check");

            if (preset is CustomComboPreset.SCH_ST_Heal_Esuna)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_Heal_EsunaOption, "Stop using when below HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SCH_DeploymentTactics)
            {
                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_DeploymentTactics_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (SCH.Config.SCH_DeploymentTactics_Adv)
                {
                    ImGui.Indent();
                    UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_DeploymentTactics_UIMouseOver,
                        "Party UI Mouseover Checking",
                        "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                        "To be used in conjunction with Redirect/Reaction/etc");
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_Aetherflow)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On Energy Drain Only", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On All Aetherflow Skills", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow_Recite)
            {
                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_Aetherflow_Recite_Excog, "On Excogitation", "", isConditionalChoice: true);
                if (SCH.Config.SCH_Aetherflow_Recite_Excog)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_ExcogMode, "Only when out of Aetherflow Stacks", "", 0);
                    UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_ExcogMode, "Always when available", "", 1);
                    ImGui.Unindent();
                }

                UserConfig.DrawAdditionalBoolChoice(SCH.Config.SCH_Aetherflow_Recite_Indom, "On Indominability", "", isConditionalChoice: true);
                if (SCH.Config.SCH_Aetherflow_Recite_Indom)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_IndomMode, "Only when out of Aetherflow Stacks", "", 0);
                    UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_IndomMode, "Always when available", "", 1);
                    ImGui.Unindent();
                }
            }

            if (preset is CustomComboPreset.SCH_Recitation)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_Recitation_Mode, "Adloquium", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Recitation_Mode, "Succor", "", 1);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Recitation_Mode, "Indomitability", "", 2);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Recitation_Mode, "Excogitation", "", 3);
            }

            #endregion
            // ====================================================================================
            #region SUMMONER

            #region PvE
            if (preset == CustomComboPreset.SMN_DemiEgiMenu_EgiOrder)
            {
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_PrimalChoice, "Titan first", "Summons Titan, Garuda then Ifrit.", 1);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_PrimalChoice, "Garuda first", "Summons Garuda, Titan then Ifrit.", 2);
            }

            if (preset == CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling)
                UserConfig.DrawSliderInt(0, 3, SMN.Config.SMN_Burst_Delay, "Sets the amount of GCDs under Demi summon to wait for oGCD use.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling)
            {
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Solar Bahamut/Bahamut", "Bursts during Bahamut phase.\nBahamut burst phase becomes Solar Bahamut at Lv100.", 1);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Phoenix", "Bursts during Phoenix phase.", 2);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Any Demi Phase", "Bursts during any Demi Summon phase.", 3);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Flexible (SpS) Option", "Bursts when Searing Light is ready, regardless of phase.", 4);
            }

            if (preset == CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi)
            {
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite", 2);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "Flexible (SpS) Option", "Swiftcasts the first available Egi when Swiftcast is ready.", 3);
            }

            if (preset == CustomComboPreset.SMN_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SMN.Config.SMN_Lucid, "Set value for your MP to be at or under for this feature to take effect.", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.SMN_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, SMN.Config.SMN_VariantCure, "HP% to be at or under", 200);

            if (preset == CustomComboPreset.SMN_ST_Egi_AstralFlow)
            {
                UserConfig.DrawHorizontalMultiChoice(SMN.Config.SMN_ST_Egi_AstralFlow, "Add Mountain Buster", "", 3, 0);
                UserConfig.DrawHorizontalMultiChoice(SMN.Config.SMN_ST_Egi_AstralFlow, "Add Crimson Cyclone", "", 3, 1);
                UserConfig.DrawHorizontalMultiChoice(SMN.Config.SMN_ST_Egi_AstralFlow, "Add Slipstream", "", 3, 2);

                if (SMN.Config.SMN_ST_Egi_AstralFlow[1])
                    UserConfig.DrawAdditionalBoolChoice(SMN.Config.SMN_ST_CrimsonCycloneMelee, "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");
            }



            #endregion

            #region PvP

            if (preset == CustomComboPreset.SMNPvP_BurstMode)
                UserConfig.DrawSliderInt(50, 100, SMNPvP.Config.SMNPvP_FesterThreshold, "Target HP% to cast Fester below.\nSet to 100 use Fester as soon as it's available.", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.SMNPvP_BurstMode_RadiantAegis)
                UserConfig.DrawSliderInt(0, 90, SMNPvP.Config.SMNPvP_RadiantAegisThreshold, "Caps at 90 to prevent waste.", 150, SliderIncrements.Ones);

            #endregion

            #endregion
            // ====================================================================================
            #region VIPER

            if ((preset == CustomComboPreset.VPR_ST_AdvancedMode && enabled) || (preset == CustomComboPreset.VPR_VicewinderCoils && enabled))
            {
                UserConfig.DrawHorizontalRadioButton(VPR.Config.VPR_Positional, "Rear First", "First positional: Swiftskin's Coil.", 0);
                UserConfig.DrawHorizontalRadioButton(VPR.Config.VPR_Positional, "Flank First", "First positional: Hunter's Coil.", 1);
            }

            if (preset == CustomComboPreset.VPR_ST_UncoiledFury && enabled)
            {
                UserConfig.DrawSliderInt(0, 3, VPR.Config.VPR_ST_UncoiledFury_HoldCharges, "How many charges to keep ready? (0 = Use all)");
                UserConfig.DrawSliderInt(0, 5, VPR.Config.VPR_ST_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");
            }

            if (preset == CustomComboPreset.VPR_AoE_UncoiledFury && enabled)
            {
                UserConfig.DrawSliderInt(0, 3, VPR.Config.VPR_AoE_UncoiledFury_HoldCharges, "How many charges to keep ready? (0 = Use all)");
                UserConfig.DrawSliderInt(0, 5, VPR.Config.VPR_AoE_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");
            }


            if (preset is CustomComboPreset.VPR_ST_Reawaken)
            {
                UserConfig.DrawRoundedSliderFloat(0, 10, VPR.Config.VPR_ST_Reawaken_Usage, "Stop using at Enemy HP %. Set to Zero to disable this check.", digits: 1);
            }

            if (preset is CustomComboPreset.VPR_AoE_Reawaken)
            {
                UserConfig.DrawRoundedSliderFloat(0, 10, VPR.Config.VPR_AoE_Reawaken_Usage, "Stop using at Enemy HP %. Set to Zero to disable this check.", digits: 1);
            }

            if (preset == CustomComboPreset.VPR_ST_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, VPR.Config.VPR_ST_SecondWind_Threshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, VPR.Config.VPR_ST_Bloodbath_Threshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.VPR_AoE_ComboHeals)
            {
                UserConfig.DrawSliderInt(0, 100, VPR.Config.VPR_AoE_SecondWind_Threshold, "Second Wind HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 100, VPR.Config.VPR_AoE_Bloodbath_Threshold, "Bloodbath HP percentage threshold (0 = Disabled)", 150, SliderIncrements.Ones);
            }

            if (preset == CustomComboPreset.VPR_ReawakenLegacy && enabled)
            {
                UserConfig.DrawRadioButton(VPR.Config.VPR_ReawakenLegacyButton, "Replaces Reawaken", "Replaces Reawaken with Full Generation - Legacy combo.", 0);
                UserConfig.DrawRadioButton(VPR.Config.VPR_ReawakenLegacyButton, "Replaces Steel Fangs", "Replaces Steel Fangs with Full Generation - Legacy combo.", 1);
            }

            #endregion
            // ====================================================================================
            #region WARRIOR

            if (preset == CustomComboPreset.WAR_ST_StormsPath_StormsEye && enabled)
                UserConfig.DrawSliderInt(0, 30, WAR.Config.WAR_SurgingRefreshRange, "Seconds remaining before refreshing Surging Tempest.");

            if (preset == CustomComboPreset.WAR_InfuriateFellCleave && enabled)
                UserConfig.DrawSliderInt(0, 50, WAR.Config.WAR_InfuriateRange, "Set how much rage to be at or under to use this feature.");

            if (preset == CustomComboPreset.WAR_ST_StormsPath_Onslaught && enabled)
                UserConfig.DrawSliderInt(0, 2, WAR.Config.WAR_KeepOnslaughtCharges, "How many charges to keep ready? (0 = Use All)");

            if (preset == CustomComboPreset.WAR_ST_StormsPath_Infuriate && enabled)
                UserConfig.DrawSliderInt(0, 2, WAR.Config.WAR_KeepInfuriateCharges, "How many charges to keep ready? (0 = Use All)");

            if (preset == CustomComboPreset.WAR_Variant_Cure)
                UserConfig.DrawSliderInt(1, 100, WAR.Config.WAR_VariantCure, "HP% to be at or under", 200);

            if (preset == CustomComboPreset.WAR_ST_StormsPath_FellCleave)
                UserConfig.DrawSliderInt(50, 100, WAR.Config.WAR_FellCleaveGauge, "Minimum gauge to spend");

            if (preset == CustomComboPreset.WAR_AoE_Overpower_Decimate)
                UserConfig.DrawSliderInt(50, 100, WAR.Config.WAR_DecimateGauge, "Minimum gauge to spend");

            if (preset == CustomComboPreset.WAR_ST_StormsPath_Infuriate)
                UserConfig.DrawSliderInt(0, 50, WAR.Config.WAR_InfuriateSTGauge, "Use when gauge is under or equal to");

            if (preset == CustomComboPreset.WAR_AoE_Overpower_Infuriate)
                UserConfig.DrawSliderInt(0, 50, WAR.Config.WAR_InfuriateAoEGauge, "Use when gauge is under or equal to");

            if (preset == CustomComboPreset.WARPvP_BurstMode_Blota)
            {
                UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, "Before Primal Rend", "", 0);
                UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, "After Primal Rend", "", 1);
            }

            #endregion
            // ====================================================================================
            #region WHITE MAGE

            if (preset is CustomComboPreset.WHM_ST_MainCombo)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_ST_MainCombo_Adv, "Advanced Action Options", "Change how actions are handled", isConditionalChoice: true);

                if (WHM.Config.WHM_ST_MainCombo_Adv)
                {
                    ImGui.Indent(); ImGui.Spacing();
                    UserConfig.DrawHorizontalMultiChoice(WHM.Config.WHM_ST_MainCombo_Adv_Actions, "On Stone/Glare", "Apply options to all Stones and Glares.", 3, 0);
                    UserConfig.DrawHorizontalMultiChoice(WHM.Config.WHM_ST_MainCombo_Adv_Actions, "On Aero/Dia", "Apply options to Aeros and Dia.", 3, 1);
                    UserConfig.DrawHorizontalMultiChoice(WHM.Config.WHM_ST_MainCombo_Adv_Actions, "On Stone II", "Apply options to Stone II.", 3, 2);
                    ImGui.Unindent();
                }
            }

            if (preset == CustomComboPreset.WHM_ST_MainCombo_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_STDPS_Lucid, "Set value for your MP to be at or under for this feature to work.", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.WHM_ST_MainCombo_DoT)
            {
                UserConfig.DrawSliderInt(0, 100, WHM.Config.WHM_STDPS_MainCombo_DoT, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_ST_MainCombo_DoT_Adv, "Advanced Options", "", isConditionalChoice: true);
                if (PluginConfiguration.GetCustomBoolValue(WHM.Config.WHM_ST_MainCombo_DoT_Adv))
                {
                    ImGui.Indent();
                    UserConfig.DrawRoundedSliderFloat(0, 4, WHM.Config.WHM_ST_MainCombo_DoT_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                    ImGui.Unindent();
                }
            }

            if (preset == CustomComboPreset.WHM_AoE_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_AoEDPS_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.WHM_AoEHeals_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_AoEHeals_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.WHM_STHeals_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_STHeals_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.WHM_STHeals_Esuna)
                UserConfig.DrawSliderInt(0, 100, WHM.Config.WHM_STHeals_Esuna, "Stop using when below HP %. Set to Zero to disable this check");

            if (preset == CustomComboPreset.WHM_AoEHeals_ThinAir)
                UserConfig.DrawSliderInt(0, 1, WHM.Config.WHM_AoEHeals_ThinAir, "How many charges to keep ready? (0 = Use all)");

            if (preset == CustomComboPreset.WHM_AoEHeals_Cure3)
                UserConfig.DrawSliderInt(1500, 8500, WHM.Config.WHM_AoEHeals_Cure3MP, "Use when MP is above", sliderIncrement: 500);

            if (preset == CustomComboPreset.WHM_STHeals)
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_STHeals_UIMouseOver, "Party UI Mouseover Checking", "Check party member's HP & Debuffs by using mouseover on the party list.\nTo be used in conjunction with Redirect/Reaction/etc.");

            if (preset == CustomComboPreset.WHM_STHeals_ThinAir)
                UserConfig.DrawSliderInt(0, 1, WHM.Config.WHM_STHeals_ThinAir, "How many charges to keep ready? (0 = Use all)");

            if (preset == CustomComboPreset.WHM_STHeals_Regen)
                UserConfig.DrawRoundedSliderFloat(0f, 6f, WHM.Config.WHM_STHeals_RegenTimer, "Time Remaining Before Refreshing");

            if (preset == CustomComboPreset.WHM_ST_MainCombo_Opener)
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_ST_MainCombo_Opener_Swiftcast, "Swiftcast Option", "Adds Swiftcast to the opener.");

            if (preset == CustomComboPreset.WHM_STHeals_Benediction)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_STHeals_BenedictionWeave, "Only Weave", "");
                UserConfig.DrawSliderInt(1, 100, WHM.Config.WHM_STHeals_BenedictionHP, "Use when target HP% is at or below.");
            }

            if (preset == CustomComboPreset.WHM_STHeals_Tetragrammaton)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_STHeals_TetraWeave, "Only Weave", "");
                UserConfig.DrawSliderInt(1, 100, WHM.Config.WHM_STHeals_TetraHP, "Use when target HP% is at or below.");
            }

            if (preset == CustomComboPreset.WHM_STHeals_Benison)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_STHeals_BenisonWeave, "Only Weave", "");
                UserConfig.DrawSliderInt(1, 100, WHM.Config.WHM_STHeals_BenisonHP, "Use when target HP% is at or below.");
            }

            if (preset == CustomComboPreset.WHM_STHeals_Aquaveil)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_STHeals_AquaveilWeave, "Only Weave", "");
                UserConfig.DrawSliderInt(1, 100, WHM.Config.WHM_STHeals_AquaveilHP, "Use when target HP% is at or below.");
            }

            if (preset == CustomComboPreset.WHM_AoEHeals_Assize)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_AoEHeals_AssizeWeave, "Only Weave", "");
            }

            if (preset == CustomComboPreset.WHM_AoEHeals_Plenary)
            {
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_AoEHeals_PlenaryWeave, "Only Weave", "");
            }

            if (preset == CustomComboPreset.WHM_AoEHeals_Medica2)
            {
                UserConfig.DrawRoundedSliderFloat(0f, 6f, WHM.Config.WHM_AoEHeals_MedicaTime, "Time Remaining on Buff to Renew");
                UserConfig.DrawAdditionalBoolChoice(WHM.Config.WHM_AoEHeals_MedicaMO, "Party UI Mousover Checking", "Check your mouseover target for the Medica II/III buff.\nTo be used in conjunction with Redirect/Reaction/etc.");
            }

            #endregion
            // ====================================================================================
            #region DOH

            #endregion
            // ====================================================================================
            #region DOL

            #endregion
            // ====================================================================================
            #region PvP VALUES

            IPlayerCharacter? pc = Service.ClientState.LocalPlayer;

            if (preset == CustomComboPreset.PvP_EmergencyHeals)
            {
                if (pc != null)
                {
                    uint maxHP = Service.ClientState.LocalPlayer?.MaxHp <= 15000 ? 0 : Service.ClientState.LocalPlayer.MaxHp - 15000;

                    if (maxHP > 0)
                    {
                        int setting = PluginConfiguration.GetCustomIntValue(PvPCommon.Config.EmergencyHealThreshold);
                        float hpThreshold = (float)maxHP / 100 * setting;

                        UserConfig.DrawSliderInt(1, 100, PvPCommon.Config.EmergencyHealThreshold, $"Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.\nHP Value to be at or under: {hpThreshold}");
                    }

                    else
                    {
                        UserConfig.DrawSliderInt(1, 100, PvPCommon.Config.EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                    }
                }

                else
                {
                    UserConfig.DrawSliderInt(1, 100, PvPCommon.Config.EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                }
            }

            if (preset == CustomComboPreset.PvP_EmergencyGuard)
                UserConfig.DrawSliderInt(1, 100, PvPCommon.Config.EmergencyGuardThreshold, "Set the percentage to be at or under for the feature to kick in.");

            if (preset == CustomComboPreset.PvP_QuickPurify)
                UserConfig.DrawPvPStatusMultiChoice(PvPCommon.Config.QuickPurifyStatuses);

            if (preset == CustomComboPreset.NINPvP_ST_Meisui)
            {
                string description = "Set the HP percentage to be at or under for the feature to kick in.\n100% is considered to start at 8,000 less than your max HP to prevent wastage.";

                if (pc != null)
                {
                    uint maxHP = pc.MaxHp <= 8000 ? 0 : pc.MaxHp - 8000;
                    if (maxHP > 0)
                    {
                        int setting = PluginConfiguration.GetCustomIntValue(NINPvP.Config.NINPvP_Meisui_ST);
                        float hpThreshold = (float)maxHP / 100 * setting;

                        description += $"\nHP Value to be at or under: {hpThreshold}";
                    }
                }

                UserConfig.DrawSliderInt(1, 100, NINPvP.Config.NINPvP_Meisui_ST, description);
            }

            if (preset == CustomComboPreset.NINPvP_AoE_Meisui)
            {
                string description = "Set the HP percentage to be at or under for the feature to kick in.\n100% is considered to start at 8,000 less than your max HP to prevent wastage.";

                if (pc != null)
                {
                    uint maxHP = pc.MaxHp <= 8000 ? 0 : pc.MaxHp - 8000;
                    if (maxHP > 0)
                    {
                        int setting = PluginConfiguration.GetCustomIntValue(NINPvP.Config.NINPvP_Meisui_AoE);
                        float hpThreshold = (float)maxHP / 100 * setting;

                        description += $"\nHP Value to be at or under: {hpThreshold}";
                    }
                }

                UserConfig.DrawSliderInt(1, 100, NINPvP.Config.NINPvP_Meisui_AoE, description);
            }


            #endregion
        }
    }

    public static class SliderIncrements
    {
        public const uint
            Ones = 1,
            Fives = 5,
            Tens = 10,
            Hundreds = 100,
            Thousands = 1000;
    }
}
