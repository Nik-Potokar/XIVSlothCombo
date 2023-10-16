using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class DRGOpenerLogic : PvE.DRG
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SpineshatterDive) < 2)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(LifeSurge) < 2)
                return false;
            if (!CustomComboFunctions.ActionReady(BattleLitany))
                return false;
            if (!CustomComboFunctions.ActionReady(DragonSight))
                return false;
            if (!CustomComboFunctions.ActionReady(DragonfireDive))
                return false;
            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 1;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private bool CanOpener => HasCooldowns() && LevelChecked;

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
                            Svc.Log.Information("Opener Failed");

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

            if (PrePullStep < 1)
                return false;

            if (CurrentState == OpenerState.PrePull)
            {
                if (PrePullStep == 1 && CustomComboFunctions.WasLastAction(All.Sprint))
                    CurrentState = OpenerState.InOpener;

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
                if (Config.DRG_OpenerChoice == 0)
                {
                    if (CustomComboFunctions.WasLastAction(TrueThrust) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = TrueThrust;

                    if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Disembowel;

                    if (CustomComboFunctions.WasLastAction(LanceCharge) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = LanceCharge;

                    if (CustomComboFunctions.WasLastAction(DragonSight) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = DragonSight;

                    if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = ChaoticSpring;

                    if (CustomComboFunctions.WasLastAction(BattleLitany) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = BattleLitany;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = WheelingThrust;

                    if (CustomComboFunctions.WasLastAction(Geirskogul) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Geirskogul;

                    if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = LifeSurge;

                    if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = FangAndClaw;

                    if (CustomComboFunctions.WasLastAction(HighJump) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = HighJump;

                    if (CustomComboFunctions.WasLastAction(MirageDive) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = MirageDive;

                    if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = RaidenThrust;

                    if (CustomComboFunctions.WasLastAction(DragonfireDive) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = DragonfireDive;

                    if (CustomComboFunctions.WasLastAction(VorpalThrust) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = VorpalThrust;

                    if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = SpineshatterDive;

                    if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = LifeSurge;

                    if (CustomComboFunctions.WasLastAction(HeavensThrust) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = HeavensThrust;

                    if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = SpineshatterDive;

                    if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = FangAndClaw;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = WheelingThrust;

                    if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = RaidenThrust;

                    if (CustomComboFunctions.WasLastAction(WyrmwindThrust) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = WyrmwindThrust;

                    if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Disembowel;

                    if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = ChaoticSpring;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 26) actionID = WheelingThrust;
                }

                else
                {
                    if (CustomComboFunctions.WasLastAction(TrueThrust) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = TrueThrust;

                    if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = Disembowel;

                    if (CustomComboFunctions.WasLastAction(LanceCharge) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = LanceCharge;

                    if (CustomComboFunctions.WasLastAction(DragonSight) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = DragonSight;

                    if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = ChaoticSpring;

                    if (CustomComboFunctions.WasLastAction(BattleLitany) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = BattleLitany;

                    if (CustomComboFunctions.WasLastAction(Geirskogul) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Geirskogul;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = WheelingThrust;

                    if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = SpineshatterDive;

                    if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = LifeSurge;

                    if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = FangAndClaw;

                    if (CustomComboFunctions.WasLastAction(HighJump) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = HighJump;

                    if (CustomComboFunctions.WasLastAction(MirageDive) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = MirageDive;

                    if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = RaidenThrust;

                    if (CustomComboFunctions.WasLastAction(DragonfireDive) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = DragonfireDive;

                    if (CustomComboFunctions.WasLastAction(VorpalThrust) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = VorpalThrust;

                    if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = SpineshatterDive;

                    if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = LifeSurge;

                    if (CustomComboFunctions.WasLastAction(HeavensThrust) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = HeavensThrust;

                    if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = FangAndClaw;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = WheelingThrust;

                    if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = RaidenThrust;

                    if (CustomComboFunctions.WasLastAction(WyrmwindThrust) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = WyrmwindThrust;

                    if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Disembowel;

                    if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = ChaoticSpring;

                    if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 26) actionID = WheelingThrust;

                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private bool DoOpenerSimple(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(TrueThrust) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = TrueThrust;

                if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Disembowel;

                if (CustomComboFunctions.WasLastAction(LanceCharge) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = LanceCharge;

                if (CustomComboFunctions.WasLastAction(DragonSight) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = DragonSight;

                if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = ChaoticSpring;

                if (CustomComboFunctions.WasLastAction(BattleLitany) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = BattleLitany;

                if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = WheelingThrust;

                if (CustomComboFunctions.WasLastAction(Geirskogul) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = Geirskogul;

                if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = LifeSurge;

                if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = FangAndClaw;

                if (CustomComboFunctions.WasLastAction(HighJump) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = HighJump;

                if (CustomComboFunctions.WasLastAction(MirageDive) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = MirageDive;

                if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = RaidenThrust;

                if (CustomComboFunctions.WasLastAction(DragonfireDive) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = DragonfireDive;

                if (CustomComboFunctions.WasLastAction(VorpalThrust) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = VorpalThrust;

                if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = SpineshatterDive;

                if (CustomComboFunctions.WasLastAction(LifeSurge) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = LifeSurge;

                if (CustomComboFunctions.WasLastAction(HeavensThrust) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = HeavensThrust;

                if (CustomComboFunctions.WasLastAction(SpineshatterDive) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = SpineshatterDive;

                if (CustomComboFunctions.WasLastAction(FangAndClaw) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = FangAndClaw;

                if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = WheelingThrust;

                if (CustomComboFunctions.WasLastAction(RaidenThrust) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = RaidenThrust;

                if (CustomComboFunctions.WasLastAction(WyrmwindThrust) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = WyrmwindThrust;

                if (CustomComboFunctions.WasLastAction(Disembowel) && OpenerStep == 24) OpenerStep++;
                else if (OpenerStep == 24) actionID = Disembowel;

                if (CustomComboFunctions.WasLastAction(ChaoticSpring) && OpenerStep == 25) OpenerStep++;
                else if (OpenerStep == 25) actionID = ChaoticSpring;

                if (CustomComboFunctions.WasLastAction(WheelingThrust) && OpenerStep == 26) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 26) actionID = WheelingThrust;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
            CurrentState = OpenerState.PrePull;
            ActionWatching.CombatActions.Clear();
            ActionWatching.LastAction = 0;
            ActionWatching.LastAbility = 0;
            ActionWatching.LastSpell = 0;
            ActionWatching.LastWeaponskill = 0;
        }
        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked) return false;

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

            if (!CustomComboFunctions.InCombat() && CurrentState is not OpenerState.PrePull)
                ResetOpener();

            return false;
        }
    }

    internal class AnimationLock
    {
        internal static readonly List<uint> FastLocks = new()
        {
            PvE.DRG.BattleLitany,
            PvE.DRG.LanceCharge,
            PvE.DRG.DragonSight,
            PvE.DRG.LifeSurge,
            PvE.DRG.Geirskogul,
            PvE.DRG.Nastrond,
            PvE.DRG.MirageDive,
            PvE.DRG.WyrmwindThrust,
            PvE.All.TrueNorth
        };

        internal static readonly List<uint> MidLocks = new()
        {
            PvE.DRG.Jump,
            PvE.DRG.HighJump,
            PvE.DRG.DragonfireDive,
            PvE.DRG.SpineshatterDive
        };

        internal static uint SlowLock => PvE.DRG.Stardiver;

        internal static bool CanDRGWeave(uint oGCD)
        {
            //GCD Ready - No Weave
            if (CustomComboFunctions.IsOffCooldown(PvE.DRG.TrueThrust))
                return false;

            var gcdTimer = CustomComboFunctions.GetCooldownRemainingTime(PvE.DRG.TrueThrust);

            if (FastLocks.Any(x => x == oGCD) && gcdTimer >= 1.1f)
                return true;

            if (MidLocks.Any(x => x == oGCD) && gcdTimer >= 1.4f)
                return true;

            if (SlowLock == oGCD && gcdTimer >= 2.2f)
                return true;

            return false;
        }
    }
}
