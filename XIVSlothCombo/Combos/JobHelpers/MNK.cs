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

            if (Gauge.Nadi != Dalamud.Game.ClientState.JobGauge.Enums.Nadi.NONE)
                return false;

            if (Gauge.RaptorFury != 0) return false;
            if (Gauge.CoeurlFury != 0) return false;

            return true;
        }

        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        private OpenerState currentState = OpenerState.PrePull;

        private uint ActionBeforeTFC = 0;

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

                if (!CustomComboFunctions.HasEffect(Buffs.FormlessFist) && !CustomComboFunctions.HasEffect(Buffs.RaptorForm) && PrePullStep == 1)
                {
                    actionID = FormShift;
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

        private bool WasLastActionNoTFC(uint actionID)
        {
            return CustomComboFunctions.WasLastAction(TheForbiddenChakra) || CustomComboFunctions.WasLastAction(ActionBeforeTFC);
        }

        private bool DoSLOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra)
                    && Gauge.Chakra >= 5
                    && OpenerStep > 2)
                {
                    actionID = TheForbiddenChakra;
                    return true;
                }

                if ((WasLastActionNoTFC(PerfectBalance) || CustomComboFunctions.HasEffect(Buffs.PerfectBalance)) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = PerfectBalance;

                if (WasLastActionNoTFC(TheForbiddenChakra) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = TheForbiddenChakra;

                if (WasLastActionNoTFC(TwinSnakes) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = TwinSnakes;

                if (WasLastActionNoTFC(Demolish) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Demolish;

                if (WasLastActionNoTFC(Brotherhood) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Brotherhood;

                if (WasLastActionNoTFC(RiddleOfFire) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = RiddleOfFire;

                // Pot

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = LeapingOpo;

                if (WasLastActionNoTFC(RiddleOfWind) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = RiddleOfWind;

                if (WasLastActionNoTFC(RisingPhoenix) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = RisingPhoenix;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = DragonKick;

                if ((WasLastActionNoTFC(FiresReply) || !CustomComboFunctions.HasEffect(Buffs.FiresRumination)) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = FiresReply;

                if ((WasLastActionNoTFC(WindsReply) || !CustomComboFunctions.HasEffect(Buffs.WindsRumination)) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = WindsReply;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = LeapingOpo;

                if ((WasLastActionNoTFC(PerfectBalance) || CustomComboFunctions.HasEffect(Buffs.PerfectBalance)) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = PerfectBalance;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = DragonKick;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = LeapingOpo;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = DragonKick;

                if (WasLastActionNoTFC(ElixirBurst) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = ElixirBurst;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 19) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 19) actionID = LeapingOpo;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3) ||
                       (OpenerStep is 6 && CustomComboFunctions.HasEffect(Buffs.RiddleOfFire)) ||
                       (OpenerStep is 8 && CustomComboFunctions.HasEffect(Buffs.RiddleOfWind)) ||
                       (OpenerStep is 5 && CustomComboFunctions.HasEffect(Buffs.Brotherhood))
                       && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    Svc.Log.Debug($"Failed at {actionID}");
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                ActionBeforeTFC = actionID;
                return true;
            }
            return false;
        }

        private bool DoLLOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra)
                    && Gauge.Chakra >= 5
                    && OpenerStep > 2)
                {
                    actionID = TheForbiddenChakra;
                    return true;
                }

                if ((WasLastActionNoTFC(PerfectBalance) || CustomComboFunctions.HasEffect(Buffs.PerfectBalance)) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = PerfectBalance;

                if (WasLastActionNoTFC(TheForbiddenChakra) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = TheForbiddenChakra;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = LeapingOpo;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = DragonKick;

                if (WasLastActionNoTFC(Brotherhood) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Brotherhood;

                if (WasLastActionNoTFC(RiddleOfFire) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = RiddleOfFire;

                // Pot

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = LeapingOpo;

                if (WasLastActionNoTFC(RiddleOfWind) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = RiddleOfWind;

                if (WasLastActionNoTFC(ElixirBurst) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = ElixirBurst;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = DragonKick;

                if ((WasLastActionNoTFC(FiresReply) || !CustomComboFunctions.HasEffect(Buffs.FiresRumination)) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = FiresReply;

                if ((WasLastActionNoTFC(WindsReply) || !CustomComboFunctions.HasEffect(Buffs.WindsRumination)) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = WindsReply;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = LeapingOpo;

                if ((WasLastActionNoTFC(PerfectBalance) || CustomComboFunctions.HasEffect(Buffs.PerfectBalance)) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = PerfectBalance;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = DragonKick;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = LeapingOpo;

                if (WasLastActionNoTFC(DragonKick) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = DragonKick;

                if (WasLastActionNoTFC(ElixirBurst) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = ElixirBurst;

                if (WasLastActionNoTFC(LeapingOpo) && OpenerStep == 19) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 19) actionID = LeapingOpo;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PerfectBalance && CustomComboFunctions.GetRemainingCharges(PerfectBalance) == 0) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3) ||
                       (OpenerStep is 6 && CustomComboFunctions.HasEffect(Buffs.RiddleOfFire)) ||
                       (OpenerStep is 8 && CustomComboFunctions.HasEffect(Buffs.RiddleOfWind)) ||
                       (OpenerStep is 5 && CustomComboFunctions.HasEffect(Buffs.Brotherhood))
                       && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    Svc.Log.Debug($"Failed at {actionID}");
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                ActionBeforeTFC = actionID;
                return true;
            }
            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID, int selectedOpener)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (selectedOpener == 1)
                {
                    if (DoLLOpener(ref actionID))
                        return true;
                }
                else if (selectedOpener == 2)
                {
                    if (DoSLOpener(ref actionID))
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
}