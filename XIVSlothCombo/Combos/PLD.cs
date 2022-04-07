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
            Interject = 7538,
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
            public const ushort
                Requiescat = 1368,
                SwordOath = 1902,
                FightOrFlight = 76;
        }

        public static class Debuffs
        {
            public const ushort
                BladeOfValor = 2721,
                GoringBlade = 725;
        }

        public static class Levels
        {
            public const byte
                FastBlade = 1,
                FightOrFlight = 2,
                RiotBlade = 4,
                TotalEclipse = 6,
                ShieldBash = 10,
                IronWill = 10,
                ShieldLob = 15,
                RageOfHalone = 26,
                SpiritsWithin = 30,
                Sheltron = 35,
                Sentinel = 38,
                Prominence = 40,
                Cover = 45,
                CircleOfScorn = 50,
                HallowedGround = 50,
                GoringBlade = 54,
                DivineVeil = 56,
                Clemency = 58,
                RoyalAuthority = 60,
                Intervention = 62,
                HolySpirit = 64,
                Requiescat = 68,
                PassageOfArms = 70,
                HolyCircle = 72,
                Intervene = 74,
                Atonement = 76,
                Confiteor = 80,
                HolySheltron = 82,
                Expiacion = 86,
                BladeOfFaith = 90,
                BladeOfTruth = 90,
                BladeOfValor = 90;
        }
    }

    internal class PaladinGoringBladeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinGoringBladeCombo;

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

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinRoyalAuthorityCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.RageOfHalone || actionID == PLD.RoyalAuthority)
            {
                var goringBladeDebuffonTarget = TargetHasEffect(PLD.Debuffs.GoringBlade);
                var goingBladeDebuffTimer = FindTargetEffect(PLD.Debuffs.GoringBlade);
                var FightOrFlight = HasEffect(PLD.Buffs.FightOrFlight);
                var FightOrFlightCD = GetCooldown(PLD.FightOrFlight);
                var reqCD = GetCooldown(PLD.Requiescat);
                var requiescat = FindEffect(PLD.Buffs.Requiescat);
                var valorDebuffTimer = FindTargetEffect(PLD.Debuffs.BladeOfValor);
                var interveneCD = GetCooldown(PLD.Intervene);
                var riotcd = GetCooldown(actionID);
                var customGCDHigh = Service.Configuration.CustomGCDValueHigh;
                var customGCDLow = Service.Configuration.CustomGCDValueLow;
                var fofremainingTime = FindEffect(PLD.Buffs.FightOrFlight);

                if (IsEnabled(CustomComboPreset.PaladinFightOrFlightFeature))
                {
                    if (level >= PLD.Levels.FightOrFlight && lastComboMove == PLD.FastBlade && riotcd.CooldownRemaining < customGCDLow && riotcd.CooldownRemaining > customGCDHigh && !FightOrFlightCD.IsCooldown)
                        return PLD.FightOrFlight;
                }

                if (IsEnabled(CustomComboPreset.PaladinExpiacionScornFeature))
                {
                    if (level >= PLD.Levels.Expiacion && IsOffCooldown(PLD.Expiacion) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade && CanWeave(actionID))
                        return PLD.Expiacion;
                    if (level >= PLD.Levels.CircleOfScorn && IsOffCooldown(PLD.CircleOfScorn) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade && CanWeave(actionID))
                        return PLD.CircleOfScorn;
                }

                if (IsEnabled(CustomComboPreset.PaladinReqMainComboFeature) && level >= PLD.Levels.Requiescat)
                {
                    if (HasEffect(PLD.Buffs.FightOrFlight) && fofremainingTime.RemainingTime < 17 && !reqCD.IsCooldown)
                        return PLD.Requiescat;
                }

                if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature) && level >= PLD.Levels.ShieldLob)
                {
                    if (!InMeleeRange(true))
                        return PLD.ShieldLob;
                }

                if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature2) && level >= PLD.Levels.HolySpirit)
                {
                    if (!InMeleeRange(true))
                        return PLD.HolySpirit;
                }

                if (IsEnabled(CustomComboPreset.PaladinInterveneFeature) && level >= PLD.Levels.Intervene)
                {
                    if (interveneCD.CooldownRemaining < 30 && CanWeave(actionID))
                        return PLD.Intervene;
                }

                if (IsEnabled(CustomComboPreset.PaladinInterveneFeatureOption) && level >= PLD.Levels.Intervene)
                {
                    if (!interveneCD.IsCooldown && CanWeave(actionID))
                        return PLD.Intervene;
                }

                if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                {
                    if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolySpirit && !FightOrFlight)
                    {
                        if (
                            level >= PLD.Levels.Confiteor &&
                            (
                                (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                                requiescat.StackCount == 1 ||
                                LocalPlayer.CurrentMp <= 2000
                            )
                        )
                            return PLD.Confiteor;
                        return PLD.HolySpirit;
                    }

                    if (lastComboMove == PLD.Confiteor && level >= PLD.Levels.BladeOfFaith)
                    {
                        return PLD.BladeOfFaith;
                    }

                    if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                    {
                        return PLD.BladeOfTruth;
                    }

                    if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
                    {
                        return PLD.BladeOfValor;
                    }
                }
                
                if (IsEnabled(CustomComboPreset.PaladinRoyalGoringOption))
                {
                    if ((lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.GoringBlade) && goingBladeDebuffTimer.RemainingTime > 10 && level >= PLD.Levels.RoyalAuthority) || (lastComboMove == PLD.RiotBlade && TargetHasEffect(PLD.Debuffs.BladeOfValor) && valorDebuffTimer.RemainingTime > 10 && level >= PLD.Levels.RoyalAuthority))
                        return PLD.RoyalAuthority;
                    else
                    if (
                        level >= PLD.Levels.GoringBlade &&
                        lastComboMove == PLD.RiotBlade &&
                        (
                            (!goringBladeDebuffonTarget) ||
                            (TargetHasEffect(PLD.Debuffs.BladeOfValor) && valorDebuffTimer.RemainingTime < 5) ||
                            (TargetHasEffect(PLD.Debuffs.GoringBlade) && goingBladeDebuffTimer.RemainingTime < 5)
                        )
                    )
                    {
                        return PLD.GoringBlade;
                    }
                }


                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RoyalAuthority)
                        return PLD.RoyalAuthority;
                    
                }
                if (IsEnabled(CustomComboPreset.PaladinAtonementTestFeature))
                {
                    if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && FightOrFlightCD.CooldownRemaining >= 2 && FightOrFlightCD.CooldownRemaining <= 50)
                        return PLD.Atonement;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        return PLD.RageOfHalone;
                }
                return PLD.FastBlade;
            }

            return actionID;
        }

    }

    internal class PaladinProminenceCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinProminenceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var reqCD = GetCooldown(PLD.Requiescat);
            if (actionID == PLD.Prominence)
            {
                if (IsEnabled(CustomComboPreset.PaladinReqAoEComboFeature) && level >= PLD.Levels.Requiescat)
                {
                    if (!reqCD.IsCooldown && CanWeave(actionID))
                        return PLD.Requiescat;
                }

                if (IsEnabled(CustomComboPreset.PaladinAoEExpiacionScornFeature))
                {
                    if (level >= PLD.Levels.Expiacion && IsOffCooldown(PLD.Expiacion) && CanWeave(actionID))
                        return PLD.Expiacion;
                    if (level >= PLD.Levels.CircleOfScorn && IsOffCooldown(PLD.CircleOfScorn) && CanWeave(actionID))
                        return PLD.CircleOfScorn;
                }

                if (IsEnabled(CustomComboPreset.PaladinHolyCircleFeature))
                {
                    if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
                    {
                        var requiescat = FindEffect(PLD.Buffs.Requiescat);

                        if (
                            level >= PLD.Levels.Confiteor &&
                            (
                                (IsEnabled(CustomComboPreset.PaladinAoEConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                                requiescat.StackCount == 1 ||
                                LocalPlayer.CurrentMp <= 2000
                            )
                        )
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
                    if (lastComboMove == PLD.Confiteor && level >= PLD.Levels.BladeOfFaith)
                    {
                        return PLD.BladeOfFaith;
                    }

                    if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                    {
                        return PLD.BladeOfTruth;
                    }

                    if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinStandaloneHolySpiritFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolySpirit)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolySpirit)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);
                    if (
                        level >= PLD.Levels.Confiteor &&
                        (
                            (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                            requiescat.StackCount == 1 ||
                            LocalPlayer.CurrentMp <= 2000
                        )
                    )
                    {
                        return PLD.Confiteor;
                    }

                    return PLD.HolySpirit;
                }
                if (lastComboMove == PLD.Confiteor && level >= PLD.Levels.BladeOfFaith)
                {
                    return PLD.BladeOfFaith;
                }

                if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                {
                    return PLD.BladeOfTruth;
                }

                if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
                {
                    return PLD.BladeOfValor;
                }
            }
            return actionID;
        }
    }
    internal class PaladinStandaloneHolyCircleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinStandaloneHolyCircleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolyCircle)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);
                    if (
                        level >= PLD.Levels.Confiteor &&
                        (
                            (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                            requiescat.StackCount == 1 ||
                            LocalPlayer.CurrentMp <= 2000
                        )
                    )
                    {
                        return PLD.Confiteor;
                    }

                    return PLD.HolyCircle;
                }

                if (lastComboMove == PLD.Confiteor && level >= PLD.Levels.BladeOfFaith)
                {
                    return PLD.BladeOfFaith;
                }

                if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                {
                    return PLD.BladeOfTruth;
                }

                if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
                {
                    return PLD.BladeOfValor;
                }
            }
            return actionID;
        }
    }
    internal class PaladinInterruptFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.ShieldBash)
            {
                var interjectCD = GetCooldown(PLD.Interject);
                var lowBlowCD = GetCooldown(PLD.LowBlow);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return PLD.Interject;
                if (!lowBlowCD.IsCooldown)
                    return PLD.LowBlow;
            }

            return actionID;
        }
    }
}
