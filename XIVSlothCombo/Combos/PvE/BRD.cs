using System;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
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
                BRD_RagingJawsRenewTime = "ragingJawsRenewTime",
                BRD_NoWasteHPPercentage = "noWasteHpPercentage";
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
        internal class BRD_StraightShotUpgrade : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == HeavyShot || actionID == BurstShot)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        var gauge = GetJobGauge<BRDGauge>();

                        if (gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                            return ApexArrow;
                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance))
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

        internal class BRD_IronJaws : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == IronJaws)
                {
                    if (IsEnabled(CustomComboPreset.BRD_IronJawsApex) && level >= Levels.ApexArrow)
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
        internal class BRD_IronJaws_Alternate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws_Alternate;

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

        internal class BRD_Apex : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Apex;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == QuickNock)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (level >= Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                        return ApexArrow;
                }

                return actionID;
            }
        }

        internal class BRD_AoE_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_oGCD;

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

        internal class BRD_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Ladonsbite || actionID == QuickNock)
                {
                    var gauge = GetJobGauge<BRDGauge>();
                    var soulVoice = gauge.SoulVoice;
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Simple_Songs) && canWeave)
                    {
                        var songTimerInSeconds = gauge.SongTimer / 1000;

                        if (songTimerInSeconds < 3 || gauge.Song == Song.NONE)
                        {
                            if (level >= Levels.WanderersMinuet &&
                                IsOffCooldown(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)) && !IsEnabled(CustomComboPreset.BRD_AoE_Simple_SongsExcludeWM))
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
                    if (level >= Levels.ApexArrow && soulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                        return ApexArrow;
                    if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                        return BlastArrow;

                }

                return actionID;
            }
        }

        internal class BRD_ST_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Bloodletter)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (IsEnabled(CustomComboPreset.BRD_oGCDSongs) && (gauge.SongTimer < 1 || gauge.Song == Song.ARMY))
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
        internal class BRD_AoE_Combo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == QuickNock || actionID == Ladonsbite)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        if (level >= Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                            return ApexArrow;

                        if (level >= Levels.BlastArrow && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Combo) && level >= Levels.Shadowbite && HasEffectAny(Buffs.ShadowbiteReady))
                    {
                        return Shadowbite;
                    }
                }

                return actionID;
            }
        }
        internal class BRD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_SimpleMode;
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
                    var canWeaveDelayed = CanDelayedWeave(actionID, 0.9);

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        openerFinished = false;
                    }

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                    {
                        return All.HeadGraze;
                    }

                    var isEnemyHealthHigh = IsEnabled(CustomComboPreset.BRD_Simple_NoWaste) ?
                        GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.BRD_NoWasteHPPercentage) : true;

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Song) && isEnemyHealthHigh)
                    {
                        var songTimerInSeconds = gauge.SongTimer / 1000;

                        // Limit optimisation to only when you are high enough to benefit from it.
                        if (level >= Levels.WanderersMinuet)
                        {
                            // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army Peon    
                            var minuetOffCooldown = IsOffCooldown(WanderersMinuet);
                            var balladOffCooldown = IsOffCooldown(MagesBallad);
                            var paeonOffCooldown = IsOffCooldown(ArmysPaeon);

                            if (gauge.Song == Song.NONE && canWeave)
                            {
                                // Do logic to determine first song

                                if (minuetOffCooldown && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon))) return WanderersMinuet;
                                if (balladOffCooldown && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon))) return MagesBallad;
                                if (paeonOffCooldown && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet))) return ArmysPaeon;
                            }

                            if (gauge.Song == Song.WANDERER && canWeave)
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

                            if (gauge.Song == Song.MAGE && canWeave)
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

                            if (gauge.Song == Song.ARMY && canWeaveDelayed)
                            {
                                // Move to Wanderer's Minuet if < 3 seconds left on song or WM off CD and have 4 repertoires of AP
                                if (songTimerInSeconds < 3 || (minuetOffCooldown && gauge.Repertoire == 4))
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

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Buffs) && (gauge.Song != Song.NONE || level < Levels.MagesBallad) && isEnemyHealthHigh)
                    {
                        if (((canWeaveBuffs && CombatEngageDuration().Minutes == 0) || (canWeaveDelayed && CombatEngageDuration().Minutes > 0)) && level >= Levels.RagingStrikes && IsOffCooldown(RagingStrikes) &&
                            (GetCooldown(BattleVoice).CooldownRemaining <= 5.38 || IsOffCooldown(BattleVoice) || level < Levels.BattleVoice))
                        {
                            return RagingStrikes;
                        }
                        if (canWeaveBuffs && IsEnabled(CustomComboPreset.BRD_Simple_BuffsRadiant) && level >= Levels.RadiantFinale &&
                            IsOffCooldown(RadiantFinale) && (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)) &&
                            (IsOffCooldown(BattleVoice) || GetCooldownRemainingTime(BattleVoice) < 0.7) && (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
                        {
                            if (!JustUsed(RagingStrikes)) return RadiantFinale;
                        }

                        if (canWeaveBuffs && level >= Levels.BattleVoice && IsOffCooldown(BattleVoice) && (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
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
                        if (level >= Levels.EmpyrealArrow && IsOffCooldown(EmpyrealArrow) &&
                            (!openerFinished || (openerFinished && GetCooldownRemainingTime(BattleVoice) >= 3.5)))
                        {
                            return EmpyrealArrow;
                        }

                        if (level >= Levels.PitchPerfect && gauge.Song == Song.WANDERER &&
                            (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && GetCooldown(EmpyrealArrow).CooldownRemaining < 2)) &&
                            (!openerFinished || (openerFinished && GetCooldownRemainingTime(BattleVoice) >= 3.5)))
                        {
                            return OriginalHook(WanderersMinuet);
                        }

                        if (level >= Levels.Sidewinder && IsOffCooldown(Sidewinder) &&
                            (!openerFinished || (openerFinished && GetCooldownRemainingTime(BattleVoice) >= 3.5)))
                        {
                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling))
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
                        }

                        if (level >= Levels.Bloodletter)
                        {
                            var bloodletterCharges = GetRemainingCharges(Bloodletter);

                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling) && level >= Levels.WanderersMinuet)
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
                                        bloodletterCharges > 2
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

                        var ragingStrikesDuration = GetBuffRemainingTime(Buffs.RagingStrikes);

                        var ragingJawsRenewTime = PluginConfiguration.GetCustomIntValue(Config.BRD_RagingJawsRenewTime);

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
                            (level >= Levels.IronJaws && IsEnabled(CustomComboPreset.BRD_Simple_RagingJaws) &&
                                HasEffect(Buffs.RagingStrikes) && ragingStrikesDuration < ragingJawsRenewTime &&
                                poisonRecast(40) && windRecast(40))
                        );

                        var dotOpener = (IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener) && !openerFinished || !IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener));

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

                            if (IsEnabled(CustomComboPreset.BRD_Simple_DoT))
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

                            if (IsEnabled(CustomComboPreset.BRD_Simple_DoT))
                            {
                                if (level >= Levels.StormBite && !stormbite && dotOpener)
                                    return Stormbite;
                                if (level >= Levels.CausticBite && !caustic && dotOpener)
                                    return CausticBite;

                            }
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
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
        internal class BRD_Buffs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Buffs;

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
        internal class BRD_OneButtonSongs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_OneButtonSongs;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WanderersMinuet)
                { // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.

                    var gauge = GetJobGauge<BRDGauge>();
                    var songTimerInSeconds = gauge.SongTimer / 1000;

                    bool canUse = (gauge.Song != Song.WANDERER || songTimerInSeconds < 3) && !JustUsed(WanderersMinuet);

                    if (level >= Levels.WanderersMinuet && IsOffCooldown(WanderersMinuet))
                        return WanderersMinuet;
                    
                    if (level >= Levels.MagesBallad && IsOffCooldown(MagesBallad) && canUse)
                    {
                        if (gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                            return OriginalHook(WanderersMinuet);

                        return MagesBallad;
                    }
                    
                    if (level >= Levels.ArmysPaeon && IsOffCooldown(ArmysPaeon) && canUse)
                    {
                        if (gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                            return OriginalHook(WanderersMinuet);

                        return ArmysPaeon;
                    }
                }

                return actionID;
            }
        }
    }
}
