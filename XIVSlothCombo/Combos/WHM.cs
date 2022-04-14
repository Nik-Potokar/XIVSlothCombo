using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class WHM
    {
        public const byte ClassID = 6;
        public const byte JobID = 24;

        public const uint
            Cure = 120,
            Medica = 124,
            Cure2 = 135,
            AfflatusSolace = 16531,
            AfflatusRapture = 16534,
            LucidDreaming = 7562,
            Raise = 125,
            Swiftcast = 7561,
            AfflatusMisery = 16535,
            Medica1 = 124,
            Medica2 = 133,

            // dps
            Glare1 = 16533,
            Glare3 = 25859,
            Stone1 = 119,
            Stone2 = 127,
            Stone3 = 3568,
            Stone4 = 7431,
            Assize = 3571,

            // DoT
            Dia = 16532,
            Aero1 = 121,
            Aero2 = 132,

            // buff
            ThinAir = 7430,
            PresenceOfMind = 136;



        public static class Buffs
        {
            public const ushort
            Swiftcast = 167,
            Medica2 = 150,
            PresenceOfMind = 157,
            ThinAir = 1217;
        }

        public static class Debuffs
        {
            public const ushort
            Dia = 1871,
            Aero = 143,
            Aero2 = 144;

        }

        public static class Levels
        {
            public const byte
                Aero1 = 4,
                PresenceOfMind = 30,
                Cure2 = 30,
                Aero2 = 46,
                AfflatusSolace = 52,
                Assize = 56,
                ThinAir = 58,
                Dia = 72,
                AfflatusRapture = 76;
        }

        public static class Config
        {
            public const string
                WHMLucidDreamingFeature = "WHMLucidDreamingFeature";
        }
    }

    internal class WhiteMageSolaceMiseryFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageSolaceMiseryFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WHM.AfflatusSolace)
            {
                var gauge = GetJobGauge<WHMGauge>();

                if (gauge.BloodLily == 3)
                    return WHM.AfflatusMisery;

            }
            return actionID;
        }
    }

    internal class WhiteMageRaptureMiseryFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageRaptureMiseryFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WHM.AfflatusRapture)
            {
                var gauge = GetJobGauge<WHMGauge>();

                if (gauge.BloodLily == 3)
                    return WHM.AfflatusMisery;

            }
            return actionID;
        }
    }

    internal class WhiteMageCureFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageCureFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WHM.Cure2)
            {
                if (level < WHM.Levels.Cure2)
                    return WHM.Cure;
            }

            return actionID;
        }
    }

    internal class WhiteMageAfflatusFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageAfflatusFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (actionID == WHM.Cure2)
            {
                if (IsEnabled(CustomComboPreset.WhiteMageAfflatusMiseryCure2Feature) && gauge.BloodLily == 3)
                    return WHM.AfflatusMisery;
                if (level >= WHM.Levels.AfflatusSolace && gauge.Lily > 0)
                    return WHM.AfflatusSolace;

                return actionID;
            }

            if (actionID == WHM.Medica)
            {
                if (level >= WHM.Levels.AfflatusRapture && gauge.Lily > 0)
                    return WHM.AfflatusRapture;

                return actionID;
            }

            return actionID;
        }

        internal class WHMRaiseFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMRaiseFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Swiftcast)
                {
                    var thinairCD = GetCooldown(WHM.ThinAir);
                    var hasThinAirBuff = HasEffect(WHM.Buffs.ThinAir);

                    if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && thinairCD.RemainingCharges > 0 && HasEffect(WHM.Buffs.Swiftcast) && !hasThinAirBuff && level >= WHM.Levels.ThinAir)
                        return WHM.ThinAir;
                    if (HasEffect(WHM.Buffs.Swiftcast))
                        return WHM.Raise;
                }

                return actionID;
            }
        }

        internal class WHMCDsonMainComboGroup : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMCDsonMainComboGroup;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Glare3 || actionID == WHM.Glare1 || actionID == WHM.Stone1 || actionID == WHM.Stone2 || actionID == WHM.Stone3 || actionID == WHM.Stone4)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var diaDebuff = FindTargetEffect(WHM.Debuffs.Dia);
                    var aero1Debuff = FindTargetEffect(WHM.Debuffs.Aero);
                    var aero2Debuff = FindTargetEffect(WHM.Debuffs.Aero2);

                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.WHMPresenceOfMindFeature) && level >= WHM.Levels.PresenceOfMind && IsOffCooldown(WHM.PresenceOfMind))
                                return WHM.PresenceOfMind;
                        if (IsEnabled(CustomComboPreset.WHMAssizeFeature) && level >= WHM.Levels.Assize && IsOffCooldown(WHM.Assize))
                                return WHM.Assize;
                        if (IsEnabled(CustomComboPreset.WHMLucidDreamingFeature) && IsOffCooldown(WHM.LucidDreaming) && LocalPlayer.CurrentMp <= 8000)
                                return WHM.LucidDreaming;
                    }

                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && inCombat)
                    {
                        if (level >= WHM.Levels.Aero1 && level < WHM.Levels.Aero2)
                        {
                            if ((aero1Debuff is null) || (aero1Debuff.RemainingTime <= 3))
                                return WHM.Aero1;
                        }

                        if (level >= WHM.Levels.Aero2 && level < WHM.Levels.Dia)
                        {
                            if ((aero2Debuff is null) || (aero2Debuff.RemainingTime <= 3))
                                return WHM.Aero2;
                        }

                        if (level >= WHM.Levels.Dia)
                        {
                            if ((diaDebuff is null) || (diaDebuff.RemainingTime <= 3))
                                return WHM.Dia;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class WHMMedicaFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMMedicaFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Medica2)
                {
                    var gauge = GetJobGauge<WHMGauge>();
                    var medica2Buff = FindEffect(WHM.Buffs.Medica2);
                    if (IsEnabled(CustomComboPreset.WhiteMageAfflatusMiseryMedicaFeature) && gauge.BloodLily == 3)
                        return WHM.AfflatusMisery;
                    if (IsEnabled(CustomComboPreset.WhiteMageAfflatusRaptureMedicaFeature) && level >= WHM.Levels.AfflatusRapture && gauge.Lily > 0 && medica2Buff.RemainingTime > 2)
                        return WHM.AfflatusRapture;
                    if (HasEffect(WHM.Buffs.Medica2) && medica2Buff.RemainingTime > 2)
                        return WHM.Medica1;
                }

                return actionID;
            }
        }

    }
    internal class WHMAlternativeRaise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMAlternativeRaise;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WHM.Raise)
            {
                var thinairCD = GetCooldown(WHM.ThinAir);
                var hasThinAirBuff = HasEffect(WHM.Buffs.ThinAir);
                var swiftCD = GetCooldown(WHM.Swiftcast);

                if (!swiftCD.IsCooldown)
                    return WHM.Swiftcast;

                if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && thinairCD.RemainingCharges > 0 && !hasThinAirBuff && level >= WHM.Levels.ThinAir)
                    return WHM.ThinAir;

                if (!swiftCD.IsCooldown)
                    return WHM.Swiftcast;
            }
            return actionID;
        }
    }

}
