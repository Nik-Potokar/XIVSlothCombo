using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Runtime.InteropServices;

namespace XIVSlothComboPlugin
{
    public static class ActionWatching
    {
        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private readonly static Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;
        private static void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook!.Original(sourceObjectId, sourceActor, position, effectHeader, effectArray, effectTrail);
            var header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);

            if (ActionType is (13 or 2)) return;
            Dalamud.Logging.PluginLog.Debug(ActionType.ToString());
            if (header.ActionId != 7 &&
                header.ActionId != 8 &&
                sourceObjectId == Service.ClientState.LocalPlayer.ObjectId)
            {
                LastAbilityUseCount++;
                if (header.ActionId != LastAbility)
                {
                    LastAbilityUseCount = 1;
                }
                LastAbility = header.ActionId;

            }
        }

        private delegate void SendActionDelegate(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
        private static readonly Hook<SendActionDelegate>? SendActionHook;
        private static void SendActionDetour(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
        {
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
            ActionType = actionType;
        }

        public static uint LastAbility { get; set; } = 0;

        public static int LastAbilityUseCount { get; set; } = 0;

        public static uint ActionType { get; set; } = 0;

        public static void Dispose()
        {
            ReceiveActionEffectHook?.Dispose();
            SendActionHook?.Dispose();
        }

        static ActionWatching()
        {
            ReceiveActionEffectHook ??= new Hook<ReceiveActionEffectDelegate>(Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), ReceiveActionEffectDetour);
            SendActionHook ??= new Hook<SendActionDelegate>(Service.SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? F3 0F 10 3D ?? ?? ?? ?? 48 8D 4D BF"), SendActionDetour);
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

    }

    internal unsafe static class ActionManagerHelper
    {
        private static readonly IntPtr actionMgrPtr;

        internal static IntPtr FpUseAction =>
            (IntPtr)ActionManager.fpUseAction;
        internal static IntPtr FpUseActionLocation =>
            (IntPtr)ActionManager.fpUseActionLocation;

        public static ushort CurrentSeq => actionMgrPtr != IntPtr.Zero
            ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x110) : (ushort)0;
        public static ushort LastRecievedSeq => actionMgrPtr != IntPtr.Zero
            ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x112) : (ushort)0;

        public static bool IsCasting => actionMgrPtr != IntPtr.Zero
            && Marshal.ReadByte(actionMgrPtr + 0x28) != 0;
        public static uint CastingActionId => actionMgrPtr != IntPtr.Zero
            ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x24) : 0u;
        public static uint CastTargetObjectId => actionMgrPtr != IntPtr.Zero
            ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x38) : 0u;

        static ActionManagerHelper()
        {
            actionMgrPtr = (IntPtr)ActionManager.Instance();
        }
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
