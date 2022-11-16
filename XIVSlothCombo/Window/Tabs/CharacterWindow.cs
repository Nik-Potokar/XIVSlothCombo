using System;
using System.Numerics;
using Dalamud.DrunkenToad;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json.Linq;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using static XIVSlothCombo.Window.Tabs.Debug;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.Window.Tabs
{
    internal class CharacterWindow : ConfigWindow, IDisposable
    {
        internal static new void Draw()
        {
            PlayerCharacter? LocalPlayer = Service.ClientState.LocalPlayer;
            
            if (LocalPlayer != null)
            { 
                if (Service.ClientState.LocalPlayer.TargetObject is BattleChara chara)
                {
                    foreach (Status? status in chara.StatusList)
                    {
                        ImGui.TextUnformatted($"TARGET STATUS CHECK: {chara.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                    }
                }
                
                var JobName = Service.ClientState.LocalPlayer.ClassJob.GameData.NameEnglish;
                var ShortJobName = Service.ClientState.LocalPlayer.ClassJob.GameData.Abbreviation;

                ImGui.BeginChild("menu", new Vector2(200,100f), true, ImGuiWindowFlags.NoBackground);
                ImGui.Indent(5);
                ImGui.TextColored(ImGuiColors.DalamudOrange, $"Welcome: {LocalPlayer.Name}! [{LocalPlayer.CompanyTag}]");
                ImGui.TextColored(ImGuiColors.DalamudWhite2, $"You're current Job is:"); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.DalamudWhite,$"{LocalPlayer.ClassJob.GameData.Abbreviation}");
                ImGui.Unindent(5);
                ImGui.Separator();
               
                ImGui.Indent(6); ImGui.TextColored(ImGuiColors.DPSRed, $"{LocalPlayer.CurrentHp} / {LocalPlayer.MaxHp} HP");
                ImGui.TextColored(ImGuiColors.ParsedPink, $"{LocalPlayer.CurrentMp} / {LocalPlayer.MaxMp} MP");
                ImGui.Unindent(6);
              //ImGui.TextColored(ImGuiColors.DalamudViolet, $"{LocalPlayer.CurrentCp}/{LocalPlayer.MaxCp} CP");
              //ImGui.TextColored(ImGuiColors.ParsedBlue, $"{LocalPlayer.CurrentGp}/{LocalPlayer.MaxGp} GP");
                ImGui.EndChild();

                ImGui.NewLine();
                
                if (ImGui.CollapsingHeader("Opener/Rotation Image buttons"))
                {
                    ImGui.BeginTable("Job images", 5, ImGuiTableFlags.Borders, new Vector2(0.0f, 0.0f), 0.0f);
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn(); // ======= Row 0
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextColored(ImGuiColors.TankBlue, "Tanks");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.HealerGreen, "Healers");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextColored(ImGuiColors.DPSRed, "Melee");
                    ImGui.TableSetColumnIndex(3);
                    ImGui.TextColored(ImGuiColors.DalamudOrange, "Ranged");
                    ImGui.TableSetColumnIndex(4);
                    ImGui.TextColored(ImGuiColors.DalamudViolet, "Caster");

                    ImGui.TableNextRow(); // ======= Row 1
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"PLD rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/htLjEzV.png");
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (ImGui.Button($"WHM rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/q8yMg5G.png");
                    }
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"MNK rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/UagNVtO.png");
                    }
                    ImGui.TableSetColumnIndex(3);
                    if (ImGui.Button($"BRD rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/s7n0Qst.png");
                    }
                    ImGui.TableSetColumnIndex(4);
                    if (ImGui.Button($"BLM rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/nLujxwj.png");
                    }

                    ImGui.TableNextRow(); // ======= Row 2
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"WAR rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/ghUafZa.png");
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (ImGui.Button($"SCH rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/G0LOyLT.png");
                    }
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"DRG rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/LnVbVbF.png");
                    }
                    ImGui.TableSetColumnIndex(3);
                    if (ImGui.Button($"MCH rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/jlG7nWf.png");
                    }
                    ImGui.TableSetColumnIndex(4);
                    if (ImGui.Button($"SUM rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/vRcwWiG.png");
                    }

                    ImGui.TableNextRow(); // ======= Row 3
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"DRK rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/2TmLJCA.png");
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (ImGui.Button($"AST rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/Ql16Eq1.jpg");
                    }
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"NIN rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/gS2xUP9.png");
                    }
                    ImGui.TableSetColumnIndex(3);
                    if (ImGui.Button($"DNC rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/aiax2BN.png");
                    }
                    ImGui.TableSetColumnIndex(4);
                    if (ImGui.Button($"RDM rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/tFo6b8w.png");
                    }

                    ImGui.TableNextRow(); // ======= Row 4
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"GNB rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/ynAltwE.png");
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (ImGui.Button($"SGE rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/9GgUzH4.png");
                    }
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"SAM rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/mOITlze.png");
                    }

                    ImGui.TableNextRow(); // ======= Row 5
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button($"RPR rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/zO3OVs7.png");
                    }
                    ImGui.EndTable();
                }

                ImGui.NewLine();

                ImGui.BeginTable("Color Previews", 2, ImGuiTableFlags.Resizable, new Vector2(0.0f, 0.0f), 0.0f);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.CollapsingHeader("Color Preview/Examples"))
                {
                    ImGui.BeginTable("Color Previews", 2, ImGuiTableFlags.Borders, new Vector2(0.0f, 0.0f), 0.0f);
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    ImGui.TableNextRow(0.0f);
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("Color Preview: Button");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text("Color Preview: Text");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.ColorButton("TankBlue", ImGuiColors.TankBlue);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.TankBlue, "Tank Blue");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.ColorButton("Healer Green", ImGuiColors.HealerGreen);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.HealerGreen, "Healer Green");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.ColorButton("DPS Red", ImGuiColors.DPSRed);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.DPSRed, "DPSRed");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.ColorButton("Dalamud Orange", ImGuiColors.DalamudOrange);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.DalamudOrange, "DalamudOrange");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.ColorButton("DalamudViolet", ImGuiColors.DalamudViolet);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextColored(ImGuiColors.DalamudViolet, "DalamudViolet");
                    ImGui.EndTable();
                }
                ImGui.EndTable();

                ImGui.Spacing();

                //if (ImGui.Button($"PLD rotation"))
                if (ImGui.Button("button1"))
                { 
                    ImGui.BeginPopupModal("popup1");
                    ImGui.ColorButton("Parsed Gold", ImGuiColors.ParsedGold);
                    ImGui.SameLine();
                    ImGui.ColorButton("Parsed Pink", ImGuiColors.ParsedPink);
                    ImGui.SameLine();
                    ImGui.ColorButton("Parsed Orange", ImGuiColors.ParsedOrange);
                    ImGui.SameLine();
                    ImGui.ColorButton("Parsed Purple", ImGuiColors.ParsedPurple);

                }

            }

            else
            {
                ImGui.TextUnformatted("Please log in to use this Plugin.");
            }
        }
    }
}