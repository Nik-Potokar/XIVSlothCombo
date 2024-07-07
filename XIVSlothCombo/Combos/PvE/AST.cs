using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
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

            //Cards
            Draw = 3590,
            Play = 17055,
            Redraw = 3593,
            //Obsolete? Left just incase it's needed
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            MinorArcana = 7443,
            //LordOfCrowns = 7444,
            //LadyOfCrown = 7445,
            Astrodyne = 25870,

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
            Ascend = 3603,
            EssentialDignity = 3614,
            CelestialOpposition = 16553,
            CelestialIntersection = 16556,
            Horoscope = 16557,
            Exaltation = 25873,
            Macrocosmos = 25874,
            Synastry = 3612;

        //Action Groups
        internal static readonly List<uint>
            MaleficList = [Malefic, Malefic2, Malefic3, Malefic4, FallMalefic],
            GravityList = [Gravity, Gravity2];

        internal static class Buffs
        {
            internal const ushort
                AspectedBenefic = 835,
                AspectedHelios = 836,
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
                BalanceDamage = 1882,
                BoleDamage = 1883,
                ArrowDamage = 1884,
                SpearDamage = 1885,
                EwerDamage = 1886,
                SpireDamage = 1887,
                Lightspeed = 841,
                SelfSynastry = 845,
                TargetSynastry = 846;
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
                AST_LucidDreaming = new("ASTLucidDreamingFeature"),
                AST_EssentialDignity = new("ASTCustomEssentialDignity"),
                AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna"),
                AST_DPS_AltMode = new("AST_DPS_AltMode"),
                AST_DPS_DivinationOption = new("AST_DPS_DivinationOption"),
                AST_DPS_LightSpeedOption = new("AST_DPS_LightSpeedOption"),
                AST_DPS_CombustOption = new("AST_DPS_CombustOption"),
                AST_QuickTarget_Override = new("AST_QuickTarget_Override"),
                AST_ST_DPS_Play_SpeedSetting = new("AST_ST_DPS_Play_SpeedSetting");
            public static UserBool
                AST_QuickTarget_SkipDamageDown = new("AST_QuickTarget_SkipDamageDown"),
                AST_QuickTarget_SkipRezWeakness = new("AST_QuickTarget_SkipRezWeakness"),
                AST_ST_SimpleHeals_Adv = new("AST_ST_SimpleHeals_Adv"),
                AST_ST_SimpleHeals_UIMouseOver = new("AST_ST_SimpleHeals_UIMouseOver"),
                AST_ST_DPS_CombustUptime_Adv = new("AST_ST_DPS_CombustUptime_Adv");
            public static UserFloat
                AST_ST_DPS_CombustUptime_Threshold = new("AST_ST_DPS_CombustUptime_Threshold");
        }

        internal class AST_Cards_DrawOnPlay : CustomCombo
        {

            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_DrawOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Play)
                {
                    var haveCard = HasEffect(Buffs.BalanceDrawn) || HasEffect(Buffs.BoleDrawn) || HasEffect(Buffs.ArrowDrawn) || HasEffect(Buffs.SpearDrawn) || HasEffect(Buffs.EwerDrawn) || HasEffect(Buffs.SpireDrawn);
                    var cardDrawn = Gauge.DrawnCards[0];


                    if (haveCard)
                    {
                        return OriginalHook(Play);
                    }

                    return OriginalHook(Draw);
                }

                return actionID;
            }
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

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                int spellsSinceDraw = ActionWatching.CombatActions.Any(x => x == Draw) ? ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Malefic), Draw) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Combust), Draw) +
                    ActionWatching.HowManyTimesUsedAfterAnotherAction(OriginalHook(Gravity), Draw) : Config.AST_ST_DPS_Play_SpeedSetting;

                if (spellsSinceDraw == 0 && DrawnCard != CardType.NONE)
                {
                    spellsSinceDraw = 1;
                }

                //Dalamud.Logging.PluginLog.Debug($"{spellsSinceDraw}");
                bool AlternateMode = GetIntOptionAsBool(Config.AST_DPS_AltMode); //(0 or 1 radio values)
                if (((!AlternateMode && MaleficList.Contains(actionID)) ||
                     (AlternateMode && CombustList.ContainsKey(actionID)) ||
                     (IsEnabled(CustomComboPreset.AST_AoE_DPS) && GravityList.Contains(actionID))) &&
                    InCombat())
                {

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
                        IsEnabled(CustomComboPreset.AST_AoE_DPS) && GravityList.Contains(actionID))
                        return Variant.VariantSpiritDart;

                    if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                        ActionReady(Lightspeed) &&
                        GetTargetHPPercent() > Config.AST_DPS_LightSpeedOption &&
                        CanSpellWeave(actionID))
                        return Lightspeed;

                    if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                        ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.AST_LucidDreaming &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;


                    //Play Card
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                        ActionReady(Play) &&
                        Gauge.DrawnCards[0] is not CardType.NONE &&
                        CanSpellWeave(actionID) &&
                        spellsSinceDraw >= Config.AST_ST_DPS_Play_SpeedSetting &&
                        !WasLastAction(Redraw))
                        return OriginalHook(Play);

                    //Card Draw
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                        ActionReady(Draw) &&
                        Gauge.DrawnCards[0] is CardType.NONE &&
                        CanDelayedWeave(actionID))
                        return Draw;

                    //Divination
                    if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                        ActionReady(Divination) &&
                        !HasEffectAny(Buffs.Divination) && //Overwrite protection
                        GetTargetHPPercent() > Config.AST_DPS_DivinationOption &&
                        CanDelayedWeave(actionID) &&
                        ActionWatching.NumberOfGcdsUsed >= 3)
                        return Divination;

                    //Minor Arcana / Lord of Crowns
                    if (ActionReady(OriginalHook(MinorArcana)) &&
                        ((IsEnabled(CustomComboPreset.AST_DPS_AutoCrownDraw) && Gauge.DrawnCrownCard is CardType.NONE) ||
                        (IsEnabled(CustomComboPreset.AST_DPS_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD && HasBattleTarget())) &&
                        CanDelayedWeave(actionID))
                        return OriginalHook(MinorArcana);

                    if (HasBattleTarget())
                    {
                        //Combust
                        if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                            !GravityList.Contains(actionID) &&
                            LevelChecked(Combust))
                        {
                            //Grab current DoT via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                            uint dot = OriginalHook(Combust);
                            Status? dotDebuff = FindTargetEffect(CombustList[dot]);
                            float refreshtimer = Config.AST_ST_DPS_CombustUptime_Adv ? Config.AST_ST_DPS_CombustUptime_Threshold : 3;

                            if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                                CanSpellWeave(actionID))
                                return Variant.VariantSpiritDart;


                            if ((dotDebuff is null || dotDebuff.RemainingTime <= refreshtimer) &&
                                GetTargetHPPercent() > Config.AST_DPS_CombustOption)
                                return dot;

                            //AlterateMode idles as Malefic
                            if (AlternateMode) return OriginalHook(Malefic);
                        }
                    }
                }
                return actionID;
            }
        }

        internal class AST_AoE_SimpleHeals_AspectedHelios : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AoE_SimpleHeals_AspectedHelios;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is AspectedHelios)
                {
                    //Level check to exit if we can't use
                    if (!LevelChecked(AspectedHelios))
                        return Helios;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_LazyLady) &&
                        ActionReady(MinorArcana) &&
                        InCombat() &&
                        Gauge.DrawnCrownCard is CardType.LADY
                        && CanSpellWeave(actionID))
                        return OriginalHook(MinorArcana);

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                        ActionReady(CelestialOpposition) &&
                        CanWeave(actionID))
                        return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
                    {
                        if (ActionReady(Horoscope) &&
                            CanSpellWeave(actionID))
                            return Horoscope;

                        if ((ActionReady(AspectedHelios) && !HasEffect(Buffs.AspectedHelios))
                             || HasEffect(Buffs.Horoscope)
                             || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                            return AspectedHelios;

                        if (HasEffect(Buffs.HoroscopeHelios) &&
                            CanSpellWeave(actionID))
                            return OriginalHook(Horoscope);
                    }

                    if (HasEffect(Buffs.AspectedHelios) && FindEffect(Buffs.AspectedHelios).RemainingTime > 2)
                        return Helios;
                }

                return actionID;
            }
        }


        internal class AST_Cards_RedrawStandalone : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_RedrawStandalone;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Draw && HasEffect(Buffs.ClarifyingDraw))
                    return Redraw;

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
                    //Grab our target (Soft->Hard->Self)
                    IGameObject? healTarget = GetHealTarget(Config.AST_ST_SimpleHeals_Adv && Config.AST_ST_SimpleHeals_UIMouseOver);

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Esuna) && ActionReady(All.Esuna) &&
                        GetTargetHPPercent(healTarget) >= Config.AST_ST_SimpleHeals_Esuna &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && ActionReady(AspectedBenefic))
                    {
                        Status? aspectedBeneficHoT = FindEffect(Buffs.AspectedBenefic, healTarget, LocalPlayer?.GameObjectId);
                        Status? NeutralSectShield = FindEffect(Buffs.NeutralSectShield, healTarget, LocalPlayer?.GameObjectId);
                        Status? NeutralSectBuff = FindEffect(Buffs.NeutralSect, healTarget, LocalPlayer?.GameObjectId);
                        if ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3)
                            || ((NeutralSectShield is null) && (NeutralSectBuff is not null)))
                            return AspectedBenefic;
                    }

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                        ActionReady(EssentialDignity) &&
                        GetTargetHPPercent(healTarget) <= Config.AST_EssentialDignity &&
                        CanSpellWeave(actionID))
                        return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                        ActionReady(Exaltation) &&
                        CanSpellWeave(actionID))
                        return Exaltation;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                        ActionReady(CelestialIntersection) &&
                        CanSpellWeave(actionID) &&
                        !(healTarget as IBattleChara)!.HasShield())
                        return CelestialIntersection;
                }
                return actionID;
            }
        }
    }
}