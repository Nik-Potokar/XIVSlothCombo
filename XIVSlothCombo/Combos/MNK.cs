using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MNK
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            Bootshine = 53,
            DragonKick = 74,
            SnapPunch = 56,
            TwinSnakes = 61,
            Demolish = 66,
            ArmOfTheDestroyer = 62,
            Rockbreaker = 70,
            FourPointFury = 16473,
            PerfectBalance = 69,
            TrueStrike = 54,
            LegSweep = 7863,
            Meditation = 3546,
            HowlingFist = 25763,
            Enlightenment = 16474,
            MasterfulBlitz = 25764,
            ElixirField = 3545,
            FlintStrike = 25882,
            RisingPhoenix = 25768,
            ShadowOfTheDestroyer = 25767,
            RiddleOfFire = 7395,
            RiddleOfWind = 25766,
            Brotherhood = 7396,
            ForbiddenChakra = 3546,
            FormShift = 4262,
            Thunderclap = 25762;


        public static class Buffs
        {
            public const ushort
                TwinSnakes = 101,
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110,
                RiddleOfFire = 1181,
                LeadenFist = 1861,
                FormlessFist = 2513,
                DisciplinedFist = 3001,
                Brotherhood = 1185;
        }

        public static class Debuffs
        {
            public const ushort
                Demolish = 246;
        }

        public static class Levels
        {
            public const byte
                TrueStrike = 4,
                SnapPunch = 6,
                Meditation = 15,
                TwinSnakes = 18,
                ArmOfTheDestroyer = 26,
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                HowlingFist = 40,
                DragonKick = 50,
                PerfectBalance = 50,
                FormShift = 52,
                MasterfulBlitz = 60,
                RiddleOfFire = 68,
                Enlightenment = 70,
                Brotherhood = 70,
                RiddleOfWind = 72,
                ShadowOfTheDestroyer = 82;
        }
    }

    internal class MnkAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.ArmOfTheDestroyer || actionID == MNK.ShadowOfTheDestroyer)
            {
                var gauge = GetJobGauge<MNKGauge>();
                var actionIDCD = GetCooldown(OriginalHook(actionID));

                if (IsEnabled(CustomComboPreset.MonkForbiddenChakraFeature) && gauge.Chakra == 5 && actionIDCD.CooldownRemaining > 0.5 && HasBattleTarget(true) && level >= 40)
                {
                    return OriginalHook(MNK.Enlightenment);
                }
                if (gauge.BlitzTimeRemaining > 0 && level >= 60)
                    return OriginalHook(MNK.MasterfulBlitz);

                if (HasEffect(MNK.Buffs.OpoOpoForm))
                    return OriginalHook(MNK.ArmOfTheDestroyer);

                if (HasEffect(MNK.Buffs.RaptorForm) && level >= MNK.Levels.FourPointFury)
                    return MNK.FourPointFury;

                if (HasEffect(MNK.Buffs.CoerlForm) && level >= MNK.Levels.Rockbreaker)
                    return MNK.Rockbreaker;

                if (HasEffect(MNK.Buffs.PerfectBalance) && IsEnabled(CustomComboPreset.MonkMasterfullBlizOnAoECombo))
                {
                    var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                    var pbCD = GetCooldown(MNK.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var solarNadi = gauge.Nadi == Nadi.SOLAR;
                    var nadiNONE = gauge.Nadi == Nadi.NONE;
                    // low level
                    if (!nadiNONE && !lunarNadi && HasEffect(MNK.Buffs.PerfectBalance) && level <= 81)
                    {
                        if (pbStacks.StackCount == 3 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;
                        if (pbStacks.StackCount == 2 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;
                        if (pbStacks.StackCount == 1 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;

                    }
                    if (nadiNONE && HasEffect(MNK.Buffs.PerfectBalance) && level <= 81)
                    {
                        if (pbStacks.StackCount == 3 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;
                        if (pbStacks.StackCount == 2 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;
                        if (pbStacks.StackCount == 1 && HasEffect(MNK.Buffs.PerfectBalance))
                            return MNK.Rockbreaker;
                    }
                    if (lunarNadi && HasEffect(MNK.Buffs.PerfectBalance) && level >= 60)
                    {
                        if (pbStacks.StackCount == 3 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                        if (pbStacks.StackCount == 2 && HasEffect(MNK.Buffs.PerfectBalance) && lastComboMove == OriginalHook(MNK.ArmOfTheDestroyer))
                            return MNK.FourPointFury;
                        if (pbStacks.StackCount == 1 && HasEffect(MNK.Buffs.PerfectBalance) && lastComboMove == MNK.FourPointFury)
                            return MNK.Rockbreaker;
                    }
                    // highlevel
                    if (!nadiNONE && !lunarNadi && HasEffect(MNK.Buffs.PerfectBalance) && level >= 82)
                    {
                        if (pbStacks.StackCount == 3 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                        if (pbStacks.StackCount == 2 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                        if (pbStacks.StackCount == 1 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                    }
                    if (nadiNONE && HasEffect(MNK.Buffs.PerfectBalance) && level >= 82)
                    {
                        if (pbStacks.StackCount == 3 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                        if (pbStacks.StackCount == 2 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                        if (pbStacks.StackCount == 1 && HasEffect(MNK.Buffs.PerfectBalance))
                            return OriginalHook(MNK.ArmOfTheDestroyer);
                    }

                }

            }
            return actionID;
        }
    }

    internal class MnkBootshineFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkBootshineFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.DragonKick)
            {
                if (IsEnabled(CustomComboPreset.MnkBootshineBalanceFeature) && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz)
                    return OriginalHook(MNK.MasterfulBlitz);

                if (HasEffect(MNK.Buffs.LeadenFist) && (
                    HasEffect(MNK.Buffs.FormlessFist) || HasEffect(MNK.Buffs.PerfectBalance) ||
                    HasEffect(MNK.Buffs.OpoOpoForm) || HasEffect(MNK.Buffs.RaptorForm) || HasEffect(MNK.Buffs.CoerlForm)))
                    return MNK.Bootshine;

                if (level < MNK.Levels.DragonKick)
                    return MNK.Bootshine;
            }

            return actionID;
        }
    }

    internal class MnkTwinSnakesFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkTwinSnakesFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.TrueStrike)
            {
                var disciplinedFistBuff = HasEffect(MNK.Buffs.DisciplinedFist);
                var disciplinedFistDuration = FindEffect(MNK.Buffs.DisciplinedFist);

                if (level >= MNK.Levels.TrueStrike)
                {
                    if ((!disciplinedFistBuff && level >= MNK.Levels.TwinSnakes) || (disciplinedFistDuration.RemainingTime < 6 && level >= MNK.Levels.TwinSnakes))
                        return MNK.TwinSnakes;
                    return MNK.TrueStrike;
                }

            }

            return actionID;
        }
    }

    internal class MnkBasicCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkBasicCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Bootshine)
            {
                var gauge = GetJobGauge<MNKGauge>();
                if (HasEffect(MNK.Buffs.RaptorForm) && level >= MNK.Levels.TrueStrike)
                {
                    if (!HasEffect(MNK.Buffs.DisciplinedFist) && level >= MNK.Levels.TwinSnakes)
                        return MNK.TwinSnakes;
                    return MNK.TrueStrike;
                }

                if (HasEffect(MNK.Buffs.CoerlForm) && level >= MNK.Levels.SnapPunch)
                {
                    if (!TargetHasEffect(MNK.Debuffs.Demolish) && level >= MNK.Levels.Demolish)
                        return MNK.Demolish;
                    return MNK.SnapPunch;
                }

                if (!HasEffect(MNK.Buffs.LeadenFist) && HasEffect(MNK.Buffs.OpoOpoForm) && level >= MNK.Levels.DragonKick)
                    return MNK.DragonKick;
                return MNK.Bootshine;
            }

            return actionID;
        }
    }

    internal class MonkPerfectBalanceFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkPerfectBalanceFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.PerfectBalance)
            {
                if (OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz && level >= 60)
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            return actionID;
        }
    }
    internal class MnkBootshineCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkBootshineCombo;

        internal static bool inOpener = false;
        internal static bool openerFinished = false;
        internal static bool loopTwinSnakes = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Bootshine)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<MNKGauge>();
                var canWeave = CanWeave(actionID, 0.4);
                var canWeaveChakra = CanWeave(actionID);

                var twinsnakeDuration = FindEffect(MNK.Buffs.DisciplinedFist);
                var demolishDuration = FindTargetEffect(MNK.Debuffs.Demolish);

                var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var solarNadi = gauge.Nadi == Nadi.SOLAR;
                var nadiNONE = gauge.Nadi == Nadi.NONE;

                // Opener for MNK
                if (IsEnabled(CustomComboPreset.MnkLunarSolarOpenerOnMainComboFeature))
                {
                    // Re-enter opener when Brotherhood is used
                    if (lastComboMove == MNK.Brotherhood)
                    {
                        inOpener = true;
                        openerFinished = false;
                        loopTwinSnakes = false;
                    }

                    if (!inCombat)
                    {
                        if (!inOpener && gauge.Chakra < 5)
                        {
                            return MNK.Meditation;
                        }
                        if (!inCombat && !inOpener && level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && comboTime <= 0)
                        {
                            return MNK.FormShift;
                        }
                        if (!InMeleeRange(true) && gauge.Chakra == 5 && HasEffect(MNK.Buffs.FormlessFist))
                        {
                            return MNK.Thunderclap;
                        }

                        if (inOpener || openerFinished)
                        {
                            inOpener = false;
                            openerFinished = false;
                            loopTwinSnakes = false;
                        }
                    }
                    else
                    {
                        if (!inOpener && !openerFinished)
                        {
                            inOpener = true;
                        }
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        if (level >= MNK.Levels.RiddleOfFire)
                        {
                            // Early exit out of opener
                            if (IsOnCooldown(MNK.RiddleOfFire) && GetCooldownRemainingTime(MNK.RiddleOfFire) <= 30)
                            {
                                inOpener = false;
                                openerFinished = true;
                                loopTwinSnakes = false;
                            }

                            if (canWeave)
                            {
                                // Delayed weave for Riddle of Fire specifically
                                if (HasEffect(MNK.Buffs.CoerlForm) && !IsOnCooldown(MNK.RiddleOfFire) &&
                                    GetCooldown(actionID).CooldownRemaining < 0.9)
                                {
                                    return MNK.RiddleOfFire;
                                }
                                if (IsOnCooldown(MNK.RiddleOfFire) && GetCooldownRemainingTime(MNK.RiddleOfFire) <= 59)
                                {
                                    if (HasEffect(MNK.Buffs.RaptorForm))
                                    {
                                        if (level >= MNK.Levels.Brotherhood && !IsOnCooldown(MNK.Brotherhood))
                                        {
                                            return MNK.Brotherhood;
                                        }
                                        if (GetRemainingCharges(MNK.PerfectBalance) > 0 && !HasEffect(MNK.Buffs.PerfectBalance) &&
                                            (lastComboMove == MNK.Bootshine || lastComboMove == MNK.DragonKick) && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz)
                                        {
                                            return MNK.PerfectBalance;
                                        }
                                    }
                                    if (level >= MNK.Levels.RiddleOfWind && HasEffect(MNK.Buffs.PerfectBalance) && !IsOnCooldown(MNK.RiddleOfWind))
                                    {
                                        return MNK.RiddleOfWind;
                                    }
                                    if (level >= MNK.Levels.Meditation && gauge.Chakra == 5 && canWeaveChakra)
                                    {
                                        return OriginalHook(MNK.Meditation);
                                    }
                                }
                            }

                            // Check if opener is over
                            if (loopTwinSnakes && lastComboMove == MNK.TwinSnakes)
                            {
                                inOpener = false;
                                openerFinished = true;
                                loopTwinSnakes = false;
                            }

                            // End the opener with Twin Snakes when we have Formless Fist and Riddle of Fire is almost over
                            if ((IsOnCooldown(MNK.RiddleOfFire) && GetCooldownRemainingTime(MNK.RiddleOfFire) <= 45) && HasEffect(MNK.Buffs.FormlessFist))
                            {
                                loopTwinSnakes = true;
                                return MNK.TwinSnakes;
                            }
                        }
                        else
                        {
                            // Automatically exit opener if we don't have Riddle of Fire
                            inOpener = false;
                            openerFinished = true;
                        }
                    }
                }

                // Buffs
                if (inCombat && !inOpener && canWeave && IsEnabled(CustomComboPreset.MnkCDsOnMainComboFeature))
                {
                    if (level >= MNK.Levels.RiddleOfFire && !IsOnCooldown(MNK.RiddleOfFire))
                    {
                        return MNK.RiddleOfFire;
                    }
                    if (IsEnabled(CustomComboPreset.MnkPerfectBalanceOnMainComboFeature) &&
                        level >= MNK.Levels.PerfectBalance && !HasEffect(MNK.Buffs.PerfectBalance) && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz)
                    {
                        // Use Perfect Balance if:
                        // 1. It's after Bootshine/Dragon Kick.
                        // 2. At max stacks / before overcap.
                        // 3. During Brotherhood.
                        // 4. During Riddle of Fire after Demolish has been applied.
                        // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                        if ((lastComboMove == MNK.Bootshine || lastComboMove == MNK.DragonKick) &&
                            ((GetRemainingCharges(MNK.PerfectBalance) == 2) ||
                            (GetRemainingCharges(MNK.PerfectBalance) == 1 && GetCooldownChargeRemainingTime(MNK.PerfectBalance) < 4) ||
                            (GetRemainingCharges(MNK.PerfectBalance) >= 1 && HasEffect(MNK.Buffs.Brotherhood)) ||
                            (GetRemainingCharges(MNK.PerfectBalance) >= 1 && FindEffect(MNK.Buffs.RiddleOfFire).RemainingTime < 10 && demolishDuration.RemainingTime > 10) ||
                            (GetRemainingCharges(MNK.PerfectBalance) >= 1 && GetCooldownRemainingTime(MNK.RiddleOfFire) < 4 && GetCooldownRemainingTime(MNK.Brotherhood) < 8)))
                        {
                            return MNK.PerfectBalance;
                        }
                    }
                    if (IsEnabled(CustomComboPreset.MnkBrotherhoodOnMainComboFeature) && level >= MNK.Levels.Brotherhood && !IsOnCooldown(MNK.Brotherhood))
                    {
                        return MNK.Brotherhood;
                    }
                    if (IsEnabled(CustomComboPreset.MnkRiddleOfWindOnMainComboFeature) && level >= MNK.Levels.RiddleOfWind && !IsOnCooldown(MNK.RiddleOfWind))
                    {
                        return MNK.RiddleOfWind;
                    }
                    if (IsEnabled(CustomComboPreset.MnkMeditationOnMainComboFeature) && level >= MNK.Levels.Meditation && gauge.Chakra == 5 && HasEffect(MNK.Buffs.DisciplinedFist) && canWeaveChakra)
                    {
                        return OriginalHook(MNK.Meditation);
                    }
                }

                // Masterful Blitz
                if (IsEnabled(CustomComboPreset.MonkMasterfulBlitzOnMainCombo) &&
                    level >= MNK.Levels.MasterfulBlitz && !HasEffect(MNK.Buffs.PerfectBalance) && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz)
                {
                    return OriginalHook(MNK.MasterfulBlitz);
                }

                // Perfect Balance
                if (HasEffect(MNK.Buffs.PerfectBalance))
                {
                    if (!inOpener && HasEffect(MNK.Buffs.RiddleOfFire) &&
                        IsOnCooldown(MNK.Brotherhood) && GetCooldownRemainingTime(MNK.Brotherhood) < 8)
                    {
                        switch (pbStacks.StackCount)
                        {
                            case 3:
                                if (!HasEffect(MNK.Buffs.LeadenFist))
                                {
                                    return MNK.DragonKick;
                                }
                                return MNK.Bootshine;
                            case 2:
                                return MNK.Demolish;
                            case 1:
                                return MNK.TwinSnakes;
                        }
                    }
                    if (!nadiNONE && !lunarNadi)
                    {
                        switch (pbStacks.StackCount)
                        {
                            case 3:
                                return MNK.DragonKick;
                            case 2:
                                return MNK.Bootshine;
                            case 1:
                                return MNK.DragonKick;
                        }
                    }
                    if (nadiNONE)
                    {
                        switch (pbStacks.StackCount)
                        {
                            case 3:
                                return MNK.DragonKick;
                            case 2:
                                return MNK.Bootshine;
                            case 1:
                                return MNK.DragonKick;
                        }
                    }
                    if (lunarNadi)
                    {
                        switch (pbStacks.StackCount)
                        {
                            case 3:
                                return MNK.TwinSnakes;
                            case 2:
                                return MNK.DragonKick;
                            case 1:
                                return MNK.Demolish;
                        }
                    }
                }

                // Monk Rotation
                if (level >= MNK.Levels.TrueStrike && HasEffect(MNK.Buffs.RaptorForm))
                {
                    if (level >= MNK.Levels.TwinSnakes && (!HasEffect(MNK.Buffs.DisciplinedFist) || twinsnakeDuration.RemainingTime < 5))
                    {
                        return MNK.TwinSnakes;
                    }
                    return MNK.TrueStrike;
                }
                if (level >= MNK.Levels.SnapPunch && HasEffect(MNK.Buffs.CoerlForm))
                {
                    if (level >= MNK.Levels.Demolish && (!TargetHasEffect(MNK.Debuffs.Demolish) || demolishDuration.RemainingTime < 5))
                    {
                        return MNK.Demolish;
                    }
                    return MNK.SnapPunch;
                }
                if (level >= MNK.Levels.DragonKick && (HasEffect(MNK.Buffs.OpoOpoForm) || HasEffect(MNK.Buffs.FormlessFist)) && !HasEffect(MNK.Buffs.LeadenFist))
                {
                    return MNK.DragonKick;
                }
            }
            return actionID;
        }
    }
    internal class MnkPerfectBalancePlus : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkPerfectBalancePlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.MasterfulBlitz)
            {
                var gauge = GetJobGauge<MNKGauge>();
                var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var nadiNONE = gauge.Nadi == Nadi.NONE;
                if (!nadiNONE && !lunarNadi)
                {
                    if (pbStacks.StackCount == 3)
                        return MNK.DragonKick;
                    if (pbStacks.StackCount == 2)
                        return MNK.Bootshine;
                    if (pbStacks.StackCount == 1)
                        return MNK.DragonKick;
                }
                if (nadiNONE)
                {
                    if (pbStacks.StackCount == 3)
                        return MNK.DragonKick;
                    if (pbStacks.StackCount == 2)
                        return MNK.Bootshine;
                    if (pbStacks.StackCount == 1)
                        return MNK.DragonKick;
                }
                if (lunarNadi)
                {
                    if (pbStacks.StackCount == 3)
                        return MNK.TwinSnakes;
                    if (pbStacks.StackCount == 2)
                        return MNK.DragonKick;
                    if (pbStacks.StackCount == 1)
                        return MNK.Demolish;
                }

            }
            return actionID;
        }
    }
    internal class MnkRiddleOfFireBrotherhoodFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkRiddleOfFireBrotherhoodFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var RoFCD = GetCooldown(MNK.RiddleOfFire);
            var BrotherhoodCD = GetCooldown(MNK.Brotherhood);

            if (actionID == MNK.RiddleOfFire)
            {
                if (level >= 68 && level < 70)
                    return MNK.RiddleOfFire;
                if (RoFCD.IsCooldown && BrotherhoodCD.IsCooldown && level >= 70)
                    return MNK.RiddleOfFire;
                if (RoFCD.IsCooldown && !BrotherhoodCD.IsCooldown && level >= 70)
                    return MNK.Brotherhood;

                return MNK.RiddleOfFire;
            }
            return actionID;
        }
    }
    internal class MonkHowlingFistMeditationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkHowlingFistMeditationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.HowlingFist || actionID == MNK.Enlightenment)
            {
                var gauge = GetJobGauge<MNKGauge>();

                if (gauge.Chakra < 5)
                {
                    return MNK.Meditation;
                }
            }
            return actionID;
        }
    }
}
