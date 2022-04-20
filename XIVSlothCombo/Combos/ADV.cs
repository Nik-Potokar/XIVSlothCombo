using Dalamud.Game.ClientState.Objects.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class ADV
    {
        public const byte ClassID = 0;
        public const byte JobID = 0;

        public const uint
            LucidDreaming = 1204;

        public static class Buffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Placeholder = 0;
        }
#if DEBUG
        internal class DEBUG : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DEBUG;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (InCombat())
                {
                    if (LocalPlayer.TargetObject is BattleChara chara)
                    {
                        foreach (var status in chara.StatusList)
                        {
                            Dalamud.Logging.PluginLog.Debug($"TARGET STATUS CHECK: {chara.Name} -> {status.StatusId}");
                        }
                    }
                    foreach (var status in (LocalPlayer as BattleChara).StatusList)
                    {
                        Dalamud.Logging.PluginLog.Debug($"SELF STATUS CHECK: {LocalPlayer.Name} -> {status.StatusId}");
                    }

                    Dalamud.Logging.PluginLog.Debug($"TARGET OBJECT KIND: {LocalPlayer.TargetObject?.ObjectKind}");
                    Dalamud.Logging.PluginLog.Debug($"PLAYER OBJECT KIND: {LocalPlayer.ObjectKind}");
                    Dalamud.Logging.PluginLog.Debug($"TARGET IS BATTLE CHARA: {LocalPlayer.TargetObject is BattleChara}");
                    Dalamud.Logging.PluginLog.Debug($"PLAYER IS BATTLE CHARA: {LocalPlayer is BattleChara}");
                    Dalamud.Logging.PluginLog.Debug($"IN MELEE RANGE: {InMeleeRange()}");
                    Dalamud.Logging.PluginLog.Debug($"LAST COMBO MOVE: {lastComboActionID}");
                    Dalamud.Logging.PluginLog.Debug($"IN PVP ZONE: {Service.ClientState.IsPvP}");
                }
                return actionID;
            }
        }
#endif

    }
}