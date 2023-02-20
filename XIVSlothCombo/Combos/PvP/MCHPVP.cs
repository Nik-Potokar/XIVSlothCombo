using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class MCHPvP
    {
        public const byte JobID = 31;

        public const uint
            BlastCharge = 29402,
            HeatBlast = 29403,
            Scattergun = 29404,
            Drill = 29405,
            BioBlaster = 29406,
            AirAnchor = 29407,
            ChainSaw = 29408,
            Wildfire = 29409,
            BishopTurret = 29412,
            AetherMortar = 29413,
            Analysis = 29414,
            MarksmanSpite = 29415;

        public static class Buffs
        {
            public const ushort
                Heat = 3148,
                Overheated = 3149,
                DrillPrimed = 3150,
                BioblasterPrimed = 3151,
                AirAnchorPrimed = 3152,
                ChainSawPrimed = 3153,
                Analysis = 3158;
        }

        public static class Debuffs
        {
            public const ushort
                Wildfire = 1323;
        }

        internal class MCHPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCHPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == BlastCharge)
                {
                    var canWeave = CanWeave(actionID);
                    var analysisStacks = GetRemainingCharges(Analysis);
                    var bigDamageStacks = GetRemainingCharges(OriginalHook(Drill));
                    var overheated = HasEffect(Buffs.Overheated);

                    if (canWeave && HasEffect(Buffs.Overheated) && IsOffCooldown(Wildfire))
                        return OriginalHook(Wildfire);

                    if (overheated)
                        return OriginalHook(HeatBlast);

                    if ((HasEffect(Buffs.DrillPrimed) ||
                        (HasEffect(Buffs.ChainSawPrimed) && !IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis)) ||
                        (HasEffect(Buffs.AirAnchorPrimed) && IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis))) &&
                        !HasEffect(Buffs.Analysis) && analysisStacks > 0 && (!IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltDrill)
                        || IsOnCooldown(Wildfire)) && !canWeave && !overheated && bigDamageStacks > 0)
                        return OriginalHook(Analysis);

                    if (bigDamageStacks > 0)
                    {
                        if (HasEffect(Buffs.DrillPrimed))
                            return OriginalHook(Drill);

                        if (HasEffect(Buffs.BioblasterPrimed) && GetTargetDistance() <= 12)
                            return OriginalHook(BioBlaster);

                        if (HasEffect(Buffs.AirAnchorPrimed))
                            return OriginalHook(AirAnchor);

                        if (HasEffect(Buffs.ChainSawPrimed))
                            return OriginalHook(ChainSaw);
                    }
                }

                return actionID;
            }
        }
    }
}