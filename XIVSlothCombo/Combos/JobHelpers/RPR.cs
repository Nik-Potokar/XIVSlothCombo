using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.PvE.RPR;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal class RPR
{
    // RPR Gauge & Extensions
    public static float GCD = GetCooldown(Slice).CooldownTotal;

    public static RPROpenerLogic RPROpener = new();
    public static RPRGauge Gauge = GetJobGauge<RPRGauge>();

    public static bool trueNorthReady => TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
                                         !HasEffect(All.Buffs.TrueNorth);

    internal class RPROpenerLogic
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
            if (GetRemainingCharges(SoulSlice) < 2)
                return false;

            if (!ActionReady(ArcaneCircle))
                return false;

            if (!ActionReady(Gluttony))
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
                if (WasLastAction(ShadowOfDeath) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = ShadowOfDeath;

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
                if (WasLastAction(SoulSlice) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = SoulSlice;

                if (WasLastAction(ArcaneCircle) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = ArcaneCircle;

                if (WasLastAction(Gluttony) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Gluttony;

                if (WasLastAction(ExecutionersGibbet) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = ExecutionersGibbet;

                if (WasLastAction(ExecutionersGallows) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = ExecutionersGallows;

                if (WasLastAction(PlentifulHarvest) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = PlentifulHarvest;

                if (WasLastAction(Enshroud) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Enshroud;

                if (WasLastAction(VoidReaping) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = VoidReaping;

                if (WasLastAction(Sacrificium) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Sacrificium;

                if (WasLastAction(CrossReaping) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = CrossReaping;

                if (WasLastAction(LemuresSlice) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = LemuresSlice;

                if (WasLastAction(VoidReaping) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = VoidReaping;

                if (WasLastAction(CrossReaping) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = CrossReaping;

                if (WasLastAction(LemuresSlice) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = LemuresSlice;

                if (WasLastAction(Communio) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Communio;

                if (WasLastAction(Perfectio) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Perfectio;

                if (WasLastAction(SoulSlice) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = SoulSlice;

                if (WasLastAction(UnveiledGibbet) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = UnveiledGibbet;

                if (WasLastAction(Gibbet) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Gibbet;

                if (WasLastAction(ShadowOfDeath) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = ShadowOfDeath;

                if (WasLastAction(Slice) && OpenerStep == 21) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 21) actionID = Slice;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == SoulSlice && GetRemainingCharges(SoulSlice) == 0) ||
                     (actionID == ArcaneCircle && IsOnCooldown(ArcaneCircle)) ||
                     (actionID == Gluttony && IsOnCooldown(Gluttony))) &&
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

    internal class RPRHelper
    {
        public static unsafe bool IsComboExpiring(float Times)
        {
            float GCD = GetCooldown(Slice).CooldownTotal * Times;

            return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD;
        }

        public static bool IsBuffExpiring(float Times)
        {
            float GCD = GetCooldown(Slice).CooldownTotal * Times;

            return (HasEffect(Buffs.EnhancedGallows) && GetBuffRemainingTime(Buffs.EnhancedGallows) < GCD) ||
                   (HasEffect(Buffs.EnhancedGibbet) && GetBuffRemainingTime(Buffs.EnhancedGibbet) < GCD);
        }

        public static bool IsDebuffExpiring(float Times)
        {
            float GCD = GetCooldown(Slice).CooldownTotal * Times;

            return TargetHasEffect(Debuffs.DeathsDesign) && GetDebuffRemainingTime(Debuffs.DeathsDesign) < GCD;
        }

        public static bool UseEnshroud(RPRGauge gauge)
        {
            if (LevelChecked(Enshroud) && (gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) &&
                !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) &&
                !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.Enshrouded))
            {
                // Before Plentiful Harvest 
                if (!LevelChecked(PlentifulHarvest))
                    return true;

                // Shroud in Arcane Circle 
                if (HasEffect(Buffs.ArcaneCircle))
                    return true;

                // Prep for double Enshroud
                if (LevelChecked(PlentifulHarvest) &&
                    GetCooldownRemainingTime(ArcaneCircle) <= GCD * 2 + 1.5)
                    return true;

                //2nd part of Double Enshroud
                if (LevelChecked(PlentifulHarvest) &&
                    JustUsed(PlentifulHarvest, 5))
                    return true;

                //Natural Odd Minute Shrouds
                if (!HasEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                    GetCooldownRemainingTime(ArcaneCircle) is >= 50 and <= 65)
                    return true;

                // Correction for 2 min windows 
                if (!HasEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                    gauge.Soul >= 90)
                    return true;
            }

            return false;
        }

        public static bool UseShadowOfDeath()
        {
            if (LevelChecked(ShadowOfDeath) && !HasEffect(Buffs.SoulReaver) &&
                !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) &&
                !HasEffect(Buffs.ImmortalSacrifice) && !IsComboExpiring(3) &&
                !JustUsed(ShadowOfDeath))
            {
                //1st part double enshroud
                if (LevelChecked(PlentifulHarvest) && HasEffect(Buffs.Enshrouded) &&
                    GetCooldownRemainingTime(ArcaneCircle) <= GCD * 2 + 1.5 && JustUsed(Enshroud))
                    return true;

                //2nd part double enshroud
                if (LevelChecked(PlentifulHarvest) && HasEffect(Buffs.Enshrouded) &&
                    (GetCooldownRemainingTime(ArcaneCircle) <= GCD || IsOffCooldown(ArcaneCircle)) &&
                    (JustUsed(VoidReaping) || JustUsed(CrossReaping)))
                    return true;

                //lvl 88+ general use
                if (LevelChecked(PlentifulHarvest) && !HasEffect(Buffs.Enshrouded) &&
                    ((IsEnabled(CustomComboPreset.RPR_ST_SimpleMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= 8) ||
                     (IsEnabled(CustomComboPreset.RPR_ST_AdvancedMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= Config.RPR_SoDRefreshRange)) &&
                    (GetCooldownRemainingTime(ArcaneCircle) > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                    return true;

                //below lvl 88 use
                if (!LevelChecked(PlentifulHarvest) &&
                    ((IsEnabled(CustomComboPreset.RPR_ST_SimpleMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= 8) ||
                     (IsEnabled(CustomComboPreset.RPR_ST_AdvancedMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= Config.RPR_SoDRefreshRange)))
                    return true;
            }

            return false;
        }
    }
}