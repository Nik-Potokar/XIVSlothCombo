using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
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
            BlastArrow = 25784,
            HeadGraze = 7551,
            RadiantFinale = 25785;
        

        public static class Buffs
        {
            public const short
                StraightShotReady = 122,
                BlastArrowReady = 2692,
                ShadowbiteReady = 3002,
                WanderersMinuet = 865,
                MagesBallad = 135,
                ArmysPaeon = 137,
                RadiantFinale = 2722,
                BattleVoice = 141,
                Barrage = 128;
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
                Barrage = 38,
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
                BlastArrow = 86,
                RadiantFinale = 90,
                HeadGraze = 24;
        }

        internal static bool SongIsNotNone(Song value) {
            return value != Song.NONE;
        }
    }

    // Replace Wanderer's Minuet with PP when in WM.
    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardWanderersPitchPerfectFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet) {
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
                if (IsEnabled(CustomComboPreset.BardApexFeature)) {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardDoTMaintain)) {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                    if (inCombat) {
                        var useIronJaws = (
                            level >= BRD.Levels.IronJaws &&
                            ((venomous && venomousDuration.RemainingTime < 4) || (caustic && causticDuration.RemainingTime < 4)) ||
                            ((windbite && windbiteDuration.RemainingTime < 4) || (stormbite && stormbiteDuration.RemainingTime < 4))
                        );

                        if (useIronJaws)
                            return BRD.IronJaws;
                        if (level < BRD.Levels.IronJaws && venomous && venomousDuration.RemainingTime < 4)
                            return BRD.VenomousBite;
                        if (level < BRD.Levels.IronJaws && windbite && windbiteDuration.RemainingTime < 4)
                            return BRD.Windbite;
                    }

                }

                if (HasEffect(BRD.Buffs.StraightShotReady)) {
                    return OriginalHook(BRD.RefulgentArrow);
                }
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws) {
                if (level < BRD.Levels.IronJaws) {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null) {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime) {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite) {
                            return BRD.Windbite;
                        }
                    }
                    
                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null)) {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite) {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade) {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite) {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite) {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite) {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite) {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite) {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite) {
                    return BRD.Stormbite;
                }
            }

            return actionID;
        }
    }
    internal class BardIronJawsAlternateFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsAlternateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws) {
                if (level < BRD.Levels.IronJaws) {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null) {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime) {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite) {
                            return BRD.Windbite;
                        }
                    }

                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null)) {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite) {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade) {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4)) {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite) {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite) {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4)) {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite) {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite) {
                    return BRD.Stormbite;
                }
            }

            return actionID;
        }
    }

    internal class BardApexFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardApexFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock) {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
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
            if (actionID == BRD.RainOfDeath) {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.WanderersMinuet && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (level >= BRD.Levels.EmpyrealArrow && !GetCooldown(BRD.EmpyrealArrow).IsCooldown)
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && !GetCooldown(BRD.Bloodletter).IsCooldown)
                    return BRD.RainOfDeath;
                if (level >= BRD.Levels.Sidewinder && !GetCooldown(BRD.Sidewinder).IsCooldown)
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
                if (level >= BRD.Levels.ApexArrow && soulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    return BRD.ApexArrow;
                if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;

                if (IsEnabled(CustomComboPreset.SimpleAoESongOption) && heavyShotOnCooldown) {
                    if ((gauge.SongTimer < 1 || gauge.Song == Song.ARMY) && GetCooldown(actionID).IsCooldown) {
                        if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
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
            if (actionID == BRD.Bloodletter) {
                var gauge = GetJobGauge<BRDGauge>();

                if (IsEnabled(CustomComboPreset.BardSongsFeature) && (gauge.SongTimer < 1 || gauge.Song == Song.ARMY)) {
                    if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                        return BRD.WanderersMinuet;
                    if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                        return BRD.MagesBallad;
                    if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                        return BRD.ArmysPaeon;
                }

                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (level >= BRD.Levels.EmpyrealArrow && !GetCooldown(BRD.EmpyrealArrow).IsCooldown)
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && !GetCooldown(BRD.Bloodletter).IsCooldown)
                    return BRD.Bloodletter;
                if (level >= BRD.Levels.Sidewinder && !GetCooldown(BRD.Sidewinder).IsCooldown)
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
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite) {
                if (IsEnabled(CustomComboPreset.BardApexFeature)) {
                    if (level >= BRD.Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardAoEComboFeature) && level >= BRD.Levels.Shadowbite && HasEffectAny(BRD.Buffs.ShadowbiteReady)) {
                    return BRD.Shadowbite;
                }
            }

            return actionID;
        }
    }
    internal class SimpleBardFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SimpleBardFeature;
        internal bool inOpener = false;
        internal bool openerFinished = false;
        internal byte step = 0;
        internal byte subStep = 0;
        internal bool usedStraightShotReady = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot) {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);


                if (IsEnabled(CustomComboPreset.BardSimpleOpener) && level >= 90) {
                    if (inCombat && lastComboMove == BRD.Stormbite && !inOpener) {
                        inOpener = true;
                    }

                    if (!inOpener) {
                        return BRD.Stormbite;
                    }

                    if (!inCombat && (inOpener || openerFinished)) {
                        inOpener = false;
                        step = 0;
                        subStep = 0;
                        usedStraightShotReady = false;
                        openerFinished = false;

                        return BRD.Stormbite;
                    }

                    if (inCombat && inOpener && !openerFinished) {
                        // Stormbite
                        // Minuet
                        // Raging Strikes
                        // Caustic Bite
                        // Empyreal Arrow
                        // Bloodletter
                        // Burst shot OR Refulgent Arrow
                        // Radiant Finale
                        // Battle Voice
                        // Burst shot OR Refulgent Arrow
                        if (step == 0) {
                            // Do this in steps, by using the ifs as the step changer...
                            if (subStep == 0) {
                                if (GetCooldown(BRD.WanderersMinuet).IsCooldown && !GetCooldown(BRD.RagingStrikes).IsCooldown) subStep++;
                                else return BRD.WanderersMinuet;
                            }
                            if (subStep == 1) {
                                if (GetCooldown(BRD.RagingStrikes).IsCooldown && !TargetHasEffect(BRD.Debuffs.CausticBite)) subStep++;
                                else return BRD.RagingStrikes;
                            }
                            if (subStep == 2) {
                                if (lastComboMove == BRD.CausticBite) subStep++;
                                else return BRD.CausticBite;
                            }
                            if (subStep == 3) {
                                if (lastComboMove == BRD.EmpyrealArrow && GetCooldown(BRD.Bloodletter).CooldownRemaining == 0) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 4) {
                                if (!HasEffect(BRD.Buffs.BattleVoice) && GetCooldown(BRD.Bloodletter).CooldownRemaining > 40 && (lastComboMove != BRD.RefulgentArrow && lastComboMove != BRD.BurstShot)) subStep++;
                                else return BRD.Bloodletter;
                            }
                            if (subStep == 5) {
                                if ((usedStraightShotReady && !HasEffect(BRD.Buffs.StraightShotReady)) || lastComboMove == BRD.BurstShot) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        usedStraightShotReady = true;
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 6) {
                                usedStraightShotReady = false;

                                if (HasEffect(BRD.Buffs.RadiantFinale)) subStep++;
                                else return BRD.RadiantFinale;
                            }
                            if (subStep == 7) {
                                if (HasEffect(BRD.Buffs.BattleVoice)) subStep++;
                                else return BRD.BattleVoice;
                            }
                            if (subStep == 8) {
                                if ((usedStraightShotReady && !HasEffect(BRD.Buffs.StraightShotReady)) || lastComboMove == BRD.BurstShot) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        usedStraightShotReady = true;
                                        return BRD.RefulgentArrow;
                                    } else return BRD.BurstShot;
                                }
                            }

                            if (HasEffect(BRD.Buffs.StraightShotReady)) step = 1;
                            else step = 2;

                            usedStraightShotReady = false;
                            subStep = 0;
                        }

                        // if Straight Shot Ready
                        // -- yes
                        // Sidewinder
                        // Refulgent Arrow
                        // Barrage
                        // Refulgent Arrow
                        // Burst Shot
                        // Burst shot OR Refulgent Arrow
                        // Empyreal Arrow
                        // Iron Jaws
                        if (step == 1) {
                            if (subStep == 0) {
                                if (GetCooldown(BRD.Sidewinder).IsCooldown) subStep++;
                                else return BRD.Sidewinder;
                            }
                            if (subStep == 1) {
                                if (!HasEffect(BRD.Buffs.StraightShotReady)) subStep++;
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 2) {
                                if (HasEffect(BRD.Buffs.Barrage)) subStep++;
                                else return BRD.Barrage;
                            }
                            if (subStep == 3) {
                                if (!HasEffect(BRD.Buffs.StraightShotReady)) subStep++;
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 4) {
                                if (lastComboMove == BRD.BurstShot) subStep++;
                                else return BRD.BurstShot;
                            }
                            if (subStep == 5) {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 4) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        return BRD.RefulgentArrow;
                                    } else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 6) {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 1) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        return BRD.RefulgentArrow;
                                    } else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 7) {
                                if (lastComboMove == BRD.EmpyrealArrow) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 8) {
                                if (FindTargetEffect(BRD.Debuffs.Stormbite).RemainingTime < 10) return BRD.IronJaws;
                            }
                            openerFinished = true;
                        }

                        // -- no
                        // Barrage
                        // Refulgent Arrow
                        // Sidewinder
                        // Burst Shot
                        // Burst Shot OR Refulgent Arrow
                        // Burst Shot OR Refulgent Arrow
                        // Empyreal Arrow
                        // Iron Jaws
                        if (step == 2) {
                            if (subStep == 0) {
                                if (HasEffect(BRD.Buffs.Barrage)) subStep++;
                                else return BRD.Barrage;
                            }
                            if (subStep == 1) {
                                if (!HasEffect(BRD.Buffs.StraightShotReady)) subStep++;
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 2) {
                                if (GetCooldown(BRD.Sidewinder).IsCooldown) subStep++;
                                else return BRD.Sidewinder;
                            }
                            if (subStep == 3) {
                                if (lastComboMove == BRD.BurstShot) subStep++;
                                else return BRD.BurstShot;
                            }
                            if (subStep == 4) {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 4) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        return BRD.RefulgentArrow;
                                    } else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 5) {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 1) subStep++;
                                else {
                                    if (HasEffect(BRD.Buffs.StraightShotReady)) {
                                        return BRD.RefulgentArrow;
                                    } else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 6) {
                                if (lastComboMove == BRD.EmpyrealArrow) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 7) {
                                if (FindTargetEffect(BRD.Debuffs.Stormbite).RemainingTime < 10) return BRD.IronJaws;
                            }
                            openerFinished = true;
                        }
                    }
                }

                if (IsEnabled(CustomComboPreset.BardSimpleInterrupt) && CanInterruptEnemy() && !GetCooldown(BRD.HeadGraze).IsCooldown) {
                    return BRD.HeadGraze;
                }

                var gauge = GetJobGauge<BRDGauge>();
                var heavyShot = GetCooldown(actionID);
                var heavyShotOnCooldown = heavyShot.CooldownRemaining > 0.7;
                var isEnemyHealthHigh = IsEnabled(CustomComboPreset.BardSimpleRaidMode) ? true : CustomCombo.EnemyHealthPercentage() > 1;

                if (IsEnabled(CustomComboPreset.SimpleSongOption) && heavyShot.IsCooldown && isEnemyHealthHigh) {
                    // Limit optimisation to only when you are high enough to benefit from it.
                    if (level >= BRD.Levels.WanderersMinuet) {
                        // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army Peon
                        var songTimerInSeconds = gauge.SongTimer / 1000;
                        var minuetCooldown = GetCooldown(BRD.WanderersMinuet).IsCooldown;
                        var balladCooldown = GetCooldown(BRD.MagesBallad).IsCooldown;
                        var paeonCooldown = GetCooldown(BRD.ArmysPaeon).IsCooldown;

                        if (gauge.Song == Song.NONE) {
                            // Do logic to determine first song

                            if (!minuetCooldown) return BRD.WanderersMinuet;
                            if (!balladCooldown) return BRD.MagesBallad;
                            if (!paeonCooldown) return BRD.ArmysPaeon;
                        }

                        if (gauge.Song == Song.WANDERER) {
                            // Spend any repertoire before switching to next song
                            if (songTimerInSeconds < 3 && gauge.Repertoire > 0) {
                                return BRD.PitchPerfect;
                            }
                            // Move to Mage's Ballad if < 3 seconds left on song
                            if (songTimerInSeconds < 3 && !balladCooldown) {
                                return BRD.MagesBallad;
                            }
                        }

                        if (gauge.Song == Song.MAGE) {
                            // Move to Army's Paeon if < 9 seconds left on song
                            if (songTimerInSeconds < 9 && !paeonCooldown) {
                                return BRD.ArmysPaeon;
                            }
                        }

                        if (gauge.Song == Song.ARMY) {
                            // Move to Wanderer's Minuet if < 3 seconds left on song
                            if (songTimerInSeconds < 3 && !minuetCooldown) {
                                return BRD.WanderersMinuet;
                            }
                        }
                    } else if (gauge.SongTimer < 1 || gauge.Song == Song.ARMY) {
                        if (level >= BRD.Levels.MagesBallad && !GetCooldown(BRD.MagesBallad).IsCooldown)
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.WanderersMinuet && !GetCooldown(BRD.WanderersMinuet).IsCooldown)
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.ArmysPaeon && !GetCooldown(BRD.ArmysPaeon).IsCooldown)
                            return BRD.ArmysPaeon;
                    }
                }

                if (IsEnabled(CustomComboPreset.BardSimpleBuffsFeature) && inCombat && heavyShotOnCooldown && gauge.Song != Song.NONE && isEnemyHealthHigh) {
                    if (level >= BRD.Levels.RagingStrikes && !GetCooldown(BRD.RagingStrikes).IsCooldown)
                        return BRD.RagingStrikes;
                    if (level >= BRD.Levels.BattleVoice && !GetCooldown(BRD.BattleVoice).IsCooldown)
                        return BRD.BattleVoice;
                    if (level >= BRD.Levels.Barrage && !GetCooldown(BRD.Barrage).IsCooldown)
                        return BRD.Barrage;
                    
                    if (IsEnabled(CustomComboPreset.BardSimpleBuffsRadiantFeature) && Array.TrueForAll(gauge.Coda, BRD.SongIsNotNone)) {
                        if (level >= BRD.Levels.RadiantFinale && !GetCooldown(BRD.RadiantFinale).IsCooldown) {
                            return BRD.RadiantFinale;
                        }
                    }
                }

                if (IsEnabled(CustomComboPreset.SimpleBardFeature) && inCombat) {
                    if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
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


                if (isEnemyHealthHigh) {
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
                        ((venomous && venomousDuration.RemainingTime < 4) || (caustic && causticDuration.RemainingTime < 4)) ||
                        level >= BRD.Levels.IronJaws &&
                        ((windbite && windbiteDuration.RemainingTime < 4) || (stormbite && stormbiteDuration.RemainingTime < 4))
                    );

                    if (level < BRD.Levels.BiteUpgrade) {
                        if (inCombat) {
                            if (useIronJaws) {
                                return BRD.IronJaws;
                            }

                            if (level < BRD.Levels.IronJaws) {
                                if (venomous && venomousDuration.RemainingTime < 4)
                                    return BRD.VenomousBite;
                                if (windbite && windbiteDuration.RemainingTime < 4)
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

                        if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature)) 
                        {
                            return BRD.ApexArrow;
                        }
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
