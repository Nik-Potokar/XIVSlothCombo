using Dalamud.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.CustomComboNS
{
    /// <summary> Base class for each combo. </summary>
    internal abstract partial class CustomCombo : CustomComboFunctions
    {
        /// <summary> Initializes a new instance of the <see cref="CustomCombo"/> class. </summary>
        protected CustomCombo()
        {
            CustomComboInfoAttribute? presetInfo = Preset.GetAttribute<CustomComboInfoAttribute>();
            JobID = presetInfo.JobID;
            ClassID = JobIDs.JobToClass(JobID);

            StartTimer();
        }

        /// <summary> Gets the preset associated with this combo. </summary>
        protected internal abstract CustomComboPreset Preset { get; }

        /// <summary> Gets the class ID associated with this combo. </summary>
        protected byte ClassID { get; }

        /// <summary> Gets the job ID associated with this combo. </summary>
        protected byte JobID { get; }

        /// <summary> Performs various checks then attempts to invoke the combo. </summary>
        /// <param name="actionID"> Starting action ID. </param>
        /// <param name="level"> Player level. </param>
        /// <param name="lastComboMove"> Last combo action ID. </param>
        /// <param name="comboTime"> Combo timer. </param>
        /// <param name="newActionID"> Replacement action ID. </param>
        /// <returns> True if the action has changed, otherwise false. </returns>

        public unsafe bool TryInvoke(uint actionID, byte level, uint lastComboMove, float comboTime, out uint newActionID)
        {
            newActionID = 0;

            if (!Svc.ClientState.IsPvP && ActionManager.Instance()->QueuedActionType == ActionType.Action && ActionManager.Instance()->QueuedActionId != actionID)
                return false;

            if (!IsEnabled(Preset))
                return false;

            uint classJobID = LocalPlayer!.ClassJob.Id;

            if (classJobID is >= 8 and <= 15)
                classJobID = DOH.JobID;

            if (classJobID is >= 16 and <= 18)
                classJobID = DOL.JobID;

            if (JobID != ADV.JobID && ClassID != ADV.ClassID &&
                JobID != classJobID && ClassID != classJobID)
                return false;

            uint resultingActionID = Invoke(actionID, lastComboMove, comboTime, level);
            //Dalamud.Logging.PluginLog.Debug(resultingActionID.ToString());

            if (resultingActionID == 0 || actionID == resultingActionID)
                return false;

            newActionID = resultingActionID;

            return true;
        }

        /// <summary> Invokes the combo. </summary>
        /// <param name="actionID"> Starting action ID. </param>
        /// <param name="lastComboActionID"> Last combo action. </param>
        /// <param name="comboTime"> Current combo time. </param>
        /// <param name="level"> Current player level. </param>
        /// <returns>The replacement action ID. </returns>
        protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);
    }
}
