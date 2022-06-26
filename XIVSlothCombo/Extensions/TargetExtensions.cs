using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using System;
using System.Numerics;
using System.Linq;
using XIVSlothCombo.CustomComboNS.Functions; //Needed for LocalPlayer for Distance Mathematics
using XIVSlothCombo.Services;


namespace XIVSlothCombo.Extensions
{
    internal static class TargetExtensions
    {
        /*
         * Example useage
         * CurrentTarget.GetHPPercent
         * CurrentTarget.IsEnemy
         * etc
        */

        /// <summary> Gets a value indicating a specified target's HP Percent</summary>
        /// <returns> float representing percentage. </returns>
        internal static float GetHPPercent(this GameObject? target) => target is not BattleChara chara ? 0 : (float)chara.CurrentHp / chara.MaxHp * 100;
        
        /// <summary> Gets a specified target's Maximum HP</summary>
        /// <returns> uint representing Max HP value </returns>
        internal static uint GetMaxHP(this GameObject? target) => target is not BattleChara chara ? 0 : chara.MaxHp;

        /// <summary> Gets a specified target's Current HP</summary>
        /// <returns> uint representing a HP value </returns>
        internal static uint GetHP(this GameObject? target) => target is not BattleChara chara ? 0 : chara.CurrentHp;

        /// <summary> Determines if the current target is an Enemy</summary>
        /// <returns> bool indicating if the current target is an Enemy</returns>
        internal static bool IsEnemy(this GameObject? target) => (target as BattleNpc)?.BattleNpcKind is BattleNpcSubKind.Enemy;

        internal static bool IsPlayer(this GameObject? target) => (target?.ObjectKind is ObjectKind.Player);

        /// <summary> Determines if the enemy can be interrupted if they are currently casting. </summary>
        /// <returns> Bool indicating whether they can be interrupted or not. </returns>
        internal static bool CanInterupt(this GameObject? target) => ((target as BattleChara)?.IsCastInterruptible is true);

        /// <summary> Gets distance from player to target </summary>
        /// <returns> Double representing distance</returns>
        internal static float GetDistance(this GameObject? target)
        {
            if (target is null || CustomComboFunctions.LocalPlayer is null)
                return 0;

            if (target is not BattleChara chara)
                return 0;

            if (target.ObjectId == CustomComboFunctions.LocalPlayer.ObjectId)
                return 0;

            var position = new Vector2(chara.Position.X, chara.Position.Z);
            var selfPosition = new Vector2(CustomComboFunctions.LocalPlayer.Position.X, CustomComboFunctions.LocalPlayer.Position.Z);

            return Math.Max(0, (Vector2.Distance(position, selfPosition) - chara.HitboxRadius) - CustomComboFunctions.LocalPlayer.HitboxRadius);
        }

        /// <summary> Gets a value indicating whether you are in melee range from the current target. </summary>
        /// <returns> Bool indicating whether you are in melee range. </returns>
        internal static bool InMeleeRange(this GameObject? target)
        {
            if (CustomComboFunctions.LocalPlayer.TargetObject == null) return false;
            var distance = target.GetDistance();

            if (distance == 0)
                return true;

            if (distance > 3 + Service.Configuration.MeleeOffset)
                return false;

            return true;
        }

        /// <summary> Checks if target is in appropriate range for targeting </summary>
        internal static bool IsInRange(this GameObject? target) => target.YalmDistanceX < 30;

        /// <summary> Attempts to target the given party member </summary>
        /// <param name="target"></param>
        /*internal static unsafe void SetAsTarget(CustomComboFunctions.TargetType target)
        {
            //CustomComboFunctions.GetTarget is protected. Extension commented out until wiser ones can fix?
            var t = CustomComboFunctions.GetTarget(target);
            if (t == null) return;
            var o = PartyTargetingService.GetObjectID(t);
            var p = Service.ObjectTable.Where(x => x.ObjectId == o).First();

            if (IsInRange(p)) Service.TargetManager.Target = p;
        }*/

        internal static void SetAsTarget(this GameObject? target)
        {
            if (target.IsInRange()) Service.TargetManager.Target = target;
        }
    }



}
