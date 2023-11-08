using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class MNK
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
            public static UserFloat
                MNK_Demolish_Apply = new("MnkDemolishApply"),
                MNK_DisciplinedFist_Apply = new("MnkDisciplinedFistApply"),
                MNK_DemolishTreshhold = new("MNK_ST_DemolishThreshold");


            public static UserInt
                MNK_ST_Adv_SecondWindThreshold = new("MNK_ST_Adv_SecondWindThreshold"),
                MNK_ST_Adv_BloodbathThreshold = new("MNK_ST_Adv_BloodbathThreshold"),
                MNK_AoE_Adv_SecondWindThreshold = new("MNK_AoE_Adv_SecondWindThreshold"),
                MNK_AoE_Adv_BloodbathThreshold = new("MNK_AoE_Adv_BloodbathThreshold"),
                MNK_OpenerChoice = new("MNK_OpenerChoice"),
                MNK_VariantCure = new("MNK_VariantCure");
        }

        internal class MNK_ST_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_BasicCombo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Bootshine)
                {
                    // Monk Rotation
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
                        if (!LevelChecked(TrueStrike))
                            return Bootshine;
                        return !LevelChecked(TwinSnakes) ||
                            GetBuffRemainingTime(Buffs.DisciplinedFist) >= Config.MNK_DisciplinedFist_Apply
                            ? TrueStrike
                            : TwinSnakes;
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                    {
                        return !LevelChecked(SnapPunch)
                            ? Bootshine
                            : !LevelChecked(Demolish) ||
                            (GetDebuffRemainingTime(Debuffs.Demolish) >= Config.MNK_Demolish_Apply) ||
                            (GetTargetHPPercent() < Config.MNK_DemolishTreshhold)
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
            internal static MNKOpenerLogic MNKOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MNKGauge? gauge = GetJobGauge<MNKGauge>();
                Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                bool solarNadi = gauge.Nadi == Nadi.SOLAR;
                float demolishTreshold = Config.MNK_DemolishTreshhold;

                if (actionID is DragonKick)
                {
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;

                    // Opener for MNK
                    if (MNKOpener.DoFullOpener(ref actionID, true))
                        return actionID;

                    // Out of combat preparation
                    if (IsEnabled(CustomComboPreset.MNK_ST_Simple_Thunderclap) &&
                        !InMeleeRange() &&
                        gauge.Chakra == 5 &&
                        (!LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        return Thunderclap;

                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // Buffs
                    if (CanWeave(actionID, 0.5))
                    {
                        if (!HasEffect(Buffs.FormlessFist) &&
                            LevelChecked(PerfectBalance) &&
                            !HasEffect(Buffs.PerfectBalance) &&
                            HasEffect(Buffs.DisciplinedFist) &&
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
                                return PerfectBalance;
                        }
                    }

                    if (CanDelayedWeave(actionID, 1.25, 0.5))
                    {

                        if (ActionReady(RiddleOfFire) &&
                            HasEffect(Buffs.DisciplinedFist) &&
                            InMeleeRange())
                            return RiddleOfFire;

                        if (TargetNeedsPositionals() &&
                        ActionReady(All.TrueNorth) &&
                        !HasEffect(All.Buffs.TrueNorth) &&
                        LevelChecked(Demolish) &&
                        HasEffect(Buffs.CoerlForm))
                        {
                            if (!TargetHasEffect(Debuffs.Demolish) || GetBuffRemainingTime(Debuffs.Demolish) <= 6)
                            {
                                if (!OnTargetsRear())
                                    return All.TrueNorth;
                            }
                            else if (!OnTargetsFlank())
                                return All.TrueNorth;
                        }
                    }

                    if (CanWeave(actionID, 0.5))
                    {
                        if (ActionReady(Brotherhood) &&
                            IsOnCooldown(RiddleOfFire))
                            return Brotherhood;

                        if (ActionReady(RiddleOfWind) &&
                            IsOnCooldown(RiddleOfFire) &&
                            IsOnCooldown(Brotherhood))
                            return RiddleOfWind;

                        if (LevelChecked(Meditation) &&
                            gauge.Chakra == 5 &&
                            (HasEffect(Buffs.DisciplinedFist) || !LevelChecked(TwinSnakes)))
                        {

                            if (!LevelChecked(RiddleOfFire) || (GetCooldownRemainingTime(RiddleOfFire) >= 1.5 &&
                                IsOnCooldown(RiddleOfFire) && lastComboMove != RiddleOfFire))
                                return OriginalHook(Meditation);
                        }
                    }


                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance) &&
                        (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) &&
                        ((!IsMoving && GetTargetDistance() < 4.5f) || (IsMoving && GetTargetDistance() < 4)))
                        return OriginalHook(MasterfulBlitz);

                    // Meditation Uptime
                    if (!InMeleeRange() && gauge.Chakra < 5 && LevelChecked(Meditation))
                        return Meditation;

                    // Masterful Blitz
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
                        return OriginalHook(MasterfulBlitz);

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
                                return TwinSnakes;

                            if (raptorChakra)
                                return Demolish;

                            if (lunarNadi && !solarNadi)
                            {
                                bool demolishFirst = !TargetHasEffect(Debuffs.Demolish);
                                if (!demolishFirst && HasEffect(Buffs.DisciplinedFist))
                                {
                                    demolishFirst = GetBuffRemainingTime(Buffs.DisciplinedFist) >= GetDebuffRemainingTime(Debuffs.Demolish);
                                }
                                return demolishFirst
                                    ? Demolish
                                    : TwinSnakes;
                            }
                        }
                        if (canSolar && (lunarNadi || !solarNadi))
                        {
                            if (!raptorChakra && (!HasEffect(Buffs.DisciplinedFist) || GetBuffRemainingTime(Buffs.DisciplinedFist) <= 2.5))
                                return TwinSnakes;

                            if (!coeurlChakra && (!TargetHasEffect(Debuffs.Demolish) || GetDebuffRemainingTime(Debuffs.Demolish) <= 2.5))
                                return Demolish;
                        }
                        return HasEffect(Buffs.LeadenFist) && HasEffect(Buffs.OpoOpoForm)
                            ? Bootshine
                            : DragonKick;
                    }
                }

                // Monk Rotation
                if (!HasEffect(Buffs.PerfectBalance))
                {
                    if (HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.OpoOpoForm))
                    {
                        return !LevelChecked(DragonKick) || HasEffect(Buffs.LeadenFist)
                            ? MNK.Bootshine
                            : MNK.DragonKick;
                    }
                }

                if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.RaptorForm))
                {
                    if (!LevelChecked(TrueStrike))
                    {
                        return Bootshine;
                    }

                    return !LevelChecked(TwinSnakes) || GetBuffRemainingTime(Buffs.DisciplinedFist) >= 6
                        ? TrueStrike
                        : TwinSnakes;
                }
                if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                {
                    return !LevelChecked(SnapPunch)
                        ? Bootshine
                        : !LevelChecked(Demolish) || (GetDebuffRemainingTime(Debuffs.Demolish) >= 6) || (GetTargetHPPercent() < demolishTreshold)
                            ? SnapPunch
                            : Demolish;
                }
                return actionID;
            }
        }

        internal class MNK_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_AdvancedMode;
            internal static MNKOpenerLogic MNKOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MNKGauge? gauge = GetJobGauge<MNKGauge>();
                Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                bool solarNadi = gauge.Nadi == Nadi.SOLAR;
                bool opoopoChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.OPOOPO);
                bool coeurlChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.COEURL);
                bool raptorChakra = Array.Exists(gauge.BeastChakra, e => e == BeastChakra.RAPTOR);
                bool canSolar = gauge.BeastChakra.Where(e => e == BeastChakra.OPOOPO).Count() != 2;
                bool demolishFirst = !TargetHasEffect(Debuffs.Demolish);

                if (actionID is DragonKick)
                {
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    // Out of combat preparation
                    if (IsEnabled(CustomComboPreset.MNK_ST_ADV_Thunderclap) && !InCombat() &&
                        !InMeleeRange() && gauge.Chakra == 5 &&
                        (!LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                        return Thunderclap;

                    // Opener for MNK
                    if (IsEnabled(CustomComboPreset.MNK_ST_Adv_Opener))
                    {
                        if (MNKOpener.DoFullOpener(ref actionID, false))
                            return actionID;
                    }

                    // Buffs
                    if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs))
                    {
                        if (CanWeave(actionID, 0.5))
                        {
                            if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_PerfectBalance) &&
                                !HasEffect(Buffs.FormlessFist) && LevelChecked(PerfectBalance) &&
                                !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.DisciplinedFist) &&
                                OriginalHook(MasterfulBlitz) == MasterfulBlitz)
                            {
                                // Use Perfect Balance if:
                                // 1. It's after Bootshine/Dragon Kick.
                                // 2. At max stacks / before overcap.
                                // 3. During Brotherhood & RoF.
                                // 4. During Riddle of Fire after Demolish has been applied.
                                // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.   
                                if ((WasLastAction(Bootshine) || WasLastAction(DragonKick)) &&
                                    ((GetRemainingCharges(PerfectBalance) == 2) ||
                                    //(GetRemainingCharges(PerfectBalance) == 1 && GetCooldownChargeRemainingTime(PerfectBalance) < 4) ||
                                    (HasCharges(PerfectBalance) && HasEffect(Buffs.Brotherhood) && HasEffect(Buffs.RiddleOfFire)) ||
                                    // (HasCharges(PerfectBalance) && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) > 40) ||
                                    (HasCharges(PerfectBalance) && HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) > 6) ||
                                    (HasCharges(PerfectBalance) && GetCooldownRemainingTime(RiddleOfFire) < 3 && GetCooldownRemainingTime(Brotherhood) < 10)))
                                    return PerfectBalance;
                            }
                        }

                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_RiddleOfFire) &&
                                ActionReady(RiddleOfFire) && HasEffect(Buffs.DisciplinedFist) && InMeleeRange())
                                return RiddleOfFire;

                            if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_Brotherhood) &&
                                ActionReady(Brotherhood) && IsOnCooldown(RiddleOfFire))
                                return Brotherhood;

                            if (IsEnabled(CustomComboPreset.MNK_ST_ADV_CDs_RiddleOfWind) &&
                               ActionReady(RiddleOfWind) && IsOnCooldown(RiddleOfFire) && IsOnCooldown(Brotherhood))
                                return RiddleOfWind;

                            if (IsEnabled(CustomComboPreset.MNK_ST_AdV_TrueNorthDynamic) &&
                                TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) &&
                                LevelChecked(Demolish) && HasEffect(Buffs.CoerlForm))
                            {
                                if (!TargetHasEffect(Debuffs.Demolish) || (GetDebuffRemainingTime(Debuffs.Demolish) <= Config.MNK_Demolish_Apply))
                                {
                                    if (!OnTargetsRear())
                                        return All.TrueNorth;
                                }
                                else if (!OnTargetsFlank())
                                    return All.TrueNorth;
                            }
                        }
                    }

                    //Meditation
                    if (IsEnabled(CustomComboPreset.MNK_ST_Meditation)
                         && LevelChecked(Meditation) && gauge.Chakra == 5 &&
                        (HasEffect(Buffs.DisciplinedFist) || !LevelChecked(TwinSnakes)))
                    {
                        //Meditation spender
                        if (!LevelChecked(RiddleOfFire) ||
                            (GetCooldownRemainingTime(RiddleOfFire) >= 1.5 && IsOnCooldown(RiddleOfFire) && !WasLastAction(RiddleOfFire) && CanWeave(actionID)))
                            return OriginalHook(Meditation);

                        // Meditation Uptime
                        if (IsEnabled(CustomComboPreset.MNK_ST_Meditation_Uptime) &&
                            !InMeleeRange() && gauge.Chakra < 5)
                            return Meditation;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_ST_Adv_MasterfulBlitz) &&
                        LevelChecked(MasterfulBlitz))
                    {
                        // Masterful Blitz ElixirField/RisingPhoenix
                        if ((OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) &&
                            ((!IsMoving && GetTargetDistance() < 4.5f) || (IsMoving && GetTargetDistance() < 4)))
                            return OriginalHook(MasterfulBlitz);

                        // Masterful Blitz
                        if (OriginalHook(MasterfulBlitz) != MasterfulBlitz &&
                            !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
                            return OriginalHook(MasterfulBlitz);

                        // Beast chackra's
                        if (HasEffect(Buffs.PerfectBalance))
                        {
                            if (opoopoChakra)
                            {
                                if (coeurlChakra)
                                    return TwinSnakes;

                                if (raptorChakra)
                                    return Demolish;

                                if (lunarNadi && !solarNadi)
                                {
                                    if (!demolishFirst && HasEffect(Buffs.DisciplinedFist))
                                    {
                                        demolishFirst = GetBuffRemainingTime(Buffs.DisciplinedFist) >= GetDebuffRemainingTime(Debuffs.Demolish);
                                    }
                                    return demolishFirst
                                        ? Demolish
                                        : TwinSnakes;
                                }
                            }

                            if (canSolar && (lunarNadi || !solarNadi))
                            {
                                if (!raptorChakra && (!HasEffect(Buffs.DisciplinedFist) || GetBuffRemainingTime(Buffs.DisciplinedFist) <= 2.5))
                                    return TwinSnakes;

                                if (!coeurlChakra && (!TargetHasEffect(Debuffs.Demolish) || GetDebuffRemainingTime(Debuffs.Demolish) <= 2.5))
                                    return Demolish;
                            }

                            return HasEffect(Buffs.LeadenFist)
                                ? Bootshine
                                : DragonKick;
                        }
                    }

                    //force twinsnakes for looping reasons
                    if (WasLastAction(ElixirField))
                        return TwinSnakes;

                    // Healing
                    if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.MNK_ST_Adv_SecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.MNK_ST_Adv_BloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }




                    // Monk Rotation
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
                        if (!LevelChecked(TrueStrike))
                            return Bootshine;
                        return !LevelChecked(TwinSnakes) ||
                            GetBuffRemainingTime(Buffs.DisciplinedFist) >= Config.MNK_DisciplinedFist_Apply
                            ? TrueStrike
                            : TwinSnakes;
                    }

                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                    {
                        return !LevelChecked(SnapPunch)
                            ? Bootshine
                            : !LevelChecked(Demolish) ||
                            (GetDebuffRemainingTime(Debuffs.Demolish) >= Config.MNK_Demolish_Apply) ||
                            (GetTargetHPPercent() < Config.MNK_DemolishTreshhold)
                                ? SnapPunch
                                : Demolish;
                    }
                }
                return actionID;
            }
        }

        internal class MNK_AOE_BasicCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_BasicCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArmOfTheDestroyer)
                {
                    if (HasEffect(Buffs.OpoOpoForm))
                        return OriginalHook(ArmOfTheDestroyer);

                    if (HasEffect(Buffs.RaptorForm) || HasEffect(Buffs.FormlessFist))
                        return LevelChecked(FourPointFury)
                            ? FourPointFury
                            : TwinSnakes;

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                        return Rockbreaker;
                }
                return actionID;
            }
        }

        internal class MNK_AOE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MNKGauge? gauge = GetJobGauge<MNKGauge>();
                Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                bool nadiNONE = gauge.Nadi == Nadi.NONE;

                if (actionID is ArmOfTheDestroyer)
                {
                    if (!InCombat())
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

                    if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;


                    // Buffs
                    if (InCombat() && CanWeave(actionID, 0.5))
                    {
                        if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                           IsEnabled(Variant.VariantRampart) &&
                           IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (LevelChecked(RiddleOfFire) && !IsOnCooldown(RiddleOfFire) && InMeleeRange())
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

                        if (ActionReady(Brotherhood))
                        {
                            return Brotherhood;
                        }

                        if (ActionReady(RiddleOfWind))
                        {
                            return RiddleOfWind;
                        }

                        if (LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist) ||
                            !LevelChecked(TwinSnakes)) && CanWeave(actionID))
                        {
                            return LevelChecked(Enlightenment)
                                ? OriginalHook(Enlightenment)
                                : OriginalHook(Meditation);
                        }
                    }


                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && (OriginalHook(MasterfulBlitz) == ElixirField
                        || OriginalHook(MasterfulBlitz) == RisingPhoenix) && ((!IsMoving && GetTargetDistance() < 4.5f)
                        || (IsMoving && GetTargetDistance() < 4)))
                    {
                        return OriginalHook(MasterfulBlitz);
                    }

                    // Masterful Blitz
                    if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz
                        && !(OriginalHook(MasterfulBlitz) == ElixirField
                        || OriginalHook(MasterfulBlitz) == RisingPhoenix))
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
                                return LevelChecked(ShadowOfTheDestroyer)
                                    ? ShadowOfTheDestroyer
                                    : Rockbreaker;
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
                        return OriginalHook(ArmOfTheDestroyer);

                    if (HasEffect(Buffs.RaptorForm) || HasEffect(Buffs.FormlessFist))
                        return LevelChecked(FourPointFury)
                            ? FourPointFury
                            : TwinSnakes;

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                        return Rockbreaker;
                }
                return actionID;
            }
        }

        internal class MNK_AOE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MNKGauge? gauge = GetJobGauge<MNKGauge>();
                Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                bool nadiNONE = gauge.Nadi == Nadi.NONE;

                if (actionID is ArmOfTheDestroyer)
                {
                    if (!InCombat())
                    {
                        if (IsEnabled(CustomComboPreset.MNK_AoE_Meditation) &&
                            gauge.Chakra < 5 && LevelChecked(Meditation))
                            return Meditation;

                        if (IsEnabled(CustomComboPreset.MNK_AoE_FormlessFist) &&
                            LevelChecked(FormShift) && !HasEffect(Buffs.FormlessFist) && comboTime <= 0)
                            return FormShift;

                        if (IsEnabled(CustomComboPreset.MNK_AoE_Thunderclap) && !InMeleeRange() && gauge.Chakra == 5
                            && (LevelChecked(FormShift) || HasEffect(Buffs.FormlessFist)))
                            return Thunderclap;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;

                    // Buffs
                    if (InCombat() && CanWeave(actionID, 0.5))
                    {
                        if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (IsEnabled(CustomComboPreset.MNK_AoE_ADV_CDs))
                        {
                            if (LevelChecked(RiddleOfFire) && ActionReady(RiddleOfFire) && InMeleeRange() &&
                                IsEnabled(CustomComboPreset.MNK_AoE_CDs_RiddleOfFire))
                                return RiddleOfFire;

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
                                    return PerfectBalance;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_AoE_CDs_Brotherhood) && ActionReady(Brotherhood))
                                return Brotherhood;

                            if (IsEnabled(CustomComboPreset.MNK_AoE_CDs_RiddleOfWind) && ActionReady(RiddleOfWind))
                                return RiddleOfWind;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_AoE_Meditation) && LevelChecked(Meditation) && gauge.Chakra == 5 && (HasEffect(Buffs.DisciplinedFist)
                            || !LevelChecked(TwinSnakes)) && CanWeave(actionID))
                            return LevelChecked(Enlightenment)
                                ? OriginalHook(Enlightenment)
                                : OriginalHook(Meditation);
                    }

                    // healing - please move if not appropriate this high priority
                    if (IsEnabled(CustomComboPreset.MNK_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.MNK_AoE_Adv_SecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= Config.MNK_AoE_Adv_BloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    // Masterful Blitz ElixirField/RisingPhoenix
                    if (IsEnabled(CustomComboPreset.MNK_AoE_MasterfulBlitz) && LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance) &&
                        (OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix) &&
                        ((!IsMoving && GetTargetDistance() < 4.5f) || (IsMoving && GetTargetDistance() < 4)))
                        return OriginalHook(MasterfulBlitz);

                    // Masterful Blitz
                    if (IsEnabled(CustomComboPreset.MNK_AoE_MasterfulBlitz) && LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz
                        && !(OriginalHook(MasterfulBlitz) == ElixirField || OriginalHook(MasterfulBlitz) == RisingPhoenix))
                        return OriginalHook(MasterfulBlitz);

                    // Perfect Balance
                    if (HasEffect(Buffs.PerfectBalance))
                    {
                        if (nadiNONE || !lunarNadi)
                        {
                            if (pbStacks?.StackCount > 0)
                            {
                                return LevelChecked(ShadowOfTheDestroyer)
                                    ? ShadowOfTheDestroyer
                                    : Rockbreaker;
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
                        return OriginalHook(ArmOfTheDestroyer);

                    if (HasEffect(Buffs.RaptorForm) || HasEffect(Buffs.FormlessFist))
                        return LevelChecked(FourPointFury)
                            ? FourPointFury
                            : TwinSnakes;

                    if (HasEffect(Buffs.CoerlForm) && LevelChecked(Rockbreaker))
                        return Rockbreaker;
                }
                return actionID;
            }
        }

        internal class MNK_DragonKick_Bootshine : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_DragonKick_Bootshine;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DragonKick)
                {
                    if (IsEnabled(CustomComboPreset.MNK_BootshineBalance)
                        && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                        return OriginalHook(MasterfulBlitz);

                    if (HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.OpoOpoForm))
                        return !LevelChecked(DragonKick) || HasEffect(Buffs.LeadenFist)
                            ? MNK.Bootshine
                            : MNK.DragonKick;
                }
                return actionID;
            }
        }

        internal class MNK_TrueStrike_TwinSnakes : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_TrueStrike_TwinSnakes;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is TrueStrike)
                {
                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.RaptorForm))
                        return !LevelChecked(TwinSnakes) || GetBuffRemainingTime(Buffs.DisciplinedFist) >= 6
                            ? TrueStrike
                            : TwinSnakes;
                }
                return actionID;
            }
        }

        internal class MNK_SnapPunch_Demolish : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_SnapPunch_Demolish;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SnapPunch)
                {
                    if (!HasEffect(Buffs.FormlessFist) && HasEffect(Buffs.CoerlForm))
                        return !LevelChecked(Demolish) || GetDebuffRemainingTime(Debuffs.Demolish) >= 6
                            ? SnapPunch
                            : Demolish;
                }
                return actionID;
            }
        }

        internal class MNK_PerfectBalance : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_PerfectBalance;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is PerfectBalance && OriginalHook(MasterfulBlitz) != MasterfulBlitz && LevelChecked(MasterfulBlitz)
                ? OriginalHook(MasterfulBlitz)
                : actionID;
        }

        internal class MNK_PerfectBalance_Plus : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_PerfectBalance_Plus;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                MNKGauge? gauge = GetJobGauge<MNKGauge>();
                Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                bool nadiNONE = gauge.Nadi == Nadi.NONE;

                if (actionID is MasterfulBlitz)
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

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is RiddleOfFire && IsOnCooldown(RiddleOfFire) && ActionReady(Brotherhood)
                ? Brotherhood
                : actionID;
        }

        internal class MNK_HowlingFistMeditation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_HowlingFistMeditation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                 actionID is HowlingFist or Enlightenment && GetJobGauge<MNKGauge>().Chakra < 5
                ? Meditation
                : actionID;
        }
    }
}