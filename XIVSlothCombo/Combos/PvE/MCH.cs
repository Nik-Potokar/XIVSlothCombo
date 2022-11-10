using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Combos.PvE.All;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class MCH
    {
        public const byte JobID = 31;

        public const uint
            RookAutoturret = 2864,
            SplitShot = 2866,
            SlugShot = 2868,
            SpreadShot = 2870,
            HotShot = 2872,
            CleanShot = 2873,
            GaussRound = 2874,
            Reassemble = 2876,
            Wildfire = 2878,
            Ricochet = 2890,
            HeatedSlugshot = 7412,
            HeatBlast = 7410,
            HeatedSplitShot = 7411,
            HeatedCleanShot = 7413,
            BarrelStabilizer = 7414,
            RookOverdrive = 7415,
            Peloton = 7557,
            AutoCrossbow = 16497,
            Drill = 16498,
            BioBlaster = 16499,
            AirAnchor = 16500,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
		      Detonator = 16766,
            Tactician = 16889,
            Hypercharge = 17209,
            Scattergun = 25786,
            ChainSaw = 25788;

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Peloton = 1199,
                Tactician = 1951,
                Wildfire = 1946;
        }

        public static class Debuffs
        {
            public const ushort
				   Wildfire = 861;
               // public const short placeholder = 0;
        }
        public static class Config
        {
            public const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",

				    MCH_ST_Simple_Battery_spender_percent = "Percent HP, use 'Automaton Queen'",
				    MCH_ST_Simple_QueenOverdrive_percent = "Percent HP, use 'Queen Overdrive'",

				    MCH_AoE_Simple_Battery_Threshold = "MCH_AoE_Simple_Battery_Threshold",
				    MCH_AoE_Simple_Bioblaster_threshold = "MCH_AoE_Simple_Bioblaster_threshold",
				    MCH_AoE_Simple_Hypercharge_threshold = "MCH_AoE_Simple_Hypercharge_threshold";
        }

        public static class Levels
        {
            public const byte
                SlugShot = 2,
                Hotshot = 4,
                GaussRound = 15,
                Peloton = 20,
                CleanShot = 26,
                Hypercharge = 30,
                HeatBlast = 35,
                RookOverdrive = 40,
                Wildfire = 45,
                Detonator = 45,
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

        #region MCH_ST_SimpleMode
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
						  var LastMinuteQueen = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_Simple_Battery_spender_percent);
						  var ByeQueen = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_Simple_QueenOverdrive_percent);
						  var enemyHP = GetTargetHPPercent();
						  var wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                    var raidbuffs = (HasEffect(RaidBuffs.Arcanecircle) && GetBuffRemainingTime(RaidBuffs.Arcanecircle) > 10)
                                 || (HasEffect(RaidBuffs.Battlelitany) && GetBuffRemainingTime(RaidBuffs.Battlelitany) > 10)
                                 || (HasEffect(RaidBuffs.Brotherhood) && GetBuffRemainingTime(RaidBuffs.Brotherhood) > 10)
                                 || (HasEffect(RaidBuffs.BattleVoice) && GetBuffRemainingTime(RaidBuffs.BattleVoice) > 10)
                                 || (HasEffect(RaidBuffs.RadiantFinale) && GetBuffRemainingTime(RaidBuffs.RadiantFinale) > 10)
                                 || (HasEffect(RaidBuffs.Embolden) && GetBuffRemainingTime(RaidBuffs.Embolden) > 10)
                                 || (HasEffect(RaidBuffs.SearingLight) && GetBuffRemainingTime(RaidBuffs.SearingLight) > 10)
                                 || (HasEffect(RaidBuffs.Divination) && GetBuffRemainingTime(RaidBuffs.Divination) > 10);
                    var potionBuff = HasEffect(All.Buffs.Medicated) && GetBuffRemainingTime(All.Buffs.Medicated) > 14;
                    var targethasBOTHraiddebuff = TargetHasEffect(RaidDebuffs.Mug) && TargetHasEffect(RaidDebuffs.ChainStratagem);
                    var targethasraiddebuff = TargetHasEffect(RaidDebuffs.Mug) || TargetHasEffect(RaidDebuffs.ChainStratagem);
                    var burstwindow = CombatEngageDuration().Minutes % 2 == 0 && CombatEngageDuration().Seconds > 6 && CombatEngageDuration().Seconds < 16;
                    var chargesGauss = GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet);
                    var chargesRico = GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound);
                    var MaxGaussCharge =  GetMaxCharges(GaussRound) >= 2;
                    var MaxRicoCharge = GetMaxCharges(Ricochet) >= 2;

                    if (!inCombat)
                    {
                        openerFinished = false;

                    }
                    #region Simple Stabilizer feature
                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && gauge.Heat <= 55 &&
                            IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && !WasLastWeaponskill(ChainSaw) &&
                            (wildfireCDTime <= 2 || (wildfireCDTime >= 80 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only)))) // && gauge.IsOverheated)) )
                        return BarrelStabilizer;
                    #endregion Simple Stabilizer feature

                    #region Simple interupt feature
                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }
                    #endregion Simple interupt feature

                    #region MCH_ST_Simple_WildCharge - Wildfire
                    if (GetTargetHPPercent() >= 1)
                    { 
                    if (openerFinished && (gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                        (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast)))) //these try to ensure the correct loops
                    {
                        if (CanDelayedWeave(actionID) && !gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                        {
                            return Wildfire;
                        } 
                        else if (CanDelayedWeave(actionID) && !gauge.IsOverheated && WasLastWeaponskill(ChainSaw))
                        {
                            return Wildfire;
                        } 
                        else if (CanWeave(actionID) && gauge.IsOverheated)
                        {
                            return Wildfire;
                        }
						      else if (CanWeave(actionID) && TargetHasEffect(Debuffs.Wildfire) && GetTargetHPPercent() <= 1)
						      {
							      return Detonator;
						      }
                     }
                    }
                    #endregion MCH_ST_Simple_WildCharge - Wildfire

                    #region MCH_ST_Simple_Gadget (sloth)
                    /*
                    if (CanWeave(actionID) && openerFinished && !gauge.IsRobotActive && IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) && (wildfireCDTime >= 2 && !WasLastAbility(Wildfire) || level < Levels.Wildfire))
                    {
                        //overflow protection
                        if (level >= Levels.RookOverdrive && gauge.Battery == 100 && CombatEngageDuration().Seconds < 55)
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        else if (level >= Levels.RookOverdrive && gauge.Battery >= 50 && (CombatEngageDuration().Seconds >= 59 || CombatEngageDuration().Seconds <= 05 || (CombatEngageDuration().Minutes == 0 && !WasLastWeaponskill(OriginalHook(CleanShot))) ))
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        //else if (gauge.Battery >= 50 && level >= Levels.RookOverdrive && (CombatEngageDuration().Seconds >= 58 || CombatEngageDuration().Seconds <= 05))
                        //{
                        //    return OriginalHook(RookAutoturret);
                        //}

                    }
                    */
                    #endregion MCH_ST_Simple_Gadget (sloth)

                    #region MCH_ST_Simple_Gadget
                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) && level >= Levels.RookOverdrive && CanWeave(actionID) && gauge.Battery >= 50 && (openerFinished = true && !gauge.IsRobotActive))
					     {
						   if ( CombatEngageDuration().Minutes == 0 && WasLastAction(ChainSaw))
						   {
							   return OriginalHook(RookAutoturret);
						   }
						   else if (gauge.Battery == 100 && (!WasLastAction(HeatedCleanShot) || !WasLastAction(CleanShot)) 
                           && (IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 3) && (IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 3) )
						   {
							   return OriginalHook(RookAutoturret);
						   }
						   else if (burstwindow && potionBuff && (targethasBOTHraiddebuff || (targethasraiddebuff && raidbuffs)) || (targethasraiddebuff || raidbuffs))
						   {
							   return OriginalHook(RookAutoturret);
						   }
						   else if (burstwindow)
						   {                                                                                                                                                                                                                                                                                                                                                                                               //Queen is used to allign with raidbuffs
							   return OriginalHook(RookAutoturret);
						   }
						   else if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Battery_spender_percent) && enemyHP <= LastMinuteQueen && IsOffCooldown(AutomatonQueen))
						   {
							   return OriginalHook(RookAutoturret);
						   }
                    }
                    #endregion MCH_ST_Simple_Gadget

                    #region Gauss/Rico behaivor outside hypercharge window
						  if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanDelayedWeave(actionID, 2.0f, 0.8f) && !gauge.IsOverheated)
						  {
						      if (HasCharges(GaussRound) && (level >= Levels.Ricochet && chargesGauss))
								   return GaussRound;
							   else if (HasCharges(Ricochet) && level >= Levels.Ricochet && chargesRico)
								   return Ricochet;
						  }
						  #endregion Gauss/Rico behaivor outside hypercharge window

                    #region MCH_ST_Simple_GaussRicochet & = IsOverheated
                    if (gauge.IsOverheated && level >= Levels.HeatBlast)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanDelayedWeave(actionID, 1.50, 0.5) && (wildfireCDTime > 2 || level < Levels.Wildfire) ) //gauss and ricochet weave
                        {
                            var gaussCharges = GetRemainingCharges(GaussRound);
                            var gaussMaxCharges = GetMaxCharges(GaussRound);

                            var overheatTime = gauge.OverheatTimeRemaining;
                            var reasmCharges = GetRemainingCharges(Reassemble);

                            //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                            if (overheatTime < 1.0 && !HasEffect(Buffs.Reassembled) && CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                (
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                ))
                                return Reassemble;
                            else if ( (!IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)) || chargesGauss || gaussMaxCharges) 
                                       ||
                                       (IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && gaussCharges >= gaussMaxCharges - 1 ) )
                            {
                                return GaussRound;
                            }
                            else if (level >= Levels.Ricochet && HasCharges(Ricochet) && !IsEnabled(CustomComboPreset.MCH_ST_Simple_High_Latency_Mode) && chargesRico)
                            {
                               return Ricochet;
                            }

                        }

                        return HeatBlast;
                    }
                    #endregion MCH_ST_Simple_GaussRicochet & = IsOverheated

                    #region MCH_ST_Simple_WildCharge => Hypercharge
                    if (CanWeave(actionID) && gauge.Heat >= 50 && openerFinished && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && level >= Levels.Hypercharge && !gauge.IsOverheated)
                    {
                        //Protection & ensures Hyper charged is double weaved with WF during reopener
                        if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                        if (level >= Levels.Drill && GetCooldownRemainingTime(Drill) >= 8)
                        {
                            if (level >= Levels.AirAnchor && GetCooldownRemainingTime(AirAnchor) >= 8)
                            {
                                if (level >= Levels.ChainSaw && GetCooldownRemainingTime(ChainSaw) >= 8)
                                {
                                    if (UseHypercharge(gauge, wildfireCDTime)) return Hypercharge;
                                }
                                else if (level < Levels.ChainSaw)
                                {
                                    if (UseHypercharge(gauge, wildfireCDTime)) return Hypercharge;
                                }
                            }
                            else if (level < Levels.AirAnchor)
                            {
                                if (UseHypercharge(gauge, wildfireCDTime)) return Hypercharge;
                            }
                        }
                        else if (level < Levels.Drill)
                        {
                            if (UseHypercharge(gauge, wildfireCDTime)) return Hypercharge; 
                        }
                    }
                    #endregion MCH_ST_Simple_WildCharge => Hypercharge

                    #region MCH_ST_SecondWind
                    // healing - please move if not appropriate priority
                    if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                    }
                    #endregion MCH_ST_SecondWind

                    #region MCH_ST_Simple_Assembling
                    #region MCH_ST_Simple_Assembling_AirAnchor
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
                    #endregion MCH_ST_Simple_Assembling_AirAnchor

                    #region MCH_ST_Simple_Assembling_Drill
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
                    #endregion MCH_ST_Simple_Assembling_Drill

                    #region MCH_ST_Simple_Assembling_ChainSaw
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
                    #endregion MCH_ST_Simple_Assembling_ChainSaw

                    #region MCH_ST_Simple_GaussRicochet
                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanDelayedWeave(actionID, 2.0f, 0.8f) && !gauge.IsOverheated)
                    {
                        if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet) || chargesGauss))
                            return GaussRound;
                        else if (HasCharges(Ricochet) && level >= Levels.Ricochet && chargesRico)
                            return Ricochet;
                    }
                    #endregion MCH_ST_Simple_GaussRicochet
                    #endregion MCH_ST_Simple_Assembling

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

            #region UseHypercharge
            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                var burstwindow = CombatEngageDuration().Minutes % 2 == 0 && CombatEngageDuration().Seconds > 6 && CombatEngageDuration().Seconds < 16;
                var raidbuffs = HasEffect(RaidBuffs.Arcanecircle) ||  HasEffect(RaidBuffs.Battlelitany) ||  HasEffect(RaidBuffs.Brotherhood) ||  HasEffect(RaidBuffs.BattleVoice) || HasEffect(RaidBuffs.RadiantFinale) || HasEffect(RaidBuffs.Embolden) || HasEffect(RaidBuffs.SearingLight) || HasEffect(RaidBuffs.Divination);
                var potionBuff = HasEffect(All.Buffs.Medicated) && GetBuffRemainingTime(All.Buffs.Medicated) > 14;
                var targethasraiddebuff = TargetHasEffect(RaidDebuffs.Mug) || TargetHasEffect(RaidDebuffs.ChainStratagem);
               
                uint wfTimer = 5; //default timer
                if (LocalPlayer.Level < Levels.BarrelStabilizer) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                {
                    return true;
                }

                if (CombatEngageDuration().Minutes > 0 && (wildfireCDTime >= wfTimer || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && (IsOffCooldown(Wildfire) || wildfireCDTime < 1))))
                {
                    if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 80)
                    {
                        return true;
                    }

                    if (gauge.Battery >= 50 && burstwindow && (targethasraiddebuff || raidbuffs) || potionBuff)
                    {
                        return true;
                    }
                }

                return false;
            }
            #endregion UseHypercharge
        }
        #endregion MCH_ST_SimpleMode

        #region MCH_AoE_SimpleMode
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
        #endregion MCH_AoE_SimpleMode

        #region MCH_HeatblastGaussRicochet
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
        #endregion MCH_HeatblastGaussRicochet

        #region MCH_GaussRoundRicochet
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
        #endregion MCH_GaussRoundRicochet

        #region MCH_Overdrive
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
        #endregion MCH_Overdrive

        #region MCH_AutoCrossbowGaussRicochet
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
        #endregion MCH_AutoCrossbowGaussRicochet

        #region MCH_ST_MainCombo
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
                    
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var chainsawCD = GetCooldown(ChainSaw);
                    
                    var battery = GetJobGauge<MCHGauge>().Battery;
                    var heat = GetJobGauge<MCHGauge>().Heat;
                    var canWeave = CanWeave(actionID);

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
                            return AirAnchor;
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
                            return AirAnchor;
                        if (level >= Levels.ChainSaw && !chainsawCD.IsCooldown && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)) )
                            return ChainSaw;
                        if (level >= Levels.Drill && !drillCD.IsCooldown && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return Drill;
                        if (level < Levels.AirAnchor && !hotshotCD.IsCooldown && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)) )
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
        #endregion MCH_ST_MainCombo

    }
}
