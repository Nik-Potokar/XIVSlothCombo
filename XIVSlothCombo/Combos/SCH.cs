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
    

    internal class ScholarSeraphConsolationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphConsolationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == FeyBlessing)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (gauge.SeraphTimer > 0)
                    return Consolation;
            }

            return actionID;
        }
    }

    internal class ScholarEnergyDrainFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarEnergyDrainFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == EnergyDrain)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (gauge.Aetherflow == 0)
                    return Aetherflow;
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
                            return Resurrection;
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
            if (actionID == Ruin2)
            {
                var gauge = GetJobGauge<SCHGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var biolysisDebuff = FindTargetEffect(Debuffs.Biolysis);
                var bio2Debuff = FindTargetEffect(Debuffs.Bio2);
                var bio1Debuff = FindTargetEffect(Debuffs.Bio1);

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= Levels.Biolysis)
                {
                    if ((!TargetHasEffect(Debuffs.Biolysis) && incombat && level >= Levels.Biolysis) || (biolysisDebuff?.RemainingTime < 5 && incombat && level >= Levels.Biolysis))
                        return Biolysis;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level >= Levels.Bio2 && level < Levels.Biolysis)
                {
                    if ((!TargetHasEffect(Debuffs.Bio2) && level < Levels.Biolysis && level >= Levels.Bio2) || (bio2Debuff?.RemainingTime < 5 && incombat && level >= Levels.Bio2 && level < Levels.Biolysis))
                        return Bio2;
                }

                if (IsEnabled(CustomComboPreset.SCHDPSAlternateFeature) && level < Levels.Bio2)
                {
                    if ((!TargetHasEffect(Debuffs.Bio1) && level < Levels.Bio2) || (bio1Debuff?.RemainingTime < 5 && incombat && level < Levels.Bio2))
                        return Bio1;
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
            if (actionID == WhisperingDawn || actionID == FeyBlessing || actionID == FeyBlessing || actionID == FeyIllumination || actionID == Dissipation || actionID == Aetherpact)
            {
                var gauge = GetJobGauge<SCHGauge>();
                if (!Service.BuddyList.PetBuddyPresent && gauge.SeraphTimer == 0)
                {
                    if ((Service.Configuration.GetCustomIntValue(Config.ScholarFairy)) == 2) return SummonSelene;
                    else return SummonEos;
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
                if (actionID is Broil4 or Broil3 or Broil2 or Broil1 or Ruin1)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var biolysis = FindTargetEffect(Debuffs.Biolysis);
                    var bio1 = FindTargetEffect(Debuffs.Bio1);
                    var bio2 = FindTargetEffect(Debuffs.Bio2);
                    var chainBuff = GetCooldown(ChainStratagem);
                    var chainTarget = TargetHasEffect(Debuffs.ChainStratagem);

                    if (IsEnabled(CustomComboPreset.ScholarLucidDPSFeature) && level >= All.Levels.LucidDreaming)
                    {
                        var lucidMPThreshold = Service.Configuration.GetCustomIntValue(Config.ScholarLucidDreaming);
                        if (IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidMPThreshold && CanSpellWeave(actionID))
                            return All.LucidDreaming;
                    }

                    if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= Levels.Biolysis && incombat)
                    {
                        if ((biolysis is null) || (biolysis?.RemainingTime <= 3))
                            return Biolysis;
                    }
                    if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= Levels.Bio2 && level < Levels.Biolysis && incombat)
                    {
                        if ((bio2 is null) || (bio2?.RemainingTime <= 3))
                            return Bio2;
                    }
                    if (IsEnabled(CustomComboPreset.ScholarDPSFeature) && level >= Levels.Bio1 && level < Levels.Bio2 && incombat)
                    {
                        if ((bio1 is null) || (bio1?.RemainingTime <= 3))
                            return Bio1;
                    }
                    if (IsEnabled(CustomComboPreset.ScholarDPSFeatureBuffOption) && level >= Levels.ChainStratagem)
                    {
                        if (!chainBuff.IsCooldown && !chainTarget && actionIDCD.IsCooldown && incombat)
                            return ChainStratagem;
                    }

                }
                return actionID;
            }
        }
    }

}
