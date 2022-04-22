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
            RepellingShot = 29399,
            WardensPaean = 29400,
            PitchPerfect = 29392,
            BlastArrow = 29394,
            FinalFantasia = 29401;


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
                    var canWeave = CanWeave(actionID);
                    //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    //if (globalAction != actionID) return globalAction;

                    if (GetCooldown(BRDPvP.EmpyrealArrow).RemainingCharges == 3 && canWeave)
                        return OriginalHook(BRDPvP.EmpyrealArrow);

                    if (!GetCooldown(BRDPvP.SilentNocturne).IsCooldown && canWeave)
                        return OriginalHook(BRDPvP.SilentNocturne);

                    if (HasEffect(BRDPvP.Buffs.BlastArrowReady))
                        return OriginalHook(BRDPvP.BlastArrow);

                    if (HasEffect(BRDPvP.Buffs.Repertoire))
                        return OriginalHook(BRDPvP.PowerfulShot);

                    if (!GetCooldown(BRDPvP.ApexArrow).IsCooldown)
                        return OriginalHook(BRDPvP.ApexArrow);


                    return OriginalHook(BRDPvP.PowerfulShot);
                }
                
                return actionID;
            }
        }
    }
    
}
