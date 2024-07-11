using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Utility;
using ECommons.DalamudServices;
using Lumina.Data.Files;
using System.Collections.Generic;

namespace XIVSlothCombo.Window
{
    internal static class Icons
    {
        public static Dictionary<uint, IDalamudTextureWrap> CachedModdedIcons = new();
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            if (jobId == 0 || jobId > 42) return null;
            var icon = GetTextureFromIconId(62100 + jobId);

            return icon;
        }

        private static string ResolvePath(string path) => Svc.TextureSubstitution.GetSubstitutedPath(path);

        public static IDalamudTextureWrap? GetTextureFromIconId(uint iconId, uint stackCount = 0, bool hdIcon = true)
        {
            GameIconLookup lookup = new GameIconLookup(iconId + stackCount, false, hdIcon);
            string path = Svc.Texture.GetIconPath(lookup);
            string resolvePath = ResolvePath(path);

            var wrap = Svc.Texture.GetFromFile(resolvePath);
            if (wrap.TryGetWrap(out var icon, out _))
                return icon;

            try
            {
                if (CachedModdedIcons.ContainsKey(iconId)) return CachedModdedIcons[iconId];
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
