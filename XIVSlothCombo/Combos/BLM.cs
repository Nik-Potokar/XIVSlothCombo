using System;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BLM
    {
        public const byte ClassID = 7;
        public const byte JobID = 25;

        public const uint
            Fire = 141,
            Blizzard = 142,
            Thunder = 144,
            Blizzard2 = 25793,
            Transpose = 149,
            Fire2 = 147,
            Fire3 = 152,
            Thunder3 = 153,
            Thunder2 = 7447,
            Thunder4 = 7420,
            Blizzard3 = 154,
            Scathe = 156,
            Freeze = 159,
            Flare = 162,
            LeyLines = 3573,
            Blizzard4 = 3576,
            Fire4 = 3577,
            BetweenTheLines = 7419,
            Despair = 16505,
            UmbralSoul = 16506,
            Paradox = 25797,
            Amplifier = 25796,
            HighFire2 = 25794,
            HighBlizzard2 = 25795,
            Xenoglossy = 16507,
            Foul = 7422,
            Sharpcast = 3574,
            Manafont = 158,
            Triplecast = 7421;

        public static class Buffs
        {
            public const ushort
                Thundercloud = 164,
                LeyLines = 737,
                Firestarter = 165,
                Sharpcast = 867,
                Triplecast = 1211;
        }

        public static class Debuffs
        {
            public const ushort
                Thunder = 161,
                Thunder2 = 162,
                Thunder3 = 163,
                Thunder4 = 1210;
        }

        public static class Levels
        {
            public const byte
                Thunder2 = 26,
                Manafont = 30,
                Fire3 = 35,
                Blizzard3 = 35,
                Freeze = 40,
                Thunder3 = 45,
                Flare = 50,
                LeyLines = 52,
                Sharpcast = 54,
                Blizzard4 = 58,
                Fire4 = 60,
                BetweenTheLines = 62,
                Thunder4 = 64,
                Triplecast = 66,
                Foul = 70,
                Despair = 72,
                UmbralSoul = 76,
                Xenoglossy = 80,
                HighFire2 = 82,
                HighBlizzard2 = 82,
                EnhancedManafont = 84,
                Amplifier = 86,
                Paradox = 90;
        }

        public static class MP
        {
            public const uint
                Thunder = 200,
                AspectThunder = 400,
                Fire = 800,
                Despair = 800,
                Blizzard3 = 800,
                AspectFire = 1600,
                Fire3 = 2000,
                MaxMP = 10000;
        }
        public static class Config
        {
            public const string BLM_PolyglotsStored = "BLM_PolyglotsStored";
            public const string BLM_AstralFireRefresh = "BLM_AstralFireRefresh";
            public const string BLM_MovementTime = "BLM_MovementTime";
        }


        internal class BLM_Blizzard : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Blizzard)
                {
                    var gauge = GetJobGauge<BLMGauge>().InUmbralIce;
                    if (level >= Levels.Freeze && !gauge)
                    {
                        return Blizzard3;
                    }
                }

                if (actionID == Freeze && level < Levels.Freeze)
                {
                    return Blizzard2;
                }

                return actionID;
            }
        }

        internal class BLM_Fire_1to3 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Fire)
                {
                    var gauge = GetJobGauge<BLMGauge>();
                    if ((level >= Levels.Fire3 && !gauge.InAstralFire) || HasEffect(Buffs.Firestarter))
                    {
                        return Fire3;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_LeyLines : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_LeyLines;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == LeyLines && HasEffect(Buffs.LeyLines) && level >= Levels.BetweenTheLines)
                {
                    return BetweenTheLines;
                }

                return actionID;
            }
        }

        internal class BLM_Mana : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Mana;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Transpose)
                {
                    var gauge = GetJobGauge<BLMGauge>();
                    if (gauge.InUmbralIce && level >= Levels.UmbralSoul)
                    {
                        return UmbralSoul;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_Enochian : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Enochian;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Scathe)
                {
                    var gauge = GetJobGauge<BLMGauge>();
                    var GCD = GetCooldown(actionID);
                    var thundercloudduration = FindEffectAny(Buffs.Thundercloud);
                    var thunderdebuffontarget = FindTargetEffect(Debuffs.Thunder3);
                    var thunderOneDebuff = FindTargetEffect(Debuffs.Thunder);
                    var thunder3DebuffOnTarget = TargetHasEffect(Debuffs.Thunder3);

                    if (gauge.InUmbralIce && level >= Levels.Blizzard4)
                    {
                        if (gauge.ElementTimeRemaining >= 0 && IsEnabled(CustomComboPreset.BLM_Thunder))
                        {
                            if (HasEffect(Buffs.Thundercloud))
                            {
                                if ((TargetHasEffect(Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                    return Thunder3;
                            }

                            if (IsEnabled(CustomComboPreset.BLM_ThunderUptime) && !thunder3DebuffOnTarget && lastComboMove != Thunder3 && LocalPlayer.CurrentMp >= 400)
                                return Thunder3;

                            if (gauge.IsParadoxActive && level >= 90)
                                return Paradox;

                            if (IsEnabled(CustomComboPreset.BLM_AspectSwap) && gauge.UmbralHearts == 3 && LocalPlayer.CurrentMp >= 10000)
                                return Fire3;

                        }

                        return Blizzard4;
                    }

                    if (level >= Levels.Fire4)
                    {
                        if (gauge.ElementTimeRemaining >= 6000 && IsEnabled(CustomComboPreset.BLM_Thunder))
                        {
                            if (HasEffect(Buffs.Thundercloud))
                            {
                                if ((TargetHasEffect(Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                    return Thunder3;
                            }

                            if (IsEnabled(CustomComboPreset.BLM_ThunderUptime) && !thunder3DebuffOnTarget && lastComboMove != Thunder3 && LocalPlayer.CurrentMp >= 400)
                                return Thunder3;
                        }

                        if (gauge.ElementTimeRemaining < 3000 && HasEffect(Buffs.Firestarter) && IsEnabled(CustomComboPreset.BLM_Fire_1to3))
                        {
                            return Fire3;
                        }

                        if (IsEnabled(CustomComboPreset.BLM_AspectSwap) && level >= Levels.Blizzard3)
                        {
                            if ((LocalPlayer.CurrentMp < 800) || (LocalPlayer.CurrentMp < 1600 && level < Levels.Despair))
                                return Blizzard3;
                        }

                        if (gauge.ElementTimeRemaining > 0 && LocalPlayer.CurrentMp < 2400 && level >= Levels.Despair && IsEnabled(CustomComboPreset.BLM_Despair))
                        {
                            return Despair;
                        }

                        if (gauge.IsEnochianActive)
                        {
                            if (gauge.ElementTimeRemaining < 6000 && !HasEffect(Buffs.Firestarter) && IsEnabled(CustomComboPreset.BLM_Fire_1to3) && level == 90 && gauge.IsParadoxActive)
                                return Paradox;
                            if (gauge.ElementTimeRemaining < 6000 && !HasEffect(Buffs.Firestarter) && IsEnabled(CustomComboPreset.BLM_Fire_1to3) && !gauge.IsParadoxActive)
                                return Fire;
                        }

                        return Fire4;
                    }

                    if (gauge.ElementTimeRemaining >= 5000 && IsEnabled(CustomComboPreset.BLM_Thunder))
                    {
                        if (level < Levels.Thunder3)
                        {
                            if (HasEffect(Buffs.Thundercloud))
                            {
                                if (TargetHasEffect(Debuffs.Thunder) && thunderOneDebuff.RemainingTime < 4)
                                    return Thunder;
                            }
                        }
                        else
                        {
                            if (HasEffect(Buffs.Thundercloud))
                            {
                                if (TargetHasEffect(Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4)
                                    return Thunder3;
                            }
                        }

                        if (level < Levels.Thunder3)
                        {
                            if (IsEnabled(CustomComboPreset.BLM_ThunderUptime) && !TargetHasEffect(Debuffs.Thunder) && lastComboMove != Thunder && LocalPlayer.CurrentMp >= 200)
                                return Thunder;
                        }
                        else
                        {
                            if (IsEnabled(CustomComboPreset.BLM_ThunderUptime) && !TargetHasEffect(Debuffs.Thunder3) && lastComboMove != Thunder3 && LocalPlayer.CurrentMp >= 400)
                                return Thunder3;
                        }
                    }

                    if (level < Levels.Fire3)
                    {
                        return Fire;
                    }

                    if (gauge.InAstralFire)
                    {
                        if (HasEffect(Buffs.Firestarter) && level == 90)
                            return Paradox;
                        if (HasEffect(Buffs.Firestarter))
                            return Fire3;
                        if (IsEnabled(CustomComboPreset.BLM_AspectSwap) && LocalPlayer.CurrentMp < 1600 && level >= Levels.Blizzard3)
                            return Blizzard3;

                        return Fire;
                    }

                    if (gauge.InUmbralIce)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_AspectSwap) && LocalPlayer.CurrentMp >= 10000 && level >= Levels.Fire3)
                            return Fire3;

                        return Blizzard;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Flare)
                {
                    var gauge = GetJobGauge<BLMGauge>();
                    var thunder4Debuff = TargetHasEffect(Debuffs.Thunder4);
                    var thunder4Timer = FindTargetEffect(Debuffs.Thunder4);
                    var thunder2Debuff = TargetHasEffect(Debuffs.Thunder2);
                    var thunder2Timer = FindTargetEffect(Debuffs.Thunder2);
                    var currentMP = LocalPlayer.CurrentMp;
                    var polyToStore = Service.Configuration.GetCustomIntValue(Config.BLM_PolyglotsStored);

                    // Polyglot usage
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Simple_Foul) && level >= Levels.Manafont && level >= Levels.Foul)
                    {
                        if (gauge.InAstralFire && currentMP <= MP.AspectFire && IsOffCooldown(Manafont) && CanSpellWeave(actionID) && lastComboMove == Foul)
                        {
                            return Manafont;
                        }

                        if ((gauge.InAstralFire && currentMP <= MP.AspectFire && IsOffCooldown(Manafont) && gauge.PolyglotStacks >= 1) || (IsOnCooldown(Manafont) && (GetCooldownRemainingTime(Manafont) >= 30 && gauge.PolyglotStacks > polyToStore)))
                        {
                            return Foul;
                        }
                    }

                    // Thunder uptime
                    if (currentMP >= MP.AspectThunder && lastComboMove != Manafont)
                    {
                        if (level >= Levels.Thunder4)
                        {
                            if (lastComboMove != Thunder4 && (!thunder4Debuff || thunder4Timer.RemainingTime <= 4) &&
                               ((gauge.InUmbralIce && gauge.UmbralHearts == 3) ||
                                (gauge.InAstralFire && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))))
                            {
                                return Thunder4;
                            }
                        }
                        else if (level >= Levels.Thunder2)
                        {
                            if (lastComboMove != Thunder2 && (!thunder2Debuff || thunder2Timer.RemainingTime <= 4) &&
                               ((gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < Levels.Blizzard4)) ||
                                (gauge.InAstralFire && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))))
                            {
                                return Thunder2;
                            }
                        }
                    }

                    // Fire 2 / Flare
                    if (gauge.InAstralFire)
                    {
                        if (currentMP >= 7000)
                        {
                            if (gauge.UmbralHearts == 1)
                            {
                                return Flare;
                            }
                            return level >= Levels.HighFire2 ? HighFire2 : Fire2;
                        }
                        else if (currentMP >= MP.Despair)
                        {
                            if (level >= Levels.Flare)
                            {
                                return Flare;
                            }
                            else if (currentMP >= MP.AspectFire)
                            {
                                return Fire2;
                            }
                        }
                        else if (level < Levels.Fire3)
                        {
                            return Transpose;
                        }
                    }

                    // Umbral Hearts
                    if (gauge.InUmbralIce)
                    {
                        if (level >= Levels.Blizzard4 && gauge.UmbralHearts <= 2)
                        {
                            return Freeze;
                        }
                        else if (level >= Levels.Freeze && currentMP < MP.MaxMP - MP.AspectThunder)
                        {
                            return Freeze;
                        }
                        if (level < Levels.Fire3)
                        {
                            return (currentMP >= MP.MaxMP - MP.AspectThunder) ? Transpose : Blizzard2;
                        }
                        return level >= Levels.HighFire2 ? HighFire2 : Fire2;
                    }

                    return level >= Levels.HighBlizzard2 ? HighBlizzard2 : Blizzard2;
                }

                return actionID;
            }
        }

        internal class BLM_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_SimpleMode;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;
            internal static double movementTime = 0.0f;
            internal static DateTime previousTime;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Scathe)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<BLMGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = Service.Configuration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;

                    var thunder = TargetHasEffect(Debuffs.Thunder);
                    var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    var thunderDuration = FindTargetEffect(Debuffs.Thunder);
                    var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    DotRecast thunderRecast = delegate (int duration)
                    {
                        return !thunder || (thunder && thunderDuration.RemainingTime < duration);
                    };
                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Opener for BLM
                    // Credit to damolitionn for providing code to be used as a base for this opener
                    if (IsEnabled(CustomComboPreset.BLM_Simple_Opener) && level >= Levels.Foul)
                    {
                        // Only enable sharpcast if it's available
                        if (!inOpener && !HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && lastComboMove != Thunder3)
                        {
                            return Sharpcast;
                        }

                        if (!inCombat && (inOpener || openerFinished))
                        {
                            inOpener = false;
                            openerFinished = false;
                        }

                        if (inCombat && !inOpener)
                        {
                            inOpener = true;
                        }

                        if (inCombat && inOpener && !openerFinished)
                        {
                            // Exit out of opener if Enochian is lost
                            if (!gauge.IsEnochianActive)
                            {
                                openerFinished = true;
                                return Blizzard3;
                            }

                            if (gauge.InAstralFire)
                            {
                                // First Triplecast
                                if (lastComboMove != Triplecast && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) >= 1)
                                {
                                    var triplecastMP = 7600;
                                    if (IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate))
                                    {
                                        triplecastMP = 6000;
                                    }
                                    if (currentMP <= triplecastMP)
                                    {
                                        return Triplecast;
                                    }
                                }

                                // Weave other oGCDs
                                if (canWeave)
                                {
                                    // Weave Amplifier and Ley Lines
                                    if (currentMP <= 4400)
                                    {
                                        if (level >= Levels.Amplifier && IsOffCooldown(Amplifier))
                                        {
                                            return Amplifier;
                                        }
                                        if (level >= Levels.LeyLines && IsOffCooldown(LeyLines))
                                        {
                                            return LeyLines;
                                        }
                                    }

                                    // Swiftcast
                                    if (IsOffCooldown(All.Swiftcast) && IsOnCooldown(LeyLines))
                                    {
                                        return All.Swiftcast;
                                    }

                                    // Manafont
                                    if (IsOffCooldown(Manafont) && (lastComboMove == Despair || lastComboMove == Fire))
                                    {
                                        if (level >= Levels.Despair)
                                        {
                                            if (currentMP < MP.Despair)
                                            {
                                                return Manafont;
                                            }
                                        }
                                        else if (currentMP < MP.AspectFire)
                                        {
                                            return Manafont;
                                        }
                                    }

                                    // Second Triplecast / Sharpcast
                                    if (!IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate))
                                    {
                                        if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) &&
                                            lastComboMove != All.Swiftcast && GetRemainingCharges(Triplecast) >= 1 && currentMP < MP.AspectFire)
                                        {
                                            return Triplecast;
                                        }

                                        if (!HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && IsOnCooldown(Manafont) &&
                                            lastComboMove == Fire4)
                                        {
                                            return Sharpcast;
                                        }
                                    }
                                }

                                // Cast Despair
                                if (level >= Levels.Despair && (currentMP < MP.AspectFire || gauge.ElementTimeRemaining <= 4000) && currentMP >= MP.Despair)
                                {
                                    return Despair;
                                }

                                // Cast Fire
                                if (level < Levels.Despair && gauge.ElementTimeRemaining <= 6000 && currentMP >= MP.AspectFire)
                                {
                                    return Fire;
                                }

                                // Cast Fire 4 after Manafont
                                if (IsOnCooldown(Manafont))
                                {
                                    if ((level < Levels.EnhancedManafont && GetCooldownRemainingTime(Manafont) >= 179) ||
                                        (level >= Levels.EnhancedManafont && GetCooldownRemainingTime(Manafont) >= 119))
                                    {
                                        return Fire4;
                                    }
                                }

                                // Fire4 / Umbral Ice
                                return currentMP >= MP.AspectFire ? Fire4 : Blizzard3;
                            }

                            if (gauge.InUmbralIce)
                            {
                                // Dump Polyglot Stacks
                                if (gauge.PolyglotStacks >= 1 && gauge.ElementTimeRemaining >= 6000)
                                {
                                    return level >= Levels.Xenoglossy ? Xenoglossy : Foul;
                                }
                                if (gauge.IsParadoxActive && level >= Levels.Paradox)
                                {
                                    return Paradox;
                                }
                                if (gauge.UmbralHearts < 3 && lastComboMove != Blizzard4)
                                {
                                    return Blizzard4;
                                }

                                // Refresh Thunder3
                                if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                                {
                                    return Thunder3;
                                }

                                openerFinished = true;
                            }
                        }
                    }

                    // Handle movement
                    if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement) && inCombat)
                    {
                        var movementTimeThreshold = Service.Configuration.GetCustomFloatValue(Config.BLM_MovementTime);
                        double deltaTime = (DateTime.Now - previousTime).TotalSeconds;
                        previousTime = DateTime.Now;
                        if (IsMoving)
                        {
                            movementTime = movementTime + deltaTime > movementTimeThreshold + 0.02 ? movementTimeThreshold + 0.02 : movementTime + deltaTime;
                        }
                        else
                        {
                            movementTime = movementTime - deltaTime < 0 ? 0 : movementTime - (deltaTime * 2);
                        }

                        if (movementTime > movementTimeThreshold && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                        {
                            if (inCombat && LocalPlayer.CurrentCastTime == 0.0f)
                            {
                                if (level >= Levels.Paradox && gauge.IsParadoxActive && gauge.InUmbralIce)
                                {
                                    return Paradox;
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Xeno) && level >= Levels.Xenoglossy && gauge.PolyglotStacks > 0)
                                {
                                    return Xenoglossy;
                                }
                                if (HasEffect(Buffs.Thundercloud))
                                {
                                    if (level < Levels.Thunder3)
                                    {
                                        if (lastComboMove != Thunder && thunderRecast(4) && !TargetHasEffect(Debuffs.Thunder2))
                                        {
                                            return Thunder;
                                        }
                                    }
                                    else if (lastComboMove != Thunder3 && thunder3Recast(4) && !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                                    {
                                        return Thunder3;
                                    }
                                }
                                if (IsOffCooldown(All.Swiftcast))
                                {
                                    return All.Swiftcast;
                                }
                                if (level >= Levels.Triplecast && GetRemainingCharges(Triplecast) >= 1)
                                {
                                    return Triplecast;
                                }
                                if (HasEffect(Buffs.Firestarter) && gauge.InAstralFire)
                                {
                                    return Fire3;
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Scathe))
                                {
                                    return Scathe;
                                }
                            }
                        }
                    }

                    // Handle thunder uptime and buffs
                    if (gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder uptime
                        if (gauge.ElementTimeRemaining >= astralFireRefresh && (HasEffect(Buffs.Thundercloud) || currentMP >= MP.AspectThunder))
                        {
                            if (level < Levels.Thunder3)
                            {
                                if (lastComboMove != Thunder && thunderRecast(4) && !TargetHasEffect(Debuffs.Thunder2))
                                {
                                    return Thunder;
                                }
                            }
                            else if (lastComboMove != Thunder3 && thunder3Recast(4) && !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                            {
                                return Thunder3;
                            }
                        }

                        // Buffs
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.BLM_Simple_Casts))
                            {
                                // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                                if (level >= Levels.Triplecast && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) > 0 &&
                                    (gauge.InAstralFire || gauge.UmbralHearts == 3) && currentMP >= MP.AspectFire * 2)
                                {
                                    if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1)
                                    {
                                        return Triplecast;
                                    }
                                }

                                // Use Swiftcast in Astral Fire
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) && level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) &&
                                     gauge.InAstralFire && currentMP >= MP.AspectFire * (HasEffect(Buffs.Triplecast) ? 3 : 1))
                                {
                                    if (level >= Levels.Despair && currentMP >= MP.Despair)
                                    {
                                        return All.Swiftcast;
                                    }
                                    else if (currentMP >= MP.AspectFire)
                                    {
                                        return All.Swiftcast;
                                    }
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs))
                            {
                                if (level >= Levels.Amplifier && IsOffCooldown(Amplifier) && gauge.PolyglotStacks < 2)
                                {
                                    return Amplifier;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs_LeyLines))
                            {
                                if (level >= Levels.LeyLines && IsOffCooldown(LeyLines))
                                {
                                    return LeyLines;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs))
                            {
                                if (IsOffCooldown(Manafont) && gauge.InAstralFire)
                                {
                                    if (level >= Levels.Despair)
                                    {
                                        if (currentMP < MP.Despair)
                                        {
                                            return Manafont;
                                        }
                                    }
                                    else if (currentMP < MP.AspectFire)
                                    {
                                        return Manafont;
                                    }
                                }
                                if (level >= Levels.Sharpcast && lastComboMove != Thunder3 && GetRemainingCharges(Sharpcast) >= 1 && !HasEffect(Buffs.Sharpcast))
                                {
                                    // Try to only sharpcast Thunder 3
                                    if (thunder3Recast(7) || GetRemainingCharges(Sharpcast) == 2 ||
                                       (thunder3Recast(15) && (gauge.InUmbralIce || (gauge.InAstralFire && !gauge.IsParadoxActive))))
                                    {
                                        return Sharpcast;
                                    }
                                }
                            }
                        }
                    }

                    // Handle initial cast
                    if ((level >= Levels.Blizzard4 && !gauge.IsEnochianActive) || gauge.ElementTimeRemaining <= 0)
                    {
                        if (level >= Levels.Fire3)
                        {
                            return (currentMP >= MP.Fire3) ? Fire3 : Blizzard3;
                        }
                        return (currentMP >= MP.Fire) ? Fire : Blizzard;
                    }

                    // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                    if (level < Levels.Blizzard3)
                    {
                        if (gauge.InAstralFire)
                        {
                            return (currentMP < MP.AspectFire) ? Transpose : Fire;
                        }
                        if (gauge.InUmbralIce)
                        {
                            return (currentMP >= MP.MaxMP - MP.AspectThunder) ? Transpose : Blizzard;
                        }
                    }

                    // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                    if (level < Levels.Fire4)
                    {
                        if (gauge.InAstralFire)
                        {
                            if (HasEffect(Buffs.Firestarter))
                            {
                                return Fire3;
                            }
                            return (currentMP < MP.AspectFire) ? Blizzard3 : Fire;
                        }
                        if (gauge.InUmbralIce)
                        {
                            if (level >= Levels.Blizzard4 && gauge.UmbralHearts < 3)
                            {
                                return Blizzard4;
                            }
                            return (currentMP >= MP.MaxMP || gauge.UmbralHearts == 3) ? Fire3 : Blizzard;
                        }
                    }

                    // Use polyglot stacks if we don't need it for a future weave
                    if (gauge.PolyglotStacks > 0 && gauge.ElementTimeRemaining >= astralFireRefresh && (gauge.InUmbralIce || (gauge.InAstralFire && gauge.UmbralHearts == 0)))
                    {
                        if (level >= Levels.Xenoglossy)
                        {
                            // Check leylines and triplecast cooldown
                            if (gauge.PolyglotStacks == 2 && GetCooldown(LeyLines).CooldownRemaining >= 20 && GetCooldown(Triplecast).ChargeCooldownRemaining >= 20 && !thunder3Recast(15))
                            {
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling))
                                {
                                    return Xenoglossy;
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) && GetRemainingCharges(Triplecast) == 0)
                                {
                                    return Xenoglossy;
                                }
                            }
                        }
                        else if (level >= Levels.Foul)
                        {
                            return Foul;
                        }
                    }

                    if (gauge.InAstralFire)
                    {
                        // Refresh AF
                        if (gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter))
                        {
                            return Fire3;
                        }
                        if (gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.AspectFire)
                        {
                            if (level >= Levels.Paradox)
                            {
                                return gauge.IsParadoxActive ? Paradox : Despair;
                            }
                            return Fire;
                        }

                        // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                        if (lastComboMove != Xenoglossy && gauge.PolyglotStacks > 0 && level >= Levels.Xenoglossy && gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            var pooledPolyglotStacks = IsEnabled(CustomComboPreset.BLM_Simple_XenoPooling) ? 1 : 0;
                            if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs) && level >= Levels.Amplifier && IsOffCooldown(Amplifier))
                            {
                                return Xenoglossy;
                            }
                            if (gauge.PolyglotStacks > pooledPolyglotStacks)
                            {
                                if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs_LeyLines))
                                {
                                    if (level >= Levels.LeyLines && IsOffCooldown(LeyLines))
                                    {
                                        return Xenoglossy;
                                    }
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_Buffs))
                                {
                                    if (level >= Levels.Triplecast && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) > 0 &&
                                        (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1))
                                    {
                                        return Xenoglossy;
                                    }
                                    if (level >= Levels.Manafont && IsOffCooldown(Manafont) && currentMP < MP.Despair)
                                    {
                                        return Xenoglossy;
                                    }
                                    if (level >= Levels.Sharpcast && GetRemainingCharges(Sharpcast) >= 1 && !HasEffect(Buffs.Sharpcast) &&
                                        thunder3Recast(15) && lastComboMove != Thunder3 && gauge.InAstralFire && !gauge.IsParadoxActive)
                                    {
                                        return Xenoglossy;
                                    }
                                }
                            }
                        }

                        // Cast Fire 4 after Manafont
                        if (IsOnCooldown(Manafont))
                        {
                            if ((level < Levels.EnhancedManafont && GetCooldownRemainingTime(Manafont) >= 179) ||
                                (level >= Levels.EnhancedManafont && GetCooldownRemainingTime(Manafont) >= 119))
                            {
                                return Fire4;
                            }
                        }

                        // Blizzard3/Despair when below Fire 4 + Despair MP
                        if (currentMP < (MP.AspectFire + MP.Despair))
                        {
                            return (level >= Levels.Despair && currentMP >= MP.Despair) ? Despair : Blizzard3;
                        }

                        return Fire4;
                    }

                    if (gauge.InUmbralIce)
                    {
                        // Use Paradox when available
                        if (level >= Levels.Paradox && gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }

                        // Fire3 when at max umbral hearts
                        return (gauge.UmbralHearts == 3 && currentMP >= MP.MaxMP - MP.AspectThunder) ? Fire3 : Blizzard4;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_Simple_Transpose : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Simple_Transpose;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Scathe)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<BLMGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = Service.Configuration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;
                    var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Only enable sharpcast if it's available
                    if (!inOpener && !HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && lastComboMove != Thunder3)
                    {
                        return Sharpcast;
                    }

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                    }

                    if (inCombat && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        // Exit out of opener if Enochian is lost
                        if (!gauge.IsEnochianActive)
                        {
                            openerFinished = true;
                            return Blizzard3;
                        }

                        if (gauge.InAstralFire)
                        {
                            // First Triplecast
                            if (lastComboMove != Triplecast && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) >= 1)
                            {
                                if (currentMP <= 6000)
                                {
                                    return Triplecast;
                                }
                            }

                            // Weave other oGCDs
                            if (canWeave)
                            {
                                // Manafont
                                if (IsOffCooldown(Manafont) && lastComboMove == Despair)
                                {
                                    if (currentMP < MP.Despair)
                                    {
                                        return Manafont;
                                    }
                                }

                                // Weave Amplifier and Ley Lines
                                if (currentMP <= 2800)
                                {
                                    if (IsOffCooldown(Amplifier))
                                    {
                                        return Amplifier;
                                    }
                                    if (IsOffCooldown(LeyLines))
                                    {
                                        return LeyLines;
                                    }
                                }

                                if (IsOnCooldown(LeyLines))
                                {
                                    // Swiftcast
                                    if (IsOffCooldown(All.Swiftcast))
                                    {
                                        return All.Swiftcast;
                                    }

                                    // Sharpcast
                                    if (!HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && IsOnCooldown(LeyLines))
                                    {
                                        return Sharpcast;
                                    }
                                }

                                // Second Triplecast
                                if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) &&
                                    lastComboMove != All.Swiftcast && GetRemainingCharges(Triplecast) >= 1 && currentMP < 6000)
                                {
                                    return Triplecast;
                                }

                                // Lucid Dreaming
                                if (GetRemainingCharges(Triplecast) == 0 && IsOffCooldown(All.LucidDreaming))
                                {
                                    return All.LucidDreaming;
                                }
                            }

                            // Cast Despair
                            if (currentMP < MP.AspectFire && currentMP >= MP.Despair)
                            {
                                return Despair;
                            }

                            // Cast Fire 4 after Manafont
                            if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                            {
                                return Fire4;
                            }

                            return currentMP >= MP.AspectFire ? Fire4 : Transpose;
                        }

                        if (gauge.InUmbralIce)
                        {
                            if (gauge.IsParadoxActive)
                            {
                                return Paradox;
                            }
                            if (gauge.PolyglotStacks >= 1 && lastComboMove != Xenoglossy)
                            {
                                return Xenoglossy;
                            }
                            if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                            {
                                return Thunder3;
                            }
                            openerFinished = true;
                        }
                    }

                    if (gauge.ElementTimeRemaining == 0 || !gauge.IsEnochianActive)
                    {
                        if (currentMP >= MP.Fire3)
                        {
                            return Fire3;
                        }
                        return Blizzard3;
                    }

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder
                        if (lastComboMove != Thunder3 && currentMP >= MP.AspectThunder &&
                            thunder3Recast(4) && !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                        {
                            return Thunder3;
                        }

                        // Buffs
                        if (canWeave)
                        {
                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (!HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) > 0 &&
                                (gauge.InAstralFire || gauge.UmbralHearts >= 1) && currentMP >= MP.AspectFire * 2)
                            {
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_Transpose_Pooling) || GetRemainingCharges(Triplecast) > 1)
                                {
                                    return Triplecast;
                                }
                            }

                            if (IsOffCooldown(Amplifier) && gauge.PolyglotStacks < 2)
                            {
                                return Amplifier;
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Simple_Transpose_LeyLines) && IsOffCooldown(LeyLines))
                            {
                                return LeyLines;
                            }

                            if (IsOffCooldown(Manafont) && gauge.InAstralFire && currentMP < MP.Despair)
                            {
                                return Manafont;
                            }

                            if (GetRemainingCharges(Sharpcast) > 0 && !HasEffect(Buffs.Sharpcast))
                            {
                                return Sharpcast;
                            }
                        }
                    }

                    if (gauge.InUmbralIce)
                    {
                        // Standard
                        if (gauge.UmbralIceStacks == 3)
                        {
                            if (gauge.PolyglotStacks == 2)
                            {
                                return Xenoglossy;
                            }
                            if (gauge.IsParadoxActive)
                            {
                                return Paradox;
                            }
                            if (gauge.UmbralHearts < 3)
                            {
                                return Blizzard4;
                            }
                            return Fire3;
                        }

                        // Transpose Instant F3
                        if (canWeave)
                        {
                            if (!HasEffect(Buffs.Firestarter) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Triplecast))
                            {
                                if (IsOffCooldown(All.Swiftcast))
                                {
                                    return All.Swiftcast;
                                }
                            }
                            if (IsOffCooldown(All.LucidDreaming))
                            {
                                return All.LucidDreaming;
                            }
                        }

                        // Paradox for Transpose Lines
                        if (gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }

                        // Filler GCDs
                        if (currentMP <= MP.MaxMP - MP.AspectFire)
                        {
                            if (lastComboMove != Xenoglossy && gauge.PolyglotStacks >= 1)
                            {
                                return Xenoglossy;
                            }
                            if (lastComboMove != Thunder3 && thunder3Recast(7))
                            {
                                return Thunder3;
                            }
                            if (gauge.PolyglotStacks >= 1)
                            {
                                return Xenoglossy;
                            }
                        }

                        if (IsOffCooldown(Transpose) && (canDelayedWeave || currentMP >= MP.MaxMP - MP.AspectFire))
                        {
                            return Transpose;
                        }
                        if (HasEffect(All.Buffs.Swiftcast))
                        {
                            return Fire3;
                        }
                        if (gauge.PolyglotStacks >= 1)
                        {
                            return Xenoglossy;
                        }
                        return Blizzard4;
                    }

                    if (gauge.InAstralFire)
                    {
                        // F3
                        if (gauge.AstralFireStacks < 3)
                        {
                            return Fire3;
                        }

                        // Xenoglossy for Manafont weave
                        if (gauge.PolyglotStacks >= 1 && IsOffCooldown(Manafont) && currentMP < MP.Despair)
                        {
                            return Xenoglossy;
                        }

                        // Early Despair
                        if (currentMP < (MP.AspectFire + MP.Despair) && currentMP >= MP.Despair)
                        {
                            return Despair;
                        }

                        // Cast Fire 4 after Manafont
                        if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                        {
                            return Fire4;
                        }

                        // Transpose if F3 is available, or Thundercloud + Xenoglossy is available
                        if (currentMP < MP.AspectFire && lastComboMove != Manafont && IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) <= 118)
                        {
                            if ((HasEffect(Buffs.LeyLines) && GetBuffRemainingTime(Buffs.LeyLines) >= 15) || HasEffect(Buffs.Firestarter) ||
                                 lastComboMove == Xenoglossy || lastComboMove == Thunder3 || (IsOffCooldown(All.Swiftcast) && (gauge.PolyglotStacks == 2)))
                            {
                                if (lastComboMove != Despair && lastComboMove != Fire4)
                                {
                                    return Transpose;
                                }
                                if (lastComboMove == Despair)
                                {
                                    if (gauge.PolyglotStacks >= 1)
                                    {
                                        return Xenoglossy;
                                    }
                                    if (HasEffect(Buffs.Thundercloud))
                                    {
                                        return Thunder3;
                                    }
                                }
                            }
                        }

                        // Regular Despair / Paradox
                        if (gauge.ElementTimeRemaining <= astralFireRefresh)
                        {
                            return !gauge.IsParadoxActive ? Despair : Paradox;
                        }
                        if (currentMP >= MP.AspectFire)
                        {
                            return Fire4;
                        }
                        return Blizzard3;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_Paradox : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Paradox;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Scathe)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var gauge = GetJobGauge<BLMGauge>();
                    var canWeave = CanSpellWeave(actionID);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var currentMP = LocalPlayer.CurrentMp;
                    var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Only enable sharpcast if it's available
                    if (!inOpener && !HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && lastComboMove != Thunder3)
                    {
                        return Sharpcast;
                    }

                    if (!inCombat && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                    }

                    if (inCombat && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (inCombat && inOpener && !openerFinished)
                    {
                        if (inCombat && inOpener && !openerFinished)
                        {
                            // Exit out of opener if Enochian is lost
                            if (!gauge.IsEnochianActive)
                            {
                                openerFinished = true;
                                return Blizzard3;
                            }

                            if (gauge.InAstralFire)
                            {
                                // First Triplecast
                                if (lastComboMove != Triplecast && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) >= 1)
                                {
                                    var triplecastMP = 7600;
                                    if (currentMP <= triplecastMP)
                                    {
                                        return Triplecast;
                                    }
                                }

                                // Weave other oGCDs
                                if (canWeave)
                                {
                                    // Weave Amplifier and Ley Lines
                                    if (currentMP <= 4400)
                                    {
                                        if (IsOffCooldown(Amplifier))
                                        {
                                            return Amplifier;
                                        }
                                        if (IsOffCooldown(LeyLines))
                                        {
                                            return LeyLines;
                                        }
                                    }

                                    // Swiftcast
                                    if (IsOffCooldown(All.Swiftcast) && IsOnCooldown(LeyLines))
                                    {
                                        return All.Swiftcast;
                                    }

                                    // Manafont
                                    if (IsOffCooldown(Manafont) && lastComboMove == Despair)
                                    {
                                        if (currentMP < MP.Despair)
                                        {
                                            return Manafont;
                                        }
                                    }

                                    // Second Triplecast / Sharpcast
                                    if (!IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate))
                                    {
                                        if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) &&
                                            lastComboMove != All.Swiftcast && GetRemainingCharges(Triplecast) >= 1 && currentMP < MP.AspectFire)
                                        {
                                            return Triplecast;
                                        }

                                        if (!HasEffect(Buffs.Sharpcast) && GetRemainingCharges(Sharpcast) >= 1 && IsOnCooldown(Manafont) &&
                                            lastComboMove == Fire4)
                                        {
                                            return Sharpcast;
                                        }
                                    }
                                }

                                // Cast Despair
                                if ((currentMP < MP.AspectFire || gauge.ElementTimeRemaining <= 4000) && currentMP >= MP.Despair)
                                {
                                    return Despair;
                                }

                                // Cast Fire 4 after Manafont
                                if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                                {
                                    return Fire4;
                                }

                                // Fire4 / Umbral Ice
                                return currentMP >= MP.AspectFire ? Fire4 : Blizzard3;
                            }

                            if (gauge.InUmbralIce)
                            {
                                // Dump Polyglot Stacks
                                if (gauge.PolyglotStacks >= 1 && gauge.ElementTimeRemaining >= 6000)
                                {
                                    return Xenoglossy;
                                }
                                if (gauge.IsParadoxActive && level >= Levels.Paradox)
                                {
                                    return Paradox;
                                }
                                if (gauge.UmbralHearts < 3 && lastComboMove != Blizzard4)
                                {
                                    return Blizzard4;
                                }

                                // Refresh Thunder3
                                if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                                {
                                    return Thunder3;
                                }

                                openerFinished = true;
                            }
                        }
                    }

                    if (gauge.ElementTimeRemaining == 0 || !gauge.IsEnochianActive)
                    {
                        if (currentMP >= MP.Fire3)
                        {
                            return Fire3;
                        }
                        return Blizzard3;
                    }

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder
                        if (lastComboMove != Thunder3 && currentMP >= MP.AspectThunder &&
                            thunder3Recast(4) && !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                        {
                            return Thunder3;
                        }

                        // Buffs
                        if (canWeave)
                        {
                            if (!HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) > 0)
                            {
                                return Triplecast;
                            }

                            if (IsOffCooldown(Amplifier) && gauge.PolyglotStacks < 2)
                            {
                                return Amplifier;
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Paradox_LeyLines) && IsOffCooldown(LeyLines))
                            {
                                return LeyLines;
                            }

                            if (IsOffCooldown(Manafont) && gauge.InAstralFire && currentMP < MP.Despair)
                            {
                                return Manafont;
                            }

                            if (IsOffCooldown(All.Swiftcast))
                            {
                                return All.Swiftcast;
                            }

                            if (GetRemainingCharges(Sharpcast) > 0 && !HasEffect(Buffs.Sharpcast))
                            {
                                return Sharpcast;
                            }
                        }
                    }

                    // Play standard while inside of leylines
                    if (HasEffect(Buffs.LeyLines))
                    {
                        if (gauge.InAstralFire)
                        {
                            if (gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter))
                            {
                                return Fire3;
                            }
                            if (gauge.ElementTimeRemaining <= 6000 && !HasEffect(Buffs.Firestarter) && currentMP >= MP.AspectFire)
                            {
                                return gauge.IsParadoxActive ? Paradox : Despair;
                            }
                            return (currentMP >= MP.AspectFire + MP.Despair) ? Fire4 : (currentMP >= MP.Despair ? Despair : Blizzard3);
                        }

                        if (gauge.InUmbralIce)
                        {
                            if (gauge.PolyglotStacks == 2)
                            {
                                return Xenoglossy;
                            }
                            return gauge.IsParadoxActive ? Paradox : (gauge.UmbralHearts == 3 ? Fire3 : Blizzard4);
                        }
                    }

                    if (gauge.InUmbralIce)
                    {
                        if (gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }
                        if (currentMP >= MP.Despair && (HasEffect(Buffs.Firestarter) || HasEffect(Buffs.Triplecast) || HasEffect(All.Buffs.Swiftcast)))
                        {
                            return Fire3;
                        }
                        if (gauge.UmbralIceStacks < 3)
                        {
                            return UmbralSoul;
                        }
                        if (IsOffCooldown(Transpose))
                        {
                            return Transpose;
                        }
                    }

                    if (gauge.InAstralFire)
                    {
                        if (gauge.AstralFireStacks < 3 && HasEffect(Buffs.Firestarter) && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                        {
                            return Fire3;
                        }

                        // Cast Despair after Manafont
                        if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                        {
                            return Despair;
                        }

                        if (HasEffect(Buffs.Triplecast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Sharpcast))
                        {
                            if (!HasEffect(Buffs.Firestarter) && currentMP >= MP.AspectFire)
                            {
                                if (gauge.IsParadoxActive)
                                {
                                    return Paradox;
                                }
                                if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                                {
                                    return Fire;
                                }
                            }
                            if (currentMP >= MP.Despair)
                            {
                                return Despair;
                            }
                        }
                        if (IsOffCooldown(Transpose) && openerFinished)
                        {
                            return Transpose;
                        }
                    }

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        if (gauge.PolyglotStacks >= 1)
                        {
                            return Xenoglossy;
                        }
                        if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                        {
                            return Thunder3;
                        }
                        return currentMP <= MP.Despair ? (gauge.InAstralFire ? Transpose : UmbralSoul) : Scathe;
                    }
                }

                return actionID;
            }
        }
    }
}
