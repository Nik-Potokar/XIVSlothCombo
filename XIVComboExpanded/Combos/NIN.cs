using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
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
            Jin = 18807;

        public static class Buffs
        {
            public const short
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                AssassinateReady = 1955;
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
                EnhancedKassatsu = 76;
        }
    }


    internal class NinjaAeolianEdgeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaAeolianEdgeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 2255)

            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(2263u) == CustomCombo.OriginalHook(18807u))
                {
                    return CustomCombo.OriginalHook(2260u);
                }
                if (comboTime > 0f)
                {
                    if (lastComboMove == 2240 && level >= 4)
                    {
                        return 2242u;
                    }
                    var huton = GetJobGauge<NINGauge>();
                    if (lastComboMove == NIN.GustSlash && level >= 45 && huton.HutonTimer < 30000)
                    {
                        return NIN.ArmorCrush;
                    }
                    if (lastComboMove == 2242 && level >= 26)
                    {
                        return 2255u;
                    }

                }
                return 2240u;
            }
            return actionID;


        }
    }
    internal class NinjaArmorCrushCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaArmorCrushCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 3563)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(2263u) == CustomCombo.OriginalHook(18807u))
                {
                    return CustomCombo.OriginalHook(2260u);
                }
                if (comboTime > 0f)
                {
                    if (lastComboMove == 2240 && level >= 4)
                    {
                        return 2242u;
                    }
                    if (lastComboMove == 2242 && level >= 54)
                    {
                        return 3563u;
                    }
                }
                return 2240u;
            }
            return actionID;
        }
    }

    internal class NinjaAssassinateFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaAssassinateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 3566 && level >= 60 && CustomCombo.HasEffect(1955))
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
            if (actionID == 16488)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(2263u) == CustomCombo.OriginalHook(18807u))
                {
                    return CustomCombo.OriginalHook(2260u);
                }
                if (comboTime > 0f && lastComboMove == 2254 && level >= 52)
                {
                    return 16488u;
                }
                return 2254u;
            }
            return actionID;
        }
        internal class NinjaHideMugFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.NinjaHideMugFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == 2245)
                {
                    if (CustomCombo.HasEffect(507) || CustomCombo.HasEffect(614))
                    {
                        return 2258u;
                    }
                    if (CustomCombo.HasCondition((Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat)))
                    {
                        return 2248u;
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
            if (actionID == 2261 && level >= 76 && CustomCombo.HasEffect(497))
            {
                return 18807u;
            }
            return actionID;
        }
    }
    internal class NinjaKassatsuTrickFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuTrickFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 2264)
            {
                if (CustomCombo.HasEffect(507) || CustomCombo.HasEffect(614))
                {
                    return 2258u;
                }
                return 2264u;
            }
            return actionID;
        }
    }
    internal class NinjaTCJMeisuiFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 7403)
            {
                if (CustomCombo.HasEffect(507))
                {
                    return 16489u;
                }
                return 7403u;
            }
            return actionID;
        }
    }


}

