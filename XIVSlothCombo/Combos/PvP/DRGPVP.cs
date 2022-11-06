using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

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
                DRGPVP_LOTD_Duration = "DRGPVP_LOTD_Duration",
                DRGPVP_LOTD_HPValue = "DRGPVP_LOTD_HPValue",
                DRGPVP_CS_HP_Threshold = "DRGPVP_CS_HP_Threshold";
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
                                if (HasEffect(Buffs.LifeOfTheDragon) && PlayerHealthPercentageHp() < GetOptionValue(Config.DRGPVP_LOTD_HPValue)
                                 || HasEffect(Buffs.LifeOfTheDragon) && GetBuffRemainingTime(Buffs.LifeOfTheDragon) < GetOptionValue(Config.DRGPVP_LOTD_Duration))
                                    return Nastrond;
                            }
                            if (IsEnabled(CustomComboPreset.DRGPvP_HorridRoar) && IsOffCooldown(HorridRoar) && InMeleeRange())
                                return HorridRoar;
                        }
                        if (IsOffCooldown(ChaoticSpring) && PlayerHealthPercentageHp() < GetOptionValue(Config.DRGPVP_CS_HP_Threshold))
                            return ChaoticSpring;
                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && IsOffCooldown(Geirskogul) && WasLastAbility(ElusiveJump) && HasEffect(Buffs.FirstmindsFocus))
                            return Geirskogul;
                        if (IsEnabled(CustomComboPreset.DRGPvP_WyrmwindThrust) && HasEffect(Buffs.FirstmindsFocus))
                            return WyrmwindThrust;
                    }
                }
                return actionID;
            }
        }
    }
}