using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class NIN
    {
        public const byte ClassID = 18;
        public const byte JobID = 30;

        public const uint
            SpinningEdge = 2240,
            GustSlash = 2242,
            Hide = 2245,
            Assassinate = 2246,
            ThrowingDaggers = 2247,
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            Bhavacakra = 7402,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Bunshin = 16493,
            Huraijin = 25876,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778,
            Hellfrog = 7401,

            //Mudras
            Ninjutsu = 2260,
            Rabbit = 2272,

            //-- initial state mudras (the ones with charges)
            Ten = 2259,
            Chi = 2261,
            Jin = 2263,

            //-- mudras used for combos (the ones used while you have the mudra buff)
            TenCombo = 18805,
            ChiCombo = 18806,
            JinCombo = 18807,

            //Ninjutsu
            FumaShuriken = 2265,
            Hyoton = 2268,
            Doton = 2270,
            Katon = 2266,
            Suiton = 2271,
            Raiton = 2267,
            Huton = 2269,
            GokaMekkyaku = 16491,
            HyoshoRanryu = 16492;

        public static class Buffs
        {
            public const ushort
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                TenChiJin = 1186,
                AssassinateReady = 1955,
                RaijuReady = 2690,
                PhantomReady = 2723,
                Doton = 501;
        }

        public static class Debuffs
        {
            public const ushort
            TrickAttack = 1054;
        }

        public static class Levels
        {
            public const byte
                SpinningEdge = 1,
                GustSlash = 4,
                Mug = 15,
                AeolianEdge = 26,
                Ten = 30,
                Chi = 35,
                Jin = 45,
                Doton = 45,
                Assassinate = 40,
                Kassatsu = 50,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54,
                Huraijin = 60,
                Bhavacakra = 68,
                Meisui = 72,
                EnhancedKassatsu = 76,
                Bunshin = 80,
                PhantomKamaitachi = 82,
                ForkedRaiju = 90;
        }

        public static class TraitLevels
        {
            public const byte
                Shukiho = 66;
        }

        public static class Config
        {
            public const string
                TrickCooldownRemaining = "TrickCooldownRemaining",
                HutonRemainingTimer = "HutonRemainingTimer",
                HutonRemainingArmorCrush = "HutonRemainingArmorCrush",
                MugNinkiGauge = "MugNinkiGauge",
                NinkiBhavaPooling = "NinkiBhavaPooling",
                NinkiBunshinPooling = "NinkiBunshinPooling";
        }

        internal class NinjaAeolianEdgeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAeolianEdgeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AeolianEdge)
                {
                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (IsEnabled(CustomComboPreset.NinjaRangedUptimeFeature) && !HasEffect(Buffs.Mudra))
                    {
                        if (!InMeleeRange())
                            return ThrowingDaggers;
                    }

                    if (IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && (HasEffect(Buffs.Mudra) || HasEffect(Buffs.Kassatsu)))
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if (IsEnabled(CustomComboPreset.NinjaFleetingRaijuFeature))
                    {
                        if (HasEffect(Buffs.RaijuReady))
                            return FleetingRaiju;
                    }

                    if (IsEnabled(CustomComboPreset.NinjaHuraijinFeature) && level >= Levels.Huraijin)
                    {
                        var gauge = GetJobGauge<NINGauge>();

                        if (gauge.HutonTimer <= 0)
                            return Huraijin;
                    }

                    if (IsEnabled(CustomComboPreset.NinjaBunshinFeature) && level >= Levels.Bunshin)
                    {
                        var canWeave = CanWeave(actionID);
                        var gauge = GetJobGauge<NINGauge>();
                        var bunshinCD = GetCooldown(Bunshin);

                        if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && canWeave)
                            return Bunshin;

                        if (HasEffect(Buffs.PhantomReady) && canWeave && level >= Levels.PhantomKamaitachi)
                            return PhantomKamaitachi;
                    }

                    if (IsEnabled(CustomComboPreset.NinjaBhavacakraFeature) && level >= Levels.Bhavacakra)
                    {
                        var actionIDCD = GetCooldown(actionID);
                        var gauge = GetJobGauge<NINGauge>();

                        if (gauge.Ninki >= 50 && actionIDCD.IsCooldown)
                            return Bhavacakra;
                    }

                    if (IsEnabled(CustomComboPreset.NinAeolianAssassinateFeature) && level >= Levels.Assassinate)
                    {
                        var actionIDCD = GetCooldown(actionID);
                        var assasinateCD = GetCooldown(Assassinate);

                        if (actionIDCD.IsCooldown && !assasinateCD.IsCooldown)
                            return OriginalHook(Assassinate);
                    }

                    if (IsEnabled(CustomComboPreset.NinAeolianMugFeature) && level >= Levels.Mug)
                    {
                        var canWeave = CanWeave(actionID);
                        var gauge = GetJobGauge<NINGauge>();
                        var mugCD = GetCooldown(Mug);
                        var mugNinkiValue = Service.Configuration.GetCustomIntValue(Config.MugNinkiGauge);

                        if (!mugCD.IsCooldown && gauge.Ninki <= mugNinkiValue && canWeave && level >= TraitLevels.Shukiho)
                            return OriginalHook(Mug);

                        if (!mugCD.IsCooldown && canWeave && level < TraitLevels.Shukiho)
                            return OriginalHook(Mug);
                    }

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= Levels.GustSlash)
                        {
                            return GustSlash;
                        }

                        var huton = GetJobGauge<NINGauge>();
                        var armorcrushTimer = Service.Configuration.GetCustomIntValue(Config.HutonRemainingArmorCrush);

                        if (lastComboMove == GustSlash && level >= Levels.ArmorCrush && huton.HutonTimer < armorcrushTimer * 1000 && IsEnabled(CustomComboPreset.NinjaArmorCrushOnMainCombo))
                        {
                            return ArmorCrush;
                        }

                        if (lastComboMove == GustSlash && level >= Levels.AeolianEdge)
                        {
                            return AeolianEdge;
                        }
                    }

                    return SpinningEdge;
                }

                return actionID;
            }
        }

        internal class SimpleNinjaSingleTarget : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinSimpleSingleTarget;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SpinningEdge)
                {
                    var canWeave = CanWeave(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var bunshinCD = GetCooldown(Bunshin);
                    var trickCDThreshold = Service.Configuration.GetCustomIntValue(Config.TrickCooldownRemaining);
                    var ninkiBhavaPooling = Service.Configuration.GetCustomIntValue(Config.NinkiBhavaPooling);
                    var ninkiBunshinPooling = Service.Configuration.GetCustomIntValue(Config.NinkiBunshinPooling);

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (HasEffect(Buffs.RaijuReady) && !HasEffect(Buffs.Mudra))
                        return FleetingRaiju;

                    if (level >= Levels.Huraijin && gauge.HutonTimer == 0 && !HasEffect(Buffs.Mudra))
                        return Huraijin;

                    if (level >= Levels.Mug && IsEnabled(CustomComboPreset.NinSimpleMug))
                    {
                        var mugCD = GetCooldown(Mug);

                        if (canWeave && !mugCD.IsCooldown && gauge.Ninki <= 60 && !HasEffect(Buffs.Mudra))
                            return OriginalHook(Mug);
                    }

                    if ((!GetCooldown(TrickAttack).IsCooldown || GetCooldown(TrickAttack).CooldownRemaining <= trickCDThreshold) && (!HasEffect(Buffs.Kassatsu) || (HasEffect(Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleTrickKassatsuFeature))) && level >= Levels.Doton && IsEnabled(CustomComboPreset.NinSimpleTrickFeature))
                    {
                        if (HasEffect(Buffs.Suiton) && !GetCooldown(TrickAttack).IsCooldown)
                            return TrickAttack;

                        if (!HasEffect(Buffs.Mudra) && !HasEffect(Buffs.Suiton) && (GetCooldown(Chi).RemainingCharges > 0 || (HasEffect(Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleTrickKassatsuFeature))))
                            return OriginalHook(Chi);

                        if (!HasEffect(Buffs.Suiton) && OriginalHook(Ninjutsu) == FumaShuriken)
                            return OriginalHook(TenCombo);

                        if (!HasEffect(Buffs.Suiton) && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                            return OriginalHook(JinCombo);

                        if (OriginalHook(Ninjutsu) is Suiton && !HasEffect(Buffs.Suiton))
                            return OriginalHook(Ninjutsu);
                    }

                    if (!GetCooldown(Kassatsu).IsCooldown && CanWeave(actionID) && !HasEffect(Buffs.Mudra) && level >= Levels.Kassatsu)
                        return Kassatsu;

                    if (level >= Levels.EnhancedKassatsu)
                    {
                        if (!HasEffect(Buffs.Kassatsu))
                        {
                            if (OriginalHook(Ninjutsu) == Raiton)
                                return OriginalHook(Ninjutsu);

                            if (OriginalHook(Ninjutsu) == FumaShuriken)
                                return OriginalHook(ChiCombo);

                            if (HasEffect(Buffs.Kassatsu))
                                return JinCombo;

                            if (GetCooldown(Jin).RemainingCharges > 0)
                                return Jin;
                        }
                        else
                        {
                            if (OriginalHook(Ninjutsu) is HyoshoRanryu or Raiton)
                                return OriginalHook(Ninjutsu);

                            if (OriginalHook(Ninjutsu) == FumaShuriken)
                                return OriginalHook(JinCombo);
                            return OriginalHook(Ten);
                        }
                    }
                    else
                    {
                        if (OriginalHook(Ninjutsu) == Raiton)
                            return OriginalHook(Ninjutsu);

                        if (OriginalHook(Ninjutsu) == FumaShuriken)
                        {
                            if (level < Levels.Chi)
                                return OriginalHook(Ninjutsu);

                            return OriginalHook(ChiCombo);
                        }

                        if (level >= Levels.Jin)
                        {
                            if (HasEffect(Buffs.Kassatsu))
                                return JinCombo;

                            if (GetCooldown(Jin).RemainingCharges > 0)
                                return Jin;
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.NinNinkiBunshinPooling))
                    {
                        if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && canWeave && level >= Levels.Bunshin)
                            return Bunshin;
                    }
                    else
                    {
                        if (gauge.Ninki >= ninkiBunshinPooling && !bunshinCD.IsCooldown && canWeave && level >= Levels.Bunshin)
                            return Bunshin;
                    }

                    if (HasEffect(Buffs.PhantomReady) && level >= Levels.PhantomKamaitachi)
                        return PhantomKamaitachi;

                    if (!IsEnabled(CustomComboPreset.NinNinkiBhavacakraPooling))
                    {
                        if (gauge.Ninki >= 50 && canWeave && level >= Levels.Bhavacakra)
                            return Bhavacakra;
                    }
                    else
                    {
                        if (gauge.Ninki >= ninkiBhavaPooling && canWeave && level >= Levels.Bhavacakra)
                            return Bhavacakra;
                    }

                    if (level >= Levels.Assassinate)
                    {
                        var assasinateCD = GetCooldown(OriginalHook(Assassinate));

                        if (canWeave && !assasinateCD.IsCooldown)
                            return OriginalHook(Assassinate);
                    }

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= Levels.GustSlash)
                            return GustSlash;

                        if (lastComboMove == GustSlash && gauge.HutonTimer < 15000 && level >= Levels.ArmorCrush)
                            return ArmorCrush;

                        if (lastComboMove == GustSlash && level >= Levels.AeolianEdge)
                            return AeolianEdge;
                    }
                    return SpinningEdge;
                }
                return actionID;
            }
        }

        internal class SimpleNinjaAoE : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinSimpleAoE;

            private static uint lastUsedJutsu { get; set; }

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DeathBlossom)
                {
                    var dotonBuff = FindEffect(Buffs.Doton);
                    var jutsuCooldown = GetCooldown(Ten);
                    var jutsuCharges = jutsuCooldown.RemainingCharges;
                    var gauge = GetJobGauge<NINGauge>();

                    lastUsedJutsu = OriginalHook(Ninjutsu) != Ninjutsu ? OriginalHook(Ninjutsu) : lastUsedJutsu;                    

                    if (gauge.HutonTimer == 0 && !HasEffect(Buffs.Mudra) && !HasEffect(Buffs.Kassatsu) && level >= Levels.Huraijin)
                        return Huraijin;

                    //Doton is really annoying. It takes a hot moment for the buff to apply. This is just logic to try and deal with it so it doesn't clip with the rest of the feature.
                    if (OriginalHook(Ninjutsu) == Doton)
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if ((!HasEffect(Buffs.Doton) || dotonBuff is { RemainingTime: <= 5 }) && (jutsuCharges > 0 || HasEffect(Buffs.Mudra)) && level >= Levels.Doton && lastUsedJutsu != Doton && IsEnabled(CustomComboPreset.NinSimpleAoeMudras))
                    {
                        if (OriginalHook(Ninjutsu) == Doton)
                        {
                            return Doton;
                        }

                        if (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku)
                        {
                            return ChiCombo;
                        }

                        if (OriginalHook(Ninjutsu) == FumaShuriken)
                        {
                            return TenCombo;
                        }

                        if (HasEffect(Buffs.Kassatsu))
                            return JinCombo;

                        if (!HasEffect(Buffs.Mudra))
                            return Jin;
                    }

                    if ((jutsuCharges > 0 || HasEffect(Buffs.Mudra) || HasEffect(Buffs.Kassatsu) || (!GetCooldown(Kassatsu).IsCooldown) && level >= Levels.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleAoeMudras))
                    {
                        if (!GetCooldown(Kassatsu).IsCooldown && !HasEffect(Buffs.Mudra) && level >= Levels.Kassatsu)
                        {
                            return Kassatsu;
                        }

                        if (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku)
                            return OriginalHook(Ninjutsu);

                        if (OriginalHook(Ninjutsu) == FumaShuriken)
                            return TenCombo;

                        if (HasEffect(Buffs.Kassatsu))
                            return JinCombo;
                    }

                    if (!HasEffect(Buffs.Mudra))
                    {
                        var actionIDCD = GetCooldown(actionID);
                        var bunshinCD = GetCooldown(Bunshin);

                        if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && actionIDCD.IsCooldown && level >= Levels.Bunshin && IsEnabled(CustomComboPreset.NinSimpleAoeBunshin))
                            return Bunshin;

                        if (HasEffect(Buffs.PhantomReady) && level >= Levels.PhantomKamaitachi && IsEnabled(CustomComboPreset.NinSimpleAoeBunshin))
                            return PhantomKamaitachi;

                        if (gauge.Ninki >= 50 && actionIDCD.IsCooldown && IsEnabled(CustomComboPreset.NinSimpleHellfrogFeature))
                            return Hellfrog;

                        if (comboTime > 0f && lastComboMove == DeathBlossom && level >= 52)
                        {
                            return HakkeMujinsatsu;
                        }

                        return DeathBlossom;
                    }
                }
                return actionID;
            }
        }

        internal class NinjaArmorCrushCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaArmorCrushCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ArmorCrush)
                {
                    if (IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && OriginalHook(Jin) == OriginalHook(JinCombo))
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= 4)
                        {
                            return GustSlash;
                        }

                        if (lastComboMove == GustSlash && level >= 54)
                        {
                            return ArmorCrush;
                        }
                    }
                    return SpinningEdge;
                }
                return actionID;
            }
        }

        internal class NinHuraijinArmorCrush : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinHuraijinArmorCrush;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Huraijin)
                {
                    if (lastComboMove == GustSlash)
                        return ArmorCrush;
                }
                return actionID;
            }
        }

        internal class NinjaHideMugFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Hide)
                {
                    if (HasEffect(Buffs.Suiton) || HasEffect(Buffs.Hidden))
                    {
                        return TrickAttack;
                    }

                    if (HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                    {
                        return Mug;
                    }
                }

                return actionID;
            }
        }

        internal class NinjaKassatsuChiJinFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Chi && level >= 76 && HasEffect(Buffs.Kassatsu))
                {
                    return Jin;
                }
                return actionID;
            }
        }

        internal class NinjaKassatsuTrickFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Kassatsu)
                {
                    if (HasEffect(Buffs.Suiton) || HasEffect(Buffs.Hidden))
                    {
                        return TrickAttack;
                    }
                    return Kassatsu;
                }
                return actionID;
            }
        }

        internal class NinjaTCJMeisuiFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == TenChiJin)
                {

                    if (HasEffect(Buffs.Suiton))
                        return Meisui;

                    if (HasEffect(Buffs.TenChiJin) && IsEnabled(CustomComboPreset.NinTCJFeature))
                    {
                        var tcjTimer = FindEffectAny(Buffs.TenChiJin).RemainingTime;

                        if (tcjTimer > 5)
                            return OriginalHook(Ten);

                        if (tcjTimer > 4)
                            return OriginalHook(Chi);

                        if (tcjTimer > 3)
                            return OriginalHook(Jin);
                    }
                }
                return actionID;
            }
        }

        internal class NinjaHuraijinRaijuFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHuraijinRaijuFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Huraijin)
                {
                    if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature1) && level >= Levels.ForkedRaiju && HasEffect(Buffs.RaijuReady))
                        return FleetingRaiju;

                    if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature2) && level >= Levels.ForkedRaiju && HasEffect(Buffs.RaijuReady))
                        return ForkedRaiju;
                }
                return actionID;
            }
        }

        internal class NinjaSimpleMudras : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaSimpleMudras;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ten or Chi or Jin)
                {
                    var mudrapath = Service.Configuration.MudraPathSelection;

                    if (HasEffect(Buffs.Mudra))
                    {
                        if (mudrapath == 0)
                        {
                            if (level >= Levels.Ten && actionID == Ten)
                            {
                                if (level >= Levels.Chi && (OriginalHook(Ninjutsu) is Hyoton or HyoshoRanryu))
                                {
                                    return OriginalHook(ChiCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (level >= Levels.Jin)
                                        return OriginalHook(JinCombo);

                                    else if (level >= Levels.Chi)
                                        return OriginalHook(ChiCombo);
                                }
                            }

                            if (level >= Levels.Chi && actionID == Chi)
                            {
                                if (level >= Levels.Jin && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                                {
                                    return OriginalHook(JinCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(TenCombo);
                                }
                            }

                            if (level >= Levels.Jin && actionID == Jin)
                            {
                                if (OriginalHook(Ninjutsu) == Raiton)
                                {
                                    return OriginalHook(TenCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(ChiCombo);
                                }
                            }

                            return OriginalHook(Ninjutsu);
                        }


                        if (mudrapath == 1)
                        {
                            if (level >= Levels.Ten && actionID == Ten)
                            {
                                if (level >= Levels.Jin && (OriginalHook(Ninjutsu) is Raiton))
                                {
                                    return OriginalHook(JinCombo);
                                }

                                if (level >= Levels.Chi && (OriginalHook(Ninjutsu) is HyoshoRanryu))
                                {
                                    return OriginalHook(ChiCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (HasEffect(Buffs.Kassatsu) && level >= Levels.EnhancedKassatsu)
                                        return JinCombo;

                                    if (level >= Levels.Jin)
                                        return OriginalHook(JinCombo);
                                    
                                    if (level >= Levels.Chi)
                                        return OriginalHook(ChiCombo);
                                }
                            }

                            if (level >= Levels.Chi && actionID == Chi)
                            {
                                if (OriginalHook(Ninjutsu) is Hyoton)
                                {
                                    return OriginalHook(TenCombo);
                                }

                                if (level >= Levels.Jin && OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(JinCombo);
                                }
                            }

                            if (level >= Levels.Jin && actionID == Jin)
                            {
                                if (OriginalHook(Ninjutsu) is GokaMekkyaku or Katon)
                                {
                                    return OriginalHook(ChiCombo);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(TenCombo);
                                }
                            }

                            return OriginalHook(Ninjutsu);
                        }

                        if (mudrapath == 2)
                        {
                            if (level >= Levels.Ten && actionID == Ten)
                            {
                                if (level >= Levels.Chi && (OriginalHook(Ninjutsu) is Hyoton or HyoshoRanryu))
                                {
                                    return OriginalHook(Chi);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (level >= Levels.Jin)
                                        return OriginalHook(JinCombo);

                                    else if (level >= Levels.Chi)
                                        return OriginalHook(ChiCombo);
                                }
                            }

                            if (level >= Levels.Chi && actionID == Chi)
                            {
                                if (level >= Levels.Jin && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                                {
                                    return OriginalHook(Jin);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    return OriginalHook(Ten);
                                }
                            }

                            if (level >= Levels.Jin && actionID == Jin)
                            {
                                if (OriginalHook(Ninjutsu) is Raiton)
                                {
                                    return OriginalHook(Ten);
                                }

                                if (OriginalHook(Ninjutsu) == GokaMekkyaku)
                                {
                                    return OriginalHook(Chi);
                                }

                                if (OriginalHook(Ninjutsu) == FumaShuriken)
                                {
                                    if (HasEffect(Buffs.Kassatsu) && level >= Levels.EnhancedKassatsu)
                                        return OriginalHook(Ten);
                                    return OriginalHook(Chi);
                                }
                            }

                            return OriginalHook(Ninjutsu);
                        }
                    }
                }
                return actionID;
            }
        }
    }
}
