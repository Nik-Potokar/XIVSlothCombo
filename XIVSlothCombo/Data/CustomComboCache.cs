using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using DalamudStatus = Dalamud.Game.ClientState.Statuses; // conflicts with structs if not defined
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Services;
using Dalamud.Plugin.Services;
using System.Collections.Concurrent;

namespace XIVSlothCombo.Data
{
    /// <summary> Cached conditional combo logic. </summary>
    internal partial class CustomComboCache : IDisposable
    {
        private const uint InvalidObjectID = 0xE000_0000;

        // Invalidate these
        private readonly ConcurrentDictionary<(uint StatusID, ulong? TargetID, ulong? SourceID), DalamudStatus.Status?> statusCache = new();
        private readonly ConcurrentDictionary<uint, CooldownData> cooldownCache = new();

        // Do not invalidate these
        private readonly ConcurrentDictionary<uint, byte> cooldownGroupCache = new();
        private readonly ConcurrentDictionary<Type, JobGaugeBase> jobGaugeCache = new();
        private readonly ConcurrentDictionary<(uint ActionID, uint ClassJobID, byte Level), (ushort CurrentMax, ushort Max)> chargesCache = new();

        /// <summary> Initializes a new instance of the <see cref="CustomComboCache"/> class. </summary>
        public CustomComboCache() => Service.Framework.Update += Framework_Update;

        private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

        /// <inheritdoc/>
        public void Dispose() => Service.Framework.Update -= Framework_Update;

        /// <summary> Gets a job gauge. </summary>
        /// <typeparam name="T"> Type of job gauge. </typeparam>
        /// <returns> The job gauge. </returns>
        internal T GetJobGauge<T>() where T : JobGaugeBase
        {
            if (!jobGaugeCache.TryGetValue(typeof(T), out JobGaugeBase? gauge))
                gauge = jobGaugeCache[typeof(T)] = Service.JobGauges.Get<T>();

            return (T)gauge;
        }

        /// <summary> Finds a status on the given object. </summary>
        /// <param name="statusID"> Status effect ID. </param>
        /// <param name="obj"> Object to look for effects on. </param>
        /// <param name="sourceID"> Source object ID. </param>
        /// <returns> Status object or null. </returns>
        internal DalamudStatus.Status? GetStatus(uint statusID, IGameObject? obj, ulong? sourceID)
        {
            var key = (statusID, obj?.GameObjectId, sourceID);
            if (statusCache.TryGetValue(key, out DalamudStatus.Status? found))
                return found;

            if (obj is null)
                return statusCache[key] = null;

            if (obj is not IBattleChara chara)
                return statusCache[key] = null;

            foreach (DalamudStatus.Status? status in chara.StatusList)
            {
                if (status.StatusId == statusID && (!sourceID.HasValue || status.SourceId == 0 || status.SourceId == InvalidObjectID || status.SourceId == sourceID))
                    return statusCache[key] = status;
            }

            return statusCache[key] = null;
        }

        /// <summary> Gets the cooldown data for an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Cooldown data. </returns>
        internal unsafe CooldownData GetCooldown(uint actionID)
        {
            if (cooldownCache.TryGetValue(actionID, out CooldownData found))
                return found;

            ActionManager* actionManager = ActionManager.Instance();
            if (actionManager == null)
                return cooldownCache[actionID] = default;

            CooldownData data = new()
            {
                ActionID = actionID,
            };

            return cooldownCache[actionID] = data;  
        }

        /// <summary> Get the maximum number of charges for an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Max number of charges at current level. </returns>
        internal unsafe ushort GetMaxCharges(uint actionID) => GetCooldown(actionID).MaxCharges;

        /// <summary> Get the resource cost of an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Returns the resource cost of an action. </returns>
        internal static unsafe int GetResourceCost(uint actionID)
        {
            ActionManager* actionManager = ActionManager.Instance();
            if (actionManager == null)
                return 0;

            int cost = ActionManager.GetActionCost(ActionType.Action, actionID, 0, 0, 0, 0);

            return cost;
        }

        /// <summary> Get the cooldown group of an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        private byte GetCooldownGroup(uint actionID)
        {
            if (cooldownGroupCache.TryGetValue(actionID, out byte cooldownGroup))
                return cooldownGroup;

            var sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
            var row = sheet.GetRow(actionID);

            return cooldownGroupCache[actionID] = row!.CooldownGroup;
        }

        /// <summary> Triggers when the game framework updates. Clears cooldown and status caches. </summary>
        private unsafe void Framework_Update(IFramework framework)
        {
            statusCache.Clear();
            cooldownCache.Clear();
        }
    }
}
