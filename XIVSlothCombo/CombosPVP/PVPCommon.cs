using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class PVPCommon
    {
        public const uint
            StandardElixir = 29055,
            Recuperate = 29711,
            Purify = 29056,
            Guard = 29735,
            Sprint = 29057;


        internal class GlobalEmergencyHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PVPEmergencyHeals;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                return actionID;
            }

            public static uint Execute(uint actionID)
            {
                var jobMaxHp = LocalPlayer.MaxHp;

                if (LocalPlayer.CurrentMp < 2500) return actionID;
                if (LocalPlayer.CurrentHp > jobMaxHp - 15000) return actionID;

                return PVPCommon.Recuperate;

            }
        }
    }

}
