using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.CustomCombo;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MCH
    {
        public const byte JobID = 31;

        public const uint
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

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
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

        internal class MCH_ST_MainCombo : CustomCombo
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
                    var reassembleCD = GetCooldown(Reassemble);
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var chainsawCD = GetCooldown(ChainSaw);
                    var barrelCD = GetCooldown(BarrelStabilizer);
                    var battery = GetJobGauge<MCHGauge>().Battery;
                    var heat = GetJobGauge<MCHGauge>().Heat;
                    if (IsEnabled(CustomComboPreset.MCH_ST_BarrelStabilizer_DriftProtection))
                    {
                        if (level >= Levels.BarrelStabilizer && heat < 20 && GetCooldown(actionID).CooldownRemaining > 0.7 && IsOffCooldown(BarrelStabilizer))
                            return BarrelStabilizer;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_HeatBlast) && gauge.IsOverheated)
                    {
                        if (heatBlastCD.CooldownRemaining < 0.7 && level >= Levels.HeatBlast) // prioritize heatblast
                            return HeatBlast;
                        if (level <= 49)
                            return GaussRound;
                        if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                            return GaussRound;
                        else
                            return Ricochet;
                    }
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_Cooldowns))
                    {
                        if (HasEffect(Buffs.Reassembled) && !airAnchorCD.IsCooldown && level >= Levels.AirAnchor)
                            return AirAnchor;
                        if (airAnchorCD.IsCooldown && !drillCD.IsCooldown && level >= Levels.Drill)
                            return Drill;
                        if (HasEffect(Buffs.Reassembled) && !chainsawCD.IsCooldown && level >= 90)
                            return ChainSaw;
                    }
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGaussCharges))
                    {
                        if (level >= Levels.Ricochet && HasCharges(Ricochet) && GetCooldown(CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                            return Ricochet;
                        if (level >= Levels.GaussRound && HasCharges(GaussRound) && GetCooldown(CleanShot).CooldownRemaining > 0.6)
                            return GaussRound;

                    }
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_RicochetGauss))
                    {
                        if (level >= Levels.Ricochet && ricochetCD.RemainingCharges > 1 && GetCooldown(CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                            return Ricochet;
                        if (level >= Levels.GaussRound && gaussCD.RemainingCharges > 1 && GetCooldown(CleanShot).CooldownRemaining > 0.6)
                            return GaussRound;

                    }
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainComboAlternate))
                    {
                        if (reassembleCD.CooldownRemaining >= 55 && !airAnchorCD.IsCooldown && level >= 76)
                            return AirAnchor;
                        if (reassembleCD.CooldownRemaining >= 55 && !drillCD.IsCooldown && level >= 58)
                            return Drill;
                        if (reassembleCD.CooldownRemaining >= 55 && !hotshotCD.IsCooldown && level <= 75)
                            return HotShot;
                        else
                        if (level >= 84)
                        {
                            if (HasEffect(Buffs.Reassembled) && reassembleCD.CooldownRemaining <= 55 && !airAnchorCD.IsCooldown)
                                return AirAnchor;
                            if (reassembleCD.CooldownRemaining >= 55 && !chainsawCD.IsCooldown && level >= 90)
                                return ChainSaw;
                            if (HasEffect(Buffs.Reassembled) && reassembleCD.CooldownRemaining <= 110 && !drillCD.IsCooldown)
                                return Drill;
                        }
                    }
                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_OverCharge))
                    {
                        if (battery == 100 && level is >= 40 and <= 79 && GetCooldown(CleanShot).CooldownRemaining > 0.7)
                            return RookAutoturret;
                        if (battery == 100 && level >= 80 && GetCooldown(CleanShot).CooldownRemaining > 0.7)
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
                    if (level <= 49)
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

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SplitShot or HeatedSplitShot)
                {
                    var inCombat = InCombat();
                    var gauge = GetJobGauge<MCHGauge>();
                    
                    var wildfireCDTime = GetCooldownRemainingTime(Wildfire);

                    if (!inCombat)
                    {
                        openerFinished = false;

                    }

                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && gauge.Heat <= 55 &&
                            IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer &&
                            (wildfireCDTime < 8 || (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only))) )
                        return BarrelStabilizer;

                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    if (openerFinished && gauge.Heat >= 50 && wildfireCDTime <= 2 && level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                        (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast)) || WasLastAbility(Hypercharge)) ) //these try to ensure the correct loops
                    {
                        if (CombatEngageDuration().Minutes == 0 && CanDelayedWeave(actionID)  ) //Ensures performs the correct opener loop
                        {
                            return Wildfire;
                        } else if (CombatEngageDuration().Minutes > 0 && CanWeave(actionID) )
                        {
                            return Wildfire;
                        }
                    }

                    if (CanWeave(actionID) && openerFinished && !gauge.IsRobotActive && IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) && 
                        !WasLastWeaponskill(OriginalHook(CleanShot)) && (wildfireCDTime >= 2 && !WasLastAbility(Wildfire) || level < Levels.Wildfire))
                    {
                        //overflow protection
                        if (gauge.Battery == 100 && level >= Levels.RookOverdrive)
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        else if (gauge.Battery >= 50 && level >= Levels.RookOverdrive && (CombatEngageDuration().Seconds >= 55 || CombatEngageDuration().Seconds <= 05 || CombatEngageDuration().Minutes == 0))
                        {
                            return OriginalHook(RookAutoturret);
                        } 

                    }

                    if (gauge.IsOverheated && level >= Levels.HeatBlast)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID, 0.6)) //gauss and ricochet weave
                        {
                            var gaussCharges = GetRemainingCharges(GaussRound);
                            var ricochetCharges = GetRemainingCharges(Ricochet);
                            var overheatTime = gauge.OverheatTimeRemaining;
                            var reasmCharges = GetRemainingCharges(Reassemble);

                            //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                            if (overheatTime < 1.7 && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                (
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                ))
                                return Reassemble;
                            else if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) && gaussCharges > 0)
                                return GaussRound;
                            else if (ricochetCharges > 0 && level >= Levels.Ricochet)
                                return Ricochet;
                            
                        }

                        return HeatBlast;
                    }

                    if (CanWeave(actionID) && gauge.Heat >= 50 && openerFinished && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge))
                    {
                        if (level >= Levels.Hypercharge && !gauge.IsOverheated && (comboTime >= 9 || WasLastAbility(Wildfire)))
                        {
                            //Protection & ensures Hyper charged is double weaved with WF during reopener
                            if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                            if (level >= Levels.Drill && GetCooldownRemainingTime(Drill) > 8 && wildfireCDTime > 8)
                            {
                                if (level >= Levels.AirAnchor && GetCooldownRemainingTime(AirAnchor) > 8 && wildfireCDTime > 8)
                                {
                                    if (level >= Levels.ChainSaw && GetCooldownRemainingTime(ChainSaw) > 8 && wildfireCDTime > 8)
                                    {
                                        // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                                        if (CombatEngageDuration().Minutes == 0 && gauge.Heat > 70 && !WasLastWeaponskill(OriginalHook(CleanShot)) ) 
                                        {
                                            return Hypercharge;
                                        } 
                                        
                                        if ( CombatEngageDuration().Minutes > 0 )
                                        {
                                            if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                                            {
                                                return Hypercharge;
                                            }
                                            
                                            if (CombatEngageDuration().Minutes % 2 == 0)
                                            {
                                                return Hypercharge;
                                            }
                                        }
                                    }
                                    else if (level < Levels.ChainSaw)
                                    {
                                        if (wildfireCDTime > 9) return Hypercharge;
                                    }
                                }
                                else if (level < Levels.AirAnchor)
                                {
                                    if (wildfireCDTime > 9) return Hypercharge;
                                }
                            }
                            else if (level < Levels.Drill)
                            {
                                if (wildfireCDTime > 9 || level < Levels.Wildfire) return Hypercharge;
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID)) //gauss and ricochet weave normally, because they are in normal gcds at this point
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        //this is to ensure that gauss/ricochet do not clip reassemble uses, due to the nature of how reassemble need to work we cant weave it when the windows allow it to,
                        //we need to force reassemble uses right before the skill we want to use it on.
                        var usingReasmSoon = IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && GetRemainingCharges(Reassemble) > 0 && openerFinished &&
                            (
                             (GetCooldownRemainingTime(Drill) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                             (GetCooldownRemainingTime(AirAnchor) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                             (GetCooldownRemainingTime(ChainSaw) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5))
                            );

                        //Prevents interruption of reopener weaves, retains original function
                        if (!WasLastWeaponskill(ChainSaw))
                        {
                            if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) && gaussCharges > 0 &&
                            level >= Levels.GaussRound && !usingReasmSoon)
                                return GaussRound;
                            else if (ricochetCharges > 0 && level >= Levels.Ricochet && !usingReasmSoon)
                                return Ricochet;
                        }
                    }

                    if (!gauge.IsOverheated)
                    {
                        if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && level >= Levels.AirAnchor)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                                GetRemainingCharges(Reassemble) > 0)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges)) return Reassemble;

                            }
                            return AirAnchor;
                        }
                        else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && level is >= Levels.Hotshot and < Levels.AirAnchor)
                            return HotShot;

                        if ((IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 1) && level >= Levels.Drill)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) &&
                                !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges)) return Reassemble;
                            }
                            return Drill;
                        }

                        if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1) && level >= Levels.ChainSaw && openerFinished)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && !HasEffect(Buffs.Reassembled) &&
                                GetRemainingCharges(Reassemble) > 0)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw_MaxCharges)) return Reassemble;
                            }
                            return ChainSaw;
                        }
                    }


                    if (lastComboMove == SplitShot && level >= Levels.SlugShot)
                        return OriginalHook(SlugShot);

                    if (lastComboMove == SlugShot && level >= Levels.CleanShot)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) &&
                            level < Levels.Drill && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            return Reassemble;
                        }
                        return OriginalHook(CleanShot);
                    }

                    if (lastComboMove == CleanShot) openerFinished = true;
                }

                return actionID;
            }
        }
    }
}
