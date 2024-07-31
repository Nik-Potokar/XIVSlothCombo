using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Combos;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using Status = Dalamud.Game.ClientState.Statuses.Status;


namespace XIVSlothCombo.Window.Tabs
{

    internal class Debug : ConfigWindow
    {

        internal class DebugCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; }

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) => actionID;
        }

        public static int debugNum = 0;
        internal unsafe static new void Draw()
        {
            IPlayerCharacter? LocalPlayer = Svc.ClientState.LocalPlayer;
            DebugCombo? comboClass = new();

            if (LocalPlayer != null)
            {
                if (Svc.ClientState.LocalPlayer.TargetObject is IBattleChara chara)
                {
                    foreach (Status? status in chara.StatusList)
                    {
                        ImGui.TextUnformatted($"TARGET STATUS CHECK: {chara.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId} {Math.Round(status.RemainingTime, 1)}");
                    }
                }

                foreach (Status? status in (Svc.ClientState.LocalPlayer as IBattleChara).StatusList)
                {
                    ImGui.TextUnformatted($"SELF STATUS CHECK: {Svc.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId} {Math.Round(status.RemainingTime, 1)}");
                }

                ImGui.TextUnformatted($"TERRITORY: {Svc.ClientState.TerritoryType}");
                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Svc.ClientState.LocalPlayer.TargetObject?.ObjectKind}");
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Svc.ClientState.LocalPlayer.TargetObject is IBattleChara}");
                ImGui.TextUnformatted($"IN COMBAT: {CustomComboFunctions.InCombat()}");
                ImGui.TextUnformatted($"IN MELEE RANGE: {CustomComboFunctions.InMeleeRange()}");
                ImGui.TextUnformatted($"DISTANCE FROM TARGET: {CustomComboFunctions.GetTargetDistance()}");
                ImGui.TextUnformatted($"TARGET HP VALUE: {CustomComboFunctions.EnemyHealthCurrentHp()}");
                ImGui.TextUnformatted($"LAST ACTION: {ActionWatching.GetActionName(ActionWatching.LastAction)} (ID:{ActionWatching.LastAction})");
                ImGui.TextUnformatted($"LAST ACTION COST: {CustomComboFunctions.GetResourceCost(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST ACTION TYPE: {ActionWatching.GetAttackType(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST WEAPONSKILL: {ActionWatching.GetActionName(ActionWatching.LastWeaponskill)}");
                ImGui.TextUnformatted($"LAST SPELL: {ActionWatching.GetActionName(ActionWatching.LastSpell)}");
                ImGui.TextUnformatted($"LAST ABILITY: {ActionWatching.GetActionName(ActionWatching.LastAbility)}");
                ImGui.TextUnformatted($"ZONE: {Svc.ClientState.TerritoryType}");
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