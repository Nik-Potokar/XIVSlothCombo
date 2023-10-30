using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class SAM
    {
        public const byte JobID = 34;

        public static int NumSen(SAMGauge gauge)
        {
            bool ka = gauge.Sen.HasFlag(Sen.KA);
            bool getsu = gauge.Sen.HasFlag(Sen.GETSU);
            bool setsu = gauge.Sen.HasFlag(Sen.SETSU);
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
            Shoha = 16487,
            Shoha2 = 25779,
            Ikishoten = 16482,
            Fuko = 25780,
            Yaten = 7493,
            Gyoten = 7492,
            OgiNamikiri = 25781,
            KaeshiHiganbana = 16484,
            KaeshiSetsugekka = 16486,
            KaeshiGoken = 16485,
            KaeshiNamikiri = 25782;

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

        public static class Config
        {
            public static UserInt
                SAM_ST_KenkiOvercapAmount = new("SamKenkiOvercapAmount"),
                SAM_AoE_KenkiOvercapAmount = new("SamAOEKenkiOvercapAmount"),
                SAM_ST_Higanbana_Threshold = new("SAM_ST_Higanbana_Threshold"),
                SAM_FillerCombo = new("SamFillerCombo"),
                SAM_STSecondWindThreshold = new("SAM_STSecondWindThreshold"),
                SAM_STBloodbathThreshold = new("SAM_STBloodbathThreshold"),
                SAM_AoESecondWindThreshold = new("SAM_AoESecondWindThreshold"),
                SAM_AoEBloodbathThreshold = new("SAM_AoEBloodbathThreshold"),
                SAM_ST_ExecuteThreshold = new("SAM_ST_ExecuteThreshold"),
                 SAM_MeikyoChoice = new("SAM_MeikyoChoice"),
                SAM_VariantCure = new("SAM_VariantCure");
        }
        internal class SAM_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_AdvancedMode;
            internal static SAMOpenerLogic SAMOpener = new();
            internal static bool inOddFiller = false;
            internal static bool inEvenFiller = false;
            internal static bool fillerComplete = false;
            internal static bool fastFillerReady = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                int SamKenkiOvercapAmount = Config.SAM_ST_KenkiOvercapAmount;
                bool oneSeal = OriginalHook(Iaijutsu) == Higanbana;
                bool twoSeal = OriginalHook(Iaijutsu) == TenkaGoken;
                bool threeSeal = OriginalHook(Iaijutsu) == Setsugekka;
                float meikyostacks = GetBuffStacks(Buffs.MeikyoShisui);
                int SamFillerCombo = Config.SAM_FillerCombo;
                int SamMeikyoChoice = Config.SAM_MeikyoChoice;
                int HiganbanaThreshold = Config.SAM_ST_Higanbana_Threshold;
                int executeThreshold = Config.SAM_ST_ExecuteThreshold;
                float enemyHP = GetTargetHPPercent();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);
                bool oddMinute = CombatEngageDuration().Minutes % 2 == 1 && gauge.Sen == Sen.NONE && !HasEffect(Buffs.MeikyoShisui) && GetDebuffRemainingTime(Debuffs.Higanbana) >= 48 && GetDebuffRemainingTime(Debuffs.Higanbana) <= 51;
                bool evenMinute = !HasEffect(Buffs.MeikyoShisui) && CombatEngageDuration().Minutes % 2 == 0 && gauge.Sen == Sen.NONE && GetRemainingCharges(TsubameGaeshi) == 0 && GetDebuffRemainingTime(Debuffs.Higanbana) >= 44 && GetDebuffRemainingTime(Debuffs.Higanbana) <= 47;


                if (actionID is Gekko)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Opener))
                    {
                        if (SAMOpener.DoFullOpener(ref actionID, false))
                            return actionID;
                    }

                    //Stops waste if you use Iaijutsu or Ogi and you've got a Kaeshi ready
                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri) &&
                        (gauge.Kaeshi == Kaeshi.NAMIKIRI) && OgiNamikiri.LevelChecked())
                        return OriginalHook(OgiNamikiri);

                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Iaijutsu) &&
                        ActionReady(TsubameGaeshi) && (gauge.Kaeshi == Kaeshi.GOKEN || gauge.Kaeshi == Kaeshi.SETSUGEKKA))
                        return OriginalHook(TsubameGaeshi);

                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_RangedUptime) &&
                        Enpi.LevelChecked() && !inEvenFiller && !inOddFiller && !InMeleeRange() && HasBattleTarget())
                        return Enpi;
                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_TrueNorth) &&
                            trueNorthReady && GetBuffStacks(Buffs.MeikyoShisui) > 0)
                            return All.TrueNorth;

                        if (gauge.Kaeshi == Kaeshi.NAMIKIRI && gauge.MeditationStacks == 3)
                            return Shoha;

                        if (twoSeal && gauge.MeditationStacks == 0 && GetCooldownRemainingTime(Ikishoten) < 110 && IsOnCooldown(Ikishoten))
                        {
                            if (gauge.Kenki >= 25)
                                return Shinten;

                            if (gauge.Kenki >= 10 && IsOffCooldown(Gyoten))
                                return Gyoten;
                        }

                        if (twoSeal && ActionReady(Ikishoten))
                            return Ikishoten;

                        if (gauge.Kenki >= 25)
                        {
                            if (oneSeal && !HasCharges(MeikyoShisui))
                                return Shinten;

                            if (GetRemainingCharges(MeikyoShisui) == 1 && IsOffCooldown(Senei) && (gauge.Kaeshi == Kaeshi.SETSUGEKKA || gauge.Sen == Sen.NONE))
                                return Senei;
                        }

                        if (gauge.Sen == Sen.NONE && GetRemainingCharges(MeikyoShisui) == 1 &&
                            GetRemainingCharges(TsubameGaeshi) == 1 && !HasEffect(Buffs.MeikyoShisui))
                            return MeikyoShisui;

                        if (gauge.Kenki >= 25 && IsOnCooldown(Shoha))
                            return Shinten;
                    }

                    //GCDs
                    if ((twoSeal && lastComboMove == Yukikaze) ||
                        (threeSeal && (GetRemainingCharges(MeikyoShisui) == 1 || !HasEffect(Buffs.OgiNamikiriReady))) ||
                        (oneSeal && !TargetHasEffect(Debuffs.Higanbana) && GetRemainingCharges(TsubameGaeshi) == 1 && enemyHP > HiganbanaThreshold))
                        return OriginalHook(Iaijutsu);

                    if ((gauge.Kaeshi == Kaeshi.NAMIKIRI) ||
                        (WasLastWeaponskill(Higanbana) && HasEffect(Buffs.OgiNamikiriReady)))
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

                    if (meikyostacks == 2 && !HasEffect(Buffs.OgiNamikiriReady) && gauge.Kaeshi == Kaeshi.NONE)
                        return Kasha;

                    if (meikyostacks == 1)
                    {
                        if (GetCooldownRemainingTime(Ikishoten) > 110)
                            return Yukikaze;

                        if (gauge.MeditationStacks == 0 || !HasEffect(Buffs.OgiNamikiriReady))
                            return Gekko;
                    }

                    //Filler Features
                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_FillerCombos) &&
                        ActionReady(OgiNamikiri) && InCombat())
                    {
                        if (GetDebuffRemainingTime(Debuffs.Higanbana) < 42)
                        {
                            if (inOddFiller || inEvenFiller)
                            {
                                inOddFiller = false;
                                inEvenFiller = false;
                                fillerComplete = false;
                                fastFillerReady = false;
                            }
                        }

                        if (!inEvenFiller && evenMinute)
                            inEvenFiller = true;

                        if (inEvenFiller)
                        {
                            if (fillerComplete)
                            {
                                inEvenFiller = false;
                                fillerComplete = false;
                            }

                            if (SamFillerCombo == 2)
                            {
                                if (WasLastAbility(Gyoten))
                                    fillerComplete = true;

                                if (WasLastAction(Enpi) && IsOffCooldown(Gyoten))
                                    return Gyoten;

                                if (WasLastAction(Yaten))
                                    return Enpi;

                                if (gauge.Sen == 0 && gauge.Kenki >= 10 && CanSpellWeave(actionID) && IsOffCooldown(Yaten))
                                    return Yaten;
                            }

                            if (SamFillerCombo == 3)
                            {
                                if (WasLastAbility(Hagakure))
                                    fillerComplete = true;

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
                            if (fillerComplete)
                            {
                                fastFillerReady = false;
                                inOddFiller = false;
                                fillerComplete = false;
                            }

                            if (SamFillerCombo == 1)
                            {
                                if (WasLastAbility(Hagakure))
                                    fillerComplete = true;

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
                                if (WasLastAbility(Hagakure))
                                    fillerComplete = true;

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
                                if (WasLastAbility(Hagakure))
                                    fillerComplete = true;
                                if (WasLastWeaponskill(Hakaze) && gauge.Sen == Sen.SETSU)
                                    fastFillerReady = true;

                                if (gauge.Kenki >= 75 && CanWeave(actionID))
                                    return Shinten;

                                if (gauge.Sen == Sen.SETSU && WasLastWeaponskill(Yukikaze) && fastFillerReady)
                                    return Hagakure;

                                if (lastComboMove == Hakaze)
                                    return Yukikaze;

                                if (gauge.Sen == 0 || gauge.Sen == Sen.SETSU)
                                    return Hakaze;
                            }
                        }
                    }

                    //Meikyo Waste Protection (Stops waste during even minute windows)
                    if (HasEffect(Buffs.MeikyoShisui) && GetBuffRemainingTime(Buffs.MeikyoShisui) < 6 && HasEffect(Buffs.OgiNamikiriReady))
                    {
                        if (!gauge.Sen.HasFlag(Sen.GETSU) && Gekko.LevelChecked())
                            return Gekko;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && gauge.Sen.HasFlag(Sen.KA) == false && Kasha.LevelChecked())
                            return Kasha;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && gauge.Sen.HasFlag(Sen.SETSU) == false && Yukikaze.LevelChecked())
                            return Yukikaze;
                    }

                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs))
                    {
                        //oGCDs
                        if (CanWeave(actionID))
                        {
                            //Meikyo Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui) && MeikyoShisui.LevelChecked() && !HasEffect(Buffs.MeikyoShisui) && GetRemainingCharges(MeikyoShisui) > 0)
                            {
                                if (CombatEngageDuration().TotalSeconds <= 10 ||
                                    (CombatEngageDuration().TotalSeconds > 10 &&
                                    ((SamMeikyoChoice is 0 or 1 && !WasLastWeaponskill(Shifu) && !WasLastWeaponskill(Jinpu) && !(lastComboMove is Hakaze && !gauge.Sen.HasFlag(Sen.SETSU) && HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))) ||
                                    (SamMeikyoChoice is 2 && (comboTime is 0.0f || WasLastWeaponskill(Yukikaze))))))
                                {
                                    if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui_Burst))
                                        return MeikyoShisui;
                                }

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_MeikyoShisui_Burst))
                                {
                                    if (GetRemainingCharges(MeikyoShisui) == 2 || (gauge.Kaeshi == Kaeshi.NONE && gauge.Sen == Sen.NONE && GetDebuffRemainingTime(Debuffs.Higanbana) <= 15))
                                        return MeikyoShisui;
                                }
                            }

                            //Senei Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei) && gauge.Kenki >= 25 && IsOffCooldown(Senei))
                            {
                                if (Shinten.LevelChecked() && !Senei.LevelChecked() && IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei_Burst))
                                    return Shinten;

                                if (Senei.LevelChecked())
                                {
                                    if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei_Burst))
                                        return Senei;

                                    if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Senei_Burst))
                                    {
                                        if (GetCooldownRemainingTime(Ikishoten) <= 100 || ((gauge.Kaeshi == Kaeshi.SETSUGEKKA || gauge.Sen == Sen.NONE) && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10))
                                            return Senei;
                                    }
                                }
                            }

                            if (Shinten.LevelChecked() && gauge.Kenki >= 25)
                            {
                                if (GetCooldownRemainingTime(Senei) > 110 || (IsEnabled(CustomComboPreset.SAM_ST_Overcap) && gauge.Kenki >= SamKenkiOvercapAmount) || (IsEnabled(CustomComboPreset.SAM_ST_Execute) && GetTargetHPPercent() <= executeThreshold))
                                    return Shinten;
                            }

                            //Ikishoten Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Ikishoten) && Ikishoten.LevelChecked())
                            {
                                //Dumps Kenki in preparation for Ikishoten
                                if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                    return Shinten;

                                if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                    return Ikishoten;
                            }

                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Shoha) && Shoha.LevelChecked() && gauge.MeditationStacks == 3)
                                return Shoha;
                        }

                        // Iaijutsu Features
                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_Iaijutsu) && Higanbana.LevelChecked())
                        {
                            if (gauge.Kaeshi == Kaeshi.SETSUGEKKA && TsubameGaeshi.LevelChecked() && GetRemainingCharges(TsubameGaeshi) > 0)
                                return OriginalHook(TsubameGaeshi);

                            if (!IsMoving)
                            {
                                if (((oneSeal || (oneSeal && meikyostacks == 2)) && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10 && enemyHP > HiganbanaThreshold) ||
                                    (twoSeal && !Setsugekka.LevelChecked()) ||
                                    (threeSeal && Setsugekka.LevelChecked()))
                                    return OriginalHook(Iaijutsu);
                            }
                        }

                        //Ogi Namikiri Features
                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri) && OgiNamikiri.LevelChecked())
                        {
                            if ((!IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                            {
                                if (IsNotEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri_Burst))
                                    return OriginalHook(OgiNamikiri);

                                if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_CDs_OgiNamikiri_Burst))
                                {
                                    if ((meikyostacks is 1 or 2 && GetDebuffRemainingTime(Debuffs.Higanbana) >= 45 && HasEffect(Buffs.MeikyoShisui)) || GetCooldownRemainingTime(Ikishoten) <= 105)
                                        return OriginalHook(OgiNamikiri);
                                }
                            }
                        }
                    }

                    // healing - please move if not appropriate this high priority
                    if (IsEnabled(CustomComboPreset.SAM_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.SAM_STSecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.SAM_STBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (!HasEffect(Buffs.Fugetsu) || (!gauge.Sen.HasFlag(Sen.GETSU) && HasEffect(Buffs.Fuka)))
                            return Gekko;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && ((!gauge.Sen.HasFlag(Sen.KA) && HasEffect(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka)))
                            return Kasha;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && !gauge.Sen.HasFlag(Sen.SETSU))
                            return Yukikaze;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Hakaze && Jinpu.LevelChecked())
                        {
                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Yukikaze) && !gauge.Sen.HasFlag(Sen.SETSU) && Yukikaze.LevelChecked() && HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                                return Yukikaze;

                            if ((!Kasha.LevelChecked() && ((GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))) ||
                               (Kasha.LevelChecked() && (!HasEffect(Buffs.Fugetsu) || (HasEffect(Buffs.Fuka) && !gauge.Sen.HasFlag(Sen.GETSU)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka))))))
                                return Jinpu;

                            if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && LevelChecked(Shifu) &&
                                ((!Kasha.LevelChecked() && ((GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka))) ||
                                (Kasha.LevelChecked() && (!HasEffect(Buffs.Fuka) || (HasEffect(Buffs.Fugetsu) && !gauge.Sen.HasFlag(Sen.KA)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)))))))
                                return Shifu;
                        }

                        if (lastComboMove == Jinpu && Gekko.LevelChecked())
                            return Gekko;

                        if (IsEnabled(CustomComboPreset.SAM_ST_GekkoCombo_Kasha) && lastComboMove == Shifu && Kasha.LevelChecked())
                            return Kasha;
                    }

                    return Hakaze;
                }
                return actionID;
            }
        }
        internal class SAM_ST_YukikazeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_YukikazeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Yukikaze)
                {
                    SAMGauge? gauge = GetJobGauge<SAMGauge>();
                    int SamKenkiOvercapAmount = Config.SAM_ST_KenkiOvercapAmount;
                    bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_TrueNorth) &&
                            trueNorthReady && GetBuffStacks(Buffs.MeikyoShisui) > 0)
                            return All.TrueNorth;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Overcap) &&
                            gauge.Kenki >= SamKenkiOvercapAmount && Shinten.LevelChecked())
                            return Shinten;
                    }

                    if (HasEffect(Buffs.MeikyoShisui) && Yukikaze.LevelChecked())
                        return Yukikaze;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Hakaze && Yukikaze.LevelChecked())
                            return Yukikaze;
                    }

                    return Hakaze;
                }

                return actionID;
            }
        }

        internal class SAM_ST_KashaCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_KashaCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte levels)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                int SamKenkiOvercapAmount = Config.SAM_ST_KenkiOvercapAmount;
                bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

                if (actionID is Kasha)
                {
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_TrueNorth) &&
                            trueNorthReady && GetBuffStacks(Buffs.MeikyoShisui) > 0)
                            return All.TrueNorth;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Overcap) &&
                            gauge.Kenki >= SamKenkiOvercapAmount && Shinten.LevelChecked())
                            return Shinten;
                    }
                    if (HasEffect(Buffs.MeikyoShisui))
                        return Kasha;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Hakaze && Shifu.LevelChecked())
                            return Shifu;

                        if (lastComboMove == Shifu && Kasha.LevelChecked())
                            return Kasha;
                    }

                    return Hakaze;
                }

                return actionID;
            }
        }

        internal class SAM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Mangetsu)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamAOEKenkiOvercapAmount = Config.SAM_AoE_KenkiOvercapAmount;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    //oGCD Features
                    if (CanWeave(actionID))
                    {
                        if (OriginalHook(Iaijutsu) == Setsugekka && LevelChecked(Hagakure))
                            return Hagakure;

                        if (ActionReady(Guren) && gauge.Kenki >= 25)
                            return Guren;

                        if (LevelChecked(Ikishoten))
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                return Kyuten;

                            if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                return Ikishoten;
                        }

                        if (gauge.Kenki >= SamAOEKenkiOvercapAmount && LevelChecked(Kyuten))
                            return Kyuten;

                        if (LevelChecked(Shoha2) && gauge.MeditationStacks == 3)
                            return Shoha2;

                        if (LevelChecked(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui) && GetRemainingCharges(MeikyoShisui) > 0)
                            return MeikyoShisui;
                    }

                    if ((ActionReady(OgiNamikiri) && !IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                        return OriginalHook(OgiNamikiri);


                    if (TenkaGoken.LevelChecked())
                    {
                        if (!IsMoving && (OriginalHook(Iaijutsu) == TenkaGoken || (OriginalHook(Iaijutsu) == Setsugekka && Setsugekka.LevelChecked())))
                            return OriginalHook(Iaijutsu);

                        if (gauge.Kaeshi == Kaeshi.GOKEN && TsubameGaeshi.LevelChecked() && GetRemainingCharges(TsubameGaeshi) > 0)
                            return OriginalHook(TsubameGaeshi);
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if ((gauge.Sen.HasFlag(Sen.GETSU) == false && HasEffect(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))
                            return Mangetsu;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) && ((gauge.Sen.HasFlag(Sen.KA) == false && HasEffect(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka)))
                            return Oka;
                    }

                    if (comboTime > 0)
                    {
                        if (Mangetsu.LevelChecked() && (lastComboMove == Fuko || lastComboMove == Fuga))
                        {
                            if (IsNotEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) ||
                                gauge.Sen.HasFlag(Sen.GETSU) == false || GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                                return Mangetsu;

                            if (Oka.LevelChecked() && (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                                return Oka;
                        }
                    }

                    if (!Oka.LevelChecked() && Kasha.LevelChecked())
                    {
                        if (lastComboMove == Shifu && Kasha.LevelChecked())
                            return Kasha;

                        if (lastComboMove == Hakaze && Shifu.LevelChecked())
                            return Shifu;

                        if (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || (!HasEffect(Buffs.Fuka) && Hakaze.LevelChecked()))
                            return Hakaze;
                    }

                    return OriginalHook(Fuko);
                }

                return actionID;
            }
        }

        internal class SAM_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Mangetsu)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var SamAOEKenkiOvercapAmount = Config.SAM_AoE_KenkiOvercapAmount;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SAM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    //oGCD Features
                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Hagakure) && OriginalHook(Iaijutsu) == Setsugekka && LevelChecked(Hagakure))
                            return Hagakure;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Guren) && ActionReady(Guren) && gauge.Kenki >= 25)
                            return Guren;

                        if (IsEnabled(CustomComboPreset.SAM_AOE_GekkoCombo_CDs_Ikishoten) && LevelChecked(Ikishoten))
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                return Kyuten;

                            if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                return Ikishoten;
                        }

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Overcap) && gauge.Kenki >= SamAOEKenkiOvercapAmount && LevelChecked(Kyuten))
                            return Kyuten;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Shoha2) && LevelChecked(Shoha2) && gauge.MeditationStacks == 3)
                            return Shoha2;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_MeikyoShisui) && LevelChecked(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui) && GetRemainingCharges(MeikyoShisui) > 0)
                            return MeikyoShisui;

                        // healing - please move if not appropriate this high priority
                        if (IsEnabled(CustomComboPreset.SAM_AoE_ComboHeals))
                        {
                            if (PlayerHealthPercentageHp() <= Config.SAM_AoESecondWindThreshold && ActionReady(All.SecondWind))
                                return All.SecondWind;
                            if (PlayerHealthPercentageHp() <= Config.SAM_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                                return All.Bloodbath;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_OgiNamikiri) && OgiNamikiri.LevelChecked())
                    {
                        if ((!IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                            return OriginalHook(OgiNamikiri);
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_TenkaGoken) && TenkaGoken.LevelChecked())
                    {
                        if (!IsMoving && (OriginalHook(Iaijutsu) == TenkaGoken || (OriginalHook(Iaijutsu) == Setsugekka && Setsugekka.LevelChecked())))
                            return OriginalHook(Iaijutsu);

                        if (gauge.Kaeshi == Kaeshi.GOKEN && TsubameGaeshi.LevelChecked() && GetRemainingCharges(TsubameGaeshi) > 0)
                            return OriginalHook(TsubameGaeshi);
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if ((gauge.Sen.HasFlag(Sen.GETSU) == false && HasEffect(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))
                            return Mangetsu;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) && ((gauge.Sen.HasFlag(Sen.KA) == false && HasEffect(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka)))
                            return Oka;
                    }

                    if (comboTime > 0)
                    {
                        if (Mangetsu.LevelChecked() && (lastComboMove == Fuko || lastComboMove == Fuga))
                        {
                            if (IsNotEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) ||
                                gauge.Sen.HasFlag(Sen.GETSU) == false || GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                                return Mangetsu;

                            if (IsEnabled(CustomComboPreset.SAM_AoE_MangetsuCombo_Oka) && Oka.LevelChecked() &&
                                (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                                return Oka;
                        }
                    }

                    if (!Oka.LevelChecked() && Kasha.LevelChecked())
                    {
                        if (lastComboMove == Shifu && Kasha.LevelChecked())
                            return Kasha;

                        if (lastComboMove == Hakaze && Shifu.LevelChecked())
                            return Shifu;

                        if (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || (!HasEffect(Buffs.Fuka) && Hakaze.LevelChecked()))
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
                    var SamAOEKenkiOvercapAmount = Config.SAM_AoE_KenkiOvercapAmount;

                    if (IsEnabled(CustomComboPreset.SAM_AoE_Overcap) && IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget) && gauge.Kenki >= SamAOEKenkiOvercapAmount && Kyuten.LevelChecked() && CanWeave(actionID))
                        return Kyuten;

                    if (HasEffect(Buffs.MeikyoShisui) && IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget))
                        return Oka;

                    //Two Target Rotation
                    if (IsEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget))
                    {
                        if (CanSpellWeave(actionID))
                        {
                            if (!HasEffect(Buffs.MeikyoShisui) && GetRemainingCharges(MeikyoShisui) > 0 && MeikyoShisui.LevelChecked())
                                return MeikyoShisui;

                            if (Senei.LevelChecked() && gauge.Kenki >= 25 && IsOffCooldown(Senei))
                                return Senei;

                            if (Shinten.LevelChecked() && gauge.Kenki >= 25)
                                return Shinten;

                            if (Shoha.LevelChecked() && gauge.MeditationStacks == 3)
                                return Shoha;
                        }

                        if (HasEffect(Buffs.MeikyoShisui))
                        {
                            if (gauge.Sen.HasFlag(Sen.SETSU) == false && Yukikaze.LevelChecked())
                                return Yukikaze;

                            if (gauge.Sen.HasFlag(Sen.GETSU) == false && Gekko.LevelChecked())
                                return Gekko;

                            if (gauge.Sen.HasFlag(Sen.KA) == false && Kasha.LevelChecked())
                                return Kasha;
                        }

                        if (TsubameGaeshi.LevelChecked() && gauge.Kaeshi == Kaeshi.SETSUGEKKA && GetRemainingCharges(TsubameGaeshi) > 0)
                            return OriginalHook(TsubameGaeshi);

                        if (Setsugekka.LevelChecked() && OriginalHook(Iaijutsu) == Setsugekka)
                            return OriginalHook(Iaijutsu);

                        if (comboTime > 0)
                        {
                            if (lastComboMove == Hakaze && Yukikaze.LevelChecked())
                                return Yukikaze;

                            if (lastComboMove is Fuko or Fuga && gauge.Sen.HasFlag(Sen.GETSU) == false && Mangetsu.LevelChecked())
                                return Mangetsu;
                        }

                        if (gauge.Sen.HasFlag(Sen.SETSU) == false)
                            return Hakaze;
                    }
                    if (comboTime > 0 && Oka.LevelChecked())
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
                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_Shoha) && Shoha.LevelChecked() && gauge.MeditationStacks >= 3 && CanSpellWeave(actionID))
                        return Shoha;

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_OgiNamikiri) && OgiNamikiri.LevelChecked() && (gauge.Kaeshi == Kaeshi.NAMIKIRI || HasEffect(Buffs.OgiNamikiriReady)))
                        return OriginalHook(OgiNamikiri);

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_TsubameGaeshi) && TsubameGaeshi.LevelChecked() && gauge.Kaeshi != Kaeshi.NONE)
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
                    if (IsEnabled(CustomComboPreset.SAM_Shinten_Shoha_Senei) && IsOffCooldown(Senei) && Senei.LevelChecked())
                        return Senei;

                    if (gauge.MeditationStacks >= 3 && Shoha.LevelChecked())
                        return Shoha;
                }

                return actionID;
            }
        }

        internal class SAM_Kyuten_Shoha2_Guren : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Kyuten_Shoha2;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (actionID == Kyuten)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Kyuten_Shoha2_Guren) && IsOffCooldown(Guren) && Guren.LevelChecked())
                        return Guren;

                    if (IsEnabled(CustomComboPreset.SAM_Kyuten_Shoha2) && gauge.MeditationStacks == 3 && Shoha2.LevelChecked())
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
                    if (OgiNamikiri.LevelChecked())
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


