using System;
using System.Linq;

using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Main plugin implementation.
    /// </summary>
    public sealed partial class XIVComboWayTooAkiiified : IDalamudPlugin
    {
        private const string Command = "/pcombo";

        private readonly WindowSystem windowSystem;
        private readonly ConfigWindow configWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="XIVComboWayTooAkiiified"/> class.
        /// </summary>
        /// <param name="pluginInterface">Dalamud plugin interface.</param>
        public XIVComboWayTooAkiiified(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();

            Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
            Service.Address = new PluginAddressResolver();
            Service.Address.Setup();

            Service.IconReplacer = new IconReplacer();

            this.configWindow = new();
            this.windowSystem = new("XIVComboWayTooAkified");
            this.windowSystem.AddWindow(this.configWindow);

            Service.Interface.UiBuilder.OpenConfigUi += this.OnOpenConfigUi;
            Service.Interface.UiBuilder.Draw += this.windowSystem.Draw;

            Service.CommandManager.AddHandler(Command, new CommandInfo(this.OnCommand)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true,
            });
        }

        /// <inheritdoc/>
        public string Name => "XIVComboWayTooAkified";

        /// <inheritdoc/>
        public void Dispose()
        {
            Service.CommandManager.RemoveHandler(Command);

            Service.Interface.UiBuilder.OpenConfigUi -= this.OnOpenConfigUi;
            Service.Interface.UiBuilder.Draw -= this.windowSystem.Draw;

            Service.IconReplacer.Dispose();
        }

        private void OnOpenConfigUi()
            => this.configWindow.IsOpen = true;

        private void OnCommand(string command, string arguments)
        {
            var argumentsParts = arguments.Split();

            switch (argumentsParts[0])
            {
                case "setall":
                    {
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            Service.Configuration.EnabledActions.Add(preset);
                        }

                        Service.ChatGui.Print("All SET");
                        Service.Configuration.Save();
                        break;
                    }

                case "unsetall":
                    {
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            Service.Configuration.EnabledActions.Remove(preset);
                        }

                        Service.ChatGui.Print("All UNSET");
                        Service.Configuration.Save();
                        break;
                    }

                case "set":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            Service.Configuration.EnabledActions.Add(preset);
                            Service.ChatGui.Print($"{preset} SET");
                        }

                        Service.Configuration.Save();
                        break;
                    }

                case "secrets":
                    {
                        Service.Configuration.EnableSecretCombos = !Service.Configuration.EnableSecretCombos;

                        Service.ChatGui.Print(Service.Configuration.EnableSecretCombos
                            ? $"Secret combos are now shown"
                            : $"Secret combos are now hidden");

                        Service.Configuration.Save();
                        break;
                    }

                case "toggle":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            if (Service.Configuration.EnabledActions.Contains(preset))
                            {
                                Service.Configuration.EnabledActions.Remove(preset);
                                Service.ChatGui.Print($"{preset} UNSET");
                            }
                            else
                            {
                                Service.Configuration.EnabledActions.Add(preset);
                                Service.ChatGui.Print($"{preset} SET");
                            }
                        }

                        Service.Configuration.Save();
                        break;
                    }

                case "unset":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            Service.Configuration.EnabledActions.Remove(preset);
                            Service.ChatGui.Print($"{preset} UNSET");
                        }

                        Service.Configuration.Save();
                        break;
                    }

                case "list":
                    {
                        var filter = argumentsParts.Length > 1
                            ? argumentsParts[1].ToLowerInvariant()
                            : "all";

                        if (filter == "set")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => Service.Configuration.IsEnabled(preset)))
                            {
                                Service.ChatGui.Print(preset.ToString());
                            }
                        }
                        else if (filter == "unset")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => !Service.Configuration.IsEnabled(preset)))
                            {
                                Service.ChatGui.Print(preset.ToString());
                            }
                        }
                        else if (filter == "all")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>())
                            {
                                Service.ChatGui.Print(preset.ToString());
                            }
                        }
                        else
                        {
                            Service.ChatGui.PrintError("Available list filters: set, unset, all");
                        }

                        break;
                    }

                default:
                    this.configWindow.Toggle();
                    break;
            }

            Service.Configuration.Save();
        }
    }
}