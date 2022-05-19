using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SMN
    {
        public const byte ClassID = 26;
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
            SearingLight = 25801;


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
                RubyRuin1 = 6,
                RubyRuin2 = 30,
                RubyRuin3 = 54,
                TopazRuin1 = 15,
                TopazRuin2 = 30,
                TopazRuin3 = 54,
                EmeralRuin1 = 22,
                EmeralRuin2 = 30,
                EmeralRuin3 = 54;
        }

        public static class Config
        {
            public const string
                SMNLucidDreamingFeature = "SMNLucidDreamingFeature",
                SMNSearingLightChoice = "SMNSearingLightChoice",
                SummonerBurstPhase = "SummonerBurstPhase",
                SummonerPrimalChoice = "SummonerPrimalChoice",
                SummonerSwiftcastPhase = "SummonerSwiftcastPhase";
        }

        internal class SummonerRaiseFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerRaiseFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == All.Swiftcast)
                {
                    if (IsOnCooldown(All.Swiftcast))
                        return Resurrection;
                }
                return actionID;
            }
        }

        internal class SummonerSpecialRuinFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerSpecialRuinFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Ruin4)
                {
                    var furtherRuin = HasEffect(Buffs.FurtherRuin);
                    if (!furtherRuin)
                        return Ruin3;
                }
                return actionID;
            }
        }

        internal class SummonerEDFesterCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerEDFesterCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Fester)
                {
                    var gauge = GetJobGauge<SMNGauge>();
                    if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                        return Ruin4;
                    if (level >= Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                        return EnergyDrain;
                }

                return actionID;
            }
        }

        internal class SummonerESPainflareCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerESPainflareCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Painflare)
                {
                    var gauge = GetJobGauge<SMNGauge>();
                    if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SummonerFesterPainflareRuinFeature))
                        return Ruin4;
                    if (level >= Levels.EnergySiphon && !gauge.HasAetherflowStacks)
                        return EnergySiphon;

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
                var summonerPrimalChoice = Service.Configuration.GetCustomIntValue(Config.SummonerPrimalChoice);
                var SummonerBurstPhase = Service.Configuration.GetCustomIntValue(Config.SummonerBurstPhase);
                var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.SMNLucidDreamingFeature);
                var swiftcasePhase = Service.Configuration.GetCustomIntValue(Config.SummonerSwiftcastPhase);

                if (actionID is Ruin or Ruin2 && InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        // Searing Light
                        if (IsEnabled(CustomComboPreset.SearingLightonRuinFeature) && IsOffCooldown(SearingLight) && level >= Levels.SearingLight)
                        {
                            if (IsEnabled(CustomComboPreset.SummonerSearingLightBurstOption))
                            {
                                if ((SummonerBurstPhase == 1 && OriginalHook(Ruin) == AstralImpulse) ||
                                    (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                    (SummonerBurstPhase == 3 && OriginalHook(Ruin) is AstralImpulse or FountainOfFire) ||
                                    (SummonerBurstPhase == 4))
                                    return SearingLight;
                            }

                            else return SearingLight;
                        }

                        // ED & Fester
                        if (IsEnabled(CustomComboPreset.SummonerEDMainComboFeature))
                        {
                            if (gauge.HasAetherflowStacks)
                            {
                                if (IsNotEnabled(CustomComboPreset.SummonerEDPoolonMainFeature))
                                    return Fester;
                                if (IsEnabled(CustomComboPreset.SummonerEDPoolonMainFeature))
                                {
                                    if (level < Levels.SearingLight)
                                        return Fester;
                                    if ((SummonerBurstPhase == 1 && OriginalHook(Ruin) == AstralImpulse) ||
                                        (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                        (SummonerBurstPhase == 3 && (GetCooldownRemainingTime(SearingLight) < 30 || GetCooldownRemainingTime(SearingLight) > 100) && OriginalHook(Ruin) is AstralImpulse or FountainOfFire) ||
                                        (SummonerBurstPhase == 4 && HasEffectAny(Buffs.SearingLight) && !HasEffect(Buffs.TitansFavor)) ||
                                        IsNotEnabled(CustomComboPreset.SummonerPrimalBurstChoice))
                                        return Fester;
                                }
                            }

                            if (level >= Levels.EnergyDrain && !gauge.HasAetherflowStacks && IsOffCooldown(EnergyDrain))
                                return EnergyDrain;
                        }

                        // Lucid
                        if (IsEnabled(CustomComboPreset.SMNLucidDreamingFeature) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                            return All.LucidDreaming;
                    }

                    //Demi Features
                    if (IsEnabled(CustomComboPreset.SummonerDemiSummonsFeature))
                    {
                        if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                            (level is >= Levels.Aethercharge and < Levels.Bahamut || //Pre Bahamut Phase
                             gauge.IsBahamutReady && level >= Levels.Bahamut || //Bahamut Phase
                             gauge.IsPhoenixReady && level >= Levels.Phoenix)) //Phoenix Phase
                            return OriginalHook(Aethercharge);

                        if (IsEnabled(CustomComboPreset.SummonerSingleTargetDemiFeature) && CanSpellWeave(actionID))
                        {
                            if (IsOffCooldown(OriginalHook(AstralFlow)) && 
                                level >= Levels.AstralFlow && (level < Levels.Bahamut || lastComboMove is AstralImpulse))
                                return OriginalHook(AstralFlow);

                            if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && 
                                level >= Levels.Bahamut && lastComboMove is AstralImpulse or FountainOfFire)
                                return OriginalHook(EnkindleBahamut);
                        }

                        if (IsEnabled(CustomComboPreset.SummonerSingleTargetRekindleOption))
                        {
                            if (IsOffCooldown(Rekindle) && lastComboMove is FountainOfFire)
                                return OriginalHook(AstralFlow);
                        }
                    }
                    
                    // Egi Features
                    if (IsEnabled(CustomComboPreset.EgisOnRuinFeature))
                    {
                        if (IsEnabled(CustomComboPreset.SummonerSwiftcastEgiFeature) && level >= All.Levels.Swiftcast)
                        {
                            //Swiftcast Garuda Feature
                            if (swiftcasePhase == 1 && level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                            {
                                if (CanSpellWeave(actionID) && IsOffCooldown(All.Swiftcast) && gauge.IsGarudaAttuned)
                                    return All.Swiftcast;
                                if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) &&
                                    ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                    (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                    return OriginalHook(AstralFlow);
                            }

                            //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                            if (swiftcasePhase == 2)
                            {
                                if (IsOffCooldown(All.Swiftcast) && gauge.IsIfritAttuned)
                                {
                                    if (IsNotEnabled(CustomComboPreset.SummonerIfritUniqueFeature) ||
                                        (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && gauge.Attunement >= 1))
                                        return All.Swiftcast;
                                }

                            }

                            //SpS Swiftcast
                            if (swiftcasePhase == 3)
                            {
                                //Swiftcast Garuda Feature
                                if (level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                                {
                                    if (CanSpellWeave(actionID) && gauge.IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                                        return All.Swiftcast;
                                    if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) &&
                                        ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                        (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                        return OriginalHook(AstralFlow);
                                }

                                //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                                if (gauge.IsIfritAttuned && IsOffCooldown(All.Swiftcast))
                                {
                                    if (IsNotEnabled(CustomComboPreset.SummonerIfritUniqueFeature) ||
                                        (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && gauge.Attunement >= 1))
                                        return All.Swiftcast;
                                }
                            }
                        }

                        if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) && (IsNotEnabled(CustomComboPreset.SummonerSwiftcastEgiFeature) || swiftcasePhase == 2) && gauge.IsGarudaAttuned && HasEffect(Buffs.GarudasFavor) || //Garuda
                            IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature) && HasEffect(Buffs.TitansFavor) && lastComboMove == TopazRite && CanSpellWeave(actionID) || //Titan
                            IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && (gauge.IsIfritAttuned && HasEffect(Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == CrimsonCyclone)) //Ifrit
                            return OriginalHook(AstralFlow);

                        if (IsEnabled(CustomComboPreset.SummonerEgiAttacksFeature) && (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                            return OriginalHook(Gemshine);

                        if (IsEnabled(CustomComboPreset.EgisOnAOEFeature) && gauge.SummonTimerRemaining == 0 && IsOnCooldown(SummonPhoenix) && IsOnCooldown(SummonBahamut))
                        {
                            if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && level >= Levels.SummonRuby)
                                return OriginalHook(SummonRuby);

                            if (summonerPrimalChoice == 1)
                            {
                                if (gauge.IsTitanReady && level >= Levels.SummonTopaz)
                                    return OriginalHook(SummonTopaz);
                                if (gauge.IsGarudaReady && level >= Levels.SummonEmerald)
                                    return OriginalHook(SummonEmerald);
                            }

                            if (summonerPrimalChoice == 2)
                            {
                                if (gauge.IsGarudaReady && level >= Levels.SummonEmerald)
                                    return OriginalHook(SummonEmerald);
                                if (gauge.IsTitanReady && level >= Levels.SummonTopaz)
                                    return OriginalHook(SummonTopaz);
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SummonerRuin4ToRuin3Feature) && level >= Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                        return Ruin4;
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
                var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.SMNLucidDreamingFeature);
                var SummonerBurstPhase = Service.Configuration.GetCustomIntValue(Config.SummonerBurstPhase);
                var summonerPrimalChoice = Service.Configuration.GetCustomIntValue(Config.SummonerPrimalChoice);
                var swiftcasePhase = Service.Configuration.GetCustomIntValue(Config.SummonerSwiftcastPhase);

                if (actionID is Tridisaster or Outburst)
                {
                    if (InCombat())
                    {
                        if (CanSpellWeave(actionID))
                        {
                            // Searing Light
                            if (IsEnabled(CustomComboPreset.SearingLightonRuinFeature) && IsOffCooldown(SearingLight) && level >= Levels.SearingLight)
                            {
                                if (IsEnabled(CustomComboPreset.SummonerSearingLightBurstOption))
                                {
                                    if ((SummonerBurstPhase == 1 && OriginalHook(Ruin) == AstralFlare) ||
                                        (SummonerBurstPhase == 2 && OriginalHook(Ruin) == BrandOfPurgatory) ||
                                        (SummonerBurstPhase == 3 && OriginalHook(Ruin) is AstralFlare or BrandOfPurgatory) ||
                                        (SummonerBurstPhase == 4))
                                        return SearingLight;
                                }

                                else return SearingLight;
                            }


                            // ED & Fester
                            if (IsEnabled(CustomComboPreset.SummonerEDMainComboFeature))
                            {
                                if (gauge.HasAetherflowStacks)
                                {
                                    if (IsNotEnabled(CustomComboPreset.SummonerEDPoolonMainFeature))
                                        return Painflare;
                                    if (IsEnabled(CustomComboPreset.SummonerEDPoolonMainFeature))
                                    {
                                        if (level < Levels.SearingLight)
                                            return Painflare;
                                        if ((SummonerBurstPhase == 1 && OriginalHook(Ruin) == AstralImpulse) ||
                                            (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                            (SummonerBurstPhase == 3 && (GetCooldownRemainingTime(SearingLight) < 30 || GetCooldownRemainingTime(SearingLight) > 100) && OriginalHook(Ruin) is AstralImpulse or FountainOfFire) ||
                                            (SummonerBurstPhase == 4 && HasEffectAny(Buffs.SearingLight) && !HasEffect(Buffs.TitansFavor)) ||
                                            IsNotEnabled(CustomComboPreset.SummonerPrimalBurstChoice))
                                            return Painflare;
                                    }
                                }

                                if (level >= Levels.EnergySiphon && !gauge.HasAetherflowStacks && IsOffCooldown(EnergySiphon))
                                    return EnergySiphon;
                            }

                            // Lucid
                            if (IsEnabled(CustomComboPreset.SMNLucidDreamingFeature) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                                return All.LucidDreaming;

                            //Demi Nuke
                            if (IsEnabled(CustomComboPreset.SummonerAOEDemiFeature) && CanSpellWeave(actionID))
                            {
                                if (IsOffCooldown(OriginalHook(AstralFlow)) && 
                                    level >= Levels.AstralFlow && (level < Levels.Bahamut || lastComboMove is AstralFlare))
                                    return OriginalHook(AstralFlow);

                                if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && 
                                    level >= Levels.Bahamut && lastComboMove is AstralFlare or BrandOfPurgatory)
                                    return OriginalHook(EnkindleBahamut);
                            }

                            //Demi Nuke 2: Electric Boogaloo
                            if (IsEnabled(CustomComboPreset.SummonerAOETargetRekindleOption))
                            {
                                if (IsOffCooldown(OriginalHook(AstralFlow)) && lastComboMove is BrandOfPurgatory)
                                    return OriginalHook(AstralFlow);
                            }
                        }

                        //Demi
                        if (IsEnabled(CustomComboPreset.SummonerDemiAoESummonsFeature))
                        {
                            if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                                (level is >= Levels.Aethercharge and < Levels.Bahamut || //Pre Bahamut Phase
                                 gauge.IsBahamutReady && level >= Levels.Bahamut || //Bahamut Phase
                                 gauge.IsPhoenixReady && level >= Levels.Phoenix)) //Phoenix Phase
                                return OriginalHook(Aethercharge);

                        }


                        // Egi Features
                    if (IsEnabled(CustomComboPreset.EgisOnAOEFeature))
                    {
                        if (IsEnabled(CustomComboPreset.SummonerSwiftcastEgiFeature) && level >= All.Levels.Swiftcast)
                        {
                            //Swiftcast Garuda Feature
                            if (swiftcasePhase == 1 && level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                            {
                                if (CanSpellWeave(actionID) && IsOffCooldown(All.Swiftcast) && gauge.IsGarudaAttuned)
                                    return All.Swiftcast;
                                if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) &&
                                    ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                    (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                    return OriginalHook(AstralFlow);
                            }

                            //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                            if (swiftcasePhase == 2)
                            {
                                if (IsOffCooldown(All.Swiftcast) && gauge.IsIfritAttuned)
                                {
                                    if (IsNotEnabled(CustomComboPreset.SummonerIfritUniqueFeature) ||
                                        (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && gauge.Attunement >= 1))
                                        return All.Swiftcast;
                                }

                            }

                            //SpS Swiftcast
                            if (swiftcasePhase == 3)
                            {
                                //Swiftcast Garuda Feature
                                if (level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                                {
                                    if (CanSpellWeave(actionID) && gauge.IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                                        return All.Swiftcast;
                                    if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) &&
                                        ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                        (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                        return OriginalHook(AstralFlow);
                                }

                                //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                                if (gauge.IsIfritAttuned && IsOffCooldown(All.Swiftcast))
                                {
                                    if (IsNotEnabled(CustomComboPreset.SummonerIfritUniqueFeature) ||
                                        (IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && gauge.Attunement >= 1))
                                        return All.Swiftcast;
                                }
                            }
                        }

                        if (IsEnabled(CustomComboPreset.SummonerGarudaUniqueFeature) && (IsNotEnabled(CustomComboPreset.SummonerSwiftcastEgiFeature) || swiftcasePhase == 2) && gauge.IsGarudaAttuned && HasEffect(Buffs.GarudasFavor) || //Garuda
                            IsEnabled(CustomComboPreset.SummonerTitanUniqueFeature) && HasEffect(Buffs.TitansFavor) && lastComboMove == TopazCata && CanSpellWeave(actionID) || //Titan
                            IsEnabled(CustomComboPreset.SummonerIfritUniqueFeature) && (gauge.IsIfritAttuned && HasEffect(Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == CrimsonCyclone)) //Ifrit
                            return OriginalHook(AstralFlow);

                        //Precious Brilliance
                        if (IsEnabled(CustomComboPreset.SummonerEgiAttacksAOEFeature) && (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                            return OriginalHook(PreciousBrilliance);

                        if (IsEnabled(CustomComboPreset.EgisOnAOEFeature) && gauge.SummonTimerRemaining == 0 && IsOnCooldown(SummonPhoenix) && IsOnCooldown(SummonBahamut))
                        {
                            if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && level >= Levels.SummonRuby)
                                return OriginalHook(SummonRuby);

                            if (summonerPrimalChoice == 1)
                            {
                                if (gauge.IsTitanReady && level >= Levels.SummonTopaz)
                                    return OriginalHook(SummonTopaz);
                                if (gauge.IsGarudaReady && level >= Levels.SummonEmerald)
                                    return OriginalHook(SummonEmerald);
                            }

                            if (summonerPrimalChoice == 2)
                            {
                                if (gauge.IsGarudaReady && level >= Levels.SummonEmerald)
                                    return OriginalHook(SummonEmerald);
                                if (gauge.IsTitanReady && level >= Levels.SummonTopaz)
                                    return OriginalHook(SummonTopaz);
                            }
                        }
                    }
                        if (IsEnabled(CustomComboPreset.SummonerRuin4ToTridisasterFeature) && level >= Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                            return Ruin4;
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
                if (actionID is Ruin or Ruin2 or Ruin3 or DreadwyrmTrance or AstralFlow or EnkindleBahamut or SearingLight or RadiantAegis or Outburst or Tridisaster or PreciousBrilliance or Gemshine)
                {
                    if (!HasPetPresent() && gauge.SummonTimerRemaining == 0 && gauge.Attunement == 0 && GetCooldownRemainingTime(Ruin) == 0)
                        return SummonCarbuncle;
                }

                return actionID;
            }
        }

        internal class SummonerAstralFlowonSummonsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SummonerAstralFlowonSummonsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if ((actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 or SummonRuby or SummonIfrit or SummonIfrit2 && HasEffect(Buffs.TitansFavor)) ||
                    (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 && HasEffect(Buffs.GarudasFavor)) ||
                    (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasEffect(Buffs.IfritsFavor) || lastComboMove == CrimsonCyclone)))
                    return OriginalHook(AstralFlow);
                return actionID;
            }
        }

        internal class SummonerPrimalAbilitiesFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SummonerPrimalAbilitiesFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SummonBahamut or SummonPhoenix)
                {
                    if (IsOffCooldown(EnkindleBahamut) && OriginalHook(Ruin) is AstralImpulse)
                        return OriginalHook(EnkindleBahamut);
                    if (IsOffCooldown(EnkindlePhoenix) && OriginalHook(Ruin) is FountainOfFire)
                        return OriginalHook(EnkindlePhoenix);
                    if (OriginalHook(AstralFlow) is Deathflare or Rekindle)
                        return OriginalHook(AstralFlow);
                }

                return actionID;
            }
        }
    }
}