using Dalamud.Interface.Colors;
using ImGuiNET;

namespace XIVSlothCombo.Window.MessagesNS
{
    internal static class Messages
    {
        internal static bool PrintBLUMessage(string jobName)
        {
            if (jobName == Attributes.CustomComboInfoAttribute.JobIDToName(36)) //Blue Mage ID
            {
                ImGui.TextColored(ImGuiColors.ParsedPink, $"Please note that even if you do not have all the required spells active, you may still use these features.\nAny spells you do not have active will be skipped over so if a feature is not working as intended then\nplease try and enable more required spells.");
            }

            return true;
        }
    }
}
