using System.Runtime.InteropServices.ComTypes;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
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
                SamKenkiOvercapAmount = "SamKenkiOvercapAmount";
        }
    }

    internal class SamuraiYukikazeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiYukikazeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Yukikaze)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var SamKenkiOvercapAmount = Service.Configuration.GetCustomIntValue(SAM.Config.SamKenkiOvercapAmount);

                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature) && gauge.Kenki >= SamKenkiOvercapAmount && CanWeave(actionID))
                {
                    if (level >= SAM.Levels.Shinten)
                        return SAM.Shinten;
                }
                
                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Yukikaze;
                if (comboTime > 0)
                {
                    if (lastComboMove == SAM.Hakaze && level >= SAM.Levels.Yukikaze)
                        return SAM.Yukikaze;
                }

                return SAM.Hakaze;
            }

            return actionID;
        }
    }

    internal class SamuraiGekkoCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiGekkoCombo;
        internal static bool inOpener = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Gekko)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var SamKenkiOvercapAmount = Service.Configuration.GetCustomIntValue(SAM.Config.SamKenkiOvercapAmount);
                var meikyoBuff = HasEffectAny(SAM.Buffs.MeikyoShisui);
                var oneSeal = OriginalHook(SAM.Iaijutsu) == SAM.Higanbana;
                var twoSeal = OriginalHook(SAM.Iaijutsu) == SAM.TenkaGoken;
                var threeSeal = OriginalHook(SAM.Iaijutsu) == SAM.Setsugekka;
                var meikyostacks = GetBuffStacks(SAM.Buffs.MeikyoShisui);

                if (IsEnabled(CustomComboPreset.SamuraiRangedUptimeFeature) && level >= SAM.Levels.Enpi)
                {
                    if (!InMeleeRange())
                        return SAM.Enpi;
                }

                if (!InCombat())
                {
                    if (!meikyoBuff && inOpener)
                    {
                        inOpener = false;
                    }

                    if (level == 90 && IsEnabled(CustomComboPreset.SamuraiOpenerFeature))
                    {
                        if (meikyoBuff && !inOpener)
                            inOpener = true;

                        if (inOpener)
                        {
                            if (GetBuffStacks(SAM.Buffs.MeikyoShisui) == 3 && (oneSeal || twoSeal || threeSeal))
                                return SAM.Hagakure;
                            if (meikyoBuff && IsOnCooldown(SAM.MeikyoShisui) && gauge.Sen == Sen.NONE)
                                return SAM.Gekko;
                        }
                    }
                }

                if (InCombat())
                {
                    if (inOpener && IsEnabled(CustomComboPreset.SamuraiOpenerFeature) && level == 90)
                    {
                        //oGCDs
                        if (CanWeave(actionID))
                        {
                            if (gauge.Kaeshi == Kaeshi.NAMIKIRI && gauge.MeditationStacks == 3)
                                return SAM.Shoha;
                            if (twoSeal && gauge.MeditationStacks == 0 && GetCooldownRemainingTime(SAM.Ikishoten) < 110 && IsOnCooldown(SAM.Ikishoten))
                            {
                                if (gauge.Kenki >= 10 && IsOffCooldown(SAM.Gyoten))
                                    return SAM.Gyoten;
                                if (gauge.Kenki >= 25)
                                    return SAM.Shinten;
                            }

                            if (twoSeal && IsOffCooldown(SAM.Ikishoten))
                                return SAM.Ikishoten;
                            if (gauge.Kenki >= 25)
                            {
                                if (oneSeal && GetRemainingCharges(SAM.MeikyoShisui) == 0 && oneSeal)
                                    return SAM.Shinten;

                                if (GetRemainingCharges(SAM.MeikyoShisui) == 1 && gauge.Kaeshi == Kaeshi.SETSUGEKKA && IsOffCooldown(SAM.Senei))
                                    return SAM.Senei;
                            }

                            if (gauge.Sen == Sen.NONE && GetRemainingCharges(SAM.MeikyoShisui) == 1)
                                return SAM.MeikyoShisui;
                            if (gauge.Kenki >= 25 && IsOnCooldown(SAM.Shoha))
                                return SAM.Shinten;
                        }

                        //GCDs
                        if (threeSeal && (GetRemainingCharges(SAM.MeikyoShisui) == 1 || !HasEffect(SAM.Buffs.OgiNamikiriReady)))
                            return OriginalHook(SAM.Iaijutsu);
                        if (oneSeal && !TargetHasEffect(SAM.Debuffs.Higanbana) && GetRemainingCharges(SAM.TsubameGaeshi) == 1)
                            return OriginalHook(SAM.Iaijutsu);
                        if (oneSeal && TargetHasEffect(SAM.Debuffs.Higanbana) && HasEffect(SAM.Buffs.OgiNamikiriReady))
                            return OriginalHook(SAM.OgiNamikiri);
                        if (gauge.Kaeshi == Kaeshi.SETSUGEKKA)
                            return OriginalHook(SAM.TsubameGaeshi);
                        if (gauge.Kaeshi == Kaeshi.NAMIKIRI)
                            return OriginalHook(OriginalHook(SAM.OgiNamikiri));
                        //1-2-3 Logic
                        if (lastComboMove == SAM.Hakaze)
                            return SAM.Yukikaze;
                        if (twoSeal && gauge.MeditationStacks == 0 && TargetHasEffect(SAM.Debuffs.Higanbana))
                            return SAM.Hakaze;
                        if (meikyostacks == 3)
                            return SAM.Gekko;
                        if (meikyostacks == 2)
                            return SAM.Kasha;
                        if (meikyostacks == 1)
                        {
                            if (GetCooldownRemainingTime(SAM.Ikishoten) > 110)
                                return SAM.Yukikaze;
                            if (gauge.MeditationStacks == 0)
                                return SAM.Gekko;
                        }

                        if (GetRemainingCharges(SAM.TsubameGaeshi) == 0)
                        {
                            inOpener = false;
                        }
                    }

                    if (!inOpener)
                    {
                        //oGCDs
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.SamuraiIkishotenonmaincombo) && level >= SAM.Levels.Ikishoten)
                            {
                                if (gauge.Kenki > 50 && GetCooldownRemainingTime(SAM.Ikishoten) < 10)
                                    return SAM.Shinten;
                                if (gauge.Kenki <= 50 && IsOffCooldown(SAM.Ikishoten))
                                    return SAM.Ikishoten;
                            }

                            //Meikyo Features
                            if (IsEnabled(CustomComboPreset.MeikyoShisuionST) && level >= SAM.Levels.MeikyoShisui && !HasEffect(SAM.Buffs.MeikyoShisui) && GetRemainingCharges(SAM.MeikyoShisui) > 0)
                            {
                                if (IsNotEnabled(CustomComboPreset.MeikyoShisuiBurstFeature))
                                    return SAM.MeikyoShisui;
                                if (IsEnabled(CustomComboPreset.MeikyoShisuiBurstFeature))
                                {
                                    if ((gauge.Kaeshi == Kaeshi.NONE && gauge.Sen == Sen.NONE && GetDebuffRemainingTime(SAM.Debuffs.Higanbana) < 10) ||
                                        (GetDebuffRemainingTime(SAM.Debuffs.Higanbana) > 45)) // Meikyo Shisui Overcap Protection
                                        return SAM.MeikyoShisui;
                                }
                            }

                            //Senei Features
                            if (IsEnabled(CustomComboPreset.SeneionST) && gauge.Kenki >= 25 && IsOffCooldown(SAM.Senei) && level >= SAM.Levels.Senei)
                            {
                                if (IsNotEnabled(CustomComboPreset.SeneiBurstFeature))
                                    return SAM.Senei;
                                if (IsEnabled(CustomComboPreset.SeneiBurstFeature))
                                {
                                    if ((gauge.Kaeshi == Kaeshi.NONE && gauge.Sen == Sen.NONE && GetDebuffRemainingTime(SAM.Debuffs.Higanbana) < 10) ||
                                        (GetCooldownRemainingTime(SAM.Ikishoten) > 100 && GetDebuffRemainingTime(SAM.Debuffs.Higanbana) > 40)) //Senei Non-Opener protection
                                        return SAM.Senei;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.SamuraiShohaSTFeature) && level >= SAM.Levels.Shoha && gauge.MeditationStacks == 3)
                                return SAM.Shoha;

                            if (GetCooldownRemainingTime(SAM.Senei) > 110 && gauge.Kenki >= 25 && level >= SAM.Levels.Shinten)
                                return SAM.Shinten;

                            if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature) && level >= SAM.Levels.Shinten && gauge.Kenki >= SamKenkiOvercapAmount)
                                return SAM.Shinten;
                        }


                        if (IsEnabled(CustomComboPreset.SamuraiOgiNamikiriSTFeature) && level >= SAM.Levels.OgiNamikiri && ((!this.IsMoving && HasEffect(SAM.Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI))
                        {
                            if (IsNotEnabled(CustomComboPreset.OgiNamikiriinBurstFeature))
                                return OriginalHook(SAM.OgiNamikiri);
                            if (IsEnabled(CustomComboPreset.OgiNamikiriinBurstFeature))
                            {
                                if ((meikyostacks == 1 && GetDebuffRemainingTime(SAM.Debuffs.Higanbana) > 50) ||
                                    (GetBuffRemainingTime(SAM.Buffs.OgiNamikiriReady) > 5)) //Waste prevention
                                    return OriginalHook(SAM.OgiNamikiri);
                            }
                        }

                        if (IsEnabled(CustomComboPreset.IaijutsuSTFeature) && level >= SAM.Levels.Setsugekka)
                        {
                            if (!this.IsMoving && (((oneSeal || oneSeal && meikyostacks == 2) && GetDebuffRemainingTime(SAM.Debuffs.Higanbana) < 5) || threeSeal))
                                return OriginalHook(SAM.Iaijutsu);

                            if (gauge.Kaeshi == Kaeshi.SETSUGEKKA && level >= SAM.Levels.TsubameGaeshi && GetRemainingCharges(SAM.TsubameGaeshi) > 0)
                                return OriginalHook(SAM.TsubameGaeshi);
                        }

                        if (HasEffect(SAM.Buffs.MeikyoShisui))
                        {
                            if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                return OriginalHook(SAM.Gekko);
                            if (IsEnabled(CustomComboPreset.KashaonST) && gauge.Sen.HasFlag(Sen.KA) == false)
                                return OriginalHook(SAM.Kasha);
                            if (IsEnabled(CustomComboPreset.YukionST) && gauge.Sen.HasFlag(Sen.SETSU) == false)
                                return OriginalHook(SAM.Yukikaze);
                        }

                        if (comboTime > 0)
                        {
                            if (lastComboMove == SAM.Hakaze && level >= SAM.Levels.Jinpu)
                            {
                                if (IsEnabled(CustomComboPreset.YukionST) && gauge.Sen.HasFlag(Sen.SETSU) == false && level >= SAM.Levels.Yukikaze && HasEffect(SAM.Buffs.Fugetsu) && HasEffect(SAM.Buffs.Fuka))
                                    return SAM.Yukikaze;
                                if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                    return SAM.Jinpu;
                                if (IsEnabled(CustomComboPreset.KashaonST) && gauge.Sen.HasFlag(Sen.KA) == false)
                                    return SAM.Shifu;
                                return SAM.Jinpu;
                            }

                            if (lastComboMove == SAM.Jinpu && level >= SAM.Levels.Gekko)
                                return SAM.Gekko;
                            if (IsEnabled(CustomComboPreset.KashaonST) && lastComboMove == SAM.Shifu && level >= SAM.Levels.Kasha)
                                return SAM.Kasha;
                        }
                    }
                }
                return SAM.Hakaze;
            }

            return actionID;
        }
    }

    internal class SamuraiKashaCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiKashaCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<SAMGauge>();
            var SamKenkiOvercapAmount = Service.Configuration.GetCustomIntValue(SAM.Config.SamKenkiOvercapAmount);

            if (actionID == SAM.Kasha)
            {
                if (CanWeave(actionID))
                {

                    if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature) && gauge.Kenki >= SamKenkiOvercapAmount && CanWeave(actionID))
                    {
                        if (level >= SAM.Levels.Shinten)
                            return SAM.Shinten;
                    }
                }

                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Kasha;

                if (comboTime > 0)
                {
                    if (lastComboMove == SAM.Hakaze && level >= SAM.Levels.Shifu)
                        return SAM.Shifu;

                    if (lastComboMove == SAM.Shifu && level >= SAM.Levels.Kasha)
                        return SAM.Kasha;
                }

                return SAM.Hakaze;
            }

            return actionID;
        }
    }

    internal class SamuraiMangetsuCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiMangetsuCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Mangetsu)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var SamKenkiOvercapAmount = Service.Configuration.GetCustomIntValue(SAM.Config.SamKenkiOvercapAmount);

                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.SamuraiIkishotenonmaincombo) && gauge.Kenki <= 50 && IsOffCooldown(SAM.Ikishoten) && level >= SAM.Levels.Ikishoten)
                        return SAM.Ikishoten;
                    if (IsEnabled(CustomComboPreset.SamuraiOvercapFeatureAoe) && gauge.Kenki >= SamKenkiOvercapAmount)
                    {
                        if (IsOffCooldown(SAM.Guren) && level >= SAM.Levels.Guren)
                            return SAM.Guren;
                        if (level >= SAM.Levels.Kyuten)
                            return SAM.Kyuten;
                    }
                }

                if (IsEnabled(CustomComboPreset.SamuraiShoha2AOEFeature) && level >= SAM.Levels.Shoha2 && gauge.MeditationStacks == 3)
                    return SAM.Shoha2;

                if (IsEnabled(CustomComboPreset.SamuraiOgiNamikiriAOEFeature) && level >= SAM.Levels.OgiNamikiri)
                {
                    if ((!this.IsMoving && HasEffect(SAM.Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                        return OriginalHook(SAM.OgiNamikiri);
                }

                if (IsEnabled(CustomComboPreset.TenkaGokenAOEFeature) && level >= SAM.Levels.TenkaGoken)
                {
                    if (!this.IsMoving && (OriginalHook(SAM.Iaijutsu) == SAM.TenkaGoken || OriginalHook(SAM.Iaijutsu) == SAM.Setsugekka))
                        return OriginalHook(SAM.Iaijutsu);
                    if ((gauge.Kaeshi == Kaeshi.GOKEN || gauge.Kaeshi == Kaeshi.SETSUGEKKA) && level >= SAM.Levels.TsubameGaeshi && GetRemainingCharges(SAM.TsubameGaeshi) > 0)
                        return OriginalHook(SAM.TsubameGaeshi);
                }

                if (HasEffect(SAM.Buffs.MeikyoShisui))
                {
                    if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                    return SAM.Mangetsu;
                    if (IsEnabled(CustomComboPreset.SamuraiOkaFeature) && gauge.Sen.HasFlag(Sen.KA) == false)
                        return SAM.Oka;
                }

                if (comboTime > 0 && level >= SAM.Levels.Mangetsu && (lastComboMove == SAM.Fuko || lastComboMove == SAM.Fuga))
                {
                    if (IsNotEnabled(CustomComboPreset.SamuraiOkaFeature) || 
                        gauge.Sen.HasFlag(Sen.GETSU) == false || GetBuffRemainingTime(SAM.Buffs.Fugetsu) < GetBuffRemainingTime(SAM.Buffs.Fuka) || !HasEffect(SAM.Buffs.Fugetsu))
                        return SAM.Mangetsu;
                    if (IsEnabled(CustomComboPreset.SamuraiOkaFeature) && level >= SAM.Levels.Oka &&
                        (gauge.Sen.HasFlag(Sen.KA) == false || GetBuffRemainingTime(SAM.Buffs.Fuka) < GetBuffRemainingTime(SAM.Buffs.Fugetsu) || !HasEffect(SAM.Buffs.Fuka)))
                        return SAM.Oka;
                }

                return OriginalHook(SAM.Fuko);
            }

            return actionID;
        }
    }

    internal class SamuraiOkaCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiOkaCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Oka)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var SamKenkiOvercapAmount = Service.Configuration.GetCustomIntValue(SAM.Config.SamKenkiOvercapAmount);

                if (CanWeave(actionID))
                {
                    if (IsEnabled(CustomComboPreset.SamuraiIkishotenonmaincombo) && gauge.Kenki <= 50 && IsOffCooldown(SAM.Ikishoten) && level >= SAM.Levels.Ikishoten)
                        return SAM.Ikishoten;

                    if (IsEnabled(CustomComboPreset.SamuraiOvercapFeatureAoe) && gauge.Kenki >= SamKenkiOvercapAmount)
                    {
                        if (IsOffCooldown(SAM.Guren) && level >= SAM.Levels.Guren)
                            return SAM.Guren;
                        if (level >= SAM.Levels.Kyuten)
                            return SAM.Kyuten;
                    }
                }

                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Oka;

                if (comboTime > 0 && level >= SAM.Levels.Oka)
                {
                    if (lastComboMove == SAM.Fuko || lastComboMove == SAM.Fuga)
                        return SAM.Oka;
                }

                return OriginalHook(SAM.Fuko);
            }

            return actionID;
        }
    }

    // internal class SamuraiJinpuShifuFeature : CustomCombo
    // {
    //     protected override CustomComboPreset Preset => CustomComboPreset.SamuraiJinpuShifuFeature;
    //
    //     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //     {
    //         if (actionID == SAM.MeikyoShisui)
    //         {
    //             if (HasEffect(SAM.Buffs.MeikyoShisui) && IsEnabled(CustomComboPreset.SamuraiJinpuShifuFeature))
    //             {
    //                 if (!HasEffect(SAM.Buffs.Jinpu))
    //                     return SAM.Jinpu;
    //
    //                 if (!HasEffect(SAM.Buffs.Shifu))
    //                     return SAM.Shifu;
    //
    //             }
    //             return SAM.MeikyoShisui;
    //         }
    //
    //         return actionID;
    //     }
    // }

    internal class SamuraiTsubameGaeshiShohaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiTsubameGaeshiShohaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.TsubameGaeshi)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;
            }

            return actionID;
        }
    }

    internal class SamuraiIaijutsuShohaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiIaijutsuShohaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Iaijutsu)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;
            }

            return actionID;
        }
    }

    internal class SamuraiTsubameGaeshiIaijutsuFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiTsubameGaeshiIaijutsuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.TsubameGaeshi)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (level >= SAM.Levels.TsubameGaeshi && gauge.Sen == Sen.NONE)
                    return OriginalHook(SAM.TsubameGaeshi);
                return OriginalHook(SAM.Iaijutsu);
            }

            return actionID;
        }
    }

    internal class SamuraiIaijutsuTsubameGaeshiFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiIaijutsuTsubameGaeshiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {

            if (actionID == SAM.Iaijutsu)
            {
                //var iaijutsuCD = GetCooldown(SAM.Iaijutsu);
                var gauge = GetJobGauge<SAMGauge>();

                if (level >= SAM.Levels.TsubameGaeshi && gauge.Sen == Sen.NONE)
                {
                    return OriginalHook(SAM.TsubameGaeshi);
                }
            }

            return actionID;
        }
    }
    internal class SamuraiSeneiFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiSeneiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Shinten)
            {
                if (IsOffCooldown(SAM.Senei) && level >= SAM.Levels.Senei)
                    return SAM.Senei;
            }
            return actionID;
        }
    }
    internal class SamuraiShohaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiShohaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Shinten)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (gauge.MeditationStacks >= 3)
                    return SAM.Shoha;
            }
            return actionID;
        }
    }
    internal class SamuraiGurenFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiGurenFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Kyuten)
            {
                if (IsOffCooldown(SAM.Guren) && level >= SAM.Levels.Guren)
                    return SAM.Guren;
            }
            return actionID;
        }
    }
    internal class SamuraiIkishotenNamikiriFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiIkishotenNamikiriFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Ikishoten)
            {
                if (level >= SAM.Levels.OgiNamikiri)
                {
                    if (HasEffect(SAM.Buffs.OgiNamikiriReady))
                    {
                        if (HasEffect(SAM.Buffs.OgiNamikiriReady))
                            return SAM.OgiNamikiri;
                    }

                    if (OriginalHook(SAM.OgiNamikiri) == SAM.KaeshiNamikiri)
                        return SAM.KaeshiNamikiri;
                }

                return SAM.Ikishoten;
            }
            return actionID;
        }
    }
    internal class SamuraiShoha2Feature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiShoha2Feature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Kyuten)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (level >= SAM.Levels.Shoha2 && gauge.MeditationStacks >= 3)
                    return SAM.Shoha2;
            }


            return actionID;
        }
    }

    internal class SamuraiYatenFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiYatenFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Gyoten)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (InMeleeRange() && gauge.Kenki >= 10)
                    return SAM.Yaten;
                if (InMeleeRange() && gauge.Kenki >= 10)
                    return SAM.Gyoten;
            }
            return actionID;
        }
    }

}

