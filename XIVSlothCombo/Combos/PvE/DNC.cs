using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DNC
    {
        public const byte JobID = 38;

        public const uint
            // Single Target
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            StarfallDance = 25792,
            // AoE
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            Tillana = 25790,
            // Dancing
            StandardStep = 15997,
            TechnicalStep = 15998,
            StandardFinish = 16192,
            TechnicalFinish = 16196,
            // Fan Dances
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            // Other
            Peloton = 7557,
            SaberDance = 16005,
            Devilment = 16011,
            ShieldSamba = 16012,
            Flourish = 16013,
            Improvisation = 16014,
            CuringWaltz = 16015;

            /* Unused
            EnAvant = 16010,
            ClosedPosition = 16006,
            Ending = 18073,
            Emboite = 15999,
            Entrechat = 16000,
            Jete = 16001,
            Pirouette = 16002,
            StandardFinish0 = 16003,
            StandardFinish1 = 16191,
            TechnicalFinish0 = 16004,
            TechnicalFinish1 = 16193,
            TechnicalFinish2 = 16194,
            TechnicalFinish3 = 16195,
            ImprovisedFinish = 25789,
            */

        public static class Buffs
        {
            public const ushort
                // Flourishing & Silken (Procs)
                FlourishingCascade = 1814,
                FlourishingFountain = 1815,
                FlourishingWindmill = 1816,
                FlourishingShower = 1817,
                FlourishingFanDance = 2021,
                SilkenSymmetry = 2693,
                SilkenFlow = 2694,
                FlourishingFinish = 2698,
                FlourishingStarfall = 2700,
                FlourishingSymmetry = 3017,
                FlourishingFlow = 3018,
                // Dances
                StandardStep = 1818,
                TechnicalStep = 1819,
                StandardFinish = 1821,
                TechnicalFinish = 1822,
                // Fan Dances
                ThreeFoldFanDance = 1820,
                FourFoldFanDance = 2699,
                // Other
                Peloton = 1199,
                ShieldSamba = 1826;
        }

        /* Traits
        public static class Traits
        {
            public const ushort
                FourfoldFantasy = 252,
                ActionDamage1 = 251,
                ActionDamage2 = 253,
                EnhancedEnAvant = 254,
                EnhancedEnAvant2 = 256,
                EnhancedTechnicalFinish = 453,
                EnhancedEsprit = 454,
                EnhancedFlourish = 455,
                EnhancedShieldSamba = 456,
                EnhancedDevilment = 457;
        }
        */

        private static DNCGauge Gauge => CustomComboFunctions.GetJobGauge<DNCGauge>();

        public static class Config
        {
            #region Legacy config
            internal static UserInt
                DNC_LegacyST_EspritTh       = new("DNC_LegacyST_EspritTh"),     // Single target Esprit threshold (L)
                DNC_LegacyAoE_EspritTh      = new("DNC_LegacyAoE_EspritTh");    // AoE Esprit threshold (L)
            #endregion

            #region Advanced ST Sliders
            internal static UserInt
                DNC_AdvST_SSBurstPct        = new("DNC_AdvST_SSBurstPct"),      // Standard Step    target HP% threshold
                DNC_AdvST_TSBurstPct        = new("DNC_AdvST_TSBurstPct"),      // Technical Step   target HP% threshold
                DNC_AdvST_FeatherBurstPct   = new("DNC_AdvST_FeatherBurstPct"), // Feather burst    target HP% threshold
                DNC_AdvST_SaberTh           = new("DNC_AdvST_SaberTh"),         // Saber Dance      Esprit     threshold
                DNC_AdvST_PanicWaltzPct     = new("DNC_AdvST_PanicWaltzPct"),   // Curing Waltz     player HP% threshold
                DNC_AdvST_PanicWindPct      = new("DNC_AdvST_PanicWindPct");    // Second Wind      player HP% threshold
            #endregion

            #region Advanced AoE Sliders
            internal static UserInt
                DNC_AdvAoE_SSBurstPct       = new("DNC_AdvAoE_SSBurstPct"),     // Standard Step    target HP% threshold
                DNC_AdvAoE_TSBurstPct       = new("DNC_AdvAoE_TSBurstPct"),     // Technical Step   target HP% threshold
                DNC_AdvAoE_SaberTh          = new("DNC_AdvAoE_SaberTh"),        // Saber Dance      Esprit     threshold
                DNC_AdvAoE_PanicWaltzPct    = new("DNC_AdvAoE_PanicWaltzPct"),  // Curing Waltz     player HP% threshold
                DNC_AdvAoE_PanicWindPct     = new("DNC_AdvAoE_PanicWindPct");   // Second Wind      player HP% threshold
            #endregion

            internal static UserInt
                DNC_VariantCurePct          = new("DNC_VariantCurePct");        // Variant Cure     player HP% threshold
        }

        internal class DNC_DanceStepSolver : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceStepSolver;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Standard Step
                if (actionID is StandardStep && Gauge.IsDancing && HasEffect(Buffs.StandardStep))
                    return Gauge.CompletedSteps < 2
                        ? Gauge.NextStep
                        : StandardFinish;

                // Technical Step
                if ((actionID is TechnicalStep) && Gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
                    return Gauge.CompletedSteps < 4
                        ? Gauge.NextStep
                        : TechnicalFinish;

                return actionID;
            }
        }

        #region Legacy Features
        internal class DNC_LegacyFeatures : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_LegacyFeatures;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Dance Combo Replacer
                if (IsEnabled(CustomComboPreset.DNC_Dance_Menu) && IsEnabled(CustomComboPreset.DNC_DanceComboReplacer) && Gauge.IsDancing)
                {
                    uint[]? actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                    if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == Cascade))     // Cascade replacement
                        return OriginalHook(Cascade);
                    if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == Flourish))    // Fountain replacement
                        return OriginalHook(Fountain);
                    if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == FanDance1))   // Reverse Cascade replacement
                        return OriginalHook(ReverseCascade);
                    if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == FanDance2))   // Fountainfall replacement
                        return OriginalHook(Fountainfall);
                }
                #endregion

                #region Fan Dance Combos
                if (IsEnabled(CustomComboPreset.DNC_FanDanceCombos))
                {
                    // FD 1 --> 3, FD 1 --> 4
                    if (actionID is FanDance1)
                    {
                        if (IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo) &&
                            HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;
                        if (IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo) &&
                            HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;
                    }

                    // FD 2 --> 3, FD 2 --> 4
                    if (actionID is FanDance2)
                    {
                        if (IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo) &&
                            HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;
                        if (IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo) &&
                            HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;
                    }
                }
                #endregion

                #region Fan Dance 3 & 4 on Flourish
                if (IsEnabled(CustomComboPreset.DNC_FlourishingFanDances) && actionID is Flourish && CanWeave(actionID))
                {
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }
                #endregion

                #region ST Multibutton
                if (IsEnabled(CustomComboPreset.DNC_ST_MultiButton) && actionID is Cascade)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // ST Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) && LevelChecked(SaberDance) &&
                        Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_LegacyST_EspritTh))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // ST Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance1) && Gauge.Feathers is 4)
                            return FanDance1;

                        // ST Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // ST base combos
                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade)
                        return Fountain;
                }
                #endregion

                #region AoE Multibutton
                if (IsEnabled(CustomComboPreset.DNC_AoE_MultiButton) && actionID is Windmill)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // AoE Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) && LevelChecked(SaberDance) &&
                        Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_LegacyAoE_EspritTh))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // AoE Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                            LevelChecked(FanDance2) && Gauge.Feathers is 4)
                            return FanDance2;

                        // AoE Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // AoE base combos
                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill)
                        return Bladeshower;
                }
                #endregion

                #region Devilment --> Starfall Dance
                if (IsEnabled(CustomComboPreset.DNC_Starfall_Devilment) && actionID is Devilment && HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;
                #endregion

                #region Combined Dances
                // One-button mode for both dances (SS/TS). SS takes priority.
                if (IsEnabled(CustomComboPreset.DNC_Dance_Menu) && IsEnabled(CustomComboPreset.DNC_CombinedDances) && actionID is StandardStep)
                {
                    // Devilment
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Devilment) && IsOnCooldown(StandardStep) && IsOffCooldown(Devilment) && !Gauge.IsDancing)
                    {
                        if ((LevelChecked(Devilment) && !LevelChecked(TechnicalStep)) ||    // Lv. 62 - 69
                            (LevelChecked(TechnicalStep) && IsOnCooldown(TechnicalStep)))   // Lv. 70+ during Tech
                            return Devilment;
                    }

                    // Flourish
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Flourish) &&
                        InCombat() && !Gauge.IsDancing &&
                        ActionReady(Flourish) &&
                        IsOnCooldown(StandardStep))
                        return Flourish;

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Tech Step
                    if (IsOnCooldown(StandardStep) && IsOffCooldown(TechnicalStep) &&
                        !Gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Dance steps
                    if (Gauge.IsDancing)
                    {
                        if (HasEffect(Buffs.StandardStep))
                        {
                            return Gauge.CompletedSteps < 2
                                ? Gauge.NextStep
                                : StandardFinish;
                        }

                        if (HasEffect(Buffs.TechnicalStep))
                        {
                            return Gauge.CompletedSteps < 4
                                ? Gauge.NextStep
                                : TechnicalFinish;
                        }
                    }
                }
                #endregion

                return actionID;
            }
        }

        /* Old code
        internal class DNC_DanceComboReplacer : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceComboReplacer;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (Gauge.IsDancing)
                {
                    uint[]? actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                    if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == Cascade))     // Cascade replacement
                        return OriginalHook(Cascade);
                    if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == Flourish))    // Fountain replacement
                        return OriginalHook(Fountain);
                    if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == FanDance1))   // Reverse Cascade replacement
                        return OriginalHook(ReverseCascade);
                    if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == FanDance2))   // Fountainfall replacement
                        return OriginalHook(Fountainfall);
                }

                return actionID;
            }
        }

        internal class DNC_FanDanceCombos : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_FanDanceCombos;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // FD 1 --> 3, FD 1 --> 4
                if (actionID is FanDance1)
                {
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo) &&
                        HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo) &&
                        HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                // FD 2 --> 3, FD 2 --> 4
                if (actionID is FanDance2)
                {
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo) &&
                        HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo) &&
                        HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                return actionID;
            }
        }

        internal class DNC_FlourishingFanDances : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_FlourishingFanDances;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Fan Dance 3 & 4 on Flourish
                if (actionID is Flourish && CanWeave(actionID))
                {
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                return actionID;
            }
        }

        internal class DNC_ST_MultiButton : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_MultiButton;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // ST Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) && LevelChecked(SaberDance) &&
                        Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_LegacyST_EspritTh))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // ST Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance1) && Gauge.Feathers is 4)
                            return FanDance1;

                        // ST Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // ST base combos
                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade)
                        return Fountain;
                }

                return actionID;
            }
        }

        internal class DNC_AoE_MultiButton : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_MultiButton;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // AoE Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) && LevelChecked(SaberDance) &&
                        Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_LegacyAoE_EspritTh))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // AoE Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                            LevelChecked(FanDance2) && Gauge.Feathers is 4)
                            return FanDance2;

                        // AoE Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // AoE base combos
                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill)
                        return Bladeshower;
                }

                return actionID;
            }
        }

        internal class DNC_Starfall_Devilment : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_Starfall_Devilment;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Devilment && HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;
                else
                    return actionID;
            }
        }

        internal class DNC_CombinedDances : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_CombinedDances;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // One-button mode for both dances (SS/TS). SS takes priority.
                if (actionID is StandardStep)
                {
                    // Devilment
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Devilment) && IsOnCooldown(StandardStep) && IsOffCooldown(Devilment) && !Gauge.IsDancing)
                    {
                        if ((LevelChecked(Devilment) && !LevelChecked(TechnicalStep)) ||    // Lv. 62 - 69
                            (LevelChecked(TechnicalStep) && IsOnCooldown(TechnicalStep)))   // Lv. 70+ during Tech
                            return Devilment;
                    }

                    // Flourish
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Flourish) &&
                        InCombat() && !Gauge.IsDancing &&
                        ActionReady(Flourish) &&
                        IsOnCooldown(StandardStep))
                        return Flourish;

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Tech Step
                    if (IsOnCooldown(StandardStep) && IsOffCooldown(TechnicalStep) &&
                        !Gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Dance steps
                    if (Gauge.IsDancing)
                    {
                        if (HasEffect(Buffs.StandardStep))
                        {
                            return Gauge.CompletedSteps < 2
                                ? Gauge.NextStep
                                : StandardFinish;
                        }

                        if (HasEffect(Buffs.TechnicalStep))
                        {
                            return Gauge.CompletedSteps < 4
                                ? Gauge.NextStep
                                : TechnicalFinish;
                        }
                    }
                }

                return actionID;
            }
        }
        #endregion
        */
        #endregion

        #region Siple/Advanced Single Target Modes
        internal class DNC_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    /*
                    #region Pre-pull
                    // ST Peloton
                    if (!InCombat() && !HasEffectAny(Buffs.Peloton) && GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;
                    #endregion
                    */

                    #region Dance Fills
                    // ST Standard (Dance) Steps & Fill
                    if (HasEffect(Buffs.StandardStep))
                        return Gauge.CompletedSteps < 2
                            ? Gauge.NextStep
                            : StandardFinish;

                    // ST Technical (Dance) Steps & Fill
                    if (HasEffect(Buffs.TechnicalStep))
                        return Gauge.CompletedSteps < 4
                            ? Gauge.NextStep
                            : TechnicalFinish;
                    #endregion

                    #region Weaves
                    // ST Devilment
                    if (CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // ST Flourish
                    if (CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish) &&
                        !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // ST Interrupt
                    if (CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_VariantCure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNC_VariantCurePct))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        // ST Feathers & Fans
                        if (LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // FD1 HP% Dump
                            if (GetTargetHPPercent() <= 3 && Gauge.Feathers > 0)
                                return FanDance1;

                            if (LevelChecked(TechnicalStep))
                            {
                                // Burst FD1
                                if (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)
                                    return FanDance1;

                                // FD1 Pooling
                                if (Gauge.Feathers > 3 &&
                                    (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                    return FanDance1;
                            }

                            // FD1 Non-pooling & under burst level
                            if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                                return FanDance1;

                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // ST Panic Heals
                        if (ActionReady(CuringWaltz) &&
                            PlayerHealthPercentageHp() < 50)
                            return CuringWaltz;

                        if (ActionReady(All.SecondWind) &&
                            PlayerHealthPercentageHp() < 30)
                            return All.SecondWind;

                        /*
                        // ST Improvisation
                        if (ActionReady(Improvisation))
                            return Improvisation;
                        */
                    }
                    #endregion

                    #region GCD
                    // ST Standard Step (outside of burst)
                    if (ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > 5) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // ST Technical Step
                    if (ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > 5) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // ST Saber Dance
                    if (LevelChecked(SaberDance) && (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (Gauge.Esprit >= 85 || (HasEffect(Buffs.TechnicalFinish) && Gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // ST combos and burst attacks
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime is < 2 and > 0)
                        return Fountain;

                    // ST Standard Step (inside of burst)
                    if (IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > 5 && (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                        return Fountain;
                    #endregion
                }

                return actionID;
            }
        }

        internal class DNC_AdvST : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AdvST;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    #region Pre-pull
                    // ST Peloton
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_Peloton) &&
                        !InCombat() && !HasEffectAny(Buffs.Peloton) && GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;
                    #endregion

                    #region Dance Fills
                    // ST Standard (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AdvST_SS) || IsEnabled(CustomComboPreset.DNC_AdvST_StandardFill)) &&
                        HasEffect(Buffs.StandardStep))
                        return Gauge.CompletedSteps < 2
                            ? Gauge.NextStep
                            : StandardFinish;

                    // ST Technical (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AdvST_TS) || IsEnabled(CustomComboPreset.DNC_AdvST_TechFill)) &&
                        HasEffect(Buffs.TechnicalStep))
                        return Gauge.CompletedSteps < 4
                            ? Gauge.NextStep
                            : TechnicalFinish;
                    #endregion

                    #region Weaves
                    // ST Devilment
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_Devilment) &&
                        CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // ST Flourish
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_Flourish) &&
                        CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish) &&
                        !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // ST Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_Interrupt) &&
                        CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_VariantCure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNC_VariantCurePct))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        // ST Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_AdvST_Feathers) && LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // FD1 HP% Dump
                            if (GetTargetHPPercent() <= PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_FeatherBurstPct) && Gauge.Feathers > 0)
                                return FanDance1;

                            if (LevelChecked(TechnicalStep))
                            {
                                // Burst FD1
                                if (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)
                                    return FanDance1;

                                // FD1 Pooling
                                if (Gauge.Feathers > 3 &&
                                    (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                    return FanDance1;
                            }

                            // FD1 Non-pooling & under burst level
                            if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                                return FanDance1;

                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // ST Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AdvST_PanicHeals))
                        {
                            if (ActionReady(CuringWaltz) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_PanicWaltzPct))
                                return CuringWaltz;

                            if (ActionReady(All.SecondWind) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_PanicWindPct))
                                return All.SecondWind;
                        }

                        // ST Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_AdvST_Improvisation) &&
                            ActionReady(Improvisation))
                            return Improvisation;
                    }
                    #endregion

                    #region GCD
                    // ST Standard Step (outside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_SS) && ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_SSBurstPct)) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // ST Technical Step
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_TS) && ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_TSBurstPct)) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // ST Saber Dance
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_SaberDance) && LevelChecked(SaberDance) &&
                        (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_SaberTh) ||
                            (HasEffect(Buffs.TechnicalFinish) && Gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // ST combos and burst attacks
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime is < 2 and > 0)
                        return Fountain;

                    // ST Standard Step (inside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AdvST_SS) && IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvST_SSBurstPct) &&
                            (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                        return Fountain;
                    #endregion
                }

                return actionID;
            }
        }
        #endregion

        #region Siple/Advanced AoE Modes
        internal class DNC_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    #region Dance Fills
                    // AoE Standard (Dance) Steps & Fill
                    if (HasEffect(Buffs.StandardStep))
                        return Gauge.CompletedSteps < 2
                            ? Gauge.NextStep
                            : StandardFinish;

                    // AoE Technical (Dance) Steps & Fill
                    if (HasEffect(Buffs.TechnicalStep))
                        return Gauge.CompletedSteps < 4
                            ? Gauge.NextStep
                            : TechnicalFinish;
                    #endregion

                    #region Weaves
                    // AoE Devilment
                    if (CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // AoE Flourish
                    if (CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish) &&
                        !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // AoE Interrupt
                    if (CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_VariantCure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNC_VariantCurePct))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        /*
                        // AoE Feathers & Fans
                        if (LevelChecked(FanDance1))
                        {
                            int minFeathers = LevelChecked(TechnicalStep)
                                ? (GetCooldownRemainingTime(TechnicalStep) < 2.5f ? 4 : 3)
                                : 0;

                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if ((Gauge.Feathers > minFeathers ||
                                (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)) &&
                                LevelChecked(FanDance2))
                                return FanDance2;
                        }
                        */

                        // AoE Feathers & Fans
                        if (LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if (LevelChecked(FanDance2))
                            {
                                if (LevelChecked(TechnicalStep))
                                {
                                    // Burst FD2
                                    if (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)
                                        return FanDance2;

                                    // FD2 Pooling
                                    if (Gauge.Feathers > 3 &&
                                        (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                        return FanDance2;
                                }

                                // FD2 Non-pooling & under burst level
                                if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                                    return FanDance2;
                            }

                            // FD1 Replacement for Lv.30-49
                            if (!LevelChecked(FanDance2) && Gauge.Feathers > 0)
                                return FanDance1;
                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // AoE Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AdvAoE_PanicHeals))
                        {
                            if (ActionReady(CuringWaltz) &&
                                PlayerHealthPercentageHp() < 50)
                                return CuringWaltz;

                            if (ActionReady(All.SecondWind) &&
                                PlayerHealthPercentageHp() < 30)
                                return All.SecondWind;
                        }

                        /*
                        // AoE Improvisation
                        if (ActionReady(Improvisation))
                            return Improvisation;
                        */
                    }
                    #endregion

                    #region GCD
                    // AoE Standard Step (outside of burst)
                    if (ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > 5) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // AoE Technical Step
                    if (ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > 5) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // AoE Saber Dance
                    if (LevelChecked(SaberDance) && (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (Gauge.Esprit >= 80 ||
                            (HasEffect(Buffs.TechnicalFinish) && Gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // AoE combos and burst attacks
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime is < 2 and > 0)
                        return Bladeshower;

                    // AoE Standard Step (inside of burst)
                    if (IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > 5 &&
                            (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                        return Bladeshower;
                    #endregion
                }

                return actionID;
            }
        }

        internal class DNC_AdvAoE : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AdvAoE;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    #region Types
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    #region Dance Fills
                    // AoE Standard (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AdvAoE_SS) || IsEnabled(CustomComboPreset.DNC_AdvAoE_StandardFill)) &&
                        HasEffect(Buffs.StandardStep))
                        return Gauge.CompletedSteps < 2
                            ? Gauge.NextStep
                            : StandardFinish;

                    // AoE Technical (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AdvAoE_TS) || IsEnabled(CustomComboPreset.DNC_AdvAoE_TechFill)) &&
                        HasEffect(Buffs.TechnicalStep))
                        return Gauge.CompletedSteps < 4
                            ? Gauge.NextStep
                            : TechnicalFinish;
                    #endregion

                    #region Weaves
                    // AoE Devilment
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Devilment) &&
                        CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // AoE Flourish
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Flourish) &&
                        CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish)
                        && !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // AoE Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Interrupt) &&
                        CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_VariantCure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNC_VariantCurePct))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        /*
                        // AoE Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Feathers) && LevelChecked(FanDance1))
                        {
                            int minFeathers = IsEnabled(CustomComboPreset.DNC_AdvAoE_FeatherPooling) && LevelChecked(TechnicalStep)
                                ? (GetCooldownRemainingTime(TechnicalStep) < 2.5f ? 4 : 3)
                                : 0;

                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if ((Gauge.Feathers > minFeathers ||
                                (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)) &&
                                LevelChecked(FanDance2))
                                return FanDance2;
                        }
                        */

                        // AoE Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Feathers) && LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if (LevelChecked(FanDance2))
                            {
                                if (LevelChecked(TechnicalStep))
                                {
                                    // Burst FD2
                                    if (HasEffect(Buffs.TechnicalFinish) && Gauge.Feathers > 0)
                                        return FanDance2;

                                    // FD2 Pooling
                                    if (Gauge.Feathers > 3 &&
                                        (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                        return FanDance2;
                                }

                                // FD2 Non-pooling & under burst level
                                if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                                    return FanDance2;
                            }

                            // FD1 Replacement for Lv.30-49
                            if (!LevelChecked(FanDance2) && Gauge.Feathers > 0)
                                return FanDance1;
                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // AoE Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AdvAoE_PanicHeals))
                        {
                            if (ActionReady(CuringWaltz) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_PanicWaltzPct))
                                return CuringWaltz;

                            if (ActionReady(All.SecondWind) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_PanicWindPct))
                                return All.SecondWind;
                        }

                        // AoE Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_AdvAoE_Improvisation) &&
                            ActionReady(Improvisation))
                            return Improvisation;
                    }
                    #endregion

                    #region GCD
                    // AoE Standard Step (outside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_SS) && ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_SSBurstPct)) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // AoE Technical Step
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_TS) && ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_TSBurstPct)) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // AoE Saber Dance
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_SaberDance) && LevelChecked(SaberDance) &&
                        (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (Gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_SaberTh) ||
                            (HasEffect(Buffs.TechnicalFinish) && Gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // AoE combos and burst attacks
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime is < 2 and > 0)
                        return Bladeshower;

                    // AoE Standard Step (inside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AdvAoE_SS) && IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNC_AdvAoE_SSBurstPct) &&
                            (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                        return Bladeshower;
                    #endregion
                }

                return actionID;
            }
        }
        #endregion
    }
}
