using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
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
                FlourishingCascade = 1814,
                FlourishingFountain = 1815,
                FlourishingWindmill = 1816,
                FlourishingShower = 1817,
                StandardStep = 1818,
                TechnicalStep = 1819,
                ShieldSamba = 1826,
                SilkenSymmetry = 2693,
                SilkenFlow = 2694,
                FlourishingSymmetry = 3017,
                FlourishingFlow = 3018,
                FlourishingFanDance = 1820,
                FlourishingStarfall = 2700,
                FlourishingFinish = 2698,
                ThreeFoldFanDance = 1820,
                FourFoldFanDance = 2699,
                TechnicalFinish = 1822;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Fountain = 2,
                StandardStep = 15,
                ReverseCascade = 20,
                Bladeshower = 25,
                FanDance1 = 30,
                RisingWindmill = 35,
                Fountainfall = 40,
                Bloodshower = 45,
                FanDance2 = 50,
                EnAvant = 50,
                CuringWaltz = 52,
                ShieldSamba = 56,
                ClosedPosition = 60,
                Devilment = 62,
                FanDance3 = 66,
                TechnicalStep = 70,
                Flourish = 72,
                SaberDance = 76,
                Improvisation = 80,
                Tillana = 82,
                FanDance4 = 86,
                StarfallDance = 90;
        }
        public static class Config
        {
            public const string
                DNCEspritThreshold_ST = "DNCEspritThreshold_ST";
            public const string
                DNCEspritThreshold_AoE = "DNCEspritThreshold_AoE";

            #region Simple ST Sliders
            public const string
                DNCSimpleSSBurstPercent = "DNCSimpleSSBurstPercent";
            public const string
                DNCSimpleTSBurstPercent = "DNCSimpleTSBurstPercent";
            public const string
                DNCSimpleFeatherBurstPercent = "DNCSimpleFeatherBurstPercent";
            public const string
                DNCSimplePanicHealWaltzPercent = "DNCSimplePanicHealWaltzPercent";
            public const string
                DNCSimplePanicHealWindPercent = "DNCSimplePanicHealWindPercent";
            #endregion

            #region Simple AoE Sliders
            public const string
                DNCSimpleSSAoEBurstPercent = "DNCSimpleSSAoEBurstPercent";
            public const string
                DNCSimpleTSAoEBurstPercent = "DNCSimpleTSAoEBurstPercent";
            public const string
                DNCSimpleAoEPanicHealWaltzPercent = "DNCSimpleAoEPanicHealWaltzPercent";
            public const string
                DNCSimpleAoEPanicHealWindPercent = "DNCSimpleAoEPanicHealWindPercent";
            #endregion
        }

        internal class DancerDanceComboCompatibility : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceComboCompatibility;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (gauge.IsDancing)
            {
                var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == Cascade))
                    return OriginalHook(Cascade);

                if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == Flourish))
                    return OriginalHook(Fountain);

                if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == FanDance1))
                    return OriginalHook(ReverseCascade);

                if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == FanDance2))
                    return OriginalHook(Fountainfall);
            }

            return actionID;
        }
    }

    internal class DancerFanDanceFeatures : CustomCombo

    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFanDanceComboFeatures;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var FD3Ready = HasEffect(Buffs.ThreeFoldFanDance);
            var FD4Ready = HasEffect(Buffs.FourFoldFanDance);

            if (actionID is DNC.FanDance1)
            {
                // FD 1 -> 3
                if (FD3Ready && IsEnabled(CustomComboPreset.DancerFanDance1_3Combo))
                    return FanDance3;

                // FD 1 -> 4
                if (FD4Ready && IsEnabled(CustomComboPreset.DancerFanDance1_4Combo))
                    return FanDance4;
            }

            if (actionID is DNC.FanDance2)
            {
                // FD 2 -> 3
                if (FD3Ready && IsEnabled(CustomComboPreset.DancerFanDance2_3Combo))
                    return FanDance3;

                // FD 2 -> 4
                if (FD4Ready && IsEnabled(CustomComboPreset.DancerFanDance2_4Combo))
                    return FanDance4;
            }

            return actionID;
        }
    }

    internal class DancerDanceStepCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<DNCGauge>();

            // Standard Step
            if (actionID is DNC.StandardStep)
            {
                if (gauge.IsDancing && HasEffect(Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return (uint)gauge.NextStep;

                    return StandardFinish2;
                }
            }

            // Technical Step
            if ((actionID is DNC.TechnicalStep) && level >= Levels.TechnicalStep)
            {
                if (gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
                {
                    if (gauge.CompletedSteps < 4)
                        return (uint)gauge.NextStep;

                    return TechnicalFinish4;
                }
            }

            return actionID;
        }
    }

    internal class DancerFlourishFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFlourishingFanDanceFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var canWeave = CanWeave(actionID);

            // Fan Dance 3 & 4 on Flourish when relevant
            if (actionID is DNC.Flourish && canWeave)
            {
                if (HasEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;

                if (HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            return actionID;
        }
    }

    internal class DancerSingleTargetMultibutton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSingleTargetMultibutton;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Cascade)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                var canWeave = CanWeave(actionID);
                var espritThreshold = Service.Configuration.GetCustomIntValue(Config.DNCEspritThreshold_ST);

                // ST Esprit overcap options
                if (level >= Levels.SaberDance && gauge.Esprit >= espritThreshold && IsEnabled(CustomComboPreset.DancerEspritOvercapSTFeature))
                        return SaberDance;

                if (canWeave)
                {
                    // ST Fan Dance overcap protection
                    if (gauge.Feathers is 4 && level >= Levels.FanDance1 && IsEnabled(CustomComboPreset.DancerFanDanceMainComboOvercapFeature))
                        return FanDance1;

                    // ST Fan Dance 3/4 on combo
                    if (IsEnabled(CustomComboPreset.DancerFanDance34OnMainComboFeature))
                    {
                        if (HasEffect(Buffs.ThreeFoldFanDance) && level >= Levels.FanDance3)
                            return FanDance3;

                        if (HasEffect(Buffs.FourFoldFanDance) && level >= Levels.FanDance4)
                            return FanDance4;
                    }
                }

                // ST From Fountain
                if (level >= Levels.Fountainfall && flow)
                    return Fountainfall;

                // ST From Cascade
                if (level >= Levels.ReverseCascade && symmetry)
                    return ReverseCascade;

                // ST Cascade Combo
                if (lastComboMove is DNC.Cascade && level >= Levels.Fountain)
                    return Fountain;

                return Cascade;
            }

            return actionID;
        }
    }

    internal class DancerAoeMultibutton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerAoEMultibutton;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Windmill)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                var canWeave = CanWeave(actionID);
                var espritThreshold = Service.Configuration.GetCustomIntValue(Config.DNCEspritThreshold_AoE);

                // AoE Esprit overcap options
                if (level >= Levels.SaberDance && gauge.Esprit >= espritThreshold && IsEnabled(CustomComboPreset.DancerEspritOvercapAoEFeature))
                    return SaberDance;

                if (canWeave)
                {
                    // AoE Fan Dance overcap protection
                    if (gauge.Feathers is 4 && level >= Levels.FanDance2 && IsEnabled(CustomComboPreset.DancerFanDanceAoEComboOvercapFeature))
                        return FanDance2;

                    // AoE Fan Dance 3/4 on combo
                    if (IsEnabled(CustomComboPreset.DancerFanDance34OnMainComboFeature))
                    {
                        if (HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;
                    }
                }

                // AoE From Bladeshower
                if (level >= Levels.Bloodshower && flow)
                    return Bloodshower;

                // AoE From Windmill
                if (level >= Levels.RisingWindmill && symmetry)
                    return RisingWindmill;

                // AoE Windmill Combo
                if (lastComboMove is DNC.Windmill && level >= Levels.Bladeshower)
                    return Bladeshower;

                return Windmill;
            }

            return actionID;
        }
    }

    internal class DancerDevilmentFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDevilmentFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Devilment && HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;

            return actionID;
        }
    }

    internal class DancerDanceStepComboTest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerCombinedDanceFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // One-button mode for both dances (SS/TS). SS takes priority.
            if (actionID is DNC.StandardStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var standardCD = GetCooldown(StandardStep);
                var techstepCD = GetCooldown(TechnicalStep);
                var devilmentCD = GetCooldown(Devilment);
                var flourishCD = GetCooldown(Flourish);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (IsEnabled(CustomComboPreset.DancerDevilmentOnCombinedDanceFeature) && standardCD.IsCooldown && !devilmentCD.IsCooldown && !gauge.IsDancing)
                {
                    if ((level >= Levels.Devilment && level < Levels.TechnicalStep) ||
                        (level >= Levels.TechnicalStep && techstepCD.IsCooldown))
                        return Devilment;
                }

                if (IsEnabled(CustomComboPreset.DancerFlourishOnCombinedDanceFeature) && !gauge.IsDancing && !flourishCD.IsCooldown &&
                    incombat && level >= Levels.Flourish && standardCD.IsCooldown)
                    return Flourish;

                if (HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;

                if (HasEffect(Buffs.FlourishingFinish))
                    return Tillana;

                if (standardCD.IsCooldown && !techstepCD.IsCooldown && !gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                    return TechnicalStep;

                if (gauge.IsDancing)
                {
                    if (HasEffect(Buffs.StandardStep))
                    {
                        if (gauge.CompletedSteps < 2)
                            return (uint)gauge.NextStep;

                        return StandardFinish2;
                    }

                    if (HasEffect(Buffs.TechnicalStep))
                    {
                        if (gauge.CompletedSteps < 4)
                            return (uint)gauge.NextStep;

                        return TechnicalFinish4;
                    }
                }

            }
                return actionID;
        }
    }

    internal class DancerSimpleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSimpleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Cascade)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var canWeave = CanWeave(actionID);
                var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                var techBurstTimer = FindEffect(Buffs.TechnicalFinish);
                var techBurst = HasEffect(Buffs.TechnicalFinish);
                var flourishReady = level >= Levels.Flourish && IsOffCooldown(Flourish);
                var devilmentReady = level >= Levels.Devilment && IsOffCooldown(Devilment);
                var improvisationReady = level >= Levels.Improvisation && IsOffCooldown(Improvisation);
                var curingWaltzReady = level >= Levels.CuringWaltz && IsOffCooldown(CuringWaltz);
                var secondWindReady = level >= All.Levels.SecondWind && IsOffCooldown(All.SecondWind);
                var interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && level >= All.Levels.HeadGraze;
                var standardStepBurstThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent);
                var technicalStepBurstThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent);
                var featherBurstThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleFeatherBurstPercent);
                var waltzThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimplePanicHealWaltzPercent);
                var secondWindThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimplePanicHealWindPercent);

                // Simple ST Tech Steps
                if (HasEffect(Buffs.TechnicalStep) && (IsEnabled(CustomComboPreset.DancerSimpleTechnicalFeature) || IsEnabled(CustomComboPreset.DancerSimpleStepFillFeature)))
                    return gauge.CompletedSteps < 4
                        ? (uint)gauge.NextStep
                        : TechnicalFinish4;

                // Simple ST Standard Steps
                if (HasEffect(Buffs.StandardStep) && (IsEnabled(CustomComboPreset.DancerSimpleStandardFeature) || IsEnabled(CustomComboPreset.DancerSimpleStepFillFeature)))
                    return gauge.CompletedSteps < 2
                        ? (uint)gauge.NextStep
                        : StandardFinish2;

                // Simple ST Interrupt
                if (IsEnabled(CustomComboPreset.DancerSimpleInterruptFeature) && interruptable)
                        return All.HeadGraze;

                // Simple ST Standard (activates dance with no target, or when target is over HP% threshold)
                if (!HasTarget() || EnemyHealthPercentage() > standardStepBurstThreshold)
                {
                    if (level >= DNC.Levels.StandardStep && IsEnabled(CustomComboPreset.DancerSimpleStandardFeature) && IsOffCooldown(DNC.StandardStep)
                        && ((!HasEffect(DNC.Buffs.TechnicalStep) && !techBurst) || techBurstTimer.RemainingTime > 5))
                        return DNC.StandardStep;
                }

                // Simple ST Tech (activates dance with no target, or when target is over HP% threshold)
                if (!HasTarget() || EnemyHealthPercentage() > technicalStepBurstThreshold)
                {
                    if (level >= DNC.Levels.TechnicalStep && IsEnabled(CustomComboPreset.DancerSimpleTechnicalFeature) && !HasEffect(DNC.Buffs.StandardStep) && IsOffCooldown(DNC.TechnicalStep))
                        return DNC.TechnicalStep;
                }

                if (canWeave)
                {
                    // Simple ST Devilment
                    if (IsEnabled(CustomComboPreset.DancerSimpleDevilmentFeature) && devilmentReady)
                    {
                        if (techBurst || (level < Levels.TechnicalStep))
                            return Devilment;
                    }

                    // Simple ST Flourish
                    if (IsEnabled(CustomComboPreset.DancerSimpleFlourishFeature) && flourishReady)
                        return Flourish;
                }
                
                // Simple ST Saber Dance
                if (level >= Levels.SaberDance && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                    return SaberDance;

                // Occurring within weave windows
                if (canWeave)
                {
                    // Simple ST Feathers
                    if (level >= Levels.FanDance1 && IsEnabled(CustomComboPreset.DancerSimpleFeatherFeature))
                    {
                        // Simple ST FD3
                        if (HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;

                        // Simple ST Feather Pooling
                        var minFeathers = IsEnabled(CustomComboPreset.DancerSimpleFeatherPoolingFeature) && level >= Levels.TechnicalStep ? 3 : 0;

                        // Simple ST Feather Overcap & Burst
                        if (gauge.Feathers > minFeathers || (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0) || EnemyHealthPercentage() < featherBurstThreshold && gauge.Feathers > 0)
                            return FanDance1;
                    }

                    // Simple ST FD4 
                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                    
                    // Simple ST Panic Heals
                    if (IsEnabled(CustomComboPreset.DancerSimplePanicHealsFeature))
                    {
                        if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                            return CuringWaltz;

                        if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                            return All.SecondWind;
                    }
                    
                    // Simple ST Improvisation
                    if (IsEnabled(CustomComboPreset.DancerSimpleImprovFeature) && improvisationReady)
                        return Improvisation;
                }

                // Simple ST Combos and burst attacks
                if (level >= Levels.Fountain && lastComboMove is DNC.Cascade && comboTime < 2 && comboTime > 0)
                    return Fountain;

                // Tillana
                if (HasEffect(Buffs.FlourishingFinish))
                    return Tillana;

                // Starfall Dance
                if (HasEffect(Buffs.FlourishingStarfall))
                    return StarfallDance;

                if (level >= Levels.Fountainfall && flow)
                    return Fountainfall;

                if (level >= Levels.ReverseCascade && symmetry)
                    return ReverseCascade;
                
                if (level >= Levels.Fountain && lastComboMove is DNC.Cascade && comboTime > 0)
                    return Fountain;

                return Cascade;
            }

            return actionID;
        }
    }

    internal class DancerSimpleAoeFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSimpleAoEFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DNC.Windmill)
                {
                    var gauge = GetJobGauge<DNCGauge>();
                    var canWeave = CanWeave(actionID);
                    var flow = (HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow));
                    var symmetry = (HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry));
                    var techBurstTimer = FindEffect(Buffs.TechnicalFinish);
                    var techBurst = HasEffect(Buffs.TechnicalFinish);
                    var flourishReady = level >= Levels.Flourish && IsOffCooldown(Flourish);
                    var devilmentReady = level >= Levels.Devilment && IsOffCooldown(Devilment);
                    var improvisationReady = level >= Levels.Improvisation && IsOffCooldown(Improvisation);
                    var curingWaltzReady = level >= Levels.CuringWaltz && IsOffCooldown(CuringWaltz);
                    var secondWindReady = level >= All.Levels.SecondWind && IsOffCooldown(All.SecondWind);
                    var interruptable = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && level >= All.Levels.HeadGraze;
                    var standardStepBurstThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent);
                    var technicalStepBurstThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent);
                    var waltzThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWaltzPercent);
                    var secondWindThreshold = Service.Configuration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWindPercent);

                    // Simple AoE Standard Step (step function)
                    if (HasEffect(Buffs.StandardStep) && (IsEnabled(CustomComboPreset.DancerSimpleAoEStandardFeature) || IsEnabled(CustomComboPreset.DancerSimpleAoEStepFillFeature)))
                        return gauge.CompletedSteps < 2
                            ? (uint)gauge.NextStep
                            : StandardFinish2;

                    // Simple AoE Tech Step (step function)
                    if (HasEffect(Buffs.TechnicalStep) && (IsEnabled(CustomComboPreset.DancerSimpleAoETechnicalFeature) || IsEnabled(CustomComboPreset.DancerSimpleAoEStepFillFeature)))
                        return gauge.CompletedSteps < 4
                            ? (uint)gauge.NextStep
                            : TechnicalFinish4;

                    // Simple AoE Interrupt
                    if (IsEnabled(CustomComboPreset.DancerSimpleAoEInterruptFeature) && interruptable)
                        return All.HeadGraze;

                    // Simple AoE Standard (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || EnemyHealthPercentage() > standardStepBurstThreshold)
                    {
                        if (level >= DNC.Levels.StandardStep && IsEnabled(CustomComboPreset.DancerSimpleAoEStandardFeature) && IsOffCooldown(DNC.StandardStep)
                            && ((!HasEffect(DNC.Buffs.TechnicalStep) && !techBurst) || techBurstTimer.RemainingTime > 5))
                            return DNC.StandardStep;
                    }

                    // Simple AoE Tech (activates dance with no target, or when target is over HP% threshold)
                    if (!HasTarget() || EnemyHealthPercentage() > technicalStepBurstThreshold)
                    {
                        if (level >= DNC.Levels.TechnicalStep && IsEnabled(CustomComboPreset.DancerSimpleAoETechnicalFeature) && !HasEffect(DNC.Buffs.StandardStep) && IsOffCooldown(DNC.TechnicalStep))
                            return DNC.TechnicalStep;
                    }

                    if (canWeave)
                    {
                        // Simple AoE Tech Devilment
                        if (IsEnabled(CustomComboPreset.DancerSimpleAoEDevilmentFeature) && devilmentReady)
                        {
                            if (HasEffect(Buffs.TechnicalFinish) || (level < Levels.TechnicalStep))
                                return Devilment;
                        }

                        // Simple AoE Flourish
                        if (IsEnabled(CustomComboPreset.DancerSimpleAoEFlourishFeature) && flourishReady)
                            return Flourish;
                    }

                    // Simple AoE Saber Dance
                    if (level >= Levels.SaberDance && (gauge.Esprit >= 85 || (techBurst && gauge.Esprit > 50)))
                        return SaberDance;

                    // Occurring within weave windows
                    if (canWeave)
                    {
                        // Simple AoE Feathers
                        if (level >= Levels.FanDance1 && IsEnabled(CustomComboPreset.DancerSimpleAoEFeatherFeature))
                        {

                            // Simple AoE Feather Pooling
                            var minFeathers = IsEnabled(CustomComboPreset.DancerSimpleAoEFeatherPoolingFeature) && level >= Levels.TechnicalStep ? 3 : 0;

                            // Simple AoE FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // Simple AoE Overcap & Burst
                            if (level >= Levels.FanDance2)
                            {
                                if (gauge.Feathers > minFeathers || (techBurst && gauge.Feathers > 0))
                                    return FanDance2;
                            }
                        }

                        // Simple AoE FD4 
                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // Simple AoE Panic Heals
                        if (IsEnabled(CustomComboPreset.DancerSimpleAoEPanicHealsFeature))
                        {
                            if (PlayerHealthPercentageHp() < waltzThreshold && curingWaltzReady)
                                return CuringWaltz;

                            if (PlayerHealthPercentageHp() < secondWindThreshold && secondWindReady)
                                return All.SecondWind;
                        }

                        // Simple AoE Improvisation
                        if (IsEnabled(CustomComboPreset.DancerSimpleAoEImprovFeature) && improvisationReady)
                            return Improvisation;
                    }

                    // Simple AoE Combos and burst attacks
                    if (level >= Levels.Bladeshower && lastComboMove is DNC.Windmill && comboTime < 2 && comboTime > 0)
                        return Bladeshower;

                    // Tillana
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Starfall Dance
                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;

                    if (level >= Levels.Bloodshower && flow)
                        return Bloodshower;

                    if (level >= Levels.RisingWindmill && symmetry)
                        return RisingWindmill;

                    if (level >= Levels.Bladeshower && lastComboMove is DNC.Windmill && comboTime > 0)
                        return Bladeshower;
                }

                return actionID;
            }
        }
    }
}
