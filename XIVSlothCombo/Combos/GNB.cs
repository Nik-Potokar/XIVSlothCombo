using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class GNB
    {
        public const byte JobID = 37;

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
            Hypervelocity = 25759;

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
                BowShock = 1838;
        }

        public static class Levels
        {
            public const byte
                BrutalShell = 4,
                SolidBarrel = 26,
                DemonSlaughter = 40,
                SonicBreak = 54,
                BowShock = 62,
                Continuation = 70,
                FatedCircle = 72,
                Bloodfest = 76,
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
                if (comboTime > 0)
                {
                    var maincomboCD1 = GetCooldown(GNB.KeenEdge);
                    var maincomboCD2 = GetCooldown(GNB.BrutalShell);
                    var maincomboCD3 = GetCooldown(GNB.SolidBarrel);
                    var blastingzoneCD = GetCooldown(GNB.BlastingZone);
                    var doubleDownCD = GetCooldown(GNB.DoubleDown);
                    var bulletGauge = GetJobGauge<GNBGauge>();

                    if (IsEnabled(CustomComboPreset.GunbreakerDangerZoneFeature))
                    {
                        if (lastComboMove == GNB.KeenEdge && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level >= 80)
                            return GNB.BlastingZone;
                        if (lastComboMove == GNB.BrutalShell && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level >= 80)
                            return GNB.BlastingZone;
                        if (lastComboMove == GNB.SolidBarrel && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level >= 80)
                            return GNB.BlastingZone;
                        if (lastComboMove == GNB.KeenEdge && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level <= 79)
                            return GNB.DangerZone;
                        if (lastComboMove == GNB.BrutalShell && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level <= 79)
                            return GNB.DangerZone;
                        if (lastComboMove == GNB.SolidBarrel && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level <= 79)
                            return GNB.DangerZone;
                    }

                    if (IsEnabled(CustomComboPreset.GunbreakerDoubleDownFeature))
                    {
                        if (lastComboMove == GNB.KeenEdge && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level == 90 && HasEffect(GNB.Buffs.NoMercy))
                            return GNB.DoubleDown;
                        if (lastComboMove == GNB.BrutalShell && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level == 90 && HasEffect(GNB.Buffs.NoMercy))
                            return GNB.DoubleDown;
                        if (lastComboMove == GNB.SolidBarrel && !blastingzoneCD.IsCooldown && maincomboCD1.CooldownRemaining > 0.7 && level == 90 && HasEffect(GNB.Buffs.NoMercy))
                            return GNB.DoubleDown;
                    }

                    if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                        return GNB.BrutalShell;
                    if (lastComboMove == GNB.BrutalShell && level >= 88 && bulletGauge.Ammo == 3)
                        return GNB.BurstStrike;
                    if (lastComboMove == GNB.BrutalShell && level >= 4 && level <= 87 && bulletGauge.Ammo == 2)
                        return GNB.BurstStrike;
                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.EnhancedContinuation && IsEnabled(CustomComboPreset.GunbreakerBurstStrikeConFeature) && HasEffect(GNB.Buffs.ReadyToBlast))
                        return GNB.Hypervelocity;
                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                        return GNB.SolidBarrel;
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
                if (level >= GNB.Levels.Continuation)
                {
                    if (HasEffect(GNB.Buffs.ReadyToGouge))
                        return GNB.EyeGouge;

                    if (HasEffect(GNB.Buffs.ReadyToTear))
                        return GNB.AbdomenTear;

                    if (HasEffect(GNB.Buffs.ReadyToRip))
                        return GNB.JugularRip;
                }

                // Gnashing Fang > Savage Claw > Wicked Talon
                return OriginalHook(GNB.GnashingFang);
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
                    if (IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature) && level >= GNB.Levels.FatedCircle)
                    {
                        var gauge = GetJobGauge<GNBGauge>();
                        var cartridgeMax = level >= 88 ? 3 : 2;

                        if (gauge.Ammo == cartridgeMax)
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

    internal class GunbreakerNoMercyFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerNoMercyFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.NoMercy)
            {
                if (HasEffect(GNB.Buffs.NoMercy))
                {
                    if (level >= GNB.Levels.BowShock && !TargetHasEffect(GNB.Debuffs.BowShock))
                        return GNB.BowShock;

                    if (level >= GNB.Levels.SonicBreak)
                        return GNB.SonicBreak;
                }
            }

            return actionID;
        }
    }
}
