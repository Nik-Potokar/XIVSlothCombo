using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.PvE.VPR;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class VPRHelpers
    {
        internal class VPROpenerLogic
        {
            private static bool HasCooldowns()
            {
                if (GetRemainingCharges(Vicewinder) < 2)
                    return false;

                if (!ActionReady(SerpentsIre))
                    return false;

                return true;
            }

            private static uint OpenerLevel => 100;

            public uint PrePullStep = 0;

            public uint OpenerStep = 0;

            public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

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
                    if (WasLastAction(ReavingFangs) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = ReavingFangs;

                    if (ActionWatching.CombatActions.Count > 2 && InCombat())
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
                    if (WasLastAction(SerpentsIre) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = SerpentsIre;

                    if (WasLastAction(SwiftskinsSting) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = SwiftskinsSting;

                    if (WasLastAction(Vicewinder) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Vicewinder;

                    if (WasLastAction(HuntersCoil) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = HuntersCoil;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = TwinfangBite;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = TwinbloodBite;

                    if (WasLastAction(SwiftskinsCoil) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = SwiftskinsCoil;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = TwinbloodBite;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = TwinfangBite;

                    if (WasLastAction(Reawaken) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = Reawaken;

                    if (WasLastAction(FirstGeneration) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = FirstGeneration;

                    if (WasLastAction(FirstLegacy) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = FirstLegacy;

                    if (WasLastAction(SecondGeneration) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = SecondGeneration;

                    if (WasLastAction(SecondLegacy) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = SecondLegacy;

                    if (WasLastAction(ThirdGeneration) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = ThirdGeneration;

                    if (WasLastAction(ThirdLegacy) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = ThirdLegacy;

                    if (WasLastAction(FourthGeneration) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = FourthGeneration;

                    if (WasLastAction(FourthLegacy) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = FourthLegacy;

                    if (WasLastAction(Ouroboros) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Ouroboros;

                    if (WasLastAction(UncoiledFury) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = UncoiledFury;

                    if (WasLastAction(UncoiledTwinfang) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = UncoiledTwinfang;

                    if (WasLastAction(UncoiledTwinblood) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = UncoiledTwinblood;

                    if (WasLastAction(UncoiledFury) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = UncoiledFury;

                    if (WasLastAction(UncoiledTwinfang) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = UncoiledTwinfang;

                    if (WasLastAction(UncoiledTwinblood) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = UncoiledTwinblood;

                    if (WasLastAction(HindstingStrike) && OpenerStep == 26) OpenerStep++;
                    else if (OpenerStep == 26) actionID = HindstingStrike;

                    if (WasLastAction(DeathRattle) && OpenerStep == 27) OpenerStep++;
                    else if (OpenerStep == 27) actionID = DeathRattle;

                    if (WasLastAction(Vicewinder) && OpenerStep == 28) OpenerStep++;
                    else if (OpenerStep == 28) actionID = Vicewinder;

                    if (WasLastAction(UncoiledFury) && OpenerStep == 29) OpenerStep++;
                    else if (OpenerStep == 29) actionID = UncoiledFury;

                    if (WasLastAction(UncoiledTwinfang) && OpenerStep == 30) OpenerStep++;
                    else if (OpenerStep == 30) actionID = UncoiledTwinfang;

                    if (WasLastAction(UncoiledTwinblood) && OpenerStep == 31) OpenerStep++;
                    else if (OpenerStep == 31) actionID = UncoiledTwinblood;

                    if (WasLastAction(HuntersCoil) && OpenerStep == 32) OpenerStep++;
                    else if (OpenerStep == 32) actionID = HuntersCoil;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 33) OpenerStep++;
                    else if (OpenerStep == 33) actionID = TwinfangBite;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 34) OpenerStep++;
                    else if (OpenerStep == 34) actionID = TwinbloodBite;

                    if (WasLastAction(SwiftskinsCoil) && OpenerStep == 35) OpenerStep++;
                    else if (OpenerStep == 35) actionID = SwiftskinsCoil;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 36) OpenerStep++;
                    else if (OpenerStep == 36) actionID = TwinbloodBite;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 37) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 37) actionID = TwinfangBite;

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                        CurrentState = OpenerState.FailedOpener;

                    if (((actionID == SerpentsIre && IsOnCooldown(SerpentsIre)) ||
                        (actionID == Vicewinder && GetRemainingCharges(Vicewinder) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

                if (!InCombat())
                {
                    ResetOpener();
                    CurrentState = OpenerState.PrePull;
                }
                return false;
            }
        }

        internal class VPROpenerLogicNoRattling
        {
            private static bool HasCooldowns()
            {
                if (GetRemainingCharges(Vicewinder) < 2)
                    return false;

                if (!ActionReady(SerpentsIre))
                    return false;

                return true;
            }

            private static uint OpenerLevel => 100;

            public uint PrePullStep = 0;

            public uint OpenerStep = 0;

            public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

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
                    if (WasLastAction(ReavingFangs) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = ReavingFangs;

                    if (ActionWatching.CombatActions.Count > 2 && InCombat())
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
                    if (WasLastAction(SerpentsIre) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = SerpentsIre;

                    if (WasLastAction(SwiftskinsSting) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = SwiftskinsSting;

                    if (WasLastAction(Vicewinder) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Vicewinder;

                    if (WasLastAction(HuntersCoil) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = HuntersCoil;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = TwinfangBite;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = TwinbloodBite;

                    if (WasLastAction(SwiftskinsCoil) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = SwiftskinsCoil;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = TwinbloodBite;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = TwinfangBite;

                    if (WasLastAction(Reawaken) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = Reawaken;

                    if (WasLastAction(FirstGeneration) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = FirstGeneration;

                    if (WasLastAction(FirstLegacy) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = FirstLegacy;

                    if (WasLastAction(SecondGeneration) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = SecondGeneration;

                    if (WasLastAction(SecondLegacy) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = SecondLegacy;

                    if (WasLastAction(ThirdGeneration) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = ThirdGeneration;

                    if (WasLastAction(ThirdLegacy) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = ThirdLegacy;

                    if (WasLastAction(FourthGeneration) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = FourthGeneration;

                    if (WasLastAction(FourthLegacy) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = FourthLegacy;

                    if (WasLastAction(Ouroboros) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Ouroboros;

                    if (WasLastAction(HindstingStrike) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = HindstingStrike;

                    if (WasLastAction(DeathRattle) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = DeathRattle;

                    if (WasLastAction(Vicewinder) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = Vicewinder;

                    if (WasLastAction(HuntersCoil) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = HuntersCoil;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = TwinfangBite;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = TwinbloodBite;

                    if (WasLastAction(SwiftskinsCoil) && OpenerStep == 26) OpenerStep++;
                    else if (OpenerStep == 26) actionID = SwiftskinsCoil;

                    if (WasLastAction(TwinbloodBite) && OpenerStep == 27) OpenerStep++;
                    else if (OpenerStep == 27) actionID = TwinbloodBite;

                    if (WasLastAction(TwinfangBite) && OpenerStep == 28) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 28) actionID = TwinfangBite;

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                        CurrentState = OpenerState.FailedOpener;

                    if (((actionID == SerpentsIre && IsOnCooldown(SerpentsIre)) ||
                        (actionID == Vicewinder && GetRemainingCharges(Vicewinder) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

                if (!InCombat())
                {
                    ResetOpener();
                    CurrentState = OpenerState.PrePull;
                }
                return false;
            }
        }

        internal class VPRCheckRattlingCoils
        {
            public static bool HasRattlingCoilStack(VPRGauge gauge)
            {
                if (gauge.RattlingCoilStacks > 0)
                    return true;

                else return false;
            }
        }

        internal class VPRCheckTimers
        {
            public static bool IsHoningExpiring(float Times)
            {
                float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

                if ((HasEffect(Buffs.HonedSteel) && GetBuffRemainingTime(Buffs.HonedSteel) < GCD) ||
                    (HasEffect(Buffs.HonedReavers) && GetBuffRemainingTime(Buffs.HonedReavers) < GCD))
                    return true;

                else return false;
            }

            public static bool IsVenomExpiring(float Times)
            {
                float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

                if ((HasEffect(Buffs.FlankstungVenom) && GetBuffRemainingTime(Buffs.FlankstungVenom) < GCD) ||
                    (HasEffect(Buffs.FlanksbaneVenom) && GetBuffRemainingTime(Buffs.FlanksbaneVenom) < GCD) ||
                    (HasEffect(Buffs.HindstungVenom) && GetBuffRemainingTime(Buffs.HindstungVenom) < GCD) ||
                    (HasEffect(Buffs.HindsbaneVenom) && GetBuffRemainingTime(Buffs.HindsbaneVenom) < GCD))
                    return true;

                else return false;
            }

            public static bool IsEmpowermentExpiring(float Times)
            {
                float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

                if (GetBuffRemainingTime(Buffs.Swiftscaled) < GCD || GetBuffRemainingTime(Buffs.HuntersInstinct) < GCD)
                    return true;

                else return false;
            }

            public unsafe static bool IsComboExpiring(float Times)
            {
                float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

                if (ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD)
                    return true;

                else return false;
            }
        }
    }
}