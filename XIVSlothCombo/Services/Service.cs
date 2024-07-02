using System;
using System.IO;
using System.Reflection;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Services
{
    /// <summary> Dalamud and plugin services. </summary>
    internal class Service
    {
        /// <summary> Gets or sets the plugin address resolver. </summary>
        internal static PluginAddressResolver Address { get; set; } = null!;

        /// <summary> Gets the Dalamud buddy list. </summary>
        [PluginService]
        internal static IBuddyList BuddyList { get; private set; } = null!;

        /// <summary> Gets the Dalamud chat gui. </summary>
        [PluginService]
        internal static IChatGui ChatGui { get; private set; } = null!;

        /// <summary> Gets the Dalamud client state. </summary>
        [PluginService]
        internal static IClientState ClientState { get; private set; } = null!;

        /// <summary> Gets or sets the plugin caching mechanism. </summary>
        internal static CustomComboCache ComboCache { get; set; } = null!;

        /// <summary> Gets the Dalamud command manager. </summary>
        [PluginService]
        internal static ICommandManager CommandManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud condition. </summary>
        [PluginService]
        internal static ICondition Condition { get; private set; } = null!;

        /// <summary> Gets or sets the plugin configuration. </summary>
        internal static PluginConfiguration Configuration { get; set; } = null!;

        /// <summary> Gets the Dalamud data manager. </summary>
        [PluginService]
        internal static IDataManager DataManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud framework manager. </summary>
        [PluginService]
        internal static IFramework Framework { get; private set; } = null!;

        /// <summary> Handles the in-game UI. </summary>
        [PluginService]
        internal static IGameGui GameGui { get; private set; } = null!;

        /// <summary> Gets or sets the plugin icon replacer. </summary>
        internal static IconReplacer IconReplacer { get; set; } = null!;

        /// <summary> Gets the Dalamud plugin interface. </summary>
        [PluginService]
        internal static IDalamudPluginInterface Interface { get; private set; } = null!;

        /// <summary> Gets the Dalamud job gauges. </summary>
        [PluginService]
        internal static IJobGauges JobGauges { get; private set; } = null!;

        /// <summary> Gets the Dalamud object table. </summary>
        [PluginService]
        internal static IObjectTable ObjectTable { get; private set; } = null!;

        /// <summary> Returns the Plugin Folder location </summary>
        public static string PluginFolder
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path)!;
            }
        }

        /// <summary> Gets the Dalamud party list. </summary>
        [PluginService]
        internal static IPartyList PartyList { get; private set; } = null!;

        /// <summary> Facilitates searching for memory signatures. </summary>
        [PluginService]
        internal static ISigScanner SigScanner { get; private set; } = null!;

        /// <summary> Gets the Dalamud target manager. </summary>
        [PluginService]
        internal static ITargetManager TargetManager { get; private set; } = null!;

        [PluginService]
        internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

        [PluginService]
        internal static IPluginLog PluginLog { get; private set; } = null!;
    }
}
