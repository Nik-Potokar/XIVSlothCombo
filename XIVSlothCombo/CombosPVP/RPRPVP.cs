using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class RPRPVP
    {
        public const byte JobID = 39;

        internal const uint
            Slice = 29538,
            WaxingSlice = 29539,
            InfernalSlice = 29540,
            Gibbet = 29541,
            Gallows = 29542,
            VoidReaping = 29543,
            CrossReaping = 29544,
            HarvestMoon = 29545,
            PlentifulHarvest = 29546,
            GrimSwathe = 29547,
            LemuresSlice = 29548,
            DeathWarrant = 29549,
            HellsIngress = 29550,
            Regress = 29551,
            ArcaneCrest = 29552,
            TenebraeLemurum = 29553,
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
                PlentifulHarvest = 3205,
                HellsIngress = 3207;
        }

        internal class Debuffs
        {
            internal const ushort
                DeathWarrant = 3206;
        }
        public static class Config
        {
            public const string
                RPRPvPImmortalStackThreshold = "RPRPvPImmortalStackThreshold";
            public const string
                RPRPvPArcaneCircleOption = "RPRPvPArcaneCircleOption";
        }
    }

    internal class RPRBurstMode : CustomCombo // Burst Mode
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPRBurstMode; // Burst Mode Preset Name

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPRPVP.Slice or RPRPVP.WaxingSlice or RPRPVP.InfernalSlice)
            {
                
                bool grimSwatheReady = !GetCooldown(RPRPVP.GrimSwathe).IsCooldown;
                bool lemuresSliceReady = !GetCooldown(RPRPVP.LemuresSlice).IsCooldown;
                bool arcaneReady = !GetCooldown(RPRPVP.ArcaneCrest).IsCooldown;
                var arcaneThreshold = Service.Configuration.GetCustomIntValue(RPRPVP.Config.RPRPvPArcaneCircleOption);
                bool deathWarrantReady = !GetCooldown(RPRPVP.DeathWarrant).IsCooldown;
                bool plentifulReady = !GetCooldown(RPRPVP.PlentifulHarvest).IsCooldown;
                var plentifulCD = GetCooldown(RPRPVP.PlentifulHarvest).CooldownRemaining;
                bool enshrouded = HasEffect(RPRPVP.Buffs.Enshrouded);
                var enshroudStacks = GetBuffStacks(RPRPVP.Buffs.Enshrouded);
                var immortalStacks = GetBuffStacks(RPRPVP.Buffs.ImmortalSacrifice);
                var immortalThreshold = Service.Configuration.GetCustomIntValue(RPRPVP.Config.RPRPvPImmortalStackThreshold);
                bool soulsow = HasEffect(RPRPVP.Buffs.Soulsow);
                bool canBind = !TargetHasEffect(PVPCommon.Debuffs.Bind);
                bool GCDStopped = !GetCooldown(OriginalHook(RPRPVP.Slice)).IsCooldown;
                var HP = PlayerHealthPercentageHp();
                bool canWeave = CanWeave(actionID);
                var distance = GetTargetDistance();

                // Plentiful Harvest Opener
                if (IsEnabled(CustomComboPreset.RPRPvPPlentifulOpenerOption) && !InCombat() && plentifulReady && distance <= 15)
                    return RPRPVP.PlentifulHarvest;

                // Arcane Cirle Option
                if (IsEnabled(CustomComboPreset.RPRPvPArcaneCircleOption) && arcaneReady && HP <= arcaneThreshold)
                    return RPRPVP.ArcaneCrest;

                // Harvest Moon Ranged Option
                if (IsEnabled(CustomComboPreset.RPRPvPRangedHarvestMoonOption) && distance > 5 && soulsow && GCDStopped)
                    return RPRPVP.HarvestMoon;

                // Occurring inside of Enshroud burst
                if (IsEnabled(CustomComboPreset.RPRPvPEnshroudedOption) && enshrouded)
                {
                    if (canWeave)
                    {
                        // Enshrouded Death Warrant Option
                        if (IsEnabled(CustomComboPreset.RPRPvPEnshroudedDeathWarrantOption) && deathWarrantReady && enshroudStacks >= 3 && distance <= 25)
                            return OriginalHook(RPRPVP.DeathWarrant);

                        // Lemure's Slice Option
                        if (IsEnabled(CustomComboPreset.RPRPvPEnshroudedLemuresOption) && lemuresSliceReady && canBind && distance <= 8)
                            return RPRPVP.LemuresSlice;

                        // Harvest Moon proc
                        if (soulsow && distance <= 25)
                            return OriginalHook(RPRPVP.DeathWarrant);
                    }

                    // Communio Finisher Option
                    if (IsEnabled(CustomComboPreset.RPRPvPEnshroudedCommunioOption) && enshroudStacks == 1 && distance <= 25)
                        return RPRPVP.Communio;
                }

                // Occurring outside of Enshroud burst
                if (!enshrouded)
                {
                    // Death Warrant Option
                    if (IsEnabled(CustomComboPreset.RPRPvPDeathWarrantOption) && deathWarrantReady && distance <= 25
                        && (plentifulCD > 20 && immortalStacks < immortalThreshold || plentifulReady && immortalStacks >= immortalThreshold))
                        return OriginalHook(RPRPVP.DeathWarrant);

                    // Plentiful Harvest Pooling Option
                    if (IsEnabled(CustomComboPreset.RPRPvPImmortalPoolingOption) && plentifulReady && immortalStacks >= immortalThreshold && TargetHasEffect(RPRPVP.Debuffs.DeathWarrant) && distance <= 15)
                        return RPRPVP.PlentifulHarvest;

                    // Weaves
                    if (canWeave)
                    {
                        // Harvest Moon Proc
                        if (soulsow && distance <= 25)
                            return OriginalHook(RPRPVP.DeathWarrant);

                        // Grim Swathe Option
                        if (IsEnabled(CustomComboPreset.RPRPvPGrimSwatheOption) && grimSwatheReady && distance <= 8)
                            return RPRPVP.GrimSwathe;
                    }

                    // Soul Slice
                    if (distance <= 5 && (GetRemainingCharges(RPRPVP.SoulSlice) == 2 || GetRemainingCharges(RPRPVP.SoulSlice) > 0 && !HasEffect(RPRPVP.Buffs.GallowsOiled) && !HasEffect(RPRPVP.Buffs.SoulReaver)))
                        return RPRPVP.SoulSlice;
                }
            }

            return actionID;
        }
    }
}