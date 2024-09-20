using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.PvE;

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

    protected static MNKGauge Gauge => GetJobGauge<MNKGauge>();

    protected static class Buffs
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

    public static class Config
    {
        public static UserInt
            MNK_ST_SecondWind_Threshold = new("MNK_ST_SecondWindThreshold", 25),
            MNK_ST_Bloodbath_Threshold = new("MNK_ST_BloodbathThreshold", 40),
            MNK_AoE_SecondWind_Threshold = new("MNK_AoE_SecondWindThreshold", 25),
            MNK_AoE_Bloodbath_Threshold = new("MNK_AoE_BloodbathThreshold", 40),
            MNK_SelectedOpener = new("MNK_SelectedOpener"),
            MNK_VariantCure = new("MNK_Variant_Cure");
    }

    internal class MNK_ST_SimpleMode : CustomCombo
    {
        internal static MNKOpenerLogic MNKOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            bool bothNadisOpen = Gauge.Nadi.ToString() == "LUNAR, SOLAR";
            bool solarNadi = Gauge.Nadi == Nadi.SOLAR;
            bool lunarNadi = Gauge.Nadi == Nadi.LUNAR;
            int opoOpoChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.OPOOPO);
            int raptorChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.RAPTOR);
            int coeurlChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.COEURL);

            if (actionID is Bootshine or LeapingOpo)
            {
                if (MNKOpener.DoFullOpener(ref actionID, 0))
                    return actionID;

                if ((!InCombat() || !InMeleeRange()) &&
                    Gauge.Chakra < 5 &&
                    !HasEffect(Buffs.RiddleOfFire) &&
                    LevelChecked(Meditation))
                    return OriginalHook(Meditation);

                if (!InCombat() && LevelChecked(FormShift) &&
                    !HasEffect(Buffs.FormlessFist))
                    return FormShift;

                //Variant Cure
                if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                    return Variant.VariantCure;

                // OGCDs
                if (CanWeave(ActionWatching.LastWeaponskill))
                {
                    //Variant Rampart
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (ActionReady(Brotherhood))
                        return Brotherhood;

                    if (ActionReady(RiddleOfFire) &&
                        CanDelayedWeave(ActionWatching.LastWeaponskill))
                        return RiddleOfFire;

                    if (ActionReady(RiddleOfWind))
                        return RiddleOfWind;

                    //Perfect Balance
                    if (ActionReady(PerfectBalance) &&
                        !HasEffect(Buffs.PerfectBalance))
                    {
                        // Odd window
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            !JustUsed(PerfectBalance, 20) &&
                            HasEffect(Buffs.RiddleOfFire) &&
                            !HasEffect(Buffs.Brotherhood))
                            return PerfectBalance;

                        // Even window
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            HasEffect(Buffs.Brotherhood) &&
                            HasEffect(Buffs.RiddleOfFire))
                            return PerfectBalance;

                        // Low level
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            ((HasEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood)) ||
                             !LevelChecked(RiddleOfFire)))
                            return PerfectBalance;
                    }

                    if (PlayerHealthPercentageHp() <= 25 &&
                        ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 &&
                        ActionReady(All.Bloodbath))
                        return All.Bloodbath;

                    if (Gauge.Chakra >= 5 &&
                        LevelChecked(SteelPeak))
                        return OriginalHook(Meditation);
                }

                // GCDs
                if (HasEffect(Buffs.FormlessFist))
                    return Gauge.OpoOpoFury == 0
                        ? DragonKick
                        : OriginalHook(Bootshine);

                // Masterful Blitz
                if (LevelChecked(MasterfulBlitz) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    !IsOriginal(MasterfulBlitz))
                    return OriginalHook(MasterfulBlitz);

                // Perfect Balance
                if (HasEffect(Buffs.PerfectBalance))
                {
                    #region Open Solar

                    if (!solarNadi && !bothNadisOpen)
                    {
                        if (coeurlChakra == 0)
                            return Gauge.CoeurlFury == 0
                                ? Demolish
                                : OriginalHook(SnapPunch);

                        if (raptorChakra == 0)
                            return Gauge.RaptorFury == 0
                                ? TwinSnakes
                                : OriginalHook(TrueStrike);

                        if (opoOpoChakra == 0)
                            return Gauge.OpoOpoFury == 0
                                ? DragonKick
                                : OriginalHook(Bootshine);
                    }

                    #endregion

                    #region Open Lunar

                    if (solarNadi || lunarNadi || bothNadisOpen)
                        return Gauge.OpoOpoFury == 0
                            ? DragonKick
                            : OriginalHook(Bootshine);

                    #endregion
                }

                if (HasEffect(Buffs.FiresRumination) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    !HasEffect(Buffs.FormlessFist) &&
                    (JustUsed(OriginalHook(Bootshine)) ||
                     JustUsed(DragonKick) ||
                     GetBuffRemainingTime(Buffs.FiresRumination) < 4))
                    return FiresReply;

                if (HasEffect(Buffs.WindsRumination) &&
                    LevelChecked(WindsReply) &&
                    (HasEffect(Buffs.RiddleOfFire) ||
                     GetBuffRemainingTime(Buffs.WindsRumination) < 4))
                    return WindsReply;

                // Standard Beast Chakras
                return MNKHelper.DetermineCoreAbility(actionID, true);
            }

            return actionID;
        }
    }

    internal class MNK_ST_AdvancedMode : CustomCombo
    {
        internal static MNKOpenerLogic MNKOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            bool bothNadisOpen = Gauge.Nadi.ToString() == "LUNAR, SOLAR";
            bool solarNadi = Gauge.Nadi == Nadi.SOLAR;
            bool lunarNadi = Gauge.Nadi == Nadi.LUNAR;
            int opoOpoChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.OPOOPO);
            int raptorChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.RAPTOR);
            int coeurlChakra = Gauge.BeastChakra.Count(x => x == BeastChakra.COEURL);

            if (actionID is Bootshine or LeapingOpo)
            {
                if (IsEnabled(CustomComboPreset.MNK_STUseOpener))
                    if (MNKOpener.DoFullOpener(ref actionID, Config.MNK_SelectedOpener))
                        return actionID;

                if (IsEnabled(CustomComboPreset.MNK_STUseMeditation) &&
                    (!InCombat() || !InMeleeRange()) &&
                    Gauge.Chakra < 5 &&
                    !HasEffect(Buffs.RiddleOfFire) &&
                    LevelChecked(Meditation))
                    return OriginalHook(Meditation);

                if (IsEnabled(CustomComboPreset.MNK_STUseFormShift) &&
                    !InCombat() && LevelChecked(FormShift) &&
                    !HasEffect(Buffs.FormlessFist))
                    return FormShift;

                //Variant Cure
                if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.MNK_STUseBuffs) &&
                    IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                    ActionReady(RiddleOfFire) &&
                    CanDelayedWeave(ActionWatching.LastWeaponskill))
                    return RiddleOfFire;

                // OGCDs
                if (CanWeave(ActionWatching.LastWeaponskill))
                {
                    //Variant Rampart
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                    {
                        if (IsEnabled(CustomComboPreset.MNK_STUseBrotherhood) &&
                            ActionReady(Brotherhood))
                            return Brotherhood;

                        if (IsEnabled(CustomComboPreset.MNK_STUseROW) &&
                            ActionReady(RiddleOfWind))
                            return RiddleOfWind;
                    }

                    //Perfect Balance
                    if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance) &&
                        ActionReady(PerfectBalance) &&
                        !HasEffect(Buffs.PerfectBalance))
                    {
                        // Odd window
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            !JustUsed(PerfectBalance, 20) &&
                            HasEffect(Buffs.RiddleOfFire) &&
                            !HasEffect(Buffs.Brotherhood))
                            return PerfectBalance;

                        // Even window
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            HasEffect(Buffs.Brotherhood) &&
                            HasEffect(Buffs.RiddleOfFire))
                            return PerfectBalance;

                        // Low level
                        if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                            ((HasEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood)) ||
                             !LevelChecked(RiddleOfFire)))
                            return PerfectBalance;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.MNK_ST_SecondWind_Threshold &&
                            ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.MNK_ST_Bloodbath_Threshold &&
                            ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra) &&
                        Gauge.Chakra >= 5 &&
                        LevelChecked(SteelPeak))
                        return OriginalHook(Meditation);
                }

                // GCDs
                if (HasEffect(Buffs.FormlessFist))
                    return Gauge.OpoOpoFury == 0
                        ? DragonKick
                        : OriginalHook(Bootshine);

                if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance))
                {
                    // Masterful Blitz
                    if (LevelChecked(MasterfulBlitz) &&
                        !HasEffect(Buffs.PerfectBalance) &&
                        !IsOriginal(MasterfulBlitz))
                        return OriginalHook(MasterfulBlitz);

                    // Perfect Balance
                    if (HasEffect(Buffs.PerfectBalance))
                    {
                        #region Open Solar

                        if (!solarNadi && !bothNadisOpen)
                        {
                            if (coeurlChakra == 0)
                                return Gauge.CoeurlFury == 0
                                    ? Demolish
                                    : OriginalHook(SnapPunch);

                            if (raptorChakra == 0)
                                return Gauge.RaptorFury == 0
                                    ? TwinSnakes
                                    : OriginalHook(TrueStrike);

                            if (opoOpoChakra == 0)
                                return Gauge.OpoOpoFury == 0
                                    ? DragonKick
                                    : OriginalHook(Bootshine);
                        }

                        #endregion

                        #region Open Lunar

                        if (solarNadi || lunarNadi || bothNadisOpen)
                            return Gauge.OpoOpoFury == 0
                                ? DragonKick
                                : OriginalHook(Bootshine);

                        #endregion
                    }
                }

                if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                        IsEnabled(CustomComboPreset.MNK_STUseFiresReply) &&
                        HasEffect(Buffs.FiresRumination) &&
                        !HasEffect(Buffs.PerfectBalance) &&
                        !HasEffect(Buffs.FormlessFist) &&
                        (JustUsed(OriginalHook(Bootshine)) ||
                         JustUsed(DragonKick) ||
                         GetBuffRemainingTime(Buffs.FiresRumination) < 4))
                        return FiresReply;

                    if (IsEnabled(CustomComboPreset.MNK_STUseROW) && 
                        IsEnabled(CustomComboPreset.MNK_STUseWindsReply) &&
                        HasEffect(Buffs.WindsRumination) &&
                        LevelChecked(WindsReply) &&
                        (HasEffect(Buffs.RiddleOfFire) ||
                         GetBuffRemainingTime(Buffs.WindsRumination) < 4))
                        return WindsReply;
                }

                // Standard Beast Chakras
                return MNKHelper.DetermineCoreAbility(actionID, IsEnabled(CustomComboPreset.MNK_STUseTrueNorth));
            }

            return actionID;
        }
    }

    internal class MNK_AOE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
            bool lunarNadi = Gauge.Nadi == Nadi.LUNAR;
            bool nadiNone = Gauge.Nadi == Nadi.NONE;

            if (actionID is ArmOfTheDestroyer or ShadowOfTheDestroyer)
            {
                if (!InCombat() && Gauge.Chakra < 5 && LevelChecked(Meditation))
                    return OriginalHook(Meditation);

                //Variant Cure
                if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                    return Variant.VariantCure;

                if (ActionReady(RiddleOfFire) &&
                    CanDelayedWeave(ActionWatching.LastWeaponskill))
                    return RiddleOfFire;

                // Buffs
                if (CanWeave(ActionWatching.LastWeaponskill))
                {
                    //Variant Rampart
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (ActionReady(Brotherhood))
                        return Brotherhood;
                    
                    if (ActionReady(RiddleOfWind))
                        return RiddleOfWind;

                    if (ActionReady(PerfectBalance) &&
                        !HasEffect(Buffs.PerfectBalance))

                        if (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance) ||
                            GetCooldownRemainingTime(PerfectBalance) <= 4 ||
                            HasEffect(Buffs.Brotherhood) ||
                            (HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                            (GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                            return PerfectBalance;

                    if (Gauge.Chakra >= 5 &&
                        LevelChecked(HowlingFist) &&
                        HasBattleTarget())
                        return OriginalHook(HowlingFist);

                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;
                }

                if (HasEffect(Buffs.WindsRumination))
                    return WindsReply;

                if (HasEffect(Buffs.FiresRumination))
                    return FiresReply;

                // Masterful Blitz
                if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance) &&
                    OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                    return OriginalHook(MasterfulBlitz);

                // Perfect Balance
                if (HasEffect(Buffs.PerfectBalance))
                {
                    if (nadiNone || !lunarNadi)
                        if (pbStacks?.StackCount > 0)
                            return LevelChecked(ShadowOfTheDestroyer)
                                ? ShadowOfTheDestroyer
                                : Rockbreaker;

                    if (lunarNadi)
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

                // Monk Rotation
                if (HasEffect(Buffs.OpoOpoForm))
                    return OriginalHook(ArmOfTheDestroyer);

                if (HasEffect(Buffs.RaptorForm))
                {
                    if (LevelChecked(FourPointFury))
                        return FourPointFury;

                    if (LevelChecked(TwinSnakes))
                        return TwinSnakes;
                }

                if (HasEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
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
            Status? pbStacks = FindEffectAny(Buffs.PerfectBalance);
            bool lunarNadi = Gauge.Nadi == Nadi.LUNAR;
            bool nadiNone = Gauge.Nadi == Nadi.NONE;

            if (actionID is ArmOfTheDestroyer or ShadowOfTheDestroyer)
            {
                if (IsEnabled(CustomComboPreset.MNK_AoEUseMeditation) &&
                    !InCombat() && Gauge.Chakra < 5 && LevelChecked(Meditation))
                    return OriginalHook(Meditation);

                //Variant Cure
                if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs) &&
                    IsEnabled(CustomComboPreset.MNK_AoEUseROF) &&
                    ActionReady(RiddleOfFire) &&
                    CanDelayedWeave(ActionWatching.LastWeaponskill))
                    return RiddleOfFire;

                // Buffs
                if (CanWeave(ActionWatching.LastWeaponskill))
                {
                    //Variant Rampart
                    if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
                    {
                        if (IsEnabled(CustomComboPreset.MNK_AoEUseBrotherhood) &&
                            ActionReady(Brotherhood))
                            return Brotherhood;

                        if (IsEnabled(CustomComboPreset.MNK_AoEUseROW) &&
                            ActionReady(RiddleOfWind))
                            return RiddleOfWind;
                    }

                    if (IsEnabled(CustomComboPreset.MNK_AoEUsePerfectBalance) &&
                        ActionReady(PerfectBalance) &&
                        !HasEffect(Buffs.PerfectBalance))

                        // Use Perfect Balance if:
                        // 1. It's after Bootshine/Dragon Kick. - This doesn't apply to AoE
                        // 2. At max stacks / before overcap.
                        // 3. During Brotherhood.
                        // 4. During Riddle of Fire.
                        // 5. Prepare Masterful Blitz for the Riddle of Fire & Brotherhood window.
                        if (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance) ||
                            GetCooldownRemainingTime(PerfectBalance) <= 4 ||
                            HasEffect(Buffs.Brotherhood) ||
                            (HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10) ||
                            (GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                            return PerfectBalance;

                    if (IsEnabled(CustomComboPreset.MNK_AoEUseHowlingFist) &&
                        Gauge.Chakra >= 5 &&
                        LevelChecked(HowlingFist) &&
                        HasBattleTarget())
                        return OriginalHook(HowlingFist);

                    if (IsEnabled(CustomComboPreset.MNK_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.MNK_AoE_SecondWind_Threshold &&
                            ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.MNK_AoE_Bloodbath_Threshold &&
                            ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }
                }

                if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_AoEUseROF) &&
                        IsEnabled(CustomComboPreset.MNK_AoEUseFiresReply) &&
                        HasEffect(Buffs.FiresRumination))
                        return FiresReply;

                    if (IsEnabled(CustomComboPreset.MNK_AoEUseROW) &&
                        IsEnabled(CustomComboPreset.MNK_AoEUseWindsReply) &&
                        HasEffect(Buffs.WindsRumination))
                        return WindsReply;
                }

                // Masterful Blitz
                if (IsEnabled(CustomComboPreset.MNK_AoEUsePerfectBalance))
                {
                    if (LevelChecked(MasterfulBlitz) &&
                        !HasEffect(Buffs.PerfectBalance) &&
                        OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                        return OriginalHook(MasterfulBlitz);

                    // Perfect Balance
                    if (HasEffect(Buffs.PerfectBalance))
                    {
                        if (nadiNone || !lunarNadi)
                            if (pbStacks?.StackCount > 0)
                                return LevelChecked(ShadowOfTheDestroyer)
                                    ? ShadowOfTheDestroyer
                                    : Rockbreaker;

                        if (lunarNadi)
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

                if (HasEffect(Buffs.RaptorForm))
                {
                    if (LevelChecked(FourPointFury))
                        return FourPointFury;

                    if (LevelChecked(TwinSnakes))
                        return TwinSnakes;
                }

                if (HasEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
                    return Rockbreaker;
            }

            return actionID;
        }
    }

    internal class MNK_PerfectBalance : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_PerfectBalance;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is PerfectBalance &&
                OriginalHook(MasterfulBlitz) != MasterfulBlitz && LevelChecked(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            return actionID;
        }
    }

    internal class MNK_Riddle_Brotherhood : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_Riddle_Brotherhood;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is RiddleOfFire && ActionReady(Brotherhood) && IsOnCooldown(RiddleOfFire)
                ? Brotherhood
                : actionID;
        }
    }

    #region Beast Chakras

    internal class MNK_BeastChakra_OpoOpo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.MNK_BC_OPOOPO) &&
                actionID is Bootshine or LeapingOpo &&
                (HasEffect(Buffs.OpoOpoForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance)))
                return Gauge.OpoOpoFury == 0 && LevelChecked(DragonKick)
                    ? DragonKick
                    : OriginalHook(Bootshine);

            return actionID;
        }
    }

    internal class MNK_BeastChakra_Raptor : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.MNK_BC_RAPTOR) &&
                actionID is TrueStrike or RisingRaptor &&
                (HasEffect(Buffs.RaptorForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance)))
                return Gauge.RaptorFury == 0 && LevelChecked(TwinSnakes)
                    ? TwinSnakes
                    : OriginalHook(TrueStrike);

            return actionID;
        }
    }

    internal class MNK_BeastChakra_Coeurl : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.MNK_BC_COEURL) &&
                actionID is SnapPunch or PouncingCoeurl &&
                (HasEffect(Buffs.CoeurlForm) || HasEffect(Buffs.FormlessFist) || HasEffect(Buffs.PerfectBalance)))
                return Gauge.CoeurlFury == 0 && LevelChecked(Demolish)
                    ? Demolish
                    : OriginalHook(SnapPunch);

            return actionID;
        }
    }

    #endregion
}