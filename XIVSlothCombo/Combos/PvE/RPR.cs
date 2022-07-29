using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class RPR
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
            public const string
                RPR_PositionalChoice = "RPRPositionChoice",
                RPR_SoDThreshold = "RPRSoDThreshold",
                RPR_SoDRefreshRange = "RPRSoDRefreshRange";
        }

        internal class RPR_ST_SliceCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_SliceCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                bool enshrouded = HasEffect(Buffs.Enshrouded);
                bool soulReaver = HasEffect(Buffs.SoulReaver);
                bool deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                double playerHP = PlayerHealthPercentageHp();
                double enemyHP = GetTargetHPPercent();
                int positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                int sodThreshold = PluginConfiguration.GetCustomIntValue(Config.RPR_SoDThreshold);
                int sodRefreshRange = PluginConfiguration.GetCustomIntValue(Config.RPR_SoDRefreshRange);
                bool trueNorthReady = GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth);

                // Gibbet and Gallows on Shadow of Death
                if (actionID is ShadowOfDeath && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows) && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_OnSoD) && soulReaver && LevelChecked(Gibbet))
                {
                    // True North overcap use
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(actionID))
                        return All.TrueNorth;

                    if (positionalChoice is 0 or 1 or 2)
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);
                        if (HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }

                    if (positionalChoice == 3)
                        return OriginalHook(Gibbet);
                    if (positionalChoice == 4)
                        return OriginalHook(Gallows);

                    if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                    {
                        if (positionalChoice is 0 or 1)
                            return Gallows;
                        if (positionalChoice == 2)
                            return Gibbet;
                    }
                }

                if (actionID is Slice)
                {
                    bool interruptReady = LevelChecked(All.LegSweep) && CanInterruptEnemy() && IsOffCooldown(All.LegSweep);

                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(actionID))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows) && HasEffect(Buffs.SoulReaver) && LevelChecked(Gibbet))
                    {
                        if (positionalChoice is 0 or 1 or 2)
                        {
                            if (HasEffect(Buffs.EnhancedGibbet))
                                return OriginalHook(Gibbet);
                            if (HasEffect(Buffs.EnhancedGallows))
                                return OriginalHook(Gallows);
                        }

                        if (positionalChoice == 3)
                            return OriginalHook(Gallows);
                        if (positionalChoice == 4)
                            return OriginalHook(Gibbet);

                        if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                        {
                            if (positionalChoice is 0 or 1)
                                return Gallows;
                            if (positionalChoice == 2)
                                return Gibbet;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_RangedFiller) && !InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget())
                    {
                        bool harvestMoonReady = LevelChecked(HarvestMoon) && HasEffect(Buffs.Soulsow);

                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                            return Communio;

                        if (harvestMoonReady)
                        {
                            return (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_EnhancedHarpe) && HasEffect(Buffs.EnhancedHarpe)) ||
                                (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_CombatHarpe) && !InCombat())
                                ? Harpe
                                : HarvestMoon;
                        }
                        return Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Stun) && interruptReady)
                        return All.LegSweep;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD) && LevelChecked(ShadowOfDeath) && !soulReaver && enemyHP > sodThreshold)
                    {
                        if ((IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD_Double) && LevelChecked(PlentifulHarvest) && enshrouded && GetCooldownRemainingTime(ArcaneCircle) < 9 &&
                            ((gauge.LemureShroud is 4 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 30) || (gauge.LemureShroud is 3 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 50))) ||    // Double Enshroud windows
                            (GetDebuffRemainingTime(Debuffs.DeathsDesign) <= sodRefreshRange && IsOffCooldown(ArcaneCircle)) ||                                                                     // Opener condition
                            (GetDebuffRemainingTime(Debuffs.DeathsDesign) <= sodRefreshRange && IsOnCooldown(ArcaneCircle)))                                                                        // Non-2-minute windows  
                            return ShadowOfDeath;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ComboHeals))
                    {
                        bool bloodbathReady = LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath);
                        bool secondWindReady = LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind);

                        if (bloodbathReady && playerHP < 65)
                            return All.Bloodbath;
                        if (secondWindReady && playerHP < 40)
                            return All.SecondWind;
                    }

                    if (InCombat())
                    {
                        bool arcaneReady = LevelChecked(ArcaneCircle) && IsOffCooldown(ArcaneCircle);
                        bool plentifulReady = LevelChecked(PlentifulHarvest) && HasEffect(Buffs.ImmortalSacrifice);

                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ArcaneCircle) && CanWeave(actionID) && arcaneReady)
                            return ArcaneCircle;
                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_PlentifulHarvest) && plentifulReady && GetBuffRemainingTime(Buffs.ImmortalSacrifice) < 26 && !soulReaver && !enshrouded)
                            return PlentifulHarvest;
                    }

                    if (HasBattleTarget() && (deathsDesign || (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD) && enemyHP < sodThreshold)))
                    {
                        if (!soulReaver && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Enshroud))
                        {
                            if (!enshrouded && LevelChecked(Enshroud) && IsOffCooldown(Enshroud) && CanWeave(actionID))
                            {
                                if (IsNotEnabled(CustomComboPreset.RPR_ST_SliceCombo_EnshroudPooling) && gauge.Shroud >= 50)
                                    return Enshroud;

                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_EnshroudPooling) &&
                                    ((!LevelChecked(PlentifulHarvest) && gauge.Shroud >= 50) ||             // Before Plentiful Harvest
                                    (HasEffect(Buffs.ArcaneCircle) && gauge.Shroud >= 50) ||                // Shroud in Arcane Circle
                                    (gauge.Shroud >= 50 && GetCooldownRemainingTime(ArcaneCircle) < 8) ||   // Prep for double Enshroud
                                    (!HasEffect(Buffs.ArcaneCircle) && gauge.Shroud >= 90)))                // Shroud pooling
                                    return Enshroud;
                            }
                        }

                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Communio) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                                    return Communio;
                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Lemure) && gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                    return OriginalHook(BloodStalk);
                                if (HasEffect(Buffs.EnhancedVoidReaping))
                                    return OriginalHook(Gibbet);
                                if (HasEffect(Buffs.EnhancedCrossReaping))
                                    return OriginalHook(Gallows);

                                if (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))
                                {
                                    if (positionalChoice is 0 or 1 or 3)
                                        return OriginalHook(Gallows);
                                    if (positionalChoice is 2 or 4)
                                        return OriginalHook(Gibbet);
                                }
                            }
                        }

                        if (!(comboTime > 0) || lastComboMove is InfernalSlice || comboTime > 10)
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GluttonyBloodStalk) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && LevelChecked(BloodStalk))
                            {
                                return gauge.Soul >= 50 && IsOffCooldown(Gluttony) && LevelChecked(Gluttony)
                                    ? Gluttony
                                    : OriginalHook(BloodStalk);
                            }

                            bool soulSliceReady = LevelChecked(SoulSlice) && GetRemainingCharges(SoulSlice) > 0;

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoulSlice) && !enshrouded && !soulReaver && gauge.Soul <= 50 && soulSliceReady)
                                return SoulSlice;
                        }
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Slice && LevelChecked(WaxingSlice))
                            return WaxingSlice;
                        if (lastComboMove is WaxingSlice && LevelChecked(InfernalSlice))
                            return InfernalSlice;
                    }
                    return Slice;
                }
                return actionID;
            }
        }

        internal class RPR_AoE_ScytheCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_ScytheCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpinningScythe)
                {
                    RPRGauge? gauge = GetJobGauge<RPRGauge>();
                    bool enshrouded = HasEffect(Buffs.Enshrouded);
                    bool soulReaver = HasEffect(Buffs.SoulReaver);
                    bool deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                    double enemyHP = GetTargetHPPercent();

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine) && soulReaver && LevelChecked(Guillotine))
                        return OriginalHook(Guillotine);
                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_WoD) && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 3 && !soulReaver && enemyHP > 5 && LevelChecked(WhorlOfDeath))
                        return WhorlOfDeath;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_ArcaneCircle) && InCombat())
                    {
                        bool plentifulReady = LevelChecked(PlentifulHarvest) && HasEffect(Buffs.ImmortalSacrifice);

                        if (IsOffCooldown(ArcaneCircle) && CanWeave(actionID) && LevelChecked(ArcaneCircle))
                            return ArcaneCircle;
                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_PlentifulHarvest) && !HasEffect(Buffs.BloodsownCircle) && !soulReaver && !enshrouded && plentifulReady)
                            return PlentifulHarvest;
                    }

                    if (deathsDesign)
                    {
                        bool enshroudReady = LevelChecked(Enshroud) && IsOffCooldown(Enshroud);
                        bool soulScytheReady = LevelChecked(SoulScythe) && GetRemainingCharges(SoulScythe) > 0;

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Enshroud) && !enshrouded && !soulReaver && enshroudReady && CanWeave(actionID) && gauge.Shroud >= 50)
                            return Enshroud;

                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Communio) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && LevelChecked(Communio))
                                    return Communio;
                                if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Lemure) && gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                                    return OriginalHook(GrimSwathe);
                                if (gauge.LemureShroud > 0)
                                    return OriginalHook(Guillotine);
                            }
                        }

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_GluttonyGrimSwathe) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && LevelChecked(GrimSwathe))
                        {
                            return gauge.Soul >= 50 && IsOffCooldown(Gluttony) && LevelChecked(Gluttony)
                                ? Gluttony
                                : GrimSwathe;
                        }

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_SoulScythe) && !enshrouded && !soulReaver && gauge.Soul <= 50 && soulScytheReady && (comboTime == 0 || comboTime > 15))
                            return SoulScythe;
                    }

                    return lastComboMove is SpinningScythe && LevelChecked(NightmareScythe)
                        ? OriginalHook(NightmareScythe)
                        : SpinningScythe;
                }
                return actionID;
            }
        }

        internal class RPR_GluttonyBloodSwathe : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_GluttonyBloodSwathe;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                RPRGauge? gauge = GetJobGauge<RPRGauge>();
                bool gluttonyReady = LevelChecked(Gluttony) && IsOffCooldown(Gluttony);

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

                    if (gluttonyReady && !HasEffect(Buffs.Enshrouded))
                        return Gluttony;
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) && HasEffect(Buffs.SoulReaver) && LevelChecked(Guillotine))
                        return Guillotine;
                }

                if (actionID is BloodStalk)
                {
                    bool trueNorthReady = GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth);

                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(Slice))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud) && HasEffect(Buffs.Enshrouded))
                    {
                        if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                            return Communio;
                        if (gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                            return OriginalHook(BloodStalk);
                        if (HasEffect(Buffs.EnhancedVoidReaping))
                            return OriginalHook(Gibbet);
                        if (HasEffect(Buffs.EnhancedCrossReaping))
                            return OriginalHook(Gallows);

                        if (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))
                        {
                            if (positionalChoice is 0 or 1 or 3)
                                return OriginalHook(Gallows);
                            if (positionalChoice is 2 or 4)
                                return OriginalHook(Gibbet);
                        }
                    }

                    if (gluttonyReady && !HasEffect(Buffs.Enshrouded))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) && HasEffect(Buffs.SoulReaver) && LevelChecked(Gibbet))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);
                        if (HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);

                        if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                        {
                            if (positionalChoice is 0 or 1 or 3)
                                return OriginalHook(Gallows);
                            if (positionalChoice is 2 or 4)
                                return OriginalHook(Gibbet);
                        }
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
                return (actionID is HellsEgress or HellsIngress) && HasEffect(Buffs.Threshold)
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
                bool soulsowReady = LevelChecked(Soulsow) && !HasEffect(Buffs.Soulsow);

                return actionID is Harpe or Slice or SpinningScythe or ShadowOfDeath or BloodStalk &&
                    soulsowReady && !InCombat()
                    ? Soulsow
                    : actionID;
            }
        }

        internal class RPR_EnshroudProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                if (actionID is Enshroud)
                {
                    bool trueNorthReady = GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth);

                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanWeave(Slice))
                        return All.TrueNorth;

                    if (HasEffect(Buffs.SoulReaver))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);
                        if (HasEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                        if (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows))
                        {
                            if (positionalChoice is 0 or 1 or 3)
                                return OriginalHook(Gallows);
                            if (positionalChoice is 2 or 4)
                                return OriginalHook(Gibbet);
                        }
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
                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) && gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }

                if (actionID is Guillotine && HasEffect(Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && LevelChecked(Communio))
                        return Communio;
                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) && gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
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