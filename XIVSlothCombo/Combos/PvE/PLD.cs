using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

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
            Intervene = 16461,
            Sheltron = 3542;

        public static class Buffs
        {
            public const ushort
                Requiescat = 1368,
                SwordOath = 1902,
                FightOrFlight = 76,
                ConfiteorReady = 3019,
                DivineMight = 2673,
                HolySheltron = 2674,
                Sheltron = 1856;
        }

        public static class Debuffs
        {
            public const ushort
                BladeOfValor = 2721,
                GoringBlade = 725;
        }


        public static class Config
        {
            public const string
                PLD_Intervene_HoldCharges = "PLDKeepInterveneCharges",
                PLD_Intervene_MeleeOnly = "PLD_Intervene_MeleeOnly",
                PLD_VariantCure = "PLD_VariantCure",
                PLD_RequiescatOption = "PLD_RequiescatOption",
                PLD_SpiritsWithinOption = "PLD_SpiritsWithinOption",
                PLD_SheltronOption = "PLD_SheltronOption";
        }

        internal class PLD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FastBlade)
                {

                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.PLD_VariantCure))
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (!InMeleeRange() && HolySpirit.LevelChecked() && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && !IsMoving)
                            return OriginalHook(HolySpirit);

                        if (!InMeleeRange() && ShieldLob.LevelChecked())
                            return OriginalHook(ShieldLob);

                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                        }

                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                if (Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                    return OriginalHook(Requiescat);

                                if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                    return OriginalHook(CircleOfScorn);

                                if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);

                            }

                            if (GoringBlade.LevelChecked() && IsOffCooldown(GoringBlade))
                                return OriginalHook(GoringBlade);

                            if (HasEffect(Buffs.Requiescat))
                            {
                                if ((HasEffect(Buffs.ConfiteorReady) || BladeOfFaith.LevelChecked()) && GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(Confiteor);

                                if (GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(HolySpirit);
                            }


                            if (HasEffect(Buffs.DivineMight) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return OriginalHook(HolySpirit);

                        }


                        if (comboTime > 1f)
                        {
                            if (lastComboActionID == OriginalHook(FastBlade) && RiotBlade.LevelChecked())
                            {
                                return OriginalHook(RiotBlade);
                            }

                            if (lastComboActionID == OriginalHook(RiotBlade) && OriginalHook(RoyalAuthority).LevelChecked())
                            {
                                return OriginalHook(RoyalAuthority);
                            }

                        }

                        if (FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight) && CanWeave(actionID))
                            return OriginalHook(FightOrFlight);

                        if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn) && CanWeave(actionID) && !WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15)
                            return OriginalHook(CircleOfScorn);

                        if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)) && CanWeave(actionID) && !WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15)
                            return OriginalHook(SpiritsWithin);

                        if (HasEffect(Buffs.DivineMight) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return OriginalHook(HolySpirit);

                        if (HasEffectAny(Buffs.SwordOath) && Atonement.LevelChecked())
                            return OriginalHook(Atonement);

                    }
                }

                return actionID;
            }
        }

        internal class PLD_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is TotalEclipse)
                {
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.PLD_VariantCure))
                        return Variant.VariantCure;

                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                    }


                    if (HasEffect(Buffs.FightOrFlight))
                    {
                        if (CanWeave(actionID))
                        {
                            if (Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                return OriginalHook(Requiescat);

                            if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                return OriginalHook(CircleOfScorn);

                            if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);

                        }


                        if (HasEffect(Buffs.Requiescat))
                        {
                            if ((HasEffect(Buffs.ConfiteorReady) || BladeOfFaith.LevelChecked()) && GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)
                                return OriginalHook(Confiteor);

                            if (GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && HolyCircle.LevelChecked())
                                return OriginalHook(HolyCircle);

                            if (GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && HolySpirit.LevelChecked())
                                return OriginalHook(HolySpirit);
                        }

                        if (HasEffect(Buffs.DivineMight) && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && HolyCircle.LevelChecked())
                            return OriginalHook(HolyCircle);
                    }

                    if (comboTime > 1f)
                    {
                        if (lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                            return OriginalHook(Prominence);
                    }


                    if (FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight) && CanWeave(actionID))
                        return OriginalHook(FightOrFlight);

                    if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn) && CanWeave(actionID) && !WasLastAction(FightOrFlight) && IsOnCooldown(FightOrFlight))
                        return OriginalHook(CircleOfScorn);

                    if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)) && CanWeave(actionID) && !WasLastAction(FightOrFlight) && IsOnCooldown(FightOrFlight))
                        return OriginalHook(SpiritsWithin);

                    if ((HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                        return OriginalHook(HolyCircle);


                    return actionID;
                }

                return actionID;
            }
        }

        internal class PLD_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FastBlade)
                {

                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.PLD_VariantCure))
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            !InMeleeRange() &&
                            HolySpirit.LevelChecked() &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp &&
                            !IsMoving)
                            return OriginalHook(HolySpirit);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob) &&
                            !InMeleeRange() &&
                            ShieldLob.LevelChecked())
                            return OriginalHook(ShieldLob);

                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) &&
                                IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) &&
                                Sheltron.LevelChecked() &&
                                !HasEffect(Buffs.Sheltron) &&
                                !HasEffect(Buffs.HolySheltron) &&
                                GetJobGauge<PLDGauge>().OathGauge >= GetOptionValue(Config.PLD_SheltronOption)
                                )
                                return OriginalHook(Sheltron);

                        }

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                                    Requiescat.LevelChecked() &&
                                    IsOffCooldown(Requiescat))
                                    return OriginalHook(Requiescat);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                    CircleOfScorn.LevelChecked() &&
                                    IsOffCooldown(CircleOfScorn))
                                    return OriginalHook(CircleOfScorn);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                    OriginalHook(SpiritsWithin).LevelChecked() &&
                                    IsOffCooldown(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) &&
                                    OriginalHook(Intervene).LevelChecked() &&
                                    GetRemainingCharges(Intervene) > GetOptionValue(Config.PLD_Intervene_HoldCharges) &&
                                    ((GetOptionBool(Config.PLD_Intervene_MeleeOnly) && InMeleeRange()) || (!GetOptionBool(Config.PLD_Intervene_MeleeOnly))))
                                    return OriginalHook(Intervene);

                            }

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                GoringBlade.LevelChecked() &&
                                IsOffCooldown(GoringBlade))
                                return OriginalHook(GoringBlade);


                            if (HasEffect(Buffs.Requiescat) && !Confiteor.LevelChecked())
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                    GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(HolySpirit);
                            }
                            else
                            {
                                if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                                    Confiteor.LevelChecked() &&
                                    HasEffect(Buffs.ConfiteorReady))
                                    ||
                                    (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                                    BladeOfFaith.LevelChecked() &&
                                    HasEffect(Buffs.Requiescat) &&
                                    OriginalHook(Confiteor) != Confiteor &&
                                    GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp))
                                    return OriginalHook(Confiteor);
                            }

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                HasEffect(Buffs.DivineMight) &&
                                GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return OriginalHook(HolySpirit);

                        }

                        if (comboTime > 1f)
                        {
                            if (lastComboActionID == OriginalHook(FastBlade) &&
                                RiotBlade.LevelChecked())
                            {
                                return OriginalHook(RiotBlade);
                            }

                            if (lastComboActionID == OriginalHook(RiotBlade) &&
                                OriginalHook(RoyalAuthority).LevelChecked())
                            {
                                return OriginalHook(RoyalAuthority);
                            }

                        }

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) &&
                            FightOrFlight.LevelChecked() &&
                            IsOffCooldown(FightOrFlight) &&
                            CanWeave(actionID) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                            return OriginalHook(FightOrFlight);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                            Requiescat.LevelChecked() && IsOffCooldown(Requiescat) &&
                            CanWeave(actionID) &&
                            (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF)) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                            return OriginalHook(Requiescat);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                            CircleOfScorn.LevelChecked() &&
                            IsOffCooldown(CircleOfScorn) &&
                            CanWeave(actionID) &&
                            (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF)) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                            return OriginalHook(CircleOfScorn);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                            OriginalHook(SpiritsWithin).LevelChecked() &&
                            IsOffCooldown(OriginalHook(SpiritsWithin)) &&
                            CanWeave(actionID) &&
                            (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF)) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                            return OriginalHook(SpiritsWithin);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return OriginalHook(HolySpirit);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                            GoringBlade.LevelChecked() &&
                            IsOffCooldown(GoringBlade) &&
                            IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF))
                            return OriginalHook(GoringBlade);

                        if (((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                            Confiteor.LevelChecked() &&
                            HasEffect(Buffs.ConfiteorReady))
                            ||
                            (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                            BladeOfFaith.LevelChecked() &&
                            HasEffect(Buffs.Requiescat) &&
                            OriginalHook(Confiteor) != Confiteor &&
                            GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)))
                            return OriginalHook(Confiteor);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                            HasEffectAny(Buffs.SwordOath) &&
                            Atonement.LevelChecked())
                            return OriginalHook(Atonement);

                    }
                }

                return actionID;
            }
        }

        internal class PLD_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is TotalEclipse)
                {
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.PLD_VariantCure))
                        return Variant.VariantCure;

                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron) &&
                            Sheltron.LevelChecked() &&
                            !HasEffect(Buffs.Sheltron) &&
                            !HasEffect(Buffs.HolySheltron) &&
                            GetJobGauge<PLDGauge>().OathGauge >= GetOptionValue(Config.PLD_SheltronOption)
                            )
                            return OriginalHook(Sheltron);

                    }


                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) && Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                return OriginalHook(Requiescat);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) && CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                return OriginalHook(CircleOfScorn);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) && OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);

                        }

                        if (HasEffect(Buffs.Requiescat) && !Confiteor.LevelChecked())
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) &&
                                GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                                return OriginalHook(HolyCircle);
                        }
                        else
                        {
                            if ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) &&
                                Confiteor.LevelChecked() &&
                                HasEffect(Buffs.ConfiteorReady))
                                ||
                                (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) &&
                                BladeOfFaith.LevelChecked() &&
                                HasEffect(Buffs.Requiescat) &&
                                OriginalHook(Confiteor) != Confiteor &&
                                GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp))
                                return OriginalHook(Confiteor);
                        }

                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && 
                            (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) && 
                            GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp &&
                            HolyCircle.LevelChecked())
                            return OriginalHook(HolyCircle);
                    }

                    if (comboTime > 1f)
                    {
                        if (lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                            return OriginalHook(Prominence);
                    }


                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight) && CanWeave(actionID))
                        return OriginalHook(FightOrFlight);

                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) &&
                        Requiescat.LevelChecked() && IsOffCooldown(Requiescat) &&
                        CanWeave(actionID) &&
                        (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF)) &&
                        !ActionWatching.WasLast2ActionsAbilities())
                        return OriginalHook(Requiescat);

                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) &&
                        CircleOfScorn.LevelChecked() &&
                        IsOffCooldown(CircleOfScorn) &&
                        CanWeave(actionID) &&
                        (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF)) &&
                        !ActionWatching.WasLast2ActionsAbilities())
                        return OriginalHook(CircleOfScorn);

                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) &&
                        OriginalHook(SpiritsWithin).LevelChecked() &&
                        IsOffCooldown(OriginalHook(SpiritsWithin)) &&
                        CanWeave(actionID) &&
                        (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF)) &&
                        !ActionWatching.WasLast2ActionsAbilities())
                        return OriginalHook(SpiritsWithin);

                    if (((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) &&
                        Confiteor.LevelChecked() &&
                        HasEffect(Buffs.ConfiteorReady))
                        ||
                        (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) &&
                        BladeOfFaith.LevelChecked() &&
                        HasEffect(Buffs.Requiescat) &&
                        OriginalHook(Confiteor) != Confiteor &&
                        GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)) &&
                        IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF))
                        return OriginalHook(Confiteor);

                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && HasEffect(Buffs.DivineMight) && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                        return OriginalHook(HolyCircle);


                    return actionID;
                }

                return actionID;
            }
        }

        internal class PLD_Requiescat_Confiteor : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_Requiescat_Options;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Requiescat)
                {
                    var choice = GetOptionValue(Config.PLD_RequiescatOption);

                    if ((choice == 1 || choice == 3) && HasEffect(Buffs.ConfiteorReady) && Confiteor.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                        return OriginalHook(Confiteor);

                    if ((choice == 2 || choice == 3) && HasEffect(Buffs.Requiescat) && OriginalHook(Confiteor) != Confiteor && BladeOfFaith.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                        return OriginalHook(Confiteor);

                    if (choice == 4 && HasEffect(Buffs.Requiescat) && HolySpirit.LevelChecked() && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                        return OriginalHook(HolySpirit);

                    if (choice == 5 && HasEffect(Buffs.Requiescat) && HolyCircle.LevelChecked() && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp)
                        return OriginalHook(HolyCircle);

                }

                return actionID;
            }
        }

        internal class PLD_CircleOfScorn : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_SpiritsWithin;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == OriginalHook(SpiritsWithin))
                {
                    var choice = GetOptionValue(Config.PLD_SpiritsWithinOption);

                    if (choice == 1 && IsOffCooldown(CircleOfScorn) && CircleOfScorn.LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                        return OriginalHook(CircleOfScorn);

                    if (choice == 2 && IsOffCooldown(CircleOfScorn) && CircleOfScorn.LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                        return OriginalHook(SpiritsWithin);

                    if (IsOffCooldown(CircleOfScorn) && CircleOfScorn.LevelChecked())
                        return OriginalHook(CircleOfScorn);

                }
                return actionID;
            }
        }
    }
}
