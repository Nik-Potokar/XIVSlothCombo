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
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Ninjutsu = 2260,
            Chi = 2261,
            JinNormal = 2263,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Jin = 18807,
            Bunshin = 16493,
            Huraijin = 25876,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778;

        public static class Buffs
        {
            public const short
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                AssassinateReady = 1955,
                RaijuReady = 2690;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                GustSlash = 4,
                AeolianEdge = 26,
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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaAeolianEdgeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.AeolianEdge)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.JinNormal) == CustomCombo.OriginalHook(NIN.Jin))
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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaArmorCrushCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.ArmorCrush)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.JinNormal) == CustomCombo.OriginalHook(NIN.Jin))
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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaAssassinateFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHakkeMujinsatsuCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.HakkeMujinsatsu)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.JinNormal) == CustomCombo.OriginalHook(NIN.Jin))
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
            protected override CustomComboPreset Preset => CustomComboPreset.NinjaHideMugFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuChiJinFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuTrickFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHuraijinRaijuFeature;

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
}
