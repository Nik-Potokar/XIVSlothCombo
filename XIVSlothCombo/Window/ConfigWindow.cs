using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using XIVSlothCombo.Window.Tabs;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.Window
{
    /// <summary> Plugin configuration window. </summary>
    internal class ConfigWindow : Dalamud.Interface.Windowing.Window, IDisposable
    {
        internal static readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets = GetGroupedPresets();
        internal static readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren = GetPresetChildren();

        internal static Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> GetGroupedPresets()
        {
            return Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && PluginConfiguration.GetParent(tpl.Preset) == null)
            .OrderBy(tpl => tpl.Info.JobName)
            .ThenBy(tpl => tpl.Info.Order)
            .GroupBy(tpl => tpl.Info.JobName)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList());
        }

        internal static Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> GetPresetChildren()
        {
            var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
                tpl => tpl,
                tpl => new List<CustomComboPreset>());

            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
            {
                CustomComboPreset? parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
                if (parent != null)
                    childCombos[parent.Value].Add(preset);
            }

            return childCombos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
                    .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                    .OrderBy(tpl => tpl.Info.Order).ToArray());
        }

        private bool visible = false;
        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        /// <summary> Initializes a new instance of the <see cref="ConfigWindow"/> class. </summary>
        public ConfigWindow() : base("XIVSlothCombo Private Configuration", ImGuiWindowFlags.AlwaysAutoResize)
        {
            RespectCloseHotkey = true;

            SizeCondition = ImGuiCond.FirstUseEver;
            Size = new Vector2(740, 490);
        }

        public override void Draw()
        {
            DrawConfig();
        }
        
        public void DrawConfig()
        {
            if (!Visible) 
                return;
            try
            { 
                var fontScale = ImGui.GetIO().FontGlobalScale;
                var sizepluginwindow = new Vector2(450 * fontScale, 386 * fontScale);
              //var size = new Vector2(450 * fontScale, 300 * fontScale);

                ImGui.SetNextWindowSize(sizepluginwindow * 2, ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSizeConstraints(sizepluginwindow, sizepluginwindow * 20);

                if (ImGui.Begin("XIVSlothCombo Private Configuration", ref visible))
                {
                    if (ImGui.BeginTabBar("##tabar"))
                    {
                        if (ImGui.BeginTabItem("PvE Features"))
                        {
                            PvEFeatures.Draw();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("PvP Features"))
                        {
                            PvPFeatures.Draw();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Plugin Settings"))
                        {
                            Settings.Draw();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("About XIVSlothCombo"))
                        {
                            ImGui.BeginTabItem("ToggleAboutSloth");
                            AboutUs.Draw();
                            ImGui.EndTabItem();
                        } 
                        if (ImGui.BeginTabItem("Debug Mode"))
                        {
                            Debug.Draw();
                            ImGui.EndTabItem();
                        }
                    }
                    ImGui.EndTabBar();
                }
            }
            finally
            {
            ImGui.End();
            }
        }

        public void Dispose()
        {
            
        }

    }
}
