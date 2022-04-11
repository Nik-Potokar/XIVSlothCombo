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
            ForbiddenChakra = 3546;


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
                DisciplinedFist = 3001;
        }

        public static class Debuffs
        {
            public const ushort
                Demolish = 246;
        }

        public static class Levels
        {
            public const byte
                Meditation = 15,
                ArmOfTheDestroyer = 26,
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                HowlingFist = 40,
                DragonKick = 50,
                PerfectBalance = 50,
                FormShift = 52,
                MasterfulBlitz = 60,
                Enlightenment = 70,
                ShadowOfTheDestroyer = 82,
                TwinSnakes = 18,
                SnapPunch = 6,
                TrueStrike = 4;
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
    internal class MnkBasicComboPlus : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkBasicComboPlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Bootshine)
            {
                var gauge = GetJobGauge<MNKGauge>();
                var twinsnakeBuff = HasEffect(MNK.Buffs.DisciplinedFist);
                var twinsnakeDuration = FindEffect(MNK.Buffs.DisciplinedFist);
                var demolishDuration = FindTargetEffect(MNK.Debuffs.Demolish);
                var RoFCD = GetCooldown(MNK.RiddleOfFire);
                var RoWCD = GetCooldown(MNK.RiddleOfWind);
                var BrotherhoodCD = GetCooldown(MNK.Brotherhood);
                //Nadi
                var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                var pbCD = GetCooldown(MNK.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var solarNadi = gauge.Nadi == Nadi.SOLAR;
                var nadiNONE = gauge.Nadi == Nadi.NONE;
                var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.MnkMainComboBuffsFeature))
                {

                    if (HasEffect(MNK.Buffs.PerfectBalance) && actionIDCD.CooldownRemaining > 0.5 && !RoFCD.IsCooldown && level >= 68)
                        return MNK.RiddleOfFire;
                    if (HasEffect(MNK.Buffs.PerfectBalance) && actionIDCD.CooldownRemaining > 0.5 && !BrotherhoodCD.IsCooldown && HasEffect(MNK.Buffs.RiddleOfFire) && level >= 70)
                        return MNK.Brotherhood;
                }
                if (IsEnabled(CustomComboPreset.MnkRiddleOfWindFeature))
                {
                    if (lastComboMove == MNK.TwinSnakes && actionIDCD.CooldownRemaining > 0.5 && !RoWCD.IsCooldown && level >= 72)
                        return MNK.RiddleOfWind;
                }

                if (!HasEffect(MNK.Buffs.LeadenFist) && HasEffect(MNK.Buffs.OpoOpoForm) && level >= MNK.Levels.DragonKick)
                    return MNK.DragonKick;
                if (IsEnabled(CustomComboPreset.MonkForbiddenChakraFeature) && gauge.Chakra == 5 && actionIDCD.CooldownRemaining > 0.5 && level >= 15)
                {
                    return OriginalHook(MNK.ForbiddenChakra);
                }
                if (gauge.BlitzTimeRemaining > 0 && level >= 60)
                {
                    return OriginalHook(MNK.MasterfulBlitz);
                }
                if (HasEffect(MNK.Buffs.RaptorForm) && level >= MNK.Levels.TrueStrike)
                {
                    if ((!HasEffect(MNK.Buffs.DisciplinedFist) && level >= MNK.Levels.TwinSnakes) || (twinsnakeDuration.RemainingTime < 5 && level >= MNK.Levels.TwinSnakes))
                        return MNK.TwinSnakes;
                    return MNK.TrueStrike;
                }
                if (HasEffect(MNK.Buffs.CoerlForm) && level >= MNK.Levels.SnapPunch)
                {
                    if ((!TargetHasEffect(MNK.Debuffs.Demolish) && level >= MNK.Levels.Demolish) || (demolishDuration.RemainingTime < 5 && level >= MNK.Levels.Demolish))
                        return MNK.Demolish;
                    return MNK.SnapPunch;
                }
                if (HasEffect(MNK.Buffs.FormlessFist))
                {
                    if (!HasEffect(MNK.Buffs.LeadenFist))
                        return MNK.DragonKick;
                }
                if (HasEffect(MNK.Buffs.PerfectBalance) && IsEnabled(CustomComboPreset.MonkMasterfullBlizOnMainCombo))
                {
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
