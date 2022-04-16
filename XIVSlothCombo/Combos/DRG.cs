using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            TrueNorth = 7546,
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
                TrueNorth = 1250,
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
            public const ushort
                ChaosThrust = 118,
                ChaoticSpring = 2719;
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
                TrueNorth = 50,
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
                    if (!InMeleeRange())
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
                    if (!InMeleeRange())
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
                var canWeave = CanWeave(actionID);

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonPlusFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange())
                        return DRG.PiercingTalon;
                }

                //(High) Jump Plus Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonHighJumpPlusFeature))
                    {
                        if (
                            level >= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.HighJump) && canWeave
                           ) return DRG.HighJump;

                        if (
                            level >= DRG.Levels.Jump && level <= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.Jump) && canWeave
                           ) return DRG.Jump;
                    }
                }

                //Life Surge Plus Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonLifeSurgePlusFeature))
                    {
                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            HasEffect(DRG.Buffs.EnhancedWheelingThrust) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            HasEffect(DRG.Buffs.SharperFangAndClaw) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                           ) return DRG.LifeSurge;
                    }
                }

                //Mirage Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonMiragePlusFeature))
                    {
                        if (
                            level >= DRG.Levels.MirageDive &&
                            HasEffect(DRG.Buffs.DiveReady) && canWeave
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
        internal static bool inOpener = false;
        internal static bool openerFinished = false;
        internal static byte step = 0;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var Disembowel = FindEffectAny(DRG.Buffs.PowerSurge);
            var gauge = GetJobGauge<DRGGauge>();
            if (actionID == DRG.FullThrust)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var canWeave = CanWeave(actionID);

                if (IsEnabled(CustomComboPreset.DragoonOpenerFeature) && level >= 88)
                {
                    if (inCombat && lastComboMove == DRG.TrueThrust && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (!inOpener)
                    {
                        //Buffs Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonBuffsFeature))
                            {
                                if (
                                    level >= DRG.Levels.LanceCharge &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.LanceCharge) && canWeave
                                   ) return DRG.LanceCharge;

                                if (
                                    level >= DRG.Levels.BattleLitany &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.BattleLitany) && canWeave
                                   ) return DRG.BattleLitany;
                            }
                        }

                        //Dragon Sight Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonDragonSightFeature))
                            {
                                if (
                                    level >= DRG.Levels.DragonSight &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.DragonSight) && canWeave
                                   ) return DRG.DragonSight;
                            }
                        }

                        //Geirskogul and Nastrond Feature Part 1
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonGeirskogulNastrondFeature))
                            {
                                if (
                                    level >= DRG.Levels.Geirskogul &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Geirskogul) && CanWeave(actionID, 0.001)
                                   ) return DRG.Geirskogul;
                            }
                        }

                        //(High) Jump Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonHighJumpFeature))
                            {
                                if (
                                    level >= DRG.Levels.HighJump &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.HighJump) && CanWeave(actionID, 0.5)
                                   ) return DRG.HighJump;

                                if (
                                    level >= DRG.Levels.Jump && level <= DRG.Levels.HighJump &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Jump) && CanWeave(actionID, 0.5)
                                   ) return DRG.Jump;
                            }
                        }

                        //Life Surge Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonLifeSurgeFeature))
                            {
                                if (
                                    HasEffect(DRG.Buffs.LanceCharge) &&
                                    HasEffect(DRG.Buffs.PowerSurge) &&
                                    !HasEffect(DRG.Buffs.LifeSurge) &&
                                    lastComboMove == DRG.VorpalThrust &&
                                    GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                                   ) return DRG.LifeSurge;

                                if (
                                    HasEffect(DRG.Buffs.RightEye) &&
                                    HasEffect(DRG.Buffs.PowerSurge) &&
                                    !HasEffect(DRG.Buffs.LifeSurge) &&
                                    lastComboMove == DRG.VorpalThrust &&
                                    GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                                   ) return DRG.LifeSurge;

                                if (
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    HasEffect(DRG.Buffs.PowerSurge) &&
                                    !HasEffect(DRG.Buffs.LifeSurge) &&
                                    HasEffect(DRG.Buffs.EnhancedWheelingThrust) &&
                                    GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                                   ) return DRG.LifeSurge;

                                if (
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    HasEffect(DRG.Buffs.PowerSurge) &&
                                    !HasEffect(DRG.Buffs.LifeSurge) &&
                                    HasEffect(DRG.Buffs.SharperFangAndClaw) &&
                                    GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, 0.001)
                                   ) return DRG.LifeSurge;
                            }
                        }

                        //Wyrmwind Thrust Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonWyrmwindFeature))
                            {
                                if (
                                    gauge.FirstmindsFocusCount == 2 && canWeave
                                   ) return DRG.WyrmwindThrust;
                            }
                        }

                        //Geirskogul and Nastrond Feature Part 2
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonGeirskogulNastrondFeature))
                            {
                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.Nastrond &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Nastrond) && CanWeave(actionID, 0.001)
                                   ) return DRG.Nastrond;
                            }
                        }

                        //Dives under Litany and Life of the Dragon Feature
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonLifeLitanyDiveFeature))
                            {
                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.DragonfireDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                                   ) return DRG.DragonfireDive;

                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.Stardiver &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.7)
                                   ) return DRG.Stardiver;

                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.SpineshatterDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                                   ) return DRG.SpineshatterDive;
                            }
                        }

                        //Dives under Litany Feature
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonLitanyDiveFeature))
                            {
                                if (
                                    level >= DRG.Levels.DragonfireDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                                   ) return DRG.DragonfireDive;

                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.Stardiver &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.3)
                                   ) return DRG.Stardiver;

                                if (
                                    level >= DRG.Levels.SpineshatterDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffect(DRG.Buffs.BattleLitany) &&
                                    GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                                   ) return DRG.SpineshatterDive;
                            }
                        }

                        //Dives Feature
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonLifeLitanyDiveFeature))
                            {
                                if (
                                    level >= DRG.Levels.DragonfireDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                                   ) return DRG.DragonfireDive;

                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.Stardiver &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.3)
                                   ) return DRG.Stardiver;

                                if (
                                    level >= DRG.Levels.SpineshatterDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                                   ) return DRG.SpineshatterDive;
                            }
                        }

                        //Dives under Lance Charge Feature
                        if (canWeave)
                        {

                            if (IsEnabled(CustomComboPreset.DragoonLanceDiveFeature))
                            {
                                if (
                                    level >= DRG.Levels.DragonfireDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffectAny(DRG.Buffs.LanceCharge) &&
                                    IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                                   ) return DRG.DragonfireDive;

                                if (
                                    gauge.IsLOTDActive == true &&
                                    level >= DRG.Levels.Stardiver &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                                   ) return DRG.Stardiver;

                                if (
                                    level >= DRG.Levels.SpineshatterDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffectAny(DRG.Buffs.LanceCharge) &&
                                    GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                                   ) return DRG.SpineshatterDive;
                            }
                        }

                        //Mirage Feature
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DragoonMirageFeature))
                            {
                                if (
                                    level >= DRG.Levels.MirageDive &&
                                    HasEffectAny(DRG.Buffs.PowerSurge) &&
                                    HasEffect(DRG.Buffs.DiveReady) && CanWeave(actionID, 0.001)
                                   ) return DRG.MirageDive;
                            }
                        }

                        //Piercing Talon Uptime Feature
                        if (IsEnabled(CustomComboPreset.DragoonPiercingTalonChaosFeature) && level >= DRG.Levels.PiercingTalon)
                        {
                            if (!InMeleeRange())
                                return DRG.PiercingTalon;
                        }

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

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        step = 0;
                        openerFinished = false;

                        return OriginalHook(DRG.TrueThrust);
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        if (step == 0)
                        {
                            if (gauge.EyeCount > 0) openerFinished = true;
                            else step++;
                        }

                        if (step == 1)
                        {
                            if (lastComboMove == DRG.TrueThrust) step++;
                            else return DRG.TrueNorth;
                        }

                        if (step == 2)
                        {
                            if (lastComboMove == DRG.Disembowel) step++;
                            else return DRG.Disembowel;
                        }

                        if (step == 3)
                        {
                            if (IsOnCooldown(DRG.LanceCharge)) step++;
                            else return DRG.LanceCharge;
                        }

                        if (step == 4)
                        {
                            if (IsOnCooldown(DRG.DragonSight)) step++;
                            else return DRG.DragonSight;
                        }

                        if (step == 5)
                        {
                            if (TargetHasEffectAny(DRG.Debuffs.ChaoticSpring)) step++;
                            return DRG.ChaoticSpring;
                        }

                        if (step == 6)
                        {
                            if (IsOnCooldown(DRG.BattleLitany)) step++;
                            else return DRG.BattleLitany;
                        }

                        if (step == 7)
                        {
                            if (IsOnCooldown(DRG.Geirskogul)) step++;
                            else return DRG.Geirskogul;
                        }

                        if (step == 8)
                        {
                            if (!HasEffectAny(DRG.Buffs.EnhancedWheelingThrust)) step++;
                            else return DRG.WheelingThrust;
                        }

                        if (step == 9)
                        {
                            if (IsOnCooldown(DRG.HighJump)) step++;
                            else return DRG.HighJump;
                        }

                        if (step == 10)
                        {
                            if (GetRemainingCharges(DRG.LifeSurge) is 0 or 1) step++;
                            else return DRG.LifeSurge;
                        }

                        if (step == 11)
                        {
                            if (!HasEffectAny(DRG.Buffs.SharperFangAndClaw)) step++;
                            else return DRG.FangAndClaw;
                        }

                        if (step == 12)
                        {
                            if (IsOnCooldown(DRG.DragonfireDive)) step++;
                            else return DRG.DragonfireDive;
                        }

                        if (step == 13)
                        {
                            if (lastComboMove == (DRG.RaidenThrust)) step++;
                            else return DRG.RaidenThrust;
                        }

                        if (step == 14)
                        {
                            if (GetRemainingCharges(DRG.SpineshatterDive) is 0 or 1) step++;
                            else return DRG.SpineshatterDive;
                        }

                        if (step == 15)
                        {
                            if (lastComboMove == DRG.VorpalThrust) step++;
                            else return DRG.VorpalThrust;
                        }

                        if (step == 16)
                        {
                            if (GetRemainingCharges(DRG.LifeSurge) == 0) step++;
                            else return DRG.LifeSurge;
                        }

                        if (step == 17)
                        {
                            if (IsOnCooldown(DRG.MirageDive)) step++;
                            else return DRG.MirageDive;
                        }

                        if (step == 18)
                        {
                            if (lastComboMove == DRG.HeavensThrust) step++;
                            else return DRG.HeavensThrust;
                        }

                        if (step == 19)
                        {
                            if (GetRemainingCharges(DRG.SpineshatterDive) == 0) step++;
                            else return DRG.SpineshatterDive;
                        }

                        if (step == 20)
                        {
                            if (!HasEffectAny(DRG.Buffs.SharperFangAndClaw)) step++;
                            else return DRG.FangAndClaw;
                        }

                        if (step == 21)
                        {
                            if (!HasEffectAny(DRG.Buffs.EnhancedWheelingThrust)) step++;
                            else return DRG.WheelingThrust;
                        }

                        openerFinished = true;
                    }
                }

                //Buffs Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonBuffsFeature))
                    {
                        if (
                            level >= DRG.Levels.LanceCharge &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.LanceCharge) && canWeave
                           ) return DRG.LanceCharge;

                        if (
                            level >= DRG.Levels.BattleLitany &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.BattleLitany) && canWeave
                           ) return DRG.BattleLitany;
                    }
                }

                //Dragon Sight Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonDragonSightFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonSight &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.DragonSight) && canWeave
                           ) return DRG.DragonSight;
                    }
                }

                //Geirskogul and Nastrond Feature Part 1
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonGeirskogulNastrondFeature))
                    {
                        if (
                            level >= DRG.Levels.Geirskogul &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Geirskogul) && CanWeave(actionID, 0.001)
                           ) return DRG.Geirskogul;
                    }
                }

                //(High) Jump Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonHighJumpFeature))
                    {
                        if (
                            level >= DRG.Levels.HighJump &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.HighJump) && CanWeave(actionID, 0.5)
                           ) return DRG.HighJump;

                        if (
                            level >= DRG.Levels.Jump && level <= DRG.Levels.HighJump &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Jump) && CanWeave(actionID, 0.5)
                           ) return DRG.Jump;
                    }
                }

                //Life Surge Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonLifeSurgeFeature))
                    {
                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            lastComboMove == DRG.VorpalThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            HasEffect(DRG.Buffs.EnhancedWheelingThrust) &&
                            lastComboMove == DRG.FangAndClaw &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.001)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffect(DRG.Buffs.PowerSurge) &&
                            !HasEffect(DRG.Buffs.LifeSurge) &&
                            HasEffect(DRG.Buffs.SharperFangAndClaw) &&
                            lastComboMove == DRG.WheelingThrust &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.001)
                           ) return DRG.LifeSurge;
                    }
                }

                //Wyrmwind Thrust Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonWyrmwindFeature))
                    {
                        if (
                            gauge.FirstmindsFocusCount == 2 && canWeave
                           ) return DRG.WyrmwindThrust;
                    }
                }

                //Geirskogul and Nastrond Feature Part 2
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonGeirskogulNastrondFeature))
                    {
                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Nastrond &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Nastrond) && CanWeave(actionID, 0.001)
                           ) return DRG.Nastrond;
                    }
                }

                //Dives under Litany and Life of the Dragon Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonLifeLitanyDiveFeature))
                    {
                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives under Litany Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonLitanyDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives under Lance Charge Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonLanceDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffectAny(DRG.Buffs.LanceCharge) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffectAny(DRG.Buffs.LanceCharge) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Mirage Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonMirageFeature))
                    {
                        if (
                            level >= DRG.Levels.MirageDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffect(DRG.Buffs.DiveReady) && CanWeave(actionID, 0.001)
                           ) return DRG.MirageDive;
                    }
                }

                //Piercing Talon Uptime Feature
                if (IsEnabled(CustomComboPreset.DragoonPiercingTalonChaosFeature) && level >= DRG.Levels.PiercingTalon)
                {
                    if (!InMeleeRange())
                        return DRG.PiercingTalon;
                }

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

    internal class DragoonSimpleAoE : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonSimpleAoE;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var canWeave = CanWeave(actionID);
            var gauge = GetJobGauge<DRGGauge>();
            if (actionID == DRG.CoerthanTorment)
            {

                //Buffs AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoEBuffsFeature))
                    {
                        if (
                            level >= DRG.Levels.LanceCharge &&
                            IsOffCooldown(DRG.LanceCharge) && CanWeave(actionID)
                           ) return DRG.LanceCharge;

                        if (
                            level >= DRG.Levels.BattleLitany &&
                            IsOffCooldown(DRG.BattleLitany) && CanWeave(actionID)
                           ) return DRG.BattleLitany;
                    }
                }

                //Dragon Sight AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoEDragonSightFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonSight &&
                            IsOffCooldown(DRG.DragonSight) && CanWeave(actionID)
                           ) return DRG.DragonSight;
                    }
                }

                //Geirskogul and Nastrond AoE Feature Part 1
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoEGeirskogulNastrondFeature))
                    {
                        if (
                            level >= DRG.Levels.Geirskogul &&
                            IsOffCooldown(DRG.Geirskogul) && CanWeave(actionID)
                           ) return DRG.Geirskogul;
                    }
                }

                //(High) Jump AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoEHighJumpFeature))
                    {
                        if (
                            level >= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.HighJump) && CanWeave(actionID, 1)
                           ) return DRG.HighJump;

                        if (
                            level >= DRG.Levels.Jump && level <= DRG.Levels.HighJump &&
                            IsOffCooldown(DRG.Jump) && CanWeave(actionID, 1)
                           ) return DRG.Jump;
                    }
                }

                //Life Surge AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoELifeSurgeFeature))
                    {
                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            lastComboMove == DRG.CoerthanTorment && level >= DRG.Levels.CoerthanTorment &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == DRG.CoerthanTorment && level >= DRG.Levels.CoerthanTorment &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            lastComboMove == DRG.SonicThrust && level >= DRG.Levels.SonicThrust && level <= DRG.Levels.CoerthanTorment &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == DRG.SonicThrust && level >= DRG.Levels.SonicThrust && level <= DRG.Levels.CoerthanTorment &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.LanceCharge) &&
                            lastComboMove == OriginalHook(DRG.DoomSpike) && level <= DRG.Levels.SonicThrust &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                        if (
                            HasEffect(DRG.Buffs.RightEye) &&
                            lastComboMove == OriginalHook(DRG.DoomSpike) && level <= DRG.Levels.SonicThrust &&
                            !HasEffectAny(DRG.Buffs.LifeSurge) &&
                            GetRemainingCharges(DRG.LifeSurge) > 0 && CanWeave(actionID, weaveTime: 0.3)
                           ) return DRG.LifeSurge;

                    }
                }

                //Wyrmwind Thrust AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoEWyrmwindFeature))
                    {
                        if (
                            gauge.FirstmindsFocusCount == 2 && CanWeave(actionID)
                           ) return DRG.WyrmwindThrust;
                    }
                }

                //Geirskogul and Nastrond AoE Feature Part 2
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoEGeirskogulNastrondFeature))
                    {
                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Nastrond &&
                            IsOffCooldown(DRG.Nastrond) && CanWeave(actionID)
                           ) return DRG.Nastrond;
                    }
                }

                //Dives under Litany and Life of the Dragon AoE Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoELifeLitanyDiveFeature))
                    {
                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives under Litany AoE Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoELitanyDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffect(DRG.Buffs.BattleLitany) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives AoE Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoEDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Dives under Lance Charge AoE Feature
                if (canWeave)
                {

                    if (IsEnabled(CustomComboPreset.DragoonAoELanceDiveFeature))
                    {
                        if (
                            level >= DRG.Levels.DragonfireDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffectAny(DRG.Buffs.LanceCharge) &&
                            IsOffCooldown(DRG.DragonfireDive) && CanWeave(actionID, 1)
                           ) return DRG.DragonfireDive;

                        if (
                            gauge.IsLOTDActive == true &&
                            level >= DRG.Levels.Stardiver &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            IsOffCooldown(DRG.Stardiver) && CanWeave(actionID, 1.5)
                           ) return DRG.Stardiver;

                        if (
                            level >= DRG.Levels.SpineshatterDive &&
                            HasEffectAny(DRG.Buffs.PowerSurge) &&
                            HasEffectAny(DRG.Buffs.LanceCharge) &&
                            GetRemainingCharges(DRG.SpineshatterDive) > 0 && CanWeave(actionID, 1)
                           ) return DRG.SpineshatterDive;
                    }
                }

                //Mirage AoE Feature
                if (canWeave)
                {
                    if (IsEnabled(CustomComboPreset.DragoonAoEMirageFeature))
                    {
                        if (
                            level >= DRG.Levels.MirageDive &&
                            HasEffect(DRG.Buffs.DiveReady) && CanWeave(actionID)
                           ) return DRG.MirageDive;
                    }
                }

                if (comboTime > 0)
                {
                    if ((lastComboMove == OriginalHook(DRG.DoomSpike)) && level >= DRG.Levels.SonicThrust)
                        return DRG.SonicThrust;

                    if (lastComboMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                        return DRG.CoerthanTorment;

                    if ((lastComboMove == DRG.DraconianFury))
                        return DRG.SonicThrust;
                }

                return OriginalHook(DRG.DoomSpike);
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
