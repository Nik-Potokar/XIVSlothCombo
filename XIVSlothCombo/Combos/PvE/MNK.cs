using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class MNK
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            Bootshine = 53,
            TrueStrike = 54,
            SnapPunch = 56,
            Meditation = 36940,
            SteelPeak = 3547,
            TwinSnakes = 61,
            ArmOfTheDestroyer = 62,
            Demolish = 66,
            Mantra = 65,
            DragonKick = 74,
            Rockbreaker = 70,
            Thunderclap = 25762,
            HowlingFist = 16474,
            FourPointFury = 16473,
            PerfectBalance = 69,
            FormShift = 4262,
            TheForbiddenChakra = 3547,
            MasterfulBlitz = 25764,
            RiddleOfEarth = 7394,
            EarthsReply = 36944,
            RiddleOfFire = 7395,
            Brotherhood = 7396,
            RiddleOfWind = 25766,
            EnlightenedMeditation = 36943,
            Enlightenment = 16474,
            SixSidedStar = 16476,
            ShadowOfTheDestroyer = 25767,
            WindsReply = 36949,
            ForbiddenMeditation = 36942,
            LeapingOpo = 36945,
            RisingRaptor = 36946,
            PouncingCoeurl = 36947,
            TrueNorth = 7546,
            ElixirBurst = 36948,
            FiresReply = 36950;

        public static class Buffs
        {
            public const ushort
                TwinSnakes = 101,
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110,
                RiddleOfFire = 1181,
                RiddleOfWind = 2687,
                LeadenFist = 1861,
                FormlessFist = 2513,
                DisciplinedFist = 3001,
                TrueNorth = 1250,
                WindsRumination = 3842,
                FiresRumination = 3843,
                Brotherhood = 1185;
        }

        public static class Levels
        {
            public const byte
                TrueStrike = 4,
                SnapPunch = 6,
                Meditation = 15,
                SteelPeak = 15,
                TwinSnakes = 18,
                ArmOfTheDestroyer = 26,
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                HowlingFist = 40,
                DragonKick = 50,
                PerfectBalance = 50,
                TrueNorth = 50,
                FormShift = 52,
                MasterfulBlitz = 60,
                RiddleOfFire = 68,
                Enlightenment = 70,
                Brotherhood = 70,
                RiddleOfWind = 72,
                TheForbiddenChakra = 54,
                ShadowOfTheDestroyer = 82,
                WindsReply = 96,
                FiresReply = 100;
        }

        public static MNKGauge Gauge => CustomComboFunctions.GetJobGauge<MNKGauge>();

        public static class Config
        {
            public const string
                MNK_STSecondWindThreshold = "MNK_STSecondWindThreshold",
                MNK_STBloodbathThreshold = "MNK_STBloodbathThreshold",
                MNK_AoESecondWindThreshold = "MNK_AoESecondWindThreshold",
                MNK_AoEBloodbathThreshold = "MNK_AoEBloodbathThreshold",
                MNK_VariantCure = "MNK_VariantCure";
        }

        internal class MNK_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_BasicAOECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmOfTheDestroyer || actionID == ShadowOfTheDestroyer)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MNKGauge>();
                    var canWeave = CanWeave(actionID, 0.5);
                    var canWeaveChakra = CanWeave(actionID);
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var nadiNONE = gauge.Nadi == Nadi.NONE;

                    if (!inCombat)
                    {
                        if (gauge.Chakra < 5 && level >= Levels.Meditation)
                        {
                            return OriginalHook(Meditation); ;
                        }
                    }

                    if (IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;

                    // Buffs
                    if (inCombat && canWeave)
                    {
                        if (IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && level >= Levels.RiddleOfFire && !IsOnCooldown(RiddleOfFire))
                        {
                            return RiddleOfFire;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && level >= Levels.PerfectBalance && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                        {
                            // Use Perfect Balance if:
                            // 1. It's after Bootshine/Dragon Kick.
                            // 2. At max stacks / before overcap.
                            // 3. During Brotherhood.
                            // 4. During Riddle of Fire.
                            // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                            if ((GetRemainingCharges(PerfectBalance) == 2) ||
                                (GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.Brotherhood)) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                            {
                                return PerfectBalance;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && level >= Levels.Brotherhood && !IsOnCooldown(Brotherhood))
                        {
                            return Brotherhood;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && level >= Levels.RiddleOfWind && !IsOnCooldown(RiddleOfWind))
                        {
                            return RiddleOfWind;
                        }

                        if (Gauge.Chakra == 5
                            && level >= Levels.HowlingFist
                            && HasBattleTarget())
                        {
                            return OriginalHook(EnlightenedMeditation);
                        }

                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_AoESecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_AoEBloodbathThreshold) && LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (inCombat)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && HasEffect(Buffs.WindsRumination) && level >= Levels.WindsReply)
                        {
                            return WindsReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_BasicAOECombo_UseCooldowns) && HasEffect(Buffs.FiresRumination) && level >= Levels.FiresReply)
                        {
                            return FiresReply;
                        }

                        // Masterful Blitz
                        if (level >= Levels.MasterfulBlitz && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                        {
                            return OriginalHook(MasterfulBlitz);
                        }

                        // Perfect Balance
                        if (HasEffect(Buffs.PerfectBalance))
                        {
                            if (nadiNONE || !lunarNadi)
                            {
                                if (pbStacks?.StackCount > 0)
                                {
                                    return level >= Levels.ShadowOfTheDestroyer ? ShadowOfTheDestroyer : Rockbreaker;
                                }
                            }
                            if (lunarNadi)
                            {
                                switch (pbStacks?.StackCount)
                                {
                                    case 3:
                                        return OriginalHook(ArmOfTheDestroyer);
                                    case 2:
                                        return FourPointFury;
                                    case 1:
                                        return Rockbreaker;
                                }
                            }
                        }

                        // Monk Rotation
                        if (HasEffect(Buffs.OpoOpoForm))
                        {
                            return OriginalHook(ArmOfTheDestroyer);
                        }

                        if (HasEffect(Buffs.RaptorForm))
                        {
                            if (FourPointFury.LevelChecked())
                                return FourPointFury;

                            if (TwinSnakes.LevelChecked())
                                return TwinSnakes;
                        }

                        if (HasEffect(Buffs.CoerlForm) && level >= Levels.Rockbreaker)
                        {
                            return Rockbreaker;
                        }
                    }
                }
                return actionID;
            }
        }

        internal class MNK_ST_CustomMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_CustomCombo;
            internal static MNKOpenerLogic MNKOpener = new();
            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == Bootshine || actionID == LeapingOpo)
                {
                    var canWeave = CanWeave(actionID, 0.5);
                    var canWeaveChakra = CanWeave(actionID);
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                    if (IsEnabled(CustomComboPreset.MNK_STUseLLOpener))
                    {
                        if (MNKOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_STUseMeditation)
                        && (!inCombat || !InMeleeRange())
                        && Gauge.Chakra < 5
                        && LevelChecked(Meditation))
                    {
                        return OriginalHook(Meditation);
                    }

                    if (inCombat && canWeave)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                        {
                            if (IsEnabled(CustomComboPreset.MNK_STUseBrotherhood)
                                && level >= Levels.Brotherhood
                                && !IsOnCooldown(Brotherhood))
                            {
                                return Brotherhood;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_STUseROW)
                                && level >= Levels.RiddleOfWind
                                && !IsOnCooldown(RiddleOfWind))
                            {
                                return RiddleOfWind;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_STUseROF)
                                && level >= Levels.RiddleOfFire
                                && !IsOnCooldown(RiddleOfFire))
                            {
                                return RiddleOfFire;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra)
                            && Gauge.Chakra >= 5
                            && level >= Levels.SteelPeak)
                        {
                            return OriginalHook(Meditation);
                        }
                    }

                    if (inCombat)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply)
                            && HasEffect(Buffs.WindsRumination)
                            && level >= Levels.WindsReply)
                        {
                            return WindsReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply)
                            && HasEffect(Buffs.FiresRumination)
                            && level >= Levels.FiresReply)
                        {
                            return FiresReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance))
                        {
                            // Masterful Blitz
                            if (level >= Levels.MasterfulBlitz && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                            {
                                return OriginalHook(MasterfulBlitz);
                            }

                            // Perfect Balance
                            if (level >= Levels.PerfectBalance && !HasEffect(Buffs.PerfectBalance))
                            {
                                if ((GetRemainingCharges(PerfectBalance) == 2) ||
                                    (GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                    (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.Brotherhood)) ||
                                    (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                                    (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                                {
                                    return PerfectBalance;
                                }
                            }

                            // Perfect Balance
                            if (HasEffect(Buffs.PerfectBalance))
                            {
                                return DeterminePBAbility(actionID);
                            }
                        }

                        return DetermineCoreAbility(actionID);
                    }
                }

                return actionID;
            }

            private uint DeterminePBAbility(uint baseActionID)
            {
                var lunarNadi = Gauge.Nadi == Nadi.LUNAR;
                var solarNadi = Gauge.Nadi == Nadi.SOLAR;

                bool opoopoChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.OPOOPO);
                bool coeurlChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.COEURL);
                bool raptorChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.RAPTOR);
                bool canSolar = Gauge.BeastChakra.Where(e => e == BeastChakra.OPOOPO).Count() != 2;

                if (opoopoChakra)
                {
                    if (coeurlChakra)
                    {
                        if (Gauge.RaptorFury == 0)
                        {
                            if (LevelChecked(Levels.TwinSnakes))
                                return TwinSnakes;
                        }
                        else
                        {
                            if (LevelChecked(Levels.TrueStrike))
                                return OriginalHook(TrueStrike);
                        }
                    }
                    if (raptorChakra)
                    {
                        if (!OnTargetsRear()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                        {
                            return TrueNorth;
                        }
                        else
                        {
                            if (LevelChecked(Levels.Demolish))
                                return Demolish;
                        }
                    }

                    if (!OnTargetsFlank()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                    {
                        return TrueNorth;
                    }
                    else
                    {
                        if (LevelChecked(Levels.SnapPunch))
                            return OriginalHook(SnapPunch);
                    }
                }

                if (canSolar && (lunarNadi || !solarNadi))
                {
                    if (!raptorChakra)
                    {
                        if (Gauge.RaptorFury == 0)
                        {
                            if (LevelChecked(Levels.TwinSnakes))
                                return TwinSnakes;
                        }
                        else
                        {
                            if (LevelChecked(Levels.TrueStrike))
                                return OriginalHook(TrueStrike);
                        }
                    }
                    if (!coeurlChakra)
                    {
                        if (!OnTargetsRear()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                        {
                            return TrueNorth;
                        }
                        else
                        {
                            if (LevelChecked(Levels.Demolish))
                                return Demolish;
                        }
                    }

                    if (!OnTargetsFlank()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                    {
                        return TrueNorth;
                    }
                    else
                    {
                        if (LevelChecked(Levels.SnapPunch))
                            return OriginalHook(SnapPunch);
                    }
                }

                if (Gauge.OpoOpoFury == 0)
                {
                    if (LevelChecked(Levels.DragonKick))
                        return DragonKick;
                }

                return OriginalHook(Bootshine);
            }

            private uint DetermineCoreAbility(uint baseActionID)
            {
                if (HasEffect(Buffs.OpoOpoForm))
                {
                    if (Gauge.OpoOpoFury == 0)
                    {
                        if (LevelChecked(Levels.DragonKick))
                            return DragonKick;
                    }
                    else
                    {
                        return OriginalHook(Bootshine);
                    }
                }

                if (HasEffect(Buffs.RaptorForm))
                {
                    if (Gauge.RaptorFury == 0)
                    {
                        if (LevelChecked(Levels.TwinSnakes))
                            return TwinSnakes;
                    }
                    else
                    {
                        if (LevelChecked(Levels.TrueStrike))
                            return OriginalHook(TrueStrike);
                    }
                }

                if (HasEffect(Buffs.CoerlForm))
                {
                    // Can we warn about the positional here?
                    if (Gauge.CoeurlFury == 0)
                    {
                        if (!OnTargetsRear()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                        {
                            return TrueNorth;
                        }
                        else
                        {
                            if (LevelChecked(Levels.Demolish))
                                return Demolish;
                        }
                    }

                    if (!OnTargetsFlank()
                            && IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                            && TargetNeedsPositionals()
                            && !HasEffect(Buffs.TrueNorth)
                            && LevelChecked(Levels.TrueNorth)
                            && HasCharges(TrueNorth))
                    {
                        return TrueNorth;
                    }
                    else
                    {
                        if (LevelChecked(Levels.SnapPunch))
                            return OriginalHook(SnapPunch);
                    }
                }

                return baseActionID;
            }
        }

        internal class MNK_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_BasicCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bootshine || actionID == LeapingOpo)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var canWeave = CanWeave(actionID, 0.5);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);

                    // Buffs
                    if (!inCombat)
                    {
                        if (Gauge.Chakra < 5 && level >= Levels.Meditation)
                        {
                            return OriginalHook(Meditation);
                        }
                    }

                    if (inCombat)
                    {
                        if (level >= Levels.Brotherhood && !IsOnCooldown(Brotherhood))
                        {
                            return Brotherhood;
                        }

                        if (level >= Levels.RiddleOfWind && !IsOnCooldown(RiddleOfWind))
                        {
                            return RiddleOfWind;
                        }

                        if (level >= Levels.RiddleOfFire && !IsOnCooldown(RiddleOfFire))
                        {
                            return RiddleOfFire;
                        }

                        if (Gauge.Chakra == 5 && level >= Levels.SteelPeak)
                        {
                            return OriginalHook(TheForbiddenChakra);
                        }

                        // Masterful Blitz
                        if (level >= Levels.MasterfulBlitz && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                        {
                            return OriginalHook(MasterfulBlitz);
                        }

                        // Perfect Balance
                        if (level >= Levels.PerfectBalance && !HasEffect(Buffs.PerfectBalance))
                        {
                            if ((GetRemainingCharges(PerfectBalance) == 2) ||
                                (GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.Brotherhood)) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                                (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                            {
                                return PerfectBalance;
                            }
                        }

                        // Perfect Balance
                        if (HasEffect(Buffs.PerfectBalance))
                        {
                            return DeterminePBAbility(actionID);
                        }

                        return DetermineCoreAbility(actionID);
                    }
                }
                return actionID;
            }

            private uint DeterminePBAbility(uint baseActionID)
            {
                var lunarNadi = Gauge.Nadi == Nadi.LUNAR;
                var solarNadi = Gauge.Nadi == Nadi.SOLAR;

                bool opoopoChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.OPOOPO);
                bool coeurlChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.COEURL);
                bool raptorChakra = Array.Exists(Gauge.BeastChakra, e => e == BeastChakra.RAPTOR);
                bool canSolar = Gauge.BeastChakra.Where(e => e == BeastChakra.OPOOPO).Count() != 2;

                if (opoopoChakra)
                {
                    if (coeurlChakra)
                    {
                        return Gauge.RaptorFury == 0 ? TwinSnakes : TrueStrike;
                    }
                    if (raptorChakra)
                    {
                        return Gauge.CoeurlFury == 0 ? Demolish : SnapPunch;
                    }
                }

                if (canSolar && (lunarNadi || !solarNadi))
                {
                    if (!raptorChakra)
                    {
                        return Gauge.RaptorFury == 0 ? TwinSnakes : TrueStrike;
                    }
                    if (!coeurlChakra)
                    {
                        return Gauge.CoeurlFury == 0 ? Demolish : SnapPunch;
                    }
                }

                return Gauge.OpoOpoFury == 0 ? DragonKick : Bootshine;
            }

            private uint DetermineCoreAbility(uint baseActionID)
            {
                if (HasEffect(Buffs.OpoOpoForm))
                {
                    return Gauge.OpoOpoFury == 0 ? DragonKick : OriginalHook(Bootshine);
                }

                if (HasEffect(Buffs.RaptorForm))
                {
                    return Gauge.RaptorFury == 0 ? TwinSnakes : OriginalHook(TrueStrike);
                }

                if (HasEffect(Buffs.CoerlForm))
                {
                    // Can we warn about the positional here?
                    return Gauge.CoeurlFury == 0 ? Demolish : OriginalHook(SnapPunch);
                }

                return baseActionID;
            }
        }
    }
}