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
            BloodWeapon = 3625,
            SaltedEarth = 3639,
            AbyssalDrain = 3641,
            CarveAndSpit = 3643,
            Quietus = 7391,
            Delirium = 7390,
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
            Unmend = 3624;

        public static class Buffs
        {
            public const ushort
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

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                var currentMp = LocalPlayer.CurrentMp;
                var gauge = GetJobGauge<DRKGauge>();
                var deliriumTime = FindEffect(DRK.Buffs.Delirium);
                var bloodgauge = GetJobGauge<DRKGauge>().Blood;
                var plungeCD = GetCooldown(DRK.Plunge);
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                // Uptime protection
                if (IsEnabled(CustomComboPreset.DarkRangedUptimeFeature) && level >= DRK.Levels.Unmend)
                {
                    if (!InMeleeRange(true))
                        return DRK.Unmend;
                }

                //Adds BloodWeapon both as an opener option only, and on main combo.
                //DRK opens with Bloodweapon, but it can feel awkward which is why it's an optional feature.
                //If you turn the Bloodweapon opener feature off, you will have to use bloodweapon yourself the first time, but after that it'll appear on the main combo
                if (IsOffCooldown(DRK.BloodWeapon))
                {
                    if (!inCombat && IsEnabled(CustomComboPreset.DarkKnightBloodweaponOpenerFeature))
                        return DRK.BloodWeapon;

                    if (inCombat && IsEnabled(CustomComboPreset.DarkKnightBloodweaponFeature))
                        return DRK.BloodWeapon;
                }

                //Adds Delirium on main combo and instantly spend bloodspillers. Not optimal in raid environments due to buff alignments.
                if (IsEnabled(CustomComboPreset.DeliriumFeature) && level >= DRK.Levels.Delirium)
                {
                    if (IsOffCooldown(DRK.Delirium) && CanWeave(actionID))
                        return DRK.Delirium;
                    if (level >= DRK.Levels.Bloodpiller && HasEffect(DRK.Buffs.Delirium))
                        return DRK.Bloodspiller;
                }

                //Adds Delirium on main combo and spend bloodspillers after 5 seconds. Should be optimal in raid environments due to buff alignments.
                if (IsEnabled(CustomComboPreset.DeliriumFeatureOption) && level >= DRK.Levels.Delirium)
                {
                    if (IsOffCooldown(DRK.Delirium) && CanWeave(actionID))
                        return DRK.Delirium;
                    if (level >= DRK.Levels.Bloodpiller && HasEffect(DRK.Buffs.Delirium) && deliriumTime.RemainingTime <= 10)
                        return DRK.Bloodspiller;
                }

                // Adds Living Shadow on main combo.
                if (bloodgauge >= 50 && IsOffCooldown(DRK.LivingShadow) && CanWeave(actionID) && level >= DRK.Levels.LivingShadow && IsEnabled(CustomComboPreset.DRKLivingShadowFeature))
                {
                    return DRK.LivingShadow;
                }

                // Adds mana overcap protection and Darkside uptime
                if (IsEnabled(CustomComboPreset.DarkManaOvercapFeature))
                {
                    if (currentMp > 8500 || gauge.DarksideTimeRemaining < 10)
                    {
                        if (level >= DRK.Levels.EdgeOfShadow && CanWeave(actionID))
                            return DRK.EdgeOfShadow;
                        if (level >= DRK.Levels.FloodOfDarkness && level < DRK.Levels.EdgeOfDarkness && CanWeave(actionID))
                            return DRK.FloodOfDarkness;
                        if (level >= DRK.Levels.EdgeOfDarkness && CanWeave(actionID))
                            return DRK.EdgeOfDarkness;
                    }
                }

                if (IsEnabled(CustomComboPreset.DarkKnightogcdFeature))
                {
                    if (IsOffCooldown(DRK.SaltedEarth) && level >= DRK.Levels.SaltedEarth && CanWeave(actionID))
                        return DRK.SaltedEarth;
                    if (IsOffCooldown(DRK.CarveAndSpit) && level >= DRK.Levels.CarveAndSpit && CanWeave(actionID))
                        return DRK.CarveAndSpit;
                    if (IsOffCooldown(DRK.SaltAndDarkness) && HasEffect(DRK.Buffs.SaltedEarth) && level >= DRK.Levels.SaltAndDarkness && CanWeave(actionID))
                        return DRK.SaltAndDarkness;
                    if (level >= DRK.Levels.Shadowbringer && CanWeave(actionID) && GetRemainingCharges(DRK.Shadowbringer) >= 1)
                        return DRK.Shadowbringer;
                }
                // Adds Blood Gauge Overcap protection on main combo.
                if (IsEnabled(CustomComboPreset.DarkBloodGaugeOvercapFeature) && level >= DRK.Levels.Bloodpiller)
                {
                    if (lastComboMove == DRK.Souleater && level >= DRK.Levels.Bloodpiller && bloodgauge >= 80)
                        return DRK.Bloodspiller;
                }

                // leaves 0 stacks of plunge on main combo
                if (IsEnabled(CustomComboPreset.DarkPlungeFeature) && level >= DRK.Levels.Plunge)
                {
                    if (plungeCD.CooldownRemaining < 30 && CanWeave(actionID))
                        return DRK.Plunge;
                }

                // leaves 1 stack of plunge on main combo
                if (IsEnabled(CustomComboPreset.DarkPlungeFeatureOption) && level >= DRK.Levels.Plunge)
                {
                    if (!plungeCD.IsCooldown && CanWeave(actionID) && plungeCD.CooldownRemaining < 60)
                        return DRK.Plunge;
                }

                // Regular 1-2-3 combo
                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;

                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;
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
