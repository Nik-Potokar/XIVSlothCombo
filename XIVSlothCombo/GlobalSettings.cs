using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVSlothComboPlugin
{
    public class GlobalSettings : Window
    {
        public GlobalSettings()
            :base("Global Settings", ImGuiWindowFlags.ChildWindow)
        {
            this.RespectCloseHotkey = true;

            this.Size = new Vector2(200, 300);
            this.SizeCondition = ImGuiCond.FirstUseEver;


        }
        public override void Draw()
        {
            ImGui.Text("Hello World");
        }
    }
}
