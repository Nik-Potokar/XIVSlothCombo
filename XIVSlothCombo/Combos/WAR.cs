using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class WAR
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;

        public const uint
            HeavySwing = 31,
            Maim = 37,
            Berserk = 38,
            Overpower = 41,
            StormsPath = 42,
            StormsEye = 45,
            InnerBeast = 49,
            SteelCyclone = 51,
            Infuriate = 52,
            FellCleave = 3549,
            Decimate = 3550,
            Upheaval = 7387,
            InnerRelease = 8768,
            RawIntuition = 3551,
            MythrilTempest = 16462,
            ChaoticCyclone = 16463,
            NascentFlash = 16464,
            InnerChaos = 16465,
            Orogeny = 25752,
            PrimalRend = 25753,
            Onslaught = 7386;

        public static class Buffs
        {
            public const short
                InnerRelease = 1177,
                SurgingTempest = 2677,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                Berserk = 86;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                StormsPath = 26,
                MythrilTempest = 40,
                StormsEye = 50,
                FellCleave = 54,
                Decimate = 60,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80,
                PrimalRend = 90;
        }
    }

    // Replace Storm's Path with Storm's Path combo and overcap feature on main combo to fellcleave
    internal class WarriorStormsPathCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.WarriorStormsPathCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.StormsPath)
            {
                var heavyswingCD = GetCooldown(WAR.HeavySwing);
                var upheavalCD = GetCooldown(WAR.Upheaval);
                var innerreleaseCD = GetCooldown(WAR.InnerRelease);
                var beserkCD = GetCooldown(WAR.Berserk);
                var stormseyeBuff = FindEffectAny(WAR.Buffs.SurgingTempest);
                var innerReleaseBuff = HasEffect(WAR.Buffs.InnerRelease);
                var onslaughtCD = GetCooldown(WAR.Onslaught);
                var actionIDCD = GetCooldown(actionID);
                var surgingtempestBuff = HasEffect(WAR.Buffs.SurgingTempest);

                if (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && HasEffect(WAR.Buffs.SurgingTempest) && level >= 80)
                    return WAR.InnerChaos;

                if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && !upheavalCD.IsCooldown && heavyswingCD.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && beserkCD.IsCooldown && level >= 64 && level <= 69)
                    return WAR.Upheaval;
                    else
                if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && !upheavalCD.IsCooldown && heavyswingCD.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && level >= 70)
                    return WAR.Upheaval;

                if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady))
                {
                    return WAR.PrimalRend;
                }

                if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                {
                    return WAR.FellCleave;
                }
                // uses all stacks
                if (IsEnabled(CustomComboPreset.WarriorOnslaughtFeature) && level >= 62)
                {
                    if (onslaughtCD.CooldownRemaining < 60 && actionIDCD.CooldownRemaining > 0.7 && surgingtempestBuff)
                        return WAR.Onslaught;
                }
                // leaves 1 stack
                if (IsEnabled(CustomComboPreset.WarriorOnslaughtFeatureOption) && level >= 62)
                {
                    if (onslaughtCD.CooldownRemaining < 30 && actionIDCD.CooldownRemaining > 0.7 && surgingtempestBuff)
                        return WAR.Onslaught;
                }
                // leaves 2 stacks
                if (IsEnabled(CustomComboPreset.WarriorOnslaughtFeatureOptionTwo) && level >= 62)
                {
                    if (onslaughtCD.CooldownRemaining < 30 && actionIDCD.CooldownRemaining > 0.7 && surgingtempestBuff)
                        return WAR.Onslaught;
                    else
                    if (level >= 84)
                    {
                        if (onslaughtCD.CooldownRemaining < 1 && actionIDCD.CooldownRemaining > 0.7 && surgingtempestBuff)
                        return WAR.Onslaught;
                     }
                }
                if (comboTime > 0)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;

                    if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 54)
                        return WAR.FellCleave;
                    if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 35 && level <= 53)
                        return WAR.InnerBeast;
                    if (lastComboMove == WAR.Maim && level >= 50 && !HasEffectAny(WAR.Buffs.SurgingTempest))
                        return WAR.StormsEye;
                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    {
                        if (gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= 35 && level <= 53)
                        {
                            return WAR.InnerBeast;
                        }

                        if (gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= 54)
                        {
                            return WAR.FellCleave;
                        }

                        return WAR.Maim;
                    }

                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        if (gauge >= 90 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= 35 && level <= 53)
                        {
                            return OriginalHook(WAR.InnerBeast);
                        }

                        if (gauge >= 90 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= 54)
                        {
                            return OriginalHook(WAR.FellCleave);
                        }


                        if (stormseyeBuff.RemainingTime < 15 && IsEnabled(CustomComboPreset.WarriorStormsEyeCombo) && level > 50)
                            return WAR.StormsEye;
                        return WAR.StormsPath;
                    }
                }

                return WAR.HeavySwing;
            }

            return actionID;
        }

        internal class WarriorStormsEyeCombo : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.WarriorStormsEyeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.StormsEye)
                {
                    if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                        return OriginalHook(WAR.FellCleave);

                    if (comboTime > 0)
                    {
                        if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                            return WAR.Maim;

                        if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                            return WAR.StormsEye;
                    }

                    return WAR.HeavySwing;
                }

                return actionID;
            }
        }

        internal class WarriorMythrilTempestCombo : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.WarriorMythrilTempestCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.Overpower)
                {
                    var orogenyCD = GetCooldown(WAR.Orogeny);
                    var mythrilCd = GetCooldown(WAR.MythrilTempest);
                    var decimateCD = GetCooldown(WAR.Decimate);

                    if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady) && level >= 90)
                        return OriginalHook(WAR.PrimalRend);
                    if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                        return OriginalHook(WAR.Decimate);
                    if (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && HasEffect(WAR.Buffs.SurgingTempest) && level >= 72)
                        return OriginalHook(WAR.ChaoticCyclone);

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature))
                        {
                            if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                                return WAR.PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WarriorMythrilTempestCombo))
                        {
                            var gauge = GetJobGauge<WARGauge>().BeastGauge;
                            if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 60)
                                return WAR.Decimate;
                            if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 45 && level <= 59)
                                return WAR.SteelCyclone;
                            if (lastComboMove == WAR.Infuriate && level >= 60)
                                return WAR.Decimate;
                            if (lastComboMove == WAR.Overpower && level >= 40)
                                return WAR.MythrilTempest;
                            if ((lastComboMove == WAR.Overpower && level >= 60) || (lastComboMove == WAR.MythrilTempest && gauge >= 90 && level >= 60 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature)))
                                return WAR.Decimate;
                        }
                        if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && !orogenyCD.IsCooldown && decimateCD.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && lastComboMove == WAR.Decimate && level >= 86)
                        {
                            return WAR.Orogeny;
                        }
                        if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && !orogenyCD.IsCooldown && mythrilCd.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && level >= 86)
                        {
                            return WAR.Orogeny;
                        }
                    }

                    return WAR.Overpower;
                }

                return actionID;
            }
        }

        internal class WarriorNascentFlashFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.WarriorNascentFlashFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.NascentFlash)
                {
                    if (level >= WAR.Levels.NascentFlash)
                        return WAR.NascentFlash;
                    return WAR.RawIntuition;
                }

                return actionID;
            }
        }
    }

    internal class WarriorPrimalRendFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.WarriorPrimalRendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.InnerBeast || actionID == WAR.SteelCyclone)
            {

                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;

                // Fell Cleave or Decimate
                return OriginalHook(actionID);


            }

            return actionID;
        }
    }
}
