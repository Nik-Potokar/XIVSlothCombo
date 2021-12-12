namespace XIVComboExpandedPlugin.Combos
{
    internal static class PLD
    {
        public const byte ClassID = 1;
        public const byte JobID = 19;

        public const float CooldownThreshold = 0.5f;

        public const uint
            FastBlade = 9,
            RiotBlade = 15,
            ShieldBash = 16,
            RageOfHalone = 21,
            CircleOfScorn = 23,
            SpiritsWithin = 29,
            GoringBlade = 3538,
            RoyalAuthority = 3539,
            LowBlow = 7540,
            TotalEclipse = 7381,
            Requiescat = 7383,
            HolySpirit = 7384,
            Prominence = 16457,
            HolyCircle = 16458,
            Confiteor = 16459,
            Expiacion = 25747,
            BladeOfFaith = 25748,
            BladeOfTruth = 25749,
            BladeOfValor = 25750,
            FightOrFlight = 20,
            Atonement = 16460;

        public static class Buffs
        {
            public const short
                Requiescat = 1368,
                SwordOath = 1902,
                FightOrFlight = 76;
        }

        public static class Debuffs
        {
            public const short
                BladeOfValor = 2721,
                GoringBlade = 725;
        }

        public static class Levels
        {
            public const byte
                RiotBlade = 4,
                RageOfHalone = 26,
                SpiritsWithin = 30,
                Prominence = 40,
                CircleOfScorn = 50,
                GoringBlade = 54,
                RoyalAuthority = 60,
                HolyCircle = 72,
                Atonement = 76,
                Confiteor = 80,
                Expiacion = 86,
                BladeOfFaith = 90,
                BladeOfTruth = 90,
                BladeOfValor = 90;
        }
    }

    internal class PaladinGoringBladeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinGoringBladeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.GoringBlade)
            {

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade)
                        return PLD.GoringBlade;
                }

                return PLD.FastBlade;
            }
            return actionID;
        }
    }

    internal class PaladinRoyalAuthorityCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinRoyalAuthorityCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.RageOfHalone || actionID == PLD.RoyalAuthority)
            {
                if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath))
                        return PLD.Atonement;
                }

                if (comboTime > 0)
                {
                    var goringBladeDebuffonTarget = TargetHasEffect(PLD.Debuffs.GoringBlade);
                    var goingBladeDebuffTimer = FindTargetEffect(PLD.Debuffs.GoringBlade);
                    var FoF = HasEffect(PLD.Buffs.FightOrFlight);
                    var valorDebuff = TargetHasEffect(PLD.Debuffs.BladeOfValor);
                    var FoFCD = GetCooldown(PLD.FightOrFlight);
                    var fastBladeCD = GetCooldown(PLD.FastBlade);
                    var reqCD = GetCooldown(PLD.Requiescat);
                    var requiescatBuff = HasEffect(PLD.Buffs.Requiescat);
                    var reqistack = FindEffect(PLD.Buffs.Requiescat);
                    var valorDebuffTimer = FindTargetEffect(PLD.Debuffs.BladeOfValor);
                    var valorDebuffonTarget = TargetHasEffect(PLD.Debuffs.BladeOfValor);

                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade && valorDebuffonTarget && valorDebuffTimer.RemainingTime < 4)
                        return PLD.GoringBlade;

                    if ((lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade && !goringBladeDebuffonTarget && !valorDebuff) || (lastComboMove == PLD.RiotBlade && (goingBladeDebuffTimer.RemainingTime < 4) && !valorDebuff) || (lastComboMove == PLD.RiotBlade && (valorDebuffTimer.RemainingTime < 4) && !goringBladeDebuffonTarget))
                        return PLD.GoringBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone && (goringBladeDebuffonTarget))
                        return OriginalHook(PLD.RageOfHalone);

                    if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                    {
                        if (HasEffect(PLD.Buffs.SwordOath))
                            return PLD.Atonement;
                    }
                    if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                    {
                        {
                            if (HasEffect(PLD.Buffs.Requiescat) && !FoF)
                            {
                                if (HasEffect(PLD.Buffs.Requiescat) && level >= 64)
                                {
                                    var requiescat = FindEffect(PLD.Buffs.Requiescat);

                                    if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000)))
                                        return PLD.Confiteor;

                                    return PLD.HolySpirit;
                                }
                            }
                        }
                        if (IsEnabled(CustomComboPreset.PaladingMainComboFeature))
                        {
                            if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                                return PLD.BladeOfValor;

                            if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                                return PLD.BladeOfTruth;

                            if (lastComboMove == PLD.Confiteor && level >= 90)
                                return PLD.BladeOfFaith;

                            if (level >= PLD.Levels.Confiteor)
                            {
                                var requiescat = FindEffect(PLD.Buffs.Requiescat);
                                if (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000))
                                    return PLD.Confiteor;
                            }
                        }
                    }

                }

                return PLD.FastBlade;
            }

            return actionID;
        }
    }

    internal class PaladinProminenceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinProminenceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.Prominence)
            {
                if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                {
                    if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
                    {
                        var requiescat = FindEffect(PLD.Buffs.Requiescat);

                        if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000)))
                            return PLD.Confiteor;

                        return PLD.HolyCircle;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;
                }
                if (IsEnabled(CustomComboPreset.PaladingMainComboFeature))
                {
                    if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                        return PLD.BladeOfValor;

                    if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                        return PLD.BladeOfTruth;

                    if (lastComboMove == PLD.Confiteor && level >= 90)
                        return PLD.BladeOfFaith;

                    if (level >= PLD.Levels.Confiteor)
                    {
                        var requiescat = FindEffect(PLD.Buffs.Requiescat);
                        if (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000))
                            return PLD.Confiteor;
                    }
                }

                return PLD.TotalEclipse;
            }

            return actionID;
        }
    }

    internal class PaladinScornfulSpiritsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinScornfulSpiritsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.SpiritsWithin || actionID == PLD.CircleOfScorn)
            {
                if (level >= PLD.Levels.SpiritsWithin && level <= PLD.Levels.Expiacion)
                    return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.CircleOfScorn);

                if (level >= PLD.Levels.Expiacion)
                    return CalcBestAction(actionID, PLD.Expiacion, PLD.CircleOfScorn);

                if (level >= PLD.Levels.CircleOfScorn)
                    return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.CircleOfScorn);

                return PLD.SpiritsWithin;
            }

            return actionID;
        }
    }
    internal class PaladinHolySpiritStandaloneFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinHolySpiritStandaloneFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolySpirit)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= 64)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);

                    if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000)))
                        return PLD.Confiteor;

                    return PLD.HolySpirit;
                }
                if (level >= PLD.Levels.Confiteor)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);
                    if (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000))
                        return PLD.Confiteor;
                }
            }
            return actionID;

        }
    }
}

