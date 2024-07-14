using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;
using static XIVSlothCombo.Combos.PvE.AST;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal static class AST
    {
        internal static void Init()
        {
            Service.Framework.Update += CheckCards;
        }

        private static void CheckCards(IFramework framework)
        {
            if (Service.ClientState.LocalPlayer is null || Service.ClientState.LocalPlayer.ClassJob.Id != 33)
                return;

            if (Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas] || Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Unconscious])
            {
                AST_QuickTargetCards.SelectedRandomMember = null;
                return;
            }

            if (DrawnCard != Gauge.DrawnCards[0])
            {
                DrawnCard = Gauge.DrawnCards[0];
            }

            if (CustomComboFunctions.IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards) && 
                (AST_QuickTargetCards.SelectedRandomMember is null || BetterTargetAvailable()))
            {
                if (CustomComboFunctions.ActionReady(Play1))
                    AST_QuickTargetCards.Invoke();
            }

            if (DrawnCard == CardType.NONE)
                AST_QuickTargetCards.SelectedRandomMember = null;

        }

        private static bool BetterTargetAvailable()
        {
            if (AST_QuickTargetCards.SelectedRandomMember is null ||
                AST_QuickTargetCards.SelectedRandomMember.IsDead ||
                CustomComboFunctions.OutOfRange(Balance, AST_QuickTargetCards.SelectedRandomMember))
                return true;

            Svc.Log.Debug($"Picking better?");
            var targets = new List<IBattleChara>();
            for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
            {
                if (CustomComboFunctions.GetPartySlot(i) is not IBattleChara member) continue;
                if (member is null) continue; //Skip nulls/disconnected people
                if (member.IsDead) continue;
                if (CustomComboFunctions.OutOfRange(Balance, member)) continue;

                if (CustomComboFunctions.FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                if (CustomComboFunctions.FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                if (Config.AST_QuickTarget_SkipDamageDown && CustomComboFunctions.TargetHasDamageDown(member)) continue;
                if (Config.AST_QuickTarget_SkipRezWeakness && CustomComboFunctions.TargetHasRezWeakness(member)) continue;

                targets.Add(member);
            }

            if (targets.Count == 0) return false;
            if ((DrawnCard is CardType.BALANCE && targets.Any(x => CustomComboFunctions.JobIDs.Melee.Any(y => y == x.ClassJob.Id))) ||
                (DrawnCard is CardType.SPEAR && targets.Any(x => CustomComboFunctions.JobIDs.Ranged.Any(y => y == x.ClassJob.Id))))
                return true;

            return false;

        }

        internal class AST_QuickTargetCards : CustomComboFunctions
        {

            internal static List<IGameObject> PartyTargets = [];

            internal static IGameObject? SelectedRandomMember;

            public static void Invoke()
            {
                if (DrawnCard is not CardType.NONE)
                {
                    if (GetPartySlot(2) is not null)
                    {
                        SetTarget();
                        Svc.Log.Debug($"Set card to {SelectedRandomMember.Name}");
                    }
                    else
                    {
                        SelectedRandomMember = LocalPlayer;
                    }
                }
                else
                {
                    SelectedRandomMember = null;
                }
            }

            private static bool SetTarget()
            {
                if (Gauge.DrawnCards[0].Equals(CardType.NONE)) return false;
                CardType cardDrawn = Gauge.DrawnCards[0];
                PartyTargets.Clear();
                for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                {
                    if (GetPartySlot(i) is not IBattleChara member) continue;
                    if (member is null) continue; //Skip nulls/disconnected people
                    if (member.IsDead) continue;
                    if (OutOfRange(Balance, member)) continue;

                    if (FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                    if (Config.AST_QuickTarget_SkipDamageDown && TargetHasDamageDown(member)) continue;
                    if (Config.AST_QuickTarget_SkipRezWeakness && TargetHasRezWeakness(member)) continue;

                    PartyTargets.Add(member);
                }
                //The inevitable "0 targets found" because of debuffs
                if (PartyTargets.Count == 0)
                {
                    for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                    {
                        if (GetPartySlot(i) is not IBattleChara member) continue;
                        if (member is null) continue; //Skip nulls/disconnected people
                        if (member.IsDead) continue;
                        if (OutOfRange(Balance, member)) continue;

                        if (FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                        PartyTargets.Add(member);
                    }
                }

                if (SelectedRandomMember is not null)
                {
                    if (PartyTargets.Any(x => x.GameObjectId == SelectedRandomMember.GameObjectId))
                    {
                        //TargetObject(SelectedRandomMember);
                        return true;
                    }
                }


                if (PartyTargets.Count > 0)
                {
                    PartyTargets.Shuffle();
                    //Give card to DPS first
                    for (int i = 0; i <= PartyTargets.Count - 1; i++)
                    {
                        byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.Id : (byte)0;
                        if (((cardDrawn is CardType.BALANCE) && JobIDs.Melee.Contains(job)) ||
                            ((cardDrawn is CardType.SPEAR) && JobIDs.Ranged.Contains(job)))
                        {
                            //TargetObject(PartyTargets[i]);
                            SelectedRandomMember = PartyTargets[i];
                            return true;
                        }
                    }
                    //Give card to unsuitable DPS next
                    for (int i = 0; i <= PartyTargets.Count - 1; i++)
                    {
                        byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.Id : (byte)0;
                        if (((cardDrawn is CardType.BALANCE) && JobIDs.Ranged.Contains(job)) ||
                            ((cardDrawn is CardType.SPEAR) && JobIDs.Melee.Contains(job)))
                        {
                            //TargetObject(PartyTargets[i]);
                            SelectedRandomMember = PartyTargets[i];
                            return true;
                        }
                    }

                    //Give cards to healers/tanks if backup is turned on
                    if (IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards_TargetExtra))
                    {
                        for (int i = 0; i <= PartyTargets.Count - 1; i++)
                        {
                            byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.Id : (byte)0;
                            if ((cardDrawn is CardType.BALANCE && JobIDs.Tank.Contains(job)) ||
                                (cardDrawn is CardType.SPEAR && JobIDs.Healer.Contains(job)))
                            {
                                //TargetObject(PartyTargets[i]);
                                SelectedRandomMember = PartyTargets[i];
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        internal static void Dispose()
        {
            Service.Framework.Update -= CheckCards;
        }
    }
}
