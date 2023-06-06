using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class ASTPvP
    {
        internal const byte JobID = 33;

        internal const uint
            Malefic = 29242,
            AspectedBenefic = 29243,
            Gravity = 29244,
            DoubleCast = 29245,
            DoubleMalefic = 29246,
            NocturnalBenefic = 29247,
            DoubleGravity = 29248,
            Draw = 29249,
            Macrocosmos = 29253,
            Microcosmos = 29254;

        internal class Buffs
        {
            internal const ushort
                BalanceDrawn = 3101,
                BoleDrawn = 3403,
                ArrowDrawn = 3404,
                Arrow = 3402,
                Balance = 1338,
                Bole = 1339;
        }

        internal class ASTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Malefic)
                {
                    // Out of combat Draw
                    if (IsOffCooldown(Draw) && !InCombat() && IsEnabled(CustomComboPreset.ASTPvP_Card))
                        return Draw;

                    // Malefic to initiate combat
                    if (!InCombat() &&
                        (IsOffCooldown(Draw) || HasEffect(Buffs.BoleDrawn) || HasEffect(Buffs.ArrowDrawn)))
                        return Malefic;

                    // Post-Draw Malefic
                    if (lastComboMove == Draw && !CanWeave(actionID))
                        return Malefic;

                    // Play "The Balance" before a Gravity/Double + Macro burst
                    if (HasCharges(DoubleCast) && IsOffCooldown(Gravity) && IsOffCooldown(Macrocosmos) && HasEffect(Buffs.BalanceDrawn) && IsEnabled(CustomComboPreset.ASTPvP_Card))
                        return OriginalHook(Draw);

                    if (!TargetHasEffectAny(PvPCommon.Buffs.Guard))
                    {
                        if (lastComboMove == DoubleGravity && IsOffCooldown(Macrocosmos))
                            return Macrocosmos;

                        if (lastComboMove == Gravity && HasCharges(DoubleCast) && IsEnabled(CustomComboPreset.ASTPvP_DoubleCast))
                            return DoubleGravity;

                        if (IsOffCooldown(Gravity))
                            return Gravity;

                        if (lastComboMove == Malefic && (GetRemainingCharges(DoubleCast) > 1 ||
                            GetCooldownRemainingTime(Gravity) > 7.5f) && CanWeave(actionID) && IsEnabled(CustomComboPreset.ASTPvP_DoubleCast))
                            return DoubleMalefic;
                    }

                    // Card waste prevention
                    if (((GetBuffRemainingTime(Buffs.BalanceDrawn) < 3) ||
                        (GetBuffRemainingTime(Buffs.BoleDrawn) < 3) ||
                        (GetBuffRemainingTime(Buffs.ArrowDrawn) < 3)) &&
                        CanWeave(actionID) && IsEnabled(CustomComboPreset.ASTPvP_Card))
                        return OriginalHook(Draw);

                    // Generic Draw
                    if (IsOffCooldown(Draw) && CanWeave(actionID) && IsEnabled(CustomComboPreset.ASTPvP_Card))
                        return Draw;

                    // Generic Play outside of necessary holding
                    if (CanWeave(actionID) && GetCooldownRemainingTime(Macrocosmos) > 7.5f && (IsOffCooldown(Draw) ||
                        HasEffect(Buffs.BalanceDrawn) || HasEffect(Buffs.BoleDrawn) || HasEffect(Buffs.ArrowDrawn)) && IsEnabled(CustomComboPreset.ASTPvP_Card))
                        return OriginalHook(Draw);
                }

                return actionID;
            }

            internal class ASTPvP_Heal : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Heal;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID is AspectedBenefic && CanWeave(actionID) &&
                        lastComboMove == AspectedBenefic &&
                        HasCharges(DoubleCast))
                        return OriginalHook(DoubleCast);

                    return actionID;
                }
            }
        }
    }
}