using Dalamud.Game.ClientState.JobGauge.Enums;
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
            if (Svc.Gauges.Get<MNKGauge>().Nadi.HasFlag(Nadi.LUNAR))
                return false;
            if (Svc.Gauges.Get<MNKGauge>().Nadi.HasFlag(Nadi.SOLAR))
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        private static uint[] LunarSolarOpener = [
            DragonKick,
            TwinSnakes,
            RiddleOfFire,
            Demolish,
            Bootshine,
            Brotherhood,
            PerfectBalance,
            DragonKick,
            RiddleOfWind,
            Bootshine,
            DragonKick,
            ElixirField,
            Bootshine,
            PerfectBalance,
            TwinSnakes,
            DragonKick,
            Demolish,
            RisingPhoenix,
            TwinSnakes,
            SnapPunch];

        private static uint[] DoubleSolarOpener = [
            DragonKick,
            PerfectBalance,
            TwinSnakes,
            RiddleOfFire,
            Demolish,
            Bootshine,
            Brotherhood,
            RisingPhoenix,
            RiddleOfWind,
            DragonKick,
            PerfectBalance,
            Bootshine,
            SnapPunch,
            TwinSnakes,
            RisingPhoenix,
            DragonKick,
            TrueStrike,
            Demolish];

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
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0)
                PrePullStep = 1;

            if (!HasCooldowns())
                PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {

                if (Svc.Gauges.Get<MNKGauge>().Chakra == 5 && PrePullStep == 1)
                    PrePullStep++;

                else if (PrePullStep == 1)
                    actionID = Meditation;

                if (CustomComboFunctions.HasEffect(Buffs.FormlessFist) && PrePullStep == 2)
                    CurrentState = OpenerState.InOpener;

                else if (PrePullStep == 2)
                    actionID = FormShift;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(uint[] OpenerActions, ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(OpenerActions[OpenerStep])) OpenerStep++;

                if (OpenerStep > 3 && Svc.Gauges.Get<MNKGauge>().Chakra == 5)
                    actionID = ForbiddenChakra;

                else if (OpenerStep == OpenerActions.Length)
                    CurrentState = OpenerState.OpenerFinished;

                else actionID = OpenerActions[OpenerStep];

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
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
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpener(LunarSolarOpener, ref actionID))
                        return true;
                }

                else
                {
                    if (Config.MNK_OpenerChoice == 0) // Lunar Solar opener choosen
                    {
                        if (DoOpener(LunarSolarOpener, ref actionID))
                            return true;
                    }

                    else
                    {
                        if (DoOpener(DoubleSolarOpener, ref actionID))
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
}