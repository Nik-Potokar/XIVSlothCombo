using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
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

            // DoT
            Dia = 16532,
            Aero1 = 121,
            Aero2 = 132;

        public static class Buffs
        {
            public const short
            Swiftcast = 167,
            Medica2 = 150;
        }

        public static class Debuffs
        {
            public const short
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
        protected override CustomComboPreset Preset => CustomComboPreset.WhiteMageSolaceMiseryFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (gauge.BloodLily == 3)
                return WHM.AfflatusMisery;

            return actionID;
        }
    }

    internal class WhiteMageRaptureMiseryFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.WhiteMageRaptureMiseryFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (gauge.BloodLily == 3)
                return WHM.AfflatusMisery;

            return actionID;
        }
    }

    internal class WhiteMageCureFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.WhiteMageCureFeature;

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
        protected override CustomComboPreset Preset => CustomComboPreset.WhiteMageAfflatusFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (actionID == WHM.Cure2)
            {
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
            protected override CustomComboPreset Preset => CustomComboPreset.WHMRaiseFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Swiftcast)
                {
                    if (IsEnabled(CustomComboPreset.WHMRaiseFeature))
                    {
                        if (HasEffect(WHM.Buffs.Swiftcast))
                            return WHM.Raise;
                    }
                }

                return actionID;
            }
        }

        internal class WHMDotMainComboFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.WHMDotMainComboFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Glare3 || actionID == WHM.Glare1 || actionID == WHM.Stone1 || actionID == WHM.Stone2 || actionID == WHM.Stone3 || actionID == WHM.Stone4)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var diaDebuff = FindTargetEffect(WHM.Debuffs.Dia);
                    var aero1Debuff = FindTargetEffect(WHM.Debuffs.Aero);
                    var aero2Debuff = FindTargetEffect(WHM.Debuffs.Aero2);
                    var lucidDreaming = GetCooldown(WHM.LucidDreaming);
                    var glare3 = GetCooldown(WHM.Glare3);

                    if (IsEnabled(CustomComboPreset.WHMLucidDreamingFeature))
                    {
                        if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && glare3.CooldownRemaining > 0.7)
                            return WHM.LucidDreaming;
                    }
                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && level >= 4 && level <= 45)
                    {
                        if ((!TargetHasEffect(WHM.Debuffs.Aero) && inCombat && level >= 4 && level <= 45) || (aero1Debuff.RemainingTime <= 3 && inCombat && level >= 4 && level <= 45))
                        {
                            return WHM.Aero1;
                        }

                    }
                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && level >= 46 && level <= 71)
                    {
                        if ((!TargetHasEffect(WHM.Debuffs.Aero2) && inCombat && level >= 46 && level <= 71) || (aero2Debuff.RemainingTime <= 3 && inCombat && level >= 46 && level <= 71))
                        {
                            return WHM.Aero2;
                        }

                    }
                    if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && level >= 72)
                    {
                        if ((!TargetHasEffect(WHM.Debuffs.Dia) && inCombat && level >= 72) || (diaDebuff.RemainingTime <= 3 && inCombat && level >= 72))
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
            protected override CustomComboPreset Preset => CustomComboPreset.WHMMedicaFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Medica2)
                {
                    if (HasEffect(WHM.Buffs.Medica2))
                        return WHM.Medica1;
                }
                return actionID;
            }
        }
    }
}
