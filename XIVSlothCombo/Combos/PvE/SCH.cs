using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class SCH
    {
        public const byte ClassID = 26;
        public const byte JobID = 28;

        private static SCHGauge Gauge => CustomComboNS.Functions.CustomComboFunctions.GetJobGauge<SCHGauge>();

        internal const uint

            // Heals
            Physick = 190,
            Adloquium = 185,
            Succor = 186,
            Lustrate = 189,
            SacredSoil = 188,
            Indomitability = 3583,
            Excogitation = 7434,
            Consolation = 16546,
            Resurrection = 173,

            // Offense
            Bio = 17864,
            Bio2 = 17865,
            Biolysis = 16540,
            Ruin = 17869,
            Ruin2 = 17870,
            Broil = 3584,
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
            DeploymentTactics = 3585;

        //Action Groups
        internal static readonly List<uint>
            BroilList = new() { Ruin, Broil, Broil2, Broil3, Broil4 },
            AetherflowList = new() { EnergyDrain, Lustrate, SacredSoil, Indomitability, Excogitation },
            FairyList = new() { WhisperingDawn, FeyBlessing, FeyIllumination, Dissipation, Aetherpact };

        internal static class Buffs
        {
            internal const ushort
                Galvanize = 297,
                Recitation = 1896;
        }

        internal static class Debuffs
        {
            internal const ushort
                Bio1 = 179,
                Bio2 = 189,
                Biolysis = 1895,
                ChainStratagem = 1221;
        }

        //Debuff Pairs of Actions and Debuff
        internal static readonly Dictionary<uint, ushort>
            BioList = new() {
                { Bio, Debuffs.Bio1 },
                { Bio2, Debuffs.Bio2 },
                { Biolysis, Debuffs.Biolysis }
            };

        internal static class Config
        {
            internal const string
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
                => actionID is FeyBlessing && LevelChecked(SummonSeraph) && Gauge.SeraphTimer > 0 ? Consolation : actionID;
        }

        // Replaces all Energy Drain actions with Aetherflow when depleted
        // Revised to a similar flow as SGE Rhizomata, but with Dissipation / Recitation as a backup
        internal class SCH_Aetherflow : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Aetherflow;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (AetherflowList.Contains(actionID) && LevelChecked(Aetherflow))
                {
                    bool HasAetherFlows = System.Convert.ToBoolean(Gauge.Aetherflow); //False if Zero stacks
                    if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite) &&
                        LevelChecked(Recitation) &&
                        (IsOffCooldown(Recitation) || HasEffect(Buffs.Recitation)))
                    {
                        //Recitation Indominability and Excogitation, with optional check against AF zero stack count
                        bool AlwaysShowReciteExcog = GetIntOptionAsBool(Config.SCH_Aetherflow_Recite_Excog);
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Excog) &&
                            (AlwaysShowReciteExcog || (!AlwaysShowReciteExcog && !HasAetherFlows)) &&
                            actionID is Excogitation)
                        {   //Do not merge this nested if with above. Won't procede with next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Excogitation : Recitation;
                        }

                        bool AlwaysShowReciteIndom = GetIntOptionAsBool(Config.SCH_Aetherflow_Recite_Indom);
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Indom) &&
                            (AlwaysShowReciteIndom || (!AlwaysShowReciteIndom && !HasAetherFlows)) &&
                            actionID is Indomitability)
                        {   //Same as above, do not nest with above. It won't procede with the next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Indomitability : Recitation;
                        }
                    }
                    if (!HasAetherFlows)
                    {
                        bool ShowAetherflowOnAll = GetIntOptionAsBool(Config.SCH_Aetherflow_Display);
                        if ((actionID is EnergyDrain && !ShowAetherflowOnAll) || ShowAetherflowOnAll)
                        {
                            if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Dissipation) &&
                                ActionReady(Dissipation) &&
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
                => actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Resurrection : actionID;
        }

        // Replaces Fairy abilities with Fairy summoning with Eos (default) or Selene
        internal class SCH_FairyReminder : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_FairyReminder;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => FairyList.Contains(actionID) && !HasPetPresent() && Gauge.SeraphTimer == 0
                    ? GetIntOptionAsBool(Config.SCH_FairyFeature) ? SummonSelene : SummonEos
                    : actionID;
        }

        /*
         * Combos Deployment Tactics with Adloquium by showing Adloquim instead,
         * while leaving the real Adloquim alone.
         * Will work on Party/Trust/Chocobo hard/soft targets
         * Recitation is optional, if one wishes to Crit the shield first
         */
        internal class SCH_DeploymentTactics : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DeploymentTactics;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DeploymentTactics)
                {
                    if (ActionReady(DeploymentTactics)) //Allows Adlo to work at sync, do not nest with above
                    {
                        bool found = false;
                        //If we have a soft target, use that, else CurrentTarget.
                        GameObject? target = Services.Service.TargetManager.SoftTarget is not null ? Services.Service.TargetManager.SoftTarget : CurrentTarget;

                        if (target is not null)
                        {
                            if (IsInParty())
                            {
                                //Search the party
                                for (int i = 1; i <= 8; i++)
                                {
                                    GameObject? member = GetPartySlot(i);
                                    if (member == null) continue; //Skip nulls/disconnected people

                                    found = (member == target);
                                    if (found) break;
                                }
                            }
                            //Check if it's our chocobo?
                            if (found is false) found = HasCompanionPresent() && target == Services.Service.BuddyList.CompanionBuddy.GameObject;
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
                            if (IsEnabled(CustomComboPreset.SCH_DeploymentTactics_Recitation) && ActionReady(Recitation))
                                return Recitation;
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
                bool AlternateMode = GetIntOptionAsBool(Config.SCH_ST_DPS_AltMode); //(0 or 1 radio values)
                if (((!AlternateMode && BroilList.Contains(actionID)) ||
                     (AlternateMode && BioList.ContainsKey(actionID))) &&
                    InCombat())
                {
                    //Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.SCH_ST_DPS_LucidOption) &&
                        CanSpellWeave(actionID)) return All.LucidDreaming;

                    //Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Aetherflow) &&
                        ActionReady(Aetherflow) &&
                        Gauge.Aetherflow is 0 &&
                        CanSpellWeave(actionID)) return Aetherflow;

                    //Target based options
                    if (HasBattleTarget())
                    {
                        //Chain Stratagem
                        if (IsEnabled(CustomComboPreset.SCH_DPS_ChainStrat) &&
                            ActionReady(ChainStratagem) &&
                            !TargetHasEffectAny(Debuffs.ChainStratagem) && //Overwrite protection
                            GetTargetHPPercent() > GetOptionValue(Config.SCH_ST_DPS_ChainStratagemOption) &&
                            CanSpellWeave(actionID)) return ChainStratagem;

                        //Ruin 2 Movement 
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Ruin2Movement) &&
                            LevelChecked(Ruin2) &&
                            IsOffCooldown(actionID) && //Check against actionID to stop seizure during cooldown 
                            IsMoving) return OriginalHook(Ruin2); //Who knows in the future

                        //Bio/Biolysis
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Bio) && LevelChecked(Bio))
                        {
                            uint dot = OriginalHook(Bio); //Grab the appropriate DoT Action
                            Status? dotDebuff = FindTargetEffect(BioList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                            if ((dotDebuff is null || dotDebuff?.RemainingTime <= 3) &&
                                (GetTargetHPPercent() > GetOptionValue(Config.SCH_ST_DPS_BioOption)))
                                return dot; //Use appropriate DoT Action

                            //AlterateMode idles as Ruin/Broil
                            if (AlternateMode) return OriginalHook(Ruin);
                        }
                    }
                }
                return actionID;
            }
        }
    }
}
