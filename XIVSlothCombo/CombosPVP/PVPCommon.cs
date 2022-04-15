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
                EmergencyGuardThreshold = "EmergencyGuardThreshold";
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
                Heavy = 1344;
        }


        internal class GlobalEmergencyHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PVPEmergencyHeals;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {   
                if (TargetHasEffect(BRDPvP.Buffs.Repertoire)) { }
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
                return actionID;
            }

            public static bool Execute()
            {
                var jobMaxHp = LocalPlayer.MaxHp;
                var threshold = Service.Configuration.GetCustomIntValue(PVPCommon.Config.EmergencyGuardThreshold);
                var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)jobMaxHp;

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
                return actionID;
            }

            public static bool Execute()
            {
                
                return true;

            }
        }
    }

}
