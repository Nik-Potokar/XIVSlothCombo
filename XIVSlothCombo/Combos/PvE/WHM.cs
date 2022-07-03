using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
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
                if (actionID is AfflatusSolace)
                {
                    if (GetJobGauge<WHMGauge>().BloodLily == 3)
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
                if (actionID is AfflatusRapture)
                {
                    if (GetJobGauge<WHMGauge>().BloodLily == 3)
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
                if (actionID is Cure2)
                {
                    if (!LevelChecked(Cure2))
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
                #region Types
                // General
                WHMGauge? gauge = GetJobGauge<WHMGauge>();
                bool hasLily = gauge.Lily > 0;
                // Divine Benison
                bool benisonPrioFeatureEnabled = IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison);
                bool benisonReady = LevelChecked(DivineBenison) && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison);
                bool benisonJustUsed = GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29;
                // Tetragrammaton
                bool tetraPrioFeatureEnabled = IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra);
                bool tetraReady = LevelChecked(Tetragrammaton) && IsOffCooldown(Tetragrammaton);
                int  tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);
                #endregion

                if (actionID is Cure2)
                {
                    if (benisonPrioFeatureEnabled && benisonReady && benisonJustUsed)
                        return actionID;

                    if (tetraPrioFeatureEnabled && tetraReady && GetTargetHPPercent() <= tetraHP)
                        return actionID;

                    else if (IsEnabled(CustomComboPreset.WHM_Cure2_Misery) && gauge.BloodLily == 3)
                        return AfflatusMisery;

                    if (LevelChecked(AfflatusSolace) && hasLily)
                        return AfflatusSolace;
                    return actionID;
                }

                if (actionID is Medica)
                {
                    if (LevelChecked(AfflatusRapture) && hasLily)
                        return AfflatusRapture;
                    return actionID;
                }
                return actionID;
            }
        }

        internal class WHM_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Raise;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast)
                {
                    bool thinAirReady = !HasEffect(Buffs.ThinAir) && LevelChecked(ThinAir) && GetCooldown(ThinAir).RemainingCharges > 0;

                    if (HasEffect(All.Buffs.Swiftcast))
                    {
                        if (IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && thinAirReady)
                            return ThinAir;
                        return Raise;
                    }
                }
                return actionID;
            }
        }

        internal class WHM_ST_MainCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_ST_MainCombo;
            internal static uint glare3Count = 0;
            internal static bool usedGlare3 = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Glare3 or Glare1 or Stone1 or Stone2 or Stone3 or Stone4)
                {
                    #region Types
                    // General
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    bool inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    bool openerDelayComplete = glare3Count >= 3;
                    // Debuffs
                    Status? diaDebuff = FindTargetEffect(Debuffs.Dia);
                    Status? aero1Debuff = FindTargetEffect(Debuffs.Aero);
                    Status? aero2Debuff = FindTargetEffect(Debuffs.Aero2);
                    // oGCDs
                    int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_ST_Lucid);
                    bool lucidReady = IsOffCooldown(All.LucidDreaming) && LevelChecked(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold;
                    bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                    bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                    // Lilies
                    bool liliesFull = gauge.Lily == 3;
                    bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;
                    #endregion

                    // No-Swift Opener
                    // Counter reset
                    if (!inCombat) glare3Count = 0;
                    
                    // Check Glare3 use
                    if (inCombat && usedGlare3 == false && lastComboMove == Glare3 && GetCooldownRemainingTime(Glare3) > 1)
                    {
                        usedGlare3 = true;  // Registers that Glare3 was used and blocks further incrementation of glare3Count
                        glare3Count++;      // Increments Glare3 counter
                    }

                    // Check Glare3 use reset
                    if (usedGlare3 == true && GetCooldownRemainingTime(Glare3) < 1) usedGlare3 = false; // Resets block to allow "Check Glare3 use"

                    // Bypass counter when disabled
                    if (IsNotEnabled(CustomComboPreset.WHM_ST_MainCombo_NoSwiftOpener) || !LevelChecked(Glare3)) glare3Count = 3;

                    if (CanSpellWeave(actionID) && openerDelayComplete)
                    {
                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind) && pomReady)
                            return PresenceOfMind;

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize) && assizeReady)
                            return Assize;

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid) && lucidReady)
                            return All.LucidDreaming;
                    }

                    // DoTs
                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && inCombat)
                    {
                        if (LevelChecked(Aero1) && !LevelChecked(Aero2))
                        {
                            if ((aero1Debuff is null) || (aero1Debuff.RemainingTime <= 3))
                                return Aero1;
                        }

                        if (LevelChecked(Aero2) && !LevelChecked(Dia))
                        {
                            if ((aero2Debuff is null) || (aero2Debuff.RemainingTime <= 3))
                                return Aero2;
                        }

                        if (LevelChecked(Dia))
                        {
                            if ((diaDebuff is null) || (diaDebuff.RemainingTime <= 3))
                                return Dia;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_LilyOvercap) && LevelChecked(AfflatusRapture) &&
                        (liliesFull || liliesNearlyFull))
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Misery_oGCD) && LevelChecked(AfflatusMisery) &&
                        gauge.BloodLily >= 3 && openerDelayComplete)
                        return AfflatusMisery;
                }
                return actionID;
            }
        }

        internal class WHM_Medica : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Medica;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Medica2)
                {
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) && GetRemainingCharges(ThinAir) > 0;

                    if (!LevelChecked(Medica2))
                        return Medica1;

                    if (IsEnabled(CustomComboPreset.WHM_Medica_Misery) && gauge.BloodLily == 3)
                        return AfflatusMisery;

                    if (IsEnabled(CustomComboPreset.WHM_Medica_Rapture) && LevelChecked(AfflatusRapture) && gauge.Lily > 0)
                        return AfflatusRapture;

                    if (HasEffect(Buffs.Medica2) && GetBuffRemainingTime(Buffs.Medica2) > 2)
                        return Medica1;

                    if (IsEnabled(CustomComboPreset.WHM_Medica_ThinAir) && thinAirReady)
                        return ThinAir;
                }
                return actionID;
            }
        }

        internal class WHM_Afflatus_oGCDHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Afflatus_oGCDHeals;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Types
                bool canWeave = CanSpellWeave(actionID);
                int tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);
                bool benisonReady = LevelChecked(DivineBenison) && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison);
                bool tetraReady = LevelChecked(Tetragrammaton) && IsOffCooldown(Tetragrammaton);
                #endregion

                if (actionID is Cure2)
                {
                    if (benisonReady && (GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29))
                    {
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_BenisonWeave) && canWeave)
                            return DivineBenison;

                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison))
                            return DivineBenison;
                    }

                    if (tetraReady && GetTargetHPPercent() <= tetraHP)
                    {
                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_TetraWeave) && canWeave)
                            return Tetragrammaton;

                        if (IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra))
                            return Tetragrammaton;
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
                    #region Types
                    // General
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    // oGCDs
                    int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_AoE_Lucid);
                    bool lucidReady = IsOffCooldown(All.LucidDreaming) && LevelChecked(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold;
                    bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                    bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                    // Lilies
                    bool liliesFullNoBlood = gauge.Lily == 3 && gauge.BloodLily < 3;
                    bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;
                    #endregion

                    if (CanSpellWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) && pomReady)
                            return PresenceOfMind;

                        if (WasLastAction(OriginalHook(Holy)) && IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) && assizeReady)
                            return Assize;

                        if (WasLastAction(OriginalHook(Holy)) && IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) && lucidReady)
                            return All.LucidDreaming;
                    }

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_LilyOvercap) && LevelChecked(AfflatusRapture) && (liliesFullNoBlood || liliesNearlyFull))
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Misery) && LevelChecked(AfflatusMisery) &&
                        gauge.BloodLily >= 3 && HasBattleTarget())
                        return AfflatusMisery;
                }
                return actionID;
            }
        }
    }
}
