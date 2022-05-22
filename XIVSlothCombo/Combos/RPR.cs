using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
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
                EnhancedGibbet = 2588,
                EnhancedGallows = 2589,
                EnhancedVoidReaping = 2590,
                EnhancedCrossReaping = 2591,
                EnhancedHarpe = 2859,
                Enshrouded = 2593,
                Soulsow = 2594,
                Threshold = 2595;
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
                HellsIngress = 20,
                HellsEgress = 20,
                SpinningScythe = 25,
                InfernalSlice = 30,
                WhorlOfDeath = 35,
                NightmareScythe = 45,
                SoulSlice = 60,
                SoulScythe = 65,
                SoulReaver = 70,
                Regress = 74,
                Gluttony = 76,
                Enshroud = 80,
                Soulsow = 82,
                HarvestMoon = 82,
                PlentifulHarvest = 88,
                Communio = 90;

        }


        internal class ReaperComboCommunioFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperComboCommunioFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Gibbet or Gallows or Guillotine)
                {
                    var gauge = GetJobGauge<RPRGauge>();

                    if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && (gauge.VoidShroud is 0 || !IsEnabled(CustomComboPreset.ReaperLemureFeature)) && level >= Levels.Communio)
                        return Communio;
                }

                return actionID;
            }
        }

        internal class ReaperLemureFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperLemureFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Gibbet or Gallows or Guillotine)
                {
                    var gauge = GetJobGauge<RPRGauge>();

                    if (HasEffect(Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                    {
                        if (actionID is RPR.Guillotine)
                            return OriginalHook(GrimSwathe);
                        return OriginalHook(BloodStalk);
                    }
                }

                return actionID;
            }
        }

        internal class ReaperSliceCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSliceCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.Slice)
                {
                    var gauge = GetJobGauge<RPRGauge>();
                    var enhancedHarpe = HasEffect(Buffs.EnhancedHarpe);
                    var enshrouded = HasEffect(Buffs.Enshrouded);
                    var enshroudedTimer = FindEffect(Buffs.Enshrouded);
                    var soulScytheReady = IsOffCooldown(SoulScythe);
                    var soulReaver = HasEffect(Buffs.SoulReaver);
                    var deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                    var deathsDesignTimer = FindTargetEffect(Debuffs.DeathsDesign);
                    var soulsow = HasEffect(Buffs.Soulsow);
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var playerHP = PlayerHealthPercentageHp();
                    var enemyHP = EnemyHealthPercentage();
                    var distance = GetTargetDistance();

                    if (IsEnabled(CustomComboPreset.ReaperRangedFillerOption) && !InMeleeRange())
                    {
                        if (HasEffect(Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return Communio;

                        if (level >= Levels.HarvestMoon && soulsow)
                        {
                            if ((IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && enhancedHarpe) || (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !inCombat))
                                return Harpe;

                            return HarvestMoon;
                        }

                        return Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperStunOption) && level >= All.Levels.LegSweep && CanInterruptEnemy() && IsOffCooldown(All.LegSweep))
                        return All.LegSweep;

                    if (IsEnabled(CustomComboPreset.ReaperSoulSliceFeature) && !enshrouded && !soulReaver && level >= Levels.SoulSlice && gauge.Soul <= 50 && !GetCooldown(SoulSlice).IsCooldown && deathsDesign)
                        return SoulSlice;

                    if (IsEnabled(CustomComboPreset.ReaperShadowOfDeathFeature) && level >= Levels.ShadowOfDeath && !(deathsDesignTimer?.RemainingTime > 3) && !soulReaver && !(enshroudedTimer?.RemainingTime <= 10) && enemyHP > 5)
                        return ShadowOfDeath;

                    if (IsEnabled(CustomComboPreset.ReaperComboHealsOption))
                    {
                        if (level >= All.Levels.Bloodbath && playerHP < 65 && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;

                        if (level >= All.Levels.SecondWind && playerHP < 40 && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperLemureFeature) && enshrouded && gauge.VoidShroud >= 2)
                        return OriginalHook(BloodStalk);

                    if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature) && enshrouded && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                        return Communio;

                    if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeature) && (soulReaver || enshrouded))
                    {
                        if ((HasEffect(Buffs.EnhancedGallows) && !enshrouded && IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption)) || (HasEffect(Buffs.EnhancedCrossReaping) && enshrouded))
                            return OriginalHook(Gallows);

                        return OriginalHook(Gibbet);
                    }

                    if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsInverseFeature) && (soulReaver || enshrouded))
                    {
                        if ((HasEffect(Buffs.EnhancedGibbet) && !enshrouded) || (HasEffect(Buffs.EnhancedVoidReaping) && enshrouded))
                            return OriginalHook(Gibbet);

                        return OriginalHook(Gallows);
                    }

                    if (level >= Levels.WaxingSlice && lastComboMove is RPR.Slice)
                        return WaxingSlice;

                    else if (level >= Levels.InfernalSlice && lastComboMove is RPR.WaxingSlice)
                        return InfernalSlice;

                }
                return actionID;
            }
        }

        internal class ReaperScytheCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperScytheCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.SpinningScythe)
                {
                    var aoecombo = 0;
                    var gauge = GetJobGauge<RPRGauge>();
                    var enshrouded = HasEffect(Buffs.Enshrouded);
                    var enshroudedTimer = FindEffect(Buffs.Enshrouded);
                    var soulScytheReady = IsOffCooldown(SoulScythe);
                    var soulReaver = HasEffect(Buffs.SoulReaver);
                    var deathsDesign = TargetHasEffect(Debuffs.DeathsDesign);
                    var deathsDesignTimer = FindTargetEffect(Debuffs.DeathsDesign);
                    var enemyHP = EnemyHealthPercentage();

                    if (IsEnabled(CustomComboPreset.ReaperSoulScytheFeature) && !enshrouded && !soulReaver
                        && level >= Levels.SoulScythe && gauge.Soul <= 50 && soulScytheReady && deathsDesign)
                        return SoulScythe;

                    if (comboTime > 0 && IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && !(deathsDesignTimer?.RemainingTime > 3) && !soulReaver && !(enshroudedTimer?.RemainingTime <= 10) && enemyHP > 5)
                    {
                        if ((lastComboMove is RPR.SpinningScythe) && (!deathsDesign || deathsDesignTimer.RemainingTime <= 3 && level >= Levels.WhorlOfDeath))
                        {
                            if (level >= Levels.NightmareScythe)
                            {
                                aoecombo = 1;
                            }

                            return WhorlOfDeath;
                        }

                        if ((aoecombo is 1) || (lastComboMove is RPR.SpinningScythe && deathsDesignTimer.RemainingTime >= 4 && level >= Levels.NightmareScythe))
                        {
                            if (aoecombo is 1)
                            {
                                aoecombo = 0;
                            }

                            return NightmareScythe;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && !(deathsDesignTimer?.RemainingTime > 4) && !soulReaver && !(enshroudedTimer?.RemainingTime <= 10) && enemyHP > 5)
                        return WhorlOfDeath;

                    if (comboTime > 0 && !IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && enemyHP > 5 && !enshrouded && !soulReaver)
                    {
                        if (lastComboMove is RPR.SpinningScythe && level >= Levels.NightmareScythe)
                            return NightmareScythe;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperLemureFeature))
                    {
                        if (enshrouded && gauge.VoidShroud >= 2)
                            return OriginalHook(GrimSwathe);
                    }

                    if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature))
                    {
                        if (enshrouded && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return Communio;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperGuillotineFeature) && (soulReaver || enshrouded))
                        return OriginalHook(Guillotine);

                    if (lastComboMove is RPR.SpinningScythe)
                        return OriginalHook(NightmareScythe);

                    return SpinningScythe;
                }

                return actionID;
            }
        }

        internal class ReaperEnshroudCommunioFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperEnshroudCommunioFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var enshroudCD = GetCooldown(Enshroud);
                if (actionID is RPR.Enshroud)
                {
                    if (level >= Levels.Communio && HasEffect(Buffs.Enshrouded) && enshroudCD.CooldownRemaining < 12)
                        return Communio;

                    return Enshroud;
                }

                return actionID;
            }
        }

        internal class ReaperGibbetGallowsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperGibbetGallowsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.ShadowOfDeath)
                {
                    if (HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && (!IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption) || (!HasEffect(Buffs.EnhancedGallows) && !HasEffect(Buffs.EnhancedGibbet))))
                        return OriginalHook(Gallows);
                }

                return actionID;
            }
        }

        internal class ReaperHarvestFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarvestFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.ArcaneCircle)
                {
                    if (HasEffect(Buffs.ImmortalSacrifice) && level >= Levels.PlentifulHarvest)
                        return PlentifulHarvest;
                }

                return actionID;
            }
        }

        internal class ReaperRegressFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperRegressFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if ((actionID is HellsEgress or HellsIngress) && HasEffect(Buffs.Threshold))
                    return Regress;

                return actionID;
            }
        }

        internal class ReaperBloodSwatheFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodSwatheFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gluttonyCD = GetCooldown(Gluttony);
                if ((actionID is GrimSwathe or BloodStalk) && !gluttonyCD.IsCooldown && level >= Levels.Gluttony)
                    return Gluttony;

                return actionID;
            }
        }

        internal class ReaperBloodStalkComboFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodStalkComboFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gluttonyCD = GetCooldown(Gluttony);
                var gauge = GetJobGauge<RPRGauge>();

                if (actionID is RPR.BloodStalk)
                {
                    if (HasEffect(Buffs.Enshrouded) && level >= Levels.Enshroud)
                    {
                        if (gauge.VoidShroud >= 2)
                            return OriginalHook(BloodStalk);

                        if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.EnhancedCrossReaping))
                            return OriginalHook(Gallows);

                        if (HasEffect(Buffs.EnhancedVoidReaping))
                            return OriginalHook(Gibbet);

                        return OriginalHook(Gibbet);
                    }

                    if (!HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded))
                    {
                        if ((actionID is RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= Levels.Gluttony)
                            return Gluttony;
                        return BloodStalk;
                    }

                    if (!HasEffect(Buffs.Enshrouded) && HasEffect(Buffs.SoulReaver) && (actionID is BloodStalk or Gluttony))
                    {
                        if (HasEffect(Buffs.EnhancedGallows) && !HasEffect(Buffs.Enshrouded))
                            return Gallows;

                        return Gibbet;
                    }
                }
                return actionID;
            }
        }

        internal class ReaperBloodStalkAlternateComboOption : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodStalkAlternateComboOption;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gluttonyCD = GetCooldown(Gluttony);
                var gauge = GetJobGauge<RPRGauge>();

                if (actionID is RPR.BloodStalk)
                {
                    if (HasEffect(Buffs.Enshrouded) && level >= (Levels.Enshroud))
                    {
                        if (gauge.VoidShroud >= 2)
                            return OriginalHook(BloodStalk);

                        if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.EnhancedCrossReaping))
                            return OriginalHook(Gallows);

                        if (HasEffect(Buffs.EnhancedVoidReaping))
                            return OriginalHook(Gibbet);

                        return OriginalHook(Gallows);
                    }

                    if (!HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded))
                    {
                        if ((actionID is RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= Levels.Gluttony)
                            return Gluttony;
                        return BloodStalk;
                    }

                    if (!HasEffect(Buffs.Enshrouded) && HasEffect(Buffs.SoulReaver) && (actionID is BloodStalk or Gluttony))
                    {
                        if (HasEffect(Buffs.EnhancedGallows))
                            return Gallows;

                        if (HasEffect(Buffs.EnhancedGibbet))
                            return Gibbet;

                        return Gallows;
                    }
                }
                return actionID;
            }
        }

        internal class ReaperGrimSwatheComboFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperGrimSwatheComboFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gluttonyCD = GetCooldown(Gluttony);
                var gauge = GetJobGauge<RPRGauge>();
                if (actionID is RPR.GrimSwathe)
                {
                    if (HasEffect(Buffs.Enshrouded) && level >= Levels.Enshroud)
                    {
                        if (gauge.VoidShroud >= 2)
                            return OriginalHook(GrimSwathe);

                        if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= Levels.Communio)
                            return OriginalHook(Communio);

                        return OriginalHook(Guillotine);
                    }

                    if (!HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded))
                    {
                        if ((actionID is RPR.GrimSwathe) && !gluttonyCD.IsCooldown && level >= Levels.Gluttony)
                            return Gluttony;
                        return GrimSwathe;
                    }

                    if (!HasEffect(Buffs.Enshrouded) && HasEffect(Buffs.SoulReaver) && (actionID is GrimSwathe or Gluttony))
                        return Guillotine;

                }
                return actionID;
            }
        }

        internal class ReaperHarpeSoulsowFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarpeSoulsowFeature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.Harpe)
                {
                    if (level >= Levels.Soulsow && !HasEffect(Buffs.Soulsow) && (!HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat) || !HasTarget()))
                        return Soulsow;
                }

                return actionID;
            }
        }

        internal class ReaperHarvestMoonFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarpeHarvestMoonFeature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RPR.Harpe)

                    if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonFeature))
                    {
                        if (level >= Levels.HarvestMoon && HasEffect(Buffs.Soulsow))
                        {

                            if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(Buffs.EnhancedHarpe))
                                return Harpe;

                            if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                                return Harpe;

                            return HarvestMoon;
                        }
                    }

                return actionID;
            }
        }
    }
}
