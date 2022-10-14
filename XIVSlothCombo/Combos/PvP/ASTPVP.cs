using XIVSlothCombo.CustomComboNS;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class ASTPVP
    {
        internal const byte JobID = 33;

        internal const uint
            Malefic = 29242,
            DiurnalBenefic = 29243,
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

        public static unsafe uint GetActionStatus(ActionType actionType, uint id) => ActionManager.Instance() is null ? uint.MaxValue : ActionManager.Instance()->GetActionStatus(ActionType.Spell, id);

        internal class ASTPVP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Malefic)
                {
                    if (GetActionStatus(ActionType.Spell, DoubleGravity) == 0)
                        return DoubleGravity;

                    if (CanWeave(actionID) && IsOffCooldown(Draw) || HasEffect(Buffs.BalanceDrawn) || HasEffect(Buffs.BoleDrawn) || HasEffect(Buffs.ArrowDrawn))
                        return OriginalHook(Draw);

                    if (IsOffCooldown(Gravity) && !TargetHasEffectAny(PvPCommon.Buffs.Guard))
                        return Gravity;
                }

                return actionID;
            }

            internal class ASTPvP_Heal : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Heal;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID is DiurnalBenefic)
                    {
                        if (CanWeave(actionID))
                        {
                            if (GetActionStatus(ActionType.Spell, NocturnalBenefic) == 0 && HasCharges(DoubleCast))
                                return OriginalHook(DoubleCast);
                        }
                    }
                    return actionID;
                }
            }
        }
    }
}