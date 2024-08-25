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
            SteelPeak = 25761,
            TwinSnakes = 61,
            ArmOfTheDestroyer = 62,
            Demolish = 66,
            Mantra = 65,
            DragonKick = 74,
            Rockbreaker = 70,
            Thunderclap = 25762,
            HowlingFist = 25763,
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

        public static MNKGauge Gauge => CustomComboFunctions.GetJobGauge<MNKGauge>();

        public static class Config
        {
            public static UserInt
                MNK_ST_SecondWind_Threshold = new("MNK_ST_SecondWindThreshold", 25),
                MNK_ST_Bloodbath_Threshold = new("MNK_ST_BloodbathThreshold", 40),
                MNK_SelectedOpener = new("MNK_SelectedOpener"),
                MNK_VariantCure = new("MNK_Variant_Cure");
        }

        internal class MNK_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_SimpleMode;
            internal static MNKOpenerLogic MNKOpener = new();
            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                bool canWeave = CanWeave(actionID, 0.5);
                bool inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                bool bothNadisOpen = Gauge.Nadi.ToString() == "LUNAR, SOLAR";

                if (actionID is Bootshine or LeapingOpo)
                {
                    if (MNKOpener.DoFullOpener(ref actionID, Config.MNK_SelectedOpener))
                        return actionID;


                    if (IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                        return Variant.VariantCure;

                    if ((!inCombat || !InMeleeRange())
                        && Gauge.Chakra < 5
                        && !HasEffect(Buffs.RiddleOfFire)
                        && LevelChecked(Meditation))
                    {
                        return OriginalHook(Meditation);
                    }

                    // OGCDs
                    if (canWeave)
                    {
                        if (IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (ActionReady(PerfectBalance) && !HasEffect(Buffs.PerfectBalance))
                        {
                            if ((WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick))
                                && (GetCooldownRemainingTime(RiddleOfFire) < 8
                                && GetCooldownRemainingTime(Brotherhood) < 7) ||
                                (GetBuffRemainingTime(Buffs.RiddleOfFire) >= 8
                                && !HasEffect(Buffs.FiresRumination)))
                            {
                                return PerfectBalance;
                            }
                        }

                        if (ActionReady(Brotherhood))
                        {
                            return Brotherhood;
                        }

                        if (ActionReady(RiddleOfFire))
                        {
                            return RiddleOfFire;
                        }

                        if (ActionReady(RiddleOfWind))
                        {
                            return RiddleOfWind;
                        }

                        if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                            return All.Bloodbath;

                        if (Gauge.Chakra >= 5
                            && SteelPeak.LevelChecked())
                        {
                            return OriginalHook(SteelPeak);
                        }
                    }

                    // GCDs

                    // Ensure usage if buff is almost depleted.
                    if (HasEffect(Buffs.FiresRumination) && GetBuffRemainingTime(Buffs.FiresRumination) < 4)
                    {
                        return FiresReply;
                    }

                    if (HasEffect(Buffs.WindsRumination) && GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                    {
                        return WindsReply;
                    }

                    if (HasEffect(Buffs.FormlessFist))
                    {
                        return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                    }

                    // Masterful Blitz
                    if (MasterfulBlitz.LevelChecked() && !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.RiddleOfFire) && !IsOriginal(MasterfulBlitz))
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
                        if (!solarNadi && !bothNadisOpen)
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
                        if (solarNadi || lunarNadi || bothNadisOpen)
                        {
                            return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                        }
                        #endregion
                    }

                    if (HasEffect(Buffs.WindsRumination))
                    {
                        return WindsReply;
                    }

                    if (HasEffect(Buffs.FiresRumination)
                        && !HasEffect(Buffs.PerfectBalance)
                        && !HasEffect(Buffs.FormlessFist)
                        && (WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick)))
                    {
                        return FiresReply;
                    }

                    // Standard Beast Chakras
                    return MNKHelper.DetermineCoreAbility(actionID);

                }

                return actionID;
            }
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

                    if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MNK_VariantCure))
                        return Variant.VariantCure;

                    // OGCDs
                    if (inCombat && canWeave)
                    {
                        if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (PerfectBalance.LevelChecked() && !HasEffect(Buffs.PerfectBalance) && HasCharges(PerfectBalance) && IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance))
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
                            if (Brotherhood.LevelChecked()
                                && !IsOnCooldown(Brotherhood))
                            {
                                return Brotherhood;
                            }

                            if (RiddleOfFire.LevelChecked()
                                && !IsOnCooldown(RiddleOfFire))
                            {
                                return RiddleOfFire;
                            }

                            if (IsEnabled(CustomComboPreset.MNK_STUseROW)
                                && RiddleOfWind.LevelChecked()
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
                            && SteelPeak.LevelChecked())
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
                            if (MasterfulBlitz.LevelChecked() && !HasEffect(Buffs.PerfectBalance) && HasEffect(Buffs.RiddleOfFire) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
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
                                if (!solarNadi && !bothNadisOpen)
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
                                if (solarNadi || lunarNadi || bothNadisOpen)
                                {
                                    return Gauge.OpoOpoFury == 0 ? OriginalHook(DragonKick) : OriginalHook(Bootshine);
                                }
                                #endregion
                            }
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply)
                            && HasEffect(Buffs.WindsRumination)
                            && WindsReply.LevelChecked())
                        {
                            return WindsReply;
                        }

                        if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply)
                            && HasEffect(Buffs.FiresRumination)
                            && !HasEffect(Buffs.PerfectBalance)
                            && !HasEffect(Buffs.FormlessFist)
                            && (WasLastWeaponskill(LeapingOpo) || WasLastWeaponskill(DragonKick))
                            && FiresReply.LevelChecked())
                        {
                            return FiresReply;
                        }

                        // Standard Beast Chakras
                        return MNKHelper.DetermineCoreAbility(actionID, IsEnabled(CustomComboPreset.MNK_STUseTrueNorth));
                    }
                }

                return actionID;
            }
        }

        internal class MNK_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_SimpleMode;

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
                        if (gauge.Chakra < 5 && Meditation.LevelChecked())
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

                        if (ActionReady(RiddleOfFire))
                        {
                            return RiddleOfFire;
                        }

                        if (PerfectBalance.LevelChecked() && !HasEffect(Buffs.PerfectBalance) && IsOriginal(MasterfulBlitz))
                        {
                            // Use Perfect Balance if:
                            // 1. It's after Bootshine/Dragon Kick. - This doesn't apply to AoE
                            // 2. At max stacks / before overcap.
                            // 3. During Brotherhood.
                            // 4. During Riddle of Fire.
                            // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                            if (HasCharges(PerfectBalance) &&
                                (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance)) ||
                                (GetCooldownRemainingTime(PerfectBalance) <= 4) ||
                                (HasEffect(Buffs.Brotherhood)) ||
                                (HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                                (GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
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

                        if (Gauge.Chakra >= 5
                            && HowlingFist.LevelChecked()
                            && HasBattleTarget())
                        {
                            return OriginalHook(HowlingFist);
                        }

                        if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                            return All.SecondWind;
                        if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (inCombat)
                    {
                        if (HasEffect(Buffs.WindsRumination))
                        {
                            return WindsReply;
                        }

                        if (HasEffect(Buffs.FiresRumination))
                        {
                            return FiresReply;
                        }

                        // Masterful Blitz
                        if (MasterfulBlitz.LevelChecked() && !HasEffect(Buffs.PerfectBalance) && OriginalHook(MasterfulBlitz) != MasterfulBlitz)
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
                                    return ShadowOfTheDestroyer.LevelChecked() ? ShadowOfTheDestroyer : Rockbreaker;
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

                        if (HasEffect(Buffs.CoeurlForm) && Rockbreaker.LevelChecked())
                        {
                            return Rockbreaker;
                        }
                    }
                }
                return actionID;
            }
        }

        #region Beast Chakras
        internal class MNK_BeastChakra_OpoOpo : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BC_OPOOPO))
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

        internal class MNK_BeastChakra_Raptor : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BC_RAPTOR))
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

        internal class MNK_BeastChakra_Coeurl : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.MNK_BC_COEURL))
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
                    if (OriginalHook(MasterfulBlitz) != MasterfulBlitz && MasterfulBlitz.LevelChecked())
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
                return actionID is RiddleOfFire && Brotherhood.LevelChecked() && IsOnCooldown(RiddleOfFire) && IsOffCooldown(Brotherhood)
                    ? Brotherhood
                    : actionID;
            }
        }
    }
}