using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            // Heals and Shields
            Diagnosis = 24284,
            Prognosis = 24286,
            Egeiro = 24287,
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
            Swiftcast = 7561,
            LucidDreaming = 7562,
            Kardia = 24285,
            Eukrasia = 24290,
            Rhizomata = 24309;
			

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                Swiftcast = 167;
        }

        public static class Debuffs
        {
            public const ushort
            EukrasianDosis1 = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616;
        }

        public static class Levels
        {
            public const ushort
                Dosis = 1,
                Prognosis = 10,
                Phlegma = 26,
                Soteria = 35,
                Druochole = 45,
                Kerachole = 50,
                Ixochole = 52,
                Physis2 = 60,
                Taurochole = 62,
                Haima = 70,
                Phlegma2 = 72,
                Dosis2 = 72,
                Rhizomata = 74,
                Holos = 76,
                Panhaima = 80,
                Phlegma3 = 82,
                Dosis3 = 82,
                Krasis = 86,
                Pneuma = 90;
        }
    }

    internal class SageKardiaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageKardiaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Soteria)
            {
                var soteriaCD = GetCooldown(SGE.Soteria);
                if (HasEffect(SGE.Buffs.Kardia) && !soteriaCD.IsCooldown)
                    return SGE.Soteria;
                return SGE.Kardia;
            }

            return actionID;
        }
    }

    internal class SageRhizomataFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageRhizomataFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Taurochole || actionID == SGE.Druochole || actionID == SGE.Ixochole || actionID == SGE.Kerachole)
            {
                if (level >= SGE.Levels.Rhizomata && GetJobGauge<SGEGauge>().Addersgall == 0)
                        return SGE.Rhizomata;
            }

            return actionID;
        }
    }

    internal class SageTauroDruoFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageTauroDruoFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Taurochole)
            {
                var taurocholeCD = GetCooldown(SGE.Taurochole);
                if (taurocholeCD.CooldownRemaining > 0)

                    return SGE.Druochole;
            }
            return actionID;
        }
    }

    internal class SagePhlegmaToxikonFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SagePhlegmaToxikonFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Phlegma || actionID == SGE.Phlegma2 || actionID == SGE.Phlegma3)
            {
                if (level >= SGE.Levels.Dosis3)
                {
                    if (GetCooldown(SGE.Phlegma3).CooldownRemaining > 45 && GetJobGauge<SGEGauge>().Addersting > 0)
                        return OriginalHook(SGE.Toxikon);
                }

                if (level >= 66)
                {
                    if (GetCooldown(SGE.Phlegma2).CooldownRemaining > 45 && GetJobGauge<SGEGauge>().Addersting > 0)
                        return OriginalHook(SGE.Toxikon);
                }

            }

            return actionID;
        }
    }

    internal class SagePhlegmaDyskrasiaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SagePhlegmaDyskrasiaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Phlegma || actionID == SGE.Phlegma2 || actionID == SGE.Phlegma3)
            {
                if (level >= SGE.Levels.Dosis3)
                {
                    if (GetCooldown(SGE.Phlegma3).CooldownRemaining > 45)
                        return SGE.Dyskrasia2;
                }

                if (level >= SGE.Levels.Dosis2)
                {
                    if (GetCooldown(SGE.Phlegma2).CooldownRemaining > 45)
                        return SGE.Dyskrasia;
                }

                if (GetCooldown(SGE.Phlegma).CooldownRemaining > 45)
                    return SGE.Dyskrasia;
            }

            return actionID;
        }
    }

    internal class SageDPSFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Dosis1 || actionID == SGE.Dosis2 || actionID == SGE.Dosis3)
            {
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var dosis3CD = GetCooldown(SGE.Dosis3);
                var dosis1Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis1);
                var dosis2Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis2);
                var dosis3Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis3);

                if (IsEnabled(CustomComboPreset.SageDPSFeature) && level >= 82 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis3;
                    if ((dosis3Debuff is null) || (dosis3Debuff.RemainingTime <= 4))
                        return SGE.Eukrasia;
                }

                if (IsEnabled(CustomComboPreset.SageDPSFeature) && level >= 72 && level <= 81 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis2;
                    if ((dosis2Debuff is null) || (dosis2Debuff.RemainingTime <= 4))
                        return SGE.Eukrasia;
                }

                if (IsEnabled(CustomComboPreset.SageDPSFeature) && level >= 30 && level <= 71 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis1;
                    if ((dosis1Debuff is null) || (dosis1Debuff.RemainingTime <= 4))
                        return SGE.Eukrasia;
                }
                if (IsEnabled(CustomComboPreset.SageLucidFeature))
                {
                    var lucidDreaming = GetCooldown(SGE.LucidDreaming);
                    var actionIDCD = GetCooldown(actionID);
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && actionIDCD.CooldownRemaining > 0.2)
                        return SGE.LucidDreaming;
                }
            }

            return actionID;
        }
    }

    internal class SageEgeiroFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageEgeiroFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Swiftcast)
            {
                if (IsEnabled(CustomComboPreset.SageEgeiroFeature))
                {
                    if (HasEffect(SGE.Buffs.Swiftcast))
                        return SGE.Egeiro;
                }

                return OriginalHook(SGE.Swiftcast);
            }

            return actionID;
        }
    }

    internal class SageAlternateEgeiroFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageAlternateEgeiroFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Egeiro)
            {
                var swiftCD = GetCooldown(SGE.Swiftcast);
                if ((swiftCD.CooldownRemaining == 0)
)
                    return SGE.Swiftcast;
            }
            return actionID;
        }
    }

    internal class SageDPSFeatureTest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageDPSFeatureTest;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Dosis1 || actionID == SGE.Dosis2 || actionID == SGE.Dosis3)
            {
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var dosis3CD = GetCooldown(SGE.Dosis3);
                var dosis1Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis1);
                var dosis2Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis2);
                var dosis3Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis3);
                var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                var CurrentHpValue = Service.Configuration.EnemyCurrentHp;

                if (IsEnabled(CustomComboPreset.SageDPSFeatureTest) && level >= 82 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis3;
                    if ((dosis3Debuff is null && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue) || ((dosis3Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue))
                        return SGE.Eukrasia;
                }

                if (IsEnabled(CustomComboPreset.SageDPSFeatureTest) && level >= 72 && level <= 81 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis2;
                    if ((dosis2Debuff is null && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue) || ((dosis2Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue))
                        return SGE.Eukrasia;
                }

                if (IsEnabled(CustomComboPreset.SageDPSFeatureTest) && level >= 30 && level <= 71 && incombat)
                {
                    if (HasEffect(SGE.Buffs.Eukrasia))
                        return SGE.EukrasianDosis1;
                    if ((dosis1Debuff is null && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue) || ((dosis1Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue))
                        return SGE.Eukrasia;
                }
                if (IsEnabled(CustomComboPreset.SageLucidFeatureTest))
                {
                    var lucidDreaming = GetCooldown(SGE.LucidDreaming);
                    var actionIDCD = GetCooldown(actionID);
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && actionIDCD.CooldownRemaining > 0.2)
                        return SGE.LucidDreaming;
                }
            }

            return actionID;
        }
    }

}
