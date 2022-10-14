using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class DRGPVP
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            WheelingThrustCombo = 56,
            RaidenThrust = 29486,
            FangAndClaw = 29487,
            WheelingThrust = 29488,
            ChaoticSpring = 29490,
            Geirskogul = 29491,
            HighJump = 29493,
            ElusiveJump = 29494,
            WyrmwindThrust = 29495,
            HorridRoar = 29496,
            HeavensThrust = 29489,
            Nastrond = 29492;


        public static class Buffs
        {
            public const ushort
                Heavensent = 3176,
                LifeOfTheDragon = 3177,
                FirstmindsFocus = 3178;
                
        }

        internal class DRGPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRGPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RaidenThrust or FangAndClaw or WheelingThrust)
                {
                    var jobMaxHp = LocalPlayer.MaxHp;
                    var maxHPThreshold = jobMaxHp - 12000;

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.DRGPvP_HighJump) && IsOffCooldown(HighJump) && InMeleeRange() || IsEnabled(CustomComboPreset.DRGPvP_HighJump_WyrmwindThrust) && WasLastWeaponskill(WyrmwindThrust) && !InMeleeRange() && IsOffCooldown(HighJump) && CanWeave(actionID))
                            return HighJump;
                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && IsOffCooldown(Geirskogul))
                            return Geirskogul;
                        if (IsEnabled(CustomComboPreset.DRGPvP_Nastrond) && HasEffect(Buffs.LifeOfTheDragon))
                            return Nastrond;
                        if(IsEnabled(CustomComboPreset.DRGPvP_HorridRoar) && IsOffCooldown(HorridRoar) && InMeleeRange())
                            return HorridRoar;
                    }
                    
                    if (IsEnabled(CustomComboPreset.DRGPvP_WyrmwindThrust) && !InMeleeRange() && HasEffect(Buffs.FirstmindsFocus))
                        return WyrmwindThrust;
                    // Sustain CS
                    if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringSustain) && IsOffCooldown(ChaoticSpring) && jobMaxHp <= maxHPThreshold)
                        return ChaoticSpring;
                    // Burst CS
                    if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringBurst) && IsOffCooldown(ChaoticSpring) && InCombat())
                        return ChaoticSpring;
                }

                return actionID;
            }
        }
    }
}