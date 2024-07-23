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
            Imperator = 36921,
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
                AtonementReady = 1902, //First Atonement Buff
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
                PLD_ST_FoF_Option = new("PLD_ST_FoF_Option", 50),
                PLD_AoE_FoF_Option = new("PLD_AoE_FoF_Option", 50),
                PLD_Intervene_HoldCharges = new("PLDKeepInterveneCharges", 1),
                PLD_VariantCure = new("PLD_VariantCure"),
                PLD_RequiescatOption = new("PLD_RequiescatOption"),
                PLD_SpiritsWithinOption = new("PLD_SpiritsWithinOption"),
                PLD_ST_SheltronOption = new("PLD_ST_SheltronOption", 50),
                PLD_AoE_SheltronOption = new("PLD_AoE_SheltronOption", 50),
                PLD_ST_SheltronHP = new("PLD_ST_SheltronHP", 70),
                PLD_AoE_SheltronHP = new("PLD_AoE_SheltronHP", 70),
                //PLD_ST_RequiescatWeave = new("PLD_ST_RequiescatWeave"),
                //PLD_AoE_RequiescatWeave = new("PLD_AoE_RequiescatWeave"),
                //PLD_ST_AtonementTiming = new("PLD_ST_EquilibriumTiming"),
                //PLD_ST_DivineMightTiming = new("PLD_ST_DivineMightTiming"),
                PLD_Intervene_MeleeOnly = new ("PLD_Intervene_MeleeOnly", 1),
                PLD_ShieldLob_SubOption = new ("PLD_ShieldLob_SubOption", 1),
                PLD_MP_Reserve = new("PLD_MP_Reserve", 1000);
        }

        internal class PLD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_SimpleMode;
            internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FastBlade)
                {
                    #region Types
                    bool hasDivineMight = HasEffect(Buffs.DivineMight);
                    bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
                    bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
                    bool inBurstPhase = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
                    bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
                    bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 10) ||
                                                (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 10) ||
                                                (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 10);
                    #endregion

                    // Criterion Stuff
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (InMeleeRange())
                        {
                            if (CanWeave(actionID))
                            {
                                // Criterion Stuff
                                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                                if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                    (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                    return Variant.VariantSpiritDart;

                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                                    IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Requiescat Usage: After Fight or Flight
                                if (ActionReady(Requiescat) && JustUsed(FightOrFlight, 8f))
                                    return OriginalHook(Requiescat);
                            }

                            // Goring Blade Usage: No Requiescat / After Requiescat
                            if (HasEffect(Buffs.GoringBladeReady) && (!LevelChecked(Requiescat) || (IsOnCooldown(Requiescat) &&
                                !HasEffect(Buffs.Requiescat) && OriginalHook(Requiescat) != BladeOfHonor)))
                                return GoringBlade;
                        }

                        // Burst Phase
                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                // Melee oGCDs
                                if (InMeleeRange())
                                {
                                    if (ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }

                                // Blade of Honor
                                if (OriginalHook(Requiescat) == BladeOfHonor)
                                    return OriginalHook(Requiescat);
                            }

                            if (HasEffect(Buffs.Requiescat))
                            {
                                // Confiteor & Blades
                                if (GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp && (HasEffect(Buffs.ConfiteorReady) || OriginalHook(Confiteor) != Confiteor))
                                    return OriginalHook(Confiteor);

                                // Pre-Blades
                                if (GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                                    return HolySpirit;
                            }
                        }

                        if (CanWeave(actionID) && InMeleeRange())
                        {
                            // Fight or Flight
                            if (ActionReady(FightOrFlight))
                            {
                                if (!LevelChecked(Requiescat))
                                {
                                    if (!LevelChecked(RageOfHalone))
                                    {
                                        // Levels 1-25
                                        if (lastComboActionID is FastBlade)
                                            return FightOrFlight;
                                    }

                                    // Levels 26-67
                                    else if (lastComboActionID is RiotBlade)
                                        return FightOrFlight;
                                }

                                // Levels 68+
                                else if (GetCooldownRemainingTime(Requiescat) < 0.5f && CanWeave(actionID, 1.5f) && (lastComboActionID is RoyalAuthority || inBurstPhase))
                                    return FightOrFlight;
                            }

                            // Melee oGCDs
                            if (GetCooldownRemainingTime(FightOrFlight) > 15)
                            {
                                if (ActionReady(CircleOfScorn))
                                    return CircleOfScorn;

                                if (ActionReady(SpiritsWithin))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // Holy Spirit Prioritization
                        if (hasDivineMight && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp)
                        {
                            // Delay Sepulchre (Before Burst Starts) / Prefer Sepulchre (Before Burst Ends)
                            if (inAtonementFinisher && (GetCooldownRemainingTime(FightOrFlight) < 6 || GetBuffRemainingTime(Buffs.FightOrFlight) > 3))
                                return HolySpirit;

                            // Fit in Burst (When Sepulchre Unavailable)
                            if (!inAtonementFinisher && HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) < 3)
                                return HolySpirit;
                        }

                        // Atonement Usage: During Burst / Before Expiring / Before Refreshing / Spend Starter
                        if (inAtonementPhase && InMeleeRange() && (JustUsed(FightOrFlight, 30f) || isAtonementExpiring || lastComboActionID is RiotBlade || inAtonementStarter))
                            return OriginalHook(Atonement);

                        // Holy Spirit Usage: During Burst / Outside Melee / Before Expiring / Before Refreshing
                        if (hasDivineMight && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && (JustUsed(FightOrFlight, 30f) ||
                            !InMeleeRange() || GetBuffRemainingTime(Buffs.DivineMight) < 10 || lastComboActionID is RiotBlade))
                            return HolySpirit;

                        // Shield Lob Outside Melee
                        if (LevelChecked(ShieldLob) && !InMeleeRange())
                            return ShieldLob;

                        // Basic Combo
                        if (comboTime > 0)
                        {
                            if (lastComboActionID is FastBlade && LevelChecked(RiotBlade))
                                return RiotBlade;

                            if (lastComboActionID is RiotBlade && LevelChecked(RageOfHalone))
                                return OriginalHook(RageOfHalone);
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
                    // Criterion Stuff
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (InMeleeRange() && CanWeave(actionID))
                        {
                            // Criterion Stuff
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            // Requiescat Usage: After Fight or Flight
                            if (ActionReady(Requiescat) && JustUsed(FightOrFlight, 8f))
                                return OriginalHook(Requiescat);
                        }

                        // Burst Phase
                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                // Melee oGCDs
                                if (InMeleeRange())
                                {
                                    if (ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }

                                // Blade of Honor
                                if (OriginalHook(Requiescat) == BladeOfHonor)
                                    return OriginalHook(Requiescat);
                            }

                            // Confiteor & Blades
                            if (HasEffect(Buffs.Requiescat) && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp &&
                                (HasEffect(Buffs.ConfiteorReady) || OriginalHook(Confiteor) != Confiteor))
                                return OriginalHook(Confiteor);
                        }

                        // Melee oGCDs
                        if (CanWeave(actionID) && InMeleeRange())
                        {
                            // Fight or Flight
                            if (ActionReady(FightOrFlight) && ((GetCooldownRemainingTime(Requiescat) < 0.5f && CanWeave(actionID, 1.5f)) || !LevelChecked(Requiescat)))
                                return FightOrFlight;

                            if (GetCooldownRemainingTime(FightOrFlight) > 15)
                            {
                                if (ActionReady(CircleOfScorn))
                                    return CircleOfScorn;

                                if (ActionReady(SpiritsWithin))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }
                    }

                    // Holy Circle
                    if (LevelChecked(HolyCircle) && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp && (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)))
                        return HolyCircle;

                    // Basic Combo
                    if (comboTime > 0 && lastComboActionID is TotalEclipse && LevelChecked(Prominence))
                        return Prominence;
                }

                return actionID;
            }
        }

        internal class PLD_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_AdvancedMode;
            internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FastBlade)
                {
                    #region Types
                    bool hasDivineMight = HasEffect(Buffs.DivineMight);
                    bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
                    bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
                    bool inBurstPhase = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
                    bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
                    bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 10) ||
                                                (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 10) ||
                                                (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 10);
                    #endregion

                    // Criterion Stuff
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (InMeleeRange())
                        {
                            if (CanWeave(actionID))
                            {
                                // Criterion Stuff
                                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                                if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                    (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                    return Variant.VariantSpiritDart;

                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                                    IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Requiescat Usage: After Fight or Flight
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) &&
                                    ActionReady(Requiescat) && JustUsed(FightOrFlight, 8f))
                                    return OriginalHook(Requiescat);
                            }

                            // Goring Blade Usage: No Requiescat / After Requiescat
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) &&
                                HasEffect(Buffs.GoringBladeReady) && (!LevelChecked(Requiescat) || (IsOnCooldown(Requiescat) &&
                                !HasEffect(Buffs.Requiescat) && OriginalHook(Requiescat) != BladeOfHonor)))
                                return GoringBlade;
                        }

                        // Burst Phase
                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                // Melee oGCDs
                                if (InMeleeRange())
                                {
                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) &&
                                    LevelChecked(Intervene) && GetRemainingCharges(Intervene) > Config.PLD_Intervene_HoldCharges && !IsMoving && !WasLastAction(Intervene) &&
                                    ((Config.PLD_Intervene_MeleeOnly == 1 && InMeleeRange()) || (GetTargetDistance() == 0 && Config.PLD_Intervene_MeleeOnly == 2)))
                                    return Intervene;

                                // Blade of Honor
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                                    return OriginalHook(Requiescat);
                            }

                            if (HasEffect(Buffs.Requiescat))
                            {
                                // Confiteor & Blades
                                if (GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp && (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve) &&
                                    ((HasEffect(Buffs.ConfiteorReady) && IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor)) ||
                                    (OriginalHook(Confiteor) != Confiteor) && IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades)))
                                    return OriginalHook(Confiteor);

                                // Pre-Blades
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp &&
                                (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve))
                                    return HolySpirit;
                            }
                        }

                        if (CanWeave(actionID) && InMeleeRange())
                        {
                            // Fight or Flight
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && ActionReady(FightOrFlight) && GetTargetHPPercent() >= Config.PLD_ST_FoF_Option)
                            {
                                if (!LevelChecked(Requiescat))
                                {
                                    if (!LevelChecked(RageOfHalone))
                                    {
                                        // Levels 1-25
                                        if (lastComboActionID is FastBlade)
                                            return FightOrFlight;
                                    }

                                    // Levels 26-67
                                    else if (lastComboActionID is RiotBlade)
                                        return FightOrFlight;
                                }

                                // Levels 68+
                                else if (GetCooldownRemainingTime(Requiescat) < 0.5f && CanWeave(actionID, 1.5f) && (lastComboActionID is RoyalAuthority || inBurstPhase))
                                    return FightOrFlight;
                            }

                            // Melee oGCDs
                            if (GetCooldownRemainingTime(FightOrFlight) > 15)
                            {
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                    return CircleOfScorn;

                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // Sheltron Overcap Protection
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) && InCombat() && CanWeave(actionID) &&
                            LevelChecked(Sheltron) && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron) &&
                            Gauge.OathGauge >= Config.PLD_ST_SheltronOption && PlayerHealthPercentageHp() <= Config.PLD_ST_SheltronHP)
                            return OriginalHook(Sheltron);

                        // Holy Spirit Prioritization
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && hasDivineMight && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp &&
                            (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve))
                        {
                            // Delay Sepulchre (Before Burst Starts) / Prefer Sepulchre (Before Burst Ends)
                            if (inAtonementFinisher && (GetCooldownRemainingTime(FightOrFlight) < 6 || GetBuffRemainingTime(Buffs.FightOrFlight) > 3))
                                return HolySpirit;

                            // Fit in Burst (When Sepulchre Unavailable)
                            if (!inAtonementFinisher && HasEffect(Buffs.FightOrFlight) && GetBuffRemainingTime(Buffs.FightOrFlight) < 3)
                                return HolySpirit;
                        }

                        // Atonement Usage: During Burst / Before Expiring / Before Refreshing / Spend Starter
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) &&
                            inAtonementPhase && InMeleeRange() && (JustUsed(FightOrFlight, 30f) || isAtonementExpiring || lastComboActionID is RiotBlade || inAtonementStarter))
                            return OriginalHook(Atonement);

                        // Holy Spirit Usage: During Burst / Outside Melee / Before Expiring / Before Refreshing
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) &&
                            hasDivineMight && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && 
                            (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve) && (JustUsed(FightOrFlight, 30f) ||
                            !InMeleeRange() || GetBuffRemainingTime(Buffs.DivineMight) < 10 || lastComboActionID is RiotBlade))
                            return HolySpirit;

                        // Out of Range Options: Shield Lob / Holy Spirit (Not Moving)
                        if (!InMeleeRange() && IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob))
                        {
                            if (!IsMoving && LevelChecked(HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && Config.PLD_ShieldLob_SubOption == 2 &&
                                (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve))
                                return HolySpirit;

                            if (LevelChecked(ShieldLob))
                                return ShieldLob;
                        }

                        // Basic Combo
                        if (comboTime > 0)
                        {
                            if (lastComboActionID is FastBlade && LevelChecked(RiotBlade))
                                return RiotBlade;

                            if (lastComboActionID is RiotBlade && LevelChecked(RageOfHalone))
                                return OriginalHook(RageOfHalone);
                        }
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
                    // Criterion Stuff
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        if (InMeleeRange() && CanWeave(actionID))
                        {
                            // Criterion Stuff
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            // Requiescat Usage: After Fight or Flight
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) && ActionReady(Requiescat) && JustUsed(FightOrFlight, 8f))
                                return OriginalHook(Requiescat);
                        }

                        // Burst Phase
                        if (HasEffect(Buffs.FightOrFlight))
                        {
                            if (CanWeave(actionID))
                            {
                                // Melee oGCDs
                                if (InMeleeRange())
                                {
                                    if (ActionReady(CircleOfScorn) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn))
                                        return CircleOfScorn;

                                    if (ActionReady(SpiritsWithin) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }

                                // Blade of Honor
                                if (OriginalHook(Requiescat) == BladeOfHonor && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_BladeOfHonor))
                                    return OriginalHook(Requiescat);
                            }

                            // Confiteor & Blades
                            if (HasEffect(Buffs.Requiescat) && GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp &&
                                (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve) &&
                                ((HasEffect(Buffs.ConfiteorReady) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor)) ||
                                (OriginalHook(Confiteor) != Confiteor && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades))))
                                return OriginalHook(Confiteor);
                        }

                        // Melee oGCDs
                        if (CanWeave(actionID) && InMeleeRange())
                        {
                            // Fight or Flight
                            if (ActionReady(FightOrFlight) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && GetTargetHPPercent() >= Config.PLD_AoE_FoF_Option &&
                                ((GetCooldownRemainingTime(Requiescat) < 0.5f && CanWeave(actionID, 1.5f)) || !LevelChecked(Requiescat)))
                                return FightOrFlight;

                            if (GetCooldownRemainingTime(FightOrFlight) > 15)
                            {
                                if (ActionReady(CircleOfScorn) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn))
                                    return CircleOfScorn;

                                if (ActionReady(SpiritsWithin) && IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin))
                                    return OriginalHook(SpiritsWithin);
                            }
                        }

                        // Sheltron Overcap Protection
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron) && InCombat() && CanWeave(actionID) &&
                            LevelChecked(Sheltron) && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron) &&
                            Gauge.OathGauge >= Config.PLD_AoE_SheltronOption && PlayerHealthPercentageHp() <= Config.PLD_AoE_SheltronHP)
                            return OriginalHook(Sheltron);
                    }

                    // Holy Circle
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && LevelChecked(HolyCircle) && GetResourceCost(HolyCircle) <= LocalPlayer.CurrentMp &&
                        (!IsEnabled(CustomComboPreset.PLD_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_MP_Reserve) &&
                        (HasEffect(Buffs.DivineMight) || HasEffect(Buffs.Requiescat)))
                        return HolyCircle;

                    // Basic Combo
                    if (comboTime > 0 && lastComboActionID is TotalEclipse && LevelChecked(Prominence))
                        return Prominence;
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

                    if ((choice is 1 || choice is 3) && HasEffect(Buffs.ConfiteorReady) && Confiteor.LevelChecked() && 
                        GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
                        return OriginalHook(Confiteor);

                    if (HasEffect(Buffs.Requiescat))
                    {
                        if ((choice is 2 || choice is 3) && OriginalHook(Confiteor) != Confiteor && BladeOfFaith.LevelChecked() &&
                            GetResourceCost(Confiteor) <= LocalPlayer.CurrentMp)
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

        internal class PLD_FoF_Requiescat : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_FoFRequiescat;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FightOrFlight)
                {
                    if (IsOffCooldown(FightOrFlight))
                        return OriginalHook(FightOrFlight);

                    if (IsOffCooldown(Requiescat) || (LevelChecked(BladeOfHonor) && (HasEffect(Buffs.Requiescat) || HasEffect(Buffs.BladeOfHonor))))
                        return OriginalHook(Requiescat);

                    return OriginalHook(FightOrFlight);
                }

                return actionID;
            }
        }

        internal class PLD_ShieldLob_HolySpirit : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ShieldLob_Feature;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is ShieldLob)
                {
                    if (LevelChecked(HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && (!IsMoving || HasEffect(Buffs.DivineMight)))
                        return HolySpirit;
                }

                return actionID;
            }
        }
    }
}
