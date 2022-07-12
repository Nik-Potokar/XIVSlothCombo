using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
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
#if !DEBUG
            if (Service.ClassLocked)
            {
                ImGui.Text("Equip your job stone to re-unlock features.");
                return;
            }
#endif

            ImGui.Text("This tab allows you to select which PvE combos and features you wish to enable.");
            ImGui.BeginChild("scrolling", new Vector2(0, 0), true);

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

                    DrawHeadingContents(jobName, i);
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

        internal static void DrawHeadingContents(string jobName, int i)
        {
            if (!Messages.PrintBLUMessage(jobName)) return;

            foreach (var (preset, info) in groupedPresets[jobName].Where(x => !PluginConfiguration.IsSecret(x.Preset)))
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
