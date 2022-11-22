using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
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

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946;
        }

        public static class Debuffs
        {
            public const ushort
                Wildfire = 861,
                BioBlaster = 1866;
        }
        public static class Config
        {
            public const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",
                MCH_VariantCure = "MCH_VariantCure",
                BalanceOpenerPotion = "BalanceOpenerPotion",

                MCH_ST_GadgetThreshold = "MCH_ST_GadgetThreshold",
                MCH_ST_WildFireThreshold = "MCH_ST_WildFireThreshold",
                MCH_ST_HyperChargeThreshold = "MCH_ST_HyperChargeThreshold",

                MCH_AoE_GadgetThreshold = "MCH_AoE_GadgetThreshold",
                MCH_AoE_BioBlasterThreshold = "MCH_AoE_BioBlasterThreshold",		
		        MCH_AoE_AirAnchorThreshold = "MCH_AoE_AirAnchorThreshold",
        		MCH_AoE_ChainsawThreshold = "MCH_AoE_ChainsawThreshold",
                MCH_AoE_HyperChargeThreshold = "MCH_AoE_HyperChargeThreshold";
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
                var HasAoETarget = HasBattleTarget();

                if ((actionID is SpreadShot or Scattergun) && HasAoETarget)
                {
                    var canWeave = CanWeave(actionID);
                    var gauge = GetJobGauge<MCHGauge>();
                    var battery = GetJobGauge<MCHGauge>().Battery;                        
                    var AoEenemyHP = GetTargetHPPercent();
                    var GadgetThreshold = AoEenemyHP > PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_GadgetThreshold);
                    var BioBlasterThreshold = AoEenemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_BioBlasterThreshold);
                    var HyperChargeThreshold = AoEenemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_HyperChargeThreshold);
                    var AirAnchorThreshold = AoEenemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_AirAnchorThreshold);
                    var ChainsawThreshold = AoEenemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_ChainsawThreshold);

                    if (IsEnabled(CustomComboPreset.MCH_AoE_OverCharge) && canWeave && GadgetThreshold)
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
                    
                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Stabilizer) && gauge.Heat <= 50 && IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && InCombat() )
                    { 
                        if (GetCooldownRemainingTime(BarrelStabilizer) < 10)
                            return BarrelStabilizer;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && canWeave && (IsEnabled(CustomComboPreset.MCH_AoE_Gauss) && (HasCharges(Ricochet) || HasCharges(GaussRound))))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);
                        if (!gauge.IsOverheated || gauge.IsOverheated)
                        {
                            if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) &&
                                level >= Levels.GaussRound)
                                return GaussRound;
                            else if (ricochetCharges >= gaussCharges && level >= Levels.Ricochet)
                                return Ricochet;
                        }
                    }

                    if ((IsOffCooldown(BioBlaster) || GetCooldownRemainingTime(BioBlaster) < 2) && BioBlasterThreshold && InActionRange(BioBlaster) && level >= Levels.BioBlaster && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster))
                        return BioBlaster;

                    if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 2) && AirAnchorThreshold && battery < 100 && level >= Levels.AirAnchor && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MCH_AoE_Simple_AirAnchor))
                    {
						     if (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) >= 1)
						     {
							     return Reassemble;
						     }
						     return AirAnchor;
					}
                    
                    if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 2) && ChainsawThreshold && battery < 100 && level >= Levels.ChainSaw && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Chainsaw))
                    {
						     if (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) >= 1)
						     {
							     return Reassemble;
						     }
						     return ChainSaw;
					}

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Hypercharge) && canWeave && HyperChargeThreshold)
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

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static bool openerFinished = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var HasEnemyTarget = HasBattleTarget();

                if ((actionID is SplitShot or HeatedSplitShot or SlugShot ) && HasEnemyTarget)
                {
                    var inCombat = InCombat();
                    var gauge = GetJobGauge<MCHGauge>();
                    var enemyHP = GetTargetHPPercent();
                    var GadgetThreshold = enemyHP > PluginConfiguration.GetCustomIntValue(Config.MCH_ST_GadgetThreshold);
                    var WildFireThreshold = enemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_ST_WildFireThreshold);
                    var HyperChargeThreshold = enemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_ST_HyperChargeThreshold);
                    var wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                    var HasTincture = HasEffect(All.Buffs.Medicated);
                    var HasMeleeRaidBuffs = HasEffect(All.MeleeRaidBuffs.BattleLitany) || HasEffect(All.MeleeRaidBuffs.ArcaneCircle) || HasEffect(All.MeleeRaidBuffs.Brotherhood) || TargetHasEffectAny(All.RaidDebuffs.VulnerabilityUp);
                    var HasRangeRaidBuffs = HasEffect(All.RangeRaidBuffs.TechnicalFinish) || HasEffect(All.RangeRaidBuffs.BattleVoice) || HasEffect(All.RangeRaidBuffs.RadiantFinale);
                    var HasCasterRaidBuffs = HasEffect(All.CasterRaidBuffs.Embolden) || HasEffect(All.CasterRaidBuffs.SearingLight);
                    var HasHealerRaidBuffs = HasEffect(All.HealerRaidBuffs.Divination)  || TargetHasEffectAny(All.RaidDebuffs.ChainStratagem);
                    // var BuffWindow = CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 15 && CombatEngageDuration().Minutes % 2 == 0 ;

                    if (!inCombat)
                    {
                        openerFinished = false;

                    }

                    // Barrel Stabiilizer
                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && gauge.Heat <= 45 &&
                            IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && !WasLastWeaponskill(ChainSaw) &&
                            (wildfireCDTime <= 3 || (wildfireCDTime >= 10 && gauge.IsOverheated))
                       )
                        return BarrelStabilizer;

                    // Headgraze
                    if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    // Wildfire
                    if (WildFireThreshold && openerFinished && (gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                        (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast))) ) //these try to ensure the correct loops
                    {
                        if (CanDelayedWeave(actionID) && !gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                        {
                            return Wildfire;
                        }
                        else if (CanDelayedWeave(actionID,1.1) && !gauge.IsOverheated && WasLastWeaponskill(ChainSaw) && (WasLastAbility(RookAutoturret) || WasLastAbility(AutomatonQueen)))
                        {
                            return Wildfire;
                        } 
                        else if (CanWeave(actionID, 0.6) && gauge.IsOverheated )
                        {
                            return Wildfire;
                        }

                    }

                    // Queen
                    if (GadgetThreshold && CanWeave(actionID) && openerFinished && !gauge.IsRobotActive && IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) && level >= Levels.RookOverdrive/* (wildfireCDTime >= 2 && !WasLastAbility(Wildfire) || level < Levels.Wildfire)*/)
                    {
                        //overflow protection
                        /*if (level >= Levels.RookOverdrive && gauge.Battery == 100 && CombatEngageDuration().Minutes > 0 && CombatEngageDuration().Seconds < 15 && CombatEngageDuration().Seconds > 10)
                        {
                            return OriginalHook(RookAutoturret);
                        }*/
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && WasLastWeaponskill(ChainSaw) && CombatEngageDuration().Minutes == 0 && CombatEngageDuration().Seconds <= 30)
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 80 && CombatEngageDuration().Minutes % 2 == 1 && CombatEngageDuration().Seconds >= 10 && CombatEngageDuration().Seconds <= 45 )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && CombatEngageDuration().Minutes % 2 == 0  && CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 30 && WasLastWeaponskill(OriginalHook(CleanShot)) )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && CombatEngageDuration().Minutes % 4 == 0  && CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 30 && WasLastWeaponskill(OriginalHook(CleanShot)) )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && CombatEngageDuration().Minutes % 6 == 0  && CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 30 && WasLastWeaponskill(OriginalHook(CleanShot)) )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && CombatEngageDuration().Minutes % 8 == 0  && CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 30 && WasLastWeaponskill(OriginalHook(CleanShot)) )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                        if (/*level >= Levels.RookOverdrive &&*/ gauge.Battery >= 50 && CombatEngageDuration().Minutes % 10 == 0  && CombatEngageDuration().Seconds >= 7 && CombatEngageDuration().Seconds <= 30 && WasLastWeaponskill(OriginalHook(CleanShot)) )
                        {
                            return OriginalHook(RookAutoturret);
                        }
                    }


                    if (gauge.IsOverheated && level >= Levels.HeatBlast)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID, 0.65) && (wildfireCDTime > 2 || level < Levels.Wildfire) ) //gauss and ricochet weave
                        {
                          //var gaussCharges = GetRemainingCharges(GaussRound);
                          //var gaussMaxCharges = GetMaxCharges(GaussRound);

                            var overheatTime = gauge.OverheatTimeRemaining;
                            var reasmCharges = GetRemainingCharges(Reassemble);

                            //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                            if (overheatTime < 1.7 && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                (
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                    ||
                                    (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && HasTincture && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                ))
                                return Reassemble;
							else if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && level > Levels.Ricochet && level > Levels.GaussRound && CanWeave(actionID))
							{
								if (GetRemainingCharges(GaussRound) == 3)
									return GaussRound;
								else if (GetRemainingCharges(Ricochet) == 3)
									return Ricochet;
								else if (GetRemainingCharges(GaussRound) == 2)
									return GaussRound;
								else if (GetRemainingCharges(Ricochet) == 2)
									return Ricochet;
								else if (GetRemainingCharges(GaussRound) == 1)
									return GaussRound;
								else if (GetRemainingCharges(Ricochet) == 1)
									return Ricochet;
							}

                        }

                        return HeatBlast;
                    }

                    if (HyperChargeThreshold && CanWeave(actionID) && gauge.Heat >= 50 /* && openerFinished */ && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && level >= Levels.Hypercharge && !gauge.IsOverheated)
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

                    // healing - please move if not appropriate priority
                    if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                            return All.SecondWind;
                    }

                    #region original air Anchor
                 /* 
                  if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && level >= Levels.AirAnchor)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                            GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && CanWeave(actionID) ) 
                                return Reassemble;
                        }
                        return AirAnchor;
                    }
                    else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && level is >= Levels.Hotshot and < Levels.AirAnchor)
                        return HotShot; 
                 */
                 #endregion                    
                    // Air Anchor && GetCooldownRemainingTime(actionID) > 1.5 
                    if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 2.5 && CanWeave(actionID, 0.7)) && level >= Levels.AirAnchor)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && CanWeave(actionID) && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            return Reassemble;
                        }
                        return AirAnchor;
                    }          

                    // Drill
                    if ((IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 2.5 && CanWeave(actionID, 0.7)) && level >= Levels.Drill)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0 && CombatEngageDuration().Minutes >= 1 && CanWeave(actionID) && HasTincture) 
                        {
                            return Reassemble;
                        }
                        return Drill;
                    }

                    //Chainsaw
                    if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 2.5 && CanWeave(actionID, 0.7)) && level >= Levels.ChainSaw && openerFinished)
                    {
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && CanWeave(actionID) && !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            return Reassemble;
                        }
                        return ChainSaw;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && level > Levels.Ricochet && level > Levels.GaussRound && CanWeave(actionID))
                    {
                        if (GetRemainingCharges(GaussRound) == 3)
							return GaussRound;
                        else if (GetRemainingCharges(Ricochet) == 3)
                            return Ricochet;
                        else if (GetRemainingCharges(GaussRound) == 2)
                            return GaussRound;
                        else if (GetRemainingCharges(Ricochet) == 2)
                            return Ricochet;
                        else if (GetRemainingCharges(GaussRound) == 1)
                            return GaussRound;
                        else if (GetRemainingCharges(Ricochet) == 1)
                            return Ricochet;
                    }

                    // Tactician
                    if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Tactician))
                    { 
                        if (CombatEngageDuration().Minutes == 0 && CombatEngageDuration().Seconds >= 0 && CombatEngageDuration().Seconds <= 9 )
                        {
                            if ((!HasEffectAny(BRD.Buffs.Troubadour) || !HasEffectAny(MCH.Buffs.Tactician) || !HasEffectAny(DNC.Buffs.ShieldSamba)))
                            { 
                                if ((IsOffCooldown(Tactician) || GetCooldownRemainingTime(Tactician) <= 5))
                                {
                                    return Tactician;
                                }
                            }
                        }    
                    }


                  /*if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID))
                    {
                        if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                            return GaussRound;
                        else if (HasCharges(Ricochet) && level >= Levels.Ricochet)
                            return Ricochet;
                    }*/
                    

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

            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                //var enemyHP = GetTargetHPPercent();
                //var HyperChargeThreshold = enemyHP >  PluginConfiguration.GetCustomIntValue(Config.MCH_ST_HyperChargeThreshold);
                uint wfTimer = 6; //default timer
                if (LocalPlayer.Level < Levels.BarrelStabilizer) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                if (/*HyperChargeThreshold &&*/ CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                {
                    return true;
                }
                if (/*HyperChargeThreshold &&*/ CombatEngageDuration().Minutes == 1 && gauge.Heat >= 50 && CombatEngageDuration().Seconds >= 30 && CombatEngageDuration().Seconds <= 33)
                {
                    return true;
                }

                if (/*HyperChargeThreshold &&*/ CombatEngageDuration().Minutes > 0 && (wildfireCDTime >= wfTimer || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && (IsOffCooldown(Wildfire) || wildfireCDTime < 1))))
                {
                    if (/*HyperChargeThreshold &&*/ CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 70)
                    {
                        return true;
                    }

                    if (/*HyperChargeThreshold &&*/ CombatEngageDuration().Minutes % 2 == 0 && CombatEngageDuration().Seconds >= 10 )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

    }
}
