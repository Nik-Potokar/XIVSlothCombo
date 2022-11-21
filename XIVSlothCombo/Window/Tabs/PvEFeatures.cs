using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using XIVSlothCombo.Window.MessagesNS;

namespace XIVSlothCombo.Window.Tabs
{
    internal class PvEFeatures : ConfigWindow
    {
        //internal static Dictionary<string, bool> showHeader = new Dictionary<string, bool>();

        internal static new void Draw()
        {
            if (Service.ClassLocked)
            {
                ImGui.Text("Equip your job stone to re-unlock features.");
                return;
            }

            ImGui.Text("This tab allows you to select which PvE combos and features you wish to enable.");
            ImGui.BeginChild("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            int i = 1;

            foreach (string? jobName in groupedPresets.Keys)
            {
                if (ImGui.CollapsingHeader(jobName))
                {
                    foreach (var otherJob in groupedPresets.Keys.Where(x => x != jobName))
                    {
                        ImGui.GetStateStorage().SetInt(ImGui.GetID(otherJob), 0);
                    }

                    if (ImGui.BeginTabBar($"subTab{jobName}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                    {
                        if (ImGui.BeginTabItem("Normal"))
                        {
                            DrawHeadingContents(jobName, i);
                            ImGui.EndTabItem();
                        }

                        if (groupedPresets[jobName].Any(x => PluginConfiguration.IsVariant(x.Preset)))
                        {
                            if (ImGui.BeginTabItem("Variant Dungeons"))
                            {
                                DrawVariantContents(jobName);
                                ImGui.EndTabItem();
                            }
                        }

                        if (groupedPresets[jobName].Any(x => PluginConfiguration.IsBozja(x.Preset)))
                        {
                            if (ImGui.BeginTabItem("Bozja"))
                            {
                                ImGui.EndTabItem();
                            }
                        }

                        if (groupedPresets[jobName].Any(x => PluginConfiguration.IsEureka(x.Preset)))
                        {
                            if (ImGui.BeginTabItem("Eureka"))
                            {
                                ImGui.EndTabItem();
                            }
                        }

                        ImGui.EndTabBar();
                    }
                }
                else
                {
                    i += groupedPresets[jobName].Where(x => !PluginConfiguration.IsSecret(x.Preset)).Count();
                    foreach (var preset in groupedPresets[jobName].Where(x => !PluginConfiguration.IsSecret(x.Preset)))
                    {
                        i += Presets.AllChildren(presetChildren[preset.Preset]);
                    }
                }
            }

            ImGui.PopStyleVar();
            ImGui.EndChild();
        }

        private static void DrawVariantContents(string jobName)
        {
            foreach (var (preset, info) in groupedPresets[jobName].Where(x => PluginConfiguration.IsVariant(x.Preset)))
            {
                int i = -1;
                InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info, ref i); } };
                presetBox.Draw();
                ImGuiHelpers.ScaledDummy(12.0f);
            }
        }

        internal static void DrawHeadingContents(string jobName, int i)
        {
            if (!Messages.PrintBLUMessage(jobName)) return;

            foreach (var (preset, info) in groupedPresets[jobName].Where(x =>   !PluginConfiguration.IsSecret(x.Preset) && 
                                                                                !PluginConfiguration.IsVariant(x.Preset) &&
                                                                                !PluginConfiguration.IsBozja(x.Preset) &&
                                                                                !PluginConfiguration.IsEureka(x.Preset)))
            {
                InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info, ref i); } };

                if (Service.Configuration.HideConflictedCombos)
                {
                    var conflictOriginals = Service.Configuration.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                    var conflictsSource = Service.Configuration.GetAllConflicts();      // Presets with the ConflictedAttribute

                    if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }

                    if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                    {
                        Service.Configuration.EnabledActions.Remove(preset);
                        Service.Configuration.Save();
                    }

                    else
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }
                }

                else
                {
                    presetBox.Draw();
                    ImGuiHelpers.ScaledDummy(12.0f);
                }
            }
        }
    }
}
