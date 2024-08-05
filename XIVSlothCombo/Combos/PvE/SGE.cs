using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class SGE
    {
        internal const byte JobID = 40;

        // Actions
        internal const uint
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
            Toxikon2 = 24316,
            Pneuma = 24318,
            EukrasianDyskrasia = 37032,
            Psyche = 37033,

            // Buffs
            Soteria = 24294,
            Zoe = 24300,
            Krasis = 24317,
            Philosophia = 37035,

            // Other
            Kardia = 24285,
            Eukrasia = 24290,
            Rhizomata = 24309;

        // Action Groups
        internal static readonly List<uint>
            AddersgallList = [Taurochole, Druochole, Ixochole, Kerachole],
            DyskrasiaList = [Dyskrasia, Dyskrasia2];

        // Action Buffs
        internal static class Buffs
        {
            internal const ushort
                Kardia = 2604,
                Kardion = 2605,
                Eukrasia = 2606,
                EukrasianDiagnosis = 2607,
                EukrasianPrognosis = 2609,
                Panhaima = 2613,
                Kerachole = 2618,
                Eudaimonia = 3899;
        }

        internal static class Debuffs
        {
            internal const ushort
                EukrasianDosis = 2614,
                EukrasianDosis2 = 2615,
                EukrasianDosis3 = 2616,
                EukrasianDyskrasia = 3897;
        }

        // Debuff Pairs of Actions and Debuff
        internal static readonly Dictionary<uint, ushort>
            DosisList = new()
            {
                { Dosis,  Debuffs.EukrasianDosis  },
                { Dosis2, Debuffs.EukrasianDosis2 },
                { Dosis3, Debuffs.EukrasianDosis3 }
            };

        // Sage Gauge & Extensions
        public static SGEGauge Gauge => CustomComboFunctions.GetJobGauge<SGEGauge>();
        public static bool HasAddersgall(this SGEGauge gauge) => gauge.Addersgall > 0;
        public static bool HasAddersting(this SGEGauge gauge) => gauge.Addersting > 0;

        public static class Config
        {
            #region DPS
            public static UserBool
                SGE_ST_DPS_Adv = new("SGE_ST_DPS_Adv"),
                SGE_ST_DPS_EDosis_Adv = new("SGE_ST_Dosis_EDosis_Adv");
            public static UserBoolArray
                SGE_ST_DPS_Movement = new("SGE_ST_DPS_Movement");
            public static UserInt
                SGE_ST_DPS_EDosisHPPer = new("SGE_ST_DPS_EDosisHPPer", 10),
                SGE_ST_DPS_Lucid = new("SGE_ST_DPS_Lucid", 6500),
                SGE_ST_DPS_Rhizo = new("SGE_ST_DPS_Rhizo"),
                SGE_ST_DPS_AddersgallProtect = new("SGE_ST_DPS_AddersgallProtect", 3),
                SGE_AoE_DPS_Lucid = new("SGE_AoE_Phlegma_Lucid", 6500),
                SGE_AoE_DPS_Rhizo = new("SGE_AoE_DPS_Rhizo"),
                SGE_AoE_DPS_AddersgallProtect = new("SGE_AoE_DPS_AddersgallProtect", 3);
            public static UserFloat
                SGE_ST_DPS_EDosisThreshold = new("SGE_ST_Dosis_EDosisThreshold", 3.0f);
            #endregion

            #region Healing
            public static UserBool
                SGE_ST_Heal_Adv = new("SGE_ST_Heal_Adv"),
                SGE_ST_Heal_UIMouseOver = new("SGE_ST_Heal_UIMouseOver"),
                SGE_AoE_Heal_KeracholeTrait = new("SGE_AoE_Heal_KeracholeTrait");
            public static UserInt
                SGE_ST_Heal_Zoe = new("SGE_ST_Heal_Zoe"),
                SGE_ST_Heal_Haima = new("SGE_ST_Heal_Haima"),
                SGE_ST_Heal_Krasis = new("SGE_ST_Heal_Krasis"),
                SGE_ST_Heal_Pepsis = new("SGE_ST_Heal_Pepsis"),
                SGE_ST_Heal_Soteria = new("SGE_ST_Heal_Soteria"),
                SGE_ST_Heal_EDiagnosisHP = new("SGE_ST_Heal_EDiagnosisHP"),
                SGE_ST_Heal_Druochole = new("SGE_ST_Heal_Druochole"),
                SGE_ST_Heal_Taurochole = new("SGE_ST_Heal_Taurochole"),
                SGE_ST_Heal_Esuna = new("SGE_ST_Heal_Esuna");
            public static UserIntArray
                SGE_ST_Heals_Priority = new("SGE_ST_Heals_Priority"),
                SGE_AoE_Heals_Priority = new("SGE_AoE_Heals_Priority");
            public static UserBoolArray
                SGE_ST_Heal_EDiagnosisOpts = new("SGE_ST_Heal_EDiagnosisOpts");
            #endregion

            public static UserInt
                SGE_Eukrasia_Mode = new("SGE_Eukrasia_Mode");
        }

        internal static class Traits
        {
            internal const ushort
                EnhancedKerachole = 375,
                OffensiveMagicMasteryII = 376;
        }


        /*
         * SGE_Kardia
         * Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
         */
        internal class SGE_Kardia : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Kardia;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Soteria && (!HasEffect(Buffs.Kardia) || IsOnCooldown(Soteria)) ? Kardia : actionID;
        }

        /*
         * SGE_Rhizo
         * Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of Addersgall stacks
         * (Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
         */
        internal class SGE_Rhizo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Rhizo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => AddersgallList.Contains(actionID) && ActionReady(Rhizomata) && !Gauge.HasAddersgall() && IsOffCooldown(actionID) ? Rhizomata : actionID;
        }

        /*
         * Druo/Tauro
         * Druochole Upgrade to Taurochole (like a trait upgrade)
         * Replaces Druocole with Taurochole when Taurochole is available
         * (As of 6.0) Taurochole (single target massive insta heal w/ cooldown), Druochole (Single target insta heal)
         */
        internal class SGE_DruoTauro : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_DruoTauro;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Druochole && ActionReady(Taurochole) ? Taurochole : actionID;
        }

        /*
         * SGE_ZoePneuma (Zoe to Pneuma Combo)
         * Places Zoe on top of Pneuma when both are available.
         */
        internal class SGE_ZoePneuma : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ZoePneuma;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe) ? Zoe : actionID;
        }

        /*
         * SGE_AoE_DPS (Dyskrasia AoE Feature)
         * Replaces Dyskrasia with Phegma/Toxikon/Misc
         */
        internal class SGE_AoE_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (DyskrasiaList.Contains(actionID))
                {
                    if (!HasEffect(Buffs.Eukrasia))
                    {
                        // Variant Rampart
                        if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantRampart;

                        // Variant Spirit Dart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantSpiritDart;

                        // Lucid Dreaming
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Lucid) &&
                            ActionReady(All.LucidDreaming) && CanSpellWeave(Dosis) &&
                            LocalPlayer.CurrentMp <= Config.SGE_AoE_DPS_Lucid)
                            return All.LucidDreaming;

                        // Rhizomata
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Rhizo) && CanSpellWeave(Dosis) &&
                            ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_AoE_DPS_Rhizo)
                            return Rhizomata;

                        // Addersgall Protection
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_AddersgallProtect) && CanSpellWeave(Dosis) &&
                            ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_AoE_DPS_AddersgallProtect)
                            return Druochole;

                        //Eukrasia for DoT
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_EDyskrasia))
                        {
                            if (IsOffCooldown(Eukrasia) &&
                                !WasLastSpell(EukrasianDyskrasia) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                                HasBattleTarget() &&
                                InActionRange(Dyskrasia) && //Same range
                                DosisList.TryGetValue(OriginalHook(Dosis), out ushort dotDebuffID))
                            {
                                float dotDebuff = Math.Max(GetDebuffRemainingTime(dotDebuffID), GetDebuffRemainingTime(Debuffs.EukrasianDyskrasia));
                                float refreshtimer = 3; //Will revisit if it's really needed....SGE_ST_DPS_EDosis_Adv ? Config.SGE_ST_DPS_EDosisThreshold : 3;
                                if (dotDebuff <= refreshtimer &&
                                    GetTargetHPPercent() > 10)//Will Revisit if Config is needed Config.SGE_ST_DPS_EDosisHPPer)
                                    return Eukrasia;
                            }
                        }

                        // Psyche
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Psyche))
                        {
                            if (ActionReady(Psyche) &&
                                HasBattleTarget() &&
                                InActionRange(Psyche) &&
                                CanSpellWeave(actionID))
                                return Psyche;
                        }

                        //Phlegma
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Phlegma))
                        {
                            uint PhlegmaID = OriginalHook(Phlegma);
                            if (ActionReady(PhlegmaID) &&
                                HasBattleTarget() &&
                                InActionRange(PhlegmaID))
                                return PhlegmaID;
                        }

                        //Toxikon
                        if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Toxikon))
                        {
                            uint ToxikonID = OriginalHook(Toxikon);
                            if (ActionReady(ToxikonID) &&
                                HasBattleTarget() &&
                                InActionRange(ToxikonID) &&
                                Gauge.HasAddersting())
                            {
                                return ToxikonID;
                            }
                        }
                    }
                }
                return actionID;
            }
        }

        /*
         * SGE_ST_DPS (Single Target DPS Combo)
         * Currently Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
         * Kardia reminder, Lucid Dreaming, & Toxikon optional
         */
        internal class SGE_ST_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_DPS;
            internal static int Dosis3Count => ActionWatching.CombatActions.Count(x => x == Dosis3);

            internal static int Toxikon2Count => ActionWatching.CombatActions.Count(x => x == Toxikon2);


            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool ActionFound = actionID is Dosis2 || (!Config.SGE_ST_DPS_Adv && DosisList.ContainsKey(actionID));

                if (ActionFound)
                {
                    bool inOpener = IsEnabled(CustomComboPreset.SGE_ST_DPS_Opener)
                                 && Dosis3Count < 4 && Gauge.HasAddersting();

                    // Kardia Reminder
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Kardia) && LevelChecked(Kardia) &&
                        FindEffect(Buffs.Kardia) is null)
                        return Kardia;

                    if (inOpener)
                    {
                        if (Dosis3Count is 0 && Toxikon2Count is 0 && 
                            !HasEffect(Buffs.Eukrasia))
                            return Eukrasia;

                        if (Dosis3Count is 0 && Toxikon2Count is 0 &&
                            HasEffect(Buffs.Eukrasia))
                            return Toxikon2;

                        if (Dosis3Count is 3)
                        {
                            if (WasLastSpell(Phlegma3) &&
                                ActionReady(Psyche) &&
                                CanWeave(actionID))
                                return Psyche;

                            if (ActionReady(Phlegma3))
                                return Phlegma3;
                        }

                        if (Dosis3Count > 0 && Toxikon2Count > 0)
                            return Dosis3;
                    }

                    // Lucid Dreaming
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Lucid) &&
                        All.CanUseLucid(actionID, Config.SGE_ST_DPS_Lucid))
                        return All.LucidDreaming;

                    // Variant
                    if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Rhizomata
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Rhizo) && CanSpellWeave(actionID) &&
                        ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_ST_DPS_Rhizo)
                        return Rhizomata;

                    // Addersgall Protection
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_AddersgallProtect) && CanSpellWeave(Dosis) &&
                        ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_ST_DPS_AddersgallProtect)
                        return Druochole;

                    if (HasBattleTarget() && !HasEffect(Buffs.Eukrasia))
                    // Buff check Above. Without it, Toxikon and any future option will interfere in the Eukrasia->Eukrasia Dosis combo
                    {
                        // Eukrasian Dosis.
                        // If we're too low level to use Eukrasia, we can stop here.
                        if (IsEnabled(CustomComboPreset.SGE_ST_DPS_EDosis) && LevelChecked(Eukrasia) && InCombat())
                        {
                            // Grab current Dosis via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                            // Using TryGetValue due to edge case where the actionID would be read as Eukrasian Dosis instead of Dosis
                            // EDosis will show for half a second if the buff is removed manually or some other act of God
                            if (DosisList.TryGetValue(OriginalHook(actionID), out ushort dotDebuffID))
                            {
                                if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_SpiritDart) &&
                                    IsEnabled(Variant.VariantSpiritDart) &&
                                    GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                                    CanSpellWeave(actionID))
                                    return Variant.VariantSpiritDart;

                                // Dosis DoT Debuff
                                float dotDebuff = GetDebuffRemainingTime(dotDebuffID);
                                // Check for the AoE DoT.  These DoTs overlap, so get time remaining of any of them
                                if (TraitLevelChecked(Traits.OffensiveMagicMasteryII))
                                    dotDebuff = Math.Max(dotDebuff, GetDebuffRemainingTime(Debuffs.EukrasianDyskrasia));

                                float refreshtimer = Config.SGE_ST_DPS_EDosis_Adv ? Config.SGE_ST_DPS_EDosisThreshold : 3;

                                if (dotDebuff <= refreshtimer &&
                                    GetTargetHPPercent() > Config.SGE_ST_DPS_EDosisHPPer)
                                    return Eukrasia;
                            }
                        }

                        // Phlegma
                        if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma) && InCombat())
                        {
                            uint phlegma = OriginalHook(Phlegma);
                            if (InActionRange(phlegma) && ActionReady(phlegma)) return phlegma;
                        }

                        // Psyche
                        if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Psyche) &&
                                                        ActionReady(Psyche) &&
                            InCombat() &&
                            CanSpellWeave(actionID) &&
                            WasLastSpell(OriginalHook(Phlegma))) //ToDo: Verify
                            return Psyche;


                        // Movement Options
                        if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Movement) && InCombat() && IsMoving)
                        {
                            // Psyche
                            if (Config.SGE_ST_DPS_Movement[3] && ActionReady(Psyche)) return Psyche;
                            // Toxikon
                            if (Config.SGE_ST_DPS_Movement[0] && LevelChecked(Toxikon) && Gauge.HasAddersting()) return OriginalHook(Toxikon);
                            // Dyskrasia
                            if (Config.SGE_ST_DPS_Movement[1] && LevelChecked(Dyskrasia) && InActionRange(Dyskrasia)) return OriginalHook(Dyskrasia);
                            // Eukrasia
                            if (Config.SGE_ST_DPS_Movement[2] && LevelChecked(Eukrasia)) return Eukrasia;
                        }
                    }
                }
                return actionID;
            }
        }

        /*
         * SGE_Raise (Swiftcast Raise)
         * Swiftcast becomes Egeiro when on cooldown
         */
        internal class SGE_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Raise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                    => actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Egeiro : actionID;
        }

        /* 
         * SGE_Eukrasia (Eukrasia combo)
         * Normally after Eukrasia is used and updates the abilities, it becomes disabled
         * This will "combo" the action to user selected action
         */
        internal class SGE_Eukrasia : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Eukrasia;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Eukrasia && HasEffect(Buffs.Eukrasia))
                {
                    switch ((int)Config.SGE_Eukrasia_Mode)
                    {
                        case 0: return OriginalHook(Dosis);
                        case 1: return OriginalHook(Diagnosis);
                        case 2: return OriginalHook(Prognosis);
                        case 3: return OriginalHook(Dyskrasia);
                        default: break;
                    }
                }

                return actionID;
            }
        }

        /* 
         * SGE_ST_Heal (Diagnosis Single Target Heal)
         * Replaces Diagnosis with various Single Target healing options, 
         * Pseudo priority set by various custom user percentages
         */
        internal class SGE_ST_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Diagnosis)
                {
                    if (HasEffect(Buffs.Eukrasia))
                        return EukrasianDiagnosis;

                    IGameObject? healTarget = GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Esuna) && ActionReady(All.Esuna) &&
                        GetTargetHPPercent(healTarget) >= Config.SGE_ST_Heal_Esuna &&
                        HasCleansableDebuff(healTarget))
                        return All.Esuna;


                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                        !Gauge.HasAddersgall())
                        return Rhizomata;

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
                        FindEffect(Buffs.Kardia) is null &&
                        FindEffect(Buffs.Kardion, healTarget, LocalPlayer?.GameObjectId) is null)
                        return Kardia;

                    foreach (var prio in Config.SGE_ST_Heals_Priority.Items.OrderBy(x => x))
                    {
                        var index = Config.SGE_ST_Heals_Priority.IndexOf(prio);
                        var config = JobHelpers.SGE.GetMatchingConfigST(index, out var spell, out bool enabled);

                        if (enabled)
                        {
                            if (GetTargetHPPercent(healTarget) <= config &&
                                ActionReady(spell))
                                return spell;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SGE_ST_Heal_EDiagnosis) && LevelChecked(Eukrasia) &&
                        GetTargetHPPercent(healTarget) <= Config.SGE_ST_Heal_EDiagnosisHP &&
                        (Config.SGE_ST_Heal_EDiagnosisOpts[0] || FindEffectOnMember(Buffs.EukrasianDiagnosis, healTarget) is null) && //Ignore existing shield check
                        (!Config.SGE_ST_Heal_EDiagnosisOpts[1] || FindEffectOnMember(SCH.Buffs.Galvanize, healTarget) is null)) //Galvenize Check
                        return Eukrasia;

                }

                return actionID;
            }
        }

        /* 
         * SGE_AoE_Heal (Prognosis AoE Heal)
         * Replaces Prognosis with various AoE healing options, 
         * Pseudo priority set by various custom user percentages
         */
        internal class SGE_AoE_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_Heal;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Prognosis)
                {
                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && HasEffect(Buffs.Eukrasia))
                        return OriginalHook(Prognosis);

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                        !Gauge.HasAddersgall())
                        return Rhizomata;

                    foreach (var prio in Config.SGE_AoE_Heals_Priority.Items.OrderBy(x => x))
                    {
                        var index = Config.SGE_AoE_Heals_Priority.IndexOf(prio);
                        var config = JobHelpers.SGE.GetMatchingConfigAoE(index, out var spell, out bool enabled);

                        if (enabled)
                        {
                            if (ActionReady(spell))
                                return spell;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && LevelChecked(Eukrasia) &&
                        (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis_IgnoreShield) ||
                         FindEffect(Buffs.EukrasianPrognosis) is null))
                        return Eukrasia;
                }

                return actionID;
            }
        }

        internal class SGE_OverProtect : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_OverProtect;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Kerachole && IsEnabled(CustomComboPreset.SGE_OverProtect_Kerachole) && ActionReady(Kerachole))
                {
                    if (HasEffectAny(Buffs.Kerachole) ||
                        (IsEnabled(CustomComboPreset.SGE_OverProtect_SacredSoil) && HasEffectAny(SCH.Buffs.SacredSoil)))
                        return SCH.SacredSoil;
                }

                if (actionID is Panhaima && IsEnabled(CustomComboPreset.SGE_OverProtect_Panhaima) &&
                    ActionReady(Panhaima) && HasEffectAny(Buffs.Panhaima)) return SCH.SacredSoil;

                if (actionID is Philosophia && IsEnabled(CustomComboPreset.SGE_OverProtect_Philosophia) &&
                    ActionReady(Philosophia) && HasEffectAny(Buffs.Eudaimonia)) return SCH.Consolation;

                return actionID;
            }
        }
    }
}