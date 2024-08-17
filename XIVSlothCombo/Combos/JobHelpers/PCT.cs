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
    #region Lvl 100 Opener
    internal class PCTOpenerLogicLvl100 : PCT
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

                if (!HasMotifs())
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


                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > 17) // Assuming 17 is the last step
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
    #endregion

    #region Lvl 92 Opener 
    internal class PCTOpenerLogicLvl92 : PCT
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
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

        private static uint OpenerLevel => 92;

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

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.LocalPlayer.CastActionId == RainbowDrip && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                if (!HasMotifs())
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

                if (CustomComboFunctions.WasLastAction(FireInRed) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = FireInRed;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = HammerBrush;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = PolishingHammer;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = RainbowDrip;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }


                if (OpenerStep > 17) // Assuming 15 is the last step
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
    #endregion

    #region Lvl 90 Opener
    internal class PCTOpenerLogicLvl90 : PCT
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
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

        private static uint OpenerLevel => 90;

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

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
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
                if (!CustomComboFunctions.InCombat())
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to not being in combat.");
                    return false;
                }

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = AeroInGreen;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = PomMuse;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = WingMotif;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = StarryMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = WingedMuse;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = HammerBrush;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = MogoftheAges;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = PolishingHammer;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = SubtractivePalette;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = CometinBlack;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 15) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 15) actionID = StoneinYellow;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }


                if (OpenerStep > 15) // Assuming 15 is the last step
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
    #endregion

    #region Lvl 80 Opener
    internal class PCTOpenerLogicLvl80 : PCT
    {
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(SteelMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(LivingMuse))
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

        private static uint OpenerLevel => 80;

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

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
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

            if (currentState == OpenerState.InOpener && CustomComboFunctions.InCombat())
            {
                if (!CustomComboFunctions.InCombat())
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to not being in combat.");
                    return false;
                }

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = AeroInGreen;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = PomMuse;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = WingMotif;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = StarryMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = WingedMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = MogoftheAges;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = SubtractivePalette;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 14) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 14) actionID = StoneinYellow;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > 14) // Assuming 15 is the last step
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
    #endregion

    #region Lvl 70 Opener
    internal class PCTOpenerLogicLvl70 : PCT
    {
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(SteelMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
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

        private static uint OpenerLevel => 70;

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

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
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

            if (currentState == OpenerState.InOpener && CustomComboFunctions.InCombat())
            {
                if (!CustomComboFunctions.InCombat())
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to not being in combat.");
                    return false;
                }

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = AeroInGreen;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = PomMuse;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = WingMotif;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = StarryMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = WingedMuse;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = MogoftheAges;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = HammerStamp;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = SubtractivePalette;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 14) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 14) actionID = StoneinYellow;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > 14) // Assuming 14 is the last step
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
    #endregion
}
