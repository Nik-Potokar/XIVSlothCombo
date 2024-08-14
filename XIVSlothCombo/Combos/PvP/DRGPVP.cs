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
                    bool closeEnough = GetTargetDistance() <= 7;
                    bool farEnough = GetTargetDistance() <= 13;
                    float enemyHP = GetTargetHPPercent();
                    float playerHP = PlayerHealthPercentageHp();
                    float GCD = GetCooldown(WheelingThrust).CooldownTotal;

                    if (!enemyGuarded)
                    {
                        if (CanWeave(actionID))
                        {
                            //ElusiveJump
                            if (IsEnabled(CustomComboPreset.DRGPvP_ElusiveJump) && ActionReady(ElusiveJump) && GetCooldownRemainingTime(HighJump) < 1.5f) //use on CD to keep rotation going
                                return ElusiveJump;

                            //HighJump
                            if (IsEnabled(CustomComboPreset.DRGPvP_HighJump) && ActionReady(HighJump) && (JustUsed(WyrmwindThrust) || enemyHP <= 10)) //use to re-engage after ElusiveJump > far WyrmwindThrust, best held for burst or to execute
                                return HighJump;

                            //Nastrond
                            if (IsEnabled(CustomComboPreset.DRGPvP_Nastrond) && farEnough)
                            {
                                if (HasEffect(Buffs.LifeOfTheDragon) && 
                                    ((IsEnabled(CustomComboPreset.DRGPvP_NastrondOpti) && (enemyHP <= 50 || GetBuffRemainingTime(Buffs.LifeOfTheDragon) <= GCD)) || //best used when TargetHP is lower than 50
                                    (IsNotEnabled(CustomComboPreset.DRGPvP_NastrondOpti) && playerHP < GetOptionValue(Config.DRGPvP_LOTD_HPValue)) || //PlayerHP slider
                                    (IsNotEnabled(CustomComboPreset.DRGPvP_NastrondOpti) && GetBuffRemainingTime(Buffs.LifeOfTheDragon) < GetOptionValue(Config.DRGPvP_LOTD_Duration)))) //Buff duration slider
                                    return Nastrond;
                            }
                            if (IsEnabled(CustomComboPreset.DRGPvP_HorridRoar) && ActionReady(HorridRoar) && HasEffect(Buffs.LifeOfTheDragon) && closeEnough) //best used when under LOTD to prevent dying due to squishiness
                                return HorridRoar;
                        }
                        if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpring) && InMeleeRange() && ActionReady(ChaoticSpring))
                        {
                            if ((!HasEffect(Buffs.FirstmindsFocus) && !HasEffect(Buffs.LifeOfTheDragon) && !ActionReady(Geirskogul) && !ActionReady(ElusiveJump)) //for damage
                                || (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringSustain) && playerHP < GetOptionValue(Config.DRGPvP_CS_HP_Threshold)) //PlayerHP slider
                                || (!IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringSustain) && playerHP < 60)) //force heal self
                                return ChaoticSpring;
                        }
                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && ActionReady(Geirskogul) && farEnough)
                            return Geirskogul;
                        if (IsEnabled(CustomComboPreset.DRGPvP_WyrmwindThrust) && JustUsed(ElusiveJump, 3f) && GetTargetDistance() >= GetOptionValue(Config.DRGPvP_Distance_Threshold))
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