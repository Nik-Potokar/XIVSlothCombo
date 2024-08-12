using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class PCTOpenerLogic : PCT
    {
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(StarryMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;
            return true;
        }
        private static bool HasMotifs()
        {
            var gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Pom))
                return false;
            if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Weapon))
                return false;
            if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Landscape))
                return false;
            return true;
        }

        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

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
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0)
            {
                PrePullStep = 1;
            }

            if (!HasCooldowns() && !HasMotifs())
            {
                PrePullStep = 0;
            }

            if (CurrentState == OpenerState.PrePull)
            {
                if (CustomComboFunctions.LocalPlayer.CastActionId == RainbowDrip && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                if(!HasMotifs())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }
            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = HolyInWhite;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = PomMuse;

                if (CustomComboFunctions.LocalPlayer.CastActionId == CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = StarryMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = SubtractivePalette;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = StoneinYellow;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = CometinBlack;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = WingedMuse;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = MogoftheAges;

                if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = StarPrism;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = HammerBrush;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = PolishingHammer;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = RainbowDrip;

                if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = HolyInWhite;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > 18) // Assuming 18 is the last step
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");
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

