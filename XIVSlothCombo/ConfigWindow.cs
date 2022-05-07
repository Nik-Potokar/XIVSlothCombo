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
                if (ImGui.BeginTabItem("Features & Options"))
                {
                    DrawMainWindow();
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
                ImGui.EndTabBar();
            }

        }

        private static void DrawAboutUs()
        {
            ImGui.BeginChild("about", new Vector2(0, 0), true);

            ImGui.TextColored(ImGuiColors.ParsedGreen, $"v3.0.13.0\n- with love from Team Sloth.");
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

            #region PvPCombos

            var showSecrets = Service.Configuration.EnableSecretCombos;
            if (ImGui.Checkbox("Show PvP Combos", ref showSecrets))
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
            ImGui.NextColumn();

            #endregion

            #region TrustIncompatibles

            //var showTrustIncompatible = Service.Configuration.EnableTrustIncompatibles;
            //if (ImGui.Checkbox("Show Trust Incompatible Combos", ref showTrustIncompatible))
            //{
            //    Service.Configuration.EnableTrustIncompatibles = showTrustIncompatible;
            //    Service.Configuration.Save();
            //}

            //if (ImGui.IsItemHovered())
            //{
            //    ImGui.BeginTooltip();
            //    ImGui.TextUnformatted("These features won't work in a trust run due to technical restraints.");
            //    ImGui.EndTooltip();
            //}
            //ImGui.NextColumn();

            #endregion

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
                ImGui.TextUnformatted("Hides all options a combo might have until you enable it.");
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
                ImGui.TextUnformatted("Hides any combos that conflict with anything you have selected.");
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

        private void DrawMainWindow()
        {
            ImGui.Text("This tab allows you to select which combos and features you wish to enable.");
            ImGui.BeginChild("scrolling", new Vector2(0, 0), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var i = 1;

            foreach (var jobName in this.groupedPresets.Keys)
            {

                if (ImGui.CollapsingHeader(jobName))
                {
                    foreach (var (preset, info) in this.groupedPresets[jobName])
                    {
                        if (Service.Configuration.HideConflictedCombos)
                        {
                            //Presets that are contained within a ConflictedAttribute
                            var conflictOriginals = Service.Configuration.GetConflicts(preset);

                            //Presets with the ConflictedAttribute
                            var conflictsSource = Service.Configuration.GetAllConflicts();

                            if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                this.DrawPreset(preset, info, ref i);
                                continue;
                            }
                            if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                            {
                                Service.Configuration.EnabledActions.Remove(preset);
                                Service.Configuration.Save();
                            }
                            else
                            {
                                this.DrawPreset(preset, info, ref i);
                                continue;
                            }

                        }
                        else
                        {
                            this.DrawPreset(preset, info, ref i);
                        }
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
            var irlsloth = Service.Configuration.SpecialEvent;

            if (secret && !showSecrets)
            {
                i++;
                i += AllChildren(this.presetChildren[preset]);
                return;
            }
                

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


            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
            if (irlsloth && !string.IsNullOrEmpty(info.MemeDescription))
            {
                ImGui.TextWrapped($"#{i}: {info.MemeDescription}");
            }
            else
            {
                ImGui.TextWrapped($"#{i}: {info.Description}");
            }

            ImGui.PopStyleColor();
            ImGui.Spacing();

            DrawUserConfigs(preset, enabled);

            if (conflicts.Length > 0)
            {
                var conflictText = conflicts.Select(conflict =>
                {
                    if (!showSecrets && Service.Configuration.IsSecret(conflict))
                        return string.Empty;


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
            if (preset == CustomComboPreset.AstrologianLucidFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, AST.Config.ASTLucidDreamingFeature, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.AstroEssentialDignity)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AstroEssentialDignity, "Set percentage value");


            #endregion
            // ====================================================================================
            #region BLACK MAGE

            if (preset == CustomComboPreset.BlackAoEFoulOption)
            {
                ConfigWindowFunctions.DrawSliderInt(0, 2, BLM.Config.BlmPolyglotsStored, "Number of Polyglot charges to store.\n(2 = Only use Polyglot with Manafont)");
            }
            if (preset == CustomComboPreset.BlackSimpleFeature || preset == CustomComboPreset.BlackSimpleTransposeFeature)
            {
                ConfigWindowFunctions.DrawSliderFloat(3.0f, 8.0f, BLM.Config.BlmAstralFireRefresh, "Seconds before refreshing Astral Fire.\n(6s = Recommended)");
            }

            #endregion
            // ====================================================================================
            #region BLUE MAGE

            #endregion
            // ====================================================================================
            #region BARD
            if (preset == CustomComboPreset.BardSimpleRagingJaws)
                ConfigWindowFunctions.DrawSliderInt(3, 5, BRD.Config.RagingJawsRenewTime, "Remaining time (In seconds)");

            if (preset == CustomComboPreset.BardSimpleNoWasteMode)
                ConfigWindowFunctions.DrawSliderInt(1, 10, BRD.Config.NoWasteHPPercentage, "Remaining target HP percentage");

            #endregion
            // ====================================================================================
            #region DANCER
            if (preset == CustomComboPreset.DancerDanceComboCompatibility)
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

            if (preset == CustomComboPreset.DNCCuringWaltzOption)
                ConfigWindowFunctions.DrawSliderInt(0, 90, DNCPVP.Config.DNCWaltzThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.###DNC", 150, SliderIncrements.Ones);

            #endregion
            // ====================================================================================
            #region DARK KNIGHT
            if (preset == CustomComboPreset.DarkEoSPoolOption && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 3000, DRK.Config.DrkMPManagement, "How much MP to save (0 = Use All)");
            if (preset == CustomComboPreset.DarkPlungeFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, DRK.Config.DrkKeepPlungeCharges, "How many charges to keep ready? (0 = Use All)");
            #endregion
            // ====================================================================================
            #region DRAGOON

            #endregion
            // ====================================================================================
            #region GUNBREAKER
            if (preset == CustomComboPreset.GunbreakerRoughDivideFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, GNB.Config.GnbKeepRoughDivideCharges, "How many charges to keep ready? (0 = Use All)");
            #endregion
            // ====================================================================================
            #region MACHINIST

            #endregion
            // ====================================================================================
            #region MONK

            #endregion
            // ====================================================================================
            #region NINJA
            if (preset == CustomComboPreset.NinjaSimpleMudras)
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
            if (preset == CustomComboPreset.NinSimpleTrickFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 15, NIN.Config.TrickCooldownRemaining, "Set the amount of time in seconds for the feature to try and set up \nSuiton in advance of Trick Attack coming off cooldown");


            if (preset == CustomComboPreset.NinjaHuraijinFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 60, NIN.Config.HutonRemainingTimer, "Set the amount of time remaining on Huton the feature\nshould wait before using Huraijin", 200);


            if (preset == CustomComboPreset.NinAeolianMugFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, NIN.Config.MugNinkiGauge, $"Set the amount of Ninki to be at or under for this feature (level {NIN.TraitLevels.Shukiho} onwards)");

            if (preset == CustomComboPreset.NinjaArmorCrushOnMainCombo)
                ConfigWindowFunctions.DrawSliderInt(0, 30, NIN.Config.HutonRemainingArmorCrush, "Set the amount of time remaining on Huton the feature\nshould wait before using Armor Crush", 200);

            if (preset == CustomComboPreset.NinNinkiBhavacakraPooling)
                ConfigWindowFunctions.DrawSliderInt(50, 100, NIN.Config.NinkiBhavaPooling, "The minimum value of Ninki to have before spending.");

            if (preset == CustomComboPreset.NinNinkiBunshinPooling)
                ConfigWindowFunctions.DrawSliderInt(50, 100, NIN.Config.NinkiBunshinPooling, "The minimum value of Ninki to have before spending.");

            #endregion
            // ====================================================================================
            #region PALADIN
            //if (preset == CustomComboPreset.PaladinAtonementDropFeature && enabled)
            //    ConfigWindowFunctions.DrawSliderInt(2, 3, PLD.Config.PLDAtonementCharges, "How many Atonements to cast right before FoF (Atonement Drop)?");

            if (preset == CustomComboPreset.PaladinInterveneFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 1, PLD.Config.PLDKeepInterveneCharges, "How many charges to keep ready? (0 = Use All)");

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

            if (preset == CustomComboPreset.RPRPvPImmortalPoolingOption && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 8, RPRPVP.Config.RPRPvPImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.###RPR", 150, SliderIncrements.Ones);

            if (preset == CustomComboPreset.RPRPvPArcaneCircleOption && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 90, RPRPVP.Config.RPRPvPArcaneCircleOption, "Set a HP percentage value. Caps at 90 to prevent waste.###RPR", 150, SliderIncrements.Ones);

            #endregion
            // ====================================================================================
            #region RED MAGE

            if (preset == CustomComboPreset.RDM_OGCD)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_OGCD_OnAction, "Use on Fleche", "", 1);

            if (preset == CustomComboPreset.RDM_OGCD)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_OGCD_OnAction, "Use on Jolt/Jolt II only", "", 2);

            if (preset == CustomComboPreset.RDM_OGCD)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_OGCD_OnAction, "Use on Scatter/Impact only", "", 3);

            if (preset == CustomComboPreset.RDM_OGCD)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_OGCD_OnAction, "Use on Jolt/Jolt II & Scatter/Impact", "[Choose Jolt or Impact for a one button rotation]\n---------------------------------------------------------------", 4);

            if (preset == CustomComboPreset.RDM_ST_MeleeCombo)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "Use on Riposte", "", 1);

            if (preset == CustomComboPreset.RDM_ST_MeleeCombo)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "Use on Jolt/Jolt II", "", 2);

            if (preset == CustomComboPreset.RDM_ST_MeleeCombo)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_ST_MeleeCombo_OnAction, "Use on Riposte & Jolt/Jolt II", "[Choose Jolt or Impact for a one button rotation]\n---------------------------------------------------------------", 3);

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "Use on Riposte & Moulinet", "", 1);

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "Use on Jolt/Jolt II & Scatter/Impact", "", 2);

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "Use on Riposte, Moulinet, Jolt/Jolt II & Scatter/Impact", "", 3);

            if (preset == CustomComboPreset.RDM_MeleeFinisher)
                ConfigWindowFunctions.DrawRadioButton(RDM.Config.RDM_MeleeFinisher_OnAction, "Use on Veraero 1/2/3 and Verthunder 1/2/3", "[Choose Jolt or Impact for a one button rotation]\n---------------------------------------------------------------", 4);

            if (preset == CustomComboPreset.RDM_LucidDreaming && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 10000, RDM.Config.RDM_LucidDreaming_Threshold, "Add Lucid Dreaming when below this MP.", 300, SliderIncrements.Hundreds);

            #endregion
            // ====================================================================================
            #region SAGE

            if (preset == CustomComboPreset.SGE_ST_Dosis_EDosisHPPer)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Dosis_EDosisHPPer, "Enemy HP %% Threshold");

            if (preset == CustomComboPreset.SGE_ST_Dosis_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SGE.Config.SGE_ST_Dosis_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.SGE_ST_Dosis_Toxikon)
                ConfigWindowFunctions.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show when moving only", "", 1);

            if (preset == CustomComboPreset.SGE_ST_Dosis_Toxikon)
                ConfigWindowFunctions.DrawRadioButton(SGE.Config.SGE_ST_Dosis_Toxikon, "Show at all times", "", 2);

            if (preset == CustomComboPreset.SGE_ST_Heal_Soteria)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Soteria, "Set HP percentage value for Soteria to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Zoe)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Zoe, "Set HP percentage value for Zoe to trigger");

            if (preset is CustomComboPreset.SGE_ST_Heal_Pepsis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Pepsis, "Set HP percentage value for Pepsis to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Taurochole)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Taurochole, "Set HP percentage value for Taurochole to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Haima)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Haima, "Set HP percentage value for Haima to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Krasis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Krasis, "Set HP percentage value for Krasis to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Druochole)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Druochole, "Set HP percentage value for Druochole to trigger");

            if (preset == CustomComboPreset.SGE_ST_Heal_Diagnosis)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.SGE_ST_Heal_Diagnosis, "Set HP percentage value for Eukrasian Diagnosis to trigger");
            #endregion
            // ====================================================================================
            #region SAMURAI
            if (preset == CustomComboPreset.SamuraiOvercapFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 85, SAM.Config.SamKenkiOvercapAmount, "Set the Kenki overcap amount for ST combos.");
            if (preset == CustomComboPreset.SamuraiOvercapFeatureAoe && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 85, SAM.Config.SamAOEKenkiOvercapAmount, "Set the Kenki overcap amount for AOE combos.");
            //PVP
            if (preset == CustomComboPreset.SAMBurstMode && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 2, SAMPvP.Config.SamSotenCharges, "How many charges of Soten to keep ready? (0 = Use All).");
            if (preset == CustomComboPreset.SamGapCloserFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SAMPvP.Config.SamSotenHP, "Use Soten on enemies below selected HP.");
            //Fillers
            if (preset == CustomComboPreset.SamuraiFillersonMainCombo)
                ConfigWindowFunctions.DrawRadioButton(SAM.Config.SamFillerCombo, "2.14+", "2 Filler GCDs", 1);
            if (preset == CustomComboPreset.SamuraiFillersonMainCombo)
                ConfigWindowFunctions.DrawRadioButton(SAM.Config.SamFillerCombo, "2.06 - 2.08", "3 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range.\nHakaze will be delayed by half a GCD after Enpi.", 2);
            if (preset == CustomComboPreset.SamuraiFillersonMainCombo)
                ConfigWindowFunctions.DrawRadioButton(SAM.Config.SamFillerCombo, "1.99 - 2.01", "4 Filler GCDs. \nWill use Yaten into Enpi as part of filler and Gyoten back into Range. \nHakaze will be delayed by half a GCD after Enpi.", 3);
            #endregion
            // ====================================================================================
            #region MONK
            if (preset == CustomComboPreset.MnkBootshineCombo)
                ConfigWindowFunctions.DrawSliderInt(5, 10, MNK.Config.MnkDemolishApply, "Seconds remaining before refreshing Demolish.");

            if (preset == CustomComboPreset.MnkBootshineCombo)
                ConfigWindowFunctions.DrawSliderInt(5, 10, MNK.Config.MnkDisciplinedFistApply, "Seconds remaining before refreshing Disciplined Fist.");
            #endregion
            // ====================================================================================
            #region SCHOLAR
            if (preset == CustomComboPreset.SCH_ST_Broil_Lucid)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SCH.Config.SCH_ST_Broil_Lucid, "MP Threshold", 150, SliderIncrements.Hundreds);
            if (preset == CustomComboPreset.SCH_ST_Broil_BioHPPer)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SCH.Config.SCH_ST_Broil_BioHPPer, "Enemy HP %% Threshold");
            if (preset == CustomComboPreset.SCH_ST_Broil_ChainStratagem)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SCH.Config.SCH_ST_Broil_ChainStratagem, "Enemy HP%% Threshold");
            if (preset == CustomComboPreset.SCH_FairyFeature)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Eos", "", 1);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_FairyFeature, "Selene", "", 2);
            }
            if (preset == CustomComboPreset.SCH_Aetherflow_Recite_Excog)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Always when available", "", 1);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Excog, "Only when out of Aetherflow Stacks", "", 2);
            }
            if (preset == CustomComboPreset.SCH_Aetherflow_Recite_Indom)
            {
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Always when available", "", 1);
                ConfigWindowFunctions.DrawRadioButton(SCH.Config.SCH_Aetherflow_Recite_Indom, "Only when out of Aetherflow Stacks", "", 2);
            }
            #endregion
            // ====================================================================================
            #region SUMMONER

            if (preset == CustomComboPreset.BuffOnSimpleAoESummoner)
            {
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 1", "Use Searing Light on cooldown, regardless of phase.", 0);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 2", "Use Searing Light only in Bahamut phase.", 1);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 3", "Use Searing Light only in Phoenix phase.", 2);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 4", "Use Searing Light only in Ifrit phase.", 3);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 5", "Use Searing Light only in Garuda phase.", 4);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SMNSearingLightChoice, "Option 6", "Use Searing Light only in Titan phase.", 5);
            }

            if (preset == CustomComboPreset.SummonerEgiSummonsonMainFeature)
            {
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SummonerPrimalChoice, "Titan", "Summons Titan first, Garuda second, Ifrit third", 1);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SummonerPrimalChoice, "Garuda", "Summons Garuda first, Titan second, Ifrit third", 2);
            }

            if (preset == CustomComboPreset.SummonerPrimalBurstChoice)
            {
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SummonerBurstPhase, "Bahamut", "Burst during Bahamut Phase", 1);
                ConfigWindowFunctions.DrawRadioButton(SMN.Config.SummonerBurstPhase, "Phoenix", "Burst during Phoenix Phase", 2);
            }

            if (preset == CustomComboPreset.SMNLucidDreamingFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SMN.Config.SMNLucidDreamingFeature, "Set value for your MP to be at or under for this feature to work", 150, SliderIncrements.Hundreds);

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
            if (preset == CustomComboPreset.PVPEmergencyHeals)
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

            if (preset == CustomComboPreset.PVPEmergencyGuard)
                ConfigWindowFunctions.DrawSliderInt(1, 100, PVPCommon.Config.EmergencyGuardThreshold, "Set the percentage to be at or under for the feature to kick in.");

            if (preset == CustomComboPreset.PVPQuickPurify)
                ConfigWindowFunctions.DrawPvPStatusMultiChoice(PVPCommon.Config.QuickPurifyStatuses);

            #endregion

        }

    }
}
