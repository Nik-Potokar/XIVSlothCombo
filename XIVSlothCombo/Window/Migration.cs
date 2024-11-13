#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.Reflection;
using ImGuiNET;

#endregion

namespace XIVSlothCombo.Window;

public class MigrationAutoInstallStatus
{
    public const string None = "";
    public const string Installing = "Being installed...";
    public const string Installed = "Installed Successfully";
    public const string Failed = "Installation Failed";
}

public class MigrationWindow : Dalamud.Interface.Windowing.Window
{
    /// <summary>
    ///     The repository URL for WrathCombo.
    /// </summary>
    private const string RepoURL = "https://love.puni.sh/ment.json";

    /// <summary>
    ///     The status of the automatic installation process.
    /// </summary>
    public static string AutomaticInstallStatus = MigrationAutoInstallStatus.None;

    /// <summary>
    ///     The task for the automatic installation process,
    ///     or null after its result is read to update <see cref="AutomaticInstallStatus" />
    ///     .
    /// </summary>
    private static Task? _automaticInstallTask;

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
        if (Svc.PluginInterface.InternalName == "XIVSlothCombo")
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

        if (!_wrathInstalled && AutomaticInstallStatus != MigrationAutoInstallStatus.Installed)
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

                #region "Automatic" Install

                // A larger padding for the big, easy button
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,
                    new Vector2(ImGui.CalcTextSize("W").X * 2,
                        ImGui.CalcTextSize("W").Y * 1.75f));

                if (ImGui.Button("\u002B Install WrathCombo for me"))
                {
                    _automaticInstallTask = DalamudReflector.AddPlugin(RepoURL, "WrathCombo");
                    AutomaticInstallStatus = MigrationAutoInstallStatus.Installing;
                }

                #region Installation Status

                if (_automaticInstallTask is { IsCompleted: true })
                {
                    // Get the result of the task
                    var success = ((Task<bool>)_automaticInstallTask).Result;

                    // Give the user feedback
                    if (success)
                    {
                        AutomaticInstallStatus = MigrationAutoInstallStatus.Installed;
                        Svc.Commands.ProcessCommand("/wrath"); // Open WrathCombo
                    }
                    else
                    {
                        AutomaticInstallStatus = MigrationAutoInstallStatus.Failed;
                    }

                    _automaticInstallTask = null; // Allow retrying
                }

                ImGui.Dummy(new Vector2(40, 0));
                ImGui.SameLine();
                if (AutomaticInstallStatus != MigrationAutoInstallStatus.Failed)
                    ImGui.Text(AutomaticInstallStatus);
                else
                    ImGui.TextColored(ImGuiColors.DalamudRed, AutomaticInstallStatus);

                #endregion

                ImGui.PopStyleVar();

                #endregion
            }

        #endregion

        if (AutomaticInstallStatus == MigrationAutoInstallStatus.Installed)
        {
            var successText = "WrathCombo has been installed successfully!";
            ImGuiHelpers.CenterCursorForText(successText);
            ImGui.TextColored(ImGuiColors.HealerGreen, successText);
        }
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
