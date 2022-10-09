using Microsoft.VisualBasic;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class ASTPVP
    {
        public const byte JobID = 33;

        public const uint
            Malefic = 29242,
            AspectedBenefic = 29243,
            Gravity = 29244,
            DoubleCast = 29245,
            DoubleMalefic = 29246,
            DoubleAspectedBenefic = 29247,
            DoubleGravity = 29248,
            Draw = 29249,
            Macrocosmos = 29253;

        internal class Buffs
        {
            internal const ushort
                BalanceDrawn = 3101,
                BoleDrawn = 3403,
                ArrowDrawn = 3404;
            
        }

        internal class ASTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Malefic)
                {
                    if (IsOffCooldown(Gravity))
                        return Gravity;

                    if (CanWeave(actionID))
                    {         
                        if (IsEnabled(CustomComboPreset.ASTPvP_DoubleCast) && HasCharges(DoubleCast))
                            return OriginalHook(DoubleCast);
                    }
                }
                return actionID;
            }
        }
        internal class ASTPvP_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Heal;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is AspectedBenefic)
                {
                    if (CanWeave(actionID))
                    {
                        if (HasCharges(DoubleCast))
                            return OriginalHook(DoubleCast);
                    }
                }
                return actionID;
            }
        }
    }
}