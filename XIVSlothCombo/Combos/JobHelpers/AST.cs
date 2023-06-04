using Dalamud.Game;
using Dalamud.Game.ClientState.JobGauge.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSlothCombo.CustomComboNS.Functions;
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

        private static void CheckCards(Framework framework)
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

        internal static void Dispose()
        {
            Service.Framework.Update -= CheckCards;
        }
    }
}
