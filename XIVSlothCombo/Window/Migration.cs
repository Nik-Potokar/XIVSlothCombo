#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.Reflection;
using ImGuiNET;

#endregion

namespace XIVSlothCombo.Window;

internal class MigrationWindow : Dalamud.Interface.Windowing.Window
{
    /// <summary>
    ///     The repository URL for WrathCombo.
    /// </summary>
    private const string RepoURL = "https://love.puni.sh/ment.json";

    /// <summary>
    ///     Whether WrathCombo is installed.
    /// </summary>
    private readonly bool _wrathInstalled;

    /// <summary>
    ///     Open the migration window.
    /// </summary>
    /// <remarks>
    ///     With a variety of flags and methods to bring it front and center;
    /// </remarks>
    public MigrationWindow() : base("XIVSlothCombo to WrathCombo Migration")
    {
        IsOpen = true;
        BringToFront();

        Flags = ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse
                | ImGuiWindowFlags.NoScrollbar;

        var wrathPlugins = Svc.PluginInterface.InstalledPlugins
            .Where(x => x.Name.ToLower().Replace(" ", "") == "wrathcombo")
            .ToArray();
        _wrathInstalled = wrathPlugins.Length > 0;
    }

    /// <summary>
    ///     Set up the large WindowPadding to center the content.
    /// </summary>
    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(60, 60));
    }

    /// <summary>
    ///     Draw the migration window.
    /// </summary>
    public override void Draw()
    {
        #region Leading Text

        CenterText("XIVSlothCombo is now WrathCombo!");
        CenterText(
            "Please follow the steps below to migrate to WrathCombo to continue receiving updates.");
        CenterDisabledText("(WrathCombo will automatically import your settings)");

        #endregion

        ImGui.Dummy(new Vector2(0, 30));

        #region Installation Steps

        if (!_wrathInstalled)
            using (ImRaii.Table("WrathMigrationSteps", 3))
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                #region "Manual" Install Step #1

                ImGui.Text("1. ");
                ImGui.SameLine();
                if (ImGui.Button("Click to automatically Add the New Repository"))
                {
                    DalamudReflector.AddRepo(RepoURL, true);
                    DalamudReflector.ReloadPluginMasters();
                }

                ImGui.Dummy(new Vector2(40, 0));
                ImGui.SameLine();
                ImGui.TextDisabled($"({RepoURL})");

                #endregion

                #region "Manual" Install Step #2

                ImGui.Text("2. ");
                ImGui.SameLine();
                if (ImGui.Button("Open the Plugin Installer"))
                    Svc.PluginInterface.OpenPluginInstallerTo(
                        PluginInstallerOpenKind.AllPlugins, "WrathCombo");
                ImGui.SameLine();
                ImGui.Text("and install WrathCombo");

                #endregion

                #region Separator

                ImGui.TableSetColumnIndex(1);
                ImGui.Dummy(new Vector2(0, ImGui.CalcTextSize("W").Y * 1.5f));
                ImGui.Dummy(new Vector2(30, 0));
                ImGui.SameLine();
                ImGui.Text("   OR");
                ImGui.SameLine();
                ImGui.Dummy(new Vector2(30, 0));
                ImGui.TableSetColumnIndex(2);

                #endregion

                // A larger padding for the big, easy button
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,
                    new Vector2(ImGui.CalcTextSize("W").X * 2,
                        ImGui.CalcTextSize("W").Y * 1.75f));

                if (ImGui.Button("\u002B Install WrathCombo for me"))
                {
                    // Add the repository if it doesn't exist
                    if (!DalamudReflector.HasRepo(RepoURL))
                    {
                        DalamudReflector.AddRepo(RepoURL, true);
                        DalamudReflector.ReloadPluginMasters();
                    }

                    //todo: use ECommons to install WrathCombo
                }

                ImGui.PopStyleVar();
            }

        #endregion

        if (!_wrathInstalled)
            ImGui.Dummy(new Vector2(0, 30));

        #region Final Step

        CenterButtonAndText("Open the Plugin Installer",
            "and disable then uninstall XIVSlothCombo",
            () => Svc.PluginInterface.OpenPluginInstallerTo(
                PluginInstallerOpenKind.InstalledPlugins, "XIVSlothCombo"));

        CenterDisabledText("XIVSlothCombo will not receive any further updates.");

        #endregion

        Center();
    }

    /// <summary>
    ///     Remove the large WindowPadding added in <see cref="PreDraw" />.
    /// </summary>
    public override void PostDraw()
    {
        ImGui.PopStyleVar(); // Remove the WindowPadding
    }

    #region Content Centering

    private static void CenterText(string text) => ImGuiHelpers.CenteredText(text);

    private static void CenterDisabledText(string text)
    {
        ImGuiHelpers.CenterCursorForText(text);
        ImGui.TextDisabled(text);
    }

    private static void CenterButtonAndText(string buttonText, string text,
        Action onClick)
    {
        var offset = (ImGui.GetContentRegionAvail().X
                      - ImGuiHelpers.GetButtonSize(buttonText).X
                      - ImGui.GetStyle().ItemSpacing.X
                      - ImGui.CalcTextSize(text).X)
                     / 2;

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offset);
        if (ImGui.Button(buttonText + "##" + buttonText + text)) onClick();
        ImGui.SameLine();
        ImGui.Text(text);
    }

    #endregion

    #region Window Centering

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);


    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left; // x position of upper-left corner
        public int Top; // y position of upper-left corner
        public int Right; // x position of lower-right corner
        public int Bottom; // y position of lower-right corner
        public Vector2 Position => new Vector2(Left, Top);
        public Vector2 Size => new Vector2(Right - Left, Bottom - Top);

        internal bool Contains(Vector2 v) => v.X > Position.X &&
                                             v.X < Position.X + Size.X &&
                                             v.Y > Position.Y &&
                                             v.Y < Position.Y + Size.Y;
    }

    /// <summary>
    ///     Centers the GUI window to the game window.
    /// </summary>
    private void Center()
    {
        // Get the pointer to the window handle.
        var hWnd = IntPtr.Zero;
        foreach (var pList in Process.GetProcesses())
            if (pList.ProcessName is "ffxiv_dx11" or "ffxiv")
                hWnd = pList.MainWindowHandle;

        // If failing to get the handle then abort.
        if (hWnd == IntPtr.Zero)
            return;

        // Get the game window rectangle
        GetWindowRect(new HandleRef(null, hWnd), out var rGameWindow);

        // Get the size of the current window.
        var vThisSize = ImGui.GetWindowSize();

        // Set the position.
        Position = rGameWindow.Position + new Vector2(
            rGameWindow.Size.X / 2 - vThisSize.X / 2,
            rGameWindow.Size.Y / 2 - vThisSize.Y / 2);
    }

    #endregion
}
