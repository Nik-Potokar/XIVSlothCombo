using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using ImGuiNET;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using Dalamud.Interface.Colors;

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
                ImGui.Separator();
                ImGui.TextUnformatted($"TARGET INFO:");
                ImGui.TextUnformatted($"TARGET OBJECT KIND: {Service.ClientState.LocalPlayer.TargetObject?.ObjectKind}"); // Possible entity kinds: (None) (Player) (BattleNpc) (EventNpc) (Treasure) (Aetheryte) (GatheringPoint) (EventObj) (MountType) (Companion) (Retainer) (Area) (Housing) (Cutscene) (CardStand) 
                ImGui.TextUnformatted($"TARGET IS BATTLE CHARA: {Service.ClientState.LocalPlayer.TargetObject is BattleChara}");
                ImGui.TextUnformatted($"TARGET HP VALUE: {CustomComboFunctions.EnemyHealthCurrentHp()}");
                ImGui.Separator();

                ImGui.TextUnformatted($"PLAYER INFO:");
                ImGui.TextUnformatted($"PLAYER IS BATTLE CHARA: {LocalPlayer is BattleChara}");
                ImGui.TextUnformatted($"IN COMBAT: {CustomComboFunctions.InCombat()}");
                ImGui.TextUnformatted($"IN MELEE RANGE: {CustomComboFunctions.InMeleeRange()}");
                ImGui.TextUnformatted($"DISTANCE FROM TARGET: {CustomComboFunctions.GetTargetDistance()}");
                if (ImGui.TreeNode("STATUS CHECK"))
                { 
                    foreach (Status? status in (Service.ClientState.LocalPlayer as BattleChara).StatusList) // Lists Players current active status
                    {
                        ImGui.TextColored(ImGuiColors.DalamudYellow, $"SELF STATUS CHECK: {Service.ClientState.LocalPlayer.Name} -> {ActionWatching.GetStatusName(status.StatusId)}: {status.StatusId}");
                    }
                    ImGui.TreePop();
                }
                ImGui.Separator();

                ImGui.TextUnformatted($"SUPPLIES CHECK:");
                if (ImGui.TreeNode("POTION INFO"))
                { 
                    ImGui.TextUnformatted($"Number Of HQ Grade 7 Dex remaining: {CustomComboFunctions.NumberOfHQPotions(37841)}");
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("FOOD INFO"))
                { 
                    ImGui.TextUnformatted($"Number of HQ Carrot Pudding remaining: {CustomComboFunctions.NumberOfItems(38264)}");
                    ImGui.TreePop();
                }
                
                ImGui.Separator();

                ImGui.TextUnformatted($"ACTION INFO:");
                ImGui.TextUnformatted($"LAST ACTION: {ActionWatching.GetActionName(ActionWatching.LastAction)} (ID:{ActionWatching.LastAction})");
                ImGui.TextUnformatted($"LAST ACTION COST: {CustomComboFunctions.GetResourceCost(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST ACTION TYPE: {ActionWatching.GetAttackType(ActionWatching.LastAction)}");
                ImGui.TextUnformatted($"LAST WEAPONSKILL: {ActionWatching.GetActionName(ActionWatching.LastWeaponskill)}");
                ImGui.TextUnformatted($"LAST SPELL: {ActionWatching.GetActionName(ActionWatching.LastSpell)}");
                ImGui.TextUnformatted($"LAST ABILITY: {ActionWatching.GetActionName(ActionWatching.LastAbility)}");
                ImGui.Separator();

                ImGui.TextUnformatted($"LOCATION:");
                ImGui.TextUnformatted($"Current Zone: {Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?.FirstOrDefault(x => x.RowId == Service.ClientState.TerritoryType).PlaceName.Value.Name}");   // Current zone location
                ImGui.TextUnformatted($"Current Zone ID: {Service.ClientState.TerritoryType}");
                ImGui.Separator();

                ImGui.BeginChild("BLUSPELLS", new Vector2(250, -1), false, ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollWithMouse);
                if (ImGui.CollapsingHeader($"SELECTED BLU SPELLS"))
                { 
                    ImGui.BeginChild("LISTBLUESPELLS", new Vector2(-1,150), true);// , ImGuiWindowFlags.AlwaysAutoResize);
                    ImGui.TextUnformatted($"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                    ImGui.EndChild();
                }
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