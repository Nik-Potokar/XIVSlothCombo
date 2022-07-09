using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class MCH
    {
        internal const byte JobID = 31;
        private static MCHGauge Gauge => CustomComboNS.CustomCombo.GetJobGauge<MCHGauge>();

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

        internal static class Debuffs
        {
            // public const short placeholder = 0;
        }

        internal class MCH_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is CleanShot or HeatedCleanShot or SplitShot or HeatedSplitShot)
                {
                    var battery = Gauge.Battery;
                    var heat = Gauge.Heat;
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.MCH_ST_BarrelStabilizer_DriftProtection) &&
                        ActionReady(BarrelStabilizer) && 
                        heat < 20 && canWeave) return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_HeatBlast) && Gauge.IsOverheated)
                    {
                        if (CanWeave(actionID, 0.6))
                        {
                            if (!LevelChecked(Ricochet) && HasCharges(GaussRound)) return GaussRound;
                            
                            if (HasCharges(GaussRound) && GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
                                return GaussRound;
                            else if (ActionReady(Ricochet))
                                return Ricochet;
                        }

                        if (LevelChecked(HeatBlast)) // prioritize heatblast
                            return HeatBlast;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_Cooldowns))
                    {
                        //T: Do Not OriginalHook
                        if (ActionReady(AirAnchor) && HasEffect(Buffs.Reassembled))
                            return OriginalHook(AirAnchor);
                        if (ActionReady(Drill) && IsOnCooldown(AirAnchor))
                            return Drill;
                        if (ActionReady(ChainSaw) && HasEffect(Buffs.Reassembled))
                            return ChainSaw;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGaussCharges) && CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (ActionReady(Ricochet)) 
                            return Ricochet;
                        if (ActionReady(GaussRound))
                            return GaussRound;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGauss) && CanWeave(actionID, 0.6)) //0.6 instead of 0.7 to more easily fit opener. a
                    {
                        if (LevelChecked(Ricochet) && GetRemainingCharges(Ricochet) > 1) 
                            return Ricochet;
                        if (LevelChecked(GaussRound) && GetRemainingCharges(GaussRound) > 1)
                            return GaussRound;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainComboAlternate))
                    {
                        
                        //ToDo: OriginalHook with AirAnchor/HotShot? 
                        if (ActionReady(AirAnchor) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return AirAnchor;
                        if (ActionReady(ChainSaw) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return ChainSaw;
                        if (ActionReady(Drill) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return Drill;
                        if (!LevelChecked(AirAnchor) && IsOffCooldown(HotShot) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return HotShot;
                    }

                    //Rook Autoturret / Queen
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_OverCharge) && canWeave && battery == 100)
                        return OriginalHook(RookAutoturret);
                    
                    if (comboTime > 0)
                    {
                        if (lastComboMove == SplitShot && LevelChecked(SlugShot))
                            return OriginalHook(SlugShot);

                        if (lastComboMove == SlugShot && LevelChecked(CleanShot))
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
                if (actionID is HeatBlast)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) &&
                        Gauge.Heat < 50 &&
                        !Gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Wildfire) &&
                        IsOffCooldown(Hypercharge) &&
                        ActionReady(Wildfire) &&
                        Gauge.Heat >= 50)
                        return Wildfire;

                    if (!Gauge.IsOverheated && LevelChecked(Hypercharge))
                        return Hypercharge;

                    if (GetCooldownRemainingTime(HeatBlast) < 0.7 && LevelChecked(HeatBlast)) // Prioritize Heat Blast
                        return HeatBlast;

                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
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
                if (actionID is GaussRound or Ricochet)
                {
                    // Prioritize the original if both are off cooldown
                    if (!LevelChecked(Ricochet))
                        return GaussRound;

                    if (IsOffCooldown(GaussRound) && IsOffCooldown(Ricochet))
                        return actionID;

                    if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
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
                if (actionID is SpreadShot or Scattergun)
                {
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_OverCharge) && canWeave && Gauge.Battery == 100)
                        return OriginalHook(RookAutoturret);
                    
                    if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && canWeave && 
                        (IsEnabled(CustomComboPreset.MCH_AoE_Gauss) || Gauge.IsOverheated) && (HasCharges(Ricochet) || HasCharges(GaussRound)))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        if ((gaussCharges >= ricochetCharges || !LevelChecked(Ricochet)) &&
                            LevelChecked(GaussRound))
                            return GaussRound;
                        else if (ricochetCharges > 0 && LevelChecked(Ricochet))
                            return Ricochet;

                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster) && ActionReady(BioBlaster) && !Gauge.IsOverheated)
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Hypercharge) && canWeave)
                    {
                        if (Gauge.Heat >= 50 && LevelChecked(AutoCrossbow) && !Gauge.IsOverheated)
                            return Hypercharge;
                    }

                    if (Gauge.IsOverheated && LevelChecked(AutoCrossbow))
                        return AutoCrossbow;

                }

                return actionID;
            }
        }

        internal class MCH_Overdrive : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is RookAutoturret or AutomatonQueen && Gauge.IsRobotActive ? OriginalHook(QueenOverdrive) : actionID;
        }

        internal class MCH_HotShotDrillChainSaw : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainSaw;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Drill or HotShot or AirAnchor)
                {
                    if (LevelChecked(ChainSaw))
                        return CalcBestAction(actionID, ChainSaw, AirAnchor, Drill);

                    if (LevelChecked(Drill)) 
                        return CalcBestAction(Drill, Drill, OriginalHook(HotShot)); //Prioritize Drill over Hotshot by making Drill original action

                    return HotShot;
                }

                return actionID;
            }
        }

        internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbowGaussRicochet;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is AutoCrossbow)
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel) &&
                        ActionReady(BarrelStabilizer) &&
                        Gauge.Heat < 50 &&
                        !Gauge.IsOverheated)
                        return BarrelStabilizer;

                    if (!Gauge.IsOverheated && LevelChecked(Hypercharge))
                        return Hypercharge;
                    if (GetCooldownRemainingTime(HeatBlast) < 0.7 && LevelChecked(AutoCrossbow)) // prioritize autocrossbow
                        return AutoCrossbow;
                    if (!LevelChecked(Ricochet))
                        return GaussRound;
                    if (GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet))
                        return GaussRound;
                    else
                        return Ricochet;
                }

                return actionID;
            }
        }

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SplitShot or HeatedSplitShot)
                {
                    var inCombat = InCombat();
                    
                    var wildfireCDTime = GetCooldownRemainingTime(Wildfire);

                    if (!inCombat)
                    {
                        openerFinished = false;

                    }

                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && Gauge.Heat <= 55 &&
                            ActionReady(BarrelStabilizer) && !WasLastWeaponskill(ChainSaw) &&
                            (wildfireCDTime <= 9 || (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only) && Gauge.IsOverheated)) )
                        return BarrelStabilizer;

                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    if (openerFinished && (Gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && LevelChecked(Wildfire) && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                        (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast))) ) //these try to ensure the correct loops
                    {
                        if (CanDelayedWeave(actionID) && !Gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                        {
                            return Wildfire;
                        } else if (CanDelayedWeave(actionID,1.1) && !Gauge.IsOverheated && WasLastWeaponskill(ChainSaw))
                        {
                            return Wildfire;
                        } else if (CanWeave(actionID, 0.6) && Gauge.IsOverheated )
                        {
                            return Wildfire;
                        }

                    }

                    if (CanWeave(actionID) && openerFinished && !Gauge.IsRobotActive && IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) && (wildfireCDTime >= 2 && !WasLastAbility(Wildfire) || !LevelChecked(Wildfire)))
                    {
                        //overflow protection
                        uint rookQueen = OriginalHook(RookAutoturret);
                        if (Gauge.Battery == 100 && LevelChecked(rookQueen))
                        {
                            return rookQueen;
                        }
                        else if (Gauge.Battery >= 50 && LevelChecked(rookQueen) && (CombatEngageDuration().Seconds >= 55 || CombatEngageDuration().Seconds <= 05 || (CombatEngageDuration().Minutes == 0 && !WasLastWeaponskill(OriginalHook(CleanShot))) ))
                        {
                            return rookQueen;
                        }
                        else if (Gauge.Battery >= 80 && LevelChecked(rookQueen) && (CombatEngageDuration().Seconds >= 50 || CombatEngageDuration().Seconds <= 05))
                        {
                            return rookQueen;
                        }

                    }

                    
                    if (Gauge.IsOverheated && LevelChecked(HeatBlast))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID, 0.6) && (wildfireCDTime > 2 || !LevelChecked(Wildfire)) ) //gauss and ricochet weave
                        {
                            var gaussCharges = GetRemainingCharges(GaussRound);
                            var gaussMaxCharges = GetMaxCharges(GaussRound);

                            var ricochetCharges = GetRemainingCharges(Ricochet);

                            var overheatTime = Gauge.OverheatTimeRemaining;
                            var reasmCharges = GetRemainingCharges(Reassemble);

                            //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                            if (overheatTime < 1.7 && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                (
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                ))
                                return Reassemble;
                            else if ( ((gaussCharges > ricochetCharges || gaussCharges == gaussMaxCharges || !LevelChecked(Ricochet)) && !IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode)) ||
                                       (gaussCharges >= gaussMaxCharges - 1 && IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode)) )
                            {
                                return GaussRound;
                            }
                            else if (LevelChecked(Ricochet) && ricochetCharges > 0 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode))
                            {
                               return Ricochet;
                            }
                                
                            
                        }

                        return HeatBlast;
                    }

                    if (CanWeave(actionID) && Gauge.Heat >= 50 && openerFinished && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && LevelChecked(Hypercharge) && !Gauge.IsOverheated)
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if (HasEffect(Buffs.Wildfire) || !LevelChecked(Wildfire)) return Hypercharge;

                        if (LevelChecked(Drill) && GetCooldownRemainingTime(Drill) >= 8)
                        {
                            if (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) >= 8)
                            {
                                if (LevelChecked(ChainSaw) && GetCooldownRemainingTime(ChainSaw) >= 8)
                                {
                                    if (UseHypercharge(Gauge, wildfireCDTime)) return Hypercharge;
                                }
                                else if (!LevelChecked(ChainSaw))
                                {
                                    if (UseHypercharge(Gauge, wildfireCDTime)) return Hypercharge;
                                }
                            }
                            else if (!LevelChecked(AirAnchor))
                            {
                                if (UseHypercharge(Gauge, wildfireCDTime)) return Hypercharge;
                            }
                        }
                        else if (!LevelChecked(Drill))
                        {
                            if (UseHypercharge(Gauge, wildfireCDTime)) return Hypercharge; 
                        }
                    }

                    if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && LevelChecked(AirAnchor))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                            GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges)) return Reassemble;

                        }
                        return AirAnchor;
                    }
                    else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && LevelChecked(HotShot) && !LevelChecked(AirAnchor))
                        return HotShot;

                    if ((IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 1) && LevelChecked(Drill))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) &&
                            !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges)) return Reassemble;
                        }
                        return Drill;
                    }

                    if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1) && LevelChecked(ChainSaw) && openerFinished)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && !HasEffect(Buffs.Reassembled) &&
                            GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges)) return Reassemble;
                        }
                        return ChainSaw;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var gaussMaxCharges = GetMaxCharges(GaussRound);

                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        if (gaussCharges > ricochetCharges || gaussCharges == gaussMaxCharges || !LevelChecked(Ricochet))
                            return GaussRound;
                        else if (ricochetCharges > 0 && LevelChecked(Ricochet))
                            return Ricochet;
                    }
                    

                    if (lastComboMove == SplitShot && LevelChecked(SlugShot))
                        return OriginalHook(SlugShot);

                    if (lastComboMove == SlugShot && LevelChecked(CleanShot))
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) &&
                            !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            return Reassemble;
                        }
                        return OriginalHook(CleanShot);
                    }

                    if (lastComboMove == CleanShot) openerFinished = true;
                }

                return actionID;
            }

            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                uint wfTimer = 6; //default timer
                if (!LevelChecked(BarrelStabilizer)) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                {
                    return true;
                }

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
            }
        }

    }
}
