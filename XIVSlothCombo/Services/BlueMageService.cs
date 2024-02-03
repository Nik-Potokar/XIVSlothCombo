using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace XIVSlothCombo.Services
{
    internal unsafe static class BlueMageService
    {
        public static void PopulateBLUSpells()
        {
            var prevList = Service.Configuration.ActiveBLUSpells.ToList();
            Service.Configuration.ActiveBLUSpells.Clear();

            for (int i = 0; i <= 24; i++)
            {
                var id = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
                if (id != 0)
                    Service.Configuration.ActiveBLUSpells.Add(id);
            }

            if (Service.Configuration.ActiveBLUSpells.Except(prevList).Any())
                Service.Configuration.Save();
        }
    }
}
