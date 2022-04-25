using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class PVPCommon
    {
        public const uint
            StandardElixir = 29055,
            Recuperate = 29711,
            Purify = 29056,
            Guard = 29054,
            Sprint = 29057;

        internal class Config
        {
            public const string
                EmergencyHealThreshold = "EmergencyHealThreshold",
                EmergencyGuardThreshold = "EmergencyGuardThreshold",
                QuickPurifyStatuses = "QuickPurifyStatuses";
        }

        internal class Debuffs
        {
            public const ushort
                Silence = 1347,
                Bind = 1345,
                Stun = 1343,
                HalfAsleep = 3022,
                Sleep = 1348,
                DeepFreeze = 3219,
                Heavy = 1344,
                Unguarded = 3021;
        }

        internal class Buffs
        {
            public const ushort
                Guard = 3054;
        }


        internal class GlobalEmergencyHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PVPEmergencyHeals;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if ((HasEffect(PVPCommon.Buffs.Guard) || JustUsed(PVPCommon.Guard)) && IsEnabled(CustomComboPreset.PVPMashCancel))
                {
                    if (actionID == PVPCommon.Guard) return PVPCommon.Guard;
                    else return OriginalHook(11);
                }
                if (Execute() && InPvP() && actionID != PVPCommon.Guard && actionID != PVPCommon.Recuperate && actionID != PVPCommon.Purify && actionID != PVPCommon.StandardElixir) return OriginalHook(PVPCommon.Recuperate);
                return actionID;
            }

            public static bool Execute()
            {
                var jobMaxHp = LocalPlayer.MaxHp;
                var threshold = Service.Configuration.GetCustomIntValue(PVPCommon.Config.EmergencyHealThreshold);
                var maxHPThreshold = jobMaxHp - 15000;
                var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;



                if (LocalPlayer.CurrentMp < 2500) return false;
                if (remainingPercentage * 100 > threshold) return false;

                return true;

            }
        }

        internal class GlobalEmergencyGuard : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PVPEmergencyGuard;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if ((HasEffect(PVPCommon.Buffs.Guard) || JustUsed(PVPCommon.Guard)) && IsEnabled(CustomComboPreset.PVPMashCancel))
                {
                    if (actionID == PVPCommon.Guard) return PVPCommon.Guard;
                    else return OriginalHook(11);
                }
                if (Execute() && InPvP() && actionID != PVPCommon.Guard && actionID != PVPCommon.Recuperate && actionID != PVPCommon.Purify && actionID != PVPCommon.StandardElixir) return OriginalHook(PVPCommon.Guard);
                return actionID;
            }

            public static bool Execute()
            {
                var jobMaxHp = LocalPlayer.MaxHp;
                var threshold = Service.Configuration.GetCustomIntValue(PVPCommon.Config.EmergencyGuardThreshold);
                var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)jobMaxHp;

                if (HasEffectAny(PVPCommon.Debuffs.Unguarded) || HasEffect(WARPVP.Buffs.InnerRelease)) return false;
                if (GetCooldown(PVPCommon.Guard).IsCooldown) return false;
                if (remainingPercentage * 100 > threshold) return false;

                return true;

            }
        }

        internal class QuickPurify : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PVPQuickPurify;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if ((HasEffect(PVPCommon.Buffs.Guard) || JustUsed(PVPCommon.Guard)) && IsEnabled(CustomComboPreset.PVPMashCancel))
                {
                    if (actionID == PVPCommon.Guard) return PVPCommon.Guard;
                    else return OriginalHook(11);
                }
                if (Execute() && InPvP() && actionID != PVPCommon.Guard && actionID != PVPCommon.Recuperate && actionID != PVPCommon.Purify && actionID != PVPCommon.StandardElixir) return OriginalHook(PVPCommon.Purify);
                return actionID;
            }

            public static bool Execute()
            {
                var selectedStatuses = Service.Configuration.GetCustomBoolArrayValue(Config.QuickPurifyStatuses);


                if (selectedStatuses.Length == 0) return false;
                if (GetCooldown(PVPCommon.Purify).IsCooldown) return false;
                if (HasEffectAny(PVPCommon.Debuffs.Stun) && selectedStatuses[0]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.DeepFreeze) && selectedStatuses[1]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.HalfAsleep) && selectedStatuses[2]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.Sleep) && selectedStatuses[3]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.Bind) && selectedStatuses[4]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.Heavy) && selectedStatuses[5]) return true;
                if (HasEffectAny(PVPCommon.Debuffs.Silence) && selectedStatuses[6]) return true;

                return false;

            }
        }

    }

}
