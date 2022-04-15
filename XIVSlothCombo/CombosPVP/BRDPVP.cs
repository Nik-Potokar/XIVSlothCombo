using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BRDPvP
    {
        public const byte ClassID = 41;
        public const byte JobID = 23;

        public const uint
            PowerfulShot = 29391,
            ApexArrow = 29393,
            SilentNocturne = 29395,
            EmpyrealArrow = 29398,
            RepellingShot = 0,
            WardensPaean = 0,
            PitchPerfect = 29392,
            BlastArrow = 29394,
            FinalFantasia = 0;


        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 3138,
                FrontlinersForte = 3140,
                Repertoire = 3137,
                BlastArrowReady = 3142;
        }


        internal class BurstShotFeaturePVP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                
                if (actionID == BRDPvP.PowerfulShot)
                {
                    uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    if (globalAction != actionID) return globalAction;

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
