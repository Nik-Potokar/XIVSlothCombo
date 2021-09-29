using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class BLM
    {
        public const byte ClassID = 7;
        public const byte JobID = 25;

        public const uint
            Fire = 141,
            Blizzard = 142,
            Thunder = 144,
            Blizzard2 = 146,
            Transpose = 149,
            Fire3 = 152,
            Thunder3 = 153,
            Blizzard3 = 154,
            Scathe = 156,
            Freeze = 159,
            Flare = 162,
            LeyLines = 3573,
            Enochian = 3575,
            Blizzard4 = 3576,
            Fire4 = 3577,
            BetweenTheLines = 7419,
            Despair = 16505,
            UmbralSoul = 16506,
            Xenoglossy = 16507;

        public static class Buffs
        {
            public const short
                Thundercloud = 164,
                LeyLines = 737,
                Firestarter = 165;
        }

        public static class Debuffs
        {
            public const short
                Thunder = 161,
                Thunder3 = 163;
        }

        public static class Levels
        {
            public const byte
                Fire3 = 34,
                Freeze = 35,
                Blizzard3 = 40,
                Thunder3 = 45,
                Flare = 50,
                Enochian = 56,
                Blizzard4 = 58,
                Fire4 = 60,
                BetweenTheLines = 62,
                Despair = 72,
                UmbralSoul = 76,
                Xenoglossy = 80;
        }
    }
    internal class BlackBlizzardFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackBlizzardFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 142)
            {
                var gauge = GetJobGauge<BLMGauge>().InUmbralIce;
                if (level >= 40 && !gauge)
                {
                    return 154u;
                }
            }
            if (actionID == 159 && level < 35)
            {
                return 146u;
            }
            return actionID;
        }
    }
    internal class BlackFireFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackFire13Feature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 141)
            {
                var gauge = CustomCombo.GetJobGauge<BLMGauge>();
                if (level >= 34 && !gauge.InAstralFire || CustomCombo.HasEffect(165))
                {
                    return CustomCombo.OriginalHook(152u);
                }
            }
            return actionID;
        }
    }
    internal class BlackLeyLinesFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackLeyLinesFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 3573 && CustomCombo.HasEffect(737) && level >= 62)
            {
                return 7419u;
            }
            return actionID;
        }
    }

    internal class BlackManaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackManaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 149)
            {
                var gauge = CustomCombo.GetJobGauge<BLMGauge>();
                if (gauge.InUmbralIce && gauge.IsEnochianActive && level >= 76)
                {
                    return 16506u;
                }
            }
            return actionID;
        }
    }
    internal class BlackEnochianFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackEnochianFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Enochian) ;
            {
                var gauge = GetJobGauge<BLMGauge>();
                var thundercloudduration = FindEffectAny(BLM.Buffs.Thundercloud);
                var thunderdebuffontarget = FindTargetEffect(BLM.Debuffs.Thunder3);
                var thunderOneDebuff = FindTargetEffect(BLM.Debuffs.Thunder);
                if (gauge.IsEnochianActive)
                {
                    if (gauge.InUmbralIce && level >= BLM.Levels.Blizzard4)
                    {
                        if (gauge.ElementTimeRemaining >= 5000 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature))
                            if (HasEffect(BLM.Buffs.Thundercloud))
                                if ((thundercloudduration.RemainingTime < 4 && thundercloudduration.RemainingTime > 0) || TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4)
                                        return BLM.Thunder3;
                        return BLM.Blizzard4;
                    }
                    if (level >= BLM.Levels.Fire4)
                        {
                        if (gauge.ElementTimeRemaining >= 6000 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature))
                            if (HasEffect(BLM.Buffs.Thundercloud))
                                if ((thundercloudduration.RemainingTime < 4 && thundercloudduration.RemainingTime > 0) || TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4)
                                    return BLM.Thunder3;
                        if (gauge.ElementTimeRemaining < 3000 && HasEffect(BLM.Buffs.Firestarter) && CustomCombo.IsEnabled(CustomComboPreset.BlackFire13Feature))
                            return BLM.Fire3;
                        if (LocalPlayer.CurrentMp < 2400 && level >= BLM.Levels.Despair && CustomCombo.IsEnabled(CustomComboPreset.BlackDespairFeature))
                            {
                                return BLM.Despair;
                            }
                        if (gauge.ElementTimeRemaining < 6000 && !HasEffect(BLM.Buffs.Firestarter) && (CustomCombo.IsEnabled(CustomComboPreset.BlackFire13Feature)))
                            return BLM.Fire;
                        return BLM.Fire4;
                        }


                    if (gauge.ElementTimeRemaining >= 5000 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature) && level < BLM.Levels.Thunder3)
                            if (HasEffect(BLM.Buffs.Thundercloud))
                                if ((thundercloudduration.RemainingTime < 4 && thundercloudduration.RemainingTime > 0) || TargetHasEffect(BLM.Debuffs.Thunder) && thunderOneDebuff.RemainingTime < 4)
                                    return BLM.Thunder;
                        if (level < BLM.Levels.Fire3)
                            return BLM.Fire;
                        if (gauge.InAstralFire && (level < BLM.Levels.Enochian || gauge.IsEnochianActive))
                        {
                            if (HasEffect(BLM.Buffs.Firestarter))
                                return BLM.Fire3;
                            return BLM.Fire;
                        }
                       
                    }
                }
            return actionID;
            }
        }
    }
