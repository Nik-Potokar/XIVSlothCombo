using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Linq;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Functions
{
    internal class Presets : ConfigWindow
    {
        internal static void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
        {
            var enabled = Service.Configuration.IsEnabled(preset);
            var secret = PluginConfiguration.IsSecret(preset);
            var conflicts = Service.Configuration.GetConflicts(preset);
            var parent = PluginConfiguration.GetParent(preset);
            var blueAttr = preset.GetAttribute<BlueInactiveAttribute>();

            ImGui.PushItemWidth(200);

            if (ImGui.Checkbox($"{info.FancyName}###{i}", ref enabled))
            {
                if (enabled)
                {
                    EnableParentPresets(preset);
                    Service.Configuration.EnabledActions.Add(preset);
                    foreach (var conflict in conflicts)
                    {
                        Service.Configuration.EnabledActions.Remove(conflict);
                    }
                }

                else
                {
                    Service.Configuration.EnabledActions.Remove(preset);
                }

                Service.Configuration.Save();
            }

            ImGui.PopItemWidth();
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);

            DrawOpenerButtons(preset);

            ImGui.Text($"#{i}: ");
            var length = ImGui.CalcTextSize($"#{i}: ");
            ImGui.SameLine();
            ImGui.PushItemWidth(length.Length());
            ImGui.TextWrapped($"{info.Description}");

            if (preset.GetHoverAttribute() != null)
            {
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(preset.GetHoverAttribute().HoverText);
                    ImGui.EndTooltip();
                }
            }


            ImGui.PopStyleColor();
            ImGui.Spacing();

            UserConfigItems.Draw(preset, enabled);

            if (preset == CustomComboPreset.NIN_ST_SimpleMode || preset == CustomComboPreset.NIN_ST_AdvancedMode)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
                if (ImGui.Button($"Image of rotation###ninrtn{i}"))
                {
                    Util.OpenLink("https://i.imgur.com/q3lXeSZ.png");
                }
            }

				if (preset == CustomComboPreset.MNK_ST_SimpleMode)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###mnkrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/C5lQhpe.png");
				}
			}

				if (preset == CustomComboPreset.AST_ST_DPS)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###astrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/kc55j4l.jpg");
				}
			}

				if (preset == CustomComboPreset.BRD_ST_SimpleMode)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###brdrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/9DByoqX.png");
				}
			}

				if (preset == CustomComboPreset.BLM_SimpleMode || preset == CustomComboPreset.BLM_Simple_Opener || preset == CustomComboPreset.BLM_Simple_Transpose)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###blmrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/SUCx9g9.png");
				}
			}

				if (preset == CustomComboPreset.DNC_ST_SimpleMode)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###dncrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/UmEGDGR.png");
				}
			}

				if (preset == CustomComboPreset.DRK_SouleaterCombo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###drkrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/QdBsQzA.png");
				}
			}

				if (preset == CustomComboPreset.DRG_ST_Opener)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###drgrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/hVm8Rwl.png");
				}
			}

				if (preset == CustomComboPreset.GNB_ST_MainCombo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###gnbrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/EEU0YWy.png");
				}
			}

				if (preset == CustomComboPreset.MCH_ST_SimpleMode)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###mchrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/1DsgLAQ.png");
				}
			}

				if (preset == CustomComboPreset.PLD_ST_RoyalAuth)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###pldrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/Yf34qdj.png");
				}
			}

				if (preset == CustomComboPreset.RPR_ST_SliceCombo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###rprrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/jyqYKSy.png");
				}
			}

				if (preset == CustomComboPreset.RDM_Balance_Opener)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###rdmrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/CBVDi07.png");
				}
			}

				if (preset == CustomComboPreset.SGE_ST_DPS)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###sgertn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/0uHdVeF.png");
				}
			}

				if (preset == CustomComboPreset.SAM_ST_GekkoCombo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###samrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/wngT2Ar.png");
				}
			}

				if (preset == CustomComboPreset.SCH_DPS)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###schrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/JzYRqjO.png");
				}
			}

				if (preset == CustomComboPreset.SMN_Simple_Combo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###smnrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/mboSORP.png");
				}
			}

				if (preset == CustomComboPreset.WAR_ST_StormsPath)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###warrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/2fehGXL.png");
				}
			}

				if (preset == CustomComboPreset.WHM_ST_MainCombo)
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + length.Length());
				if (ImGui.Button($"Image of rotation###whmrtn{i}"))
				{
					Util.OpenLink("https://i.imgur.com/VtQBAzP.png");
				}
			}

            if (conflicts.Length > 0)
            {
                var conflictText = conflicts.Select(conflict =>
                {
                    var conflictInfo = conflict.GetComboAttribute();

                    return $"\n - {conflictInfo.FancyName}";


                }).Aggregate((t1, t2) => $"{t1}{t2}");

                if (conflictText.Length > 0)
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, $"Conflicts with: {conflictText}");
                    ImGui.Spacing();
                }
            }

            if (blueAttr != null)
            {
                if (blueAttr.Actions.Count > 0)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudOrange);
                    ImGui.Text($"Missing active spells: {string.Join(", ", blueAttr.Actions.Select(x => ActionWatching.GetActionName(x)))}");
                    ImGui.PopStyleColor();
                }

                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    ImGui.Text($"All required spells active!");
                    ImGui.PopStyleColor();
                }
            }

            i++;

            var hideChildren = Service.Configuration.HideChildren;
            var children = presetChildren[preset];

            if (children.Length > 0)
            {
                if (enabled || !hideChildren)
                {
                    ImGui.Indent();

                    foreach (var (childPreset, childInfo) in children)
                    {
                        if (Service.Configuration.HideConflictedCombos)
                        {
                            var conflictOriginals = Service.Configuration.GetConflicts(childPreset);    // Presets that are contained within a ConflictedAttribute
                            var conflictsSource = Service.Configuration.GetAllConflicts();              // Presets with the ConflictedAttribute

                            if (!conflictsSource.Where(x => x == childPreset || x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                DrawPreset(childPreset, childInfo, ref i);
                                continue;
                            }

                            if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                            {
                                Service.Configuration.EnabledActions.Remove(childPreset);
                                Service.Configuration.Save();
                            }

                            else
                            {
                                DrawPreset(childPreset, childInfo, ref i);
                                continue;
                            }
                        }

                        else
                        {
                            DrawPreset(childPreset, childInfo, ref i);
                        }
                    }

                    ImGui.Unindent();
                }   
                else
                {
                    i += AllChildren(presetChildren[preset]);

                }
            }
        }

        private static void DrawOpenerButtons(CustomComboPreset preset)
        {
            if (preset.GetReplaceAttribute() != null)
            {
                string skills = string.Join(", ", preset.GetReplaceAttribute().ActionNames);

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted($"Replaces: {skills}");
                    ImGui.EndTooltip();
                }
            }
        }

        internal static int AllChildren((CustomComboPreset Preset, CustomComboInfoAttribute Info)[] children)
        {
            var output = 0;

            foreach (var (Preset, Info) in children)
            {
                output++;
                output += AllChildren(presetChildren[Preset]);
            }

            return output;
        }

        /// <summary> Iterates up a preset's parent tree, enabling each of them. </summary>
        /// <param name="preset"> Combo preset to enabled. </param>
        private static void EnableParentPresets(CustomComboPreset preset)
        {
            var parentMaybe = PluginConfiguration.GetParent(preset);

            while (parentMaybe != null)
            {
                var parent = parentMaybe.Value;

                if (!Service.Configuration.EnabledActions.Contains(parent))
                {
                    Service.Configuration.EnabledActions.Add(parent);
                    foreach (var conflict in Service.Configuration.GetConflicts(parent))
                    {
                        Service.Configuration.EnabledActions.Remove(conflict);
                    }
                }

                parentMaybe = PluginConfiguration.GetParent(parent);
            }
        }
    }
}
