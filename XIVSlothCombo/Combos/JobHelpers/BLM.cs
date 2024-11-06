using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.PvE.BLM;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal class BLM
{
    // BLM Gauge & Extensions
    public static BLMGauge Gauge => GetJobGauge<BLMGauge>();

    public static Status? thunderDebuffST =
        FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer.GameObjectId);

    public static Status? thunderDebuffAoE =
        FindEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget, LocalPlayer.GameObjectId);

    public static float elementTimer = Gauge.ElementTimeRemaining / 1000f;
    public static double gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(ActionWatching.LastSpell));

    public static int Fire4Count => ActionWatching.CombatActions.Count(x => x == Fire4);

    public static bool HasPolyglotStacks(BLMGauge gauge) => gauge.PolyglotStacks > 0;

    internal class BLMOpenerLogic
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep = 1;

        public uint PrePullStep;

        private static uint OpenerLevel => 100;

        public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
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

        private static bool HasCooldowns()
        {
            if (!ActionReady(Manafont))
                return false;

            if (GetRemainingCharges(Triplecast) < 2)
                return false;

            if (!ActionReady(All.Swiftcast))
                return false;

            if (!ActionReady(Amplifier))
                return false;

            if (!ActionReady(LeyLines))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (WasLastAction(Fire3) && HasEffect(Buffs.Thunderhead) && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = Fire3;

                if (ActionWatching.CombatActions.Count > 2 && InCombat())
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
                if (WasLastAction(HighThunder) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = HighThunder;

                if (WasLastAction(All.Swiftcast) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = All.Swiftcast;

                if (WasLastAction(Amplifier) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Amplifier;

                if (WasLastAction(Fire4) && Fire4Count is 1 && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Fire4;

                if (WasLastAction(Fire4) && Fire4Count is 2 && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Fire4;

                if (WasLastAction(Xenoglossy) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Xenoglossy;

                if (WasLastAction(Triplecast) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Triplecast;

                if (WasLastAction(LeyLines) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = LeyLines;

                if (WasLastAction(Fire4) && Fire4Count is 3 && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Fire4;

                if (WasLastAction(Fire4) && Fire4Count is 4 && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Fire4;

                if (WasLastAction(Despair) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Despair;

                if (WasLastAction(Manafont) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Manafont;

                if (WasLastAction(Triplecast) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Triplecast;

                if (WasLastAction(Fire4) && Fire4Count is 5 && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Fire4;

                if (WasLastAction(Fire4) && Fire4Count is 6 && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Fire4;

                if (WasLastAction(FlareStar) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = FlareStar;

                if (WasLastAction(Fire4) && Fire4Count is 7 && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Fire4;

                if (WasLastAction(HighThunder) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = HighThunder;

                if (WasLastAction(Paradox) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Paradox;

                if (WasLastAction(Fire4) && Fire4Count is 8 && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Fire4;

                if (WasLastAction(Fire4) && Fire4Count is 9 && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Fire4;

                if (WasLastAction(Fire4) && Fire4Count is 10 && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = Fire4;

                if (WasLastAction(Despair) && OpenerStep == 23) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 23) actionID = Despair;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Triplecast && GetRemainingCharges(Triplecast) == 0) ||
                     (actionID == Amplifier && IsOnCooldown(Amplifier)) ||
                     (actionID == LeyLines && IsOnCooldown(LeyLines)) ||
                     (actionID == Manafont && IsOnCooldown(Manafont)) ||
                     (actionID == All.Swiftcast && IsOnCooldown(All.Swiftcast)) ||
                     (actionID == Xenoglossy && Gauge.PolyglotStacks == 0)) &&
                    ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
            if (!LevelChecked) return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    internal class BLMHelper
    {
        public static float MPAfterCast()
        {
            uint castedSpell = LocalPlayer.CastActionId;

            int nextMpGain = Gauge.UmbralIceStacks switch
            {
                0 => 0,
                1 => 2500,
                2 => 5000,
                3 => 10000,
                _ => 0
            };

            if (castedSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
                return Math.Max(LocalPlayer.MaxMp, LocalPlayer.CurrentMp + nextMpGain);

            return Math.Max(0, LocalPlayer.CurrentMp - GetResourceCost(castedSpell));
        }

        public static bool DoubleBlizz()
        {
            List<uint> spells = ActionWatching.CombatActions.Where(x =>
                ActionWatching.GetAttackType(x) == ActionWatching.ActionAttackType.Spell &&
                x != OriginalHook(Thunder) && x != OriginalHook(Thunder2)).ToList();

            if (spells.Count < 1) return false;

            uint firstSpell = spells[^1];

            if (firstSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
            {
                uint castedSpell = LocalPlayer.CastActionId;

                if (castedSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
                    return true;

                if (spells.Count >= 2)
                {
                    uint secondSpell = spells[^2];

                    if (secondSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
                        return true;
                }
            }

            return false;
        }
    }
}