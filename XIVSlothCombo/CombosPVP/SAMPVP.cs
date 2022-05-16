namespace XIVSlothComboPlugin.Combos
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
                SamSotenCharges = "SamSotenCharges",
                SamSotenHP = "SamSotenHP";

        }

        internal class SAMBurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAMBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var sotenCharges = Service.Configuration.GetCustomIntValue(Config.SamSotenCharges);
                
                if ((IsNotEnabled(CustomComboPreset.SamPVPMainComboFeature) && actionID == MeikyoShisui) ||
                    (IsEnabled(CustomComboPreset.SamPVPMainComboFeature) && actionID is Yukikaze or Gekko or Kasha or Hyosetsu or Oka or Mangetsu))
                {
                    //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    if (!TargetHasEffectAny(PVPCommon.Buffs.Guard))
                    {
                        if (IsOffCooldown(MeikyoShisui))
                            return OriginalHook(MeikyoShisui);
                        if (IsEnabled(CustomComboPreset.SAMBurstChitenFeature) && IsOffCooldown(Chiten) && InCombat() && PlayerHealthPercentageHp() <= 95)
                            return OriginalHook(Chiten);
                        if (GetCooldownRemainingTime(Soten) < 1 && CanWeave(Yukikaze))
                            return OriginalHook(Soten);
                        if (OriginalHook(MeikyoShisui) == Midare && !this.IsMoving)
                            return OriginalHook(MeikyoShisui);
                        if (IsEnabled(CustomComboPreset.SAMBurstStunFeature) && IsOffCooldown(Mineuchi))
                            return OriginalHook(Mineuchi);
                        if (IsOffCooldown(OgiNamikiri) && !this.IsMoving)
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

        internal class SamPvPKashaFeatures : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamPvPKashaFeatures;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var SamSotenHP = Service.Configuration.GetCustomIntValue(Config.SamSotenHP);

                if (actionID is Yukikaze or Gekko or Kasha or Hyosetsu or Mangetsu or Oka)
                {
                    if (!InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.SamGapCloserFeature) && GetRemainingCharges(Soten) > 0 && EnemyHealthPercentage() <= SamSotenHP)
                            return OriginalHook(Soten);
                        if (IsEnabled(CustomComboPreset.SamAOEMeleeFeature) && !IsOriginal(Yukikaze) && !HasEffect(Buffs.Midare) && IsOnCooldown(MeikyoShisui) && IsOnCooldown(OgiNamikiri) && OriginalHook(OgiNamikiri) != Kaeshi)
                            return SAM.Yukikaze;
                    }
                }

                return actionID;
            }
        }
    }
}