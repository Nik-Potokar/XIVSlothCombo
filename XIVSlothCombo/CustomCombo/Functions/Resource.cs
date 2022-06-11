using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS
{
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Gets the Resource Cost of the action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns></returns>
        public int GetResourceCost(uint actionID)
            => Service.ComboCache.GetResourceCost(actionID);

        /// <summary>
        /// Gets the Resource Type of the action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns></returns>
        public bool IsResourceTypeNormal(uint actionID)
            => Service.ComboCache.GetResourceCost(actionID) >= 100 || Service.ComboCache.GetResourceCost(actionID) == 0;

        /// <summary>
        /// Get a job gauge.
        /// </summary>
        /// <typeparam name="T">Type of job gauge.</typeparam>
        /// <returns>The job gauge.</returns>
        public T GetJobGauge<T>() where T : JobGaugeBase
            => Service.ComboCache.GetJobGauge<T>();
    }
}
