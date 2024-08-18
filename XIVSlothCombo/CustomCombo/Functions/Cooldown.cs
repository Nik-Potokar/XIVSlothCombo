using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Gets the cooldown data for an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Cooldown data. </returns>
        public static CooldownData GetCooldown(uint actionID) => Service.ComboCache.GetCooldown(actionID);

        /// <summary> Gets the cooldown total remaining time. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Total remaining time of the cooldown. </returns>
        public static float GetCooldownRemainingTime(uint actionID) => Service.ComboCache.GetCooldown(actionID).CooldownRemaining;

        /// <summary> Gets the cooldown remaining time for the next charge. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Remaining time for the next charge of the cooldown. </returns>
        public static float GetCooldownChargeRemainingTime(uint actionID) => Service.ComboCache.GetCooldown(actionID).ChargeCooldownRemaining;

        /// <summary> Gets the elapsed cooldown time.</summary>
        /// <param name="actionID">Action ID to check</param>
        /// <returns> Time passed since action went on cooldown.</returns>
        public static float GetCooldownElapsed(uint actionID) => Service.ComboCache.GetCooldown(actionID).CooldownElapsed;

        /// <summary> Gets a value indicating whether an action is on cooldown. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> True or false. </returns>
        public static bool IsOnCooldown(uint actionID) => GetCooldown(actionID).IsCooldown;

        /// <summary> Gets a value indicating whether an action is off cooldown. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> True or false. </returns>
        public static bool IsOffCooldown(uint actionID) => !GetCooldown(actionID).IsCooldown;

        /// <summary> Check if the Cooldown was just used. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <param name="variance"> Variance of how long to check the elapsed cooldown</param>
        /// <returns> True or false. </returns>
        public static bool JustUsed(uint actionID, float variance = 3f) => GetMaxCharges(actionID) == 0 ? IsOnCooldown(actionID) && GetCooldownElapsed(actionID) <= variance : ActionWatching.ChargeTimestamps.ContainsKey(actionID) ? (Environment.TickCount64 - ActionWatching.ChargeTimestamps[actionID]) / 1000f <= variance : false;

        /// <summary> Gets a value indicating whether an action has any available charges. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> True or false. </returns>
        public static bool HasCharges(uint actionID) => GetCooldown(actionID).RemainingCharges > 0;

        /// <summary> Get the current number of charges remaining for an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Number of charges. </returns>
        public static uint GetRemainingCharges(uint actionID) => GetCooldown(actionID).RemainingCharges;

        /// <summary> Get the maximum number of charges for an action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns> Number of charges. </returns>
        public static ushort GetMaxCharges(uint actionID) => GetCooldown(actionID).MaxCharges;

        /// <summary> Get if an action is enabled.</summary>
        /// <param name="actionID"> Action ID to check</param>
        /// <returns> If the action is currently enabled.</returns>
        public unsafe static bool IsEnabled(uint actionID) => ActionManager.Instance()->GetActionStatus(ActionType.Action, actionID) == 0;
    }
}
