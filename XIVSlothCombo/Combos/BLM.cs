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
            public const string BlmPolygotsStored = "BlmPolygotsStored";
            public const string BlmAstralFireRefresh = "BlmAstralFireRefresh";
        }
    }

    internal class BlackBlizzardFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackBlizzardFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Blizzard)
            {
                var gauge = GetJobGauge<BLMGauge>().InUmbralIce;
                if (level >= BLM.Levels.Freeze && !gauge)
                {
                    return BLM.Blizzard3;
                }
            }

            if (actionID == BLM.Freeze && level < BLM.Levels.Freeze)
            {
                return BLM.Blizzard2;
            }

            return actionID;
        }
    }

    internal class BlackFire13Feature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFire13Feature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Fire)
            {
                var gauge = GetJobGauge<BLMGauge>();
                if ((level >= BLM.Levels.Fire3 && !gauge.InAstralFire) || HasEffect(BLM.Buffs.Firestarter))
                {
                    return BLM.Fire3;
                }
            }

            return actionID;
        }
    }

    internal class BlackLeyLinesFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackLeyLinesFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.LeyLines && HasEffect(BLM.Buffs.LeyLines) && level >= BLM.Levels.BetweenTheLines)
            {
                return BLM.BetweenTheLines;
            }

            return actionID;
        }
    }

    internal class BlackManaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackManaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Transpose)
            {
                var gauge = GetJobGauge<BLMGauge>();
                if (gauge.InUmbralIce && level >= BLM.Levels.UmbralSoul)
                {
                    return BLM.UmbralSoul;
                }
            }

            return actionID;
        }
    }

    internal class BlackEnochianFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackEnochianFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Scathe)
            {
                var gauge = GetJobGauge<BLMGauge>();
                var GCD = GetCooldown(actionID);
                var thundercloudduration = FindEffectAny(BLM.Buffs.Thundercloud);
                var thunderdebuffontarget = FindTargetEffect(BLM.Debuffs.Thunder3);
                var thunderOneDebuff = FindTargetEffect(BLM.Debuffs.Thunder);
                var thunder3DebuffOnTarget = TargetHasEffect(BLM.Debuffs.Thunder3);

                if (gauge.InUmbralIce && level >= BLM.Levels.Blizzard4)
                {
                    if (gauge.ElementTimeRemaining >= 0 && IsEnabled(CustomComboPreset.BlackThunderFeature))
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if ((TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(BLM.Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                return BLM.Thunder3;
                        }

                        if (IsEnabled(CustomComboPreset.BlackThunderUptimeFeature) && !thunder3DebuffOnTarget && lastComboMove != BLM.Thunder3 && LocalPlayer.CurrentMp >= 400)
                            return BLM.Thunder3;

                        if (gauge.IsParadoxActive && level >= 90)
                            return BLM.Paradox;

                        if (IsEnabled(CustomComboPreset.BlackAspectSwapFeature) && gauge.UmbralHearts == 3 && LocalPlayer.CurrentMp >= 10000)
                            return BLM.Fire3;

                    }

                    return BLM.Blizzard4;
                }

                if (level >= BLM.Levels.Fire4)
                {
                    if (gauge.ElementTimeRemaining >= 6000 && IsEnabled(CustomComboPreset.BlackThunderFeature))
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if ((TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(BLM.Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                return BLM.Thunder3;
                        }

                        if (IsEnabled(CustomComboPreset.BlackThunderUptimeFeature) && !thunder3DebuffOnTarget && lastComboMove != BLM.Thunder3 && LocalPlayer.CurrentMp >= 400)
                            return BLM.Thunder3;
                    }

                    if (gauge.ElementTimeRemaining < 3000 && HasEffect(BLM.Buffs.Firestarter) && IsEnabled(CustomComboPreset.BlackFire13Feature))
                    {
                        return BLM.Fire3;
                    }

                    if (IsEnabled(CustomComboPreset.BlackAspectSwapFeature) && level >= BLM.Levels.Blizzard3)
                    {
                        if ((LocalPlayer.CurrentMp < 800) || (LocalPlayer.CurrentMp < 1600 && level < BLM.Levels.Despair))
                            return BLM.Blizzard3;
                    }

                    if (gauge.ElementTimeRemaining > 0 && LocalPlayer.CurrentMp < 2400 && level >= BLM.Levels.Despair && IsEnabled(CustomComboPreset.BlackDespairFeature))
                    {
                        return BLM.Despair;
                    }

                    if (gauge.IsEnochianActive)
                    {
                        if (gauge.ElementTimeRemaining < 6000 && !HasEffect(BLM.Buffs.Firestarter) && IsEnabled(CustomComboPreset.BlackFire13Feature) && level == 90 && gauge.IsParadoxActive)
                            return BLM.Paradox;
                        if (gauge.ElementTimeRemaining < 6000 && !HasEffect(BLM.Buffs.Firestarter) && IsEnabled(CustomComboPreset.BlackFire13Feature) && !gauge.IsParadoxActive)
                            return BLM.Fire;
                    }

                    return BLM.Fire4;
                }

                if (gauge.ElementTimeRemaining >= 5000 && IsEnabled(CustomComboPreset.BlackThunderFeature))
                {
                    if (level < BLM.Levels.Thunder3)
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if (TargetHasEffect(BLM.Debuffs.Thunder) && thunderOneDebuff.RemainingTime < 4)
                                return BLM.Thunder;
                        }
                    }
                    else
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if (TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4)
                                return BLM.Thunder3;
                        }
                    }

                    if (level < BLM.Levels.Thunder3)
                    {
                        if (IsEnabled(CustomComboPreset.BlackThunderUptimeFeature) && !TargetHasEffect(BLM.Debuffs.Thunder) && lastComboMove != BLM.Thunder && LocalPlayer.CurrentMp >= 200)
                            return BLM.Thunder;
                    }
                    else
                    {
                        if (IsEnabled(CustomComboPreset.BlackThunderUptimeFeature) && !TargetHasEffect(BLM.Debuffs.Thunder3) && lastComboMove != BLM.Thunder3 && LocalPlayer.CurrentMp >= 400)
                            return BLM.Thunder3;
                    }
                }

                if (level < BLM.Levels.Fire3)
                {
                    return BLM.Fire;
                }

                if (gauge.InAstralFire)
                {
                    if (HasEffect(BLM.Buffs.Firestarter) && level == 90)
                        return BLM.Paradox;
                    if (HasEffect(BLM.Buffs.Firestarter))
                        return BLM.Fire3;
                    if (IsEnabled(CustomComboPreset.BlackAspectSwapFeature) && LocalPlayer.CurrentMp < 1600 && level >= BLM.Levels.Blizzard3)
                        return BLM.Blizzard3;

                    return BLM.Fire;
                }

                if (gauge.InUmbralIce)
                {
                    if (IsEnabled(CustomComboPreset.BlackAspectSwapFeature) && LocalPlayer.CurrentMp >= 10000 && level >= BLM.Levels.Fire3)
                        return BLM.Fire3;

                    return BLM.Blizzard;
                }
            }

            return actionID;
        }
    }

    internal class BlackAoEComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackAoEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Flare)
            {
                var gauge = GetJobGauge<BLMGauge>();
                var thunder4Debuff = TargetHasEffect(BLM.Debuffs.Thunder4);
                var thunder4Timer = FindTargetEffect(BLM.Debuffs.Thunder4);
                var thunder2Debuff = TargetHasEffect(BLM.Debuffs.Thunder2);
                var thunder2Timer = FindTargetEffect(BLM.Debuffs.Thunder2);
                var currentMP = LocalPlayer.CurrentMp;
                var polyToStore = Service.Configuration.GetCustomIntValue(BLM.Config.BlmPolygotsStored);

                // Polygot usage
                if (IsEnabled(CustomComboPreset.BlackAoEFoulOption) && level >= BLM.Levels.Manafont && level >= BLM.Levels.Foul)
                {
                    if (gauge.InAstralFire && currentMP <= BLM.MP.AspectFire && IsOffCooldown(BLM.Manafont) && CanSpellWeave(actionID) && lastComboMove == BLM.Foul)
                    {
                        return BLM.Manafont;
                    }

                    if ((gauge.InAstralFire && currentMP <= BLM.MP.AspectFire && IsOffCooldown(BLM.Manafont) && gauge.PolyglotStacks >= 1) || (IsOnCooldown(BLM.Manafont) && (GetCooldownRemainingTime(BLM.Manafont) >= 30 && gauge.PolyglotStacks > polyToStore)))
                    {
                        return BLM.Foul;
                    }
                }

                // Thunder uptime
                if (currentMP >= BLM.MP.AspectThunder && lastComboMove != BLM.Manafont)
                {
                    if (level >= BLM.Levels.Thunder4)
                    {
                        if (lastComboMove != BLM.Thunder4 && (!thunder4Debuff || thunder4Timer.RemainingTime <= 4) && 
                           ((gauge.InUmbralIce && gauge.UmbralHearts == 3) || 
                            (gauge.InAstralFire && !HasEffect(BLM.Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))))
                        {
                            return BLM.Thunder4;
                        }
                    }
                    else if (level >= BLM.Levels.Thunder2)
                    {
                        if (lastComboMove != BLM.Thunder2 && (!thunder2Debuff || thunder2Timer.RemainingTime <= 4) &&
                           ((gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < BLM.Levels.Blizzard4)) || 
                            (gauge.InAstralFire && !HasEffect(BLM.Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))))
                        {
                            return BLM.Thunder2;
                        }
                    }
                }

                // Fire 2 / Flare
                if (gauge.InAstralFire)
                {
                    if (currentMP >= 7000)
                    {
                        return level >= BLM.Levels.HighFire2 ? BLM.HighFire2 : BLM.Fire2;
                    }
                    else if (currentMP >= BLM.MP.Despair)
                    {
                        return BLM.Flare;
                    }
                }

                // Umbral Hearts
                if (gauge.InUmbralIce)
                {
                    if (level >= BLM.Levels.Blizzard4 && gauge.UmbralHearts <= 2)
                    {
                        return BLM.Freeze;
                    }
                    else if (level >= BLM.Levels.Freeze && currentMP < BLM.MP.MaxMP - BLM.MP.AspectThunder)
                    {
                        return BLM.Freeze;
                    }
                    if (level < BLM.Levels.Fire3)
                    {
                        return (currentMP >= BLM.MP.MaxMP - BLM.MP.AspectThunder) ? BLM.Transpose : BLM.Blizzard2;
                    }
                    return level >= BLM.Levels.HighFire2 ? BLM.HighFire2 : BLM.Fire2;
                }

                return level >= BLM.Levels.HighBlizzard2 ? BLM.HighBlizzard2 : BLM.Blizzard2;
            }

            return actionID;
        }
    }
    
    internal class BlackSimpleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackSimpleFeature;
        
        internal static bool inOpener = false;
        internal static bool openerFinished = false;

        internal delegate bool DotRecast(int value); 

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Scathe)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<BLMGauge>();
                var canWeave = CanSpellWeave(actionID);
                var currentMP = LocalPlayer.CurrentMp;
                var astralFireRefresh = Service.Configuration.GetCustomFloatValue(BLM.Config.BlmAstralFireRefresh) * 1000;

                // Opener for BLM
                // Credit to damolitionn for providing code to be used as a base for this opener
                if (IsEnabled(CustomComboPreset.BlackSimpleOpenerFeature) && level >= BLM.Levels.Foul)
                {
                    // Only enable sharpcast if it's available
                    if ((!inOpener || (openerFinished && CanSpellWeave(actionID))) && !HasEffect(BLM.Buffs.Sharpcast) && (GetRemainingCharges(BLM.Sharpcast) >= 1))
                    {
                        return BLM.Sharpcast;
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
                            return BLM.Blizzard3;
                        }

                        if (gauge.InAstralFire)
                        {
                            // First Triplecast
                            if (lastComboMove != BLM.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) == 2)
                            {
                                var triplecastMP = 7600;
                                if (IsEnabled(CustomComboPreset.BlackSimpleAltOpenerFeature))
                                {
                                    triplecastMP = 6000;
                                }
                                if (currentMP <= triplecastMP)
                                {
                                    return BLM.Triplecast;
                                }
                            }

                            // Weave other oGCDs
                            if (canWeave)
                            {
                                // Weave Amplifier and Ley Lines
                                if (currentMP <= 4400)
                                {
                                    if (level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier))
                                    {
                                        return BLM.Amplifier;
                                    }
                                    if (level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                                    {
                                        return BLM.LeyLines;
                                    }
                                }

                                // Swiftcast
                                if (IsOffCooldown(All.Swiftcast) && IsOnCooldown(BLM.LeyLines))
                                {
                                    return All.Swiftcast;
                                }

                                // Manafont
                                if (IsOffCooldown(BLM.Manafont) && (lastComboMove == BLM.Despair || lastComboMove == BLM.Fire))
                                {
                                    if (level >= BLM.Levels.Despair && currentMP < BLM.MP.Despair)
                                    {
                                        return BLM.Manafont;
                                    }
                                    else if (currentMP < BLM.MP.AspectFire)
                                    {
                                        return BLM.Manafont;
                                    }
                                }

                                // Second Triplecast / Sharpcast
                                if (!IsEnabled(CustomComboPreset.BlackSimpleAltOpenerFeature))
                                {
                                    if (!HasEffect(BLM.Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) && 
                                        lastComboMove != All.Swiftcast && GetRemainingCharges(BLM.Triplecast) >= 1 && currentMP < BLM.MP.AspectFire)
                                    {
                                        return BLM.Triplecast;
                                    }

                                    if (!HasEffect(BLM.Buffs.Sharpcast) && GetRemainingCharges(BLM.Sharpcast) >= 1 && IsOnCooldown(BLM.Manafont) && 
                                        lastComboMove == BLM.Fire4)
                                    {
                                        return BLM.Sharpcast;
                                    }
                                }
                            }

                            // Cast Despair
                            if (level >= BLM.Levels.Despair && (currentMP < BLM.MP.AspectFire || gauge.ElementTimeRemaining <= 4000) && currentMP >= BLM.MP.Despair)
                            {
                                return BLM.Despair;
                            }

                            // Cast Fire
                            if (level < BLM.Levels.Despair && gauge.ElementTimeRemaining <= 6000 && currentMP >= BLM.MP.AspectFire)
                            {
                                return BLM.Fire;
                            }

                            // Fire4 / Umbral Ice
                            return (currentMP >= BLM.MP.AspectFire || lastComboMove == BLM.Manafont) ? BLM.Fire4 : BLM.Blizzard3;
                        }
                        
                        if (gauge.InUmbralIce)
                        {
                            // Dump Polygot Stacks
                            if (gauge.PolyglotStacks >= 1 && gauge.ElementTimeRemaining >= 6000)
                            {
                                return level >= BLM.Levels.Xenoglossy ? BLM.Xenoglossy : BLM.Foul;
                            }
                            if (gauge.IsParadoxActive && level >= BLM.Levels.Paradox)
                            {
                                return BLM.Paradox;
                            }
                            if (gauge.UmbralHearts < 3 && lastComboMove != BLM.Blizzard4)
                            {
                                return BLM.Blizzard4;
                            }

                            // Refresh Thunder3
                            if (HasEffect(BLM.Buffs.Thundercloud) && lastComboMove != BLM.Thunder3)
                            {
                                return BLM.Thunder3;
                            }

                            openerFinished = true;
                        }
                    }
                }

                // Handle thunder uptime and buffs
                if (gauge.ElementTimeRemaining > 0)
                {
                    var thunder = TargetHasEffect(BLM.Debuffs.Thunder);
                    var thunder3 = TargetHasEffect(BLM.Debuffs.Thunder3);
                    var thunderDuration = FindTargetEffect(BLM.Debuffs.Thunder);
                    var thunder3Duration = FindTargetEffect(BLM.Debuffs.Thunder3);

                    DotRecast thunderRecast = delegate (int duration)
                    {
                        return !thunder || (thunder && thunderDuration.RemainingTime < duration);
                    };
                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Thunder uptime
                    if (gauge.ElementTimeRemaining >= 6000 && currentMP >= BLM.MP.AspectThunder)
                    {
                        if (level < BLM.Levels.Thunder3 && lastComboMove != BLM.Thunder && thunderRecast(4) && !TargetHasEffect(BLM.Debuffs.Thunder2))
                        {
                            return BLM.Thunder;
                        }
                        else if (lastComboMove != BLM.Thunder3 && thunder3Recast(4) && !TargetHasEffect(BLM.Debuffs.Thunder2) && !TargetHasEffect(BLM.Debuffs.Thunder4))
                        {
                            return BLM.Thunder3;
                        }
                    }

                    // Buffs
                    if (canWeave)
                    {
                        if (IsEnabled(CustomComboPreset.BlackSimpleCastsFeature))
                        {
                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (level >= BLM.Levels.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) > 0 && 
                                (gauge.InAstralFire || gauge.UmbralHearts == 3) && currentMP >= BLM.MP.AspectFire * 2)
                            {
                                if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) || GetRemainingCharges(BLM.Triplecast) > 1)
                                {
                                    return BLM.Triplecast;
                                }
                            }

                            // Use Swiftcast in Astral Fire
                            if (level >= All.Levels.Swiftcast && IsOffCooldown(All.Swiftcast) && gauge.InAstralFire)
                            {
                                if (level >= BLM.Levels.Despair && currentMP >= BLM.MP.Despair)
                                {
                                    return All.Swiftcast;
                                }
                                else if (currentMP >= BLM.MP.AspectFire)
                                {
                                    return All.Swiftcast;
                                }
                            }
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature))
                        {
                            if (level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier) && gauge.PolyglotStacks < 2)
                            {
                                return BLM.Amplifier;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsLeylinesFeature))
                        {
                            if (level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                            {
                                return BLM.LeyLines;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature))
                        {
                            if (IsOffCooldown(BLM.Manafont) && gauge.InAstralFire)
                            {
                                if (level >= BLM.Levels.Despair && currentMP < BLM.MP.Despair)
                                {
                                    return BLM.Manafont;
                                }
                                else if (currentMP < BLM.MP.AspectFire)
                                {
                                    return BLM.Manafont;
                                }
                            }
                            if (level >= BLM.Levels.Sharpcast && GetRemainingCharges(BLM.Sharpcast) > 0 && !HasEffect(BLM.Buffs.Sharpcast))
                            {
                                return BLM.Sharpcast;
                            }
                        }
                    }
                }

                // Handle initial cast
                if ((level >= BLM.Levels.Blizzard4 && !gauge.IsEnochianActive) || gauge.ElementTimeRemaining <= 0)
                {
                    if (level >= BLM.Levels.Fire3)
                    {
                        return (currentMP >= BLM.MP.Fire3) ? BLM.Fire3 : BLM.Blizzard3;
                    }
                    return (currentMP >= BLM.MP.Fire) ? BLM.Fire : BLM.Blizzard;
                }

                // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                if (level < BLM.Levels.Blizzard3)
                {
                    if (gauge.InAstralFire)
                    {
                        return (currentMP < BLM.MP.AspectFire) ? BLM.Transpose : BLM.Fire;
                    }
                    if (gauge.InUmbralIce)
                    {
                        return (currentMP >= BLM.MP.MaxMP - BLM.MP.AspectThunder) ? BLM.Transpose : BLM.Blizzard;
                    }
                }

                // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                if (level < BLM.Levels.Fire4)
                {
                    if (gauge.InAstralFire)
                    {
                        if (HasEffect(BLM.Buffs.Firestarter))
                        {
                            return BLM.Fire3;
                        }
                        return (currentMP < BLM.MP.AspectFire) ? BLM.Blizzard3 : BLM.Fire;
                    }
                    if (gauge.InUmbralIce)
                    {
                        if (level >= BLM.Levels.Blizzard4 && gauge.UmbralHearts < 3)
                        {
                            return BLM.Blizzard4;
                        }
                        return (currentMP >= BLM.MP.MaxMP || gauge.UmbralHearts == 3) ? BLM.Fire3 : BLM.Blizzard;
                    }
                }

                // Use polygot stacks if we don't need it for a future weave
                if (gauge.PolyglotStacks > 0 && gauge.ElementTimeRemaining >= 5000 &&  (gauge.InUmbralIce || (gauge.InAstralFire && gauge.UmbralHearts == 0)))
                {
                    if (level >= BLM.Levels.Xenoglossy)
                    {
                        // Check leylines and triplecast cooldown
                        if (gauge.PolyglotStacks == 2 && GetCooldown(BLM.LeyLines).CooldownRemaining >= 30 && GetCooldown(BLM.Triplecast).ChargeCooldownRemaining >= 30)
                        {
                            if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature))
                            {
                                return BLM.Xenoglossy;
                            }
                            if (IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) && GetRemainingCharges(BLM.Triplecast) == 0)
                            {
                                return BLM.Xenoglossy;
                            }
                        }
                    }
                    else if (level >= BLM.Levels.Foul)
                    {
                        return BLM.Foul;
                    }
                }

                if (gauge.InAstralFire)
                {
                    // Refresh AF
                    if (gauge.ElementTimeRemaining <= 3000 && HasEffect(BLM.Buffs.Firestarter))
                    {
                        return BLM.Fire3;
                    }
                    if (gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(BLM.Buffs.Firestarter) && currentMP >= BLM.MP.AspectFire)
                    {
                        return (level >= BLM.Levels.Paradox && gauge.IsParadoxActive) ? BLM.Paradox : (level >= BLM.Levels.Despair ? BLM.Despair : BLM.Fire);
                    }

                    // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                    if (lastComboMove != BLM.Xenoglossy && gauge.PolyglotStacks > 0 && level >= BLM.Levels.Xenoglossy)
                    {
                        var pooledPolygotStacks = IsEnabled(CustomComboPreset.BlackSimplePoolingFeature) ? 1 : 0;
                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature) && level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier))
                        {
                            return BLM.Xenoglossy;
                        }
                        if (gauge.PolyglotStacks > pooledPolygotStacks)
                        {
                            if (IsEnabled(CustomComboPreset.BlackSimpleBuffsLeylinesFeature))
                            {
                                if (level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                                {
                                    return BLM.Xenoglossy;
                                }
                            }
                            if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature))
                            {
                                if (level >= BLM.Levels.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) > 0 &&
                                    (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) || GetRemainingCharges(BLM.Triplecast) > 1))
                                {
                                    return BLM.Xenoglossy;
                                }
                                if (level >= BLM.Levels.Manafont && IsOffCooldown(BLM.Manafont) && currentMP < BLM.MP.Despair)
                                {
                                    return BLM.Xenoglossy;
                                }
                            }
                        }
                    }

                    // Blizzard3/Despair when below Fire 4 + Despair MP
                    if (currentMP < (BLM.MP.AspectFire + BLM.MP.Despair))
                    {
                        return (level >= BLM.Levels.Despair && currentMP >= BLM.MP.Despair) ? BLM.Despair : BLM.Blizzard3;
                    }

                    return BLM.Fire4;
                }

                if (gauge.InUmbralIce)
                {
                    // Use Paradox when available
                    if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive)
                    {
                        return BLM.Paradox;
                    }

                    // Fire3 when at max umbral hearts
                    return gauge.UmbralHearts == 3 ? BLM.Fire3 : BLM.Blizzard4;
                }
            }

            return actionID;
        }
    }
    
    internal class BlackSimpleTranposeFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackSimpleTransposeFeature;

        internal static bool inOpener = false;
        internal static bool openerFinished = false;

        internal delegate bool DotRecast(int value);

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Scathe)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<BLMGauge>();
                var canWeave = CanSpellWeave(actionID);
                var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                var currentMP = LocalPlayer.CurrentMp;
                var astralFireRefresh = Service.Configuration.GetCustomFloatValue(BLM.Config.BlmAstralFireRefresh) * 1000;
                var thunder3 = TargetHasEffect(BLM.Debuffs.Thunder3);
                var thunder3Duration = FindTargetEffect(BLM.Debuffs.Thunder3);

                DotRecast thunder3Recast = delegate (int duration)
                {
                    return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                };

                // Only enable sharpcast if it's available
                if ((!inOpener || (openerFinished && canWeave)) && !HasEffect(BLM.Buffs.Sharpcast) && (GetRemainingCharges(BLM.Sharpcast) >= 1))
                {
                    return BLM.Sharpcast;
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
                        return BLM.Blizzard3;
                    }

                    if (gauge.InAstralFire)
                    {
                        // First Triplecast
                        if (lastComboMove != BLM.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) == 2)
                        {
                            if (currentMP <= 6000)
                            {
                                return BLM.Triplecast;
                            }
                        }

                        // Weave other oGCDs
                        if (canWeave)
                        {
                            // Weave Amplifier and Ley Lines
                            if (currentMP <= 2800)
                            {
                                if (level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier))
                                {
                                    return BLM.Amplifier;
                                }
                                if (level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                                {
                                    return BLM.LeyLines;
                                }
                            }

                            if (IsOnCooldown(BLM.LeyLines))
                            {
                                // Swiftcast
                                if (IsOffCooldown(All.Swiftcast))
                                {
                                    return All.Swiftcast;
                                }

                                // Sharpcast
                                if (!HasEffect(BLM.Buffs.Sharpcast) && GetRemainingCharges(BLM.Sharpcast) >= 1 && IsOnCooldown(BLM.LeyLines))
                                {
                                    return BLM.Sharpcast;
                                }
                            }


                            // Manafont
                            if (IsOffCooldown(BLM.Manafont) && lastComboMove == BLM.Despair)
                            {
                                if (level >= BLM.Levels.Despair && currentMP < BLM.MP.Despair)
                                {
                                    return BLM.Manafont;
                                }
                                else if (currentMP < BLM.MP.AspectFire)
                                {
                                    return BLM.Manafont;
                                }
                            }

                            // Second Triplecast
                            if (!HasEffect(BLM.Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) &&
                                lastComboMove != All.Swiftcast && GetRemainingCharges(BLM.Triplecast) >= 1 && currentMP < 6000)
                            {
                                return BLM.Triplecast;
                            }

                            // Lucid Dreaming
                            if (GetRemainingCharges(BLM.Triplecast) == 0 && IsOffCooldown(All.LucidDreaming))
                            {
                                return All.LucidDreaming;
                            }
                        }

                        // Cast Despair
                        if (currentMP < BLM.MP.AspectFire && currentMP >= BLM.MP.Despair)
                        {
                            return BLM.Despair;
                        }
                        if (currentMP >= BLM.MP.AspectFire)
                        {
                            return BLM.Fire4;
                        }
                        return BLM.Transpose;
                    }

                    if (gauge.InUmbralIce)
                    {
                        if (gauge.IsParadoxActive)
                        {
                            return BLM.Paradox;
                        }
                        if (gauge.PolyglotStacks >= 1 && lastComboMove != BLM.Xenoglossy)
                        {
                            return BLM.Xenoglossy;
                        }
                        if (HasEffect(BLM.Buffs.Thundercloud) && lastComboMove != BLM.Thunder3)
                        {
                            return BLM.Thunder3;
                        }
                        openerFinished = true;
                    }
                }
                
                if (gauge.ElementTimeRemaining == 0 || !gauge.IsEnochianActive)
                {
                    if (currentMP >= BLM.MP.Fire3)
                    {
                        return BLM.Fire3;
                    }
                    return BLM.Blizzard3;
                }

                if (gauge.ElementTimeRemaining > 0)
                {
                    // Thunder
                    if (lastComboMove != BLM.Thunder3 && currentMP >= BLM.MP.AspectThunder && 
                        thunder3Recast(4) && !TargetHasEffect(BLM.Debuffs.Thunder2) && !TargetHasEffect(BLM.Debuffs.Thunder4))
                    {
                        return BLM.Thunder3;
                    }

                    // Buffs
                    if (canWeave)
                    {
                        // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                        if (!HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) > 0 &&
                            (gauge.InAstralFire || gauge.UmbralHearts >= 1) && currentMP >= BLM.MP.AspectFire * 2)
                        {
                            if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) || GetRemainingCharges(BLM.Triplecast) > 1)
                            {
                                return BLM.Triplecast;
                            }
                        }

                        if (IsOffCooldown(BLM.Amplifier) && gauge.PolyglotStacks < 2)
                        {
                            return BLM.Amplifier;
                        }

                        if (IsOffCooldown(BLM.LeyLines))
                        {
                            return BLM.LeyLines;
                        }

                        if (IsOffCooldown(BLM.Manafont) && gauge.InAstralFire && currentMP < BLM.MP.Despair)
                        {
                            return BLM.Manafont;
                        }

                        if (GetRemainingCharges(BLM.Sharpcast) > 0 && !HasEffect(BLM.Buffs.Sharpcast))
                        {
                            return BLM.Sharpcast;
                        }
                    }
                }

                if (gauge.InUmbralIce)
                {
                    // Standard
                    if (gauge.UmbralIceStacks == 3)
                    {
                        if (gauge.IsParadoxActive)
                        {
                            return BLM.Paradox;
                        }
                        if (gauge.UmbralHearts < 3)
                        {
                            return BLM.Blizzard4;
                        }
                        return BLM.Fire3;
                    }

                    // Transpose Instant F3
                    if (canWeave)
                    {
                        if (!HasEffect(BLM.Buffs.Firestarter) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(BLM.Buffs.Triplecast))
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
                        return BLM.Paradox;
                    }

                    // Filler GCDs
                    if (currentMP <= BLM.MP.MaxMP - BLM.MP.AspectFire)
                    {
                        if (lastComboMove != BLM.Xenoglossy && gauge.PolyglotStacks >= 1)
                        {
                            return BLM.Xenoglossy;
                        }
                        if (lastComboMove != BLM.Thunder3 && thunder3Recast(7))
                        {
                            return BLM.Thunder3;
                        }
                        if (gauge.PolyglotStacks >= 1)
                        {
                            return BLM.Xenoglossy;
                        }
                    }

                    if (IsOffCooldown(BLM.Transpose) && (canDelayedWeave || currentMP >= BLM.MP.MaxMP - BLM.MP.AspectFire))
                    {
                        return BLM.Transpose;
                    }
                    if (HasEffect(All.Buffs.Swiftcast))
                    {
                        return BLM.Fire3;
                    }
                    if (gauge.PolyglotStacks >= 1)
                    {
                        return BLM.Xenoglossy;
                    }
                    return BLM.Blizzard4;
                }

                if (gauge.InAstralFire)
                {
                    // F3
                    if (gauge.AstralFireStacks < 3)
                    {
                        return BLM.Fire3;
                    }

                    // Xenoglossy for Manafont weave
                    if (gauge.PolyglotStacks >= 1 && IsOffCooldown(BLM.Manafont) && currentMP < BLM.MP.Despair)
                    {
                        return BLM.Xenoglossy;
                    }

                    // Early Despair
                    if (currentMP < (BLM.MP.AspectFire + BLM.MP.Despair) && currentMP >= BLM.MP.Despair)
                    {
                        return BLM.Despair;
                    }

                    // Transpose if F3 is available, or Thundercloud + Xenoglossy is available
                    if (currentMP < BLM.MP.AspectFire && lastComboMove != BLM.Manafont && IsOnCooldown(BLM.Manafont) && GetCooldownRemainingTime(BLM.Manafont) <= 118)
                    {
                        if ((HasEffect(BLM.Buffs.LeyLines) && FindEffect(BLM.Buffs.LeyLines).RemainingTime >= 15) || HasEffect(BLM.Buffs.Firestarter) ||
                             lastComboMove == BLM.Xenoglossy || lastComboMove == BLM.Thunder3 || (IsOffCooldown(All.Swiftcast) && (gauge.PolyglotStacks == 2)))
                        {
                            if (lastComboMove != BLM.Despair && lastComboMove != BLM.Fire4)
                            {
                                return BLM.Transpose;
                            }
                            if (lastComboMove == BLM.Despair)
                            {
                                if (gauge.PolyglotStacks >= 1)
                                {
                                    return BLM.Xenoglossy;
                                }
                                if (HasEffect(BLM.Buffs.Thundercloud))
                                {
                                    return BLM.Thunder3;
                                }
                            }
                        }
                    }

                    // Regular Despair / Paradox
                    if (gauge.ElementTimeRemaining <= astralFireRefresh)
                    {
                        return !gauge.IsParadoxActive ? BLM.Despair : BLM.Paradox;
                    }
                    if (currentMP >= BLM.MP.AspectFire)
                    {
                        return BLM.Fire4;
                    }
                    return BLM.Blizzard3;
                }
            }

            return actionID;
        }
    }
}
