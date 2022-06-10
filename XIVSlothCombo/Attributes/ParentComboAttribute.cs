using System;

namespace XIVSlothComboPlugin.Attributes
{
    /// <summary> Attribute documenting required combo relationships. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class ParentComboAttribute : Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="ParentComboAttribute"/> class. </summary>
        /// <param name="parentPreset">Presets that conflict with the given combo.</param>
        internal ParentComboAttribute(CustomComboPreset parentPreset) => ParentPreset = parentPreset;

        /// <summary> Gets the display name. </summary>
        public CustomComboPreset ParentPreset { get; }
    }
}
