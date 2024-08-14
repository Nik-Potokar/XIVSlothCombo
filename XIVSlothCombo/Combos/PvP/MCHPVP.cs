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
                    var hasAnalysis = GetRemainingCharges(Analysis); //How many stacks of Analysis we have
                    var hasDrill = GetRemainingCharges(OriginalHook(Drill)); //How many charges
                    var hasHeat = HasEffect(Buffs.Overheated);

                    //Wildfire
                    if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Wildfire) && 
                        canWeave && hasHeat && ActionReady(Wildfire)) 
                        return OriginalHook(Wildfire);

                    //HeatBlast
                    if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_HeatBlast) && hasHeat)
                        return OriginalHook(HeatBlast);

                    //Analysis
                    if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Analysis) && (HasEffect(Buffs.DrillPrimed) || //Drill
                        (HasEffect(Buffs.ChainSawPrimed) && !IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis)) || //Chainsaw
                        (HasEffect(Buffs.AirAnchorPrimed) && IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis))) && //Alternate AirAnchor
                        !HasEffect(Buffs.Analysis) && hasAnalysis > 0 && (!IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltDrill) //Althernate Drill
                        || !ActionReady(Wildfire)) && !canWeave && !hasHeat && hasDrill > 0)
                        return OriginalHook(Analysis);

                    //Primed skills
                    if (hasDrill > 0)
                    {
                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Drill) && HasEffect(Buffs.DrillPrimed))
                            return OriginalHook(Drill);

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_BioBlaster) && HasEffect(Buffs.BioblasterPrimed) && GetTargetDistance() <= 10)
                            return OriginalHook(BioBlaster);

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AirAnchor) && HasEffect(Buffs.AirAnchorPrimed))
                            return OriginalHook(AirAnchor);

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_ChainSaw) && HasEffect(Buffs.ChainSawPrimed))
                            return OriginalHook(ChainSaw);
                    }
                }

                return actionID;
            }
        }
    }
}