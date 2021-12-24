using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SCH
    {
        public const byte ClassID = 15;
        public const byte JobID = 28;

        public const uint
            FeyBless = 16543,
            Consolation = 16546,
            EnergyDrain = 167,
            LucidDreaming = 7562,
            Resurrection = 173,
            Swiftcast = 7561,
            Aetherflow = 166,
            Bio1 = 17864,
            Bio2 = 17865,
            Biolysis = 16540,
            Ruin1 = 163,
            Ruin2 = 17870;

        public static class Buffs
        {
            public const short
            Swiftcast = 167;
        }

        public static class Debuffs
        {
            public const short
            Bio1 = 179,
            Bio2 = 189,
            Biolysis = 1895;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }

    internal class ScholarSeraphConsolationFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ScholarSeraphConsolationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.FeyBless)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (gauge.SeraphTimer > 0)
                    return SCH.Consolation;
            }

            return actionID;
        }
    }

    internal class ScholarEnergyDrainFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ScholarEnergyDrainFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.EnergyDrain)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (gauge.Aetherflow == 0)
                    return SCH.Aetherflow;
            }

            return actionID;
        }

        internal class SchRaiseFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.SchRaiseFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SCH.Swiftcast)
                {
                    if (IsEnabled(CustomComboPreset.SchRaiseFeature))
                    {
                        if (HasEffect(SCH.Buffs.Swiftcast))
                            return SCH.Resurrection;
                    }

                    return OriginalHook(SCH.Swiftcast);
                }

                return actionID;
            }
        }
    }

    internal class SCHDPSFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SCHDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.Ruin2)
            {
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var biolysisDebuff = FindTargetEffect(SCH.Debuffs.Biolysis);
                var bio2Debuff = FindTargetEffect(SCH.Debuffs.Bio2);
                var bio1Debuff = FindTargetEffect(SCH.Debuffs.Bio1);

                if (IsEnabled(CustomComboPreset.SCHDPSFeature) && level >= 72)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Biolysis) && incombat && level >= 72) || (biolysisDebuff.RemainingTime < 5 && incombat && level >= 72))
                        return SCH.Biolysis;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSFeature) && level >= 26 && level <= 71)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio2) && level <= 71) || (bio2Debuff.RemainingTime < 5 && incombat && level >= 26 && level <= 71))
                        return SCH.Bio2;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSFeature) && level >= 2 && level <= 25)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio1) && level >= 2 && level <= 25) || (bio1Debuff.RemainingTime < 5 && incombat && level >= 2 && level <= 25))
                        return SCH.Bio1;
                }
            }

            return actionID;
        }
    }
}
