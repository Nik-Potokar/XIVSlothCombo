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
            AfflatusMisery = 16535,
            Medica1 = 124,
            Medica2 = 133,
            Tetragrammaton = 3570,
            DivineBenison = 7432,

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
            Medica2 = 150,
            PresenceOfMind = 157,
            ThinAir = 1217,
            DivineBenison = 1218;
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
                Medica2 = 50,
                AfflatusSolace = 52,
                Assize = 56,
                ThinAir = 58,
                Tetragrammaton = 60,
                DivineBenison = 66,
                Dia = 72,
                AfflatusMisery = 74,
                AfflatusRapture = 76,
                Glare3 = 82;
        }

        public static class Config
        {
            public const string
                WHMLucidDreamingFeature = "WHMLucidDreamingFeature",
                WHMogcdHealsShieldsFeature = "WHMogcdHealsShieldsFeature";
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
            var tetraHP = Service.Configuration.GetCustomIntValue(WHM.Config.WHMogcdHealsShieldsFeature);


            if (actionID == WHM.Cure2)
            {
                if (IsEnabled(CustomComboPreset.WHMPrioritizeoGCDHealsShields) && IsEnabled(CustomComboPreset.WHMBenisonGCDOption) //Is the priority option enabled
                    && level >= WHM.Levels.DivineBenison && !TargetHasEffectAny(WHM.Buffs.DivineBenison) && HasCharges(WHM.DivineBenison) //Can I use Divine Benison
                     && (GetCooldown(WHM.DivineBenison).RemainingCharges == 2 || GetCooldown(WHM.DivineBenison).ChargeCooldownRemaining <= 29)) //Did I just use Divine Benison
                    return actionID;
                if (IsEnabled(CustomComboPreset.WHMPrioritizeoGCDHealsShields) && IsEnabled(CustomComboPreset.WHMTetraOnGCDOption)
                    && IsOffCooldown(WHM.Tetragrammaton) && level >= WHM.Levels.Tetragrammaton && EnemyHealthPercentage() <= tetraHP)
                    return actionID;
                else if (IsEnabled(CustomComboPreset.WhiteMageAfflatusMiseryCure2Feature) && gauge.BloodLily == 3)
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
                if (actionID == All.Swiftcast)
                {
                    var thinairCD = GetCooldown(WHM.ThinAir);
                    var hasThinAirBuff = HasEffect(WHM.Buffs.ThinAir);

                    if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && thinairCD.RemainingCharges > 0 && HasEffect(All.Buffs.Swiftcast) && !hasThinAirBuff && level >= WHM.Levels.ThinAir)
                        return WHM.ThinAir;
                    if (HasEffect(All.Buffs.Swiftcast))
                        return WHM.Raise;
                }

                return actionID;
            }
        }

        internal class WHMCDsonMainComboGroup : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMCDsonMainComboGroup;
            internal static uint glare3Count = 0;
            internal static bool usedGlare3 = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == WHM.Glare3 || actionID == WHM.Glare1 || actionID == WHM.Stone1 || actionID == WHM.Stone2 || actionID == WHM.Stone3 || actionID == WHM.Stone4)
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var diaDebuff = FindTargetEffect(WHM.Debuffs.Dia);
                    var aero1Debuff = FindTargetEffect(WHM.Debuffs.Aero);
                    var aero2Debuff = FindTargetEffect(WHM.Debuffs.Aero2);
                    var lucidThreshold = Service.Configuration.GetCustomIntValue(WHM.Config.WHMLucidDreamingFeature);
                    var gauge = GetJobGauge<WHMGauge>();

                    //WHM_NO_SWIFT_OPENER_MACHINE
                    //COUNTER_RESET
                    if (!inCombat) glare3Count = 0; // Resets counter
                    //CHECK_GLARE3_USE
                    if (inCombat && usedGlare3 == false && lastComboMove == WHM.Glare3 && GetCooldownRemainingTime(WHM.Glare3) > 1)
                    {
                        usedGlare3 = true; // Registers that Glare3 was used and blocks further incrementation of glare3Count
                        glare3Count++; // Increments Glare3 counter
                    }
                    //CHECK_GLARE3_USE_RESET
                    if (usedGlare3 == true && GetCooldownRemainingTime(WHM.Glare3) < 1) usedGlare3 = false; // Resets block to allow CHECK_GLARE3_USE
                    //BYPASS_COUNTER_WHEN_DISABLED
                    if (IsNotEnabled(CustomComboPreset.WHMNoSwiftOpenerOption) || level < WHM.Levels.Glare3) glare3Count = 3;

                    if (CanSpellWeave(actionID) && glare3Count >= 3)
                    {
                        if (IsEnabled(CustomComboPreset.WHMPresenceOfMindFeature) && level >= WHM.Levels.PresenceOfMind && IsOffCooldown(WHM.PresenceOfMind))
                            return WHM.PresenceOfMind;
                        if (IsEnabled(CustomComboPreset.WHMAssizeFeature) && level >= WHM.Levels.Assize && IsOffCooldown(WHM.Assize))
                            return WHM.Assize;
                        if (IsEnabled(CustomComboPreset.WHMLucidDreamingFeature) && IsOffCooldown(WHM.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold)
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

                    if (IsEnabled(CustomComboPreset.WHMLilyOvercapFeature) && level >= WHM.Levels.AfflatusRapture && ((gauge.Lily == 3) || (gauge.Lily == 2 && gauge.LilyTimer >= 17000)))
                                return WHM.AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHMAfflatusMiseryOGCDFeature) && level >= WHM.Levels.AfflatusMisery && gauge.BloodLily >= 3 && glare3Count >= 3)
                                return WHM.AfflatusMisery;
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
                    if (level < WHM.Levels.Medica2)
                        return WHM.Medica1;
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
                var swiftCD = GetCooldown(All.Swiftcast);

                if (!swiftCD.IsCooldown)
                    return All.Swiftcast;

                if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && thinairCD.RemainingCharges > 0 && !hasThinAirBuff && level >= WHM.Levels.ThinAir)
                    return WHM.ThinAir;

                if (!swiftCD.IsCooldown)
                    return All.Swiftcast;
            }
            return actionID;
        }
    }
    internal class WHMogcdHealsShieldsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMogcdHealsShieldsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var tetraHP = Service.Configuration.GetCustomIntValue(WHM.Config.WHMogcdHealsShieldsFeature);

            if (actionID == WHM.Cure2)
            {
                if (level >= WHM.Levels.DivineBenison && HasCharges(WHM.DivineBenison) && !TargetHasEffectAny(WHM.Buffs.DivineBenison)
                    && (GetCooldown(WHM.DivineBenison).RemainingCharges == 2 || GetCooldown(WHM.DivineBenison).ChargeCooldownRemaining <= 29))
                {
                    if (IsEnabled(CustomComboPreset.WHMBenisonOGCDOption) && CanSpellWeave(actionID)) { return WHM.DivineBenison; }
                    if (IsEnabled(CustomComboPreset.WHMBenisonGCDOption)) { return WHM.DivineBenison; }
                }
                if (level >= WHM.Levels.Tetragrammaton && IsOffCooldown(WHM.Tetragrammaton) && EnemyHealthPercentage() <= tetraHP)
                {
                    if (IsEnabled(CustomComboPreset.WHMTetraOnOGCDOption) && CanSpellWeave(actionID)) { return WHM.Tetragrammaton; }
                    if (IsEnabled(CustomComboPreset.WHMTetraOnGCDOption)) { return WHM.Tetragrammaton; }
                }
            }

            return actionID;
        }
    }

}
