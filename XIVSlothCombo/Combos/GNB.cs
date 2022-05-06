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
                NoMercy = 2,
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
        public static class Config
        {
            public const string
                GnbKeepRoughDivideCharges = "GnbKeepRoughDivideCharges";
        }
    }

    internal class GunbreakerSolidBarrelCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerSolidBarrelCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.SolidBarrel)
            {
                var gauge = GetJobGauge<GNBGauge>();
                var roughDivideChargesRemaining = Service.Configuration.GetCustomIntValue(GNB.Config.GnbKeepRoughDivideCharges);
                var quarterWeave = GetCooldown(actionID).CooldownRemaining < 1 && GetCooldown(actionID).CooldownRemaining > 0;

                if (IsEnabled(CustomComboPreset.GunbreakerRangedUptimeFeature))
                {
                    if (!InMeleeRange())
                        return GNB.LightningShot;
                }
                
                if (comboTime > 0)
                {
                    if (quarterWeave && IsEnabled(CustomComboPreset.GunbreakerMainComboCDsGroup) && IsEnabled(CustomComboPreset.GunbreakerNoMercyonST))
                    {
                        if (level >= GNB.Levels.NoMercy && IsOffCooldown(GNB.NoMercy))
                        {
                            if (level >= GNB.Levels.BurstStrike && 
                                ((gauge.Ammo == 1 && IsOffCooldown(GNB.GnashingFang) && IsOffCooldown(GNB.Bloodfest)) || //Opener Conditions
                                (gauge.Ammo == 2 && IsOnCooldown(GNB.GnashingFang) && GetCooldownRemainingTime(GNB.Bloodfest) < 3) || //GFNM windows
                                gauge.Ammo == GNB.MaxCartridges(level) && GetCooldownRemainingTime(GNB.GnashingFang) < 2)) //Regular NMGF
                                return GNB.NoMercy;
                            if (level < GNB.Levels.BurstStrike) //no cartridges unlocked
                                return GNB.NoMercy;
                        }
                    }

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerMainComboCDsGroup) && IsEnabled(CustomComboPreset.GunbreakerBloodfestonST))
                        {
                            if (gauge.Ammo == 0 && IsOffCooldown(GNB.Bloodfest) && level >= GNB.Levels.Bloodfest && IsOnCooldown(GNB.GnashingFang))
                                    return GNB.Bloodfest;
                        }

                        //Rough Divide Feature
                        if (level >= GNB.Levels.RoughDivide && IsEnabled(CustomComboPreset.GunbreakerRoughDivideFeature) && GetRemainingCharges(GNB.RoughDivide) > roughDivideChargesRemaining)
                                return GNB.RoughDivide;

                        //Blasting Zone outside of NM
                        if (IsEnabled(CustomComboPreset.GunbreakerMainComboCDsGroup) && level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.DangerZone))
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerDZOnMainComboFeature) && !HasEffect(GNB.Buffs.NoMercy)  &&
                                ((IsOnCooldown(GNB.GnashingFang) && gauge.AmmoComboStep != 1 && GetCooldown(GNB.GnashingFang).CooldownRemaining > 20) || //Post Gnashing Fang
                                (level < GNB.Levels.GnashingFang) && IsOnCooldown(GNB.NoMercy))) //Pre Gnashing Fang
                                return OriginalHook(GNB.DangerZone);
                        }

                        //60 second weaves
                        if (IsOnCooldown(GNB.DoubleDown))
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerDZOnMainComboFeature) && IsOffCooldown(GNB.DangerZone))
                                return OriginalHook(GNB.DangerZone);
                            if (IsEnabled(CustomComboPreset.GunbreakerBSOnMainComboFeature) && IsOffCooldown(GNB.BowShock))
                                return GNB.BowShock;
                        }

                        //30 second weaves
                        if (IsOnCooldown(GNB.SonicBreak))
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerBSOnMainComboFeature) && level >= GNB.Levels.BowShock && IsOffCooldown(GNB.BowShock))
                                return GNB.BowShock;
                            if (IsEnabled(CustomComboPreset.GunbreakerDZOnMainComboFeature) && level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.DangerZone))
                                return OriginalHook(GNB.DangerZone);
                        }

                        //Continuation
                        if (IsEnabled(CustomComboPreset.GunbreakerGnashingFangOnMain) && level >= GNB.Levels.Continuation &&
                            (HasEffect(GNB.Buffs.ReadyToRip) || HasEffect(GNB.Buffs.ReadyToTear) || HasEffect(GNB.Buffs.ReadyToGouge)) && level >= GNB.Levels.Continuation)
                                return OriginalHook(GNB.Continuation);
                    }

                    // 60s window features
                    if (GetCooldownRemainingTime(GNB.NoMercy) > 57 || HasEffect(GNB.Buffs.NoMercy) && IsEnabled(CustomComboPreset.GunbreakerMainComboCDsGroup))
                    {
                        if (level >= GNB.Levels.DoubleDown)
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerDDonMain) && IsOffCooldown(GNB.DoubleDown) && gauge.Ammo >= 2 && !HasEffect(GNB.Buffs.ReadyToRip) && gauge.AmmoComboStep >= 1)
                                return GNB.DoubleDown;
                            if (IsEnabled(CustomComboPreset.GunbreakerSBOnMainComboFeature) && IsOffCooldown(GNB.SonicBreak) && IsOnCooldown(GNB.DoubleDown))
                                return GNB.SonicBreak;
                        }

                        if (level < GNB.Levels.DoubleDown)
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerSBOnMainComboFeature) && level >= GNB.Levels.SonicBreak && IsOffCooldown(GNB.SonicBreak) && !HasEffect(GNB.Buffs.ReadyToRip))
                                return GNB.SonicBreak;

                            //sub level 54 functionality
                            if (IsEnabled(CustomComboPreset.GunbreakerDZOnMainComboFeature) && level >= GNB.Levels.DangerZone && level < GNB.Levels.SonicBreak && IsOffCooldown(GNB.DangerZone))
                                return OriginalHook(GNB.DangerZone);
                        }
                    }

                    //Pre Gnashing Fang stuff
                    if (IsEnabled(CustomComboPreset.GunbreakerGnashingFangOnMain) && level >= GNB.Levels.GnashingFang)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerGFStartonMain) && IsOffCooldown(GNB.GnashingFang) && gauge.AmmoComboStep == 0 &&
                            (gauge.Ammo == GNB.MaxCartridges(level) && GetCooldownRemainingTime(GNB.NoMercy) > 55 || //Regular 60 second GF/NM timing
                            (gauge.Ammo > 0 && GetCooldownRemainingTime(GNB.NoMercy) > 17 && GetCooldownRemainingTime(GNB.NoMercy) < 35) || //Regular 30 second window                                                                        
                            (gauge.Ammo == 3 && GetCooldownRemainingTime(GNB.Bloodfest) < 2 && GetCooldownRemainingTime(GNB.NoMercy) < 2) || //3 minute window
                            (gauge.Ammo == 1 && GetCooldownRemainingTime(GNB.NoMercy) > 55 && IsOffCooldown(GNB.Bloodfest)))) //Opener Conditions
                                return GNB.GnashingFang;
                        if (gauge.AmmoComboStep is 1 or 2)
                            return OriginalHook(GNB.GnashingFang);
                    }

                    if ((HasEffect(GNB.Buffs.NoMercy)|| HasEffect(All.Buffs.Medicated)) && gauge.AmmoComboStep == 0 && level >= GNB.Levels.BurstStrike)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature) && level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                            return GNB.Hypervelocity;
                        if (IsEnabled(CustomComboPreset.GunbreakerBSinNMFeature) && IsEnabled(CustomComboPreset.GunbreakerMainComboCDsGroup) && gauge.Ammo != 0 && IsOnCooldown(GNB.GnashingFang))
                            return GNB.BurstStrike;
                    }

                    //final check if Burst Strike is used right before No Mercy ends
                    if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature) && level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                        return GNB.Hypervelocity;

                    // Regular 1-2-3 combo with overcap feature
                    if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                        return GNB.BrutalShell;
                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                    {
                        if (IsEnabled(CustomComboPreset.GunbreakerAmmoOvercapFeature))
                        {
                            if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature) && level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                                return GNB.Hypervelocity;
                            if (level >= GNB.Levels.BurstStrike && (gauge.Ammo == GNB.MaxCartridges(level) ||
                                (IsEnabled(CustomComboPreset.GunbreakerBloodfestonST) && GetCooldownRemainingTime(GNB.Bloodfest) < 6 && gauge.Ammo != 0 && IsOnCooldown(GNB.NoMercy)))) //Burns Ammo for Bloodfest
                                return GNB.BurstStrike;
                        }

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

                if (IsOffCooldown(GNB.NoMercy) &&CanDelayedWeave(actionID) && IsOffCooldown(GNB.GnashingFang) && IsEnabled(CustomComboPreset.GunbreakerNoMercyonGF))
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
                                if (IsOffCooldown(GNB.DangerZone))
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
                            if (level >= GNB.Levels.DangerZone && IsOffCooldown(GNB.DangerZone))
                                return OriginalHook(GNB.DangerZone);
                        }
                    }

                }

                if (CanWeave(actionID))
                {
                    if (level >= GNB.Levels.DangerZone && IsOnCooldown(GNB.GnashingFang) && !HasEffect(GNB.Buffs.NoMercy) && IsOffCooldown(GNB.DangerZone) && gauge.AmmoComboStep != 1 && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF))
                        return OriginalHook(GNB.DangerZone);
                    if ((HasEffect(GNB.Buffs.ReadyToRip) || HasEffect(GNB.Buffs.ReadyToTear) || HasEffect(GNB.Buffs.ReadyToGouge)) && level >= GNB.Levels.Continuation)
                        return OriginalHook(GNB.Continuation);
                }

                if ((gauge.AmmoComboStep == 0 && IsOffCooldown(GNB.GnashingFang)) || gauge.AmmoComboStep is 1 or 2)
                    return OriginalHook(GNB.GnashingFang);
                if (HasEffect(GNB.Buffs.NoMercy) && HasEffect(All.Buffs.Medicated) && IsEnabled(CustomComboPreset.GunbreakerCDsOnGF) && gauge.AmmoComboStep == 0)
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
            var gauge = GetJobGauge<GNBGauge>();
            if (actionID == GNB.DemonSlaughter)
            {
                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerNoMercyAOEOption) && IsOffCooldown(GNB.NoMercy) && level >= GNB.Levels.NoMercy)
                        return GNB.NoMercy;
                    if (IsEnabled(CustomComboPreset.GunbreakerBloodfestAOEOption) && gauge.Ammo == 0 && IsOffCooldown(GNB.Bloodfest) && level >= GNB.Levels.Bloodfest)
                        return GNB.Bloodfest;
                }

                if (IsEnabled(CustomComboPreset.GunbreakerDoubleDownAOEOption) && gauge.Ammo >= 2 && IsOffCooldown(GNB.DoubleDown) && level >= GNB.Levels.DoubleDown)
                    return GNB.DoubleDown;
                if (IsEnabled(CustomComboPreset.GunbreakerBloodfestAOEOption) && gauge.Ammo != 0 && GetCooldownRemainingTime(GNB.Bloodfest) < 6 &&  level >= GNB.Levels.FatedCircle)
                    return GNB.FatedCircle;

                if (comboTime > 0 && lastComboMove == GNB.DemonSlice && level >= GNB.Levels.DemonSlaughter)
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerAmmoOvercapFeature) && level >= GNB.Levels.FatedCircle && gauge.Ammo == GNB.MaxCartridges(level))
                            return GNB.FatedCircle;
                    if (IsEnabled(CustomComboPreset.GunbreakerBowShockFeature) && level >= GNB.Levels.BowShock && IsOffCooldown(GNB.BowShock))
                            return GNB.BowShock;
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
            var gauge = GetJobGauge<GNBGauge>().Ammo;
            if (actionID == GNB.BurstStrike)
            {
                if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature) && level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
                if (gauge == 0 && level >= GNB.Levels.Bloodfest)
                    return GNB.Bloodfest;
            }

            return actionID;
        }
    }

    internal class GunbreakerDDonBurstStrikeFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerDDonBurstStrikeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<GNBGauge>().Ammo;
            if (actionID == GNB.BurstStrike)
            {
                if (HasEffect(GNB.Buffs.NoMercy) && IsOffCooldown(GNB.DoubleDown) && gauge >= 2)
                    return GNB.DoubleDown;
            }

            return actionID;
        }
    }
    
}