using Dalamud.Game.ClientState.JobGauge.Types;

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
            Egeiro = 24287,
            Swiftcast = 7561,
            LucidDreaming = 7562;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                Swiftcast = 167,
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

        public static class Levels //Per 6.0 Patch https://na.finalfantasyxiv.com/jobguide/sage/
        {
            public const byte
                Dosis = 1,
                Diagnosis = 2,
                Kardia = 4,
                Prognosis = 10,
                Egeiro = 12,
                Physis = 20,
                LucidDreaming = 24,
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
                CustomSGELucidDreaming = "CustomSGELucidDreaming",
                CustomZoe = "CustomZoe",
                CustomHaima = "CustomHaima",
                CustomKrasis = "CustomKrasis",
                CustomPepsis = "CustomPepsis",
                CustomSoteria = "CustomSoteria",
                CustomIxochole = "CustomIxochole",
                CustomDiagnosis = "CustomDiagnosis",
                CustomKerachole = "CustomKerachole",
                CustomRhizomata = "CustomRhizomata",
                CustomDruochole = "CustomDruochole",
                CustomTaurochole = "CustomTaurochole";
        }
    }

    //SageKardiaFeature
    //Soteria becomes Kardia when Kardia's Buff is not active and Soteria is on cooldown.
    internal class SageKardiaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageKardiaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Soteria)
            {
                var soteriaCD = GetCooldown(SGE.Soteria);

                if (HasEffect(SGE.Buffs.Kardia) && !soteriaCD.IsCooldown)
                    return SGE.Soteria;

                return SGE.Kardia;
            }

            return actionID;
        }
    }

    //SageRhizomataFeature
    //Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of stacks of Addersgall
    //(Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
    internal class SageRhizomataFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageRhizomataFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Taurochole or SGE.Druochole or SGE.Ixochole or SGE.Kerachole)
            {
                if (level >= SGE.Levels.Rhizomata && GetJobGauge<SGEGauge>().Addersgall == 0)
                        return SGE.Rhizomata;
            }

            return actionID;
        }
    }
    //SageTauroDruoFeature
    //Replaces Taurochole with Druocole when Taurochole is on cooldown or unavailable.
    //(As of 6.0) Taurochole (single target massive insta heal w/ cooldown), Druochole (Single target insta heal)
    internal class SageTauroDruoFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageTauroDruoFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Taurochole)
            {
                var taurocholeCD = GetCooldown(SGE.Taurochole);

                if (taurocholeCD.CooldownRemaining > 0 || level < SGE.Levels.Taurochole)
                    return SGE.Druochole;
            }

            return actionID;
        }
    }

    //Phlegma Replacement Feature
    //Can replace Zero Charges/Stacks of Phlegma with Toxikon (if you can use it) or Dyskrasia 
    internal class SagePhlegmaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SagePhlegmaToxikonFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Phlegma or SGE.Phlegma2 or SGE.Phlegma3)
            {
                if ((IsEnabled(CustomComboPreset.SagePhlegmaToxikonFeature)   && level >= SGE.Levels.Toxikon)   || 
                    (IsEnabled(CustomComboPreset.SagePhlegmaDyskrasiaFeature) && level >= SGE.Levels.Dyskrasia))

                {
                    uint Phlegma; //Phlegma placeholder
                    //Find which version of Phlegma based on player's level that we need to update with
                    switch (level)
                    {
                        case >= (byte)SGE.Levels.Phlegma3: Phlegma = SGE.Phlegma3; break;
                        case >= (byte)SGE.Levels.Phlegma2: Phlegma = SGE.Phlegma2; break;
                        default : Phlegma = SGE.Phlegma; break;
                }

                    //Check for "out of Phlegma stacks" 
                    if (GetCooldown(Phlegma).CooldownRemaining > 45) {
                        //and if we have Adderstings to use for Toxikon
                        //Has Priority over Dyskrasia
                        if ( IsEnabled(CustomComboPreset.SagePhlegmaToxikonFeature) && (level >= SGE.Levels.Toxikon) && (GetJobGauge<SGEGauge>().Addersting > 0) )
                        {
                            return OriginalHook(SGE.Toxikon);  //OriginalHook used so game can use Toxikon 1 & 2
                        }
                        if ( IsEnabled(CustomComboPreset.SagePhlegmaDyskrasiaFeature) && level >= SGE.Levels.Dyskrasia ) 
                        {
                            return OriginalHook(SGE.Dyskrasia); //OriginalHook used so game can use Dyskrasia 1 & 2
                        }
                    }
                }
            }
            return actionID;
        }
    }

    //SageDPSFeature
    //Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
    //Lucid Dreaming optional
    internal class SageDPSFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Dosis1 or SGE.Dosis2 or SGE.Dosis3)
            {
                if (HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                {
                    //If we're too low level to use Eukrasia, we can stop here.
                    if ((CurrentTarget.ObjectKind is Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc) && (level >= SGE.Levels.Eukrasia))
                    {
                        //Eukrasian Dosis vars
                        Dalamud.Game.ClientState.Statuses.Status? DosisDebuffID;

                        //Find which version of Eukrasian Dosis we need 
                        switch (level)
                        {
                            case >= SGE.Levels.Dosis3:
                                DosisDebuffID = FindTargetEffect(SGE.Debuffs.EukrasianDosis3);                                
                                break;
                            case >= SGE.Levels.Dosis2:
                                DosisDebuffID = FindTargetEffect(SGE.Debuffs.EukrasianDosis2);
                                break;
                            default: //Ekrasia Dosis unlocks with Eukrasia, checked at the start
                                DosisDebuffID = FindTargetEffect(SGE.Debuffs.EukrasianDosis1);
                                break;
                        }
                        if (HasEffect(SGE.Buffs.Eukrasia))
                            return OriginalHook(SGE.Dosis1); //OriginalHook will autoselect the correct Dosis for us

                        //Got our Debuff for our level, check for it and procede 
                        if ((DosisDebuffID is null) || (DosisDebuffID.RemainingTime <= 3))
                        {
                            //Advanced Test Options Enabled to procede with auto-Eukrasia
                            if (IsEnabled(CustomComboPreset.SageDPSFeatureAdvTest))
                            {
                                var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                                var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                                var CurrentHpValue = Service.Configuration.EnemyCurrentHp;
                                if ((DosisDebuffID is null && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue) || 
                                    ((DosisDebuffID.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue))
                                {
                                    return SGE.Eukrasia;
                                }
                            }

                            else //End Advanced Test Options. If it needs to be removed, leave the next line
                                return SGE.Eukrasia;
                        }
                    }

                    //Lucid should be usable outside of whatever is targetted
                    if (IsEnabled(CustomComboPreset.SageLucidFeature) && level >= SGE.Levels.LucidDreaming)
                    {
                        var lucidDreaming = GetCooldown(SGE.LucidDreaming);
                        //Get slider configuration
                        int MinMP = Service.Configuration.GetCustomIntValue("CustomSGELucidDreaming", 8000);
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= MinMP && CanWeave(actionID))
                            return SGE.LucidDreaming;
                    }
                }
            }
            return actionID;
        }
    }

    //SageEgeiroFeature
    //Swiftcast combos to Egeiro (Raise) while Swiftcast buff is active
    internal class SageEgeiroFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageEgeiroFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Swiftcast)
            {
                if (IsEnabled(CustomComboPreset.SageEgeiroFeature))
                {
                    if (HasEffect(SGE.Buffs.Swiftcast))
                        return SGE.Egeiro;
                }

                return OriginalHook(SGE.Swiftcast);
            }
            return actionID;
        }
    }

    //SageAlternateEgeiroFeature
    //Egeiro (Raise) is replaced with Swiftcast at all times while not active or on cooldown
    //Egeiro is basically your swiftcast button under this Option.
    internal class SageAlternateEgeiroFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageAlternateEgeiroFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is SGE.Egeiro)
            {
                var swiftCD = GetCooldown(SGE.Swiftcast);

                if ((swiftCD.CooldownRemaining is 0))
                    return SGE.Swiftcast;
            }
            return actionID;
        }
    }

    internal class SageSingleTargetHeal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageSingleTargetHealFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var zoeCD = GetCooldown(SGE.Zoe);
            var HaimaCD = GetCooldown(SGE.Haima);
            var PepsisCD = GetCooldown(SGE.Pepsis);
            var KrasisCD = GetCooldown(SGE.Krasis);
            var SoteriaCD = GetCooldown(SGE.Soteria);
            var RhizomataCD = GetCooldown(SGE.Rhizomata);
            var DruocholeCD = GetCooldown(SGE.Druochole);
            var TaurocholeCD = GetCooldown(SGE.Taurochole);
            var Addersgall = GetJobGauge<SGEGauge>().Addersgall;
            var Kardia = FindEffect(SGE.Buffs.Kardia);
            var Kardion = FindTargetEffect(SGE.Buffs.Kardion);
            var EukrasianDiagnosis = FindTargetEffect(SGE.Buffs.EukrasianDiagnosis);
            var CustomZoe = Service.Configuration.GetCustomIntValue(SGE.Config.CustomZoe);
            var CustomHaima = Service.Configuration.GetCustomIntValue(SGE.Config.CustomHaima);
            var CustomKrasis = Service.Configuration.GetCustomIntValue(SGE.Config.CustomKrasis);
            var CustomPepsis = Service.Configuration.GetCustomIntValue(SGE.Config.CustomPepsis);
            var CustomSoteria = Service.Configuration.GetCustomIntValue(SGE.Config.CustomSoteria);
            var CustomDiagnosis = Service.Configuration.GetCustomIntValue(SGE.Config.CustomDiagnosis);
            var CustomDruochole = Service.Configuration.GetCustomIntValue(SGE.Config.CustomDruochole);
            var CustomTaurochole = Service.Configuration.GetCustomIntValue(SGE.Config.CustomTaurochole);

            if (actionID is SGE.Diagnosis )
            {

                if (IsEnabled(CustomComboPreset.CustomDruocholeFeature) && DruocholeCD.CooldownRemaining is 0 && Addersgall >= 1 && level >= SGE.Levels.Druochole && EnemyHealthPercentage() <= CustomDruochole)
                    return SGE.Druochole;

                if (IsEnabled(CustomComboPreset.CustomTaurocholeFeature) && TaurocholeCD.CooldownRemaining is 0 && Addersgall >= 1 && level >= SGE.Levels.Taurochole && EnemyHealthPercentage() <= CustomTaurochole)
                    return SGE.Taurochole;

                if (IsEnabled(CustomComboPreset.RhizomataFeatureAoE) && RhizomataCD.CooldownRemaining is 0 && Addersgall is 0 && level >= SGE.Levels.Rhizomata)
                    return SGE.Rhizomata;

                if (IsEnabled(CustomComboPreset.AutoApplyKardia) && (Kardion is null) && (Kardia is null))
                    return SGE.Kardia;

                if (CurrentTarget.ObjectKind is Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player)
                {

                    if (IsEnabled(CustomComboPreset.CustomSoteriaFeature) && SoteriaCD.CooldownRemaining is 0 && level >= SGE.Levels.Soteria && EnemyHealthPercentage() <= CustomSoteria)
                        return SGE.Soteria;

                    if (IsEnabled(CustomComboPreset.CustomZoeFeature) && zoeCD.CooldownRemaining is 0 && level >= SGE.Levels.Zoe && EnemyHealthPercentage() <= CustomZoe)
                        return SGE.Zoe;

                    if (IsEnabled(CustomComboPreset.CustomKrasisFeature) && KrasisCD.CooldownRemaining is 0 && level >= SGE.Levels.Krasis && EnemyHealthPercentage() <= CustomKrasis)
                        return SGE.Krasis;

                    if (IsEnabled(CustomComboPreset.CustomPepsisFeature) && PepsisCD.CooldownRemaining is 0 && level >= SGE.Levels.Pepsis && EnemyHealthPercentage() <= CustomPepsis && EukrasianDiagnosis is not null)
                        return SGE.Pepsis;

                    if (IsEnabled(CustomComboPreset.CustomHaimaFeature) && HaimaCD.CooldownRemaining is 0 && level >= SGE.Levels.Haima && EnemyHealthPercentage() <= CustomHaima)
                        return SGE.Haima;
                }
                
                if (IsEnabled(CustomComboPreset.CustomEukrasianDiagnosisFeature) && EukrasianDiagnosis is null && EnemyHealthPercentage() <= CustomDiagnosis)
                {
                    if (!HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.Eukrasia;
                    else return SGE.EukrasianDiagnosis;
                }
            }
            return actionID;
        }
    }

    internal class SageAoEHeal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageAoEHealFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {

            var HolosCD = GetCooldown(SGE.Holos);
            var PhysisCD = GetCooldown(SGE.Physis);
            var PepsisCD = GetCooldown(SGE.Pepsis);
            var Physis2CD = GetCooldown(SGE.Physis2);
            var PanhaimaCD = GetCooldown(SGE.Panhaima);
            var IxocholeCD = GetCooldown(SGE.Ixochole);
            var RhizomataCD = GetCooldown(SGE.Rhizomata);
            var KeracholeCD = GetCooldown(SGE.Kerachole);
            var Addersgall = GetJobGauge<SGEGauge>().Addersgall;
            var EukrasianPrognosis = FindEffect(SGE.Buffs.EukrasianPrognosis);

            if (actionID is SGE.Prognosis)
            {
                if (IsEnabled(CustomComboPreset.RhizomataFeatureAoE) && RhizomataCD.CooldownRemaining is 0 && Addersgall is 0 && level >= SGE.Levels.Rhizomata)
                    return SGE.Rhizomata;

                if (IsEnabled(CustomComboPreset.KeracholeFeature) && KeracholeCD.CooldownRemaining is 0 && Addersgall >= 1 && level >= SGE.Levels.Kerachole)
                    return SGE.Kerachole;

                if (IsEnabled(CustomComboPreset.IxocholeFeature) && IxocholeCD.CooldownRemaining is 0 && Addersgall >= 1 &&  level >= SGE.Levels.Ixochole)
                    return SGE.Ixochole;

                if (IsEnabled(CustomComboPreset.PhysisFeature))
                {
                    if (PhysisCD.CooldownRemaining is 0 && level >= SGE.Levels.Physis && level < SGE.Levels.Physis2)
                        return SGE.Physis;
                    if (Physis2CD.CooldownRemaining is 0 && level >= SGE.Levels.Physis2)
                        return SGE.Physis2;
                }

                if (IsEnabled(CustomComboPreset.EukrasianPrognosisFeature) && EukrasianPrognosis is null)
                {
                    if (!HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.Eukrasia;
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianPrognosis;
                }

                if (IsEnabled(CustomComboPreset.HolosFeature) && HolosCD.CooldownRemaining is 0 && level >= SGE.Levels.Holos)
                    return SGE.Holos;

                if (IsEnabled(CustomComboPreset.PanhaimaFeature) && PanhaimaCD.CooldownRemaining is 0 && level >= SGE.Levels.Panhaima)
                    return SGE.Panhaima;

                if (IsEnabled(CustomComboPreset.PepsisFeature) && PepsisCD.CooldownRemaining is 0 && level >= SGE.Levels.Pepsis && EukrasianPrognosis is not null)
                    return SGE.Pepsis;

            }
            return actionID;
        }
    }
}
