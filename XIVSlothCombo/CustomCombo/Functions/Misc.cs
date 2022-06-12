using System.Collections.Generic;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Determine if the given preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is enabled. </returns>
        public bool IsEnabled(CustomComboPreset preset) => (int)preset < 100 || Service.Configuration.IsEnabled(preset);

        /// <summary> Determine if the given preset is not enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is not enabled. </returns>
        public bool IsNotEnabled(CustomComboPreset preset) => !IsEnabled(preset);

        // Job & Class Names
        public class JobNames
        {
            public static readonly List<string> Melee = new() {
                // English
                "monk", "dragoon", "ninja", "reaper", "samurai", "pugilist", "lancer", "rogue",
                // Chinese
                "武僧", "龙骑士", "忍者", "钐镰客", "武士", "格斗家", "枪术师", "双剑师",
                // Japanese
                "モンク", "竜騎士", "忍者", "リーパー", "侍", "格闘士", "槍術士", "双剣士",
                // French (ninja is french for ninja)
                "moine", "chevalier dragon", "faucheur", "samouraï", "pugiliste", "maître d'hast", "surineur",
                // German (dragoon/ninja/samurai are as is)
                "mönch", "schnitter", "faustkämpfer", "pikenier", "schurke"
            };

            public static readonly List<string> Ranged = new() {
                // English
                "bard", "machinist", "dancer", "red mage", "black mage", "summoner", "blue mage", "archer", "thaumaturge", "arcanist",
                // Chinese
                "吟游诗人", "机工士", "舞者", "赤魔法师", "黑魔法师", "召唤师", "青魔法师", "弓箭手", "咒术师", "秘术师",
                // Japanese
                "吟遊詩人", "機工士", "踊り子", "赤魔道士", "黒魔道士", "召喚士", "青魔道士", "弓術士", "呪術士", "巴術士",
                // French (archer skipped)
                "barde", "machiniste", "danseur", "mage rouge", "mage noir", "invocateur", "mage bleu", "occultiste", "arcaniste",
                // German (bard skipped)
                "maschinist", "tänzser", "rotmagier", "schwarzmagier", "beschwörer", "blaumagier", "waldäufer", "thaumaturg", "hermetiker"
            };

            public static readonly List<string> Tank = new()
            {
                // English
                "paladin", "warrior", "dark knight", "gunbreaker", "gladiator", "marauder",
                // Chinese
                "骑士", "战士", "暗黑骑士", "绝枪战士", "剑术师", "斧术师",
                // Japanese
                "ナイト", "戦士", "暗黒騎士", "ガンブレイカー", "剣術士", "斧術士",
                // French (paladin)
                "guerrier", "chevalier noir", "pistosabreur", "gladiateur", "maraudeur",
                // German (paladin/gladiator are as is)
                "krieger", "dunkelritter", "revolverklinge", "marodeur"
            };

            public static readonly List<string> Healer = new()
            {
                // English
                "white mage", "astrologian", "scholar", "sage", "conjurer",
                // Chinese
                "白魔法师", "占星术士", "学者", "贤者", "幻术师",
                // Japanese
                "白魔道士", "占星術師", "学者", "賢者", "幻術士",
                // French (sage is the same as en)
                "mage blanc", "astromancien", "érudits", "élémentaliste",
                // German
                "weißmagier", "weissmagier", "gelehrter", "astrologe", "weiser", "druide"
            };
        }
    }
}
