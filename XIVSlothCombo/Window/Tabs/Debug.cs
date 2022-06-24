using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Combos;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

#if DEBUG
namespace XIVSlothCombo.Window.Tabs
{

    internal class Debug : ConfigWindow
    {

        internal class DebugCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; }

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                return actionID;
            }
        }

        internal static new void Draw()
        {
            var LocalPlayer = Service.ClientState.LocalPlayer;
            var comboClass = new DebugCombo();

            if (LocalPlayer != null)
            {
                if (Service.ClientState.LocalPlayer.TargetObject is BattleChara chara)
                {
                    foreach (var status in chara.StatusList)
                    {
                        ImGui.TextUnformatted($"TARGET STATUS CHECK: {chara.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                    }
                }

                foreach (var status in (Service.ClientState.LocalPlayer as BattleChara).StatusList)
                {
                    ImGui.TextUnformatted($"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                }

                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Service.ClientState.LocalPlayer.TargetObject?.ObjectKind}");
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Service.ClientState.LocalPlayer.TargetObject is BattleChara}");
                ImGui.TextUnformatted($"PLAYER IS BATTLE CHARA: {LocalPlayer is BattleChara}");
                ImGui.TextUnformatted($"IN COMBAT: {CustomComboNS.Functions.CustomComboFunctions.InCombat()}");
                ImGui.TextUnformatted($"IN MELEE RANGE: {comboClass.InMeleeRange()}");
                ImGui.TextUnformatted($"DISTANCE FROM TARGET: {comboClass.GetTargetDistance()}");
                ImGui.TextUnformatted($"TARGET HP VALUE: {CustomComboNS.Functions.CustomComboFunctions.EnemyHealthCurrentHp()}");
                ImGui.TextUnformatted($"LAST ACTION: {ActionWatching.GetActionName(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST ACTION COST: {CustomComboNS.Functions.CustomComboFunctions.GetResourceCost(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST ACTION TYPE: {ActionWatching.GetAttackType(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST WEAPONSKILL: {ActionWatching.GetActionName(ActionWatching.LastWeaponskill)}");
                ImGui.TextUnformatted($"LAST SPELL: {ActionWatching.GetActionName(ActionWatching.LastSpell)}");
                ImGui.TextUnformatted($"LAST ABILITY: {ActionWatching.GetActionName(ActionWatching.LastAbility)}");
                ImGui.TextUnformatted($"ZONE: {Service.ClientState.TerritoryType}");
                ImGui.BeginChild("BLUSPELLS", new Vector2(250, 100), false);
                ImGui.TextUnformatted($"SELECTED BLU SPELLS:\n{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                ImGui.EndChild();
            }

            else
            {
                ImGui.TextUnformatted("Please log in to use this tab.");
            }
        }
    }

}
#endif