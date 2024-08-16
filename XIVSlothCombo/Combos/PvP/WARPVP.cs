using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class WARPvP
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        internal const uint
            HeavySwing = 29074,
            Maim = 29075,
            StormsPath = 29076,
            PrimalRend = 29084,
            Onslaught = 29079,
            Orogeny = 29080,
            Blota = 29081,
            PrimalScream = 29083, //LB
            Bloodwhetting = 29082;

        internal class Buffs
        {
            internal const ushort
                NascentChaos = 1992,
                InnerRelease = 1303;
        }

        internal class Debuffs
        {
            internal const ushort
                Onslaught = 3029;
        }

        internal class WARPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WARPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                bool enemyStun = TargetHasEffectAny(PvPCommon.Debuffs.Stun);
                bool enemyGuard = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                var canWeave = CanWeave(actionID);

                if (actionID is HeavySwing or Maim or StormsPath)
                {
                    if (canWeave && !enemyGuard)
                    {
                        //Orogeny
                        if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_Orogeny) &&
                            ActionReady(Orogeny) && InMeleeRange() && !enemyGuard) //use on CD
                            return OriginalHook(Orogeny);
                    }

                    //Bloodwhetting
                    if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_Bloodwhetting) &&
                        ActionReady(Bloodwhetting) && (ActionReady(PrimalRend) || GetCooldownRemainingTime(PrimalRend) > 5) && GetCooldownRemainingTime(Onslaught) < 1) //use before Primal Rend burst
                        return OriginalHook(Bloodwhetting);

                    //Blota
                    if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_Blota) && !enemyStun && !enemyGuard &&
                        (!InMeleeRange() && ActionReady(Blota) && !enemyStun && canWeave) || //use when out of range
                        (!IsEnabled(CustomComboPreset.WARPvP_BurstMode_Blota) && IsEnabled(CustomComboPreset.WARPvP_BurstMode_Stunlock) && JustUsed(PrimalRend, 3f))) //stunlock
                        return OriginalHook(Blota);

                    //Onslaught
                    if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_Onslaught) && 
                        ActionReady(Onslaught) && (ActionReady(PrimalRend)) && !enemyGuard)
                        return OriginalHook(Onslaught);

                    //ChaoticCyclone
                    if (IsEnabled(CustomComboPreset.WARPvP_BurstMode_ChaoticCyclone) && 
                        HasEffect(Buffs.NascentChaos) && !ActionReady(PrimalRend) && InMeleeRange()) //use on CD
                        return OriginalHook(Bloodwhetting);

                    //PrimalRend
                    if (!enemyGuard && JustUsed(Onslaught, 3f) && ActionReady(PrimalRend) &&
                        (IsEnabled(CustomComboPreset.WARPvP_BurstMode_PrimalRend) || //use on CD
                       (!IsEnabled(CustomComboPreset.WARPvP_BurstMode_PrimalRend) && IsEnabled(CustomComboPreset.WARPvP_BurstMode_Stunlock) && GetCooldownRemainingTime(Blota) < 0.6f))) //stunlock
                        return OriginalHook(PrimalRend);
                }

                return actionID;
            }
        }
    }
}