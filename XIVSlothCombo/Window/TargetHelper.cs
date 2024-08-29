using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window;

internal class TargetHelper : Dalamud.Interface.Windowing.Window
{
    internal TargetHelper() : base("###SlothComboTargeteHelper", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize, true)
    {
        this.IsOpen = true;
        this.RespectCloseHotkey = false;
    }

    internal unsafe void DrawTargetHelper()
    {
        if (ASTHelper.AST_QuickTargetCards.SelectedRandomMember is not null)
        {
            IntPtr partyPTR = Svc.GameGui.GetAddonByName("_PartyList", 1);
            if (partyPTR == IntPtr.Zero)
                return;

            AddonPartyList plist = Marshal.PtrToStructure<AddonPartyList>(partyPTR);
            if (!plist.IsVisible) return;

            for (int i = 1; i <= 8; i++)
            {
                if (CustomComboFunctions.GetPartySlot(i) is null) continue;
                if (CustomComboFunctions.GetPartySlot(i).GameObjectId == ASTHelper.AST_QuickTargetCards.SelectedRandomMember.GameObjectId)
                {
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

    public override void Draw() => DrawTargetHelper();
}
