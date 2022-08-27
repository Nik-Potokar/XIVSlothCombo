using System.Numerics;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        public Vector2 Position { get; set; }
        public float PlayerSpeed { get; set; }
        public uint MovingCounter { get; set; }
        public static bool IsMoving { get; set; }

        /// <summary> Checks player movement </summary>
        public unsafe void CheckMovement()
        {
            IsMoving = FFXIVClientStructs.FFXIV.Client.UI.Agent.AgentMap.Instance()->IsPlayerMoving > 0;
        }
    }
}
