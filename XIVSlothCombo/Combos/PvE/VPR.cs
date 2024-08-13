using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.JobHelpers.VPRHelpers;

namespace XIVSlothCombo.Combos.PvE
{
    internal class VPR
    {
        public const byte JobID = 41;

        public const uint
            ReavingFangs = 34607,
            ReavingMaw = 34615,
            Vicewinder = 34620,
            HuntersCoil = 34621,
            HuntersDen = 34624,
            HuntersSnap = 39166,
            Vicepit = 34623,
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
                PoisedForTwinblood = 3666,
                HonedReavers = 3772,
                HonedSteel = 3672;
        }

        public static class Debuffs
        {
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
                VPR_ST_UncoiledFury_Threshold = new("VPR_ST_UncoiledFury_Threshold", 1),
                VPR_AoE_UncoiledFury_Threshold = new("VPR_AoE_UncoiledFury_Threshold", 1),
                VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton");

            public static UserFloat
                VPR_ST_Reawaken_Usage = new("VPR_ST_Reawaken_Usage", 2),
                VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 2);
        }

        internal class VPR_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_SimpleMode;
            internal static VPROpenerLogic VPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                double enemyHP = GetTargetHPPercent();
                bool VicewinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;
                float GCD = GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;
                int RattlingCoils = gauge.RattlingCoilStacks;
                bool CappedOnCoils = (TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 2) || (!TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 1);

                if (actionID is SteelFangs)
                {
                    // Opener for VPR
                    if (VPROpener.DoFullOpener(ref actionID))
                        return actionID;

                    //All Weaves
                    if (CanWeave(actionID))
                    {
                        // Death Rattle
                        if (LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                            return OriginalHook(SerpentsTail);

                        // Legacy Weaves
                        if (TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                            && OriginalHook(SerpentsTail) is not SerpentsTail)
                            return OriginalHook(SerpentsTail);

                        // Uncoiled weaves
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);
                        if (!HasEffect(Buffs.Reawakened))
                        {
                            //Vicewinder weaves
                            if (HasEffect(Buffs.HuntersVenom) && InMeleeRange())
                                return OriginalHook(Twinfang);

                            if (HasEffect(Buffs.SwiftskinsVenom) && InMeleeRange())
                                return OriginalHook(Twinblood);
                        }
                    }

                    //Serpents Ire usage
                    if (!CappedOnCoils && CanWeave(Ouroboros) && ActionReady(SerpentsIre))
                        return SerpentsIre;

                    if (LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                        return VPRCheckRattlingCoils.HasRattlingCoilStack(gauge)
                            ? UncoiledFury
                            : WrithingSnap;

                    //Vicewinder Combo
                    if (!HasEffect(Buffs.Reawakened) && LevelChecked(Vicewinder) && InMeleeRange())
                    {
                        // Swiftskin's Coil
                        if ((VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals())) || HuntersCoilReady)
                            return SwiftskinsCoil;

                        // Hunter's Coil
                        if ((VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals())) || SwiftskinsCoilReady)
                            return HuntersCoil;
                    }

                    //Reawakend Usage
                    if (UseReawaken(gauge))
                        return Reawaken;

                    //Overcap protection
                    if (CappedOnCoils && //3 coils
                        ((HasCharges(Vicewinder) && !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.Reawakened)) || //spend if Vicewinder is up, after Reawaken
                        (GetCooldownRemainingTime(SerpentsIre) <= GCD * 5))) //spend in case under Reawaken right as Ire comes up
                        return UncoiledFury;

                    //Vicewinder Usage
                    if (ActionReady(Vicewinder) && !HasEffect(Buffs.Reawakened) && HasEffect(Buffs.Swiftscaled) &&
                        ((GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) || !LevelChecked(SerpentsIre)) && InMeleeRange() &&
                         !VPRCheckTimers.IsVenomExpiring(6) && !VPRCheckTimers.IsComboExpiring(6) && !VPRCheckTimers.IsHoningExpiring(6))
                        return Vicewinder;

                    // Uncoiled Fury usage
                    if (LevelChecked(UncoiledFury) && HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        (((gauge.RattlingCoilStacks > 1) ||
                        (enemyHP < 1 && VPRCheckRattlingCoils.HasRattlingCoilStack(gauge))) &&
                        !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                         !VPRCheckTimers.IsEmpowermentExpiring(6) && !VPRCheckTimers.IsVenomExpiring(6) &&
                        !VPRCheckTimers.IsComboExpiring(6) && !VPRCheckTimers.IsHoningExpiring(6)))
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
                                return OriginalHook(ReavingFangs);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsCoil);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(ReavingFangs);

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
                        if (lastComboMove is ReavingFangs or SteelFangs)
                        {
                            if (LevelChecked(HuntersSting) &&
                                (HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)))
                                return OriginalHook(SteelFangs);

                            if (LevelChecked(SwiftskinsSting) &&
                                (HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))))
                                return OriginalHook(ReavingFangs);
                        }

                        if (lastComboMove is HuntersSting or SwiftskinsSting)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) && LevelChecked(FlanksbaneFang))
                            {
                                if (trueNorthReady && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom))
                                    return All.TrueNorth;

                                if (trueNorthReady && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom))
                                    return All.TrueNorth;

                                return OriginalHook(SteelFangs);
                            }

                            if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) && LevelChecked(HindstingStrike))
                            {
                                if (trueNorthReady && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom))
                                    return All.TrueNorth;

                                if (trueNorthReady && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom))
                                    return All.TrueNorth;

                                return OriginalHook(ReavingFangs);
                            }
                        }

                        if (lastComboMove is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                            return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                                ? OriginalHook(ReavingFangs)
                                : OriginalHook(SteelFangs);
                    }
                    //for lower lvls
                    return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                               ? OriginalHook(ReavingFangs)
                               : OriginalHook(SteelFangs);
                }
                return actionID;
            }

            private static bool UseReawaken(VPRGauge gauge)
            {
                if (LevelChecked(Reawaken) && !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                    !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                    !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
                    !VPRCheckTimers.IsEmpowermentExpiring(6))
                {
                    if ((!JustUsed(SerpentsIre, 3f) && HasEffect(Buffs.ReadyToReawaken)) || //2min burst
                        (WasLastWeaponskill(Ouroboros) && gauge.SerpentOffering >= 50) || //2nd RA
                        (gauge.SerpentOffering is >= 50 and <= 80 && GetCooldownRemainingTime(SerpentsIre) is >= 50 and <= 65) || //1min
                        (gauge.SerpentOffering >= 100) || //overcap
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(FourthGeneration) && !LevelChecked(Ouroboros))) //<100
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
                int uncoiledThreshold = Config.VPR_ST_UncoiledFury_Threshold;
                double enemyHP = GetTargetHPPercent();
                bool VicewinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;
                float GCD = GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;
                int RattlingCoils = gauge.RattlingCoilStacks;
                bool CappedOnCoils = (TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 2) || (!TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 1);


                if (actionID is SteelFangs)
                {
                    // Opener for VPR
                    if (IsEnabled(CustomComboPreset.VPR_ST_Opener))
                    {
                        if (VPROpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    //All Weaves
                    if (CanWeave(actionID))
                    {
                        // Death Rattle
                        if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsTail) &&
                            LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                            return OriginalHook(SerpentsTail);

                        // Legacy Weaves
                        if (IsEnabled(CustomComboPreset.VPR_ST_ReawakenCombo) &&
                            TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                            && OriginalHook(SerpentsTail) is not SerpentsTail)
                            return OriginalHook(SerpentsTail);

                        // Uncoiled weaves
                        if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFuryCombo))
                        {
                            if (HasEffect(Buffs.PoisedForTwinfang))
                                return OriginalHook(Twinfang);

                            if (HasEffect(Buffs.PoisedForTwinblood))
                                return OriginalHook(Twinblood);
                        }

                        if (!HasEffect(Buffs.Reawakened))
                        {
                            if (IsEnabled(CustomComboPreset.VPR_ST_CDs))
                            {
                                //vicewinder weaves
                                if (IsEnabled(CustomComboPreset.VPR_ST_VicewinderCombo) && InMeleeRange())
                                {
                                    if (HasEffect(Buffs.HuntersVenom))
                                        return OriginalHook(Twinfang);

                                    if (HasEffect(Buffs.SwiftskinsVenom))
                                        return OriginalHook(Twinblood);
                                }
                            }
                        }
                    }

                    //Serpents Ire usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsIre) &&
                        CanWeave(Ouroboros) &&
                        !CappedOnCoils && ActionReady(SerpentsIre))
                        return SerpentsIre;

                    if (IsEnabled(CustomComboPreset.VPR_ST_RangedUptime) &&
                        LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                        return (IsEnabled(CustomComboPreset.VPR_ST_RangedUptimeUncoiledFury) && VPRCheckRattlingCoils.HasRattlingCoilStack(gauge))
                            ? UncoiledFury
                            : WrithingSnap;

                    //Vicewinder Combo
                    if (IsEnabled(CustomComboPreset.VPR_ST_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_ST_VicewinderCombo) &&
                        !HasEffect(Buffs.Reawakened) && LevelChecked(Vicewinder) && InMeleeRange())
                    {
                        // Swiftskin's Coil
                        if ((VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals())) || HuntersCoilReady)
                            return SwiftskinsCoil;

                        // Hunter's Coil
                        if ((VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals())) || SwiftskinsCoilReady)
                            return HuntersCoil;
                    }

                    //Reawakend Usage
                    if (UseReawaken(gauge))
                        return Reawaken;

                    //Overcap protection
                    if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) && CappedOnCoils &&
                        ((HasCharges(Vicewinder) && !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.Reawakened)) || //spend if Vicewinder is up, after Reawaken
                        (GetCooldownRemainingTime(SerpentsIre) <= GCD * 5))) //spend in case under Reawaken right as Ire comes up
                        return UncoiledFury;

                    //Vicewinder Usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_ST_Vicewinder) && HasEffect(Buffs.Swiftscaled) &&
                        ActionReady(Vicewinder) && !HasEffect(Buffs.Reawakened) && InMeleeRange() &&
                        ((GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) || !LevelChecked(SerpentsIre)) &&
                         !VPRCheckTimers.IsVenomExpiring(3) && !VPRCheckTimers.IsComboExpiring(3) && !VPRCheckTimers.IsHoningExpiring(3))
                        return Vicewinder;

                    // Uncoiled Fury usage
                    if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) &&
                        LevelChecked(UncoiledFury) && HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        ((gauge.RattlingCoilStacks > Config.VPR_ST_UncoiledFury_HoldCharges) ||
                        (enemyHP < uncoiledThreshold && VPRCheckRattlingCoils.HasRattlingCoilStack(gauge))) &&
                        !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) && !WasLastWeaponskill(Ouroboros) &&
                         !VPRCheckTimers.IsEmpowermentExpiring(6) && !VPRCheckTimers.IsVenomExpiring(3) &&
                        !VPRCheckTimers.IsComboExpiring(3) && !VPRCheckTimers.IsHoningExpiring(3))
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
                                return OriginalHook(ReavingFangs);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersCoil);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsCoil);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelFangs);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(ReavingFangs);

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
                        if (lastComboMove is ReavingFangs or SteelFangs)
                        {
                            if (LevelChecked(HuntersSting) &&
                                (HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)))
                                return OriginalHook(SteelFangs);

                            if (LevelChecked(SwiftskinsSting) &&
                                (HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                                (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))))
                                return OriginalHook(ReavingFangs);
                        }

                        if (lastComboMove is HuntersSting or SwiftskinsSting)
                        {
                            if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) && LevelChecked(FlanksbaneFang))
                            {
                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthReady && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom))
                                    return All.TrueNorth;

                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthReady && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom))
                                    return All.TrueNorth;

                                return OriginalHook(SteelFangs);
                            }

                            if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) && LevelChecked(HindstingStrike))
                            {
                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthReady && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom) && CanDelayedWeave(actionID))
                                    return All.TrueNorth;

                                if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                                    trueNorthReady && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom) && CanDelayedWeave(actionID))
                                    return All.TrueNorth;

                                return OriginalHook(ReavingFangs);
                            }
                        }

                        if (lastComboMove is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                            return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                                ? OriginalHook(ReavingFangs)
                                : OriginalHook(SteelFangs);
                    }
                    //for lower lvls
                    return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                               ? OriginalHook(ReavingFangs)
                               : OriginalHook(SteelFangs);
                }
                return actionID;
            }

            private static bool UseReawaken(VPRGauge gauge)
            {
                if (LevelChecked(Reawaken) && !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                    !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                    !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
                    !VPRCheckTimers.IsEmpowermentExpiring(6))
                {
                    if ((!JustUsed(SerpentsIre, 3f) && HasEffect(Buffs.ReadyToReawaken)) || //2min burst
                        (WasLastWeaponskill(Ouroboros) && gauge.SerpentOffering >= 50) || //2nd RA
                        (gauge.SerpentOffering is >= 50 and <= 80 && GetCooldownRemainingTime(SerpentsIre) is >= 50 and <= 65) || //1min
                        (gauge.SerpentOffering >= 100) || //overcap
                        (gauge.SerpentOffering >= 50 && WasLastWeaponskill(FourthGeneration) && !LevelChecked(Ouroboros))) //<100
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
                bool VicepitReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;
                bool HuntersDenReady = gauge.DreadCombo == DreadCombo.HuntersDen;
                float GCD = GetCooldown(ReavingMaw).CooldownTotal;
                int RattlingCoils = gauge.RattlingCoilStacks;
                bool CappedOnCoils = (TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 2) || (!TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 1);

                if (actionID is SteelMaw)
                {
                    if (CanWeave(actionID))
                    {
                        // Death Rattle
                        if (LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is LastLash)
                            return OriginalHook(SerpentsTail);

                        // Legacy Weaves
                        if (TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                            && OriginalHook(SerpentsTail) is not SerpentsTail)
                            return OriginalHook(SerpentsTail);

                        // Uncoiled combo
                        if (HasEffect(Buffs.PoisedForTwinfang))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.PoisedForTwinblood))
                            return OriginalHook(Twinblood);

                        if (!HasEffect(Buffs.Reawakened))
                        {
                            //Vicepit weaves
                            if (HasEffect(Buffs.FellhuntersVenom) && InMeleeRange())
                                return OriginalHook(Twinfang);

                            if (HasEffect(Buffs.FellskinsVenom) && InMeleeRange())
                                return OriginalHook(Twinblood);

                            //Serpents Ire usage
                            if (!CappedOnCoils && ActionReady(SerpentsIre))
                                return SerpentsIre;
                        }
                    }

                    //Vicepit combo
                    if (!HasEffect(Buffs.Reawakened) && InMeleeRange())
                    {
                        if (SwiftskinsDenReady)
                            return HuntersDen;

                        if (VicepitReady)
                            return SwiftskinsDen;
                    }

                    //Reawakend Usage
                    if ((HasEffect(Buffs.ReadyToReawaken) || gauge.SerpentOffering >= 50) && LevelChecked(Reawaken) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                        !HasEffect(Buffs.FellhuntersVenom) && !HasEffect(Buffs.FellskinsVenom) &&
                        !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                        return Reawaken;

                    //Overcap protection
                    if (((HasCharges(Vicepit) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom)) ||
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 2) && !HasEffect(Buffs.Reawakened) && CappedOnCoils)
                        return UncoiledFury;

                    //Vicepit Usage
                    if (ActionReady(Vicepit) && !HasEffect(Buffs.Reawakened) &&
                        ((GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) || !LevelChecked(SerpentsIre)) && InMeleeRange())
                        return Vicepit;

                    // Uncoiled Fury usage
                    if (LevelChecked(UncoiledFury) &&
                        VPRCheckRattlingCoils.HasRattlingCoilStack(gauge) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) &&
                        !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre))
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
                                return OriginalHook(ReavingMaw);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsDen);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(ReavingMaw);

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
                        if (lastComboMove is ReavingMaw or SteelMaw)
                        {
                            if (LevelChecked(HuntersBite) &&
                                HasEffect(Buffs.GrimhuntersVenom))
                                return OriginalHook(SteelMaw);

                            if (LevelChecked(SwiftskinsBite) &&
                                (HasEffect(Buffs.GrimskinsVenom) || (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))))
                                return OriginalHook(ReavingMaw);
                        }

                        if (lastComboMove is HuntersBite or SwiftskinsBite)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                                return OriginalHook(SteelMaw);

                            if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                                return OriginalHook(ReavingMaw);
                        }

                        if (lastComboMove is BloodiedMaw or JaggedMaw)
                            return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                                ? OriginalHook(ReavingMaw)
                                : OriginalHook(SteelMaw);
                    }
                    //for lower lvls
                    return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                               ? OriginalHook(ReavingMaw)
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
                int uncoiledThreshold = Config.VPR_AoE_UncoiledFury_Threshold;
                double enemyHP = GetTargetHPPercent();
                bool VicepitReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;
                bool HuntersDenReady = gauge.DreadCombo == DreadCombo.HuntersDen;
                float GCD = GetCooldown(ReavingMaw).CooldownTotal;
                int RattlingCoils = gauge.RattlingCoilStacks;
                bool CappedOnCoils = (TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 2) || (!TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoils > 1);

                if (actionID is SteelMaw)
                {
                    if (CanWeave(actionID))
                    {
                        // Death Rattle
                        if (IsEnabled(CustomComboPreset.VPR_AoE_SerpentsTail) &&
                            LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is LastLash)
                            return OriginalHook(SerpentsTail);

                        // Legacy Weaves
                        if (IsEnabled(CustomComboPreset.VPR_AoE_ReawakenCombo) &&
                            TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                            && OriginalHook(SerpentsTail) is not SerpentsTail)
                            return OriginalHook(SerpentsTail);

                        // Uncoiled combo
                        if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFuryCombo))
                        {
                            if (HasEffect(Buffs.PoisedForTwinfang))
                                return OriginalHook(Twinfang);

                            if (HasEffect(Buffs.PoisedForTwinblood))
                                return OriginalHook(Twinblood);
                        }

                        if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                            !HasEffect(Buffs.Reawakened))
                        {
                            //Vicepit weaves
                            if (IsEnabled(CustomComboPreset.VPR_AoE_VicepitCombo) &&
                                InMeleeRange())
                            {
                                if (HasEffect(Buffs.FellhuntersVenom))
                                    return OriginalHook(Twinfang);

                                if (HasEffect(Buffs.FellskinsVenom))
                                    return OriginalHook(Twinblood);
                            }

                            //Serpents Ire usage
                            if (IsEnabled(CustomComboPreset.VPR_AoE_SerpentsIre) &&
                                !CappedOnCoils && ActionReady(SerpentsIre))
                                return SerpentsIre;
                        }
                    }

                    //Vicepit combo
                    if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_AoE_VicepitCombo) &&
                        !HasEffect(Buffs.Reawakened) && InMeleeRange())
                    {
                        if (SwiftskinsDenReady)
                            return HuntersDen;

                        if (VicepitReady)
                            return SwiftskinsDen;
                    }

                    //Reawakend Usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_Reawaken) &&
                        (HasEffect(Buffs.ReadyToReawaken) || gauge.SerpentOffering >= 50) && LevelChecked(Reawaken) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                        !HasEffect(Buffs.FellhuntersVenom) && !HasEffect(Buffs.FellskinsVenom) &&
                        !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                        return Reawaken;

                    //Overcap protection
                    if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                        ((HasCharges(Vicepit) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom)) ||
                        GetCooldownRemainingTime(SerpentsIre) <= GCD * 2) && !HasEffect(Buffs.Reawakened) && CappedOnCoils)
                        return UncoiledFury;

                    //Vicepit Usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_CDs) &&
                        IsEnabled(CustomComboPreset.VPR_AoE_Vicepit) &&
                        ActionReady(Vicepit) && !HasEffect(Buffs.Reawakened) && InMeleeRange() &&
                        ((GetCooldownRemainingTime(SerpentsIre) >= GCD * 5) || !LevelChecked(SerpentsIre)))
                        return Vicepit;

                    // Uncoiled Fury usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                        LevelChecked(UncoiledFury) &&
                        ((gauge.RattlingCoilStacks > Config.VPR_AoE_UncoiledFury_HoldCharges) || (enemyHP < uncoiledThreshold && VPRCheckRattlingCoils.HasRattlingCoilStack(gauge))) &&
                        HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                        !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                        !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) &&
                        !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre))
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
                                return OriginalHook(ReavingMaw);

                            if (gauge.AnguineTribute is 2)
                                return OriginalHook(HuntersDen);

                            if (gauge.AnguineTribute is 1)
                                return OriginalHook(SwiftskinsDen);
                        }

                        //With Ouroboros
                        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        {
                            if (gauge.AnguineTribute is 5)
                                return OriginalHook(SteelMaw);

                            if (gauge.AnguineTribute is 4)
                                return OriginalHook(ReavingMaw);

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
                        if (lastComboMove is ReavingMaw or SteelMaw)
                        {
                            if (LevelChecked(HuntersBite) &&
                                HasEffect(Buffs.GrimhuntersVenom))
                                return OriginalHook(SteelMaw);

                            if (LevelChecked(SwiftskinsBite) &&
                                (HasEffect(Buffs.GrimskinsVenom) || (!HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct))))
                                return OriginalHook(ReavingMaw);
                        }

                        if (lastComboMove is HuntersBite or SwiftskinsBite)
                        {
                            if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                                return OriginalHook(SteelMaw);

                            if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                                return OriginalHook(ReavingMaw);
                        }

                        if (lastComboMove is BloodiedMaw or JaggedMaw)
                            return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                                ? OriginalHook(ReavingMaw)
                                : OriginalHook(SteelMaw);
                    }
                    //for lower lvls
                    return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                               ? OriginalHook(ReavingMaw)
                               : OriginalHook(SteelMaw);
                }
                return actionID;
            }
        }

        internal class VPR_VicewinderCoils : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_VicewinderCoils;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool VicewinderReady = gauge.DreadCombo == DreadCombo.Dreadwinder;
                bool HuntersCoilReady = gauge.DreadCombo == DreadCombo.HuntersCoil;
                bool SwiftskinsCoilReady = gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

                if (actionID is Vicewinder)
                {
                    if (IsEnabled(CustomComboPreset.VPR_VicewinderCoils_oGCDs))
                    {
                        if (HasEffect(Buffs.HuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.SwiftskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    // Vicewinder Combo
                    if (LevelChecked(Vicewinder))
                    {
                        // Swiftskin's Coil
                        if ((VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals())) || HuntersCoilReady)
                            return SwiftskinsCoil;

                        // Hunter's Coil
                        if ((VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals())) || SwiftskinsCoilReady)
                            return HuntersCoil;
                    }
                }
                return actionID;
            }
        }

        internal class VPR_VicepitDens : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_VicepitDens;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool VicepitReady = gauge.DreadCombo == DreadCombo.PitOfDread;
                bool SwiftskinsDenReady = gauge.DreadCombo == DreadCombo.SwiftskinsDen;

                if (actionID is Vicepit)
                {
                    if (IsEnabled(CustomComboPreset.VPR_VicepitDens_oGCDs))
                    {
                        if (HasEffect(Buffs.FellhuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.FellskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    if (SwiftskinsDenReady)
                        return HuntersDen;

                    if (VicepitReady)
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
                    // Legacy Weaves
                    if (IsEnabled(CustomComboPreset.VPR_ReawakenLegacyWeaves) &&
                        TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                        && OriginalHook(SerpentsTail) is not SerpentsTail)
                        return OriginalHook(SerpentsTail);

                    if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                    {
                        if (gauge.AnguineTribute is 4)
                            return OriginalHook(SteelFangs);

                        if (gauge.AnguineTribute is 3)
                            return OriginalHook(ReavingFangs);

                        if (gauge.AnguineTribute is 2)
                            return OriginalHook(HuntersCoil);

                        if (gauge.AnguineTribute is 1)
                            return OriginalHook(SwiftskinsCoil);
                    }

                    if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                    {
                        if (gauge.AnguineTribute is 5)
                            return OriginalHook(SteelFangs);

                        if (gauge.AnguineTribute is 4)
                            return OriginalHook(ReavingFangs);

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
                    // Death Rattle
                    if (LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                        return OriginalHook(SerpentsTail);

                    // Legacy Weaves
                    if (TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                        && OriginalHook(SerpentsTail) is not SerpentsTail)
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