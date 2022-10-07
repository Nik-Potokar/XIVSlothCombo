using Dalamud.Interface.Colors;
using ImGuiNET;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.MessagesNS
{
    internal static class Messages
    {
        internal static bool PrintBLUMessage(string jobName)
        {
            if (jobName == Attributes.CustomComboInfoAttribute.JobIDToName(36)) //Blue Mage ID
            {
                if (Service.Configuration.ActiveBLUSpells.Count == 0)
                {
                    ImGui.Text("Please open the Blue Magic Spellbook to populate your active spells and enable features.");
                    return false;
                }

                else
                {
                    ImGui.TextColored(ImGuiColors.ParsedPink, $"Please note that even if you do not have all the required spells active, you may still use these features.\nAny spells you do not have active will be skipped over so if a feature is not working as intended then\nplease try and enable more required spells.");
                }
            }

            return true;
        }
    }
}
