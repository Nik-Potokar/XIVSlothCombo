using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            PiercingTalon = 90,
            LanceCharge = 85,
            DragonSight = 7398,
            BattleLitany = 3557,
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
            ChaoticSpring = 25772,
            DragonfireDive = 96,
            SpineshatterDive = 95,
            Geirskogul = 3555,
            Nastrond = 7400,
            HeavensThrust = 25771;

        public static class Buffs
        {
            public const ushort
                LanceCharge = 1864,
                RightEye = 1910,
                BattleLitany = 786,
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
                PiercingTalon = 15,
                Disembowel = 18,
                FullThrust = 26,
                LanceCharge = 30,
                Jump = 30,
                SpineshatterDive = 45,
                DragonfireDive = 50,
                ChaosThrust = 50,
                BattleLitany = 52,
                FangAndClaw = 56,
                WheelingThrust = 58,
                Geirskogul = 60,
                SonicThrust = 62,
                DragonSight = 66,
                MirageDive = 68,
                Nastrond = 70,
                CoerthanTorment = 72,
                HighJump = 74,
                RaidenThrust = 76,
                Stardiver = 80,
                DraconianFury = 82,
                ChaoticSpring = 86,
                HeavensThrust = 86;
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

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonChaosFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange(true))
                        return DRG.PiercingTalon;
                }

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

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonFullFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange(true))
                        return DRG.PiercingTalon;
                }

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
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var canWeaveAbilities = (
                    CanWeave(DRG.TrueThrust) ||
                    CanWeave(DRG.VorpalThrust) ||
                    CanWeave(DRG.Disembowel) ||
                    CanWeave(DRG.FullThrust) ||
                    CanWeave(DRG.ChaosThrust) ||
                    CanWeave(DRG.FangAndClaw) ||
                    CanWeave(DRG.WheelingThrust) ||
                    CanWeave(DRG.HeavensThrust) ||
                    CanWeave(DRG.ChaoticSpring)
                );

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonPlusFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange(true))
                        return DRG.PiercingTalon;
                }

                //(High) Jump Plus Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonHighJumpPlusFeature))
                    {
                        if (
                            level >= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.HighJump) && canWeaveAbilities
                           ) return DRG.HighJump;

                        if (
                            level >= DRG.Levels.Jump && level <= 73 &&
                            IsOffCooldown(DRG.Jump) && canWeaveAbilities
                           ) return DRG.Jump;
                    }
                }

                //Mirage Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonMiragePlusFeature))
                    {
                        if (
                            level >= DRG.Levels.MirageDive &&
                            HasEffect(DRG.Buffs.DiveReady) && canWeaveAbilities
                           ) return DRG.MirageDive;
                    }
                }

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

    internal class DragoonSimple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonSimple;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<DRGGauge>();
            if (actionID == DRG.FullThrust)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var canWeaveAbilities = (
                    CanWeave(DRG.TrueThrust) ||
                    CanWeave(DRG.VorpalThrust) ||
                    CanWeave(DRG.Disembowel) ||
                    CanWeave(DRG.FullThrust) ||
                    CanWeave(DRG.ChaosThrust) ||
                    CanWeave(DRG.FangAndClaw) ||
                    CanWeave(DRG.WheelingThrust) ||
                    CanWeave(DRG.HeavensThrust) ||
                    CanWeave(DRG.ChaoticSpring)
                );

                //Buffs Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonBuffsFeature))
                    {
                        if (
                            level >= DRG.Levels.LanceCharge &&
                            IsOffCooldown(DRG.LanceCharge) && canWeaveAbilities
                           ) return DRG.LanceCharge;

                        if (
                            level >= DRG.Levels.BattleLitany &&
                            IsOffCooldown(DRG.BattleLitany) && canWeaveAbilities
                           ) return DRG.BattleLitany;
                    }
                }

                //Dragon Sight Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonDragonSightFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonSight &&
                            IsOffCooldown(DRG.DragonSight) && canWeaveAbilities
                           ) return DRG.DragonSight;
                    }
                }

                //Wyrmwind Thrust Feature
                if (canWeaveAbilities)
                {
                    if (gauge.FirstmindsFocusCount == 2 && canWeaveAbilities)
                        return DRG.WyrmwindThrust;
                }

                //Life Surge Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonLifeSurgeFeature))
                    {
                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == DRG.FangAndClaw &&
                            HasEffect(DRG.Buffs.EnhancedWheelingThrust) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == DRG.WheelingThrust &&
                            HasEffect(DRG.Buffs.SharperFangAndClaw) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && canWeaveAbilities
                           ) return DRG.LifeSurge;
                    }
                }

                //Geirskogul and Nastrond Feature
                if (canWeaveAbilities)
                {

                    if (IsEnabled(CustomComboPreset.DragoonGeirskogulNastrondFeature))
                    {
                        if (
                            level >= DRG.Levels.Geirskogul &&
                            IsOffCooldown(DRG.Geirskogul) && canWeaveAbilities
                           ) return DRG.Geirskogul;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Nastrond &&
                            IsOffCooldown(DRG.Nastrond) && canWeaveAbilities
                           ) return DRG.Nastrond;
                    }
                }

                //(High) Jump Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonHighJumpFeature))
                    {
                        if (
                            level >= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.HighJump) && canWeaveAbilities
                           ) return DRG.HighJump;

                        if (
                            level >= DRG.Levels.Jump && level <= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.Jump) && canWeaveAbilities
                           ) return DRG.Jump;
                    }
                }

                //Dives under Litany and Life of the Dragon Feature
                if (canWeaveAbilities)
                {

                    if (IsEnabled(CustomComboPreset.DragoonLifeLitanyDiveFeature))
                    {
                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, weaveTime: 0.9)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, weaveTime: 1.7)
                           ) return DRG.Stardiver;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, weaveTime: 0.9)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives under Litany Feature
                if (canWeaveAbilities)
                {

                    if (IsEnabled(CustomComboPreset.DragoonLitanyDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, weaveTime: 0.9)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, weaveTime: 1.7)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, weaveTime: 0.9)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Mirage Feature
                if (canWeaveAbilities)
                {
                    if (IsEnabled(CustomComboPreset.DragoonMirageFeature))
                    {
                        if (
                            level >= DRG.Levels.MirageDive &&
                            HasEffect(DRG.Buffs.DiveReady) && canWeaveAbilities
                           ) return DRG.MirageDive;
                    }
                }

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonChaosFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange(true))
                        return DRG.PiercingTalon;
                }

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
