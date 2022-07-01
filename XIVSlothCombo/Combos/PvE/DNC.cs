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
                TechnicalFinish = 1822,
                // Fan Dances
                ThreeFoldFanDance = 1820,
                FourFoldFanDance = 2699,
                // Other
                Peloton = 1199,
                ShieldSamba = 1826;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
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
                    var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                    // Cascade replacement
                    if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == Cascade))
                        return OriginalHook(Cascade);

                    // Fountain replacement
                    if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == Flourish))
                        return OriginalHook(Fountain);

                    // Reverse Cascade replacement
                    if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == FanDance1))
                        return OriginalHook(ReverseCascade);

                    // Fountainfall replacement
                    if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == FanDance2))
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
                var FD3Ready = HasEffect(Buffs.ThreeFoldFanDance);
                var FD4Ready = HasEffect(Buffs.FourFoldFanDance);

                if (actionID is FanDance1)
                {
                    // FD 1 -> 3
                    if (FD3Ready && IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo))
                        return FanDance3;

                    // FD 1 -> 4
                    if (FD4Ready && IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo))
                        return FanDance4;
                }

                if (actionID is FanDance2)
                {
                    // FD 2 -> 3
                    if (FD3Ready && IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo))
                        return FanDance3;

                    // FD 2 -> 4
                    if (FD4Ready && IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo))
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
                var gauge = GetJobGauge<DNCGauge>();

                // Standard Step
                if (actionID is StandardStep)
                {
                    if (gauge.IsDancing && HasEffect(Buffs.StandardStep))
                    {
                        if (gauge.CompletedSteps < 2)
                            return gauge.NextStep;
                        return StandardFinish2;
                    }
                }

                // Technical Step
                if ((actionID is TechnicalStep) && LevelChecked(TechnicalStep))
                {
                    if (gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
                    {
                        if (gauge.CompletedSteps < 4)
                            return gauge.NextStep;
                        return TechnicalFinish4;
                    }
                }
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
                    // FD3
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;

                    // FD4
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
                    var gauge = GetJobGauge<DNCGauge>();
                    var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                    var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                    var canWeave = CanWeave(actionID);
                    var espritThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST);
                    #endregion

                    // ST Esprit overcap options
                    if (LevelChecked(SaberDance) && gauge.Esprit >= espritThreshold && IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap))
                        return SaberDance;

                    if (canWeave)
                    {
                        // ST Fan Dance overcap protection
                        if (gauge.Feathers is 4 && LevelChecked(FanDance1) && IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap))
                            return FanDance1;

                        // ST Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance) && LevelChecked(FanDance3))
                                return FanDance3;

                            // FD4
                            if (HasEffect(Buffs.FourFoldFanDance) && LevelChecked(FanDance4))
                                return FanDance4;
                        }
                    }

                    // ST From Fountain
                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;

                    // ST From Cascade
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;

                    // ST Cascade Combo
                    if (lastComboMove is Cascade && LevelChecked(Fountain))
                        return Fountain;
                    return Cascade;
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
                    var gauge = GetJobGauge<DNCGauge>();
                    var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                    var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                    var canWeave = CanWeave(actionID);
                    var espritThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_AoE);
                    #endregion

                    // AoE Esprit overcap options
                    if (LevelChecked(SaberDance) && gauge.Esprit >= espritThreshold && IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap))
                        return SaberDance;

                    if (canWeave)
                    {
                        // AoE Fan Dance overcap protection
                        if (gauge.Feathers is 4 && LevelChecked(FanDance2) && IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap))
                            return FanDance2;

                        // AoE Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // FD4
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // AoE From Bladeshower
                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;

                    // AoE From Windmill
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;

                    // AoE Windmill Combo
                    if (lastComboMove is Windmill && LevelChecked(Bladeshower))
                        return Bladeshower;
                    return Windmill;
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
                    #region Types
                    var gauge = GetJobGauge<DNCGauge>();
                    var standardCD = GetCooldown(StandardStep);
                    var techstepCD = GetCooldown(TechnicalStep);
                    var devilmentCD = GetCooldown(Devilment);
                    var flourishCD = GetCooldown(Flourish);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    #endregion

                    // Devilment
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Devilment) && standardCD.IsCooldown && !devilmentCD.IsCooldown && !gauge.IsDancing)
                    {
                        if (LevelChecked(Devilment) && !LevelChecked(TechnicalStep) ||  // Lv.62 - 69
                            (LevelChecked(TechnicalStep) && techstepCD.IsCooldown))     // Lv. 70+ during Tech
                            return Devilment;
                    }

                    // Flourish
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Flourish) && !gauge.IsDancing && !flourishCD.IsCooldown &&
                        incombat && LevelChecked(Flourish) && standardCD.IsCooldown)
                        return Flourish;

                    // Starfall Dance
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    // Tillana
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Tech Step
                    if (standardCD.IsCooldown && !techstepCD.IsCooldown && !gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Dance steps
                    if (gauge.IsDancing)
                    {
                        // SS Steps
                        if (HasEffect(Buffs.StandardStep))
                        {
                            if (gauge.CompletedSteps < 2)
                                return gauge.NextStep;
                            return StandardFinish2;
                        }

                        // TS Steps
                        if (HasEffect(Buffs.TechnicalStep))
                        {
                            if (gauge.CompletedSteps < 4)
                                return gauge.NextStep;
                            return TechnicalFinish4;
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
                    var gauge = GetJobGauge<DNCGauge>();
                    var canWeave = CanWeave(actionID);
                    var flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    var symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    var techBurstTimer = GetBuffRemainingTime(Buffs.TechnicalFinish);
                    var techBurst = HasEffect(Buffs.TechnicalFinish);
                    var flourishReady = LevelChecked(Flourish) && IsOffCooldown(Flourish) && !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) && !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow);
                    var devilmentReady = LevelChecked(Devilment) && IsOffCooldown(Devilment);
                    var improvisationReady = LevelChecked(Improvisation) && IsOffCooldown(Improvisation);
                    var curingWaltzReady = LevelChecked(CuringWaltz) && IsOffCooldown(CuringWaltz);
                    var secondWindReady = LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind);
                    var interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && LevelChecked(All.HeadGraze);
                    var standardStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent);
                    var technicalStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSBurstPercent);
                    var featherBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleFeatherBurstPercent);
                    var waltzThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWaltzPercent);
                    var secondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWindPercent);
                    #endregion

                    // Simple Pre-pull Peloton
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Peloton) && !InCombat() && !HasEffectAny(Buffs.Peloton) && GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;

                    // Simple ST Standard Steps
                    if (HasEffect(Buffs.StandardStep) && (IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_StandardFill)))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // Simple ST Tech Steps & Fill Feature
                    if (HasEffect(Buffs.TechnicalStep) && (IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_TechFill)))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;

                    // Simple ST Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Interrupt) && interruptable)
                            return All.HeadGraze;

                    // Simple ST Standard (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || GetTargetHPPercent() > standardStepBurstThreshold)
                    {
                        if (LevelChecked(StandardStep) && IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) && IsOffCooldown(StandardStep)
                            && ((!HasEffect(Buffs.TechnicalStep) && !techBurst) || techBurstTimer > 5))
                            return StandardStep;
                    }

                    // Simple ST Tech (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || GetTargetHPPercent() > technicalStepBurstThreshold)
                    {
                        if (LevelChecked(TechnicalStep) && IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) && !HasEffect(Buffs.StandardStep) && IsOffCooldown(TechnicalStep))
                            return TechnicalStep;
                    }

                    if (canWeave)
                    {
                        // Simple ST Devilment
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Devilment) && devilmentReady)
                        {
                            if (techBurst || !LevelChecked(TechnicalStep))
                                return Devilment;
                        }

                        // Simple ST Flourish
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Flourish) && flourishReady)
                            return Flourish;
                    }

                    // Occurring within weave windows
                    if (canWeave)
                    {
                        // Simple ST Feathers
                        if (LevelChecked(FanDance1) && IsEnabled(CustomComboPreset.DNC_ST_Simple_Feathers))
                        {
                            // Simple ST FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // Simple ST Feather Pooling
                            var minFeathers = IsEnabled(CustomComboPreset.DNC_ST_Simple_FeatherPooling) && LevelChecked(TechnicalStep) ? 3 : 0;

                            // Simple ST Feather Overcap & Burst
                            if (gauge.Feathers > minFeathers || (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0) || GetTargetHPPercent() < featherBurstThreshold && gauge.Feathers > 0)
                                return FanDance1;
                        }

                        // Simple ST FD4 
                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // Simple ST Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_PanicHeals))
                        {
                            if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                                return CuringWaltz;

                            if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                                return All.SecondWind;
                        }

                        // Simple ST Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Improvisation) && improvisationReady)
                            return Improvisation;
                    }

                    // Simple ST Saber Dance
                    if (LevelChecked(SaberDance) && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                        return SaberDance;

                    // Simple ST Combos and burst attacks
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime is < 2 and > 0)
                        return Fountain;

                    // Tillana
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Starfall Dance
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    // Fountainfall
                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;

                    // Reverse Cascade
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                
                    // Fountain
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                        return Fountain;
                    return Cascade;
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
                    var gauge = GetJobGauge<DNCGauge>();
                    var canWeave = CanWeave(actionID);
                    var flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    var symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    var techBurstTimer = GetBuffRemainingTime(Buffs.TechnicalFinish);
                    var techBurst = HasEffect(Buffs.TechnicalFinish);
                    var flourishReady = LevelChecked(Flourish) && IsOffCooldown(Flourish);
                    var devilmentReady = LevelChecked(Devilment) && IsOffCooldown(Devilment);
                    var improvisationReady = LevelChecked(Improvisation) && IsOffCooldown(Improvisation);
                    var curingWaltzReady = LevelChecked(CuringWaltz) && IsOffCooldown(CuringWaltz);
                    var secondWindReady = LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind);
                    var interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && LevelChecked(All.HeadGraze);
                    var standardStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent);
                    var technicalStepBurstThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSAoEBurstPercent);
                    var waltzThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWaltzPercent);
                    var secondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWindPercent);
                    #endregion

                    // Simple AoE Standard Step (step function)
                    if (HasEffect(Buffs.StandardStep) && (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_StandardFill)))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // Simple AoE Tech Step (step function)
                    if (HasEffect(Buffs.TechnicalStep) && (IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_TechFill)))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;

                    // Simple AoE Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Interrupt) && interruptable)
                        return All.HeadGraze;

                    // Simple AoE Standard (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || GetTargetHPPercent() > standardStepBurstThreshold)
                    {
                        if (LevelChecked(StandardStep) && IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) && IsOffCooldown(StandardStep)
                            && ((!HasEffect(Buffs.TechnicalStep) && !techBurst) || techBurstTimer > 5))
                            return StandardStep;
                    }

                    // Simple AoE Tech (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || GetTargetHPPercent() > technicalStepBurstThreshold)
                    {
                        if (LevelChecked(TechnicalStep) && IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) && !HasEffect(Buffs.StandardStep) && IsOffCooldown(TechnicalStep))
                            return TechnicalStep;
                    }

                    if (canWeave)
                    {
                        // Simple AoE Tech Devilment
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Devilment) && devilmentReady)
                        {
                            if (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep))
                                return Devilment;
                        }

                        // Simple AoE Flourish
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Flourish) && flourishReady)
                            return Flourish;
                    }

                    // Simple AoE Saber Dance
                    if (LevelChecked(SaberDance) && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                        return SaberDance;

                    // Occurring within weave windows
                    if (canWeave)
                    {
                        // Simple AoE Feathers
                        if (LevelChecked(FanDance1) && IsEnabled(CustomComboPreset.DNC_AoE_Simple_Feathers))
                        {
                            // Simple AoE Feather Pooling
                            var minFeathers = IsEnabled(CustomComboPreset.DNC_AoE_Simple_FeatherPooling) && LevelChecked(TechnicalStep) ? 3 : 0;

                            // Simple AoE FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // Simple AoE Overcap & Burst
                            if (LevelChecked(FanDance2))
                            {
                                if (gauge.Feathers > minFeathers || (techBurst && gauge.Feathers > 0))
                                    return FanDance2;
                            }
                        }

                        // Simple AoE FD4 
                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // Simple AoE Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_PanicHeals))
                        {
                            // Curing Waltz
                            if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                                return CuringWaltz;

                            // Second Wind
                            if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                                return All.SecondWind;
                        }

                        // Simple AoE Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Improvisation) && improvisationReady)
                            return Improvisation;
                    }

                    // Simple AoE combos and burst attacks
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime is < 2 and > 0)
                        return Bladeshower;

                    // Tillana
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Starfall Dance
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    // Bloodshower
                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;

                    // Rising Windmill
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;

                    // Bladeshower
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                        return Bladeshower;
                }
                return actionID;
            }
        }
    }
}
