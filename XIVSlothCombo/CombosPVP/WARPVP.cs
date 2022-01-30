using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class WARPVP
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;

        public const uint
            HeavySwing = 8758,
            Maim = 8761,
            StormsPath = 8762,
            FellCleave = 8763,
            Decimate = 17695,
            MythrilTempest = 18904,
            SteelCyclone = 18905;
 

        public static class Buffs
        {
            public const ushort
                InnerRelease = 1303,
                NascentChaos = 1992;

        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                StormsPath = 26,
                MythrilTempest = 40,
                StormsEye = 50,
                FellCleave = 54,
                Decimate = 60,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80,
                PrimalRend = 90;
        }
    }
    internal class StormsPathComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.StormsPathComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WARPVP.HeavySwing || actionID == WARPVP.Maim || actionID == WARPVP.StormsPath)
            {
                var gauge = GetJobGauge<WARGauge>();
                if (gauge.BeastGauge >= 50 || HasEffect(WARPVP.Buffs.InnerRelease) || HasEffect(WARPVP.Buffs.NascentChaos))
                {
                    return OriginalHook(WARPVP.FellCleave);
                }
            }

            return OriginalHook(actionID);
        }
    }
    internal class SteelCycloneFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SteelCycloneFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WARPVP.SteelCyclone || actionID == WARPVP.MythrilTempest)
            {
                var gauge = GetJobGauge<WARGauge>();
                if (gauge.BeastGauge >= 50 || HasEffect(WARPVP.Buffs.InnerRelease) || HasEffect(WARPVP.Buffs.NascentChaos))
                {
                    return OriginalHook(WARPVP.Decimate);
                }
            }

            return OriginalHook(actionID);
        }
    }

}
