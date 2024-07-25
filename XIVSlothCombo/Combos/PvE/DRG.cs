using System;
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
            Geirskogul = 3555,
            Nastrond = 7400,
            HeavensThrust = 25771,
            Drakesbane = 36952,
            RiseOfTheDragon = 36953,
            LanceBarrage = 36954,
            SpiralBlow = 36955,
            Starcross = 36956;

        public static class Buffs
        {
            public const ushort
                LanceCharge = 1864,
                BattleLitany = 786,
                DiveReady = 1243,
                RaidenThrustReady = 1863,
                PowerSurge = 2720,
                LifeSurge = 116,
                DraconianFire = 1863,
                NastrondReady = 3844,
                StarcrossReady = 3846,
                DragonsFlight = 3845;
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
                EnhancedLifeSurge = 438;
        }

        public static class Config
        {
            public static UserInt
                DRG_Variant_Cure = new("DRG_VariantCure"),
                DRG_ST_LitanyHP = new("DRG_ST_LitanyHP", 2),
                DRG_ST_SightHP = new("DRG_ST_SightHP", 2),
                DRG_ST_LanceChargeHP = new("DRG_ST_LanceChargeHP", 2),
                DRG_ST_SecondWind_Threshold = new("DRG_STSecondWindThreshold", 25),
                DRG_ST_Bloodbath_Threshold = new("DRG_STBloodbathThreshold", 40),
                DRG_AoE_LitanyHP = new("DRG_AoE_LitanyHP", 5),
                DRG_AoE_SightHP = new("DRG_AoE_SightHP", 5),
                DRG_AoE_LanceChargeHP = new("DRG_AoE_LanceChargeHP", 5),
                DRG_AoE_SecondWind_Threshold = new("DRG_AoE_SecondWindThreshold", 25),
                DRG_AoE_Bloodbath_Threshold = new("DRG_AoE_BloodbathThreshold", 40);
        }

        internal class DRG_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;
            internal static DRGOpenerLogic DRGOpener = new();
            float GCD = GetCooldown(TrueThrust).CooldownTotal;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (LevelChecked(ChaoticSpring))
                    ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Opener for DRG
                    if (DRGOpener.DoFullOpener(ref actionID))
                        return actionID;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        //Lance Charge Feature
                        if (ActionReady(LanceCharge) &&
                            AnimationLock.CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) &&
                            AnimationLock.CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Life Surge Feature
                        if (((GetCooldownRemainingTime(LifeSurge) < GCD * 16) || (GetCooldownRemainingTime(BattleLitany) > GCD * 20)) &&
                            AnimationLock.CanDRGWeave(LifeSurge) &&
                            (HasEffect(Buffs.LanceCharge) &&
                            ActionReady(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                            ((WasLastWeaponskill(WheelingThrust) && LevelChecked(Drakesbane)) ||
                            (WasLastWeaponskill(FangAndClaw) && LevelChecked(Drakesbane)) ||
                            (WasLastWeaponskill(OriginalHook(VorpalThrust)) && LevelChecked(OriginalHook(FullThrust))))))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                            gauge.FirstmindsFocusCount is 2)
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (AnimationLock.CanDRGWeave(Geirskogul) && ActionReady(Geirskogul))
                            return Geirskogul;

                        //(High) Jump Feature   
                        if (AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                            ActionReady(OriginalHook(Jump)) && !IsMoving)
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (AnimationLock.CanDRGWeave(DragonfireDive) &&
                            ActionReady(DragonfireDive) && !IsMoving)
                            return DragonfireDive;

                        //StarDiver Feature
                        if (AnimationLock.CanDRGWeave(Stardiver) &&
                            gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving)
                            return Stardiver;

                        //Starcross Feature
                        if (AnimationLock.CanDRGWeave(Starcross) &&
                            HasEffect(Buffs.StarcrossReady))
                            return OriginalHook(Stardiver);

                        //Rise of the Dragon Feature
                        if (AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                             HasEffect(Buffs.DragonsFlight))
                            return OriginalHook(DragonfireDive);

                        //Mirage Feature
                        if (AnimationLock.CanDRGWeave(MirageDive) &&
                            HasEffect(Buffs.DiveReady))
                            return OriginalHook(HighJump);

                        //Nastrond Feature
                        if (AnimationLock.CanDRGWeave(Nastrond) &&
                            HasEffect(Buffs.NastrondReady) &&
                            gauge.IsLOTDActive)
                            return OriginalHook(Geirskogul);
                    }

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is TrueThrust or RaidenThrust)
                        {
                            return (LevelChecked(OriginalHook(Disembowel)) &&
                                (ChaosDoTDebuff is null ||
                                ChaosDoTDebuff.RemainingTime < GCD * 5 ||
                                GetBuffRemainingTime(Buffs.PowerSurge) < GCD * 7 ||
                                GetCooldownRemainingTime(LanceCharge) < GCD * 4)) ||
                                (!LevelChecked(ChaosThrust) &&
                                GetBuffRemainingTime(Buffs.PowerSurge) < GCD * 4)
                                ? OriginalHook(Disembowel)
                                : OriginalHook(VorpalThrust);
                        }

                        if (lastComboMove is Disembowel or SpiralBlow && LevelChecked(OriginalHook(ChaosThrust)))
                        {
                            if (trueNorthReady && CanDelayedWeave(actionID) &&
                                !OnTargetsRear())
                                return All.TrueNorth;

                            return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove is ChaosThrust or ChaoticSpring && LevelChecked(WheelingThrust))
                        {
                            if (trueNorthReady && CanDelayedWeave(actionID) &&
                              !OnTargetsRear())
                                return All.TrueNorth;

                            return WheelingThrust;
                        }

                        if (lastComboMove is VorpalThrust or LanceBarrage && LevelChecked(OriginalHook(FullThrust)))
                            return OriginalHook(FullThrust);


                        if (lastComboMove is FullThrust or HeavensThrust && LevelChecked(FangAndClaw))
                        {
                            if (trueNorthReady && CanDelayedWeave(actionID) &&
                                !OnTargetsFlank())
                                return All.TrueNorth;

                            return FangAndClaw;
                        }

                        if (lastComboMove is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                            return Drakesbane;
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
            float GCD = GetCooldown(TrueThrust).CooldownTotal;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (LevelChecked(ChaoticSpring))
                    ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Opener for DRG
                    if (IsEnabled(CustomComboPreset.DRG_ST_Opener))
                    {
                        if (DRGOpener.DoFullOpener(ref actionID))
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
                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                                AnimationLock.CanDRGWeave(LanceCharge) &&
                                ActionReady(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LanceChargeHP)
                                return LanceCharge;

                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                                AnimationLock.CanDRGWeave(BattleLitany) &&
                                ActionReady(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LitanyHP)
                                return BattleLitany;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) &&
                                ((GetCooldownRemainingTime(LifeSurge) < GCD * 16) || (GetCooldownRemainingTime(BattleLitany) > GCD * 20)) &&
                                AnimationLock.CanDRGWeave(LifeSurge) &&
                                (HasEffect(Buffs.LanceCharge) &&
                                ActionReady(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                                ((WasLastWeaponskill(WheelingThrust) && LevelChecked(Drakesbane)) ||
                                (WasLastWeaponskill(FangAndClaw) && LevelChecked(Drakesbane)) ||
                                (WasLastWeaponskill(OriginalHook(VorpalThrust)) && LevelChecked(OriginalHook(FullThrust))))))
                                return LifeSurge;

                            //Dragonfire Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive) &&
                                AnimationLock.CanDRGWeave(DragonfireDive) &&
                                ActionReady(DragonfireDive) &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Movement) && !IsMoving)))
                                return DragonfireDive;

                            //StarDiver Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) &&
                                AnimationLock.CanDRGWeave(Stardiver) &&
                                ActionReady(Stardiver) && gauge.IsLOTDActive &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Movement) && !IsMoving)))
                                return Stardiver;

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                                AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                                ActionReady(OriginalHook(Jump)) &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_HighJump_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_HighJump_Movement) && !IsMoving)))
                                return OriginalHook(Jump);

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                                AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                                gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Geirskogul Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                                AnimationLock.CanDRGWeave(Geirskogul) &&
                                ActionReady(Geirskogul))
                                return Geirskogul;

                            //Starcross Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Starcross) &&
                                AnimationLock.CanDRGWeave(Starcross) &&
                                HasEffect(Buffs.StarcrossReady))
                                return OriginalHook(Stardiver);

                            //Rise of the Dragon Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_RiseOfTheDragon) &&
                                AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight))
                                return OriginalHook(DragonfireDive);

                            //Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Nastrond) &&
                                AnimationLock.CanDRGWeave(Nastrond) &&
                                HasEffect(Buffs.NastrondReady) &&
                                gauge.IsLOTDActive)
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) &&
                                AnimationLock.CanDRGWeave(MirageDive) &&
                                HasEffect(Buffs.DiveReady))
                                return OriginalHook(HighJump);

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
                    if (comboTime > 0)
                    {
                        if (lastComboMove is TrueThrust or RaidenThrust)
                        {
                            return (LevelChecked(OriginalHook(Disembowel)) &&
                                (ChaosDoTDebuff is null ||
                                ChaosDoTDebuff.RemainingTime < GCD * 5 ||
                                GetBuffRemainingTime(Buffs.PowerSurge) < GCD * 7 ||
                                GetCooldownRemainingTime(LanceCharge) < GCD * 4)) ||
                                (!LevelChecked(ChaosThrust) &&
                                GetBuffRemainingTime(Buffs.PowerSurge) < GCD * 4)
                                ? OriginalHook(Disembowel)
                                : OriginalHook(VorpalThrust);
                        }

                        if (lastComboMove is Disembowel or SpiralBlow && LevelChecked(OriginalHook(ChaosThrust)))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && CanDelayedWeave(actionID) &&
                                !OnTargetsRear())
                                return All.TrueNorth;

                            return OriginalHook(ChaosThrust);
                        }
                        if (lastComboMove is ChaosThrust or ChaoticSpring && LevelChecked(WheelingThrust))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                              trueNorthReady && CanDelayedWeave(actionID) &&
                              !OnTargetsRear())
                                return All.TrueNorth;

                            return WheelingThrust;
                        }

                        if (lastComboMove is VorpalThrust or LanceBarrage && LevelChecked(OriginalHook(FullThrust)))
                            return OriginalHook(FullThrust);


                        if (lastComboMove is FullThrust or HeavensThrust && LevelChecked(FangAndClaw))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && CanDelayedWeave(actionID) &&
                                !OnTargetsFlank())
                                return All.TrueNorth;

                            return FangAndClaw;
                        }

                        if (lastComboMove is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                            return Drakesbane;
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
                    if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        //Lance Charge Feature
                        if (ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Life Surge Feature
                        if (ActionReady(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                            ((WasLastWeaponskill(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                            (WasLastWeaponskill(DoomSpike) && LevelChecked(SonicThrust)) ||
                            (WasLastWeaponskill(DoomSpike) && !LevelChecked(SonicThrust))))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                            gauge.FirstmindsFocusCount is 2)
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (AnimationLock.CanDRGWeave(Geirskogul) && ActionReady(Geirskogul))
                            return Geirskogul;

                        //(High) Jump Feature   
                        if (AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                            ActionReady(OriginalHook(Jump)) && !IsMoving)
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (AnimationLock.CanDRGWeave(DragonfireDive) &&
                            ActionReady(DragonfireDive) && !IsMoving)
                            return DragonfireDive;

                        //StarDiver Feature
                        if (AnimationLock.CanDRGWeave(Stardiver) &&
                            gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving)
                            return Stardiver;

                        //Starcross Feature
                        if (AnimationLock.CanDRGWeave(Starcross) &&
                            HasEffect(Buffs.StarcrossReady))
                            return OriginalHook(Stardiver);

                        //Rise of the Dragon Feature
                        if (AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                             HasEffect(Buffs.DragonsFlight))
                            return OriginalHook(DragonfireDive);

                        //Mirage Feature
                        if (AnimationLock.CanDRGWeave(MirageDive) &&
                            HasEffect(Buffs.DiveReady))
                            return OriginalHook(HighJump);

                        //Nastrond Feature
                        if (AnimationLock.CanDRGWeave(Nastrond) &&
                            HasEffect(Buffs.NastrondReady) &&
                            gauge.IsLOTDActive)
                            return OriginalHook(Geirskogul);
                    }

                    if (comboTime > 0)
                    {
                        if (!SonicThrust.LevelChecked())
                        {
                            if (lastComboMove == TrueThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove == Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);
                        }

                        else
                        {
                            if (lastComboMove is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                                return SonicThrust;

                            if (lastComboMove == SonicThrust && LevelChecked(CoerthanTorment))
                                return CoerthanTorment;
                        }
                    }

                    return HasEffect(Buffs.PowerSurge) || LevelChecked(SonicThrust)
                        ? OriginalHook(DoomSpike)
                        : OriginalHook(TrueThrust);
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

                if (actionID is DoomSpike)
                {
                    if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        AnimationLock.CanDRGWeave(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) &&
                        LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                        {
                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Lance) &&
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LanceChargeHP)
                                return LanceCharge;

                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LitanyHP)
                                return BattleLitany;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_AoE_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                                ActionReady(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                                ((WasLastWeaponskill(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                                (WasLastWeaponskill(DoomSpike) && LevelChecked(SonicThrust)) ||
                                (WasLastWeaponskill(DoomSpike) && !LevelChecked(SonicThrust))))
                                return LifeSurge;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                                AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                                gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Geirskogul Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Geirskogul) &&
                                AnimationLock.CanDRGWeave(Geirskogul) &&
                                ActionReady(Geirskogul))
                                return Geirskogul;

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                                AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                                ActionReady(OriginalHook(Jump)) &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Movement) && !IsMoving)))
                                return OriginalHook(Jump);

                            //Dragonfire Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive) &&
                                AnimationLock.CanDRGWeave(DragonfireDive) &&
                                ActionReady(DragonfireDive) &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Movement) && !IsMoving)))
                                return DragonfireDive;

                            //StarDiver Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) &&
                                AnimationLock.CanDRGWeave(Stardiver) &&
                                ActionReady(Stardiver) && gauge.IsLOTDActive &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Movement) && !IsMoving)))
                                return Stardiver;

                            //Starcross Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Starcross) &&
                                AnimationLock.CanDRGWeave(Starcross) &&
                                HasEffect(Buffs.StarcrossReady))
                                return OriginalHook(Stardiver);

                            //Rise of the Dragon Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_RiseOfTheDragon) &&
                                AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight))
                                return OriginalHook(DragonfireDive);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) &&
                                AnimationLock.CanDRGWeave(MirageDive) &&
                                HasEffect(Buffs.DiveReady))
                                return OriginalHook(HighJump);

                            //Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Nastrond) &&
                                AnimationLock.CanDRGWeave(Nastrond) &&
                                HasEffect(Buffs.NastrondReady) &&
                                gauge.IsLOTDActive)
                                return OriginalHook(Geirskogul);
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.DRG_AoE_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.DRG_AoE_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (!SonicThrust.LevelChecked())
                        {
                            if (lastComboMove == TrueThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove == Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);
                        }

                        else
                        {
                            if (lastComboMove is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                                return SonicThrust;

                            if (lastComboMove == SonicThrust && LevelChecked(CoerthanTorment))
                                return CoerthanTorment;
                        }
                    }
                    return HasEffect(Buffs.PowerSurge) || LevelChecked(SonicThrust)
                        ? OriginalHook(DoomSpike)
                        : OriginalHook(TrueThrust);
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
                    if (IsOnCooldown(LanceCharge) && ActionReady(BattleLitany))
                        return BattleLitany;
                }
                return actionID;
            }
        }
    }
}

