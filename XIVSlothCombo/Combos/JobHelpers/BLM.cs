using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class BLMOpenerLogic : PvE.BLM
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Triplecast) < 2)
                return false;
            if (!CustomComboFunctions.ActionReady(Manafont))
                return false;
            if (!CustomComboFunctions.ActionReady(All.Swiftcast))
                return false;
            if (!CustomComboFunctions.ActionReady(Amplifier))
                return false;
            if (!CustomComboFunctions.ActionReady(All.LucidDreaming) &&
                Config.BLM_Advanced_OpenerSelection == 1)
                return false;
            if (!CustomComboFunctions.ActionReady(LeyLines))
                return false;

            return true;
        }

        public static bool HasPrePullCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Sharpcast) < 2)
                return false;

            if (CustomComboFunctions.LocalPlayer.CurrentMp < 10000)
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 1;

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
                if (CustomComboFunctions.HasEffect(Buffs.Sharpcast) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = Sharpcast;

                if (CustomComboFunctions.LocalPlayer.CastActionId == Fire3 && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = Fire3;

                if (PrePullStep == 2 && !CustomComboFunctions.HasEffect(Buffs.Sharpcast))
                    CurrentState = OpenerState.FailedOpener;

                if (PrePullStep > 1 && CustomComboFunctions.GetResourceCost(actionID) > CustomComboFunctions.LocalPlayer.CurrentMp && ActionWatching.TimeSinceLastAction.TotalSeconds >= 2)
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
                if (Config.BLM_Advanced_OpenerSelection == 0)
                {
                    if (CustomComboFunctions.LocalPlayer.CastActionId == Thunder3 && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = Thunder3;

                    if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Triplecast;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 2 && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 1 && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Amplifier) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Amplifier;

                    if (CustomComboFunctions.WasLastAction(LeyLines) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = LeyLines;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 0 && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = All.Swiftcast;

                    if (CustomComboFunctions.WasLastAction(Fire4) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = Triplecast;

                    if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Despair;

                    if (CustomComboFunctions.WasLastAction(Manafont) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = Manafont;

                    if (CustomComboFunctions.WasLastAction(Fire4) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Sharpcast) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Sharpcast;

                    if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Despair;

                    if (CustomComboFunctions.WasLastAction(Blizzard3) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = Blizzard3;

                    if (CustomComboFunctions.WasLastAction(Xenoglossy) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Xenoglossy;

                    if (CustomComboFunctions.WasLastAction(Paradox) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = Paradox;

                    if (CustomComboFunctions.LocalPlayer.CastActionId == Blizzard4 && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Blizzard4;

                    if (CustomComboFunctions.WasLastAction(Thunder3) && OpenerStep == 20) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 20) actionID = Thunder3;

                    if (((actionID == Triplecast && CustomComboFunctions.GetRemainingCharges(Triplecast) < 2) ||
                        (actionID == Amplifier && CustomComboFunctions.IsOnCooldown(Amplifier)) ||
                        (actionID == LeyLines && CustomComboFunctions.IsOnCooldown(LeyLines)) ||
                        (actionID == All.LucidDreaming && CustomComboFunctions.IsOnCooldown(All.LucidDreaming)) ||
                        (actionID == Manafont && CustomComboFunctions.IsOnCooldown(Manafont)) ||
                        (actionID == Sharpcast && CustomComboFunctions.GetRemainingCharges(Sharpcast) < 1) ||
                        (actionID == All.Swiftcast && CustomComboFunctions.IsOnCooldown(All.Swiftcast)) ||
                        (actionID == Xenoglossy && Svc.Gauges.Get<BLMGauge>().PolyglotStacks < 1)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    {
                        CurrentState = OpenerState.FailedOpener;
                        return false;
                    }
                }

                else
                {

                    if (CustomComboFunctions.LocalPlayer.CastActionId == Thunder3 && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = Thunder3;

                    if (CustomComboFunctions.LocalPlayer.CastActionId == Fire4 && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Triplecast;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 2 && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 1 && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Amplifier) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Amplifier;

                    if (CustomComboFunctions.WasLastAction(LeyLines) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = LeyLines;

                    if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 0 && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = Triplecast;

                    if (CustomComboFunctions.WasLastAction(All.LucidDreaming) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = All.LucidDreaming;

                    if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = Despair;

                    if (CustomComboFunctions.WasLastAction(Manafont) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = Manafont;

                    if (CustomComboFunctions.WasLastAction(Fire4) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Sharpcast) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Sharpcast;

                    if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Despair;

                    if (CustomComboFunctions.WasLastAction(Transpose) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = Transpose;

                    if (CustomComboFunctions.WasLastAction(Paradox) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Paradox;

                    if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = All.Swiftcast;

                    if (CustomComboFunctions.WasLastAction(Xenoglossy) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Xenoglossy;

                    if ((CustomComboFunctions.LocalPlayer.CastActionId == Thunder3 || CustomComboFunctions.WasLastAction(Thunder3)) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = Thunder3;

                    if (CustomComboFunctions.LocalPlayer.CurrentMp == CustomComboFunctions.LocalPlayer.MaxMp && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = Blizzard3;

                    if (CustomComboFunctions.WasLastAction(Transpose) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = Transpose;

                    if ((CustomComboFunctions.LocalPlayer.CastActionId == Fire3 || CustomComboFunctions.WasLastAction(Fire3)) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = Fire3;

                    if ((CustomComboFunctions.LocalPlayer.CastActionId == Fire4 || CustomComboFunctions.WasLastAction(Fire4)) && OpenerStep == 24 && ActionWatching.CombatActions.Count == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Fire4;

                    if ((CustomComboFunctions.LocalPlayer.CastActionId == Fire4 || CustomComboFunctions.WasLastAction(Fire4)) && OpenerStep == 25 && ActionWatching.CombatActions.Count == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = Fire4;

                    if ((CustomComboFunctions.LocalPlayer.CastActionId == Fire4 || CustomComboFunctions.WasLastAction(Fire4)) && OpenerStep == 26 && ActionWatching.CombatActions.Count == 26) OpenerStep++;
                    else if (OpenerStep == 26) actionID = Fire4;

                    if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 27) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 27) actionID = Despair;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (CustomComboFunctions.GetResourceCost(actionID) > CustomComboFunctions.LocalPlayer.CurrentMp && ActionWatching.TimeSinceLastAction.TotalSeconds >= 2)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Triplecast && CustomComboFunctions.GetRemainingCharges(Triplecast) < 2) ||
                    (actionID == Amplifier && CustomComboFunctions.IsOnCooldown(Amplifier)) ||
                    (actionID == LeyLines && CustomComboFunctions.IsOnCooldown(LeyLines)) ||
                    (actionID == All.LucidDreaming && CustomComboFunctions.IsOnCooldown(All.LucidDreaming)) ||
                    (actionID == Manafont && CustomComboFunctions.IsOnCooldown(Manafont)) ||
                    (actionID == Sharpcast && CustomComboFunctions.GetRemainingCharges(Sharpcast) < 1) ||
                    (actionID == All.Swiftcast && CustomComboFunctions.IsOnCooldown(All.Swiftcast)) ||
                    (actionID == Xenoglossy && Svc.Gauges.Get<BLMGauge>().PolyglotStacks < 1)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
                if (CustomComboFunctions.LocalPlayer.CastActionId == Thunder3 && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Thunder3;

                if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Triplecast;

                if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 2 && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Fire4;

                if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 1 && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Fire4;

                if (CustomComboFunctions.WasLastAction(Amplifier) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Amplifier;

                if (CustomComboFunctions.WasLastAction(LeyLines) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = LeyLines;

                if (CustomComboFunctions.WasLastAction(Fire4) && CustomComboFunctions.GetBuffStacks(Buffs.Triplecast) == 0 && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Fire4;

                if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = All.Swiftcast;

                if (CustomComboFunctions.WasLastAction(Fire4) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Fire4;

                if (CustomComboFunctions.WasLastAction(Triplecast) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Triplecast;

                if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Despair;

                if (CustomComboFunctions.WasLastAction(Manafont) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Manafont;

                if (CustomComboFunctions.WasLastAction(Fire4) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Fire4;

                if (CustomComboFunctions.WasLastAction(Sharpcast) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Sharpcast;

                if (CustomComboFunctions.WasLastAction(Despair) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Despair;

                if (CustomComboFunctions.WasLastAction(Blizzard3) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Blizzard3;

                if (CustomComboFunctions.WasLastAction(Xenoglossy) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Xenoglossy;

                if (CustomComboFunctions.WasLastAction(Paradox) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Paradox;

                if (CustomComboFunctions.LocalPlayer.CastActionId == Blizzard4 && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Blizzard4;

                if (CustomComboFunctions.WasLastAction(Thunder3) && OpenerStep == 20) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 20) actionID = Thunder3;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (CustomComboFunctions.GetResourceCost(actionID) > CustomComboFunctions.LocalPlayer.CurrentMp && ActionWatching.TimeSinceLastAction.TotalSeconds >= 2)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Triplecast && CustomComboFunctions.GetRemainingCharges(Triplecast) < 2) ||
                    (actionID == Amplifier && CustomComboFunctions.IsOnCooldown(Amplifier)) ||
                    (actionID == LeyLines && CustomComboFunctions.IsOnCooldown(LeyLines)) ||
                    (actionID == All.LucidDreaming && CustomComboFunctions.IsOnCooldown(All.LucidDreaming)) ||
                    (actionID == Manafont && CustomComboFunctions.IsOnCooldown(Manafont)) ||
                    (actionID == Sharpcast && CustomComboFunctions.GetRemainingCharges(Sharpcast) < 1) ||
                    (actionID == All.Swiftcast && CustomComboFunctions.IsOnCooldown(All.Swiftcast)) ||
                    (actionID == Xenoglossy && Svc.Gauges.Get<BLMGauge>().PolyglotStacks < 1)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

    internal static class BLMHelpers
    {
        public static bool HasPolyglotStacks(this BLMGauge gauge) => gauge.PolyglotStacks > 0;
    }
}