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
    internal class MainWindow : ConfigWindow
    {   
        internal static void DrawMenuWindow()
        { 
            PlayerCharacter? LocalPlayer = Service.ClientState.LocalPlayer;
            DebugCombo? comboClass = new();
            if (Service.ClientState.LocalPlayer is BattleChara chara)
            {
                ImGui.BeginChild("Menu", new Vector2(150, 75), true);
                ImGui.BeginGroup();
                ImGui.TextColored(ImGuiColors.DalamudOrange, $"Welcome, {chara.Name}");
                ImGui.Separator();
                ImGui.TextColored(ImGuiColors.DPSRed, $"{chara.CurrentHp} HP");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.TankBlue, $"{chara.CurrentMp} MP");
                ImGui.EndGroup();
                ImGui.EndChild();

                ImGui.Separator();

                ImGui.BeginTable("table1", 0, ImGuiTableFlags.Borders, new Vector2(0, 100), 0f);
                ImGui.TableHeader("tableheader1");
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"Image of NIN rotation"))
                {
                    Util.OpenLink("https://i.imgur.com/q3lXeSZ.png");
                }
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"Image of DRG rotation"))
                {
                    Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/drg/drg_ew_opener.png");
                }
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"Image of RPR rotation"))
                {
                    Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/rpr/earlyshroud.png");
                }
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"Image of MNK rotation"))
                {
                    Util.OpenLink("https://i.imgur.com/srvYnTD.png");
                }
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"Image of DNC rotation"))
                {
                    Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/dnc/dncopener.png");
                }
                ImGui.EndTable();

                ImGui.Separator();

                ImGui.BeginTable("table2", 2, ImGuiTableFlags.Resizable, new Vector2(0, 0), 0f);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.CollapsingHeader("CollapsingHeader1"))
                {   
                    ImGui.Button($"{LocalPlayer.Name}");
                }
                ImGui.EndTable();

                ImGui.Separator();

                ImGui.ColorButton("DPS Red", ImGuiColors.DPSRed);
                ImGui.SameLine();
                ImGui.ColorButton("Tank Blue", ImGuiColors.TankBlue);
                ImGui.SameLine();
                ImGui.ColorButton("Healer Green", ImGuiColors.HealerGreen);

                ImGui.Separator();

                ImGui.BeginTable("table_colors", 2);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.ColorButton("DPS Red", ImGuiColors.DPSRed); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.DPSRed, "DPSRed");
                ImGui.SameLine(); ImGui.ColorButton("Tank Blue", ImGuiColors.TankBlue); 
                ImGui.SameLine(); ImGui.ColorButton("Healer Green", ImGuiColors.HealerGreen);
                ImGui.SameLine();
                ImGui.ColorButton("Parsed Gold", ImGuiColors.ParsedGold); ImGui.SameLine(); ImGui.ColorButton("Parsed Pink", ImGuiColors.ParsedPink); ImGui.SameLine(); ImGui.ColorButton("Parsed Orange", ImGuiColors.ParsedOrange); ImGui.SameLine(); ImGui.ColorButton("Parsed Purple", ImGuiColors.ParsedPurple);
                ImGui.EndTable();
            }
        }
    }
}
                /*if (ImGui.BeginTable("table2", 3, ImGuiTableFlags.Borders, new Vector2(600,250), 150.50f))
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
                }*/