using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using XIVSlothCombo.Combos.PvE.Content;
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
            RadiantFinale = 25785,
            WideVolley = 36974,
            ResonantArrow = 36976,
            RadiantEncore = 36977;

        public static class Buffs
        {
            public const ushort
                RagingStrikes = 125,
                Barrage = 128,
                MagesBallad = 135,
                ArmysPaeon = 137,
                BattleVoice = 141,
                WanderersMinuet = 865,
                Troubadour = 1934,
                BlastArrowReady = 2692,
                RadiantFinale = 2722,
                ShadowbiteReady = 3002,
                HawksEye = 3861,
                ResonantArrowReady = 3862,
                RadiantEncoreReady = 3863;
        }

        public static class Debuffs
        {
            public const ushort
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Config
        {
            public const string
                BRD_RagingJawsRenewTime = "ragingJawsRenewTime",
                BRD_NoWasteHPPercentage = "noWasteHpPercentage",
                BRD_STSecondWindThreshold = "BRD_STSecondWindThreshold",
                BRD_AoESecondWindThreshold = "BRD_AoESecondWindThreshold",
                BRD_VariantCure = "BRD_VariantCure";
        }

        #region Song status
        internal static bool SongIsNotNone(Song value) => value != Song.NONE;
        internal static bool SongIsNone(Song value) => value == Song.NONE;
        internal static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
        #endregion

        // Replace HS/BS with SS/RA when procced.
        internal class BRD_StraightShotUpgrade : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeavyShot or BurstShot)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();

                        if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow) && gauge.SoulVoice == 100)
                            return ApexArrow;
                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        bool caustic = TargetHasEffect(Debuffs.CausticBite);
                        bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);
                        float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                        float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                        if (InCombat())
                        {
                            if (LevelChecked(IronJaws) &&
                                ((venomous && venomRemaining < 4) || (caustic && causticRemaining < 4)) ||
                                (windbite && windRemaining < 4) || (stormbite && stormRemaining < 4))
                                return IronJaws;
                            if (!LevelChecked(IronJaws) && venomous && venomRemaining < 4)
                                return VenomousBite;
                            if (!LevelChecked(IronJaws) && windbite && windRemaining < 4)
                                return Windbite;
                        }
                    }

                    if (HasEffect(Buffs.HawksEye))
                        return LevelChecked(RefulgentArrow)
                            ? RefulgentArrow
                            : StraightShot;
                }

                return actionID;
            }
        }

        internal class BRD_IronJaws : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is IronJaws)
                {
                    if (IsEnabled(CustomComboPreset.BRD_IronJawsApex) && LevelChecked(ApexArrow))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();

                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                        if (gauge.SoulVoice == 100 && IsOffCooldown(ApexArrow))
                            return ApexArrow;
                    }

                    if (!LevelChecked(IronJaws))
                    {
                        Status? venomous = FindTargetEffect(Debuffs.VenomousBite);
                        Status? windbite = FindTargetEffect(Debuffs.Windbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            if (LevelChecked(VenomousBite) && venomRemaining < windRemaining)
                                return VenomousBite;
                            if (LevelChecked(Windbite))
                                return Windbite;
                        }

                        if (LevelChecked(VenomousBite) && (!LevelChecked(Windbite) || windbite is not null))
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    if (!LevelChecked(Stormbite))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);

                        if (LevelChecked(IronJaws) && venomous && windbite)
                            return IronJaws;
                        if (LevelChecked(VenomousBite) && windbite)
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    bool caustic = TargetHasEffect(Debuffs.CausticBite);
                    bool stormbite = TargetHasEffect(Debuffs.Stormbite);

                    if (LevelChecked(IronJaws) && caustic && stormbite)
                        return IronJaws;
                    if (LevelChecked(CausticBite) && stormbite)
                        return CausticBite;
                    if (LevelChecked(Stormbite))
                        return Stormbite;
                }

                return actionID;
            }
        }

        internal class BRD_IronJaws_Alternate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws_Alternate;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is IronJaws)
                {
                    if (!LevelChecked(IronJaws))
                    {
                        Status? venomous = FindTargetEffect(Debuffs.VenomousBite);
                        Status? windbite = FindTargetEffect(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                            float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                            if (LevelChecked(VenomousBite) && venomRemaining < windRemaining)
                                return VenomousBite;
                            if (LevelChecked(Windbite))
                                return Windbite;
                        }

                        if (LevelChecked(VenomousBite) && (!LevelChecked(Windbite) || windbite is not null))
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    if (!LevelChecked(Stormbite))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                        if (LevelChecked(IronJaws) && venomous && windbite &&
                            (venomRemaining < 4 || windRemaining < 4))
                            return IronJaws;
                        if (LevelChecked(VenomousBite) && windbite)
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    bool caustic = TargetHasEffect(Debuffs.CausticBite);
                    bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                    float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                    float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                    if (LevelChecked(IronJaws) && caustic && stormbite &&
                        (causticRemaining < 4 || stormRemaining < 4))
                        return IronJaws;
                    if (LevelChecked(CausticBite) && stormbite)
                        return CausticBite;
                    if (LevelChecked(Stormbite))
                        return Stormbite;
                }

                return actionID;
            }
        }

        internal class BRD_Apex : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Apex;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is QuickNock)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();

                    if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow) && LevelChecked(ApexArrow) && gauge.SoulVoice == 100)
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
                if (actionID is RainOfDeath)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                    bool bloodletterReady = LevelChecked(Bloodletter) && IsOffCooldown(Bloodletter);
                    bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);

                    if (LevelChecked(WanderersMinuet) && songWanderer && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (empyrealReady)
                        return EmpyrealArrow;
                    if (bloodletterReady)
                        return RainOfDeath;
                    if (sidewinderReady)
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
                if (actionID is Ladonsbite or QuickNock)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Simple_Songs) && canWeave)
                    {
                        int songTimerInSeconds = gauge.SongTimer / 1000;
                        bool songNone = gauge.Song == Song.NONE;

                        if (songTimerInSeconds < 3 || songNone)
                        {
                            if (LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet) &&
                                !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)) &&
                                !IsEnabled(CustomComboPreset.BRD_AoE_Simple_SongsExcludeWM))
                                return WanderersMinuet;

                            if (LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad) &&
                                !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;

                            if (LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon) &&
                                !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }
                    }

                    if (canWeave)
                    {
                        bool songWanderer = gauge.Song == Song.WANDERER;
                        bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                        bool rainOfDeathReady = LevelChecked(RainOfDeath) && GetRemainingCharges(RainOfDeath) > 0;
                        bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);
                        bool barrageReady = LevelChecked(Shadowbite) && IsOffCooldown(Barrage); // Adding Barrage to AoE, but only if we can use Shadowbite.

                        if (LevelChecked(PitchPerfect) && songWanderer && gauge.Repertoire == 3)
                            return OriginalHook(WanderersMinuet);
                        if (empyrealReady)
                            return EmpyrealArrow;
                        if (rainOfDeathReady)
                            return RainOfDeath;
                        if (sidewinderReady)
                            return Sidewinder;
                        if (barrageReady)
                            return Barrage;

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.BRD_AoE_SecondWind))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_AoESecondWindThreshold) && ActionReady(All.SecondWind))
                                return All.SecondWind;
                        }
                    }

                    bool wideVolleyReady = LevelChecked(WideVolley) && HasEffect(Buffs.HawksEye);
                    bool blastArrowReady = LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady);
                    bool resonantArrowReady = LevelChecked(ResonantArrow) && HasEffect(Buffs.ResonantArrowReady);

                    if (wideVolleyReady)
                        return OriginalHook(WideVolley);
                    if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                        return ApexArrow;
                    if (blastArrowReady)
                        return BlastArrow;
                    if (resonantArrowReady)
                        return ResonantArrow;
                    if (HasEffect(Buffs.RadiantEncoreReady))
                        return RadiantEncore;

                }

                return actionID;
            }
        }

        internal class BRD_ST_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Bloodletter)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool songArmy = gauge.Song == Song.ARMY;
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool minuetReady = LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet);
                    bool balladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                    bool paeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);
                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                    bool bloodletterReady = LevelChecked(Bloodletter) && IsOffCooldown(Bloodletter);
                    bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);

                    if (IsEnabled(CustomComboPreset.BRD_oGCDSongs) &&
                        (gauge.SongTimer < 1 || songArmy))
                    {
                        if (minuetReady)
                            return WanderersMinuet;
                        if (balladReady)
                            return MagesBallad;
                        if (paeonReady)
                            return ArmysPaeon;
                    }

                    if (songWanderer && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (empyrealReady)
                        return EmpyrealArrow;
                    if (bloodletterReady)
                        return OriginalHook(Bloodletter);
                    if (sidewinderReady)
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
                if (actionID is QuickNock or Ladonsbite)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();
                        bool blastReady = LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady);

                        if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                            return ApexArrow;
                        if (blastReady)
                            return BlastArrow;
                    }

                    bool wideVolleyReady = LevelChecked(WideVolley) && HasEffect(Buffs.HawksEye);

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Combo) && wideVolleyReady)
                        return WideVolley;
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
                if (actionID is HeavyShot or BurstShot)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);
                    bool canWeaveBuffs = CanWeave(actionID, 0.6);
                    bool canWeaveDelayed = CanDelayedWeave(actionID, 0.9);
                    bool songNone = gauge.Song == Song.NONE;
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool songMage = gauge.Song == Song.MAGE;
                    bool songArmy = gauge.Song == Song.ARMY;
                    bool canInterrupt = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze);
                    int targetHPThreshold = PluginConfiguration.GetCustomIntValue(Config.BRD_NoWasteHPPercentage);
                    bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_Simple_NoWaste) || GetTargetHPPercent() > targetHPThreshold;

                    if (!InCombat() && (inOpener || openerFinished))
                    {
                        openerFinished = false;
                    }

                    if (!IsEnabled(CustomComboPreset.BRD_Simple_NoWaste))
                        openerFinished = true;

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Interrupt) && canInterrupt)
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Song) && isEnemyHealthHigh)
                    {
                        int songTimerInSeconds = gauge.SongTimer / 1000;

                        // Limit optimisation to when you are high enough level to benefit from it.
                        if (LevelChecked(WanderersMinuet))
                        {
                            // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army's Paeon    
                            bool minuetReady = IsOffCooldown(WanderersMinuet);
                            bool balladReady = IsOffCooldown(MagesBallad);
                            bool paeonReady = IsOffCooldown(ArmysPaeon);

                            if (canWeave)
                            {
                                if (songNone)
                                {
                                    // Logic to determine first song
                                    if (minuetReady && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                        return WanderersMinuet;
                                    if (balladReady && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                        return MagesBallad;
                                    if (paeonReady && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                        return ArmysPaeon;
                                }

                                if (songWanderer)
                                {
                                    if (songTimerInSeconds < 3 && gauge.Repertoire > 0) // Spend any repertoire before switching to next song
                                        return OriginalHook(WanderersMinuet);
                                    if (songTimerInSeconds < 3 && balladReady)          // Move to Mage's Ballad if < 3 seconds left on song
                                        return MagesBallad;
                                }

                                if (songMage)
                                {
                                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);

                                    // Move to Army's Paeon if < 12 seconds left on song
                                    if (songTimerInSeconds < 12 && paeonReady)
                                    {
                                        // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                                        if (empyrealReady)
                                            return EmpyrealArrow;
                                        return ArmysPaeon;
                                    }
                                }
                            }

                            if (songArmy && canWeaveDelayed)
                            {
                                // Move to Wanderer's Minuet if < 3 seconds left on song or WM off CD and have 4 repertoires of AP
                                if (songTimerInSeconds < 3 || (minuetReady && gauge.Repertoire == 4))
                                    return WanderersMinuet;
                            }
                        }
                        else if (songTimerInSeconds < 3 && canWeave)
                        {
                            bool balladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                            bool paeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);

                            if (balladReady)
                                return MagesBallad;
                            if (paeonReady)
                                return ArmysPaeon;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Buffs) && (!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
                    {
                        bool radiantReady = LevelChecked(RadiantFinale) && IsOffCooldown(RadiantFinale);
                        bool ragingReady = LevelChecked(RagingStrikes) && IsOffCooldown(RagingStrikes);
                        bool battleVoiceReady = LevelChecked(BattleVoice) && IsOffCooldown(BattleVoice);
                        bool barrageReady = LevelChecked(Barrage) && IsOffCooldown(Barrage);
                        bool firstMinute = CombatEngageDuration().Minutes == 0;
                        bool restOfFight = CombatEngageDuration().Minutes > 0;

                        if (ragingReady && ((canWeaveBuffs && firstMinute) || (canWeaveDelayed && restOfFight)) &&
                            (GetCooldownRemainingTime(BattleVoice) <= 5.38 || battleVoiceReady || !LevelChecked(BattleVoice)))
                            return RagingStrikes;

                        if (canWeaveBuffs && IsEnabled(CustomComboPreset.BRD_Simple_BuffsRadiant) && radiantReady &&
                            (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)) &&
                            (battleVoiceReady || GetCooldownRemainingTime(BattleVoice) < 0.7) &&
                            (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
                        {
                            if (!JustUsed(RagingStrikes))
                                return RadiantFinale;
                        }

                        if (canWeaveBuffs && battleVoiceReady &&
                            (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
                        {
                            if (!JustUsed(RagingStrikes))
                                return BattleVoice;
                        }

                        if (canWeaveBuffs && barrageReady && !HasEffect(Buffs.HawksEye) && HasEffect(Buffs.RagingStrikes))
                        {
                            if (LevelChecked(RadiantFinale) && HasEffect(Buffs.RadiantFinale))
                                return Barrage;
                            else if (LevelChecked(BattleVoice) && HasEffect(Buffs.BattleVoice))
                                return Barrage;
                            else if (!LevelChecked(BattleVoice) && HasEffect(Buffs.RagingStrikes))
                                return Barrage;
                        }
                    }

                    if (HasEffect(Buffs.RadiantEncoreReady))
                        return RadiantEncore;

                    if (canWeave)
                    {
                        bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                        bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);
                        float battleVoiceCD = GetCooldownRemainingTime(BattleVoice);
                        float empyrealCD = GetCooldownRemainingTime(EmpyrealArrow);
                        float ragingCD = GetCooldownRemainingTime(RagingStrikes);
                        float radiantCD = GetCooldownRemainingTime(RadiantFinale);

                        if (empyrealReady && ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5) || !IsEnabled(CustomComboPreset.BRD_Simple_Buffs)))
                            return EmpyrealArrow;

                        if (LevelChecked(PitchPerfect) && songWanderer &&
                            (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && empyrealCD < 2)) &&
                            ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5)))
                            return OriginalHook(WanderersMinuet);

                        if (sidewinderReady && ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5) || !IsEnabled(CustomComboPreset.BRD_Simple_Buffs)))
                        {
                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling))
                            {
                                if (songWanderer)
                                {
                                    if ((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                        (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10) &&
                                        (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                        !LevelChecked(RadiantFinale)))
                                        return Sidewinder;
                                }
                                else return Sidewinder;
                            }
                            else return Sidewinder;
                        }


                        if (LevelChecked(Bloodletter) && ((!openerFinished && IsOnCooldown(RagingStrikes)) || openerFinished))
                        {
                            uint bloodletterCharges = GetRemainingCharges(Bloodletter);

                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling) && LevelChecked(WanderersMinuet))
                            {
                                if (songWanderer)
                                {
                                    if (((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                        (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10 ||
                                        !LevelChecked(BattleVoice)) &&
                                        (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                        !LevelChecked(RadiantFinale)) &&
                                        bloodletterCharges > 0) || bloodletterCharges > 2)
                                        return OriginalHook(Bloodletter);
                                }

                                if (songArmy && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0)))
                                    return OriginalHook(Bloodletter);
                                if (songMage && bloodletterCharges > 0)
                                    return OriginalHook(Bloodletter);
                                if (songNone && bloodletterCharges == 3)
                                    return OriginalHook(Bloodletter);
                            }
                            else if (bloodletterCharges > 0)
                                return OriginalHook(Bloodletter);
                        }

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_STSecondWindThreshold) && ActionReady(All.SecondWind))
                                return All.SecondWind;
                        }
                    }

                    if (isEnemyHealthHigh)
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        bool caustic = TargetHasEffect(Debuffs.CausticBite);
                        bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);
                        float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                        float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                        DotRecast poisonRecast = delegate (int duration)
                        {
                            return (venomous && venomRemaining < duration) || (caustic && causticRemaining < duration);
                        };

                        DotRecast windRecast = delegate (int duration)
                        {
                            return (windbite && windRemaining < duration) || (stormbite && stormRemaining < duration);
                        };

                        float ragingStrikesDuration = GetBuffRemainingTime(Buffs.RagingStrikes);
                        int ragingJawsRenewTime = PluginConfiguration.GetCustomIntValue(Config.BRD_RagingJawsRenewTime);
                        bool useIronJaws = (LevelChecked(IronJaws) && poisonRecast(4)) ||
                            (LevelChecked(IronJaws) && windRecast(4)) ||
                            (LevelChecked(IronJaws) && IsEnabled(CustomComboPreset.BRD_Simple_RagingJaws) &&
                            HasEffect(Buffs.RagingStrikes) && ragingStrikesDuration < ragingJawsRenewTime &&
                            poisonRecast(40) && windRecast(40));
                        bool dotOpener = (IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener) && !openerFinished) || !IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener);

                        if (!LevelChecked(Stormbite))
                        {
                            if (useIronJaws)
                            {
                                openerFinished = true;
                                return IronJaws;
                            }

                            if (!LevelChecked(IronJaws))
                            {
                                if (windbite && windRemaining < 4)
                                {
                                    openerFinished = true;
                                    return Windbite;
                                }

                                if (venomous && venomRemaining < 4)
                                {
                                    openerFinished = true;
                                    return VenomousBite;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BRD_Simple_DoT))
                            {
                                if (LevelChecked(Windbite) && !windbite && dotOpener)
                                    return Windbite;
                                if (LevelChecked(VenomousBite) && !venomous && dotOpener)
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
                                if (LevelChecked(Stormbite) && !stormbite && dotOpener)
                                    return Stormbite;
                                if (LevelChecked(CausticBite) && !caustic && dotOpener)
                                    return CausticBite;
                            }
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                    {
                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;

                        if (LevelChecked(ApexArrow))
                        {
                            int songTimerInSeconds = gauge.SongTimer / 1000;

                            if (songMage && gauge.SoulVoice == 100)
                                return ApexArrow;
                            if (songMage && gauge.SoulVoice >= 80 &&
                                songTimerInSeconds > 18 && songTimerInSeconds < 22)
                                return ApexArrow;
                            if (songWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                                (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                                return ApexArrow;
                        }
                    }

                    if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                        return OriginalHook(StraightShot);

                    if (HasEffect(Buffs.ResonantArrowReady))
                        return ResonantArrow;

                }

                return actionID;
            }
        }
        internal class BRD_Buffs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Buffs;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Barrage)
                {
                    bool ragingReady = LevelChecked(RagingStrikes) && IsOffCooldown(RagingStrikes);
                    bool battleVoiceReady = LevelChecked(BattleVoice) && IsOffCooldown(BattleVoice);

                    if (ragingReady)
                        return RagingStrikes;
                    if (battleVoiceReady)
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
                if (actionID is WanderersMinuet)
                {
                    // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    int songTimerInSeconds = gauge.SongTimer / 1000;
                    bool wanderersMinuetReady = LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet);
                    bool magesBalladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                    bool armysPaeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);

                    if (wanderersMinuetReady || (gauge.Song == Song.WANDERER && songTimerInSeconds > 2))
                        return WanderersMinuet;

                    if (magesBalladReady || (gauge.Song == Song.MAGE && songTimerInSeconds > 11))
                        return MagesBallad;

                    if (armysPaeonReady || (gauge.Song == Song.ARMY && songTimerInSeconds > 2))
                        return ArmysPaeon;

                }

                return actionID;
            }
        }
    }
}
