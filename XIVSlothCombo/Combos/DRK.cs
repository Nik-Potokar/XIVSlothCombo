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
            BlackestNight = 7393,
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

    internal class DarkSimple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkSimple;
        internal static bool inOpener = false;
        internal static bool openerFinished = false;
        internal static byte step = 0;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRK.Souleater)
            {
                var canWeave = CanWeave(actionID);
                var currentMp = LocalPlayer.CurrentMp;
                var gauge = GetJobGauge<DRKGauge>();
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (IsEnabled(CustomComboPreset.DarkSimpleOpener) && level == 90)
                {
                    if (IsOnCooldown(DRK.BloodWeapon) || IsOnCooldown(DRK.BlackestNight))
                    {
                        inOpener = true;
                    }

                    if (!inOpener)
                    {
                        //Delirium Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleDelirium))
                            {
                                if (
                                    level >= DRK.Levels.Delirium &&
                                    IsOffCooldown(DRK.Delirium) && CanWeave(actionID, 0.3)
                                   ) return DRK.Delirium;
                            }
                        }

                        //Blood Weapon Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleBloodWeapon))
                            {
                                if (
                                    level >= DRK.Levels.BloodWeapon &&
                                    GetCooldownRemainingTime(DRK.Delirium) <= 60 &&
                                    GetCooldownRemainingTime(DRK.Delirium) >= 50 &&
                                    GetBuffStacks(DRK.Buffs.Delirium) <= 1 &&
                                    IsOffCooldown(DRK.BloodWeapon) && canWeave
                                   ) return DRK.BloodWeapon;
                            }
                        }

                        //Living Shadow Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleLivingShadow))
                            {
                                if (
                                    level >= DRK.Levels.LivingShadow &&
                                    gauge.Blood >= 50 &&
                                    IsOffCooldown(DRK.LivingShadow) && canWeave
                                   ) return DRK.LivingShadow;
                            }
                        }

                        //Shadowbringer Feature Part 1
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleShadowbringer))
                            {
                                if (
                                    level >= DRK.Levels.Shadowbringer &&
                                    GetRemainingCharges(DRK.Shadowbringer) > 1 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.Shadowbringer;
                            }
                        }

                        //Salted Earth Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleSaltedEarth))
                            {
                                if (
                                    level >= DRK.Levels.SaltedEarth &&
                                    IsOffCooldown(DRK.SaltedEarth) && canWeave
                                   ) return DRK.SaltedEarth;
                            }
                        }

                        //Salt and Darkness Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleSaltAndDarkness))
                            {
                                if (
                                    level >= DRK.Levels.SaltAndDarkness &&
                                    IsOffCooldown(DRK.SaltAndDarkness) &&
                                    HasEffect(DRK.Buffs.SaltedEarth) && canWeave
                                   ) return DRK.SaltAndDarkness;
                            }
                        }

                        //Carve and Spit Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleCarveAndSpit))
                            {
                                if (
                                    level >= DRK.Levels.CarveAndSpit &&
                                    gauge.ShadowTimeRemaining > 0 &&
                                    IsOffCooldown(DRK.CarveAndSpit) && canWeave
                                   ) return DRK.CarveAndSpit;

                                if (
                                    level >= DRK.Levels.CarveAndSpit &&
                                    GetCooldownRemainingTime(DRK.BloodWeapon) <= 60 &&
                                    GetCooldownRemainingTime(DRK.BloodWeapon) >= 35 &&
                                    IsOffCooldown(DRK.CarveAndSpit) && canWeave
                                   ) return DRK.CarveAndSpit;
                            }
                        }

                        //Plunge Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimplePlunge))
                            {
                                if (
                                    level >= DRK.Levels.Plunge &&
                                    gauge.ShadowTimeRemaining > 0 &&
                                    GetRemainingCharges(DRK.Plunge) > 0 && canWeave
                                   ) return DRK.Plunge;

                                if (
                                    level >= DRK.Levels.Plunge &&
                                    GetCooldownRemainingTime(DRK.BloodWeapon) <= 60 &&
                                    GetCooldownRemainingTime(DRK.BloodWeapon) >= 35 &&
                                    GetRemainingCharges(DRK.Plunge) > 0 && canWeave
                                   ) return DRK.Plunge;
                            }
                        }

                        //Shadowbringer Feature Part 2
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleShadowbringer))
                            {
                                if (
                                    level >= DRK.Levels.Shadowbringer &&
                                    GetRemainingCharges(DRK.Shadowbringer) == 1 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.Shadowbringer;
                            }
                        }

                        //Edge Feature
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleEdge))
                            {
                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 3000 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 3000 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.EdgeOfDarkness;

                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp > 8500 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp > 8500 && canWeave
                                   ) return DRK.EdgeOfDarkness;

                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 3000 &&
                                    gauge.DarksideTimeRemaining < 10 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 3000 &&
                                    gauge.DarksideTimeRemaining < 10 && canWeave
                                   ) return DRK.EdgeOfDarkness;
                            }
                        }

                        //Edge Mana Protection Feature
                        {
                            if (IsEnabled(CustomComboPreset.DarkSimpleEdgeProtection))
                            {
                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 6000 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 6000 &&
                                    gauge.ShadowTimeRemaining > 0 && canWeave
                                   ) return DRK.EdgeOfDarkness;

                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp > 8500 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp > 8500 && canWeave
                                   ) return DRK.EdgeOfDarkness;

                                if (
                                    level >= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 6000 &&
                                    gauge.DarksideTimeRemaining < 10 && canWeave
                                   ) return DRK.EdgeOfShadow;

                                if (
                                    level >= DRK.Levels.EdgeOfDarkness &&
                                    level <= DRK.Levels.EdgeOfShadow &&
                                    currentMp >= 6000 &&
                                    gauge.DarksideTimeRemaining < 10 && canWeave
                                   ) return DRK.EdgeOfDarkness;
                            }
                        }

                        //Bloodspiller Feature
                        if (IsEnabled(CustomComboPreset.DarkSimpleBloodspiller))
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) > 0)
                                return DRK.Bloodspiller;

                            if (gauge.Blood >= 90)
                                return DRK.Bloodspiller;
                        }

                        //1-2-3 Combo
                        if (comboTime > 0)
                        {
                            if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                                return DRK.SyphonStrike;

                            if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                                return DRK.Souleater;

                            if (lastComboMove == DRK.Souleater)
                                return DRK.HardSlash;
                        }

                        return DRK.HardSlash;

                    }

                        if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        step = 0;
                        openerFinished = false;

                        return DRK.HardSlash;
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        if (step == 0)
                        {
                            if (lastComboMove == DRK.HardSlash) step++;
                            else return DRK.HardSlash;
                        }

                        if (step == 1)
                        {
                            if (gauge.DarksideTimeRemaining > 0) step++;
                            else return DRK.EdgeOfShadow;
                        }

                        if (step == 2)
                        {
                            if (IsOnCooldown(DRK.Delirium)) step++;
                            else return DRK.Delirium;
                        }

                        if (step == 3)
                        {
                            if (lastComboMove == DRK.SyphonStrike) step++;
                            else return DRK.SyphonStrike;
                        }

                        if (step == 4)
                        {
                            if (lastComboMove == DRK.Souleater) step++;
                            else return DRK.Souleater;
                        }

                        if (step == 5)
                        {
                            if (IsOnCooldown(DRK.SaltedEarth)) step++;
                            else return DRK.SaltedEarth;
                        }

                        if (step == 6)
                        {
                            if (IsOnCooldown(DRK.LivingShadow)) step++;
                            if (gauge.Blood < 50) step++;
                            return DRK.LivingShadow;
                        }

                        if (step == 7)
                        {
                            if (lastComboMove == DRK.HardSlash) step++;
                            else return DRK.HardSlash;
                        }

                        if (step == 8)
                        {
                            if (GetRemainingCharges(DRK.Shadowbringer) is 0 or 1) step++;
                            else return DRK.Shadowbringer;
                        }

                        if (step == 9)
                        {
                            if (gauge.DarksideTimeRemaining > 25000) step++;
                            else return DRK.EdgeOfShadow;
                        }

                        if (step == 10)
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) < 3) step++;
                            else return DRK.Bloodspiller;
                        }

                        if (step == 11)
                        {
                            if (IsOnCooldown(DRK.CarveAndSpit)) step++;
                            else return DRK.CarveAndSpit;
                        }

                        if (step == 12)
                        {
                            if (GetRemainingCharges(DRK.Plunge) is 0 or 1) step++;
                            else return DRK.Plunge;
                        }

                        if (step == 13)
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) < 2) step++;
                            else return DRK.Bloodspiller;
                        }

                        if (step == 14)
                        {
                            if (GetRemainingCharges(DRK.Shadowbringer) == 0) step++;
                            else return DRK.Shadowbringer;
                        }

                        if (step == 15)
                        {
                            if (gauge.DarksideTimeRemaining > 50000) step++;
                            else return DRK.EdgeOfShadow;
                        }

                        if (step == 16)
                        {
                            if (GetBuffStacks(DRK.Buffs.Delirium) < 1) step++;
                            else return DRK.Bloodspiller;
                        }

                        if (step == 17)
                        {
                            if (IsOnCooldown(DRK.SaltAndDarkness)) step++;
                            else return DRK.SaltAndDarkness;
                        }

                        if (step == 18)
                        {
                            if (gauge.DarksideTimeRemaining == 60000) step++;
                            if (currentMp < 6000) step++;
                            else return DRK.EdgeOfShadow;
                        }

                        if (step == 19)
                        {
                            if (lastComboMove == DRK.SyphonStrike) step++;
                            else return DRK.SyphonStrike;
                        }

                        if (step == 20)
                        {
                            if (gauge.DarksideTimeRemaining == 60000) step++;
                            if (currentMp < 3000) step++;
                            else return DRK.EdgeOfShadow;
                        }

                        if (step == 21)
                        {
                            if (GetRemainingCharges(DRK.Plunge) == 0) step++;
                            else return DRK.Plunge;
                        }

                        openerFinished = true;
                    }
                }

                //Delirium Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleDelirium))
                    {
                        if (
                            level >= DRK.Levels.Delirium &&
                            IsOffCooldown(DRK.Delirium) && CanWeave(actionID, 0.3)
                           ) return DRK.Delirium;
                    }
                }

                //Blood Weapon Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleBloodWeapon))
                    {
                        if (
                            level >= DRK.Levels.BloodWeapon &&
                            GetCooldownRemainingTime(DRK.Delirium) <= 60 &&
                            GetCooldownRemainingTime(DRK.Delirium) >= 50 &&
                            GetBuffStacks(DRK.Buffs.Delirium) <= 1 &&
                            IsOffCooldown(DRK.BloodWeapon) && canWeave
                           ) return DRK.BloodWeapon;
                    }
                }

                //Living Shadow Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleLivingShadow))
                    {
                        if (
                            level >= DRK.Levels.LivingShadow &&
                            gauge.Blood >= 50 &&
                            IsOffCooldown(DRK.LivingShadow) && canWeave
                           ) return DRK.LivingShadow;
                    }
                }

                //Shadowbringer Feature Part 1
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleShadowbringer))
                    {
                        if (
                            level >= DRK.Levels.Shadowbringer &&
                            GetRemainingCharges(DRK.Shadowbringer) > 1 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.Shadowbringer;
                    }
                }

                //Salted Earth Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleSaltedEarth))
                    {
                        if (
                            level >= DRK.Levels.SaltedEarth &&
                            IsOffCooldown(DRK.SaltedEarth) && canWeave
                           ) return DRK.SaltedEarth;
                    }
                }

                //Salt and Darkness Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleSaltAndDarkness))
                    {
                        if (
                            level >= DRK.Levels.SaltAndDarkness &&
                            IsOffCooldown(DRK.SaltAndDarkness) &&
                            HasEffect(DRK.Buffs.SaltedEarth) && canWeave
                           ) return DRK.SaltAndDarkness;
                    }
                }

                //Carve and Spit Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleCarveAndSpit))
                    {
                        if (
                            level >= DRK.Levels.CarveAndSpit &&
                            gauge.ShadowTimeRemaining > 0 &&
                            IsOffCooldown(DRK.CarveAndSpit) && canWeave
                           ) return DRK.CarveAndSpit;

                        if (
                            level >= DRK.Levels.CarveAndSpit &&
                            GetCooldownRemainingTime(DRK.BloodWeapon) <= 60 &&
                            GetCooldownRemainingTime(DRK.BloodWeapon) >= 35 &&
                            IsOffCooldown(DRK.CarveAndSpit) && canWeave
                           ) return DRK.CarveAndSpit;
                    }
                }

                //Plunge Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimplePlunge))
                    {
                        if (
                            level >= DRK.Levels.Plunge &&
                            gauge.ShadowTimeRemaining > 0 &&
                            GetRemainingCharges(DRK.Plunge) > 0 && canWeave
                           ) return DRK.Plunge;

                        if (
                            level >= DRK.Levels.Plunge &&
                            GetCooldownRemainingTime(DRK.BloodWeapon) <= 60 &&
                            GetCooldownRemainingTime(DRK.BloodWeapon) >= 35 &&
                            GetRemainingCharges(DRK.Plunge) > 0 && canWeave
                           ) return DRK.Plunge;
                    }
                }

                //Shadowbringer Feature Part 2
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleShadowbringer))
                    {
                        if (
                            level >= DRK.Levels.Shadowbringer &&
                            GetRemainingCharges(DRK.Shadowbringer) == 1 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.Shadowbringer;
                    }
                }

                //Edge Feature
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleEdge))
                    {
                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 3000 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 3000 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.EdgeOfDarkness;

                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp > 8500 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp > 8500 && canWeave
                           ) return DRK.EdgeOfDarkness;

                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 3000 &&
                            gauge.DarksideTimeRemaining < 10 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 3000 &&
                            gauge.DarksideTimeRemaining < 10 && canWeave
                           ) return DRK.EdgeOfDarkness;
                    }
                }

                //Edge Mana Protection Feature
                {
                    if (IsEnabled(CustomComboPreset.DarkSimpleEdgeProtection))
                    {
                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 6000 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 6000 &&
                            gauge.ShadowTimeRemaining > 0 && canWeave
                           ) return DRK.EdgeOfDarkness;

                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp > 8500 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp > 8500 && canWeave
                           ) return DRK.EdgeOfDarkness;

                        if (
                            level >= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 6000 &&
                            gauge.DarksideTimeRemaining < 10 && canWeave
                           ) return DRK.EdgeOfShadow;

                        if (
                            level >= DRK.Levels.EdgeOfDarkness &&
                            level <= DRK.Levels.EdgeOfShadow &&
                            currentMp >= 6000 &&
                            gauge.DarksideTimeRemaining < 10 && canWeave
                           ) return DRK.EdgeOfDarkness;
                    }
                }

                //Bloodspiller Feature
                if (IsEnabled(CustomComboPreset.DarkSimpleBloodspiller))
                {
                    if (GetBuffStacks(DRK.Buffs.Delirium) > 0)
                        return DRK.Bloodspiller;

                    if (gauge.Blood >= 90)
                        return DRK.Bloodspiller;
                }

                //1-2-3 Combo
                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;

                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;

                    if (lastComboMove == DRK.Souleater)
                        return DRK.HardSlash;
                }

                return DRK.HardSlash;
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
