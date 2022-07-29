using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Hooking;
using Dalamud.Logging;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Core
{
    /// <summary> This class facilitates icon replacement. </summary>
    internal sealed partial class IconReplacer : IDisposable
    {
        private readonly List<CustomCombo> customCombos;
        private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;
        private readonly Hook<GetIconDelegate> getIconHook;

        private IntPtr actionManager = IntPtr.Zero;
        private readonly IntPtr module = IntPtr.Zero;

        /// <summary> Initializes a new instance of the <see cref="IconReplacer"/> class. </summary>
        public IconReplacer()
        {
            customCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                .Where(t => !t.IsAbstract && t.BaseType == typeof(CustomCombo))
                .Select(t => Activator.CreateInstance(t))
                .Cast<CustomCombo>()
                .OrderByDescending(x => x.Preset)
                .ToList();

            getIconHook = Hook<GetIconDelegate>.FromAddress(Service.Address.GetAdjustedActionId, GetIconDetour);
            isIconReplaceableHook = Hook<IsIconReplaceableDelegate>.FromAddress(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);

            getIconHook.Enable();
            isIconReplaceableHook.Enable();
        }

        private delegate ulong IsIconReplaceableDelegate(uint actionID);

        private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

        /// <inheritdoc/>
        public void Dispose()
        {
            getIconHook?.Dispose();
            isIconReplaceableHook?.Dispose();
        }

        /// <summary> Calls the original hook. </summary>
        /// <param name="actionID"> Action ID. </param>
        /// <returns> The result from the hook. </returns>
        internal uint OriginalHook(uint actionID) => getIconHook.Original(actionManager, actionID);

        private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
        {
            this.actionManager = actionManager;

            try
            {
                if (Service.ClientState.LocalPlayer == null)
                    return OriginalHook(actionID);

                if (ClassLocked()) return OriginalHook(actionID);

                uint lastComboMove = *(uint*)Service.Address.LastComboMove;
                float comboTime = *(float*)Service.Address.ComboTimer;
                byte level = Service.ClientState.LocalPlayer?.Level ?? 0;

                BlueMageService.PopulateBLUSpells();

                foreach (CustomCombo? combo in customCombos)
                {
                    if (combo.TryInvoke(actionID, level, lastComboMove, comboTime, out uint newActionID))
                        return newActionID;
                }

                return OriginalHook(actionID);
            }

            catch (Exception ex)
            {
                PluginLog.Error(ex, "Preset error");
                return OriginalHook(actionID);
            }
        }

        // Class locking
        private static bool ClassLocked()
        {
            if (Service.ClientState.LocalPlayer.Level <= 35) Service.ClassLocked = false;

            if (Service.ClientState.LocalPlayer.ClassJob.Id is
                (>= 8 and <= 25) or 27 or 28 or >= 30)
                Service.ClassLocked = false;

            if ((Service.ClientState.LocalPlayer.ClassJob.Id is 1 or 2 or 3 or 4 or 5 or 6 or 7 or 26 or 29) &&
                !Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty] &&
                Service.ClientState.LocalPlayer.Level > 35) Service.ClassLocked = true;

            return Service.ClassLocked;
        }

        private ulong IsIconReplaceableDetour(uint actionID) => 1;
    }
}