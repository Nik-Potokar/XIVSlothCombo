using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Data;
using static XIVSlothCombo.Combos.PvE.SGE;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal class SGE
{
    public static SGEGauge Gauge = GetJobGauge<SGEGauge>();

    public static int Dosis3Count => ActionWatching.CombatActions.Count(x => x == Dosis3);

    public static int Toxikon2Count => ActionWatching.CombatActions.Count(x => x == Toxikon2);

    public static bool HasAddersgall(SGEGauge gauge)
    {
        return gauge.Addersgall > 0;
    }

    public static bool HasAddersting(SGEGauge gauge)
    {
        return gauge.Addersting > 0;
    }

    internal class SGEOpenerLogic
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep = 1;

        public uint PrePullStep;

        private static uint OpenerLevel => 92;

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
            if (GetRemainingCharges(Phlegma3) < 2)
                return false;

            if (!ActionReady(Psyche))
                return false;

            if (!HasAddersting(Gauge))
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
                if (WasLastAction(Eukrasia) && HasEffect(Buffs.Eukrasia) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = Eukrasia;

                if (WasLastAction(Toxikon2) && HasEffect(Buffs.Eukrasia) && PrePullStep == 2)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = Toxikon2;

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
                if (WasLastAction(EukrasianDosis3) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = EukrasianDosis3;

                if (WasLastAction(Dosis3) && Dosis3Count == 1 && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Dosis3;

                if (WasLastAction(Dosis3) && Dosis3Count == 2 && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Dosis3;

                if (WasLastAction(Dosis3) && Dosis3Count == 3 && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Dosis3;

                if (WasLastAction(Phlegma3) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = Phlegma3;

                if (WasLastAction(Psyche) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Psyche;

                if (WasLastAction(Phlegma3) && OpenerStep == 7) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 7) actionID = Phlegma3;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Phlegma3 && GetRemainingCharges(Phlegma3) == 0) ||
                     (actionID == Psyche && IsOnCooldown(Psyche))) &&
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

    internal class SGEHelper
    {
        public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
        {
            IGameObject? healTarget = GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

            switch (i)
            {
                case 0:
                    action = Soteria;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria);

                    return Config.SGE_ST_Heal_Soteria;

                case 1:
                    action = Zoe;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe);

                    return Config.SGE_ST_Heal_Zoe;

                case 2:
                    action = Pepsis;

                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) &&
                              FindEffect(Buffs.EukrasianDiagnosis, healTarget,
                                  LocalPlayer?.GameObjectId) is not null;

                    return Config.SGE_ST_Heal_Pepsis;

                case 3:
                    action = Taurochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && HasAddersgall(Gauge);

                    return Config.SGE_ST_Heal_Taurochole;

                case 4:
                    action = Haima;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima);

                    return Config.SGE_ST_Heal_Haima;

                case 5:
                    action = Krasis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis);

                    return Config.SGE_ST_Heal_Krasis;

                case 6:
                    action = Druochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && HasAddersgall(Gauge);

                    return Config.SGE_ST_Heal_Druochole;
            }

            enabled = false;
            action = 0;

            return 0;
        }

        public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
        {
            switch (i)
            {
                case 0:
                    action = Kerachole;

                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) &&
                              (!Config.SGE_AoE_Heal_KeracholeTrait || (Config.SGE_AoE_Heal_KeracholeTrait &&
                                                                       TraitLevelChecked(
                                                                           Traits.EnhancedKerachole))) &&
                              HasAddersgall(Gauge);

                    return 0;

                case 1:
                    action = Ixochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && HasAddersgall(Gauge);

                    return 0;

                case 2:
                    action = OriginalHook(Physis);
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis);

                    return 0;

                case 3:
                    action = Holos;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos);

                    return 0;

                case 4:
                    action = Panhaima;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima);

                    return 0;

                case 5:
                    action = Pepsis;

                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) &&
                              FindEffect(Buffs.EukrasianPrognosis) is not null;

                    return 0;

                case 6:
                    action = Philosophia;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Philosophia);

                    return 0;
            }

            enabled = false;
            action = 0;

            return 0;
        }
    }
}