using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class WARPVP
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        internal const uint
            HeavySwing = 29074,
            Maim = 29075,
            StormsPath = 29076,
            PrimalRend = 29084,
            Onslaught = 29079,
            Orogeny = 29080,
            Blota = 29081,
            Bloodwhetting = 29082;
            
        internal class Buffs
        {
            internal const ushort
                NascentChaos = 1992,
                InnerRelease = 1303;
        }
    }

    internal class WARBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WARBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is WARPVP.HeavySwing or WARPVP.Maim or WARPVP.StormsPath)
            {
                var canWeave = CanWeave(actionID);

                if (!GetCooldown(WARPVP.Bloodwhetting).IsCooldown && (IsEnabled(CustomComboPreset.WARBurstOption) || canWeave))
                    return OriginalHook(WARPVP.Bloodwhetting);
                    
                if (!GetCooldown(WARPVP.PrimalRend).IsCooldown)
                    return OriginalHook(WARPVP.PrimalRend);
                
                if (!InMeleeRange() && !GetCooldown(WARPVP.Blota).IsCooldown && !TargetHasEffectAny(PVPCommon.Debuffs.Stun) && 
                    (IsNotEnabled(CustomComboPreset.WARBurstBlotaOption) || GetCooldown(WARPVP.PrimalRend).CooldownRemaining >= 5) )
                    return OriginalHook(WARPVP.Blota);

                if (!GetCooldown(WARPVP.Onslaught).IsCooldown && canWeave)
                    return OriginalHook(WARPVP.Onslaught);

                if (InMeleeRange())
                {
                    if (HasEffect(WARPVP.Buffs.NascentChaos))
                        return OriginalHook(WARPVP.Bloodwhetting);

                    if (!GetCooldown(WARPVP.Orogeny).IsCooldown && canWeave)
                        return OriginalHook(WARPVP.Orogeny);
                }
            }
            return actionID;
        }
    }
}