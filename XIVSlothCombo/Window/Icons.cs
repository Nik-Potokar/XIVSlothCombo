using Dalamud.Interface.Internal;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSlothCombo.Window
{
    internal static class Icons
    {
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            if (jobId == 0) return null;
            return Svc.Texture.GetIcon(62100 + jobId);
        }
    }
}
