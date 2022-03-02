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
            AngelWhisper = 18317,
            SongOfTorment = 11386,
            RamsVoice = 11419,
            Ultravibration = 23277,
            Devour = 18320,
            Offguard = 11411,
            BadBreath = 11388,
            LucidDreaming = 7562,
            MagicHammer = 18305,
            WhiteKnightsTour = 18310,
            BlackKnightsTour = 18311,
            PeripheralSynthesis = 23286,
            MustardBomb = 23279;

        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                MoonFlute = 1718,
                Bristle = 1716,
                Tingle = 2492,
                Whistle = 2118,
                TankMimicry = 2124,
                DPSMimicry = 2125;
        }

        public static class Debuffs
        {
            public const ushort
                Slow = 9,
                Bind = 13,
                Addle = 1203,
                SongOfTorment = 273,
                DeepFreeze = 1731,
                Offguard = 1717,
                Malodorous = 1715,
                Conked = 2115,
                Lightheaded = 2501;
        }

        public static class Levels
        {
            public const byte
                LucidDreaming = 24;
        }
    }

    internal class BluBuffedSoT : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluBuffedSoT;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluOpener;

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
                if (!GetCooldown(BLU.MatraMagic).IsCooldown && HasEffect(BLU.Buffs.DPSMimicry))
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluFinalSting;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.FinalSting)
            {

                if (!HasEffect(BLU.Buffs.MoonFlute))
                    return BLU.MoonFlute;
                if (IsEnabled(CustomComboPreset.BluPrimals))
                {

                    if (!GetCooldown(BLU.RoseOfDestruction).IsCooldown)
                        return BLU.RoseOfDestruction;
                    if (!GetCooldown(BLU.FeatherRain).IsCooldown)
                        return BLU.FeatherRain;
                    if (!GetCooldown(BLU.GlassDance).IsCooldown)
                        return BLU.GlassDance;
                    if (!GetCooldown(BLU.JKick).IsCooldown)
                        return BLU.JKick;
                }

                if (!HasEffect(BLU.Buffs.Tingle))
                    return BLU.Tingle;
                if (!GetCooldown(BLU.ShockStrike).IsCooldown && IsEnabled(CustomComboPreset.BluPrimals))
                    return BLU.ShockStrike;
                if (!HasEffect(BLU.Buffs.Whistle))
                    return BLU.Whistle;
                if (!GetCooldown(BLU.Swiftcast).IsCooldown)
                    return BLU.Swiftcast;
                return BLU.FinalSting;
            }

            return actionID;
        }
    }

    internal class BluRez : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluRez;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.AngelWhisper)
            {
                var swiftCD = GetCooldown(BLU.Swiftcast);
                var angelCD = GetCooldown(BLU.AngelWhisper);
                if (!swiftCD.IsCooldown && !angelCD.IsCooldown)
                    return BLU.Swiftcast;
            }

            return actionID;
        }
    }

    internal class BluUltravibrationCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluUltravibrate;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.Ultravibration)
            {
                var freezeDebuff = FindTargetEffect(BLU.Debuffs.DeepFreeze);
                var swiftCD = GetCooldown(BLU.Swiftcast);
                var ultraCD = GetCooldown(BLU.Ultravibration);

                if (freezeDebuff is null && !ultraCD.IsCooldown)
                    return BLU.RamsVoice;
                if (freezeDebuff is not null)
                {
                    if (!swiftCD.IsCooldown)
                        return BLU.Swiftcast;
                    return BLU.Ultravibration;
                }
            }

            return actionID;
        }
    }

    internal class BluDebuffCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluDebuffCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.Devour || actionID == BLU.Offguard || actionID == BLU.BadBreath)
            {
                var devourCD = GetCooldown(BLU.Devour);
                var offguardDebuff = FindTargetEffect(BLU.Debuffs.Offguard);
                var offguardCD = GetCooldown(BLU.Offguard);
                var lucidCD = GetCooldown(BLU.LucidDreaming);

                if (offguardDebuff is null && !offguardCD.IsCooldown)
                    return BLU.Offguard;
                if (TargetHasEffect(BLU.Debuffs.Malodorous) && HasEffect(BLU.Buffs.TankMimicry))
                    return BLU.BadBreath;
                if (!devourCD.IsCooldown && HasEffect(BLU.Buffs.TankMimicry))
                    return BLU.Devour;
                if (!lucidCD.IsCooldown && LocalPlayer.CurrentMp <= 9000)
                    return BLU.LucidDreaming;
            }

            return actionID;
        }
    }

    internal class BluAddleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluAddleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.MagicHammer)
            {
                var addleCD = GetCooldown(BLU.Addle);
                var hammerCD = GetCooldown(BLU.MagicHammer);

                if (hammerCD.IsCooldown&& !addleCD.IsCooldown && !TargetHasEffect(BLU.Debuffs.Addle) && !TargetHasEffect(BLU.Debuffs.Conked))
                    return BLU.Addle;
            }

            return actionID;
        }
    }

    internal class BluPrimalFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluPrimalFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.FeatherRain)
            {
                var rainCD = GetCooldown(BLU.FeatherRain);
                var shockCD = GetCooldown(BLU.ShockStrike);
                var glassCD = GetCooldown(BLU.GlassDance);
                var kickCD = GetCooldown(BLU.JKick);
                var roseCD = GetCooldown(BLU.RoseOfDestruction);

                if (!rainCD.IsCooldown)
                    return BLU.FeatherRain;
                if (!shockCD.IsCooldown)
                    return BLU.ShockStrike;
                if (!roseCD.IsCooldown)
                    return BLU.RoseOfDestruction;
                if (!glassCD.IsCooldown)
                    return BLU.GlassDance;
                if (!kickCD.IsCooldown)
                    return BLU.JKick;
            }

            return actionID;
        }
    }

    internal class BluKnightCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluKnightFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.WhiteKnightsTour || actionID == BLU.BlackKnightsTour)
            {
                if (TargetHasEffect(BLU.Debuffs.Slow))
                    return BLU.BlackKnightsTour;
                if (TargetHasEffect(BLU.Debuffs.Bind))
                    return BLU.WhiteKnightsTour;
            }

            return actionID;
        }
    }
    internal class BluLightheadedCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluLightheadedCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLU.PeripheralSynthesis)
            {
                if (!TargetHasEffect(BLU.Debuffs.Lightheaded))
                    return BLU.PeripheralSynthesis;
                if (TargetHasEffect(BLU.Debuffs.Lightheaded))
                    return BLU.MustardBomb;
            }

            return actionID;
        }
    }
}