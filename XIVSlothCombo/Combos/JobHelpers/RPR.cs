using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class RPROpenerLogic : PvE.RPR
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SoulSlice) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ArcaneCircle))
                return false;

            if (!CustomComboFunctions.ActionReady(Gluttony))
                return false;

            return true;
        }

        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        private static uint[] StandardOpenerFlankFirst = [
            ShadowOfDeath,
            SoulSlice,
            ArcaneCircle,
            Gluttony,
            ExecutionersGibbet,
            ExecutionersGallows,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            Sacrificium,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            Perfectio,
            SoulSlice,
            UnveiledGibbet,
            Gibbet,
            ShadowOfDeath,
            Slice];

        private static uint[] StandardOpenerRearFirst = [
           ShadowOfDeath,
            SoulSlice,
            ArcaneCircle,
            Gluttony,
            ExecutionersGallows,
            ExecutionersGibbet,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            Sacrificium,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            Perfectio,
            SoulSlice,
            UnveiledGallows,
            Gallows,
            ShadowOfDeath,
            Slice];

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
                if (CustomComboFunctions.HasEffect(Buffs.Soulsow) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = Soulsow;

                if (CustomComboFunctions.LocalPlayer.CastActionId == Harpe && CustomComboFunctions.HasEffect(Buffs.Soulsow) && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = Harpe;

                if (PrePullStep == 2 && !CustomComboFunctions.HasEffect(Buffs.Soulsow))
                    CurrentState = OpenerState.FailedOpener;

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
                if (CustomComboFunctions.WasLastAction(OpenerActions[OpenerStep]))
                    OpenerStep++;

                if (OpenerStep == OpenerActions.Length)
                    CurrentState = OpenerState.OpenerFinished;

                else actionID = OpenerActions[OpenerStep];

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == SoulSlice && CustomComboFunctions.GetRemainingCharges(SoulSlice) == 0) ||
                       (actionID == ArcaneCircle && CustomComboFunctions.IsOnCooldown(ArcaneCircle)) ||
                       (actionID == Gluttony && CustomComboFunctions.IsOnCooldown(Gluttony))) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
                    if (DoOpener(StandardOpenerFlankFirst, ref actionID))
                        return true;
                }

                else

                if (Config.RPR_Positional == 0)
                {
                    if (DoOpener(StandardOpenerRearFirst, ref actionID))
                        return true;
                }

                if (Config.RPR_Positional == 1)
                {
                    if (DoOpener(StandardOpenerFlankFirst, ref actionID))
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