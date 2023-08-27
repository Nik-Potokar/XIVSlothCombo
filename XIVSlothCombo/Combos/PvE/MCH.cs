using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

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
            public static UserInt
                MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold"),
                MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold"),
                MCH_ST_QueenThreshold = new("MCH_ST_QueenThreshold"),
                MCH_ST_OpenerSelection = new("MCH_ST_OpenerSelection"),
                MCH_ST_RotationSelection = new("MCH_ST_RotationSelection"),
                MCH_VariantCure = new("MCH_VariantCure");
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
                            IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener) && level >= 90 && openerReady)
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
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                int openerSelection = Config.MCH_ST_OpenerSelection;
                int rotationSelection = Config.MCH_ST_RotationSelection;

                if (actionID is SplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener) && level >= 90)
                    {
                        //Standard opener
                        if (openerSelection is 0)
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
                                IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener) && level >= 90 && openerReady)
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

                        //123Tools Opener
                        if (openerSelection is 1)
                        {
                            // Check to start opener
                            if (openerStarted && WasLastAction(HeatedSplitShot) && gauge.Heat is 5 && InCombat() && GetRemainingCharges(GaussRound) is 3)
                            {
                                inOpener = true;
                                openerStarted = false;
                                readyOpener = false;
                            }

                            if ((readyOpener || openerStarted) &&
                                WasLastAction(HeatedSplitShot) && !inOpener)
                            {
                                openerStarted = true;
                                return GaussRound;
                            }

                            else
                            {
                                openerStarted = false;
                            }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return HeatedSplitShot;
                            }

                            else
                            {
                                readyOpener = false;
                            }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step is 1 && WasLastAction(GaussRound) && gauge.Heat > 5) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                                inOpener = false;

                            if (inOpener)
                            {

                                if (step == 0)
                                {
                                    if (WasLastAbility(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 1)
                                {
                                    if (WasLastAbility(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 2)
                                {
                                    if (WasLastWeaponskill(HeatedSlugshot)) step++;
                                    else return HeatedSlugshot;
                                }

                                if (step == 3)
                                {
                                    if (IsOnCooldown(BarrelStabilizer)) step++;
                                    else return BarrelStabilizer;
                                }

                                if (step == 4)
                                {
                                    if (WasLastWeaponskill(HeatedCleanShot)) step++;
                                    else return HeatedCleanShot;
                                }

                                if (step == 5)
                                {
                                    if (IsOnCooldown(AirAnchor)) step++;
                                    else return AirAnchor;
                                }

                                if (step == 6)
                                {
                                    if (GetRemainingCharges(Reassemble) == 1) step++;
                                    else return Reassemble;
                                }

                                if (step == 7)
                                {
                                    if (GetRemainingCharges(GaussRound) == 1) step++;
                                    else return GaussRound;
                                }

                                if (step == 8)
                                {
                                    if (IsOnCooldown(Drill)) step++;
                                    else return Drill;
                                }

                                if (step == 9)
                                {
                                    if (GetRemainingCharges(Reassemble) == 0) step++;
                                    else return Reassemble;
                                }

                                if (step == 10)
                                {
                                    if (IsOnCooldown(Wildfire)) step++;
                                    else return Wildfire;
                                }

                                if (step == 11)
                                {
                                    if (IsOnCooldown(ChainSaw)) step++;
                                    else return ChainSaw;
                                }

                                if (step == 12)
                                {
                                    if (WasLastAbility(AutomatonQueen)) step++;
                                    else return AutomatonQueen;
                                }

                                if (step == 13)
                                {
                                    if (WasLastAbility(Hypercharge)) step++;
                                    else return Hypercharge;
                                }

                                if (step == 14)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 15)
                                {
                                    if (WasLastAbility(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 16)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 17)
                                {
                                    if (WasLastAbility(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 18)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 19)
                                {
                                    if (WasLastAbility(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 20)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 21)
                                {
                                    if (WasLastAbility(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 22)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 23)
                                {
                                    if (WasLastAbility(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 24)
                                {
                                    if (WasLastWeaponskill(HeatedSplitShot)) step++;
                                    else return HeatedSplitShot; inOpener = false;
                                }

                            }
                        }

                        //Early Tools Opener
                        if (openerSelection is 2)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Reassembled))
                            {
                                inOpener = true;
                                openerStarted = false;
                                readyOpener = false;
                            }

                            if ((readyOpener || openerStarted) &&
                                HasEffect(Buffs.Reassembled) && !inOpener)
                            {
                                openerStarted = true;
                                return AirAnchor;
                            }
                            else
                            {
                                openerStarted = false;
                            }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return Reassemble;
                            }
                            else
                            {
                                readyOpener = false;
                            }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 0 && IsOnCooldown(AirAnchor) && !HasEffect(Buffs.Reassembled)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                                inOpener = false;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (IsOnCooldown(AirAnchor)) step++;
                                    else return AirAnchor;
                                }

                                if (step == 1)
                                {
                                    if (WasLastAction(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 2)
                                {
                                    if (WasLastAction(GaussRound)) step++;
                                    else return GaussRound;
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
                                    if (WasLastAction(Reassemble)) step++;
                                    else return Reassemble;
                                }

                                if (step == 6)
                                {
                                    if (IsOnCooldown(ChainSaw)) step++;
                                    else return ChainSaw;
                                }

                                if (step == 7)
                                {
                                    if (WasLastAction(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 8)
                                {
                                    if (WasLastAction(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 9)
                                {
                                    if (WasLastAction(HeatedSplitShot)) step++;
                                    else return HeatedSplitShot;
                                }

                                if (step == 10)
                                {
                                    if (WasLastAction(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 11)
                                {
                                    if (WasLastAction(HeatedSlugshot)) step++;
                                    else return HeatedSlugshot;
                                }

                                if (step == 12)
                                {
                                    if (WasLastAction(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 13)
                                {
                                    if (IsOnCooldown(Wildfire)) step++;
                                    else return Wildfire;
                                }

                                if (step == 14)
                                {
                                    if (WasLastAction(HeatedCleanShot)) step++;
                                    else return HeatedCleanShot;
                                }

                                if (step == 15)
                                {
                                    if (WasLastAction(AutomatonQueen)) step++;
                                    else return AutomatonQueen;
                                }

                                if (step == 16)
                                {
                                    if (WasLastAction(Hypercharge)) step++;
                                    else return Hypercharge;
                                }

                                if (step == 17)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 18)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 19)
                                {
                                    if (WasLastAction(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 20)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 21)
                                {
                                    if (WasLastAction(GaussRound)) step++;
                                    else return GaussRound;
                                }

                                if (step == 22)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 23)
                                {
                                    if (WasLastAction(Ricochet)) step++;
                                    else return Ricochet;
                                }

                                if (step == 24)
                                {
                                    if (WasLastAction(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                inOpener = false;
                            }
                        }
                    }

                    if (!inOpener)
                    {
                        //Standard Rotation
                        if (rotationSelection is 0)
                        {
                            //gauss and ricochet overcap protection
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                                CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire))
                            {
                                if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                                    return GaussRound;

                                if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                                    return Ricochet;
                            }

                            // Interrupt
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Interrupt) &&
                                CanWeave(actionID) && CanInterruptEnemy() && ActionReady(All.HeadGraze))
                                return All.HeadGraze;

                            //queen
                            if (IsEnabled(CustomComboPreset.MCH_Adv_QueenUsage) && !gauge.IsOverheated && LevelChecked(OriginalHook(RookAutoturret)))
                            {
                                if (level >= 90)
                                {
                                    // First condition
                                    if (gauge.Battery is 50 && CombatEngageDuration().TotalSeconds > 61 && CombatEngageDuration().TotalSeconds < 68)
                                        return OriginalHook(RookAutoturret);

                                    // Second condition
                                    if (gauge.Battery is 100 && gauge.LastSummonBatteryPower == 50 &&
                                        (GetCooldownRemainingTime(AirAnchor) <= 3 || ActionReady(AirAnchor)))
                                        return OriginalHook(RookAutoturret);

                                    // Third condition
                                    while (gauge.LastSummonBatteryPower is 100 && gauge.Battery >= 80)
                                        return OriginalHook(RookAutoturret);

                                    // Fourth condition
                                    while (gauge.LastSummonBatteryPower != 50 && gauge.Battery is 100 && (GetCooldownRemainingTime(AirAnchor) <= 3 || ActionReady(AirAnchor)))
                                        return OriginalHook(RookAutoturret);
                                }
                                else if (LevelChecked(RookOverdrive) && gauge.Battery >= 50)
                                    return OriginalHook(RookAutoturret);
                            }

                            // BarrelStabelizer use
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer) &&
                                gauge.Heat <= 55 && ActionReady(BarrelStabilizer) & !WasLastWeaponskill(ChainSaw) &&
                                ((wildfireCDTime <= 9 && IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer_Wildfire_Only)) ||
                                (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer_Wildfire_Only) && gauge.IsOverheated)))
                                return BarrelStabilizer;

                            // Wildfire
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_WildFire) &&
                                (gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && LevelChecked(Wildfire) &&
                                (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast)))) //these try to ensure the correct loops
                            {
                                if (CanDelayedWeave(actionID))
                                {
                                    if (!gauge.IsOverheated && IsOnCooldown(AirAnchor) && HasEffect(Buffs.Reassembled)) //WF EVEN BURST
                                        return Wildfire;

                                    else if (gauge.IsOverheated && level < 90)
                                        return Wildfire;
                                }
                            }

                            // Hypercharge
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Hypercharge) &&
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
                            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassembled) && !HasEffect(Buffs.Wildfire) &&
                                (ActionReady(ChainSaw) || ActionReady(AirAnchor) || (!LevelChecked(AirAnchor) && ActionReady(Drill))) &&
                                !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                return Reassemble;

                            if (ActionReady(ChainSaw) && IsEnabled(CustomComboPreset.MCH_ST_Adv_ChainSaw))
                                return ChainSaw;

                            if (ActionReady(AirAnchor) && IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor))
                                return AirAnchor;

                            if (ActionReady(Drill) && IsEnabled(CustomComboPreset.MCH_ST_Adv_Drill))
                                return Drill;

                            if (!LevelChecked(AirAnchor) && ActionReady(HotShot) && IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor))
                                return HotShot;
                        }

                        //123Tools Rotation
                        if (rotationSelection is 1 && level >= 90)
                        {
                            //Barrel Stabilizer
                            if (CanWeave(actionID) && gauge.Heat <= 55 && ActionReady(BarrelStabilizer))
                                return BarrelStabilizer;

                            //Wildfire stuff
                            //these try to ensure the correct loop, HC > CS > WF
                            if (IsOffCooldown(Wildfire))
                            {
                                if (CanDelayedWeave(actionID, 0.8) && gauge.IsOverheated && WasLastWeaponskill(ChainSaw))
                                    return Wildfire;

                                else if (CanWeave(actionID) && gauge.IsOverheated)
                                    return Wildfire;
                            }

                            //Queen aka Robot
                            if (CanWeave(actionID) && !gauge.IsRobotActive && (!WasLastAbility(Wildfire)))
                            {
                                // First condition
                                if (gauge.Battery == 50 && CombatEngageDuration().TotalSeconds > 61 && CombatEngageDuration().TotalSeconds < 68)
                                    return AutomatonQueen;

                                // Second condition
                                if (!WasLastAction(OriginalHook(CleanShot)))
                                {
                                    if (gauge.Battery == 100 && gauge.LastSummonBatteryPower == 50 && (GetCooldownRemainingTime(AirAnchor) <= 3 || IsOffCooldown(AirAnchor)))
                                        return AutomatonQueen;
                                }

                                // Third condition
                                while (gauge.LastSummonBatteryPower == 100 && gauge.Battery >= 90) //was previously 80 with 30 overcap for 10mins
                                    return AutomatonQueen;

                                // Fourth condition
                                while (gauge.LastSummonBatteryPower != 50 && gauge.Battery == 100 && (GetCooldownRemainingTime(AirAnchor) <= 3 || IsOffCooldown(AirAnchor)))
                                    return AutomatonQueen;
                            }

                            //Overheated Reassemble & Heatblast & GaussRico featuring a small ChainSaw addendum
                            if (gauge.IsOverheated && LevelChecked(HeatBlast))
                            {
                                if (CanWeave(actionID, 0.6) && wildfireCDTime > 2) //is this why its been breaking?
                                {

                                    if (HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                        return GaussRound;

                                    else if (ActionReady(Ricochet))
                                        return Ricochet;
                                }

                                if (GetCooldownRemainingTime(ChainSaw) <= 1 || (IsOffCooldown(ChainSaw) && (wildfireCDTime < 3 || IsOffCooldown(Wildfire))))
                                    return ChainSaw;

                                return HeatBlast;
                            }

                            //HYPERCHARGE!!
                            if (gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                            {
                                //Protection & ensures Hyper charged is double weaved with WF during reopener
                                //if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                                if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                                {
                                    if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                    {
                                        if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) <= 2 && (wildfireCDTime <= 4 || IsOffCooldown(Wildfire)))
                                        {
                                            if (CanDelayedWeave(actionID) && UseHypercharge123Tools(gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                        else if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                        {
                                            if (CanWeave(actionID) && UseHypercharge123Tools(gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                        else if (!LevelChecked(ChainSaw))
                                        {
                                            if (CanWeave(actionID) && UseHypercharge123Tools(gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                    }
                                    else if (!LevelChecked(AirAnchor))
                                    {
                                        if (CanWeave(actionID) && UseHypercharge123Tools(gauge, wildfireCDTime))
                                            return Hypercharge;
                                    }
                                }
                                else if (!LevelChecked(Drill))
                                {
                                    if (CanWeave(actionID) && UseHypercharge123Tools(gauge, wildfireCDTime))
                                        return Hypercharge;
                                }
                            }

                            // TOOLS!! ChainSaw Drill Air Anchor
                            if (ActionReady(AirAnchor) || (GetCooldownRemainingTime(AirAnchor) < 1.2 && LevelChecked(AirAnchor)))
                            {
                                if (CanWeave(actionID) && !HasEffect(Buffs.Reassembled) &&
                                    GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble) &&
                                    (wildfireCDTime <= 10 || IsOffCooldown(Wildfire)))
                                    return Reassemble;
                                return AirAnchor;
                            }

                            if (ActionReady(Drill) || (GetCooldownRemainingTime(Drill) < 1.2 && LevelChecked(Drill)))
                            {
                                if (CanWeave(actionID) && !HasEffect(Buffs.Reassembled) &&
                                    HasCharges(Reassemble) && (wildfireCDTime <= 8 || IsOffCooldown(Wildfire)))
                                    return Reassemble;
                                return Drill;
                            }

                            if (ActionReady(ChainSaw) || (GetCooldownRemainingTime(ChainSaw) < 1 && LevelChecked(ChainSaw)))
                            {
                                if (CanWeave(actionID) && !HasEffect(Buffs.Reassembled) &&
                                    HasCharges(Reassemble) && wildfireCDTime < 65 && wildfireCDTime > 55)
                                    return Reassemble;
                                return ChainSaw;
                            }

                            if (CanWeave(actionID))
                            {
                                if (HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                    return GaussRound;

                                else if (ActionReady(Ricochet))
                                    return Ricochet;
                            }
                        }

                        //Early Tools Rotation
                        if (rotationSelection is 2 && level >= 90)
                        {
                            //Barrel Stabilizer
                            if (CanWeave(actionID) && gauge.Heat <= 55 && ActionReady(BarrelStabilizer))
                                return BarrelStabilizer;


                            //Wildfire stuff
                            //these try to ensure the correct loop, HC > CS > WF
                            if (ActionReady(Wildfire))
                            {
                                if (CanDelayedWeave(actionID, 0.8) &&
                                (WasLastWeaponskill(HeatedSplitShot) || WasLastWeaponskill(HeatedSlugshot) || WasLastWeaponskill(HeatedCleanShot)))
                                    return Wildfire;

                                else if (CanWeave(actionID) && gauge.IsOverheated)
                                    return Wildfire;
                            }

                            //Queen aka Robot
                            if (CanWeave(actionID) && !gauge.IsRobotActive && !WasLastAbility(Wildfire))
                            {
                                // First condition
                                if (gauge.Battery == 70 && CombatEngageDuration().TotalSeconds > 61 && CombatEngageDuration().TotalSeconds < 68)
                                    return AutomatonQueen;

                                // Second condition
                                if (!WasLastAction(OriginalHook(CleanShot)))
                                {
                                    if (gauge.Battery >= 90 && gauge.LastSummonBatteryPower == 70)
                                        return AutomatonQueen;
                                }

                                // Third condition
                                if (gauge.LastSummonBatteryPower >= 90 && gauge.Battery >= 90)
                                    return AutomatonQueen;

                                // Fourth condition
                                while (gauge.LastSummonBatteryPower != 50 && gauge.Battery == 100)
                                    return AutomatonQueen;

                                // Fifth condition
                                while (gauge.LastSummonBatteryPower == 100 && gauge.Battery >= 90) //was previously 80 with 30 overcap for 10mins
                                    return AutomatonQueen;
                            }

                            //Overheated Reassemble & Heatblast & GaussRico featuring a small ChainSaw addendum
                            if (gauge.IsOverheated && LevelChecked(HeatBlast))
                            {
                                if (CanWeave(actionID, 0.6) /*&& wildfireCDTime > 2*/) //is this why its been breaking?
                                {
                                    if (HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                        return GaussRound;

                                    else if (ActionReady(Ricochet))
                                        return Ricochet;
                                }
                                return HeatBlast;
                            }

                            //HYPERCHARGE!!
                            if (gauge.Heat >= 50 && ActionReady(Hypercharge) && !gauge.IsOverheated)
                            {
                                //Protection & ensures Hyper charged is double weaved with WF during reopener
                                //if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                                if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                                {
                                    if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                    {
                                        if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                        {
                                            if (CanWeave(actionID) && UseHyperchargeEarlyRotation(gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                        else if (!LevelChecked(ChainSaw))
                                        {
                                            if (CanWeave(actionID) && UseHyperchargeEarlyRotation(gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                    }
                                    else if (!LevelChecked(AirAnchor))
                                    {
                                        if (CanWeave(actionID) && UseHyperchargeEarlyRotation(gauge, wildfireCDTime))
                                            return Hypercharge;
                                    }
                                }
                                else if (!LevelChecked(Drill))
                                {
                                    if (CanWeave(actionID) && UseHyperchargeEarlyRotation(gauge, wildfireCDTime))
                                        return Hypercharge;
                                }
                            }

                            // TOOLS!! ChainSaw Drill Air Anchor
                            if (ActionReady(AirAnchor) || (GetCooldownRemainingTime(AirAnchor) < 1 && LevelChecked(AirAnchor)))
                                return AirAnchor;


                            if (ActionReady(Drill) || (GetCooldownRemainingTime(Drill) < 1.2 && LevelChecked(Drill)))
                            {
                                if (CanWeave(actionID) && !HasEffect(Buffs.Reassembled) &&
                                    GetRemainingCharges(Reassemble) <= 2 &&
                                    (wildfireCDTime <= 14 || ActionReady(Wildfire)))
                                    return Reassemble;
                                return Drill;
                            }

                            if (ActionReady(ChainSaw) || (GetCooldownRemainingTime(ChainSaw) < 1.2 && LevelChecked(ChainSaw)))
                            {
                                if (CanWeave(actionID) && !HasEffect(Buffs.Reassembled) &&
                                    GetRemainingCharges(Reassemble) <= 2 &&
                                    (wildfireCDTime <= 12 || IsOffCooldown(Wildfire)))
                                    return Reassemble;
                                return ChainSaw;
                            }

                            if (CanWeave(actionID))
                            {
                                if (HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                    return GaussRound;

                                else if (ActionReady(Ricochet))
                                    return Ricochet;
                            }
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_SecondWind) &&
                        CanWeave(actionID, 0.6) && PlayerHealthPercentageHp() <= Config.MCH_ST_SecondWindThreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassembled) &&
                                !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                ? Reassemble
                                : OriginalHook(CleanShot);
                    }

                    return OriginalHook(SplitShot);
                }

                return actionID;
            }

            private bool UseHyperchargeStandard(MCHGauge gauge, float wildfireCDTime)
            {
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 50 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                    return true;

                if (CombatEngageDuration().Minutes > 0)
                {
                    if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                        return true;

                    if (CombatEngageDuration().Minutes % 2 == 0)
                        return true;
                }

                return false;
            }

            private bool UseHypercharge123Tools(MCHGauge gauge, float wildfireCDTime)
            {
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat >= 60 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                    return true;

                if (CombatEngageDuration().Minutes > 0)
                {
                    if (gauge.Heat >= 50 && GetCooldownRemainingTime(ChainSaw) <= 1 && (wildfireCDTime <= 4 || IsOffCooldown(Wildfire)))
                        return true;

                    if (gauge.Heat >= 50 && wildfireCDTime <= 38 && wildfireCDTime >= 4)
                        return false;

                    if (gauge.Heat >= 55)
                        return true;

                    if (gauge.Heat >= 50 && wildfireCDTime >= 99)
                        return true;
                }

                return false;
            }

            private bool UseHyperchargeEarlyRotation(MCHGauge gauge, float wildfireCDTime)
            {
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat >= 50 || CombatEngageDuration().Seconds <= 30) && WasLastWeaponskill(HeatedSplitShot))
                    return true;

                if (CombatEngageDuration().Minutes > 0)
                {
                    if (gauge.Heat >= 50 && wildfireCDTime <= 36 && wildfireCDTime >= 1)
                        return false;

                    if (gauge.Heat >= 60)
                        return true;

                    if (gauge.Heat >= 50 && wildfireCDTime >= 99)
                        return true;
                }

                return false;
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

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Queen) && !gauge.IsOverheated)
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
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                        gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                        gauge.IsOverheated && LevelChecked(AutoCrossbow))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && WasLastAction(AutoCrossbow) && CanWeave(actionID))
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

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= Config.MCH_AoE_SecondWindThreshold && ActionReady(All.SecondWind))
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