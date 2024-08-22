using System.Collections.Generic;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Determine if the given preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is enabled. </returns>
        public static bool IsEnabled(CustomComboPreset preset) => (int)preset < 100 || PresetStorage.IsEnabled(preset);

        /// <summary> Determine if the given preset is not enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is not enabled. </returns>
        public static bool IsNotEnabled(CustomComboPreset preset) => !IsEnabled(preset);

        public class JobIDs
        {
            //  Job IDs     ClassIDs (no jobstone) (Lancer, Pugilist, etc)
            public static readonly List<byte> Melee =
            [
                DRG.JobID, DRG.ClassID,
                MNK.JobID, MNK.ClassID,
                NIN.JobID, NIN.ClassID,
                VPR.JobID,
                RPR.JobID,
                SAM.JobID
            ];

            public static readonly List<byte> Ranged =
            [
                BLM.JobID, BLM.ClassID,
                BRD.JobID, BRD.ClassID,
                SMN.JobID, SMN.ClassID,
                PCT.JobID,
                MCH.JobID,
                RDM.JobID,
                DNC.JobID,
                BLU.JobID
            ];

            public static readonly List<byte> Tank =
            [
                PLD.JobID, PLD.ClassID,
                WAR.JobID, WAR.ClassID,
                DRK.JobID,
                GNB.JobID
            ];

            public static readonly List<byte> Healer =
            [
                WHM.JobID, WHM.ClassID,
                SCH.JobID,
                AST.JobID,
                SGE.JobID
            ];

            public static byte JobToClass(uint jobID)
            {
                return jobID switch
                {
                    ADV.JobID => ADV.ClassID,
                    BLM.JobID => BLM.ClassID,
                    BRD.JobID => BRD.ClassID,
                    DRG.JobID => DRG.ClassID,
                    MNK.JobID => MNK.ClassID,
                    NIN.JobID => NIN.ClassID,
                    PLD.JobID => PLD.ClassID,
                    SCH.JobID => SCH.ClassID,
                    SMN.JobID => SMN.ClassID,
                    WAR.JobID => WAR.ClassID,
                    WHM.JobID => WHM.ClassID,
                    _ => 0xFF,
                };
            }

        }
    }
}
