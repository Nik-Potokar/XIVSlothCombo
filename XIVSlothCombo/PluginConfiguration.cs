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
    /// <summary>
    /// Plugin configuration.
    /// </summary>
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
        /// Gets or sets a value indicating whether to output combat log to the chatbox.
        /// </summary>
        public bool EnabledOutputLog { get; set; } = false;


        /// <summary>
        /// Gets or sets a value indicating wheteher to allow and display trust incompatible combos.
        /// </summary>
        public bool EnableTrustIncompatibles { get; set; } = false;

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
            => this.EnabledActions.Contains(preset);

        /// <summary>
        /// Gets a value indicating whether a preset is secret.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsSecret(CustomComboPreset preset)
            => SecretCombos.Contains(preset);

        /// <summary>
        /// Gets a value indicating whether a preset is trust incompatible.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>The boolean representation.</returns>
        public bool IsTrustIncompatible(CustomComboPreset preset)
            => TrustIncompatibles.Contains(preset);

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

        public bool SpecialEvent { get; set; } = false;

        public bool HideMessageOfTheDay { get; set; } = false;

        [JsonProperty]
        public Dictionary<string, byte[]> ImageCache { get; set; } = new();

        [JsonProperty]
        private static Dictionary<string, float> CustomFloatValues { get; set; } = new Dictionary<string, float>();

        [JsonProperty]
        private static Dictionary<string, int> CustomIntValues { get; set; } = new Dictionary<string, int>();

        [JsonProperty]
        private static Dictionary<string, bool> CustomBoolValues { get; set; } = new Dictionary<string, bool>();

        [JsonProperty]
        private static Dictionary<string, bool[]> CustomBoolArrayValues { get; set; } = new Dictionary<string, bool[]>();
        public float GetCustomFloatValue(string config, float defaultMinValue = 0)
        {
            float configValue;

            if (!CustomFloatValues.TryGetValue(config, out configValue)) return defaultMinValue;

            return configValue;
        }

        public void SetCustomFloatValue(string config, float value)
        {
            CustomFloatValues[config] = value;
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


        public byte[]? GetImageInCache(string url)
        {
            byte[]? output;

            if (!ImageCache.TryGetValue(url, out output)) return null;

            return output;
        }

        public void SetImageInCache(string url, byte[] image)
        {
            ImageCache[url] = image;

        }

        public bool[] GetCustomBoolArrayValue(string config)
        {
            bool[]? configValue;

            if (!CustomBoolArrayValues.TryGetValue(config, out configValue)) return Array.Empty<bool>();

            return configValue;
        }

        public void SetCustomBoolArrayValue(string config, bool[] value)
        {
            CustomBoolArrayValues[config] = value;
        }

        public bool GetJobGridValue(string config, byte jobID)
        {
            var index = JobIDToArrayIndex(jobID);
            var array = GetCustomBoolArrayValue(config);

            if (index == -1) return false;
            if (array == Array.Empty<bool>()) return false;
            return array[index];
        }

        private static int JobIDToArrayIndex(byte key)
        {
            return key switch
            {
                PLD.JobID => 0,
                PLD.ClassID => 0,
                WAR.JobID => 1,
                WAR.ClassID => 1,
                DRK.JobID => 2,
                GNB.JobID => 3,
                WHM.JobID => 4,
                WHM.ClassID => 4,
                SCH.JobID => 5,
                SCH.ClassID => 5,
                AST.JobID => 6,
                SGE.JobID => 7,
                MNK.JobID => 8,
                MNK.ClassID => 8,
                DRG.JobID => 9,
                DRG.ClassID => 9,
                NIN.JobID => 10,
                NIN.ClassID => 10,
                SAM.JobID => 11,
                RPR.JobID => 12,
                BRD.JobID => 13,
                BRD.ClassID => 13,
                MCH.JobID => 14,
                DNC.JobID => 15,
                BLM.JobID => 16,
                BLM.ClassID => 16,
                SMN.JobID => 17,
                RDM.JobID => 18,
                BLU.JobID => 19,
                _ => -1
            };
        }

        public bool GetRoleGridValue(string config, byte jobID)
        {
            var index = JobIDToArrayIndex(jobID);
            var array = GetCustomBoolArrayValue(config);

            if (index == -1) return false;
            if (array == Array.Empty<bool>()) return false;
            return array[index];
        }

        public List<uint> ActiveBLUSpells { get; set; } = new List<uint>();

        private static int RoleIDToArrayIndex(byte key)
        {
            return key switch
            {
                PLD.JobID => 0,
                PLD.ClassID => 0,
                WAR.JobID => 1,
                WAR.ClassID => 1,
                DRK.JobID => 2,
                GNB.JobID => 3,
                WHM.JobID => 4,
                WHM.ClassID => 4,
                SCH.JobID => 5,
                SCH.ClassID => 5,
                AST.JobID => 6,
                SGE.JobID => 7,
                MNK.JobID => 8,
                MNK.ClassID => 8,
                DRG.JobID => 9,
                DRG.ClassID => 9,
                NIN.JobID => 10,
                NIN.ClassID => 10,
                SAM.JobID => 11,
                RPR.JobID => 12,
                BRD.JobID => 13,
                BRD.ClassID => 13,
                MCH.JobID => 14,
                DNC.JobID => 15,
                BLM.JobID => 16,
                BLM.ClassID => 16,
                SMN.JobID => 17,
                RDM.JobID => 18,
                BLU.JobID => 19,
                _ => -1
            };
        }
    }
}
