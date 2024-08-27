using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Extensions
{
    internal static class UIntExtensions
    {
        internal static bool LevelChecked(this uint value) => LevelChecked(value);

        internal static bool TraitLevelChecked(this uint value) => TraitLevelChecked(value);

        internal static string ActionName(this uint value) => GetActionName(value);
    }

    internal static class UShortExtensions
    {
        internal static string StatusName(this ushort value) => ActionWatching.GetStatusName(value);
    }
}
