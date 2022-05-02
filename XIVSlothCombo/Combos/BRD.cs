using System;
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
            Troubadour = 7405,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            BurstShot = 16495,
            ApexArrow = 16496,
            Shadowbite = 16494,
            Ladonsbite = 25783,
            BlastArrow = 25784,
            RadiantFinale = 25785;


        public static class Buffs
        {
            public const ushort
                StraightShotReady = 122,
                Troubadour = 1934,
                BlastArrowReady = 2692,
                ShadowbiteReady = 3002,
                WanderersMinuet = 865,
                MagesBallad = 135,
                ArmysPaeon = 137,
                RadiantFinale = 2722,
                BattleVoice = 141,
                Barrage = 128,
                RagingStrikes = 125;
        }

        public static class Debuffs
        {
            public const ushort
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
                Barrage = 38,
                BattleVoice = 50,
                PitchPerfect = 52,
                EmpyrealArrow = 54,
                IronJaws = 56,
                WanderersMinuet = 52,
                Sidewinder = 60,
                Troubadour = 62,
                CausticBite = 64,
                StormBite = 64,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                Shadowbite = 72,
                BurstShot = 76,
                ApexArrow = 80,
                Ladonsbite = 82,
                BlastArrow = 86,
                RadiantFinale = 90;
        }

        public static class Config
        {
            public const string
                RagingJawsRenewTime = "ragingJawsRenewTime",
                NoWasteHPPercentage = "noWasteHpPercentage";
        }

        internal static bool SongIsNotNone(Song value)
        {
            return value != Song.NONE;
        }

        internal static bool SongIsNone(Song value)
        {
            return value == Song.NONE;
        }

        internal static bool SongIsWandererMinuet(Song value)
        {
            return value == Song.WANDERER;
        }


        // Replace HS/BS with SS/RA when procced.
        internal class BardStraightShotUpgradeFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardStraightShotUpgradeFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HeavyShot || actionID == BurstShot)
                {
                    if (IsEnabled(CustomComboPreset.BardApexFeature))
                    {
                        var gauge = GetJobGauge<BRDGauge>();

                        if (gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                            return ApexArrow;
                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BardDoTMaintain))
                    {
                        var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                        var venomous = TargetHasEffect(Debuffs.VenomousBite);
                        var windbite = TargetHasEffect(Debuffs.Windbite);
                        var caustic = TargetHasEffect(Debuffs.CausticBite);
                        var stormbite = TargetHasEffect(Debuffs.Stormbite);
                        var venomousDuration = FindTargetEffect(Debuffs.VenomousBite);
                        var windbiteDuration = FindTargetEffect(Debuffs.Windbite);
                        var causticDuration = FindTargetEffect(Debuffs.CausticBite);
                        var stormbiteDuration = FindTargetEffect(Debuffs.Stormbite);

                        if (inCombat)
                        {
                            var useIronJaws = (
                                level >= Levels.IronJaws &&
                                ((venomous && venomousDuration.RemainingTime < 4) || (caustic && causticDuration.RemainingTime < 4)) ||
                                ((windbite && windbiteDuration.RemainingTime < 4) || (stormbite && stormbiteDuration.RemainingTime < 4))
                            );

                            if (useIronJaws)
                                return IronJaws;
                            if (level < Levels.IronJaws && venomous && venomousDuration.RemainingTime < 4)
                                return VenomousBite;
                            if (level < Levels.IronJaws && windbite && windbiteDuration.RemainingTime < 4)
                                return Windbite;
                        }

                    }

                    if (HasEffect(Buffs.StraightShotReady))
                    {
                        return (level >= Levels.RefulgentArrow) ? RefulgentArrow : StraightShot;
                    }
                }

                return actionID;
            }
        }

        internal class BardIronJawsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardIronJawsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == IronJaws)
                {
                    if (IsEnabled(CustomComboPreset.BardIronJawsApexFeature) && level >= Levels.ApexArrow)
                    {
                        var gauge = GetJobGauge<BRDGauge>();

                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady)) return BlastArrow;
                        if (gauge.SoulVoice == 100 && IsOffCooldown(ApexArrow)) return ApexArrow;
                    }


                    if (level < Levels.IronJaws)
                    {
                        var venomous = FindTargetEffect(Debuffs.VenomousBite);
                        var windbite = FindTargetEffect(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            if (level >= Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                            {
                                return VenomousBite;
                            }

                            if (level >= Levels.Windbite)
                            {
                                return Windbite;
                            }
                        }

                        if (level >= Levels.VenomousBite && (level < Levels.Windbite || windbite is not null))
                        {
                            return VenomousBite;
                        }

                        if (level >= Levels.Windbite)
                        {
                            return Windbite;
                        }
                    }

                    if (level < Levels.BiteUpgrade)
                    {
                        var venomous = TargetHasEffect(Debuffs.VenomousBite);
                        var windbite = TargetHasEffect(Debuffs.Windbite);
                        var venomousDuration = FindTargetEffect(Debuffs.VenomousBite);
                        var windbiteDuration = FindTargetEffect(Debuffs.Windbite);

                        if (level >= Levels.IronJaws && venomous && windbite)
                        {
                            return IronJaws;
                        }

                        if (level >= Levels.VenomousBite && windbite)
                        {
                            return VenomousBite;
                        }

                        if (level >= Levels.Windbite)
                        {
                            return Windbite;
                        }
                    }

                    var caustic = TargetHasEffect(Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(Debuffs.Stormbite);
                    var causticDuration = FindTargetEffect(Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(Debuffs.Stormbite);

                    if (level >= Levels.IronJaws && caustic && stormbite)
                    {
                        return IronJaws;
                    }

                    if (level >= Levels.CausticBite && stormbite)
                    {
                        return CausticBite;
                    }

                    if (level >= Levels.StormBite)
                    {
                        return Stormbite;
                    }
                }

                return actionID;
            }
        }
        internal class BardIronJawsAlternateFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardIronJawsAlternateFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == IronJaws)
                {
                    if (level < Levels.IronJaws)
                    {
                        var venomous = FindTargetEffect(Debuffs.VenomousBite);
                        var windbite = FindTargetEffect(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            if (level >= Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                            {
                                return VenomousBite;
                            }

                            if (level >= Levels.Windbite)
                            {
                                return Windbite;
                            }
                        }

                        if (level >= Levels.VenomousBite && (level < Levels.Windbite || windbite is not null))
                        {
                            return VenomousBite;
                        }

                        if (level >= Levels.Windbite)
                        {
                            return Windbite;
                        }
                    }

                    if (level < Levels.BiteUpgrade)
                    {
                        var venomous = TargetHasEffect(Debuffs.VenomousBite);
                        var windbite = TargetHasEffect(Debuffs.Windbite);
                        var venomousDuration = FindTargetEffect(Debuffs.VenomousBite);
                        var windbiteDuration = FindTargetEffect(Debuffs.Windbite);

                        if (level >= Levels.IronJaws && venomous && windbite && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4))
                        {
                            return IronJaws;
                        }

                        if (level >= Levels.VenomousBite && windbite)
                        {
                            return VenomousBite;
                        }

                        if (level >= Levels.Windbite)
                        {
                            return Windbite;
                        }
                    }

                    var caustic = TargetHasEffect(Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(Debuffs.Stormbite);
                    var causticDuration = FindTargetEffect(Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(Debuffs.Stormbite);

                    if (level >= Levels.IronJaws && caustic && stormbite && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4))
                    {
                        return IronJaws;
                    }

                    if (level >= Levels.CausticBite && stormbite)
                    {
                        return CausticBite;
                    }

                    if (level >= Levels.StormBite)
                    {
                        return Stormbite;
                    }
                }

                return actionID;
            }
        }

        internal class BardApexFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardApexFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == QuickNock)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (level >= Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return ApexArrow;
                }

                return actionID;
            }
        }

        internal class BardoGCDAoEFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDAoEFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == RainOfDeath)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (level >= Levels.WanderersMinuet && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow))
                        return EmpyrealArrow;
                    if (level >= Levels.Bloodletter && IsOffCooldown(Bloodletter))
                        return RainOfDeath;
                    if (level >= Levels.Sidewinder && IsOffCooldown(Sidewinder))
                        return Sidewinder;

                }

                return actionID;
            }
        }

        internal class BardSimpleAoEFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardSimpleAoEFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Ladonsbite || actionID == QuickNock)
                {
                    var gauge = GetJobGauge<BRDGauge>();
                    var soulVoice = gauge.SoulVoice;
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.SimpleAoESongOption) && canWeave)
                    {
                        var songTimerInSeconds = gauge.SongTimer / 1000;

                        if (songTimerInSeconds < 3 || gauge.Song == Song.NONE)
                        {
                            if (level >= Levels.WanderersMinuet &&
                                IsOffCooldown(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)) && !IsEnabled(CustomComboPreset.SimpleAoESongOptionExcludeWM))
                                return WanderersMinuet;
                            if (level >= Levels.MagesBallad &&
                                IsOffCooldown(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;
                            if (level >= Levels.ArmysPaeon &&
                                IsOffCooldown(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }
                    }

                    if (canWeave)
                    {
                        if (level >= Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                            return OriginalHook(WanderersMinuet);
                        if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow))
                            return EmpyrealArrow;
                        if (level >= Levels.RainOfDeath && GetRemainingCharges(RainOfDeath) > 0)
                            return RainOfDeath;
                        if (level >= Levels.Sidewinder && IsOffCooldown(Sidewinder))
                            return Sidewinder;
                    }

                    if (level >= Levels.Shadowbite && HasEffect(Buffs.ShadowbiteReady))
                        return Shadowbite;
                    if (level >= Levels.ApexArrow && soulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return ApexArrow;
                    if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                        return BlastArrow;

                }

                return actionID;
            }
        }

        internal class BardoGCDSingleTargetFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDSingleTargetFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bloodletter)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (IsEnabled(CustomComboPreset.BardSongsFeature) && (gauge.SongTimer < 1 || gauge.Song == Song.ARMY))
                    {
                        if (level >= Levels.WanderersMinuet && IsOffCooldown(WanderersMinuet))
                            return WanderersMinuet;
                        if (level >= Levels.MagesBallad && IsOffCooldown(MagesBallad))
                            return MagesBallad;
                        if (level >= Levels.ArmysPaeon && IsOffCooldown(ArmysPaeon))
                            return ArmysPaeon;
                    }

                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow))
                        return EmpyrealArrow;
                    if (level >= Levels.Bloodletter && IsOffCooldown(Bloodletter))
                        return Bloodletter;
                    if (level >= Levels.Sidewinder && IsOffCooldown(Sidewinder))
                        return Sidewinder;
                }

                return actionID;
            }
        }
        internal class BardAoEComboFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardAoEComboFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == QuickNock || actionID == Ladonsbite)
                {
                    if (IsEnabled(CustomComboPreset.BardApexFeature))
                    {
                        if (level >= Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                            return ApexArrow;

                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BardAoEComboFeature) && level >= Levels.Shadowbite && HasEffectAny(Buffs.ShadowbiteReady))
                    {
                        return Shadowbite;
                    }
                }

                return actionID;
            }
        }
        internal class SimpleBardFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleBardFeature;
            internal static bool inOpener = false;
            internal static bool openerFinished = false;
            internal static byte step = 0;
            internal static byte subStep = 0;

            internal static bool usedStraightShotReady = false;
            internal static bool usedPitchPerfect = false;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HeavyShot || actionID == BurstShot)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<BRDGauge>();

                    var canWeave = CanWeave(actionID);
                    var canWeaveBuffs = CanWeave(actionID, 0.6);
                    var canWeaveDelayed = CanDelayedWeave(actionID);

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        openerFinished = false;
                    }

                    if (IsEnabled(CustomComboPreset.BardSimpleInterrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    var isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BardSimpleNoWasteMode) || EnemyHealthPercentage() > Service.Configuration.GetCustomIntValue(Config.NoWasteHPPercentage);

                    if (IsEnabled(CustomComboPreset.SimpleSongOption) && canWeave && isEnemyHealthHigh)
                    {
                        var songTimerInSeconds = gauge.SongTimer / 1000;

                        // Limit optimisation to only when you are high enough to benefit from it.
                        if (level >= Levels.WanderersMinuet)
                        {
                            // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army Peon    
                            var minuetOffCooldown = IsOffCooldown(WanderersMinuet);
                            var balladOffCooldown = IsOffCooldown(MagesBallad);
                            var paeonOffCooldown = IsOffCooldown(ArmysPaeon);

                            if (gauge.Song == Song.NONE)
                            {
                                // Do logic to determine first song

                                if (minuetOffCooldown && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon))) return WanderersMinuet;
                                if (balladOffCooldown && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon))) return MagesBallad;
                                if (paeonOffCooldown && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet))) return ArmysPaeon;
                            }

                            if (gauge.Song == Song.WANDERER)
                            {
                                // Spend any repertoire before switching to next song
                                if (songTimerInSeconds < 3 && gauge.Repertoire > 0)
                                {
                                    return OriginalHook(WanderersMinuet);
                                }
                                // Move to Mage's Ballad if < 3 seconds left on song
                                if (songTimerInSeconds < 3 && balladOffCooldown)
                                {
                                    return MagesBallad;
                                }
                            }

                            if (gauge.Song == Song.MAGE)
                            {
                                // Move to Army's Paeon if < 12 seconds left on song
                                if (songTimerInSeconds < 12 && paeonOffCooldown)
                                {
                                    // Very special case for Empyreal, it needs to be cast before you change to it to avoid drift!!!
                                    if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow))
                                        return EmpyrealArrow;

                                    return ArmysPaeon;
                                }
                            }

                            if (gauge.Song == Song.ARMY)
                            {
                                // Move to Wanderer's Minuet if < 3 seconds left on song or WM off CD
                                if (songTimerInSeconds < 3 || minuetOffCooldown)
                                {
                                    return WanderersMinuet;
                                }
                            }
                        }
                        else if (songTimerInSeconds < 3)
                        {
                            if (level >= Levels.MagesBallad && IsOffCooldown(MagesBallad))
                                return MagesBallad;
                            if (level >= Levels.ArmysPaeon && IsOffCooldown(ArmysPaeon))
                                return ArmysPaeon;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.BardSimpleBuffsFeature) && gauge.Song != Song.NONE && isEnemyHealthHigh)
                    {
                        if (canWeaveDelayed && level >= Levels.RagingStrikes && IsOffCooldown(RagingStrikes) &&
                            (GetCooldown(BattleVoice).CooldownRemaining < 4.5 || IsOffCooldown(BattleVoice)))
                        {
                            return RagingStrikes;
                        }
                        if (IsEnabled(CustomComboPreset.BardSimpleBuffsRadiantFeature) && level >= Levels.RadiantFinale && canWeaveBuffs &&
                            IsOffCooldown(RadiantFinale) && (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)) &&
                            (IsOffCooldown(BattleVoice) || GetCooldownRemainingTime(BattleVoice) < 0.5) && (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16 || openerFinished))
                        {
                            if (!JustUsed(RagingStrikes)) return RadiantFinale;
                        }

                        if (canWeaveBuffs && level >= Levels.BattleVoice && IsOffCooldown(BattleVoice) && (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16 || openerFinished))
                        {
                            if (!JustUsed(RagingStrikes)) return BattleVoice;
                        }
                        if (canWeaveBuffs && level >= Levels.Barrage && IsOffCooldown(Barrage) && !HasEffect(Buffs.StraightShotReady) && HasEffect(Buffs.RagingStrikes))
                        {
                            if (level >= Levels.RadiantFinale && HasEffect(Buffs.RadiantFinale))
                                return Barrage;
                            else if (level >= Levels.BattleVoice && HasEffect(Buffs.BattleVoice))
                                return Barrage;
                            else if (level < Levels.BattleVoice && HasEffect(Buffs.RagingStrikes))
                                return Barrage;
                        }
                    }


                    if (canWeave)
                    {
                        var bvProtection = IsOffCooldown(BattleVoice) || GetCooldownRemainingTime(BattleVoice) > 2.5;

                        if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow) && bvProtection)

                            return EmpyrealArrow;

                        if (level >= Levels.PitchPerfect && gauge.Song == Song.WANDERER &&
                            (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && GetCooldown(EmpyrealArrow).CooldownRemaining < 2)) && bvProtection)

                            return OriginalHook(WanderersMinuet);

                        if (level >= Levels.Sidewinder && IsOffCooldown(Sidewinder) && bvProtection)
                            if (IsEnabled(CustomComboPreset.BardSimplePooling))
                            {
                                if (gauge.Song == Song.WANDERER)
                                {
                                    if (
                                        (HasEffect(Buffs.RagingStrikes) || GetCooldown(RagingStrikes).CooldownRemaining > 10) &&
                                        (HasEffect(Buffs.BattleVoice) || GetCooldown(BattleVoice).CooldownRemaining > 10) &&
                                        (
                                            HasEffect(Buffs.RadiantFinale) || GetCooldown(RadiantFinale).CooldownRemaining > 10 ||
                                            level < Levels.RadiantFinale
                                        )
                                        )
                                    {
                                        return Sidewinder;
                                    }
                                }
                                else return Sidewinder;
                            }
                            else return Sidewinder;
                        if (level >= Levels.Bloodletter)
                        {
                            var bloodletterCharges = GetRemainingCharges(Bloodletter);

                            if (IsEnabled(CustomComboPreset.BardSimplePooling) && level >= Levels.WanderersMinuet)
                            {
                                if (gauge.Song == Song.WANDERER)
                                {
                                    if (
                                        ((HasEffect(Buffs.RagingStrikes) || GetCooldown(RagingStrikes).CooldownRemaining > 10) &&
                                        (
                                            HasEffect(Buffs.BattleVoice) || GetCooldown(BattleVoice).CooldownRemaining > 10 ||
                                            level < Levels.BattleVoice
                                        ) &&
                                        (
                                            HasEffect(Buffs.RadiantFinale) || GetCooldown(RadiantFinale).CooldownRemaining > 10 ||
                                            level < Levels.RadiantFinale
                                        ) &&
                                        bloodletterCharges > 0) ||
                                        (bloodletterCharges > 2 && bvProtection)
                                    )
                                    {
                                        return Bloodletter;
                                    }
                                }
                                if (gauge.Song == Song.ARMY && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0))) return Bloodletter;
                                if (gauge.Song == Song.MAGE && bloodletterCharges > 0) return Bloodletter;
                                if (gauge.Song == Song.NONE && bloodletterCharges == 3) return Bloodletter;
                            }
                            else if (bloodletterCharges > 0)
                            {
                                return Bloodletter;
                            }
                        }
                    }


                    if (isEnemyHealthHigh)
                    {
                        var venomous = TargetHasEffect(Debuffs.VenomousBite);
                        var windbite = TargetHasEffect(Debuffs.Windbite);
                        var caustic = TargetHasEffect(Debuffs.CausticBite);
                        var stormbite = TargetHasEffect(Debuffs.Stormbite);

                        var venomousDuration = FindTargetEffect(Debuffs.VenomousBite);
                        var windbiteDuration = FindTargetEffect(Debuffs.Windbite);
                        var causticDuration = FindTargetEffect(Debuffs.CausticBite);
                        var stormbiteDuration = FindTargetEffect(Debuffs.Stormbite);

                        var ragingStrikesDuration = FindEffect(Buffs.RagingStrikes);

                        var ragingJawsRenewTime = Service.Configuration.GetCustomIntValue(Config.RagingJawsRenewTime);

                        DotRecast poisonRecast = delegate (int duration)
                        {
                            return (venomous && venomousDuration.RemainingTime < duration) || (caustic && causticDuration.RemainingTime < duration);
                        };
                        DotRecast windRecast = delegate (int duration)
                        {
                            return (windbite && windbiteDuration.RemainingTime < duration) || (stormbite && stormbiteDuration.RemainingTime < duration);
                        };

                        var useIronJaws = (
                            (level >= Levels.IronJaws && poisonRecast(4)) ||
                            (level >= Levels.IronJaws && windRecast(4)) ||
                            (level >= Levels.IronJaws && IsEnabled(CustomComboPreset.BardSimpleRagingJaws) &&
                                HasEffect(Buffs.RagingStrikes) && ragingStrikesDuration.RemainingTime < ragingJawsRenewTime &&
                                poisonRecast(40) && windRecast(40))
                        );

                        var dotOpener = (IsEnabled(CustomComboPreset.BardSimpleDotOpener) && !openerFinished || !IsEnabled(CustomComboPreset.BardSimpleDotOpener));

                        if (level < Levels.BiteUpgrade)
                        {
                            if (useIronJaws)
                            {
                                openerFinished = true;
                                return IronJaws;
                            }

                            if (level < Levels.IronJaws)
                            {
                                if (windbite && windbiteDuration.RemainingTime < 4)
                                {
                                    openerFinished = true;
                                    return Windbite;
                                }
                                if (venomous && venomousDuration.RemainingTime < 4)
                                {
                                    openerFinished = true;
                                    return VenomousBite;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                            {
                                if (level >= Levels.Windbite && !windbite && dotOpener)
                                    return Windbite;
                                if (level >= Levels.VenomousBite && !venomous && dotOpener)
                                    return VenomousBite;
                            }
                        }
                        else
                        {

                            if (useIronJaws)
                            {
                                openerFinished = true;
                                return IronJaws;
                            }

                            if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                            {
                                if (level >= Levels.StormBite && !stormbite && dotOpener)
                                    return Stormbite;
                                if (level >= Levels.CausticBite && !caustic && dotOpener)
                                    return CausticBite;

                            }
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    {
                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                        if (level >= Levels.ApexArrow)
                        {
                            var songTimerInSeconds = gauge.SongTimer / 1000;

                            if (gauge.Song == Song.MAGE && gauge.SoulVoice == 100) return ApexArrow;
                            if (gauge.Song == Song.MAGE && gauge.SoulVoice >= 80 && songTimerInSeconds > 18 && songTimerInSeconds < 22) return ApexArrow;
                            if (gauge.Song == Song.WANDERER && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                                (HasEffect(Buffs.RadiantFinale) || level < Levels.RadiantFinale) && gauge.SoulVoice >= 80) return ApexArrow;
                        }
                    }

                    if (HasEffect(Buffs.StraightShotReady))
                    {
                        return (level >= Levels.RefulgentArrow) ? RefulgentArrow : StraightShot;
                    }

                }

                return actionID;
            }
        }
        internal class BardBuffsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardBuffsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Barrage)
                {
                    if (level >= Levels.RagingStrikes && IsOffCooldown(RagingStrikes))
                        return RagingStrikes;
                    if (level >= Levels.BattleVoice && IsOffCooldown(BattleVoice))
                        return BattleVoice;
                }

                return actionID;
            }
        }
        internal class BardOneButtonSongs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardOneButtonSongs;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WanderersMinuet)
                { // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                    if (level >= Levels.WanderersMinuet && IsOffCooldown(WanderersMinuet))
                        return WanderersMinuet;
                    if (level >= Levels.MagesBallad && IsOffCooldown(MagesBallad))
                        return MagesBallad;
                    if (level >= Levels.ArmysPaeon && IsOffCooldown(ArmysPaeon))
                        return ArmysPaeon;
                }

                return actionID;
            }
        }
    }
}
