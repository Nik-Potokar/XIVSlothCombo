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
            Reprisal = 7535,
            InnerRelease = 7389,
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
            public const ushort
                Reprisal = 1193;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                Berserk = 6,
                Tomahawk = 15,
                StormsPath = 26,
                InnerBeast = 35,
                MythrilTempest = 40,
                SteelCyclone = 45,
                StormsEye = 50,
                Infuriate = 50,
                FellCleave = 54,
                Decimate = 60,
                Onslaught = 62,
                Upheaval = 64,
                ChaoticCyclone = 72,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80,
                Orogeny = 86,
                PrimalRend = 90;
        }

        public static class Config
        {
            public const string
                WarInfuriateRange = "WarInfuriateRange";
            public const string
                WarSurgingRefreshRange = "WarSurgingRefreshRange";
            public const string
                WarKeepOnslaughtCharges = "WarKeepOnslaughtCharges";
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
                var gauge = GetJobGauge<WARGauge>().BeastGauge;
                var surgingThreshold = Service.Configuration.GetCustomIntValue(WAR.Config.WarSurgingRefreshRange);
                var onslaughtChargesRemaining = Service.Configuration.GetCustomIntValue(WAR.Config.WarKeepOnslaughtCharges);

                if (IsEnabled(CustomComboPreset.WARRangedUptimeFeature) && level >= WAR.Levels.Tomahawk)
                {
                    if (!InMeleeRange(true))
                        return WAR.Tomahawk;
                }

                if (IsEnabled(CustomComboPreset.WarriorInfuriateonST) && level >= WAR.Levels.Infuriate && GetRemainingCharges(WAR.Infuriate) >= 1 && !HasEffect(WAR.Buffs.NascentChaos) && gauge <= 50 && CanWeave(actionID))
                    return WAR.Infuriate;

                if (HasEffect(WAR.Buffs.SurgingTempest) && HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                {
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.WarriorIRonST) && CanDelayedWeave(actionID) && IsOffCooldown(OriginalHook(WAR.Berserk)) && level >= WAR.Levels.Berserk)
                            return OriginalHook(WAR.Berserk);
                        if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && IsOffCooldown(WAR.Upheaval) && level >= WAR.Levels.Upheaval)
                            return WAR.Upheaval;
                        if (IsEnabled(CustomComboPreset.WarriorOnslaughtFeature) && level >= WAR.Levels.Onslaught && GetRemainingCharges(WAR.Onslaught) > onslaughtChargesRemaining)
                            return WAR.Onslaught;
                    }

                    if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady))
                        return WAR.PrimalRend;

                    if ((IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.InnerBeast) ||
                        (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && level >= WAR.Levels.InnerChaos) ||
                        (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= WAR.Levels.InnerBeast &&
                        (IsOffCooldown(WAR.InnerRelease) || GetCooldown(WAR.InnerRelease).CooldownRemaining > 35 || HasEffect(WAR.Buffs.NascentChaos))))
                            return OriginalHook(WAR.InnerBeast);
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    {
                        if (gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= WAR.Levels.InnerBeast)
                            return OriginalHook(WAR.InnerBeast);
                        return WAR.Maim;
                    }

                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= WAR.Levels.InnerBeast && HasEffectAny(WAR.Buffs.SurgingTempest) && gauge >= 90)
                            return OriginalHook(WAR.InnerBeast);
                        if ((GetBuffRemainingTime(WAR.Buffs.SurgingTempest) <= surgingThreshold) && level >= WAR.Levels.StormsEye)
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

                    if (HasEffect(WAR.Buffs.SurgingTempest))
                    {
                        if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && IsOffCooldown(WAR.Orogeny) && CanWeave(actionID) && level >= WAR.Levels.Orogeny && HasEffect(WAR.Buffs.SurgingTempest))
                            return WAR.Orogeny;
                        if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(WAR.Buffs.PrimalRendReady) && level >= WAR.Levels.PrimalRend)
                            return OriginalHook(WAR.PrimalRend);
                    }

                    if ((IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.SteelCyclone && HasEffect(WAR.Buffs.SurgingTempest)) ||
                        (IsEnabled(CustomComboPreset.WarriorSpenderOption) && gauge >= 50 && level >= WAR.Levels.SteelCyclone) ||
                        (IsEnabled(CustomComboPreset.WarriorInnerChaosOption) && HasEffect(WAR.Buffs.NascentChaos) && level >= WAR.Levels.ChaoticCyclone))
                            return OriginalHook(WAR.SteelCyclone);

                    if (comboTime > 0)
                    {
                        if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                        {
                            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.SteelCyclone)
                                return OriginalHook(WAR.SteelCyclone);
                            return WAR.MythrilTempest;
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
                if (HasEffect(WAR.Buffs.InnerRelease) || HasEffect(WAR.Buffs.NascentChaos))
                    return OriginalHook(WAR.FellCleave);
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

    internal class WarriorInfuriateFellCleave : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorInfuriateFellCleave;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is WAR.InnerBeast or WAR.FellCleave or WAR.SteelCyclone or WAR.Decimate)
            {
                var rageGauge = GetJobGauge<WARGauge>();
                var rageThreshold = Service.Configuration.GetCustomIntValue(WAR.Config.WarInfuriateRange);
                var isZerking = HasEffect(WAR.Buffs.InnerRelease);
                var hasNascent = HasEffect(WAR.Buffs.NascentChaos);

                if (!InCombat()) return actionID;

                if (rageGauge.BeastGauge <= rageThreshold && GetCooldown(WAR.Infuriate).RemainingCharges > 0 && !isZerking && !hasNascent && level >= WAR.Levels.Infuriate)
                    return OriginalHook(WAR.Infuriate);
            }

            return actionID;
        }
    }
    internal class WarriorPrimalRendOnInnerRelease : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalRendOnInnerRelease;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is WAR.Berserk or WAR.InnerRelease)
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            return actionID;
        }
    }
    internal class WarriorReprisalProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorReprisalProtection;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is WAR.Reprisal)
            {
                if (TargetHasEffectAny(WAR.Debuffs.Reprisal) && IsOffCooldown(WAR.Reprisal))
                    return WHM.Stone1;
            }
            return actionID;
        }
    }
}
