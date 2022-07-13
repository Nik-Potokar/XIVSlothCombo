using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Find if an effect on the player exists. The effect may be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool HasEffect(ushort effectID) => FindEffect(effectID) is not null;

        public static float GetBuffStacks(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.StackCount ?? 0;
        }

        public static float GetBuffRemainingTime(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.RemainingTime ?? 0;
        }

        /// <summary> Finds an effect on the player. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffect(ushort effectID) => FindEffect(effectID, LocalPlayer, LocalPlayer?.ObjectId);

        /// <summary> Find if an effect on the target exists. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool TargetHasEffect(ushort effectID) => FindTargetEffect(effectID) is not null;

        /// <summary> Finds an effect on the current target. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindTargetEffect(ushort effectID) => FindEffect(effectID, CurrentTarget, LocalPlayer?.ObjectId);

        /// <summary></summary>
        public static float GetDebuffRemainingTime(ushort effectId)
        {
            Status? eff = FindTargetEffect(effectId);
            return eff?.RemainingTime ?? 0;
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
        public static Status? FindEffect(ushort effectID, GameObject? obj, uint? sourceID) => Service.ComboCache.GetStatus(effectID, obj, sourceID);

        ///<summary> Checks a member object for an effect. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <param name="obj"></param>
        /// <return> Status object or null. </return>
        public static Status? FindEffectOnMember(ushort effectID, GameObject? obj) => Service.ComboCache.GetStatus(effectID, obj, null);

        /// <summary> Returns the name of a status effect from its ID. </summary>
        /// <param name="id"> ID of the status. </param>
        /// <returns></returns>
        public static string GetStatusName(uint id) => ActionWatching.GetStatusName(id);

        /// <summary> Checks if the character has the Silence status. </summary>
        /// <returns></returns>
        public static bool HasSilence()
        {
            foreach (uint status in ActionWatching.GetStatusesByName("Silence"))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Pacification status. </summary>
        /// <returns></returns>
        public static bool HasPacification()
        {
            foreach (uint status in ActionWatching.GetStatusesByName("Pacification"))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Amnesia status. </summary>
        /// <returns></returns>
        public static bool HasAmnesia()
        {
            foreach (uint status in ActionWatching.GetStatusesByName("Amnesia"))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }
    }
}
