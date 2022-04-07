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
            HighFireII = 25794,
            HighBlizzardII = 25795,
            Xenoglossy = 16507,
            Foul = 7422,
            Sharpcast = 3574,
            Manafont = 158,
            Swiftcast = 7561,
            Triplecast = 7421;

        public static class Buffs
        {
            public const ushort
                Thundercloud = 164,
                LeyLines = 737,
                Firestarter = 165,
                Sharpcast = 867,
                Swiftcast = 1987,
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
                Swiftcast = 18,
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
                Triplecast = 66,
                Foul = 70,
                Despair = 72,
                UmbralSoul = 76,
                Xenoglossy = 80,
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
    }

    internal class BlackBlizzardFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackBlizzardFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Blizzard)
            {
                var gauge = GetJobGauge<BLMGauge>().InUmbralIce;
                if (level >= 40 && !gauge)
                {
                    return BLM.Blizzard3;
                }
            }

            if (actionID == BLM.Freeze && level < 40)
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
                var gauge = CustomCombo.GetJobGauge<BLMGauge>();
                if ((level >= 34 && !gauge.InAstralFire) || CustomCombo.HasEffect(BLM.Buffs.Firestarter))
                {
                    return CustomCombo.OriginalHook(BLM.Fire3);
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
            if (actionID == 3573 && CustomCombo.HasEffect(737) && level >= 62)
            {
                return 7419u;
            }

            return actionID;
        }
    }

    internal class BlackManaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackManaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == 149)
            {
                var gauge = CustomCombo.GetJobGauge<BLMGauge>();
                if (gauge.InUmbralIce && level >= 76)
                {
                    return 16506u;
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
                        {
                            return BLM.Blizzard3;
                        }
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
                    {
                        return BLM.Blizzard3;
                    }

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

                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature))
                {
                    if ((!gauge.InUmbralIce && !gauge.InAstralFire) || (gauge.InAstralFire && currentMP <= 100))
                    {
                        if (level <= 81)
                            return BLM.Blizzard2;
                        if (level >= 82)
                            return BLM.HighBlizzardII;
                    }
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature))
                {
                    if (gauge.InUmbralIce && gauge.UmbralHearts <= 2)
                    {
                        if (level >= BLM.Levels.Blizzard4)
                        {
                            return BLM.Freeze;
                        }
                        else if (level >= 40 && currentMP < 10000 && thunder2Debuff)
                        {
                            return BLM.Freeze;
                        }
                    }
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 26 && level <= 63)
                {
                    if ((gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < BLM.Levels.Blizzard4) && !thunder2Debuff) ||
                        (gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < BLM.Levels.Blizzard4) && thunder2Timer.RemainingTime <= 3 && level >= 26 && level <= 63) ||
                        (gauge.InAstralFire && !thunder2Debuff && level >= 26 && level <= 63))
                    {
                        if (lastComboMove == BLM.Thunder2)
                        {
                        }
                        else
                        {
                            return BLM.Thunder2;
                        }
                    }
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 64)
                {
                    if ((gauge.InUmbralIce && gauge.UmbralHearts == 3 && !thunder4Debuff) || (gauge.InUmbralIce && gauge.UmbralHearts == 3 && thunder4Timer.RemainingTime <= 3) || (gauge.InAstralFire && !thunder4Debuff))
                    {
                        if (lastComboMove == BLM.Thunder4)
                        {
                        }
                        else
                        {
                            return BLM.Thunder4;
                        }
                    }
                }
                // low level
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 26 && level <= 63)
                {
                    if ((gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < BLM.Levels.Blizzard4) && thunder2Debuff && thunder2Timer.RemainingTime >= 3) ||
                        (gauge.InUmbralIce && (gauge.UmbralHearts == 3 || level < BLM.Levels.Blizzard4) && lastComboMove == BLM.Thunder2))
                    {
                        if (level <= 81)
                            return BLM.Fire2;
                    }
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 26 && level <= 63)
                {
                    if ((gauge.InAstralFire && LocalPlayer.CurrentMp > 7000 && thunder2Debuff) || (gauge.InAstralFire && LocalPlayer.CurrentMp > 7000 && lastComboMove == BLM.Thunder2))
                    {
                        if (level <= 81)
                            return BLM.Fire2;
                    }
                }
                // highlevel
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature))
                {
                    if ((gauge.InUmbralIce && gauge.UmbralHearts == 3 && thunder4Debuff && thunder4Timer.RemainingTime >= 3) || (gauge.InUmbralIce && gauge.UmbralHearts == 3 && lastComboMove == BLM.Thunder4))
                    {
                        if (level <= 81)
                            return BLM.Fire2;
                        if (level >= 82)
                            return BLM.HighFireII;
                    }
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature))
                {
                    if ((gauge.InAstralFire && LocalPlayer.CurrentMp > 7000 && thunder4Debuff) || (gauge.InAstralFire && LocalPlayer.CurrentMp > 7000 && lastComboMove == BLM.Thunder4))
                    {
                        if (level <= 81)
                            return BLM.Fire2;
                        if (level >= 82)
                            return BLM.HighFireII;
                    }
                }
                // lowlevel
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 50 && level <= 63)
                {
                    if ((gauge.InAstralFire && LocalPlayer.CurrentMp <= 7000 && thunder2Debuff) || (gauge.InAstralFire && LocalPlayer.CurrentMp <= 7000 && lastComboMove == BLM.Thunder2))
                        return BLM.Flare;
                }
                // highlevel
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 64)
                {
                    if ((gauge.InAstralFire && LocalPlayer.CurrentMp <= 7000 && thunder4Debuff) || (gauge.InAstralFire && LocalPlayer.CurrentMp <= 7000 && lastComboMove == BLM.Thunder4))
                        return BLM.Flare;
                }
                if (level <= 81)
                    return BLM.Blizzard2;
                if (level >= 82)
                    return BLM.HighBlizzardII;
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
                var canWeave = CanWeave(actionID, 0.6);
                var currentMP = LocalPlayer.CurrentMp;

                // Opener for BLM
                // Credit to damolitionn for providing code to be used as a base for this opener
                if (IsEnabled(CustomComboPreset.BlackSimpleOpenerFeature) && level >= BLM.Levels.Foul)
                {
                    // Only enable sharpcast if it's available
                    if (!inOpener && !HasEffect(BLM.Buffs.Sharpcast) && (GetRemainingCharges(BLM.Sharpcast) >= 1))
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
                            if (lastComboMove != BLM.Triplecast && !HasEffect(BLM.Buffs.Triplecast) &&
                                GetRemainingCharges(BLM.Triplecast) == 2)
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
                                if (IsOffCooldown(BLM.Swiftcast) && IsOnCooldown(BLM.LeyLines))
                                {
                                    return BLM.Swiftcast;
                                }

                                // Second Triplecast
                                if (!HasEffect(BLM.Buffs.Triplecast) && !HasEffect(BLM.Buffs.Swiftcast) && 
                                    IsOnCooldown(BLM.Swiftcast) && lastComboMove != BLM.Swiftcast &&
                                    GetRemainingCharges(BLM.Triplecast) >= 1 && currentMP < BLM.MP.AspectFire)
                                {
                                    if (!IsEnabled(CustomComboPreset.BlackSimpleAltOpenerFeature))
                                    {
                                        return BLM.Triplecast;
                                    }
                                }

                                // Manafont
                                if (IsOffCooldown(BLM.Manafont))
                                {
                                    if (level >= BLM.Levels.Despair)
                                    {
                                        if (currentMP < BLM.MP.Despair)
                                        {
                                            return BLM.Manafont;
                                        }
                                    }
                                    else
                                    {
                                        if (currentMP < BLM.MP.AspectFire)
                                        {
                                            return BLM.Manafont;
                                        }
                                    }
                                }

                                // Sharpcast
                                if (!IsEnabled(CustomComboPreset.BlackSimpleAltOpenerFeature))
                                {
                                    if (!HasEffect(BLM.Buffs.Sharpcast) && GetRemainingCharges(BLM.Sharpcast) >= 1 &&
                                        IsOnCooldown(BLM.Manafont) && lastComboMove == BLM.Fire4)
                                    {
                                        return BLM.Sharpcast;
                                    }
                                }
                            }

                            // Cast Despair
                            if (level >= BLM.Levels.Despair &&
                                currentMP < BLM.MP.AspectFire && currentMP >= BLM.MP.Despair)
                            {
                                return BLM.Despair;
                            }

                            // Cast Fire
                            if (level < BLM.Levels.Despair &&
                                gauge.ElementTimeRemaining <= 4000 &&
                                currentMP >= BLM.MP.AspectFire)
                            {
                                return BLM.Fire;
                            }

                            // Go to Umbral Ice
                            if (currentMP < BLM.MP.Despair && lastComboMove != BLM.Manafont && IsOnCooldown(BLM.Manafont))
                            {
                                return BLM.Blizzard3;
                            }

                            return BLM.Fire4;
                        }
                        
                        if (gauge.InUmbralIce)
                        {
                            // Dump Polygot Stacks
                            if (gauge.PolyglotStacks >= 1 && gauge.ElementTimeRemaining >= 6000)
                            {
                                if (level >= BLM.Levels.Xenoglossy)
                                {
                                    return BLM.Xenoglossy;
                                }
                                return BLM.Foul;
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
                    if (gauge.ElementTimeRemaining >= 6000)
                    {
                        if (level < BLM.Levels.Thunder3)
                        {
                            if (lastComboMove != BLM.Thunder && thunderRecast(4) && currentMP >= BLM.MP.AspectThunder)
                            {
                                return BLM.Thunder;
                            }
                        }
                        else
                        {
                            if (lastComboMove != BLM.Thunder3 && thunder3Recast(4) && currentMP >= BLM.MP.AspectThunder)
                            {
                                return BLM.Thunder3;
                            }
                        }
                    }

                    // Buffs
                    if (canWeave)
                    {
                        if (IsEnabled(CustomComboPreset.BlackSimpleCastsFeature))
                        {
                            // Use Triplecast only with Astral Fire/Umbral Hearts
                            if (level >= BLM.Levels.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) > 0 && (gauge.InAstralFire || gauge.UmbralHearts == 3))
                            {
                                if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) || GetRemainingCharges(BLM.Triplecast) > 1)
                                {
                                    return BLM.Triplecast;
                                }
                            }

                            // Use Swiftcast in Astral Fire
                            if (level >= BLM.Levels.Swiftcast && IsOffCooldown(BLM.Swiftcast) && gauge.InAstralFire)
                            {
                                if (level >= BLM.Levels.Despair)
                                {
                                    if (currentMP >= BLM.MP.Despair)
                                    {
                                        return BLM.Swiftcast;
                                    }
                                }
                                else
                                {
                                    if (currentMP >= BLM.MP.AspectFire)
                                    {
                                        return BLM.Swiftcast;
                                    }
                                }
                            }
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature) &&
                            level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier) && gauge.PolyglotStacks < 2)
                        {
                            return BLM.Amplifier;
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsLeylinesFeature) &&
                            level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                        {
                            return BLM.LeyLines;
                        }

                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature))
                        {
                            if (IsOffCooldown(BLM.Manafont) && gauge.InAstralFire)
                            {
                                if (level >= BLM.Levels.Despair)
                                {
                                    if (currentMP < BLM.MP.Despair)
                                    {
                                        return BLM.Manafont;
                                    }
                                }
                                else
                                {
                                    if (currentMP < BLM.MP.AspectFire)
                                    {
                                        return BLM.Manafont;
                                    }
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
                if ((level >= BLM.Levels.Blizzard4 && !gauge.IsEnochianActive) ||
                    gauge.ElementTimeRemaining <= 0)
                {
                    if (level >= BLM.Levels.Fire3)
                    {
                        if (currentMP >= BLM.MP.Fire3)
                        {
                            return BLM.Fire3;
                        }
                        return BLM.Blizzard3;
                    }
                    if (currentMP >= BLM.MP.Fire)
                    {
                        return BLM.Fire;
                    }
                    return BLM.Blizzard;
                }

                // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                if (level < BLM.Levels.Blizzard3)
                {
                    if (gauge.InAstralFire)
                    {
                        if (currentMP < BLM.MP.AspectFire)
                        {
                            return BLM.Transpose;
                        }
                        return BLM.Fire;
                    }
                    if (gauge.InUmbralIce)
                    {
                        if (currentMP >= BLM.MP.MaxMP)
                        {
                            return BLM.Transpose;
                        }
                        return BLM.Blizzard;
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
                        if (currentMP < BLM.MP.AspectFire)
                        {
                            return BLM.Blizzard3;
                        }
                        return BLM.Fire;
                    }
                    if (gauge.InUmbralIce)
                    {
                        if (level >= BLM.Levels.Blizzard4 && gauge.UmbralHearts < 3)
                        {
                            return BLM.Blizzard4;
                        }
                        if (currentMP >= BLM.MP.MaxMP)
                        {
                            return BLM.Fire3;
                        }
                        return BLM.Blizzard;
                    }
                }

                // Use polygot stacks if we don't need it for a future weave
                if (gauge.PolyglotStacks > 0 &&
                    gauge.ElementTimeRemaining >= 5000 && 
                    (gauge.InUmbralIce || (gauge.InAstralFire && gauge.UmbralHearts == 0)))
                {
                    if (level >= BLM.Levels.Xenoglossy)
                    {
                        // Check leylines cooldown
                        if (gauge.PolyglotStacks == 2 && GetCooldown(BLM.LeyLines).CooldownRemaining >= 20)
                        {
                            // Check triplecast cooldown
                            if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) && GetCooldown(BLM.Triplecast).ChargeCooldownRemaining >= 20)
                            {
                                return BLM.Xenoglossy;
                            }
                            if (IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) && 
                                GetRemainingCharges(BLM.Triplecast) == 0 ||
                                GetCooldown(BLM.Triplecast).ChargeCooldownRemaining >= 20)
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

                // Full rotation
                if (gauge.InAstralFire)
                {
                    // Refresh AF
                    if (gauge.ElementTimeRemaining <= 3000 && HasEffect(BLM.Buffs.Firestarter))
                    {
                        return BLM.Fire3;
                    }
                    if (gauge.ElementTimeRemaining <= 6000 && !HasEffect(BLM.Buffs.Firestarter) && currentMP >= BLM.MP.AspectFire)
                    {
                        if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive)
                        {
                            return BLM.Paradox;
                        }
                        return BLM.Fire;
                    }

                    // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                    if (lastComboMove != BLM.Xenoglossy && gauge.PolyglotStacks > 0)
                    {
                        var pooledPolygotStacks = IsEnabled(CustomComboPreset.BlackSimplePoolingFeature) ? 1 : 0;
                        if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature) &&
                            level >= BLM.Levels.Amplifier && IsOffCooldown(BLM.Amplifier))
                        {
                            return BLM.Xenoglossy;
                        }
                        if (gauge.PolyglotStacks > pooledPolygotStacks)
                        {
                            if (IsEnabled(CustomComboPreset.BlackSimpleBuffsLeylinesFeature) &&
                                level >= BLM.Levels.LeyLines && IsOffCooldown(BLM.LeyLines))
                            {
                                return BLM.Xenoglossy;
                            }
                            if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature) &&
                                level >= BLM.Levels.Triplecast && !HasEffect(BLM.Buffs.Triplecast) && GetRemainingCharges(BLM.Triplecast) > 0)
                            {
                                if (!IsEnabled(CustomComboPreset.BlackSimpleCastPoolingFeature) || GetRemainingCharges(BLM.Triplecast) > 1)
                                {
                                    return BLM.Xenoglossy;
                                }
                            }
                            if (IsEnabled(CustomComboPreset.BlackSimpleBuffsFeature) &&
                                level >= BLM.Levels.Manafont && IsOffCooldown(BLM.Manafont) && currentMP < BLM.MP.Despair)
                            {
                                return BLM.Xenoglossy;
                            }
                        }
                    }

                    // Blizzard3/Despair when below Fire MP
                    if (currentMP < BLM.MP.AspectFire)
                    {
                        if (level >= BLM.Levels.Despair && currentMP >= BLM.MP.Despair)
                        {
                            return BLM.Despair;
                        }
                        return BLM.Blizzard3;
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
                    if (gauge.UmbralHearts == 3)
                    {
                        return BLM.Fire3;
                    }

                    return BLM.Blizzard4;
                }
            }

            return actionID;
        }
    }
}
