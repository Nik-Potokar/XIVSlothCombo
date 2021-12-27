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
            Xenoglossy = 16507;

        public static class Buffs
        {
            public const short
                Thundercloud = 164,
                LeyLines = 737,
                Firestarter = 165;
        }

        public static class Debuffs
        {
            public const short
                Thunder = 161,
                Thunder2 = 162,
                Thunder3 = 163,
                Thunder4 = 1210;
        }

        public static class Levels
        {
            public const byte
                Fire3 = 34,
                Freeze = 35,
                Blizzard3 = 40,
                Thunder3 = 45,
                Flare = 50,
                Blizzard4 = 58,
                Fire4 = 60,
                BetweenTheLines = 62,
                Despair = 72,
                UmbralSoul = 76,
                Xenoglossy = 80;
        }
    }

    internal class BlackBlizzardFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackBlizzardFeature;

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

            if (actionID == BLM.Freeze && level < 35)
            {
                return 146u;
            }

            return actionID;
        }
    }

    internal class BlackFireFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackFire13Feature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.BlackLeyLinesFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.BlackManaFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.BlackEnochianFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BLM.Scathe)
            {
                var gauge = GetJobGauge<BLMGauge>();
                var thundercloudduration = FindEffectAny(BLM.Buffs.Thundercloud);
                var thunderdebuffontarget = FindTargetEffect(BLM.Debuffs.Thunder3);
                var thunderOneDebuff = FindTargetEffect(BLM.Debuffs.Thunder);
                var thunder3DebuffOnTarget = TargetHasEffect(BLM.Debuffs.Thunder3);

                if (gauge.InUmbralIce && level >= BLM.Levels.Blizzard4)
                {
                    if (gauge.ElementTimeRemaining >= 0 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature))
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if ((TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(BLM.Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                return BLM.Thunder3;
                        }

                        if (gauge.IsParadoxActive && level >= 90)
                            return BLM.Paradox;
                    }

                    return BLM.Blizzard4;
                }

                if (level >= BLM.Levels.Fire4)
                {
                    if (gauge.ElementTimeRemaining >= 6000 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature))
                    {
                        if (HasEffect(BLM.Buffs.Thundercloud))
                        {
                            if ((TargetHasEffect(BLM.Debuffs.Thunder3) && thunderdebuffontarget.RemainingTime < 4) || (!thunder3DebuffOnTarget && HasEffect(BLM.Buffs.Thundercloud) && thundercloudduration.RemainingTime > 0 && thundercloudduration.RemainingTime < 35))
                                return BLM.Thunder3;
                        }
                    }

                    if (gauge.ElementTimeRemaining < 3000 && HasEffect(BLM.Buffs.Firestarter) && CustomCombo.IsEnabled(CustomComboPreset.BlackFire13Feature))
                        return BLM.Fire3;
                    if (LocalPlayer.CurrentMp < 2400 && level >= BLM.Levels.Despair && CustomCombo.IsEnabled(CustomComboPreset.BlackDespairFeature))
                    {
                        return BLM.Despair;
                    }

                    if (gauge.ElementTimeRemaining < 6000 && !HasEffect(BLM.Buffs.Firestarter) && CustomCombo.IsEnabled(CustomComboPreset.BlackFire13Feature) && level == 90 && gauge.IsParadoxActive)
                        return BLM.Paradox;
                    if (gauge.ElementTimeRemaining < 6000 && !HasEffect(BLM.Buffs.Firestarter) && CustomCombo.IsEnabled(CustomComboPreset.BlackFire13Feature) && !gauge.IsParadoxActive)
                        return BLM.Fire;
                    return BLM.Fire4;
                }

                if (gauge.ElementTimeRemaining >= 5000 && CustomCombo.IsEnabled(CustomComboPreset.BlackThunderFeature) && level < BLM.Levels.Thunder3)
                {
                    if (HasEffect(BLM.Buffs.Thundercloud))
                    {
                        if (TargetHasEffect(BLM.Debuffs.Thunder) && thunderOneDebuff.RemainingTime < 4)
                            return BLM.Thunder;
                    }
                }

                if (level < BLM.Levels.Fire3)
                    return BLM.Fire;
                if (gauge.InAstralFire)
                {
                    if (HasEffect(BLM.Buffs.Firestarter) && level == 90)
                        return BLM.Paradox;
                    if (HasEffect(BLM.Buffs.Firestarter))
                        return BLM.Fire3;
                    return BLM.Fire;
                }
            }

            return actionID;
        }
    }

    internal class BlackAoEComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BlackAoEComboFeature;

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
                        return BLM.Freeze;
                }
                if (IsEnabled(CustomComboPreset.BlackAoEComboFeature) && level >= 26 && level <= 63)
                {
                    if ((gauge.InUmbralIce && gauge.UmbralHearts == 3 && !thunder2Debuff) || (gauge.InUmbralIce && gauge.UmbralHearts == 3 && thunder2Timer.RemainingTime <= 3 && level >= 26 && level <= 63) || (gauge.InAstralFire && !thunder2Debuff && level >= 26 && level <= 63))
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
                    if ((gauge.InUmbralIce && gauge.UmbralHearts == 3 && thunder2Debuff && thunder2Timer.RemainingTime >= 3) || (gauge.InUmbralIce && gauge.UmbralHearts == 3 && lastComboMove == BLM.Thunder2))
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
}
