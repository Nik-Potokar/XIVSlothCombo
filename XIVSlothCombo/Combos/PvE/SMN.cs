using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using System;

namespace XIVSlothCombo.Combos.PvE
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
                SMN_Lucid = "SMN_Lucid",
                SMN_BurstPhase = "SMN_BurstPhase",
                SMN_PrimalChoice = "SMN_PrimalChoice",
                SMN_SwiftcastPhase = "SMN_SwiftcastPhase";
        }

        internal class SMN_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Raise;

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

        internal class SMN_RuinMobility : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_RuinMobility;

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

        internal class SMN_EDFester : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_EDFester;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Fester)
                {
                    var gauge = GetJobGauge<SMNGauge>();
                    if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SMN_EDFester_Ruin4))
                        return Ruin4;
                    if (level >= Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                        return EnergyDrain;
                }

                return actionID;
            }
        }

        internal class SMN_ESPainflare : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_ESPainflare;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Painflare)
                {
                    var gauge = GetJobGauge<SMNGauge>();
                    if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SMN_ESPainflare_Ruin4))
                        return Ruin4;
                    if (level >= Levels.EnergySiphon && !gauge.HasAetherflowStacks)
                        return EnergySiphon;

                }

                return actionID;
            }
        }

        internal class SMN_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_ST_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_PrimalChoice);
                var SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_BurstPhase);
                var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_Lucid);
                var swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_SwiftcastPhase);

                if (actionID is Ruin or Ruin2 && InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        // Searing Light
                        if (IsEnabled(CustomComboPreset.SMN_SearingLight) && IsOffCooldown(SearingLight) && level >= Levels.SearingLight)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_SearingLight_Burst))
                            {
                                if ((SummonerBurstPhase is 0 or 1 && OriginalHook(Ruin) == AstralImpulse) ||
                                    (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                    (SummonerBurstPhase == 3 && OriginalHook(Ruin) is AstralImpulse or FountainOfFire) ||
                                    (SummonerBurstPhase == 4))
                                    return SearingLight;
                            }

                            else return SearingLight;
                        }

                        // ED & Fester
                        if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_EDFester))
                        {
                            if (gauge.HasAetherflowStacks)
                            {
                                if (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling))
                                    return Fester;
                                if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling))
                                {
                                    if (level < Levels.SearingLight)
                                        return Fester;
                                    if ((SummonerBurstPhase is 0 or 1 && OriginalHook(Ruin) == AstralImpulse) ||
                                        (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                        (SummonerBurstPhase == 3 && (GetCooldownRemainingTime(SearingLight) < 30 || GetCooldownRemainingTime(SearingLight) > 100) && OriginalHook(Ruin) is AstralImpulse or FountainOfFire) ||
                                        (SummonerBurstPhase == 4 && HasEffectAny(Buffs.SearingLight) && !HasEffect(Buffs.TitansFavor)) ||
                                        IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_BurstChoice))
                                        return Fester;
                                }
                            }

                            if (level >= Levels.EnergyDrain && !gauge.HasAetherflowStacks && IsOffCooldown(EnergyDrain))
                                return EnergyDrain;
                        }

                        // Lucid
                        if (IsEnabled(CustomComboPreset.SMN_Lucid) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                            return All.LucidDreaming;

                        //Demi Nuke
                        if (OriginalHook(Ruin) is AstralImpulse or FountainOfFire)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_DemiSummons_Attacks))
                            {
                                if (IsOffCooldown(Deathflare) && level >= Levels.AstralFlow && (level < Levels.Bahamut || lastComboMove is AstralImpulse))
                                    return OriginalHook(AstralFlow);

                                if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && level >= Levels.Bahamut && lastComboMove is AstralImpulse or FountainOfFire)
                                    return OriginalHook(EnkindleBahamut);
                            }

                            //Demi Nuke 2: Electric Boogaloo
                            if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_DemiSummons_Rekindle))
                            {
                                if (IsOffCooldown(Rekindle) && lastComboMove is FountainOfFire)
                                    return OriginalHook(AstralFlow);
                            }
                        }
                    }

                    //Demi
                    if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_DemiSummons))
                    {
                        if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                            (level is >= Levels.Aethercharge and < Levels.Bahamut || //Pre Bahamut Phase
                             gauge.IsBahamutReady && level >= Levels.Bahamut || //Bahamut Phase
                             gauge.IsPhoenixReady && level >= Levels.Phoenix)) //Phoenix Phase
                            return OriginalHook(Aethercharge);
                    }
                    
                    // Egi Features
                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) && level >= All.Levels.Swiftcast)
                    {
                        //Swiftcast Garuda Feature
                        if (swiftcastPhase is 0 or 1 && level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                        {
                            if (CanSpellWeave(actionID) && IsOffCooldown(All.Swiftcast) && gauge.IsGarudaAttuned)
                                return All.Swiftcast;
                            if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) &&
                                ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                 (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                return OriginalHook(AstralFlow);
                        }

                        //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                        if (swiftcastPhase == 2)
                        {
                            if (IsOffCooldown(All.Swiftcast) && gauge.IsIfritAttuned)
                            {
                                if (IsNotEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) ||
                                    (IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) && gauge.Attunement >= 1))
                                    return All.Swiftcast;
                            }

                        }

                        //SpS Swiftcast
                        if (swiftcastPhase == 3)
                        {
                            //Swiftcast Garuda Feature
                            if (level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                            {
                                if (CanSpellWeave(actionID) && gauge.IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                                    return All.Swiftcast;
                                if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) &&
                                    ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                     (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                    return OriginalHook(AstralFlow);
                            }

                            //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                            if (gauge.IsIfritAttuned && IsOffCooldown(All.Swiftcast))
                            {
                                if (IsNotEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) ||
                                    (IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) && gauge.Attunement >= 1))
                                    return All.Swiftcast;
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) && (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) || swiftcastPhase == 2) && gauge.IsGarudaAttuned && HasEffect(Buffs.GarudasFavor) || //Garuda
                        IsEnabled(CustomComboPreset.SMN_Titan_MountainBuster) && HasEffect(Buffs.TitansFavor) && lastComboMove == TopazRite && CanSpellWeave(actionID) || //Titan
                        IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) && (gauge.IsIfritAttuned && HasEffect(Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == CrimsonCyclone)) //Ifrit
                        return OriginalHook(AstralFlow);

                    // Gemshine
                    if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_EgiSummons_Attacks) && (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                        return OriginalHook(Gemshine);

                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_EgiOrder) && gauge.SummonTimerRemaining == 0 && IsOnCooldown(SummonPhoenix) && IsOnCooldown(SummonBahamut))
                    {
                        if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && level >= Levels.SummonRuby)
                            return OriginalHook(SummonRuby);

                        if (summonerPrimalChoice is 0 or 1)
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
                    

                    if (IsEnabled(CustomComboPreset.SMN_ST_MainCombo_Ruin4) && level >= Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                        return Ruin4;
                }

                return actionID;
            }
        }

        internal class SMN_AoE_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_AoE_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_Lucid);
                var SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_BurstPhase);
                var summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_PrimalChoice);
                var swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_SwiftcastPhase);

                if (actionID is Tridisaster or Outburst && InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        // Searing Light
                        if (IsEnabled(CustomComboPreset.SMN_SearingLight) &&
                            IsNotEnabled(CustomComboPreset.SMN_SearingLight_STOnly) && IsOffCooldown(SearingLight) &&
                            level >= Levels.SearingLight)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_SearingLight_Burst))
                            {
                                if ((SummonerBurstPhase is 0 or 1 && OriginalHook(Tridisaster) == AstralFlare) ||
                                    (SummonerBurstPhase == 2 && OriginalHook(Tridisaster) == BrandOfPurgatory) ||
                                    (SummonerBurstPhase == 3 && OriginalHook(Tridisaster) is AstralFlare or BrandOfPurgatory) ||
                                    (SummonerBurstPhase == 4))
                                    return SearingLight;
                            }

                            else return SearingLight;
                        }


                        // ES & Painflare
                        if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_ESPainflare))
                        {
                            if (gauge.HasAetherflowStacks && level >= Levels.Painflare)
                            {
                                if (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling))
                                    return Painflare;
                                if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling) && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling_Only))
                                {
                                    if (level < Levels.SearingLight)
                                        return Painflare;
                                    if ((SummonerBurstPhase is 0 or 1 && OriginalHook(Tridisaster) == AstralFlare) ||
                                        (SummonerBurstPhase == 2 && OriginalHook(Tridisaster) == BrandOfPurgatory) ||
                                        (SummonerBurstPhase == 3 && (GetCooldownRemainingTime(SearingLight) < 30 || GetCooldownRemainingTime(SearingLight) > 100) && OriginalHook(Tridisaster) is AstralFlare or BrandOfPurgatory) ||
                                        (SummonerBurstPhase == 4 && HasEffectAny(Buffs.SearingLight) &&
                                         !HasEffect(Buffs.TitansFavor)) || IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_BurstChoice))
                                        return Painflare;
                                }
                            }

                            if (level >= Levels.EnergySiphon && !gauge.HasAetherflowStacks &&
                                IsOffCooldown(EnergySiphon))
                                return EnergySiphon;
                        }

                        // Lucid
                        if (IsEnabled(CustomComboPreset.SMN_Lucid) && IsOffCooldown(All.LucidDreaming) &&
                            LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                            return All.LucidDreaming;

                        //Demi Nuke
                        if (OriginalHook(Tridisaster) is AstralFlare or BrandOfPurgatory)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_Demis))
                            {
                                if (IsOffCooldown(Deathflare) && level >= Levels.AstralFlow && (level < Levels.Bahamut || lastComboMove is AstralFlare))
                                    return OriginalHook(AstralFlow);

                                if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && level >= Levels.Bahamut && lastComboMove is AstralFlare or BrandOfPurgatory)
                                    return OriginalHook(EnkindleBahamut);
                            }
                            
                            //Demi Nuke 2: Electric Boogaloo
                            if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_Rekindle))
                            {
                                if (IsOffCooldown(Rekindle) && lastComboMove is BrandOfPurgatory)
                                    return OriginalHook(AstralFlow);
                            }
                        }
                    }

                    //Demi
                    if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_DemiSummons))
                    {
                        if (gauge.AttunmentTimerRemaining == 0 && gauge.SummonTimerRemaining == 0 &&
                            IsOffCooldown(OriginalHook(Aethercharge)) &&
                            (level is >= Levels.Aethercharge and < Levels.Bahamut || //Pre Bahamut Phase
                             gauge.IsBahamutReady && level >= Levels.Bahamut || //Bahamut Phase
                             gauge.IsPhoenixReady && level >= Levels.Phoenix)) //Phoenix Phase
                            return OriginalHook(Aethercharge);
                    }

                    // Egi Features
                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) && level >= All.Levels.Swiftcast)
                    {
                        //Swiftcast Garuda Feature
                        if (swiftcastPhase is 0 or 1 && level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                        {
                            if (CanSpellWeave(actionID) && IsOffCooldown(All.Swiftcast) && gauge.IsGarudaAttuned &&
                                IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only))
                                return All.Swiftcast;
                            if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) &&
                                ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                 (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                return OriginalHook(AstralFlow);
                        }

                        //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                        if (swiftcastPhase == 2)
                        {
                            if (IsOffCooldown(All.Swiftcast) && gauge.IsIfritAttuned)
                            {
                                if ((IsNotEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) ||
                                     (IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) &&
                                      gauge.Attunement >= 1)) &&
                                    IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only))
                                    return All.Swiftcast;
                            }

                        }

                        //SpS Swiftcast
                        if (swiftcastPhase == 3)
                        {
                            //Swiftcast Garuda Feature
                            if (level >= Levels.Slipstream && HasEffect(Buffs.GarudasFavor))
                            {
                                if (CanSpellWeave(actionID) && gauge.IsGarudaAttuned && IsOffCooldown(All.Swiftcast) &&
                                    IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only))
                                    return All.Swiftcast;
                                if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) &&
                                    ((gauge.IsGarudaAttuned && HasEffect(All.Buffs.Swiftcast)) ||
                                     (gauge.Attunement == 0))) //Astral Flow if Swiftcast is not ready throughout Garuda
                                    return OriginalHook(AstralFlow);
                            }

                            //Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                            if (gauge.IsIfritAttuned && IsOffCooldown(All.Swiftcast))
                            {
                                if ((IsNotEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) ||
                                     (IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) &&
                                      gauge.Attunement >= 1)) &&
                                    IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only))
                                    return All.Swiftcast;
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SMN_Garuda_Slipstream) && (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) || swiftcastPhase == 2) && gauge.IsGarudaAttuned && HasEffect(Buffs.GarudasFavor) || //Garuda
                        IsEnabled(CustomComboPreset.SMN_Titan_MountainBuster) && HasEffect(Buffs.TitansFavor) && lastComboMove == TopazCata && CanSpellWeave(actionID) || //Titan
                        IsEnabled(CustomComboPreset.SMN_Ifrit_Cyclone) && (gauge.IsIfritAttuned && HasEffect(Buffs.IfritsFavor) || gauge.IsIfritAttuned && lastComboMove == CrimsonCyclone)) //Ifrit
                        return OriginalHook(AstralFlow);

                    //Precious Brilliance
                    if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_EgiAttacks) &&
                        (gauge.IsGarudaAttuned || gauge.IsTitanAttuned || gauge.IsIfritAttuned))
                        return OriginalHook(PreciousBrilliance);

                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_EgiOrder) && gauge.SummonTimerRemaining == 0 && IsOnCooldown(SummonPhoenix) && IsOnCooldown(SummonBahamut))
                    {
                        if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady &&
                            level >= Levels.SummonRuby)
                            return OriginalHook(SummonRuby);

                        if (summonerPrimalChoice is 0 or 1)
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

                    if (IsEnabled(CustomComboPreset.SMN_AoE_MainCombo_Ruin4) && level >= Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                        return Ruin4;
                }
                
                return actionID;
            }
        }

        internal class SMN_CarbuncleReminder : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_CarbuncleReminder;
            internal static bool carbyPresent = false;
            internal static DateTime noPetTime;
            internal static DateTime presentTime;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {                            
                if (actionID is Ruin or Ruin2 or Ruin3 or DreadwyrmTrance or AstralFlow or EnkindleBahamut or SearingLight or RadiantAegis or Outburst or Tridisaster or PreciousBrilliance or Gemshine)
                {
                    presentTime = DateTime.Now;
                    int deltaTime = (presentTime - noPetTime).Milliseconds;
                    var gauge = GetJobGauge<SMNGauge>();

                    if (HasPetPresent())
                    {
                        carbyPresent = true;
                        noPetTime = DateTime.Now;
                    }
                 
                    //Deals with the game's half second pet refresh
                    if (deltaTime > 500 && !HasPetPresent() && gauge.SummonTimerRemaining == 0 && gauge.Attunement == 0 && GetCooldownRemainingTime(Ruin) == 0)
                        carbyPresent = false;
                    if (carbyPresent == false)
                        return SummonCarbuncle;
                }

                return actionID;
            }
        }

        internal class SMN_Egi_AstralFlow : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_Egi_AstralFlow;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if ((actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 or SummonRuby or SummonIfrit or SummonIfrit2 && HasEffect(Buffs.TitansFavor)) ||
                    (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 && HasEffect(Buffs.GarudasFavor)) ||
                    (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasEffect(Buffs.IfritsFavor) || lastComboMove == CrimsonCyclone)))
                    return OriginalHook(AstralFlow);
                return actionID;
            }
        }

        internal class SMN_DemiAbilities : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_DemiAbilities;

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