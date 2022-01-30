using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class GNBPVP
    {
        public const byte JobID = 37;

        public const uint
            KeenEdge = 17703,
            BrutalShell = 17704,
            SolidBarrel = 17705,
            DemonSlice = 18910,
            DemonSlaughter = 18911,
            GnashingFang = 17706,
            SavageClaw = 17707,
            WickedTalon = 17708,
            FatedCircle = 17710,
            BurstStrike = 17709,
            JugularRip = 17712,
            AbdomenTear = 17713,
            EyeGouge = 17714,
            Continuation = 17711,
            Bowshock = 17748;



        public static class Buffs
        {
            public const short
                ReadyToRip = 2002,
                ReadyToTear = 2003,
                ReadyToGouge = 2004,
                ReadyToBlast = 2005;
        }

        public static class Debuffs
        {
            public const short
                BowShock = 1838,
                SonicBreak = 1837;
        }

        public static class Levels
        {
            public const byte
                BrutalShell = 4,
                SolidBarrel = 26,
                DemonSlaughter = 40,
                SonicBreak = 54,
                BowShock = 62,
                Continuation = 70,
                FatedCircle = 72,
                Bloodfest = 76,
                EnhancedContinuation = 86,
                CartridgeCharge3 = 88,
                DoubleDown = 90;
        }
    }


    internal class SolidBarrelComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SolidBarrelComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNBPVP.KeenEdge || actionID == GNBPVP.BrutalShell || actionID == GNBPVP.SolidBarrel)
            {
                var gauge = GetJobGauge<GNBGauge>();
                var bowshockCD = GetCooldown(GNBPVP.Bowshock);
                var actionIDCD = GetCooldown(actionID);
                if (InMeleeRange(true) && !bowshockCD.IsCooldown && actionIDCD.IsCooldown)
                    return GNBPVP.Bowshock;
                if (gauge.Ammo >= 2)
                    return GNBPVP.BurstStrike;

            }

            return actionID;
        }
    }
    internal class DemonSlaughterComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DemonSlaughterComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNBPVP.DemonSlaughter || actionID == GNBPVP.DemonSlice)
            {
                var gauge = GetJobGauge<GNBGauge>();
                var bowshockCD = GetCooldown(GNBPVP.Bowshock);
                var actionIDCD = GetCooldown(actionID);
                if (InMeleeRange(true) && !bowshockCD.IsCooldown && actionIDCD.IsCooldown)
                    return GNBPVP.Bowshock;
                if (gauge.Ammo >= 2)
                    return GNBPVP.FatedCircle;

            }

            return actionID;
        }
    }
}
