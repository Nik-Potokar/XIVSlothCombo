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
            Plunge = 3640;

        public static class Buffs
        {
            public const short
                BloodWeapon = 742,
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
                SyphonStrike = 2,
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
                SaltAndDarkness = 86,
                Shadowbringer = 90;
        }
    }

    internal class DarkSouleaterCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DarkSouleaterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                var currentMp = LocalPlayer.CurrentMp;
                var gcd = GetCooldown(DRK.HardSlash);
                var gauge = GetJobGauge<DRKGauge>();
                var deliriumTime = FindEffect(DRK.Buffs.Delirium);
                var bloodgauge = GetJobGauge<DRKGauge>().Blood;
                var shadowCooldown = GetCooldown(DRK.LivingShadow);
                var gcdCooldown1 = GetCooldown(DRK.HardSlash);
                var gcdCooldown2 = GetCooldown(DRK.SyphonStrike);
                var gcdCooldown3 = GetCooldown(DRK.Souleater);
                var darkSide = GetJobGauge<DRKGauge>().DarksideTimeRemaining;
                var plungeCD = GetCooldown(DRK.Plunge);
                var actionIDCD = GetCooldown(actionID);
                var livingshadowCD = GetCooldown(DRK.LivingShadow);
                var saltedCD = GetCooldown(DRK.SaltedEarth);
                var carveCD = GetCooldown(DRK.CarveAndSpit);
                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= DRK.Levels.Bloodpiller && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                        return DRK.Bloodspiller;
                }
                if (HasEffect(DRK.Buffs.Delirium) && deliriumTime.RemainingTime <= 10 && deliriumTime.RemainingTime > 0 && IsEnabled(CustomComboPreset.DeliriumFeatureOption))
                {
                    if (level >= DRK.Levels.Bloodpiller && level >= DRK.Levels.Delirium)
                        return DRK.Bloodspiller;
                }
                if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature))
                {
                    if (currentMp > 8500 || gauge.DarksideTimeRemaining < 10)
                    {
                        if (level >= DRK.Levels.EdgeOfShadow && gcd.CooldownRemaining > 0.7)
                            return DRK.EdgeOfShadow;
                        if (level >= DRK.Levels.FloodOfDarkness && level < DRK.Levels.EdgeOfDarkness && gcd.CooldownRemaining > 0.7)
                            return DRK.FloodOfDarkness;
                        if (level >= DRK.Levels.EdgeOfDarkness && gcd.CooldownRemaining > 0.7)
                            return DRK.EdgeOfDarkness;
                    }
                }
                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;

                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;
                }
                if (bloodgauge >= 50 && !shadowCooldown.IsCooldown && (double)gcdCooldown1.CooldownRemaining > 0.8 && level >= 80 && IsEnabled(CustomComboPreset.DRKLivingShadowFeature))
                {
                    return DRK.LivingShadow;
                }

                if (bloodgauge >= 50 && !shadowCooldown.IsCooldown && (double)gcdCooldown2.CooldownRemaining > 0.8 && level >= 80 && IsEnabled(CustomComboPreset.DRKLivingShadowFeature))
                {
                    return DRK.LivingShadow;
                }

                if (bloodgauge >= 50 && !shadowCooldown.IsCooldown && (double)gcdCooldown3.CooldownRemaining > 0.8 && level >= 80 && IsEnabled(CustomComboPreset.DRKLivingShadowFeature))
                {
                    return DRK.LivingShadow;
                }

                if (lastComboMove == DRK.Souleater && level >= DRK.Levels.Bloodpiller && bloodgauge >= 80)
                {
                    return DRK.Bloodspiller;
                }
                if (IsEnabled(CustomComboPreset.DarkPlungeFeature) && level >= 54)
                {
                    if (plungeCD.CooldownRemaining < 30 && actionIDCD.CooldownRemaining > 0.7)
                        return DRK.Plunge;
                }
                // leaves 1 stack
                if (IsEnabled(CustomComboPreset.DarkPlungeFeatureOption) && level >= 54)
                {
                    if (!plungeCD.IsCooldown && actionIDCD.CooldownRemaining > 0.7 && plungeCD.CooldownRemaining < 60)
                        return DRK.Plunge;
                }

                return DRK.HardSlash;
            }

            return actionID;
        }
    }

    internal class DarkStalwartSoulCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DarkStalwartSoulCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.StalwartSoul)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var deliriumTime = FindEffect(DRK.Buffs.Delirium);
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
                if (HasEffect(DRK.Buffs.Delirium) && deliriumTime.RemainingTime <= 10 && deliriumTime.RemainingTime > 0 && IsEnabled(CustomComboPreset.DeliriumFeatureOption))
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
        protected override CustomComboPreset Preset => CustomComboPreset.DarkoGCDFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
            {
                var shbCD = GetCooldown(DRK.Shadowbringer);
                var gauge = GetJobGauge<DRKGauge>();
                var livingshadowCD = GetCooldown(DRK.LivingShadow);
                var saltedCD = GetCooldown(DRK.SaltedEarth);
                var actionIDCD = GetCooldown(actionID);

                if (gauge.Blood >= 50 && !livingshadowCD.IsCooldown && level >= 80 )
                    return DRK.LivingShadow;
                if (!saltedCD.IsCooldown && level >= DRK.Levels.SaltedEarth)
                    return DRK.SaltedEarth;
                if (!actionIDCD.IsCooldown &&  level >= DRK.Levels.CarveAndSpit)
                    return actionID;
                if (HasEffect(DRK.Buffs.SaltedEarth) && level >= DRK.Levels.SaltAndDarkness)
                    return DRK.SaltAndDarkness;
                if (IsEnabled(CustomComboPreset.DarkShadowbringeroGCDFeature) && shbCD.CooldownRemaining < 60)
                    return DRK.Shadowbringer;

            }

            return OriginalHook(actionID);

        }
    }
}
