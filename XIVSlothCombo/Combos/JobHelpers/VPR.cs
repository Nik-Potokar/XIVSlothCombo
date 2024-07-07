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

        private static readonly uint[] StandardOpenerFlankFirst = [
            SerpentsIre,
            SwiftskinsSting,
            Dreadwinder,
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            Reawaken,
            FirstGeneration,
            FirstLegacy,
            SecondGeneration,
            SecondLegacy,
            ThirdGeneration,
            ThirdLegacy,
            FourthGeneration,
            FourthLegacy,
            Ouroboros,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            HindstingStrike,
            DeathRattle,
            Dreadwinder,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite];

        private static readonly uint[] StandardOpenerRearFirst = [
           SerpentsIre,
            SwiftskinsSting,
            Dreadwinder,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            Reawaken,
            FirstGeneration,
            FirstLegacy,
            SecondGeneration,
            SecondLegacy,
            ThirdGeneration,
            ThirdLegacy,
            FourthGeneration,
            FourthLegacy,
            Ouroboros,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            HindstingStrike,
            DeathRattle,
            Dreadwinder,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite];

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
                    if (value == OpenerState.InOpener) OpenerStep = 0;
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
                PrePullStep = 1;

            if (!HasCooldowns())
                PrePullStep = 0;

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

        private bool DoOpener(uint[] OpenerActions, ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(OpenerActions[OpenerStep]))
                    OpenerStep++;

                if (OpenerStep == OpenerActions.Length)
                    CurrentState = OpenerState.OpenerFinished;

                else actionID = OpenerActions[OpenerStep];

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
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

        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked) return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpener(StandardOpenerRearFirst, ref actionID))
                        return true;
                }
                else
                {
                    if (Config.VPR_Positional == 0)
                    {
                        if (DoOpener(StandardOpenerRearFirst, ref actionID))
                            return true;
                    }

                    if (Config.VPR_Positional == 1)
                    {
                        if (DoOpener(StandardOpenerFlankFirst, ref actionID))
                            return true;
                    }
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
    internal static class VPRHelpers
    {
        public static bool HasRattlingCoilStack(this VPRGauge gauge) => gauge.RattlingCoilStacks > 0;
    }
}