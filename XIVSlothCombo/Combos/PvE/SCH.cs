using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using XIVSlothCombo.CustomComboNS;


namespace XIVSlothCombo.Combos.PvE
{
    internal static class SCH
    {
        public const byte ClassID = 26;
        public const byte JobID = 28;

        public const uint

            // Heals
            Physick = 190,
            Adloquium = 185,
            Succor = 186,
            Lustrate = 189,
            SacredSoil = 188,
            Indomitability = 3583,
            Excogitation = 7434,
            Consolation = 16546,

            // Offense
            Bio1 = 17864,
            Bio2 = 17865,
            Biolysis = 16540,
            Ruin1 = 17869,
            Ruin2 = 17870,
            Broil1 = 3584,
            Broil2 = 7435,
            Broil3 = 16541,
            Broil4 = 25865,
            Scourge = 16539,
            EnergyDrain = 167,

            // Faerie
            SummonSeraph = 16545,
            SummonEos = 17215,
            SummonSelene = 17216,
            WhisperingDawn = 16537,
            FeyIllumination = 16538,
            Dissipation = 3587,
            Aetherpact = 7437,
            FeyBlessing = 16543,

            // Other
            Aetherflow = 166,
            Recitation = 16542,
            ChainStratagem = 7436,
            DeploymentTactics = 3585,

            // Role
            Resurrection = 173;

        public static class Buffs
        {
            public const ushort
            Galvanize = 297,
            Recitation = 1896;
        }

        public static class Debuffs
        {
            public const ushort
            Bio1 = 179,
            Bio2 = 189,
            Biolysis = 1895,
            ChainStratagem = 1221;
        }

        public static class Levels
        {
            public const byte

                Bio1 = 2,
                Physick = 4,
                WhisperingDawn = 20,
                Bio2 = 26,
                Adloquium = 30,
                Succor = 35,
                Ruin2 = 38,
                FeyIllumination = 40,
                Aetherflow = 45,
                EnergyDrain = 45,
                Lustrate = 45,
                Scourge = 46,
                SacredSoil = 50,
                Indomitability = 52,
                Broil = 54,
                DeploymentTactics = 56,
                EmergencyTactics = 58,
                Dissipation = 60,
                Excogitation = 62,
                Broil2 = 64,
                ChainStratagem = 66,
                Aetherpact = 70,
                Biolysis = 72,
                Broil3 = 72,
                Recitation = 74,
                FeyBlessing = 76,
                SummonSeraph = 80,
                Broil4 = 82,
                Scoura = 82,
                Protraction = 86,
                Expedient = 90;
        }

        public static class Config
        {
            public const string
                SCH_ST_DPS_AltMode = "SCH_ST_DPS_AltMode",
                SCH_ST_DPS_LucidOption = "SCH_ST_DPS_LucidOption",
                SCH_ST_DPS_BioOption = "SCH_ST_DPS_BioOption",
                SCH_ST_DPS_ChainStratagemOption = "SCH_ST_DPS_ChainStratagemOption",
                SCH_Aetherflow_Display = "SCH_Aetherflow_Display",
                SCH_Aetherflow_Recite_Excog = "SCH_Aetherflow_Recite_Excog",
                SCH_Aetherflow_Recite_Indom = "SCH_Aetherflow_Recite_Indom",
                SCH_FairyFeature = "SCH_FairyFeature";
        }


        // Even though Summon Seraph becomes Consolation, this Feature puts the temporary Fairy AoE heal+barrier ontop of the existing fairy AoE skill, Fey Blessing
        internal class SCH_Consolation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Consolation;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FeyBlessing &&
                    level >= Levels.SummonSeraph &&
                    GetJobGauge<SCHGauge>().SeraphTimer > 0
                   ) return Consolation;
                else return actionID;
            }
        }

        // Replaces all Energy Drain actions with Aetherflow when depleted
        // Revised to a similar flow as SGE Rhizomata, but with Dissipation / Recitation as a backup
        internal class SCH_Aetherflow : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Aetherflow;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is EnergyDrain or Lustrate or SacredSoil or Indomitability or Excogitation &&
                    level >= Levels.Aetherflow)
                {
                    var HasAetherFlows = System.Convert.ToBoolean(GetJobGauge<SCHGauge>().Aetherflow); //False if Zero stacks
                    if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite) &&
                        level >= Levels.Recitation &&
                        (IsOffCooldown(Recitation) || HasEffect(Buffs.Recitation)))
                    {
                        //Recitation Indominability and Excogitation, with optional check against AF zero stack count
                        bool AlwaysShowReciteExcog = GetOptionBool(Config.SCH_Aetherflow_Recite_Excog);
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Excog) &&
                            (AlwaysShowReciteExcog || (!AlwaysShowReciteExcog && !HasAetherFlows)) &&
                            actionID is Excogitation)
                        {   //Do not merge this nested if with above. Won't procede with next set
                            if (HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation)) return Excogitation; else return Recitation;
                        }

                        bool AlwaysShowReciteIndom = GetOptionBool(Config.SCH_Aetherflow_Recite_Indom);
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Indom) &&
                            (AlwaysShowReciteIndom || (!AlwaysShowReciteIndom && !HasAetherFlows)) &&
                            actionID is Indomitability)
                        {
                            if (HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation)) return Indomitability; else return Recitation;
                        }
                    }
                    if (!HasAetherFlows)
                    {
                        bool ShowAetherflowOnAll = GetOptionBool(Config.SCH_Aetherflow_Display);
                        if ((actionID is EnergyDrain && !ShowAetherflowOnAll) || ShowAetherflowOnAll)
                        {
                            if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Dissipation) &&
                                level >= Levels.Dissipation &&
                                IsOffCooldown(Dissipation) &&
                                IsOnCooldown(Aetherflow) &&
                                //Dissipation requires fairy, can't seem to make it replace dissipation with fairy summon feature *shrug*
                                HasPetPresent()) return Dissipation;

                            else return Aetherflow;
                        }
                    }
                }
                return actionID;
            }
        }

        // Swiftcast changes to Raise when activated / on cooldown
        internal class SCH_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast)) return Resurrection;
                else return actionID;
            }
        }

        // Replaces Fairy abilities with Fairy summoning with Eos (default) or Selene
        internal class SCH_FairyReminder : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_FairyReminder;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is WhisperingDawn or FeyBlessing or FeyIllumination or Dissipation or Aetherpact &&
                    !HasPetPresent() && 
                    GetJobGauge<SCHGauge>().SeraphTimer == 0)
                {
                    if (GetOptionBool(Config.SCH_FairyFeature)) return SummonSelene; else return SummonEos;
                }
                return actionID;
            }
        }

        internal class SCH_DeploymentTactics : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DeploymentTactics;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DeploymentTactics)
                {
                    if (IsOffCooldown(DeploymentTactics) && LevelChecked(DeploymentTactics)) //Allows Adlo to work at sync
                    {
                        bool found = false;
                        //If we have a soft target, use that, else CurrentTarget.
                        GameObject? target = Services.Service.TargetManager.SoftTarget is not null ? Services.Service.TargetManager.SoftTarget : CurrentTarget;

                        if (target is not null)
                        {

                            //What's our party size? Check if trust, and add 1 (trust does not include player), else get Party Count
                            //It's okay if both Buddy/Party are Zero. For loop will skip
                            int PartySize = Services.Service.BuddyList.Length > 0 ? Services.Service.BuddyList.Length + 1 : GetPartyMembers().Length;

                            //Check if our target is in the party. Will skip if partysize is zero
                            for (int i = 1; i <= PartySize; i++)
                            {
                                found = (GetPartySlot(i) == target);
                                if (found) break;
                            }
                            //Check if it's our chocobo?
                            if (found is false) found = (HasCompanionPresent() && target == Services.Service.BuddyList.CompanionBuddy.GameObject);
                        }

                        //Fall back to self, skills won't work with anyone else.
                        if (target is null || found is false)
                        {
                            target = LocalPlayer;
                            found = true;
                        }

                        if (found)
                        {
                            if (FindEffect(Buffs.Galvanize, target, LocalPlayer.ObjectId) is not null) return DeploymentTactics;
                            //Recitation is down here as not to waste it on bad targets.
                            if (IsEnabled(CustomComboPreset.SCH_DeploymentTactics_Recitation) 
                                && LevelChecked(Recitation)
                                && IsOffCooldown(Recitation)
                               ) return Recitation;
                        }
                    }
                    return Adloquium;
                }
                return actionID;
            }
        }

        /*
        Overrides main DPS ability family, The Broils (and Ruin 1)
        Implements Ruin 2 as the movement option
        Chain Stratagem has overlap protection
        */
        internal class SCH_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool AlternateMode = GetOptionBool(Config.SCH_ST_DPS_AltMode); //(0 or 1 radio values)
                if ((!AlternateMode &&  actionID is Ruin1 or Broil1 or Broil2 or Broil3 or Broil4
                     || (AlternateMode && actionID is Bio1 or Bio2 or Biolysis)) 
                    && InCombat())
                {
                    //Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Lucid) &&
                        level >= All.Levels.LucidDreaming &&
                        IsOffCooldown(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.SCH_ST_DPS_LucidOption) &&
                        CanSpellWeave(actionID)
                       ) return All.LucidDreaming;

                    //Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Aetherflow) &&
                        level >= Levels.Aetherflow &&
                        GetJobGauge<SCHGauge>().Aetherflow == 0 &&
                        IsOffCooldown(Aetherflow) &&
                        CanSpellWeave(actionID)
                       ) return Aetherflow;

                    //Chain Stratagem
                    if (IsEnabled(CustomComboPreset.SCH_DPS_ChainStrat) &&
                        level >= Levels.ChainStratagem &&
                        IsOffCooldown(ChainStratagem) &&
                        !TargetHasEffectAny(Debuffs.ChainStratagem) && //Overwrite protection
                        GetTargetHPPercent() > GetOptionValue(Config.SCH_ST_DPS_ChainStratagemOption) &&
                        CanSpellWeave(actionID)
                       ) return ChainStratagem;

                    //Ruin 2 Movement 
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Ruin2Movement) &&
                        level >= Levels.Ruin2 &&
                        HasBattleTarget() &&
                        this.IsMoving
                       ) return OriginalHook(Ruin2); //Who knows in the future

                    //Bio/Biolysis
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Bio) && 
                        level >= Levels.Bio1 && 
                        (CurrentTarget as BattleNpc)?.BattleNpcKind is BattleNpcSubKind.Enemy)
                    {
                        //Determine which Bio debuff to check
                        var BioDebuffID = level switch
                        {
                            //Using FindEffect b/c we have a custom Target variable
                            >= Levels.Biolysis => FindTargetEffect(Debuffs.Biolysis),
                            >= Levels.Bio2 => FindTargetEffect(Debuffs.Bio2),
                            //Bio 1 checked at the start, fine for default
                            _ => FindTargetEffect(Debuffs.Bio1),
                        };
                        if ((BioDebuffID is null || BioDebuffID?.RemainingTime <= 3) &&
                            (GetTargetHPPercent() > GetOptionValue(Config.SCH_ST_DPS_BioOption))
                           ) return OriginalHook(Bio1);

                        //AlterateMode idles as Ruin/Broil
                        if (AlternateMode) return OriginalHook(Ruin1);
                    }
                }
                return actionID;
            }
        }
    }
}
