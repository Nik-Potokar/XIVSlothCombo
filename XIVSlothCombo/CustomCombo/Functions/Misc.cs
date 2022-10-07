using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Determine if the given preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is enabled. </returns>
        public static bool IsEnabled(CustomComboPreset preset) => (int)preset < 100 || Service.Configuration.IsEnabled(preset);

        /// <summary> Determine if the given preset is not enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is not enabled. </returns>
        public static bool IsNotEnabled(CustomComboPreset preset) => !IsEnabled(preset);

        internal static Dictionary<uint, ClassJob> ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!.ToDictionary(i => i.RowId, i => i);
        
        public static string JobIDToName(byte key)
        {
            //Override DOH/DOL
            if (key is DOH.JobID) key = 16; //Set to Miner
            if (key is DOL.JobID) key = 08; //Set to Carpenter
            if (ClassJobs.TryGetValue(key, out ClassJob? job))
            {
                //Grab Category name for DOH/DOL, else the normal Name for the rest
                string jobname = key is 08 or 16 ? job.ClassJobCategory.Value.Name : job.Name;
                //Job names are all lowercase by default. This capitalizes based on regional rules
                string cultureID = Service.ClientState.ClientLanguage switch
                {
                    Dalamud.ClientLanguage.French => "fr-FR",
                    Dalamud.ClientLanguage.Japanese => "ja-JP",
                    Dalamud.ClientLanguage.German => "de-DE",
                    _ => "en-us",
                };
                TextInfo textInfo = new CultureInfo(cultureID, false).TextInfo;
                return textInfo.ToTitleCase(jobname);

            } //Misc or unknown
            else return key == 99 ? "Global" : "Unknown";
        }

        // Job & Class Names
        public class JobIDs
        {
            //  Job IDs     ClassIDs (no jobstone) (Lancer, Pugilist, etc)
            public static readonly List<byte> Melee = new()
            {
                DRG.JobID, DRG.ClassID,
                MNK.JobID, MNK.ClassID,
                NIN.JobID, NIN.ClassID,
                RPR.JobID,
                SAM.JobID
            };

            public static readonly List<byte> Ranged = new()
            {
                BLM.JobID, BLM.ClassID,
                BRD.JobID, BRD.ClassID,
                SMN.JobID, SMN.ClassID,
                MCH.JobID,
                RDM.JobID,
                DNC.JobID,
                BLU.JobID
            };

            public static readonly List<byte> Tank = new()
            {
                PLD.JobID, PLD.ClassID,
                WAR.JobID, WAR.ClassID,
                DRK.JobID,
                GNB.JobID
            };

            public static readonly List<byte> Healer = new()
            {
                WHM.JobID, WHM.ClassID,
                SCH.JobID,
                AST.JobID,
                SGE.JobID
            };

        }
    }
}
