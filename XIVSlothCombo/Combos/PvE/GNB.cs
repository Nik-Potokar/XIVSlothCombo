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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (!InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    //NoMercy
                    if (ActionReady(NoMercy))
                    {
                        if (CanWeave(actionID))
                        {
                            if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                            || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3)) //Lv100 1min
                            || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min 2cart force
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && Ammo >= 2) //Lv90 1min
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener lateweave (1cart)
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (bfCD is < 90 and > 15 || (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave) //<=Lv80 lateweave use
                            || (!LevelChecked(BurstStrike) && quarterWeave)) //<Lv30
                                return NoMercy;
                        }
                    }

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Variant SpiritDart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //VariantUltimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Bloodfest
                        if (ActionReady(Bloodfest) && Ammo is 0 && (JustUsed(NoMercy, 20f)))
                            return Bloodfest;

                        //Zone
                        if (ActionReady(DangerZone) && !JustUsed(NoMercy))
                        {
                            //Lv90
                            if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((!ActionReady(GnashingFang) && nmCD > 17) || //>=Lv60
                                !LevelChecked(GnashingFang))) //<Lv60
                                return OriginalHook(DangerZone);
                            //Lv100 use
                            if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || nmCD > 17))
                                return OriginalHook(DangerZone);
                        }

                        //Continuation
                        if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                            return OriginalHook(Continuation);

                        //60s weaves
                        if (HasEffect(Buffs.NoMercy) && (GetBuffRemainingTime(Buffs.NoMercy) < 17.5))
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

                    //Hypervelocity, procced from BurstStrike usage - forced to avoid loss
                    if (JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && nmCD > 1)
                        return Hypervelocity;

                    //Continuation, procced from GnashingFang combo usage - forced to avoid loss
                    if (LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (bfCD < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && bfCD is < 90 and > 15 && !ActionReady(DoubleDown) && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (bfCD is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90-Lv99
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if (JustUsed(NoMercy, 3f) &&
                                ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 && bfCD < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (bfCD is < 90 and > 15 && Ammo == 3) //1min 3 carts
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //<=Lv89
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
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) < 0.7f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && bfCD < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90-Lv99
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo >= 2 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak, 3f) && (bfCD < GCD * 6 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak, 3f) && bfCD is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel, 3f) && bfCD is < 90 and > 15) //1min NM 2 carts
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3)) //Opener
                                return DoubleDown;
                        }

                        //<=Lv89
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && !ActionReady(GnashingFang))
                                return SonicBreak;
                            if (ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || nmCD < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //GnashingFang
                    if (LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) < 0.7f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0 && ActionReady(GnashingFang)
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && nmCD > GCD * 20 && JustUsed(DoubleDown, 3f)) //Lv90 odd minute scuffed windows
                            || (LevelChecked(DoubleDown) && !ActionReady(Bloodfest) && JustUsed(DoubleDown, 3f)) //Lv90+ Opener/Reopener conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && JustUsed(NoMercy, 3f)) //Lv80 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && JustUsed(NoMercy, 3f) && ActionReady(Bloodfest)) //Lv80 Opener/Reopener
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && !LevelChecked(Bloodfest) && JustUsed(NoMercy, 3f)) //<=Lv79 use
                            || (nmCD > GCD * 7 && nmCD < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //ReadyToReign combo
                    if (LevelChecked(ReignOfBeasts))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(GnashingFang) && !ActionReady(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    //BurstStrike
                    if (LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (Ammo >= 1 &&
                                ((LevelChecked(ReignOfBeasts) && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && GunStep == 0 && !ActionReady(DoubleDown) && !ActionReady(GnashingFang) && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (LevelChecked(ReignOfBeasts) && ((nmCD <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (bfCD < GCD * 12 || ActionReady(Bloodfest))))
                        return BurstStrike;

                    //GnashingFang combo safety net
                    if (GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    //123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && nmCD > 1) //Lv100 Hypervelocity fit into NM check
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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) &&
                        !InMeleeRange() && LevelChecked(LightningShot) && HasBattleTarget())
                        return LightningShot;

                    //NoMercy
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(actionID))
                            {
                                if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3)) //Lv100 1min
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min 2cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && Ammo >= 2) //Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (bfCD is < 90 and > 15 || (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave) //<=Lv80 lateweave use
                                || (!LevelChecked(BurstStrike) && quarterWeave)) //<Lv30
                                    return NoMercy;
                            }
                        }
                    }

                    //oGCDs
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
                                if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((!ActionReady(GnashingFang) && nmCD > 17) || //>=Lv60
                                    !LevelChecked(GnashingFang))) //<Lv60
                                    return OriginalHook(DangerZone);
                                //Lv100 use
                                if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || nmCD > 17))
                                    return OriginalHook(DangerZone);
                            }

                            //Continuation
                            if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
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
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && nmCD > 1)
                        return Hypervelocity;

                    //GF combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (bfCD < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && bfCD is < 90 and > 15 && !ActionReady(DoubleDown) && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (bfCD is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90-Lv99
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if (JustUsed(NoMercy, 3f) &&
                                ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 && bfCD < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (bfCD is < 90 and > 15 && Ammo == 3) //1min 3 carts
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //<=Lv89
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
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) < 0.7f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && bfCD < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90-Lv99
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo >= 2 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak, 3f) && (bfCD < GCD * 6 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak, 3f) && bfCD is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel, 3f) && bfCD is < 90 and > 15) //1min NM 2 carts
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3)) //Opener
                                return DoubleDown;
                        }

                        //<=Lv89
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && !ActionReady(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || nmCD < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //GnashingFang
                    if (IsEnabled(CustomComboPreset.GNB_ST_GnashingStarter) && LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) < 0.7f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0 && ActionReady(GnashingFang)
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && nmCD > GCD * 20 && JustUsed(DoubleDown, 3f)) //Lv90 odd minute scuffed windows
                            || (LevelChecked(DoubleDown) && !ActionReady(Bloodfest) && JustUsed(DoubleDown, 3f)) //Lv90+ Opener/Reopener conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && JustUsed(NoMercy, 3f)) //Lv80 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && JustUsed(NoMercy, 3f) && ActionReady(Bloodfest)) //Lv80 Opener/Reopener
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && !LevelChecked(Bloodfest) && JustUsed(NoMercy, 3f)) //<=Lv79 use
                            || (nmCD > GCD * 7 && nmCD < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //ReadyToReign combo
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(GnashingFang) && !ActionReady(DoubleDown) && GunStep == 0)
                        {
                            if (JustUsed(WickedTalon) || (JustUsed(EyeGouge)))
                                return OriginalHook(ReignOfBeasts);
                        }

                        if (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood))
                        {
                            return OriginalHook(ReignOfBeasts);
                        }
                    }

                    //BurstStrike
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (Ammo >= 1 &&
                                ((LevelChecked(ReignOfBeasts) && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && GunStep == 0 && !ActionReady(DoubleDown) && !ActionReady(GnashingFang) && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) &&
                        LevelChecked(ReignOfBeasts) && ((nmCD <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (bfCD < GCD * 12 || ActionReady(Bloodfest))))
                        return BurstStrike;

                    //GnashingFang combo safety net
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);

                    //123 (overcap included)
                    if (comboTime > 0)
                    {
                        if (lastComboMove == KeenEdge && LevelChecked(BrutalShell))
                            return BrutalShell;
                        if (lastComboMove == BrutalShell && LevelChecked(SolidBarrel))
                        {
                            if (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && nmCD > 1) //Lv100 Hypervelocity fit into NM check
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
                if (actionID is GnashingFang)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var quarterWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x

                    //No Mercy
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_NoMercy))
                    {
                        if (ActionReady(NoMercy))
                        {
                            if (CanWeave(ActionWatching.LastWeaponskill))
                            {
                                if ((LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest) && GetCooldownRemainingTime(DoubleDown) < GCD * 2) //Lv100 Opener/Reopener (0cart)
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && ((Ammo == 2 && lastComboMove is BrutalShell) || Ammo == 3)) //Lv100 1min
                                || (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) //Lv100 2min 2cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && Ammo == 0 && lastComboMove is BrutalShell && ActionReady(Bloodfest)) //Lv90 Opener/Reopener (0cart)
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 3) //Lv90 2min 3cart force
                                || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= GCD * 2 && bfCD is < 90 and > 15 && Ammo >= 2) //Lv90 1min 2 or 3cart
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && lastComboMove is SolidBarrel && ActionReady(Bloodfest) && Ammo == 1 && quarterWeave) //<=Lv80 Opener/Reopener (1cart)
                                || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && (bfCD is < 90 and > 15 || (bfCD < GCD * 12 || ActionReady(Bloodfest)) && Ammo == 2) && quarterWeave) //<=Lv80 lateweave use
                                || (!LevelChecked(BurstStrike) && quarterWeave)) //<Lv30
                                    return NoMercy;
                            }
                        }
                    }

                    //oGCDs
                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        //CDs
                        if (IsEnabled(CustomComboPreset.GNB_GF_Features))
                        {
                            //Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && ActionReady(Bloodfest) && Ammo is 0 && (JustUsed(NoMercy, 20f)))
                                return Bloodfest;

                            //Zone
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone) && !JustUsed(NoMercy))
                            {
                                //Lv90
                                if (!LevelChecked(ReignOfBeasts) && !HasEffect(Buffs.NoMercy) && ((!ActionReady(GnashingFang) && nmCD > 17) || //>=Lv60
                                    !LevelChecked(GnashingFang))) //<Lv60
                                    return OriginalHook(DangerZone);
                                //Lv100 use
                                if (LevelChecked(ReignOfBeasts) && (JustUsed(DoubleDown, 3f) || nmCD > 17))
                                    return OriginalHook(DangerZone);
                            }

                            //Continuation
                            if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                                return OriginalHook(Continuation);

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
                    }

                    //Hypervelocity
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && JustUsed(BurstStrike) && LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast) && nmCD > 1)
                        return Hypervelocity;

                    //GF combo
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && LevelChecked(Continuation) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge)))
                        return OriginalHook(Continuation);

                    //Sonic Break 
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && JustUsed(NoMercy, 20f))
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts))
                        {
                            if ((Ammo == 2 && JustUsed(NoMercy, 3f) && !HasEffect(Buffs.ReadyToBlast) && (bfCD < GCD * 12 || ActionReady(Bloodfest))) //2min
                                || (JustUsed(GnashingFang, 3f) && bfCD is < 90 and > 15 && !ActionReady(DoubleDown) && Ammo == 0 && !HasEffect(Buffs.ReadyToRip) && HasEffect(Buffs.ReadyToBreak)) //1min 2cart
                                || (Ammo == 3 && (bfCD is < 90 and > 15 && JustUsed(NoMercy, 3f)) //1min 3cart
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
                                return SonicBreak;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown))
                        {
                            if (JustUsed(NoMercy, 3f) &&
                                ((!HasEffect(Buffs.ReadyToBlast) && Ammo == 3 && bfCD < GCD * 12 || ActionReady(Bloodfest)) //2min
                                || (bfCD is < 90 and > 15 && Ammo == 3) //1min 3 carts
                                || (JustUsed(Bloodfest, 2f) && JustUsed(BrutalShell)))) //opener
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
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_DoubleDown) &&
                        (JustUsed(NoMercy, 20f) || GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && Ammo >= 2)
                    {
                        //Lv100
                        if (LevelChecked(ReignOfBeasts) && GetCooldownRemainingTime(DoubleDown) < 0.7f)
                        {
                            if ((JustUsed(SonicBreak, 3f) && !HasEffect(Buffs.ReadyToBreak) && bfCD < GCD * 6 || ActionReady(Bloodfest)) //2min
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3) //1min NM 3 carts
                                || (JustUsed(SolidBarrel, 3f) && Ammo == 3 && HasEffect(Buffs.ReadyToBreak) && HasEffect(Buffs.NoMercy))) //1min NM 2 carts
                                return DoubleDown;
                        }

                        //Lv90
                        if (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) <= 0.6f)
                        {
                            if ((Ammo >= 2 && !HasEffect(Buffs.ReadyToBreak) && JustUsed(SonicBreak, 3f) && (bfCD < GCD * 6 || ActionReady(Bloodfest))) //2min NM 3 carts
                                || (!HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SonicBreak, 3f) && bfCD is < 90 and > 15) //1min NM 3 carts
                                || (HasEffect(Buffs.ReadyToBreak) && Ammo == 3 && JustUsed(SolidBarrel, 3f) && bfCD is < 90 and > 15) //1min NM 2 carts
                                || (JustUsed(SonicBreak, 3f) && Ammo == 3)) //Opener
                                return DoubleDown;
                        }

                        //<Lv90
                        if (!LevelChecked(DoubleDown) && !LevelChecked(ReignOfBeasts))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && HasEffect(Buffs.ReadyToBreak) && (GetBuffRemainingTime(Buffs.NoMercy) >= GCD * 4) && !HasEffect(Buffs.ReadyToRip) && !ActionReady(GnashingFang))
                                return SonicBreak;
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && ActionReady(DangerZone) && !LevelChecked(SonicBreak) && HasEffect(Buffs.NoMercy) || nmCD < 30)  //subLv54
                                return OriginalHook(DangerZone);
                        }
                    }

                    //Gnashing Fang
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) < 0.7f && Ammo > 0)
                    {
                        if (!HasEffect(Buffs.ReadyToBlast) && GunStep == 0 && ActionReady(GnashingFang)
                            && (LevelChecked(ReignOfBeasts) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv100 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && HasEffect(Buffs.NoMercy) && JustUsed(DoubleDown, 3f)) //Lv90 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && LevelChecked(DoubleDown) && nmCD > GCD * 20 && JustUsed(DoubleDown, 3f)) //Lv90 odd minute scuffed windows
                            || (LevelChecked(DoubleDown) && !ActionReady(Bloodfest) && JustUsed(DoubleDown, 3f)) //Lv90+ Opener/Reopener conditions
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && JustUsed(NoMercy, 3f)) //Lv80 odd/even minute use
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && LevelChecked(Bloodfest) && Ammo == 1 && JustUsed(NoMercy, 3f) && ActionReady(Bloodfest)) //Lv80 Opener/Reopener
                            || (!LevelChecked(ReignOfBeasts) && !LevelChecked(DoubleDown) && !LevelChecked(Bloodfest) && JustUsed(NoMercy, 3f)) //<=Lv79 use
                            || (nmCD > GCD * 7 && nmCD < GCD * 14)) //30s use
                            return GnashingFang;
                    }

                    //Reign combo
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(GnashingFang) && !ActionReady(DoubleDown) && GunStep == 0)
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
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && LevelChecked(BurstStrike))
                    {
                        if (HasEffect(Buffs.NoMercy))
                        {
                            if (Ammo >= 1 &&
                                ((LevelChecked(ReignOfBeasts) && GunStep == 0 && GetBuffRemainingTime(Buffs.NoMercy) <= GCD * 3 && !HasEffect(Buffs.ReadyToReign))
                                || (!LevelChecked(ReignOfBeasts) && GunStep == 0 && !ActionReady(DoubleDown) && !ActionReady(GnashingFang) && HasEffect(Buffs.NoMercy) && !HasEffect(Buffs.ReadyToBreak))))
                                return BurstStrike;
                        }
                    }

                    //Lv100 2cart 2min starter
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) &&
                        LevelChecked(ReignOfBeasts) && ((nmCD <= GCD || ActionReady(NoMercy)) && Ammo is 3 && (bfCD < GCD * 12 || ActionReady(Bloodfest))))
                        return BurstStrike;

                    //GF combo safety net
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && GunStep is 1 or 2)
                        return OriginalHook(GnashingFang);
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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x

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
                        if (Ammo >= 2 && ActionReady(DoubleDown) && HasEffect(Buffs.NoMercy)) //use on CD under NM
                            return DoubleDown;
                        //Reign
                        if (LevelChecked(ReignOfBeasts)) //because leaving this out anywhere is a waste
                        {
                            if ((GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(DoubleDown) && GunStep == 0) ||
                                (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood)))
                                return OriginalHook(ReignOfBeasts);
                        }
                        //FatedCircle - if not unlocked, use BurstStrike
                        if (Ammo > 0 && LevelChecked(FatedCircle) && 
                            (HasEffect(Buffs.NoMercy) && !ActionReady(DoubleDown) && GunStep == 0) || //use when under NM after DD & ignores GF
                            (bfCD < 6)) // Bloodfest prep
                            return FatedCircle;
                        if (Ammo > 0 && !LevelChecked(FatedCircle) && LevelChecked(BurstStrike) &&
                            (HasEffect(Buffs.NoMercy) && !ActionReady(DoubleDown) && GunStep == 0)) //use when under NM after DD & ignores GF
                            return BurstStrike;
                    }

                    //1-2
                    if (comboTime > 0)
                    {
                        if (lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                        {
                            if (Ammo == MaxCartridges(level))
                            {
                                if (LevelChecked(FatedCircle))
                                    return FatedCircle;
                                if (!LevelChecked(FatedCircle))
                                    return BurstStrike;
                            }
                            if (Ammo != MaxCartridges(level))
                            {
                                return DemonSlaughter;
                            }
                        }
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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total
                    float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x

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
                        //Reign
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Reign) && LevelChecked(ReignOfBeasts)) //because leaving this out anywhere is a waste
                        {
                            if ((GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(DoubleDown) && GunStep == 0) ||
                                (JustUsed(ReignOfBeasts) || JustUsed(NobleBlood)))
                                return OriginalHook(ReignOfBeasts);
                        }
                        //FatedCircle - if not unlocked, use BurstStrike
                        if (Ammo > 0 && LevelChecked(FatedCircle) && 
                            ((IsEnabled(CustomComboPreset.GNB_AoE_FatedCircle) && HasEffect(Buffs.NoMercy) && !ActionReady(DoubleDown) && GunStep == 0) || //use when under NM after DD & ignores GF
                            (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && bfCD < 6))) // Bloodfest prep
                            return FatedCircle;
                        if (Ammo > 0 && !LevelChecked(FatedCircle) && LevelChecked(BurstStrike) &&
                            (HasEffect(Buffs.NoMercy) && GunStep == 0)) // Bloodfest prep
                            return BurstStrike;
                    }

                    //1-2
                    if (comboTime > 0)
                    {
                        if (lastComboMove == DemonSlice && LevelChecked(DemonSlaughter))
                        {
                            if (Ammo == MaxCartridges(level))
                            {
                                if (LevelChecked(FatedCircle))
                                    return FatedCircle;
                                if (!LevelChecked(FatedCircle))
                                    return BurstStrike;
                            }
                            if (Ammo != MaxCartridges(level))
                            {
                                return DemonSlaughter;
                            }
                        }
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
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; // For GnashingFang & (possibly) ReignCombo purposes
                    var bfCD = GetCooldownRemainingTime(Bloodfest); // Bloodfest's cooldown; 120s total

                    if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && HasEffect(Buffs.ReadyToBlast) && LevelChecked(Hypervelocity))
                        return Hypervelocity;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToBlast) && bfCD < 0.6f)
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && HasEffect(Buffs.NoMercy) && GetCooldownRemainingTime(DoubleDown) < 2 && Ammo >= 2 && LevelChecked(DoubleDown))
                        return DoubleDown;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if (HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(DoubleDown) && GunStep == 0)
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
                            if (GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(GnashingFang) && !ActionReady(DoubleDown) && GunStep == 0)
                            {
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