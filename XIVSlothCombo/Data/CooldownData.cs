using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Data
{
    internal class CooldownData
    {
        /// <summary> Gets a value indicating whether the action is on cooldown. </summary>
        public bool IsCooldown
        {
            get
            {
                return RemainingCharges == MaxCharges
                    ? false
                    : CooldownElapsed < CooldownTotal;
            }
        }

        /// <summary> Gets the action ID on cooldown. </summary>
        public uint ActionID;

        /// <summary> Gets the elapsed cooldown time. </summary>
        public unsafe float CooldownElapsed => ActionManager.Instance()->GetRecastGroupDetail(ActionManager.Instance()->GetRecastGroup(1, ActionID))->Elapsed;

        /// <summary> Gets the total cooldown time. </summary>
        public unsafe float CooldownTotal => (ActionManager.GetAdjustedRecastTime(ActionType.Action, ActionID) / 1000f) * MaxCharges;

        /// <summary> Gets the cooldown time remaining. </summary>
        public unsafe float CooldownRemaining => IsCooldown ? CooldownTotal - CooldownElapsed : 0;

        /// <summary> Gets the maximum number of charges for an action at the current level. </summary>
        /// <returns> Number of charges. </returns>
        public ushort MaxCharges => Service.ComboCache.GetMaxCharges(ActionID);

        /// <summary> Gets a value indicating whether the action has charges, not charges available. </summary>
        public bool HasCharges => MaxCharges > 1;

        /// <summary> Gets the remaining number of charges for an action. </summary>
        public unsafe uint RemainingCharges => ActionManager.Instance()->GetCurrentCharges(ActionID);

        /// <summary> Gets the cooldown time remaining until the next charge. </summary>
        public float ChargeCooldownRemaining
        {
            get
            {
                if (!IsCooldown)
                    return 0;

                var maxCharges = ActionManager.GetMaxCharges(ActionID, 100);
                var timePerCharge = CooldownTotal / maxCharges;

                return CooldownRemaining % (CooldownTotal / MaxCharges);
            }
        }
    }
}
