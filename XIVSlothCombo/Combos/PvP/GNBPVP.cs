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
            RelentlessRush = 29130,
            TerminalTrigger = 29131,
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
                JugularRip = 3048,
                AbdomenTear = 3049,
                EyeGouge = 3050,
                JunctionTank = 3044,
                JunctionDPS = 3045,
                JunctionHealer = 3046;
        }

        internal class GNBPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_Burst;

            float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.4 base in PvP
            bool enemyGuard = TargetHasEffect(PvPCommon.Buffs.Guard); //Guard check
            bool inGF = JustUsed(GnashingFang, 3f) || JustUsed(SavageClaw, 3f) || JustUsed(WickedTalon, 2f);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {
                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        //Continuation
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_Continuation) &&
                            HasEffect(Buffs.ReadyToBlast) || //Hypervelocity
                            HasEffect(Buffs.ReadyToRip) || //GunStep1
                            HasEffect(Buffs.ReadyToTear) || //GunStep2
                            HasEffect(Buffs.ReadyToGouge)) //GunStep reset to 0
                            return OriginalHook(Continuation);

                        //Draw&Junction buffs
                        if (!enemyGuard &&
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionDPS) && HasEffect(Buffs.JunctionDPS) && HasEffect(Buffs.NoMercy) && ActionReady(JunctionedCast)) || //BlastingZone
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionHealer) && HasEffect(Buffs.JunctionHealer) && ActionReady(JunctionedCast)) || //Aurora
                            (IsEnabled(CustomComboPreset.GNBPvP_JunctionTank) && HasEffect(Buffs.JunctionTank) && ActionReady(JunctionedCast))) //Nebula
                            return OriginalHook(JunctionedCast);

                        //Draw&Junction
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_DrawAndJunction) && ActionReady(DrawAndJunction) && !HasEffect(Buffs.PowderBarrel) && !HasEffect(Buffs.ReadyToBlast))
                            return DrawAndJunction;

                        //RoughDivide
                        if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide) && !enemyGuard && 
                            ActionReady(RoughDivide) && !HasEffect(Buffs.NoMercy) && //Mashing would be a waste
                            (GetCooldownRemainingTime(DoubleDown) <= GCD || //Keeps 1 charge always for DoubleDown usage
                            GetCooldownRemainingTime(GnashingFang) <= GCD)) //Keeps 1 charge always for GnashingFang usage
                            return RoughDivide;
                    }

                    //RoughDivide overcap protection
                    if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide) && !enemyGuard && HasCharges(RoughDivide) &&
                        (ActionReady(DoubleDown) && GetRemainingCharges(RoughDivide) > 1) || //force for DoubleDown
                        GetRemainingCharges(RoughDivide) == 2) //force if at 2
                        return RoughDivide;

                    //SavageClaw & WickedTalon
                    if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) &&
                        JustUsed(GnashingFang, 3f) || JustUsed(SavageClaw, 3f))
                        return OriginalHook(GnashingFang);

                    //DoubleDown
                    if (IsEnabled(CustomComboPreset.GNBPvP_DoubleDown) && !enemyGuard && ActionReady(DoubleDown) &&
                        HasEffect(Buffs.NoMercy) && InMeleeRange() && !inGF) //DoubleDown breaks Gnashing combo in PvP
                        return DoubleDown;

                    //BurstStrike
                    if (IsEnabled(CustomComboPreset.GNBPvP_BurstStrike) && HasEffect(Buffs.PowderBarrel) &&
                        (!HasEffect(Buffs.JugularRip) || !HasEffect(Buffs.AbdomenTear) || !HasEffect(Buffs.EyeGouge) && //Do not use when in GnashingFang combo
                        !HasEffect(Buffs.ReadyToBlast) || !HasEffect(Buffs.ReadyToRip) || !HasEffect(Buffs.ReadyToTear) || !HasEffect(Buffs.ReadyToGouge))) //Burst Strike has prio over GnashingFang, but not DoubleDown
                        return BurstStrike;

                    //GnashingFang
                    if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) && ActionReady(GnashingFang) && !HasEffect(Buffs.PowderBarrel)) //BurstStrike first to avoid losing the buff if applicable
                        return GnashingFang;
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