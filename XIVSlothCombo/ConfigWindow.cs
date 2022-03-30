using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVSlothComboPlugin.Attributes;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Plugin configuration window.
    /// </summary>
    internal class ConfigWindow : Window
    {
        private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;
        private readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren;
        private readonly Vector4 shadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
        /// </summary>
        public ConfigWindow()
            : base("Sloth Combo Setup")
        {
            this.RespectCloseHotkey = true;

            this.groupedPresets = Enum
                .GetValues<CustomComboPreset>()
                .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
                .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .Where(tpl => tpl.Info != null && Service.Configuration.GetParent(tpl.Preset) == null)
                .OrderBy(tpl => tpl.Info.JobName)
                .ThenBy(tpl => tpl.Info.Order)
                .GroupBy(tpl => tpl.Info.JobName)
                .ToDictionary(
                    tpl => tpl.Key,
                    tpl => tpl.ToList());

            var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
                tpl => tpl,
                tpl => new List<CustomComboPreset>());

            foreach (var preset in Enum.GetValues<CustomComboPreset>())
            {
                var parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
                if (parent != null)
                    childCombos[parent.Value].Add(preset);
            }

            this.presetChildren = childCombos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
                    .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                    .OrderBy(tpl => tpl.Info.Order).ToArray());

            this.SizeCondition = ImGuiCond.FirstUseEver;
            this.Size = new Vector2(740, 490);
        }
        public override void Draw()
        {
            ImGui.Text("This window allows you to enable and disable custom combos to your liking.");

            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudRed, $" Notice! All Settings Have Been Reset!");

            var isAprilFools = DateTime.Now.Day == 1 && DateTime.Now.Month == 4 ? true : false;


            var showSecrets = Service.Configuration.EnableSecretCombos;
            if (ImGui.Checkbox("Enable PvP Combos", ref showSecrets))
            {
                Service.Configuration.EnableSecretCombos = showSecrets;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Adds PVP Combos To The Combo Setup Screen");
                ImGui.EndTooltip();
            }


            var hideChildren = Service.Configuration.HideChildren;
            if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
            {
                Service.Configuration.HideChildren = hideChildren;
                Service.Configuration.Save();
            }

            var slothIrl = Service.Configuration.AprilFoolsSlothIrl;
            if (isAprilFools)
            {

                if (ImGui.Checkbox("Actually turn into a sloth irl", ref slothIrl))
                {
                    Service.Configuration.AprilFoolsSlothIrl = slothIrl;
                    Service.Configuration.Save();
                }
            }

            ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var i = 1;

            foreach (var jobName in this.groupedPresets.Keys)
            {

                if (ImGui.CollapsingHeader(jobName))
                {
                    foreach (var (preset, info) in this.groupedPresets[jobName])
                    {
                        this.DrawPreset(preset, info, ref i);
                    }
                }
                else
                {
                    i += this.groupedPresets[jobName].Count;
                }

            }

            ImGui.PopStyleVar();

            ImGui.EndChild();

        }

        private void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
        {
            var enabled = Service.Configuration.IsEnabled(preset);
            var secret = Service.Configuration.IsSecret(preset);
            var showSecrets = Service.Configuration.EnableSecretCombos;
            var conflicts = Service.Configuration.GetConflicts(preset);
            var parent = Service.Configuration.GetParent(preset);
            var irlsloth = Service.Configuration.AprilFoolsSlothIrl;


            if (secret && !showSecrets)
                return;

            ImGui.PushItemWidth(200);

            if (ImGui.Checkbox(info.FancyName, ref enabled))
            {
                if (enabled)
                {
                    this.EnableParentPresets(preset);
                    Service.Configuration.EnabledActions.Add(preset);
                    foreach (var conflict in conflicts)
                    {
                        Service.Configuration.EnabledActions.Remove(conflict);
                    }
                }
                else
                {
                    Service.Configuration.EnabledActions.Remove(preset);
                }

                Service.Configuration.Save();
            }


            if (secret)
            {
                ImGui.SameLine();
                ImGui.Text("  ");
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedOrange);
                ImGui.Text(FontAwesomeIcon.SkullCrossbones.ToIconString());
                ImGui.PopStyleColor();
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("This is a PVP Combo (Only Works in PVP Enabled Areas)");
                    ImGui.EndTooltip();
                }
            }

            ImGui.PopItemWidth();

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);
            ImGui.TextWrapped($"#{i}: {info.Description}");
            ImGui.PopStyleColor();
            ImGui.Spacing();

            if (conflicts.Length > 0)
            {
                var conflictText = conflicts.Select(conflict =>
                {
                    if (!showSecrets && Service.Configuration.IsSecret(conflict))
                        return string.Empty;

                    var conflictInfo = conflict.GetAttribute<CustomComboInfoAttribute>();
                    return $"\n - {conflictInfo.FancyName}";
                }).Aggregate((t1, t2) => $"{t1}{t2}");

                if (conflictText.Length > 0)
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, $"Conflicts with: {conflictText}");
                    ImGui.Spacing();
                }
            }

            if (preset == CustomComboPreset.DancerDanceComboCompatibility && enabled)
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
            if (preset == CustomComboPreset.CustomValuesTest && enabled || preset == CustomComboPreset.SageDPSFeatureTest && enabled)
            {
                var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                var CurrentHpValue = Service.Configuration.EnemyCurrentHp;

                var inputChanged = false;
                ImGui.PushItemWidth(75);
                inputChanged |= ImGui.InputFloat("Input Target MAX Hp  (If targets MAX Hp is BELOW this value it will not use DoT)", ref MaxHpValue);
                inputChanged |= ImGui.InputFloat("Input Current Enemy Hp (Flat Value) (If targets Current HP is BELOW this value it will not use DoT)", ref CurrentHpValue);
                inputChanged |= ImGui.InputFloat("Input Current Enemy % Hp (If targets Current % Hp is BELOW this value it will not use DoT)", ref PercentageHpValue);


                if (inputChanged)
                {
                    Service.Configuration.EnemyHealthMaxHp = MaxHpValue;
                    Service.Configuration.EnemyHealthPercentage = PercentageHpValue;
                    Service.Configuration.EnemyCurrentHp = CurrentHpValue;

                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }
            if (preset == CustomComboPreset.PaladinFightOrFlightMainComboFeatureTest)
            {
                var CustomGCDValueHigh = Service.Configuration.CustomGCDValueHigh;
                var CustomGCDValueLow = Service.Configuration.CustomGCDValueLow;

                var inputChanged = false;
                ImGui.PushItemWidth(75);
                inputChanged |= ImGui.InputFloat("Input Custom GCD Value For a Skill to be used in-between the GCD Value Low", ref CustomGCDValueHigh);
                inputChanged |= ImGui.InputFloat("Input Custom GCD Value For a Skill to be used in-between the GCD Value High", ref CustomGCDValueLow);




                if (inputChanged)
                {
                    Service.Configuration.CustomGCDValueHigh = CustomGCDValueHigh;
                    Service.Configuration.CustomGCDValueLow = CustomGCDValueLow;

                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }
            if (preset == CustomComboPreset.SkillCooldownRemaining)
            {
                var SkillCooldownRemaining = Service.Configuration.SkillCooldownRemaining;



                var inputChanged = false;
                ImGui.PushItemWidth(75);
                inputChanged |= ImGui.InputFloat("Input Skill Cooldown remaining Time", ref SkillCooldownRemaining);

                if (inputChanged)
                {
                    Service.Configuration.SkillCooldownRemaining = SkillCooldownRemaining;

                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }
            if (preset == CustomComboPreset.BardSimpleRagingJaws && enabled)
            {
                var ragingJawsRenewTime = Service.Configuration.GetCustomConfigValue(BRD.Config.RagingJawsRenewTime);

                var inputChanged = false;
                ImGui.PushItemWidth(75);
                inputChanged |= ImGui.InputFloat("Remaining time (In seconds)", ref ragingJawsRenewTime);

                if (inputChanged)
                {
                    ragingJawsRenewTime = ragingJawsRenewTime < 3 ? 3 : ragingJawsRenewTime;

                    Service.Configuration.SetCustomConfigValue(BRD.Config.RagingJawsRenewTime, ragingJawsRenewTime);

                    Service.Configuration.Save();
                }

                ImGui.Spacing();
            }
            if (preset == CustomComboPreset.NinjaSimpleMudras && enabled)
            {
                var mudrapath = Service.Configuration.MudraPathSelection;

                bool path1 = mudrapath == 1 ? true : false;
                bool path2 = mudrapath == 2 ? true : false;

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
            if (preset == CustomComboPreset.ScholarLucidDPSFeature && enabled)
            {
                DrawSliderInt(0, 10000, SCH.Config.ScholarLucidDreaming, "Set value for your MP to be at or under for this feature to work");
            }
            if (preset == CustomComboPreset.NinSimpleTrickFeature && enabled)
            {
                DrawSliderInt(0, 15, NIN.Config.TrickCooldownRemaining, "Set the amount of time in seconds for the feature to try and set up \nSuiton in advance of Trick Attack coming off cooldown");
            }

            i++;

            var hideChildren = Service.Configuration.HideChildren;
            if (enabled || !hideChildren)
            {
                var children = this.presetChildren[preset];
                if (children.Length > 0)
                {
                    ImGui.Indent();

                    foreach (var (childPreset, childInfo) in children)
                        this.DrawPreset(childPreset, childInfo, ref i);

                    ImGui.Unindent();
                }
            }

        }

        /// <summary>
        /// Iterates up a preset's parent tree, enabling each of them.
        /// </summary>
        /// <param name="preset">Combo preset to enabled.</param>
        private void EnableParentPresets(CustomComboPreset preset)
        {
            var parentMaybe = Service.Configuration.GetParent(preset);
            while (parentMaybe != null)
            {
                var parent = parentMaybe.Value;

                if (!Service.Configuration.EnabledActions.Contains(parent))
                {
                    Service.Configuration.EnabledActions.Add(parent);
                    foreach (var conflict in Service.Configuration.GetConflicts(parent))
                    {
                        Service.Configuration.EnabledActions.Remove(conflict);
                    }
                }

                parentMaybe = Service.Configuration.GetParent(parent);
            }
        }

        private void DrawSliderInt(int minValue, int maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = Service.Configuration.GetCustomIntValue(config);
            Dalamud.Logging.PluginLog.Debug(output.ToString());
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

        private void DrawSliderFloat(float minValue, float maxValue, string config, string sliderDescription, float itemWidth = 150)
        {
            var output = Service.Configuration.GetCustomConfigValue(config);
            Dalamud.Logging.PluginLog.Debug(output.ToString());
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
    }
}
