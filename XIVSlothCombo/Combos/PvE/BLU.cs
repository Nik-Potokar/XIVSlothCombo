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
            SharpenedKnife = 11400,
            GlassDance = 11430,
            SonicBoom = 18308,
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
            PerpetualRay = 18314,
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
            MustardBomb = 23279,
            WingedReprobation = 34576,
            SeaShanty = 34580,
            BeingMortal = 34582,
            BreathOfMagic = 34567,
            MortalFlame = 34579,
            PeatPelt = 34569,
            DeepClean = 34570;

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
                BasicInstinct = 2498,
                WingedReprobation = 3640;
        }

        public static class Debuffs
        {
            public const ushort
                Slow = 9,
                Bind = 13,
                Stun = 142,
                SongOfTorment = 273,
                DeepFreeze = 1731,
                Offguard = 1717,
                Malodorous = 1715,
                Conked = 2115,
                Lightheaded = 2501,
                MortalFlame = 3643,
                BreathOfMagic = 3712,
                Begrimed = 3636;
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
                if (actionID is MoonFlute)
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
                    if (IsOffCooldown(Eruption) && IsSpellActive(Eruption))
                        return Eruption;
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
                    if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle) && !WasLastAction(Whistle))
                        return Whistle;
                    if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle))
                        return Tingle;
                    if (!HasEffect(Buffs.MoonFlute) && !WasLastSpell(MoonFlute) && IsSpellActive(MoonFlute))
                        return MoonFlute;
                    if (IsEnabled(CustomComboPreset.BLU_Primals))
                    {
                        if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                            return RoseOfDestruction;
                        if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                            return FeatherRain;
                        if (IsOffCooldown(Eruption) && IsSpellActive(Eruption))
                            return Eruption;
                        if (IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                            return MatraMagic;
                        if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                            return GlassDance;
                        if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike))
                            return ShockStrike;
                    }

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
                    if (IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= 9000 && LevelChecked(All.LucidDreaming))
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
                return (actionID is MagicHammer && IsOnCooldown(MagicHammer) && IsOffCooldown(All.Addle) && !TargetHasEffect(All.Debuffs.Addle) && !TargetHasEffect(Debuffs.Conked)) ? All.Addle : actionID;
            }
        }

        internal class BLU_PrimalCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_PrimalCombo;
            internal static bool surpanakhaReady = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FeatherRain or Eruption)
                {
                    if (HasEffect(Buffs.PhantomFlurry))
                        return OriginalHook(PhantomFlurry);

                    if (!HasEffect(Buffs.PhantomFlurry))
                    {
                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_WingedReprobation) && FindEffect(Buffs.WingedReprobation)?.StackCount > 1 && IsOffCooldown(WingedReprobation))
                            return OriginalHook(WingedReprobation);

                        if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                            return FeatherRain;
                        if (IsOffCooldown(Eruption) && IsSpellActive(Eruption) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                            return Eruption;
                        if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 60 || IsOffCooldown(Nightbloom)))))
                            return ShockStrike;
                        if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                            return RoseOfDestruction;
                        if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 90 || IsOffCooldown(Nightbloom)))))
                            return GlassDance;
                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_JKick) && IsOffCooldown(JKick) && IsSpellActive(JKick) &&
                            (IsNotEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) || (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 60 || IsOffCooldown(Nightbloom)))))
                            return JKick;
                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Nightbloom) && IsOffCooldown(Nightbloom) && IsSpellActive(Nightbloom))
                            return Nightbloom;
                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Matra) && IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                            return MatraMagic;
                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_Suparnakha) && IsSpellActive(Surpanakha))
                        {
                            if (GetRemainingCharges(Surpanakha) == 4) surpanakhaReady = true;
                            if (surpanakhaReady && GetRemainingCharges(Surpanakha) > 0) return Surpanakha;
                            if (GetRemainingCharges(Surpanakha) == 0) surpanakhaReady = false;
                        }

                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_WingedReprobation) && IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation))
                            return OriginalHook(WingedReprobation);

                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_SeaShanty) && IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                            return SeaShanty;

                        if (IsEnabled(CustomComboPreset.BLU_PrimalCombo_PhantomFlurry) && IsOffCooldown(PhantomFlurry) && IsSpellActive(PhantomFlurry))
                            return PhantomFlurry;
                    }
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

        internal class BLU_PerpetualRayStunCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_PerpetualRayStunCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return (actionID is PerpetualRay && (TargetHasEffectAny(Debuffs.Stun) || WasLastAction(PerpetualRay)) && IsSpellActive(SharpenedKnife) && InMeleeRange()) ? SharpenedKnife : actionID;
            }
        }

        internal class BLU_MeleeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_MeleeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return (actionID is SonicBoom && GetTargetDistance() <= 3 && IsSpellActive(SharpenedKnife)) ? SharpenedKnife : actionID;
            }
        }

        internal class BLU_PeatClean : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_PeatClean;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is DeepClean)
                {
                    if (IsSpellActive(PeatPelt) && !TargetHasEffect(Debuffs.Begrimed))
                        return PeatPelt;
                }

                return actionID;
            }
        }
        internal class BLU_NewMoonFluteOpener : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLU_NewMoonFluteOpener;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is MoonFlute)
                {
                    if (!HasEffect(Buffs.MoonFlute))
                    {
                        if (IsSpellActive(Whistle) && !HasEffect(Buffs.Whistle) && !WasLastAction(Whistle))
                            return Whistle;

                        if (IsSpellActive(Tingle) && !HasEffect(Buffs.Tingle))
                            return Tingle;

                        if (IsSpellActive(RoseOfDestruction) && GetCooldown(RoseOfDestruction).CooldownRemaining < 1f)
                            return RoseOfDestruction;

                        if (IsSpellActive(MoonFlute))
                            return MoonFlute;
                    }

                    if (IsSpellActive(JKick) && IsOffCooldown(JKick))
                        return JKick;

                    if (IsSpellActive(TripleTrident) && IsOffCooldown(TripleTrident))
                        return TripleTrident;

                    if (IsSpellActive(Nightbloom) && IsOffCooldown(Nightbloom))
                        return Nightbloom;

                    if (IsEnabled(CustomComboPreset.BLU_NewMoonFluteOpener_DoTOpener))
                    {
                        if ((!TargetHasEffectAny(Debuffs.BreathOfMagic) && IsSpellActive(BreathOfMagic)) || (!TargetHasEffectAny(Debuffs.MortalFlame) && IsSpellActive(MortalFlame)))
                        {
                            if (IsSpellActive(Bristle) && !HasEffect(Buffs.Bristle))
                                return Bristle;

                            if (IsSpellActive(FeatherRain) && IsOffCooldown(FeatherRain))
                                return FeatherRain;

                            if (IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                                return SeaShanty;

                            if (IsSpellActive(BreathOfMagic) && !TargetHasEffectAny(Debuffs.BreathOfMagic))
                                return BreathOfMagic;
                            else if (IsSpellActive(MortalFlame) && !TargetHasEffectAny(Debuffs.MortalFlame))
                                return MortalFlame;
                        }
                    }
                    else
                    {
                        if (IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation) && !WasLastSpell(WingedReprobation) && !WasLastAbility(FeatherRain) && (!HasEffect(Buffs.WingedReprobation) || FindEffect(Buffs.WingedReprobation)?.StackCount < 2))
                            return WingedReprobation;

                        if (IsSpellActive(FeatherRain) && IsOffCooldown(FeatherRain))
                            return FeatherRain;

                        if (IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                            return SeaShanty;
                    }

                    if (IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation) && !WasLastAbility(ShockStrike) && FindEffect(Buffs.WingedReprobation)?.StackCount < 2)
                        return WingedReprobation;

                    if (IsSpellActive(ShockStrike) && IsOffCooldown(ShockStrike))
                        return ShockStrike;

                    if (IsSpellActive(BeingMortal) && IsOffCooldown(BeingMortal) && IsNotEnabled(CustomComboPreset.BLU_NewMoonFluteOpener_DoTOpener))
                        return BeingMortal;

                    if (IsSpellActive(Bristle) && !HasEffect(Buffs.Bristle) && IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                        return Bristle;

                    if (IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (IsSpellActive(Surpanakha))
                    {
                        if (GetRemainingCharges(Surpanakha) > 0) return Surpanakha;
                    }

                    if (IsSpellActive(MatraMagic) && HasEffect(All.Buffs.Swiftcast))
                        return MatraMagic;

                    if (IsSpellActive(BeingMortal) && IsOffCooldown(BeingMortal) && IsEnabled(CustomComboPreset.BLU_NewMoonFluteOpener_DoTOpener))
                        return BeingMortal;

                    if (IsSpellActive(PhantomFlurry) && IsOffCooldown(PhantomFlurry))
                        return PhantomFlurry;

                    if (HasEffect(Buffs.PhantomFlurry) && FindEffect(Buffs.PhantomFlurry)?.RemainingTime < 2)
                        return OriginalHook(PhantomFlurry);

                    if (HasEffect(Buffs.MoonFlute))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }
    }
}