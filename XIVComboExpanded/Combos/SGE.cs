using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            Diagnosis = 24284,
            Holos = 24310,
            Ixochole = 24299,
            Egeiro = 24287,
            Kardia = 24285,
            Soteria = 24294,
            Phlegma = 24289,
            Phlegmara = 24307,
            Phlegmaga = 24313,
            Dyskrasia = 24297,
            Eukrasia = 24290,

            // dps
            Dosis1 = 24283,
            Dosis2 = 24306,
            Dosis3 = 24312,
            EukrasianDosis1 = 24293,
            EukrasianDosis2 = 24308,
            EukrasianDosis3 = 24314,
            // Other
            Swiftcast = 7561;

        public static class Buffs
        {
            public const short
                Kardia = 2604,
                Eukrasia = 2606,
                Swiftcast = 167;
        }

        public static class Debuffs
        {
            public const short
            EukrasianDosis1 = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616;
        }

        public static class Levels
        {
            public const ushort
                Dosis = 1,
                Prognosis = 10,
                Druochole = 45,
                Kerachole = 50,
                Taurochole = 62,
                Ixochole = 52,
                Dosis2 = 72,
                Holos = 76,
                Rizomata = 74,
                Dosis3 = 82;
        }
    }

    internal class SageKardiaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageKardiaFeature;

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

    internal class SagePhlegmaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SagePhlegmaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (level >= SGE.Levels.Dosis3)
            {
                if (GetCooldown(SGE.Phlegmaga).CooldownRemaining > 45)
                    return SGE.Dyskrasia;
            }

            if (level >= SGE.Levels.Dosis2)
            {
                if (GetCooldown(SGE.Phlegmara).CooldownRemaining > 45)
                    return SGE.Dyskrasia;
            }

            if (GetCooldown(SGE.Phlegma).CooldownRemaining > 45)
                return SGE.Dyskrasia;

            return actionID;
        }
    }

    internal class SageDPSFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageDPSFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Dosis1 || actionID == SGE.Dosis2 || actionID == SGE.Dosis3)
            {
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var dosis1CD = GetCooldown(SGE.Dosis1);
                var dosis1Debuff = FindTargetEffect(SGE.Debuffs.EukrasianDosis1);

                if (!TargetHasEffect(SGE.Debuffs.EukrasianDosis1) && HasEffect(SGE.Buffs.Eukrasia) && incombat && level >= 30)
                    return SGE.EukrasianDosis1;
                if (!TargetHasEffect(SGE.Debuffs.EukrasianDosis1) && incombat)
                    return SGE.Eukrasia;
            }

            return actionID;
        }
    }

    internal class SageEgeiroFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageEgeiroFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Egeiro)
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
}