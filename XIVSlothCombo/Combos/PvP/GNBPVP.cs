using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class GNBPvP
    {

        public const uint
            KeenEdge = 29098,
            BrutalShell = 29099,
            SolidBarrel = 29100,
            GnashingFang = 29102,
            SavageClaw = 29103,
            WickedTalon = 29104,
            DoubleDown = 29105,
            Continuation = 29106,
            JugularRip = 29108,
            AbdomenTear = 29109,
            EyeGouge = 29110,
            RoughDivide = 29123,
            DrawAndJunction = 29124,
            Guard = 29054,
            JunctionedCast = 29125;

        internal class Debuffs
        {
            internal const ushort
                Stun = 1343;
        }

        internal class Buffs
        {
            internal const ushort
                ReadyToRip = 2002,
                ReadyToTear = 2003,
                ReadyToGouge = 2004,
                ReadyToBlast = 3041,
                NoMercy = 3042,
                PowderBarrel = 3043,
                Guard = 3054,
                JunctionTank = 3044,
                JunctionDPS = 3045,
                JunctionHealer = 3046;
        }

        internal class GNBPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {
                    // Buff Effects
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_Continuation) &&
                            (HasEffect(Buffs.ReadyToBlast) || HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);

                        if ((IsEnabled(CustomComboPreset.GNBPvP_JunctionDPS) && HasEffect(Buffs.JunctionDPS) && HasEffect(Buffs.NoMercy) && IsOffCooldown(JunctionedCast)) ||
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionHealer) && HasEffect(Buffs.JunctionHealer) && IsOffCooldown(JunctionedCast)) ||
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionTank) && HasEffect(Buffs.JunctionTank) && IsOffCooldown(JunctionedCast)))
                            return OriginalHook(JunctionedCast);

                        if (IsEnabled(CustomComboPreset.GNBPvP_DrawAndJunction) && IsOffCooldown(DrawAndJunction) && !HasEffect(Buffs.PowderBarrel) && !HasEffect(Buffs.ReadyToBlast))
                            return DrawAndJunction;

                        if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide) && HasEffect(Buffs.NoMercy) &&
                            GetBuffRemainingTime(Buffs.NoMercy) <= 1.5f && GetBuffRemainingTime(Buffs.NoMercy) > 0 &&
                            GetRemainingCharges(RoughDivide) == 1)
                            return RoughDivide;
                    }

                    if (IsOffCooldown(DoubleDown) &&
                        GetRemainingCharges(RoughDivide) > 1)
                        return RoughDivide;

                    // Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNBPvP_DoubleDown) && HasEffect(Buffs.NoMercy) && IsOffCooldown(DoubleDown) && InMeleeRange() && !TargetHasEffect(Buffs.Guard))
                        return DoubleDown;

                    if ((IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) && IsOffCooldown(GnashingFang) && !TargetHasEffect(Buffs.Guard) && HasEffect(Buffs.NoMercy)) ||
                        WasLastWeaponskill(GnashingFang) || WasLastWeaponskill(JugularRip) || WasLastWeaponskill(SavageClaw))
                        return OriginalHook(GnashingFang);
                }

                return actionID;
            }
        }
        internal class GNBPvP_GnashingFang : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_GnashingFang;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is GnashingFang &&
                    CanWeave(actionID) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge))
                    ? OriginalHook(Continuation)
                    : actionID;
        }
    }
}