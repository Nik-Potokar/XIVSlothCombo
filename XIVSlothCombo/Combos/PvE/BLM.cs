using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class BLM
    {
        public const byte ClassID = 7;
        public const byte JobID = 25;

        internal const uint
            Fire = 141,
            Blizzard = 142,
            Thunder = 144,
            Blizzard2 = 25793,
            Transpose = 149,
            Fire2 = 147,
            Fire3 = 152,
            Thunder3 = 153,
            Thunder2 = 7447,
            Thunder4 = 7420,
            Blizzard3 = 154,
            Scathe = 156,
            Freeze = 159,
            Flare = 162,
            AetherialManipulation = 155,
            LeyLines = 3573,
            Blizzard4 = 3576,
            Fire4 = 3577,
            BetweenTheLines = 7419,
            Despair = 16505,
            UmbralSoul = 16506,
            Paradox = 25797,
            Amplifier = 25796,
            HighFire2 = 25794,
            HighBlizzard2 = 25795,
            Xenoglossy = 16507,
            Foul = 7422,
            Sharpcast = 3574,
            Manafont = 158,
            Triplecast = 7421;

        internal static class Buffs
        {
            internal const ushort
                Thundercloud = 164,
                LeyLines = 737,
                CircleOfPower = 738,
                Firestarter = 165,
                Sharpcast = 867,
                Triplecast = 1211,
                EnhancedFlare = 2960;
        }

        internal static class Debuffs
        {
            internal const ushort
                Thunder = 161,
                Thunder2 = 162,
                Thunder3 = 163,
                Thunder4 = 1210;
        }

        internal static class Traits
        {
            internal const uint
                AspectMasteryIII = 459,
                EnhancedManafont = 463,
                EnhancedFreeze = 295,
                EnhancedPolyGlot = 297,
                EnhancedFoul = 461;
        }

        internal static class MP
        {
            internal const int MaxMP = 10000;

            internal const int AllMPSpells = 800; //"ALL MP" spell. Only caring about the absolute minimum.
            internal static int Thunder => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Thunder));
            internal static int ThunderAoE => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Thunder2));
            internal static int Fire => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Fire));
            internal static int FireAoE => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Fire2));
            internal static int Fire3 => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Fire3));
        }

        // Debuff Pairs of Actions and Debuff
        internal static readonly Dictionary<uint, ushort>
            ThunderList = new()
            {
                { Thunder,  Debuffs.Thunder  },
                { Thunder2, Debuffs.Thunder2 },
                { Thunder3, Debuffs.Thunder3 },
                { Thunder4, Debuffs.Thunder4 }
            };

        private static bool HasPolyglotStacks(this BLMGauge gauge) => gauge.PolyglotStacks > 0;

        internal static class Config
        {
            internal const string BLM_AstralFire_Refresh = "Blm_AstralFire_Refresh";
            internal const string BLM_VariantCure = "Blm_VariantCure";
            internal const string BLM_Simple_OpenerSelection = "BLM_Simple_OpenerSelection";
            internal const string BLM_Advanced_OpenerSelection = "BLM_Advanced_OpenerSelection";
            internal const string BLM_Adv_Cooldowns = "BLM_Adv_Cooldowns";

            internal static UserBoolArray
                BLM_Adv_Cooldowns_Choice = new("BLM_Adv_Cooldowns_Choice"),
                BLM_Adv_Movement_Choice = new("BLM_Adv_Movement_Choice");
        }
        internal class BLM_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_SimpleMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    uint currentMP = LocalPlayer.CurrentMp;
                    float astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFire_Refresh) * 1000;
                    bool openerReady = ActionReady(Manafont) && ActionReady(Amplifier) && ActionReady(LeyLines);
                    int openerSelection = PluginConfiguration.GetCustomIntValue(Config.BLM_Simple_OpenerSelection);
                    Status? dotDebuff = FindTargetEffect(ThunderList[OriginalHook(Thunder)]); // Match DoT with its debuff ID, and check for the debuff
                    BLMGauge? gauge = GetJobGauge<BLMGauge>();

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Spam Umbral Soul/Transpose when there's no target
                    if (CurrentTarget is null && gauge.ElementTimeRemaining > 0)
                    {
                        if (gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // Opener for BLM
                    // Standard opener
                    if (level >= 90)
                    {
                        if (openerSelection is 0 or 1)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Sharpcast))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(Buffs.Sharpcast) && !inOpener)
                            { openerStarted = true; return Fire3; }
                            else
                            { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return Sharpcast;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 1 && lastComboMove is Fire3 && !HasEffect(Buffs.Sharpcast)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()) ||
                                (InCombat() && inOpener && gauge.ElementTimeRemaining <= 0))
                                inOpener = false;

                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Sharpcast) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (lastComboMove == Fire3) step++;
                                    else return Fire3;
                                }

                                if (step == 1)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                if (step == 2)
                                {
                                    if (GetRemainingCharges(Triplecast) < 2) step++;
                                    else return Triplecast;
                                }

                                if (step == 3)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Fire4;
                                }

                                if (step == 4)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 5)
                                {
                                    if (IsOnCooldown(Amplifier)) step++;
                                    else return Amplifier;
                                }

                                if (step == 6)
                                {
                                    if (IsOnCooldown(LeyLines)) step++;
                                    else return LeyLines;
                                }

                                if (step == 7)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Fire4;
                                }

                                if (step == 8)
                                {
                                    if (IsOnCooldown(All.Swiftcast)) step++;
                                    else return All.Swiftcast;
                                }

                                if (step == 9)
                                {
                                    if (currentMP <= MP.Fire) step++;
                                    else return Fire4;
                                }

                                if (step == 10)
                                {
                                    if (GetRemainingCharges(Triplecast) < 1) step++;
                                    else return Triplecast;
                                }

                                if (step == 11)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Despair;
                                }

                                if (step == 12)
                                {
                                    if (IsOnCooldown(Manafont)) step++;
                                    else return Manafont;
                                }

                                if (step == 13)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 14)
                                {
                                    if (HasEffect(Buffs.Sharpcast)) step++;
                                    else return Sharpcast;
                                }

                                if (step == 15)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Despair;
                                }

                                if (step == 16)
                                {
                                    if (gauge.IsParadoxActive) step++;
                                    else return Blizzard3;
                                }

                                if (step == 17)
                                {
                                    if (lastComboMove == Xenoglossy) step++;
                                    else return Xenoglossy;
                                }

                                if (step == 18)
                                {
                                    if (lastComboMove == Paradox) step++;
                                    else return Paradox;
                                }

                                if (step == 19)
                                {
                                    if (lastComboMove == Blizzard4) step++;
                                    else return Blizzard4;
                                }

                                if (step == 20)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                inOpener = false;
                            }
                        }
                    }

                    if (!inOpener)
                    {
                        // Handle initial cast
                        if (gauge.ElementTimeRemaining <= 0)
                        {
                            if (LevelChecked(Fire3))
                                return (currentMP >= MP.Fire3)
                                    ? Fire3
                                    : Blizzard3;

                            return (currentMP >= MP.Fire)
                                ? Fire
                                : Blizzard;
                        }

                        // Before Blizzard III; Fire until 0 MP, then Blizzard until max MP.
                        if (!LevelChecked(Blizzard3))
                        {
                            if (gauge.InAstralFire)
                                return (currentMP < MP.Fire)
                                    ? Transpose
                                    : Fire;

                            if (gauge.InUmbralIce)
                                return (currentMP >= MP.MaxMP - MP.Thunder)
                                    ? Transpose
                                    : Blizzard;
                        }

                        // Before Fire IV; Fire until 0 MP (w/ Firestarter), then Blizzard III and Blizzard/Blizzard IV until max MP.
                        if (!LevelChecked(Fire4))
                        {
                            if (gauge.InAstralFire)
                            {
                                if (HasEffect(Buffs.Firestarter) && currentMP <= MP.Fire)
                                    return Fire3;

                                return (currentMP < MP.Fire)
                                    ? Blizzard3
                                    : Fire;
                            }

                            if (gauge.InUmbralIce)
                            {
                                if (LevelChecked(Blizzard4) && gauge.UmbralHearts < 3)
                                    return Blizzard4;

                                return (currentMP >= MP.MaxMP || gauge.UmbralHearts is 3)
                                    ? Fire3
                                    : Blizzard;
                            }
                        }

                        // Use under Fire or Ice
                        if (gauge.ElementTimeRemaining > 0)
                        {
                            // Handle movement
                            if (IsMoving && InCombat())
                            {
                                if (!HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast))
                                    return Sharpcast;

                                if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast) &&
                                    (dotDebuff is null || dotDebuff?.RemainingTime <= 10))
                                    return OriginalHook(Thunder);

                                if (HasEffect(Buffs.Firestarter) && gauge.InAstralFire && LevelChecked(Fire3))
                                    return Fire3;

                                if (LevelChecked(Paradox) && gauge.IsParadoxActive && gauge.InUmbralIce)
                                    return Paradox;

                                if (LevelChecked(Xenoglossy) && gauge.PolyglotStacks > 1)
                                    return Xenoglossy;

                                if (ActionReady(All.Swiftcast) && !HasEffect(Buffs.Triplecast))
                                    return All.Swiftcast;

                                if (HasCharges(Triplecast) && GetBuffStacks(Buffs.Triplecast) is 0 && !HasEffect(All.Buffs.Swiftcast))
                                    return Triplecast;

                                if (GetBuffStacks(Buffs.Triplecast) is 0)
                                    return Scathe;
                            }

                            // Thunder uptime
                            if (gauge.ElementTimeRemaining >= astralFireRefresh &&
                                !ThunderList.ContainsKey(lastComboMove) && !TargetHasEffect(Debuffs.Thunder2) &&
                                !TargetHasEffect(Debuffs.Thunder4) && LevelChecked(lastComboMove) &&
                                ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || currentMP >= MP.Thunder) &&
                                (dotDebuff is null || dotDebuff?.RemainingTime <= 4))
                                return OriginalHook(Thunder);

                            // Weave Buffs
                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (CanSpellWeave(actionID))
                            {
                                if (GetRemainingCharges(Triplecast) is 2 && ActionReady(Triplecast) &&
                                    !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) &&
                                    (gauge.InAstralFire || gauge.UmbralHearts is 3) &&
                                    currentMP >= MP.Fire * 2)
                                    return Triplecast;

                                if (ActionReady(Amplifier) && gauge.PolyglotStacks < 2)
                                    return Amplifier;

                                if (ActionReady(LeyLines))
                                    return LeyLines;
                            }

                            // Use Polyglot stacks if we don't need it for a future weave
                            if (!HasCharges(Triplecast) && gauge.PolyglotStacks is 2 && gauge.ElementTimeRemaining >= astralFireRefresh &&
                                (gauge.InUmbralIce || (gauge.InAstralFire && gauge.UmbralHearts is 0)) &&
                                GetCooldownRemainingTime(LeyLines) >= 20 &&
                                GetCooldownRemainingTime(Triplecast) >= 20)
                                return LevelChecked(Xenoglossy)
                                        ? Xenoglossy
                                        : Foul;
                        }

                        // Normal Fire Phase
                        if (gauge.InAstralFire)
                        {
                            // Xenoglossy overcap protection
                            if ((gauge.PolyglotStacks is 2 && (gauge.EnochianTimer <= 3000) && TraitLevelChecked(Traits.EnhancedPolyGlot)) ||
                                (gauge.PolyglotStacks is 1 && (gauge.EnochianTimer <= 6000) && !TraitLevelChecked(Traits.EnhancedPolyGlot)))
                                return LevelChecked(Xenoglossy)
                                    ? Xenoglossy
                                    : Foul;

                            // Fire III proc or Swiftcast Fire III during Transpose lines(< 3 Astral Fire stacks)
                            if (gauge.AstralFireStacks < 3 || (gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter)))
                                return Fire3;

                            // Use Paradox instead of hardcasting Fire if we can
                            if (gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                                return LevelChecked(Paradox) && gauge.IsParadoxActive
                                    ? Paradox
                                    : Fire;

                            if (ActionReady(Manafont) && WasLastAction(Despair))
                                return Manafont;

                            // Cast Fire IV after Manafont
                            if (IsOnCooldown(Manafont) && WasLastAction(Manafont))
                                return Fire4;

                            // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                            if ((ActionReady(LeyLines) || (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) && GetRemainingCharges(Triplecast) > 1) ||
                               (ActionReady(Manafont) && currentMP < MP.AllMPSpells) ||
                               (ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast))) &&
                               !WasLastAction(Xenoglossy) && gauge.ElementTimeRemaining >= astralFireRefresh && LevelChecked(Xenoglossy))
                                return Xenoglossy;

                            // Blizzard III/Despair when below Fire IV + Despair MP
                            if (currentMP < MP.Fire || gauge.ElementTimeRemaining <= 5000)
                            {
                                return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells)
                                    ? Despair
                                    : Blizzard3;
                            }

                            return Fire4;
                        }

                        // Normal Ice Phase
                        if (gauge.InUmbralIce)
                        {
                            // Xenoglossy overcap protection
                            if (gauge.EnochianTimer <= 20000 &&
                                ((gauge.PolyglotStacks is 2 && TraitLevelChecked(Traits.EnhancedPolyGlot)) ||
                                (gauge.PolyglotStacks is 1 && !TraitLevelChecked(Traits.EnhancedPolyGlot))))
                                return LevelChecked(Xenoglossy)
                                    ? Xenoglossy
                                    : Foul;

                            // Sharpcast
                            if (ActionReady(Sharpcast) && !WasLastAction(Thunder3) && !HasEffect(Buffs.Sharpcast) && CanSpellWeave(actionID))
                                return Sharpcast;

                            // Use Paradox when available
                            if (LevelChecked(Paradox) && gauge.IsParadoxActive)
                                return Paradox;

                            // Fire III when at max Umbral Hearts
                            return (gauge.UmbralHearts is 3 && currentMP >= MP.MaxMP - MP.Thunder)
                                ? Fire3
                                : Blizzard4;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class BLM_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AdvancedMode;
            internal static bool inOpener = false;
            internal static bool readyOpener = false;
            internal static bool openerStarted = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    uint currentMP = LocalPlayer.CurrentMp;
                    float astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFire_Refresh) * 1000;
                    bool openerReady = ActionReady(Manafont) && ActionReady(Amplifier) && ActionReady(LeyLines);
                    int openerSelection = PluginConfiguration.GetCustomIntValue(Config.BLM_Advanced_OpenerSelection);
                    int pooledPolyglotStacks = Config.BLM_Adv_Movement_Choice[5] ? 1 : 0;
                    Status? dotDebuff = FindTargetEffect(ThunderList[OriginalHook(Thunder)]); // Match DoT with its debuff ID, and check for the debuff
                    BLMGauge? gauge = GetJobGauge<BLMGauge>();

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_Adv_UmbralSoul) &&
                        CurrentTarget is null && gauge.ElementTimeRemaining > 0)
                    {
                        if (gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // Opener for BLM
                    // Standard opener
                    if (IsEnabled(CustomComboPreset.BLM_Adv_Opener) && level >= 90)
                    {
                        if (openerSelection is 0 or 1)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Sharpcast))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(Buffs.Sharpcast) && !inOpener)
                            { openerStarted = true; return Fire3; }
                            else
                            { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return Sharpcast;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 1 && lastComboMove is Fire3 && !HasEffect(Buffs.Sharpcast)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()) ||
                                (InCombat() && inOpener && gauge.ElementTimeRemaining <= 0))
                                inOpener = false;

                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Sharpcast) &&
                                IsEnabled(CustomComboPreset.BLM_Adv_Opener) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (lastComboMove == Fire3) step++;
                                    else return Fire3;
                                }

                                if (step == 1)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                if (step == 2)
                                {
                                    if (GetRemainingCharges(Triplecast) < 2) step++;
                                    else return Triplecast;
                                }

                                if (step == 3)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Fire4;
                                }

                                if (step == 4)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 5)
                                {
                                    if (IsOnCooldown(Amplifier)) step++;
                                    else return Amplifier;
                                }

                                if (step == 6)
                                {
                                    if (IsOnCooldown(LeyLines)) step++;
                                    else return LeyLines;
                                }

                                if (step == 7)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Fire4;
                                }

                                if (step == 8)
                                {
                                    if (IsOnCooldown(All.Swiftcast)) step++;
                                    else return All.Swiftcast;
                                }

                                if (step == 9)
                                {
                                    if (currentMP <= MP.Fire) step++;
                                    else return Fire4;
                                }

                                if (step == 10)
                                {
                                    if (GetRemainingCharges(Triplecast) < 1) step++;
                                    else return Triplecast;
                                }

                                if (step == 11)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Despair;
                                }

                                if (step == 12)
                                {
                                    if (IsOnCooldown(Manafont)) step++;
                                    else return Manafont;
                                }

                                if (step == 13)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 14)
                                {
                                    if (HasEffect(Buffs.Sharpcast)) step++;
                                    else return Sharpcast;
                                }

                                if (step == 15)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Despair;
                                }

                                if (step == 16)
                                {
                                    if (gauge.IsParadoxActive) step++;
                                    else return Blizzard3;
                                }

                                if (step == 17)
                                {
                                    if (lastComboMove == Xenoglossy) step++;
                                    else return Xenoglossy;
                                }

                                if (step == 18)
                                {
                                    if (lastComboMove == Paradox) step++;
                                    else return Paradox;
                                }

                                if (step == 19)
                                {
                                    if (lastComboMove == Blizzard4) step++;
                                    else return Blizzard4;
                                }

                                if (step == 20)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                inOpener = false;
                            }
                        }

                        // F3 OPENER DOUBLE TRANSPOSE VARIATION 
                        if (openerSelection is 2)
                        {
                            // Check to start opener
                            if (openerStarted && HasEffect(Buffs.Sharpcast))
                            { inOpener = true; openerStarted = false; readyOpener = false; }

                            if ((readyOpener || openerStarted) && HasEffect(Buffs.Sharpcast) && !inOpener)
                            { openerStarted = true; return Fire3; }
                            else
                            { openerStarted = false; }

                            // Reset check for opener
                            if (openerReady && !InCombat() && !inOpener && !openerStarted)
                            {
                                readyOpener = true;
                                inOpener = false;
                                step = 0;
                                return Sharpcast;
                            }
                            else
                            { readyOpener = false; }

                            // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the InCombat() check can be slow
                            if ((step == 1 && lastComboMove is Fire3 && !HasEffect(Buffs.Sharpcast)) ||
                                (inOpener && step >= 2 && IsOffCooldown(actionID) && !InCombat()) ||
                                (InCombat() && inOpener && gauge.ElementTimeRemaining <= 0)) inOpener = false;

                            if (InCombat() && CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Sharpcast) &&
                                IsEnabled(CustomComboPreset.BLM_Adv_Opener) && level >= 90 && openerReady)
                                inOpener = true;

                            if (inOpener)
                            {
                                if (step == 0)
                                {
                                    if (lastComboMove == Fire3) step++;
                                    else return Fire3;
                                }

                                if (step == 1)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                if (step == 2)
                                {
                                    if (lastComboMove == Fire4) step++;
                                    else return Fire4;
                                }

                                if (step == 3)
                                {
                                    if (GetRemainingCharges(Triplecast) < 2) step++;
                                    else return Triplecast;
                                }

                                if (step == 4)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Fire4;
                                }

                                if (step == 5)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 6)
                                {
                                    if (IsOnCooldown(Amplifier)) step++;
                                    else return Amplifier;
                                }

                                if (step == 7)
                                {
                                    if (IsOnCooldown(LeyLines)) step++;
                                    else return LeyLines;
                                }

                                if (step == 8)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Fire4;
                                }

                                if (step == 9)
                                {
                                    if (GetRemainingCharges(Triplecast) < 1) step++;
                                    else return Triplecast;
                                }

                                if (step == 10)
                                {
                                    if (IsOnCooldown(All.LucidDreaming)) step++;
                                    else return All.LucidDreaming;
                                }

                                if (step == 11)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 2) step++;
                                    else return Despair;
                                }

                                if (step == 12)
                                {
                                    if (IsOnCooldown(Manafont)) step++;
                                    else return Manafont;
                                }

                                if (step == 13)
                                {
                                    if ((lastComboMove == Fire4) && GetBuffStacks(Buffs.Triplecast) is 1) step++;
                                    else return Fire4;
                                }

                                if (step == 14)
                                {
                                    if (HasEffect(Buffs.Sharpcast)) step++;
                                    else return Sharpcast;
                                }

                                if (step == 15)
                                {
                                    if ((lastComboMove == Despair) && GetBuffStacks(Buffs.Triplecast) is 0) step++;
                                    else return Despair;
                                }

                                if (step == 16)
                                {
                                    if (gauge.IsParadoxActive) step++;
                                    else return Transpose;
                                }

                                if (step == 17)
                                {
                                    if (lastComboMove == Paradox) step++;
                                    else return Paradox;
                                }

                                if (step == 18)
                                {
                                    if (IsOnCooldown(All.Swiftcast)) step++;
                                    else return All.Swiftcast;
                                }

                                if (step == 19)
                                {
                                    if (lastComboMove == Xenoglossy) step++;
                                    else return Xenoglossy;
                                }

                                if (step == 20)
                                {
                                    if (lastComboMove == Thunder3) step++;
                                    else return Thunder3;
                                }

                                if (step == 21)
                                {
                                    if (currentMP == MP.MaxMP) step++;
                                    else return Blizzard3;
                                }

                                if (step == 22)
                                {
                                    if (gauge.InAstralFire && currentMP == MP.MaxMP) step++;
                                    else return Transpose;
                                }

                                if (step == 23)
                                {
                                    if (lastComboMove == Fire3) step++;
                                    else return Fire3;
                                }

                                if (step == 24)
                                {
                                    if (currentMP <= MP.Fire) step++;
                                    else return Fire4;
                                }

                                if (step == 25)
                                {
                                    if (lastComboMove == Despair) step++;
                                    else return Despair;
                                }

                                inOpener = false;
                            }
                        }
                    }

                    if (!inOpener)
                    {
                        // Use under Fire or Ice
                        if (gauge.ElementTimeRemaining > 0)
                        {
                            // Handle movement
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Movement) && IsMoving && InCombat())
                            {
                                if (Config.BLM_Adv_Movement_Choice[0] &&
                                    !HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast))
                                    return Sharpcast;

                                if (Config.BLM_Adv_Movement_Choice[1] &&
                                    HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast) &&
                                    (dotDebuff is null || dotDebuff?.RemainingTime <= 10))
                                    return OriginalHook(Thunder);

                                if (Config.BLM_Adv_Movement_Choice[2] &&
                                    HasEffect(Buffs.Firestarter) && gauge.InAstralFire && LevelChecked(Fire3))
                                    return Fire3;

                                if (Config.BLM_Adv_Movement_Choice[3] &&
                                    LevelChecked(Paradox) && gauge.IsParadoxActive && gauge.InUmbralIce)
                                    return Paradox;

                                if (Config.BLM_Adv_Movement_Choice[4] &&
                                    LevelChecked(Xenoglossy) && gauge.PolyglotStacks > 1)
                                    return Xenoglossy;

                                if ((IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Rotation) || level < 90) &&
                                    Config.BLM_Adv_Movement_Choice[5] &&
                                    ActionReady(All.Swiftcast) && !HasEffect(Buffs.Triplecast))
                                    return All.Swiftcast;

                                if (Config.BLM_Adv_Movement_Choice[6] &&
                                    HasCharges(Triplecast) && GetBuffStacks(Buffs.Triplecast) is 0 && !HasEffect(All.Buffs.Swiftcast))
                                    return Triplecast;

                                if (Config.BLM_Adv_Movement_Choice[7] && (GetBuffStacks(Buffs.Triplecast) is 0))
                                    return Scathe;
                            }

                            // Thunder uptime
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Thunder) &&
                                gauge.ElementTimeRemaining >= astralFireRefresh &&
                                !ThunderList.ContainsKey(lastComboMove) && !TargetHasEffect(Debuffs.Thunder2) &&
                                !TargetHasEffect(Debuffs.Thunder4) && LevelChecked(lastComboMove) &&
                                ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || currentMP >= MP.Thunder) &&
                                (dotDebuff is null || dotDebuff?.RemainingTime <= 4))
                                return OriginalHook(Thunder);

                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Casts) &&
                                (IsNotEnabled(CustomComboPreset.BLM_Adv_Triplecast_Pooling) || GetRemainingCharges(Triplecast) is 2) &&
                                LevelChecked(Triplecast) && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) &&
                                (gauge.InAstralFire || gauge.UmbralHearts is 3) &&
                                currentMP >= MP.Fire * 2)
                                return Triplecast;

                            // Weave Buffs
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Cooldowns) && CanSpellWeave(actionID))
                            {
                                if (Config.BLM_Adv_Cooldowns_Choice[2] &&
                                    ActionReady(Amplifier) && gauge.PolyglotStacks < 2)
                                    return Amplifier;

                                if (Config.BLM_Adv_Cooldowns_Choice[3] && ActionReady(LeyLines))
                                    return LeyLines;
                            }
                        }
                        // Handle initial cast
                        if (gauge.ElementTimeRemaining <= 0)
                        {
                            if (LevelChecked(Fire3))
                                return (currentMP >= MP.Fire3)
                                    ? Fire3
                                    : Blizzard3;

                            return (currentMP >= MP.Fire)
                                ? Fire
                                : Blizzard;
                        }

                        // Before Blizzard III; Fire until 0 MP, then Blizzard until max MP.
                        if (!LevelChecked(Blizzard3))
                        {
                            if (gauge.InAstralFire)
                                return (currentMP < MP.Fire)
                                    ? Transpose
                                    : Fire;

                            if (gauge.InUmbralIce)
                                return (currentMP >= MP.MaxMP - MP.Thunder)
                                    ? Transpose
                                    : Blizzard;
                        }

                        // Before Fire IV; Fire until 0 MP (w/ Firestarter), then Blizzard III and Blizzard/Blizzard IV until max MP.
                        if (!LevelChecked(Fire4))
                        {
                            if (gauge.InAstralFire)
                            {
                                if (HasEffect(Buffs.Firestarter) && currentMP <= MP.Fire)
                                    return Fire3;

                                return (currentMP < MP.Fire)
                                    ? Blizzard3
                                    : Fire;
                            }

                            if (gauge.InUmbralIce)
                            {
                                if (LevelChecked(Blizzard4) && gauge.UmbralHearts < 3)
                                    return Blizzard4;

                                return (currentMP == MP.MaxMP || gauge.UmbralHearts is 3)
                                    ? Fire3
                                    : Blizzard;
                            }
                        }

                        // Use Polyglot stacks if we don't need it for a future weave
                        // Only when we're not using Transpose lines
                        if ((IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Rotation) || level < 90) && IsEnabled(CustomComboPreset.BLM_Adv_Cooldowns) &&
                            (IsNotEnabled(CustomComboPreset.BLM_Adv_Triplecast_Pooling) || (IsEnabled(CustomComboPreset.BLM_Adv_Triplecast_Pooling) && !HasCharges(Triplecast))) &&
                            gauge.PolyglotStacks is 2 && gauge.ElementTimeRemaining >= astralFireRefresh &&
                            (gauge.InUmbralIce || (gauge.InAstralFire && gauge.UmbralHearts is 0)) &&
                            GetCooldownRemainingTime(LeyLines) >= 20 && GetCooldownRemainingTime(Triplecast) >= 20)
                            return LevelChecked(Xenoglossy)
                                    ? Xenoglossy
                                    : Foul;

                        // Normal Fire phase
                        if (gauge.InAstralFire)
                        {
                            // Xenoglossy overcap protection
                            if ((gauge.PolyglotStacks is 2 && (gauge.EnochianTimer <= 3000) && TraitLevelChecked(Traits.EnhancedPolyGlot)) ||
                                (gauge.PolyglotStacks is 1 && (gauge.EnochianTimer <= 6000) && !TraitLevelChecked(Traits.EnhancedPolyGlot)))
                                return LevelChecked(Xenoglossy)
                                    ? Xenoglossy
                                    : Foul;

                            // Fire III proc or Swiftcast Fire III during Transpose lines(< 3 Astral Fire stacks)
                            if (gauge.AstralFireStacks < 3 || (gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter)))
                                return Fire3;

                            // Use Paradox instead of hardcasting Fire if we can
                            if (gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                                return LevelChecked(Paradox) && gauge.IsParadoxActive
                                    ? Paradox
                                    : Fire;

                            if (Config.BLM_Adv_Cooldowns_Choice[0] &&
                                ActionReady(Manafont) && WasLastAction(Despair))
                                return Manafont;

                            // Cast Fire IV after Manafont
                            if (IsOnCooldown(Manafont) && WasLastAction(Manafont))
                                return Fire4;

                            // Transpose lines Fire phase
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Rotation) && level >= 90 &&
                                !WasLastAction(Manafont) && IsOnCooldown(Manafont) && ActionReady(All.Swiftcast) &&
                                currentMP < MP.Fire && gauge.PolyglotStacks is 2)
                            {
                                if (WasLastAction(Despair))
                                    return Transpose;

                                if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                                    return Thunder3;
                            }

                            // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                            // Only when we're not using Transpose lines 
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Cooldowns) && (IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Rotation) || level < 90) &&
                                ((Config.BLM_Adv_Cooldowns_Choice[3] && ActionReady(LeyLines)) ||
                                (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) && (IsNotEnabled(CustomComboPreset.BLM_Adv_Triplecast_Pooling) || GetRemainingCharges(Triplecast) > 1)) ||
                                (Config.BLM_Adv_Cooldowns_Choice[0] && ActionReady(Manafont) && currentMP < MP.AllMPSpells) ||
                                (Config.BLM_Adv_Cooldowns_Choice[1] && ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast) &&
                                !WasLastAction(Xenoglossy) && gauge.ElementTimeRemaining >= astralFireRefresh &&
                                gauge.PolyglotStacks > pooledPolyglotStacks && LevelChecked(Xenoglossy))))
                                return Xenoglossy;


                            // Blizzard III/Despair when below Fire IV + Despair MP
                            if (currentMP < MP.Fire || gauge.ElementTimeRemaining <= 5000)
                            {
                                return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells)
                                    ? Despair
                                    : Blizzard3;
                            }

                            return Fire4;
                        }
                    }

                    // Normal Ice phase
                    if (gauge.InUmbralIce)
                    {
                        // Xenoglossy overcap protection
                        if (gauge.EnochianTimer <= 20000 &&
                            ((gauge.PolyglotStacks is 2 && TraitLevelChecked(Traits.EnhancedPolyGlot)) ||
                            (gauge.PolyglotStacks is 1 && !TraitLevelChecked(Traits.EnhancedPolyGlot))))
                            return LevelChecked(Xenoglossy)
                                ? Xenoglossy
                                : Foul;

                        // Sharpcast
                        if (Config.BLM_Adv_Cooldowns_Choice[1] &&
                            ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast) &&
                            !WasLastAction(Thunder3) && CanSpellWeave(actionID))
                            return Sharpcast;

                        // Use Paradox when available
                        if (LevelChecked(Paradox) && gauge.IsParadoxActive)
                            return Paradox;


                        // Transpose lines Ice phase
                        if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Rotation) && level >= 90 && ActionReady(All.Swiftcast))
                        {
                            if (gauge.UmbralIceStacks < 3 && ActionReady(All.LucidDreaming))
                                return All.LucidDreaming;

                            if (gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy))
                                return Xenoglossy;

                            if (HasEffect(All.Buffs.LucidDreaming) && currentMP >= MP.MaxMP - MP.Thunder)
                                return All.Swiftcast;

                            if (!gauge.HasPolyglotStacks() && WasLastAction(Xenoglossy) && HasEffect(All.Buffs.Swiftcast))
                                return Transpose;
                        }

                        // Fire III when at max Umbral Hearts
                        return (gauge.UmbralHearts is 3 && currentMP >= MP.MaxMP - MP.Thunder)
                            ? Fire3
                            : Blizzard4;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Blizzard2)
                {
                    uint currentMP = LocalPlayer.CurrentMp;
                    BLMGauge? gauge = GetJobGauge<BLMGauge>();
                    Status? dotDebuff = FindTargetEffect(ThunderList[OriginalHook(Thunder)]); // Match DoT with its debuff ID, and check for the debuff

                    // Spam Umbral Soul/Transpose when there's no target
                    if (CurrentTarget is null && gauge.ElementTimeRemaining > 0)
                    {
                        if (gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // 2xHF2 Transpose with Freeze [A7]
                    if (!InCombat())
                    {
                        return (gauge.UmbralHearts is 3)
                            ? OriginalHook(Fire2)
                            : OriginalHook(Blizzard2);
                    }

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                            return Variant.VariantCure;

                        if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantRampart;
                    }

                    // Fire phase
                    if (gauge.InAstralFire)
                    {
                        // Manafont weave
                        if (ActionReady(Manafont) && currentMP is 0)
                            return Manafont;

                        // Use Flare after Manafont
                        if (IsOnCooldown(Manafont) && WasLastAction(Manafont))
                            return Flare;

                        // Polyglot usage 
                        if (LevelChecked(Foul) && gauge.HasPolyglotStacks() && WasLastAction(OriginalHook(Fire2)))
                            return Foul;

                        if (currentMP >= MP.AllMPSpells)
                        {
                            if (!TraitLevelChecked(Traits.AspectMasteryIII))
                                return Transpose;

                            if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
                                (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE))
                                return Flare;

                            if (currentMP > MP.FireAoE)
                                return OriginalHook(Fire2);
                        }

                        if (currentMP is 0)
                            return Transpose;
                    }

                    // Ice phase
                    if (gauge.InUmbralIce)
                    {
                        if (gauge.UmbralHearts < 3 && LevelChecked(Freeze))
                            return Freeze;

                        if (!ThunderList.ContainsKey(lastComboMove) && LevelChecked(lastComboMove) &&
                            !TargetHasEffect(Debuffs.Thunder) && !TargetHasEffect(Debuffs.Thunder3) &&
                            ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || currentMP >= MP.Thunder) &&
                            (dotDebuff is null || dotDebuff?.RemainingTime <= 4))
                            return OriginalHook(Thunder);

                        if (gauge.UmbralHearts is 3)
                            return OriginalHook(Fire2);
                    }
                }

                return actionID;
            }
        }

        internal class BLM_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Blizzard2)
                {
                    uint currentMP = LocalPlayer.CurrentMp;
                    BLMGauge? gauge = GetJobGauge<BLMGauge>();
                    Status? dotDebuff = FindTargetEffect(ThunderList[OriginalHook(Thunder)]); // Match DoT with its debuff ID, and check for the debuff

                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Adv_UmbralSoul) &&
                        CurrentTarget is null && gauge.ElementTimeRemaining > 0)
                    {
                        if (gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // 2xHF2 Transpose with Freeze [A7]
                    if (!InCombat())
                    {
                        return (gauge.UmbralHearts is 3)
                            ? OriginalHook(Fire2)
                            : OriginalHook(Blizzard2);
                    }

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                            return Variant.VariantCure;

                        if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantRampart;
                    }

                    // Fire phase
                    if (gauge.InAstralFire)
                    {
                        // Manafont weave
                        if (IsEnabled(CustomComboPreset.BLM_AoE_Adv_Manafont) && ActionReady(Manafont) &&
                            currentMP is 0)
                            return Manafont;

                        // Use Flare after Manafont
                        if (IsOnCooldown(Manafont) && WasLastAction(Manafont))
                            return Flare;

                        // Polyglot usage 
                        if (IsEnabled(CustomComboPreset.BLM_AoE_Adv_Foul) &&
                            LevelChecked(Foul) && gauge.HasPolyglotStacks() && WasLastAction(OriginalHook(Fire2)))
                            return Foul;

                        if (currentMP >= MP.AllMPSpells)
                        {
                            if (!TraitLevelChecked(Traits.AspectMasteryIII))
                                return Transpose;

                            if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
                                (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE))
                                return Flare;

                            if (currentMP > MP.FireAoE)
                                return OriginalHook(Fire2);
                        }

                        if (currentMP is 0)
                            return Transpose;
                    }

                    // Ice phase
                    if (gauge.InUmbralIce)
                    {
                        if (gauge.UmbralHearts < 3 && LevelChecked(Freeze))
                            return Freeze;

                        if (!ThunderList.ContainsKey(lastComboMove) && LevelChecked(lastComboMove) &&
                            !TargetHasEffect(Debuffs.Thunder) && !TargetHasEffect(Debuffs.Thunder3) &&
                            ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || currentMP >= MP.Thunder) &&
                            (dotDebuff is null || dotDebuff?.RemainingTime <= 4))
                            return OriginalHook(Thunder);

                        if (gauge.UmbralHearts is 3)
                            return OriginalHook(Fire2);
                    }
                }

                return actionID;
            }
        }

        internal class BLM_Variant_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) =>
                (actionID is All.Swiftcast && HasEffect(All.Buffs.Swiftcast) && IsEnabled(Variant.VariantRaise))
                ? Variant.VariantRaise
                : actionID;
        }

        internal class BLM_Scathe_Xeno : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                (actionID is Scathe && LevelChecked(Xenoglossy) && GetJobGauge<BLMGauge>().HasPolyglotStacks())
                ? Xenoglossy
                : actionID;
        }

        internal class BLM_Blizzard_1to3 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Blizzard && LevelChecked(Freeze) && !GetJobGauge<BLMGauge>().InUmbralIce)
                    return Blizzard3;

                if (actionID is Freeze && !LevelChecked(Freeze))
                    return Blizzard2;

                return actionID;
            }
        }

        internal class BLM_Fire_1to3 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                (actionID is Fire && ((LevelChecked(Fire3) && !GetJobGauge<BLMGauge>().InAstralFire) || HasEffect(Buffs.Firestarter)))
                ? Fire3
                : actionID;
        }

        internal class BLM_Between_The_LeyLines : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_Aetherial_Manipulation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is AetherialManipulation &&
                ActionReady(BetweenTheLines) &&
                HasEffect(Buffs.LeyLines) &&
                !HasEffect(Buffs.CircleOfPower) &&
                !IsMoving
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_UmbralSoul : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Transpose && GetJobGauge<BLMGauge>().InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
        }
    }
}