using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class DRK
    {
        public const byte JobID = 32;

        public const uint
            HardSlash = 3617,
            Unleash = 3621,
            SyphonStrike = 3623,
            Souleater = 3632,
            Quietus = 7391,
            Bloodspiller = 7392,
            FloodOfDarkness = 16466,
            EdgeOfDarkness = 16467,
            StalwartSoul = 16468,
            FloodOfShadow = 16469,
            LivingShadow = 16472,
            EdgeOfShadow = 16470;

        public static class Buffs
        {
            public const short
                BloodWeapon = 742,
                Delirium = 1972;
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
                Bloodpiller = 62,
                Quietus = 64,
                Delirium = 68,
                StalwartSoul = 72,
                EdgeOfShadow = 74,
                Shadow = 74;

        }
    }

    internal class DarkSouleaterCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DarkSouleaterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= DRK.Levels.Bloodpiller && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                        return DRK.Bloodspiller;
                }

                if (comboTime > 0)
                {
                    var currentMp = LocalPlayer.CurrentMp;
                    var gcd = GetCooldown(DRK.HardSlash);

                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;
                        if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature))
                        {
                            if (currentMp > 8000)
                            {
                            if (level >= DRK.Levels.EdgeOfShadow && gcd.CooldownRemaining > 0.7)
                                return DRK.EdgeOfShadow;
                            if (level >= DRK.Levels.FloodOfDarkness && level < DRK.Levels.EdgeOfDarkness && gcd.CooldownRemaining > 0.7)
                                return DRK.FloodOfDarkness;
                            if (level >= DRK.Levels.EdgeOfDarkness && gcd.CooldownRemaining > 0.7)
                                return DRK.EdgeOfDarkness;

                            }
                        }
                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;
                }
                var bloodgauge = GetJobGauge<DRKGauge>().Blood;
                var shadowCooldown = GetCooldown(DRK.LivingShadow);
                var gcdCooldown1 = GetCooldown(DRK.HardSlash);
                var gcdCooldown2 = GetCooldown(DRK.SyphonStrike);
                var gcdCooldown3 = GetCooldown(DRK.Souleater);
                var darkSide = GetJobGauge<DRKGauge>().DarksideTimeRemaining; 

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
                if (IsEnabled(CustomComboPreset.DRKOvercapFeature))
                {
                    var gauge = GetJobGauge<DRKGauge>();
                    if (gauge.Blood >= 90 && HasEffect(DRK.Buffs.BloodWeapon))
                        return DRK.Quietus;
                }
                if (IsEnabled(CustomComboPreset.DeliriumFeature))
                {
                    if (level >= DRK.Levels.Quietus && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                        return DRK.Quietus;
                }

                if (comboTime > 0 && lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                    return DRK.StalwartSoul;

                return DRK.Unleash;
            }

            return actionID;
        }
    }
}
