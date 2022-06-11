using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS
{
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Determine if the given preset is enabled.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>A value indicating whether the preset is enabled.</returns>
        public bool IsEnabled(CustomComboPreset preset)
            => (int)preset < 100 || Service.Configuration.IsEnabled(preset);

        /// <summary>
        /// Determine if the given preset is not enabled.
        /// </summary>
        /// <param name="preset">Preset to check.</param>
        /// <returns>A value indicating whether the preset is not enabled.</returns>
        public bool IsNotEnabled(CustomComboPreset preset)
            => !IsEnabled(preset);

        /// <summary>
        /// Calculate the best action to use, based on cooldown remaining.
        /// If there is a tie, the original is used.
        /// </summary>
        /// <param name="original">The original action.</param>
        /// <param name="actions">Action data.</param>
        /// <returns>The appropriate action to use.</returns>
        public uint CalcBestAction(uint original, params uint[] actions)
        {
            static (uint ActionID, CooldownData Data) Compare(
                uint original,
                (uint ActionID, CooldownData Data) a1,
                (uint ActionID, CooldownData Data) a2)
            {
                // Neither, return the first parameter
                if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
                    return original == a1.ActionID ? a1 : a2;

                // Both, return soonest available
                if (a1.Data.IsCooldown && a2.Data.IsCooldown)
                {
                    if (a1.Data.HasCharges && a2.Data.HasCharges)
                    {
                        if (a1.Data.RemainingCharges == a2.Data.RemainingCharges)
                        {
                            return a1.Data.ChargeCooldownRemaining < a2.Data.ChargeCooldownRemaining
                                ? a1 : a2;
                        }

                        return a1.Data.RemainingCharges > a2.Data.RemainingCharges
                            ? a1 : a2;
                    }
                    else if (a1.Data.HasCharges)
                    {
                        if (a1.Data.RemainingCharges > 0)
                            return a1;

                        return a1.Data.ChargeCooldownRemaining < a2.Data.CooldownRemaining
                            ? a1 : a2;
                    }
                    else if (a2.Data.HasCharges)
                    {
                        if (a2.Data.RemainingCharges > 0)
                            return a2;

                        return a2.Data.ChargeCooldownRemaining < a1.Data.CooldownRemaining
                            ? a2 : a1;
                    }
                    else
                    {
                        return a1.Data.CooldownRemaining < a2.Data.CooldownRemaining
                            ? a1 : a2;
                    }
                }

                // One or the other
                return a1.Data.IsCooldown ? a2 : a1;
            }

            (uint ActionID, CooldownData Data) Selector(uint actionID)
                => (actionID, GetCooldown(actionID));

            return actions
                .Select(Selector)
                .Aggregate((a1, a2) => Compare(original, a1, a2))
                .ActionID;
        }

        /// <summary>
        /// Checks if the provided action ID has cooldown remaining enough to weave against it
        /// without causing clipping
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <param name="weaveTime">Time when weaving window is over. Defaults to 0.7.</param>
        /// <returns>True or false.</returns>
        public bool CanWeave(uint actionID, double weaveTime = 0.7)
           => (GetCooldown(actionID).CooldownRemaining > weaveTime) || (HasPacification() && HasSilence());

        /// <summary>
        /// Checks if the provided action ID has cooldown remaining enough to weave against it
        /// without causing clipping and checks if you're casting a spell to make it mage friendly
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <param name="weaveTime">Time when weaving window is over. Defaults to 0.6.</param>
        /// <returns>True or false.</returns>
        public bool CanSpellWeave(uint actionID, double weaveTime = 0.6)
        {
            var castTimeRemaining = LocalPlayer.TotalCastTime - LocalPlayer.CurrentCastTime;

            if (GetCooldown(actionID).CooldownRemaining > weaveTime && // Prevent GCD delay
                (castTimeRemaining <= 0.5 && // Show in last 0.5sec of cast so game can queue ability
                GetCooldown(actionID).CooldownRemaining - castTimeRemaining - weaveTime >= 0)) // Don't show if spell is still casting in weave window
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the provided action ID has cooldown remaining enough to weave against it
        /// at the later half of the gcd without causing clipping (aka Delayed Weaving)
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <param name="start">Time (in seconds) to start to check for the weave window.</param>
        /// <param name="end">Time (in seconds) to end the check for the weave window.</param>
        /// <returns>True or false.</returns>
        public bool CanDelayedWeave(uint actionID, double start = 1.25, double end = 0.6)
           => GetCooldown(actionID).CooldownRemaining < start && GetCooldown(actionID).CooldownRemaining > end;

        //Job & Class Names
        public class JobNames
        {
            public static readonly List<string> Melee = new() {
                //English
                "monk", "dragoon", "ninja", "reaper", "samurai", "pugilist", "lancer", "rogue",
                //Chinese
                "武僧", "龙骑士", "忍者", "钐镰客", "武士", "格斗家", "枪术师", "双剑师",
                //Japanese
                "モンク", "竜騎士", "忍者", "リーパー", "侍", "格闘士", "槍術士", "双剣士",
                //French (ninja is french for ninja)
                "moine", "chevalier dragon", "faucheur", "samouraï", "pugiliste", "maître d'hast", "surineur",
                //German (dragoon/ninja/samurai are as is)
                "mönch", "schnitter", "faustkämpfer", "pikenier", "schurke"
            };

            public static readonly List<string> Ranged = new() {
                //English
                "bard", "machinist", "dancer", "red mage", "black mage", "summoner", "blue mage", "archer", "thaumaturge", "arcanist",
                //Chinese
                "吟游诗人", "机工士", "舞者", "赤魔法师", "黑魔法师", "召唤师", "青魔法师", "弓箭手", "咒术师", "秘术师",
                //Japanese
                "吟遊詩人", "機工士", "踊り子", "赤魔道士", "黒魔道士", "召喚士", "青魔道士", "弓術士", "呪術士", "巴術士",
                //French (archer skipped)
                "barde", "machiniste", "danseur", "mage rouge", "mage noir", "invocateur", "mage bleu", "occultiste", "arcaniste",
                //German (barde skipped)
                "maschinist", "tänzser", "rotmagier", "schwarzmagier", "beschwörer", "blaumagier", "waldäufer", "thaumaturg", "hermetiker"
            };

            public static readonly List<string> Tank = new()
            {
                //English
                "paladin", "warrior", "dark knight", "gunbreaker", "gladiator", "marauder",
                //Chinese
                "骑士", "战士", "暗黑骑士", "绝枪战士", "剑术师", "斧术师",
                //Japanese
                "ナイト", "戦士", "暗黒騎士", "ガンブレイカー", "剣術士", "斧術士",
                //French (paladin)
                "guerrier", "chevalier noir", "pistosabreur", "gladiateur", "maraudeur",
                //German (paladin/gladiator are as is)
                "krieger", "dunkelritter", "revolverklinge", "marodeur"
            };

            public static readonly List<string> Healer = new()
            {
                //English
                "white mage", "astrologian", "scholar", "sage", "conjurer",
                //Chinese
                "白魔法师", "占星术士", "学者", "贤者", "幻术师",
                //Japanese
                "白魔道士", "占星術師", "学者", "賢者", "幻術士",
                //French (sage is the same as en)
                "mage blanc", "astromancien", "érudits", "élémentaliste",
                //German
                "weißmagier", "weissmagier", "gelehrter", "astrologe", "weiser", "druide"
            };
        }
    }
}
