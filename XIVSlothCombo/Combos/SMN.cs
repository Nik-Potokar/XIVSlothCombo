using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SMN
    {
        public const byte ClassID = 15;
        public const byte JobID = 27;

        public const float CooldownThreshold = 0.5f;

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

            SummonCarbuncle = 25798,

            // summon abilities
            Gemshine = 25883,
            PreciousBrilliance = 25884,
            DreadwyrmTrance = 3581,

            // summon ruins
            RubyRuin1 = 25808,
            RubyRuin2 = 25811,
            RubyRuin3 = 25817,
            TopazRuin1 = 25809,
            TopazRuin2 = 25812,
            TopazRuin3 = 25818,
            EmeralRuin1 = 25810,
            EmeralRuin2 = 25813,
            EmeralRuin3 = 25819,

            // summon outbursts
            Outburst = 16511,
            RubyOutburst = 25814,
            TopazOutburst = 25815,
            EmeraldOutburst = 25816,

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
            AstralImpulse = 25820, // single target bahamut gcd
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
            Ruin = 163,
            Ruin2 = 172,
            Ruin3 = 3579,
            Ruin4 = 7426,
            Tridisaster = 25826,

            // summoner AoE
            RubyDisaster = 25827,
            TopazDisaster = 25828,
            EmeraldDisaster = 25829,

            // summoner ogcds
            EnergyDrain = 16508,
            Fester = 181,
            EnergySiphon = 16510,
            Painflare = 3578,

            // revive
            Resurrection = 173,

            // buff 
            Aethercharge = 25800,
            SearingLight = 25801,

            // other
            Sleep = 25880,
            Swiftcast = 7561;


        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                FurtherRuin = 2701,
                GarudasFavor = 2725,
                TitansFavor = 2853,
                IfritsFavor = 2724,
                EverlastingFlight = 16517,
                SearingLight = 2703;
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
                OutburstMastery2 = 82,
                Slipstream = 86,
                MountainBuster = 86,
                SearingLight = 66,

                Bahamut = 70,
                Phoenix = 80,

                // summoner ruins lvls
                RubyRuin1 = 22,
                RubyRuin2 = 30,
                RubyRuin3 = 54,
                TopazRuin1 = 22,
                TopazRuin2 = 30,
                TopazRuin3 = 54,
                EmeralRuin1 = 22,
                EmeralRuin2 = 30,
                EmeralRuin3 = 54;
        }

        public static class Config
        {
            public const string
                SMNLucidDreamingFeature = "SMNLucidDreamingFeature";
        }
    }
    internal class SummonerRaiseFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerRaiseFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Swiftcast)
            {
                var swiftCD = GetCooldown(SMN.Swiftcast);
                if (swiftCD.IsCooldown)
                    return SMN.Resurrection;
            }
            return actionID;
        }
    }


    internal class SummonerSpecialRuinFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerSpecialRuinFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin4)
            {
                var furtherRuin = HasEffect(SMN.Buffs.FurtherRuin);
                if (!furtherRuin)
                    return SMN.Ruin3;
            }
            return actionID;
        }
    }

    internal class SummonerEDFesterCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerEDFesterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Fester)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var furtherRuin = HasEffect(SMN.Buffs.FurtherRuin);
                var edrainCD = GetCooldown(SMN.EnergyDrain);
                if (furtherRuin && edrainCD.IsCooldown && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                    return SMN.Ruin4;
                if (level >= SMN.Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                    return SMN.EnergyDrain;
            }

            return actionID;
        }
    }

    internal class SummonerESPainflareCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerESPainflareCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Painflare)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var furtherRuin = HasEffect(SMN.Buffs.FurtherRuin);
                var energysyphonCD = GetCooldown(SMN.EnergySiphon);
                if (furtherRuin && energysyphonCD.IsCooldown && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                    return SMN.Ruin4;
                if (level >= SMN.Levels.EnergySyphon && !gauge.HasAetherflowStacks)
                    return SMN.EnergySiphon;

            }

            return actionID;
        }
    }

    internal class SummonerMainComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerMainComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin3 || actionID == SMN.Ruin2 || (actionID == SMN.Ruin))
            {
                var gauge = GetJobGauge<SMNGauge>();
                var furtheRuin = FindEffect(SMN.Buffs.FurtherRuin);
                var bahaCD = GetCooldown(SMN.SummonBahamut);
                var phoenixCD = GetCooldown(SMN.SummonPhoenix);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var buffCD = GetCooldown(SMN.SearingLight);
                var searingLight = HasEffect(SMN.Buffs.SearingLight);
                var swiftCD = GetCooldown(SMN.Swiftcast);
                var slipstreamCD = GetCooldown(SMN.Slipstream);
                var energyDrainCD = GetCooldown(SMN.EnergyDrain);
                var astralimpulseCD = GetCooldown(SMN.AstralImpulse);
                var fofCD = GetCooldown(SMN.FountainOfFire);
                var smnBahamut = GetCooldown(SMN.SummonBahamut);
                var smnPhoenix = GetCooldown(SMN.SummonPhoenix);
                var astralCD = GetCooldown(SMN.AstralImpulse);
                var deathflare = GetCooldown(SMN.Deathflare);
                var fountainfireCD = GetCooldown(SMN.FountainOfFire);
                var enkindleBahamut = GetCooldown(SMN.EnkindleBahamut);
                var enkindlePhoenix = GetCooldown(SMN.EnkindlePhoenix);
                var rekindle = GetCooldown(SMN.Rekindle);
                if (IsEnabled(CustomComboPreset.SummonerRuin4WastePrevention) && incombat)
                {
                    if (HasEffect(SMN.Buffs.FurtherRuin) && furtheRuin.RemainingTime > 0 && furtheRuin.RemainingTime <= 5 && gauge.SummonTimerRemaining == 0)
                    {
                        return SMN.Ruin4;
                    }
                }
                if (IsEnabled(CustomComboPreset.SimpleSummoner))
                {
                    if (IsEnabled(CustomComboPreset.BuffOnSimpleSummoner) && gauge.IsBahamutReady && !bahaCD.IsCooldown && !buffCD.IsCooldown && incombat && level >= SMN.Levels.SearingLight)
                        return SMN.SearingLight;

                    // Egis
                    if (gauge.IsTitanReady && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat)
                        return OriginalHook(SMN.SummonTopaz);
                    if (gauge.IsGarudaReady && !gauge.IsTitanAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonEmerald);
                    if (gauge.IsIfritReady && !gauge.IsGarudaAttuned && !gauge.IsTitanAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonRuby);
                    // Demi
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown && buffCD.IsCooldown && searingLight)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown && !searingLight)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SimpleSummonerOption2))
                {
                    if (IsEnabled(CustomComboPreset.BuffOnSimpleSummoner) && gauge.IsBahamutReady && !bahaCD.IsCooldown && !buffCD.IsCooldown && incombat && level >= SMN.Levels.SearingLight)
                        return SMN.SearingLight;

                    // Egis
                    if (gauge.IsGarudaReady && !gauge.IsTitanAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonEmerald);
                    if (gauge.IsTitanReady && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat)
                        return OriginalHook(SMN.SummonTopaz);
                    if (gauge.IsIfritReady && !gauge.IsGarudaAttuned && !gauge.IsTitanAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonRuby);

                    // Demi
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown && buffCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureGaruda))
                {
                    if (!swiftCD.IsCooldown && HasEffect(SMN.Buffs.GarudasFavor) && gauge.IsGarudaAttuned)
                        return SMN.Swiftcast;
                }
                if (IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureIfrit))
                {
                    if (!swiftCD.IsCooldown && gauge.IsIfritAttuned && lastComboMove == OriginalHook(SMN.RubyRite))
                        return SMN.Swiftcast;
                }
                if (IsEnabled(CustomComboPreset.SummonerDemiSummonsFeature))
                {
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SummonerLazyFesterFeature))
                {
                    if (lastComboMove == SMN.AstralImpulse && gauge.HasAetherflowStacks && astralimpulseCD.CooldownRemaining > 0.99)
                        return SMN.Fester;
                    if (lastComboMove == SMN.AstralImpulse && gauge.HasAetherflowStacks && astralimpulseCD.CooldownRemaining > 0.2)
                        return SMN.Fester;
                    if ((lastComboMove == SMN.AstralImpulse && !gauge.HasAetherflowStacks && !energyDrainCD.IsCooldown && astralimpulseCD.CooldownRemaining > 0.95) || (lastComboMove == SMN.FountainOfFire && !gauge.HasAetherflowStacks && !energyDrainCD.IsCooldown && astralimpulseCD.CooldownRemaining > 0.95))
                        return SMN.EnergyDrain;
                }
                if (IsEnabled(CustomComboPreset.SummonerSingleTargetDemiFeature))
                {
                    // Bahamut
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && !deathflare.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.99)
                        return SMN.Deathflare;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && !deathflare.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.3)
                        return SMN.Deathflare;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && deathflare.IsCooldown && !enkindleBahamut.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.99)
                        return SMN.EnkindleBahamut;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && deathflare.IsCooldown && !enkindleBahamut.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.3)
                        return SMN.EnkindleBahamut;
                    // Phoenix
                    if (level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !enkindlePhoenix.IsCooldown && fountainfireCD.CooldownRemaining > 0.99)
                        return SMN.EnkindlePhoenix;
                    if (level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !enkindlePhoenix.IsCooldown && fountainfireCD.CooldownRemaining > 0.3)
                        return SMN.EnkindlePhoenix;
                    if (IsEnabled(CustomComboPreset.SummonerRekindlePhoenix) && level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !rekindle.IsCooldown && fountainfireCD.CooldownRemaining > 0.3)
                        return SMN.Rekindle;
                }
                if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature))
                {
                    if (gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor))
                        return SMN.Slipstream;
                }
                if (IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature))
                {
                    if (HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazRite)
                        return SMN.MountainBuster;
                }
                if (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                {
                    if (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor))
                        return SMN.CrimsonCyclone;
                    if (gauge.IsIfritAttuned && !HasEffect(SMN.Buffs.IfritsFavor) && lastComboMove == SMN.CrimsonCyclone)
                        return SMN.CrimsonStrike;
                }
                if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature))
                {
                    if (gauge.IsGarudaAttuned && level >= 72)
                        return SMN.EmeraldRite;
                    if (gauge.IsTitanAttuned && level >= 72)
                        return SMN.TopazRite;
                    if (gauge.IsIfritAttuned && level >= 72)
                        return SMN.RubyRite;

                    // low level 54-71
                    if (gauge.IsGarudaAttuned && level >= 54)
                        return SMN.EmeralRuin3;
                    if (gauge.IsTitanAttuned && level >= 54)
                        return SMN.TopazRuin3;
                    if (gauge.IsIfritAttuned && level >= 54)
                        return SMN.RubyRuin3;


                    // low level 30-53
                    if (gauge.IsGarudaAttuned && level >= 30)
                        return SMN.EmeralRuin2;
                    if (gauge.IsTitanAttuned && level >= 30)
                        return SMN.TopazRuin2;
                    if (gauge.IsIfritAttuned && level >= 30)
                        return SMN.RubyRuin2;

                    // low level 1-29
                    if (gauge.IsGarudaAttuned)
                        return SMN.EmeralRuin1;
                    if (gauge.IsTitanAttuned)
                        return SMN.TopazRuin1;
                    if (gauge.IsIfritAttuned)
                        return SMN.RubyRuin1;
                }
                if (IsEnabled(CustomComboPreset.SummonerRuin4ToRuin3Feature) && incombat)
                {
                    if (level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                        return SMN.Ruin4;
                }
                if (IsEnabled(CustomComboPreset.SummonerCarbuncleSummonFeature))
                {
                    var carbyPresent = Service.BuddyList.PetBuddyPresent;
                    if (!carbyPresent && gauge.SummonTimerRemaining == 0 && gauge.Attunement == 0 && gauge.AttunmentTimerRemaining == 0)
                        return SMN.SummonCarbuncle;
                }
            }

            return actionID;
        }
    }

    internal class SummonerAOEComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerAOEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Tridisaster || actionID == SMN.Outburst)
            {
                var gauge = GetJobGauge<SMNGauge>();
                if (IsEnabled(CustomComboPreset.SimpleAoESummoner))
                {
                    var bahaCD = GetCooldown(SMN.SummonBahamut);
                    var phoenixCD = GetCooldown(SMN.SummonPhoenix);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var buffCD = GetCooldown(SMN.SearingLight);

                    if (IsEnabled(CustomComboPreset.BuffOnSimpleAoESummoner) && gauge.IsBahamutReady && !bahaCD.IsCooldown && !buffCD.IsCooldown && incombat && level >= SMN.Levels.SearingLight)
                        return SMN.SearingLight;

                    // Egis
                    if (gauge.IsTitanReady && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat)
                        return OriginalHook(SMN.SummonTopaz);
                    if (gauge.IsGarudaReady && !gauge.IsTitanAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonEmerald);
                    if (gauge.IsIfritReady && !gauge.IsGarudaAttuned && !gauge.IsTitanAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonRuby);

                    // Demi
                    if ((gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown && buffCD.IsCooldown) || gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);


                }
                if (IsEnabled(CustomComboPreset.SummonerDemiAoESummonsFeature))
                {
                    var bahaCD = GetCooldown(SMN.SummonBahamut);
                    var phoenixCD = GetCooldown(SMN.SummonPhoenix);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SummonerLazyFesterFeature))
                {
                    var energyDrainCD = GetCooldown(SMN.EnergyDrain);
                    var energySiphonCD = GetCooldown(SMN.EnergySiphon);
                    var astralimpulseCD = GetCooldown(SMN.AstralImpulse);
                    var astralflareCD = GetCooldown(SMN.AstralFlare);
                    var fofCD = GetCooldown(SMN.FountainOfFire);
                    if (lastComboMove == SMN.AstralFlare && gauge.HasAetherflowStacks && astralflareCD.CooldownRemaining > 0.99)
                        return SMN.Painflare;
                    if (lastComboMove == SMN.AstralFlare && gauge.HasAetherflowStacks && astralflareCD.CooldownRemaining > 0.2)
                        return SMN.Painflare;
                    if ((lastComboMove == SMN.AstralFlare && !gauge.HasAetherflowStacks && !energySiphonCD.IsCooldown && astralflareCD.CooldownRemaining > 0.95) || (lastComboMove == SMN.BrandOfPurgatory && !gauge.HasAetherflowStacks && !energySiphonCD.IsCooldown && astralflareCD.CooldownRemaining > 0.95))
                        return SMN.EnergySiphon;
                }
                if (IsEnabled(CustomComboPreset.SummonerAOEDemiFeature))
                {
                    var smnBahamut = GetCooldown(SMN.SummonBahamut);
                    var smnPhoenix = GetCooldown(SMN.SummonPhoenix);

                    var astralflareCD = GetCooldown(SMN.AstralFlare);
                    var deathflare = GetCooldown(SMN.Deathflare);
                    var brandofpurgaCD = GetCooldown(SMN.BrandOfPurgatory);
                    var enkindleBahamut = GetCooldown(SMN.EnkindleBahamut);
                    var enkindlePhoenix = GetCooldown(SMN.EnkindlePhoenix);
                    var rekindle = GetCooldown(SMN.Rekindle);


                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralFlare && !deathflare.IsCooldown && !deathflare.IsCooldown && astralflareCD.CooldownRemaining > 0.7)
                        return SMN.Deathflare;
                    if (IsEnabled(CustomComboPreset.SummonerRekindlePhoenix) && level >= SMN.Levels.Phoenix && lastComboMove == SMN.BrandOfPurgatory && !rekindle.IsCooldown && brandofpurgaCD.CooldownRemaining > SMN.CooldownThreshold)
                        return SMN.Rekindle;
                    if (level >= SMN.Levels.Bahamut && (lastComboMove == SMN.AstralFlare || lastComboMove == SMN.SummonBahamut) && !enkindleBahamut.IsCooldown && astralflareCD.CooldownRemaining > SMN.CooldownThreshold)
                        return SMN.EnkindleBahamut;
                    if (level > SMN.Levels.Phoenix && (lastComboMove == SMN.BrandOfPurgatory || lastComboMove == SMN.SummonPhoenix) && !enkindlePhoenix.IsCooldown && brandofpurgaCD.CooldownRemaining > SMN.CooldownThreshold)
                        return SMN.EnkindlePhoenix;
                }

                if (gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor) && IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature))
                    return SMN.Slipstream;
                if (HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazCata && IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature))
                    return SMN.MountainBuster;
                if (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor) && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonCyclone;
                if (gauge.IsIfritAttuned && !HasEffect(SMN.Buffs.IfritsFavor) && lastComboMove == SMN.CrimsonCyclone && IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                    return SMN.CrimsonStrike;

                if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature))
                {
                    // low level 1-29
                    if (gauge.IsGarudaAttuned && level >= 26 && level <= 73)
                        return SMN.EmeraldOutburst;
                    if (gauge.IsTitanAttuned && level >= 26 && level <= 73)
                        return SMN.TopazOutburst;
                    if (gauge.IsIfritAttuned && level >= 26 && level <= 73)
                        return SMN.RubyOutburst;

                    if (gauge.IsGarudaAttuned && level >= 82)
                        return SMN.EmeraldCata;
                    if (gauge.IsGarudaAttuned && level <= 81)
                        return SMN.EmeraldDisaster;
                    if (gauge.IsTitanAttuned && level >= 82)
                        return SMN.TopazCata;
                    if (gauge.IsTitanAttuned && level <= 81)
                        return SMN.TopazDisaster;
                    if (gauge.IsIfritAttuned && level >= 82)
                        return SMN.RubyCata;
                    if (gauge.IsIfritAttuned && level <= 81)
                        return SMN.RubyDisaster;
                }
                if (IsEnabled(CustomComboPreset.SummonerRuin4ToTridisasterFeature))
                {
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    if (level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin) && incombat)
                        return SMN.Ruin4;
                }
            }

            return actionID;
        }
    }
    internal class SummonerMainComboFeatureRuin1 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerMainComboFeatureRuin1;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SMN.Ruin)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var furtheRuin = FindEffect(SMN.Buffs.FurtherRuin);
                var bahaCD = GetCooldown(SMN.SummonBahamut);
                var phoenixCD = GetCooldown(SMN.SummonPhoenix);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var buffCD = GetCooldown(SMN.SearingLight);
                var searingLight = HasEffect(SMN.Buffs.SearingLight);
                var swiftCD = GetCooldown(SMN.Swiftcast);
                var slipstreamCD = GetCooldown(SMN.Slipstream);
                var energyDrainCD = GetCooldown(SMN.EnergyDrain);
                var astralimpulseCD = GetCooldown(SMN.AstralImpulse);
                var fofCD = GetCooldown(SMN.FountainOfFire);
                var smnBahamut = GetCooldown(SMN.SummonBahamut);
                var smnPhoenix = GetCooldown(SMN.SummonPhoenix);
                var astralCD = GetCooldown(SMN.AstralImpulse);
                var deathflare = GetCooldown(SMN.Deathflare);
                var fountainfireCD = GetCooldown(SMN.FountainOfFire);
                var enkindleBahamut = GetCooldown(SMN.EnkindleBahamut);
                var enkindlePhoenix = GetCooldown(SMN.EnkindlePhoenix);
                var rekindle = GetCooldown(SMN.Rekindle);
                if (IsEnabled(CustomComboPreset.SummonerRuin4WastePrevention) && incombat)
                {
                    if (HasEffect(SMN.Buffs.FurtherRuin) && furtheRuin.RemainingTime > 0 && furtheRuin.RemainingTime <= 5 && gauge.SummonTimerRemaining == 0)
                    {
                        return SMN.Ruin4;
                    }
                }
                if (IsEnabled(CustomComboPreset.SimpleSummoner))
                {
                    if (IsEnabled(CustomComboPreset.BuffOnSimpleSummoner) && gauge.IsBahamutReady && !bahaCD.IsCooldown && !buffCD.IsCooldown && incombat && level >= SMN.Levels.SearingLight)
                        return SMN.SearingLight;

                    // Egis
                    if (gauge.IsTitanReady && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat)
                        return OriginalHook(SMN.SummonTopaz);
                    if (gauge.IsGarudaReady && !gauge.IsTitanAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonEmerald);
                    if (gauge.IsIfritReady && !gauge.IsGarudaAttuned && !gauge.IsTitanAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonRuby);
                    // Demi
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown && buffCD.IsCooldown && searingLight)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown && !searingLight)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SimpleSummonerOption2))
                {
                    if (IsEnabled(CustomComboPreset.BuffOnSimpleSummoner) && gauge.IsBahamutReady && !bahaCD.IsCooldown && !buffCD.IsCooldown && incombat && level >= SMN.Levels.SearingLight)
                        return SMN.SearingLight;

                    // Egis
                    if (gauge.IsGarudaReady && !gauge.IsTitanAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonEmerald);
                    if (gauge.IsTitanReady && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat)
                        return OriginalHook(SMN.SummonTopaz);
                    if (gauge.IsIfritReady && !gauge.IsGarudaAttuned && !gauge.IsTitanAttuned && gauge.SummonTimerRemaining == 0 && bahaCD.IsCooldown && phoenixCD.IsCooldown && incombat && !HasEffect(SMN.Buffs.TitansFavor))
                        return OriginalHook(SMN.SummonRuby);

                    // Demi
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown && buffCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureGaruda))
                {
                    if (!swiftCD.IsCooldown && HasEffect(SMN.Buffs.GarudasFavor) && gauge.IsGarudaAttuned)
                        return SMN.Swiftcast;
                }
                if (IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureIfrit))
                {
                    if (!swiftCD.IsCooldown && gauge.IsIfritAttuned && lastComboMove == OriginalHook(SMN.RubyRite))
                        return SMN.Swiftcast;
                }
                if (IsEnabled(CustomComboPreset.SummonerDemiSummonsFeature))
                {
                    if (gauge.IsBahamutReady && !gauge.IsPhoenixReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !bahaCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                    if (gauge.IsPhoenixReady && !gauge.IsBahamutReady && gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && incombat && !phoenixCD.IsCooldown)
                        return OriginalHook(SMN.Aethercharge);
                }
                if (IsEnabled(CustomComboPreset.SummonerLazyFesterFeature))
                {
                    if (lastComboMove == SMN.AstralImpulse && gauge.HasAetherflowStacks && astralimpulseCD.CooldownRemaining > 0.99)
                        return SMN.Fester;
                    if (lastComboMove == SMN.AstralImpulse && gauge.HasAetherflowStacks && astralimpulseCD.CooldownRemaining > 0.2)
                        return SMN.Fester;
                    if ((lastComboMove == SMN.AstralImpulse && !gauge.HasAetherflowStacks && !energyDrainCD.IsCooldown && astralimpulseCD.CooldownRemaining > 0.95) || (lastComboMove == SMN.FountainOfFire && !gauge.HasAetherflowStacks && !energyDrainCD.IsCooldown && astralimpulseCD.CooldownRemaining > 0.95))
                        return SMN.EnergyDrain;
                }
                if (IsEnabled(CustomComboPreset.SummonerSingleTargetDemiFeature))
                {
                    // Bahamut
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && !deathflare.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.99)
                        return SMN.Deathflare;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && !deathflare.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.3)
                        return SMN.Deathflare;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && deathflare.IsCooldown && !enkindleBahamut.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.99)
                        return SMN.EnkindleBahamut;
                    if (level >= SMN.Levels.Bahamut && lastComboMove == SMN.AstralImpulse && deathflare.IsCooldown && !enkindleBahamut.IsCooldown && !gauge.HasAetherflowStacks && astralCD.CooldownRemaining > 0.3)
                        return SMN.EnkindleBahamut;
                    // Phoenix
                    if (level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !enkindlePhoenix.IsCooldown && fountainfireCD.CooldownRemaining > 0.99)
                        return SMN.EnkindlePhoenix;
                    if (level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !enkindlePhoenix.IsCooldown && fountainfireCD.CooldownRemaining > 0.3)
                        return SMN.EnkindlePhoenix;
                    if (IsEnabled(CustomComboPreset.SummonerRekindlePhoenix) && level >= SMN.Levels.Phoenix && lastComboMove == SMN.FountainOfFire && !rekindle.IsCooldown && fountainfireCD.CooldownRemaining > 0.3)
                        return SMN.Rekindle;
                }
                if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature))
                {
                    if (gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor))
                        return SMN.Slipstream;
                }
                if (IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature))
                {
                    if (HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazRite)
                        return SMN.MountainBuster;
                }
                if (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature))
                {
                    if (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor))
                        return SMN.CrimsonCyclone;
                    if (gauge.IsIfritAttuned && !HasEffect(SMN.Buffs.IfritsFavor) && lastComboMove == SMN.CrimsonCyclone)
                        return SMN.CrimsonStrike;
                }
                if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature))
                {
                    if (gauge.IsGarudaAttuned && level >= 72)
                        return SMN.EmeraldRite;
                    if (gauge.IsTitanAttuned && level >= 72)
                        return SMN.TopazRite;
                    if (gauge.IsIfritAttuned && level >= 72)
                        return SMN.RubyRite;

                    // low level 54-71
                    if (gauge.IsGarudaAttuned && level >= 54)
                        return SMN.EmeralRuin3;
                    if (gauge.IsTitanAttuned && level >= 54)
                        return SMN.TopazRuin3;
                    if (gauge.IsIfritAttuned && level >= 54)
                        return SMN.RubyRuin3;


                    // low level 30-53
                    if (gauge.IsGarudaAttuned && level >= 30)
                        return SMN.EmeralRuin2;
                    if (gauge.IsTitanAttuned && level >= 30)
                        return SMN.TopazRuin2;
                    if (gauge.IsIfritAttuned && level >= 30)
                        return SMN.RubyRuin2;

                    // low level 1-29
                    if (gauge.IsGarudaAttuned)
                        return SMN.EmeralRuin1;
                    if (gauge.IsTitanAttuned)
                        return SMN.TopazRuin1;
                    if (gauge.IsIfritAttuned)
                        return SMN.RubyRuin1;
                }
                if (IsEnabled(CustomComboPreset.SummonerRuin4ToRuin3Feature) && incombat)
                {
                    if (level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                        return SMN.Ruin4;
                }
                if (IsEnabled(CustomComboPreset.SummonerCarbuncleSummonFeature))
                {
                    var carbyPresent = Service.BuddyList.PetBuddyPresent;
                    if (!carbyPresent && gauge.SummonTimerRemaining == 0 && gauge.Attunement == 0 && gauge.AttunmentTimerRemaining == 0)
                        return SMN.SummonCarbuncle;
                }
            }

            return actionID;
        }
    }
}
