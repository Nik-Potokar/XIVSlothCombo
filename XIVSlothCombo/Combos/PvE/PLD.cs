using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
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

        private static PLDGauge Gauge => CustomComboFunctions.GetJobGauge<PLDGauge>();

        public static class Config
        {
            public static UserInt
                PLD_Intervene_HoldCharges = new("PLDKeepInterveneCharges"),
                PLD_VariantCure = new("PLD_VariantCure"),
                PLD_RequiescatOption = new("PLD_RequiescatOption"),
                PLD_SpiritsWithinOption = new("PLD_SpiritsWithinOption"),
                PLD_SheltronOption = new("PLD_SheltronOption");
            public static UserBool
                PLD_Intervene_MeleeOnly = new("PLD_Intervene_MeleeOnly");

        }

        internal class PLD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FastBlade)
                {

                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
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

                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;
                        }

                        // Actions under FoF burst
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
                                // Confiteor & Blades
                                if ((HasEffect(Buffs.ConfiteorReady) || BladeOfFaith.LevelChecked()) && GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(Confiteor);

                                if (GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(HolySpirit);
                            }

                            // HS under DM
                            if (HasEffect(Buffs.DivineMight) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return OriginalHook(HolySpirit);
                        }

                        // Base combo
                        if (comboTime > 1f)
                        {
                            if (lastComboActionID == OriginalHook(FastBlade) && RiotBlade.LevelChecked())
                                return OriginalHook(RiotBlade);

                            if (lastComboActionID == OriginalHook(RiotBlade) && OriginalHook(RoyalAuthority).LevelChecked())
                                return OriginalHook(RoyalAuthority);
                        }


                        if (CanWeave(actionID))
                        {
                            // FoF (Starts burst)
                            if (FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight))
                                return OriginalHook(FightOrFlight);

                            // Usage outside of burst
                            if (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15)
                            {
                                if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                    return OriginalHook(CircleOfScorn);

                                if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // HS under DM
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
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;
                    }

                    // Actions under FoF burst
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

                    if (comboTime > 1f && lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                        return OriginalHook(Prominence);

                    if (CanWeave(actionID))
                    {
                        if (FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight))
                            return OriginalHook(FightOrFlight);

                        if (!WasLastAction(FightOrFlight) && IsOnCooldown(FightOrFlight))
                        {
                            if (CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                return OriginalHook(CircleOfScorn);

                            if (OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

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
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        // HS when out of range
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
                                IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            // (Holy) Sheltron overcap protection
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) &&
                                Sheltron.LevelChecked() && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron) &&
                                Gauge.OathGauge >= Config.PLD_SheltronOption)
                                return OriginalHook(Sheltron);
                        }

                        // Actions under FoF burst
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                                    Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                    return OriginalHook(Requiescat);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                    CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                    return OriginalHook(CircleOfScorn);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                    OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) &&
                                    OriginalHook(Intervene).LevelChecked() &&
                                    !WasLastAction(Intervene) &&
                                    GetRemainingCharges(Intervene) > Config.PLD_Intervene_HoldCharges &&
                                    ((Config.PLD_Intervene_MeleeOnly && InMeleeRange()) || (!Config.PLD_Intervene_MeleeOnly)))
                                    return OriginalHook(Intervene);
                            }

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                GoringBlade.LevelChecked() &&
                                IsOffCooldown(GoringBlade))
                                return OriginalHook(GoringBlade);

                            // HS when Confiteor not unlocked
                            if (HasEffect(Buffs.Requiescat) && !Confiteor.LevelChecked())
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                    GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(HolySpirit);
                            }
                            else
                            {
                                // Confiteor & Blades
                                if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                                    Confiteor.LevelChecked() && HasEffect(Buffs.ConfiteorReady))
                                    ||
                                    (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                                    BladeOfFaith.LevelChecked() && HasEffect(Buffs.Requiescat) &&
                                    OriginalHook(Confiteor) != Confiteor &&
                                    GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp))
                                    return OriginalHook(Confiteor);
                            }

                            // HS under DM
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                HasEffect(Buffs.DivineMight) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return OriginalHook(HolySpirit);
                        }

                        // FoF (Starts burst)
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) &&
                            FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight) && CanWeave(actionID) &&
                            !ActionWatching.WasLast2ActionsAbilities() &&
                            ActionWatching.CombatActions.Where(x => x == RoyalAuthority).Any())
                            return OriginalHook(FightOrFlight);

                        // Base combo
                        if (comboTime > 1f)
                        {
                            if (lastComboActionID == OriginalHook(FastBlade) && RiotBlade.LevelChecked())
                                return OriginalHook(RiotBlade);

                            if (lastComboActionID == OriginalHook(RiotBlade) && OriginalHook(RoyalAuthority).LevelChecked())
                                return OriginalHook(RoyalAuthority);
                        }

                        if (CanWeave(actionID) && !ActionWatching.WasLast2ActionsAbilities())
                        {
                            // Usage outside of burst (desync for Req, 30s windows for CoS/SW)
                            if (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF))
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                                    Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                    return OriginalHook(Requiescat);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                    CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                    return OriginalHook(CircleOfScorn);

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                    OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // HS under DM (outside of burst)
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return OriginalHook(HolySpirit);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                            GoringBlade.LevelChecked() && IsOffCooldown(GoringBlade) &&
                            IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF))
                            return OriginalHook(GoringBlade);

                        // Confiteor & Blades
                        if (((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                            Confiteor.LevelChecked() && HasEffect(Buffs.ConfiteorReady))
                            ||
                            (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                            BladeOfFaith.LevelChecked() && HasEffect(Buffs.Requiescat) &&
                            OriginalHook(Confiteor) != Confiteor &&
                            GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)))
                            return OriginalHook(Confiteor);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                            HasEffectAny(Buffs.SwordOath) && Atonement.LevelChecked() &&
                            GetCooldownRemainingTime(FightOrFlight) >= 7)
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
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        // (Holy) Sheltron overcap protection
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron) &&
                            Sheltron.LevelChecked() && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron) &&
                            Gauge.OathGauge >= Config.PLD_SheltronOption)
                            return OriginalHook(Sheltron);
                    }

                    // Actions under FoF burst
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) &&
                                Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                return OriginalHook(Requiescat);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) &&
                                CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                return OriginalHook(CircleOfScorn);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) &&
                                OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }

                        // HS when Confiteor not unlocked
                        if (HasEffect(Buffs.Requiescat) && !Confiteor.LevelChecked())
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) &&
                                GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                                return OriginalHook(HolyCircle);
                        }
                        else
                        {
                            // Confiteor & Blades
                            if ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) &&
                                Confiteor.LevelChecked() && HasEffect(Buffs.ConfiteorReady))
                                ||
                                (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) &&
                                BladeOfFaith.LevelChecked() && HasEffect(Buffs.Requiescat) &&
                                OriginalHook(Confiteor) != Confiteor &&
                                GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp))
                                return OriginalHook(Confiteor);
                        }

                        // HS under DM/Req
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && 
                            (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) && 
                            GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp &&
                            HolyCircle.LevelChecked())
                            return OriginalHook(HolyCircle);
                    }

                    if (comboTime > 1f && lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                        return OriginalHook(Prominence);

                    if (CanWeave(actionID))
                    {
                        // FoF (Starts burst)
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && FightOrFlight.LevelChecked() && IsOffCooldown(FightOrFlight))
                            return OriginalHook(FightOrFlight);

                        // Usage outside of burst (desync for Req, 30s windows for CoS/SW)
                        if ((!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF)) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) &&
                                Requiescat.LevelChecked() && IsOffCooldown(Requiescat))
                                return OriginalHook(Requiescat);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) &&
                                CircleOfScorn.LevelChecked() && IsOffCooldown(CircleOfScorn))
                                return OriginalHook(CircleOfScorn);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) &&
                                OriginalHook(SpiritsWithin).LevelChecked() && IsOffCooldown(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }
                    }
                    
                    // Confiteor & Blades
                    if (((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) &&
                        Confiteor.LevelChecked() && HasEffect(Buffs.ConfiteorReady))
                        ||
                        (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) &&
                        BladeOfFaith.LevelChecked() && HasEffect(Buffs.Requiescat) &&
                        OriginalHook(Confiteor) != Confiteor &&
                        GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp)) &&
                        IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF))
                        return OriginalHook(Confiteor);

                    // HS under DM (outside of burst)
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && HasEffect(Buffs.DivineMight) &&
                        GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
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
                    int choice = Config.PLD_RequiescatOption;

                    if ((choice == 1 || choice == 3) && HasEffect(Buffs.ConfiteorReady) && Confiteor.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                        return OriginalHook(Confiteor);

                    if (HasEffect(Buffs.Requiescat))
                    {
                        if ((choice == 2 || choice == 3) && OriginalHook(Confiteor) != Confiteor && BladeOfFaith.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                            return OriginalHook(Confiteor);

                        if (choice == 4 && HolySpirit.LevelChecked() && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return OriginalHook(HolySpirit);

                        if (choice == 5 && HolyCircle.LevelChecked() && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp)
                            return OriginalHook(HolyCircle);
                    }
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
                    int choice = Config.PLD_SpiritsWithinOption;

                    if (IsOffCooldown(CircleOfScorn) && CircleOfScorn.LevelChecked())
                    {
                        if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                        {
                            if (choice == 1)
                                return OriginalHook(CircleOfScorn);

                            if (choice == 2)
                                return OriginalHook(SpiritsWithin);
                        }
                        
                        return OriginalHook(CircleOfScorn);
                    }
                }

                return actionID;
            }
        }
    }
}
