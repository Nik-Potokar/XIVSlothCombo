using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

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
            Aero = 121,
            Aero2 = 132,
            Dia = 16532,
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
            Aero = 143,
            Aero2 = 144,
            Dia = 1871;
        }

        //Debuff Pairs of Actions and Debuff
        internal static readonly Dictionary<uint, ushort>
            AeroList = new() {
                { Aero, Debuffs.Aero },
                { Aero2, Debuffs.Aero2 },
                { Dia, Debuffs.Dia }
            };

        public static class Config
        {
            internal static UserInt
                WHM_ST_Lucid = new("WHMLucidDreamingFeature"),
                WHM_ST_MainCombo_DoT = new("WHM_ST_MainCombo_DoT"),
                WHM_AoE_Lucid = new("WHM_AoE_Lucid"),
                WHM_oGCDHeals = new("WHMogcdHealsShieldsFeature"),
                WHM_Medica_ThinAir = new("WHM_Medica_ThinAir");
            internal static UserBool
                WHM_ST_MainCombo_DoT_Adv = new("WHM_ST_MainCombo_DoT_Adv"),
                WHM_Afflatus_Adv = new("WHM_Afflatus_Adv"),
                WHM_Afflatus_UIMouseOver = new("WHM_Afflatus_UIMouseOver");
            internal static UserFloat
                WHM_ST_MainCombo_DoT_Threshold = new("WHM_ST_MainCombo_DoT_Threshold");
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
                    bool benisonReady = ActionReady(DivineBenison) && !TargetHasEffectAny(Buffs.DivineBenison);
                    bool benisonJustUsed = GetRemainingCharges(DivineBenison) == 2 || GetRemainingCharges(DivineBenison) <= 29;
                    bool tetraPrioFeatureEnabled = IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Prio) && IsEnabled(CustomComboPreset.WHM_Afflatus_oGCDHeals_Tetra);
                    int tetraHP = Config.WHM_oGCDHeals;

                    //Grab our target (Soft->Hard->Self)
                    GameObject? healTarget = GetHealTarget(Config.WHM_Afflatus_Adv && Config.WHM_Afflatus_UIMouseOver);

                    if (IsEnabled(CustomComboPreset.WHM_Cure2_Esuna) && ActionReady(All.Esuna) &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;

                    // Are these first two statements supposed to return 'actionID'?
                    // Seems like a weird condition set to return Cure II. -k
                    if (benisonPrioFeatureEnabled && benisonReady && benisonJustUsed)
                        return actionID;
                    if (tetraPrioFeatureEnabled && ActionReady(Tetragrammaton) && GetTargetHPPercent(healTarget) <= tetraHP)
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
                        bool lucidReady = IsOffCooldown(All.LucidDreaming) && LevelChecked(All.LucidDreaming) && LocalPlayer.CurrentMp <= Config.WHM_ST_Lucid;
                        bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                        bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                        bool pomEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind);
                        bool assizeEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize);
                        bool lucidEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid);


                        if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantRampart;

                        if (pomEnabled && pomReady)
                            return PresenceOfMind;
                        if (assizeEnabled && assizeReady)
                            return Assize;
                        if (lucidEnabled && lucidReady)
                            return All.LucidDreaming;
                    }

                    // DoTs
                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && InCombat() && LevelChecked(Aero) && HasBattleTarget())
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantSpiritDart;

                        uint dot = OriginalHook(Aero); //Grab the appropriate DoT Action
                        Status? dotDebuff = FindTargetEffect(AeroList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                        // DoT Uptime & HP% threshold
                        float refreshtimer = Config.WHM_ST_MainCombo_DoT_Adv ? Config.WHM_ST_MainCombo_DoT_Threshold : 3;
                        if ((dotDebuff is null || dotDebuff.RemainingTime <= refreshtimer) &&
                            GetTargetHPPercent() > Config.WHM_ST_MainCombo_DoT)
                            return OriginalHook(Aero);
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
                    bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) && GetRemainingCharges(ThinAir) > Config.WHM_Medica_ThinAir;

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
                    int tetraHP = Config.WHM_oGCDHeals;
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

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) && ActionReady(Assize))
                        return Assize;

                    if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart))
                        return Variant.VariantRampart;

                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                        HasBattleTarget())
                        return Variant.VariantSpiritDart;

                    if (CanSpellWeave(actionID) || IsMoving)
                    {
                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) && ActionReady(PresenceOfMind))
                            return PresenceOfMind;
                        if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) && ActionReady(All.LucidDreaming) &&
                            LocalPlayer.CurrentMp <= Config.WHM_AoE_Lucid)
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
