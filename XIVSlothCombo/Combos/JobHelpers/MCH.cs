using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;
using XIVSlothCombo.Combos.PvE;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MCH
    {
        internal class DelayedOpener : PvE.MCH
        {
            ///<summary> Checks if the player is able to use the Heavy buff allignment Opener. </summary>
            private bool CanDelayedOpener()
            {
                if (CustomComboFunctions.GetRemainingCharges(GaussRound) == 3
                 && CustomComboFunctions.GetRemainingCharges(Ricochet) == 3
                 && CustomComboFunctions.IsOffCooldown(Drill)
                 && CustomComboFunctions.IsOffCooldown(BarrelStabilizer)
                 && CustomComboFunctions.GetRemainingCharges(Reassemble) == 2
                 && CustomComboFunctions.IsOffCooldown(AirAnchor)
                 && CustomComboFunctions.IsOffCooldown(Wildfire)
                 && CustomComboFunctions.IsOffCooldown(ChainSaw) 
                 && !Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat]
                    )
                    return true;

                if (Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                return false;
            }

            private MCHOpenerState currentMCHOpener = MCHOpenerState.NoneReady;
            public MCHOpenerState CurrentMCHOpener
            {
                get
                {
                    return currentMCHOpener;
                }
                set
                {
                    if (value == MCHOpenerState.NoneReady)
                    {
                        justResetDelayedOpener = true;
                    }
                    else
                    {
                        justResetDelayedOpener = false;
                    }

                    currentMCHOpener = value;
                }
            }


            ///<summary> Simple method of using GaussRound.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingGaussRound(ref uint actionID)
            {
                if (GaussRound.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.GaussRoundReady)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Raiton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingRaiton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using Ricochet.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingRicochet(ref uint actionID)
            {
                if (Ricochet.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingKaton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ten);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Katon)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Chi);
                    CurrentMCHOpener = MCHOpenerState.CastingKaton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using Drill.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingDrill(ref uint actionID)
            {
                if (Hyoton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingHyoton)
                {
                    if (!CanSpecialOpener() || CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingHyoton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using BarrelStabilizer.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingBarrelStabilizer(ref uint actionID)
            {
                if (Suiton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingSuiton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Raiton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Suiton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingSuiton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using Reassemble.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingReassemble(ref uint actionID)
            {
                if (Reassemble.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingFumaShuriken)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingFumaShuriken;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using AirAnchor.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingAirAnchor(ref uint actionID)
            {
                if (Huton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingHuton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ten);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Huton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Chi);
                    CurrentMCHOpener = MCHOpenerState.CastingHuton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using Wildfire.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingWildfire(ref uint actionID)
            {
                if (GokaMekkyaku.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingGokaMekkyaku)
                {
                    if (!CanSpecialOpener() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ten);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is GokaMekkyaku)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Chi);
                    CurrentMCHOpener = MCHOpenerState.CastingGokaMekkyaku;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using ChainSaw.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingChainSaw(ref uint actionID)
            {
                if (Doton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingDoton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Doton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingDoton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }
            ///<summary> Simple method of using Automaton Queen.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingQueen(ref uint actionID)
            {
                if (Doton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingDoton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Doton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingDoton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using HyperCharge.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingHyperchargre(ref uint actionID)
            {
                if (Doton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingDoton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Doton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingDoton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            ///<summary> Simple method of using HeatBlast.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to use the aforementioned MCH skill, modifies actionID to the aforementioned MCH skill.</returns>
            public bool UsingHeatBlast(ref uint actionID)
            {
                if (Doton.LevelChecked() && CurrentMCHOpener is MCHOpenerState.NoneReady or MCHOpenerState.CastingDoton)
                {
                    if (!CanSpecialOpener())
                    {
                        CurrentMCHOpener = MCHOpenerState.NoneReady;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Hyoton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Chi);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is Doton)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMCHOpener = MCHOpenerState.CastingDoton;
                    return true;
                }

                CurrentMCHOpener = MCHOpenerState.NoneReady;
                return false;
            }

            private bool justResetDelayedOpener = false;
            public bool ContinueDelayedOpener(ref uint actionID)
            {
                if ((CustomComboFunctions.WasLastAction(FumaShuriken) ||
                    CustomComboFunctions.WasLastAction(Katon) ||
                    CustomComboFunctions.WasLastAction(Raiton) ||
                    CustomComboFunctions.WasLastAction(Hyoton) ||
                    CustomComboFunctions.WasLastAction(Huton) ||
                    CustomComboFunctions.WasLastAction(Doton) ||
                    CustomComboFunctions.WasLastAction(Suiton) ||
                    CustomComboFunctions.WasLastAction(GokaMekkyaku) ||
                    CustomComboFunctions.WasLastAction(HyoshoRanryu)) &&
                    !justResetDelayedOpener)
                    CurrentMCHOpener = MCHOpenerState.NoneReady;


                return CurrentMCHOpener switch
                {
                    MCHOpenerState.NoneReady => false,
                    MCHOpenerState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                    MCHOpenerState.CastingKaton => CastKaton(ref actionID),
                    MCHOpenerState.CastingRaiton => CastRaiton(ref actionID),
                    MCHOpenerState.CastingHyoton => CastHyoton(ref actionID),
                    MCHOpenerState.CastingHuton => CastHuton(ref actionID),
                    MCHOpenerState.CastingDoton => CastDoton(ref actionID),
                    MCHOpenerState.CastingSuiton => CastSuiton(ref actionID),
                    MCHOpenerState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                    MCHOpenerState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                    _ => false,
                };
            }
            public enum MCHOpenerState
            {
                NoneReady,
                GaussRoundReady,
                RicochetReady,
                ReassembleReady,
                DrillReady,
                AirAnchorReady,
                ChainSawReady,
                BarrelStabilizerReady,
                WildfireReady,
                EverythingReady

            }

        }

        internal class MCHOpenerLogic : PvE.MCH
        {
            private static bool HasCooldowns()
            {
                if (CustomComboFunctions.GetRemainingCharges(GaussRound) < 3) return false;
                if (CustomComboFunctions.GetRemainingCharges(Ricochet) < 3) return false;
                if (CustomComboFunctions.GetRemainingCharges(Reassemble) < 2) return false;
                if (CustomComboFunctions.IsOnCooldown(BarrelStabilizer)) return false;
                if (CustomComboFunctions.IsOnCooldown(Wildfire)) return false;
                if (CustomComboFunctions.IsOnCooldown(Drill)) return false;
                if (CustomComboFunctions.IsOnCooldown(AirAnchor)) return false;
                if (CustomComboFunctions.IsOnCooldown(ChainSaw)) return false;

                return true;
            }

            private static uint OpenerLevel => 90;

            public uint PrePullStep = 1;

            public uint OpenerStep = 1;

            public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

            private bool CanOpener => HasCooldowns() && LevelChecked;

            private OpenerState currentState = OpenerState.OpenerFinished;
            
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
                        if (value == OpenerState.PrePull) PrePullStep = 1;
                        if (value == OpenerState.InOpener) OpenerStep = 1;
                        if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener) { PrePullStep = 0; OpenerStep = 0; }

                        currentState = value;
                    }
                }
            }

            private bool DoPrePullSteps(ref uint actionID, MudraCasting mudraState)
            {
                if (!LevelChecked) return false;

                if (CanOpener && PrePullStep == 0 && !CustomComboFunctions.InCombat()) { CurrentState = OpenerState.PrePull; }

                if (CurrentState == OpenerState.PrePull)
                {
                    if (ActionWatching.TimeSinceLastAction.TotalSeconds > 5 && !CustomComboFunctions.InCombat())
                    {
                        mudraState.CastHuton(ref actionID);
                        PrePullStep = 1;
                        return true;
                    }

                    if (CustomComboFunctions.WasLastAction(Huton) && PrePullStep == 1) PrePullStep++;
                    else if (PrePullStep == 1) mudraState.CastHuton(ref actionID);

                    if (CustomComboFunctions.WasLastAction(Hide) && PrePullStep == 2) PrePullStep++;
                    else if (PrePullStep == 2) { actionID = CustomComboFunctions.OriginalHook(Hide); }

                    if (CustomComboFunctions.WasLastAction(Suiton) && PrePullStep == 3) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 3) mudraState.CastSuiton(ref actionID);

                    //Failure states
                    if (PrePullStep is (1 or 2) && CustomComboFunctions.InCombat()) { mudraState.CurrentMCHOpener = MudraCasting.MCHOpenerState.NoneReady; ResetOpener(); }

                    return true;

                }

                PrePullStep = 0;
                return false;
            }

            private bool DoOpener(ref uint actionID, MudraCasting mudraState)
            {
                if (!LevelChecked) return false;
                CustomComboFunctions.SetPrimePotion(Config.BalanceOpenerPotion);

                if (CurrentState == OpenerState.InOpener)
                {
                    bool inLateWeaveWindow = CustomComboFunctions.CanDelayedWeave(GustSlash, 1, 0);
                    
                    if (CustomComboFunctions.PrimedPotion == 0 && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 3) actionID = CustomComboFunctions.ReturnSetPotion(Config.BalanceOpenerPotion, ref actionID);

                    if (CustomComboFunctions.WasLastAction(Kassatsu) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 1) actionID = CustomComboFunctions.OriginalHook(SplitShot);

                    if (CustomComboFunctions.WasLastAction(SpinningEdge) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 2) actionID = CustomComboFunctions.OriginalHook(GaussRound);

                    if (CustomComboFunctions.WasLastAction(GustSlash) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = CustomComboFunctions.OriginalHook(Ricochet);

                    if (CustomComboFunctions.WasLastAction(Mug) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = CustomComboFunctions.OriginalHook(Drill);

                    if (CustomComboFunctions.WasLastAction(Bunshin) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = CustomComboFunctions.OriginalHook(BarrelStabilizer);

                    if (CustomComboFunctions.WasLastAction(PhantomKamaitachi) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = CustomComboFunctions.OriginalHook(SlugShot);

                    if (CustomComboFunctions.WasLastAction(TrickAttack) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8 && inLateWeaveWindow) actionID = CustomComboFunctions.OriginalHook(Ricochet);

                    if (CustomComboFunctions.WasLastAction(AeolianEdge) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = CustomComboFunctions.OriginalHook(CleanShot);

                    if (CustomComboFunctions.WasLastAction(DreamWithinADream) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = CustomComboFunctions.OriginalHook(DreamWithinADream);

                    if (CustomComboFunctions.WasLastAction(HyoshoRanryu) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) mudraState.CastHyoshoRanryu(ref actionID);

                    if (CustomComboFunctions.WasLastAction(Raiton) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) mudraState.CastRaiton(ref actionID);

                    if (CustomComboFunctions.WasLastAction(TenChiJin) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = CustomComboFunctions.OriginalHook(TenChiJin);

                    if (CustomComboFunctions.WasLastAction(TCJFumaShurikenTen) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = CustomComboFunctions.OriginalHook(Ten);

                    if (CustomComboFunctions.WasLastAction(TCJRaiton) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = CustomComboFunctions.OriginalHook(Chi);

                    if (CustomComboFunctions.WasLastAction(TCJSuiton) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = CustomComboFunctions.OriginalHook(Jin);

                    if (CustomComboFunctions.WasLastAction(Meisui) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = CustomComboFunctions.OriginalHook(Meisui);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(Bhavacakra) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(Bhavacakra) && OpenerStep == 21) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 21) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);


                    //Failure states
                    if ((OpenerStep is 14 or 15 or 16 && CustomComboFunctions.IsMoving) ||
                        (OpenerStep is 8 && !CustomComboFunctions.HasEffect(Buffs.Suiton)) ||
                        (OpenerStep is 19 or 21 && CustomComboFunctions.GetJobGauge<NINGauge>().Ninki < 45) ||
                        (OpenerStep is 18 or 20 && !CustomComboFunctions.HasEffect(Buffs.RaijuReady)) ||
                        (OpenerStep is 11 && !CustomComboFunctions.HasEffect(Buffs.Kassatsu)))
                        ResetOpener();


                    return true;
                }

                return false;
            }
            private void ResetOpener()
            {
                CurrentState = OpenerState.FailedOpener;
            }

            private bool openerEventsSetup = false;

            public bool DoFullOpener(ref uint actionID, MudraCasting mudraState)
            {
                if (!LevelChecked) return false;

                if (!openerEventsSetup) { Service.Condition.ConditionChange += CheckCombatStatus; openerEventsSetup = true; }

                if (CurrentState == OpenerState.PrePull || CurrentState == OpenerState.FailedOpener)
                    if (DoPrePullSteps(ref actionID, mudraState)) return true;

                if (CurrentState == OpenerState.InOpener)
                    if (DoOpener(ref actionID, mudraState)) return true;

                if (CurrentState == OpenerState.OpenerFinished && !CustomComboFunctions.InCombat())
                    ResetOpener();

                return false;
            }
            private void CheckCombatStatus(ConditionFlag flag, bool value)
            {
                if (flag == ConditionFlag.InCombat && value == false) ResetOpener();
            }
        }
    }
}
