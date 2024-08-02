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
                AtonementReady = 1902, // First Atonement Buff
                SupplicationReady = 3827, // Second Atonement Buff
                SepulchreReady = 3828, // Third Atonement Buff
                GoringBladeReady = 3847,
                BladeOfHonor = 3831,
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
                PLD_ST_FoF_Trigger = new("PLD_ST_FoF_Trigger", 0),
                PLD_AoE_FoF_Trigger = new("PLD_AoE_FoF_Trigger", 0),
                PLD_Intervene_HoldCharges = new("PLDKeepInterveneCharges", 1),
                PLD_VariantCure = new("PLD_VariantCure"),
                PLD_RequiescatOption = new("PLD_RequiescatOption"),
                PLD_SpiritsWithinOption = new("PLD_SpiritsWithinOption"),
                PLD_ST_SheltronOption = new("PLD_ST_SheltronOption", 50),
                PLD_AoE_SheltronOption = new("PLD_AoE_SheltronOption", 50),
                PLD_Intervene_MeleeOnly = new("PLD_Intervene_MeleeOnly", 1),
                PLD_ShieldLob_SubOption = new("PLD_ShieldLob_SubOption", 1),
                PLD_ST_MP_Reserve = new("PLD_ST_MP_Reserve", 1000),
                PLD_AoE_MP_Reserve = new("PLD_AoE_MP_Reserve", 1000);
        }

        internal class PLD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_SimpleMode;
            internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                #region Variables
                bool canWeave = CanWeave(actionID);
                bool canEarlyWeave = CanWeave(actionID, 1.5f);
                bool hasRequiescat = HasEffect(Buffs.Requiescat);
                bool hasDivineMight = HasEffect(Buffs.DivineMight);
                bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
                bool hasDivineMagicMP = GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp;
                bool inBurstWindow = JustUsed(FightOrFlight, 30f);
                bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
                bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
                bool afterOpener = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
                bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
                bool isDivineMightExpiring = GetBuffRemainingTime(Buffs.DivineMight) < 10;
                bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 10) ||
                                            (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 10) ||
                                            (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 10);
                #endregion

                if (actionID is FastBlade)
                {
                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        // Variant DoT Check
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        // Weavables
                        if (canWeave)
                        {
                            if (InMeleeRange())
                            {
                                // Requiescat
                                if (ActionReady(Requiescat) && JustUsed(FightOrFlight, 10f))
                                    return OriginalHook(Requiescat);

                                // Fight or Flight
                                if (ActionReady(FightOrFlight))
                                {
                                    if (!LevelChecked(Requiescat))
                                    {
                                        if (!LevelChecked(RageOfHalone))
                                        {
                                            // Level 2-25
                                            if (lastComboActionID is FastBlade)
                                                return FightOrFlight;
                                        }

                                        // Level 26-67
                                        else if (lastComboActionID is RiotBlade)
                                            return FightOrFlight;
                                    }

                                    // Level 68+
                                    else if (GetCooldownRemainingTime(Requiescat) < 0.5f && canEarlyWeave && (lastComboActionID is RoyalAuthority || afterOpener))
                                        return FightOrFlight;
                                }

                                // Variant Ultimatum
                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                                    IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Circle of Scorn / Spirits Within
                                if (hasFightOrFlight || GetCooldownRemainingTime(FightOrFlight) > 15)
                                {
                                    if (ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }
                            }

                            // Variant Spirit Dart
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            // Blade of Honor
                            if (LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                                return OriginalHook(Requiescat);
                        }

                        // Requiescat Phase
                        if (hasRequiescat && hasDivineMagicMP)
                        {
                            // Confiteor & Blades
                            if (HasEffect(Buffs.ConfiteorReady) || (LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                                return OriginalHook(Confiteor);

                            // Pre-Blades
                            return HolySpirit;
                        }

                        // Goring Blade
                        if (HasEffect(Buffs.GoringBladeReady) && InMeleeRange() && (!LevelChecked(Requiescat) || IsOnCooldown(Requiescat)))
                            return GoringBlade;

                        // Holy Spirit Prioritization
                        if (hasDivineMight && hasDivineMagicMP)
                        {
                            // Delay Sepulchre / Prefer Sepulchre 
                            if (inAtonementFinisher && (GetCooldownRemainingTime(FightOrFlight) < 6 || GetBuffRemainingTime(Buffs.FightOrFlight) > 3))
                                return HolySpirit;

                            // Fit in Burst
                            if (!inAtonementFinisher && hasFightOrFlight && GetBuffRemainingTime(Buffs.FightOrFlight) < 3)
                                return HolySpirit;
                        }

                        // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                        if (inAtonementPhase && InMeleeRange() && (inBurstWindow || isAtonementExpiring || inAtonementStarter || lastComboActionID is RiotBlade))
                            return OriginalHook(Atonement);

                        // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                        if (hasDivineMight && hasDivineMagicMP && (inBurstWindow || isDivineMightExpiring || !InMeleeRange() || lastComboActionID is RiotBlade))
                            return HolySpirit;

                        // Out of Range
                        if (!InMeleeRange())
                        {
                            // Holy Spirit (Not Moving)
                            if (LevelChecked(HolySpirit) && hasDivineMagicMP && !IsMoving)
                                return HolySpirit;

                            // Shield Lob
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

        internal class PLD_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                #region Variables
                bool canWeave = CanWeave(actionID);
                bool canEarlyWeave = CanWeave(actionID, 1.5f);
                bool hasRequiescat = HasEffect(Buffs.Requiescat);
                bool hasDivineMight = HasEffect(Buffs.DivineMight);
                bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
                bool hasDivineMagicMP = GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp;
                #endregion

                if (actionID is TotalEclipse)
                {
                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        // Variant DoT Check
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        // Weavables
                        if (canWeave)
                        {
                            if (InMeleeRange())
                            {
                                // Requiescat
                                if (ActionReady(Requiescat) && JustUsed(FightOrFlight, 10f))
                                    return OriginalHook(Requiescat);

                                // Fight or Flight
                                if (ActionReady(FightOrFlight) && ((GetCooldownRemainingTime(Requiescat) < 0.5f && canEarlyWeave) || !LevelChecked(Requiescat)))
                                    return FightOrFlight;

                                // Variant Ultimatum
                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Circle of Scorn / Spirits Within
                                if (hasFightOrFlight || GetCooldownRemainingTime(FightOrFlight) > 15)
                                {
                                    if (ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }
                            }

                            // Variant Spirit Dart
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) && (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            // Blade of Honor
                            if (LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                                return OriginalHook(Requiescat);
                        }

                        // Confiteor & Blades
                        if (hasRequiescat && hasDivineMagicMP && (HasEffect(Buffs.ConfiteorReady) || (LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)))
                            return OriginalHook(Confiteor);
                    }

                    // Holy Circle
                    if (LevelChecked(HolyCircle) && hasDivineMagicMP && (hasDivineMight || hasRequiescat))
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
                #region Variables
                bool canWeave = CanWeave(actionID);
                bool canEarlyWeave = CanWeave(actionID, 1.5f);
                bool hasRequiescat = HasEffect(Buffs.Requiescat);
                bool hasDivineMight = HasEffect(Buffs.DivineMight);
                bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
                bool hasDivineMagicMP = GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp;
                bool inBurstWindow = JustUsed(FightOrFlight, 30f);
                bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
                bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
                bool afterOpener = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
                bool isAboveMPReserve = !IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_ST_MP_Reserve;
                bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
                bool isDivineMightExpiring = GetBuffRemainingTime(Buffs.DivineMight) < 10;
                bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 10) ||
                                            (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 10) ||
                                            (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 10);
                #endregion

                if (actionID is FastBlade)
                {
                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        // Variant DoT Check
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        // Weavables
                        if (canWeave)
                        {
                            if (InMeleeRange())
                            {
                                // Requiescat
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) && ActionReady(Requiescat) && JustUsed(FightOrFlight, 10f))
                                    return OriginalHook(Requiescat);

                                // Fight or Flight
                                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && ActionReady(FightOrFlight) && GetTargetHPPercent() >= Config.PLD_ST_FoF_Trigger)
                                {
                                    if (!LevelChecked(Requiescat))
                                    {
                                        if (!LevelChecked(RageOfHalone))
                                        {
                                            // Level 2-25
                                            if (lastComboActionID is FastBlade)
                                                return FightOrFlight;
                                        }

                                        // Level 26-67
                                        else if (lastComboActionID is RiotBlade)
                                            return FightOrFlight;
                                    }

                                    // Level 68+
                                    else if (GetCooldownRemainingTime(Requiescat) < 0.5f && canEarlyWeave && (lastComboActionID is RoyalAuthority || afterOpener))
                                        return FightOrFlight;
                                }

                                // Variant Ultimatum
                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Circle of Scorn / Spirits Within
                                if (hasFightOrFlight || GetCooldownRemainingTime(FightOrFlight) > 15)
                                {
                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }
                            }

                            // Variant Spirit Dart
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            // Intervene
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) && LevelChecked(Intervene) && hasFightOrFlight &&
                                GetRemainingCharges(Intervene) > Config.PLD_Intervene_HoldCharges && !IsMoving && !WasLastAction(Intervene) &&
                                ((Config.PLD_Intervene_MeleeOnly == 1 && InMeleeRange()) || (GetTargetDistance() == 0 && Config.PLD_Intervene_MeleeOnly == 2)))
                                return Intervene;

                            // Blade of Honor
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_BladeOfHonor) && LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                                return OriginalHook(Requiescat);

                            // Sheltron Overcap Protection
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) && LevelChecked(Sheltron) &&
                                Gauge.OathGauge >= Config.PLD_ST_SheltronOption && PlayerHealthPercentageHp() < 100 &&
                                InCombat() && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron))
                                return OriginalHook(Sheltron);
                        }

                        // Requiescat Phase
                        if (hasRequiescat && hasDivineMagicMP && isAboveMPReserve)
                        {
                            // Confiteor & Blades
                            if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) && HasEffect(Buffs.ConfiteorReady)) ||
                                (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                                return OriginalHook(Confiteor);

                            // Pre-Blades
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit))
                                return HolySpirit;
                        }

                        // Goring Blade
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) && HasEffect(Buffs.GoringBladeReady) &&
                            InMeleeRange() && (!LevelChecked(Requiescat) || IsOnCooldown(Requiescat)))
                            return GoringBlade;

                        // Holy Spirit Prioritization
                        if (hasDivineMight && hasDivineMagicMP && isAboveMPReserve)
                        {
                            // Delay Sepulchre / Prefer Sepulchre 
                            if (inAtonementFinisher && (GetCooldownRemainingTime(FightOrFlight) < 6 || GetBuffRemainingTime(Buffs.FightOrFlight) > 3))
                                return HolySpirit;

                            // Fit in Burst
                            if (!inAtonementFinisher && hasFightOrFlight && GetBuffRemainingTime(Buffs.FightOrFlight) < 3)
                                return HolySpirit;
                        }

                        // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) && inAtonementPhase && InMeleeRange() &&
                            (inBurstWindow || isAtonementExpiring || inAtonementStarter || lastComboActionID is RiotBlade))
                            return OriginalHook(Atonement);

                        // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && hasDivineMight && hasDivineMagicMP && isAboveMPReserve &&
                            (inBurstWindow || isDivineMightExpiring || !InMeleeRange() || lastComboActionID is RiotBlade))
                            return HolySpirit;

                        // Out of Range
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob) && !InMeleeRange())
                        {
                            // Holy Spirit (Not Moving)
                            if (LevelChecked(HolySpirit) && hasDivineMagicMP && isAboveMPReserve && !IsMoving && Config.PLD_ShieldLob_SubOption == 2)
                                return HolySpirit;

                            // Shield Lob
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
                #region Variables
                bool canWeave = CanWeave(actionID);
                bool canEarlyWeave = CanWeave(actionID, 1.5f);
                bool hasRequiescat = HasEffect(Buffs.Requiescat);
                bool hasDivineMight = HasEffect(Buffs.DivineMight);
                bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
                bool hasDivineMagicMP = GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp;
                bool isAboveMPReserve = !IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve) || LocalPlayer.CurrentMp >= Config.PLD_AoE_MP_Reserve;
                #endregion

                if (actionID is TotalEclipse)
                {
                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                        return Variant.VariantCure;

                    if (HasBattleTarget())
                    {
                        // Variant DoT Check
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                        // Weavables
                        if (canWeave)
                        {
                            if (InMeleeRange())
                            {
                                // Requiescat
                                if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) && ActionReady(Requiescat) && JustUsed(FightOrFlight, 10f))
                                    return OriginalHook(Requiescat);

                                // Fight or Flight
                                if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && ActionReady(FightOrFlight) && GetTargetHPPercent() >= Config.PLD_AoE_FoF_Trigger &&
                                    ((GetCooldownRemainingTime(Requiescat) < 0.5f && canEarlyWeave) || !LevelChecked(Requiescat)))
                                    return FightOrFlight;

                                // Variant Ultimatum
                                if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                    return Variant.VariantUltimatum;

                                // Circle of Scorn / Spirits Within
                                if (hasFightOrFlight || GetCooldownRemainingTime(FightOrFlight) > 15)
                                {
                                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                        return CircleOfScorn;

                                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                        return OriginalHook(SpiritsWithin);
                                }
                            }

                            // Variant Spirit Dart
                            if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            // Blade of Honor
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_BladeOfHonor) && LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                                return OriginalHook(Requiescat);

                            // Sheltron Overcap Protection
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron) && LevelChecked(Sheltron) &&
                                Gauge.OathGauge >= Config.PLD_AoE_SheltronOption && PlayerHealthPercentageHp() < 100 &&
                                InCombat() && !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron))
                                return OriginalHook(Sheltron);
                        }

                        // Confiteor & Blades
                        if (hasRequiescat && hasDivineMagicMP && isAboveMPReserve &&
                            ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) && HasEffect(Buffs.ConfiteorReady)) ||
                            (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)))
                            return OriginalHook(Confiteor);
                    }

                    // Holy Circle
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && LevelChecked(HolyCircle) &&
                        hasDivineMagicMP && isAboveMPReserve && (hasDivineMight || hasRequiescat))
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
