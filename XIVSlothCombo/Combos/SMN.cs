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
            RadiantAegis = 25799,
            Aethercharge = 25800,
            SearingLight = 25801,

            // other
            Sleep = 25880;


        public static class Buffs
        {
            public const ushort
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
                Aethercharge = 6,
                SummonRuby = 6,
                SummonTopaz = 15,
                SummonEmerald = 22,
                Painflare = 52,
                Ruin3 = 54,
                AstralFlow = 60,
                EnhancedEgiAssault = 74,
                Ruin4 = 62,
                EnergyDrain = 10,
                EnergySiphon = 52,
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
            public const string
                SummonerPrimalChoice = "SummonerPrimalChoice";
        }
    }
    internal class SummonerRaiseFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerRaiseFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.Swiftcast)
            {
                if (IsOnCooldown(All.Swiftcast))
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
                if (HasEffect(SMN.Buffs.FurtherRuin) && IsOnCooldown(SMN.EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
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
                if (HasEffect(SMN.Buffs.FurtherRuin) && IsOnCooldown(SMN.EnergySiphon) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                    return SMN.Ruin4;
                if (level >= SMN.Levels.EnergySiphon && !gauge.HasAetherflowStacks)
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
            var gauge = GetJobGauge<SMNGauge>();
            var summonerPrimalChoice = Service.Configuration.GetCustomIntValue(SMN.Config.SummonerPrimalChoice);

            if (actionID is SMN.Ruin or SMN.Ruin2)
            {
                if (InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SearingLightonRuinFeature) && IsOffCooldown(SMN.SearingLight) && level >= SMN.Levels.SearingLight && gauge.IsBahamutReady && GetCooldownRemainingTime(SMN.SummonBahamut) >= 55)
                            return SMN.SearingLight;
                        if (IsEnabled(CustomComboPreset.SummonerEDMainComboFeature))
                        {
                            if (gauge.HasAetherflowStacks && (IsNotEnabled(CustomComboPreset.SummonerEDPoolonMainFeature) ||
                                IsEnabled(CustomComboPreset.SummonerEDPoolonMainFeature) && HasEffect(SMN.Buffs.SearingLight)))
                                return SMN.Fester;
                            if (level >= SMN.Levels.EnergyDrain && !gauge.HasAetherflowStacks && IsOffCooldown(SMN.EnergyDrain))
                                return SMN.EnergyDrain;
                        }
                    }

                    // Egi Features
                    if (IsEnabled(CustomComboPreset.EgisOnRuinFeature))
                    {
                        if (IsOffCooldown(All.Swiftcast) && level >= All.Levels.Swiftcast &&
                            (IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureGaruda) && HasEffect(SMN.Buffs.GarudasFavor) && level >= SMN.Levels.Slipstream && gauge.IsGarudaAttuned || //Swiftcast Garuda
                            IsEnabled(CustomComboPreset.SummonerSwiftcastFeatureIfrit) && gauge.IsIfritAttuned && lastComboMove is SMN.RubyRuin1 or SMN.RubyRuin2 or SMN.RubyRuin3 or SMN.RubyRite && level >= SMN.Levels.RubyRuin1)) //Swiftcast Ifrit
                            return All.Swiftcast;

                        if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) && gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor) || //Garuda
                            IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature) && HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazRite && CanSpellWeave(actionID) || //Titan
                            IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == SMN.CrimsonCyclone)) //Ifrit
                            return OriginalHook(SMN.AstralFlow);

                        if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature) && (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                            return OriginalHook(SMN.Gemshine);

                        if (IsEnabled(CustomComboPreset.SummonerEgiSummonsonMainFeature) && gauge.SummonTimerRemaining == 0 && IsOnCooldown(SMN.SummonPhoenix) && IsOnCooldown(SMN.SummonBahamut))
                        {
                            if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && level >= SMN.Levels.SummonRuby)
                                return OriginalHook(SMN.SummonRuby);
                            if (summonerPrimalChoice == 1)
                            {
                                if (gauge.IsTitanReady && level >=SMN.Levels.SummonTopaz)
                                    return OriginalHook(SMN.SummonTopaz);
                                if (gauge.IsGarudaReady && level >= SMN.Levels.SummonEmerald)
                                    return OriginalHook(SMN.SummonEmerald);
                            }

                            if (summonerPrimalChoice == 2)
                            {
                                if (gauge.IsGarudaReady && level >= SMN.Levels.SummonEmerald)
                                    return OriginalHook(SMN.SummonEmerald);
                                if (gauge.IsTitanReady && level >= SMN.Levels.SummonTopaz)
                                    return OriginalHook(SMN.SummonTopaz);
                            }
                        }
                    }

                    //Demi Features
                    if (IsEnabled(CustomComboPreset.SummonerDemiSummonsFeature))
                    {
                        if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(SMN.Aethercharge)) &&
                            (level >= SMN.Levels.Aethercharge && level < SMN.Levels.Bahamut || //Pre Bahamut Phase
                            gauge.IsBahamutReady  && level >= SMN.Levels.Bahamut || //Bahamut Phase
                            gauge.IsPhoenixReady  && level >= SMN.Levels.Phoenix)) //Phoenix Phase
                            return OriginalHook(SMN.Aethercharge);

                        if (IsEnabled(CustomComboPreset.SummonerSingleTargetDemiFeature) && CanSpellWeave(actionID))
                        {
                            if (IsOffCooldown(OriginalHook(SMN.AstralFlow)) && level >= SMN.Levels.AstralFlow && (level < SMN.Levels.Bahamut || lastComboMove is SMN.AstralImpulse))
                                return OriginalHook(SMN.AstralFlow);
                            if (IsOffCooldown(OriginalHook(SMN.EnkindleBahamut)) && level >= SMN.Levels.Bahamut && lastComboMove is SMN.AstralImpulse or SMN.FountainOfFire)
                                return OriginalHook(SMN.EnkindleBahamut); 
                        }

                        if (IsEnabled(CustomComboPreset.SummonerSingleTargetRekindleOption))
                        {
                            if (IsOffCooldown(OriginalHook(SMN.AstralFlow)) && lastComboMove is SMN.FountainOfFire)
                                return OriginalHook(SMN.AstralFlow);
                        }
                    }
                    
                    if (IsEnabled(CustomComboPreset.SummonerRuin4ToRuin3Feature) && level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                        return SMN.Ruin4;
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
            var gauge = GetJobGauge<SMNGauge>();

            if (actionID is SMN.Tridisaster or SMN.Outburst)
            {
                if (InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.BuffOnSimpleAoESummoner) && IsOffCooldown(SMN.SearingLight) && level >= SMN.Levels.SearingLight && gauge.IsBahamutReady && GetCooldownRemainingTime(SMN.SummonBahamut) >= 55)
                            return SMN.SearingLight;
                        if (IsEnabled(CustomComboPreset.SummonerESAOEFeature))
                        {
                            if (gauge.HasAetherflowStacks && HasEffect(SMN.Buffs.SearingLight))
                                return SMN.Painflare;
                            if (level >= SMN.Levels.EnergySiphon && !gauge.HasAetherflowStacks && IsOffCooldown(SMN.EnergySiphon))
                                return SMN.EnergySiphon;
                        }
                    }

                    // Egis
                    if (IsEnabled(CustomComboPreset.EgisOnAOEFeature))
                    {
                        if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) && gauge.IsGarudaAttuned && HasEffect(SMN.Buffs.GarudasFavor) || //Garuda
                            IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature) && HasEffect(SMN.Buffs.TitansFavor) && lastComboMove == SMN.TopazCata && CanSpellWeave(actionID) || //Titan
                            IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && (gauge.IsIfritAttuned && HasEffect(SMN.Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == SMN.CrimsonCyclone)) //Ifrit
                            return OriginalHook(SMN.AstralFlow);

                        if (IsEnabled(CustomComboPreset.SummonerEgiAttacksAOEFeature) && (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                            return OriginalHook(SMN.PreciousBrilliance);

                        if (gauge.SummonTimerRemaining == 0 && IsOnCooldown(SMN.SummonPhoenix) && IsOnCooldown(SMN.SummonBahamut))
                        {
                            if (gauge.IsTitanReady && level >= SMN.Levels.SummonTopaz)
                                return OriginalHook(SMN.SummonTopaz);
                            if (gauge.IsGarudaReady && level >= SMN.Levels.SummonEmerald)
                                return OriginalHook(SMN.SummonEmerald);
                            if (gauge.IsIfritReady && level >= SMN.Levels.SummonRuby)
                                return OriginalHook(SMN.SummonRuby);
                        }
                    }

                    //Demi
                    if (IsEnabled(CustomComboPreset.SummonerDemiAoESummonsFeature))
                    {
                        if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(SMN.Aethercharge)) &&
                            (level >= SMN.Levels.Aethercharge && level < SMN.Levels.Bahamut || //Pre Bahamut Phase
                            gauge.IsBahamutReady && level >= SMN.Levels.Bahamut || //Bahamut Phase
                            gauge.IsPhoenixReady && level >= SMN.Levels.Phoenix)) //Phoenix Phase
                            return OriginalHook(SMN.Aethercharge);

                        if (IsEnabled(CustomComboPreset.SummonerAOEDemiFeature) && CanSpellWeave(actionID))
                        {
                            if (IsOffCooldown(OriginalHook(SMN.AstralFlow)) && level >= SMN.Levels.AstralFlow && (level < SMN.Levels.Bahamut || lastComboMove is SMN.AstralFlare))
                                return OriginalHook(SMN.AstralFlow);
                            if (IsOffCooldown(OriginalHook(SMN.EnkindleBahamut)) && level >= SMN.Levels.Bahamut && lastComboMove is SMN.AstralFlare or SMN.BrandOfPurgatory)
                                return OriginalHook(SMN.EnkindleBahamut);
                        }
                        
                        if (IsEnabled(CustomComboPreset.SummonerAOETargetRekindleOption))
                        {
                            if (IsOffCooldown(OriginalHook(SMN.AstralFlow)) && lastComboMove is SMN.BrandOfPurgatory)
                                return OriginalHook(SMN.AstralFlow);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SummonerRuin4ToTridisasterFeature) && level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                        return SMN.Ruin4;
                }
            }

            return actionID;
        }
    }

    internal class SummonerCarbuncleSummonFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SummonerCarbuncleSummonFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<SMNGauge>();
            if (actionID is SMN.Ruin or SMN.Ruin2 or SMN.Ruin3 or SMN.DreadwyrmTrance or SMN.AstralFlow or SMN.EnkindleBahamut or SMN.SearingLight or SMN.RadiantAegis or SMN.Outburst or SMN.Tridisaster or SMN.PreciousBrilliance or SMN.Gemshine)
            {
                if (!HasPetPresent() && gauge.SummonTimerRemaining == 0 && gauge.Attunement == 0)
                    return SMN.SummonCarbuncle;
            }

            return actionID;
        }
    }
}
