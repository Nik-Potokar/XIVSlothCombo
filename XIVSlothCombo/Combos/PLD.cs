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
            public const ushort
                Requiescat = 1368,
                SwordOath = 1902,
                FightOrFlight = 76,
                BladeOfFaithReady = 3019;
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

        public static class Config
        {
            public const string
                PLDKeepInterveneCharges = "PLDKeepInterveneCharges";
        }
    }

    internal class PaladinGoringBladeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinGoringBladeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is PLD.GoringBlade)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove is PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove is PLD.RiotBlade && level >= PLD.Levels.GoringBlade)
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
            if (actionID is PLD.RageOfHalone or PLD.RoyalAuthority)
            {
                var interveneChargesRemaining = Service.Configuration.GetCustomIntValue(PLD.Config.PLDKeepInterveneCharges);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                // Uptime Features
                if (!InMeleeRange() && !(HasEffect(PLD.Buffs.BladeOfFaithReady) || lastComboMove is PLD.BladeOfFaith || lastComboMove is PLD.BladeOfTruth))
                {
                    if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature) && level >= PLD.Levels.ShieldLob && !HasEffect(PLD.Buffs.Requiescat))
                        return PLD.ShieldLob;
                    if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature2) && level >= PLD.Levels.HolySpirit)
                        return PLD.HolySpirit;
                }

                // Buffs
                if (GetCooldown(actionID).CooldownRemaining < 0.9 && GetCooldown(actionID).CooldownRemaining > 0.2)
                {
                    if (IsEnabled(CustomComboPreset.PaladinFightOrFlightFeature) && level >= PLD.Levels.FightOrFlight && lastComboMove is PLD.FastBlade && IsOffCooldown(PLD.FightOrFlight))
                        return PLD.FightOrFlight;
                    if (IsEnabled(CustomComboPreset.PaladinReqMainComboFeature) && level >= PLD.Levels.Requiescat && HasEffect(PLD.Buffs.FightOrFlight) && GetBuffRemainingTime(PLD.Buffs.FightOrFlight) < 17 && IsOffCooldown(PLD.Requiescat))
                        return PLD.Requiescat;
                }

                // oGCD features
                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.PaladinExpiacionScornFeature) && incombat && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade)
                    {
                        if (level >= PLD.Levels.SpiritsWithin && IsOffCooldown(PLD.SpiritsWithin))
                            return OriginalHook(PLD.SpiritsWithin);

                        if (level >= PLD.Levels.CircleOfScorn && IsOffCooldown(PLD.CircleOfScorn))
                            return PLD.CircleOfScorn;
                    }

                    if (IsEnabled(CustomComboPreset.PaladinInterveneFeature) && level >= PLD.Levels.Intervene && GetRemainingCharges(PLD.Intervene) > interveneChargesRemaining)
                    {
                        if (IsNotEnabled(CustomComboPreset.PaladinMeleeInterveneOption) ||
                            (IsEnabled(CustomComboPreset.PaladinMeleeInterveneOption) && HasEffect(PLD.Buffs.FightOrFlight) && GetTargetDistance() <= 1))
                            return PLD.Intervene;
                    }
                }

                // GCDs
                if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                {
                    if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolySpirit && !HasEffect(PLD.Buffs.FightOrFlight) && LocalPlayer.CurrentMp >= 1000)
                    {
                        if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor &&
                            ((GetBuffRemainingTime(PLD.Buffs.Requiescat) <= 3 && GetBuffRemainingTime(PLD.Buffs.Requiescat) >= 0) || GetBuffStacks(PLD.Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000)) //Confiteor Conditions
                                return PLD.Confiteor;

                            return PLD.HolySpirit;
                    }

                    if (HasEffect(PLD.Buffs.BladeOfFaithReady) || lastComboMove is PLD.BladeOfFaith || lastComboMove is PLD.BladeOfTruth)
                        return OriginalHook(PLD.Confiteor);
                }

                if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (IsNotEnabled(CustomComboPreset.PaladinAtonementDropFeature))
                        return PLD.Atonement;

                    if ((IsEnabled(CustomComboPreset.PaladinAtonementDropFeature) &&
                         GetCooldownRemainingTime(PLD.FightOrFlight) <= 15 && GetBuffStacks(PLD.Buffs.SwordOath) > 1) ||
                        (HasEffect(PLD.Buffs.Requiescat) && GetCooldownRemainingTime(PLD.FightOrFlight) <= 49))
                        return PLD.Atonement;
                }

                // 1-2-3 Combo
                    if (comboTime > 0)
                {
                    if (lastComboMove is PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove is PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                    {
                        if (IsEnabled(CustomComboPreset.PaladinRoyalGoringOption) && level > PLD.Levels.GoringBlade &&
                            ((GetDebuffRemainingTime(PLD.Debuffs.BladeOfValor) > 0 && GetDebuffRemainingTime(PLD.Debuffs.BladeOfValor) < 5) ||
                            (FindTargetEffect(PLD.Debuffs.BladeOfValor) is null && GetDebuffRemainingTime(PLD.Debuffs.GoringBlade) < 5)))
                                return PLD.GoringBlade;

                            return OriginalHook(PLD.RageOfHalone);
                    }
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
            var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

            if (actionID is PLD.Prominence)
            {
                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.PaladinReqAoEComboFeature) && level >= PLD.Levels.Requiescat && IsOffCooldown(PLD.Requiescat))
                            return PLD.Requiescat;

                    if (IsEnabled(CustomComboPreset.PaladinAoEExpiacionScornFeature) && incombat)
                    {
                        if (level >= PLD.Levels.SpiritsWithin && IsOffCooldown(PLD.SpiritsWithin))
                            return OriginalHook(PLD.SpiritsWithin);

                        if (level >= PLD.Levels.CircleOfScorn && IsOffCooldown(PLD.CircleOfScorn))
                            return PLD.CircleOfScorn;
                    }
                }

                if (IsEnabled(CustomComboPreset.PaladinHolyCircleFeature) && HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle && LocalPlayer.CurrentMp >= 1000)
                {
                    if (IsEnabled(CustomComboPreset.PaladinAoEConfiteorFeature) && level >= PLD.Levels.Confiteor &&
                        ((GetBuffRemainingTime(PLD.Buffs.Requiescat) <= 3 && GetBuffRemainingTime(PLD.Buffs.Requiescat) >= 0) || GetBuffStacks(PLD.Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000))
                            return PLD.Confiteor;

                    return PLD.HolyCircle;

                }

                if (IsEnabled(CustomComboPreset.PaladinAoEConfiteorFeature) &&
                    (HasEffect(PLD.Buffs.BladeOfFaithReady) || lastComboMove is PLD.BladeOfFaith || lastComboMove is PLD.BladeOfTruth))
                        return OriginalHook(PLD.Confiteor);

                if (comboTime > 0)
                {
                    if (lastComboMove is PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;
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
            if (actionID is PLD.SpiritsWithin or PLD.CircleOfScorn)
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
            if (actionID is PLD.HolySpirit)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolySpirit)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);

                    if (level >= PLD.Levels.Confiteor &&
                            ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                            requiescat.StackCount is 1 || LocalPlayer.CurrentMp <= 2000))
                            return PLD.Confiteor;

                        return PLD.HolySpirit;
                }

                if (HasEffect(PLD.Buffs.BladeOfFaithReady) || lastComboMove is PLD.BladeOfFaith || lastComboMove is PLD.BladeOfTruth)
                    return OriginalHook(PLD.Confiteor);
            }

            return actionID;
        }
    }
    internal class PaladinStandaloneHolyCircleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinStandaloneHolyCircleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is PLD.HolyCircle)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
                {
                    var requiescat = FindEffect(PLD.Buffs.Requiescat);

                    if (level >= PLD.Levels.Confiteor &&((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime <= 3 && requiescat.RemainingTime > 0) ||
                            requiescat.StackCount is 1 || LocalPlayer.CurrentMp <= 2000))
                            return PLD.Confiteor;

                        return PLD.HolyCircle;
                }

                if (HasEffect(PLD.Buffs.BladeOfFaithReady) || lastComboMove is PLD.BladeOfFaith || lastComboMove is PLD.BladeOfTruth)
                        return OriginalHook(PLD.Confiteor);
            }

            return actionID;
        }
    }
}
