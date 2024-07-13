using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class MCH
    {
        public const byte JobID = 31;

        public const uint
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            HeatedSlugShot = 7412,
            GaussRound = 2874,
            Ricochet = 2890,
            Reassemble = 2876,
            Drill = 16498,
            HotShot = 2872,
            AirAnchor = 16500,
            Hypercharge = 17209,
            Heatblast = 7410,
            SpreadShot = 2870,
            Scattergun = 25786,
            AutoCrossbow = 16497,
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
            Tactician = 16889,
            Chainsaw = 25788,
            BioBlaster = 16499,
            BarrelStabilizer = 7414,
            Wildfire = 2878,
            Dismantle = 2887,
            Flamethrower = 7418,
            BlazingShot = 36978,
            DoubleCheck = 36979,
            CheckMate = 36980,
            Excavator = 36981,
            FullMetalField = 36982;

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946,
                Overheated = 2688,
                Flamethrower = 1205,
                Hypercharged = 3864,
                ExcavatorReady = 3865,
                FullMetalMachinist = 3866;
        }

        public static class Debuffs
        {
            public const ushort
                Dismantled = 2887,
                Bioblaster = 1866;
        }

        public static class Traits
        {
            public const ushort
                EnhancedDrill = 605;
        }

        public static class Config
        {
            public static UserInt
                MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold", 25),
                MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold", 25),
                MCH_VariantCure = new("MCH_VariantCure"),
                MCH_ST_TurretUsage = new("MCH_ST_Adv_TurretGauge"),
                MCH_AoE_TurretUsage = new("MCH_AoE_TurretUsage"),
                MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool", 0),
                MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool", 0),
                MCH_ST_WildfireHP = new("MCH_ST_WildfireHP", 1),
                MCH_ST_HyperchargeHP = new("MCH_ST_HyperchargeHP", 1),
                MCH_ST_QueenOverDrive = new("MCH_ST_QueenOverDrive");
            public static UserBoolArray
                MCH_ST_Reassembled = new("MCH_ST_Reassembled"),
                MCH_AoE_Reassembled = new("MCH_AoE_Reassembled");
            public static UserBool
                MCH_AoE_Hypercharge = new("MCH_AoE_Hypercharge");
        }

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static MCHOpenerLogic MCHOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy() && CanDelayedWeave(actionID);
                float heatblastRC = GetCooldown(Heatblast).CooldownTotal;
                bool drillCD = !LevelChecked(Drill) || (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) > heatblastRC * 6);
                bool anchorCD = !LevelChecked(AirAnchor) || (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) > heatblastRC * 6);
                bool sawCD = !LevelChecked(Chainsaw) || (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);
                float GCD = GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

                if (actionID is SplitShot or HeatedSplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // Opener for MCH
                    if (MCHOpener.DoFullOpener(ref actionID))
                        return actionID;

                    // Interrupt
                    if (interruptReady)
                        return All.HeadGraze;

                    // Wildfire
                    if (ActionReady(Wildfire) && WasLastAction(Hypercharge))
                        return Wildfire;

                    //Queen
                    if (UseQueen(gauge))
                        return OriginalHook(RookAutoturret);

                    // Barrel Stabilizer
                    if (!gauge.IsOverheated && CanWeave(actionID) && ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    //gauss and ricochet outside HC
                    if (CanWeave(actionID) && !gauge.IsOverheated &&
                        (WasLastWeaponskill(OriginalHook(AirAnchor)) || WasLastWeaponskill(OriginalHook(Chainsaw)) || WasLastWeaponskill(Drill)))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && !WasLastAction(OriginalHook(GaussRound)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && !WasLastAction(OriginalHook(Ricochet)))
                            return OriginalHook(Ricochet);
                    }


                    if (ReassembledTools(ref actionID, gauge) && !gauge.IsOverheated)
                        return actionID;

                    //Full metal field
                    if (HasEffect(Buffs.FullMetalMachinist) && WasLastWeaponskill(Excavator) &&
                        IsOnCooldown(Chainsaw) && IsOnCooldown(AirAnchor))
                        return OriginalHook(BarrelStabilizer);

                    if (CanWeave(actionID) && (gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) &&
                        LevelChecked(Hypercharge) && !gauge.IsOverheated && !WasLastAction(BarrelStabilizer))
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if ((LevelChecked(FullMetalField) && WasLastWeaponskill(FullMetalField) && ActionReady(Wildfire)) ||
                            (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        if (drillCD && anchorCD && sawCD &&
                            ((GetCooldownRemainingTime(Wildfire) > GCD * 8 && LevelChecked(Wildfire)) || !LevelChecked(Wildfire)))
                            return Hypercharge;
                    }

                    //Heatblast, Gauss, Rico
                    if (CanWeave(actionID) && WasLastAction(OriginalHook(Heatblast)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                        return OriginalHook(Heatblast);

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (lastComboMove == OriginalHook(SlugShot) &&
                            !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                            return Reassemble;

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);
                }
                return actionID;
            }
            // TOOLS!! Chainsaw Drill Air Anchor Excavator
            private static bool ReassembledTools(ref uint actionID, MCHGauge gauge)
            {
                bool battery = Svc.Gauges.Get<MCHGauge>().Battery >= 90;

                if ((!gauge.IsOverheated && !WasLastWeaponskill(OriginalHook(Heatblast)) &&
                    !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) && (CanWeave(actionID) || !InCombat()) &&
                    GetRemainingCharges(Reassemble) > Config.MCH_ST_ReassemblePool &&
                    LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady) && !battery) ||
                    (LevelChecked(Chainsaw) && ((GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw)) && !battery) ||
                    (LevelChecked(AirAnchor) && ((GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(AirAnchor)) && !battery) ||
                    (LevelChecked(Drill) && ((GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill))))
                {
                    actionID = Reassemble;
                    return true;
                }

                if (LevelChecked(OriginalHook(Chainsaw)) &&
                   !battery &&
                   HasEffect(Buffs.ExcavatorReady) && IsOnCooldown(AirAnchor))
                {
                    actionID = OriginalHook(Chainsaw);
                    return true;
                }

                if (LevelChecked(Chainsaw) &&
                    !battery &&
                    ((GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw)))
                {
                    actionID = Chainsaw;
                    return true;
                }

                if (LevelChecked(OriginalHook(AirAnchor)) &&
                     !battery &&
                     ((GetCooldownRemainingTime(OriginalHook(AirAnchor)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(OriginalHook(AirAnchor))))
                {
                    actionID = OriginalHook(AirAnchor);
                    return true;
                }

                if (LevelChecked(Drill) &&
                    ((!TraitLevelChecked(Traits.EnhancedDrill) && ((GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill))) ||
                    (TraitLevelChecked(Traits.EnhancedDrill) && !WasLastWeaponskill(Drill) && ((GetCooldownChargeRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill)))) &&
                    ((LevelChecked(FullMetalField) && !WasLastWeaponskill(FullMetalField)) || !LevelChecked(FullMetalField)))
                {
                    actionID = Drill;
                    return true;
                }
                return false;
            }

            private static bool UseQueen(MCHGauge gauge)
            {
                if (CanWeave(OriginalHook(SplitShot)) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                    LevelChecked(OriginalHook(RookAutoturret)) && !gauge.IsRobotActive && gauge.Battery >= 50 &&
                    ((LevelChecked(FullMetalField) && !WasLastWeaponskill(FullMetalField)) || !LevelChecked(FullMetalField)))
                {
                    int queensUsed = ActionWatching.CombatActions.Count(x => x == OriginalHook(RookAutoturret));

                    if (queensUsed < 2)
                        return true;

                    if (queensUsed >= 2 && gauge.Battery >= 90)
                        return true;
                }

                return false;
            }
        }

        internal class MCH_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_AdvancedMode;
            internal static MCHOpenerLogic MCHOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy() && CanDelayedWeave(actionID);
                float heatblastRC = GetCooldown(Heatblast).CooldownTotal;
                bool drillCD = !ActionReady(Drill) || (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) > heatblastRC * 6);
                bool anchorCD = !ActionReady(AirAnchor) || (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) > heatblastRC * 6);
                bool sawCD = !ActionReady(Chainsaw) || (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);
                float GCD = GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

                if (actionID is SplitShot or HeatedSplitShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // Opener for MCH
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener))
                    {
                        if (MCHOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    // Interrupt
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Interrupt) && interruptReady)
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_QueenOverdrive) && gauge.IsRobotActive && GetTargetHPPercent() <= Config.MCH_ST_QueenOverDrive &&
                        CanWeave(actionID) && ActionReady(OriginalHook(RookOverdrive)))
                        return OriginalHook(RookOverdrive);

                    // Wildfire
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_WildFire) &&
                        ActionReady(Wildfire) && WasLastAction(Hypercharge) &&
                        GetTargetHPPercent() >= Config.MCH_ST_WildfireHP)
                        return Wildfire;

                    //Queen
                    if (UseQueen(gauge))
                        return OriginalHook(RookAutoturret);

                    // BarrelStabilizer use and Full metal field
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer) &&
                        !gauge.IsOverheated && CanWeave(actionID) && ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    //gauss and ricochet outside HC
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                        CanWeave(actionID) && !gauge.IsOverheated &&
                        (WasLastWeaponskill(OriginalHook(AirAnchor)) || WasLastWeaponskill(OriginalHook(Chainsaw)) || WasLastWeaponskill(Drill)))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && !WasLastAction(OriginalHook(GaussRound)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && !WasLastAction(OriginalHook(Ricochet)))
                            return OriginalHook(Ricochet);
                    }

                    if (ReassembledTools(ref actionID, gauge) && !gauge.IsOverheated)
                        return actionID;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                        HasEffect(Buffs.FullMetalMachinist) && WasLastWeaponskill(Excavator) &&
                        IsOnCooldown(Chainsaw) && IsOnCooldown(AirAnchor))
                        return OriginalHook(BarrelStabilizer);

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Hypercharge) &&
                        CanWeave(actionID) && (gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) &&
                        LevelChecked(Hypercharge) && !gauge.IsOverheated && !WasLastAction(BarrelStabilizer) &&
                        GetTargetHPPercent() >= Config.MCH_ST_HyperchargeHP)
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if ((LevelChecked(FullMetalField) && WasLastWeaponskill(FullMetalField) && ActionReady(Wildfire)) ||
                            (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        if (drillCD && anchorCD && sawCD &&
                            ((GetCooldownRemainingTime(Wildfire) > GCD * 8 && LevelChecked(Wildfire)) || !LevelChecked(Wildfire)))
                            return Hypercharge;
                    }

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                        CanWeave(actionID) && WasLastAction(OriginalHook(Heatblast)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Heatblast) &&
                        gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                        return OriginalHook(Heatblast);

                    // healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_SecondWind) &&
                        CanWeave(actionID) && PlayerHealthPercentageHp() <= Config.MCH_ST_SecondWindThreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[4] && lastComboMove == OriginalHook(SlugShot) &&
                            !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                            return Reassemble;

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);
                }
                return actionID;
            }

            private static bool ReassembledTools(ref uint actionID, MCHGauge gauge)
            {
                bool battery = Svc.Gauges.Get<MCHGauge>().Battery >= 90;
                bool reassembledExcavator = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[0] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[0] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));
                bool reassembledChainsaw = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[1] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[1] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));
                bool reassembledAnchor = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[2] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[2] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));
                bool reassembledDrill = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[3] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[3] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));

                // TOOLS!! Chainsaw Drill Air Anchor Excavator
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) &&
                    !gauge.IsOverheated && !WasLastWeaponskill(OriginalHook(Heatblast)) &&
                    !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) && (CanWeave(actionID) || !InCombat()) &&
                    GetRemainingCharges(Reassemble) > Config.MCH_ST_ReassemblePool &&
                    ((Config.MCH_ST_Reassembled[0] && LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady) && !battery) ||
                    (Config.MCH_ST_Reassembled[1] && LevelChecked(Chainsaw) && ((GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw)) && !battery) ||
                    (Config.MCH_ST_Reassembled[2] && LevelChecked(AirAnchor) && ((GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(AirAnchor)) && !battery) ||
                    (Config.MCH_ST_Reassembled[3] && LevelChecked(Drill) && ((GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill)))))
                {
                    actionID = Reassemble;
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Excavator) &&
                    reassembledExcavator &&
                    LevelChecked(OriginalHook(Chainsaw)) &&
                    !battery &&
                    HasEffect(Buffs.ExcavatorReady) && IsOnCooldown(AirAnchor))
                {
                    actionID = OriginalHook(Chainsaw);
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Chainsaw) &&
                    reassembledChainsaw &&
                    LevelChecked(Chainsaw) &&
                    !battery &&
                    ((GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw)))
                {
                    actionID = Chainsaw;
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor) &&
                     reassembledAnchor &&
                     LevelChecked(OriginalHook(AirAnchor)) &&
                     !battery &&
                     ((GetCooldownRemainingTime(OriginalHook(AirAnchor)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(OriginalHook(AirAnchor))))
                {
                    actionID = OriginalHook(AirAnchor);
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Drill) &&
                    reassembledDrill &&
                    LevelChecked(Drill) &&
                    ((!TraitLevelChecked(Traits.EnhancedDrill) && ((GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill))) ||
                    (TraitLevelChecked(Traits.EnhancedDrill) && !WasLastWeaponskill(Drill) && ((GetCooldownChargeRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill)))) &&
                    ((LevelChecked(FullMetalField) && !WasLastWeaponskill(FullMetalField)) || !LevelChecked(FullMetalField)))
                {
                    actionID = Drill;
                    return true;
                }
                return false;
            }

            private static bool UseQueen(MCHGauge gauge)
            {
                if (IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) && Config.MCH_ST_TurretUsage == 1 &&
                    CanWeave(OriginalHook(SplitShot)) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                    LevelChecked(OriginalHook(RookAutoturret)) && !gauge.IsRobotActive && gauge.Battery >= 50 &&
                    ((LevelChecked(FullMetalField) && !WasLastWeaponskill(FullMetalField)) || !LevelChecked(FullMetalField)))
                {
                    int queensUsed = ActionWatching.CombatActions.Count(x => x == OriginalHook(RookAutoturret));

                    if (queensUsed < 2)
                        return true;

                    if (queensUsed >= 2 && gauge.Battery >= 90)
                        return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) && Config.MCH_ST_TurretUsage == 0 &&
                    CanWeave(OriginalHook(SplitShot)) && LevelChecked(OriginalHook(RookAutoturret)) && gauge.Battery >= 50)
                    return true;

                return false;
            }
        }

        internal class MCH_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is SpreadShot)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) &&
                     PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower))
                        return OriginalHook(11);

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // BarrelStabilizer use and Full metal field
                    if (!gauge.IsOverheated && ((CanWeave(actionID) && ActionReady(BarrelStabilizer)) ||
                        HasEffect(Buffs.FullMetalMachinist)))
                        return OriginalHook(BarrelStabilizer);

                    if (ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster))
                        return OriginalHook(BioBlaster);

                    if (ActionReady(Flamethrower) && !IsMoving)
                        return OriginalHook(Flamethrower);

                    if (!gauge.IsOverheated && gauge.Battery == 100)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge        
                    if ((gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) && LevelChecked(AutoCrossbow) && !gauge.IsOverheated &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) || !BioBlaster.LevelChecked()) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) || !Flamethrower.LevelChecked()))
                        return Hypercharge;

                    //AutoCrossbow, Gauss, Rico
                    if (CanWeave(actionID) && WasLastAction(OriginalHook(AutoCrossbow)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (gauge.IsOverheated && AutoCrossbow.LevelChecked())
                        return OriginalHook(AutoCrossbow);

                    if (!HasEffect(Buffs.Wildfire) &&
                        !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                        (Scattergun.LevelChecked() ||
                        (gauge.IsOverheated && AutoCrossbow.LevelChecked()) ||
                        (GetCooldownRemainingTime(Chainsaw) < 1 && Chainsaw.LevelChecked()) ||
                        (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Excavator.LevelChecked())))
                        return Reassemble;

                    if (LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                        return OriginalHook(Chainsaw);

                    if ((LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) < 1) ||
                        ActionReady(Chainsaw))
                        return Chainsaw;

                    if (LevelChecked(AutoCrossbow) && gauge.IsOverheated)
                        return AutoCrossbow;
                }
                return actionID;
            }
        }

        internal class MCH_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpreadShot or Scattergun)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();
                    bool reassembledScattergun = IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[0] && HasEffect(Buffs.Reassembled);
                    bool reassembledCrossbow = (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[1] && HasEffect(Buffs.Reassembled)) || (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[1] && !HasEffect(Buffs.Reassembled)) || (!IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble));
                    bool reassembledChainsaw = (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[2] && HasEffect(Buffs.Reassembled)) || (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[2] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble));
                    bool reassembledExcavator = (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[3] && HasEffect(Buffs.Reassembled)) || (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[3] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble));

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) &&
                     PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower))
                        return OriginalHook(11);

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // BarrelStabilizer use and Full metal field
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer) &&
                        !gauge.IsOverheated && ((CanWeave(actionID) && ActionReady(BarrelStabilizer)) ||
                        (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer_FullMetalField) && HasEffect(Buffs.FullMetalMachinist))))
                        return OriginalHook(BarrelStabilizer);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster) &&
                        ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster))
                        return OriginalHook(BioBlaster);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower) &&
                        ActionReady(Flamethrower) && !IsMoving)
                        return OriginalHook(Flamethrower);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Queen) && !gauge.IsOverheated)
                    {
                        if (gauge.Battery >= Config.MCH_AoE_TurretUsage)
                            return OriginalHook(RookAutoturret);
                    }

                    // Hypercharge        
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                        (gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) && LevelChecked(AutoCrossbow) && !gauge.IsOverheated &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) || !BioBlaster.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster)) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) || !Flamethrower.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower)))
                        return Hypercharge;

                    //AutoCrossbow, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && !Config.MCH_AoE_Hypercharge &&
                        CanWeave(actionID) && WasLastAction(OriginalHook(AutoCrossbow)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (gauge.IsOverheated && AutoCrossbow.LevelChecked())
                        return OriginalHook(AutoCrossbow);

                    //gauss and ricochet outside HC
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && Config.MCH_AoE_Hypercharge &&
                        CanWeave(actionID) && !gauge.IsOverheated)
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && !WasLastAction(OriginalHook(GaussRound)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && !WasLastAction(OriginalHook(Ricochet)))
                            return OriginalHook(Ricochet);
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !HasEffect(Buffs.Wildfire) &&
                        !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                        GetRemainingCharges(Reassemble) > Config.MCH_AoE_ReassemblePool &&
                        ((Config.MCH_AoE_Reassembled[0] && Scattergun.LevelChecked()) ||
                        (gauge.IsOverheated && Config.MCH_AoE_Reassembled[1] && AutoCrossbow.LevelChecked()) ||
                        (GetCooldownRemainingTime(Chainsaw) < 1 && Config.MCH_AoE_Reassembled[2] && Chainsaw.LevelChecked()) ||
                        (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Config.MCH_AoE_Reassembled[3] && Excavator.LevelChecked())))
                        return Reassemble;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Excavator) &&
                        reassembledExcavator &&
                        LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                        return OriginalHook(Chainsaw);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Chainsaw) &&
                        reassembledChainsaw &&
                        ((LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) < 1) ||
                        ActionReady(Chainsaw)))
                        return Chainsaw;

                    if (reassembledScattergun)
                        return OriginalHook(Scattergun);

                    if (reassembledCrossbow &&
                        LevelChecked(AutoCrossbow) && gauge.IsOverheated)
                        return AutoCrossbow;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_SecondWind))
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
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Heatblast;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is Heatblast)
                {
                    if (IsEnabled(CustomComboPreset.MCH_Heatblast_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_Heatblast_Wildfire) &&
                        ActionReady(Hypercharge) &&
                        ActionReady(Wildfire) &&
                        WasLastAction(Hypercharge))
                        return Wildfire;

                    if (!gauge.IsOverheated && LevelChecked(Hypercharge) &&
                        (gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_Heatblast_GaussRound) &&
                        CanWeave(actionID) && WasLastAction(OriginalHook(Heatblast)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                        return OriginalHook(Heatblast);
                }
                return actionID;
            }
        }

        internal class MCH_GaussRoundRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_GaussRoundRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID is GaussRound or Ricochet && CanWeave(actionID))
                {
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }
                }

                return actionID;
            }
        }

        internal class MCH_Overdrive : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;
            MCHGauge? gauge = GetJobGauge<MCHGauge>();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RookAutoturret or AutomatonQueen)
                {
                    if (gauge.IsRobotActive)
                        return OriginalHook(QueenOverdrive);
                }
                return actionID;
            }
        }

        internal class MCH_HotShotDrillChainsawExcavator : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainsawExcavator;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Drill || actionID is HotShot || actionID is AirAnchor || actionID is Chainsaw)
                {
                    if (LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                        return CalcBestAction(actionID, Excavator, Chainsaw, AirAnchor, Drill);

                    if (LevelChecked(Chainsaw))
                        return CalcBestAction(actionID, Chainsaw, AirAnchor, Drill);

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
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbow;


            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MCHGauge? gauge = GetJobGauge<MCHGauge>();

                if (actionID is AutoCrossbow)
                {
                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_AutoBarrel) &&
                            ActionReady(BarrelStabilizer) && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (!gauge.IsOverheated && LevelChecked(Hypercharge) && (gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_GaussRound) && CanWeave(actionID) && WasLastAction(OriginalHook(AutoCrossbow)) &&
                        (ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    if (gauge.IsOverheated && LevelChecked(OriginalHook(AutoCrossbow)))
                        return OriginalHook(AutoCrossbow);
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
                        return OriginalHook(11);

                return actionID;
            }
        }
    }
}
