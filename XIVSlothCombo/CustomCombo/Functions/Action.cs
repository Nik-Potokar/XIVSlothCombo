using FFXIVClientStructs.FFXIV.Client.Game;
using System.Linq;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Calls the original hook. </summary>
        /// <param name="actionID"> Action ID. </param>
        /// <returns> The result from the hook. </returns>
        public static uint OriginalHook(uint actionID) => Service.IconReplacer.OriginalHook(actionID);

        /// <summary> Compare the original hook to the given action ID. </summary>
        /// <param name="actionID"> Action ID. </param>
        /// <returns> A value indicating whether the action would be modified. </returns>
        public static bool IsOriginal(uint actionID) => Service.IconReplacer.OriginalHook(actionID) == actionID;

        /// <summary> Checks if the player is high enough level to use the passed Action ID. </summary>
        /// <param name="actionid"> ID of the action. </param>
        /// <returns></returns>
        public static bool LevelChecked(uint actionid) => LocalPlayer.Level >= GetLevel(actionid) && NoBlockingStatuses(actionid) && IsActionUnlocked(actionid);

        /// <summary> Checks if the player is high enough level to use the passed Trait ID. </summary>
        /// <param name="traitid"> ID of the action. </param>
        /// <returns></returns>
        public static bool TraitLevelChecked(uint traitid) => LocalPlayer.Level >= GetTraitLevel(traitid);

        /// <summary> Returns the name of an action from its ID. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static string GetActionName(uint id) => ActionWatching.GetActionName(id);

        /// <summary> Returns the level required for an action from its ID. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static int GetLevel(uint id) => ActionWatching.GetLevel(id);

        /// <summary> Get the Cast time of an action. </summary>
        /// <param name="id"> Action ID to check. </param>
        /// <returns> Returns the cast time of an action. </returns>
        internal static unsafe float GetActionCastTime(uint id) => ActionWatching.GetActionCastTime(id);

        /// <summary> Checks if the player is in range to use an action. Best used with actions with irregular ranges.</summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static bool InActionRange(uint id)
        {
            int range = ActionWatching.GetActionRange(id);
            switch (range)
            {
                case -2:
                    return false; //Error catch, Doesn't exist in ActionWatching
                case -1:
                    return InMeleeRange();//In the Sheet, all Melee skills appear to be -1
                case 0: //Self Use Skills (Second Wind) or attacks (Art of War, Dyskrasia)
                    {
                        //NOTES: HOUSING DUMMIES ARE FUCKING CURSED BASTARDS THAT DON'T REGISTER ATTACKS CORRECTLY WITH SELF RADIUS ATTACKS
                        //Use Explorer Mode dungeon, field map dummies, or let Thancred tank.

                        //Check if there is a radius
                        float radius = ActionWatching.GetActionEffectRange(id);
                        //Player has a 0.5y radius inside hitbox.
                        //GetTargetDistance measures hitbox to hitbox (correct usage for ranged abilities so far)
                        //But attacks from player must include personal space (0.5y).
                        if (radius > 0)
                        {   //Do not nest with above
                            if (HasTarget()) return GetTargetDistance() <= (radius - 0.5f); else return false;
                        }
                        else return true; //Self use targets (Second Wind) have no radius
                    }
                default:
                    return GetTargetDistance() <= range;
            }
        }

        /// <summary> Returns the level of a trait. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static int GetTraitLevel(uint id) => ActionWatching.GetTraitLevel(id);

        /// <summary> Checks if the player can use an action based on the level required and off cooldown / has charges.</summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        //Note: Testing so far shows non charge skills have a max charge of 1, and it's zero during cooldown
        public unsafe static bool ActionReady(uint id) => (LevelChecked(id) && (HasCharges(id) || GetCooldown(id).CooldownTotal <= 3)) || ActionManager.Instance()->GetActionStatus(ActionType.Action, id) == 0;

        /// <summary> Checks if the last action performed was the passed ID. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static bool WasLastAction(uint id) => ActionWatching.LastAction == id;

        /// <summary> Returns how many times in a row the last action was used. </summary>
        /// <returns></returns>
        public static int LastActionCounter() => ActionWatching.LastActionUseCount;

        /// <summary> Checks if the last weaponskill used was the passed ID. Does not have to be the last action performed, just the last weaponskill used. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static bool WasLastWeaponskill(uint id) => ActionWatching.LastWeaponskill == id;

        /// <summary> Checks if the last spell used was the passed ID. Does not have to be the last action performed, just the last spell used. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static bool WasLastSpell(uint id) => ActionWatching.LastSpell == id;

        /// <summary> Checks if the last ability used was the passed ID. Does not have to be the last action performed, just the last ability used. </summary>
        /// <param name="id"> ID of the action. </param>
        /// <returns></returns>
        public static bool WasLastAbility(uint id) => ActionWatching.LastAbility == id;

        /// <summary> Returns if the player has set the spell as active in the Blue Mage Spellbook </summary>
        /// <param name="id"> ID of the BLU spell. </param>
        /// <returns></returns>
        public static bool IsSpellActive(uint id) => Service.Configuration.ActiveBLUSpells.Contains(id);

        /// <summary> Calculate the best action to use, based on cooldown remaining. If there is a tie, the original is used. </summary>
        /// <param name="original"> The original action. </param>
        /// <param name="actions"> Action data. </param>
        /// <returns> The appropriate action to use. </returns>
        public static uint CalcBestAction(uint original, params uint[] actions)
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

            static (uint ActionID, CooldownData Data) Selector(uint actionID) => (actionID, GetCooldown(actionID));

            return actions
                .Select(Selector)
                .Aggregate((a1, a2) => Compare(original, a1, a2))
                .ActionID;
        }

        /// <summary> Checks if the provided actionID has enough cooldown remaining to weave against it without causing clipping.</summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <param name="weaveTime"> Time when weaving window is over. Defaults to 0.7. </param>
        /// <returns> True or false. </returns>
        public static bool CanWeave(uint actionID, double weaveTime = 0.7) => (GetCooldown(actionID).CooldownRemaining > weaveTime) || (HasSilence() && HasPacification());

        /// <summary> Checks if the provided actionID has enough cooldown remaining to weave against it without causing clipping and checks if you're casting a spell. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <param name="weaveTime"> Time when weaving window is over. Defaults to 0.6. </param>
        /// <returns> True or false. </returns>
        public static bool CanSpellWeave(uint actionID, double weaveTime = 0.6)
        {
            float castTimeRemaining = LocalPlayer.TotalCastTime - LocalPlayer.CurrentCastTime;

            if (GetCooldown(actionID).CooldownRemaining > weaveTime &&                          // Prevent GCD delay
                castTimeRemaining <= 0.5 &&                                                     // Show in last 0.5sec of cast so game can queue ability
                GetCooldown(actionID).CooldownRemaining - castTimeRemaining - weaveTime >= 0)   // Don't show if spell is still casting in weave window
                return true;
            return false;
        }

        /// <summary> Checks if the provided actionID has enough cooldown remaining to weave against it in the later portion of the GCD without causing clipping. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <param name="start"> Time (in seconds) to start to check for the weave window. </param>
        /// <param name="end"> Time (in seconds) to end the check for the weave window. </param>
        /// <returns> True or false. </returns>
        public static bool CanDelayedWeave(uint actionID, double start = 1.25, double end = 0.6) => GetCooldown(actionID).CooldownRemaining <= start && GetCooldown(actionID).CooldownRemaining >= end;

        /// <summary>
        /// Returns the current combo timer.
        /// </summary>
        public unsafe static float ComboTimer => ActionManager.Instance()->Combo.Timer;

        /// <summary>
        /// Returns the last combo action.
        /// </summary>
        public unsafe static uint ComboAction => ActionManager.Instance()->Combo.Action;
    }
}
