using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class MCH
    {
        public const byte JobID = 31;

        internal const uint
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            GaussRound = 2874,
            Ricochet = 2890,
            HeatedSlugshot = 7412,
            Drill = 16498,
            HotShot = 2872,
            Reassemble = 2876,
            AirAnchor = 16500,
            Hypercharge = 17209,
            HeatBlast = 7410,
            SpreadShot = 2870,
            Scattergun = 25786,
            AutoCrossbow = 16497,
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
            Tactician = 16889,
            ChainSaw = 25788,
            BioBlaster = 16499,
            BarrelStabilizer = 7414,
            Wildfire = 2878;

        internal static class Buffs
        {
            internal const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946;
        }

        private static MCHGauge Gauge => CustomComboFunctions.GetJobGauge<MCHGauge>();

        internal static class Config
        {
            internal const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",
                MCH_ST_QueenThreshold = "MCH_ST_QueenThreshold",
                MCH_OpenerSelection = "MCH_OpenerSelection",
                MCH_VariantCure = "MCH_VariantCure";
        }

        internal static class Levels
        {
            internal const byte
                SlugShot = 2,
                Hotshot = 4,
                GaussRound = 15,
                CleanShot = 26,
                Hypercharge = 30,
                HeatBlast = 35,
                RookOverdrive = 40,
                Wildfire = 45,
                Ricochet = 50,
                Drill = 58,
                AirAnchor = 76,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                Tactician = 56,
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                BioBlaster = 72,
                ChargedActionMastery = 74,
                QueenOverdrive = 80,
                Scattergun = 82,
                BarrelStabilizer = 66,
                ChainSaw = 90;
        }
        
     /*   internal class MCH_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == CleanShot || actionID == HeatedCleanShot || actionID == SplitShot || actionID == HeatedSplitShot)
                {
                    var gauge = GetJobGauge<MCHGauge>();
                    var drillCD = GetCooldown(Drill);
                    var airAnchorCD = GetCooldown(AirAnchor);
                    var hotshotCD = GetCooldown(HotShot);

                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var chainsawCD = GetCooldown(ChainSaw);

                    var battery = GetJobGauge<MCHGauge>().Battery;
                    var heat = GetJobGauge<MCHGauge>().Heat;
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_ST_BarrelStabilizer_DriftProtection))
                    {
                        if (level >= Levels.BarrelStabilizer && heat < 20 && canWeave && IsOffCooldown(BarrelStabilizer))
                            return BarrelStabilizer;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_HeatBlast) && gauge.IsOverheated)
                    {
                        if (CanWeave(actionID, 0.6))
                        {
                            if (level <= Levels.Ricochet && HasCharges(GaussRound))
                                return GaussRound;

                            if (HasCharges(GaussRound) && gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                                return GaussRound;
                            else if (level >= Levels.Ricochet && HasCharges(Ricochet))
                                return Ricochet;
                        }

                        if (level >= Levels.HeatBlast) // prioritize heatblast
                            return HeatBlast;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_Cooldowns))
                    {
                        if (HasEffect(Buffs.Reassembled) && !airAnchorCD.IsCooldown && level >= Levels.AirAnchor)
                        {
                            return AirAnchor;
                        }
                        if (airAnchorCD.IsCooldown && !drillCD.IsCooldown && level >= Levels.Drill)
                            return Drill;
                        if (HasEffect(Buffs.Reassembled) && !chainsawCD.IsCooldown && level >= Levels.ChainSaw)
                            return ChainSaw;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGaussCharges) && CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (level >= Levels.Ricochet && HasCharges(Ricochet))
                            return Ricochet;
                        if (level >= Levels.GaussRound && HasCharges(GaussRound))
                            return GaussRound;

                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGauss) && CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (level >= Levels.Ricochet && GetRemainingCharges(Ricochet) > 1)
                            return Ricochet;
                        if (level >= Levels.GaussRound && GetRemainingCharges(GaussRound) > 1)
                            return GaussRound;

                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainComboAlternate))
                    {
                        if (level >= Levels.AirAnchor && !airAnchorCD.IsCooldown && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                        {
                            return AirAnchor;
                        }
                        if (level >= Levels.ChainSaw && !chainsawCD.IsCooldown && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return ChainSaw;
                        if (level >= Levels.Drill && !drillCD.IsCooldown && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return Drill;
                        if (level < Levels.AirAnchor && !hotshotCD.IsCooldown && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return HotShot;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_OverCharge) && canWeave)
                    {
                        if (battery == 100 && level is >= 40 and <= 79)
                            return RookAutoturret;
                        if (battery == 100 && level >= 80)
                            return AutomatonQueen;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == SplitShot && level >= Levels.SlugShot)
                            return OriginalHook(SlugShot);

                        if (lastComboMove == SlugShot && level >= Levels.CleanShot)
                            return OriginalHook(CleanShot);
                    }
                    return OriginalHook(SplitShot);
                }
                return actionID;

            }
        }

        internal class MCH_HeatblastGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HeatblastGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HeatBlast)
                {
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);

                    var gauge = GetJobGauge<MCHGauge>();
                    var heat = GetJobGauge<MCHGauge>().Heat;
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel)
                        && level >= Levels.BarrelStabilizer
                        && heat < 50
                        && IsOffCooldown(BarrelStabilizer)
                        && !gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Wildfire)
                        && IsOffCooldown(Hypercharge)
                        && IsOffCooldown(Wildfire)
                        && level >= Levels.Wildfire
                        && heat >= 50)
                        return Wildfire;

                    if (!gauge.IsOverheated && level >= Levels.Hypercharge)
                        return Hypercharge;

                    if (heatBlastCD.CooldownRemaining < 0.7 && level >= Levels.HeatBlast) // Prioritize Heat Blast
                        return HeatBlast;

                    if (level <= Levels.Ricochet)
                        return GaussRound;

                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    return Ricochet;
                }

                return actionID;
            }
        }

        internal class MCH_GaussRoundRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_GaussRoundRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == GaussRound || actionID == Ricochet)
                {
                    var gaussCd = GetCooldown(GaussRound);
                    var ricochetCd = GetCooldown(Ricochet);

                    // Prioritize the original if both are off cooldown
                    if (level <= Levels.Ricochet)
                        return GaussRound;

                    if (!gaussCd.IsCooldown && !ricochetCd.IsCooldown)
                        return actionID;

                    if (gaussCd.CooldownRemaining < ricochetCd.CooldownRemaining)
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }

        internal class MCH_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SpreadShot || actionID == Scattergun)
                {
                    var canWeave = CanWeave(actionID);
                    var gauge = GetJobGauge<MCHGauge>();
                    var battery = GetJobGauge<MCHGauge>().Battery;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_OverCharge) && canWeave)
                    {
                        if (battery == 100 && level >= Levels.QueenOverdrive)
                            return AutomatonQueen;
                        if (battery == 100 && level >= Levels.RookOverdrive)
                            return RookAutoturret;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && canWeave && (IsEnabled(CustomComboPreset.MCH_AoE_Gauss) || gauge.IsOverheated) && (HasCharges(Ricochet) || HasCharges(GaussRound)))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) &&
                            level >= Levels.GaussRound)
                            return GaussRound;
                        else if (ricochetCharges > 0 && level >= Levels.Ricochet)
                            return Ricochet;

                    }

                    if (IsOffCooldown(BioBlaster) && level >= Levels.BioBlaster && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster))
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Hypercharge) && canWeave)
                    {
                        if (gauge.Heat >= 50 && level >= Levels.AutoCrossbow && !gauge.IsOverheated)
                            return Hypercharge;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_SecondWindThreshold) && (LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind)))
                            return All.SecondWind;
                    }

                    if (gauge.IsOverheated && level >= Levels.AutoCrossbow)
                        return AutoCrossbow;

                }

                return actionID;
            }
        }

        internal class MCH_Overdrive : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == RookAutoturret || actionID == AutomatonQueen)
                {
                    var gauge = GetJobGauge<MCHGauge>();
                    if (gauge.IsRobotActive)
                        return OriginalHook(QueenOverdrive);
                }

                return actionID;
            }

            internal class MCH_HotShotDrillChainSaw : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainSaw;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == Drill || actionID == HotShot || actionID == AirAnchor)
                    {
                        if (level >= Levels.ChainSaw)
                            return CalcBestAction(actionID, ChainSaw, AirAnchor, Drill);

                        if (level >= Levels.AirAnchor)
                            return CalcBestAction(actionID, AirAnchor, Drill);

                        if (level >= Levels.Drill)
                            return CalcBestAction(actionID, Drill, HotShot);

                        return HotShot;
                    }

                    return actionID;
                }
            }
        }

        internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbowGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AutoCrossbow)
                {
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var heat = GetJobGauge<MCHGauge>().Heat;

                    var gauge = GetJobGauge<MCHGauge>();
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel)
                        && level >= Levels.BarrelStabilizer
                        && heat < 50
                        && IsOffCooldown(BarrelStabilizer)
                        && !gauge.IsOverheated
                       ) return BarrelStabilizer;

                    if (!gauge.IsOverheated && level >= Levels.Hypercharge)
                        return Hypercharge;
                    if (heatBlastCD.CooldownRemaining < 0.7 && level >= Levels.AutoCrossbow) // prioritize autocrossbow
                        return AutoCrossbow;
                    if (level <= Levels.Ricochet)
                        return GaussRound;
                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }
        */
        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SplitShot)
                {
                    int openerSelection = PluginConfiguration.GetCustomIntValue(Config.MCH_OpenerSelection);
                    bool openerReady = ActionReady(ChainSaw) && ActionReady(Wildfire) && ActionReady(BarrelStabilizer);
                    float wildfireCDTime = GetCooldownRemainingTime(Wildfire);

                    if (IsEnabled(CustomComboPreset.MCH_ST_Opener) && level >= 90)
                    {
                        if (openerSelection is 0 or 1)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Reassembled)) { inOpener = true; openerStarted = false; readyOpener = false; }
                            if ((readyOpener || openerStarted) && HasEffect(Buffs.Reassembled) && !inOpener) { openerStarted = true; return AirAnchor; } else { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return Reassemble;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                            if ((step == 1 && lastComboMove is AirAnchor && !HasEffect(Buffs.Reassembled))
                                || (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat())) inOpener = false;


                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Reassembled) &&
                                IsEnabled(CustomComboPreset.MCH_ST_Opener) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                //we do it in steps to be able to control it
                                if (step is 0)
                                {
                                    if (IsOnCooldown(AirAnchor)) step++;
                                    else return AirAnchor;
                                }

                                if (step == 1)
                                {
                                    if (GetRemainingCharges(GaussRound) < 3) step++;
                                    else return GaussRound;
                                }

                                if (step == 2)
                                {
                                    if (GetRemainingCharges(Ricochet) < 3) step++;
                                    else return Ricochet;
                                }

                                if (step == 3)
                                {
                                    if (IsOnCooldown(Drill)) step++;
                                    else return Drill;
                                }

                                if (step == 4)
                                {
                                    if (IsOnCooldown(BarrelStabilizer)) step++;
                                    else return BarrelStabilizer;
                                }

                                if (step == 5)
                                {
                                    if (WasLastWeaponskill(HeatedSplitShot)) step++;
                                    else return HeatedSplitShot;
                                }

                                if (step == 6)
                                {
                                    if (WasLastWeaponskill(HeatedSlugshot)) step++;
                                    else return HeatedSlugshot;
                                }

                                if (step == 7)
                                {
                                    if (GetRemainingCharges(GaussRound) < 2) step++;
                                    else return GaussRound;
                                }

                                if (step == 8)
                                {
                                    if (GetRemainingCharges(Ricochet) < 2) step++;
                                    else return Ricochet;
                                }

                                if (step == 9)
                                {
                                    if (WasLastWeaponskill(HeatedCleanShot)) step++;
                                    else return HeatedCleanShot;
                                }

                                if (step == 10)
                                {
                                    if (GetRemainingCharges(Reassemble) is 0) step++;
                                    else return Reassemble;
                                }

                                if (step == 11)
                                {
                                    if (IsOnCooldown(Wildfire)) step++;
                                    else return Wildfire;
                                }

                                if (step == 12)
                                {
                                    if (IsOnCooldown(ChainSaw)) step++;
                                    else return ChainSaw;
                                }

                                if (step == 13)
                                {
                                    if (IsOnCooldown(AutomatonQueen)) step++;
                                    else return AutomatonQueen;
                                }

                                if (step == 14)
                                {
                                    if (IsOnCooldown(Hypercharge)) step++;
                                    else return Hypercharge;
                                }

                                if (step == 15)
                                {
                                    if (WasLastWeaponskill(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 16)
                                {
                                    if (GetRemainingCharges(Ricochet) < 2) step++;
                                    else return Ricochet;
                                }

                                if (step == 17)
                                {
                                    if (WasLastWeaponskill(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 18)
                                {
                                    if (GetRemainingCharges(GaussRound) < 2) step++;
                                    else return GaussRound;
                                }

                                if (step == 19)
                                {
                                    if (WasLastWeaponskill(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 20)
                                {
                                    if (GetRemainingCharges(Ricochet) < 2) step++;
                                    else return Ricochet;
                                }

                                if (step == 21)
                                {
                                    if (WasLastWeaponskill(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 22)
                                {
                                    if (GetRemainingCharges(GaussRound) < 2) step++;
                                    else return GaussRound;
                                }

                                if (step == 23)
                                {
                                    if (WasLastWeaponskill(HeatBlast)) step++;
                                    else return HeatBlast;
                                }

                                if (step == 24)
                                {
                                    if (GetRemainingCharges(Ricochet) < 2) step++;
                                    else return Ricochet;
                                }

                                if (step == 2)
                                {
                                    if (IsOnCooldown(Drill)) step++;
                                    else return Drill;
                                }

                                inOpener = false;
                            }
                        }
                    }
                        /*
                        if (!inOpener)
                        {
                            if (openerSelection is 2 && (!InCombat() || inOpener) && HasBattleTarget() &&
                             GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble) &&
                             Gauge.Heat == 0 && Gauge.Battery == 0 &&
                             !HasEffect(All.Buffs.Weakness) &&
                             IsOffCooldown(AirAnchor) && IsOffCooldown(Drill)) //this code is kinda all over the place right now. can you tell?
                         {
                             return OriginalHook(SplitShot);
                         }
                         if (openerSelection is 0 or 1 && lastComboMove == CleanShot)
                         {
                             inOpener = false;
                         }
                         if (openerSelection is 2 && WasLastWeaponskill(AirAnchor))
                         {
                             inOpener = false;
                         }
                         if (WasLastWeaponskill(Drill) && !WasLastWeaponskill(OriginalHook(SplitShot)) && IsOffCooldown(BarrelStabilizer))
                         {
                             //Barrel Stabilizer, need to tighten this a bit maybe
                             return BarrelStabilizer;
                         }
                         else if (!inOpener && lastComboMove == CleanShot)
                         {
                             inOpener = false;

                         }

                            // uhh should be 2nd gcd in theory but idk why. might line up stuff better down the line and solve misaligned issues like why BS & WF feature needed
                            if (CombatEngageDuration().Minutes >= 2 && CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && 
                                Gauge.Heat <= 55 &&
                                    IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && !WasLastWeaponskill(ChainSaw) &&
                                    (wildfireCDTime <= 9 || (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only) && Gauge.IsOverheated)))
                            {
                                return BarrelStabilizer;
                            }

                            // Queen
                          /*  if ((CanWeave(actionID) || (CanWeave(actionID, 0.6))) && !inOpener && !Gauge.IsRobotActive && 
                                IsEnabled(CustomComboPreset.MCH_ST_QueenThreshold) && (wildfireCDTime >= 2 &&
                                !WasLastAbility(Wildfire) || level < Levels.Wildfire))
                            {
                                //queen slider for accurate-ish punches under buff windows 
                                if (openerSelection is 2 && level >= Levels.RookOverdrive && Gauge.Battery >= 50)
                                {
                                    if (CombatEngageDuration().Minutes == 0 && !WasLastWeaponskill(OriginalHook(CleanShot)))
                                    {
                                        return OriginalHook(RookAutoturret);
                                    }

                                    else if (CombatEngageDuration().Minutes % 2 == 0 && (CombatEngageDuration().Seconds >= 01 && 
                                        CombatEngageDuration().Seconds <= queenThreshold))
                                    {
                                        return OriginalHook(RookAutoturret);
                                    }

                                    else if (CombatEngageDuration().Minutes % 2 == 1 && (CombatEngageDuration().Seconds >= 55 && CombatEngageDuration().Seconds <= 59))
                                    {
                                        return OriginalHook(RookAutoturret);
                                    }
                                } 

                                //fix this later for General Purpose Opener 2nd Queen
                                if (openerSelection is 0 or 1 && level >= Levels.RookOverdrive && Gauge.Battery >= 70 &&
                                   (CombatEngageDuration().Minutes == 0 && !WasLastWeaponskill(OriginalHook(CleanShot))))
                                {
                                    return OriginalHook(RookAutoturret);
                                }
                                //else if (LevelChecked(AutomatonQueen) && CombatEngageDuration().Seconds >= queenThreshold && 
                                //    gauge.Battery >= 80 && CombatEngageDuration().Minutes % 2 == 0 && gauge.Heat >= 100)
                                //{
                                //    Dalamud.Logging.PluginLog.Log("Queen only at 2:06 please");
                                //    return OriginalHook(RookAutoturret);
                                //}

                                //else if (gauge.Battery >= 50 && level >= Levels.RookOverdrive && (CombatEngageDuration().Seconds >= 58 || CombatEngageDuration().Seconds <= 05))
                                //{
                                //    return OriginalHook(RookAutoturret);
                                //}
                            }

                            // Interrupt, works okay
                            if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && 
                                CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                            {
                                return All.HeadGraze;
                            }

                            // Wildfire shenanigans
                            if ((Gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && 
                                level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge))
                            {
                                if (openerSelection is 0 or 1)
                                {
                                    if (//WasLastAction(Reassemble) &&
                                        IsOnCooldown(ChainSaw) && CanDelayedWeave(actionID))
                                        return Wildfire;
                                    else if (JustUsed(ChainSaw) && CanDelayedWeave(actionID) && CombatEngageDuration().TotalMinutes >= 2)
                                        return Wildfire;
                                }

                                if (openerSelection is 2)
                                {
                                    if (//WasLastAction(Reassemble) &&
                                        IsOffCooldown(ChainSaw) && CanDelayedWeave(actionID))
                                        return Wildfire;
                                    else if (//WasLastAction(Reassemble) &&
                                        (IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) <= 1.9) && (CanDelayedWeave(actionID)) && CombatEngageDuration().TotalMinutes >= 2)
                                        return Wildfire;
                                }
                                // 6.2 rotation does not use Wildfire within Hypercharge windows anymore due to minor drifting. Chainsaw and WF should be right next to each other always.
                                // Old code left here in case something breaks or death? lolol
                                else if (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && 
                                    !WasLastWeaponskill(HeatBlast))) //these try to ensure the correct loops
                                {
                                    if (CanDelayedWeave(actionID) && !Gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                                    {
                                        return Wildfire;
                                    }
                                    if (CanDelayedWeave(actionID) && !Gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                                    {
                                        return Wildfire;
                                    }
                                    else if (CanDelayedWeave(actionID, 1.1) && !Gauge.IsOverheated && WasLastWeaponskill(ChainSaw))
                                    {
                                        return Wildfire;
                                    }
                                    else if (CanWeave(actionID, 0.6) && Gauge.IsOverheated)
                                    {
                                        return Wildfire;
                                    }
                                }
                            }
                        
                            //Heatblast, Gauss, Rico
                            if (Gauge.IsOverheated && LevelChecked(HeatBlast))
                            {
                                if (CanWeave(actionID, 0.6) && IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && (wildfireCDTime > 2 || !LevelChecked(Wildfire))) //gauss and ricochet weave
                                {
                                    var gaussCharges = GetRemainingCharges(GaussRound);
                                    var gaussMaxCharges = GetMaxCharges(GaussRound);

                                    var overheatTime = Gauge.OverheatTimeRemaining;
                                    var reasmCharges = GetRemainingCharges(Reassemble);

                                    //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                                    if (overheatTime < 1.7 && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                        (
                                            (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && 
                                            reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                            ||
                                            (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                                            reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                            ||
                                            (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && 
                                            reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                        ))
                                        return Reassemble;

                                   //having issues with 2min queen
                                       else if (IsEnabled(CustomComboPreset.MCH_ST_QueenThreshold) && LevelChecked(AutomatonQueen) && (CombatEngageDuration().Seconds ==
                                                   queenThreshold && gauge.Battery >= 50))
                                       {
                                           return OriginalHook(RookAutoturret);
                                       }
                                       else if ((!IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))) ||
                                                  (IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && gaussCharges >= gaussMaxCharges - 1))
                                       {
                                           return GaussRound;
                                       }
                                       else if (level >= Levels.Ricochet && HasCharges(Ricochet) && !IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode))
                                       {
                                           return Ricochet;
                                       }

                                }

                                return HeatBlast;
                            }
                            // Hypercharge!
                            if (CanWeave(actionID) && Gauge.Heat >= 50 && 
                                IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && LevelChecked(Hypercharge) && !Gauge.IsOverheated)
                            {
                                //Protection & ensures Hyper charged is double weaved with WF during reopener
                                if (HasEffect(Buffs.Wildfire) || !LevelChecked(Wildfire))
                                    return Hypercharge;

                                if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                                {
                                    if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                                    {
                                        if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                        {
                                            if (UseHypercharge(Gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                        else if (!LevelChecked(ChainSaw))
                                        {
                                            if (UseHypercharge(Gauge, wildfireCDTime))
                                                return Hypercharge;
                                        }
                                    }
                                    else if (!LevelChecked(AirAnchor))
                                    {
                                        if (UseHypercharge(Gauge, wildfireCDTime))
                                            return Hypercharge;
                                    }
                                }
                                else if (!LevelChecked(Drill))
                                {
                                    if (UseHypercharge(Gauge, wildfireCDTime))
                                        return Hypercharge;
                                }
                            }
                            //Gauss & Rico Suave
                            if (CanWeave(actionID, 0.6) && IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet))
                            {
                                //Dalamud.Logging.PluginLog.Log("Tier 1 check"); this is kinda useful
                                if (openerSelection is 1 && openerReady && IsEnabled(CustomComboPreset.MCH_ST_Opener) && CombatEngageDuration().TotalSeconds < 3.5 && WasLastWeaponskill(AirAnchor))
                                {
                                    //Dalamud.Logging.PluginLog.Log("Tier 2 check");
                                    if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                        return GaussRound;
                                    else if (HasCharges(Ricochet) && level >= Levels.Ricochet)
                                        return Ricochet;
                                }
                                if (openerSelection is 2 && openerReady && JustUsed(HeatedSplitShot))
                                {
                                    if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                    {
                                        return GaussRound;
                                    }
                                    else if (HasCharges(Ricochet) && level >= Levels.Ricochet)
                                    {
                                        return Ricochet;
                                    }
                                }
                                else
                                {
                                    if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                    {
                                        return GaussRound;
                                    }
                                    else if (HasCharges(Ricochet) && level >= Levels.Ricochet)
                                    {
                                        return Ricochet;
                                    }
                                }
                            }

                            // TOOLS & REASSEMBLE
                            if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && level >= Levels.AirAnchor)
                            {
                                Dalamud.Logging.PluginLog.Log("Air Anchor Reassemble");
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                                    GetRemainingCharges(Reassemble) > 0)
                                {
                                    Dalamud.Logging.PluginLog.Log("Air Anchor Reassemble Tier 2");
                                    if (openerSelection is 0 or 1 & openerReady ||
                                        openerSelection is 2 & openerReady && WasLastWeaponskill(HeatedCleanShot) && InCombat() && CombatEngageDuration().TotalSeconds >= 1 &&
                                        Gauge.Heat > 10 ||
                                       !openerReady && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) &&
                                       GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        //idk why someone would have 2 charges of air anchor, but this could be a pseudo-recovery thing if someone was dead super long??
                                        return Reassemble; // General Purpose Opener & protection if players don't enable Max Charges Reassemble. kinda messy code

                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) && !openerReady &&
                                        CombatEngageDuration().Minutes % 2 == 1 && CombatEngageDuration().TotalSeconds >= 1)
                                        return Reassemble;

                                }
                                if (openerSelection is 0 or 1 && openerReady)
                                {
                                    return AirAnchor;
                                }
                                if (InCombat() && openerSelection is 2 && openerReady && WasLastWeaponskill(HeatedCleanShot) && HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) == 1)
                                {
                                    return AirAnchor;
                                }
                                else if (!openerReady && InCombat())
                                {
                                    return AirAnchor;
                                }
                            }
                            else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && level is >= Levels.Hotshot and < Levels.AirAnchor)
                                return HotShot;

                            if (InCombat() && (IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 1) && level >= Levels.Drill)
                            {
                                Dalamud.Logging.PluginLog.Log("Drill Reassemble");
                                if (openerReady &&
                                   (openerSelection is 0 or 1 && IsOnCooldown(AirAnchor) ||
                                   (openerSelection is 2 && WasLastWeaponskill(HeatedSplitShot))))
                                    return Drill;

                                if (!openerReady && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) &&
                                    !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                                {
                                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        return Reassemble;
                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges))
                                        return Reassemble;
                                }
                                return Drill;
                            }
                            // Chainsaw Reassemble
                            if ((ActionReady(ChainSaw) || GetCooldownRemainingTime(ChainSaw) <= 2) && ActionReady(ChainSaw))
                            {
                                Dalamud.Logging.PluginLog.Log("ChainSaw Reassemble");
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && !HasEffect(Buffs.Reassembled) &&
                                    GetRemainingCharges(Reassemble) > 0)
                                {
                                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble))
                                        return Reassemble;

                                    else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges))
                                        return Reassemble;
                                }
                                if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1))
                                    return ChainSaw;
                            }
                            // healing - please move if not appropriate priority
                            // Moved 
                            if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) && CanWeave(actionID, 0.6))
                            {
                                if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold) && ActionReady(All.SecondWind))
                                    return All.SecondWind;
                            }
                            if (lastComboMove == SplitShot && LevelChecked(SlugShot))
                                return OriginalHook(SlugShot);

                            if (lastComboMove == SlugShot && level >= Levels.CleanShot)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) &&
                                    !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                                {
                                    return Reassemble;
                                }
                                return OriginalHook(CleanShot);
                            }
                        } */
                    
                }
                return actionID;
            }
            /*
            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                uint wfTimer = 6; //default timer
                if (LocalPlayer.Level < Levels.BarrelStabilizer) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                // Did Aug write this? ^ Kinda impossible to have more than 70 heat with the code written currently. 65 heat during opener max
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                {
                    return true;
                }
                //Change ChainSaw code here for Delayed maybe later
                if (CombatEngageDuration().Minutes > 0 && (wildfireCDTime >= wfTimer || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && (IsOffCooldown(Wildfire) || wildfireCDTime < 1))))
                {
                    if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                    {
                        return true;
                    }

                    if (CombatEngageDuration().Minutes % 2 == 0)
                    {
                        return true;
                    }
                }

                return false;
            }*/
        }

    }
}