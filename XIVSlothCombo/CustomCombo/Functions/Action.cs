using XIVSlothComboPlugin;
namespace XIVSlothCombo.CustomCombo
{
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Calls the original hook.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <returns>The result from the hook.</returns>
        public uint OriginalHook(uint actionID)
            => Service.IconReplacer.OriginalHook(actionID);

        /// <summary>
        /// Compare the original hook to the given action ID.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <returns>A value indicating whether the action would be modified.</returns>
        public bool IsOriginal(uint actionID)
            => Service.IconReplacer.OriginalHook(actionID) == actionID;

        /// <summary>
        /// Checks if the player is high enough level to use the passed ID.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public bool LevelChecked(uint id)
        {
            if (LocalPlayer.Level < GetLevel(id))
                return false;

            return true;
        }

        /// <summary>
        /// Returns the name of an action from its ID.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public string GetActionName(uint id)
            => ActionWatching.GetActionName(id);

        /// <summary>
        /// Returns the level required for an action from its ID.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public int GetLevel(uint id)
            => ActionWatching.GetLevel(id);

        /// <summary>
        /// Checks if the last action performed was the passed ID.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public bool WasLastAction(uint id)
            => ActionWatching.LastAction == id;

        /// <summary>
        /// Returns how many times in a row the last action was used.
        /// </summary>
        /// <returns></returns>
        public int LastActionCounter()
            => ActionWatching.LastActionUseCount;

        /// <summary>
        /// Checks if the last weaponskill used was the passed ID. Does not have to be the last action performed, just the last weaponskill used.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public bool WasLastWeaponskill(uint id)
            => ActionWatching.LastWeaponskill == id;

        /// <summary>
        /// Checks if the last spell used was the passed ID. Does not have to be the last action performed, just the last spell used.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public bool WasLastSpell(uint id)
            => ActionWatching.LastSpell == id;

        /// <summary>
        /// Checks if the last ability used was the passed ID. Does not have to be the last action performed, just the last ability used.
        /// </summary>
        /// <param name="id">ID of the action.</param>
        /// <returns></returns>
        public bool WasLastAbility(uint id)
            => ActionWatching.LastAbility == id;

        /// <summary>
        /// Returns if the player has set the spell as active in the Blue Mage Spellbook
        /// </summary>
        /// <param name="id">ID of the BLU spell.</param>
        /// <returns></returns>
        public bool IsSpellActive(uint id)
            => Service.Configuration.ActiveBLUSpells.Contains(id);
    }
}
