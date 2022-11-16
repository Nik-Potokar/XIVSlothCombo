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

                ImGui.BeginChild("menu", new Vector2(550,100f), true);
                ImGui.TextColored(ImGuiColors.DalamudOrange, $"Welcome, {LocalPlayer.Name} [{LocalPlayer.CompanyTag}]");
                //ImGui.Text($"You're currently in {Service.DataManager.GetExcelSheet<World>}");
                ImGui.Separator();
                ImGui.TextColored(ImGuiColors.DPSRed, $"{LocalPlayer.CurrentHp}/"); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.DPSRed, $"{LocalPlayer.MaxHp} HP");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedPink, $"{LocalPlayer.CurrentMp}/"); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.ParsedPink, $"{LocalPlayer.MaxMp} MP");
                ImGui.TextColored(ImGuiColors.DalamudViolet, $"{LocalPlayer.CurrentCp}/"); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.DalamudViolet, $"{LocalPlayer.MaxCp} CP");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedBlue, $"{LocalPlayer.CurrentGp}/"); ImGui.SameLine(); ImGui.TextColored(ImGuiColors.ParsedBlue, $"{LocalPlayer.MaxGp} GP");
                //ImGui.TextColored(ImGuiColors.ParsedBlue, $"{LocalPlayer.HomeWorld}");
                //ImGui.TextColored(ImGuiColors.ParsedBlue, $"{LocalPlayer.CurrentWorld}");
                ImGui.EndChild();

                ImGui.Spacing();
                
                if (ImGui.CollapsingHeader("Opener/Rotation Image buttons"))
                {
                    ImGui.BeginTable("Job images", 5, ImGuiTableFlags.Borders, new Vector2(0.0f, 0.0f), 0.0f);
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("Tank");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text("Healer");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text("Melee");
                    ImGui.TableSetColumnIndex(3);
                    ImGui.Text("Ranged");
                    ImGui.TableSetColumnIndex(4);
                    ImGui.Text("Caster");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"NIN rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/q3lXeSZ.png");
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"DRG rotation"))
                    {
                        Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/drg/drg_ew_opener.png");
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"RPR rotation"))
                    {
                        Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/rpr/earlyshroud.png");
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"MNK rotation"))
                    {
                        Util.OpenLink("https://i.imgur.com/srvYnTD.png");
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.Button($"DNC rotation"))
                    {
                        Util.OpenLink("https://www.thebalanceffxiv.com/img/jobs/dnc/dncopener.png");
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

                /*
                ImGui.BeginChild("popup", new Vector2(0.0f, 0.0f), true, ImGuiWindowFlags.Popup);
                ImGui.ColorButton("Parsed Gold", ImGuiColors.ParsedGold);
                ImGui.SameLine();
                ImGui.ColorButton("Parsed Pink", ImGuiColors.ParsedPink);
                ImGui.SameLine();
                ImGui.ColorButton("Parsed Orange", ImGuiColors.ParsedOrange);
                ImGui.SameLine();
                ImGui.ColorButton("Parsed Purple", ImGuiColors.ParsedPurple);
                ImGui.EndChild();
                */
            }

            else
            {
                ImGui.TextUnformatted("Please log in to use this Plugin.");
            }
        }
    }
}