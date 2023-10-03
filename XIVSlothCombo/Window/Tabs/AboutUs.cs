using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Plugin;
using Dalamud.Utility;
using ECommons.ImGuiMethods;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Tabs
{
    internal class AboutUs : ConfigWindow
    {
        public static Version version = null!;

        private static Dictionary<string, IDalamudTextureWrap> Images = new();
        internal static new void Draw()
        {
            version ??= Assembly.GetExecutingAssembly().GetName().Version!;

            PvEFeatures.HasToOpenJob = true;

            ImGui.BeginChild("About", new Vector2(0, 0), true);

            ImGuiEx.ImGuiLineCentered("Header", delegate
            {
                ImGuiEx.TextUnderlined($"XIVSlothCombo - v{version}");
            });

            ImGuiEx.ImGuiLineCentered("AboutHeader", delegate
            {
                ImGuiEx.Text($"With ");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.SameLine(0, 0);
                ImGuiEx.Text(ImGuiColors.DalamudRed, FontAwesomeIcon.Heart.ToIconString());
                ImGui.PopFont();
                ImGui.SameLine(0, 0);
                ImGuiEx.Text($" from Team Sloth");
            });

            LoadAllImages();

            if (Images.Count == 12)
            {
                ImGuiEx.ImGuiLineCentered("AboutImage", delegate
                {
                    ImGui.Image(Images["teamsloth"].ImGuiHandle, new(310, 80));

                });
                ImGuiEx.ImGuiLineCentered("AboutTeamMembers", delegate
                {
                    ImGui.Image(Images["aki"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Aki");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["augporto"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Augporto");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["genesis"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Genesis-Nova");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["tartarga"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Tartarga");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["taurenkey"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Taurenkey");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["damo"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Damolitionn");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["ele"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Ele-Starshade");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["grimgal"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Grimgal");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.Image(Images["kkz"].ImGuiHandle, new(50, 50));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("k-kz");
                        ImGui.EndTooltip();
                    }
                });

                ImGuiHelpers.ScaledDummy(5f);

                ImGuiEx.ImGuiLineCentered("Others", delegate
                {
                    ImGuiEx.Text("Also with thanks to");
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 6);
                    var x = ImGui.GetCursorPosX();
                    var textSize = ImGui.CalcTextSize("our contributors!");
                    if (ImGui.InvisibleButton("contributors", new Vector2(textSize.X,textSize.Y)))
                    {
                        Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/graphs/contributors");
                    }
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(x);
                    ImGuiEx.TextUnderlined(ImGuiColors.TankBlue, "our contributors!");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }

                });
                ImGuiHelpers.ScaledDummy(10f);

                ImGuiEx.ImGuiLineCentered("DiscordButton", delegate
                {
                    if (IconButtons.IconImageButton(Images["discord"], "Click here to join our Discord Server!", new(0, 0), imageScale: 0.05f))
                    {
                        Util.OpenLink("https://discord.gg/xT7zyjzjtY");
                    }
                });
                ImGuiEx.ImGuiLineCentered("GitHubButton", delegate
                {
                    if (IconButtons.IconImageButton(Images["github"], "Got an issue? Click this button and report it!", new(0, 0), imageScale: 0.1f))
                    {
                        Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/issues");
                    }
                });

                ImGui.EndChild();
            }
            else
            {
                ImGuiEx.ImGuiLineCentered("Loading", delegate
                {
                    ImGuiEx.Text("Loading...");
                });

            }
        }

        private static void LoadAllImages()
        {
            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/team-sloth.png", out var texture))
                Images.TryAdd("teamsloth", texture);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/Aki.png", out var aki))
                Images.TryAdd("aki", aki);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/Augporto.jpg", out var augporto))
                Images.TryAdd("augporto", augporto);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/Genesis-Nova.png", out var genesis))
                Images.TryAdd("genesis", genesis);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/Tartarga.png", out var tartarga))
                Images.TryAdd("tartarga", tartarga);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/Taurenkey.png", out var taurenkey))
                Images.TryAdd("taurenkey", taurenkey);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/damolitionn.png", out var damo))
                Images.TryAdd("damo", damo);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/ele-starshade.png", out var ele))
                Images.TryAdd("ele", ele);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/grimgal.png", out var grimgal))
                Images.TryAdd("grimgal", grimgal);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/raw/main/res/team_sloth_images/Members/k-kz.png", out var kkz))
                Images.TryAdd("kkz", kkz);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://assets-global.website-files.com/6257adef93867e50d84d30e2/636e0a6cc3c481a15a141738_icon_clyde_white_RGB.png", out var discord))
                Images.TryAdd("discord", discord);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Taurenkey/XIVSlothCombo/raw/main/res/plugin/github-mark-white.png", out var github))
                Images.TryAdd("github", github);
        }
    }
}
