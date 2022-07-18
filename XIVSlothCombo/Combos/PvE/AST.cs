using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
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
            //Balance = 4401,
            //Bole = 4404,
            //Arrow = 4402,
            //Spear = 4403,
            //Ewer = 4405,
            //Spire = 4406,
            MinorArcana = 7443,
            LordOfCrowns = 7444,
            LadyOfCrown = 7445,
            CrownPlay = 25869,
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
            Exaltation = 25873;

        //Action Groups
        internal static readonly List<uint>
            MaleficList = new() { Malefic, Malefic2, Malefic3, Malefic4, FallMalefic },
            GravityList = new() { Gravity, Gravity2 };

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
                SpireDamage = 1887;
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

        private static ASTGauge Gauge => CustomComboFunctions.GetJobGauge<ASTGauge>();

        internal static class Config
        {
            internal const string
                AST_LucidDreaming = "ASTLucidDreamingFeature",
                AST_EssentialDignity = "ASTCustomEssentialDignity",
                AST_DPS_AltMode = "AST_DPS_AltMode",
                AST_DPS_DivinationOption = "AST_DPS_DivinationOption",
                AST_DPS_LightSpeedOption = "AST_DPS_LightSpeedOption",
                AST_DPS_CombustOption = "AST_DPS_CombustOption";
        }

        internal class AST_Cards_DrawOnPlay : CustomCombo
        {
            private new bool GetTarget = true;

            private new GameObject? CurrentTarget;

            private readonly List<GameObject> PartyTargets = new();

            private GameObject? SelectedRandomMember;
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_DrawOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Play)
                {
                    var haveCard = HasEffect(Buffs.BalanceDrawn) || HasEffect(Buffs.BoleDrawn) || HasEffect(Buffs.ArrowDrawn) || HasEffect(Buffs.SpearDrawn) || HasEffect(Buffs.EwerDrawn) || HasEffect(Buffs.SpireDrawn);
                    var cardDrawn = Gauge.DrawnCard;

                    if (IsEnabled(CustomComboPreset.AST_Cards_AstrodyneOnPlay) && LevelChecked(Astrodyne) && !Gauge.ContainsSeal(SealType.NONE) &&
                        (Gauge.DrawnCard != CardType.NONE || HasCharges(Draw)))
                        return Astrodyne;

                    if (haveCard)
                    {
                        if (HasEffect(Buffs.ClarifyingDraw) && IsEnabled(CustomComboPreset.AST_Cards_Redraw))
                        {
                            if ((cardDrawn is CardType.BALANCE or CardType.BOLE && Gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn is CardType.ARROW or CardType.EWER && Gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn is CardType.SPEAR or CardType.SPIRE && Gauge.Seals.Contains(SealType.CELESTIAL)))
                                return Redraw;
                        }
                        if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_AutoCardTarget))
                        {
                            if (GetTarget || IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_TargetLock))
                                SetTarget();
                        }

                        return OriginalHook(Play);
                    }

                    if (!GetTarget && (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_ReFocusTarget) || IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_ReTargetPrev)))
                    {
                        if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_ReTargetPrev))
                        {
                            TargetObject(CurrentTarget);
                        }

                        if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_ReFocusTarget))
                            TargetObject(TargetType.FocusTarget);
                    }

                    GetTarget = true;
                    SelectedRandomMember = null;
                    return OriginalHook(Draw);
                }

                return actionID;
            }

            private bool SetTarget()
            {
                if (Gauge.DrawnCard.Equals(CardType.NONE)) return false;
                CardType cardDrawn = Gauge.DrawnCard;
                if (GetTarget) CurrentTarget = LocalPlayer.TargetObject;

                for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                {
                    if (GetPartySlot(i) is not BattleChara member) continue;
                    //GameObject? member = GetPartySlot(i);
                    if (member is null) continue; //Skip nulls/disconnected people

                    if (FindEffectOnMember(Buffs.BalanceDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.ArrowDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.BoleDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.EwerDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpireDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpearDamage, member) is not null) continue;


                    PartyTargets.Add(member);
                }

                if (SelectedRandomMember is not null)
                {
                    if (PartyTargets.Any(x => x.ObjectId == SelectedRandomMember.ObjectId))
                    {
                        TargetObject(SelectedRandomMember);
                        GetTarget = false;
                        return true;
                    }
                }

                if (PartyTargets.Count > 0)
                {
                    PartyTargets.Shuffle();
                    //Give card to DPS first
                    for (int i = 0; i <= PartyTargets.Count - 1; i++)
                    {
                        byte job = PartyTargets[i] is BattleChara ? (byte)(PartyTargets[i] as BattleChara).ClassJob.Id : (byte)0;
                        if (((cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR) && JobIDs.Melee.Contains(job)) ||
                            ((cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE) && JobIDs.Ranged.Contains(job)))
                        {
                            TargetObject(PartyTargets[i]);
                            SelectedRandomMember = PartyTargets[i];
                            GetTarget = false;
                            return true;
                        }
                    }
                    //Give cards to healers/tanks if backup is turned on
                    if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_TargetExtra))
                    {
                        for (int i = 0; i <= PartyTargets.Count - 1; i++)
                        {
                            byte job = PartyTargets[i] is BattleChara ? (byte)(PartyTargets[i] as BattleChara).ClassJob.Id : (byte)0;
                            if ((cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR && JobIDs.Tank.Contains(job)) ||
                                (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE && JobIDs.Healer.Contains(job)))
                            {
                                TargetObject(PartyTargets[i]);
                                SelectedRandomMember = PartyTargets[i];
                                GetTarget = false;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        internal class AST_Cards_CrownPlay : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_CrownPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is CrownPlay && LevelChecked(MinorArcana) && Gauge.DrawnCrownCard is CardType.NONE ? MinorArcana : actionID;
        }

        internal class AST_Benefic : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Benefic;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Benefic2 && !LevelChecked(Benefic2) ? Benefic : actionID;
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
                bool AlternateMode = System.Convert.ToBoolean(GetOptionValue(Config.AST_DPS_AltMode)); //(0 or 1 radio values)
                if (((!AlternateMode && MaleficList.Contains(actionID)) ||
                     (AlternateMode && CombustList.ContainsKey(actionID)) ||
                     (IsEnabled(CustomComboPreset.AST_AoE_DPS) && GravityList.Contains(actionID))) &&
                    InCombat())
                {
                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                            ActionReady(Lightspeed) &&
                            GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_LightSpeedOption))
                            return Lightspeed;

                        if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                            ActionReady(All.LucidDreaming) &&
                            LocalPlayer.CurrentMp <= GetOptionValue(Config.AST_LucidDreaming))
                            return All.LucidDreaming;

                        //Divination
                        if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                            ActionReady(Divination) &&
                            !HasEffect(Buffs.Divination) && //Overwrite protection
                            GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_DivinationOption))
                            return Divination;

                        //Astrodyne
                        if (IsEnabled(CustomComboPreset.AST_DPS_Astrodyne) &&
                            LevelChecked(Astrodyne) &&
                            !Gauge.ContainsSeal(SealType.NONE))
                            return Astrodyne;

                        //Card Draw
                        if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                            ActionReady(Draw) &&
                            Gauge.DrawnCard.Equals(CardType.NONE))
                            return Draw;

                        //Minor Arcana
                        if (IsEnabled(CustomComboPreset.AST_DPS_AutoCrownDraw) &&
                            ActionReady(MinorArcana) &&
                            Gauge.DrawnCrownCard is CardType.NONE)
                            return MinorArcana;
                    }

                    if (HasBattleTarget())
                    {
                        //Lord of Crowns
                        if (IsEnabled(CustomComboPreset.AST_DPS_LazyLord) &&
                            LevelChecked(CrownPlay) &&
                            Gauge.DrawnCrownCard is CardType.LORD &&
                            CanSpellWeave(actionID))
                            return LordOfCrowns;

                        //Combust
                        if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                            !GravityList.Contains(actionID) &&
                            LevelChecked(Combust))
                        {
                            //Grab current DoT via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                            uint dot = OriginalHook(Combust);
                            var dotDebuff = FindTargetEffect(CombustList[dot]);
                            if (((dotDebuff is null) || (dotDebuff.RemainingTime <= 3)) &&
                                (GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_CombustOption)))
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
                        LevelChecked(CrownPlay) &&
                        InCombat() &&
                        Gauge.DrawnCrownCard is CardType.LADY)
                        return LadyOfCrown;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                        ActionReady(CelestialOpposition))
                        return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
                    {
                        if (ActionReady(Horoscope))
                            return Horoscope;

                        if ((LevelChecked(AspectedHelios) && !HasEffect(Buffs.AspectedHelios))
                             || HasEffect(Buffs.Horoscope)
                             || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                            return AspectedHelios;

                        if (HasEffect(Buffs.HoroscopeHelios))
                            return OriginalHook(Horoscope);
                    }

                    if (HasEffect(Buffs.AspectedHelios) && FindEffect(Buffs.AspectedHelios).RemainingTime > 2)
                        return Helios;
                }

                return actionID;
            }
        }

        //Works With AST_Cards_DrawOnPlay as a feature, or by itself if AST_Cards_DrawOnPlay is disabled.
        //Do not do ConflictingCombos with AST_Cards_DrawOnPlay
        internal class AST_Cards_AstrodyneOnPlay : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_AstrodyneOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Play && !IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay) && !Gauge.ContainsSeal(SealType.NONE)
                    ? Astrodyne
                    : actionID;
        }

        internal class AST_ST_SimpleHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_SimpleHeals;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Benefic2)
                {
                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && LevelChecked(AspectedBenefic))
                    {
                        var aspectedBeneficHoT = FindTargetEffect(Buffs.AspectedBenefic);
                        var NeutralSectShield = FindTargetEffect(Buffs.NeutralSectShield);
                        var NeutralSectBuff = FindTargetEffect(Buffs.NeutralSect);
                        if (((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3))
                            || ((NeutralSectShield is null) && (NeutralSectBuff is not null)))
                            return AspectedBenefic;
                    }

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                        ActionReady(EssentialDignity) &&
                        GetTargetHPPercent() <= GetOptionValue(Config.AST_EssentialDignity))
                        return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                        ActionReady(Exaltation))
                        return Exaltation;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                        ActionReady(CelestialIntersection))
                        return CelestialIntersection;
                }
                return actionID;
            }
        }
    }
}