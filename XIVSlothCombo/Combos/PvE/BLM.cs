using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;
using static XIVSlothCombo.Combos.JobHelpers.BLM;

namespace XIVSlothCombo.Combos.PvE;

internal class BLM
{
    public const byte ClassID = 7;
    public const byte JobID = 25;

    public const uint
        Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        AetherialManipulation = 155,
        Scathe = 156,
        Manafont = 158,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Thunder4 = 7420,
        Triplecast = 7421,
        Foul = 7422,
        Thunder2 = 7447,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Amplifier = 25796,
        Paradox = 25797,
        HighThunder = 36986,
        HighThunder2 = 36987,
        FlareStar = 36989;

    // Debuff Pairs of Actions and Debuff
    public static readonly Dictionary<uint, ushort>
        ThunderList = new()
        {
            { Thunder, Debuffs.Thunder },
            { Thunder2, Debuffs.Thunder2 },
            { Thunder3, Debuffs.Thunder3 },
            { Thunder4, Debuffs.Thunder4 },
            { HighThunder, Debuffs.HighThunder },
            { HighThunder2, Debuffs.HighThunder2 }
        };

    protected static BLMGauge gauge = GetJobGauge<BLMGauge>();

    public static class Buffs
    {
        public const ushort
            Thundercloud = 164,
            Firestarter = 165,
            LeyLines = 737,
            CircleOfPower = 738,
            Sharpcast = 867,
            Triplecast = 1211,
            Thunderhead = 3870;
    }

    public static class Debuffs
    {
        public const ushort
            Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            Thunder4 = 1210,
            HighThunder = 3871,
            HighThunder2 = 3872;
    }

    public static class Traits
    {
        public const uint
            UmbralHeart = 295,
            EnhancedPolyglot = 297,
            AspectMasteryIII = 459,
            EnhancedFoul = 461,
            EnhancedManafont = 463,
            Enochian = 460,
            EnhancedPolyglotII = 615;
    }

    public static class MP
    {
        public const int MaxMP = 10000;

        public const int AllMPSpells = 800; //"ALL MP" spell. Only caring about the absolute minimum.

        public static int FireI => GetResourceCost(OriginalHook(Fire));

        public static int FlareAoE => GetResourceCost(OriginalHook(Flare));

        public static int FireAoE => GetResourceCost(OriginalHook(Fire2));

        public static int FireIII => GetResourceCost(OriginalHook(Fire3));

        public static int BlizzardAoE =>
            GetResourceCost(OriginalHook(Blizzard2));

        public static int BlizzardI =>
            GetResourceCost(OriginalHook(Blizzard));

        public static int Freeze => GetResourceCost(OriginalHook(BLM.Freeze));

        public static int Despair =>
            GetResourceCost(OriginalHook(BLM.Despair));
    }

    public static class Config
    {
        public static UserBool
            BLM_Adv_Xeno_Burst = new("BLM_Adv_Xeno_Burst");

        public static UserBoolArray
            BLM_Adv_Cooldowns_Choice = new("BLM_Adv_Cooldowns_Choice"),
            BLM_AoE_Adv_Cooldowns_Choice = new("BLM_AoE_Adv_Cooldowns_Choice"),
            BLM_Adv_Movement_Choice = new("BLM_Adv_Movement_Choice");

        public static UserInt
            BLM_VariantCure = new("BLM_VariantCure"),
            BLM_Adv_Cooldowns = new("BLM_Adv_Cooldowns"),
            BLM_Adv_Thunder = new("BLM_Adv_Thunder"),
            BLM_Adv_Rotation_Options = new("BLM_Adv_Rotation_Options"),
            BLM_Advanced_OpenerSelection = new("BLM_Advanced_OpenerSelection"),
            BLM_ST_Adv_ThunderHP = new("BLM_ST_Adv_ThunderHP"),
            BLM_AoE_Adv_ThunderHP = new("BLM_AoE_Adv_ThunderHP"),
            BLM_AoE_Adv_ThunderUptime = new("BLM_AoE_Adv_ThunderUptime"),
            BLM_Adv_ThunderCloud = new("BLM_Adv_ThunderCloud"),
            BLM_Adv_InitialCast = new("BLM_Adv_InitialCast");

        public static UserFloat
            BLM_AstralFire_Refresh = new("BLM_AstralFire_Refresh");
    }

    internal class BLM_ST_SimpleMode : CustomCombo
    {
        internal static BLMOpenerLogic BLMOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID != Fire) return actionID;

            int maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
                TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
            int maxPolyglotCD = maxPolyglot * 30000;

            int remainingPolyglotCD = Math.Max(0,
                (maxPolyglot - gauge.PolyglotStacks) * 30000 + (gauge.EnochianTimer - 30000));
            uint curMp = LocalPlayer.CurrentMp;

            int nextMpGain = gauge.UmbralIceStacks switch
            {
                0 => 0,
                1 => 2500,
                2 => 5000,
                3 => 10000,
                _ => 0
            };

            Status? thunderDebuff =
                FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer.GameObjectId);
            float elementTimer = gauge.ElementTimeRemaining / 1000f;
            double gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(Fire));

            bool canSwiftB3 = IsOffCooldown(All.Swiftcast) || ActionReady(Triplecast) ||
                              GetBuffStacks(Buffs.Triplecast) > 0;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave(actionID))
                return Variant.VariantRampart;

            if (BLMOpener.DoFullOpener(ref actionID))
                return actionID;

            if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1)
                if (thunderDebuff is null || thunderDebuff.RemainingTime < 3)
                    return OriginalHook(Thunder);

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave(ActionWatching.LastSpell))
                return Amplifier;

            if (remainingPolyglotCD < 6000 && gcdsInTimer > 2 && gauge.HasPolyglotStacks())
                return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;

            if (IsMoving)
            {
                if (ActionReady(Amplifier) && gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (gauge.HasPolyglotStacks())
                    return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;
            }

            if (CanSpellWeave(actionID) && ActionReady(LeyLines))
                return LeyLines;

            if (gauge.InAstralFire)
            {
                if (gauge.IsParadoxActive && gcdsInTimer < 2 && curMp >= MP.FireI)
                    return Paradox;

                if (HasEffect(Buffs.Firestarter))
                    if (gcdsInTimer < 2 || curMp < MP.FireI || WasLastAbility(Transpose))
                        return Fire3;

                if (curMp < MP.FireI && Despair.LevelChecked() && curMp >= MP.Despair)
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                        return Triplecast;

                    return Despair;
                }

                if (curMp == 0 && FlareStar.LevelChecked() && gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (Fire4.LevelChecked())
                    if (gcdsInTimer > 1 && curMp >= MP.FireI)
                        return Fire4;

                if (curMp >= MP.FireI)
                    return Fire;

                if (ActionReady(Manafont))
                    return Manafont;

                if (ActionReady(Blizzard3) && !canSwiftB3)
                    return Blizzard3;

                if (ActionReady(Transpose))
                    return Transpose;
            }

            if (gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                        return Triplecast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return Blizzard3;
                }

                if (Blizzard4.LevelChecked() && gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (gauge.IsParadoxActive)
                    return Paradox;

                if (gauge.HasPolyglotStacks())
                    return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;

                if (curMp + nextMpGain >= 7500 && (LocalPlayer.CastActionId == Blizzard || WasLastSpell(Blizzard) ||
                                                   WasLastSpell(Blizzard4)))
                {
                    if (Fire3.LevelChecked())
                        return Fire3;

                    return Fire;
                }

                if (curMp + nextMpGain <= 10000 || curMp < 7500)
                    return Blizzard;

                if (ActionReady(Transpose) && CanSpellWeave(ActionWatching.LastSpell))
                    return Transpose;

                if (Fire3.LevelChecked())
                    return Fire3;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;

            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        internal static BLMOpenerLogic BLMOpener = new();

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID != Fire) return actionID;

            int maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
                TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
            int maxPolyglotCD = maxPolyglot * 30000;

            int remainingPolyglotCD = Math.Max(0,
                (maxPolyglot - gauge.PolyglotStacks) * 30000 + (gauge.EnochianTimer - 30000));
            uint curMp = LocalPlayer.CurrentMp;

            int nextMpGain = gauge.UmbralIceStacks switch
            {
                0 => 0,
                1 => 2500,
                2 => 5000,
                3 => 10000,
                _ => 0
            };

            Status? thunderDebuff =
                FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer.GameObjectId);
            float elementTimer = gauge.ElementTimeRemaining / 1000f;
            double gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(Fire));

            bool canSwiftB3 = IsOffCooldown(All.Swiftcast) || ActionReady(Triplecast) ||
                              GetBuffStacks(Buffs.Triplecast) > 0;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave(actionID))
                return Variant.VariantRampart;

            if (IsEnabled(CustomComboPreset.BLM_ST_Opener))
                if (BLMOpener.DoFullOpener(ref actionID))
                    return actionID;

            if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1)
                if (thunderDebuff is null || thunderDebuff.RemainingTime < 3)
                    return OriginalHook(Thunder);

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave(ActionWatching.LastSpell))
                return Amplifier;

            if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                remainingPolyglotCD < 6000 && gcdsInTimer > 2 && gauge.HasPolyglotStacks())
                return Xenoglossy.LevelChecked()
                    ? Xenoglossy
                    : Foul;

            if (IsMoving)
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglotMoving) &&
                    gauge.HasPolyglotStacks())
                    return Xenoglossy.LevelChecked()
                        ? Xenoglossy
                        : Foul;
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_LeyLines) &&
                CanSpellWeave(actionID) && ActionReady(LeyLines))
                return LeyLines;

            if (gauge.InAstralFire)
            {
                if (gauge.IsParadoxActive && gcdsInTimer < 2 && curMp >= MP.FireI)
                    return Paradox;

                if (HasEffect(Buffs.Firestarter))
                    if (gcdsInTimer < 2 || curMp < MP.FireI || WasLastAbility(Transpose))
                        return Fire3;

                if (IsEnabled(CustomComboPreset.BLM_ST_Despair) &&
                    curMp < MP.FireI && Despair.LevelChecked() && curMp >= MP.Despair)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                        return Triplecast;

                    return Despair;
                }

                if (IsEnabled(CustomComboPreset.BLM_ST_Flarestar) &&
                    curMp == 0 && FlareStar.LevelChecked() && gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (Fire4.LevelChecked())
                    if (gcdsInTimer > 1 && curMp >= MP.FireI)
                        return Fire4;

                if (curMp >= MP.FireI)
                    return Fire;

                if (IsEnabled(CustomComboPreset.BLM_ST_Manafont) &&
                    ActionReady(Manafont))
                    return Manafont;

                if (ActionReady(Blizzard3) && !canSwiftB3)
                    return Blizzard3;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose))
                    return Transpose;
            }

            if (gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return Blizzard3;
                }

                if (Blizzard4.LevelChecked() && gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (gauge.IsParadoxActive)
                    return Paradox;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                    gauge.HasPolyglotStacks())
                    return Xenoglossy.LevelChecked()
                        ? Xenoglossy
                        : Foul;

                if (curMp + nextMpGain >= 7500 &&
                    (LocalPlayer.CastActionId == Blizzard ||
                     WasLastSpell(Blizzard) ||
                     WasLastSpell(Blizzard4)))
                    return Fire3.LevelChecked()
                        ? Fire3
                        : Fire;

                if (curMp + nextMpGain <= 10000 || curMp < 7500)
                    return Blizzard;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose) && CanSpellWeave(ActionWatching.LastSpell))
                    return Transpose;

                if (Fire3.LevelChecked())
                    return Fire3;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;

            return actionID;
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2)) return actionID;

            int maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
                TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
            int maxPolyglotCD = maxPolyglot * 30000;

            int remainingPolyglotCD = Math.Max(0,
                (maxPolyglot - gauge.PolyglotStacks) * 30000 + (gauge.EnochianTimer - 30000));
            uint curMp = LocalPlayer.CurrentMp;

            int nextMpGain = gauge.UmbralIceStacks switch
            {
                0 => 0,
                1 => 2500,
                2 => 5000,
                3 => 10000,
                _ => 0
            };

            Status? thunderDebuff =
                FindEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget, LocalPlayer.GameObjectId);
            float elementTimer = gauge.ElementTimeRemaining / 1000f;
            double gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(ActionWatching.LastSpell));

            bool canSwiftF = TraitLevelChecked(Traits.AspectMasteryIII) && (IsOffCooldown(All.Swiftcast) ||
                                                                            ActionReady(Triplecast) ||
                                                                            GetBuffStacks(Buffs.Triplecast) > 0);
            bool canWeave = CanSpellWeave(ActionWatching.LastSpell);

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 && Thunder2.LevelChecked())
                if (thunderDebuff is null || thunderDebuff.RemainingTime < 3)
                    return OriginalHook(Thunder2);

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && canWeave)
                return Amplifier;

            if (IsMoving)
            {
                if (ActionReady(Amplifier) && gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (gauge.HasPolyglotStacks())
                    return Foul;
            }

            if (canWeave && ActionReady(LeyLines))
                return LeyLines;

            if (gauge.InAstralFire)
            {
                if (curMp == 0 && FlareStar.LevelChecked() && gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (!FlareStar.LevelChecked() && Fire2.LevelChecked() && curMp >= MP.FireAoE &&
                    (gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (Flare.LevelChecked() && curMp >= MP.FlareAoE)
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 && canWeave)
                        return Triplecast;

                    return Flare;
                }

                if (Fire2.LevelChecked())
                    if (gcdsInTimer > 1 && curMp >= MP.FireAoE)
                        return OriginalHook(Fire2);

                if (ActionReady(Manafont))
                    return Manafont;

                if (ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || canSwiftF))
                    return Transpose;

                if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);
            }

            if (gauge.InUmbralIce)
            {
                if (ActionWatching.WhichOfTheseActionsWasLast(OriginalHook(Fire2), OriginalHook(Freeze),
                        OriginalHook(Flare), OriginalHook(FlareStar)) == OriginalHook(Freeze) &&
                    FlareStar.LevelChecked())
                {
                    if (ActionReady(Transpose) && canWeave)
                        return Transpose;

                    return OriginalHook(Fire2);
                }

                if (ActionReady(OriginalHook(Blizzard2)) && gauge.UmbralIceStacks < 3 &&
                    TraitLevelChecked(Traits.AspectMasteryIII))
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 && canWeave)
                        return Triplecast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast) && canWeave)
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return OriginalHook(Blizzard2);
                }

                if (gauge.UmbralIceStacks < 3 && ActionReady(OriginalHook(Blizzard2)))
                    return OriginalHook(Blizzard2);

                if (Freeze.LevelChecked() && gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Freeze;

                if (gauge.HasPolyglotStacks())
                    return Foul;

                if (BLMHelper.DoubleBlizz())
                    if (Fire2.LevelChecked())
                        return OriginalHook(Fire2);

                if (curMp < LocalPlayer.MaxMp)
                    return Freeze.LevelChecked() ? OriginalHook(Freeze) : OriginalHook(Blizzard2);

                if (ActionReady(Transpose) &&
                    ((canWeave && Flare.LevelChecked()) || !TraitLevelChecked(Traits.AspectMasteryIII)))
                    return Transpose;

                if (Fire2.LevelChecked() && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked())
                return OriginalHook(Blizzard2);

            return actionID;
        }
    }

    internal class BLM_Variant_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            return actionID is All.Swiftcast && HasEffect(All.Buffs.Swiftcast) && IsEnabled(Variant.VariantRaise)
                ? Variant.VariantRaise
                : actionID;
        }
    }

    internal class BLM_Scathe_Xeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is Scathe && LevelChecked(Xenoglossy) && GetJobGauge<BLMGauge>().HasPolyglotStacks()
                ? Xenoglossy
                : actionID;
        }
    }

    internal class BLM_Blizzard_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Blizzard && LevelChecked(Freeze) && !GetJobGauge<BLMGauge>().InUmbralIce)
                return Blizzard3;

            if (actionID is Freeze && !LevelChecked(Freeze))
                return Blizzard2;

            return actionID;
        }
    }

    internal class BLM_Fire_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is Fire && ((LevelChecked(Fire3) && !GetJobGauge<BLMGauge>().InAstralFire) ||
                                        HasEffect(Buffs.Firestarter))
                ? Fire3
                : actionID;
        }
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
        }
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is AetherialManipulation &&
                   ActionReady(BetweenTheLines) &&
                   HasEffect(Buffs.LeyLines) &&
                   !HasEffect(Buffs.CircleOfPower) &&
                   !IsMoving
                ? BetweenTheLines
                : actionID;
        }
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is Transpose && GetJobGauge<BLMGauge>().InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
        }
    }
}