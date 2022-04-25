 namespace XIVSlothComboPlugin.Combos
{
    internal static class All
    {
        public const byte JobID = 99;

        public const uint
            SecondWind = 7541,
            Addle = 7560,
            Swiftcast = 7561,
            Resurrection = 173,
            Raise = 125,
            Reprisal = 7535,
            SolidReason = 232,
            AgelessWords = 215,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Interject = 7538;

        public static class Buffs
        {
            public const ushort
                Weakness = 43,
                Medicated = 49,
                Swiftcast = 167;
        }

        public static class Debuffs
        {
            public const ushort
                Addle = 1203,
                Reprisal = 1193;
        }

        public static class Levels
        {
            public const byte
                SecondWind = 8,
                Addle = 8,
                LowBlow = 12,
                Raise = 12,
                Interject = 18;
        }
    }

    //Tank Features
    internal class AllTankInterruptFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllTankInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.LowBlow or PLD.ShieldBash)
            {
                if (IsOffCooldown(All.LowBlow) && level >= All.Levels.LowBlow)
                    return All.LowBlow;
                if (CanInterruptEnemy() && IsOffCooldown(All.Interject) && level >= All.Levels.Interject)
                    return All.Interject;
                if (actionID == PLD.ShieldBash && IsOnCooldown(All.LowBlow))
                    return actionID;
            }

            return actionID;
        }
    }

    internal class AllTankReprisalFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllTankReprisalFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.Reprisal)
            {
                if (TargetHasEffectAny(All.Debuffs.Reprisal) && IsOffCooldown(All.Reprisal))
                    return WHM.Stone1;
            }
            return actionID;
        }
    }

    //Caster Features
    internal class AllCasterAddleFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllCasterAddleFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.Addle)
            {
                if (TargetHasEffectAny(All.Debuffs.Addle) && IsOffCooldown(All.Addle))
                    return WAR.FellCleave;
            }
            return actionID;
        }
    }

    /*
    internal class DoMSwiftcastFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DoMSwiftcastFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.DoMSwiftcastFeature))
            {
                if (actionID == WHM.Raise || actionID == SMN.Resurrection || actionID == SGE.Egeiro || actionID == AST.Ascend || actionID == RDM.Verraise)
                {
                    var swiftCD = GetCooldown(All.Swiftcast);
                    if ((swiftCD.CooldownRemaining == 0 && !HasEffect(RDM.Buffs.Dualcast))
                        || level <= All.Levels.Raise
                        || (level <= RDM.Levels.Verraise && actionID == RDM.Verraise))
                        return All.Swiftcast;
                }
            }

            return actionID;
        }
    }

    internal class AllTankReprisalFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AllTankReprisalFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.Reprisal)
            {
                if (TargetHasEffectAny(All.Debuffs.Reprisal))
                    return WHM.Stone1;
            }

            return actionID;
        }
    }
    */
}

