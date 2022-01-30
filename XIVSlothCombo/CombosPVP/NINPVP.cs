using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class NINPVP
    {
        public const byte ClassID = 18;
        public const byte JobID = 30;

        public const uint
            SpinningEdge = 8807,
            GustSlash = 8808,
            AeolianEdge = 8809,
            Bunshin = 17734,
            Assassinate = 8814,
            HellfrogMedium = 17731,
            Smite = 18992,
            Bhavacakra = 8815;


        public static class Buffs
        {
            public const ushort
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                AssassinateReady = 1955,
                RaijuReady = 2690;
        }
    }

    internal class NinjaAeolianEdgePvpCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAeolianEdgePvpCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NINPVP.SpinningEdge || actionID == NINPVP.GustSlash || actionID == NINPVP.AeolianEdge)
            {
                var gauge = GetJobGauge<NINGauge>();
                var actionIDCD = GetCooldown(actionID);
                var smiteCD = GetCooldown(NINPVP.Smite);
                var assassinateCD = GetCooldown(NINPVP.Assassinate);
                var bunshinCD = GetCooldown(NINPVP.Bunshin);

                if (!bunshinCD.IsCooldown && actionIDCD.IsCooldown)
                    return NINPVP.Bunshin;
                if (gauge.Ninki >= 50 && actionIDCD.IsCooldown)
                    return NINPVP.Bhavacakra;
                if (!assassinateCD.IsCooldown && actionIDCD.IsCooldown)
                    return NINPVP.Assassinate;
                if (!smiteCD.IsCooldown && actionIDCD.IsCooldown && InMeleeRange(true) && EnemyHealthPercentage() <= 30)
                    return NINPVP.Smite;

            }

            return actionID;
        }
    }
}
