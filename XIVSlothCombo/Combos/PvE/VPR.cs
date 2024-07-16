using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvE
{
    internal class VPR
    {
        public const byte JobID = 41;

        public const uint
            DreadFangs = 34607,
            DreadMaw = 34615,
            Dreadwinder = 34620,
            HuntersCoil = 34621,
            HuntersDen = 34624,
            HuntersSnap = 39166,
            PitofDread = 34623,
            RattlingCoil = 39189,
            Reawaken = 34626,
            SerpentsIre = 34647,
            SerpentsTail = 35920,
            Slither = 34646,
            SteelFangs = 34606,
            SteelMaw = 34614,
            SwiftskinsCoil = 34622,
            SwiftskinsDen = 34625,
            Twinblood = 35922,
            Twinfang = 35921,
            UncoiledFury = 34633,
            WrithingSnap = 34632,
            SwiftskinsSting = 34609,
            TwinfangBite = 34636,
            TwinbloodBite = 34637,
            UncoiledTwinfang = 34644,
            UncoiledTwinblood = 34645,
            HindstingStrike = 34612,
            DeathRattle = 34634,
            HuntersSting = 34608,
            HindsbaneFang = 34613,
            FlankstingStrike = 34610,
            FlanksbaneFang = 34611,
            HuntersBite = 34616,
            JaggedMaw = 34618,
            SwiftskinsBite = 34617,
            BloodiedMaw = 34619,
            FirstGeneration = 34627,
            FirstLegacy = 34640,
            SecondGeneration = 34628,
            SecondLegacy = 34641,
            ThirdGeneration = 34629,
            ThirdLegacy = 34642,
            FourthGeneration = 34630,
            FourthLegacy = 34643,
            Ouroboros = 34631,
            LastLash = 34635;

        public static class Buffs
        {
            public const ushort
                FellhuntersVenom = 3659,
                FellskinsVenom = 3660,
                FlanksbaneVenom = 3646,
                FlankstungVenom = 3645,
                HindstungVenom = 3647,
                HindsbaneVenom = 3648,
                GrimhuntersVenom = 3649,
                GrimskinsVenom = 3650,
                HuntersVenom = 3657,
                SwiftskinsVenom = 3658,
                HuntersInstinct = 3668,
                Swiftscaled = 3669,
                Reawakened = 3670,
                ReadyToReawaken = 3671,
                PoisedForTwinfang = 3665,
                PoisedForTwinblood = 3666;
        }

        public static class Debuffs
        {
            public const ushort
                NoxiousGnash = 3667;
        }

        public static class Traits
        {
            public const uint
                EnhancedVipersRattle = 530,
                EnhancedSerpentsLineage = 533,
                SerpentsLegacy = 534;
        }

        public static class Config
        {
            public static UserInt
                VPR_ST_SecondWind_Threshold = new("VPR_ST_SecondWindThreshold", 25),
                VPR_ST_Bloodbath_Threshold = new("VPR_ST_BloodbathThreshold", 40),
                VPR_AoE_SecondWind_Threshold = new("VPR_AoE_SecondWindThreshold", 25),
                VPR_AoE_Bloodbath_Threshold = new("VPR_AoE_BloodbathThreshold", 40),
                VPR_ST_UncoiledFury_HoldCharges = new("VPR_ST_UncoiledFury_HoldCharges", 1),
                VPR_AoE_UncoiledFury_HoldCharges = new("VPR_AoE_UncoiledFury_HoldCharges", 0),
                VPR_Positional = new("VPR_Positional"),
                VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton");

            public static UserFloat
                VPR_ST_Reawaken_Usage = new("VPR_ST_Reawaken_Usage", 2),
                VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 2),
                VPR_ST_NoxiousDebuffRefresh = new("VPR_ST_NoxiousDebuffRefresh", 20.0f),
                VPR_AoE_NoxiousDebuffRefresh = new("VPR_AoE_NoxiousDebuffRefresh", 20.0f);
        }

        internal class VPR_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_SimpleMode;
            internal static VPROpenerLogic VPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                bool trueNorthDynReady = trueNorthReady;
                bool DreadwinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;
                float GCD = GetCooldown(OriginalHook(DreadFangs)).CooldownTotal;


                if (actionID is SteelFangs)
                {
                    // Opener for VPR
                    if (VPROpener.DoFullOpener(ref actionID))
                        return actionID;

                    // Uncoiled combo
                    if (!HasEffect(Buffs.Reawakened) && CanWeave(actionID))
                    {
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);
                    }

                    //Dreadwinder Combo
                    if (!HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken))
                    {
                        if (HasEffect(Buffs.HuntersVenom) && CanWeave(actionID))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.SwiftskinsVenom) && CanWeave(actionID))
                            return OriginalHook(Twinblood);

                        if (SwiftskinsCoilReady)
                            return HuntersCoil;

                        if (DreadwinderReady)
                            return SwiftskinsCoil;
                    }

                    if (LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                        return gauge.HasRattlingCoilStack()
                            ? UncoiledFury
                            : WrithingSnap;

                    //Reawakend Usage
                    if (UseReawaken(gauge))
                        return Reawaken;

                    //Overcap protection
                    if ((HasCharges(Dreadwinder) || GetCooldownRemainingTime(SerpentsIre) <= GCD * 5) && !HasEffect(Buffs.Reawakened) &&
                        ((gauge.RattlingCoilStacks is 3 && TraitLevelChecked(Traits.EnhancedVipersRattle)) ||
                        (gauge.RattlingCoilStacks is 2 && !TraitLevelChecked(Traits.EnhancedVipersRattle))))
                        return UncoiledFury;

                    //Serpents Ire usage
                    if (CanWeave(actionID) && gauge.RattlingCoilStacks <= 2 &&
                        ActionReady(SerpentsIre) && !HasEffect(Buffs.Reawakened))
                        return SerpentsIre;

                    //Dreadwinder Usage
                    if (ActionReady(Dreadwinder) && !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                        GetDebuffRemainingTime(Debuffs.NoxiousGnash) < 20 &&
                        (GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) &&
                        (GetBuffRemainingTime(Buffs.FlankstungVenom) >= 20 || GetBuffRemainingTime(Buffs.FlanksbaneVenom) >= 20 ||
                        GetBuffRemainingTime(Buffs.HindstungVenom) >= 20 || GetBuffRemainingTime(Buffs.HindsbaneVenom) >= 20))
                    {
                        if (trueNorthReady)
                            return All.TrueNorth;

                        return Dreadwinder;
                    }

                    // Uncoiled Fury usage
                    if (LevelChecked(UncoiledFury) && gauge.HasRattlingCoilStack() && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !DreadwinderReady && !HuntersCoilReady && !SwiftskinsCoilReady && !HasEffect(Buffs.Reawakened) &&
                        !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) &&
                        !WasLastWeaponskill(FlankstingStrike) && !WasLastWeaponskill(FlanksbaneFang) &&
                        !WasLastWeaponskill(HindstingStrike) && !WasLastWeaponskill(HindsbaneFang) && !WasLastAbility(SerpentsIre) &&
                        (GetBuffRemainingTime(Buffs.FlankstungVenom) >= 20 || GetBuffRemainingTime(Buffs.FlanksbaneVenom) >= 20 ||
                        GetBuffRemainingTime(Buffs.HindstungVenom) >= 20 || GetBuffRemainingTime(Buffs.HindsbaneVenom) >= 20))
                        return UncoiledFury;

                    //Reawaken combo
                    if (HasEffect(Buffs.Reawakened))
                    {
                        //Pre Ouroboros
                        if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(DreadFangs);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsCoil);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            //Legacy weaves
                            if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                                (WasLastAction(OriginalHook(SteelFangs)) || WasLastAction(OriginalHook(DreadFangs)) ||
                                WasLastAction(OriginalHook(HuntersCoil)) || WasLastAction(OriginalHook(SwiftskinsCoil))))
                                return OriginalHook(SerpentsTail);

                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(DreadFangs);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(SwiftskinsCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(Reawaken);
                        }
                    }

                    // healing
                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;

                    //1-2-3 (4-5-6) Combo
                    if (comboTime > 0 && !HasEffect(Buffs.Reawakened))
                    {
                        if (lastComboMove is DreadFangs or SteelFangs)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)) && LevelChecked(HuntersSting))
                                return OriginalHook(SteelFangs);

                            if ((HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))) && LevelChecked(SwiftskinsSting))
                                return OriginalHook(DreadFangs);
                        }

                        if (lastComboMove is HuntersSting or SwiftskinsSting)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) && LevelChecked(FlanksbaneFang))
                            {
                                if (trueNorthDynReady && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom))
                                    return All.TrueNorth;

                                if (trueNorthDynReady && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom))
                                    return All.TrueNorth;

                                return OriginalHook(SteelFangs);
                            }

                            if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) && LevelChecked(HindstingStrike))
                            {
                                if (trueNorthDynReady && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom))
                                    return All.TrueNorth;

                                if (trueNorthDynReady && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom))
                                    return All.TrueNorth;

                                return OriginalHook(DreadFangs);
                            }
                        }

                        if (lastComboMove is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                        {
                            if (CanWeave(actionID) && LevelChecked(SerpentsTail) && HasCharges(DeathRattle) &&
                                (WasLastWeaponskill(HindstingStrike) || WasLastWeaponskill(HindsbaneFang) ||
                                WasLastWeaponskill(FlankstingStrike) || WasLastWeaponskill(FlanksbaneFang)))
                                return OriginalHook(SerpentsTail);

                        }
                        return ((GetDebuffRemainingTime(Debuffs.NoxiousGnash) < 20 ||
                            GetCooldownRemainingTime(SerpentsIre) <= GCD * 4) &&
                            LevelChecked(DreadFangs) && !ActionReady(Dreadwinder))
                            ? OriginalHook(DreadFangs)
                            : OriginalHook(SteelFangs);
                    }
                    return LevelChecked(DreadFangs)
                            ? OriginalHook(DreadFangs)
                            : OriginalHook(SteelFangs);
                }
                return actionID;
            }

            private static bool UseReawaken(VPRGauge gauge)
            {
                float AwGCD = GetCooldown(FirstGeneration).CooldownTotal;
                float GCD = GetCooldown(OriginalHook(DreadFangs)).CooldownTotal;
                int SerpentsIreUsed = ActionWatching.CombatActions.Count(x => x == SerpentsIre);

                if (HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                    !HasEffect(Buffs.Reawakened) &&
                    !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                    !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))

                {
                    //even minutes
                    if ((SerpentsIreUsed <= 3 || SerpentsIreUsed >= 5) && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (WasLastAbility(SerpentsIre) ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros)) ||
                        HasEffect(Buffs.ReadyToReawaken) ||
                        (gauge.SerpentOffering >= 95 && WasLastAbility(SerpentsIre))))
                        return true;

                    // odd minutes
                    if (GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 7 &&
                        gauge.SerpentOffering >= 50 &&
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 30 &&
                        GetCooldownRemainingTime(SerpentsIre) >= GCD * 20)
                        return true;

                    // 6 minutes
                    if (SerpentsIreUsed == 4 && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (WasLastAbility(SerpentsIre) ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros)) ||
                        HasEffect(Buffs.ReadyToReawaken) ||
                        (gauge.SerpentOffering >= 95 && WasLastAbility(SerpentsIre))))
                        return true;

                    // 7min 2RA
                    if (SerpentsIreUsed == 4 && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (gauge.SerpentOffering >= 95 ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros))) &&
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 36 &&
                        GetCooldownRemainingTime(SerpentsIre) >= GCD * 18)
                        return true;
                }

                return false;
            }
        }

        internal class VPR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_AdvancedMode;
            internal static VPROpenerLogic VPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                bool trueNorthDynReady = trueNorthReady;
                float ST_NoxiousDebuffRefresh = Config.VPR_ST_NoxiousDebuffRefresh;
                int positionalChoice = Config.VPR_Positional;
                bool DreadwinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;
                float GCD = GetCooldown(OriginalHook(DreadFangs)).CooldownTotal;

                // Prevent the dynamic true north option from using the last charge
                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                    IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic_HoldCharge) &&
                    GetRemainingCharges(All.TrueNorth) < 2 && trueNorthDynReady)
                    trueNorthDynReady = false;

                if (actionID is SteelFangs)
                {
                    // Opener for VPR
                    if (IsEnabled(CustomComboPreset.VPR_ST_Opener))
                    {
                        if (VPROpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    // Uncoiled combo
                    if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFuryCombo) &&
                        !HasEffect(Buffs.Reawakened) && CanWeave(actionID))
                    {
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);
                    }

                    //Dreadwinder Combo
                    if (IsEnabled(CustomComboPreset.VPR_ST_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_ST_DreadwinderCombo) &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken))
                    {
                        if (HasEffect(Buffs.HuntersVenom) && CanWeave(actionID))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.SwiftskinsVenom) && CanWeave(actionID))
                            return OriginalHook(Twinblood);

                        if (positionalChoice is 0)
                        {
                            if (SwiftskinsCoilReady)
                                return HuntersCoil;

                            if (DreadwinderReady)
                                return SwiftskinsCoil;
                        }

                        if (positionalChoice is 1)
                        {
                            if (HuntersCoilReady)
                                return SwiftskinsCoil;

                            if (DreadwinderReady)
                                return HuntersCoil;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.VPR_ST_RangedUptime) &&
                        LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                        return (IsEnabled(CustomComboPreset.VPR_ST_RangedUptimeUncoiledFury) && gauge.HasRattlingCoilStack())
                            ? UncoiledFury
                            : WrithingSnap;

                    //Reawakend Usage
                    if (UseReawaken(gauge))
                        return Reawaken;

                    //Overcap protection
                    if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) &&
                        (HasCharges(Dreadwinder) || GetCooldownRemainingTime(SerpentsIre) <= GCD * 5) && !HasEffect(Buffs.Reawakened) &&
                        ((gauge.RattlingCoilStacks is 3 && TraitLevelChecked(Traits.EnhancedVipersRattle)) ||
                        (gauge.RattlingCoilStacks is 2 && !TraitLevelChecked(Traits.EnhancedVipersRattle))))
                        return UncoiledFury;

                    //Serpents Ire usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_ST_SerpentsIre) &&
                        CanWeave(actionID) && gauge.RattlingCoilStacks <= 2 && ActionReady(SerpentsIre) && !HasEffect(Buffs.Reawakened))
                        return SerpentsIre;

                    //Dreadwinder Usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_ST_Dreadwinder) &&
                        ActionReady(Dreadwinder) && !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                        GetDebuffRemainingTime(Debuffs.NoxiousGnash) < ST_NoxiousDebuffRefresh &&
                        (GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) &&
                        (GetBuffRemainingTime(Buffs.FlankstungVenom) >= 20 || GetBuffRemainingTime(Buffs.FlanksbaneVenom) >= 20 ||
                        GetBuffRemainingTime(Buffs.HindstungVenom) >= 20 || GetBuffRemainingTime(Buffs.HindsbaneVenom) >= 20))
                    {
                        if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                            trueNorthReady)
                            return All.TrueNorth;

                        return Dreadwinder;
                    }

                    // Uncoiled Fury usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) &&
                        LevelChecked(UncoiledFury) && gauge.RattlingCoilStacks > Config.VPR_ST_UncoiledFury_HoldCharges &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !DreadwinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                        !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) &&
                        !WasLastWeaponskill(FlankstingStrike) && !WasLastWeaponskill(FlanksbaneFang) &&
                        !WasLastWeaponskill(HindstingStrike) && !WasLastWeaponskill(HindsbaneFang) && !WasLastAbility(SerpentsIre) &&
                        (GetBuffRemainingTime(Buffs.FlankstungVenom) >= 20 || GetBuffRemainingTime(Buffs.FlanksbaneVenom) >= 20 ||
                        GetBuffRemainingTime(Buffs.HindstungVenom) >= 20 || GetBuffRemainingTime(Buffs.HindsbaneVenom) >= 20))
                        return UncoiledFury;

                    //Reawaken combo
                    if (IsEnabled(CustomComboPreset.VPR_ST_ReawakenCombo) &&
                        HasEffect(Buffs.Reawakened))
                    {
                        //Pre Ouroboros
                        if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(DreadFangs);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsCoil);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            //Legacy weaves
                            if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                                (WasLastAction(OriginalHook(SteelFangs)) || WasLastAction(OriginalHook(DreadFangs)) ||
                                WasLastAction(OriginalHook(HuntersCoil)) || WasLastAction(OriginalHook(SwiftskinsCoil))))
                                return OriginalHook(SerpentsTail);

                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(DreadFangs);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(SwiftskinsCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(Reawaken);
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.VPR_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.VPR_ST_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.VPR_ST_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 (4-5-6) Combo
                    if (comboTime > 0 && !HasEffect(Buffs.Reawakened))
                    {
                        if (lastComboMove is DreadFangs or SteelFangs)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)) && LevelChecked(HuntersSting))
                                return OriginalHook(SteelFangs);

                            if ((HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))) && LevelChecked(SwiftskinsSting))
                                return OriginalHook(DreadFangs);
                        }

                        if (lastComboMove is HuntersSting or SwiftskinsSting)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) && LevelChecked(FlanksbaneFang))
                            {
                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom))
                                    return All.TrueNorth;

                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom))
                                    return All.TrueNorth;

                                return OriginalHook(SteelFangs);
                            }

                            if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) && LevelChecked(HindstingStrike))
                            {
                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom))
                                    return All.TrueNorth;

                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom))
                                    return All.TrueNorth;

                                return OriginalHook(DreadFangs);
                            }
                        }

                        if (lastComboMove is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                        {
                            if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsTail) &&
                                CanWeave(actionID) && LevelChecked(SerpentsTail) && HasCharges(DeathRattle) &&
                                (WasLastWeaponskill(HindstingStrike) || WasLastWeaponskill(HindsbaneFang) ||
                                WasLastWeaponskill(FlankstingStrike) || WasLastWeaponskill(FlanksbaneFang)))
                                return OriginalHook(SerpentsTail);

                        }
                        return (IsEnabled(CustomComboPreset.VPR_ST_NoxiousGnash) &&
                            (GetDebuffRemainingTime(Debuffs.NoxiousGnash) < ST_NoxiousDebuffRefresh ||
                            GetCooldownRemainingTime(SerpentsIre) <= GCD * 4) && LevelChecked(DreadFangs) &&
                            ((IsEnabled(CustomComboPreset.VPR_ST_Dreadwinder) &&
                            (!ActionReady(Dreadwinder))) ||
                            !IsEnabled(CustomComboPreset.VPR_ST_Dreadwinder)))
                            ? OriginalHook(DreadFangs)
                            : OriginalHook(SteelFangs);
                    }
                    return IsEnabled(CustomComboPreset.VPR_ST_NoxiousGnash) && LevelChecked(DreadFangs)
                            ? OriginalHook(DreadFangs)
                            : OriginalHook(SteelFangs);
                }
                return actionID;
            }

            private static bool UseReawaken(VPRGauge gauge)
            {
                float AwGCD = GetCooldown(FirstGeneration).CooldownTotal;
                float GCD = GetCooldown(OriginalHook(DreadFangs)).CooldownTotal;
                int SerpentsIreUsed = ActionWatching.CombatActions.Count(x => x == SerpentsIre);

                if (IsEnabled(CustomComboPreset.VPR_ST_Reawaken) &&
                    HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                    !HasEffect(Buffs.Reawakened) &&
                    !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                    !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
                    GetTargetHPPercent() >= Config.VPR_ST_Reawaken_Usage)
                {
                    //even minutes
                    if ((SerpentsIreUsed <= 3 || SerpentsIreUsed >= 5) && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (WasLastAbility(SerpentsIre) ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros)) ||
                        HasEffect(Buffs.ReadyToReawaken) ||
                        (gauge.SerpentOffering >= 95 && WasLastAbility(SerpentsIre))))
                        return true;

                    // odd minutes
                    if (GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 7 &&
                        gauge.SerpentOffering >= 50 &&
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 30 &&
                        GetCooldownRemainingTime(SerpentsIre) >= GCD * 20)
                        return true;

                    // 6 minutes
                    if (SerpentsIreUsed == 4 && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (WasLastAbility(SerpentsIre) ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros)) ||
                        HasEffect(Buffs.ReadyToReawaken) ||
                        (gauge.SerpentOffering >= 95 && WasLastAbility(SerpentsIre))))
                        return true;

                    // 7min 2RA
                    if (SerpentsIreUsed == 4 && GetDebuffRemainingTime(Debuffs.NoxiousGnash) >= AwGCD * 10 &&
                        (gauge.SerpentOffering >= 95 ||
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(Ouroboros))) &&
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 36 &&
                        GetCooldownRemainingTime(SerpentsIre) >= GCD * 18)
                        return true;
                }

                return false;
            }
        }

        internal class VPR_AoE_Simplemode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool PitOfDreadReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;
                bool HuntersDenReady = gauge.DreadCombo == DreadCombo.HuntersDen;
                float GCD = GetCooldown(DreadMaw).CooldownTotal;
                float AwGCD = GetCooldown(FirstGeneration).CooldownTotal;

                if (actionID is SteelMaw)
                {
                    // Uncoiled combo
                    if (!HasEffect(Buffs.Reawakened))
                    {
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);
                    }

                    if (HasEffect(Buffs.FellhuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.FellskinsVenom))
                        return OriginalHook(Twinblood);

                    if (SwiftskinsDenReady)
                        return HuntersDen;

                    if (PitOfDreadReady)
                        return SwiftskinsDen;

                    //Reawakend Usage
                    if ((HasEffect(Buffs.ReadyToReawaken) || gauge.SerpentOffering >= 50) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !HasEffect(Buffs.Reawakened) &&
                        !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                        !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                        return Reawaken;

                    //Overcap protection
                    if ((HasCharges(Dreadwinder) || GetCooldownRemainingTime(SerpentsIre) <= GCD * 2) && !HasEffect(Buffs.Reawakened) &&
                        ((gauge.RattlingCoilStacks is 3 && TraitLevelChecked(Traits.EnhancedVipersRattle)) ||
                        (gauge.RattlingCoilStacks is 2 && !TraitLevelChecked(Traits.EnhancedVipersRattle))))
                        return UncoiledFury;

                    //Serpents Ire usage
                    if (CanWeave(actionID) && gauge.RattlingCoilStacks <= 2 && ActionReady(SerpentsIre) && !HasEffect(Buffs.Reawakened))
                        return SerpentsIre;

                    //Pit of Dread Usage
                    if (ActionReady(PitofDread) && !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                    GetDebuffRemainingTime(Debuffs.NoxiousGnash) < 20 &&
                    (GetCooldownRemainingTime(SerpentsIre) >= GCD * 5))
                        return PitofDread;

                    // Uncoiled Fury usage
                    if (LevelChecked(UncoiledFury) && gauge.HasRattlingCoilStack() &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        PitOfDreadReady && !HuntersDenReady && !SwiftskinsDenReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                        !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) &&
                        !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) &&
                        !WasLastAbility(SerpentsIre))
                        return UncoiledFury;

                    //Reawaken combo
                    if (HasEffect(Buffs.Reawakened))
                    {
                        //Pre Ouroboros
                        if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(DreadMaw);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsDen);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            //Legacy weaves
                            if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                                (WasLastAction(OriginalHook(SteelMaw)) || WasLastAction(OriginalHook(DreadMaw)) ||
                                WasLastAction(OriginalHook(HuntersDen)) || WasLastAction(OriginalHook(SwiftskinsDen))))
                                return OriginalHook(SerpentsTail);

                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(DreadMaw);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(SwiftskinsDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(Reawaken);
                        }
                    }

                    // healing
                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;

                    //1-2-3 (4-5-6) Combo
                    if (comboTime > 0 && !HasEffect(Buffs.Reawakened))
                    {
                        if (lastComboMove is DreadMaw or SteelMaw)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(HuntersBite))
                                return OriginalHook(SteelMaw);

                            if ((HasEffect(Buffs.GrimskinsVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))) && LevelChecked(SwiftskinsBite))
                                return OriginalHook(DreadMaw);
                        }

                        if (lastComboMove is HuntersBite or SwiftskinsBite)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                                return OriginalHook(SteelMaw);

                            if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                                return OriginalHook(DreadMaw);
                        }

                        if (lastComboMove is BloodiedMaw or JaggedMaw)
                        {
                            if (CanWeave(actionID) && LevelChecked(SerpentsTail) && HasCharges(LastLash) &&
                                (WasLastWeaponskill(BloodiedMaw) || WasLastWeaponskill(JaggedMaw)))
                                return OriginalHook(SerpentsTail);
                        }

                        return ((GetDebuffRemainingTime(Debuffs.NoxiousGnash) < 20 ||
                           GetCooldownRemainingTime(SerpentsIre) <= GCD * 4) && LevelChecked(DreadMaw) && !ActionReady(PitofDread))
                           ? OriginalHook(DreadMaw)
                           : OriginalHook(SteelMaw);
                    }
                    return LevelChecked(DreadMaw)
                            ? OriginalHook(DreadMaw)
                            : OriginalHook(SteelMaw);
                }
                return actionID;
            }
        }

        internal class VPR_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                float AoE_NoxiousDebuffRefresh = Config.VPR_AoE_NoxiousDebuffRefresh;
                bool PitOfDreadReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;
                bool HuntersDenReady = gauge.DreadCombo == DreadCombo.HuntersDen;
                float GCD = GetCooldown(DreadMaw).CooldownTotal;
                float AwGCD = GetCooldown(FirstGeneration).CooldownTotal;

                if (actionID is SteelMaw)
                {
                    // Uncoiled combo
                    if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFuryCombo) &&
                        !HasEffect(Buffs.Reawakened))
                    {
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);
                    }

                    if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_AoE_PitOfDreadCombo))
                    {
                        if (HasEffect(Buffs.FellhuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.FellskinsVenom))
                            return OriginalHook(Twinblood);

                        if (SwiftskinsDenReady)
                            return HuntersDen;

                        if (PitOfDreadReady)
                            return SwiftskinsDen;
                    }

                    //Reawakend Usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_Reawaken) &&
                        (HasEffect(Buffs.ReadyToReawaken) || gauge.SerpentOffering >= 50) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !HasEffect(Buffs.Reawakened) &&
                        !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                        !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                        return Reawaken;

                    //Overcap protection
                    if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                        (HasCharges(Dreadwinder) || GetCooldownRemainingTime(SerpentsIre) <= GCD * 2) && !HasEffect(Buffs.Reawakened) &&
                        ((gauge.RattlingCoilStacks is 3 && TraitLevelChecked(Traits.EnhancedVipersRattle)) ||
                        (gauge.RattlingCoilStacks is 2 && !TraitLevelChecked(Traits.EnhancedVipersRattle))))
                        return UncoiledFury;

                    //Serpents Ire usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_AoE_SerpentsIre) &&
                       CanWeave(actionID) && gauge.RattlingCoilStacks <= 2 && ActionReady(SerpentsIre) && !HasEffect(Buffs.Reawakened))
                        return SerpentsIre;

                    //Pit of Dread Usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                    IsEnabled(CustomComboPreset.VPR_AoE_PitOfDread) &&
                    ActionReady(PitofDread) && !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                    GetDebuffRemainingTime(Debuffs.NoxiousGnash) < AoE_NoxiousDebuffRefresh &&
                    (GetCooldownRemainingTime(SerpentsIre) >= GCD * 5))
                        return PitofDread;

                    // Uncoiled Fury usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                        LevelChecked(UncoiledFury) && gauge.RattlingCoilStacks > Config.VPR_AoE_UncoiledFury_HoldCharges &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        PitOfDreadReady && !HuntersDenReady && !SwiftskinsDenReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                        !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) &&
                        !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) &&
                        !WasLastAbility(SerpentsIre))
                        return UncoiledFury;

                    //Reawaken combo
                    if (IsEnabled(CustomComboPreset.VPR_AoE_ReawakenCombo) &&
                        HasEffect(Buffs.Reawakened))
                    {
                        //Pre Ouroboros
                        if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(DreadMaw);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsDen);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            //Legacy weaves
                            if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                                (WasLastAction(OriginalHook(SteelMaw)) || WasLastAction(OriginalHook(DreadMaw)) ||
                                WasLastAction(OriginalHook(HuntersDen)) || WasLastAction(OriginalHook(SwiftskinsDen))))
                                return OriginalHook(SerpentsTail);

                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(DreadMaw);

                            if (gauge.AnguineTribute is 3)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(SwiftskinsDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(Reawaken);
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.VPR_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.VPR_AoE_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.VPR_AoE_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 (4-5-6) Combo
                    if (comboTime > 0 && !HasEffect(Buffs.Reawakened))
                    {
                        if (lastComboMove is DreadMaw or SteelMaw)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(HuntersBite))
                                return OriginalHook(SteelMaw);

                            if ((HasEffect(Buffs.GrimskinsVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))) && LevelChecked(SwiftskinsBite))
                                return OriginalHook(DreadMaw);
                        }

                        if (lastComboMove is HuntersBite or SwiftskinsBite)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                                return OriginalHook(SteelMaw);

                            if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                                return OriginalHook(DreadMaw);
                        }

                        if (lastComboMove is BloodiedMaw or JaggedMaw)
                        {
                            if (IsEnabled(CustomComboPreset.VPR_AoE_SerpentsTail) &&
                                CanWeave(actionID) && LevelChecked(SerpentsTail) && HasCharges(LastLash) &&
                                (WasLastWeaponskill(BloodiedMaw) || WasLastWeaponskill(JaggedMaw)))
                                return OriginalHook(SerpentsTail);
                        }
                        return (IsEnabled(CustomComboPreset.VPR_AoE_NoxiousGnash) &&
                            (GetDebuffRemainingTime(Debuffs.NoxiousGnash) < AoE_NoxiousDebuffRefresh ||
                            GetCooldownRemainingTime(SerpentsIre) <= GCD * 4) && LevelChecked(DreadMaw) &&
                            ((IsEnabled(CustomComboPreset.VPR_AoE_PitOfDread) &&
                            (!ActionReady(PitofDread))) ||
                            !IsEnabled(CustomComboPreset.VPR_AoE_PitOfDread)))
                            ? OriginalHook(DreadMaw)
                            : OriginalHook(SteelMaw);
                    }
                    return IsEnabled(CustomComboPreset.VPR_AoE_NoxiousGnash) &&
                        LevelChecked(DreadMaw)
                            ? OriginalHook(DreadMaw)
                            : OriginalHook(SteelMaw);
                }
                return actionID;
            }
        }

        internal class VPR_DreadwinderCoils : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_DreadwinderCoils;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                int positionalChoice = Config.VPR_Positional;
                bool DreadwinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

                if (actionID is Dreadwinder)
                {
                    if (WasLastWeaponskill(HuntersCoil) || WasLastWeaponskill(SwiftskinsCoil))
                    {
                        if (HasEffect(Buffs.HuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.SwiftskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    if (positionalChoice is 0)
                    {
                        if (SwiftskinsCoilReady)
                            return HuntersCoil;

                        if (DreadwinderReady)
                            return SwiftskinsCoil;
                    }

                    if (positionalChoice is 1)
                    {
                        if (HuntersCoilReady)
                            return SwiftskinsCoil;

                        if (DreadwinderReady)
                            return HuntersCoil;
                    }
                }
                return actionID;
            }
        }

        internal class VPR_PitOfDreadDens : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_PitOfDreadDens;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool PitOfDreadReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;

                if (actionID is PitofDread)
                {
                    if (WasLastWeaponskill(HuntersDen) || WasLastWeaponskill(SwiftskinsDen))
                    {
                        if (HasEffect(Buffs.FellhuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.FellskinsVenom))
                            return OriginalHook(Twinblood);
                    }
                    if (SwiftskinsDenReady)
                        return HuntersDen;

                    if (PitOfDreadReady)
                        return SwiftskinsDen;
                }
                return actionID;
            }
        }

        internal class VPR_UncoiledTwins : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_UncoiledTwins;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is UncoiledFury)
                {
                    if (HasEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);
                }
                return actionID;
            }
        }

        internal class VPR_ReawakenLegacy : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ReawakenLegacy;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                int buttonChoice = Config.VPR_ReawakenLegacyButton;

                if ((buttonChoice is 0 && actionID is Reawaken && HasEffect(Buffs.Reawakened)) ||
                    (buttonChoice is 1 && actionID is SteelFangs && HasEffect(Buffs.Reawakened)))
                {
                    if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                    {
                        if (gauge.AnguineTribute is 4)
                            return OriginalHook(SteelFangs);

                        if (gauge.AnguineTribute is 3)
                            return OriginalHook(DreadFangs);

                        if (gauge.AnguineTribute is 2)
                            return OriginalHook(HuntersCoil);

                        if (gauge.AnguineTribute is 1)
                            return OriginalHook(SwiftskinsCoil);
                    }

                    if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                    {
                        //Legacy weaves
                        if (IsEnabled(CustomComboPreset.VPR_ReawakenLegacyWeaves))
                        {
                            if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                                (WasLastAction(OriginalHook(SteelFangs)) || WasLastAction(OriginalHook(DreadFangs)) ||
                                WasLastAction(OriginalHook(HuntersCoil)) || WasLastAction(OriginalHook(SwiftskinsCoil))))
                                return OriginalHook(SerpentsTail);
                        }

                        if (gauge.AnguineTribute is 5)
                            return OriginalHook(SteelFangs);

                        if (gauge.AnguineTribute is 4)
                            return OriginalHook(DreadFangs);

                        if (gauge.AnguineTribute is 3)
                            return OriginalHook(HuntersCoil);

                        if (gauge.AnguineTribute is 2)
                            return OriginalHook(SwiftskinsCoil);

                        if (gauge.AnguineTribute is 1)
                            return OriginalHook(Reawaken);
                    }
                }
                return actionID;
            }
        }

        internal class VPR_TwinTails : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_TwinTails;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SerpentsTail)
                {
                    if (TraitLevelChecked(Traits.SerpentsLegacy) && CanWeave(actionID) &&
                        (WasLastAction(OriginalHook(SteelMaw)) || WasLastAction(OriginalHook(DreadMaw)) ||
                        WasLastAction(OriginalHook(HuntersDen)) || WasLastAction(OriginalHook(SwiftskinsDen))))
                        return OriginalHook(SerpentsTail);

                    if (HasEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);

                    if (HasEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);

                    if (HasEffect(Buffs.FellhuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.FellskinsVenom))
                        return OriginalHook(Twinblood);

                }
                return actionID;
            }
        }
    }
}
