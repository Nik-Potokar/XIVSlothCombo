using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
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

        public static class Levels
        {
            public const byte
                WaxingSlice = 5,
                ShadowOfDeath = 10,
                Harpe = 15,
                HellsIngress = 20,
                HellsEgress = 20,
                SpinningScythe = 25,
                InfernalSlice = 30,
                WhorlOfDeath = 35,
                NightmareScythe = 45,
                BloodStalk = 50,
                GrimSwathe = 55,
                SoulSlice = 60,
                SoulScythe = 65,
                SoulReaver = 70,
                Gibbet = 70,
                Gallows = 70,
                Guillotine = 70,
                ArcaneCircle = 72,
                Regress = 74,
                Gluttony = 76,
                Enshroud = 80,
                Soulsow = 82,
                HarvestMoon = 82,
                LemuresScythe = 86,
                LemuresSlice = 86,
                PlentifulHarvest = 88,
                Communio = 90;
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
                var gauge = GetJobGauge<RPRGauge>();
                var enshrouded = HasEffect(Buffs.Enshrouded);
                var soulReaver = HasEffect(Buffs.SoulReaver);
                var deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                var playerHP = PlayerHealthPercentageHp();
                var enemyHP = GetTargetHPPercent();
                var positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                var sodThreshold = PluginConfiguration.GetCustomIntValue(Config.RPR_SoDThreshold);
                var sodRefreshRange = PluginConfiguration.GetCustomIntValue(Config.RPR_SoDRefreshRange);

                //Gibbet and Gallows on SoD
                if (actionID is ShadowOfDeath && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows) && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_OnSoD) && soulReaver && level >= Levels.Gibbet)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth) && CanWeave(actionID))
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
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth) && CanWeave(actionID))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows) && HasEffect(Buffs.SoulReaver) && level >= Levels.Gibbet)
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

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_RangedFiller) && !InMeleeRange() && level >= Levels.Harpe && (CurrentTarget as BattleNpc)?.BattleNpcKind is BattleNpcSubKind.Enemy)
                    {
                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return Communio;

                        if (level >= Levels.HarvestMoon && HasEffect(Buffs.Soulsow))
                        {
                            if ((IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_EnhancedHarpe) && HasEffect(Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_CombatHarpe) && !InCombat()))
                                return Harpe;
                            return HarvestMoon;
                        }

                        return Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Stun) && level >= All.Levels.LegSweep && CanInterruptEnemy() && IsOffCooldown(All.LegSweep))
                        return All.LegSweep;

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD) && level >= Levels.ShadowOfDeath && !soulReaver && enemyHP > sodThreshold)
                    {
                        if ((IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD_Double) && level >= Levels.PlentifulHarvest && enshrouded && GetCooldownRemainingTime(ArcaneCircle) < 9 &&
                            ((gauge.LemureShroud is 4 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 30) || gauge.LemureShroud is 3 && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 50)) || //double shroud windows
                            (GetDebuffRemainingTime(Debuffs.DeathsDesign) <= sodRefreshRange && IsOffCooldown(ArcaneCircle)) || //Opener Condition
                            (GetDebuffRemainingTime(Debuffs.DeathsDesign) <= sodRefreshRange && IsOnCooldown(ArcaneCircle)))  //non 2 minute windows  
                            return ShadowOfDeath;
                    }

                    if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ComboHeals))
                    {
                        if (level >= All.Levels.Bloodbath && playerHP < 65 && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;

                        if (level >= All.Levels.SecondWind && playerHP < 40 && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                    }

                    if (InCombat())
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_ArcaneCircle) && IsOffCooldown(ArcaneCircle) && CanWeave(actionID) && level >= Levels.ArcaneCircle)
                            return ArcaneCircle;

                        if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_PlentifulHarvest) && HasEffect(Buffs.ImmortalSacrifice) && GetBuffRemainingTime(Buffs.ImmortalSacrifice) < 26 && !soulReaver && !enshrouded && level >= Levels.PlentifulHarvest)
                            return PlentifulHarvest;
                    }

                    if (HasBattleTarget() && (deathsDesign || (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoD) && enemyHP < sodThreshold)))
                    {
                        if (!soulReaver && IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_Enshroud))
                        {
                            if (!enshrouded && level >= Levels.Enshroud && IsOffCooldown(Enshroud) && CanWeave(actionID))
                            {
                                if (IsNotEnabled(CustomComboPreset.RPR_ST_SliceCombo_EnshroudPooling) && gauge.Shroud >= 50)
                                    return Enshroud;

                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_EnshroudPooling) &&
                                    ((level < Levels.PlentifulHarvest && gauge.Shroud >= 50) || // Pre Plentiful Harvest
                                    (HasEffect(Buffs.ArcaneCircle) && gauge.Shroud >= 50) || //Shroud in Arcane Circle
                                    (gauge.Shroud >= 50 && GetCooldownRemainingTime(ArcaneCircle) < 8) || //Prep for double shroud
                                    (!HasEffect(Buffs.ArcaneCircle) && gauge.Shroud >= 90))) //Shroud pooling
                                    return Enshroud;
                            }
                        }

                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Communio) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                                    return Communio;

                                if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GibbetGallows_Lemure) && gauge.VoidShroud >= 2 && level >= Levels.LemuresSlice)
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
                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_GluttonyBloodStalk) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && level >= Levels.BloodStalk)
                            {
                                if (gauge.Soul >= 50 && IsOffCooldown(Gluttony) && level >= Levels.Gluttony)
                                    return Gluttony;

                                return OriginalHook(BloodStalk);
                            }

                            if (IsEnabled(CustomComboPreset.RPR_ST_SliceCombo_SoulSlice) && !enshrouded && !soulReaver && level >= Levels.SoulSlice && gauge.Soul <= 50 && GetRemainingCharges(SoulSlice) > 0)
                                return SoulSlice;
                        }
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Slice && level >= Levels.WaxingSlice)
                            return WaxingSlice;

                        if (lastComboMove is WaxingSlice && level >= Levels.InfernalSlice)
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
                    var gauge = GetJobGauge<RPRGauge>();
                    var enshrouded = HasEffect(Buffs.Enshrouded);
                    var soulReaver = HasEffect(Buffs.SoulReaver);
                    var deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                    var enemyHP = GetTargetHPPercent();

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine) && soulReaver && level >= Levels.Guillotine)
                        return OriginalHook(Guillotine);

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_WoD) && GetDebuffRemainingTime(Debuffs.DeathsDesign) < 3 && !soulReaver && enemyHP > 5 && level >= Levels.WhorlOfDeath)
                        return WhorlOfDeath;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_ArcaneCircle) && InCombat())
                    {
                        if (IsOffCooldown(ArcaneCircle) && CanWeave(actionID) && level >= Levels.ArcaneCircle)
                            return ArcaneCircle;

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_PlentifulHarvest) && HasEffect(Buffs.ImmortalSacrifice) && !HasEffect(Buffs.BloodsownCircle) && !soulReaver && !enshrouded && level >= Levels.PlentifulHarvest)
                            return PlentifulHarvest;
                    }

                    if (deathsDesign)
                    {
                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Enshroud) && !enshrouded && !soulReaver && level >= Levels.Enshroud && IsOffCooldown(Enshroud) && CanWeave(actionID) && gauge.Shroud >= 50)
                            return Enshroud;

                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Guillotine))
                            {
                                if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Communio) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                                    return Communio;

                                if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_Lemure) && gauge.VoidShroud >= 2 && level >= Levels.LemuresScythe)
                                    return OriginalHook(GrimSwathe);

                                if (gauge.LemureShroud > 0)
                                    return OriginalHook(Guillotine);
                            }
                        }

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_GluttonyGrimSwathe) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && level >= Levels.GrimSwathe)
                        {
                            if (gauge.Soul >= 50 && IsOffCooldown(Gluttony) && level >= Levels.Gluttony)
                                return Gluttony;

                            return GrimSwathe;
                        }

                        if (IsEnabled(CustomComboPreset.RPR_AoE_ScytheCombo_SoulScythe) && !enshrouded && !soulReaver && level >= Levels.SoulScythe && gauge.Soul <= 50 && GetRemainingCharges(SoulScythe) > 0 && (comboTime == 0 || comboTime > 15))
                            return SoulScythe;
                    }

                    if (lastComboMove is SpinningScythe && level >= Levels.NightmareScythe)
                        return OriginalHook(NightmareScythe);

                    return SpinningScythe;
                }

                return actionID;
            }
        }

        internal class RPR_GluttonyBloodSwathe : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_GluttonyBloodSwathe;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                var gauge = GetJobGauge<RPRGauge>();

                if (actionID is GrimSwathe)
                {
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud) && HasEffect(Buffs.Enshrouded))
                    {
                        if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= Levels.Communio)
                            return Communio;

                        if (gauge.VoidShroud >= 2 && level >= Levels.LemuresScythe)
                            return OriginalHook(GrimSwathe);

                        if (gauge.LemureShroud > 1)
                            return OriginalHook(Guillotine);
                    }

                    if (IsOffCooldown(Gluttony) && level >= Levels.Gluttony && !HasEffect(Buffs.Enshrouded))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) && HasEffect(Buffs.SoulReaver) && level >= Levels.Guillotine)
                        return Guillotine;
                }

                if (actionID is BloodStalk)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth) && CanWeave(Slice))
                        return All.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud) && HasEffect(Buffs.Enshrouded))
                    {
                        if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= Levels.Communio)
                            return Communio;

                        if (gauge.VoidShroud >= 2 && level >= Levels.LemuresSlice)
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

                    if (IsOffCooldown(Gluttony) && level >= Levels.Gluttony && !HasEffect(Buffs.Enshrouded))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) && HasEffect(Buffs.SoulReaver) && level >= Levels.Gibbet)
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
                    if (HasEffect(Buffs.ImmortalSacrifice) && level >= Levels.PlentifulHarvest)
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
                if ((actionID is HellsEgress or HellsIngress) && HasEffect(Buffs.Threshold))
                    return Regress;

                return actionID;
            }
        }

        internal class RPR_Soulsow_HarpeHarvestMoon : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Harpe && level >= Levels.HarvestMoon && HasEffect(Buffs.Soulsow))
                {
                    if ((IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_EnhancedHarpe) && HasEffect(Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.RPR_Soulsow_HarpeHarvestMoon_CombatHarpe) && !InCombat()))
                        return Harpe;

                    return HarvestMoon;
                }

                return actionID;
            }
        }

        internal class RPR_Soulsow : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Harpe or Slice or SpinningScythe or ShadowOfDeath or BloodStalk)
                {
                    if (level >= Levels.Soulsow && !HasEffect(Buffs.Soulsow) && !InCombat())
                        return Soulsow;
                }

                return actionID;
            }
        }

        internal class RPR_EnshroudProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var positionalChoice = PluginConfiguration.GetCustomIntValue(Config.RPR_PositionalChoice);
                if (actionID is Enshroud)
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorth) && GetBuffStacks(Buffs.SoulReaver) is 2 && GetRemainingCharges(All.TrueNorth) > 0 && !HasEffect(All.Buffs.TrueNorth) && CanWeave(Slice))
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
                var gauge = GetJobGauge<RPRGauge>();

                if (actionID is Gibbet or Gallows && HasEffect(Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= Levels.Communio)
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) && gauge.VoidShroud >= 2 && level >= Levels.LemuresSlice)
                        return OriginalHook(BloodStalk);
                }

                if (actionID is Guillotine && HasEffect(Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= Levels.Communio)
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) && gauge.VoidShroud >= 2 && level >= Levels.LemuresScythe)
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
                if (actionID is Enshroud && HasEffect(Buffs.Enshrouded) && level >= Levels.Communio)
                    return Communio;

                return actionID;
            }
        }
    }    
}