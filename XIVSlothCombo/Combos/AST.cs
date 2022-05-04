using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;

namespace XIVSlothComboPlugin.Combos
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
            Combust = 3599,
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
                EssentialDignity = 15,
                Benefic2 = 26,
                MinorArcana = 50,
                Draw = 30,
                AspectedBenefic = 34,
                AspectedHelios = 42,
                CrownPlay = 70,
                CelestialOpposition = 60,
                CelestialIntersection = 74,
                Horoscope = 76,
                NeutralSect = 80,
                Exaltation = 86;
        }

        public static class Config
        {
            public const string
                ASTLucidDreamingFeature = "ASTLucidDreamingFeature";
            public const string
                AstroEssentialDignity = "ASTCustomEssentialDignity";
        }

        public static class MeleeCardTargets
        {
            public const string
                Monk = "monk",
                Dragoon = "dragoon",
                Ninja = "ninja",
                Reaper = "reaper",
                Samurai = "samurai",
                Pugilist = "pugilist",
                Lancer = "lancer",
                Rogue = "rogue";
        }

        public static class RangedCardTargets
        {
            public const string
                Bard = "bard",
                Machinist = "machinist",
                Dancer = "dancer",
                RedMage = "red mage",
                BlackMage = "black mage",
                Summoner = "summoner",
                BlueMage = "blue mage",
                Archer = "archer",
                Thaumaturge = "thaumaturge",
                Arcanist = "arcanist";

        }

        public static class TankCardTargets
        {
            public const string
                Paladin = "paladin",
                Warrior = "warrior",
                DarkKnight = "dark knight",
                Gunbreaker = "gunbreaker",
                Gladiator = "gladiator",
                Marauder = "marauder";
        }

        public static class HealerCardTargets
        {
            public const string
                WhiteMage = "white mage",
                Astrologian = "astrologian",
                Scholar = "scholar",
                Sage = "sage",
                Conjurer = "conjurer";
        }

        public static class MeleeCardTargetsCN
        {
            public const string
                Monk = "武僧",
                Dragoon = "龙骑士",
                Ninja = "忍者",
                Reaper = "钐镰客",
                Samurai = "武士",
                Pugilist = "格斗家",
                Lancer = "枪术师",
                Rogue = "双剑师";
        }

        public static class RangedCardTargetsCN
        {
            public const string
                Bard = "吟游诗人",
                Machinist = "机工士",
                Dancer = "舞者",
                RedMage = "赤魔法师",
                BlackMage = "黑魔法师",
                Summoner = "召唤师",
                BlueMage = "青魔法师",
                Archer = "弓箭手",
                Thaumaturge = "咒术师",
                Arcanist = "秘术师";

        }

        public static class TankCardTargetsCN
        {
            public const string
                Paladin = "骑士",
                Warrior = "战士",
                DarkKnight = "暗黑骑士",
                Gunbreaker = "绝枪战士",
                Gladiator = "剑术师",
                Marauder = "斧术师";
        }

        public static class HealerCardTargetsCN
        {
            public const string
                WhiteMage = "白魔法师",
                Astrologian = "占星术士",
                Scholar = "学者",
                Sage = "贤者",
                Conjurer = "幻术师";
        }

        public static class MeleeCardTargetsJP
        {
            public const string
                Monk = "モンク",
                Dragoon = "竜騎士",
                Ninja = "忍者",
                Reaper = "リーパー",
                Samurai = "侍",
                Pugilist = "格闘士",
                Lancer = "槍術士",
                Rogue = "双剣士";
        }

        public static class RangedCardTargetsJP
        {
            public const string
                Bard = "吟遊詩人",
                Machinist = "機工士",
                Dancer = "踊り子",
                RedMage = "赤魔道士",
                BlackMage = "黒魔道士",
                Summoner = "召喚士",
                BlueMage = "青魔道士",
                Archer = "弓術士",
                Thaumaturge = "呪術士",
                Arcanist = "巴術士";

        }

        public static class TankCardTargetsJP
        {
            public const string
                Paladin = "ナイト",
                Warrior = "戦士",
                DarkKnight = "暗黒騎士",
                Gunbreaker = "ガンブレイカー",
                Gladiator = "剣術士",
                Marauder = "斧術士";
        }

        public static class HealerCardTargetsJP
        {
            public const string
                WhiteMage = "白魔道士",
                Astrologian = "占星術師",
                Scholar = "学者",
                Sage = "賢者",
                Conjurer = "幻術士";
        }


        internal class AstrologianCardsOnDrawFeaturelikewhat : CustomCombo
        {
            private new bool GetTarget = true;

            private new GameObject? CurrentTarget;
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Play)
                {
                    var gauge = GetJobGauge<ASTGauge>();
                    var haveCard = HasEffect(Buffs.Balance) || HasEffect(Buffs.Bole) || HasEffect(Buffs.Arrow) || HasEffect(Buffs.Spear) || HasEffect(Buffs.Ewer) || HasEffect(Buffs.Spire);
                    var cardDrawn = gauge.DrawnCard;

                    if (!gauge.ContainsSeal(SealType.NONE) && IsEnabled(CustomComboPreset.AstrologianAstrodyneOnPlayFeature) && (gauge.DrawnCard != CardType.NONE || GetCooldown(Draw).CooldownRemaining > 30))
                        return Astrodyne;

                    if (haveCard)
                    {
                        if (HasEffect(Buffs.ClarifyingDraw) && IsEnabled(CustomComboPreset.AstRedrawFeature))
                        {
                            if ((cardDrawn == CardType.BALANCE && gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.ARROW && gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPEAR && gauge.Seals.Contains(SealType.CELESTIAL)) ||
                                (cardDrawn == CardType.BOLE && gauge.Seals.Contains(SealType.SUN)) ||
                                (cardDrawn == CardType.EWER && gauge.Seals.Contains(SealType.MOON)) ||
                                (cardDrawn == CardType.SPIRE && gauge.Seals.Contains(SealType.CELESTIAL)))

                                return Redraw;
                        }
                        if (IsEnabled(CustomComboPreset.AstAutoCardTarget))
                        {
                            if (GetTarget || (IsEnabled(CustomComboPreset.AstrologianTargetLock)))
                                SetTarget();


                        }

                        return OriginalHook(Play);
                    }

                    if (!GetTarget && (IsEnabled(CustomComboPreset.AstReFocusFeature) || IsEnabled(CustomComboPreset.AstReTargetFeature)))
                    {
                        if (IsEnabled(CustomComboPreset.AstReTargetFeature))
                        {
                            TargetObject(CurrentTarget);
                        }


                        if (IsEnabled(CustomComboPreset.AstReFocusFeature))
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

                    if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR)
                    {
                        if (typeof(MeleeCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(MeleeCardTargetsCN).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(MeleeCardTargetsJP).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                        {
                            TargetObject(member);
                            GetTarget = false;
                            return true;
                        }

                    }
                    if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE)
                    {
                        if (typeof(RangedCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(RangedCardTargetsCN).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(RangedCardTargetsJP).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                        {
                            TargetObject(member);
                            GetTarget = false;
                            return true;
                        }
                    }
                }

                if (IsEnabled(CustomComboPreset.AstrologianTargetExtraFeature))
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

                        if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR)
                        {
                            if (typeof(TankCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(TankCardTargetsCN).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(TankCardTargetsJP).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                            {
                                TargetObject(member);
                                GetTarget = false;
                                return true;
                            }

                        }
                        if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE)
                        {
                            if (typeof(HealerCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(HealerCardTargetsCN).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job) || typeof(HealerCardTargetsJP).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                            {
                                TargetObject(member);
                                GetTarget = false;
                                return true;
                            }

                        }
                    }

                }

                return false;

            }
        }


        internal class AstrologianCrownPlayFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCrownPlayFeature;

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

        internal class AstrologianBeneficFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianBeneficFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Benefic2)
                {
                    if (level < Levels.Benefic2)
                        return Benefic;
                }

                return actionID;
            }
        }

        internal class AstrologianAscendFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAscendFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == All.Swiftcast)
                {
                    if (IsEnabled(CustomComboPreset.AstrologianAscendFeature))
                    {
                        if (HasEffect(All.Buffs.Swiftcast))
                            return Ascend;
                    }

                    return OriginalHook(All.Swiftcast);
                }

                return actionID;
            }
        }

        internal class AstrologianDpsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == FallMalefic || actionID == Malefic4 || actionID == Malefic3 || actionID == Malefic2 || actionID == Malefic1)
                {

                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var combust3Debuff = FindTargetEffect(Debuffs.Combust3);
                    var combust2Debuff = FindTargetEffect(Debuffs.Combust2);
                    var combust1Debuff = FindTargetEffect(Debuffs.Combust1);
                    var gauge = GetJobGauge<ASTGauge>();
                    var lucidDreaming = GetCooldown(All.LucidDreaming);
                    var fallmalefic = GetCooldown(FallMalefic);
                    var minorarcanaCD = GetCooldown(MinorArcana);
                    var drawCD = GetCooldown(Draw);
                    var actionIDCD = GetCooldown(actionID);
                    var lucidMPThreshold = Service.Configuration.GetCustomIntValue(Config.ASTLucidDreamingFeature);

                    if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                    {
                        var lightspeed = GetCooldown(Lightspeed);
                        if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                            return Lightspeed;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                    {
                        if (!gauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6 && level >= 50)
                            return Astrodyne;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                    {
                        if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                            return Draw;

                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return MinorArcana;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= All.Levels.LucidDreaming)
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= lucidMPThreshold && fallmalefic.CooldownRemaining > 0.2)
                            return All.LucidDreaming;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return LordOfCrowns;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 72 && incombat)
                    {
                        if ((combust3Debuff is null) || (combust3Debuff?.RemainingTime <= 3))
                            return Combust3;
                    }

                    if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 46 && level <= 71 && incombat)
                    {
                        if ((combust2Debuff is null) || (combust2Debuff?.RemainingTime <= 3))
                            return Combust2;
                    }

                    if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 4 && level <= 45 && incombat)
                    {
                        if ((combust1Debuff is null) || (combust1Debuff?.RemainingTime <= 3))
                            return Combust1;
                    }
                }

                return actionID;
            }
        }

        internal class AstrologianHeliosFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianHeliosFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AspectedHelios)
                {
                    var heliosBuff = FindEffect(Buffs.AspectedHelios);
                    var horoscopeCD = GetCooldown(Horoscope);
                    var celestialOppositionCD = GetCooldown(CelestialOpposition);
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<ASTGauge>();

                    if (level < Levels.AspectedHelios)
                        return Helios;

                    if (IsEnabled(CustomComboPreset.AstrologianLazyLadyFeature))
                    {
                        if (gauge.DrawnCrownCard == CardType.LADY && incombat && level >= Levels.CrownPlay)
                            return LadyOfCrown;
                    }

                    if (IsEnabled(CustomComboPreset.AstrologianCelestialOppositionFeature) && celestialOppositionCD.CooldownRemaining == 0 && level >= Levels.CelestialOpposition)
                        return CelestialOpposition;

                    if (IsEnabled(CustomComboPreset.AstrologianHoroscopeFeature))
                    {
                        if (horoscopeCD.CooldownRemaining == 0 && level >= Levels.Horoscope)
                            return Horoscope;

                        if ((!HasEffect(Buffs.AspectedHelios) && level >= Levels.AspectedHelios)
                             || HasEffect(Buffs.Horoscope)
                             || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                            return AspectedHelios;

                        if (HasEffect(Buffs.HoroscopeHelios))
                            return OriginalHook(Horoscope);
                    }

                    if (HasEffect(Buffs.AspectedHelios) && heliosBuff?.RemainingTime > 2)
                        return Helios;
                }

                return actionID;
            }
        }
        internal class AstrologianDpsAoEFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsAoEFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Gravity || actionID == Gravity2)
                {

                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<ASTGauge>();
                    var lucidDreaming = GetCooldown(All.LucidDreaming);
                    var minorarcanaCD = GetCooldown(MinorArcana);
                    var drawCD = GetCooldown(Draw);
                    var actionIDCD = GetCooldown(actionID);
                    var lucidMPThreshold = Service.Configuration.GetCustomIntValue(Config.ASTLucidDreamingFeature);

                    if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                    {
                        var lightspeed = GetCooldown(Lightspeed);
                        if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                            return Lightspeed;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                    {
                        if (!gauge.ContainsSeal(SealType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 50)
                            return Astrodyne;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                    {
                        if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                            return Draw;

                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return LordOfCrowns;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return MinorArcana;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= All.Levels.LucidDreaming)
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= lucidMPThreshold && actionIDCD.CooldownRemaining > 0.2)
                            return All.LucidDreaming;
                    }
                }
                return actionID;
            }
        }
        internal class AstrologianAstrodyneOnPlayFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAstrodyneOnPlayFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Play && !IsEnabled(CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat))
                {
                    var gauge = GetJobGauge<ASTGauge>();
                    if (!gauge.ContainsSeal(SealType.NONE))
                        return Astrodyne;
                }
                return actionID;

            }
        }
        internal class AstrologianAlternateDpsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAlternateDpsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Combust1 || actionID == Combust2 || actionID == Combust3)
                {

                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var combust3Debuff = FindTargetEffect(Debuffs.Combust3);
                    var combust2Debuff = FindTargetEffect(Debuffs.Combust2);
                    var combust1Debuff = FindTargetEffect(Debuffs.Combust1);
                    var gauge = GetJobGauge<ASTGauge>();
                    var lucidDreaming = GetCooldown(All.LucidDreaming);
                    var fallmalefic = GetCooldown(FallMalefic);
                    var minorarcanaCD = GetCooldown(MinorArcana);
                    var drawCD = GetCooldown(Draw);
                    var actionIDCD = GetCooldown(actionID);
                    var lucidMPThreshold = Service.Configuration.GetCustomIntValue(Config.ASTLucidDreamingFeature);


                    if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                    {
                        var lightspeed = GetCooldown(Lightspeed);
                        if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                            return Lightspeed;
                    }
                    if (!HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                    {
                        return OriginalHook(Malefic1);
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                    {
                        if (!gauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6 && level >= 50)
                            return Astrodyne;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                    {
                        if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                            return Draw;

                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return MinorArcana;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= All.Levels.LucidDreaming)
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= lucidMPThreshold && fallmalefic.CooldownRemaining > 0.2)
                            return All.LucidDreaming;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                            return LordOfCrowns;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 72 && incombat)
                    {
                        if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                            return Combust3;
                    }

                    if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 46 && level <= 71 && incombat)
                    {
                        if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                            return Combust2;
                    }

                    if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 4 && level <= 45 && incombat)
                    {
                        if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                            return Combust1;
                    }
                    return OriginalHook(Malefic1);
                }
                return actionID;
            }
        }
        internal class CustomValuesTest : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CustomValuesTest;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == FallMalefic || actionID == Malefic4 || actionID == Malefic3 || actionID == Malefic2 || actionID == Malefic1)
                {

                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var combust3Debuff = FindTargetEffect(Debuffs.Combust3);
                    var combust2Debuff = FindTargetEffect(Debuffs.Combust2);
                    var combust1Debuff = FindTargetEffect(Debuffs.Combust1);
                    var gauge = GetJobGauge<ASTGauge>();
                    var lucidDreaming = GetCooldown(All.LucidDreaming);
                    var fallmalefic = GetCooldown(FallMalefic);
                    var minorarcanaCD = GetCooldown(MinorArcana);
                    var drawCD = GetCooldown(Draw);
                    var actionIDCD = GetCooldown(actionID);
                    var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                    var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                    var CurrentHpValue = Service.Configuration.EnemyCurrentHp;
                    var lucidMPThreshold = Service.Configuration.GetCustomIntValue(Config.ASTLucidDreamingFeature);

                    if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                    {
                        var lightspeed = GetCooldown(Lightspeed);
                        if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4)
                            return Lightspeed;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                    {
                        if (!gauge.ContainsSeal(SealType.NONE) && lastComboMove == OriginalHook(actionID) && fallmalefic.CooldownRemaining >= 0.4 && level >= 50)
                            return Astrodyne;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                    {
                        if (gauge.DrawnCard.Equals(CardType.NONE) && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4 && drawCD.CooldownRemaining < 30 && level >= 30)
                            return Draw;

                    }
                    if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                    {
                        if (gauge.DrawnCrownCard == CardType.NONE && lastComboMove == OriginalHook(actionID) && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.4 && level >= 70)
                            return MinorArcana;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= All.Levels.LucidDreaming)
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= lucidMPThreshold && fallmalefic.CooldownRemaining > 0.4)
                            return All.LucidDreaming;
                    }
                    if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                    {
                        var buff = HasEffect(Buffs.Divination);
                        var buffcd = GetCooldown(Divination);
                        if (gauge.DrawnCrownCard == CardType.LORD && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4 && level >= 70 && buff || gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.4 && level >= 70 && buffcd.IsCooldown)
                            return LordOfCrowns;
                    }
                    if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 72 && incombat)
                    {
                        if ((combust3Debuff is null) && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue || (combust3Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue)
                            return Combust3;
                    }

                    if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 46 && level <= 71 && incombat)
                    {
                        if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                            return Combust2;
                    }

                    if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 4 && level <= 45 && incombat)
                    {
                        if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                            return Combust1;
                    }
                }
                return actionID;
            }

        }

        internal class AstrologianSimpleSingleTargetHeal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianSimpleSingleTargetHeal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Benefic2)
                {
                    var aspectedBeneficHoT = FindTargetEffect(Buffs.AspectedBenefic);
                    var NeutralSectBuff = FindTargetEffect(Buffs.NeutralSect);
                    var NeutralSectShield = FindTargetEffect(Buffs.NeutralSectShield);
                    var customEssentialDignity = Service.Configuration.GetCustomIntValue(Config.AstroEssentialDignity);
                    var exaltationCD = GetCooldown(Exaltation);

                    if (IsEnabled(CustomComboPreset.AspectedBeneficFeature) && ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3)) || ((NeutralSectShield is null) && (NeutralSectBuff is not null)))
                        return AspectedBenefic;

                    if (IsEnabled(CustomComboPreset.AstroEssentialDignity) && GetCooldown(EssentialDignity).RemainingCharges > 0 && level >= Levels.EssentialDignity && EnemyHealthPercentage() <= customEssentialDignity)
                        return EssentialDignity;

                    if (IsEnabled(CustomComboPreset.ExaltationFeature) && exaltationCD.CooldownRemaining == 0 && level >= Levels.Exaltation)
                        return Exaltation;

                    if (IsEnabled(CustomComboPreset.CelestialIntersectionFeature) && GetCooldown(CelestialIntersection).RemainingCharges > 0 && level >= Levels.CelestialIntersection)
                        return CelestialIntersection;
                }


                return actionID;
            }
        }
    }
}