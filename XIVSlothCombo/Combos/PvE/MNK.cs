using System;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
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
            Thunderclap = 25762,
            RiddleOfEarth = 7394;

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
                Brotherhood = 1185,
                RiddleOfEarth = 1179;
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

        public static class Config
        {
            public const string
                MNK_Demolish_Apply = "MnkDemolishApply",
                MNK_DisciplinedFist_Apply = "MnkDisciplinedFistApply",
                MNK_STSecondWindThreshold = "MNK_STSecondWindThreshold",
                MNK_STBloodbathThreshold = "MNK_STBloodbathThreshold",
                MNK_AoESecondWindThreshold = "MNK_AoESecondWindThreshold",
                MNK_AoEBloodbathThreshold = "MNK_AoEBloodbathThreshold",
                MNK_DemolishTreshhold = "MNK_ST_DemolishThreshold";
        }


        internal class MNK_ST_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_BasicCombo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bootshine)
                {
                    var DemolishTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_DemolishTreshhold);
                    var EnemyHP = GetTargetHPPercent();
                    var twinsnakeDuration = GetBuffRemainingTime(Buffs.DisciplinedFist);
                    var demolishDuration = GetDebuffRemainingTime(Debuffs.Demolish);
                    var DemolishApply = PluginConfiguration.GetCustomFloatValue(Config.MNK_Demolish_Apply);
                    var DisciplinedFistApply = PluginConfiguration.GetCustomFloatValue(Config.MNK_DisciplinedFist_Apply);

                    if (!HasEffect(Buffs.PerfectBalance))
                    {

                        if (HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.OpoOpoForm))
                        {
                            return !LevelChecked(DragonKick) || HasEffect(Buffs.LeadenFist)
                            ? Bootshine
                            : DragonKick;
                        }
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.RaptorForm))
                    {
                        if (LevelChecked(TrueStrike))
                        {
                            return Bootshine;
                        }
                        return !LevelChecked(TwinSnakes) || (twinsnakeDuration >= DisciplinedFistApply)
                        ? TrueStrike
                        : TwinSnakes;
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                    {
                        return !LevelChecked(SnapPunch)
                        ? Bootshine
                        : !LevelChecked(Demolish) || (demolishDuration >= DemolishApply)
                            ? SnapPunch
                            : Demolish;
                    }
                }
                return actionID;
            }
        }

        internal class MNK_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_SimpleMode;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bootshine)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MNKGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var twinsnakeDuration = GetBuffRemainingTime(Buffs.DisciplinedFist);
                    var demolishDuration = GetDebuffRemainingTime(Debuffs.Demolish);
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var solarNadi = gauge.Nadi == Nadi.SOLAR;
                    var enemyHP = GetTargetHPPercent();
                    var DemolishTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_DemolishTreshhold);


                    // Opener for MNK
                    // Re-enter opener when Brotherhood is used
                    if (lastComboMove == Brotherhood)
                    {
                        inOpener = true;
                        openerFinished = false;
                    }

                    if (!inCombat)
                    {
                        if (inOpener || openerFinished)
                        {
                            inOpener = false;
                            openerFinished = false;
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
                        if (LevelChecked(RiddleOfFire))
                        {
                            // Early exit out of opener
                            if (!ActionReady(RiddleOfFire) && GetCooldownRemainingTime(RiddleOfFire) <= 40)
                            {
                                inOpener = false;
                                openerFinished = true;
                            }

                            // Delayed weave for Riddle of Fire specifically
                            if (canDelayedWeave)
                            {
                                if ((HasEffect(Buffs.CoerlForm) || lastComboMove == TwinSnakes) && ActionReady(RiddleOfFire) && InMeleeRange())
                                {
                                    return RiddleOfFire;
                                }
                            }

                            if (canWeave)
                            {
                                if (!ActionReady(RiddleOfFire) && GetCooldownRemainingTime(RiddleOfFire) <= 59)
                                {
                                    if (LevelChecked(Brotherhood) && ActionReady(Brotherhood) && !ActionReady(RiddleOfFire) &&
                                       (lastComboMove == Bootshine || lastComboMove == DragonKick))
                                    {
                                        return Brotherhood;
                                    }

                                    if (GetRemainingCharges(PerfectBalance) > 0 && !HasEffect(Buffs.PerfectBalance) && !HasEffect(Buffs.FormlessFist) &&
                                       (lastComboMove == Bootshine || lastComboMove == DragonKick) && OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                                    {
                                        return PerfectBalance;
                                    }

                                    if (LevelChecked(RiddleOfWind) && HasEffect(Buffs.PerfectBalance) && ActionReady(RiddleOfWind))
                                    {
                                        return RiddleOfWind;
                                    }

                                    if (gauge.Chakra == 5)
                                    {
                                        return OriginalHook(Meditation);
                                    }
                                }
                            }
                        }

                        else
                        {
                            // Automatically exit opener if we don't have Riddle of Fire
                            inOpener = false;
                            openerFinished = true;
                        }
                    }
                    // Out of combat preparation
                    if (!inCombat)
                    {
                        if (!inOpener && gauge.Chakra < 5 && LevelChecked(Meditation))
                        {
                            return Meditation;
                        }

                        if (!inOpener && LevelChecked(FormShift) && !HasEffect(Buffs.FormlessFist) && comboTime <= 0)
                        {
                            return FormShift;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_ST_Simple_Thunderclap) && !InMeleeRange() && gauge.Chakra == 5 && (!LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        {
                            return Thunderclap;
                        }
                    }

                    // Buffs
                    if (inCombat && !inOpener)
                    {
                        {
                            if (canWeave)
                            {
                                if (!HasEffect(Buffs.FormlessFist) &&
                                    LevelChecked(PerfectBalance) && !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.DisciplinedFist) &&
                                    OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                                {
                                    // Use Perfect Balance if:
                                    // 1. It's after Bootshine/Dragon Kick.
                                    // 2. At max stacks / before overcap.
                                    // 3. During Brotherhood.
                                    // 4. During Riddle of Fire after Demolish has been applied.
                                    // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                                    if ((lastComboMove == Bootshine || lastComboMove == DragonKick) &&
                                        ((GetRemainingCharges(PerfectBalance) == 2) ||
                                        (GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.Brotherhood)) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) > 40) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) > 6) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) < 10)))
                                    {
                                        return PerfectBalance;
                                    }
                                }
                            }

                            if (canDelayedWeave)
                            {
                                if (LevelChecked(RiddleOfFire) && ActionReady(RiddleOfFire) && HasEffect(Buffs.DisciplinedFist) && InMeleeRange())
                                {
                                    return RiddleOfFire;
                                }
                            }

                            if (canWeave)
                            {
                                if (LevelChecked(Brotherhood) &&
                                   ActionReady(Brotherhood) && !ActionReady(RiddleOfFire))
                                {
                                    return Brotherhood;
                                }

                                if (LevelChecked(RiddleOfWind) &&
                                   ActionReady(RiddleOfWind) && !ActionReady(RiddleOfFire) && !ActionReady(Brotherhood))
                                {
                                    return RiddleOfWind;
                                }
                            }
                        }

                        if (canWeave)
                        {
                            if (LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist) || !LevelChecked(TwinSnakes)))
                            {
                                if (!LevelChecked(RiddleOfFire) || (GetCooldownRemainingTime(RiddleOfFire) >= 1.5 && !ActionReady(RiddleOfFire) && lastComboMove != RiddleOfFire))
                                {
                                    return OriginalHook(Meditation);
                                }
                            }
                        }
                    }
                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) && ((!IsMoving && InActionRange(ElixirField)) || (IsMoving && InActionRange(RisingPhoenix))))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }

                    // Meditation Uptime
                    if (!InMeleeRange() && gauge.Chakra < 5 && LevelChecked(Meditation))
                    {
                        return Meditation;
                    }

                    // Masterful Blitz
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }

                    // Perfect Balance
                    if (HasEffect(Buffs.PerfectBalance))
                    {
                        bool opoopoChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.OPOOPO);
                        bool coeurlChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.COEURL);
                        bool raptorChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.RAPTOR);
                        bool canSolar = gauge.BeastChakra.Where(e => e == BeastChakra.OPOOPO).Count() != 2;
                        if (opoopoChakra)
                        {
                            if (coeurlChakra)
                            {
                                return TwinSnakes;
                            }
                            if (raptorChakra)
                            {
                                return Demolish;
                            }
                            if (lunarNadi && !solarNadi)
                            {
                                bool demolishFirst = !TargetHasEffect(Debuffs.Demolish);
                                if (!demolishFirst && HasEffect(Buffs.DisciplinedFist))
                                {
                                    demolishFirst = twinsnakeDuration >= demolishDuration;
                                }
                                return demolishFirst ? Demolish : TwinSnakes;
                            }
                        }
                        if (canSolar && (lunarNadi || !solarNadi))
                        {
                            if (!raptorChakra && (!HasEffect(Buffs.DisciplinedFist) || twinsnakeDuration <= 2.5))
                            {
                                return TwinSnakes;
                            }
                            if (!coeurlChakra && (!TargetHasEffect(Debuffs.Demolish) || demolishDuration <= 2.5))
                            {
                                return Demolish;
                            }
                        }
                        return HasEffect(Buffs.LeadenFist) && HasEffect(Buffs.OpoOpoForm) ? Bootshine : DragonKick;
                    }

                    // Monk Rotation
                    if ((LevelChecked(DragonKick) && HasEffect(Buffs.OpoOpoForm)) || HasEffect(Buffs.FormlessFist))
                    {
                        return HasEffect(Buffs.LeadenFist) ? Bootshine : DragonKick;
                    }

                    if (LevelChecked(TrueStrike) && HasEffect(Buffs.RaptorForm))
                    {
                        if (LevelChecked(TwinSnakes) && (!HasEffect(Buffs.DisciplinedFist) || twinsnakeDuration <= 6))
                        {
                            return TwinSnakes;
                        }
                        return TrueStrike;
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.RaptorForm))
                    {
                        if (LevelChecked(TrueStrike))
                        {
                            return Bootshine;
                        }
                        return !LevelChecked(TwinSnakes) || (twinsnakeDuration >= 6)
                           ? TrueStrike
                           : TwinSnakes;
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                    {
                        return !LevelChecked(SnapPunch)
                            ? Bootshine
                            : !LevelChecked(Demolish) || (demolishDuration >= 6)
                                ? SnapPunch
                                : Demolish;
                    }
                }
                return actionID;
            }
        }
        internal class MNK_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_AdvancedMode;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bootshine)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MNKGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var twinsnakeDuration = GetBuffRemainingTime(Buffs.DisciplinedFist);
                    var demolishDuration = GetDebuffRemainingTime(Debuffs.Demolish);
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var solarNadi = gauge.Nadi == Nadi.SOLAR;
                    var enemyHP = GetTargetHPPercent();
                    var DemolishTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_DemolishTreshhold);
                    var SecondWindTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_STSecondWindThreshold);
                    var BloodBathTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_STBloodbathThreshold);
                    var DemolishApply = PluginConfiguration.GetCustomFloatValue(Config.MNK_Demolish_Apply);
                    var DisciplinedFistApply = PluginConfiguration.GetCustomFloatValue(Config.MNK_DisciplinedFist_Apply);


                    // Opener for MNK
                    if (IsEnabled(CustomComboPreset.MNK_ST_LunarSolarOpener))
                    {
                        // Re-enter opener when Brotherhood is used
                        if (lastComboMove == Brotherhood)
                        {
                            inOpener = true;
                            openerFinished = false;
                        }

                        if (!inCombat)
                        {
                            if (inOpener || openerFinished)
                            {
                                inOpener = false;
                                openerFinished = false;
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
                            if (LevelChecked(RiddleOfFire))
                            {
                                // Early exit out of opener
                                if (!ActionReady(RiddleOfFire) && GetCooldownRemainingTime(RiddleOfFire) <= 40)
                                {
                                    inOpener = false;
                                    openerFinished = true;
                                }

                                // Delayed weave for Riddle of Fire specifically
                                if (canDelayedWeave)
                                {
                                    if ((HasEffect(Buffs.CoerlForm) || lastComboMove == TwinSnakes) && ActionReady(RiddleOfFire) && InMeleeRange())
                                    {
                                        return RiddleOfFire;
                                    }
                                }

                                if (canWeave)
                                {
                                    if (!ActionReady(RiddleOfFire) && GetCooldownRemainingTime(RiddleOfFire) <= 59)
                                    {
                                        if (LevelChecked(Brotherhood) && ActionReady(Brotherhood) && !ActionReady(RiddleOfFire) &&
                                           (lastComboMove == Bootshine || lastComboMove == DragonKick))
                                        {
                                            return Brotherhood;
                                        }

                                        if (GetRemainingCharges(PerfectBalance) > 0 && !HasEffect(Buffs.PerfectBalance) && !HasEffect(Buffs.FormlessFist) &&
                                           (lastComboMove == Bootshine || lastComboMove == DragonKick) && OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                                        {
                                            return PerfectBalance;
                                        }

                                        if (LevelChecked(RiddleOfWind) && HasEffect(Buffs.PerfectBalance) && ActionReady(RiddleOfWind))
                                        {
                                            return RiddleOfWind;
                                        }

                                        if (gauge.Chakra == 5)
                                        {
                                            return OriginalHook(Meditation);
                                        }
                                    }
                                    // Healing 
                                    if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                                    {
                                        if (PlayerHealthPercentageHp() <= SecondWindTreshold && LevelChecked(All.SecondWind) && ActionReady(All.SecondWind))
                                            return All.SecondWind;
                                        if (PlayerHealthPercentageHp() <= BloodBathTreshold && LevelChecked(All.Bloodbath) && ActionReady(All.Bloodbath))
                                            return All.Bloodbath;
                                    }
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

                    // Out of combat preparation
                    if (!inCombat)
                    {
                        if (!inOpener && gauge.Chakra < 5 && LevelChecked(Meditation))
                        {
                            return Meditation;
                        }

                        if (!inOpener && LevelChecked(FormShift) && !HasEffect(Buffs.FormlessFist) && comboTime <= 0)
                        {
                            return FormShift;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_ST_ADV_Thunderclap) && !InMeleeRange() && gauge.Chakra == 5 && (!LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        {
                            return Thunderclap;
                        }
                    }

                    // Buffs
                    if (inCombat && !inOpener)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs))
                        {
                            if (canWeave)
                            {
                                if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_PerfectBalance) && !HasEffect(Buffs.FormlessFist) &&
                                    LevelChecked(PerfectBalance) && !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.DisciplinedFist) &&
                                    OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                                {
                                    // Use Perfect Balance if:
                                    // 1. It's after Bootshine/Dragon Kick.
                                    // 2. At max stacks / before overcap.
                                    // 3. During Brotherhood.
                                    // 4. During Riddle of Fire after Demolish has been applied.
                                    // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                                    if ((lastComboMove == Bootshine || lastComboMove == DragonKick) &&
                                        ((GetRemainingCharges(PerfectBalance) == 2) ||
                                        (GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.Brotherhood)) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) > 40) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) > 6) ||
                                        (GetRemainingCharges(PerfectBalance) >= 1 && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) < 10)))
                                    {
                                        return PerfectBalance;
                                    }
                                }
                            }

                            if (canDelayedWeave)
                            {
                                if (LevelChecked(RiddleOfFire) && ActionReady(RiddleOfFire) && HasEffect(Buffs.DisciplinedFist) && InMeleeRange())
                                {
                                    return RiddleOfFire;
                                }
                            }

                            if (canWeave)
                            {
                                if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_Brotherhood) && LevelChecked(Brotherhood) &&
                                   ActionReady(Brotherhood) && !ActionReady(RiddleOfFire))
                                {
                                    return Brotherhood;
                                }

                                if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_RiddleOfWind) && LevelChecked(RiddleOfWind) &&
                                   ActionReady(RiddleOfWind) && !ActionReady(RiddleOfFire) && !ActionReady(Brotherhood))
                                {
                                    return RiddleOfWind;
                                }


                                // Healing
                                if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                                {
                                    if (PlayerHealthPercentageHp() <= SecondWindTreshold && LevelChecked(All.SecondWind) && ActionReady(All.SecondWind))
                                        return All.SecondWind;
                                    if (PlayerHealthPercentageHp() <= BloodBathTreshold && LevelChecked(All.Bloodbath) && ActionReady(All.Bloodbath))
                                        return All.Bloodbath;
                                }
                            }
                        }

                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.MNK_ST_Meditation) && LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist) || !LevelChecked(TwinSnakes)))
                            {
                                if (!LevelChecked(RiddleOfFire) || !IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs) || (GetCooldownRemainingTime(RiddleOfFire) >= 1.5 && !ActionReady(RiddleOfFire) && lastComboMove != RiddleOfFire))
                                {
                                    return OriginalHook(Meditation);
                                }
                            }
                        }
                    }
                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (IsEnabled(CustomComboPreset.MNK_ST_MasterfulBlitz) && level >= Levels.MasterfulBlitz && !HasEffect(Buffs.PerfectBalance)
                        && (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) && ((!IsMoving && InActionRange(MasterfulBlitz))))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }
                    // Meditation Uptime
                    if (IsEnabled(CustomComboPreset.MNK_ST_Meditation_Uptime) && !InMeleeRange() && gauge.Chakra < 5 && LevelChecked(Meditation))
                    {
                        return Meditation;
                    }

                    // Masterful Blitz
                    if (IsEnabled(CustomComboPreset.MNK_ST_MasterfulBlitz) && LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }

                    // Perfect Balance
                    if (HasEffect(Buffs.PerfectBalance))
                    {
                        bool opoopoChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.OPOOPO);
                        bool coeurlChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.COEURL);
                        bool raptorChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.RAPTOR);
                        bool canSolar = gauge.BeastChakra.Where(e => e == BeastChakra.OPOOPO).Count() != 2;
                        if (opoopoChakra)
                        {
                            if (coeurlChakra)
                            {
                                return TwinSnakes;
                            }
                            if (raptorChakra)
                            {
                                return Demolish;
                            }
                            if (lunarNadi && !solarNadi)
                            {
                                bool demolishFirst = !TargetHasEffect(Debuffs.Demolish);
                                if (!demolishFirst && HasEffect(Buffs.DisciplinedFist))
                                {
                                    demolishFirst = twinsnakeDuration >= demolishDuration;
                                }
                                return demolishFirst ? Demolish : TwinSnakes;
                            }
                        }
                        if (canSolar && (lunarNadi || !solarNadi))
                        {
                            if (!raptorChakra && (!HasEffect(Buffs.DisciplinedFist) || twinsnakeDuration <= 2.5))
                            {
                                return TwinSnakes;
                            }
                            if (!coeurlChakra && (!TargetHasEffect(Debuffs.Demolish) || demolishDuration <= 2.5))
                            {
                                return Demolish;
                            }
                        }
                        return HasEffect(Buffs.LeadenFist) && HasEffect(Buffs.OpoOpoForm) ? Bootshine : DragonKick;
                    }

                    // Monk Rotation
                    if ((LevelChecked(DragonKick) && HasEffect(Buffs.OpoOpoForm)) || HasEffect(Buffs.FormlessFist))
                    {
                        return HasEffect(Buffs.LeadenFist) ? Bootshine : DragonKick;
                    }

                    if (HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.OpoOpoForm))
                    {
                        return !LevelChecked(DragonKick) || HasEffect(Buffs.LeadenFist)
                            ? Bootshine
                            : DragonKick;
                    }

                if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.RaptorForm))
                {
                    if (LevelChecked(TrueStrike))
                    {
                        return Bootshine;
                    }
                    return !LevelChecked(TwinSnakes) || (twinsnakeDuration >= DisciplinedFistApply)
                       ? TrueStrike
                       : TwinSnakes;
                }

                if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                {
                    return !LevelChecked(SnapPunch)
                        ? Bootshine
                        : !LevelChecked(Demolish) || ((demolishDuration >= DemolishApply) && (enemyHP < DemolishTreshold))
                            ? SnapPunch
                            : Demolish;
                }
            }
                    return actionID;
                }
    }
        internal class MNK_DragonKick_Bootshine : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_DragonKick_Bootshine;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DragonKick)
                {
                    if (IsEnabled(CustomComboPreset.MNK_BootshineBalance) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                        return OriginalHook(MasterfulBlitz);

                    if (HasEffect(Buffs.LeadenFist) &&
                       (HasEffect(Buffs.FormlessFist) ||
                       HasEffect(Buffs.PerfectBalance) ||
                       HasEffect(Buffs.OpoOpoForm)))
                        return Bootshine;

                    if (!LevelChecked(DragonKick))
                        return Bootshine;
                }

                return actionID;
            }
        }

        internal class MNK_TwinSnakes : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_TwinSnakes;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == TrueStrike)
                {
                    var disciplinedFistBuff = HasEffect(Buffs.DisciplinedFist);
                    var disciplinedFistDuration = GetBuffRemainingTime(Buffs.DisciplinedFist);

                    if (LevelChecked(TrueStrike))
                    {
                        if ((!disciplinedFistBuff && LevelChecked(TwinSnakes)) || (disciplinedFistDuration < 6 && LevelChecked(TwinSnakes)))
                            return TwinSnakes;
                        return TrueStrike;
                    }
                }
                return actionID;
            }
        }

        internal class MNK_PerfectBalance : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_PerfectBalance;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == PerfectBalance)
                {
                    if (OriginalHook(MasterfulBlitz) != MasterfulBlitz && LevelChecked(MasterfulBlitz))
                        return OriginalHook(MasterfulBlitz);
                }

                return actionID;
            }
        }

        internal class MNK_PerfectBalance_Plus : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_PerfectBalance_Plus;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<MNKGauge>();
                var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                var nadiNONE = gauge.Nadi == Nadi.NONE;


                if (actionID == MasterfulBlitz)
                {

                    if (!nadiNONE && !lunarNadi)
                    {
                        if (pbStacks?.StackCount == 3)
                            return DragonKick;

                        if (pbStacks?.StackCount == 2)
                            return Bootshine;

                        if (pbStacks?.StackCount == 1)
                            return DragonKick;
                    }

                    if (nadiNONE)
                    {
                        if (pbStacks?.StackCount == 3)
                            return DragonKick;

                        if (pbStacks?.StackCount == 2)
                            return Bootshine;

                        if (pbStacks?.StackCount == 1)
                            return DragonKick;
                    }

                    if (lunarNadi)
                    {
                        if (pbStacks?.StackCount == 3)
                            return TwinSnakes;

                        if (pbStacks?.StackCount == 2)
                            return DragonKick;

                        if (pbStacks?.StackCount == 1)
                            return Demolish;
                    }

                }
                return actionID;
            }
        }

        internal class MNK_Riddle_Brotherhood : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_Riddle_Brotherhood;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RiddleOfFire && LevelChecked(Brotherhood) && !ActionReady(RiddleOfFire) && ActionReady(Brotherhood))
                    return Brotherhood;
                else return actionID;
            }
        }

        internal class MNK_HowlingFistMeditation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_HowlingFistMeditation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HowlingFist || actionID == Enlightenment)
                {
                    var gauge = GetJobGauge<MNKGauge>();

                    if (gauge.Chakra < 5)
                    {
                        return Meditation;
                    }
                }
                return actionID;
            }
        }
        //Riddle Of Earth spam Protection 
        internal class MNK_RiddleOfEarthProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_RiddleOfEarthProtection;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RiddleOfEarth && HasEffect(Buffs.RiddleOfEarth))
                    return BLM.Fire;
                else
                    return actionID;
            }
        }

        internal class MNK_AOE_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_BasicCombo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmOfTheDestroyer || actionID == ShadowOfTheDestroyer)
                {
                    if (HasEffect(Buffs.OpoOpoForm))
                    {
                        return OriginalHook(ArmOfTheDestroyer);
                    }

                    if (HasEffect(Buffs.RaptorForm) && LevelChecked(FourPointFury) || HasEffect(Buffs.FormlessFist))
                    {
                        return FourPointFury;
                    }

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                    {
                        return Rockbreaker;
                    }
                }
                return actionID;
            }
        }

        internal class MNK_AOE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmOfTheDestroyer || actionID == ShadowOfTheDestroyer)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MNKGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canWeaveChakra = CanWeave(actionID);
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var nadiNONE = gauge.Nadi == Nadi.NONE;

                    if (!inCombat)
                    {
                        if (gauge.Chakra < 5 && LevelChecked(Meditation))
                        {
                            return Meditation;
                        }

                        if (LevelChecked(FormShift) && !HasEffect(Buffs.FormlessFist) && comboTime <= 0)
                        {
                            return FormShift;
                        }

                        if (!InMeleeRange() && gauge.Chakra == 5 && (!LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        {
                            return Thunderclap;
                        }
                    }

                    // Buffs
                    if (inCombat && canWeave)
                    {
                        if (LevelChecked(RiddleOfFire) && !IsOnCooldown(RiddleOfFire))
                        {
                            return RiddleOfFire;
                        }

                        if (LevelChecked(PerfectBalance) && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) == MasterfulBlitz)
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

                        if (LevelChecked(Brotherhood) && ActionReady(Brotherhood))
                        {
                            return Brotherhood;
                        }

                        if (LevelChecked(RiddleOfWind) && ActionReady(RiddleOfWind))
                        {
                            return RiddleOfWind;
                        }

                        if (LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist) ||
                            !LevelChecked(TwinSnakes)) && canWeaveChakra)
                        {
                            return LevelChecked(Enlightenment) ? OriginalHook(Enlightenment) : OriginalHook(Meditation);
                        }
                    }


                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) && ((!IsMoving && InActionRange(ElixirField)) || (IsMoving && InActionRange(RisingPhoenix))))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }

                    // Masterful Blitz
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
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
                                return LevelChecked(ShadowOfTheDestroyer) ? ShadowOfTheDestroyer : Rockbreaker;
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

                    if (HasEffect(Buffs.RaptorForm) && LevelChecked(FourPointFury) || HasEffect(Buffs.FormlessFist))
                    {
                        return FourPointFury;
                    }

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                    {
                        return Rockbreaker;
                    }
                }
                return actionID;
            }
        }

        internal class MNK_AOE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmOfTheDestroyer || actionID == ShadowOfTheDestroyer)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MNKGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canWeaveChakra = CanWeave(actionID);
                    var pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    var lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    var nadiNONE = gauge.Nadi == Nadi.NONE;
                    var SecondWindTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_STSecondWindThreshold);
                    var BloodBathTreshold = PluginConfiguration.GetCustomIntValue(Config.MNK_STBloodbathThreshold);

                    if (!inCombat)
                    {
                        if (gauge.Chakra < 5 && LevelChecked(Meditation))
                        {
                            return Meditation;
                        }

                        if (LevelChecked(FormShift) && !HasEffect(Buffs.FormlessFist) && comboTime <= 0)
                        {
                            return FormShift;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_AoE_Thunderclap) && !InMeleeRange() && gauge.Chakra == 5 && (LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        {
                            return Thunderclap;
                        }
                    }

                    // Buffs
                    if (inCombat && canWeave)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_AoE_ADV_CDs))
                        {
                            if (LevelChecked(RiddleOfFire) && ActionReady(RiddleOfFire))
                            {
                                return RiddleOfFire;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_AoE_CDs_PerfectBalance) && LevelChecked(PerfectBalance) && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) == MasterfulBlitz)
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

                            if (IsEnabled(CustomComboPreset.MNK_AoE_CDs_Brotherhood) && LevelChecked(Brotherhood) && ActionReady(Brotherhood))
                            {
                                return Brotherhood;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_AoE_CDs_RiddleOfWind) && LevelChecked(RiddleOfWind) && ActionReady(RiddleOfWind))
                            {
                                return RiddleOfWind;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_AoE_Meditation) && LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist) ||
                            !LevelChecked(TwinSnakes)) && canWeaveChakra)
                        {
                            return LevelChecked(Enlightenment) ? OriginalHook(Enlightenment) : OriginalHook(Meditation);
                        }
                    }

                    // healing - please move if not appropriate this high priority
                    if (IsEnabled(CustomComboPreset.MNK_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= SecondWindTreshold && LevelChecked(All.SecondWind) && ActionReady(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= BloodBathTreshold && LevelChecked(All.Bloodbath) && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                // Masterful Blitz ElixirField/RisingPhoenix
                if (IsEnabled(CustomComboPreset.MNK_AoE_MasterfulBlitz) && LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                    && (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) && ((!IsMoving && InActionRange(ElixirField)) || (IsMoving && InActionRange(RisingPhoenix))))
                {
                    return OriginalHook(MasterfulBlitz);
                }

                    // Masterful Blitz
                    if (IsEnabled(CustomComboPreset.MNK_AoE_MasterfulBlitz) && LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
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
                                return LevelChecked(ShadowOfTheDestroyer) ? ShadowOfTheDestroyer : Rockbreaker;
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

                    if (HasEffect(Buffs.RaptorForm) && LevelChecked(FourPointFury) || HasEffect(Buffs.FormlessFist))
                    {
                        return FourPointFury;
                    }

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                    {
                        return Rockbreaker;
                    }
                }
                return actionID;
            }
        }
    }
}