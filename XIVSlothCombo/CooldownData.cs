using System.Runtime.InteropServices;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Internal cooldown data.
    /// </summary>
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
        private readonly float cooldownTotal;

        /// <summary>
        /// Gets a value indicating whether the action is on cooldown.
        /// </summary>
        public bool IsCooldown
        {
            get
            {
                var (cur, max) = Service.ComboCache.GetMaxCharges(this.ActionID);
                if (cur == max)
                    return this.isCooldown;

                return this.cooldownElapsed < this.CooldownTotal;
            }
        }

        /// <summary>
        /// Gets the action ID on cooldown.
        /// </summary>
        public uint ActionID => this.actionID;

        /// <summary>
        /// Gets the elapsed cooldown time.
        /// </summary>
        public float CooldownElapsed
        {
            get
            {
                if (this.cooldownElapsed == 0)
                    return 0;

                if (this.cooldownElapsed > this.CooldownTotal)
                    return 0;

                return this.cooldownElapsed;
            }
        }

        /// <summary>
        /// Gets the total cooldown time.
        /// </summary>
        public float CooldownTotal
        {
            get
            {
                if (this.cooldownTotal == 0)
                    return 0;

                var (cur, max) = Service.ComboCache.GetMaxCharges(this.ActionID);
                if (cur == max)
                    return this.cooldownTotal;

                // Rebase to the current charge count
                var total = this.cooldownTotal / max * cur;

                if (this.cooldownElapsed > total)
                    return 0;

                return total;
            }
        }

        /// <summary>
        /// Gets the cooldown time remaining.
        /// </summary>
        /// 
        public float CooldownRemaining => this.IsCooldown ? this.CooldownTotal - this.CooldownElapsed : 0;


        /// <summary>
        /// Gets the maximum number of charges for an action at the current level.
        /// </summary>
        /// <returns>Number of charges.</returns>
        public ushort MaxCharges => Service.ComboCache.GetMaxCharges(this.ActionID).Current;

        /// <summary>
        /// Gets a value indicating whether the action has charges, not charges available.
        /// </summary>
        public bool HasCharges => this.MaxCharges > 1;

        /// <summary>
        /// Gets the remaining number of charges for an action.
        /// </summary>
        public ushort RemainingCharges
        {
            get
            {
                var (cur, _) = Service.ComboCache.GetMaxCharges(this.ActionID);

                if (!this.IsCooldown)
                    return cur;

                return (ushort)(this.CooldownElapsed / (this.CooldownTotal / this.MaxCharges));
            }
        }

        /// <summary>
        /// Gets the cooldown time remaining until the next charge.
        /// </summary>
        public float ChargeCooldownRemaining
        {
            get
            {
                if (!this.IsCooldown)
                    return 0;

                var (cur, _) = Service.ComboCache.GetMaxCharges(this.ActionID);

                return this.CooldownRemaining % (this.CooldownTotal / cur);
            }
        }
    }
}
