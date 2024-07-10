using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;

namespace XIVSlothCombo.Window
{
    internal static class Icons
    {
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            if (jobId == 0 || jobId > 42) return null;
            var icon = Svc.Texture.GetFromGameIcon(62100 + jobId);
            if (!icon.TryGetWrap(out var wrap, out _))
                return null;
            return wrap;
        }
    }
}
