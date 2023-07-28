using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class MCH
    {
        public const byte JobID = 31;

        public const uint
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

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946,
                Overheated = 2688;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }
        public static class Config
        {
            public static UserInt
                MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold"),
                MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold"),
                MCH_VariantCure = new("MCH_VariantCure");
        }

        public static class Levels
        {
            public const byte
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
                ChainSaw = 90,
                Dismantle = 62;
        }

        internal class MCH_ST_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_BasicCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeatedSplitShot)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(SlugShot))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(CleanShot))
                            return OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);
                }
                return actionID;
            }
        }

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is SplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_ST_BarrelStabilizer_DriftProtection) &&
                        ActionReady(BarrelStabilizer) && gauge.Heat < 20 && CanWeave(actionID))
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_HeatBlast) &&
                        gauge.IsOverheated)
                    {
                        if (CanWeave(actionID, 0.6))
                        {
                            if (!LevelChecked(Ricochet) && HasCharges(GaussRound))
                                return GaussRound;

                            if (HasCharges(GaussRound) && GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
                                return GaussRound;

                            else if (LevelChecked(Ricochet) && HasCharges(Ricochet))
                                return Ricochet;
                        }

                        if (LevelChecked(HeatBlast)) // prioritize heatblast
                            return HeatBlast;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_Cooldowns))
                    {
                        if (HasEffect(Buffs.Reassembled) && ActionReady(AirAnchor))
                            return AirAnchor;

                        if (IsOnCooldown(AirAnchor) && ActionReady(Drill))
                            return Drill;

                        if (HasEffect(Buffs.Reassembled) && ActionReady(ChainSaw))
                            return ChainSaw;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGaussCharges) &&
                        CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (LevelChecked(Ricochet) && HasCharges(Ricochet))
                            return Ricochet;

                        if (LevelChecked(GaussRound) && HasCharges(GaussRound))
                            return GaussRound;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGauss) &&
                        CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (LevelChecked(Ricochet) && HasCharges(Ricochet))
                            return Ricochet;

                        if (LevelChecked(GaussRound) && HasCharges(GaussRound))
                            return GaussRound;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainComboAlternate))
                    {
                        if (ActionReady(AirAnchor) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return AirAnchor;

                        if (ActionReady(ChainSaw) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return ChainSaw;

                        if (ActionReady(Drill) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return Drill;

                        if (!LevelChecked(AirAnchor) && ActionReady(HotShot) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return HotShot;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_OverCharge) &&
                    CanWeave(actionID) && gauge.Battery is 100 &&
                    LevelChecked(OriginalHook(RookAutoturret)))
                        return OriginalHook(RookAutoturret);

                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(SlugShot))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(CleanShot))
                            return OriginalHook(CleanShot);
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
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                float wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                int gaussCharges = GetRemainingCharges(GaussRound);
                int gaussMaxCharges = GetMaxCharges(GaussRound);
                short overheatTime = gauge.OverheatTimeRemaining;
                int reasmCharges = GetRemainingCharges(Reassemble);
                int ST_secondWindTreshold = Config.MCH_ST_SecondWindThreshold;
                bool openerReady = ActionReady(ChainSaw) && ActionReady(Wildfire) && ActionReady(BarrelStabilizer) && GetRemainingCharges(Ricochet) == 3 && GetRemainingCharges(GaussRound) == 3;

                if (actionID is SplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener) && level >= 90)
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
                        if ((step == 0 && gauge.Heat > 5) ||
                            (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                            inOpener = false;

                        if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && WasLastAction(HeatedSplitShot) &&
                            IsEnabled(CustomComboPreset.MCH_123Tools_Opener) && level >= 90 && openerReady)
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
                        if (CanWeave(actionID))
                        {
                            //Barrel Stabilizer
                            if (!IsEnabled(CustomComboPreset.MCH_123Tools_Opener) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer)
                            && gauge.Heat <= 55 && inOpener == false && ActionReady(BarrelStabilizer) & !WasLastWeaponskill(ChainSaw) && ((wildfireCDTime <= 9 &&
                            IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only)) || (wildfireCDTime >= 110 &&
                            !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only) && gauge.IsOverheated)))
                                return BarrelStabilizer;

                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                                gauge.Heat <= 55 && ActionReady(BarrelStabilizer))
                                return BarrelStabilizer;

                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) &&
                                CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                                return All.HeadGraze;
                        }

                        //Wildfire stuff (separated because idk how else, drunk af rn)
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && !IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                            (gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && LevelChecked(Wildfire) &&
                            (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast)))) //these try to ensure the correct loops
                        {
                            if (CanDelayedWeave(actionID))
                            {
                                if (!gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                                    return Wildfire;

                                else if (!gauge.IsOverheated && !WasLastWeaponskill(ChainSaw)) //maybe change this later, need WF then ChainSaw   
                                    return Wildfire;

                                else if (gauge.IsOverheated)
                                    return Wildfire;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                            (wildfireCDTime < 1 || (IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                            ActionReady(Wildfire) && WasLastWeaponskill(ChainSaw))) && CanDelayedWeave(actionID, 0.8) && gauge.IsOverheated)
                            //these try to ensure the correct loop, HC > CS > WF
                            return Wildfire;

                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) &&
                            CanWeave(actionID) && !gauge.IsRobotActive && (!WasLastAbility(Wildfire) || !LevelChecked(Wildfire)))
                        {
                            //steps to control robot timings
                            if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener) && level >= 90)
                            {
                                // First condition
                                if (gauge.Battery is 50 && CombatEngageDuration().TotalSeconds > 61 && CombatEngageDuration().TotalSeconds < 68)
                                    return OriginalHook(RookAutoturret);

                                // Second condition
                                if (!WasLastAction(OriginalHook(CleanShot)))
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

                            //if (LevelChecked(RookOverdrive && gauge.Battery == 100 && CombatEngageDuration().Seconds < 55 && !IsEnabled(CustomComboPreset.MCH_123Tools_Opener))
                            //{
                            //    return OriginalHook(RookAutoturret);
                            //} fix this later?
                            else if (!IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                                LevelChecked(RookOverdrive) && gauge.Battery >= 50)
                                return OriginalHook(RookAutoturret);

                            //else if (gauge.Battery >= 50 && LevelChecked(RookOverdrive && (CombatEngageDuration().Seconds >= 58 || CombatEngageDuration().Seconds <= 05))
                            //{
                            //    return OriginalHook(RookAutoturret);
                            //}

                        }

                        if (gauge.IsOverheated && LevelChecked(HeatBlast))
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID, 0.6) && (wildfireCDTime > 2 || !LevelChecked(Wildfire))) //gauss and ricochet weave
                            {
                                //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) &&
                                    overheatTime < 1.7 && !HasEffect(Buffs.Reassembled) && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                    ((IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2) ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2) ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)))
                                    return Reassemble;

                                else if ((!IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) &&
                                    HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))) ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && gaussCharges >= gaussMaxCharges - 1))
                                    return GaussRound;

                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) &&
                                    ActionReady(Ricochet))
                                    return Ricochet;
                            }

                            if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener) && (GetCooldownRemainingTime(ChainSaw) <= 1 || IsOffCooldown(ChainSaw)) &&
                               (wildfireCDTime < 3 || IsOffCooldown(Wildfire)))
                                return ChainSaw;

                            return HeatBlast;
                        }

                        //HYPERCHARGE!!
                        if (!IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                            CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                                gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                            {
                                //Protection & ensures Hyper charged is double weaved with WF during reopener
                                if (HasEffect(Buffs.Wildfire) || !LevelChecked(Wildfire))
                                    return Hypercharge;

                                if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                                {
                                    if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                    {
                                        if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                            if (UseHypercharge(gauge, wildfireCDTime))
                                                return Hypercharge;

                                            else if (!LevelChecked(ChainSaw))
                                                if (UseHypercharge(gauge, wildfireCDTime))
                                                    return Hypercharge;
                                    }

                                    else if (!LevelChecked(AirAnchor))
                                        if (UseHypercharge(gauge, wildfireCDTime))
                                            return Hypercharge;
                                }

                                else if (!LevelChecked(Drill))
                                    if (UseHypercharge(gauge, wildfireCDTime))
                                        return Hypercharge;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener) &&
                            IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                            gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                        {
                            if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                            {
                                if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                {
                                    if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) <= 2 && (wildfireCDTime <= 4 || IsOffCooldown(Wildfire)))
                                        if (CanDelayedWeave(actionID) && UseHypercharge(gauge, wildfireCDTime))
                                            return Hypercharge;

                                        else if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                            if (CanWeave(actionID) && UseHypercharge(gauge, wildfireCDTime))
                                                return Hypercharge;

                                            else if (!LevelChecked(ChainSaw))
                                                if (CanWeave(actionID) && UseHypercharge(gauge, wildfireCDTime))
                                                    return Hypercharge;
                                }

                                else if (!LevelChecked(AirAnchor))
                                    if (CanWeave(actionID) && UseHypercharge(gauge, wildfireCDTime))
                                        return Hypercharge;
                            }

                            else if (!LevelChecked(Drill))
                                if (CanWeave(actionID) && UseHypercharge(gauge, wildfireCDTime))
                                    return Hypercharge;
                        }

                        // TOOLS!! ChainSaw Drill Air Anchor
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling))
                        {
                            if (ActionReady(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                                    !HasEffect(Buffs.Reassembled) &&
                                    HasCharges(Reassemble))
                                {
                                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) &&
                                        GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        return Reassemble;

                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges))
                                        return Reassemble;
                                }

                                return AirAnchor;
                            }

                            else if ((ActionReady(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && !LevelChecked(AirAnchor))
                                return HotShot;

                            if (ActionReady(Drill) || GetCooldownRemainingTime(Drill) < 1)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) &&
                                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                {
                                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges) &&
                                        GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        return Reassemble;

                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges))
                                        return Reassemble;
                                }

                                return Drill;
                            }

                            if (ActionReady(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) &&
                                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                {
                                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges) &&
                                        GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        return Reassemble;

                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges))
                                        return Reassemble;
                                }

                                return ChainSaw;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) &&
                            CanWeave(actionID))
                        {
                            if (HasCharges(GaussRound) && (!LevelChecked(Ricochet) || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                return GaussRound;

                            else if (ActionReady(Ricochet))
                                return Ricochet;
                        }
                    }

                    // healing - please move if not appropriate priority
                    if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) &&
                        CanWeave(actionID, 0.6) &&
                        PlayerHealthPercentageHp() <= ST_secondWindTreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(SlugShot))
                            return OriginalHook(SlugShot);

                        if (lastComboMove is SlugShot && LevelChecked(CleanShot))
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) &&
                                !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                return Reassemble;

                            return OriginalHook(CleanShot);
                        }
                    }

                    return OriginalHook(SplitShot);

                }
                return actionID;
            }

            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                uint wfTimer = 6; //default timer
                if (!LevelChecked(BarrelStabilizer)) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol

                if (!IsEnabled(CustomComboPreset.MCH_123Tools_Opener))
                {
                    if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 ||
                        CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                        return true;

                    if (CombatEngageDuration().Minutes > 0 && (wildfireCDTime >= wfTimer ||
                        WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && (IsOffCooldown(Wildfire) || wildfireCDTime < 1))))
                    {
                        if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                            return true;

                        if (CombatEngageDuration().Minutes % 2 == 0)
                            return true;
                    }
                }

                if (IsEnabled(CustomComboPreset.MCH_123Tools_Opener))
                {
                    if (CombatEngageDuration().Minutes == 0 && (gauge.Heat >= 60 ||
                        CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                        return true;

                    if (CombatEngageDuration().Minutes > 0)
                    {
                        if (gauge.Heat >= 50 && GetCooldownRemainingTime(ChainSaw) <= 1 &&
                            (wildfireCDTime <= 4 || ActionReady(Wildfire)))
                            return true;


                        if (gauge.Heat < 50 && GetCooldownRemainingTime(AirAnchor) < 25 &&
                            GetCooldownRemainingTime(ChainSaw) < 30)
                            return false;


                        if (gauge.Heat >= 50 && wildfireCDTime <= 26)
                            return true;


                        if (gauge.Heat >= 50 && wildfireCDTime >= 60)
                            return true;
                    }
                }

                return false;
            }
        }

        internal class MCH_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                int gaussCharges = GetRemainingCharges(GaussRound);
                int ricochetCharges = GetRemainingCharges(Ricochet);
                int AoE_secondWindTreshold = Config.MCH_AoE_SecondWindThreshold;

                if (actionID is SpreadShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_OverCharge) && CanWeave(actionID) &&
                        gauge.Battery is 100 && LevelChecked(OriginalHook(RookOverdrive)))
                        return OriginalHook(RookAutoturret);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && CanWeave(actionID) &&
                        (IsEnabled(CustomComboPreset.MCH_AoE_Gauss) || gauge.IsOverheated) &&
                        (HasCharges(Ricochet) || HasCharges(GaussRound)))
                    {
                        if ((gaussCharges >= ricochetCharges || !LevelChecked(Ricochet)) &&
                            LevelChecked(GaussRound))
                            return GaussRound;

                        else if (ActionReady(Ricochet))
                            return Ricochet;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster) &&
                        ActionReady(BioBlaster) && !gauge.IsOverheated)
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Hypercharge) &&
                        CanWeave(actionID) && gauge.Heat >= 50 && LevelChecked(AutoCrossbow) && !gauge.IsOverheated)
                        return AutoCrossbow;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_SecondWind) &&
                        CanWeave(actionID, 0.6) && PlayerHealthPercentageHp() <= AoE_secondWindTreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;
                }

                return actionID;
            }
        }

        internal class MCH_HeatblastGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HeatblastGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is HeatBlast)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) && gauge.Heat < 50 && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Wildfire) &&
                        LevelChecked(Hypercharge) && ActionReady(Wildfire) && gauge.Heat >= 50)
                        return Wildfire;

                    if (!gauge.IsOverheated && LevelChecked(Hypercharge))
                        return Hypercharge;

                    if (GetCooldownRemainingTime(HeatBlast) < 0.7 && LevelChecked(HeatBlast)) // Prioritize Heat Blast
                        return HeatBlast;

                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
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
                int gaussCharges = GetRemainingCharges(GaussRound);
                int ricochetCharges = GetRemainingCharges(Ricochet);

                if ((actionID is GaussRound) && level >= 50)
                {

                    // Prioritize the original if both are off cooldown

                    if (ActionReady(GaussRound) && ActionReady(Ricochet))
                        return actionID;

                    if ((gaussCharges >= ricochetCharges || !LevelChecked(Ricochet)) &&
                        LevelChecked(GaussRound))
                        return GaussRound;

                    else if (ricochetCharges > 0 && LevelChecked(Ricochet))
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
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is RookAutoturret && gauge.IsRobotActive)
                    return OriginalHook(QueenOverdrive);

                return actionID;
            }
        }

        internal class MCH_HotShotDrillChainSaw : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainSaw;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HotShot)
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

        internal class MCH_DismantleTactician : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_DismantleTactician;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Dismantle
                    && (IsOnCooldown(Dismantle) || !LevelChecked(Dismantle))
                    && ActionReady(Tactician)
                    && !HasEffect(Buffs.Tactician))
                    return Tactician;

                return actionID;
            }
        }

        internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbowGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is AutoCrossbow)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) && gauge.Heat < 50 && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (!gauge.IsOverheated && LevelChecked(Hypercharge))
                        return Hypercharge;

                    if (GetCooldownRemainingTime(HeatBlast) < 0.7 && LevelChecked(AutoCrossbow)) // prioritize autocrossbow
                        return AutoCrossbow;

                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }
    }
}