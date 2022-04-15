using System;
using System.Collections.Generic;

using Dalamud.Game;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Cached conditional combo logic.
    /// </summary>
    internal partial class CustomComboCache : IDisposable
    {
        private const uint InvalidObjectID = 0xE000_0000;

        // Invalidate these
        private readonly Dictionary<(uint StatusID, uint? TargetID, uint? SourceID), Status?> statusCache = new();
        private readonly Dictionary<uint, CooldownData> cooldownCache = new();

        // Do not invalidate these
        private readonly Dictionary<uint, byte> cooldownGroupCache = new();
        private readonly Dictionary<Type, JobGaugeBase> jobGaugeCache = new();
        private readonly Dictionary<(uint ActionID, uint ClassJobID, byte Level), (ushort CurrentMax, ushort Max)> chargesCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComboCache"/> class.
        /// </summary>
        public CustomComboCache()
        {
            Service.Framework.Update += this.Framework_Update;
        }

        private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

        /// <inheritdoc/>
        public void Dispose()
        {
            Service.Framework.Update -= this.Framework_Update;
        }

        /// <summary>
        /// Get a job gauge.
        /// </summary>
        /// <typeparam name="T">Type of job gauge.</typeparam>
        /// <returns>The job gauge.</returns>
        internal T GetJobGauge<T>() where T : JobGaugeBase
        {
            if (!this.jobGaugeCache.TryGetValue(typeof(T), out var gauge))
                gauge = this.jobGaugeCache[typeof(T)] = Service.JobGauges.Get<T>();

            return (T)gauge;
        }

        /// <summary>
        /// Finds a status on the given object.
        /// </summary>
        /// <param name="statusID">Status effect ID.</param>
        /// <param name="obj">Object to look for effects on.</param>
        /// <param name="sourceID">Source object ID.</param>
        /// <returns>Status object or null.</returns>
        internal Status? GetStatus(uint statusID, GameObject? obj, uint? sourceID)
        {
            var key = (statusID, obj?.ObjectId, sourceID);
            if (this.statusCache.TryGetValue(key, out var found))
                return found;

            if (obj is null)
                return this.statusCache[key] = null;

            if (obj is not BattleChara chara)
                return this.statusCache[key] = null;

            foreach (var status in chara.StatusList)
            {
#if DEBUG
                Dalamud.Logging.PluginLog.Debug($"{chara.Name} -> {status.StatusId}");
#endif

                if (status.StatusId == statusID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == InvalidObjectID || status.SourceID == sourceID))
                    return this.statusCache[key] = status;
            }

            return this.statusCache[key] = null;
        }

        /// <summary>
        /// Gets the cooldown data for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Cooldown data.</returns>
        internal unsafe CooldownData GetCooldown(uint actionID)
        {
            if (this.cooldownCache.TryGetValue(actionID, out var found))
                return found;

            var actionManager = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            if (actionManager == null)
                return this.cooldownCache[actionID] = default;

            var cooldownGroup = this.GetCooldownGroup(actionID);

            var cooldownPtr = actionManager->GetRecastGroupDetail(cooldownGroup - 1);
            cooldownPtr->ActionID = actionID;

            return this.cooldownCache[actionID] = *(CooldownData*)cooldownPtr;
        }

        /// <summary>
        /// Get the maximum number of charges for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Max number of charges at current and max level.</returns>
        internal unsafe (ushort Current, ushort Max) GetMaxCharges(uint actionID)
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null)
                return (0, 0);

            var job = player.ClassJob.Id;
            var level = player.Level;
            if (job == 0 || level == 0)
                return (0, 0);

            var key = (actionID, job, level);
            if (this.chargesCache.TryGetValue(key, out var found))
                return found;

            var cur = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.GetMaxCharges(actionID, 0);
            var max = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.GetMaxCharges(actionID, 90);
            return this.chargesCache[key] = (cur, max);
        }

        private byte GetCooldownGroup(uint actionID)
        {
            if (this.cooldownGroupCache.TryGetValue(actionID, out var cooldownGroup))
                return cooldownGroup;

            var sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
            var row = sheet.GetRow(actionID);

            return this.cooldownGroupCache[actionID] = row!.CooldownGroup;
        }

        private unsafe void Framework_Update(Framework framework)
        {
            this.statusCache.Clear();
            this.cooldownCache.Clear();
        }
    }
}
