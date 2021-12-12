using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class RDM
    {
        public const byte JobID = 35;

        public const uint
            Verthunder = 7505,
            Veraero = 7507,
            Veraero2 = 16525,
            Veraero3 = 25856,
            Verthunder2 = 16524,
            Verthunder3 = 25855,
            Impact = 16526,
            Redoublement = 7516,
            EnchantedRedoublement = 7529,
            Zwerchhau = 7512,
            EnchantedZwerchhau = 7528,
            Riposte = 7504,
            EnchantedRiposte = 7527,
            Scatter = 7509,
            Verstone = 7511,
            Verfire = 7510,
            Jolt = 7503,
            Jolt2 = 7524,
            Verholy = 7526,
            Verflare = 7525,
            Fleche = 7517,
            ContreSixte = 7519,
            Engagement = 16527,
            Verraise = 7523,
            Scorch = 16530;

        public static class Buffs
        {
            public const short
                Swiftcast = 167,
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Jolt = 2,
                Verthunder = 4,
                Veraero = 10,
                Verraise = 64,
                Zwerchhau = 35,
                Redoublement = 50,
                Vercure = 54,
                Jolt2 = 62,
                Impact = 66,
                Verflare = 68,
                Verholy = 70,
                Fleche = 45,
                ContreSixte = 56,
                Engagement = 72,
                Scorch = 80,
                Veraero3 = 82,
                Verthunder3 = 82;
        }
    }

    internal class RedMageAoECombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.RedMageAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RDM.Veraero2)
            {
                if (HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast))
                    return OriginalHook(RDM.Impact);

                return RDM.Veraero2;
            }

            if (actionID == RDM.Verthunder2)
            {
                if (HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast))
                    return OriginalHook(RDM.Impact);

                return RDM.Verthunder2;
            }

            return actionID;
        }
    }

    internal class RedMageMeleeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.RedMageMeleeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RDM.Redoublement)
            {
                var gauge = GetJobGauge<RDMGauge>();

                if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus))
                {
                    if (lastComboMove == RDM.EnchantedRedoublement)
                    {
                        if (gauge.BlackMana >= gauge.WhiteMana && level >= RDM.Levels.Verholy)
                        {
                            if (HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady) && (gauge.BlackMana - gauge.WhiteMana <= 9))
                                return RDM.Verflare;

                            return RDM.Verholy;
                        }
                        else if (level >= RDM.Levels.Verflare)
                        {
                            if (!HasEffect(RDM.Buffs.VerstoneReady) && HasEffect(RDM.Buffs.VerfireReady) && level >= RDM.Levels.Verholy && (gauge.WhiteMana - gauge.BlackMana <= 9))
                                return RDM.Verholy;

                            return RDM.Verflare;
                        }
                    }
                }

                if ((lastComboMove == RDM.Riposte || lastComboMove == RDM.EnchantedRiposte) && level >= RDM.Levels.Zwerchhau)
                    return OriginalHook(RDM.Zwerchhau);

                if (lastComboMove == RDM.Zwerchhau && level >= RDM.Levels.Redoublement)
                    return OriginalHook(RDM.Redoublement);

                if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus))
                {
                    if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && level >= RDM.Levels.Scorch)
                        return RDM.Scorch;
                }

                return OriginalHook(RDM.Riposte);
            }

            return actionID;
        }
    }

    internal class RedMageVerprocCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.RedMageVerprocCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RDM.Verstone)
            {
                if (level >= RDM.Levels.Scorch && (lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy))
                    return RDM.Scorch;

                if (lastComboMove == RDM.EnchantedRedoublement && level >= RDM.Levels.Verholy)
                    return RDM.Verholy;

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Veraero3)
                        return RDM.Veraero3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerstoneReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Veraero3)
                        return RDM.Veraero3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Veraero && level < RDM.Levels.Veraero3)
                        return RDM.Veraero;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerstoneReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Veraero && level < RDM.Levels.Veraero3)
                        return RDM.Veraero;
                }

                if (HasEffect(RDM.Buffs.VerstoneReady))
                    return RDM.Verstone;

                return OriginalHook(RDM.Jolt2);
            }

            if (actionID == RDM.Verfire)
            {
                if (level >= RDM.Levels.Scorch && (lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy))
                    return RDM.Scorch;

                if (lastComboMove == RDM.EnchantedRedoublement && level >= RDM.Levels.Verflare)
                    return RDM.Verflare;

                // Thunder 3
                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Verthunder3)
                        return RDM.Verthunder3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder3)
                        return RDM.Verthunder3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Verthunder && level < RDM.Levels.Verthunder3)
                        return RDM.Verthunder;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder && level < RDM.Levels.Verthunder3)
                        return RDM.Verthunder;
                }

                if (HasEffect(RDM.Buffs.VerfireReady))
                    return RDM.Verfire;

                return OriginalHook(RDM.Jolt2);
            }

            return actionID;
        }
    }

    internal class RedMageOgcdCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.RedMageOgcdCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
            {
                if (actionID == RDM.ContreSixte || actionID == RDM.Fleche)
                {
                    if (level >= RDM.Levels.ContreSixte && level <= RDM.Levels.Fleche)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    if (level >= RDM.Levels.ContreSixte)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    if (level >= RDM.Levels.Fleche)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    return RDM.Fleche;
                }

                return actionID;
            }

            return actionID;
        }
    }
}