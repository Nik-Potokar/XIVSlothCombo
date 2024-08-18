using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.JobHelpers.RPR;

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
            ExecutionersGibbet = 36970,
            ExecutionersGallows = 36971,
            ExecutionersGuillotine = 36972,

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
            Sacrificium = 36969,
            Perfectio = 36973,
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
                BloodsownCircle = 2972,
                IdealHost = 3905,
                Oblatio = 3857,
                Executioner = 3858,
                PerfectioParata = 3860;
        }

        public static class Debuffs
        {
            public const ushort
                DeathsDesign = 2586;
        }

        public static class Config
        {
            public static UserInt
                RPR_SoDThreshold = new("RPRSoDThreshold", 1),
                RPR_WoDThreshold = new("RPRWoDThreshold", 1),
                RPR_SoDRefreshRange = new("RPRSoDRefreshRange", 8),
                RPR_Positional = new("RPR_Positional", 0),
                RPR_VariantCure = new("RPRVariantCure"),
                RPR_STSecondWindThreshold = new("RPR_STSecondWindThreshold", 25),
                RPR_STBloodbathThreshold = new("RPR_STBloodbathThreshold", 40),
                RPR_AoESecondWindThreshold = new("RPR_AoESecondWindThreshold", 25),
                RPR_AoEBloodbathThreshold = new("RPR_AoEBloodbathThreshold", 40);
            public static UserBoolArray
               RPR_SoulsowOptions = new("RPR_SoulsowOptions");
            public static UserBool
               RPR_ST_TrueNorth_Moving = new("RPR_ST_TrueNorth_Moving");

        }

        internal class RPR_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_SimpleMode;
            internal static RPROpenerLogic RPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                double enemyHP = GetTargetHPPercent();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                bool trueNorthDynReady = trueNorthReady;
                float GCD = GetCooldown(Slice).CooldownTotal;

                if (actionID is Slice)
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

                    if (RPROpener.DoFullOpener(ref actionID))
                        return actionID;

                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        if (ActionReady(ArcaneCircle))
                            return ArcaneCircle;

                        if (RPRHelpers.UseEnshroud(gauge))
                            return Enshroud;

                        if (!HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                            !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.ImmortalSacrifice) &&
                            !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.PerfectioParata) && !RPRHelpers.IsComboExpiring(2) &&
                            gauge.Soul >= 50)
                        {
                            if (!JustUsed(Perfectio) && ActionReady(Gluttony))
                            {
                                if (trueNorthReady)
                                    return All.TrueNorth;

                                return Gluttony;
                            }

                            if (LevelChecked(BloodStalk) &&
                                (!LevelChecked(Gluttony) ||
                                (LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                                (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5))))
                                return OriginalHook(BloodStalk);
                        }

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (gauge.LemureShroud is 2 && gauge.VoidShroud is 1 &&
                                HasEffect(Buffs.Oblatio) && LevelChecked(Sacrificium))
                                return OriginalHook(Gluttony);

                            if (gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);
                        }
                    }

                    if (!InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget())
                    {
                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                            return Communio;

                        return (HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon))
                                ? HarvestMoon
                                : Harpe;
                    }

                    if (RPRHelpers.UseShadowOfDeath() && enemyHP > Config.RPR_SoDThreshold)
                        return ShadowOfDeath;

                    if (TargetHasEffect(Debuffs.DeathsDesign))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio) && !RPRHelpers.IsComboExpiring(1))
                            return OriginalHook(Communio);

                        if (LevelChecked(Gibbet) && !HasEffect(Buffs.Enshrouded) &&
                           (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)))
                        {
                            if (HasEffect(Buffs.EnhancedGibbet))
                            {
                                if (trueNorthDynReady && !OnTargetsFlank())
                                    return All.TrueNorth;

                                return OriginalHook(Gibbet);
                            }

                            if (HasEffect(Buffs.EnhancedGallows) ||
                                (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                            {
                                if (trueNorthDynReady && !OnTargetsRear())
                                    return All.TrueNorth;

                                return OriginalHook(Gallows);
                            }
                        }

                        if (LevelChecked(PlentifulHarvest) &&
                            !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                            !HasEffect(Buffs.Executioner) && HasEffect(Buffs.ImmortalSacrifice) &&
                            (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                            return PlentifulHarvest;

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                                return Communio;

                            if (HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (HasEffect(Buffs.EnhancedCrossReaping) ||
                                (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping)))
                                return OriginalHook(Gallows);
                        }

                        if (!HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.IdealHost) &&
                            !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.ImmortalSacrifice) &&
                            !RPRHelpers.IsComboExpiring(2) && gauge.Soul <= 50 && ActionReady(SoulSlice))
                            return SoulSlice;
                    }

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;

                    if (comboTime > 0)
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

        internal class RPR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_AdvancedMode;
            internal static RPROpenerLogic RPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                double enemyHP = GetTargetHPPercent();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                bool trueNorthDynReady = trueNorthReady;
                int PositionalChoice = Config.RPR_Positional;
                float GCD = GetCooldown(Slice).CooldownTotal;

                // Prevent the dynamic true north option from using the last charge
                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                    IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                    GetRemainingCharges(All.TrueNorth) < 2 && trueNorthReady)
                    trueNorthDynReady = false;

                if (actionID is Slice)
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

                    if (IsEnabled(CustomComboPreset.RPR_ST_Opener))
                    {
                        if (RPROpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_CDs))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_ArcaneCircle) &&
                                ActionReady(ArcaneCircle))
                                return ArcaneCircle;

                            if (IsEnabled(CustomComboPreset.RPR_ST_Enshroud) &&
                                RPRHelpers.UseEnshroud(gauge))
                                return Enshroud;

                            if (!HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                                !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.ImmortalSacrifice) &&
                                !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.PerfectioParata) && !RPRHelpers.IsComboExpiring(2) &&
                                gauge.Soul >= 50)
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_Gluttony) &&
                                    !JustUsed(Perfectio) && ActionReady(Gluttony))
                                {
                                    if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                        trueNorthReady)
                                        return All.TrueNorth;

                                    return Gluttony;
                                }

                                if (IsEnabled(CustomComboPreset.RPR_ST_Bloodstalk) &&
                                    LevelChecked(BloodStalk) &&
                                    (!LevelChecked(Gluttony) ||
                                    (LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                                    (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5))))
                                    return OriginalHook(BloodStalk);
                            }
                        }

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_Sacrificium) &&
                                 gauge.LemureShroud is 2 && gauge.VoidShroud is 1 &&
                                HasEffect(Buffs.Oblatio) && LevelChecked(Sacrificium))
                                return OriginalHook(Gluttony);

                            if (IsEnabled(CustomComboPreset.RPR_ST_Lemure) &&
                                gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_RangedFiller) &&
                       !InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget())
                    {
                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                            return Communio;

                        return (IsEnabled(CustomComboPreset.RPR_ST_RangedFillerHarvestMoon) &&
                            HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon))
                                ? HarvestMoon
                                : Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SoD) &&
                         RPRHelpers.UseShadowOfDeath() && enemyHP > Config.RPR_SoDThreshold)
                        return ShadowOfDeath;

                    if (TargetHasEffect(Debuffs.DeathsDesign))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_Perfectio) &&
                            HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio) && !RPRHelpers.IsComboExpiring(1))
                            return OriginalHook(Communio);

                        if (IsEnabled(CustomComboPreset.RPR_ST_GibbetGallows) &&
                            LevelChecked(Gibbet) && !HasEffect(Buffs.Enshrouded) &&
                           (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)))
                        {
                            if (HasEffect(Buffs.EnhancedGibbet) ||
                                (PositionalChoice is 1 && !HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsFlank())
                                    return All.TrueNorth;

                                return OriginalHook(Gibbet);
                            }

                            if (HasEffect(Buffs.EnhancedGallows) ||
                                 (PositionalChoice is 0 && !HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                    trueNorthDynReady && !OnTargetsRear())
                                    return All.TrueNorth;

                                return OriginalHook(Gallows);
                            }
                        }

                        if (IsEnabled(CustomComboPreset.RPR_ST_CDs) &&
                            IsEnabled(CustomComboPreset.RPR_ST_PlentifulHarvest) &&
                            LevelChecked(PlentifulHarvest) &&
                            !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                            !HasEffect(Buffs.Executioner) && HasEffect(Buffs.ImmortalSacrifice) &&
                            (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                            return PlentifulHarvest;

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_Communio) &&
                                gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                                return Communio;

                            if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                                HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                                (HasEffect(Buffs.EnhancedCrossReaping) ||
                                (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))))
                                return OriginalHook(Gallows);
                        }

                        if (IsEnabled(CustomComboPreset.RPR_ST_SoulSlice) &&
                            !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.IdealHost) &&
                            !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.ImmortalSacrifice) &&
                            !RPRHelpers.IsComboExpiring(2) && gauge.Soul <= 50 && ActionReady(SoulSlice))
                            return SoulSlice;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.RPR_STSecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.RPR_STBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
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

        internal class RPR_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                float GCD = GetCooldown(SpinningScythe).CooldownTotal;

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

                    if (LevelChecked(WhorlOfDeath) &&
                        GetDebuffRemainingTime(Debuffs.DeathsDesign) < 6 && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner))
                        return WhorlOfDeath;

                    if (TargetHasEffect(Debuffs.DeathsDesign))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.Executioner) &&
                            (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                            return PlentifulHarvest;

                        if (CanWeave(actionID))
                        {
                            if (LevelChecked(ArcaneCircle) &&
                                ((GetCooldownRemainingTime(ArcaneCircle) <= GCD + 0.25) || ActionReady(ArcaneCircle)))
                                return ArcaneCircle;

                            if (!HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.Executioner) &&
                                ActionReady(Enshroud) && (gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) && !RPRHelpers.IsComboExpiring(6))
                                return Enshroud;

                            if (LevelChecked(Gluttony) && gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) &&
                             !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                             ((GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25) || ActionReady(Gluttony)))
                                return Gluttony;

                            if (LevelChecked(GrimSwathe) && !HasEffect(Buffs.Enshrouded) &&
                              !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) && !HasEffect(Buffs.Executioner) &&
                              gauge.Soul >= 50 && (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) &&
                              (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5))))
                                return GrimSwathe;
                        }

                        if (!HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) &&
                            ActionReady(SoulScythe) && gauge.Soul <= 50)
                            return SoulScythe;

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && ActionReady(Communio))
                                return Communio;

                            if (gauge.LemureShroud is 2 && gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            if (gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                                return OriginalHook(GrimSwathe);

                            if (gauge.LemureShroud > 0)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (HasEffect(Buffs.SoulReaver) || (HasEffect(Buffs.Executioner)
                        && !HasEffect(Buffs.Enshrouded) && LevelChecked(Guillotine)))
                        return OriginalHook(Guillotine);

                    return lastComboMove == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                        ? OriginalHook(NightmareScythe)
                        : OriginalHook(SpinningScythe);
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
                float GCD = GetCooldown(SpinningScythe).CooldownTotal;

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

                    if (IsEnabled(CustomComboPreset.RPR_AoE_WoD) &&
                        LevelChecked(WhorlOfDeath) &&
                        GetDebuffRemainingTime(Debuffs.DeathsDesign) < 6 && !HasEffect(Buffs.SoulReaver) &&
                        enemyHP > Config.RPR_WoDThreshold)
                        return WhorlOfDeath;

                    if (TargetHasEffect(Debuffs.DeathsDesign))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_AoE_Perfectio) &&
                            HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (IsEnabled(CustomComboPreset.RPR_AoE_CDs))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_PlentifulHarvest) &&
                                HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                                !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                                (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                                return PlentifulHarvest;

                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_AoE_ArcaneCircle) &&
                                   LevelChecked(ArcaneCircle) &&
                                   ((GetCooldownRemainingTime(ArcaneCircle) <= GCD + 0.25) || ActionReady(ArcaneCircle)))
                                    return ArcaneCircle;

                                if (IsEnabled(CustomComboPreset.RPR_AoE_Enshroud) &&
                                    !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                                    ActionReady(Enshroud) && (gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) && !RPRHelpers.IsComboExpiring(6))
                                    return Enshroud;

                                if (IsEnabled(CustomComboPreset.RPR_AoE_Gluttony) &&
                                 LevelChecked(Gluttony) && gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) &&
                                 !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                                 ((GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25) || ActionReady(Gluttony)))
                                    return Gluttony;

                                if (IsEnabled(CustomComboPreset.RPR_AoE_GrimSwathe) &&
                                  LevelChecked(GrimSwathe) && !HasEffect(Buffs.Enshrouded) &&
                                  !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) && gauge.Soul >= 50 &&
                                  (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) &&
                                  (gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5))))
                                    return GrimSwathe;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.RPR_AoE_SoulScythe) &&
                            !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) &&
                            ActionReady(SoulScythe) && gauge.Soul <= 50)
                            return SoulScythe;

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_Communio) &&
                               gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && ActionReady(Communio))
                                return Communio;

                            if (IsEnabled(CustomComboPreset.RPR_AoE_Sacrificium) &&
                                gauge.LemureShroud is 2 && gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio) && CanWeave(actionID))
                                return OriginalHook(Gluttony);

                            if (IsEnabled(CustomComboPreset.RPR_AoE_Lemure) &&
                                gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                                return OriginalHook(GrimSwathe);

                            if (IsEnabled(CustomComboPreset.RPR_AoE_Reaping) &&
                                gauge.LemureShroud > 0)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.RPR_AoESecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.RPR_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_AoE_Guillotine) &&
                        (HasEffect(Buffs.SoulReaver) || (HasEffect(Buffs.Executioner)
                        && !HasEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))))
                        return OriginalHook(Guillotine);

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
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                                return Communio;

                            if (gauge.LemureShroud is 2 && gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            if (gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);

                            if (gauge.LemureShroud > 1)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)) && LevelChecked(Guillotine))
                        return Guillotine;
                }

                if (actionID is BloodStalk)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorthGluttony) &&
                         trueNorthReady && CanWeave(actionID))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                                return Communio;

                            if (gauge.LemureShroud is 2 && gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            if (gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);

                            if (HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (HasEffect(Buffs.EnhancedCrossReaping))
                                return OriginalHook(Gallows);

                            if (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gallows);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)) && LevelChecked(Gibbet))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows) ||
                            (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
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
                        GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanDelayedWeave(Slice))
                        return All.TrueNorth;

                    if (HasEffect(Buffs.SoulReaver))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows) ||
                            (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
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
                if (actionID is Enshroud)
                {
                    if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                        return OriginalHook(Communio);

                    if (HasEffect(Buffs.Enshrouded) && LevelChecked(Communio))
                        return Communio;
                }
                return actionID;
            }
        }
    }
}