using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

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
                DRG_ST_DiveOptions = new("DRG_ST_DiveOptions"),
                DRG_AOE_DiveOptions = new("DRG_AOE_DiveOptions"),
                DRG_OpenerChoice = new("DRG_OpenerChoice"),
                DRG_VariantCure = new("DRG_VariantCure"),
                DRG_STSecondWindThreshold = new("DRG_STSecondWindThreshold"),
                DRG_STBloodbathThreshold = new("DRG_STBloodbathThreshold"),
                DRG_AoESecondWindThreshold = new("DRG_AoESecondWindThreshold"),
                DRG_AoEBloodbathThreshold = new("DRG_AoEBloodbathThreshold");
        }

        internal class DRG_ST_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_BasicCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                bool openerReady = IsOffCooldown(LanceCharge) && IsOffCooldown(BattleLitany) && IsOffCooldown(DragonSight);
                Status? ChaosDoTDebuff;

                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is FullThrust)
                {
                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                        return OriginalHook(FangAndClaw);

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                        return OriginalHook(WheelingThrust);


                    if (comboTime > 0)
                    {
                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                return Disembowel;

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

        internal class DRG_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;
            internal static DRGOpenerLogic DRGOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;

                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_VariantCure)
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
                        if (!HasEffect(All.Buffs.TrueNorth) &&
                            AnimationLock.CanDRGWeave(All.TrueNorth) && !OnTargetsFlank() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(FangAndClaw);
                    }

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                    {
                        // If we are not on the rear, but need to use Wheeling, pop true north if not already up
                        if (!HasEffect(All.Buffs.TrueNorth) &&
                            AnimationLock.CanDRGWeave(All.TrueNorth) && !OnTargetsRear() && !HasEffect(Buffs.RightEye))
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

        internal class DRG_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_AdvancedMode;
            internal static DRGOpenerLogic DRGOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                int diveOptions = Config.DRG_ST_DiveOptions;
                Status? ChaosDoTDebuff;
                bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);
                bool trueNorthReadyDyn = trueNorthReady;

                if (LevelChecked(ChaoticSpring))
                    ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                // Prevent the dynamic true north option from using the last charge
                if (trueNorthReady && IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                    IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic_HoldCharge) && GetRemainingCharges(All.TrueNorth) < 2)
                    trueNorthReadyDyn = false;


                if (actionID is TrueThrust)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_VariantCure)
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
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany))
                                return BattleLitany;

                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge))
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) &&
                                ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight))
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
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives) &&
                                (IsNotEnabled(CustomComboPreset.DRG_ST_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_ST_Dives_Melee) &&
                                GetTargetDistance() <= 1)) && !IsMoving && LevelChecked(LanceCharge))
                            {
                                if (diveOptions is 0 || //Dives on cooldown
                                   (diveOptions is 1 && !TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                                   (diveOptions is 1 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                   (diveOptions is 2 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                {
                                    if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                        return DragonfireDive;

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
                                gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                return Stardiver;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.DRG_STSecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.DRG_STBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                    {
                        // If we are not on the flank, but need to use Fangs, pop true north if not already up
                        if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                            trueNorthReadyDyn && !HasEffect(All.Buffs.TrueNorth) &&
                            AnimationLock.CanDRGWeave(All.TrueNorth) && !OnTargetsFlank() && !HasEffect(Buffs.RightEye))
                            return All.TrueNorth;

                        return OriginalHook(FangAndClaw);
                    }

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                    {
                        // If we are not on the rear, but need to use Wheeling, pop true north if not already up
                        if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                            trueNorthReadyDyn && !HasEffect(All.Buffs.TrueNorth) &&
                            AnimationLock.CanDRGWeave(All.TrueNorth) && !OnTargetsRear() && !HasEffect(Buffs.RightEye))
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
                        PlayerHealthPercentageHp() <= Config.DRG_VariantCure)
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
                        if (gauge.IsLOTDActive && ActionReady(Stardiver) && AnimationLock.CanDRGWeave(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                            return Stardiver;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is DoomSpike or DraconianFury)
                            return (LevelChecked(SonicThrust) || GetBuffRemainingTime(Buffs.PowerSurge) > 10)
                                ? SonicThrust
                                : TrueThrust;

                        if (lastComboMove is TrueThrust)
                            return Disembowel;

                        if (lastComboMove is Disembowel)
                            return LevelChecked(OriginalHook(ChaosThrust))
                            ? ChaosThrust
                            : DoomSpike;

                        if (lastComboMove is SonicThrust && LevelChecked(CoerthanTorment))
                            return CoerthanTorment;
                    }

                    return OriginalHook(DoomSpike);
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
                int diveOptions = Config.DRG_AOE_DiveOptions;

                if (actionID is DoomSpike)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_VariantCure)
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
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany))
                                return BattleLitany;

                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Lance) &&
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge))
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) &&
                                ActionReady(DragonSight) && AnimationLock.CanDRGWeave(DragonSight))
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

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Dives) &&
                                (IsNotEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) &&
                                GetTargetDistance() <= 1)) && !IsMoving && LevelChecked(LanceCharge))
                            {
                                if (diveOptions is 0 || //Dives on cooldown
                                   (diveOptions is 1 && !TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                                   (diveOptions is 1 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                   (diveOptions is 2 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                {
                                    if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                        return DragonfireDive;

                                    if (ActionReady(SpineshatterDive) && AnimationLock.CanDRGWeave(SpineshatterDive))
                                        return SpineshatterDive;
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
                               gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                return Stardiver;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.DRG_AoESecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.DRG_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is DoomSpike or DraconianFury)
                            return (LevelChecked(SonicThrust) || GetBuffRemainingTime(Buffs.PowerSurge) > 10)
                                ? SonicThrust
                                : TrueThrust;

                        if (lastComboMove is TrueThrust)
                            return Disembowel;

                        if (lastComboMove is Disembowel)
                            return LevelChecked(OriginalHook(ChaosThrust))
                            ? ChaosThrust
                            : DoomSpike;

                        if (lastComboMove is SonicThrust && LevelChecked(CoerthanTorment))
                            return CoerthanTorment;
                    }

                    return OriginalHook(DoomSpike);
                }

                return actionID;
            }
        }

        internal class DRG_JumpFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_Jump;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is DRG.Jump or DRG.HighJump && HasEffect(DRG.Buffs.DiveReady) ? DRG.MirageDive : actionID;
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
