using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;

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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (!InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    //No Mercy
                    if (ActionReady(NoMercy))
                    {
                        if (CanWeave(actionID))
                        {
                            if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3) //Lv100 1min
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2) //Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave))) //<=Lv80 lateweave use
                                return NoMercy;
                            //<Lv30
                            if (!LevelChecked(BurstStrike) && quarterWeave)
                                return NoMercy;
                        }
                    }

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Variant Spirit Dart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Bloodfest
                        if (ActionReady(Bloodfest) && Ammo is 0 && (JustUsed(NoMercy, 20f)))
                            return Bloodfest;

                        //Zone
                        if (ActionReady(DangerZone) && !JustUsed(NoMercy))
                        {
                            //Lv90
                            if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || //>=Lv60
                                !LevelChecked(GnashingFang))) //<Lv60
                                return OriginalHook(DangerZone);
                            //Lv100 use
                            if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || GetCooldownRemainingTime(NoMercy) > 17))
                                return OriginalHook(DangerZone);
                        }

                        //Continuation
                        if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);

                        //60s weaves
                        if (HasEffect(Buffs.NoMercy) && (!JustUsed(NoMercy, 3f)))
                        {
                            //>=Lv90
                            if (ActionReady(BowShock) && LevelChecked(BowShock))
                                return BowShock;
                            if (ActionReady(DangerZone))
                                return OriginalHook(DangerZone);

                            //<Lv90
                            if (!LevelChecked(DoubleDown))
                            {
                                if (ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);
                                if (ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                            }
                        }
                    }

                    //Hypervelocity
                    if (JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) > 1)
                        return Hypervelocity;

                    //GF combo
                    if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && GetCooldownRemainingTime(Bloodfest) > GCD * 15 && !ActionReady(DoubleDown) && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2 &&
                                (JustUsed(KeenEdge) || JustUsed(BrutalShell) || JustUsed(SolidBarrel)))) //1min 3 carts
                                return SonicBreak;
                        }

                        //<Lv80
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if (!HasEffect(Buffs.ReadyToBlast) && JustUsed(GnashingFang, 3f))
                                return SonicBreak;
                        }
                    }

                    //Double Down
                    if ((JustUsed(NoMercy, 20f) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && Ammo >= 2)
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15)) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //<Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //Gnashing Fang
                    if (LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) <= 0.6f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) > GCD * 20 && JustUsed(DoubleDown, 3f)) //Lv90 odd minute scuffed windows
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 4 && ActionReady(Bloodfest)) //Opener/Reopener Conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) >= GCD * 24) //<Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && GetCooldownRemainingTime(NoMercy) >= GCD * 24 && ActionReady(Bloodfest)) //<Lv90 Opener/Reopener
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //Reign combo
                    if ((LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    //Burst Strike
                    if (LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (GetCooldownRemainingTime(DoubleDown) > GCD * 3 &&
                                ((LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (LevelChecked(ReignOfBeasts) && (GetCooldownRemainingTime(NoMercy) <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)))
                        return BurstStrike;

                    //GF combo safety net
                    if (GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    //123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) > 1) //Lv100 Hypervelocity fit into NM check
                                return Hypervelocity;
                            if (LevelChecked(BurstStrike) && Ammo == MaxCartridges(level))
                                return BurstStrike;
                            return SolidBarrel;
                        }
                        if (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && GunStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell) && Ammo == 2)
                            return SolidBarrel;
                        if (!LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && GunStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell || JustUsed(BurstStrike)) && Ammo == 2)
                            return SolidBarrel;
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }

        internal class GNB_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) && 
                        !InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    //No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(actionID))
                            {
                                if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3) //Lv100 1min
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2) //Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave))) //<=Lv80 lateweave use
                                    return NoMercy;
                            }
                        }

                        //<Lv30
                        if (!LevelChecked(BurstStrike) && quarterWeave)
                            return NoMercy;
                    }

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Variant Spirit Dart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //CDs
                        if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup))
                        {
                            //Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) && ActionReady(Bloodfest) && Ammo is 0 && (JustUsed(NoMercy, 20f)))
                                    return Bloodfest;

                            //Zone
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !JustUsed(NoMercy))
                            {
                                //Lv90
                                if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || //>=Lv60
                                    !LevelChecked(GnashingFang))) //<Lv60
                                    return OriginalHook(DangerZone);
                                //Lv100 use
                                if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || GetCooldownRemainingTime(NoMercy) > 17))
                                    return OriginalHook(DangerZone);
                            }

                            //Continuation
                            if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                                return OriginalHook(Continuation);

                            //60s weaves
                            if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
                            {
                                //>=Lv90
                                if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                                if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);

                                //<Lv90
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

                    //Hypervelocity
                    if (JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) > 1)
                        return Hypervelocity;

                    //GF combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && !ActionReady(DoubleDown) && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2 &&
                                (JustUsed(KeenEdge) || JustUsed(BrutalShell) || JustUsed(SolidBarrel)))) //1min 3 carts
                                return SonicBreak;
                        }

                        //<Lv80
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if (!HasEffect(Buffs.ReadyToBlast) && JustUsed(GnashingFang, 3f))
                                return SonicBreak;
                        }
                    }

                    //Double Down
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) &&
                        (JustUsed(NoMercy, 20f) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && Ammo >= 2)
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15)) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //<Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang_Starter) && LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) <= 0.6f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0 
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) > GCD * 20 && JustUsed(DoubleDown)) //Lv90 odd minute scuffed windows
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 4 && ActionReady(Bloodfest)) //Opener/Reopener Conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) >= GCD * 24) //<Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && GetCooldownRemainingTime(NoMercy) >= GCD * 24 && ActionReady(Bloodfest)) //<Lv90 Opener/Reopener
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //Reign combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    //Burst Strike
                    if (IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (GetCooldownRemainingTime(DoubleDown) > GCD * 3 &&
                                ((LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (LevelChecked(ReignOfBeasts) && (GetCooldownRemainingTime(NoMercy) <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)))
                        return BurstStrike;

                    //GF combo safety net
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    //123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) > 1) //Lv100 Hypervelocity fit into NM check
                                return Hypervelocity;
                            if (LevelChecked(BurstStrike) && Ammo == MaxCartridges(level))
                                return BurstStrike;
                            return SolidBarrel;
                        }
                        if (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && GunStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell) && Ammo == 2)
                            return SolidBarrel;
                        if (!LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && GunStep == 0 && LevelChecked(BurstStrike) && (lastComboMove is BrutalShell || JustUsed(BurstStrike)) && Ammo == 2)
                            return SolidBarrel;
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }

        internal class GNB_GF_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_GF_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    //No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_GF_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(actionID))
                            {
                                if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3) //Lv100 1min
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2) //Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 || (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave))) //<=Lv80 lateweave use
                                    return NoMercy;
                            }
                        }

                        //<Lv30
                        if (!LevelChecked(BurstStrike) && quarterWeave)
                            return NoMercy;
                    }

                    //oGCDs
                    if (CanWeave(SavageClaw))
                    {
                        //Bloodfest
                        if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && ActionReady(Bloodfest) && Ammo is 0 && (JustUsed(NoMercy, 20f)))
                            return Bloodfest;

                        //Zone
                        if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone) && !JustUsed(NoMercy))
                        {
                            //Lv90
                            if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((IsOnCooldown(GnashingFang) && GetCooldownRemainingTime(NoMercy) > 17) || //>=Lv60
                                !LevelChecked(GnashingFang))) //<Lv60
                                return OriginalHook(DangerZone);
                            //Lv100 use
                            if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || GetCooldownRemainingTime(NoMercy) > 17))
                                return OriginalHook(DangerZone);
                        }

                        //60s weaves
                        if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
                        {
                            //>=Lv90
                            if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                return BowShock;
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone))
                                return OriginalHook(DangerZone);

                            //<Lv90
                            if (!LevelChecked(DoubleDown))
                            {
                                if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone))
                                    return OriginalHook(DangerZone);
                                if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock))
                                    return BowShock;
                            }
                        }
                    }

                    //Hypervelocity
                    if (JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(NoMercy) > 1)
                        return Hypervelocity;

                    //GF combo
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && GetCooldownRemainingTime(Bloodfest) > GCD * 14 && GetCooldownRemainingTime(DoubleDown) > GCD * 14 && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 &&
                                GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (GetCooldownRemainingTime(Bloodfest) is < 90 and > 15 && Ammo >= 2 &&
                                (JustUsed(KeenEdge) || JustUsed(BrutalShell) || JustUsed(SolidBarrel)))) //1min 3 carts
                                return SonicBreak;
                        }

                        //<Lv80
                        if (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown))
                        {
                            if (!HasEffect(Buffs.ReadyToBlast) && JustUsed(GnashingFang, 3f))
                                return SonicBreak;
                        }
                    }

                    //Double Down
                    if (IsEnabled(CustomComboPreset.GNB_GF_DoubleDown) &&
                        (JustUsed(NoMercy, 20f) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && Ammo >= 2)
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && Ammo == 2 && GetCooldownRemainingTime(Bloodfest) < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo == 3 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak) && (GetCooldownRemainingTime(Bloodfest) < GCD * 4 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel) && GetCooldownRemainingTime(Bloodfest) is < 90 and > 15)) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //<Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && IsOnCooldown(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(NoMercy) < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) <= 0.6f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) > GCD * 20 && JustUsed(DoubleDown)) //Lv90 odd minute scuffed windows
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 4 && ActionReady(Bloodfest)) //Opener/Reopener Conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && GetCooldownRemainingTime(NoMercy) >= GCD * 24) //<Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && GetCooldownRemainingTime(NoMercy) >= GCD * 24 && ActionReady(Bloodfest)) //<Lv90 Opener/Reopener
                            || (GetCooldownRemainingTime(NoMercy) > GCD * 7 && GetCooldownRemainingTime(NoMercy) < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //Reign combo
                    if (IsEnabled(CustomComboPreset.GNB_GF_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    //Burst Strike
                    if (IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (GetCooldownRemainingTime(DoubleDown) > GCD * 3 &&
                                ((LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && Ammo >= 1 && GunStep == 0 && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && LevelChecked(ReignOfBeasts) && (GetCooldownRemainingTime(NoMercy) <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (GetCooldownRemainingTime(Bloodfest) < GCD * 12 || ActionReady(Bloodfest)))
                        return BurstStrike;

                    //GF combo safety net
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    return KeenEdge;
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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            //Variant SpiritDart
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            //Variant Ultimatum
                            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            //NoMercy
                            if (ActionReady(NoMercy)) //use on CD
                                return NoMercy;
                            //BowShock
                            if (ActionReady(BowShock) && LevelChecked(BowShock) && HasEffect(Buffs.NoMercy)) //use on CD under NM
                                return BowShock;
                            //Zone
                            if (ActionReady(DangerZone) && (HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(GnashingFang) <= GCD * 7)) //use on CD after first usage in NM
                                return OriginalHook(DangerZone);
                            //Bloodfest
                            if (Ammo == 0 && ActionReady(Bloodfest) && LevelChecked(Bloodfest) && HasEffect(Buffs.NoMercy)) //use when Ammo is 0 in burst
                                return Bloodfest;
                            //Continuation
                            if (LevelChecked(FatedBrand) && HasEffect(Buffs.ReadyToRaze) && JustUsed(FatedCircle) && LevelChecked(FatedBrand)) //FatedCircle weave
                                return FatedBrand;
                        }

                        //SonicBreak
                        if (HasEffect(Buffs.ReadyToBreak) && !HasEffect(Buffs.ReadyToRaze) && HasEffect(Buffs.NoMercy)) //use on CD
                            return SonicBreak;
                        //DoubleDown
                        if (        Ammo >= 2 && ActionReady(DoubleDown) && HasEffect(Buffs.NoMercy)) //use on CD under NM
                            return DoubleDown;
                        //FatedCircle
                        if ((HasEffect(Buffs.NoMercy) && !ActionReady(DoubleDown) && GunStep == 0) || //use when under NM after DD & ignores GF
                            (Ammo > 0 && GetCooldownRemainingTime(Bloodfest) < 6 && LevelChecked(FatedCircle))) // Bloodfest prep
                            return FatedCircle;
                        //Reign
                        if (LevelChecked(ReignOfBeasts)) //because leaving this out anywhere is a waste
                        {
                            if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(DoubleDown) && GunStep == 0)
                            {
                                if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                    return OriginalHook(ReignOfBeasts);
                            }

                            if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                            {
                                return OriginalHook(ReignOfBeasts);
                            }
                        }
                    }

                    //1-2
                    if (comboTime > 0 && lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                    {
                        return (LevelChecked(FatedCircle) && Ammo == MaxCartridges(level)) ? FatedCircle : DemonSlaughter;
                    }

                    return DemonSlice;
                }

                return actionID;
            }
        }

        internal class GNB_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DemonSlice)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            //Variant SpiritDart
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;
                            
                            //Variant Ultimatum
                            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            //NoMercy
                            if (IsEnabled(CustomComboPreset.GNB_AoE_NoMercy) && ActionReady(NoMercy)) //use on CD
                                return NoMercy;
                            //BowShock
                            if (IsEnabled(CustomComboPreset.GNB_AoE_BowShock) && ActionReady(BowShock) && LevelChecked(BowShock) && HasEffect(Buffs.NoMercy)) //use on CD under NM
                                return BowShock;
                            //Zone
                            if (IsEnabled(CustomComboPreset.GNB_AoE_DangerZone) && ActionReady(DangerZone) && (HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(GnashingFang) <= GCD * 7)) //use on CD after first usage in NM
                                return OriginalHook(DangerZone);
                            //Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && Ammo == 0 && ActionReady(Bloodfest) && LevelChecked(Bloodfest) && HasEffect(Buffs.NoMercy)) //use when Ammo is 0 in burst
                                return Bloodfest;
                            //Continuation
                            if (LevelChecked(FatedBrand) && HasEffect(Buffs.ReadyToRaze) && JustUsed(FatedCircle) && LevelChecked(FatedBrand)) //FatedCircle weave
                                return FatedBrand;
                        }

                        //SonicBreak
                        if (IsEnabled(CustomComboPreset.GNB_AoE_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && !HasEffect(Buffs.ReadyToRaze) && HasEffect(Buffs.NoMercy)) //use on CD
                            return SonicBreak;
                        //DoubleDown
                        if (IsEnabled(CustomComboPreset.GNB_AoE_DoubleDown) && Ammo >= 2 && ActionReady(DoubleDown) && HasEffect(Buffs.NoMercy)) //use on CD under NM
                            return DoubleDown;
                        //FatedCircle
                        if ((IsEnabled(CustomComboPreset.GNB_AoE_FatedCircle) && HasEffect(Buffs.NoMercy) && !ActionReady(DoubleDown) && GunStep == 0) || //use when under NM after DD & ignores GF
                            (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && Ammo > 0 && GetCooldownRemainingTime(Bloodfest) < 6 && LevelChecked(FatedCircle))) // Bloodfest prep
                            return FatedCircle;
                        //Reign
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Reign) && LevelChecked(ReignOfBeasts)) //because leaving this out anywhere is a waste
                        {
                            if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(DoubleDown) && GunStep == 0)
                            {
                                if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                    return OriginalHook(ReignOfBeasts);
                            }

                            if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                            {
                                return OriginalHook(ReignOfBeasts);
                            }
                        }
                    }

                    //1-2
                    if (comboTime > 0 && lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                    {
                        return (IsEnabled(CustomComboPreset.GNB_AoE_Overcap) && LevelChecked(FatedCircle) && Ammo == MaxCartridges(level)) ? FatedCircle : DemonSlaughter;
                    }

                    return DemonSlice;
                }

                return actionID;
            }
        }

        internal class GNB_BS_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_BS_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is BurstStrike)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy

                    if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && HasEffect(Buffs.ReadyToBlast) && LevelChecked(Hypervelocity))
                        return Hypervelocity;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToBlast) && GetCooldownRemainingTime(Bloodfest) < 0.6f)
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && HasEffect(Buffs.NoMercy) && GetCooldownRemainingTime(DoubleDown) < 2 && Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }
                }

                return actionID;
            }
        }

        internal class GNB_FC_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_FC_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FatedCircle)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts

                    if (IsEnabled(CustomComboPreset.GNB_FC_Continuation) && HasEffect(Buffs.ReadyToRaze) && LevelChecked(FatedBrand) && CanWeave(actionID))
                        return FatedBrand;
                    if (IsEnabled(CustomComboPreset.GNB_FC_Bloodfest) && Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToRaze))
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && GetCooldownRemainingTime(DoubleDown) < 2 && Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                }

                return actionID;
            }
        }

        internal class GNB_NM_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_NM_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NoMercy)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // GF/Reign combo
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 supported, 2.45 is iffy
                    if (JustUsed(NoMercy, 20f) && InCombat())
                    {
                        //oGCDs
                        if (CanWeave(ActionWatching.LastWeaponskill))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && ActionReady(Bloodfest) && Ammo == 0)
                                return Bloodfest;
                            if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && ActionReady(OriginalHook(DangerZone)) && (HasEffect(Buffs.NoMercy) || GetCooldownRemainingTime(GnashingFang) > 17))
                                return OriginalHook(DangerZone);
                            if (IsEnabled(CustomComboPreset.GNB_NM_BS) && ActionReady(BowShock) && HasEffect(Buffs.NoMercy))
                                return BowShock;
                        }

                        //GCDs
                        if (IsEnabled(CustomComboPreset.GNB_NM_SB) && HasEffect(Buffs.ReadyToBreak))
                            return SonicBreak;
                        if (IsEnabled(CustomComboPreset.GNB_NM_DD) && LevelChecked(DoubleDown) && ActionReady(DoubleDown) && Ammo >= 2 && LevelChecked(DoubleDown))
                            return DoubleDown;
                        if (IsEnabled(CustomComboPreset.GNB_NM_Reign) && LevelChecked(ReignOfBeasts))
                        {
                            if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && GunStep == 0)
                            {
                                if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                    return OriginalHook(ReignOfBeasts);
                            }

                            if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                            {
                                return OriginalHook(ReignOfBeasts);
                            }
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