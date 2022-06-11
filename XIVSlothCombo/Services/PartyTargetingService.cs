using System;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace XIVSlothComboPlugin
{
    public static unsafe class PartyTargetingService
    {
        private static readonly IntPtr pronounModule = (IntPtr)Framework.Instance()->GetUiModule()->GetPronounModule();
        public static GameObject* UITarget => (GameObject*)*(IntPtr*)(pronounModule + 0x290);

        public static long GetObjectID(GameObject* o)
        {
            var id = o->GetObjectID();
            return (id.Type * 0x1_0000_0000) | id.ObjectID;
        }

        private static readonly delegate* unmanaged<long, GameObject*> getGameObjectFromObjectID = (delegate* unmanaged<long, GameObject*>)Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 44 0F B6 C3 48 8B D0");
        public static GameObject* GetGameObjectFromObjectID(long id) => getGameObjectFromObjectID(id);

        private static readonly delegate* unmanaged<IntPtr, uint, GameObject*> getGameObjectFromPronounID = (delegate* unmanaged<IntPtr, uint, GameObject*>)Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 0F 85 ?? ?? ?? ?? 8D 4F DD");
        public static GameObject* GetGameObjectFromPronounID(uint id) => getGameObjectFromPronounID(pronounModule, id);
    }
}
