using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

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
            BanefulImpaction =  37012,

            // Faerie
            SummonSeraph = 16545,
            SummonEos = 17215,
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
            BroilList = [Ruin, Broil, Broil2, Broil3, Broil4],
            AetherflowList = [EnergyDrain, Lustrate, SacredSoil, Indomitability, Excogitation],
            FairyList = [WhisperingDawn, FeyBlessing, FeyIllumination, Dissipation, Aetherpact];

        internal static class Buffs
        {
            internal const ushort
                Galvanize = 297,
                SacredSoil = 299,
                Recitation = 1896,
                ImpactImminent = 3882;
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

        public static class Config
        {
            #region DPS
            public static UserInt
                SCH_ST_DPS_AltMode = new("SCH_ST_DPS_AltMode"),
                SCH_ST_DPS_LucidOption = new("SCH_ST_DPS_LucidOption", 6500),
                SCH_ST_DPS_BioOption = new("SCH_ST_DPS_BioOption", 10),
                SCH_ST_DPS_ChainStratagemOption = new("SCH_ST_DPS_ChainStratagemOption", 10);
            public static UserBool
                SCH_ST_DPS_Adv = new("SCH_ST_DPS_Adv"),
                SCH_ST_DPS_Bio_Adv = new("SCH_ST_DPS_Bio_Adv"),
                SCH_ST_DPS_EnergyDrain_Adv = new("SCH_ST_DPS_EnergyDrain_Adv");
            public static UserFloat
                SCH_ST_DPS_Bio_Threshold = new("SCH_ST_DPS_Bio_Threshold", 3.0f),
                SCH_ST_DPS_EnergyDrain = new("SCH_ST_DPS_EnergyDrain", 10.0f);
            public static UserBoolArray
                SCH_ST_DPS_Adv_Actions = new("SCH_ST_DPS_Adv_Actions");
            #endregion

            #region Healing
            public static UserInt
                SCH_AoE_LucidOption = new("SCH_AoE_LucidOption", 6500),
                SCH_AoE_Heal_Lucid = new("SCH_AoE_Heal_Lucid", 6500),
                SCH_ST_Heal_LucidOption = new("SCH_ST_Heal_LucidOption", 6500),
                SCH_ST_Heal_AdloquiumOption = new("SCH_ST_Heal_AdloquiumOption"),
                SCH_ST_Heal_LustrateOption = new("SCH_ST_Heal_LustrateOption"),
                SCH_ST_Heal_EsunaOption = new("SCH_ST_Heal_EsunaOption");
            public static UserBool
                SCH_ST_Heal_Adv = new("SCH_ST_Heal_Adv"),
                SCH_ST_Heal_UIMouseOver = new("SCH_ST_Heal_UIMouseOver"),
                SCH_DeploymentTactics_Adv = new ("SCH_DeploymentTactics_Adv"),
                SCH_DeploymentTactics_UIMouseOver = new ("SCH_DeploymentTactics_UIMouseOver");
            #endregion

            #region Utility
            internal static UserBool
                SCH_Aetherflow_Recite_Indom = new("SCH_Aetherflow_Recite_Indom"),
                SCH_Aetherflow_Recite_Excog = new("SCH_Aetherflow_Recite_Excog");
            internal static UserInt
                SCH_Aetherflow_Display = new("SCH_Aetherflow_Display"),
                SCH_Aetherflow_Recite_ExcogMode = new("SCH_Aetherflow_Recite_ExcogMode"),
                SCH_Aetherflow_Recite_IndomMode = new("SCH_Aetherflow_Recite_IndomMode"),
                SCH_Recitation_Mode = new("SCH_Recitation_Mode");
            #endregion

        }

        internal class SCH_DPS : CustomCombo
        {
            /*
 * SCH_DPS
 * Overrides main DPS ability family, The Broils (and Ruin 1)
 * Implements Ruin 2 as the movement option
 * Chain Stratagem has overlap protection
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DPS;

            internal OpenerState openerState = OpenerState.PreOpener;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool ActionFound;

                if (Config.SCH_ST_DPS_Adv && Config.SCH_ST_DPS_Adv_Actions.Count > 0)
                {
                    bool onBroils = Config.SCH_ST_DPS_Adv_Actions[0] && BroilList.Contains(actionID);
                    bool onBios = Config.SCH_ST_DPS_Adv_Actions[1] && BioList.ContainsKey(actionID);
                    bool onRuinII = Config.SCH_ST_DPS_Adv_Actions[2] && actionID is Ruin2;
                    ActionFound = onBroils || onBios || onRuinII;
                }
                else ActionFound = BroilList.Contains(actionID); //default handling

                if (ActionFound)
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

                    //Emergency Lucid
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.SCH_DPS_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

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
                        // Chain Stratagem
                        if (IsEnabled(CustomComboPreset.SCH_DPS_ChainStrat))
                        {
                            // If CS is available and usable, or if the Impact Buff is on Player
                            if (ActionReady(ChainStratagem) &&
                                !TargetHasEffectAny(Debuffs.ChainStratagem) &&
                                GetTargetHPPercent() > Config.SCH_ST_DPS_ChainStratagemOption &&
                                InCombat() &&
                                CanSpellWeave(actionID))
                                return ChainStratagem;

                            if (LevelChecked(BanefulImpaction) &&
                                HasEffect(Buffs.ImpactImminent) &&
                                InCombat() &&
                                CanSpellWeave(actionID))
                                return BanefulImpaction;
                            // Don't use OriginalHook(ChainStratagem), because player can disable ingame action replacement
                        }

                        // Energy Drain
                        if (IsEnabled(CustomComboPreset.SCH_DPS_EnergyDrain))
                        {
                            float edTime = Config.SCH_ST_DPS_EnergyDrain_Adv ? Config.SCH_ST_DPS_EnergyDrain : 10f;
                            if (LevelChecked(EnergyDrain) && InCombat() && Gauge.HasAetherflow()
                                && (GetCooldownRemainingTime(Aetherflow) <= edTime || (IsEnabled(CustomComboPreset.SCH_ST_DPS_ED_Dissipation) && GetCooldownRemainingTime(Dissipation) <= edTime))
                                && (!IsEnabled(CustomComboPreset.SCH_DPS_EnergyDrain_BurstSaver) || (LevelChecked(ChainStratagem) && GetCooldownRemainingTime(ChainStratagem) > 10) || !LevelChecked(ChainStratagem))
                                && CanSpellWeave(actionID))
                                return EnergyDrain;
                        }

                        //Bio/Biolysis
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Bio) && LevelChecked(Bio) && InCombat())
                        {
                            uint dot = OriginalHook(Bio); //Grab the appropriate DoT Action
                            Status? dotDebuff = FindTargetEffect(BioList[dot]); //Match it with it's Debuff ID, and check for the Debuff
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            float refreshtimer = Config.SCH_ST_DPS_Bio_Adv ? Config.SCH_ST_DPS_Bio_Threshold : 3;

                            if (IsEnabled(CustomComboPreset.SCH_DPS_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                                CanSpellWeave(actionID))
                                return Variant.VariantSpiritDart;

                            if ((dotDebuff is null || dotDebuff?.RemainingTime <= refreshtimer) &&
                                GetTargetHPPercent() > Config.SCH_ST_DPS_BioOption)
                                return dot; //Use appropriate DoT Action
                        }

                        //Ruin 2 Movement 
                        if (IsEnabled(CustomComboPreset.SCH_DPS_Ruin2Movement) &&
                            LevelChecked(Ruin2) &&
                            IsMoving) return OriginalHook(Ruin2);
                    }
                }
                return actionID;
            }
        }

        internal class SCH_AoE : CustomCombo
        {
            /*
* SCH_AoE
* Overrides main AoE DPS ability, Art of War
* Lucid Dreaming and Aetherflow weave options
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArtOfWar or ArtOfWarII)
                {
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.SCH_DPS_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                        HasBattleTarget() &&
                        CanSpellWeave(actionID))
                        return Variant.VariantSpiritDart;

                    //Emergency Lucid
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

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

        internal class SCH_ST_Heal : CustomCombo
        {
            /*
* SCH_ST_Heal
* Overrides main AoE Healing abiility, Succor
* Lucid Dreaming and Atherflow weave options
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_ST_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Physick)
                {
                    //Emergency Lucid
                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

                    // Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Aetherflow) &&
                        ActionReady(Aetherflow) && !Gauge.HasAetherflow() &&
                        InCombat() && CanSpellWeave(actionID))
                        return Aetherflow;

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.SCH_ST_Heal_LucidOption &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;

                    //Grab our target (Soft->Hard->Self)
                    IGameObject? healTarget = GetHealTarget(Config.SCH_ST_Heal_Adv && Config.SCH_ST_Heal_UIMouseOver);

                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Esuna) && ActionReady(All.Esuna) &&
                        GetTargetHPPercent(healTarget) >= Config.SCH_ST_Heal_EsunaOption &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;

                    //Check for the Galvanize shield buff. Start applying if it doesn't exist or Target HP is below %
                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Adloquium) &&
                        ActionReady(Adloquium) &&
                        (FindEffectOnMember(Buffs.Galvanize, healTarget) is null || GetTargetHPPercent(healTarget) <= Config.SCH_ST_Heal_AdloquiumOption))
                    {
                        return OriginalHook(Adloquium);
                    }

                    //Cast Lustrate if you have Aetherflow and Target HP is below %
                    if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Lustrate) &&
                        ActionReady(Lustrate) &&
                        Gauge.HasAetherflow() &&
                        GetTargetHPPercent(healTarget) <= Config.SCH_ST_Heal_LustrateOption)
                    {
                        return Lustrate;
                    }
                }
                return actionID;
            }
        }

        internal class SCH_AoE_Heal : CustomCombo
        {
            /*
* SCH_AoE_Heal
* Overrides main AoE Healing abiility, Succor
* Lucid Dreaming and Atherflow weave options
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Succor)
                {
                    //Emergency Lucid
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

                    // Aetherflow
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Aetherflow) &&
                        ActionReady(Aetherflow) && !Gauge.HasAetherflow() &&
                        !(IsEnabled(CustomComboPreset.SCH_AoE_Heal_Aetherflow_Indomitability) && GetCooldownRemainingTime(Indomitability) <= 0.6f) &&
                            InCombat())
                        return Aetherflow;

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp < Config.SCH_AoE_Heal_Lucid
                        && CanSpellWeave(actionID))
                        return All.LucidDreaming;

                    // Indomitability
                    if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Indomitability) &&
                        ActionReady(Indomitability) &&
                        Gauge.HasAetherflow())
                        return Indomitability;
                }
                return actionID;
            }
        }

        internal class SCH_Consolation : CustomCombo
        {
            /*
 * SCH_Consolation
 * Even though Summon Seraph becomes Consolation, 
 * This Feature also places Seraph's AoE heal+barrier ontop of the existing fairy AoE skill, Fey Blessing
 */
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Consolation;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is FeyBlessing && LevelChecked(SummonSeraph) && Gauge.SeraphTimer > 0 ? Consolation : actionID;
        }

        internal class SCH_Lustrate : CustomCombo
        {
            /*
 * SCH_Lustrate
 * Replaces Lustrate with Excogitation when Excogitation is ready.
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Lustrate;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Lustrate && LevelChecked(Excogitation) && IsOffCooldown(Excogitation) ? Excogitation : actionID;
        }

        internal class SCH_Recitation : CustomCombo
        {
            /*
 * SCH_Recitation
 * Replaces Recitation with selected one of its combo skills.
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Recitation;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Recitation && HasEffect(Buffs.Recitation))
                {
                    switch ((int)Config.SCH_Recitation_Mode)
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

        internal class SCH_Aetherflow : CustomCombo
        {
            /*
 * SCH_Aetherflow
 * Replaces all Energy Drain actions with Aetherflow when depleted, or just Energy Drain
 * Dissipation option to show if Aetherflow is on Cooldown
 * Recitation also an option
*/
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
                        bool AlwaysShowReciteExcog = (Config.SCH_Aetherflow_Recite_ExcogMode == 1);
                        if (Config.SCH_Aetherflow_Recite_Excog &&
                            (AlwaysShowReciteExcog || (!AlwaysShowReciteExcog && !HasAetherFlows)) &&
                            actionID is Excogitation)
                        {   //Do not merge this nested if with above. Won't procede with next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Excogitation : Recitation;
                        }

                        bool AlwaysShowReciteIndom = (Config.SCH_Aetherflow_Recite_IndomMode == 1);
                        if (Config.SCH_Aetherflow_Recite_Indom &&
                            (AlwaysShowReciteIndom || (!AlwaysShowReciteIndom && !HasAetherFlows)) &&
                            actionID is Indomitability)
                        {   //Same as above, do not nest with above. It won't procede with the next set
                            return HasEffect(Buffs.Recitation) && IsOffCooldown(Excogitation) ? Indomitability : Recitation;
                        }
                    }
                    if (!HasAetherFlows)
                    {
                        bool ShowAetherflowOnAll = (Config.SCH_Aetherflow_Display == 1);
                        if (((actionID is EnergyDrain && !ShowAetherflowOnAll) || ShowAetherflowOnAll) &&
                            IsOffCooldown(actionID))
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

        internal class SCH_Raise : CustomCombo
        {
            /*
 * SCH_Raise (Swiftcast Raise combo)
 * Swiftcast changes to Raise when swiftcast is on cooldown
 */
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Resurrection : actionID;
        }

        internal class SCH_FairyReminder : CustomCombo
        {
            // Replaces Fairy abilities with Fairy summoning with Eos
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_FairyReminder;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => FairyList.Contains(actionID) && !HasPetPresent() && Gauge.SeraphTimer == 0 ? SummonEos : actionID;
        }

        internal class SCH_Aetherflow_Dissipation_Drain : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Dissipation &&
                Gauge.HasAetherflow() &&
                IsEnabled(CustomComboPreset.SCH_Aetherflow_Dissipation_Drain) &&
                IsOffCooldown(Dissipation) ? EnergyDrain : actionID;
        }

        internal class SCH_DeploymentTactics : CustomCombo
        {
            /*
 * SCH_DeploymentTactics
 * Combos Deployment Tactics with Adloquium by showing Adloquim when Deployment Tactics is ready,
 * Recitation is optional, if one wishes to Crit the shield first
 * Supports soft targetting and self as a fallback.
 */
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DeploymentTactics;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DeploymentTactics && ActionReady(DeploymentTactics))
                {
                    //Grab our target (Soft->Hard->Self)
                    IGameObject? healTarget = GetHealTarget(Config.SCH_DeploymentTactics_Adv && Config.SCH_DeploymentTactics_UIMouseOver);

                    //Check for the Galvanize shield buff. Start applying if it doesn't exist
                    if (FindEffect(Buffs.Galvanize, healTarget, LocalPlayer.GameObjectId) is null)
                    {
                        if (IsEnabled(CustomComboPreset.SCH_DeploymentTactics_Recitation) && ActionReady(Recitation))
                            return Recitation;

                        return OriginalHook(Adloquium);
                    }
                }
                return actionID;
            }
        }

        internal class SCH_Fairy_Combo : CustomCombo
        {
            /*
* SCH_Fairy_Combo
* Overrides Whispering Dawn
*/
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Fairy_Combo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is WhisperingDawn)
                {

                    // FeyIllumination
                    if (ActionReady(FeyIllumination))
                        return OriginalHook(FeyIllumination);

                    // FeyBlessing
                    if (ActionReady(FeyBlessing) && !(Gauge.SeraphTimer > 0))
                        return OriginalHook(FeyBlessing);

                    if (IsEnabled(CustomComboPreset.SCH_Fairy_Combo_Consolation) && ActionReady(WhisperingDawn))
                        return OriginalHook(actionID);

                    if (IsEnabled(CustomComboPreset.SCH_Fairy_Combo_Consolation) && Gauge.SeraphTimer > 0 && GetRemainingCharges(Consolation) > 0)
                    return OriginalHook(Consolation);
                }
                return actionID;
            }
        }
        
    }
}