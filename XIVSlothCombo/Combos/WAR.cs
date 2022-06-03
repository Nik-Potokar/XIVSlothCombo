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
                Placeholder = 1;
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
                WarInfuriateRange = "WarInfuriateRange",
                WarSurgingRefreshRange = "WarSurgingRefreshRange",
                WarKeepOnslaughtCharges = "WarKeepOnslaughtCharges";
        }


        // Replace Storm's Path with Storm's Path combo and overcap feature on main combo to fellcleave
        internal class WarriorStormsPathCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.WarriorStormsPathCombo) && actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var surgingThreshold = Service.Configuration.GetCustomIntValue(Config.WarSurgingRefreshRange);
                    var onslaughtChargesRemaining = Service.Configuration.GetCustomIntValue(Config.WarKeepOnslaughtCharges);

                    if (IsEnabled(CustomComboPreset.WARRangedUptimeFeature) && level >= Levels.Tomahawk && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;
                    if (IsEnabled(CustomComboPreset.WarriorInfuriateonST) && level >= Levels.Infuriate && GetRemainingCharges(Infuriate) >= 1 && !HasEffect(Buffs.NascentChaos) && gauge <= 40 && CanWeave(actionID))
                        return Infuriate;

                    //Sub Storm's Eye level check
                    if (IsEnabled(CustomComboPreset.WarriorIRonST) && CanDelayedWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && level is >= Levels.Berserk and < Levels.StormsEye && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.WarriorIRonST) && CanDelayedWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && level >= Levels.Berserk)
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WarriorUpheavalMainComboFeature) && IsOffCooldown(Upheaval) && level >= Levels.Upheaval)
                                return Upheaval;
                            if (IsEnabled(CustomComboPreset.WarriorOnslaughtFeature) && level >= Levels.Onslaught && GetRemainingCharges(Onslaught) > onslaughtChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.WarriorMeleeOnslaughtOption) ||
                                    (IsEnabled(CustomComboPreset.WarriorMeleeOnslaughtOption) && GetTargetDistance() <= 1 && GetCooldownRemainingTime(InnerRelease) > 40))
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(Buffs.PrimalRendReady) && level >= Levels.PrimalRend)
                        {
                            if (IsEnabled(CustomComboPreset.WarriorPrimalRendCloseRangeFeature) && (GetTargetDistance() <= 1 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10))
                                return PrimalRend;
                            if (IsNotEnabled(CustomComboPreset.WarriorPrimalRendCloseRangeFeature))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && level >= Levels.InnerBeast)
                        {
                            if (gauge >= 50 && (IsOffCooldown(InnerRelease) || GetCooldownRemainingTime(InnerRelease) > 35 || HasEffect(Buffs.NascentChaos)))
                                return OriginalHook(InnerBeast);
                            if (HasEffect(Buffs.InnerRelease))
                                return OriginalHook(InnerBeast);
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == HeavySwing && level >= Levels.Maim)
                        {
                            if (gauge == 100 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= Levels.InnerBeast)
                                return OriginalHook(InnerBeast);
                            return Maim;
                        }

                        if (lastComboMove == Maim && level >= Levels.StormsPath)
                        {
                            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && level >= Levels.InnerBeast && (level < Levels.StormsEye || HasEffectAny(Buffs.SurgingTempest)) && gauge >= 90)
                                return OriginalHook(InnerBeast);
                            if ((GetBuffRemainingTime(Buffs.SurgingTempest) <= surgingThreshold) && level >= Levels.StormsEye)
                                return StormsEye;
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
                }

                return actionID;
            }

            internal class WarriorStormsEyeCombo : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == StormsEye)
                    {
                        if (comboTime > 0)
                        {
                            if (lastComboMove == HeavySwing && level >= Levels.Maim)
                                return Maim;

                            if (lastComboMove == Maim && level >= Levels.StormsEye)
                                return StormsEye;
                        }

                        return HeavySwing;
                    }

                    return actionID;
                }
            }

            internal class WarriorMythrilTempestCombo : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == Overpower)
                    {
                        var gauge = GetJobGauge<WARGauge>().BeastGauge;

                        if (IsEnabled(CustomComboPreset.WarriorInfuriateOnAOE) && level >= Levels.Infuriate && GetRemainingCharges(Infuriate) >= 1 && !HasEffect(Buffs.NascentChaos) && gauge <= 50 && CanWeave(actionID))
                            return Infuriate;

                        //Sub Mythril Tempest level check
                        if (IsEnabled(CustomComboPreset.WarriorIRonAOE) && CanDelayedWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && level is >= Levels.Berserk and < Levels.MythrilTempest && InCombat())
                            return OriginalHook(Berserk);

                        if (HasEffect(Buffs.SurgingTempest) && InCombat())
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.WarriorIRonAOE) && CanDelayedWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && level >= Levels.Berserk)
                                    return OriginalHook(Berserk);
                                if (IsEnabled(CustomComboPreset.WarriorOrogenyFeature) && IsOffCooldown(Orogeny) && level >= Levels.Orogeny && HasEffect(Buffs.SurgingTempest))
                                    return Orogeny;
                            }

                            if (IsEnabled(CustomComboPreset.WarriorPrimalRendFeature) && HasEffect(Buffs.PrimalRendReady) && level >= Levels.PrimalRend)
                            {
                                if (IsEnabled(CustomComboPreset.WarriorPrimalRendCloseRangeFeature) && (GetTargetDistance() <= 3 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10))
                                    return PrimalRend;
                                if (IsNotEnabled(CustomComboPreset.WarriorPrimalRendCloseRangeFeature))
                                    return PrimalRend;
                            }

                            if (IsEnabled(CustomComboPreset.WarriorSpenderOption) && level >= Levels.SteelCyclone && (gauge >= 50 || HasEffect(Buffs.InnerRelease)))
                                return OriginalHook(SteelCyclone);
                        }

                        if (comboTime > 0)
                        {
                            if (lastComboMove == Overpower && level >= Levels.MythrilTempest)
                            {
                                if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= Levels.SteelCyclone)
                                    return OriginalHook(SteelCyclone);
                                return MythrilTempest;
                            }
                        }

                        return Overpower;
                    }

                    return actionID;
                }
            }

            internal class WarriorNascentFlashFeature : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashFeature;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == NascentFlash)
                    {
                        if (level >= Levels.NascentFlash)
                            return NascentFlash;
                        return RawIntuition;
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
                if (actionID == InnerBeast || actionID == SteelCyclone)
                {

                    if (level >= Levels.PrimalRend && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;

                    // Fell Cleave or Decimate
                    return OriginalHook(actionID);


                }

                return actionID;
            }
        }

        internal class WarriorInfuriateFellCleave : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorInfuriateFellCleave;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is InnerBeast or FellCleave or SteelCyclone or Decimate)
                {
                    var rageGauge = GetJobGauge<WARGauge>();
                    var rageThreshold = Service.Configuration.GetCustomIntValue(Config.WarInfuriateRange);
                    var hasNascent = HasEffect(Buffs.NascentChaos);
                    var hasInnerRelease = HasEffect(Buffs.InnerRelease);

                    if (InCombat() && rageGauge.BeastGauge <= rageThreshold && GetCooldown(Infuriate).RemainingCharges > 0 && !hasNascent && level >= Levels.Infuriate
                    && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WarriorUseInnerReleaseFirst)))
                        return OriginalHook(Infuriate);
                }

                return actionID;
            }
        }
        internal class WarriorPrimalRendOnInnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalRendOnInnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Berserk or InnerRelease)
                {
                    if (level >= Levels.PrimalRend && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                }

                return actionID;
            }
        }
    }
}
