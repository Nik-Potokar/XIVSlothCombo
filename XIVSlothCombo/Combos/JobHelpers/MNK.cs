using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MNKOpenerLogic : MNK
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(PerfectBalance) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(Brotherhood))
                return false;

            if (!CustomComboFunctions.ActionReady(RiddleOfFire))
                return false;

            if (!CustomComboFunctions.ActionReady(RiddleOfWind))
                return false;

            if (!CustomComboFunctions.ActionReady(Meditation) && Gauge.Chakra < 5)
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
                if (Gauge.Chakra < 5 && PrePullStep == 1)
                {
                    actionID = ForbiddenMeditation;
                    return true;
                }

                if (CustomComboFunctions.WasLastAction(DragonKick) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = DragonKick;

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
                if (CustomComboFunctions.WasLastAction(PerfectBalance) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = PerfectBalance;

                if (CustomComboFunctions.WasLastAction(TheForbiddenChakra) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = TheForbiddenChakra;

                if (CustomComboFunctions.WasLastAction(LeapingOpo) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = LeapingOpo;

                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = DragonKick;

                // Pot

                if (CustomComboFunctions.WasLastAction(LeapingOpo) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = LeapingOpo;

                if (CustomComboFunctions.WasLastAction(Brotherhood) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Brotherhood;

                if (CustomComboFunctions.WasLastAction(RiddleOfFire) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = RiddleOfFire;

                if (CustomComboFunctions.WasLastAction(ElixirField) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = ElixirField;

                if (CustomComboFunctions.WasLastAction(RiddleOfWind) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = RiddleOfWind;

                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(FiresReply) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = FiresReply;

                if (CustomComboFunctions.WasLastAction(WindsReply) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = WindsReply;

                if (CustomComboFunctions.WasLastAction(LeapingOpo) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = LeapingOpo;

                if (CustomComboFunctions.WasLastAction(PerfectBalance) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = PerfectBalance;

                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(LeapingOpo) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = LeapingOpo;

                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(ElixirField) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = ElixirField;

                if (CustomComboFunctions.WasLastAction(LeapingOpo) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = LeapingOpo;

                if (CustomComboFunctions.WasLastAction(TwinSnakes) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = TwinSnakes;

                if (CustomComboFunctions.WasLastAction(Demolish) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Demolish;

                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = DragonKick;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) < 1) ||
                       (actionID == RiddleOfFire && CustomComboFunctions.IsOnCooldown(RiddleOfFire)) ||
                       (actionID == RiddleOfWind && CustomComboFunctions.IsOnCooldown(RiddleOfWind)) ||
                       (actionID == Brotherhood && CustomComboFunctions.IsOnCooldown(Brotherhood))
                       && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3))
                {
                    Svc.Log.Debug($"Failed at {actionID}");
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
}