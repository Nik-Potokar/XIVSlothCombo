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
            Aero1 = 143,
            Aero2 = 144;
        }

        public static class Config
        {
            public const string
                WHM_ST_Lucid = "WHMLucidDreamingFeature",
                WHM_ST_MainCombo_DoT = "WHM_ST_MainCombo_DoT",
                WHM_AoE_Lucid = "WHM_AoE_Lucid",
                WHM_oGCDHeals = "WHMogcdHealsShieldsFeature";
        }

        internal class WHM_SolaceMisery : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_SolaceMisery;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                byte BloodLilies = GetJobGauge<WHMGauge>().BloodLily;

                return actionID is AfflatusSolace && BloodLilies == 3
                    ? AfflatusMisery
                    : actionID;
            }
        }

        internal class WHM_RaptureMisery : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_RaptureMisery;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                byte BloodLilies = GetJobGauge<WHMGauge>().BloodLily;

                return actionID is AfflatusRapture && BloodLilies == 3
                    ? AfflatusMisery
                    : actionID;
            }
        }

        internal class WHM_CureSync : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_CureSync;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is Cure2 && !LevelChecked(Cure2)
                    ? Cure
                    : actionID;
            }
        }

        internal class WHM_Afflatus : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Afflatus;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool hasLily = GetJobGauge<WHMGauge>().Lily > 0;
                byte BloodLilies = GetJobGauge<WHMGauge>().BloodLily;

                if (actionID is Cure2)
                {
                    bool benisonPrioFeatureEnabled = IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison);
                    bool benisonReady = LevelChecked(DivineBenison) && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison);
                    bool benisonJustUsed = GetCooldown(DivineBenison).RemainingCharges == 2 || GetCooldown(DivineBenison).ChargeCooldownRemaining <= 29;
                    bool tetraPrioFeatureEnabled = IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra);
                    bool tetraReady = LevelChecked(Tetragrammaton) && IsOffCooldown(Tetragrammaton);
                    int tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);

                    // Are these first two statements supposed to return 'actionID'?
                    // Seems like a weird condition set to return Cure II. -k
                    if (benisonPrioFeatureEnabled && benisonReady && benisonJustUsed)
                        return actionID;
                    if (tetraPrioFeatureEnabled && tetraReady && GetTargetHPPercent() <= tetraHP)
                        return actionID;
                    else if (IsEnabled(CustomComboPreset.WHM_Cure2_Misery) && BloodLilies == 3)
                        return AfflatusMisery;
                    return LevelChecked(AfflatusSolace) && hasLily
                        ? AfflatusSolace
                        : actionID;
                }

                if (actionID is Medica)
                    return LevelChecked(AfflatusRapture) && hasLily
                        ? AfflatusRapture
                        : actionID;

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
                    bool thinAirReady = !HasEffect(Buffs.ThinAir) && LevelChecked(ThinAir) && HasCharges(ThinAir);

                    if (HasEffect(All.Buffs.Swiftcast))
                        return IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && thinAirReady
                            ? ThinAir
                            : Raise;
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
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    bool openerDelayComplete = glare3Count >= 3;
                    int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_ST_Lucid);
                    bool liliesFull = gauge.Lily == 3;
                    bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;
                    float glare3CD = GetCooldownRemainingTime(Glare3);

                    // No-Swift Opener
                    // Counter reset
                    if (!InCombat()) glare3Count = 0;

                    // Check Glare3 use
                    if (InCombat() && usedGlare3 == false && lastComboMove == Glare3 && glare3CD > 1)
                    {
                        usedGlare3 = true;  // Registers that Glare3 was used and blocks further incrementation of glare3Count
                        glare3Count++;      // Increments Glare3 counter
                    }

                    // Check Glare3 use reset
                    if (usedGlare3 == true && glare3CD < 1) usedGlare3 = false; // Resets block to allow "Check Glare3 use"

                    // Bypass counter when disabled
                    if (IsNotEnabled(CustomComboPreset.WHM_ST_MainCombo_NoSwiftOpener) || !LevelChecked(Glare3)) glare3Count = 3;

                    if (CanSpellWeave(actionID) && openerDelayComplete)
                    {
                        bool lucidReady = IsOffCooldown(All.LucidDreaming) && LevelChecked(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold;
                        bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                        bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                        bool pomEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind);
                        bool assizeEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize);
                        bool lucidEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid);

                        if (pomEnabled && pomReady)
                            return PresenceOfMind;
                        if (assizeEnabled && assizeReady)
                            return Assize;
                        if (lucidEnabled && lucidReady)
                            return All.LucidDreaming;
                    }

                    // DoTs
                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && InCombat() && LevelChecked(Aero1))
                    {
                        // Fetch appropriate debuff for player level
                        Status? DoTDebuff;
                        if (LevelChecked(Dia)) DoTDebuff = FindTargetEffect(Debuffs.Dia);
                        else if (LevelChecked(Aero2)) DoTDebuff = FindTargetEffect(Debuffs.Aero2);
                        else DoTDebuff = FindTargetEffect(Debuffs.Aero1);

                        // DoT Uptime & HP% threshold
                        if (((DoTDebuff is null) || (DoTDebuff.RemainingTime <= 3)) &&
                            (GetTargetHPPercent() > GetOptionValue(Config.WHM_ST_MainCombo_DoT)))
                            return OriginalHook(Aero1);
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
                if (actionID is Cure2)
                {
                    bool canWeave = CanSpellWeave(actionID);
                    int tetraHP = PluginConfiguration.GetCustomIntValue(Config.WHM_oGCDHeals);
                    bool benisonReady = LevelChecked(DivineBenison) && HasCharges(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison);
                    bool tetraReady = LevelChecked(Tetragrammaton) && IsOffCooldown(Tetragrammaton);

                    if (benisonReady && (GetRemainingCharges(DivineBenison) == 2 || GetCooldownRemainingTime(DivineBenison) <= 29) &&
                        ((IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_BenisonWeave) && canWeave) ||
                        IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Benison)))
                        return DivineBenison;
                    if (tetraReady && GetTargetHPPercent() <= tetraHP &&
                        ((IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_TetraWeave) && canWeave) ||
                        IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra)))
                        return Tetragrammaton;
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
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();

                    bool liliesFullNoBlood = gauge.Lily == 3 && gauge.BloodLily < 3;
                    bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;

                    if (CanSpellWeave(actionID))
                    {
                        bool holyLast = WasLastAction(OriginalHook(Holy));
                        int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.WHM_AoE_Lucid);
                        bool lucidReady = IsOffCooldown(All.LucidDreaming) && LevelChecked(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold;
                        bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                        bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);

                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) && pomReady)
                            return PresenceOfMind;
                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) && holyLast && assizeReady)
                            return Assize;
                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) && holyLast && lucidReady)
                            return All.LucidDreaming;
                    }

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_LilyOvercap) && LevelChecked(AfflatusRapture) &&
                        (liliesFullNoBlood || liliesNearlyFull))
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
