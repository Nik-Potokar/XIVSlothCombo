using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
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
            Ruin2 = 17870,
            SummonSeraph = 16545,
            SummonEos = 17215,
            SummonSelene = 17216,
            Broil1 = 3584,
            Broil2 = 7435,
            Broil3 = 16541,
            Broil4 = 25865,
            Indomitability = 3583,
            WhisperingDawn = 16537,
            FeyIllumination = 16538,
            Dissipation = 3587,
            Aetherpact = 7437,
            ChainStratagem = 7436;

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
            Biolysis = 1895,
            ChainStratagem = 1221;
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

    internal class SCHDPSAlternateFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SCHDPSAlternateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.Ruin2)
            {
                var gauge = GetJobGauge<SCHGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var biolysisDebuff = FindTargetEffect(SCH.Debuffs.Biolysis);
                var bio2Debuff = FindTargetEffect(SCH.Debuffs.Bio2);
                var bio1Debuff = FindTargetEffect(SCH.Debuffs.Bio1);

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= 72)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Biolysis) && incombat && level >= 72) || (biolysisDebuff.RemainingTime < 5 && incombat && level >= 72))
                        return SCH.Biolysis;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= 26 && level <= 71)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio2) && level <= 71 && level >= 26) || (bio2Debuff.RemainingTime < 5 && incombat && level >= 26 && level <= 71))
                        return SCH.Bio2;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= 2 && level <= 25)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio1) && level >= 2 && level <= 25) || (bio1Debuff.RemainingTime < 5 && incombat && level >= 2 && level <= 25))
                        return SCH.Bio1;
                }
            }
            return actionID;
        }
    }
    internal class ScholarFairyFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ScholarFairyFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<SCHGauge>();
            if (!Service.BuddyList.PetBuddyPresent && gauge.SeraphTimer == 0)
                return SCH.SummonSelene;

            return actionID;
        }
    }
    internal class ScholarDPSFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ScholarDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == SCH.Broil4 || actionID == SCH.Broil3 || actionID == SCH.Broil2 || actionID == SCH.Broil1 || actionID == SCH.Ruin1)
            {
                var actionIDCD = GetCooldown(actionID);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<SCHGauge>();
                var lucidDreaming = GetCooldown(SCH.LucidDreaming);
                // biosys
                var biosys = FindTargetEffect(SCH.Debuffs.Biolysis);
                // bio 1
                var bio1 = FindTargetEffect(SCH.Debuffs.Bio1);
                // bio 2
                var bio2 = FindTargetEffect(SCH.Debuffs.Bio2);
                // buff
                var chainBuff = GetCooldown(SCH.ChainStratagem);
                var chainTarget = TargetHasEffect(SCH.Debuffs.ChainStratagem);
                


                if (IsEnabled(CustomComboPreset.ScholarLucidDPSFeature))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && actionIDCD.CooldownRemaining > 0.2)
                        return SCH.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= 72 && incombat)
                {
                    if ((biosys is null) || (biosys.RemainingTime <= 3))
                        return SCH.Biolysis;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= 26 && level <= 71 && incombat)
                {
                    if ((bio2 is null) || (bio2.RemainingTime <= 3))
                        return SCH.Bio2;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= 4 && level <= 25 && incombat)
                {
                    if ((bio1 is null) || (bio1.RemainingTime <= 3))
                        return SCH.Bio1;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeatureBuffOption) && level >= 66)
                {
                    if (!chainBuff.IsCooldown && !chainTarget && actionIDCD.IsCooldown && incombat)
                        return SCH.ChainStratagem;
                }


            }
            return OriginalHook(SCH.Broil4);
        }
    }

}
