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
            //Supplication = 36918, // Second Atonement
            //Sepulchre = 36919, // Third Atonement
            Intervene = 16461,
            BladeOfHonor = 36922,
            Sheltron = 3542;

        public static class Buffs
        {
            public const ushort
                Requiescat = 1368,
                SwordOath = 1902,
                SupplicationReady = 3827, //Second Atonement buff
                SepulchreReady = 3828, // Third Atonement buff
                GoringBladeReady = 3847, //Goring Blade Buff after use of FoF
                BladeOfHonor = 3831, // BladeOfHonor Buff after Confitiour Combo. (oGCD)
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
                PLD_SheltronOption = new("PLD_SheltronOption"),
                PLD_ST_RequiescatWeave = new("PLD_ST_RequiescatWeave"),
                PLD_AoE_RequiescatWeave = new("PLD_AoE_RequiescatWeave"),
                PLD_ST_AtonementTiming = new("PLD_ST_EquilibriumTiming"),
                PLD_ST_DivineMightTiming = new("PLD_ST_DivineMightTiming");
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

                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (!InMeleeRange() && HolySpirit.LevelChecked() &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && (!IsMoving || HasEffect(Buffs.DivineMight)))
                            return HolySpirit;

                        if (!InMeleeRange() && ShieldLob.LevelChecked())
                            return ShieldLob;

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
                        }

                        // Requiescat inside burst (checking for FoF buff causes a late weave and can misalign over long fights with some ping)
                        if (CanWeave(actionID) && (WasLastAbility(FightOrFlight) || JustUsed(FightOrFlight, 6f)) && ActionReady(Requiescat))
                            return OriginalHook(Requiescat);

                        // Actions under FoF burst
                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                if (ActionReady(CircleOfScorn))
                                    return CircleOfScorn;

                                if (ActionReady(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);
                            }

                            // Lv90-100 Goring use
                            if  (HasEffect(Buffs.GoringBladeReady) && InMeleeRange() &&
                                (WasLastSpell(BladeOfValor) && !HasEffect(Buffs.BladeOfHonor) || WasLastAbility(BladeOfHonor)))
                                return GoringBlade;

                            // Lv80-89 Goring use
                            if  (HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(BladeOfFaith) && !LevelChecked(BladeOfHonor) && WasLastSpell(Confiteor))
                                return GoringBlade;

                            // Lv68-79 Goring use
                            if  (HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(Confiteor) && WasLastSpell(HolySpirit))
                                return GoringBlade;

                            // Lv54-68 Goring use
                            if  (HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(HolySpirit) && WasLastSpell(HolySpirit))
                                return GoringBlade;

                            if (HasEffect(Buffs.Requiescat))
                            {
                                // Confiteor & Blades
                                if ((HasEffect(Buffs.ConfiteorReady) || BladeOfFaith.LevelChecked()) &&
                                    GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                                    return OriginalHook(Confiteor);

                                if (GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return HolySpirit;
                            }

                            // New Spell after Confi Combo (Weave)
                            if (CanWeave(actionID) && HasEffect(Buffs.BladeOfHonor) && WasLastSpell(BladeOfValor))
                                return OriginalHook(Requiescat);

                            // HS under DM
                            if (HasEffect(Buffs.DivineMight) &&
                                GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return HolySpirit;
                        }

                        if (CanWeave(actionID))
                        {
                            // FoF (Starts burst)
                            if (ActionReady(FightOrFlight) &&
                                ActionWatching.CombatActions.Where(x => x == OriginalHook(RoyalAuthority)).Any()) // Check RA has been used for opener exception
                                return FightOrFlight;

                            // Usage outside of burst
                            if (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15)
                            {
                                if (ActionReady(CircleOfScorn))
                                    return CircleOfScorn;

                                if (ActionReady(OriginalHook(SpiritsWithin)))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // Base combo
                        if (comboTime > 0)
                        {
                            if (lastComboActionID is FastBlade &&
                                RiotBlade.LevelChecked())
                                return RiotBlade;

                            if (lastComboActionID is RiotBlade &&
                                RageOfHalone.LevelChecked())
                            {
                                if (HasEffect(Buffs.SwordOath) || HasEffect(Buffs.SepulchreReady) || HasEffect(Buffs.SupplicationReady))
                                    return OriginalHook(Atonement);

                                return (HasEffect(Buffs.DivineMight) &&
                                        GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    ? HolySpirit
                                    : OriginalHook(RageOfHalone);
                            }
                        }
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
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) &&
                            IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;
                    }

                    // Requiescat inside burst (checking for FoF buff causes a late weave and can misalign over long fights with some ping)
                    if (CanWeave(actionID) && (WasLastAbility(FightOrFlight) || JustUsed(FightOrFlight, 6f)) && ActionReady(Requiescat))
                        return OriginalHook(Requiescat);

                    // Actions under FoF burst
                    if (HasEffect(Buffs.FightOrFlight))
                    {
                        if (CanWeave(actionID))
                        {
                            if (ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (ActionReady(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }

                        if (HasEffect(Buffs.Requiescat))
                        {
                            if ((HasEffect(Buffs.ConfiteorReady) || BladeOfFaith.LevelChecked()) &&
                                GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                                return OriginalHook(Confiteor);

                            if (HolyCircle.LevelChecked() &&
                                GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp)
                                return HolyCircle;

                            if (HolySpirit.LevelChecked() &&
                                GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return HolySpirit;
                        }

                        if (HasEffect(Buffs.DivineMight) &&
                            GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && HolyCircle.LevelChecked())
                            return HolyCircle;
                    }

                    if (comboTime > 0 && lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                        return Prominence;

                    if (CanWeave(actionID))
                    {
                        if (ActionReady(FightOrFlight))
                            return FightOrFlight;

                        if (!WasLastAction(FightOrFlight) && IsOnCooldown(FightOrFlight))
                        {
                            if (ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (ActionReady(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    if ((HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) &&
                        GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                        return HolyCircle;

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
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (!InMeleeRange() && !HasEffect(Buffs.Requiescat))
                        {
                            // HS when out of range
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                (!IsMoving || HasEffect(Buffs.DivineMight)) &&
                                HolySpirit.LevelChecked() &&
                                GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return HolySpirit;

                            // Shield lob uptime only if unable to stop and HS
                            // (arguably better to delay by less than a whole GCD and just stop moving to cast)
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob) &&
                                ShieldLob.LevelChecked() &&
                                ((HolySpirit.LevelChecked() && GetResourceCost(HolySpirit) > LocalPlayer.CurrentMp) || (!HolySpirit.LevelChecked()) || IsMoving))
                                return ShieldLob;
                        }

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

                            // (Holy) Sheltron overcap protection
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) &&
                                Sheltron.LevelChecked() &&
                                !HasEffect(Buffs.Sheltron) &&
                                !HasEffect(Buffs.HolySheltron) &&
                                Gauge.OathGauge >= Config.PLD_SheltronOption)
                                return OriginalHook(Sheltron);
                        }

                        // Requiescat inside burst (checking for FoF buff causes a late weave and can misalign over long fights with some ping)
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                            (WasLastAbility(FightOrFlight) || JustUsed(FightOrFlight, 6f)) && ActionReady(Requiescat))
                        {
                            if ((Config.PLD_ST_RequiescatWeave == 0 && CanWeave(actionID) ||
                                (Config.PLD_ST_RequiescatWeave == 1 && CanDelayedWeave(actionID, 2.0, 0.6)))) // These weave timings make no sense but they work for some reason
                                return OriginalHook(Requiescat);
                        }

                        // Actions under FoF burst
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID) && !WasLastAbility(FightOrFlight))
                            {
                                if (InMeleeRange())
                                {
                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                        ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                        ActionReady(OriginalHook(SpiritsWithin)))
                                        return OriginalHook(SpiritsWithin);
                                }

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) &&
                                    OriginalHook(Intervene).LevelChecked() &&
                                    !WasLastAction(Intervene) &&
                                    GetRemainingCharges(Intervene) > Config.PLD_Intervene_HoldCharges &&
                                    GetCooldownRemainingTime(CircleOfScorn) > 3 &&
                                    GetCooldownRemainingTime(OriginalHook(CircleOfScorn)) > 3 &&
                                    ((Config.PLD_Intervene_MeleeOnly && InMeleeRange()) || (!Config.PLD_Intervene_MeleeOnly)))
                                    return OriginalHook(Intervene);
                            }

                            // Lv90-100 Goring use
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && 
                                (WasLastSpell(BladeOfValor) && !HasEffect(Buffs.BladeOfHonor) || WasLastAbility(BladeOfHonor)))
                                return GoringBlade;

                            // Lv80-89 Goring use
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(BladeOfFaith) && !LevelChecked(BladeOfHonor) && WasLastSpell(Confiteor))
                                return GoringBlade;

                            // Lv68-79 Goring use
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(Confiteor) && WasLastSpell(HolySpirit))
                                return GoringBlade;

                            // Lv54-68 Goring use
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && !LevelChecked(HolySpirit))
                                return GoringBlade;

                            if (HasEffect(Buffs.Requiescat))
                            {
                                // Confiteor & Blades
                                if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                                    HasEffect(Buffs.ConfiteorReady))
                                    ||
                                    (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                                    BladeOfFaith.LevelChecked() &&
                                    OriginalHook(Confiteor) != Confiteor &&
                                    GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp))
                                    return OriginalHook(Confiteor);

                                // HS when Confiteor not unlocked or Confiteor used
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                    GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return HolySpirit;
                            }

                            // New Spell after Confi Combo (Weave) -- Maybe need an option for advanced mode - currently only available after blade combo.
                            if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) && CanWeave(actionID) && HasEffect(Buffs.BladeOfHonor)))
                                return OriginalHook(Requiescat);

                            // HS under DM
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                HasEffect(Buffs.DivineMight) &&
                                GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                return HolySpirit;

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                                (HasEffect(Buffs.SwordOath) || HasEffect(Buffs.SepulchreReady) || HasEffect(Buffs.SupplicationReady)))
                                return OriginalHook(Atonement);
                        }

                        // FoF (Starts burst)
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) &&
                            ActionReady(FightOrFlight) && CanWeave(actionID) &&
                            ActionWatching.CombatActions.Where(x => x == OriginalHook(RoyalAuthority)).Any()) // Check RA has been used for opener exception
                            return FightOrFlight;

                        // CoS/SW outside of burst
                        if (CanWeave(actionID, 0.6) &&
                            (!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 ||
                            IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF)) && InMeleeRange())
                        {
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                ActionReady(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }

                        // Goring on cooldown (burst features disabled) -- Goring Blade is only available with FoF
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                            HasEffect(Buffs.GoringBladeReady) &&
                            IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                            return GoringBlade;

                        // Goring on cooldown (no Spirit, Confiteor, Blades)
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) && HasEffect(Buffs.GoringBladeReady) && HasEffect(Buffs.FightOrFlight))
                        {
                            if (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit))
                                return GoringBlade;
                            else if (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor))
                                return GoringBlade;
                            else if (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades))
                                return GoringBlade;
                            else if (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) && IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades))
                                return GoringBlade;
                        }

                        //Req without FoF
                        if (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) && CanWeave(actionID)) && ActionReady(Requiescat))
                            return OriginalHook(Requiescat);

                        // New Spell after Confi Combo (Weave) -- Maybe need an option for advanced mode - currently only available after blade combo.
                        if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) && CanWeave(actionID) && HasEffect(Buffs.BladeOfHonor)) && IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF))
                            return OriginalHook(Requiescat);


                        // Confiteor & Blades
                        if (((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) &&
                            Confiteor.LevelChecked() &&
                            HasEffect(Buffs.ConfiteorReady))
                            ||
                            (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) &&
                            BladeOfFaith.LevelChecked() &&
                            HasEffect(Buffs.Requiescat) &&
                            OriginalHook(Confiteor) != Confiteor &&
                            GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)))
                            return OriginalHook(Confiteor);

                        //Req HS
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            HasEffect(Buffs.Requiescat) &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return HolySpirit;

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                            (HasEffect(Buffs.SwordOath) || HasEffect(Buffs.SepulchreReady) || HasEffect(Buffs.SupplicationReady)) && InMeleeRange())
                            return OriginalHook(Atonement);

                        // Base combo
                        if (comboTime > 0)
                        {
                            if (lastComboActionID is FastBlade &&
                                RiotBlade.LevelChecked())
                                return RiotBlade;

                            if (lastComboActionID is RiotBlade && RageOfHalone.LevelChecked())
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                                    (HasEffect(Buffs.SwordOath) || HasEffect(Buffs.SepulchreReady) || HasEffect(Buffs.SupplicationReady)) && InMeleeRange() &&
                                    (Config.PLD_ST_AtonementTiming == 2 || (Config.PLD_ST_AtonementTiming == 3 && ActionWatching.CombatActions.Count(x => x == FightOrFlight) % 2 == 0)))
                                    return OriginalHook(Atonement);

                                return (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                                    HasEffect(Buffs.DivineMight) &&
                                    GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp &&
                                    (Config.PLD_ST_DivineMightTiming == 2 || (Config.PLD_ST_DivineMightTiming == 3 && ActionWatching.CombatActions.Count(x => x == FightOrFlight) % 2 == 1)))
                                    ? HolySpirit
                                    : OriginalHook(RageOfHalone);
                            }
                        }

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                            (HasEffect(Buffs.SwordOath) || HasEffect(Buffs.SepulchreReady) || HasEffect(Buffs.SupplicationReady)) && InMeleeRange() &&
                            (Config.PLD_ST_AtonementTiming == 1 || (Config.PLD_ST_AtonementTiming == 3 && ActionWatching.CombatActions.Count(x => x == FightOrFlight) % 2 == 1)))
                            return OriginalHook(Atonement);

                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            HasEffect(Buffs.DivineMight) &&
                            GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp &&
                            (Config.PLD_ST_DivineMightTiming == 1 || (Config.PLD_ST_DivineMightTiming == 3 && ActionWatching.CombatActions.Count(x => x == FightOrFlight) % 2 == 0)))
                            return HolySpirit;
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

                    // Requiescat inside burst (checking for FoF buff causes a late weave and can misalign over long fights with some ping)
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) &&
                        (WasLastAbility(FightOrFlight) || JustUsed(FightOrFlight, 6f)) && ActionReady(Requiescat))
                    {
                        if ((Config.PLD_AoE_RequiescatWeave == 0 && CanWeave(actionID) ||
                            (Config.PLD_AoE_RequiescatWeave == 1 && CanDelayedWeave(actionID, 2.0, 0.6))))
                            return Requiescat;
                    }

                    // Actions under FoF burst
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && HasEffect(Buffs.FightOrFlight))
                    {
                        if (CanWeave(actionID) && !WasLastAbility(FightOrFlight))
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) &&
                                ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) &&
                                ActionReady(OriginalHook(SpiritsWithin)))
                                return OriginalHook(SpiritsWithin);
                        }

                        if (HasEffect(Buffs.Requiescat))
                        {
                            // Confiteor & Blades
                            if ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) &&
                                HasEffect(Buffs.ConfiteorReady))
                                ||
                                (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) &&
                                BladeOfFaith.LevelChecked() &&
                                OriginalHook(Confiteor) != Confiteor &&
                                GetResourceCost(OriginalHook(Confiteor)) <= LocalPlayer.CurrentMp))
                                return OriginalHook(Confiteor);

                            // HC when Confiteor not unlocked
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) &&
                                GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                                return HolyCircle;

                        }

                        // New Spell after Confi Combo (Weave) -- Maybe need an option for advanced mode - currently only available after blade combo.
                        if ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) && CanWeave(actionID) && HasEffect(Buffs.BladeOfHonor)))
                            return OriginalHook(Requiescat);

                        // HC under DM/Req
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) &&
                            (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)) &&
                            GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp &&
                            HolyCircle.LevelChecked())
                            return HolyCircle;
                    }

                    if (comboTime > 0 && lastComboActionID is TotalEclipse && Prominence.LevelChecked())
                        return Prominence;

                    if (CanWeave(actionID))
                    {
                        // FoF (Starts burst)
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && ActionReady(FightOrFlight))
                            return FightOrFlight;

                        // Usage outside of burst (desync for Req, 30s windows for CoS/SW)
                        if ((!WasLastAction(FightOrFlight) && GetCooldownRemainingTime(FightOrFlight) >= 15 || IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF)) &&
                            !ActionWatching.WasLast2ActionsAbilities())
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) && ActionReady(Requiescat))
                                return OriginalHook(Requiescat);

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) &&
                                ActionReady(OriginalHook(SpiritsWithin)))
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

                    // New Spell after Confi Combo (Weave) -- Maybe need an option for advanced mode - currently only available after blade combo.
                    if ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) && CanWeave(actionID) && HasEffect(Buffs.BladeOfHonor)))
                        return OriginalHook(Requiescat);

                    // HS under DM (outside of burst)
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && HasEffect(Buffs.DivineMight) &&
                        GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && LevelChecked(HolyCircle))
                        return HolyCircle;

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

                    if ((choice is 1 || choice is 3) && HasEffect(Buffs.ConfiteorReady) && Confiteor.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                        return OriginalHook(Confiteor);

                    if (HasEffect(Buffs.Requiescat))
                    {
                        if ((choice is 2 || choice is 3) && OriginalHook(Confiteor) != Confiteor && BladeOfFaith.LevelChecked() && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                            return OriginalHook(Confiteor);

                        if (choice is 4 && HolySpirit.LevelChecked() && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                            return HolySpirit;

                        if (choice is 5 && HolyCircle.LevelChecked() && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp)
                            return HolyCircle;
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
                if ((actionID == SpiritsWithin || actionID == Expiacion) && ActionReady(CircleOfScorn))
                {
                    if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                    {
                        int choice = Config.PLD_SpiritsWithinOption;

                        switch (choice)
                        {
                            case 1: return CircleOfScorn;
                            case 2: return OriginalHook(SpiritsWithin);
                        }
                    }

                    return CircleOfScorn;
                }

                return actionID;
            }
        }
    }
}
