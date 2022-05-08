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
                RPRPositionChoice = "RPRPositionChoice",
                RPRSoDThreshold = "RPRSoDThreshold";
        }
    }

    internal class ReaperSliceCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSliceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<RPRGauge>();
            var enshrouded = HasEffect(RPR.Buffs.Enshrouded);
            var soulReaver = HasEffect(RPR.Buffs.SoulReaver);
            var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
            var playerHP = PlayerHealthPercentageHp();
            var enemyHP = EnemyHealthPercentage();
            var positionalChoice = Service.Configuration.GetCustomIntValue(RPR.Config.RPRPositionChoice);
            var sodThreshold = Service.Configuration.GetCustomIntValue(RPR.Config.RPRSoDThreshold);

            //Gibbet and Gallows on SoD
            if (actionID is RPR.ShadowOfDeath && IsEnabled(CustomComboPreset.GibbetGallowsonSTFeature) && IsEnabled(CustomComboPreset.GibbetGallowsonSoD) && soulReaver && level >= RPR.Levels.Gibbet)
            {
                if (HasEffect(RPR.Buffs.EnhancedGibbet))
                    return OriginalHook(RPR.Gibbet);
                if (HasEffect(RPR.Buffs.EnhancedGallows))
                    return OriginalHook(RPR.Gallows);
                if (!HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.EnhancedGallows))
                {
                    if (positionalChoice == 1)
                        return RPR.Gallows;
                    if (positionalChoice == 2)
                        return RPR.Gibbet;
                }
            }

            if (actionID is RPR.Slice)
            {
                if (IsEnabled(CustomComboPreset.GibbetGallowsonSTFeature) && HasEffect(RPR.Buffs.SoulReaver) && level >= RPR.Levels.Gibbet)
                {
                    if (HasEffect(RPR.Buffs.EnhancedGibbet))
                        return OriginalHook(RPR.Gibbet);
                    if (HasEffect(RPR.Buffs.EnhancedGallows))
                        return OriginalHook(RPR.Gallows);
                    if (!HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.EnhancedGallows))
                    {
                        if (positionalChoice == 1)
                            return RPR.Gallows;
                        if (positionalChoice == 2)
                            return RPR.Gibbet;
                    }
                }

                if (IsEnabled(CustomComboPreset.ReaperRangedFillerOption) && !InMeleeRange() && level >= RPR.Levels.Harpe)
                {
                    if (HasEffect(RPR.Buffs.Enshrouded) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                    if (level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow))
                    {
                        if ((IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(RPR.Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !InCombat()))
                            return RPR.Harpe;
                        return RPR.HarvestMoon;
                    }

                    return RPR.Harpe;
                }

                if (IsEnabled(CustomComboPreset.ReaperStunOption) && level >= All.Levels.LegSweep && CanInterruptEnemy() && IsOffCooldown(All.LegSweep))
                    return All.LegSweep;
                if (IsEnabled(CustomComboPreset.ReaperShadowOfDeathFeature) && level >= RPR.Levels.ShadowOfDeath && !soulReaver && enemyHP > sodThreshold)
                {
                    if ((IsEnabled(CustomComboPreset.DoubleSoDOption) && enshrouded && GetCooldownRemainingTime(RPR.ArcaneCircle) < 9 &&
                        ((gauge.LemureShroud is 4 && GetDebuffRemainingTime(RPR.Debuffs.DeathsDesign) < 30) || gauge.LemureShroud is 3 && GetDebuffRemainingTime(RPR.Debuffs.DeathsDesign) < 50)) || //double shroud windows
                        (GetDebuffRemainingTime(RPR.Debuffs.DeathsDesign) < 3 && IsOffCooldown(RPR.ArcaneCircle)) || //Opener Condition
                        (GetDebuffRemainingTime(RPR.Debuffs.DeathsDesign) < 3 && IsOnCooldown(RPR.ArcaneCircle)))  //non 2 minute windows  
                        return RPR.ShadowOfDeath;
                }

                if (IsEnabled(CustomComboPreset.ReaperComboHealsOption))
                {
                    if (level >= All.Levels.Bloodbath && playerHP < 65 && IsOffCooldown(All.Bloodbath))
                        return All.Bloodbath;
                    if (level >= All.Levels.SecondWind && playerHP < 40 && IsOffCooldown(All.SecondWind))
                        return All.SecondWind;
                }

                if (IsEnabled(CustomComboPreset.ArcaneCircleonSTFeature))
                {
                    if (IsOffCooldown(RPR.ArcaneCircle) && CanWeave(actionID) && level >= RPR.Levels.ArcaneCircle)
                        return RPR.ArcaneCircle;
                    if (IsEnabled(CustomComboPreset.PlentifulHarvestonSTOption) && HasEffect(RPR.Buffs.ImmortalSacrifice) && GetBuffRemainingTime(RPR.Buffs.ImmortalSacrifice) < 26 && !soulReaver && !enshrouded && level >= RPR.Levels.PlentifulHarvest)
                        return RPR.PlentifulHarvest;
                }

                if (deathsDesign || enemyHP < sodThreshold)
                {
                    if (!soulReaver && IsEnabled(CustomComboPreset.ReaperEnshroudonSTFeature))
                    {
                        if (!enshrouded && level >= RPR.Levels.Enshroud && IsOffCooldown(RPR.Enshroud) && CanWeave(actionID))
                        {
                            if (IsNotEnabled(CustomComboPreset.ReaperEnshroudPoolOption) && gauge.Shroud >= 50)
                                return RPR.Enshroud;
                            if (IsEnabled(CustomComboPreset.ReaperEnshroudPoolOption) &&
                                ((HasEffect(RPR.Buffs.ArcaneCircle) && gauge.Shroud >= 50) || //Shroud in Arcane Circle
                                (gauge.Shroud >= 50 && GetCooldownRemainingTime(RPR.ArcaneCircle) < 8) || //Prep for double shroud
                                (!HasEffect(RPR.Buffs.ArcaneCircle) && gauge.Shroud >= 90))) //Shroud pooling
                                return RPR.Enshroud;
                        }

                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.CommunioOnSTOption) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                                return RPR.Communio;
                            if (IsEnabled(CustomComboPreset.LemureonSTOption) && gauge.VoidShroud >= 2 && level >= RPR.Levels.LemuresSlice)
                                return OriginalHook(RPR.BloodStalk);
                            if (IsEnabled(CustomComboPreset.GibbetGallowsonSTFeature))
                            {
                                if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                                    return OriginalHook(RPR.Gibbet);
                                if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                                    return OriginalHook(RPR.Gallows);
                                if (!HasEffect(RPR.Buffs.EnhancedCrossReaping) && !HasEffect(RPR.Buffs.EnhancedVoidReaping))
                                {
                                    if (positionalChoice == 1)
                                        return OriginalHook(RPR.Gallows);
                                    if (positionalChoice == 2)
                                        return OriginalHook(RPR.Gibbet);
                                }
                            }
                        }
                    }

                    if (!(comboTime > 0) || lastComboMove is RPR.InfernalSlice || comboTime > 10)
                    {
                        if (IsEnabled(CustomComboPreset.GluttonyStalkonSTFeature) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && level >= RPR.Levels.BloodStalk)
                        {
                            if (gauge.Soul >= 50 && IsOffCooldown(RPR.Gluttony) && level >= RPR.Levels.Gluttony)
                                return RPR.Gluttony;
                            return RPR.BloodStalk;
                        }

                        if (IsEnabled(CustomComboPreset.ReaperSoulSliceFeature) && !enshrouded && !soulReaver && level >= RPR.Levels.SoulSlice && gauge.Soul <= 50 && GetRemainingCharges(RPR.SoulSlice) > 0)
                            return RPR.SoulSlice;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove is RPR.Slice && level >= RPR.Levels.WaxingSlice)
                        return RPR.WaxingSlice;
                    if (lastComboMove is RPR.WaxingSlice && level >= RPR.Levels.InfernalSlice)
                        return RPR.InfernalSlice;
                }

                return RPR.Slice;
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
                var enshrouded = HasEffect(RPR.Buffs.Enshrouded);
                var soulReaver = HasEffect(RPR.Buffs.SoulReaver);
                var deathsDesign = TargetHasEffect(RPR.Debuffs.DeathsDesign);
                var enemyHP = EnemyHealthPercentage();

                if (IsEnabled(CustomComboPreset.ReaperGuillotineFeature) && (soulReaver || enshrouded) && level >= RPR.Levels.Guillotine)
                    return OriginalHook(RPR.Guillotine);
                if (IsEnabled(CustomComboPreset.ReaperWhorlOfDeathFeature) && GetDebuffRemainingTime(RPR.Debuffs.DeathsDesign) < 3 && !soulReaver && enemyHP > 5 && level >= RPR.Levels.WhorlOfDeath)
                    return RPR.WhorlOfDeath;
                if (IsEnabled(CustomComboPreset.ArcaneCircleonAOEFeature))
                {
                    if (IsOffCooldown(RPR.ArcaneCircle) && CanWeave(actionID) && level >= RPR.Levels.ArcaneCircle)
                        return RPR.ArcaneCircle;
                    if (IsEnabled(CustomComboPreset.PlentifulHarvestonAOEOption) && HasEffect(RPR.Buffs.ImmortalSacrifice) && !HasEffect(RPR.Buffs.BloodsownCircle) && !soulReaver && !enshrouded && level >= RPR.Levels.PlentifulHarvest)
                        return RPR.PlentifulHarvest;
                }

                if (deathsDesign)
                {
                    if (IsEnabled(CustomComboPreset.ReapearEnshroudonAOEFeature))
                    {
                        if (!enshrouded && !soulReaver && level >= RPR.Levels.Enshroud && IsOffCooldown(RPR.Enshroud) && CanWeave(actionID) && gauge.Shroud >= 50)
                            return RPR.Enshroud;
                        if (enshrouded)
                        {
                            if (IsEnabled(CustomComboPreset.ReaperComboCommunioAOEFeature) && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                                return RPR.Communio;
                            if (IsEnabled(CustomComboPreset.ReaperLemureAOEFeature) && gauge.VoidShroud >= 2 && level >= RPR.Levels.LemuresScythe)
                                return OriginalHook(RPR.GrimSwathe);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.GluttonyStalkonAOEFeature) && !soulReaver && !enshrouded && gauge.Soul >= 50 && CanWeave(actionID) && level >= RPR.Levels.GrimSwathe)
                    {
                        if (gauge.Soul >= 50 && IsOffCooldown(RPR.Gluttony) && level >= RPR.Levels.Gluttony)
                            return RPR.Gluttony;
                        return RPR.GrimSwathe;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperSoulScytheFeature) && !enshrouded && !soulReaver && level >= RPR.Levels.SoulScythe && gauge.Soul <= 50 && GetRemainingCharges(RPR.SoulScythe) > 0 && (comboTime == 0 || comboTime > 15))
                        return RPR.SoulScythe;
                }

                if (lastComboMove is RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                    return OriginalHook(RPR.NightmareScythe);
                return RPR.SpinningScythe;
            }

            return actionID;
        }
    }

    internal class ReaperBloodStalkComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperBloodStalkComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var positionalChoice = Service.Configuration.GetCustomIntValue(RPR.Config.RPRPositionChoice);
            var gauge = GetJobGauge<RPRGauge>();

            if (actionID is RPR.GrimSwathe or RPR.BloodStalk && IsEnabled(CustomComboPreset.ReaperBloodSwatheFeature) && IsOffCooldown(RPR.Gluttony) && level >= RPR.Levels.Gluttony)
                return RPR.Gluttony;
            if (actionID is RPR.GrimSwathe)
            {
                if (IsEnabled(CustomComboPreset.ReaperEnshroudonStalkComboFeature) && HasEffect(RPR.Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                    if (gauge.VoidShroud >= 2 && level >= RPR.Levels.LemuresScythe)
                        return OriginalHook(RPR.GrimSwathe);
                    if (gauge.LemureShroud > 1)
                        return OriginalHook(RPR.Guillotine);
                }

                if (HasEffect(RPR.Buffs.SoulReaver) && level >= RPR.Levels.Guillotine)
                    return RPR.Guillotine;
            }

            if (actionID is RPR.BloodStalk)
            {
                if (IsEnabled(CustomComboPreset.ReaperEnshroudonStalkComboFeature) && HasEffect(RPR.Buffs.Enshrouded))
                {
                    if (gauge.LemureShroud == 1 && gauge.VoidShroud == 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;
                    if (gauge.VoidShroud >= 2 && level >= RPR.Levels.LemuresSlice)
                        return OriginalHook(RPR.BloodStalk);
                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                        return OriginalHook(RPR.Gibbet);
                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                        return OriginalHook(RPR.Gallows);
                    if (!HasEffect(RPR.Buffs.EnhancedCrossReaping) && !HasEffect(RPR.Buffs.EnhancedVoidReaping))
                    {
                        if (positionalChoice == 1)
                            return OriginalHook(RPR.Gallows);
                        if (positionalChoice == 2)
                            return OriginalHook(RPR.Gibbet);
                    }
                }

                if (HasEffect(RPR.Buffs.SoulReaver) && level >= RPR.Levels.Gibbet)
                {
                    if (HasEffect(RPR.Buffs.EnhancedGibbet))
                        return OriginalHook(RPR.Gibbet);
                    if (HasEffect(RPR.Buffs.EnhancedGallows))
                        return OriginalHook(RPR.Gallows);
                    if (!HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.EnhancedGallows))
                    {
                        if (positionalChoice == 1)
                            return OriginalHook(RPR.Gallows);
                        if (positionalChoice == 2)
                            return OriginalHook(RPR.Gibbet);
                    }
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

    internal class ReaperHarvestMoonFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarpeHarvestMoonFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPR.Harpe && level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow))
            {
                if ((IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(RPR.Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !InCombat()))
                    return RPR.Harpe;
                return RPR.HarvestMoon;
            }

            return actionID;
        }
    }

    internal class ReaperSoulSowReminderFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSoulSowReminderFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPR.Harpe or RPR.Slice or RPR.SpinningScythe or RPR.ShadowOfDeath or RPR.BloodStalk)
            {
                if (level >= RPR.Levels.Soulsow && !HasEffect(RPR.Buffs.Soulsow) && !InCombat())
                    return RPR.Soulsow;
            }

            return actionID;
        }
    }
    internal class ReaperEnshroudProtectionFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperEnshroudProtectionFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var positionalChoice = Service.Configuration.GetCustomIntValue(RPR.Config.RPRPositionChoice);
            if (actionID is RPR.Enshroud)
            {
                if (HasEffect(RPR.Buffs.SoulReaver))
                {
                    if (HasEffect(RPR.Buffs.EnhancedGibbet))
                        return OriginalHook(RPR.Gibbet);
                    if (HasEffect(RPR.Buffs.EnhancedGallows))
                        return OriginalHook(RPR.Gallows);
                    if (!HasEffect(RPR.Buffs.EnhancedGibbet) && !HasEffect(RPR.Buffs.EnhancedGallows))
                    {
                        if (positionalChoice == 1)
                            return RPR.Gallows;
                        if (positionalChoice == 2)
                            return RPR.Gibbet;
                    }
                }
            }

            return actionID;
        }
    }
}