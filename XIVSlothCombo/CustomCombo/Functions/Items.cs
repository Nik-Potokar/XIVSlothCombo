using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        internal static Dictionary<uint, Lumina.Excel.GeneratedSheets.Item> ItemSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>()!
            .Where(x => !x.RowId.Equals(4570) && x.RowId >= 4594 && x.ItemUICategory.Value.RowId == 44)
            .ToDictionary(i => i.RowId, i => i);

        internal static uint PrimedPotion { get; set; }
        internal static uint ReturnSetPotion(string config, ref uint actionID)
        {
            if (SetPrimePotion(config))
            actionID = 16436;

            return actionID;
        }

        internal static bool SetPrimePotion(string config)
        {
            var pot = (uint)PluginConfiguration.GetCustomIntValue(config);

            if (pot > 0)
            {
                if (NumberOfHQPotions(pot) > 0)
                    PrimedPotion = pot + 1000000;
                else if (NumberOfNQPotions(pot) > 0)
                    PrimedPotion = pot;
                else
                    PrimedPotion = 0;

                if (PrimedPotion > 0)
                {
                    if (GetItemStatus(PrimedPotion) == 0)
                        return true;
                    else
                        PrimedPotion = 0;
                }
            }

            return false;
        }

        internal unsafe static int NumberOfNQPotions(uint potionID)
        {
            return InventoryManager.Instance()->GetInventoryItemCount(potionID, false);
        }

        internal unsafe static int NumberOfHQPotions(uint potionID)
        {
            return InventoryManager.Instance()->GetInventoryItemCount(potionID, true);
        }

        internal unsafe static int NumberOfItems(uint itemID)
        {
            return InventoryManager.Instance()->GetInventoryItemCount(itemID, true);
        }

        internal static unsafe bool UseItem(uint itemID) =>
    ActionManager.Instance() is not null &&
    (GetItemStatus(itemID + 1000000) is 0 && ActionManager.Instance()->UseAction(ActionType.Item, itemID + 1000000)) ||
    (GetItemStatus(itemID) is 0 && ActionManager.Instance()->UseAction(ActionType.Item, itemID));
        internal static unsafe uint GetItemStatus(uint itemID) =>
            ActionManager.Instance() is null ? uint.MaxValue : ActionManager.Instance()->GetActionStatus(ActionType.Item, itemID);

    }
}
