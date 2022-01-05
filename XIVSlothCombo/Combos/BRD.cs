using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BRD
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

        public const uint
            HeavyShot = 97,
            StraightShot = 98,
            VenomousBite = 100,
            RagingStrikes = 101,
            QuickNock = 106,
            Barrage = 107,
            Bloodletter = 110,
            Windbite = 113,
            MagesBallad = 114,
            ArmysPaeon = 116,
            RainOfDeath = 117,
            BattleVoice = 118,
            EmpyrealArrow = 3558,
            WanderersMinuet = 3559,
            IronJaws = 3560,
            Sidewinder = 3562,
            PitchPerfect = 7404,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            BurstShot = 16495,
            ApexArrow = 16496,
            Shadowbite = 16494,
            Ladonsbite = 25783,
            BlastArrow = 25784;

        public static class Buffs
        {
            public const short
                StraightShotReady = 122,
                BlastArrowReady = 2692,
                ShadowbiteReady = 3002;
        }

        public static class Debuffs
        {
            public const short
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Levels
        {
            public const byte
                StraightShot = 2,
                RagingStrikes = 4,
                VenomousBite = 6,
                Bloodletter = 12,
                Windbite = 30,
                MagesBallad = 30,
                ArmysPaeon = 40,
                RainOfDeath = 45,
                BattleVoice = 50,
                PitchPerfect = 52,
                EmpyrealArrow = 54,
                IronJaws = 56,
                WanderersMinuet = 52,
                Sidewinder = 60,
                CausticBite = 64,
                StormBite = 64,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                Shadowbite = 72,
                BurstShot = 76,
                ApexArrow = 80,
                Ladonsbite = 82,
                BlastArrow = 86;
        }
    }

    // Replace Wanderer's Minuet with PP when in WM.
    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardWanderersPitchPerfectFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)
            {
                if (GetJobGauge<BRDGauge>().Song == Song.WANDERER)
                    return BRD.PitchPerfect;
            }

            return actionID;
        }
    }

    // Replace HS/BS with SS/RA when procced.
    internal class BardStraightShotUpgradeFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardStraightShotUpgradeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {       
                    var gauge = GetJobGauge<BRDGauge>().SoulVoice;
                    if (gauge == 100)
                        return BRD.ApexArrow;
                    if (HasEffect(BRD.Buffs.BlastArrowReady) && level >= BRD.Levels.BlastArrow)
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardDoTMaintain))
                {
                    var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);
                    {
                        if ((venomous && windbite && level >= BRD.Levels.IronJaws && incombat) && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4 && level >= BRD.Levels.IronJaws && incombat))
                            return BRD.IronJaws;
                        if ((caustic && stormbite && level >= BRD.Levels.IronJaws && incombat) && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4 && level >= BRD.Levels.IronJaws && incombat))
                            return BRD.IronJaws;
                        if (venomous && level < BRD.Levels.IronJaws && incombat && (venomousDuration.RemainingTime < 4))
                            return BRD.VenomousBite;
                        if (windbite && level < BRD.Levels.IronJaws && incombat && (windbiteDuration.RemainingTime < 4))
                            return BRD.Windbite;
                    }
                }
                    if (HasEffect(BRD.Buffs.StraightShotReady))
                    return OriginalHook(BRD.RefulgentArrow);
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);
                    if (venomous is not null && windbite is not null)
                    {
                        if (venomous?.RemainingTime < windbite?.RemainingTime)
                            return BRD.VenomousBite;
                        return BRD.Windbite;
                    }
                    else if (windbite is not null || level < BRD.Levels.Windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    return BRD.Windbite;
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous && windbite)
                        return BRD.IronJaws;

                    if (windbite)
                        return BRD.VenomousBite;

                    return BRD.Windbite;
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (caustic && stormbite)
                    return BRD.IronJaws;

                if (stormbite)
                    return BRD.CausticBite;

                return BRD.Stormbite;
            }

            return actionID;
        }
    }
    internal class BardIronJawsAlternateFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsAlternateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);
                    if (venomous is not null && windbite is not null)
                    {
                        if (venomous?.RemainingTime < windbite?.RemainingTime)
                            return BRD.VenomousBite;
                        return BRD.Windbite;
                    }
                    else if (windbite is not null || level < BRD.Levels.Windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    return BRD.Windbite;
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous && windbite && venomousDuration.RemainingTime < 4 || venomous && windbite && windbiteDuration.RemainingTime < 4)
                        return BRD.IronJaws;

                    if (windbite)
                        return BRD.VenomousBite;

                    return BRD.Windbite;
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (caustic && stormbite && causticDuration.RemainingTime < 4 || caustic && stormbite && stormbiteDuration.RemainingTime < 4)
                    return BRD.IronJaws;

                if (stormbite)
                    return BRD.CausticBite;

                return BRD.Stormbite;
            }

            return actionID;
        }
    }

    internal class BardApexFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardApexFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.SoulVoice == 100)
                    return BRD.ApexArrow;
            }

            return actionID;
        }
    }

    internal class BardoGCDAoEFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardoGCDAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.RainOfDeath)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (!GetCooldown(BRD.EmpyrealArrow).IsCooldown && (level >= BRD.Levels.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (!GetCooldown(BRD.Bloodletter).IsCooldown)
                    return BRD.RainOfDeath;
                if (!GetCooldown(BRD.Sidewinder).IsCooldown && (level >= BRD.Levels.Sidewinder))
                    return BRD.Sidewinder;

            }

            return OriginalHook(actionID);
        }
    }

    internal class BardSimpleAoEFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardSimpleAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Ladonsbite || actionID == BRD.QuickNock)
            {
                var gauge = GetJobGauge<BRDGauge>();
                var soulvoice = GetJobGauge<BRDGauge>().SoulVoice;
                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3 && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                    return BRD.PitchPerfect;
                if (!GetCooldown(BRD.EmpyrealArrow).IsCooldown && (level >= BRD.Levels.EmpyrealArrow) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                    return BRD.EmpyrealArrow;
                if (GetCooldown(BRD.RainOfDeath).CooldownRemaining < 30 && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                    return BRD.RainOfDeath;
                if (!GetCooldown(BRD.Sidewinder).IsCooldown && (level >= BRD.Levels.Sidewinder) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                    return BRD.Sidewinder;
                if (HasEffect(BRD.Buffs.ShadowbiteReady) && level >= BRD.Levels.Shadowbite)
                    return BRD.Shadowbite;
                if (soulvoice == 100 && level >= BRD.Levels.ApexArrow)
                    return BRD.ApexArrow;
                if (HasEffect(BRD.Buffs.BlastArrowReady) && level >= BRD.Levels.BlastArrow)
                    return BRD.BlastArrow;

                if (IsEnabled(CustomComboPreset.SimpleAoESongOption))
                {
                    if (gauge.SongTimer < 1 && GetCooldown(actionID).IsCooldown || gauge.Song == Song.ARMY && GetCooldown(actionID).IsCooldown)
                    {
                        if (!GetCooldown(BRD.WanderersMinuet).IsCooldown && (level >= BRD.Levels.WanderersMinuet) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.WanderersMinuet;
                        if (!GetCooldown(BRD.MagesBallad).IsCooldown && (level >= BRD.Levels.MagesBallad) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.MagesBallad;
                        if (!GetCooldown(BRD.ArmysPaeon).IsCooldown && (level >= BRD.Levels.ArmysPaeon) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.ArmysPaeon;
                    }
                }


            }

            return OriginalHook(actionID);
        }
    }

    internal class BardoGCDSingleTargetFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardoGCDSingleTargetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Bloodletter)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (IsEnabled(CustomComboPreset.BardSongsFeature) && gauge.SongTimer < 1 || IsEnabled(CustomComboPreset.BardSongsFeature) && gauge.Song == Song.ARMY)
                {
                    if (!GetCooldown(BRD.WanderersMinuet).IsCooldown && (level >= BRD.Levels.WanderersMinuet))
                        return BRD.WanderersMinuet;
                    if (!GetCooldown(BRD.MagesBallad).IsCooldown && (level >= BRD.Levels.MagesBallad))
                        return BRD.MagesBallad;
                    if (!GetCooldown(BRD.ArmysPaeon).IsCooldown && (level >= BRD.Levels.ArmysPaeon))
                        return BRD.ArmysPaeon;
                }

                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (!GetCooldown(BRD.EmpyrealArrow).IsCooldown && (level >= BRD.Levels.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (!GetCooldown(BRD.Bloodletter).IsCooldown)
                    return BRD.Bloodletter;
                if (!GetCooldown(BRD.Sidewinder).IsCooldown && (level >= BRD.Levels.Sidewinder))
                    return BRD.Sidewinder;
            }
            return actionID;
        }
    }
    internal class BardAoEComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardAoEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {

                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {

                    if (level >= BRD.Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100)
                        return BRD.ApexArrow;

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;

                }

                if (IsEnabled(CustomComboPreset.BardAoEComboFeature) && level >= BRD.Levels.Shadowbite && HasEffectAny(BRD.Buffs.ShadowbiteReady))
                    return BRD.Shadowbite;

            }

            return actionID;
        }
    }
    internal class SimpleBardFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SimpleBardFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                var gauge = GetJobGauge<BRDGauge>();
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);
                var heavyshotCD = GetCooldown(actionID);
                var soulvoice = GetJobGauge<BRDGauge>().SoulVoice;
                if (IsEnabled(CustomComboPreset.SimpleBardFeature) && incombat)
                {
                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3 && level >= BRD.Levels.PitchPerfect && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                        return BRD.PitchPerfect;
                    if (!GetCooldown(BRD.EmpyrealArrow).IsCooldown && (level >= BRD.Levels.EmpyrealArrow) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                        return BRD.EmpyrealArrow;
                    if (GetCooldown(BRD.Bloodletter).CooldownRemaining < 30 && level >= BRD.Levels.Bloodletter && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                        return BRD.Bloodletter;
                    if (!GetCooldown(BRD.Sidewinder).IsCooldown && (level >= BRD.Levels.Sidewinder) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                        return BRD.Sidewinder;
                    if (soulvoice == 100 && level >= BRD.Levels.ApexArrow)
                        return BRD.ApexArrow;
                    if (HasEffect(BRD.Buffs.BlastArrowReady) && level >= BRD.Levels.BlastArrow)
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.SimpleSongOption))
                {
                    if (gauge.SongTimer < 1 && heavyshotCD.IsCooldown || gauge.Song == Song.ARMY && heavyshotCD.IsCooldown)
                    {
                        if (!GetCooldown(BRD.MagesBallad).IsCooldown && (level >= BRD.Levels.MagesBallad) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.MagesBallad;
                        if (!GetCooldown(BRD.WanderersMinuet).IsCooldown && (level >= BRD.Levels.WanderersMinuet) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.WanderersMinuet;
                        if (!GetCooldown(BRD.ArmysPaeon).IsCooldown && (level >= BRD.Levels.ArmysPaeon) && GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7)
                            return BRD.ArmysPaeon;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    if ((venomous && windbite && level >= BRD.Levels.IronJaws && incombat) && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4 && level >= BRD.Levels.IronJaws && incombat))
                        return BRD.IronJaws;
                    if (venomous && level < BRD.Levels.IronJaws && incombat && (venomousDuration.RemainingTime < 4))
                        return BRD.VenomousBite;
                    if (windbite && level < BRD.Levels.IronJaws && incombat && (windbiteDuration.RemainingTime < 4))
                        return BRD.Windbite;

                    if (IsEnabled(CustomComboPreset.SimpleDoTOption) && incombat)
                    {
                            if (!windbite && level >= BRD.Levels.Windbite)
                            return OriginalHook(BRD.Windbite);
                            if (!venomous && level >= BRD.Levels.VenomousBite)
                            return OriginalHook(BRD.VenomousBite);
                    }
                    else if (HasEffect(BRD.Buffs.StraightShotReady))
                        return OriginalHook(BRD.RefulgentArrow);
                    return OriginalHook(BRD.BurstShot);
                }

                if ((caustic && stormbite && level >= BRD.Levels.IronJaws && incombat) && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4 && level >= BRD.Levels.IronJaws && incombat))
                    return BRD.IronJaws;
                if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                {
                    if (!caustic && level >= BRD.Levels.CausticBite && incombat)
                        return BRD.CausticBite;
                    if (!stormbite && level >= BRD.Levels.StormBite && incombat)
                        return BRD.Stormbite;
                }
                if (gauge.SoulVoice == 100 && level >= BRD.Levels.ApexArrow)
                    return BRD.ApexArrow;

                if (HasEffect(BRD.Buffs.StraightShotReady))
                    return OriginalHook(BRD.RefulgentArrow);
                return OriginalHook(BRD.BurstShot);
            }
            return actionID;
        }
    }
    internal class BardBuffsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardBuffsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Barrage)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (!GetCooldown(BRD.RagingStrikes).IsCooldown && (level >= BRD.Levels.RagingStrikes))
                    return BRD.RagingStrikes;
                if (!GetCooldown(BRD.BattleVoice).IsCooldown && (level >= BRD.Levels.BattleVoice))
                    return BRD.BattleVoice;
            }
            return OriginalHook(actionID);
        }
    }
    internal class BardOneButtonSongs : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardOneButtonSongs;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)  // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
            {
                if (!GetCooldown(BRD.WanderersMinuet).IsCooldown && (level >= BRD.Levels.WanderersMinuet))
                    return BRD.WanderersMinuet;
                if (!GetCooldown(BRD.MagesBallad).IsCooldown && (level >= BRD.Levels.MagesBallad))
                    return BRD.MagesBallad;
                if (!GetCooldown(BRD.ArmysPaeon).IsCooldown && (level >= BRD.Levels.ArmysPaeon))
                    return BRD.ArmysPaeon;
            }
            return OriginalHook(actionID);
        }
    }
}


