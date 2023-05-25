using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Combos.JobHelpers.RDM;

namespace XIVSlothCombo.Combos.PvE
{
    internal class RDM
    {
        public const byte JobID = 35;

        public const uint
            Verthunder = 7505,
            Veraero = 7507,
            Veraero2 = 16525,
            Veraero3 = 25856,
            Verthunder2 = 16524,
            Verthunder3 = 25855,
            Impact = 16526,
            Redoublement = 7516,
            EnchantedRedoublement = 7529,
            Zwerchhau = 7512,
            EnchantedZwerchhau = 7528,
            Riposte = 7504,
            EnchantedRiposte = 7527,
            Scatter = 7509,
            Verstone = 7511,
            Verfire = 7510,
            Vercure = 7514,
            Jolt = 7503,
            Jolt2 = 7524,
            Verholy = 7526,
            Verflare = 7525,
            Fleche = 7517,
            ContreSixte = 7519,
            Engagement = 16527,
            Verraise = 7523,
            Scorch = 16530,
            Resolution = 25858,
            Moulinet = 7513,
            EnchantedMoulinet = 7530,
            Corpsacorps = 7506,
            Displacement = 7515,
            Reprise = 16529,
            MagickBarrier = 25857,

            //Buffs
            Acceleration = 7518,
            Manafication = 7521,
            Embolden = 7520;

        public static class Buffs
        {
            public const ushort
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249,
                Chainspell = 2560,
                Acceleration = 1238,
                Embolden = 1239;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        protected static RDMGauge Gauge => CustomComboFunctions.GetJobGauge<RDMGauge>();

        public static class Config
        {
            public static UserInt
                RDM_VariantCure = new("RDM_VariantCure"),
                RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold"),
                RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold"),
                RDM_AoE_MoulinetRange = new("RDM_MoulinetRange");
            public static UserBool
                RDM_ST_oGCD_OnAction_Adv = new("RDM_ST_oGCD_OnAction_Adv"),
                RDM_ST_oGCD_Fleche = new("RDM_ST_oGCD_Fleche"),
                RDM_ST_oGCD_ContraSixte = new("RDM_ST_oGCD_ContraSixte"),
                RDM_ST_oGCD_Engagement = new("RDM_ST_oGCD_Engagement"),
                RDM_ST_oGCD_Engagement_Pooling = new("RDM_ST_oGCD_Engagement_Pooling"),
                RDM_ST_oGCD_CorpACorps = new("RDM_ST_oGCD_CorpACorps"),
                RDM_ST_oGCD_CorpACorps_Melee = new("RDM_ST_oGCD_CorpACorps_Melee"),
                RDM_ST_oGCD_CorpACorps_Pooling = new("RDM_ST_oGCD_CorpACorps_Pooling"),
                RDM_ST_MeleeCombo_Adv = new("RDM_ST_MeleeCombo_Adv"),
                RDM_ST_MeleeFinisher_Adv = new("RDM_ST_MeleeFinisher_Adv"),

                RDM_AoE_oGCD_OnAction_Adv = new("RDM_AoE_oGCD_OnAction_Adv"),
                RDM_AoE_oGCD_Fleche = new("RDM_AoE_oGCD_Fleche"),
                RDM_AoE_oGCD_ContraSixte = new("RDM_AoE_oGCD_ContraSixte"),
                RDM_AoE_oGCD_Engagement = new("RDM_AoE_oGCD_Engagement"),
                RDM_AoE_oGCD_Engagement_Pooling = new("RDM_AoE_oGCD_Engagement_Pooling"),
                RDM_AoE_oGCD_CorpACorps = new("RDM_AoE_oGCD_CorpACorps"),
                RDM_AoE_oGCD_CorpACorps_Melee = new("RDM_AoE_oGCD_CorpACorps_Melee"),
                RDM_AoE_oGCD_CorpACorps_Pooling = new("RDM_AoE_oGCD_CorpACorps_Pooling"),
                RDM_AoE_MeleeCombo_Adv = new("RDM_AoE_MeleeCombo_Adv"),
                RDM_AoE_MeleeFinisher_Adv = new("RDM_AoE_MeleeFinisher_Adv");                
            public static UserBoolArray
                RDM_ST_oGCD_OnAction = new("RDM_ST_oGCD_OnAction"),
                RDM_ST_MeleeCombo_OnAction = new("RDM_ST_MeleeCombo_OnAction"),
                RDM_ST_MeleeFinisher_OnAction = new("RDM_ST_MeleeFinisher_OnAction"),

                RDM_AoE_oGCD_OnAction = new("RDM_AoE_oGCD_OnAction"),
                RDM_AoE_MeleeCombo_OnAction = new("RDM_AoE_MeleeCombo_OnAction"),
                RDM_AoE_MeleeFinisher_OnAction = new("RDM_AoE_MeleeFinisher_OnAction");
        }

        internal class RDM_VariantVerCure : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Vercure && IsEnabled(CustomComboPreset.RDM_Variant_Cure2) && IsEnabled(Variant.VariantCure)
                    ? Variant.VariantCure : actionID;
        }

        internal class RDM_ST_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_DPS;

            protected internal ManaBalancer manaState = new();
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                //MAIN_COMBO_VARIABLES
                int blackmana = Gauge.BlackMana;
                int whitemana = Gauge.WhiteMana;
                //END_MAIN_COMBO_VARIABLES

                if (actionID is Jolt or Jolt2)
                {
                    //Bozja stuff - Riley (Luna)

                    if (IsEnabled(CustomComboPreset.ALL_BozjaOffClassTankSct) &&
                        IsEnabled(Bozja.LostIncense) && IsOffCooldown(Bozja.LostIncense) &&
                        HasBattleTarget())
                    {
                        //Congrats your a tank now, good luck!
                        return Bozja.LostIncense;
                    }

                    if (IsEnabled(CustomComboPreset.ALL_BozjaCureSelfheal))
                    {
                        if (IsEnabled(Bozja.LostCure4) &&
                        PlayerHealthPercentageHp() <= 50 &&
                        CanWeave(actionID))
                            return Bozja.LostCure4;

                        if (IsEnabled(Bozja.LostCure3) &&
                        PlayerHealthPercentageHp() <= 50)
                            return Bozja.LostCure3;

                        if (IsEnabled(Bozja.LostCure2) &&
                        PlayerHealthPercentageHp() <= 50 &&
                        CanWeave(actionID))
                            return Bozja.LostCure2;

                        if (IsEnabled(Bozja.LostCure) &&
                        PlayerHealthPercentageHp() <= 50)
                            return Bozja.LostCure;
                    }

                    if (IsEnabled(CustomComboPreset.ALL_BozjaCure4Caster))
                    {
                        if (IsEnabled(Bozja.LostCure4) &&
                        !HasEffect(Bozja.Buffs.LostBravery2) &&
                        CanWeave(actionID))
                            return Bozja.LostCure4;
                    }

                    if (IsEnabled(CustomComboPreset.ALL_BozjaMagicDPS))
                    {
                        if (IsEnabled(Bozja.LostExcellence) && IsOffCooldown(Bozja.LostExcellence))
                            return Bozja.LostExcellence;

                        if (IsEnabled(Bozja.FontOfMagic) && IsOffCooldown(Bozja.FontOfMagic))
                            return Bozja.FontOfMagic;

                        // Checks to see if you have Font of Magic, and lines up Banners to Font
                        if (IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                        {
                            if (HasEffect(Bozja.Buffs.FontOfMagic))
                            {
                                if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                                    return Bozja.BannerOfHonoredSacrifice;

                                if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                                    return Bozja.BannerOfNobleEnds;

                                if (IsEnabled(Bozja.LostChainspell) && IsOffCooldown(Bozja.LostChainspell))
                                    return Bozja.LostChainspell;

                                //Other devs could we check for chainspell before using swiftcast?
                            }
                        }

                        if (!IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                        {
                            if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                                return Bozja.BannerOfHonoredSacrifice;

                            if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                                return Bozja.BannerOfNobleEnds;

                            if (IsEnabled(Bozja.LostChainspell) && IsOffCooldown(Bozja.LostChainspell))
                                return Bozja.LostChainspell;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.ALL_BozjaFlareStar))
                    {
                        if (ActionReady(All.LucidDreaming))
                            return All.LucidDreaming;

                        if (IsEnabled(Bozja.LostFlareStar) && !TargetHasEffect(Bozja.Debuffs.LostFlareStar) &&
                            (LocalPlayer.CurrentMp >= 9000))
                            return Bozja.LostFlareStar;
                    }

                    //RDM_BALANCE_OPENER
                    if (IsEnabled(CustomComboPreset.RDM_Balance_Opener) && level >= 90)
                    {
                        bool inCombat = HasCondition(ConditionFlag.InCombat);

                        // Check to start opener
                        if (openerStarted && lastComboMove is Verthunder3 && HasEffect(Buffs.Dualcast)) { inOpener = true; openerStarted = false; readyOpener = false; }
                        if ((readyOpener || openerStarted) && !inOpener && LocalPlayer.CastActionId == Verthunder3) { openerStarted = true; return Veraero3; } else { openerStarted = false; }

                        // Reset check for opener
                        if ((IsEnabled(CustomComboPreset.RDM_Balance_Opener_AnyMana) || (blackmana == 0 && whitemana == 0))
                            && IsOffCooldown(Embolden) && IsOffCooldown(Manafication) && IsOffCooldown(All.Swiftcast)
                            && GetRemainingCharges(Acceleration) == 2 && GetRemainingCharges(Corpsacorps) == 2 && GetRemainingCharges(Engagement) == 2
                            && IsOffCooldown(Fleche) && IsOffCooldown(ContreSixte)
                            && GetTargetHPPercent() == 100 && !inCombat && !inOpener && !openerStarted)
                        {
                            readyOpener = true;
                            inOpener = false;
                            step = 0;
                            return Verthunder3;
                        }
                        else
                        { readyOpener = false; }

                        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                        if ((step == 0 && lastComboMove is Verthunder3 && !HasEffect(Buffs.Dualcast))
                            || (inOpener && step >= 1 && IsOffCooldown(actionID) && !inCombat)) inOpener = false;

                        // Start Opener
                        if (inOpener)
                        {
                            //veraero
                            //swiftcast
                            //accel
                            //verthunder
                            //verthunder
                            //embolden
                            //manafication
                            //Riposte
                            //Fleche
                            //Zwercchau
                            //Contre-sixte
                            //Redoublement
                            //Corps-a-corps
                            //Engagement
                            //Verholy
                            //Corps-a-corps
                            //Engagement
                            //Scorch
                            //Resolution

                            //we do it in steps to be able to control it
                            if (step == 0)
                            {
                                if (lastComboMove == Veraero3) step++;
                                else return Veraero3;
                            }

                            if (step == 1)
                            {
                                if (IsOnCooldown(All.Swiftcast)) step++;
                                else return All.Swiftcast;
                            }

                            if (step == 2)
                            {
                                if (GetRemainingCharges(Acceleration) < 2) step++;
                                else return Acceleration;
                            }

                            if (step == 3)
                            {
                                if (lastComboMove == Verthunder3 && !HasEffect(Buffs.Acceleration)) step++;
                                else return Verthunder3;
                            }

                            if (step == 4)
                            {
                                if (lastComboMove == Verthunder3 && !HasEffect(All.Buffs.Swiftcast)) step++;
                                else return Verthunder3;
                            }

                            if (step == 5)
                            {
                                if (IsOnCooldown(Embolden)) step++;
                                else return Embolden;
                            }

                            if (step == 6)
                            {
                                if (IsOnCooldown(Manafication)) step++;
                                else return Manafication;
                            }

                            if (step == 7)
                            {
                                if (lastComboMove == Riposte) step++;
                                else return EnchantedRiposte;
                            }

                            if (step == 8)
                            {
                                if (IsOnCooldown(Fleche)) step++;
                                else return Fleche;
                            }

                            if (step == 9)
                            {
                                if (lastComboMove == Zwerchhau) step++;
                                else return EnchantedZwerchhau;
                            }

                            if (step == 10)
                            {
                                if (IsOnCooldown(ContreSixte)) step++;
                                else return ContreSixte;
                            }

                            if (step == 11)
                            {
                                if (lastComboMove == Redoublement || Gauge.ManaStacks == 3) step++;
                                else return EnchantedRedoublement;
                            }

                            if (step == 12)
                            {
                                if (GetRemainingCharges(Corpsacorps) < 2) step++;
                                else return Corpsacorps;
                            }

                            if (step == 13)
                            {
                                if (GetRemainingCharges(Engagement) < 2) step++;
                                else return Engagement;
                            }

                            if (step == 14)
                            {
                                if (lastComboMove == Verholy) step++;
                                else return Verholy;
                            }

                            if (step == 15)
                            {
                                if (GetRemainingCharges(Corpsacorps) < 1) step++;
                                else return Corpsacorps;
                            }

                            if (step == 16)
                            {
                                if (GetRemainingCharges(Engagement) < 1) step++;
                                else return Engagement;
                            }

                            if (step == 17)
                            {
                                if (lastComboMove == Scorch) step++;
                                else return Scorch;
                            }

                            if (step == 18)
                            {
                                if (lastComboMove == Resolution) step++;
                                else return Resolution;
                            }

                            inOpener = false;
                        }
                    }
                    //END_RDM_BALANCE_OPENER

                    //VARIANTS
                    if (IsEnabled(CustomComboPreset.RDM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.RDM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.RDM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;
                }

                //Lucid Dreaming
                if (IsEnabled(CustomComboPreset.RDM_ST_Lucid)
                    && actionID is Jolt or Jolt2
                    && All.CanUseLucid(actionID, Config.RDM_ST_Lucid_Threshold)
                    && InCombat()
                    && RDMLucid.SafetoUse(lastComboMove)) //Don't interupt certain combos
                    return All.LucidDreaming;

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_ST_oGCD))
                {
                    bool ActionFound = 
                        ( (!Config.RDM_ST_oGCD_OnAction_Adv && actionID is Jolt or Jolt2) || 
                          (Config.RDM_ST_oGCD_OnAction_Adv &&
                            ((Config.RDM_ST_oGCD_OnAction[0] && actionID is Jolt or Jolt2) ||
                             (Config.RDM_ST_oGCD_OnAction[1] && actionID is Fleche) ||
                             (Config.RDM_ST_oGCD_OnAction[2] && actionID is Riposte) ||
                             (Config.RDM_ST_oGCD_OnAction[3] && actionID is Reprise)
                            )
                          )
                        );
                    if (ActionFound && LevelChecked(Corpsacorps))
                    {
                        if (OGCDHelper.CanUse(actionID, true, out uint oGCDAction)) return oGCDAction;
                    }
                }
                //END_RDM_OGCD

                //RDM_MELEEFINISHER
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeFinisher))
                {
                    bool ActionFound =
                        (!Config.RDM_ST_MeleeFinisher_Adv && actionID is Jolt or Jolt2) ||
                        (Config.RDM_ST_MeleeFinisher_Adv &&
                            ((Config.RDM_ST_MeleeFinisher_OnAction[0] && actionID is Jolt or Jolt2) ||
                             (Config.RDM_ST_MeleeFinisher_OnAction[1] && actionID is Riposte or EnchantedRiposte) || 
                             (Config.RDM_ST_MeleeFinisher_OnAction[2] && actionID is Veraero or Veraero3 or Verthunder or Verthunder3)));

                    if (ActionFound && MeleeFinisher.CanUse(lastComboMove, out uint finisherAction))
                        return finisherAction;
                }
                //END_RDM_MELEEFINISHER

                //RDM_ST_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo)
                    && LocalPlayer.IsCasting == false)
                {
                    bool ActionFound =
                        (!Config.RDM_ST_MeleeCombo_Adv && actionID is Jolt or Jolt2) ||
                        (Config.RDM_ST_MeleeCombo_Adv &&
                            ((Config.RDM_ST_MeleeCombo_OnAction[0] && actionID is Jolt or Jolt2) ||
                             (Config.RDM_ST_MeleeCombo_OnAction[1] && actionID is Riposte or EnchantedRiposte)));

                    if (ActionFound)
                    {
                        //RDM_ST_MANAFICATIONEMBOLDEN
                        if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden)
                            && LevelChecked(Embolden)
                            && HasCondition(ConditionFlag.InCombat)
                            && !HasEffect(Buffs.Dualcast)
                            && !HasEffect(All.Buffs.Swiftcast)
                            && !HasEffect(Buffs.Acceleration)
                            && (GetTargetDistance() <= 3 || (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser) && HasCharges(Corpsacorps))))
                        { 
                            //Situation 1: Manafication first
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && Gauge.ManaStacks == 0
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && Math.Max(blackmana, whitemana) <= 50
                                && (Math.Max(blackmana, whitemana) >= 42
                                    || (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana) && blackmana == whitemana && blackmana >= 38 && HasCharges(Acceleration)))
                                && Math.Min(blackmana, whitemana) >= 31
                                && IsOffCooldown(Manafication)
                                && (IsOffCooldown(Embolden) || GetCooldownRemainingTime(Embolden) <= 3))
                            {
                                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                    && blackmana == whitemana
                                    && blackmana <= 44
                                    && blackmana >= 38
                                    && HasCharges(Acceleration))
                                    return Acceleration;

                                return Manafication;
                            }
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && lastComboMove is Zwerchhau or EnchantedZwerchhau
                                && Math.Max(blackmana, whitemana) >= 57
                                && Math.Min(blackmana, whitemana) >= 46
                                && GetCooldownRemainingTime(Manafication) >= 100
                                && IsOffCooldown(Embolden))
                            {
                                return Embolden;
                            }

                            //Situation 2: Embolden first
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && lastComboMove is Zwerchhau or EnchantedZwerchhau
                                && Math.Max(blackmana, whitemana) <= 57
                                && Math.Min(blackmana, whitemana) <= 46
                                && (GetCooldownRemainingTime(Manafication) <= 7 || IsOffCooldown(Manafication))
                                && IsOffCooldown(Embolden))
                            {
                                return Embolden;
                            }
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && (Gauge.ManaStacks == 0 || Gauge.ManaStacks == 3)
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && Math.Max(blackmana, whitemana) <= 50
                                && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                                && IsOffCooldown(Manafication))
                            {
                                return Manafication;
                            }

                            //Situation 3: Just use them together
                            if ((IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo) || level < 90)
                                && ActionReady(Embolden)
                                && Gauge.ManaStacks == 0
                                && Math.Max(blackmana, whitemana) <= 50
                                && (IsOffCooldown(Manafication) || !LevelChecked(Manafication)))
                            {
                                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                    && blackmana == whitemana
                                    && blackmana <= 44
                                    && HasCharges(Acceleration))
                                    return Acceleration;

                                return Embolden;
                            }
                            if ((IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo) || level < 90)
                                && ActionReady(Manafication)
                                && (Gauge.ManaStacks == 0 || Gauge.ManaStacks == 3)
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && Math.Max(blackmana, whitemana) <= 50
                                && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                            {
                                return Manafication;
                            }

                            //Situation 4: Level 58 or 59
                            if (!LevelChecked(Manafication) &&
                                ActionReady(Embolden) &&
                                Math.Min(blackmana, whitemana) >= 50)
                            {
                                return Embolden;
                            }

                        } //END_RDM_ST_MANAFICATIONEMBOLDEN

                        //Normal Combo
                        if ((lastComboMove is Riposte or EnchantedRiposte)
                            && LevelChecked(Zwerchhau))
                            return OriginalHook(Zwerchhau);

                        if (lastComboMove is Zwerchhau
                            && LevelChecked(Redoublement))
                            return OriginalHook(Redoublement);

                        if (((Math.Min(Gauge.WhiteMana, Gauge.BlackMana) >= 50 && LevelChecked(Redoublement))
                            || (Math.Min(Gauge.WhiteMana, Gauge.BlackMana) >= 35 && !LevelChecked(Redoublement))
                            || (Math.Min(Gauge.WhiteMana, Gauge.BlackMana) >= 20 && !LevelChecked(Zwerchhau)))
                            && !HasEffect(Buffs.Dualcast))
                        {
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser)
                                && LevelChecked(Corpsacorps) && HasCharges(Corpsacorps)
                                && GetTargetDistance() > 3)
                                return Corpsacorps;

                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                && LevelChecked(Acceleration)
                                && blackmana == whitemana
                                && blackmana >= 50
                                && !HasEffect(Buffs.Embolden))
                            {
                                if (HasEffect(Buffs.Acceleration) || WasLastAction(Buffs.Acceleration))
                                {
                                    //Run the Mana Balance Computer
                                    manaState.CheckBalance();
                                    if (manaState.useAero && LevelChecked(OriginalHook(Veraero))) return OriginalHook(Veraero);
                                    if (manaState.useThunder && LevelChecked(OriginalHook(Verthunder))) return OriginalHook(Verthunder);
                                }

                                if (HasCharges(Acceleration)) return Acceleration;
                            }

                            if (GetTargetDistance() <= 3)
                                return OriginalHook(Riposte);
                        }
                    }
                }
                //END_RDM_ST_MELEECOMBO

                //RDM_ST_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel)
                    && actionID is Jolt or Jolt2
                    && HasCondition(ConditionFlag.InCombat)
                    && LocalPlayer.IsCasting == false
                    && Gauge.ManaStacks == 0
                    && lastComboMove is not Verflare
                    && lastComboMove is not Verholy
                    && lastComboMove is not Scorch
                    && !HasEffect(Buffs.VerfireReady)
                    && !HasEffect(Buffs.VerstoneReady)
                    && !HasEffect(Buffs.Acceleration)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (ActionReady(Acceleration)
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel_Swiftcast)
                        && ActionReady(All.Swiftcast)
                        && !HasCharges(Acceleration))
                        return All.Swiftcast;
                }
                //END_RDM_ST_ACCELERATION

                //RDM_VERFIREVERSTONE
                if (IsEnabled(CustomComboPreset.RDM_ST_FireStone)
                    && actionID is Jolt or Jolt2
                    && !HasEffect(Buffs.Acceleration)
                    && !HasEffect(Buffs.Dualcast))
                {
                    //Run the Mana Balance Computer
                    manaState.CheckBalance();
                    if (manaState.useFire) return Verfire;
                    if (manaState.useStone) return Verstone;
                }
                //END_RDM_VERFIREVERSTONE

                //RDM_VERTHUNDERVERAERO
                if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero)
                    && actionID is Jolt or Jolt2)
                {
                    //Run the Mana Balance Computer
                    manaState.CheckBalance();
                    if (manaState.useThunder) return OriginalHook(Verthunder);
                    if (manaState.useAero) return OriginalHook(Veraero);
                }
                //END_RDM_VERTHUNDERVERAERO

                //NO_CONDITIONS_MET
                if (!LevelChecked(Jolt)) return Riposte;
                return actionID;
            }
        }

        internal class RDM_AoE_DPS : CustomCombo
        {
            protected internal ManaBalancer manaState = new();
            protected internal MeleeFinisher meleeFinisher = new();
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int black = Gauge.BlackMana;
                int white = Gauge.WhiteMana;

                // LUCID
                if (IsEnabled(CustomComboPreset.RDM_AoE_Lucid)
                    && actionID is Scatter or Impact
                    && All.CanUseLucid(actionID, Config.RDM_AoE_Lucid_Threshold)
                    && InCombat()
                    && RDMLucid.SafetoUse(lastComboMove))
                    return All.LucidDreaming;

                    //RDM_OGCD
                    if (IsEnabled(CustomComboPreset.RDM_AoE_oGCD)
                    && LevelChecked(Corpsacorps) &&
                    actionID is Scatter or Impact &&
                    OGCDHelper.CanUse(actionID, false, out uint oGCDAction)) return oGCDAction;

                //RDM_MELEEFINISHER
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeFinisher))
                {
                    bool ActionFound =
                        (!Config.RDM_AoE_MeleeFinisher_Adv && actionID is Scatter or Impact) ||
                        (Config.RDM_AoE_MeleeFinisher_Adv &&
                            ((Config.RDM_AoE_MeleeFinisher_OnAction[0] && actionID is Scatter or Impact) ||
                             (Config.RDM_AoE_MeleeFinisher_OnAction[1] && actionID is Moulinet or EnchantedMoulinet) ||
                             (Config.RDM_AoE_MeleeFinisher_OnAction[2] && actionID is Veraero2 or Verthunder2)));


                    if (ActionFound && MeleeFinisher.CanUse(lastComboMove, out uint finisherAction))
                        return finisherAction;
                }
                //END_RDM_MELEEFINISHER

                //RDM_AOE_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo))
                {
                    bool ActionFound =
                        (!Config.RDM_AoE_MeleeCombo_Adv && actionID is Scatter or Impact) ||
                        (Config.RDM_AoE_MeleeCombo_Adv &&
                            ((Config.RDM_AoE_MeleeCombo_OnAction[0] && actionID is Scatter or Impact) ||
                                (Config.RDM_AoE_MeleeCombo_OnAction[1] && actionID is Moulinet or EnchantedMoulinet)));


                    if (ActionFound)
                    {
                        //RDM_AOE_MANAFICATIONEMBOLDEN
                        if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_ManaEmbolden))
                        {
                            if (HasCondition(ConditionFlag.InCombat)
                                && !HasEffect(Buffs.Dualcast)
                                && !HasEffect(All.Buffs.Swiftcast)
                                && !HasEffect(Buffs.Acceleration)
                                && ((GetTargetDistance() <= Config.RDM_AoE_MoulinetRange && Gauge.ManaStacks == 0) || Gauge.ManaStacks > 0))
                            {
                                if (ActionReady(Manafication))
                                {
                                    //Situation 1: Embolden First (Double)
                                    if (Gauge.ManaStacks == 2
                                        && Math.Min(black, white) >= 22
                                        && IsOffCooldown(Embolden))
                                    {
                                        return Embolden;
                                    }
                                    if (((Gauge.ManaStacks == 3 && Math.Min(black, white) >= 2) || (Gauge.ManaStacks == 0 && Math.Min(black, white) >= 10))
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && Math.Max(black, white) <= 50
                                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                                    {
                                        return Manafication;
                                    }

                                    //Situation 2: Embolden First (Single)
                                    if (Gauge.ManaStacks == 0
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && Math.Max(black, white) <= 50
                                        && Math.Min(black, white) >= 10
                                        && IsOffCooldown(Embolden))
                                    {
                                        return Embolden;
                                    }
                                    if (Gauge.ManaStacks == 0
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && Math.Max(black, white) <= 50
                                        && Math.Min(black, white) >= 10
                                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                                    {
                                        return Manafication;
                                    }
                                }

                                //Below Manafication Level
                                if (ActionReady(Embolden) && !LevelChecked(Manafication)
                                    && Math.Min(black, white) >= 20)
                                {
                                    return Embolden;
                                }
                            }
                            //END_RDM_AOE_MANAFICATIONEMBOLDEN
                        }

                        if (LevelChecked(Moulinet)
                            && LocalPlayer.IsCasting == false
                            && !HasEffect(Buffs.Dualcast)
                            && !HasEffect(All.Buffs.Swiftcast)
                            && !HasEffect(Buffs.Acceleration)
                            && ((Math.Min(Gauge.BlackMana, Gauge.WhiteMana) + (Gauge.ManaStacks * 20) >= 60) || 
                                (!LevelChecked(Verflare) && Math.Min(Gauge.BlackMana, Gauge.WhiteMana) >= 20)))
                        {
                            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_CorpsGapCloser)
                                && ActionReady(Corpsacorps)
                                && GetTargetDistance() > Config.RDM_AoE_MoulinetRange)
                                return Corpsacorps;

                            if ((GetTargetDistance() <= Config.RDM_AoE_MoulinetRange && Gauge.ManaStacks == 0) || Gauge.ManaStacks >= 1)
                            return OriginalHook(Moulinet);
                        }
                    }
                }
                //END_RDM_AOE_MELEECOMBO

                //RDM_AoE_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_AoE_Accel)
                    && actionID is Scatter or Impact
                    && LocalPlayer.IsCasting == false
                    && Gauge.ManaStacks == 0
                    && lastComboMove is not Verflare
                    && lastComboMove is not Verholy
                    && lastComboMove is not Scorch
                    && !WasLastAction(Embolden)
                    && (IsNotEnabled(CustomComboPreset.RDM_AoE_Accel_Weave) || CanSpellWeave(actionID))
                    && !HasEffect(Buffs.Acceleration)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast))
                {
                    //if (level >= Levels.Acceleration
                    //    && GetCooldown(Acceleration).RemainingCharges > 0
                    //    && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                    if (ActionReady(Acceleration) //check for level and 1 charge minimum
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_AoE_Accel_Swiftcast)
                        && ActionReady(All.Swiftcast)
                        && !HasCharges(Acceleration)
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return All.Swiftcast;
                }
                //END_RDM_AoE_ACCELERATION

                //RDM_VERTHUNDERIIVERAEROII
                if (actionID is Scatter or Impact)
                {
                    manaState.CheckBalance();
                    if (manaState.useThunder2) return OriginalHook(Verthunder2);
                    if (manaState.useAero2) return OriginalHook(Veraero2);
                }
                //END_RDM_VERTHUNDERIIVERAEROII

                return actionID;
            }
        }

        /*
        RDM_Verraise
        Swiftcast combos to Verraise when:
        -Swiftcast is on cooldown.
        -Swiftcast is available, but we we have Dualcast (Dualcasting Verraise)
        Using this variation other than the alternate feature style, as Verraise is level 63
        and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
        */
        internal class RDM_Verraise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast)
                {
                    if (HasEffect(All.Buffs.Swiftcast) && IsEnabled(CustomComboPreset.SMN_Variant_Raise) && IsEnabled(Variant.VariantRaise))
                        return Variant.VariantRaise;

                    if (LevelChecked(Verraise) &&
                        (GetCooldownRemainingTime(All.Swiftcast) > 0 ||     // Condition 1: Swiftcast is on cooldown
                        HasEffect(Buffs.Dualcast)))                              // Condition 2: Swiftcast is available, but we have Dualcast)
                        return Verraise;
                }

                // Else we just exit normally and return Swiftcast
                return actionID;
            }
        }

        internal class RDM_CorpsDisplacement : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_CorpsDisplacement;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) => 
                actionID is Displacement
                && LevelChecked(Displacement)
                && HasTarget()
                && GetTargetDistance() >= 5 ? Corpsacorps : actionID;
        }

        internal class RDM_EmboldenManafication : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenManafication;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) => 
                actionID is Embolden
                && IsOnCooldown(Embolden)
                && ActionReady(Manafication) ? Manafication : actionID;
        }

        internal class RDM_MagickBarrierAddle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickBarrierAddle;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) => 
                actionID is MagickBarrier
                && (IsOnCooldown(MagickBarrier) || !LevelChecked(MagickBarrier))
                && ActionReady(All.Addle)
                && !TargetHasEffectAny(All.Debuffs.Addle) ? All.Addle : actionID;
        }
    }
}
