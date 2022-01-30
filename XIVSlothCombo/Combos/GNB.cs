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
            public const short
                NoMercy = 1831,
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844,
                ReadyToBlast = 2686;
        }

        public static class Debuffs
        {
            public const short
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
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerSolidBarrelCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.SolidBarrel)
            {
                if (IsEnabled(CustomComboPreset.GunbreakerRangedUptimeFeature))
                {
                    if(!InMeleeRange(true))
                    return GNB.LightningShot;
                }
                if (comboTime > 0)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    var GCD = GetCooldown(actionID);
                    var blastingZoneCD = GetCooldown(GNB.BlastingZone);
                    var doubleDownCD = GetCooldown(GNB.DoubleDown);
                    var sonicBreakCD = GetCooldown(GNB.SonicBreak);
                    var bowShockCD = GetCooldown(GNB.BowShock);
                    var gnashingFangCD = GetCooldown(GNB.GnashingFang);
                    var roughDivideCD = GetCooldown(GNB.RoughDivide);

                    // Gnashing Fang combo + Continuation, Gnashing Fang needs to be used manually in order for users to control for any delay based on fight times
                    if (level >= GNB.Levels.GnashingFang && IsEnabled(CustomComboPreset.GunbreakerGnashingFangOnMain))
                    {
                        if (level < GNB.Levels.DoubleDown && !blastingZoneCD.IsCooldown && level >= GNB.Levels.DangerZone && gauge.AmmoComboStep == 1 && IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature))
                            return OriginalHook(GNB.DangerZone);
                        if (HasEffect(GNB.Buffs.ReadyToRip) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                            return GNB.JugularRip;

                        if (HasEffect(GNB.Buffs.NoMercy))
                        {
                            if (level >= GNB.Levels.DoubleDown)
                            {
                                if (!doubleDownCD.IsCooldown && gauge.Ammo is 2 or 3 && IsEnabled(CustomComboPreset.GunbreakerDDonMain))
                                    return GNB.DoubleDown;
                                if (IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature) && doubleDownCD.IsCooldown)
                                {
                                    if (!blastingZoneCD.IsCooldown)
                                        return OriginalHook(GNB.DangerZone);
                                    if (!bowShockCD.IsCooldown)
                                        return GNB.BowShock;
                                    if (!sonicBreakCD.IsCooldown)
                                        return GNB.SonicBreak;
                                }
                            }

                            if (level < GNB.Levels.DoubleDown && IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature))
                            {
                                if (level >= GNB.Levels.SonicBreak && !sonicBreakCD.IsCooldown)
                                    return GNB.SonicBreak;
                                if (level >= GNB.Levels.BowShock && !bowShockCD.IsCooldown)
                                    return GNB.BowShock;
                            }
                        }

                        if (gauge.AmmoComboStep == 1)
                            return OriginalHook(GNB.GnashingFang);
                        if (HasEffect(GNB.Buffs.ReadyToTear) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                            return GNB.AbdomenTear;
                        //aligns it with 2nd GCD in NM.
                        if (!HasEffect(GNB.Buffs.NoMercy) && !blastingZoneCD.IsCooldown && level >= GNB.Levels.DangerZone && gauge.AmmoComboStep == 2 && IsEnabled(CustomComboPreset.GunbreakerCDsOnMainComboFeature))
                            return OriginalHook(GNB.DangerZone);

                        if (gauge.AmmoComboStep == 2)
                            return OriginalHook(GNB.GnashingFang);
                        if (HasEffect(GNB.Buffs.ReadyToGouge) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                            return GNB.EyeGouge;
                        if (HasEffect(GNB.Buffs.NoMercy) && gnashingFangCD.IsCooldown && gauge.AmmoComboStep == 0)
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

                    // uses all stacks
                    if (IsEnabled(CustomComboPreset.GunbreakerRoughDivide2StackOption) && level >= 56)
                    {
                        if (roughDivideCD.CooldownRemaining < 30 && GCD.CooldownRemaining > 0.7)
                            return GNB.RoughDivide;
                    }
                    // leaves 1 stack
                    if (IsEnabled(CustomComboPreset.GunbreakerRoughDivide1StackOption) && level >= 56)
                    {
                        if (roughDivideCD.CooldownRemaining < 60 && !roughDivideCD.IsCooldown && GCD.CooldownRemaining > 0.7)
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
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerGnashingFangCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.GnashingFang)
            {
                var gauge = GetJobGauge<GNBGauge>();
                var GCD = GetCooldown(actionID);
                var gnashingFangCD = GetCooldown(GNB.GnashingFang);
                var blastingZoneCD = GetCooldown(GNB.BlastingZone);
                var doubleDownCD = GetCooldown(GNB.DoubleDown);
                var sonicBreakCD = GetCooldown(GNB.SonicBreak);
                var bowShockCD = GetCooldown(GNB.BowShock);
                var noMercyCD = GetCooldown(GNB.NoMercy);

                if (!noMercyCD.IsCooldown && !gnashingFangCD.IsCooldown && IsEnabled(CustomComboPreset.GunbreakerNoMercyonGF))
                    return GNB.NoMercy;
                if (gauge.AmmoComboStep == 0 && !gnashingFangCD.IsCooldown)
                    return OriginalHook(GNB.GnashingFang);
                if (level < GNB.Levels.DoubleDown && !blastingZoneCD.IsCooldown && level >= GNB.Levels.DangerZone && gauge.AmmoComboStep == 1 && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                    return OriginalHook(GNB.DangerZone);
                if (HasEffect(GNB.Buffs.ReadyToRip) && level >= GNB.Levels.Continuation)
                        return GNB.JugularRip;
                if (HasEffect(GNB.Buffs.NoMercy))
                {
                    if (level >= GNB.Levels.DoubleDown)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerDDOnGF) && !doubleDownCD.IsCooldown && gauge.Ammo is 2 or 3)
                            return GNB.DoubleDown;
                        if (doubleDownCD.IsCooldown && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                        {
                            if (!blastingZoneCD.IsCooldown)
                                return OriginalHook(GNB.DangerZone);
                            if (!bowShockCD.IsCooldown)
                                return GNB.BowShock;
                            if (!sonicBreakCD.IsCooldown)
                                return GNB.SonicBreak;
                        }
                    }

                    if (level < GNB.Levels.DoubleDown && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                    {
                        if (level >= GNB.Levels.SonicBreak && !sonicBreakCD.IsCooldown)
                            return GNB.SonicBreak;
                        if (level >= GNB.Levels.BowShock && !bowShockCD.IsCooldown)
                            return GNB.BowShock;
                    }

                }

                if (gauge.AmmoComboStep == 1)
                    return OriginalHook(GNB.GnashingFang);
                if (HasEffect(GNB.Buffs.ReadyToTear) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                    return GNB.AbdomenTear;
                //aligns it with 2nd GCD in NM.
                if (level >= GNB.Levels.DoubleDown && !HasEffect(GNB.Buffs.NoMercy) && !blastingZoneCD.IsCooldown && gauge.AmmoComboStep == 2 && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                    return OriginalHook(GNB.DangerZone); 

                if (gauge.AmmoComboStep == 2)
                    return OriginalHook(GNB.GnashingFang);
                if (HasEffect(GNB.Buffs.ReadyToGouge) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                    return GNB.EyeGouge;
                if (HasEffect(GNB.Buffs.NoMercy) && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
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
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerBurstStrikeConFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerDemonSlaughterCombo;

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

                    return GNB.DemonSlaughter;
                }

                return GNB.DemonSlice;
            }

            return actionID;
        }
    }

    internal class GunbreakerBloodfestOvercapFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerBloodfestOvercapFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerNoMercyRotationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.NoMercy)
            {
                var gauge = GetJobGauge<GNBGauge>();
                var GCD = GetCooldown(actionID);
                var gnashingFangCD = GetCooldown(GNB.GnashingFang);
                var blastingZoneCD = GetCooldown(GNB.BlastingZone);
                var doubleDownCD = GetCooldown(GNB.DoubleDown);
                var sonicBreakCD = GetCooldown(GNB.SonicBreak);
                var bowShockCD = GetCooldown(GNB.BowShock);

                if (level >= GNB.Levels.GnashingFang && HasEffect(GNB.Buffs.NoMercy))
                {
                    if (gauge.AmmoComboStep == 0 && !gnashingFangCD.IsCooldown)
                        return OriginalHook(GNB.GnashingFang);
                    if (HasEffect(GNB.Buffs.ReadyToRip) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                        return OriginalHook(GNB.Continuation);
                    if (level >= GNB.Levels.DoubleDown)
                    {
                        if (!doubleDownCD.IsCooldown && gauge.Ammo is 2 or 3)
                            return GNB.DoubleDown;
                        if (doubleDownCD.IsCooldown)
                        {
                            if (!blastingZoneCD.IsCooldown)
                                return OriginalHook(GNB.DangerZone);
                            if (!bowShockCD.IsCooldown)
                                return GNB.BowShock;
                            if (!sonicBreakCD.IsCooldown)
                                return GNB.SonicBreak;
                        }
                    }

                    if (level < GNB.Levels.DoubleDown)
                    {
                        if (!blastingZoneCD.IsCooldown)
                            return OriginalHook(GNB.DangerZone);
                        if (!sonicBreakCD.IsCooldown)
                            return GNB.SonicBreak;
                        if (level >= GNB.Levels.BowShock && !bowShockCD.IsCooldown)
                            return GNB.BowShock;
                    }


                    if (gauge.AmmoComboStep == 1)
                        return OriginalHook(GNB.GnashingFang);
                    if (HasEffect(GNB.Buffs.ReadyToTear) && level >= GNB.Levels.Continuation && GCD.CooldownRemaining > 0.7)
                        return OriginalHook(GNB.Continuation);
                    if (gauge.AmmoComboStep == 2)
                        return OriginalHook(GNB.GnashingFang);
                    if (HasEffect(GNB.Buffs.ReadyToGouge) && level >= GNB.Levels.Continuation)
                        return OriginalHook(GNB.Continuation);
                    if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                        return GNB.Hypervelocity;
                    if (gauge.Ammo != 0)
                        return GNB.BurstStrike;
                }

                //final check if Burst Strike is used right before No Mercy ends
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
                return GNB.NoMercy;
            }

            return actionID;
        }
    }
}