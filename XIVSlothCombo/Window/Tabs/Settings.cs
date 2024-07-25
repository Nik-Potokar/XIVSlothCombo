using ImGuiNET;
using System;
using System.Numerics;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Tabs
{
    internal class Settings : ConfigWindow
    {
        internal static new void Draw()
        {
            PvEFeatures.HasToOpenJob = true;
            ImGui.BeginChild("main", new Vector2(0, 0), true);
            ImGui.Text("This tab allows you to customise your options when enabling features.");

            #region SubCombos

            bool hideChildren = Service.Configuration.HideChildren;

            if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
            {
                Service.Configuration.HideChildren = hideChildren;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Hides the sub-options of disabled features.");
                ImGui.EndTooltip();
            }
            ImGui.NextColumn();

            #endregion

            #region Conflicting

            bool hideConflicting = Service.Configuration.HideConflictedCombos;
            if (ImGui.Checkbox("Hide Conflicted Combos", ref hideConflicting))
            {
                Service.Configuration.HideConflictedCombos = hideConflicting;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Hides any combos that conflict with others you have selected.");
                ImGui.EndTooltip();
            }

            #endregion

            #region Combat Log

            bool showCombatLog = Service.Configuration.EnabledOutputLog;

            if (ImGui.Checkbox("Output Log to Chat", ref showCombatLog))
            {
                Service.Configuration.EnabledOutputLog = showCombatLog;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Every time you use an action, the plugin will print it to the chat.");
                ImGui.EndTooltip();
            }
            #endregion

            #region SpecialEvent

            bool isSpecialEvent = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
            bool slothIrl = isSpecialEvent && Service.Configuration.SpecialEvent;

            if (isSpecialEvent)

            {

                if (ImGui.Checkbox("Sloth Mode!?", ref slothIrl))
                {
                    Service.Configuration.SpecialEvent = slothIrl;
                    Service.Configuration.Save();
                }
            }

            else
            {
                Service.Configuration.SpecialEvent = false;
                Service.Configuration.Save();
            }

            float offset = (float)Service.Configuration.MeleeOffset;
            ImGui.PushItemWidth(75);

            bool inputChangedeth = false;
            inputChangedeth |= ImGui.InputFloat("Melee Distance Offset", ref offset);

            if (inputChangedeth)
            {
                Service.Configuration.MeleeOffset = (double)offset;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Offset of melee check distance for features that use it.\r\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.");
                ImGui.EndTooltip();
            }

            #endregion

            #region Message of the Day

            bool motd = Service.Configuration.HideMessageOfTheDay;

            if (ImGui.Checkbox("Hide Message of the Day", ref motd))
            {
                Service.Configuration.HideMessageOfTheDay = motd;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Disables the Message of the Day message in your chat when you login.");
                ImGui.EndTooltip();
            }
            ImGui.NextColumn();

            #endregion

            Vector4 colour = Service.Configuration.TargetHighlightColor;
            if (ImGui.ColorEdit4("Target Highlight Colour", ref colour, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
            {
                Service.Configuration.TargetHighlightColor = colour;
                Service.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"Used for {CustomComboInfoAttribute.JobIDToName(33)} card targeting features.\r\nSet Alpha to 0 to hide the box.");
                ImGui.EndTooltip();
            }

            ImGui.EndChild();
        }
    }
}
