using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class WHM
    {
        public const byte ClassID = 6;
        public const byte JobID = 24;

        public const uint

            // Heals
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

            // DPS
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

            // Buffs
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
                WHM_ST_Lucid = "WHMLucidDreamingFeature",
                WHM_AoE_Lucid = "WHM_AoE_Lucid",
                WHM_oGCDHeals = "WHMogcdHealsShieldsFeature";
        }

        internal class WHM_SolaceMisery : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_SolaceMisery;

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

        internal class WHM_RaptureMisery : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_RaptureMisery;

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

        internal class WHM_CureSync : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_CureSync;

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

        internal class WHM_Afflatus : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Afflatus;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<WHMGauge>();
                var tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);


                if (actionID == Cure2)
                {
                    if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison) //Is the priority option enabled
                        && level >= Levels.DivineBenison && !TargetHasEffectAny(Buffs.DivineBenison) && HasCharges(DivineBenison) //Can I use Divine Benison
                         && (GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29)) //Did I just use Divine Benison
                        return actionID;
                    if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra)
                        && IsOffCooldown(Tetragrammaton) && level >= Levels.Tetragrammaton && GetTargetHPPercent() <= tetraHP)
                        return actionID;
                    else if (IsEnabled(CustomComboPreset.WHM_Cure2_Misery) && gauge.BloodLily == 3)
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
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Raise;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == All.Swiftcast)
                    {
                        var thinairCD = GetCooldown(ThinAir);
                        var hasThinAirBuff = HasEffect(Buffs.ThinAir);

                        if (IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && thinairCD.RemainingCharges > 0 && HasEffect(All.Buffs.Swiftcast) && !hasThinAirBuff && level >= Levels.ThinAir)
                            return ThinAir;
                        if (HasEffect(All.Buffs.Swiftcast))
                            return Raise;
                    }

                    return actionID;
                }
            }

            internal class WHMCDsonMainComboGroup : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_ST_MainCombo;
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
                        var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_ST_Lucid);
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
                        if (IsNotEnabled(CustomComboPreset.WHM_ST_MainCombo_NoSwiftOpener) || level < Levels.Glare3) glare3Count = 3;

                        if (CanSpellWeave(actionID) && glare3Count >= 3)
                        {
                            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind) && level >= Levels.PresenceOfMind && IsOffCooldown(PresenceOfMind))
                                return PresenceOfMind;
                            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize) && level >= Levels.Assize && IsOffCooldown(Assize))
                                return Assize;
                            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                                return All.LucidDreaming;
                        }

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && inCombat)
                        {
                            if (level is >= Levels.Aero1 and < Levels.Aero2)
                            {
                                if ((aero1Debuff is null) || (aero1Debuff.RemainingTime <= 3))
                                    return Aero1;
                            }

                            if (level is >= Levels.Aero2 and < Levels.Dia)
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

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_LilyOvercap) && level >= Levels.AfflatusRapture && ((gauge.Lily == 3) || (gauge.Lily == 2 && gauge.LilyTimer >= 17000)))
                            return AfflatusRapture;

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Misery_oGCD) && level >= Levels.AfflatusMisery && gauge.BloodLily >= 3 && glare3Count >= 3)
                            return AfflatusMisery;
                    }

                    return actionID;
                }
            }

            internal class WHMMedicaFeature : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Medica;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID == Medica2)
                    {
                        var gauge = GetJobGauge<WHMGauge>();
                        var medica2Buff = GetBuffRemainingTime(Buffs.Medica2);
                        if (level < Levels.Medica2)
                            return Medica1;
                        if (IsEnabled(CustomComboPreset.WHM_Medica_Misery) && gauge.BloodLily == 3)
                            return AfflatusMisery;
                        if (IsEnabled(CustomComboPreset.WHM_Medica_Rapture) && level >= Levels.AfflatusRapture && gauge.Lily > 0)
                            return AfflatusRapture;
                        if (HasEffect(Buffs.Medica2) && medica2Buff > 2)
                            return Medica1;
                    }

                    return actionID;
                }
            }

        }

        internal class WHM_Afflatus_oGCDHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Afflatus_oGCDHeals;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);

                if (actionID == Cure2)
                {
                    if (level >= Levels.DivineBenison && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison)
                        && (GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29))
                    {
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_BenisonWeave) && CanSpellWeave(actionID)) { return DivineBenison; }
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison)) { return DivineBenison; }
                    }
                    if (level >= Levels.Tetragrammaton && IsOffCooldown(Tetragrammaton) && GetTargetHPPercent() <= tetraHP)
                    {
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra) && CanSpellWeave(actionID)) { return Tetragrammaton; }
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra)) { return Tetragrammaton; }
                    }
                }

                return actionID;
            }
        }

        internal class WHM_AoE_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_AoE_DPS;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Holy or Holy3)
                {
                    var lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_AoE_Lucid);
                    var gauge = GetJobGauge<WHMGauge>();

                    if (WasLastAction(OriginalHook(Holy)) && IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) && IsOffCooldown(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold && level >= All.Levels.LucidDreaming)
                        return All.LucidDreaming;

                    if (WasLastAction(OriginalHook(Holy)) && IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) && level >= Levels.Assize && IsOffCooldown(Assize))
                        return Assize;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_LilyOvercap) && level >= Levels.AfflatusRapture && ((gauge.Lily == 3 && gauge.BloodLily < 3) || (gauge.Lily == 2 && gauge.LilyTimer >= 17000)))
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Misery) && level >= Levels.AfflatusMisery && gauge.BloodLily >= 3 && CurrentTarget is Dalamud.Game.ClientState.Objects.Types.BattleNpc)
                        return AfflatusMisery;
                }

                return actionID;
            }
        }

    }
}
