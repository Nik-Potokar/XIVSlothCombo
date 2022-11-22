using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ImGuiNET;
using XIVSlothCombo.Combos;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
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

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) => actionID;
        }

        internal static new void Draw()
        {
            PlayerCharacter? LocalPlayer = Service.ClientState.LocalPlayer;
            DebugCombo? comboClass = new();

            if (LocalPlayer != null)
            {		
                if (ImGui.TreeNode("Colorful Text"))
		        {
                    foreach (Status? status in (Service.ClientState.LocalPlayer as BattleChara).StatusList) // Lists Players current active status
                    {
                      ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), $"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                    }
			        ImGui.TreePop();
		        }
                ImGui.Separator();

                foreach (Status? status in (Service.ClientState.LocalPlayer as BattleChara).StatusList) // Lists Players current active status
                {
                    ImGui.TextUnformatted($"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                }

                ImGui.Separator();
                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Service.ClientState.LocalPlayer.TargetObject?.ObjectKind}"); // Possible entity kinds: (None) (Player) (BattleNpc) (EventNpc) (Treasure) (Aetheryte) (GatheringPoint) (EventObj) (MountType) (Companion) (Retainer) (Area) (Housing) (Cutscene) (CardStand) 
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Service.ClientState.LocalPlayer.TargetObject is BattleChara}");
                ImGui.TextUnformatted($"PLAYER IS BATTLE CHARA: {LocalPlayer is BattleChara}");
                ImGui.TextUnformatted($"IN COMBAT: {CustomComboFunctions.InCombat()}");
                ImGui.Separator();
                ImGui.TextUnformatted($"IN MELEE RANGE: {CustomComboFunctions.InMeleeRange()}");
                ImGui.TextUnformatted($"DISTANCE FROM TARGET: {CustomComboFunctions.GetTargetDistance()}");
                ImGui.TextUnformatted($"TARGET HP VALUE: {CustomComboFunctions.EnemyHealthCurrentHp()}");
                ImGui.Separator();
                ImGui.TextUnformatted($"LAST ACTION: {ActionWatching.GetActionName(ActionWatching.LastAction)} (ID:{ActionWatching.LastAction})");
                ImGui.TextUnformatted($"LAST ACTION COST: {CustomComboFunctions.GetResourceCost(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST ACTION TYPE: {ActionWatching.GetAttackType(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST WEAPONSKILL: {ActionWatching.GetActionName(ActionWatching.LastWeaponskill)}");
                ImGui.TextUnformatted($"LAST SPELL: {ActionWatching.GetActionName(ActionWatching.LastSpell)}");
                ImGui.TextUnformatted($"LAST ABILITY: {ActionWatching.GetActionName(ActionWatching.LastAbility)}");
                ImGui.Separator();
                ImGui.TextUnformatted($"ZONE: {Service.ClientState.TerritoryType}");
                ImGui.TextUnformatted($"Current Zone: {Service.ClientState.TerritoryType}");
                ImGui.Separator();
                ImGui.BeginChild("BLUSPELLS", new Vector2(250, 150), true, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.TextUnformatted($"SELECTED BLU SPELLS:");
                ImGui.Separator();
                ImGui.TextUnformatted($"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                ImGui.EndChild();

                ImGui.TextUnformatted($"Number Of NQ Potions: {CustomComboFunctions.NumberOfNQPotions(12669)}");
                ImGui.TextUnformatted($"Number Of HQ Potions: {CustomComboFunctions.NumberOfHQPotions(12669)}");

                ImGui.TextUnformatted($"Number Of HQ Grade 7 Dex remaining: {CustomComboFunctions.NumberOfHQPotions(37841)}");
                ImGui.TextUnformatted($"Number of HQ Carrot Pudding remaining: {CustomComboFunctions.NumberOfHQPotions(38264)}");

            }
            else
            {
                ImGui.TextUnformatted("Please log in to use this tab.");
            }
        }
    }
}
#endif