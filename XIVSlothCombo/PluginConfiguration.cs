using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration
    {
        /// <summary>
        /// Gets or sets the configuration version.
        /// </summary>
        public int Version { get; set; } = 4;

        /// <summary>
        /// Gets or sets the collection of enabled combos.
        /// </summary>
        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions { get; set; } = new ();

        /// <summary>
        /// Gets or sets a value indicating whether to allow and display secret combos.
        /// </summary>
        [JsonProperty("Debug")]
        public bool EnableSecretCombos { get; set; } = false;

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
            => preset.GetAttribute<SecretCustomComboAttribute>() != default;

        /// <summary>
        /// Gets an array of conflicting combo presets.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The conflicting presets.</returns>
        public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
            => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>();

        /// <summary>
        /// Gets an array of dependencies.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The dependencies of preset.</returns>
        public CustomComboPreset[] GetDependencies(CustomComboPreset preset)
            => preset.GetAttribute<DependentCombosAttribute>()?.DependentPresets ?? Array.Empty<CustomComboPreset>();
    }

}
