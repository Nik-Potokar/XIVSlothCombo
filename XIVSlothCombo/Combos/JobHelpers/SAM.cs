using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class SAMHelper : SAM
    {
        internal static int SenCount => GetSenCount();
        private static int GetSenCount()
        {
            var gauge = CustomComboFunctions.GetJobGauge<SAMGauge>();
            var senCount = 0;
            if (gauge.HasGetsu) senCount++;
            if (gauge.HasSetsu) senCount++;
            if (gauge.HasKa) senCount++;

            return senCount;
        }
        internal static bool ComboStarted => GetComboStarted();
        private unsafe static bool GetComboStarted()
        {
            var comboAction = ActionManager.Instance()->Combo.Action;
            if (comboAction == CustomComboFunctions.OriginalHook(Hakaze) || comboAction == CustomComboFunctions.OriginalHook(Jinpu) || comboAction == CustomComboFunctions.OriginalHook(Shifu))
                return true;
            return false;
        }
    }


    internal class SAMOpenerLogic : SAM
    {
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(Senei))
                return false;

            if (!CustomComboFunctions.ActionReady(Ikishoten))
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

        private static uint OpenerLevel => 100;

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
            if (!LevelChecked)
                return false;

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

                if (CustomComboFunctions.HasEffect(All.Buffs.TrueNorth) && PrePullStep == 2) currentState = OpenerState.InOpener;
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

                if (CustomComboFunctions.WasLastAction(TendoSetsugekka) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = TendoSetsugekka;

                if (CustomComboFunctions.WasLastAction(Senei) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Senei;

                if (CustomComboFunctions.WasLastAction(TendoKaeshiSetsugekka) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = TendoKaeshiSetsugekka;

                if (CustomComboFunctions.WasLastAction(MeikyoShisui) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = MeikyoShisui;

                if (CustomComboFunctions.WasLastAction(All.TrueNorth) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = All.TrueNorth;

                if (CustomComboFunctions.WasLastAction(Gekko) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Gekko;

                if (CustomComboFunctions.WasLastAction(Zanshin) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Zanshin;

                if (CustomComboFunctions.WasLastAction(Higanbana) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Higanbana;

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

                if (CustomComboFunctions.WasLastAction(Gyofu) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Gyofu;

                if (CustomComboFunctions.WasLastAction(Yukikaze) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Yukikaze;

                if (CustomComboFunctions.WasLastAction(Shinten) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = Shinten;

                if (CustomComboFunctions.WasLastAction(TendoSetsugekka) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = TendoSetsugekka;

                if (CustomComboFunctions.WasLastAction(TendoKaeshiSetsugekka) && OpenerStep == 24) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 24) actionID = TendoKaeshiSetsugekka;


                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Senei && CustomComboFunctions.IsOnCooldown(Senei)) ||
                   (actionID == Ikishoten && CustomComboFunctions.IsOnCooldown(Ikishoten)) ||
                   (actionID == MeikyoShisui && CustomComboFunctions.GetRemainingCharges(MeikyoShisui) < 1)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
            OpenerStep = 1;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (DoOpener(ref actionID))
                    return true;
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }
            return false;
        }
    }
}