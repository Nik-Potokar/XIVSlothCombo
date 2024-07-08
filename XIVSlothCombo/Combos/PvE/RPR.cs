using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class RPR
    {
        public const byte JobID = 39;

        public const uint
            // Single Target
            Slice = 24373,
            WaxingSlice = 24374,
            InfernalSlice = 24375,
            ShadowOfDeath = 24378,
            SoulSlice = 24380,
            // AoE
            SpinningScythe = 24376,
            NightmareScythe = 24377,
            WhorlOfDeath = 24379,
            SoulScythe = 24381,
            // Unveiled
            Gibbet = 24382,
            Gallows = 24383,
            Guillotine = 24384,
            UnveiledGibbet = 24390,
            UnveiledGallows = 24391,

            // Reaver
            BloodStalk = 24389,
            GrimSwathe = 24392,
            Gluttony = 24393,
            // Sacrifice
            ArcaneCircle = 24405,
            PlentifulHarvest = 24385,
            // Enshroud
            Enshroud = 24394,
            Communio = 24398,
            LemuresSlice = 24399,
            LemuresScythe = 24400,
            VoidReaping = 24395,
            CrossReaping = 24396,
            GrimReaping = 24397,
            // Miscellaneous
            HellsIngress = 24401,
            HellsEgress = 24402,
            Regress = 24403,
            Harpe = 24386,
            Soulsow = 24387,
            HarvestMoon = 24388;

        public static class Buffs
        {
            public const ushort
                SoulReaver = 2587,
                ImmortalSacrifice = 2592,
                ArcaneCircle = 2599,
                EnhancedGibbet = 2588,
                EnhancedGallows = 2589,
                EnhancedVoidReaping = 2590,
                EnhancedCrossReaping = 2591,
                EnhancedHarpe = 2845,
                Enshrouded = 2593,
                Soulsow = 2594,
                Threshold = 2595,
                BloodsownCircle = 2972;
        }

        public static class Debuffs
        {
            public const ushort
                DeathsDesign = 2586;
        }

        public static class Config
        {
            public static UserInt
                RPR_SoDThreshold = new("RPRSoDThreshold"),
                RPR_WoDThreshold = new("RPRWoDThreshold"),
                RPR_SoDRefreshRange = new("RPRSoDRefreshRange"),
                RPR_OpenerChoice = new("RPR_OpenerChoice"),
                RPR_Positional = new("RPR_Positional"),
                RPR_VariantCure = new("RPRVariantCure"),
                RPR_STSecondWindThreshold = new("RPR_STSecondWindThreshold"),
                RPR_STBloodbathThreshold = new("RPR_STBloodbathThreshold"),
                RPR_AoESecondWindThreshold = new("RPR_AoESecondWindThreshold"),
                RPR_AoEBloodbathThreshold = new("RPR_AoEBloodbathThreshold");
            public static UserBoolArray
               RPR_SoulsowOptions = new("RPR_SoulsowOptions");
            public static UserBool
               RPR_ST_TrueNorth_Moving = new("RPR_ST_TrueNorth_Moving");

        }

        internal class RPR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_AdvancedMode;
            internal static RPROpenerLogic RPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                double enemyHP = GetTargetHPPercent();
                int sodRefreshRange = Config.RPR_SoDRefreshRange;
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);
                bool trueNorthReadyDyn = trueNorthReady;

                // Prevent the dynamic true north option from using the last charge
                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                    IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                    GetRemainingCharges(All.TrueNorth) < 2 && trueNorthReady)
                    trueNorthReadyDyn = false;

                if (actionID is Slice)
                {
                    if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                        GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(actionID))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Opener))
                    {
                        if (RPROpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_RangedFiller) &&
                       !InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget())
                    {
                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                            return Communio;

                        if (HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon))
                        {
                            return (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_EnhancedHarpe) && HasEffect(Buffs.EnhancedHarpe)) ||
                                (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_CombatHarpe) && !InCombat())
                                ? Harpe
                                : HarvestMoon;
                        }

                        return Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD) && LevelChecked(ShadowOfDeath) && !HasEffect(Buffs.SoulReaver) && enemyHP > Config.RPR_SoDThreshold &&
                        ((LevelChecked(PlentifulHarvest) && ((GetCooldownRemainingTime(ArcaneCircle) < 9 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 28) || (GetCooldownRemainingTime(ArcaneCircle) < 4 && gauge.LemureShroud is 3 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 50))) || // Double Enshroud windows
                        (GetDebuffRemainingTime(Debuffs.DeathsDesign) <= sodRefreshRange && (IsOnCooldown(ArcaneCircle) || !LevelChecked(ArcaneCircle))))) // Other times
                        return ShadowOfDeath;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_CDs) && TargetHasEffect(Debuffs.DeathsDesign))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_PlentifulHarvest) &&
                            HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                            (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || WasLastAction(Communio)))
                            return PlentifulHarvest;

                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_EnshroudHarvestMoon) &&
                            HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon) && LevelChecked(Communio) &&
                            HasEffect(Buffs.ArcaneCircle) && gauge.LemureShroud is 1 && gauge.VoidShroud is 2 && WasLastAbility(LemuresSlice))
                            return HarvestMoon;

                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ArcaneCircle) &&
                                ((ActionReady(ArcaneCircle) && !LevelChecked(Communio)) ||
                                (ActionReady(ArcaneCircle) && LevelChecked(Communio) && gauge.LemureShroud is 2 &&
                                (WasLastAction(VoidReaping) || WasLastAction(CrossReaping))) ||
                                (ActionReady(ArcaneCircle) && combatDuration.Seconds < 10)))
                                return ArcaneCircle;

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Enshroud) &&
                                 LevelChecked(Enshroud) && gauge.Shroud >= 50 && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                                (!LevelChecked(PlentifulHarvest) || // Before Plentiful Harvest                               
                                HasEffect(Buffs.ArcaneCircle) || // Shroud in Arcane Circle
                                GetCooldownRemainingTime(ArcaneCircle) < 6 || // Prep for double Enshroud
                                WasLastAction(PlentifulHarvest) || //2nd part of Double Enshroud
                                (!HasEffect(Buffs.ArcaneCircle) && GetCooldownRemainingTime(ArcaneCircle) is >= 50 and <= 65) || //Natural Odd Minute Shrouds
                                (!HasEffect(Buffs.ArcaneCircle) && gauge.Soul >= 90))) // Correction for 2 min windows
                                return Enshroud;

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Gluttony) &&
                                LevelChecked(Gluttony) && gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                                (GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25 || ActionReady(Gluttony)))
                                return Gluttony;

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Bloodstalk) &&
                                LevelChecked(BloodStalk) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) && gauge.Soul >= 50 &&
                                (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) && (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) >= 10) && !WasLastAction(Gluttony))))
                                return OriginalHook(BloodStalk);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoulSlice) &&
                        !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                        ActionReady(SoulSlice) && gauge.Soul <= 50)
                        return SoulSlice;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.RPR_STSecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.RPR_STBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows))
                    {
                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Communio) &&
                            gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && ActionReady(Communio))
                                return Communio;

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Lemure) &&
                                CanWeave(actionID) && gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice) &&
                                GetCooldownRemainingTime(ArcaneCircle) > 8 && !WasLastAction(ArcaneCircle) && !WasLastAction(ShadowOfDeath))
                                return OriginalHook(BloodStalk);

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_VoidCross) &&
                                HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_VoidCross) &&
                                HasEffect(Buffs.EnhancedCrossReaping))
                                return OriginalHook(Gallows);

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_VoidCross) &&
                                !HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gallows);
                        }

                        if (HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && LevelChecked(Gibbet))
                        {
                            if (HasEffect(Buffs.EnhancedGibbet))
                            {
                                // If we are not on the flank, but need to use gibbet, pop true north if not already up
                                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                    trueNorthReadyDyn && CanWeave(actionID) && !OnTargetsFlank())
                                {
                                    return All.TrueNorth;
                                }
                                return OriginalHook(Gibbet);
                            }

                            if (HasEffect(Buffs.EnhancedGallows))
                            {
                                // If we are not on the rear, but need to use gallows, pop true north if not already up
                                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                    trueNorthReadyDyn && CanWeave(actionID) && !OnTargetsRear())
                                {
                                    return All.TrueNorth;
                                }
                                return OriginalHook(Gallows);
                            }

                            if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows) && HasBattleTarget())
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                    trueNorthReadyDyn && CanWeave(actionID) && !OnTargetsRear())
                                {
                                    return All.TrueNorth;
                                }
                                return Gallows;
                            }
                        }
                    }

                    if (comboTime > 1f)
                    {
                        if (lastComboMove == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                            return OriginalHook(WaxingSlice);

                        if (lastComboMove == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                            return OriginalHook(InfernalSlice);
                    }
                    return OriginalHook(Slice);
                }
                return actionID;
            }
        }

        internal class RPR_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                double enemyHP = GetTargetHPPercent();

                if (actionID is SpinningScythe)
                {
                    if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_WoD) &&
                        LevelChecked(WhorlOfDeath) && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 6 && !HasEffect(Buffs.SoulReaver) && enemyHP > Config.RPR_WoDThreshold)
                        return WhorlOfDeath;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_CDs))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_PlentifulHarvest) &&
                            HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                            (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || WasLastAction(Communio)))
                            return PlentifulHarvest;

                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_ArcaneCircle) && ActionReady(ArcaneCircle))
                                return ArcaneCircle;

                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Enshroud) &&
                                !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && ActionReady(Enshroud) && gauge.Shroud >= 50)
                                return Enshroud;

                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Gluttony) &&
                             LevelChecked(Gluttony) && gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                             ((GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25) || ActionReady(Gluttony)))
                                return Gluttony;

                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_GrimSwathe) &&
                              LevelChecked(GrimSwathe) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) && gauge.Soul >= 50 &&
                              (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) && (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) >= 10))))
                                return GrimSwathe;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_SoulScythe) &&
                        !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                        ActionReady(SoulScythe) && gauge.Soul <= 50)
                        return SoulScythe;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.RPR_AoESecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.RPR_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine))
                    {
                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Communio) &&
                                gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                                return Communio;

                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Lemure) &&
                                gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                                return OriginalHook(GrimSwathe);

                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine_GrimReaping) &&
                                gauge.LemureShroud > 0)
                                return OriginalHook(Guillotine);
                        }

                        if (HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))
                            return Guillotine;
                    }

                    return lastComboMove == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                        ? OriginalHook(NightmareScythe)
                        : OriginalHook(SpinningScythe);
                }
                return actionID;
            }
        }

        internal class RPR_GluttonyBloodSwathe : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_GluttonyBloodSwathe;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (actionID is GrimSwathe)
                {
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud) && HasEffect(Buffs.Enshrouded))
                    {
                        if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                            return Communio;

                        if (gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                            return OriginalHook(GrimSwathe);

                        if (gauge.LemureShroud > 1)
                            return OriginalHook(Guillotine);
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        HasEffect(Buffs.SoulReaver) && LevelChecked(Guillotine))
                        return Guillotine;
                }

                if (actionID is BloodStalk)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorthGluttony) &&
                         trueNorthReady && CanWeave(actionID))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud) && HasEffect(Buffs.Enshrouded))
                    {
                        if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                            return Communio;

                        if (gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice) && CanWeave(actionID))
                            return OriginalHook(BloodStalk);

                        if (HasEffect(Buffs.EnhancedVoidReaping))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedCrossReaping))
                            return OriginalHook(Gallows);

                        if (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))
                            return OriginalHook(Gallows);
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        HasEffect(Buffs.SoulReaver) && LevelChecked(Gibbet))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);

                        if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }
                }
                return actionID;
            }
        }

        internal class RPR_ArcaneCirclePlentifulHarvest : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ArcaneCirclePlentifulHarvest;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArcaneCircle)
                {
                    if (HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest))
                        return PlentifulHarvest;
                }
                return actionID;
            }
        }

        internal class RPR_Regress : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Regress;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return (actionID is HellsEgress or HellsIngress) && FindEffect(Buffs.Threshold)?.RemainingTime <= 9
                    ? Regress
                    : actionID;
            }
        }

        internal class RPR_Soulsow_HarpeHarvestMoon : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Harpe && LevelChecked(HarvestMoon) && HasEffect(Buffs.Soulsow))
                {
                    return (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_EnhancedHarpe) && HasEffect(Buffs.EnhancedHarpe)) ||
                        (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_CombatHarpe) && !InCombat())
                        ? Harpe
                        : HarvestMoon;
                }
                return actionID;
            }
        }

        internal class RPR_Soulsow : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var soulSowOptions = PluginConfiguration.GetCustomBoolArrayValue(Config.RPR_SoulsowOptions);
                bool soulsowReady = LevelChecked(Soulsow) && !HasEffect(Buffs.Soulsow);

                return (((soulSowOptions.Length > 0) && ((actionID is Harpe && soulSowOptions[0]) ||
                    (actionID is Slice && soulSowOptions[1]) ||
                    (actionID is SpinningScythe && soulSowOptions[2]) ||
                    (actionID is ShadowOfDeath && soulSowOptions[3]) ||
                    (actionID is BloodStalk && soulSowOptions[4])) && soulsowReady && !InCombat()) ||
                    (IsEnabled(CustomComboPreset.RPR_Soulsow_Combat) && actionID is Harpe && !HasBattleTarget())) ?
                    Soulsow : actionID;
            }
        }

        internal class RPR_EnshroudProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (actionID is Enshroud)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorthEnshroud) &&
                        GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(Slice))
                        return All.TrueNorth;

                    if (HasEffect(Buffs.SoulReaver))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);

                        if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }
                }

                return actionID;
            }
        }

        internal class RPR_CommunioOnGGG : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_CommunioOnGGG;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();

                if (actionID is Gibbet or Gallows && HasEffect(Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice) && CanWeave(actionID))
                        return OriginalHook(BloodStalk);
                }

                if (actionID is Guillotine && HasEffect(Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                        return OriginalHook(GrimSwathe);
                }

                return actionID;
            }
        }

        internal class RPR_EnshroudCommunio : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudCommunio;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is Enshroud && HasEffect(Buffs.Enshrouded) && LevelChecked(Communio)
                    ? Communio
                    : actionID;
            }
        }
    }
}