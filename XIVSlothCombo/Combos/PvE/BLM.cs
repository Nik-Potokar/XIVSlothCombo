using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Gui;
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
            internal const string BLM_PolyglotsStored = "BlmPolyglotsStored   ";
            internal const string BLM_AstralFireRefresh = "BlmAstralFireRefresh   ";
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

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Fire && ((LevelChecked(Fire3) && !Gauge.InAstralFire) || HasEffect(Buffs.Firestarter)))
                    return Fire3;

                return actionID;
            }
        }

        internal class BLM_LeyLines : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_LeyLines;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines) ? BetweenTheLines : actionID;
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
                ? BetweenTheLines : actionID;
        }

        internal class BLM_Mana : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Mana;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Transpose && Gauge.InUmbralIce && LevelChecked(UmbralSoul) ? UmbralSoul : actionID;
        }

        internal class BLM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Flare)
                {
                    var currentMP = LocalPlayer.CurrentMp;
                    var polyToStore = PluginConfiguration.GetCustomIntValue(Config.BLM_PolyglotsStored);

                    // Only enable sharpcast if it's available
                    if (!HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && lastComboMove != Thunder3)
                    {
                        return Sharpcast;
                    }

                    // Polyglot usage
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Simple_Manafont) && LevelChecked(Manafont))
                    {
                        if (Gauge.InAstralFire && currentMP <= MP.AllMPSpells && IsOffCooldown(Manafont))
                        {
                            return Manafont;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Simple_Foul) && LevelChecked(Foul))
                    {
                        if ((Gauge.InAstralFire && currentMP <= MP.FireAoE && Gauge.HasPolyglotStacks()))
                        {
                            return Foul;
                        }
                    }

                    // Thunder uptime
                    if (currentMP >= MP.ThunderAoE && lastComboMove != Manafont)
                    {
                        uint thunderAOE = OriginalHook(Thunder2); //Grab whichever Thunder AoE player can use
                        if (ThunderList.TryGetValue(thunderAOE, out ushort dotDebuffID))
                        {
                            var thunderAOEDebuff = TargetHasEffect(dotDebuffID);
                            var thunderAOETimer = FindTargetEffect(dotDebuffID);

                            if (LevelChecked(thunderAOE))
                            {
                                if (lastComboMove != thunderAOE && (!thunderAOEDebuff || thunderAOETimer.RemainingTime <= 7) &&
                                   ((Gauge.InUmbralIce && Gauge.UmbralHearts == 3) ||
                                    (Gauge.InAstralFire && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))))
                                {
                                    return thunderAOE;
                                }
                            }
                        }
                    }

                    // Fire 2 / Flare
                    if (Gauge.InAstralFire)
                    {
                        //use Flare after manafont
                        if (IsOnCooldown(Manafont))
                        {
                            if ((!TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 179) ||
                                (TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 119))
                            {
                                return OriginalHook(Flare);
                            }
                        }

                        //Grab Fire 2 / High Fire 2 action ID
                        if (Gauge.UmbralHearts == 1 && LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare))
                        {
                            return Flare;
                        }
                        if (currentMP >= MP.AllMPSpells)
                        {
                            if (currentMP >= MP.FireAoE || !HasEffect(Buffs.EnhancedFlare))
                            {
                                return OriginalHook(Fire2);
                            }
                            else if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare))
                            {
                                return Flare;
                            }
                            else if (!TraitLevelChecked(Traits.AspectMasteryIII))
                            {
                                return Transpose;
                            }
                        }
                    }

                    // Umbral Hearts
                    if (Gauge.InUmbralIce)
                    {
                        if (TraitLevelChecked(Traits.EnhancedFreeze) && Gauge.UmbralHearts <= 2)
                        {
                            return Freeze;
                        }
                        else if (LevelChecked(Freeze) && currentMP < (MP.MaxMP - MP.ThunderAoE))
                        {
                            return Freeze;
                        }
                        if (!TraitLevelChecked(Traits.AspectMasteryIII))
                        {
                            return (currentMP >= (MP.MaxMP - MP.ThunderAoE)) ? Transpose : Blizzard2;
                        }
                        if (LevelChecked(Fire2)) return OriginalHook(Fire2);
                    }

                    if (LevelChecked(Blizzard2)) return OriginalHook(Blizzard2);
                }

                return actionID;
            }
        }

        internal class BLM_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_SimpleMode;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    var canWeave = CanSpellWeave(actionID);
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;

                    var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Opener for BLM
                    // Credit to damolitionn for providing code to be used as a base for this opener

                    // Only enable sharpcast if it's available
                    if (!inOpener && !HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && lastComboMove != Thunder3)
                    {
                        return Sharpcast;
                    }

                    if (!InCombat() && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                    }

                    if (InCombat() && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (InCombat() && inOpener && !openerFinished)
                    {
                        // Exit out of opener if u died
                        if (HasEffect(All.Buffs.Weakness))
                        {
                            openerFinished = true;
                            return Blizzard3;
                        }

                        if (Gauge.InAstralFire)
                        {

                            //thunder3
                            if (lastComboMove != Thunder3 && !TargetHasEffect(Debuffs.Thunder3))
                            {
                                return Thunder3;
                            }
                            // First Triplecast
                            if (lastComboMove != Triplecast && !HasEffect(Buffs.Triplecast) && HasCharges(Triplecast) && (lastComboMove == OriginalHook(Thunder)))
                            {
                                return Triplecast;
                            }

                            // Weave other oGCDs
                            if (canWeave)
                            {
                                // Weave Amplifier and Ley Lines
                                if (lastComboMove == Fire4 && (GetBuffStacks(Buffs.Triplecast) == 1))
                                {
                                    if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2)
                                    {
                                        return Amplifier;
                                    }
                                    if (ActionReady(LeyLines))
                                    {
                                        return LeyLines;
                                    }
                                }

                                // Swiftcast
                                if (IsOffCooldown(All.Swiftcast) && IsOnCooldown(LeyLines))
                                {
                                    return All.Swiftcast;
                                }

                                // Manafont
                                if (IsOffCooldown(Manafont) && (lastComboMove == Despair || lastComboMove == Fire))
                                {
                                    if (LevelChecked(Despair))
                                    {
                                        if (currentMP < MP.AllMPSpells)
                                        {
                                            return Manafont;
                                        }
                                    }
                                    else if (currentMP < MP.Fire)
                                    {
                                        return Manafont;
                                    }
                                }

                                // Second Triplecast / Sharpcast
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate) && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) && lastComboMove != All.Swiftcast && HasCharges(Triplecast) && currentMP < MP.Fire)
                                {
                                    return Triplecast;
                                }

                                if (IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate) && !HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && IsOnCooldown(Manafont) && lastComboMove == Fire4)
                                {
                                    return Sharpcast;
                                }
                            }

                            // Cast Despair
                            if (LevelChecked(Despair) && (currentMP < MP.Fire || Gauge.ElementTimeRemaining <= 4000) && currentMP >= MP.AllMPSpells)
                            {
                                return Despair;
                            }

                            // Cast Fire
                            if (!LevelChecked(Despair) && Gauge.ElementTimeRemaining <= 6000 && currentMP >= MP.Fire)
                            {
                                return Fire;
                            }

                            // Cast Fire 4 after Manafont
                            if (IsOnCooldown(Manafont))
                            {
                                if ((!TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 179) ||
                                    (TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 119))
                                {
                                    return Fire4;
                                }
                            }

                            // Fire4 / Umbral Ice
                            return currentMP >= MP.Fire ? Fire4 : Blizzard3;
                        }
                        if (IsEnabled(CustomComboPreset.BLM_SimpleUmbralSoul) && CurrentTarget is null && Gauge.IsEnochianActive)
                        {
                            if (Gauge.InAstralFire && LevelChecked(Transpose))
                            {
                                return Transpose;
                            }
                            if (LevelChecked(UmbralSoul))
                            {
                                return UmbralSoul;
                            }

                        }

                        if (Gauge.InUmbralIce)
                        {
                            // Dump Polyglot Stacks
                            if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= 6000)
                            {
                                return LevelChecked(Xenoglossy) ? Xenoglossy : Foul;
                            }
                            if (Gauge.IsParadoxActive && LevelChecked(Paradox))
                            {
                                return Paradox;
                            }
                            if (Gauge.UmbralHearts < 3 && lastComboMove != Blizzard4)
                            {
                                return Blizzard4;
                            }

                            // Refresh Thunder3
                            if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                            {
                                return Thunder3;
                            }

                            openerFinished = true;
                        }
                    }

                    // Handle movement
                    if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement) && InCombat())
                    {
                        if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                        {
                            if (IsMoving)
                            {
                                if (LevelChecked(Paradox) && Gauge.IsParadoxActive && Gauge.InUmbralIce)
                                {
                                    return Paradox;
                                }
                                if (LevelChecked(Xenoglossy) && Gauge.HasPolyglotStacks())
                                {
                                    return Xenoglossy;
                                }
                                if (HasEffect(Buffs.Thundercloud))
                                {
                                    if (!ThunderList.ContainsKey(lastComboMove) && //Is not 1 2 3 or 4
                                        !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                                    {
                                        uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                        Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                        if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                            return dot; //Use appropriate DoT Action
                                    }
                                }
                                if (IsOffCooldown(All.Swiftcast))
                                {
                                    return All.Swiftcast;
                                }
                                if (ActionReady(Triplecast))
                                {
                                    return Triplecast;
                                }
                                if (HasEffect(Buffs.Firestarter) && Gauge.InAstralFire)
                                {
                                    return Fire3;
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_CastMovement_Scathe))
                                {
                                    return Scathe;
                                }
                            }
                        }
                    }

                    // Handle thunder uptime and buffs
                    if (Gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder uptime
                        if (Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            if (!ThunderList.ContainsKey(lastComboMove) &&
                                !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                            {
                                if (HasEffect(Buffs.Thundercloud) && currentMP >= MP.Thunder)
                                {
                                    uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                    Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                    if (dotDebuff is null || dotDebuff?.RemainingTime <= 3)
                                        return dot; //Use appropriate DoT Action
                                }
                            }
                        }

                        // Buffs
                        if (canWeave)
                        {

                            // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                            if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                (Gauge.InAstralFire || Gauge.UmbralHearts == 3) && currentMP >= MP.Fire * 2)
                            {
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1)
                                {
                                    return Triplecast;
                                }
                            }

                            // Use Swiftcast in Astral Fire
                            if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) && ActionReady(All.Swiftcast) &&
                                 Gauge.InAstralFire && currentMP >= MP.Fire * (HasEffect(Buffs.Triplecast) ? 3 : 1))
                            {
                                if (LevelChecked(Despair) && currentMP >= MP.AllMPSpells)
                                {
                                    return All.Swiftcast;
                                }
                                else if (currentMP >= MP.Fire)
                                {
                                    return All.Swiftcast;
                                }
                            }

                            if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2 && LevelChecked(Amplifier))
                            {
                                return Amplifier;
                            }

                            if (ActionReady(LeyLines) && LevelChecked(LeyLines))
                            {
                                return LeyLines;
                            }

                            if (IsOffCooldown(Manafont) && Gauge.InAstralFire)
                            {
                                if (LevelChecked(Despair))
                                {
                                    if (currentMP < MP.AllMPSpells)
                                    {
                                        return Manafont;
                                    }
                                }
                                else if (currentMP < MP.Fire)
                                {
                                    return Manafont;
                                }
                            }
                            if (ActionReady(Sharpcast) && lastComboMove != Thunder3 && !HasEffect(Buffs.Sharpcast))
                            {
                                {
                                    return Sharpcast;
                                }
                            }
                        }
                    }

                    // 20220906 Cleanup Note, could use OriginalHook

                    // Handle initial cast
                    if ((LevelChecked(Blizzard3) && !Gauge.IsEnochianActive) || Gauge.ElementTimeRemaining <= 0)
                    {
                        if (LevelChecked(Fire3))
                        {
                            return (currentMP >= MP.Fire3) ? Fire3 : Blizzard3;
                        }
                        return (currentMP >= MP.Fire) ? Fire : Blizzard;
                    }

                    // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                    if (!LevelChecked(Blizzard3))
                    {
                        if (Gauge.InAstralFire)
                        {
                            return (currentMP < MP.Fire) ? Transpose : Fire;
                        }
                        if (Gauge.InUmbralIce)
                        {
                            return (currentMP >= MP.MaxMP - MP.Thunder) ? Transpose : Blizzard;
                        }
                    }

                    // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                    if (!LevelChecked(Fire4))
                    {
                        if (Gauge.InAstralFire)
                        {
                            if (HasEffect(Buffs.Firestarter))
                            {
                                return Fire3;
                            }
                            return (currentMP < MP.Fire) ? Blizzard3 : Fire;
                        }
                        if (Gauge.InUmbralIce)
                        {
                            if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3)
                            {
                                return Blizzard4;
                            }
                            return (currentMP >= MP.MaxMP || Gauge.UmbralHearts == 3) ? Fire3 : Blizzard;
                        }
                    }

                    // Use polyglot stacks if we don't need it for a future weave
                    if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= astralFireRefresh && (Gauge.InUmbralIce || (Gauge.InAstralFire && Gauge.UmbralHearts == 0)))
                    {
                        if (LevelChecked(Xenoglossy))
                        {
                            // Check leylines and triplecast cooldown
                            if (Gauge.PolyglotStacks == 2 && GetCooldown(LeyLines).CooldownRemaining >= 20 && GetCooldown(Triplecast).ChargeCooldownRemaining >= 20 && !thunder3Recast(15))
                            {
                                if (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling))
                                {
                                    return Xenoglossy;
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) && !HasCharges(Triplecast))
                                {
                                    return Xenoglossy;
                                }
                            }
                        }
                        else if (LevelChecked(Foul))
                        {
                            return Foul;
                        }
                    }

                    if (Gauge.InAstralFire)
                    {
                        // Refresh AF
                        if (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter))
                        {
                            return Fire3;
                        }
                        if (Gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                        {
                            if (LevelChecked(Paradox))
                            {
                                return Gauge.IsParadoxActive ? Paradox : Despair;
                            }
                            return Fire;
                        }

                        // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                        if (lastComboMove != Xenoglossy && Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy) && Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            var pooledPolyglotStacks = IsEnabled(CustomComboPreset.BLM_Simple_XenoPooling) ? 1 : 0;
                            if (ActionReady(Amplifier))
                            {
                                return Xenoglossy;
                            }
                            if (Gauge.PolyglotStacks > pooledPolyglotStacks)
                            {
                                if (ActionReady(LeyLines))
                                {
                                    return Xenoglossy;
                                }

                                if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                    (!IsEnabled(CustomComboPreset.BLM_Simple_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1))
                                {
                                    return Xenoglossy;
                                }
                                if (ActionReady(Manafont) && currentMP < MP.AllMPSpells)
                                {
                                    return Xenoglossy;
                                }
                                if (ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast) &&
                                    lastComboMove != Thunder3)
                                {
                                    return Xenoglossy;
                                }
                            }
                        }

                        // Cast Fire 4 after Manafont
                        if (IsOnCooldown(Manafont))
                        {
                            if ((!TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 179) ||
                                (TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 119))
                            {
                                return Fire4;
                            }
                        }

                        // Blizzard3/Despair when below Fire 4 + Despair MP
                        if (currentMP < (MP.Fire + MP.AllMPSpells))
                        {
                            return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells) ? Despair : Blizzard3;
                        }

                        return Fire4;
                    }

                    if (Gauge.InUmbralIce)
                    {
                        // Use Paradox when available
                        if (LevelChecked(Paradox) && Gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }

                        // Fire3 when at max umbral hearts
                        return (Gauge.UmbralHearts == 3 && currentMP >= MP.MaxMP - MP.Thunder) ? Fire3 : Blizzard4;
                    }


                }

                return actionID;
            }
        }

        internal class BLM_AdvancedMode : CustomCombo
        {

            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AdvancedMode;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;
            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID is Scathe)
                {
                    var canWeave = CanSpellWeave(actionID);
                    var canDelayedWeave = CanWeave(actionID, 0.0) && GetCooldown(actionID).CooldownRemaining < 0.7;
                    var currentMP = LocalPlayer.CurrentMp;
                    var astralFireRefresh = PluginConfiguration.GetCustomFloatValue(Config.BLM_AstralFireRefresh) * 1000;
                    //   var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    //   var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    /*   DotRecast thunder3Recast = delegate (int duration)
                       {
                           return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                       };*/


                    // Spam Umbral Soul/Transpose when there's no target
                    if (IsEnabled(CustomComboPreset.BLM_AdvUmbralSoul) && CurrentTarget is null && Gauge.IsEnochianActive)
                    {
                        if (Gauge.InAstralFire && LevelChecked(Transpose))
                        {
                            return Transpose;
                        }
                        if (LevelChecked(UmbralSoul))
                        {
                            return UmbralSoul;
                        }
                    }

                    // Opener for BLM
                    // Credit to damolitionn for providing code to be used as a base for this opener
                    if (IsEnabled(CustomComboPreset.BLM_Adv_Opener) && LevelChecked(Xenoglossy))
                    {
                        // Only enable sharpcast if it's available
                        if (!inOpener && !HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && lastComboMove != Thunder3 && LevelChecked(Sharpcast))
                        {
                            return Sharpcast;
                        }

                        if (!InCombat() && (inOpener || openerFinished))
                        {
                            inOpener = false;
                            openerFinished = false;
                        }

                        if (InCombat() && !inOpener)
                        {
                            inOpener = true;
                        }

                        if (InCombat() && inOpener && !openerFinished)
                        {
                            // Exit out of opener if u died
                            if (HasEffect(All.Buffs.Weakness))
                            {
                                openerFinished = true;
                                return Blizzard3;
                            }

                            if (Gauge.InAstralFire)
                            {
                                // Thunder3
                                if (lastComboMove != Thunder3 && !TargetHasEffect(Debuffs.Thunder3))
                                {
                                    return Thunder3;
                                }

                                // First Triplecast
                                if (!HasEffect(Buffs.Triplecast) && HasCharges(Triplecast) && (lastComboMove == Fire4) && LevelChecked(Triplecast))
                                {
                                    return Triplecast;
                                }

                                // Weave other oGCDs
                                if (canWeave)
                                {
                                    // Weave Amplifier and Ley Lines
                                    if (lastComboMove == Fire4 && (GetBuffStacks(Buffs.Triplecast) == 1))
                                    {
                                        if (ActionReady(Amplifier) && LevelChecked(Amplifier) && Gauge.PolyglotStacks < 2)
                                        {
                                            return Amplifier;
                                        }
                                        if (ActionReady(LeyLines) && LevelChecked(LeyLines))
                                        {
                                            return LeyLines;
                                        }
                                    }
                                }

                                // Lucid Dreaming     
                                if (IsOffCooldown(All.LucidDreaming) && lastComboMove == Triplecast && currentMP < MP.Fire && LevelChecked(All.LucidDreaming))
                                {
                                    return All.LucidDreaming;
                                }

                                // Manafont
                                if (IsOffCooldown(Manafont) && (lastComboMove == Despair) && LevelChecked(Manafont))
                                {
                                    return Manafont;
                                }

                                // Second Triplecast
                                if ((GetBuffStacks(Buffs.Triplecast) == 0) && HasCharges(Triplecast) && lastComboMove == Fire4 && LevelChecked(Triplecast))
                                {
                                    return Triplecast;
                                }

                                // Cast Despair
                                if (LevelChecked(Despair) && (currentMP < MP.Fire))
                                {
                                    return Despair;
                                }
                                
                                // Cast Fire
                                if (!LevelChecked(Despair) && Gauge.ElementTimeRemaining <= 6000 && currentMP >= MP.Fire)
                                {
                                    return Fire;
                                }

                                // Cast Fire 4 after Manafont
                                if (IsOnCooldown(Manafont) && lastComboMove != Despair)
                                {
                                    return Fire4;
                                }

                                //Sharpcast
                                if (!HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && IsOnCooldown(Manafont) && lastComboMove == Fire4 && LevelChecked(Sharpcast))
                                {
                                    return Sharpcast;
                                }

                                //if not at full astralfire stacks
                                if (Gauge.AstralFireStacks < 3)
                                {
                                    return Fire3;
                                }

                                if (currentMP >= MP.Fire)
                                    return Fire4;

                                // Use Transpose lines opener F3 - Single Transpose variation, double weave version
                                if (currentMP < MP.Fire && lastComboMove != Manafont && IsOnCooldown(Manafont))
                                {
                                    if (lastComboMove == Despair && IsOffCooldown(All.Swiftcast))
                                    {
                                        return Transpose;
                                    }
                                }
                            }

                            if (Gauge.InUmbralIce)
                            {
                                if (Gauge.IsParadoxActive && LevelChecked(Paradox))
                                {
                                    return Paradox;
                                }

                                if (IsOffCooldown(All.Swiftcast) && lastComboMove == Paradox)
                                {
                                    return All.Swiftcast;
                                }

                                // Dump Polyglot Stacks
                                if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= 6000)
                                {
                                    return LevelChecked(Xenoglossy) ? Xenoglossy : Foul;
                                }

                                // Refresh Thunder3
                                if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3 && HasEffect(Buffs.Sharpcast))
                                {
                                    return Thunder3;
                                }

                                if (lastComboMove == Thunder3)
                                {
                                    return Transpose;
                                }
                                openerFinished = true;
                            }
                        }
                    }

                    // Handle movement
                    if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement))
                    {
                        if (IsMoving && InCombat())
                        {
                            if (LevelChecked(Paradox) && Gauge.IsParadoxActive && Gauge.InUmbralIce)
                            {
                                return Paradox;
                            }
                            if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Xeno) && !IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) && LevelChecked(Xenoglossy) && Gauge.HasPolyglotStacks())
                            {
                                return Xenoglossy;
                            }
                            if (HasEffect(Buffs.Thundercloud))
                            {
                                if (!ThunderList.ContainsKey(lastComboMove) && //Is not 1 2 3 or 4
                                    !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4))
                                {
                                    uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                    Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                    if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                    {
                                        return dot; //Use appropriate DoT Action
                                    }
                                }
                            }
                            if (!IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) && IsOffCooldown(All.Swiftcast) && LevelChecked(All.Swiftcast))
                            {
                                return All.Swiftcast;
                            }
                            if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 && LevelChecked(Triplecast))
                            {
                                return Triplecast;
                            }
                            if (HasEffect(Buffs.Firestarter) && Gauge.InAstralFire && LevelChecked(Fire3))
                            {
                                return Fire3;
                            }
                            if (IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Scathe))
                            {
                                return Scathe;
                            }
                        }
                    }

                    // Use under Fire or Ice
                    if (Gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder uptime
                        if (IsEnabled(CustomComboPreset.BLM_AdvThunder) && Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            if (!ThunderList.ContainsKey(lastComboMove) && !TargetHasEffect(Debuffs.Thunder2) && !TargetHasEffect(Debuffs.Thunder4) && LevelChecked(lastComboMove))
                            {
                                if (IsEnabled(CustomComboPreset.BLM_AdvThunderUptime) && HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast) || (IsEnabled(CustomComboPreset.BLM_AdvThunderUptime) && currentMP >= MP.Thunder))
                                {
                                    uint dot = OriginalHook(Thunder); //Grab the appropriate DoT Action
                                    Status? dotDebuff = FindTargetEffect(ThunderList[dot]); //Match it with it's Debuff ID, and check for the Debuff

                                    if (dotDebuff is null || dotDebuff?.RemainingTime <= 5)
                                        return dot; //Use appropriate DoT Action
                                }
                            }
                        }
                        // Weave Buffs
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Casts))
                            {
                                // Use Triplecast only with Astral Fire/Umbral Hearts, and we have enough MP to cast Fire IV twice
                                if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) && (Gauge.InAstralFire || Gauge.UmbralHearts == 3) && currentMP >= MP.Fire * 2)
                                {
                                    if (!IsEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) || GetRemainingCharges(Triplecast) == 2)
                                    {
                                        return Triplecast;
                                    }
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs))
                            {
                                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < 2 && LevelChecked(Amplifier))
                                {
                                    return Amplifier;
                                }

                                if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs_LeyLines))
                                {
                                    if (ActionReady(LeyLines) && LevelChecked(LeyLines))
                                    {
                                        return LeyLines;
                                    }
                                }
                            }
                        }

                        // Transpose Lines Ice phase
                        if (Gauge.InUmbralIce && Gauge.PolyglotStacks > 0 && ActionReady(All.Swiftcast) && IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))   
                        {
                            if (Gauge.UmbralIceStacks < 3 && ActionReady(All.LucidDreaming) && ActionReady(All.Swiftcast))
                            {
                                return All.LucidDreaming;
                            }

                            if (HasEffect(All.Buffs.LucidDreaming) && ActionReady(All.Swiftcast))
                            {
                                return All.Swiftcast;
                            } 
                        }
                    }

                    // Handle initial cast
                    if ((LevelChecked(Blizzard3) && !Gauge.IsEnochianActive) || Gauge.ElementTimeRemaining <= 0)
                    {
                        if (LevelChecked(Fire3))
                        {
                            return (currentMP >= MP.Fire3) ? Fire3 : Blizzard3;
                        }
                        return (currentMP >= MP.Fire) ? Fire : Blizzard;
                    }

                    // Before Blizzard 3; Fire until 0 MP, then Blizzard until max MP.
                    if (!LevelChecked(Blizzard3))
                    {
                        if (Gauge.InAstralFire)
                        {
                            return (currentMP < MP.Fire) ? Transpose : Fire;
                        }
                        if (Gauge.InUmbralIce)
                        {
                            return (currentMP >= MP.MaxMP - MP.Thunder) ? Transpose : Blizzard;
                        }
                    }

                    // Before Fire4; Fire until 0 MP (w/ Firestarter), then Blizzard 3 and Blizzard/Blizzard4 until max MP.
                    if (!LevelChecked(Fire4))
                    {
                        if (Gauge.InAstralFire)
                        {
                            if (HasEffect(Buffs.Firestarter))
                            {
                                return Fire3;
                            }
                            return (currentMP < MP.Fire) ? Blizzard3 : Fire;
                        }
                        if (Gauge.InUmbralIce)
                        {
                            if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3)
                            {
                                return Blizzard4;
                            }
                            return (currentMP >= MP.MaxMP || Gauge.UmbralHearts == 3) ? Fire3 : Blizzard;
                        }
                    }

                    // Use polyglot stacks if we don't need it for a future weave
                    // only when we're not using Transpose lines
                    if (!IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                    {
                        if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= astralFireRefresh && (Gauge.InUmbralIce || (Gauge.InAstralFire && Gauge.UmbralHearts == 0)))
                        {
                            if (LevelChecked(Xenoglossy))
                            {
                                // Check leylines and triplecast cooldown
                                if (Gauge.PolyglotStacks == 2 && GetCooldown(LeyLines).CooldownRemaining >= 20 && GetCooldown(Triplecast).ChargeCooldownRemaining >= 20)
                                {
                                    if (!IsEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling))
                                    {
                                        return Xenoglossy;
                                    }
                                    if (IsEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) && !HasCharges(Triplecast))
                                    {
                                        return Xenoglossy;
                                    }
                                }
                            }
                            else if (LevelChecked(Foul))
                            {
                                return Foul;
                            }
                        }
                    }

                    //Normal Fire Phase
                    if (Gauge.InAstralFire)
                    {
                        //xenoglossy overcap protection
                         if (Gauge.PolyglotStacks == 2 && (Gauge.EnochianTimer <= 3000) && LevelChecked(Xenoglossy))
                         {
                             return Xenoglossy;
                         }

                        // F3 proc or swiftcast F3 during transpose lines(< 3 astral fire stacks)
                        if (Gauge.AstralFireStacks < 3 || (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter)))
                        {
                            return Fire3;
                        }

                        // Use Paradox instead of hardcasting Fire3 if we can
                        if (Gauge.ElementTimeRemaining <= astralFireRefresh && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                        {
                            if (LevelChecked(Paradox))
                            {
                                return Gauge.IsParadoxActive ? Paradox : Despair;
                            }
                            return Fire;
                        }

                        if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs) && IsOffCooldown(Manafont) && lastComboMove == Despair && LevelChecked(Manafont))
                        {
                            return Manafont;
                        }


                        // Double Transpose Line during normal rotation every min Swiftcast is up!
                        if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                        {
                            if (currentMP < MP.Fire && lastComboMove != Manafont && IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) <= 118)
                            {
                                if (IsOffCooldown(All.Swiftcast) && (Gauge.PolyglotStacks == 2))
                                {
                                    if ((lastComboMove == Despair) && Gauge.PolyglotStacks == 2)
                                    {
                                        return Transpose;
                                    }

                                    if (HasEffect(Buffs.Thundercloud) && HasEffect(Buffs.Sharpcast))
                                    {
                                        return Thunder3;
                                    }
                                   /* if (Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy))
                                    {
                                        return Xenoglossy;

                                    }*/
                                }
                            }
                        }

                        // Use Xenoglossy if Amplifier/Triplecast/Leylines/Manafont is available to weave
                        // only when we're not using Transpose Lines 
                        if (!IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) && lastComboMove != Xenoglossy && Gauge.HasPolyglotStacks() && LevelChecked(Xenoglossy) && Gauge.ElementTimeRemaining >= astralFireRefresh)
                        {
                            var pooledPolyglotStacks = IsEnabled(CustomComboPreset.BLM_Adv_CastMovement_Xeno) ? 1 : 0;
                            if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs) && ActionReady(Amplifier))
                            {
                                return Xenoglossy;
                            }
                            if (Gauge.PolyglotStacks > pooledPolyglotStacks)
                            {
                                if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs_LeyLines))
                                {
                                    if (ActionReady(LeyLines))
                                    {
                                        return Xenoglossy;
                                    }
                                }
                                if (IsEnabled(CustomComboPreset.BLM_Adv_Buffs) && !IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                                {
                                    if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast) &&
                                        (!IsEnabled(CustomComboPreset.BLM_Adv_Casts_Pooling) || GetRemainingCharges(Triplecast) > 1))
                                    {
                                        return Xenoglossy;
                                    }
                                    if (ActionReady(Manafont) && currentMP < MP.AllMPSpells)
                                    {
                                        return Xenoglossy;
                                    }
                                    if (ActionReady(Sharpcast) && !HasEffect(Buffs.Sharpcast))
                                    {
                                        return Xenoglossy;
                                    }
                                }
                            }
                        }

                        // Xenoglossy for Manafont weave
                        if (Gauge.HasPolyglotStacks() && IsOffCooldown(Manafont) && currentMP < MP.AllMPSpells && !IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines) && LevelChecked(Xenoglossy)) 
                        {
                            return Xenoglossy;
                        }

                        // Cast Fire 4 after Manafont
                        if (IsOnCooldown(Manafont))
                        {
                           // if ((!TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 179) ||
                           //     (TraitLevelChecked(Traits.EnhancedManafont) && GetCooldownRemainingTime(Manafont) >= 119))
                           // {
                                return Fire4;
                          //  }
                        }

                        // Blizzard3/Despair when below Fire 4 + Despair MP
                        if (currentMP < (MP.Fire + MP.AllMPSpells))
                        {
                            return (LevelChecked(Despair) && currentMP >= MP.AllMPSpells) ? Despair : Blizzard3;
                        }

                        return Fire4;
                    }

                    //Normal Ice Phase
                    if (Gauge.InUmbralIce)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_Adv_Transpose_Lines))
                        {
                            // Transpose lines will use 2 xenoglossy stacks and then transpose
                            if (HasEffect(All.Buffs.LucidDreaming) && Gauge.PolyglotStacks > 0 && LevelChecked(Xenoglossy))
                            {
                                return Xenoglossy;
                            }
                            if (HasEffect(All.Buffs.LucidDreaming) && lastComboMove == Xenoglossy && (Gauge.PolyglotStacks == 0))
                            {
                                return Transpose;
                            }
                        }

                        // Use Paradox when available
                        if (LevelChecked(Paradox) && Gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }

                        if (HasCharges(Sharpcast) && lastComboMove != Thunder3 && !HasEffect(Buffs.Sharpcast))
                        {   
                            return Sharpcast;
                        }
                       
                        //Xenoglossy overcap protection
                        if (Gauge.PolyglotStacks == 2 && (Gauge.EnochianTimer <= 20000) && LevelChecked(Xenoglossy) && Gauge.InUmbralIce)
                        {
                            return Xenoglossy;
                        }
                        // Fire3 when at max umbral hearts
                        return (Gauge.UmbralHearts == 3 && currentMP >= MP.MaxMP - MP.Thunder) ? Fire3 : Blizzard4;

                    }
                }
                return actionID;
            }
        }
            
     

        internal class BLM_Paradox : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Paradox;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;

            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    var canWeave = CanSpellWeave(actionID);
                    var currentMP = LocalPlayer.CurrentMp;
                    var thunder3 = TargetHasEffect(Debuffs.Thunder3);
                    var thunder3Duration = FindTargetEffect(Debuffs.Thunder3);

                    DotRecast thunder3Recast = delegate (int duration)
                    {
                        return !thunder3 || (thunder3 && thunder3Duration.RemainingTime < duration);
                    };

                    // Only enable sharpcast if it's available
                    if (!inOpener && !HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && lastComboMove != Thunder3)
                    {
                        return Sharpcast;
                    }

                    if (!InCombat() && (inOpener || openerFinished))
                    {
                        inOpener = false;
                        openerFinished = false;
                    }

                    if (InCombat() && !inOpener)
                    {
                        inOpener = true;
                    }

                    if (InCombat() && inOpener && !openerFinished)
                    {
                        if (InCombat() && inOpener && !openerFinished)
                        {
                            // Exit out of opener if u died
                            if (HasEffect(All.Buffs.Weakness))
                            {
                                openerFinished = true;
                                return Blizzard3;
                            }

                            if (Gauge.InAstralFire)
                            {
                                // First Triplecast
                                if (lastComboMove != Triplecast && !HasEffect(Buffs.Triplecast) && HasCharges(Triplecast))
                                {
                                    var triplecastMP = 7600;
                                    if (currentMP <= triplecastMP)
                                    {
                                        return Triplecast;
                                    }
                                }

                                // Weave other oGCDs
                                if (canWeave)
                                {
                                    // Weave Amplifier and Ley Lines
                                    if (currentMP <= 4400)
                                    {
                                        if (IsOffCooldown(Amplifier))
                                        {
                                            return Amplifier;
                                        }
                                        if (IsOffCooldown(LeyLines))
                                        {
                                            return LeyLines;
                                        }
                                    }

                                    // Swiftcast
                                    if (IsOffCooldown(All.Swiftcast) && IsOnCooldown(LeyLines))
                                    {
                                        return All.Swiftcast;
                                    }

                                    // Manafont
                                    if (IsOffCooldown(Manafont) && lastComboMove == Despair)
                                    {
                                        if (currentMP < MP.AllMPSpells)
                                        {
                                            return Manafont;
                                        }
                                    }

                                    // Second Triplecast / Sharpcast
                                    if (!IsEnabled(CustomComboPreset.BLM_Simple_OpenerAlternate))
                                    {
                                        if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast) && IsOnCooldown(All.Swiftcast) &&
                                            lastComboMove != All.Swiftcast && HasCharges(Triplecast) && currentMP < MP.Fire)
                                        {
                                            return Triplecast;
                                        }

                                        if (!HasEffect(Buffs.Sharpcast) && HasCharges(Sharpcast) && IsOnCooldown(Manafont) &&
                                            lastComboMove == Fire4)
                                        {
                                            return Sharpcast;
                                        }
                                    }
                                }

                                // Cast Despair
                                if ((currentMP < MP.Fire || Gauge.ElementTimeRemaining <= 4000) && currentMP >= MP.AllMPSpells)
                                {
                                    return Despair;
                                }

                                // Cast Fire 4 after Manafont
                                if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                                {
                                    return Fire4;
                                }

                                // Fire4 / Umbral Ice
                                return currentMP >= MP.Fire ? Fire4 : Blizzard3;
                            }

                            if (Gauge.InUmbralIce)
                            {
                                // Dump Polyglot Stacks
                                if (Gauge.HasPolyglotStacks() && Gauge.ElementTimeRemaining >= 6000)
                                {
                                    return Xenoglossy;
                                }
                                if (Gauge.IsParadoxActive && LevelChecked(Paradox))
                                {
                                    return Paradox;
                                }
                                if (Gauge.UmbralHearts < 3 && lastComboMove != Blizzard4)
                                {
                                    return Blizzard4;
                                }

                                // Refresh Thunder3
                                if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                                {
                                    return Thunder3;
                                }

                                openerFinished = true;
                            }
                        }
                    }

                    if (Gauge.ElementTimeRemaining == 0)
                    {
                        if (currentMP >= MP.Fire3)
                        {
                            return Fire3;
                        }
                        return Blizzard3;
                    }

                    if (Gauge.ElementTimeRemaining > 0)
                    {
                        // Thunder
                        if (lastComboMove != Thunder3 && currentMP >= MP.Thunder &&
                            thunder3Recast(4) && !TargetHasEffect(Debuffs.Thunder) && !TargetHasEffect(Debuffs.Thunder3))
                        {
                            return OriginalHook(Thunder);
                        }

                        // Buffs
                        if (canWeave)
                        {
                            if (!HasEffect(Buffs.Triplecast) && HasCharges(Triplecast))
                            {
                                return Triplecast;
                            }

                            if (IsOffCooldown(Amplifier) && Gauge.PolyglotStacks < 2)
                            {
                                return Amplifier;
                            }

                            if (IsEnabled(CustomComboPreset.BLM_Paradox_LeyLines) && IsOffCooldown(LeyLines))
                            {
                                return LeyLines;
                            }

                            if (IsOffCooldown(Manafont) && Gauge.InAstralFire && currentMP < MP.AllMPSpells)
                            {
                                return Manafont;
                            }

                            if (IsOffCooldown(All.Swiftcast))
                            {
                                return All.Swiftcast;
                            }

                            if (HasCharges(Sharpcast) && !HasEffect(Buffs.Sharpcast))
                            {
                                return Sharpcast;
                            }
                        }
                    }

                    // Play standard while inside of leylines
                    if (HasEffect(Buffs.LeyLines))
                    {
                        if (Gauge.InAstralFire)
                        {
                            if (Gauge.ElementTimeRemaining <= 3000 && HasEffect(Buffs.Firestarter))
                            {
                                return Fire3;
                            }
                            if (Gauge.ElementTimeRemaining <= 6000 && !HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                            {
                                return Gauge.IsParadoxActive ? Paradox : Despair;
                            }
                            return (currentMP >= MP.Fire + MP.AllMPSpells) ? Fire4 : (currentMP >= MP.AllMPSpells ? Despair : Blizzard3);
                        }

                        if (Gauge.InUmbralIce)
                        {
                            if (Gauge.PolyglotStacks == 2)
                            {
                                return Xenoglossy;
                            }
                            return Gauge.IsParadoxActive ? Paradox : (Gauge.UmbralHearts == 3 ? Fire3 : Blizzard4);
                        }
                    }

                    if (Gauge.InUmbralIce)
                    {
                        if (Gauge.IsParadoxActive)
                        {
                            return Paradox;
                        }
                        if (currentMP >= MP.AllMPSpells && (HasEffect(Buffs.Firestarter) || HasEffect(Buffs.Triplecast) || HasEffect(All.Buffs.Swiftcast)))
                        {
                            return Fire3;
                        }
                        if (Gauge.UmbralIceStacks < 3)
                        {
                            return UmbralSoul;
                        }
                        if (IsOffCooldown(Transpose))
                        {
                            return Transpose;
                        }
                    }

                    if (Gauge.InAstralFire)
                    {
                        if (Gauge.AstralFireStacks < 3 && HasEffect(Buffs.Firestarter) && !HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                        {
                            return Fire3;
                        }

                        // Cast Despair after Manafont
                        if (IsOnCooldown(Manafont) && GetCooldownRemainingTime(Manafont) >= 119)
                        {
                            return Despair;
                        }

                        if (HasEffect(Buffs.Triplecast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Sharpcast))
                        {
                            if (!HasEffect(Buffs.Firestarter) && currentMP >= MP.Fire)
                            {
                                if (Gauge.IsParadoxActive)
                                {
                                    return Paradox;
                                }
                                if (!HasEffect(Buffs.Triplecast) && !HasEffect(All.Buffs.Swiftcast))
                                {
                                    return Fire;
                                }
                            }
                            if (currentMP >= MP.AllMPSpells)
                            {
                                return Despair;
                            }
                        }
                        if (IsOffCooldown(Transpose) && openerFinished)
                        {
                            return Transpose;
                        }
                    }

                    if (Gauge.ElementTimeRemaining > 0)
                    {
                        if (Gauge.HasPolyglotStacks())
                        {
                            return Xenoglossy;
                        }
                        if (HasEffect(Buffs.Thundercloud) && lastComboMove != Thunder3)
                        {
                            return Thunder3;
                        }
                        return currentMP <= MP.AllMPSpells ? (Gauge.InAstralFire ? Transpose : UmbralSoul) : Scathe;
                    }
                }

                return actionID;
            }
        }

        internal class BLM_ScatheXeno : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ScatheXeno;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Scathe)
                {
                    if (LevelChecked(Xenoglossy) && Gauge.PolyglotStacks > 0)
                        return Xenoglossy;
                }
                return actionID;
            }
        }
    }
}