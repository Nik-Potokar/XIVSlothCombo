using System.Runtime.InteropServices;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Data
{
    /// <summary> Internal cooldown data. </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct CooldownData
    {
        [FieldOffset(0x0)]
        private readonly bool isCooldown;

        [FieldOffset(0x4)]
        private readonly uint actionID;

        [FieldOffset(0x8)]
        private readonly float cooldownElapsed;

        [FieldOffset(0xC)]
        private float cooldownTotal;

        /// <summary> Gets a value indicating whether the action is on cooldown. </summary>
        public readonly bool IsCooldown
        {
            get
            {
                var (cur, max) = Service.ComboCache.GetMaxCharges(ActionID);
                return cur == max
                    ? isCooldown
                    : cooldownElapsed < CooldownTotal;
            }
        }

        /// <summary> Gets the action ID on cooldown. </summary>
        public readonly uint ActionID => actionID;

        /// <summary> Gets the elapsed cooldown time. </summary>
        public readonly float CooldownElapsed
        {
            get
            {
                if (cooldownElapsed == 0)
                    return 0;

                if (cooldownElapsed > CooldownTotal)
                    return 0;

                return cooldownElapsed;
            }
        }

        /// <summary> Gets the total cooldown time. </summary>
        public float CooldownTotal
        {
            readonly get
            {
                if (cooldownTotal == 0)
                    return 0;

                var (cur, max) = Service.ComboCache.GetMaxCharges(ActionID);

                if (cur == max)
                    return cooldownTotal;

                // Rebase to the current charge count
                float total = cooldownTotal / max * cur;

                return cooldownElapsed > total
                    ? 0
                    : total;
            }
            set
            {
                cooldownTotal = value;
            }
        }

        /// <summary> Gets the cooldown time remaining. </summary>
        public readonly float CooldownRemaining => IsCooldown ? CooldownTotal - CooldownElapsed : 0;

        /// <summary> Gets the maximum number of charges for an action at the current level. </summary>
        /// <returns> Number of charges. </returns>
        public readonly ushort MaxCharges => Service.ComboCache.GetMaxCharges(ActionID).Current;

        /// <summary> Gets a value indicating whether the action has charges, not charges available. </summary>
        public readonly bool HasCharges => MaxCharges > 1;

        /// <summary> Gets the remaining number of charges for an action. </summary>
        public readonly ushort RemainingCharges
        {
            get
            {
                var (cur, _) = Service.ComboCache.GetMaxCharges(ActionID);

                return !IsCooldown
                    ? cur
                    : (ushort)(CooldownElapsed / (CooldownTotal / MaxCharges));
            }
        }

        /// <summary> Gets the cooldown time remaining until the next charge. </summary>
        public readonly float ChargeCooldownRemaining
        {
            get
            {
                if (!IsCooldown)
                    return 0;

                var (cur, _) = Service.ComboCache.GetMaxCharges(ActionID);

                return CooldownRemaining % (CooldownTotal / cur);
            }
        }
    }
}
