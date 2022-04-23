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
            Reprisal = 7535,
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
            public const ushort
                Reprisal = 1193;
        }

        public static class Levels
        {
            public const byte
                HardSlash = 1,
                SyphonStrike = 2,
                Unleash = 6,
                Souleater = 26,
                FloodOfDarkness = 30,
                BloodWeapon = 35,
                EdgeOfDarkness = 40,
                SaltedEarth = 52,
                AbyssalDrain = 56,
                CarveAndSpit = 60,
                Bloodpiller = 62,
                Quietus = 64,
                Delirium = 68,
                StalwartSoul = 40,
                Shadow = 74,
                EdgeOfShadow = 74,
                LivingShadow = 80,
                SaltAndDarkness = 86,
                Shadowbringer = 90,
                Plunge = 54,
                Unmend = 15;
        }
        public static class Config
        {
            public const string
                DrkKeepPlungeCharges = "DrkKeepPlungeCharges";
            public const string
                DrkMPManagement = "DrkMPManagement";
        }
    }

    internal class DarkSouleaterCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkSouleaterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var plungeChargesRemaining = Service.Configuration.GetCustomIntValue(DRK.Config.DrkKeepPlungeCharges);
                var mpRemaining = Service.Configuration.GetCustomIntValue(DRK.Config.DrkMPManagement);

                if (IsEnabled(CustomComboPreset.DarkRangedUptimeFeature) && level >= DRK.Levels.Unmend)
                {
                    if (!InMeleeRange())
                        return DRK.Unmend;
                }

                if (InCombat())
                {
                    // oGCDs
                    if (CanWeave(actionID))
                    {
                        //Mana Features
                        if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature))
                        {
                            if (IsEnabled(CustomComboPreset.DarkEoSPoolOption) && gauge.ShadowTimeRemaining >= 1 && (gauge.HasDarkArts || LocalPlayer.CurrentMp > (mpRemaining + 3000)) && level >= DRK.Levels.EdgeOfDarkness && CanDelayedWeave(actionID))
                                return OriginalHook(DRK.EdgeOfDarkness);
                            if (LocalPlayer.CurrentMp > 8500 || gauge.DarksideTimeRemaining < 10)
                            {
                                if (level >= DRK.Levels.EdgeOfDarkness)
                                    return OriginalHook(DRK.EdgeOfDarkness);
                                if (level >= DRK.Levels.FloodOfDarkness && level < DRK.Levels.EdgeOfDarkness)
                                    return DRK.FloodOfDarkness;
                            }
                        }

                        //oGCD Features
                        if (gauge.DarksideTimeRemaining > 1)
                        {
                            if (IsEnabled(CustomComboPreset.DarkMainComboBuffsGroup))
                            {
                                if (IsEnabled(CustomComboPreset.DarkBloodWeaponOption) && IsOffCooldown(DRK.BloodWeapon) && level >= DRK.Levels.BloodWeapon)
                                    return DRK.BloodWeapon;
                                if (IsEnabled(CustomComboPreset.DarkDeliriumOnCD) && IsOffCooldown(DRK.Delirium) && level >= DRK.Levels.Delirium)
                                    return DRK.Delirium;
                            }

                            if (IsEnabled(CustomComboPreset.DarkMainComboCDsGroup))
                            {
                                if (IsEnabled(CustomComboPreset.DRKLivingShadowFeature) && gauge.Blood >= 50 && IsOffCooldown(DRK.LivingShadow) && level >= DRK.Levels.LivingShadow)
                                    return DRK.LivingShadow;
                                if (IsEnabled(CustomComboPreset.DarkSaltedEarthFeature) && level >= DRK.Levels.SaltedEarth)
                                {
                                    if ((IsOffCooldown(DRK.SaltedEarth) && !HasEffect(DRK.Buffs.SaltedEarth)) || //Salted Earth
                                        (HasEffect(DRK.Buffs.SaltedEarth) && IsOffCooldown(DRK.SaltAndDarkness) && IsOnCooldown(DRK.SaltedEarth) && level >= DRK.Levels.SaltAndDarkness)) //Salt and Darkness
                                        return OriginalHook(DRK.SaltedEarth);
                                }

                                if (level >= DRK.Levels.Shadowbringer && IsEnabled(CustomComboPreset.DarkShBFeature))
                                {
                                    if ((GetRemainingCharges(DRK.Shadowbringer) > 0 && IsNotEnabled(CustomComboPreset.DarkBurstShBOption)) ||
                                        (IsEnabled(CustomComboPreset.DarkBurstShBOption) && GetRemainingCharges(DRK.Shadowbringer) > 0 && gauge.ShadowTimeRemaining > 1 && IsOnCooldown(DRK.Delirium))) //burst feature
                                        return DRK.Shadowbringer;
                                }

                                if (IsEnabled(CustomComboPreset.DarkCnSFeature) && IsOffCooldown(DRK.CarveAndSpit) && level >= DRK.Levels.CarveAndSpit)
                                    return DRK.CarveAndSpit;
                                if (level >= DRK.Levels.Plunge && IsEnabled(CustomComboPreset.DarkPlungeFeature))
                                {
                                    if ((GetRemainingCharges(DRK.Plunge) > plungeChargesRemaining && IsNotEnabled(CustomComboPreset.DarkPlungeBurstOption)) ||
                                        (IsEnabled(CustomComboPreset.DarkPlungeBurstOption) && GetRemainingCharges(DRK.Plunge) > 0 && gauge.ShadowTimeRemaining > 1 && IsOnCooldown(DRK.Delirium))) //burst feature
                                        return DRK.Plunge;
                                }
                            }
                        }
                    }

                    //Delirium Features
                    if (level >= DRK.Levels.Delirium && IsEnabled(CustomComboPreset.DeliriumFeature) && IsEnabled(CustomComboPreset.DarkMainComboCDsGroup))
                    {
                        //Regular Delirium
                        if (GetBuffStacks(DRK.Buffs.Delirium) > 0 && (level < DRK.Levels.LivingShadow || IsNotEnabled(CustomComboPreset.DelayedDeliriumFeatureOption)))
                            return DRK.Bloodspiller;

                        //Delayed Delirium
                        if (IsEnabled(CustomComboPreset.DelayedDeliriumFeatureOption) && GetBuffStacks(DRK.Buffs.Delirium) > 0 &&
                            (GetBuffStacks(DRK.Buffs.BloodWeapon) is 0 or 1 or 2))
                            return DRK.Bloodspiller;

                        //Blood management before Delirium
                        if (IsEnabled(CustomComboPreset.DarkDeliriumOnCD) && ((gauge.Blood >= 50 && GetCooldownRemainingTime(DRK.BloodWeapon) < 6 && GetCooldownRemainingTime(DRK.Delirium) > 0) || (IsOffCooldown(DRK.Delirium) && gauge.Blood >= 50)))
                            return DRK.Bloodspiller;
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

                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.DarkManaOvercapAoEFeature) && level >= DRK.Levels.FloodOfDarkness && (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || gauge.DarksideTimeRemaining < 10))
                            return OriginalHook(DRK.FloodOfDarkness);
                    if (gauge.DarksideTimeRemaining > 0)
                    {
                        if (IsEnabled(CustomComboPreset.DRKStalwartabyssalDrainFeature) && level >= DRK.Levels.AbyssalDrain && IsOffCooldown(DRK.AbyssalDrain) && PlayerHealthPercentageHp() <= 60)
                            return DRK.AbyssalDrain;
                        if (IsEnabled(CustomComboPreset.DRKStalwartShadowbringerFeature) && level >= DRK.Levels.Shadowbringer && GetRemainingCharges(DRK.Shadowbringer) > 0)
                            return DRK.Shadowbringer;
                    }
                }

                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium) && gauge.DarksideTimeRemaining > 0)
                        return DRK.Quietus;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                    {
                        if (IsEnabled(CustomComboPreset.DRKOvercapFeature) && gauge.Blood >= 90 && level >= DRK.Levels.Quietus && gauge.DarksideTimeRemaining > 0)
                            return DRK.Quietus;
                        return DRK.StalwartSoul;
                    }
                }

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
            var gauge = GetJobGauge<DRKGauge>();

            if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
            {
                if (gauge.Blood >= 50 && IsOffCooldown(DRK.LivingShadow) && level >= DRK.Levels.LivingShadow)
                    return DRK.LivingShadow;
                if (IsOffCooldown(DRK.SaltedEarth) && level >= DRK.Levels.SaltedEarth)
                    return DRK.SaltedEarth;
                if (IsOffCooldown(DRK.CarveAndSpit))
                    return DRK.CarveAndSpit;
                if (IsOffCooldown(DRK.SaltAndDarkness) && HasEffect(DRK.Buffs.SaltedEarth) && level >= DRK.Levels.SaltAndDarkness)
                    return DRK.SaltAndDarkness;
                if (IsEnabled(CustomComboPreset.DarkShadowbringeroGCDFeature) && GetCooldownRemainingTime(DRK.Shadowbringer) < 60 && level >= DRK.Levels.Shadowbringer && gauge.DarksideTimeRemaining > 0)
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
    internal class DarkKnightReprisalProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkKnightReprisalProtection;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is DRK.Reprisal)
            {
                if (TargetHasEffectAny(DRK.Debuffs.Reprisal) && IsOffCooldown(DRK.Reprisal))
                    return WHM.Stone1;
            }
            return actionID;
        }
    }
}
