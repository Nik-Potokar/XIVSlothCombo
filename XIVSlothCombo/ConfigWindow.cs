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
        private readonly Vector4 shadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
        /// </summary>
        public ConfigWindow()
            : base("Sloth Combo Setup")
        {
            var p = Service.Configuration.AprilFoolsSlothIrl;

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
            ImGui.Columns(2, null, false);
            ImGui.Text("This window allows you to enable and disable custom combos to your liking.");

            ImGui.NextColumn();
            ImGui.TextColored(ImGuiColors.DalamudRed, $"NOTICE: We are still updating some jobs for 6.1 compatibility.\nBe patient and check the discord for a full status report!");
            ImGui.NextColumn();

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


            ImGui.NextColumn();
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGuiColors.HealerGreen);
            if (ImGui.Button("Click here to join our Discord Server!"))
            {
                Util.OpenLink("https://discord.gg/xT7zyjzjtY");
            }
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.Columns(1);
            var isAprilFools = DateTime.Now.Day == 1 && DateTime.Now.Month == 4 ? true : false;


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

            float offset = (float)Service.Configuration.MeleeOffset;

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
                ImGui.TextUnformatted("Offset of melee check distance for features that use it.\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.");
                ImGui.EndTooltip();
            }

            var slothIrl = isAprilFools ? Service.Configuration.AprilFoolsSlothIrl : false;
            if (isAprilFools)
            {

                if (ImGui.Checkbox("Sloth Mode!?", ref slothIrl))
                {
                    Service.Configuration.AprilFoolsSlothIrl = slothIrl;
                    Service.Configuration.Save();
                }
            }
            else
            {
                Service.Configuration.AprilFoolsSlothIrl = false;
                Service.Configuration.Save();
            }


            ImGui.BeginChild("scrolling", new Vector2(0, -30), true);

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

                            if (conflictsSource.Where(x => x == preset).Count() == 0 || conflictOriginals.Length == 0)
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

            
            if (ImGui.Button("Got an issue? Click this button and report it!"))
            {
                Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/issues");
            }

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

            if (irlsloth && !string.IsNullOrEmpty(info.MemeName))
            {
                if (ImGui.Checkbox(info.MemeName, ref enabled))
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
            }
            else
            {
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

            DrawUserConfigs(preset, enabled);
            
            i++;

            var hideChildren = Service.Configuration.HideChildren;
            if (enabled || !hideChildren)
            {
                var children = this.presetChildren[preset];
                if (children.Length > 0)
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

                            if (conflictsSource.Where(x => x == childPreset || x == preset).Count() == 0 || conflictOriginals.Length == 0)
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

        /// <summary>
        /// Draws the User Configurable settings.
        /// </summary>
        /// <param name="preset">The preset it's attached to</param>
        /// <param name="enabled">If it's enabled or not</param>
        private void DrawUserConfigs(CustomComboPreset preset, bool enabled)
        {
            //WARNING: IF USING SAME DESCRIPTION FOR YOUR SLIDER AS ANOTHER SLIDER, PLEASE ENSURE YOU USE APPEND ### PLUS AN ID FOR THE SLIDER EG. ###MYSLIDER OR ###THISSLIDER.

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
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, AST.Config.ASTLucidDreamingFeature, "Set value for your MP to be at or under for this feature to work###AST", 150, SliderIncrements.Hundreds);
           
            if (preset == CustomComboPreset.AstroEssentialDignity)
                ConfigWindowFunctions.DrawSliderInt(0, 100, AST.Config.AstroEssentialDignity, "Set percentage value");

            #endregion
            // ====================================================================================
            #region BLACK MAGE
            
            if (preset == CustomComboPreset.BlackAoEFoulOption && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 2, BLM.Config.BlmPolygotsStored, "Number of Polygot charges to store.\n(2 = Only use Polygot with Manafont)");

            #endregion
            // ====================================================================================
            #region BLUE MAGE

            #endregion
            // ====================================================================================
            #region BARD
            if (preset == CustomComboPreset.BardSimpleRagingJaws)
                ConfigWindowFunctions.DrawSliderFloat(3, 5, BRD.Config.RagingJawsRenewTime, "Remaining time (In seconds)");

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
            if (preset == CustomComboPreset.NinSimpleTrickFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 15, NIN.Config.TrickCooldownRemaining, "Set the amount of time in seconds for the feature to try and set up \nSuiton in advance of Trick Attack coming off cooldown");
            
            if (preset == CustomComboPreset.NinjaHuraijinFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 60, NIN.Config.HutonRemainingTimer, "Set the amount of time remaining on Huton the feature\nshould wait before using Huraijin", 200);
            
            if (preset == CustomComboPreset.NinAeolianMugFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, NIN.Config.MugNinkiGauge, "Set the amount of Ninki to be at or under for this feature (level 66 onwards)");
            
            if (preset == CustomComboPreset.NinjaArmorCrushOnMainCombo)
                ConfigWindowFunctions.DrawSliderInt(0, 100, NIN.Config.HutonRemainingArmorCrush, "Set the amount of time remaining on Huton the feature\nshould wait before using Armor Crush", 200);

            #endregion
            // ====================================================================================
            #region PALADIN
            if (preset == CustomComboPreset.PaladinAtonementFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(2, 3, PLD.Config.PLDAtonementCharges, "How many Atonements to cast right before FoF (Atonement Drop)?");
                    
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

            #endregion
            // ====================================================================================
            #region RED MAGE

            if (preset == CustomComboPreset.RedMageLucidOnJolt && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 10000, RDM.Config.RdmLucidMpThreshold, "Add Lucid Dreaming when below this MP.",300,100);
                
            #endregion
            // ====================================================================================
            #region SAGE
            if (preset == CustomComboPreset.SageDPSFeatureAdvTest)
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

            if (preset == CustomComboPreset.SageLucidFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SGE.Config.CustomSGELucidDreaming, "Set value for your MP to be at or under for this feature to work###SGE", 150, SliderIncrements.Hundreds);

            if (preset == CustomComboPreset.CustomSoteriaFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomSoteria, "Set HP percentage value for Soteria to trigger");
           
            if (preset == CustomComboPreset.CustomZoeFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomZoe, "Set HP percentage value for Zoe to trigger");

            if (preset == CustomComboPreset.CustomPepsisFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomPepsis, "Set HP percentage value for Pepsis to trigger");

            if (preset == CustomComboPreset.CustomTaurocholeFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomTaurochole, "Set HP percentage value for Taurochole to trigger");

            if (preset == CustomComboPreset.CustomHaimaFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomHaima, "Set HP percentage value for Haima to trigger");

            if (preset == CustomComboPreset.CustomKrasisFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomKrasis, "Set HP percentage value for Krasis to trigger");

            if (preset == CustomComboPreset.CustomDruocholeFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomDruochole, "Set HP percentage value for Druochole to trigger");
            
            if (preset == CustomComboPreset.CustomEukrasianDiagnosisFeature)
                ConfigWindowFunctions.DrawSliderInt(0, 100, SGE.Config.CustomDiagnosis, "Set HP percentage value for Eukrasian Diagnosis to trigger");


            #endregion
            // ====================================================================================
            #region SAMURAI

            #endregion
            // ====================================================================================
            #region SCHOLAR
            if (preset == CustomComboPreset.ScholarLucidDPSFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, SCH.Config.ScholarLucidDreaming, "Set value for your MP to be at or under for this feature to work###SCH", 150, SliderIncrements.Hundreds);
            
            
            #endregion
            // ====================================================================================
            #region SUMMONER

            #endregion
            // ====================================================================================
            #region WARRIOR
            if (preset == CustomComboPreset.WarriorInfuriateFellCleave)
                ConfigWindowFunctions.DrawSliderInt(0, 50, WAR.Config.WarInfuriateRange, "Set how much rage to be at or under to use this feature.");
                
            if (preset == CustomComboPreset.WarriorStormsPathCombo && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 30, WAR.Config.WarSurgingRefreshRange, "Seconds remaining before refreshing Surging Tempest.");
                
            if (preset == CustomComboPreset.WarriorOnslaughtFeature && enabled)
                ConfigWindowFunctions.DrawSliderInt(0, 2, WAR.Config.WarKeepOnslaughtCharges, "How many charges to keep ready? (0 = Use All)");

            #endregion
            // ====================================================================================
            #region WHITE MAGE
            if (preset == CustomComboPreset.WHMLucidDreamingFeature)
                ConfigWindowFunctions.DrawSliderInt(4000, 9500, WHM.Config.WHMLucidDreamingFeature, "Set value for your MP to be at or under for this feature to work###WHM", 150, SliderIncrements.Hundreds);
            
            #endregion
            // ====================================================================================
            #region DOH

            #endregion
            // ====================================================================================
            #region DOL

            #endregion
            // ====================================================================================
            #region PVP VALUES

            #endregion

        }
    }
}
