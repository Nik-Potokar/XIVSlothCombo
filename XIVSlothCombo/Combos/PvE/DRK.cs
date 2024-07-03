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
            BloodWeapon = 7390,       // todo: remove this
            Delirium = 7390,          // still the same :D

            // Burst Window
            LivingShadow = 16472,
            Shadowbringer = 25757,
            Disesteem = 36932,

            // Gap-Closer
            Plunge = 38512,          // todo: remove this
            ShadowStride = 38512,    // todo: take this out of rotation

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
                DRK_KeepPlungeCharges = "DrkKeepPlungeCharges",
                DRK_MPManagement = "DrkMPManagement",
                DRK_VariantCure = "DRKVariantCure";
        }

        internal class DRK_SouleaterCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_SouleaterCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Souleater)
                {
                    var gauge = GetJobGauge<DRKGauge>();
                    var plungeChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.DRK_KeepPlungeCharges);
                    var mpRemaining = PluginConfiguration.GetCustomIntValue(Config.DRK_MPManagement);

                    if (IsEnabled(CustomComboPreset.DRK_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRK_RangedUptime) && LevelChecked(Unmend) && !InMeleeRange() && HasBattleTarget())
                        return Unmend;

                    if (InCombat())
                    {
                        // oGCDs
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            //Mana Features
                            if (IsEnabled(CustomComboPreset.DRK_ManaOvercap))
                            {
                                if ((CombatEngageDuration().TotalSeconds < 7 && gauge.DarksideTimeRemaining == 0) || CombatEngageDuration().TotalSeconds >= 6)
                                {
                                    if (IsEnabled(CustomComboPreset.DRK_EoSPooling) && GetCooldownRemainingTime(Delirium) >= 40 && (gauge.HasDarkArts || LocalPlayer.CurrentMp > (mpRemaining + 3000)) && LevelChecked(EdgeOfDarkness) && CanDelayedWeave(actionID))
                                        return OriginalHook(EdgeOfDarkness);
                                    if (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || (gauge.DarksideTimeRemaining < 10000 && LocalPlayer.CurrentMp >= 3000))
                                    {
                                        if (LevelChecked(EdgeOfDarkness))
                                            return OriginalHook(EdgeOfDarkness);
                                        if (LevelChecked(FloodOfDarkness) && !LevelChecked(EdgeOfDarkness))
                                            return FloodOfDarkness;
                                    }
                                }
                            }

                            //oGCD Features
                            if (gauge.DarksideTimeRemaining > 1)
                            {
                                if (IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group) && IsEnabled(CustomComboPreset.DRK_LivingShadow) && gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && LevelChecked(LivingShadow))
                                    return LivingShadow;

                                if (IsEnabled(CustomComboPreset.DRK_MainComboBuffs_Group))
                                {
                                    if (IsEnabled(CustomComboPreset.DRK_BloodWeapon) && IsOffCooldown(BloodWeapon) && LevelChecked(BloodWeapon))
                                        return BloodWeapon;
                                    if (IsEnabled(CustomComboPreset.DRK_Delirium) && IsOffCooldown(Delirium) && LevelChecked(Delirium))
                                        return Delirium;
                                }

                                if (IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group))
                                {

                                    if (IsEnabled(CustomComboPreset.DRK_SaltedEarth) && LevelChecked(SaltedEarth))
                                    {
                                        if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                            (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && LevelChecked(SaltAndDarkness) && GetBuffRemainingTime(Buffs.SaltedEarth) < 9)) //Salt and Darkness
                                            return OriginalHook(SaltedEarth);
                                    }

                                    if (LevelChecked(Shadowbringer) && IsEnabled(CustomComboPreset.DRK_Shadowbringer))
                                    {
                                        if ((GetRemainingCharges(Shadowbringer) > 0 && IsNotEnabled(CustomComboPreset.DRK_ShadowbringerBurst)) ||
                                            (IsEnabled(CustomComboPreset.DRK_ShadowbringerBurst) && GetRemainingCharges(Shadowbringer) > 0 && gauge.ShadowTimeRemaining > 1 && IsOnCooldown(Delirium))) //burst feature
                                            return Shadowbringer;
                                    }

                                    if (IsEnabled(CustomComboPreset.DRK_CarveAndSpit) && IsOffCooldown(CarveAndSpit) && LevelChecked(CarveAndSpit))
                                        return CarveAndSpit;
                                    if (LevelChecked(Plunge) && IsEnabled(CustomComboPreset.DRK_Plunge) && GetRemainingCharges(Plunge) > plungeChargesRemaining)
                                    {
                                        if (IsNotEnabled(CustomComboPreset.DRK_MeleePlunge) ||
                                            (IsEnabled(CustomComboPreset.DRK_MeleePlunge) && GetTargetDistance() <= 1 &&
                                            ((GetMaxCharges(Plunge) > 1 && GetCooldownRemainingTime(Delirium) >= 45) ||
                                            GetMaxCharges(Plunge) == 1)))
                                            return Plunge;
                                    }
                                }
                            }
                        }

                        //Delirium Features
                        if (LevelChecked(Delirium) && IsEnabled(CustomComboPreset.DRK_Bloodspiller) && IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group))
                        {
                            //Regular Delirium
                            if (GetBuffStacks(Buffs.Delirium) > 0 && IsNotEnabled(CustomComboPreset.DRK_DelayedBloodspiller))
                                return Bloodspiller;

                            //Delayed Delirium
                            if (IsEnabled(CustomComboPreset.DRK_DelayedBloodspiller) && GetBuffStacks(Buffs.Delirium) > 0 && IsOnCooldown(BloodWeapon) && GetBuffStacks(Buffs.BloodWeapon) < 2)
                                return Bloodspiller;

                            //Blood management before Delirium
                            if (IsEnabled(CustomComboPreset.DRK_Delirium) &&
                                ((gauge.Blood >= 60 && GetCooldownRemainingTime(BloodWeapon) is > 0 and < 3) || (gauge.Blood >= 50 && GetCooldownRemainingTime(Delirium) > 37 && !HasEffect(Buffs.Delirium))))
                                return Bloodspiller;
                        }

                        // 1-2-3 combo
                        if (comboTime > 0)
                        {
                            if (lastComboMove == HardSlash && LevelChecked(SyphonStrike))
                                return SyphonStrike;
                            if (lastComboMove == SyphonStrike && LevelChecked(Souleater))
                            {
                                if (IsEnabled(CustomComboPreset.DRK_BloodGaugeOvercap) && LevelChecked(Bloodspiller) && gauge.Blood >= 90)
                                    return Bloodspiller;
                                return Souleater;
                            }
                        }
                    }

                    return HardSlash;
                }

                return actionID;
            }
        }

        internal class DRK_StalwartSoulCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_StalwartSoulCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StalwartSoul)
                {
                    var gauge = GetJobGauge<DRKGauge>();

                    if (IsEnabled(CustomComboPreset.DRK_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                        return Variant.VariantCure;


                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        if (IsEnabled(CustomComboPreset.DRK_AoE_ManaOvercap) && LevelChecked(FloodOfDarkness) && (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || (gauge.DarksideTimeRemaining < 10 && LocalPlayer.CurrentMp >= 3000)))
                            return OriginalHook(FloodOfDarkness);
                        if (gauge.DarksideTimeRemaining > 1)
                        {
                            if (IsEnabled(CustomComboPreset.DRK_AoE_BloodWeapon) && IsOffCooldown(BloodWeapon) && LevelChecked(BloodWeapon))
                                return BloodWeapon;
                            if (IsEnabled(CustomComboPreset.DRK_AoE_Delirium) && IsOffCooldown(Delirium) && LevelChecked(Delirium))
                                return Delirium;
                            if (IsEnabled(CustomComboPreset.DRK_AoE_LivingShadow) && gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && LevelChecked(LivingShadow))
                                return LivingShadow;
                            if (IsEnabled(CustomComboPreset.DRK_AoE_SaltedEarth) && LevelChecked(SaltedEarth))
                            {
                                if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                    (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && LevelChecked(SaltAndDarkness))) //Salt and Darkness
                                    return OriginalHook(SaltedEarth);
                            }

                            if (IsEnabled(CustomComboPreset.DRK_AoE_AbyssalDrain) && LevelChecked(AbyssalDrain) && IsOffCooldown(AbyssalDrain) && PlayerHealthPercentageHp() <= 60)
                                return AbyssalDrain;
                            if (IsEnabled(CustomComboPreset.DRK_AoE_Shadowbringer) && LevelChecked(Shadowbringer) && GetRemainingCharges(Shadowbringer) > 0)
                                return Shadowbringer;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.DRK_Delirium))
                    {
                        if (LevelChecked(Delirium) && HasEffect(Buffs.Delirium) && gauge.DarksideTimeRemaining > 0)
                            return Quietus;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Unleash && LevelChecked(StalwartSoul))
                        {
                            if (IsEnabled(CustomComboPreset.DRK_Overcap) && gauge.Blood >= 90 && LevelChecked(Quietus))
                                return Quietus;
                            return StalwartSoul;
                        }
                    }

                    return Unleash;
                }

                return actionID;
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
                    if (gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && LevelChecked(LivingShadow))
                        return LivingShadow;
                    if (IsOffCooldown(SaltedEarth) && LevelChecked(SaltedEarth))
                        return SaltedEarth;
                    if (IsOffCooldown(CarveAndSpit) && LevelChecked(AbyssalDrain))
                        return actionID;
                    if (IsOffCooldown(SaltAndDarkness) && HasEffect(Buffs.SaltedEarth) && LevelChecked(SaltAndDarkness))
                        return SaltAndDarkness;
                    if (IsEnabled(CustomComboPreset.DRK_Shadowbringer_oGCD) && GetCooldownRemainingTime(Shadowbringer) < 60 && LevelChecked(Shadowbringer) && gauge.DarksideTimeRemaining > 0)
                        return Shadowbringer;
                }
                return actionID;
            }
        }
    }
}