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

        internal class GNB_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6;
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // GCD is 2.5sks only

                    // Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    // Ranged option
                    if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) && !InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    // No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_ST_MainCombo_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(actionID))
                            {
                                int minutes = CombatEngageDuration().Minutes;

                                if ((CombatEngageDuration().TotalSeconds < 30 && lastComboMove is BrutalShell) // Opener
                                    || (LevelChecked(ReignOfBeasts) && gauge.Ammo >= 2 && IsOffCooldown(NoMercy)) // Lv100 on CD use
                                    || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && minutes % 2 == 1 && gauge.Ammo >= 2 && IsOffCooldown(NoMercy)) // Lv90 1min On CD use
                                    || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) < 30 || IsOffCooldown(Bloodfest)) && gauge.Ammo == 3) // Lv90 2min 3cart force
                                    || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) < 30 || IsOffCooldown(Bloodfest)) && gauge.Ammo >= 1)) // subLv80 ON CD use
                                    return NoMercy;
                            }
                        }

                        // sub-Lv30
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

                        if (IsEnabled(CustomComboPreset.GNB_ST_MainCombo_CooldownsGroup))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) && ActionReady(Bloodfest) && gauge.Ammo is 0 && HasEffect(Buffs.NoMercy))
                            {
                                if (IsOnCooldown(NoMercy))
                                    return Bloodfest;
                            }

                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone))
                            {
                                // Zone outside of NM
                                if (!HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || // Post GF
                                    !LevelChecked(GnashingFang))) // Pre GF
                                    return OriginalHook(DangerZone);

                                // Lv100 use
                                if (HasEffect(Buffs.NoMercy) && LevelChecked(DoubleDown) && WasLastWeaponskill(DoubleDown))
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
                                // Post DD
                                if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                                if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);

                                // Pre DD
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

                    // GF combo usage
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(Continuation) &&
                        (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    // more Sonic Break 1min 2cart 
                    if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 6 && HasEffect(Buffs.ReadyToBreak))
                    {
                        if (LevelChecked(ReignOfBeasts) || !LevelChecked(ReignOfBeasts))
                        {
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && gauge.Ammo == 0 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD * 5 &&
                                (WasLastWeaponskill(GnashingFang) || WasLastAbility(SavageClaw)))
                                return SonicBreak;

                            // 2.45?
                            if (GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && !HasEffect(Buffs.ReadyToReign) && gauge.Ammo == 2 &&
                                !HasEffect(Buffs.ReadyToRip) && GetBuffRemainingTime(Buffs.NoMercy) < GCD)
                                return SonicBreak;
                        }
                    }

                    // Double Down
                    if ((HasEffect(Buffs.NoMercy) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && IsEnabled(CustomComboPreset.GNB_ST_MainCombo_CooldownsGroup))
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts) && (IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 1))
                        {
                            if ((WasLastWeaponskill(SonicBreak) && gauge.Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest)) // 2min NM
                                || (!HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && gauge.Ammo == 3) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SolidBarrel) && gauge.Ammo == 3 && (GetBuffRemainingTime(Buffs.NoMercy) < 17))) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD)
                        {
                            if ((gauge.Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && WasLastWeaponskill(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || IsOffCooldown(Bloodfest))) // 2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SonicBreak) && GetCooldownRemainingTime(Bloodfest) > GCD * 12) // 1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && gauge.Ammo == 3 && WasLastWeaponskill(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) > GCD * 12)) // 1min NM 2 carts
                                return DoubleDown;
                        }

                        // subLv80
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  // subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    // Sonic Break 
                    if (HasEffect(Buffs.NoMercy) && HasEffect(Buffs.ReadyToBreak))
                    {
                        // Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            // 2min
                            if ((IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                !HasEffect(Buffs.ReadyToBlast) && gauge.Ammo >= 2 && (GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7) &&
                                WasLastWeaponskill(BurstStrike) && (GetCooldownRemainingTime(Bloodfest) < 30 || IsOffCooldown(Bloodfest)))
                                // 1min 3 carts
                                || (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                GetCooldownRemainingTime(Bloodfest) > 30 && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(NoMercy) > GCD &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel))))
                                return SonicBreak;
                        }

                        // Lv90 & below
                        if (!LevelChecked(ReignOfBeasts))
                        {
                            // 2min
                            if ((IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                !HasEffect(Buffs.ReadyToBlast) && gauge.Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < 30 || IsOffCooldown(Bloodfest))
                                // 1min 3 carts
                                || (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                                GetCooldownRemainingTime(Bloodfest) > 30 && gauge.Ammo == 3 && GetBuffRemainingTime(Buffs.NoMercy) > GCD * 7 &&
                                (WasLastWeaponskill(KeenEdge) || WasLastWeaponskill(BrutalShell) || WasLastWeaponskill(SolidBarrel))))
                                return SonicBreak;
                        }
                    }

                    // Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(GnashingFang))
                    {
                        if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang_Starter) && GetCooldownRemainingTime(GnashingFang) <= 0.5 && !HasEffect(Buffs.ReadyToBlast) && gauge.AmmoComboStep == 0 &&
                            ((HasEffect(Buffs.NoMercy) && WasLastWeaponskill(DoubleDown)) // 60s use; DD>GF (as of 7.0  DT)
                            || (gauge.Ammo == 1 && HasEffect(Buffs.NoMercy) && GetCooldownRemainingTime(DoubleDown) > GCD * 4) //NMDDGF windows/Scuffed windows
                            || (gauge.Ammo > 0 && GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14) // 30s use                                                                    
                            || (gauge.Ammo == 1 && GetCooldownRemainingTime(NoMercy) > GCD * 4 && ((IsOffCooldown(Bloodfest) && LevelChecked(Bloodfest)) || !LevelChecked(Bloodfest))))) // Opener Conditions
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
                    if (IsEnabled(CustomComboPreset.GNB_ST_BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (!LevelChecked(ReignOfBeasts)
                                && gauge.Ammo >= 1 && gauge.AmmoComboStep == 0
                                && GetCooldownRemainingTime(NoMercy) <= GCD * 3
                                && !HasEffect(Buffs.ReadyToReign) && !HasEffect(Buffs.ReadyToBreak)
                                && (GetCooldownRemainingTime(Bloodfest) > 60))
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
                        if (HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike) && lastComboMove is BrutalShell && gauge.Ammo == 2)
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
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // GCD is 2.5sks only

                    if (IsOffCooldown(NoMercy) && IsOffCooldown(GnashingFang) && IsEnabled(CustomComboPreset.GNB_GF_NoMercy))
                        return NoMercy;

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.GNB_GF_Cooldowns))
                        {
                            if (ActionReady(Bloodfest) && gauge.Ammo is 0 && HasEffect(Buffs.NoMercy) && IsOnCooldown(NoMercy))
                                return Bloodfest;

                            if (ActionReady(DangerZone))
                            {
                                // Zone outside of NM
                                if (!HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || // Post GF
                                    !LevelChecked(GnashingFang))) // Pre GF
                                    return OriginalHook(DangerZone);

                                // stops Zone drift
                                if (HasEffect(Buffs.NoMercy))
                                    return OriginalHook(DangerZone);
                            }

                            // Continuation
                            if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                                return OriginalHook(Continuation);

                            // 60s weaves
                            if (HasEffect(Buffs.NoMercy))
                            {
                                // Post DD
                                if (ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);
                                if (ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;

                                // Pre DD
                                if (!LevelChecked(DoubleDown))
                                {
                                    if (ActionReady(BowShock) && LevelChecked(BowShock))
                                        return BowShock;
                                    if (ActionReady(DangerZone))
                                        return OriginalHook(DangerZone);
                                }
                            }
                        }

                        if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);
                    }

                    // 60s window features
                    if (GetCooldownRemainingTime(NoMercy) > 57 || HasEffect(Buffs.NoMercy))
                    {
                        if (LevelChecked(DoubleDown) && GetCooldownRemainingTime(GnashingFang) > 20)
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && !HasEffect(Buffs.ReadyToRip) && gauge.AmmoComboStep >= 1)
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && IsOffCooldown(DoubleDown) && gauge.Ammo >= 2)
                                return DoubleDown;
                        }

                        if (!LevelChecked(DoubleDown) && IsEnabled(CustomComboPreset.GNB_GF_Cooldowns))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && ActionReady(SonicBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= 10 && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang)))
                                return SonicBreak;
                            // subLv54
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak))
                                return OriginalHook(DangerZone);
                        }
                    }

                    if ((gauge.AmmoComboStep == 0 && IsOffCooldown(GnashingFang)) || gauge.AmmoComboStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    if (IsEnabled(CustomComboPreset.GNB_GF_Cooldowns))
                    {
                        if (HasEffect(Buffs.NoMercy) && gauge.AmmoComboStep == 0 && LevelChecked(BurstStrike))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast))
                                return Hypervelocity;
                            if (gauge.Ammo != 0 && GetCooldownRemainingTime(GnashingFang) > 4)
                                return BurstStrike;
                        }

                        //final check if Burst Strike is used right before No Mercy ends
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
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // GCD is 2.5sks only

                    if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && HasEffect(Buffs.ReadyToBlast) && LevelChecked(Hypervelocity))
                        return Hypervelocity;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && gauge.Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToBlast))
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && (LevelChecked(ReignOfBeasts)))
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
                    if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && HasEffect(Buffs.NoMercy) && GetCooldownRemainingTime(DoubleDown) < 2 && gauge.Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                }

                return actionID;
            }
        }

        internal class GNB_AoE_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID == DemonSlice)
                {
                    var gauge = GetJobGauge<GNBGauge>();
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; // GCD is 2.5sks only

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