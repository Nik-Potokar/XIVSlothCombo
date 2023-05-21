using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Tabs;

namespace XIVSlothCombo.Window
{
    /// <summary> Plugin configuration window. </summary>
    internal class ConfigWindow : Dalamud.Interface.Windowing.Window, IDisposable
    {
        internal static readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets = GetGroupedPresets();
        internal static readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren = GetPresetChildren();

        internal static Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> GetGroupedPresets()
        {
            return Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && PluginConfiguration.GetParent(tpl.Preset) == null)
            .OrderByDescending(tpl => tpl.Info.JobID == 0)
            .ThenBy(tpl => tpl.Info.JobName)
            .ThenBy(tpl => tpl.Info.Order)
            .GroupBy(tpl => tpl.Info.JobName)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList())!;
        }

        internal static Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> GetPresetChildren()
        {
            var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
                tpl => tpl,
                tpl => new List<CustomComboPreset>());

            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
            {
                CustomComboPreset? parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
                if (parent != null)
                    childCombos[parent.Value].Add(preset);
            }

            return childCombos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
                    .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                    .OrderBy(tpl => tpl.Info.Order).ToArray())!;
        }

        private bool visible = false;
        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigWindow"/> class. </summary>
        public ConfigWindow() : base("XIVSlothCombo Configuration", ImGuiWindowFlags.AlwaysAutoResize)
        {
            RespectCloseHotkey = true;

            SizeCondition = ImGuiCond.FirstUseEver;
            Size = new Vector2(740, 490);
        }

        public override void Draw()
        {
            DrawConfig();
        }

        public void DrawConfig()
        {
            DrawTargetHelper();
            if (!Visible)
            {
                return;
            }

            if (ImGui.Begin("XIVSlothCombo Configuration", ref visible))
            {
                if (ImGui.BeginTabBar("Config Tabs"))
                {
                    if (ImGui.BeginTabItem("PvE Features"))
                    {
                        PvEFeatures.Draw();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("PvP Features"))
                    {
                        PvPFeatures.Draw();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Settings"))
                    {
                        Settings.Draw();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("About XIVSlothCombo / Report an Issue"))
                    {
                        AboutUs.Draw();
                        ImGui.EndTabItem();
                    }

#if DEBUG
                    if (ImGui.BeginTabItem("Debug Mode"))
                    {
                        Debug.Draw();
                        ImGui.EndTabItem();
                    }
#endif
                    ImGui.EndTabBar();
                }
            }
        }

        private unsafe void DrawTargetHelper()
        {
            if (AST.AST_QuickTargetCards.SelectedRandomMember is not null)
            {
                for (int i = 1; i <= 8; i++)
                {
                    if (CustomComboFunctions.GetPartySlot(i) == AST.AST_QuickTargetCards.SelectedRandomMember)
                    {
                        IntPtr partyPTR = Service.GameGui.GetAddonByName("_PartyList", 1);
                        if (partyPTR == IntPtr.Zero)
                            return;

                        AddonPartyList plist = Marshal.PtrToStructure<AddonPartyList>(partyPTR);

                        var member = i switch
                        {
                            1 => plist.PartyMember.PartyMember0.TargetGlow,
                            2 => plist.PartyMember.PartyMember1.TargetGlow,
                            3 => plist.PartyMember.PartyMember2.TargetGlow,
                            4 => plist.PartyMember.PartyMember3.TargetGlow,
                            5 => plist.PartyMember.PartyMember4.TargetGlow,
                            6 => plist.PartyMember.PartyMember5.TargetGlow,
                            7 => plist.PartyMember.PartyMember6.TargetGlow,
                            8 => plist.PartyMember.PartyMember7.TargetGlow,
                            _ => plist.PartyMember.PartyMember0.TargetGlow,
                        };

                        DrawOutline(member->AtkResNode.PrevSiblingNode);
                        
                    }
                }
            }
        }

        private unsafe void DrawOutline(AtkResNode* node)
        {
            var position = GetNodePosition(node);
            var scale = GetNodeScale(node);
            var size = new Vector2(node->Width, node->Height) * scale;

            position += ImGuiHelpers.MainViewport.Pos;

            var colour = Service.Configuration.TargetHighlightColor;
            ImGui.GetForegroundDrawList(ImGuiHelpers.MainViewport).AddRect(position, position + size, ImGui.GetColorU32(colour), 0, ImDrawFlags.RoundCornersAll, 2);
        }
        public unsafe Vector2 GetNodePosition(AtkResNode* node)
        {
            var pos = new Vector2(node->X, node->Y);
            var par = node->ParentNode;
            while (par != null)
            {
                pos *= new Vector2(par->ScaleX, par->ScaleY);
                pos += new Vector2(par->X, par->Y);
                par = par->ParentNode;
            }

            return pos;
        }

        public unsafe Vector2 GetNodeScale(AtkResNode* node)
        {
            if (node == null) return new Vector2(1, 1);
            var scale = new Vector2(node->ScaleX, node->ScaleY);
            while (node->ParentNode != null)
            {
                node = node->ParentNode;
                scale *= new Vector2(node->ScaleX, node->ScaleY);
            }

            return scale;
        }

        public void Dispose()
        {
            
        }
    }
}
