using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SMN
    {
        public const byte ClassID = 15;
        public const byte JobID = 27;

        public const float cooldownThreshold = 0.5f;

        public const uint
            // summons
            SummonRuby = 25802,
            SummonTopaz = 25803,
            SummonEmerald = 25804,

            SummonIfrit = 25805,
            SummonTitan = 25806,
            SummonGaruda = 25807,

            SummonIfrit2 = 25838,
            SummonTitan2 = 25839,
            SummonGaruda2 = 25840,

            // summon abilities
            Gemshine = 25883,
            PreciousBrilliance = 25884,

            // summon single targets
            RubyRite = 25823,
            TopazRite = 25824,
            EmeraldRite = 25825,

            // summon aoes
            RubyCata = 25832,
            TopazCata = 25833,
            EmeraldCata = 25834,

            // summon astral flows
            CrimsonCyclone = 25835, // dash
            CrimsonStrike = 25885, // melee
            MountainBuster = 25836,
            Slipstream = 25837,




            // demisummons
            SummonBahamut = 7427,
            SummonPhoenix = 25831,

            // demisummon abilities
            AstralImpule = 25820, // single target bahamut gcd
            AstralFlare = 25821, // aoe bahamut gcd
            Deathflare = 3582, // damage ogcd bahamut
            EnkindleBahamut = 7429,

            FountainOfFire = 16514, // single target phoenix gcd
            BrandOfPurgatory = 16515, // aoe phoenix gcd
            Rekindle = 25830, // healing ogcd phoenix
            EnkindlePhoenix = 16516,

            // shared summon abilities
            AstralFlow = 25822,

            // summoner gcds
            Ruin3 = 172,
            Ruin4 = 7426,
            Tridisaster = 16511,



            // summoner ogcds
            EnergyDrain = 16508,
            Fester = 181,
            EnergySyphon = 16510,
            Painflare = 3578,

            // revive
            Resurrection = 173;



        public static class Buffs
        {
            public const short
                FurtherRuin = 2701,
                GarudasFavor = 2725,
                TitansFavor = 2853,
                IfritsFavor = 2724;

        }

        public static class Levels
        {
            public const byte
                Painflare = 52,
                Ruin3 = 54,
                EnhancedEgiAssault = 74,
                Ruin4 = 62,
                EnergyDrain = 10,
                EnergySyphon = 52,
                EnhancedFirebirdTrance = 80,
                Slipstream = 86,
                MountainBuster = 86;
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
                var furtherRuin = HasEffect(SMN.Buffs.FurtherRuin);
                var edrainCD = GetCooldown(SMN.EnergyDrain);
                if (level >= SMN.Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                    return SMN.EnergyDrain;
                if (furtherRuin && edrainCD.IsCooldown && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                    return SMN.Ruin4;
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
                var furtherRuin = HasEffect(SMN.Buffs.FurtherRuin);
                var energysyphonCD = GetCooldown(SMN.EnergySyphon);
                if (level >= SMN.Levels.EnergySyphon && !gauge.HasAetherflowStacks)
                    return SMN.EnergySyphon;
                if (furtherRuin && energysyphonCD.IsCooldown && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                    return SMN.Ruin4;
            }
            return actionID;
        }
    }


    internal class SummonerMainComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerMainComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin3)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (IsEnabled(CustomComboPreset.SummonerSingleTargetDemiFeature))
                {
                    var astralcD = GetCooldown(SMN.AstralImpule);
                    var deathflare = GetCooldown(SMN.Deathflare);
                    var fountainfireCD = GetCooldown(SMN.FountainOfFire);
                    var enkindleBahamut = GetCooldown(SMN.EnkindleBahamut);
                    var enkindlePhoenix = GetCooldown(SMN.EnkindlePhoenix);
                    var rekindle = GetCooldown(SMN.Rekindle);

                    if (lastComboMove == SMN.AstralImpule && !deathflare.IsCooldown && astralcD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.Deathflare;
                    else if (lastComboMove == SMN.AstralImpule && !enkindleBahamut.IsCooldown && astralcD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.EnkindleBahamut;
                    else if (lastComboMove == SMN.FountainOfFire && !enkindlePhoenix.IsCooldown && fountainfireCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.EnkindlePhoenix;
                    else if (lastComboMove == SMN.FountainOfFire && !rekindle.IsCooldown && fountainfireCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.Rekindle;
                }

                if (gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor) && IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature))
                    return SMN.Slipstream;
                else if (HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazRite && IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature))
                    return SMN.MountainBuster;
                else if (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor) && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonCyclone;
                else if (gauge.IsIfritAttuned && !HasEffect(SMN.Buffs.IfritsFavor) && lastComboMove == SMN.CrimsonCyclone && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonStrike;

                if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature))
                {
                    if (gauge.IsGarudaAttuned)
                        return SMN.EmeraldRite;
                    else if (gauge.IsTitanAttuned)
                        return SMN.TopazRite;
                    else if (gauge.IsIfritAttuned)
                        return SMN.RubyRite;
                }
            }
            return actionID;
        }
    }
    internal class SummonerAOEComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SummonerAOEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Tridisaster)
            {
                var gauge = GetJobGauge<SMNGauge>();

                if (IsEnabled(CustomComboPreset.SummonerAOEDemiFeature))
                {
                    var astralflareCD = GetCooldown(SMN.AstralFlare);
                    var deathflare = GetCooldown(SMN.Deathflare);
                    var brandofpurgaCD = GetCooldown(SMN.BrandOfPurgatory);
                    var enkindleBahamut = GetCooldown(SMN.EnkindleBahamut);
                    var enkindlePhoenix = GetCooldown(SMN.EnkindlePhoenix);
                    var rekindle = GetCooldown(SMN.Rekindle);

                    if (lastComboMove == SMN.AstralFlare && !deathflare.IsCooldown && astralflareCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.Deathflare;
                    else if (lastComboMove == SMN.AstralFlare && !enkindleBahamut.IsCooldown && astralflareCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.EnkindleBahamut;
                    else if (lastComboMove == SMN.BrandOfPurgatory && !enkindlePhoenix.IsCooldown && brandofpurgaCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.EnkindlePhoenix;
                    else if (lastComboMove == SMN.BrandOfPurgatory && !rekindle.IsCooldown && brandofpurgaCD.CooldownRemaining > SMN.cooldownThreshold)
                        return SMN.Rekindle;
                }

                if (gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor) && IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature))
                    return SMN.Slipstream;
                else if (HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazCata && IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature))
                    return SMN.MountainBuster;
                else if (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor) && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonCyclone;
                else if (gauge.IsIfritAttuned && !HasEffect(SMN.Buffs.IfritsFavor) && lastComboMove == SMN.CrimsonCyclone && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonStrike;

                if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature))
                {
                    if (gauge.IsGarudaAttuned)
                        return SMN.EmeraldCata;
                    else if (gauge.IsTitanAttuned)
                        return SMN.TopazCata;
                    else if (gauge.IsIfritAttuned)
                        return SMN.RubyCata;
                }
            }
            return actionID;
        }
    }
}