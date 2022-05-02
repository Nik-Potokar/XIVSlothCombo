using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
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
            Scorch = 16530,
            Resolution = 25858,
            Moulinet = 7513,
            EnchantedMoulinet = 7530,
            Corpsacorps = 7506,
            Displacement = 7515,

            //Buffs
            Acceleration = 7518,
            Manafication = 7521,
            Embolden = 7520;

        public static class Buffs
        {
            public const ushort
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249,
                Chainspell = 2560,
                Acceleration = 1238;
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
                Corpsacorps = 6,
                Veraero = 10,
                Verthunder2 = 18,
                Veraero2 = 22,
                Verraise = 64,
                Zwerchhau = 35,
                Displacement = 40,
                Acceleration = 50,
                Redoublement = 50,
                Moulinet = 52,
                Vercure = 54,
                Embolden = 58,
                Manafication = 60,
                Jolt2 = 62,
                Impact = 66,
                ManaStack = 68,
                Verflare = 68,
                Verholy = 70,
                Fleche = 45,
                ContreSixte = 56,
                Engagement = 40,
                Scorch = 80,
                Veraero3 = 82,
                Verthunder3 = 82,
                Resolution = 90;
        }

        public static class Config
        {
            public const string
                RdmLucidMpThreshold = "RdmLucidMpThreshold";
        }


        internal class RedMageAoECombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageAoECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var swiftcast = HasEffect(All.Buffs.Swiftcast);
                var dualcast = HasEffect(Buffs.Dualcast);
                var chainspell = HasEffect(Buffs.Chainspell);
                var canWeave = CanWeave(actionID);

                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !dualcast)
                {
                    if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                        return Fleche;

                    if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                        return ContreSixte;
                }

                if (actionID is RDM.Veraero2 && level >= Levels.Veraero2)
                {
                    if (swiftcast || dualcast || chainspell)
                        return OriginalHook(Impact);

                    return Veraero2;
                }

                if (actionID is RDM.Verthunder2 && level >= Levels.Verthunder2)
                {
                    if (swiftcast || dualcast || chainspell)
                        return OriginalHook(Impact);

                    return Verthunder2;
                }

                return actionID;
            }
        }

        internal class RedMageMeleeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMeleeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Redoublement or Riposte or Zwerchhau)
                {
                    var gauge = GetJobGauge<RDMGauge>();
                    var canWeave = CanWeave(OriginalHook(actionID));

                    if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && GetCooldownRemainingTime(Engagement) < 35 &&
                         InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte) && level >= Levels.Engagement && !HasEffect(Buffs.Dualcast))
                        return Engagement;

                    if (IsEnabled(CustomComboPreset.RedMageCorpsACorpsFeature) && canWeave && GetCooldownRemainingTime(Corpsacorps) < 35 &&
                        ((InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte)) ||
                         (IsEnabled(CustomComboPreset.RedMageCorpsACorpsPullFeature) &&
                         gauge.BlackMana >= 50 && gauge.WhiteMana >= 50)) && level >= Levels.Corpsacorps && !HasEffect(Buffs.Dualcast))
                        return Corpsacorps;

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus))
                    {
                        if (lastComboMove is RDM.EnchantedRedoublement)
                        {
                            if (gauge.BlackMana >= gauge.WhiteMana && level >= Levels.Verholy)
                            {
                                if (HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.VerfireReady) && (gauge.BlackMana - gauge.WhiteMana <= 9))
                                    return Verflare;

                                return Verholy;
                            }
                            else if (level >= Levels.Verflare)
                            {
                                if (!HasEffect(Buffs.VerstoneReady) && HasEffect(Buffs.VerfireReady) && level >= Levels.Verholy && (gauge.WhiteMana - gauge.BlackMana <= 9))
                                    return Verholy;

                                return Verflare;
                            }
                        }
                    }

                    if ((lastComboMove is Riposte or EnchantedRiposte) && level >= Levels.Zwerchhau)
                        return OriginalHook(Zwerchhau);

                    if (lastComboMove is RDM.Zwerchhau && level >= Levels.Redoublement)
                        return OriginalHook(Redoublement);

                    if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus))
                    {
                        if ((lastComboMove is Verflare or Verholy) && level >= Levels.Scorch)
                            return Scorch;
                    }

                    if (IsEnabled(CustomComboPreset.RedmageResolutionFinisherMelee))
                    {
                        if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                            return Resolution;
                    }

                    return OriginalHook(Riposte);
                }

                return actionID;
            }
        }

        internal class RedMageVerprocCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageVerprocCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Verstone)
                {
                    var canWeave = CanWeave(actionID);
                    var inCombat = HasCondition(ConditionFlag.InCombat);

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (IsEnabled(CustomComboPreset.RedmageResolutionFinisher))
                    {
                        if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                            return Resolution;
                    }

                    if (level >= Levels.Scorch && (lastComboMove is Verholy or Verflare))
                        return Scorch;

                    if (lastComboMove is RDM.EnchantedRedoublement && level >= Levels.Verholy)
                        return Verholy;

                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                    {
                        if ((HasEffect(Buffs.Dualcast) || HasEffect(Buffs.Chainspell) || HasEffect(All.Buffs.Swiftcast)) && level >= Levels.Veraero3)
                            return Veraero3;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                    {
                        if (!HasEffect(Buffs.VerstoneReady) && !inCombat && level >= Levels.Veraero3)
                            return Veraero3;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                    {
                        if ((HasEffect(Buffs.Dualcast) || HasEffect(Buffs.Chainspell) || HasEffect(All.Buffs.Swiftcast)) && level >= Levels.Veraero && level < Levels.Veraero3)
                            return Veraero;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                    {
                        if (!HasEffect(Buffs.VerstoneReady) && !inCombat && level >= Levels.Veraero && level < Levels.Veraero3)
                            return Veraero;
                    }

                    if (HasEffect(Buffs.VerstoneReady))
                        return Verstone;

                    return OriginalHook(Jolt2);
                }

                if (actionID == Verfire)
                {
                    var inCombat = HasCondition(ConditionFlag.InCombat);

                    if (level >= Levels.Scorch && (lastComboMove is Verholy or Verflare))
                        return Scorch;

                    if (lastComboMove is RDM.EnchantedRedoublement && level >= Levels.Verflare)
                        return Verflare;

                    // Thunder 3
                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                    {
                        if ((HasEffect(Buffs.Dualcast) || HasEffect(Buffs.Chainspell) || HasEffect(All.Buffs.Swiftcast)) && level >= Levels.Verthunder3)
                            return Verthunder3;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                    {
                        if (!HasEffect(Buffs.VerfireReady) && !inCombat && level >= Levels.Verthunder3)
                            return Verthunder3;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                    {
                        if ((HasEffect(Buffs.Dualcast) || HasEffect(Buffs.Chainspell) || HasEffect(All.Buffs.Swiftcast)) && level >= Levels.Verthunder && level < Levels.Verthunder3)
                            return Verthunder;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                    {
                        if (!HasEffect(Buffs.VerfireReady) && !inCombat && level >= Levels.Verthunder && level < Levels.Verthunder3)
                            return Verthunder;
                    }

                    if (HasEffect(Buffs.VerfireReady))
                        return Verfire;
                    if (IsEnabled(CustomComboPreset.RedmageResolutionFinisher))
                    {
                        if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                            return Resolution;
                    }

                    return OriginalHook(Jolt2);
                }

                return actionID;
            }
        }

        internal class RedMageOgcdCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageOgcdCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.RedMageOgcdCombo))
                {
                    if (actionID is ContreSixte or Fleche)
                    {
                        if (level >= Levels.ContreSixte && level <= Levels.Fleche)
                            return CalcBestAction(actionID, ContreSixte, Fleche);

                        if (level >= Levels.ContreSixte)
                            return CalcBestAction(actionID, ContreSixte, Fleche);

                        if (level >= Levels.Fleche)
                            return CalcBestAction(actionID, ContreSixte, Fleche);

                        return Fleche;
                    }

                    return actionID;
                }

                return actionID;
            }
        }

        internal class RedMageSmartcastAoECombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSmartcastAoECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Veraero2 or Verthunder2)
                {
                    const int
                    FINISHER_DELTA = 11,
                    IMBALANCE_DIFF_MAX = 30;

                    var accelBuff = HasEffect(Buffs.Acceleration);
                    var dualcastBuff = HasEffect(Buffs.Dualcast);
                    var swiftcastBuff = HasEffect(All.Buffs.Swiftcast);
                    var gauge = GetJobGauge<RDMGauge>();
                    int black = gauge.BlackMana;
                    int white = gauge.WhiteMana;
                    int blackThreshold = white + IMBALANCE_DIFF_MAX;
                    int whiteThreshold = black + IMBALANCE_DIFF_MAX;
                    var canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                        return Resolution;

                    if (level >= Levels.Scorch && (lastComboMove is Verholy or Verflare))
                        return Scorch;

                    if (gauge.ManaStacks == 3 && level >= Levels.Verflare)
                    {
                        if (black >= white && level >= Levels.Verholy)
                        {
                            if (HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                                return Verflare;

                            return Verholy;
                        }
                        if (HasEffect(Buffs.VerfireReady) && !HasEffect(Buffs.VerstoneReady) && level >= Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                            return Verholy;

                        return Verflare;
                    }

                    if (dualcastBuff || accelBuff || swiftcastBuff || HasEffect(Buffs.Chainspell) || level <= Levels.Verthunder2)
                        return OriginalHook(Impact);

                    if (level <= Levels.Verthunder2)
                        return Verthunder2;

                    if (gauge.BlackMana > gauge.WhiteMana)
                        return Veraero2;

                    if (gauge.WhiteMana > gauge.BlackMana)
                        return Verthunder2;
                }

                return actionID;
            }
        }

        internal class RedMageSmartSingleTargetCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSmartSingleTargetCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID is Veraero or Verthunder or Verstone or Verfire)
                {
                    const int
                    LONG_DELTA = 6,
                    PROC_DELTA = 5,
                    FINISHER_DELTA = 11,
                    IMBALANCE_DIFF_MAX = 30;

                    bool verfireUp = HasEffect(Buffs.VerfireReady);
                    bool verstoneUp = HasEffect(Buffs.VerstoneReady);
                    RDMGauge gauge = GetJobGauge<RDMGauge>();
                    int black = gauge.BlackMana;
                    int white = gauge.WhiteMana;
                    var engagementCD = GetCooldown(Engagement);
                    var canWeave = CanWeave(OriginalHook(actionID));

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerSmartCastFeature))
                    {
                        if (!HasEffect(Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= Levels.Verthunder)
                            return OriginalHook(Verthunder);
                    }

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;
                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (actionID is Veraero or Verthunder)
                    {

                        if (level < Levels.Verthunder)
                            return Jolt;

                        if (level is < Levels.Veraero and >= Levels.Verthunder)
                            return OriginalHook(Verthunder);

                        // This is for the long opener only, so we're not bothered about fast casting or finishers or anything like that
                        if (black < white)
                            return OriginalHook(Verthunder);

                        if (white < black)
                            return OriginalHook(Veraero);

                        return actionID;
                    }

                    if (actionID is Verstone or Verfire)
                    {

                        bool fastCasting = HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast);
                        bool accelerated = HasEffect(Buffs.Acceleration);
                        bool isFinishing1 = gauge.ManaStacks == 3;
                        bool isFinishing2 = comboTime > 0 && lastComboMove is Verholy or Verflare;
                        bool isFinishing3 = comboTime > 0 && lastComboMove is RDM.Scorch;
                        bool canFinishWhite = level >= Levels.Verholy;
                        bool canFinishBlack = level >= Levels.Verflare;
                        int blackThreshold = white + IMBALANCE_DIFF_MAX;
                        int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                        // If we're ready to Scorch or Resolution, just do that. Nice and simple. Sadly, that's where the simple ends.
                        if (isFinishing3 && level >= Levels.Resolution)
                            return Resolution;

                        if (isFinishing2 && level >= Levels.Scorch)
                            return Scorch;

                        if (isFinishing1 && canFinishBlack)
                        {

                            if (black >= white && canFinishWhite)
                            {

                                // If we can already Verstone, but we can't Verfire, and Verflare WON'T imbalance us, use Verflare
                                if (verstoneUp && !verfireUp && (black + FINISHER_DELTA <= blackThreshold))
                                    return Verflare;

                                return Verholy;
                            }

                            // If we can already Verfire, but we can't Verstone, and we can use Verholy, and it WON'T imbalance us, use Verholy
                            if (verfireUp && !verstoneUp && canFinishWhite && (white + FINISHER_DELTA <= whiteThreshold))
                                return Verholy;

                            return Verflare;
                        }

                        if (fastCasting || accelerated)
                        {

                            if (level is < Levels.Veraero and >= Levels.Verthunder)
                                return Verthunder;

                            if (verfireUp == verstoneUp)
                            {

                                // Either both procs are already up or neither is - use whatever gives us the mana we need
                                if (black < white)
                                    return OriginalHook(Verthunder);

                                if (white < black)
                                    return OriginalHook(Veraero);

                                // If mana levels are equal, prioritise the colour that the original button was
                                return actionID is RDM.Verstone
                                    ? OriginalHook(Veraero)
                                    : OriginalHook(Verthunder);
                            }

                            if (verfireUp)
                            {

                                // If Veraero is feasible, use it
                                if (white + LONG_DELTA <= whiteThreshold)
                                    return OriginalHook(Veraero);

                                return OriginalHook(Verthunder);
                            }

                            if (verstoneUp)
                            {

                                // If Verthunder is feasible, use it
                                if (black + LONG_DELTA <= blackThreshold)
                                    return OriginalHook(Verthunder);

                                return OriginalHook(Veraero);
                            }
                        }

                        if (verfireUp && verstoneUp)
                        {

                            // Decide by mana levels
                            if (black < white)
                                return Verfire;

                            if (white < black)
                                return Verstone;

                            // If mana levels are equal, prioritise the original button
                            return actionID;
                        }

                        // Only use Verfire if it won't imbalance us
                        if (verfireUp && black + PROC_DELTA <= blackThreshold)
                            return Verfire;

                        // Only use Verstone if it won't imbalance us
                        if (verstoneUp && white + PROC_DELTA <= whiteThreshold)
                            return Verstone;

                        // If neither's up or the one that is would imbalance us, just use Jolt
                        return OriginalHook(Jolt2);
                    }
                }
                return actionID;
            }
        }

        internal class RedMageMeleeAoECombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMeleeAoECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is EnchantedMoulinet or Moulinet)
                {
                    const int
                    FINISHER_DELTA = 11,
                    IMBALANCE_DIFF_MAX = 30;
                    var gauge = GetJobGauge<RDMGauge>();
                    int black = gauge.BlackMana;
                    int white = gauge.WhiteMana;
                    int blackThreshold = white + IMBALANCE_DIFF_MAX;
                    int whiteThreshold = black + IMBALANCE_DIFF_MAX;
                    var canWeave = CanWeave(OriginalHook(actionID));

                    if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && GetCooldownRemainingTime(Engagement) < 35 &&
                        InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte) && level >= Levels.Engagement && !HasEffect(Buffs.Dualcast))
                        return Engagement;

                    if (IsEnabled(CustomComboPreset.RedMageCorpsACorpsFeature) && canWeave && GetCooldownRemainingTime(Corpsacorps) < 35 &&
                        ((InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte)) ||
                         (IsEnabled(CustomComboPreset.RedMageCorpsACorpsPullFeature) &&
                         gauge.BlackMana >= 60 && gauge.WhiteMana >= 60)) && level >= Levels.Corpsacorps && !HasEffect(Buffs.Dualcast))
                        return Corpsacorps;

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                        return Resolution;

                    if (level >= Levels.Scorch && (lastComboMove is Verholy or Verflare))
                        return Scorch;

                    if (gauge.ManaStacks == 3 && level >= Levels.Verflare)
                    {
                        if (black >= white && level >= Levels.Verholy)
                        {
                            if (HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                                return Verflare;

                            return Verholy;
                        }

                        if (HasEffect(Buffs.VerfireReady) && !HasEffect(Buffs.VerstoneReady) && level >= Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                            return Verholy;

                        return Verflare;
                    }
                }

                return actionID;
            }
        }

        internal class SimpleRedMage : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleRedMage;

            internal static bool inOpener = false;
            internal static bool openerFinished = false;
            internal static byte step = 0;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)

            {
                if (actionID is Jolt or Veraero or Verthunder or Verstone
                    or Verfire or Riposte or Zwerchhau or Redoublement)
                {
                    const int
                    LONG_DELTA = 6,
                    PROC_DELTA = 5,
                    FINISHER_DELTA = 11,
                    IMBALANCE_DIFF_MAX = 30;

                    var inCombat = HasCondition(ConditionFlag.InCombat);

                    bool verfireUp = HasEffect(Buffs.VerfireReady);
                    bool verstoneUp = HasEffect(Buffs.VerstoneReady);
                    RDMGauge gauge = GetJobGauge<RDMGauge>();
                    int black = gauge.BlackMana;
                    int white = gauge.WhiteMana;
                    var canWeave = CanWeave(actionID);


                    if (IsEnabled(CustomComboPreset.SimpleRedMageOpener) && level >= Levels.Resolution)
                    {
                        if (inCombat && lastComboMove is RDM.Verthunder3 && HasEffect(Buffs.Dualcast) && !inOpener)
                        {
                            inOpener = true;
                        }

                        if (!inOpener)
                        {
                            return Verthunder3;
                        }

                        if (!inCombat && (inOpener || openerFinished))
                        {
                            inOpener = false;
                            openerFinished = false;
                            step = 0;

                            return Verthunder3;
                        }

                        if (inCombat && inOpener && !openerFinished)
                        {
                            //veraero
                            //swiftcast
                            //accel
                            //verthunder
                            //verthunder
                            //embolden
                            //manafication
                            //Riposte
                            //Fleche
                            //Zwercchau
                            //Contre-sixte
                            //Redoublement
                            //Corps-a-corps
                            //Engagement
                            //Verholy
                            //Corps-a-corps
                            //Engagement
                            //Scorch
                            //Resolution

                            //we do it in steps to be able to control it
                            if (step == 0)
                            {
                                if (lastComboMove == Veraero3) step++;
                                else return Veraero3;
                            }

                            if (step == 1)
                            {
                                if (IsOnCooldown(All.Swiftcast)) step++;
                                else return All.Swiftcast;
                            }

                            if (step == 2)
                            {
                                if (GetRemainingCharges(Acceleration) < 2) step++;
                                else return Acceleration;
                            }

                            if (step == 3)
                            {
                                if (lastComboMove == Verthunder3) step++;
                                else return Verthunder3;
                            }

                            if (step == 4)
                            {
                                if (lastComboMove == Verthunder3) step++;
                                else return Verthunder3;
                            }

                            if (step == 5)
                            {
                                if (IsOnCooldown(Embolden)) step++;
                                else return Embolden;
                            }

                            if (step == 6)
                            {
                                if (IsOnCooldown(Manafication)) step++;
                                else return Manafication;
                            }

                            if (step == 7)
                            {
                                if (lastComboMove == Riposte) step++;
                                else return EnchantedRiposte;
                            }

                            if (step == 8)
                            {
                                if (IsOnCooldown(Fleche)) step++;
                                else return Fleche;
                            }

                            if (step == 9)
                            {
                                if (lastComboMove == Zwerchhau) step++;
                                else return EnchantedZwerchhau;
                            }

                            if (step == 10)
                            {
                                if (IsOnCooldown(ContreSixte)) step++;
                                else return ContreSixte;
                            }

                            if (step == 11)
                            {
                                if (lastComboMove == Redoublement || gauge.ManaStacks == 3) step++;
                                else return EnchantedRedoublement;
                            }

                            if (step == 12)
                            {
                                if (GetRemainingCharges(Corpsacorps) < 2) step++;
                                else return Corpsacorps;
                            }

                            if (step == 13)
                            {
                                if (GetRemainingCharges(Engagement) < 2) step++;
                                else return Engagement;
                            }

                            if (step == 14)
                            {
                                if (lastComboMove == Verholy) step++;
                                else return Verholy;
                            }

                            if (step == 15)
                            {
                                if (GetRemainingCharges(Corpsacorps) < 1) step++;
                                else return Corpsacorps;
                            }

                            if (step == 16)
                            {
                                if (GetRemainingCharges(Engagement) < 1) step++;
                                else return Engagement;
                            }

                            if (step == 17)
                            {
                                if (lastComboMove == Scorch) step++;
                                else return Scorch;
                            }

                            if (step == 18)
                            {
                                if (lastComboMove == Resolution) step++;
                                else return Resolution;
                            }

                            openerFinished = true;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && GetCooldownRemainingTime(Engagement) < 35 &&
                        InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte) && level >= Levels.Engagement && !HasEffect(Buffs.Dualcast))
                        return Engagement;

                    if (IsEnabled(CustomComboPreset.RedMageCorpsACorpsFeature) && canWeave && GetCooldownRemainingTime(Corpsacorps) < 35 &&
                        ((InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte)) ||
                         (IsEnabled(CustomComboPreset.RedMageCorpsACorpsPullFeature) &&
                         gauge.BlackMana >= 50 && gauge.WhiteMana >= 50)) && level >= Levels.Corpsacorps && !HasEffect(Buffs.Dualcast))
                        return Corpsacorps;

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave)
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if ((lastComboMove is Riposte or EnchantedRiposte) && gauge.WhiteMana >= 30 && gauge.BlackMana >= 30 && (gauge.ManaStacks == 1 || level < Levels.ManaStack))
                    {
                        if (level >= Levels.Zwerchhau)
                            return OriginalHook(Zwerchhau);

                        else if (gauge.WhiteMana >= 20 && gauge.BlackMana >= 20)
                            return EnchantedRiposte;
                    }

                    if ((lastComboMove is Zwerchhau or EnchantedRiposte) && gauge.WhiteMana >= 15 && gauge.BlackMana >= 15 && (gauge.ManaStacks == 2 || level < Levels.ManaStack))
                    {
                        if (level >= Levels.Redoublement)
                            return OriginalHook(Redoublement);

                        else if (gauge.WhiteMana >= 20 && gauge.BlackMana >= 20)
                            return EnchantedRiposte;
                    }


                    if (InMeleeRange() && gauge.WhiteMana >= 50 && gauge.BlackMana >= 50 && !HasEffect(Buffs.Dualcast) &&
                        lastComboMove is not (Verholy or Verflare or Scorch) && (gauge.ManaStacks == 0 || level < Levels.ManaStack))
                        return EnchantedRiposte;

                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerSmartCastFeature))
                    {
                        if (!HasEffect(Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= Levels.Verthunder)
                            return OriginalHook(Verthunder);
                    }

                    if (IsEnabled(CustomComboPreset.SimpleRedMageFishing) && inCombat && canWeave)
                    {
                        if (!HasEffect(Buffs.VerfireReady) && !HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.Dualcast) &&
                            gauge.ManaStacks != 3 && lastComboMove is not (Verholy or Verflare or Scorch))
                        {
                            if (!HasEffect(Buffs.Acceleration) && HasCharges(Acceleration) && level >= Levels.Acceleration)
                                return Acceleration;

                            if (!IsEnabled(CustomComboPreset.SimpleRedMageAccelOnlyFishing) && !HasEffect(All.Buffs.Swiftcast) &&
                                IsOffCooldown(All.Swiftcast) && level >= All.Levels.Swiftcast)
                                return All.Swiftcast;
                        }
                    }

                    if (actionID is Veraero or Verthunder)
                    {

                        if (level < Levels.Verthunder)
                            return Jolt;

                        if (level is < Levels.Veraero and >= Levels.Verthunder)
                            return OriginalHook(Verthunder);

                        // This is for the long opener only, so we're not bothered about fast casting or finishers or anything like that
                        if (black < white)
                            return OriginalHook(Verthunder);

                        if (white < black)
                            return OriginalHook(Veraero);

                        return actionID;
                    }

                    if (actionID is Verstone or Verfire)
                    {
                    }

                    bool fastCasting = HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast);
                    bool accelerated = HasEffect(Buffs.Acceleration);
                    bool isFinishing1 = gauge.ManaStacks == 3;
                    bool isFinishing2 = comboTime > 0 && lastComboMove is Verholy or Verflare;
                    bool isFinishing3 = comboTime > 0 && lastComboMove is RDM.Scorch;
                    bool canFinishWhite = level >= Levels.Verholy;
                    bool canFinishBlack = level >= Levels.Verflare;
                    int blackThreshold = white + IMBALANCE_DIFF_MAX;
                    int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                    // If we're ready to Scorch or Resolution, just do that. Nice and simple. Sadly, that's where the simple ends.
                    if (isFinishing3 && level >= Levels.Resolution)
                        return Resolution;

                    if (isFinishing2 && level >= Levels.Scorch)
                        return Scorch;

                    if (isFinishing1 && canFinishBlack)
                    {
                        if (black >= white && canFinishWhite)
                        {

                            // If we can already Verstone, but we can't Verfire, and Verflare WON'T imbalance us, use Verflare
                            if (verstoneUp && !verfireUp && (black + FINISHER_DELTA <= blackThreshold))
                                return Verflare;

                            return Verholy;
                        }

                        // If we can already Verfire, but we can't Verstone, and we can use Verholy, and it WON'T imbalance us, use Verholy
                        if (verfireUp && !verstoneUp && canFinishWhite && (white + FINISHER_DELTA <= whiteThreshold))
                            return Verholy;

                        return Verflare;
                    }

                    if (fastCasting || accelerated)
                    {
                        if (level is < Levels.Veraero and >= Levels.Verthunder)
                            return Verthunder;

                        if (verfireUp == verstoneUp)
                        {

                            // Either both procs are already up or neither is - use whatever gives us the mana we need
                            if (black < white)
                                return OriginalHook(Verthunder);

                            if (white < black)
                                return OriginalHook(Veraero);

                            // If mana levels are equal, prioritise the colour that the original button was
                            return actionID is RDM.Verstone
                                ? OriginalHook(Veraero)
                                : OriginalHook(Verthunder);
                        }

                        if (verfireUp)
                        {

                            // If Veraero is feasible, use it
                            if (white + LONG_DELTA <= whiteThreshold)
                                return OriginalHook(Veraero);

                            return OriginalHook(Verthunder);
                        }

                        if (verstoneUp)
                        {

                            // If Verthunder is feasible, use it
                            if (black + LONG_DELTA <= blackThreshold)
                                return OriginalHook(Verthunder);

                            return OriginalHook(Veraero);
                        }
                    }

                    if (verfireUp && verstoneUp)
                    {

                        // Decide by mana levels
                        if (black < white)
                            return Verfire;

                        if (white < black)
                            return Verstone;

                        // If mana levels are equal, prioritise the original button
                        return actionID;
                    }

                    // Only use Verfire if it won't imbalance us
                    if (verfireUp && black + PROC_DELTA <= blackThreshold)
                        return Verfire;

                    // Only use Verstone if it won't imbalance us
                    if (verstoneUp && white + PROC_DELTA <= whiteThreshold)
                        return Verstone;

                    // If neither's up or the one that is would imbalance us, just use Jolt
                    return OriginalHook(Jolt2);
                }

                return actionID;
            }
        }

        internal class SimpleRedMageAoE : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleRedMageAoE;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Veraero2 or Verthunder2)
                {
                    const int
                    FINISHER_DELTA = 11,
                    IMBALANCE_DIFF_MAX = 30;
                    var accelBuff = HasEffect(Buffs.Acceleration);
                    var dualcastBuff = HasEffect(Buffs.Dualcast);
                    var swiftcastBuff = HasEffect(All.Buffs.Swiftcast);
                    var gauge = GetJobGauge<RDMGauge>();
                    int black = gauge.BlackMana;
                    int white = gauge.WhiteMana;
                    int blackThreshold = white + IMBALANCE_DIFF_MAX;
                    int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                    var canWeave = CanWeave(actionID);
                    var inCombat = HasCondition(ConditionFlag.InCombat);

                    if (inCombat && (lastComboMove is Veraero2 or Verthunder2))
                    {
                        SimpleRedMage.openerFinished = true;
                        SimpleRedMage.inOpener = true;
                    }

                    if (!inCombat)
                    {
                        SimpleRedMage.inOpener = false;
                        SimpleRedMage.step = 0;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && GetCooldownRemainingTime(Engagement) < 35 &&
                        InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte) && level >= Levels.Engagement && !HasEffect(Buffs.Dualcast))
                        return Engagement;

                    if (IsEnabled(CustomComboPreset.RedMageCorpsACorpsFeature) && canWeave && GetCooldownRemainingTime(Corpsacorps) < 35 &&
                        ((InMeleeRange() && IsOnCooldown(Fleche) && IsOnCooldown(ContreSixte)) ||
                         (IsEnabled(CustomComboPreset.RedMageCorpsACorpsPullFeature) &&
                         gauge.BlackMana >= 60 && gauge.WhiteMana >= 60)) && level >= Levels.Corpsacorps && !HasEffect(Buffs.Dualcast))
                        return Corpsacorps;

                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(Buffs.Dualcast))
                    {
                        if (level >= Levels.Fleche && IsOffCooldown(Fleche))
                            return Fleche;

                        if (level >= Levels.ContreSixte && IsOffCooldown(ContreSixte))
                            return ContreSixte;
                    }

                    if (!HasEffect(Buffs.Dualcast) && level >= Levels.Moulinet &&
                        lastComboMove is not (Verholy or Verflare or Scorch))
                    {
                        if (gauge.WhiteMana >= 20 && gauge.BlackMana >= 20 && (gauge.ManaStacks == 2 || level < Levels.ManaStack))
                            return EnchantedMoulinet;

                        if (gauge.WhiteMana >= 40 && gauge.BlackMana >= 40 && (gauge.ManaStacks == 1 || level < Levels.ManaStack))
                            return EnchantedMoulinet;

                        if (gauge.WhiteMana >= 60 && gauge.BlackMana >= 60 && InMeleeRange() && (gauge.ManaStacks == 0 || level < Levels.ManaStack))
                            return EnchantedMoulinet;
                    }

                    if (lastComboMove is RDM.Scorch && level >= Levels.Resolution)
                        return Resolution;

                    if (level >= Levels.Scorch && (lastComboMove is Verholy or Verflare))
                        return Scorch;

                    if (gauge.ManaStacks == 3 && level >= Levels.Verflare)
                    {
                        if (black >= white && level >= Levels.Verholy)
                        {
                            if (HasEffect(Buffs.VerstoneReady) && !HasEffect(Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                                return Verflare;

                            return Verholy;
                        }

                        if (HasEffect(Buffs.VerfireReady) && !HasEffect(Buffs.VerstoneReady) && level >= Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                            return Verholy;

                        return Verflare;
                    }

                    if (dualcastBuff || accelBuff || swiftcastBuff || HasEffect(Buffs.Chainspell))
                        return OriginalHook(Impact);

                    if (level < Levels.Verthunder2)
                        return Jolt;

                    if (gauge.BlackMana > gauge.WhiteMana && level >= Levels.Veraero2)
                        return Veraero2;

                    if (gauge.WhiteMana > gauge.BlackMana && level >= Levels.Verthunder2)
                        return Verthunder2;
                }

                return actionID;
            }
        }

        internal class RedMageMovementFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMovementFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RDM.Corpsacorps)
                {
                    if (InMeleeRange() && HasCharges(Displacement) && level >= Levels.Displacement)
                        return Displacement;
                }
                return actionID;
            }
        }

        // RedMageJoltVerprocCombo
        // Simple Single Target Combo. Shoves the Verfire/Verstone onto Jolt's button
        // Gives a user 3 sets of buttons for single target casting, Jolt+Procs, VerAero, VerThunder.
        // If Fire and Stone are both ready, use the one that is the lowest, while maintaining gauge balance
        internal class RedMageJoltVerprocCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageJoltVerprocCombo;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == OriginalHook(Jolt))
                {
                    var gauge = GetJobGauge<RDMGauge>();

                    if (lastComboMove is Verflare or Verholy or Scorch) return OriginalHook(actionID);

                    // If both are proc'd, use the one based on mana-gauge
                    if (HasEffect(Buffs.VerfireReady) && HasEffect(Buffs.VerstoneReady))
                    {
                        if (gauge.WhiteMana > gauge.BlackMana) return Verfire; else return Verstone;
                    }

                    // Avoiding Offbalance difference of 30, Proc adds 5, so 25
                    else if (HasEffect(Buffs.VerfireReady) && (gauge.BlackMana - gauge.WhiteMana) < 25) return Verfire;
                    else if (HasEffect(Buffs.VerstoneReady) && (gauge.WhiteMana - gauge.BlackMana) < 25) return Verstone;
                }
                return actionID;
            }
        }

        internal class RedMageLucidOnJolt : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageLucidOnJolt;

            internal static bool showLucid = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Verthunder or Veraero or Scatter or Verthunder3 or Veraero3 or Verthunder2 or Veraero2 or Impact or Jolt or Jolt2)
                {
                    var lucidThreshold = Service.Configuration.GetCustomIntValue(Config.RdmLucidMpThreshold);

                    if (level >= All.Levels.LucidDreaming && LocalPlayer.CurrentMp <= lucidThreshold) // Check to show Lucid Dreaming
                    {
                        showLucid = true;
                    }

                    if (showLucid && CanSpellWeave(actionID) && HasCondition(ConditionFlag.InCombat) && IsOffCooldown(All.LucidDreaming) && !HasEffect(Buffs.Dualcast)
                        && lastComboMove != EnchantedRiposte && lastComboMove != EnchantedZwerchhau
                        && lastComboMove != EnchantedRedoublement && lastComboMove != Verflare
                        && lastComboMove != Verholy && lastComboMove != Scorch) // Change abilities to Lucid Dreaming for entire weave window
                    {
                        return All.LucidDreaming;
                    }
                    showLucid = false;
                }
                return actionID;
            }
        }

        // RedMageSwiftVerraise
        // Swiftcast combos to Verraise when:
        //  Swiftcast is on cooldown.
        //  Swiftcast is available, but we we have Dualcast (Dualcasting verraise)
        //   Using this variation other than the alternatefeature style, as verrise is level 63
        //   and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
        internal class RedMageSwiftVerraise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSwiftVerraise;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast && level >= Levels.Verraise)
                {
                    if (GetCooldown(All.Swiftcast).CooldownRemaining > 0 ||   // Condition 1: Swiftcast is on cooldown
                        HasEffect(Buffs.Dualcast))                        // Condition 2: Swiftcast is available, but we have DualCast)
                        return Verraise;
                }

                // Else we just exit normally and return SwiftCast
                return actionID;
            }
        }
    }
}


