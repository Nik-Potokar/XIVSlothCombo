using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class SMN
    {
        public const byte ClassID = 26;
        public const byte JobID = 27;

        public const float CooldownThreshold = 0.5f;

        public const uint
            // Summons
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

            // Summon abilities
            Gemshine = 25883,
            PreciousBrilliance = 25884,
            DreadwyrmTrance = 3581,

            // Summon Ruins
            RubyRuin1 = 25808,
            RubyRuin2 = 25811,
            RubyRuin3 = 25817,
            TopazRuin1 = 25809,
            TopazRuin2 = 25812,
            TopazRuin3 = 25818,
            EmeralRuin1 = 25810,
            EmeralRuin2 = 25813,
            EmeralRuin3 = 25819,

            // Summon Outbursts
            Outburst = 16511,
            RubyOutburst = 25814,
            TopazOutburst = 25815,
            EmeraldOutburst = 25816,

            // Summon single targets
            RubyRite = 25823,
            TopazRite = 25824,
            EmeraldRite = 25825,

            // Summon AoEs
            RubyCata = 25832,
            TopazCata = 25833,
            EmeraldCata = 25834,

            // Summon Astral Flows
            CrimsonCyclone = 25835,     // Dash
            CrimsonStrike = 25885,      // Melee
            MountainBuster = 25836,
            Slipstream = 25837,

            // Demi summons
            SummonBahamut = 7427,
            SummonPhoenix = 25831,
            SummonSolarBahamut = 36992,

            // Demi summon abilities
            AstralImpulse = 25820,      // Single target Bahamut GCD
            AstralFlare = 25821,        // AoE Bahamut GCD
            Deathflare = 3582,          // Damage oGCD Bahamut
            EnkindleBahamut = 7429,

            FountainOfFire = 16514,     // Single target Phoenix GCD
            BrandOfPurgatory = 16515,   // AoE Phoenix GCD
            Rekindle = 25830,           // Healing oGCD Phoenix
            EnkindlePhoenix = 16516,
            
            UmbralImpulse = 36994,          //Single target Solar Bahamut GCD
            UmbralFlare = 36995,            //AoE Solar Bahamut GCD
            Sunflare = 36996,               //Damage oGCD Solar Bahamut
            EnkindleSolarBahamut = 36998,
            LuxSolaris = 36997,             //Healing oGCD Solar Bahamut

            // Shared summon abilities
            AstralFlow = 25822,

            // Summoner GCDs
            Ruin = 163,
            Ruin2 = 172,
            Ruin3 = 3579,
            Ruin4 = 7426,
            Tridisaster = 25826,

            // Summoner AoE
            RubyDisaster = 25827,
            TopazDisaster = 25828,
            EmeraldDisaster = 25829,

            // Summoner oGCDs
            EnergyDrain = 16508,
            Fester = 181,
            EnergySiphon = 16510,
            Painflare = 3578,
            SearingFlash = 36991,

            // Revive
            Resurrection = 173,

            // Buff 
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
                SearingLight = 2703,
                RubysGlimmer = 3873,
                RefulgentLux = 3874;
        }

        public static class Config
        {
            public static string
                SMN_Lucid = "SMN_Lucid",
                SMN_BurstPhase = "SMN_BurstPhase",
                SMN_PrimalChoice = "SMN_PrimalChoice",
                SMN_SwiftcastPhase = "SMN_SwiftcastPhase",
                SMN_Burst_Delay = "SMN_Burst_Delay",
                SMN_VariantCure = "SMN_VariantCure";

            public static UserBoolArray
                SMN_ST_Egi_AstralFlow = new("SMN_ST_Egi_AstralFlow");

            public static UserBool
                SMN_ST_CrimsonCycloneMelee = new("SMN_ST_CrimsonCycloneMelee");
        }

        internal class SMN_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Raise;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == All.Swiftcast)
                {
                    if (HasEffect(All.Buffs.Swiftcast) && IsEnabled(CustomComboPreset.SMN_Variant_Raise) && IsEnabled(Variant.VariantRaise))
                        return Variant.VariantRaise;

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

                    if (LevelChecked(EnergyDrain) && !gauge.HasAetherflowStacks)
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
                var gauge = GetJobGauge<SMNGauge>();

                if (actionID == Painflare && LevelChecked(Painflare) && !gauge.HasAetherflowStacks)
                {
                    if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && IsEnabled(CustomComboPreset.SMN_ESPainflare_Ruin4))
                        return Ruin4;

                    if (LevelChecked(EnergySiphon))
                        return EnergySiphon;

                    if (LevelChecked(EnergyDrain))
                        return EnergyDrain;
                }

                return actionID;
            }
        }

        internal class SMN_Simple_Combo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Simple_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var STCombo = actionID is Ruin or Ruin2;
                var AoECombo = actionID is Outburst or Tridisaster;
                var IsGarudaAttuned = OriginalHook(Gemshine) is EmeralRuin1 or EmeralRuin2 or EmeralRuin3 or EmeraldRite;
                var IsTitanAttuned = OriginalHook(Gemshine) is TopazRuin1 or TopazRuin2 or TopazRuin3 or TopazRite;
                var IsIfritAttuned = OriginalHook(Gemshine) is RubyRuin1 or RubyRuin2 or RubyRuin3 or RubyRite;
                var IsBahamutReady = OriginalHook(Aethercharge) is SummonBahamut;
                var IsPhoenixReady = OriginalHook(Aethercharge) is SummonPhoenix;
                var IsSolarBahamutReady = OriginalHook(Aethercharge) is SummonSolarBahamut;

                if (actionID is Ruin or Ruin2 or Outburst or Tridisaster)
                {
                    if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanSpellWeave(actionID))
                    {
                        if (IsOffCooldown(SearingLight) && LevelChecked(SearingLight) && ((!LevelChecked(SummonSolarBahamut) && OriginalHook(Ruin) is AstralImpulse) || OriginalHook(Ruin) is UmbralImpulse))
                            return SearingLight;

                        if (!gauge.HasAetherflowStacks && IsOffCooldown(EnergyDrain))
                        {
                            if ((STCombo || (AoECombo && !LevelChecked(EnergySiphon))) && LevelChecked(EnergyDrain))
                                return EnergyDrain;

                            if (AoECombo && LevelChecked(EnergySiphon))
                                return EnergySiphon;
                        }

                        if (HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                            return SearingFlash;
                        
                        if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire)
                        {
                            if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                                return OriginalHook(EnkindleBahamut);

                            if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse or UmbralImpulse)
                                return OriginalHook(AstralFlow);

                            if (IsOffCooldown(Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                                return OriginalHook(AstralFlow);
                            
                            if (IsOffCooldown(LuxSolaris) && HasEffect(Buffs.RefulgentLux))
                                return OriginalHook(LuxSolaris);
                        }

                        if (gauge.HasAetherflowStacks && CanSpellWeave(actionID))
                        {
                            if (!LevelChecked(SearingLight))
                            {
                                if (STCombo)
                                    return OriginalHook(Fester);

                                if (AoECombo && LevelChecked(Painflare))
                                    return Painflare;
                            }

                            if (HasEffect(Buffs.SearingLight))
                            {
                                if (STCombo)
                                    return OriginalHook(Fester);

                                if (AoECombo && LevelChecked(Painflare))
                                    return Painflare;
                            }
                        }

                        if (ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 4000)
                            return All.LucidDreaming;
                    }

                    if (InCombat() && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                        ((LevelChecked(Aethercharge) && !LevelChecked(SummonBahamut)) ||
                         (gauge.IsBahamutReady && LevelChecked(SummonBahamut)) ||
                         (gauge.IsPhoenixReady && LevelChecked(SummonPhoenix))))
                        return OriginalHook(Aethercharge);

                    if (LevelChecked(All.Swiftcast))
                    {
                        if (LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                        {
                            if (CanSpellWeave(actionID) && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                                return All.Swiftcast;

                            if (HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast))
                                return OriginalHook(AstralFlow);
                        }

                        if (IsIfritAttuned && gauge.Attunement >= 1 && IsOffCooldown(All.Swiftcast))
                            return All.Swiftcast;
                    }

                    if (IsIfritAttuned && gauge.Attunement >= 1 && HasEffect(All.Buffs.Swiftcast) && lastComboMove is not CrimsonCyclone)
                    {
                        if (STCombo)
                            return OriginalHook(Gemshine);

                        if (AoECombo && LevelChecked(PreciousBrilliance))
                            return OriginalHook(PreciousBrilliance);
                    }

                    if ((HasEffect(Buffs.GarudasFavor) && gauge.Attunement is 0) ||
                        (HasEffect(Buffs.TitansFavor) && lastComboMove is TopazRite or TopazCata && CanSpellWeave(actionID)) ||
                        (HasEffect(Buffs.IfritsFavor) && (IsMoving || gauge.Attunement is 0)) || (lastComboMove == CrimsonCyclone && InMeleeRange()))
                        return OriginalHook(AstralFlow);

                    if (HasEffect(Buffs.FurtherRuin) && ((!HasEffect(All.Buffs.Swiftcast) && IsIfritAttuned && IsMoving) || (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
                        return Ruin4;

                    if (IsGarudaAttuned || IsTitanAttuned || IsIfritAttuned)
                    {
                        if (STCombo)
                            return OriginalHook(Gemshine);

                        if (AoECombo && LevelChecked(PreciousBrilliance))
                            return OriginalHook(PreciousBrilliance);
                    }

                    if (gauge.SummonTimerRemaining == 0 && IsOnCooldown(SummonPhoenix) && IsOnCooldown(SummonBahamut))
                    {
                        if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && LevelChecked(SummonRuby))
                            return OriginalHook(SummonRuby);

                        if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                            return OriginalHook(SummonTopaz);

                        if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                            return OriginalHook(SummonEmerald);
                    }

                    if (LevelChecked(Ruin4) && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                        return Ruin4;
                }

                return actionID;
            }
        }
        internal class SMN_Advanced_Combo : CustomCombo
        {
            internal static uint DemiAttackCount = 0;
            internal static bool UsedDemiAttack = false;
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Advanced_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SMNGauge>();
                var summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_PrimalChoice);
                var SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_BurstPhase);
                var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_Lucid);
                var swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_SwiftcastPhase);
                var burstDelay = PluginConfiguration.GetCustomIntValue(Config.SMN_Burst_Delay);
                var inOpener = CombatEngageDuration().TotalSeconds < 40;
                var STCombo = actionID is Ruin or Ruin2;
                var AoECombo = actionID is Outburst or Tridisaster;
                var IsGarudaAttuned = OriginalHook(Gemshine) is EmeralRuin1 or EmeralRuin2 or EmeralRuin3 or EmeraldRite;
                var IsTitanAttuned = OriginalHook(Gemshine) is TopazRuin1 or TopazRuin2 or TopazRuin3 or TopazRite;
                var IsIfritAttuned = OriginalHook(Gemshine) is RubyRuin1 or RubyRuin2 or RubyRuin3 or RubyRite;
                var IsBahamutReady = OriginalHook(Aethercharge) is SummonBahamut;
                var IsPhoenixReady = OriginalHook(Aethercharge) is SummonPhoenix;
                var IsSolarBahamutReady = OriginalHook(Aethercharge) is SummonSolarBahamut;

                if (WasLastAction(OriginalHook(Aethercharge))) DemiAttackCount = 0;    // Resets counter

                if (IsEnabled(CustomComboPreset.SMN_Advanced_Burst_Delay_Option) && !inOpener) DemiAttackCount = 6; // If SMN_Advanced_Burst_Delay_Option is active and outside opener window, set DemiAttackCount to 6 to ignore delayed oGCDs 

                if (GetCooldown(OriginalHook(Aethercharge)).CooldownElapsed >= 12.5) DemiAttackCount = 6; // Sets DemiAttackCount to 6 if for whatever reason you're in a position that you can't demi attack to prevent ogcd waste.

                if (gauge.SummonTimerRemaining == 0 && !InCombat()) DemiAttackCount = 0;

                //CHECK_DEMIATTACK_USE
                if (UsedDemiAttack == false && lastComboMove is AstralImpulse or UmbralImpulse or FountainOfFire or AstralFlare or UmbralFlare or BrandOfPurgatory && DemiAttackCount is not 6 && GetCooldownRemainingTime(AstralImpulse) > 1)
                {
                    UsedDemiAttack = true;      // Registers that a Demi Attack was used and blocks further incrementation of DemiAttackCountCount
                    DemiAttackCount++;          // Increments DemiAttack counter
                }

                //CHECK_DEMIATTACK_USE_RESET
                if (UsedDemiAttack && GetCooldownRemainingTime(AstralImpulse) < 1) UsedDemiAttack = false;  // Resets block to allow CHECK_DEMIATTACK_USE

                if (actionID is Ruin or Ruin2 or Outburst or Tridisaster)
                {
                    if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanSpellWeave(actionID))
                    {
                        // Searing Light
                        if (IsEnabled(CustomComboPreset.SMN_SearingLight) && CanDelayedWeave(actionID) && IsOffCooldown(SearingLight) && LevelChecked(SearingLight))
                        {
                            if (IsEnabled(CustomComboPreset.SMN_SearingLight_Burst))
                            {
                                if (SummonerBurstPhase is 0 or 1 && ((!LevelChecked(SummonSolarBahamut) && OriginalHook(Ruin) is AstralImpulse) || OriginalHook(Ruin) is UmbralImpulse) ||
                                    (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                                    (SummonerBurstPhase == 3 && OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire) ||
                                    (SummonerBurstPhase == 4))
                                {
                                    if (STCombo || (AoECombo && IsNotEnabled(CustomComboPreset.SMN_SearingLight_STOnly)))
                                        return SearingLight;
                                }
                            }
                            else return SearingLight;
                        }

                        // Emergency priority Demi Nuke to prevent waste if you can't get demi attacks out to satisfy the slider check.
                        if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire && 
                            IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Attacks) && GetCooldown(OriginalHook(Aethercharge)).CooldownElapsed >= 12.5)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Attacks))
                            {
                                if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                                    return OriginalHook(EnkindleBahamut);

                                if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                                    return OriginalHook(AstralFlow);

                                if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix))
                                    return OriginalHook(EnkindlePhoenix);
                                
                                if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Rekindle))
                                    if (IsOffCooldown(Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                                        return OriginalHook(AstralFlow);

                                if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                                    return OriginalHook(EnkindleSolarBahamut);

                                if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                                    return OriginalHook(AstralFlow);
                            }
                        }

                        // ED/ES
                        if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_EDFester) && !gauge.HasAetherflowStacks && IsOffCooldown(EnergyDrain) && (!LevelChecked(DreadwyrmTrance) || !inOpener || DemiAttackCount >= burstDelay))
                        {
                            if ((STCombo || (AoECombo && !LevelChecked(EnergySiphon))) && LevelChecked(EnergyDrain))
                                return EnergyDrain;

                            if (AoECombo && LevelChecked(EnergySiphon))
                                return EnergySiphon;
                        }

                        // First set Fester/Painflare if ED is close to being off CD, or off CD while you have aetherflow stacks.
                        if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_EDFester) && IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling) && gauge.HasAetherflowStacks)
                        {
                            if (GetCooldown(EnergyDrain).CooldownRemaining <= 3.2)
                            {
                                if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_Advanced_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                                    (SummonerBurstPhase is not 4)) ||
                                    (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                                {
                                    if (STCombo)
                                        return OriginalHook(Fester);

                                    if (AoECombo && LevelChecked(Painflare) && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling_Only))
                                        return Painflare;
                                }
                            }
                        }

                        if (IsEnabled(CustomComboPreset.SMN_SearingFlash) && HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                            return SearingFlash;

                        // Demi Nuke
                        if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire)
                        {
                            if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Attacks) && IsBahamutReady && (!LevelChecked(SummonSolarBahamut) || DemiAttackCount >= burstDelay))
                            {
                                if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                                    return OriginalHook(EnkindleBahamut);

                                if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                                    return OriginalHook(AstralFlow);
                            }

                            // Demi Nuke 2: Electric Boogaloo
                            if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Attacks) && IsPhoenixReady)
                            {
                                if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix) && OriginalHook(Ruin) is FountainOfFire)
                                    return OriginalHook(EnkindlePhoenix);
                                
                                if (IsOffCooldown(Rekindle) && IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                                    return OriginalHook(AstralFlow);
                            }
                            
                            // Demi Nuke 3: More Boogaloo
                            if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Attacks) && IsSolarBahamutReady && DemiAttackCount >= burstDelay)
                            {
                                if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                                    return OriginalHook(EnkindleSolarBahamut);

                                if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                                    return OriginalHook(AstralFlow);
                                
                                if (IsOffCooldown(LuxSolaris) && IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_LuxSolaris) && HasEffect(Buffs.RefulgentLux))
                                    return OriginalHook(LuxSolaris);
                            }
                        }

                        // Fester/Painflare
                        if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_EDFester))
                        {
                            if (gauge.HasAetherflowStacks && CanSpellWeave(actionID))
                            {
                                if (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling))
                                {
                                    if (STCombo)
                                        return OriginalHook(Fester);

                                    if (AoECombo && LevelChecked(Painflare))
                                        return Painflare;
                                }

                                if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling))
                                {
                                    if (!LevelChecked(SearingLight))
                                    {
                                        if (STCombo)
                                            return OriginalHook(Fester);

                                        if (AoECombo && LevelChecked(Painflare) && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling_Only))
                                            return Painflare;
                                    }

                                    if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_Advanced_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                                        SummonerBurstPhase is 0 or 1 or 2 or 3 && DemiAttackCount >= burstDelay) ||
                                        (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                                    {
                                        if (STCombo)
                                            return OriginalHook(Fester);

                                        if (AoECombo && LevelChecked(Painflare) && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling_Only))
                                            return Painflare;
                                    }
                                }
                            }
                        }

                        // Lucid Dreaming
                        if (IsEnabled(CustomComboPreset.SMN_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold)
                            return All.LucidDreaming;
                    }

                    // Demi
                    if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons))
                    {
                        if (gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                            ((LevelChecked(Aethercharge) && !LevelChecked(SummonBahamut) && InCombat()) ||   // Pre-Bahamut Phase
                             (IsBahamutReady && LevelChecked(SummonBahamut)) ||            // Bahamut Phase
                             (IsPhoenixReady && LevelChecked(SummonPhoenix)) ||            // Phoenix Phase
                             (IsSolarBahamutReady && LevelChecked(SummonSolarBahamut))))   // Solar Bahamut Phase
                            return OriginalHook(Aethercharge);
                    }

                    //Ruin4 in Egi Phases
                    if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_Ruin4) && HasEffect(Buffs.FurtherRuin) &&
                        ((!HasEffect(All.Buffs.Swiftcast) && IsMoving && ((HasEffect(Buffs.GarudasFavor) && !IsGarudaAttuned) || (IsIfritAttuned && lastComboMove is not CrimsonCyclone))) ||
                        GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0))
                        return Ruin4;

                    // Egi Features
                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) && LevelChecked(All.Swiftcast))
                    {
                        // Swiftcast Garuda Feature
                        if (swiftcastPhase is 0 or 1 && LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                        {
                            if (CanSpellWeave(actionID) && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                            {
                                if (STCombo || (AoECombo && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only)))
                                    return All.Swiftcast;
                            }

                            if (Config.SMN_ST_Egi_AstralFlow[2] &&
                                ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                                return OriginalHook(AstralFlow);
                        }

                        // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                        if (swiftcastPhase == 2)
                        {
                            if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && lastComboMove is not CrimsonCyclone)
                            {
                                if (!Config.SMN_ST_Egi_AstralFlow[1] || (Config.SMN_ST_Egi_AstralFlow[1] && gauge.Attunement >= 1))
                                {
                                    if (STCombo || (AoECombo && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only)))
                                        return All.Swiftcast;
                                }
                            }
                        }

                        // SpS Swiftcast
                        if (swiftcastPhase == 3)
                        {
                            // Swiftcast Garuda Feature
                            if (LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                            {
                                if (CanSpellWeave(actionID) && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                                {
                                    if (STCombo || (AoECombo && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only)))
                                        return All.Swiftcast;
                                }

                                if (Config.SMN_ST_Egi_AstralFlow[2] &&
                                    ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                                    return OriginalHook(AstralFlow);
                            }

                            // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                            if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && lastComboMove is not CrimsonCyclone)
                            {
                                if (!Config.SMN_ST_Egi_AstralFlow[1] || (Config.SMN_ST_Egi_AstralFlow[1] && gauge.Attunement >= 1))
                                {
                                    if (STCombo || (AoECombo && IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi_Only)))
                                        return All.Swiftcast;
                                }
                            }
                        }
                    }

                    // Gemshine/Precious Brilliance priority casting
                    if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_EgiSummons_Attacks) &&
                        ((IsIfritAttuned && gauge.Attunement >= 1 && HasEffect(All.Buffs.Swiftcast) && lastComboMove is not CrimsonCyclone) ||
                        (HasEffect(Buffs.GarudasFavor) && gauge.Attunement >= 1 && !HasEffect(All.Buffs.Swiftcast) && IsMoving)))
                    {
                        if (STCombo)
                            return OriginalHook(Gemshine);

                        if (AoECombo && LevelChecked(PreciousBrilliance))
                            return OriginalHook(PreciousBrilliance);
                    }

                    if ((Config.SMN_ST_Egi_AstralFlow[2] && HasEffect(Buffs.GarudasFavor) && (IsNotEnabled(CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi) || swiftcastPhase == 2)) ||                 // Garuda
                        (Config.SMN_ST_Egi_AstralFlow[0] && HasEffect(Buffs.TitansFavor) && lastComboMove is TopazRite or TopazCata && CanSpellWeave(actionID)) ||                                  // Titan
                        (Config.SMN_ST_Egi_AstralFlow[1] && (HasEffect(Buffs.IfritsFavor) && !Config.SMN_ST_CrimsonCycloneMelee && (IsMoving || gauge.Attunement == 0) || (lastComboMove is CrimsonCyclone && InMeleeRange()))) ||
                        (Config.SMN_ST_Egi_AstralFlow[1] && HasEffect(Buffs.IfritsFavor) && Config.SMN_ST_CrimsonCycloneMelee && InMeleeRange()))  // Ifrit
                        return OriginalHook(AstralFlow);

                    // Gemshine/Precious Brilliance
                    if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_EgiSummons_Attacks) && (IsGarudaAttuned || IsTitanAttuned || IsIfritAttuned))
                    {
                        if (STCombo)
                            return OriginalHook(Gemshine);

                        if (AoECombo && LevelChecked(PreciousBrilliance))
                            return OriginalHook(PreciousBrilliance);
                    }

                    // Egi Order
                    if (IsEnabled(CustomComboPreset.SMN_DemiEgiMenu_EgiOrder) && gauge.SummonTimerRemaining == 0)
                    {
                        if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && LevelChecked(SummonRuby))
                            return OriginalHook(SummonRuby);

                        if (summonerPrimalChoice is 0 or 1)
                        {
                            if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                                return OriginalHook(SummonTopaz);

                            if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                                return OriginalHook(SummonEmerald);
                        }

                        if (summonerPrimalChoice == 2)
                        {
                            if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                                return OriginalHook(SummonEmerald);

                            if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                                return OriginalHook(SummonTopaz);
                        }
                    }

                    // Ruin 4
                    if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_Ruin4) && LevelChecked(Ruin4) && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
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
                    (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasEffect(Buffs.IfritsFavor) || (lastComboMove == CrimsonCyclone && InMeleeRange()))))
                    return OriginalHook(AstralFlow);

                return actionID;
            }
        }

        internal class SMN_DemiAbilities : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_DemiAbilities;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Aethercharge or DreadwyrmTrance or SummonBahamut or SummonPhoenix or SummonSolarBahamut)
                {
                    if (IsOffCooldown(EnkindleBahamut) && OriginalHook(Ruin) is AstralImpulse)
                        return OriginalHook(EnkindleBahamut);

                    if (IsOffCooldown(EnkindlePhoenix) && OriginalHook(Ruin) is FountainOfFire)
                        return OriginalHook(EnkindlePhoenix);

                    if (IsOffCooldown(EnkindleSolarBahamut) && OriginalHook(Ruin) is UmbralImpulse)
                        return OriginalHook(EnkindleBahamut);
                    
                    if ((OriginalHook(AstralFlow) is Deathflare && IsOffCooldown(Deathflare)) || (OriginalHook(AstralFlow) is Rekindle && IsOffCooldown(Rekindle)))
                        return OriginalHook(AstralFlow);
                    
                    if (OriginalHook(AstralFlow) is Sunflare && IsOffCooldown(Sunflare))
                        return OriginalHook(Sunflare);
                }

                return actionID;
            }
        }
    }
}