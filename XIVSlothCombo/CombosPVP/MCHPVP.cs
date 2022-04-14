using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MCHPVP
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
    }
    internal class HeatedCleanShotFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCHBlastChargeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCHPVP.BlastCharge )
            {
                var canWeave = CanWeave(actionID); 
                var analysisStacks = GetRemainingCharges(MCHPVP.Analysis);
                var bigDamageStacks = GetRemainingCharges(OriginalHook(MCHPVP.Drill));

                if (IsEnabled(CustomComboPreset.PVPEmergencyHeals) && PVPCommon.GlobalEmergencyHeals.Execute(actionID)) return PVPCommon.Recuperate;

                if (canWeave && HasEffect(MCHPVP.Buffs.Overheated) && IsOffCooldown(MCHPVP.Wildfire))
                    return OriginalHook(MCHPVP.Wildfire);

                if (HasEffect(MCHPVP.Buffs.Overheated))
                    return OriginalHook(MCHPVP.HeatBlast);

                if ((HasEffect(MCHPVP.Buffs.DrillPrimed) || HasEffect(MCHPVP.Buffs.ChainSawPrimed)) && 
                    !HasEffect(MCHPVP.Buffs.Analysis) && analysisStacks > 0 && IsOnCooldown(MCHPVP.Wildfire))
                    return OriginalHook(MCHPVP.Analysis);

                if (HasEffect(MCHPVP.Buffs.Analysis) && HasEffect(MCHPVP.Buffs.DrillPrimed) && bigDamageStacks > 0)
                    return OriginalHook(MCHPVP.Drill);

                if (HasEffect(MCHPVP.Buffs.BioblasterPrimed) && bigDamageStacks > 0)
                    return OriginalHook(MCHPVP.BioBlaster);

                if (HasEffect(MCHPVP.Buffs.AirAnchorPrimed) && bigDamageStacks > 0)
                    return OriginalHook(MCHPVP.AirAnchor);

                if (HasEffect(MCHPVP.Buffs.ChainSawPrimed) && bigDamageStacks > 0)
                    return OriginalHook(MCHPVP.ChainSaw);


            }

            return actionID;
        }
    }

}
