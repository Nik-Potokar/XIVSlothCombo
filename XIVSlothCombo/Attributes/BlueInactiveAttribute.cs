using System;
using System.Collections.Generic;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Attributes
{
    /// <summary> Attribute documenting which skill the feature uses the user does not have active currently. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BlueInactiveAttribute : Attribute
    {
        /// <summary> List of each action the feature uses the user does not have active. Initializes a new instance of the <see cref="BlueInactiveAttribute"/> class. </summary>
        /// <param name="actionIDs"> List of actions the preset replaces. </param>
        internal BlueInactiveAttribute(params uint[] actionIDs)
        {
            if (Service.Configuration is null)
                return;

            NoneSet = true;
            foreach (uint id in actionIDs)
            {
                if (Service.Configuration.ActiveBLUSpells.Contains(id))
                {
                    NoneSet = false;
                    continue;
                }

                Actions.Add(id);
            }
        }

        internal List<uint> Actions { get; set; } = new();
        internal bool NoneSet { get; set; } = false;
    }
}
