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
                Placeholder = 1;
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
                DrkKeepPlungeCharges = "DrkKeepPlungeCharges",
                DrkMPManagement = "DrkMPManagement";
        }
    

    internal class DarkSouleaterCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkSouleaterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == Souleater)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var plungeChargesRemaining = Service.Configuration.GetCustomIntValue(Config.DrkKeepPlungeCharges);
                var mpRemaining = Service.Configuration.GetCustomIntValue(Config.DrkMPManagement);

                if (IsEnabled(CustomComboPreset.DarkRangedUptimeFeature) && level >= Levels.Unmend)
                {
                    if (!InMeleeRange())
                        return Unmend;
                }

                if (InCombat())
                {
                    // oGCDs
                    if (CanWeave(actionID))
                    {
                        //Mana Features
                        if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature))
                        {
                            if (IsEnabled(CustomComboPreset.DarkEoSPoolOption) && gauge.ShadowTimeRemaining >= 1 && (gauge.HasDarkArts || LocalPlayer.CurrentMp > (mpRemaining + 3000)) && level >= Levels.EdgeOfDarkness && CanDelayedWeave(actionID))
                                return OriginalHook(EdgeOfDarkness);
                            if (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || gauge.DarksideTimeRemaining < 10)
                            {
                                if (level >= Levels.EdgeOfDarkness)
                                    return OriginalHook(EdgeOfDarkness);
                                if (level is >= Levels.FloodOfDarkness and < Levels.EdgeOfDarkness)
                                    return FloodOfDarkness;
                            }
                        }

                        //oGCD Features
                        if (gauge.DarksideTimeRemaining > 1)
                        {
                            if (IsEnabled(CustomComboPreset.DarkMainComboBuffsGroup))
                            {
                                if (IsEnabled(CustomComboPreset.DarkBloodWeaponOption) && IsOffCooldown(BloodWeapon) && level >= Levels.BloodWeapon)
                                    return BloodWeapon;
                                if (IsEnabled(CustomComboPreset.DarkDeliriumOnCD) && IsOffCooldown(Delirium) && level >= Levels.Delirium)
                                    return Delirium;
                            }

                            if (IsEnabled(CustomComboPreset.DarkMainComboCDsGroup))
                            {
                                if (IsEnabled(CustomComboPreset.DRKLivingShadowFeature) && gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && level >= Levels.LivingShadow)
                                    return LivingShadow;
                                if (IsEnabled(CustomComboPreset.DarkSaltedEarthFeature) && level >= Levels.SaltedEarth)
                                {
                                    if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                        (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && level >= Levels.SaltAndDarkness) && GetBuffRemainingTime(Buffs.SaltedEarth) < 9) //Salt and Darkness
                                        return OriginalHook(SaltedEarth);
                                }

                                if (level >= Levels.Shadowbringer && IsEnabled(CustomComboPreset.DarkShBFeature))
                                {
                                    if ((GetRemainingCharges(Shadowbringer) > 0 && IsNotEnabled(CustomComboPreset.DarkBurstShBOption)) ||
                                        (IsEnabled(CustomComboPreset.DarkBurstShBOption) && GetRemainingCharges(Shadowbringer) > 0 && gauge.ShadowTimeRemaining > 1 && IsOnCooldown(Delirium))) //burst feature
                                        return Shadowbringer;
                                }

                                if (IsEnabled(CustomComboPreset.DarkCnSFeature) && IsOffCooldown(CarveAndSpit) && level >= Levels.CarveAndSpit)
                                    return CarveAndSpit;
                                if (level >= Levels.Plunge && IsEnabled(CustomComboPreset.DarkPlungeFeature) && GetRemainingCharges(Plunge) > plungeChargesRemaining)
                                {
                                    if (IsNotEnabled(CustomComboPreset.DarkMeleePlungeOption) ||
                                        (IsEnabled(CustomComboPreset.DarkMeleePlungeOption) && GetCooldownRemainingTime(Delirium) >= 45 && GetTargetDistance() <= 1))
                                        return Plunge;
                                }
                            }
                        }
                    }

                    //Delirium Features
                    if (level >= Levels.Delirium && IsEnabled(CustomComboPreset.DeliriumFeature) && IsEnabled(CustomComboPreset.DarkMainComboCDsGroup))
                    {
                        //Regular Delirium
                        if (GetBuffStacks(Buffs.Delirium) > 0 && (level < Levels.LivingShadow || IsNotEnabled(CustomComboPreset.DelayedDeliriumFeatureOption)))
                            return Bloodspiller;

                        //Delayed Delirium
                        if (IsEnabled(CustomComboPreset.DelayedDeliriumFeatureOption) && GetBuffStacks(Buffs.Delirium) > 0 &&
                            (GetBuffStacks(Buffs.BloodWeapon) is 0 or 1 or 2))
                            return Bloodspiller;

                        //Blood management before Delirium
                        if (IsEnabled(CustomComboPreset.DarkDeliriumOnCD) && ((gauge.Blood >= 50 && GetCooldownRemainingTime(BloodWeapon) < 6 && GetCooldownRemainingTime(Delirium) > 0) || (IsOffCooldown(Delirium) && gauge.Blood >= 50)))
                            return Bloodspiller;
                    }

                    // 1-2-3 combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove == HardSlash && level >= Levels.SyphonStrike)
                            return SyphonStrike;

                        if (lastComboMove == SyphonStrike && level >= Levels.Souleater)
                        {
                            if (IsEnabled(CustomComboPreset.DarkBloodGaugeOvercapFeature) && level >= Levels.Bloodpiller && gauge.Blood >= 90)
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

    internal class DarkStalwartSoulCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkStalwartSoulCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == StalwartSoul)
            {
                var gauge = GetJobGauge<DRKGauge>();

                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.DarkManaOvercapAoEFeature) && level >= Levels.FloodOfDarkness && (gauge.HasDarkArts || LocalPlayer.CurrentMp > 8500 || gauge.DarksideTimeRemaining < 10))
                            return OriginalHook(FloodOfDarkness);
                    if (gauge.DarksideTimeRemaining > 1)
                    {
                        if (IsEnabled(CustomComboPreset.DarkBloodWeaponAOEOption) && IsOffCooldown(BloodWeapon) && level >= Levels.BloodWeapon)
                            return BloodWeapon;
                        if (IsEnabled(CustomComboPreset.DarkDeliriumAOEOption) && IsOffCooldown(Delirium) && level >= Levels.Delirium)
                            return Delirium;
                        if (IsEnabled(CustomComboPreset.DarkLivingShadowAOEOption) && gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && level >= Levels.LivingShadow)
                            return LivingShadow;
                        if (IsEnabled(CustomComboPreset.DarkSaltedEarthAOEOption) && level >= Levels.SaltedEarth)
                        {
                            if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                (HasEffect(Buffs.SaltedEarth) && IsOffCooldown(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && level >= Levels.SaltAndDarkness)) //Salt and Darkness
                                return OriginalHook(SaltedEarth);
                        }

                        if (IsEnabled(CustomComboPreset.DRKStalwartabyssalDrainFeature) && level >= Levels.AbyssalDrain && IsOffCooldown(AbyssalDrain) && PlayerHealthPercentageHp() <= 60)
                            return AbyssalDrain;
                        if (IsEnabled(CustomComboPreset.DRKStalwartShadowbringerFeature) && level >= Levels.Shadowbringer && GetRemainingCharges(Shadowbringer) > 0)
                            return Shadowbringer;
                    }
                }

                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= Levels.Delirium && HasEffect(Buffs.Delirium) && gauge.DarksideTimeRemaining > 0)
                        return Quietus;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == Unleash && level >= Levels.StalwartSoul)
                    {
                        if (IsEnabled(CustomComboPreset.DRKOvercapFeature) && gauge.Blood >= 90 && level >= Levels.Quietus && gauge.DarksideTimeRemaining > 0)
                            return Quietus;
                        return StalwartSoul;
                    }
                }

                return Unleash;
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

                if (actionID == CarveAndSpit || actionID == AbyssalDrain)
                {
                    if (gauge.Blood >= 50 && IsOffCooldown(LivingShadow) && level >= Levels.LivingShadow)
                        return LivingShadow;
                    if (IsOffCooldown(SaltedEarth) && level >= Levels.SaltedEarth)
                        return SaltedEarth;
                    if (IsOffCooldown(CarveAndSpit) && level >= Levels.AbyssalDrain)
                        return actionID;
                    if (IsOffCooldown(SaltAndDarkness) && HasEffect(Buffs.SaltedEarth) && level >= Levels.SaltAndDarkness)
                        return SaltAndDarkness;
                    if (IsEnabled(CustomComboPreset.DarkShadowbringeroGCDFeature) && GetCooldownRemainingTime(Shadowbringer) < 60 && level >= Levels.Shadowbringer && gauge.DarksideTimeRemaining > 0)
                        return Shadowbringer;
                }
                return actionID;
            }
        }
    }
}
