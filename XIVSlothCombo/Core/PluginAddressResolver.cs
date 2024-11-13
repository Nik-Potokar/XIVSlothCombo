using Dalamud.Game;
using ECommons.DalamudServices;
using System;

namespace XIVSlothCombo.Core
{
    /// <summary> Plugin address resolver. </summary>
    internal class PluginAddressResolver
    {
        /// <summary> Gets the address of fpIsIconReplacable. </summary>
        public IntPtr IsActionIdReplaceable { get; private set; }

        /// <inheritdoc/>
        public unsafe void Setup(ISigScanner scanner)
        {
            IsActionIdReplaceable = scanner.ScanText("40 53 48 83 EC 20 8B D9 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 1B");

            Svc.Log.Debug("===== X I V S L O T H C O M B O =====");
            Svc.Log.Debug($"{nameof(IsActionIdReplaceable)} 0x{IsActionIdReplaceable:X}");
        }
    }
}
