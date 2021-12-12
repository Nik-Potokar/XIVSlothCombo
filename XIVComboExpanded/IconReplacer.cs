using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// This class facilitates the icon replacing.
    /// </summary>
    internal sealed partial class IconReplacer : IDisposable
    {
        private readonly List<CustomCombo> customCombos;
        private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;
        private readonly Hook<GetIconDelegate> getIconHook;

        private IntPtr actionManager = IntPtr.Zero;
        private HashSet<uint> comboActionIDs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="IconReplacer"/> class.
        /// </summary>
        public IconReplacer()
        {
            this.customCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                .Where(t => t.BaseType == typeof(CustomCombo))
                .Select(t => Activator.CreateInstance(t))
                .Cast<CustomCombo>()
                .ToList();

            this.UpdateEnabledActionIDs();

            this.getActionCooldownSlot = Marshal.GetDelegateForFunctionPointer<GetActionCooldownSlotDelegate>(Service.Address.GetActionCooldown);

            this.getIconHook = new Hook<GetIconDelegate>(Service.Address.GetAdjustedActionId, this.GetIconDetour);
            this.isIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, this.IsIconReplaceableDetour);

            this.getIconHook.Enable();
            this.isIconReplaceableHook.Enable();
        }

        private delegate ulong IsIconReplaceableDelegate(uint actionID);

        private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

        /// <inheritdoc/>
        public void Dispose()
        {
            this.getIconHook.Dispose();
            this.isIconReplaceableHook.Dispose();
        }

        /// <summary>
        /// Update what action IDs are allowed to be modified. This pulls from <see cref="PluginConfiguration.EnabledActions"/>.
        /// </summary>
        internal void UpdateEnabledActionIDs()
        {
            this.comboActionIDs = Enum
                .GetValues<CustomComboPreset>()
                .Select(preset => preset.GetAttribute<CustomComboInfoAttribute>()!)
                .SelectMany(comboInfo => comboInfo.ActionIDs)
                .Concat(Service.Configuration.DancerDanceCompatActionIDs)
                .ToHashSet();
        }

        /// <summary>
        /// Calls the original hook.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <returns>The result from the hook.</returns>
        internal uint OriginalHook(uint actionID) => this.getIconHook.Original(this.actionManager, actionID);

        private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
        {
            this.actionManager = actionManager;

            try
            {
                var localPlayer = Service.ClientState.LocalPlayer;
                if (localPlayer == null || !this.comboActionIDs.Contains(actionID))
                    return this.OriginalHook(actionID);

                var lastComboMove = *(uint*)Service.Address.LastComboMove;
                var comboTime = *(float*)Service.Address.ComboTimer;
                var level = localPlayer.Level;

                foreach (var combo in this.customCombos)
                {
                    if (combo.TryInvoke(actionID, lastComboMove, comboTime, level, out var newActionID))
                        return newActionID;
                }

                return this.OriginalHook(actionID);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Don't crash the game");
                return this.OriginalHook(actionID);
            }
        }

        private ulong IsIconReplaceableDetour(uint actionID) => 1;
    }

    /// <summary>
    /// Cooldown getters.
    /// </summary>
    internal sealed partial class IconReplacer
    {
        private readonly Dictionary<uint, byte> cooldownGroupCache = new();
        private readonly GetActionCooldownSlotDelegate getActionCooldownSlot;

        private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

        /// <summary>
        /// Gets the cooldown data for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Cooldown data.</returns>
        internal CooldownData GetCooldown(uint actionID)
        {
            var cooldownGroup = this.GetCooldownGroup(actionID);
            if (this.actionManager == IntPtr.Zero)
                return new CooldownData() { ActionID = actionID };

            var cooldownPtr = this.getActionCooldownSlot(this.actionManager, cooldownGroup - 1);
            return Marshal.PtrToStructure<CooldownData>(cooldownPtr);
        }

        private byte GetCooldownGroup(uint actionID)
        {
            if (this.cooldownGroupCache.TryGetValue(actionID, out var cooldownGroup))
                return cooldownGroup;

            var sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
            var row = sheet.GetRow(actionID);

            return this.cooldownGroupCache[actionID] = row!.CooldownGroup;
        }

        /// <summary>
        /// Internal cooldown data.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        internal struct CooldownData
        {
            /// <summary>
            /// A value indicating whether the action is on cooldown.
            /// </summary>
            [FieldOffset(0x0)]
            public bool IsCooldown;

            /// <summary>
            /// Action ID on cooldown.
            /// </summary>
            [FieldOffset(0x4)]
            public uint ActionID;

            /// <summary>
            /// The elapsed cooldown time.
            /// </summary>
            [FieldOffset(0x8)]
            public float CooldownElapsed;

            /// <summary>
            /// The total cooldown time.
            /// </summary>
            [FieldOffset(0xC)]
            public float CooldownTotal;

            /// <summary>
            /// Gets the cooldown time remaining.
            /// </summary>
            public float CooldownRemaining => this.IsCooldown ? this.CooldownTotal - this.CooldownElapsed : 0;

        }
    }

}
