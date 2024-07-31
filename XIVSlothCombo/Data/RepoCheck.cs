using ECommons.DalamudServices;
using Newtonsoft.Json;
using System.IO;

namespace XIVSlothCombo.Data
{
    public class RepoCheck
    {
        public string? InstalledFromUrl { get; set; }
    }

    public static class RepoCheckFunctions
    {
        public static RepoCheck? FetchCurrentRepo()
        {
            FileInfo? f = Svc.PluginInterface.AssemblyLocation;
            var manifest = Path.Join(f.DirectoryName, "XIVSlothCombo.json");

            if (File.Exists(manifest))
            {
                RepoCheck? repo = JsonConvert.DeserializeObject<RepoCheck>(File.ReadAllText(manifest));
                return repo;
            }
            else
            {
                return null;
            }

        }

        public static bool IsFromSlothRepo()
        {
            RepoCheck? repo = FetchCurrentRepo();
            if (repo is null) return false;

            if (repo.InstalledFromUrl is null) return false;

            if (repo.InstalledFromUrl == "https://raw.githubusercontent.com/Nik-Potokar/MyDalamudPlugins/main/pluginmaster.json")
                return true;
            else
                return false;
        }
    }
}
