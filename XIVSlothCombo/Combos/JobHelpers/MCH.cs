using Dalamud.Game.ClientState.Conditions;
using ECommons.Logging;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MCHOpenerLogic : PvE.MCH
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Ricochet) < 3)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(GaussRound) < 3)
                return false;
            if (!CustomComboFunctions.ActionReady(ChainSaw))
                return false;
            if (!CustomComboFunctions.ActionReady(Wildfire))
                return false;
            if (!CustomComboFunctions.ActionReady(BarrelStabilizer))
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;
        public uint PrePullStep = 1;
        public uint OpenerStep = 1;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private bool CanOpener => HasCooldowns() && LevelChecked;

        private OpenerState currentState = OpenerState.OpenerFinished;

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
                    if (value == OpenerState.PrePull) PrePullStep = 1;
                    if (value == OpenerState.InOpener) OpenerStep = 1;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener) { PrePullStep = 0; OpenerStep = 0; }
                    if (value == OpenerState.OpenerFinished) DuoLog.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0 && !CustomComboFunctions.InCombat())
            {
                CurrentState = OpenerState.PrePull;
            }

            if (CurrentState == OpenerState.PrePull)
            {
                if (Config.MCH_ST_OpenerSelection == 0 || Config.MCH_ST_OpenerSelection == 1)
                {
                    if (CustomComboFunctions.WasLastAction(HeatedSplitShot) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = HeatedSplitShot;
                }

                if (Config.MCH_ST_OpenerSelection == 2)
                {
                    if (CustomComboFunctions.HasEffect(Buffs.Reassembled) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = Reassemble;
                }
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
                if (Config.MCH_ST_OpenerSelection == 0)
                {
                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Drill;

                    if (CustomComboFunctions.WasLastAction(BarrelStabilizer) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = BarrelStabilizer;

                    if (CustomComboFunctions.WasLastAction(HeatedSlugshot) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = HeatedSlugshot;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatedCleanShot) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = HeatedCleanShot;

                    if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Reassemble;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(AirAnchor) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = AirAnchor;

                    if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Reassemble;

                    if (CustomComboFunctions.WasLastAction(Wildfire) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = Wildfire;

                    if (CustomComboFunctions.WasLastAction(ChainSaw) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = ChainSaw;

                    if (CustomComboFunctions.WasLastAction(AutomatonQueen) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = AutomatonQueen;

                    if (CustomComboFunctions.WasLastAction(Hypercharge) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Hypercharge;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 4 && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 3 && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 2 && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 1 && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 0 && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 26) actionID = Drill;
                }

                else if (Config.MCH_ST_OpenerSelection == 1)
                {
                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatedSlugshot) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = HeatedSlugshot;

                    if (CustomComboFunctions.WasLastAction(BarrelStabilizer) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = BarrelStabilizer;

                    if (CustomComboFunctions.WasLastAction(HeatedCleanShot) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = HeatedCleanShot;

                    if (CustomComboFunctions.WasLastAction(AirAnchor) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = AirAnchor;

                    if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Reassemble;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = Drill;

                    if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = Reassemble;

                    if (CustomComboFunctions.WasLastAction(Wildfire) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Wildfire;

                    if (CustomComboFunctions.WasLastAction(ChainSaw) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = ChainSaw;

                    if (CustomComboFunctions.WasLastAction(AutomatonQueen) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = AutomatonQueen;

                    if (CustomComboFunctions.WasLastAction(Hypercharge) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Hypercharge;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 4 && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 3 && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 2 && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 1 && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 0 && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatedSplitShot) && OpenerStep == 25) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 25) actionID = HeatedSplitShot;
                }

                else
                {
                    if (CustomComboFunctions.WasLastAction(AirAnchor) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = AirAnchor;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Drill;

                    if (CustomComboFunctions.WasLastAction(BarrelStabilizer) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = BarrelStabilizer;

                    if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Reassemble;

                    if (CustomComboFunctions.WasLastAction(ChainSaw) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = ChainSaw;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatedSplitShot) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = HeatedSplitShot;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatedSlugshot) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = HeatedSlugshot;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(Wildfire) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Wildfire;

                    if (CustomComboFunctions.WasLastAction(HeatedCleanShot) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = HeatedCleanShot;

                    if (CustomComboFunctions.WasLastAction(AutomatonQueen) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = AutomatonQueen;

                    if (CustomComboFunctions.WasLastAction(Hypercharge) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Hypercharge;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 4 && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 3 && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 2 && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = GaussRound;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 1 && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = HeatBlast;

                    if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = Ricochet;

                    if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 0 && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 26) actionID = HeatBlast;
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
                if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = GaussRound;

                if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Ricochet;

                if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Drill;

                if (CustomComboFunctions.WasLastAction(BarrelStabilizer) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = BarrelStabilizer;

                if (CustomComboFunctions.WasLastAction(HeatedSlugshot) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = HeatedSlugshot;

                if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Ricochet;

                if (CustomComboFunctions.WasLastAction(HeatedCleanShot) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = HeatedCleanShot;

                if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = Reassemble;

                if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = GaussRound;

                if (CustomComboFunctions.WasLastAction(AirAnchor) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = AirAnchor;

                if (CustomComboFunctions.WasLastAction(Reassemble) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Reassemble;

                if (CustomComboFunctions.WasLastAction(Wildfire) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Wildfire;

                if (CustomComboFunctions.WasLastAction(ChainSaw) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = ChainSaw;

                if (CustomComboFunctions.WasLastAction(AutomatonQueen) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = AutomatonQueen;

                if (CustomComboFunctions.WasLastAction(Hypercharge) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Hypercharge;

                if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 4 && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = HeatBlast;

                if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Ricochet;

                if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 3 && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = HeatBlast;

                if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = GaussRound;

                if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 2 && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = HeatBlast;

                if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Ricochet;

                if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 1 && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = HeatBlast;

                if (CustomComboFunctions.WasLastAction(GaussRound) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = GaussRound;

                if (CustomComboFunctions.WasLastAction(HeatBlast) && CustomComboFunctions.GetBuffStacks(Buffs.Overheated) == 0 && OpenerStep == 24) OpenerStep++;
                else if (OpenerStep == 24) actionID = HeatBlast;

                if (CustomComboFunctions.WasLastAction(Ricochet) && OpenerStep == 25) OpenerStep++;
                else if (OpenerStep == 25) actionID = Ricochet;

                if (CustomComboFunctions.WasLastAction(Drill) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 26) actionID = Drill;

                return true;
            }
            return false;
        }

        private void ResetOpener()
        {
            CurrentState = OpenerState.FailedOpener;
        }

        private bool openerEventsSetup = false;

        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked) return false;

            if (!openerEventsSetup) { Service.Condition.ConditionChange += CheckCombatStatus; openerEventsSetup = true; }

            if (CurrentState == OpenerState.PrePull || CurrentState == OpenerState.FailedOpener)
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

            if (CurrentState == OpenerState.OpenerFinished && !CustomComboFunctions.InCombat())
                ResetOpener();

            return false;
        }

        private void CheckCombatStatus(ConditionFlag flag, bool value)
        {
            if (flag == ConditionFlag.InCombat && value == false) ResetOpener();
        }

        internal void Dispose()
        {
            Service.Condition.ConditionChange -= CheckCombatStatus;
        }
    }
}
