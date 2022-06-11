using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Benefic = 3594,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            MinorArcana = 7443,
            SleeveDraw = 7448,
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

            // aoes
            Gravity = 3615,
            Gravity2 = 25872,

            // dots
            Combust3 = 16554,
            Combust2 = 3608,
            Combust1 = 3599,


            // heals
            Helios = 3600,
            AspectedHelios = 3601,
            CelestialOpposition = 16553,
            Benefic2 = 3610,
            EssentialDignity = 3614,
            CelestialIntersection = 16556,
            AspectedBenefic = 3595,
            Horoscope = 16557,
            Exaltation = 25873;

        public static class Buffs
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

        public static class Levels
        {
            public const byte
                Combust = 4,
                Lightspeed = 6,
                EssentialDignity = 15,
                Benefic2 = 26,
                Draw = 30,
                AspectedBenefic = 34,
                AspectedHelios = 42,
                Combust2 = 46,
                Divination = 50,
                Astrodyne = 50,
                CelestialOpposition = 60,
                MinorArcana = 70,
                CrownPlay = 70,
                Combust3 = 72,
                CelestialIntersection = 74,
                Horoscope = 76,
                NeutralSect = 80,
                Exaltation = 86;
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
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_DrawOnPlay;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Play)
                {
                    var gauge = GetJobGauge<ASTGauge>();
                    var haveCard = HasEffect(Buffs.Balance) || HasEffect(Buffs.Bole) || HasEffect(Buffs.Arrow) || HasEffect(Buffs.Spear) || HasEffect(Buffs.Ewer) || HasEffect(Buffs.Spire);
                    var cardDrawn = gauge.DrawnCard;

                    if (!gauge.ContainsSeal(SealType.NONE) && IsEnabled(CustomComboPreset.AST_Cards_AstrodyneOnPlay) && (gauge.DrawnCard != CardType.NONE || GetCooldown(Draw).CooldownRemaining > 30))
                        return Astrodyne;

                    if (haveCard)
                    {
                        if (HasEffect(Buffs.ClarifyingDraw) && IsEnabled(CustomComboPreset.AST_Cards_Redraw))
                        {
                            if ((cardDrawn == CardType.BALANCE && gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.ARROW && gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPEAR && gauge.Seals.Contains(SealType.CELESTIAL)) ||
                                (cardDrawn == CardType.BOLE && gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.EWER && gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPIRE && gauge.Seals.Contains(SealType.CELESTIAL)))

                                return Redraw;
                        }
                        if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_AutoCardTarget))
                        {
                            if (GetTarget || (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_TargetLock)))
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
                    return OriginalHook(Draw);
                }

                return actionID;
            }

            private bool SetTarget()
            {
                var gauge = GetJobGauge<ASTGauge>();
                if (gauge.DrawnCard.Equals(CardType.NONE)) return false;
                var cardDrawn = gauge.DrawnCard;
                if (GetTarget) CurrentTarget = LocalPlayer.TargetObject;
                //Checks for trusts then normal parties
                int maxPartySize = GetPartySlot(5) == null ? 4 : 8;
                if (GetPartyMembers().Length > 0) maxPartySize = GetPartyMembers().Length;
                if (GetPartyMembers().Length == 0 && Service.BuddyList.Length == 0) maxPartySize = 0;

                for (int i = 2; i <= maxPartySize; i++)
                {
                    GameObject? member = GetPartySlot(i);

                    if (member == null) break;
                    string job = "";
                    if (member is BattleNpc) job = (member as BattleNpc).ClassJob.GameData.Name.ToString();
                    if (member is BattleChara) job = (member as BattleChara).ClassJob.GameData.Name.ToString();

                    if (FindEffectOnMember(Buffs.BalanceDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.ArrowDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.BoleDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.EwerDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpireDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpearDamage, member) is not null) continue;

                    if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR && JobNames.Melee.Contains(job))
                    {
                        TargetObject(member);
                        GetTarget = false;
                        return true;
                    }

                    if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE && JobNames.Ranged.Contains(job))
                    {
                        TargetObject(member);
                        GetTarget = false;
                        return true;
                    }
                }

                if (IsEnabled(CustomComboPreset.AST_Cards_DrawOnPlay_TargetExtra))
                {
                    for (int i = 1; i <= maxPartySize; i++)
                    {
                        GameObject? member = GetPartySlot(i);
                        if (member == null) break;
                        string job = "";
                        if (member is BattleNpc) job = (member as BattleNpc).ClassJob.GameData.Name.ToString();
                        if (member is BattleChara) job = (member as BattleChara).ClassJob.GameData.Name.ToString();

                        if (FindEffectOnMember(Buffs.BalanceDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.ArrowDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.BoleDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.EwerDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpireDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpearDamage, member) is not null) continue;

                        if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR && JobNames.Tank.Contains(job))
                        { 
                            TargetObject(member);
                            GetTarget = false;
                            return true;
                        }

                        if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE && JobNames.Healer.Contains(job))
                        { 
                            TargetObject(member);
                            GetTarget = false;
                            return true;
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
                    var gauge = GetJobGauge<ASTGauge>();
                    /*var ladyofCrown = HasEffect(AST.Buffs.LadyOfCrownsDrawn);
                    var lordofCrown = HasEffect(AST.Buffs.LordOfCrownsDrawn);
                    var minorArcanaCD = GetCooldown(AST.MinorArcana);*/
                    if (level >= Levels.MinorArcana && gauge.DrawnCrownCard == CardType.NONE)
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
                if (actionID is Benefic2 && level < Levels.Benefic2) return Benefic;
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
                     (AlternateMode && actionID is Combust1 or Combust2 or Combust3 ) ||
                     (IsEnabled(CustomComboPreset.AST_AoE_DPS) && actionID is Gravity or Gravity2)) && 
                    InCombat())
                {
                    if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                        level >= Levels.Lightspeed &&
                        IsOffCooldown(Lightspeed) &&
                        GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_LightSpeedOption) &&
                        CanSpellWeave(actionID)
                       ) return Lightspeed;

                    if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                        level >= All.Levels.LucidDreaming &&
                        IsOffCooldown(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.AST_LucidDreaming) &&
                        CanSpellWeave(actionID)
                       ) return All.LucidDreaming;

                    //Divination
                    if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                        level >= Levels.Divination &&
                        IsOffCooldown(Divination) &&
                        !HasEffect(Buffs.Divination) && //Overwrite protection
                        GetTargetHPPercent() > GetOptionValue(Config.AST_DPS_DivinationOption) &&
                        CanSpellWeave(actionID)
                       ) return Divination;

                    //Astrodyne
                    if (IsEnabled(CustomComboPreset.AST_DPS_Astrodyne) &&
                        level >= Levels.Astrodyne &&
                        !GetJobGauge<ASTGauge>().ContainsSeal(SealType.NONE) &&
                        CanSpellWeave(actionID)
                        ) return Astrodyne;
                    
                    //Card Draw
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                        level >= Levels.Draw &&
                        GetJobGauge<ASTGauge>().DrawnCard.Equals(CardType.NONE) &&
                        GetCooldown(Draw).RemainingCharges > 0 &&
                        CanSpellWeave(actionID)
                       ) return Draw;

                    //Minor Arcana
                    if (IsEnabled(CustomComboPreset.AST_DPS_AutoCrownDraw) &&
                        level >= Levels.MinorArcana &&
                        GetJobGauge<ASTGauge>().DrawnCrownCard == CardType.NONE &&
                        IsOffCooldown(MinorArcana) &&
                        CanSpellWeave(actionID)
                       ) return MinorArcana;

                    //Lord of Crowns
                    if (IsEnabled(CustomComboPreset.AST_DPS_LazyLord) &&
                        level >= Levels.CrownPlay &&
                        GetJobGauge<ASTGauge>().DrawnCrownCard is CardType.LORD &&
                        CanSpellWeave(actionID)
                       ) return LordOfCrowns;

                    //Combust
                    if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                        actionID is not Gravity and not Gravity2 &&
                        level >= Levels.Combust &&
                        (CurrentTarget as BattleNpc)?.BattleNpcKind is BattleNpcSubKind.Enemy)
                    {
                        //Determine which Combust debuff to check
                        var CombustDebuffID = level switch
                        {
                            //Using FindEffect b/c we have a custom Target variable
                            >= Levels.Combust3 => FindTargetEffect(Debuffs.Combust3),
                            >= Levels.Combust2 => FindTargetEffect(Debuffs.Combust2),
                            //Combust 1 level checked at the start, fine for default
                            _ => FindTargetEffect(Debuffs.Combust1),
                        };
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
                    if (level < Levels.AspectedHelios)
                        return Helios;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_LazyLady) &&
                        level >= Levels.CrownPlay &&
                        InCombat() &&
                        GetJobGauge<ASTGauge>().DrawnCrownCard == CardType.LADY 
                       ) return LadyOfCrown;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                        level >= Levels.CelestialOpposition &&
                        IsOffCooldown(CelestialOpposition) 
                       ) return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
                    {
                        if (level >= Levels.Horoscope && 
                            IsOffCooldown(Horoscope)
                           ) return Horoscope;

                        if ( (level >= Levels.AspectedHelios && !HasEffect(Buffs.AspectedHelios))
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
                    var gauge = GetJobGauge<ASTGauge>();
                    if (!gauge.ContainsSeal(SealType.NONE))
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
                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && level >= Levels.AspectedBenefic)
                    {
                        var aspectedBeneficHoT = FindTargetEffect(Buffs.AspectedBenefic);
                        var NeutralSectShield = FindTargetEffect(Buffs.NeutralSectShield);
                        var NeutralSectBuff = FindTargetEffect(Buffs.NeutralSect);
                        if (((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3))
                            || ((NeutralSectShield is null) && (NeutralSectBuff is not null))
                           ) return AspectedBenefic;
                    }

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                        level >= Levels.EssentialDignity && 
                        GetCooldown(EssentialDignity).RemainingCharges > 0 && 
                        GetTargetHPPercent() <= GetOptionValue(Config.AST_EssentialDignity)
                       ) return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) && 
                        level >= Levels.Exaltation &&
                        IsOffCooldown(Exaltation)
                       ) return Exaltation;

                    if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                        level >= Levels.CelestialIntersection &&
                        GetCooldown(CelestialIntersection).RemainingCharges > 0
                       ) return CelestialIntersection;
                }
                return actionID;
            }
        }
    }
}