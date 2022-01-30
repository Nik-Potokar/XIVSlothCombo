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
            PresenceOfMind = 157;
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
                Cure2 = 30,
                AfflatusSolace = 52,
                AfflatusRapture = 76;
        }
    }

    internal class WhiteMageSolaceMiseryFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageSolaceMiseryFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == WHM.AfflatusSolace)
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
                    if (IsEnabled(CustomComboPreset.WHMRaiseFeature))
                    {
                        var thinairCD = GetCooldown(WHM.ThinAir);
                        if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && !thinairCD.IsCooldown && HasEffect(WHM.Buffs.Swiftcast) && level >= 58)
                            return WHM.ThinAir;
                        if (HasEffect(WHM.Buffs.Swiftcast))
                            return WHM.Raise;
                    }
                }

                return actionID;
            }
        }

        internal class WHMDotMainComboFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMDotMainComboFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Glare3 || actionID == WHM.Glare1 || actionID == WHM.Stone1 || actionID == WHM.Stone2 || actionID == WHM.Stone3 || actionID == WHM.Stone4)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var diaDebuff = FindTargetEffect(WHM.Debuffs.Dia);
                    var aero1Debuff = FindTargetEffect(WHM.Debuffs.Aero);
                    var aero2Debuff = FindTargetEffect(WHM.Debuffs.Aero2);
                    var lucidDreaming = GetCooldown(WHM.LucidDreaming);
                    var presenceofmindCD = GetCooldown(WHM.PresenceOfMind);
                    var assizeCD = GetCooldown(WHM.Assize);
                    var glare3 = GetCooldown(WHM.Glare3);

                    if (IsEnabled(CustomComboPreset.WHMPresenceOfMindFeature) && level >= 30)
                    {
                        if (!presenceofmindCD.IsCooldown && glare3.CooldownRemaining > 0.2)
                            return WHM.PresenceOfMind;
                    }
                    if (IsEnabled(CustomComboPreset.WHMAssizeFeature) && level >= 56)
                    {
                        if (!assizeCD.IsCooldown && glare3.CooldownRemaining > 0.2)
                            return WHM.Assize;
                    }
                    if (IsEnabled(CustomComboPreset.WHMLucidDreamingFeature))
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && glare3.CooldownRemaining > 0.2)
                            return WHM.LucidDreaming;
                    }

                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && !IsEnabled(CustomComboPreset.WHMRemoveDotFromDPSFeature) && level >= 4 && level <= 45 && inCombat)
                    {
                        if ((aero1Debuff is null) || (aero1Debuff.RemainingTime <= 3))
                        {
                            return WHM.Aero1;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && !IsEnabled(CustomComboPreset.WHMRemoveDotFromDPSFeature) && level >= 46 && level <= 71 && inCombat)
                    {
                        if ((aero2Debuff is null) || (aero2Debuff.RemainingTime <= 3))
                        {
                            return WHM.Aero2;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && !IsEnabled(CustomComboPreset.WHMRemoveDotFromDPSFeature) && level >= 72 && inCombat)
                    {
                        if ((diaDebuff is null) || (diaDebuff.RemainingTime <= 3))
                        {
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
                    if (IsEnabled(CustomComboPreset.WhiteMageAfflatusRaptureMedicaFeature) && level >= WHM.Levels.AfflatusRapture && gauge.Lily > 0)
                        return WHM.AfflatusRapture;
                    if (HasEffect(WHM.Buffs.Medica2) && medica2Buff.RemainingTime > 2)

                        return WHM.Medica1;
                }

                return actionID;
            }
        }
    }
}
