using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class NIN
    {
        internal class NINHelper : PvE.NIN
        {
            internal static bool TrickDebuff => TargetHasTrickDebuff();
            private static bool TargetHasTrickDebuff()
            {
                return CustomComboFunctions.TargetHasEffect(Debuffs.TrickAttack) || CustomComboFunctions.TargetHasEffect(Debuffs.Dokumori);
            }

            internal static bool MugDebuff => TargetHasMugDebuff();

            private static bool TargetHasMugDebuff()
            {
                return CustomComboFunctions.TargetHasEffect(Debuffs.Mug) || CustomComboFunctions.TargetHasEffect(Debuffs.KunaisBane);
            }

            internal static bool InMudra => GetInMudra();

            private static bool GetInMudra()
            {
                return !CustomComboFunctions.IsOriginal(Ninjutsu);
            }
        }

        internal class MudraCasting : PvE.NIN
        {
            ///<summary> Checks if the player is in a state to be able to cast a ninjitsu.</summary>
            private static bool CanCast()
            {
                var gcd = CustomComboFunctions.GetCooldown(GustSlash).CooldownTotal;

                if (gcd == 0.5) return true;

                if (CustomComboFunctions.GetRemainingCharges(Ten) == 0 &&
                    !CustomComboFunctions.HasEffect(Buffs.Mudra) &&
                    !CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    return false;

                return true;
            }


            private MudraState currentMudra = MudraState.None;
            public MudraState CurrentMudra
            {
                get
                {
                    return currentMudra;
                }
                set
                {
                    if (value == MudraState.None)
                    {
                        justResetMudra = true;
                    }
                    else
                    {
                        justResetMudra = false;
                    }

                    currentMudra = value;
                }
            }

            ///<summary> Simple method of casting Fuma Shuriken.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastFumaShuriken(ref uint actionID)
            {
                if (FumaShuriken.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    CurrentMudra = MudraState.CastingFumaShuriken;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }


            ///<summary> Simple method of casting Raiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastRaiton(ref uint actionID)
            {
                if (Raiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingRaiton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Katon.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastKaton(ref uint actionID)
            {
                if (Katon.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingKaton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingKaton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Hyoton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHyoton(ref uint actionID)
            {
                if (Hyoton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoton)
                {
                    if (!CanCast() || CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingHyoton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Huton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHuton(ref uint actionID)
            {
                if (Huton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHuton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingHuton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Doton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastDoton(ref uint actionID)
            {
                if (Doton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingDoton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingDoton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Suiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastSuiton(ref uint actionID)
            {
                if (Suiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
                {
                    if (!CanCast())
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingSuiton;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Goka Mekkyaku.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastGokaMekkyaku(ref uint actionID)
            {
                if (GokaMekkyaku.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
                {
                    if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
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
                    CurrentMudra = MudraState.CastingGokaMekkyaku;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            ///<summary> Simple method of casting Hyosho Ranryu.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public bool CastHyoshoRanryu(ref uint actionID)
            {
                if (HyoshoRanryu.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
                {
                    if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                    {
                        CurrentMudra = MudraState.None;
                        return false;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Jin);
                        return true;
                    }

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is HyoshoRanryu)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Chi);
                    CurrentMudra = MudraState.CastingHyoshoRanryu;
                    return true;
                }

                CurrentMudra = MudraState.None;
                return false;
            }

            private bool justResetMudra = false;
            public bool ContinueCurrentMudra(ref uint actionID)
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
                    !justResetMudra)
                    CurrentMudra = MudraState.None;


                return CurrentMudra switch
                {
                    MudraState.None => false,
                    MudraState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                    MudraState.CastingKaton => CastKaton(ref actionID),
                    MudraState.CastingRaiton => CastRaiton(ref actionID),
                    MudraState.CastingHyoton => CastHyoton(ref actionID),
                    MudraState.CastingHuton => CastHuton(ref actionID),
                    MudraState.CastingDoton => CastDoton(ref actionID),
                    MudraState.CastingSuiton => CastSuiton(ref actionID),
                    MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                    MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                    _ => false,
                };
            }

            public enum MudraState
            {
                None,
                CastingFumaShuriken,
                CastingKaton,
                CastingRaiton,
                CastingHyoton,
                CastingHuton,
                CastingDoton,
                CastingSuiton,
                CastingGokaMekkyaku,
                CastingHyoshoRanryu

            }
        }

        internal class NINOpenerLogic : PvE.NIN
        {
            private static bool HasCooldowns()
            {
                if (CustomComboFunctions.GetRemainingCharges(Ten) < 1) return false;
                if (CustomComboFunctions.IsOnCooldown(Mug)) return false;
                if (CustomComboFunctions.IsOnCooldown(TenChiJin)) return false;
                if (CustomComboFunctions.IsOnCooldown(PhantomKamaitachi)) return false;
                if (CustomComboFunctions.IsOnCooldown(Bunshin)) return false;
                if (CustomComboFunctions.IsOnCooldown(DreamWithinADream)) return false;
                if (CustomComboFunctions.IsOnCooldown(Kassatsu)) return false;

                return true;
            }

            private static uint OpenerLevel => 100;

            public uint PrePullStep = 1;

            private uint openerStep = 1;

            public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

            private static bool CanOpener => HasCooldowns() && LevelChecked;

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

            public uint OpenerStep
            {
                get => openerStep; set
                {
                    if (value != openerStep)
                    {
                        Svc.Log.Debug($"{value}");
                    }
                    openerStep = value;
                }
            }

            private bool DoPrePullSteps(ref uint actionID, MudraCasting mudraState)
            {
                if (!LevelChecked) return false;

                if (CanOpener && PrePullStep == 0 && !CustomComboFunctions.InCombat()) { CurrentState = OpenerState.PrePull; }

                if (CurrentState == OpenerState.PrePull)
                {
                    if (CustomComboFunctions.WasLastAction(Suiton) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) mudraState.CastSuiton(ref actionID);

                    ////Failure states
                    //if (PrePullStep is (1 or 2) && CustomComboFunctions.InCombat()) { mudraState.CurrentMudra = MudraCasting.MudraState.None; ResetOpener(); }

                    return true;

                }

                PrePullStep = 0;
                return false;
            }

            private bool DoOpener(ref uint actionID, MudraCasting mudraState)
            {
                if (!LevelChecked) return false;

                if (CurrentState == OpenerState.InOpener)
                {
                    bool inLateWeaveWindow = CustomComboFunctions.CanDelayedWeave(GustSlash, 1, 0);

                    if (CustomComboFunctions.WasLastAction(Kassatsu) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = CustomComboFunctions.OriginalHook(Kassatsu);

                    if (CustomComboFunctions.WasLastAction(SpinningEdge) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = CustomComboFunctions.OriginalHook(SpinningEdge);

                    if (CustomComboFunctions.WasLastAction(GustSlash) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = CustomComboFunctions.OriginalHook(GustSlash);

                    if (CustomComboFunctions.WasLastAction(CustomComboFunctions.OriginalHook(Mug)) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = CustomComboFunctions.OriginalHook(Mug);

                    if (CustomComboFunctions.WasLastAction(Bunshin) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = CustomComboFunctions.OriginalHook(Bunshin);

                    if (CustomComboFunctions.WasLastAction(PhantomKamaitachi) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = CustomComboFunctions.OriginalHook(PhantomKamaitachi);

                    if (CustomComboFunctions.WasLastAction(ArmorCrush) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = CustomComboFunctions.OriginalHook(ArmorCrush);

                    if (CustomComboFunctions.WasLastAction(CustomComboFunctions.OriginalHook(TrickAttack)) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8 && inLateWeaveWindow) actionID = CustomComboFunctions.OriginalHook(TrickAttack);

                    if (CustomComboFunctions.WasLastAction(HyoshoRanryu) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) mudraState.CastHyoshoRanryu(ref actionID);

                    if (CustomComboFunctions.WasLastAction(DreamWithinADream) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = CustomComboFunctions.OriginalHook(DreamWithinADream);

                    if (CustomComboFunctions.WasLastAction(Raiton) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) mudraState.CastRaiton(ref actionID);

                    if (CustomComboFunctions.WasLastAction(TenChiJin) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = CustomComboFunctions.OriginalHook(TenChiJin);

                    if (CustomComboFunctions.WasLastAction(TCJFumaShurikenTen) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = CustomComboFunctions.OriginalHook(Ten);

                    if (CustomComboFunctions.WasLastAction(TCJRaiton) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = CustomComboFunctions.OriginalHook(Chi);

                    if (CustomComboFunctions.WasLastAction(TCJSuiton) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = CustomComboFunctions.OriginalHook(Jin);

                    if (CustomComboFunctions.WasLastAction(Meisui) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = CustomComboFunctions.OriginalHook(Meisui);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(ZeshoMeppo) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);

                    if (CustomComboFunctions.WasLastAction(TenriJendo) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = CustomComboFunctions.OriginalHook(TenriJendo);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(CustomComboFunctions.OriginalHook(Bhavacakra)) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);

                    if (CustomComboFunctions.WasLastAction(Raiton) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) mudraState.CastRaiton(ref actionID);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 23) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 23) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);


                    //Failure states
                    if ((OpenerStep is 13 or 14 or 15 && CustomComboFunctions.IsMoving) ||
                        (OpenerStep is 8 && !CustomComboFunctions.HasEffect(Buffs.ShadowWalker)) ||
                        (OpenerStep is 18 or 21 && CustomComboFunctions.GetJobGauge<NINGauge>().Ninki < 40) ||
                        (OpenerStep is 17 or 20 && !CustomComboFunctions.HasEffect(Buffs.RaijuReady)) ||
                        (OpenerStep is 9 && !CustomComboFunctions.HasEffect(Buffs.Kassatsu)))
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

                if (!openerEventsSetup) { Svc.Condition.ConditionChange += CheckCombatStatus; openerEventsSetup = true; }

                if (CurrentState == OpenerState.PrePull || CurrentState == OpenerState.FailedOpener)
                    if (DoPrePullSteps(ref actionID, mudraState)) return true;

                if (CurrentState == OpenerState.InOpener)
                    if (DoOpener(ref actionID, mudraState)) return true;

                if (CurrentState == OpenerState.OpenerFinished && !CustomComboFunctions.InCombat())
                    ResetOpener();

                return false;
            }

            internal void Dispose()
            {
                Svc.Condition.ConditionChange -= CheckCombatStatus;
            }

            private void CheckCombatStatus(ConditionFlag flag, bool value)
            {
                if (flag == ConditionFlag.InCombat && value == false) ResetOpener();
            }
        }
    }
}
