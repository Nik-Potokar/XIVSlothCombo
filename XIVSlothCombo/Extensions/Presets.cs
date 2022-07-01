using Dalamud.Utility;
using System.Collections.Generic;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;

namespace XIVSlothCombo.Extensions
{
    internal static class PresetExtensions
    {
        internal static Dictionary<CustomComboPreset, ReplaceSkillAttribute> replaceSkillCache = new Dictionary<CustomComboPreset, ReplaceSkillAttribute>();
        internal static Dictionary<CustomComboPreset, CustomComboInfoAttribute> comboInfoCache = new Dictionary<CustomComboPreset, CustomComboInfoAttribute>();
        internal static Dictionary<CustomComboPreset, HoverInfoAttribute> hoverInfoCache = new Dictionary<CustomComboPreset, HoverInfoAttribute>();

        ///<summary> Retrieves the <see cref="ReplaceSkillAttribute"/> for the preset if it exists.</summary>
        internal static ReplaceSkillAttribute? GetReplaceAttribute(this CustomComboPreset preset)
        {
            if (replaceSkillCache.TryGetValue(preset, out var replaceSkillAttribute))
            {
                return replaceSkillAttribute;
            }

            ReplaceSkillAttribute att = preset.GetAttribute<ReplaceSkillAttribute>();
            return replaceSkillCache.TryAdd(preset, att) ? replaceSkillCache[preset] : null;
        }

        ///<summary> Retrieves the <see cref="CustomComboInfoAttribute"/> for the preset if it exists.</summary>
        internal static CustomComboInfoAttribute? GetComboAttribute(this CustomComboPreset preset)
        {
            if (comboInfoCache.TryGetValue(preset, out var customComboInfoAttribute))
            {
                return customComboInfoAttribute;
            }

            CustomComboInfoAttribute att = preset.GetAttribute<CustomComboInfoAttribute>();
            return comboInfoCache.TryAdd(preset, att) ? comboInfoCache[preset] : null;

        }

        ///<summary> Retrieves the <see cref="HoverInfoAttribute"/> for the preset if it exists.</summary>
        internal static HoverInfoAttribute? GetHoverAttribute(this CustomComboPreset preset)
        {
            if (hoverInfoCache.TryGetValue(preset, out var hoverInfoAttribute))
            {
                return hoverInfoAttribute;
            }

            HoverInfoAttribute att = preset.GetAttribute<HoverInfoAttribute>();
            return hoverInfoCache.TryAdd(preset, att) ? hoverInfoCache[preset] : null;

        }
    }
}
