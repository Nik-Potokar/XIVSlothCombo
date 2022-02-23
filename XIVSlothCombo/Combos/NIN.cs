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
            Assassinate = 8814,
            ThrowingDaggers = 2247,
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            Bavacakra = 7402,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Bunshin = 16493,
            Huraijin = 25876,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778,

            //Mudras
            Ninjutsu = 2260,

            //-- initial state mudras (the ones with charges)
            Ten = 2259, 
            Chi = 2261,
            Jin = 2263,

            //-- mudras used for combos (the one used while you have the mudra buff)
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
                RaijuReady = 2690;
        }

        public static class Debuffs
        {
            public const ushort
            TrickAttack = 1054;
        }

        public static class Levels
        {
            public const byte
                GustSlash = 4,
                AeolianEdge = 26,
                Ten = 30,
                Chi = 35,
                Jin = 45,
                Assassinate = 40,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54,
                Meisui = 72,
                EnhancedKassatsu = 76,
                Bunshin = 80,
                PhantomKamaitachi = 82,
                ForkedRaiju = 90;
        }
    }

    internal class NinjaAeolianEdgeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAeolianEdgeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.AeolianEdge)
            {
                if (IsEnabled(CustomComboPreset.NinjaRangedUptimeFeature))
                {
                    if (!InMeleeRange(true))
                        return NIN.ThrowingDaggers;
                }
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.Jin) == CustomCombo.OriginalHook(NIN.JinCombo))
                {
                    return CustomCombo.OriginalHook(NIN.Ninjutsu);
                }
                if (IsEnabled(CustomComboPreset.NinjaFleetingRaijuFeature))
                {
                    if (HasEffect(NIN.Buffs.RaijuReady))
                        return NIN.FleetingRaiju;
                }
                if (IsEnabled(CustomComboPreset.NinjaHuraijinFeature) && level >= 60)
                {
                    var gauge = GetJobGauge<NINGauge>();
                    if (gauge.HutonTimer == 0)
                        return NIN.Huraijin;
                }
                if (IsEnabled(CustomComboPreset.NinjaBunshinFeature) && level >= 80)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var bunshinCD = GetCooldown(NIN.Bunshin);
                    if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && actionIDCD.IsCooldown)
                        return NIN.Bunshin;
                }
                if (IsEnabled(CustomComboPreset.NinjaBavacakraFeature) && level >= 80)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var bunshinCD = GetCooldown(NIN.Bunshin);
                    if (gauge.Ninki >= 50 && actionIDCD.IsCooldown)
                        return NIN.Bavacakra;
                }
                // Probably better to use with trick
                if (IsEnabled(CustomComboPreset.NinjaDreamWithinADream) && level >= 40)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var assasinateCD = GetCooldown(NIN.Assassinate);
                    var dreamCD = GetCooldown(NIN.DreamWithinADream);
                    if (actionIDCD.IsCooldown && !dreamCD.IsCooldown && level >= 56)
                        return OriginalHook(NIN.DreamWithinADream);
                    if (actionIDCD.IsCooldown && !assasinateCD.IsCooldown && level >= 40 && level <= 55)
                        return OriginalHook(NIN.Assassinate);

                }
                if (comboTime > 0f)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= 4)
                    {
                        return NIN.GustSlash;
                    }

                    var huton = GetJobGauge<NINGauge>();
                    if (lastComboMove == NIN.GustSlash && level >= 20 && huton.HutonTimer < 15000 && IsEnabled(CustomComboPreset.NinjaArmorCrushOnMainCombo) && level >= 54)
                    {
                        return NIN.ArmorCrush;
                    }

                    if (lastComboMove == NIN.GustSlash && level >= 26)
                    {
                        return NIN.AeolianEdge;
                    }
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }

    internal class NinjaArmorCrushCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaArmorCrushCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.ArmorCrush)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.Jin) == CustomCombo.OriginalHook(NIN.JinCombo))
                {
                    return CustomCombo.OriginalHook(NIN.Ninjutsu);
                }

                if (comboTime > 0f)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= 4)
                    {
                        return NIN.GustSlash;
                    }

                    if (lastComboMove == NIN.GustSlash && level >= 54)
                    {
                        return NIN.ArmorCrush;
                    }
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }

    internal class NinjaAssassinateFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAssassinateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.DreamWithinADream && level >= 60 && CustomCombo.HasEffect(NIN.Buffs.AssassinateReady))
            {
                return 2246u;
            }

            return actionID;
        }
    }

    internal class NinjaHakkeMujinsatsuCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHakkeMujinsatsuCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.HakkeMujinsatsu)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.Jin) == CustomCombo.OriginalHook(NIN.JinCombo))
                {
                    return CustomCombo.OriginalHook(NIN.Ninjutsu);
                }

                if (comboTime > 0f && lastComboMove == NIN.DeathBlossom && level >= 52)
                {
                    return NIN.HakkeMujinsatsu;
                }

                return NIN.DeathBlossom;
            }

            return actionID;
        }

        internal class NinjaHideMugFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NIN.Hide)
                {
                    if (CustomCombo.HasEffect(NIN.Buffs.Suiton) || CustomCombo.HasEffect(NIN.Buffs.Hidden))
                    {
                        return NIN.TrickAttack;
                    }

                    if (CustomCombo.HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                    {
                        return NIN.Mug;
                    }
                }

                return actionID;
            }
        }
    }

    internal class NinjaKassatsuChiJinFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Chi && level >= 76 && CustomCombo.HasEffect(NIN.Buffs.Kassatsu))
            {
                return NIN.Jin;
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuTrickFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Kassatsu)
            {
                if (CustomCombo.HasEffect(NIN.Buffs.Suiton) || CustomCombo.HasEffect(NIN.Buffs.Hidden))
                {
                    return NIN.TrickAttack;
                }

                return NIN.Kassatsu;
            }

            return actionID;
        }
    }

    internal class NinjaTCJMeisuiFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaTCJMeisuiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.TenChiJin)
            {
                if (CustomCombo.HasEffect(NIN.Buffs.Suiton))
                {
                    return NIN.Meisui;
                }

                return NIN.TenChiJin;
            }

            return actionID;
        }
    }

    internal class NinjaHuraijinRaijuFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHuraijinRaijuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Huraijin)
            {
                if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature1) && level >= NIN.Levels.ForkedRaiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.FleetingRaiju;

                if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature2) && level >= NIN.Levels.ForkedRaiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.ForkedRaiju;
            }

            return actionID;
        }
    }

    internal class NinjaSimpleMudras : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaSimpleMudras;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is NIN.Ten or NIN.Chi or NIN.Jin )
            {
                if (HasEffect(NIN.Buffs.Mudra)) 
                {
                    if (level >= NIN.Levels.Ten && actionID == NIN.Ten)
                    {
                        if (level >= NIN.Levels.Chi && (OriginalHook(NIN.Ninjutsu) is NIN.Hyoton or NIN.HyoshoRanryu))
                        {
                            return OriginalHook(NIN.ChiCombo);
                        }
                        if (level >= NIN.Levels.Jin && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                        {
                            return OriginalHook(NIN.JinCombo);
                        }
                    }

                    if (level >= NIN.Levels.Chi && actionID == NIN.Chi)
                    {
                        if (level >= NIN.Levels.Jin && (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku))
                        {
                            return OriginalHook(NIN.JinCombo);
                        }
                        if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                        {
                            return OriginalHook(NIN.TenCombo);
                        }
                    }

                    if (level >= NIN.Levels.Jin && actionID == NIN.Jin)
                    {
                        if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                        {
                            return OriginalHook(NIN.TenCombo);
                        }
                        if (level >= NIN.Levels.Chi && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                        {
                            return OriginalHook(NIN.ChiCombo);
                        }
                    }

                    return OriginalHook(NIN.Ninjutsu);
                }
            }

            return OriginalHook(actionID);
        }
    }
}
