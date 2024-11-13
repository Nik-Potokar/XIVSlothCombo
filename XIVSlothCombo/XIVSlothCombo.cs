using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Combos.PvP;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window;
using XIVSlothCombo.Window.Tabs;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo
{
    /// <summary> Main plugin implementation. </summary>
    public sealed partial class XIVSlothCombo : IDalamudPlugin
    {
        private const string Command = "/scombo";

        private readonly ConfigWindow ConfigWindow;
        private readonly TargetHelper TargetHelper;
        internal readonly AboutUs AboutUs;
        internal static XIVSlothCombo? P = null!;
        internal WindowSystem ws;
        private readonly HttpClient httpClient = new();

        private readonly TextPayload starterMotd = new("[Sloth Message of the Day] ");
        private static uint? jobID;

        public static readonly List<uint> DisabledJobsPVE =
        [
            //ADV.JobID,
            //AST.JobID,
            //BLM.JobID,
            //BLU.JobID,
            //BRD.JobID,
            //DNC.JobID,
            //DOL.JobID,
            //DRG.JobID,
            //DRK.JobID,
            //GNB.JobID,
            //MCH.JobID,
            //MNK.JobID,
            //NIN.JobID,
            //PCT.JobID,
            //PLD.JobID,
            //RDM.JobID,
            //RPR.JobID,
            //SAM.JobID,
            //SCH.JobID,
            //SGE.JobID,
            //SMN.JobID,
            //VPR.JobID,
            //WAR.JobID,
            //WHM.JobID
        ];

        public static readonly List<uint> DisabledJobsPVP = [];

        public static uint? JobID
        {
            get => jobID;
            set
            {
                if (jobID != value && value != null)
                {
                    Combos.JobHelpers.AST.AST_QuickTargetCards.SelectedRandomMember = null;
                    Svc.Log.Debug($"Switched to job {value}");
                    PvEFeatures.HasToOpenJob = true;
                }
                jobID = value;
            }
        }

        /// <summary> Initializes a new instance of the <see cref="XIVSlothCombo"/> class. </summary>
        /// <param name="pluginInterface"> Dalamud plugin interface. </param>
        public XIVSlothCombo(IDalamudPluginInterface pluginInterface)
        {
            P = this;
            pluginInterface.Create<Service>();
            ECommonsMain.Init(pluginInterface, this);
            
            Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
            Service.Address = new PluginAddressResolver();
            Service.Address.Setup(Svc.SigScanner);
            PresetStorage.Init();

            Service.ComboCache = new CustomComboCache();
            Service.IconReplacer = new IconReplacer();
            ActionWatching.Enable();
            Combos.JobHelpers.AST.Init();

            ConfigWindow = new ConfigWindow();
            TargetHelper = new();
            AboutUs = new();
            ws = new();
            ws.AddWindow(ConfigWindow);
            ws.AddWindow(TargetHelper);

            Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
            Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

            Svc.Commands.AddHandler(Command, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true,
            });

            Svc.ClientState.Login += PrintLoginMessage;
            if (Svc.ClientState.IsLoggedIn) ResetFeatures();

            Svc.Framework.Update += OnFrameworkUpdate;

            KillRedundantIDs();
            HandleConflictedCombos();

#if DEBUG
            ConfigWindow.IsOpen = true;
#endif
        }

        private static void HandleConflictedCombos()
        {
            var enabledCopy = Service.Configuration.EnabledActions.ToHashSet(); //Prevents issues later removing during enumeration
            foreach (var preset in enabledCopy)
            {
                if (!PresetStorage.IsEnabled(preset)) continue;

                var conflictingCombos = preset.GetAttribute<ConflictingCombosAttribute>();
                if (conflictingCombos != null)
                {
                    foreach (var conflict in conflictingCombos.ConflictingPresets)
                    {
                        if (PresetStorage.IsEnabled(conflict))
                        {
                            Service.Configuration.EnabledActions.Remove(conflict);
                            Service.Configuration.Save();
                        }
                    }
                }
            }
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            if (Svc.ClientState.LocalPlayer is not null)
                JobID = Svc.ClientState.LocalPlayer?.ClassJob.RowId;

            BlueMageService.PopulateBLUSpells();
            TargetHelper.Draw();
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

        private void DrawUI() => ConfigWindow.Draw();

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
                List<Payload>? payloads =
                [
                    starterMotd,
                    EmphasisItalicPayload.ItalicsOn,
                    string.IsNullOrEmpty(data) ? new TextPayload(basicMessage) : new TextPayload(data.Trim()),
                    EmphasisItalicPayload.ItalicsOff
                ];

                Svc.Chat.Print(new XivChatEntry
                {
                    Message = new SeString(payloads),
                    Type = XivChatType.Echo
                });
            }

            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Unable to retrieve MotD");
            }
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used for non-static only window initialization")]
        public string Name => "XIVSlothCombo";

        /// <inheritdoc/>
        public void Dispose()
        {
            ConfigWindow?.Dispose();

            ws.RemoveAllWindows();
            Svc.Commands.RemoveHandler(Command);
            Svc.Framework.Update -= OnFrameworkUpdate;
            Svc.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
            Svc.PluginInterface.UiBuilder.Draw -= DrawUI;

            Service.IconReplacer?.Dispose();
            Service.ComboCache?.Dispose();
            ActionWatching.Dispose();
            Combos.JobHelpers.AST.Dispose();
            DisposeOpeners();

            Svc.ClientState.Login -= PrintLoginMessage;
            P = null;
        }


        private static void DisposeOpeners()
        {
            NIN.NIN_ST_SimpleMode.NINOpener.Dispose();
            NIN.NIN_ST_AdvancedMode.NINOpener.Dispose();
        }
        private void OnOpenConfigUi() => ConfigWindow.IsOpen = !ConfigWindow.IsOpen;

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

                        Svc.Chat.Print("All UNSET");
                        Service.Configuration.Save();
                        break;
                    }

                case "set": // set a feature
                    {
                        if (!Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                if (!preset.ToString().Equals(targetPreset, StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                Service.Configuration.EnabledActions.Add(preset);
                                Svc.Chat.Print($"{preset} SET");
                            }

                            Service.Configuration.Save();
                        }

                        else
                        {
                            Svc.Chat.PrintError("Features cannot be set in combat.");
                        }

                        break;
                    }

                case "toggle": // toggle a feature
                    {
                        if (!Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                if (!preset.ToString().Equals(targetPreset, StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                if (!Service.Configuration.EnabledActions.Remove(preset))
                                {
                                    Service.Configuration.EnabledActions.Add(preset);
                                    Svc.Chat.Print($"{preset} SET");
                                }
                                else
                                {
                                    Svc.Chat.Print($"{preset} UNSET");
                                }
                            }

                            Service.Configuration.Save();
                        }

                        else
                        {
                            Svc.Chat.PrintError("Features cannot be toggled in combat.");
                        }

                        break;
                    }

                case "unset": // unset a feature
                    {
                        if (!Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                        {
                            string? targetPreset = argumentsParts[1].ToLowerInvariant();
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                if (!preset.ToString().Equals(targetPreset, StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                Service.Configuration.EnabledActions.Remove(preset);
                                Svc.Chat.Print($"{preset} UNSET");
                            }

                            Service.Configuration.Save();
                        }

                        else
                        {
                            Svc.Chat.PrintError("Features cannot be unset in combat.");
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
                                .Select(preset => PresetStorage.IsEnabled(preset)))
                            {
                                Svc.Chat.Print(preset.ToString());
                            }
                        }

                        else if (filter == "unset") // list unset features
                        {
                            foreach (bool preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => !PresetStorage.IsEnabled(preset)))
                            {
                                Svc.Chat.Print(preset.ToString());
                            }
                        }

                        else if (filter == "all") // list all features
                        {
                            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
                            {
                                Svc.Chat.Print(preset.ToString());
                            }
                        }

                        else
                        {
                            Svc.Chat.PrintError("Available list filters: set, unset, all");
                        }

                        break;
                    }

                case "enabled": // list all currently enabled features
                    {
                        foreach (CustomComboPreset preset in Service.Configuration.EnabledActions.OrderBy(x => x))
                        {
                            if (int.TryParse(preset.ToString(), out int pres)) continue;
                            Svc.Chat.Print($"{(int)preset} - {preset}");
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
                                $"{Svc.ClientState.LocalPlayer.ClassJob.Value.Name} / " +                                // - Client Name
                                $"{Svc.ClientState.LocalPlayer.ClassJob.Value.NameEnglish} / " +                         // - EN Name
                                $"{Svc.ClientState.LocalPlayer.ClassJob.Value.Abbreviation}");                           // - Abbreviation
                            file.WriteLine($"Current Job Index: {Svc.ClientState.LocalPlayer.ClassJob.RowId}");                // Job Index
                            file.WriteLine($"Current Job Level: {Svc.ClientState.LocalPlayer.Level}");                      // Job Level
                            file.WriteLine("");
                            file.WriteLine($"Current Zone: {Svc.Data.GetExcelSheet<TerritoryType>()?.FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name}");   // Current zone location
                            file.WriteLine($"Current Party Size: {Svc.Party.Length}");                                  // Current party size
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

                                    if (preset.ToString()[..3].Equals(specificJob, StringComparison.CurrentCultureIgnoreCase) ||  // Job identifier
                                        preset.ToString()[..3].Equals("all", StringComparison.CurrentCultureIgnoreCase) ||        // Adds in Globals
                                        preset.ToString()[..3].Equals("pvp", StringComparison.CurrentCultureIgnoreCase))          // Adds in PvP Globals
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
                                var jobID = Svc.Data.GetExcelSheet<ClassJob>()?
                                    .Where(x => x.Name.ToString().Equals(jobname, StringComparison.CurrentCultureIgnoreCase))
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
                                    41 => typeof(VPR.Config),
                                    42 => typeof(PCT.Config),
                                    _ => throw new NotImplementedException(),
                                };

                                foreach (var config in whichConfig.GetMembers().Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property))
                                {
                                    string key = config.Name!;

                                    if (PluginConfiguration.CustomIntValues.TryGetValue(key, out int intvalue)) { file.WriteLine($"{key} - {intvalue}"); continue; }
                                    if (PluginConfiguration.CustomFloatValues.TryGetValue(key, out float floatvalue)) { file.WriteLine($"{key} - {floatvalue}"); continue; }
                                    if (PluginConfiguration.CustomBoolValues.TryGetValue(key, out bool boolvalue)) { file.WriteLine($"{key} - {boolvalue}"); continue; }
                                    if (PluginConfiguration.CustomBoolArrayValues.TryGetValue(key, out bool[]? boolarrayvalue)) { file.WriteLine($"{key} - {string.Join(", ", boolarrayvalue)}"); continue; }

                                    file.WriteLine($"{key} - NOT SET");
                                }

                                foreach (var config in typeof(PvPCommon.Config).GetMembers().Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property))
                                {
                                    string key = config.Name!;

                                    if (PluginConfiguration.CustomIntValues.TryGetValue(key, out int intvalue)) { file.WriteLine($"{key} - {intvalue}"); continue; }
                                    if (PluginConfiguration.CustomFloatValues.TryGetValue(key, out float floatalue)) { file.WriteLine($"{key} - {floatalue}"); continue; }
                                    if (PluginConfiguration.CustomBoolValues.TryGetValue(key, out bool boolvalue)) { file.WriteLine($"{key} - {boolvalue}"); continue; }
                                    if (PluginConfiguration.CustomBoolArrayValues.TryGetValue(key, out bool[]? boolarrayvalue)) { file.WriteLine($"{key} - {string.Join(", ", boolarrayvalue)}"); continue; }

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

                            file.WriteLine($"Status Effect Count: {Svc.ClientState.LocalPlayer.StatusList.Count(x => x != null)}");

                            if (Svc.ClientState.LocalPlayer.StatusList.Length > 0)
                            {
                                file.WriteLine($"START STATUS EFFECTS");
                                foreach (Status? status in Svc.ClientState.LocalPlayer.StatusList)
                                {
                                    file.WriteLine($"ID: {status.StatusId}, COUNT: {status.StackCount}, SOURCE: {status.SourceId} NAME: {ActionWatching.GetStatusName(status.StatusId)}");
                                }

                                file.WriteLine($"END STATUS EFFECTS");
                            }

                            file.WriteLine("END DEBUG LOG");
                            Svc.Chat.Print("Please check your desktop for SlothDebug.txt and upload this file where requested.");

                            break;
                        }

                        catch (Exception ex)
                        {
                            Svc.Log.Error(ex, "Debug Log");
                            Svc.Chat.Print("Unable to write Debug log.");
                            break;
                        }
                    }
                default:
                    ConfigWindow.IsOpen = !ConfigWindow.IsOpen;
                    PvEFeatures.HasToOpenJob = true;
                    if (argumentsParts[0].Length > 0)
                    {
                        var jobname = ConfigWindow.groupedPresets.Where(x => x.Value.Any(y => y.Info.JobShorthand.Equals(argumentsParts[0].ToLower(), StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault().Key;
                        var header = $"{jobname} - {argumentsParts[0].ToUpper()}";
                        Svc.Log.Debug($"{jobname}");
                        PvEFeatures.HeaderToOpen = header;
                    }
                    break;
            }

            Service.Configuration.Save();
        }
    }
}
