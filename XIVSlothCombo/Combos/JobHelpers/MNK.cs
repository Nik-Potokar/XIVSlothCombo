using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MNKOpenerLogic : PvE.MNK
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


            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 1;

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
                if (Svc.Gauges.Get<MNKGauge>().Chakra == 5 && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = Meditation;

                if (CustomComboFunctions.HasEffect(Buffs.FormlessFist) && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = FormShift;

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
                if (Config.MNK_OpenerChoice == 0)
                {
                    if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(TwinSnakes) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = TwinSnakes;

                    if (CustomComboFunctions.WasLastAction(RiddleOfFire) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = RiddleOfFire;

                    if (CustomComboFunctions.WasLastAction(Demolish) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Demolish;

                    if (CustomComboFunctions.WasLastAction(ForbiddenChakra) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = ForbiddenChakra;

                    if (CustomComboFunctions.WasLastAction(Bootshine) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Bootshine;

                    if (CustomComboFunctions.WasLastAction(Brotherhood) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Brotherhood;

                    if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 1 && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = PerfectBalance;

                    if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(RiddleOfWind) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = RiddleOfWind;

                    if (CustomComboFunctions.WasLastAction(Bootshine) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Bootshine;

                    if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(ForbiddenChakra) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = ForbiddenChakra;

                    if (CustomComboFunctions.WasLastAction(ElixirField) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = ElixirField;

                    if (CustomComboFunctions.WasLastAction(Bootshine) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Bootshine;

                    if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0 && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = PerfectBalance;

                    if (CustomComboFunctions.WasLastAction(TwinSnakes) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = TwinSnakes;

                    if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(Demolish) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Demolish;

                    if (CustomComboFunctions.WasLastAction(RisingPhoenix) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = RisingPhoenix;

                    if (CustomComboFunctions.WasLastAction(TwinSnakes) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = TwinSnakes;

                    if (CustomComboFunctions.WasLastAction(SnapPunch) && OpenerStep == 22) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 22) actionID = SnapPunch;

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                        CurrentState = OpenerState.FailedOpener;

                    if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0) ||
                           (actionID == Brotherhood && CustomComboFunctions.IsOnCooldown(Brotherhood)) ||
                           (actionID == RiddleOfFire && CustomComboFunctions.IsOnCooldown(RiddleOfFire)) ||
                           (actionID == RiddleOfWind && CustomComboFunctions.IsOnCooldown(RiddleOfWind))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    {
                        CurrentState = OpenerState.FailedOpener;
                        return false;
                    }

                }

                else
                {
                    if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 1 && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = PerfectBalance;

                    if (CustomComboFunctions.WasLastAction(TwinSnakes) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = TwinSnakes;

                    if (CustomComboFunctions.WasLastAction(RiddleOfFire) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = RiddleOfFire;

                    if (CustomComboFunctions.WasLastAction(Demolish) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Demolish;

                    if (CustomComboFunctions.WasLastAction(ForbiddenChakra) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = ForbiddenChakra;

                    if (CustomComboFunctions.WasLastAction(Bootshine) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Bootshine;

                    if (CustomComboFunctions.WasLastAction(Brotherhood) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Brotherhood;

                    if (CustomComboFunctions.WasLastAction(RisingPhoenix) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = RisingPhoenix;

                    if (CustomComboFunctions.WasLastAction(RiddleOfWind) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = RiddleOfWind;

                    if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0 && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = PerfectBalance;

                    if (CustomComboFunctions.WasLastAction(Bootshine) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = Bootshine;

                    if (CustomComboFunctions.WasLastAction(SnapPunch) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = SnapPunch;

                    if (CustomComboFunctions.WasLastAction(TwinSnakes) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = TwinSnakes;

                    if (CustomComboFunctions.WasLastAction(RisingPhoenix) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = RisingPhoenix;

                    if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = DragonKick;

                    if (CustomComboFunctions.WasLastAction(TrueStrike) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = TrueStrike;

                    if (CustomComboFunctions.WasLastAction(Demolish) && OpenerStep == 19) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 19) actionID = Demolish;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0) ||
                       (actionID == Brotherhood && CustomComboFunctions.IsOnCooldown(Brotherhood)) ||
                       (actionID == RiddleOfFire && CustomComboFunctions.IsOnCooldown(RiddleOfFire)) ||
                       (actionID == RiddleOfWind && CustomComboFunctions.IsOnCooldown(RiddleOfWind))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
                if (CustomComboFunctions.WasLastAction(DragonKick) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(TwinSnakes) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = TwinSnakes;

                if (CustomComboFunctions.WasLastAction(RiddleOfFire) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = RiddleOfFire;

                if (CustomComboFunctions.WasLastAction(Demolish) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Demolish;

                if (CustomComboFunctions.WasLastAction(ForbiddenChakra) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = ForbiddenChakra;

                if (CustomComboFunctions.WasLastAction(Bootshine) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Bootshine;

                if (CustomComboFunctions.WasLastAction(Brotherhood) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Brotherhood;

                if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 1 && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = PerfectBalance;

                if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(RiddleOfWind) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = RiddleOfWind;

                if (CustomComboFunctions.WasLastAction(Bootshine) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Bootshine;

                if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(ElixirField) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = ElixirField;

                if (CustomComboFunctions.WasLastAction(Bootshine) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Bootshine;

                if (CustomComboFunctions.WasLastAction(PerfectBalance) && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0 && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = PerfectBalance;

                if (CustomComboFunctions.WasLastAction(TwinSnakes) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 2 && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = TwinSnakes;

                if (CustomComboFunctions.WasLastAction(DragonKick) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 1 && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = DragonKick;

                if (CustomComboFunctions.WasLastAction(Demolish) && CustomComboFunctions.GetBuffStacks(Buffs.PerfectBalance) == 0 && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Demolish;

                if (CustomComboFunctions.WasLastAction(RisingPhoenix) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = RisingPhoenix;

                if (CustomComboFunctions.WasLastAction(TwinSnakes) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = TwinSnakes;

                if (CustomComboFunctions.WasLastAction(SnapPunch) && OpenerStep == 21) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 21) actionID = SnapPunch;


                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0) ||
                       (actionID == Brotherhood && CustomComboFunctions.IsOnCooldown(Brotherhood)) ||
                       (actionID == RiddleOfFire && CustomComboFunctions.IsOnCooldown(RiddleOfFire)) ||
                       (actionID == RiddleOfWind && CustomComboFunctions.IsOnCooldown(RiddleOfWind))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
                if (DoPrePullSteps(ref actionID)) return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpenerSimple(ref actionID)) return true;
                }
                else
                {
                    if (DoOpener(ref actionID)) return true;
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
}