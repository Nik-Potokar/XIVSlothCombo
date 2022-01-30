using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BLMPVP
    {
        public const byte ClassID = 7;
        public const byte JobID = 25;

        public const uint
            Fire = 8858,
            Blizzard = 8859,
            Thunder = 8860,
            Thunder2 = 18935,
            Flare = 8866,
            Freeze = 8867,
            Enochian = 8862,
            Fire4 = 8863,
            Blizzard4 = 8864,
            Thunder3 = 8861,
            Thunder4 = 18936,
            PhantomDart = 17684,
            Xenoglossy = 17775,
            Fould = 8865;
    }
    internal class BlackEnochianPVPFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackEnochianPVPFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLMPVP.Enochian)
            {
                var gauge = GetJobGauge<BLMGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var dartCD = GetCooldown(BLMPVP.PhantomDart);
                if (lastComboMove == BLMPVP.Fire && gauge.InAstralFire && !dartCD.IsCooldown)
                    return BLMPVP.PhantomDart;
                if (!gauge.InAstralFire && !gauge.InUmbralIce)
                    return BLMPVP.Fire;
                if(gauge.InAstralFire && gauge.ElementTimeRemaining < 3)
                    return BLMPVP.Fire;
                if (gauge.InAstralFire && LocalPlayer.CurrentMp <= 1000)
                    return BLMPVP.Blizzard;
                if (gauge.InUmbralIce && LocalPlayer.CurrentMp >= 10000)
                    return BLMPVP.Fire;
            }

            return actionID;
        }
    }
}
