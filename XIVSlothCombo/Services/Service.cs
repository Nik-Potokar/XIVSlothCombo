using System;
using System.IO;
using System.Reflection;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
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
        internal static BuddyList BuddyList { get; private set; } = null!;

        /// <summary> Gets the Dalamud chat gui. </summary>
        [PluginService]
        internal static ChatGui ChatGui { get; private set; } = null!;

        /// <summary> Facilitates class-based locking. </summary>
        internal static bool ClassLocked { get; set; } = true;

        /// <summary> Gets the Dalamud client state. </summary>
        [PluginService]
        internal static ClientState ClientState { get; private set; } = null!;

        /// <summary> Gets or sets the plugin caching mechanism. </summary>
        internal static CustomComboCache ComboCache { get; set; } = null!;

        /// <summary> Gets the Dalamud command manager. </summary>
        [PluginService]
        internal static CommandManager CommandManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud condition. </summary>
        [PluginService]
        internal static Condition Condition { get; private set; } = null!;

        /// <summary> Gets or sets the plugin configuration. </summary>
        internal static PluginConfiguration Configuration { get; set; } = null!;

        /// <summary> Gets the Dalamud data manager. </summary>
        [PluginService]
        internal static DataManager DataManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud framework manager. </summary>
        [PluginService]
        internal static Framework Framework { get; private set; } = null!;

        /// <summary> Handles the in-game UI. </summary>
        [PluginService]
        internal static GameGui GameGui { get; private set; } = null!;

        /// <summary> Gets or sets the plugin icon replacer. </summary>
        internal static IconReplacer IconReplacer { get; set; } = null!;

        /// <summary> Gets the Dalamud plugin interface. </summary>
        [PluginService]
        internal static DalamudPluginInterface Interface { get; private set; } = null!;

        /// <summary> Gets the Dalamud job gauges. </summary>
        [PluginService]
        internal static JobGauges JobGauges { get; private set; } = null!;

        /// <summary> Gets the Dalamud object table. </summary>
        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; } = null!;

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
        internal static PartyList PartyList { get; private set; } = null!;

        /// <summary> Facilitates searching for memory signatures. </summary>
        [PluginService]
        internal static SigScanner SigScanner { get; private set; } = null!;

        /// <summary> Gets the Dalamud target manager. </summary>
        [PluginService]
        internal static TargetManager TargetManager { get; private set; } = null!;
    }
}
