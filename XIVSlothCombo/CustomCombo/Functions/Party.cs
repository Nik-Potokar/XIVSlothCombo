using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using System.Linq;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Gets the party list </summary>
        /// <returns> Current party list. </returns>
        public static PartyList GetPartyMembers() => Service.PartyList;

        protected unsafe static GameObject? GetPartySlot(int slot)
        {
            try
            {
                var o = slot switch
                {
                    1 => GetTarget(TargetType.Self),
                    2 => GetTarget(TargetType.P2),
                    3 => GetTarget(TargetType.P3),
                    4 => GetTarget(TargetType.P4),
                    5 => GetTarget(TargetType.P5),
                    6 => GetTarget(TargetType.P6),
                    7 => GetTarget(TargetType.P7),
                    8 => GetTarget(TargetType.P8),
                    _ => GetTarget(TargetType.Self),
                };
                var i = PartyTargetingService.GetObjectID(o);
                if (Service.ObjectTable.Where(x => x.ObjectId == i).Any())
                    return Service.ObjectTable.Where(x => x.ObjectId == i).First();

                return null;
            }

            catch
            {
                return null;
            }
        }
    }
}
