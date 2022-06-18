using Dalamud.Game.ClientState.Conditions;
using System;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class NIN
    {
        internal class MudraCasting : PvE.NIN
        {
            ///<summary> Checks if the player is in a state to be able to cast a ninjitsu.</summary>
            private static bool CanCast()
            {
                if (CustomComboFunctions.GetRemainingCharges(Ten) == 0 &&
                    !CustomComboFunctions.HasEffect(Buffs.Mudra) &&
                    !CustomComboFunctions.HasEffect(Buffs.Kassatsu)) return false;

                return true;
            }

            ///<summary> Simple method of casting Fuma Shuriken.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastFumaShuriken(ref uint actionID)
            {
                if (FumaShuriken.LevelChecked())
                {
                    if (!CanCast()) return false;

                    if (CustomComboFunctions.OriginalHook(Ninjutsu) is FumaShuriken)
                    {
                        actionID = CustomComboFunctions.OriginalHook(Ninjutsu);
                        return true;
                    }

                    actionID = CustomComboFunctions.OriginalHook(Ten);
                    return true;
                }

                return false;
            }


            ///<summary> Simple method of casting Raiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastRaiton(ref uint actionID)
            {
                if (Raiton.LevelChecked())
                {
                    if (!CanCast()) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Katon.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastKaton(ref uint actionID)
            {
                if (Katon.LevelChecked())
                {
                    if (!CanCast()) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Hyoton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastHyoton(ref uint actionID)
            {
                if (Hyoton.LevelChecked())
                {
                    if (!CanCast() || CustomComboFunctions.HasEffect(Buffs.Kassatsu)) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Huton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastHuton(ref uint actionID)
            {
                if (Huton.LevelChecked())
                {
                    if (!CanCast()) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Doton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastDoton(ref uint actionID)
            {
                if (Doton.LevelChecked())
                {
                    if (!CanCast()) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Suiton.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastSuiton(ref uint actionID)
            {
                if (Suiton.LevelChecked())
                {
                    if (!CanCast()) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Goka Mekkyaku.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastGokaMekkyaku(ref uint actionID)
            {
                if (GokaMekkyaku.LevelChecked())
                {
                    if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu)) return false;

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
                    return true;
                }

                return false;
            }

            ///<summary> Simple method of casting Hyosho Ranryu.</summary>
            /// <param name="actionID">The actionID from the combo.</param>
            /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
            public static bool CastHyoshoRanryu(ref uint actionID)
            {
                if (HyoshoRanryu.LevelChecked())
                {
                    if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu)) return false;

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
                    return true;
                }

                return false;
            }
        }

        internal class OpenerLogic : PvE.NIN
        {
            private static bool HasCooldowns()
            {
                if (CustomComboFunctions.GetRemainingCharges(Ten) < 2) return false;
                if (CustomComboFunctions.IsOnCooldown(Mug)) return false;
                if (CustomComboFunctions.IsOnCooldown(TenChiJin)) return false;
                if (CustomComboFunctions.IsOnCooldown(PhantomKamaitachi)) return false;
                if (CustomComboFunctions.IsOnCooldown(Bunshin)) return false;
                if (CustomComboFunctions.IsOnCooldown(DreamWithinADream)) return false;
                if (CustomComboFunctions.IsOnCooldown(Kassatsu)) return false;

                return true;
            }

            private static uint OpenerLevel => 90;

            private static uint PrePullStep = 0;

            private static uint OpenerStep = 0;

            private static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

            private static bool CanOpener => HasCooldowns() && LevelChecked;

            private static OpenerState currentState = OpenerState.NoState;

            private static OpenerState CurrentState
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
                        if (value == OpenerState.OpenerFinished || value == OpenerState.NoState) { PrePullStep = 0; OpenerStep = 0; }

                        currentState = value;
                    }
                }
            }

            private static bool DoPrePullSteps(ref uint actionID)
            {
                if (CanOpener && PrePullStep == 0 && !CustomComboFunctions.InCombat()) { CurrentState = OpenerState.PrePull; }

                if (CurrentState == OpenerState.PrePull)
                {
                    if (CustomComboFunctions.WasLastAction(Huton) && PrePullStep == 1) PrePullStep++;
                    else if (PrePullStep == 1) MudraCasting.CastHuton(ref actionID);

                    if (PrePullStep == 2 && CustomComboFunctions.InCombat()) ResetOpener();

                    if (CustomComboFunctions.WasLastAction(Hide) && PrePullStep == 2) PrePullStep++;
                    else if (PrePullStep == 2) { actionID = CustomComboFunctions.OriginalHook(Hide); }

                    if (CustomComboFunctions.WasLastAction(Suiton) && PrePullStep == 3) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 3) MudraCasting.CastSuiton(ref actionID);

                    return true;

                }

                PrePullStep = 0;
                return false;
            }

            private static bool DoOpener(ref uint actionID)
            {
                if (CurrentState == OpenerState.InOpener)
                {
                    //Failure states
                    if ((OpenerStep is 14 or 15 or 16 && CustomComboFunctions.IsMoving) ||
                        (OpenerStep is 7 && !CustomComboFunctions.HasEffect(Buffs.Suiton)) ||
                        (OpenerStep is 10 && !CustomComboFunctions.HasEffect(Buffs.Kassatsu))) 
                        CurrentState = OpenerState.OpenerFinished;

                    if (CustomComboFunctions.WasLastAction(Kassatsu) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = CustomComboFunctions.OriginalHook(Kassatsu);

                    if (CustomComboFunctions.WasLastAction(SpinningEdge) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = CustomComboFunctions.OriginalHook(SpinningEdge);

                    if (CustomComboFunctions.WasLastAction(GustSlash) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = CustomComboFunctions.OriginalHook(GustSlash);

                    if (CustomComboFunctions.WasLastAction(Mug) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = CustomComboFunctions.OriginalHook(Mug);

                    if (CustomComboFunctions.WasLastAction(Bunshin) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = CustomComboFunctions.OriginalHook(Bunshin);

                    if (CustomComboFunctions.WasLastAction(PhantomKamaitachi) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = CustomComboFunctions.OriginalHook(PhantomKamaitachi);

                    if (CustomComboFunctions.WasLastAction(TrickAttack) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = CustomComboFunctions.OriginalHook(TrickAttack);

                    if (CustomComboFunctions.WasLastAction(AeolianEdge) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = CustomComboFunctions.OriginalHook(AeolianEdge);

                    if (CustomComboFunctions.WasLastAction(DreamWithinADream) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = CustomComboFunctions.OriginalHook(DreamWithinADream);

                    if (CustomComboFunctions.WasLastAction(HyoshoRanryu) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) MudraCasting.CastHyoshoRanryu(ref actionID);

                    if (CustomComboFunctions.WasLastAction(Raiton) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) MudraCasting.CastRaiton(ref actionID);

                    if (CustomComboFunctions.WasLastAction(TenChiJin) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = CustomComboFunctions.OriginalHook(TenChiJin);

                    if (CustomComboFunctions.WasLastAction(TCJFumaShuriken) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = CustomComboFunctions.OriginalHook(Ten);

                    if (CustomComboFunctions.WasLastAction(TCJRaiton) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = CustomComboFunctions.OriginalHook(Chi);

                    if (CustomComboFunctions.WasLastAction(TCJSuiton) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = CustomComboFunctions.OriginalHook(Jin);

                    if (CustomComboFunctions.WasLastAction(Meisui) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = CustomComboFunctions.OriginalHook(Meisui);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(Bhavacakra) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);

                    if (CustomComboFunctions.WasLastAction(FleetingRaiju) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = CustomComboFunctions.OriginalHook(FleetingRaiju);

                    if (CustomComboFunctions.WasLastAction(Bhavacakra) && OpenerStep == 20) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 20) actionID = CustomComboFunctions.OriginalHook(Bhavacakra);

                    return true;
                }

                return false;
            }

            private static void ResetOpener()
            {
                CurrentState = OpenerState.NoState;
            }

            private static bool openerEventsSetup = false;

            public static bool DoFullOpener(ref uint actionID)
            {
                if (!openerEventsSetup) { Service.Condition.ConditionChange += CheckCombatStatus; openerEventsSetup = true; }

                if (CurrentState == OpenerState.PrePull || CurrentState == OpenerState.NoState)
                    if (DoPrePullSteps(ref actionID)) return true;

                if (CurrentState == OpenerState.InOpener)
                    if (DoOpener(ref actionID)) return true;

                if (CurrentState == OpenerState.OpenerFinished && !CustomComboFunctions.InCombat())
                    ResetOpener();

                return false;
            }

            private static void CheckCombatStatus(ConditionFlag flag, bool value)
            {
                if (flag == ConditionFlag.InCombat && value == false) ResetOpener();
            }
        }

        internal enum OpenerState
        {
            PrePull,
            InOpener,
            OpenerFinished,
            NoState
        }
    }
}
