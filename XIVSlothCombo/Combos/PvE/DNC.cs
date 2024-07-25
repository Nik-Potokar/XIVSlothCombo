using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
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
            CuringWaltz = 16015,
            LastDance = 36983,
            FinishingMove = 36984,
            DanceOfTheDawn = 36985;

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
                ShieldSamba = 1826,
                LastDanceReady = 3867,
                FinishingMoveReady = 3868,
                DanceOfTheDawnReady = 3869,
                Devilment = 1825;
        }

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
                DNCSimpleSaberThreshold = "DNCSimpleSaberThreshold";                        // Saber Dance      Esprit threshold
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
                DNCSimpleAoESaberThreshold = "DNCSimpleAoESaberThreshold";                  // Saber Dance      Esprit threshold
            public const string
                DNCSimpleAoEPanicHealWaltzPercent = "DNCSimpleAoEPanicHealWaltzPercent";    // Curing Waltz     player HP% threshold 
            public const string
                DNCSimpleAoEPanicHealWindPercent = "DNCSimpleAoEPanicHealWindPercent";      // Second Wind      player HP% threshold
            #endregion

            public static UserInt
                DNCVariantCurePercent = new("DNCVariantCurePercent");                            // Variant Cure     player HP% threshold
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
                if (actionID is TechnicalStep && gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
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

        internal class DNC_Starfall_Devilment : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_Starfall_Devilment;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Devilment && HasEffect(Buffs.FlourishingStarfall)
                    ? StarfallDance
                    : actionID;
        }

        internal class DNC_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is not Cascade) return actionID;

                #region Variables
                DNCGauge? gauge = GetJobGauge<DNCGauge>();

                var flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                var symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                var targetHpThresholdFeather = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleFeatherBurstPercent);
                var targetHpThresholdStandard = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent);
                var targetHpThresholdTechnical = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSBurstPercent);

                var needToTech =
                    IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) && // Enabled
                    GetCooldownRemainingTime(TechnicalStep) < 0.05 && // Up or about to be (some anti-drift)
                    !HasEffect(Buffs.StandardStep) && // After Standard
                    IsOnCooldown(StandardStep) &&
                    GetTargetHPPercent() > targetHpThresholdTechnical &&// HP% check
                    LevelChecked(TechnicalStep);

                var needToStandardOrFinish =
                    IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) && // Enabled
                    GetCooldownRemainingTime(StandardStep) < 0.05 && // Up or about to be (some anti-drift)
                    GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                    (IsOffCooldown(TechnicalStep) || // Checking burst is ready for standard
                     GetCooldownRemainingTime(TechnicalStep) > 5) && // Don't mangle
                    LevelChecked(StandardStep);

                var needToFinish =
                    HasEffect(Buffs.FinishingMoveReady) &&
                    !HasEffect(Buffs.LastDanceReady);

                var needToStandard =
                    !HasEffect(Buffs.FinishingMoveReady) &&
                    (IsOffCooldown(Flourish) ||
                     GetCooldownRemainingTime(Flourish) > 5) &&
                    !HasEffect(Buffs.TechnicalFinish);
                #endregion

                #region Pre-pull

                if (!InCombat())
                {
                    // ST Standard Step (Pre-pull)
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SS_Prepull) &&
                        ActionReady(StandardStep) &&
                        !HasEffect(Buffs.FinishingMoveReady) &&
                        !HasEffect(Buffs.TechnicalFinish) &&
                        IsOffCooldown(TechnicalStep) &&
                        IsOffCooldown(StandardStep) &&
                        !HasTarget())
                        return StandardStep;

                    // ST Standard Steps (Pre-pull)
                    if ((IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) ||
                         IsEnabled(CustomComboPreset.DNC_ST_Simple_StandardFill) ||
                         IsEnabled(CustomComboPreset.DNC_ST_Simple_SS_Prepull)) &&
                        HasEffect(Buffs.StandardStep) &&
                        gauge.CompletedSteps < 2 &&
                        !HasTarget())
                        return gauge.NextStep;

                    // ST Peloton
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Peloton) &&
                        !HasEffectAny(Buffs.Peloton) &&
                        GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;
                }
                #endregion

                #region Dance Fills
                // ST Standard (Dance) Steps & Fill
                if ((IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) ||
                     IsEnabled(CustomComboPreset.DNC_ST_Simple_StandardFill)) &&
                    HasEffect(Buffs.StandardStep))
                    return gauge.CompletedSteps < 2
                        ? gauge.NextStep
                        : StandardFinish2;

                // ST Technical (Dance) Steps & Fill
                if ((IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_TechFill)) &&
                    HasEffect(Buffs.TechnicalStep))
                    return gauge.CompletedSteps < 4
                        ? gauge.NextStep
                        : TechnicalFinish4;
                #endregion

                #region Weaves
                // ST Devilment
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Devilment) &&
                    CanWeave(actionID) &&
                    LevelChecked(Devilment) &&
                    GetCooldownRemainingTime(Devilment) < 0.05 &&
                    (HasEffect(Buffs.TechnicalFinish) ||
                     WasLastAction(TechnicalFinish4) ||
                     !LevelChecked(TechnicalStep)))
                    return Devilment;

                // ST Flourish
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Flourish) &&
                    CanWeave(actionID) &&
                    ActionReady(Flourish) &&
                    !WasLastWeaponskill(TechnicalFinish4) &&
                    IsOnCooldown(Devilment) &&
                    (GetCooldownRemainingTime(Devilment) > 50 ||
                     (HasEffect(Buffs.Devilment) &&
                      GetBuffRemainingTime(Buffs.Devilment) < 19)) &&
                    !HasEffect(Buffs.ThreeFoldFanDance) &&
                    !HasEffect(Buffs.FourFoldFanDance) &&
                    !HasEffect(Buffs.FlourishingSymmetry) &&
                    !HasEffect(Buffs.FlourishingFlow) &&
                    !HasEffect(Buffs.FinishingMoveReady) &&
                    ((CombatEngageDuration().TotalSeconds < 20 &&
                      HasEffect(Buffs.TechnicalFinish)) ||
                     CombatEngageDuration().TotalSeconds > 20))
                    return Flourish;

                // ST Interrupt
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Interrupt) &&
                    CanInterruptEnemy() &&
                    ActionReady(All.HeadGraze) &&
                    !HasEffect(Buffs.TechnicalFinish))
                    return All.HeadGraze;

                // Variant Cure
                if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, Config.DNCVariantCurePercent))
                    return Variant.VariantCure;

                // Variant Rampart
                if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, actionID))
                    return Variant.VariantRampart;

                if (CanWeave(actionID) && !WasLastWeaponskill(TechnicalFinish4))
                {
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;

                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;

                    // ST Feathers & Fans
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Feathers) &&
                        LevelChecked(FanDance1))
                    {
                        // FD1 HP% Dump
                        if (GetTargetHPPercent() <= targetHpThresholdFeather && gauge.Feathers > 0)
                            return FanDance1;

                        if (LevelChecked(TechnicalStep))
                        {
                            // Burst FD1
                            if (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0)
                                return FanDance1;

                            // FD1 Pooling
                            if (gauge.Feathers > 3 &&
                                (GetCooldownRemainingTime(TechnicalStep) > 2.5f ||
                                 IsOffCooldown(TechnicalStep)))
                                return FanDance1;
                        }

                        // FD1 Non-pooling & under burst level
                        if (!LevelChecked(TechnicalStep) && gauge.Feathers > 0)
                            return FanDance1;
                    }

                    // ST Panic Heals
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_PanicHeals))
                    {
                        if (ActionReady(CuringWaltz) &&
                            PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWaltzPercent))
                            return CuringWaltz;

                        if (ActionReady(All.SecondWind) &&
                            PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWindPercent))
                            return All.SecondWind;
                    }

                    // ST Improvisation
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Improvisation) &&
                        ActionReady(Improvisation) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return Improvisation;
                }
                #endregion

                #region GCD
                // ST Technical Step
                if (needToTech)
                    return TechnicalStep;

                // ST Last Dance
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_LD) && // Enabled
                    HasEffect(Buffs.LastDanceReady) && // Ready
                    (HasEffect(Buffs.TechnicalFinish) || // Has Tech
                     !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                       GetCooldownRemainingTime(TechnicalStep) < 20 &&
                       GetBuffRemainingTime(Buffs.LastDanceReady) > GetCooldownRemainingTime(TechnicalStep) + 4) ||
                     GetBuffRemainingTime(Buffs.LastDanceReady) < 4)) // Or last second
                    return LastDance;

                // ST Standard Step (Finishing Move)
                if (needToStandardOrFinish && needToFinish)
                    return OriginalHook(FinishingMove);

                // ST Standard Step
                if (needToStandardOrFinish && needToStandard)
                    return StandardStep;

                // Emergency Starfall usage
                if (HasEffect(Buffs.FlourishingStarfall) &&
                    GetBuffRemainingTime(Buffs.FlourishingStarfall) < 4)
                    return StarfallDance;

                // ST Dance of the Dawn
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_DawnDance) &&
                    HasEffect(Buffs.DanceOfTheDawnReady) &&
                    LevelChecked(DanceOfTheDawn) &&
                    (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                     IsOffCooldown(TechnicalStep)) && // Tech is up
                    (gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) || // above esprit threshold use
                     (GetBuffRemainingTime(Buffs.DanceOfTheDawnReady) < 5 && gauge.Esprit >= 50))) // emergency use
                    return OriginalHook(DanceOfTheDawn);

                // ST Saber Dance (Emergency Use)
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SaberDance) &&
                    LevelChecked(SaberDance) &&
                    gauge.Esprit >= 80 &&
                    ActionReady(SaberDance))
                    return SaberDance;

                if (HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;

                // ST Tillana
                if (HasEffect(Buffs.FlourishingFinish) &&
                    IsEnabled(CustomComboPreset.DNC_ST_Simple_Tillana))
                    return Tillana;

                // ST Saber Dance
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SaberDance) &&
                    LevelChecked(SaberDance) &&
                    gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) || // Above esprit threshold use
                    (HasEffect(Buffs.TechnicalFinish) && gauge.Esprit >= 50) && // Burst
                    (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                     IsOffCooldown(TechnicalStep))) // Tech is up
                    return SaberDance;

                // ST combos and burst attacks
                if (LevelChecked(Fountain) &&
                    lastComboMove is Cascade &&
                    comboTime is < 2 and > 0)
                    return Fountain;

                if (LevelChecked(Fountainfall) && flow)
                    return Fountainfall;
                if (LevelChecked(ReverseCascade) && symmetry)
                    return ReverseCascade;
                if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                    return Fountain;
                #endregion

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
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // ST Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                        LevelChecked(DanceOfTheDawn) &&
                        HasEffect(Buffs.DanceOfTheDawnReady) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST))
                        return OriginalHook(DanceOfTheDawn);
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                        LevelChecked(SaberDance) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // ST Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance1) && gauge.Feathers is 4)
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

        internal class DNC_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is not Windmill) return actionID;

                #region Variables
                DNCGauge? gauge = GetJobGauge<DNCGauge>();

                bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                var targetHpThresholdStandard = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent);
                var targetHpThresholdTechnical = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSAoEBurstPercent);

                var needToTech =
                    IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) && // Enabled
                    ActionReady(TechnicalStep) && // Up
                    !HasEffect(Buffs.StandardStep) && // After Standard
                    IsOnCooldown(StandardStep) &&
                    GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                    LevelChecked(TechnicalStep);

                var needToStandardOrFinish =
                    IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) && // Enabled
                    ActionReady(StandardStep) && // Up
                    GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                    (IsOffCooldown(TechnicalStep) || // Checking burst is ready for standard
                     GetCooldownRemainingTime(TechnicalStep) > 5) && // Don't mangle
                    LevelChecked(StandardStep);

                var needToFinish =
                    HasEffect(Buffs.FinishingMoveReady) &&
                    !HasEffect(Buffs.LastDanceReady);

                var needToStandard =
                    !HasEffect(Buffs.FinishingMoveReady) &&
                    (IsOffCooldown(Flourish) ||
                     GetCooldownRemainingTime(Flourish) > 5) &&
                    !HasEffect(Buffs.TechnicalFinish);
                #endregion

                #region Dance Fills
                // AoE Standard (Dance) Steps & Fill
                if ((IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) ||
                     IsEnabled(CustomComboPreset.DNC_AoE_Simple_StandardFill)) &&
                    HasEffect(Buffs.StandardStep))
                    return gauge.CompletedSteps < 2
                        ? gauge.NextStep
                        : StandardFinish2;

                // AoE Technical (Dance) Steps & Fill
                if ((IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) ||
                     IsEnabled(CustomComboPreset.DNC_AoE_Simple_TechFill)) &&
                    HasEffect(Buffs.TechnicalStep))
                    return gauge.CompletedSteps < 4
                        ? gauge.NextStep
                        : TechnicalFinish4;
                #endregion

                #region Weaves
                // AoE Devilment
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Devilment) &&
                    CanWeave(actionID) &&
                    LevelChecked(Devilment) &&
                    GetCooldownRemainingTime(Devilment) < 0.05 &&
                    (HasEffect(Buffs.TechnicalFinish) ||
                     WasLastAction(TechnicalFinish4) ||
                     !LevelChecked(TechnicalStep)))
                    return Devilment;

                // AoE Flourish
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Flourish) &&
                    CanWeave(actionID) &&
                    ActionReady(Flourish) &&
                    !WasLastWeaponskill(TechnicalFinish4) &&
                    IsOnCooldown(Devilment) &&
                    (GetCooldownRemainingTime(Devilment) > 50 ||
                     (HasEffect(Buffs.Devilment) &&
                      GetBuffRemainingTime(Buffs.Devilment) < 19)) &&
                    !HasEffect(Buffs.ThreeFoldFanDance) &&
                    !HasEffect(Buffs.FourFoldFanDance) &&
                    !HasEffect(Buffs.FlourishingSymmetry) &&
                    !HasEffect(Buffs.FlourishingFlow) &&
                    !HasEffect(Buffs.FinishingMoveReady))
                    return Flourish;

                // AoE Interrupt
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Interrupt) &&
                    CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                    !HasEffect(Buffs.TechnicalFinish))
                    return All.HeadGraze;

                if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, Config.DNCVariantCurePercent))
                    return Variant.VariantCure;

                if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, actionID))
                    return Variant.VariantRampart;

                if (CanWeave(actionID) && !WasLastWeaponskill(TechnicalFinish4))
                {
                    // AoE Feathers & Fans
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Feathers) &&
                        LevelChecked(FanDance1))
                    {
                        // FD3
                        if (HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;

                        if (LevelChecked(FanDance2))
                        {
                            if (LevelChecked(TechnicalStep))
                            {
                                // Burst FD2
                                if (HasEffect(Buffs.TechnicalFinish) &&
                                    gauge.Feathers > 0)
                                    return FanDance2;

                                // FD2 Pooling
                                if (gauge.Feathers > 3 &&
                                    (GetCooldownRemainingTime(TechnicalStep) > 2.5f ||
                                     IsOffCooldown(TechnicalStep)))
                                    return FanDance2;
                            }

                            // FD2 Non-pooling & under burst level
                            if (!LevelChecked(TechnicalStep) &&
                                gauge.Feathers > 0)
                                return FanDance2;
                        }

                        // FD1 Replacement for Lv.30-49
                        if (!LevelChecked(FanDance2) &&
                            gauge.Feathers > 0)
                            return FanDance1;
                    }

                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;

                    // AoE Panic Heals
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_PanicHeals))
                    {
                        if (ActionReady(CuringWaltz) &&
                            PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWaltzPercent))
                            return CuringWaltz;

                        if (ActionReady(All.SecondWind) &&
                            PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWindPercent))
                            return All.SecondWind;
                    }

                    // AoE Improvisation
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Improvisation) &&
                        ActionReady(Improvisation) &&
                        !HasEffect(Buffs.TechnicalStep))
                        return Improvisation;
                }
                #endregion

                #region GCD
                // AoE Technical Step
                if (needToTech)
                    return TechnicalStep;

                // AoE Last Dance
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_LD) && // Enabled
                    HasEffect(Buffs.LastDanceReady) && // Ready
                    (HasEffect(Buffs.TechnicalFinish) || // Has Tech
                     !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                       GetCooldownRemainingTime(TechnicalStep) < 20 &&
                       GetBuffRemainingTime(Buffs.LastDanceReady) > GetCooldownRemainingTime(TechnicalStep) + 4) ||
                     GetBuffRemainingTime(Buffs.LastDanceReady) < 4)) // Or last second
                    return LastDance;

                // AoE Standard Step (Finishing Move)
                if (needToStandardOrFinish && needToFinish)
                    return OriginalHook(FinishingMove);

                // AoE Standard Step
                if (needToStandardOrFinish && needToStandard)
                    return StandardStep;

                // Emergency Starfall usage
                if (HasEffect(Buffs.FlourishingStarfall) &&
                    GetBuffRemainingTime(Buffs.FlourishingStarfall) < 4)
                    return StarfallDance;

                // AoE Dance of the Dawn
                if (IsEnabled(CustomComboPreset.DNC_ST_Simple_DawnDance) &&
                    HasEffect(Buffs.DanceOfTheDawnReady) &&
                    LevelChecked(DanceOfTheDawn) &&
                    (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                     IsOffCooldown(TechnicalStep)) && // Tech is up
                    (gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) || // above esprit threshold use
                     (GetBuffRemainingTime(Buffs.DanceOfTheDawnReady) < 5 && gauge.Esprit >= 50))) // emergency use
                    return OriginalHook(DanceOfTheDawn);

                // AoE Saber Dance (Emergency Use)
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SaberDance) &&
                    LevelChecked(SaberDance) &&
                    gauge.Esprit >= 80 &&
                    ActionReady(SaberDance))
                    return SaberDance;

                if (HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;

                // AoE Tillana
                if (HasEffect(Buffs.FlourishingFinish) &&
                    IsEnabled(CustomComboPreset.DNC_AoE_Simple_Tillana))
                    return Tillana;

                // AoE Saber Dance
                if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SaberDance) &&
                    LevelChecked(SaberDance) &&
                    gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) || // Above esprit threshold use
                    (HasEffect(Buffs.TechnicalFinish) && gauge.Esprit >= 50) && // Burst
                    (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                     IsOffCooldown(TechnicalStep))) // Tech is up
                    return SaberDance;

                // AoE combos and burst attacks
                if (LevelChecked(Bladeshower) &&
                    lastComboMove is Windmill &&
                    comboTime is < 2 and > 0)
                    return Bladeshower;

                if (LevelChecked(Bloodshower) && flow)
                    return Bloodshower;
                if (LevelChecked(RisingWindmill) && symmetry)
                    return RisingWindmill;
                if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                    return Bladeshower;
                #endregion

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
                    #endregion

                    // AoE Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                        LevelChecked(DanceOfTheDawn) &&
                        HasEffect(Buffs.DanceOfTheDawnReady) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST))
                        return OriginalHook(DanceOfTheDawn);
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                        LevelChecked(SaberDance) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_AoE))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // AoE Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                            LevelChecked(FanDance2) && gauge.Feathers is 4)
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
    }
}
