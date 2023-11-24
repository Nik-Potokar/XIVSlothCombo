using Dalamud.Game;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Combos.PvP;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window;
using XIVSlothCombo.Window.Tabs;
using ECommons;
using Dalamud.Plugin.Services;
using System.Reflection;
using ECommons.DalamudServices;

namespace XIVSlothCombo
{
    /// <summary> Main plugin implementation. </summary>
    public sealed partial class XIVSlothCombo : IDalamudPlugin
    {
        private const string Command = "/scombo";

        private readonly ConfigWindow configWindow;
        private HttpClient httpClient = new();
        
        private readonly TextPayload starterMotd = new("[Sloth Message of the Day] ");
        private static uint? jobID;

        public static uint? JobID
        {
            get => jobID;
            set
            {
                if (jobID != value && value != null)
                {
                    Service.PluginLog.Debug($"Switched to job {value}");
                    PvEFeatures.HasToOpenJob = true;
                }
                jobID = value;
            }
        }

        /// <summary> Initializes a new instance of the <see cref="XIVSlothCombo"/> class. </summary>
        /// <param name="pluginInterface"> Dalamud plugin interface. </param>
        public XIVSlothCombo(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            ECommonsMain.Init(pluginInterface, this);

            Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
            Service.Address = new PluginAddressResolver();
            Service.Address.Setup(Service.SigScanner);

            if (Service.Configuration.Version == 4)
                UpgradeConfig4();

            Service.ComboCache = new CustomComboCache();
            Service.IconReplacer = new IconReplacer();
            ActionWatching.Enable();
            Combos.JobHelpers.AST.Init();

            configWindow = new ConfigWindow(this);

            Service.Interface.UiBuilder.Draw += DrawUI;
            Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

            Service.CommandManager.AddHandler(Command, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true,
            });

            Service.ClientState.Login += PrintLoginMessage;
            if (Service.ClientState.IsLoggedIn) ResetFeatures();

            Service.Framework.Update += CheckCurrentJob;

            KillRedundantIDs();

#if DEBUG
            PvEFeatures.HasToOpenJob = false;
            configWindow.Visible = true;
#endif
        }

        private static void CheckCurrentJob(IFramework framework)
        {
            if (Service.ClientState.LocalPlayer is not null)
            JobID = Service.ClientState.LocalPlayer?.ClassJob?.Id;
        }
        private static void KillRedundantIDs()
        {
            List<int> redundantIDs = Service.Configuration.EnabledActions.Where(x => int.TryParse(x.ToString(), out _)).OrderBy(x => x).Cast<int>().ToList();
            foreach (int id in redundantIDs)
            {
                Service.Configuration.EnabledActions.RemoveWhere(x => (int)x == id);
            }

            Service.Configuration.Save();

        }

        private static void ResetFeatures()
        {
            // Enumerable.Range is a start and count, not a start and end.
            // Enumerable.Range(Start, Count)
            Service.Configuration.ResetFeatures("v3.0.17.0_NINRework", Enumerable.Range(10000, 100).ToArray());
            Service.Configuration.ResetFeatures("v3.0.17.0_DRGCleanup", Enumerable.Range(6100, 400).ToArray());
            Service.Configuration.ResetFeatures("v3.0.18.0_GNBCleanup", Enumerable.Range(7000, 700).ToArray());
            Service.Configuration.ResetFeatures("v3.0.18.0_PvPCleanup", Enumerable.Range(80000, 11000).ToArray());
            Service.Configuration.ResetFeatures("v3.0.18.1_PLDRework", Enumerable.Range(11000, 100).ToArray());
            Service.Configuration.ResetFeatures("v3.1.0.1_BLMRework", Enumerable.Range(2000, 100).ToArray());
            Service.Configuration.ResetFeatures("v3.1.1.0_DRGRework", Enumerable.Range(6000, 800).ToArray());
        }

        private void DrawUI() => configWindow.Draw();

        private void PrintLoginMessage()
        {
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(task => ResetFeatures());

            if (!Service.Configuration.HideMessageOfTheDay)
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(task => PrintMotD());
        }

        private void PrintMotD()
        {
            try
            {
                string basicMessage = $"Welcome to XIVSlothCombo v{this.GetType().Assembly.GetName().Version}!";
                using HttpResponseMessage? motd = httpClient.GetAsync("https://raw.githubusercontent.com/Nik-Potokar/XIVSlothCombo/main/res/motd.txt").Result;
                motd.EnsureSuccessStatusCode();
                string? data = motd.Content.ReadAsStringAsync().Result;
                List<Payload>? payloads = new()
                {
                    starterMotd,
                    EmphasisItalicPayload.ItalicsOn,
                    string.IsNullOrEmpty(data) ? new TextPayload(basicMessage) : new TextPayload(data.Trim()),
                    EmphasisItalicPayload.ItalicsOff
                };

                Service.ChatGui.Print(new XivChatEntry
                {
                    Message = new SeString(payloads),
                    Type = XivChatType.Echo
                });
            }

            catch (Exception ex)
            {
                Service.PluginLog.Error(ex, "Unable to retrieve MotD");
            }
        }

        /// <inheritdoc/>
        public string Name => "XIVSlothCombo";

        /// <inheritdoc/>
        public void Dispose()
        {
            configWindow?.Dispose();

            Service.CommandManager.RemoveHandler(Command);
            Service.Framework.Update -= CheckCurrentJob;
            Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
            Service.Interface.UiBuilder.Draw -= DrawUI;

            Service.IconReplacer?.Dispose();
            Service.ComboCache?.Dispose();
            ActionWatching.Dispose();
            Combos.JobHelpers.AST.Dispose();
            DisposeOpeners();

            Service.ClientState.Login -= PrintLoginMessage;
        }


        private void DisposeOpeners()
        {
            NIN.NIN_ST_SimpleMode.NINOpener.Dispose();
            NIN.NIN_ST_AdvancedMode.NINOpener.Dispose();
            NIN.NIN_ST_SimpleMode.NINOpener.Dispose();
            NIN.NIN_ST_AdvancedMode.NINOpener.Dispose();
        }
        private void OnOpenConfigUi() => configWindow.Visible = !configWindow.Visible;

        private void OnCommand(string command, string arguments)
        {
            string[]? argumentsParts = arguments.Split();

            switch (argumentsParts[0].ToLower())
            {
                case "unsetall": // unset all features
                    {
                        foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                        {
                            Service.Configuration.EnabledActions.Remove(preset);
                        }

                        Service.ChatGui.Print("All UNSET");
                        Service.Configuration.Save();
                        break;
                    }

                case "set": // set a feature
                    {
                        if (!Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                if (preset.ToString().ToLowerInvariant() != targetPreset)
                                    continue;

                                Service.Configuration.EnabledActions.Add(preset);
                                Service.ChatGui.Print($"{preset} SET");
                            }

                            Service.Configuration.Save();
                        }

                        else
                        {
                            Service.ChatGui.PrintError("Features cannot be set in combat.");
                        }

                        break;
                    }

                case "toggle": // toggle a feature
                    {
                        if (!Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
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
                        }

                        else
                        {
                            Service.ChatGui.PrintError("Features cannot be toggled in combat.");
                        }

                        break;
                    }

                case "unset": // unset a feature
                    {
                        if (!Service.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                if (preset.ToString().ToLowerInvariant() != targetPreset)
                                    continue;

                                Service.Configuration.EnabledActions.Remove(preset);
                                Service.ChatGui.Print($"{preset} UNSET");
                            }

                            Service.Configuration.Save();
                        }

                        else
                        {
                            Service.ChatGui.PrintError("Features cannot be unset in combat.");
                        }

                        break;
                    }

                case "list": // list features
                    {
                        string? filter = argumentsParts.Length > 1
                            ? argumentsParts[1].ToLowerInvariant()
                            : "all";

                        if (filter == "set") // list set features
                        {
                            foreach (bool preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => Service.Configuration.IsEnabled(preset)))
                            {
                                Service.ChatGui.Print(preset.ToString());
                            }
                        }

                        else if (filter == "unset") // list unset features
                        {
                            foreach (bool preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => !Service.Configuration.IsEnabled(preset)))
                            {
                                Service.ChatGui.Print(preset.ToString());
                            }
                        }

                        else if (filter == "all") // list all features
                        {
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
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

                case "enabled": // list all currently enabled features
                    {
                        foreach (CustomComboPreset preset in Service.Configuration.EnabledActions.OrderBy(x => x))
                        {
                            if (int.TryParse(preset.ToString(), out int pres)) continue;
                            Service.ChatGui.Print($"{(int)preset} - {preset}");
                        }

                        break;
                    }

                case "debug": // debug logging
                    {
                        try
                        {
                            string? specificJob = argumentsParts.Length == 2 ? argumentsParts[1].ToLower() : "";

                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                            using StreamWriter file = new($"{desktopPath}/SlothDebug.txt", append: false);  // Output path

                            file.WriteLine("START DEBUG LOG");
                            file.WriteLine("");
                            file.WriteLine($"Plugin Version: {GetType().Assembly.GetName().Version}");                          // Plugin version
                            file.WriteLine("");
                            file.WriteLine($"Installation Repo: {RepoCheckFunctions.FetchCurrentRepo()?.InstalledFromUrl}");    // Installation Repo
                            file.WriteLine("");
                            file.WriteLine($"Current Job: " +                                                                   // Current Job
                                $"{Service.ClientState.LocalPlayer.ClassJob.GameData.Name} / " +                                // - Client Name
                                $"{Service.ClientState.LocalPlayer.ClassJob.GameData.NameEnglish} / " +                         // - EN Name
                                $"{Service.ClientState.LocalPlayer.ClassJob.GameData.Abbreviation}");                           // - Abbreviation
                            file.WriteLine($"Current Job Index: {Service.ClientState.LocalPlayer.ClassJob.Id}");                // Job Index
                            file.WriteLine($"Current Job Level: {Service.ClientState.LocalPlayer.Level}");                      // Job Level
                            file.WriteLine("");
                            file.WriteLine($"Current Zone: {Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?.FirstOrDefault(x => x.RowId == Service.ClientState.TerritoryType).PlaceName.Value.Name}");   // Current zone location
                            file.WriteLine($"Current Party Size: {Service.PartyList.Length}");                                  // Current party size
                            file.WriteLine("");
                            file.WriteLine($"START ENABLED FEATURES");

                            int i = 0;
                            if (string.IsNullOrEmpty(specificJob))
                            {
                                foreach (CustomComboPreset preset in Service.Configuration.EnabledActions.OrderBy(x => x))
                                {
                                    if (int.TryParse(preset.ToString(), out _)) { i++; continue; }

                                    file.WriteLine($"{(int)preset} - {preset}");
                                }
                            }

                            else
                            {
                                foreach (CustomComboPreset preset in Service.Configuration.EnabledActions.OrderBy(x => x))
                                {
                                    if (int.TryParse(preset.ToString(), out _)) { i++; continue; }

                                    if (preset.ToString()[..3].ToLower() == specificJob ||  // Job identifier
                                        preset.ToString()[..3].ToLower() == "all" ||        // Adds in Globals
                                        preset.ToString()[..3].ToLower() == "pvp")          // Adds in PvP Globals
                                        file.WriteLine($"{(int)preset} - {preset}");
                                }
                            }


                            file.WriteLine($"END ENABLED FEATURES");
                            file.WriteLine("");

                            file.WriteLine("START CONFIG SETTINGS");
                            if (string.IsNullOrEmpty(specificJob))
                            {
                                file.WriteLine("---INT VALUES---");
                                foreach (var item in PluginConfiguration.CustomIntValues.OrderBy(x => x.Key))
                                {
                                    file.WriteLine($"{item.Key.Trim()} - {item.Value}");
                                }
                                file.WriteLine("");
                                file.WriteLine("---FLOAT VALUES---");
                                foreach (var item in PluginConfiguration.CustomFloatValues.OrderBy(x => x.Key))
                                {
                                    file.WriteLine($"{item.Key.Trim()} - {item.Value}");
                                }
                                file.WriteLine("");
                                file.WriteLine("---BOOL VALUES---");
                                foreach (var item in PluginConfiguration.CustomBoolValues.OrderBy(x => x.Key))
                                {
                                    file.WriteLine($"{item.Key.Trim()} - {item.Value}");
                                }
                                file.WriteLine("");
                                file.WriteLine("---BOOL ARRAY VALUES---");
                                foreach (var item in PluginConfiguration.CustomBoolArrayValues.OrderBy(x => x.Key))
                                {
                                    file.WriteLine($"{item.Key.Trim()} - {string.Join(", ", item.Value)}");
                                }
                            }
                            else
                            {
                                var jobname = ConfigWindow.groupedPresets.Where(x => x.Value.Any(y => y.Info.JobShorthand.Equals(specificJob.ToLower(), StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault().Key;
                                var jobID = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>()?
                                    .Where(x => x.Name.RawString.Equals(jobname, StringComparison.CurrentCultureIgnoreCase))
                                    .First()
                                    .RowId;

                                var whichConfig = jobID switch
                                {
                                    1 or 19 => typeof(PLD.Config),
                                    2 or 20 => typeof(MNK.Config),
                                    3 or 21 => typeof(WAR.Config),
                                    4 or 22 => typeof(DRG.Config),
                                    5 or 23 => typeof(BRD.Config),
                                    6 or 24 => typeof(WHM.Config),
                                    7 or 25 => typeof(BLM.Config),
                                    26 or 27 => typeof(SMN.Config),
                                    28 => typeof(SCH.Config),
                                    29 or 30 => typeof(NIN.Config),
                                    31 => typeof(MCH.Config),
                                    32 => typeof(DRK.Config),
                                    33 => typeof(AST.Config),
                                    34 => typeof(SAM.Config),
                                    35 => typeof(RDM.Config),
                                    //36 => typeof(BLU.Config),
                                    37 => typeof(GNB.Config),
                                    38 => typeof(DNC.Config),
                                    39 => typeof(RPR.Config),
                                    40 => typeof(SGE.Config),
                                    _ => throw new NotImplementedException(),
                                };

                                foreach (var config in whichConfig.GetMembers().Where(x => x.MemberType == System.Reflection.MemberTypes.Field || x.MemberType == System.Reflection.MemberTypes.Property))
                                {
                                    string key = config.Name!;

                                    if (PluginConfiguration.CustomIntValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomIntValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomFloatValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomFloatValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomBoolValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomBoolValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomBoolArrayValues.ContainsKey(key)) { file.WriteLine($"{key} - {string.Join(", ", PluginConfiguration.CustomBoolArrayValues[key])}"); continue; }

                                    file.WriteLine($"{key} - NOT SET");
                                }

                                foreach (var config in typeof(PvPCommon.Config).GetMembers().Where(x => x.MemberType == System.Reflection.MemberTypes.Field || x.MemberType == System.Reflection.MemberTypes.Property))
                                {
                                    string key = config.Name!;

                                    if (PluginConfiguration.CustomIntValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomIntValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomFloatValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomFloatValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomBoolValues.ContainsKey(key)) { file.WriteLine($"{key} - {PluginConfiguration.CustomBoolValues[key]}"); continue; }
                                    if (PluginConfiguration.CustomBoolArrayValues.ContainsKey(key)) { file.WriteLine($"{key} - {string.Join(", ", PluginConfiguration.CustomBoolArrayValues[key])}"); continue; }

                                    file.WriteLine($"{key} - NOT SET");
                                }
                            }


                            file.WriteLine("END CONFIG SETTINGS");
                            file.WriteLine("");
                            file.WriteLine($"Redundant IDs found: {i}");

                            if (i > 0)
                            {
                                file.WriteLine($"START REDUNDANT IDs");
                                foreach (CustomComboPreset preset in Service.Configuration.EnabledActions.Where(x => int.TryParse(x.ToString(), out _)).OrderBy(x => x))
                                {
                                    file.WriteLine($"{(int)preset}");
                                }

                                file.WriteLine($"END REDUNDANT IDs");
                                file.WriteLine("");
                            }

                            file.WriteLine($"Status Effect Count: {Service.ClientState.LocalPlayer.StatusList.Count(x => x != null)}");

                            if (Service.ClientState.LocalPlayer.StatusList.Length > 0)
                            {
                                file.WriteLine($"START STATUS EFFECTS");
                                foreach (Status? status in Service.ClientState.LocalPlayer.StatusList)
                                {
                                    file.WriteLine($"ID: {status.StatusId}, COUNT: {status.StackCount}, SOURCE: {status.SourceId} NAME: {ActionWatching.GetStatusName(status.StatusId)}");
                                }

                                file.WriteLine($"END STATUS EFFECTS");
                            }

                            file.WriteLine("END DEBUG LOG");
                            Service.ChatGui.Print("Please check your desktop for SlothDebug.txt and upload this file where requested.");

                            break;
                        }

                        catch (Exception ex)
                        {
                            Service.PluginLog.Error(ex, "Debug Log");
                            Service.ChatGui.Print("Unable to write Debug log.");
                            break;
                        }
                    }
                default:
                    configWindow.Visible = !configWindow.Visible;
                    PvEFeatures.HasToOpenJob = true;
                    if (argumentsParts[0].Length > 0)
                    {
                        var jobname = ConfigWindow.groupedPresets.Where(x => x.Value.Any(y => y.Info.JobShorthand.Equals(argumentsParts[0].ToLower(), StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault().Key;
                        var header = $"{jobname} - {argumentsParts[0].ToUpper()}";
                        Service.PluginLog.Debug($"{jobname}");
                        PvEFeatures.HeaderToOpen = header;
                    }
                    break;
            }

            Service.Configuration.Save();
        }

        private static void UpgradeConfig4()
        {
            Service.Configuration.Version = 5;
            Service.Configuration.EnabledActions = Service.Configuration.EnabledActions4
                .Select(preset => (int)preset switch
                    {
                        27 => 3301,
                        75 => 3302,
                        73 => 3303,
                        25 => 2501,
                        26 => 2502,
                        56 => 2503,
                        70 => 2504,
                        71 => 2505,
                        110 => 2506,
                        95 => 2507,
                        41 => 2301,
                        42 => 2302,
                        63 => 2303,
                        74 => 2304,
                        33 => 3801,
                        31 => 3802,
                        34 => 3803,
                        43 => 3804,
                        50 => 3805,
                        72 => 3806,
                        103 => 3807,
                        44 => 2201,
                        0 => 2202,
                        1 => 2203,
                        2 => 2204,
                        3 => 3201,
                        4 => 3202,
                        57 => 3203,
                        85 => 3204,
                        20 => 3701,
                        52 => 3702,
                        96 => 3703,
                        97 => 3704,
                        22 => 3705,
                        30 => 3706,
                        83 => 3707,
                        84 => 3708,
                        23 => 3101,
                        24 => 3102,
                        47 => 3103,
                        58 => 3104,
                        66 => 3105,
                        102 => 3106,
                        54 => 2001,
                        82 => 2002,
                        106 => 2003,
                        17 => 3001,
                        18 => 3002,
                        19 => 3003,
                        87 => 3004,
                        88 => 3005,
                        89 => 3006,
                        90 => 3007,
                        91 => 3008,
                        92 => 3009,
                        107 => 3010,
                        108 => 3011,
                        5 => 1901,
                        6 => 1902,
                        59 => 1903,
                        7 => 1904,
                        55 => 1905,
                        86 => 1906,
                        69 => 1907,
                        48 => 3501,
                        49 => 3502,
                        68 => 3503,
                        53 => 3504,
                        93 => 3505,
                        101 => 3506,
                        94 => 3507,
                        11 => 3401,
                        12 => 3402,
                        13 => 3403,
                        14 => 3404,
                        15 => 3405,
                        81 => 3406,
                        60 => 3407,
                        61 => 3408,
                        64 => 3409,
                        65 => 3410,
                        109 => 3411,
                        29 => 2801,
                        37 => 2802,
                        39 => 2701,
                        40 => 2702,
                        8 => 2101,
                        9 => 2102,
                        10 => 2103,
                        78 => 2104,
                        79 => 2105,
                        67 => 2106,
                        104 => 2107,
                        35 => 2401,
                        36 => 2402,
                        76 => 2403,
                        77 => 2404,
                        _ => 0,
                    })
                .Where(id => id != 0)
                .Select(id => (CustomComboPreset)id)
                .ToHashSet();
            Service.Configuration.EnabledActions4 = new();
            Service.Configuration.Save();
        }
    }
}