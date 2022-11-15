using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Attributes
{
    internal class PotionAttribute : Attribute
    {
        internal (uint Id, string Name, IEnumerable<PotionType> types)[]? Pots;
        internal string Config { get; set; }
        internal PotionAttribute(string config, params PotionType[] potionTypes)
        {
            Pots = Service.DataManager.GetExcelSheet<Item>()?.Where(x => !x.RowId.EqualsAny<uint>(4570) && x.RowId >= 4594 && x.ItemUICategory.Value.RowId == 44 && IsCombatAttribute(x)).Select(x => (x.RowId, x.Name.ToString(), GetTypes(x))).ToArray();

            Config = config;

            if (Service.Configuration is null || Pots.Length == 0)
                return;

            foreach (PotionType potion in potionTypes)
            {
                switch (potion)
                {
                    case PotionType.Strength:
                        foreach (var pot in Pots.Where(x => x.types.Any(y => y == PotionType.Strength)))
                        {
                            Potions.Add(pot.Id, pot.Name);
                        }
                        break;
                    case PotionType.Dexterity:
                        foreach (var pot in Pots.Where(x => x.types.Any(y => y == PotionType.Dexterity)))
                        {
                            Potions.Add(pot.Id, pot.Name);
                        }
                        break;
                    case PotionType.Vitality:
                        foreach (var pot in Pots.Where(x => x.types.Any(y => y == PotionType.Vitality)))
                        {
                            Potions.Add(pot.Id, pot.Name);
                        }
                        break;
                    case PotionType.Intelligence:
                        foreach (var pot in Pots.Where(x => x.types.Any(y => y == PotionType.Intelligence)))
                        {
                            Potions.Add(pot.Id, pot.Name);
                        }
                        break;
                    case PotionType.Mind:
                        foreach (var pot in Pots.Where(x => x.types.Any(y => y == PotionType.Mind)))
                        {
                            Potions.Add(pot.Id, pot.Name);
                        }
                        break;
                }
            }
        }

        internal Dictionary<uint, string> Potions { get; set; } = new();

        internal static bool IsCombatAttribute(Item x)
        {
            try
            {
                foreach (var z in x.ItemAction.Value?.Data)
                {
                    if (Service.DataManager.GetExcelSheet<ItemFood>().GetRow(z).UnkData1[0].BaseParam.EqualsAny<byte>(1, 2, 3, 4, 5))
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        internal IEnumerable<PotionType> GetTypes(Item x)
        {
            if (x == null) yield return PotionType.Unknown;

            foreach (var z in x.ItemAction.Value.Data)
            {
                if (z == 0) continue;

                var baseParam = Service.DataManager.GetExcelSheet<ItemFood>().GetRow(z)?.UnkData1[0];
                if (baseParam is null) continue;

                switch (baseParam.BaseParam)
                {
                    case 1:
                        yield return PotionType.Strength;
                        break;
                    case 2:
                        yield return PotionType.Dexterity;
                        break;
                    case 3:
                        yield return PotionType.Vitality;
                        break;
                    case 4:
                        yield return PotionType.Intelligence;
                        break;
                    case 5:
                        yield return PotionType.Mind;
                        break;
                    default:
                        yield return PotionType.Unknown;
                        break;
                }

            }
            yield return PotionType.Unknown;
        }
    }

    public enum PotionType
    {
        Unknown,
        Strength,
        Dexterity,
        Vitality,
        Intelligence,
        Mind
    }
}