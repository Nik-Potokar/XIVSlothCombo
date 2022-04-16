using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class WARPVP
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        internal const uint
            HeavySwing = 29074,
            Maim = 29075,
            StormsPath = 29076,
            PrimalRend = 29084,
            Onslaught = 29079,
            Orogeny = 29080,
            Blota = 29081,
            Bloodwhetting = 29082;
            
        internal class Buffs
        {
            internal const ushort
                NascentChaos = 1992,
                InnerRelease = 1303;
        }
    }

    internal class WARBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WARBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == WARPVP.HeavySwing)
            {
                uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                if (globalAction != actionID) return globalAction;

                if (!GetCooldown(WARPVP.Bloodwhetting).IsCooldown)
                    return OriginalHook(WARPVP.Bloodwhetting);

                if (!InMeleeRange() && !GetCooldown(WARPVP.Blota).IsCooldown)
                    return OriginalHook(WARPVP.Blota);

                if (!GetCooldown(WARPVP.PrimalRend).IsCooldown)
                    return OriginalHook(WARPVP.PrimalRend);

                if (!GetCooldown(WARPVP.Onslaught).IsCooldown)
                    return OriginalHook(WARPVP.Onslaught);

                if (InMeleeRange())
                {
                    if (HasEffect(WARPVP.Buffs.NascentChaos))
                        return OriginalHook(WARPVP.Bloodwhetting);

                    if (!GetCooldown(WARPVP.Orogeny).IsCooldown)
                        return OriginalHook(WARPVP.Orogeny);

                    return OriginalHook(WARPVP.HeavySwing);
                }
            }

            return actionID;
        }
    }
}
