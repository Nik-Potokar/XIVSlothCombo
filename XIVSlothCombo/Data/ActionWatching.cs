using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Data
{
    public static class ActionWatching
    {
        internal static Dictionary<uint, Lumina.Excel.GeneratedSheets.Action> ActionSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!
            .Where(i => i.RowId is not 7)
            .ToDictionary(i => i.RowId, i => i);

        internal static Dictionary<uint, Lumina.Excel.GeneratedSheets.Status> StatusSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>()!
            .ToDictionary(i => i.RowId, i => i);

        private static readonly Dictionary<string, List<uint>> statusCache = new();

        public static List<uint> CombatActions = new List<uint>();

        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private readonly static Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;
        private static void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook!.Original(sourceObjectId, sourceActor, position, effectHeader, effectArray, effectTrail);
            TimeLastActionUsed = DateTime.Now;
            if (!CustomComboNS.Functions.CustomComboFunctions.InCombat()) CombatActions.Clear();
            ActionEffectHeader header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);

            if (ActionType is 13 or 2) return;
            if (header.ActionId != 7 &&
                header.ActionId != 8 &&
                sourceObjectId == Service.ClientState.LocalPlayer.ObjectId)
            {
                LastActionUseCount++;
                if (header.ActionId != LastAction)
                {
                    LastActionUseCount = 1;
                }

                LastAction = header.ActionId;

                ActionSheet.TryGetValue(header.ActionId, out var sheet);
                if (sheet != null)
                {
                    switch (sheet.ActionCategory.Value.Name)
                    {
                        case "Spell":
                            LastSpell = header.ActionId;
                            break;
                        case "Weaponskill":
                            LastWeaponskill = header.ActionId;
                            break;
                        case "Ability":
                            LastAbility = header.ActionId;
                            break;
                    }
                }

                CombatActions.Add(header.ActionId);

                if (Service.Configuration.EnabledOutputLog)
                    OutputLog();
            }
        }

        private delegate void SendActionDelegate(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
        private static readonly Hook<SendActionDelegate>? SendActionHook;
        private static void SendActionDetour(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
        {
            try
            {
                SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
                TimeLastActionUsed = DateTime.Now;
                ActionType = actionType;

                Dalamud.Logging.PluginLog.Debug($"{actionId} {sequence} {a5} {a6} {a7} {a8} {a9}");
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.Error(ex, "SendActionDetour");
                SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
            }
        }

        public static uint WhichOfTheseActionsWasLast(params uint[] actions)
        {
            if (CombatActions.Count == 0) return 0;

            int currentLastIndex = 0;
            foreach (var action in actions)
            {
                if (CombatActions.Any(x => x == action))
                {
                    int index = CombatActions.LastIndexOf(action);

                    if (index > currentLastIndex) currentLastIndex = index;
                }
            }

            return CombatActions[currentLastIndex];
        }

        public static int HowManyTimesUsedAfterAnotherAction(uint lastUsedIDToCheck, uint idToCheckAgainst)
        {
            if (CombatActions.Count < 2) return 0;
            if (WhichOfTheseActionsWasLast(lastUsedIDToCheck, idToCheckAgainst) != lastUsedIDToCheck) return 0;

            int startingIndex = CombatActions.LastIndexOf(idToCheckAgainst);
            if (startingIndex == -1) return 0;

            int count = 0;
            for (int i = startingIndex + 1; i < CombatActions.Count; i++)
            {
                if (CombatActions[i] == lastUsedIDToCheck) count++;
            }

            return count;
        }

        public static uint LastAction { get; set; } = 0;
        public static int LastActionUseCount { get; set; } = 0;
        public static uint ActionType { get; set; } = 0;
        public static uint LastWeaponskill { get; set; } = 0;
        public static uint LastAbility { get; set; } = 0;
        public static uint LastSpell { get; set; } = 0;

        public static TimeSpan TimeSinceLastAction => DateTime.Now - TimeLastActionUsed;

        private static DateTime TimeLastActionUsed { get; set; } = DateTime.Now;

        public static void OutputLog()
        {
            Service.ChatGui.Print($"You just used: {GetActionName(LastAction)} x{LastActionUseCount}");
        }

        public static void Dispose()
        {
            ReceiveActionEffectHook?.Dispose();
            SendActionHook?.Dispose();
        }

        static ActionWatching()
        {
            ReceiveActionEffectHook ??= Hook<ReceiveActionEffectDelegate>.FromAddress(Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), ReceiveActionEffectDetour);
            SendActionHook ??= Hook<SendActionDelegate>.FromAddress(Service.SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? F3 0F 10 3D ?? ?? ?? ?? 48 8D 4D BF"), SendActionDetour);
        }

        public static void Enable()
        {
            ReceiveActionEffectHook?.Enable();
            SendActionHook?.Enable();
        }

        public static void Disable()
        {
            ReceiveActionEffectHook.Disable();
            SendActionHook?.Disable();
        }

        public static int GetLevel(uint id) => ActionSheet.TryGetValue(id, out var action) ? action.ClassJobLevel : 0;
        public static string GetActionName(uint id) => ActionSheet.TryGetValue(id, out var action) ? (string)action.Name : "UNKNOWN ABILITY";
        public static string GetStatusName(uint id) => StatusSheet.TryGetValue(id, out var status) ? (string)status.Name : "Unknown Status";

        public static List<uint>? GetStatusesByName(string status)
        {
            if (statusCache.TryGetValue(status, out List<uint>? list))
                return list;

            return statusCache.TryAdd(status, StatusSheet.Where(x => x.Value.Name.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Key).ToList())
                ? statusCache[status]
                : null;

        }

        public static ActionAttackType GetAttackType(uint id)
        {
            if (!ActionSheet.TryGetValue(id, out var action)) return ActionAttackType.Unknown;

            return action.ActionCategory.Value.Name.RawString switch
            {
                "Spell" => ActionAttackType.Spell,
                "Weaponskill" => ActionAttackType.Weaponskill,
                "Ability" => ActionAttackType.Ability,
                _ => ActionAttackType.Unknown
            };
        }

        public enum ActionAttackType
        {
            Ability,
            Spell,
            Weaponskill,
            Unknown
        }
    }

    internal unsafe static class ActionManagerHelper
    {
        private static readonly IntPtr actionMgrPtr;
        internal static IntPtr FpUseAction => (IntPtr)ActionManager.fpUseAction;
        internal static IntPtr FpUseActionLocation => (IntPtr)ActionManager.fpUseActionLocation;
        internal static IntPtr CheckActionResources => (IntPtr)ActionManager.fpCheckActionResources;
        public static ushort CurrentSeq => actionMgrPtr != IntPtr.Zero ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x110) : (ushort)0;
        public static ushort LastRecievedSeq => actionMgrPtr != IntPtr.Zero ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x112) : (ushort)0;
        public static bool IsCasting => actionMgrPtr != IntPtr.Zero && Marshal.ReadByte(actionMgrPtr + 0x28) != 0;
        public static uint CastingActionId => actionMgrPtr != IntPtr.Zero ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x24) : 0u;
        public static uint CastTargetObjectId => actionMgrPtr != IntPtr.Zero ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x38) : 0u;
        static ActionManagerHelper() => actionMgrPtr = (IntPtr)ActionManager.Instance();
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ActionEffectHeader
    {
        [FieldOffset(0x0)] public long TargetObjectId;
        [FieldOffset(0x8)] public uint ActionId;
        [FieldOffset(0x14)] public uint UnkObjectId;
        [FieldOffset(0x18)] public ushort Sequence;
        [FieldOffset(0x1A)] public ushort Unk_1A;
    }
}
