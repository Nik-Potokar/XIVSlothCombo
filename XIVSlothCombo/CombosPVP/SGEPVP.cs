using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class SGEPVP
    {
        internal const uint
            Dosis = 29256,
            Phlegma = 29259,
            Pneuma = 29260,
            Eukrasia = 29258,
            Icarus = 29261,
            Toxikon = 29262,
            Kardia = 29264,
            EukrasianDosis = 29257,
            Toxicon2 = 29263;

        internal class Debuffs
        {
            internal const ushort
                EukrasianDosis = 3108,
                Toxicon = 3113;
        }

        internal class Buffs
        {
            internal const ushort
                Kardia = 2871,
                Kardion = 2872,
                Eukrasia = 3107,
                Addersting = 3115,
                Haima = 3110,
                Haimatinon = 3111;
        }

    }


    internal class SGEBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGEBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == SGEPVP.Dosis)
            {
                //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                //if (globalAction != actionID) return globalAction;

                if (!HasEffectAny(SGEPVP.Buffs.Kardia))
                    return SGEPVP.Kardia;

                if (!GetCooldown(SGEPVP.Pneuma).IsCooldown)
                    return SGEPVP.Pneuma;

                if (InMeleeRange() && !HasEffect(SGEPVP.Buffs.Eukrasia) && GetCooldown(SGEPVP.Phlegma).RemainingCharges > 0)
                    return SGEPVP.Phlegma;

                if (HasEffect(SGEPVP.Buffs.Addersting) && !HasEffect(SGEPVP.Buffs.Eukrasia))
                    return SGEPVP.Toxicon2;

                if (!TargetHasEffectAny(SGEPVP.Debuffs.EukrasianDosis) && GetCooldown(SGEPVP.Eukrasia).RemainingCharges > 0 && !HasEffect(SGEPVP.Buffs.Eukrasia))
                    return SGEPVP.Eukrasia;

                if (HasEffect(SGEPVP.Buffs.Eukrasia))
                    return OriginalHook(SGEPVP.Dosis);

                if (!TargetHasEffect(SGEPVP.Debuffs.Toxicon) && GetCooldown(SGEPVP.Toxikon).RemainingCharges > 0)
                    return OriginalHook(SGEPVP.Toxikon);

            }
            return actionID;
        }
    }
}
