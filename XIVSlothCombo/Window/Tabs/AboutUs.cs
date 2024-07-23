using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace XIVSlothCombo.Window.Tabs
{
    internal class AboutUs : ConfigWindow
    {
        public Version version = null!;

        private readonly Dictionary<string, IDalamudTextureWrap> Images = [];

        public AboutUs()
        {
            LoadAllImages();
        }

        public override void Draw()
        {
            try
            {
                version ??= Assembly.GetExecutingAssembly().GetName().Version!;

                PvEFeatures.HasToOpenJob = true;
                LoadAllImages();
                ImGuiEx.LineCentered("Header", delegate
                {
                    ImGuiEx.TextUnderlined($"XIVSlothCombo - v{version}");
                });

                ImGuiEx.LineCentered("AboutHeader", delegate
                {
                    ImGuiEx.Text($"With ");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.SameLine(0, 0);
                    ImGuiEx.Text(ImGuiColors.DalamudRed, FontAwesomeIcon.Heart.ToIconString());
                    ImGui.PopFont();
                    ImGui.SameLine(0, 0);
                    ImGuiEx.Text($" from Team Sloth");
                });

                if (Images.Count == 12)
                {
                    ImGuiEx.LineCentered("AboutImage", delegate
                    {
                        ImGui.Image(Images["teamsloth"].ImGuiHandle, new(310, 80));

                    });
                    ImGuiEx.LineCentered("AboutTeamMembers", delegate
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

                    ImGuiEx.LineCentered("Others", delegate
                    {
                        ImGuiEx.Text("Also with thanks to");
                        ImGui.SameLine();
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 6);
                        var x = ImGui.GetCursorPosX();
                        var textSize = ImGui.CalcTextSize("our contributors!");
                        if (ImGui.InvisibleButton("contributors", new Vector2(textSize.X, textSize.Y)))
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

                    ImGuiEx.LineCentered("DiscordButton", delegate
                    {
                        if (IconButtons.IconImageButton(Images["discord"], "Click here to join our Discord Server!", new(0, 0), imageScale: 0.05f))
                        {
                            Util.OpenLink("https://discord.gg/xT7zyjzjtY");
                        }
                    });
                    ImGuiEx.LineCentered("GitHubButton", delegate
                    {
                        if (IconButtons.IconImageButton(Images["github"], "Got an issue? Click this button and report it!", new(0, 0), imageScale: 0.1f))
                        {
                            Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/issues");
                        }
                    });
                }
                else
                {
                    ImGuiEx.LineCentered("Loading", delegate
                    {
                        ImGuiEx.Text($"Loading...");
                    });

                }
            }
            catch
            {

            }
        }

        private void LoadAllImages()
        {
            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/team-sloth.png?raw=true", out var texture))
                Images.TryAdd("teamsloth", texture);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/Aki.png?raw=true", out var aki))
                Images.TryAdd("aki", aki);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/Augporto.jpg?raw=true", out var augporto))
                Images.TryAdd("augporto", augporto);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/Genesis-Nova.png?raw=true", out var genesis))
                Images.TryAdd("genesis", genesis);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/Tartarga.png?raw=true", out var tartarga))
                Images.TryAdd("tartarga", tartarga);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/Taurenkey.png?raw=true", out var taurenkey))
                Images.TryAdd("taurenkey", taurenkey);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/damolitionn.png?raw=true", out var damo))
                Images.TryAdd("damo", damo);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/ele-starshade.png?raw=true", out var ele))
                Images.TryAdd("ele", ele);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/grimgal.png?raw=true", out var grimgal))
                Images.TryAdd("grimgal", grimgal);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/team_sloth_images/Members/k-kz.png?raw=true", out var kkz))
                Images.TryAdd("kkz", kkz);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://assets-global.website-files.com/6257adef93867e50d84d30e2/636e0a6cc3c481a15a141738_icon_clyde_white_RGB.png", out var discord))
                Images.TryAdd("discord", discord);

            if (ThreadLoadImageHandler.TryGetTextureWrap(@"https://github.com/Nik-Potokar/XIVSlothCombo/blob/main/res/plugin/github-mark-white.png?raw=true", out var github))
                Images.TryAdd("github", github);
        }
    }
}
