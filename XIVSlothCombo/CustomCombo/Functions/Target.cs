using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;
using System.Linq;
using System.Numerics;
using XIVSlothCombo.Data;
using XIVSlothCombo.Services;
using StructsObject = FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Gets the current target or null. </summary>
        public static IGameObject? CurrentTarget => Svc.Targets.Target;

        /// <summary> Find if the player has a target. </summary>
        /// <returns> A value indicating whether the player has a target. </returns>
        public static bool HasTarget() => CurrentTarget is not null;

        /// <summary> Gets the distance from the target. </summary>
        /// <returns> Double representing the distance from the target. </returns>
        public static float GetTargetDistance()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return 0;

            if (CurrentTarget is not IBattleChara chara)
                return 0;

            if (CurrentTarget.GameObjectId == LocalPlayer.GameObjectId)
                return 0;

            Vector2 position = new(chara.Position.X, chara.Position.Z);
            Vector2 selfPosition = new(LocalPlayer.Position.X, LocalPlayer.Position.Z);

            return Math.Max(0, Vector2.Distance(position, selfPosition) - chara.HitboxRadius - LocalPlayer.HitboxRadius);
        }

        /// <summary> Gets a value indicating whether you are in melee range from the current target. </summary>
        /// <returns> Bool indicating whether you are in melee range. </returns>
        public static bool InMeleeRange()
        {
            if (LocalPlayer.TargetObject == null)
                return false;

            float distance = GetTargetDistance();

            if (distance == 0)
                return true;

            if (distance > 3 + Service.Configuration.MeleeOffset)
                return false;

            return true;
        }

        /// <summary> Gets a value indicating target's HP Percent. CurrentTarget is default unless specified </summary>
        /// <returns> Double indicating percentage. </returns>
        public static float GetTargetHPPercent(IGameObject? OurTarget = null)
        {
            if (OurTarget is null)
            {
                OurTarget = CurrentTarget; // Fallback to CurrentTarget
                if (OurTarget is null)
                    return 0;
            }

            return OurTarget is not IBattleChara chara
                ? 0
                : (float)chara.CurrentHp / chara.MaxHp * 100;
        }

        public static float EnemyHealthMaxHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not IBattleChara chara)
                return 0;

            return chara.MaxHp;
        }

        public static float EnemyHealthCurrentHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not IBattleChara chara)
                return 0;

            return chara.CurrentHp;
        }

        public static float PlayerHealthPercentageHp() => (float)LocalPlayer.CurrentHp / LocalPlayer.MaxHp * 100;

        public static bool HasBattleTarget() => CurrentTarget is IBattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy or (BattleNpcSubKind)1 };

        public static bool HasFriendlyTarget(IGameObject? OurTarget = null)
        {
            if (OurTarget is null)
            {
                //Fallback to CurrentTarget
                OurTarget = CurrentTarget;
                if (OurTarget is null)
                    return false;
            }

            //Humans and Trusts
            if (OurTarget.ObjectKind is ObjectKind.Player)
                return true;
            //AI
            if (OurTarget is IBattleNpc) return (OurTarget as IBattleNpc).BattleNpcKind is not BattleNpcSubKind.Enemy and not (BattleNpcSubKind)1;
            return false;
        }

        /// <summary> Grabs healable target. Checks Soft Target then Hard Target. 
        /// If Party UI Mouseover is enabled, find the target and return that. Else return the player. </summary>
        /// <param name="checkMOPartyUI">Checks for a mouseover target.</param>
        /// <param name="restrictToMouseover">Forces only the mouseover target, may return null.</param>
        /// <returns> IGameObject of a player target. </returns>
        public static unsafe IGameObject? GetHealTarget(bool checkMOPartyUI = false, bool restrictToMouseover = false)
        {
            IGameObject? healTarget = null;
            ITargetManager tm = Svc.Targets;

            if (HasFriendlyTarget(tm.SoftTarget)) healTarget = tm.SoftTarget;
            if (healTarget is null && HasFriendlyTarget(CurrentTarget) && !restrictToMouseover) healTarget = CurrentTarget;
            //if (checkMO && HasFriendlyTarget(tm.MouseOverTarget)) healTarget = tm.MouseOverTarget;
            if (checkMOPartyUI)
            {
                StructsObject.GameObject* t = Framework.Instance()->GetUIModule()->GetPronounModule()->UiMouseOverTarget;
                if (t != null && t->GetGameObjectId().ObjectId != 0)
                {
                    IGameObject? uiTarget = Svc.Objects.Where(x => x.GameObjectId == t->GetGameObjectId().ObjectId).FirstOrDefault();
                    if (uiTarget != null && HasFriendlyTarget(uiTarget)) healTarget = uiTarget;

                    if (restrictToMouseover)
                        return healTarget;
                }

                if (restrictToMouseover)
                    return healTarget;
            }
            healTarget ??= LocalPlayer;
            return healTarget;
        }

        /// <summary> Determines if the enemy can be interrupted if they are currently casting. </summary>
        /// <returns> Bool indicating whether they can be interrupted or not. </returns>
        public static bool CanInterruptEnemy()
        {
            if (CurrentTarget is null)
                return false;
            if (CurrentTarget is not IBattleChara chara)
                return false;
            if (chara.IsCasting)
                return chara.IsCastInterruptible;

            return false;
        }

        /// <summary> Sets the player's target. </summary>
        /// <param name="target"> Target must be a game object that the player can normally click and target. </param>
        public static void SetTarget(IGameObject? target) => Svc.Targets.Target = target;

        /// <summary> Checks if target is in appropriate range for targeting </summary>
        /// <param name="target"> The target object to check </param>
        public static bool IsInRange(IGameObject? target)
        {
            if (target == null || target.YalmDistanceX >= 30)
                return false;

            return true;
        }

        public static bool TargetNeedsPositionals()
        {
            if (!HasBattleTarget()) return false;
            if (TargetHasEffectAny(3808)) return false; // Directional Disregard Effect (Patch 7.01)
            if (ActionWatching.BNpcSheet.TryGetValue(CurrentTarget.DataId, out var bnpc) && !bnpc.Unknown10) return true;
            return false;
        }

        /// <summary> Attempts to target the given party member </summary>
        /// <param name="target"></param>
        protected static unsafe void TargetObject(TargetType target)
        {
            StructsObject.GameObject* t = GetTarget(target);
            if (t == null) return;
            ulong o = PartyTargetingService.GetObjectID(t);
            IGameObject? p = Svc.Objects.Where(x => x.GameObjectId == o).First();

            if (IsInRange(p)) SetTarget(p);
        }

        public static void TargetObject(IGameObject? target)
        {
            if (IsInRange(target)) SetTarget(target);
        }

        public unsafe static StructsObject.GameObject* GetTarget(TargetType target)
        {
            IGameObject? o = null;

            switch (target)
            {
                case TargetType.Target:
                    o = Svc.Targets.Target;
                    break;
                case TargetType.SoftTarget:
                    o = Svc.Targets.SoftTarget;
                    break;
                case TargetType.FocusTarget:
                    o = Svc.Targets.FocusTarget;
                    break;
                case TargetType.UITarget:
                    return PartyTargetingService.UITarget;
                case TargetType.FieldTarget:
                    o = Svc.Targets.MouseOverTarget;
                    break;
                case TargetType.TargetsTarget when Svc.Targets.Target is { TargetObjectId: not 0xE0000000 }:
                    o = Svc.Targets.Target.TargetObject;
                    break;
                case TargetType.Self:
                    o = Svc.ClientState.LocalPlayer;
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

            return o != null ? (StructsObject.GameObject*)o.Address : null;
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

        /// <summary>
        /// Get angle to target.
        /// </summary>
        /// <returns>Angle relative to target</returns>
        public static float AngleToTarget()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return 0;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return 0;

            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            if ((regionDegrees >= 45) && (regionDegrees <= 135))
            {
                return 1;
            }
            if ((regionDegrees >= 135) && (regionDegrees <= 225))
            {
                return 2;
            }
            if ((regionDegrees >= 225) && (regionDegrees <= 315))
            {
                return 3;
            }
            if ((regionDegrees >= 315) || (regionDegrees <= 45))
            {
                return 4;
            }
            return 0;
        }

        /// <summary>
        /// Is player on target's rear.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool OnTargetsRear()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return false;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return false;

            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            if ((regionDegrees >= 135) && (regionDegrees <= 225))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is player on target's flank.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool OnTargetsFlank()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return false;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return false;


            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            // left flank
            if ((regionDegrees >= 45) && (regionDegrees <= 135))
            {
                return true;
            }
            // right flank
            if ((regionDegrees >= 225) && (regionDegrees <= 315))
            {
                return true;
            }
            return false;
        }

        // the following is all lifted from the excellent Resonant plugin
        internal static class PositionalMath
        {
            static internal float Radians(float degrees)
            {
                return (float)Math.PI * degrees / 180.0f;
            }

            static internal double Degrees(float radians)
            {
                return (180 / Math.PI) * radians;
            }

            static internal float AngleXZ(Vector3 a, Vector3 b)
            {
                return (float)Math.Atan2(b.X - a.X, b.Z - a.Z);
            }
        }

        internal unsafe static bool OutOfRange(uint actionID, IGameObject target) => ActionWatching.OutOfRange(actionID, Svc.ClientState.LocalPlayer!, target);

    }
}
