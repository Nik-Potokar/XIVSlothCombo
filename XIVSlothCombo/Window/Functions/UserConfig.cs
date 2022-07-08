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
        public static void DrawSliderInt(int minValue, int maxValue, string config, string sliderDescription, float itemWidth = 150, uint sliderIncrement = SliderIncrements.Ones)
        {
            var output = PluginConfiguration.GetCustomIntValue(config, minValue);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            inputChanged |= ImGui.SliderInt($"{sliderDescription}###{config}", ref output, minValue, maxValue);

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

            ImGui.Spacing();
        }

        /// <summary> Draws a slider that lets the user set a given value for their feature. </summary>
        /// <param name="minValue"> The absolute minimum value you'll let the user pick. </param>
        /// <param name="maxValue"> The absolute maximum value you'll let the user pick. </param>
        /// <param name="config"> The config ID. </param>
        /// <param name="sliderDescription"> Description of the slider. Appends to the right of the slider. </param>
        /// <param name="itemWidth"> How long the slider should be. </param>
        public static void DrawSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = PluginConfiguration.GetCustomFloatValue(config, minValue);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            inputChanged |= ImGui.SliderFloat($"{sliderDescription}###{config}", ref output, minValue, maxValue);

            if (inputChanged)
            {
                PluginConfiguration.SetCustomFloatValue(config, output);
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        /// <summary> Draws a slider that lets the user set a given value for their feature. </summary>
        /// <param name="minValue"> The absolute minimum value you'll let the user pick. </param>
        /// <param name="maxValue"> The absolute maximum value you'll let the user pick. </param>
        /// <param name="config"> The config ID. </param>
        /// <param name="sliderDescription"> Description of the slider. Appends to the right of the slider. </param>
        /// <param name="itemWidth"> How long the slider should be. </param>
        public static void DrawRoundedSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = PluginConfiguration.GetCustomFloatValue(config, minValue);
            var inputChanged = false;
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            inputChanged |= ImGui.SliderFloat($"{sliderDescription}###{config}", ref output, minValue, maxValue, "%.1f");

            if (inputChanged)
            {
                PluginConfiguration.SetCustomFloatValue(config, output);
                Service.Configuration.Save();
            }

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
            var output = PluginConfiguration.GetCustomIntValue(config, outputValue);
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            var enabled = output == outputValue;

            if (ImGui.Checkbox($"{checkBoxName}###{config}{outputValue}", ref enabled))
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
            ImGui.Indent();
            if (descriptionColor == new Vector4()) descriptionColor = ImGuiColors.DalamudYellow;
            var output = PluginConfiguration.GetCustomIntValue(config);
            ImGui.PushItemWidth(itemWidth);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(21, 0));
            ImGui.SameLine();
            var enabled = output == outputValue;

            ImGui.PushStyleColor(ImGuiCol.Text, descriptionColor);
            if (ImGui.Checkbox($"{checkBoxName}###{config}{outputValue}", ref enabled))
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

            ImGui.Unindent();
        }

        public static void DrawPvPStatusMultiChoice(string config)
        {
            var values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(7, $"{config}", false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 7);

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
            var values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 5);

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
            var value = PluginConfiguration.GetCustomIntValue(config);
            bool[] values = new bool[20];

            for (int i = 0; i <= 4; i++)
            {
                if (value == i) values[i] = true;
                else
                    values[i] = false;
            }

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 5);

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
            var values = PluginConfiguration.GetCustomBoolArrayValue(config);

            ImGui.Columns(5, $"{config}", false);

            if (values.Length == 0) Array.Resize<bool>(ref values, 20);

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
            var value = PluginConfiguration.GetCustomIntValue(config);
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
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_DPS_Divination)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_DPS_LightSpeed)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_DPS_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity)
                UserConfig.DrawSliderInt(0, 100, AST.Config.AST_EssentialDignity, "Set percentage value");

            #endregion
            // ====================================================================================
            #region BLACK MAGE

            if (preset == CustomComboPreset.BLM_AoE_Simple_Foul)
                UserConfig.DrawSliderInt(0, 2, BLM.Config.BLM_PolyglotsStored, "Number of Polyglot charges to store.\n(2 = Only use Polyglot with Manafont)");

            if (preset == CustomComboPreset.BLM_SimpleMode || preset == CustomComboPreset.BLM_Simple_Transpose)
                UserConfig.DrawRoundedSliderFloat(3.0f, 8.0f, BLM.Config.BLM_AstralFireRefresh, "Seconds before refreshing Astral Fire.\n(6s = Recommended)");

            if (preset == CustomComboPreset.BLM_Simple_CastMovement)
                UserConfig.DrawRoundedSliderFloat(0.0f, 1.0f, BLM.Config.BLM_MovementTime, "Seconds of movement before using the movement feature.");

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

            #endregion
            // ====================================================================================
            #region DANCER

            if (preset == CustomComboPreset.DNC_DanceComboReplacer)
            {
                var actions = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                var inputChanged = false;

                inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
                inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
                inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
                inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

                if (inputChanged)
                {
                    Service.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }

            if (preset == CustomComboPreset.DNC_ST_EspritOvercap)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_ST, "Esprit", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_EspritOvercap)
                UserConfig.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_AoE, "Esprit", 150, SliderIncrements.Ones);

            #region Simple ST Sliders

            if (preset == CustomComboPreset.DNC_ST_Simple_SS)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleSSBurstPercent, "Target HP percentage to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_TS)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleTSBurstPercent, "Target HP percentage to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_FeatherPooling)
                UserConfig.DrawSliderInt(0, 5, DNC.Config.DNCSimpleFeatherBurstPercent, "Target HP percentage to dump all pooled feathers below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWaltzPercent, "Curing Waltz HP percent", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWindPercent, "Second Wind HP percent", 200, SliderIncrements.Ones);

            #endregion

            #region Simple AoE Sliders

            if (preset == CustomComboPreset.DNC_AoE_Simple_SS)
                UserConfig.DrawSliderInt(0, 10, DNC.Config.DNCSimpleSSAoEBurstPercent, "Target HP percentage to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_TS)
                UserConfig.DrawSliderInt(0, 10, DNC.Config.DNCSimpleTSAoEBurstPercent, "Target HP percentage to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWaltzPercent, "Curing Waltz HP percent", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                UserConfig.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWindPercent, "Second Wind HP percent", 200, SliderIncrements.Ones);

            #endregion

            #region PvP Sliders

            if (preset == CustomComboPreset.DNCPvP_BurstMode_CuringWaltz)
                UserConfig.DrawSliderInt(0, 90, DNCPvP.Config.DNCPvP_WaltzThreshold, "Caps at 90 to prevent waste.###DNCPvP", 150, SliderIncrements.Ones);

            #endregion

            #endregion
            // ====================================================================================
            #region DARK KNIGHT

            if (preset == CustomComboPreset.DRK_EoSPooling && enabled)
                UserConfig.DrawSliderInt(0, 3000, DRK.Config.DRK_MPManagement, "How much MP to save (0 = Use All)", 150, SliderIncrements.Thousands);

            if (preset == CustomComboPreset.DRK_Plunge && enabled)
                UserConfig.DrawSliderInt(0, 1, DRK.Config.DRK_KeepPlungeCharges, "How many charges to keep ready? (0 = Use All)", 75, SliderIncrements.Ones);

            #endregion
            // ====================================================================================
            #region DRAGOON

            #endregion
            // ====================================================================================
            #region GUNBREAKER

            if (preset == CustomComboPreset.GNB_ST_RoughDivide && enabled)
                UserConfig.DrawSliderInt(0, 1, GNB.Config.GNB_RoughDivide_HeldCharges, "How many charges to keep ready? (0 = Use All)");

            #endregion
            // ====================================================================================
            #region MACHINIST

            #endregion
            // ====================================================================================
            #region MONK

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                UserConfig.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_Demolish_Apply, "Seconds remaining before refreshing Demolish.");

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                UserConfig.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_DisciplinedFist_Apply, "Seconds remaining before refreshing Disciplined Fist.");

            #endregion
            // ====================================================================================
            #region NINJA

            if (preset == CustomComboPreset.NIN_Simple_Mudras)
            {
                var mudrapath = Service.Configuration.MudraPathSelection;
                bool path1 = mudrapath == 1;
                bool path2 = mudrapath == 2;

                ImGui.Indent();
                ImGui.PushItemWidth(75);

                if (ImGui.Checkbox("Mudra Path Set 1", ref path1))
                {
                    Service.Configuration.MudraPathSelection = 1;
                    Service.Configuration.Save();
                }

                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                ImGui.TextWrapped($"1. Ten Mudras -> Fuma Shuriken, Raiton/Hyosho Ranryu, Suiton (Doton under Kassatsu).\nChi Mudras -> Fuma Shuriken, Hyoton, Huton.\nJin Mudras -> Fuma Shuriken, Katon/Goka Mekkyaku, Doton");
                ImGui.PopStyleColor();

                if (ImGui.Checkbox("Mudra Path Set 2", ref path2))
                {
                    Service.Configuration.MudraPathSelection = 2;
                    Service.Configuration.Save();
                }

                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                ImGui.TextWrapped($"2. Ten Mudras -> Fuma Shuriken, Hyoton/Hyosho Ranryu, Doton.\nChi Mudras -> Fuma Shuriken, Katon, Suiton.\nJin Mudras -> Fuma Shuriken, Raiton/Goka Mekkyaku, Huton (Doton under Kassatsu).");
                ImGui.PopStyleColor();

                ImGui.Unindent();
                ImGui.Spacing();
            }

            if (preset == CustomComboPreset.NIN_ST_Simple_Trick)
                UserConfig.DrawSliderInt(0, 15, NIN.Config.Trick_CooldownRemaining, "Set the amount of time in seconds for the feature to try and set up \nSuiton in advance of Trick Attack coming off cooldown");

            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_Huraijin)
                UserConfig.DrawSliderInt(0, 60, NIN.Config.Huton_RemainingTimer, "Set the amount of time remaining on Huton the feature\nshould wait before using Huraijin", 200);

            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_Mug)
                UserConfig.DrawSliderInt(0, 100, NIN.Config.Mug_NinkiGauge, $"Set the amount of Ninki to be at or under for this feature (level {NIN.TraitLevels.Shukiho} onwards)");

            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_ArmorCrush)
                UserConfig.DrawSliderInt(0, 30, NIN.Config.Huton_RemainingArmorCrush, "Set the amount of time remaining on Huton the feature\nshould wait before using Armor Crush", 200);

            if (preset == CustomComboPreset.NIN_NinkiPooling_Bhavacakra)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BhavaPooling, "The minimum value of Ninki to have before spending.");

            if (preset == CustomComboPreset.NIN_NinkiPooling_Bunshin)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPooling, "The minimum value of Ninki to have before spending.");

            #endregion
            // ====================================================================================
            #region PALADIN

            //if (preset == CustomComboPreset.PaladinAtonementDropFeature && enabled)
            //    ConfigWindowFunctions.DrawSliderInt(2, 3, PLD.Config.PLDAtonementCharges, "How many Atonements to cast right before FoF (Atonement Drop)?");

            if (preset == CustomComboPreset.PLD_ST_RoyalAuth_Intervene && enabled)
                UserConfig.DrawSliderInt(0, 1, PLD.Config.PLD_Intervene_HoldCharges, "How many charges to keep ready? (0 = Use all)");

            //if (preset == CustomComboPreset.SkillCooldownRemaining)
            //{
            //    var SkillCooldownRemaining = Service.Configuration.SkillCooldownRemaining;

            //    var inputChanged = false;
            //    ImGui.PushItemWidth(75);
            //    inputChanged |= ImGui.InputFloat("Input Skill Cooldown remaining Time", ref SkillCooldownRemaining);

            //    if (inputChanged)
            //    {
            //        Service.Configuration.SkillCooldownRemaining = SkillCooldownRemaining;

            //        Service.Configuration.Save();
            //    }

            //    ImGui.Spacing();
            //}

            #endregion
            // ====================================================================================
            #region REAPER

            if (preset == CustomComboPreset.RPRPvP_Burst_ImmortalPooling && enabled)
                UserConfig.DrawSliderInt(0, 8, RPRPVP.Config.RPRPvP_ImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.###RPR", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPRPvP_Burst_ArcaneCircle && enabled)
                UserConfig.DrawSliderInt(5, 90, RPRPVP.Config.RPRPvP_ArcaneCircleThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.###RPR", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.ReaperPositionalConfig && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear First", "First positional: Gallows (Rear), Void Reaping.", 1);
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Flank First", "First positional: Gibbet (Flank), Cross Reaping.", 2);
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear: Slice, Flank: SoD", "Rear positionals on Slice, Flank positionals on Shadow of Death.", 3);
                UserConfig.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear: SoD, Flank: Slice", "Rear positionals on Shadow of Death, Flank positionals on Slice.", 4);
            }

            if (preset == CustomComboPreset.RPR_ST_SliceCombo_SoD && enabled)
            {
                UserConfig.DrawSliderInt(0, 6, RPR.Config.RPR_SoDRefreshRange, "Seconds remaining before refreshing Death's Design.", 150, SliderIncrements.Ones);
                UserConfig.DrawSliderInt(0, 5, RPR.Config.RPR_SoDThreshold, "Set a HP% Threshold for when SoD will not be automatically applied to the target.", 150, SliderIncrements.Ones);
            }

            #endregion
            // ====================================================================================
            #region RED MAGE

            if (preset == CustomComboPreset.RDM_oGCD)
            {
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Fleche", "", 1);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Jolt\n-Jolt II", "Select for one button rotation", 2);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Scatter\n-Impact", "Select for one button rotation", 3);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 4);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Riposte\n-Moulinet", "", 5);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Fleche\n-Riposte\n-Moulinet", "", 6);
            }

            if (preset == CustomComboPreset.RDM_ST_MeleeCombo)
            {
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Riposte", "", 1);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Jolt\n-Jolt II", "Select for one button rotation", 2);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Riposte\n-Jolt\n-Jolt II", "Select for one button rotation", 3);
            }

            if (preset == CustomComboPreset.RDM_AoE_MeleeCombo)
            {
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_AoE_MeleeCombo_OnAction, "-Moulinet", "", 1);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_AoE_MeleeCombo_OnAction, "-Moulinet\n-Scatter\n-Impact", "Select for one button rotation", 2);
            }

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
            {
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Riposte\n-Moulinet", "", 1);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 2);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Riposte\n-Moulinet\n-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 3);
                UserConfig.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Veraero 1/2/3\n-Verthunder 1/2/3", "", 4);
            }

            if (preset == CustomComboPreset.RDM_Lucid && enabled)
                UserConfig.DrawSliderInt(0, 10000, RDM.Config.RDM_Lucid_Threshold, "Add Lucid Dreaming when below this MP", 300, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.RDM_AoE_MeleeCombo && enabled)
                UserConfig.DrawSliderInt(3, 8, RDM.Config.RDM_MoulinetRange, "Range to use first Moulinet; no range restrictions after first Moulinet", 150, SliderIncrements.Ones);

            #endregion
            // ====================================================================================
            #region SAGE

            if (preset is CustomComboPreset.SGE_ST_Dosis_EDosis)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Dosis_EDosisHPPer, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SGE_ST_Dosis_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SGE.Config.SGE_ST_Dosis_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Dosis_Toxikon)
            {
                UserConfig.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show when moving only", "", 0);
                UserConfig.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show at all times", "", 1);
            }

            if (preset is CustomComboPreset.SGE_AoE_Phlegma_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SGE.Config.SGE_AoE_Phlegma_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Heal_Soteria)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Soteria, "Use Soteria when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Zoe)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Zoe, "Use Zoe when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Pepsis)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Pepsis, "Use Pepsis when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Taurochole)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Taurochole, "Use Taurochole when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Haima)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Haima, "Use Haima when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Krasis)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Krasis, "Use Krasis when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Druochole)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Druochole, "Use Druochole when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Diagnosis)
                UserConfig.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Diagnosis, "Use Diagnosis when Target HP is at or below set percentage");

            #endregion
            // ====================================================================================
            #region SAMURAI

            if (preset == CustomComboPreset.SAM_ST_Overcap && enabled)
                UserConfig.DrawSliderInt(0, 85, SAM.Config.SAM_ST_KenkiOvercapAmount, "Set the Kenki overcap amount for ST combos.");

            if (preset == CustomComboPreset.SAM_AoE_Overcap && enabled)
                UserConfig.DrawSliderInt(0, 85, SAM.Config.SAM_AoE_KenkiOvercapAmount, "Set the Kenki overcap amount for AOE combos.");

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
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "1.99 - 2.01", "4 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range. \nHakaze will be delayed by half a GCD after Enpi.", 3);
            }

            #endregion
            // ====================================================================================
            #region SCHOLAR

            if (preset is CustomComboPreset.SCH_DPS)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_ST_DPS_AltMode, "On Ruin I / Broils", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_ST_DPS_AltMode, "On Bio", "Alternative DPS Mode. Leaves Ruin I / Broil alone for pure DPS, becomes Ruin I / Broil when features are on cooldown", 1);
            }

            if (preset is CustomComboPreset.SCH_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, SCH.Config.SCH_ST_DPS_LucidOption, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_DPS_Bio)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_BioOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SCH_DPS_ChainStrat)
                UserConfig.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_ChainStratagemOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SCH_FairyReminder)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Eos", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Selene", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On Energy Drain Only", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On All Aetherflow Skills", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Excog)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Only when out of Aetherflow Stacks", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Always when available", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Indom)
            {
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Only when out of Aetherflow Stacks", "", 0);
                UserConfig.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Always when available", "", 1);
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
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Bahamut", "Bursts during Bahamut phase.", 1);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Phoenix", "Bursts during Phoenix phase.", 2);
                UserConfig.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Bahamut or Phoenix", "Bursts during Bahamut or Phoenix phase (whichever comes first).", 3);
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
            #endregion

            #region PvP

            if (preset == CustomComboPreset.SMNPvP_BurstMode)
                UserConfig.DrawSliderInt(50, 100, SMNPvP.Config.SMNPvP_FesterThreshold, "Target HP% to cast Fester below.\nSet to 100 use Fester as soon as it's available.###SMNPvP", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.SMNPvP_BurstMode_RadiantAegis)
                UserConfig.DrawSliderInt(0, 90, SMNPvP.Config.SMNPvP_RadiantAegisThreshold, "Caps at 90 to prevent waste.###SMNPvP", 150, SliderIncrements.Ones);

            #endregion

            #endregion
            // ====================================================================================
            #region WARRIOR

            if (preset == CustomComboPreset.WAR_InfuriateFellCleave && enabled)
                UserConfig.DrawSliderInt(0, 50, WAR.Config.WAR_InfuriateRange, "Set how much rage to be at or under to use this feature.");

            if (preset == CustomComboPreset.WAR_ST_StormsPath && enabled)
                UserConfig.DrawSliderInt(0, 30, WAR.Config.WAR_SurgingRefreshRange, "Seconds remaining before refreshing Surging Tempest.");

            if (preset == CustomComboPreset.WAR_ST_StormsPath_Onslaught && enabled)
                UserConfig.DrawSliderInt(0, 2, WAR.Config.WAR_KeepOnslaughtCharges, "How many charges to keep ready? (0 = Use All)");

            #endregion
            // ====================================================================================
            #region WHITE MAGE

            if (preset == CustomComboPreset.WHM_ST_MainCombo_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_ST_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);
            if (preset is CustomComboPreset.WHM_ST_MainCombo_DoT)
                UserConfig.DrawSliderInt(0, 100, WHM.Config.WHM_ST_MainCombo_DoT, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset == CustomComboPreset.WHM_AoE_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, WHM.Config.WHM_AoE_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.WHM_Afflatus_oGCDHeals)
                UserConfig.DrawSliderInt(0, 100, WHM.Config.WHM_oGCDHeals, "Set HP% of target to use Tetragrammaton");

            #endregion
            // ====================================================================================
            #region DOH

            #endregion
            // ====================================================================================
            #region DOL

            #endregion
            // ====================================================================================
            #region PvP VALUES

            if (preset == CustomComboPreset.PvP_EmergencyHeals)
            {
                var pc = Service.ClientState.LocalPlayer;
                if (pc != null)
                {
                    var maxHP = Service.ClientState.LocalPlayer?.MaxHp <= 15000 ? 0 : Service.ClientState.LocalPlayer.MaxHp - 15000;

                    if (maxHP > 0)
                    {
                        var setting = PluginConfiguration.GetCustomIntValue(PvPCommon.Config.EmergencyHealThreshold);
                        var hpThreshold = ((float)maxHP / 100 * setting);

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

            #endregion
        }
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
