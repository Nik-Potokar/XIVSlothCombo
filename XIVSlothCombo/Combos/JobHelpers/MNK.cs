using Dalamud.Game.ClientState.JobGauge.Enums;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Data;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal abstract class MNKHelper : MNK
{
    public static uint DetermineCoreAbility(uint actionId, bool useTrueNorthIfEnabled)
    {
        if (HasEffect(Buffs.OpoOpoForm) || HasEffect(Buffs.FormlessFist))
            return Gauge.OpoOpoFury == 0 && LevelChecked(DragonKick)
                ? DragonKick
                : OriginalHook(Bootshine);

        if (HasEffect(Buffs.RaptorForm))
            return Gauge.RaptorFury == 0 && LevelChecked(TwinSnakes)
                ? TwinSnakes
                : OriginalHook(TrueStrike);

        if (HasEffect(Buffs.CoeurlForm))
        {
            if (Gauge.CoeurlFury == 0 && LevelChecked(Demolish))
            {
                if (!OnTargetsRear() &&
                    TargetNeedsPositionals() &&
                    !HasEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return Demolish;
            }

            if (LevelChecked(SnapPunch))
            {
                if (!OnTargetsFlank() &&
                    TargetNeedsPositionals() &&
                    !HasEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return OriginalHook(SnapPunch);
            }
        }

        return actionId;
    }
}

internal class MNKOpenerLogic : MNK
{
    private OpenerState currentState = OpenerState.PrePull;

    public uint OpenerStep;

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
        if (GetRemainingCharges(PerfectBalance) < 2)
            return false;

        if (!ActionReady(Brotherhood))
            return false;

        if (!ActionReady(RiddleOfFire))
            return false;

        if (!ActionReady(RiddleOfWind))
            return false;

        if (!ActionReady(Meditation) && Gauge.Chakra < 5)
            return false;

        if (Gauge.Nadi != Nadi.NONE)
            return false;

        if (Gauge.RaptorFury != 0 || Gauge.CoeurlFury != 0)
            return false;

        return true;
    }

    private bool DoPrePullSteps(ref uint actionID)
    {
        if (!LevelChecked)
            return false;

        if (CanOpener && PrePullStep == 0) PrePullStep = 1;

        if (!HasCooldowns()) PrePullStep = 0;

        if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
        {
            if (Gauge.Chakra == 5 && PrePullStep == 1) PrePullStep++;
            else if (PrePullStep == 1) actionID = OriginalHook(Meditation);

            if (HasEffect(Buffs.FormlessFist) && Gauge.Chakra == 5 && PrePullStep == 2) PrePullStep++;
            else if (PrePullStep == 2) actionID = FormShift;

            if (WasLastAction(DragonKick) && PrePullStep == 3) CurrentState = OpenerState.InOpener;
            else if (PrePullStep == 3) actionID = DragonKick;

            if (!HasEffect(Buffs.FormlessFist) && Gauge.Chakra == 5 && PrePullStep == 3)
                currentState = OpenerState.FailedOpener;

            if (ActionWatching.CombatActions.Count > 2 && InCombat())
                CurrentState = OpenerState.FailedOpener;

            return true;
        }
        PrePullStep = 0;

        return false;
    }

    private bool DoSlOpener(ref uint actionID)
    {
        if (!LevelChecked)
            return false;

        if (currentState == OpenerState.InOpener)
        {
            if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra) &&
                CanWeave(ActionWatching.LastWeaponskill) &&
                Gauge.Chakra >= 5 &&
                OpenerStep > 9)
            {
                actionID = TheForbiddenChakra;

                return true;
            }

            if (WasLastAction(PerfectBalance) && GetRemainingCharges(PerfectBalance) is 1 && OpenerStep == 1)
                OpenerStep++;
            else if (OpenerStep == 1) actionID = PerfectBalance;

            if (WasLastWeaponskill(TwinSnakes) && OpenerStep == 2) OpenerStep++;
            else if (OpenerStep == 2) actionID = TwinSnakes;

            if (WasLastWeaponskill(Demolish) && OpenerStep == 3) OpenerStep++;
            else if (OpenerStep == 3) actionID = Demolish;

            if (WasLastAbility(Brotherhood) && OpenerStep == 4) OpenerStep++;
            else if (OpenerStep == 4) actionID = Brotherhood;

            if (WasLastAction(RiddleOfFire) && OpenerStep == 5) OpenerStep++;
            else if (OpenerStep == 5 && CanDelayedWeave(ActionWatching.LastWeaponskill)) actionID = RiddleOfFire;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 6) OpenerStep++;
            else if (OpenerStep == 6) actionID = LeapingOpo;

            if (WasLastAction(TheForbiddenChakra) && OpenerStep == 7) OpenerStep++;
            else if (OpenerStep == 7) actionID = TheForbiddenChakra;

            if (WasLastAction(RiddleOfWind) && OpenerStep == 8) OpenerStep++;
            else if (OpenerStep == 8 && CanDelayedWeave(ActionWatching.LastWeaponskill)) actionID = RiddleOfWind;

            if (WasLastWeaponskill(RisingPhoenix) && OpenerStep == 9) OpenerStep++;
            else if (OpenerStep == 9) actionID = RisingPhoenix;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 10) OpenerStep++;
            else if (OpenerStep == 10) actionID = DragonKick;

            if (WasLastWeaponskill(WindsReply) && OpenerStep == 11) OpenerStep++;
            else if (OpenerStep == 11) actionID = WindsReply;

            if (WasLastWeaponskill(FiresReply) && OpenerStep == 12) OpenerStep++;
            else if (OpenerStep == 12) actionID = FiresReply;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 13) OpenerStep++;
            else if (OpenerStep == 13) actionID = LeapingOpo;

            if (WasLastAction(PerfectBalance) && GetRemainingCharges(PerfectBalance) is 0 && OpenerStep == 14)
                OpenerStep++;
            else if (OpenerStep == 14) actionID = PerfectBalance;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 15) OpenerStep++;
            else if (OpenerStep == 15) actionID = DragonKick;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 16) OpenerStep++;
            else if (OpenerStep == 16) actionID = LeapingOpo;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 17) OpenerStep++;
            else if (OpenerStep == 17) actionID = DragonKick;

            if (WasLastWeaponskill(ElixirBurst) && OpenerStep == 18) OpenerStep++;
            else if (OpenerStep == 18) actionID = ElixirBurst;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 19) CurrentState = OpenerState.OpenerFinished;
            else if (OpenerStep == 19) actionID = LeapingOpo;

            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                CurrentState = OpenerState.FailedOpener;

            if (((actionID == PerfectBalance && GetRemainingCharges(PerfectBalance) == 0) ||
                 (actionID is RiddleOfFire && IsOnCooldown(RiddleOfFire)) ||
                 (actionID is RiddleOfWind && IsOnCooldown(RiddleOfWind)) ||
                 (actionID is Brotherhood && IsOnCooldown(Brotherhood)))
                && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
            {
                Svc.Log.Debug($"Failed at {actionID}");
                CurrentState = OpenerState.FailedOpener;

                return false;
            }

            return true;
        }

        return false;
    }

    private bool DoLlOpener(ref uint actionID)
    {
        if (!LevelChecked)
            return false;

        if (currentState == OpenerState.InOpener)
        {
            if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra) &&
                CanWeave(ActionWatching.LastWeaponskill) &&
                Gauge.Chakra >= 5 &&
                OpenerStep > 9)
            {
                actionID = TheForbiddenChakra;

                return true;
            }

            if (WasLastAction(PerfectBalance) && GetRemainingCharges(PerfectBalance) is 1 && OpenerStep == 1)
                OpenerStep++;
            else if (OpenerStep == 1) actionID = PerfectBalance;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 2) OpenerStep++;
            else if (OpenerStep == 2) actionID = LeapingOpo;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 3) OpenerStep++;
            else if (OpenerStep == 3) actionID = DragonKick;

            if (WasLastAbility(Brotherhood) && OpenerStep == 4) OpenerStep++;
            else if (OpenerStep == 4) actionID = Brotherhood;

            if (WasLastAction(RiddleOfFire) && OpenerStep == 5) OpenerStep++;
            else if (OpenerStep == 5 && CanDelayedWeave(ActionWatching.LastWeaponskill)) actionID = RiddleOfFire;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 6) OpenerStep++;
            else if (OpenerStep == 6) actionID = LeapingOpo;

            if (WasLastAction(TheForbiddenChakra) && OpenerStep == 7) OpenerStep++;
            else if (OpenerStep == 7) actionID = TheForbiddenChakra;

            if (WasLastAction(RiddleOfWind) && OpenerStep == 8) OpenerStep++;
            else if (OpenerStep == 8 && CanDelayedWeave(ActionWatching.LastWeaponskill)) actionID = RiddleOfWind;

            if (WasLastWeaponskill(ElixirBurst) && OpenerStep == 9) OpenerStep++;
            else if (OpenerStep == 9) actionID = ElixirBurst;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 10) OpenerStep++;
            else if (OpenerStep == 10) actionID = DragonKick;

            if (WasLastWeaponskill(WindsReply) && OpenerStep == 11) OpenerStep++;
            else if (OpenerStep == 11) actionID = WindsReply;

            if (WasLastWeaponskill(FiresReply) && OpenerStep == 12) OpenerStep++;
            else if (OpenerStep == 12) actionID = FiresReply;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 13) OpenerStep++;
            else if (OpenerStep == 13) actionID = LeapingOpo;

            if (WasLastAction(PerfectBalance) && GetRemainingCharges(PerfectBalance) is 0 && OpenerStep == 14)
                OpenerStep++;
            else if (OpenerStep == 14) actionID = PerfectBalance;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 15) OpenerStep++;
            else if (OpenerStep == 15) actionID = DragonKick;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 16) OpenerStep++;
            else if (OpenerStep == 16) actionID = LeapingOpo;

            if (WasLastWeaponskill(DragonKick) && OpenerStep == 17) OpenerStep++;
            else if (OpenerStep == 17) actionID = DragonKick;

            if (WasLastWeaponskill(ElixirBurst) && OpenerStep == 18) OpenerStep++;
            else if (OpenerStep == 18) actionID = ElixirBurst;

            if (WasLastWeaponskill(LeapingOpo) && OpenerStep == 19) CurrentState = OpenerState.OpenerFinished;
            else if (OpenerStep == 19) actionID = LeapingOpo;

            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                CurrentState = OpenerState.FailedOpener;

            if (((actionID == PerfectBalance && GetRemainingCharges(PerfectBalance) == 0) ||
                 (actionID is RiddleOfFire && IsOnCooldown(RiddleOfFire)) ||
                 (actionID is RiddleOfWind && IsOnCooldown(RiddleOfWind)) ||
                 (actionID is Brotherhood && IsOnCooldown(Brotherhood)))
                && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

    public bool DoFullOpener(ref uint actionID, int selectedOpener)
    {
        if (!LevelChecked)
            return false;

        if (CurrentState == OpenerState.PrePull)
            if (DoPrePullSteps(ref actionID))
                return true;

        if (CurrentState == OpenerState.InOpener)
            switch (selectedOpener)
            {
                case 0 when DoLlOpener(ref actionID):

                case 1 when DoSlOpener(ref actionID):
                    return true;
            }

        if (!InCombat())
        {
            ResetOpener();
            CurrentState = OpenerState.PrePull;
        }

        return false;
    }
}