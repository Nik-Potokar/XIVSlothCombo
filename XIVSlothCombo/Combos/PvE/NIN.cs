using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using System;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using static XIVSlothCombo.Combos.JobHelpers.NIN;

namespace XIVSlothCombo.Combos.PvE
{
    internal class NIN
    {
        public const byte ClassID = 29;
        public const byte JobID = 30;

        public const uint
            SpinningEdge = 2240,
            ShadeShift = 2241,
            GustSlash = 2242,
            Hide = 2245,
            Assassinate = 2246,
            ThrowingDaggers = 2247,
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            Bhavacakra = 7402,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Bunshin = 16493,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778,
            Hellfrog = 7401,
            HollowNozuchi = 25776,
            TenriJendo = 36961,
            KunaisBane = 36958,
            ZeshoMeppo = 36960,

            //Mudras
            Ninjutsu = 2260,
            Rabbit = 2272,

            //-- initial state mudras (the ones with charges)
            Ten = 2259,
            Chi = 2261,
            Jin = 2263,

            //-- mudras used for combos (the ones used while you have the mudra buff)
            TenCombo = 18805,
            ChiCombo = 18806,
            JinCombo = 18807,

            //Ninjutsu
            FumaShuriken = 2265,
            Hyoton = 2268,
            Doton = 2270,
            Katon = 2266,
            Suiton = 2271,
            Raiton = 2267,
            Huton = 2269,
            GokaMekkyaku = 16491,
            HyoshoRanryu = 16492,

            //TCJ Jutsus (why they have another ID I will never know)
            TCJFumaShurikenTen = 18873,
            TCJFumaShurikenChi = 18874,
            TCJFumaShurikenJin = 18875,
            TCJKaton = 18876,
            TCJRaiton = 18877,
            TCJHyoton = 18878,
            TCJHuton = 18879,
            TCJDoton = 18880,
            TCJSuiton = 18881;

        public static class Buffs
        {
            public const ushort
                Mudra = 496,
                Kassatsu = 497,
                //Suiton = 507,
                Higi = 3850,
                TenriJendo = 3851,
                ShadowWalker = 3848,
                Hidden = 614,
                TenChiJin = 1186,
                AssassinateReady = 1955,
                RaijuReady = 2690,
                PhantomReady = 2723,
                Meisui = 2689,
                Doton = 501,
                Bunshin = 1954;
        }

        public static class Debuffs
        {
            public const ushort
                Dokumori = 3849,
                TrickAttack = 3254,
                KunaisBane = 3906,
                Mug = 638;
        }

        public static class Traits
        {
            public const uint
                EnhancedKasatsu = 250;
        }

        public static class Config
        {
            public const string
                Trick_CooldownRemaining = "Trick_CooldownRemaining",
                Mug_NinkiGauge = "Mug_NinkiGauge",
                Ninki_BhavaPooling = "Ninki_BhavaPooling",
                Ninki_HellfrogPooling = "Ninki_HellfrogPooling",
                NIN_SimpleMudra_Choice = "NIN_SimpleMudra_Choice",
                Ninki_BunshinPoolingST = "Ninki_BunshinPoolingST",
                Ninki_BunshinPoolingAoE = "Ninki_BunshinPoolingAoE",
                Advanced_Trick_Cooldown = "Advanced_Trick_Cooldown",
                Advanced_DotonTimer = "Advanced_DotonTimer",
                Advanced_DotonHP = "Advanced_DotonHP",
                Advanced_TCJEnderAoE = "Advanced_TCJEnderAoe",
                Advanced_ChargePool = "Advanced_ChargePool",
                SecondWindThresholdST = "SecondWindThresholdST",
                ShadeShiftThresholdST = "ShadeShiftThresholdST",
                BloodbathThresholdST = "BloodbathThresholdST",
                SecondWindThresholdAoE = "SecondWindThresholdAoE",
                ShadeShiftThresholdAoE = "ShadeShiftThresholdAoE",
                BloodbathThresholdAoE = "BloodbathThresholdAoE",
                NIN_VariantCure = "NIN_VariantCure";
        }

        internal class NIN_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ST_AdvancedMode;

            protected internal MudraCasting mudraState = new();

            protected internal static NINOpenerLogic NINOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SpinningEdge)
                {
                    NINGauge gauge = GetJobGauge<NINGauge>();
                    bool canWeave = CanWeave(SpinningEdge);
                    var canDelayedWeave = CanDelayedWeave(SpinningEdge);
                    bool inTrickBurstSaveWindow = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Cooldowns) && IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack) && GetCooldownRemainingTime(TrickAttack) <= GetOptionValue(Config.Advanced_Trick_Cooldown);
                    bool useBhakaBeforeTrickWindow = GetCooldownRemainingTime(TrickAttack) >= 3;
                    bool setupSuitonWindow = GetCooldownRemainingTime(OriginalHook(TrickAttack) ) <= GetOptionValue(Config.Trick_CooldownRemaining) && !HasEffect(Buffs.ShadowWalker);
                    bool setupKassatsuWindow = GetCooldownRemainingTime(TrickAttack) <= 10 && HasEffect(Buffs.ShadowWalker);
                    bool chargeCheck = IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_ChargeHold) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_ChargeHold) && (GetRemainingCharges(Ten) == 2 || (GetRemainingCharges(Ten) == 1 && GetCooldownChargeRemainingTime(Ten) < 3)));
                    bool poolCharges = !GetOptionBool(Config.Advanced_ChargePool) || (GetRemainingCharges(Ten) == 1 && GetCooldownChargeRemainingTime(Ten) < 2) || NINHelper.TrickDebuff;
                    bool raitonUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiton_Uptime);
                    bool suitonUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Suiton_Uptime);
                    int bhavaPool = GetOptionValue(Config.Ninki_BhavaPooling);
                    int bunshinPool = GetOptionValue(Config.Ninki_BunshinPoolingST);
                    int SecondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.SecondWindThresholdST);
                    int ShadeShiftThreshold = PluginConfiguration.GetCustomIntValue(Config.ShadeShiftThresholdST);
                    int BloodbathThreshold = PluginConfiguration.GetCustomIntValue(Config.BloodbathThresholdST);
                    double playerHP = PlayerHealthPercentageHp();
                    bool phantomUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Phantom_Uptime);
                    var comboLength = GetCooldown(GustSlash).CooldownTotal * 3;

                    if (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus) || (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat()))
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) && IsOnCooldown(TrickAttack) && mudraState.CurrentMudra == MudraCasting.MudraState.CastingSuiton && !setupSuitonWindow)
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) && IsOnCooldown(TrickAttack) && mudraState.CurrentMudra != MudraCasting.MudraState.CastingSuiton && setupSuitonWindow)
                        mudraState.CurrentMudra = MudraCasting.MudraState.CastingSuiton;

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_BalanceOpener) && NINOpenerLogic.LevelChecked && NINOpener.DoFullOpener(ref actionID, mudraState))
                        return actionID;

                    if (HasEffect(Buffs.TenChiJin))
                    {
                        if (OriginalHook(Ten) == TCJFumaShurikenTen) return OriginalHook(Ten);
                        if (OriginalHook(Chi) == TCJRaiton) return OriginalHook(Chi);
                        if (OriginalHook(Jin) == TCJSuiton) return OriginalHook(Jin);
                    }

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu_HyoshoRaynryu) &&
                        HasEffect(Buffs.Kassatsu) &&
                        NINHelper.TrickDebuff &&
                        (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))))
                        mudraState.CurrentMudra = MudraCasting.MudraState.CastingHyoshoRanryu;

                    if (mudraState.CurrentMudra != MudraCasting.MudraState.None)
                    {
                        if (mudraState.ContinueCurrentMudra(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat() && !InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin_Phantom) &&
                            HasEffect(Buffs.PhantomReady) &&
                            ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady) && GetBuffRemainingTime(Buffs.PhantomReady) < 5) || NINHelper.TrickDebuff || (HasEffect(Buffs.Bunshin) && TargetHasEffect(Debuffs.Mug))) &&
                            PhantomKamaitachi.LevelChecked()
                            && phantomUptime)
                            return OriginalHook(PhantomKamaitachi);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) &&
                            setupSuitonWindow &&
                            TrickAttack.LevelChecked() &&
                            !HasEffect(Buffs.ShadowWalker) &&
                            chargeCheck &&
                            suitonUptime &&
                            mudraState.CastSuiton(ref actionID))
                            return actionID;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton) &&
                            !inTrickBurstSaveWindow &&
                            chargeCheck &&
                            poolCharges &&
                            raitonUptime &&
                            mudraState.CastRaiton(ref actionID))
                            return actionID;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_RangedUptime) && ThrowingDaggers.LevelChecked() && HasTarget() && !HasEffect(Buffs.RaijuReady))
                            return OriginalHook(ThrowingDaggers);
                    }

                    if (canWeave && !NINHelper.InMudra)
                    {
                        if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) &&
                            IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignBefore) &&
                            HasEffect(Buffs.ShadowWalker) &&
                            GetCooldownRemainingTime(TrickAttack) <= 3 &&
                            ((IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed) && InCombat() && combatDuration.TotalSeconds > 6) ||
                            IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed)) &&
                            IsOffCooldown(Mug) &&
                            Mug.LevelChecked())
                            return OriginalHook(Mug);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack) &&
                            HasEffect(Buffs.ShadowWalker) &&
                            IsOffCooldown(TrickAttack) &&
                            ((IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed) && InCombat() && combatDuration.TotalSeconds > 8) ||
                            IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed)))
                            return OriginalHook(TrickAttack);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TenriJindo) && HasEffect(Buffs.TenriJendo) && ((NINHelper.TrickDebuff && NINHelper.MugDebuff) || GetBuffRemainingTime(Buffs.TenriJendo) <= 3))
                            return OriginalHook(TenriJendo);    

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin) && Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= bunshinPool)
                            return OriginalHook(Bunshin);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu) && (NINHelper.TrickDebuff || setupKassatsuWindow) && IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                            return OriginalHook(Kassatsu);

                        //healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                            return ShadeShift;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) &&
                            ((NINHelper.TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki == 100)) &&
                            (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                            Bhavacakra.LevelChecked())
                            return OriginalHook(Bhavacakra);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) &&
                            ((NINHelper.TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki >= 60)) &&
                            (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                            !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                            return OriginalHook(Hellfrog);

                        if (!inTrickBurstSaveWindow)
                        {
                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOffCooldown(Mug) && Mug.LevelChecked())
                            {
                                if (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignAfter) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignAfter) && NINHelper.TrickDebuff))
                                    return OriginalHook(Mug);
                            }

                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Meisui) && HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                                return OriginalHook(Meisui);

                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) && gauge.Ninki >= bhavaPool && Bhavacakra.LevelChecked())
                                return OriginalHook(Bhavacakra);

                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) && gauge.Ninki >= bhavaPool && !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                                return OriginalHook(Hellfrog);

                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_AssassinateDWAD) && IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                                return OriginalHook(Assassinate);

                            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TCJ) && IsOffCooldown(TenChiJin) && !IsMoving && TenChiJin.LevelChecked())
                                return OriginalHook(TenChiJin);
                        }

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                            return ShadeShift;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;
                    }


                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiju) && HasEffect(Buffs.RaijuReady))
                    {
                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiju_Forked) && !InMeleeRange())
                            return OriginalHook(ForkedRaiju);
                        return OriginalHook(FleetingRaiju);
                    }

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin_Phantom) &&
                        HasEffect(Buffs.PhantomReady) &&
                        ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady) && GetBuffRemainingTime(Buffs.PhantomReady) < 5) || NINHelper.TrickDebuff || (HasEffect(Buffs.Bunshin) && TargetHasEffect(Debuffs.Mug))) &&
                        PhantomKamaitachi.LevelChecked())
                        return OriginalHook(PhantomKamaitachi);


                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu_HyoshoRaynryu) &&
                        !inTrickBurstSaveWindow &&
                        (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                        mudraState.CastHyoshoRanryu(ref actionID))
                        return actionID;

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus))
                    {
                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) &&
                            setupSuitonWindow &&
                            TrickAttack.LevelChecked() &&
                            !HasEffect(Buffs.ShadowWalker) &&
                            chargeCheck &&
                            mudraState.CastSuiton(ref actionID))
                            return actionID;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton) &&
                            !inTrickBurstSaveWindow &&
                            chargeCheck &&
                            poolCharges &&
                            mudraState.CastRaiton(ref actionID))
                            return actionID;

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_FumaShuriken) &&
                            !Raiton.LevelChecked() &&
                            chargeCheck &&
                            mudraState.CastFumaShuriken(ref actionID))
                            return actionID;
                    }

                    if (comboTime > 1f)
                    {
                        if (lastComboMove == SpinningEdge && GustSlash.LevelChecked())
                            return OriginalHook(GustSlash);

                        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrueNorth) && TargetNeedsPositionals() &&
                            IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrueNorth_ArmorCrush) &&
                            lastComboMove == GustSlash && GetRemainingCharges(All.TrueNorth) > 0 &&
                            All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) &&
                            canWeave)
                            return OriginalHook(All.TrueNorth);

                        if (lastComboMove == GustSlash && ArmorCrush.LevelChecked())
                        {
                            if ((!NINHelper.MugDebuff) || (NINHelper.MugDebuff &&  gauge.Kazematoi == 0))
                            {
                                if (gauge.Kazematoi < 4)
                                    return OriginalHook(ArmorCrush);
                            }
                        }

                        if (lastComboMove == GustSlash && AeolianEdge.LevelChecked() && (gauge.Kazematoi > 0 || !ArmorCrush.LevelChecked()))
                            return OriginalHook(AeolianEdge);
                    }

                    return OriginalHook(SpinningEdge);
                }
                return actionID;
            }
        }

        internal class NIN_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AoE_AdvancedMode;

            protected internal MudraCasting mudraState = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DeathBlossom)
                {
                    Status? dotonBuff = FindEffect(Buffs.Doton);
                    NINGauge? gauge = GetJobGauge<NINGauge>();
                    bool canWeave = CanWeave(GustSlash);
                    bool chargeCheck = IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_ChargeHold) || (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_ChargeHold) && GetRemainingCharges(Ten) == 2);
                    bool inMudraState = HasEffect(Buffs.Mudra);
                    int hellfrogPool = GetOptionValue(Config.Ninki_HellfrogPooling);
                    int dotonTimer = GetOptionValue(Config.Advanced_DotonTimer);
                    int dotonThreshold = GetOptionValue(Config.Advanced_DotonHP);
                    int tcjPath = GetOptionValue(Config.Advanced_TCJEnderAoE);
                    int bunshingPool = GetOptionValue(Config.Ninki_BunshinPoolingAoE);
                    int SecondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.SecondWindThresholdAoE);
                    int ShadeShiftThreshold = PluginConfiguration.GetCustomIntValue(Config.ShadeShiftThresholdAoE);
                    int BloodbathThreshold = PluginConfiguration.GetCustomIntValue(Config.BloodbathThresholdAoE);
                    double playerHP = PlayerHealthPercentageHp();

                    if (IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus) || (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat()))
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (HasEffect(Buffs.TenChiJin))
                    {
                        if (tcjPath == 0)
                        {
                            if (OriginalHook(Chi) == TCJFumaShurikenChi) return OriginalHook(Chi);
                            if (OriginalHook(Ten) == TCJKaton) return OriginalHook(Ten);
                            if (OriginalHook(Jin) == TCJSuiton) return OriginalHook(Jin);
                        }
                        else
                        {
                            if (OriginalHook(Jin) == TCJFumaShurikenJin) return OriginalHook(Jin);
                            if (OriginalHook(Ten) == TCJKaton) return OriginalHook(Ten);
                            if (OriginalHook(Chi) == TCJDoton) return OriginalHook(Chi);
                        }

                    }

                    if (JustUsed(Kassatsu, 0.5f))
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_KunaisBane))
                    {
                        if (!HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && GetCooldownRemainingTime(KunaisBane) < 5 && mudraState.CastHuton(ref actionID))
                            return actionID;

                        if (HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && IsOffCooldown(KunaisBane) && canWeave)
                            return KunaisBane;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_GokaMekkyaku) && HasEffect(Buffs.Kassatsu))
                        mudraState.CurrentMudra = MudraCasting.MudraState.CastingGokaMekkyaku;

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus) && mudraState.CurrentMudra != MudraCasting.MudraState.None)
                    {
                        if (mudraState.ContinueCurrentMudra(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                        return Variant.VariantCure;

                    if (canWeave && !inMudraState)
                    {
                        if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_TenriJindo) && HasEffect(Buffs.TenriJendo))
                            return OriginalHook(TenriJendo);

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin) && Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= bunshingPool)
                            return OriginalHook(Bunshin);

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium) && gauge.Ninki >= hellfrogPool && Hellfrog.LevelChecked())
                        {
                            if (HasEffect(Buffs.Meisui) && level >= 88)
                                return OriginalHook(Bhavacakra);

                            return OriginalHook(Hellfrog);
                        }

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium) && gauge.Ninki >= hellfrogPool && !Hellfrog.LevelChecked() && Bhavacakra.LevelChecked())
                        {
                            return OriginalHook(Bhavacakra);
                        }

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Kassatsu) &&
                            IsOffCooldown(Kassatsu) &&
                            Kassatsu.LevelChecked() &&
                            ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && (dotonBuff != null || GetTargetHPPercent() < dotonThreshold)) ||
                            IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton)))
                            return OriginalHook(Kassatsu);

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Meisui) && HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                            return OriginalHook(Meisui);

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_AssassinateDWAD) && IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                            return OriginalHook(Assassinate);

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                            return ShadeShift;

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                            return All.Bloodbath;

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_TCJ) &&
                            IsOffCooldown(TenChiJin) &&
                            !IsMoving &&
                            TenChiJin.LevelChecked())
                        {
                            if ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && tcjPath == 1 &&
                               (dotonBuff?.RemainingTime <= dotonTimer || dotonBuff is null) &&
                               GetTargetHPPercent() >= dotonThreshold &&
                               !WasLastAction(Doton)) ||
                               tcjPath == 0)
                                return OriginalHook(TenChiJin);
                        }

                    }

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_GokaMekkyaku) &&
                        mudraState.CastGokaMekkyaku(ref actionID))
                        return actionID;

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus))
                    {
                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) &&
                            (dotonBuff?.RemainingTime <= dotonTimer || dotonBuff is null) &&
                            GetTargetHPPercent() >= dotonThreshold &&
                            chargeCheck &&
                            !(WasLastAction(Doton) || WasLastAction(TCJDoton) || dotonBuff is not null) &&
                            mudraState.CastDoton(ref actionID))
                            return actionID;

                        if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Katon) &&
                            chargeCheck &&
                            ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && (dotonBuff != null || GetTargetHPPercent() < dotonThreshold)) ||
                            IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton)) &&
                            mudraState.CastKaton(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin_Phantom) && HasEffect(Buffs.PhantomReady) && PhantomKamaitachi.LevelChecked())
                        return OriginalHook(PhantomKamaitachi);

                    if (comboTime > 1f)
                    {
                        if (lastComboMove is DeathBlossom && HakkeMujinsatsu.LevelChecked())
                            return OriginalHook(HakkeMujinsatsu);
                    }

                    return OriginalHook(DeathBlossom);
                }
                return actionID;
            }
        }


        internal class NIN_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ST_SimpleMode;

            protected internal MudraCasting mudraState = new();

            protected internal static NINOpenerLogic NINOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SpinningEdge)
                {
                    NINGauge gauge = GetJobGauge<NINGauge>();
                    bool canWeave = CanWeave(SpinningEdge);
                    bool inTrickBurstSaveWindow = GetCooldownRemainingTime(TrickAttack) <= 15 && Suiton.LevelChecked();
                    bool useBhakaBeforeTrickWindow = GetCooldownRemainingTime(TrickAttack) >= 3;

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (IsEnabled(CustomComboPreset.NIN_ST_SimpleMode_BalanceOpener) && NINOpenerLogic.LevelChecked && NINOpener.DoFullOpener(ref actionID, mudraState))
                        return actionID;

                    if (HasEffect(Buffs.TenChiJin))
                    {
                        if (WasLastAction(TCJFumaShurikenTen)) return OriginalHook(Chi);
                        if (WasLastAction(TCJRaiton)) return OriginalHook(Jin);
                        return OriginalHook(Ten);
                    }

                    if (NINHelper.InMudra)
                    {
                        if (mudraState.ContinueCurrentMudra(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                        return Variant.VariantCure;

                    if (mudraState.CastHyoshoRanryu(ref actionID))
                        return actionID;

                    if (GetCooldownRemainingTime(TrickAttack) < 15 && TrickAttack.LevelChecked() && !HasEffect(Buffs.ShadowWalker))
                        if (mudraState.CastSuiton(ref actionID))
                            return actionID;

                    if (!inTrickBurstSaveWindow)
                    {
                        if (mudraState.CastRaiton(ref actionID))
                            return actionID;
                    }

                    if (canWeave && !NINHelper.InMudra)
                    {
                        if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (HasEffect(Buffs.TenriJendo))
                            return OriginalHook(TenriJendo);

                        if (Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= 50)
                            return OriginalHook(Bunshin);

                        if (HasEffect(Buffs.ShadowWalker) && IsOffCooldown(TrickAttack))
                            return OriginalHook(TrickAttack);

                        if (Bhavacakra.LevelChecked() && ((NINHelper.TrickDebuff && gauge.Ninki >= 50) || useBhakaBeforeTrickWindow && gauge.Ninki == 100))
                            return OriginalHook(Bhavacakra);

                        if ((NINHelper.TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki == 100) && !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                            return OriginalHook(Hellfrog);

                        if (!inTrickBurstSaveWindow)
                        {
                            if (HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                                return OriginalHook(Meisui);

                            if (IsOffCooldown(Mug) && Mug.LevelChecked())
                                return OriginalHook(Mug);

                            if (gauge.Ninki >= 85 && Bhavacakra.LevelChecked())
                                return OriginalHook(Bhavacakra);

                            if (IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                                return OriginalHook(Assassinate);

                            if (IsOffCooldown(TenChiJin) && TenChiJin.LevelChecked())
                                return OriginalHook(TenChiJin);

                            if (IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                                return OriginalHook(Kassatsu);
                        }
                    }
                    else
                    {
                        if (HasEffect(Buffs.RaijuReady))
                            return OriginalHook(FleetingRaiju);

                        if (HasEffect(Buffs.PhantomReady) && PhantomKamaitachi.LevelChecked())
                            return OriginalHook(PhantomKamaitachi);
                    }

                    if (comboTime > 1f)
                    {
                        if (lastComboMove == SpinningEdge && GustSlash.LevelChecked())
                            return OriginalHook(GustSlash);

                        if (lastComboMove == GustSlash && TargetNeedsPositionals() && GetRemainingCharges(All.TrueNorth) > 0 && All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) && canWeave)
                            return OriginalHook(All.TrueNorth);

                        if (lastComboMove == GustSlash && AeolianEdge.LevelChecked() && (gauge.Kazematoi > 0 || !ArmorCrush.LevelChecked()))
                            return OriginalHook(AeolianEdge);

                        if (lastComboMove == GustSlash && ArmorCrush.LevelChecked() && gauge.Kazematoi < 5)
                            return OriginalHook(ArmorCrush);


                    }

                    return OriginalHook(SpinningEdge);
                }
                return actionID;
            }
        }

        internal class NIN_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AoE_SimpleMode;

            private MudraCasting mudraState = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DeathBlossom)
                {
                    var dotonBuff = FindEffect(Buffs.Doton);
                    var gauge = GetJobGauge<NINGauge>();
                    var canWeave = CanWeave(GustSlash);

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat())
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (HasEffect(Buffs.TenChiJin))
                    {
                        if (WasLastAction(TCJFumaShurikenJin)) return OriginalHook(Ten);
                        if (WasLastAction(TCJKaton) || WasLastAction(HollowNozuchi)) return OriginalHook(Chi);
                        return OriginalHook(Jin);
                    }

                    if (JustUsed(Kassatsu, 0.5f))
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;

                    if (NINHelper.InMudra)
                    {
                        if (mudraState.ContinueCurrentMudra(ref actionID))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                        return Variant.VariantCure;

                    if (HasEffect(Buffs.Kassatsu))
                    {
                        if (GokaMekkyaku.LevelChecked())
                        {
                            if (mudraState.CastGokaMekkyaku(ref actionID))
                                return actionID;
                        }
                        else
                        {
                            if (mudraState.CastKaton(ref actionID))
                                return actionID;
                        }
                    }

                    if (!HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && GetCooldownRemainingTime(KunaisBane) < 5 && mudraState.CastHuton(ref actionID))
                        return actionID;

                    if (HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && IsOffCooldown(KunaisBane) && canWeave)
                        return KunaisBane;

                    if (GetTargetHPPercent() > 20 && (dotonBuff is null || dotonBuff?.RemainingTime <= GetCooldownChargeRemainingTime(Ten)) && !JustUsed(Doton) && IsOnCooldown(TenChiJin))
                    {
                        if (mudraState.CastDoton(ref actionID))
                            return actionID;
                    }
                    else if (mudraState.CurrentMudra == MudraCasting.MudraState.CastingDoton)
                        mudraState.CurrentMudra = MudraCasting.MudraState.None;


                    if (canWeave && !NINHelper.InMudra)
                    {
                        if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart))
                            return Variant.VariantRampart;

                        if (IsOffCooldown(TenChiJin) && TenChiJin.LevelChecked())
                            return OriginalHook(TenChiJin);

                        if (HasEffect(Buffs.TenriJendo))
                            return TenriJendo;

                        if (IsOffCooldown(Bunshin) && gauge.Ninki >= 50 && Bunshin.LevelChecked())
                            return OriginalHook(Bunshin);

                        if (HasEffect(Buffs.ShadowWalker) && gauge.Ninki < 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                            return OriginalHook(Meisui);

                        if (HasEffect(Buffs.Meisui) && gauge.Ninki >= 50)
                            return OriginalHook(Bhavacakra);

                        if (gauge.Ninki >= 50 && Hellfrog.LevelChecked())
                            return OriginalHook(Hellfrog);

                        if (gauge.Ninki >= 50 && !Hellfrog.LevelChecked() && Bhavacakra.LevelChecked())
                            return OriginalHook(Bhavacakra);

                        if (IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                            return OriginalHook(Kassatsu);
                    }
                    else
                    {
                        if (HasEffect(Buffs.PhantomReady))
                            return OriginalHook(PhantomKamaitachi);
                    }

                    if (mudraState.CastKaton(ref actionID))
                        return actionID;

                    if (comboTime > 1f)
                    {
                        if (lastComboMove is DeathBlossom && HakkeMujinsatsu.LevelChecked())
                            return OriginalHook(HakkeMujinsatsu);
                    }

                    return OriginalHook(DeathBlossom);
                }
                return actionID;
            }
        }

        internal class NIN_ArmorCrushCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ArmorCrushCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmorCrush)
                {
                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= 4)
                        {
                            return GustSlash;
                        }

                        if (lastComboMove == GustSlash && level >= 54)
                        {
                            return ArmorCrush;
                        }
                    }
                    return SpinningEdge;
                }
                return actionID;
            }
        }


        internal class NIN_HideMug : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_HideMug;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Hide)
                {
                    if (HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                    {
                        return Mug;
                    }

                    if (HasEffect(Buffs.Hidden))
                    {
                        return TrickAttack;
                    }

                }

                return actionID;
            }
        }

        internal class NIN_KassatsuChiJin : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuChiJin;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Chi && level >= 76 && HasEffect(Buffs.Kassatsu))
                {
                    return Jin;
                }
                return actionID;
            }
        }

        internal class NIN_KassatsuTrick : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuTrick;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Kassatsu)
                {
                    if (HasEffect(Buffs.ShadowWalker) || HasEffect(Buffs.Hidden))
                    {
                        return OriginalHook(TrickAttack);
                    }
                    return OriginalHook(Kassatsu);
                }
                return actionID;
            }
        }

        internal class NIN_TCJMeisui : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.NIN_TCJMeisui;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == TenChiJin)
                {

                    if (HasEffect(Buffs.ShadowWalker))
                        return Meisui;

                    if (HasEffect(Buffs.TenChiJin) && IsEnabled(CustomComboPreset.NIN_TCJ))
                    {
                        var tcjTimer = FindEffectAny(Buffs.TenChiJin).RemainingTime;

                        if (tcjTimer > 5)
                            return OriginalHook(Ten);

                        if (tcjTimer > 4)
                            return OriginalHook(Chi);

                        if (tcjTimer > 3)
                            return OriginalHook(Jin);
                    }
                }
                return actionID;
            }
        }

        internal class NIN_Simple_Mudras : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_Simple_Mudras;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ten or Chi or Jin)
                {
                    var mudrapath = GetOptionValue(Config.NIN_SimpleMudra_Choice);

                    if (HasEffect(Buffs.Mudra))
                    {

                        if (mudrapath == 1)
                        {
                            if (Ten.LevelChecked() && actionID == Ten)
                            {
                                if (Jin.LevelChecked() && (OriginalHook(Ninjutsu) is Raiton))
                                {
                                    return OriginalHook(JinCombo);
                                }

                                if (Chi.LevelChecked() && (OriginalHook(Ninjutsu) is HyoshoRanryu))
                                {
                                    return OriginalHook(ChiCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (HasEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                                        return JinCombo;

                                    if (Chi.LevelChecked())
                                        return OriginalHook(ChiCombo);

                                    if (Jin.LevelChecked())
                                        return OriginalHook(JinCombo);
                                }
                            }

                            if (Chi.LevelChecked() && actionID == Chi)
                            {
                                if (OriginalHook(Ninjutsu) is Hyoton)
                                {
                                    return OriginalHook(TenCombo);
                                }

                                if (Jin.LevelChecked() && OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(JinCombo);
                                }
                            }

                            if (Jin.LevelChecked() && actionID == Jin)
                            {
                                if (OriginalHook(Ninjutsu) is GokaMekkyaku or Katon)
                                {
                                    return OriginalHook(ChiCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(TenCombo);
                                }
                            }

                            return OriginalHook(Ninjutsu);
                        }

                        if (mudrapath == 2)
                        {
                            if (Ten.LevelChecked() && actionID == Ten)
                            {
                                if (Chi.LevelChecked() && (OriginalHook(Ninjutsu) is Hyoton or HyoshoRanryu))
                                {
                                    return OriginalHook(Chi);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (Jin.LevelChecked())
                                        return OriginalHook(JinCombo);

                                    else if (Chi.LevelChecked())
                                        return OriginalHook(ChiCombo);
                                }
                            }

                            if (Chi.LevelChecked() && actionID == Chi)
                            {
                                if (Jin.LevelChecked() && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                                {
                                    return OriginalHook(Jin);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(Ten);
                                }
                            }

                            if (Jin.LevelChecked() && actionID == Jin)
                            {
                                if (OriginalHook(Ninjutsu) is Raiton)
                                {
                                    return OriginalHook(Ten);
                                }

                                if (OriginalHook(Ninjutsu) == GokaMekkyaku)
                                {
                                    return OriginalHook(Chi);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (HasEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                                        return OriginalHook(Ten);
                                    return OriginalHook(Chi);
                                }
                            }

                            return OriginalHook(Ninjutsu);
                        }
                    }
                }
                return actionID;
            }
        }
    }
}
