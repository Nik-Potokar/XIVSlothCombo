using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
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
                Flamethrower = 1205;
        }

        public static class Debuffs
        {
            public const ushort
            Dismantled = 2887;
        }

        internal static class Levels
        {
            internal const byte
                SlugShot = 2,
                Hotshot = 4,
                Reassemble = 10,
                GaussRound = 15,
                SpreadShot = 18,
                CleanShot = 26,
                Hypercharge = 30,
                Heatblast = 35,
                RookOverdrive = 40,
                Wildfire = 45,
                Ricochet = 50,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                Tactician = 56,
                Drill = 58,
                HeatedSlugshot = 60,
                Dismantle = 62,
                HeatedCleanShot = 64,
                BarrelStabilizer = 66,
                BlazingShot = 68,
                FlameThrower = 70,
                BioBlaster = 72,
                AirAnchor = 76,
                ChargedActionMastery = 74,
                QueenOverdrive = 80,
                Scattergun = 82,
                Chainsaw = 90,
                DoubleCheck = 92,
                CheckMate = 92,
                Excavator = 96,
                FullMetalField = 100;  
        }

        public static class Config
        {
            public static UserInt
                MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold"),
                MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold"),
                MCH_VariantCure = new("MCH_VariantCure"),
                MCH_ST_TurretUsage = new("MCH_ST_Adv_TurretGauge"),
                MCH_AoE_TurretUsage = new("MCH_AoE_TurretUsage"),
                MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool"),
                MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool"),
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
                float wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                MCHGauge? gauge = GetJobGauge<MCHGauge>();
                bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy();

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
                    if (MCHOpener.DoFullOpener(ref actionID, true))
                        return actionID;

                    // Interrupt
                    if (interruptReady)
                        return All.HeadGraze;

                    // BarrelStabilizer use
                    if (CanWeave(actionID) && gauge.Heat <= 45 && LevelChecked(BarrelStabilizer) && ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    // Wildfire

                    if (gauge.Heat >= 50 && CanDelayedWeave(actionID) && LevelChecked(Chainsaw) && ActionReady(Wildfire) &&
                        !gauge.IsOverheated && WasLastWeaponskill(AirAnchor)) //these try to ensure the correct loops
                        return Wildfire;

                    else if (gauge.Heat >= 50 && ActionReady(Wildfire))
                        return Wildfire;

                    //queen
                    if (CanWeave(actionID) && !gauge.IsOverheated && LevelChecked(OriginalHook(RookAutoturret)) && gauge.Battery > 0)
                    {
                        if (LevelChecked(Chainsaw) &&
                            ((gauge.Battery is 50 && CombatEngageDuration().TotalSeconds > 59 && CombatEngageDuration().TotalSeconds < 68) || // First Minute Queen 
                            (gauge.Battery is 100 && wildfireCDTime <= 7 && GetCooldownRemainingTime(AirAnchor) <= 3 && CombatEngageDuration().Minutes % 2 == 0) || // Even Minute Queen
                            (gauge.Battery >= 80 && CombatEngageDuration().Minutes % 2 == 1 && wildfireCDTime > 45 && wildfireCDTime < 70))) // Odd minute Queen
                            return OriginalHook(RookAutoturret);

                        else if (gauge.Battery is 100)
                            return OriginalHook(RookAutoturret);
                    }

                    if (CanWeave(actionID) && gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if ((WasLastAction(Chainsaw) && HasEffect(Buffs.Wildfire)) ||
                            (!LevelChecked(Chainsaw) && HasEffect(Buffs.Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        if (LevelChecked(OriginalHook(AirAnchor)) && GetCooldownRemainingTime(OriginalHook(AirAnchor)) >= 8)
                        {
                            if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                            {
                                if (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) >= 8)
                                {
                                    if (UseHyperchargSimpleMode(gauge, wildfireCDTime))
                                        return Hypercharge;
                                }

                                else if (!LevelChecked(Chainsaw))
                                {
                                    if (UseHyperchargSimpleMode(gauge, wildfireCDTime))
                                        return Hypercharge;
                                }
                            }

                            else if (!LevelChecked(Drill))
                            {
                                if (UseHyperchargSimpleMode(gauge, wildfireCDTime))
                                    return Hypercharge;
                            }

                        }
                        else if (!LevelChecked(OriginalHook(AirAnchor)))
                        {
                            if (UseHyperchargSimpleMode(gauge, wildfireCDTime))
                                return Hypercharge;
                        }

                    }

                    //Heatblast, Gauss, Rico
                    if (gauge.IsOverheated)
                    {
                        if (CanWeave(actionID) && WasLastAction(Heatblast) && ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                        {
                            if (LevelChecked(GaussRound) && GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                return GaussRound;

                            if (LevelChecked(Ricochet) && GetRemainingCharges(Ricochet) > GetRemainingCharges(GaussRound))
                                return Ricochet;
                        }

                        if (LevelChecked(Heatblast))
                            return Heatblast;
                    }

                    if (ReassembledTools(ref actionID))
                        return actionID;

                    //gauss and ricochet overcap protection
                    if (CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire))
                    {
                        if (GetRemainingCharges(GaussRound) >= 2 && LevelChecked(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) >= 2 && LevelChecked(Ricochet))
                            return Ricochet;
                    }

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (!LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) && lastComboMove is SlugShot)
                            return Reassemble;

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return OriginalHook(CleanShot);
                    }

                    return OriginalHook(SplitShot);
                }

                return actionID;
            }
            private static bool ReassembledTools(ref uint actionId)
            {
                // TOOLS!! Chainsaw Drill Air Anchor
                if (!HasEffect(Buffs.Wildfire) &&
                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                    ((LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw) ||
                    (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(AirAnchor) ||
                    (!LevelChecked(AirAnchor) && LevelChecked(Drill) && GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill)))
                {
                    actionId = Reassemble;
                    return true;
                }

                if ((LevelChecked(Chainsaw) &&
                    GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw))
                {
                    actionId = Chainsaw;
                    return true;
                }

                if ((LevelChecked(OriginalHook(AirAnchor)) &&
                    GetCooldownRemainingTime(OriginalHook(AirAnchor)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(OriginalHook(AirAnchor)))
                {
                    actionId = OriginalHook(AirAnchor);
                    return true;
                }

                if ((Drill.LevelChecked() &&
                    GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Drill))
                {
                    actionId = Drill;
                    return true;
                }

                return false;
            }

            private bool UseHyperchargSimpleMode(MCHGauge gauge, float wildfireCDTime)
            {
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat == 60 || CombatEngageDuration().Seconds <= 33))
                    return true;

                if (CombatEngageDuration().Minutes > 0)
                {
                    if (gauge.Heat >= 55 && wildfireCDTime > 25)
                        return true;

                    if (gauge.Heat >= 50 && wildfireCDTime <= 25 && wildfireCDTime >= 1)
                        return false;

                    if (gauge.Heat >= 55)
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
                bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy();
                double heatblastRC = 1.5;
                bool drillCD = !Drill.LevelChecked() || (Drill.LevelChecked() && GetCooldownRemainingTime(Drill) > heatblastRC * 6);
                bool anchorCD = !OriginalHook(AirAnchor).LevelChecked() || (OriginalHook(AirAnchor).LevelChecked() && GetCooldownRemainingTime(OriginalHook(AirAnchor)) > heatblastRC * 6);
                bool sawCD = !Chainsaw.LevelChecked() || (Chainsaw.LevelChecked() && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);

                if (actionID is SplitShot)
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
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener) && HasBattleTarget())
                    {
                        if (MCHOpener.DoFullOpener(ref actionID, false))
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
                        gauge.Heat >= 50 && CanWeave(actionID) && !gauge.IsOverheated && ActionReady(Wildfire) &&
                        (GetCooldownRemainingTime(Chainsaw) <= 1 || level < 90))
                        return Wildfire;


                    //Queen
                    if (UseQueen(gauge))
                        return OriginalHook(RookAutoturret);

                    // BarrelStabilizer use
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer) && CanWeave(actionID) &&
                        gauge.Heat <= 50 && ActionReady(BarrelStabilizer) && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Hypercharge) &&
                            CanWeave(actionID) && gauge.Heat >= 50 && LevelChecked(Hypercharge) && !gauge.IsOverheated)
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if ((WasLastAction(Chainsaw) && HasEffect(Buffs.Wildfire)) ||
                            (!LevelChecked(Chainsaw) && HasEffect(Buffs.Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        if (drillCD && anchorCD && sawCD)
                            return Hypercharge;
                    }

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) && CanWeave(actionID) && WasLastAction(Heatblast) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                    {
                        if (ActionReady(GaussRound) && GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                            return GaussRound;

                        if (ActionReady(Ricochet) && GetRemainingCharges(Ricochet) > GetRemainingCharges(GaussRound))
                            return Ricochet;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Heatblast) &&
                        gauge.IsOverheated && LevelChecked(Heatblast))
                        return Heatblast;

                    if (ReassembledTools(ref actionID))
                        return actionID;


                    //gauss and ricochet overcap protection
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                        CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                    {
                        if (GetRemainingCharges(GaussRound) > 1 && LevelChecked(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) > 1 && LevelChecked(Ricochet))
                            return Ricochet;
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_SecondWind) &&
                        CanWeave(actionID) && PlayerHealthPercentageHp() <= Config.MCH_ST_SecondWindThreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                            return OriginalHook(SlugShot);

                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[3] && lastComboMove == OriginalHook(SlugShot) &&
                            !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                            return Reassemble;

                        if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                            return OriginalHook(CleanShot);
                    }

                    return OriginalHook(SplitShot);
                }

                return actionID;
            }

            private static bool ReassembledTools(ref uint actionId)
            {
                bool battery = Svc.Gauges.Get<MCHGauge>().Battery == 100;
                bool reassembledAnchor = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[0] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[0] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));
                bool reassembledDrill = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[1] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[1] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));
                bool reassembledChainsaw = (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[2] && (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) || (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[2] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble));

                // TOOLS!! Chainsaw Drill Air Anchor
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !HasEffect(Buffs.Wildfire) && !ActionWatching.WasLast2ActionsAbilities() &&
                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                    GetRemainingCharges(Reassemble) > Config.MCH_ST_ReassemblePool &&
                    ((GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) && Config.MCH_ST_Reassembled[0] && LevelChecked(AirAnchor) && !battery) ||
                    (GetCooldownRemainingTime(OriginalHook(Drill)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) && Config.MCH_ST_Reassembled[1] && LevelChecked(Drill) && !LevelChecked(AirAnchor)) ||
                    (GetCooldownRemainingTime(OriginalHook(Chainsaw)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) && Config.MCH_ST_Reassembled[2] && LevelChecked(Chainsaw) && !battery)))
                {
                    actionId = Reassemble;
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Chainsaw) &&
                    reassembledChainsaw &&
                    LevelChecked(Chainsaw) &&
                    !battery &&
                    (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(Chainsaw)))
                {
                    actionId = Chainsaw;
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor) &&
                     reassembledAnchor &&
                     LevelChecked(OriginalHook(AirAnchor)) &&
                     !battery &&
                     (GetCooldownRemainingTime(OriginalHook(AirAnchor)) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(OriginalHook(AirAnchor))))
                {
                    actionId = OriginalHook(AirAnchor);
                    return true;
                }

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Drill) &&
                    reassembledDrill &&
                    LevelChecked(Drill) &&
                    (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(Drill)))
                {
                    actionId = Drill;
                    return true;
                }

                return false;
            }

            private static bool UseQueen(MCHGauge gauge)
            {
                if (IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) && Config.MCH_ST_TurretUsage == 1 &&
                    CanWeave(OriginalHook(SplitShot)) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                    LevelChecked(OriginalHook(RookAutoturret)) && !gauge.IsRobotActive && gauge.Battery >= 50)
                {
                    var queensUsed = ActionWatching.CombatActions.Count(x => x == OriginalHook(RookAutoturret));

                    if (queensUsed < 2)
                        return true;

                    if (queensUsed >= 2 && queensUsed % 2 == 0 && gauge.Battery == 100)
                        return true;

                    if (queensUsed >= 2 && queensUsed % 2 == 1 && gauge.Battery >= 80)
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
                if (actionID is SpreadShot)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

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

                    if ((LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25) || ActionReady(Chainsaw))
                        return Chainsaw;

                    if (ActionReady(BioBlaster))
                        return OriginalHook(BioBlaster);

                    if (ActionReady(Flamethrower) && !IsMoving)
                        return OriginalHook(Flamethrower);

                    if (!gauge.IsOverheated && gauge.Battery >= Config.MCH_AoE_TurretUsage)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge        
                    if (gauge.Heat >= 50 && LevelChecked(Hypercharge) && LevelChecked(AutoCrossbow) && !gauge.IsOverheated &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) || !BioBlaster.LevelChecked()) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) || !Flamethrower.LevelChecked()))
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if ((WasLastAction(SpreadShot) || WasLastAction(AutoCrossbow) || Config.MCH_AoE_Hypercharge) && CanWeave(actionID) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                    {
                        if (LevelChecked(GaussRound) && GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                            return GaussRound;

                        if (LevelChecked(Ricochet) && GetRemainingCharges(Ricochet) > GetRemainingCharges(GaussRound))
                            return Ricochet;
                    }

                    if (gauge.IsOverheated && AutoCrossbow.LevelChecked())
                        return OriginalHook(AutoCrossbow);

                    //gauss and ricochet overcap protection
                    if (CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                    {
                        if (GetRemainingCharges(GaussRound) >= 2 && LevelChecked(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) >= 2 && LevelChecked(Ricochet))
                            return Ricochet;
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
                if (actionID is SpreadShot or Scattergun)
                {
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();
                    bool reassembledScattergun = IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[0] && HasEffect(Buffs.Reassembled);
                    bool reassembledCrossbow = (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[1] && HasEffect(Buffs.Reassembled)) || (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[1] && !HasEffect(Buffs.Reassembled)) || (!IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble));
                    bool reassembledChainsaw = (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[2] && HasEffect(Buffs.Reassembled)) || (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[2] && !HasEffect(Buffs.Reassembled)) || (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) || (!IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble));


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

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !HasEffect(Buffs.Wildfire) &&
                        !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                        GetRemainingCharges(Reassemble) > Config.MCH_AoE_ReassemblePool &&
                        ((Config.MCH_AoE_Reassembled[0] && Scattergun.LevelChecked()) ||
                        (gauge.IsOverheated && Config.MCH_AoE_Reassembled[1] && AutoCrossbow.LevelChecked()) ||
                        (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Config.MCH_AoE_Reassembled[2] && Chainsaw.LevelChecked())))
                        return Reassemble;

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

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster) && ActionReady(BioBlaster))
                        return OriginalHook(BioBlaster);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower) && ActionReady(Flamethrower) && !IsMoving)
                        return OriginalHook(Flamethrower);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Queen) && !gauge.IsOverheated)
                    {
                        if (gauge.Battery >= Config.MCH_AoE_TurretUsage)
                            return OriginalHook(RookAutoturret);
                    }

                    // Hypercharge        
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                        gauge.Heat >= 50 && LevelChecked(Hypercharge) && LevelChecked(AutoCrossbow) && !gauge.IsOverheated &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) || !BioBlaster.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster)) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) || !Flamethrower.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower)))
                        return Hypercharge;

                    //Heatblast, Gauss, Rico
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && CanWeave(actionID) &&
                        (Config.MCH_AoE_Hypercharge || (!Config.MCH_AoE_Hypercharge && gauge.IsOverheated)))
                    {
                        if ((WasLastAction(SpreadShot) || WasLastAction(AutoCrossbow) || Config.MCH_AoE_Hypercharge) &&
                            ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                        {
                            if (LevelChecked(GaussRound) && GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                return GaussRound;

                            if (LevelChecked(Ricochet) && GetRemainingCharges(Ricochet) > GetRemainingCharges(GaussRound))
                                return Ricochet;
                        }
                    }

                    if (gauge.IsOverheated && AutoCrossbow.LevelChecked())
                        return OriginalHook(AutoCrossbow);

                    //gauss and ricochet overcap protection
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) &&
                        CanWeave(actionID) && !gauge.IsOverheated && !HasEffect(Buffs.Wildfire) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                    {
                        if (GetRemainingCharges(GaussRound) >= 2 && LevelChecked(GaussRound))
                            return GaussRound;

                        if (GetRemainingCharges(Ricochet) >= 2 && LevelChecked(Ricochet))
                            return Ricochet;
                    }

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
                        ActionReady(BarrelStabilizer) &&
                        gauge.Heat < 50 &&
                        !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_Heatblast_Wildfire) &&
                        ActionReady(Hypercharge) &&
                        ActionReady(Wildfire) &&
                        gauge.Heat >= 50)
                        return Wildfire;

                    if (!gauge.IsOverheated && LevelChecked(Hypercharge) && gauge.Heat >= 50)
                        return Hypercharge;

                    if (GetCooldownRemainingTime(Heatblast) < 0.7 && LevelChecked(Heatblast)) // Prioritize Heat Blast
                        return Heatblast;

                    if (IsEnabled(CustomComboPreset.MCH_Heatblast_GaussRound) && gauge.IsOverheated)
                    {
                        if (!LevelChecked(Ricochet))
                            return GaussRound;

                        if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
                            return GaussRound;
                        return Ricochet;
                    }
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
                    var gaussCharges = GetRemainingCharges(GaussRound);
                    var ricochetCharges = GetRemainingCharges(Ricochet);

                    // Prioritize the original if both are off cooldown

                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (gaussCharges >= ricochetCharges)
                        return GaussRound;
                    else if (ricochetCharges > 0)
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

        internal class MCH_HotShotDrillChainsaw : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainsaw;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Drill || actionID is HotShot || actionID is AirAnchor || actionID is Chainsaw)
                {
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
                if (actionID is AutoCrossbow)
                {
                    var heatBlastCD = GetCooldown(Heatblast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    MCHGauge? gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) &&
                        gauge.Heat < 50 &&
                        !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (!gauge.IsOverheated && ActionReady(Hypercharge) && gauge.Heat >= 50)
                        return Hypercharge;

                    if (heatBlastCD.CooldownRemaining < 0.7 && LevelChecked(AutoCrossbow)) // prioritize autocrossbow
                        return AutoCrossbow;

                    if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_GaussRound) && gauge.IsOverheated)
                    {
                        if (!LevelChecked(Ricochet))
                            return GaussRound;
                        if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                            return GaussRound;
                        else
                            return Ricochet;
                    }
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