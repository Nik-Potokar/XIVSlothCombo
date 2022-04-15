using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSlothComboPlugin;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class RDMPVP
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

    }

    internal class RedMageBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDMBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is RDMPVP.Verstone or RDMPVP.Verfire)
            {
                uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                if (globalAction != actionID) return globalAction;

                if (!GetCooldown(RDMPVP.Frazzle).IsCooldown && HasEffect(RDMPVP.Buffs.BlackShift))
                    return OriginalHook(RDMPVP.Frazzle);

                if (!GetCooldown(RDMPVP.Resolution).IsCooldown)
                    return OriginalHook(RDMPVP.Resolution);

                if (!InMeleeRange() && GetCooldown(RDMPVP.CorpsACorps).RemainingCharges > 0 && !GetCooldown(RDMPVP.EnchantedRiposte).IsCooldown)
                    return OriginalHook(RDMPVP.CorpsACorps);

                if (InMeleeRange() && !GetCooldown(RDMPVP.EnchantedRiposte).IsCooldown)
                    return OriginalHook(RDMPVP.EnchantedRiposte);

                if (InMeleeRange() && lastComboActionID == RDMPVP.EnchantedRiposte)
                    return OriginalHook(RDMPVP.EnchantedRiposte);

                if (InMeleeRange() && lastComboActionID == RDMPVP.EnchantedZwerchhau)
                    return OriginalHook(RDMPVP.EnchantedRiposte);

                if (InMeleeRange() && lastComboActionID == RDMPVP.EnchantedRedoublement && GetCooldown(RDMPVP.Displacement).RemainingCharges > 0)
                    return OriginalHook(RDMPVP.Displacement);

                if (HasEffect(RDMPVP.Buffs.VermilionRadiance))
                    return OriginalHook(RDMPVP.EnchantedRiposte);


                return OriginalHook(RDMPVP.Verstone);

            }

            return actionID;
        }
    }
}
