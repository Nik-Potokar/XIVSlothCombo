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
            Bloodbath = 7542,
            HeadGraze = 7551,
            FootGraze = 7553,
            LegGraze = 7554,
            Feint = 7549,
            Interject = 7538,
            Peloton = 7557,
            LegSweep = 7863;

        public static class Buffs
        {
            public const ushort
                Weakness = 43,
                Medicated = 49,
                Bloodbath = 84,
                Swiftcast = 167,
                Peloton = 1199;
        }

        public static class Debuffs
        {
            public const ushort
                Bind = 13,
                Heavy = 14,
                Addle = 1203,
                Reprisal = 1193,
                Feint = 1195;
        }

        public static class Levels
        {
            public const byte
                LegGraze = 6,
                SecondWind = 8,
                Addle = 8,
                FootGraze = 10,
                LegSweep = 10,
                LowBlow = 12,
                Bloodbath = 12,
                Raise = 12,
                Interject = 18,
                Swiftcast = 18,
                Peloton = 20,
                Feint = 22,
                HeadGraze = 24;
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

    //Melee DPS Features
    internal class AllMeleeFeintFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllMeleeFeintFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.Feint)
            {
                if (TargetHasEffectAny(All.Debuffs.Feint) && IsOffCooldown(All.Feint))
                    return BLM.Fire;
            }

            return actionID;
        }
    }

    //Ranged Physical Features
    internal class AllRangedPhysicalMitigationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllRangedPhysicalMitigationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is BRD.Troubadour or MCH.Tactician or DNC.ShieldSamba)
            {
                if ((HasEffectAny(BRD.Buffs.Troubadour) || HasEffectAny(MCH.Buffs.Tactician) || HasEffectAny(DNC.Buffs.ShieldSamba)))
                    return DRG.Stardiver;
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

    */
}

