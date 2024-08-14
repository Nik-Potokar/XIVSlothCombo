using System;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class GNBPvP
    {

        public const uint
            KeenEdge = 29098,
            BrutalShell = 29099,
            SolidBarrel = 29100,
            BurstStrike = 29101,
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
                JunctionTank = 3044,
                JunctionDPS = 3045,
                JunctionHealer = 3046;
        }

        internal class GNBPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_Burst;
            
            float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.4 base in PvP
            bool enemyGuard = TargetHasEffect(PvPCommon.Buffs.Guard); //Guard check

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {
                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        //Continuation
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_Continuation) &&
                            (HasEffect(Buffs.ReadyToBlast) || HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);

                        //Draw&Junction buffs
                        if (!enemyGuard &&
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionDPS) && HasEffect(Buffs.JunctionDPS) && HasEffect(Buffs.NoMercy) && ActionReady(JunctionedCast)) ||
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionHealer) && HasEffect(Buffs.JunctionHealer) && ActionReady(JunctionedCast)) ||
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionTank) && HasEffect(Buffs.JunctionTank) && ActionReady(JunctionedCast)))
                            return OriginalHook(JunctionedCast);

                        //Draw&Junction
                        if (IsEnabled(CustomComboPreset.GNBPvP_DrawAndJunction) && ActionReady(DrawAndJunction) && !HasEffect(Buffs.PowderBarrel) && !HasEffect(Buffs.ReadyToBlast))
                            return DrawAndJunction;

                        //RoughDivide
                        if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide) && ActionReady(RoughDivide) && !HasEffect(Buffs.NoMercy) &&
                            (GetCooldownRemainingTime(DoubleDown) <= GCD || //Keeps 1 charge always for DoubleDown usage
                            GetCooldownRemainingTime(GnashingFang) <= GCD)) //Keeps 1 charge always for GnashingFang usage
                            return RoughDivide;
                    }

                    //RoughDivide overcap protection
                    if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide) && ActionReady(DoubleDown) && GetRemainingCharges(RoughDivide) == 2)
                        return RoughDivide;

                    //DoubleDown
                    if (IsEnabled(CustomComboPreset.GNBPvP_DoubleDown) && !enemyGuard && HasEffect(Buffs.NoMercy) && ActionReady(DoubleDown) && InMeleeRange() &&
                        (!JustUsed(GnashingFang) || !JustUsed(JugularRip) || !JustUsed(SavageClaw) && //Do not use when in GnashingFang combo
                        !HasEffect(Buffs.ReadyToBlast) || !HasEffect(Buffs.ReadyToRip) || !HasEffect(Buffs.ReadyToTear) || !HasEffect(Buffs.ReadyToGouge))) //Continuation buff check
                        return DoubleDown;

                    if (IsEnabled(CustomComboPreset.GNBPvP_BurstStrike) && HasEffect(Buffs.PowderBarrel) && ActionReady(GnashingFang)) //Burst Strike has prio over GnashingFang, but not DoubleDown
                        return BurstStrike;

                    //GnashingFang
                    if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) && ActionReady(GnashingFang) && !enemyGuard && HasEffect(Buffs.NoMercy) ||
                        JustUsed(GnashingFang) || JustUsed(JugularRip) || JustUsed(SavageClaw))
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

        internal class GNBPvP_DrawJunction : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_DrawAndJunction;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is DrawAndJunction &&
                    CanWeave(actionID) && (HasEffect(Buffs.JunctionDPS) || HasEffect(Buffs.JunctionHealer) || HasEffect(Buffs.JunctionTank))
                    ? OriginalHook(JunctionedCast)
                    : actionID;
        }
    }
}