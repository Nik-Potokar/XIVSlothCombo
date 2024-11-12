using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Linq;
using XIVSlothCombo.Combos;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using Action = Lumina.Excel.Sheets.Action;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.Window.Tabs
{
    internal class Debug : ConfigWindow
    {
        public static int debugNum = 0;

        internal class DebugCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; }
            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) => actionID;
        }

        internal static Action? debugSpell;
        internal unsafe static new void Draw()
        {
            DebugCombo? comboClass = new();
            IPlayerCharacter? LocalPlayer = Svc.ClientState.LocalPlayer;
            uint[] statusBlacklist = { 360, 361, 362, 363, 364, 365, 366, 367, 368 }; // Duration will not be displayed for these status effects

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
                    foreach (Status? status in LocalPlayer.StatusList)
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
                        }
                        else CustomStyleText("", status.StatusId);

                        // Duration + Blacklist Check
                        float buffDuration = CustomComboFunctions.GetBuffRemainingTime((ushort)status.StatusId, false);
                        if (buffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                        {
                            string formattedDuration;
                            if (buffDuration >= 60)
                            {
                                int minutes = (int)(buffDuration / 60);
                                formattedDuration = $"{minutes}m";
                            }
                            else formattedDuration = $"{buffDuration:F1}s";

                            ImGui.SameLine(0, 4f);
                            CustomStyleText("", $"({formattedDuration})");
                        }
                    }
                }

                // Target Status Effects
                if (ImGui.CollapsingHeader("Target Status Effects"))
                {
                    if (LocalPlayer.TargetObject is IBattleChara chara)
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
                            }
                            else CustomStyleText("", status.StatusId);

                            // Duration + Blacklist Check
                            float debuffDuration = CustomComboFunctions.GetDebuffRemainingTime((ushort)status.StatusId, false);
                            if (debuffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                            {
                                string formattedDuration;
                                if (debuffDuration >= 60)
                                {
                                    int minutes = (int)(debuffDuration / 60);
                                    formattedDuration = $"{minutes}m";
                                }
                                else formattedDuration = $"{debuffDuration:F1}s";

                                ImGui.SameLine(0, 4f);
                                CustomStyleText("", $"({formattedDuration})");
                            }
                        }

                    }
                }
                if (ImGui.CollapsingHeader("Action Info"))
                {
                    string prev = debugSpell == null ? "Select Action" : $"({debugSpell.RowId}) Lv.{debugSpell.ClassJobLevel}. {debugSpell.Name} - {(debugSpell.IsPvP ? "PvP" : "Normal")}";
                    ImGuiEx.SetNextItemFullWidth();
                    using (var comboBox = ImRaii.Combo("###ActionCombo", prev))
                    {
                        if (comboBox)
                        {
                            if (ImGui.Selectable("", debugSpell == null))
                            {
                                debugSpell = null;
                            }

                            var classId = CustomComboFunctions.JobIDs.JobToClass(JobID.Value);
                            foreach (var act in Svc.Data.GetExcelSheet<Action>().Where(x => x.IsPlayerAction && x.ClassJob.Row == classId || x.ClassJob.Row == JobID.Value).OrderBy(x => x.ClassJobLevel))
                            {
                                if (ImGui.Selectable($"({act.RowId}) Lv.{act.ClassJobLevel}. {act.Name} - {(act.IsPvP ? "PvP" : "Normal")}", debugSpell?.RowId == act.RowId))
                                {
                                    debugSpell = act;
                                }
                            }
                        }
                    }

                    if (debugSpell != null)
                    {
                        var actionStatus = ActionManager.Instance()->GetActionStatus(ActionType.Action, debugSpell.RowId);
                        var icon = Svc.Texture.GetFromGameIcon(new(debugSpell.Icon)).GetWrapOrEmpty().ImGuiHandle;
                        ImGui.Image(icon, new System.Numerics.Vector2(60f.Scale(), 60f.Scale()));
                        ImGui.SameLine();
                        ImGui.Image(icon, new System.Numerics.Vector2(30f.Scale(), 30f.Scale()));
                        CustomStyleText($"Action Status:", $"{actionStatus} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(actionStatus).Text})");
                        CustomStyleText($"Action Type:", debugSpell.ActionCategory.Value.Name);
                        if (debugSpell.UnlockLink != 0)
                        CustomStyleText($"Quest:", $"{Svc.Data.GetExcelSheet<Quest>().GetRow(debugSpell.UnlockLink).Name} ({(UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(debugSpell.UnlockLink) ? "Completed" : "Not Completed")})");
                        CustomStyleText($"Base Recast:", $"{debugSpell.Recast100ms / 10f}s");
                        CustomStyleText($"Max Charges:", $"{debugSpell.MaxCharges}");
                        if (ActionWatching.ActionTimestamps.ContainsKey(debugSpell.RowId))
                            CustomStyleText($"Time Since Last Use:", $"{(Environment.TickCount64 - ActionWatching.ActionTimestamps[debugSpell.RowId])/1000f:F2}");
                    }
                }

                // Player Info
                ImGui.Spacing();
                ImGui.Text("Player Info");
                ImGui.Separator();
                CustomStyleText("Job:", $"{LocalPlayer.ClassJob.GameData.NameEnglish} (ID: {LocalPlayer.ClassJob.RowId})");
                CustomStyleText("Zone:", $"{Svc.Data.GetExcelSheet<TerritoryType>()?.FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name} (ID: {Svc.ClientState.TerritoryType})");
                CustomStyleText("In PvP:", CustomComboFunctions.InPvP());
                CustomStyleText("In Combat:", CustomComboFunctions.InCombat());
                CustomStyleText("Hitbox Radius:", LocalPlayer.HitboxRadius);
                ImGui.Spacing();

                // Target Info
                ImGui.Spacing();
                ImGui.Text("Target Info");
                ImGui.Separator();
                CustomStyleText("ObjectId:", LocalPlayer.TargetObject?.GameObjectId);
                CustomStyleText("ObjectKind:", LocalPlayer.TargetObject?.ObjectKind);
                CustomStyleText("Is BattleChara:", LocalPlayer.TargetObject is IBattleChara);
                CustomStyleText("Is PlayerCharacter:", LocalPlayer.TargetObject is IPlayerCharacter);
                CustomStyleText("Distance:", $"{Math.Round(CustomComboFunctions.GetTargetDistance(), 2)}y");
                CustomStyleText("Hitbox Radius:", LocalPlayer.TargetObject?.HitboxRadius);
                CustomStyleText("In Melee Range:", CustomComboFunctions.InMeleeRange());
                CustomStyleText("Relative Position:", CustomComboFunctions.AngleToTarget() is 2 ? "Rear" : (CustomComboFunctions.AngleToTarget() is 1 or 3) ? "Flank" : CustomComboFunctions.AngleToTarget() is 4 ? "Front" : "");
                CustomStyleText("Health:", $"{CustomComboFunctions.EnemyHealthCurrentHp().ToString("N0")} / {CustomComboFunctions.EnemyHealthMaxHp().ToString("N0")} ({Math.Round(CustomComboFunctions.GetTargetHPPercent(), 2)}%)");
                ImGui.Spacing();

                // Action Info
                ImGui.Spacing();
                ImGui.Text("Action Info");
                ImGui.Separator();
                CustomStyleText("Last Action:", ActionWatching.LastAction == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(ActionWatching.LastAction)) ? "Unknown" : ActionWatching.GetActionName(ActionWatching.LastAction))} (ID: {ActionWatching.LastAction})");
                CustomStyleText("Last Action Cost:", CustomComboFunctions.GetResourceCost(ActionWatching.LastAction));
                CustomStyleText("Last Action Type:", ActionWatching.GetAttackType(ActionWatching.LastAction));
                CustomStyleText("Last Weaponskill:", ActionWatching.GetActionName(ActionWatching.LastWeaponskill));
                CustomStyleText("Last Spell:", ActionWatching.GetActionName(ActionWatching.LastSpell));
                CustomStyleText("Last Ability:", ActionWatching.GetActionName(ActionWatching.LastAbility));
                CustomStyleText("Combo Timer:", $"{CustomComboFunctions.ComboTimer:F1}");
                CustomStyleText("Combo Action:", CustomComboFunctions.ComboAction == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(CustomComboFunctions.ComboAction)) ? "Unknown" : ActionWatching.GetActionName(CustomComboFunctions.ComboAction))} (ID: {CustomComboFunctions.ComboAction})");
                CustomStyleText("Cast Time:", $"{LocalPlayer.CurrentCastTime:F2} / {LocalPlayer.TotalCastTime:F2}");
                CustomStyleText("Cast Action:", LocalPlayer.CastActionId == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(LocalPlayer.CastActionId)) ? "Unknown" : ActionWatching.GetActionName(LocalPlayer.CastActionId))} (ID: {LocalPlayer.CastActionId})");
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