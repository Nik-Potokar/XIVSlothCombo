using ECommons.DalamudServices;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class SGEOpenerLogic : CustomComboFunctions
    {
        internal static int DosisCount => ActionWatching.CombatActions.Count(x => x == PvE.SGE.Dosis3);

        private static bool HasCooldowns()
        {
            if (GetRemainingCharges(PvE.SGE.Phlegma) < 2)
                return false;

            if (!ActionReady(PvE.SGE.Psyche))
                return false;

            if (!PvE.SGE.Gauge.HasAddersting())
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
                if (HasEffect(PvE.SGE.Buffs.Eukrasia) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = PvE.SGE.Eukrasia;

                if (HasEffect(PvE.SGE.Buffs.Eukrasia) && WasLastSpell(PvE.SGE.Toxikon2) && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = PvE.SGE.Toxikon2;

                if (PrePullStep == 2 && !HasEffect(PvE.SGE.Buffs.Eukrasia))
                    CurrentState = OpenerState.FailedOpener;

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
                if (WasLastSpell(PvE.SGE.EukrasianDosis3) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = PvE.SGE.EukrasianDosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 1 && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 2 && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = PvE.SGE.Dosis3;

                if (WasLastAction(PvE.SGE.Phlegma3) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = PvE.SGE.Phlegma3;

                if (WasLastAction(PvE.SGE.Psyche) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = PvE.SGE.Psyche;

                if (WasLastAction(PvE.SGE.Phlegma3) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = PvE.SGE.Phlegma3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 4 && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 5 && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 6 && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 7 && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Eukrasia) && DosisCount is 7 && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = PvE.SGE.Eukrasia;

                if (WasLastSpell(PvE.SGE.EukrasianDosis3) && DosisCount is 7 && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = PvE.SGE.EukrasianDosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 8 && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 9 && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = PvE.SGE.Dosis3;

                if (WasLastSpell(PvE.SGE.Dosis3) && DosisCount is 10 && OpenerStep == 15) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 15) actionID = PvE.SGE.Dosis3;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == PvE.SGE.Psyche && IsOnCooldown(PvE.SGE.Psyche)) ||
                    (actionID == PvE.SGE.Phlegma3 && GetRemainingCharges(PvE.SGE.Phlegma3) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
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

    internal class SGE : CustomComboFunctions
    {
        public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
        {
            var healTarget = GetHealTarget(PvE.SGE.Config.SGE_ST_Heal_Adv && PvE.SGE.Config.SGE_ST_Heal_UIMouseOver);

            switch (i)
            {
                case 0:
                    action = PvE.SGE.Soteria;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria);
                    return PvE.SGE.Config.SGE_ST_Heal_Soteria;
                case 1:
                    action = PvE.SGE.Zoe;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe);
                    return PvE.SGE.Config.SGE_ST_Heal_Zoe;
                case 2:
                    action = PvE.SGE.Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianDiagnosis, healTarget, LocalPlayer?.GameObjectId) is not null;
                    return PvE.SGE.Config.SGE_ST_Heal_Pepsis;
                case 3:
                    action = PvE.SGE.Taurochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && PvE.SGE.Gauge.HasAddersgall();
                    return PvE.SGE.Config.SGE_ST_Heal_Taurochole;
                case 4:
                    action = PvE.SGE.Haima;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima);
                    return PvE.SGE.Config.SGE_ST_Heal_Haima;
                case 5:
                    action = PvE.SGE.Krasis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis);
                    return PvE.SGE.Config.SGE_ST_Heal_Krasis;
                case 6:
                    action = PvE.SGE.Druochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && PvE.SGE.Gauge.HasAddersgall();
                    return PvE.SGE.Config.SGE_ST_Heal_Druochole;
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
                    action = PvE.SGE.Kerachole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && (!PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait || (PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait && TraitLevelChecked(PvE.SGE.Traits.EnhancedKerachole))) && PvE.SGE.Gauge.HasAddersgall();
                    return 0;
                case 1:
                    action = PvE.SGE.Ixochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && PvE.SGE.Gauge.HasAddersgall();
                    return 0;
                case 2:
                    action = OriginalHook(PvE.SGE.Physis);
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis);
                    return 0;
                case 3:
                    action = PvE.SGE.Holos;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos);
                    return 0;
                case 4:
                    action = PvE.SGE.Panhaima;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima);
                    return 0;
                case 5:
                    action = PvE.SGE.Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianPrognosis) is not null;
                    return 0;
                case 6:
                    action = PvE.SGE.Philosophia;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Philosophia);
                    return 0;
            }

            enabled = false;
            action = 0;
            return 0;
        }
    }
}
