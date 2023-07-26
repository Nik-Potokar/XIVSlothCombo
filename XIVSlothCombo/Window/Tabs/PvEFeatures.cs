using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using XIVSlothCombo.Window.MessagesNS;

namespace XIVSlothCombo.Window.Tabs
{
    internal class PvEFeatures : ConfigWindow
    {
        //internal static Dictionary<string, bool> showHeader = new Dictionary<string, bool>();

        internal static List<float> allHeights = new();
        internal static bool HasToOpenJob = true;

        internal static new void Draw()
        {
#if !DEBUG
            if (Service.ClassLocked)
            {
                ImGui.Text("Equip your job stone to re-unlock features.");
                return;
            }
#endif

            ImGui.Text("This tab allows you to select which PvE combos and features you wish to enable.");
            ImGui.BeginChild("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            int i = 1;

            Positions.Clear();
            allHeights.Clear();
            foreach (string? jobName in groupedPresets.Keys)
            {
                string abbreviation = groupedPresets[jobName].First().Info.JobShorthand;
                string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                if (Positions.Count > 0)
                {
                    var currentPos = ImGui.GetCursorPos().Y;
                    var lastPos = Positions.Last().Value.Y;

                    allHeights.Add(currentPos - lastPos);
                }
                Positions[header] = ImGui.GetCursorPos();

                if (ImGui.CollapsingHeader($"{header}"))
                {
                    foreach (var otherJob in groupedPresets.Keys.Where(x => x != jobName))
                    {
                        string otherAbbreviation = groupedPresets[otherJob].First().Info.JobShorthand;
                        string otherHeader = string.IsNullOrEmpty(otherAbbreviation) ? otherJob : $"{otherJob} - {otherAbbreviation}";
                        ImGui.GetStateStorage().SetInt(ImGui.GetID(otherHeader), 0);
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

            OpenJobAutomatically();

            if (!string.IsNullOrEmpty(HeaderToOpen))
            {
                foreach (var job in groupedPresets.Keys)
                {
                    string otherAbbreviation = groupedPresets[job].First().Info.JobShorthand;
                    string otherHeader = string.IsNullOrEmpty(otherAbbreviation) ? job : $"{job} - {otherAbbreviation}";
                    ImGui.GetStateStorage().SetInt(ImGui.GetID(otherHeader), 0);
                }

                float headerPos = 0;
                float normalHeight = allHeights.OrderBy(x => x).First();
                foreach (var job in groupedPresets.Keys)
                {
                    string otherAbbreviation = groupedPresets[job].First().Info.JobShorthand;
                    string otherHeader = string.IsNullOrEmpty(otherAbbreviation) ? job : $"{job} - {otherAbbreviation}";
                    if (otherHeader != HeaderToOpen)
                        headerPos += normalHeight;
                    else
                        break;

                }

                if (headerPos > 0)
                {
                    ImGui.GetStateStorage().SetInt(ImGui.GetID(HeaderToOpen), 1);
                    ImGui.SetScrollY(headerPos);
                    HeaderToOpen = null;
                }
            }

            ImGui.EndChild();
        }


        private static void OpenJobAutomatically()
        {
            if (Service.Configuration.AutomaticallyOpenToCurrentJob && HasToOpenJob)
            {
                var id = Service.ClientState.LocalPlayer?.ClassJob?.Id;
                id = id switch
                {
                    ADV.ClassID => ADV.JobID,
                    BLM.ClassID => BLM.JobID,
                    BRD.ClassID => BRD.JobID,
                    DRG.ClassID => DRG.JobID,
                    MNK.ClassID => MNK.JobID,
                    NIN.ClassID => NIN.JobID,
                    PLD.ClassID => PLD.JobID,
                    SMN.ClassID => SMN.JobID,
                    WAR.ClassID => WAR.JobID,
                    WHM.ClassID => WHM.JobID,
                    _ => id,
                };

                if (id is >= 8 and <= 15)
                    id = DOH.JobID;

                if (id is >= 16 and <= 18)
                    id = DOL.JobID;

                if (id is not null)
                {
                    var currentJob = CustomComboInfoAttribute.JobIDToName((byte)id);

                    if (!string.IsNullOrEmpty(currentJob))
                    {
                        string abbreviation = groupedPresets[currentJob].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? currentJob : $"{currentJob} - {abbreviation}";

                        foreach (var job in groupedPresets.Keys)
                        {
                            string otherAbbreviation = groupedPresets[job].First().Info.JobShorthand;
                            string otherHeader = string.IsNullOrEmpty(otherAbbreviation) ? job : $"{job} - {otherAbbreviation}";
                            ImGui.GetStateStorage().SetInt(ImGui.GetID(otherHeader), 0);
                        }

                        float headerPos = 0;
                        float normalHeight = allHeights.OrderBy(x => x).First();
                        foreach (var job in groupedPresets.Keys)
                        {
                            string otherAbbreviation = groupedPresets[job].First().Info.JobShorthand;
                            string otherHeader = string.IsNullOrEmpty(otherAbbreviation) ? job : $"{job} - {otherAbbreviation}";
                            if (otherHeader != header)
                                headerPos += normalHeight;
                            else
                                break;

                        }

                        if (headerPos > 0)
                        {
                            ImGui.GetStateStorage().SetInt(ImGui.GetID(header), 1);
                            ImGui.SetScrollY(headerPos);
                            
                            if (ImGui.GetScrollY() == headerPos)
                            {
                                HasToOpenJob = false;
                            }
                        }
                    }
                }
            }
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

        internal static string? HeaderToOpen;

        internal static Dictionary<string, Vector2> Positions = new();
    }
}
