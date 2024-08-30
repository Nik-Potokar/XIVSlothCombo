using static XIVSlothCombo.Combos.PvE.SCH;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class SCHHelper
    {
        public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
        {
            var healTarget = GetHealTarget(Config.SCH_ST_Heal_Adv && Config.SCH_ST_Heal_UIMouseOver);

            switch (i)
            {
                case 0:
                    action = Lustrate;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Lustrate) && Gauge.HasAetherflow();
                    return Config.SCH_ST_Heal_LustrateOption;
                case 1:
                    action = Excogitation;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Excogitation) && (Gauge.HasAetherflow() || HasEffect(Buffs.Recitation));
                    return Config.SCH_ST_Heal_ExcogitationOption;
                case 2:
                    action = Protraction;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Protraction);
                    return Config.SCH_ST_Heal_ProtractionOption;
            }

            enabled = false;
            action = 0;
            return 0;
        }

        //public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
        //{
        //    switch (i)
        //    {
        //        case 0:
        //            action = PvE.SGE.Kerachole;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && (!PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait || (PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait && TraitLevelChecked(PvE.SGE.Traits.EnhancedKerachole))) && PvE.SGE.Gauge.HasAddersgall();
        //            return 0;
        //        case 1:
        //            action = PvE.SGE.Ixochole;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && PvE.SGE.Gauge.HasAddersgall();
        //            return 0;
        //        case 2:
        //            action = OriginalHook(PvE.SGE.Physis);
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis);
        //            return 0;
        //        case 3:
        //            action = PvE.SGE.Holos;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos);
        //            return 0;
        //        case 4:
        //            action = PvE.SGE.Panhaima;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima);
        //            return 0;
        //        case 5:
        //            action = PvE.SGE.Pepsis;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianPrognosis) is not null;
        //            return 0;
        //        case 6:
        //            action = PvE.SGE.Philosophia;
        //            enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Philosophia);
        //            return 0;
        //    }

        //    enabled = false;
        //    action = 0;
        //    return 0;
        //}
    }
}
