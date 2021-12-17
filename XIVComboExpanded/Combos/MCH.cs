using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
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
            AutoCrossbow = 16497,
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
            Wildfire = 2878;

        public static class Buffs
        {
            public const short
                Reassembled = 851;
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
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                ChargedActionMastery = 74,
                QueenOverdrive = 80;
        }
    }

    internal class MachinistMainCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistMainCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot)
            {
                var gauge = GetJobGauge<MCHGauge>();
                var drillCD = GetCooldown(MCH.Drill);
                var airAnchorCD = GetCooldown(MCH.AirAnchor);
                var hotshotCD = GetCooldown(MCH.HotShot);
                var reassembleCD = GetCooldown(MCH.Reassemble);
                var wildfireCD = GetCooldown(MCH.Wildfire);
                var heatBlastCD = GetCooldown(MCH.HeatBlast);
                var gaussCD = GetCooldown(MCH.GaussRound);
                var ricochetCD = GetCooldown(MCH.Ricochet);

                if (IsEnabled(CustomComboPreset.MachinistHeatBlastOnMainCombo) && gauge.IsOverheated)
                {
                    if (!wildfireCD.IsCooldown && level >= MCH.Levels.Wildfire)
                        return MCH.Wildfire;
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
                }

                if (IsEnabled(CustomComboPreset.MachinistAlternateMainCombo))
                {
                    if (reassembleCD.CooldownRemaining >= 55 && !airAnchorCD.IsCooldown && level >= MCH.Levels.AirAnchor)
                        return MCH.AirAnchor;
                    if (reassembleCD.CooldownRemaining >= 55 && !drillCD.IsCooldown && level >= MCH.Levels.Drill)
                        return MCH.Drill;
                    if (reassembleCD.CooldownRemaining >= 55 && !hotshotCD.IsCooldown && level <= 75)
                        return MCH.HotShot;
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
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistHeatblastGaussRicochetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HeatBlast)
            {
                var wildfireCD = GetCooldown(MCH.Wildfire);
                var heatBlastCD = GetCooldown(MCH.HeatBlast);
                var gaussCD = GetCooldown(MCH.GaussRound);
                var ricochetCD = GetCooldown(MCH.Ricochet);

                var gauge = GetJobGauge<MCHGauge>();
                if (!gauge.IsOverheated && level >= MCH.Levels.Hypercharge)
                    return MCH.Hypercharge;
                if (!wildfireCD.IsCooldown && level >= MCH.Levels.Wildfire)
                    return MCH.Wildfire;
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
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistGaussRoundRicochetFeature;

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

    internal class MachinistOverheatFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistOverheatFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HeatBlast || actionID == MCH.AutoCrossbow)
            {
                var gauge = GetJobGauge<MCHGauge>();
                if (!gauge.IsOverheated && level >= MCH.Levels.Hypercharge)
                    return MCH.Hypercharge;

                if (level < MCH.Levels.AutoCrossbow)
                    return MCH.HeatBlast;
            }

            return actionID;
        }
    }

    internal class MachinistSpreadShotFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistSpreadShotFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.AutoCrossbow)
            {
                var gauge = GetJobGauge<MCHGauge>();
                if (!gauge.IsOverheated)
                    return MCH.SpreadShot;
                if (gauge.IsOverheated && level >= MCH.Levels.AutoCrossbow)
                    return MCH.AutoCrossbow;
            }

            return actionID;
        }
    }

    internal class MachinistOverdriveFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.MachinistOverdriveFeature;

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

        internal class MchDrillAirFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.MchDrillAirFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == MCH.Drill || actionID == MCH.HotShot || actionID == MCH.AirAnchor)
                {
                    if (level < MCH.Levels.Drill)
                        return MCH.HotShot;

                    var anchorHSCD = GetCooldown(MCH.AirAnchor);
                    var drillCD = GetCooldown(MCH.Drill);

                    if (!drillCD.IsCooldown && !anchorHSCD.IsCooldown)
                        return actionID;

                    return drillCD.CooldownRemaining < anchorHSCD.CooldownRemaining
                        ? MCH.Drill
                        : MCH.AirAnchor;
                }

                return actionID;
            }
        }
    }
}
