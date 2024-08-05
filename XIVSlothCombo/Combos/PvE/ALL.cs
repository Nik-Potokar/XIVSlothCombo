using ECommons.DalamudServices;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class All
    {
        public const byte JobID = 0;

        public const uint
            Rampart = 7531,
            SecondWind = 7541,
            TrueNorth = 7546,
            Addle = 7560,
            Swiftcast = 7561,
            LucidDreaming = 7562,
            Resurrection = 173,
            Raise = 125,
            Provoke = 7533,
            Shirk = 7537,
            Reprisal = 7535,
            Esuna = 7568,
            Rescue = 7571,
            SolidReason = 232,
            AgelessWords = 215,
            Sleep = 25880,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Bloodbath = 7542,
            HeadGraze = 7551,
            FootGraze = 7553,
            LegGraze = 7554,
            Feint = 7549,
            Interject = 7538,
            Peloton = 7557,
            LegSweep = 7863,
            Repose = 16560,
            Sprint = 3;
        private const uint
            IsleSprint = 31314;

        public static class Buffs
        {
            public const ushort
                Weakness = 43,
                Medicated = 49,
                Bloodbath = 84,
                Swiftcast = 167,
                Rampart = 1191,
                Peloton = 1199,
                LucidDreaming = 1204,
                TrueNorth = 1250,
                Sprint = 50;
        }

        public static class Debuffs
        {
            public const ushort
                Sleep = 3,
                Bind = 13,
                Heavy = 14,
                Addle = 1203,
                Reprisal = 1193,
                Feint = 1195;
        }

        /// <summary>
        /// Quick Level, Offcooldown, spellweave, and MP check of Lucid Dreaming
        /// </summary>
        /// <param name="actionID">action id to check weave</param>
        /// <param name="MPThreshold">Player MP less than Threshold check</param>
        /// <param name="weave">Spell Weave check by default</param>
        /// <returns></returns>
        public static bool CanUseLucid(uint actionID, int MPThreshold, bool weave = true) =>
            CustomComboFunctions.ActionReady(LucidDreaming)
            && CustomComboFunctions.LocalPlayer.CurrentMp <= MPThreshold
            && (weave && CustomComboFunctions.CanSpellWeave(actionID));

        internal class ALL_IslandSanctuary_Sprint : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_IslandSanctuary_Sprint;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Sprint && Svc.ClientState.TerritoryType is 1055) return IsleSprint;
                else return actionID;
            }
        }

        //Tank Features
        internal class ALL_Tank_Interrupt : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Interrupt;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is LowBlow or PLD.ShieldBash)
                {
                    if (CanInterruptEnemy() && ActionReady(Interject))
                        return Interject;
                    if (ActionReady(LowBlow))
                        return LowBlow;
                    if (actionID == PLD.ShieldBash && IsOnCooldown(LowBlow))
                        return actionID;
                }

                return actionID;
            }
        }

        internal class ALL_Tank_Reprisal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Reprisal;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Reprisal)
                {
                    if (TargetHasEffectAny(Debuffs.Reprisal) && IsOffCooldown(Reprisal))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        //Healer Features
        internal class ALL_Healer_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Healer_Raise;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if ((actionID is WHM.Raise or AST.Ascend or SGE.Egeiro)
                    || (actionID is SCH.Resurrection && LocalPlayer.ClassJob.Id is SCH.JobID))
                {
                    if (ActionReady(Swiftcast))
                        return Swiftcast;

                    if (actionID == WHM.Raise && IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && ActionReady(WHM.ThinAir) && !HasEffect(WHM.Buffs.ThinAir))
                        return WHM.ThinAir;

                    return actionID;
                }

                return actionID;
            }
        }

        //Caster Features
        internal class ALL_Caster_Addle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Addle;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Addle)
                {
                    if (TargetHasEffectAny(Debuffs.Addle) && IsOffCooldown(Addle))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        internal class ALL_Caster_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Raise;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if ((actionID is BLU.AngelWhisper or RDM.Verraise)
                    || (actionID is SMN.Resurrection && LocalPlayer.ClassJob.Id is SMN.JobID))
                {
                    if (HasEffect(Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast))
                        return actionID;
                    if (IsOffCooldown(Swiftcast))
                        return Swiftcast;
                    if (LocalPlayer.ClassJob.Id is RDM.JobID)
                        return RDM.Vercure;
                }

                return actionID;
            }
        }

        //Melee DPS Features
        internal class ALL_Melee_Feint : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_Feint;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Feint)
                {
                    if (TargetHasEffectAny(Debuffs.Feint) && IsOffCooldown(Feint))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        internal class ALL_Melee_TrueNorth : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_TrueNorth;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is TrueNorth)
                {
                    if (HasEffect(Buffs.TrueNorth))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        //Ranged Physical Features
        internal class ALL_Ranged_Mitigation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Mitigation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is BRD.Troubadour or MCH.Tactician or DNC.ShieldSamba)
                {
                    if ((HasEffectAny(BRD.Buffs.Troubadour) || HasEffectAny(MCH.Buffs.Tactician) || HasEffectAny(DNC.Buffs.ShieldSamba)) && IsOffCooldown(actionID))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        internal class ALL_Ranged_Interrupt : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Interrupt;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return (actionID is FootGraze && CanInterruptEnemy() && ActionReady(HeadGraze)) ? HeadGraze : actionID;
            }
        }
    }
}

