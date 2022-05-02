using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint

            // Heals and Shields
            Diagnosis = 24284,
            Prognosis = 24286,
            Physis = 24288,
            Druochole = 24296,
            Kerachole = 24298,
            Ixochole = 24299,
            Pepsis = 24301,
            Physis2 = 24302,
            Taurochole = 24303,
            Haima = 24305,
            Panhaima = 24311,
            Holos = 24310,
            EukrasianDiagnosis = 24291,
            EukrasianPrognosis = 24292,

            // DPS
            Dosis1 = 24283,
            Dosis2 = 24306,
            Dosis3 = 24312,
            EukrasianDosis1 = 24293,
            EukrasianDosis2 = 24308,
            EukrasianDosis3 = 24314,
            Phlegma = 24289,
            Phlegma2 = 24307,
            Phlegma3 = 24313,
            Dyskrasia = 24297,
            Dyskrasia2 = 24315,
            Toxikon = 24304,
            Pneuma = 24318,

            // Buffs
            Soteria = 24294,
            Zoe = 24300,
            Krasis = 24317,

            // Other
            Kardia = 24285,
            Eukrasia = 24290,
            Rhizomata = 24309,

            // Role
            Swiftcast = 756,
            Egeiro = 24287,
            LucidDreaming = 7562;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                EukrasianDiagnosis = 2607,
                Kardion = 2872,
                EukrasianPrognosis = 2609;
        }

        public static class Debuffs
        {
            public const ushort
            EukrasianDosis1 = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616;
        }

        public static class Levels //Per 6.1 Patch https://na.finalfantasyxiv.com/jobguide/sage/
        {
            public const byte
                Dosis = 1,
                Diagnosis = 2,
                Kardia = 4,
                Prognosis = 10,
                Egeiro = 12,
                Physis = 20,
                Phlegma = 26,
                Eukrasia = 30, //includes Dosis, Diagnosis, & Prognosis
                Soteria = 35,
                Icarus = 40,
                Druochole = 45,
                Dyskrasia = 46,
                Kerachole = 50,
                Ixochole = 52,
                Zoe = 56,
                Pepsis = 58,
                Physis2 = 60,
                Taurochole = 62,
                Toxikon = 66,
                Haima = 70,
                Dosis2 = 72, //includes Eukrasian Dosis 2 
                Phlegma2 = 72,
                Rhizomata = 74,
                Holos = 76,
                Panhaima = 80,
                Dosis3 = 82, //includes Eukrasian Dosis 3
                Dyskrasia2 = 82,
                Phlegma3 = 82,
                Toxikon2 = 82,
                Krasis = 86,
                Pneuma = 90;
        }

        public static class Config
        {
            public const string
                //GUI Customization Storage Names
                SGE_ST_Dosis_EDosisHPPer = "SGE_ST_Dosis_EDosisHPPer",
                SGE_ST_Dosis_Lucid = "SGE_ST_Dosis_Lucid",
                SGE_ST_Dosis_Toxikon = "SGE_ST_Dosis_Toxikon",
                SGE_ST_Heal_Zoe = "SGE_ST_Heal_Zoe",
                SGE_ST_Heal_Haima = "SGE_ST_Heal_Haima",
                SGE_ST_Heal_Krasis = "SGE_ST_Heal_Krasis",
                SGE_ST_Heal_Pepsis = "SGE_ST_Heal_Pepsis",
                SGE_ST_Heal_Soteria = "SGE_ST_Heal_Soteria",
                SGE_ST_Heal_Diagnosis = "SGE_ST_Heal_Diagnosis",
                SGE_ST_Heal_Druochole = "SGE_ST_Heal_Druochole",
                SGE_ST_Heal_Taurochole = "SGE_ST_Heal_Taurochole";
        }
    }


    //SageSoteriaKardia
    //Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
    internal class SageSoteriaKardiaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_KardiaFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Soteria && 
                (!HasEffect(SGE.Buffs.Kardia) || IsOnCooldown(SGE.Soteria)) 
               ) return SGE.Kardia;
            else return actionID;
        }
    }

    //SageRhizomata
    //Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of Addersgall stacks
    //(Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
    internal class SageRhizomataFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_RhizoFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Taurochole or SGE.Druochole or SGE.Ixochole or SGE.Kerachole &&
                level >= SGE.Levels.Rhizomata &&
                GetJobGauge<SGEGauge>().Addersgall == 0
               ) return SGE.Rhizomata;
            else return actionID;
        }
    }

    //SageDruoTauro
    //Druochole Upgrade to Taurochole (like a trait upgrade)
    //Replaces Druocole with Taurochole when Taurochole is available
    //(As of 6.0) Taurochole (single target massive insta heal w/ cooldown), Druochole (Single target insta heal)
    internal class SageDruoTauroFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_DruoTauroFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Druochole && level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole)) return SGE.Taurochole;
            else return actionID;
        }
    }

    //Sage AoE / Phlegma Replacement
    //Replaces Zero Charges/Stacks of Phlegma with Toxikon (if you can use it) or Dyskrasia 
    internal class SageAoEPhlegmaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_PhlegmaFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Phlegma or SGE.Phlegma2 or SGE.Phlegma3)
            {
                //Check for "out of Phlegma stacks" 
                if (GetCooldown(OriginalHook(SGE.Phlegma)).RemainingCharges == 0)
                {
                    //Toxikon Checks
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Toxikon) &&
                        level >= SGE.Levels.Toxikon &&
                        HasBattleTarget() &&
                        GetJobGauge<SGEGauge>().Addersting > 0
                       ) return OriginalHook(SGE.Toxikon);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Dyskrasia) && level >= SGE.Levels.Dyskrasia)
                        return OriginalHook(SGE.Dyskrasia);
                }
                //Sub-Sub Feature. Allows running around in a dungeon/field with nothing targetted, saving charges.
                //Will switch back to Phlegma/Toxikon when targetting something, as those two are target only skills
                if (IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Dyskrasia) && //Check for parent until GUI fixes for an active child feature with a disabled parent
                    IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Dyskrasia_NoTarget) &&
                    level >= SGE.Levels.Dyskrasia &&
                    CurrentTarget == null
                   ) return OriginalHook(SGE.Dyskrasia);
            }
            return actionID;
        }
    }

    //SageSTDosis
    //Single Target Dosis Combo
    //Currently Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
    //Lucid Dreaming, Target of Target optional
    internal class SageSTDosisFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_DosisFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Dosis1 or SGE.Dosis2 or SGE.Dosis3 && InCombat())
            {
                //Lucid Dreaming
                if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Lucid) &&
                    level >= All.Levels.LucidDreaming &&
                    IsOffCooldown(All.LucidDreaming) &&
                    LocalPlayer.CurrentMp <= GetOptionValue(SGE.Config.SGE_ST_Dosis_Lucid) &&
                    CanSpellWeave(actionID)
                   ) return All.LucidDreaming;

                //Eukrasian Dosis.
                //If we're too low level to use Eukrasia, we can stop here.
                if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosis) && (level >= SGE.Levels.Eukrasia) && CurrentTarget is not null)
                {
                    var OurTarget = CurrentTarget;
                    //Check if our Target is there and not an enemy
                    if ((CurrentTarget as BattleNpc)?.BattleNpcKind is not BattleNpcSubKind.Enemy)
                    {
                        //If ToT is enabled, Check if ToT is not null
                        if ((IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosisToT)) &&
                            (CurrentTarget.TargetObject is not null) &&
                            ((CurrentTarget.TargetObject as BattleNpc)?.BattleNpcKind is BattleNpcSubKind.Enemy))
                            //Set Ourtarget as the Target of Target
                            OurTarget = CurrentTarget.TargetObject;
                        //Our Target of Target wasn't hostile, our target isn't hostile, time to exit, nothing to check debuff on, fuck this shit we're out
                        else return actionID;
                    }

                    //Determine which Dosis debuff to check
                    var DosisDebuffID = level switch
                    {
                        //Using FindEffect b/c we have a custom Target variable
                        >= SGE.Levels.Dosis3 => FindEffect(SGE.Debuffs.EukrasianDosis3, OurTarget, LocalPlayer?.ObjectId),
                        >= SGE.Levels.Dosis2 => FindEffect(SGE.Debuffs.EukrasianDosis2, OurTarget, LocalPlayer?.ObjectId),
                        //Ekrasia Dosis unlocks with Eukrasia, checked at the start
                        _ => FindEffect(SGE.Debuffs.EukrasianDosis1, OurTarget, LocalPlayer?.ObjectId),
                    };

                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return OriginalHook(SGE.Dosis1); //OriginalHook will autoselect the correct Dosis for us

                    //Got our Debuff for our level, check for it and procede 
                    if ((DosisDebuffID is null) || (DosisDebuffID.RemainingTime <= 3))
                    {
                        //Advanced Options Enabled to procede with auto-Eukrasia
                        //Incompatible with ToT due to Enemy checks that are using CurrentTarget.
                        if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosisHPPer))
                        {
                            if (EnemyHealthPercentage() > GetOptionValue(SGE.Config.SGE_ST_Dosis_EDosisHPPer)) return SGE.Eukrasia;
                        }
                        else return SGE.Eukrasia;
                    }
                }

                //Toxikon
                if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Toxikon) &&
                    level >= SGE.Levels.Toxikon &&
                    HasBattleTarget() &&
                    ((GetOptionValue(SGE.Config.SGE_ST_Dosis_Toxikon) == 1 && this.IsMoving) || (GetOptionValue(SGE.Config.SGE_ST_Dosis_Toxikon) == 2)) &&
                    GetJobGauge<SGEGauge>().Addersting > 0 &&
                    CanSpellWeave(actionID)
                   ) return OriginalHook(SGE.Toxikon);
            }
            return actionID;
        }
    }

    //SageRaise
    //Swiftcast combos to Egeiro (Raise) while Swiftcast is on cooldown
    internal class SageRaiseFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_RaiseFeature;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast)) return SGE.Egeiro;
            else return actionID;
        }
    }

    internal class SageSingleTargetHealFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_HealFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Diagnosis )
            {
                if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && 
                    level >= SGE.Levels.Druochole &&
                    IsOffCooldown(SGE.Druochole) &&
                    GetJobGauge<SGEGauge>().Addersgall >= 1 && 
                    EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Druochole)
                   ) return SGE.Druochole;

                if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) &&
                    level >= SGE.Levels.Taurochole && 
                    IsOffCooldown(SGE.Taurochole) && 
                    GetJobGauge<SGEGauge>().Addersgall >= 1 &&
                    EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Taurochole)
                   ) return SGE.Taurochole;

                if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) &&
                    level >= SGE.Levels.Rhizomata &&
                    IsOffCooldown(SGE.Rhizomata) &&
                    GetJobGauge<SGEGauge>().Addersgall is 0
                   ) return SGE.Rhizomata;

                if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && 
                    level >= SGE.Levels.Kardia &&
                    FindEffect(SGE.Buffs.Kardia) is null &&
                    FindTargetEffect(SGE.Buffs.Kardion) is null
                   ) return SGE.Kardia;

                if (CurrentTarget?.ObjectKind is ObjectKind.Player)
                {
                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria) &&
                        level >= SGE.Levels.Soteria &&
                        IsOffCooldown(SGE.Soteria) && 
                        EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Soteria)
                       ) return SGE.Soteria;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe) &&
                        level >= SGE.Levels.Zoe &&
                        IsOffCooldown(SGE.Zoe) && 
                        EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Zoe)
                       ) return SGE.Zoe;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis) &&
                        level >= SGE.Levels.Krasis &&
                        IsOffCooldown(SGE.Krasis) && EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Krasis)
                       ) return SGE.Krasis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) &&
                        level >= SGE.Levels.Pepsis && 
                        IsOffCooldown(SGE.Pepsis) && EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Pepsis) &&
                        FindTargetEffect(SGE.Buffs.EukrasianDiagnosis) is not null
                       ) return SGE.Pepsis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima) &&
                        level >= SGE.Levels.Haima &&
                        IsOffCooldown(SGE.Haima) && EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Haima)
                       ) return SGE.Haima;
                }
                
                if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Diagnosis) &&
                    level >= SGE.Levels.Eukrasia &&
                    FindTargetEffect(SGE.Buffs.EukrasianDiagnosis) is null && 
                    EnemyHealthPercentage() <= GetOptionValue(SGE.Config.SGE_ST_Heal_Diagnosis))
                {
                    if (!HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.Eukrasia;
                    else return SGE.EukrasianDiagnosis;
                }
            }
            return actionID;
        }
    }

    internal class SageAoEHealFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_HealFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Prognosis)
            {
                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) &&
                    level >= SGE.Levels.Rhizomata &&
                    IsOffCooldown(SGE.Rhizomata) &&
                    GetJobGauge<SGEGauge>().Addersgall is 0 
                   ) return SGE.Rhizomata;

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) &&
                    level >= SGE.Levels.Kerachole &&
                    IsOffCooldown(SGE.Kerachole) && 
                    GetJobGauge<SGEGauge>().Addersgall >= 1
                   ) return SGE.Kerachole;

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) &&
                    level >= SGE.Levels.Ixochole &&
                    IsOffCooldown(SGE.Ixochole) &&
                    GetJobGauge<SGEGauge>().Addersgall >= 1                     
                   ) return SGE.Ixochole;

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis) &&
                    level >= SGE.Levels.Physis &&
                    IsOffCooldown(OriginalHook(SGE.Physis))
                   ) return OriginalHook(SGE.Physis);

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EkPrognosis) &&
                    level >= SGE.Levels.Eukrasia &&
                    FindEffect(SGE.Buffs.EukrasianPrognosis) is null)
                {
                    if (!HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.Eukrasia;
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianPrognosis;
                }

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos) &&
                    level >= SGE.Levels.Holos &&
                    IsOffCooldown(SGE.Holos)
                   ) return SGE.Holos;

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima) &&
                    level >= SGE.Levels.Panhaima &&
                    IsOffCooldown(SGE.Panhaima)
                   ) return SGE.Panhaima;

                if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) &&
                    level >= SGE.Levels.Pepsis &&
                    IsOffCooldown(SGE.Pepsis) &&
                    FindEffect(SGE.Buffs.EukrasianPrognosis) is not null
                   ) return SGE.Pepsis;
            }
            return actionID;
        }
    }
}