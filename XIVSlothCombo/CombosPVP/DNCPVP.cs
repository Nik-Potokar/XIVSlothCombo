using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class DNCPVP
    {
        public const byte JobID = 38;

        internal const uint
            FountainCombo = 54,
            Cascade = 29416,
            Fountain = 29417,
            ReverseCascade = 29418,
            Fountainfall = 29419,
            SaberDance = 29420,
            StarfallDance = 29421,
            HoningDance = 29422,
            HoningOvation = 29470,
            FanDance = 29428,
            CuringWaltz = 29429,
            EnAvant = 29430,
            ClosedPosition = 29431,
            Contradance = 29432;

        internal class Buffs
        {
            internal const ushort
                EnAvant = 2048,
                FanDance = 2052,
                Bladecatcher = 3159,
                FlourishingSaberDance = 3160,
                StarfallDance = 3161,
                HoningDance = 3162,
                Acclaim = 3163,
                HoningOvation = 3164;
        }
        public static class Config
        {
            public const string
                DNCWaltzThreshold = "DNCWaltzThreshold";
        }
    }

    internal class DNCBurstMode : CustomCombo // Burst Mode
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNCBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // Start by replacing the base actions of the combo, NOT the "combo" itself in-game!
            if (actionID is DNCPVP.Cascade or DNCPVP.Fountain or DNCPVP.ReverseCascade or DNCPVP.Fountainfall)
            {
                bool starfallDanceReady = !GetCooldown(DNCPVP.StarfallDance).IsCooldown;
                bool starfallDance = HasEffect(DNCPVP.Buffs.StarfallDance);
                bool enAvant = HasEffect(DNCPVP.Buffs.EnAvant);
                var enAvantCharges = GetCooldown(DNCPVP.EnAvant).RemainingCharges;
                bool curingWaltzReady = !GetCooldown(DNCPVP.CuringWaltz).IsCooldown;
                bool honingDanceReady = !GetCooldown(DNCPVP.HoningDance).IsCooldown;
                var acclaimStacks = GetBuffStacks(DNCPVP.Buffs.Acclaim);
                bool canWeave = CanWeave(actionID);
                var distance = GetTargetDistance();
                var HPThreshold = Service.Configuration.GetCustomIntValue(DNCPVP.Config.DNCWaltzThreshold);
                var HP = PlayerHealthPercentageHp();

                if (IsEnabled(CustomComboPreset.DNCHoningDanceOption) && honingDanceReady && HasTarget() && distance <= 5)
                {
                    if (HasEffect(DNCPVP.Buffs.Acclaim) && acclaimStacks < 4)
                        return WHM.Assize;

                    return DNCPVP.HoningDance;
                }

                if (canWeave)
                {
                    // Curing Waltz Burst Option
                    if (IsEnabled(CustomComboPreset.DNCCuringWaltzOption) && curingWaltzReady && HP <= HPThreshold) // Add slider to this next
                        return OriginalHook(DNCPVP.CuringWaltz);

                    // Fan Dance weaved on Main Combo
                    if (IsOffCooldown(DNCPVP.FanDance) && distance < 13) // 2y below max to avoid waste?
                        return OriginalHook(DNCPVP.FanDance);
                }

                // Starfall Dance on Main Combo
                if (!starfallDance && starfallDanceReady && distance < 20) // 5y below max to avoid waste?
                    return OriginalHook(DNCPVP.StarfallDance);

                /*
                // En Avant Overcap Protection Option
                if (IsEnabled(CustomComboPreset.DNCEnAvantOvercapOption) && !enAvant && enAvantCharges == 4) // Probably just remove this altogether it's stupid. Good to test with though, I guess?
                    return OriginalHook(DNCPVP.EnAvant);
                */

            }

            return actionID;
        }
    }
}
