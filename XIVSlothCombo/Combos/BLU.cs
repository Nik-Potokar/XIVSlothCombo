using Dalamud.Game.ClientState.Conditions;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BLU
    {
        public const byte JobID = 36;

        public const uint
            Addle = 7560,
            Swiftcast = 7561,
            RoseOfDestruction = 23275,
            ShockStrike = 11429,
            FeatherRain = 11426,
            JKick = 18325,
            Eruption = 11427,
            GlassDance = 11430,
            Surpanakha = 18323,
            Nightbloom = 23290,
            MoonFlute = 11415,
            Whistle = 18309,
            Tingle = 23265,
            TripleTrident = 23264,
            MatraMagic = 23285,
            FinalSting = 11407,
            Bristle = 11393,
            PhantomFlurry = 23288,
            SongOfTorment = 11386;

        public static class Buffs
        {
            public const short
                MoonFlute = 1718,
                Bristle = 1716,
                Tingle = 2492,
                Whistle = 2118;
        }

        public static class Debuffs
        {
            public const short
                SongOfTorment = 273;
        }

        public static class Levels
        {
            public const byte
                LucidDreaming = 24;
        }
    }

    internal class BluBuffedSoT : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BluBuffedSoT;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.SongOfTorment)
            {
                if (!HasEffect(BLU.Buffs.Bristle))
                    return BLU.Bristle;
                return BLU.SongOfTorment;
            }

            return actionID;
        }
    }

    internal class BluOpener : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BluOpener;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.MoonFlute || actionID == BLU.Whistle)
            {
                if (GetCooldown(BLU.TripleTrident).CooldownRemaining < 3)
                {
                    if (!HasEffect(BLU.Buffs.Whistle))
                        return BLU.Whistle;
                    if (!HasEffect(BLU.Buffs.Tingle))
                        return BLU.Tingle;
                    if (!HasEffect(BLU.Buffs.MoonFlute))
                        return BLU.MoonFlute;
                    if (!GetCooldown(BLU.JKick).IsCooldown)
                        return BLU.JKick;
                    if (!GetCooldown(BLU.TripleTrident).IsCooldown)
                        return BLU.TripleTrident;
                }

                if (!HasEffect(BLU.Buffs.Whistle) && !GetCooldown(BLU.JKick).IsCooldown)
                    return BLU.Whistle;
                if (!HasEffect(BLU.Buffs.MoonFlute))
                    return BLU.MoonFlute;
                if (!GetCooldown(BLU.JKick).IsCooldown)
                    return BLU.JKick;
                if (!GetCooldown(BLU.Nightbloom).IsCooldown)
                    return BLU.Nightbloom;
                if (!GetCooldown(BLU.RoseOfDestruction).IsCooldown)
                    return BLU.RoseOfDestruction;
                if (!GetCooldown(BLU.FeatherRain).IsCooldown)
                    return BLU.FeatherRain;
                if (!HasEffect(BLU.Buffs.Bristle) && !GetCooldown(BLU.Swiftcast).IsCooldown)
                    return BLU.Bristle;
                if (!GetCooldown(BLU.Swiftcast).IsCooldown)
                    return BLU.Swiftcast;
                if (!GetCooldown(BLU.GlassDance).IsCooldown)
                    return BLU.GlassDance;
                if (GetCooldown(BLU.Surpanakha).CooldownRemaining < 95)
                    return BLU.Surpanakha;
                if (!GetCooldown(BLU.MatraMagic).IsCooldown)
                    return BLU.MatraMagic;
                if (!GetCooldown(BLU.ShockStrike).IsCooldown)
                    return BLU.ShockStrike;
                return BLU.PhantomFlurry;
            }

            return actionID;
        }
    }

    internal class BluFinalSting : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BluFinalSting;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.FinalSting)
            {

                if (!HasEffect(BLU.Buffs.MoonFlute))
                    return BLU.MoonFlute;
                if (!GetCooldown(BLU.RoseOfDestruction).IsCooldown)
                    return BLU.RoseOfDestruction;
                if (!GetCooldown(BLU.FeatherRain).IsCooldown)
                    return BLU.FeatherRain;
                if (!GetCooldown(BLU.GlassDance).IsCooldown)
                    return BLU.GlassDance;
                if (!GetCooldown(BLU.JKick).IsCooldown)
                    return BLU.JKick;
                if (!HasEffect(BLU.Buffs.Tingle))
                    return BLU.Tingle;
                if (!GetCooldown(BLU.ShockStrike).IsCooldown)
                    return BLU.ShockStrike;
                if (!HasEffect(BLU.Buffs.Whistle))
                    return BLU.Whistle;
                return BLU.FinalSting;
            }

            return actionID;
        }
    }
}