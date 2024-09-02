using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Utility;
using ECommons.DalamudServices;
using Lumina.Data.Files;
using System.Collections.Generic;
using XIVSlothCombo.Combos.PvE;

namespace XIVSlothCombo.Window
{
    internal static class Icons
    {
        public static Dictionary<uint, IDalamudTextureWrap> CachedModdedIcons = new();
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            switch (jobId)
            {
                case All.JobID: jobId = 62146; break; //Adventurer / General
                case > All.JobID and <= 42: jobId += 62100; break; //Classes
                case DOL.JobID: jobId = 82096; break;
                default: return null; //Unknown, return null
            }
            return GetTextureFromIconId(jobId);
        }

        private static string ResolvePath(string path) => Svc.TextureSubstitution.GetSubstitutedPath(path);

        public static IDalamudTextureWrap? GetTextureFromIconId(uint iconId, uint stackCount = 0, bool hdIcon = true)
        {
            GameIconLookup lookup = new(iconId + stackCount, false, hdIcon);
            string path = Svc.Texture.GetIconPath(lookup);
            string resolvePath = ResolvePath(path);

            var wrap = Svc.Texture.GetFromFile(resolvePath);
            if (wrap.TryGetWrap(out var icon, out _))
                return icon;

            try
            {
                if (CachedModdedIcons.TryGetValue(iconId, out IDalamudTextureWrap? cachedIcon)) return cachedIcon;
                var tex = Svc.Data.GameData.GetFileFromDisk<TexFile>(resolvePath);
                var output = Svc.Texture.CreateFromRaw(RawImageSpecification.Rgba32(tex.Header.Width, tex.Header.Width), tex.GetRgbaImageData());
                if (output != null)
                {
                    CachedModdedIcons[iconId] = output;
                    return output;
                }
            }
            catch { }


            return Svc.Texture.GetFromGame(path).GetWrapOrDefault();
        }
    }
}
