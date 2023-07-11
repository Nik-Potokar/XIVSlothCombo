using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            TrueNorth = 7546,
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

        public static class Config
        {

            public static UserInt
                DRG_ST_DiveOptions = new("DRG_ST_DiveOptions"),
                DRG_AOE_DiveOptions = new("DRG_AOE_DiveOptions"),
                DRG_OpenerOptions = new("DRG_OpenerOptions"),
                DRG_VariantCure = new("DRG_VariantCure"),
                DRG_STSecondWindThreshold = new("DRG_STSecondWindThreshold"),
                DRG_STBloodbathThreshold = new("DRG_STBloodbathThreshold"),
                DRG_AoESecondWindThreshold = new("DRG_AoESecondWindThreshold"),
                DRG_AoEBloodbathThreshold = new("DRG_AoEBloodbathThreshold");
        }

        internal class DRG_STCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_STCombo;
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

                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    // Opener for DRG
                    // Standard opener - 2.5
                    if (IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88)
                    {
                        if (openerSelection is 0)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.TrueNorth))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(Buffs.TrueNorth) && !inOpener)
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
                            if ((step == 0 && lastComboMove is TrueThrust && !HasEffect(Buffs.TrueNorth)) ||
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
                            if (openerStarted && HasEffect(Buffs.TrueNorth))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(Buffs.TrueNorth) && !inOpener)
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
                            if ((step == 0 && lastComboMove is TrueThrust && !HasEffect(Buffs.TrueNorth)) ||
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

                        // Piercing Talon Uptime Option
                        if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) && LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                            return PiercingTalon;

                        if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                            return Variant.VariantCure;

                        if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                                IsEnabled(Variant.VariantRampart) &&
                                IsOffCooldown(Variant.VariantRampart) &&
                                CanWeave(actionID))
                            return Variant.VariantRampart;

                        if (HasEffect(Buffs.PowerSurge))
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                                {
                                    //Battle Litany Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Litany) && ActionReady(BattleLitany) && CanWeave(actionID, 1.3))
                                        return BattleLitany;

                                    //Lance Charge Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Lance) && ActionReady(LanceCharge))
                                        return LanceCharge;

                                    //Dragon Sight Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) && ActionReady(DragonSight))
                                        return DragonSight;
                                }

                                if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                                {
                                    //Life Surge Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && !HasEffect(Buffs.LifeSurge) && GetRemainingCharges(LifeSurge) > 0 &&
                                        (((HasEffect(Buffs.RightEye) || HasEffect(Buffs.LanceCharge)) && lastComboMove is VorpalThrust) ||
                                        (HasEffect(Buffs.BattleLitany) && ((HasEffect(Buffs.EnhancedWheelingThrust) && WasLastWeaponskill(FangAndClaw)) ||
                                        HasEffect(Buffs.SharperFangAndClaw) && WasLastWeaponskill(WheelingThrust)))))
                                        return LifeSurge;

                                    //(High) Jump Feature   
                                    if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) && ActionReady(OriginalHook(Jump)) &&
                                        !IsMoving)
                                        return OriginalHook(Jump);

                                    //Geirskogul and Nastrond Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_GeirskogulNastrond) && ActionReady(OriginalHook(Geirskogul)) &&
                                        IsOnCooldown(OriginalHook(Jump)))
                                        return OriginalHook(Geirskogul);

                                    //Mirage Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && HasEffect(Buffs.DiveReady) && IsOnCooldown(OriginalHook(Geirskogul)))
                                        return MirageDive;

                                    //Wyrmwind Thrust Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) && gauge.FirstmindsFocusCount is 2)
                                        return WyrmwindThrust;
                                }

                                //Dives Feature
                                if (!IsMoving)
                                {
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Dives) && (IsNotEnabled(CustomComboPreset.DRG_ST_Dives_Melee) ||
                                        (IsEnabled(CustomComboPreset.DRG_ST_Dives_Melee) && GetTargetDistance() <= 1)))
                                    {
                                        if ((ActionWatching.GetAttackType(ActionWatching.LastAction) != ActionWatching.ActionAttackType.Ability) &&
                                            diveOptions is 0 or 1 or 2 or 3 && gauge.IsLOTDActive && ActionReady(Stardiver) &&
                                            IsOnCooldown(DragonfireDive) &&
                                            (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                            return Stardiver;


                                        if (diveOptions is 0 or 1 || //Dives on cooldown
                                           (diveOptions is 2 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                           (diveOptions is 3 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                        {
                                            if (ActionReady(DragonfireDive))
                                                return DragonfireDive;

                                            if (ActionReady(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                                return SpineshatterDive;
                                        }
                                    }
                                }
                            }
                        }

                        // healing - please move if not appropriate this high priority 
                        if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_STSecondWindThreshold)
                                && ActionReady(All.SecondWind))
                                return All.SecondWind;

                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_STBloodbathThreshold) &&
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
                            bool canChaosThrust = LevelChecked(ChaosThrust);
                            if ((canChaosThrust && (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6)) ||
                                GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                            {
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                    return Disembowel;

                                if (lastComboMove is Disembowel && canChaosThrust)
                                    return OriginalHook(ChaosThrust);
                            }

                            if (lastComboMove is TrueThrust or RaidenThrust)
                                return VorpalThrust;

                            if (lastComboMove is VorpalThrust)
                                return OriginalHook(FullThrust);
                        }
                        return OriginalHook(TrueThrust);
                    }
                }
                return actionID;
            }
        }

        internal class DRG_AoECombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AoECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DoomSpike)
                {
                    var gauge = GetJobGauge<DRGGauge>();
                    var diveOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_AOE_DiveOptions);

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                        return Variant.VariantCure;

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) && LevelChecked(PiercingTalon) && GetTargetDistance() > 10 && HasBattleTarget())
                        return PiercingTalon;

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanWeave(actionID))
                            return Variant.VariantRampart;

                        if (HasEffect(Buffs.PowerSurge))
                        {
                            //Buffs AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                            {
                                if (ActionReady(LanceCharge))
                                    return LanceCharge;

                                if (ActionReady(BattleLitany))
                                    return BattleLitany;

                                //Dragon Sight AoE Feature
                                if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) && ActionReady(DragonSight))
                                    return DragonSight;
                            }

                            //(High) Jump AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) && ActionReady(OriginalHook(Jump)) && !IsMoving)
                                return OriginalHook(Jump);

                            //Geirskogul and Nastrond AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && ActionReady(OriginalHook(Geirskogul)) && IsOnCooldown(OriginalHook(Jump)))
                                return OriginalHook(Geirskogul);

                            //Mirage Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && HasEffect(Buffs.DiveReady) && IsOnCooldown(OriginalHook(Geirskogul)))
                                return MirageDive;

                            //Life Surge AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                                !HasEffect(Buffs.LifeSurge) && GetRemainingCharges(LifeSurge) > 0 && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye)) &&
                                ((lastComboMove is CoerthanTorment && LevelChecked(CoerthanTorment)) ||
                                (lastComboMove is SonicThrust && LevelChecked(SonicThrust) && !LevelChecked(CoerthanTorment)) ||
                                (lastComboMove is DoomSpike && !LevelChecked(SonicThrust))))
                                return LifeSurge;

                            //Wyrmwind Thrust AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_WyrmwindFeature) && gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Dives AoE Feature
                            if (!IsMoving)
                            {
                                if (IsEnabled(CustomComboPreset.DRG_AoE_Dives) && (IsNotEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) && GetTargetDistance() <= 1)))
                                {
                                    if (diveOptions is 0 or 1 or 2 or 3 && gauge.IsLOTDActive && ActionReady(Stardiver) && IsOnCooldown(DragonfireDive) &&
                                        (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye) || HasEffect(Buffs.BattleLitany)))
                                        return Stardiver;

                                    if (diveOptions is 0 or 1 || //Dives on cooldown
                                        (diveOptions is 2 && HasEffect(Buffs.LanceCharge) && HasEffect(Buffs.RightEye)) || //Dives under LanceCharge and Dragon Sight -- optimized with the balance
                                       (diveOptions is 3 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                    {
                                        if (ActionReady(DragonfireDive))
                                            return DragonfireDive;

                                        if (ActionReady(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                            return SpineshatterDive;
                                    }
                                }
                            }
                        }

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_AoESecondWindThreshold) &&
                                ActionReady(All.SecondWind))
                                return All.SecondWind;
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_AoEBloodbathThreshold) &&
                                ActionReady(All.Bloodbath))
                                return All.Bloodbath;
                        }
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
                if (actionID is Stardiver)
                {
                    var gauge = GetJobGauge<DRGGauge>();

                    if (gauge.IsLOTDActive && IsOffCooldown(Stardiver) && LevelChecked(Stardiver))
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
                        if (IsEnabled(CustomComboPreset.DRG_BurstCDFeature_DragonSight) && IsOffCooldown(DragonSight) && LevelChecked(DragonSight))
                            return DragonSight;
                        if (LevelChecked(BattleLitany) && IsOffCooldown(BattleLitany))
                            return BattleLitany;
                    }
                }

                return actionID;
            }
        }
    }
}
