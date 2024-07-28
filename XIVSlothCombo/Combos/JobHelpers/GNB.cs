using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;


namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class GNBOpenerLogic : GNB
    {
        #region Variables
        //ints
        public uint _openerStep = 0;
        public uint _prePullStep = 0;
        
        private static uint _openerLevel => 100;
        
        //bools
        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(NoMercy)) return false;
            if (!CustomComboFunctions.ActionReady(Bloodfest)) return false;
            
            return true;
        }
        
        public static bool _levelChecked => CustomComboFunctions.LocalPlayer.Level >= _openerLevel;
        private static bool _canDoOpener => HasCooldowns() && _levelChecked;
        
        //States
        private OpenerState _currentState = OpenerState.PrePull;

        #endregion
        
        #region Functions
        
        private void ResetOpener()
        {
            _prePullStep = 0;
            _openerStep = 0;
        }

        public OpenerState CurrentOpenerState
        {
            get
            {
                return _currentState;
            }
            set
            {
                if (value != _currentState)
                {
                    Svc.Log.Debug($"Changing Opener State from {_currentState} to {value}");
                    if (value == OpenerState.PrePull)
                    {
                        Svc.Log.Debug("Entered PrePull Opener");
                    }
                    if (value == OpenerState.InOpener)
                    {
                        _openerStep = 1;
                        Svc.Log.Debug("Opener Step set to 1");
                    }
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {_openerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");
                    
                    _currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            //if level is not high enough, return false
            if (!_levelChecked) return false;
    
            //if we can do the opener and our prepull is 0, set it to 1
            if (_canDoOpener && _prePullStep == 0)
            {
                _prePullStep = 1;
                Svc.Log.Information($"PrePull Step {_prePullStep}");
            }
    
            //if no cooldowns reset opener
            if (!HasCooldowns())
            {
                _prePullStep = 0;
            }

            if (CurrentOpenerState == OpenerState.PrePull && _prePullStep > 0)
            {
                //Svc.Log.Debug($"Checking transition conditions for InOpener: LastAction={CustomComboFunctions.WasLastAction(LightningShot)}, PrePullStep={_prePullStep}");
        
                if (CustomComboFunctions.WasLastAction(LightningShot) && _prePullStep == 1) 
                {
                    Svc.Log.Debug("Transitioning to InOpener");
                    CurrentOpenerState = OpenerState.InOpener;
                }
                else if (_prePullStep == 1) 
                {
                    actionID = LightningShot;
                }
        
                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentOpenerState = OpenerState.FailedOpener;
        
                return true;
            }
            _prePullStep = 0;
            return false;
        }
        
        private bool DoOpenerSteps(ref uint actionID)
        {
            if (!_levelChecked) return false;
            
            if (_currentState == OpenerState.InOpener)
            {
                // Solid Barrel
                if (CustomComboFunctions.WasLastWeaponskill(KeenEdge) && _openerStep == 1)
                {
                    _openerStep++;
                }
                else if (_openerStep == 1) 
                {
                    actionID = KeenEdge;
                }

                // Brutal Shell
                if (CustomComboFunctions.WasLastWeaponskill(BrutalShell) && _openerStep == 2)
                {
                    _openerStep++;
                    Svc.Log.Debug($"Incremented _openerStep to {_openerStep} after BrutalShell");
                }
                else if (_openerStep == 2) 
                {
                    actionID = BrutalShell;
                    Svc.Log.Debug("Setting actionID to BrutalShell");
                }

                //Weave No Mercy
                if (CustomComboFunctions.WasLastAbility(NoMercy) && _openerStep == 3) _openerStep++;
                else if (_openerStep == 3) actionID = NoMercy;

                //Weave Bloodfest
                if (CustomComboFunctions.WasLastAbility(Bloodfest) && _openerStep == 4) _openerStep++;
                else if (_openerStep == 4) actionID = Bloodfest;

                //Sonic Break
                if (CustomComboFunctions.WasLastWeaponskill(SonicBreak) && _openerStep == 5) _openerStep++;
                else if (_openerStep == 5) actionID = SonicBreak;

                //Weave Bow Shock
                if (CustomComboFunctions.WasLastAbility(BowShock) && _openerStep == 6) _openerStep++;
                else if (_openerStep == 6) actionID = BowShock;

                //Doubledown
                if (CustomComboFunctions.WasLastWeaponskill(DoubleDown) && _openerStep == 7) _openerStep++;
                else if (_openerStep == 7) actionID = DoubleDown;

                //Blasting Zone
                if (CustomComboFunctions.WasLastAbility(BlastingZone) && _openerStep == 8) _openerStep++;
                else if (_openerStep == 8) actionID = BlastingZone;

                //Gnashing Fang
                if (CustomComboFunctions.WasLastWeaponskill(GnashingFang) && _openerStep == 9) _openerStep++;
                else if (_openerStep == 9) actionID = GnashingFang;

                //Jugular Rip
                if (CustomComboFunctions.WasLastAbility(JugularRip) && _openerStep == 10) _openerStep++;
                else if (_openerStep == 10) actionID = JugularRip;

                //Savage Claw
                if (CustomComboFunctions.WasLastAction(SavageClaw) && _openerStep == 11) _openerStep++;
                else if (_openerStep == 11) actionID = SavageClaw;

                //Abdomen Tear
                if (CustomComboFunctions.WasLastAbility(AbdomenTear) && _openerStep == 12) _openerStep++;
                else if (_openerStep == 12) actionID = AbdomenTear;

                //Wicked Talon
                if (CustomComboFunctions.WasLastAction(WickedTalon) && _openerStep == 13) _openerStep++;
                else if (_openerStep == 13) actionID = WickedTalon;

                //Eye Gouge
                if (CustomComboFunctions.WasLastAbility(EyeGouge) && _openerStep == 14) _openerStep++;
                else if (_openerStep == 14) actionID = EyeGouge;

                //Reign of Beasts
                if (CustomComboFunctions.WasLastAction(ReignOfBeasts) && _openerStep == 15) _openerStep++;
                else if (_openerStep == 15) actionID = ReignOfBeasts;

                //Noble Blood
                if (CustomComboFunctions.WasLastAction(NobleBlood) && _openerStep == 16) _openerStep++;
                else if (_openerStep == 16) actionID = NobleBlood;

                //Lionheart
                if (CustomComboFunctions.WasLastAction(LionHeart) && _openerStep == 17) _openerStep++;
                else if (_openerStep == 17) actionID = LionHeart;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                {
                    CurrentOpenerState = OpenerState.FailedOpener;
                }

                return true;
            }
            return false;
        }
        
        public bool DoFullOpener(ref uint actionID)
        {
            if (CurrentOpenerState == OpenerState.PrePull)
            {
                if (DoPrePullSteps(ref actionID)) return true;
            }
            
            if (CurrentOpenerState == OpenerState.InOpener)
            {
                if (DoOpenerSteps(ref actionID)) return true;
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentOpenerState = OpenerState.PrePull;
            }
            return false;
        }
        
        #endregion

        #region GNB Helpers

        internal static class GNBHelpers
        {
            //check it out later
            public static bool IsNoMercyReady()
            {
                return CustomComboFunctions.ActionReady(NoMercy);
            }
            
            public static bool IsBloodfestReady()
            {
                return CustomComboFunctions.ActionReady(Bloodfest);
            }
            
            public static bool IsDoubleDownReady()
            {
                return CustomComboFunctions.ActionReady(DoubleDown);
            }
        }

        #endregion


    }
}

