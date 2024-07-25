using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class WHM
    {
        public const byte ClassID = 6;
        public const byte JobID = 24;

        public const uint
            // Heals
            Cure = 120,
            Cure2 = 135,
            Cure3 = 131,
            Regen = 137,
            AfflatusSolace = 16531,
            AfflatusRapture = 16534,
            Raise = 125,
            Benediction = 140,
            AfflatusMisery = 16535,
            Medica1 = 124,
            Medica2 = 133,
            Medica3 = 37010,
            Tetragrammaton = 3570,
            DivineBenison = 7432,
            Aquaveil = 25861,
            DivineCaress = 37011,
            // DPS
            Glare1 = 16533,
            Glare3 = 25859,
            Glare4 = 37009,
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
            PresenceOfMind = 136,
            PlenaryIndulgence = 7433;

        //Action Groups
        internal static readonly List<uint>
            StoneGlareList = [Stone1, Stone2, Stone3, Stone4, Glare1, Glare3];

        public static class Buffs
        {
            public const ushort
            Regen = 158,
            Medica2 = 150,
            Medica3 = 3880,
            PresenceOfMind = 157,
            ThinAir = 1217,
            DivineBenison = 1218,
            Aquaveil = 2708,
            SacredSight = 3879,
            DivineGrace = 3881;
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
                WHM_STDPS_Lucid = new("WHMLucidDreamingFeature"),
                WHM_STDPS_MainCombo_DoT = new("WHM_ST_MainCombo_DoT"),
                WHM_AoEDPS_Lucid = new("WHM_AoE_Lucid"),
                WHM_STHeals_Lucid = new("WHM_STHeals_Lucid"),
                WHM_STHeals_ThinAir = new("WHM_STHeals_ThinAir"),
                WHM_STHeals_Esuna = new("WHM_Cure2_Esuna"),
                WHM_STHeals_BenedictionHP = new("WHM_STHeals_BenedictionHP"),
                WHM_STHeals_TetraHP = new("WHM_STHeals_TetraHP"),
                WHM_STHeals_BenisonHP = new("WHM_STHeals_BenisonHP"),
                WHM_STHeals_AquaveilHP = new("WHM_STHeals_AquaveilHP"),
                WHM_AoEHeals_Lucid = new("WHM_AoEHeals_Lucid"),
                WHM_AoEHeals_ThinAir = new("WHM_AoE_ThinAir"),
                WHM_AoEHeals_Cure3MP = new("WHM_AoE_Cure3MP");
            internal static UserBool
                WHM_ST_MainCombo_DoT_Adv = new("WHM_ST_MainCombo_DoT_Adv"),
                WHM_ST_MainCombo_Adv = new("WHM_ST_MainCombo_Adv"),
                WHM_ST_MainCombo_Opener_Swiftcast = new("WHM_ST_Opener_Swiftcast"),
                WHM_STHeals_UIMouseOver = new("WHM_STHeals_UIMouseOver"),
                WHM_STHeals_BenedictionWeave = new("WHM_STHeals_BenedictionWeave"),
                WHM_STHeals_TetraWeave = new("WHM_STHeals_TetraWeave"),
                WHM_STHeals_BenisonWeave = new("WHM_STHeals_BenisonWeave"),
                WHM_STHeals_AquaveilWeave = new("WHM_STHeals_AquaveilWeave"),
                WHM_AoEHeals_PlenaryWeave = new("WHM_AoEHeals_PlenaryWeave"),
                WHM_AoEHeals_AssizeWeave = new("WHM_AoEHeals_AssizeWeave"),
                WHM_AoEHeals_MedicaMO = new("WHM_AoEHeals_MedicaMO");
            internal static UserFloat
                WHM_ST_MainCombo_DoT_Threshold = new("WHM_ST_MainCombo_DoT_Threshold"),
                WHM_STHeals_RegenTimer = new("WHM_STHeals_RegenTimer"),
                WHM_AoEHeals_MedicaTime = new("WHM_AoEHeals_MedicaTime");
            public static UserBoolArray
                WHM_ST_MainCombo_Adv_Actions = new("WHM_ST_MainCombo_Adv_Actions");
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
            internal static int Glare3Count => ActionWatching.CombatActions.Count(x => x == OriginalHook(Glare3));
            internal static int DiaCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(Dia));

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool ActionFound;

                if (Config.WHM_ST_MainCombo_Adv && Config.WHM_ST_MainCombo_Adv_Actions.Count > 0)
                {
                    bool onStones = Config.WHM_ST_MainCombo_Adv_Actions[0] && StoneGlareList.Contains(actionID);
                    bool onAeros = Config.WHM_ST_MainCombo_Adv_Actions[1] && AeroList.ContainsKey(actionID);
                    bool onStone2 = Config.WHM_ST_MainCombo_Adv_Actions[2] && actionID is Stone2;
                    ActionFound = onStones || onAeros || onStone2;
                }
                else ActionFound = StoneGlareList.Contains(actionID); //default handling

                if (ActionFound)
                {
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    bool inOpener = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Opener)
                                    && Glare3Count < 4
                                    && !HasEffect(Buffs.SacredSight);
                    bool liliesFull = gauge.Lily == 3;
                    bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;

                    if (inOpener)
                    {
                        if (Glare3Count == 0)
                            return OriginalHook(Glare3);

                        if (DiaCount == 0)
                            return OriginalHook(Dia);

                        if (Glare3Count == 3 && CanWeave(actionID))
                        {
                            if (ActionReady(All.Swiftcast) && Config.WHM_ST_MainCombo_Opener_Swiftcast)
                                return OriginalHook(All.Swiftcast);

                            if (ActionReady(PresenceOfMind))
                                return PresenceOfMind;
                        }

                        if (Glare3Count == 4)
                        {
                            if (ActionReady(PresenceOfMind) && Config.WHM_ST_MainCombo_Opener_Swiftcast)
                                return OriginalHook(PresenceOfMind);

                            if (ActionReady(Assize))
                            return Assize;
                        }

                        if (Glare3Count > 0)
                            return OriginalHook(Glare3);
                    }

                    if (CanSpellWeave(actionID))
                    {
                        bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                        bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                        bool pomEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind);
                        bool assizeEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize);

                        if (Variant.CanRampart(CustomComboPreset.WHM_DPS_Variant_Rampart, actionID, true))
                            return Variant.VariantRampart;

                        if (pomEnabled && pomReady)
                            return PresenceOfMind;
                        if (assizeEnabled && assizeReady)
                            return Assize;
                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid) && 
                            All.CanUseLucid(actionID, Config.WHM_STDPS_Lucid))
                            return All.LucidDreaming;
                    }

                    if (InCombat())
                    {
                        // DoTs
                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && LevelChecked(Aero) && HasBattleTarget())
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
                                GetTargetHPPercent() > Config.WHM_STDPS_MainCombo_DoT)
                                return OriginalHook(Aero);
                        }

                        // Glare IV
                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_GlareIV)
                            && HasEffect(Buffs.SacredSight)
                            && GetBuffStacks(Buffs.SacredSight) > 0)
                            return OriginalHook(Glare4);

                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_LilyOvercap) && LevelChecked(AfflatusRapture) &&
                            (liliesFull || liliesNearlyFull))
                            return AfflatusRapture;
                        if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Misery_oGCD) && LevelChecked(AfflatusMisery) &&
                            gauge.BloodLily >= 3)
                            return AfflatusMisery;

                        return OriginalHook(Stone1);
                    }
                }

                return actionID;
            }
        }

        internal class WHM_AoEHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_AoEHeals;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Medica1)
                {
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) && GetRemainingCharges(ThinAir) > Config.WHM_AoEHeals_ThinAir;
                    var canWeave = CanSpellWeave(actionID, 0.3);
                    bool plenaryReady = ActionReady(PlenaryIndulgence) && (!Config.WHM_AoEHeals_PlenaryWeave || (Config.WHM_AoEHeals_PlenaryWeave && canWeave));
                    bool divineCaressReady = ActionReady(DivineCaress) && HasEffect(Buffs.DivineGrace);
                    bool assizeReady = ActionReady(Assize) && (!Config.WHM_AoEHeals_AssizeWeave || (Config.WHM_AoEHeals_AssizeWeave && canWeave));
                    var healTarget = GetHealTarget(Config.WHM_AoEHeals_MedicaMO);

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Assize) && assizeReady)
                        return Assize;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Plenary) && plenaryReady)
                        return PlenaryIndulgence;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_DivineCaress) && divineCaressReady)
                        return OriginalHook(DivineCaress);

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Lucid) && All.CanUseLucid(actionID, Config.WHM_AoEHeals_Lucid, true, 0.3))
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Misery) && gauge.BloodLily == 3)
                        return AfflatusMisery;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Rapture) && LevelChecked(AfflatusRapture) && gauge.Lily > 0)
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_ThinAir) && thinAirReady)
                        return ThinAir;

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Medica2)
                        && ((FindEffectOnMember(Buffs.Medica2, healTarget) == null && FindEffectOnMember(Buffs.Medica3, healTarget) == null)
                            || FindEffectOnMember(Buffs.Medica2, healTarget).RemainingTime <= Config.WHM_AoEHeals_MedicaTime
                            || FindEffectOnMember(Buffs.Medica3, healTarget).RemainingTime <= Config.WHM_AoEHeals_MedicaTime)
                        && (ActionReady(Medica2) || ActionReady(Medica3)))
                    {
                        // Medica 3 upgrade
                        if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Medica3)
                            && LevelChecked(Medica3))
                            return Medica3;

                        return Medica2;
                    }

                    if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Cure3) && ActionReady(Cure3) && (LocalPlayer.CurrentMp >= Config.WHM_AoEHeals_Cure3MP || HasEffect(Buffs.ThinAir)))
                        return Cure3;

                }

                return actionID;
            }
        }

        internal class WHM_ST_Heals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_STHeals;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cure)
                {
                    WHMGauge? gauge = GetJobGauge<WHMGauge>();
                    IGameObject? healTarget = GetHealTarget(Config.WHM_STHeals_UIMouseOver);
                    bool canWeave = CanSpellWeave(actionID, 0.3);
                    bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) && GetRemainingCharges(ThinAir) > Config.WHM_STHeals_ThinAir;
                    bool tetraReady = ActionReady(Tetragrammaton) && (!Config.WHM_STHeals_TetraWeave || (Config.WHM_STHeals_TetraWeave && canWeave)) && GetTargetHPPercent(healTarget) <= Config.WHM_STHeals_TetraHP;
                    bool benisonReady = ActionReady(DivineBenison) && (!Config.WHM_STHeals_BenisonWeave || (Config.WHM_STHeals_BenisonWeave && canWeave)) && GetTargetHPPercent(healTarget) <= Config.WHM_STHeals_BenisonHP;
                    bool aquaReady = ActionReady(Aquaveil) && (!Config.WHM_STHeals_AquaveilWeave || (Config.WHM_STHeals_AquaveilWeave && canWeave)) && GetTargetHPPercent(healTarget) <= Config.WHM_STHeals_AquaveilHP;
                    bool benedictionReady = ActionReady(Benediction) && (!Config.WHM_STHeals_BenedictionWeave || (Config.WHM_STHeals_BenedictionWeave && canWeave)) && GetTargetHPPercent(healTarget) <= Config.WHM_STHeals_BenedictionHP;
                    bool regenReady = ActionReady(Regen) && (FindEffectOnMember(Buffs.Regen, healTarget) is null || FindEffectOnMember(Buffs.Regen, healTarget)?.RemainingTime <= Config.WHM_STHeals_RegenTimer);

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Benediction) && benedictionReady)
                        return Benediction;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Esuna) && ActionReady(All.Esuna) &&
                        GetTargetHPPercent(healTarget) >= Config.WHM_STHeals_Esuna &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Tetragrammaton) && tetraReady)
                        return Tetragrammaton;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Lucid) && All.CanUseLucid(actionID, Config.WHM_STHeals_Lucid))
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Benison) && benisonReady && FindEffectOnMember(Buffs.DivineBenison, healTarget) is null)
                        return DivineBenison;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Aquaveil) && aquaReady && FindEffectOnMember(Buffs.Aquaveil, healTarget) is null)
                        return Aquaveil;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Regen) && regenReady)
                        return Regen;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_Solace) && gauge.Lily > 0 && ActionReady(AfflatusSolace))
                        return AfflatusSolace;

                    if (IsEnabled(CustomComboPreset.WHM_STHeals_ThinAir) && thinAirReady)
                        return ThinAir;

                    if (ActionReady(Cure2))
                        return Cure2;

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

                    if (Variant.CanRampart(CustomComboPreset.WHM_DPS_Variant_Rampart, actionID, true))
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
                            LocalPlayer.CurrentMp <= Config.WHM_AoEDPS_Lucid)
                            return All.LucidDreaming;
                    }

                    // Glare IV
                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_GlareIV)
                        && HasEffect(Buffs.SacredSight)
                        && GetBuffStacks(Buffs.SacredSight) > 0)
                        return OriginalHook(Glare4);

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
