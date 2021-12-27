using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothComboPlugin;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothCombo.Combos
{
    internal static class All
    {
        public const byte JobID = 0;

        public const uint
            Swiftcast = 7561,
            Resurrection = 173,
            Verraise = 7523,
            Raise = 125,
            Ascend = 3603,
            Egeiro = 24287,
            SolidReason = 232,
            AgelessWords = 215,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Interject = 7538;

        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                EurekaMoment = 2765;
        }

        public static class Levels
        {
            public const byte
                Raise = 12;
        }
    }
    internal class InterruptFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.InterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.LowBlow)
            {
                var interjectCD = GetCooldown(All.Interject);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return All.Interject;
            }

            return actionID;
        }
    }
}
