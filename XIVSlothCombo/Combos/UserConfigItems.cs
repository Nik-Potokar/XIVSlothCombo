using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using System.Linq;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Combos.PvP;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Functions
{
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

            if (preset is CustomComboPreset.BLM_SimpleMode or CustomComboPreset.BLM_Simple_Transpose)
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
                int[]? actions = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                bool inputChanged = false;

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
            if (preset == CustomComboPreset.DRG_ST_Dives && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_ST_DiveOptions, "On Cooldown", "Single Weave friendly. Uses skills on cooldown.", 1);
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_ST_DiveOptions, "Under Battle Litany and Life of the Dragon", "Requires Double Weaving. Uses Spineshatter Dive and Dragonfire Dive under Battle Litany and Life of the Dragon, and Stardiver under Life of the Dragon.", 2);
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_ST_DiveOptions, "Under Lance Charge", "Single Weave friendly. Uses Spineshatter Dive and Dragonfire Dive under Lance Charge, and Stardiver under Life of the Dragon.", 3);
            }

            if (preset == CustomComboPreset.DRG_AoE_Dives && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_AOE_DiveOptions, "On Cooldown", "Single Weave friendly. Uses skills on cooldown.", 1);
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_AOE_DiveOptions, "Under Battle Litany and Life of the Dragon", "Requires Double Weaving. Uses Spineshatter Dive and Dragonfire Dive under Battle Litany and Life of the Dragon, and Stardiver under Life of the Dragon.", 2);
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_AOE_DiveOptions, "Under Lance Charge", "Single Weave friendly. Uses Spineshatter Dive and Dragonfire Dive under Lance Charge, and Stardiver under Life of the Dragon.", 3);
            }

            if (preset == CustomComboPreset.DRG_ST_Opener && enabled)
            {
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_OpenerOptions, "Standard Opener", "Uses the Standard Tincture Opener.", 1);
                UserConfig.DrawHorizontalRadioButton(DRG.Config.DRG_OpenerOptions, "Low Ping Opener", "Uses the Low Ping Opener. Use Lance Charge after True Thrust for the No Tincture opener.", 2);                
            }
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
                UserConfig.DrawSliderInt(0, 15, NIN.Config.Trick_CooldownRemaining, "Set the amount of time remaining on Trick Attack cooldown before trying to set up with Suiton.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_Bunshin)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPoolingST, "Set the amount of Ninki required to have before spending on Bunshin.");

            if (preset == CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin)
                UserConfig.DrawSliderInt(50, 100, NIN.Config.Ninki_BunshinPoolingAoE, "Set the amount of Ninki required to have before spending on Bunshin.");

            if (preset == CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Cooldowns)
                UserConfig.DrawSliderInt(0, 15, NIN.Config.Advanced_Trick_Cooldown, "Set the amount of time remaining on Trick Attack cooldown to start saving cooldowns.");

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

            if (preset == CustomComboPreset.RPR_Soulsow && enabled)
            {
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Harpe", "Adds Soulsow to Harpe.", 5, 0);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Slice", "Adds Soulsow to Slice.", 5, 1);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Spinning Scythe", "Adds Soulsow to Spinning Scythe", 5, 2);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Shadow of Death", "Adds Soulsow to Shadow of Death.", 5, 3);
                UserConfig.DrawHorizontalMultiChoice(RPR.Config.RPR_SoulsowOptions, "Blood Stalk", "Adds Soulsow to Blood Stalk.", 5, 4);
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

            if (preset is CustomComboPreset.SGE_ST_Dosis)
            {
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_ST_Dosis_AltMode), "On All Dosis Actions", "", 0);
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_ST_Dosis_AltMode), "On Dosis II", "Alternative DPS Mode. Leaves Dosis & Dosis III alone for normal DPS", 1);
            }

            if (preset is CustomComboPreset.SGE_ST_Dosis_EDosis)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Dosis_EDosisHPPer), "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SGE_ST_Dosis_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, nameof(SGE.Config.SGE_ST_Dosis_Lucid), "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Dosis_Toxikon)
            {
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_ST_Dosis_Toxikon), "Show when moving only", "", 0);
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_ST_Dosis_Toxikon), "Show at all times", "", 1);
            }

            if (preset is CustomComboPreset.SGE_AoE_Phlegma_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, nameof(SGE.Config.SGE_AoE_Phlegma_Lucid), "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SGE_ST_Heal_Soteria)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Soteria), "Use Soteria when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Zoe)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Zoe), "Use Zoe when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Pepsis)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Pepsis), "Use Pepsis when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Taurochole)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Taurochole), "Use Taurochole when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Haima)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Haima), "Use Haima when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Krasis)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Krasis), "Use Krasis when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Druochole)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Druochole), "Use Druochole when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_ST_Heal_Diagnosis)
                UserConfig.DrawSliderInt(0, 100, nameof(SGE.Config.SGE_ST_Heal_Diagnosis), "Use Diagnosis when Target HP is at or below set percentage");

            if (preset is CustomComboPreset.SGE_Eukrasia)
            {
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_Eukrasia_Mode), "Eukrasian Dosis", "", 0);
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_Eukrasia_Mode), "Eukrasian Diagnosis", "", 1);
                UserConfig.DrawRadioButton(nameof(SGE.Config.SGE_Eukrasia_Mode), "Eukrasian Prognosis", "", 2);
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
                UserConfig.DrawHorizontalRadioButton(SAM.Config.SAM_MeikyoChoice,"Use outside of combo chain" ,"Uses Meikyo Shisui outside of a combo chain.", 2);
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

            #endregion
            // ====================================================================================
            #region SCHOLAR

            if (preset is CustomComboPreset.SCH_DPS)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_ST_DPS_AltMode), "On Ruin I / Broils", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_ST_DPS_AltMode), "On Bio", "Alternative DPS Mode. Leaves Ruin I / Broil alone for pure DPS, becomes Ruin I / Broil when features are on cooldown", 1);
            }

            if (preset is CustomComboPreset.SCH_DPS_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, nameof(SCH.Config.SCH_ST_DPS_LucidOption), "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_DPS_Bio)
                UserConfig.DrawSliderInt(0, 100, nameof(SCH.Config.SCH_ST_DPS_BioOption), "Stop using at Enemy HP %. Set to Zero to disable this check");

            if (preset is CustomComboPreset.SCH_DPS_ChainStrat)
                UserConfig.DrawSliderInt(0, 100, nameof(SCH.Config.SCH_ST_DPS_ChainStratagemOption), "Stop using at Enemy HP %. Set to Zero to disable this check");
            
            if (preset is CustomComboPreset.SCH_DPS_EnergyDrain)
                UserConfig.DrawSliderInt(0, 10, nameof(SCH.Config.SCH_ST_DPS_EnergyDrain), "Time remaining in seconds");

            if (preset is CustomComboPreset.SCH_AoE_Lucid)
                UserConfig.DrawSliderInt(4000, 9500, nameof(SCH.Config.SCH_AoE_LucidOption), "MP Threshold", 150, SliderIncrements.Hundreds);

            if (preset is CustomComboPreset.SCH_FairyReminder)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_FairyFeature), "Eos", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_FairyFeature), "Selene", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Display), "Show Aetherflow On Energy Drain Only", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Display), "Show Aetherflow On All Aetherflow Skills", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Excog)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Recite_Excog), "Only when out of Aetherflow Stacks", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Recite_Excog), "Always when available", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Aetherflow_Recite_Indom)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Recite_Indom), "Only when out of Aetherflow Stacks", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Aetherflow_Recite_Indom), "Always when available", "", 1);
            }

            if (preset is CustomComboPreset.SCH_Recitation)
            {
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Recitation_Mode), "Adloquium", "", 0);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Recitation_Mode), "Succor", "", 1);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Recitation_Mode), "Indomitability", "", 2);
                UserConfig.DrawRadioButton(nameof(SCH.Config.SCH_Recitation_Mode), "Excogitation", "", 3);
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

            PlayerCharacter? pc = Service.ClientState.LocalPlayer;

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
                        int setting = PluginConfiguration.GetCustomIntValue(NINPVP.Config.NINPvP_Meisui_ST);
                        float hpThreshold = (float)maxHP / 100 * setting;

                        description += $"\nHP Value to be at or under: {hpThreshold}";
                    }
                }

                UserConfig.DrawSliderInt(1, 100, NINPVP.Config.NINPvP_Meisui_ST, description);
            }

            if (preset == CustomComboPreset.NINPvP_AoE_Meisui)
            {
                string description = "Set the HP percentage to be at or under for the feature to kick in.\n100% is considered to start at 8,000 less than your max HP to prevent wastage.";

                if (pc != null)
                {
                    uint maxHP = pc.MaxHp <= 8000 ? 0 : pc.MaxHp - 8000;
                    if (maxHP > 0)
                    {
                        int setting = PluginConfiguration.GetCustomIntValue(NINPVP.Config.NINPvP_Meisui_AoE);
                        float hpThreshold = (float)maxHP / 100 * setting;

                        description += $"\nHP Value to be at or under: {hpThreshold}";
                    }
                }

                UserConfig.DrawSliderInt(1, 100, NINPVP.Config.NINPvP_Meisui_AoE, description);
            }


            #endregion
        }
    }
}
