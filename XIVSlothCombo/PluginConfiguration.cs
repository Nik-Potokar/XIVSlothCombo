using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Configuration;
using Dalamud.Utility;
using ImGuiScene;
using Newtonsoft.Json;
using XIVSlothComboPlugin.Attributes;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary> Plugin configuration. </summary>
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration
    {
        private static readonly HashSet<CustomComboPreset> SecretCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos;  // child: parent
        private static readonly HashSet<CustomComboPreset> TrustIncompatibles;

        static PluginConfiguration()
        {
            SecretCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<SecretCustomComboAttribute>() != default)
                .ToHashSet();

            ConflictingCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>());

            ParentCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);

            TrustIncompatibles = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<TrustIncompatibleAttribute>() != default)
                .ToHashSet();

        }

        /// <summary> Gets or sets the configuration version. </summary>
        public int Version { get; set; } = 5;

        /// <summary> Gets or sets the collection of enabled combos. </summary>
        [JsonProperty("EnabledActionsV5")]
        public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

        /// <summary> Gets or sets the collection of enabled combos. </summary>
        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

        /// <summary> Gets or sets a value indicating whether to output combat log to the chatbox. </summary>
        public bool EnabledOutputLog { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide combos which conflict with enabled presets. </summary>
        public bool HideConflictedCombos { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide the children of a feature if it is disabled. </summary>
        public bool HideChildren { get; set; } = false;

        /// <summary> Gets or sets the offset of the melee range check. Default is 0. </summary>
        public double MeleeOffset { get; set; } = 0;

        /// <summary> Save the configuration to disk. </summary>
        public void Save()
            => Service.Interface.SavePluginConfig(this);

        /// <summary> Gets a value indicating whether a preset is enabled. </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsEnabled(CustomComboPreset preset)
            => this.EnabledActions.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsSecret(CustomComboPreset preset)
            => SecretCombos.Contains(preset);

        /// <summary> Gets an array of conflicting combo presets. </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The conflicting presets.</returns>
        public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
            => ConflictingCombos[preset];

        /// <summary> Gets the parent combo preset if it exists, or null. </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The parent preset.</returns>
        public CustomComboPreset? GetParent(CustomComboPreset preset)
            => ParentCombos[preset];

        /// <summary> Gets the full list of conflicted combos. </summary>
        public List<CustomComboPreset> GetAllConflicts()
            => ConflictingCombos.Keys.ToList();

        /// <summary> Get all the info from conflicted combos. </summary>
        public List<CustomComboPreset[]> GetAllConflictOriginals()
            => ConflictingCombos.Values.ToList();

        /// <summary> Handles 'special event' feature naming. </summary>
        public bool SpecialEvent { get; set; } = false;

        /// <summary> Hide MotD. </summary>
        public bool HideMessageOfTheDay { get; set; } = false;

        #region Image Caching
        [JsonProperty]
        public Dictionary<string, byte[]> ImageCache { get; set; } = new();

        /// <summary> Gets an image in the cache. </summary>
        public byte[]? GetImageInCache(string url)
        {
            byte[]? output;

            if (!ImageCache.TryGetValue(url, out output)) return null;

            return output;
        }

        /// <summary> Sets an image in the cache. </summary>
        public void SetImageInCache(string url, byte[] image)
        {
            ImageCache[url] = image;

        }
        #endregion

        #region Custom Float Values
        [JsonProperty]
        private static Dictionary<string, float> CustomFloatValues { get; set; } = new Dictionary<string, float>();

        /// <summary> Gets a custom float value. </summary>
        public float GetCustomFloatValue(string config, float defaultMinValue = 0)
        {
            float configValue;

            if (!CustomFloatValues.TryGetValue(config, out configValue)) { SetCustomFloatValue(config, defaultMinValue); return defaultMinValue; }

            return configValue;
        }

        /// <summary> Sets a custom float value. </summary>
        public void SetCustomFloatValue(string config, float value)
        {
            CustomFloatValues[config] = value;
        }
        #endregion

        #region Custom Int Values
        [JsonProperty]
        private static Dictionary<string, int> CustomIntValues { get; set; } = new Dictionary<string, int>();

        /// <summary> Gets a custom integer value. </summary>
        public int GetCustomIntValue(string config, int defaultMinVal = 0)
        {
            int configValue;

            if (!CustomIntValues.TryGetValue(config, out configValue)) { SetCustomIntValue(config, defaultMinVal); return defaultMinVal; }

            return configValue;
        }

        /// <summary> Sets a custom integer value. </summary>
        public void SetCustomIntValue(string config, int value)
        {
            CustomIntValues[config] = value;
        }
        #endregion

        #region Custom Bool Values
        [JsonProperty]
        private static Dictionary<string, bool> CustomBoolValues { get; set; } = new Dictionary<string, bool>();

        /// <summary> Gets a custom boolean value. </summary>
        public bool GetCustomBoolValue(string config)
        {
            bool configValue;

            if (!CustomBoolValues.TryGetValue(config, out configValue)) { SetCustomBoolValue(config, false); return false; }

            return configValue;
        }

        /// <summary> Sets a custom boolean value. </summary>
        public void SetCustomBoolValue(string config, bool value)
        {
            CustomBoolValues[config] = value;
        }
        #endregion

        #region Custom Bool Array Values
        [JsonProperty]
        private static Dictionary<string, bool[]> CustomBoolArrayValues { get; set; } = new Dictionary<string, bool[]>();

        /// <summary> Gets a custom boolean array value. </summary>
        public bool[] GetCustomBoolArrayValue(string config)
        {
            bool[]? configValue;

            if (!CustomBoolArrayValues.TryGetValue(config, out configValue)) { SetCustomBoolArrayValue(config, Array.Empty<bool>()); return Array.Empty<bool>(); }

            return configValue;
        }

        /// <summary> Sets a custom boolean array value. </summary>
        public void SetCustomBoolArrayValue(string config, bool[] value)
        {
            CustomBoolArrayValues[config] = value;
        }
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

        /// <summary> Handles Mudra path selection for NIN_Simple_Mudras. </summary>
        public int MudraPathSelection { get; set; } = 0;
        #endregion
    }
}
