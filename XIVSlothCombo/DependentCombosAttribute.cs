using System;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Attribute documenting dependencies for each combo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class DependentCombosAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentCombosAttribute"/> class.
        /// </summary>
        /// <param name="dependentPresets">Presets that the given combo depends on.</param>
        internal DependentCombosAttribute(params CustomComboPreset[] dependentPresets)
        {
            this.DependentPresets = dependentPresets;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public CustomComboPreset[] DependentPresets { get; }
    }
}
