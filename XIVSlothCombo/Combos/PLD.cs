namespace XIVSlothComboPlugin.Combos
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
            ShieldLob = 24,
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
            Atonement = 16460,
            Intervene = 16461;

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
                var goringBladeDebuffonTarget = TargetHasEffect(PLD.Debuffs.GoringBlade);
                var goingBladeDebuffTimer = FindTargetEffect(PLD.Debuffs.GoringBlade);
                var foF = HasEffect(PLD.Buffs.FightOrFlight);
                var valorDebuff = TargetHasEffect(PLD.Debuffs.BladeOfValor);
                var foFCD = GetCooldown(PLD.FightOrFlight);
                var fastBladeCD = GetCooldown(PLD.FastBlade);
                var reqCD = GetCooldown(PLD.Requiescat);
                var requiescatBuff = HasEffect(PLD.Buffs.Requiescat);
                var requiescat = FindEffect(PLD.Buffs.Requiescat);
                var valorDebuffTimer = FindTargetEffect(PLD.Debuffs.BladeOfValor);
                var valorDebuffonTarget = TargetHasEffect(PLD.Debuffs.BladeOfValor);
                var interveneCD = GetCooldown(PLD.Intervene);
                var actionIDCD = GetCooldown(actionID);
                if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature))
                {
                    if (!InMeleeRange(true))
                        return PLD.ShieldLob;
                }
                if (IsEnabled(CustomComboPreset.PaladinInterveneFeature) && level >= 74)
                {
                    if (interveneCD.CooldownRemaining < 30 && actionIDCD.CooldownRemaining > 0.7)
                        return PLD.Intervene;
                }
                if (IsEnabled(CustomComboPreset.PaladinInterveneFeatureOption) && level >= 74)
                {
                    if (!interveneCD.IsCooldown && actionIDCD.CooldownRemaining > 0.7)
                        return PLD.Intervene;
                }
                if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                {
                    if (HasEffect(PLD.Buffs.Requiescat) && level >= 64 && !foF)
                    {
                        if ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0 && level >= 80) || (requiescat.StackCount == 1 && level >= 80) || LocalPlayer.CurrentMp <= 2000)
                            return PLD.Confiteor;
                        return PLD.HolySpirit;
                    }

                    if (lastComboMove == PLD.Confiteor && level >= 90)
                    {
                        return PLD.BladeOfFaith;
                    }

                    if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                    {
                        return PLD.BladeOfTruth;
                    }

                    if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                    {
                        return PLD.BladeOfValor;
                    }
                }

                if (IsEnabled(CustomComboPreset.PaladinRoyalGoringOption))
                {
                    if ((lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.GoringBlade) && goingBladeDebuffTimer.RemainingTime > 10) || (lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.BladeOfValor) && valorDebuffTimer.RemainingTime > 10))
                        return PLD.RoyalAuthority;
                    else
                    if ((lastComboMove == PLD.RiotBlade && !goringBladeDebuffonTarget && level >= 54) || (lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.BladeOfValor) && valorDebuffTimer.RemainingTime < 5 && level >= 54) || (lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.GoringBlade) && goingBladeDebuffTimer.RemainingTime < 5 && level >= 54))
                        return PLD.GoringBlade;
                }

                if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (lastComboMove == PLD.RiotBlade && level >= 60)
                    {
                        return PLD.RoyalAuthority;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade)
                        return PLD.RiotBlade;
                }

                if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath))
                        return PLD.Atonement;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.RiotBlade)
                        return actionID;
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

                        if ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0 && level >= 80) || (requiescat.StackCount == 1 && level >= 80) || LocalPlayer.CurrentMp <= 2000)
                            return PLD.Confiteor;
                        return PLD.HolyCircle;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;
                }

                if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature))
                {
                    if (lastComboMove == PLD.Confiteor && level >= 90)
                    {
                        return PLD.BladeOfFaith;
                    }

                    if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                    {
                        return PLD.BladeOfTruth;
                    }

                    if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                    {
                        return PLD.BladeOfValor;
                    }

                    if (level >= PLD.Levels.Confiteor)
                    {
                        var requiescat = FindEffect(PLD.Buffs.Requiescat);
                        if (requiescat != null)
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
    internal class PaladinStandaloneHolySpiritFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinStandaloneHolySpiritFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == PLD.HolySpirit)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= 64 )
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);
                    if ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0 && level >= 80) || (requiescat.StackCount == 1 && level >= 80) || LocalPlayer.CurrentMp <= 2000)
                        return PLD.Confiteor;
                    return PLD.HolySpirit;
                }
                if (lastComboMove == PLD.Confiteor && level >= 90)
                {
                    return PLD.BladeOfFaith;
                }

                if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                {
                    return PLD.BladeOfTruth;
                }

                if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                {
                    return PLD.BladeOfValor;
                }
            }
            return OriginalHook(actionID);
        }
    }
    internal class PaladinStandaloneHolyCircleFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinStandaloneHolyCircleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolyCircle)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= 64)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);
                    if ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0 && level >= 80) || (requiescat.StackCount == 1 && level >= 80) || LocalPlayer.CurrentMp <= 2000)
                        return PLD.Confiteor;
                    return PLD.HolyCircle;
                }
            
                if (lastComboMove == PLD.Confiteor && level >= 90)
                {
                    return PLD.BladeOfFaith;
                }

                if (lastComboMove == PLD.BladeOfFaith && level >= 90)
                {
                    return PLD.BladeOfTruth;
                }

                if (lastComboMove == PLD.BladeOfTruth && level >= 90)
                {
                    return PLD.BladeOfValor;
                }
            }
            return OriginalHook(actionID);
        }
    }
}
