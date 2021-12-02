using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Structs;


namespace XIVComboExpandedPlugin.Combos
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
            InnerChaos = 16465;

        public static class Buffs
        {
            public const short
                InnerRelease = 1177,
                StormsEye = 90,
                NascentChaos = 1897;
                
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
                InnerBeast = 35,
                InnerChaos = 80;
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
                var InnerreleaseCD = GetCooldown(WAR.InnerRelease);
                var beserkCD = GetCooldown(WAR.Berserk);
                var stormseyeBuff = FindEffectAny(WAR.Buffs.StormsEye);
                var innerReleaseBuff = HasEffect(WAR.Buffs.InnerRelease);
                if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeatureDuringIR) && !upheavalCD.IsCooldown && HasEffect(WAR.Buffs.StormsEye) && heavyswingCD.CooldownRemaining > 0.7 && level >= 70 && (InnerreleaseCD.CooldownRemaining > 25 || innerReleaseBuff))
                {
                    return WAR.Upheaval;
                }
                if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && !upheavalCD.IsCooldown && HasEffect(WAR.Buffs.StormsEye) && heavyswingCD.CooldownRemaining > 0.7 && level >= 64 || (InnerreleaseCD.CooldownRemaining > 25 || beserkCD.CooldownRemaining > 2))
                {
                    return WAR.Upheaval;
                }
                if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                {
                    return WAR.FellCleave;
                }
                if (comboTime > 0)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye && !HasEffectAny(WAR.Buffs.StormsEye))
                        return WAR.StormsEye;
                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    {
                        if (gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
                        {
                            return WAR.FellCleave;
                        }
                        return WAR.Maim;
                    }
                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        if (gauge >= 90 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
                        {
                            return WAR.FellCleave;
                        }
                        if (stormseyeBuff.RemainingTime < 15 && IsEnabled(CustomComboPreset.WarriorStormsEyeCombo) && level >= 50)
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
            if (actionID == WAR.MythrilTempest)
            {
                 if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                     return OriginalHook(WAR.Decimate);

                 if (comboTime > 0)
                {
                    if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                    {
                         var gauge = GetJobGauge<WARGauge>().BeastGauge;
                         if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.MythrilTempestTrait)
                             return OriginalHook(WAR.Decimate);

                         return WAR.MythrilTempest;
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

}
