using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class RDM
    {
        static bool HasEffect(ushort id) => CustomComboFunctions.HasEffect(id);
        static float GetBuffRemainingTime(ushort effectid) => CustomComboFunctions.GetBuffRemainingTime(effectid);
        static bool LevelChecked(uint id) => CustomComboFunctions.LevelChecked(id);
        static float GetActionCastTime(uint actionID) => CustomComboFunctions.GetActionCastTime(actionID);
        static uint GetRemainingCharges(uint actionID) => CustomComboFunctions.GetRemainingCharges(actionID);
        static float GetCooldownRemainingTime(uint actionID) => CustomComboFunctions.GetCooldownRemainingTime(actionID);
        static bool ActionReady(uint id) => CustomComboFunctions.ActionReady(id);
        static bool CanSpellWeave(uint id) => CustomComboFunctions.CanSpellWeave(id);
        static bool HasCharges(uint id) => CustomComboFunctions.HasCharges(id);

        internal class ManaBalancer : PvE.RDM
        {
            internal bool useFire;
            internal bool useStone;
            internal bool useThunder;
            internal bool useAero;
            internal bool useThunder2;
            internal bool useAero2;

            internal void CheckBalance()
            {
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
                int blackmana = Gauge.BlackMana;
                int whitemana = Gauge.WhiteMana;
                //Reset outputs
                useFire = false;
                useStone = false;
                useThunder = false;
                useAero = false;
                useThunder2 = false;
                useAero2 = false;

                //ST
                if (LevelChecked(Verthunder)
                    && (HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Acceleration)))
                {
                    if (blackmana <= whitemana || HasEffect(Buffs.VerstoneReady)) useThunder = true;
                    if (whitemana <= blackmana || HasEffect(Buffs.VerfireReady)) useAero = true;
                    if (!LevelChecked(Veraero)) useThunder = true;
                }
                if (!HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    //Checking the time remaining instead of just the effect, to stop last second bad casts
                    bool VerFireReady = GetBuffRemainingTime(Buffs.VerfireReady) >= GetActionCastTime(Verfire);
                    bool VerStoneReady = GetBuffRemainingTime(Buffs.VerstoneReady) >= GetActionCastTime(Verstone);

                    //Prioritize mana balance
                    if (blackmana <= whitemana && VerFireReady) useFire = true;
                    if (whitemana <= blackmana && VerStoneReady) useStone = true;
                    //Else use the action if we can
                    if (!useFire && !useStone && VerFireReady) useFire = true;
                    if (!useFire && !useStone && VerStoneReady) useStone = true;
                }

                //AoE
                if (LevelChecked(Verthunder2)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    if (blackmana <= whitemana || !LevelChecked(Veraero2)) useThunder2 = true;
                    else useAero2 = true;
                }
                //END_SYSTEM_MANA_BALANCING_MACHINE
            }
        }

        internal class MeleeFinisher : PvE.RDM
        { 
            internal static bool CanUse(in uint lastComboMove, out uint actionID)
            {
                int blackmana = Gauge.BlackMana;
                int whitemana = Gauge.WhiteMana;

                if (Gauge.ManaStacks >= 3)
                {
                    if (blackmana >= whitemana && LevelChecked(Verholy))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && !HasEffect(Buffs.VerfireReady)
                            && (HasEffect(Buffs.VerstoneReady) && GetBuffRemainingTime(Buffs.VerstoneReady) >= 10)
                            && (blackmana - whitemana <= 18))
                        {
                            actionID = Verflare;
                            return true;
                        }
                        actionID = Verholy;
                        return true;
                    }
                    else if (LevelChecked(Verflare))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && (HasEffect(Buffs.VerfireReady) && GetBuffRemainingTime(Buffs.VerfireReady) >= 10)
                            && !HasEffect(Buffs.VerstoneReady)
                            && LevelChecked(Verholy)
                            && (whitemana - blackmana <= 18))
                        {
                            actionID = Verholy;
                            return true;
                        }
                        actionID = Verflare;
                        return true;
                    }
                }
                if ((lastComboMove is Verflare or Verholy)
                    && LevelChecked(Scorch))
                {
                    actionID = Scorch;
                    return true;
                }

                if (lastComboMove is Scorch
                    && LevelChecked(Resolution))
                {
                    actionID = Resolution;
                    return true;
                }

                actionID = 0;
                return false;
            }
        }

        internal class OGCDHelper : PvE.RDM
        {
            internal static bool CanUse(in uint actionID, in bool SingleTarget, out uint newActionID)
            {
                var distance = CustomComboFunctions.GetTargetDistance();
                
                uint placeOGCD = 0;

                bool fleche = SingleTarget ? Config.RDM_ST_oGCD_Fleche : Config.RDM_AoE_oGCD_Fleche;
                bool contra = SingleTarget ? Config.RDM_ST_oGCD_ContraSixte : Config.RDM_AoE_oGCD_ContraSixte;
                bool engagement = SingleTarget ? Config.RDM_ST_oGCD_Engagement : Config.RDM_AoE_oGCD_Engagement;
                int engagementPool = (SingleTarget && Config.RDM_ST_oGCD_Engagement_Pooling) || (!SingleTarget && Config.RDM_AoE_oGCD_Engagement_Pooling) ? 1 : 0;

                bool corpacorps = SingleTarget ? Config.RDM_ST_oGCD_CorpACorps : Config.RDM_AoE_oGCD_CorpACorps;
                int corpsacorpsPool = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) ? 1 : 0;
                int corpacorpsRange = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) ? 3 : 25;


                //Grabs an oGCD to return based on radio options
                if (engagement
                    && (GetRemainingCharges(Engagement) > engagementPool
                        || (GetRemainingCharges(Engagement) == 1 && GetCooldownRemainingTime(Engagement) < 3))
                    && LevelChecked(Engagement)
                    && distance <= 3)
                    placeOGCD = Engagement;
                if (corpacorps
                    && (GetRemainingCharges(Corpsacorps) > corpsacorpsPool
                        || (GetRemainingCharges(Corpsacorps) == 1 && GetCooldownRemainingTime(Corpsacorps) < 3))
                    && ((GetRemainingCharges(Corpsacorps) >= GetRemainingCharges(Engagement)) || !LevelChecked(Engagement)) // Try to alternate between Corps-a-corps and Engagement
                    && LevelChecked(Corpsacorps)
                    && distance <= corpacorpsRange)
                    placeOGCD = Corpsacorps;
                if (contra
                    && ActionReady(ContreSixte))
                    placeOGCD = ContreSixte;
                if (fleche && ActionReady(Fleche))
                    placeOGCD = Fleche;

                if (CanSpellWeave(actionID) && placeOGCD != 0)
                {
                    newActionID = placeOGCD;
                    return true;
                }

                if (actionID is Fleche && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
                {
                    placeOGCD = Fleche;
                    if (contra
                        && LevelChecked(ContreSixte)
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(ContreSixte))
                        placeOGCD = ContreSixte;
                    if (corpacorps
                        && LevelChecked(Corpsacorps)
                        && !HasCharges(Corpsacorps)
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Corpsacorps))
                        placeOGCD = Corpsacorps;
                    if (engagement
                        && LevelChecked(Engagement)
                        && GetCooldownRemainingTime(Engagement) == 0
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Engagement))
                        placeOGCD = Engagement;
                }
                if (actionID is Fleche)
                {
                    newActionID = placeOGCD;
                    return true;
                }

                newActionID = 0;
                return false;
            }
        }

        internal class RDMLucid : PvE.RDM
        {
            internal static bool SafetoUse(in uint lastComboMove)
            {
                return
                    !CustomComboFunctions.HasEffect(Buffs.Dualcast)
                    && lastComboMove != EnchantedRiposte
                    && lastComboMove != EnchantedZwerchhau
                    && lastComboMove != EnchantedRedoublement
                    && lastComboMove != Verflare
                    && lastComboMove != Verholy
                    && lastComboMove != Scorch; // Change abilities to Lucid Dreaming for entire weave window
            }
        }
    }
}