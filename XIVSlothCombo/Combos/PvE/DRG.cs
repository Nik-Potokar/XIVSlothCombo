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
                    if (DRGOpener.DoFullOpener(ref actionID))
                        return actionID;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) &&
                        !InMeleeRange() &&
                        HasBattleTarget())
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
                        if (ActionReady(LifeSurge) &&
                            ((GetCooldownRemainingTime(LifeSurge) < 40) || (GetCooldownRemainingTime(BattleLitany) > 50)) &&
                            AnimationLock.CanDRGWeave(LifeSurge) &&
                            HasEffect(Buffs.LanceCharge) &&
                            !HasEffect(Buffs.LifeSurge) &&
                            ((JustUsed(WheelingThrust) && LevelChecked(Drakesbane)) ||
                            (JustUsed(FangAndClaw) && LevelChecked(Drakesbane)) ||
                            (JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(FullThrust))))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (LevelChecked(WyrmwindThrust) &&
                            AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                            gauge.FirstmindsFocusCount is 2)
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (ActionReady(Geirskogul) &&
                            AnimationLock.CanDRGWeave(Geirskogul))
                            return Geirskogul;

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) &&
                            AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                            !IsMoving)
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (ActionReady(DragonfireDive) &&
                            AnimationLock.CanDRGWeave(DragonfireDive) &&
                            !IsMoving)
                            return DragonfireDive;

                        //StarDiver Feature
                        if (ActionReady(Stardiver) &&
                            AnimationLock.CanDRGWeave(Stardiver) &&
                            gauge.IsLOTDActive && !IsMoving)
                            return Stardiver;

                        //Starcross Feature
                        if (LevelChecked(Starcross) &&
                            AnimationLock.CanDRGWeave(Starcross) &&
                            HasEffect(Buffs.StarcrossReady))
                            return Starcross;

                        //Rise of the Dragon Feature
                        if (LevelChecked(RiseOfTheDragon) &&
                            AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                            HasEffect(Buffs.DragonsFlight))
                            return RiseOfTheDragon;

                        //Nastrond Feature
                        if (LevelChecked(Nastrond) &&
                            AnimationLock.CanDRGWeave(Nastrond) &&
                            HasEffect(Buffs.NastrondReady) &&
                            gauge.IsLOTDActive)
                            return Nastrond;

                        //Mirage Feature
                        if (LevelChecked(MirageDive) &&
                            AnimationLock.CanDRGWeave(MirageDive) &&
                            HasEffect(Buffs.DiveReady))
                            return MirageDive;
                    }

                    //1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                        {
                            return (LevelChecked(Disembowel) &&
                                 (((ChaosDoTDebuff is null) && LevelChecked(ChaosThrust)) ||
                                 GetBuffRemainingTime(Buffs.PowerSurge) < 15))
                                 ? OriginalHook(Disembowel)
                                 : OriginalHook(VorpalThrust);
                        }

                        if (lastComboMove == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                        {
                            if (trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
                                !OnTargetsRear())
                                return All.TrueNorth;

                            return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                        {
                            if (trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
                              !OnTargetsRear())
                                return All.TrueNorth;

                            return WheelingThrust;
                        }

                        if (lastComboMove == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                            return OriginalHook(FullThrust);

                        if (lastComboMove == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                        {
                            if (trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
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
                                ActionReady(LanceCharge) &&
                                AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LanceChargeHP)
                                return LanceCharge;

                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                                ActionReady(BattleLitany) &&
                                AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_ST_LitanyHP)
                                return BattleLitany;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) &&
                                ActionReady(LifeSurge) &&
                                ((GetCooldownRemainingTime(LifeSurge) < 40) || (GetCooldownRemainingTime(BattleLitany) > 50)) &&
                                AnimationLock.CanDRGWeave(LifeSurge) &&
                                HasEffect(Buffs.LanceCharge) &&
                                !HasEffect(Buffs.LifeSurge) &&
                                ((JustUsed(WheelingThrust) && LevelChecked(Drakesbane)) ||
                                (JustUsed(FangAndClaw) && LevelChecked(Drakesbane)) ||
                                (JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(FullThrust))))
                                return LifeSurge;

                            //Dragonfire Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive) &&
                                ActionReady(DragonfireDive) &&
                                AnimationLock.CanDRGWeave(DragonfireDive) &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Movement) && !IsMoving)))
                                return DragonfireDive;

                            //StarDiver Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) &&
                                ActionReady(Stardiver) &&
                                AnimationLock.CanDRGWeave(Stardiver) &&
                                gauge.IsLOTDActive &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Movement) && !IsMoving)))
                                return Stardiver;

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                                ActionReady(OriginalHook(Jump)) &&
                                AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                                (!IsEnabled(CustomComboPreset.DRG_ST_HighJump_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_ST_HighJump_Movement) && !IsMoving)))
                                return OriginalHook(Jump);

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                                LevelChecked(WyrmwindThrust) &&
                                AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                                gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Geirskogul Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                                ActionReady(Geirskogul) &&
                                AnimationLock.CanDRGWeave(Geirskogul))
                                return Geirskogul;

                            //Starcross Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Starcross) &&
                                LevelChecked(Starcross) &&
                                AnimationLock.CanDRGWeave(Starcross) &&
                                HasEffect(Buffs.StarcrossReady))
                                return Starcross;

                            //Rise of the Dragon Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Dives_RiseOfTheDragon) &&
                                AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight))
                                return RiseOfTheDragon;

                            //Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Nastrond) &&
                                LevelChecked(Nastrond) &&
                                AnimationLock.CanDRGWeave(Nastrond) &&
                                HasEffect(Buffs.NastrondReady) &&
                                gauge.IsLOTDActive)
                                return Nastrond;

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) &&
                                LevelChecked(MirageDive) &&
                                AnimationLock.CanDRGWeave(MirageDive) &&
                                HasEffect(Buffs.DiveReady))
                                return MirageDive;
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
                        if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                        {
                            return (LevelChecked(Disembowel) &&
                                 (((ChaosDoTDebuff is null) && LevelChecked(ChaosThrust)) ||
                                 GetBuffRemainingTime(Buffs.PowerSurge) < 15))
                                 ? OriginalHook(Disembowel)
                                 : OriginalHook(VorpalThrust);
                        }

                        if (lastComboMove == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
                                !OnTargetsRear())
                                return All.TrueNorth;

                            return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
                                !OnTargetsRear())
                                return All.TrueNorth;

                            return WheelingThrust;
                        }

                        if (lastComboMove == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                            return OriginalHook(FullThrust);

                        if (lastComboMove == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                                trueNorthReady && AnimationLock.CanDRGWeave(All.TrueNorth) &&
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
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
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
                        if (ActionReady(LanceCharge) &&
                            AnimationLock.CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) &&
                            AnimationLock.CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Life Surge Feature
                        if (ActionReady(LifeSurge) &&
                            AnimationLock.CanDRGWeave(LifeSurge) &&
                            !HasEffect(Buffs.LifeSurge) &&
                            ((JustUsed(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                            (JustUsed(DoomSpike) && LevelChecked(SonicThrust)) ||
                            (JustUsed(DoomSpike) && !LevelChecked(SonicThrust))))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (LevelChecked(WyrmwindThrust) &&
                            AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                            gauge.FirstmindsFocusCount is 2)
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (ActionReady(Geirskogul) &&
                            AnimationLock.CanDRGWeave(Geirskogul))
                            return Geirskogul;

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) &&
                            AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                            !IsMoving)
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (ActionReady(DragonfireDive) &&
                            AnimationLock.CanDRGWeave(DragonfireDive) &&
                            !IsMoving)
                            return DragonfireDive;

                        //StarDiver Feature
                        if (ActionReady(Stardiver) &&
                            AnimationLock.CanDRGWeave(Stardiver) &&
                            gauge.IsLOTDActive && !IsMoving)
                            return Stardiver;

                        //Starcross Feature
                        if (LevelChecked(Starcross) &&
                            AnimationLock.CanDRGWeave(Starcross) &&
                            HasEffect(Buffs.StarcrossReady))
                            return OriginalHook(Stardiver);

                        //Rise of the Dragon Feature
                        if (LevelChecked(RiseOfTheDragon) &&
                            AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                             HasEffect(Buffs.DragonsFlight))
                            return OriginalHook(DragonfireDive);

                        //Mirage Feature
                        if (LevelChecked(MirageDive) &&
                            AnimationLock.CanDRGWeave(MirageDive) &&
                            HasEffect(Buffs.DiveReady))
                            return OriginalHook(HighJump);

                        //Nastrond Feature
                        if (LevelChecked(Nastrond) &&
                            AnimationLock.CanDRGWeave(Nastrond) &&
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

                            if (lastComboMove == Disembowel && LevelChecked(ChaosThrust))
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
                    return !HasEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                        ? OriginalHook(TrueThrust)
                        : OriginalHook(DoomSpike);
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
                                ActionReady(LanceCharge) &&
                                AnimationLock.CanDRGWeave(LanceCharge) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LanceChargeHP)
                                return LanceCharge;

                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                                ActionReady(BattleLitany) &&
                                AnimationLock.CanDRGWeave(BattleLitany) &&
                                GetTargetHPPercent() >= Config.DRG_AoE_LitanyHP)
                                return BattleLitany;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_AoE_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                                ActionReady(LifeSurge) &&
                                AnimationLock.CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                                ((JustUsed(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                                (JustUsed(DoomSpike) && LevelChecked(SonicThrust)) ||
                                (JustUsed(DoomSpike) && !LevelChecked(SonicThrust))))
                                return LifeSurge;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                                LevelChecked(WyrmwindThrust) &&
                                AnimationLock.CanDRGWeave(WyrmwindThrust) &&
                                gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Geirskogul Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Geirskogul) &&
                                ActionReady(Geirskogul) &&
                                AnimationLock.CanDRGWeave(Geirskogul))
                                return Geirskogul;

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                                ActionReady(OriginalHook(Jump)) &&
                                AnimationLock.CanDRGWeave(OriginalHook(Jump)) &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Movement) && !IsMoving)))
                                return OriginalHook(Jump);

                            //Dragonfire Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive) &&
                                ActionReady(DragonfireDive) &&
                                AnimationLock.CanDRGWeave(DragonfireDive) &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Movement) && !IsMoving)))
                                return DragonfireDive;

                            //StarDiver Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) &&
                                ActionReady(Stardiver) &&
                                AnimationLock.CanDRGWeave(Stardiver) &&
                                gauge.IsLOTDActive &&
                                (!IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Movement) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Movement) && !IsMoving)))
                                return Stardiver;

                            //Starcross Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Starcross) &&
                                LevelChecked(Starcross) &&
                                AnimationLock.CanDRGWeave(Starcross) &&
                                HasEffect(Buffs.StarcrossReady))
                                return OriginalHook(Stardiver);

                            //Rise of the Dragon Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_RiseOfTheDragon) &&
                                LevelChecked(RiseOfTheDragon) &&
                                AnimationLock.CanDRGWeave(RiseOfTheDragon) &&
                                HasEffect(Buffs.DragonsFlight))
                                return OriginalHook(DragonfireDive);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) &&
                                LevelChecked(MirageDive) &&
                                AnimationLock.CanDRGWeave(MirageDive) &&
                                HasEffect(Buffs.DiveReady))
                                return OriginalHook(HighJump);

                            //Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Nastrond) &&
                                LevelChecked(Nastrond) &&
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
                        if (IsEnabled(CustomComboPreset.DRG_AoE_Disembowel) &&
                            !SonicThrust.LevelChecked())
                        {
                            if (lastComboMove == TrueThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove == Disembowel && LevelChecked(ChaosThrust))
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
                    return IsEnabled(CustomComboPreset.DRG_AoE_Disembowel) &&
                        !HasEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                        ? OriginalHook(TrueThrust)
                        : OriginalHook(DoomSpike);
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

