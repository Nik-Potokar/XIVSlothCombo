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
                DNCPvP_WaltzThreshold = "DNCPvP_WaltzThreshold";
        }

        internal class DNCPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNCPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade or Fountain or ReverseCascade or Fountainfall)
                {
                    bool starfallDanceReady = !GetCooldown(StarfallDance).IsCooldown;
                    bool starfallDance = HasEffect(Buffs.StarfallDance);
                    bool enAvant = HasEffect(Buffs.EnAvant);
                    var enAvantCharges = GetCooldown(EnAvant).RemainingCharges;
                    bool curingWaltzReady = !GetCooldown(CuringWaltz).IsCooldown;
                    bool honingDanceReady = !GetCooldown(HoningDance).IsCooldown;
                    var acclaimStacks = GetBuffStacks(Buffs.Acclaim);
                    bool canWeave = CanWeave(actionID);
                    var distance = GetTargetDistance();
                    var HPThreshold = Service.Configuration.GetCustomIntValue(Config.DNCPvP_WaltzThreshold);
                    var HP = PlayerHealthPercentageHp();

                    // Honing Dance Option
                    if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_HoningDance) && honingDanceReady && HasTarget() && distance <= 5)
                    {
                        if (HasEffect(Buffs.Acclaim) && acclaimStacks < 4)
                            return WHM.Assize;

                        return HoningDance;
                    }

                    if (canWeave)
                    {
                        // Curing Waltz Option
                        if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_CuringWaltz) && curingWaltzReady && HP <= HPThreshold)
                            return OriginalHook(CuringWaltz);

                        // Fan Dance weave
                        if (IsOffCooldown(FanDance) && distance < 13) // 2y below max to avoid waste
                            return OriginalHook(FanDance);
                    }

                    // Starfall Dance
                    if (!starfallDance && starfallDanceReady && distance < 20) // 5y below max to avoid waste
                        return OriginalHook(StarfallDance);
                }

                return actionID;
            }
        }
    }
}
