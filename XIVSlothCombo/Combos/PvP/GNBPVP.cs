using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class GNBPVP
    {

        public const uint
            KeenEdge = 29098,
            BrutalShell = 29099,
            SolidBarrel = 29100,
            DoubleDown = 29105,
            Continuation = 29106,
            RoughDivide = 29123,
            DrawAndJunction = 29124,
            //GnashingStuff
            GnashingFang = 29102,
            SavageClaw = 29103,
            WickedTalon = 29104;

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
                PowderBarrel = 3043;
        }

        internal class GNBPVP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPVP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {

                    if (IsEnabled(CustomComboPreset.GNBPVP_DoubleDown) && HasEffect(Buffs.NoMercy) && IsOffCooldown(DoubleDown))
                        return DoubleDown;

                    //BuffEffects
                    if (CanWeave(actionID))
                    {
                        if (HasEffect(Buffs.ReadyToBlast))
                            return OriginalHook(Continuation);
                        if (IsEnabled(CustomComboPreset.GNBPVP_DrawAndJunction) && IsOffCooldown(DrawAndJunction) && !HasEffect(Buffs.PowderBarrel) && !HasEffect(Buffs.ReadyToBlast))
                            return DrawAndJunction;
                    }

                }
                return actionID;
            }
        }
        internal class GNBPVP_GnashingFang : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPVP_GnashingFang;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is GnashingFang)
                {
                    if (CanWeave(actionID))
                    {
                        if (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge))
                            return OriginalHook(Continuation);
                    }

                }
                return actionID;
            }
        }
    }
}