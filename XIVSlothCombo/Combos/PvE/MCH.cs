using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
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
            Wildfire = 2878,
            Dismantle = 2887;

        internal static class Buffs
        {
            internal const ushort
                Reassembled = 851,
                Tactician = 1951,
                Wildfire = 1946,
                Overheated = 2688;
        }

        internal static class Debuffs
        {
            internal const ushort
            Dismantled = 2887;
        }

        private static MCHGauge Gauge => CustomComboFunctions.GetJobGauge<MCHGauge>();

        internal static class Config
        {
            internal const string
                MCH_ST_SecondWindThreshold = "MCH_ST_SecondWindThreshold",
                MCH_AoE_SecondWindThreshold = "MCH_AoE_SecondWindThreshold",
                MCH_ST_QueenThreshold = "MCH_ST_QueenThreshold",
                MCH_ST_Simple_OpenerSelection = "MCH_ST_Simple_OpenerSelection",
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

        internal class All_PRanged_Dismantle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.All_PRanged_Dismantle;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Dismantle)
                    if (TargetHasEffectAny(Debuffs.Dismantled) && IsOffCooldown(Dismantle))
                        return BLM.Fire;

                return actionID;
            }
        }

        internal class MCH_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_MainCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is CleanShot)
                {
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var battery = GetJobGauge<MCHGauge>().Battery;
                    var heat = GetJobGauge<MCHGauge>().Heat;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_ST_BarrelStabilizer_DriftProtection))
                    {
                        if (ActionReady(BarrelStabilizer) && heat < 20 && CanWeave(actionID))
                            return BarrelStabilizer;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_HeatBlast) && HasEffect(Buffs.Overheated))
                    {
                        if (CanWeave(actionID, 0.6))
                        {
                            if (!ActionReady(Ricochet) && HasCharges(GaussRound))
                                return GaussRound;

                            if (HasCharges(GaussRound) && gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
                                return GaussRound;

                            else if (ActionReady(Ricochet))
                                return Ricochet;
                        }

                        if (LevelChecked(HeatBlast)) // prioritize heatblast
                            return HeatBlast;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_Cooldowns))
                    {
                        if (HasEffect(Buffs.Reassembled) && ActionReady(AirAnchor))
                            return AirAnchor;

                        if (IsOnCooldown(AirAnchor) && ActionReady(Drill))
                            return Drill;

                        if (HasEffect(Buffs.Reassembled) && ActionReady(ChainSaw))
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
                        if (ActionReady(Ricochet) && GetRemainingCharges(Ricochet) > 1)
                            return Ricochet;

                        if (ActionReady(GaussRound) && GetRemainingCharges(GaussRound) > 1)
                            return GaussRound;
                    }


                    if (IsEnabled(CustomComboPreset.MCH_ST_MainComboAlternate))
                    {
                        if (ActionReady(AirAnchor) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return AirAnchor;

                        if (ActionReady(ChainSaw) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return ChainSaw;

                        if (ActionReady(Drill) && (HasEffect(Buffs.Reassembled) || !HasCharges(Reassemble)))
                            return Drill;

                        if (!LevelChecked(AirAnchor) && ActionReady(HotShot) && (GetCooldownChargeRemainingTime(Reassemble) >= 55 || !HasCharges(Reassemble)))
                            return HotShot;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_ST_MainCombo_OverCharge) && CanWeave(actionID))
                    {
                        if (battery == 100)
                            return OriginalHook(RookAutoturret);

                        if (comboTime > 0)
                        {
                            if (lastComboMove == SplitShot && LevelChecked(OriginalHook(SlugShot)))
                                return OriginalHook(SlugShot);

                            if (lastComboMove == SlugShot && LevelChecked(OriginalHook(CleanShot)))
                                return OriginalHook(CleanShot);
                        }
                        return OriginalHook(SplitShot);
                    }
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
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var heat = GetJobGauge<MCHGauge>().Heat;

                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel)
                        && ActionReady(BarrelStabilizer)
                        && heat < 50
                        && !HasEffect(Buffs.Overheated))
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_ST_Wildfire)
                        && IsOffCooldown(Hypercharge)
                        && ActionReady(Wildfire)
                        && heat >= 50)
                        return Wildfire;

                    if (!HasEffect(Buffs.Overheated) && LevelChecked(Hypercharge))
                        return Hypercharge;

                    if (heatBlastCD.CooldownRemaining < 0.7 && LevelChecked(HeatBlast)) // Prioritize Heat Blast
                        return HeatBlast;

                    if (!LevelChecked(Ricochet))
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
                if (actionID is GaussRound or Ricochet)
                {
                    var gaussCd = GetCooldown(GaussRound);
                    var ricochetCd = GetCooldown(Ricochet);

                    // Prioritize the original if both are off cooldown
                    if (!LevelChecked(Ricochet))
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

        internal class MCH_Overdrive : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RookAutoturret or AutomatonQueen)
                {
                    var gauge = GetJobGauge<MCHGauge>();
                    if (gauge.IsRobotActive)
                        return OriginalHook(QueenOverdrive);
                }

                return actionID;
            }
        }
            
        internal class MCH_HotShotDrillChainSaw : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainSaw;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == Drill || actionID == HotShot || actionID == AirAnchor)
                    {
                        if (LevelChecked(ChainSaw))
                            return CalcBestAction(actionID, ChainSaw, AirAnchor, Drill);

                        if (LevelChecked(AirAnchor))
                            return CalcBestAction(actionID, AirAnchor, Drill);

                        if (LevelChecked(Drill))
                            return CalcBestAction(actionID, Drill, HotShot);

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
                    var heatBlastCD = GetCooldown(HeatBlast);
                    var gaussCD = GetCooldown(GaussRound);
                    var ricochetCD = GetCooldown(Ricochet);
                    var heat = GetJobGauge<MCHGauge>().Heat;

                    var gauge = GetJobGauge<MCHGauge>();
                    if (IsEnabled(CustomComboPreset.MCH_ST_AutoBarrel)
                        && ActionReady(BarrelStabilizer)
                        && heat < 50
                        && !HasEffect(Buffs.Overheated)
                       ) return BarrelStabilizer;

                    if (!HasEffect(Buffs.Overheated) && ActionReady(Hypercharge))
                        return Hypercharge;
                    if (heatBlastCD.CooldownRemaining < 0.7 && LevelChecked(AutoCrossbow)) // prioritize autocrossbow
                        return AutoCrossbow;
                    if (!LevelChecked(Ricochet))
                        return GaussRound;
                    if (gaussCD.CooldownRemaining < ricochetCD.CooldownRemaining)
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
                if (actionID is SpreadShot)
                {
                    var gauge = GetJobGauge<MCHGauge>();
                    var battery = GetJobGauge<MCHGauge>().Battery;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                     IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.MCH_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_OverCharge) && !gauge.IsOverheated)
                    {
                        if (battery == 100)
                            return OriginalHook(RookAutoturret);
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_GaussRicochet) && CanWeave(actionID) &&
                     (IsEnabled(CustomComboPreset.MCH_AoE_Gauss) || HasEffect(Buffs.Overheated)) && (HasCharges(Ricochet) || HasCharges(GaussRound)))
                    {
                        var gaussCharges = GetRemainingCharges(GaussRound);
                        var ricochetCharges = GetRemainingCharges(Ricochet);

                        if ((gaussCharges >= ricochetCharges || !LevelChecked(Ricochet)) &&
                            LevelChecked(GaussRound))
                            return GaussRound;

                        else if (ricochetCharges > 0 && LevelChecked(Ricochet))
                            return Ricochet;

                    }

                    if (ActionReady(BioBlaster) && !HasEffect(Buffs.Overheated) && IsEnabled(CustomComboPreset.MCH_AoE_Simple_Bioblaster))
                        return BioBlaster;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Simple_Hypercharge) && CanWeave(actionID))
                    {
                        if (gauge.Heat >= 50 && LevelChecked(AutoCrossbow) && !HasEffect(Buffs.Overheated))
                            return Hypercharge;
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_SecondWind) && CanWeave(actionID, 0.6))
                    {
                        if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.MCH_AoE_SecondWindThreshold) && (ActionReady(All.SecondWind)))
                            return All.SecondWind;
                    }

                    if (HasEffect(Buffs.Overheated) && LevelChecked(AutoCrossbow))
                        return AutoCrossbow;

                }

                return actionID;
            }
        }

        internal class MCH_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte openerstep = 0;
            internal static byte evenBurstStep = 0;
            internal static byte oddBurstStep = 0;
            internal static bool inOddFiller = false;
            internal static bool inEvenFiller = false;
            internal static bool fillerComplete = false;


            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SplitShot)
                {
                    int openerSelection = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_Simple_OpenerSelection);
                    bool openerReady = ActionReady(ChainSaw) && ActionReady(Wildfire) && ActionReady(BarrelStabilizer) && GetRemainingCharges(Ricochet) == 3 && GetRemainingCharges(GaussRound) == 3;
                    float wildfireCDTime = GetCooldownRemainingTime(Wildfire);
                    bool oddMinute = CombatEngageDuration().Minutes % 2 == 1;
                    bool evenMinute = CombatEngageDuration().Minutes % 2 == 0;
                    float hpTreshold = PluginConfiguration.GetCustomIntValue(Config.MCH_ST_SecondWindThreshold);

                    if (IsEnabled(CustomComboPreset.MCH_ST_Opener) && level >= 90)
                    {
                        if (openerSelection is 0)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Reassembled)) { inOpener = true; openerStarted = false; readyOpener = false; }
                            if ((readyOpener || openerStarted) && HasEffect(Buffs.Reassembled) && !inOpener) { openerStarted = true; return AirAnchor; } else { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                openerstep = 0;
                                return Reassemble;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires openerstep 0 and 1 to be explicit since the inCombat check can be slow
                            if ((openerstep == 1 && lastComboMove is AirAnchor && !HasEffect(Buffs.Reassembled))
                                || (inOpener && openerstep >= 2 && IsOffCooldown(actionID) && !InCombat())) inOpener = false;


                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Reassembled) &&
                                IsEnabled(CustomComboPreset.MCH_ST_Opener) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                //we do it in steps to be able to control it
                                if (openerstep is 0)
                                {
                                    if (IsOnCooldown(AirAnchor)) openerstep++;
                                    else return AirAnchor;
                                }

                                if (openerstep == 1)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 2)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 3)
                                {
                                    if (IsOnCooldown(Drill)) openerstep++;
                                    else return Drill;
                                }

                                if (openerstep == 4)
                                {
                                    if (IsOnCooldown(BarrelStabilizer)) openerstep++;
                                    else return BarrelStabilizer;
                                }

                                if (openerstep == 5)
                                {
                                    if (WasLastWeaponskill(HeatedSplitShot)) openerstep++;
                                    else return HeatedSplitShot;
                                }

                                if (openerstep == 6)
                                {
                                    if (WasLastWeaponskill(HeatedSlugshot)) openerstep++;
                                    else return HeatedSlugshot;
                                }

                                if (openerstep == 7)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 8)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 9)
                                {
                                    if (WasLastWeaponskill(HeatedCleanShot)) openerstep++;
                                    else return HeatedCleanShot;
                                }

                                if (openerstep == 10)
                                {
                                    if (GetRemainingCharges(Reassemble) is 0) openerstep++;
                                    else return Reassemble;
                                }

                                if (openerstep == 11)
                                {
                                    if (IsOnCooldown(Wildfire)) openerstep++;
                                    else return Wildfire;
                                }

                                if (openerstep == 12)
                                {
                                    if (IsOnCooldown(ChainSaw)) openerstep++;
                                    else return ChainSaw;
                                }

                                if (openerstep == 13)
                                {
                                    if (IsOnCooldown(AutomatonQueen)) openerstep++;
                                    else return AutomatonQueen;
                                }

                                if (openerstep == 14)
                                {
                                    if (IsOnCooldown(Hypercharge)) openerstep++;
                                    else return Hypercharge;
                                }

                                if (openerstep == 15)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 4) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 16)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 17)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 3) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 18)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 19)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 2) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 20)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 21)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 1) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 22)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 23)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 0) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 24)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 25)
                                {
                                    if (WasLastAction(Drill)) openerstep++;
                                    else return Drill;
                                }

                                inOpener = false;
                            }
                        }

                        if (openerSelection is 1)
                        {
                            // Check to start opener
                            if (openerStarted && WasLastAction(HeatedSplitShot)) { inOpener = true; openerStarted = false; readyOpener = false; }
                            if ((readyOpener || openerStarted) && WasLastAction(HeatedSplitShot) && !inOpener) { openerStarted = true; return AirAnchor; } else { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                openerstep = 0;
                                return HeatedSplitShot;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires openerstep 0 and 1 to be explicit since the inCombat check can be slow
                            if ((openerstep == 1 && WasLastAction(HeatedSplitShot))
                                || (inOpener && openerstep >= 2 && IsOffCooldown(actionID) && !InCombat())) inOpener = false;


                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && WasLastAction(HeatedSplitShot) &&
                                IsEnabled(CustomComboPreset.MCH_ST_Opener) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                //we do it in steps to be able to control it
                                if (openerstep == 0)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 1)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 2)
                                {
                                    if (IsOnCooldown(Drill)) openerstep++;
                                    else return Drill;
                                }

                                if (openerstep == 3)
                                {
                                    if (IsOnCooldown(BarrelStabilizer)) openerstep++;
                                    else return BarrelStabilizer;
                                }

                                if (openerstep == 4)
                                {
                                    if (WasLastWeaponskill(HeatedSlugshot)) openerstep++;
                                    else return HeatedSlugshot;
                                }

                                if (openerstep == 5)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 6)
                                {
                                    if (WasLastWeaponskill(HeatedCleanShot)) openerstep++;
                                    else return HeatedCleanShot;
                                }

                                if (openerstep == 7)
                                {
                                    if (GetRemainingCharges(Reassemble) is 1) openerstep++;
                                    else return Reassemble;
                                }

                                if (openerstep == 8)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 9)
                                {
                                    if (IsOnCooldown(AirAnchor)) openerstep++;
                                    else return AirAnchor;
                                }

                                if (openerstep == 10)
                                {
                                    if (GetRemainingCharges(Reassemble) is 0) openerstep++;
                                    else return Reassemble;
                                }

                                if (openerstep == 11)
                                {
                                    if (IsOnCooldown(Wildfire)) openerstep++;
                                    else return Wildfire;
                                }

                                if (openerstep == 12)
                                {
                                    if (IsOnCooldown(ChainSaw)) openerstep++;
                                    else return ChainSaw;
                                }

                                if (openerstep == 13)
                                {
                                    if (IsOnCooldown(AutomatonQueen)) openerstep++;
                                    else return AutomatonQueen;
                                }

                                if (openerstep == 14)
                                {
                                    if (IsOnCooldown(Hypercharge)) openerstep++;
                                    else return Hypercharge;
                                }

                                if (openerstep == 15)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 4) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 16)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 17)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 3) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 18)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 19)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 2) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 20)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 21)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 1) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 22)
                                {
                                    if (WasLastAction(GaussRound)) openerstep++;
                                    else return GaussRound;
                                }

                                if (openerstep == 23)
                                {
                                    if (WasLastAction(HeatBlast) && GetBuffStacks(Buffs.Overheated) is 0) openerstep++;
                                    else return HeatBlast;
                                }

                                if (openerstep == 24)
                                {
                                    if (WasLastAction(Ricochet)) openerstep++;
                                    else return Ricochet;
                                }

                                if (openerstep == 25)
                                {
                                    if (WasLastAction(Drill)) openerstep++;
                                    else return Drill;
                                }

                                inOpener = false;
                            }
                        }
                    }

                    if (!inOpener)
                    {
                        //BS in even burst
                        if (openerSelection is 0 && evenMinute && CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) &&
                                Gauge.Heat <= 55 && ActionReady(BarrelStabilizer) && WasLastAction(HeatBlast) && Gauge.IsOverheated && (wildfireCDTime <= 9 || (wildfireCDTime >= 110 &&
                                IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only))))
                            return BarrelStabilizer;

                        if (openerSelection is 1 && evenMinute && CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) &&
                            Gauge.Heat <= 55 && ActionReady(BarrelStabilizer) && WasLastAction(Drill) && (wildfireCDTime <= 9 || (wildfireCDTime >= 110 &&
                            IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only))))
                            return BarrelStabilizer;

                        // General BS use
                        if (level < 90 && CanWeave(actionID) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer) &&
                                Gauge.Heat <= 55 && ActionReady(BarrelStabilizer) && (wildfireCDTime <= 9 || (wildfireCDTime >= 110 &&
                                IsEnabled(CustomComboPreset.MCH_ST_Simple_Stabilizer_Wildfire_Only))))
                            return BarrelStabilizer;

                        //gauss and ricochet overcap protection
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_GaussRicochet) &&
                            CanWeave(actionID) && !Gauge.IsOverheated && !HasEffect(Buffs.Wildfire))
                        {
                            if (GetRemainingCharges(GaussRound) >= GetMaxCharges(GaussRound))
                                return GaussRound;

                            if (GetRemainingCharges(Ricochet) >= GetMaxCharges(Ricochet))
                                return Ricochet;
                        }

                        //queen
                        if (IsEnabled(CustomComboPreset.MCH_Simple_QueenUsage) && !Gauge.IsOverheated && LevelChecked(OriginalHook(RookAutoturret)))
                        {

                            if (openerSelection is 0 && evenMinute && Gauge.Battery == 100 && WasLastAction(Drill))
                                return OriginalHook(RookAutoturret);

                            if (openerSelection is 0 && CombatEngageDuration().Minutes == 1 && Gauge.Battery == 70 && ActionReady(ChainSaw))
                                return OriginalHook(RookAutoturret);

                            if (openerSelection is 0 && oddMinute && Gauge.Battery >= 80 && Gauge.Battery <= 90 && ActionReady(ChainSaw))
                                return OriginalHook(RookAutoturret);

                            if (openerSelection is 1 && evenMinute && Gauge.Battery == 100 && GetCooldownRemainingTime(AirAnchor) == 2)
                                return OriginalHook(RookAutoturret);

                            if (openerSelection is 1 && CombatEngageDuration().Minutes == 1 && Gauge.Battery == 60)
                                return OriginalHook(RookAutoturret);

                            if (openerSelection is 1 && oddMinute && Gauge.Battery >= 80 && Gauge.Battery <= 90 && ActionReady(ChainSaw))
                                return OriginalHook(RookAutoturret);

                            if (level >= 80 && level < 90 && Gauge.Battery == 100)
                                return OriginalHook(RookAutoturret);

                            if (level  < 80 && Gauge.Battery == 80)
                                return OriginalHook(RookAutoturret);
                        }

                        // Wildfire
                        if (Gauge.Heat >= 50 && ActionReady(Wildfire) &&
                            IsEnabled(CustomComboPreset.MCH_ST_Simple_WildFire))
                        {
                            if (openerSelection is 0 && WasLastAction(ChainSaw) && CanWeave(actionID) && evenMinute)
                                return Wildfire;

                            if (openerSelection is 1 && HasEffect(Buffs.Reassembled) && IsOffCooldown(ChainSaw) && (CanDelayedWeave(actionID)) && evenMinute)
                                return Wildfire;

                            return Wildfire;
                        }

                        // Hypercharge
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Hypercharge) &&
                            Gauge.Heat >= 50 && LevelChecked(Hypercharge) && !Gauge.IsOverheated)  
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

                        //Heatblast, Gauss, Rico
                        if (Gauge.IsOverheated && LevelChecked(HeatBlast))
                        {
                            if (WasLastAction(HeatBlast) && CanWeave(actionID))
                            {
                                if (GetRemainingCharges(GaussRound) >= GetRemainingCharges(Ricochet))
                                    return GaussRound;

                                if (GetRemainingCharges(Ricochet) >= GetRemainingCharges(GaussRound))
                                    return Ricochet;
                            }
                            return HeatBlast;
                        }
                        

                        // OGCD's
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Reassembled) &&
                            (ActionReady(ChainSaw) || ActionReady(AirAnchor) || (!LevelChecked(AirAnchor) && ActionReady(Drill))) &&
                            !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                            return Reassemble;

                        if (ActionReady(ChainSaw) && HasEffect(Buffs.Reassembled) && IsEnabled(CustomComboPreset.MCH_ST_Simple_ChainSaw))
                            return ChainSaw;

                        if (ActionReady(OriginalHook(AirAnchor)) && IsEnabled(CustomComboPreset.MCH_ST_Simple_AirAnchor))
                            return OriginalHook(AirAnchor);

                        if (ActionReady(Drill) && IsEnabled(CustomComboPreset.MCH_ST_Simple_Drill))
                            return Drill;

                        // Interrupt
                        if (IsEnabled(CustomComboPreset.MCH_ST_Simple_Interrupt) &&
                            CanWeave(actionID) && CanInterruptEnemy() && ActionReady(All.HeadGraze))
                            return All.HeadGraze;

                        // healing
                        if (IsEnabled(CustomComboPreset.MCH_ST_SecondWind) &&
                            CanWeave(actionID, 0.6) && PlayerHealthPercentageHp() <= hpTreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        //1-2-3 Combo
                        if (comboTime > 0)
                        {
                            if (lastComboMove is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                                return OriginalHook(SlugShot);

                            if (lastComboMove is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                                return (IsEnabled(CustomComboPreset.MCH_ST_Simple_Reassembled) &&
                                    !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble))
                                    ? Reassemble
                                    : OriginalHook(CleanShot);
                        }
                        return OriginalHook(SplitShot);
                    }
                }
                return actionID;
            }

            private bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
            {
                uint wfTimer = 6; //default timer
                if (!LevelChecked(BarrelStabilizer)) wfTimer = 12; // just a little space to breathe and not delay the WF too much while you don't have access to the Barrel Stabilizer

                // i really do not remember why i put > 70 here for heat, and im afraid if i remove it itll break it lol
               if (CombatEngageDuration().Minutes == 0 && (gauge.Heat > 70 || CombatEngageDuration().Seconds <= 30) && !WasLastWeaponskill(OriginalHook(CleanShot)))
                    return true;

                if (CombatEngageDuration().Minutes > 0 && (wildfireCDTime >= wfTimer || WasLastAbility(Wildfire) || (WasLastWeaponskill(ChainSaw) && (IsOffCooldown(Wildfire) || wildfireCDTime < 1))))
                {
                    if (CombatEngageDuration().Minutes % 2 == 1 && gauge.Heat >= 90)
                        return true;

                    if (CombatEngageDuration().Minutes % 2 == 0)
                        return true;
                }

                return false;
            }
        }
    }
}
