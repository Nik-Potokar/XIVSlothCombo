using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class BLMPvP
    {
        public const uint
            Fire = 29649,
            Blizzard = 29653,
            Burst = 29657,
            Paradox = 29663,
            NightWing = 29659,
            AetherialManipulation = 29660,
            Superflare = 29661,
            Fire4 = 29650,
            Flare = 29651,
            Blizzard4 = 29654,
            Freeze = 29655,
            Foul = 29371;

        public static class Buffs
        {
            public const ushort
                AstralFire2 = 3212,
                AstralFire3 = 3213,
                UmbralIce2 = 3214,
                UmbralIce3 = 3215,
                Burst = 3221,
                SoulResonance = 3222,
                Polyglot = 3169;
        }

        public static class Debuffs
        {
            public const ushort
                AstralWarmth = 3216,
                UmbralFreeze = 3217,
                Burns = 3218,
                DeepFreeze = 3219;
        }

        internal class BLMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Fire or Fire4 or Flare)
                {
                    bool canWeave = CanSpellWeave(actionID);

                    if (HasEffect(Buffs.Polyglot))
                        return Foul;

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_AetherialManip) &&
                        GetCooldown(AetherialManipulation).RemainingCharges > 0 &&
                        !InMeleeRange() && IsOffCooldown(Burst) && canWeave)
                        return AetherialManipulation;

                    if (InMeleeRange() &&
                        IsOffCooldown(Burst))
                        return Burst;

                    if (!TargetHasEffect(Debuffs.AstralWarmth))
                        return OriginalHook(Fire);

                    if (FindTargetEffect(Debuffs.AstralWarmth).StackCount < 3 &&
                        IsOffCooldown(Paradox))
                        return Paradox;

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_NightWing) &&
                        IsOffCooldown(NightWing))
                        return NightWing;

                    if (FindTargetEffect(Debuffs.AstralWarmth).StackCount == 3 &&
                        GetCooldown(Superflare).RemainingCharges > 0 &&
                        !TargetHasEffect(Debuffs.Burns))
                        return Superflare;

                }

                if (actionID is Blizzard or Blizzard4 or Freeze)
                {
                    bool canWeave = CanSpellWeave(actionID);

                    if (HasEffect(Buffs.Polyglot))
                        return Foul;

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_AetherialManip) &&
                        GetCooldown(AetherialManipulation).RemainingCharges > 0 &&
                        !InMeleeRange() &&
                        IsOffCooldown(Burst) &&
                        canWeave)
                        return AetherialManipulation;

                    if (InMeleeRange() &&
                        IsOffCooldown(Burst))
                        return Burst;

                    if (!TargetHasEffect(Debuffs.UmbralFreeze))
                        return OriginalHook(Blizzard);

                    if (FindTargetEffect(Debuffs.UmbralFreeze).StackCount < 3 &&
                        IsOffCooldown(Paradox))
                        return Paradox;

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_NightWing) &&
                        IsOffCooldown(NightWing))
                        return NightWing;

                    if (FindTargetEffect(Debuffs.UmbralFreeze).StackCount == 3 &&
                        GetCooldown(Superflare).RemainingCharges > 0 &&
                        !TargetHasEffect(Debuffs.DeepFreeze))
                        return Superflare;
                }

                return actionID;
            }
        }
    }
}
