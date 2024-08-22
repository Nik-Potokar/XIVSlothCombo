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
using XIVSlothCombo.Extensions;

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
            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
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

            bool isEarlyOpenerEnabled = CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                // If the early opener is not enabled, include HolyInWhite
                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = HolyInWhite;
                }

                // Adjust step numbers based on if HolyInWhite was skipped
                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.LocalPlayer.CastActionId == CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarPrism;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerBrush;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PolishingHammer;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = RainbowDrip;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > adjustedStep)
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
            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
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
                bool isEarlyOpenerEnabled = CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = HolyInWhite;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.LocalPlayer.CastActionId == CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(FireInRed) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = FireInRed;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerBrush;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PolishingHammer;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = RainbowDrip;

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > adjustedStep)
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
            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
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

                bool isEarlyOpenerEnabled = CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerBrush;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PolishingHammer;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }

                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > adjustedStep)
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
            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
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
                bool isEarlyOpenerEnabled = CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;
                if ((CustomComboFunctions.WasLastAction(HammerStamp) || CustomComboFunctions.WasLastAction(HammerBrush)) && OpenerStep == adjustedStep)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == adjustedStep)
                {
                    if (HammerBrush.LevelChecked())
                    {
                        actionID = HammerBrush;
                    }
                    else
                    {
                        actionID = HammerStamp;
                    }
                }

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;
                if ((CustomComboFunctions.WasLastAction(HammerStamp) || CustomComboFunctions.WasLastAction(PolishingHammer)) && OpenerStep == adjustedStep)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == adjustedStep)
                {
                    if (PolishingHammer.LevelChecked())
                    {
                        actionID = PolishingHammer;
                    }
                    else
                    {
                        actionID = HammerStamp;
                    }
                }

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }


                Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");
                    return false;
                }

                if (OpenerStep > (isEarlyOpenerEnabled ? 14 : 15))
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
            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
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

            if (currentState == OpenerState.InOpener)
            {
                bool isEarlyOpenerEnabled = CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep) OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }

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
}
