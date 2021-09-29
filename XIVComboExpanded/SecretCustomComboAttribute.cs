using System;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Attribute designating secret combos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class SecretCustomComboAttribute : Attribute
    {
    }
}
