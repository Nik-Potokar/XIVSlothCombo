using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class DRGOpenerLogic : DRG
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(LifeSurge) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(BattleLitany))
                return false;

            if (!CustomComboFunctions.ActionReady(DragonfireDive))
                return false;

            if (!CustomComboFunctions.ActionReady(LanceCharge))
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
                if (CustomComboFunctions.WasLastAction(TrueThrust) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = TrueThrust;

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
                if (CustomComboFunctions.WasLastAction(SpiralBlow) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = SpiralBlow;

                if (CustomComboFunctions.WasLastAction(LanceCharge) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = LanceCharge;

                if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = ChaoticSpring;

                if (CustomComboFunctions.WasLastAction(BattleLitany) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = BattleLitany;

                if (CustomComboFunctions.WasLastAction(Geirskogul) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Geirskogul;

                if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = WheelingThrust;

                if (CustomComboFunctions.WasLastAction(HighJump) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = HighJump;

                if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = LifeSurge;

                if (CustomComboFunctions.WasLastAction(Drakesbane) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Drakesbane;

                if (CustomComboFunctions.WasLastAction(DragonfireDive) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = DragonfireDive;

                if (CustomComboFunctions.WasLastAction(Nastrond) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Nastrond;

                if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = RaidenThrust;

                if (CustomComboFunctions.WasLastAction(Stardiver) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Stardiver;

                if (CustomComboFunctions.WasLastAction(LanceBarrage) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = LanceBarrage;

                if (CustomComboFunctions.WasLastAction(Starcross) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Starcross;

                if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = LifeSurge;

                if (CustomComboFunctions.WasLastAction(HeavensThrust) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = HeavensThrust;

                if (CustomComboFunctions.WasLastAction(Nastrond) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Nastrond;

                if (CustomComboFunctions.WasLastAction(RiseOfTheDragon) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = RiseOfTheDragon;

                if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = FangAndClaw;

                if (CustomComboFunctions.WasLastAction(Nastrond) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Nastrond;

                if (CustomComboFunctions.WasLastAction(MirageDive) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = MirageDive;

                if (CustomComboFunctions.WasLastAction(Drakesbane) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = Drakesbane;

                if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 24) OpenerStep++;
                else if (OpenerStep == 24) actionID = RaidenThrust;

                if (CustomComboFunctions.WasLastAction(WyrmwindThrust) && OpenerStep == 25) OpenerStep++;
                else if (OpenerStep == 25) actionID = WyrmwindThrust;

                if (CustomComboFunctions.WasLastAction(SpiralBlow) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 26) actionID = SpiralBlow;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == DragonfireDive && CustomComboFunctions.IsOnCooldown(DragonfireDive)) ||
                    (actionID == BattleLitany && CustomComboFunctions.IsOnCooldown(BattleLitany)) ||
                    (actionID == LanceCharge && CustomComboFunctions.IsOnCooldown(LanceCharge)) ||
                    (actionID == LifeSurge && CustomComboFunctions.GetRemainingCharges(LifeSurge) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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
    internal class AnimationLock
    {
        internal static readonly List<uint> FastLocks =
        [
            DRG.BattleLitany,
            DRG.LanceCharge,
            DRG.LifeSurge,
            DRG.Geirskogul,
            DRG.Nastrond,
            DRG.MirageDive,
            DRG.WyrmwindThrust,
            DRG.RiseOfTheDragon,
            DRG.Starcross,
            PvE.Content.Variant.VariantRampart,
            PvE.All.TrueNorth
        ];

        internal static readonly List<uint> MidLocks =
        [
            DRG.Jump,
            DRG.HighJump,
            DRG.DragonfireDive,
        ];

        internal static uint SlowLock => DRG.Stardiver;

        internal static bool CanDRGWeave(uint oGCD)
        {
            //GCD Ready - No Weave
            if (CustomComboFunctions.IsOffCooldown(DRG.TrueThrust))
                return false;

            var gcdTimer = CustomComboFunctions.GetCooldownRemainingTime(DRG.TrueThrust);

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