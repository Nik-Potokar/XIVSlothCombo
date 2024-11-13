using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using static XIVSlothCombo.Combos.JobHelpers.MCHHelpers;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.PvE;

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

    protected static MCHGauge? Gauge = GetJobGauge<MCHGauge>();

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
            EnhancedMultiWeapon = 605;
    }

    public static class Config
    {
        public static UserInt
            MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold", 25),
            MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold", 25),
            MCH_VariantCure = new("MCH_VariantCure"),
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
        internal static MCHOpenerLogic MCHOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy() && CanDelayedWeave(actionID);
            float heatblastRC = GetCooldown(Heatblast).CooldownTotal;

            bool drillCD = !LevelChecked(Drill) || (!TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
                                                    GetCooldownRemainingTime(Drill) > heatblastRC * 6) ||
                           (TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
                            GetRemainingCharges(Drill) < GetMaxCharges(Drill) &&
                            GetCooldownRemainingTime(Drill) > heatblastRC * 6);

            bool anchorCD = !LevelChecked(AirAnchor) ||
                            (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) > heatblastRC * 6);

            bool sawCD = !LevelChecked(Chainsaw) ||
                         (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);
            float GCD = GetCooldown(OriginalHook(SplitShot)).CooldownTotal;
            int BSUsed = ActionWatching.CombatActions.Count(x => x == BarrelStabilizer);

            if (actionID is SplitShot or HeatedSplitShot)
            {
                if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanWeave(actionID))
                    return Variant.VariantRampart;

                // Opener
                if (MCHOpener.DoFullOpener(ref actionID))
                    return actionID;

                // Interrupt
                if (interruptReady)
                    return All.HeadGraze;

                // All weaves
                if (CanWeave(ActionWatching.LastWeaponskill) &&
                    !ActionWatching.HasDoubleWeaved())
                {
                    // Wildfire
                    if (JustUsed(Hypercharge) && ActionReady(Wildfire))
                        return Wildfire;

                    // BarrelStabilizer
                    if (!Gauge.IsOverheated && ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    // Hypercharge
                    if ((Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && !MCHExtensions.IsComboExpiring(6) &&
                        LevelChecked(Hypercharge) && !Gauge.IsOverheated)
                    {
                        // Ensures Hypercharge is double weaved with WF
                        if ((LevelChecked(FullMetalField) && JustUsed(FullMetalField) &&
                             (GetCooldownRemainingTime(Wildfire) < GCD || ActionReady(Wildfire))) ||
                            (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        // Only Hypercharge when tools are on cooldown
                        if (drillCD && anchorCD && sawCD &&
                            ((GetCooldownRemainingTime(Wildfire) > 40 && LevelChecked(Wildfire)) ||
                             !LevelChecked(Wildfire)))
                            return Hypercharge;
                    }

                    //Queen
                    if (MCHExtensions.UseQueen(Gauge) &&
                        GetCooldownRemainingTime(Wildfire) > GCD)
                        return OriginalHook(RookAutoturret);

                    // Gauss Round and Ricochet during HC
                    if (JustUsed(OriginalHook(Heatblast)) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) !=
                        ActionWatching.ActionAttackType.Ability)
                    {
                        if (ActionReady(OriginalHook(GaussRound)) &&
                            GetRemainingCharges(OriginalHook(GaussRound)) >=
                            GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) &&
                            GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    // Gauss Round and Ricochet outside HC
                    if (!Gauge.IsOverheated &&
                        (JustUsed(OriginalHook(AirAnchor)) || JustUsed(Chainsaw) ||
                         JustUsed(Drill) || JustUsed(Excavator)))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && !JustUsed(OriginalHook(GaussRound)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && !JustUsed(OriginalHook(Ricochet)))
                            return OriginalHook(Ricochet);
                    }

                    // Healing
                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind) && !Gauge.IsOverheated)
                        return All.SecondWind;
                }

                // Full Metal Field
                if (HasEffect(Buffs.FullMetalMachinist) &&
                    (GetCooldownRemainingTime(Wildfire) <= GCD || ActionReady(Wildfire) ||
                     GetBuffRemainingTime(Buffs.FullMetalMachinist) <= 6) &&
                    LevelChecked(FullMetalField))
                    return FullMetalField;

                // Heatblast
                if (Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                    return OriginalHook(Heatblast);

                // Reassemble and Tools
                if (ReassembledTools(ref actionID, Gauge))
                    return actionID;

                // Excavator
                if (LevelChecked(Excavator) &&
                    HasEffect(Buffs.ExcavatorReady) &&
                    (BSUsed is 1 ||
                     (BSUsed % 3 is 2 && Gauge.Battery <= 40) ||
                     (BSUsed % 3 is 0 && Gauge.Battery <= 50) ||
                     (BSUsed % 3 is 1 && Gauge.Battery <= 60) ||
                     GetBuffRemainingTime(Buffs.ExcavatorReady) < 6))
                    return OriginalHook(Chainsaw);

                // 1-2-3 Combo
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

        private static bool ReassembledTools(ref uint actionID, MCHGauge gauge)
        {
            bool battery = Svc.Gauges.Get<MCHGauge>().Battery >= 100;

            if (!gauge.IsOverheated && !JustUsed(OriginalHook(Heatblast)) && !ActionWatching.HasDoubleWeaved() &&
                !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                (CanWeave(ActionWatching.LastWeaponskill) || !InCombat()) &
                ((LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady) && !battery && gauge.IsRobotActive &&
                  GetCooldownRemainingTime(Wildfire) > 3) ||
                 (LevelChecked(Chainsaw) && !LevelChecked(Excavator) &&
                  (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(Chainsaw)) && !battery) ||
                 (LevelChecked(AirAnchor) &&
                  (GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(AirAnchor)) && !battery) ||
                 (LevelChecked(Drill) && !LevelChecked(AirAnchor) &&
                  (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(Drill)))))
            {
                actionID = Reassemble;

                return true;
            }

            if (LevelChecked(Chainsaw) &&
                !battery &&
                (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Chainsaw)))
            {
                actionID = Chainsaw;

                return true;
            }

            if (LevelChecked(OriginalHook(AirAnchor)) &&
                !battery &&
                (GetCooldownRemainingTime(OriginalHook(AirAnchor)) <=
                    GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(OriginalHook(AirAnchor))))
            {
                actionID = OriginalHook(AirAnchor);

                return true;
            }

            if (LevelChecked(Drill) &&
                !JustUsed(Drill) &&
                (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Drill)) &&
                GetCooldownRemainingTime(Wildfire) is >= 20 or <= 10)
            {
                actionID = Drill;

                return true;
            }

            return false;
        }
    }

    internal class MCH_ST_AdvancedMode : CustomCombo
    {
        internal static MCHOpenerLogic MCHOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool interruptReady = ActionReady(All.HeadGraze) && CanInterruptEnemy() && CanDelayedWeave(actionID);
            float heatblastRC = GetCooldown(Heatblast).CooldownTotal;

            bool drillCD = !LevelChecked(Drill) || (!TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
                                                    GetCooldownRemainingTime(Drill) > heatblastRC * 6) ||
                           (TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
                            GetRemainingCharges(Drill) < GetMaxCharges(Drill) &&
                            GetCooldownRemainingTime(Drill) > heatblastRC * 6);

            bool anchorCD = !LevelChecked(AirAnchor) ||
                            (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) > heatblastRC * 6);

            bool sawCD = !LevelChecked(Chainsaw) ||
                         (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);
            float GCD = GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

            bool reassembledExcavator =
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[0] &&
                 (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[0] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);
            int BSUsed = ActionWatching.CombatActions.Count(x => x == BarrelStabilizer);

            if (actionID is SplitShot or HeatedSplitShot)
            {
                if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanWeave(actionID))
                    return Variant.VariantRampart;

                // Opener
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener))
                    if (MCHOpener.DoFullOpener(ref actionID))
                        return actionID;

                // Interrupt
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Interrupt) && interruptReady)
                    return All.HeadGraze;

                // All weaves
                if (CanWeave(ActionWatching.LastWeaponskill) &&
                    !ActionWatching.HasDoubleWeaved())
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_QueenOverdrive) &&
                        Gauge.IsRobotActive && GetTargetHPPercent() <= Config.MCH_ST_QueenOverDrive &&
                        ActionReady(OriginalHook(RookOverdrive)))
                        return OriginalHook(RookOverdrive);

                    // Wildfire
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_WildFire) &&
                        JustUsed(Hypercharge) && ActionReady(Wildfire) &&
                        GetTargetHPPercent() >= Config.MCH_ST_WildfireHP)
                        return Wildfire;

                    // BarrelStabilizer
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer) &&
                        !Gauge.IsOverheated && ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    // Hypercharge
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Hypercharge) &&
                        (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && !MCHExtensions.IsComboExpiring(6) &&
                        LevelChecked(Hypercharge) && !Gauge.IsOverheated &&
                        GetTargetHPPercent() >= Config.MCH_ST_HyperchargeHP)
                    {
                        // Ensures Hypercharge is double weaved with WF
                        if ((LevelChecked(FullMetalField) && JustUsed(FullMetalField) &&
                             (GetCooldownRemainingTime(Wildfire) < GCD || ActionReady(Wildfire))) ||
                            (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        // Only Hypercharge when tools are on cooldown
                        if (drillCD && anchorCD && sawCD &&
                            ((GetCooldownRemainingTime(Wildfire) > 40 && LevelChecked(Wildfire)) ||
                             !LevelChecked(Wildfire)))
                            return Hypercharge;
                    }

                    // Queen
                    if (IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) &&
                        MCHExtensions.UseQueen(Gauge) &&
                        GetCooldownRemainingTime(Wildfire) > GCD)
                        return OriginalHook(RookAutoturret);

                    // Gauss Round and Ricochet during HC
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                        JustUsed(OriginalHook(Heatblast)) &&
                        ActionWatching.GetAttackType(ActionWatching.LastAction) !=
                        ActionWatching.ActionAttackType.Ability)
                    {
                        if (ActionReady(OriginalHook(GaussRound)) &&
                            GetRemainingCharges(OriginalHook(GaussRound)) >=
                            GetRemainingCharges(OriginalHook(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) &&
                            GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                            return OriginalHook(Ricochet);
                    }

                    // Gauss Round and Ricochet outside HC
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                        !Gauge.IsOverheated &&
                        (JustUsed(OriginalHook(AirAnchor)) || JustUsed(Chainsaw) ||
                         JustUsed(Drill) || JustUsed(Excavator)))
                    {
                        if (ActionReady(OriginalHook(GaussRound)) && !JustUsed(OriginalHook(GaussRound)))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) && !JustUsed(OriginalHook(Ricochet)))
                            return OriginalHook(Ricochet);
                    }

                    // Healing
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_SecondWind) &&
                        PlayerHealthPercentageHp() <= Config.MCH_ST_SecondWindThreshold &&
                        ActionReady(All.SecondWind) && !Gauge.IsOverheated)
                        return All.SecondWind;
                }

                // Full Metal Field
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                    HasEffect(Buffs.FullMetalMachinist) &&
                    (GetCooldownRemainingTime(Wildfire) <= GCD || ActionReady(Wildfire) ||
                     GetBuffRemainingTime(Buffs.FullMetalMachinist) <= 6) &&
                    LevelChecked(FullMetalField))
                    return FullMetalField;

                // Heatblast
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Heatblast) &&
                    Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                    return OriginalHook(Heatblast);

                // Reassemble and Tools
                if (ReassembledTools(ref actionID, Gauge))
                    return actionID;

                // Excavator
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Excavator) &&
                    reassembledExcavator &&
                    LevelChecked(Excavator) &&
                    HasEffect(Buffs.ExcavatorReady) &&
                    (BSUsed is 1 ||
                     (BSUsed % 3 is 2 && Gauge.Battery <= 40) ||
                     (BSUsed % 3 is 0 && Gauge.Battery <= 50) ||
                     (BSUsed % 3 is 1 && Gauge.Battery <= 60) ||
                     GetBuffRemainingTime(Buffs.ExcavatorReady) < 6))
                    return OriginalHook(Chainsaw);

                // 1-2-3 Combo
                if (comboTime > 0)
                {
                    if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                        return OriginalHook(SlugShot);

                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[4] &&
                        lastComboMove == OriginalHook(SlugShot) &&
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
            bool battery = Svc.Gauges.Get<MCHGauge>().Battery >= 100;

            bool reassembledChainsaw =
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[1] &&
                 (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[1] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

            bool reassembledAnchor =
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[2] &&
                 (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[2] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

            bool reassembledDrill =
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[3] &&
                 (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
                (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[3] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) &&
                !gauge.IsOverheated && !JustUsed(OriginalHook(Heatblast)) && !ActionWatching.HasDoubleWeaved() &&
                !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                (CanWeave(ActionWatching.LastWeaponskill) || !InCombat()) &&
                GetRemainingCharges(Reassemble) > Config.MCH_ST_ReassemblePool &&
                ((Config.MCH_ST_Reassembled[0] && LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady) &&
                  !battery && gauge.IsRobotActive && GetCooldownRemainingTime(Wildfire) > 3) ||
                 (Config.MCH_ST_Reassembled[1] && LevelChecked(Chainsaw) &&
                  (!LevelChecked(Excavator) || !Config.MCH_ST_Reassembled[0]) &&
                  (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(Chainsaw)) && !battery) ||
                 (Config.MCH_ST_Reassembled[2] && LevelChecked(AirAnchor) &&
                  (GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(AirAnchor)) && !battery) ||
                 (Config.MCH_ST_Reassembled[3] && LevelChecked(Drill) &&
                  (!LevelChecked(AirAnchor) || !Config.MCH_ST_Reassembled[2]) &&
                  (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                   ActionReady(Drill)))))
            {
                actionID = Reassemble;

                return true;
            }

            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Chainsaw) &&
                reassembledChainsaw &&
                LevelChecked(Chainsaw) &&
                !battery &&
                (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Chainsaw)))
            {
                actionID = Chainsaw;

                return true;
            }

            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor) &&
                reassembledAnchor &&
                LevelChecked(OriginalHook(AirAnchor)) &&
                !battery &&
                (GetCooldownRemainingTime(OriginalHook(AirAnchor)) <=
                    GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(OriginalHook(AirAnchor))))
            {
                actionID = OriginalHook(AirAnchor);

                return true;
            }

            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Drill) &&
                reassembledDrill &&
                LevelChecked(Drill) &&
                !JustUsed(Drill) &&
                (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Drill)) &&
                GetCooldownRemainingTime(Wildfire) is >= 20 or <= 10)
            {
                actionID = Drill;

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
            float GCD = GetCooldown(OriginalHook(SpreadShot)).CooldownTotal;

            if (actionID is SpreadShot or Scattergun)
            {
                if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                    return Variant.VariantCure;

                if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower))
                    return OriginalHook(11);

                if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanWeave(actionID))
                    return Variant.VariantRampart;

                //Full Metal Field
                if (HasEffect(Buffs.FullMetalMachinist) && LevelChecked(FullMetalField))
                    return FullMetalField;

                // BarrelStabilizer 
                if (!Gauge.IsOverheated && CanWeave(actionID) && ActionReady(BarrelStabilizer))
                    return BarrelStabilizer;

                if (ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster))
                    return OriginalHook(BioBlaster);

                if (ActionReady(Flamethrower) && !IsMoving)
                    return OriginalHook(Flamethrower);

                if (!Gauge.IsOverheated && Gauge.Battery == 100)
                    return OriginalHook(RookAutoturret);

                // Hypercharge        
                if ((Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                    LevelChecked(AutoCrossbow) && !Gauge.IsOverheated &&
                    ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) ||
                     !BioBlaster.LevelChecked()) &&
                    ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) ||
                     !Flamethrower.LevelChecked()))
                    return Hypercharge;

                //AutoCrossbow, Gauss, Rico
                if (CanWeave(actionID) && JustUsed(OriginalHook(AutoCrossbow)) &&
                    ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }

                if (Gauge.IsOverheated && AutoCrossbow.LevelChecked())
                    return OriginalHook(AutoCrossbow);

                if (!HasEffect(Buffs.Wildfire) &&
                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                    (Scattergun.LevelChecked() ||
                     (Gauge.IsOverheated && AutoCrossbow.LevelChecked()) ||
                     (GetCooldownRemainingTime(Chainsaw) < 1 && Chainsaw.LevelChecked()) ||
                     (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Excavator.LevelChecked())))
                    return Reassemble;

                if (LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                    return OriginalHook(Chainsaw);

                if ((LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) <= GCD + 0.25) ||
                    ActionReady(Chainsaw))
                    return Chainsaw;

                if (LevelChecked(AutoCrossbow) && Gauge.IsOverheated)
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
            float GCD = GetCooldown(OriginalHook(SpreadShot)).CooldownTotal;

            bool reassembledScattergun = IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) &&
                                         Config.MCH_AoE_Reassembled[0] && HasEffect(Buffs.Reassembled);

            bool reassembledCrossbow =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[1] &&
                 HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[1] &&
                 !HasEffect(Buffs.Reassembled)) || !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            bool reassembledChainsaw =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[2] &&
                 HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[2] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            bool reassembledExcavator =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[3] &&
                 HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[3] &&
                 !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            if (actionID is SpreadShot or Scattergun)
            {
                if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                    return Variant.VariantCure;

                if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower))
                    return OriginalHook(11);

                if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanWeave(actionID))
                    return Variant.VariantRampart;

                //Full Metal Field
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer_FullMetalField) &&
                    HasEffect(Buffs.FullMetalMachinist) && LevelChecked(FullMetalField))
                    return FullMetalField;

                // BarrelStabilizer
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer) &&
                    !Gauge.IsOverheated && CanWeave(actionID) && ActionReady(BarrelStabilizer))
                    return BarrelStabilizer;

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster) &&
                    ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster))
                    return OriginalHook(BioBlaster);

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower) &&
                    ActionReady(Flamethrower) && !IsMoving)
                    return OriginalHook(Flamethrower);

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Queen) && !Gauge.IsOverheated)
                    if (Gauge.Battery >= Config.MCH_AoE_TurretUsage)
                        return OriginalHook(RookAutoturret);

                // Hypercharge        
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                    (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                    LevelChecked(AutoCrossbow) && !Gauge.IsOverheated &&
                    ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) ||
                     !BioBlaster.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster)) &&
                    ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) ||
                     !Flamethrower.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower)))
                    return Hypercharge;

                //AutoCrossbow, Gauss, Rico
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && !Config.MCH_AoE_Hypercharge &&
                    CanWeave(actionID) && JustUsed(OriginalHook(AutoCrossbow)) &&
                    ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }

                if (Gauge.IsOverheated && AutoCrossbow.LevelChecked())
                    return OriginalHook(AutoCrossbow);

                //gauss and ricochet outside HC
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) && Config.MCH_AoE_Hypercharge &&
                    CanWeave(actionID) && !Gauge.IsOverheated)
                {
                    if (ActionReady(OriginalHook(GaussRound)) && !JustUsed(OriginalHook(GaussRound)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) && !JustUsed(OriginalHook(Ricochet)))
                        return OriginalHook(Ricochet);
                }

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !HasEffect(Buffs.Wildfire) &&
                    !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                    GetRemainingCharges(Reassemble) > Config.MCH_AoE_ReassemblePool &&
                    ((Config.MCH_AoE_Reassembled[0] && Scattergun.LevelChecked()) ||
                     (Gauge.IsOverheated && Config.MCH_AoE_Reassembled[1] && AutoCrossbow.LevelChecked()) ||
                     (GetCooldownRemainingTime(Chainsaw) < 1 && Config.MCH_AoE_Reassembled[2] &&
                      Chainsaw.LevelChecked()) ||
                     (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Config.MCH_AoE_Reassembled[3] &&
                      Excavator.LevelChecked())))
                    return Reassemble;

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Excavator) &&
                    reassembledExcavator &&
                    LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                    return OriginalHook(Chainsaw);

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Chainsaw) &&
                    reassembledChainsaw &&
                    LevelChecked(Chainsaw) &&
                    (GetCooldownRemainingTime(Chainsaw) <= GCD + 0.25 || ActionReady(Chainsaw)))
                    return Chainsaw;

                if (reassembledScattergun)
                    return OriginalHook(Scattergun);

                if (reassembledCrossbow &&
                    LevelChecked(AutoCrossbow) && Gauge.IsOverheated)
                    return AutoCrossbow;

                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_SecondWind))
                    if (PlayerHealthPercentageHp() <= Config.MCH_AoE_SecondWindThreshold && ActionReady(All.SecondWind))
                        return All.SecondWind;
            }

            return actionID;
        }
    }

    internal class MCH_HeatblastGaussRicochet : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Heatblast;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Heatblast or BlazingShot)
            {
                if (IsEnabled(CustomComboPreset.MCH_Heatblast_AutoBarrel) &&
                    ActionReady(BarrelStabilizer) && !Gauge.IsOverheated)
                    return BarrelStabilizer;

                if (IsEnabled(CustomComboPreset.MCH_Heatblast_Wildfire) &&
                    ActionReady(Wildfire) &&
                    JustUsed(Hypercharge))
                    return Wildfire;

                if (!Gauge.IsOverheated && LevelChecked(Hypercharge) &&
                    (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                    return Hypercharge;

                //Heatblast, Gauss, Rico
                if (IsEnabled(CustomComboPreset.MCH_Heatblast_GaussRound) &&
                    CanWeave(actionID) && JustUsed(OriginalHook(Heatblast)) &&
                    ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }

                if (Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                    return OriginalHook(Heatblast);
            }

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
                if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_AutoBarrel) &&
                    ActionReady(BarrelStabilizer) && !Gauge.IsOverheated)
                    return BarrelStabilizer;

                if (!Gauge.IsOverheated && LevelChecked(Hypercharge) &&
                    (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                    return Hypercharge;

                //Autocrossbow, Gauss, Rico
                if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_GaussRound) && CanWeave(actionID) &&
                    JustUsed(OriginalHook(AutoCrossbow)) &&
                    ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }

                if (Gauge.IsOverheated && LevelChecked(OriginalHook(AutoCrossbow)))
                    return OriginalHook(AutoCrossbow);
            }

            return actionID;
        }
    }

    internal class MCH_GaussRoundRicochet : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_GaussRoundRicochet;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is GaussRound or Ricochet or CheckMate or DoubleCheck)
            {
                if (ActionReady(OriginalHook(GaussRound)) &&
                    GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet)))
                    return OriginalHook(GaussRound);

                if (ActionReady(OriginalHook(Ricochet)) &&
                    GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                    return OriginalHook(Ricochet);
            }

            return actionID;
        }
    }

    internal class MCH_Overdrive : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RookAutoturret or AutomatonQueen && Gauge.IsRobotActive)
                return OriginalHook(QueenOverdrive);

            return actionID;
        }
    }

    internal class MCH_HotShotDrillChainsawExcavator : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.MCH_HotShotDrillChainsawExcavator;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Drill or HotShot or AirAnchor or Chainsaw)
                return LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady)
                    ? CalcBestAction(actionID, Excavator, Chainsaw, AirAnchor, Drill)
                    : LevelChecked(Chainsaw)
                        ? CalcBestAction(actionID, Chainsaw, AirAnchor, Drill)
                        : LevelChecked(AirAnchor)
                            ? CalcBestAction(actionID, AirAnchor, Drill)
                            : LevelChecked(Drill)
                                ? CalcBestAction(actionID, Drill, HotShot)
                                : HotShot;

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

    internal class All_PRanged_Dismantle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.All_PRanged_Dismantle;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Dismantle && TargetHasEffectAny(Debuffs.Dismantled) && IsOffCooldown(Dismantle))
                return OriginalHook(11);

            return actionID;
        }
    }
}