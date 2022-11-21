using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Services;
using XIVSlothCombo.Extensions;
using System.Numerics;

namespace XIVSlothCombo.Core
{
    /// <summary> Plugin configuration. </summary>
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration
    {
        private static readonly HashSet<CustomComboPreset> SecretCombos;
        private static readonly HashSet<CustomComboPreset> VariantCombos;
        private static readonly HashSet<CustomComboPreset> BozjaCombos;
        private static readonly HashSet<CustomComboPreset> EurekaCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos;  // child: parent

        static PluginConfiguration()
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

        #region Version

        /// <summary> Gets or sets the configuration version. </summary>
        public int Version { get; set; } = 5;

        #endregion

        #region EnabledActions

        /// <summary> Gets or sets the collection of enabled combos. </summary>
        [JsonProperty("EnabledActionsV5")]
        public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

        /// <summary> Gets or sets the collection of enabled combos. </summary>
        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

        #endregion

        #region Settings Options

        /// <summary> Gets or sets a value indicating whether to output combat log to the chatbox. </summary>
        public bool EnabledOutputLog { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide combos which conflict with enabled presets. </summary>
        public bool HideConflictedCombos { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide the children of a feature if it is disabled. </summary>
        public bool HideChildren { get; set; } = false;

        /// <summary> Gets or sets the offset of the melee range check. Default is 0. </summary>
        public double MeleeOffset { get; set; } = 0;

        public Vector4 TargetHighlightColor { get; set; } = new() { W = 1, X = 0.5f, Y = 0.5f, Z = 0.5f };

        #endregion

        #region Combo Preset Checks

        /// <summary> Gets a value indicating whether a preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public bool IsEnabled(CustomComboPreset preset) => EnabledActions.Contains(preset);

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

        #endregion

        #region Conflicting Combos

        /// <summary> Gets an array of conflicting combo presets. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The conflicting presets. </returns>
        public CustomComboPreset[] GetConflicts(CustomComboPreset preset) => ConflictingCombos[preset];

        /// <summary> Gets the full list of conflicted combos. </summary>
        public List<CustomComboPreset> GetAllConflicts() => ConflictingCombos.Keys.ToList();

        /// <summary> Get all the info from conflicted combos. </summary>
        public List<CustomComboPreset[]> GetAllConflictOriginals() => ConflictingCombos.Values.ToList();

        #endregion

        #region Custom Float Values

        [JsonProperty]
        private static Dictionary<string, float> CustomFloatValues { get; set; } = new Dictionary<string, float>();

        /// <summary> Gets a custom float value. </summary>
        public static float GetCustomFloatValue(string config, float defaultMinValue = 0)
        {
            if (!CustomFloatValues.TryGetValue(config, out float configValue))
            {
                SetCustomFloatValue(config, defaultMinValue);
                return defaultMinValue;
            }

            return configValue;
        }

        /// <summary> Sets a custom float value. </summary>
        public static void SetCustomFloatValue(string config, float value) => CustomFloatValues[config] = value;

        #endregion

        #region Custom Int Values

        [JsonProperty]
        private static Dictionary<string, int> CustomIntValues { get; set; } = new Dictionary<string, int>();

        /// <summary> Gets a custom integer value. </summary>
        public static int GetCustomIntValue(string config, int defaultMinVal = 0)
        {
            if (!CustomIntValues.TryGetValue(config, out int configValue))
            {
                SetCustomIntValue(config, defaultMinVal);
                return defaultMinVal;
            }

            return configValue;
        }

        /// <summary> Sets a custom integer value. </summary>
        public static void SetCustomIntValue(string config, int value) => CustomIntValues[config] = value;

        #endregion

        #region Custom Bool Values

        [JsonProperty]
        private static Dictionary<string, bool> CustomBoolValues { get; set; } = new Dictionary<string, bool>();

        /// <summary> Gets a custom boolean value. </summary>
        public static bool GetCustomBoolValue(string config)
        {
            if (!CustomBoolValues.TryGetValue(config, out bool configValue))
            {
                SetCustomBoolValue(config, false);
                return false;
            }

            return configValue;
        }

        /// <summary> Sets a custom boolean value. </summary>
        public static void SetCustomBoolValue(string config, bool value) => CustomBoolValues[config] = value;

        #endregion

        #region Custom Bool Array Values

        [JsonProperty]
        private static Dictionary<string, bool[]> CustomBoolArrayValues { get; set; } = new Dictionary<string, bool[]>();

        /// <summary> Gets a custom boolean array value. </summary>
        public static bool[] GetCustomBoolArrayValue(string config)
        {
            if (!CustomBoolArrayValues.TryGetValue(config, out bool[]? configValue))
            {
                SetCustomBoolArrayValue(config, Array.Empty<bool>());
                return Array.Empty<bool>();
            }

            return configValue;
        }

        /// <summary> Sets a custom boolean array value. </summary>
        public static void SetCustomBoolArrayValue(string config, bool[] value) => CustomBoolArrayValues[config] = value;

        #endregion

        #region Job-specific

        /// <summary> Gets active Blue Mage (BLU) spells. </summary>
        public List<uint> ActiveBLUSpells { get; set; } = new List<uint>();

        /// <summary> Gets or sets an array of 4 ability IDs to interact with the <see cref="CustomComboPreset.DNC_DanceComboReplacer"/> combo. </summary>
        public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[]
        {
            DNC.Cascade,
            DNC.Flourish,
            DNC.FanDance1,
            DNC.FanDance2,
        };

        #endregion

        #region Preset Resetting

        [JsonProperty]
        private static Dictionary<string, bool> ResetFeatureCatalog { get; set; } = new Dictionary<string, bool>();

        private bool GetResetValues(string config)
        {
            if (ResetFeatureCatalog.TryGetValue(config, out var value)) return value;

            return false;
        }

        private void SetResetValues(string config, bool value)
        {
            ResetFeatureCatalog[config] = value;
        }

        public void ResetFeatures(string config, int[] values)
        {
            Dalamud.Logging.PluginLog.Debug($"{config} {GetResetValues(config)}");
            if (!GetResetValues(config))
            {
                bool needToResetMessagePrinted = false;

                var presets = Enum.GetValues<CustomComboPreset>().Cast<int>();

                foreach (int value in values)
                {
                    Dalamud.Logging.PluginLog.Debug(value.ToString());
                    if (presets.Contains(value))
                    {
                        var preset = Enum.GetValues<CustomComboPreset>()
                            .Where(preset => (int)preset == value)
                            .First();

                        if (!IsEnabled(preset)) continue;

                        if (!needToResetMessagePrinted)
                        {
                            Service.ChatGui.PrintError($"[XIVSlothCombo] Some features have been disabled due to an internal configuration update:");
                            needToResetMessagePrinted = !needToResetMessagePrinted;
                        }

                        var info = preset.GetComboAttribute();
                        Service.ChatGui.PrintError($"[XIVSlothCombo] - {info.JobName}: {info.FancyName}");
                        EnabledActions.Remove(preset);
                    }
                }
                
                if (needToResetMessagePrinted)
                Service.ChatGui.PrintError($"[XIVSlothCombo] Please re-enable these features to use them again. We apologise for the inconvenience");
            }
            SetResetValues(config, true);
            Save();
        }

        #endregion

        #region Other (SpecialEvent, MotD, Save)

        /// <summary> Handles 'special event' feature naming. </summary>
        public bool SpecialEvent { get; set; } = false;

        /// <summary> Hides the message of the day. </summary>
        public bool HideMessageOfTheDay { get; set; } = false;

        /// <summary> Save the configuration to disk. </summary>
        public void Save() => Service.Interface.SavePluginConfig(this);

        #endregion
    }
}
