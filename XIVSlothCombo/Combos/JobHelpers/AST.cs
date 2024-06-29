using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
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

            if (DrawnCard != Gauge.DrawnCard)
            {
                DrawnCard = Gauge.DrawnCard;
                if (CustomComboFunctions.IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards))
                {
                    AST_QuickTargetCards.SelectedRandomMember = null;
                    AST_QuickTargetCards.Invoke();
                }
                if (DrawnCard == CardType.NONE)
                    AST_QuickTargetCards.SelectedRandomMember = null;

            }
        }

        internal class AST_QuickTargetCards : CustomComboFunctions
        {

            internal static List<IGameObject> PartyTargets = [];

            internal static IGameObject? SelectedRandomMember;

            public static void Invoke()
            {
                if (GetPartySlot(2) is not null && DrawnCard is not CardType.NONE)
                {
                    if (SelectedRandomMember is null || SelectedRandomMember.IsDead)
                    {
                        SetTarget();
                    }
                }
                else
                {
                    SelectedRandomMember = null;
                }
            }

            private static bool SetTarget()
            {
                if (Gauge.DrawnCard.Equals(CardType.NONE)) return false;
                CardType cardDrawn = Gauge.DrawnCard;
                PartyTargets.Clear();
                for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                {
                    if (GetPartySlot(i) is not IBattleChara member) continue;
                    if (member is null) continue; //Skip nulls/disconnected people
                    if (member.IsDead) continue;
                    if (OutOfRange(Bole, member)) continue;

                    if (FindEffectOnMember(Buffs.BalanceDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.ArrowDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.BoleDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.EwerDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpireDamage, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpearDamage, member) is not null) continue;

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
                        if (OutOfRange(Bole, member)) continue;

                        if (FindEffectOnMember(Buffs.BalanceDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.ArrowDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.BoleDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.EwerDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpireDamage, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpearDamage, member) is not null) continue;

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
                        if (((cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR) && JobIDs.Melee.Contains(job)) ||
                            ((cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE) && JobIDs.Ranged.Contains(job)))
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
                            if ((cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR && JobIDs.Tank.Contains(job)) ||
                                (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE && JobIDs.Healer.Contains(job)))
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
