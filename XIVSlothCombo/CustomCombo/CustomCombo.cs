using Dalamud.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Component.GUI;
using XIVSlothCombo.Services;


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
            ClassID = JobID switch
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

    /// <summary> Base class for each combo. </summary>
    internal abstract partial class CustomCombo : CustomComboFunctions
    {
        #region Boss Check
        // Rank 0 - Trash
        // Rank 1 - Hunt Target (S/A/B). Eureka Pazuzu is also Rank 1. Fate bosses might also be R1. Haven't checked.
        // Rank 2 - Final Dungeon Boss, Trial Boss, Raid Boss, Alliance Raid Boss.
        // Rank 3 - Trash
        // Rank 4 - Raid Trash (Alexander)
        // Rank 5 - There is no Rank 5
        // Rank 6 - First 2 bosses in dungeons.
        // Rank 7 - PvP stuff? 
        // Rank 8 - A puppy.
        // 32, 33 34, 35, 36, 37 - Old Diadem mobs.
        // Rank checks are very reliable. The HP check is done for unsynced content mainly.
        // There are probably better ways of doing than relying on HP for unsync stuff but this works.
        // For how to use, obviously just add a BossCheck() on dots for example. (Higanbana would be the perfect use case since the dot takes so long to be worth using)
        protected static uint DataId()
        {
            if (CurrentTarget is not IBattleChara chara)
                return 0;
            return chara.DataId;
        }
        public static bool BossCheck()
        {
            double maxHealth = LocalPlayer.MaxHp;
            var rank = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.BNpcBase>()?.GetRow((uint)DataId());

            return rank != null && EnemyHealthMaxHp() >= maxHealth * 11 && (rank.Rank == 2 || rank.Rank == 6);
        }
        public static bool BossCheckLast()
        {
            double maxHealth = LocalPlayer.MaxHp;
            var rank = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.BNpcBase>()?.GetRow((uint)DataId());

            return rank != null && EnemyHealthMaxHp() >= maxHealth * 11 && (rank.Rank == 2);
        }
        #endregion

        #region Item Usage
        public unsafe void UseItem(uint itemId)
        {
            FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->UseAction(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, itemId, 0xE0000000, 65535, 0, 0, null);
        }
        #endregion
        #region Can use potion
        public unsafe static bool CanUse()
        {
            var PotionCDGroup = 68;
            bool canpot = ActionManager.Instance()->GetRecastGroupDetail(PotionCDGroup)->IsActive == 0;
            return canpot;
        }
        // Execution can be called with something like this if (CanUse()) UseItem(1038956); 
        // that's the code for a HQ Hyper-Potion. 
        // This does NOT replace the action on the hotbar. The item is just...used so the conditions must be quite strict, but it works.

        #endregion

        #region Limit Break
        // Not much else to say. Gets called like if (HasPVPLimitBreak()) return MarksmanSpite;
        // Ofc, additional conditions as required, no guard, no full on HP.
        public unsafe bool HasPVPLimitBreak()
        {
            AtkUnitBase* LBWidget = (AtkUnitBase*)Svc.GameGui.GetAddonByName("_LimitBreak", 1);
            if (LBWidget->UldManager.SearchNodeById(6)->GetComponent()->UldManager.SearchNodeById(3)->Width >= 146)
                return true;
            return false;
        }
        #endregion

    }

}
