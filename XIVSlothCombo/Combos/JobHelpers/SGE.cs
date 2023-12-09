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
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && FindEffect(PvE.SGE.Buffs.EukrasianDiagnosis, healTarget, LocalPlayer?.ObjectId) is not null;
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
    }
}
