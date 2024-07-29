using Dalamud.Game.ClientState.Conditions;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Combos.JobHelpers.RDMHelper;

namespace XIVSlothCombo.Combos.PvE
{
    internal class RDM
    {
        //7.0 Note
        //Gauge information is available via RDMMana
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
            Jolt3 = 37004,
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
            EnchantedMoulinetDeux = 37002,
            EnchantedMoulinetTrois = 37003,
            Corpsacorps = 7506,
            Displacement = 7515,
            Reprise = 16529,
            ViceOfThorns = 37005,
            GrandImpact = 37006,
            Prefulgence = 37007,

            //Buffs
            Acceleration = 7518,
            Manafication = 7521,
            Embolden = 7520,
            MagickBarrier = 25857;

        public static class Buffs
        {
            public const ushort
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249,
                Chainspell = 2560,
                Acceleration = 1238,
                Embolden = 1239,
                EmboldenOthers = 1297,
                Manafication = 1971,
                MagickBarrier = 2707,
                MagickedSwordPlay = 3875,
                ThornedFlourish = 3876,
                GrandImpactReady = 3877,
                PrefulugenceReady = 3878;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }



        public static class Traits
        {
            public const uint
                EnhancedEmbolden = 620,
                EnhancedManaficationII = 622,
                EnhancedManaficationIII = 622,
                EnhancedAccelerationII = 624;
        }

        public static class Config
        {
            public static UserInt
                RDM_VariantCure = new("RDM_VariantCure"),
                RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
                RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
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
                RDM_ST_oGCD_ViceOfThorns = new("RDM_ST_oGCD_ViceOfThorns"),
                RDM_ST_oGCD_Prefulgence = new("RDM_ST_oGCD_Prefulgence"),
                RDM_ST_MeleeCombo_Adv = new("RDM_ST_MeleeCombo_Adv"),
                RDM_ST_MeleeFinisher_Adv = new("RDM_ST_MeleeFinisher_Adv"),
                RDM_ST_MeleeEnforced = new("RDM_ST_MeleeEnforced"),

                RDM_AoE_oGCD_OnAction_Adv = new("RDM_AoE_oGCD_OnAction_Adv"),
                RDM_AoE_oGCD_Fleche = new("RDM_AoE_oGCD_Fleche"),
                RDM_AoE_oGCD_ContraSixte = new("RDM_AoE_oGCD_ContraSixte"),
                RDM_AoE_oGCD_Engagement = new("RDM_AoE_oGCD_Engagement"),
                RDM_AoE_oGCD_Engagement_Pooling = new("RDM_AoE_oGCD_Engagement_Pooling"),
                RDM_AoE_oGCD_CorpACorps = new("RDM_AoE_oGCD_CorpACorps"),
                RDM_AoE_oGCD_CorpACorps_Melee = new("RDM_AoE_oGCD_CorpACorps_Melee"),
                RDM_AoE_oGCD_CorpACorps_Pooling = new("RDM_AoE_oGCD_CorpACorps_Pooling"),
                RDM_AoE_oGCD_ViceOfThorns = new("RDM_AoE_oGCD_ViceOfThorns"),
                RDM_AoE_oGCD_Prefulgence = new("RDM_AoE_oGCD_Prefulgence"),
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
            internal static RDMOpenerLogic RDMOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                //MAIN_COMBO_VARIABLES

                int blackmana = RDMMana.Black;//Gauge.BlackMana;
                int whitemana = RDMMana.White;//Gauge.WhiteMana;

                //END_MAIN_COMBO_VARIABLES

                if (actionID is Jolt or Jolt2 or Jolt3)
                {
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

                    // Opener for RDM
                    if (IsEnabled(CustomComboPreset.RDM_Balance_Opener))
                    {
                        if (RDMOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }
                }

                //Lucid Dreaming
                if (IsEnabled(CustomComboPreset.RDM_ST_Lucid)
                    && actionID is Jolt or Jolt2 or Jolt3
                    && All.CanUseLucid(actionID, Config.RDM_ST_Lucid_Threshold)
                    && InCombat()
                    && RDMLucid.SafetoUse(lastComboMove)) //Don't interupt certain combos
                    return All.LucidDreaming;

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_ST_oGCD))
                {
                    bool ActionFound =
                        (!Config.RDM_ST_oGCD_OnAction_Adv && actionID is Jolt or Jolt2 or Jolt3) ||
                          (Config.RDM_ST_oGCD_OnAction_Adv &&
                            ((Config.RDM_ST_oGCD_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
                             (Config.RDM_ST_oGCD_OnAction[1] && actionID is Fleche) ||
                             (Config.RDM_ST_oGCD_OnAction[2] && actionID is Riposte) ||
                             (Config.RDM_ST_oGCD_OnAction[3] && actionID is Reprise)
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
                        (!Config.RDM_ST_MeleeFinisher_Adv && actionID is Jolt or Jolt2 or Jolt3) ||
                        (Config.RDM_ST_MeleeFinisher_Adv &&
                            ((Config.RDM_ST_MeleeFinisher_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
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
                        (!Config.RDM_ST_MeleeCombo_Adv && (actionID is Jolt or Jolt2 or Jolt3)) ||
                        (Config.RDM_ST_MeleeCombo_Adv &&
                            ((Config.RDM_ST_MeleeCombo_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
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
                                && RDMMana.ManaStacks == 0
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && RDMMana.Max <= 50
                                && (RDMMana.Max >= 42
                                    || (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana) && blackmana == whitemana && blackmana >= 38 && HasCharges(Acceleration)))
                                && RDMMana.Min >= 31
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
                                && RDMMana.Max >= 57
                                && RDMMana.Min >= 46
                                && GetCooldownRemainingTime(Manafication) >= 100
                                && IsOffCooldown(Embolden))
                            {
                                return Embolden;
                            }

                            //Situation 2: Embolden first
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && lastComboMove is Zwerchhau or EnchantedZwerchhau
                                && RDMMana.Max <= 57
                                && RDMMana.Min <= 46
                                && (GetCooldownRemainingTime(Manafication) <= 7 || IsOffCooldown(Manafication))
                                && IsOffCooldown(Embolden))
                            {
                                return Embolden;
                            }
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                                && level >= 90
                                && (RDMMana.ManaStacks == 0 || RDMMana.ManaStacks == 3)
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && RDMMana.Max <= 50
                                && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                                && IsOffCooldown(Manafication))
                            {
                                return Manafication;
                            }

                            //Situation 3: Just use them together
                            if ((IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo) || level < 90)
                                && ActionReady(Embolden)
                                && RDMMana.ManaStacks == 0
                                && RDMMana.Max <= 50
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
                                && (RDMMana.ManaStacks == 0 || RDMMana.ManaStacks == 3)
                                && lastComboMove is not Verflare
                                && lastComboMove is not Verholy
                                && lastComboMove is not Scorch
                                && RDMMana.Max <= 50
                                && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                            {
                                return Manafication;
                            }

                            //Situation 4: Level 58 or 59
                            if (!LevelChecked(Manafication) &&
                                ActionReady(Embolden) &&
                                RDMMana.Min >= 50)
                            {
                                return Embolden;
                            }

                        } //END_RDM_ST_MANAFICATIONEMBOLDEN

                        //Normal Combo
                        if (GetTargetDistance() <= 3 || Config.RDM_ST_MeleeEnforced)
                        {
                            if ((lastComboMove is Riposte or EnchantedRiposte)
                                && LevelChecked(Zwerchhau)
                                && comboTime > 0f)
                                return OriginalHook(Zwerchhau);

                            if (lastComboMove is Zwerchhau
                                && LevelChecked(Redoublement)
                                && comboTime > 0f)
                                return OriginalHook(Redoublement);
                        }

                        if (((RDMMana.Min >= 50 && LevelChecked(Redoublement))
                            || (RDMMana.Min >= 35 && !LevelChecked(Redoublement))
                            || (RDMMana.Min >= 20 && !LevelChecked(Zwerchhau)))
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
                                    var actions = RDMMana.CheckBalance();

                                    if (actions.useAero && LevelChecked(OriginalHook(Veraero))) return OriginalHook(Veraero);
                                    if (actions.useThunder && LevelChecked(OriginalHook(Verthunder))) return OriginalHook(Verthunder);
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
                    && actionID is Jolt or Jolt2 or Jolt3
                    && HasCondition(ConditionFlag.InCombat)
                    && LocalPlayer.IsCasting == false
                    && RDMMana.ManaStacks == 0
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

                if (actionID is Jolt or Jolt2 or Jolt3)
                {
                    if (TraitLevelChecked(Traits.EnhancedAccelerationII)
                        && HasEffect(Buffs.GrandImpactReady))
                        return GrandImpact;

                    //RDM_VERFIREVERSTONE
                    if (IsEnabled(CustomComboPreset.RDM_ST_FireStone)
                        && !HasEffect(Buffs.Acceleration)
                        && !HasEffect(Buffs.Dualcast))
                    {
                        //Run the Mana Balance Computer
                        var actions = RDMMana.CheckBalance();
                        if (actions.useFire) return Verfire;
                        if (actions.useStone) return Verstone;
                    }
                    //END_RDM_VERFIREVERSTONE

                    //RDM_VERTHUNDERVERAERO
                    if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero))
                    {
                        //Run the Mana Balance Computer
                        var actions = RDMMana.CheckBalance();
                        if (actions.useThunder) return OriginalHook(Verthunder);
                        if (actions.useAero) return OriginalHook(Veraero);
                    }
                    //END_RDM_VERTHUNDERVERAERO

                }

                //NO_CONDITIONS_MET
                return actionID;
            }
        }

        internal class RDM_AoE_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
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
                             (Config.RDM_AoE_MeleeFinisher_OnAction[1] && actionID is Moulinet) ||
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
                                (Config.RDM_AoE_MeleeCombo_OnAction[1] && actionID is Moulinet)));


                    if (ActionFound)
                    {
                        //Finish the combo
                        if (LevelChecked(Moulinet)
                            && lastComboMove is EnchantedMoulinet or EnchantedMoulinetDeux
                            && comboTime > 0f)
                            return OriginalHook(Moulinet);

                        //RDM_AOE_MANAFICATIONEMBOLDEN
                        if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_ManaEmbolden))
                        {
                            if (HasCondition(ConditionFlag.InCombat)
                                && !HasEffect(Buffs.Dualcast)
                                && !HasEffect(All.Buffs.Swiftcast)
                                && !HasEffect(Buffs.Acceleration)
                                && ((GetTargetDistance() <= Config.RDM_AoE_MoulinetRange && RDMMana.ManaStacks == 0) || RDMMana.ManaStacks > 0))
                            {
                                if (ActionReady(Manafication))
                                {
                                    //Situation 1: Embolden First (Double)
                                    if (RDMMana.ManaStacks == 2
                                        && RDMMana.Min >= 22
                                        && IsOffCooldown(Embolden))
                                    {
                                        return Embolden;
                                    }
                                    if (((RDMMana.ManaStacks == 3 && RDMMana.Min >= 2) || (RDMMana.ManaStacks == 0 && RDMMana.Min >= 10))
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && RDMMana.Max <= 50
                                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                                    {
                                        return Manafication;
                                    }

                                    //Situation 2: Embolden First (Single)
                                    if (RDMMana.ManaStacks == 0
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && RDMMana.Max <= 50
                                        && RDMMana.Min >= 10
                                        && IsOffCooldown(Embolden))
                                    {
                                        return Embolden;
                                    }
                                    if (RDMMana.ManaStacks == 0
                                        && lastComboMove is not Verflare
                                        && lastComboMove is not Verholy
                                        && lastComboMove is not Scorch
                                        && RDMMana.Max <= 50
                                        && RDMMana.Min >= 10
                                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                                    {
                                        return Manafication;
                                    }
                                }

                                //Below Manafication Level
                                if (ActionReady(Embolden) && !LevelChecked(Manafication)
                                    && RDMMana.Min >= 20)
                                {
                                    return Embolden;
                                }
                            }
                            //END_RDM_AOE_MANAFICATIONEMBOLDEN
                        }

                        //7.0 Manification Magic Mana
                        //int Mana = Math.Min(Gauge.WhiteMana, Gauge.BlackMana);
                        //if (LevelChecked(Manafication))
                        //{
                        //    int ManaBuff = GetBuffStacks(Buffs.MagickedSwordPlay);
                        //    if (ManaBuff > 0) Mana = 50; //ITS FREE REAL ESTATE
                        //}

                        if (LevelChecked(Moulinet)
                            && LocalPlayer.IsCasting == false
                            && !HasEffect(Buffs.Dualcast)
                            && !HasEffect(All.Buffs.Swiftcast)
                            && !HasEffect(Buffs.Acceleration)
                            && RDMMana.Min >= 50)
                        {
                            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_CorpsGapCloser)
                                && ActionReady(Corpsacorps)
                                && GetTargetDistance() > Config.RDM_AoE_MoulinetRange)
                                return Corpsacorps;

                            if ((GetTargetDistance() <= Config.RDM_AoE_MoulinetRange && RDMMana.ManaStacks == 0) || RDMMana.ManaStacks >= 1)
                                return OriginalHook(Moulinet);
                        }
                    }
                }
                //END_RDM_AOE_MELEECOMBO

                //RDM_AoE_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_AoE_Accel)
                    && actionID is Scatter or Impact
                    && LocalPlayer.IsCasting == false
                    && RDMMana.ManaStacks == 0
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

                    if (TraitLevelChecked(Traits.EnhancedAccelerationII)
                        && HasEffect(Buffs.GrandImpactReady))
                        return GrandImpact;

                    var actions = RDMMana.CheckBalance();
                    if (actions.useThunder2) return OriginalHook(Verthunder2);
                    if (actions.useAero2) return OriginalHook(Veraero2);
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

        internal class RDM_EmboldenProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Embolden &&
                ActionReady(Embolden) &&
                HasEffectAny(Buffs.EmboldenOthers) ? OriginalHook(11) : actionID;
        }

        internal class RDM_MagickProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is MagickBarrier &&
                ActionReady(MagickBarrier) &&
                HasEffectAny(Buffs.MagickBarrier) ? OriginalHook(11) : actionID;
        }
    }
}
