namespace XIVComboExpandedPlugin.Combos
{
    internal static class PLD
    {
        public const byte ClassID = 1;
        public const byte JobID = 19;

        public const uint
            FastBlade = 9,
            RiotBlade = 15,
            ShieldBash = 16,
            RageOfHalone = 21,
            GoringBlade = 3538,
            RoyalAuthority = 3539,
            LowBlow = 7540,
            TotalEclipse = 7381,
            Requiescat = 7383,
            HolySpirit = 7384,
            Prominence = 16457,
            HolyCircle = 16458,
            Confiteor = 16459,
            Atonement = 16460;

        public static class Buffs
        {
            public const short
                Requiescat = 1368,
                SwordOath = 1902;
        }

        public static class Debuffs
        {
            public const short
                GoringBlade = 725;
        }

        public static class Levels
        {
            public const byte
                RiotBlade = 4,
                RageOfHalone = 26,
                Prominence = 40,
                GoringBlade = 54,
                RoyalAuthority = 60,
                HolyCircle = 72,
                Atonement = 76,
                Confiteor = 80;
        }
    }

    internal class PaladinGoringBladeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinGoringBladeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.GoringBlade)
            {
                 if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                {
                    // Replace with Holy Spirit when Requiescat is up
                    if (HasEffect(PLD.Buffs.Requiescat))
                     {
                        if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && LocalPlayer.CurrentMp < 4000)
                            return PLD.Confiteor;

                        return PLD.HolySpirit;
                    }
                }

                 if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade)
                        return PLD.GoringBlade;
                }

                 return PLD.FastBlade;
            }

            return actionID;
        }
    }

    internal class PaladinRoyalAuthorityCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinRoyalAuthorityCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.RoyalAuthority || actionID == PLD.RageOfHalone)
            {
                 if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                 {
                     // Replace with Holy Spirit when Requiescat is up
                     if (HasEffect(PLD.Buffs.Requiescat))
                     {
                         // Replace with Confiteor when under 4000 MP
                         if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && LocalPlayer.CurrentMp < 4000)
                             return PLD.Confiteor;
                         return PLD.HolySpirit;
                    }
                 }

                 if (comboTime > 0)
                {
                    var goringBladeDebuff = TargetHasEffect(PLD.Debuffs.GoringBlade);
                    var goingBladeDebuffTimer = FindTargetEffectAny(PLD.Debuffs.GoringBlade);
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade && !goringBladeDebuff)
                        return PLD.GoringBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade && goingBladeDebuffTimer.RemainingTime < 5)
                        return PLD.GoringBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        return OriginalHook(PLD.RageOfHalone);
                }

                 if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (HasEffect(PLD.Buffs.SwordOath))
                        return PLD.Atonement;
                }

                 return PLD.FastBlade;
            }

            return actionID;
        }
    }

    internal class PaladinProminenceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinProminenceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.Prominence)
            {
                 if (IsEnabled(CustomComboPreset.PaladinRequiescatFeature))
                 {
                     if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
                     {
                         if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature) && level >= PLD.Levels.Confiteor && LocalPlayer.CurrentMp < 4000)
                             return PLD.Confiteor;

                         return PLD.HolyCircle;
                     }
                 }

                 if (comboTime > 0)
                {
                    if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;
                }

                 return PLD.TotalEclipse;
            }

            return actionID;
        }
    }

    internal class PaladinConfiteorFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinConfiteorFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolySpirit)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.Confiteor && LocalPlayer?.CurrentMp < 4000)
                    return PLD.Confiteor;

                return PLD.HolySpirit;
            }

            if (actionID == PLD.HolyCircle)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.Confiteor && LocalPlayer?.CurrentMp < 4000)
                    return PLD.Confiteor;

                return PLD.HolyCircle;
            }

            return actionID;
        }
    }

    internal class PaladinRequiescatCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinRequiescatCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.Requiescat)
            {
                if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.Confiteor)
                    return PLD.Confiteor;

                return PLD.Requiescat;
            }

            return actionID;
        }
    }
}
