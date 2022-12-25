using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class SCHPvP
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

        internal class SCHPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Broil && InCombat())
                {
                    // Uses Expedient when available and target isn't affected with Biolysis
                    if (IsEnabled(CustomComboPreset.SCHPvP_Expedient) && IsOffCooldown(Expedient) && !TargetHasEffect(Debuffs.Biolysis))
                        return Expedient;

                    // Uses Biolysis under Recitation, or on cooldown when option active
                    if ((IsEnabled(CustomComboPreset.SCHPvP_Biolysis) && IsOffCooldown(Biolysis)) || (HasEffect(Buffs.Recitation) && IsOffCooldown(Biolysis)))
                        return Biolysis;

                    // Uses Deployment Tactics when available
                    if (IsEnabled(CustomComboPreset.SCHPvP_DeploymentTactics) && GetRemainingCharges(DeploymentTactics) > 1)
                        return DeploymentTactics;
                }

                return actionID;
            }
        }
    }
}