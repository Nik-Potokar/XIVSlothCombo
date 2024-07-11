using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
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
            IPlayerCharacter? LocalPlayer = Service.ClientState.LocalPlayer;
            DebugCombo? comboClass = new();

            if (LocalPlayer != null)
            {
                if (Service.ClientState.LocalPlayer.TargetObject is IBattleChara chara)
                {
                    foreach (Status? status in chara.StatusList)
                    {
                        ImGui.TextUnformatted($"TARGET STATUS CHECK: {chara.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId} {Math.Round(status.RemainingTime, 1)}");
                    }
                }

                foreach (Status? status in (Service.ClientState.LocalPlayer as IBattleChara).StatusList)
                {
                    ImGui.TextUnformatted($"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId} {Math.Round(status.RemainingTime, 1)}");
                }

                ImGui.TextUnformatted($"TERRITORY: {Service.ClientState.TerritoryType}");
                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Service.ClientState.LocalPlayer.TargetObject?.ObjectKind}");
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Service.ClientState.LocalPlayer.TargetObject is IBattleChara}");
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
                ImGui.TextUnformatted($"ZONE: {Service.ClientState.TerritoryType}");
                ImGui.BeginChild("BLUSPELLS", new Vector2(250, 100), false);
                ImGui.TextUnformatted($"SELECTED BLU SPELLS:\n{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                ImGui.EndChild();

                var gauge = CustomComboFunctions.GetJobGauge<ASTGauge>();
                ImGui.Text($"{gauge.DrawnCards[0]}");
                ImGui.Text($"{gauge.DrawnCards[1]}");
                ImGui.Text($"{gauge.DrawnCards[2]}");
                ImGui.Text($"{gauge.ActiveDraw}");
                ImGui.Text($"{gauge.DrawnCrownCard}");

                ImGui.Text($"{CustomComboFunctions.GetPartySlot(1).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(2).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(3).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(4).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(5).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(6).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(7).Name}");
                ImGui.Text($"{CustomComboFunctions.GetPartySlot(8).Name}");
            }

            else
            {
                ImGui.TextUnformatted("Please log in to use this tab.");
            }
        }
    }
}