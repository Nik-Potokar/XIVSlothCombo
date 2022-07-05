using Dalamud.Game.ClientState.Conditions;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
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
            HydroPull = 23282,
            MustardBomb = 23279;

        public static class Buffs
        {
            public const ushort
                MoonFlute = 1718,
                Bristle = 1716,
                WaningNocturne = 1727,
                PhantomFlurry = 2502,
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

        internal class BLU_BuffedSoT : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_BuffedSoT;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SongOfTorment)
                {
                    if (!HasEffect(Buffs.Bristle) && IsSpellActive(Bristle))
                        return Bristle;
                    if (IsSpellActive(SongOfTorment))
                        return SongOfTorment;
                }

                return actionID;
            }
        }

        internal class BLU_Opener : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_Opener;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is MoonFlute or Whistle)
                {
                    //If Triple Trident is saved for Crit/Det builds
                    if (GetCooldownRemainingTime(TripleTrident) <= 3 && IsSpellActive(TripleTrident))
                    {
                        if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle) && !WasLastSpell(Whistle) && IsOffCooldown(JKick))
                            return Whistle;
                        if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle) && IsOffCooldown(JKick))
                            return Tingle;
                        if (!HasEffect(Buffs.MoonFlute) && !HasEffect(Buffs.WaningNocturne) && IsSpellActive(MoonFlute) && !WasLastSpell(MoonFlute))
                            return MoonFlute;
                        if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                            return JKick;
                        if (IsOffCooldown(TripleTrident))
                            return TripleTrident;
                    }

                    //If Triple Trident is used on CD for Crit/Sps builds or Triple Trident isn't active
                    if ((GetCooldownRemainingTime(TripleTrident) > 3 && IsSpellActive(TripleTrident)) || !IsSpellActive(TripleTrident))
                    {
                        if (!HasEffect(Buffs.Whistle) && IsOffCooldown(JKick) && !WasLastSpell(Whistle) && IsSpellActive(Whistle) && IsOffCooldown(JKick))
                            return Whistle;
                        if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle) && IsOffCooldown(JKick))
                            return Tingle;
                        if (!HasEffect(Buffs.MoonFlute) && !HasEffect(Buffs.WaningNocturne) && IsSpellActive(MoonFlute))
                            return MoonFlute;
                        if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                            return JKick;
                    }

                    if (IsOffCooldown(Nightbloom) && IsSpellActive(Nightbloom))
                        return Nightbloom;
                    if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                        return FeatherRain;
                    if (!HasEffect(Buffs.Bristle) && IsOffCooldown(All.Swiftcast) && IsSpellActive(Bristle))
                        return Bristle;
                    if (IsOffCooldown(All.Swiftcast) && LevelChecked(All.Swiftcast))
                        return All.Swiftcast;
                    if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                        return GlassDance;
                    if (GetCooldownRemainingTime(Surpanakha) < 95 && IsSpellActive(Surpanakha))
                        return Surpanakha;
                    if (IsOffCooldown(MatraMagic) && HasEffect(Buffs.DPSMimicry) && IsSpellActive(MatraMagic))
                        return MatraMagic;
                    if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike))
                        return ShockStrike;
                    if ((IsOffCooldown(PhantomFlurry) && IsSpellActive(PhantomFlurry)) || HasEffect(Buffs.PhantomFlurry))
                        return PhantomFlurry;
                }

                return actionID;
            }
        }

        internal class BLU_FinalSting : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_FinalSting;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FinalSting)
                {
                    if (IsEnabled(CustomComboPreset.BLU_SoloMode) && HasCondition(ConditionFlag.BoundByDuty) && !HasEffect(Buffs.BasicInstinct) && GetPartyMembers().Length == 0 && IsSpellActive(BasicInstinct))
                        return BasicInstinct;
                    if (!HasEffect(Buffs.MoonFlute) && !WasLastSpell(MoonFlute) && IsSpellActive(MoonFlute))
                        return MoonFlute;
                    if (IsEnabled(CustomComboPreset.BLU_Primals))
                    {
                        if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                            return RoseOfDestruction;
                        if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                            return FeatherRain;
                        if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                            return GlassDance;
                        if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                            return JKick;
                    }

                    if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle))
                        return Tingle;
                    if (IsOffCooldown(ShockStrike) && IsEnabled(CustomComboPreset.BLU_Primals) && IsSpellActive(ShockStrike))
                        return ShockStrike;
                    if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle) && !WasLastAction(Whistle))
                        return Whistle;
                    if (IsOffCooldown(All.Swiftcast) && LevelChecked(All.Swiftcast))
                        return All.Swiftcast;
                    if (IsSpellActive(FinalSting))
                        return FinalSting;
                }

                return actionID;
            }
        }

        internal class BLU_Ultravibrate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_Ultravibrate;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ultravibration)
                {
                    if (IsEnabled(CustomComboPreset.BLU_HydroPull) && !InMeleeRange() && IsSpellActive(HydroPull))
                        return HydroPull;
                    if (!TargetHasEffectAny(Debuffs.DeepFreeze) && IsOffCooldown(Ultravibration) && IsSpellActive(RamsVoice))
                        return RamsVoice;

                    if (TargetHasEffectAny(Debuffs.DeepFreeze))
                    {
                        if (IsOffCooldown(All.Swiftcast))
                            return All.Swiftcast;
                        if (IsSpellActive(Ultravibration) && IsOffCooldown(Ultravibration))
                            return Ultravibration;
                    }
                }

                return actionID;
            }
        }

        internal class BLU_DebuffCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_DebuffCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Devour or Offguard or BadBreath)
                {
                    if (!TargetHasEffectAny(Debuffs.Offguard) && IsOffCooldown(Offguard) && IsSpellActive(Offguard))
                        return Offguard;
                    if (!TargetHasEffectAny(Debuffs.Malodorous) && HasEffect(Buffs.TankMimicry) && IsSpellActive(BadBreath))
                        return BadBreath;
                    if (IsOffCooldown(Devour) && HasEffect(Buffs.TankMimicry) && IsSpellActive(Devour))
                        return Devour;
                    if (IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= 9000 & LevelChecked(All.LucidDreaming))
                        return All.LucidDreaming;
                }

                return actionID;
            }
        }

        internal class BLU_Addle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_Addle;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is MagicHammer)
                {
                    if (IsOnCooldown(MagicHammer) && IsOffCooldown(All.Addle) && !TargetHasEffect(All.Debuffs.Addle) && !TargetHasEffect(Debuffs.Conked))
                        return All.Addle;
                }

                return actionID;
            }
        }

        internal class BLU_PrimalCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_PrimalCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FeatherRain)
                {
                    if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                        return FeatherRain;
                    if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike))
                        return ShockStrike;
                    if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                        return GlassDance;
                    if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                        return JKick;
                }

                return actionID;
            }
        }

        internal class BLU_KnightCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_KnightCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is WhiteKnightsTour or BlackKnightsTour)
                {
                    if (TargetHasEffect(Debuffs.Slow) && IsSpellActive(BlackKnightsTour))
                        return BlackKnightsTour;
                    if (TargetHasEffect(Debuffs.Bind) && IsSpellActive(WhiteKnightsTour))
                        return WhiteKnightsTour;
                }

                return actionID;
            }
        }

        internal class BLU_LightHeadedCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_LightHeadedCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is PeripheralSynthesis)
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