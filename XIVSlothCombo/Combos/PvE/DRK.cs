using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DRK
    {
        public const byte JobID = 32;

        public const uint
            // Single-Target 1-2-3 Combo
            HardSlash = 3617,
            SyphonStrike = 3623,
            Souleater = 3632,

            // AoE 1-2-3 Combo
            Unleash = 3621,
            StalwartSoul = 16468,

            // Single-Target oGCDs
            CarveAndSpit = 3643,      // With AbyssalDrain
            EdgeOfDarkness = 16467,   // For MP
            EdgeOfShadow = 16470,     // For MP // Upgrade of EdgeOfDarkness
            Bloodspiller = 7392,      // For Blood
            ScarletDelirium = 36928,  // Under Delirium
            Comeuppance = 36929,      // Under Delirium
            Torcleaver = 36930,       // Under Delirium

            // AoE oGCDs
            AbyssalDrain = 3641,      // With CarveAndSpit
            FloodOfDarkness = 16466,  // For MP
            FloodOfShadow = 16469,    // For MP // Upgrade of FloodOfDarkness
            Quietus = 7391,           // For Blood
            SaltedEarth = 3639,
            SaltAndDarkness = 25755,  // Recast of Salted Earth
            Impalement = 36931,       // Under Delirium

            // Buffing oGCDs
            Delirium = 7390,

            // Burst Window
            LivingShadow = 16472,
            Shadowbringer = 25757,
            Disesteem = 36932,

            // Ranged Option
            Unmend = 3624;

        public static class Buffs
        {
            public const ushort
                // Main Buffs
                BloodWeapon = 742,
                Delirium = 3836,

                // Periodic Buffs
                Darkside = 741,
                BlackestNight = 1178,

                // "DoT" Buffs
                SaltedEarth = 749,
                Scorn = 3837;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 1;
        }

        public static class Config
        {
            public const string
                DRK_ST_ManaSpenderPooling = "DRK_ST_ManaSpenderPooling",
                DRK_ST_LivingDeadThreshold = "DRK_ST_LivingDeadThreshold",
                DRK_AoE_LivingDeadThreshold = "DRK_AoE_LivingDeadThreshold",
                DRK_VariantCure = "DRKVariantCure";
        }

        // todo: chop down very long ifs

        internal class DRK_Souleater_Combo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_ST_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Bail if not looking at the replaced action
                if (actionID != Souleater) return actionID;

                var gauge = GetJobGauge<DRKGauge>();
                var mpRemaining = PluginConfiguration.GetCustomIntValue(Config.DRK_ST_ManaSpenderPooling);
                // todo: add hpRemaining for DRK_ST_LivingDeadThreshold

                // Variant Cure - Heal: Priority to save your life
                if (IsEnabled(CustomComboPreset.DRK_Variant_Cure)
                    && IsEnabled(Variant.VariantCure)
                    && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                    return Variant.VariantCure;

                // Unmend Option
                if (IsEnabled(CustomComboPreset.DRK_ST_RangedUptime)
                    && LevelChecked(Unmend)
                    && !InMeleeRange()
                    && HasBattleTarget())
                    return Unmend;

                // Bail if not in combat
                if (!InCombat()) return HardSlash;

                // oGCDs
                if (CanWeave(actionID))
                {
                    // Variant Spirit Dart - DoT
                    var sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart)
                        && IsEnabled(Variant.VariantSpiritDart)
                        && (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Variant Ultimatum - AoE Agro stun
                    if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum)
                        && IsEnabled(Variant.VariantUltimatum)
                        && IsOffCooldown(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    //Mana Features
                    if (IsEnabled(CustomComboPreset.DRK_ST_ManaOvercap))
                    {
                        if ((CombatEngageDuration().TotalSeconds < 7 && gauge.DarksideTimeRemaining == 0) // Initial Darkside upping
                            || CombatEngageDuration().TotalSeconds >= 6)
                        {
                            // todo: this needs to be checked
                            if (IsEnabled(CustomComboPreset.DRK_ST_ManaSpenderPooling)
                                && GetCooldownRemainingTime(Delirium) >= 40
                                && (gauge.HasDarkArts || LocalPlayer.CurrentMp > (mpRemaining + 3000))
                                && LevelChecked(EdgeOfDarkness) && CanDelayedWeave(actionID))
                                return OriginalHook(EdgeOfDarkness);

                            if (gauge.HasDarkArts
                                || LocalPlayer.CurrentMp > 8500 // todo: this needs to be checked, at least
                                || (gauge.DarksideTimeRemaining < 10000 && LocalPlayer.CurrentMp >= 3000))
                            {
                                // Return Edge of Darkness if available
                                if (LevelChecked(EdgeOfDarkness))
                                    return OriginalHook(EdgeOfDarkness);
                                if (LevelChecked(FloodOfDarkness)
                                    && !LevelChecked(EdgeOfDarkness))
                                    return FloodOfDarkness;
                            }
                        }
                    }

                    //oGCD Features
                    if (gauge.DarksideTimeRemaining > 1)
                    {
                        // Living Shadow
                        // todo: this needs to check with DRK_ST_LivingDeadThreshold
                        if (IsEnabled(CustomComboPreset.DRK_ST_CDs)
                            && IsEnabled(CustomComboPreset.DRK_ST_CDs_LivingShadow)
                            && IsOffCooldown(LivingShadow)
                            && LevelChecked(LivingShadow))
                            return LivingShadow;

                        // Delirium
                        if (IsEnabled(CustomComboPreset.DRK_ST_Delirium)
                            && IsOffCooldown(Delirium)
                            && LevelChecked(Delirium))
                            return Delirium;

                        // todo: Add Delirium combo

                        if (IsEnabled(CustomComboPreset.DRK_ST_CDs))
                        {
                            // Salted Earth
                            // todo: simplify this to make it easier to read
                            if (IsEnabled(CustomComboPreset.DRK_ST_CDs_SaltedEarth)
                                && LevelChecked(SaltedEarth)
                                )
                            {
                                if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                    (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && LevelChecked(SaltAndDarkness) && GetBuffRemainingTime(Buffs.SaltedEarth) < 9)) //Salt and Darkness
                                    return OriginalHook(SaltedEarth);
                            }

                            // Shadowbringer
                            // todo: simplify this to make it easier to read
                            if (LevelChecked(Shadowbringer)
                                && IsEnabled(CustomComboPreset.DRK_ST_CDs_Shadowbringer))
                            {
                                if ((GetRemainingCharges(Shadowbringer) > 0 && IsNotEnabled(CustomComboPreset.DRK_ST_CDs_ShadowbringerBurst)) || // Dump
                                    (IsEnabled(CustomComboPreset.DRK_ST_CDs_ShadowbringerBurst) && GetRemainingCharges(Shadowbringer) > 0 && gauge.ShadowTimeRemaining > 1 && IsOnCooldown(Delirium))) // Burst
                                    return Shadowbringer;
                            }

                            // Carve and Spit
                            if (IsEnabled(CustomComboPreset.DRK_ST_CDs_CarveAndSpit)
                                && IsOffCooldown(CarveAndSpit)
                                && LevelChecked(CarveAndSpit))
                                return CarveAndSpit;
                        }
                    }
                }

                //Delirium Features
                if (LevelChecked(Delirium)
                    && IsEnabled(CustomComboPreset.DRK_ST_CDs_Bloodspiller)
                    && IsEnabled(CustomComboPreset.DRK_ST_CDs))
                {
                    //Regular Delirium
                    if (GetBuffStacks(Buffs.Delirium) > 0
                        && IsNotEnabled(CustomComboPreset.DRK_ST_CDs_DelayedBloodspiller))
                        return Bloodspiller;

                    //Delayed Delirium
                    // todo: we can use this for mana generation on odd minutes
                    if (IsEnabled(CustomComboPreset.DRK_ST_CDs_DelayedBloodspiller)
                        && GetBuffStacks(Buffs.Delirium) > 0
                        && IsOnCooldown(Delirium)
                        && GetBuffStacks(Buffs.BloodWeapon) < 2)
                        return Bloodspiller;

                    //Blood management before Delirium
                    // todo: this needs to be checked
                    if (IsEnabled(CustomComboPreset.DRK_ST_Delirium)
                        && (
                            (gauge.Blood >= 60 && GetCooldownRemainingTime(Delirium) is > 0 and < 3)
                            || (gauge.Blood >= 50 && GetCooldownRemainingTime(Delirium) > 37 && !HasEffect(Buffs.Delirium))
                            ))
                        return Bloodspiller;
                }

                // 1-2-3 combo
                if (!(comboTime > 0)) return HardSlash;
                if (lastComboMove == HardSlash && LevelChecked(SyphonStrike))
                    return SyphonStrike;
                if (lastComboMove == SyphonStrike && LevelChecked(Souleater))
                {
                    // Blood management
                    // todo: this needs to be checked
                    if (IsEnabled(CustomComboPreset.DRK_ST_BloodOvercap)
                        && LevelChecked(Bloodspiller) && gauge.Blood >= 90)
                        return Bloodspiller;

                    return Souleater;
                }

                return HardSlash;

            }
        }

        internal class DRK_StalwartSoul_Combo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_AoE_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Bail if not looking at the replaced action
                if (actionID != StalwartSoul) return actionID;

                var gauge = GetJobGauge<DRKGauge>();

                // Variant Cure - Heal: Priority to save your life
                if (IsEnabled(CustomComboPreset.DRK_Variant_Cure)
                    && IsEnabled(Variant.VariantCure)
                    && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                    return Variant.VariantCure;


                // oGCDs
                if (CanWeave(actionID))
                {
                    // Variant Spirit Dart - DoT
                    var sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart)
                        && IsEnabled(Variant.VariantSpiritDart)
                        && (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Variant Ultimatum - AoE Agro stun
                    if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum)
                        && IsEnabled(Variant.VariantUltimatum)
                        && IsOffCooldown(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    // Mana Features
                    if (IsEnabled(CustomComboPreset.DRK_AoE_ManaOvercap)
                        && LevelChecked(FloodOfDarkness)
                        && (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || (gauge.DarksideTimeRemaining < 10 && LocalPlayer.CurrentMp >= 3000)))
                        return OriginalHook(FloodOfDarkness);

                    if (gauge.DarksideTimeRemaining > 1)
                    {
                        // Delirium
                        if (IsEnabled(CustomComboPreset.DRK_AoE_Delirium)
                            && IsOffCooldown(Delirium)
                            && LevelChecked(Delirium))
                            return Delirium;

                        // todo: Add Impalement

                        // Living Shadow
                        // todo: this needs to check with DRK_AoE_LivingDeadThreshold
                        if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_LivingShadow)
                            && IsOffCooldown(LivingShadow)
                            && LevelChecked(LivingShadow))
                            return LivingShadow;

                        // Salted Earth
                        if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_SaltedEarth)
                            && LevelChecked(SaltedEarth))
                        {
                            if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && LevelChecked(SaltAndDarkness))) //Salt and Darkness
                                return OriginalHook(SaltedEarth);
                        }

                        // Abyssal Drain
                        if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_AbyssalDrain)
                            && LevelChecked(AbyssalDrain)
                            && IsOffCooldown(AbyssalDrain)
                            && PlayerHealthPercentageHp() <= 60)
                            return AbyssalDrain;

                        // Shadowbringer
                        if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_Shadowbringer)
                            && LevelChecked(Shadowbringer)
                            && GetRemainingCharges(Shadowbringer) > 0)
                            return Shadowbringer;
                    }
                }

                if (IsEnabled(CustomComboPreset.DRK_AoE_Delirium))
                {
                    if (LevelChecked(Delirium)
                        && HasEffect(Buffs.Delirium)
                        && gauge.DarksideTimeRemaining > 0)
                        return Quietus;
                }

                // 1-2-3 combo
                if (!(comboTime > 0)) return Unleash;
                if (lastComboMove == Unleash && LevelChecked(StalwartSoul))
                {
                    if (IsEnabled(CustomComboPreset.DRK_AoE_BloodOvercap)
                        && gauge.Blood >= 90
                        && LevelChecked(Quietus))
                        return Quietus;
                    return StalwartSoul;
                }

                return Unleash;

            }
        }

        internal class DRK_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<DRKGauge>();

                if (actionID == CarveAndSpit || actionID == AbyssalDrain)
                {
                    if (gauge.Blood >= 50
                        && IsOffCooldown(LivingShadow)
                        && LevelChecked(LivingShadow))
                        return LivingShadow;

                    if (IsOffCooldown(SaltedEarth)
                        && LevelChecked(SaltedEarth))
                        return SaltedEarth;

                    if (IsOffCooldown(CarveAndSpit)
                        && LevelChecked(AbyssalDrain))
                        return actionID;

                    if (IsOffCooldown(SaltAndDarkness)
                        && HasEffect(Buffs.SaltedEarth)
                        && LevelChecked(SaltAndDarkness))
                        return SaltAndDarkness;

                    if (IsEnabled(CustomComboPreset.DRK_Shadowbringer_oGCD)
                        && GetCooldownRemainingTime(Shadowbringer) < 60
                        && LevelChecked(Shadowbringer)
                        && gauge.DarksideTimeRemaining > 0)
                        return Shadowbringer;
                }
                return actionID;
            }
        }
    }
}