using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class AST
    {
        public const byte JobID = 33;
        private static ASTGauge Gauge => CustomComboNS.Functions.CustomComboFunctions.GetJobGauge<ASTGauge>();

        public const uint
            Benefic = 3594,
            Draw = 3590,
            /*
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            */
            MinorArcana = 7443,
            //SleeveDraw = 7448,
            Malefic4 = 16555,
            Ascend = 3603,
            CrownPlay = 25869,
            Astrodyne = 25870,
            FallMalefic = 25871,
            Malefic1 = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Play = 17055,
            LordOfCrowns = 7444,
            LadyOfCrown = 7445,
            Divination = 16552,
            Lightspeed = 3606,
            Redraw = 3593,

            // AoEs
            Gravity = 3615,
            Gravity2 = 25872,

            // DoTs
            Combust3 = 16554,
            Combust2 = 3608,
            Combust1 = 3599,

            // Heals
            Helios = 3600,
            AspectedHelios = 3601,
            CelestialOpposition = 16553,
            Benefic2 = 3610,
            EssentialDignity = 3614,
            CelestialIntersection = 16556,
            AspectedBenefic = 3595,
            Horoscope = 16557,
            Exaltation = 25873;

        private static class Buffs
        {
            public const ushort
                Divination = 1878,
                LordOfCrownsDrawn = 2054,
                LadyOfCrownsDrawn = 2055,
                AspectedHelios = 836,
                Balance = 913,
                Bole = 914,
                Arrow = 915,
                Spear = 916,
                Ewer = 917,
                Spire = 918,
                BalanceDamage = 1882,
                BoleDamage = 1883,
                ArrowDamage = 1884,
                SpearDamage = 1885,
                EwerDamage = 1886,
                SpireDamage = 1887,
                Horoscope = 1890,
                HoroscopeHelios = 1891,
                AspectedBenefic = 835,
                NeutralSect = 1892,
                NeutralSectShield = 1921,
                ClarifyingDraw = 2713;
        }

        public static class Debuffs
        {
            public const ushort
                Combust1 = 838,
                Combust2 = 843,
                Combust3 = 1881;
        }

        public static class Config
        {
            public const string
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

            private List<GameObject> PartyTargets = new();

            private GameObject? SelectedRandomMember;
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_DrawOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Play)
                {
                    bool haveCard = HasEffect(Buffs.Balance) || HasEffect(Buffs.Bole) || HasEffect(Buffs.Arrow) || HasEffect(Buffs.Spear) || HasEffect(Buffs.Ewer) || HasEffect(Buffs.Spire);
                    CardType cardDrawn = Gauge.DrawnCard;

                    if (!Gauge.ContainsSeal(SealType.NONE) &&
                        IsEnabled(CustomComboPreset.AST_Cards_AstrodyneOnPlay) &&
                        (Gauge.DrawnCard != CardType.NONE || GetCooldown(Draw).CooldownRemaining > 30)
                       ) return Astrodyne;

                    if (haveCard)
                    {
                        if (HasEffect(Buffs.ClarifyingDraw) && IsEnabled(CustomComboPreset.AST_Cards_Redraw))
                        {
                            if ((cardDrawn == CardType.BALANCE && Gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.ARROW && Gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPEAR && Gauge.Seals.Contains(SealType.CELESTIAL)) ||
                                (cardDrawn == CardType.BOLE && Gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.EWER && Gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPIRE && Gauge.Seals.Contains(SealType.CELESTIAL)))

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
                //Checks for trusts then normal parties. Buddylist does not include player, so +1
                int maxPartySize = GetPartySlot(5) == null ? 4 : 8;

                for (int i = 1; i <= maxPartySize; i++)
                {
                    GameObject? member = GetPartySlot(i);
                    if (member == null) continue; //Skip nulls/disconnected people

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
            {
                if (actionID == CrownPlay)
                {
                    if (LevelChecked(MinorArcana) && Gauge.DrawnCrownCard == CardType.NONE)
                        return MinorArcana;
                }

                return actionID;
            }
        }

        internal class AST_Benefic : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Benefic;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Benefic2 && !LevelChecked(Benefic2)) return Benefic;
                else return actionID;
            }
        }

        internal class AST_Raise_Alternative : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Raise_Alternative;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast)) return Ascend;
                else return actionID;
            }
        }

        internal class AST_ST_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool AlternateMode = System.Convert.ToBoolean(GetOptionValue(Config.AST_DPS_AltMode)); //(0 or 1 radio values)
                if (((!AlternateMode && actionID is FallMalefic or Malefic4 or Malefic3 or Malefic2 or Malefic1) ||
                     (AlternateMode && actionID is Combust1 or Combust2 or Combust3) ||
                     (IsEnabled(CustomComboPreset.AST_AoE_DPS) && actionID is Gravity or Gravity2)) &&
                    InCombat())
                {
                    if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                        LevelChecked(Lightspeed) &&
                        IsOffCooldown(Lightspeed) &&
                        GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_LightSpeedOption) &&
                        CanSpellWeave(actionID)
                       ) return Lightspeed;

                    if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                        LevelChecked(All.LucidDreaming) &&
                        IsOffCooldown(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.AST_LucidDreaming) &&
                        CanSpellWeave(actionID)
                       ) return All.LucidDreaming;

                    //Divination
                    if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                        LevelChecked(Divination) &&
                        IsOffCooldown(Divination) &&
                        !HasEffect(Buffs.Divination) && //Overwrite protection
                        GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_DivinationOption) &&
                        CanSpellWeave(actionID)
                       ) return Divination;

                    //Astrodyne
                    if (IsEnabled(CustomComboPreset.AST_DPS_Astrodyne) &&
                        LevelChecked(Astrodyne) &&
                        !Gauge.ContainsSeal(SealType.NONE) &&
                        CanSpellWeave(actionID)
                        ) return Astrodyne;

                    //Card Draw
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                        LevelChecked(Draw) &&
                        Gauge.DrawnCard.Equals(CardType.NONE) &&
                        GetCooldown(Draw).RemainingCharges > 0 &&
                        CanSpellWeave(actionID)
                       ) return Draw;

                    //Minor Arcana
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoCrownDraw) &&
                        LevelChecked(MinorArcana) &&
                        Gauge.DrawnCrownCard == CardType.NONE &&
                        IsOffCooldown(MinorArcana) &&
                        CanSpellWeave(actionID)
                       ) return MinorArcana;

                    //Lord of Crowns
                    if (IsEnabled(CustomComboPreset.AST_DPS_LazyLord) &&
                        LevelChecked(CrownPlay) &&
                        Gauge.DrawnCrownCard is CardType.LORD &&
                        CanSpellWeave(actionID)
                       ) return LordOfCrowns;

                    //Combust
                    if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                        actionID is not Gravity and not Gravity2 &&
                        LevelChecked(Combust1) &&
                        HasBattleTarget())
                    {
                        //Determine which Combust debuff to check
                        Status? CombustDebuffID;
                        if (LevelChecked(Combust3)) CombustDebuffID = FindTargetEffect(Debuffs.Combust3);
                        else if (LevelChecked(Combust2)) CombustDebuffID = FindTargetEffect(Debuffs.Combust2);
                        else CombustDebuffID = FindTargetEffect(Debuffs.Combust1);

                        if ((CombustDebuffID is null || CombustDebuffID?.RemainingTime <= 3) &&
                            (GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_CombustOption))
                           ) return OriginalHook(Combust1);

                        //AlterateMode idles as Malefic
                        if (AlternateMode) return OriginalHook(Malefic1);
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
                        Gauge.DrawnCrownCard == CardType.LADY
                       ) return LadyOfCrown;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                        LevelChecked(CelestialOpposition) &&
                        IsOffCooldown(CelestialOpposition)
                       ) return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
                    {
                        if (LevelChecked(Horoscope) &&
                            IsOffCooldown(Horoscope)
                           ) return Horoscope;

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

        internal class AST_Cards_AstrodyneOnPlay : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_AstrodyneOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Play && !IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay))
                {
                    if (!Gauge.ContainsSeal(SealType.NONE))
                        return Astrodyne;
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
                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && LevelChecked(AspectedBenefic))
                    {
                        Status? aspectedBeneficHoT = FindTargetEffect(Buffs.AspectedBenefic);
                        Status? NeutralSectShield = FindTargetEffect(Buffs.NeutralSectShield);
                        Status? NeutralSectBuff = FindTargetEffect(Buffs.NeutralSect);
                        if ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3)
                            || ((NeutralSectShield is null) && (NeutralSectBuff is not null))
                           ) return AspectedBenefic;
                    }

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                        LevelChecked(EssentialDignity) &&
                        GetCooldown(EssentialDignity).RemainingCharges > 0 &&
                        GetTargetHPPercent() <= GetOptionValue(Config.AST_EssentialDignity)
                       ) return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                        LevelChecked(Exaltation) &&
                        IsOffCooldown(Exaltation)
                       ) return Exaltation;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                        LevelChecked(CelestialIntersection) &&
                        GetCooldown(CelestialIntersection).RemainingCharges > 0
                       ) return CelestialIntersection;
                }
                return actionID;
            }
        }
    }
}