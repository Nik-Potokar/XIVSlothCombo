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




        internal class SGEBurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGEBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == Dosis)
                {
                    //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    //if (globalAction != actionID) return globalAction;

                    if (!HasEffectAny(Buffs.Kardia))
                        return Kardia;

                    if (!GetCooldown(Pneuma).IsCooldown)
                        return Pneuma;

                    if (InMeleeRange() && !HasEffect(Buffs.Eukrasia) && GetCooldown(Phlegma).RemainingCharges > 0)
                        return Phlegma;

                    if (HasEffect(Buffs.Addersting) && !HasEffect(Buffs.Eukrasia))
                        return Toxicon2;

                    if (!TargetHasEffectAny(Debuffs.EukrasianDosis) && GetCooldown(Eukrasia).RemainingCharges > 0 && !HasEffect(Buffs.Eukrasia))
                        return Eukrasia;

                    if (HasEffect(Buffs.Eukrasia))
                        return OriginalHook(Dosis);

                    if (!TargetHasEffect(Debuffs.Toxicon) && GetCooldown(Toxikon).RemainingCharges > 0)
                        return OriginalHook(Toxikon);

                }
                return actionID;
            }
        }
    }
}
