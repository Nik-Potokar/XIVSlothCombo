using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
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
            HowlingFist = 25763,
            Enlightenment = 16474,
            MasterfulBlitz = 25764,
            ElixirField = 3545,
            FlintStrike = 25882,
            RisingPhoenix = 25768,
            ShadowOfTheDestroyer = 25767;

        public static class Buffs
        {
            public const short
                TwinSnakes = 101,
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110,
                LeadenFist = 1861,
                FormlessFist = 2513,
                DisciplinedFist = 3001;
        }

        public static class Debuffs
        {
            public const short
                Demolish = 246;
        }

        public static class Levels
        {
            public const byte
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                DragonKick = 50,
                TwinSnakes = 18,
                TrueStrike = 4,
                SnapPunch = 6;
        }
    }

    internal class MnkAoECombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Rockbreaker)
            {
                if (HasEffect(MNK.Buffs.PerfectBalance) || HasEffect(MNK.Buffs.FormlessFist))
                    return MNK.Rockbreaker;

                if (HasEffect(MNK.Buffs.OpoOpoForm) && level >= 26 && level <= 81)
                    return MNK.ArmOfTheDestroyer;

                if (HasEffect(MNK.Buffs.OpoOpoForm) && level >= 82)
                    return MNK.ShadowOfTheDestroyer;

                if (HasEffect(MNK.Buffs.RaptorForm) && level >= MNK.Levels.FourPointFury)
                    return MNK.FourPointFury;

                if (HasEffect(MNK.Buffs.CoerlForm) && level >= MNK.Levels.Rockbreaker)
                    return MNK.Rockbreaker;

                return MNK.ArmOfTheDestroyer;
            }

            return actionID;
        }
    }

    internal class MnkBootshineFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkBootshineFeature;

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

    internal class MnkBasicCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkBasicCombo;

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

            return MNK.Bootshine;
        }
    }

    internal class MonkPerfectBalanceFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MonkPerfectBalanceFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.PerfectBalance)
            {
                if (OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz)
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            return actionID;
        }
    }
    internal class MnkBasicComboPlus : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkBasicComboPlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Bootshine)
            {
                var gauge = GetJobGauge<MNKGauge>();
                var twinsnakeBuff = HasEffect(MNK.Buffs.DisciplinedFist);
                var twinsnakeDuration = FindEffect(MNK.Buffs.DisciplinedFist);
                var demolishDuration = FindTargetEffect(MNK.Debuffs.Demolish);
                //Nadi
                var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                var pbCD = GetCooldown(MNK.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var solarNadi = gauge.Nadi == Nadi.SOLAR;
                var nadiNONE = gauge.Nadi == Nadi.NONE;
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
                    if(!HasEffect(MNK.Buffs.LeadenFist))
                        return MNK.DragonKick;
                }
                if (!HasEffect(MNK.Buffs.LeadenFist) && HasEffect(MNK.Buffs.OpoOpoForm) && level >= MNK.Levels.DragonKick)
                    return MNK.DragonKick;
                return MNK.Bootshine;
                if (HasEffect(MNK.Buffs.FormlessFist))
                    return MNK.DragonKick;

            }
            return MNK.Bootshine;
        }
    }
    internal class MnkPerfectBalancePlus : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkPerfectBalancePlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == MNK.MasterfulBlitz)

            {
                var gauge = GetJobGauge<MNKGauge>();
                var pbStacks = FindEffectAny(MNK.Buffs.PerfectBalance);
                var pbCD = GetCooldown(MNK.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var solarNadi = gauge.Nadi == Nadi.SOLAR;
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
}