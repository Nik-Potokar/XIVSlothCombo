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
            Holy = 139,
            Holy3 = 25860,

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
                WHM_AoE_Lucid = "WHM_AoE_Lucid",
                WHMogcdHealsShieldsFeature = "WHMogcdHealsShieldsFeature";
        }


        internal class WhiteMageSolaceMiseryFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageSolaceMiseryFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AfflatusSolace)
                {
                    var gauge = GetJobGauge<WHMGauge>();

                    if (gauge.BloodLily == 3)
                        return AfflatusMisery;

                }
                return actionID;
            }
        }

        internal class WhiteMageRaptureMiseryFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageRaptureMiseryFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AfflatusRapture)
                {
                    var gauge = GetJobGauge<WHMGauge>();

                    if (gauge.BloodLily == 3)
                        return AfflatusMisery;

                }
                return actionID;
            }
        }

        internal class WhiteMageCureFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageCureFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Cure2)
                {
                    if (level < Levels.Cure2)
                        return Cure;
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
                var tetraHP = Service.Configuration.GetCustomIntValue(Config.WHMogcdHealsShieldsFeature);


                if (actionID == Cure2)
                {
                    if (IsEnabled(CustomComboPreset.WHMPrioritizeoGCDHealsShields) && IsEnabled(CustomComboPreset.WHMBenisonGCDOption) //Is the priority option enabled
                        && level >= Levels.DivineBenison && !TargetHasEffectAny(Buffs.DivineBenison) && HasCharges(DivineBenison) //Can I use Divine Benison
                         && (GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29)) //Did I just use Divine Benison
                        return actionID;
                    if (IsEnabled(CustomComboPreset.WHMPrioritizeoGCDHealsShields) && IsEnabled(CustomComboPreset.WHMTetraOnGCDOption)
                        && IsOffCooldown(Tetragrammaton) && level >= Levels.Tetragrammaton && EnemyHealthPercentage() <= tetraHP)
                        return actionID;
                    else if (IsEnabled(CustomComboPreset.WhiteMageAfflatusMiseryCure2Feature) && gauge.BloodLily == 3)
                        return AfflatusMisery;
                    if (level >= Levels.AfflatusSolace && gauge.Lily > 0)
                        return AfflatusSolace;

                    return actionID;
                }

                if (actionID == Medica)
                {
                    if (level >= Levels.AfflatusRapture && gauge.Lily > 0)
                        return AfflatusRapture;

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
                        var thinairCD = GetCooldown(ThinAir);
                        var hasThinAirBuff = HasEffect(Buffs.ThinAir);

                        if (IsEnabled(CustomComboPreset.WHMThinAirFeature) && thinairCD.RemainingCharges > 0 && HasEffect(All.Buffs.Swiftcast) && !hasThinAirBuff && level >= Levels.ThinAir)
                            return ThinAir;
                        if (HasEffect(All.Buffs.Swiftcast))
                            return Raise;
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
                    if (actionID == Glare3 || actionID == Glare1 || actionID == Stone1 || actionID == Stone2 || actionID == Stone3 || actionID == Stone4)
                    {
                        var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                        var diaDebuff = FindTargetEffect(Debuffs.Dia);
                        var aero1Debuff = FindTargetEffect(Debuffs.Aero);
                        var aero2Debuff = FindTargetEffect(Debuffs.Aero2);
                        var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.WHMLucidDreamingFeature);
                        var gauge = GetJobGauge<WHMGauge>();

                        //WHM_NO_SWIFT_OPENER_MACHINE
                        //COUNTER_RESET
                        if (!inCombat) glare3Count = 0; // Resets counter
                                                        //CHECK_GLARE3_USE
                        if (inCombat && usedGlare3 == false && lastComboMove == Glare3 && GetCooldownRemainingTime(Glare3) > 1)
                        {
                            usedGlare3 = true; // Registers that Glare3 was used and blocks further incrementation of glare3Count
                            glare3Count++; // Increments Glare3 counter
                        }
                        //CHECK_GLARE3_USE_RESET
                        if (usedGlare3 == true && GetCooldownRemainingTime(Glare3) < 1) usedGlare3 = false; // Resets block to allow CHECK_GLARE3_USE
                                                                                                                //BYPASS_COUNTER_WHEN_DISABLED
                        if (IsNotEnabled(CustomComboPreset.WHMNoSwiftOpenerOption) || level < Levels.Glare3) glare3Count = 3;

                        if (CanSpellWeave(actionID) && glare3Count >= 3)
                        {
                            if (IsEnabled(CustomComboPreset.WHMPresenceOfMindFeature) && level >= Levels.PresenceOfMind && IsOffCooldown(PresenceOfMind))
                                return PresenceOfMind;
                            if (IsEnabled(CustomComboPreset.WHMAssizeFeature) && level >= Levels.Assize && IsOffCooldown(Assize))
                                return Assize;
                            if (IsEnabled(CustomComboPreset.WHMLucidDreamingFeature) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                                return All.LucidDreaming;
                        }

                        if (IsEnabled(CustomComboPreset.WHMDotMainComboFeature) && inCombat)
                        {
                            if (level >= Levels.Aero1 && level < Levels.Aero2)
                            {
                                if ((aero1Debuff is null) || (aero1Debuff.RemainingTime <= 3))
                                    return Aero1;
                            }

                            if (level >= Levels.Aero2 && level < Levels.Dia)
                            {
                                if ((aero2Debuff is null) || (aero2Debuff.RemainingTime <= 3))
                                    return Aero2;
                            }

                            if (level >= Levels.Dia)
                            {
                                if ((diaDebuff is null) || (diaDebuff.RemainingTime <= 3))
                                    return Dia;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WHMLilyOvercapFeature) && level >= Levels.AfflatusRapture && ((gauge.Lily == 3) || (gauge.Lily == 2 && gauge.LilyTimer >= 17000)))
                            return AfflatusRapture;

                        if (IsEnabled(CustomComboPreset.WHMAfflatusMiseryOGCDFeature) && level >= Levels.AfflatusMisery && gauge.BloodLily >= 3 && glare3Count >= 3)
                            return AfflatusMisery;
                    }

                    return actionID;
                }
            }

            internal class WHMMedicaFeature : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMMedicaFeature;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == Medica2)
                    {
                        var gauge = GetJobGauge<WHMGauge>();
                        var medica2Buff = FindEffect(Buffs.Medica2);
                        if (level < Levels.Medica2)
                            return Medica1;
                        if (IsEnabled(CustomComboPreset.WhiteMageAfflatusMiseryMedicaFeature) && gauge.BloodLily == 3)
                            return AfflatusMisery;
                        if (IsEnabled(CustomComboPreset.WhiteMageAfflatusRaptureMedicaFeature) && level >= Levels.AfflatusRapture && gauge.Lily > 0 && medica2Buff.RemainingTime > 2)
                            return AfflatusRapture;
                        if (HasEffect(Buffs.Medica2) && medica2Buff.RemainingTime > 2)
                            return Medica1;
                    }

                    return actionID;
                }
            }

        }

        internal class WHMogcdHealsShieldsFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMogcdHealsShieldsFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var tetraHP = Service.Configuration.GetCustomIntValue(Config.WHMogcdHealsShieldsFeature);

                if (actionID == Cure2)
                {
                    if (level >= Levels.DivineBenison && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison)
                        && (GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29))
                    {
                        if (IsEnabled(CustomComboPreset.WHMBenisonOGCDOption) && CanSpellWeave(actionID)) { return DivineBenison; }
                        if (IsEnabled(CustomComboPreset.WHMBenisonGCDOption)) { return DivineBenison; }
                    }
                    if (level >= Levels.Tetragrammaton && IsOffCooldown(Tetragrammaton) && EnemyHealthPercentage() <= tetraHP)
                    {
                        if (IsEnabled(CustomComboPreset.WHMTetraOnOGCDOption) && CanSpellWeave(actionID)) { return Tetragrammaton; }
                        if (IsEnabled(CustomComboPreset.WHMTetraOnGCDOption)) { return Tetragrammaton; }
                    }
                }

                return actionID;
            }
        }
        internal class WHM_AoE_DPS_Feature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_AoE_DPS_Feature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Holy or Holy3)
                {
                    var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.WHM_AoE_Lucid);
                    var gauge = GetJobGauge<WHMGauge>();

                    if (CanSpellWeave(actionID) && IsEnabled(CustomComboPreset.WHM_AoE_Lucid) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                        return All.LucidDreaming;

                    if (CanSpellWeave(actionID) && IsEnabled(CustomComboPreset.WHM_AoE_Assize) && level >= Levels.Assize && IsOffCooldown(Assize))
                        return Assize;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_LilyOvercap) && level >= Levels.AfflatusRapture && ((gauge.Lily == 3 && gauge.BloodLily < 3) || (gauge.Lily == 2 && gauge.LilyTimer >= 17000)))
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_AfflatusMisery) && level >= Levels.AfflatusMisery && gauge.BloodLily >= 3 && CurrentTarget is Dalamud.Game.ClientState.Objects.Types.BattleNpc)
                        return AfflatusMisery;
                }

                return actionID;
            }
        }

    }

}
