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
                Trick_CooldownRemaining = "TrickCooldownRemaining",
                Huton_RemainingTimer = "HutonRemainingTimer",
                Huton_RemainingArmorCrush = "HutonRemainingArmorCrush",
                Mug_NinkiGauge = "MugNinkiGauge",
                Ninki_BhavaPooling = "NinkiBhavaPooling",
                Ninki_BunshinPooling = "NinkiBunshinPooling";
        }

        internal class NIN_AeolianEdgeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AeolianEdgeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is AeolianEdge)
                {
                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (IsEnabled(CustomComboPreset.NIN_RangedUptime) && !HasEffect(Buffs.Mudra))
                    {
                        if (!InMeleeRange())
                            return ThrowingDaggers;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_GCDsNinjutsu) && (HasEffect(Buffs.Mudra) || HasEffect(Buffs.Kassatsu)))
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_Fleeting))
                    {
                        if (HasEffect(Buffs.RaijuReady))
                            return FleetingRaiju;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_Huraijin) && level >= Levels.Huraijin)
                    {
                        var gauge = GetJobGauge<NINGauge>();

                        if (gauge.HutonTimer <= 0)
                            return Huraijin;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_Bunshin) && level >= Levels.Bunshin)
                    {
                        var gauge = GetJobGauge<NINGauge>();

                        if (gauge.Ninki >= 50 && IsOffCooldown(Bunshin) && CanWeave(actionID))
                            return Bunshin;

                        if (HasEffect(Buffs.PhantomReady) && canWeave && level >= Levels.PhantomKamaitachi)
                            return PhantomKamaitachi;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_Bhavacakra) && level >= Levels.Bhavacakra)
                    {
                        var gauge = GetJobGauge<NINGauge>();

                        if (gauge.Ninki >= 50 && IsOnCooldown(actionID)) //20220606 Tsusai: Not a weave?
                            return Bhavacakra;
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_AssassinateDWAD)
                        && level >= Levels.Assassinate
                        && IsOnCooldown(actionID) 
                        && IsOffCooldown(Assassinate)
                       ) return OriginalHook(Assassinate);

                    
                    if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_Mug) 
                        && level >= Levels.Mug
                        && CanWeave(actionID)
                        && IsOffCooldown(Mug))
                    {
                        var gauge = GetJobGauge<NINGauge>();
                        var mugNinkiValue = GetOptionValue(Config.Mug_NinkiGauge);

                        if ((level >= TraitLevels.Shukiho && gauge.Ninki <= mugNinkiValue)
                            || level < TraitLevels.Shukiho
                            ) return OriginalHook(Mug);
                    }

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= Levels.GustSlash)
                        {
                            return GustSlash;
                        }

                        var huton = GetJobGauge<NINGauge>();
                        var armorcrushTimer = GetOptionValue(Config.Huton_RemainingArmorCrush);

                        if (IsEnabled(CustomComboPreset.NIN_AeolianEdgeCombo_ArmorCrush)
                            && lastComboMove == GustSlash 
                            && level >= Levels.ArmorCrush 
                            && huton.HutonTimer < armorcrushTimer * 1000 
                           ) return ArmorCrush;

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

        internal class NIN_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SpinningEdge)
                {
                    var gauge = GetJobGauge<NINGauge>();
                    var trickCDThreshold = GetOptionValue(Config.Trick_CooldownRemaining);
                    var ninkiBhavaPooling = GetOptionValue(Config.Ninki_BhavaPooling);
                    var ninkiBunshinPooling = GetOptionValue(Config.Ninki_BunshinPooling);

                    if (OriginalHook(Ninjutsu) is Rabbit)
                        return OriginalHook(Ninjutsu);

                    if (HasEffect(Buffs.RaijuReady) && !HasEffect(Buffs.Mudra))
                        return FleetingRaiju;

                    if (level >= Levels.Huraijin && gauge.HutonTimer == 0 && !HasEffect(Buffs.Mudra))
                        return Huraijin;

                    if (IsEnabled(CustomComboPreset.NIN_ST_Simple_Mug) 
                        && level >= Levels.Mug 
                        && CanWeave(actionID)
                        && IsOffCooldown(Mug) 
                        && gauge.Ninki <= 60 
                        && !HasEffect(Buffs.Mudra)
                       ) return OriginalHook(Mug);

                    
                    if (IsEnabled(CustomComboPreset.NIN_ST_Simple_Trick)
                        && level >= Levels.Doton
                        && (IsOffCooldown(TrickAttack) || GetCooldownRemainingTime(TrickAttack) <= trickCDThreshold) 
                        && (!HasEffect(Buffs.Kassatsu) || (HasEffect(Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NIN_ST_Simple_Trick_Kassatsu))) )
                    {
                        if (HasEffect(Buffs.Suiton) && IsOffCooldown(TrickAttack))
                            return TrickAttack;

                        if (!HasEffect(Buffs.Mudra) 
                            && !HasEffect(Buffs.Suiton) 
                            && HasCharges(Chi)
                            || (HasEffect(Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NIN_ST_Simple_Trick_Kassatsu)))
                            return OriginalHook(Chi);

                        if (!HasEffect(Buffs.Suiton) && OriginalHook(Ninjutsu) == FumaShuriken)
                            return OriginalHook(TenCombo);

                        if (!HasEffect(Buffs.Suiton) && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                            return OriginalHook(JinCombo);

                        if (OriginalHook(Ninjutsu) is Suiton && !HasEffect(Buffs.Suiton))
                            return OriginalHook(Ninjutsu);
                    }

                    if (level >= Levels.Kassatsu 
                        && IsOffCooldown(Kassatsu) 
                        && CanWeave(actionID) 
                        && !HasEffect(Buffs.Mudra)
                       ) return Kassatsu;

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

                            if (HasCharges(Jin))
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

                            if (HasCharges(Jin))
                                return Jin;
                        }
                    }

                    //20220606 Tsusai: Pooling option appears to be a slider from 50 to 100. Is it needed as a option if the minimum is indeed 50?
                    if (level >= Levels.Bunshin
                        && IsOffCooldown(Bunshin)
                        && CanWeave(actionID)
                        && ( (!IsEnabled(CustomComboPreset.NIN_NinkiPooling_Bunshin) && gauge.Ninki >= 50) || (gauge.Ninki >= ninkiBunshinPooling) )
                       ) return Bunshin;

                    if (HasEffect(Buffs.PhantomReady) && level >= Levels.PhantomKamaitachi)
                        return PhantomKamaitachi;

                    //Question as the above
                    if (level >= Levels.Bhavacakra
                        && IsOffCooldown(Bhavacakra)
                        && CanWeave(actionID)
                        && ( (!IsEnabled(CustomComboPreset.NIN_NinkiPooling_Bhavacakra) && gauge.Ninki >= 50) || (gauge.Ninki >= ninkiBhavaPooling) )
                       ) return Bhavacakra;

                    if (level >= Levels.Assassinate
                        && IsOffCooldown(Assassinate)
                        && CanWeave(actionID)
                       ) return OriginalHook(Assassinate);

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= Levels.GustSlash)
                            return GustSlash;

                        if (lastComboMove == GustSlash && level >= Levels.AeolianEdge)
                        {
                            if (level >= Levels.ArmorCrush && gauge.HutonTimer < 15000) return ArmorCrush;
                            else return AeolianEdge;
                        }
                    }
                    return SpinningEdge;
                }
                return actionID;
            }
        }

        internal class NIN_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AoE_SimpleMode;

            private static uint lastUsedJutsu { get; set; }

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DeathBlossom)
                {
                    var gauge = GetJobGauge<NINGauge>();

                    lastUsedJutsu = OriginalHook(Ninjutsu) != Ninjutsu ? OriginalHook(Ninjutsu) : lastUsedJutsu;

                    if (level >= Levels.Huraijin && gauge.HutonTimer == 0 && !HasEffect(Buffs.Mudra) && !HasEffect(Buffs.Kassatsu))
                        return Huraijin;

                    //Doton is really annoying. It takes a hot moment for the buff to apply. This is just logic to try and deal with it so it doesn't clip with the rest of the feature.
                    if (OriginalHook(Ninjutsu) == Doton)
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if (IsEnabled(CustomComboPreset.NIN_AoE_Simple_Mudras)
                        && level >= Levels.Doton
                        && (!HasEffect(Buffs.Doton) || GetBuffRemainingTime(Buffs.Doton) <= 5 ) 
                        && (HasCharges(Ten) || HasEffect(Buffs.Mudra)) 
                        && lastUsedJutsu != Doton)
                    {
                        switch (OriginalHook(Ninjutsu))
                        {
                            case Doton:
                                return Doton;
                            case Katon or GokaMekkyaku:
                                return ChiCombo;
                            case FumaShuriken:
                                return TenCombo;
                        }

                        if (HasEffect(Buffs.Kassatsu))
                            return JinCombo;

                        if (!HasEffect(Buffs.Mudra))
                            return Jin;
                    }

                    //20220606 Tsusai: Not sure what's with this below, could this not be simplified / merged with the above? Need a flow chart on this
                    if (IsEnabled(CustomComboPreset.NIN_AoE_Simple_Mudras) 
                        && (HasCharges(Ten) || HasEffect(Buffs.Mudra) || HasEffect(Buffs.Kassatsu) || (IsOffCooldown(Kassatsu)) && level >= Levels.Kassatsu))
                    {
                        if (level >= Levels.Kassatsu && IsOffCooldown(Kassatsu) && !HasEffect(Buffs.Mudra))
                        {
                            return Kassatsu;
                        }

                        switch (OriginalHook(Ninjutsu))
                        {
                            case Katon or GokaMekkyaku:
                                return OriginalHook(Ninjutsu);
                            case FumaShuriken:
                                return TenCombo;
                            default:
                                return OriginalHook(Jin);
                        }
                    }

                    if (!HasEffect(Buffs.Mudra))
                    {
                        if (IsEnabled(CustomComboPreset.NIN_AoE_Simple_Bunshin)
                            && level >= Levels.Bunshin)
                        {
                            if (gauge.Ninki >= 50
                                && IsOffCooldown(Bunshin)
                                && IsOnCooldown(actionID) //20220606 Tsusai: Is this a CanWeave situation?
                               ) return Bunshin;

                            if (level >= Levels.PhantomKamaitachi && HasEffect(Buffs.PhantomReady)) return PhantomKamaitachi;
                        }

                        if (IsEnabled(CustomComboPreset.NIN_AoE_Simple_Hellfrog) 
                            && gauge.Ninki >= 50 
                            && IsOnCooldown(actionID)
                           ) return Hellfrog;

                        if (comboTime > 0f && lastComboMove == DeathBlossom && level >= Levels.HakkeMujinsatsu)
                        {
                            return HakkeMujinsatsu;
                        }

                        return DeathBlossom;
                    }
                }
                return actionID;
            }
        }

        internal class NIN_ArmorCrushCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ArmorCrushCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ArmorCrush)
                {
                    if (IsEnabled(CustomComboPreset.NIN_GCDsNinjutsu) && OriginalHook(Jin) == OriginalHook(JinCombo))
                    {
                        return OriginalHook(Ninjutsu);
                    }

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == SpinningEdge && level >= Levels.GustSlash)
                        {
                            return GustSlash;
                        }

                        if (lastComboMove == GustSlash && level >= Levels.ArmorCrush)
                        {
                            return ArmorCrush;
                        }
                    }
                    return SpinningEdge;
                }
                return actionID;
            }
        }

        internal class NIN_HuraijinArmorCrush : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_HuraijinArmorCrush;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Huraijin && lastComboMove is GustSlash) return ArmorCrush;
                return actionID;
            }
        }

        internal class NIN_HideMug : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_HideMug;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Hide)
                {
                    if (HasEffect(Buffs.Suiton) || HasEffect(Buffs.Hidden))
                    {
                        return TrickAttack;
                    }

                    if (InCombat()) return Mug;
                }

                return actionID;
            }
        }

        internal class NIN_KassatsuChiJin : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuChiJin;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Chi && level >= Levels.EnhancedKassatsu && HasEffect(Buffs.Kassatsu))
                {
                    return Jin;
                }
                return actionID;
            }
        }

        internal class NIN_KassatsuTrick : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuTrick;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Kassatsu)
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

        internal class NIN_TCJMeisui : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.NIN_TCJMeisui;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is TenChiJin)
                {

                    if (HasEffect(Buffs.Suiton))
                        return Meisui;

                    if (HasEffect(Buffs.TenChiJin) && IsEnabled(CustomComboPreset.NIN_TCJ))
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

        internal class NIN_HuraijinRaiju : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_HuraijinRaiju;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Huraijin)
                {
                    if (IsEnabled(CustomComboPreset.NIN_HuraijinRaiju_Fleeting) && level >= Levels.ForkedRaiju && HasEffect(Buffs.RaijuReady))
                        return FleetingRaiju;

                    if (IsEnabled(CustomComboPreset.NIN_HuraijinRaiju_Forked) && level >= Levels.ForkedRaiju && HasEffect(Buffs.RaijuReady))
                        return ForkedRaiju;
                }
                return actionID;
            }
        }

        internal class NIN_Simple_Mudras : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_Simple_Mudras;

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

                                    if (level >= Levels.Chi)
                                        return OriginalHook(ChiCombo);

                                    if (level >= Levels.Jin)
                                        return OriginalHook(JinCombo);
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