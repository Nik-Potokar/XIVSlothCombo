using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;


namespace XIVSlothComboPlugin.Combos
{
    internal static class BRDPvP
    {
        public const byte ClassID = 41;
        public const byte JobID = 23;

        public const uint
            PowerfulShot = 0,
            ApexArrow = 0,
            SilentNocturne = 0,
            EmpyrealArrow = 0,
            RepellingShot = 0,
            WardensPaean = 0,
            PitchPerfect = 0,
            BlastArrow = 0,
            FinalFantasia = 0;


        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 0,
                FrontlinersForte = 0,
                Repertoire = 0,
                BlastArrowReady = 0;
        }


        internal class BurstShotFeaturePVP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == BRDPvP.PowerfulShot)
                {
                    if (GetCooldown(BRDPvP.EmpyrealArrow).RemainingCharges == 3)
                        return OriginalHook(BRDPvP.EmpyrealArrow);

                    if (HasEffect(BRDPvP.Buffs.BlastArrowReady))
                        return OriginalHook(BRDPvP.BlastArrow);

                    if (HasEffect(BRDPvP.Buffs.Repertoire))
                        return OriginalHook(BRDPvP.PowerfulShot);

                    if (!GetCooldown(BRDPvP.ApexArrow).IsCooldown)
                        return OriginalHook(BRDPvP.ApexArrow);

                    if (!GetCooldown(BRDPvP.SilentNocturne).IsCooldown)
                        return OriginalHook(BRDPvP.SilentNocturne);

                    return OriginalHook(BRDPvP.PowerfulShot);
                }

                return actionID;
            }
        }
    }
    
}
