using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class DRGPvP
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
            Nastrond = 29492,
            Purify = 29056,
            Guard = 29054;


        public static class Buffs
        {
            public const ushort
            FirstmindsFocus = 3178,
            LifeOfTheDragon = 3177,
            Heavensent = 3176;


        }
        internal static class Config
        {
            internal const string
                DRGPvP_LOTD_Duration = "DRGPvP_LOTD_Duration",
                DRGPvP_LOTD_HPValue = "DRGPvP_LOTD_HPValue",
                DRGPvP_CS_HP_Threshold = "DRGPvP_CS_HP_Threshold",
                DRGPvP_Distance_Threshold = "DRGPvP_Distance_Threshold";
        }

        internal class DRGPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRGPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RaidenThrust or FangAndClaw or WheelingThrust)
                {
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);

                    if (!enemyGuarded)
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.DRGPvP_HighJump) && IsOffCooldown(HighJump) && HasEffect(Buffs.LifeOfTheDragon))
                                return HighJump;

                            if (IsEnabled(CustomComboPreset.DRGPvP_Nastrond) && InMeleeRange())
                            {
                                if (HasEffect(Buffs.LifeOfTheDragon) && PlayerHealthPercentageHp() < GetOptionValue(Config.DRGPvP_LOTD_HPValue)
                                 || HasEffect(Buffs.LifeOfTheDragon) && GetBuffRemainingTime(Buffs.LifeOfTheDragon) < GetOptionValue(Config.DRGPvP_LOTD_Duration))
                                    return Nastrond;
                            }
                            if (IsEnabled(CustomComboPreset.DRGPvP_HorridRoar) && IsOffCooldown(HorridRoar) && InMeleeRange())
                                return HorridRoar;
                        }
                        if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringSustain) && IsOffCooldown(ChaoticSpring) && PlayerHealthPercentageHp() < GetOptionValue(Config.DRGPvP_CS_HP_Threshold))
                        {
                            if (!HasEffect(Buffs.FirstmindsFocus) && !HasEffect(Buffs.LifeOfTheDragon) && IsOnCooldown(Geirskogul) && IsOnCooldown(ElusiveJump)
                             || !HasEffect(Buffs.FirstmindsFocus) && HasEffect(Buffs.LifeOfTheDragon) && IsOnCooldown(Geirskogul) && IsOnCooldown(ElusiveJump) && WasLastWeaponskill(HeavensThrust))
                                return ChaoticSpring;
                        }
                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && IsOffCooldown(Geirskogul) && WasLastAbility(ElusiveJump) && HasEffect(Buffs.FirstmindsFocus))
                            return Geirskogul;
                        if (IsEnabled(CustomComboPreset.DRGPvP_WyrmwindThrust) && HasEffect(Buffs.FirstmindsFocus) && GetTargetDistance() >= GetOptionValue(Config.DRGPvP_Distance_Threshold))
                            return WyrmwindThrust;
                    }
                }
                return actionID;
            }
        }
        internal class DRGPvP_BurstProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRGPvP_BurstProtection;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ElusiveJump)
                {
                    if (HasEffect(Buffs.FirstmindsFocus) || IsOnCooldown(Geirskogul))
                    {
                        return 26;
                    }
                }
                return actionID;
            }
        }
    }
}