using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class WAR
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        public const uint
            HeavySwing = 31,
            Maim = 37,
            Berserk = 38,
            Overpower = 41,
            StormsPath = 42,
            StormsEye = 45,
            Tomahawk = 46,
            InnerBeast = 49,
            SteelCyclone = 51,
            Infuriate = 52,
            FellCleave = 3549,
            Decimate = 3550,
            Upheaval = 7387,
            InnerRelease = 7389,
            RawIntuition = 3551,
            MythrilTempest = 16462,
            ChaoticCyclone = 16463,
            NascentFlash = 16464,
            InnerChaos = 16465,
            Orogeny = 25752,
            PrimalRend = 25753,
            PrimalWrath = 36924,
            PrimalRuination = 36925,
            Onslaught = 7386;

        public static class Buffs
        {
            public const ushort
                InnerReleaseStacks = 1177,
                InnerReleaseBuff = 1303,
                SurgingTempest = 2677,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                Wrathful = 3901,
                PrimalRuinationReady = 3834,
                BurgeoningFury = 3833,
                Berserk = 86;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 1;
        }

        public static class Config
        {
            public const string
                WAR_InfuriateRange = "WarInfuriateRange",
                WAR_SurgingRefreshRange = "WarSurgingRefreshRange",
                WAR_KeepOnslaughtCharges = "WarKeepOnslaughtCharges",
                WAR_KeepInfuriateCharges = "WarKeepInfuriateCharges",
                WAR_VariantCure = "WAR_VariantCure",
                WAR_FellCleaveGauge = "WAR_FellCleaveGauge",
                WAR_DecimateGauge = "WAR_DecimateGauge",
                WAR_InfuriateSTGauge = "WAR_InfuriateSTGauge",
                WAR_InfuriateAoEGauge = "WAR_InfuriateAoEGauge";
        }

        internal class WAR_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    if (LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;
                    if (InCombat() && LevelChecked(Infuriate) && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= 40 && CanWeave(actionID))
                        return Infuriate;
                    if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(StormsEye) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (ActionReady(Upheaval) && LevelChecked(Upheaval))
                                return Upheaval;
                            if (HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                            if (LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > 1)
                            {
                                if (!IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease)))
                                    return Onslaught;
                            }
                        }

                        if (HasEffect(Buffs.PrimalRendReady) && !JustUsed(InnerRelease) && ((!IsMoving && GetTargetDistance() <= 1) || GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD))
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend))
                            return PrimalRuination;

                        if (LevelChecked(InnerBeast))
                        {
                            if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos)))
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && !LevelChecked(InnerChaos) && gauge >= 50)
                                return OriginalHook(Decimate);
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge >= 90)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                        {
                            return Maim;
                        }

                        if (lastComboMove == Maim && LevelChecked(StormsPath))
                        {
                            if (GetBuffRemainingTime(Buffs.SurgingTempest) <= 29 && LevelChecked(StormsEye))
                                return StormsEye;
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }

        internal class WAR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced) && actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var surgingThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_SurgingRefreshRange);
                    var onslaughtChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.WAR_KeepOnslaughtCharges);
                    var infuriateChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.WAR_KeepInfuriateCharges);
                    var fellCleaveGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_FellCleaveGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateSTGauge);
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && 
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_RangedUptime) &&
                        LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Infuriate) && 
                        InCombat() && LevelChecked(Infuriate) && ActionReady(Infuriate) && 
                        !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks)
                        && gauge <= infuriateGauge && CanWeave(actionID) && GetRemainingCharges(Infuriate) > infuriateChargesRemaining)
                        return Infuriate;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(StormsEye) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;


                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Upheaval) && ActionReady(Upheaval) && LevelChecked(Upheaval))
                                return Upheaval;
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught) && LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > onslaughtChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) ||
                                    (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) && !IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))))
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && !JustUsed(InnerRelease))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late)
                                && GetBuffStacks(Buffs.InnerReleaseStacks) is 0 && GetBuffStacks(Buffs.BurgeoningFury) is 0
                                && !HasEffect(Buffs.Wrathful))
                                return PrimalRend;
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late) && ((!IsMoving && GetTargetDistance() <= 1) || GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend))
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast))
                            {
                                if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos)))
                                    return OriginalHook(InnerBeast);

                                if (HasEffect(Buffs.NascentChaos) && !LevelChecked(InnerChaos) && gauge >= 50)
                                    return OriginalHook(Decimate);
                            }
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge >= fellCleaveGaugeSpend)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                        {
                            return Maim;
                        }

                        if (lastComboMove == Maim && LevelChecked(StormsPath) && IsEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye))
                        {
                            if (GetBuffRemainingTime(Buffs.SurgingTempest) <= surgingThreshold && LevelChecked(StormsEye))
                                return StormsEye;
                            return StormsPath;
                        }
                        if (lastComboMove == Maim && LevelChecked(StormsPath) && IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye))
                        {
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }

        internal class WAR_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;

                    if (InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= 40 && CanWeave(actionID))
                        return Infuriate;
                    if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(MythrilTempest) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (ActionReady(Orogeny) && LevelChecked(Orogeny) && HasEffect(Buffs.SurgingTempest))
                                return Orogeny;
                            if (HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                        }

                        if (HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD)
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend, 4f))
                            return PrimalRuination;
                        if (LevelChecked(SteelCyclone) && (gauge >= 90 || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos)))
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Overpower && LevelChecked(MythrilTempest))
                        {
                            return MythrilTempest;
                        }
                    }

                    return Overpower;
                }

                return actionID;
            }
        }

        internal class WAR_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var decimateGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_DecimateGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateAoEGauge);
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Infuriate) && InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Mythril Tempest level check
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(MythrilTempest) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Orogeny) && ActionReady(Orogeny) && LevelChecked(Orogeny) && HasEffect(Buffs.SurgingTempest))
                                return Orogeny;
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                            return PrimalRend;
                        if (IsNotEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD)
                            return PrimalRend;
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend, 4f))
                            return PrimalRuination;
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Decimate) && LevelChecked(SteelCyclone) && (gauge >= decimateGaugeSpend || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos)))
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Overpower && LevelChecked(MythrilTempest))
                        {
                            return MythrilTempest;
                        }
                    }

                    return Overpower;
                }

                return actionID;
            }
        }

        internal class War_ST_StormsEye : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.War_ST_StormsEye;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsEye)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                            return Maim;

                        if (lastComboMove == Maim && LevelChecked(StormsEye))
                            return StormsEye;
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }

        internal class WAR_NascentFlash : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_NascentFlash;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NascentFlash)
                {
                    if (LevelChecked(NascentFlash))
                        return NascentFlash;
                    return RawIntuition;
                }

                return actionID;
            }
        }


        internal class WAR_ST_Advanced_PrimalCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced_PrimalRend;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == InnerBeast || actionID == SteelCyclone)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                    if (LevelChecked(PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && JustUsed(PrimalRend))
                        return PrimalRuination;
                }

                // fell cleave or decimate
                return OriginalHook(actionID);
            }
        }

        internal class WAR_InfuriateFellCleave : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_InfuriateFellCleave;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is InnerBeast or FellCleave or SteelCyclone or Decimate)
                {
                    var rageGauge = GetJobGauge<WARGauge>();
                    var rageThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateRange);
                    var hasNascent = HasEffect(Buffs.NascentChaos);
                    var hasInnerRelease = HasEffect(Buffs.InnerReleaseStacks);

                    if (InCombat() && rageGauge.BeastGauge <= rageThreshold && ActionReady(Infuriate) && !hasNascent
                    && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst)))
                        return OriginalHook(Infuriate);
                }

                return actionID;
            }
        }

        internal class WAR_PrimalCombo_InnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalCombo_InnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Berserk or InnerRelease)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                    if (LevelChecked(PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && JustUsed(PrimalRend))
                        return PrimalRuination;
                }

                return actionID;
            }
        }
    }
}
