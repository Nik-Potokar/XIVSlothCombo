using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;
using XIVSlothComboPlugin.Attributes;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace XIVSlothComboPlugin.Combos
{
    /// <summary>
    /// Base class for each combo.
    /// </summary>
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCombo"/> class.
        /// </summary>
        protected CustomCombo()
        {
            var presetInfo = this.Preset.GetAttribute<CustomComboInfoAttribute>();
            this.JobID = presetInfo.JobID;
            this.ClassID = this.JobID switch
            {
                ADV.JobID => ADV.ClassID,
                BLM.JobID => BLM.ClassID,
                BRD.JobID => BRD.ClassID,
                DRG.JobID => DRG.ClassID,
                MNK.JobID => MNK.ClassID,
                NIN.JobID => NIN.ClassID,
                PLD.JobID => PLD.ClassID,
                SCH.JobID => SCH.ClassID,
                SMN.JobID => SMN.ClassID,
                WAR.JobID => WAR.ClassID,
                WHM.JobID => WHM.ClassID,
                _ => 0xFF,
            };

            combatTimer = new Timer(1000); // in miliseconds
            combatTimer.Elapsed += UpdateCombatTimer;
            combatTimer.Start();

        }

        /// <summary>
        /// Gets the preset associated with this combo.
        /// </summary>
        protected internal abstract CustomComboPreset Preset { get; }

        /// <summary>
        /// Gets the class ID associated with this combo.
        /// </summary>
        protected byte ClassID { get; }

        /// <summary>
        /// Gets the job ID associated with this combo.
        /// </summary>
        protected byte JobID { get; }
        protected Vector2 Position { get; set; }
        protected float PlayerSpeed { get; set; }
        protected uint MovingCounter { get; set; }
        protected bool IsMoving { get; set; }

        /// <summary>
        /// Function that keeps getting called by the timer set up in the constructor,
        /// to keep track of combat duration.
        /// </summary>
        private bool restartCombatTimer = true;
        private TimeSpan combatDuration = new();
        private DateTime combatStart;
        private DateTime combatEnd;
        private readonly Timer combatTimer;
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

        /// <summary>
        /// Performs various checks then attempts to invoke the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="level">Player level.</param>
        /// <param name="lastComboMove">Last combo action ID.</param>
        /// <param name="comboTime">Combo timer.</param>
        /// <param name="newActionID">Replacement action ID.</param>
        /// <returns>True if the action has changed, otherwise false.</returns>

        public bool TryInvoke(uint actionID, byte level, uint lastComboMove, float comboTime, out uint newActionID)
        {
            newActionID = 0;
            // Movement
            if (this.MovingCounter == 0)
            {
                Vector2 newPosition = LocalPlayer is null ? Vector2.Zero : new Vector2(LocalPlayer.Position.X, LocalPlayer.Position.Z);
                this.PlayerSpeed = Vector2.Distance(newPosition, this.Position);
                this.IsMoving = this.PlayerSpeed > 0;
                this.Position = LocalPlayer is null ? Vector2.Zero : newPosition;
                // refreshes every 50 dalamud ticks for a more accurate representation of speed, otherwise it'll report 0.
                this.MovingCounter = 50;
            }

            if (this.MovingCounter > 0)
                this.MovingCounter--;


            if (!IsEnabled(this.Preset))
                return false;

            var classJobID = LocalPlayer!.ClassJob.Id;

            if (classJobID >= 8 && classJobID <= 15)
                classJobID = DOH.JobID;

            if (classJobID >= 16 && classJobID <= 18)
                classJobID = DoL.JobID;

            if (this.JobID != ADV.JobID && this.ClassID != ADV.ClassID &&
                this.JobID != classJobID && this.ClassID != classJobID)
                return false;

            var resultingActionID = this.Invoke(actionID, lastComboMove, comboTime, level);
            //Dalamud.Logging.PluginLog.Debug(resultingActionID.ToString());

            if (resultingActionID == 0 || actionID == resultingActionID)
                return false;

            newActionID = resultingActionID;

            return true;
        }

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
        /// Invokes the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="lastComboActionID">Last combo action.</param>
        /// <param name="comboTime">Current combo time.</param>
        /// <param name="level">Current player level.</param>
        /// <returns>The replacement action ID.</returns>
        protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);
    }

    /// <summary>
    /// Passthrough methods and properties to IconReplacer. Shortens what it takes to call each method.
    /// </summary>
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Gets the player or null.
        /// </summary>
        public PlayerCharacter? LocalPlayer
            => Service.ClientState.LocalPlayer;

        /// <summary>
        /// Gets the current target or null.
        /// </summary>
        public GameObject? CurrentTarget
            => Service.TargetManager.Target;

        /// <summary>
        /// Find if the player has a target.
        /// </summary>
        /// <returns>A value indicating whether the player has a target.</returns>
        public bool HasTarget()
            => CurrentTarget is not null;

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
        /// Find if an effect on the player exists.
        /// The effect may be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        public bool HasEffect(ushort effectID)
            => FindEffect(effectID) is not null;
        public float GetBuffStacks(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.StackCount ?? 0;
        }
        public float GetBuffRemainingTime(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.RemainingTime ?? 0;
        }

        ///<summary>
        ///Checks a member object for an effect.
        ///The effect may be owned by anyone or unowned.
        ///</summary>
        ///<param name="effectID">Status effect ID.</param>
        ///<param name="obj"></param>
        ///<return>Status object or null.</return>
        public Status? FindEffectOnMember(ushort effectID, GameObject? obj)
            => Service.ComboCache.GetStatus(effectID, obj, null);


        /// <summary>
        /// Finds an effect on the player.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        public Status? FindEffect(ushort effectID)
            => FindEffect(effectID, LocalPlayer, LocalPlayer?.ObjectId);

        /// <summary>
        /// Find if an effect on the target exists.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        public bool TargetHasEffect(ushort effectID)
            => FindTargetEffect(effectID) is not null;

        /// <summary>
        /// Finds an effect on the current target.
        /// The effect must be owned by the player or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        public Status? FindTargetEffect(ushort effectID)
            => FindEffect(effectID, CurrentTarget, LocalPlayer?.ObjectId);

        public float GetDebuffRemainingTime(ushort effectId)
        {
            Status? eff = FindTargetEffect(effectId);
            return eff?.RemainingTime ?? 0;
        }

        /// <summary>
        /// Find if an effect on the player exists.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        public bool HasEffectAny(ushort effectID)
            => FindEffectAny(effectID) is not null;

        /// <summary>
        /// Finds an effect on the player.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        public Status? FindEffectAny(ushort effectID)
            => FindEffect(effectID, LocalPlayer, null);

        /// <summary>
        /// Find if an effect on the target exists.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>A value indicating if the effect exists.</returns>
        public bool TargetHasEffectAny(ushort effectID)
            => FindTargetEffectAny(effectID) is not null;

        /// <summary>
        /// Finds an effect on the current target.
        /// The effect may be owned by anyone or unowned.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <returns>Status object or null.</returns>
        public Status? FindTargetEffectAny(ushort effectID)
            => FindEffect(effectID, CurrentTarget, null);

        /// <summary>
        /// Finds an effect on the given object.
        /// </summary>
        /// <param name="effectID">Status effect ID.</param>
        /// <param name="obj">Object to look for effects on.</param>
        /// <param name="sourceID">Source object ID.</param>
        /// <returns>Status object or null.</returns>
        public Status? FindEffect(ushort effectID, GameObject? obj, uint? sourceID)
            => Service.ComboCache.GetStatus(effectID, obj, sourceID);

        /// <summary>
        /// Gets the cooldown data for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Cooldown data.</returns>
        public CooldownData GetCooldown(uint actionID)
            => Service.ComboCache.GetCooldown(actionID);

        /// <summary>
        /// Gets the cooldown total remaining time.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Total remaining time of the cooldown.</returns>
        public float GetCooldownRemainingTime(uint actionID)
            => Service.ComboCache.GetCooldown(actionID).CooldownRemaining;

        /// <summary>
        /// Gets the cooldown remaining time for the next charge.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Remaining time for the next charge of the cooldown.</returns>
        public float GetCooldownChargeRemainingTime(uint actionID)
            => Service.ComboCache.GetCooldown(actionID).ChargeCooldownRemaining;

        /// <summary>
        /// Gets a value indicating whether an action is on cooldown.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>True or false.</returns>
        public bool IsOnCooldown(uint actionID)
            => GetCooldown(actionID).IsCooldown;

        /// <summary>
        /// Gets a value indicating whether an action is off cooldown.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>True or false.</returns>
        public bool IsOffCooldown(uint actionID)
            => !GetCooldown(actionID).IsCooldown;

        /// <summary>
        /// Check if the Cooldown was just used.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>True or false.</returns>
        public bool JustUsed(uint actionID)
           => IsOnCooldown(actionID) && GetCooldownRemainingTime(actionID) > (GetCooldown(actionID).CooldownTotal - 3);


        /// <summary>
        /// Gets a value indicating whether an action has any available charges.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>True or false.</returns>
        public bool HasCharges(uint actionID)
            => GetCooldown(actionID).RemainingCharges > 0;

        /// <summary>
        /// Get the current number of charges remaining for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Number of charges.</returns>
        public ushort GetRemainingCharges(uint actionID)
            => GetCooldown(actionID).RemainingCharges;

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
        /// Get the maximum number of charges for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Number of charges.</returns>
        public ushort GetMaxCharges(uint actionID)
            => GetCooldown(actionID).MaxCharges;

        /// <summary>
        /// Checks if the provided action ID has cooldown remaining enough to weave against it
        /// without causing clipping
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <param name="weaveTime">Time when weaving window is over. Defaults to 0.7.</param>
        /// <returns>True or false.</returns>
        public bool CanWeave(uint actionID, double weaveTime = 0.7)
           => GetCooldown(actionID).CooldownRemaining > weaveTime;

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

        /// <summary>
        /// Get a job gauge.
        /// </summary>
        /// <typeparam name="T">Type of job gauge.</typeparam>
        /// <returns>The job gauge.</returns>
        public T GetJobGauge<T>() where T : JobGaugeBase
            => Service.ComboCache.GetJobGauge<T>();

        /// <summary>
        /// Gets the distance from the target.
        /// </summary>
        /// <returns>Double representing the distance from the target.</returns>
        public double GetTargetDistance()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return 0;

            if (CurrentTarget is not BattleChara chara)
                return 0;

            if (CurrentTarget.ObjectId == LocalPlayer.ObjectId)
                return 0;

            var position = new Vector2(chara.Position.X, chara.Position.Z);
            var selfPosition = new Vector2(LocalPlayer.Position.X, LocalPlayer.Position.Z);

            return Math.Max(0, (Vector2.Distance(position, selfPosition) - chara.HitboxRadius) - LocalPlayer.HitboxRadius);
        }

        /// <summary>
        /// Gets a value indicating whether you are in melee range from the current target.
        /// </summary>
        /// <returns>Bool indicating whether you are in melee range.</returns>
        public bool InMeleeRange()
        {
            if (LocalPlayer.TargetObject == null) return false;

            var distance = GetTargetDistance();

            if (distance == 0)
                return true;

            if (distance > 3 + Service.Configuration.MeleeOffset)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating target's HP Percent. CurrentTarget is default unless specified
        /// </summary>
        /// <returns>Double indicating percentage.</returns>
        public double GetTargetHPPercent(GameObject? OurTarget = null)
        {
            if (OurTarget is null)
            {
                //Fallback to CurrentTarget
                OurTarget = CurrentTarget;
                if (OurTarget is null) return 0;
            }
            if (OurTarget is not BattleChara chara)
                return 0;

            double health = chara.CurrentHp;
            double maxHealth = chara.MaxHp;

            return health / maxHealth * 100;
        }
        public double EnemyHealthMaxHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not BattleChara chara)
                return 0;

            double maxHealth = chara.MaxHp;

            return maxHealth;
        }
        public double EnemyHealthCurrentHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not BattleChara chara)
                return 0;

            double currentHp = chara.CurrentHp;

            return currentHp;
        }
        public double PlayerHealthPercentageHp()
        {
            double maxHealth = LocalPlayer.MaxHp;
            double currentHealth = LocalPlayer.CurrentHp;

            return currentHealth / maxHealth * 100;
        }
        public bool HasBattleTarget()
        {
            if (CurrentTarget is null)
                return false;
            if (CurrentTarget is not BattleChara)
                return false;

            return true;
        }
        /// <summary>
        /// Determines if the enemy can be interrupted if they are currently casting.
        /// </summary>
        /// <returns>Bool indicating whether they can be interrupted or not.</returns>
        public bool CanInterruptEnemy()
        {
            if (CurrentTarget is null)
                return false;
            if (CurrentTarget is not BattleChara chara)
                return false;
            if (chara.IsCasting)
                return chara.IsCastInterruptible;
            return false;
        }


        /// <summary>
        /// Gets the party list
        /// </summary>
        /// <returns>Current party list.</returns>
        public PartyList GetPartyMembers() => Service.PartyList;

        /// <summary>
        /// Sets the player's target. 
        /// </summary>
        /// <param name="target">Target must be a game object that the player can normally click and target.</param>
        public void SetTarget(GameObject? target) =>
            Service.TargetManager.Target = target;


        /// <summary>
        /// Checks if target is in appropriate range for targeting
        /// </summary>
        /// <param name="target">The target object to check</param>
        public bool IsInRange(GameObject? target)
        {
            if (target == null) return false;
            if (target.YalmDistanceX >= 30) return false;

            return true;
        }

        /// <summary>
        /// Attempts to target the given party member
        /// </summary>
        /// <param name="target"></param>
        protected unsafe void TargetObject(TargetType target)
        {
            var t = GetTarget(target);
            if (t == null) return;
            var o = PartyTargetingService.GetObjectID(t);
            var p = Service.ObjectTable.Where(x => x.ObjectId == o).First();

            if (IsInRange(p)) SetTarget(p);
        }

        public void TargetObject(GameObject? target)
        {
            if (IsInRange(target)) SetTarget(target);
        }

        protected unsafe static GameObject? GetPartySlot(int slot)
        {
            try
            {
                var o = slot switch
                {
                    1 => GetTarget(TargetType.Self),
                    2 => GetTarget(TargetType.P2),
                    3 => GetTarget(TargetType.P3),
                    4 => GetTarget(TargetType.P4),
                    5 => GetTarget(TargetType.P5),
                    6 => GetTarget(TargetType.P6),
                    7 => GetTarget(TargetType.P7),
                    8 => GetTarget(TargetType.P8),
                    _ => GetTarget(TargetType.Self),
                };
                var i = PartyTargetingService.GetObjectID(o);
                if (Service.ObjectTable.Where(x => x.ObjectId == i).Any())
                    return Service.ObjectTable.Where(x => x.ObjectId == i).First();

                return null;
            }
            catch
            {
                return null;
            }

        }

        public int GetOptionValue(string SliderID)
        {
            return Service.Configuration.GetCustomIntValue(SliderID);
        }

        protected unsafe static FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* GetTarget(TargetType target)
        {
            GameObject? o = null;

            switch (target)
            {
                case TargetType.Target:
                    o = Service.TargetManager.Target;
                    break;
                case TargetType.SoftTarget:
                    o = Service.TargetManager.SoftTarget;
                    break;
                case TargetType.FocusTarget:
                    o = Service.TargetManager.FocusTarget;
                    break;
                case TargetType.UITarget:
                    return PartyTargetingService.UITarget;
                case TargetType.FieldTarget:
                    o = Service.TargetManager.MouseOverTarget;
                    break;
                case TargetType.TargetsTarget when Service.TargetManager.Target is { TargetObjectId: not 0xE0000000 }:
                    o = Service.TargetManager.Target.TargetObject;
                    break;
                case TargetType.Self:
                    o = Service.ClientState.LocalPlayer;
                    break;
                case TargetType.LastTarget:
                    return PartyTargetingService.GetGameObjectFromPronounID(1006);
                case TargetType.LastEnemy:
                    return PartyTargetingService.GetGameObjectFromPronounID(1084);
                case TargetType.LastAttacker:
                    return PartyTargetingService.GetGameObjectFromPronounID(1008);
                case TargetType.P2:
                    return PartyTargetingService.GetGameObjectFromPronounID(44);
                case TargetType.P3:
                    return PartyTargetingService.GetGameObjectFromPronounID(45);
                case TargetType.P4:
                    return PartyTargetingService.GetGameObjectFromPronounID(46);
                case TargetType.P5:
                    return PartyTargetingService.GetGameObjectFromPronounID(47);
                case TargetType.P6:
                    return PartyTargetingService.GetGameObjectFromPronounID(48);
                case TargetType.P7:
                    return PartyTargetingService.GetGameObjectFromPronounID(49);
                case TargetType.P8:
                    return PartyTargetingService.GetGameObjectFromPronounID(50);
            }

            return o != null ? (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)o.Address : null;
        }

        public enum TargetType
        {
            Target,
            SoftTarget,
            FocusTarget,
            UITarget,
            FieldTarget,
            TargetsTarget,
            Self,
            LastTarget,
            LastEnemy,
            LastAttacker,
            P2,
            P3,
            P4,
            P5,
            P6,
            P7,
            P8
        }

        public static Dictionary<uint, Lumina.Excel.GeneratedSheets.TerritoryType>? PvPZones = Service.DataManager?.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?
                        .Where(i => i.Bg.RawString.StartsWith("ffxiv/pvp"))
                        .ToDictionary(i => i.RowId, i => i);


        /// <summary>
        /// Checks if the player is in a PVP enabled zone.
        /// </summary>
        /// <returns></returns>
        public bool InPvP()
            => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();   

        public bool LevelChecked(uint id)
        {
            if (LocalPlayer.Level < GetLevel(id))
                return false;

            return true;
        }

        public string GetActionName(uint id)
            => ActionWatching.GetActionName(id);

        public string GetStatusName(uint id)
            => ActionWatching.GetStatusName(id);

        public int GetLevel(uint id)
            => ActionWatching.GetLevel(id);

        public bool WasLastAction(uint id)
            => ActionWatching.LastAction == id;

        public int LastActionCounter()
            => ActionWatching.LastActionUseCount;

        public bool WasLastWeaponskill(uint id)
            => ActionWatching.LastWeaponskill == id;

        public bool WasLastSpell(uint id)
            => ActionWatching.LastSpell == id;

        public bool WasLastAbility(uint id)
            => ActionWatching.LastAbility == id;

        /// <summary>
        /// Returns if the player has set the spell as active in the Blue Mage Spellbook
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSpellActive(uint id)
            => Service.Configuration.ActiveBLUSpells.Contains(id);

        //Class Names
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
                "white mage","astrologian","scholar","sage","conjurer",
                //Chinese
                "白魔法师","占星术士","学者","贤者","幻术师",
                //Japanese
                "白魔道士","占星術師","学者","賢者","幻術士",
                //French (sage is the same as en)
                "mage blanc", "astromancien", "érudits", "élémentaliste",
                //German
                "weißmagier", "weissmagier", "gelehrter", "astrologe", "weiser", "druide"
            };
        }

        //public bool CanUseAction(uint id)
        //{
        //    if (!ActionWatching.ActionSheet.TryGetValue(id, out var actionFromSheet)) return false;
        //    if (!LevelChecked(id)) return false;
        //    if (!IsOffCooldown(id)) return false;
        //    if (GetTargetDistance() < actionFromSheet.Range) return false;
        //    if (IsResourceTypeNormal(id) && GetResourceCost(id) > LocalPlayer.CurrentMp) return false;

        //    switch (JobID)
        //    {
        //        case AST.JobID:
        //            if (id == AST.Astrodyne && GetJobGauge<ASTGauge>().Seals.Count() < 3) return false;
        //            if (id == AST.Play && GetJobGauge<ASTGauge>().DrawnCard == Dalamud.Game.ClientState.JobGauge.Enums.CardType.NONE) return false;
        //            if (id == AST.MinorArcana && GetJobGauge<ASTGauge>().DrawnCrownCard == Dalamud.Game.ClientState.JobGauge.Enums.CardType.NONE) return false;
        //            break;
        //        case BLM.JobID:
        //            if ((id is BLM.Fire4 or BLM.Despair or BLM.Flare) && !GetJobGauge<BLMGauge>().InAstralFire) return false;
        //            if ((id is BLM.Blizzard4 or BLM.Freeze or BLM.UmbralSoul) && !GetJobGauge<BLMGauge>().InUmbralIce) return false;
        //            if (id is BLM.Amplifier && (!GetJobGauge<BLMGauge>().InUmbralIce && !GetJobGauge<BLMGauge>().InAstralFire)) return false;
        //            if (id is BLM.Paradox && !GetJobGauge<BLMGauge>().IsParadoxActive) return false;
        //            break;
        //        case BRD.JobID:
        //            if (GetJobGauge<BRDGauge>().SoulVoice < GetResourceCost(id)) return false;
        //            break;
        //        case DNC.JobID:
        //            if (id is DNC.FanDance1 or DNC.FanDance2 && GetJobGauge<DNCGauge>().Feathers == 0) return false;
        //            if (GetJobGauge<DNCGauge>().Esprit < GetResourceCost(id)) return false;
        //            break;
        //        case DRG.JobID:
        //            if (id is DRG.Stardiver && !GetJobGauge<DRGGauge>().IsLOTDActive) return false;
        //            break;
        //        case DRK.JobID:

        //            break;
        //        case GNB.JobID:

        //            break;
        //        case MCH.JobID:
        //            if ((id == MCH.AutomatonQueen || id == MCH.RookAutoturret) && GetJobGauge<MCHGauge>().Battery < GetResourceCost(id)) return false;
        //            if (GetJobGauge<MCHGauge>().Heat < GetResourceCost(id)) return false;
        //            break;
        //        case MNK.JobID:

        //            break;
        //        case NIN.JobID:
        //            if (GetJobGauge<NINGauge>().Ninki < GetResourceCost(id)) return false;
        //            break;
        //        case PLD.JobID:
        //            if (GetJobGauge<PLDGauge>().OathGauge < GetResourceCost(id)) return false;
        //            break;
        //        case RDM.JobID:

        //            break;
        //        case RPR.JobID:

        //            break;
        //        case SAM.JobID:

        //            break;
        //        case SCH.JobID:

        //            break;
        //        case SGE.JobID:

        //            break;
        //        case SMN.JobID:

        //            break;
        //        case WAR.JobID:

        //            break;
        //        case WHM.JobID:

        //            break;
        //    }

        //    return true;

        //}
    }
}
