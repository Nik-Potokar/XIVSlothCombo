using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class GNBPVP
    {

        public const uint
            KeenEdge = 29098,
            BrutalShell = 29099,
            SolidBarrel = 29100,
            SavageClaw = 29103,
            WickedTalon = 29014,
            DoubleDown = 29105,
            Continuation = 29106,
            JugularRip = 29108,
            AbdomenTear = 29109,
            EyeGouge = 29110,
            RoughDivide = 29123,
            DrawAndJunction = 29124,
            JunctionedCast = 29125,
            //GnashingStuff
            GnashingFang = 29102;

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
                JunctionDPS = 3045,
                JunctionHealer = 3046,
                JunctionTank = 3044;
        }

        internal class GNBPVP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPVP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {
                    //BuffEffects
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.GNBPVP_ST_Continuation) && HasEffect(Buffs.ReadyToBlast) || HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge))
                            return OriginalHook(Continuation);
                        if (IsEnabled(CustomComboPreset.GNBPVP_JunctionDPS) && HasEffect(Buffs.JunctionDPS) && HasEffect(Buffs.NoMercy) && IsOffCooldown(JunctionedCast) || IsEnabled(CustomComboPreset.GNBPVP_JunctionHealer) && HasEffect(Buffs.JunctionHealer) && IsOffCooldown(JunctionedCast) || IsEnabled(CustomComboPreset.GNBPVP_JunctionTank) && HasEffect(Buffs.JunctionTank) && IsOffCooldown(JunctionedCast))
                            return OriginalHook(JunctionedCast);
                        if (IsEnabled(CustomComboPreset.GNBPVP_DrawAndJunction) && IsOffCooldown(DrawAndJunction) && !HasEffect(Buffs.PowderBarrel) && !HasEffect(Buffs.ReadyToBlast))
                            return DrawAndJunction;
                        if (IsEnabled(CustomComboPreset.GNBPVP_RoughDivide) && HasEffect(Buffs.NoMercy) && GetBuffRemainingTime(Buffs.NoMercy) <= 1.5f && GetBuffRemainingTime(Buffs.NoMercy) > 0 && GetRemainingCharges(RoughDivide) == 1)
                            return RoughDivide;


                    }

                    //Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNBPVP_DoubleDown) && HasEffect(Buffs.NoMercy) && IsOffCooldown(DoubleDown))
                        return DoubleDown;
                    if (IsEnabled(CustomComboPreset.GNBPVP_ST_GnashingFang) && IsOffCooldown(GnashingFang) && HasEffect(Buffs.NoMercy) || WasLastWeaponskill(GnashingFang) || WasLastWeaponskill(JugularRip) || WasLastWeaponskill(SavageClaw) || WasLastWeaponskill(WickedTalon))
                        return OriginalHook(GnashingFang);


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