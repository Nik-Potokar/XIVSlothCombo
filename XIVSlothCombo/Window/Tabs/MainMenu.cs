using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using Dalamud.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using static XIVSlothCombo.Window.Tabs.Debug;
using XIVSlothCombo.Combos.PvP;
using Dalamud.Interface.Colors;

namespace XIVSlothCombo.Window.Tabs
{
    //chara.Name
    internal class MainMenu : ConfigWindow
    {   
        internal static new void Draw()
        { 
            PlayerCharacter? LocalPlayer = Service.ClientState.LocalPlayer;
            DebugCombo? comboClass = new();
            if (Service.ClientState.LocalPlayer is BattleChara chara)
            {
                ImGui.BeginChild("Menu", new Vector2(300, 100), true);
                ImGui.TextColored(ImGuiColors.DalamudOrange, $"Welcome,{chara.Name}");
                ImGui.TextColored(ImGuiColors.DPSRed, $"{chara.CurrentHp} HP");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.TankBlue, $"{chara.CurrentMp} MP");
                ImGui.EndChild();
                ImGui.Separator();
                
                ImGui.BeginTable("table1", 5, ImGuiTableFlags.Borders, new Vector2(200,100), 10.10f);
                    ImGui.TableNextRow();
                    if (ImGui.CollapsingHeader("kek xD Header"))
                    {
                        if (ImGui.Button($"Image of NIN rotation"))
                        {
                            Util.OpenLink("https://i.imgur.com/q3lXeSZ.png");
                        }
                        ImGui.Separator();
                        if (ImGui.Button($"Image of DRG rotation"))
                        {
                            Util.OpenLink("https://www.thebalanceffxiv.com/jobs/melee/dragoon/openers/");
                        }
                        ImGui.Separator();
                        if (ImGui.Button($"Image of RPR rotation"))
                        {
                            Util.OpenLink("https://www.thebalanceffxiv.com/jobs/melee/reaper/openers/");
                        }
                        ImGui.Separator();
                        if (ImGui.Button($"Image of RPR rotation"))
                        {
                            Util.OpenLink("https://www.thebalanceffxiv.com/jobs/melee/reaper/openers/");
                        }
                        ImGui.Separator();
                }
                ImGui.EndTable();

                if (ImGui.BeginTable("table2", 3, ImGuiTableFlags.Borders, new Vector2(600,250), 150.50f))
                {
                    for (int row = 0; row < 4; row++)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        if (ImGui.Button($"Image of NIN rotation"))
                        {
                            Util.OpenLink("https://i.imgur.com/q3lXeSZ.png");
                        }
                        ImGui.TableNextColumn();
                        ImGui.Text("Some contents");
                        ImGui.TableNextColumn();
                        ImGui.Text("123.456");
                    }
                    ImGui.EndTable();
                }
                
            }
            
        }
    }
}