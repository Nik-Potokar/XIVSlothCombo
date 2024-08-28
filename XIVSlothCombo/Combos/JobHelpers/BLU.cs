using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class BLUHelper
    {
        public static string GetBLUIndex(uint id)
        {
            var aozKey = Svc.Data.GetExcelSheet<AozAction>()!.First(x => x.Action.Row == id).RowId;
            var index = Svc.Data.GetExcelSheet<AozActionTransient>().GetRow(aozKey).Number;

            return $"#{index} ";
        }
    }
}