using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SMN
    {
        public const byte ClassID = 15;
        public const byte JobID = 27;

        public const uint
            Deathflare = 3582,
            EnkindlePhoenix = 16516,
            EnkindleBahamut = 7429,
            EnkindleInferno = 16803,
            DreadwyrmTrance = 3581,
            SummonBahamut = 7427,
            FirebirdTranceLow = 16513,
            FirebirdTranceHigh = 16549,
            Ruin1 = 163,
            Ruin3 = 3579,
            Ruin4 = 7426,
            BrandOfPurgatory = 16515,
            FountainOfFire = 16514,
            Fester = 181,
            EnergyDrain = 16508,
            Painflare = 3578,
            Miasma3 = 7425,
            Bio3 = 7424,
            Psysick = 16230,
            Resurrection = 173,
            EnergySyphon = 16510;


        public static class Buffs
        {
            public const short
                FurtherRuin = 1212,
                HellishConduit = 1867;

        }

        public static class Debuffs
        {
            public const short
            Miasma3 = 1327,
            Bio3 = 1326;
        }

        public static class Levels
        {
            public const byte
                Painflare = 52,
                Ruin3 = 54,
                EnhancedFirebirdTrance = 80;
        }
    }

    internal class SummonerDemiCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerDemiCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // Replace Deathflare with demi enkindles
            if (actionID == SMN.Deathflare)
            {
                var gauge = GetJobGauge<SMNGauge>();

                if (gauge.IsPhoenixReady)
                    return SMN.EnkindlePhoenix;

                if (gauge.TimerRemaining > 0 && gauge.ReturnSummon != SummonPet.NONE)
                    return SMN.EnkindleBahamut;

                return actionID;
            }

            // Replace DWT with demi summons
            if (actionID == SMN.DreadwyrmTrance)
            {
                var gauge = GetJobGauge<SMNGauge>();

                 if (IsEnabled(CustomComboPreset.SummonerDemiComboUltra) && gauge.TimerRemaining > 0)
                 {
                    if (gauge.IsPhoenixReady)
                         return SMN.EnkindlePhoenix;
                
                     if (gauge.ReturnSummon != SummonPet.NONE)
                         return SMN.EnkindleBahamut;
                
                     return SMN.Deathflare;
                 }

                if (gauge.IsBahamutReady)
                    return SMN.SummonBahamut;

                if (gauge.IsPhoenixReady)
                    return OriginalHook(SMN.FirebirdTranceLow);

                return actionID;
            }

            return actionID;
        }
    }

    internal class SummonerBoPCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerBoPCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin1 || actionID == SMN.Ruin3)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (gauge.TimerRemaining > 0 && gauge.IsPhoenixReady)
                {
                    if (HasEffect(SMN.Buffs.HellishConduit))
                        return SMN.BrandOfPurgatory;

                    return SMN.FountainOfFire;
                }

                return OriginalHook(SMN.Ruin3);
            }

            return actionID;
        }
    }

    internal class SummonerEDFesterCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerEDFesterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Fester)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (!gauge.HasAetherflowStacks)
                    return SMN.EnergyDrain;
            }

            return actionID;
        }
    }

    internal class SummonerESPainflareCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerESPainflareCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Painflare)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (!gauge.HasAetherflowStacks)
                    return SMN.EnergySyphon;

                if (level >= SMN.Levels.Painflare)
                    return SMN.Painflare;

                return SMN.EnergySyphon;
            }

            return actionID;
        }
    }

    internal class SummonerEasyRotation : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerEasyRotation;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin3)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (gauge.TimerRemaining >= 18000 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
                if (gauge.TimerRemaining >= 17000)
                    return SMN.EnkindleBahamut;
                if (gauge.TimerRemaining >= 16000)
                    return SMN.Ruin3;
                if (gauge.TimerRemaining >= 13000)
                    return SMN.Ruin3;
                if (gauge.TimerRemaining >= 10000)
                    return SMN.Ruin3;
                if (gauge.TimerRemaining >= 7000)
                    return SMN.Ruin3;
                if (gauge.TimerRemaining >= 5000 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
                if (gauge.TimerRemaining >= 4000)
                    return SMN.EnkindleBahamut;
                if (gauge.TimerRemaining >= 3000 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
                if (gauge.TimerRemaining >= 1000 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
            }
            return actionID;
        }
        
    }

}

