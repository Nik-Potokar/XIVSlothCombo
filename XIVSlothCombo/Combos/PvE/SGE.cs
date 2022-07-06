using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
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

            // DPS
            Dosis1 = 24283,
            Dosis2 = 24306,
            Dosis3 = 24312,
            EukrasianDosis1 = 24293,
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
            Rhizomata = 24309,

            // Role
            Egeiro = 24287;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                EukrasianDiagnosis = 2607,
                Kardion = 2872,
                EukrasianPrognosis = 2609;
        }

        public static class Debuffs
        {
            public const ushort
                EukrasianDosis1 = 2614,
                EukrasianDosis2 = 2615,
                EukrasianDosis3 = 2616;
        }

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
                return actionID is Taurochole or Druochole or Ixochole or Kerachole &&
                    LevelChecked(Rhizomata) && IsOffCooldown(actionID) &&
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
                return actionID is Druochole && LevelChecked(Taurochole) && IsOffCooldown(Taurochole)
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
                return actionID is Pneuma && LevelChecked(Pneuma) && IsOffCooldown(Pneuma) && IsOffCooldown(Zoe)
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
                if (actionID is Phlegma or Phlegma2 or Phlegma3)
                {
                    bool lucidReady = LevelChecked(All.LucidDreaming) && IsOffCooldown(All.LucidDreaming);
                    bool NoPhlegmaToxikon = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaToxikon);
                    bool OutOfRangeToxikon = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_OutOfRangeToxikon);
                    bool NoPhlegmaDyskrasia = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoPhlegmaDyskrasia);
                    bool NoTargetDyskrasia = IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_NoTargetDyskrasia);

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Phlegma_Lucid) && lucidReady &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.SGE_AoE_Phlegma_Lucid) &&
                        CanSpellWeave(actionID))
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
        Lucid Dreaming, Target of Target optional
        */
        internal class SGE_ST_Dosis : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Dosis;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Dosis1 or Dosis2 or Dosis3 && InCombat())
                {
                    bool lucidReady = LevelChecked(All.LucidDreaming) && IsOffCooldown(All.LucidDreaming);

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Lucid) && lucidReady &&
                        LocalPlayer.CurrentMp <= GetOptionValue(Config.SGE_ST_Dosis_Lucid) &&
                        CanSpellWeave(actionID))
                        return All.LucidDreaming;

                    // Eukrasian Dosis.
                    // If we're too low level to use Eukrasia, we can stop here.
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_EDosis) && LevelChecked(Eukrasia) && HasBattleTarget())
                    {
                        // If we're already Eukrasian'd, the whole point of this section is moot
                        if (HasEffect(Buffs.Eukrasia))
                            return OriginalHook(Dosis1); // OriginalHook will select the correct Dosis for us

                        // Determine which Dosis debuff to check
                        Status? DosisDebuffID;
                        if (LevelChecked(Dosis3)) DosisDebuffID = FindTargetEffect(Debuffs.EukrasianDosis3);
                        else if (LevelChecked(Dosis2)) DosisDebuffID = FindTargetEffect(Debuffs.EukrasianDosis2);
                        else DosisDebuffID = FindTargetEffect(Debuffs.EukrasianDosis1);

                        // Got our Debuff for our level, check for it and procede 
                        if (((DosisDebuffID is null) || (DosisDebuffID.RemainingTime <= 3)) &&
                            (GetTargetHPPercent() > GetOptionValue(Config.SGE_ST_Dosis_EDosisHPPer)))
                            return Eukrasia;
                    }

                    // Toxikon
                    if (IsEnabled(CustomComboPreset.SGE_ST_Dosis_Toxikon) &&
                        LevelChecked(Toxikon) && HasBattleTarget() &&
                        ((!GetOptionBool(Config.SGE_ST_Dosis_Toxikon) && IsMoving) || GetOptionBool(Config.SGE_ST_Dosis_Toxikon)) &&
                        Gauge.Addersting > 0)
                        return OriginalHook(Toxikon);
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
                    bool druocholeReady = LevelChecked(Druochole) && IsOffCooldown(Druochole);
                    bool taurocholeReady = LevelChecked(Taurochole) && IsOffCooldown(Taurochole);
                    bool rhizomataReady = LevelChecked(Rhizomata) && IsOffCooldown(Rhizomata);
                    bool soteriaReady = LevelChecked(Soteria) && IsOffCooldown(Soteria);
                    bool zoeReady = LevelChecked(Zoe) && IsOffCooldown(Zoe);
                    bool krasisReady = LevelChecked(Krasis) && IsOffCooldown(Krasis);
                    bool pepsisReady = LevelChecked(Pepsis) && IsOffCooldown(Pepsis);
                    bool haimaReady = LevelChecked(Haima) && IsOffCooldown(Haima);

                    // Set Target. Soft -> Hard -> Self priority, matching normal in-game behavior
                    GameObject? HealTarget;
                    if (Services.Service.TargetManager.SoftTarget?.ObjectKind is ObjectKind.Player) HealTarget = Services.Service.TargetManager.SoftTarget;
                    else HealTarget = CurrentTarget?.ObjectKind is ObjectKind.Player
                        ? CurrentTarget
                        : LocalPlayer;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && druocholeReady &&
                        Gauge.Addersgall >= 1 &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Druochole))
                        return Druochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && taurocholeReady &&
                        Gauge.Addersgall >= 1 &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Taurochole))
                        return Taurochole;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && rhizomataReady
                        && Gauge.Addersgall is 0)
                        return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
                        FindEffect(Buffs.Kardia) is null &&
                        FindEffect(Buffs.Kardion, HealTarget, LocalPlayer?.ObjectId) is null)
                        return Kardia;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria) && soteriaReady &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Soteria))
                        return Soteria;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe) && zoeReady &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Zoe))
                        return Zoe;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis) && krasisReady &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Krasis))
                        return Krasis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && pepsisReady &&
                        GetTargetHPPercent(HealTarget) <= GetOptionValue(Config.SGE_ST_Heal_Pepsis) &&
                        FindEffect(Buffs.EukrasianDiagnosis, HealTarget, LocalPlayer?.ObjectId) is not null) // Update for HealTarget
                        return Pepsis;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima) && haimaReady &&
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
                    bool rhizomataReady = LevelChecked(Rhizomata) && IsOffCooldown(Rhizomata);
                    bool keracholeReady = LevelChecked(Kerachole) && IsOffCooldown(Kerachole);
                    bool ixocholeReady = LevelChecked(Ixochole) && IsOffCooldown(Ixochole);
                    bool physisReady = LevelChecked(Physis) && IsOffCooldown(OriginalHook(Physis));
                    bool holosReady = LevelChecked(Holos) && IsOffCooldown(Holos);
                    bool panhaimaReady = LevelChecked(Panhaima) && IsOffCooldown(Panhaima);
                    bool pepsisReady = LevelChecked(Pepsis) && IsOffCooldown(Pepsis);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) && rhizomataReady &&
                        Gauge.Addersgall is 0)
                        return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && keracholeReady &&
                        Gauge.Addersgall >= 1)
                        return Kerachole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && ixocholeReady &&
                        Gauge.Addersgall >= 1)
                        return Ixochole;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis) && physisReady)
                        return OriginalHook(Physis);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && LevelChecked(Eukrasia) &&
                        FindEffect(Buffs.EukrasianPrognosis) is null)
                    {
                        if (!HasEffect(Buffs.Eukrasia))
                            return Eukrasia;

                        if (HasEffect(Buffs.Eukrasia))
                            return EukrasianPrognosis;
                    }

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos) && holosReady)
                        return Holos;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima) && panhaimaReady)
                        return Panhaima;

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && pepsisReady &&
                        FindEffect(Buffs.EukrasianPrognosis) is not null)
                        return Pepsis;
                }

                return actionID;
            }
        }
    }
}