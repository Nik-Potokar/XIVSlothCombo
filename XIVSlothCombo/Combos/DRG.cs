namespace XIVSlothComboPlugin.Combos
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            Jump = 92,
            LifeSurge = 83,
            HighJump = 16478,
            MirageDive = 7399,
            BloodOfTheDragon = 3553,
            Stardiver = 16480,
            CoerthanTorment = 16477,
            DoomSpike = 86,
            SonicThrust = 7397,
            ChaosThrust = 88,
            RaidenThrust = 16479,
            TrueThrust = 75,
            Disembowel = 87,
            FangAndClaw = 3554,
            WheelingThrust = 3556,
            FullThrust = 84,
            VorpalThrust = 78,
            WyrmwindThrust = 25773,
            DraconianFury = 25770,
            ChaoticSpring = 25772;

        public static class Buffs
        {
            public const ushort
                SharperFangAndClaw = 802,
                EnhancedWheelingThrust = 803,
                DiveReady = 1243,
                RaidenThrustReady = 1863,
                PowerSurge = 2720,
                LifeSurge = 116,
                DraconianFire = 1863;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                VorpalThrust = 4,
                Disembowel = 18,
                FullThrust = 26,
                Jump = 30,
                ChaosThrust = 50,
                FangAndClaw = 56,
                WheelingThrust = 58,
                SonicThrust = 62,
                CoerthanTorment = 72,
                HighJump = 74,
                RaidenThrust = 76,
                Stardiver = 80,
                DraconianFury = 82,
                ChaoticSpring = 86;
        }
    }

    internal class DragoonJumpFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonJumpFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.Jump)
            {
                if (HasEffect(DRG.Buffs.DiveReady))
                    return DRG.MirageDive;

                return OriginalHook(DRG.HighJump);
            }

            return actionID;
        }
    }

    internal class DragoonCoerthanTormentCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonCoerthanTormentCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.CoerthanTorment)
            {
                if (comboTime > 0)
                {
                    if ((lastComboMove == DRG.DoomSpike || lastComboMove == DRG.DraconianFury) && level >= DRG.Levels.SonicThrust)
                        return DRG.SonicThrust;
                    if (lastComboMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                        return DRG.CoerthanTorment;
                }
                return OriginalHook(DRG.DoomSpike);
            }
            return actionID;
        }
    }

    internal class DragoonChaosThrustCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonChaosThrustCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.ChaosThrust || actionID == DRG.ChaoticSpring)
            {
                if (comboTime > 0)
                {
                    if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.Disembowel)
                        return DRG.Disembowel;

                    if (lastComboMove == DRG.Disembowel && level >= DRG.Levels.ChaosThrust)
                        return OriginalHook(DRG.ChaosThrust);
                }

                if (IsEnabled(CustomComboPreset.DragoonFangThrustFeature) && (HasEffect(DRG.Buffs.SharperFangAndClaw) || HasEffect(DRG.Buffs.EnhancedWheelingThrust)))
                    return DRG.WheelingThrust;

                if (HasEffect(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                    return DRG.FangAndClaw;

                if (HasEffect(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                    return DRG.WheelingThrust;

                return DRG.TrueThrust;
            }

            return actionID;
        }
    }

    internal class DragoonFullThrustCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonFullThrustCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.FullThrust)
            {
                if (comboTime > 0)
                {
                    if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.VorpalThrust)
                        return DRG.VorpalThrust;

                    if (lastComboMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                        return DRG.FullThrust;
                }

                if (HasEffect(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                    return DRG.FangAndClaw;

                if (HasEffect(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                    return DRG.WheelingThrust;

                return OriginalHook(DRG.TrueThrust);
            }

            return actionID;
        }
    }
    internal class DragoonFullThrustComboPlus : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonFullThrustComboPlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.FullThrust)
            {
                var Disembowel = FindEffectAny(DRG.Buffs.PowerSurge);
                if (comboTime > 0)
                {
                    if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.Disembowel && (Disembowel == null || (Disembowel.RemainingTime < 10)))
                        return DRG.Disembowel;

                    if (lastComboMove == DRG.Disembowel && level >= DRG.Levels.ChaoticSpring)
                        return DRG.ChaoticSpring;

                    if (lastComboMove == DRG.Disembowel && level >= DRG.Levels.ChaosThrust)
                        return DRG.ChaosThrust;

                    if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.VorpalThrust)
                        return DRG.VorpalThrust;

                    if (lastComboMove == DRG.VorpalThrust && !HasEffect(DRG.Buffs.LifeSurge) && GetRemainingCharges(DRG.LifeSurge) > 0)
                        return DRG.LifeSurge;

                    if (lastComboMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                        return DRG.FullThrust;

                    if (lastComboMove == DRG.FullThrust && !GetCooldown(DRG.HighJump).IsCooldown && level >= DRG.Levels.HighJump)
                        return DRG.HighJump;

                    if (lastComboMove == DRG.FullThrust && !GetCooldown(DRG.Jump).IsCooldown && level >= DRG.Levels.Jump && level <= 73)
                        return DRG.Jump;
                }

                if (HasEffect(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                    return DRG.FangAndClaw;

                if (HasEffect(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                    return DRG.WheelingThrust;

                return OriginalHook(DRG.TrueThrust);
            }

            return actionID;
        }
    }
    internal class DragoonFangAndClawFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonFangAndClawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.FangAndClaw)
            {
                if (HasEffect(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                    return DRG.WheelingThrust;
                if (HasEffect(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                    return DRG.FangAndClaw;

            }

            return actionID;
        }
    }
}
