using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Find if an effect on the player exists. The effect may be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool HasEffect(ushort effectID) => FindEffect(effectID) is not null;

        public static byte GetBuffStacks(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.StackCount ?? 0;
        }

        public unsafe static float GetBuffRemainingTime(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            if (eff is null) return 0;
            if (eff.RemainingTime < 0) return (eff.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
            return eff.RemainingTime;
        }

        /// <summary> Finds an effect on the player. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffect(ushort effectID) => FindEffect(effectID, LocalPlayer, LocalPlayer?.GameObjectId);

        /// <summary> Find if an effect on the target exists. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool TargetHasEffect(ushort effectID) => FindTargetEffect(effectID) is not null;

        /// <summary> Finds an effect on the current target. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindTargetEffect(ushort effectID) => FindEffect(effectID, CurrentTarget, LocalPlayer?.GameObjectId);

        /// <summary></summary>
        public unsafe static float GetDebuffRemainingTime(ushort effectId)
        {
            Status? eff = FindTargetEffect(effectId);
            if (eff is null) return 0;
            if (eff.RemainingTime < 0) return (eff.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
            return eff.RemainingTime;
        }

        /// <summary> Find if an effect on the player exists. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool HasEffectAny(ushort effectID) => FindEffectAny(effectID) is not null;

        /// <summary> Finds an effect on the player. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffectAny(ushort effectID) => FindEffect(effectID, LocalPlayer, null);

        /// <summary> Find if an effect on the target exists. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool TargetHasEffectAny(ushort effectID) => FindTargetEffectAny(effectID) is not null;

        /// <summary> Finds an effect on the current target. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindTargetEffectAny(ushort effectID) => FindEffect(effectID, CurrentTarget, null);

        /// <summary> Finds an effect on the given object. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <param name="obj"> Object to look for effects on. </param>
        /// <param name="sourceID"> Source object ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffect(ushort effectID, IGameObject? obj, ulong? sourceID) => Service.ComboCache.GetStatus(effectID, obj, sourceID);

        ///<summary> Checks a member object for an effect. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <param name="obj"></param>
        /// <return> Status object or null. </return>
        public static Status? FindEffectOnMember(ushort effectID, IGameObject? obj) => Service.ComboCache.GetStatus(effectID, obj, null);

        /// <summary> Returns the name of a status effect from its ID. </summary>
        /// <param name="id"> ID of the status. </param>
        /// <returns></returns>
        public static string GetStatusName(uint id) => ActionWatching.GetStatusName(id);

        /// <summary> Checks if the character has the Silence status. </summary>
        /// <returns></returns>
        public static bool HasSilence()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(7)))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Pacification status. </summary>
        /// <returns></returns>
        public static bool HasPacification()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(6)))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Amnesia status. </summary>
        /// <returns></returns>
        public static bool HasAmnesia()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(5)))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        public static bool TargetHasDamageDown(IGameObject? target)
        {
            foreach (var status in ActionWatching.GetStatusesByName(GetStatusName(62)))
            {
                if (FindEffectOnMember((ushort)status, target) is not null) return true;
            }

            return false;
        }

        public static bool TargetHasRezWeakness(IGameObject? target)
        {
            foreach (var status in ActionWatching.GetStatusesByName(GetStatusName(43)))
            {
                if (FindEffectOnMember((ushort)status, target) is not null) return true;
            }
            foreach (var status in ActionWatching.GetStatusesByName(GetStatusName(44)))
            {
                if (FindEffectOnMember((ushort)status, target) is not null) return true;
            }

            return false;
        }


        public static bool HasCleansableDebuff(IGameObject? OurTarget = null)
        {
            OurTarget ??= CurrentTarget;
            if (HasFriendlyTarget(OurTarget) && (OurTarget is IBattleChara chara))
            {
                foreach (Status status in chara.StatusList)
                {
                    if (ActionWatching.StatusSheet.TryGetValue(status.StatusId, out var statusItem) && statusItem.CanDispel)
                        return true;
                }
            }
            return false;
        }

        public static bool NoBlockingStatuses(uint actionId)
        {
            switch (ActionWatching.GetAttackType(actionId))
            {
                case ActionWatching.ActionAttackType.Weaponskill:
                    if (HasPacification()) return false;
                    return true;
                case ActionWatching.ActionAttackType.Spell:
                    if (HasSilence()) return false;
                    return true;
                case ActionWatching.ActionAttackType.Ability:
                    if (HasAmnesia()) return false;
                    return true;

            }

            return true;
        }
    }
}
