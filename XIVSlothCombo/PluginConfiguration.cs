using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVSlothComboPlugin.Attributes;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration
    {
        private static readonly HashSet<CustomComboPreset> SecretCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
        private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos;  // child: parent

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
        }

        /// <summary>
        /// Gets or sets the configuration version.
        /// </summary>
        public int Version { get; set; } = 5;

        /// <summary>
        /// Gets or sets the collection of enabled combos.
        /// </summary>
        [JsonProperty("EnabledActionsV5")]
        public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of enabled combos.
        /// </summary>
        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether to allow and display secret combos.
        /// </summary>
        [JsonProperty("Debug")]
        public bool EnableSecretCombos { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to hide combos which conflict with enabled presets.
        /// </summary>
        public bool HideConflictedCombos { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to hide the children of a feature if it is disabled.
        /// </summary>
        public bool HideChildren { get; set; } = false;

        /// <summary>
        /// Gets or sets an array of 4 ability IDs to interact with the <see cref="CustomComboPreset.DancerDanceComboCompatibility"/> combo.
        /// </summary>
        public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[]
        {
            DNC.Cascade,
            DNC.Flourish,
            DNC.FanDance1,
            DNC.FanDance2,
        };

        /// <summary>
        /// Gets or sets the offset of the melee range check. Default is 0.
        /// </summary>
        public double MeleeOffset { get; set; } = 0;

        /// <summary>
        /// Save the configuration to disk.
        /// </summary>
        public void Save()
            => Service.Interface.SavePluginConfig(this);

        /// <summary>
        /// Gets a value indicating whether a preset is enabled.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsEnabled(CustomComboPreset preset)
            => this.EnabledActions.Contains(preset) && (this.EnableSecretCombos || !this.IsSecret(preset));

        /// <summary>
        /// Gets a value indicating whether a preset is secret.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsSecret(CustomComboPreset preset)
            => SecretCombos.Contains(preset);

        /// <summary>
        /// Gets an array of conflicting combo presets.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The conflicting presets.</returns>
        public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
            => ConflictingCombos[preset];

        /// <summary>
        /// Gets the parent combo preset if it exists, or null.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The parent preset.</returns>
        public CustomComboPreset? GetParent(CustomComboPreset preset)
            => ParentCombos[preset];

        /// <summary>
        /// Gets the full list of conflicted combos
        /// </summary>
        public List<CustomComboPreset> GetAllConflicts()
            => ConflictingCombos.Keys.ToList();

        /// <summary>
        /// Get all the info from conflicted combos
        /// </summary>
        public List<CustomComboPreset[]> GetAllConflictOriginals()
            => ConflictingCombos.Values.ToList();

        public float EnemyHealthPercentage { get; set; } = 0;

        public float EnemyHealthMaxHp { get; set; } = 0;

        public float EnemyCurrentHp { get; set; } = 0;

        public float SkillCooldownRemaining { get; set; } = 0;

        public int MudraPathSelection { get; set; } = 0;

        public bool AprilFoolsSlothIrl { get; set; } = false;

        [JsonProperty]
        private static Dictionary<string,float> CustomConfigValues { get; set; } = new Dictionary<string,float>();

        [JsonProperty]
        private static Dictionary<string, int> CustomIntValues { get; set; } = new Dictionary<string, int>();

        [JsonProperty]
        private static Dictionary<string, bool> CustomBoolValues { get; set; } = new Dictionary<string, bool>();

        public float GetCustomConfigValue(string config, float defaultMinValue = 0)
        {
            float configValue;

            if (!CustomConfigValues.TryGetValue(config, out configValue)) return defaultMinValue;

            return configValue;
        }

        public void SetCustomConfigValue(string config, float value)
        {
            CustomConfigValues[config] = value;
        }

        public int GetCustomIntValue(string config, int defaultMinVal = 0)
        {
            int configValue;

            if (!CustomIntValues.TryGetValue(config, out configValue)) return defaultMinVal;

            return configValue;
        }

        public void SetCustomIntValue(string config, int value)
        {
            CustomIntValues[config] = value;
        }

        public bool GetCustomBoolValue(string config)
        {
            bool configValue;

            if (!CustomBoolValues.TryGetValue(config, out configValue)) return false;

            return configValue;
        }

        public void SetCustomBoolValue(string config, bool value)
        {
            CustomBoolValues[config] = value;
        }
    }
}
