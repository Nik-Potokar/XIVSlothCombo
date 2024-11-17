using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.PvE.DRG;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal class DRG
{
    // DRG Gauge & Extensions
    public static DRGGauge Gauge = GetJobGauge<DRGGauge>();
    public static DRGOpenerLogic DRGOpener = new();

    public static Status? ChaosDoTDebuff => FindTargetEffect(LevelChecked(ChaoticSpring)
        ? Debuffs.ChaoticSpring
        : Debuffs.ChaosThrust);

    public static bool trueNorthReady => TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
                                         !HasEffect(All.Buffs.TrueNorth);

    internal class DRGOpenerLogic
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

                    if (value is OpenerState.OpenerFinished or OpenerState.FailedOpener)
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
            if (GetRemainingCharges(LifeSurge) < 2)
                return false;

            if (!ActionReady(BattleLitany))
                return false;

            if (!ActionReady(DragonfireDive))
                return false;

            if (!ActionReady(LanceCharge))
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
                if (WasLastAction(TrueThrust) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = TrueThrust;

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
                if (WasLastAction(SpiralBlow) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = SpiralBlow;

                if (WasLastAction(LanceCharge) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = LanceCharge;

                if (WasLastAction(ChaoticSpring) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = ChaoticSpring;

                if (WasLastAction(BattleLitany) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = BattleLitany;

                if (WasLastAction(Geirskogul) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Geirskogul;

                if (WasLastAction(WheelingThrust) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = WheelingThrust;

                if (WasLastAction(HighJump) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = HighJump;

                if (WasLastAction(LifeSurge) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = LifeSurge;

                if (WasLastAction(Drakesbane) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Drakesbane;

                if (WasLastAction(DragonfireDive) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = DragonfireDive;

                if (WasLastAction(Nastrond) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Nastrond;

                if (WasLastAction(RaidenThrust) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = RaidenThrust;

                if (WasLastAction(Stardiver) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Stardiver;

                if (WasLastAction(LanceBarrage) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = LanceBarrage;

                if (WasLastAction(Starcross) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Starcross;

                if (WasLastAction(LifeSurge) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = LifeSurge;

                if (WasLastAction(HeavensThrust) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = HeavensThrust;

                if (WasLastAction(RiseOfTheDragon) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = RiseOfTheDragon;

                if (WasLastAction(MirageDive) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = MirageDive;

                if (WasLastAction(FangAndClaw) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = FangAndClaw;

                if (WasLastAction(Drakesbane) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Drakesbane;

                if (WasLastAction(RaidenThrust) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = RaidenThrust;

                if (WasLastAction(WyrmwindThrust) && OpenerStep == 23) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 23) actionID = WyrmwindThrust;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == DragonfireDive && IsOnCooldown(DragonfireDive)) ||
                     (actionID == BattleLitany && IsOnCooldown(BattleLitany)) ||
                     (actionID == LanceCharge && IsOnCooldown(LanceCharge)) ||
                     (actionID == LifeSurge && GetRemainingCharges(LifeSurge) < 2)) &&
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
            if (!LevelChecked)
                return false;

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

    internal class AnimationLock
    {
        internal static readonly List<uint> FastLocks =
        [
            BattleLitany,
            LanceCharge,
            LifeSurge,
            Geirskogul,
            Nastrond,
            MirageDive,
            WyrmwindThrust,
            RiseOfTheDragon,
            Starcross,
            Variant.VariantRampart,
            All.TrueNorth
        ];

        internal static readonly List<uint> MidLocks =
        [
            Jump,
            HighJump,
            DragonfireDive
        ];

        internal static uint SlowLock => Stardiver;

        internal static bool CanDRGWeave(uint oGCD)
        {
            float gcdTimer = GetCooldownRemainingTime(TrueThrust);

            //GCD Ready - No Weave
            if (IsOffCooldown(TrueThrust))
                return false;

            if (FastLocks.Any(x => x == oGCD) && gcdTimer >= 0.6f)
                return true;

            if (MidLocks.Any(x => x == oGCD) && gcdTimer >= 0.8f)
                return true;

            if (SlowLock == oGCD && gcdTimer >= 1.5f)
                return true;

            return false;
        }
    }
}