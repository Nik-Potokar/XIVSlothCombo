using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;

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
            StandardFinish0 = 16003,
            StandardFinish1 = 16191,
            StandardFinish2 = 16192,
            TechnicalFinish0 = 16004,
            TechnicalFinish1 = 16193,
            TechnicalFinish2 = 16194,
            TechnicalFinish3 = 16195,
            TechnicalFinish4 = 16196,
            // Fan Dances
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            // Other
            Peloton = 7557,
            SaberDance = 16005,
            EnAvant = 16010,
            Devilment = 16011,
            ShieldSamba = 16012,
            Flourish = 16013,
            Improvisation = 16014,
            CuringWaltz = 16015;

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

        /*
        public static class Debuffs
        {
            public const short placeholder = 0;
        }
        */

        public static class Config
        {
            public const string
                DNCEspritThreshold_ST = "DNCEspritThreshold_ST";                            // Single target Esprit threshold
            public const string
                DNCEspritThreshold_AoE = "DNCEspritThreshold_AoE";                          // AoE Esprit threshold

            #region Simple ST Sliders
            public const string
                DNCSimpleSSBurstPercent = "DNCSimpleSSBurstPercent";                        // Standard Step    target HP% threshold
            public const string
                DNCSimpleTSBurstPercent = "DNCSimpleTSBurstPercent";                        // Technical Step   target HP% threshold
            public const string
                DNCSimpleFeatherBurstPercent = "DNCSimpleFeatherBurstPercent";              // Feather burst    target HP% threshold
            public const string
                DNCSimplePanicHealWaltzPercent = "DNCSimplePanicHealWaltzPercent";          // Curing Waltz     player HP% threshold
            public const string
                DNCSimplePanicHealWindPercent = "DNCSimplePanicHealWindPercent";            // Second Wind      player HP% threshold
            #endregion

            #region Simple AoE Sliders
            public const string
                DNCSimpleSSAoEBurstPercent = "DNCSimpleSSAoEBurstPercent";                  // Standard Step    target HP% threshold
            public const string
                DNCSimpleTSAoEBurstPercent = "DNCSimpleTSAoEBurstPercent";                  // Technical Step   target HP% threshold
            public const string
                DNCSimpleAoEPanicHealWaltzPercent = "DNCSimpleAoEPanicHealWaltzPercent";    // Curing Waltz     player HP% threshold 
            public const string
                DNCSimpleAoEPanicHealWindPercent = "DNCSimpleAoEPanicHealWindPercent";      // Second Wind      player HP% threshold
            #endregion
        }

        internal class DNC_DanceComboReplacer : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceComboReplacer;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (GetJobGauge<DNCGauge>().IsDancing)
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
                var gauge = GetJobGauge<DNCGauge>();
                var FD3Ready = HasEffect(Buffs.ThreeFoldFanDance);
                var FD4Ready = HasEffect(Buffs.FourFoldFanDance);
                var flourishReady = level >= Levels.Flourish && IsOffCooldown(Flourish);

                if (actionID is FanDance1)
                {
                    // FD 1 -> Flourish
                    if (flourishReady && IsEnabled(CustomComboPreset.DNC_FanDance_1toFlourish_Combo))
                        return Flourish;

                    // FD 1 full
                    if (gauge.Feathers is 4 && level >= Levels.FanDance1)
                        return FanDance1;

                    // FD 1 -> 4
                    if (FD4Ready && IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo))
                        return FanDance4;
                }

                if (actionID is FanDance2)
                {
                    // FD 2 -> Flourish
                    if (flourishReady && IsEnabled(CustomComboPreset.DNC_FanDance_2toFlourish_Combo))
                        return Flourish;

                    // FD 2 full
                    if (gauge.Feathers is 4 && level >= Levels.FanDance2)
                        return FanDance2;

                    // FD 2 -> 4
                    if (FD4Ready && IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo))
                        return FanDance4;
                }
            }
        }

        internal class DNC_DanceStepCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceStepCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DNCGauge? gauge = GetJobGauge<DNCGauge>();

                // Standard Step
                if (actionID is StandardStep && gauge.IsDancing && HasEffect(Buffs.StandardStep))
                    return gauge.CompletedSteps < 2
                        ? gauge.NextStep
                        : StandardFinish2;

                // Technical Step
                if ((actionID is TechnicalStep) && gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
                    return gauge.CompletedSteps < 4
                        ? gauge.NextStep
                        : TechnicalFinish4;

                return actionID;
            }
        }

        internal class DNC_FlourishingFanDances : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_FlourishingFanDances;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Fan Dance 3 & 4 on Flourish when relevant
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
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool fd3 = HasEffect(Buffs.ThreeFoldFanDance);
                    bool fd4 = HasEffect(Buffs.FourFoldFanDance);
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    bool canWeave = CanWeave(actionID);
                    int espritThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST);
                    #endregion

                    if (canWeave)
                    {
                        // ST Fan Dance 3 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34) && LevelChecked(FanDance3) && fd3)
                            return FanDance3;

                        // ST Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance1) && gauge.Feathers is 4)
                            return FanDance1;
                        
                        // ST Fan Dance 4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34) && LevelChecked(FanDance4) && fd4)
                            return FanDance4;
                    }

                    // ST Esprit overcap options
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                        LevelChecked(SaberDance) && gauge.Esprit >= espritThreshold)
                        return SaberDance;

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
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    bool canWeave = CanWeave(actionID);
                    int espritThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_AoE);
                    #endregion

                    if (canWeave)
                    {
                        // AoE Fan Dance 3 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34) && LevelChecked(FanDance3) && fd3)
                            return FanDance3;

                        // AoE Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance2) && gauge.Feathers is 4)
                            return FanDance2;
                        
                        // AoE Fan Dance 4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34) && LevelChecked(FanDance4) && fd4)
                            return FanDance4;
                    }

                    // AoE Esprit overcap
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                        LevelChecked(SaberDance) && gauge.Esprit >= espritThreshold)
                        return SaberDance;

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
                return actionID is Devilment && HasEffect(Buffs.FlourishingStarfall)
                    ? StarfallDance
                    : actionID;
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
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();

                    // Devilment
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Devilment) && IsOnCooldown(StandardStep) && IsOffCooldown(Devilment) && !gauge.IsDancing)
                    {
                        if ((LevelChecked(Devilment) && !LevelChecked(TechnicalStep)) ||    // Lv. 62 - 69
                            (LevelChecked(TechnicalStep) && IsOnCooldown(TechnicalStep)))   // Lv. 70+ during Tech
                            return Devilment;
                    }

                    // Flourish
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Flourish) && InCombat() && !gauge.IsDancing &&
                        IsOffCooldown(Flourish) && LevelChecked(Flourish) &&
                        IsOnCooldown(StandardStep))
                        return Flourish;

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Tech Step
                    if (IsOnCooldown(StandardStep) && IsOffCooldown(TechnicalStep) && !gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Dance steps
                    if (gauge.IsDancing)
                    {
                        if (HasEffect(Buffs.StandardStep))
                        {
                            return gauge.CompletedSteps < 2
                                ? gauge.NextStep
                                : StandardFinish2;
                        }

                        if (HasEffect(Buffs.TechnicalStep))
                        {
                            return gauge.CompletedSteps < 4
                                ? gauge.NextStep
                                : TechnicalFinish4;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class DNC_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool canWeave = CanWeave(actionID);
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    float techBurstTimer = GetBuffRemainingTime(Buffs.TechnicalFinish);
                    bool techBurst = HasEffect(Buffs.TechnicalFinish);
                    bool improvisationReady = LevelChecked(Improvisation) && IsOffCooldown(Improvisation);
                    bool standardStepReady = LevelChecked(StandardStep) && IsOffCooldown(StandardStep);
                    bool technicalStepReady = LevelChecked(TechnicalStep) && IsOffCooldown(TechnicalStep);
                    bool interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && LevelChecked(All.HeadGraze);
                    int standardStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent);
                    int technicalStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSBurstPercent);
                    #endregion

                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Peloton) && !InCombat() && !HasEffectAny(Buffs.Peloton) && GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;

                    // Simple ST Standard Steps & Fill Feature
                    if (HasEffect(Buffs.StandardStep) &&
                        (IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_StandardFill)))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // Simple ST Tech Steps & Fill Feature
                    if (HasEffect(Buffs.TechnicalStep) &&
                        (IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_TechFill)))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;

                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Interrupt) && interruptable)
                        return All.HeadGraze;

                    // Simple ST Standard (activates dance with no target, or when target is over HP% threshold)
                    if ((!HasTarget() || GetTargetHPPercent() > standardStepBurstThreshold) &&
                        IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) && standardStepReady &&
                        ((!HasEffect(Buffs.TechnicalStep) && !techBurst) || techBurstTimer > 5))
                        return StandardStep;

                    // Simple ST Tech (activates dance with no target, or when target is over HP% threshold)
                    if ((!HasTarget() || GetTargetHPPercent() > technicalStepBurstThreshold) &&
                        IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) && technicalStepReady && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Devilment & Flourish
                    if (canWeave)
                    {
                        bool flourishReady = LevelChecked(Flourish) && IsOffCooldown(Flourish) && !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) && !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow);
                        bool devilmentReady = LevelChecked(Devilment) && IsOffCooldown(Devilment);

                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Devilment) && devilmentReady && (techBurst || !LevelChecked(TechnicalStep)))
                            return Devilment;
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Flourish) && flourishReady)
                            return Flourish;
                    }

                    if (canWeave)
                    {
                        // Feathers
                        if (LevelChecked(FanDance1) && IsEnabled(CustomComboPreset.DNC_ST_Simple_Feathers))
                        {
                            int featherBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleFeatherBurstPercent);
                            int minFeathers = IsEnabled(CustomComboPreset.DNC_ST_Simple_FeatherPooling) && LevelChecked(TechnicalStep)
                                ? 3
                                : 0;

                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (gauge.Feathers > minFeathers ||
                                (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0) ||
                                (GetTargetHPPercent() < featherBurstThreshold && gauge.Feathers > 0))
                                return FanDance1;
                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_PanicHeals))
                        {
                            bool curingWaltzReady = LevelChecked(CuringWaltz) && IsOffCooldown(CuringWaltz);
                            bool secondWindReady = LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind);
                            int waltzThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWaltzPercent);
                            int secondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWindPercent);

                            if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                                return CuringWaltz;
                            if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                                return All.SecondWind;
                        }

                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Improvisation) && improvisationReady)
                            return Improvisation;
                    }

                    if (LevelChecked(SaberDance) && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                        return SaberDance;

                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime is < 2 and > 0)
                        return Fountain;

                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                        return Fountain;
                }

                return actionID;
            }
        }

        internal class DNC_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool canWeave = CanWeave(actionID);
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    float techBurstTimer = GetBuffRemainingTime(Buffs.TechnicalFinish);
                    bool techBurst = HasEffect(Buffs.TechnicalFinish);
                    bool improvisationReady = LevelChecked(Improvisation) && IsOffCooldown(Improvisation);
                    bool standardStepReady = LevelChecked(StandardStep) && IsOffCooldown(StandardStep);
                    bool technicalStepReady = LevelChecked(TechnicalStep) && IsOffCooldown(TechnicalStep);
                    bool interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && LevelChecked(All.HeadGraze);
                    int standardStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent);
                    int technicalStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSAoEBurstPercent);
                    #endregion

                    // Simple AoE Standard Steps & Fill Feature
                    if (HasEffect(Buffs.StandardStep) && (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_StandardFill)))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // Simple AoE Tech Steps & Fill Feature
                    if (HasEffect(Buffs.TechnicalStep) && (IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_TechFill)))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;

                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Interrupt) && interruptable)
                        return All.HeadGraze;

                    // Simple AoE Standard (activates dance with no target, or when target is over HP% threshold)
                    if ((!HasTarget() || GetTargetHPPercent() > standardStepBurstThreshold) &&
                        IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) && standardStepReady &&
                        ((!HasEffect(Buffs.TechnicalStep) && !techBurst) || techBurstTimer > 5))
                        return StandardStep;

                    // Simple AoE Tech (activates dance with no target, or when target is over HP% threshold)
                    if ((!HasTarget() || GetTargetHPPercent() > technicalStepBurstThreshold) &&
                        IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) && technicalStepReady && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    if (canWeave)
                    {
                        bool flourishReady = LevelChecked(Flourish) && IsOffCooldown(Flourish) && !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) && !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow);
                        bool devilmentReady = LevelChecked(Devilment) && IsOffCooldown(Devilment);

                        // Simple AoE Tech Devilment
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Devilment) && devilmentReady &&
                            (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                            return Devilment;
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Flourish) && flourishReady)
                            return Flourish;
                    }

                    if (LevelChecked(SaberDance) && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                        return SaberDance;

                    if (canWeave)
                    {
                        // Feathers
                        if (LevelChecked(FanDance1) && IsEnabled(CustomComboPreset.DNC_AoE_Simple_Feathers))
                        {
                            // Pooling
                            int minFeathers = IsEnabled(CustomComboPreset.DNC_AoE_Simple_FeatherPooling) && LevelChecked(TechnicalStep)
                                ? 3
                                : 0;

                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (LevelChecked(FanDance2) && (gauge.Feathers > minFeathers || (techBurst && gauge.Feathers > 0)))
                                return FanDance2;
                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_PanicHeals))
                        {
                            bool curingWaltzReady = LevelChecked(CuringWaltz) && IsOffCooldown(CuringWaltz);
                            bool secondWindReady = LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind);
                            int waltzThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWaltzPercent);
                            int secondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWindPercent);

                            if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                                return CuringWaltz;
                            if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                                return All.SecondWind;
                        }

                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Improvisation) && improvisationReady)
                            return Improvisation;
                    }

                    // Simple AoE combos and burst attacks
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime is < 2 and > 0)
                        return Bladeshower;

                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                        return Bladeshower;
                }

                return actionID;
            }
        }
    }
}
