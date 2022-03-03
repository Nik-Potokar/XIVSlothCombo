using Dalamud.Game.ClientState.JobGauge.Types;

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
            ChainSaw = 25788,
            BioBlaster = 16499,
            BarrelStabilizer = 7414,
            Wildfire = 2878,
            HeadGraze = 7551;

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
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
                HeadGraze = 24,
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
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                BioBlaster = 72,
                ChargedActionMastery = 74,
                QueenOverdrive = 80,
                BarrelStabilizer = 66,
                ChainSaw = 90;
        }
    }

    internal class MachinistMainCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistMainCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot || actionID == MCH.SplitShot || actionID == MCH.HeatedSplitShot)
            {
                var gauge = GetJobGauge<MCHGauge>();
                var drillCD = GetCooldown(MCH.Drill);
                var airAnchorCD = GetCooldown(MCH.AirAnchor);
                var hotshotCD = GetCooldown(MCH.HotShot);
                var reassembleCD = GetCooldown(MCH.Reassemble);
                var heatBlastCD = GetCooldown(MCH.HeatBlast);
                var gaussCD = GetCooldown(MCH.GaussRound);
                var ricochetCD = GetCooldown(MCH.Ricochet);
                var chainsawCD = GetCooldown(MCH.ChainSaw);
                var barrelCD = GetCooldown(MCH.BarrelStabilizer);
                var battery = GetJobGauge<MCHGauge>().Battery;
                var heat = GetJobGauge<MCHGauge>().Heat;
                if (IsEnabled(CustomComboPreset.BarrelStabilizerDrift))
                {
                    if (level >= MCH.Levels.BarrelStabilizer && heat < 20 && GetCooldown(actionID).CooldownRemaining > 0.7 && IsOffCooldown(MCH.BarrelStabilizer))
                        return MCH.BarrelStabilizer;
                }
                if (IsEnabled(CustomComboPreset.MachinistHeatBlastOnMainCombo) && gauge.IsOverheated)
                {
                    if (heatBlastCD.CooldownRemaining < 0.7) // prioritize heatblast
                        return MCH.HeatBlast;
                    if (level <= 49)
                        return MCH.GaussRound;
                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return MCH.GaussRound;
                    else
                        return MCH.Ricochet;
                }
                if (IsEnabled(CustomComboPreset.MachinistDrillAirOnMainCombo))
                {
                    if (HasEffect(MCH.Buffs.Reassembled) && !airAnchorCD.IsCooldown && level >= MCH.Levels.AirAnchor)
                        return MCH.AirAnchor;
                    if (airAnchorCD.IsCooldown && !drillCD.IsCooldown && level >= MCH.Levels.Drill)
                        return MCH.Drill;
                    if (HasEffect(MCH.Buffs.Reassembled) && !chainsawCD.IsCooldown && level >= 90)
                        return MCH.ChainSaw;
                }
                if (IsEnabled(CustomComboPreset.MachinistRicochetGaussChargesMainCombo))
                {
                    if (level >= MCH.Levels.Ricochet && HasCharges(MCH.Ricochet) && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                        return MCH.Ricochet;
                    if (level >= MCH.Levels.GaussRound && HasCharges(MCH.GaussRound) && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.6)
                        return MCH.GaussRound;

                }
                if (IsEnabled(CustomComboPreset.MachinistRicochetGaussMainCombo))
                {
                    if (level >= MCH.Levels.Ricochet && ricochetCD.CooldownRemaining <= 30 && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                        return MCH.Ricochet;
                    if (level >= MCH.Levels.GaussRound && gaussCD.CooldownRemaining <= 30 && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.6)
                        return MCH.GaussRound;

                }
                if (IsEnabled(CustomComboPreset.MachinistAlternateMainCombo))
                {
                    if (reassembleCD.CooldownRemaining >= 55 && !airAnchorCD.IsCooldown && level >= 76)
                        return MCH.AirAnchor;
                    if (reassembleCD.CooldownRemaining >= 55 && !drillCD.IsCooldown && level >= 58)
                        return MCH.Drill;
                    if (reassembleCD.CooldownRemaining >= 55 && !hotshotCD.IsCooldown && level <= 75)
                        return MCH.HotShot;
                    else
                    if (level >= 84)
                    {
                        if (HasEffect(MCH.Buffs.Reassembled) && reassembleCD.CooldownRemaining <= 55 && !airAnchorCD.IsCooldown)
                            return MCH.AirAnchor;
                        if (reassembleCD.CooldownRemaining >= 55 && !chainsawCD.IsCooldown && level >= 90)
                            return MCH.ChainSaw;
                        if (HasEffect(MCH.Buffs.Reassembled) && reassembleCD.CooldownRemaining <= 110 && !drillCD.IsCooldown)
                            return MCH.Drill;
                    }
                }
                if (IsEnabled(CustomComboPreset.MachinistOverChargeOption))
                {
                    if (battery == 100 && level >= 40 && level <= 79 && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.7)
                        return MCH.RookAutoturret;
                    if (battery == 100 && level >= 80 && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.7)
                        return MCH.AutomatonQueen;
                }
                if (comboTime > 0)
                {
                    if (lastComboMove == MCH.SplitShot && level >= MCH.Levels.SlugShot)
                        return OriginalHook(MCH.SlugShot);

                    if (lastComboMove == MCH.SlugShot && level >= MCH.Levels.CleanShot)
                        return OriginalHook(MCH.CleanShot);
                }
                return OriginalHook(MCH.SplitShot);
            }
            return actionID;

        }
    }
    internal class MachinistHeatblastGaussRicochetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHeatblastGaussRicochetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HeatBlast)
            {
                var wildfireCD = GetCooldown(MCH.Wildfire);
                var heatBlastCD = GetCooldown(MCH.HeatBlast);
                var gaussCD = GetCooldown(MCH.GaussRound);
                var ricochetCD = GetCooldown(MCH.Ricochet);

                var gauge = GetJobGauge<MCHGauge>();
                var heat = GetJobGauge<MCHGauge>().Heat;
                if (IsEnabled(CustomComboPreset.MachinistAutoBarrel) && heat < 50 && IsOffCooldown(MCH.BarrelStabilizer) && level >= 66)
                    return MCH.BarrelStabilizer;
                if (!wildfireCD.IsCooldown && level >= MCH.Levels.Wildfire && heat >= 50 && IsEnabled(CustomComboPreset.MachinistWildfireFeature))
                    return MCH.Wildfire;
                if (!gauge.IsOverheated && level >= MCH.Levels.Hypercharge)
                    return MCH.Hypercharge;
                if (heatBlastCD.CooldownRemaining < 0.7) // prioritize heatblast
                    return MCH.HeatBlast;
                if (level <= 49)
                    return MCH.GaussRound;
                if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                    return MCH.GaussRound;
                else
                    return MCH.Ricochet;
            }

            return actionID;
        }
    }

    internal class MachinistGaussRoundRicochetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistGaussRoundRicochetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.GaussRound || actionID == MCH.Ricochet)
            {
                var gaussCd = GetCooldown(MCH.GaussRound);
                var ricochetCd = GetCooldown(MCH.Ricochet);

                // Prioritize the original if both are off cooldown
                if (level <= 49)
                    return MCH.GaussRound;

                if (!gaussCd.IsCooldown && !ricochetCd.IsCooldown)
                    return actionID;

                if (gaussCd.CooldownRemaining < ricochetCd.CooldownRemaining)
                    return MCH.GaussRound;
                else
                    return MCH.Ricochet;
            }

            return actionID;
        }
    }

    internal class MachinistSpreadShotFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistSpreadShotFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.SpreadShot || actionID == MCH.Scattergun)
            {
                var battery = GetJobGauge<MCHGauge>().Battery;
                if (IsEnabled(CustomComboPreset.MachinistAoEOverChargeOption) && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.7)
                {
                    if (battery == 100 && level >= 40 && level <= 79)
                        return MCH.RookAutoturret;
                    if (battery == 100 && level >= 80)
                        return MCH.AutomatonQueen;
                }
                var gauge = GetJobGauge<MCHGauge>();


                if (IsEnabled(CustomComboPreset.MachinistAoEGaussRicochetFeature) && GetCooldown(MCH.CleanShot).CooldownRemaining > 0.7 && (IsEnabled(CustomComboPreset.MachinistAoEGaussOption) || gauge.IsOverheated))
                {
                    var gaussCd = GetCooldown(MCH.GaussRound);
                    var ricochetCd = GetCooldown(MCH.Ricochet);

                    // Prioritize the original if both are off cooldown
                    if (level <= 49)
                        return MCH.GaussRound;

                    if (gaussCd.CooldownRemaining < ricochetCd.CooldownRemaining)
                        return MCH.GaussRound;

                    return MCH.Ricochet;
                }

                var bioblaster = GetCooldown(MCH.BioBlaster);
                if (!bioblaster.IsCooldown && level >= 72 && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MachinistBioblasterFeature))
                    return MCH.BioBlaster;
                if (!gauge.IsOverheated && level >= 82)
                    return MCH.Scattergun;
                if (gauge.IsOverheated && level >= MCH.Levels.AutoCrossbow)
                    return MCH.AutoCrossbow;


            }

            return actionID;
        }
    }

    internal class MachinistOverdriveFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverdriveFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.RookAutoturret || actionID == MCH.AutomatonQueen)
            {
                var gauge = GetJobGauge<MCHGauge>();
                if (gauge.IsRobotActive)
                    return OriginalHook(MCH.QueenOverdrive);
            }

            return actionID;
        }

        internal class MachinistHotShotDrillChainsawFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHotShotDrillChainsawFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == MCH.Drill || actionID == MCH.HotShot || actionID == MCH.AirAnchor)
                {
                    if (level >= MCH.Levels.ChainSaw)
                        return CalcBestAction(actionID, MCH.ChainSaw, MCH.AirAnchor, MCH.Drill);

                    if (level >= MCH.Levels.AirAnchor)
                        return CalcBestAction(actionID, MCH.AirAnchor, MCH.Drill);

                    if (level >= MCH.Levels.Drill)
                        return CalcBestAction(actionID, MCH.Drill, MCH.HotShot);

                    return MCH.HotShot;
                }

                return actionID;
            }
        }
    }
    internal class MachinistAutoCrossBowGaussRicochetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistAutoCrossBowGaussRicochetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.AutoCrossbow)
            {
                var heatBlastCD = GetCooldown(MCH.HeatBlast);
                var gaussCD = GetCooldown(MCH.GaussRound);
                var ricochetCD = GetCooldown(MCH.Ricochet);
                var heat = GetJobGauge<MCHGauge>().Heat;

                var gauge = GetJobGauge<MCHGauge>();
                if (IsEnabled(CustomComboPreset.MachinistAutoBarrel) && heat < 50 && IsOffCooldown(MCH.BarrelStabilizer) && level >= 66)
                    return MCH.BarrelStabilizer;
                if (!gauge.IsOverheated && level >= MCH.Levels.Hypercharge)
                    return MCH.Hypercharge;
                if (heatBlastCD.CooldownRemaining < 0.7) // prioritize heatblast
                    return MCH.AutoCrossbow;
                if (level <= 49)
                    return MCH.GaussRound;
                if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                    return MCH.GaussRound;
                else
                    return MCH.Ricochet;
            }

            return actionID;
        }
    }

    internal class MachinistSimpleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistSimpleFeature;
        internal static bool openerFinished = false;
        
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if ( actionID is MCH.SplitShot or MCH.HeatedSplitShot )
            {
                var inCombat = InCombat();
                var gauge = GetJobGauge<MCHGauge>();

                if (!inCombat)
                {
                    openerFinished = false;
                    
                }

                if (CanWeave(actionID)) // normal weaves
                {
                    if (IsEnabled(CustomComboPreset.MachinistSimpleInterrupt) && CanInterruptEnemy() && IsOffCooldown(MCH.HeadGraze))
                    {
                        return MCH.HeadGraze;
                    }

                    if (gauge.Heat < 50 && IsOffCooldown(MCH.BarrelStabilizer) && level >= MCH.Levels.BarrelStabilizer &&
                        GetCooldown(MCH.Wildfire).CooldownRemaining < 12 )
                        return MCH.BarrelStabilizer;

                    if (openerFinished && !gauge.IsRobotActive)
                    {
                        //overflow protection
                        if (gauge.Battery == 100)
                        {
                            if (level >= MCH.Levels.QueenOverdrive)
                            {
                                return MCH.AutomatonQueen;
                            }

                            if (level >= MCH.Levels.RookOverdrive)
                            {
                                return MCH.RookAutoturret;
                            }
                        }
                        // even bursts ?
                        else if (gauge.Battery >= 80 && IsOnCooldown(MCH.Wildfire) && GetCooldown(MCH.AirAnchor).CooldownRemaining < 7 &&
                            GetCooldown(MCH.Drill).CooldownRemaining < 10 && GetCooldown(MCH.ChainSaw).CooldownRemaining < 17)
                        {
                            if (level >= MCH.Levels.QueenOverdrive)
                            {
                                return MCH.AutomatonQueen;
                            }

                            if (level >= MCH.Levels.RookOverdrive)
                            {
                                return MCH.RookAutoturret;
                            }
                        //odd bursts ?
                        } else if (gauge.Battery >= 50 && IsOnCooldown(MCH.Wildfire) &&
                            GetCooldown(MCH.ChainSaw).CooldownRemaining < 17)
                        {
                            if (level >= MCH.Levels.QueenOverdrive)
                            {
                                return MCH.AutomatonQueen;
                            }

                            if (level >= MCH.Levels.RookOverdrive)
                            {
                                return MCH.RookAutoturret;
                            }
                        //opener
                        } else if (gauge.LastSummonBatteryPower == 0 && gauge.Battery >= 50)
                        {
                            if (level >= MCH.Levels.QueenOverdrive)
                            {
                                return MCH.AutomatonQueen;
                            }

                            if (level >= MCH.Levels.RookOverdrive)
                            {
                                return MCH.RookAutoturret;
                            }
                        } 
                        
                    }

                    if (gauge.Heat >= 50 && openerFinished)
                    {
                        if (level >= MCH.Levels.Hypercharge && !gauge.IsOverheated )
                        {
                            //protection
                            if (HasEffect(MCH.Buffs.Wildfire) || gauge.Heat >= 90) return MCH.Hypercharge;

                            if (level >= MCH.Levels.Drill && GetCooldown(MCH.Drill).CooldownRemaining > 6)
                            {
                                if (level >= MCH.Levels.AirAnchor && GetCooldown(MCH.AirAnchor).CooldownRemaining > 8)
                                {
                                    if (level >= MCH.Levels.ChainSaw && GetCooldown(MCH.ChainSaw).CooldownRemaining > 8)
                                    {
                                        if (level >= MCH.Levels.Wildfire && IsOffCooldown(MCH.Wildfire))
                                            return MCH.Wildfire;

                                        if (GetCooldown(MCH.Wildfire).CooldownRemaining > 9) return MCH.Hypercharge;
                                    }
                                    else if (level < MCH.Levels.ChainSaw)
                                    {
                                        if (level >= MCH.Levels.Wildfire && IsOffCooldown(MCH.Wildfire))
                                            return MCH.Wildfire;

                                        if (GetCooldown(MCH.Wildfire).CooldownRemaining > 9) return MCH.Hypercharge;
                                    }
                                }
                                else if (level < MCH.Levels.AirAnchor)
                                {
                                    if (level >= MCH.Levels.Wildfire && IsOffCooldown(MCH.Wildfire))
                                        return MCH.Wildfire;

                                    if (GetCooldown(MCH.Wildfire).CooldownRemaining > 9) return MCH.Hypercharge;
                                }
                            }
                            else if (level < MCH.Levels.Drill)
                            {
                                if (level >= MCH.Levels.Wildfire && IsOffCooldown(MCH.Wildfire))
                                    return MCH.Wildfire;

                                if (GetCooldown(MCH.Wildfire).CooldownRemaining > 9)  return MCH.Hypercharge;
                            }
                        }       
                    }
                }

                if ( GetCooldown(actionID).CooldownRemaining > 0.6 ) //gauss and ricochet weave
                {
                    var gaussCharges = GetRemainingCharges(MCH.GaussRound);
                    var ricochetCharges = GetRemainingCharges(MCH.Ricochet);

                    var chargeLimit = openerFinished ? 0 : 1;
                    
                    if (gaussCharges >= ricochetCharges && gaussCharges > chargeLimit && level >= MCH.Levels.GaussRound &&
                        (GetCooldown(MCH.ChainSaw).CooldownRemaining > 1 || !openerFinished))
                        return MCH.GaussRound;
                    else if (ricochetCharges > 0 && level >= MCH.Levels.Ricochet && 
                        (GetCooldown(MCH.ChainSaw).CooldownRemaining > 1 || !openerFinished))
                        return MCH.Ricochet;
                }

                if (gauge.IsOverheated && level >= MCH.Levels.HeatBlast )
                {
                    return MCH.HeatBlast;
                }

                if (IsOffCooldown(MCH.AirAnchor) && level >= MCH.Levels.AirAnchor)
                {
                    if (!openerFinished && !HasEffect(MCH.Buffs.Reassembled) && GetRemainingCharges(MCH.Reassemble) > 0)
                    {
                        return MCH.Reassemble;
                    }
                    return MCH.AirAnchor;
                } else if (IsOffCooldown(MCH.HotShot) && level >= MCH.Levels.Hotshot && level < MCH.Levels.AirAnchor)
                    return MCH.HotShot;


                if (IsOffCooldown(MCH.Drill) && level >= MCH.Levels.Drill)
                {
                    if (level < MCH.Levels.AirAnchor && !HasEffect(MCH.Buffs.Reassembled) && GetRemainingCharges(MCH.Reassemble) > 0)
                    {
                        return MCH.Reassemble;
                    }
                    return MCH.Drill;
                }
                   
                if (IsOffCooldown(MCH.ChainSaw) && level >= MCH.Levels.ChainSaw && openerFinished)
                {
                    if (!HasEffect(MCH.Buffs.Reassembled) && GetRemainingCharges(MCH.Reassemble) > 0)
                    {
                        return MCH.Reassemble;
                    }
                    return MCH.ChainSaw;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == MCH.SplitShot && level >= MCH.Levels.SlugShot)
                        return OriginalHook(MCH.SlugShot);

                    if (lastComboMove == MCH.SlugShot && level >= MCH.Levels.CleanShot)
                    {
                        if (level < MCH.Levels.Drill && !HasEffect(MCH.Buffs.Reassembled) && GetRemainingCharges(MCH.Reassemble) > 0)
                        {
                            return MCH.Reassemble;
                        }
                        return OriginalHook(MCH.CleanShot);
                    }
                } 

                if (lastComboMove == MCH.CleanShot) openerFinished = true;
            }

            return actionID;
        }
    }
}
