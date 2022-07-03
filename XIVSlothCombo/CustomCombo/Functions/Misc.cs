using System.Collections.Generic;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Determine if the given preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is enabled. </returns>
        public bool IsEnabled(CustomComboPreset preset) => (int)preset < 100 || Service.Configuration.IsEnabled(preset);

        /// <summary> Determine if the given preset is not enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is not enabled. </returns>
        public bool IsNotEnabled(CustomComboPreset preset) => !IsEnabled(preset);

        
        // Job & Class Names
        public class JobIDs
        {
            //  Job IDs               ClassIDs (no jobstone) (Lancer, Pugilist, etc)
            public static readonly List<byte> Melee = new()
            {
                Combos.PvE.DRG.JobID, Combos.PvE.DRG.ClassID,
                Combos.PvE.MNK.JobID, Combos.PvE.MNK.ClassID,
                Combos.PvE.NIN.JobID, Combos.PvE.NIN.ClassID,
                Combos.PvE.RPR.JobID,
                Combos.PvE.SAM.JobID
            };

            public static readonly List<byte> Ranged = new()
            {
                Combos.PvE.BLM.JobID, Combos.PvE.BLM.ClassID,
                Combos.PvE.BRD.JobID, Combos.PvE.BRD.ClassID,
                Combos.PvE.SMN.JobID, Combos.PvE.SMN.ClassID,
                Combos.PvE.MCH.JobID,
                Combos.PvE.RDM.JobID,
                Combos.PvE.DNC.JobID,
                Combos.PvE.BLU.JobID
            };

            public static readonly List<byte> Tank = new()
            {
                Combos.PvE.PLD.JobID, Combos.PvE.PLD.ClassID,
                Combos.PvE.WAR.JobID, Combos.PvE.WAR.ClassID,
                Combos.PvE.DRK.JobID,
                Combos.PvE.GNB.JobID
            };
            
            public static readonly List<byte> Healer = new()
            {
                Combos.PvE.WHM.JobID, Combos.PvE.WHM.ClassID,
                Combos.PvE.SCH.JobID,
                Combos.PvE.AST.JobID,
                Combos.PvE.SGE.JobID
            };

        }
    }
}
