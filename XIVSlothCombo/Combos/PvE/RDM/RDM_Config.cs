using Dalamud.Interface.Colors;
using ImGuiNET;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Window.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal partial class RDM
    {
        internal static class Config
        {
            public static UserInt
                RDM_VariantCure = new("RDM_VariantCure"),
                RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
                RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
                RDM_AoE_MoulinetRange = new("RDM_MoulinetRange");
            public static UserBool
                RDM_ST_oGCD_OnAction_Adv = new("RDM_ST_oGCD_OnAction_Adv"),
                RDM_ST_oGCD_Fleche = new("RDM_ST_oGCD_Fleche"),
                RDM_ST_oGCD_ContreSixte = new("RDM_ST_oGCD_ContreSixte"),
                RDM_ST_oGCD_Engagement = new("RDM_ST_oGCD_Engagement"),
                RDM_ST_oGCD_Engagement_Pooling = new("RDM_ST_oGCD_Engagement_Pooling"),
                RDM_ST_oGCD_CorpACorps = new("RDM_ST_oGCD_CorpACorps"),
                RDM_ST_oGCD_CorpACorps_Melee = new("RDM_ST_oGCD_CorpACorps_Melee"),
                RDM_ST_oGCD_CorpACorps_Pooling = new("RDM_ST_oGCD_CorpACorps_Pooling"),
                RDM_ST_oGCD_ViceOfThorns = new("RDM_ST_oGCD_ViceOfThorns"),
                RDM_ST_oGCD_Prefulgence = new("RDM_ST_oGCD_Prefulgence"),
                RDM_ST_MeleeCombo_Adv = new("RDM_ST_MeleeCombo_Adv"),
                RDM_ST_MeleeFinisher_Adv = new("RDM_ST_MeleeFinisher_Adv"),
                RDM_ST_MeleeEnforced = new("RDM_ST_MeleeEnforced"),

                RDM_AoE_oGCD_OnAction_Adv = new("RDM_AoE_oGCD_OnAction_Adv"),
                RDM_AoE_oGCD_Fleche = new("RDM_AoE_oGCD_Fleche"),
                RDM_AoE_oGCD_ContreSixte = new("RDM_AoE_oGCD_ContreSixte"),
                RDM_AoE_oGCD_Engagement = new("RDM_AoE_oGCD_Engagement"),
                RDM_AoE_oGCD_Engagement_Pooling = new("RDM_AoE_oGCD_Engagement_Pooling"),
                RDM_AoE_oGCD_CorpACorps = new("RDM_AoE_oGCD_CorpACorps"),
                RDM_AoE_oGCD_CorpACorps_Melee = new("RDM_AoE_oGCD_CorpACorps_Melee"),
                RDM_AoE_oGCD_CorpACorps_Pooling = new("RDM_AoE_oGCD_CorpACorps_Pooling"),
                RDM_AoE_oGCD_ViceOfThorns = new("RDM_AoE_oGCD_ViceOfThorns"),
                RDM_AoE_oGCD_Prefulgence = new("RDM_AoE_oGCD_Prefulgence"),
                RDM_AoE_MeleeCombo_Adv = new("RDM_AoE_MeleeCombo_Adv"),
                RDM_AoE_MeleeFinisher_Adv = new("RDM_AoE_MeleeFinisher_Adv");
            public static UserBoolArray
                RDM_ST_oGCD_OnAction = new("RDM_ST_oGCD_OnAction"),
                RDM_ST_MeleeCombo_OnAction = new("RDM_ST_MeleeCombo_OnAction"),
                RDM_ST_MeleeFinisher_OnAction = new("RDM_ST_MeleeFinisher_OnAction"),

                RDM_AoE_oGCD_OnAction = new("RDM_AoE_oGCD_OnAction"),
                RDM_AoE_MeleeCombo_OnAction = new("RDM_AoE_MeleeCombo_OnAction"),
                RDM_AoE_MeleeFinisher_OnAction = new("RDM_AoE_MeleeFinisher_OnAction");

            internal static void Draw(CustomComboPreset preset)
            {
                if (preset is CustomComboPreset.RDM_ST_oGCD)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_OnAction_Adv, "Advanced Action Options.", "Changes which action this option will replace.", isConditionalChoice: true);
                    if (RDM_ST_oGCD_OnAction_Adv)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Jolts", "", 4, 0, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Fleche", "", 4, 1, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Riposte", "", 4, 2, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Reprise", "", 4, 3, descriptionColor: ImGuiColors.DalamudYellow);
                        ImGui.Unindent();
                    }

                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_Fleche, "Fleche", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_ContreSixte, "Contre Sixte", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                    if (RDM_ST_oGCD_Engagement)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                        ImGui.Unindent();
                    }
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                    if (RDM_ST_oGCD_CorpACorps)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                        UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                        ImGui.Unindent();
                    }
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_ViceOfThorns, "Vice of Thorns", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_oGCD_Prefulgence, "Prefulgence", "");
                }

                if (preset is CustomComboPreset.RDM_ST_MeleeCombo)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                    if (RDM_ST_MeleeCombo_Adv)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, "Jolts", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, "Riposte", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                        ImGui.Unindent();
                    }
                }

                if (preset is CustomComboPreset.RDM_ST_MeleeFinisher)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                    if (RDM_ST_MeleeFinisher_Adv)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "Jolts", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "Riposte", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "VerAero & VerThunder", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                        ImGui.Unindent();
                    }
                }

                if (preset is CustomComboPreset.RDM_ST_Lucid)
                    UserConfig.DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

                // AoE
                if (preset is CustomComboPreset.RDM_AoE_oGCD)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_Fleche, "Fleche", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_ContreSixte, "Contre Sixte", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                    if (RDM_AoE_oGCD_Engagement)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                        ImGui.Unindent();
                    }
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                    if (RDM_AoE_oGCD_CorpACorps)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                        UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                        ImGui.Unindent();
                    }
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_ViceOfThorns, "Vice of Thorns", "");
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_oGCD_Prefulgence, "Prefulgence", "");
                }

                if (preset is CustomComboPreset.RDM_AoE_MeleeCombo)
                {
                    UserConfig.DrawSliderInt(3, 8, RDM_AoE_MoulinetRange, "Range to use first Moulinet; no range restrictions after first Moulinet", sliderIncrement: SliderIncrements.Ones);
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                    if (RDM_AoE_MeleeCombo_Adv)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, "Scatter/Impact", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, "Moulinet", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                        ImGui.Unindent();
                    }
                }

                if (preset is CustomComboPreset.RDM_AoE_MeleeFinisher)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_AoE_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                    if (RDM_AoE_MeleeFinisher_Adv)
                    {
                        ImGui.Indent(); ImGui.Spacing();
                        UserConfig.DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "Scatter/Impact", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "Moulinet", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                        UserConfig.DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "VerAero II & VerThunder II", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                        ImGui.Unindent();
                    }
                }

                if (preset is CustomComboPreset.RDM_AoE_Lucid)
                    UserConfig.DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

                if (preset is CustomComboPreset.RDM_Variant_Cure)
                    UserConfig.DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);

                if (preset is CustomComboPreset.RDM_ST_MeleeCombo)
                {
                    UserConfig.DrawAdditionalBoolChoice(RDM_ST_MeleeEnforced, "Enforced Melee Check", "Once the melee combo has started, don't switch away even if target is out of range.");
                }
            }
        }
    }
}
