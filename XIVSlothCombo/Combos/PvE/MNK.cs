using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
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
            RisingPhoenix = 25768,
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
                CoeurlForm = 109,
                PerfectBalance = 110,
                RiddleOfFire = 1181,
                RiddleOfWind = 2687,
                FormlessFist = 2513,
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
            public static UserInt
                MNK_ST_SecondWind_Threshold = new("MNK_ST_SecondWindThreshold", 25),
                MNK_ST_Bloodbath_Threshold = new("MNK_ST_BloodbathThreshold", 40),
                MNK_AoE_SecondWind_Threshold = new("MNK_AoE_SecondWindThreshold", 25),
                MNK_AoE_Bloodbath_Threshold = new("MNK_AoE_BloodbathThreshold", 40),
                MNK_SelectedOpener = new("MNK_SelectedOpener");
        }

        internal class MNK_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_AdvancedMode;
            internal static MNKOpenerLogic MNKOpener = new();
            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                bool canWeave = CanWeave(actionID, 0.5);
                bool inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                bool bothNadisOpen = Gauge.Nadi.ToString() == "LUNAR, SOLAR";

                if (actionID is Bootshine or LeapingOpo)
                {
                    if (IsEnabled(CustomComboPreset.MNK_STUseOpener))
                    {
                        if (MNKOpener.DoFullOpener(ref actionID, Config.MNK_SelectedOpener))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_STUseMeditation)
                        && (!inCombat || !InMeleeRange())
                        && Gauge.Chakra < 5
                        && !HasEffect(Buffs.RiddleOfFire)
                        && LevelChecked(Meditation))
                    {
                        return OriginalHook(Meditation);
                    }

                    // OGCDs
                    if (inCombat && canWeave)
                    {
                        if (level >= Levels.PerfectBalance && !HasEffect(Buffs.PerfectBalance) && HasCharges(PerfectBalance) && IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance))
                        {
                            if ((WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick))
                                && GetCooldownRemainingTime(RiddleOfFire) < 7
                                && GetCooldownRemainingTime(Brotherhood) < 7)
                            {
                                return PerfectBalance;
                            }
                            else if ((WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick)) 
                                && HasEffect(Buffs.RiddleOfFire) 
                                && GetBuffRemainingTime(Buffs.RiddleOfFire) > 8 
                                && !HasEffect(Buffs.FiresRumination))
                            {
                                return PerfectBalance;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                        {
                            if (level >= Levels.Brotherhood
                                && !IsOnCooldown(Brotherhood))
                            {
                                return Brotherhood;
                            }

                            if (level >= Levels.RiddleOfFire
                                && !IsOnCooldown(RiddleOfFire))
                            {
                                return RiddleOfFire;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_STUseROW)
                                && level >= Levels.RiddleOfWind
                                && !IsOnCooldown(RiddleOfWind))
                            {
                                return RiddleOfWind;
                            }
                        }

                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_ST_SecondWind_Threshold) && IsEnabled(CustomComboPreset.MNK_ST_ComboHeals) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_ST_Bloodbath_Threshold) && IsEnabled(CustomComboPreset.MNK_ST_ComboHeals) && LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;

                        if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra)
                            && Gauge.Chakra >= 5
                            && level >= Levels.SteelPeak)
                        {
                            return OriginalHook(Meditation);
                        }
                    }

                    // GCDs
                    if (inCombat)
                    {
                        // Ensure usage if buff is almost depleted.
                        if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply)
                            && HasEffect(Buffs.FiresRumination)
                            && GetBuffRemainingTime(Buffs.FiresRumination) < 4)
                        {
                            return FiresReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply)
                            && HasEffect(Buffs.WindsRumination)
                            && GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                        {
                            return WindsReply;
                        }

                        if (HasEffect(Buffs.FormlessFist))
                        {
                            return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance))
                        {
                            // Masterful Blitz
                            if (level >= Levels.MasterfulBlitz && !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.RiddleOfFire) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                            {
                                return OriginalHook(MasterfulBlitz);
                            }

                            // Perfect Balance
                            if (HasEffect(Buffs.PerfectBalance))
                            {
                                bool solarNadi = Gauge.Nadi == Nadi.SOLAR;
                                bool lunarNadi = Gauge.Nadi == Nadi.LUNAR;
                                int opoOpoChakra = Gauge.BeastChakra.Where(x => x == BeastChakra.OPOOPO).Count();
                                int raptorChakra = Gauge.BeastChakra.Where(x => x == BeastChakra.RAPTOR).Count();
                                int coeurlChakra = Gauge.BeastChakra.Where(x => x == BeastChakra.COEURL).Count();

                                #region Open Solar
                                if (!solarNadi)
                                {
                                    if (coeurlChakra == 0)
                                    {
                                        return Gauge.CoeurlFury == 0 ? OriginalHook(Demolish) : OriginalHook(SnapPunch);
                                    }
                                    else if (raptorChakra == 0)
                                    {
                                        return Gauge.RaptorFury == 0 ? OriginalHook(TwinSnakes) : OriginalHook(TrueStrike);
                                    }
                                    else if (opoOpoChakra == 0)
                                    {
                                        return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                                    }
                                }
                                #endregion
                                #region Open Lunar
                                if (solarNadi || lunarNadi)
                                {
                                    return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                                }
                                #endregion
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply)
                            && HasEffect(Buffs.WindsRumination)
                            && level >= Levels.WindsReply)
                        {
                            return WindsReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply)
                            && HasEffect(Buffs.FiresRumination)
                            && !HasEffect(Buffs.PerfectBalance)
                            && !HasEffect(Buffs.FormlessFist)
                            && (WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick))
                            && level >= Levels.FiresReply)
                        {
                            return FiresReply;
                        }

                        // Standard Balls
                        return DetermineCoreAbility(actionID);
                    }
                }

                return actionID;
            }
        }

        internal class MNK_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_BasicMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArmOfTheDestroyer or ShadowOfTheDestroyer)
                {
                    bool inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    MNKGauge gauge = GetJobGauge<MNKGauge>();
                    bool canWeave = CanWeave(actionID, 0.5);
                    _ = CanWeave(actionID);
                    Dalamud.Game.ClientState.Statuses.Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
                    bool lunarNadi = gauge.Nadi == Nadi.LUNAR;
                    bool nadiNONE = gauge.Nadi == Nadi.NONE;

                    if (!inCombat)
                    {
                        if (gauge.Chakra < 5 && level >= Levels.Meditation)
                        {
                            return OriginalHook(Meditation); ;
                        }
                    }

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

                        if (Gauge.Chakra >= 5
                            && level >= Levels.HowlingFist
                            && HasBattleTarget())
                        {
                            return OriginalHook(EnlightenedMeditation);
                        }

                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_AoE_SecondWind_Threshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MNK_AoE_Bloodbath_Threshold) && LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath))
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

                        if (HasEffect(Buffs.CoeurlForm) && level >= Levels.Rockbreaker)
                        {
                            return Rockbreaker;
                        }
                    }
                }
                return actionID;
            }
        }

        #region Ball Handlers
        internal class MNK_BallHandler_OpoOpo : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BallHandlers;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BALLS_OPOOPO))
                {
                    if (actionID is Bootshine or LeapingOpo)
                    {
                        if (HasEffect(Buffs.OpoOpoForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance))
                        {
                            if (Gauge.OpoOpoFury == 0)
                            {
                                if (LevelChecked(DragonKick))
                                    return DragonKick;
                            }
                            else
                            {
                                return OriginalHook(Bootshine);
                            }
                        }
                    }
                }

                return actionID;
            }
        }

        internal class MNK_BallHandler_Raptor : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BallHandlers;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BALLS_RAPTOR))
                {
                    if (actionID is TrueStrike or RisingRaptor)
                    {
                        if (HasEffect(Buffs.RaptorForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance))
                        {
                            if (Gauge.RaptorFury == 0)
                            {
                                if (LevelChecked(TwinSnakes))
                                    return TwinSnakes;
                            }
                            else
                            {
                                return OriginalHook(TrueStrike);
                            }
                        }
                    }
                }

                return actionID;
            }
        }

        internal class MNK_BallHandler_Coeurl : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BallHandlers;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BALLS_COEURL))
                {
                    if (actionID is SnapPunch or PouncingCoeurl)
                    {
                        if (HasEffect(Buffs.CoeurlForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance))
                        {
                            if (Gauge.CoeurlFury == 0)
                            {
                                if (LevelChecked(Demolish))
                                    return Demolish;
                            }
                            else
                            {
                                return OriginalHook(SnapPunch);
                            }
                        }
                    }
                }

                return actionID;
            }
        }
        #endregion

        internal class MNK_PerfectBalance : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_PerfectBalance;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == PerfectBalance)
                {
                    if (OriginalHook(MasterfulBlitz) != MasterfulBlitz && level >= Levels.MasterfulBlitz)
                        return OriginalHook(MasterfulBlitz);
                }

                return actionID;
            }
        }

        internal class MNK_Riddle_Brotherhood : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_Riddle_Brotherhood;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is RiddleOfFire && level >= Levels.Brotherhood && IsOnCooldown(RiddleOfFire) && IsOffCooldown(Brotherhood)
                    ? Brotherhood
                    : actionID;
            }
        }

        public static uint DetermineCoreAbility(uint actionId, bool useTrueNorthIfEnabled = true)
        {
            if (CustomComboFunctions.HasEffect(Buffs.OpoOpoForm) || CustomComboFunctions.HasEffect(Buffs.FormlessFist))
            {
                if (Gauge.OpoOpoFury == 0)
                {
                    if (CustomComboFunctions.LevelChecked(DragonKick))
                        return DragonKick;
                }
                else
                {
                    return CustomComboFunctions.OriginalHook(Bootshine);
                }
            }

            if (CustomComboFunctions.HasEffect(Buffs.RaptorForm))
            {
                if (Gauge.RaptorFury == 0)
                {
                    if (CustomComboFunctions.LevelChecked(TwinSnakes))
                        return TwinSnakes;
                }
                else
                {
                    if (CustomComboFunctions.LevelChecked(TrueStrike))
                        return CustomComboFunctions.OriginalHook(TrueStrike);
                }
            }

            if (CustomComboFunctions.HasEffect(Buffs.CoeurlForm))
            {
                if (Gauge.CoeurlFury == 0)
                {
                    if (!CustomComboFunctions.OnTargetsRear()
                        && CustomComboFunctions.IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                        && CustomComboFunctions.TargetNeedsPositionals()
                        && !CustomComboFunctions.HasEffect(Buffs.TrueNorth)
                        && CustomComboFunctions.LevelChecked(TrueNorth)
                        && CustomComboFunctions.HasCharges(TrueNorth)
                        && useTrueNorthIfEnabled)
                    {
                        return TrueNorth;
                    }
                    else
                    {
                        if (CustomComboFunctions.LevelChecked(Demolish))
                            return Demolish;
                    }
                }

                if (!CustomComboFunctions.OnTargetsFlank()
                        && CustomComboFunctions.IsEnabled(CustomComboPreset.MNK_STUseTrueNorth)
                        && CustomComboFunctions.TargetNeedsPositionals()
                        && !CustomComboFunctions.HasEffect(Buffs.TrueNorth)
                        && CustomComboFunctions.LevelChecked(TrueNorth)
                        && CustomComboFunctions.HasCharges(TrueNorth)
                        && useTrueNorthIfEnabled)
                {
                    return TrueNorth;
                }
                else
                {
                    if (CustomComboFunctions.LevelChecked(SnapPunch))
                        return CustomComboFunctions.OriginalHook(SnapPunch);
                }
            }

            return actionId;
        }
    }
}