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


        internal class MachinistMainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistMainCombo;

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
                    if (IsEnabled(CustomComboPreset.BarrelStabilizerDrift))
                    {
                        if (level >= Levels.BarrelStabilizer && heat < 20 && GetCooldown(actionID).CooldownRemaining > 0.7 && IsOffCooldown(BarrelStabilizer))
                            return BarrelStabilizer;
                    }

                    if (IsEnabled(CustomComboPreset.MachinistHeatBlastOnMainCombo) && gauge.IsOverheated)
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
                    if (IsEnabled(CustomComboPreset.MachinistDrillAirOnMainCombo))
                    {
                        if (HasEffect(Buffs.Reassembled) && !airAnchorCD.IsCooldown && level >= Levels.AirAnchor)
                            return AirAnchor;
                        if (airAnchorCD.IsCooldown && !drillCD.IsCooldown && level >= Levels.Drill)
                            return Drill;
                        if (HasEffect(Buffs.Reassembled) && !chainsawCD.IsCooldown && level >= 90)
                            return ChainSaw;
                    }
                    if (IsEnabled(CustomComboPreset.MachinistRicochetGaussChargesMainCombo))
                    {
                        if (level >= Levels.Ricochet && HasCharges(Ricochet) && GetCooldown(CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                            return Ricochet;
                        if (level >= Levels.GaussRound && HasCharges(GaussRound) && GetCooldown(CleanShot).CooldownRemaining > 0.6)
                            return GaussRound;

                    }
                    if (IsEnabled(CustomComboPreset.MachinistRicochetGaussMainCombo))
                    {
                        if (level >= Levels.Ricochet && ricochetCD.RemainingCharges > 1 && GetCooldown(CleanShot).CooldownRemaining > 0.6) //0.6 instead of 0.7 to more easily fit opener. a
                            return Ricochet;
                        if (level >= Levels.GaussRound && gaussCD.RemainingCharges > 1 && GetCooldown(CleanShot).CooldownRemaining > 0.6)
                            return GaussRound;

                    }
                    if (IsEnabled(CustomComboPreset.MachinistAlternateMainCombo))
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
                    if (IsEnabled(CustomComboPreset.MachinistOverChargeOption))
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
        internal class MachinistHeatblastGaussRicochetFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHeatblastGaussRicochetFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HeatBlast)
                {
                    var wildfireCD = GetCooldown(Wildfire);
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);

                    var gauge = GetJobGauge<MCHGauge>();
                    var heat = GetJobGauge<MCHGauge>().Heat;
                    if (IsEnabled(CustomComboPreset.MachinistAutoBarrel) && heat < 50 && IsOffCooldown(BarrelStabilizer) && level >= 66)
                        return BarrelStabilizer;

                    if (!wildfireCD.IsCooldown && level >= Levels.Wildfire && heat >= 50 && IsEnabled(CustomComboPreset.MachinistWildfireFeature))
                        return Wildfire;

                    if (!gauge.IsOverheated && level >= Levels.Hypercharge)
                        return Hypercharge;

                    if (heatBlastCD.CooldownRemaining < 0.7 && level >= Levels.HeatBlast) // Prioritize Heat Blast
                        return HeatBlast;

                    if (level <= 49)
                        return GaussRound;

                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    return Ricochet;
                }

                return actionID;
            }
        }

        internal class MachinistGaussRoundRicochetFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistGaussRoundRicochetFeature;

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

        internal class MachinistSpreadShotFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistSpreadShotFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SpreadShot || actionID == Scattergun)
                {
                    var canWeave = CanWeave(actionID);

                    var battery = GetJobGauge<MCHGauge>().Battery;
                    if (IsEnabled(CustomComboPreset.MachinistAoEOverChargeOption) && canWeave)
                    {
                        if (battery == 100 && level >= Levels.QueenOverdrive)
                            return AutomatonQueen;
                        if (battery == 100 && level >= Levels.RookOverdrive)
                            return RookAutoturret;
                    }
                    var gauge = GetJobGauge<MCHGauge>();

                    if (IsEnabled(CustomComboPreset.MachinistAoEHyperchargeFeature) && canWeave)
                    {
                        if (gauge.Heat >= 50 && level >= Levels.Hypercharge && !gauge.IsOverheated)
                            return Hypercharge;
                    }

                    if (IsEnabled(CustomComboPreset.MachinistAoEGaussRicochetFeature) && canWeave && (IsEnabled(CustomComboPreset.MachinistAoEGaussOption) || gauge.IsOverheated) && (HasCharges(Ricochet) || HasCharges(GaussRound)))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) &&
                            level >= Levels.GaussRound)
                            return GaussRound;
                        else if (ricochetCharges > 0 && level >= Levels.Ricochet)
                            return Ricochet;

                    }

                    if (gauge.IsOverheated && level >= Levels.AutoCrossbow)
                        return AutoCrossbow;

                    var bioblaster = GetCooldown(BioBlaster);
                    if (!bioblaster.IsCooldown && level >= Levels.BioBlaster && !gauge.IsOverheated && IsEnabled(CustomComboPreset.MachinistBioblasterFeature))
                        return BioBlaster;



                }

                return actionID;
            }
        }

        internal class MachinistOverdriveFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverdriveFeature;

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

            internal class MachinistHotShotDrillChainsawFeature : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHotShotDrillChainsawFeature;

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
        internal class MachinistAutoCrossBowGaussRicochetFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistAutoCrossBowGaussRicochetFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AutoCrossbow)
                {
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var heat = GetJobGauge<MCHGauge>().Heat;

                    var gauge = GetJobGauge<MCHGauge>();
                    if (IsEnabled(CustomComboPreset.MachinistAutoBarrel) && heat < 50 && IsOffCooldown(BarrelStabilizer) && level >= 66)
                        return BarrelStabilizer;
                    if (!gauge.IsOverheated && level >= Levels.Hypercharge)
                        return Hypercharge;
                    if (heatBlastCD.CooldownRemaining < 0.7 && level >= Levels.AutoCrossbow) // prioritize autocrossbow
                        return AutoCrossbow;
                    if (level <= 49)
                        return GaussRound;
                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                        return GaussRound;
                    else
                        return Ricochet;
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
                if (actionID is SplitShot or HeatedSplitShot)
                {
                    var inCombat = InCombat();
                    var gauge = GetJobGauge<MCHGauge>();
                    var canWeaveNormal = CanWeave(actionID);

                    if (!inCombat)
                    {
                        openerFinished = false;

                    }

                    if (canWeaveNormal && IsEnabled(CustomComboPreset.MachinistSimpleStabilizer) && gauge.Heat <= 55 &&
                            IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer &&
                            GetCooldown(Wildfire).CooldownRemaining < 8)
                        return BarrelStabilizer;

                    if (canWeaveNormal && IsEnabled(CustomComboPreset.MachinistSimpleInterrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    if (canWeaveNormal && openerFinished && !gauge.IsRobotActive && IsEnabled(CustomComboPreset.MachinistSimpleGadget))
                    {
                        //overflow protection
                        if (gauge.Battery == 100)
                        {
                            if (level >= Levels.QueenOverdrive)
                            {
                                return AutomatonQueen;
                            }

                            if (level >= Levels.RookOverdrive)
                            {
                                return RookAutoturret;
                            }
                        }
                        else if (gauge.Battery >= 50 && (CombatEngageDuration().Seconds >= 55 || CombatEngageDuration().Seconds <= 05))
                        {
                            if (level >= Levels.QueenOverdrive)
                            {
                                return AutomatonQueen;
                            }

                            if (level >= Levels.RookOverdrive)
                            {
                                return RookAutoturret;
                            }
                        }
                        else if (gauge.LastSummonBatteryPower == 0 && gauge.Battery >= 50)
                        {
                            if (level >= Levels.QueenOverdrive)
                            {
                                return AutomatonQueen;
                            }

                            if (level >= Levels.RookOverdrive)
                            {
                                return RookAutoturret;
                            }
                        }

                    }

                    if (canWeaveNormal && gauge.Heat >= 50 && openerFinished && IsOffCooldown(Wildfire) && level >= Levels.Wildfire &&
                            IsEnabled(CustomComboPreset.MachinistSimpleWildCharge))
                    {
                        if (CombatEngageDuration().Minutes == 0 && GetRemainingCharges(Reassemble) == 0 && CanDelayedWeave(actionID)) return Wildfire;
                        else if (CombatEngageDuration().Minutes > 0 && (GetCooldownRemainingTime(ChainSaw) > 50 || level < Levels.ChainSaw)) return Wildfire;
                    }

                    if (gauge.IsOverheated && level >= Levels.HeatBlast)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistSimpleGaussRicochet) && CanWeave(actionID, 0.6)) //gauss and ricochet weave
                        {
                            var gaussCharges = GetRemainingCharges(GaussRound);
                            var ricochetCharges = GetRemainingCharges(Ricochet);
                            var usingReasmSoon = IsEnabled(CustomComboPreset.MachinistSimpleAssembling) && GetRemainingCharges(Reassemble) > 0 && openerFinished &&
                                (
                                 (GetCooldownRemainingTime(Drill) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                                 (GetCooldownRemainingTime(AirAnchor) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                                 (GetCooldownRemainingTime(ChainSaw) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5))
                                );

                            //var chargeLimit = openerFinished || level < MCH.Levels.Ricochet ? 0 : 1;

                            if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) && gaussCharges > 0 &&
                                !usingReasmSoon)
                                return GaussRound;
                            else if (ricochetCharges > 0 && level >= Levels.Ricochet && !usingReasmSoon)
                                return Ricochet;
                        }

                        return HeatBlast;
                    }

                    if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && level >= Levels.AirAnchor)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistSimpleAssembling) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MachinistSimpleAssemblingAirAnchor) &&
                            GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MachinistSimpleAssemblingAirAnchorMaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MachinistSimpleAssemblingAirAnchorMaxCharges)) return Reassemble;

                        }
                        return AirAnchor;
                    }
                    else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && level is >= Levels.Hotshot and < Levels.AirAnchor)
                        return HotShot;

                    if ((IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 1) && level >= Levels.Drill)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistSimpleAssembling) && IsEnabled(CustomComboPreset.MachinistSimpleAssemblingDrill) &&
                            !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MachinistSimpleAssemblingDrillMaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MachinistSimpleAssemblingDrillMaxCharges)) return Reassemble;
                        }
                        return Drill;
                    }

                    if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1) && level >= Levels.ChainSaw && openerFinished)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistSimpleAssembling) && IsEnabled(CustomComboPreset.MachinistSimpleAssemblingChainSaw) && !HasEffect(Buffs.Reassembled) &&
                            GetRemainingCharges(Reassemble) > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MachinistSimpleAssemblingChainSawMaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                            else if (!IsEnabled(CustomComboPreset.MachinistSimpleAssemblingChainSawMaxCharges)) return Reassemble;
                        }
                        return ChainSaw;
                    }

                    if (canWeaveNormal && gauge.Heat >= 50 && openerFinished && IsEnabled(CustomComboPreset.MachinistSimpleWildCharge))
                    {
                        if (level >= Levels.Hypercharge && !gauge.IsOverheated)
                        {
                            //protection
                            if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                            if (level >= Levels.Drill && GetCooldown(Drill).CooldownRemaining > 8)
                            {
                                if (level >= Levels.AirAnchor && GetCooldown(AirAnchor).CooldownRemaining > 8)
                                {
                                    if (level >= Levels.ChainSaw && (GetCooldown(ChainSaw).CooldownRemaining > 8 || CombatEngageDuration().Minutes % 2 == 0))
                                    {
                                        if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                                        {
                                            return Hypercharge;
                                        }
                                        else if (CombatEngageDuration().Minutes % 2 == 0)
                                        {
                                            if (CombatEngageDuration().Minutes != 0)
                                            {
                                                return Hypercharge;
                                            }
                                        }
                                    }
                                    else if (level < Levels.ChainSaw)
                                    {
                                        if (GetCooldown(Wildfire).CooldownRemaining > 8) return Hypercharge;
                                    }
                                }
                                else if (level < Levels.AirAnchor)
                                {
                                    if (GetCooldown(Wildfire).CooldownRemaining > 8) return Hypercharge;
                                }
                            }
                            else if (level < Levels.Drill)
                            {
                                if (GetCooldown(Wildfire).CooldownRemaining > 8 || level < Levels.Wildfire) return Hypercharge;
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.MachinistSimpleGaussRicochet) && CanWeave(actionID, 0.6)) //gauss and ricochet weave
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);
                        var usingReasmSoon = IsEnabled(CustomComboPreset.MachinistSimpleAssembling) && GetRemainingCharges(Reassemble) > 0 && openerFinished &&
                            (
                             (GetCooldownRemainingTime(Drill) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                             (GetCooldownRemainingTime(AirAnchor) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5)) ||
                             (GetCooldownRemainingTime(ChainSaw) < 2.5 && (!gauge.IsOverheated || gauge.OverheatTimeRemaining < 2.5))
                            );

                        //var chargeLimit = openerFinished || level < MCH.Levels.Ricochet ? 0 : 1;

                        if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) && gaussCharges > 0 &&
                            level >= Levels.GaussRound && !usingReasmSoon)
                            return GaussRound;
                        else if (ricochetCharges > 0 && level >= Levels.Ricochet && !usingReasmSoon)
                            return Ricochet;
                    }


                    if (lastComboMove == SplitShot && level >= Levels.SlugShot)
                        return OriginalHook(SlugShot);

                    if (lastComboMove == SlugShot && level >= Levels.CleanShot)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistSimpleAssembling) &&
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
