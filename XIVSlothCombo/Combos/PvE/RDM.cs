using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class RDM
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

        public static class Levels
        {
            public const byte
                Jolt = 2,
                Verthunder = 4,
                Corpsacorps = 6,
                Veraero = 10,
                Verthunder2 = 18,
                Veraero2 = 22,
                Verfire = 26,
                Verstone = 30,
                Verraise = 64,
                Zwerchhau = 35,
                Displacement = 40,
                Acceleration = 50,
                Redoublement = 50,
                Moulinet = 52,
                Vercure = 54,
                Embolden = 58,
                Manafication = 60,
                Jolt2 = 62,
                Impact = 66,
                ManaStack = 68,
                Verflare = 68,
                Verholy = 70,
                Fleche = 45,
                ContreSixte = 56,
                Engagement = 40,
                Scorch = 80,
                Veraero3 = 82,
                Verthunder3 = 82,
                MagickBarrier = 86,
                Resolution = 90;
        }

        public static class Config
        {
            public const string RDM_OGCD_OnAction = "RDM_OGCD_OnAction";
            public const string RDM_ST_MeleeCombo_OnAction = "RDM_ST_MeleeCombo_OnAction";
            public const string RDM_MeleeFinisher_OnAction = "RDM_MeleeFinisher_OnAction";
            public const string RDM_Lucid_Threshold = "RDM_LucidDreaming_Threshold";
            public const string RDM_MoulinetRange = "RDM_MoulinetRange";
        }


        internal class RDM_Main_Combos : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)

            {
                //MAIN_COMBO_VARIABLES
                RDMGauge gauge = GetJobGauge<RDMGauge>();
                var moulinetRange = PluginConfiguration.GetCustomIntValue(Config.RDM_MoulinetRange);
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                //END_MAIN_COMBO_VARIABLES

                //RDM_BALANCE_OPENER
                if (IsEnabled(CustomComboPreset.RDM_Balance_Opener) && level >= 90 && actionID is Jolt or Jolt2)
                {
                    bool inCombat = HasCondition(ConditionFlag.InCombat);

                    // Check to start opener
                    if (openerStarted && lastComboMove is Verthunder3 && HasEffect(Buffs.Dualcast)) { inOpener = true; openerStarted = false; readyOpener = false; }
                    if ((readyOpener || openerStarted) && !inOpener && LocalPlayer.CastActionId == Verthunder3) { openerStarted = true; return Veraero3; } else { openerStarted = false; }

                    // Reset check for opener
                    if ((IsEnabled(CustomComboPreset.RDM_Balance_Opener_AnyMana) || (gauge.BlackMana == 0 && gauge.WhiteMana == 0))
                        && IsOffCooldown(Embolden) && IsOffCooldown(Manafication) && IsOffCooldown(All.Swiftcast)
                        && GetCooldown(Acceleration).RemainingCharges == 2 && GetCooldown(Corpsacorps).RemainingCharges == 2 && GetCooldown(Engagement).RemainingCharges == 2
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
                            if (lastComboMove == Redoublement || gauge.ManaStacks == 3) step++;
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

                //RDM_ST_MANAFICATIONEMBOLDEN
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden)
                    && level >= Levels.Embolden
                    && HasCondition(ConditionFlag.InCombat)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration)
                    && (GetTargetDistance() <= 3 || (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser)
                    && GetCooldown(Corpsacorps).RemainingCharges >= 1)))
                {
                    var radioButton = PluginConfiguration.GetCustomIntValue(Config.RDM_ST_MeleeCombo_OnAction);

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte)
                        || (radioButton == 2 && actionID is Jolt or Jolt2)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Jolt or Jolt2))
                    {
                        //Situation 1: Manafication first
                        if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                            && level >= 90
                            && gauge.ManaStacks == 0
                            && lastComboMove is not Verflare
                            && lastComboMove is not Verholy
                            && lastComboMove is not Scorch
                            && System.Math.Max(black, white) <= 50
                            && (System.Math.Max(black, white) >= 42
                                || (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana) && black == white && black >= 38 && GetCooldown(Acceleration).RemainingCharges > 0))
                            && System.Math.Min(black, white) >= 31
                            && IsOffCooldown(Manafication)
                            && (IsOffCooldown(Embolden) || GetCooldown(Embolden).CooldownRemaining <= 3))
                        {
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                && black == white
                                && black <= 44
                                && black >= 38
                                && GetCooldown(Acceleration).RemainingCharges > 0)
                                return Acceleration;

                            return Manafication;
                        }
                        if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                            && level >= 90
                            && lastComboMove is Zwerchhau or EnchantedZwerchhau
                            && System.Math.Max(black, white) >= 57
                            && System.Math.Min(black, white) >= 46
                            && GetCooldown(Manafication).CooldownRemaining >= 100
                            && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }

                        //Situation 2: Embolden first
                        if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                            && level >= 90
                            && lastComboMove is Zwerchhau or EnchantedZwerchhau
                            && System.Math.Max(black, white) <= 57
                            && System.Math.Min(black, white) <= 46
                            && (GetCooldown(Manafication).CooldownRemaining <= 7 || IsOffCooldown(Manafication))
                            && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }
                        if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo)
                            && level >= 90
                            && (gauge.ManaStacks == 0 || gauge.ManaStacks == 3)
                            && lastComboMove is not Verflare 
                            && lastComboMove is not Verholy 
                            && lastComboMove is not Scorch
                            && System.Math.Max(black, white) <= 50
                            && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                            && IsOffCooldown(Manafication))
                        {
                            return Manafication;
                        }

                        //Situation 3: Just use them together
                        if ((IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo) || level < 90) 
                            && level >= Levels.Embolden 
                            && gauge.ManaStacks == 0
                            && System.Math.Max(black, white) <= 50
                            && (IsOffCooldown(Manafication) || level < Levels.Manafication)
                            && IsOffCooldown(Embolden))
                        {
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                && black == white
                                && black <= 44
                                && GetCooldown(Acceleration).RemainingCharges > 0)
                                return Acceleration;

                            return Embolden;
                        }
                        if ((IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo) || level < 90) 
                            && level >= Levels.Manafication 
                            && (gauge.ManaStacks == 0 || gauge.ManaStacks == 3)
                            && lastComboMove is not Verflare 
                            && lastComboMove is not Verholy 
                            && lastComboMove is not Scorch
                            && System.Math.Max(black, white) <= 50
                            && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                            && IsOffCooldown(Manafication))
                        {
                            return Manafication;
                        }

                        //Situation 4: Level 58 or 59
                        if (level is < Levels.Manafication and >= Levels.Embolden 
                            && System.Math.Min(black, white) >= 50 
                            && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }
                    }
                }
                //END_RDM_ST_MANAFICATIONEMBOLDEN

                //RDM_AOE_MANAFICATIONEMBOLDEN
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_ManaEmbolden)
                    && actionID is Scatter or Impact 
                    && level >= Levels.Embolden 
                    && HasCondition(ConditionFlag.InCombat)
                    && !HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast) 
                    && !HasEffect(Buffs.Acceleration)
                    && ((GetTargetDistance() <= moulinetRange && gauge.ManaStacks == 0) || gauge.ManaStacks > 0))
                {
                    //Situation 1: Embolden First (Double)
                    if (level >= Levels.Manafication
                        && gauge.ManaStacks == 2
                        && System.Math.Min(black, white) >= 22
                        && IsOffCooldown(Manafication)
                        && IsOffCooldown(Embolden))
                    {
                        return Embolden;
                    }
                    if (level >= Levels.Manafication 
                        && ((gauge.ManaStacks == 3 && System.Math.Min(black, white) >= 2) || (gauge.ManaStacks == 0 && System.Math.Min(black, white) >= 10))
                        && lastComboMove is not Verflare 
                        && lastComboMove is not Verholy 
                        && lastComboMove is not Scorch
                        && System.Math.Max(black, white) <= 50
                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                        && IsOffCooldown(Manafication))
                    {
                        return Manafication;
                    }

                    //Situation 2: Embolden First (Single)
                    if (level >= Levels.Manafication 
                        && gauge.ManaStacks == 0
                        && lastComboMove is not Verflare 
                        && lastComboMove is not Verholy 
                        && lastComboMove is not Scorch
                        && System.Math.Max(black, white) <= 50 
                        && System.Math.Min(black, white) >= 10
                        && IsOffCooldown(Manafication) 
                        && IsOffCooldown(Embolden))
                    {
                        return Embolden;
                    }
                    if (level >= Levels.Manafication 
                        && gauge.ManaStacks == 0 
                        && lastComboMove is not Verflare 
                        && lastComboMove is not Verholy 
                        && lastComboMove is not Scorch
                        && System.Math.Max(black, white) <= 50 
                        && System.Math.Min(black, white) >= 10
                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                        && IsOffCooldown(Manafication))
                    {
                        return Manafication;
                    }

                    //Below Manafication Level
                    if (level is < Levels.Manafication and >= Levels.Embolden 
                        && System.Math.Min(black, white) >= 20 
                        && IsOffCooldown(Embolden))
                    {
                        return Embolden;
                    }
                }
                //END_RDM_AOE_MANAFICATIONEMBOLDEN

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_oGCD) 
                    && level >= Levels.Corpsacorps)
                {
                    var radioButton = PluginConfiguration.GetCustomIntValue(Config.RDM_OGCD_OnAction);
                    //Radio Button Settings:
                    //1: Fleche
                    //2: Jolt
                    //3: Impact
                    //4: Jolt + Impact

                    uint placeOGCD = 0;

                    var distance = GetTargetDistance();
                    var corpacorpsRange = 25;
                    var corpsacorpsPool = 0;
                    var engagementPool = 0;

                    if (IsEnabled(CustomComboPreset.RDM_oGCD_CorpsACorps_MeleeRange)) corpacorpsRange = 3;
                    if (IsEnabled(CustomComboPreset.RDM_oGCD_CorpsACorps) && IsEnabled(CustomComboPreset.RDM_oGCD_CorpsACorps_Pooling)) corpsacorpsPool = 1;
                    if (IsEnabled(CustomComboPreset.RDM_oGCD_Engagement) && IsEnabled(CustomComboPreset.RDM_oGCD_Engagement_Pooling)) engagementPool = 1;

                    if (actionID is Jolt or Jolt2 or Scatter or Impact or Fleche or Riposte or Moulinet)
                    {
                        if (IsEnabled(CustomComboPreset.RDM_oGCD_Engagement) 
                            && (GetCooldown(Engagement).RemainingCharges > engagementPool
                                || (GetCooldown(Engagement).RemainingCharges == 1 && GetCooldown(Engagement).CooldownRemaining < 3))
                            && level >= Levels.Engagement 
                            && distance <= 3) 
                            placeOGCD = Engagement;
                        if (IsEnabled(CustomComboPreset.RDM_oGCD_CorpsACorps) 
                            && (GetCooldown(Corpsacorps).RemainingCharges > corpsacorpsPool
                                || (GetCooldown(Corpsacorps).RemainingCharges == 1 && GetCooldown(Corpsacorps).CooldownRemaining < 3))
                            && ((GetCooldown(Corpsacorps).RemainingCharges >= GetCooldown(Engagement).RemainingCharges) || level < Levels.Engagement) // Try to alternate between Corps-a-corps and Engagement
                            && level >= Levels.Corpsacorps 
                            && distance <= corpacorpsRange) 
                            placeOGCD = Corpsacorps;
                        if (IsEnabled(CustomComboPreset.RDM_oGCD_ContraSixte) 
                            && IsOffCooldown(ContreSixte) 
                            && level >= Levels.ContreSixte) 
                            placeOGCD = ContreSixte;
                        if ((radioButton == 1 || IsEnabled(CustomComboPreset.RDM_oGCD_Fleche)) 
                            && IsOffCooldown(Fleche) && level >= Levels.Fleche) 
                            placeOGCD = Fleche;

                        if ((actionID is Jolt or Jolt2) && (radioButton is 2 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                        if ((actionID is Scatter or Impact) && (radioButton is 3 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                        if ((actionID is Riposte or Moulinet) && (radioButton is 5 or 6) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                        if (actionID is Fleche && radioButton is 1 or 6 && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
                        {
                            placeOGCD = Fleche;
                            if (IsEnabled(CustomComboPreset.RDM_oGCD_ContraSixte) 
                                && level >= Levels.ContreSixte
                                && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(ContreSixte).CooldownRemaining) 
                                placeOGCD = ContreSixte;
                            if (IsEnabled(CustomComboPreset.RDM_oGCD_CorpsACorps) 
                                && level >= Levels.Corpsacorps
                                && GetCooldown(Corpsacorps).RemainingCharges == 0
                                && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(Corpsacorps).CooldownRemaining) 
                                placeOGCD = Corpsacorps;
                            if (IsEnabled(CustomComboPreset.RDM_oGCD_Engagement) 
                                && level >= Levels.Engagement
                                && GetCooldown(Engagement).RemainingCharges == 0
                                && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(Engagement).CooldownRemaining) 
                                placeOGCD = Engagement;
                        }
                        if (actionID is Fleche && radioButton == 1) return placeOGCD;
                    }
                }
                //END_RDM_OGCD

                //SYSTEM_MANA_BALANCING_MACHINE
                //Machine to decide which ver spell should be used.
                //Rules:
                //1.Avoid perfect balancing [NOT DONE]
                //   - Jolt adds 2/2 mana
                //   - Scatter/Impact adds 3/3 mana
                //   - Verstone/Verfire add 5 mana
                //   - Veraero/Verthunder add 6 mana
                //   - Veraero2/Verthunder2 add 7 mana
                //   - Verholy/Verflare add 11 mana
                //   - Scorch adds 4/4 mana
                //   - Resolution adds 4/4 mana
                //2.Stay within difference limit [DONE]
                //3.Strive to achieve correct mana for double melee combo burst [DONE]
                bool useFire = false;
                bool useStone = false;
                bool useThunder = false;
                bool useAero = false;
                bool useThunder2 = false;
                bool useAero2 = false;

                if (level >= Levels.Verthunder 
                    && (HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Acceleration)))
                {
                    if (black <= white || HasEffect(Buffs.VerstoneReady)) useThunder = true;
                    if (white <= black || HasEffect(Buffs.VerfireReady)) useAero = true;
                    if (level < Levels.Veraero) useThunder = true;
                }
                if (!HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast) 
                    && !HasEffect(Buffs.Acceleration))
                {
                    if (black <= white && HasEffect(Buffs.VerfireReady)) useFire = true;
                    if (white <= black && HasEffect(Buffs.VerstoneReady)) useStone = true;
                    if (!useFire && !useStone && HasEffect(Buffs.VerfireReady)) useFire = true;
                    if (!useFire && !useStone && HasEffect(Buffs.VerstoneReady)) useStone = true;
                }
                if (level >= Levels.Verthunder2 
                    && !HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast) 
                    && !HasEffect(Buffs.Acceleration))
                {
                    if (black <= white || level < Levels.Veraero2) useThunder2 = true;
                    else useAero2 = true;
                }
                //END_SYSTEM_MANA_BALANCING_MACHINE

                //RDM_MELEEFINISHER
                if (IsEnabled(CustomComboPreset.RDM_MeleeFinisher))
                {
                    var radioButton = PluginConfiguration.GetCustomIntValue(Config.RDM_MeleeFinisher_OnAction);

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte or Moulinet or EnchantedMoulinet)
                        || (radioButton == 2 && actionID is Jolt or Jolt2 or Scatter or Impact)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Moulinet or EnchantedMoulinet or Jolt or Jolt2 or Scatter or Impact)
                        || (radioButton == 4 && actionID is Veraero or Veraero2 or Veraero3 or Verthunder or Verthunder2 or Verthunder3))
                    {
                        if (gauge.ManaStacks >= 3)
                        {
                            if (black >= white && level >= Levels.Verholy)
                            {
                                if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                                    && !HasEffect(Buffs.VerfireReady)
                                    && (HasEffect(Buffs.VerstoneReady) && GetBuffRemainingTime(Buffs.VerstoneReady) >= 10)
                                    && (black - white <= 18))
                                    return Verflare;

                                return Verholy;
                            }
                            else if (level >= Levels.Verflare)
                            {
                                if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                                    && (HasEffect(Buffs.VerfireReady) && GetBuffRemainingTime(Buffs.VerfireReady) >= 10)
                                    && !HasEffect(Buffs.VerstoneReady)
                                    && level >= Levels.Verholy 
                                    && (white - black <= 18))
                                    return Verholy;

                                return Verflare;
                            }
                        }
                        if ((lastComboMove is Verflare or Verholy) 
                            && level >= Levels.Scorch)
                            return Scorch;

                        if (lastComboMove is Scorch 
                            && level >= Levels.Resolution)
                            return Resolution;
                    }
                }
                //END_RDM_MELEEFINISHER

                //RDM_ST_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo) 
                    && LocalPlayer.IsCasting == false)
                {
                    var radioButton = PluginConfiguration.GetCustomIntValue(Config.RDM_ST_MeleeCombo_OnAction);
                    var distance = GetTargetDistance();

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte)
                        || (radioButton == 2 && actionID is Jolt or Jolt2)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Jolt or Jolt2))
                    {
                        if ((lastComboMove is Riposte or EnchantedRiposte) 
                            && level >= Levels.Zwerchhau)
                            return OriginalHook(Zwerchhau);

                        if (lastComboMove is Zwerchhau 
                            && level >= Levels.Redoublement)
                            return OriginalHook(Redoublement);

                        if (((System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 50 && level >= Levels.Redoublement)
                            || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 35 && level < Levels.Redoublement)
                            || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 20 && level < Levels.Zwerchhau))
                            && !HasEffect(Buffs.Dualcast))
                        {
                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser) 
                                && level >= Levels.Corpsacorps && GetCooldown(Corpsacorps).RemainingCharges >= 1 
                                && distance > 3) 
                                return Corpsacorps;

                            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)
                                && level >= Levels.Acceleration
                                && black == white
                                && black >= 50
                                && !HasEffect(Buffs.Embolden))
                            {
                                if (HasEffect(Buffs.Acceleration) || WasLastAction(Buffs.Acceleration))
                                {
                                    if (useAero && level >= Levels.Veraero3) return Veraero3;
                                    if (useThunder && level >= Levels.Verthunder3) return Verthunder3;
                                    if (useAero && level < Levels.Veraero3) return Veraero;
                                    if (useThunder && level < Levels.Verthunder3) return Verthunder;
                                }

                                if (GetCooldown(Acceleration).RemainingCharges > 0)
                                    return Acceleration;
                            }

                            if (distance <= 3) 
                                return OriginalHook(Riposte);
                        }
                    }
                }
                //END_RDM_ST_MELEECOMBO

                //RDM_AOE_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo) 
                    && level >= Levels.Moulinet 
                    && actionID is Scatter or Impact 
                    && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast) 
                    && !HasEffect(Buffs.Acceleration)
                    && ((System.Math.Min(gauge.BlackMana, gauge.WhiteMana) + (gauge.ManaStacks * 20) >= 60) || (level < Levels.Verflare && System.Math.Min(gauge.BlackMana, gauge.WhiteMana) >= 20))
                    && ((GetTargetDistance() <= moulinetRange && gauge.ManaStacks == 0) || gauge.ManaStacks >= 1))
                    return OriginalHook(EnchantedMoulinet);
                //END_RDM_AOE_MELEECOMBO

                //RDM_ST_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel) 
                    && actionID is Jolt or Jolt2 
                    && HasCondition(ConditionFlag.InCombat) 
                    && LocalPlayer.IsCasting == false 
                    && gauge.ManaStacks == 0
                    && lastComboMove is not Verflare 
                    && lastComboMove is not Verholy 
                    && lastComboMove is not Scorch
                    && !HasEffect(Buffs.VerfireReady) 
                    && !HasEffect(Buffs.VerstoneReady) 
                    && !HasEffect(Buffs.Acceleration) 
                    && !HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (level >= Levels.Acceleration 
                        && GetCooldown(Acceleration).RemainingCharges > 0 
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel_Swiftcast) 
                        && level >= All.Levels.Swiftcast 
                        && IsOffCooldown(All.Swiftcast) 
                        && GetCooldown(Acceleration).RemainingCharges == 0)
                        return All.Swiftcast;
                }
                //END_RDM_ST_ACCELERATION

                //RDM_AoE_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_AoE_Accel) 
                    && actionID is Scatter or Impact 
                    && LocalPlayer.IsCasting == false 
                    && gauge.ManaStacks == 0
                    && lastComboMove is not Verflare 
                    && lastComboMove is not Verholy 
                    && lastComboMove is not Scorch 
                    && !WasLastAction(Embolden)
                    && (IsNotEnabled(CustomComboPreset.RDM_AoE_Accel_Weave) || CanSpellWeave(actionID))
                    && !HasEffect(Buffs.Acceleration) 
                    && !HasEffect(Buffs.Dualcast) 
                    && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (level >= Levels.Acceleration 
                        && GetCooldown(Acceleration).RemainingCharges > 0
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_AoE_Accel_Swiftcast) 
                        && level >= All.Levels.Swiftcast 
                        && IsOffCooldown(All.Swiftcast) 
                        && GetCooldown(Acceleration).RemainingCharges == 0
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                        return All.Swiftcast;
                }
                //END_RDM_AoE_ACCELERATION

                //RDM_VERFIREVERSTONE
                if (IsEnabled(CustomComboPreset.RDM_ST_FireStone) 
                    && actionID is Jolt or Jolt2
                    && !HasEffect(Buffs.Acceleration) 
                    && !HasEffect(Buffs.Dualcast))
                {
                    if (useFire) return Verfire;
                    if (useStone) return Verstone;
                }
                //END_RDM_VERFIREVERSTONE

                //RDM_VERTHUNDERVERAERO
                if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) 
                    && actionID is Jolt or Jolt2)
                {
                    if (useThunder) return OriginalHook(Verthunder);
                    if (useAero) return OriginalHook(Veraero);
                }
                //END_RDM_VERTHUNDERVERAERO

                //RDM_VERTHUNDERIIVERAEROII
                if (IsEnabled(CustomComboPreset.RDM_AoE_Thunder2Aero2) 
                    && actionID is Scatter or Impact)
                {
                    if (useThunder2) return Verthunder2;
                    if (useAero2) return Veraero2;
                }
                //END_RDM_VERTHUNDERIIVERAEROII


                //NO_CONDITIONS_MET
                if (level < Levels.Jolt && actionID is Jolt or Jolt2) { return Riposte; }
                return actionID;
            }
        }

        internal class RDM_Lucid : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Lucid;

            internal static bool showLucid = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Jolt or Jolt2 or Veraero or Veraero2 or Veraero3 or Verthunder or Verthunder2 or Verthunder3 or Scatter or Impact)
                {
                    var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.RDM_Lucid_Threshold);

                    if (level >= All.Levels.LucidDreaming && LocalPlayer.CurrentMp <= lucidThreshold) // Check to show Lucid Dreaming
                    {
                        showLucid = true;
                    }

                    if (showLucid && CanSpellWeave(actionID) 
                        && HasCondition(ConditionFlag.InCombat) 
                        && IsOffCooldown(All.LucidDreaming) 
                        && !HasEffect(Buffs.Dualcast)
                        && lastComboMove != EnchantedRiposte 
                        && lastComboMove != EnchantedZwerchhau
                        && lastComboMove != EnchantedRedoublement 
                        && lastComboMove != Verflare
                        && lastComboMove != Verholy 
                        && lastComboMove != Scorch) // Change abilities to Lucid Dreaming for entire weave window
                    {
                        return All.LucidDreaming;
                    }
                    showLucid = false;
                }
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
                if (actionID is All.Swiftcast && level >= Levels.Verraise)
                {
                    if (GetCooldown(All.Swiftcast).CooldownRemaining > 0 ||     // Condition 1: Swiftcast is on cooldown
                        HasEffect(Buffs.Dualcast))                              // Condition 2: Swiftcast is available, but we have Dualcast)
                        return Verraise;
                }

                // Else we just exit normally and return Swiftcast
                return actionID;
            }
        }

        internal class RDM_CorpsDisplacement : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_CorpsDisplacement;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var distance = GetTargetDistance();

                if (actionID is Displacement 
                    && level >= Levels.Displacement 
                    && HasTarget() 
                    && distance >= 5)
                    return Corpsacorps;

                return actionID;
            }
        }

        internal class RDM_EmboldenManafication : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenManafication;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Embolden 
                    && level >= Levels.Manafication 
                    && IsOnCooldown(Embolden) 
                    && IsOffCooldown(Manafication))
                    return Manafication;

                return actionID;
            }
        }

        internal class RDM_MagickBarrierAddle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickBarrierAddle;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is MagickBarrier
                    && level >= All.Levels.Addle
                    && (IsOnCooldown(MagickBarrier) || level < Levels.MagickBarrier)
                    && IsOffCooldown(All.Addle)
                    && !TargetHasEffectAny(All.Debuffs.Addle))
                    return All.Addle;

                return actionID;
            }
        }
    }
}
