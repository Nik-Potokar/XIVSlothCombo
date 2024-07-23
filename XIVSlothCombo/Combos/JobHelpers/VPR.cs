using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class VPROpenerLogic : VPR
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Dreadwinder) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(SerpentsIre))
                return false;

            return true;
        }

        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

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
                if (CustomComboFunctions.WasLastAction(DreadFangs) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = DreadFangs;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }
            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(SerpentsIre) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = SerpentsIre;

                if (CustomComboFunctions.WasLastAction(SwiftskinsSting) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = SwiftskinsSting;

                if (CustomComboFunctions.WasLastAction(Dreadwinder) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Dreadwinder;

                if (CustomComboFunctions.WasLastAction(HuntersCoil) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = HuntersCoil;

                if (CustomComboFunctions.WasLastAction(TwinfangBite) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = TwinfangBite;

                if (CustomComboFunctions.WasLastAction(TwinbloodBite) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = TwinbloodBite;

                if (CustomComboFunctions.WasLastAction(SwiftskinsCoil) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = SwiftskinsCoil;

                if (CustomComboFunctions.WasLastAction(TwinbloodBite) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = TwinbloodBite;

                if (CustomComboFunctions.WasLastAction(TwinfangBite) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = TwinfangBite;

                if (CustomComboFunctions.WasLastAction(Reawaken) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Reawaken;

                if (CustomComboFunctions.WasLastAction(FirstGeneration) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = FirstGeneration;

                if (CustomComboFunctions.WasLastAction(FirstLegacy) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = FirstLegacy;

                if (CustomComboFunctions.WasLastAction(SecondGeneration) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = SecondGeneration;

                if (CustomComboFunctions.WasLastAction(SecondLegacy) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = SecondLegacy;

                if (CustomComboFunctions.WasLastAction(ThirdGeneration) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = ThirdGeneration;

                if (CustomComboFunctions.WasLastAction(ThirdLegacy) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = ThirdLegacy;

                if (CustomComboFunctions.WasLastAction(FourthGeneration) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = FourthGeneration;

                if (CustomComboFunctions.WasLastAction(FourthLegacy) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = FourthLegacy;

                if (CustomComboFunctions.WasLastAction(Ouroboros) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Ouroboros;

                if (CustomComboFunctions.WasLastAction(HindstingStrike) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = HindstingStrike;

                if (CustomComboFunctions.WasLastAction(DeathRattle) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = DeathRattle;

                if (CustomComboFunctions.WasLastAction(UncoiledFury) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = UncoiledFury;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinfang) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = UncoiledTwinfang;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinblood) && OpenerStep == 24) OpenerStep++;
                else if (OpenerStep == 24) actionID = UncoiledTwinblood;

                if (CustomComboFunctions.WasLastAction(UncoiledFury) && OpenerStep == 25) OpenerStep++;
                else if (OpenerStep == 25) actionID = UncoiledFury;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinfang) && OpenerStep == 26) OpenerStep++;
                else if (OpenerStep == 26) actionID = UncoiledTwinfang;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinblood) && OpenerStep == 27) OpenerStep++;
                else if (OpenerStep == 27) actionID = UncoiledTwinblood;

                if (CustomComboFunctions.WasLastAction(Dreadwinder) && OpenerStep == 28) OpenerStep++;
                else if (OpenerStep == 28) actionID = Dreadwinder;

                if (CustomComboFunctions.WasLastAction(UncoiledFury) && OpenerStep == 29) OpenerStep++;
                else if (OpenerStep == 29) actionID = UncoiledFury;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinfang) && OpenerStep == 30) OpenerStep++;
                else if (OpenerStep == 30) actionID = UncoiledTwinfang;

                if (CustomComboFunctions.WasLastAction(UncoiledTwinblood) && OpenerStep == 31) OpenerStep++;
                else if (OpenerStep == 31) actionID = UncoiledTwinblood;

                if (CustomComboFunctions.WasLastAction(HuntersCoil) && OpenerStep == 32) OpenerStep++;
                else if (OpenerStep == 32) actionID = HuntersCoil;

                if (CustomComboFunctions.WasLastAction(TwinfangBite) && OpenerStep == 33) OpenerStep++;
                else if (OpenerStep == 33) actionID = TwinfangBite;

                if (CustomComboFunctions.WasLastAction(TwinbloodBite) && OpenerStep == 34) OpenerStep++;
                else if (OpenerStep == 34) actionID = TwinbloodBite;

                if (CustomComboFunctions.WasLastAction(SwiftskinsCoil) && OpenerStep == 35) OpenerStep++;
                else if (OpenerStep == 35) actionID = SwiftskinsCoil;

                if (CustomComboFunctions.WasLastAction(TwinbloodBite) && OpenerStep == 36) OpenerStep++;
                else if (OpenerStep == 36) actionID = TwinbloodBite;

                if (CustomComboFunctions.WasLastAction(TwinfangBite) && OpenerStep == 37) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 37) actionID = TwinfangBite;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == SerpentsIre && CustomComboFunctions.IsOnCooldown(SerpentsIre)) ||
                    (actionID == Dreadwinder && CustomComboFunctions.GetRemainingCharges(Dreadwinder) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

    internal static class VPRHelpers
    {
        public static bool HasRattlingCoilStack(this VPRGauge gauge) => gauge.RattlingCoilStacks > 0;
    }
}