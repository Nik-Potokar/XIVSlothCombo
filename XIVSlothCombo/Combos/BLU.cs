using Dalamud.Game.ClientState.Conditions;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BLU
    {
        public const byte JobID = 36;

        public const uint
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
            MagicHammer = 18305,
            WhiteKnightsTour = 18310,
            BlackKnightsTour = 18311,
            PeripheralSynthesis = 23286,
            BasicInstinct = 23276,
            MustardBomb = 23279;

        public static class Buffs
        {
            public const ushort
                MoonFlute = 1718,
                Bristle = 1716,
                Tingle = 2492,
                Whistle = 2118,
                TankMimicry = 2124,
                DPSMimicry = 2125,
                BasicInstinct = 2498;
        }

        public static class Debuffs
        {
            public const ushort
                Slow = 9,
                Bind = 13,
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
                Placeholder = 1;
        }


        internal class BluBuffedSoT : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluBuffedSoT;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SongOfTorment)
                {
                    if (!HasEffect(Buffs.Bristle) && IsSpellActive(Bristle))
                        return Bristle;

                    if (IsSpellActive(SongOfTorment))
                    return SongOfTorment;
                }

                return actionID;
            }
        }

        internal class BluOpener : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluOpener;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == MoonFlute || actionID == Whistle)
                {
                    if (GetCooldown(TripleTrident).CooldownRemaining < 3 && IsSpellActive(TripleTrident))
                    {
                        if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle))
                            return Whistle;
                        if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle))
                            return Tingle;
                        if (!HasEffect(Buffs.MoonFlute) && IsSpellActive(MoonFlute))
                            return MoonFlute;
                        if (!GetCooldown(JKick).IsCooldown && IsSpellActive(JKick))
                            return JKick;
                        if (!GetCooldown(TripleTrident).IsCooldown)
                            return TripleTrident;
                    }

                    if (!HasEffect(Buffs.Whistle) && !GetCooldown(JKick).IsCooldown && IsSpellActive(Whistle))
                        return Whistle;
                    if (!HasEffect(Buffs.MoonFlute) && IsSpellActive(MoonFlute))
                        return MoonFlute;
                    if (!GetCooldown(JKick).IsCooldown && IsSpellActive(JKick))
                        return JKick;
                    if (!GetCooldown(Nightbloom).IsCooldown && IsSpellActive(Nightbloom))
                        return Nightbloom;
                    if (!GetCooldown(RoseOfDestruction).IsCooldown && IsSpellActive(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (!GetCooldown(FeatherRain).IsCooldown && IsSpellActive(FeatherRain))
                        return FeatherRain;
                    if (!HasEffect(Buffs.Bristle) && !GetCooldown(All.Swiftcast).IsCooldown && IsSpellActive(Bristle))
                        return Bristle;
                    if (!GetCooldown(All.Swiftcast).IsCooldown)
                        return All.Swiftcast;
                    if (!GetCooldown(GlassDance).IsCooldown && IsSpellActive(GlassDance))
                        return GlassDance;
                    if (GetCooldown(Surpanakha).CooldownRemaining < 95 && IsSpellActive(Surpanakha))
                        return Surpanakha;
                    if (!GetCooldown(MatraMagic).IsCooldown && HasEffect(Buffs.DPSMimicry) && IsSpellActive(MatraMagic))
                        return MatraMagic;
                    if (!GetCooldown(ShockStrike).IsCooldown && IsSpellActive(ShockStrike))
                        return ShockStrike;

                    if (IsSpellActive(PhantomFlurry))
                    return PhantomFlurry;
                }

                return actionID;
            }
        }

        internal class BluFinalSting : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluFinalSting;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == FinalSting)
                {
                    if (IsEnabled(CustomComboPreset.BluSoloMode) && HasCondition(ConditionFlag.BoundByDuty) && !HasEffect(Buffs.BasicInstinct) && GetPartyMembers().Length == 0 && IsSpellActive(BasicInstinct))
                        return BasicInstinct;
                    if (!HasEffect(Buffs.MoonFlute) && IsSpellActive(MoonFlute))
                        return MoonFlute;
                    if (IsEnabled(CustomComboPreset.BluPrimals))
                    {
                        if (!GetCooldown(RoseOfDestruction).IsCooldown && IsSpellActive(RoseOfDestruction))
                            return RoseOfDestruction;
                        if (!GetCooldown(FeatherRain).IsCooldown && IsSpellActive(FeatherRain))
                            return FeatherRain;
                        if (!GetCooldown(GlassDance).IsCooldown && IsSpellActive(GlassDance))
                            return GlassDance;
                        if (!GetCooldown(JKick).IsCooldown && IsSpellActive(JKick))
                            return JKick;
                    }

                    if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle))
                        return Tingle;
                    if (!GetCooldown(ShockStrike).IsCooldown && IsEnabled(CustomComboPreset.BluPrimals) && IsSpellActive(ShockStrike))
                        return ShockStrike;
                    if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle))
                        return Whistle;
                    if (!GetCooldown(All.Swiftcast).IsCooldown)
                        return All.Swiftcast;

                    if(IsSpellActive(FinalSting))
                    return FinalSting;
                }

                return actionID;
            }
        }

        internal class BluUltravibrationCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluUltravibrate;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Ultravibration)
                {
                    var freezeDebuff = FindTargetEffect(Debuffs.DeepFreeze);
                    var swiftCD = GetCooldown(All.Swiftcast);
                    var ultraCD = GetCooldown(Ultravibration);

                    if (freezeDebuff is null && !ultraCD.IsCooldown && IsSpellActive(RamsVoice))
                        return RamsVoice;
                    if (freezeDebuff is not null)
                    {
                        if (!swiftCD.IsCooldown)
                            return All.Swiftcast;
                        if (IsSpellActive(Ultravibration))
                        return Ultravibration;
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
                if (actionID == Devour || actionID == Offguard || actionID == BadBreath)
                {
                    var devourCD = GetCooldown(Devour);
                    var offguardDebuff = FindTargetEffect(Debuffs.Offguard);
                    var offguardCD = GetCooldown(Offguard);
                    var lucidCD = GetCooldown(All.LucidDreaming);

                    if (offguardDebuff is null && !offguardCD.IsCooldown && IsSpellActive(Offguard))
                        return Offguard;
                    if (TargetHasEffect(Debuffs.Malodorous) && HasEffect(Buffs.TankMimicry) && IsSpellActive(BadBreath))
                        return BadBreath;
                    if (!devourCD.IsCooldown && HasEffect(Buffs.TankMimicry) && IsSpellActive(Devour))
                        return Devour;
                    if (!lucidCD.IsCooldown && LocalPlayer.CurrentMp <= 9000 & level >= All.Levels.LucidDreaming)
                        return All.LucidDreaming;
                }

                return actionID;
            }
        }

        internal class BluAddleFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluAddleFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == MagicHammer)
                {
                    var addleCD = GetCooldown(All.Addle);
                    var hammerCD = GetCooldown(MagicHammer);

                    if (hammerCD.IsCooldown && !addleCD.IsCooldown && !TargetHasEffect(All.Debuffs.Addle) && !TargetHasEffect(Debuffs.Conked))
                        return All.Addle;
                }

                return actionID;
            }
        }

        internal class BluPrimalFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluPrimalFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == FeatherRain)
                {
                    var rainCD = GetCooldown(FeatherRain);
                    var shockCD = GetCooldown(ShockStrike);
                    var glassCD = GetCooldown(GlassDance);
                    var kickCD = GetCooldown(JKick);
                    var roseCD = GetCooldown(RoseOfDestruction);

                    if (!rainCD.IsCooldown && IsSpellActive(FeatherRain))
                        return FeatherRain;
                    if (!shockCD.IsCooldown && IsSpellActive(ShockStrike))
                        return ShockStrike;
                    if (!roseCD.IsCooldown && IsSpellActive(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (!glassCD.IsCooldown && IsSpellActive(GlassDance))
                        return GlassDance;
                    if (!kickCD.IsCooldown && IsSpellActive(JKick))
                        return JKick;
                }

                return actionID;
            }
        }

        internal class BluKnightCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluKnightFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WhiteKnightsTour || actionID == BlackKnightsTour)
                {
                    if (TargetHasEffect(Debuffs.Slow) && IsSpellActive(BlackKnightsTour))
                        return BlackKnightsTour;
                    if (TargetHasEffect(Debuffs.Bind) && IsSpellActive(WhiteKnightsTour))
                        return WhiteKnightsTour;
                }

                return actionID;
            }
        }
        internal class BluLightheadedCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BluLightheadedCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == PeripheralSynthesis)
                {
                    if (!TargetHasEffect(Debuffs.Lightheaded) && IsSpellActive(PeripheralSynthesis))
                        return PeripheralSynthesis;
                    if (TargetHasEffect(Debuffs.Lightheaded) && IsSpellActive(MustardBomb))
                        return MustardBomb;
                }

                return actionID;
            }
        }
    }
}