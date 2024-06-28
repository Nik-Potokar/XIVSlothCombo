using Dalamud.Interface.Internal;
using ECommons.DalamudServices;

namespace XIVSlothCombo.Window
{
    internal static class Icons
    {
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            if (jobId == 0) return null;
            return Svc.Texture.GetFromGameIcon(new Dalamud.Interface.Textures.GameIconLookup(62100 + jobId)).GetWrapOrEmpty();
        }
    }
}
