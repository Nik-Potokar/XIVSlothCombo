using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Core;

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
            public const string
                DRG_ST_DiveOptions = "DRG_ST_DiveOptions",
                DRG_AOE_DiveOptions = "DRG_AOE_DiveOptions",
                DRG_OpenerOptions = "DRG_OpenerOptions",
                DRG_STSecondWindThreshold = "DRG_STSecondWindThreshold",
                DRG_STBloodbathThreshold = "DRG_STBloodbathThreshold",
                DRG_AoESecondWindThreshold = "DRG_AoESecondWindThreshold",
                DRG_AoEBloodbathThreshold = "DRG_AoEBloodbathThreshold",
                DRG_VariantCure = "DRG_VariantCure";
        }

        internal class DRG_JumpFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_Jump;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) => 
                actionID is DRG.Jump or DRG.HighJump && HasEffect(DRG.Buffs.DiveReady) ? DRG.MirageDive : actionID;
        }

        internal class DRG_STCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_STCombo;
            internal static bool inOpener = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<DRGGauge>();
                bool openerReady = IsOffCooldown(LanceCharge) && IsOffCooldown(BattleLitany);
                var diveOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_ST_DiveOptions);
                var openerOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_OpenerOptions);

                Status? ChaosDoTDebuff;
                if (LevelChecked(ChaoticSpring)) ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaoticSpring);
                else ChaosDoTDebuff = FindTargetEffect(Debuffs.ChaosThrust);

                if (actionID is TrueThrust)
                {
                    // Lvl88+ Opener
                    if (!InCombat() && IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88)
                    {
                        inOpener = false;

                        if (HasEffect(Buffs.TrueNorth) && openerReady)
                            inOpener = true;
                        if (inOpener)
                            return OriginalHook(TrueThrust);
                    }

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) && LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (InCombat())
                    {
                        if (CombatEngageDuration().TotalSeconds < 3 && IsOnCooldown(ElusiveJump) && openerReady)
                            inOpener = true;

                        if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
                            return Variant.VariantCure;

                        if (inOpener)
                        {
                            if (IsOnCooldown(BattleLitany) && !HasEffect(Buffs.LanceCharge))
                                inOpener = false;

                            //oGCDs
                            if (CanWeave(actionID))
                            {
                                if (WasLastWeaponskill(Disembowel) && openerOptions is 0 or 1 or 2)
                                {
                                    if (ActionReady(LanceCharge))
                                        return LanceCharge;
                                    if (ActionReady(DragonSight))
                                        return DragonSight;
                                }

                                if (WasLastWeaponskill(ChaoticSpring))
                                {
                                    if (openerOptions is 0 or 1 or 2 && ActionReady(BattleLitany))
                                        return BattleLitany;
                                    if (openerOptions is 2 && GetRemainingCharges(SpineshatterDive) > 1)
                                        return OriginalHook(SpineshatterDive);
                                }

                                if (WasLastWeaponskill(WheelingThrust) && openerOptions is 0 or 1 or 2)
                                {
                                    if (ActionReady(Geirskogul))
                                        return Geirskogul;
                                    if (GetRemainingCharges(LifeSurge) > 0 && !HasEffect(Buffs.LifeSurge))
                                        return LifeSurge;
                                }

                                if (WasLastWeaponskill(FangAndClaw))
                                {
                                    if (openerOptions is 0 or 1)
                                    {
                                        if (GetRemainingCharges(SpineshatterDive) < 2 && !WasLastAction(SpineshatterDive))
                                            return SpineshatterDive;
                                        if (ActionReady(OriginalHook(Jump)) && !HasEffect(Buffs.DiveReady))
                                            return OriginalHook(Jump);
                                    }

                                    if (openerOptions is 2)
                                    {
                                        if (ActionReady(OriginalHook(Jump)))
                                            return OriginalHook(Jump);
                                        if (HasEffect(Buffs.DiveReady))
                                            return MirageDive;
                                    }
                                }

                                if (WasLastWeaponskill(RaidenThrust))
                                {
                                    if (openerOptions is 0 or 1 or 2 && ActionReady(DragonfireDive))
                                        return DragonfireDive;
                                }

                                if (WasLastWeaponskill(VorpalThrust))
                                {
                                    if (openerOptions is 0 or 1)
                                    {
                                        if (GetRemainingCharges(LifeSurge) > 0 && !HasEffect(Buffs.LifeSurge))
                                            return LifeSurge;
                                        if (HasEffect(Buffs.DiveReady))
                                            return MirageDive;
                                    }

                                    if (openerOptions is 2)
                                    {
                                        if (ActionReady(SpineshatterDive))
                                            return SpineshatterDive;
                                        if (GetRemainingCharges(LifeSurge) > 0 && !HasEffect(Buffs.LifeSurge))
                                            return LifeSurge;
                                    }
                                }

                                if (WasLastWeaponskill(HeavensThrust) && GetRemainingCharges(SpineshatterDive) > 0 && !WasLastAction(SpineshatterDive) && openerOptions is 0 or 1)
                                    return SpineshatterDive;
                            }
                        }

                        if (!inOpener)
                        {
                            if (CanWeave(actionID))
                            {
                                if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                                    IsEnabled(Variant.VariantRampart) &&
                                    IsOffCooldown(Variant.VariantRampart) &&
                                    CanWeave(actionID))
                                    return Variant.VariantRampart;

                                if (HasEffect(Buffs.PowerSurge))
                                {
                                    //Wyrmwind Thrust Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_CDs) && IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) && gauge.FirstmindsFocusCount is 2)
                                        return WyrmwindThrust;

                                    if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                                    {
                                        //Lance Charge Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_Lance) && LevelChecked(LanceCharge) && IsOffCooldown(LanceCharge))
                                            return LanceCharge;

                                        //Dragon Sight Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) && LevelChecked(DragonSight) && IsOffCooldown(DragonSight))
                                            return DragonSight;

                                        //Battle Litany Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_Litany) && LevelChecked(BattleLitany) && IsOffCooldown(BattleLitany) && CanWeave(actionID, 1.3))
                                            return BattleLitany;
                                    }

                                    if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                                    {
                                        //Geirskogul and Nastrond Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_GeirskogulNastrond) && LevelChecked(Geirskogul) && ((gauge.IsLOTDActive && IsOffCooldown(Nastrond)) || IsOffCooldown(Geirskogul)))
                                            return OriginalHook(Geirskogul);

                                        //(High) Jump Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) && ActionReady(OriginalHook(Jump)))
                                            return OriginalHook(Jump);

                                        //Mirage Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && HasEffect(Buffs.DiveReady))
                                            return MirageDive;

                                        //Life Surge Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && !HasEffect(Buffs.LifeSurge) && GetRemainingCharges(LifeSurge) > 0 &&
                                            (((HasEffect(Buffs.RightEye) || HasEffect(Buffs.LanceCharge)) && lastComboMove is VorpalThrust) ||
                                            (HasEffect(Buffs.BattleLitany) && ((HasEffect(Buffs.EnhancedWheelingThrust) && WasLastWeaponskill(FangAndClaw)) || HasEffect(Buffs.SharperFangAndClaw) && WasLastWeaponskill(WheelingThrust)))))
                                            return LifeSurge;

                                        //Dives Feature
                                        if (IsEnabled(CustomComboPreset.DRG_ST_Dives) && (IsNotEnabled(CustomComboPreset.DRG_ST_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_ST_Dives_Melee) && GetTargetDistance() <= 1)))
                                        {
                                            if (diveOptions is 0 or 1 or 2 or 3 && gauge.IsLOTDActive && ActionReady(Stardiver) && IsOnCooldown(DragonfireDive))
                                                return Stardiver;

                                            if (diveOptions is 0 or 1 || //Dives on cooldown
                                               (diveOptions is 2 && ((gauge.IsLOTDActive && LevelChecked(Nastrond)) || !LevelChecked(Nastrond)) && HasEffectAny(Buffs.BattleLitany)) || //Dives under Litany and Life of the Dragon
                                               (diveOptions is 3 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                            {
                                                if (LevelChecked(DragonfireDive) && IsOffCooldown(DragonfireDive))
                                                    return DragonfireDive;
                                                if (LevelChecked(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                                    return SpineshatterDive;
                                            }
                                        }
                                    }
                                }

                                // healing - please move if not appropriate this high priority
                                if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
                                {
                                    if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_STSecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                                        return All.SecondWind;
                                    if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_STBloodbathThreshold) && LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath))
                                        return All.Bloodbath;
                                }
                            }
                        }

                        //1-2-3 Combo
                        if (HasEffect(Buffs.SharperFangAndClaw))
                            return FangAndClaw;
                        if (HasEffect(Buffs.EnhancedWheelingThrust))
                            return WheelingThrust;
                        if (comboTime > 0)
                        {
                            if (ChaosDoTDebuff is null || ChaosDoTDebuff.RemainingTime < 6 || GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                            {
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                                    return Disembowel;
                                if (lastComboMove is Disembowel && LevelChecked(ChaosThrust))
                                    return OriginalHook(ChaosThrust);
                            }

                            if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                                return VorpalThrust;
                            if (lastComboMove is VorpalThrust && LevelChecked(FullThrust))
                                return OriginalHook(FullThrust);
                        }

                    }

                    return OriginalHook(TrueThrust);
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
                    var DiveOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_AOE_DiveOptions);

                    if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DRG_VariantCure))
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
                                if (LevelChecked(LanceCharge) && IsOffCooldown(LanceCharge))
                                    return LanceCharge;
                                if (LevelChecked(BattleLitany) && IsOffCooldown(BattleLitany))
                                    return BattleLitany;

                                //Dragon Sight AoE Feature
                                if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) && LevelChecked(DragonSight) && IsOffCooldown(DragonSight))
                                    return DragonSight;
                            }

                            //Geirskogul and Nastrond AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && LevelChecked(Geirskogul) && ((gauge.IsLOTDActive && IsOffCooldown(Nastrond)) || IsOffCooldown(Geirskogul)))
                                return OriginalHook(Geirskogul);

                            //(High) Jump AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) && ActionReady(OriginalHook(Jump)) && CanWeave(actionID, 1))
                                return OriginalHook(Jump);

                            //Mirage Dive Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && HasEffect(Buffs.DiveReady))
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
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Dives) && (IsNotEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) || (IsEnabled(CustomComboPreset.DRG_AoE_Dives_Melee) && GetTargetDistance() <= 1)))
                            {
                                if (DiveOptions is 0 or 1 or 2 or 3 && gauge.IsLOTDActive && LevelChecked(Stardiver) && IsOffCooldown(Stardiver) && CanWeave(actionID, 1.3) && IsOnCooldown(DragonfireDive))
                                    return Stardiver;

                                if (DiveOptions is 0 or 1 || //Dives on cooldown
                                   (DiveOptions is 2 && ((LevelChecked(Nastrond) && gauge.IsLOTDActive) || !LevelChecked(Nastrond)) && HasEffectAny(Buffs.BattleLitany)) || //Dives under Litany and Life of the Dragon
                                   (DiveOptions is 3 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                {
                                    if (LevelChecked(DragonfireDive) && IsOffCooldown(DragonfireDive))
                                        return DragonfireDive;
                                    if (LevelChecked(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                        return SpineshatterDive;
                                }
                            }
                        }

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_AoESecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                                return All.SecondWind;
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.DRG_AoEBloodbathThreshold) && LevelChecked(All.Bloodbath) && IsOffCooldown(All.Bloodbath))
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
