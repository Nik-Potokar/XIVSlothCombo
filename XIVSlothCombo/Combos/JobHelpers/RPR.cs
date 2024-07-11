using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class RPROpenerLogic : RPR
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

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(ShadowOfDeath) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = ShadowOfDeath;

                if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = SoulSlice;

                if (CustomComboFunctions.WasLastAction(ArcaneCircle) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = ArcaneCircle;

                if (CustomComboFunctions.WasLastAction(Gluttony) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Gluttony;

                if (CustomComboFunctions.WasLastAction(ExecutionersGibbet) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = ExecutionersGibbet;

                if (CustomComboFunctions.WasLastAction(ExecutionersGallows) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = ExecutionersGallows;

                if (CustomComboFunctions.WasLastAction(PlentifulHarvest) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = PlentifulHarvest;

                if (CustomComboFunctions.WasLastAction(Enshroud) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = Enshroud;

                if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = VoidReaping;

                if (CustomComboFunctions.WasLastAction(Sacrificium) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Sacrificium;

                if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = CrossReaping;

                if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = LemuresSlice;

                if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = VoidReaping;

                if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = CrossReaping;

                if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = LemuresSlice;

                if (CustomComboFunctions.WasLastAction(Communio) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Communio;

                if (CustomComboFunctions.WasLastAction(Perfectio) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Perfectio;

                if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = SoulSlice;

                if (CustomComboFunctions.WasLastAction(UnveiledGibbet) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = UnveiledGibbet;

                if (CustomComboFunctions.WasLastAction(Gibbet) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Gibbet;

                if (CustomComboFunctions.WasLastAction(ShadowOfDeath) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = ShadowOfDeath;

                if (CustomComboFunctions.WasLastAction(Slice) && OpenerStep == 22) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 22) actionID = Slice;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
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