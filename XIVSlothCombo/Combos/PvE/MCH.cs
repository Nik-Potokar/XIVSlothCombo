using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class MCH
    {
        public const byte JobID = 31;

        internal const uint
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            GaussRound = 2874,
            Ricochet = 2890,
            HeatedSlugshot = 7412,
            Drill = 16498,
            HotShot = 2872,
            Reassemble = 2876,
            AirAnchor = 16500,
            Hypercharge = 17209,
            HeatBlast = 7410,
            SpreadShot = 2870,
            Scattergun = 25786,
            AutoCrossbow = 16497,
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
            Tactician = 16889,
            ChainSaw = 25788,
            BioBlaster = 16499,
            BarrelStabilizer = 7414,
            Wildfire = 2878,
            Dismantle = 2887;

        internal static class Buffs
        {
            internal const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946,
                Overheated = 2688;
        }

        internal static class Debuffs
        {
            internal const ushort
            Dismantled = 2887;
        }

        internal static class Config
        {
            internal const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",
                MCH_ST_QueenThreshold = "MCH_ST_QueenThreshold",
                MCH_ST_Simple_OpenerSelection = "MCH_ST_Simple_OpenerSelection",
                MCH_VariantCure = "MCH_VariantCure";
        }

        internal static class Levels
        {
            internal const byte
                SlugShot = 2,
                Hotshot = 4,
                GaussRound = 15,
                CleanShot = 26,
                Hypercharge = 30,
                HeatBlast = 35,
                RookOverdrive = 40,
                Wildfire = 45,
                Ricochet = 50,
                Drill = 58,
                AirAnchor = 76,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                Tactician = 56,
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                BioBlaster = 72,
                ChargedActionMastery = 74,
                QueenOverdrive = 80,
                Scattergun = 82,
                BarrelStabilizer = 66,
                ChainSaw = 90;
        }

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;

            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;


            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool openerReady = ActionReady(ChainSaw) && ActionReady(Wildfire) && ActionReady(BarrelStabilizer) && GetRemainingCharges(Ricochet) == 3 && GetRemainingCharges(GaussRound) == 3;
                float hpTreshold = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold);
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is SplitShot)
                {
                    if (level >= 90)
                    {
                        // Check to start opener
                        if (openerStarted && HasEffect(Buffs.Reassembled)) { inOpener = true; openerStarted = false; readyOpener = false; }
                        if ((readyOpener || openerStarted) && HasEffect(Buffs.Reassembled) && !inOpener) { openerStarted = true; return AirAnchor; } else { openerStarted = false; }

                        // Reset check for opener
                        if (openerReady && !InCombat() && !inOpener && !openerStarted)
                        {
                            readyOpener = true;
                            inOpener = false;
                            step = 0;
                            return Reassemble;
                        }
                        else
                        { readyOpener = false; }

                        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                        if ((step == 1 && IsOffCooldown(AirAnchor))
                            || (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat())) inOpener = false;


                        if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Reassembled) &&
                            IsEnabled(CustomComboPreset.MCH_ST_Advanced_Opener) && level >= 90 && openerReady)
                            inOpener = true;

                        if (inOpener)
                        {
                            //we do it in steps to be able to control it
                            if (step is 0)
                            {
                                if (IsOnCooldown(AirAnchor)) step++;
                                else return AirAnchor;
                            }

                            if (step == 1)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 2)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 3)
                            {
                                if (IsOnCooldown(Drill)) step++;
                                else return Drill;
                            }

                            if (step == 4)
                            {
                                if (IsOnCooldown(BarrelStabilizer)) step++;
                                else return BarrelStabilizer;
                            }

                            if (step == 5)
                            {
                                if (WasLastWeaponskill(HeatedSplitShot)) step++;
                                else return HeatedSplitShot;
                            }

                            if (step == 6)
                            {
                                if (WasLastWeaponskill(HeatedSlugshot)) step++;
                                else return HeatedSlugshot;
                            }

                            if (step == 7)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 8)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 9)
                            {
                                if (WasLastWeaponskill(HeatedCleanShot)) step++;
                                else return HeatedCleanShot;
                            }

                            if (step == 10)
                            {
                                if (GetRemainingCharges(Reassemble) is 0) step++;
                                else return Reassemble;
                            }

                            if (step == 11)
                            {
                                if (IsOnCooldown(Wildfire)) step++;
                                else return Wildfire;
                            }

                            if (step == 12)
                            {
                                if (IsOnCooldown(ChainSaw)) step++;
                                else return ChainSaw;
                            }

                            if (step == 13)
                            {
                                if (IsOnCooldown(AutomatonQueen)) step++;
                                else return AutomatonQueen;
                            }

                            if (step == 14)
                            {
                                if (IsOnCooldown(Hypercharge)) step++;
                                else return Hypercharge;
                            }

                            if (step == 15)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 4) step++;
                                else return HeatBlast;
                            }

                            if (step == 16)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 17)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 3) step++;
                                else return HeatBlast;
                            }

                            if (step == 18)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 19)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 2) step++;
                                else return HeatBlast;
                            }

                            if (step == 20)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 21)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 1) step++;
                                else return HeatBlast;
                            }

                            if (step == 22)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 23)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 0) step++;
                                else return HeatBlast;
                            }

                            if (step == 24)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 25)
                            {
                                if (WasLastAction(Drill)) step++;
                                else return Drill;
                            }

                            inOpener = false;
                        }
                    }

                    if (!inOpener)
                    {
                        //gauss and ricochet overcap protection
                        if (CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire))
                        {
                            if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                                return GaussRound;

                            if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                                return Ricochet;
                        }

                        // Interrupt
                        if (CanWeave(actionID) && CanInterruptEnemy() && ActionReady(All.HeadGraze))
                            return All.HeadGraze;

                        //queen
                        if (!gauge.IsOverheated && LevelChecked(OriginalHook(RookAutoturret)))
                        {
                            if (level >= 90)
                            {
                                if (CombatEngageDuration().Minutes == 1 && gauge.Battery == 70 && ActionReady(ChainSaw))
                                    return OriginalHook(RookAutoturret);

                                if (CombatEngageDuration().Minutes % 2 == 0 && gauge.Battery == 100 && WasLastAction(Drill))
                                    return OriginalHook(RookAutoturret);

                                if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Battery >= 80 && gauge.Battery <= 90 && ActionReady(ChainSaw))
                                    return OriginalHook(RookAutoturret);
                            }

                            if (level >= 80 && gauge.Battery == 100)
                                return OriginalHook(RookAutoturret);

                            if (level < 80 && gauge.Battery == 80)
                                return OriginalHook(RookAutoturret);
                        }

                        // OGCD's
                        if ((ActionReady(ChainSaw) || ActionReady(AirAnchor) || (!LevelChecked(AirAnchor) && ActionReady(Drill))) &&
                            !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                            return Reassemble;

                        if (ActionReady(ChainSaw))
                            return ChainSaw;

                        if (ActionReady(OriginalHook(AirAnchor)))
                            return OriginalHook(AirAnchor);

                        if (ActionReady(Drill))
                            return Drill;

                        // BarrelStabelizer use
                        if (CanWeave(actionID) && gauge.Heat <= 55 && ActionReady(BarrelStabilizer) && ActionReady(Wildfire))
                            return BarrelStabilizer;

                        // Wildfire
                        if (gauge.Heat >= 50 && ActionReady(Wildfire) &&
                            WasLastAction(ChainSaw) && CanWeave(actionID) && CombatEngageDuration().Minutes % 2 == 0)
                            return Wildfire;

                        // Hypercharge
                        if (gauge.Heat >= 50 && ActionReady(Hypercharge) && !gauge.IsOverheated)
                        {
                            //Protection & ensures Hyper charged is double weaved with WF during reopener
                            if (HasEffect(Buffs.Wildfire) || !LevelChecked(Wildfire))
                                return Hypercharge;

                            if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                            {
                                if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                {
                                    if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                        return Hypercharge;

                                    else if (!LevelChecked(ChainSaw))
                                        return Hypercharge;
                                }

                                else if (!LevelChecked(AirAnchor))
                                    return Hypercharge;
                            }

                            else if (!LevelChecked(Drill))
                                return Hypercharge;
                        }

                        //Heatblast, Gauss, Rico
                        if (gauge.IsOverheated && LevelChecked(HeatBlast))
                        {
                            if (WasLastAction(HeatBlast) && CanWeave(actionID))
                            {
                                if (GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                    return GaussRound;

                                if (GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound))
                                    return Ricochet;
                            }
                            return HeatBlast;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_SecondWind) &&
                        CanWeave(actionID) && PlayerHealthPercentageHp() <= hpTreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return (!LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                ? Reassemble
                                : OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);

                }

                return actionID;
            }
        }

        internal class MCH_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_AdvancedMode;

            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool openerReady = ActionReady(ChainSaw) && ActionReady(Wildfire) && ActionReady(BarrelStabilizer) && GetRemainingCharges(Ricochet) == 3 && GetRemainingCharges(GaussRound) == 3;
                float wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                float hpTreshold = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold);
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is SplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Opener) && level >= 90)
                    {
                        // Check to start opener
                        if (openerStarted && WasLastAction(HeatedSplitShot))
                        { inOpener = true; openerStarted = false; readyOpener = false; }

                        if ((readyOpener || openerStarted) && WasLastAction(HeatedSplitShot) && !inOpener)
                        { openerStarted = true; return GaussRound; }
                        else
                        { openerStarted = false; }

                        // Reset check for opener
                        if (openerReady && !InCombat() && !inOpener && !openerStarted)
                        {
                            readyOpener = true;
                            inOpener = false;
                            step = 0;
                            return HeatedSplitShot;
                        }
                        else
                        { readyOpener = false; }

                        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                        if ((step == 1 && WasLastAction(HeatedSplitShot)) ||
                            (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                            inOpener = false;

                        if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && WasLastAction(HeatedSplitShot) &&
                            IsEnabled(CustomComboPreset.MCH_ST_Advanced_Opener) && level >= 90 && openerReady)
                            inOpener = true;

                        if (inOpener)
                        {
                            //we do it in steps to be able to control it
                            if (step == 0)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 1)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 2)
                            {
                                if (IsOnCooldown(Drill)) step++;
                                else return Drill;
                            }

                            if (step == 3)
                            {
                                if (IsOnCooldown(BarrelStabilizer)) step++;
                                else return BarrelStabilizer;
                            }

                            if (step == 4)
                            {
                                if (WasLastWeaponskill(HeatedSlugshot)) step++;
                                else return HeatedSlugshot;
                            }

                            if (step == 5)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 6)
                            {
                                if (WasLastWeaponskill(HeatedCleanShot)) step++;
                                else return HeatedCleanShot;
                            }

                            if (step == 7)
                            {
                                if (GetRemainingCharges(Reassemble) is 1) step++;
                                else return Reassemble;
                            }

                            if (step == 8)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 9)
                            {
                                if (IsOnCooldown(AirAnchor)) step++;
                                else return AirAnchor;
                            }

                            if (step == 10)
                            {
                                if (GetRemainingCharges(Reassemble) is 0) step++;
                                else return Reassemble;
                            }

                            if (step == 11)
                            {
                                if (IsOnCooldown(Wildfire)) step++;
                                else return Wildfire;
                            }

                            if (step == 12)
                            {
                                if (IsOnCooldown(ChainSaw)) step++;
                                else return ChainSaw;
                            }

                            if (step == 13)
                            {
                                if (IsOnCooldown(AutomatonQueen)) step++;
                                else return AutomatonQueen;
                            }

                            if (step == 14)
                            {
                                if (IsOnCooldown(Hypercharge)) step++;
                                else return Hypercharge;
                            }

                            if (step == 15)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 4) step++;
                                else return HeatBlast;
                            }

                            if (step == 16)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 17)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 3) step++;
                                else return HeatBlast;
                            }

                            if (step == 18)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 19)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 2) step++;
                                else return HeatBlast;
                            }

                            if (step == 20)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 21)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 1) step++;
                                else return HeatBlast;
                            }

                            if (step == 22)
                            {
                                if (WasLastAction(GaussRound)) step++;
                                else return GaussRound;
                            }

                            if (step == 23)
                            {
                                if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 0) step++;
                                else return HeatBlast;
                            }

                            if (step == 24)
                            {
                                if (WasLastAction(Ricochet)) step++;
                                else return Ricochet;
                            }

                            if (step == 25)
                            {
                                if (WasLastAction(Drill)) step++;
                                else return Drill;
                            }

                            inOpener = false;
                        }
                    }

                    if (!inOpener)
                    {
                        //gauss and ricochet overcap protection
                        if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_GaussRicochet) &&
                            CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire))
                        {
                            if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                                return GaussRound;

                            if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                                return Ricochet;
                        }

                        // Interrupt
                        if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Interrupt) &&
                            CanWeave(actionID) && CanInterruptEnemy() && ActionReady(All.HeadGraze))
                            return All.HeadGraze;

                        //queen
                        if (IsEnabled(CustomComboPreset.MCH_Advanced_QueenUsage) && !gauge.IsOverheated && LevelChecked(OriginalHook(RookAutoturret)))
                        {
                            if (level >= 90)
                            {
                                if (CombatEngageDuration().Minutes == 1 && gauge.Battery >= 50)
                                    return OriginalHook(RookAutoturret);

                                if (CombatEngageDuration().Minutes % 2 == 0 && gauge.Battery == 100)
                                    return OriginalHook(RookAutoturret);

                                if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Battery >= 80)
                                    return OriginalHook(RookAutoturret);
                            }

                            if (level >= 80 && level < 90 && gauge.Battery == 100)
                                return OriginalHook(RookAutoturret);

                            if (level < 80 && gauge.Battery == 80)
                                return OriginalHook(RookAutoturret);
                        }

                        // BarrelStabelizer use
                        if (CanWeave(actionID) && gauge.Heat <= 45 && IsEnabled(CustomComboPreset.MCH_ST_Advanced_Stabilizer) &&
                            ((IsNotEnabled(CustomComboPreset.MCH_ST_Advanced_Stabilizer_Wildfire_Only) && ActionReady(BarrelStabilizer)) ||
                            (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Stabilizer_Wildfire_Only) && ActionReady(BarrelStabilizer) && ActionReady(Wildfire))))
                            return BarrelStabilizer;

                        // Wildfire
                        if (gauge.Heat >= 50 && ActionReady(Wildfire) &&
                            IsEnabled(CustomComboPreset.MCH_ST_Advanced_WildFire))
                        {
                            if (HasEffect(Buffs.Reassembled) && IsOffCooldown(ChainSaw) &&
                                CanDelayedWeave(actionID))
                                return Wildfire;

                            return Wildfire;
                        }

                        // Hypercharge
                        if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Hypercharge) &&
                            gauge.Heat >= 50 && ActionReady(Hypercharge) && !gauge.IsOverheated && (GetCooldownRemainingTime(Wildfire) >= 9 || !LevelChecked(Wildfire)))
                        {
                            //Protection & ensures Hyper charged is double weaved with WF during reopener
                            if (HasEffect(Buffs.Wildfire) || !LevelChecked(Wildfire))
                                return Hypercharge;

                            if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                            {
                                if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                {
                                    if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                        return Hypercharge;

                                    else if (!LevelChecked(ChainSaw))
                                        return Hypercharge;
                                }

                                else if (!LevelChecked(AirAnchor))
                                    return Hypercharge;
                            }

                            else if (!LevelChecked(Drill))
                                return Hypercharge;
                        }

                        //Heatblast, Gauss, Rico
                        if (gauge.IsOverheated && LevelChecked(HeatBlast))
                        {
                            if (WasLastAction(HeatBlast) && CanWeave(actionID))
                            {
                                if (GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                    return GaussRound;

                                if (GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound))
                                    return Ricochet;
                            }
                            return HeatBlast;
                        }

                        // OGCD's
                        if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Reassembled) && !HasEffect(Buffs.Wildfire) &&
                            (ActionReady(ChainSaw) || ActionReady(AirAnchor) || (!LevelChecked(AirAnchor) && ActionReady(Drill))) &&
                            !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                            return Reassemble;

                        if (ActionReady(ChainSaw) && IsEnabled(CustomComboPreset.MCH_ST_Advanced_ChainSaw))
                            return ChainSaw;

                        if (ActionReady(AirAnchor) && IsEnabled(CustomComboPreset.MCH_ST_Advanced_AirAnchor))
                            return AirAnchor;

                        if (ActionReady(Drill) && IsEnabled(CustomComboPreset.MCH_ST_Advanced_Drill))
                            return Drill;

                        if (!LevelChecked(AirAnchor) && ActionReady(HotShot) && IsEnabled(CustomComboPreset.MCH_ST_Advanced_AirAnchor))
                            return HotShot;
                    }
                    // healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Advanced_SecondWind) &&
                        CanWeave(actionID, 0.6) && PlayerHealthPercentageHp() <= hpTreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return (IsEnabled(CustomComboPreset.MCH_ST_Advanced_Reassembled) &&
                                !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                ? Reassemble
                                : OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);

                }

                return actionID;
            }
        }

        internal class MCH_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpreadShot)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (!gauge.IsOverheated)
                    {
                        if (gauge.Battery == 100)
                            return OriginalHook(RookAutoturret);
                    }

                    //gauss and ricochet overcap protection
                    if (CanWeave(actionID) && !gauge.IsOverheated)
                    {
                        if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                            return Ricochet;
                    }

                    // Hypercharge        
                    if (gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (gauge.IsOverheated && LevelChecked(AutoCrossbow))
                    {
                        if (WasLastAction(AutoCrossbow) && CanWeave(actionID))
                        {
                            if (GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                return GaussRound;

                            if (GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound))
                                return Ricochet;
                        }
                        return AutoCrossbow;
                    }

                    if (ActionReady(BioBlaster) && !HasEffect(Buffs.Overheated) && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster))
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_SecondWindThreshold) && ActionReady(All.SecondWind))
                            return All.SecondWind;
                    }
                }

                return actionID;
            }
        }

        internal class MCH_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpreadShot)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Queen) && !gauge.IsOverheated)
                    {
                        if (gauge.Battery == 100)
                            return OriginalHook(RookAutoturret);
                    }

                    //gauss and ricochet overcap protection
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Always_Gauss_Ricochet) &&
                        CanWeave(actionID) && !gauge.IsOverheated)
                    {
                        if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                            return Ricochet;
                    }

                    // Hypercharge        
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Advanced_Hypercharge) &&
                        gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Advanced_Hypercharge) &&
                        gauge.IsOverheated && LevelChecked(AutoCrossbow))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && WasLastAction(AutoCrossbow) && CanWeave(actionID))
                        {
                            if (GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                return GaussRound;

                            if (GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound))
                                return Ricochet;
                        }
                        return AutoCrossbow;
                    }

                    if (ActionReady(BioBlaster) && !HasEffect(Buffs.Overheated) && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster))
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Advanced_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_SecondWindThreshold) && ActionReady(All.SecondWind))
                            return All.SecondWind;
                    }
                }

                return actionID;
            }
        }

        internal class MCH_HeatblastGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HeatblastGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeatBlast)
                {
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbowGaussRicochet_AutoBarrel)
                        && ActionReady(BarrelStabilizer)
                        && gauge.Heat < 50
                        && !HasEffect(Buffs.Overheated))
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Wildfire)
                        && IsOffCooldown(Hypercharge)
                        && ActionReady(Wildfire)
                        && gauge.Heat >= 50)
                        return Wildfire;

                    if (!HasEffect(Buffs.Overheated) && LevelChecked(Hypercharge))
                        return Hypercharge;

                    if (GetCooldownRemainingTime(HeatBlast) < 0.7 && LevelChecked(HeatBlast)) // Prioritize Heat Blast
                        return HeatBlast;

                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    return Ricochet;
                }

                return actionID;
            }
        }

        internal class MCH_GaussRoundRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_GaussRoundRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is GaussRound or Ricochet)
                {
                    var gaussCd = GetCooldown(GaussRound);
                    var ricochetCd = GetCooldown(Ricochet);

                    // Prioritize the original if both are off cooldown
                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (!gaussCd.IsCooldown && !ricochetCd.IsCooldown)
                        return actionID;

                    if (gaussCd.CooldownRemaining < ricochetCd.CooldownRemaining)
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }

        internal class MCH_Overdrive : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RookAutoturret or AutomatonQueen)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();
                    if (gauge.IsRobotActive)
                        return OriginalHook(QueenOverdrive);
                }

                return actionID;
            }
        }

        internal class MCH_HotShotDrillChainSaw : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainSaw;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Drill || actionID == HotShot || actionID == AirAnchor)
                {
                    if (LevelChecked(ChainSaw))
                        return CalcBestAction(actionID, ChainSaw, AirAnchor, Drill);

                    if (LevelChecked(AirAnchor))
                        return CalcBestAction(actionID, AirAnchor, Drill);

                    if (LevelChecked(Drill))
                        return CalcBestAction(actionID, Drill, HotShot);

                    return HotShot;
                }

                return actionID;
            }
        }

        internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbowGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is AutoCrossbow)
                {
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbowGaussRicochet_AutoBarrel)
                        && ActionReady(BarrelStabilizer)
                        && gauge.Heat < 50
                        && !HasEffect(Buffs.Overheated)
                       ) return BarrelStabilizer;

                    if (!HasEffect(Buffs.Overheated) && ActionReady(Hypercharge))
                        return Hypercharge;
                    if (heatBlastCD.CooldownRemaining < 0.7 && LevelChecked(AutoCrossbow)) // prioritize autocrossbow
                        return AutoCrossbow;
                    if (!LevelChecked(Ricochet))
                        return GaussRound;
                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }

        internal class All_PRanged_Dismantle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.All_PRanged_Dismantle;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Dismantle)
                    if (TargetHasEffectAny(Debuffs.Dismantled) && IsOffCooldown(Dismantle))
                        return BLM.Fire;

                return actionID;
            }
        }
    }
}
