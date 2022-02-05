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
            // Fans
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            // Other
            SaberDance = 16005,
            EnAvant = 16010,
            Flourish = 16013,
            Improvisation = 16014,
            Devilment = 16011,
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
                StandardStep = 15,
                ReverseCascade = 20,
                HeadGraze = 24,
                Bladeshower = 25,
                FanDance = 30,
                FourfoldFantasy = 30,
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

    internal class DancerFanDanceCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFanDanceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == DNC.Cascade || actionID == DNC.Windmill)
            {
                var gauge = GetJobGauge<DNCGauge>();
                if (gauge.Feathers == 0)
                {
                    if (gauge.Esprit >= 90 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerOvercapFeature))
                        return DNC.SaberDance;
                }
                if (actionID == DNC.FanDance1)
                {
                    if (HasEffect(DNC.Buffs.FlourishingFanDance))
                        return DNC.FanDance3;

                    return DNC.FanDance1;
                }

                if (actionID == DNC.FanDance2)
                {
                    if (HasEffect(DNC.Buffs.FlourishingFanDance))
                        return DNC.FanDance3;

                    return DNC.FanDance2;
                }
            }

            return actionID;
        }
    }

    internal class DancerDanceStepCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.StandardStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
                if (gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return (uint)gauge.NextStep;

                    return DNC.StandardFinish2;
                }
            }
            if (actionID == DNC.TechnicalStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFlourishFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Flourish)
            {
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
            if (actionID == DNC.Cascade)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var actionIDCD = GetCooldown(actionID);
                // Espirit Overcap Options
                if (gauge.Esprit >= 50 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerSaberDanceInstantSaberDanceComboFeature))
                    return DNC.SaberDance;
                if (gauge.Esprit >= 90 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerOvercapFeature))
                    return DNC.SaberDance;
                // FanDances
                if (gauge.Feathers == 4 && actionIDCD.IsCooldown && level >= 30 && IsEnabled(CustomComboPreset.DancerFanDanceOnMainComboFeature))
                    return DNC.FanDance1;
                if (HasEffect(DNC.Buffs.ThreeFoldFanDance) && actionIDCD.IsCooldown && level >= 66 && IsEnabled(CustomComboPreset.DancerFanDanceOnMainComboFeature))
                    return DNC.FanDance3;
                if (HasEffect(DNC.Buffs.FourFoldFanDance) && actionIDCD.IsCooldown && level >= 86 && IsEnabled(CustomComboPreset.DancerFanDanceOnMainComboFeature))
                    return DNC.FanDance4;
                // From Fountain
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                // From Cascade
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;

                // Cascade Combo
                if (lastComboMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;

                return DNC.Cascade;
            }

            return actionID;
        }
    }

    internal class DancerAoeMultibutton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerAoeMultibutton;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Windmill)
            {
                var actionIDCD = GetCooldown(actionID);
                var gauge = GetJobGauge<DNCGauge>();
                // Espirit Overcap Options
                if (gauge.Esprit >= 50 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerSaberDanceInstantSaberDanceComboFeature))
                    return DNC.SaberDance;
                if (gauge.Esprit >= 90 && level >= DNC.Levels.SaberDance && IsEnabled(CustomComboPreset.DancerOvercapFeature))
                    return DNC.SaberDance;
                // FanDances
                if (gauge.Feathers == 4 && actionIDCD.IsCooldown && level >= 50 && IsEnabled(CustomComboPreset.DancerFanDanceOnAoEComboFeature))
                    return DNC.FanDance2;
                if (HasEffect(DNC.Buffs.ThreeFoldFanDance) && actionIDCD.IsCooldown && level >= 66 && IsEnabled(CustomComboPreset.DancerFanDanceOnAoEComboFeature))
                    return DNC.FanDance3;
                if (HasEffect(DNC.Buffs.FourFoldFanDance) && actionIDCD.IsCooldown && level >= 86 && IsEnabled(CustomComboPreset.DancerFanDanceOnAoEComboFeature))
                    return DNC.FanDance4;
                // From Bladeshower
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Bloodshower;

                // From Windmill
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.RisingWindmill;

                // Windmill Combo
                if (lastComboMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
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
            if (actionID == DNC.Devilment)
            {
                if (level >= 90 && HasEffect(DNC.Buffs.FlourishingStarfall))
                    return DNC.StarfallDance;

                return DNC.Devilment;
            }

            return actionID;
        }
    }
    internal class DancerDanceStepComboTest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepComboTest;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.StandardStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
                var standardCD = GetCooldown(DNC.StandardStep);
                var techstepCD = GetCooldown(DNC.TechnicalStep);
                var devilmentCD = GetCooldown(DNC.Devilment);
                var flourishCD = GetCooldown(DNC.Flourish);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (IsEnabled(CustomComboPreset.DancerDevilmentOnCombinedDanceFeature))
                {
                    if (level >= 62 && level <= 69 && standardCD.IsCooldown && !devilmentCD.IsCooldown && !gauge.IsDancing)
                        return DNC.Devilment;
                    if (level >= 70 && standardCD.IsCooldown && techstepCD.IsCooldown && !devilmentCD.IsCooldown && !gauge.IsDancing)
                        return DNC.Devilment;
                }
                if (IsEnabled(CustomComboPreset.DancerFlourishOnCombinedDanceFeature) && !gauge.IsDancing && !flourishCD.IsCooldown && incombat && level >= 72 && standardCD.IsCooldown)
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
                if (gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return (uint)gauge.NextStep;

                    return DNC.StandardFinish2;
                }
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
    internal class DancerSaberFanDanceFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSaberFanDanceFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.FanDance1 || actionID == DNC.FanDance2 || actionID == DNC.FanDance3 || actionID == DNC.FanDance4)
            {
                var gauge = GetJobGauge<DNCGauge>();
                if (gauge.Feathers == 0 && gauge.Esprit >= 50)
                    return DNC.SaberDance;
            }

            return actionID;
        }
    }

    internal class DancerSimpleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSimpleFeature;

        internal bool inOpener = false;
        internal bool openerFinished = false;
        internal byte step = 0;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Cascade) {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<DNCGauge>();
                var canWeaveAbilities = (
                    CanWeave(DNC.Windmill) ||
                    CanWeave(DNC.Bladeshower) ||
                    CanWeave(DNC.RisingWindmill) ||
                    CanWeave(DNC.Bloodshower) ||
                    CanWeave(DNC.Tillana)
                );

                if (IsEnabled(CustomComboPreset.DancerSimpleOpenerFeature) && level >= 90)
                {
                    if (inCombat && HasEffect(DNC.Buffs.StandardStep) && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                        step = 0;

                        return DNC.StandardStep;
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        // StandardStep (Done first)
                        // Step #1
                        if (step == 0) {
                            if (gauge.CompletedSteps == 1) step++;
                            else return (uint)gauge.NextStep;
                        }
                        // Step #2
                        if (step == 1) {
                            if (gauge.CompletedSteps == 2) step++;
                            else return (uint)gauge.NextStep;
                        }
                        // Peloton
                        if (step == 2) {
                            if (HasEffect(DNC.Buffs.Peloton)) step++;
                            else return DNC.Peloton;
                        }
                        // Standard Finish
                        if (step == 3) {
                            if (!gauge.IsDancing) step++;
                            else return DNC.StandardFinish2;
                        }
                        // Technical Step
                        if (step == 4) {
                            if (HasEffect(DNC.Buffs.TechnicalStep)) step++;
                            else return DNC.TechnicalStep;
                        }
                        // Step #1
                        // Step #2
                        // Step #3
                        // Step #4
                        // Technical Finish
                        // [Weave] Devilment
                        // Starfall Dance
                        // [Weave] Flourish
                        // [Weave] Dan Dance 3
                        // Tillana
                        // [Weave] Fan Dance 4
                        // Saber Dance OR Fountainfall
                        // (feather?) [Weave] Fan Dance 1
                        // (proc?) [Weave] Fan Dance 3
                        // Standard Step
                        // Step #1
                        // Step #2
                        // Standard Finish
                    }
                }
            
                if (level >= DNC.Levels.HeadGraze && IsEnabled(CustomComboPreset.DancerSimpleInterruptFeature))
                {
                    if (CanInterruptEnemy() && IsOffCooldown(DNC.HeadGraze))
                    {
                        return DNC.HeadGraze;
                    }
                }

                if (IsEnabled(CustomComboPreset.DancerSimpleDancesFeature))
                {
                    if (HasEffect(DNC.Buffs.TechnicalStep))
                        return gauge.CompletedSteps < 4
                            ? (uint)gauge.NextStep
                            : DNC.TechnicalFinish4;
                    
                    if (HasEffect(DNC.Buffs.StandardStep))
                            return gauge.CompletedSteps < 2
                                ? (uint)gauge.NextStep
                                : DNC.StandardFinish2;

                    if (level >= DNC.Levels.StandardStep && (!HasTarget() || EnemyHealthPercentage() > 5) && IsOffCooldown(DNC.StandardStep))
                        return DNC.StandardStep;

                    if (level >= DNC.Levels.TechnicalStep && (!HasTarget() || EnemyHealthPercentage() > 5) && IsOffCooldown(DNC.TechnicalStep) && GetCooldown(DNC.StandardStep).CooldownRemaining > 5)
                        return DNC.TechnicalStep;
                }

                if (IsEnabled(CustomComboPreset.DancerSimpleBuffsFeature))
                {
                    if (level >= DNC.Levels.Devilment && IsOffCooldown(DNC.Devilment) && canWeaveAbilities)
                        return DNC.Devilment;

                    if (level >= DNC.Levels.Tillana && HasEffect(DNC.Buffs.FlourishingFinish) && IsOffCooldown(DNC.Tillana))
                        return DNC.Tillana;

                    if (level >= DNC.Levels.Flourish && IsOffCooldown(DNC.Flourish) && canWeaveAbilities)
                        return DNC.Flourish;
                }
                
                if (level >= DNC.Levels.SaberDance && (gauge.Esprit >= 80 || (HasEffect(DNC.Buffs.TechnicalFinish) && gauge.Esprit > 50)) && IsOffCooldown(DNC.SaberDance))
                {
                    return DNC.SaberDance;
                }

                if (level >= DNC.Levels.FanDance && IsEnabled(CustomComboPreset.DancerSimpleFeatherFeature) && canWeaveAbilities)
                {
                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance)) return DNC.FanDance3;

                    var minFeathers = IsEnabled(CustomComboPreset.DancerSimpleFeatherPoolingFeature) && level >= DNC.Levels.TechnicalStep ? 3 : 0;

                    if (gauge.Feathers > minFeathers || (HasEffect(DNC.Buffs.TechnicalFinish) && gauge.Feathers > 0))
                        return DNC.FanDance1;
                }

                if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance)) return DNC.FanDance4;
                if (level >= DNC.Levels.Fountainfall && HasEffect(DNC.Buffs.FlourishingFlow)) return DNC.Fountainfall;
                if (level >= DNC.Levels.ReverseCascade && HasEffect(DNC.Buffs.FlourishingSymmetry)) return DNC.ReverseCascade;
                if (level >= DNC.Levels.Fountain && lastComboMove == DNC.Cascade) return DNC.Fountain;

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
            if (actionID == DNC.Windmill) {
                var gauge = GetJobGauge<DNCGauge>();
                var canWeaveAbilities = (
                    CanWeave(DNC.Windmill) ||
                    CanWeave(DNC.Bladeshower) ||
                    CanWeave(DNC.RisingWindmill) ||
                    CanWeave(DNC.Bloodshower)
                );

                if (HasEffect(DNC.Buffs.StandardStep) && IsEnabled(CustomComboPreset.DancerSimpleAoeStandardFeature))
                    return gauge.CompletedSteps < 2
                        ? (uint)gauge.NextStep
                        : DNC.StandardFinish2;
                
                if (HasEffect(DNC.Buffs.TechnicalStep) && IsEnabled(CustomComboPreset.DancerSimpleAoeTechnicalFeature))
                    return gauge.CompletedSteps < 4
                        ? (uint)gauge.NextStep
                        : DNC.TechnicalFinish4;


                if (level >= DNC.Levels.StandardStep && IsEnabled(CustomComboPreset.DancerSimpleAoeStandardFeature) && (!HasTarget() || EnemyHealthPercentage() > 5) && !HasEffect(DNC.Buffs.TechnicalStep))
                {
                    if (IsOffCooldown(DNC.StandardStep)) return DNC.StandardStep;
                }

                if (level >= DNC.Levels.TechnicalStep && IsEnabled(CustomComboPreset.DancerSimpleAoeTechnicalFeature) && (!HasTarget() || EnemyHealthPercentage() > 5) && !HasEffect(DNC.Buffs.StandardStep))
                {
                    if (IsOffCooldown(DNC.TechnicalStep)) return DNC.TechnicalStep;
                }

                if (IsEnabled(CustomComboPreset.DancerSimpleAoeBuffsFeature))
                {
                    if (level >= DNC.Levels.Devilment && IsOffCooldown(DNC.Devilment) && canWeaveAbilities)
                        return DNC.Devilment;

                    if (level >= DNC.Levels.Tillana && HasEffect(DNC.Buffs.FlourishingFinish) && IsOffCooldown(DNC.Tillana))
                        return DNC.Tillana;

                    if (level >= DNC.Levels.Flourish && IsOffCooldown(DNC.Flourish) && canWeaveAbilities)
                        return DNC.Flourish;
                }
                
                if (level >= DNC.Levels.SaberDance && gauge.Esprit >= 80 && IsOffCooldown(DNC.SaberDance))
                {
                    return DNC.SaberDance;
                }

                if (level >= DNC.Levels.FanDance2 && IsEnabled(CustomComboPreset.DancerSimpleAoeFeatherFeature) && canWeaveAbilities)
                {
                    if (HasEffect(DNC.Buffs.ThreeFoldFanDance)) return DNC.FanDance3;

                    var minFeathers = (
                        IsEnabled(CustomComboPreset.DancerSimpleAoeFeatherPoolingFeature) &&
                        level >= DNC.Levels.TechnicalStep &&
                        EnemyHealthPercentage() > 5
                     ) ? 3 : 0;

                    if (gauge.Feathers > minFeathers) return DNC.FanDance2;
                }

                if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourFoldFanDance)) return DNC.FanDance4;
                if (level >= DNC.Levels.Bloodshower && HasEffect(DNC.Buffs.FlourishingFlow)) return DNC.Bloodshower;
                if (level >= DNC.Levels.RisingWindmill && HasEffect(DNC.Buffs.FlourishingSymmetry)) return DNC.RisingWindmill;
                if (level >= DNC.Levels.Bladeshower && lastComboMove == DNC.Windmill) return DNC.Bladeshower;
            
                return DNC.Windmill;
            }

            return actionID;
        }
    }
}
