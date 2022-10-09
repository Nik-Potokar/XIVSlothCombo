using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class SCHPVP
    {
        public const byte JobID = 28;

        public const uint
            Broil = 29231,
            Aqloquilum = 29232,
            Biolysis = 29233,
            DeploymentTactics = 29234,
            Expedient = 29236;

        internal class Buffs
        {
            internal const ushort
                Recitation = 3094;
        }
        internal class Debuffs
        {
            internal const ushort
                Biolysis = 3089,
                Biolytic = 3090;
        }

        internal class SCHPVP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHPVP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Broil)
                {
                    if (IsEnabled(CustomComboPreset.SCHPVP_Expedient) && InCombat() && IsOffCooldown(Expedient) && !TargetHasEffect(Debuffs.Biolysis))
                        return Expedient;
                    if (IsEnabled(CustomComboPreset.SCHPVP_Biolysis) && InCombat() && IsOffCooldown(Biolysis) || HasEffect(Buffs.Recitation) && IsOffCooldown(Biolysis))
                        return Biolysis;
                    if (IsEnabled(CustomComboPreset.SCHPVP_DeploymentTactics) && InCombat() && GetRemainingCharges(DeploymentTactics) > 1)
                        return DeploymentTactics;
                }
                return actionID;
            }
        }
    }
}