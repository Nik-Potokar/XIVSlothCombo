using System;
using System.Linq;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;

namespace XIVSlothComboPlugin.Combos
{
    /// <summary>
    /// Base class for each combo.
    /// </summary>
    internal abstract partial class CustomCombo
    {
        private const uint InvalidObjectID = 0xE000_0000;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCombo"/> class.
        /// </summary>
        protected CustomCombo()
        {
            var presetInfo = this.Preset.GetAttribute<CustomComboInfoAttribute>();
            this.JobID = presetInfo.JobID;
            this.ClassID = this.JobID switch
            {
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
            this.ActionIDs = presetInfo.ActionIDs;
        }

        /// <summary>
        /// Gets the preset associated with this combo.
        /// </summary>
        protected abstract CustomComboPreset Preset { get; }

        /// <summary>
        /// Gets the class ID associated with this combo.
        /// </summary>
        protected byte ClassID { get; }

        /// <summary>
        /// Gets the job ID associated with this combo.
        /// </summary>
        protected byte JobID { get; }

        /// <summary>
        /// Gets the action IDs associated with this combo.
        /// </summary>
        protected virtual uint[] ActionIDs { get; }

        /// <summary>
        /// Performs various checks then attempts to invoke the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="lastComboActionID">Last combo action.</param>
        /// <param name="comboTime">Current combo time.</param>
        /// <param name="level">Current player level.</param>
        /// <param name="newActionID">Replacement action ID.</param>
        /// <returns>True if the action has changed, otherwise false.</returns>
        public bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
        {
            newActionID = 0;

            if (!IsEnabled(this.Preset))
                return false;

            var classJobID = LocalPlayer?.ClassJob.Id;

            if (this.JobID != classJobID && this.ClassID != classJobID && this.JobID != 99)
                return false;

            if (!this.ActionIDs.Contains(actionID))
                return false;

            var resultingActionID = this.Invoke(actionID, lastComboActionID, comboTime, level);
            if (resultingActionID == 0 || actionID == resultingActionID)
                return false;

            newActionID = resultingActionID;
            return true;
        }

        /// <summary>
        /// Invokes the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="lastComboActionID">Last combo action.</param>
        /// <param name="comboTime">Current combo time.</param>
        /// <param name="level">Current player level.</param>
        /// <returns>The replacement action ID.</returns>
        protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);
    }

    /// <summary>
    /// Passthrough methods and properties to IconReplacer. Shortens what it takes to call each method.
    /// </summary>
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Gets the player or null.
        /// </summary>
        protected static PlayerCharacter? LocalPlayer => Service.ClientState.LocalPlayer;

        /// <summary>
        /// Gets the current target or null.
        /// </summary>
        protected static GameObject? CurrentTarget => Service.TargetManager.Target;

        /// <summary>
        /// Calls the original hook.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <returns>The result from the hook.</returns>
        protected static uint OriginalHook(uint actionID) => Service.IconReplacer.OriginalHook(actionID);

        /// <summary>
        /// Determine if the given preset is enabled.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>A value indicating whether the preset is enabled.</returns>
        protected static bool IsEnabled(CustomComboPreset preset) => Service.Configuration.IsEnabled(preset);

        /// <summary>
        /// Find if the player is in condition.
        /// </summary>
        /// <param name="flag">Condition flag.</param>
        /// <returns>A value indicating whether the player is in the condition.</returns>
        protected static bool HasCondition(ConditionFlag flag) => Service.Condition[flag];

        /// <summary>
        /// Find if an effect on the player exists.
        /// The effect may be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        protected static bool HasEffect(short effectID) => FindEffect(effectID) is not null;

        /// <summary>
        /// Finds an effect on the player.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        protected static Status? FindEffect(short effectID) => FindEffect(effectID, LocalPlayer, LocalPlayer?.ObjectId);

        /// <summary>
        /// Find if an effect on the target exists.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        protected static bool TargetHasEffect(short effectID) => FindTargetEffect(effectID) is not null;

        /// <summary>
        /// Finds an effect on the current target.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        protected static Status? FindTargetEffect(short effectID) => FindEffect(effectID, CurrentTarget, LocalPlayer?.ObjectId);

        /// <summary>
        /// Find if an effect on the player exists.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        protected static bool HasEffectAny(short effectID) => FindEffectAny(effectID) is not null;

        /// <summary>
        /// Finds an effect on the player.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        protected static Status? FindEffectAny(short effectID) => FindEffect(effectID, LocalPlayer, null);

        /// <summary>
        /// Find if an effect on the target exists.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        protected static bool TargetHasEffectAny(short effectID) => FindTargetEffectAny(effectID) is not null;

        /// <summary>
        /// Finds an effect on the current target.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        protected static Status? FindTargetEffectAny(short effectID) => FindEffect(effectID, CurrentTarget, null);

        /// <summary>
        /// Finds an effect on the given object.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <param name="obj">Object to look for effects on.</param>
        /// <param name="sourceID">Source object ID.</param>
        /// <returns>Status object or null.</returns>
        protected static Status? FindEffect(short effectID, GameObject? obj, uint? sourceID)
        {
            if (obj is null)
                return null;

            if (obj is not BattleChara chara)
                return null;

            foreach (var status in chara.StatusList)
            {
                if (status.StatusId == effectID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == InvalidObjectID || status.SourceID == sourceID))
                    return status;
            }

            return null;
        }

        /// <summary>
        /// Gets the cooldown data for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Cooldown data.</returns>
        protected static IconReplacer.CooldownData GetCooldown(uint actionID) => Service.IconReplacer.GetCooldown(actionID);

        /// <summary>
        /// Gets the job gauge.
        /// </summary>
        /// <typeparam name="T">Type of job gauge.</typeparam>
        /// <returns>The job gauge.</returns>
        protected static T GetJobGauge<T>() where T : JobGaugeBase => Service.JobGauges.Get<T>();

        protected static uint CalcBestAction(uint original, params uint[] actions)
        {
            static (uint ActionID, IconReplacer.CooldownData Data) Compare(
                uint original,
                (uint ActionID, IconReplacer.CooldownData Data) a1,
                (uint ActionID, IconReplacer.CooldownData Data) a2)
            {
                // Neither, return the first parameter
                if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
                    return original == a1.ActionID ? a1 : a2;

                // Both, return soonest available
                if (a1.Data.IsCooldown && a2.Data.IsCooldown)
                    return a1.Data.CooldownRemaining < a2.Data.CooldownRemaining ? a1 : a2;

                // One or the other
                return a1.Data.IsCooldown ? a2 : a1;
            }

            static (uint ActionID, IconReplacer.CooldownData Data) Selector(uint actionID)
            {
                return (actionID, GetCooldown(actionID));
            }

            return actions
                .Select(Selector)
                .Aggregate((a1, a2) => Compare(original, a1, a2))
                .ActionID;
        }
        /// <summary>
        /// Determines if the enemy can be interrupted if they are currently casting.
        /// </summary>
        /// <returns>Bool indicating whether they can be interrupted or not.</returns>
        protected static bool CanInterruptEnemy()
        {
            if (CurrentTarget is null)
                return false;
            if (CurrentTarget is not BattleChara chara)
                return false;
            if (chara.IsCasting)
                return chara.IsCastInterruptible;
            return false;
        }
        /// <summary>
        /// Gets the distance from the target.
        /// </summary>
        /// <returns>Double representing the distance from the target.</returns>
        protected static double GetTargetDistance()
        {
            if (CurrentTarget is null)
                return 0;

            if (CurrentTarget is not BattleChara chara)
                return 0;

            double distanceX = chara.YalmDistanceX;
            double distanceY = chara.YalmDistanceZ;

            return Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        /// <summary>
        /// Determines if you are in melee range from the current target.
        /// </summary>
        /// <returns>Bool indicating whether you are in melee range.</returns>
        protected static bool InMeleeRange(bool v)
        {
            var distance = GetTargetDistance();

            if (distance == 0)
                return true;

            if (distance > 3)
                return false;

            return true;
        }
        /*
         * #ToDo implement
         * Track targets current hp (In some cases you want to use a certain skill instead of another one depenidng on targets hp aka dots)
            if (Service.TargetManager.Target is Character chara)
            {
                var currentHp = chara.CurrentHp;
                var maxHp = chara.maxHp;
                var percentageMaxHp = (chara.CurrentHp / maxHp) * 100;

                if (currentHp > amount)
                    return class.skillname;
            }
        */
    }
}