using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class GNB
    {
        public const byte JobID = 37;

        public static int MaxCartridges(byte level)
        {
            return level >= Levels.CartridgeCharge3 ? 3 : 2;
        }

        public const uint
            LowBlow = 7540,
            Interject = 7538,
            KeenEdge = 16137,
            NoMercy = 16138,
            BrutalShell = 16139,
            DemonSlice = 16141,
            SolidBarrel = 16145,
            GnashingFang = 16146,
            SavageClaw = 16147,
            DemonSlaughter = 16149,
            WickedTalon = 16150,
            SonicBreak = 16153,
            Continuation = 16155,
            JugularRip = 16156,
            AbdomenTear = 16157,
            EyeGouge = 16158,
            BowShock = 16159,
            BurstStrike = 16162,
            FatedCircle = 16163,
            DoubleDown = 25760,
            DangerZone = 16144,
            BlastingZone = 16165,
            Bloodfest = 16164,
            Hypervelocity = 25759,
            RoughDivide = 16154,
            LightningShot = 16143;

        public static class Buffs
        {
            public const ushort
                NoMercy = 1831,
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844,
                ReadyToBlast = 2686;
        }

        public static class Debuffs
        {
            public const ushort
                BowShock = 1838,
                SonicBreak = 1837;
        }

        public static class Levels
        {
            public const byte
                BrutalShell = 4,
                DangerZone = 18,
                SolidBarrel = 26,
                BurstStrike = 30,
                DemonSlaughter = 40,
                SonicBreak = 54,
                RoughDivide = 56,
                GnashingFang = 60,
                BowShock = 62,
                Continuation = 70,
                FatedCircle = 72,
                Bloodfest = 76,
                BlastingZone = 80,
                EnhancedContinuation = 86,
                CartridgeCharge3 = 88,
                DoubleDown = 90;
        }
    }

    internal class GunbreakerSolidBarrelCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerSolidBarrelCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.SolidBarrel)
            {
                if (IsEnabled(CustomComboPreset.GunbreakerRangedUptimeFeature))
                {
                    if (!InMeleeRange(true))
                        return GNB.LightningShot;
                }
                
                if (comboTime > 0)
                {
                    var gauge = GetJobGauge<GNBGauge>();

                    // Gnashing Fang combo + Continuation, Gnashing Fang needs to be used manually in order for users to control for any delay based on fight times
                    if (level >= GNB.Levels.GnashingFang && IsEnabled(CustomComboPreset.GunbreakerGnashingFangOnMain))
                    {

                        if (HasEffect(GNB.Buffs.NoMercy) && IsOnCooldown(GNB.GnashingFang))
                        {
                            if (level >= GNB.Levels.DoubleDown)
                            {
                                if (IsOffCooldown(GNB.DoubleDown) && gauge.Ammo is 2 or 3 && IsEnabled(CustomComboPreset.GunbreakerDDonMain) && !HasEffect(GNB.Buffs.ReadyToRip))
                                    return GNB.DoubleDown;
                                if (IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature) && IsOnCooldown(GNB.DoubleDown))
                                {
                                    if (CanWeave(actionID))
                                    {
                                        if (IsOffCooldown(GNB.BlastingZone))
                                            return OriginalHook(GNB.DangerZone);
                                        if (IsOffCooldown(GNB.BowShock))
                                            return GNB.BowShock;
                                    }

                                    if (IsOffCooldown(GNB.SonicBreak))
                                        return GNB.SonicBreak;
                                }
                            }

                            if (level < GNB.Levels.DoubleDown && IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature))
                            {
                                if (level >= GNB.Levels.SonicBreak && IsOffCooldown(GNB.SonicBreak) && !HasEffect(GNB.Buffs.ReadyToRip))
                                    return GNB.SonicBreak;
                                if (IsOnCooldown(GNB.SonicBreak) && CanWeave(actionID))
                                {
                                    if (level >= GNB.Levels.BowShock && IsOffCooldown(GNB.BowShock))
                                        return GNB.BowShock;
                                    if (level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.BlastingZone))
                                        return OriginalHook(GNB.DangerZone);
                                }
                            }
                        }

                        if (CanWeave(actionID))
                        {
                            if (level >= GNB.Levels.DangerZone && IsOnCooldown(GNB.GnashingFang) && !HasEffect(GNB.Buffs.NoMercy) && IsOffCooldown(GNB.BlastingZone) && gauge.AmmoComboStep != 1 && IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature))
                                return OriginalHook(GNB.DangerZone);
                            if ((HasEffect(GNB.Buffs.ReadyToRip) || HasEffect(GNB.Buffs.ReadyToTear) || HasEffect(GNB.Buffs.ReadyToGouge)) && level >= GNB.Levels.Continuation)
                                return OriginalHook(GNB.Continuation);
                        }

                        if (gauge.AmmoComboStep is 1 or 2)
                            return OriginalHook(GNB.GnashingFang);

                        if (HasEffect(GNB.Buffs.NoMercy) && gauge.AmmoComboStep == 0)
                        {
                            if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast) && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature))
                                return GNB.Hypervelocity;
                            if ((gauge.Ammo != 0) && level >= GNB.Levels.BurstStrike)
                                return GNB.BurstStrike;
                        }

                        //final check if Burst Strike is used right before No Mercy ends
                        if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast) && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature))
                            return GNB.Hypervelocity;
                    }

                    if (CanWeave(actionID) && level >= GNB.Levels.RoughDivide)
                    {
                        
                        if (IsEnabled(CustomComboPreset.GunbreakerRoughDivide2StackOption) && GetRemainingCharges(GNB.RoughDivide) > 0 || // uses all stacks
                            IsEnabled(CustomComboPreset.GunbreakerRoughDivide1StackOption) && GetRemainingCharges(GNB.RoughDivide) > 1) // leaves 1 stack
                            return GNB.RoughDivide;
                    }

                    // Regular 1-2-3 combo with overcap feature
                    if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                        return GNB.BrutalShell;
                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerAmmoOvercapFeature))
                        {
                            if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast) && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature))
                                return GNB.Hypervelocity;
                        }

                        if (level >= GNB.Levels.BurstStrike && gauge.Ammo == GNB.MaxCartridges(level))
                            return GNB.BurstStrike;

                        return GNB.SolidBarrel;
                    }
                }

                return GNB.KeenEdge;
            }

            return actionID;
        }
    }

    internal class GunbreakerGnashingFangCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerGnashingFangCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.GnashingFang)
            {
                var gauge = GetJobGauge<GNBGauge>();

                if (IsOffCooldown(GNB.NoMercy) && IsOffCooldown(GNB.GnashingFang) && IsEnabled(CustomComboPreset.GunbreakerNoMercyonGF))
                    return GNB.NoMercy;

                if (HasEffect(GNB.Buffs.NoMercy) && IsOnCooldown(GNB.GnashingFang))
                {
                    if (level >= GNB.Levels.DoubleDown)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerDDOnGF) && IsOffCooldown(GNB.DoubleDown) && gauge.Ammo is 2 or 3 && !HasEffect(GNB.Buffs.ReadyToRip))
                            return GNB.DoubleDown;
                        if (IsOnCooldown(GNB.DoubleDown) && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsOffCooldown(GNB.BlastingZone))
                                    return OriginalHook(GNB.DangerZone);
                                if (IsOffCooldown(GNB.BowShock))
                                    return GNB.BowShock;
                            }

                            if (IsOffCooldown(GNB.SonicBreak))
                                return GNB.SonicBreak;
                        }
                    }

                    if (level < GNB.Levels.DoubleDown && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                    {
                        if (level >= GNB.Levels.SonicBreak && IsOffCooldown(GNB.SonicBreak) && !HasEffect(GNB.Buffs.ReadyToRip))
                            return GNB.SonicBreak;
                        if (IsOnCooldown(GNB.SonicBreak) && CanWeave(actionID))
                        {
                            if (level >= GNB.Levels.BowShock && IsOffCooldown(GNB.BowShock))
                                return GNB.BowShock;
                            if (level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.BlastingZone))
                                return OriginalHook(GNB.DangerZone);
                        }
                    }

                }

                if (CanWeave(actionID))
                {
                    if (level >= GNB.Levels.DangerZone && IsOnCooldown(GNB.GnashingFang) && !HasEffect(GNB.Buffs.NoMercy) && IsOffCooldown(GNB.BlastingZone) && gauge.AmmoComboStep != 1 && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                        return OriginalHook(GNB.DangerZone);
                    if ((HasEffect(GNB.Buffs.ReadyToRip) || HasEffect(GNB.Buffs.ReadyToTear) || HasEffect(GNB.Buffs.ReadyToGouge)) && level >= GNB.Levels.Continuation)
                        return OriginalHook(GNB.Continuation);
                }

                if ((gauge.AmmoComboStep == 0 && IsOffCooldown(GNB.GnashingFang)) || gauge.AmmoComboStep is 1 or 2)
                    return OriginalHook(GNB.GnashingFang);
                if (HasEffect(GNB.Buffs.NoMercy) && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF) && gauge.AmmoComboStep == 0)
                {
                    if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast) && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature))
                        return GNB.Hypervelocity;
                    if ((gauge.Ammo != 0) && level >= GNB.Levels.BurstStrike)
                        return GNB.BurstStrike;
                }
                //final check if Burst Strike is used right before No Mercy ends
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast) && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature))
                    return GNB.Hypervelocity;
            }

            return actionID;
        }
    }

    internal class GunbreakerBurstStrikeConFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerBurstStrikeConFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.BurstStrike)
            {
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
            }

            return actionID;
        }
    }

    internal class GunbreakerDemonSlaughterCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerDemonSlaughterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.DemonSlaughter)
            {
                if (comboTime > 0 && lastComboMove == GNB.DemonSlice && level >= GNB.Levels.DemonSlaughter)
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerAmmoOvercapFeature) && level >= GNB.Levels.FatedCircle)
                    {
                        var gauge = GetJobGauge<GNBGauge>();

                        if (gauge.Ammo == GNB.MaxCartridges(level))
                        {
                            return GNB.FatedCircle;
                        }
                    }

                    if(IsEnabled(CustomComboPreset.GunbreakerBowShockFeature) && level >= GNB.Levels.BowShock)
                    {
                        if (IsOffCooldown(GNB.BowShock))
                            return GNB.BowShock;
                    }

                    return GNB.DemonSlaughter;
                }

                return GNB.DemonSlice;
            }

            return actionID;
        }
    }

    internal class GunbreakerBloodfestOvercapFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerBloodfestOvercapFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.BurstStrike)
            {
                var gauge = GetJobGauge<GNBGauge>().Ammo;
                if (gauge == 0 && level >= GNB.Levels.Bloodfest)
                    return GNB.Bloodfest;
            }

            return actionID;
        }
    }

    internal class GunbreakerNoMercyRotationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerNoMercyRotationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.NoMercy)
            {
                var gauge = GetJobGauge<GNBGauge>();

                if (level >= GNB.Levels.GnashingFang && HasEffect(GNB.Buffs.NoMercy))
                {
                    if (IsOnCooldown(GNB.GnashingFang))
                    {
                        if (level >= GNB.Levels.DoubleDown)
                        {
                            if (IsOffCooldown(GNB.DoubleDown) && gauge.Ammo is 2 or 3 && !HasEffect(GNB.Buffs.ReadyToRip))
                                return GNB.DoubleDown;
                            if (IsOnCooldown(GNB.DoubleDown))
                            {
                                if (CanWeave(actionID))
                                {
                                    if (IsOffCooldown(GNB.BlastingZone))
                                        return OriginalHook(GNB.DangerZone);
                                    if (IsOffCooldown(GNB.BowShock))
                                        return GNB.BowShock;
                                }

                                if (IsOffCooldown(GNB.SonicBreak))
                                    return GNB.SonicBreak;
                            }
                        }

                        if (level < GNB.Levels.DoubleDown)
                        {
                            if (level >= GNB.Levels.SonicBreak && IsOffCooldown(GNB.SonicBreak) && !HasEffect(GNB.Buffs.ReadyToRip))
                                return GNB.SonicBreak;
                            if (CanWeave(actionID) && IsOnCooldown(GNB.SonicBreak))
                            {
                                if (level >= GNB.Levels.BowShock && IsOffCooldown(GNB.BowShock))
                                    return GNB.BowShock;
                                if (level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.BlastingZone))
                                    return OriginalHook(GNB.DangerZone);
                            }
                        }
                    }

                    if (CanWeave(actionID))
                    {
                        if (level >= GNB.Levels.Continuation && (HasEffect(GNB.Buffs.ReadyToRip) || HasEffect(GNB.Buffs.ReadyToTear) || HasEffect(GNB.Buffs.ReadyToGouge)))
                            return OriginalHook(GNB.Continuation);
                    }

                    if ((gauge.AmmoComboStep == 0 && IsOffCooldown(GNB.GnashingFang)) || gauge.AmmoComboStep is 1 or 2)
                        return OriginalHook(GNB.GnashingFang);
                    if (gauge.AmmoComboStep == 0)
                    {
                        if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                            return GNB.Hypervelocity;
                        if (gauge.Ammo != 0 && level >= GNB.Levels.BurstStrike)
                            return GNB.BurstStrike;
                    }
                }

                //final check if Burst Strike is used right before No Mercy ends
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
                return GNB.NoMercy;
            }

            return actionID;
        }
    }
    internal class GunbreakerInterruptFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.LowBlow)
            {
                var interjectCD = GetCooldown(GNB.Interject);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return GNB.Interject;
            }

            return actionID;
        }
    }
}