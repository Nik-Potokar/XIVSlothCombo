using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class WAR
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        public const uint
            HeavySwing = 31,
            Maim = 37,
            Berserk = 38,
            Overpower = 41,
            StormsPath = 42,
            StormsEye = 45,
            Tomahawk = 46,
            InnerBeast = 49,
            SteelCyclone = 51,
            Infuriate = 52,
            FellCleave = 3549,
            Decimate = 3550,
            Upheaval = 7387,
            LowBlow = 7540,
            Interject = 7538,
            InnerRelease = 8768,
            RawIntuition = 3551,
            MythrilTempest = 16462,
            ChaoticCyclone = 16463,
            NascentFlash = 16464,
            InnerChaos = 16465,
            Orogeny = 25752,
            PrimalRend = 25753,
            Onslaught = 7386;

        public static class Buffs
        {
            public const ushort
                InnerRelease = 1177,
                SurgingTempest = 2677,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                Berserk = 86;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                Tomahawk = 15,
                StormsPath = 26,
                InnerBeast = 35,
                MythrilTempest = 40,
                StormsEye = 50,
                FellCleave = 54,
                Decimate = 60,
                Onslaught = 62,
                Upheaval = 64,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80,
                PrimalRend = 90;
        }
    }

    // Replace Storm's Path with Storm's Path combo and overcap feature on main combo to fellcleave
    internal class WarriorStormsPathCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.WarriorStormsPathCombo) && actionID == WAR.StormsPath)
            {
                var stormseyeBuff = FindEffectAny(WAR.Buffs.SurgingTempest);
                var gauge = GetJobGauge<WARGauge>().BeastGauge;

                if (IsEnabled(CustomComboPreset.WARRangedUptimeFeature) && level >= WAR.Levels.Tomahawk)
                {
                    if (!InMeleeRange(true))
                        return WAR.Tomahawk;
                }

                if (HasEffect(WAR.Buffs.SurgingTempest))
                {
                    if (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && gauge >= 50 && level >= WAR.Levels.InnerChaos)
                        return WAR.InnerChaos;
                    if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady))
                        return WAR.PrimalRend;
                    if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                        return WAR.FellCleave;
                    if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= WAR.Levels.InnerBeast)
                        return OriginalHook(WAR.InnerBeast);
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && IsOffCooldown(WAR.Upheaval) && level >= WAR.Levels.Upheaval)
                            return WAR.Upheaval;

                        if (level >= WAR.Levels.Onslaught &&
                            ((IsEnabled(CustomComboPreset.WarriorOnslaughtFeature) && GetRemainingCharges(WAR.Onslaught) is 1 or 2 or 3) || //uses all stacks
                            (IsEnabled(CustomComboPreset.WarriorOnslaughtFeatureOptionTwo) && GetRemainingCharges(WAR.Onslaught) == 3 && level >= 88) || // leaves 2 stacks
                            (IsEnabled(CustomComboPreset.WarriorOnslaughtFeatureOption) && ((GetRemainingCharges(WAR.Onslaught) is 2 or 3 && level >= 88) || (GetRemainingCharges(WAR.Onslaught) == 2 && level < 88))))) // leaves 1 stack
                                return WAR.Onslaught;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    {
                        if (HasEffectAny(WAR.Buffs.SurgingTempest) && gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= WAR.Levels.InnerBeast)
                            return OriginalHook(WAR.InnerBeast);
                        return WAR.Maim;
                    }

                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        if (HasEffectAny(WAR.Buffs.SurgingTempest) && gauge >= 90 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= WAR.Levels.InnerBeast)
                            return OriginalHook(WAR.InnerBeast);
                        if ((!HasEffectAny(WAR.Buffs.SurgingTempest) || stormseyeBuff.RemainingTime < 15) && level >= WAR.Levels.StormsEye)
                            return WAR.StormsEye;
                        return WAR.StormsPath;
                    }
                }

                return WAR.HeavySwing;
            }

            return actionID;
        }

        internal class WarriorStormsEyeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.StormsEye)
                {
                    if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                        return OriginalHook(WAR.FellCleave);

                    if (comboTime > 0)
                    {
                        if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                            return WAR.Maim;

                        if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                            return WAR.StormsEye;
                    }

                    return WAR.HeavySwing;
                }

                return actionID;
            }
        }

        internal class WarriorMythrilTempestCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var orogenyCD = GetCooldown(WAR.Orogeny);
                    var mythrilCd = GetCooldown(WAR.MythrilTempest);
                    var decimateCD = GetCooldown(WAR.Decimate);


                    if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && !orogenyCD.IsCooldown && decimateCD.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && lastComboMove == WAR.Decimate && level >= 86)
                        return WAR.Orogeny;
                    if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && !orogenyCD.IsCooldown && mythrilCd.CooldownRemaining > 0.7 && HasEffect(WAR.Buffs.SurgingTempest) && level >= 86)
                        return WAR.Orogeny;


                    if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady) && level >= 90)
                        return OriginalHook(WAR.PrimalRend);
                    if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                        return OriginalHook(WAR.Decimate);
                    if (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && HasEffect(WAR.Buffs.SurgingTempest) && gauge >= 50 && level >= 72)
                        return OriginalHook(WAR.ChaoticCyclone);

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature))
                        {
                            if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                                return WAR.PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WarriorMythrilTempestCombo))
                        {
                            if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 60)
                                return WAR.Decimate;
                            if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= 45 && level <= 59)
                                return WAR.SteelCyclone;
                            if (lastComboMove == WAR.Infuriate && level >= 60)
                                return WAR.Decimate;
                            if (lastComboMove == WAR.Overpower && level >= 40)
                                return WAR.MythrilTempest;
                            if ((lastComboMove == WAR.Overpower && level >= 60) || (lastComboMove == WAR.MythrilTempest && gauge >= 90 && level >= 60 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature)))
                                return WAR.Decimate;
                        }

                    }

                    return WAR.Overpower;
                }

                return actionID;
            }
        }

        internal class WarriorNascentFlashFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WAR.NascentFlash)
                {
                    if (level >= WAR.Levels.NascentFlash)
                        return WAR.NascentFlash;
                    return WAR.RawIntuition;
                }

                return actionID;
            }
        }
    }

    internal class WarriorPrimalRendFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalRendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.InnerBeast || actionID == WAR.SteelCyclone)
            {

                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;

                // Fell Cleave or Decimate
                return OriginalHook(actionID);


            }

            return actionID;
        }
    }
    internal class WarriorInfuriateFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorInfuriateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.Infuriate)
            {
                if (HasEffect(WAR.Buffs.InnerRelease))
                    return WAR.FellCleave;
                if (HasEffect(WAR.Buffs.NascentChaos))
                    return WAR.InnerChaos;
            }

            return actionID;
        }
    }
    internal class WarriorInterruptFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.LowBlow)
            {
                var interjectCD = GetCooldown(WAR.Interject);
                var lowBlowCD = GetCooldown(WAR.LowBlow);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return WAR.Interject;
            }

            return actionID;
        }
    }
}
