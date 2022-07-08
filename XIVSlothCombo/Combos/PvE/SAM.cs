using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class SAM
    {
        public const byte JobID = 34;

        public static int NumSen(SAMGauge gauge)
        {
            var ka = gauge.Sen.HasFlag(Sen.KA);
            var getsu = gauge.Sen.HasFlag(Sen.GETSU);
            var setsu = gauge.Sen.HasFlag(Sen.SETSU);
            return (ka ? 1 : 0) + (getsu ? 1 : 0) + (setsu ? 1 : 0);
        }

        public const uint
            Hakaze = 7477,
            Yukikaze = 7480,
            Gekko = 7481,
            Enpi = 7486,
            Jinpu = 7478,
            Kasha = 7482,
            Shifu = 7479,
            Mangetsu = 7484,
            Fuga = 7483,
            Oka = 7485,
            Higanbana = 7489,
            TenkaGoken = 7488,
            Setsugekka = 7487,
            Shinten = 7490,
            Kyuten = 7491,
            Hagakure = 7495,
            Guren = 7496,
            Senei = 16481,
            MeikyoShisui = 7499,
            Seigan = 7501,
            ThirdEye = 7498,
            Iaijutsu = 7867,
            TsubameGaeshi = 16483,
            KaeshiHiganbana = 16484,
            Shoha = 16487,
            Shoha2 = 25779,
            Ikishoten = 16482,
            Fuko = 25780,
            OgiNamikiri = 25781,
            KaeshiNamikiri = 25782,
            Yaten = 7493,
            Gyoten = 7492;

        public static class Buffs
        {
            public const ushort
                MeikyoShisui = 1233,
                EnhancedEnpi = 1236,
                EyesOpen = 1252,
                OgiNamikiriReady = 2959,
                Fuka = 1299,
                Fugetsu = 1298;
        }

        public static class Debuffs
        {
            public const ushort
                Higanbana = 1228;
        }

        public static class Levels
        {
            public const byte
                Jinpu = 4,
                Enpi = 15,
                Shifu = 18,
                Higanbana = 30,
                Gekko = 30,
                Iaijutsu = 30,
                Mangetsu = 35,
                Kasha = 40,
                TenkaGoken = 40,
                Oka = 45,
                MeikyoShisui = 50,
                Yukikaze = 50,
                Setsugekka = 50,
                Shinten = 52,
                Gyoten = 54,
                Kyuten = 62,
                Ikishoten = 68,
                Guren = 70,
                Senei = 72,
                TsubameGaeshi = 76,
                Shoha = 80,
                Shoha2 = 82,
                OgiNamikiri = 90;
        }
        public static class Config
        {
            public const string
                SAM_ST_KenkiOvercapAmount = "SamKenkiOvercapAmount";
            public const string
                SAM_AoE_KenkiOvercapAmount = "SamAOEKenkiOvercapAmount";
            public const string
                SAM_FillerCombo = "SamFillerCombo";
        }


        internal class SAM_ST_YukikazeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_YukikazeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Yukikaze)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamKenkiOvercapAmount = PluginConfiguration.GetCustomIntValue(Config.SAM_ST_KenkiOvercapAmount);

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_TrueNorth) && GetBuffStacks(Buffs.MeikyoShisui) > 0 && !HasEffect(All.Buffs.TrueNorth) && GetRemainingCharges(All.TrueNorth) > 0)
                            return All.TrueNorth;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Overcap) && gauge.Kenki >= SamKenkiOvercapAmount && level >= Levels.Shinten)
                            return Shinten;
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                        return Yukikaze;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Hakaze && level >= Levels.Yukikaze)
                            return Yukikaze;
                    }

                    return Hakaze;
                }

                return actionID;
            }
        }

        internal class SAM_ST_GekkoCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_GekkoCombo;
            internal static bool inOpener = false;
            internal static bool inOddFiller = false;
            internal static bool inEvenFiller = false;
            internal static bool nonOpener = false;
            internal static bool hasDied = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Gekko)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamKenkiOvercapAmount = PluginConfiguration.GetCustomIntValue(Config.SAM_ST_KenkiOvercapAmount);
                    var meikyoBuff = HasEffect(Buffs.MeikyoShisui);
                    var oneSeal = OriginalHook(Iaijutsu) == Higanbana;
                    var twoSeal = OriginalHook(Iaijutsu) == TenkaGoken;
                    var threeSeal = OriginalHook(Iaijutsu) == Setsugekka;
                    var meikyostacks = GetBuffStacks(Buffs.MeikyoShisui);
                    var SamFillerCombo = PluginConfiguration.GetCustomIntValue(Config.SAM_FillerCombo);
                    bool openerReady = GetRemainingCharges(MeikyoShisui) == 1 && IsOffCooldown(Senei) && IsOffCooldown(Ikishoten) && GetRemainingCharges(TsubameGaeshi) == 2;
                    
                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_RangedUptime) && level >= Levels.Enpi && !inEvenFiller && !inOddFiller && !InMeleeRange() && HasBattleTarget())
                        return Enpi;

                    if (CanSpellWeave(actionID) && IsEnabled(CustomComboPreset.SAM_TrueNorth) && GetBuffStacks(Buffs.MeikyoShisui) > 0 && !HasEffect(All.Buffs.TrueNorth) && GetRemainingCharges(All.TrueNorth) > 0)
                        return All.TrueNorth;

                    if (!InCombat())
                    {
                        hasDied = false;
                        nonOpener = true;
                        inOpener = false;

                        if (level == 90 && IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Opener))
                        {
                            if (meikyoBuff && openerReady)
                            {
                                if (!inOpener)
                                    inOpener = true;
                                nonOpener = false;
                            }

                            if (inOpener)
                            {
                                if (GetBuffStacks(Buffs.MeikyoShisui) == 3 && (oneSeal || twoSeal || threeSeal))
                                    return Hagakure;
                            }
                        }
                        //Prep for Opener
                        if (meikyoBuff && IsOnCooldown(MeikyoShisui) && gauge.Sen == Sen.NONE)
                            return Gekko;

                        //Stops waste if you use Iaijutsu or Ogi and you've got a Kaeshi ready
                        if (!inOpener)
                        {
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri) && (gauge.Kaeshi == Kaeshi.NAMIKIRI))
                                return OriginalHook(OgiNamikiri);

                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Iaijutsu) && (gauge.Kaeshi == Kaeshi.GOKEN || gauge.Kaeshi == Kaeshi.SETSUGEKKA))
                                return OriginalHook(TsubameGaeshi);
                        }
                    }

                    if (InCombat())
                    {
                        if (inOpener && IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Opener) && level == 90 && !hasDied && !nonOpener)
                        {
                            //oGCDs
                            if (CanSpellWeave(actionID))
                            {
                                if (gauge.Kaeshi == Kaeshi.NAMIKIRI && gauge.MeditationStacks == 3)
                                    return Shoha;

                                if (twoSeal && gauge.MeditationStacks == 0 && GetCooldownRemainingTime(Ikishoten) < 110 && IsOnCooldown(Ikishoten))
                                {
                                    if (gauge.Kenki >= 10 && IsOffCooldown(Gyoten))
                                        return Gyoten;

                                    if (gauge.Kenki >= 25)
                                        return Shinten;
                                }

                                if (twoSeal && IsOffCooldown(Ikishoten))
                                    return Ikishoten;

                                if (gauge.Kenki >= 25)
                                {
                                    if (oneSeal && GetRemainingCharges(MeikyoShisui) == 0 && oneSeal)
                                        return Shinten;

                                    if (GetRemainingCharges(MeikyoShisui) == 1 && IsOffCooldown(Senei) && (gauge.Kaeshi == Kaeshi.SETSUGEKKA || gauge.Sen == Sen.NONE))
                                        return Senei;
                                }

                                if (gauge.Sen == Sen.NONE && GetRemainingCharges(MeikyoShisui) == 1)
                                    return MeikyoShisui;

                                if (gauge.Kenki >= 25 && IsOnCooldown(Shoha))
                                    return Shinten;
                            }

                            //GCDs
                            if ((twoSeal && lastComboMove == Yukikaze) ||
                                (threeSeal && (GetRemainingCharges(MeikyoShisui) == 1 || !HasEffect(Buffs.OgiNamikiriReady))) ||
                                (oneSeal && !TargetHasEffect(Debuffs.Higanbana) && GetRemainingCharges(TsubameGaeshi) == 1))
                                return OriginalHook(Iaijutsu);

                            if ((gauge.Kaeshi == Kaeshi.NAMIKIRI) ||
                                (oneSeal && TargetHasEffect(Debuffs.Higanbana) && HasEffect(Buffs.OgiNamikiriReady)))
                                return OriginalHook(OgiNamikiri);

                            if (gauge.Kaeshi == Kaeshi.SETSUGEKKA || gauge.Kaeshi == Kaeshi.GOKEN)
                                return OriginalHook(TsubameGaeshi);

                            //1-2-3 Logic
                            if (lastComboMove == Hakaze)
                                return Yukikaze;

                            if (twoSeal && gauge.MeditationStacks == 0 && TargetHasEffect(Debuffs.Higanbana))
                                return Hakaze;

                            if (meikyostacks == 3)
                                return Gekko;

                            if (meikyostacks == 2)
                                return Kasha;

                            if (meikyostacks == 1)
                            {
                                if (GetCooldownRemainingTime(Ikishoten) > 110)
                                    return Yukikaze;

                                if (gauge.MeditationStacks == 0 || !HasEffect(Buffs.OgiNamikiriReady))
                                    return Gekko;
                            }

                            if (GetRemainingCharges(TsubameGaeshi) == 0)
                                inOpener = false;

                            if (lastComboMove == Yukikaze && oneSeal)
                            {
                                inOpener = false;
                                nonOpener = true;
                            }
                        }

                        if (!inOpener)
                        {
                            //Death desync check
                            if (HasEffect(All.Buffs.Weakness))
                                hasDied = true;

                            //Filler Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_FillerCombos) && !hasDied && !nonOpener && level == 90)
                            {
                                bool oddMinute = GetCooldownRemainingTime(Ikishoten) < 60 && gauge.Sen == Sen.NONE && !meikyoBuff && GetDebuffRemainingTime(Debuffs.Higanbana) > 45;
                                bool evenMinute = !meikyoBuff && GetCooldownRemainingTime(Ikishoten) > 60 && gauge.Sen == Sen.NONE && GetRemainingCharges(TsubameGaeshi) == 0 && GetDebuffRemainingTime(Debuffs.Higanbana) > 42 && gauge.Kenki > 15;

                                if (GetDebuffRemainingTime(Debuffs.Higanbana) < 40)
                                {
                                    if (inOddFiller || inEvenFiller)
                                    {
                                        inOddFiller = false;
                                        inEvenFiller = false;
                                    }
                                }

                                if (!inEvenFiller && evenMinute)
                                    inEvenFiller = true;

                                if (inEvenFiller)
                                {
                                    if (hasDied || IsOnCooldown(Hagakure) || (InMeleeRange() && !HasEffect(Buffs.EnhancedEnpi)))
                                        inEvenFiller = false;

                                    if (SamFillerCombo == 2)
                                    {
                                        if (!InMeleeRange() && !HasEffect(Buffs.EnhancedEnpi) && gauge.Kenki >= 10)
                                            return Gyoten;

                                        if (HasEffect(Buffs.EnhancedEnpi))
                                            return Enpi;

                                        if (gauge.Sen == 0 && gauge.Kenki >= 10)
                                            return Yaten;
                                    }

                                    if (SamFillerCombo == 3)
                                    {
                                        if (gauge.Kenki >= 75 && CanWeave(actionID))
                                            return Shinten;

                                        if (gauge.Sen == Sen.SETSU)
                                            return Hagakure;

                                        if (lastComboMove == Hakaze)
                                            return Yukikaze;

                                        if (gauge.Sen == 0)
                                            return Hakaze;
                                    }

                                }

                                if (!inOddFiller && oddMinute)
                                    inOddFiller = true;

                                if (inOddFiller)
                                {
                                    if (hasDied || IsOnCooldown(Hagakure))
                                        inOddFiller = false;

                                    if (SamFillerCombo == 1)
                                    {
                                        if (gauge.Kenki >= 75 && CanWeave(actionID))
                                            return Shinten;

                                        if (gauge.Sen == Sen.SETSU)
                                            return Hagakure;

                                        if (lastComboMove == Hakaze)
                                            return Yukikaze;

                                        if (gauge.Sen == 0)
                                            return Hakaze;
                                    }

                                    if (SamFillerCombo == 2)
                                    {
                                        if (gauge.Kenki >= 75 && CanWeave(actionID))
                                            return Shinten;

                                        if (gauge.Sen == Sen.GETSU)
                                            return Hagakure;

                                        if (lastComboMove == Jinpu)
                                            return Gekko;

                                        if (lastComboMove == Hakaze)
                                            return Jinpu;

                                        if (gauge.Sen == 0)
                                            return Hakaze;
                                    }

                                    if (SamFillerCombo == 3)
                                    {
                                        if (!InMeleeRange() && !HasEffect(Buffs.EnhancedEnpi) && gauge.Kenki >= 10)
                                            return Gyoten;

                                        if (gauge.Kenki >= 75 && CanWeave(actionID))
                                            return Shinten;

                                        if (gauge.Sen == Sen.GETSU)
                                            return Hagakure;

                                        if (lastComboMove == Jinpu)
                                            return Gekko;

                                        if (lastComboMove == Hakaze)
                                            return Jinpu;

                                        if (InMeleeRange() && !HasEffect(Buffs.EnhancedEnpi) && IsOnCooldown(Gyoten))
                                            return Hakaze;

                                        if (HasEffect(Buffs.EnhancedEnpi))
                                            return Enpi;

                                        if (gauge.Sen == 0 && gauge.Kenki >= 10)
                                            return Yaten;
                                    }
                                }
                            }

                            //Meikyo Waste Protection (Stops waste during even minute windows)
                            if (meikyoBuff && GetBuffRemainingTime(Buffs.MeikyoShisui) < 6 && HasEffect(Buffs.OgiNamikiriReady))
                            {
                                if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                    return Gekko;

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && gauge.Sen.HasFlag(Sen.KA) == false)
                                    return Kasha;

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && gauge.Sen.HasFlag(Sen.SETSU) == false)
                                    return Yukikaze;
                            }

                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs))
                            {
                                //oGCDs
                                if (CanSpellWeave(actionID))
                                {
                                    //Senei Features
                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei) && gauge.Kenki >= 25 && IsOffCooldown(Senei) && level >= Levels.Senei)
                                    {
                                        if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei_Burst))
                                            return Senei;

                                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei_Burst))
                                        {
                                            if (hasDied || nonOpener || GetCooldownRemainingTime(Ikishoten) <= 100 || ((gauge.Kaeshi == Kaeshi.SETSUGEKKA || gauge.Sen == Sen.NONE) && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10))
                                                return Senei;
                                        }
                                    }

                                    if (level >= Levels.Shinten && gauge.Kenki >= 25)
                                    {
                                        if (GetCooldownRemainingTime(Senei) > 110 || (IsEnabled(CustomComboPreset.SAM_ST_Overcap) && gauge.Kenki >= SamKenkiOvercapAmount))
                                            return Shinten;
                                    }

                                    //Ikishoten Features
                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Ikishoten) && level >= Levels.Ikishoten)
                                    {
                                        //Dumps Kenki in preparation for Ikishoten
                                        if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                            return Shinten;

                                        if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                            return Ikishoten;
                                    }

                                    //Meikyo Features
                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui) && level >= Levels.MeikyoShisui && !meikyoBuff && GetRemainingCharges(MeikyoShisui) > 0)
                                    {
                                        if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui_Burst))
                                            return MeikyoShisui;

                                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui_Burst))
                                        {
                                            if (hasDied || nonOpener || GetRemainingCharges(MeikyoShisui) == 2 || (gauge.Kaeshi == Kaeshi.NONE && gauge.Sen == Sen.NONE && GetDebuffRemainingTime(Debuffs.Higanbana) <= 15))
                                                return MeikyoShisui;
                                        }
                                    }

                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Shoha) && level >= Levels.Shoha && gauge.MeditationStacks == 3)
                                        return Shoha;
                                }

                                // Iaijutsu Features
                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Iaijutsu) && level >= Levels.Higanbana)
                                {
                                    if (gauge.Kaeshi == Kaeshi.SETSUGEKKA && level >= Levels.TsubameGaeshi && GetRemainingCharges(TsubameGaeshi) > 0)
                                        return OriginalHook(TsubameGaeshi);

                                    if (!this.IsMoving)
                                    {
                                        if (((oneSeal || (oneSeal && meikyostacks == 2)) && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10) ||
                                            (twoSeal && level < Levels.Setsugekka) ||
                                            (threeSeal && level >= Levels.Setsugekka))
                                            return OriginalHook(Iaijutsu);
                                    }
                                }

                                //Ogi Namikiri Features
                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri) && level >= Levels.OgiNamikiri)
                                {
                                    if ((!this.IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                                    {
                                        if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri_Burst))
                                            return OriginalHook(OgiNamikiri);

                                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri_Burst))
                                        {
                                            if (hasDied || nonOpener || (meikyostacks == 1 && GetDebuffRemainingTime(Debuffs.Higanbana) >= 45 && HasEffect(Buffs.MeikyoShisui)) || GetCooldownRemainingTime(Ikishoten) <= 105)
                                                return OriginalHook(OgiNamikiri);
                                        }
                                    }
                                }
                            }

                            if (HasEffect(Buffs.MeikyoShisui))
                            {
                                if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                    return Gekko;

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && gauge.Sen.HasFlag(Sen.KA) == false)
                                    return Kasha;

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && gauge.Sen.HasFlag(Sen.SETSU) == false)
                                    return Yukikaze;
                            }

                            if (comboTime > 0)
                            {
                                if (lastComboMove == Hakaze && level >= Levels.Jinpu)
                                {
                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && 
                                        gauge.Sen.HasFlag(Sen.SETSU) == false && 
                                        level >= Levels.Yukikaze && 
                                        HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                                        return Yukikaze;

                                    if ((level < Levels.Kasha && ((GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))) || 
                                        (level >= Levels.Kasha && gauge.Sen.HasFlag(Sen.GETSU) == false))
                                        return Jinpu;

                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && LevelChecked(Shifu) &&
                                        ((level < Levels.Kasha && ((GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka))) || (level >= Levels.Kasha && gauge.Sen.HasFlag(Sen.KA) == false)))
                                        return Shifu;

                                    return Jinpu;
                                }

                                if (lastComboMove == Jinpu && level >= Levels.Gekko)
                                    return Gekko;

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && lastComboMove == Shifu && level >= Levels.Kasha)
                                    return Kasha;
                            }
                        }
                    }
                    return Hakaze;
                }

                return actionID;
            }
        }

        internal class SAM_ST_KashaCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_KashaCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var SamKenkiOvercapAmount = PluginConfiguration.GetCustomIntValue(Config.SAM_ST_KenkiOvercapAmount);

                if (actionID == Kasha)
                {
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_TrueNorth) && GetBuffStacks(Buffs.MeikyoShisui) > 0 && !HasEffect(All.Buffs.TrueNorth) && GetRemainingCharges(All.TrueNorth) > 0)
                            return All.TrueNorth;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Overcap) && gauge.Kenki >= SamKenkiOvercapAmount && level >= Levels.Shinten)
                            return Shinten;
                    }
                    if (HasEffect(Buffs.MeikyoShisui))
                        return Kasha;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Hakaze && level >= Levels.Shifu)
                            return Shifu;

                        if (lastComboMove == Shifu && level >= Levels.Kasha)
                            return Kasha;
                    }

                    return Hakaze;
                }

                return actionID;
            }
        }

        internal class SAM_AoE_MangetsuCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_MangetsuCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Mangetsu)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamAOEKenkiOvercapAmount = PluginConfiguration.GetCustomIntValue(Config.SAM_AoE_KenkiOvercapAmount);

                    //oGCD Features
                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Guren) && IsOffCooldown(Guren) && level >= Levels.Guren && gauge.Kenki >= 25)
                            return Guren;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Ikishoten) && gauge.Kenki <= 50 && IsOffCooldown(Ikishoten) && level >= Levels.Ikishoten)
                            return Ikishoten;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Overcap) && gauge.Kenki >= SamAOEKenkiOvercapAmount && level >= Levels.Kyuten)
                            return Kyuten;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Shoha2) && level >= Levels.Shoha2 && gauge.MeditationStacks == 3)
                            return Shoha2;
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_OgiNamikiri) && level >= Levels.OgiNamikiri)
                    {
                        if ((!this.IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                            return OriginalHook(OgiNamikiri);
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_TenkaGoken) && level >= Levels.TenkaGoken)
                    {
                        if (!this.IsMoving && (OriginalHook(Iaijutsu) == TenkaGoken || (OriginalHook(Iaijutsu) == Setsugekka && level >= Levels.Setsugekka)))
                            return OriginalHook(Iaijutsu);

                        if ((gauge.Kaeshi == Kaeshi.GOKEN || gauge.Kaeshi == Kaeshi.SETSUGEKKA) && level >= Levels.TsubameGaeshi && GetRemainingCharges(TsubameGaeshi) > 0)
                            return OriginalHook(TsubameGaeshi);
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                            return Mangetsu;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) && gauge.Sen.HasFlag(Sen.KA) == false)
                            return Oka;
                    }

                    if (comboTime > 0)
                    {
                        if (level >= Levels.Mangetsu && (lastComboMove == Fuko || lastComboMove == Fuga))
                        {
                            if (IsNotEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) ||
                                gauge.Sen.HasFlag(Sen.GETSU) == false || GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                                return Mangetsu;

                            if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) && level >= Levels.Oka &&
                                (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                                return Oka;
                        }
                    }

                    if (level is < Levels.Oka and >= Levels.Kasha)
                    {
                        if (lastComboMove == Shifu)
                            return Kasha;

                        if (lastComboMove == Hakaze)
                            return Shifu;

                        if (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka))
                            return Hakaze;
                    }

                    return OriginalHook(Fuko);
                }

                return actionID;
            }
        }

        internal class SAM_AoE_OkaCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_OkaCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Oka)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamAOEKenkiOvercapAmount = PluginConfiguration.GetCustomIntValue(Config.SAM_AoE_KenkiOvercapAmount);

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_AoE_Overcap) && IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget) && gauge.Kenki >= SamAOEKenkiOvercapAmount && level >= Levels.Kyuten)
                            return Kyuten;
                    }

                    if (HasEffect(Buffs.MeikyoShisui) && IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget))
                        return Oka;

                    //Two Target Rotation
                    if (IsEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget))
                    {
                        if (CanSpellWeave(actionID))
                        {
                            if (level >= Levels.Senei && gauge.Kenki >= 25 && IsOffCooldown(Senei))
                                return Senei;

                            if (level >= Levels.Shinten && gauge.Kenki >= 25)
                                return Shinten;

                            if (level >= Levels.Shoha && gauge.MeditationStacks == 3)
                                return Shoha;
                        }

                        if (HasEffect(Buffs.MeikyoShisui))
                        {
                            if (gauge.Sen.HasFlag(Sen.SETSU) == false && level >= Levels.Yukikaze)
                                return Yukikaze;

                            if (gauge.Sen.HasFlag(Sen.GETSU) == false && level >= Levels.Gekko)
                                return Gekko;

                            if (gauge.Sen.HasFlag(Sen.KA) == false && level >= Levels.Kasha)
                                return Kasha;
                        }

                        if (level >= Levels.TsubameGaeshi && gauge.Kaeshi == Kaeshi.SETSUGEKKA && GetRemainingCharges(TsubameGaeshi) > 0)
                            return OriginalHook(TsubameGaeshi);

                        if (level >= Levels.Setsugekka && OriginalHook(Iaijutsu) == Setsugekka)
                            return OriginalHook(Iaijutsu);

                        if (comboTime > 0)
                        {
                            if (lastComboMove == Hakaze && level >= Levels.Yukikaze)
                                return Yukikaze;

                            if (lastComboMove is Fuko or Fuga && gauge.Sen.HasFlag(Sen.GETSU) == false && level >= Levels.Mangetsu)
                                return Mangetsu;
                        }

                        if (gauge.Sen.HasFlag(Sen.SETSU) == false)
                            return Hakaze;
                    }

                    if (comboTime > 0 && level >= Levels.Oka)
                    {
                        if (lastComboMove == Fuko || lastComboMove == Fuga)
                            return Oka;
                    }

                    return OriginalHook(Fuko);
                }

                return actionID;
            }
        }

        internal class SAM_JinpuShifu : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SAM_JinpuShifu;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();

                if (actionID == MeikyoShisui)
                {
                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (!HasEffect(Buffs.Fugetsu) || gauge.Sen.HasFlag(Sen.GETSU) == false)
                            return Gekko;

                        if (!HasEffect(Buffs.Fuka) || gauge.Sen.HasFlag(Sen.KA) == false)
                            return Kasha;

                        if (gauge.Sen.HasFlag(Sen.SETSU) == false)
                            return Yukikaze;
                    }

                    return MeikyoShisui;
                }

                return actionID;
            }
        }

        internal class SAM_Iaijutsu : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Iaijutsu;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (actionID == Iaijutsu)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_Shoha) && level >= Levels.Shoha && gauge.MeditationStacks >= 3 && CanSpellWeave(actionID))
                        return Shoha;

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_OgiNamikiri) && level >= Levels.OgiNamikiri && (gauge.Kaeshi == Kaeshi.NAMIKIRI || HasEffect(Buffs.OgiNamikiriReady)))
                        return OriginalHook(OgiNamikiri);

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_TsubameGaeshi) && level >= Levels.TsubameGaeshi && gauge.Kaeshi != Kaeshi.NONE)
                        return OriginalHook(TsubameGaeshi);
                }

                return actionID;
            }
        }

        internal class SAM_Shinten_Shoha : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Shinten_Shoha;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (actionID == Shinten)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Shinten_Shoha_Senei) && IsOffCooldown(Senei) && level >= Levels.Senei)
                        return Senei;

                    if (gauge.MeditationStacks >= 3 && level >= Levels.Shoha)
                        return Shoha;
                }

                return actionID;
            }
        }

        internal class SAM_Kyuten_Shoha2_Guren : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Kyuten_Shoha2_Guren;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (actionID == Kyuten)
                {
                    if (IsOffCooldown(Guren) && level >= Levels.Guren)
                        return Guren;

                    if (IsEnabled(CustomComboPreset.SAM_Kyuten_Shoha2) && gauge.MeditationStacks == 3 && level >= Levels.Shoha2)
                        return Shoha2;
                }

                return actionID;
            }
        }

        internal class SAM_Ikishoten_OgiNamikiri : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Ikishoten_OgiNamikiri;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Ikishoten)
                {
                    if (level >= Levels.OgiNamikiri)
                    {
                        if (HasEffect(Buffs.OgiNamikiriReady))
                        {
                            if (HasEffect(Buffs.OgiNamikiriReady))
                                return OgiNamikiri;
                        }

                        if (OriginalHook(OgiNamikiri) == KaeshiNamikiri)
                            return KaeshiNamikiri;
                    }

                    return Ikishoten;
                }

                return actionID;
            }
        }

        internal class SAM_GyotenYaten : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_GyotenYaten;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Gyoten)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    if (gauge.Kenki >= 10)
                    {
                        if (InMeleeRange())
                            return Yaten;

                        if (!InMeleeRange())
                            return Gyoten;
                    }
                }

                return actionID;
            }
        }
    }
}

