using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class DRK
    {
        public const byte JobID = 32;

        public const uint
            HardSlash = 3617,
            Unleash = 3621,
            SyphonStrike = 3623,
            Souleater = 3632,
            SaltedEarth = 3639,
            AbyssalDrain = 3641,
            CarveAndSpit = 3643,
            Delirium = 7390,
            Quietus = 7391,
            Bloodspiller = 7392,
            LowBlow = 7540,
            Interject = 7538,
            FloodOfDarkness = 16466,
            EdgeOfDarkness = 16467,
            StalwartSoul = 16468,
            FloodOfShadow = 16469,
            EdgeOfShadow = 16470,
            LivingShadow = 16472,
            SaltAndDarkness = 25755,
            Shadowbringer = 25757,
            Plunge = 3640,
            BloodWeapon = 3625,
            Unmend = 3624;

        public static class Buffs
        {
            public const ushort
                BloodWeapon = 742,
                Darkside = 751,
                BlackestNight = 1178,
                Delirium = 1972,
                SaltedEarth = 749;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                HardSlash = 1,
                SyphonStrike = 2,
                Unleash = 6,
                Souleater = 26,
                FloodOfDarkness = 30,
                EdgeOfDarkness = 40,
                SaltedEarth = 52,
                AbyssalDrain = 56,
                CarveAndSpit = 60,
                Bloodpiller = 62,
                Quietus = 64,
                Delirium = 68,
                StalwartSoul = 72,
                Shadow = 74,
                EdgeOfShadow = 74,
                LivingShadow = 80,
                SaltAndDarkness = 86,
                Shadowbringer = 90,
                Plunge = 54,
                Unmend = 15;
        }
    }

    internal class DarkSouleaterCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkSouleaterCombo;
        internal static bool inOpener = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                var currentMp = LocalPlayer.CurrentMp;
                var gauge = GetJobGauge<DRKGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (IsEnabled(CustomComboPreset.DarkRangedUptimeFeature) && level >= DRK.Levels.Unmend)
                {
                    if (!InMeleeRange(true))
                        return DRK.Unmend;
                }

                if (!incombat)
                {
                    if (IsEnabled(CustomComboPreset.DarkOpenerFeature) && level == 90)
                    {
                        if (HasEffectAny(DRK.Buffs.BloodWeapon) || HasEffect(DRK.Buffs.BlackestNight))
                            inOpener = true;
                    }

                    if (IsEnabled(CustomComboPreset.DarkBloodWeaponOpener) && inOpener)
                    {
                        if (IsOnCooldown(DRK.BloodWeapon))
                            return DRK.HardSlash;
                        if (IsEnabled(CustomComboPreset.DarkOpenerFeature) && inOpener && HasEffect(DRK.Buffs.BlackestNight))
                            return DRK.BloodWeapon;
                    }
                }

                if (incombat)
                {
                    if (IsEnabled(CustomComboPreset.DarkOpenerFeature) && inOpener && level == 90)
                    {
                        if (lastComboMove == DRK.Souleater && GetRemainingCharges(DRK.Plunge) == 0)
                        {
                            inOpener = false;
                        }

                        // oGCDs
                        if (CanWeave(actionID))
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) == 2)
                            {
                                if (IsOffCooldown(DRK.CarveAndSpit))
                                    return DRK.CarveAndSpit;
                                if (IsOnCooldown(DRK.CarveAndSpit))
                                    return DRK.Plunge;
                            }

                            if (GetBuffStacks(DRK.Buffs.Delirium) == 1)
                            {
                                if (GetRemainingCharges(DRK.Shadowbringer) == 1)
                                    return DRK.Shadowbringer;
                                if (GetRemainingCharges(DRK.Shadowbringer) == 0)
                                    return DRK.EdgeOfShadow;

                            }

                            if (lastComboMove == DRK.HardSlash)
                            {
                                if (gauge.Blood == 10 && HasEffect(DRK.Buffs.BloodWeapon) && IsOffCooldown(DRK.LivingShadow))
                                {
                                    if (gauge.DarksideTimeRemaining == 0)
                                        return DRK.EdgeOfShadow;
                                    if (gauge.DarksideTimeRemaining > 0)
                                        return DRK.Delirium;
                                }

                                if (IsOnCooldown(DRK.LivingShadow) && GetBuffStacks(DRK.Buffs.Delirium) == 3)
                                {
                                    if (GetRemainingCharges(DRK.Shadowbringer) == 2)
                                        return DRK.Shadowbringer;
                                    if (GetRemainingCharges(DRK.Shadowbringer) == 1)
                                        return DRK.EdgeOfShadow;
                                }

                                if (IsOnCooldown(DRK.Shadowbringer) && !HasEffect(DRK.Buffs.Delirium))
                                {
                                    if (IsOffCooldown(DRK.SaltAndDarkness))
                                        return DRK.SaltAndDarkness;
                                    if (IsOnCooldown(DRK.SaltAndDarkness))
                                        return DRK.EdgeOfShadow;
                                }

                            }

                            if (lastComboMove == DRK.Souleater)
                            {
                                if (IsOffCooldown(DRK.LivingShadow))
                                    return DRK.LivingShadow;
                                if (IsOnCooldown(DRK.LivingShadow))
                                    return DRK.SaltedEarth;
                            }

                            if (lastComboMove == DRK.SyphonStrike && GetRemainingCharges(DRK.Shadowbringer) == 0)
                            {
                                if (GetRemainingCharges(DRK.Plunge) == 1)
                                    return DRK.Plunge;
                                if (GetRemainingCharges(DRK.Plunge) == 0)
                                    return DRK.EdgeOfShadow;
                            }
                        }

                        // GCDs
                        if (lastComboMove == DRK.HardSlash)
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) > 0 && GetRemainingCharges(DRK.Shadowbringer) < 2)
                                return DRK.Bloodspiller;
                            return DRK.SyphonStrike;
                        }

                        if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                            return DRK.Souleater;
                    }

                    if (!inOpener)
                    {
                        //Delirium Features
                        if (level >= DRK.Levels.Delirium && IsEnabled(CustomComboPreset.DeliriumFeature))
                        {
                            if (IsEnabled(CustomComboPreset.DelayedDeliriumFeatureOption) && HasEffect(DRK.Buffs.Delirium) && GetBuffRemainingTime(DRK.Buffs.Delirium) <= 10 && GetBuffRemainingTime(DRK.Buffs.Delirium) > 0)
                                return DRK.Bloodspiller;

                            if (GetBuffStacks(DRK.Buffs.Delirium) > 0 && IsNotEnabled(CustomComboPreset.DelayedDeliriumFeatureOption))
                                return DRK.Bloodspiller;
                        }

                        // oGCDs
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.DRKLivingShadowFeature) && gauge.Blood >= 50 && IsOffCooldown(DRK.LivingShadow) && level >= DRK.Levels.LivingShadow)
                                return DRK.LivingShadow;

                            if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature) && (currentMp > 8500 || gauge.DarksideTimeRemaining < 10))
                            {
                                if (level >= DRK.Levels.EdgeOfDarkness)
                                    return OriginalHook(DRK.EdgeOfDarkness);
                                if (level >= DRK.Levels.FloodOfDarkness && level < DRK.Levels.EdgeOfDarkness)
                                    return DRK.FloodOfDarkness;
                            }

                            if (level >= DRK.Levels.Plunge &&
                                ((IsEnabled(CustomComboPreset.DarkPlungeFeature) && GetRemainingCharges(DRK.Plunge) > 0) || //leave 0 stacks
                                (IsEnabled(CustomComboPreset.DarkPlungeFeatureOption) && GetRemainingCharges(DRK.Plunge) > 1))) //leaves 1 stack
                                    return DRK.Plunge;
                        }
                        // 1-2-3 combo
                        if (comboTime > 0)
                        {
                            if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                                return DRK.SyphonStrike;

                            if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                            {
                                if (IsEnabled(CustomComboPreset.DarkBloodGaugeOvercapFeature) && level >= DRK.Levels.Bloodpiller && gauge.Blood >= 90)
                                    return DRK.Bloodspiller;
                                return DRK.Souleater;
                            }
                        }
                    }
                }

                return DRK.HardSlash;
            }

            return actionID;
        }
    }

    internal class DarkStalwartSoulCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkStalwartSoulCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.StalwartSoul)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var deliriumTime = FindEffect(DRK.Buffs.Delirium);
                var actionIDCD = GetCooldown(actionID);
                if (IsEnabled(CustomComboPreset.DarkManaOvercapAoEFeature))
                {
                    if (LocalPlayer.CurrentMp > 8500 || gauge.DarksideTimeRemaining < 10)
                    {
                        var gcd = GetCooldown(actionID);
                        if (level >= DRK.Levels.FloodOfDarkness && gcd.IsCooldown)
                            return OriginalHook(DRK.FloodOfDarkness);

                    }
                }
                if (IsEnabled(CustomComboPreset.DRKStalwartabyssalDrainFeature) && level >= DRK.Levels.AbyssalDrain)
                {
                    if (actionIDCD.IsCooldown && IsOffCooldown(DRK.AbyssalDrain) && PlayerHealthPercentageHp() <= 60)
                        return DRK.AbyssalDrain;
                }
                if (IsEnabled(CustomComboPreset.DRKStalwartShadowbringerFeature) && level >= DRK.Levels.Shadowbringer)
                {
                    if (actionIDCD.IsCooldown && GetRemainingCharges(DRK.Shadowbringer) > 0 && gauge.DarksideTimeRemaining > 0)
                        return DRK.Shadowbringer;
                }
                if (IsEnabled(CustomComboPreset.DRKOvercapFeature))
                {
                    if (lastComboMove == DRK.Unleash && gauge.Blood >= 90)
                        return DRK.Quietus;
                }
                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= DRK.Levels.Quietus && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                        return DRK.Quietus;
                }
                if (HasEffect(DRK.Buffs.Delirium) && deliriumTime.RemainingTime <= 10 && deliriumTime.RemainingTime > 0 && IsEnabled(CustomComboPreset.DelayedDeliriumFeatureOption))
                {
                    if (level >= DRK.Levels.Quietus && level >= DRK.Levels.Delirium)
                        return DRK.Quietus;
                }

                if (comboTime > 0 && lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                    return DRK.StalwartSoul;

                return DRK.Unleash;
            }

            return actionID;
        }
    }
    internal class DarkoGCDFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkoGCDFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
            {
                var shbCD = GetCooldown(DRK.Shadowbringer);
                var gauge = GetJobGauge<DRKGauge>();
                var livingshadowCD = GetCooldown(DRK.LivingShadow);
                var saltedCD = GetCooldown(DRK.SaltedEarth);
                var actionIDCD = GetCooldown(actionID);

                if (gauge.Blood >= 50 && !livingshadowCD.IsCooldown && level >= DRK.Levels.LivingShadow)
                    return DRK.LivingShadow;
                if (!saltedCD.IsCooldown && level >= DRK.Levels.SaltedEarth)
                    return DRK.SaltedEarth;
                if (!actionIDCD.IsCooldown && level >= DRK.Levels.CarveAndSpit)
                    return actionID;
                if (HasEffect(DRK.Buffs.SaltedEarth) && level >= DRK.Levels.SaltAndDarkness)
                    return DRK.SaltAndDarkness;
                if (IsEnabled(CustomComboPreset.DarkShadowbringeroGCDFeature) && shbCD.CooldownRemaining < 60)
                    return DRK.Shadowbringer;

            }

            return actionID;

        }
    }
    internal class DarkKnightInterruptFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkKnightInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.LowBlow)
            {
                var interjectCD = GetCooldown(DRK.Interject);
                var lowBlowCD = GetCooldown(DRK.LowBlow);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return DRK.Interject;
            }

            return actionID;
        }
    }
}
