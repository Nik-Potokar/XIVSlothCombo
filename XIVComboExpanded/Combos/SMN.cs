using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SMN
    {
        public const byte ClassID = 15;
        public const byte JobID = 27;

        public const uint
            Gemshine = 25883,
            EnergyDrain = 16508,
            Fester = 181,
            Resurrection = 173,
            SummonTopaz = 25803,
            SummonEmerald = 25804,
            SummonRuby = 25802,
            PreciousBrilliance = 25884,
            Ruin3 = 3579,
            Ruin4 = 7426,
            DreadwyrmTrance = 3581,
            AstralFlow = 25822,
            SummonBahamut = 7427,
            SummonPhoenix = 25831,
            EnkindleBahamut = 7429,
            EnkindlePhoenix = 16516,
            Deathflare = 3582,
            Painflare = 3578,
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
            Miasma3 = 1215,
            Bio3 = 1214;
        }

        public static class Levels
        {
            public const byte
                Painflare = 52,
                Ruin3 = 54,
                EnhancedEgiAssault = 74,
                EnhancedFirebirdTrance = 80;
        }
    }

  

    internal class SummonerEDFesterCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerEDFesterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.EnergyDrain)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (gauge.HasAetherflowStacks)
                    return SMN.Fester;
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
        internal class SummonerBahamutPhoenixFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.SummonerBahamutPhoenixFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SMN.Ruin3)
                {
                    var summonBahamutCD = GetCooldown(SMN.SummonBahamut);
                    var summonPhoenixCD = GetCooldown(SMN.SummonPhoenix);
                    var deathFlareCD = GetCooldown(SMN.Deathflare);
                    var gauge = GetJobGauge<SMNGauge>();
                    var ruin3CD = GetCooldown(SMN.Ruin3);
                    var enkindleBahamutCD = GetCooldown(SMN.EnkindleBahamut);
                    if (IsEnabled(CustomComboPreset.SummonnerTesting))
                    {
                        if (!summonBahamutCD.IsCooldown && gauge.IsBahamutReady)
                            return SMN.SummonBahamut;
                        if (!deathFlareCD.IsCooldown && summonBahamutCD.IsCooldown && lastComboMove == SMN.SummonBahamut)
                            return SMN.Deathflare;
                        if (ruin3CD.CooldownRemaining > 0.7 && !enkindleBahamutCD.IsCooldown && !deathFlareCD.IsCooldown && lastComboMove == SMN.Deathflare)
                            return SMN.EnkindleBahamut;
                    }
                    return OriginalHook(SMN.Ruin3);
                }
                return actionID;
            }
        }
    }
}

