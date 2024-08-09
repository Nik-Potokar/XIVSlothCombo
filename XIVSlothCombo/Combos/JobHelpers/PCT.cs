using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class PCTOpenerLogic100 : PCT
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(MogoftheAges))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 3)
                return false;

            return true;
        }

        private static uint OpenerLevel => 100;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

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
            if (!LevelChecked)
                return false;

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
                if (CustomComboFunctions.WasLastAction(RainbowDrip) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
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

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = StoneinYellow;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = CometinBlack;

                if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = StarPrism;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 17) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 17) actionID = RainbowDrip;

                if (CustomComboFunctions.WasLastAction(All.Sleep))
                    CurrentState = OpenerState.FailedOpener;

                if (actionID == RainbowDrip && CustomComboFunctions.IsOnCooldown(ScenicMuse))
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

    internal class PCTOpenerLogic92 : PCT
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(MogoftheAges))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static uint OpenerLevel => 92;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

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
            if (!LevelChecked)
                return false;

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
                if (CustomComboFunctions.WasLastAction(RainbowDrip) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
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

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = BlizzardinCyan;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = StoneinYellow;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = ThunderinMagenta;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = CometinBlack;

                if (CustomComboFunctions.WasLastAction(FireInRed) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = FireInRed;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 17) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 17) actionID = RainbowDrip;

                if (CustomComboFunctions.WasLastAction(All.Sleep))
                    CurrentState = OpenerState.FailedOpener;

                if (actionID == RainbowDrip && CustomComboFunctions.IsOnCooldown(ScenicMuse))
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

    internal class PCTOpenerLogic90 : PCT
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(MogoftheAges))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

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
            if (!LevelChecked)
                return false;

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
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
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

                if (actionID == StoneinYellow && CustomComboFunctions.IsOnCooldown(ScenicMuse))
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                if (CustomComboFunctions.WasLastAction(All.Swiftcast))
                    CurrentState = OpenerState.FailedOpener;

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

    internal class PCTOpenerLogic70 : PCT
    {
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(SteelMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(MogoftheAges))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static uint OpenerLevel => 70;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

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
            if (!LevelChecked)
                return false;

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
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
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

                if (actionID == StoneinYellow && CustomComboFunctions.IsOnCooldown(ScenicMuse))
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                if (CustomComboFunctions.WasLastAction(All.Swiftcast))
                    CurrentState = OpenerState.FailedOpener;

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