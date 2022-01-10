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

    internal class BardSimpleAoEFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardSimpleAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Ladonsbite || actionID == BRD.QuickNock) {
                var gauge = GetJobGauge<BRDGauge>();
                var soulVoice = gauge.SoulVoice;
                var heavyShotOnCooldown = GetCooldown(BRD.HeavyShot).CooldownRemaining > 0.7;
                
                if (heavyShotOnCooldown) {
                    if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        return BRD.PitchPerfect;
                    if (level >= BRD.Levels.EmpyrealArrow && !GetCooldown(BRD.EmpyrealArrow).IsCooldown)
                        return BRD.EmpyrealArrow;
                    if (level >= BRD.Levels.RainOfDeath && GetCooldown(BRD.RainOfDeath).CooldownRemaining < 30)
                        return BRD.RainOfDeath;
                    if (level >= BRD.Levels.Sidewinder && !GetCooldown(BRD.Sidewinder).IsCooldown)
                        return BRD.Sidewinder;
                }

                if (level >= BRD.Levels.Shadowbite && HasEffect(BRD.Buffs.ShadowbiteReady))
                    return BRD.Shadowbite;
                if (level >= BRD.Levels.ApexArrow && soulVoice == 100)
                    return BRD.ApexArrow;
                if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;

                if (IsEnabled(CustomComboPreset.SimpleAoESongOption) && heavyShotOnCooldown) {
                    if ((gauge.SongTimer < 3) && GetCooldown(actionID).IsCooldown) {
                        if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                            return BRD.ArmysPaeon;
                        if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                            return BRD.WanderersMinuet;
                    }
                }
            }

            return OriginalHook(actionID);
        }
    }

    internal class SimpleBardFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SimpleBardFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot) {
                var gauge = GetJobGauge<BRDGauge>();
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var heavyShot = GetCooldown(actionID);
                var heavyShotOnCooldown = heavyShot.CooldownRemaining > 0.7;

                if (IsEnabled(CustomComboPreset.SimpleBardFeature) && inCombat) {
                    if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100)
                        return BRD.ApexArrow;
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;

                    if (heavyShotOnCooldown) {
                        if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                            return BRD.PitchPerfect;
                        if (level >= BRD.Levels.EmpyrealArrow && !GetCooldown(BRD.EmpyrealArrow).IsCooldown)
                            return BRD.EmpyrealArrow;
                        if (level >= BRD.Levels.Bloodletter && GetCooldown(BRD.Bloodletter).CooldownRemaining < 30)
                            return BRD.Bloodletter;
                        if (level >= BRD.Levels.Sidewinder && !GetCooldown(BRD.Sidewinder).IsCooldown)
                            return BRD.Sidewinder;
                    }
                }

                if (IsEnabled(CustomComboPreset.SimpleSongOption)) {
                    if (heavyShot.IsCooldown && (gauge.SongTimer < 3)) {
                        if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                            return BRD.ArmysPaeon;
                    }
                }


                var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                var useIronJaws = 
                    level >= BRD.Levels.IronJaws &&
                    ((venomous && venomousDuration.RemainingTime < 6) || (caustic && causticDuration.RemainingTime < 6)) ||
                    level >= BRD.Levels.IronJaws &&
                    (windbite && windbiteDuration.RemainingTime < 6) || (stormbite && stormbiteDuration.RemainingTime < 6);

                if (level < BRD.Levels.BiteUpgrade) {
                    if (inCombat) {
                        if (useIronJaws) {
                            return BRD.IronJaws;
                        }

                        if (level < BRD.Levels.IronJaws) {
                            if (venomous && venomousDuration.RemainingTime < 6)
                                return BRD.VenomousBite;
                            if (windbite && windbiteDuration.RemainingTime < 6)
                                return BRD.Windbite;
                        }

                        if (IsEnabled(CustomComboPreset.SimpleDoTOption)) {
                            if (level >= BRD.Levels.Windbite && !windbite)
                                return OriginalHook(BRD.Windbite);
                            if (level >= BRD.Levels.VenomousBite && !venomous)
                                return OriginalHook(BRD.VenomousBite);
                        }
                    }

                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                        return OriginalHook(BRD.RefulgentArrow);
                    }

                    return OriginalHook(BRD.BurstShot);
                }

                if (inCombat) {
                    if (useIronJaws) {
                        return BRD.IronJaws;
                    }

                    if (IsEnabled(CustomComboPreset.SimpleDoTOption)) {
                        if (level >= BRD.Levels.CausticBite && !caustic)
                            return BRD.CausticBite;
                        if (level >= BRD.Levels.StormBite && !stormbite)
                            return BRD.Stormbite;
                    }

                    if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100) {
                        return BRD.ApexArrow;
                    }
                }

                if (HasEffect(BRD.Buffs.StraightShotReady)) {
                    return OriginalHook(BRD.RefulgentArrow);
                }

                return OriginalHook(BRD.BurstShot);
            }

            return actionID;
        }
    }
    internal class BardApplyDots : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardApplyDots;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                var gauge = GetJobGauge<BRDGauge>();
                var heavyShot = GetCooldown(actionID);
                var heavyShotOnCooldown = heavyShot.CooldownRemaining > 0.7;

                if (IsEnabled(CustomComboPreset.BardApplyDots))
                {
                    if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100)
                        return BRD.ApexArrow;
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;

                    if (heavyShotOnCooldown)
                    {
                        if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                            return BRD.PitchPerfect;
                        if (level >= BRD.Levels.EmpyrealArrow && !GetCooldown(BRD.EmpyrealArrow).IsCooldown)
                            return BRD.EmpyrealArrow;
                        if (level >= BRD.Levels.Bloodletter && GetCooldown(BRD.Bloodletter).CooldownRemaining < 30)
                            return BRD.Bloodletter;
                        if (level >= BRD.Levels.Sidewinder && !GetCooldown(BRD.Sidewinder).IsCooldown)
                            return BRD.Sidewinder;
                    }
                }

                if (IsEnabled(CustomComboPreset.BardApplyDots))
                {
                    if (heavyShot.IsCooldown && (gauge.SongTimer < 3))
                    {
                        if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                            return BRD.ArmysPaeon;
                    }
                }


                var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                var useIronJaws = (
                    level >= BRD.Levels.IronJaws &&
                    ((venomous && venomousDuration.RemainingTime < 6) || (caustic && causticDuration.RemainingTime < 6)) ||
                    level >= BRD.Levels.IronJaws &&
                    ((windbite && windbiteDuration.RemainingTime < 6) || (stormbite && stormbiteDuration.RemainingTime < 6))
                );

                if (level < BRD.Levels.BiteUpgrade)
                {
                    if (useIronJaws)
                    {
                        return BRD.IronJaws;
                    }

                    if (level < BRD.Levels.IronJaws)
                    {
                        if (venomous && venomousDuration.RemainingTime < 6)
                            return BRD.VenomousBite;
                        if (windbite && windbiteDuration.RemainingTime < 6)
                            return BRD.Windbite;
                    }

                    if (IsEnabled(CustomComboPreset.BardApplyDots))
                    {
                        if (level >= BRD.Levels.Windbite && !windbite)
                            return OriginalHook(BRD.Windbite);
                        if (level >= BRD.Levels.VenomousBite && !venomous)
                            return OriginalHook(BRD.VenomousBite);
                    }

                    if (HasEffect(BRD.Buffs.StraightShotReady))
                    {
                        return OriginalHook(BRD.RefulgentArrow);
                    }

                    return OriginalHook(BRD.BurstShot);
                }

                if (useIronJaws)
                {
                    return BRD.IronJaws;
                }

                if (IsEnabled(CustomComboPreset.BardApplyDots))
                {
                    if (level >= BRD.Levels.CausticBite && !caustic)
                        return BRD.CausticBite;
                    if (level >= BRD.Levels.StormBite && !stormbite)
                        return BRD.Stormbite;
                }

                if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100)
                {
                    return BRD.ApexArrow;
                }

                if (HasEffect(BRD.Buffs.StraightShotReady))
                {
                    return OriginalHook(BRD.RefulgentArrow);
                }

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
            if (actionID == BRD.Barrage) {
                if (level >= BRD.Levels.RagingStrikes && !GetCooldown(BRD.RagingStrikes).IsCooldown)
                    return BRD.RagingStrikes;
                if (level >= BRD.Levels.BattleVoice && !GetCooldown(BRD.BattleVoice).IsCooldown)
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
            if (actionID == BRD.WanderersMinuet) { // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                    return BRD.WanderersMinuet;
                if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                    return BRD.MagesBallad;
                if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                    return BRD.ArmysPaeon;
            }

            return OriginalHook(actionID);
        }
    }
}
