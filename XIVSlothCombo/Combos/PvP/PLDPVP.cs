using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class PLDPVP
    {
        public const byte JobID = 19;

        public const uint
            FastBlade = 29058,
            RiotBlade = 29059,
            RoyalAuthority = 29060,
            ShieldBash = 29064,
            Confiteor = 29070;

        internal class Debuffs
        {
            internal const ushort
                Stun = 1343;
        }

        internal class PLDPVP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLDPVP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FastBlade or RiotBlade or RoyalAuthority)
                {
                    if (IsEnabled(CustomComboPreset.PLDPVP_ShieldBash) && InCombat() && IsOffCooldown(ShieldBash) && CanWeave(actionID))
                        return ShieldBash;
                        
                    if (IsEnabled(CustomComboPreset.PLDPVP_Confiteor))
                    {
                       if(TargetHasEffect(Debuffs.Stun) && IsOffCooldown(Confiteor) || IsOffCooldown(Confiteor))
                        return Confiteor;
                    }
                }
                return actionID;
            }
        }
    }
}