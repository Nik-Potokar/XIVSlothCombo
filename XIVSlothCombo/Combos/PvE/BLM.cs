using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
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
                EnhancedFreeze = 295;
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
        private static BLMGauge Gauge => CustomComboFunctions.GetJobGauge<BLMGauge>();
        private static bool HasPolyglotStacks(this BLMGauge gauge) => gauge.PolyglotStacks > 0;

        internal static class Config
        {
            internal const string BLM_AstralFireRefresh = "BlmAstralFireRefresh";
            internal const string BLM_MovementTime = "BlmMovementTime";
            internal const string BLM_VariantCure = "BlmVariantCure";
        }

        internal class BLM_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_SimpleMode;

            internal static bool openerFinished = false;
            internal static bool inOpener = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID is Scathe)
                {
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) && 
                        IsEnabled(Variant.VariantCure) && 
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_SimpleUmbralSoul) && 
                        CurrentTarget is null && Gauge.IsEnochianActive)
                    {
                        if (Gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (Gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // Opener for BLM
                    // Credit to damolitionn for providing code to be used as a base for this opener
                    // F3 OPENER DOUBLE TRANSPOSE VARIATION 
                    // Only enable sharpcast if it's available
                    if (!InCombat() && !HasEffect(Buffs.Sharpcast))
                        return Sharpcast;

                    //check to start opener
                    if (!InCombat() &&
                        (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                    }

                    if (InCombat() && !inOpener)
                        inOpener = true;

                    if (InCombat() && inOpener && !openerFinished)
                    {
                        // Exit out of opener if Enochian is lost
                        if (!Gauge.IsEnochianActive)
                        {
                            openerFinished = true;
                            inOpener = false;
                            return Blizzard3;
                        }

                        if (Gauge.InAstralFire)
                        {
                            // Thunder3
                            if (lastComboMove != Thunder3 && !TargetHasEffect(Debuffs.Thunder3))
                                return Thunder3;

                            // First Triplecast
                            if (!HasEffect(Buffs.Triplecast) && (GetRemainingCharges(Triplecast) is 2) && (lastComboMove is Fire4))
                                return Triplecast;

                            // Weave other oGCDs
                            if (CanSpellWeave(actionID))
                            {
                                // Weave Amplifier and Ley Lines
                                if (lastComboMove is Fire4 && (GetBuffStacks(Buffs.Triplecast) is 1))
                                {
                                    if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2)
                                        return Amplifier;

                                    if (ActionReady(LeyLines))
                                        return LeyLines;
                                }
                            }

                            // Manafont  
                            if (ActionReady(Manafont) && (lastComboMove is Despair))
                                return Manafont;

                            // Second Triplecast
                            if ((GetBuffStacks(Buffs.Triplecast) is 0) && (GetRemainingCharges(Triplecast) is 1) && lastComboMove is Fire4)
                                return Triplecast;

                            // Lucid Dreaming
                            if (ActionReady(All.LucidDreaming) && lastComboMove is Fire4 && currentMP < MP.Fire)
                                return All.LucidDreaming;

                            //Sharpcast
                            if (!HasEffect(Buffs.Sharpcast) && ActionReady(Sharpcast) &&
                                IsOnCooldown(Manafont) && lastComboMove is Fire4)
                                return Sharpcast;

                            // Cast Despair
                            if (LevelChecked(Despair) &&
                                (currentMP < MP.Fire || Gauge.ElementTimeRemaining <= 4000) &&
                                currentMP >= MP.AllMPSpells)
                                return Despair;

                            // Cast Fire 4 after Manafont
                            if (IsOnCooldown(Manafont) && (GetCooldownRemainingTime(Manafont) >= 119))
                                return Fire4;

                            //if not at full astralfire stacks
                            if (Gauge.AstralFireStacks < 3 ||
                                (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter)))
                                return Fire3;

                            // Use Transpose lines
                            if (currentMP is 0 && IsOnCooldown(Manafont))
                            {
                                if (ActionReady(All.Swiftcast))
                                    return Transpose;
                                else inOpener = false;
                            }

                            //cast Fire4 until only enough mana for despair
                            if (currentMP >= MP.Fire)
                                return Fire4;
                        }


                        if (Gauge.InUmbralIce)
                        {
                            if (Gauge.IsParadoxActive)
                                return Paradox;

                            if (ActionReady(All.Swiftcast) && lastComboMove is Paradox)
                                return All.Swiftcast;

                            // Dump Polyglot Stacks   
                            if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= 6000)
                            {
                                return LevelChecked(Xenoglossy)
                                    ? Xenoglossy
                                    : Foul;
                            }


                            // Refresh Thunder3             
                            if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3 && HasEffect(Buffs.Sharpcast))
                                return Thunder3;

                            if (lastComboMove is Thunder3)
                                return Transpose;

                            inOpener = false;
                        }
                    }



                    // Handle movement
                    if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement))
                    {
                        if (IsMoving && InCombat())
                        {
                            if (LevelChecked(Paradox) && Gauge.IsParadoxActive && Gauge.InUmbralIce)
                                return Paradox;

                            if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Xeno) &&
                                IsNotEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines) &&
                                LevelChecked(Xenoglossy) && Gauge.HasPolyglotStacks())
                                return Xenoglossy;

                            if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                            {
                                if (!ThunderList.ContainsKey(lastComboMove) && //Is not 1 2 3 or 4
                                    !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                                {
                                    uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                    Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                    if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                        return dot;
                                }
                            }
                            if (IsNotEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines) &&
                                ActionReady(All.Swiftcast))
                                return All.Swiftcast;

                            if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) is 0)
                                return Triplecast;

                            if (HasEffect(Buffs.Firestarter) && Gauge.InAstralFire && LevelChecked(Fire3))
                                return Fire3;

                            if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Scathe) && (GetBuffStacks(Buffs.Triplecast) is 0))
                                return Scathe;
                        }
                    }

                    // Use under Fire or Ice
                    if (Gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder uptime
                        if (Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            if (!ThunderList.ContainsKey(lastComboMove) && !TargetHasEffect(Debuffs.Thunder2) && 
                                !TargetHasEffect(Debuffs.Thunder4) && LevelChecked(lastComboMove))
                            {
                                if ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || 
                                    currentMP >= MP.Thunder)
                                {
                                    uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                    Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                    if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                        return dot; //Use appropriate DoT Action
                                }
                            }
                        }
                        // Weave Buffs
                        if (CanSpellWeave(actionID))
                        {
                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                            (Gauge.InAstralFire || Gauge.UmbralHearts is 3) &&
                            currentMP >= MP.Fire * 2)
                            {
                                if (IsNotEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || 
                                    GetRemainingCharges(Triplecast) is 2)
                                    return Triplecast;
                            }

                            if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2)
                                return Amplifier;

                            if (ActionReady(LeyLines))
                                return LeyLines;
                        }

                        // Transpose Lines Ice phase

                        if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) &&
                            Gauge.InUmbralIce && Gauge.HasPolyglotStacks() && ActionReady(All.Swiftcast))
                        {
                            if (Gauge.UmbralIceStacks < 3 && ActionReady(All.LucidDreaming) && ActionReady(All.Swiftcast))
                                return All.LucidDreaming;

                            if (HasEffect(All.Buffs.LucidDreaming) && ActionReady(All.Swiftcast))
                                return All.Swiftcast;
                        }
                    }

                    // Handle initial cast
                    if (Gauge.ElementTimeRemaining <= 0)
                    {
                        if (LevelChecked(Fire3))
                            return (currentMP >= MP.Fire3)
                                ? Fire3
                                : Blizzard3;
                        return (currentMP >= MP.Fire)
                            ? Fire
                            : Blizzard;
                    }

                    // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                    if (!LevelChecked(Blizzard3))
                    {
                        if (Gauge.InAstralFire)
                            return (currentMP < MP.Fire)
                                ? Transpose
                                : Fire;

                        if (Gauge.InUmbralIce)
                            return (currentMP >= MP.MaxMP - MP.Thunder)
                                ? Transpose
                                : Blizzard;
                    }

                    // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                    if (!LevelChecked(Fire4))
                    {
                        if (Gauge.InAstralFire)
                        {
                            if (HasEffect(Buffs.Firestarter) && (Gauge.ElementTimeRemaining <= 3000))
                                return Fire3;
                            return (currentMP < MP.Fire)
                                ? Blizzard3
                                : Fire;
                        }

                        if (Gauge.InUmbralIce)
                        {
                            if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3)
                                return Blizzard4;
                            return (currentMP >= MP.MaxMP || Gauge.UmbralHearts is 3)
                                ? Fire3
                                : Blizzard;
                        }
                    }

                    // Use polyglot stacks if we don't need it for a future weave
                    // only when we're not using Transpose lines
                    if (IsNotEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines))
                    {
                        if (CanSpellWeave(actionID))
                        {
                            if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= astralFireRefresh &&
                                (Gauge.InUmbralIce || (Gauge.InAstralFire && Gauge.UmbralHearts is 0)))
                            {
                                if (LevelChecked(Xenoglossy))
                                {
                                    // Check leylines and triplecast cooldown
                                    if (Gauge.PolyglotStacks is 2 && GetCooldown(LeyLines).CooldownRemaining >= 20 && 
                                        GetCooldown(Triplecast).ChargeCooldownRemaining >= 20)
                                    {
                                        if (IsNotEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling))
                                            return Xenoglossy;

                                        if (IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) && !HasCharges(Triplecast))
                                            return Xenoglossy;
                                    }
                                }
                                else if (LevelChecked(Foul))
                                    return Foul;
                            }
                        }
                    }

                    //Normal Fire Phase
                    if (Gauge.InAstralFire)
                    {
                        //xenoglossy overcap protection
                        if (Gauge.PolyglotStacks is 2 && (Gauge.EnochianTimer <= 3000) && LevelChecked(Xenoglossy))
                            return Xenoglossy;

                        // F3 proc or swiftcast F3 during transpose lines(< 3 astral fire stacks)
                        if (Gauge.AstralFireStacks < 3 || (Gauge.ElementTimeRemaining <= 3000 &&
                            HasEffect(Buffs.Firestarter)))
                            return Fire3;

                        // Use Paradox instead of hardcasting Fire3 if we can
                        if (Gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                        {
                            if (LevelChecked(Paradox))
                                return Gauge.IsParadoxActive
                                    ? Paradox
                                    : Despair;

                            return Fire;
                        }

                        if (ActionReady(Manafont) && lastComboMove is Despair)
                            return Manafont;

                        // Cast Fire 4 after Manafont
                        if (ActionReady(Manafont) && (GetCooldownRemainingTime(Manafont) >= 179) || (GetCooldownRemainingTime(Manafont) >= 119))
                            return Fire4;

                        // Double Transpose Line during normal rotation every min Swiftcast is up!
                        if (IsEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines))
                        {
                            if (currentMP < MP.Fire && lastComboMove != Manafont && IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) <= 118)
                            {
                                if (ActionReady(All.Swiftcast))
                                {
                                    if ((lastComboMove is Despair) && Gauge.PolyglotStacks is 2)
                                        return Transpose;

                                    if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                                        return Thunder3;
                                }
                            }
                        }

                        // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                        // only when we're not using Transpose Lines 
                        if (IsNotEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines) &&
                            lastComboMove != Xenoglossy && Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy) &&
                            Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            var pooledPolyglotStacks = IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Xeno) ? 1 : 0;

                            if (ActionReady(Amplifier))
                                return Xenoglossy;

                            if (Gauge.PolyglotStacks > pooledPolyglotStacks)
                            {
                                if (ActionReady(LeyLines))
                                    return Xenoglossy;

                                if (IsNotEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines))
                                {
                                    if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                        (IsNotEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1))
                                        return Xenoglossy;

                                    if (ActionReady(Manafont) && currentMP < MP.AllMPSpells)
                                        return Xenoglossy;

                                    if (ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast))
                                        return Xenoglossy;
                                }
                            }

                            // Xenoglossy for Manafont weave
                            if (Gauge.HasPolyglotStacks() && ActionReady(Manafont) && currentMP < MP.AllMPSpells && LevelChecked(Xenoglossy) &&
                                IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                return Xenoglossy;
                        }

                        // Blizzard3/Despair when below Fire 4 + Despair MP
                        if (currentMP < (MP.Fire + MP.AllMPSpells))
                            return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells)
                                ? Despair
                                : Blizzard3;

                        return Fire4;
                    }

                    //Normal Ice Phase
                    if (Gauge.InUmbralIce)
                    {
                        //Xenoglossy overcap protection
                        if (Gauge.PolyglotStacks is 2 && (Gauge.EnochianTimer <= 20000) && LevelChecked(Xenoglossy))
                            return Xenoglossy;

                        //sharpcast
                        if (ActionReady(Sharpcast) && lastComboMove != Thunder3 && !HasEffect(Buffs.Sharpcast))
                            return Sharpcast;

                        // Use Paradox when available
                        if (LevelChecked(Paradox) && Gauge.IsParadoxActive)
                            return Paradox;
                        if (IsEnabled(CustomComboPreset.BLM_Simple_Transpose_Lines))
                        {
                            // Transpose lines will use 2 xenoglossy stacks and then transpose
                            if (HasEffect(All.Buffs.LucidDreaming) && Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy))
                                return Xenoglossy;

                            if (HasEffect(All.Buffs.LucidDreaming) && lastComboMove is Xenoglossy && (Gauge.PolyglotStacks is 0))
                                return Transpose;
                        }

                        // Fire3 when at max umbral hearts
                        return (Gauge.UmbralHearts is 3 && currentMP >= MP.MaxMP - MP.Thunder)
                            ? Fire3
                            : Blizzard4;
                    }
                }
                return actionID;
            }
        }

        internal class BLM_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AdvancedMode;
            internal static bool inOpener = false;
            internal static byte step = 0;


            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;
                    bool openerReady = ActionReady(Manafont) && ActionReady(Amplifier) && ActionReady(LeyLines);

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BLM_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_AdvUmbralSoul) &&
                        CurrentTarget is null && Gauge.IsEnochianActive)
                    {
                        if (Gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (Gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    // Opener for BLM
                    // F3 OPENER DOUBLE TRANSPOSE VARIATION 
                    if (!InCombat() && IsEnabled(CustomComboPreset.BLM_Adv_Opener) && level >= 90)
                    {
                        inOpener = false;

                        if (HasEffect(Buffs.Sharpcast) && openerReady)
                            inOpener = true;

                        if (inOpener)
                            return Fire3;

                        return Sharpcast;
                    }

                    if (InCombat())
                    {
                        if (CombatEngageDuration().TotalSeconds < 10 && HasEffect(Buffs.Sharpcast) &&
                            IsEnabled(CustomComboPreset.BLM_Adv_Opener) && level >= 90 && openerReady)
                            inOpener = true;
                      
                        // Reset if opener is interrupted, requires step 0 and 1 to be explicit since the inCombat check can be slow
                        if ((step == 0 && lastComboMove is Fire3 && !HasEffect(Buffs.Sharpcast))
                            || (inOpener && step >= 1 && IsOffCooldown(actionID) && !InCombat())) 
                            inOpener = false;

                        if (inOpener)
                        {
                            //we do it in steps to be able to control it
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
                                if (Gauge.IsParadoxActive) step++;
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
                                if (Gauge.InAstralFire && currentMP == MP.MaxMP) step++;
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


                        if (!inOpener)
                        {
                            // Handle movement
                            if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement))
                            {
                                if (IsMoving && InCombat())
                                {
                                    if (HasEffect(Buffs.Firestarter) && Gauge.InAstralFire && LevelChecked(Fire3))
                                        return Fire3;

                                    if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                                    {
                                        if (!ThunderList.ContainsKey(lastComboMove) && //Is not 1 2 3 or 4
                                            !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                                        {
                                            uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                            Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                            if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                                return dot;
                                        }
                                    }

                                    if (LevelChecked(Paradox) && Gauge.IsParadoxActive && Gauge.InUmbralIce)
                                        return Paradox;

                                    if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Xeno) &&
                                        IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) &&
                                        LevelChecked(Xenoglossy) && Gauge.HasPolyglotStacks())
                                        return Xenoglossy;

                                    if (IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) &&
                                        ActionReady(All.Swiftcast))
                                        return All.Swiftcast;

                                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) is 0)
                                        return Triplecast;

                                    if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Scathe) && (GetBuffStacks(Buffs.Triplecast) is 0) && !Gauge.HasPolyglotStacks())
                                        return Scathe;
                                }
                            }

                            // Use under Fire or Ice
                            if (Gauge.ElementTimeRemaining > 0)
                            {
                                // Thunder uptime
                                if (IsEnabled(CustomComboPreset.BLM_AdvThunder) &&
                                    Gauge.ElementTimeRemaining >= astralFireRefresh)
                                {
                                    if (!ThunderList.ContainsKey(lastComboMove) && !TargetHasEffect(Debuffs.Thunder2) &&
                                        !TargetHasEffect(Debuffs.Thunder4) && LevelChecked(lastComboMove))
                                    {
                                        if (IsEnabled(CustomComboPreset.BLM_AdvThunderUptime) &&
                                            ((HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast)) || currentMP >= MP.Thunder))
                                        {
                                            uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                            Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff
                                            if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                                return dot; //Use appropriate DoT Action
                                        }
                                    }
                                }
                                // Weave Buffs

                                if (IsEnabled(CustomComboPreset.BLM_Adv_Casts))
                                {
                                    // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice

                                    if ((IsNotEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) || GetRemainingCharges(Triplecast) is 2) &&
                                        ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                        (Gauge.InAstralFire || Gauge.UmbralHearts is 3) &&
                                        currentMP >= MP.Fire * 2)
                                        return Triplecast;

                                    if (CanSpellWeave(actionID))
                                    {
                                        if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2)
                                            return Amplifier;

                                        if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs_LeyLines) &&
                                            ActionReady(LeyLines))
                                            return LeyLines;
                                    }

                                    // Transpose Lines Ice phase
                                    if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) &&
                                        Gauge.InUmbralIce && Gauge.HasPolyglotStacks() && ActionReady(All.Swiftcast))
                                    {
                                        if (Gauge.UmbralIceStacks < 3 &&
                                            ActionReady(All.LucidDreaming) && ActionReady(All.Swiftcast))
                                            return All.LucidDreaming;

                                        if (HasEffect(All.Buffs.LucidDreaming) && ActionReady(All.Swiftcast))
                                            return All.Swiftcast;
                                    }
                                }
                            }

                            // Handle initial cast
                            if (Gauge.ElementTimeRemaining <= 0)
                            {
                                if (LevelChecked(Fire3))
                                    return (currentMP >= MP.Fire3)
                                        ? Fire3
                                        : Blizzard3;

                                return (currentMP >= MP.Fire)
                                    ? Fire
                                    : Blizzard;
                            }

                            // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                            if (!LevelChecked(Blizzard3))
                            {
                                if (Gauge.InAstralFire)
                                    return (currentMP < MP.Fire)
                                        ? Transpose
                                        : Fire;

                                if (Gauge.InUmbralIce)
                                    return (currentMP >= MP.MaxMP - MP.Thunder)
                                        ? Transpose
                                        : Blizzard;
                            }

                            // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                            if (!LevelChecked(Fire4))
                            {
                                if (Gauge.InAstralFire)
                                {
                                    if (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter))
                                        return Fire3;

                                    return (currentMP < MP.Fire)
                                        ? Blizzard3
                                        : Fire;
                                }

                                if (Gauge.InUmbralIce)
                                {
                                    if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3)
                                        return Blizzard4;

                                    return (currentMP >= MP.MaxMP || Gauge.UmbralHearts is 3)
                                        ? Fire3
                                        : Blizzard;
                                }
                            }

                            // Use polyglot stacks if we don't need it for a future weave
                            // only when we're not using Transpose lines
                            if (IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                            {
                                if (CanSpellWeave(actionID))
                                {
                                    if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= astralFireRefresh &&
                                        (Gauge.InUmbralIce || (Gauge.InAstralFire && Gauge.UmbralHearts is 0)))
                                    {
                                        if (LevelChecked(Xenoglossy))
                                        {
                                            // Check leylines and triplecast cooldown
                                            if (Gauge.PolyglotStacks is 2 && GetCooldown(LeyLines).CooldownRemaining >= 20 && GetCooldown(Triplecast).ChargeCooldownRemaining >= 20)
                                            {
                                                if (IsNotEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling))
                                                    return Xenoglossy;

                                                if (IsEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) && !HasCharges(Triplecast))
                                                    return Xenoglossy;
                                            }
                                        }
                                        else if (LevelChecked(Foul))
                                            return Foul;
                                    }
                                }
                            }

                            //Normal Fire Phase
                            if (Gauge.InAstralFire)
                            {
                                //xenoglossy overcap protection
                                if (Gauge.PolyglotStacks is 2 && (Gauge.EnochianTimer <= 3000) && LevelChecked(Xenoglossy))
                                    return Xenoglossy;

                                // F3 proc or swiftcast F3 during transpose lines(< 3 astral fire stacks)
                                if (Gauge.AstralFireStacks < 3 || (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter)))
                                    return Fire3;

                                // Use Paradox instead of hardcasting Fire3 if we can
                                if (Gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                                {
                                    if (LevelChecked(Paradox))
                                        return Gauge.IsParadoxActive
                                            ? Paradox
                                            : Despair;
                                    return Fire;
                                }

                                if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs)
                                    && ActionReady(Manafont) && lastComboMove is Despair)
                                    return Manafont;

                                // Cast Fire 4 after Manafont
                                if (IsOnCooldown(Manafont) &&
                                    (GetCooldownRemainingTime(Manafont) >= 179) || (GetCooldownRemainingTime(Manafont) >= 119))
                                    return Fire4;

                                // Double Transpose Line during normal rotation every min Swiftcast is up!
                                if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                {
                                    if (currentMP < MP.Fire && lastComboMove != Manafont &&
                                        IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) <= 118)
                                    {
                                        if (ActionReady(All.Swiftcast) && (Gauge.PolyglotStacks is 2))
                                        {
                                            if ((lastComboMove is Despair) && Gauge.PolyglotStacks is 2)
                                                return Transpose;

                                            if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                                                return Thunder3;
                                        }
                                    }
                                }

                                // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                                // only when we're not using Transpose Lines 
                                if (IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) &&
                                    lastComboMove != Xenoglossy && LevelChecked(Xenoglossy) && Gauge.ElementTimeRemaining >= astralFireRefresh)
                                {
                                    var pooledPolyglotStacks = IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Xeno) ? 1 : 0;

                                    //  if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs) && ActionReady(Amplifier))
                                    //      return Xenoglossy;

                                    if (Gauge.PolyglotStacks > pooledPolyglotStacks)
                                    {
                                        if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs_LeyLines))
                                        {
                                            if (ActionReady(LeyLines))
                                                return Xenoglossy;
                                        }
                                        if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs) &&
                                            IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                        {
                                            if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                                (IsNotEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1))
                                                return Xenoglossy;

                                            if (ActionReady(Manafont) && currentMP < MP.AllMPSpells)
                                                return Xenoglossy;

                                            if (ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast))
                                                return Xenoglossy;
                                        }
                                    }

                                    // Xenoglossy for Manafont weave
                                    if (Gauge.HasPolyglotStacks() && ActionReady(Manafont) &&
                                        currentMP < MP.AllMPSpells && LevelChecked(Xenoglossy) &&
                                        IsNotEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                        return Xenoglossy;
                                }

                                // Blizzard3/Despair when below Fire 4 + Despair MP
                                if (currentMP < MP.Fire || Gauge.ElementTimeRemaining <= 4000)
                                {
                                    return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells)
                                        ? Despair
                                        : Blizzard3;
                                }
                                return Fire4;
                            }

                            //Normal Ice Phase
                            if (Gauge.InUmbralIce)
                            {
                                //Xenoglossy overcap protection
                                if (Gauge.PolyglotStacks is 2 && (Gauge.EnochianTimer <= 20000) && LevelChecked(Xenoglossy))
                                    return Xenoglossy;

                                //sharpcast
                                if (ActionReady(Sharpcast) && lastComboMove != Thunder3 && !HasEffect(Buffs.Sharpcast))
                                    return Sharpcast;

                                // Use Paradox when available
                                if (LevelChecked(Paradox) && Gauge.IsParadoxActive)
                                    return Paradox;

                                if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                {
                                    // Transpose lines will use 2 xenoglossy stacks and then transpose
                                    if (HasEffect(All.Buffs.LucidDreaming) && Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy))
                                        return Xenoglossy;

                                    if (HasEffect(All.Buffs.LucidDreaming) && (Gauge.PolyglotStacks is 0) && lastComboMove is Xenoglossy)
                                        return Transpose;

                                    if (!LevelChecked(Xenoglossy) && LevelChecked(Foul))
                                        return Foul;
                                }

                                // Fire3 when at max umbral hearts
                                return (Gauge.UmbralHearts is 3 && currentMP >= MP.MaxMP - MP.Thunder)
                                    ? Fire3
                                    : Blizzard4;
                            }
                        }
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
                    var currentMP = LocalPlayer.CurrentMp;

                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_AoEUmbralSoul) &&
                        CurrentTarget is null && Gauge.IsEnochianActive)
                    {
                        if (Gauge.InAstralFire && LevelChecked(Transpose))
                            return Transpose;

                        if (Gauge.InUmbralIce && LevelChecked(UmbralSoul))
                            return UmbralSoul;
                    }

                    //2xHF2 Transpose with Freeze [A7]
                    if (!InCombat())
                        return OriginalHook(Blizzard2);

                    if (InCombat())
                    {
                        if (Gauge.ElementTimeRemaining > 0)
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
                        if (Gauge.InAstralFire)
                        {
                            //Grab Fire 2 / High Fire 2 action ID
                            if (Gauge.UmbralHearts is 1 && LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare))
                                return Flare;

                            // Polyglot usage 
                            if (IsEnabled(CustomComboPreset.BLM_AoE_Simple_Foul) &&
                                LevelChecked(Foul) && Gauge.HasPolyglotStacks() && lastComboMove is Flare)
                                return Foul;
                            

                            if (currentMP >= MP.AllMPSpells)
                            {
                                if (currentMP >= MP.FireAoE || !HasEffect(Buffs.EnhancedFlare))
                                    return OriginalHook(Fire2);

                                else if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare))
                                    return Flare;

                                else if (!TraitLevelChecked(Traits.AspectMasteryIII))
                                    return Transpose;
                            }

                            if (currentMP < MP.AllMPSpells)
                                return Transpose;
                        }

                        // Ice phase
                        if (Gauge.InUmbralIce)
                        {
                            if (Gauge.UmbralHearts < 3)
                                return Freeze;

                            if (lastComboMove is Transpose)
                                return OriginalHook(Thunder2);

                            return (Gauge.UmbralHearts is 3 && currentMP is MP.MaxMP)
                                ? OriginalHook(Fire2)
                                : OriginalHook(Blizzard2);
                        }
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

        internal class BLM_ScatheXeno : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ScatheXeno;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                (actionID is Scathe && LevelChecked(Xenoglossy) && Gauge.HasPolyglotStacks())
                ? Xenoglossy
                : actionID;
        }

        internal class BLM_Blizzard : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Blizzard && LevelChecked(Freeze) && !Gauge.InUmbralIce)
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
                (actionID is Fire && ((LevelChecked(Fire3) && !Gauge.InAstralFire) || HasEffect(Buffs.Firestarter)))
                ? Fire3
                : actionID;
        }

        internal class BLM_LeyLines : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_LeyLines;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_AetherialManipulation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AetherialManipulation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is AetherialManipulation &&
                ActionReady(BetweenTheLines) &&
                HasEffect(Buffs.LeyLines) &&
                !HasEffect(Buffs.CircleOfPower) &&
                !IsMoving
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_Mana : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Mana;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Transpose && Gauge.InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
        }
    }
}