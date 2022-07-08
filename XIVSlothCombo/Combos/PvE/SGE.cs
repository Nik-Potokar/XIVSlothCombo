using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public static SGEGauge Gauge = CustomComboNS.Functions.CustomComboFunctions.GetJobGauge<SGEGauge>();

        public const uint
            // Heals and Shields
            Diagnosis = 24284,
            Prognosis = 24286,
            Physis = 24288,
            Druochole = 24296,
            Kerachole = 24298,
            Ixochole = 24299,
            Pepsis = 24301,
            Physis2 = 24302,
            Taurochole = 24303,
            Haima = 24305,
            Panhaima = 24311,
            Holos = 24310,
            EukrasianDiagnosis = 24291,
            EukrasianPrognosis = 24292,
            Egeiro = 24287,

            // DPS
            Dosis = 24283,
            Dosis2 = 24306,
            Dosis3 = 24312,
            EukrasianDosis = 24293,
            EukrasianDosis2 = 24308,
            EukrasianDosis3 = 24314,
            Phlegma = 24289,
            Phlegma2 = 24307,
            Phlegma3 = 24313,
            Dyskrasia = 24297,
            Dyskrasia2 = 24315,
            Toxikon = 24304,
            Pneuma = 24318,

            // Buffs
            Soteria = 24294,
            Zoe = 24300,
            Krasis = 24317,

            // Other
            Kardia = 24285,
            Eukrasia = 24290,
            Rhizomata = 24309;

        // Action Groups
        public static readonly List<uint>
            AddersgallList = new() { Taurochole, Druochole, Ixochole, Kerachole },
            PhlegmaList = new () { Phlegma, Phlegma2, Phlegma3 };

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                EukrasianDiagnosis = 2607,
                EukrasianPrognosis = 2609,
                Kardion = 2872;
        }

        public static class Debuffs
        {
            public const ushort
                EukrasianDosis = 2614,
                EukrasianDosis2 = 2615,
                EukrasianDosis3 = 2616;
        }

        // Debuff Pairs of Actions and Debuff
        public static readonly Dictionary<uint, ushort>
            DosisList = new()
            {
                { Dosis,  Debuffs.EukrasianDosis  },
                { Dosis2, Debuffs.EukrasianDosis2 },
                { Dosis3, Debuffs.EukrasianDosis3 }
            };

        public static class Range
        {
            public const byte Phlegma = 6;
        }

        public static class Config
        {
            public const string
                // GUI Customization Storage Names
                SGE_ST_Dosis_EDosisHPPer = "SGE_ST_Dosis_EDosisHPPer",
                SGE_ST_Dosis_Lucid = "SGE_ST_Dosis_Lucid",
                SGE_ST_Dosis_Toxikon = "SGE_ST_Dosis_Toxikon",
                SGE_ST_Heal_Zoe = "SGE_ST_Heal_Zoe",
                SGE_ST_Heal_Haima = "SGE_ST_Heal_Haima",
                SGE_ST_Heal_Krasis = "SGE_ST_Heal_Krasis",
                SGE_ST_Heal_Pepsis = "SGE_ST_Heal_Pepsis",
                SGE_ST_Heal_Soteria = "SGE_ST_Heal_Soteria",
                SGE_ST_Heal_Diagnosis = "SGE_ST_Heal_Diagnosis",
                SGE_ST_Heal_Druochole = "SGE_ST_Heal_Druochole",
                SGE_ST_Heal_Taurochole = "SGE_ST_Heal_Taurochole",
                SGE_AoE_Phlegma_Lucid = "SGE_AoE_Phlegma_Lucid";
        }

        // Soteria Kardia
        // Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
        internal class SGE_Kardia : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Kardia;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is Soteria && (!HasEffect(Buffs.Kardia) || IsOnCooldown(Soteria))
                    ? Kardia
                    : actionID;
            }
        }

        /*
        Rhizomata
        Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of Addersgall stacks
        (Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
        */
        internal class SGE_Rhizo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Rhizo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return AddersgallList.Contains(actionID) && 
                    ActionReady(Rhizomata) &&
                    Gauge.Addersgall is 0
                    ? Rhizomata
                    : actionID;
            }
        }

        /*
        Druo/Tauro
        Druochole Upgrade to Taurochole (like a trait upgrade)
        Replaces Druocole with Taurochole when Taurochole is available
        (As of 6.0) Taurochole (single target massive insta heal w/ cooldown), Druochole (Single target insta heal)
        */
        internal class SGE_DruoTauro : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_DruoTauro;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is Druochole && ActionReady(Taurochole)
                    ? Taurochole
                    : actionID;
            }
        }

        // Zoe Pneuma
        // Places Zoe on top of Pneuma when both are available.
        internal class SGE_ZoePneuma : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ZoePneuma;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe)
                    ? Zoe
                    : actionID;
            }
        }

        // AoE/Phlegma Replacement
        // Replaces Zero Charges/Stacks of Phlegma with Toxikon (if you can use it) or Dyskrasia 
        internal class SGE_AoE_Phlegma : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_Phlegma;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (PhlegmaList.Contains(actionID))
                {
                    bool NoPhlegmaToxikon = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaToxikon);
                    bool OutOfRangeToxikon = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_OutOfRangeToxikon);
                    bool NoPhlegmaDyskrasia = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaDyskrasia);
                    bool NoTargetDyskrasia = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoTargetDyskrasia);
                    int lucidMPThreshold = GetOptionValue(Config.SGE_AoE_Phlegma_Lucid);

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Lucid) &&
                        ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) &&
                        LocalPlayer.CurrentMp <= lucidMPThreshold)
                        return All.LucidDreaming;

                    if ((NoPhlegmaToxikon || OutOfRangeToxikon) &&
                        LevelChecked(Toxikon) && HasBattleTarget() &&
                        Gauge.Addersting > 0)
                    {
                        if ((NoPhlegmaToxikon && !HasCharges(OriginalHook(Phlegma))) ||
                            (OutOfRangeToxikon && (GetTargetDistance() > Range.Phlegma)))
                            return OriginalHook(Toxikon);
                    }

                    if ((NoPhlegmaDyskrasia || NoTargetDyskrasia) && LevelChecked(Phlegma))
                    {
                        if ((NoPhlegmaDyskrasia && !HasCharges(OriginalHook(Phlegma))) ||
                            (NoTargetDyskrasia && CurrentTarget is null))
                            return OriginalHook(Dyskrasia);
                    }
                }

                return actionID;
            }
        }

        /*
        Single Target Dosis Combo
        Currently Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
        Lucid Dreaming, Toxikon optional
        */
        internal class SGE_ST_Dosis : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Dosis;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (DosisList.ContainsKey(actionID) && InCombat())
                {
                    int lucidMPThreshold = GetOptionValue(Config.SGE_AoE_Phlegma_Lucid);

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Lucid) &&
                        ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) &&
                        LocalPlayer.CurrentMp <= lucidMPThreshold)
                        return All.LucidDreaming;

                    if (HasBattleTarget())
                    {
                        bool toxikonMovement = GetOptionBool(Config.SGE_ST_Dosis_Toxikon);

                        // Eukrasian Dosis.
                        // If we're too low level to use Eukrasia, we can stop here.
                        if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosis) && LevelChecked(Eukrasia) && (!HasEffect(Buffs.Eukrasia)))
                        {
                            // Grab current Dosis via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                            Status? dotDebuff = FindTargetEffect(DosisList[OriginalHook(actionID)]);
                            int eDosisHPThreshold = GetOptionValue(Config.SGE_ST_Dosis_EDosisHPPer);

                            if (((dotDebuff is null) || (dotDebuff.RemainingTime <= 3)) &&
                                (GetTargetHPPercent() > eDosisHPThreshold))
                                return Eukrasia;
                        }

                        // Toxikon
                        if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Toxikon) &&
                            LevelChecked(Toxikon) && IsOffCooldown(actionID) &&     // Cooldown check against original action to stop cooldown animation seizure
                            ((!toxikonMovement && IsMoving) || toxikonMovement) &&
                            Gauge.Addersting > 0)
                            return OriginalHook(Toxikon);
                    }
                }

                return actionID;
            }
        }

        // Swiftcast combos to Egeiro (Raise) while Swiftcast is on cooldown
        internal class SGE_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast)
                    ? Egeiro
                    : actionID;
            }
        }

        internal class SGE_ST_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Diagnosis)
                {
                    // Set Target. Soft -> Hard -> Self priority, matching normal in-game behavior
                    GameObject? HealTarget;

                    if (Services.Service.TargetManager.SoftTarget?.ObjectKind is ObjectKind.Player) HealTarget = Services.Service.TargetManager.SoftTarget;
                    else HealTarget = CurrentTarget?.ObjectKind is ObjectKind.Player
                        ? CurrentTarget
                        : LocalPlayer;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && ActionReady(Druochole) &&
                        Gauge.Addersgall >= 1 &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Druochole))
                        return Druochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && ActionReady(Taurochole) &&
                        Gauge.Addersgall >= 1 &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Taurochole))
                        return Taurochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                        Gauge.Addersgall is 0)
                        return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
                        FindEffect(Buffs.Kardia) is null &&
                        FindEffect(Buffs.Kardion, HealTarget, LocalPlayer?.ObjectId) is null)
                        return Kardia;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria) && ActionReady(Soteria) &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Soteria))
                        return Soteria;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe) && ActionReady(Zoe) &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Zoe))
                        return Zoe;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis) && ActionReady(Krasis) &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Krasis))
                        return Krasis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && ActionReady(Pepsis) &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Pepsis) &&
                        FindEffect(Buffs.EukrasianDiagnosis, HealTarget, LocalPlayer?.ObjectId) is not null)
                        return Pepsis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima) && ActionReady(Haima) &&
                        GetTargetHPPercent() <= GetOptionValue(Config.SGE_ST_Heal_Haima))
                        return Haima;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Diagnosis) && LevelChecked(Eukrasia) &&
                        FindEffect(Buffs.EukrasianDiagnosis, HealTarget, LocalPlayer?.ObjectId) is null &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Diagnosis))
                    {
                        return !HasEffect(Buffs.Eukrasia)
                            ? Eukrasia
                            : EukrasianDiagnosis;
                    }
                }

                return actionID;
            }
        }

        internal class SGE_AoE_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Prognosis)
                {
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                        Gauge.Addersgall is 0)
                        return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && ActionReady(Kerachole) &&
                        Gauge.Addersgall >= 1)
                        return Kerachole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && ActionReady(Ixochole) &&
                        Gauge.Addersgall >= 1)
                        return Ixochole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis) && ActionReady(Physis))
                        return OriginalHook(Physis);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && LevelChecked(Eukrasia) &&
                        FindEffect(Buffs.EukrasianPrognosis) is null)
                    {
                        if (!HasEffect(Buffs.Eukrasia))
                            return Eukrasia;
                        if (HasEffect(Buffs.Eukrasia))
                            return EukrasianPrognosis;
                    }

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos) && ActionReady(Holos))
                        return Holos;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima) && ActionReady(Panhaima))
                        return Panhaima;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && ActionReady(Pepsis) &&
                        FindEffect(Buffs.EukrasianPrognosis) is not null)
                        return Pepsis;
                }

                return actionID;
            }
        }
    }
}