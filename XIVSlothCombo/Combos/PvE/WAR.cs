using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Extensions;

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
                InnerRelease = 1177,
                SurgingTempest = 2677,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                Wrathful = 3901,
                PrimalRuinationReady = 3834,
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
                WAR_VariantCure = "WAR_VariantCure",
                WAR_FellCleaveGauge = "WAR_FellCleaveGauge",
                WAR_DecimateGauge = "WAR_DecimateGauge",
                WAR_InfuriateSTGauge = "WAR_InfuriateSTGauge",
                WAR_InfuriateAoEGauge = "WAR_InfuriateAoEGauge";
        }

        // Replace Storm's Path with Storm's Path combo and overcap feature on main combo to fellcleave
        internal class WAR_ST_StormsPath : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_StormsPath;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath) && actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var surgingThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_SurgingRefreshRange);
                    var onslaughtChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.WAR_KeepOnslaughtCharges);
                    var fellCleaveGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_FellCleaveGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateSTGauge);

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_RangedUptime) && LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;

                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Infuriate) && InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && gauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Storm's Eye level check
                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_InnerRelease) && CanWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(StormsEye) && InCombat())
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

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;


                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_InnerRelease) && CanWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Upheaval) && IsOffCooldown(Upheaval) && LevelChecked(Upheaval))
                                return Upheaval;
                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught) && LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > onslaughtChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught_MeleeSpender) ||
                                    (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught_MeleeSpender) && !IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))))
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetCooldownRemainingTime(InnerRelease) < 57.5)
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange) && !IsMoving && (GetTargetDistance() <= 1 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10))
                                return PrimalRend;
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && WasLastWeaponskill(PrimalRend))
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_FellCleave) && LevelChecked(InnerBeast))
                        {
                            if (HasEffect(Buffs.InnerRelease) || (HasEffect(Buffs.NascentChaos) && InnerChaos.LevelChecked()))
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && !InnerChaos.LevelChecked())
                                return OriginalHook(Decimate);
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_FellCleave) && LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge >= fellCleaveGaugeSpend)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                        {
                            return Maim;
                        }

                        if (lastComboMove == Maim && LevelChecked(StormsPath))
                        {
                            if ((GetBuffRemainingTime(Buffs.SurgingTempest) <= surgingThreshold) && LevelChecked(StormsEye))
                                return StormsEye;
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
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

        internal class WAR_AoE_Overpower : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Overpower;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var decimateGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_DecimateGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateAoEGauge);

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Infuriate) && InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && gauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Mythril Tempest level check
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_InnerRelease) && CanWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(MythrilTempest) && InCombat())
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

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_InnerRelease) && CanWeave(actionID) && IsOffCooldown(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Orogeny) && IsOffCooldown(Orogeny) && LevelChecked(Orogeny) && HasEffect(Buffs.SurgingTempest))
                                return Orogeny;
                            if (IsEnabled(CustomComboPreset.WAR_AOE_Overpower_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetCooldownRemainingTime(InnerRelease) < 57.5)
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange) && (GetTargetDistance() <= 3 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10))
                                return PrimalRend;
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && WasLastWeaponskill(PrimalRend))
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Decimate) && LevelChecked(SteelCyclone) && (gauge >= decimateGaugeSpend || HasEffect(Buffs.InnerRelease) || HasEffect(Buffs.NascentChaos)))
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


        internal class WAR_ST_StormsPath_PrimalRend : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_StormsPath_PrimalRend;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == InnerBeast || actionID == SteelCyclone)
                {

                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;

                    // Fell Cleave or Decimate
                    return OriginalHook(actionID);


                }

                return actionID;
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
                    var hasInnerRelease = HasEffect(Buffs.InnerRelease);

                    if (InCombat() && rageGauge.BeastGauge <= rageThreshold && ActionReady(Infuriate) && !hasNascent
                    && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst)))
                        return OriginalHook(Infuriate);
                }

                return actionID;
            }
        }

        internal class WAR_PrimalRend_InnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalRend_InnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Berserk or InnerRelease)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                }

                return actionID;
            }
        }
    }
}
