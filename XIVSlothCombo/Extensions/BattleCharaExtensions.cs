using Dalamud.Game.ClientState.Objects.Types;

namespace XIVSlothCombo.Extensions
{
    internal static class BattleCharaExtensions
    {
        public unsafe static uint RawShieldValue(this BattleChara chara)
        {
            FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
            var value = baseVal->Character.CharacterData.ShieldValue;
            var rawValue = chara.MaxHp / 100 * value;

            return rawValue;
        }

        public unsafe static byte ShieldPercentage(this BattleChara chara)
        {
            FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
            var value = baseVal->Character.CharacterData.ShieldValue;

            return value;
        }

        public static bool HasShield(this BattleChara chara) => chara.RawShieldValue() > 0;
    }
}
