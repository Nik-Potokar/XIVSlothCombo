using System;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Attributes
{
    /// <summary> Attribute documenting which skill each preset replace. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReplaceSkillAttribute : Attribute
    {
        private static readonly Dictionary<uint, Lumina.Excel.GeneratedSheets.Action>? ActionSheet = Service.DataManager?.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?
                    .Where(i => i.ClassJobCategory.Row > 0 && i.ActionCategory.Row <= 4 && i.RowId is not 7)
                    .ToDictionary(i => i.RowId, i => i);

        /// <summary> List of each action the feature replaces. Initializes a new instance of the <see cref="ReplaceSkillAttribute"/> class. </summary>
        /// <param name="actionIDs"> List of actions the preset replaces. </param>
        internal ReplaceSkillAttribute(params uint[] actionIDs)
        {
            foreach (uint id in actionIDs)
            {
                if (ActionSheet.TryGetValue(id, out var action) && action != null)
                {
                    ActionNames.Add($"{action.Name}");
                }
            }
        }

        internal List<string> ActionNames { get; set; } = new();
    }
}
