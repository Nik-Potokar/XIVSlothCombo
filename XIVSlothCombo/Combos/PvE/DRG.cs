using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Combos.JobHelpers.DRG;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            PiercingTalon = 90,
            ElusiveJump = 94,
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
            public const ushort
                ChaosThrust = 118,
                ChaoticSpring = 2719;
        }

        public static class Traits
        {
            public const uint
                EnhancedSpineshatterDive = 436,
                EnhancedLifeSurge = 438;
        }

        public static class Config
        {
            public static UserFloat
                DRG_VariantCure = new("DRG_VariantCure"),
                DRG_STSecondWindThreshold = new("DRG_STSecondWindThreshold"),
                DRG_STBloodbathThreshold = new("DRG_STBloodbathThreshold"),
                DRG_AoESecondWindThreshold = new("DRG_AoESecondWindThreshold"),
                DRG_AoEBloodbathThreshold = new("DRG_AoEBloodbathThreshold");

            public static UserInt
                DRG_ST_DiveOptions = new("DRG_ST_DiveOptions"),
                DRG_AOE_DiveOptions = new("DRG_AOE_DiveOptions"),
                DRG_OpenerOptions = new("DRG_OpenerOptions");
        }

        internal class DRG_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                bool openerReady = IsOffCooldown(LanceCharge) && IsOffCooldown(BattleLitany) && IsOffCooldown(DragonSight);
                Status? ChaosDoTDebuff;

                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    // Opener for DRG
                    //2.5 GCD
                    if (level >= 88)
                    {
                        // Check to start opener
                        if (openerStarted && HasEffect(All.Buffs.Sprint))
                        { inOpener = true; openerStarted = false; readyOpener = false; }

                        if ((readyOpener || openerStarted) && HasEffect(All.Buffs.Sprint) && !inOpener)
                        { openerStarted = true; return TrueThrust; }
                        else
                        { openerStarted = false; }

                        // Reset check for opener
                        if (openerReady && !InCombat() && !inOpener && !openerStarted)
                        {
                            readyOpener = true;
                            inOpener = false;
                            step = 0;
                            return TrueThrust;
                        }
                        else
                        { readyOpener = false; }

                        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                        if ((step == 0 && lastComboMove is TrueThrust && !HasEffect(All.Buffs.Sprint)) ||
                            (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                            inOpener = false;

                        if (CombatEngageDuration().TotalSeconds < 10 && IsOnCooldown(ElusiveJump) && level >= 88 && openerReady)
                            inOpener = true;

                        if (inOpener)
                        {
                            if (step == 0)
                            {
                                if (lastComboMove == TrueThrust) step++;
                                else return TrueThrust;
                            }

                            if (step == 1)
                            {
                                if (lastComboMove == Disembowel) step++;
                                else return Disembowel;
                            }

                            if (step == 2)
                            {
                                if (IsOnCooldown(LanceCharge)) step++;
                                else return LanceCharge;
                            }

                            if (step == 3)
                            {
                                if (IsOnCooldown(DragonSight)) step++;
                                else return DragonSight;
                            }

                            if (step == 4)
                            {
                                if (lastComboMove == ChaoticSpring) step++;
                                else return ChaoticSpring;
                            }

                            if (step == 5)
                            {
                                if (IsOnCooldown(BattleLitany)) step++;
                                else return BattleLitany;
                            }

                            if (step == 6)
                            {
                                if (lastComboMove == WheelingThrust) step++;
                                else return WheelingThrust;
                            }

                            if (step == 7)
                            {
                                if (WasLastAction(Geirskogul)) step++;
                                else return Geirskogul;
                            }

                            if (step == 8)
                            {
                                if (GetRemainingCharges(LifeSurge) < 2) step++;
                                else return LifeSurge;
                            }

                            if (step == 9)
                            {
                                if (lastComboMove == FangAndClaw) step++;
                                else return FangAndClaw;
                            }

                            if (step == 10)
                            {
                                if (IsOnCooldown(HighJump)) step++;
                                else return HighJump;
                            }

                            if (step == 11)
                            {
                                if (IsOnCooldown(MirageDive)) step++;
                                else return MirageDive;
                            }

                            if (step == 12)
                            {
                                if (lastComboMove == RaidenThrust) step++;
                                else return RaidenThrust;
                            }

                            if (step == 13)
                            {
                                if (IsOnCooldown(DragonfireDive)) step++;
                                else return DragonfireDive;
                            }

                            if (step == 14)
                            {
                                if (lastComboMove == VorpalThrust) step++;
                                else return VorpalThrust;
                            }

                            if (step == 15)
                            {
                                if (GetRemainingCharges(SpineshatterDive) < 2) step++;
                                else return SpineshatterDive;
                            }

                            if (step == 16)
                            {
                                if (GetRemainingCharges(LifeSurge) < 1) step++;
                                else return LifeSurge;
                            }

                            if (step == 17)
                            {
                                if (lastComboMove == HeavensThrust) step++;
                                else return HeavensThrust;
                            }

                            if (step == 18)
                            {
                                if (GetRemainingCharges(SpineshatterDive) < 1) step++;
                                else return SpineshatterDive;
                            }

                            if (step == 19)
                            {
                                if (lastComboMove == FangAndClaw) step++;
                                else return FangAndClaw;
                            }

                            if (step == 20)
                            {
                                if (lastComboMove == WheelingThrust) step++;
                                else return WheelingThrust;
                            }

                            if (step == 21)
                            {
                                if (lastComboMove == RaidenThrust) step++;
                                else return RaidenThrust;
                            }

                            if (step == 22)
                            {
                                if (WasLastAction(WyrmwindThrust)) step++;
                                else return WyrmwindThrust;
                            }
                            if (step == 23)
                            {
                                if (lastComboMove == Disembowel) step++;
                                else return Disembowel;
                            }

                            if (step == 24)
                            {
                                if (lastComboMove == ChaoticSpring) step++;
                                else return ChaoticSpring;
                            }

                            if (step == 25)
                            {
                                if (lastComboMove == WheelingThrust) step++;
                                else return WheelingThrust;
                            }
                            inOpener = false;
                        }
                    }

                    if (!inOpener)
                    {
                        if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                            return Variant.VariantCure;

                        // Piercing Talon Uptime Option
                        if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                            return PiercingTalon;

                        if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanWeave(actionID))
                            return Variant.VariantRampart;

                        if (HasEffect(Buffs.PowerSurge))
                        {
                            //Battle Litany Feature
                            if (ActionReady(BattleLitany) && CanDRGWeave(BattleLitany))
                                return BattleLitany;

                            //Lance Charge Feature
                            if (ActionReady(LanceCharge) && CanDRGWeave(LanceCharge))
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (ActionReady(DragonSight) && CanDRGWeave(DragonSight))
                                return DragonSight;

                            //Life Surge Feature
                            if (!HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) && CanDRGWeave(LifeSurge) &&
                                ((HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                (HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                (HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && (HasEffect(Buffs.EnhancedWheelingThrust) || HasEffect(Buffs.SharperFangAndClaw))) ||
                                (IsOnCooldown(DragonSight) && IsOnCooldown(LanceCharge) && lastComboMove is VorpalThrust)))
                                return LifeSurge;

                            //StarDives Feature
                            if (gauge.IsLOTDActive && ActionReady(Stardiver) && CanDRGWeave(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                return Stardiver;

                            //Dives Feature
                            if (!IsMoving && LevelChecked(LanceCharge))
                            {
                                if ((!TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                                   (HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye))) //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                {
                                    if (ActionReady(DragonfireDive) && CanDRGWeave(DragonfireDive))
                                        return DragonfireDive;

                                    if (ActionReady(SpineshatterDive) && CanDRGWeave(SpineshatterDive))
                                        return SpineshatterDive;
                                }
                            }

                            //(High) Jump Feature   
                            if (ActionReady(OriginalHook(Jump)) && !IsMoving && CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond Feature
                            if (IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)) && CanDRGWeave(OriginalHook(Geirskogul)))
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady) && CanDRGWeave(MirageDive))
                                return MirageDive;

                            //Wyrmwind Thrust Feature
                            if (gauge.FirstmindsFocusCount is 2 && CanDRGWeave(WyrmwindThrust))
                                return WyrmwindThrust;
                        }
                    }

                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                        return FangAndClaw;

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                        return WheelingThrust;

                    if (comboTime > 0)
                    {
                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove is Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove is TrueThrust or RaidenThrust)
                            return VorpalThrust;

                        if (lastComboMove is VorpalThrust)
                            return OriginalHook(FullThrust);
                    }

                    return OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_AdvancedMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                bool openerReady = IsOffCooldown(LanceCharge) && IsOffCooldown(BattleLitany) && IsOffCooldown(DragonSight);
                int diveOptions = Config.DRG_ST_DiveOptions;
                int openerSelection = Config.DRG_OpenerOptions;
                Status? ChaosDoTDebuff;
                float ST_secondWindTreshold = Config.DRG_STSecondWindThreshold;
                float ST_bloodBathTreshold = Config.DRG_STBloodbathThreshold;

                if (LevelChecked(ChaoticSpring))
                    ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    // Opener for DRG
                    if (IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88)
                    {
                        // Standard opener - 2.5
                        if (openerSelection is 0)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(All.Buffs.Sprint))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(All.Buffs.Sprint) && !inOpener)
                            { openerStarted = true; return TrueThrust; }
                            else
                            { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return TrueThrust;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 0 && lastComboMove is TrueThrust && !HasEffect(All.Buffs.Sprint)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                                inOpener = false;

                            if (CombatEngageDuration().TotalSeconds < 10 && IsOnCooldown(ElusiveJump) &&
                                IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (lastComboMove == TrueThrust) step++;
                                    else return TrueThrust;
                                }

                                if (step == 1)
                                {
                                    if (lastComboMove == Disembowel) step++;
                                    else return Disembowel;
                                }

                                if (step == 2)
                                {
                                    if (IsOnCooldown(LanceCharge)) step++;
                                    else return LanceCharge;
                                }

                                if (step == 3)
                                {
                                    if (IsOnCooldown(DragonSight)) step++;
                                    else return DragonSight;
                                }

                                if (step == 4)
                                {
                                    if (lastComboMove == ChaoticSpring) step++;
                                    else return ChaoticSpring;
                                }

                                if (step == 5)
                                {
                                    if (IsOnCooldown(BattleLitany)) step++;
                                    else return BattleLitany;
                                }

                                if (step == 6)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }

                                if (step == 7)
                                {
                                    if (WasLastAction(Geirskogul)) step++;
                                    else return Geirskogul;
                                }

                                if (step == 8)
                                {
                                    if (GetRemainingCharges(LifeSurge) < 2) step++;
                                    else return LifeSurge;
                                }

                                if (step == 9)
                                {
                                    if (lastComboMove == FangAndClaw) step++;
                                    else return FangAndClaw;
                                }

                                if (step == 10)
                                {
                                    if (IsOnCooldown(HighJump)) step++;
                                    else return HighJump;
                                }

                                if (step == 11)
                                {
                                    if (IsOnCooldown(MirageDive)) step++;
                                    else return MirageDive;
                                }

                                if (step == 12)
                                {
                                    if (lastComboMove == RaidenThrust) step++;
                                    else return RaidenThrust;
                                }

                                if (step == 13)
                                {
                                    if (IsOnCooldown(DragonfireDive)) step++;
                                    else return DragonfireDive;
                                }

                                if (step == 14)
                                {
                                    if (lastComboMove == VorpalThrust) step++;
                                    else return VorpalThrust;
                                }

                                if (step == 15)
                                {
                                    if (GetRemainingCharges(SpineshatterDive) < 2) step++;
                                    else return SpineshatterDive;
                                }

                                if (step == 16)
                                {
                                    if (GetRemainingCharges(LifeSurge) < 1) step++;
                                    else return LifeSurge;
                                }

                                if (step == 17)
                                {
                                    if (lastComboMove == HeavensThrust) step++;
                                    else return HeavensThrust;
                                }

                                if (step == 18)
                                {
                                    if (GetRemainingCharges(SpineshatterDive) < 1) step++;
                                    else return SpineshatterDive;
                                }

                                if (step == 19)
                                {
                                    if (lastComboMove == FangAndClaw) step++;
                                    else return FangAndClaw;
                                }

                                if (step == 20)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }

                                if (step == 21)
                                {
                                    if (lastComboMove == RaidenThrust) step++;
                                    else return RaidenThrust;
                                }

                                if (step == 22)
                                {
                                    if (WasLastAction(WyrmwindThrust)) step++;
                                    else return WyrmwindThrust;
                                }
                                if (step == 23)
                                {
                                    if (lastComboMove == Disembowel) step++;
                                    else return Disembowel;
                                }

                                if (step == 24)
                                {
                                    if (lastComboMove == ChaoticSpring) step++;
                                    else return ChaoticSpring;
                                }

                                if (step == 25)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }
                                inOpener = false;
                            }
                        }

                        // 2.46 OPENER                         
                        if (openerSelection is 1)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(All.Buffs.Sprint))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(All.Buffs.Sprint) && !inOpener)
                            { openerStarted = true; return TrueThrust; }
                            else
                            { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return TrueThrust;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 0 && lastComboMove is TrueThrust && !HasEffect(All.Buffs.Sprint)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()))
                                inOpener = false;

                            if (CombatEngageDuration().TotalSeconds < 10 && IsOnCooldown(ElusiveJump) &&
                                IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (lastComboMove == TrueThrust) step++;
                                    else return TrueThrust;
                                }

                                if (step == 1)
                                {
                                    if (lastComboMove == Disembowel) step++;
                                    else return Disembowel;
                                }

                                if (step == 2)
                                {
                                    if (IsOnCooldown(LanceCharge)) step++;
                                    else return LanceCharge;
                                }

                                if (step == 3)
                                {
                                    if (IsOnCooldown(DragonSight)) step++;
                                    else return DragonSight;
                                }

                                if (step == 4)
                                {
                                    if (lastComboMove == ChaoticSpring) step++;
                                    else return ChaoticSpring;
                                }

                                if (step == 5)
                                {
                                    if (IsOnCooldown(BattleLitany)) step++;
                                    else return BattleLitany;
                                }

                                if (step == 6)
                                {
                                    if (WasLastAction(Geirskogul)) step++;
                                    else return Geirskogul;
                                }

                                if (step == 7)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }

                                if (step == 8)
                                {
                                    if (GetRemainingCharges(SpineshatterDive) < 2) step++;
                                    else return SpineshatterDive;
                                }

                                if (step == 9)
                                {
                                    if (GetRemainingCharges(LifeSurge) < 2) step++;
                                    else return LifeSurge;
                                }

                                if (step == 10)
                                {
                                    if (lastComboMove == FangAndClaw) step++;
                                    else return FangAndClaw;
                                }

                                if (step == 11)
                                {
                                    if (IsOnCooldown(HighJump)) step++;
                                    else return HighJump;
                                }

                                if (step == 12)
                                {
                                    if (IsOnCooldown(MirageDive)) step++;
                                    else return MirageDive;
                                }

                                if (step == 13)
                                {
                                    if (lastComboMove == RaidenThrust) step++;
                                    else return RaidenThrust;
                                }

                                if (step == 14)
                                {
                                    if (IsOnCooldown(DragonfireDive)) step++;
                                    else return DragonfireDive;
                                }

                                if (step == 15)
                                {
                                    if (lastComboMove == VorpalThrust) step++;
                                    else return VorpalThrust;
                                }

                                if (step == 16)
                                {
                                    if (GetRemainingCharges(SpineshatterDive) < 1) step++;
                                    else return SpineshatterDive;
                                }

                                if (step == 17)
                                {
                                    if (GetRemainingCharges(LifeSurge) < 1) step++;
                                    else return LifeSurge;
                                }

                                if (step == 18)
                                {
                                    if (lastComboMove == HeavensThrust) step++;
                                    else return HeavensThrust;
                                }

                                if (step == 19)
                                {
                                    if (lastComboMove == FangAndClaw) step++;
                                    else return FangAndClaw;
                                }

                                if (step == 20)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }

                                if (step == 21)
                                {
                                    if (lastComboMove == RaidenThrust) step++;
                                    else return RaidenThrust;
                                }

                                if (step == 22)
                                {
                                    if (WasLastAction(WyrmwindThrust)) step++;
                                    else return WyrmwindThrust;
                                }

                                if (step == 23)
                                {
                                    if (lastComboMove == Disembowel) step++;
                                    else return Disembowel;
                                }

                                if (step == 24)
                                {
                                    if (lastComboMove == ChaoticSpring) step++;
                                    else return ChaoticSpring;
                                }

                                if (step == 25)
                                {
                                    if (lastComboMove == WheelingThrust) step++;
                                    else return WheelingThrust;
                                }

                                inOpener = false;
                            }
                        }
                    }

                    if (!inOpener)
                    {
                        if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                            return Variant.VariantCure;

                        // Piercing Talon Uptime Option
                        if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) &&
                            LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                            return PiercingTalon;

                        if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanWeave(actionID))
                            return Variant.VariantRampart;

                        if (HasEffect(Buffs.PowerSurge))
                        {
                            if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                            {
                                //Battle Litany Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                                    ActionReady(BattleLitany) && CanDRGWeave(BattleLitany))
                                    return BattleLitany;

                                //Lance Charge Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                                    ActionReady(LanceCharge) && CanDRGWeave(LanceCharge))
                                    return LanceCharge;

                                //Dragon Sight Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) &&
                                    ActionReady(DragonSight) && CanDRGWeave(DragonSight))
                                    return DragonSight;
                            }

                            if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                            {
                                //Life Surge Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && CanDRGWeave(LifeSurge) &&
                                    !HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                    ((HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                    (HasEffect(Buffs.LanceCharge) && lastComboMove is VorpalThrust) ||
                                    (HasEffect(Buffs.RightEye) && HasEffect(Buffs.LanceCharge) && (HasEffect(Buffs.EnhancedWheelingThrust) || HasEffect(Buffs.SharperFangAndClaw))) ||
                                    (IsOnCooldown(DragonSight) && IsOnCooldown(LanceCharge) && lastComboMove is VorpalThrust)))
                                    return LifeSurge;

                                //StarDives Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) && CanDRGWeave(Stardiver) &&
                                    gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                    return Stardiver;

                                //Dives Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Dives) &&
                                    (IsNotEnabled(CustomComboPreset.DRG_ST_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_ST_Dives_Melee) &&
                                    GetTargetDistance() <= 1)) && !IsMoving && LevelChecked(LanceCharge))
                                {
                                    if (diveOptions is 0 || //Dives on cooldown
                                       (diveOptions is 1 && !TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                                       (diveOptions is 1 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                       (diveOptions is 2 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                    {
                                        if (ActionReady(DragonfireDive) && CanDRGWeave(DragonfireDive))
                                            return DragonfireDive;

                                        if (ActionReady(SpineshatterDive) && CanDRGWeave(SpineshatterDive))
                                            return SpineshatterDive;
                                    }
                                }

                                //(High) Jump Feature   
                                if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                                    ActionReady(OriginalHook(Jump)) && !IsMoving && CanDRGWeave(OriginalHook(Jump)))
                                    return OriginalHook(Jump);

                                //Geirskogul and Nastrond Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_GeirskogulNastrond) && CanDRGWeave(OriginalHook(Geirskogul)) && (ActionReady(OriginalHook(Geirskogul)) ||
                                    (IsEnabled(CustomComboPreset.DRG_ST_Optimized_Rotation) && IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)))))
                                    return OriginalHook(Geirskogul);

                                //Mirage Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && ((HasEffect(Buffs.DiveReady) && CanDRGWeave(MirageDive)) ||
                                   (IsEnabled(CustomComboPreset.DRG_ST_Optimized_Rotation) && IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady))))
                                    return MirageDive;

                                //Wyrmwind Thrust Feature
                                if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                                    gauge.FirstmindsFocusCount is 2 && CanDRGWeave(WyrmwindThrust))
                                    return WyrmwindThrust;
                            }
                        }

                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= ST_secondWindTreshold &&
                            ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= ST_bloodBathTreshold &&
                            ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 Combo
                    if (HasEffect(Buffs.SharperFangAndClaw))
                        return FangAndClaw;

                    if (HasEffect(Buffs.EnhancedWheelingThrust))
                        return WheelingThrust;

                    if (comboTime > 0)
                    {
                        if ((LevelChecked(OriginalHook(ChaosThrust)) && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                        {
                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                return Disembowel;

                            if (lastComboMove is Disembowel && LevelChecked(OriginalHook(ChaosThrust)))
                                return OriginalHook(ChaosThrust);
                        }

                        if (lastComboMove is TrueThrust or RaidenThrust)
                            return VorpalThrust;

                        if (lastComboMove is VorpalThrust)
                            return OriginalHook(FullThrust);
                    }

                    return OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_AOE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();

                if (actionID is DoomSpike)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                        return Variant.VariantCure;

                    // Piercing Talon Uptime Option
                    if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        //Battle Litany Feature
                        if (ActionReady(BattleLitany) && CanDRGWeave(BattleLitany))
                            return BattleLitany;

                        //Lance Charge Feature
                        if (ActionReady(LanceCharge) && CanDRGWeave(LanceCharge))
                            return LanceCharge;

                        //Dragon Sight Feature
                        if (ActionReady(DragonSight) && CanDRGWeave(DragonSight))
                            return DragonSight;

                        //Life Surge Feature
                        if (!HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                lastComboMove is SonicThrust && LevelChecked(CoerthanTorment) && CanDRGWeave(LifeSurge))
                            return LifeSurge;

                        //StarDives Feature
                        if (gauge.IsLOTDActive && ActionReady(Stardiver) && CanDRGWeave(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                            return Stardiver;

                        //Dives Feature
                        if (!IsMoving && LevelChecked(LanceCharge))
                        {
                            if ((!TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                               (HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye))) //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                            {
                                if (ActionReady(DragonfireDive) && CanDRGWeave(DragonfireDive))
                                    return DragonfireDive;

                                if (ActionReady(SpineshatterDive) && CanDRGWeave(SpineshatterDive))
                                    return SpineshatterDive;
                            }
                        }

                        //(High) Jump Feature   
                        if (ActionReady(OriginalHook(Jump)) && !IsMoving && CanDRGWeave(OriginalHook(Jump)))
                            return OriginalHook(Jump);

                        //Geirskogul and Nastrond Feature
                        if (IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)) && CanDRGWeave(OriginalHook(Geirskogul)))
                            return OriginalHook(Geirskogul);

                        //Mirage Feature
                        if (IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady) && CanDRGWeave(MirageDive))
                            return MirageDive;

                        //Wyrmwind Thrust Feature
                        if (gauge.FirstmindsFocusCount is 2 && CanDRGWeave(WyrmwindThrust))
                            return WyrmwindThrust;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                            return SonicThrust;

                        if (lastComboMove is SonicThrust && LevelChecked(CoerthanTorment))
                            return CoerthanTorment;
                    }

                    return OriginalHook(DoomSpike);
                }

                return actionID;
            }
        }

        internal class DRG_AOE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();
                int diveOptions = Config.DRG_AOE_DiveOptions;
                float AoE_secondWindTreshold = Config.DRG_AoESecondWindThreshold;
                float AoE_bloodBathTreshold = Config.DRG_AoEBloodbathThreshold;

                if (actionID is DoomSpike)
                {
                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                        return Variant.VariantCure;

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) &&
                        LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;


                    if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (HasEffect(Buffs.PowerSurge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                        {
                            //Battle Litany Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                                ActionReady(BattleLitany) && CanDRGWeave(BattleLitany))
                                return BattleLitany;

                            //Lance Charge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Lance) &&
                                ActionReady(LanceCharge) && CanDRGWeave(LanceCharge))
                                return LanceCharge;

                            //Dragon Sight Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) &&
                                ActionReady(DragonSight) && CanDRGWeave(DragonSight))
                                return DragonSight;
                        }

                        if (IsEnabled(CustomComboPreset.DRG_AoE_CDs))
                        {
                            //Life Surge Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                               !HasEffect(Buffs.LifeSurge) && HasCharges(LifeSurge) &&
                                lastComboMove is SonicThrust && LevelChecked(CoerthanTorment) && CanDRGWeave(LifeSurge))
                                return LifeSurge;

                            //StarDives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) && CanDRGWeave(Stardiver) &&
                               gauge.IsLOTDActive && ActionReady(Stardiver) && !IsMoving && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                return Stardiver;

                            //Dives Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Dives) &&
                                (IsNotEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) &&
                                GetTargetDistance() <= 1)) && !IsMoving && LevelChecked(LanceCharge))
                            {
                                if (diveOptions is 0 || //Dives on cooldown
                                   (diveOptions is 1 && !TraitLevelChecked(Traits.EnhancedSpineshatterDive) && HasEffect(Buffs.LanceCharge)) || //Dives for synched
                                   (diveOptions is 1 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                   (diveOptions is 2 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                {
                                    if (ActionReady(DragonfireDive) && CanDRGWeave(DragonfireDive))
                                        return DragonfireDive;

                                    if (ActionReady(SpineshatterDive) && CanDRGWeave(SpineshatterDive))
                                        return SpineshatterDive;
                                }
                            }

                            //(High) Jump Feature   
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                                ActionReady(OriginalHook(Jump)) && !IsMoving && CanDRGWeave(OriginalHook(Jump)))
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && CanDRGWeave(OriginalHook(Geirskogul)) && (ActionReady(OriginalHook(Geirskogul)) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_Optimized_Rotation) && IsOnCooldown(OriginalHook(Jump)) && ActionReady(OriginalHook(Geirskogul)))))
                                return OriginalHook(Geirskogul);

                            //Mirage Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && CanDRGWeave(MirageDive) && (HasEffect(Buffs.DiveReady) ||
                               (IsEnabled(CustomComboPreset.DRG_AoE_Optimized_Rotation) && IsOnCooldown(OriginalHook(Geirskogul)) && HasEffect(Buffs.DiveReady))))
                                return MirageDive;

                            //Wyrmwind Thrust Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                                gauge.FirstmindsFocusCount is 2 && CanDRGWeave(WyrmwindThrust))
                                return WyrmwindThrust;
                        }
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= AoE_secondWindTreshold &&
                            ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= AoE_bloodBathTreshold &&
                            ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                            return SonicThrust;

                        if (lastComboMove is SonicThrust && LevelChecked(CoerthanTorment))
                            return CoerthanTorment;
                    }

                    return OriginalHook(DoomSpike);
                }

                return actionID;
            }
        }

        internal class DRG_JumpFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_Jump;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is DRG.Jump or DRG.HighJump && HasEffect(DRG.Buffs.DiveReady) ? DRG.MirageDive : actionID;
        }

        internal class DRG_StardiverFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_StardiverFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRGGauge? gauge = GetJobGauge<DRGGauge>();

                if (actionID is Stardiver)
                {
                    if (gauge.IsLOTDActive && ActionReady(Stardiver))
                        return Stardiver;

                    if ((LevelChecked(Geirskogul) && !gauge.IsLOTDActive) || gauge.IsLOTDActive)
                        return OriginalHook(Geirskogul);
                }

                return actionID;
            }
        }

        internal class DRG_BurstCDFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_BurstCDFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is LanceCharge)
                {
                    if (IsOnCooldown(LanceCharge))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_BurstCDFeature_DragonSight) &&
                            ActionReady(DragonSight))
                            return DragonSight;

                        if (ActionReady(BattleLitany))
                            return BattleLitany;
                    }
                }

                return actionID;
            }
        }
    }
}

