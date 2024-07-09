using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using XIVSlothCombo.Window.MessagesNS;

namespace XIVSlothCombo.Window.Tabs
{
    internal class PvEFeatures : ConfigWindow
    {
        //internal static Dictionary<string, bool> showHeader = new Dictionary<string, bool>();

        internal static bool HasToOpenJob = true;
        internal static string OpenJob = string.Empty;

        internal static new void Draw()
        {
            //#if !DEBUG
            if (IconReplacer.ClassLocked())
            {
                ImGui.TextWrapped("Equip your job stone to re-unlock features.");
                return;
            }
            //#endif

            using (var scrolling = ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
            {
                int i = 1;
                var indentwidth = 12f.Scale();
                var indentwidth2 = indentwidth + 42f.Scale();
                if (OpenJob == string.Empty)
                {
                    ImGui.SameLine(indentwidth);
                    ImGuiEx.LineCentered(() =>
                    {
                        ImGuiEx.TextUnderlined("Select a job from below to enable and configure features for it.");
                    });

                    foreach (string? jobName in groupedPresets.Keys)
                    {
                        string abbreviation = groupedPresets[jobName].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = groupedPresets[jobName].First().Info.JobID;
                        IDalamudTextureWrap? icon = Icons.GetJobIcon(id);
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVE.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenJob == jobName, ImGuiSelectableFlags.None, icon == null ? new Vector2(0) : new Vector2(0, (icon.Size.Y / 2f) * ImGui.GetIO().FontGlobalScale)))
                            {
                                OpenJob = jobName;
                            }
                            ImGui.SameLine(indentwidth);
                            if (icon != null)
                            {
                                ImGui.Image(icon.ImGuiHandle, (icon.Size / 2f) * ImGui.GetIO().FontGlobalScale);
                                ImGui.SameLine(indentwidth2);
                            }
                            ImGui.Text($"{header} {(disabled ? "(Disabled due to update)" : "")}");
                        }
                    }
                }
                else
                {
                    var id = groupedPresets[OpenJob].First().Info.JobID;
                    IDalamudTextureWrap? icon = Icons.GetJobIcon(id);

                    using (var headingTab = ImRaii.Child("HeadingTab", new Vector2(ImGui.GetContentRegionAvail().X, icon is null ? 24f.Scale() : (icon.Size.Y / icon.Size.Y / 2f * 1f.Scale()) + 4f)))
                    {
                        if (ImGui.Button("Back"))
                        {
                            OpenJob = "";
                            return;
                        }
                        ImGui.SameLine();
                        ImGuiEx.LineCentered(() =>
                        {
                            if (icon != null)
                            {
                                ImGui.Image(icon.ImGuiHandle, (icon.Size / icon.Size.Y / 2f * 1f.Scale()));
                                ImGui.SameLine();
                            }
                            ImGuiEx.Text($"{OpenJob}");
                        });

                    }

                    using (var contents = ImRaii.Child("Contents", new Vector2(0)))
                    {

                        try
                        {
                            if (ImGui.BeginTabBar($"subTab{OpenJob}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                            {
                                if (ImGui.BeginTabItem("Normal"))
                                {
                                    DrawHeadingContents(OpenJob, i);
                                    ImGui.EndTabItem();
                                }

                                if (groupedPresets[OpenJob].Any(x => PresetStorage.IsVariant(x.Preset)))
                                {
                                    if (ImGui.BeginTabItem("Variant Dungeons"))
                                    {
                                        DrawVariantContents(OpenJob);
                                        ImGui.EndTabItem();
                                    }
                                }

                                if (groupedPresets[OpenJob].Any(x => PresetStorage.IsBozja(x.Preset)))
                                {
                                    if (ImGui.BeginTabItem("Bozja"))
                                    {
                                        ImGui.EndTabItem();
                                    }
                                }

                                if (groupedPresets[OpenJob].Any(x => PresetStorage.IsEureka(x.Preset)))
                                {
                                    if (ImGui.BeginTabItem("Eureka"))
                                    {
                                        ImGui.EndTabItem();
                                    }
                                }

                                ImGui.EndTabBar();
                            }
                        }
                        catch { }

                    }
                }

            }
        }

        private static void DrawVariantContents(string jobName)
        {
            foreach (var (preset, info) in groupedPresets[jobName].Where(x => PresetStorage.IsVariant(x.Preset)))
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

            foreach (var (preset, info) in groupedPresets[jobName].Where(x => !PresetStorage.IsPvP(x.Preset) &&
                                                                                !PresetStorage.IsVariant(x.Preset) &&
                                                                                !PresetStorage.IsBozja(x.Preset) &&
                                                                                !PresetStorage.IsEureka(x.Preset)))
            {
                InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 2f.Scale(), ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info, ref i); } };

                if (Service.Configuration.HideConflictedCombos)
                {
                    var conflictOriginals = PresetStorage.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                    var conflictsSource = PresetStorage.GetAllConflicts();      // Presets with the ConflictedAttribute

                    if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }

                    if (conflictOriginals.Any(x => PresetStorage.IsEnabled(x)))
                    {
                        Service.Configuration.EnabledActions.Remove(preset);
                        Service.Configuration.Save();
                    }

                    else
                    {
                        presetBox.Draw();
                        
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

        internal static string? HeaderToOpen;
    }
}
