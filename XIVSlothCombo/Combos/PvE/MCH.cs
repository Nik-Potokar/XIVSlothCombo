using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
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
            Wildfire = 2878,
            Dismantle = 2887;

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
        public static class Config
        {
            public const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",
                MCH_VariantCure = "MCH_VariantCure";
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
                ChainSaw = 90,
                Dismantle = 62;
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
                if ((actionID == GaussRound || actionID == Ricochet) && level >= 50)
                {
                    var gaussCharges = GetRemainingCharges(GaussRound);
                    var ricochetCharges = GetRemainingCharges(Ricochet);

                    // Prioritize the original if both are off cooldown

                    if (IsOffCooldown(GaussRound) && IsOffCooldown(Ricochet))
                    return actionID;

                    if ((gaussCharges >= ricochetCharges || level < Levels.Ricochet) &&
                        level >= Levels.GaussRound)
                        return GaussRound;
                    else if (ricochetCharges > 0 && level >= Levels.Ricochet)
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
        internal class MCH_DismantleTactician : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_DismantleTactician;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Dismantle
                    && (IsOnCooldown(Dismantle) || level < Levels.Dismantle)
                    && ActionReady(Tactician)
                    && !HasEffect(Buffs.Tactician))
                    return Tactician;

                return actionID;
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

            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;
            internal static byte robotstep = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)

            {

                if (IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) && level >= 90 && actionID is SplitShot or HeatedSplitShot)
                {
                    bool inCombat = HasCondition(ConditionFlag.InCombat);
                    var gauge = GetJobGauge<MCHGauge>();

                    if (//(readyOpener || openerStarted) && 
                        inOpener == false && !inCombat && gauge.Heat < 5) { openerStarted = true; return HeatedSplitShot; }
                    if (openerStarted == true && (WasLastWeaponskill(HeatedSplitShot) || lastComboMove is HeatedSplitShot || WasLastAction(HeatedSplitShot)) && 
                        gauge.Heat == 5 && inCombat && GetRemainingCharges(GaussRound) == 3) { openerStarted = false; inOpener = true; return GaussRound; }
                    
                    //Reset check for opener
                    if (//gauge.Heat == 0 && gauge.Battery == 0
                         IsOffCooldown(ChainSaw) && IsOffCooldown(Drill) && IsOffCooldown(AirAnchor)
                        //&& GetRemainingCharges(GaussRound) == 3 && GetRemainingCharges(Ricochet) == 3 && GetRemainingCharges(Reassemble) == 2
                        //&& IsOffCooldown(Wildfire) && IsOffCooldown(BarrelStabilizer)
                         && GetTargetHPPercent() == 100 && !inCombat) 
                        //&& !inOpener && !openerStarted)
                    {
                        openerStarted = true;
                        inOpener = false;
                        step = 0;
                        //robotstep = 0;
                        return HeatedSplitShot;
                    }

                    if (inOpener == true)
                    {
                        
                        //heatedsplitshot
                        //gaussround
                        //ricochet
                        //drill
                        //barrelstabilizer
                        //heatedslugshot
                        //ricochet
                        //heatedcleanshot
                        //reassemble
                        //gaussround
                        //airanchor
                        //reassemble
                        //wildfire
                        //chainsaw
                        //automatonqueen
                        //hypercharge
                        //heatblast
                        //ricochet
                        //heatblast
                        //gaussround
                        //heatblast
                        //ricochet
                        //heatblast
                        //gaussround
                        //heatblast
                        //ricochet
                        //drill
                        if (step == 0)
                        {
                            if (WasLastAbility(GaussRound)) step++;
                            else return GaussRound;
                        }

                        if (step == 1)
                        {
                            if (WasLastAbility(Ricochet)) step++;
                            else return Ricochet;
                        }

                        if (step == 2)
                        {
                            if (IsOnCooldown(Drill)) step++;
                            else return Drill;
                        }

                        if (step == 3)
                        {
                            if (IsOnCooldown(BarrelStabilizer)) step++;
                            else return BarrelStabilizer;
                        }

                        if (step == 4)
                        {
                            if (gauge.Heat == 60) step++;
                            else return HeatedSlugshot;
                        }

                        if (step == 5)
                        {
                            if (GetRemainingCharges(Ricochet) == 1) step++;
                            else return Ricochet;
                        }

                        if (step == 6)
                        {
                            if (gauge.Heat == 65) step++;
                            else return HeatedCleanShot;
                        }

                        if (step == 7)
                        {
                            if (GetRemainingCharges(Reassemble) == 1) step++;
                            else return Reassemble;
                        }

                        if (step == 8)
                        {
                            if (GetRemainingCharges(GaussRound) == 1) step++;
                            else return GaussRound;
                        }

                        if (step == 9)
                        {
                            if (IsOnCooldown(AirAnchor)) step++;
                            else return AirAnchor;
                        }

                        if (step == 10)
                        {
                            if (GetRemainingCharges(Reassemble) < 1) step++;
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
                            if (WasLastAbility(AutomatonQueen)) step++;
                            else return AutomatonQueen;
                        }

                        if (step == 14)
                        {
                            if (WasLastAbility(Hypercharge)) step++;
                            else return Hypercharge;
                        }

                        if (step == 15)
                        {
                            if (WasLastAction(HeatBlast)) step++;
                            else return HeatBlast;
                        }

                        if (step == 16)
                        {
                            if (WasLastAbility(Ricochet)) step++;
                            else return Ricochet;
                        }

                        if (step == 17)
                        {
                            if (WasLastAction(HeatBlast)) step++;
                            else return HeatBlast;
                        }

                        if (step == 18)
                        {
                            if (WasLastAbility(GaussRound)) step++;
                            else return GaussRound;
                        }

                        if (step == 19)
                        {
                            if (WasLastAction(HeatBlast)) step++;
                            else return HeatBlast;
                        }

                        if (step == 20)
                        {
                            if (WasLastAbility(Ricochet)) step++;
                            else return Ricochet;
                        }

                        if (step == 21)
                        {
                            if (WasLastAction(HeatBlast)) step++;
                            else return HeatBlast;
                        }

                        if (step == 22)
                        {
                            if (WasLastAbility(GaussRound)) step++;
                            else return GaussRound;
                        }

                        if (step == 23)
                        {
                            if (WasLastAction(HeatBlast)) step++;
                            else return HeatBlast;
                        }

                        if (step == 24)
                        {
                            if (WasLastAbility(Ricochet)) step++;
                            else return Ricochet;
                        }

                        if (step == 25)
                        {
                            if (GetCooldownRemainingTime(Drill) > 17) step++;
                            else return Drill;
                        }
                        inOpener = false;
                    }
                }

                {
                    if (actionID is SplitShot or HeatedSplitShot && (step >= 26 || !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener)))
                    {
                        var inCombat = InCombat();
                        var gauge = GetJobGauge<MCHGauge>();

                        var wildfireCDTime = GetCooldownRemainingTime(Wildfire);

                        if (!inCombat && !inOpener)
                        {
                            openerFinished = false;

                        }

                        /*if (CanWeave(actionID,0.1) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && gauge.Heat <= 55 && inOpener == false 
                            && IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && !WasLastWeaponskill(ChainSaw) 
                            && (wildfireCDTime <= 9 || (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only) 
                            && gauge.IsOverheated)))
                            return BarrelStabilizer;*/
                        if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) && gauge.Heat <= 55 && inOpener == false && IsOffCooldown(BarrelStabilizer) && level >= Levels.BarrelStabilizer && !WasLastWeaponskill(ChainSaw) && (wildfireCDTime <= 9 || (wildfireCDTime >= 110 && !IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only) && gauge.IsOverheated)))
                        {
                            return BarrelStabilizer;
                        }
                        if (CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                        {
                            return All.HeadGraze;
                        }

                        //Wildfire stuff (separated because idk how else, drunk af rn)
                        if (!IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                        {
                            if (openerFinished && inOpener == false && (gauge.Heat >= 50 || WasLastAbility(Hypercharge)) && wildfireCDTime <= 2 && level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) &&
                                (WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast)))) //these try to ensure the correct loops
                            {
                                if (CanDelayedWeave(actionID) && !gauge.IsOverheated && !WasLastWeaponskill(ChainSaw))
                                {
                                    return Wildfire;
                                }
                                else if (CanDelayedWeave(actionID) && !gauge.IsOverheated && !WasLastWeaponskill(ChainSaw)) //maybe change this later, need WF then ChainSs
                                {
                                    return Wildfire;
                                }
                                else if (CanDelayedWeave(actionID) && gauge.IsOverheated)
                                {
                                    return Wildfire;
                                }

                            }
                        }

                        if (IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                        {
                            if (openerFinished && inOpener == false && /*(gauge.Heat >= 50 || WasLastAbility(Hypercharge)) &&*/ wildfireCDTime <= 1 && level >= Levels.Wildfire && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge)) /*&&*/
                                /*WasLastWeaponskill(ChainSaw) || (!WasLastWeaponskill(Drill) && !WasLastWeaponskill(AirAnchor) && !WasLastWeaponskill(HeatBlast))))*/ //these try to ensure the correct loops
                            {
                                if (CanDelayedWeave(actionID) && !gauge.IsOverheated && WasLastWeaponskill(AirAnchor) && HasEffect(Buffs.Reassembled))
                                {
                                    return Wildfire;
                                }
                                else if (CanDelayedWeave(actionID) && !gauge.IsOverheated && !WasLastWeaponskill(ChainSaw) && GetRemainingCharges(Reassemble) == 0) //maybe change this later, need WF then ChainSs
                                {
                                    return Wildfire;
                                }
                                else if (CanDelayedWeave(actionID) && gauge.IsOverheated)
                                {
                                    return Wildfire;
                                }

                            }
                        }

                        if (CanWeave(actionID) && !gauge.IsRobotActive && IsEnabled(CustomComboPreset.MCH_ST_Simple_Gadget) &&
                            (/*wildfireCDTime >= 2 && */(!WasLastAbility(Wildfire) || level < Levels.Wildfire)))
                        {
                            //steps to control robot timings
                            if (IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) && level >= 90)
                            {
                                // First condition
                                if (gauge.Battery == 50 && CombatEngageDuration().TotalSeconds > 61 && CombatEngageDuration().TotalSeconds < 68)
                                {
                                    return AutomatonQueen;
                                }

                                // Second condition
                                if (!WasLastAction(OriginalHook(CleanShot)))
                                {
                                    if (gauge.Battery == 100 && gauge.LastSummonBatteryPower == 50 && (GetCooldownRemainingTime(AirAnchor) <= 3 || IsOffCooldown(AirAnchor)))
                                        return AutomatonQueen;
                                }

                                // Third condition
                                while (gauge.LastSummonBatteryPower == 100 && gauge.Battery >= 80)
                                {
                                    return AutomatonQueen;
                                }

                                // Fourth condition
                                while (gauge.LastSummonBatteryPower != 50 && gauge.Battery == 100 && (GetCooldownRemainingTime(AirAnchor) <= 3 || IsOffCooldown(AirAnchor)))
                                {
                                    return AutomatonQueen;
                                }
                            }
                            if (level >= Levels.RookOverdrive && gauge.Battery == 100 && CombatEngageDuration().Seconds < 55 && !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                            {
                                Dalamud.Logging.PluginLog.Log("HUH DELAYTOOL IS OFF"); return OriginalHook(RookAutoturret);
                            }
                            else if (level >= Levels.RookOverdrive && gauge.Battery >= 50 && !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                            {
                                Dalamud.Logging.PluginLog.Log("HUH DELAYTOOL IS OFF"); return OriginalHook(RookAutoturret);
                            }
                            //else if (gauge.Battery >= 50 && level >= Levels.RookOverdrive && (CombatEngageDuration().Seconds >= 58 || CombatEngageDuration().Seconds <= 05))
                            //{
                            //    return OriginalHook(RookAutoturret);
                            //}

                        }


                        if (gauge.IsOverheated && level >= Levels.HeatBlast)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) && CanWeave(actionID, 0.6) && (wildfireCDTime > 2 || level < Levels.Wildfire)) //gauss and ricochet weave
                            {
                                var gaussCharges = GetRemainingCharges(GaussRound);
                                var gaussMaxCharges = GetMaxCharges(GaussRound);

                                var overheatTime = gauge.OverheatTimeRemaining;
                                var reasmCharges = GetRemainingCharges(Reassemble);

                                //Makes sure Reassemble isnt double weaved after a Gauss/Richochet during Hypercharge
                                if (overheatTime < 1.7 && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && inOpener == false && (WasLastAbility(GaussRound) || WasLastAbility(Ricochet)) &&
                                    (
                                        (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_ChainSaw) && reasmCharges >= 1 && GetCooldownRemainingTime(ChainSaw) <= 2)
                                        ||
                                        (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) && reasmCharges >= 1 && GetCooldownRemainingTime(AirAnchor) <= 2)
                                        ||
                                        (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) && reasmCharges >= 1 && GetCooldownRemainingTime(Drill) <= 2)
                                    ))
                                    return Reassemble;
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
                            if (IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) && GetCooldownRemainingTime(ChainSaw) < 2 && (wildfireCDTime < 5 || IsOffCooldown(Wildfire)))
                            {
                                return ChainSaw;
                            }
                            else return HeatBlast;
                        }

                        //HYPERCHARGE!!
                        if (CanWeave(actionID))
                        {
                            /*if (!IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))*/
                            {
                                if (gauge.Heat >= 50 && openerFinished && inOpener == false && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && level >= Levels.Hypercharge && !gauge.IsOverheated)
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
                            }

                            /*if (IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                            {
                                if (gauge.Heat >= 50 && openerFinished && inOpener == false && IsEnabled(CustomComboPreset.MCH_ST_Simple_WildCharge) && level >= Levels.Hypercharge && !gauge.IsOverheated)
                                {
                                    //Protection & ensures Hyper charged is double weaved with WF during reopener
                                    //if (HasEffect(Buffs.Wildfire) || level < Levels.Wildfire) return Hypercharge;

                                    if (level >= Levels.Drill && GetCooldownRemainingTime(Drill) >= 8)
                                    {
                                        if (level >= Levels.AirAnchor && GetCooldownRemainingTime(AirAnchor) >= 8)
                                        {
                                            if (level >= Levels.ChainSaw && GetCooldownRemainingTime(ChainSaw) <= 1 && (IsOffCooldown(Wildfire) || wildfireCDTime < 4))
                                            {
                                                if (UseHypercharge(gauge, wildfireCDTime)) return Hypercharge;
                                            }
                                            else if (level >= Levels.ChainSaw && GetCooldownRemainingTime(ChainSaw) >= 8)
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
                            }*/
                        }
                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) && CanWeave(actionID, 0.6))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold) && LevelChecked(All.SecondWind) && IsOffCooldown(All.SecondWind))
                                return All.SecondWind;
                        }
                        //tools
                        if ((IsOffCooldown(AirAnchor) || GetCooldownRemainingTime(AirAnchor) < 1) && level >= Levels.AirAnchor && inOpener == false)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && !HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor) &&
                                GetRemainingCharges(Reassemble) > 0)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble) && inOpener == false) return Reassemble;
                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_AirAnchor_MaxCharges) && inOpener == false) return Reassemble;

                            }
                            return AirAnchor;
                        }
                        else if ((IsOffCooldown(HotShot) || GetCooldownRemainingTime(HotShot) < 1) && level is >= Levels.Hotshot and < Levels.AirAnchor)
                            return HotShot;

                        if ((IsOffCooldown(Drill) || GetCooldownRemainingTime(Drill) < 1) && level >= Levels.Drill && inOpener == false)
                        {
                            if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill) &&
                                !HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) > 0)
                            {
                                if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges) && GetRemainingCharges(Reassemble) == GetMaxCharges(Reassemble)) return Reassemble;
                                else if (!IsEnabled(CustomComboPreset.MCH_ST_Simple_Assembling_Drill_MaxCharges)) return Reassemble;
                            }
                            return Drill;
                        }

                        if ((IsOffCooldown(ChainSaw) || GetCooldownRemainingTime(ChainSaw) < 1) && level >= Levels.ChainSaw && openerFinished && inOpener == false)
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
                            if (HasCharges(GaussRound) && (level < Levels.Ricochet || GetCooldownRemainingTime(GaussRound) < GetCooldownRemainingTime(Ricochet)))
                                return GaussRound;
                            else if (HasCharges(Ricochet) && level >= Levels.Ricochet)
                                return Ricochet;
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

                        if (lastComboMove == CleanShot && inOpener == false) openerFinished = true;
                    }

                    return actionID;
                }
            }
                private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
                {
                    uint wfTimer = 6; //default timer
                    if (LocalPlayer.Level < Levels.BarrelStabilizer) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                    // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
                    if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 60 && IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) || 
                        gauge.Heat > 70 && !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) || 
                        CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                    {
                        return true;
                    }

                    if (CombatEngageDuration().Minutes > 0 /*&& (wildfireCDTime >= wfTimer || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && 
                                                            * (IsOffCooldown(Wildfire) || wildfireCDTime < 1)))*/)
                    {
                    //if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90 && !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener) || 
                    //    wildfireCDTime <= 44 && gauge.Heat >= 60 && IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                    //{
                    //    return true;
                    //}
                        if ((wildfireCDTime >= 35 || wildfireCDTime <= 6 || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && IsOffCooldown(Wildfire))) && 
                            gauge.Heat >= 50 && IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                        {
                            return true;
                        }
                        if (wildfireCDTime <= 35 && gauge.Heat >= 50 && gauge.Battery >= 90 && !WasLastWeaponskill(OriginalHook(CleanShot)))
                        {
                            return true;
                        }
                        if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90 & !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                        {
                            return true;
                        }
                        if (CombatEngageDuration().Minutes % 2 == 0 && !IsEnabled(CustomComboPreset.MCH_DelayedTools_Opener))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
    }
