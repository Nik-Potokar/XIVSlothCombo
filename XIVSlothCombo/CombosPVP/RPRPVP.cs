using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class RPRPVP
    {
        public const byte JobID = 39;

        public const uint
            Slice = 27795,
            WaxingSlice = 27796,
            InfernalSlice = 27797,
            SpinningScythe = 27802,
            NightmareScythe = 27803,
            SoulSlice = 27808,
            SoulScythe = 27809,
            BloodStalk = 27810,
            GrimSwate = 27813,
            Gluttony = 27814,
            LemureSlice = 27815,
            Communio = 27807,
            VoidReaping = 27800,
            CrossReaping = 27801,
            Smite = 18992,
            GrimReaping = 27805;



        public static class Buffs
        {
            public const ushort
                Enshrouded = 2863;
        }
    }

    internal class InfernalSliceComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.InfernalSliceComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPRPVP.Slice || actionID == RPRPVP.WaxingSlice || actionID == RPRPVP.InfernalSlice || actionID == RPRPVP.VoidReaping || actionID == RPRPVP.CrossReaping)
            {
                var gauge = GetJobGauge<RPRGauge>();
                var actionIDCD = GetCooldown(actionID);
                var smiteCD = GetCooldown(RPRPVP.Smite);
                var gluttonyCD = GetCooldown(RPRPVP.Gluttony);

                if (gauge.Soul >= 50 && !gluttonyCD.IsCooldown && actionIDCD.IsCooldown)
                    return RPRPVP.Gluttony;
                if (gauge.Soul >= 50 && gluttonyCD.IsCooldown && actionIDCD.IsCooldown)
                    return RPRPVP.BloodStalk;
                if (HasEffect(RPRPVP.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                    return RPRPVP.LemureSlice;
                if (HasEffect(RPRPVP.Buffs.Enshrouded) && gauge.LemureShroud == 1)
                    return RPRPVP.Communio;
                if (EnemyHealthPercentage() <= 30 && InMeleeRange(true) && actionIDCD.IsCooldown && !smiteCD.IsCooldown)
                    return RPRPVP.Smite;
            }

            return actionID;
        }
    }
    internal class NightmareScytheComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NightmareScytheComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPRPVP.SpinningScythe || actionID == RPRPVP.NightmareScythe || actionID == RPRPVP.GrimReaping)
            {
                var gauge = GetJobGauge<RPRGauge>();
                var actionIDCD = GetCooldown(actionID);
                var smiteCD = GetCooldown(RPRPVP.Smite);
                var gluttonyCD = GetCooldown(RPRPVP.Gluttony);

                if (gauge.Soul >= 50 && !gluttonyCD.IsCooldown && actionIDCD.IsCooldown)
                    return RPRPVP.Gluttony;
                if (gauge.Soul >= 50 && gluttonyCD.IsCooldown && actionIDCD.IsCooldown)
                    return RPRPVP.GrimSwate;
                if (HasEffect(RPRPVP.Buffs.Enshrouded) && gauge.VoidShroud >= 2)
                    return RPRPVP.LemureSlice;
                if (HasEffect(RPRPVP.Buffs.Enshrouded) && gauge.LemureShroud == 1)
                    return RPRPVP.Communio;
                if (EnemyHealthPercentage() <= 30 && InMeleeRange(true) && actionIDCD.IsCooldown && !smiteCD.IsCooldown)
                    return RPRPVP.Smite;
            }

            return actionID;
        }
    }
}