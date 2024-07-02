using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class SGE : CustomComboFunctions
    {
        public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
        {
            var healTarget = GetHealTarget(PvE.SGE.Config.SGE_ST_Heal_Adv && PvE.SGE.Config.SGE_ST_Heal_UIMouseOver);
            
            switch (i)
            {
                case 0:
                    action = PvE.SGE.Soteria;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria);
                    return PvE.SGE.Config.SGE_ST_Heal_Soteria;
                case 1:
                    action = PvE.SGE.Zoe;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe);
                    return PvE.SGE.Config.SGE_ST_Heal_Zoe;
                case 2:
                    action = PvE.SGE.Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianDiagnosis, healTarget, LocalPlayer?.GameObjectId) is not null;
                    return PvE.SGE.Config.SGE_ST_Heal_Pepsis;
                case 3:
                    action = PvE.SGE.Taurochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && PvE.SGE.Gauge.HasAddersgall();
                    return PvE.SGE.Config.SGE_ST_Heal_Taurochole;
                case 4:
                    action = PvE.SGE.Haima;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima);
                    return PvE.SGE.Config.SGE_ST_Heal_Haima;
                case 5:
                    action = PvE.SGE.Krasis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis);
                    return PvE.SGE.Config.SGE_ST_Heal_Krasis;
                case 6:
                    action = PvE.SGE.Druochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && PvE.SGE.Gauge.HasAddersgall();
                    return PvE.SGE.Config.SGE_ST_Heal_Druochole;
            }

            enabled = false;
            action = 0;
            return 0;
        }

        public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
        {
            switch (i)
            {
                case 0:
                    action = PvE.SGE.Kerachole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && (!PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait || (PvE.SGE.Config.SGE_AoE_Heal_KeracholeTrait && TraitLevelChecked(PvE.SGE.Traits.EnhancedKerachole))) && PvE.SGE.Gauge.HasAddersgall();
                    return 0;
                case 1:
                    action = PvE.SGE.Ixochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && PvE.SGE.Gauge.HasAddersgall();
                    return 0;
                case 2:
                    action = OriginalHook(PvE.SGE.Physis);
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis);
                    return 0;
                case 3:
                    action = PvE.SGE.Holos;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos);
                    return 0;
                case 4:
                    action = PvE.SGE.Panhaima;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima);
                    return 0;
                case 5:
                    action = PvE.SGE.Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianPrognosis) is not null;
                    return 0;
            }

            enabled = false;
            action = 0;
            return 0;
        }
    }
}
