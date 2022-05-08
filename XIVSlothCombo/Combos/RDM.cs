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
                Embolden = 2282;
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
                Resolution = 90;
        }

        public static class Config
        {
            //            public const string RdmLucidMpThreshold = "RdmLucidMpThreshold";
            public const string RDM_OGCD_OnAction = "RDM_OGCD_OnAction";
            public const string RDM_ST_MeleeCombo_OnAction = "RDM_ST_MeleeCombo_OnAction";
            public const string RDM_MeleeFinisher_OnAction = "RDM_MeleeFinisher_OnAction";
            public const string RDM_LucidDreaming_Threshold = "RDM_LucidDreaming_Threshold";
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
                    if ((IsEnabled(CustomComboPreset.RDM_Opener_Any_Mana) || (gauge.BlackMana == 0 && gauge.WhiteMana == 0))
                        && IsOffCooldown(Embolden) && IsOffCooldown(Manafication) && IsOffCooldown(All.Swiftcast)
                        && GetCooldown(Acceleration).RemainingCharges == 2 && GetCooldown(Corpsacorps).RemainingCharges == 2 && GetCooldown(Engagement).RemainingCharges == 2
                        && IsOffCooldown(Fleche) && IsOffCooldown(ContreSixte)
                        && EnemyHealthPercentage() == 100 && !inOpener && !openerStarted)
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
                if (IsEnabled(CustomComboPreset.RDM_ST_ManaficationEmbolden) && level >= Levels.Embolden && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration)
                    && (GetTargetDistance() <= 3 || (IsEnabled(CustomComboPreset.RDM_ST_CorpsGapClose) && GetCooldown(Corpsacorps).RemainingCharges >= 1)))
                {
                    var radioButton = Service.Configuration.GetCustomIntValue(Config.RDM_ST_MeleeCombo_OnAction);

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte)
                        || (radioButton == 2 && actionID is Jolt or Jolt2)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Jolt or Jolt2))
                    {
                        //Situation 1: Manafication first
                        if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                        && System.Math.Max(black, white) <= 50 && System.Math.Max(black, white) >= 42 && System.Math.Min(black, white) >= 31
                        && IsOffCooldown(Manafication) && gauge.ManaStacks == 0
                        && (IsOffCooldown(Embolden) || GetCooldown(Embolden).CooldownRemaining <= 3))
                        {
                            return Manafication;
                        }
                        if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                            && lastComboMove is Zwerchhau && level >= Levels.Redoublement
                            && System.Math.Max(black, white) >= 55 && System.Math.Min(black, white) >= 46
                            && GetCooldown(Manafication).CooldownRemaining >= 100
                            && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }

                        //Situation 2: Embolden first
                        if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                            && lastComboMove is Zwerchhau && level >= Levels.Redoublement
                            && System.Math.Max(black, white) <= 57 && System.Math.Min(black, white) <= 46
                            && (GetCooldown(Manafication).CooldownRemaining <= 7 || IsOffCooldown(Manafication))
                            && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }
                        if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                            && System.Math.Max(black, white) <= 50
                            && IsOffCooldown(Manafication)
                            && (gauge.ManaStacks == 3 || lastComboMove is Resolution)
                            && GetCooldown(Embolden).CooldownRemaining >= 105)
                        {
                            return Manafication;
                        }

                        //Situation 3: Just use them together
                        if ((IsNotEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) || level < 90) && level >= Levels.Embolden
                            && System.Math.Max(black, white) <= 50
                            && (IsOffCooldown(Manafication) || level < Levels.Manafication) && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }
                        if ((IsNotEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) || level < 90) && level >= Levels.Manafication
                            && System.Math.Max(black, white) <= 50
                            && IsOffCooldown(Manafication) && gauge.ManaStacks == 0
                            && GetCooldown(Embolden).CooldownRemaining >= 110)
                        {
                            return Manafication;
                        }

                        //Situation 4: Level 58 or 59
                        if (level < Levels.Manafication && level >= Levels.Embolden
                            && System.Math.Min(black, white) >= 50 && IsOffCooldown(Embolden))
                        {
                            return Embolden;
                        }
                    }
                }
                //END_RDM_ST_MANAFICATIONEMBOLDEN

                //RDM_AOE_MANAFICATIONEMBOLDEN
                if (IsEnabled(CustomComboPreset.RDM_AoE_ManaficationEmbolden) && level >= Levels.Embolden && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration)
                    && GetTargetDistance() < 8 && actionID is Scatter or Impact)
                {
                    if (level >= Levels.Manafication
                    && System.Math.Max(black, white) <= 50 && System.Math.Min(black, white) >= 10
                    && (IsOffCooldown(Manafication) || level < Levels.Manafication) && IsOffCooldown(Embolden))
                    {
                        return Embolden;
                    }
                    if (level >= Levels.Manafication
                        && System.Math.Max(black, white) <= 50 && System.Math.Min(black, white) >= 10
                        && IsOffCooldown(Manafication) && gauge.ManaStacks == 0
                        && GetCooldown(Embolden).CooldownRemaining >= 110)
                    {
                        return Manafication;
                    }
                    if (level < Levels.Manafication && level >= Levels.Embolden
                        && System.Math.Min(black, white) >= 20 && IsOffCooldown(Embolden))
                    {
                        return Embolden;
                    }
                }
                //END_RDM_AOE_MANAFICATIONEMBOLDEN

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_OGCD) && level >= Levels.Corpsacorps)
                {
                    var radioButton = Service.Configuration.GetCustomIntValue(Config.RDM_OGCD_OnAction);
                    //Radio Button Settings:
                    //1: Fleche
                    //2: Jolt
                    //3: Impact
                    //4: Jolt + Impact

                    uint placeOGCD = 0;

                    var distance = GetTargetDistance();
                    var corpacorpsRange = 25;
                    var corpsacorpsPool = 0;

                    if (IsEnabled(CustomComboPreset.RDM_Corpsacorps_MeleeRange)) corpacorpsRange = 3;
                    if (IsEnabled(CustomComboPreset.RDM_ST_CorpsGapClose) && IsEnabled(CustomComboPreset.RDM_ST_PoolCorps)) corpsacorpsPool = 1;

                    if (actionID is Jolt or Jolt2 or Scatter or Impact or Fleche)
                    {
                        if (IsEnabled(CustomComboPreset.RDM_Engagement) && GetCooldown(Engagement).RemainingCharges > 0
                            && level >= Levels.Engagement && distance <= 3) placeOGCD = Engagement;
                        if (IsEnabled(CustomComboPreset.RDM_Corpsacorps) && GetCooldown(Corpsacorps).RemainingCharges > corpsacorpsPool
                            && ((GetCooldown(Corpsacorps).RemainingCharges >= GetCooldown(Engagement).RemainingCharges) || level < Levels.Engagement) // Try to alternate between Corps-a-corps and Engagement
                            && level >= Levels.Corpsacorps && distance <= corpacorpsRange) placeOGCD = Corpsacorps;
                        if (IsEnabled(CustomComboPreset.RDM_ContraSixte) && IsOffCooldown(ContreSixte) && level >= Levels.ContreSixte) placeOGCD = ContreSixte;
                        if ((radioButton == 1 || IsEnabled(CustomComboPreset.RDM_Fleche)) && IsOffCooldown(Fleche) && level >= Levels.Fleche) placeOGCD = Fleche;

                        if ((actionID is Jolt or Jolt2) && (radioButton is 2 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                        if ((actionID is Scatter or Impact) && (radioButton is 3 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                        if (actionID is Fleche && radioButton == 1 && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
                        {
                            placeOGCD = Fleche;
                            if (IsEnabled(CustomComboPreset.RDM_ContraSixte) && level >= Levels.ContreSixte
                                && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(ContreSixte).CooldownRemaining) placeOGCD = ContreSixte;
                            if (IsEnabled(CustomComboPreset.RDM_Corpsacorps) && level >= Levels.Corpsacorps
                                && ((IsNotEnabled(CustomComboPreset.RDM_ST_PoolCorps) && GetCooldown(Corpsacorps).RemainingCharges >= 0) || GetCooldown(Corpsacorps).RemainingCharges >= 2)
                                && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(Corpsacorps).ChargeCooldownRemaining
                                && distance <= corpacorpsRange) placeOGCD = Corpsacorps;
                            if (placeOGCD == Corpsacorps)
                            {
                                if (IsEnabled(CustomComboPreset.RDM_Engagement) && level >= Levels.Engagement
                                    && GetCooldown(placeOGCD).ChargeCooldownRemaining > GetCooldown(Engagement).ChargeCooldownRemaining
                                    && distance <= 3) placeOGCD = Engagement;
                            }
                            else if (IsEnabled(CustomComboPreset.RDM_Engagement)
                              && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(Engagement).ChargeCooldownRemaining
                              && distance <= 3) placeOGCD = Engagement;
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

                if (level >= Levels.Verthunder && (HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Acceleration)))
                {
                    if (black <= white || HasEffect(Buffs.VerstoneReady)) useThunder = true;
                    if (white <= black || HasEffect(Buffs.VerfireReady)) useAero = true;
                    if (level < Levels.Veraero) useThunder = true;
                }
                if (!HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration))
                {
                    if (black <= white && HasEffect(Buffs.VerfireReady)) useFire = true;
                    if (white <= black && HasEffect(Buffs.VerstoneReady)) useStone = true;
                    if (!useFire && !useStone && HasEffect(Buffs.VerfireReady)) useFire = true;
                    if (!useFire && !useStone && HasEffect(Buffs.VerstoneReady)) useStone = true;
                }
                if (level >= Levels.Verthunder2 && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration))
                {
                    if (black <= white || level < Levels.Veraero2) useThunder2 = true;
                    else useAero2 = true;
                }
                //END_SYSTEM_MANA_BALANCING_MACHINE

                //RDM_MELEEFINISHER
                if (IsEnabled(CustomComboPreset.RDM_MeleeFinisher))
                {
                    var radioButton = Service.Configuration.GetCustomIntValue(Config.RDM_MeleeFinisher_OnAction);

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte or Moulinet or EnchantedMoulinet)
                        || (radioButton == 2 && actionID is Jolt or Jolt2 or Scatter or Impact)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Moulinet or EnchantedMoulinet or Jolt or Jolt2 or Scatter or Impact)
                        || (radioButton == 4 && actionID is Veraero or Veraero2 or Veraero3 or Verthunder or Verthunder2 or Verthunder3))
                    {
                        if (gauge.ManaStacks >= 3)
                        {
                            if (black >= white && level >= Levels.Verholy)
                            {
                                if (HasEffect(Buffs.VerstoneReady) && (!HasEffect(Buffs.VerfireReady) || HasEffect(Buffs.Embolden)) && (black - white <= 9))
                                    return Verflare;

                                return Verholy;
                            }
                            else if (level >= Levels.Verflare)
                            {
                                if (!HasEffect(Buffs.VerstoneReady) && (HasEffect(Buffs.VerfireReady) || HasEffect(Buffs.Embolden)) && level >= Levels.Verholy && (white - black <= 9))
                                    return Verholy;

                                return Verflare;
                            }
                        }
                        if ((lastComboMove is Verflare or Verholy) && level >= Levels.Scorch)
                            return Scorch;

                        if (lastComboMove is Scorch && level >= Levels.Resolution)
                            return Resolution;
                    }
                }
                //END_RDM_MELEEFINISHER

                //RDM_ST_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo) && LocalPlayer.IsCasting == false)
                {
                    var radioButton = Service.Configuration.GetCustomIntValue(Config.RDM_ST_MeleeCombo_OnAction);
                    var distance = GetTargetDistance();

                    if ((radioButton == 1 && actionID is Riposte or EnchantedRiposte)
                        || (radioButton == 2 && actionID is Jolt or Jolt2)
                        || (radioButton == 3 && actionID is Riposte or EnchantedRiposte or Jolt or Jolt2))
                    {
                        if ((lastComboMove is Riposte or EnchantedRiposte) && level >= Levels.Zwerchhau)
                            return OriginalHook(Zwerchhau);

                        if (lastComboMove is Zwerchhau && level >= Levels.Redoublement)
                            return OriginalHook(Redoublement);

                        if (((System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 50 && level >= Levels.Redoublement)
                                || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 35 && level < Levels.Redoublement)
                                || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 20 && level < Levels.Zwerchhau))
                            && (!HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration))) //Not sure if Swift and Accel are necessary, but better to clear I think.
                        {
                            if (IsEnabled(CustomComboPreset.RDM_ST_CorpsGapClose) && level >= Levels.Corpsacorps && GetCooldown(Corpsacorps).RemainingCharges >= 1 && distance > 3) return Corpsacorps;
                            if (distance <= 3) return OriginalHook(Riposte);
                        }
                    }
                }
                //END_RDM_ST_MELEECOMBO

                //RDM_AOE_MELEECOMBO
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo) && level >= Levels.Moulinet && actionID is Scatter or Impact && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Acceleration)
                    && (System.Math.Min(gauge.BlackMana, gauge.WhiteMana) + (gauge.ManaStacks * 20) >= 60 || (level < Levels.Manafication && System.Math.Min(gauge.BlackMana, gauge.WhiteMana) >= 20))
                    && ((GetTargetDistance() <= 7 && gauge.ManaStacks == 0) || gauge.ManaStacks > 0))
                    return OriginalHook(EnchantedMoulinet);
                //END_RDM_AOE_MELEECOMBO

                //RDM_ST_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_ST_Acceleration) && actionID is Jolt or Jolt2 && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.VerfireReady) && !HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.Acceleration) && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (level >= Levels.Acceleration && GetCooldown(Acceleration).RemainingCharges > 0 && GetCooldown(Acceleration).ChargeCooldownRemaining < 54)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_ST_AccelSwiftCast) && level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) && GetCooldown(Acceleration).RemainingCharges == 0)
                        return All.Swiftcast;
                }
                //END_RDM_ST_ACCELERATION

                //RDM_AoE_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_AoE_Acceleration) && actionID is Scatter or Impact && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Acceleration) && !HasEffect(Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (level >= Levels.Acceleration && GetCooldown(Acceleration).RemainingCharges > 0 && GetCooldown(Acceleration).ChargeCooldownRemaining < 54)
                        return Acceleration;
                    if (IsEnabled(CustomComboPreset.RDM_AoE_AccelSwiftCast) && level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) && GetCooldown(Acceleration).RemainingCharges == 0)
                        return All.Swiftcast;
                }
                //END_RDM_AoE_ACCELERATION

                //RDM_VERFIREVERSTONE
                if (IsEnabled(CustomComboPreset.RDM_VerfireVerstone) && actionID is Jolt or Jolt2
                    && !HasEffect(Buffs.Acceleration) && !HasEffect(Buffs.Dualcast))
                {
                    if (useFire) return Verfire;
                    if (useStone) return Verstone;
                }
                //END_RDM_VERFIREVERSTONE

                //RDM_VERTHUNDERVERAERO
                if (IsEnabled(CustomComboPreset.RDM_VerthunderVeraero) && actionID is Jolt or Jolt2)
                {
                    if (useThunder) return OriginalHook(Verthunder);
                    if (useAero) return OriginalHook(Veraero);
                }
                //END_RDM_VERTHUNDERVERAERO

                //RDM_VERTHUNDERIIVVERAEROII
                if (IsEnabled(CustomComboPreset.RDM_VerthunderIIVeraeroII) && actionID is Scatter or Impact)
                {
                    if (useThunder2) return Verthunder2;
                    if (useAero2) return Veraero2;
                }
                //END_RDM_VERTHUNDERIIVVERAEROII


                //NO_CONDITIONS_MET
                if (level < Levels.Jolt && actionID is Jolt or Jolt2) { return Riposte; }
                return actionID;
            }
        }

        internal class RDM_LucidDreaming : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_LucidDreaming;

            internal static bool showLucid = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Jolt or Jolt2 or Veraero or Veraero2 or Veraero3 or Verthunder or Verthunder2 or Verthunder3 or Scatter or Impact)
                {
                    var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.RDM_LucidDreaming_Threshold);

                    if (level >= All.Levels.LucidDreaming && LocalPlayer.CurrentMp <= lucidThreshold) // Check to show Lucid Dreaming
                    {
                        showLucid = true;
                    }

                    if (showLucid && CanSpellWeave(actionID) && HasCondition(ConditionFlag.InCombat) && IsOffCooldown(All.LucidDreaming) && !HasEffect(Buffs.Dualcast)
                        && lastComboMove != EnchantedRiposte && lastComboMove != EnchantedZwerchhau
                        && lastComboMove != EnchantedRedoublement && lastComboMove != Verflare
                        && lastComboMove != Verholy && lastComboMove != Scorch) // Change abilities to Lucid Dreaming for entire weave window
                    {
                        return All.LucidDreaming;
                    }
                    showLucid = false;
                }
                return actionID;
            }
        }

        // RDM_Verraise
        // Swiftcast combos to Verraise when:
        //  -Swiftcast is on cooldown.
        //  -Swiftcast is available, but we we have Dualcast (Dualcasting verraise)
        // Using this variation other than the alternatefeature style, as verrise is level 63
        // and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
        internal class RDM_Verraise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Verraise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast && level >= Levels.Verraise)
                {
                    if (GetCooldown(All.Swiftcast).CooldownRemaining > 0 ||   // Condition 1: Swiftcast is on cooldown
                        HasEffect(Buffs.Dualcast))                        // Condition 2: Swiftcast is available, but we have DualCast)
                        return Verraise;
                }

                // Else we just exit normally and return SwiftCast
                return actionID;
            }
        }

        internal class RDM_CorpsDisplacement : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_CorpsDisplacement;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var distance = GetTargetDistance();

                if (actionID is Displacement && level >= Levels.Displacement && HasTarget() && distance >= 5) { return Corpsacorps; }
                return actionID;
            }
        }

    }
}