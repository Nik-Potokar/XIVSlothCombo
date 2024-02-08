using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            PiercingTalon = 90,
            ElusiveJump = 94,
            LanceCharge = 85,
            DragonSight = 7398,
            BattleLitany = 3557,
            Jump = 92,
            LifeSurge = 83,
            HighJump = 16478,
            MirageDive = 7399,
            BloodOfTheDragon = 3553,
            Stardiver = 16480,
            CoerthanTorment = 16477,
            DoomSpike = 86,
            SonicThrust = 7397,
            ChaosThrust = 88,
            RaidenThrust = 16479,
            TrueThrust = 75,
            Disembowel = 87,
            FangAndClaw = 3554,
            WheelingThrust = 3556,
            FullThrust = 84,
            VorpalThrust = 78,
            WyrmwindThrust = 25773,
            DraconianFury = 25770,
            ChaoticSpring = 25772,
            DragonfireDive = 96,
            SpineshatterDive = 95,
            Geirskogul = 3555,
            Nastrond = 7400,
            HeavensThrust = 25771;

        public static class Buffs
        {
            public const ushort
                LanceCharge = 1864,
                RightEye = 1910,
                BattleLitany = 786,
                SharperFangAndClaw = 802,
                EnhancedWheelingThrust = 803,
                DiveReady = 1243,
                RaidenThrustReady = 1863,
                PowerSurge = 2720,
                LifeSurge = 116,
                DraconianFire = 1863;
        }

        public static class Debuffs
        {
            public const ushort
                ChaosThrust = 118,
                ChaoticSpring = 2719;
        }

        public static class Traits
        {
            public const uint
                EnhancedSpineshatterDive = 436,
                EnhancedLifeSurge = 438;
        }

        public static class Config
        {
            public static UserInt
                DRG_Opener_Choice = new("DRG_OpenerChoice"),
                DRG_Variant_Cure = new("DRG_VariantCure"),
                DRG_ST_LitanyHP = new("DRG_ST_LitanyHP"),
                DRG_ST_SightHP = new("DRG_ST_SightHP"),
                DRG_ST_LanceChargeHP = new("DRG_ST_LanceChargeHP"),
                DRG_ST_SecondWind_Threshold = new("DRG_STSecondWindThreshold"),
                DRG_ST_Bloodbath_Threshold = new("DRG_STBloodbathThreshold"),
                DRG_AoE_LitanyHP = new("DRG_AoE_LitanyHP"),
                DRG_AoE_SightHP = new("DRG_AoE_SightHP"),
                DRG_AoE_LanceChargeHP = new("DRG_AoE_LanceChargeHP"),
                DRG_AoE_SecondWind_Threshold = new("DRG_AoESecondWindThreshold"),
                DRG_AoEBloodbath_Threshold = new("DRG_AoEBloodbathThreshold");
            public static UserBool
                DRG_ST_TrueNorth_Moving = new("DRG_ST_TrueNorth_Moving"),
                DRG_ST_TrueNorth_FirstOnly = new("DRG_ST_TrueNorth_FirstOnly");
            public static UserBoolArray
                DRG_ST_DivesOption_Dragonfire = new("DRG_ST_DivesOption_Dragonfire"),
                DRG_ST_DivesOption_Spineshatter = new("DRG_ST_DivesOption_Spineshatter"),
                DRG_AoE_DivesOption_Dragonfire = new("DRG_AoE_DivesOption_Dragonfire"),
                DRG_AoE_DivesOption_Spineshatter = new("DRG_AoE_DivesOption_Spineshatter");
        }

        internal class DRG_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;
            internal static DRGOpenerLogic DRGOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;
                bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Opener for DRG
                    if (DRGOpener.DoFullOpener(ref actionID, true))
                        return actionID;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Lance Charge Feature
                        if (ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Dragon Sight Feature
                        if (ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight))
                            return DragonSight;

                        //Life Surge Feature
                        if (!HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) &&
                            ((HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                            (HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                            (HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && (HasEffect(Buffs.EnhancedWheelingThrust) || HasEffect(Buffs.SharperFangAndClaw))) ||
                            (IsOnCooldown(DragonSight) && IsOnCooldown(LanceCharge) && lastComboMove is VorpalThrust)))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                            return WyrmwindThrust;

                        //Dives Feature
                        if (!IsMoving && LevelChecked(LanceCharge))
                        {
                            if ((!TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                               (HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye))) //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                            {
                                if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                    return DragonfireDive;

                                if (ActionReady(SpineshatterDive) && AnimationLock.CanDRGWeave(SpineshatterDive))
                                    return SpineshatterDive;
                            }
                        }

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                            return OriginalHook(Jump);

                        //Geirskogul and Nastrond Feature
                        if (IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)) && AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)))
                            return OriginalHook(Geirskogul);

                        //Mirage Feature
                        if (IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady) && AnimationLock.CanDRGWeave(MirageDive))
                            return MirageDive;

                        //StarDives Feature
                        if (gauge.IsLOTDActive && ActionReady(Stardiver) && AnimationLock.CanDRGWeave(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                            return Stardiver;
                    }

                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                    {
                        // If we are not on the flank, but need to use Fangs, pop true north if not already up
                        if (trueNorthReady && CanDelayedWeave(actionID) &&
                            !OnTargetsFlank() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(FangAndClaw);
                    }

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                    {
                        // If we are not on the rear, but need to use Wheeling, pop true north if not already up
                        if (trueNorthReady && CanDelayedWeave(All.TrueNorth) &&
                            !OnTargetsRear() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(WheelingThrust);
                    }

                    if (comboTime > 0)
                    {
                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove is Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return trueNorthReady && CanDelayedWeave(All.TrueNorth) &&
                                    !OnTargetsRear() && !HasEffect(Buffs.RightEye)
                                    ? All.TrueNorth
                                    : OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                            return VorpalThrust;

                        if (lastComboMove is VorpalThrust && LevelChecked(FullThrust))
                            return OriginalHook(FullThrust);
                    }

                    return OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_AdvancedMode;
            internal static DRGOpenerLogic DRGOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;
                bool dragonfireAny = Config.DRG_ST_DivesOption_Dragonfire.All(x => x == false);
                bool spineshatterAny = Config.DRG_ST_DivesOption_Spineshatter.All(x => x == false);
                bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);
                bool tnMoving = (Config.DRG_ST_TrueNorth_Moving && !IsMoving) || (!Config.DRG_ST_TrueNorth_Moving);
                bool tnFirstOnly = (Config.DRG_ST_TrueNorth_FirstOnly && !WasLastWeaponskill(OriginalHook(WheelingThrust)) && !WasLastWeaponskill(OriginalHook(FangAndClaw)) && !WasLastWeaponskill(OriginalHook(ChaosThrust))) || (!Config.DRG_ST_TrueNorth_FirstOnly);
                bool allowedToTN = tnMoving && tnFirstOnly;

                if (LevelChecked(ChaoticSpring))
                    ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Opener for BLM
                    if (IsEnabled(CustomComboPreset.DRG_ST_Opener))
                    {
                        if (DRGOpener.DoFullOpener(ref actionID, false))
                            return actionID;
                    }

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) &&
                        LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                        {
                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LitanyHP)
                                return BattleLitany;

                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LanceChargeHP)
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) &&
                                ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight) &&
                                GetTargetHPPercent() >= Config.DRG_ST_SightHP)
                                return DragonSight;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && ActionReady(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                                ((HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                (HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                (HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && (HasEffect(Buffs.EnhancedWheelingThrust) || HasEffect(Buffs.SharperFangAndClaw))) ||
                                (IsOnCooldown(DragonSight) && IsOnCooldown(LanceCharge) && lastComboMove is VorpalThrust)))
                                return LifeSurge;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                                gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                                return WyrmwindThrust;

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_Dragonfire) && !IsMoving && LevelChecked(LanceCharge))
                            {
                                if (dragonfireAny || //Dives on cooldown
                                   (((Config.DRG_ST_DivesOption_Dragonfire[0] && HasEffect(Buffs.LanceCharge)) || (!Config.DRG_ST_DivesOption_Dragonfire[0]) || (!LanceCharge.LevelChecked())) &&
                                   ((Config.DRG_ST_DivesOption_Dragonfire[1] && HasEffect(Buffs.RightEye)) || (!Config.DRG_ST_DivesOption_Dragonfire[1]) || (!DragonSight.LevelChecked())) &&
                                   ((Config.DRG_ST_DivesOption_Dragonfire[2] && HasEffect(Buffs.BattleLitany)) || (!Config.DRG_ST_DivesOption_Dragonfire[2]) || (!BattleLitany.LevelChecked()))))
                                {
                                    if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                        return DragonfireDive;

                                }
                            }

                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_Spineshatter) && !IsMoving && LevelChecked(LanceCharge))
                            {
                                if (spineshatterAny || //Dives on cooldown
                                   (((Config.DRG_ST_DivesOption_Spineshatter[0] && HasEffect(Buffs.LanceCharge)) || (!Config.DRG_ST_DivesOption_Spineshatter[0]) || (!LanceCharge.LevelChecked())) &&
                                   ((Config.DRG_ST_DivesOption_Spineshatter[1] && HasEffect(Buffs.RightEye)) || (!Config.DRG_ST_DivesOption_Spineshatter[1]) || (!DragonSight.LevelChecked())) &&
                                   ((Config.DRG_ST_DivesOption_Spineshatter[2] && HasEffect(Buffs.BattleLitany)) || (!Config.DRG_ST_DivesOption_Spineshatter[2]) || (!BattleLitany.LevelChecked()))))
                                {
                                    if (ActionReady(SpineshatterDive) && AnimationLock.CanDRGWeave(SpineshatterDive))
                                        return SpineshatterDive;
                                }
                            }


                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                                ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_GeirskogulNastrond) && AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)) && (ActionReady(OriginalHook(Geirskogul)) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_Optimized_Rotation) && IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)))))
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && AnimationLock.CanDRGWeave(MirageDive) && (HasEffect(Buffs.DiveReady) ||
                               (IsEnabled(CustomComboPreset.DRG_ST_Optimized_Rotation) && IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady))))
                                return MirageDive;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) && AnimationLock.CanDRGWeave(Stardiver) &&
                                gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany) || gauge.LOTDTimer <= 4000))
                                return Stardiver;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.DRG_ST_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.DRG_ST_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                    {
                        // If we are not on the flank, but need to use Fangs, pop true north if not already up
                        if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                            trueNorthReady && allowedToTN && CanDelayedWeave(actionID) &&
                            !OnTargetsFlank() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(FangAndClaw);
                    }

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                    {
                        // If we are not on the rear, but need to use Wheeling, pop true north if not already up
                        if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                            trueNorthReady && allowedToTN && CanDelayedWeave(actionID) &&
                            !OnTargetsRear() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(WheelingThrust);
                    }

                    if (comboTime > 0)
                    {
                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && allowedToTN && CanDelayedWeave(actionID) &&
                                !OnTargetsRear() && !HasEffect(Buffs.RightEye))
                                return All.TrueNorth;

                            if (lastComboMove is Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                            return VorpalThrust;

                        if (lastComboMove is VorpalThrust && LevelChecked(FullThrust))
                            return OriginalHook(FullThrust);
                    }

                    return OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_AOE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();

                if (actionID is DoomSpike)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                        return Variant.VariantCure;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Lance Charge Feature
                        if (ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Dragon Sight Feature
                        if (ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight))
                            return DragonSight;

                        //Life Surge Feature
                        if (!HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                lastComboMove is SonicThrust && LevelChecked(CoerthanTorment) && AnimationLock.CanDRGWeave(LifeSurge))
                            return LifeSurge;


                        //Wyrmwind Thrust Feature
                        if (gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                            return WyrmwindThrust;

                        //Dives Feature
                        if (!IsMoving && LevelChecked(LanceCharge))
                        {
                            if ((!TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                               (HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye))) //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                            {
                                if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                    return DragonfireDive;

                                if (ActionReady(SpineshatterDive) && AnimationLock.CanDRGWeave(SpineshatterDive))
                                    return SpineshatterDive;
                            }
                        }

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                            return OriginalHook(Jump);

                        //Geirskogul and Nastrond Feature
                        if (IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)) && AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)))
                            return OriginalHook(Geirskogul);

                        //Mirage Feature
                        if (IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady) && AnimationLock.CanDRGWeave(MirageDive))
                            return MirageDive;

                        //StarDives Feature
                        if (gauge.IsLOTDActive && ActionReady(Stardiver) && AnimationLock.CanDRGWeave(Stardiver) && !IsMoving &&
                            (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                            return Stardiver;
                    }

                    if (comboTime > 0)
                    {
                        if (!SonicThrust.LevelChecked())
                        {
                            if (lastComboMove == TrueThrust)
                                return Disembowel;

                            if (lastComboMove == Disembowel && OriginalHook(ChaosThrust).LevelChecked())
                                return OriginalHook(ChaosThrust);
                        }
                        else
                        {
                            if (lastComboMove is DoomSpike or DraconianFury)
                                return SonicThrust;

                            if (lastComboMove == SonicThrust && CoerthanTorment.LevelChecked())
                                return CoerthanTorment;
                        }
                    }

                    return HasEffect(Buffs.PowerSurge) || SonicThrust.LevelChecked() ? OriginalHook(DoomSpike) : OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_AOE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                bool dragonfireAny = Config.DRG_AoE_DivesOption_Dragonfire.All(x => x == false);
                bool spineshatterAny = Config.DRG_AoE_DivesOption_Spineshatter.All(x => x == false);

                if (actionID is DoomSpike)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                        return Variant.VariantCure;

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) &&
                        LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                        {
                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LitanyHP)
                                return BattleLitany;

                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Lance) &&
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LanceChargeHP)
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) &&
                                ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_SightHP)
                                return DragonSight;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_AoE_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                               !HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                lastComboMove is SonicThrust && LevelChecked(CoerthanTorment) && AnimationLock.CanDRGWeave(LifeSurge))
                                return LifeSurge;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                                gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                                return WyrmwindThrust;

                            if (!IsMoving)
                            {
                                if (IsEnabled(CustomComboPreset.DRG_AoE_Dragonfire_Dive))
                                {
                                    if (dragonfireAny || //Dives on cooldown
                                       (((Config.DRG_AoE_DivesOption_Dragonfire[0] && HasEffect(Buffs.LanceCharge)) || (!Config.DRG_AoE_DivesOption_Dragonfire[0]) || (!LanceCharge.LevelChecked())) &&
                                       ((Config.DRG_AoE_DivesOption_Dragonfire[1] && HasEffect(Buffs.RightEye)) || (!Config.DRG_AoE_DivesOption_Dragonfire[1]) || (!DragonSight.LevelChecked())) &&
                                       ((Config.DRG_AoE_DivesOption_Dragonfire[2] && HasEffect(Buffs.BattleLitany)) || (!Config.DRG_AoE_DivesOption_Dragonfire[2]) || (!BattleLitany.LevelChecked()))))
                                    {
                                        if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                            return DragonfireDive;
                                    }
                                }

                                if (IsEnabled(CustomComboPreset.DRG_AoE_Spineshatter_Dive))
                                {
                                    if (spineshatterAny || //Dives on cooldown
                                       (((Config.DRG_AoE_DivesOption_Spineshatter[0] && HasEffect(Buffs.LanceCharge)) || (!Config.DRG_AoE_DivesOption_Spineshatter[0]) || (!LanceCharge.LevelChecked())) &&
                                       ((Config.DRG_AoE_DivesOption_Spineshatter[1] && HasEffect(Buffs.RightEye)) || (!Config.DRG_AoE_DivesOption_Spineshatter[1]) || (!DragonSight.LevelChecked())) &&
                                       ((Config.DRG_AoE_DivesOption_Spineshatter[2] && HasEffect(Buffs.BattleLitany)) || (!Config.DRG_AoE_DivesOption_Spineshatter[2]) || (!BattleLitany.LevelChecked()))))
                                    {
                                        if (ActionReady(SpineshatterDive) && AnimationLock.CanDRGWeave(SpineshatterDive))
                                            return SpineshatterDive;
                                    }
                                }
                            }

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                                ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)) && (ActionReady(OriginalHook(Geirskogul)) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_Optimized_Rotation) && IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)))))
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && AnimationLock.CanDRGWeave(MirageDive) && (HasEffect(Buffs.DiveReady) ||
                               (IsEnabled(CustomComboPreset.DRG_AoE_Optimized_Rotation) && IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady))))
                                return MirageDive;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) && AnimationLock.CanDRGWeave(Stardiver) &&
                               gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving &&
                               (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                return Stardiver;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.DRG_AoE_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.DRG_AoEBloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (!SonicThrust.LevelChecked())
                        {
                            if (lastComboMove == TrueThrust)
                                return Disembowel;

                            if (lastComboMove == Disembowel && OriginalHook(ChaosThrust).LevelChecked())
                                return OriginalHook(ChaosThrust);
                        }
                        else
                        {
                            if (lastComboMove is DoomSpike or DraconianFury)
                                return SonicThrust;

                            if (lastComboMove == SonicThrust && CoerthanTorment.LevelChecked())
                                return CoerthanTorment;
                        }
                    }

                    return HasEffect(Buffs.PowerSurge) || SonicThrust.LevelChecked() ? OriginalHook(DoomSpike) : OriginalHook(TrueThrust);

                }

                return actionID;
            }
        }

        internal class DRG_JumpFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_Jump;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Jump or HighJump && HasEffect(Buffs.DiveReady) ? MirageDive : actionID;
        }

        internal class DRG_StardiverFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_StardiverFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();

                if (actionID is Stardiver)
                {
                    if (gauge.IsLOTDActive && ActionReady(Stardiver))
                        return Stardiver;

                    if ((LevelChecked(Geirskogul) && !gauge.IsLOTDActive) || gauge.IsLOTDActive)
                        return OriginalHook(Geirskogul);
                }

                return actionID;
            }
        }

        internal class DRG_BurstCDFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_BurstCDFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is LanceCharge)
                {
                    if (IsOnCooldown(LanceCharge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_BurstCDFeature_DragonSight) &&
                            ActionReady(DragonSight))
                            return DragonSight;

                        if (ActionReady(BattleLitany))
                            return BattleLitany;
                    }
                }

                return actionID;
            }
        }
    }
}

