using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class Variant
    {
        public const uint
            VariantCure = 29729,
            VariantUltimatum = 29730,
            VariantRaise = 29731,
            VariantSpiritDart = 29732,
            VariantRampart = 29733,
            VariantRaise2 = 297334;

        public static class Buffs
        {
            public const ushort
                EmnityUp = 3358,
                VulnDown = 3360,
                DamageBarrier = 3405,
                Rehabilitation = 3367;

        }

        public static class Debuffs
        {
            public const ushort
                SustainedDamage = 3359;
        }

    }
}
