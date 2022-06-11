using System;
using System.Timers;

namespace XIVSlothCombo.CustomComboNS
{
    internal abstract partial class CustomCombo
    {
        private bool restartCombatTimer = true;
        private TimeSpan combatDuration = new();
        private DateTime combatStart;
        private DateTime combatEnd;
        private readonly Timer combatTimer;

        /// <summary>
        /// Function that keeps getting called by the timer set up in the constructor,
        /// to keep track of combat duration.
        /// </summary>
        private void UpdateCombatTimer(object sender, EventArgs e)
        {
            if (InCombat())
            {
                if (restartCombatTimer)
                {
                    restartCombatTimer = false;
                    combatStart = DateTime.Now;
                }

                combatEnd = DateTime.Now;
            }
            else
            {
                restartCombatTimer = true;
                combatDuration = TimeSpan.Zero;
            }

            combatDuration = combatEnd - combatStart;
        }

        /// <summary>
        /// Tells the elapsed time since the combat started.
        /// </summary>
        /// <returns>Combat time in seconds.</returns>
        protected TimeSpan CombatEngageDuration()
        {
            return combatDuration;
        }
    }
}
