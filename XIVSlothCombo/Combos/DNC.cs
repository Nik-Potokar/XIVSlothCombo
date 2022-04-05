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
            CuringWaltz = 16015,
            // Role
            SecondWind = 7541,
            Peloton = 7557,
            HeadGraze = 7551;

        public static class Buffs
        {
            public const ushort
                FlourishingCascade = 1814,
                FlourishingFountain = 1815,
                FlourishingWindmill = 1816,
                FlourishingShower = 1817,
                StandardStep = 1818,
                TechnicalStep = 1819,
                FlourishingSymmetry = 2693,
                FlourishingFlow = 2694,
                FlourishingFanDance = 1820,
                FlourishingStarfall = 2700,
                FlourishingFinish = 2698,
                ThreeFoldFanDance = 1820,
                FourFoldFanDance = 2699,
                Peloton = 1199,
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
                SecondWind = 8,
                StandardStep = 15,
                ReverseCascade = 20,
                Peloton = 20,
                HeadGraze = 24,
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

                if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == DNC.Cascade))
                    return OriginalHook(DNC.Cascade);

                if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == DNC.Flourish))
                    return OriginalHook(DNC.Fountain);

                if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == DNC.FanDance1))
                    return OriginalHook(DNC.ReverseCascade);

                if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == DNC.FanDance2))
                    return OriginalHook(DNC.Fountainfall);
            }

            return actionID;
        }
    }

    internal class DancerFanDanceFeatures : CustomCombo

    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFanDanceComboFeatures;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.FanDance1)
            {
                // FD 1 -> 3
                if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreeFoldFanDance) && IsEnabled(CustomComboPreset.DancerFanDance1_3Combo))
                    return DNC.FanDance3;
                // FD 1 -> 4
                if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance) && IsEnabled(CustomComboPreset.DancerFanDance1_4Combo))
                    return DNC.FanDance4;
            }

            if (actionID is DNC.FanDance2)
            {
                // FD 2 -> 3
                if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreeFoldFanDance) && IsEnabled(CustomComboPreset.DancerFanDance2_3Combo))
                    return DNC.FanDance3;
                // FD 2 -> 4
                if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance) && IsEnabled(CustomComboPreset.DancerFanDance2_4Combo))
                    return DNC.FanDance4;
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
                if (gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return (uint)gauge.NextStep;

                    return DNC.StandardFinish2;
                }
            }

            // Technical Step
            if ((actionID is DNC.TechnicalStep) && level >= DNC.Levels.TechnicalStep)
            {
                if (gauge.IsDancing && HasEffect(DNC.Buffs.TechnicalStep))
                {
                    if (gauge.CompletedSteps < 4)
                        return (uint)gauge.NextStep;

                    return DNC.TechnicalFinish4;
                }
            }

            return actionID;
        }
    }

    internal class DancerFlourishFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFlourishProcFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Flourish)
            {
                var canWeave = CanWeave(actionID);

                // Fan Dance Weave Option
                if (IsEnabled(CustomComboPreset.DancerFlourishProcFanDanceWeaveOption) && canWeave)
                {
                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance))
                        return DNC.FanDance3;

                    if (HasEffect(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                }

                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;

                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Bloodshower;

                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.RisingWindmill;

                return DNC.Flourish;
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
                var canWeave = CanWeave(actionID);

                // Esprit overcap options
                if (level >= DNC.Levels.SaberDance)
                {
                    if (gauge.Esprit >= 50 && IsEnabled(CustomComboPreset.DancerEspritOvercapSTInstantOption))
                        return DNC.SaberDance;

                    if (gauge.Esprit >= 85 && IsEnabled(CustomComboPreset.DancerEspritOvercapSTFeature))
                        return DNC.SaberDance;
                }

                if (canWeave)
                {
                    // Fan Dance overcap protection
                    if (gauge.Feathers is 4 && level >= DNC.Levels.FanDance1 && IsEnabled(CustomComboPreset.DancerFanDanceMainComboOvercapFeature))
                        return DNC.FanDance1;

                    // Fan Dance 3/4 on combo
                    if (IsEnabled(CustomComboPreset.DancerFanDance34OnMainComboFeature))
                    {
                        if (HasEffect(DNC.Buffs.ThreeFoldFanDance) && level >= DNC.Levels.FanDance3)
                            return DNC.FanDance3;

                        if (HasEffect(DNC.Buffs.FourFoldFanDance) && level >= DNC.Levels.FanDance4)
                            return DNC.FanDance4;
                    }
                }

                // From Fountain
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                // From Cascade
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;

                // Cascade Combo
                if (lastComboMove is DNC.Cascade && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;

                return DNC.Cascade;
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
                var canWeave = CanWeave(actionID);

                // Esprit Overcap Options
                if (gauge.Esprit >= 50 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerEspritOvercapAoEInstantOption))
                    return DNC.SaberDance;

                if (gauge.Esprit >= 85 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerEspritOvercapAoEFeature))
                    return DNC.SaberDance;

                // FanDances
                if (canWeave && IsEnabled(CustomComboPreset.DancerFanDanceOnAoEComboFeature))
                {
                    if (gauge.Feathers is 4 && level >= DNC.Levels.FanDance2)
                        return DNC.FanDance2;

                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance) && level >= DNC.Levels.FanDance3)
                        return DNC.FanDance3;

                    if (HasEffect(DNC.Buffs.FourFoldFanDance) && level >= DNC.Levels.FanDance4)
                        return DNC.FanDance4;
                }

                // From Bladeshower
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Bloodshower;

                // From Windmill
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.RisingWindmill;

                // Windmill Combo
                if (lastComboMove is DNC.Windmill && level >= DNC.Levels.Bladeshower)
                    return DNC.Bladeshower;

                return DNC.Windmill;
            }

            return actionID;
        }
    }

    internal class DancerDevilmentFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDevilmentFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Devilment)
            {
                if (level >= DNC.Levels.StarfallDance && HasEffect(DNC.Buffs.FlourishingStarfall))
                    return DNC.StarfallDance;
            }

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
                var standardCD = GetCooldown(DNC.StandardStep);
                var techstepCD = GetCooldown(DNC.TechnicalStep);
                var devilmentCD = GetCooldown(DNC.Devilment);
                var flourishCD = GetCooldown(DNC.Flourish);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (IsEnabled(CustomComboPreset.DancerDevilmentOnCombinedDanceFeature) && standardCD.IsCooldown && !devilmentCD.IsCooldown && !gauge.IsDancing)
                {
                    if (level >= DNC.Levels.Devilment && level < DNC.Levels.TechnicalStep)
                        return DNC.Devilment;

                    if (level >= DNC.Levels.TechnicalStep && techstepCD.IsCooldown)
                        return DNC.Devilment;
                }

                if (IsEnabled(CustomComboPreset.DancerFlourishOnCombinedDanceFeature) && !gauge.IsDancing && !flourishCD.IsCooldown && incombat && level >= DNC.Levels.Flourish && standardCD.IsCooldown)
                {
                    return DNC.Flourish;
                }

                if (HasEffect(DNC.Buffs.FlourishingStarfall))
                {
                    return DNC.StarfallDance;
                }

                if (HasEffect(DNC.Buffs.FlourishingFinish))
                {
                    return DNC.Tillana;
                }

                if (standardCD.IsCooldown && !techstepCD.IsCooldown && !gauge.IsDancing && !HasEffect(DNC.Buffs.StandardStep))
                {
                    return DNC.TechnicalStep;
                }

                if (gauge.IsDancing)
                {
                    if (HasEffect(DNC.Buffs.StandardStep))
                    {
                        if (gauge.CompletedSteps < 2)
                            return (uint)gauge.NextStep;

                        return DNC.StandardFinish2;
                    }

                    if (HasEffect(DNC.Buffs.TechnicalStep))
                    {
                        if (gauge.CompletedSteps < 4)
                            return (uint)gauge.NextStep;

                        return DNC.TechnicalFinish4;
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
                
                // Simple Interrupt
                if (level >= DNC.Levels.HeadGraze && IsEnabled(CustomComboPreset.DancerSimpleInterruptFeature))
                {
                    if (CanInterruptEnemy() && IsOffCooldown(DNC.HeadGraze))
                    {
                        return DNC.HeadGraze;
                    }
                }

                // Simple Tech Step
                if (HasEffect(DNC.Buffs.TechnicalStep) && IsEnabled(CustomComboPreset.DancerSimpleTechnicalFeature))
                    return gauge.CompletedSteps < 4
                        ? (uint)gauge.NextStep
                        : DNC.TechnicalFinish4;

                // Simple Standard Step
                if (HasEffect(DNC.Buffs.StandardStep) && IsEnabled(CustomComboPreset.DancerSimpleStandardFeature))
                    return gauge.CompletedSteps < 2
                        ? (uint)gauge.NextStep
                        : DNC.StandardFinish2;
                

                if (!HasTarget() || EnemyHealthPercentage() > 5)
                {
                    if (level >= DNC.Levels.StandardStep && IsEnabled(CustomComboPreset.DancerSimpleStandardFeature) && !HasEffect(DNC.Buffs.TechnicalStep) && IsOffCooldown(DNC.StandardStep))
                        return DNC.StandardStep;

                    if (level >= DNC.Levels.TechnicalStep && IsEnabled(CustomComboPreset.DancerSimpleTechnicalFeature) && !HasEffect(DNC.Buffs.StandardStep) && IsOffCooldown(DNC.TechnicalStep))
                        return DNC.TechnicalStep;
                }

                // Simple Devilment
                if (IsEnabled(CustomComboPreset.DancerSimpleDevilmentFeature) && canWeave && level >= DNC.Levels.Devilment && (HasEffect(DNC.Buffs.TechnicalFinish) && IsOffCooldown(DNC.Devilment)))
                        return DNC.Devilment;
                
                // Simple Flourish
                if (IsEnabled(CustomComboPreset.DancerSimpleFlourishFeature) && canWeave && level >= DNC.Levels.Flourish && IsOffCooldown(DNC.Flourish))
                        return DNC.Flourish;
                
                // Simple Saber Dance
                if (level >= DNC.Levels.SaberDance && (gauge.Esprit >= 85 || (HasEffect(DNC.Buffs.TechnicalFinish) && gauge.Esprit > 50)))
                    return DNC.SaberDance;

                // Occurring within weave windows
                if (canWeave)
                {
                    // Simple Feathers
                    if (level >= DNC.Levels.FanDance1 && IsEnabled(CustomComboPreset.DancerSimpleFeatherFeature))
                    {
                        if (HasEffect(DNC.Buffs.ThreeFoldFanDance))
                            return DNC.FanDance3;

                        var minFeathers = IsEnabled(CustomComboPreset.DancerSimpleFeatherPoolingFeature) && level >= DNC.Levels.TechnicalStep ? 3 : 0;

                        if (gauge.Feathers > minFeathers || (HasEffect(DNC.Buffs.TechnicalFinish) && gauge.Feathers > 0))
                            return DNC.FanDance1;
                    }

                    if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                    
                    // Simple Panic Heals
                    if (IsEnabled(CustomComboPreset.DancerSimplePanicHealsFeature))
                    {
                        if (level >= DNC.Levels.CuringWaltz && PlayerHealthPercentageHp() < 30 && IsOffCooldown(DNC.CuringWaltz))
                            return DNC.CuringWaltz;
                        if (level >= DNC.Levels.SecondWind && PlayerHealthPercentageHp() < 50 && IsOffCooldown(DNC.SecondWind))
                            return DNC.SecondWind;
                    }
                    
                    // Simple Improvisation
                    if (IsEnabled(CustomComboPreset.DancerSimpleImprovFeature) && level >= DNC.Levels.Improvisation && IsOffCooldown(DNC.Improvisation))
                        return DNC.Improvisation;
                }

                // Combos and burst attacks
                if (level >= DNC.Levels.Fountain && lastComboMove is DNC.Cascade && comboTime < 2 && comboTime > 0)
                    return DNC.Fountain;

                if (level >= DNC.Levels.Tillana && HasEffect(DNC.Buffs.FlourishingFinish))
                    return DNC.Tillana;

                if (level >= DNC.Levels.StarfallDance && HasEffect(DNC.Buffs.FlourishingStarfall))
                    return DNC.StarfallDance;

                if (level >= DNC.Levels.Fountainfall && HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                if (level >= DNC.Levels.ReverseCascade && HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;
                
                if (level >= DNC.Levels.Fountain && lastComboMove is DNC.Cascade && comboTime > 0) return DNC.Fountain;

                return DNC.Cascade;
            }

            return actionID;
        }
    }

    internal class DancerSimpleAoeFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSimpleAoeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is DNC.Windmill) 
            {
                var gauge = GetJobGauge<DNCGauge>();
                var canWeave = CanWeave(actionID);

                if (HasEffect(DNC.Buffs.StandardStep) && IsEnabled(CustomComboPreset.DancerSimpleAoeStandardFeature))
                    return gauge.CompletedSteps < 2
                        ? (uint)gauge.NextStep
                        : DNC.StandardFinish2;
                
                if (HasEffect(DNC.Buffs.TechnicalStep) && IsEnabled(CustomComboPreset.DancerSimpleAoeTechnicalFeature))
                    return gauge.CompletedSteps < 4
                        ? (uint)gauge.NextStep
                        : DNC.TechnicalFinish4;


                if (!HasTarget() || EnemyHealthPercentage() > 5)
                {
                    if (level >= DNC.Levels.StandardStep && IsEnabled(CustomComboPreset.DancerSimpleAoeStandardFeature) && !HasEffect(DNC.Buffs.TechnicalStep) && IsOffCooldown(DNC.StandardStep))
                        return DNC.StandardStep;

                    if (level >= DNC.Levels.TechnicalStep && IsEnabled(CustomComboPreset.DancerSimpleAoeTechnicalFeature) && !HasEffect(DNC.Buffs.StandardStep) && IsOffCooldown(DNC.TechnicalStep))
                        return DNC.TechnicalStep;
                }

                if (IsEnabled(CustomComboPreset.DancerSimpleAoeBuffsFeature) && canWeave)
                {
                    if (level >= DNC.Levels.Devilment && IsOffCooldown(DNC.Devilment))
                        return DNC.Devilment;

                    if (level >= DNC.Levels.Flourish && IsOffCooldown(DNC.Flourish))
                        return DNC.Flourish;
                }
                
                if (level >= DNC.Levels.SaberDance && gauge.Esprit >= 85)
                    return DNC.SaberDance;

                if (canWeave)
                {
                    if (level >= DNC.Levels.FanDance2 && IsEnabled(CustomComboPreset.DancerSimpleAoeFeatherFeature))
                    {
                        var minFeathers = (IsEnabled(CustomComboPreset.DancerSimpleAoeFeatherPoolingFeature) && level >= DNC.Levels.TechnicalStep) ? 3 : 0;

                        if (HasEffect(DNC.Buffs.ThreeFoldFanDance))
                            return DNC.FanDance3;

                        if (gauge.Feathers > minFeathers)
                            return DNC.FanDance2;
                    }

                    if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                }

                if (level >= DNC.Levels.Bladeshower && lastComboMove is DNC.Windmill && comboTime < 2 && comboTime > 0)
                    return DNC.Bladeshower;

                if (level >= DNC.Levels.Tillana && HasEffect(DNC.Buffs.FlourishingFinish))
                    return DNC.Tillana;

                if (level >= DNC.Levels.StarfallDance && HasEffect(DNC.Buffs.FlourishingStarfall))
                    return DNC.StarfallDance;

                if (level >= DNC.Levels.Bloodshower && HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Bloodshower;

                if (level >= DNC.Levels.RisingWindmill && HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.RisingWindmill;

                if (level >= DNC.Levels.Bladeshower && lastComboMove is DNC.Windmill && comboTime > 0)
                    return DNC.Bladeshower;
            }

            return actionID;
        }
    }
}
