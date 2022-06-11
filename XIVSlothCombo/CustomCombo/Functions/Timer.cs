using System;
using System.Timers;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        internal bool restartCombatTimer = true;
        internal TimeSpan combatDuration = new();
        internal DateTime combatStart;
        internal DateTime combatEnd;
        internal Timer? combatTimer;

        /// <summary>
        /// Function that keeps getting called by the timer set up in the constructor,
        /// to keep track of combat duration.
        /// </summary>
        internal void UpdateCombatTimer(object sender, EventArgs e)
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
