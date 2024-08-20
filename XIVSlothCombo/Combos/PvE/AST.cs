using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class AST
    {
        internal const byte JobID = 33;

        internal const uint
            //DPS
            Malefic = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Malefic4 = 16555,
            FallMalefic = 25871,
            Gravity = 3615,
            Gravity2 = 25872,
            Oracle = 37029,

            //Cards
            AstralDraw = 37017,
            Play1 = 37019,
            Play2 = 37020,
            Play3 = 37021,
            Arrow = 37024,
            Balance = 37023,
            Bole = 37027,
            Ewer = 37028,
            Spear = 37026,
            Spire = 37025,
            MinorArcana = 37022,
            //LordOfCrowns = 7444,
            //LadyOfCrown = 7445,

            //Utility
            Divination = 16552,
            Lightspeed = 3606,

            //DoT
            Combust = 3599,
            Combust2 = 3608,
            Combust3 = 16554,

            //Healing
            Benefic = 3594,
            Benefic2 = 3610,
            AspectedBenefic = 3595,
            Helios = 3600,
            AspectedHelios = 3601,
            HeliosConjuction = 37030,
            Ascend = 3603,
            EssentialDignity = 3614,
            CelestialOpposition = 16553,
            CelestialIntersection = 16556,
            Horoscope = 16557,
            Exaltation = 25873,
            Macrocosmos = 25874,
            Synastry = 3612,
            CollectiveUnconscious = 3613;

        //Action Groups
        internal static readonly List<uint>
            MaleficList = [Malefic, Malefic2, Malefic3, Malefic4, FallMalefic],
            GravityList = [Gravity, Gravity2];

        internal static class Buffs
        {
            internal const ushort
                AspectedBenefic = 835,
                AspectedHelios = 836,
                HeliosConjunction = 3894,
                Horoscope = 1890,
                HoroscopeHelios = 1891,
                NeutralSect = 1892,
                NeutralSectShield = 1921,
                Divination = 1878,
                LordOfCrownsDrawn = 2054,
                LadyOfCrownsDrawn = 2055,
                ClarifyingDraw = 2713,
                Macrocosmos = 2718,
                //The "Buff" that shows when you're holding onto the card
                BalanceDrawn = 913,
                BoleDrawn = 914,
                ArrowDrawn = 915,
                SpearDrawn = 916,
                EwerDrawn = 917,
                SpireDrawn = 918,
                //The actual buff that buffs players
                BalanceBuff = 3887,
                BoleBuff = 3890,
                ArrowBuff = 3888,
                SpearBuff = 3889,
                EwerBuff = 3891,
                SpireBuff = 3892,
                Lightspeed = 841,
                SelfSynastry = 845,
                TargetSynastry = 846,
                Divining = 3893;
        }

        internal static class Debuffs
        {
            internal const ushort
                Combust = 838,
                Combust2 = 843,
                Combust3 = 1881;
        }

        //Debuff Pairs of Actions and Debuff
        internal static Dictionary<uint, ushort>
            CombustList = new() {
                { Combust,  Debuffs.Combust  },
                { Combust2, Debuffs.Combust2 },
                { Combust3, Debuffs.Combust3 }
            };

        public static ASTGauge Gauge => CustomComboFunctions.GetJobGauge<ASTGauge>();

        public static CardType DrawnCard { get; set; }

        public static class Config
        {
            public static UserInt
                AST_LucidDreaming = new("ASTLucidDreamingFeature", 8000),
                AST_EssentialDignity = new("ASTCustomEssentialDignity", 50),
                AST_Spire = new("AST_Spire", 80),
                AST_Ewer = new("AST_Ewer", 80),
                AST_Arrow = new("AST_Arrow", 80),
                AST_Bole = new("AST_Bole", 80),
                AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna", 100),
                AST_DPS_AltMode = new("AST_DPS_AltMode"),
                AST_AoEHeals_AltMode = new("AST_AoEHeals_AltMode"),
                AST_DPS_DivinationOption = new("AST_DPS_DivinationOption"),
                AST_AOE_DivinationOption = new("AST_AOE_DivinationOption"),
                AST_DPS_LightSpeedOption = new("AST_DPS_LightSpeedOption"),
                AST_AOE_LightSpeedOption = new("AST_AOE_LightSpeedOption"),
                AST_DPS_CombustOption = new("AST_DPS_CombustOption"),
                AST_QuickTarget_Override = new("AST_QuickTarget_Override"),
                AST_ST_DPS_Play_SpeedSetting = new("AST_ST_DPS_Play_SpeedSetting");                
            public static UserBool
                AST_QuickTarget_SkipDamageDown = new("AST_QuickTarget_SkipDamageDown"),
                AST_QuickTarget_SkipRezWeakness = new("AST_QuickTarget_SkipRezWeakness"),
                AST_ST_SimpleHeals_Adv = new("AST_ST_SimpleHeals_Adv"),
                AST_ST_SimpleHeals_UIMouseOver = new("AST_ST_SimpleHeals_UIMouseOver"),
                AST_ST_SimpleHeals_WeaveDignity = new("AST_ST_SimpleHeals_WeaveDignity"),
                AST_ST_SimpleHeals_WeaveIntersection = new("AST_ST_SimpleHeals_WeaveIntersection"),
                AST_ST_SimpleHeals_WeaveEwer = new("AST_ST_SimpleHeals_WeaveEwer"),
                AST_ST_SimpleHeals_WeaveSpire = new("AST_ST_SimpleHeals_WeaveSpire"),
                AST_ST_SimpleHeals_WeaveArrow = new("AST_ST_SimpleHeals_WeaveArrow"),
                AST_ST_SimpleHeals_WeaveBole = new("AST_ST_SimpleHeals_WeaveBole"),
                AST_ST_SimpleHeals_WeaveExalt = new("AST_ST_SimpleHeals_WeaveExalt"),
                AST_AoE_SimpleHeals_WeaveLady = new("AST_AoE_SimpleHeals_WeaveLady"),
                AST_AoE_SimpleHeals_Opposition = new("AST_AoE_SimpleHeals_Opposition"),
                AST_AoE_SimpleHeals_Horoscope = new("AST_AoE_SimpleHeals_Horoscope"),
                AST_ST_DPS_OverwriteCards = new("AST_ST_DPS_OverwriteCards"),
                AST_AOE_DPS_OverwriteCards = new("AST_AOE_DPS_OverwriteCards"),
                AST_ST_DPS_CombustUptime_Adv = new("AST_ST_DPS_CombustUptime_Adv");
            public static UserFloat
                AST_ST_DPS_CombustUptime_Threshold = new("AST_ST_DPS_CombustUptime_Threshold");
        }
        

        internal class AST_Benefic : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Benefic;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Benefic2 && !ActionReady(Benefic2) ? Benefic : actionID;
        }

        internal class AST_Raise_Alternative : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Raise_Alternative;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Ascend : actionID;
        }

        internal class AST_ST_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;
            internal static int MaleficCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(Malefic));
            internal static int CombustCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(Combust));

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int spellsSinceDraw = ActionWatching.CombatActions.Any(x => x == OriginalHook(AstralDraw)) ? ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Malefic), OriginalHook(AstralDraw)) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Combust), OriginalHook(AstralDraw)) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Gravity), OriginalHook(AstralDraw)) : Config.AST_ST_DPS_Play_SpeedSetting;

                if (spellsSinceDraw == 0 && DrawnCard != CardType.NONE)
                {
                    spellsSinceDraw = 1;
                }
                
                bool AlternateMode = GetIntOptionAsBool(Config.AST_DPS_AltMode); //(0 or 1 radio values)
                bool inOpener = IsEnabled(CustomComboPreset.AST_ST_DPS_Opener) && MaleficCount < 6;

                // Out of combat Card Draw
                if (((!AlternateMode && MaleficList.Contains(actionID)) ||
                    (AlternateMode && CombustList.ContainsKey(actionID)) &&
                    !InCombat()))

                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                        ActionReady(OriginalHook(AstralDraw)) && (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_ST_DPS_OverwriteCards)))
                        return OriginalHook(AstralDraw);

                //In combat
                if (((!AlternateMode && MaleficList.Contains(actionID)) ||
                     (AlternateMode && CombustList.ContainsKey(actionID))) &&
                     InCombat())
                {
                    //Variant stuff
                    if (IsEnabled(CustomComboPreset.AST_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantSpiritDart;

                    //Opener
                    if (inOpener)
                    {
                        if (MaleficCount == 0)
                            return OriginalHook(Malefic);

                        if (MaleficCount == 1 && CombustCount == 0)
                            return OriginalHook(Combust);

                        if (MaleficCount == 1 && (CombustCount == 1) && ActionReady(Lightspeed) && CanDelayedWeave(actionID))
                            return OriginalHook(Lightspeed);

                        if (MaleficCount == 3 && CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                            ActionReady(Divination) &&
                            !HasEffectAny(Buffs.Divination) && //Overwrite protection
                            GetTargetHPPercent() > Config.AST_DPS_DivinationOption &&
                            CanWeave(actionID))
                                return Divination;

                            if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                            ActionReady(Play1) &&
                            Gauge.DrawnCards[0] is not CardType.NONE)
                                return OriginalHook(Play1);
                        }

                        if (MaleficCount == 4 && CanWeave(actionID))
                        {
                            if (ActionReady(OriginalHook(MinorArcana)) &&
                            IsEnabled(CustomComboPreset.AST_DPS_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD &&
                            HasBattleTarget())
                                return OriginalHook(MinorArcana);

                            if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) && Gauge.DrawnCrownCard is not CardType.LORD && CanDelayedWeave(actionID))
                                return OriginalHook(AstralDraw);
                        }
                        if (MaleficCount == 5 && CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                            ActionReady(Play1) &&
                            Gauge.DrawnCards[0] is not CardType.NONE)
                                return OriginalHook(Play1);

                            if (IsEnabled(CustomComboPreset.AST_DPS_Oracle) &&
                            HasEffect(Buffs.Divining) &&
                            CanSpellWeave(actionID))
                                return Oracle;
                        }

                        if (MaleficCount > 0)
                            return OriginalHook(Malefic);
                    }

                    //End opener

                    if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                        ActionReady(Lightspeed) &&
                        GetTargetHPPercent() > Config.AST_DPS_LightSpeedOption &&
                        IsMoving &&
                        !HasEffect(Buffs.Lightspeed))
                        return Lightspeed;



                    if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.AST_LucidDreaming &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;


                    //Play Card
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                        ActionReady(Play1) &&
                        Gauge.DrawnCards[0] is not CardType.NONE &&
                        CanSpellWeave(actionID) &&
                        spellsSinceDraw >= Config.AST_ST_DPS_Play_SpeedSetting)
                        return OriginalHook(Play1);

                    //Card Draw
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                        ActionReady(OriginalHook(AstralDraw)) &&
                        (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_ST_DPS_OverwriteCards)) &&
                        CanDelayedWeave(actionID))
                        return OriginalHook(AstralDraw);

                    //Divination
                    if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                        ActionReady(Divination) &&
                        !HasEffectAny(Buffs.Divination) && //Overwrite protection
                        GetTargetHPPercent() > Config.AST_DPS_DivinationOption &&
                        CanDelayedWeave(actionID) &&
                        ActionWatching.NumberOfGcdsUsed >= 3)
                        return Divination;

                    if (IsEnabled(CustomComboPreset.AST_DPS_Oracle) &&
                        HasEffect(Buffs.Divining) &&
                        CanSpellWeave(actionID))
                        return Oracle;

                    //Minor Arcana / Lord of Crowns
                    if (ActionReady(OriginalHook(MinorArcana)) &&
                        IsEnabled(CustomComboPreset.AST_DPS_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD &&
                        HasBattleTarget() &&
                        CanDelayedWeave(actionID))
                        return OriginalHook(MinorArcana);

                    if (HasBattleTarget())
                    {
                        //Combust
                        if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                            !GravityList.Contains(actionID) &&
                            LevelChecked(Combust) &&
                            CombustList.TryGetValue(OriginalHook(Combust), out ushort dotDebuffID))
                        {
                            if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                                CanSpellWeave(actionID))
                                return Variant.VariantSpiritDart;
                            
                            float refreshtimer = Config.AST_ST_DPS_CombustUptime_Adv ? Config.AST_ST_DPS_CombustUptime_Threshold : 3;
                            if (GetDebuffRemainingTime(dotDebuffID) <= refreshtimer &&
                                GetTargetHPPercent() > Config.AST_DPS_CombustOption)
                                return OriginalHook(Combust);

                            //AlterateMode idles as Malefic
                            if (AlternateMode) return OriginalHook(Malefic);
                        }
                    }
                }
                return actionID;
            }
        }
        internal class AST_AOE_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int spellsSinceDraw = ActionWatching.CombatActions.Any(x => x == OriginalHook(AstralDraw)) ? ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Malefic), OriginalHook(AstralDraw)) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Combust), OriginalHook(AstralDraw)) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Gravity), OriginalHook(AstralDraw)) : Config.AST_ST_DPS_Play_SpeedSetting;

                if (spellsSinceDraw == 0 && DrawnCard != CardType.NONE)
                {
                    spellsSinceDraw = 1;
                }

                bool AlternateMode = GetIntOptionAsBool(Config.AST_DPS_AltMode); //(0 or 1 radio values)



                if (IsEnabled(CustomComboPreset.AST_AOE_DPS) && GravityList.Contains(actionID) &&
                InCombat())
                {
                    //Variant stuff
                    if (IsEnabled(CustomComboPreset.AST_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                        CanSpellWeave(actionID) &&
                        IsEnabled(CustomComboPreset.AST_AOE_DPS) && GravityList.Contains(actionID))
                        return Variant.VariantSpiritDart;                                   

                    if (IsEnabled(CustomComboPreset.AST_AOE_LightSpeed) &&
                        ActionReady(Lightspeed) &&
                        GetTargetHPPercent() > Config.AST_AOE_LightSpeedOption &&
                        IsMoving &&
                        !HasEffect(Buffs.Lightspeed))
                        return Lightspeed;

                    if (IsEnabled(CustomComboPreset.AST_AOE_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.AST_LucidDreaming &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;

                    //Play Card
                    if (IsEnabled(CustomComboPreset.AST_AOE_AutoPlay) &&
                        ActionReady(Play1) &&
                        Gauge.DrawnCards[0] is not CardType.NONE &&
                        CanSpellWeave(actionID))
                        return OriginalHook(Play1);

                    //Card Draw
                    if (IsEnabled(CustomComboPreset.AST_AOE_AutoDraw) &&
                        ActionReady(OriginalHook(AstralDraw)) &&
                        (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_AOE_DPS_OverwriteCards)) &&
                        CanDelayedWeave(actionID))
                        return OriginalHook(AstralDraw);

                    //Divination
                    if (IsEnabled(CustomComboPreset.AST_AOE_Divination) &&
                        ActionReady(Divination) &&
                        !HasEffectAny(Buffs.Divination) && //Overwrite protection
                        GetTargetHPPercent() > Config.AST_AOE_DivinationOption &&
                        CanDelayedWeave(actionID) &&
                        ActionWatching.NumberOfGcdsUsed >= 3)
                        return Divination;

                    if (IsEnabled(CustomComboPreset.AST_AOE_Oracle) &&
                        HasEffect(Buffs.Divining) &&
                        CanSpellWeave(actionID))
                        return Oracle;

                    //Minor Arcana / Lord of Crowns
                    if (ActionReady(OriginalHook(MinorArcana)) &&
                        IsEnabled(CustomComboPreset.AST_AOE_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD &&
                        HasBattleTarget() &&
                        CanDelayedWeave(actionID))
                        return OriginalHook(MinorArcana);                 
                                            
                }
                return actionID;
            }
        }
        internal class AST_AoE_SimpleHeals_AspectedHelios : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AoE_SimpleHeals_AspectedHelios;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool NonaspectedMode = GetIntOptionAsBool(Config.AST_AoEHeals_AltMode); //(0 or 1 radio values)

                if (NonaspectedMode && actionID is Helios || !NonaspectedMode && actionID is AspectedHelios or HeliosConjuction)

                
                {
                    var canLady = (Config.AST_AoE_SimpleHeals_WeaveLady && CanSpellWeave(actionID)) || !Config.AST_AoE_SimpleHeals_WeaveLady;
                    var canHoroscope = (Config.AST_AoE_SimpleHeals_Horoscope && CanSpellWeave(actionID)) || !Config.AST_AoE_SimpleHeals_Horoscope;
                    var canOppose = (Config.AST_AoE_SimpleHeals_Opposition && CanSpellWeave(actionID)) || !Config.AST_AoE_SimpleHeals_Opposition;

                    if (!LevelChecked(AspectedHelios)) //Level check to return helios immediately below 40
                        return Helios;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_LazyLady) &&
                        ActionReady(MinorArcana) &&
                        Gauge.DrawnCrownCard is CardType.LADY
                        && canLady)
                        return OriginalHook(MinorArcana);

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                        ActionReady(CelestialOpposition) &&
                        canOppose)
                        return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
                    {
                        if (ActionReady(Horoscope) &&
                            canHoroscope)
                            return Horoscope;                                               

                        if (HasEffect(Buffs.HoroscopeHelios) &&
                            canHoroscope)
                            return OriginalHook(Horoscope);
                    }

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Aspected) && NonaspectedMode) // So helios mode can run aspected option 
                    {
                        if ((ActionReady(AspectedHelios)
                                 && !HasEffect(Buffs.AspectedHelios)
                                 && !HasEffect(Buffs.HeliosConjunction))
                             || HasEffect(Buffs.Horoscope)
                             || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                            return OriginalHook(AspectedHelios);
                    }

                    if (!NonaspectedMode) //So aspected mode runs normal without having to select it from the option as well
                    {
                        if ((ActionReady(AspectedHelios)
                                 && !HasEffect(Buffs.AspectedHelios)
                                 && !HasEffect(Buffs.HeliosConjunction))
                             || HasEffect(Buffs.Horoscope)
                             || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                            return OriginalHook(AspectedHelios);
                    }

                    if ((HasEffect(Buffs.AspectedHelios)
                         || HasEffect(Buffs.HeliosConjunction))
                        && (FindEffect(Buffs.AspectedHelios)?.RemainingTime > 2
                            || FindEffect(Buffs.HeliosConjunction)?.RemainingTime > 2))
                        return Helios;
                }

                return actionID;
            }
        }


        internal class AST_ST_SimpleHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_SimpleHeals;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Benefic2)
                {
                    var canDignity = (Config.AST_ST_SimpleHeals_WeaveDignity && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveDignity;
                    var canIntersect = (Config.AST_ST_SimpleHeals_WeaveIntersection && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveIntersection;
                    var canExalt = (Config.AST_ST_SimpleHeals_WeaveExalt && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveExalt;
                    var canEwer = (Config.AST_ST_SimpleHeals_WeaveEwer && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveEwer;
                    var canSpire = (Config.AST_ST_SimpleHeals_WeaveSpire && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveSpire;
                    var canBole = (Config.AST_ST_SimpleHeals_WeaveBole && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveBole;
                    var canArrow = (Config.AST_ST_SimpleHeals_WeaveArrow && CanSpellWeave(actionID)) || !Config.AST_ST_SimpleHeals_WeaveArrow;

                    //Grab our target (Soft->Hard->Self)
                    IGameObject? healTarget = GetHealTarget(Config.AST_ST_SimpleHeals_Adv && Config.AST_ST_SimpleHeals_UIMouseOver);

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Esuna) && ActionReady(All.Esuna) &&
                        GetTargetHPPercent(healTarget) >= Config.AST_ST_SimpleHeals_Esuna &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Spire) &&
                        Gauge.DrawnCards[2] == CardType.SPIRE &&
                        GetTargetHPPercent(healTarget) <= Config.AST_Spire &&
                        canSpire)
                        return OriginalHook(Play3);

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Ewer) &&
                        Gauge.DrawnCards[2] == CardType.EWER &&
                        GetTargetHPPercent(healTarget) <= Config.AST_Ewer &&
                        canEwer)
                        return OriginalHook(Play3);

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Arrow) &&
                        Gauge.DrawnCards[1] == CardType.ARROW &&
                        GetTargetHPPercent(healTarget) <= Config.AST_Arrow &&
                        canArrow)
                        return OriginalHook(Play2);
                    
                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Bole) &&
                        Gauge.DrawnCards[1] == CardType.BOLE &&
                        GetTargetHPPercent(healTarget) <= Config.AST_Bole &&
                        canBole)
                        return OriginalHook(Play2);

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                        ActionReady(EssentialDignity) &&
                        GetTargetHPPercent(healTarget) <= Config.AST_EssentialDignity &&
                        canDignity)
                        return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                        ActionReady(Exaltation) &&
                        canExalt)
                        return Exaltation;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                        ActionReady(CelestialIntersection) &&
                        canIntersect &&
                        !(healTarget as IBattleChara)!.HasShield())
                        return CelestialIntersection;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && ActionReady(AspectedBenefic))
                    {
                        Status? aspectedBeneficHoT = FindEffect(Buffs.AspectedBenefic, healTarget, LocalPlayer?.GameObjectId);
                        Status? NeutralSectShield = FindEffect(Buffs.NeutralSectShield, healTarget, LocalPlayer?.GameObjectId);
                        Status? NeutralSectBuff = FindEffect(Buffs.NeutralSect, healTarget, LocalPlayer?.GameObjectId);
                        if ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3)
                            || ((NeutralSectShield is null) && (NeutralSectBuff is not null)))
                            return AspectedBenefic;
                    }
                }
                return actionID;
            }
        }
    }
}