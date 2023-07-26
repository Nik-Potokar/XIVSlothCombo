using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class DRG
    {
        internal static readonly List<uint> FastLocks = new()
        {
            PvE.DRG.BattleLitany,
            PvE.DRG.LanceCharge,
            PvE.DRG.DragonSight,
            PvE.DRG.LifeSurge,
            PvE.DRG.Geirskogul,
            PvE.DRG.Nastrond,
            PvE.DRG.MirageDive,
            PvE.DRG.WyrmwindThrust
        };

        internal static readonly List<uint> MidLocks = new()
        {
            PvE.DRG.Jump,
            PvE.DRG.HighJump,
            PvE.DRG.DragonfireDive,
            PvE.DRG.SpineshatterDive
        };

        internal static uint SlowLock => PvE.DRG.Stardiver;

        internal static bool CanDRGWeave(uint oGCD)
        {
            //GCD Ready - No Weave
            if (CustomComboFunctions.IsOffCooldown(PvE.DRG.TrueThrust))
                return false;

            var gcdTimer = CustomComboFunctions.GetCooldownRemainingTime(PvE.DRG.TrueThrust);

            if (FastLocks.Any(x => x == oGCD) && gcdTimer >= 0.6f)
                return true;

            if (MidLocks.Any(x => x == oGCD) && gcdTimer >= 0.8f)
                return true;

            if (SlowLock == oGCD && gcdTimer >= 1.5f)
                return true;

            return false;
        }
    }
}
