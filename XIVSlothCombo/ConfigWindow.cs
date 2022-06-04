using Dalamud.Game.ClientState.Objects.Types;
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
using XIVSlothComboPlugin.ConfigFunctions;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Plugin configuration window.
    /// </summary>
    internal class ConfigWindow : Window
    {
        private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;
        private readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
        /// </summary>
        public ConfigWindow()
            : base("Sloth Combo Setup")
        {
            var p = Service.Configuration.SpecialEvent;

            this.RespectCloseHotkey = true;

            if (p)
            {
                this.groupedPresets = Enum
                .GetValues<CustomComboPreset>()
                .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
                .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .Where(tpl => tpl.Info != null && Service.Configuration.GetParent(tpl.Preset) == null)
                .OrderBy(tpl => tpl.Info.MemeJobName)
                .ThenBy(tpl => tpl.Info.Order)
                .GroupBy(tpl => tpl.Info.MemeJobName)
                .ToDictionary(
                    tpl => tpl.Key,
                    tpl => tpl.ToList());
            }
            else
            {
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
            }


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
            if (ImGui.BeginTabBar("SlothBar"))
            {
                if (ImGui.BeginTabItem("PvE Features"))
                {
                    DrawPVEWindow();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("PvP Features"))
                {
                    DrawPVPWindow();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    DrawGlobalSettings();
                    ImGui.EndTabItem();
                }


                if (ImGui.BeginTabItem("About XIVSlothCombo / Report an Issue"))
                {
                    DrawAboutUs();
                    ImGui.EndTabItem();
                }

#if DEBUG
                if (ImGui.BeginTabItem("Debug Mode"))
                {
                    DrawDebug();
                    ImGui.EndTabItem();
                }
#endif

                ImGui.EndTabBar();
            }

        }

#if DEBUG
        internal class Debug : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; }

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                return actionID;
            }
        }
        private void DrawDebug()
        {
            var LocalPlayer = Service.ClientState.LocalPlayer;
            var comboClass = new Debug();

            if (LocalPlayer != null)
            {
                if (Service.ClientState.LocalPlayer.TargetObject is BattleChara chara)
                {
                    foreach (var status in chara.StatusList)
                    {
                        ImGui.TextUnformatted($"TARGET STATUS CHECK: {chara.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                    }
                }
                foreach (var status in (Service.ClientState.LocalPlayer as BattleChara).StatusList)
                {
                    ImGui.TextUnformatted($"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                }

                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Service.ClientState.LocalPlayer.TargetObject?.ObjectKind}");
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Service.ClientState.LocalPlayer.TargetObject is BattleChara}");
                ImGui.TextUnformatted($"PLAYER IS BATTLE CHARA: {LocalPlayer is BattleChara}");
                ImGui.TextUnformatted($"IN COMBAT: {comboClass.InCombat()}");
                ImGui.TextUnformatted($"IN MELEE RANGE: {comboClass.InMeleeRange()}");
                ImGui.TextUnformatted($"DISTANCE FROM TARGET: {comboClass.GetTargetDistance()}");
                ImGui.TextUnformatted($"TARGET HP VALUE: {comboClass.EnemyHealthCurrentHp()}");
                ImGui.TextUnformatted($"LAST ACTION: {ActionWatching.GetActionName(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST WEAPONSKILL: {ActionWatching.GetActionName(ActionWatching.LastWeaponskill)}");
                ImGui.TextUnformatted($"LAST SPELL: {ActionWatching.GetActionName(ActionWatching.LastSpell)}");
                ImGui.TextUnformatted($"LAST ABILITY: {ActionWatching.GetActionName(ActionWatching.LastAbility)}");
                ImGui.TextUnformatted($"ZONE: {Service.ClientState.TerritoryType}");
                ImGui.TextUnformatted($"SELECTED BLU SPELLS: {string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");

            }
            else
            {
                ImGui.TextUnformatted("Plese log in to use this tab.");
            }
        }
#endif
        private void DrawPVPWindow()
        {
            ImGui.Text("This tab allows you to select which PvP combos and features you wish to enable.");

            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
            ImGui.PopFont();
            ImGui.SameLine();
            ImGui.TextUnformatted("These are PvP features. They will only work in PvP-enabled zones.");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
            ImGui.PopFont();

            ImGui.BeginChild("scrolling", new Vector2(0, 0), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var i = 1;

            foreach (var jobName in this.groupedPresets.Keys)
            {
                if (this.groupedPresets[jobName].Where(x => Service.Configuration.IsSecret(x.Preset)).Count() == 0) continue;

                if (ImGui.CollapsingHeader(jobName))
                {
                    foreach (var (preset, info) in this.groupedPresets[jobName].Where(x => Service.Configuration.IsSecret(x.Preset)))
                    {
                        InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { this.DrawPreset(preset, info, ref i); } };
                        if (Service.Configuration.HideConflictedCombos)
                        {
                            //Presets that are contained within a ConflictedAttribute
                            var conflictOriginals = Service.Configuration.GetConflicts(preset);

                            //Presets with the ConflictedAttribute
                            var conflictsSource = Service.Configuration.GetAllConflicts();

                            if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                presetBox.Draw();
                                ImGuiHelpers.ScaledDummy(12.0f);
                                continue;
                            }
                            if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                            {
                                Service.Configuration.EnabledActions.Remove(preset);
                                Service.Configuration.Save();
                            }
                            else
                            {
                                presetBox.Draw();
                                ImGuiHelpers.ScaledDummy(12.0f);
                                
                                continue;
                            }

                        }
                        else
                        {
                            presetBox.Draw();
                            ImGuiHelpers.ScaledDummy(12.0f);
                        }
                    }
                }
                else
                {
                    i += this.groupedPresets[jobName].Where(x => Service.Configuration.IsSecret(x.Preset)).Count();
                    foreach (var preset in this.groupedPresets[jobName].Where(x => Service.Configuration.IsSecret(x.Preset)))
                    {
                        i += AllChildren(this.presetChildren[preset.Preset]);
                    }
                }

            }

            ImGui.PopStyleVar();
            ImGui.EndChild();
        }

        private static void DrawAboutUs()
        {
            ImGui.BeginChild("about", new Vector2(0, 0), true);

            ImGui.TextColored(ImGuiColors.ParsedGreen, $"v3.0.15.2\n- with love from Team Sloth.");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped($@"Big Thanks to attick and daemitus for creating most of the original code, as well as Grammernatzi and PrincessRTFM for providing a lot of extra tweaks and inspiration. Please show them support for their original work! <3");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped("Brought to you by: \nAki, k-kz, ele-starshade, damolitionn, Taurenkey, Augporto, grimgal and many other contributors!");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGuiColors.HealerGreen);
            if (ImGui.Button("Click here to join our Discord Server!"))
            {
                Util.OpenLink("https://discord.gg/xT7zyjzjtY");
            }
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            if (ImGui.Button("Got an issue? Click this button and report it!"))
            {
                Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/issues");
            }

            ImGui.EndChild();

        }

        private static void DrawGlobalSettings()
        {
            ImGui.BeginChild("main", new Vector2(0, 0), true);
            ImGui.Text("This tab allows you to customise your options when enabling features.");

            #region SubCombos


            var hideChildren = Service.Configuration.HideChildren;
            if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
            {
                Service.Configuration.HideChildren = hideChildren;
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Hides the sub-options of disabled features.");
                ImGui.EndTooltip();
            }
            ImGui.NextColumn();

            #endregion

            #region Conflicting

            var hideConflicting = Service.Configuration.HideConflictedCombos;
            if (ImGui.Checkbox("Hide Conflicted Combos", ref hideConflicting))
            {
                Service.Configuration.HideConflictedCombos = hideConflicting;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Hides any combos that conflict with others you have selected.");
                ImGui.EndTooltip();
            }

            #endregion

            #region Combat Log
            var showCombatLog = Service.Configuration.EnabledOutputLog;

            if (ImGui.Checkbox("Output Log to Chat", ref showCombatLog))
            {
                Service.Configuration.EnabledOutputLog = showCombatLog;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Every time you use an action, the plugin will print it to the chat.");
                ImGui.EndTooltip();
            }

            #endregion

            #region SpecialEvent

            var isSpecialEvent = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
            var slothIrl = isSpecialEvent && Service.Configuration.SpecialEvent;
            if (isSpecialEvent)

            {

                if (ImGui.Checkbox("Sloth Mode!?", ref slothIrl))
                {
                    Service.Configuration.SpecialEvent = slothIrl;
                    Service.Configuration.Save();
                }
            }
            else
            {
                Service.Configuration.SpecialEvent = false;
                Service.Configuration.Save();
            }


            float offset = (float)Service.Configuration.MeleeOffset;
            ImGui.PushItemWidth(75);

            var inputChangedeth = false;
            inputChangedeth |= ImGui.InputFloat("Melee Distance Offset", ref offset);

            if (inputChangedeth)
            {
                Service.Configuration.MeleeOffset = (double)offset;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Offset of melee check distance for features that use it. For those who don't want to immediately use their ranged attack if the boss walks slightly out of range.");
                ImGui.EndTooltip();
            }

            #endregion

            #region Message of the Day
            var motd = Service.Configuration.HideMessageOfTheDay;
            if (ImGui.Checkbox("Hide Message of the Day", ref motd))
            {
                Service.Configuration.HideMessageOfTheDay = motd;
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Disables the Message of the Day message in your chat when you login.");
                ImGui.EndTooltip();
            }
            ImGui.NextColumn();

            #endregion

            ImGui.EndChild();
        }

        private void DrawPVEWindow()
        {
            ImGui.Text("This tab allows you to select which PvE combos and features you wish to enable.");
            ImGui.BeginChild("scrolling", new Vector2(0, 0), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var i = 1;

            foreach (var jobName in this.groupedPresets.Keys)
            {
                if (ImGui.CollapsingHeader(jobName))
                {
                    if (!PrintBLUMessage(jobName)) continue;

                    foreach (var (preset, info) in this.groupedPresets[jobName].Where(x => !Service.Configuration.IsSecret(x.Preset)))
                    {
                        InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { this.DrawPreset(preset, info, ref i); } };
                        if (Service.Configuration.HideConflictedCombos)
                        {
                            //Presets that are contained within a ConflictedAttribute
                            var conflictOriginals = Service.Configuration.GetConflicts(preset);

                            //Presets with the ConflictedAttribute
                            var conflictsSource = Service.Configuration.GetAllConflicts();

                            if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                presetBox.Draw();
                                ImGuiHelpers.ScaledDummy(12.0f);
                                continue;
                            }
                            if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                            {
                                Service.Configuration.EnabledActions.Remove(preset);
                                Service.Configuration.Save();
                            }
                            else
                            {
                                presetBox.Draw();
                                ImGuiHelpers.ScaledDummy(12.0f);
                                continue;
                            }

                        }
                        else
                        {
                            presetBox.Draw();
                            ImGuiHelpers.ScaledDummy(12.0f);
                        }
                    }
                }
                else
                {
                    i += this.groupedPresets[jobName].Where(x => !Service.Configuration.IsSecret(x.Preset)).Count();
                    foreach (var preset in this.groupedPresets[jobName].Where(x => !Service.Configuration.IsSecret(x.Preset)))
                    {
                        i += AllChildren(this.presetChildren[preset.Preset]);
                    }
                }

            }

            ImGui.PopStyleVar();
            ImGui.EndChild();




        }

        private bool PrintBLUMessage(string jobName)
        {
            if (jobName == "Blue Mage")
            {
                if (Service.Configuration.ActiveBLUSpells.Count == 0)
                {
                    ImGui.Text("Please open the Blue Magic Spellbook to populate your active spells and enable features.");
                    return false;
                }
                else
                {
                    ImGui.TextColored(ImGuiColors.ParsedPink, $"Please note that even if you do not have all the required spells active, you may still use these features.\nAny spells you do not have active will be skipped over so if a feature is not working as intended then\nplease try and enable more required spells.");
                }
            }

            return true;
        }

        private void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
        {
            var enabled = Service.Configuration.IsEnabled(preset);
            var secret = Service.Configuration.IsSecret(preset);
            var conflicts = Service.Configuration.GetConflicts(preset);
            var parent = Service.Configuration.GetParent(preset);
            var irlsloth = Service.Configuration.SpecialEvent;
            var blueAttr = preset.GetAttribute<BlueInactiveAttribute>();

            ImGui.PushItemWidth(200);

            if (irlsloth && !string.IsNullOrEmpty(info.MemeName))
            {
                if (ImGui.Checkbox(info.MemeName, ref enabled))
                {
                    if (enabled)
                    {
                        EnableParentPresets(preset);
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
            }
            else
            {
                if (ImGui.Checkbox($"{info.FancyName}###{i}", ref enabled))
                {
                    if (enabled)
                    {
                        EnableParentPresets(preset);
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

            }

            ImGui.PopItemWidth();


            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
            if (irlsloth && !string.IsNullOrEmpty(info.MemeDescription))
            {
                ImGui.TextWrapped($"#{i}: {info.MemeDescription}");
            }
            else
            {
                if (preset.GetAttribute<ReplaceSkillAttribute>() != null)
                {
                    string skills = string.Join(", ", preset.GetAttribute<ReplaceSkillAttribute>().ActionNames);

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted($"Replaces: {skills}");
                        ImGui.EndTooltip();
                    }
                }
                ImGui.TextWrapped($"#{i}: {info.Description}");

                if (preset.GetAttribute<HoverInfoAttribute>() != null)
                {
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(preset.GetAttribute<HoverInfoAttribute>().HoverText);
                        ImGui.EndTooltip();
                    }
                }
            }

            ImGui.PopStyleColor();
            ImGui.Spacing();

            DrawUserConfigs(preset, enabled);

            if (conflicts.Length > 0)
            {
                var conflictText = conflicts.Select(conflict =>
                {
                    var conflictInfo = conflict.GetAttribute<CustomComboInfoAttribute>();
                    if (irlsloth)
                    {
                        return $"\n - {conflictInfo.MemeName}";
                    }
                    else
                    {
                        return $"\n - {conflictInfo.FancyName}";

                    }

                }).Aggregate((t1, t2) => $"{t1}{t2}");

                if (conflictText.Length > 0)
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, $"Conflicts with: {conflictText}");
                    ImGui.Spacing();
                }
            }

            if (blueAttr != null)
            {
                if (blueAttr.Actions.Count > 0)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudOrange);
                    ImGui.Text($"Missing active spells: {string.Join(", ", blueAttr.Actions.Select(x => ActionWatching.GetActionName(x)))}");
                    ImGui.PopStyleColor();
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    ImGui.Text($"All required spells active!");
                    ImGui.PopStyleColor();
                }

            }

            i++;

            var hideChildren = Service.Configuration.HideChildren;
            var children = this.presetChildren[preset];

            if (children.Length > 0)
            {
                if (enabled || !hideChildren)
                {
                    ImGui.Indent();

                    foreach (var (childPreset, childInfo) in children)
                    {

                        if (Service.Configuration.HideConflictedCombos)
                        {
                            //Presets that are contained within a ConflictedAttribute
                            var conflictOriginals = Service.Configuration.GetConflicts(childPreset);

                            //Presets with the ConflictedAttribute
                            var conflictsSource = Service.Configuration.GetAllConflicts();

                            if (!conflictsSource.Where(x => x == childPreset || x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                this.DrawPreset(childPreset, childInfo, ref i);
                                continue;
                            }
                            if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                            {
                                Service.Configuration.EnabledActions.Remove(childPreset);
                                Service.Configuration.Save();
                            }
                            else
                            {
                                this.DrawPreset(childPreset, childInfo, ref i);
                                continue;
                            }

                        }
                        else
                        {
                            this.DrawPreset(childPreset, childInfo, ref i);
                        }


                    }


                    ImGui.Unindent();
                }
                else
                {
                    i += AllChildren(this.presetChildren[preset]);

                }
            }
        }

        private int AllChildren((CustomComboPreset Preset, CustomComboInfoAttribute Info)[] children)
        {
            var output = 0;

            foreach (var child in children)
            {
                output++;
                output += AllChildren(this.presetChildren[child.Preset]);
            }

            return output;
        }

        /// <summary>
        /// Iterates up a preset's parent tree, enabling each of them.
        /// </summary>
        /// <param name="preset">Combo preset to enabled.</param>
        private static void EnableParentPresets(CustomComboPreset preset)
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

        /// <summary>
        /// Draws the User Configurable settings.
        /// </summary>
        /// <param name="preset">The preset it's attached to</param>
        /// <param name="enabled">If it's enabled or not</param>
        private static void DrawUserConfigs(CustomComboPreset preset, bool enabled)
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
                ConfigWindowFunctions.DrawRadioButton(AST.Config.AST_DPS_AltMode, "On Malefic", "", 0);
                ConfigWindowFunctions.DrawRadioButton(AST.Config.AST_DPS_AltMode, "On Combust", "Alternative DPS Mode. Leaves Malefic alone for pure DPS, becomes Malefic when features are on cooldown", 1);
            }
            if (preset is CustomComboPreset.AST_DPS_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, AST.Config.AST_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.AST_ST_DPS_CombustUptime)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_DPS_Divination)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AST_DPS_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_DPS_LightSpeed)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AST_DPS_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AST_EssentialDignity, "Set percentage value");

            #endregion
            // ====================================================================================
            #region BLACK MAGE

            if (preset == CustomComboPreset.BLM_AoE_Simple_Foul)
                ConfigWindowFunctions.DrawSliderInt(0, 2, BLM.Config.BLM_PolyglotsStored, "Number of Polyglot charges to store.\n(2 = Only use Polyglot with Manafont)");

            if (preset == CustomComboPreset.BLM_SimpleMode || preset == CustomComboPreset.BLM_Simple_Transpose)
                ConfigWindowFunctions.DrawRoundedSliderFloat(3.0f, 8.0f, BLM.Config.BLM_AstralFireRefresh, "Seconds before refreshing Astral Fire.\n(6s = Recommended)");

            if (preset == CustomComboPreset.BLM_Simple_CastMovement)
                ConfigWindowFunctions.DrawRoundedSliderFloat(0.0f, 1.0f, BLM.Config.BLM_MovementTime, "Seconds of movement before using the movement feature.");

            #endregion
            // ====================================================================================
            #region BLUE MAGE

            #endregion
            // ====================================================================================
            #region BARD
            if (preset == CustomComboPreset.BRD_Simple_RagingJaws)
                ConfigWindowFunctions.DrawSliderInt(3, 5, BRD.Config.RagingJawsRenewTime, "Remaining time (In seconds)");

            if (preset == CustomComboPreset.BRD_Simple_NoWaste)
                ConfigWindowFunctions.DrawSliderInt(1, 10, BRD.Config.NoWasteHPPercentage, "Remaining target HP percentage");

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
                ConfigWindowFunctions.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_ST, "Esprit", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_EspritOvercap)
                ConfigWindowFunctions.DrawSliderInt(50, 100, DNC.Config.DNCEspritThreshold_AoE, "Esprit", 150, SliderIncrements.Ones);

            #region Simple ST Sliders
            if (preset == CustomComboPreset.DNC_ST_Simple_SS)
                ConfigWindowFunctions.DrawSliderInt(0, 5, DNC.Config.DNCSimpleSSBurstPercent, "Target HP percentage to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_TS)
                ConfigWindowFunctions.DrawSliderInt(0, 5, DNC.Config.DNCSimpleTSBurstPercent, "Target HP percentage to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_FeatherPooling)
                ConfigWindowFunctions.DrawSliderInt(0, 5, DNC.Config.DNCSimpleFeatherBurstPercent, "Target HP percentage to dump all pooled feathers below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                ConfigWindowFunctions.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWaltzPercent, "Curing Waltz HP percent", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_ST_Simple_PanicHeals)
                ConfigWindowFunctions.DrawSliderInt(0, 100, DNC.Config.DNCSimplePanicHealWindPercent, "Second Wind HP percent", 200, SliderIncrements.Ones);
            #endregion

            #region Simple AoE Sliders
            if (preset == CustomComboPreset.DNC_AoE_Simple_SS)
                ConfigWindowFunctions.DrawSliderInt(0, 10, DNC.Config.DNCSimpleSSAoEBurstPercent, "Target HP percentage to stop using Standard Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_TS)
                ConfigWindowFunctions.DrawSliderInt(0, 10, DNC.Config.DNCSimpleTSAoEBurstPercent, "Target HP percentage to stop using Technical Step below", 75, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                ConfigWindowFunctions.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWaltzPercent, "Curing Waltz HP percent", 200, SliderIncrements.Ones);

            if (preset == CustomComboPreset.DNC_AoE_Simple_PanicHeals)
                ConfigWindowFunctions.DrawSliderInt(0, 100, DNC.Config.DNCSimpleAoEPanicHealWindPercent, "Second Wind HP percent", 200, SliderIncrements.Ones);
            #endregion

            #region PvP Sliders
            if (preset == CustomComboPreset.DNCPvP_BurstMode_CuringWaltz)
                ConfigWindowFunctions.DrawSliderInt(0, 90, DNCPVP.Config.DNCPvP_WaltzThreshold, "Caps at 90 to prevent waste.###DNCPvP", 150, SliderIncrements.Ones);
            #endregion

            #endregion
            // ====================================================================================
            #region DARK KNIGHT
            if (preset == CustomComboPreset.DRK_EoSPooling && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 3000, DRK.Config.DrkMPManagement, "How much MP to save (0 = Use All)", 150, SliderIncrements.Thousands);
            if (preset == CustomComboPreset.DRK_Plunge && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, DRK.Config.DrkKeepPlungeCharges, "How many charges to keep ready? (0 = Use All)", 75, SliderIncrements.Ones);
            #endregion
            // ====================================================================================
            #region DRAGOON

            #endregion
            // ====================================================================================
            #region GUNBREAKER
            if (preset == CustomComboPreset.GNB_ST_RoughDivide && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, GNB.Config.GNB_RoughDivide_HeldCharges, "How many charges to keep ready? (0 = Use All)");
            #endregion
            // ====================================================================================
            #region MACHINIST

            #endregion
            // ====================================================================================
            #region MONK

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                ConfigWindowFunctions.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_Demolish_Apply, "Seconds remaining before refreshing Demolish.");

            if (preset == CustomComboPreset.MNK_ST_SimpleMode)
                ConfigWindowFunctions.DrawRoundedSliderFloat(5.0f, 10.0f, MNK.Config.MNK_DisciplinedFist_Apply, "Seconds remaining before refreshing Disciplined Fist.");

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
                ConfigWindowFunctions.DrawSliderInt(0, 15, NIN.Config.Trick_CooldownRemaining, "Set the amount of time in seconds for the feature to try and set up \nSuiton in advance of Trick Attack coming off cooldown");


            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_Huraijin)
                ConfigWindowFunctions.DrawSliderInt(0, 60, NIN.Config.Huton_RemainingTimer, "Set the amount of time remaining on Huton the feature\nshould wait before using Huraijin", 200);


            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_Mug)
                ConfigWindowFunctions.DrawSliderInt(0, 100, NIN.Config.Mug_NinkiGauge, $"Set the amount of Ninki to be at or under for this feature (level {NIN.TraitLevels.Shukiho} onwards)");

            if (preset == CustomComboPreset.NIN_AeolianEdgeCombo_ArmorCrush)
                ConfigWindowFunctions.DrawSliderInt(0, 30, NIN.Config.Huton_RemainingArmorCrush, "Set the amount of time remaining on Huton the feature\nshould wait before using Armor Crush", 200);

            if (preset == CustomComboPreset.NIN_NinkiPooling_Bhavacakra)
                ConfigWindowFunctions.DrawSliderInt(50, 100, NIN.Config.Ninki_BhavaPooling, "The minimum value of Ninki to have before spending.");

            if (preset == CustomComboPreset.NIN_NinkiPooling_Bunshin)
                ConfigWindowFunctions.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPooling, "The minimum value of Ninki to have before spending.");

            #endregion
            // ====================================================================================
            #region PALADIN
            //if (preset == CustomComboPreset.PaladinAtonementDropFeature && enabled)
            //    ConfigWindowFunctions.DrawSliderInt(2, 3, PLD.Config.PLDAtonementCharges, "How many Atonements to cast right before FoF (Atonement Drop)?");

            if (preset == CustomComboPreset.PLD_ST_RoyalAuth_Intervene && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, PLD.Config.PLD_Intervene_HoldCharges, "How many charges to keep ready? (0 = Use all)");

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
                ConfigWindowFunctions.DrawSliderInt(0, 8, RPRPVP.Config.RPRPvP_ImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.###RPR", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPRPvP_Burst_ArcaneCircle && enabled)
                ConfigWindowFunctions.DrawSliderInt(5, 90, RPRPVP.Config.RPRPvP_ArcaneCircleThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.###RPR", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.ReaperPositionalConfig && enabled)
            {
                    ConfigWindowFunctions.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear First", "First positional: Gallows (Rear), Void Reaping.", 1);
                    ConfigWindowFunctions.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Flank First", "First positional: Gibbet (Flank), Cross Reaping.", 2);
                    ConfigWindowFunctions.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear: Slice, Flank: SoD", "Rear positionals on Slice, Flank positionals on Shadow of Death.", 3);
                    ConfigWindowFunctions.DrawHorizontalRadioButton(RPR.Config.RPR_PositionalChoice, "Rear: SoD, Flank: Slice", "Rear positionals on Shadow of Death, Flank positionals on Slice.", 4);
            }

            if (preset == CustomComboPreset.RPR_ST_SliceCombo_SoD && enabled)
            {
                ConfigWindowFunctions.DrawSliderInt(0, 6, RPR.Config.RPR_SoDRefreshRange, "Seconds remaining before refreshing Death's Design.", 150, SliderIncrements.Ones);
                ConfigWindowFunctions.DrawSliderInt(0, 5, RPR.Config.RPR_SoDThreshold, "Set a HP% Threshold for when SoD will not be automatically applied to the target.", 150, SliderIncrements.Ones);
            }

            #endregion
            // ====================================================================================
            #region RED MAGE

            if (preset == CustomComboPreset.RDM_oGCD)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Fleche", "", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Jolt\n-Jolt II", "Select for one button rotation", 2);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Scatter\n-Impact", "Select for one button rotation", 3);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 4);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Riposte\n-Moulinet", "", 5);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_OGCD_OnAction, "-Fleche\n-Riposte\n-Moulinet", "", 6);
            }

            if (preset == CustomComboPreset.RDM_ST_MeleeCombo)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Riposte", "", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Jolt\n-Jolt II", "Select for one button rotation", 2);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "-Riposte\n-Jolt\n-Jolt II", "Select for one button rotation", 3);
            }

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Riposte\n-Moulinet", "", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 2);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Riposte\n-Moulinet\n-Jolt\n-Jolt II\n-Scatter\n-Impact", "Select for one button rotation", 3);
                ConfigWindowFunctions.DrawHorizontalRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "-Veraero 1/2/3\n-Verthunder 1/2/3", "", 4);
            }

            if (preset == CustomComboPreset.RDM_Lucid && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 10000, RDM.Config.RDM_Lucid_Threshold, "Add Lucid Dreaming when below this MP", 300, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.RDM_AoE_MeleeCombo && enabled)
                ConfigWindowFunctions.DrawSliderInt(3, 8, RDM.Config.RDM_MoulinetRange, "Range to use first Moulinet; no range restrictions after first Moulinet", 150, SliderIncrements.Ones);

            #endregion
            // ====================================================================================
            #region SAGE

            if (preset is CustomComboPreset.SGE_ST_Dosis_EDosis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Dosis_EDosisHPPer, "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SGE_ST_Dosis_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SGE.Config.SGE_ST_Dosis_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Dosis_Toxikon)
            {
                ConfigWindowFunctions.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show when moving only", "", 0);
                ConfigWindowFunctions.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show at all times", "", 1);
            }

            if (preset is CustomComboPreset.SGE_AoE_Phlegma_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SGE.Config.SGE_AoE_Phlegma_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Heal_Soteria)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Soteria, "Set HP percentage value for Soteria to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Zoe)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Zoe, "Set HP percentage value for Zoe to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Pepsis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Pepsis, "Set HP percentage value for Pepsis to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Taurochole)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Taurochole, "Set HP percentage value for Taurochole to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Haima)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Haima, "Set HP percentage value for Haima to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Krasis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Krasis, "Set HP percentage value for Krasis to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Druochole)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Druochole, "Set HP percentage value for Druochole to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Diagnosis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Diagnosis, "Set HP percentage value for Eukrasian Diagnosis to trigger");
            #endregion
            // ====================================================================================
            #region SAMURAI
            if (preset == CustomComboPreset.SAM_ST_Overcap && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 85, SAM.Config.SAM_ST_KenkiOvercapAmount, "Set the Kenki overcap amount for ST combos.");
            if (preset == CustomComboPreset.SAM_AoE_Overcap && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 85, SAM.Config.SAM_AoE_KenkiOvercapAmount, "Set the Kenki overcap amount for AOE combos.");
            //PVP
            if (preset == CustomComboPreset.SAMPvP_BurstMode && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 2, SAMPvP.Config.SAMPvP_SotenCharges, "How many charges of Soten to keep ready? (0 = Use All).");
            if (preset == CustomComboPreset.SAMPvP_KashaFeatures_GapCloser && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SAMPvP.Config.SAMPvP_SotenHP, "Use Soten on enemies below selected HP.");
            //Fillers
            if (preset == CustomComboPreset.SAM_ST_GekkoCombo_FillerCombos)
            {
                    ConfigWindowFunctions.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "2.14+", "2 Filler GCDs", 1);
                    ConfigWindowFunctions.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "2.06 - 2.08", "3 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range.\nHakaze will be delayed by half a GCD after Enpi.", 2);
                    ConfigWindowFunctions.DrawHorizontalRadioButton(SAM.Config.SAM_FillerCombo, "1.99 - 2.01", "4 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range. \nHakaze will be delayed by half a GCD after Enpi.", 3);
            }
            #endregion
            // ====================================================================================
            #region SCHOLAR
            if (preset is CustomComboPreset.SCH_DPS_Feature)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_ST_DPS_AltMode, "On Ruin I / Broils", "", 0);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_ST_DPS_AltMode, "On Bio", "Alternative DPS Mode. Leaves Ruin I / Broil alone for pure DPS, becomes Ruin I / Broil when features are on cooldown", 1);
            }
            if (preset is CustomComboPreset.SCH_DPS_LucidOption)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SCH.Config.SCH_ST_DPS_LucidOption, "MP Threshold", 150, SliderIncrements.Hundreds);
            if (preset is CustomComboPreset.SCH_DPS_BioOption)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_BioOption, "Stop using at Enemy HP %. Set to Zero to disable this check");
            if (preset is CustomComboPreset.SCH_DPS_ChainStratagemOption)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SCH.Config.SCH_ST_DPS_ChainStratagemOption, "Stop using at Enemy HP %. Set to Zero to disable this check");
            if (preset is CustomComboPreset.SCH_FairyFeature)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Eos", "", 0);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Selene", "", 1);
            }
            if (preset is CustomComboPreset.SCH_AetherflowFeature)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On Energy Drain Only","", 0);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Display, "Show Aetherflow On All Aetherflow Skills", "", 1);
            }
            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Excog)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Only when out of Aetherflow Stacks", "", 0);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Always when available", "", 1);
            }
            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Indom)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Only when out of Aetherflow Stacks", "", 0);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Always when available", "", 1);
            }
            #endregion
            // ====================================================================================
            #region SUMMONER

            if (preset == CustomComboPreset.SMN_DemiEgiMenu_EgiOrder)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_PrimalChoice, "Titan first", "Summons Titan first, Garuda second, Ifrit third", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_PrimalChoice, "Garuda first", "Summons Garuda first, Titan second, Ifrit third", 2);
            }

            
            if (preset == CustomComboPreset.SMN_DemiEgiMenu_BurstChoice)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Bahamut", "Burst during Bahamut Phase", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Phoenix", "Burst during Phoenix Phase", 2);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "Bahamut or Phoenix", "Burst during Bahamut or Phoenix Phase (whichever happens first)", 3);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_BurstPhase, "SpS Friendly Option", "Bursts when Searing Light is ready regardless of Phase", 4);
            }

            if (preset == CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi)
            {
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "Garuda", "Swiftcast Slipstream", 1);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "Ifrit", "Swiftcast Ruby Ruin/Rite", 2);
                ConfigWindowFunctions.DrawHorizontalRadioButton(SMN.Config.SMN_SwiftcastPhase, "SpS Friendly Option", "Swiftcasts whichever Primal is available when Swiftcast is ready", 3);
            }

            if (preset == CustomComboPreset.SMN_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SMN.Config.SMN_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            #endregion
            // ====================================================================================
            #region WARRIOR
            if (preset == CustomComboPreset.WarriorInfuriateFellCleave && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 50, WAR.Config.WarInfuriateRange, "Set how much rage to be at or under to use this feature.");

            if (preset == CustomComboPreset.WarriorStormsPathCombo && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 30, WAR.Config.WarSurgingRefreshRange, "Seconds remaining before refreshing Surging Tempest.");

            if (preset == CustomComboPreset.WarriorOnslaughtFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 2, WAR.Config.WarKeepOnslaughtCharges, "How many charges to keep ready? (0 = Use All)");

            #endregion
            // ====================================================================================
            #region WHITE MAGE
            if (preset == CustomComboPreset.WHMLucidDreamingFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, WHM.Config.WHMLucidDreamingFeature, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.WHM_AoE_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, WHM.Config.WHM_AoE_Lucid, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.WHMogcdHealsShieldsFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, WHM.Config.WHMogcdHealsShieldsFeature, "Set HP% of target to use Tetragrammaton");

            #endregion
            // ====================================================================================
            #region DOH

            #endregion
            // ====================================================================================
            #region DOL

            #endregion
            // ====================================================================================
            #region PVP VALUES
            if (preset == CustomComboPreset.PvP_EmergencyHeals)
            {
                var pc = Service.ClientState.LocalPlayer;
                if (pc != null)
                {
                    var maxHP = Service.ClientState.LocalPlayer?.MaxHp <= 15000 ? 0 : Service.ClientState.LocalPlayer.MaxHp - 15000;
                    if (maxHP > 0)
                    {
                        var setting = Service.Configuration.GetCustomIntValue(PVPCommon.Config.EmergencyHealThreshold);
                        var hpThreshold = ((float)maxHP / 100 * setting);

                        ConfigWindowFunctions.DrawSliderInt(1, 100, PVPCommon.Config.EmergencyHealThreshold, $"Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.\nHP Value to be at or under: {hpThreshold}");
                    }
                    else
                    {
                        ConfigWindowFunctions.DrawSliderInt(1, 100, PVPCommon.Config.EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                    }
                }
                else
                {
                    ConfigWindowFunctions.DrawSliderInt(1, 100, PVPCommon.Config.EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                }
            }

            if (preset == CustomComboPreset.PvP_EmergencyGuard)
                ConfigWindowFunctions.DrawSliderInt(1, 100, PVPCommon.Config.EmergencyGuardThreshold, "Set the percentage to be at or under for the feature to kick in.");

            if (preset == CustomComboPreset.PvP_QuickPurify)
                ConfigWindowFunctions.DrawPvPStatusMultiChoice(PVPCommon.Config.QuickPurifyStatuses);

            #endregion

        }

    }
}
