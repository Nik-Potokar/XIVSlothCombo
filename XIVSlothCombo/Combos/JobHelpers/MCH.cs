using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MCHOpenerLogic : PvE.MCH
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(CheckMate) < 3)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(DoubleCheck) < 3)
                return false;
            if (!CustomComboFunctions.ActionReady(Chainsaw))
                return false;
            if (!CustomComboFunctions.ActionReady(Wildfire))
                return false;
            if (!CustomComboFunctions.ActionReady(BarrelStabilizer))
                return false;
            if (!CustomComboFunctions.ActionReady(Excavator))
                return false;
            if (!CustomComboFunctions.ActionReady(FullMetalField))
                return false;

            return true;
        }

        public static bool HasPrePullCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Reassemble) < 2) return false;

            return true;
        }


        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        private static uint[] StandardOpener = [
            AirAnchor,
            CheckMate,
            DoubleCheck,
            Drill,
            BarrelStabilizer,
            Chainsaw,
            Excavator,
            AutomatonQueen,
            Reassemble,
            Drill,
            CheckMate,
            Wildfire,
            FullMetalField,
            DoubleCheck,
            Hypercharge,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            Drill,
            DoubleCheck,
            CheckMate,
            HeatedSplitShot,
            DoubleCheck,
            HeatedSlugShot,
            HeatedCleanShot];

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

                if (CustomComboFunctions.HasEffect(Buffs.Reassembled) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = Reassemble;


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

                if (((actionID == Ricochet && CustomComboFunctions.GetRemainingCharges(CheckMate) < 3) ||
                        (actionID == Chainsaw && CustomComboFunctions.IsOnCooldown(Chainsaw)) ||
                        (actionID == Wildfire && CustomComboFunctions.IsOnCooldown(Wildfire)) ||
                        (actionID == BarrelStabilizer && CustomComboFunctions.IsOnCooldown(BarrelStabilizer)) ||
                        (actionID == BarrelStabilizer && CustomComboFunctions.IsOnCooldown(Excavator)) ||
                        (actionID == BarrelStabilizer && CustomComboFunctions.IsOnCooldown(FullMetalField)) ||
                        (actionID == GaussRound && CustomComboFunctions.GetRemainingCharges(DoubleCheck) < 3)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
                    if (DoOpener(StandardOpener, ref actionID))
                        return true;
                }
                else
                {
                    if (DoOpener(StandardOpener, ref actionID))
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

    internal static class MCHExtensions
    {
        private static uint lastBattery = 0;
        internal static uint LastSummonBattery(this MCHGauge gauge)
        {
            if (!CustomComboFunctions.InCombat() || ActionWatching.CombatActions.Count(x => x == CustomComboFunctions.OriginalHook(MCH.RookAutoturret)) == 0)
                lastBattery = 0;

            if (ActionWatching.CombatActions.Count(x => x == CustomComboFunctions.OriginalHook(MCH.RookAutoturret)) > 0)
                lastBattery = gauge.LastSummonBatteryPower;

            return lastBattery;
        }
    }
}