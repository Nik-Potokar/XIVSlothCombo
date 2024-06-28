using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class WARPvP
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

        public static class Config
        {
            public static UserInt
                WARPVP_BlotaTiming = new("WARPVP_BlotaTiming");

        }
        internal class WARPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WARPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is HeavySwing or Maim or StormsPath)
                {
                    var canWeave = CanWeave(actionID);

                    if (!GetCooldown(Bloodwhetting).IsCooldown && (IsEnabled(CustomComboPreset.WARPvP_BurstMode_Bloodwhetting) || canWeave))
                        return OriginalHook(Bloodwhetting);

                    if (!InMeleeRange() && IsOffCooldown(Blota) && !TargetHasEffectAny(PvPCommon.Debuffs.Stun) && IsEnabled(CustomComboPreset.WARPvP_BurstMode_Blota) && Config.WARPVP_BlotaTiming == 0 && IsOffCooldown(PrimalRend))
                        return OriginalHook(Blota);

                    if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_PrimalRend) && IsOffCooldown(PrimalRend))
                        return OriginalHook(PrimalRend);

                    if (!InMeleeRange() && IsOffCooldown(Blota) && !TargetHasEffectAny(PvPCommon.Debuffs.Stun) && IsEnabled(CustomComboPreset.WARPvP_BurstMode_Blota) && Config.WARPVP_BlotaTiming == 1 && IsOnCooldown(PrimalRend))
                        return OriginalHook(Blota);

                    if (!GetCooldown(Onslaught).IsCooldown && canWeave)
                        return OriginalHook(Onslaught);

                    if (InMeleeRange())
                    {
                        if (HasEffect(Buffs.NascentChaos))
                            return OriginalHook(Bloodwhetting);

                        if (!GetCooldown(Orogeny).IsCooldown && canWeave)
                            return OriginalHook(Orogeny);
                    }
                }
                return actionID;
            }
        }
    }
}