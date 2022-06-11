using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using XIVSlothCombo.Services;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace XIVSlothCombo.CustomComboNS
{
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Gets the player or null.
        /// </summary>
        public PlayerCharacter? LocalPlayer
            => Service.ClientState.LocalPlayer;

        /// <summary>
        /// Find if the player has a certain condition.
        /// </summary>
        /// <param name="flag">Condition flag.</param>
        /// <returns>A value indicating whether the player is in the condition.</returns>
        public bool HasCondition(ConditionFlag flag)
            => Service.Condition[flag];

        /// <summary>
        /// Find if the player is in combat.
        /// </summary>
        /// <returns>A value indicating whether the player is in combat.</returns>
        public bool InCombat()
            => Service.Condition[ConditionFlag.InCombat];

        /// <summary>
        /// Find if the player has a pet present.
        /// </summary>
        /// <returns>A value indicating whether the player has a pet present.</returns>
        public bool HasPetPresent()
            => Service.BuddyList.PetBuddyPresent;


        /// <summary>
        /// Checks if the player is in a PVP enabled zone.
        /// </summary>
        /// <returns></returns>
        public bool InPvP()
            => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();


    }
}
