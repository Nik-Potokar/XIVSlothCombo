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
            Gluttony = 24393,
            // AoE
            SpinningScythe = 24376,
            NightmareScythe = 24377,
            WhorlOfDeath = 24379,
            GrimSwathe = 24392,
            // Soul Reaver
            BloodStalk = 24389,
            Gibbet = 24382,
            Gallows = 24383,
            Guillotine = 24384,
            // Sacrifice
            ArcaneCircle = 24405,
            PlentifulHarvest = 24385,
            // Shroud
            Enshroud = 24394,
            Communio = 24398,
            LemuresSlice = 24399,
            LemuresScythe = 24400,
            // Misc
            ShadowOfDeath = 24378,
            HellsIngress = 24401,
            HellsEgress = 24402,
            Regress = 24403,
            Harpe = 24386,
            // Gauge
            SoulSlice = 24380,
            SoulScythe = 24381;

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
                HellsIngress = 20,
                HellsEgress = 20,
                SpinningScythe = 25,
                InfernalSlice = 30,
                NightmareScythe = 45,
                SoulReaver = 70,
                Regress = 74,
                Gluttony = 76,
                Enshroud = 80,
                PlentifulHarvest = 88,
                Communio = 90,
                WhorlOfDeath = 35;
        }
    }

    internal class ReaperComboCommunioFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperComboCommunioFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.Gibbet || actionID == RPR.Gallows || actionID == RPR.Guillotine)
            {
                var gauge = GetJobGauge<RPRGauge>();

                if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud == 1 && (gauge.VoidShroud == 0 || !IsEnabled(CustomComboPreset.ReaperLemureFeature)) && level >= RPR.Levels.Communio)
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
            if (actionID == RPR.Gibbet || actionID == RPR.Gallows || actionID == RPR.Guillotine)
            {
                var gauge = GetJobGauge<RPRGauge>();

                if (HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                {
                    if (actionID == RPR.Guillotine)
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
            if (actionID == RPR.Slice)
            {
                var gauge = GetJobGauge<RPRGauge>();
                var actionIDCD = GetCooldown(RPR.SoulScythe);
                if (IsEnabled(CustomComboPreset.ReaperSoulSliceFeature))
                {
                    if (gauge.Soul <= 50 && !actionIDCD.IsCooldown && TargetHasEffect(RPR.Debuffs.DeathsDesign))
                        return RPR.SoulSlice;
                }
                if ((IsEnabled(CustomComboPreset.ReaperShadowOfDeathFeature) && !(FindTargetEffect(RPR.Debuffs.DeathsDesign)?.RemainingTime > 3)) && !HasEffectAny(RPR.Buffs.SoulReaver) && !(FindEffect(RPR.Buffs.Enshrouded)?.RemainingTime <= 10))
                {
                    return RPR.ShadowOfDeath;
                }

                if (IsEnabled(CustomComboPreset.ReaperLemureFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                    {
                        return OriginalHook(RPR.BloodStalk);
                    }
                }

                if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                }

                if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeature) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                {
                    if ((HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.Enshrouded) && IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption)) || (HasEffect(RPR.Buffs.EnhancedCrossReaping) && HasEffect(RPR.Buffs.Enshrouded)))
                        return OriginalHook(RPR.Gallows);

                    return OriginalHook(RPR.Gibbet);
                }


                //this seems not okay
                if (IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeatureOption) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                {
                    if ((HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.Enshrouded) && IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeatureOption)) || (HasEffect(RPR.Buffs.EnhancedCrossReaping) && HasEffect(RPR.Buffs.Enshrouded)))
                        return OriginalHook(RPR.Gibbet);
                    if ((HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.Enshrouded) && IsEnabled(CustomComboPreset.ReaperGibbetGallowsFeatureOption)) || (HasEffect(RPR.Buffs.EnhancedCrossReaping) && HasEffect(RPR.Buffs.Enshrouded)))
                        return OriginalHook(RPR.Gallows);

                    return OriginalHook(RPR.Gallows);
                }

                if (level >= RPR.Levels.WaxingSlice && lastComboMove == RPR.Slice)
                {
                    return RPR.WaxingSlice;
                }
                else if (level >= RPR.Levels.InfernalSlice && lastComboMove == RPR.WaxingSlice)
                {
                    return RPR.InfernalSlice;
                }

            }
            return actionID;
        }
    }

    internal class ReaperScytheCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperScytheCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.SpinningScythe)
            {
                var gauge = GetJobGauge<RPRGauge>();
                var aoecombo = 0;
                var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.ReaperSoulScytheFeature))
                {
                    if (gauge.Soul <= 50 && IsOffCooldown(RPR.SoulScythe) && TargetHasEffect(RPR.Debuffs.DeathsDesign))
                        return RPR.SoulScythe;
                }
                if (IsEnabled(CustomComboPreset.ReaperLemureFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                    {
                        return OriginalHook(RPR.GrimSwathe);
                    }
                }

                if (IsEnabled(CustomComboPreset.ReaperComboCommunioFeature))
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                }

                if (IsEnabled(CustomComboPreset.ReaperGuillotineFeature) && (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Enshrouded)))
                    return OriginalHook(RPR.Guillotine);

                if (comboTime > 0 && IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature))
                {
                    var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
                    var deathsDesignTimer = FindTargetEffect(RPR.Debuffs.DeathsDesign);

                    if ((lastComboMove == RPR.SpinningScythe) && ((!deathsDesign || deathsDesignTimer.RemainingTime <= 3) && level >= RPR.Levels.WhorlOfDeath))
                    {
                        if (level >= RPR.Levels.NightmareScythe)
                        {
                            aoecombo = 1;
                        }

                        return RPR.WhorlOfDeath;
                    }

                    if ((aoecombo == 1) || ((lastComboMove == RPR.SpinningScythe && deathsDesignTimer.RemainingTime >= 4) && level >= RPR.Levels.NightmareScythe))
                    {
                        if (aoecombo == 1)
                        {
                            aoecombo = 0;
                        }

                        return RPR.NightmareScythe;
                    }
                }

                if (comboTime > 0 && !IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature))
                {
                    if (lastComboMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                        return RPR.NightmareScythe;
                }

                if (IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature))
                {
                    var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
                    var deathsDesignTimer = FindTargetEffect(RPR.Debuffs.DeathsDesign);
                    var soulReaverBuff = HasEffectAny(RPR.Buffs.SoulReaver);

                    if (((!deathsDesign && !soulReaverBuff) || (deathsDesignTimer.RemainingTime < 4 && !soulReaverBuff)) && level >= RPR.Levels.WhorlOfDeath)
                        return RPR.WhorlOfDeath;
                }

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
            if (actionID == RPR.Enshroud)
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
            if (actionID == RPR.ShadowOfDeath)
            {
                if (HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded) && (!IsEnabled(CustomComboPreset.ReaperGibbetGallowsOption) || (!HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.EnhancedGibbet))))
                {
                    return OriginalHook(RPR.Gallows);
                }
            }

            return actionID;
        }
    }

    internal class ReaperHarvestFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarvestFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.ArcaneCircle)
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
            if ((actionID == RPR.HellsEgress || actionID == RPR.HellsIngress) && HasEffect(RPR.Buffs.Threshold))
            {
                return RPR.Regress;
            }

            return actionID;
        }
    }

    internal class ReaperBloodSwatheFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodSwatheFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            if ((actionID == RPR.GrimSwathe || actionID == RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= 76)
                return RPR.Gluttony;

            return actionID;
        }
    }
    internal class ReaperBloodStalkComboOption : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodSwatheComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            var gauge = GetJobGauge<RPRGauge>();

            if (actionID == RPR.BloodStalk)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= 80)
                {
                    if (gauge.VoidShroud >= 2)
                    {
                        return OriginalHook(RPR.BloodStalk);
                    }
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= 90)
                    {
                        return OriginalHook(RPR.Communio);
                    }
                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                    {
                        return OriginalHook(RPR.Gallows);
                    }
                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                    {
                        return OriginalHook(RPR.Gibbet);
                    }
                    return OriginalHook(RPR.Gibbet);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID == RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= 76)
                        return RPR.Gluttony;
                    return RPR.BloodStalk;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID == RPR.BloodStalk || actionID == RPR.Gluttony))
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

            if (actionID == RPR.BloodStalk)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= 80)
                {
                    if (gauge.VoidShroud >= 2)
                    {
                        return OriginalHook(RPR.BloodStalk);
                    }
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= 90)
                    {
                        return OriginalHook(RPR.Communio);
                    }
                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                    {
                        return OriginalHook(RPR.Gallows);
                    }
                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                    {
                        return OriginalHook(RPR.Gibbet);
                    }
                    return OriginalHook(RPR.Gallows);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID == RPR.BloodStalk) && !gluttonyCD.IsCooldown && level >= 76)
                        return RPR.Gluttony;
                    return RPR.BloodStalk;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID == RPR.BloodStalk || actionID == RPR.Gluttony))
                {
                    if (HasEffect(RPR.Buffs.EnhancedGallows) && !HasEffect(RPR.Buffs.Enshrouded))
                        return RPR.Gallows;

                    if (HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.Enshrouded))
                        return RPR.Gibbet;

                    return RPR.Gallows;
                }
            }
            return actionID;
        }
    }


    internal class ReaperGrimSwatheComboOption : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperGrimSwatheComboOption;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gluttonyCD = GetCooldown(RPR.Gluttony);
            var gauge = GetJobGauge<RPRGauge>();
            if (actionID == RPR.GrimSwathe)
            {
                if (HasEffect(RPR.Buffs.Enshrouded) && level >= 80)
                {
                    if (gauge.VoidShroud >= 2)
                    {
                        return OriginalHook(RPR.GrimSwathe);
                    }
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= 90)
                    {
                        return OriginalHook(RPR.Communio);
                    }
                    return OriginalHook(RPR.Guillotine);
                }

                if (!HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(RPR.Buffs.Enshrouded))
                {
                    if ((actionID == RPR.GrimSwathe) && !gluttonyCD.IsCooldown && level >= 76)
                        return RPR.Gluttony;
                    return RPR.GrimSwathe;
                }

                if (!HasEffect(RPR.Buffs.Enshrouded) && HasEffect(RPR.Buffs.SoulReaver) && (actionID == RPR.GrimSwathe || actionID == RPR.Gluttony))
                {
                    return RPR.Guillotine;
                }
            }
            return actionID;
        }
    }
}
