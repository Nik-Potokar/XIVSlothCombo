using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class RDMPvP
    {
        public const byte JobID = 35;

        public const uint
            Verstone = 29683,
            EnchantedRiposte = 29689,
            Resolution = 29695,
            MagickBarrier = 29697,
            CorpsACorps = 29699,
            Displacement = 29700,
            Veraero3 = 29684,
            Verholy = 29685,
            Verfire = 29686,
            Verthunder3 = 29687,
            Verflare = 29688,
            EnchantedZwerchhau = 29690,
            EnchantedRedoublement = 29691,
            Frazzle = 29698,
            WhiteShift = 29703,
            BlackShift = 29702,
            SouthernCross = 29704;

        public static class Buffs
        {
            public const ushort
                WhiteShift = 3245,
                BlackShift = 3246,
                Dualcast = 1393,
                EnchantedRiposte = 3234,
                EnchantedRedoublement = 3236,
                EnchantedZwerchhau = 3235,
                VermilionRadiance = 3233,
                MagickBarrier = 3240;
        }

        internal class RDMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Verstone or Verfire)
                {

                    if (!GetCooldown(Frazzle).IsCooldown && HasEffect(Buffs.BlackShift) && IsNotEnabled(CustomComboPreset.RDMPvP_FrazzleOption))
                        return OriginalHook(Frazzle);

                    if (!GetCooldown(Resolution).IsCooldown)
                        return OriginalHook(Resolution);

                    if (!InMeleeRange() && GetCooldown(CorpsACorps).RemainingCharges > 0 && !GetCooldown(EnchantedRiposte).IsCooldown)
                        return OriginalHook(CorpsACorps);

                    if (InMeleeRange())
                    {
                        if (!GetCooldown(EnchantedRiposte).IsCooldown || lastComboActionID == EnchantedRiposte || lastComboActionID == EnchantedZwerchhau)
                            return OriginalHook(EnchantedRiposte);

                        if (lastComboActionID == EnchantedRedoublement && GetCooldown(Displacement).RemainingCharges > 0)
                            return OriginalHook(Displacement);
                    }

                    if (HasEffect(Buffs.VermilionRadiance))
                        return OriginalHook(EnchantedRiposte);

                    return OriginalHook(Verstone);
                }

                return actionID;
            }
        }
    }
}