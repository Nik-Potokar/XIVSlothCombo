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
            public const ushort
                StraightShotReady = 122,
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

        internal static bool SongIsNotNone(Song value)
        {
            return value != Song.NONE;
        }
        
        internal static bool SongIsNone(Song value)
        {
            return value == Song.NONE;
        }
    }

    // Replace Wanderer's Minuet with PP when in WM.
    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardWanderersPitchPerfectFeature;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardStraightShotUpgradeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardDoTMaintain))
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                    if (inCombat)
                    {
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

                if (HasEffect(BRD.Buffs.StraightShotReady))
                {
                    return (level >= BRD.Levels.RefulgentArrow) ? BRD.RefulgentArrow : BRD.StraightShot;
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
            if (actionID == BRD.IronJaws)
            {
                if (IsEnabled(CustomComboPreset.BardIronJawsApexFeature) && level >= BRD.Levels.ApexArrow)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady)) return BRD.BlastArrow;
                    if (gauge.SoulVoice == 100 && IsOffCooldown(BRD.ApexArrow)) return BRD.ApexArrow;
                }


                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null)
                    {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                        {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite)
                        {
                            return BRD.Windbite;
                        }
                    }

                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null))
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite)
                    {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite)
                {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite)
                {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite)
                {
                    return BRD.Stormbite;
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
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null)
                    {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                        {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite)
                        {
                            return BRD.Windbite;
                        }
                    }

                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null))
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4))
                    {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4))
                {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite)
                {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite)
                {
                    return BRD.Stormbite;
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
            if (actionID == BRD.QuickNock)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    return BRD.ApexArrow;
            }

            return actionID;
        }
    }

    internal class BardoGCDAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.RainOfDeath)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.WanderersMinuet && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && IsOffCooldown(BRD.Bloodletter))
                    return BRD.RainOfDeath;
                if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                    return BRD.Sidewinder;

            }

            return actionID;
        }
    }

    internal class BardSimpleAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardSimpleAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Ladonsbite || actionID == BRD.QuickNock)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);

                if (inCombat && (lastComboMove == BRD.Ladonsbite || lastComboMove == BRD.QuickNock))
                {
                    SimpleBardFeature.openerFinished = true;
                    SimpleBardFeature.inOpener = true;
                }

                if (!inCombat)
                {
                    SimpleBardFeature.inOpener = false;
                    SimpleBardFeature.step = 0;
                    SimpleBardFeature.subStep = 0;
                    SimpleBardFeature.usedStraightShotReady = false;
                    SimpleBardFeature.openerFinished = false;
                }

                var gauge = GetJobGauge<BRDGauge>();
                var soulVoice = gauge.SoulVoice;
                var heavyShotOnCooldown = CanWeave(BRD.HeavyShot);

                if (heavyShotOnCooldown)
                {
                    if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        return BRD.PitchPerfect;
                    if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;
                    if (level >= BRD.Levels.RainOfDeath && GetRemainingCharges(BRD.RainOfDeath) > 0)
                        return BRD.RainOfDeath;
                    if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                        return BRD.Sidewinder;
                }

                if (level >= BRD.Levels.Shadowbite && HasEffect(BRD.Buffs.ShadowbiteReady))
                    return BRD.Shadowbite;
                if (level >= BRD.Levels.ApexArrow && soulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    return BRD.ApexArrow;
                if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;

                if (IsEnabled(CustomComboPreset.SimpleAoESongOption) && heavyShotOnCooldown)
                {
                    if ((gauge.SongTimer < 1 || gauge.Song == Song.ARMY) && IsOnCooldown(actionID))
                    {
                        if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                            return BRD.ArmysPaeon;
                    }
                }
            }

            return actionID;
        }
    }

    internal class BardoGCDSingleTargetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDSingleTargetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Bloodletter)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (IsEnabled(CustomComboPreset.BardSongsFeature) && (gauge.SongTimer < 1 || gauge.Song == Song.ARMY))
                {
                    if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                        return BRD.WanderersMinuet;
                    if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                        return BRD.MagesBallad;
                    if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                        return BRD.ArmysPaeon;
                }

                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && IsOffCooldown(BRD.Bloodletter))
                    return BRD.Bloodletter;
                if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                    return BRD.Sidewinder;
            }

            return actionID;
        }
    }
    internal class BardAoEComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardAoEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {
                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {
                    if (level >= BRD.Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardAoEComboFeature) && level >= BRD.Levels.Shadowbite && HasEffectAny(BRD.Buffs.ShadowbiteReady))
                {
                    return BRD.Shadowbite;
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

        internal delegate bool DotRecast(int value);

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<BRDGauge>();

                if (IsEnabled(CustomComboPreset.BardSimpleOpener) && level >= 90)
                {
                    if (inCombat && lastComboMove == BRD.Stormbite && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (!inOpener)
                    {
                        return BRD.Stormbite;
                    }

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        step = 0;
                        subStep = 0;
                        usedStraightShotReady = false;
                        openerFinished = false;

                        return BRD.Stormbite;
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
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
                        if (step == 0)
                        {
                            // Do this in steps, by using the ifs as the step changer...
                            if (subStep == 0)
                            {
                                if (IsOnCooldown(BRD.WanderersMinuet)) subStep++;
                                else return BRD.WanderersMinuet;
                            }
                            if (subStep == 1)
                            {
                                if (IsOnCooldown(BRD.RagingStrikes)) subStep++;
                                else return BRD.RagingStrikes;
                            }
                            if (subStep == 2)
                            {
                                if (lastComboMove == BRD.CausticBite) subStep++;
                                else return BRD.CausticBite;
                            }
                            if (subStep == 3)
                            {
                                if (IsOnCooldown(BRD.EmpyrealArrow)) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 4)
                            {
                                if (GetRemainingCharges(BRD.Bloodletter) < 3) subStep++;
                                else return BRD.Bloodletter;
                            }
                            if (subStep == 5)
                            {
                                if ((usedStraightShotReady && !HasEffect(BRD.Buffs.StraightShotReady)) || lastComboMove == BRD.BurstShot) subStep++;
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        usedStraightShotReady = true;
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 6)
                            {
                                usedStraightShotReady = false;

                                if (HasEffect(BRD.Buffs.RadiantFinale) || Array.TrueForAll(gauge.Coda, BRD.SongIsNone)) subStep++;
                                else return BRD.RadiantFinale;
                            }
                            if (subStep == 7)
                            {
                                if (HasEffect(BRD.Buffs.BattleVoice) || IsOnCooldown(BRD.BattleVoice)) subStep++;
                                else return BRD.BattleVoice;
                            }
                            if (subStep == 8)
                            {
                                if ((usedStraightShotReady && !HasEffect(BRD.Buffs.StraightShotReady)) || lastComboMove == BRD.BurstShot) subStep++;
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        usedStraightShotReady = true;
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 9)
                            {
                                if (GetRemainingCharges(BRD.Bloodletter) < 2) subStep++;
                                else return BRD.Bloodletter;
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
                        if (step == 1)
                        {
                            if (subStep == 0)
                            {
                                if (IsOnCooldown(BRD.Sidewinder)) subStep++;
                                else return BRD.Sidewinder;
                            }
                            if (subStep == 1)
                            {
                                if (!HasEffect(BRD.Buffs.StraightShotReady)) subStep++;
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 2)
                            {
                                if (GetRemainingCharges(BRD.Bloodletter) == 0) subStep++;
                                else return BRD.Bloodletter;
                            }
                            if (subStep == 3)
                            {
                                if (HasEffect(BRD.Buffs.Barrage)) subStep++;
                                else return BRD.Barrage;
                            }
                            if (subStep == 4)
                            {
                                if (!HasEffect(BRD.Buffs.StraightShotReady))
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 5)
                            {
                                if (lastComboMove == BRD.BurstShot) subStep++;
                                else return BRD.BurstShot;
                            }
                            if (subStep == 6)
                            {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 6)
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 7)
                            {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 3)
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 8)
                            {
                                if (IsOffCooldown(BRD.EmpyrealArrow))
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 9)
                            {
                                if (IsOnCooldown(BRD.EmpyrealArrow)) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 10)
                            {
                                if (FindTargetEffect(BRD.Debuffs.Stormbite).RemainingTime < 40) return BRD.IronJaws;
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
                        if (step == 2)
                        {
                            if (subStep == 0)
                            {
                                if (HasEffect(BRD.Buffs.Barrage)) subStep++;
                                else return BRD.Barrage;
                            }
                            if (subStep == 1)
                            {
                                if (!HasEffect(BRD.Buffs.StraightShotReady)) subStep++;
                                else return BRD.RefulgentArrow;
                            }
                            if (subStep == 2)
                            {
                                if (GetRemainingCharges(BRD.Bloodletter) < 1) subStep++;
                                else return BRD.Bloodletter;
                            }
                            if (subStep == 3)
                            {
                                if (IsOnCooldown(BRD.Sidewinder)) subStep++;
                                else return BRD.Sidewinder;
                            }
                            if (subStep == 4)
                            {
                                if (lastComboMove == BRD.BurstShot)
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else return BRD.BurstShot;
                            }
                            if (subStep == 5)
                            {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 6)
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 6)
                            {
                                if (GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 3)
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }

                            if (subStep == 7)
                            {
                                if (IsOffCooldown(BRD.EmpyrealArrow))
                                {
                                    if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3) return BRD.PitchPerfect;
                                    subStep++;
                                }
                                else
                                {
                                    if (HasEffect(BRD.Buffs.StraightShotReady))
                                    {
                                        return BRD.RefulgentArrow;
                                    }
                                    else return BRD.BurstShot;
                                }
                            }
                            if (subStep == 8)
                            {
                                if (IsOnCooldown(BRD.EmpyrealArrow)) subStep++;
                                else return BRD.EmpyrealArrow;
                            }
                            if (subStep == 9)
                            {
                                if (FindTargetEffect(BRD.Debuffs.Stormbite).RemainingTime < 40) return BRD.IronJaws;
                            }
                            openerFinished = true;
                        }
                    }
                }

                if (IsEnabled(CustomComboPreset.BardSimpleInterrupt) && CanInterruptEnemy() && IsOffCooldown(BRD.HeadGraze))
                {
                    return BRD.HeadGraze;
                }

                var heavyShot = GetCooldown(actionID);
                var canWeave = (
                    CanWeave(actionID) ||
                    CanWeave(BRD.BlastArrow) ||
                    CanWeave(BRD.ApexArrow) ||
                    CanWeave(BRD.Stormbite) ||
                    CanWeave(BRD.CausticBite) ||
                    CanWeave(BRD.VenomousBite) ||
                    CanWeave(BRD.Windbite) ||
                    CanWeave(BRD.StraightShot) ||
                    CanWeave(BRD.RefulgentArrow) ||
                    CanWeave(BRD.IronJaws)
                );
                var isEnemyHealthHigh = IsEnabled(CustomComboPreset.BardSimpleRaidMode) ? true : CustomCombo.EnemyHealthPercentage() > 1;

                if (IsEnabled(CustomComboPreset.SimpleSongOption) && canWeave && isEnemyHealthHigh)
                {
                    // Limit optimisation to only when you are high enough to benefit from it.
                    if (level >= BRD.Levels.WanderersMinuet)
                    {
                        // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army Peon
                        var songTimerInSeconds = gauge.SongTimer / 1000;
                        var minuetOffCooldown = IsOffCooldown(BRD.WanderersMinuet);
                        var balladOffCooldown = IsOffCooldown(BRD.MagesBallad);
                        var paeonOffCooldown = IsOffCooldown(BRD.ArmysPaeon);

                        if (gauge.Song == Song.NONE)
                        {
                            // Do logic to determine first song

                            if (minuetOffCooldown) return BRD.WanderersMinuet;
                            if (balladOffCooldown) return BRD.MagesBallad;
                            if (paeonOffCooldown) return BRD.ArmysPaeon;
                        }

                        if (gauge.Song == Song.WANDERER)
                        {
                            // Spend any repertoire before switching to next song
                            if (songTimerInSeconds <= 3 && gauge.Repertoire > 0)
                            {
                                return BRD.PitchPerfect;
                            }
                            // Move to Mage's Ballad if < 3 seconds left on song
                            if (songTimerInSeconds < 3 && balladOffCooldown)
                            {
                                return BRD.MagesBallad;
                            }
                        }

                        if (gauge.Song == Song.MAGE)
                        {
                            // Move to Army's Paeon if < 9 seconds left on song
                            if (songTimerInSeconds < 12 && paeonOffCooldown)
                            {
                                return BRD.ArmysPaeon;
                            }
                        }

                        if (gauge.Song == Song.ARMY)
                        {
                            // Move to Wanderer's Minuet if < 3 seconds left on song
                            if (songTimerInSeconds < 3 || minuetOffCooldown)
                            {
                                return BRD.WanderersMinuet;
                            }
                        }
                    }
                    else if (gauge.SongTimer < 1 || gauge.Song == Song.ARMY)
                    {
                        if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                            return BRD.ArmysPaeon;
                    }
                }

                if (IsEnabled(CustomComboPreset.BardSimpleBuffsFeature) && inCombat && canWeave && gauge.Song != Song.NONE && isEnemyHealthHigh)
                {
                    if (level >= BRD.Levels.RagingStrikes && IsOffCooldown(BRD.RagingStrikes))
                        return BRD.RagingStrikes;
                    if (IsEnabled(CustomComboPreset.BardSimpleBuffsRadiantFeature) && Array.TrueForAll(gauge.Coda, BRD.SongIsNotNone) && IsOffCooldown(BRD.BattleVoice))
                    {
                        if (level >= BRD.Levels.RadiantFinale && IsOffCooldown(BRD.RadiantFinale))
                        {
                            return BRD.RadiantFinale;
                        }
                    }
                    if (level >= BRD.Levels.BattleVoice && IsOffCooldown(BRD.BattleVoice))
                        return BRD.BattleVoice;
                    if (level >= BRD.Levels.Barrage && IsOffCooldown(BRD.Barrage))
                        return BRD.Barrage;
                }

                if (IsEnabled(CustomComboPreset.SimpleBardFeature) && inCombat)
                {
                    if (canWeave)
                    {
                        if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                            return BRD.PitchPerfect;
                        if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                            return BRD.EmpyrealArrow;
                        if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                            return BRD.Sidewinder;
                        if (level >= BRD.Levels.Bloodletter)
                        {
                            var bloodletterCharges = GetRemainingCharges(BRD.Bloodletter);

                            if (IsEnabled(CustomComboPreset.BardSimplePooling) && level >= BRD.Levels.WanderersMinuet)
                            {
                                if (gauge.Song == Song.WANDERER)
                                {
                                    if (
                                        (HasEffect(BRD.Buffs.RagingStrikes) || GetCooldown(BRD.RagingStrikes).CooldownRemaining > 10) &&
                                        bloodletterCharges > 0
                                    )
                                    {
                                        return BRD.Bloodletter;
                                    }
                                }

                                if (gauge.Song == Song.ARMY && bloodletterCharges == 3) return BRD.Bloodletter;
                                if (gauge.Song == Song.MAGE && bloodletterCharges > 0) return BRD.Bloodletter;
                                if (gauge.Song == Song.NONE && bloodletterCharges == 3) return BRD.Bloodletter;
                            }
                            else if (bloodletterCharges > 0)
                            {
                                return BRD.Bloodletter;
                            }
                        }
                    }

                    if (IsEnabled(CustomComboPreset.BardSimpleOpener) && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    {
                        if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                            return BRD.BlastArrow;
                        if (level >= BRD.Levels.ApexArrow)
                        {
                            var songTimerInSeconds = gauge.SongTimer / 1000;
                            
                            if (gauge.Song == Song.MAGE && gauge.SoulVoice == 100) return BRD.ApexArrow;
                            if (gauge.Song == Song.MAGE && gauge.SoulVoice >= 80 && songTimerInSeconds > 18 && songTimerInSeconds < 22) return BRD.ApexArrow;
                            if (gauge.Song == Song.WANDERER && HasEffect(BRD.Buffs.RagingStrikes) && HasEffect(BRD.Buffs.BattleVoice) && HasEffect(BRD.Buffs.RadiantFinale) && gauge.SoulVoice >= 80) return BRD.ApexArrow;
                        }
                    }
                    else
                    {
                        if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                            return BRD.BlastArrow;

                        if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                            return BRD.ApexArrow;
                    }
                }


                if (isEnemyHealthHigh)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                    var ragingStrikesDuration = FindEffect(BRD.Buffs.RagingStrikes);
                    DotRecast poisonRecast = delegate (int duration)
                    {
                       return (venomous && venomousDuration.RemainingTime < duration) || (caustic && causticDuration.RemainingTime < duration);
                    };
                    DotRecast windRecast = delegate (int duration)
                    {
                        return (windbite && windbiteDuration.RemainingTime < duration) || (stormbite && stormbiteDuration.RemainingTime < duration);
                    }; 

                   
                    var useIronJaws = (
                        (level >= BRD.Levels.IronJaws && poisonRecast(4)) ||
                        (level >= BRD.Levels.IronJaws && windRecast(4)) ||
                        (level >= BRD.Levels.IronJaws && IsEnabled(CustomComboPreset.BardSimpleRagingJaws) &&
                            HasEffect(BRD.Buffs.RagingStrikes) && ragingStrikesDuration.RemainingTime < 3 &&
                            poisonRecast(40) && windRecast(40))
                    );

                    if (level < BRD.Levels.BiteUpgrade)
                    {
                        if (inCombat)
                        {
                            if (useIronJaws)
                            {
                                return BRD.IronJaws;
                            }

                            if (level < BRD.Levels.IronJaws)
                            {
                                if (venomous && venomousDuration.RemainingTime < 4)
                                    return BRD.VenomousBite;
                                if (windbite && windbiteDuration.RemainingTime < 4)
                                    return BRD.Windbite;
                            }

                            if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                            {
                                if (level >= BRD.Levels.Windbite && !windbite)
                                    return BRD.Windbite;
                                if (level >= BRD.Levels.VenomousBite && !venomous)
                                    return BRD.VenomousBite;
                            }
                        }

                        if (HasEffect(BRD.Buffs.StraightShotReady))
                        {
                            return (level >= BRD.Levels.RefulgentArrow) ? BRD.RefulgentArrow : BRD.StraightShot;
                        }

                        return (level >= BRD.Levels.BurstShot) ? BRD.BurstShot : BRD.HeavyShot;
                    }

                    if (inCombat)
                    {
                        if (useIronJaws)
                        {
                            return BRD.IronJaws;
                        }

                        if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                        {
                            if (level >= BRD.Levels.CausticBite && !caustic)
                                return BRD.CausticBite;
                            if (level >= BRD.Levels.StormBite && !stormbite)
                                return BRD.Stormbite;
                        }
                    }
                }

                if (HasEffect(BRD.Buffs.StraightShotReady))
                {
                    return (level >= BRD.Levels.RefulgentArrow) ? BRD.RefulgentArrow : BRD.StraightShot;
                }

                return (level >= BRD.Levels.BurstShot) ? BRD.BurstShot : BRD.HeavyShot;
            }

            return actionID;
        }
    }
    internal class BardBuffsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardBuffsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Barrage)
            {
                if (level >= BRD.Levels.RagingStrikes && IsOffCooldown(BRD.RagingStrikes))
                    return BRD.RagingStrikes;
                if (level >= BRD.Levels.BattleVoice && IsOffCooldown(BRD.BattleVoice))
                    return BRD.BattleVoice;
            }

            return actionID;
        }
    }
    internal class BardOneButtonSongs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardOneButtonSongs;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)
            { // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
                if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                    return BRD.MagesBallad;
                if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                    return BRD.ArmysPaeon;
            }

            return actionID;
        }
    }
}
