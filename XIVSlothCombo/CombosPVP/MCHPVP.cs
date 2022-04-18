using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MCHPVP
    {
        public const byte JobID = 31;

        public const uint
            
            Purify = 29056,
            BlastCharge = 29402,
            HeatBlast = 29403,
            Scattergun = 29404,
            Drill = 29405,
            Bioblaster = 29406,
            AirAnchor = 29407,
            ChainSaw = 29408,
            Wildfire = 29409,
            Recuperate = 29711,
            BishopAutoturrent = 29412,
            Analysis = 29414,
            MarkmansSpite = 29415;
        
        public static class Buffs
        {
            public const ushort
                DrillPrimed = 3150,
                BioblasterPrimed = 3151,
                AirAnchorPrimed = 3152,
                ChainSawPrimed = 3153;
        }

        public static class Debuffs
        {
            public const ushort
                Wildfire = 1323,
                Stun = 1343,
                Heavy = 1344,
                Bind = 1345,
                Silence = 1347,
                Sleep = 1348,
                HalfAsleep = 3022,
                DeepFreeze = 3219;
        }
    }
    
    internal class PurifyFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PurifyFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCHPVP.BlastCharge)
            {
                var actionIDCD = GetCooldown(actionID);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                if ((HasEffectAny(MCHPVP.Debuffs.Stun) || HasEffectAny(MCHPVP.Debuffs.Heavy) || HasEffectAny(MCHPVP.Debuffs.Bind) || HasEffectAny(MCHPVP.Debuffs.Silence) || HasEffectAny(MCHPVP.Debuffs.Sleep) || HasEffectAny(MCHPVP.Debuffs.HalfAsleep) || HasEffectAny(MCHPVP.Debuffs.DeepFreeze)) && IsOffCooldown(MCHPVP.Purify))
                {
                    return MCHPVP.Purify;
                }
            }

            return actionID;
        }
    }
}
