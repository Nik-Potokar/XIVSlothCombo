using System;
using XIVSlothCombo.Combos;

namespace XIVSlothCombo.Attributes
{
    /// <summary> Attribute documenting required combo relationships. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class VariantParentAttribute : Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="VariantParentAttribute"/> class. </summary>
        /// <param name="parentPresets"> Presets that require the given combo to be enabled. </param>
        internal VariantParentAttribute(params CustomComboPreset[] parentPresets) => ParentPresets = parentPresets;

        /// <summary> Gets the display name. </summary>
        public CustomComboPreset[] ParentPresets { get; }
    }
}
