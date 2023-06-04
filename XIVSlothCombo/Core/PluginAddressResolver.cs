using System;
using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace XIVSlothCombo.Core
{
    /// <summary> Plugin address resolver. </summary>
    internal class PluginAddressResolver : BaseAddressResolver
    {
        /// <summary> Gets the address of the member ComboTimer. </summary>
        public IntPtr ComboTimer { get; private set; }

        /// <summary> Gets the address of the member LastComboMove. </summary>
        public IntPtr LastComboMove => ComboTimer + 0x4;

        /// <summary> Gets the address of fpGetAdjustedActionId. </summary>
        public IntPtr GetAdjustedActionId { get; private set; }

        /// <summary> Gets the address of fpIsIconReplacable. </summary>
        public IntPtr IsActionIdReplaceable { get; private set; }

        /// <inheritdoc/>
        protected unsafe override void Setup64Bit(SigScanner scanner)
        {
            ComboTimer = new IntPtr(&ActionManager.Instance()->Combo.Timer);

            GetAdjustedActionId = scanner.ScanText("E8 ?? ?? ?? ?? 89 03 8B 03");  // Client::Game::ActionManager.GetAdjustedActionId

            IsActionIdReplaceable = scanner.ScanText("E8 ?? ?? ?? ?? 84 C0 74 4C 8B D3");

            PluginLog.Verbose("===== X I V S L O T H C O M B O =====");
            PluginLog.Verbose($"{nameof(GetAdjustedActionId)}   0x{GetAdjustedActionId:X}");
            PluginLog.Verbose($"{nameof(IsActionIdReplaceable)} 0x{IsActionIdReplaceable:X}");
            PluginLog.Verbose($"{nameof(ComboTimer)}            0x{ComboTimer:X}");
            PluginLog.Verbose($"{nameof(LastComboMove)}         0x{LastComboMove:X}");
        }
    }
}
