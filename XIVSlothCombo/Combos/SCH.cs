using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SCH
    {
        public const byte ClassID = 15;
        public const byte JobID = 28;

        public const uint

            // Heals
            Physick = 190,
            Adloquium = 185,
            Succor = 186,
            Lustrate = 189,
            SacredSoil = 188,
            Indomitability = 3583,
            Consolation = 16546,

            // Offense
            Bio1 = 17864,
            Bio2 = 17865,
            Biolysis = 16540,
            Ruin1 = 163,
            Ruin2 = 17870,
            Broil1 = 3584,
            Broil2 = 7435,
            Broil3 = 16541,
            Broil4 = 25865,
            Scourge = 16539,
            EnergyDrain = 167,

            // Faerie
            SummonSeraph = 16545,
            SummonEos = 17215,
            SummonSelene = 17216,
            WhisperingDawn = 16537,
            FeyIllumination = 16538,
            Dissipation = 3587,
            Aetherpact = 7437,
            FeyBlessing = 16543,

            // Other
            Aetherflow = 166,
            ChainStratagem = 7436,

            // Role
            Resurrection = 173,
            Esuna = 5768,
            LucidDreaming = 7562;

        public static class Buffs
        {
            public const ushort
            Placeholder = 1;
        }

        public static class Debuffs
        {
            public const ushort
            Bio1 = 179,
            Bio2 = 189,
            Biolysis = 1895,
            ChainStratagem = 1221;
        }

        public static class Levels
        {
            public const byte

                Bio1 = 2,
                Physick = 4,
                WhisperingDawn = 20,
                Bio2 = 26,
                Adloquium = 30,
                Succor = 35,
                Ruin2 = 38,
                FeyIllumination = 40,
                Aetherflow = 45,
                EnergyDrain = 45,
                Lustrate = 45,
                Scourge = 46,
                SacredSoil = 50,
                Indomitability = 52,
                Broil = 54,
                DeploymentTactics = 56,
                EmergencyTactics = 58,
                Dissipation = 60,
                Excogitation = 62,
                Broil2 = 64,
                ChainStratagem = 66,
                Aetherpact = 70,
                Biolysis = 72,
                Broil3 = 72,
                Recitation = 74,
                FeyBlessing = 76,
                SummonSeraph = 80,
                Broil4 = 82,
                Scoura = 82,
                Protraction = 86,
                Expedient = 90;
        }

        public static class Config
        {
            public const string
                ScholarLucidDreaming = "LucidScholar",
                ScholarFairy = "ScholarFairy";
        }
    }

    internal class ScholarSeraphConsolationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphConsolationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.FeyBlessing)
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarEnergyDrainFeature;

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
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchRaiseFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == All.Swiftcast)
                {
                    if (IsEnabled(CustomComboPreset.SchRaiseFeature))
                    {
                        if (HasEffect(All.Buffs.Swiftcast))
                            return SCH.Resurrection;
                    }

                    return OriginalHook(All.Swiftcast);
                }

                return actionID;
            }
        }
    }

    internal class SCHDPSAlternateFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHDPSAlternateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.Ruin2)
            {
                var gauge = GetJobGauge<SCHGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var biolysisDebuff = FindTargetEffect(SCH.Debuffs.Biolysis);
                var bio2Debuff = FindTargetEffect(SCH.Debuffs.Bio2);
                var bio1Debuff = FindTargetEffect(SCH.Debuffs.Bio1);

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= SCH.Levels.Biolysis)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Biolysis) && incombat && level >= SCH.Levels.Biolysis) || (biolysisDebuff.RemainingTime < 5 && incombat && level >= SCH.Levels.Biolysis))
                        return SCH.Biolysis;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= SCH.Levels.Bio2 && level < SCH.Levels.Biolysis)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio2) && level < SCH.Levels.Biolysis && level >= SCH.Levels.Bio2) || (bio2Debuff.RemainingTime < 5 && incombat && level >= SCH.Levels.Bio2 && level < SCH.Levels.Biolysis))
                        return SCH.Bio2;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level < SCH.Levels.Bio2)
                {
                    if ((!TargetHasEffect(SCH.Debuffs.Bio1) && level < SCH.Levels.Bio2) || (bio1Debuff.RemainingTime < 5 && incombat && level < SCH.Levels.Bio2))
                        return SCH.Bio1;
                }
            }
            return actionID;
        }
    }
    internal class ScholarFairyFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarFairyFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.WhisperingDawn || actionID == SCH.FeyBlessing || actionID == SCH.FeyBlessing || actionID == SCH.FeyIllumination || actionID == SCH.Dissipation || actionID == SCH.Aetherpact)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (!Service.BuddyList.PetBuddyPresent && gauge.SeraphTimer == 0)
                {
                    if ((Service.Configuration.GetCustomIntValue(SCH.Config.ScholarFairy)) == 2) return SCH.SummonSelene;
                    else return SCH.SummonEos;
                }
            }
            return actionID;
        }
    }

    internal class ScholarDPSFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SCH.Broil4 or SCH.Broil3 or SCH.Broil2 or SCH.Broil1 or SCH.Ruin1)
            {
                var actionIDCD = GetCooldown(actionID);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var biolysis = FindTargetEffect(SCH.Debuffs.Biolysis);
                var bio1 = FindTargetEffect(SCH.Debuffs.Bio1);
                var bio2 = FindTargetEffect(SCH.Debuffs.Bio2);
                var chainBuff = GetCooldown(SCH.ChainStratagem);
                var chainTarget = TargetHasEffect(SCH.Debuffs.ChainStratagem);

                if (IsEnabled(CustomComboPreset.ScholarLucidDPSFeature) && level >= All.Levels.LucidDreaming)
                {
                    var lucidMPThreshold = Service.Configuration.GetCustomIntValue(SCH.Config.ScholarLucidDreaming);
                    if ( IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidMPThreshold && CanSpellWeave(actionID) )
                        return All.LucidDreaming;
                }

                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= SCH.Levels.Biolysis && incombat)
                {
                    if ((biolysis is null) || (biolysis.RemainingTime <= 3))
                        return SCH.Biolysis;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= SCH.Levels.Bio2 && level < SCH.Levels.Biolysis && incombat)
                {
                    if ((bio2 is null) || (bio2.RemainingTime <= 3))
                        return SCH.Bio2;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= SCH.Levels.Bio1 && level < SCH.Levels.Bio2 && incombat)
                {
                    if ((bio1 is null) || (bio1.RemainingTime <= 3))
                        return SCH.Bio1;
                }
                if (IsEnabled(CustomComboPreset.ScholarDPSFeatureBuffOption) && level >= SCH.Levels.ChainStratagem)
                {
                    if (!chainBuff.IsCooldown && !chainTarget && actionIDCD.IsCooldown && incombat)
                        return SCH.ChainStratagem;
                }

            }
            return actionID;
        }
    }

}
