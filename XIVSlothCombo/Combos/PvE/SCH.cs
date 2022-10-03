using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class SCH
    {
        public const byte ClassID = 26;
        public const byte JobID = 28;

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
            EnergyDrain = 167,
            ArtOfWar = 16539,
            ArtOfWarII = 25866,

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

        // Class Gauge
        private static SCHGauge Gauge => CustomComboFunctions.GetJobGauge<SCHGauge>();
        private static bool HasAetherflow(this SCHGauge gauge) => (gauge.Aetherflow > 0);

        internal enum OpenerState
        {
            PreOpener,
            InOpener,
            PostOpener,
        }

        internal static class Config
        {
            internal static bool SCH_ST_DPS_AltMode => CustomComboFunctions.GetIntOptionAsBool(nameof(SCH_ST_DPS_AltMode));
            internal static int SCH_ST_DPS_LucidOption => CustomComboFunctions.GetOptionValue(nameof(SCH_ST_DPS_LucidOption));
            internal static int SCH_ST_DPS_BioOption => CustomComboFunctions.GetOptionValue(nameof(SCH_ST_DPS_BioOption));
            internal static int SCH_ST_DPS_ChainStratagemOption => CustomComboFunctions.GetOptionValue(nameof(SCH_ST_DPS_ChainStratagemOption));
            internal static float SCH_ST_DPS_EnergyDrain => CustomComboFunctions.GetOptionFloat(nameof(SCH_ST_DPS_EnergyDrain));
            internal static int SCH_AoE_LucidOption => CustomComboFunctions.GetOptionValue(nameof(SCH_AoE_LucidOption));
            internal static bool SCH_Aetherflow_Display => CustomComboFunctions.GetIntOptionAsBool(nameof(SCH_Aetherflow_Display));
            internal static bool SCH_Aetherflow_Recite_Excog => CustomComboFunctions.GetIntOptionAsBool(nameof(SCH_Aetherflow_Recite_Excog));
            internal static bool SCH_Aetherflow_Recite_Indom => CustomComboFunctions.GetIntOptionAsBool(nameof(SCH_Aetherflow_Recite_Indom));
            internal static bool SCH_FairyFeature => CustomComboFunctions.GetIntOptionAsBool(nameof(SCH_FairyFeature));
            internal static int SCH_Recitation_Mode => CustomComboFunctions.GetOptionValue(nameof(SCH_Recitation_Mode));

            internal const string SCH_ST_DPS_Bio_Threshold = "SCH_ST_DPS_Bio_Threshold";

        }

        /*
         * SCH_Consolation
         * Even though Summon Seraph becomes Consolation, 
         * This Feature also places Seraph's AoE heal+barrier ontop of the existing fairy AoE skill, Fey Blessing
         */
        internal class SCH_Consolation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Consolation;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is FeyBlessing && LevelChecked(SummonSeraph) && Gauge.SeraphTimer > 0 ? Consolation : actionID;
        }

        /*
         * SCH_Lustrate
         * Replaces Lustrate with Excogitation when Excogitation is ready.
        */
        internal class SCH_Lustrate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Lustrate;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Lustrate && LevelChecked(Excogitation) && IsOffCooldown(Excogitation) ? Excogitation : actionID;
        }

        /*
         * SCH_Recitation
         * Replaces Recitation with selected one of its combo skills.
        */
        internal class SCH_Recitation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Recitation;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Recitation && HasEffect(Buffs.Recitation))
                {
                    switch (Config.SCH_Recitation_Mode)
                    {
                        case 0: return OriginalHook(Adloquium);
                        case 1: return OriginalHook(Succor);
                        case 2: return OriginalHook(Indomitability);
                        case 3: return OriginalHook(Excogitation);
                        default: break;
                    }
                }

                return actionID;
            }
        }


        /*
         * SCH_Aetherflow
         * Replaces all Energy Drain actions with Aetherflow when depleted, or just Energy Drain
         * Dissipation option to show if Aetherflow is on Cooldown
         * Recitation also an option
        */
        internal class SCH_Aetherflow : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Aetherflow;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (AetherflowList.Contains(actionID) && LevelChecked(Aetherflow))
                {
                    bool HasAetherFlows = Gauge.HasAetherflow(); //False if Zero stacks
                    if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite) &&
                        LevelChecked(Recitation) &&
                        (IsOffCooldown(Recitation) || HasEffect(Buffs.Recitation)))
                    {
                        //Recitation Indominability and Excogitation, with optional check against AF zero stack count
                        bool AlwaysShowReciteExcog = Config.SCH_Aetherflow_Recite_Excog;
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Excog) &&
                            (AlwaysShowReciteExcog || (!AlwaysShowReciteExcog && !HasAetherFlows)) &&
                            actionID is Excogitation)
                        {   //Do not merge this nested if with above. Won't procede with next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Excogitation : Recitation;
                        }

                        bool AlwaysShowReciteIndom = Config.SCH_Aetherflow_Recite_Indom;
                        if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite_Indom) &&
                            (AlwaysShowReciteIndom || (!AlwaysShowReciteIndom && !HasAetherFlows)) &&
                            actionID is Indomitability)
                        {   //Same as above, do not nest with above. It won't procede with the next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Indomitability : Recitation;
                        }
                    }
                    if (!HasAetherFlows)
                    {
                        bool ShowAetherflowOnAll = Config.SCH_Aetherflow_Display;
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

        /*
         * SCH_Raise (Swiftcast Raise combo)
         * Swiftcast changes to Raise when swiftcast is on cooldown
         */
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
                    ? Config.SCH_FairyFeature ? SummonSelene : SummonEos
                    : actionID;
        }

        /*
         * SCH_DeploymentTactics
         * Combos Deployment Tactics with Adloquium by showing Adloquim when Deployment Tactics is ready,
         * Recitation is optional, if one wishes to Crit the shield first
         * Supports soft targetting and self as a fallback.
         */
        internal class SCH_DeploymentTactics : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DeploymentTactics;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DeploymentTactics && ActionReady(DeploymentTactics))
                {
                    //Grab our target (Soft->Hard->Self)
                    GameObject? healTarget = null;
                    GameObject? softTarget = Service.TargetManager.SoftTarget;
                    if (HasFriendlyTarget(softTarget)) healTarget = softTarget;
                    if (healTarget is null && HasFriendlyTarget(CurrentTarget)) healTarget = CurrentTarget;
                    if (healTarget is null) healTarget = LocalPlayer;

                    //Check for the Galvanize shield buff. Start applying if it doesn't exist
                    if (FindEffect(Buffs.Galvanize, healTarget, LocalPlayer.ObjectId) is null)
                    {
                        if (IsEnabled(CustomComboPreset.SCH_DeploymentTactics_Recitation) && ActionReady(Recitation))
                            return Recitation;

                        return Adloquium;
                    }
                }
                return actionID;
            }
        }

        /*
         * SCH_DPS
         * Overrides main DPS ability family, The Broils (and Ruin 1)
         * Implements Ruin 2 as the movement option
         * Chain Stratagem has overlap protection
        */
        internal class SCH_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DPS;

            internal OpenerState openerState = OpenerState.PreOpener;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool AlternateMode = Config.SCH_ST_DPS_AltMode; //(0 or 1 radio values)
                if (((!AlternateMode && BroilList.Contains(actionID)) ||
                     (AlternateMode && BioList.ContainsKey(actionID))))
                {
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    if (!incombat)
                    {
                        openerState = OpenerState.PreOpener;
                    }
                    else if (Gauge.HasAetherflow())
                    {
                        openerState = OpenerState.PostOpener;
                    }
                    else if (IsEnabled(CustomComboPreset.SCH_DPS_Dissipation_Opener) && (openerState != OpenerState.PostOpener))
                    {
                        openerState = OpenerState.InOpener;
                    }

                    // Dissipation
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Dissipation_Opener) &&
                        ActionReady(Dissipation) && HasPetPresent() && !Gauge.HasAetherflow() &&
                        (openerState == OpenerState.InOpener) && InCombat() && CanSpellWeave(actionID))
                        return Dissipation;

                    // Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Aetherflow) &&
                        ActionReady(Aetherflow) && !Gauge.HasAetherflow() &&
                        InCombat() && CanSpellWeave(actionID))
                        return Aetherflow;

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.SCH_ST_DPS_LucidOption &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;

                    //Target based options
                    if (HasBattleTarget())
                    {
                        // Energy Drain
                        if (IsEnabled(CustomComboPreset.SCH_DPS_EnergyDrain) &&
                            LevelChecked(EnergyDrain) && InCombat() &&
                            Gauge.HasAetherflow() &&
                            GetCooldownRemainingTime(Aetherflow) <= (Config.SCH_ST_DPS_EnergyDrain * Gauge.Aetherflow) &&
                            (!IsEnabled(CustomComboPreset.SCH_DPS_EnergyDrain_BurstSaver) || GetCooldownRemainingTime(ChainStratagem) > 10) &&
                            CanSpellWeave(actionID))
                            return EnergyDrain;

                        // Chain Stratagem
                        if (IsEnabled(CustomComboPreset.SCH_DPS_ChainStrat) &&
                            ActionReady(ChainStratagem) && InCombat() &&
                            !TargetHasEffectAny(Debuffs.ChainStratagem) && //Overwrite protection
                            GetTargetHPPercent() > Config.SCH_ST_DPS_ChainStratagemOption &&
                            CanSpellWeave(actionID))
                            return ChainStratagem;

                        //Bio/Biolysis
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Bio) && LevelChecked(Bio) && InCombat())
                        {
                            uint dot = OriginalHook(Bio); //Grab the appropriate DoT Action
                            Status? dotDebuff = FindTargetEffect(BioList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                            if (dotDebuff is null || (dotDebuff.RemainingTime <= GetOptionFloat(Config.SCH_ST_DPS_Bio_Threshold) &&
                                (GetTargetHPPercent() > Config.SCH_ST_DPS_BioOption)))
                                return dot; //Use appropriate DoT Action

                            // DoT Uptime Timer
                            if ((dotDebuff is null) || (dotDebuff.RemainingTime <= GetOptionFloat(Config.SCH_ST_DPS_Bio_Threshold)))
                                return OriginalHook(dot);
                        }

                        //Ruin 2 Movement 
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Ruin2Movement) &&
                            LevelChecked(Ruin2) && InCombat() &&
                            IsMoving) return OriginalHook(Ruin2); //Who knows in the future

                        //AlterateMode idles as Ruin/Broil
                        if (AlternateMode && InCombat())
                            return OriginalHook(Ruin);
                    }
                }
                return actionID;
            }
        }

        /*
        * SCH_AoE
        * Overrides main AoE DPS ability, Art of War
        * Lucid Dreaming and Aetherflow weave options
       */
        internal class SCH_AoE : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArtOfWar or ArtOfWarII)
                {
                    // Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Aetherflow) &&
                        ActionReady(Aetherflow) && !Gauge.HasAetherflow() &&
                        InCombat() && CanSpellWeave(actionID))
                        return Aetherflow;

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.SCH_AoE_LucidOption &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;
                }
                return actionID;
            }
        }

        /*
        * SCH_Ruin2
        * Replaces Ruin II with Bio I/II for DoT Uptime
       */
        internal class SCH_Ruin2 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Ruin2;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ruin2 && LevelChecked(Bio))
                {
                    uint dot = OriginalHook(Bio); // Grab the appropriate DoT Action
                    Status? dotDebuff = FindTargetEffect(BioList[dot]); // Match it with it's Debuff ID, and check for the Debuff

                    if ((dotDebuff is null || dotDebuff?.RemainingTime <= Config.SCH_ST_DPS_EnergyDrain) &&
                        (GetTargetHPPercent() > Config.SCH_ST_DPS_BioOption))
                        return dot; // Use appropriate DoT Action
                }
                return actionID;
            }
        }
    }
}
