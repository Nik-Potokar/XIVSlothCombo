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
        }

        internal class DRG_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;
            internal static DRGOpenerLogic DRGOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                Status? ChaosDoTDebuff;

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

                    // Opener for DRG
                    if (DRGOpener.DoFullOpener(ref actionID))
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

                        //Life Surge Feature
                        if (ActionReady(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                            (WasLastWeaponskill(WheelingThrust) || WasLastWeaponskill(FangAndClaw) || WasLastWeaponskill(OriginalHook(VorpalThrust))))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                            return WyrmwindThrust;

                        //Dives Feature
                        if (ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive) && !IsMoving && HasEffect(Buffs.LanceCharge))
                            return DragonfireDive;

                        //Dives Feature
                        if (HasEffect(Buffs.DragonsFlight) && AnimationLock.CanDRGWeave(RiseOfTheDragon) && !IsMoving)
                            return OriginalHook(DragonfireDive);

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                            return OriginalHook(Jump);

                        //Geirskogul and Nastrond Feature
                        if (AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)) && ActionReady(OriginalHook(Geirskogul)))
                            return OriginalHook(Geirskogul);

                        //Mirage Feature
                        if (AnimationLock.CanDRGWeave(MirageDive) && HasEffect(Buffs.DiveReady))
                            return MirageDive;

                        //StarDives Feature
                        if (AnimationLock.CanDRGWeave(Stardiver) &&
                            gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving)
                            return Stardiver;

                        if (AnimationLock.CanDRGWeave(Starcross) &&
                           HasEffect(Buffs.StarcrossReady) && !IsMoving)
                            return OriginalHook(Stardiver);
                    }

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (WasLastWeaponskill(FangAndClaw) || WasLastWeaponskill(WheelingThrust))
                            return Drakesbane;

                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove == OriginalHook(TrueThrust) && LevelChecked(OriginalHook(Disembowel)))
                                return OriginalHook(Disembowel);

                            if (CanDelayedWeave(actionID) && !OnTargetsRear())
                                return All.TrueNorth;

                            if (lastComboMove == OriginalHook(Disembowel) && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);

                            if (lastComboMove == OriginalHook(ChaosThrust) && LevelChecked(OriginalHook(WheelingThrust)))
                            {
                                // If we are not on the rear, but need to use Wheeling, pop true north if not already up
                                if (CanDelayedWeave(actionID) && !OnTargetsRear())
                                    return All.TrueNorth;

                                return OriginalHook(WheelingThrust);
                            }
                        }

                        if (lastComboMove == OriginalHook(TrueThrust) && LevelChecked(OriginalHook(VorpalThrust)))
                            return OriginalHook(VorpalThrust);

                        if (lastComboMove == OriginalHook(VorpalThrust) && LevelChecked(OriginalHook(FullThrust)))
                            return OriginalHook(FullThrust);

                        if (lastComboMove == OriginalHook(FullThrust) && LevelChecked(OriginalHook(FangAndClaw)))
                        {
                            // If we are not on the flank, but need to use Fangs, pop true north if not already up
                            if (CanDelayedWeave(actionID) && !OnTargetsFlank())
                                return All.TrueNorth;

                            return OriginalHook(FangAndClaw);
                        }
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
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

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
                                ActionReady(LanceCharge) && AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LanceChargeHP)
                                return LanceCharge;

                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                                ActionReady(BattleLitany) && AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LitanyHP)
                                return BattleLitany;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && ActionReady(LifeSurge) && AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                            (WasLastWeaponskill(WheelingThrust) || WasLastWeaponskill(FangAndClaw) || WasLastWeaponskill(OriginalHook(VorpalThrust))))
                                return LifeSurge;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                                gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                                return WyrmwindThrust;

                            //Geirskogul Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                                AnimationLock.CanDRGWeave(Geirskogul) && ActionReady(Geirskogul))
                                return Geirskogul;

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                                ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_Dragonfire) &&
                                !IsMoving && ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive) && (GetBuffStacks(Buffs.NastrondReady) == 3 || !LevelChecked(Nastrond)))
                                return DragonfireDive;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) && AnimationLock.CanDRGWeave(Stardiver) &&
                                gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && GetBuffStacks(Buffs.NastrondReady) == 2)
                                return Stardiver;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Starcross) && AnimationLock.CanDRGWeave(Starcross) &&
                                HasEffect(Buffs.StarcrossReady) && !IsMoving && WasLastAbility(Stardiver))
                                return OriginalHook(Stardiver);

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight) && AnimationLock.CanDRGWeave(RiseOfTheDragon) && (GetBuffStacks(Buffs.NastrondReady) == 1 || !LevelChecked(Nastrond)))
                                return OriginalHook(DragonfireDive);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && AnimationLock.CanDRGWeave(MirageDive) && HasEffect(Buffs.DiveReady) &&
                               ((GetBuffStacks(Buffs.NastrondReady) == 0 && JustUsed(Nastrond, 2f)) || !LevelChecked(OriginalHook(Geirskogul))))
                                return MirageDive;

                            //Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Nastrond) &&
                                AnimationLock.CanDRGWeave(Nastrond) && HasEffect(Buffs.NastrondReady))
                                return OriginalHook(Geirskogul);
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
                            return (LevelChecked(OriginalHook(Disembowel)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 10 || GetBuffRemainingTime(Buffs.PowerSurge) < 15))
                                ? OriginalHook(Disembowel)
                                : OriginalHook(VorpalThrust);
                        }

                        if (lastComboMove is Disembowel or SpiralBlow)
                            return OriginalHook(ChaosThrust);

                        if (lastComboMove is ChaosThrust or ChaoticSpring)
                            return WheelingThrust;

                        if (lastComboMove is VorpalThrust or LanceBarrage)
                            return OriginalHook(FullThrust);

                        if (lastComboMove is FullThrust or HeavensThrust)
                            return FangAndClaw;

                        if (lastComboMove is WheelingThrust or FangAndClaw)
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

                        //Life Surge Feature
                        if (!HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                lastComboMove is SonicThrust && LevelChecked(CoerthanTorment) && AnimationLock.CanDRGWeave(LifeSurge))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (gauge.FirstmindsFocusCount is 2 && AnimationLock.CanDRGWeave(WyrmwindThrust))
                            return WyrmwindThrust;

                        //Dives Feature
                        if (!IsMoving && HasEffect(Buffs.LanceCharge) && ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                            return DragonfireDive;

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
                            (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.BattleLitany)))
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

                    return HasEffect(Buffs.PowerSurge) || SonicThrust.LevelChecked()
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
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Dragonfire_Dive) &&
                                !IsMoving && ActionReady(DragonfireDive) && AnimationLock.CanDRGWeave(DragonfireDive))
                                return DragonfireDive;

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight) && AnimationLock.CanDRGWeave(RiseOfTheDragon))
                                return OriginalHook(DragonfireDive);

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                                ActionReady(OriginalHook(Jump)) && !IsMoving && AnimationLock.CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && AnimationLock.CanDRGWeave(OriginalHook(Geirskogul)) && ActionReady(OriginalHook(Geirskogul)))
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && AnimationLock.CanDRGWeave(MirageDive) && HasEffect(Buffs.DiveReady))
                                return MirageDive;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) && AnimationLock.CanDRGWeave(Stardiver) &&
                               gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving &&
                               (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.BattleLitany)))
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

                    return HasEffect(Buffs.PowerSurge) || SonicThrust.LevelChecked()
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

