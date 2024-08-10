using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class GNB
    {
        public const byte JobID = 37;

        public static int MaxCartridges(byte level) => level >= 88 ? 3 : 2;

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
            HeartOfLight = 16160,
            BurstStrike = 16162,
            FatedCircle = 16163,
            Aurora = 16151,
            DoubleDown = 25760,
            DangerZone = 16144,
            BlastingZone = 16165,
            Bloodfest = 16164,
            Hypervelocity = 25759,
            LionHeart = 36939,
            NobleBlood = 36938,
            ReignOfBeasts = 36937,
            FatedBrand = 36936,
            LightningShot = 16143;

        public static class Buffs
        {
            public const ushort
                NoMercy = 1831,
                Aurora = 1835,
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844,
                ReadyToRaze = 3839,
                ReadyToBreak = 3886,
                ReadyToReign = 3840,
                ReadyToBlast = 2686;
        }

        public static class Debuffs
        {
            public const ushort
                BowShock = 1838,
                SonicBreak = 1837;
        }

        public static class Config
        {
            public const string
                GNB_VariantCure = "GNB_VariantCure";
        }

        internal class GNB_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6;
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    // Ranged Uptime
                    if (!InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    // No Mercy
                    if (ActionReady(NoMercy))
                    {
                        if (CanWeave(actionID))
                        {
                            if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && gauge.Ammo >= 2 && IsOffCooldown(NoMercy)) // Lv100 on CD use (2 or 3 cart, never 1)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 3) // Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2) // Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && IsOffCooldown(Bloodfest) && gauge.Ammo == 1 && quarterWeave) // <=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) > GCD * 12 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 2) && quarterWeave)) // <=Lv80 lateweave use
                                return NoMercy;
                        }
                    }

                    // <Lv30
                    if (!LevelChecked(BurstStrike) && quarterWeave)
                        return NoMercy;

                    // oGCDs
                    if (CanWeave(actionID))
                    {
                        // Bloodfest
                        if (ActionReady(Bloodfest) && gauge.Ammo is 0 && (JustUsed(NoMercy, 20f)))
                            return Bloodfest;

                        // Zone
                        if (ActionReady(DangerZone) && !JustUsed(NoMercy))
                        {
                            // Lv90
                            if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || // >=Lv60
                                !LevelChecked(GnashingFang))) // <Lv60
                                return OriginalHook(DangerZone);
                            // Lv100 use
                            if (LevelChecked(ReignOfBeasts) && (WasLastWeaponskill(DoubleDown) || GetCooldownRemainingTime(NoMercy) > 17))
                                return OriginalHook(DangerZone);
                        }

                        // Hypervelocity
                        if (WasLastWeaponskill(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                            return Hypervelocity;

                        // Continuation
                        if (LevelChecked(Continuation) &&
                            (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);

                        // 60s weaves
                        if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
                        {
                            // >=Lv90
                            if (ActionReady(BowShock) && LevelChecked(BowShock))
                                return BowShock;
                            if (ActionReady(DangerZone))
                                return OriginalHook(DangerZone);

                            // <Lv90
                            if (!LevelChecked(DoubleDown))
                            {
                                if (ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);
                                if (ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                            }
                        }
                    }

                    // GF combo
                    if (LevelChecked(Continuation) &&
                        (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    // Sonic Break special conditions 
                    if (GetBuffRemainingTime(Buffs.NoMercy) < GCD * 6 && HasEffect(Buffs.ReadyToBreak))
                    {
                        if (LevelChecked(ReignOfBeasts) || !LevelChecked(ReignOfBeasts))
                        {
                            // 1min 2cart
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && gauge.Ammo == 0 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 5 &&
                                (WasLastWeaponskill(GnashingFang) || WasLastAbility(SavageClaw)))
                                return SonicBreak;

                            // sks 9th GCD
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && !HasEffect(Buffs.ReadyToReign) && gauge.Ammo == 2 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD)
                                return SonicBreak;
                        }
                    }

                    // Double Down
                    if ((HasEffect(Buffs.NoMercy) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && gauge.Ammo >= 2)
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((WasLastWeaponskill(SonicBreak) && gauge.Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest)) // 2min
                                || (!HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && gauge.Ammo == 3) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SolidBarrel) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < 17))) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((gauge.Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest))) // 2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SonicBreak) && GetCooldownRemainingTime(Bloodfest) > GCD * 12) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) > GCD * 12)) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // <Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  // subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    // Sonic Break 
                    if (JustUsed(NoMercy, 20f) || (HasEffect(Buffs.NoMercy) && HasEffect(Buffs.ReadyToBreak)))
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 2 && (GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7) &&
                                WasLastWeaponskill(BurstStrike) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min
                                || (!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < GCD) &&
                                (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min 3 carts
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(NoMercy) > GCD &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) // 2min
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2 && GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7 &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // <Lv80
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if (!HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) < 58 && WasLastWeaponskill(GnashingFang))
                                return SonicBreak;
                        }
                    }

                    // Gnashing Fang
                    if (LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) <= 0.6f && gauge.Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && gauge.AmmoComboStep == 0
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && WasLastWeaponskill(DoubleDown)) // Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && WasLastWeaponskill(DoubleDown)) // Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) > GCD * 20 && WasLastWeaponskill(DoubleDown)) // Lv90 odd minute scuffed windows
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 4 && IsOffCooldown(Bloodfest)) // Opener/Reopener Conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) >= GCD * 24) // <Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && gauge.Ammo == 1 && GetCooldownRemainingTime(NoMercy) >= GCD * 24 && IsOffCooldown(Bloodfest)) // <Lv90 Opener/Reopener
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14)) // 30s use
                            return GnashingFang;
                    }

                    // Reign combo
                    if (LevelChecked(ReignOfBeasts))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && gauge.AmmoComboStep == 0 && GetCooldownRemainingTime(Bloodfest) > GCD * 12)
                        {
                            if (WasLastWeaponskill(WickedTalon) || (WasLastAbility(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (WasLastWeaponskill(ReignOfBeasts) || WasLastWeaponskill(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    // Burst Strike
                    if (LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            // Lv100 use
                            if ((LevelChecked(ReignOfBeasts)
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3
                                && !HasEffect(Buffs.ReadyToReign))
                                // subLv90 use
                                || (!LevelChecked(ReignOfBeasts)
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak)
                                && IsOnCooldown(DoubleDown) && IsOnCooldown(GnashingFang)))
                                return BurstStrike;
                        }
                    }

                    // Lv100 2cart 2min starter
                    if (LevelChecked(ReignOfBeasts)
                        && GetCooldownRemainingTime(NoMercy) <= GCD
                        && gauge.Ammo is 3
                        && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)))
                        return BurstStrike;

                    // GF combo
                    if (gauge.AmmoComboStep is 1 or 2) // GF
                        return OriginalHook(GnashingFang);

                    // 123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                                return Hypervelocity;
                            if (LevelChecked(BurstStrike) && gauge.Ammo == MaxCartridges(level))
                                return BurstStrike;
                            return SolidBarrel;
                        }
                        if (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell) && gauge.Ammo == 2)
                            return SolidBarrel;
                        if (!LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell || WasLastWeaponskill(BurstStrike)) && gauge.Ammo == 2)
                            return SolidBarrel;
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }

        internal class GNB_ST_Advanced : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6;
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    // Ranged Uptime
                    if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) && !InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    // No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(actionID))
                            {
                                if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv100 Opener/Reopener (0cart)
                                    || (LevelChecked(ReignOfBeasts) && gauge.Ammo >= 2 && IsOffCooldown(NoMercy)) // Lv100 on CD use (2 or 3 cart, never 1)
                                    || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv90 Opener/Reopener (0cart)
                                    || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 3) // Lv90 2min 3cart force
                                    || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2) // Lv90 1min 2 or 3cart
                                    || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && IsOffCooldown(Bloodfest) && gauge.Ammo == 1 && quarterWeave) // <=Lv80 Opener/Reopener (1cart)
                                    || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) > GCD * 12 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 2) && quarterWeave)) // <=Lv80 lateweave use
                                    return NoMercy;
                            }
                        }

                        // <Lv30
                        if (!LevelChecked(BurstStrike) && quarterWeave)
                            return NoMercy;
                    }

                    // oGCDs
                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup))
                        {
                            // Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) && ActionReady(Bloodfest) && gauge.Ammo is 0 && (JustUsed(NoMercy, 20f)))
                                    return Bloodfest;

                            // Zone
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !JustUsed(NoMercy))
                            {
                                // Lv90
                                if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || // >=Lv60
                                    !LevelChecked(GnashingFang))) // <Lv60
                                    return OriginalHook(DangerZone);
                                // Lv100 use
                                if (LevelChecked(ReignOfBeasts) && (WasLastWeaponskill(DoubleDown) || GetCooldownRemainingTime(NoMercy) > 17))
                                    return OriginalHook(DangerZone);
                            }

                            // Hypervelocity
                            if (WasLastWeaponskill(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                                return Hypervelocity;

                            // Continuation
                            if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(Continuation) &&
                                (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                                return OriginalHook(Continuation);

                            // 60s weaves
                            if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
                            {
                                // >=Lv90
                                if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                                if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);

                                // <Lv90
                                if (!LevelChecked(DoubleDown))
                                {
                                    if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone))
                                        return OriginalHook(DangerZone);
                                    if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                        return BowShock;
                                }
                            }
                        }
                    }

                    // GF combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(Continuation) &&
                        (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    // Sonic Break special conditions 
                    if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 6 && HasEffect(Buffs.ReadyToBreak))
                    {
                        if (LevelChecked(ReignOfBeasts) || !LevelChecked(ReignOfBeasts))
                        {
                            // 1min 2cart
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && gauge.Ammo == 0 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 5 &&
                                (WasLastWeaponskill(GnashingFang) || WasLastAbility(SavageClaw)))
                                return SonicBreak;

                            // sks 9th GCD
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && !HasEffect(Buffs.ReadyToReign) && gauge.Ammo == 2 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD)
                                return SonicBreak;
                        }
                    }

                    // Double Down
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && (HasEffect(Buffs.NoMercy) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && gauge.Ammo >= 2)
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts) && (IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f))
                        {
                            if ((WasLastWeaponskill(SonicBreak) && gauge.Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest)) // 2min
                                || (!HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && gauge.Ammo == 3) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SolidBarrel) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < 17))) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((gauge.Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest))) // 2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SonicBreak) && GetCooldownRemainingTime(Bloodfest) > GCD * 12) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) > GCD * 12)) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // <Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  // subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    // Sonic Break 
                    if (JustUsed(NoMercy, 20f) || (HasEffect(Buffs.NoMercy) && HasEffect(Buffs.ReadyToBreak)))
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                (!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 2 && (GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7) &&
                                WasLastWeaponskill(BurstStrike) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min
                                || (!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < GCD) &&
                                (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min 3 carts
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(NoMercy) > GCD &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                (!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) // 2min
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2 && GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7 &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // <Lv80
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                (!HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) < 58 && WasLastWeaponskill(GnashingFang)))
                                return SonicBreak;
                        }
                    }

                    // Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) <= 0.6f && gauge.Ammo > 0)
                    {
                        if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang_Starter) && !HasEffect(Buffs.ReadyToBlast) && gauge.AmmoComboStep == 0 
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && WasLastWeaponskill(DoubleDown)) // Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && WasLastWeaponskill(DoubleDown)) // Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) > GCD * 20 && WasLastWeaponskill(DoubleDown)) // Lv90 odd minute scuffed windows
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 4 && IsOffCooldown(Bloodfest)) // Opener/Reopener Conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) >= GCD * 24) // <Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && gauge.Ammo == 1 && GetCooldownRemainingTime(NoMercy) >= GCD * 24 && IsOffCooldown(Bloodfest)) // <Lv90 Opener/Reopener
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14)) // 30s use
                            return GnashingFang;
                    }

                    // Reign combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && gauge.AmmoComboStep == 0 && GetCooldownRemainingTime(Bloodfest) > GCD * 12)
                        {
                            if (WasLastWeaponskill(WickedTalon) || (WasLastAbility(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (WasLastWeaponskill(ReignOfBeasts) || WasLastWeaponskill(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    // Burst Strike
                    if (IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                                // Lv100 use
                            if ((LevelChecked(ReignOfBeasts) 
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3
                                && !HasEffect(Buffs.ReadyToReign))
                                // subLv90 use
                                || (!LevelChecked(ReignOfBeasts)
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak)
                                && IsOnCooldown(DoubleDown) && IsOnCooldown(GnashingFang)))
                                return BurstStrike;
                        }
                    }

                    // Lv100 2cart 2min starter
                    if (LevelChecked(ReignOfBeasts)
                        && GetCooldownRemainingTime(NoMercy) <= GCD
                        && gauge.Ammo is 3
                        && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)))
                        return BurstStrike;

                    // GF combo
                    if (gauge.AmmoComboStep is 1 or 2) // GF
                        return OriginalHook(GnashingFang);

                    // 123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                                return Hypervelocity;
                            if (LevelChecked(BurstStrike) && gauge.Ammo == MaxCartridges(level))
                                return BurstStrike;
                            return SolidBarrel;
                        }
                        if (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell) && gauge.Ammo == 2)
                            return SolidBarrel;
                        if (!LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell || WasLastWeaponskill(BurstStrike)) && gauge.Ammo == 2)
                            return SolidBarrel;
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }

        internal class GNB_GF_Continuation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_GF_Continuation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == GnashingFang)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6;
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    // No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && ActionReady(NoMercy) &&
                        (LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv100 Opener/Reopener (0cart)
                        || (LevelChecked(ReignOfBeasts) && gauge.Ammo >= 2 && IsOffCooldown(NoMercy)) // Lv100 on CD use (2 or 3 cart, never 1)
                        || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && gauge.Ammo == 0 && lastComboMove is BrutalShell && IsOffCooldown(Bloodfest)) // Lv90 Opener/Reopener (0cart)
                        || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 3) // Lv90 2min 3cart force
                        || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2) // Lv90 1min 2 or 3cart
                        || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && IsOffCooldown(Bloodfest) && gauge.Ammo == 1 && quarterWeave) // subLv80 Opener/Reopener (1cart)
                        || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) > GCD * 12 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 2) && quarterWeave)) // subLv80 lateweave use
                        return NoMercy;

                    // oGCDs
                    if (CanWeave(SavageClaw))
                    {
                        if (IsEnabled(CustomComboPreset.GNB_GF_Cooldowns))
                        {
                            // Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && ActionReady(Bloodfest) && 
                                gauge.Ammo is 0 && JustUsed(NoMercy, 20f))
                                    return Bloodfest;

                            // Zone
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone) && !JustUsed(NoMercy))
                            {
                                // Lv90
                                if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || // >=Lv60
                                    !LevelChecked(GnashingFang))) // <Lv60
                                    return OriginalHook(DangerZone);
                                // Lv100
                                if (LevelChecked(ReignOfBeasts) && (WasLastWeaponskill(DoubleDown) || IsOnCooldown(DoubleDown) || GetCooldownRemainingTime(NoMercy) > 17))
                                    return OriginalHook(DangerZone);
                            }

                            // Hypervelocity
                            if (WasLastWeaponskill(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                                return Hypervelocity;

                            // Continuation
                            if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                                return OriginalHook(Continuation);

                            // Zone & BowShock weaves
                            if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
                            {
                                // >=Lv90
                                if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                                if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);

                                // <Lv90
                                if (!LevelChecked(DoubleDown))
                                {
                                    if (IsEnabled(CustomComboPreset.GNB_GF_Zone) &&ActionReady(DangerZone))
                                        return OriginalHook(DangerZone);
                                    if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                        return BowShock;
                                }
                            }
                        }
                    }

                    // Sonic Break special conditions
                    if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 6 && HasEffect(Buffs.ReadyToBreak))
                    {
                        if (LevelChecked(ReignOfBeasts) || !LevelChecked(ReignOfBeasts))
                        {
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && gauge.Ammo == 0 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 5 &&
                                (WasLastWeaponskill(GnashingFang) || WasLastAbility(SavageClaw)))
                                return SonicBreak;

                            // sks 9th GCD
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && !HasEffect(Buffs.ReadyToReign) && gauge.Ammo == 2 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD)
                                return SonicBreak;
                        }
                    }

                    // Double Down
                    if (IsEnabled(CustomComboPreset.GNB_GF_DoubleDown) && (HasEffect(Buffs.NoMercy) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && gauge.Ammo >= 2)
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((WasLastWeaponskill(SonicBreak) && gauge.Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest)) // 2min NM
                                || (!HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && gauge.Ammo == 3) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SolidBarrel) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < 17))) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((gauge.Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest))) // 2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SonicBreak) && GetCooldownRemainingTime(Bloodfest) > GCD * 12) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) > GCD * 12)) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // <Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_GF_Cooldowns) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  // subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    // Sonic Break 
                    if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && (JustUsed(NoMercy) || (HasEffect(Buffs.NoMercy) && HasEffect(Buffs.ReadyToBreak))))
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 2 && (GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7) &&
                                WasLastWeaponskill(BurstStrike) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min
                                || (!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < GCD) &&
                                (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest))) // 2min 3 carts
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo == 3 && GetCooldownRemainingTime(NoMercy) > GCD &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || IsOffCooldown(Bloodfest)) // 2min
                                || (GetCooldownRemainingTime(Bloodfest) > GCD * 12 && gauge.Ammo >= 2 && GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7 &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel)))) // 1min 3 carts
                                return SonicBreak;
                        }

                        // <Lv90
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) < 58 && WasLastWeaponskill(GnashingFang)))
                                return SonicBreak;
                        }
                    }

                    // Reign combo
                    if (IsEnabled(CustomComboPreset.GNB_GF_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && gauge.AmmoComboStep == 0 && GetCooldownRemainingTime(Bloodfest) > GCD * 12)
                        {
                            if (WasLastWeaponskill(WickedTalon) || (WasLastAbility(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (WasLastWeaponskill(ReignOfBeasts) || WasLastWeaponskill(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.GNB_GF_BurstStrike))
                    {
                        if ((LevelChecked(ReignOfBeasts) // Lv100
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && !HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3
                                && ((HasEffect(Buffs.NoMercy)) || (!HasEffect(Buffs.NoMercy) && gauge.Ammo == 3 && ComboAction == BrutalShell)))
                                // subLv90 use
                                || (!LevelChecked(ReignOfBeasts) // <=Lv90
                                && gauge.Ammo == MaxCartridges(level) && gauge.AmmoComboStep == 0
                                && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak)
                                && IsOnCooldown(DoubleDown) && IsOnCooldown(GnashingFang)))
                            return BurstStrike;
                        if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                            return Hypervelocity;
                    }
                }

                return actionID;
            }
        }


        internal class GNB_BS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_BS;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is BurstStrike)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && HasEffect(Buffs.ReadyToBlast) && LevelChecked(Hypervelocity))
                        return Hypervelocity;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && gauge.Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(Bloodfest) < 0.6f)
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && HasEffect(Buffs.NoMercy) && GetCooldownRemainingTime(DoubleDown) < 2 && gauge.Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && gauge.AmmoComboStep == 0 && GetCooldownRemainingTime(Bloodfest) > GCD * 12)
                        {
                            if (WasLastWeaponskill(WickedTalon) || (WasLastAbility(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (WasLastWeaponskill(ReignOfBeasts) || WasLastWeaponskill(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }
                }

                return actionID;
            }
        }

        internal class GNB_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID == DemonSlice)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    if (InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (ActionReady(NoMercy))
                                return NoMercy;
                            if (ActionReady(BowShock) && LevelChecked(BowShock) && HasEffect(Buffs.NoMercy))
                                return BowShock;
                            if (ActionReady(DangerZone) && (HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(GnashingFang) <= GCD * 7))
                                return OriginalHook(DangerZone);
                            if (gauge.Ammo == 0 && ActionReady(Bloodfest) && LevelChecked(Bloodfest) && HasEffect(Buffs.NoMercy))
                                return Bloodfest;
                            if (LevelChecked(FatedBrand) && HasEffect(Buffs.ReadyToRaze) && WasLastWeaponskill(FatedCircle) && LevelChecked(FatedBrand))
                                return FatedBrand;
                        }

                        if (HasEffect(Buffs.ReadyToBreak) && !HasEffect(Buffs.ReadyToRaze) && HasEffect(Buffs.NoMercy))
                            return SonicBreak;
                        if (gauge.Ammo >= 2 && ActionReady(DoubleDown) && HasEffect(Buffs.NoMercy))
                            return DoubleDown;
                        if (gauge.Ammo != 0 && GetCooldownRemainingTime(Bloodfest) < 6 && LevelChecked(FatedCircle))
                            return FatedCircle;
                    }

                    if (comboTime > 0 && lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                    {
                        return (LevelChecked(FatedCircle) && gauge.Ammo == MaxCartridges(level)) ? FatedCircle : DemonSlaughter;
                    }

                    return DemonSlice;
                }

                return actionID;
            }
        }

        internal class GNB_AoE_Advanced : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID == DemonSlice)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.5 supported, 2.45 is iffy

                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.GNB_AoE_NoMercy) && ActionReady(NoMercy))
                                return NoMercy;
                            if (IsEnabled(CustomComboPreset.GNB_AoE_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock) && HasEffect(Buffs.NoMercy))
                                return BowShock;
                            if (IsEnabled(CustomComboPreset.GNB_AOE_DangerZone) && ActionReady(DangerZone) && (HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(GnashingFang) <= GCD * 7))
                                return OriginalHook(DangerZone);
                            if (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && gauge.Ammo == 0 && ActionReady(Bloodfest) && LevelChecked(Bloodfest) && HasEffect(Buffs.NoMercy))
                                return Bloodfest;
                            if (LevelChecked(FatedBrand) && HasEffect(Buffs.ReadyToRaze) && WasLastWeaponskill(FatedCircle) && LevelChecked(FatedBrand))
                                return FatedBrand;
                        }

                        if (IsEnabled(CustomComboPreset.GNB_AOE_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && !HasEffect(Buffs.ReadyToRaze) && HasEffect(Buffs.NoMercy))
                            return SonicBreak;
                        if (IsEnabled(CustomComboPreset.GNB_AoE_DoubleDown) && gauge.Ammo >= 2 && ActionReady(DoubleDown) && HasEffect(Buffs.NoMercy))
                            return DoubleDown;
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && gauge.Ammo != 0 && GetCooldownRemainingTime(Bloodfest) < 6 && LevelChecked(FatedCircle))
                            return FatedCircle;
                    }

                    if (comboTime > 0 && lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                    {
                        return (IsEnabled(CustomComboPreset.GNB_AOE_Overcap) && LevelChecked(FatedCircle) && gauge.Ammo == MaxCartridges(level)) ? FatedCircle : DemonSlaughter;
                    }

                    return DemonSlice;
                }

                return actionID;
            }
        }

        internal class GNB_FC : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_FC;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FatedCircle)
                {
                    var gauge = GetJobGauge<GNBGauge>();

                    if (IsEnabled(CustomComboPreset.GNB_FC_Continuation) && HasEffect(Buffs.ReadyToRaze) && LevelChecked(FatedBrand) && CanWeave(actionID))
                        return FatedBrand;
                    if (IsEnabled(CustomComboPreset.GNB_FC_Bloodfest) && gauge.Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToRaze))
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && GetCooldownRemainingTime(DoubleDown) < 2 && gauge.Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                }

                return actionID;
            }
        }

        internal class GNB_NoMercy_Cooldowns : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_NoMercy_Cooldowns;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NoMercy)
                {
                    var gauge = GetJobGauge<GNBGauge>().Ammo;
                    if (IsOnCooldown(NoMercy) && InCombat())
                    {
                        if (IsEnabled(CustomComboPreset.GNB_NoMercy_Cooldowns_DD) && IsOffCooldown(DoubleDown) && gauge >= 2 && LevelChecked(DoubleDown))
                            return DoubleDown;
                        if (IsEnabled(CustomComboPreset.GNB_NoMercy_Cooldowns_SonicBreakBowShock))
                        {
                            if (HasEffect(Buffs.ReadyToBreak))
                                return SonicBreak;
                            if (IsOffCooldown(BowShock))
                                return BowShock;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class GNB_AuroraProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AuroraProtection;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Aurora)
                {
                    if ((HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora)) || (!HasFriendlyTarget() && HasEffectAny(Buffs.Aurora)))
                        return OriginalHook(11);
                }
                return actionID;
            }
        }
    }
}