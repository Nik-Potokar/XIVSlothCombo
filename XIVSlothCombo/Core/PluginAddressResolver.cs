using System;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Core
{
    /// <summary> Plugin address resolver. </summary>
    internal class PluginAddressResolver
    {
        /// <summary> Gets the address of the member ComboTimer. </summary>
        public IntPtr ComboTimer { get; private set; }

        /// <summary> Gets the address of the member LastComboMove. </summary>
        public IntPtr LastComboMove => ComboTimer + 0x4;

        /// <inheritdoc/>
        public unsafe void Setup(ISigScanner scanner)
        {
            ComboTimer = new IntPtr(&ActionManager.Instance()->Combo.Timer);

            Service.PluginLog.Verbose("===== X I V S L O T H C O M B O =====");
            Service.PluginLog.Verbose($"{nameof(ComboTimer)}            0x{ComboTimer:X}");
            Service.PluginLog.Verbose($"{nameof(LastComboMove)}         0x{LastComboMove:X}");
        }
    }
}
