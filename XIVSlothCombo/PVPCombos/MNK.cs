using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MNKPVP
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            Bootshine = 8780,
            TrueStrike = 8781,
            SnapPunch = 8782,
            Smite = 18992,
            AxeKick = 17670,
            TornadoKick = 8789;
    }

    internal class MnkBootshinePvPFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MnkBootshinePvPFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNKPVP.Bootshine || actionID == MNKPVP.TrueStrike || actionID == MNKPVP.SnapPunch)
            {
                var tornadoCD = GetCooldown(MNKPVP.TornadoKick);
                var axeKickCD = GetCooldown(MNKPVP.AxeKick);
                var actionIDCD = GetCooldown(actionID);
                var smiteCD = GetCooldown(MNKPVP.Smite);
                if (actionIDCD.IsCooldown && !axeKickCD.IsCooldown)
                    return MNKPVP.AxeKick;
                if (actionIDCD.IsCooldown && tornadoCD.CooldownRemaining < 15 || !tornadoCD.IsCooldown && actionIDCD.IsCooldown)
                    return MNKPVP.TornadoKick;
                if (actionIDCD.IsCooldown && !smiteCD.IsCooldown && InMeleeRange(true) && EnemyHealthPercentage() <= 30)
                    return MNKPVP.Smite;
            }

            return actionID;
        }
    }
}