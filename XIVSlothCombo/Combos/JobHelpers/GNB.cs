using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class GNBOpenerLogic : GNB
    {
        #region Public Variables

        //INT

        //Opener
        public uint prePullStep;
        public uint openerStep;

        //BOOL
        public static bool OpenerLevelChecked => CustomComboFunctions.LocalPlayer.Level >= _openerLevel;

        #endregion

        #region Private Variables

        //INT
        private static uint _openerLevel => 100;

        //BOOL

        private static bool CanDoOpener => GNB_Helpers.IsNoMercyReady() && GNB_Helpers.IsBloodfestReady() && OpenerLevelChecked;

        //STATES
        private OpenerState _currentOpenerState = OpenerState.PrePull;

        #endregion
        
        #region Getters & Setters
        
        public OpenerState CurrentOpenerState
        {
            get
            {
                return _currentOpenerState;
            }
            set
            {
                if (value != _currentOpenerState)
                {
                    if (value == OpenerState.PrePull)
                    {
                        Svc.Log.Debug($"Entered PrePull Opener");
                    }
                    if (value == OpenerState.InOpener) openerStep = 1;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {openerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    _currentOpenerState = value;
                }
            }
        }
        
        #endregion

        #region Private Functions

        #region Opener Logic

        //------------- Opener Logic Begin -------------
        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!OpenerLevelChecked) return false;

            if (CanDoOpener && prePullStep == 0) prePullStep = 1;

            if (!GNB_Helpers.IsNoMercyReady() && !GNB_Helpers.IsBloodfestReady()) prePullStep = 0;

            if (CurrentOpenerState == OpenerState.PrePull && prePullStep > 0)
            {
                if (CustomComboFunctions.WasLastWeaponskill(LightningShot) && prePullStep == 1)
                    CurrentOpenerState = OpenerState.InOpener;
                else if (prePullStep == 1) actionID = LightningShot;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentOpenerState = OpenerState.FailedOpener;

                return true;
            }

            prePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!OpenerLevelChecked) return false;

            if (_currentOpenerState == OpenerState.InOpener)
            {
                //Keen Edge
                if (CustomComboFunctions.WasLastWeaponskill(KeenEdge) && openerStep == 1) openerStep++;
                else if (openerStep == 1) actionID = KeenEdge;

                //Brutal Shell
                if (CustomComboFunctions.WasLastWeaponskill(BrutalShell) && openerStep == 2) openerStep++;
                else if (openerStep == 2) actionID = BrutalShell;

                //No Mercy
                if (CustomComboFunctions.WasLastAbility(NoMercy) && openerStep == 3) openerStep++;
                else if (openerStep == 3) actionID = NoMercy;

                //Bloodfest
                if (CustomComboFunctions.WasLastAbility(Bloodfest) && openerStep == 4) openerStep++;
                else if (openerStep == 4) actionID = Bloodfest;
                
                //Sonic Break
                if (CustomComboFunctions.WasLastWeaponskill(SonicBreak) && openerStep == 5) openerStep++;
                else if (openerStep == 5) actionID = SonicBreak;
                
                //Bow Shock
                if (CustomComboFunctions.WasLastAbility(BowShock) && openerStep == 6) openerStep++;
                else if (openerStep == 6) actionID = BowShock;
                
                //DoubleDown
                if (CustomComboFunctions.WasLastWeaponskill(DoubleDown) && openerStep == 7) openerStep++;
                else if (openerStep == 7) actionID = DoubleDown;
                
                //Blasting Zone
                if (CustomComboFunctions.WasLastAbility(BlastingZone) && openerStep == 8) openerStep++;
                else if (openerStep == 8) actionID = BlastingZone;
                
                //Gnashing Fang
                if (CustomComboFunctions.WasLastWeaponskill(GnashingFang) && openerStep == 9) openerStep++;
                else if (openerStep == 9) actionID = GnashingFang;
                
                //Jugular Rip
                if (CustomComboFunctions.WasLastAbility(JugularRip) && openerStep == 10) openerStep++;
                else if (openerStep == 10) actionID = JugularRip;
                
                //Savage Claw
                if (CustomComboFunctions.WasLastWeaponskill(SavageClaw) && openerStep == 11) openerStep++;
                else if (openerStep == 11) actionID = SavageClaw;
                
                //Abdomen Tear
                if (CustomComboFunctions.WasLastAbility(AbdomenTear) && openerStep == 12) openerStep++;
                else if (openerStep == 12) actionID = AbdomenTear;
                
                //Wicked Talon
                if (CustomComboFunctions.WasLastWeaponskill(WickedTalon) && openerStep == 13) openerStep++;
                else if (openerStep == 13) actionID = WickedTalon;
                
                //Eye Gouge
                if (CustomComboFunctions.WasLastAbility(EyeGouge) && openerStep == 14) openerStep++;
                else if (openerStep == 14) actionID = EyeGouge;
                
                //Reign of Beasts
                if (CustomComboFunctions.WasLastWeaponskill(ReignOfBeasts) && openerStep == 15) openerStep++;
                else if (openerStep == 15) actionID = ReignOfBeasts;

                //Nobleblood
                if (CustomComboFunctions.WasLastWeaponskill(NobleBlood) && openerStep == 16) openerStep++;
                else if (openerStep == 16) actionID = NobleBlood;

                //LionHeart
                if (CustomComboFunctions.WasLastWeaponskill(LionHeart) && openerStep == 17) CurrentOpenerState = OpenerState.OpenerFinished;
                else if (openerStep == 17) actionID = LionHeart;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5) CurrentOpenerState = OpenerState.FailedOpener;

                return true;

            }

            return false;
        }

        private void ResetOpener()
        {
            prePullStep = 0;
            openerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!OpenerLevelChecked) return false;

            if (CurrentOpenerState == OpenerState.PrePull)
            {
                if (DoPrePullSteps(ref actionID)) return true;
            }

            if (CurrentOpenerState == OpenerState.InOpener)
            {
                if (DoOpener(ref actionID)) return true;
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentOpenerState = OpenerState.PrePull;
            }

            return false;
        }

        //------------- Opener Logic End ----------------

        #endregion
        
        #endregion
    }

    #region GNB Helper

    internal static class GNB_Helpers
    {
        public static bool IsNoMercyReady()
        {
            return CustomComboFunctions.ActionReady(GNB.NoMercy);
        }
        
        public static bool IsBloodfestReady()
        {
            return CustomComboFunctions.ActionReady(GNB.Bloodfest);
        }
    }

    #endregion
}

