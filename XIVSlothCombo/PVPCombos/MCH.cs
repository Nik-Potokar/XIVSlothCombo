using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MCHPVP
    {
        public const byte JobID = 31;

        public const uint
            SplitShot = 8848,
            SlugShot = 8849,
            CleanShot = 8850,
            Drill = 17749,
            Airanchor = 17750,
            SpreadShot = 18932,
            Bioblaster = 17752,
            GaussRound = 18933,
            Ricochet = 17753,
            Wildfire = 8855,
            Blank = 8853;


        public static class Buffs
        {
            public const short
                Concentrate = 2186;
        }

        public static class Debuffs
        {
            public const short
                Wildfire = 1323;
        }
    }
    internal class HeatedCleanShotFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.HeatedCleanShotFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCHPVP.SplitShot || actionID == MCHPVP.SlugShot || actionID == MCHPVP.CleanShot)
            {
                var actionIDCD = GetCooldown(actionID);
                var gaussCD = GetCooldown(MCHPVP.GaussRound);
                var ricoCD = GetCooldown(MCHPVP.Ricochet);
                var drillCD = GetCooldown(MCHPVP.Drill);
                var airCD = GetCooldown(MCHPVP.Airanchor);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                if (actionIDCD.IsCooldown && gaussCD.CooldownRemaining < ricoCD.CooldownRemaining)
                    return MCHPVP.GaussRound;
                else
                    if(actionIDCD.IsCooldown)
                    return MCHPVP.Ricochet;
                if (HasEffect(MCHPVP.Buffs.Concentrate) && !drillCD.IsCooldown)
                    return MCHPVP.Drill;
                if (!airCD.IsCooldown && incombat || HasEffect(MCHPVP.Buffs.Concentrate) && !airCD.IsCooldown)
                    return MCHPVP.Airanchor;
            }

            return OriginalHook(actionID);
        }
    }
    internal class WildfireBlankFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.WildfireBlankFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCHPVP.Wildfire)
            {
                if (TargetHasEffect(MCHPVP.Debuffs.Wildfire) && InMeleeRange(true))
                    return MCHPVP.Blank;
            }

            return OriginalHook(actionID);
        }
    }
}
