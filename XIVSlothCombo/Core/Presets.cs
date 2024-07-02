using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Core
{
    internal static class PresetStorage
    {
        private static HashSet<CustomComboPreset>? SecretCombos;
        private static HashSet<CustomComboPreset>? VariantCombos;
        private static HashSet<CustomComboPreset>? BozjaCombos;
        private static HashSet<CustomComboPreset>? EurekaCombos;
        private static Dictionary<CustomComboPreset, CustomComboPreset[]>? ConflictingCombos;
        private static Dictionary<CustomComboPreset, CustomComboPreset?>? ParentCombos;  // child: parent

        public static void Init()
        {
            // Secret combos
            SecretCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<SecretCustomComboAttribute>() != default)
                .ToHashSet();

            VariantCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<VariantAttribute>() != default)
                .ToHashSet();

            BozjaCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<BozjaAttribute>() != default)
                .ToHashSet();

            EurekaCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<EurekaAttribute>() != default)
                .ToHashSet();

            // Conflicting combos
            ConflictingCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>());

            // Parent combos
            ParentCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);
        }


        /// <summary> Gets a value indicating whether a preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsEnabled(CustomComboPreset preset) => Service.Configuration.EnabledActions.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsSecret(CustomComboPreset preset) => SecretCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsVariant(CustomComboPreset preset) => VariantCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsBozja(CustomComboPreset preset) => BozjaCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsEureka(CustomComboPreset preset) => EurekaCombos.Contains(preset);

        /// <summary> Gets the parent combo preset if it exists, or null. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The parent preset. </returns>
        public static CustomComboPreset? GetParent(CustomComboPreset preset) => ParentCombos[preset];

        /// <summary> Gets an array of conflicting combo presets. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The conflicting presets. </returns>
        public static CustomComboPreset[] GetConflicts(CustomComboPreset preset) => ConflictingCombos[preset];

        /// <summary> Gets the full list of conflicted combos. </summary>
        public static List<CustomComboPreset> GetAllConflicts() => ConflictingCombos.Keys.ToList();

        /// <summary> Get all the info from conflicted combos. </summary>
        public static List<CustomComboPreset[]> GetAllConflictOriginals() => ConflictingCombos.Values.ToList();
    }
}
