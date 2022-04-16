using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class RPR
    {
        public const byte JobID = 39; // Job & skill IDs

        public const uint
            // Single Target
            Slice = 24373,
            WaxingSlice = 24374,
            InfernalSlice = 24375,
            ShadowOfDeath = 24378,
            SoulSlice = 24380,
            // AoE
            SpinningScythe = 24376,
            NightmareScythe = 24377,
            WhorlOfDeath = 24379,
            SoulScythe = 24381,
            // Unveiled
            Gibbet = 24382,
            Gallows = 24383,
            UnveiledGibbet = 24390,
            UnveiledGallows = 24391,
            Guillotine = 24384,
            // Reaver
            BloodStalk = 24389,
            GrimSwathe = 24392,
            Gluttony = 24393,
            // Immortal Sacrifice
            ArcaneCircle = 24405,
            PlentifulHarvest = 24385,
            // Enshroud (Burst)
            Enshroud = 24394,
            Communio = 24398,
            LemuresSlice = 24399,
            LemuresScythe = 24400,
            VoidReaping = 24395,
            CrossReaping = 24396,
            GrimReaping = 24397,
            // Miscellaneous
            Harpe = 24386,
            Soulsow = 24387,
            HarvestMoon = 24388,
            HellsIngress = 24401,
            HellsEgress = 24402,
            Regress = 24403,
            // Role
            SecondWind = 7541,
            Bloodbath = 7542,
            LegSweep = 7863,
            Feint = 7549;


        public static class Buffs // Buff IDs
        {
            public const ushort
                SoulReaver = 2587,
                ImmortalSacrifice = 2592,
                EnhancedGibbet = 2588,
                EnhancedGallows = 2589,
                EnhancedVoidReaping = 2590,
                EnhancedCrossReaping = 2591,
                EnhancedHarpe = 2859,
                Enshrouded = 2593,
                Soulsow = 2594,
                Threshold = 2595;
        }

        public static class Debuffs // Debuff IDs
        {
            public const ushort
                DeathsDesign = 2586;
        }

        public static class Levels // Level check values
        {
            public const byte
                WaxingSlice = 5,
                SecondWind = 8,
                LegSweep = 10,
                ShadowOfDeath = 10,
                Bloodbath = 12,
                HellsIngress = 20,
                HellsEgress = 20,
                Feint = 22,
                SpinningScythe = 25,
                InfernalSlice = 30,
                WhorlOfDeath = 35,
                NightmareScythe = 45,
                SoulSlice = 60,
                SoulScythe = 65,
                SoulReaver = 70,
                Regress = 74,
                Gluttony = 76,
                Enshroud = 80,
                Soulsow = 82,
                HarvestMoon = 82,
                PlentifulHarvest = 88,
                Communio = 90;

        }

        public static class Config // Config
        {
            public const string
                RPRSoulSliceCharges = "RPRSoulSliceCharges", // Single Target Charges
                RPRSoulScytheCharges = "RPRSoulScytheCharges", // AoE Charges
                RPRSoulGaugeThreshold = "RPRSoulGaugeThreshold"; // Soul Gauge Overcap Threshold
        }
    }
    internal class ReaperSliceCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSliceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RPR.Slice)
            {
                var gauge = GetJobGauge<RPRGauge>();
                var enshrouded = HasEffect(RPR.Buffs.Enshrouded);
                var soulReaver = HasEffect(RPR.Buffs.SoulReaver);
                var soulSliceCooldown = GetCooldown(RPR.SoulSlice);
                var soulSliceCharges = Service.Configuration.GetCustomIntValue(RPR.Config.RPRSoulSliceCharges);
                var soulGaugeThreshold = Service.Configuration.GetCustomIntValue(RPR.Config.RPRSoulGaugeThreshold);

                // Ranged actions
                // Ranged Filler Option
                if (IsEnabled(CustomComboPreset.ReaperRangedFillerOption) && !InMeleeRange(true))
                {
                    // Communio Override
                    if (enshrouded && gauge.LemureShroud is 1 && gauge.VoidShroud is 0 && level >= RPR.Levels.Communio)
                        return RPR.Communio;

                    if (IsEnabled(CustomComboPreset.) && level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow))
                    {
                        if ((IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedOption) && HasEffect(RPR.Buffs.EnhancedHarpe)) || (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatOption) && !HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat)))
                            return RPR.Harpe;

                        return RPR.HarvestMoon;
                    }

                    return RPR.Harpe;
                }

                // Melee-range actions
                // Stun Option
                if (IsEnabled(CustomComboPreset.ReaperStunOption))
                {
                    if (level >= RPR.Levels.LegSweep && CanInterruptEnemy() && IsOffCooldown(RPR.LegSweep))
                        return RPR.LegSweep;
                }

                // Soul Slice Overcap Option
                if (IsEnabled(CustomComboPreset.ReaperSoulSliceOvercapOption) && level >= RPR.Levels.SoulSlice) // Define this in ccp.cs
                {
                    if (!enshrouded && !soulReaver && GetRemainingCharges(RPR.SoulSlice) > soulSliceCharges && gauge.Soul <= soulGaugeThreshold)
                        return RPR.SoulSlice;
                }

                // Base 1-2-3 Combo
                if (comboTime > 0)
                {
                    if (level >= RPR.Levels.WaxingSlice && lastComboMove is RPR.Slice)
                        return RPR.WaxingSlice;

                    if (level >= RPR.Levels.InfernalSlice && lastComboMove is RPR.WaxingSlice)
                        return RPR.InfernalSlice;
                }

                return RPR.Slice;
            }

            return actionID;
        }
    }
}