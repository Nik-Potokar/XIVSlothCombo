using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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
    internal class ConfigWindow : Dalamud.Interface.Windowing.Window
    {
        internal static readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets = GetGroupedPresets();
        internal static readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren = GetPresetChildren();

        internal static Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> GetGroupedPresets()
        {
            return Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && PresetStorage.GetParent(tpl.Preset) == null)
            .OrderByDescending(tpl => tpl.Info.JobID == 0)
            .ThenByDescending(tpl => tpl.Info.JobID == DOL.JobID)
            .ThenByDescending(tpl => tpl.Info.JobID == DOH.JobID)
            .ThenByDescending(tpl => tpl.Info.Role == 1)
            .ThenByDescending(tpl => tpl.Info.Role == 4)
            .ThenByDescending(tpl => tpl.Info.Role == 2)
            .ThenByDescending(tpl => tpl.Info.Role == 3)
            .ThenBy(tpl => tpl.Info.ClassJobCategory)
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

        public OpenWindow OpenWindow { get; set; } = OpenWindow.PvE;

        /// <summary> Initializes a new instance of the <see cref="ConfigWindow"/> class. </summary>
        public ConfigWindow() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###SlothCombo")
        {
            RespectCloseHotkey = true;

            SizeCondition = ImGuiCond.FirstUseEver;
            Size = new Vector2(800, 650);
            SetMinSize();

            Svc.PluginInterface.UiBuilder.DefaultFontHandle.ImFontChanged += SetMinSize;
        }

        private void SetMinSize(IFontHandle? fontHandle = null, ILockedImFont? lockedFont = null)
        {
            SizeConstraints = new()
            {
                MinimumSize = new Vector2(700f.Scale(), 10f.Scale())
            };
        }

        public override void Draw()
        {
            DrawTargetHelper();

            var region = ImGui.GetContentRegionAvail();
            var itemSpacing = ImGui.GetStyle().ItemSpacing;

            var topLeftSideHeight = region.Y;

            using (var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, new Vector2(4, 0)))
            {
                using (var table = ImRaii.Table("###MainTable", 2, ImGuiTableFlags.Resizable))
                {
                    if (!table)
                        return;


                    ImGui.TableSetupColumn("##LeftColumn", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 3);

                    ImGui.TableNextColumn();

                    var regionSize = ImGui.GetContentRegionAvail();

                    ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));

                    using (var leftChild = ImRaii.Child($"###SlothLeftSide", regionSize with { Y = topLeftSideHeight }, false, ImGuiWindowFlags.NoDecoration))
                    {
                        if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Taurenkey/XIVSlothCombo/blob/main/res/plugin/xivslothcombo.png?raw=true", out var logo))
                        {
                            ImGuiEx.LineCentered("###SlothLogo", () =>
                            {
                                ImGui.Image(logo.ImGuiHandle, new(125f.Scale(), 125f.Scale()));
                            });

                        }
                        ImGui.Spacing();
                        ImGui.Separator();

                        if (ImGui.Selectable("PvE Features", OpenWindow == OpenWindow.PvE))
                        {
                            OpenWindow = OpenWindow.PvE;
                        }
                        if (ImGui.Selectable("PvP Features", OpenWindow == OpenWindow.PvP))
                        {
                            OpenWindow = OpenWindow.PvP;
                        }
                        ImGui.Spacing();
                        if (ImGui.Selectable("Misc. Settings", OpenWindow == OpenWindow.Settings))
                        {
                            OpenWindow = OpenWindow.Settings;
                        }
                        ImGui.Spacing();
                        if (ImGui.Selectable("About", OpenWindow == OpenWindow.About))
                        {
                            OpenWindow = OpenWindow.About;
                        }

#if DEBUG
                        ImGui.Spacing();
                        if (ImGui.Selectable("DEBUG", OpenWindow == OpenWindow.Debug))
                        {
                            OpenWindow = OpenWindow.Debug;
                        }
                        ImGui.Spacing();
#endif

                    }

                    ImGui.PopStyleVar();
                    ImGui.TableNextColumn();
                    using (var rightChild = ImRaii.Child($"###SlothRightSide", Vector2.Zero, false))
                    {
                        switch (OpenWindow)
                        {
                            case OpenWindow.PvE:
                                PvEFeatures.Draw();
                                break;
                            case OpenWindow.PvP:
                                PvPFeatures.Draw();
                                break;
                            case OpenWindow.Settings:
                                Settings.Draw();
                                break;
                            case OpenWindow.About:
                                P.AboutUs.Draw();
                                break;
                            case OpenWindow.Debug:
                                Debug.Draw();
                                break;
                            default:
                                break;
                        };
                    }
                }
            }
        }

        private unsafe void DrawTargetHelper()
        {
            if (Combos.JobHelpers.AST.AST_QuickTargetCards.SelectedRandomMember is not null)
            {
                for (int i = 1; i <= 8; i++)
                {
                    if (CustomComboFunctions.GetPartySlot(i) == Combos.JobHelpers.AST.AST_QuickTargetCards.SelectedRandomMember)
                    {
                        IntPtr partyPTR = Service.GameGui.GetAddonByName("_PartyList", 1);
                        if (partyPTR == IntPtr.Zero)
                            return;

                        AddonPartyList plist = Marshal.PtrToStructure<AddonPartyList>(partyPTR);

                        var member = i switch
                        {
                            1 => plist.PartyMembers[0].TargetGlow,
                            2 => plist.PartyMembers[1].TargetGlow,
                            3 => plist.PartyMembers[2].TargetGlow,
                            4 => plist.PartyMembers[3].TargetGlow,
                            5 => plist.PartyMembers[4].TargetGlow,
                            6 => plist.PartyMembers[5].TargetGlow,
                            7 => plist.PartyMembers[6].TargetGlow,
                            8 => plist.PartyMembers[7].TargetGlow,
                            _ => plist.PartyMembers[0].TargetGlow,
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
            Svc.PluginInterface.UiBuilder.DefaultFontHandle.ImFontChanged -= SetMinSize;
        }
    }

    public enum OpenWindow
    {
        None = 0,
        PvE = 1,
        PvP = 2,
        Settings = 3,
        About = 4,
        Debug = 5,
    }
}
