using Dalamud.Game.ClientState.JobGauge.Enums;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class SAMOpenerLogic : PvE.SAM
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(TsubameGaeshi) < 2)
                return false;
            if (!CustomComboFunctions.ActionReady(Ikishoten))
                return false;
            if (!CustomComboFunctions.ActionReady(Senei))
                return false;

            return true;
        }
        public static bool HasPrePullCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(MeikyoShisui) < 2)
                return false;

            if (CustomComboFunctions.GetRemainingCharges(All.TrueNorth) < 2)
                return false;

            return true;
        }
        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 1;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasPrePullCooldowns() && LevelChecked;

        private OpenerState currentState = OpenerState.PrePull;

        public OpenerState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull)
                    {
                        Svc.Log.Debug($"Entered PrePull Opener");
                    }
                    if (value == OpenerState.InOpener) OpenerStep = 1;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0)
            {
                PrePullStep = 1;
            }

            if (!HasCooldowns())
            {
                PrePullStep = 0;
            }

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.HasEffect(Buffs.MeikyoShisui) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = MeikyoShisui;

                if (CustomComboFunctions.HasEffect(All.Buffs.TrueNorth) && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = All.TrueNorth;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Kasha) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Kasha;

                if (CustomComboFunctions.WasLastAction(Ikishoten) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Ikishoten;

                if (CustomComboFunctions.WasLastAction(Yukikaze) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Yukikaze;

                if (CustomComboFunctions.WasLastAction(Setsugekka) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Setsugekka;

                if (CustomComboFunctions.WasLastAction(Senei) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Senei;

                if (CustomComboFunctions.WasLastAction(KaeshiSetsugekka) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = KaeshiSetsugekka;

                if (CustomComboFunctions.WasLastAction(MeikyoShisui) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = MeikyoShisui;

                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Higanbana) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Higanbana;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(OgiNamikiri) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = OgiNamikiri;

                if (CustomComboFunctions.WasLastAction(Shoha) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Shoha;

                if (CustomComboFunctions.WasLastAction(KaeshiNamikiri) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = KaeshiNamikiri;

                if (CustomComboFunctions.WasLastAction(Kasha) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Kasha;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Gyoten) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Gyoten;

                if (CustomComboFunctions.WasLastAction(Hakaze) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Hakaze;

                if (CustomComboFunctions.WasLastAction(Yukikaze) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Yukikaze;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Setsugekka) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = Setsugekka;

                if (CustomComboFunctions.WasLastAction(KaeshiSetsugekka) && OpenerStep == 24) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 24) actionID = KaeshiSetsugekka;


                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == TsubameGaeshi && CustomComboFunctions.GetRemainingCharges(TsubameGaeshi) == 0) ||
                    (actionID == MeikyoShisui && CustomComboFunctions.GetRemainingCharges(MeikyoShisui) == 0) ||
                       (actionID == Ikishoten && CustomComboFunctions.IsOnCooldown(Ikishoten)) ||
                       (actionID == Senei && CustomComboFunctions.IsOnCooldown(Senei))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool DoOpenerSimple(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Kasha) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Kasha;

                if (CustomComboFunctions.WasLastAction(Ikishoten) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Ikishoten;

                if (CustomComboFunctions.WasLastAction(Yukikaze) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Yukikaze;

                if (CustomComboFunctions.WasLastAction(Setsugekka) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Setsugekka;

                if (CustomComboFunctions.WasLastAction(Senei) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Senei;

                if (CustomComboFunctions.WasLastAction(KaeshiSetsugekka) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = KaeshiSetsugekka;

                if (CustomComboFunctions.WasLastAction(MeikyoShisui) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = MeikyoShisui;

                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Higanbana) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Higanbana;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(OgiNamikiri) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = OgiNamikiri;

                if (CustomComboFunctions.WasLastAction(Shoha) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Shoha;

                if (CustomComboFunctions.WasLastAction(KaeshiNamikiri) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = KaeshiNamikiri;

                if (CustomComboFunctions.WasLastAction(Kasha) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Kasha;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Gyoten) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Gyoten;

                if (CustomComboFunctions.WasLastAction(Hakaze) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Hakaze;

                if (CustomComboFunctions.WasLastAction(Yukikaze) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Yukikaze;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(Setsugekka) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = Setsugekka;

                if (CustomComboFunctions.WasLastAction(KaeshiSetsugekka) && OpenerStep == 24) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 24) actionID = KaeshiSetsugekka;

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == TsubameGaeshi && CustomComboFunctions.GetRemainingCharges(TsubameGaeshi) == 0) ||
                    (actionID == MeikyoShisui && CustomComboFunctions.GetRemainingCharges(MeikyoShisui) == 0) ||
                       (actionID == Ikishoten && CustomComboFunctions.IsOnCooldown(Ikishoten)) ||
                       (actionID == Senei && CustomComboFunctions.IsOnCooldown(Senei))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpenerSimple(ref actionID))
                        return true;
                }

                else
                {
                    if (DoOpener(ref actionID))
                        return true;
                }
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    internal class SAMFillerLogic : PvE.SAM
    {
        bool oddMinute = CustomComboFunctions.CombatEngageDuration().Minutes % 2 == 1 && 
            gauge.Sen == Sen.NONE && !CustomComboFunctions.HasEffect(Buffs.MeikyoShisui) && CustomComboFunctions.GetDebuffRemainingTime(Debuffs.Higanbana) >= 48 && 
            CustomComboFunctions.GetDebuffRemainingTime(Debuffs.Higanbana) <= 51;

        bool evenMinute = !CustomComboFunctions.HasEffect(Buffs.MeikyoShisui) && CustomComboFunctions.CombatEngageDuration().Minutes % 2 == 0 && 
            gauge.Sen == Sen.NONE && CustomComboFunctions.GetRemainingCharges(TsubameGaeshi) == 0 && 
            CustomComboFunctions.GetDebuffRemainingTime(Debuffs.Higanbana) >= 44 && CustomComboFunctions.GetDebuffRemainingTime(Debuffs.Higanbana) <= 47;

        int SamFillerCombo = Config.SAM_FillerCombo;

        internal static bool inOddFiller = false;
        internal static bool inEvenFiller = false;
        internal static bool fillerComplete = false;
        internal static bool fastFillerReady = false;

        private bool Filler(uint actionID, uint lastComboMove)
        {
            if (CustomComboFunctions.GetDebuffRemainingTime(Debuffs.Higanbana) < 42)
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
                    if (CustomComboFunctions.WasLastAbility(Gyoten))
                        fillerComplete = true;

                    if (CustomComboFunctions.WasLastAction(Enpi) && CustomComboFunctions.IsOffCooldown(Gyoten))
                        return Gyoten;

                    if (CustomComboFunctions.WasLastAction(Yaten))
                        return Enpi;

                    if (gauge.Sen == 0 && gauge.Kenki >= 10 && CustomComboFunctions.CanSpellWeave(actionID) && CustomComboFunctions.IsOffCooldown(Yaten))
                        return Yaten;
                }

                if (SamFillerCombo == 3)
                {
                    if (CustomComboFunctions.WasLastAbility(Hagakure))
                        fillerComplete = true;

                    if (gauge.Kenki >= 75 && CustomComboFunctions.CanWeave(actionID))
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
                    if (CustomComboFunctions.WasLastAbility(Hagakure))
                        fillerComplete = true;

                    if (gauge.Kenki >= 75 && CustomComboFunctions.CanWeave(actionID))
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
                    if (CustomComboFunctions.WasLastAbility(Hagakure))
                        fillerComplete = true;

                    if (gauge.Kenki >= 75 && CustomComboFunctions.CanWeave(actionID))
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
                    if (CustomComboFunctions.WasLastAbility(Hagakure))
                        fillerComplete = true;
                    if (CustomComboFunctions.WasLastWeaponskill(Hakaze) && gauge.Sen == Sen.SETSU)
                        fastFillerReady = true;

                    if (gauge.Kenki >= 75 && CustomComboFunctions.CanWeave(actionID))
                        return Shinten;

                    if (gauge.Sen == Sen.SETSU && CustomComboFunctions.WasLastWeaponskill(Yukikaze) && fastFillerReady)
                        return Hagakure;

                    if (lastComboMove == Hakaze)
                        return Yukikaze;

                    if (gauge.Sen == 0 || gauge.Sen == Sen.SETSU)
                        return Hakaze;
                }
            }
        }
    }
}