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


        internal class PaladinGoringBladeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinGoringBladeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is GoringBlade)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove is FastBlade && level >= Levels.RiotBlade)
                            return RiotBlade;

                        if (lastComboMove is RiotBlade && level >= Levels.GoringBlade)
                            return GoringBlade;
                    }

                    return FastBlade;
                }

                return actionID;
            }
        }

        internal class PaladinRoyalAuthorityCombo : CustomCombo
        {

            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinRoyalAuthorityCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RageOfHalone or RoyalAuthority)
                {
                    var interveneChargesRemaining = Service.Configuration.GetCustomIntValue(Config.PLDKeepInterveneCharges);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                    // Uptime Features
                    if (!InMeleeRange() && !(HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth))
                    {
                        if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature) && level >= Levels.ShieldLob && !HasEffect(Buffs.Requiescat))
                            return ShieldLob;
                        if (IsEnabled(CustomComboPreset.PaladinRangedUptimeFeature2) && level >= Levels.HolySpirit)
                            return HolySpirit;
                    }

                    // Buffs
                    if (GetCooldown(actionID).CooldownRemaining < 0.9 && GetCooldown(actionID).CooldownRemaining > 0.2)
                    {
                        if (IsEnabled(CustomComboPreset.PaladinFightOrFlightFeature) && level >= Levels.FightOrFlight && lastComboMove is FastBlade && IsOffCooldown(FightOrFlight))
                            return FightOrFlight;
                        if (IsEnabled(CustomComboPreset.PaladinReqMainComboFeature) && level >= Levels.Requiescat && HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) < 17 && IsOffCooldown(Requiescat))
                            return Requiescat;
                    }

                    // oGCD features
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.PaladinExpiacionScornFeature) && incombat && IsOffCooldown(OriginalHook(SpiritsWithin)) && level >= Levels.SpiritsWithin)
                        {
                            if (IsNotEnabled(CustomComboPreset.PaladinExpiacionScornOption) ||
                                (IsEnabled(CustomComboPreset.PaladinExpiacionScornOption) && HasEffect(Buffs.FightOrFlight) || IsOnCooldown(FightOrFlight)))
                                return OriginalHook(SpiritsWithin);
                        }
                        
                        if (IsEnabled(CustomComboPreset.PaladinExpiacionScornFeature) && incombat && IsOffCooldown(CircleOfScorn) && level >= Levels.CircleOfScorn)
                        {
                            if (IsNotEnabled(CustomComboPreset.PaladinExpiacionScornOption) ||
                                (IsEnabled(CustomComboPreset.PaladinExpiacionScornOption) && HasEffect(Buffs.FightOrFlight) || IsOnCooldown(FightOrFlight)))
                                return CircleOfScorn;
                        }

                        if (IsEnabled(CustomComboPreset.PaladinInterveneFeature) && level >= Levels.Intervene && GetRemainingCharges(Intervene) > interveneChargesRemaining)
                        {
                            if (IsNotEnabled(CustomComboPreset.PaladinMeleeInterveneOption) ||
                                (IsEnabled(CustomComboPreset.PaladinMeleeInterveneOption) && HasEffect(Buffs.FightOrFlight) && GetTargetDistance() <= 1))
                                return Intervene;
                        }
                    }

                    // GCDs
                    if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                    {
                        if (HasEffect(Buffs.Requiescat) && level >= Levels.HolySpirit && !HasEffect(Buffs.FightOrFlight) && LocalPlayer.CurrentMp >= 1000)
                        {
                            if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= Levels.Confiteor &&
                                ((GetBuffRemainingTime(Buffs.Requiescat) <= 3 && GetBuffRemainingTime(Buffs.Requiescat) >= 0) || GetBuffStacks(Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000)) //Confiteor Conditions
                                return Confiteor;

                            return HolySpirit;
                        }

                        if (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth)
                            return OriginalHook(Confiteor);
                    }

                    if (level >= Levels.Atonement && HasEffect(Buffs.SwordOath) && IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                    {
                        if (IsNotEnabled(CustomComboPreset.PaladinAtonementDropFeature))
                            return Atonement;

                        if ((IsEnabled(CustomComboPreset.PaladinAtonementDropFeature) &&
                             GetCooldownRemainingTime(FightOrFlight) <= 15 && GetBuffStacks(Buffs.SwordOath) > 1) ||
                            (HasEffect(Buffs.Requiescat) && GetCooldownRemainingTime(FightOrFlight) <= 49))
                            return Atonement;
                    }

                    // 1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is FastBlade && level >= Levels.RiotBlade)
                            return RiotBlade;

                        if (lastComboMove is RiotBlade && level >= Levels.RageOfHalone)
                        {
                            if (IsEnabled(CustomComboPreset.PaladinRoyalGoringOption) && level > Levels.GoringBlade &&
                                ((GetDebuffRemainingTime(Debuffs.BladeOfValor) > 0 && GetDebuffRemainingTime(Debuffs.BladeOfValor) < 5) ||
                                (FindTargetEffect(Debuffs.BladeOfValor) is null && GetDebuffRemainingTime(Debuffs.GoringBlade) < 5)))
                                return GoringBlade;

                            return OriginalHook(RageOfHalone);
                        }
                    }

                    return FastBlade;
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

                if (actionID is Prominence)
                {
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.PaladinReqAoEComboFeature) && level >= Levels.Requiescat && IsOffCooldown(Requiescat))
                            return Requiescat;

                        if (IsEnabled(CustomComboPreset.PaladinAoEExpiacionScornFeature) && incombat)
                        {
                            if (level >= Levels.SpiritsWithin && IsOffCooldown(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);

                            if (level >= Levels.CircleOfScorn && IsOffCooldown(CircleOfScorn))
                                return CircleOfScorn;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.PaladinHolyCircleFeature) && HasEffect(Buffs.Requiescat) && level >= Levels.HolyCircle && LocalPlayer.CurrentMp >= 1000)
                    {
                        if (IsEnabled(CustomComboPreset.PaladinAoEConfiteorFeature) && level >= Levels.Confiteor &&
                            ((GetBuffRemainingTime(Buffs.Requiescat) <= 3 && GetBuffRemainingTime(Buffs.Requiescat) >= 0) || GetBuffStacks(Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000))
                            return Confiteor;

                        return HolyCircle;

                    }

                    if (IsEnabled(CustomComboPreset.PaladinAoEConfiteorFeature) &&
                        (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth))
                        return OriginalHook(Confiteor);

                    if (comboTime > 0)
                    {
                        if (lastComboMove is TotalEclipse && level >= Levels.Prominence)
                            return Prominence;
                    }

                    return TotalEclipse;
                }

                return actionID;
            }
        }

        internal class PaladinScornfulSpiritsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpiritsWithin or CircleOfScorn)
                {
                    if (level is >= Levels.SpiritsWithin and <= Levels.Expiacion)
                        return CalcBestAction(actionID, SpiritsWithin, CircleOfScorn);

                    if (level >= Levels.Expiacion)
                        return CalcBestAction(actionID, Expiacion, CircleOfScorn);

                    if (level >= Levels.CircleOfScorn)
                        return CalcBestAction(actionID, SpiritsWithin, CircleOfScorn);

                    return SpiritsWithin;
                }

                return actionID;
            }
        }
        internal class PaladinStandaloneHolySpiritFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinStandaloneHolySpiritFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HolySpirit)
                {
                    if (HasEffect(Buffs.Requiescat) && level >= Levels.HolySpirit)
                    {
                        var requiescat = FindEffect(Buffs.Requiescat);

                        if (level >= Levels.Confiteor &&
                                ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime is <= 3 and > 0) ||
                                requiescat.StackCount is 1 || LocalPlayer.CurrentMp <= 2000))
                            return Confiteor;

                        return HolySpirit;
                    }

                    if (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth)
                        return OriginalHook(Confiteor);
                }

                return actionID;
            }
        }
        internal class PaladinStandaloneHolyCircleFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinStandaloneHolyCircleFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HolyCircle)
                {
                    if (HasEffect(Buffs.Requiescat) && level >= Levels.HolyCircle)
                    {
                        var requiescat = FindEffect(Buffs.Requiescat);

                        if (level >= Levels.Confiteor && ((IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && requiescat.RemainingTime is <= 3 and > 0) ||
                                requiescat.StackCount is 1 || LocalPlayer.CurrentMp <= 2000))
                            return Confiteor;

                        return HolyCircle;
                    }

                    if (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth)
                        return OriginalHook(Confiteor);
                }

                return actionID;
            }
        }
    }
}
