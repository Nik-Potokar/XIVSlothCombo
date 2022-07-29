using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
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
                PLD_Intervene_HoldCharges = "PLDKeepInterveneCharges";
        }

        internal class PLD_GoringBladeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_GoringBladeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is GoringBlade)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove is FastBlade && LevelChecked(RiotBlade))
                            return RiotBlade;

                        if (lastComboMove is RiotBlade && LevelChecked(GoringBlade))
                            return GoringBlade;
                    }

                    return FastBlade;
                }

                return actionID;
            }
        }

        internal class PLD_ST_RoyalAuth : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_RoyalAuth;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RageOfHalone or RoyalAuthority)
                {

                    if (!InCombat())
                    {
                        inOpener = false;
                        openerFinished = false;
                    }
                    else if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_FoFOpener) && LevelChecked(Requiescat) && !openerFinished && !inOpener)
                    {
                        inOpener = true;
                    }

                    // Uptime Feature
                    if (!InMeleeRange() && IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_RangedUptime) &&
                        !(HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth) && HasBattleTarget())
                    {
                        if (LevelChecked(HolySpirit) && LocalPlayer.CurrentMp >= 1000 && !this.IsMoving && !HasEffect(Buffs.Requiescat) ||
                            LevelChecked(Confiteor) && HasEffect(Buffs.Requiescat) && GetBuffStacks(Buffs.Requiescat) is not 1 && (GetBuffRemainingTime(Buffs.Requiescat) >= 3) && LocalPlayer.CurrentMp >= 2000)
                        {
                            return HolySpirit;
                        }
                        else if (LevelChecked(ShieldLob) && (!HasEffect(Buffs.Requiescat) || LocalPlayer.CurrentMp < 1000))
                        {
                            return ShieldLob;
                        }
                    }

                    // Buffs
                    if (GetCooldown(actionID).CooldownRemaining < 0.9 && GetCooldown(actionID).CooldownRemaining > 0.2)
                    {
                        if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_FoF) && LevelChecked(FightOrFlight) && lastComboMove is FastBlade && IsOffCooldown(FightOrFlight))
                            return FightOrFlight;
                        if (!inOpener && IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Requiescat) && LevelChecked(Requiescat) && HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) < 17 && IsOffCooldown(Requiescat))
                            return Requiescat;
                    }

                    // oGCD features
                    if (CanWeave(actionID))
                    {
                        if (inOpener)
                        {
                            if (lastComboMove is Confiteor || (!HasEffect(Buffs.Requiescat) && IsOnCooldown(Requiescat) && GetCooldownRemainingTime(Requiescat) <= 59))
                            {
                                inOpener = false;
                                openerFinished = true;
                            }

                            if (HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) <= 19)
                            {
                                if (lastComboMove is not FastBlade && TargetHasEffect(Debuffs.GoringBlade))
                                {
                                    if (IsOffCooldown(CircleOfScorn))
                                        return CircleOfScorn;
                                    if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_FoFOpener_Intervene) && LevelChecked(Intervene) && GetRemainingCharges(Intervene) == 2)
                                        return Intervene;
                                    if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                                        return OriginalHook(SpiritsWithin);
                                    if (IsOffCooldown(Requiescat))
                                        return Requiescat;
                                    if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_FoFOpener_Intervene) && LevelChecked(Intervene) && GetRemainingCharges(Intervene) > 0)
                                        return Intervene;
                                }

                                if (GetBuffRemainingTime(Buffs.FightOrFlight) <= 8)
                                {
                                    if (IsOffCooldown(Requiescat))
                                        return Requiescat;
                                    if (IsOffCooldown(CircleOfScorn))
                                        return CircleOfScorn;
                                    if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                                        return OriginalHook(SpiritsWithin);
                                    if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_FoFOpener_Intervene) && LevelChecked(Intervene) && GetRemainingCharges(Intervene) > 0)
                                        return Intervene;
                                }
                            }
                        }
                        else
                        {
                            if (IsEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn) && LevelChecked(SpiritsWithin) && InCombat() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                            {
                                if (IsNotEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn_FoFOption) ||
                                    (IsEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn_FoFOption) && HasEffect(Buffs.FightOrFlight) || IsOnCooldown(FightOrFlight)))
                                    return OriginalHook(SpiritsWithin);
                            }

                            if (IsEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn) && LevelChecked(CircleOfScorn) && InCombat() && IsOffCooldown(CircleOfScorn))
                            {
                                if (IsNotEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn_FoFOption) ||
                                    (IsEnabled(CustomComboPreset.PLD_RoyalAuth_ExpiacionScorn_FoFOption) && HasEffect(Buffs.FightOrFlight) || IsOnCooldown(FightOrFlight)))
                                    return CircleOfScorn;
                            }

                            var interveneChargesRemaining = GetOptionValue(Config.PLD_Intervene_HoldCharges);
                            if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Intervene) && LevelChecked(Intervene) && GetRemainingCharges(Intervene) > interveneChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Intervene_Melee) ||
                                    (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Intervene_Melee) && HasEffect(Buffs.FightOrFlight) && GetTargetDistance() <= 1))
                                {
                                    if (!IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Requiescat) || GetCooldownRemainingTime(Requiescat) >= 3)
                                        return Intervene;
                                }
                            }
                        }
                    }

                    // GCDs
                    if (IsEnabled(CustomComboPreset.PLD_RoyalAuth_Requiescat_HolySpirit))
                    {
                        if (inOpener)
                        {
                            if (lastComboMove is GoringBlade && HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) <= 3)
                                return HolySpirit;
                        }

                        if (HasEffect(Buffs.Requiescat) && LevelChecked(HolySpirit) && (!HasEffect(Buffs.FightOrFlight) || !InMeleeRange()) && LocalPlayer.CurrentMp >= 1000)
                        {
                            if (IsEnabled(CustomComboPreset.PLD_RoyalAuth_Requiescat_Confiteor) && LevelChecked(Confiteor) &&
                                ((GetBuffRemainingTime(Buffs.Requiescat) <= 3 && GetBuffRemainingTime(Buffs.Requiescat) >= 0) || GetBuffStacks(Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000)) //Confiteor Conditions
                                return Confiteor;

                            return HolySpirit;
                        }

                        if (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth)
                            return OriginalHook(Confiteor);
                    }

                    if (LevelChecked(Atonement) && HasEffect(Buffs.SwordOath) && IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Atonement))
                    {
                        if (IsNotEnabled(CustomComboPreset.PLD_AtonementDrop))
                            return Atonement;

                        if (IsEnabled(CustomComboPreset.PLD_AtonementDrop))
                        {
                            if (HasEffect(Buffs.FightOrFlight))
                            {
                                if (lastComboMove == Atonement || lastComboMove == RoyalAuthority)
                                {
                                    return Atonement;
                                }
                            }
                            else if (GetBuffStacks(Buffs.SwordOath) > 1)
                            {
                                return Atonement;
                            }
                        }
                    }

                    // 1-2-3 Combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is FastBlade && LevelChecked(RiotBlade))
                            return RiotBlade;

                        if (lastComboMove is RiotBlade && LevelChecked(RageOfHalone))
                        {
                            if (IsEnabled(CustomComboPreset.PLD_ST_RoyalAuth_Goring) && LevelChecked(GoringBlade) &&
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

        internal class PLD_AoE_Prominence : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_Prominence;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Prominence)
                {
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.PLD_AoE_Prominence_HolyCircle_Requiescat) && LevelChecked(Requiescat) && IsOffCooldown(Requiescat))
                            return Requiescat;

                        if (IsEnabled(CustomComboPreset.PLD_AoE_Prominence_ExpiacionScorn) && InCombat())
                        {
                            if (LevelChecked(SpiritsWithin) && IsOffCooldown(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);

                            if (LevelChecked(CircleOfScorn) && IsOffCooldown(CircleOfScorn))
                                return CircleOfScorn;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.PLD_AoE_Prominence_HolyCircle) && HasEffect(Buffs.Requiescat) && LevelChecked(HolyCircle) && LocalPlayer.CurrentMp >= 1000)
                    {
                        if (IsEnabled(CustomComboPreset.PLD_AoE_Prominence_HolyCircle_Confiteor) && LevelChecked(Confiteor) &&
                            ((GetBuffRemainingTime(Buffs.Requiescat) <= 3 && GetBuffRemainingTime(Buffs.Requiescat) >= 0) || GetBuffStacks(Buffs.Requiescat) is 1 || LocalPlayer.CurrentMp <= 2000))
                            return Confiteor;

                        return HolyCircle;

                    }

                    if (IsEnabled(CustomComboPreset.PLD_AoE_Prominence_HolyCircle_Confiteor) &&
                        (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth))
                        return OriginalHook(Confiteor);

                    if (comboTime > 0)
                    {
                        if (lastComboMove is TotalEclipse && LevelChecked(Prominence))
                            return Prominence;
                    }

                    return TotalEclipse;
                }

                return actionID;
            }
        }

        internal class PLD_ScornfulSpirits : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ScornfulSpirits;

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

        internal class PLD_HolySpirit_Standalone : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_HolySpirit_Standalone;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HolySpirit)
                {
                    if (HasEffect(Buffs.Requiescat) && LevelChecked(HolySpirit))
                    {
                        var requiescatTime = GetBuffRemainingTime(Buffs.Requiescat);
                        var requiescatStacks = GetBuffStacks(Buffs.Requiescat);

                        if (LevelChecked(Confiteor) &&
                                ((IsEnabled(CustomComboPreset.PLD_RoyalAuth_Requiescat_Confiteor) && requiescatTime is <= 3 and > 0) ||
                                requiescatStacks is 1 || LocalPlayer.CurrentMp <= 2000))
                            return Confiteor;

                        return HolySpirit;
                    }

                    if (HasEffect(Buffs.BladeOfFaithReady) || lastComboMove is BladeOfFaith || lastComboMove is BladeOfTruth)
                        return OriginalHook(Confiteor);
                }

                return actionID;
            }
        }

        internal class PLD_HolyCircle_Standalone : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_HolyCircle_Standalone;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HolyCircle)
                {
                    if (HasEffect(Buffs.Requiescat) && LevelChecked(HolyCircle))
                    {
                        var requiescatTime = GetBuffRemainingTime(Buffs.Requiescat);
                        var requiescatStacks = GetBuffStacks(Buffs.Requiescat);

                        if (LevelChecked(Confiteor) && ((IsEnabled(CustomComboPreset.PLD_RoyalAuth_Requiescat_Confiteor) && requiescatTime is <= 3 and > 0) ||
                                requiescatStacks is 1 || LocalPlayer.CurrentMp <= 2000))
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
