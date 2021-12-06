using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
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
            Bloodfest = 16164;

        public static class Buffs
        {
            public const short
                NoMercy = 1831,
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844;
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
                Bloodfest = 76;
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
                    var dangerZoneCD = GetCooldown(GNB.DangerZone);
                    var doubleDownCD = GetCooldown(GNB.DoubleDown);
                    var bulletGauge = GetJobGauge<GNBGauge>();
                    if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                        return GNB.BrutalShell;

                    if (lastComboMove == GNB.BrutalShell && level >= 4 && bulletGauge.Ammo == 2)
                        return GNB.BurstStrike;

                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                        return GNB.SolidBarrel;
                    if (maincomboCD1.CooldownRemaining > 0.7 || (maincomboCD2.CooldownRemaining > 0.7) || ((maincomboCD3.CooldownRemaining > 0.7) && !dangerZoneCD.IsCooldown && level > 18 && IsEnabled(CustomComboPreset.GunbreakerDangerZoneFeature)))
                        return GNB.DangerZone;
                    if (maincomboCD1.CooldownRemaining > 0.7 || (maincomboCD2.CooldownRemaining > 0.7) || ((maincomboCD3.CooldownRemaining > 0.7) && !doubleDownCD.IsCooldown && HasEffect(GNB.Buffs.NoMercy) && level >= 90 && IsEnabled(CustomComboPreset.GunbreakerDoubleDownFeature)))
                        return GNB.DoubleDown;
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
            if (actionID == GNB.WickedTalon)
            {
                if (IsEnabled(CustomComboPreset.GunbreakerGnashingFangCont))
                {
                    if (level >= GNB.Levels.Continuation)
                    {
                        if (HasEffect(GNB.Buffs.ReadyToRip))
                            return GNB.JugularRip;

                        if (HasEffect(GNB.Buffs.ReadyToTear))
                            return GNB.AbdomenTear;

                        if (HasEffect(GNB.Buffs.ReadyToGouge))
                            return GNB.EyeGouge;
                    }
                }

                var gauge = GetJobGauge<GNBGauge>();
                return gauge.AmmoComboStep switch
                {
                    1 => GNB.SavageClaw,
                    2 => GNB.WickedTalon,
                    _ => GNB.GnashingFang,
                };
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
                    if (IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature))
                    {
                        var gauge = GetJobGauge<GNBGauge>();
                        if (gauge.Ammo == 2 && level >= GNB.Levels.FatedCircle)
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
