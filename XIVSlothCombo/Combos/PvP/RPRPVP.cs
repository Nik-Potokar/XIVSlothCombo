using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class RPRPvP
    {
        public const byte JobID = 39;

        internal const uint
            Slice = 29538,
            WaxingSlice = 29539,
            InfernalSlice = 29540,
            HarvestMoon = 29545,
            PlentifulHarvest = 29546,
            GrimSwathe = 29547,
            LemuresSlice = 29548,
            DeathWarrant = 29549,
            ArcaneCrest = 29552,
            HellsIngress = 29550,
            Regress = 29551,
            Communio = 29554,
            SoulSlice = 29566;

        internal class Buffs
        {
            internal const ushort
                Soulsow = 2750,
                SoulReaver = 2854,
                GallowsOiled = 2856,
                Enshrouded = 2863,
                ImmortalSacrifice = 3204,
                PlentifulHarvest = 3205;
        }

        internal class Debuffs
        {
            internal const ushort
                DeathWarrant = 3206;
        }

        public static class Config
        {
            public const string
                RPRPvP_ImmortalStackThreshold = "RPRPvPImmortalStackThreshold";
            public const string
                RPRPvP_ArcaneCircleThreshold = "RPRPvPArcaneCircleOption";
        }

        internal class RPRPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPRPvP_Burst;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Slice or WaxingSlice or InfernalSlice)
                {
                    #region types
                    bool canWeave = CanWeave(actionID);
                    bool GCDStopped = !GetCooldown(OriginalHook(Slice)).IsCooldown;
                    double distance = GetTargetDistance();
                    double playerHP = PlayerHealthPercentageHp();
                    bool canBind = !TargetHasEffect(PvPCommon.Debuffs.Bind);
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                    bool grimSwatheReady = IsOffCooldown(GrimSwathe);
                    bool lemuresSliceReady = IsOffCooldown(LemuresSlice);
                    bool arcaneReady = IsOffCooldown(ArcaneCrest);
                    int arcaneThreshold = PluginConfiguration.GetCustomIntValue(Config.RPRPvP_ArcaneCircleThreshold);
                    bool deathWarrantReady = IsOffCooldown(DeathWarrant);
                    bool plentifulReady = IsOffCooldown(PlentifulHarvest);
                    float plentifulCD = GetCooldownRemainingTime(PlentifulHarvest);
                    bool enshrouded = HasEffect(Buffs.Enshrouded);
                    float enshroudStacks = GetBuffStacks(Buffs.Enshrouded);
                    float immortalStacks = GetBuffStacks(Buffs.ImmortalSacrifice);
                    int immortalThreshold = PluginConfiguration.GetCustomIntValue(Config.RPRPvP_ImmortalStackThreshold);
                    bool soulsow = HasEffect(Buffs.Soulsow);
                    #endregion

                    // Arcane Cirle Option
                    if (IsEnabled(CustomComboPreset.RPRPvP_Burst_ArcaneCircle)
                        && arcaneReady && playerHP <= arcaneThreshold)
                        return ArcaneCrest;

                    if (!enemyGuarded) // Guard check on target
                    {
                        // Plentiful Harvest Opener
                        if (IsEnabled(CustomComboPreset.RPRPvP_Burst_PlentifulOpener) &&
                            !InCombat() && plentifulReady && distance <= 15)
                            return PlentifulHarvest;

                        // Harvest Moon Ranged Option
                        if (IsEnabled(CustomComboPreset.RPRPvP_Burst_RangedHarvest) &&
                            distance > 5 && soulsow && GCDStopped)
                            return HarvestMoon;

                        // Enshroud
                        if (IsEnabled(CustomComboPreset.RPRPvP_Burst_Enshrouded) && enshrouded)
                        {
                            if (canWeave)
                            {
                                // Enshrouded Death Warrant Option
                                if (IsEnabled(CustomComboPreset.RPRPvP_Burst_Enshrouded_DeathWarrant) &&
                                    deathWarrantReady && enshroudStacks >= 3 && distance <= 25)
                                    return OriginalHook(DeathWarrant);

                                // Lemure's Slice
                                if (lemuresSliceReady && canBind && distance <= 8)
                                    return LemuresSlice;

                                // Harvest Moon proc
                                if (soulsow && distance <= 25)
                                    return OriginalHook(DeathWarrant);
                            }

                            // Communio Option
                            if (IsEnabled(CustomComboPreset.RPRPvP_Burst_Enshrouded_Communio) &&
                                enshroudStacks == 1 && distance <= 25)
                            {
                                // Holds Communio when moving & Enshrouded Time Remaining > 2s
                                // Returns a Void/Cross Reaping if under 2s to avoid charge waste
                                if (IsMoving && GetBuffRemainingTime(Buffs.Enshrouded) > 2)
                                    return BLM.Xenoglossy;

                                // Returns Communio if stationary
                                if (!IsMoving)
                                    return Communio;
                            }
                        }

                        // Outside of Enshroud
                        if (!enshrouded)
                        {
                            // Death Warrant Option
                            if (IsEnabled(CustomComboPreset.RPRPvP_Burst_DeathWarrant) &&
                                deathWarrantReady && distance <= 25 &&
                                ((plentifulCD > 20 && immortalStacks < immortalThreshold) ||
                                (plentifulReady && immortalStacks >= immortalThreshold)))
                                return OriginalHook(DeathWarrant);

                            // Plentiful Harvest Pooling Option
                            if (IsEnabled(CustomComboPreset.RPRPvP_Burst_ImmortalPooling) &&
                                plentifulReady && immortalStacks >= immortalThreshold &&
                                TargetHasEffect(Debuffs.DeathWarrant) && distance <= 15)
                                return PlentifulHarvest;

                            // Weaves
                            if (canWeave)
                            {
                                // Harvest Moon Proc
                                if (soulsow && distance <= 25)
                                    return OriginalHook(DeathWarrant);

                                // Grim Swathe Option
                                if (grimSwatheReady && distance <= 8)
                                    return GrimSwathe;
                            }
                        }
                    }

                    // Soul Slice
                    if (!enshrouded && distance <= 5 &&
                        (GetRemainingCharges(SoulSlice) == 2 ||
                        (GetRemainingCharges(SoulSlice) > 0 && !HasEffect(Buffs.GallowsOiled) && !HasEffect(Buffs.SoulReaver))))
                        return SoulSlice;
                }

                return actionID;
            }
        }
    }
}