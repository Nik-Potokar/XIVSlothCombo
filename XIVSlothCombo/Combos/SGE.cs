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
            Egeiro = 24287;

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
        public static class Range
        {
            public const byte Phlegma = 6;
        }

        public static class Config
        {
            public const string
                //GUI Customization Storage Names
                SGE_ST_Dosis_EDosisHPPer = "SGE_ST_Dosis_EDosisHPPer",
                SGE_ST_Dosis_EDosisHPMax = "SGE_ST_Dosis_EDosisHPMax",
                SGE_ST_Dosis_EDosisCurHP = "SGE_ST_Dosis_EDosisCurHP",
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



        //SageSoteriaKardia
        //Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
        internal class SageSoteriaKardiaFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_KardiaFeature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Soteria &&
                    (!HasEffect(Buffs.Kardia) || IsOnCooldown(Soteria))
                   ) return Kardia;
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
                if (actionID is Taurochole or Druochole or Ixochole or Kerachole &&
                    level >= Levels.Rhizomata &&
                    GetJobGauge<SGEGauge>().Addersgall == 0
                   ) return Rhizomata;
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
                if (actionID is Druochole && level >= Levels.Taurochole && IsOffCooldown(Taurochole)) return Taurochole;
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
                if (actionID is Phlegma or Phlegma2 or Phlegma3)
                {
                    var NoPhlegmaToxikon  = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaToxikon);
                    var OutOfRangeToxikon = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_OutOfRangeToxikon);
                    if ((NoPhlegmaToxikon || OutOfRangeToxikon) &&
                        level >= Levels.Toxikon &&
                        HasBattleTarget() && 
                        GetJobGauge<SGEGauge>().Addersting > 0)
                    {
                        if ((NoPhlegmaToxikon && GetCooldown(OriginalHook(Phlegma)).RemainingCharges == 0) ||
                            (OutOfRangeToxikon && (GetTargetDistance() > Range.Phlegma)))
                           return OriginalHook(Toxikon);
                    }
                    var NoPhlegmaDyskrasia = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaDyskrasia);
                    var NoTargetDyskrasia  = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoTargetDyskrasia);
                    if ((NoPhlegmaDyskrasia || NoTargetDyskrasia) &&
                        level >= Levels.Phlegma)
                    {
                        if ((NoPhlegmaDyskrasia && GetCooldown(OriginalHook(Phlegma)).RemainingCharges == 0) ||
                            (NoTargetDyskrasia && CurrentTarget is null))
                           return OriginalHook(Dyskrasia);
                    }
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
                if (actionID is Dosis1 or Dosis2 or Dosis3 && InCombat())
                {
                    //Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Lucid) &&
                        level >= All.Levels.LucidDreaming &&
                        IsOffCooldown(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.SGE_ST_Dosis_Lucid) &&
                        CanSpellWeave(actionID)
                       ) return All.LucidDreaming;

                    //Eukrasian Dosis.
                    //If we're too low level to use Eukrasia, we can stop here.
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosis) && (level >= Levels.Eukrasia) && CurrentTarget is not null)
                    {

                        //If we're already Eukrasian'd, the whole point of this section is moot
                        if (HasEffect(Buffs.Eukrasia)) return OriginalHook(Dosis1); //OriginalHook will autoselect the correct Dosis for us

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
                            >= Levels.Dosis3 => FindEffect(Debuffs.EukrasianDosis3, OurTarget, LocalPlayer?.ObjectId),
                            >= Levels.Dosis2 => FindEffect(Debuffs.EukrasianDosis2, OurTarget, LocalPlayer?.ObjectId),
                            //Ekrasia Dosis unlocks with Eukrasia, checked at the start
                            _ => FindEffect(Debuffs.EukrasianDosis1, OurTarget, LocalPlayer?.ObjectId),
                        };
                    
                        //Got our Debuff for our level, check for it and procede 
                        if ((DosisDebuffID is null) || (DosisDebuffID.RemainingTime <= 3))
                        {
                            //Advanced Options Enabled to procede with auto-Eukrasia
                            //Incompatible with ToT due to Enemy checks that are using CurrentTarget.
                            if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosisHPLimiters))
                            {
                                var MaxHpValue = GetOptionValue(Config.SGE_ST_Dosis_EDosisHPMax);
                                var PercentageHpValue = GetOptionValue(Config.SGE_ST_Dosis_EDosisHPPer);
                                var CurrentHpValue = GetOptionValue(Config.SGE_ST_Dosis_EDosisCurHP);

                                if ( (DosisDebuffID is null && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue) ||
                                    ((DosisDebuffID?.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue) )
                                   return Eukrasia;
                            }
                            else return Eukrasia;
                        } 
                    }

                    //Toxikon
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Toxikon) &&
                        level >= Levels.Toxikon &&
                        HasBattleTarget() &&
                        ((GetOptionValue(Config.SGE_ST_Dosis_Toxikon) == 1 && this.IsMoving) || (GetOptionValue(Config.SGE_ST_Dosis_Toxikon) == 2)) &&
                        GetJobGauge<SGEGauge>().Addersting > 0
                       ) return OriginalHook(Toxikon);
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
                if (actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast)) return Egeiro;
                else return actionID;
            }
        }

        internal class SageSingleTargetHealFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_HealFeature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Diagnosis)
                {
                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) &&
                        level >= Levels.Druochole &&
                        IsOffCooldown(Druochole) &&
                        GetJobGauge<SGEGauge>().Addersgall >= 1 &&
                        EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Druochole)
                       ) return Druochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) &&
                        level >= Levels.Taurochole &&
                        IsOffCooldown(Taurochole) &&
                        GetJobGauge<SGEGauge>().Addersgall >= 1 &&
                        EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Taurochole)
                       ) return Taurochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) &&
                        level >= Levels.Rhizomata &&
                        IsOffCooldown(Rhizomata) &&
                        GetJobGauge<SGEGauge>().Addersgall is 0
                       ) return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) &&
                        level >= Levels.Kardia &&
                        FindEffect(Buffs.Kardia) is null &&
                        FindTargetEffect(Buffs.Kardion) is null
                       ) return Kardia;

                    if (CurrentTarget?.ObjectKind is ObjectKind.Player)
                    {
                        if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria) &&
                            level >= Levels.Soteria &&
                            IsOffCooldown(Soteria) &&
                            EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Soteria)
                           ) return Soteria;

                        if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe) &&
                            level >= Levels.Zoe &&
                            IsOffCooldown(Zoe) &&
                            EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Zoe)
                           ) return Zoe;

                        if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis) &&
                            level >= Levels.Krasis &&
                            IsOffCooldown(Krasis) && EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Krasis)
                           ) return Krasis;

                        if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) &&
                            level >= Levels.Pepsis &&
                            IsOffCooldown(Pepsis) && EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Pepsis) &&
                            FindTargetEffect(Buffs.EukrasianDiagnosis) is not null
                           ) return Pepsis;

                        if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima) &&
                            level >= Levels.Haima &&
                            IsOffCooldown(Haima) && EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Haima)
                           ) return Haima;
                    }

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Diagnosis) &&
                        level >= Levels.Eukrasia &&
                        FindTargetEffect(Buffs.EukrasianDiagnosis) is null &&
                        EnemyHealthPercentage() <= GetOptionValue(Config.SGE_ST_Heal_Diagnosis))
                    {
                        if (!HasEffect(Buffs.Eukrasia))
                            return Eukrasia;
                        else return EukrasianDiagnosis;
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
                if (actionID is Prognosis)
                {
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) &&
                        level >= Levels.Rhizomata &&
                        IsOffCooldown(Rhizomata) &&
                        GetJobGauge<SGEGauge>().Addersgall is 0
                       ) return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) &&
                        level >= Levels.Kerachole &&
                        IsOffCooldown(Kerachole) &&
                        GetJobGauge<SGEGauge>().Addersgall >= 1
                       ) return Kerachole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) &&
                        level >= Levels.Ixochole &&
                        IsOffCooldown(Ixochole) &&
                        GetJobGauge<SGEGauge>().Addersgall >= 1
                       ) return Ixochole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis) &&
                        level >= Levels.Physis &&
                        IsOffCooldown(OriginalHook(Physis))
                       ) return OriginalHook(Physis);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EkPrognosis) &&
                        level >= Levels.Eukrasia &&
                        FindEffect(Buffs.EukrasianPrognosis) is null)
                    {
                        if (!HasEffect(Buffs.Eukrasia))
                            return Eukrasia;
                        if (HasEffect(Buffs.Eukrasia))
                            return EukrasianPrognosis;
                    }

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos) &&
                        level >= Levels.Holos &&
                        IsOffCooldown(Holos)
                       ) return Holos;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima) &&
                        level >= Levels.Panhaima &&
                        IsOffCooldown(Panhaima)
                       ) return Panhaima;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) &&
                        level >= Levels.Pepsis &&
                        IsOffCooldown(Pepsis) &&
                        FindEffect(Buffs.EukrasianPrognosis) is not null
                       ) return Pepsis;
                }
                return actionID;
            }
        }
    }
}