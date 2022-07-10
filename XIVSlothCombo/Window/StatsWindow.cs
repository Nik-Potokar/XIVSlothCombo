using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Window
{
    internal class StatsWindow : Dalamud.Interface.Windowing.Window, IDisposable
    {
        public StatsWindow() : base("Sloth Stats Window")
        {
            RespectCloseHotkey = true;
        }

        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public void Dispose()
        {
            
        }

        public override void Draw()
        {
            DrawStatWindow();
        }

        private void DrawStatWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(200, 200), ImGuiCond.FirstUseEver);

            if (ImGui.Begin(this.WindowName, ref visible, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize))
            {
                foreach (var action in ActionWatching.CombatActions.Distinct().OrderBy(x => CustomComboFunctions.GetActionName(x)))
                {
                    ImGui.Text($"{CustomComboFunctions.GetActionName(action)} x{ActionWatching.CombatActions.Where(x => x == action).Count()}");
                }
            }
        }
    }
}
