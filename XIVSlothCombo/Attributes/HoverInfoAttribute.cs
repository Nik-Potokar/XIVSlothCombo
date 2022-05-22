using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSlothComboPlugin.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HoverInfoAttribute :  Attribute
    {
        internal HoverInfoAttribute(string hoverText)
        {
            this.HoverText = hoverText;
        }

        public string HoverText { get; set; }
    }
}
