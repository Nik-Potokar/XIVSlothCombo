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
            HarvestMoon = 24388,
            // Role
            SecondWind = 7541,
            Bloodbath = 7542,
            LegSweep = 7863,
            Feint = 7549;
        

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
                SecondWind = 8,
                LegSweep = 10,
                ShadowOfDeath = 10,
                Bloodbath = 12,
                HellsIngress = 20,
                HellsEgress = 20,
                Feint = 22,
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
    }

    internal class ReaperComboCommunioFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperComboCommunioFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPR.Gibbet or RPR.Gallows or RPR.Guillotine)
            {
                var gauge = GetJobGauge<RPRGauge>();

                if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud is 1 && (gauge.VoidShroud is 0 || !IsEnabled(CustomComboPreset.ReaperLemureFeature)) && level >= RPR.Levels.Communio)
                    return RPR.Communio;
            }

            return actionID;
        }
    }

    internal class ReaperLemureFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperLemureFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPR.Gibbet or RPR.Gallows or RPR.Guillotine)
            {
                var gauge = GetJobGauge<RPRGauge>();

                if (HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                {
                    if (actionID is RPR.Guillotine)
                        return OriginalHook(RPR.GrimSwathe);
                    return OriginalHook(RPR.BloodStalk);
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
                var actionIDCD = GetCooldown(RPR.SoulSlice);
                var distance = GetTargetDistance();

                if (IsEnabled(CustomComboPreset.ReaperRangedFillerOption) && distance > 3)
                {

                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;

                    if (level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow)) 
                    {
                        if ((IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(RPR.Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat)))
                            return RPR.Harpe;

                        return RPR.HarvestMoon;
                    }
                    return RPR.Harpe;
                }

                if (IsEnabled(CustomComboPreset.ReaperStunOption) && level >= RPR.Levels.LegSweep && CanInterruptEnemy() && IsOffCooldown(RPR.LegSweep))
                    return RPR.LegSweep;

                if (IsEnabled(CustomComboPreset.ReaperSoulSliceFeature) && !HasEffect(RPR.Buffs.Enshrouded) && !HasEffectAny(RPR.Buffs.SoulReaver) && level >= RPR.Levels.SoulSlice && gauge.Soul <= 50 && !actionIDCD.IsCooldown && TargetHasEffect(RPR.Debuffs.DeathsDesign))
                    return RPR.SoulSlice;

                if (IsEnabled(CustomComboPreset.ReaperShadowOfDeathFeature) && level >= RPR.Levels.ShadowOfDeath && !(FindTargetEffect(RPR.Debuffs.DeathsDesign)?.RemainingTime > 3) && !HasEffectAny(RPR.Buffs.SoulReaver) && !(FindEffect(RPR.Buffs.Enshrouded)?.RemainingTime <= 10) && EnemyHealthPercentage() > 5)
                    return RPR.ShadowOfDeath;

                if (IsEnabled(CustomComboPreset.ReaperComboHealsOption))
                {
                    if (level >= RPR.Levels.Bloodbath && PlayerHealthPercentageHp() < 65 && IsOffCooldown(RPR.Bloodbath))
                        return RPR.Bloodbath;

                    if (level >= RPR.Levels.SecondWind && PlayerHealthPercentageHp() < 40 && IsOffCooldown(RPR.SecondWind))
                        return RPR.SecondWind;
                }

                if (IsEnabled(CustomComboPreset.ReaperLemureFeature) && HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                        return OriginalHook(RPR.BloodStalk);

                if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature) && HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;

                if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeature) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                {
                    if ((HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.Enshrouded) && IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption)) || (HasEffect(RPR.Buffs.EnhancedCrossReaping) && HasEffect(RPR.Buffs.Enshrouded)))
                        return OriginalHook(RPR.Gallows);

                    return OriginalHook(RPR.Gibbet);
                }

                if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsInverseFeature) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                {
                    if ((HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.Enshrouded)) || (HasEffect(RPR.Buffs.EnhancedVoidReaping) && HasEffect(RPR.Buffs.Enshrouded)))
                        return OriginalHook(RPR.Gibbet);

                    return OriginalHook(RPR.Gallows);
                }

                if (level >= RPR.Levels.WaxingSlice && lastComboMove is RPR.Slice)
                    return RPR.WaxingSlice;

                else if (level >= RPR.Levels.InfernalSlice && lastComboMove is RPR.WaxingSlice)
                    return RPR.InfernalSlice;

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
                var gauge = GetJobGauge<RPRGauge>();
                var aoecombo = 0;

                if (IsEnabled(CustomComboPreset.ReaperSoulScytheFeature) && !HasEffect(RPR.Buffs.Enshrouded) && !HasEffect(RPR.Buffs.SoulReaver) && level >= RPR.Levels.SoulScythe && gauge.Soul <= 50 && IsOffCooldown(RPR.SoulScythe) && TargetHasEffect(RPR.Debuffs.DeathsDesign))
                    return RPR.SoulScythe;

                if (comboTime > 0 && IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && !(FindTargetEffect(RPR.Debuffs.DeathsDesign)?.RemainingTime > 3) && !HasEffect(RPR.Buffs.SoulReaver) && !(FindEffect(RPR.Buffs.Enshrouded)?.RemainingTime <= 10) && EnemyHealthPercentage() > 5)
                {
                    var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
                    var deathsDesignTimer = FindTargetEffect(RPR.Debuffs.DeathsDesign);

                    if ((lastComboMove is RPR.SpinningScythe) && ((!deathsDesign || deathsDesignTimer.RemainingTime <= 3) && level >= RPR.Levels.WhorlOfDeath))
                    {
                        if (level >= RPR.Levels.NightmareScythe)
                        {
                            aoecombo = 1;
                        }

                        return RPR.WhorlOfDeath;
                    }

                    if ((aoecombo is 1) || ((lastComboMove is RPR.SpinningScythe && deathsDesignTimer.RemainingTime >= 4) && level >= RPR.Levels.NightmareScythe))
                    {
                        if (aoecombo is 1)
                        {
                            aoecombo = 0; 
                        }

                        return RPR.NightmareScythe;
                    }
                }

                if (IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && !(FindTargetEffect(RPR.Debuffs.DeathsDesign)?.RemainingTime > 4) && !HasEffectAny(RPR.Buffs.SoulReaver) && !(FindEffect(RPR.Buffs.Enshrouded)?.RemainingTime <= 10) && EnemyHealthPercentage() > 5)
                {
                    //var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
                    //var deathsDesignTimer = FindTargetEffect(RPR.Debuffs.DeathsDesign);
                    //var soulReaverBuff = HasEffectAny(RPR.Buffs.SoulReaver);

                    //if (((!deathsDesign && !soulReaverBuff) || (deathsDesignTimer.RemainingTime < 4 && !soulReaverBuff)) && level >= RPR.Levels.WhorlOfDeath)
                    return RPR.WhorlOfDeath;
                }

                if (comboTime > 0 && !IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && EnemyHealthPercentage() > 5)
                {
                    if (lastComboMove is RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                        return RPR.NightmareScythe;
                }

                if (IsEnabled(CustomComboPreset.ReaperLemureFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                        return OriginalHook(RPR.GrimSwathe);
                }

                if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                }

                if (IsEnabled(CustomComboPreset.ReaperGuillotineFeature) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                    return OriginalHook(RPR.Guillotine);

                if (lastComboMove is RPR.SpinningScythe)
                    return OriginalHook(RPR.NightmareScythe);

                return RPR.SpinningScythe;
            }

            return actionID;
        }
    }

    internal class ReaperEnshroudCommunioFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperEnshroudCommunioFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var enshroudCD = GetCooldown(RPR.Enshroud);
            if (actionID is RPR.Enshroud)
            {
                if (level >= RPR.Levels.Communio && HasEffect(RPR.Buffs.Enshrouded) && enshroudCD.CooldownRemaining < 12)
                    return RPR.Communio;

                return RPR.Enshroud;
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
                if (HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded) && (!IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption) || (!HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.EnhancedGibbet))))
                    return OriginalHook(RPR.Gallows);
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
                if (HasEffect(RPR.Buffs.ImmortalSacrifice) && level >= RPR.Levels.PlentifulHarvest)
                    return RPR.PlentifulHarvest;
            }

            return actionID;
        }
    }

    internal class ReaperRegressFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperRegressFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if ((actionID is RPR.HellsEgress or RPR.HellsIngress) && HasEffect(RPR.Buffs.Threshold))
                return RPR.Regress;

            return actionID;
        }
    }

    internal class ReaperBloodSwatheFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodSwatheFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            if ((actionID is RPR.GrimSwathe or RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= RPR.Levels.Gluttony)
                return RPR.Gluttony;

            return actionID;
        }
    }

    internal class ReaperBloodStalkComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodStalkComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            var gauge = GetJobGauge<RPRGauge>();

            if (actionID is RPR.BloodStalk)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= RPR.Levels.Enshroud)
                {
                    if (gauge.VoidShroud >= 2)
                        return OriginalHook(RPR.BloodStalk);

                    if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return OriginalHook(RPR.Communio);

                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                        return OriginalHook(RPR.Gallows);

                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                        return OriginalHook(RPR.Gibbet);

                    return OriginalHook(RPR.Gibbet);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID is RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= RPR.Levels.Gluttony)
                        return RPR.Gluttony;
                    return RPR.BloodStalk;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID is RPR.BloodStalk or RPR.Gluttony))
                {
                    if (HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.Enshrouded))
                        return RPR.Gallows;

                    return RPR.Gibbet;
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
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            var gauge = GetJobGauge<RPRGauge>();

            if (actionID is RPR.BloodStalk)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= (RPR.Levels.Enshroud))
                {
                    if (gauge.VoidShroud >= 2)
                        return OriginalHook(RPR.BloodStalk);

                    if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return OriginalHook(RPR.Communio);

                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                        return OriginalHook(RPR.Gallows);

                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                        return OriginalHook(RPR.Gibbet);

                    return OriginalHook(RPR.Gallows);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID is RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= RPR.Levels.Gluttony)
                        return RPR.Gluttony;
                    return RPR.BloodStalk;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID is RPR.BloodStalk or RPR.Gluttony))
                {
                    if (HasEffect(RPR.Buffs.EnhancedGallows))
                        return RPR.Gallows;

                    if (HasEffect(RPR.Buffs.EnhancedGibbet))
                        return RPR.Gibbet;

                    return RPR.Gallows;
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
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            var gauge = GetJobGauge<RPRGauge>();
            if (actionID is RPR.GrimSwathe)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= RPR.Levels.Enshroud)
                {
                    if (gauge.VoidShroud >= 2)
                        return OriginalHook(RPR.GrimSwathe);

                    if (gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return OriginalHook(RPR.Communio);

                    return OriginalHook(RPR.Guillotine);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID is RPR.GrimSwathe) && !gluttonyCD.IsCooldown && level >= RPR.Levels.Gluttony)
                        return RPR.Gluttony;
                    return RPR.GrimSwathe;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID is RPR.GrimSwathe or RPR.Gluttony))
                    return RPR.Guillotine;

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
                if (level >= RPR.Levels.Soulsow && !HasEffect(RPR.Buffs.Soulsow) && (!HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat) || !HasTarget()))
                    return RPR.Soulsow;
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
                    if (level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow))
                    {

                        if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(RPR.Buffs.EnhancedHarpe))
                                return RPR.Harpe;

                        if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                                return RPR.Harpe;

                        return RPR.HarvestMoon;
                    }
                }

            return actionID;
        }
    }
}
