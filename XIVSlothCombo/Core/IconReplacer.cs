using Dalamud.Hooking;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Extensions;
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

            getIconHook = Svc.Hook.HookFromAddress<GetIconDelegate>((nint)ActionManager.Addresses.GetAdjustedActionId.Value, GetIconDetour);
            isIconReplaceableHook = Svc.Hook.HookFromAddress<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);

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
                if (Svc.ClientState.LocalPlayer == null)
                    return OriginalHook(actionID);

                if (ClassLocked() ||
                    (DisabledJobsPVE.Any(x => x == Svc.ClientState.LocalPlayer.ClassJob.RowId) && !Svc.ClientState.IsPvP) ||
                    (DisabledJobsPVP.Any(x => x == Svc.ClientState.LocalPlayer.ClassJob.RowId) && Svc.ClientState.IsPvP))
                    return OriginalHook(actionID);

                uint lastComboMove = ActionManager.Instance()->Combo.Action;
                float comboTime = ActionManager.Instance()->Combo.Action != 0 ? ActionManager.Instance()->Combo.Timer : 0;
                byte level = Svc.ClientState.LocalPlayer?.Level ?? 0;

                foreach (CustomCombo? combo in customCombos)
                {
                    if (combo.TryInvoke(actionID, level, lastComboMove, comboTime, out uint newActionID))
                        return newActionID;
                    
                }

                return OriginalHook(actionID);
            }

            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Preset error");
                return OriginalHook(actionID);
            }
        }

        // Class locking
        public unsafe static bool ClassLocked()
        {
            if (Svc.ClientState.LocalPlayer is null) return false;

            if (Svc.ClientState.LocalPlayer.Level <= 35) return false;

            if (Svc.ClientState.LocalPlayer.ClassJob.RowId is
                (>= 8 and <= 25) or 27 or 28 or >= 30)
                return false;

            if (!UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(66049))
                return false;

            if ((Svc.ClientState.LocalPlayer.ClassJob.RowId is 1 or 2 or 3 or 4 or 5 or 6 or 7 or 26 or 29) &&
                Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty] &&
                Svc.ClientState.LocalPlayer.Level > 35) return true;

            return false;
        }

        private ulong IsIconReplaceableDetour(uint actionID) => 1;
    }
}