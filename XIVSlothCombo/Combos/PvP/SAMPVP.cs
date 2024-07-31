using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class SAMPvP
    {
        public const byte JobID = 34;

        public const uint
            KashakCombo = 58,
            Yukikaze = 29523,
            Gekko = 29524,
            Kasha = 29525,
            Hyosetsu = 29526,
            Mangetsu = 29527,
            Oka = 29528,
            OgiNamikiri = 29530,
            Soten = 29532,
            Chiten = 29533,
            Mineuchi = 29535,
            MeikyoShisui = 29536,
            Midare = 29529,
            Kaeshi = 29531,
            Zantetsuken = 29537;

        public static class Buffs
        {
            public const ushort
                Kaiten = 3201,
                Midare = 3203;
        }

        public static class Debuffs
        {
            public const ushort
                Kuzushi = 3202;
        }

        public static class Config
        {
            public const string
                SAMPvP_SotenCharges = "SamSotenCharges",
                SAMPvP_SotenHP = "SamSotenHP";

        }

        internal class SAMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var sotenCharges = PluginConfiguration.GetCustomIntValue(Config.SAMPvP_SotenCharges);

                if ((IsNotEnabled(CustomComboPreset.SAMPvP_BurstMode_MainCombo) && actionID == MeikyoShisui) ||
                    (IsEnabled(CustomComboPreset.SAMPvP_BurstMode_MainCombo) && actionID is Yukikaze or Gekko or Kasha or Hyosetsu or Oka or Mangetsu))
                {

                    if (!TargetHasEffectAny(PvPCommon.Buffs.Guard))
                    {
                        if (IsOffCooldown(MeikyoShisui))
                            return OriginalHook(MeikyoShisui);

                        if (IsEnabled(CustomComboPreset.SAMPvP_BurstMode_Chiten) && IsOffCooldown(Chiten) && InCombat() && PlayerHealthPercentageHp() <= 95)
                            return OriginalHook(Chiten);

                        if (GetCooldownRemainingTime(Soten) < 1 && CanWeave(Yukikaze))
                            return OriginalHook(Soten);

                        if (OriginalHook(MeikyoShisui) == Midare && !IsMoving)
                            return OriginalHook(MeikyoShisui);

                        if (IsEnabled(CustomComboPreset.SAMPvP_BurstMode_Stun) && IsOffCooldown(Mineuchi))
                            return OriginalHook(Mineuchi);

                        if (IsOffCooldown(OgiNamikiri) && !IsMoving)
                            return OriginalHook(OgiNamikiri);

                        if (GetRemainingCharges(Soten) > sotenCharges && CanWeave(Yukikaze))
                            return OriginalHook(Soten);

                        if (OriginalHook(OgiNamikiri) == Kaeshi)
                            return OriginalHook(OgiNamikiri);
                    }
                }

                return actionID;
            }
        }

        internal class SAMPvP_KashaFeatures : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAMPvP_KashaFeatures;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var SamSotenHP = PluginConfiguration.GetCustomIntValue(Config.SAMPvP_SotenHP);

                if (actionID is Yukikaze or Gekko or Kasha or Hyosetsu or Mangetsu or Oka)
                {
                    if (!InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.SAMPvP_KashaFeatures_GapCloser) && GetRemainingCharges(Soten) > 0 && GetTargetHPPercent() <= SamSotenHP)
                            return OriginalHook(Soten);

                        if (IsEnabled(CustomComboPreset.SAMPvP_KashaFeatures_AoEMeleeProtection) && !IsOriginal(Yukikaze) && !HasEffect(Buffs.Midare) && IsOnCooldown(MeikyoShisui) && IsOnCooldown(OgiNamikiri) && OriginalHook(OgiNamikiri) != Kaeshi)
                            return SAM.Yukikaze;
                    }
                }

                return actionID;
            }
        }
    }
}