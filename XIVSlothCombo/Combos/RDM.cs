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
            if (IsEnabled(CustomComboPreset.RDM_Balance_Opener) && level >= 90 && actionID is RDM.Jolt or RDM.Jolt2)
            {
                bool inCombat = HasCondition(ConditionFlag.InCombat);

                // Check to start opener
                if (openerStarted && lastComboMove is RDM.Verthunder3 && HasEffect(RDM.Buffs.Dualcast)) { inOpener = true; openerStarted = false; readyOpener = false; }
                if ((readyOpener || openerStarted) && !inOpener && LocalPlayer.CastActionId == RDM.Verthunder3) { openerStarted = true; return RDM.Veraero3; } else { openerStarted = false; }

                // Reset check for opener
                if ((IsEnabled(CustomComboPreset.RDM_Opener_Any_Mana) || (gauge.BlackMana == 0 && gauge.WhiteMana == 0)) 
                    && IsOffCooldown(RDM.Embolden) && IsOffCooldown(RDM.Manafication) && IsOffCooldown(All.Swiftcast)
                    && GetCooldown(RDM.Acceleration).RemainingCharges == 2 && GetCooldown(RDM.Corpsacorps).RemainingCharges == 2 && GetCooldown(RDM.Engagement).RemainingCharges == 2
                    && IsOffCooldown(RDM.Fleche) && IsOffCooldown(RDM.ContreSixte)
                    && EnemyHealthPercentage() == 100 && !inOpener && !openerStarted)
                {
                    readyOpener = true;
                    inOpener = false;
                    step = 0;
                    return RDM.Verthunder3;
                }
                else
                { readyOpener = false; }

                // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                if ((step == 0 && lastComboMove is RDM.Verthunder3 && !HasEffect(RDM.Buffs.Dualcast))
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
                        if (lastComboMove == RDM.Veraero3) step++;
                        else return RDM.Veraero3;
                    }

                    if (step == 1)
                    {
                        if (IsOnCooldown(All.Swiftcast)) step++;
                        else return All.Swiftcast;
                    }

                    if (step == 2)
                    {
                        if (GetRemainingCharges(RDM.Acceleration) < 2) step++;
                        else return RDM.Acceleration;
                    }

                    if (step == 3)
                    {
                        if (lastComboMove == RDM.Verthunder3 && !HasEffect(RDM.Buffs.Acceleration)) step++;
                        else return RDM.Verthunder3;
                    }

                    if (step == 4)
                    {
                        if (lastComboMove == RDM.Verthunder3 && !HasEffect(All.Buffs.Swiftcast)) step++;
                        else return RDM.Verthunder3;
                    }

                    if (step == 5)
                    {
                        if (IsOnCooldown(RDM.Embolden)) step++;
                        else return RDM.Embolden;
                    }

                    if (step == 6)
                    {
                        if (IsOnCooldown(RDM.Manafication)) step++;
                        else return RDM.Manafication;
                    }

                    if (step == 7)
                    {
                        if (lastComboMove == RDM.Riposte) step++;
                        else return RDM.EnchantedRiposte;
                    }

                    if (step == 8)
                    {
                        if (IsOnCooldown(RDM.Fleche)) step++;
                        else return RDM.Fleche;
                    }

                    if (step == 9)
                    {
                        if (lastComboMove == RDM.Zwerchhau) step++;
                        else return RDM.EnchantedZwerchhau;
                    }

                    if (step == 10)
                    {
                        if (IsOnCooldown(RDM.ContreSixte)) step++;
                        else return RDM.ContreSixte;
                    }

                    if (step == 11)
                    {
                        if (lastComboMove == RDM.Redoublement || gauge.ManaStacks == 3) step++;
                        else return RDM.EnchantedRedoublement;
                    }

                    if (step == 12)
                    {
                        if (GetRemainingCharges(RDM.Corpsacorps) < 2) step++;
                        else return RDM.Corpsacorps;
                    }

                    if (step == 13)
                    {
                        if (GetRemainingCharges(RDM.Engagement) < 2) step++;
                        else return RDM.Engagement;
                    }

                    if (step == 14)
                    {
                        if (lastComboMove == RDM.Verholy) step++;
                        else return RDM.Verholy;
                    }

                    if (step == 15)
                    {
                        if (GetRemainingCharges(RDM.Corpsacorps) < 1) step++;
                        else return RDM.Corpsacorps;
                    }

                    if (step == 16)
                    {
                        if (GetRemainingCharges(RDM.Engagement) < 1) step++;
                        else return RDM.Engagement;
                    }

                    if (step == 17)
                    {
                        if (lastComboMove == RDM.Scorch) step++;
                        else return RDM.Scorch;
                    }

                    if (step == 18)
                    {
                        if (lastComboMove == RDM.Resolution) step++;
                        else return RDM.Resolution;
                    }

                    inOpener = false;
                }
            }
            //END_RDM_BALANCE_OPENER

            //RDM_ST_MANAFICATIONEMBOLDEN
            if (IsEnabled(CustomComboPreset.RDM_ST_ManaficationEmbolden) && level >= RDM.Levels.Embolden && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration) 
                && (GetTargetDistance() <= 3 || (IsEnabled(CustomComboPreset.RDM_ST_CorpsGapClose) && GetCooldown(RDM.Corpsacorps).RemainingCharges >= 1)))
            {
                var radioButton = Service.Configuration.GetCustomIntValue(RDM.Config.RDM_ST_MeleeCombo_OnAction);

                if ((radioButton == 1 && actionID is RDM.Riposte or RDM.EnchantedRiposte)
                    || (radioButton == 2 && actionID is RDM.Jolt or RDM.Jolt2)
                    || (radioButton == 3 && actionID is RDM.Riposte or RDM.EnchantedRiposte or RDM.Jolt or RDM.Jolt2))
                {
                    //Situation 1: Manafication first
                    if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                    && System.Math.Max(black, white) <= 50 && System.Math.Max(black, white) >= 42 && System.Math.Min(black, white) >= 31
                    && IsOffCooldown(RDM.Manafication) && gauge.ManaStacks == 0
                    && (IsOffCooldown(RDM.Embolden) || GetCooldown(RDM.Embolden).CooldownRemaining <= 3))
                    {
                        return RDM.Manafication;
                    }
                    if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                        && lastComboMove is RDM.Zwerchhau && level >= RDM.Levels.Redoublement
                        && System.Math.Max(black, white) >= 55 && System.Math.Min(black, white) >= 46
                        && GetCooldown(RDM.Manafication).CooldownRemaining >= 100
                        && IsOffCooldown(RDM.Embolden))
                    {
                        return RDM.Embolden;
                    }

                    //Situation 2: Embolden first
                    if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                        && lastComboMove is RDM.Zwerchhau && level >= RDM.Levels.Redoublement
                        && System.Math.Max(black, white) <= 57 && System.Math.Min(black, white) <= 46
                        && (GetCooldown(RDM.Manafication).CooldownRemaining <= 7 || IsOffCooldown(RDM.Manafication))
                        && IsOffCooldown(RDM.Embolden))
                    {
                        return RDM.Embolden;
                    }
                    if (IsEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) && level >= 90
                        && System.Math.Max(black, white) <= 50
                        && IsOffCooldown(RDM.Manafication)
                        && (gauge.ManaStacks == 3 || lastComboMove is RDM.Resolution)
                        && GetCooldown(RDM.Embolden).CooldownRemaining >= 105)
                    {
                        return RDM.Manafication;
                    }

                    //Situation 3: Just use them together
                    if ((IsNotEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) || level < 90) && level >= RDM.Levels.Embolden
                        && System.Math.Max(black, white) <= 50
                        && (IsOffCooldown(RDM.Manafication) || level < RDM.Levels.Manafication) && IsOffCooldown(RDM.Embolden))
                    {
                        return RDM.Embolden;
                    }
                    if ((IsNotEnabled(CustomComboPreset.RDM_ST_DoubleMeleeCombo) || level < 90) && level >= RDM.Levels.Manafication
                        && System.Math.Max(black, white) <= 50
                        && IsOffCooldown(RDM.Manafication) && gauge.ManaStacks == 0
                        && GetCooldown(RDM.Embolden).CooldownRemaining >= 110)
                    {
                        return RDM.Manafication;
                    }

                    //Situation 4: Level 58 or 59
                    if (level < RDM.Levels.Manafication && level >= RDM.Levels.Embolden
                        && System.Math.Min(black, white) >= 50 && IsOffCooldown(RDM.Embolden))
                    {
                        return RDM.Embolden;
                    }
                }
            }
            //END_RDM_ST_MANAFICATIONEMBOLDEN

            //RDM_AOE_MANAFICATIONEMBOLDEN
            if (IsEnabled(CustomComboPreset.RDM_AoE_ManaficationEmbolden) && level >= RDM.Levels.Embolden && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration)
                && GetTargetDistance() < 8 && actionID is RDM.Scatter or RDM.Impact)
            {
                if (level >= RDM.Levels.Manafication
                && System.Math.Max(black, white) <= 50 && System.Math.Min(black, white) >= 10
                && (IsOffCooldown(RDM.Manafication) || level < RDM.Levels.Manafication) && IsOffCooldown(RDM.Embolden))
                {
                    return RDM.Embolden;
                }
                if (level >= RDM.Levels.Manafication
                    && System.Math.Max(black, white) <= 50 && System.Math.Min(black, white) >= 10
                    && IsOffCooldown(RDM.Manafication) && gauge.ManaStacks == 0
                    && GetCooldown(RDM.Embolden).CooldownRemaining >= 110)
                {
                    return RDM.Manafication;
                }
                if (level < RDM.Levels.Manafication && level >= RDM.Levels.Embolden
                    && System.Math.Min(black, white) >= 20 && IsOffCooldown(RDM.Embolden))
                {
                    return RDM.Embolden;
                }
            }
            //END_RDM_AOE_MANAFICATIONEMBOLDEN

            //RDM_OGCD
            if (IsEnabled(CustomComboPreset.RDM_OGCD) && level >= RDM.Levels.Corpsacorps)
            {
                var radioButton = Service.Configuration.GetCustomIntValue(RDM.Config.RDM_OGCD_OnAction);
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

                if (actionID is RDM.Jolt or RDM.Jolt2 or RDM.Scatter or RDM.Impact or RDM.Fleche)
                {
                    if (IsEnabled(CustomComboPreset.RDM_Engagement) && GetCooldown(RDM.Engagement).RemainingCharges > 0
                        && level >= RDM.Levels.Engagement && distance <= 3) placeOGCD = RDM.Engagement;
                    if (IsEnabled(CustomComboPreset.RDM_Corpsacorps) && GetCooldown(RDM.Corpsacorps).RemainingCharges > corpsacorpsPool
                        && ((GetCooldown(RDM.Corpsacorps).RemainingCharges >= GetCooldown(RDM.Engagement).RemainingCharges) || level < RDM.Levels.Engagement) // Try to alternate between Corps-a-corps and Engagement
                        && level >= RDM.Levels.Corpsacorps && distance <= corpacorpsRange) placeOGCD = RDM.Corpsacorps;
                    if (IsEnabled(CustomComboPreset.RDM_ContraSixte) && IsOffCooldown(RDM.ContreSixte) && level >= RDM.Levels.ContreSixte) placeOGCD = RDM.ContreSixte;
                    if ((radioButton == 1 || IsEnabled(CustomComboPreset.RDM_Fleche)) && IsOffCooldown(RDM.Fleche) && level >= RDM.Levels.Fleche) placeOGCD = RDM.Fleche;

                    if ((actionID is RDM.Jolt or RDM.Jolt2) && (radioButton is 2 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                    if ((actionID is RDM.Scatter or RDM.Impact) && (radioButton is 3 or 4) && CanSpellWeave(actionID) && placeOGCD != 0) return placeOGCD;
                    if (actionID is RDM.Fleche && radioButton == 1 && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
                    {
                        placeOGCD = RDM.Fleche;
                        if (IsEnabled(CustomComboPreset.RDM_ContraSixte) && level >= RDM.Levels.ContreSixte
                            && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(RDM.ContreSixte).CooldownRemaining) placeOGCD = RDM.ContreSixte;
                        if (IsEnabled(CustomComboPreset.RDM_Corpsacorps) && level >= RDM.Levels.Corpsacorps
                            && ((IsNotEnabled(CustomComboPreset.RDM_ST_PoolCorps) && GetCooldown(RDM.Corpsacorps).RemainingCharges >= 0) || GetCooldown(RDM.Corpsacorps).RemainingCharges >= 2)
                            && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(RDM.Corpsacorps).ChargeCooldownRemaining
                            && distance <= corpacorpsRange) placeOGCD = RDM.Corpsacorps;
                        if (placeOGCD == RDM.Corpsacorps)
                        {
                            if (IsEnabled(CustomComboPreset.RDM_Engagement) && level >= RDM.Levels.Engagement
                                && GetCooldown(placeOGCD).ChargeCooldownRemaining > GetCooldown(RDM.Engagement).ChargeCooldownRemaining
                                && distance <= 3) placeOGCD = RDM.Engagement;
                        } else if (IsEnabled(CustomComboPreset.RDM_Engagement)
                            && GetCooldown(placeOGCD).CooldownRemaining > GetCooldown(RDM.Engagement).ChargeCooldownRemaining
                            && distance <= 3) placeOGCD = RDM.Engagement;
                    }
                    if (actionID is RDM.Fleche && radioButton == 1) return placeOGCD;
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

            if (level >= RDM.Levels.Verthunder && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Acceleration)))
            {
                if (black <= white || HasEffect(RDM.Buffs.VerstoneReady)) useThunder = true;
                if (white <= black || HasEffect(RDM.Buffs.VerfireReady)) useAero = true;
                if (level < RDM.Levels.Veraero) useThunder = true;
            }
            if (!HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration))
            {
                if (black <= white && HasEffect(RDM.Buffs.VerfireReady)) useFire = true;
                if (white <= black && HasEffect(RDM.Buffs.VerstoneReady)) useStone = true;
                if (!useFire && !useStone && HasEffect(RDM.Buffs.VerfireReady)) useFire = true;
                if (!useFire && !useStone && HasEffect(RDM.Buffs.VerstoneReady)) useStone = true;
            }
            if (level >= RDM.Levels.Verthunder2 && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration))
            {
                if (black <= white || level < RDM.Levels.Veraero2) useThunder2 = true;
                else useAero2 = true;
            }
            //END_SYSTEM_MANA_BALANCING_MACHINE

            //RDM_MELEEFINISHER
            if (IsEnabled(CustomComboPreset.RDM_MeleeFinisher))
            {
                var radioButton = Service.Configuration.GetCustomIntValue(RDM.Config.RDM_MeleeFinisher_OnAction);

                if ((radioButton == 1 && actionID is RDM.Riposte or RDM.EnchantedRiposte or RDM.Moulinet or RDM.EnchantedMoulinet)
                    || (radioButton == 2 && actionID is RDM.Jolt or RDM.Jolt2 or RDM.Scatter or RDM.Impact)
                    || (radioButton == 3 && actionID is RDM.Riposte or RDM.EnchantedRiposte or RDM.Moulinet or RDM.EnchantedMoulinet or RDM.Jolt or RDM.Jolt2 or RDM.Scatter or RDM.Impact))
                {
                    if (gauge.ManaStacks >= 3)
                    {
                        if (black >= white && level >= RDM.Levels.Verholy)
                        {
                            if (HasEffect(RDM.Buffs.VerstoneReady) && (!HasEffect(RDM.Buffs.VerfireReady) || HasEffect(RDM.Buffs.Embolden)) && (black - white <= 9))
                                return RDM.Verflare;

                            return RDM.Verholy;
                        }
                        else if (level >= RDM.Levels.Verflare)
                        {
                            if (!HasEffect(RDM.Buffs.VerstoneReady) && (HasEffect(RDM.Buffs.VerfireReady) || HasEffect(RDM.Buffs.Embolden)) && level >= RDM.Levels.Verholy && (white - black <= 9))
                                return RDM.Verholy;

                            return RDM.Verflare;
                        }
                    }
                    if ((lastComboMove is RDM.Verflare or RDM.Verholy) && level >= RDM.Levels.Scorch)
                        return RDM.Scorch;

                    if (lastComboMove is RDM.Scorch && level >= RDM.Levels.Resolution)
                        return RDM.Resolution;
                }
            }
            //END_RDM_MELEEFINISHER

            //RDM_ST_MELEECOMBO
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo) && LocalPlayer.IsCasting == false)
            {
                var radioButton = Service.Configuration.GetCustomIntValue(RDM.Config.RDM_ST_MeleeCombo_OnAction);
                var distance = GetTargetDistance();

                if ((radioButton == 1 && actionID is RDM.Riposte or RDM.EnchantedRiposte)
                    || (radioButton == 2 && actionID is RDM.Jolt or RDM.Jolt2)
                    || (radioButton == 3 && actionID is RDM.Riposte or RDM.EnchantedRiposte or RDM.Jolt or RDM.Jolt2))
                {
                    if ((lastComboMove is RDM.Riposte or RDM.EnchantedRiposte) && level >= RDM.Levels.Zwerchhau)
                        return OriginalHook(RDM.Zwerchhau);

                    if (lastComboMove is RDM.Zwerchhau && level >= RDM.Levels.Redoublement)
                        return OriginalHook(RDM.Redoublement);

                    if (((System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 50 && level >= RDM.Levels.Redoublement) 
                            || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 35 && level < RDM.Levels.Redoublement)
                            || (System.Math.Min(gauge.WhiteMana, gauge.BlackMana) >= 20 && level < RDM.Levels.Zwerchhau))
                        && (!HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration))) //Not sure if Swift and Accel are necessary, but better to clear I think.
                    {
                        if (IsEnabled(CustomComboPreset.RDM_ST_CorpsGapClose) && level >= RDM.Levels.Corpsacorps && GetCooldown(RDM.Corpsacorps).RemainingCharges >= 1 && distance > 3) return RDM.Corpsacorps;
                        if (distance <= 3) return OriginalHook(RDM.Riposte);
                    }
                }
            }
            //END_RDM_ST_MELEECOMBO

            //RDM_AOE_MELEECOMBO
            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo) && level >= RDM.Levels.Moulinet && actionID is RDM.Scatter or RDM.Impact && LocalPlayer.IsCasting == false
                && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(RDM.Buffs.Acceleration)
                && (System.Math.Min(gauge.BlackMana, gauge.WhiteMana) + (gauge.ManaStacks * 20) >= 60 || (level < RDM.Levels.Manafication && System.Math.Min(gauge.BlackMana, gauge.WhiteMana) >= 20))
                && ((GetTargetDistance() <= 7 && gauge.ManaStacks == 0) || gauge.ManaStacks > 0))
                return OriginalHook(RDM.EnchantedMoulinet);
            //END_RDM_AOE_MELEECOMBO

            //RDM_ST_ACCELERATION
            if (IsEnabled(CustomComboPreset.RDM_ST_Acceleration) && actionID is RDM.Jolt or RDM.Jolt2 && HasCondition(ConditionFlag.InCombat) && LocalPlayer.IsCasting == false
                && !HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.Acceleration) && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast))
            {
                if (level >= RDM.Levels.Acceleration && GetCooldown(RDM.Acceleration).RemainingCharges > 0 && GetCooldown(RDM.Acceleration).ChargeCooldownRemaining < 54)
                    return RDM.Acceleration;
                if (IsEnabled(CustomComboPreset.RDM_ST_AccelSwiftCast) && level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) && GetCooldown(RDM.Acceleration).RemainingCharges == 0)
                    return All.Swiftcast;
            }
            //END_RDM_ST_ACCELERATION

            //RDM_AoE_ACCELERATION
            if (IsEnabled(CustomComboPreset.RDM_AoE_Acceleration) && actionID is RDM.Scatter or RDM.Impact && LocalPlayer.IsCasting == false
                && !HasEffect(RDM.Buffs.Acceleration) && !HasEffect(RDM.Buffs.Dualcast) && !HasEffect(All.Buffs.Swiftcast))
            {
                if (level >= RDM.Levels.Acceleration && GetCooldown(RDM.Acceleration).RemainingCharges > 0 && GetCooldown(RDM.Acceleration).ChargeCooldownRemaining < 54)
                    return RDM.Acceleration;
                if (IsEnabled(CustomComboPreset.RDM_AoE_AccelSwiftCast) && level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) && GetCooldown(RDM.Acceleration).RemainingCharges == 0)
                    return All.Swiftcast;
            }
            //END_RDM_AoE_ACCELERATION

            //RDM_VERFIREVERSTONE
            if (IsEnabled(CustomComboPreset.RDM_VerfireVerstone) && actionID is RDM.Jolt or RDM.Jolt2
                && !HasEffect(RDM.Buffs.Acceleration) && !HasEffect(RDM.Buffs.Dualcast))
            {
                if (useFire) return RDM.Verfire;
                if (useStone) return RDM.Verstone;
            }
            //END_RDM_VERFIREVERSTONE

            //RDM_VERTHUNDERVERAERO
            if (IsEnabled(CustomComboPreset.RDM_VerthunderVeraero) && actionID is RDM.Jolt or RDM.Jolt2)
            {
                if (useThunder) return OriginalHook(RDM.Verthunder);
                if (useAero) return OriginalHook(RDM.Veraero);
            }
            //END_RDM_VERTHUNDERVERAERO

            //RDM_VERTHUNDERIIVVERAEROII
            if (IsEnabled(CustomComboPreset.RDM_VerthunderIIVeraeroII) && actionID is RDM.Scatter or RDM.Impact)
            {
                if (useThunder2) return RDM.Verthunder2;
                if (useAero2) return RDM.Veraero2;
            }
            //END_RDM_VERTHUNDERIIVVERAEROII

            //NO_CONDITIONS_MET
            if (level < RDM.Levels.Jolt && actionID is RDM.Jolt or RDM.Jolt2) { return RDM.Riposte; }
            return actionID;
        }
    }

    internal class RDM_LucidDreaming : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_LucidDreaming;

        internal static bool showLucid = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RDM.Verthunder or RDM.Veraero or RDM.Scatter or RDM.Verthunder3 or RDM.Veraero3 or RDM.Verthunder2 or RDM.Veraero2 or RDM.Impact or RDM.Jolt or RDM.Jolt2)
            {
                var lucidThreshold = Service.Configuration.GetCustomIntValue(RDM.Config.RDM_LucidDreaming_Threshold);

                if (level >= All.Levels.LucidDreaming && LocalPlayer.CurrentMp <= lucidThreshold) // Check to show Lucid Dreaming
                {
                    showLucid = true;
                }

                if (showLucid && CanSpellWeave(actionID) && HasCondition(ConditionFlag.InCombat) && IsOffCooldown(All.LucidDreaming) && !HasEffect(RDM.Buffs.Dualcast)
                    && lastComboMove != RDM.EnchantedRiposte && lastComboMove != RDM.EnchantedZwerchhau
                    && lastComboMove != RDM.EnchantedRedoublement && lastComboMove != RDM.Verflare
                    && lastComboMove != RDM.Verholy && lastComboMove != RDM.Scorch) // Change abilities to Lucid Dreaming for entire weave window
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
            if (actionID is All.Swiftcast && level >= RDM.Levels.Verraise)
            {
                if (GetCooldown(All.Swiftcast).CooldownRemaining > 0 ||   // Condition 1: Swiftcast is on cooldown
                    HasEffect(RDM.Buffs.Dualcast))                        // Condition 2: Swiftcast is available, but we have DualCast)
                    return RDM.Verraise;
            }

            // Else we just exit normally and return SwiftCast
            return actionID;
        }
    }
}