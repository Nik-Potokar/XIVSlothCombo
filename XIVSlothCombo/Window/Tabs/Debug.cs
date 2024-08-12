using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Linq;
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

            // Custom Styling
            static void CustomStyleText(string label, object? value)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    ImGui.TextUnformatted(label);
                    ImGui.SameLine(0, 4f);
                }
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                ImGui.TextUnformatted(value?.ToString() ?? "");
                ImGui.PopStyleColor();
            }

            if (LocalPlayer != null)
            {
                // Player Status Effects
                if (ImGui.CollapsingHeader("Player Status Effects"))
                {
                    foreach (Status? status in Svc.ClientState.LocalPlayer.StatusList)
                    {
                        // Null Check (Source Name)
                        if (status.SourceObject is not null)
                        {
                            ImGui.TextUnformatted($"{status.SourceObject.Name} ->");
                            ImGui.SameLine(0, 4f);
                        }

                        // Null Check (Status Name)
                        if (!string.IsNullOrEmpty(ActionWatching.GetStatusName(status.StatusId)))
                        {
                            CustomStyleText(ActionWatching.GetStatusName(status.StatusId) + ":", status.StatusId);
                            ImGui.SameLine(0, 4f);
                            CustomStyleText("", $"({Math.Round(status.RemainingTime, 1)})");
                        }
                        else
                        {
                            CustomStyleText("", $"{status.StatusId} ({Math.Round(status.RemainingTime, 1)})");
                        }
                    }
                }

                // Target Status Effects
                if (ImGui.CollapsingHeader("Target Status Effects"))
                {
                    if (Svc.ClientState.LocalPlayer.TargetObject is IBattleChara chara)
                    {
                        foreach (Status? status in chara.StatusList)
                        {
                            // Null Check (Source Name)
                            if (status.SourceObject is not null)
                            {
                                ImGui.TextUnformatted($"{status.SourceObject.Name} ->");
                                ImGui.SameLine(0, 4f);
                            }

                            // Null Check (Status Name)
                            if (!string.IsNullOrEmpty(ActionWatching.GetStatusName(status.StatusId)))
                            {
                                CustomStyleText(ActionWatching.GetStatusName(status.StatusId) + ":", status.StatusId);
                                ImGui.SameLine(0, 4f);
                                CustomStyleText("", $"({Math.Round(status.RemainingTime, 1)})");
                            }
                            else
                            {
                                CustomStyleText("", $"{status.StatusId} ({Math.Round(status.RemainingTime, 1)})");
                            }
                        }

                    }
                }
                ImGui.Spacing();

                // Player Info
                ImGui.Spacing();
                ImGui.Text("Player Info");
                ImGui.Separator();
                CustomStyleText("Job:", $"{Svc.ClientState.LocalPlayer.ClassJob.GameData.NameEnglish} (ID: {Svc.ClientState.LocalPlayer.ClassJob.Id})");
                CustomStyleText("Zone:", $"{Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?.FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name} (ID: {Svc.ClientState.TerritoryType})");
                CustomStyleText("In PvP:", CustomComboFunctions.InPvP());
                CustomStyleText("In Combat:", CustomComboFunctions.InCombat());
                CustomStyleText("Hitbox Radius:", Svc.ClientState.LocalPlayer.HitboxRadius);
                ImGui.Spacing();

                // Target Info
                ImGui.Spacing();
                ImGui.Text("Target Info");
                ImGui.Separator();
                CustomStyleText("ObjectId:", Svc.ClientState.LocalPlayer.TargetObject?.GameObjectId);
                CustomStyleText("ObjectKind:", Svc.ClientState.LocalPlayer.TargetObject?.ObjectKind);
                CustomStyleText("Is BattleChara:", Svc.ClientState.LocalPlayer.TargetObject is IBattleChara);
                CustomStyleText("Is PlayerCharacter:", Svc.ClientState.LocalPlayer.TargetObject is IPlayerCharacter);
                CustomStyleText("Distance:", $"{Math.Round(CustomComboFunctions.GetTargetDistance(), 2)}y");
                CustomStyleText("Hitbox Radius:", Svc.ClientState.LocalPlayer.TargetObject?.HitboxRadius);
                CustomStyleText("In Melee Range:", CustomComboFunctions.InMeleeRange());
                CustomStyleText("Relative Direction:", CustomComboFunctions.AngleToTarget() == 2 ? "Rear" : (CustomComboFunctions.AngleToTarget() == 1 || CustomComboFunctions.AngleToTarget() == 3) ? "Flank" : CustomComboFunctions.AngleToTarget() == 4 ? "Front" : "");
                CustomStyleText("Health:", $"{CustomComboFunctions.EnemyHealthCurrentHp().ToString("N0")} / {CustomComboFunctions.EnemyHealthMaxHp().ToString("N0")} ({Math.Round(CustomComboFunctions.GetTargetHPPercent(), 2)}%)");
                ImGui.Spacing();

                // Action Info
                ImGui.Spacing();
                ImGui.Text("Action Info");
                ImGui.Separator();
                CustomStyleText("Last Action:", ActionWatching.LastAction == 0 ? string.Empty : $"{ActionWatching.GetActionName(ActionWatching.LastAction)} (ID: {ActionWatching.LastAction})");
                CustomStyleText("Last Action Cost:", CustomComboFunctions.GetResourceCost(ActionWatching.LastAction));
                CustomStyleText("Last Action Type:", ActionWatching.GetAttackType(ActionWatching.LastAction));
                CustomStyleText("Last Weaponskill:", ActionWatching.GetActionName(ActionWatching.LastWeaponskill));
                CustomStyleText("Last Spell:", ActionWatching.GetActionName(ActionWatching.LastSpell));
                CustomStyleText("Last Ability:", ActionWatching.GetActionName(ActionWatching.LastAbility));
                CustomStyleText("Combo Timer:", Math.Round(CustomComboFunctions.ComboTimer, 1));
                CustomStyleText("Combo Action:", CustomComboFunctions.ComboAction == 0 ? string.Empty : $"{ActionWatching.GetActionName(CustomComboFunctions.ComboAction)} (ID: {CustomComboFunctions.ComboAction})");
                CustomStyleText("Cast Action:", Svc.ClientState.LocalPlayer.CastActionId == 0 ? string.Empty : $"{ActionWatching.GetActionName(Svc.ClientState.LocalPlayer.CastActionId)} (ID: {Svc.ClientState.LocalPlayer.CastActionId})");
                CustomStyleText("Cast Time (Total):", Math.Round(Svc.ClientState.LocalPlayer.TotalCastTime, 2));
                CustomStyleText("Cast Time (Current):", Math.Round(Svc.ClientState.LocalPlayer.CurrentCastTime, 2));
                ImGui.Spacing();

                // Party Info
                ImGui.Spacing();
                ImGui.Text("Party Info");
                ImGui.Separator();
                CustomStyleText("Party ID:", Svc.Party.PartyId);
                CustomStyleText("Party Size:", Svc.Party.Length);
                if (ImGui.CollapsingHeader("Party Members"))
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        if (CustomComboFunctions.GetPartySlot(i) is not IBattleChara member || member is null) continue;
                        ImGui.TextUnformatted($"Slot {i} ->");
                        ImGui.SameLine(0, 4f);
                        CustomStyleText($"{CustomComboFunctions.GetPartySlot(i).Name}", $"({member.ClassJob.GameData.Abbreviation})");
                    }
                }
                ImGui.Spacing();

                // Misc. Info
                ImGui.Spacing();
                ImGui.Text("Miscellaneous Info");
                ImGui.Separator();
                if (ImGui.CollapsingHeader("Active Blue Mage Spells"))
                {
                    ImGui.TextUnformatted($"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                }
            }

            else
            {
                ImGui.TextUnformatted("Please log into the game to use this tab.");
            }
        }
    }
}