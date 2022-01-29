using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SAM
    {
        public const byte JobID = 34;

        public const uint
            Hakaze = 7477,
            Yukikaze = 7480,
            Gekko = 7481,
            Jinpu = 7478,
            Kasha = 7482,
            Shifu = 7479,
            Mangetsu = 7484,
            Fuga = 7483,
            Oka = 7485,
            Shinten = 7490,
            Kyuten = 7491,
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
            Kaiten = 7494,
            Yaten = 7493,
            Gyoten = 7492;

        public static class Buffs
        {
            public const short
                MeikyoShisui = 1233,
                EyesOpen = 1252,
                Jinpu = 1298,
                Shifu = 1299,
                OgiNamikiriReady = 2959,
                Fuka = 1299,
                Fugetsu = 1298,
                Kaiten = 1229;
        }

        public static class Debuffs
        {
            public const short
                Higanbana = 1228;
        }
        
        public static class Levels
        {
            public const byte
                Jinpu = 4,
                Shifu = 18,
                Gekko = 30,
                Iaijutsu = 30,
                Mangetsu = 35,
                Kasha = 40,
                Oka = 45,
                Yukikaze = 50,
                TsubameGaeshi = 76,
                Shoha = 80;
        }
    }

    internal class SamuraiYukikazeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiYukikazeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Yukikaze)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var actionIDCD = GetCooldown(actionID);
                var seneiCD = GetCooldown(SAM.Senei);
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature75))
                {

                    if (gauge.Kenki >= 75 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 75 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature85))
                {
                    if (gauge.Kenki >= 85 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 85 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Yukikaze;

                if (comboTime > 0 && lastComboMove == SAM.Hakaze && level >= SAM.Levels.Yukikaze)
                    return SAM.Yukikaze;

                return SAM.Hakaze;
            }

            return actionID;
        }
    }

    internal class SamuraiGekkoCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiGekkoCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Gekko)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var actionIDCD = GetCooldown(actionID);
                var seneiCD = GetCooldown(SAM.Senei);
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature75))
                {

                    if (gauge.Kenki >= 75 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 75 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature85))
                {
                    if (gauge.Kenki >= 85 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 85 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Gekko;

                if (comboTime > 0)
                {
                    if (lastComboMove == SAM.Hakaze && level >= SAM.Levels.Jinpu)
                        return SAM.Jinpu;

                    if (lastComboMove == SAM.Jinpu && level >= SAM.Levels.Gekko)
                        return SAM.Gekko;
                }

                return SAM.Hakaze;
            }

            return actionID;
        }
    }

    internal class SamuraiKashaCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiKashaCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Kasha)
            {
                var gauge = GetJobGauge<SAMGauge>();
                var actionIDCD = GetCooldown(actionID);
                var seneiCD = GetCooldown(SAM.Senei);
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature75))
                {

                    if (gauge.Kenki >= 75 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 75 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature85))
                {
                    if (gauge.Kenki >= 85 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 85 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiMangetsuCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Mangetsu)
            {
                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Mangetsu;

                if (comboTime > 0 && lastComboMove == SAM.Fuko && level >= SAM.Levels.Mangetsu)
                    return SAM.Mangetsu;
                if (comboTime > 0 && lastComboMove == SAM.Fuga && level >= SAM.Levels.Mangetsu)
                    return SAM.Mangetsu;

                return OriginalHook(SAM.Fuko);
            }

            return actionID;
        }
    }

    internal class SamuraiOkaCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiOkaCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Oka)
            {
                if (HasEffect(SAM.Buffs.MeikyoShisui))
                    return SAM.Oka;

                if (comboTime > 0 && lastComboMove == SAM.Fuko && level >= SAM.Levels.Oka)
                    return SAM.Oka;
                if (comboTime > 0 && lastComboMove == SAM.Fuga && level >= SAM.Levels.Oka)
                    return SAM.Oka;

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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiTsubameGaeshiShohaFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiIaijutsuShohaFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiTsubameGaeshiIaijutsuFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiIaijutsuTsubameGaeshiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {


            //var iaijutsuCD = GetCooldown(SAM.Iaijutsu);
            var gauge = GetJobGauge<SAMGauge>();
            var kaiten = HasEffect(SAM.Buffs.Kaiten);
            var ka = gauge.Sen.HasFlag(Sen.KA);
            var getsu = gauge.Sen.HasFlag(Sen.GETSU);
            var setsu = gauge.Sen.HasFlag(Sen.SETSU);
            var hasBanana = TargetHasEffect(SAM.Debuffs.Higanbana);
            var banana = FindTargetEffect(SAM.Debuffs.Higanbana);


            if (!kaiten && level >= SAM.Levels.Iaijutsu)
            {
                int numSen = (ka ? 1 : 0) + (getsu ? 1 : 0) + (setsu ? 1 : 0);
                if (IsEnabled(CustomComboPreset.SamuraiKaitenFeature1) && numSen == 1)
                {
                    if ((!hasBanana || banana.RemainingTime < 5) && gauge.Kenki >= 20)
                        return SAM.Kaiten;
                    return OriginalHook(SAM.Iaijutsu);
                }
                if (IsEnabled(CustomComboPreset.SamuraiKaitenFeature2) && numSen == 2)
                {
                    if (gauge.Kenki >= 20)
                        return SAM.Kaiten;
                    return OriginalHook(SAM.Iaijutsu);
                }
                if (IsEnabled(CustomComboPreset.SamuraiKaitenFeature3) && numSen == 3)
                {
                    if (gauge.Kenki >= 20)
                        return SAM.Kaiten;
                    return OriginalHook(SAM.Iaijutsu);
                }

            }

            if (actionID == SAM.Iaijutsu)
            {
                if (level >= SAM.Levels.TsubameGaeshi && gauge.Sen == Sen.NONE)
                    return OriginalHook(SAM.TsubameGaeshi);
                return OriginalHook(SAM.Iaijutsu);
            }

            return actionID;
        }
    }
    internal class SamuraiSeneiFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiSeneiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Shinten)
            {
                var seneiCD = GetCooldown(SAM.Senei);
                if (!seneiCD.IsCooldown && level >= 72)
                    return SAM.Senei;
            }
            return actionID;
        }
    }
    internal class SamuraiShohaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiShohaFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiGurenFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Kyuten)
            {
                var gurenCD = GetCooldown(SAM.Guren);
                if (!gurenCD.IsCooldown && level >= 70)
                    return SAM.Guren;
            }
            return actionID;
        }
    }
    internal class SamuraiIkishotenNamikiriFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiIkishotenNamikiriFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Ikishoten)
            {
                if (level >= 90)
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    if (HasEffect(SAM.Buffs.OgiNamikiriReady))
                    {
                        if (gauge.MeditationStacks >= 3)
                            return SAM.Shoha;
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
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiShoha2Feature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Kyuten)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (level >= 82 && gauge.MeditationStacks >= 3)
                    return SAM.Shoha2;
            }


            return actionID;
        }
    }
    // testing
    internal class SamuraiSimpleSamuraiFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiSimpleSamuraiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Hakaze)
            {   
                if(IsEnabled(CustomComboPreset.SamuraiSimpleSamuraiFeature))
                {
                    var gauge = GetJobGauge<SAMGauge>();
                    var meikyo = HasEffect(SAM.Buffs.MeikyoShisui);
                    var meikyoStacks = FindEffect(SAM.Buffs.MeikyoShisui);
                    var fuka = HasEffect(SAM.Buffs.Fuka);
                    var fugetsu = HasEffect(SAM.Buffs.Fugetsu);
                    var higanbana = TargetHasEffect(SAM.Debuffs.Higanbana);
                    var seneiCD = GetCooldown(SAM.Senei);
                    var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature75))
                {

                    if (gauge.Kenki >= 75 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 75 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (IsEnabled(CustomComboPreset.SamuraiOvercapFeature85))
                {
                    if (gauge.Kenki >= 85 && !seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 72)
                        return SAM.Senei;
                    if (gauge.Kenki >= 85 && seneiCD.IsCooldown && actionIDCD.IsCooldown && level >= 62)
                        return SAM.Shinten;
                }
                if (comboTime > 0 && level >= SAM.Levels.Shifu)
                    {
                        if(HasEffect(SAM.Buffs.MeikyoShisui))
                        {
                            if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                return OriginalHook(SAM.Gekko);
                            if (gauge.Sen.HasFlag(Sen.KA) == false)
                                return OriginalHook(SAM.Kasha);
                            if (gauge.Sen.HasFlag(Sen.SETSU) == false)
                                return OriginalHook(SAM.Yukikaze);

                        }
                        if (lastComboMove == SAM.Hakaze)
                        {
                            if (gauge.Sen.HasFlag(Sen.SETSU) == false && level >= SAM.Levels.Yukikaze)
                                return SAM.Yukikaze;
                            if (gauge.Sen.HasFlag(Sen.GETSU) == false)
                                return SAM.Jinpu;
                            if (gauge.Sen.HasFlag(Sen.KA) == false)
                                return SAM.Shifu;
                            return SAM.Jinpu;
                        }

                        if (lastComboMove == SAM.Jinpu && level >= SAM.Levels.Gekko)
                            return SAM.Gekko;
                        if (lastComboMove == SAM.Shifu && level >= SAM.Levels.Kasha)
                            return SAM.Kasha;
                    }
                    if (meikyo)
                    {
                        if (level >= SAM.Levels.Gekko && gauge.Sen.HasFlag(Sen.GETSU) == false)
                            return SAM.Gekko;
                        if (level >= SAM.Levels.Kasha && gauge.Sen.HasFlag(Sen.KA) == false)
                            return SAM.Kasha;
                        if (level >= SAM.Levels.Yukikaze && gauge.Sen.HasFlag(Sen.SETSU) == false)
                            return SAM.Yukikaze;

                        return actionID;
                    }

                }
            }
            return actionID;
        }
    }
    internal class SamuraiSimpleSamuraiAoECombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiSimpleSamuraiAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Oka)
            {
                var gauge = GetJobGauge<SAMGauge>();

                if (HasEffect(SAM.Buffs.MeikyoShisui) && gauge.Sen.HasFlag(Sen.KA) == false)
                    return OriginalHook(SAM.Oka);
                if (HasEffect(SAM.Buffs.MeikyoShisui) && gauge.Sen.HasFlag(Sen.GETSU) == false)
                    return OriginalHook(SAM.Mangetsu);

                if (comboTime > 0 && lastComboMove == SAM.Fuko && level >= SAM.Levels.Mangetsu)
                {
                    if (level >= SAM.Levels.Oka && gauge.Sen.HasFlag(Sen.KA) == false && FindEffect(SAM.Buffs.Shifu)?.RemainingTime < 10)
                        return SAM.Oka;
                    if (level < SAM.Levels.Oka || gauge.Sen.HasFlag(Sen.GETSU) == false || FindEffect(SAM.Buffs.Jinpu)?.RemainingTime < FindEffect(SAM.Buffs.Shifu)?.RemainingTime || !HasEffect(SAM.Buffs.Fugetsu))
                        return SAM.Mangetsu;
                    if (level >= SAM.Levels.Oka)
                        return SAM.Oka;
                }
                if (comboTime > 0 && lastComboMove == SAM.Fuga && level >= SAM.Levels.Mangetsu)
                {
                    if (level >= SAM.Levels.Oka && gauge.Sen.HasFlag(Sen.KA) == false && FindEffect(SAM.Buffs.Shifu)?.RemainingTime < 10)
                        return SAM.Oka;
                    if (level < SAM.Levels.Oka || gauge.Sen.HasFlag(Sen.GETSU) == false || FindEffect(SAM.Buffs.Jinpu)?.RemainingTime < FindEffect(SAM.Buffs.Shifu)?.RemainingTime || !HasEffect(SAM.Buffs.Fugetsu))
                        return SAM.Mangetsu;
                    if (level >= SAM.Levels.Oka)
                        return SAM.Oka;
                }
                return OriginalHook(SAM.Fuko);
            }

            return actionID;
        }
    }
    internal class SamuraiYatenFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiYatenFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.Gyoten)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (InMeleeRange(true) && gauge.Kenki >= 10)
                    return SAM.Yaten;
                if (InMeleeRange(false) && gauge.Kenki >= 10)
                    return SAM.Gyoten;
            }
            return actionID;
        }
    }
    internal class SamuraiOgiNamikiriFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SamuraiOgiNamikiriFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SAM.OgiNamikiri)
            {
                var gauge = GetJobGauge<SAMGauge>();
                if (HasEffect(SAM.Buffs.Kaiten) && HasEffect(SAM.Buffs.OgiNamikiriReady))
                    return OriginalHook(SAM.OgiNamikiri);
                if (HasEffect(SAM.Buffs.OgiNamikiriReady) && gauge.Kenki >= 20 )
                    return SAM.Kaiten;

            }
            return OriginalHook(SAM.OgiNamikiri);
        }
    }

}

