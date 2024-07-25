using XIVSlothCombo.Services;
using static XIVSlothCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace XIVSlothCombo.Combos.PvE.Content
{
    internal static class Variant
    {
        public const uint
            VariantUltimatum = 29730,
            VariantRaise = 29731,
            VariantRaise2 = 29734;

        //The following actions change ID based on Location
        //1069 = The Sil'dihn Subterrane
        //1137 = Mount Rokkon
        //1176 = Aloalo Island

        public static uint VariantCure => Service.ClientState.TerritoryType switch
        {
            1069 => 29729,
            1137 or 1176 => 33862,
            _ => 0
        };

        public static uint VariantSpiritDart => Service.ClientState.TerritoryType switch
        {
            1069 => 29732,
            1137 or 1176 => 33863,
            _ => 0
        };

        public static uint VariantRampart => Service.ClientState.TerritoryType switch
        {
            1069 => 29733,
            1137 or 1176 => 33864,
            _ => 0
        };

        public static class Buffs
        {
            public const ushort
                EmnityUp = 3358,
                VulnDown = 3360,
                Rehabilitation = 3367,
                DamageBarrier = 3405;
        }

        public static class Debuffs
        {
            public const ushort
                SustainedDamage = 3359;
        }

        /// <summary>
        /// Checks to see if Variant Ultimatum can be used
        /// </summary>
        /// <param name="preset">The class/job preset</param>
        /// <returns>Boolean stating if Ultimatum can be used</returns>
        public static bool CanUltimatum(CustomComboPreset preset) =>
            IsEnabled(preset) &&
            IsEnabled(VariantUltimatum) &&
            IsOffCooldown(VariantUltimatum);

        /// <summary>
        /// Checks to see if Variant Rampart can be used
        /// </summary>
        /// <param name="preset">The class/job preset</param>
        /// <param name="actionID">Original actionID to check for Weaving</param>
        /// <param name="caster">Is this for a caster? If so, use CanSpellWeave instead of CanWeave</param>
        /// <returns>Boolean stating if Rampart can be used</returns>
        public static bool CanRampart(CustomComboPreset preset, uint actionID, bool caster = false) =>
            IsEnabled(preset) &&
            IsEnabled(VariantRampart) &&
            IsOffCooldown(VariantRampart) &&
            ((!caster && CanWeave(actionID)) || (caster && CanSpellWeave(actionID)));

        /// <summary>
        /// Checks to see if Variant Cure can be used
        /// </summary>
        /// <param name="preset">The class/job preset</param>
        /// <param name="HPPercent">The HP Percent Threshold to activate</param>
        /// <returns></returns>
        public static bool CanCure(CustomComboPreset preset, int HPPercent) =>
            IsEnabled(preset) &&
            IsEnabled(VariantCure) &&
            PlayerHealthPercentageHp() <= HPPercent;
    }
}
