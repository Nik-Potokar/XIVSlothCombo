using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class SMNPvP
    {
        public const byte ClassID = 26;
        public const byte JobID = 27;

        internal const uint
            Ruin3 = 29664,
            AstralImpulse = 29665,
            FountainOfFire = 29666,
            CrimsonCyclone = 29667,
            CrimsonStrike = 29668,
            Slipstream = 29669,
            RadiantAegis = 29670,
            MountainBuster = 29671,
            Fester = 29672,
            EnkindleBahamut = 29674,
            Megaflare = 29675,          // unused
            Wyrmwave = 29676,           // unused
            AkhMorn = 29677,            // unused
            EnkindlePhoenix = 29679,
            ScarletFlame = 29681,       // unused
            Revelation = 29682;         // unused

        public static class Config
        {
            public const string
                SMNPvP_RadiantAegisThreshold = "SMNPvP_RadiantAegisThreshold";
            public const string
                SMNPvP_FesterThreshold = "SMNPvP_FesterThreshold";
        }

        internal class SMNPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMNPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ruin3)
                {
                    #region Types
                    bool canWeave = CanWeave(actionID);
                    bool bahamutBurst = OriginalHook(Ruin3) is AstralImpulse;
                    bool phoenixBurst = OriginalHook(Ruin3) is FountainOfFire;
                    double playerHP = PlayerHealthPercentageHp();
                    double targetHP = GetTargetHPPercent();
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                    bool canBind = !TargetHasEffectAny(PvPCommon.Debuffs.Bind);
                    int radiantThreshold = PluginConfiguration.GetCustomIntValue(Config.SMNPvP_RadiantAegisThreshold);
                    int festerThreshold = PluginConfiguration.GetCustomIntValue(Config.SMNPvP_FesterThreshold);
                    #endregion

                    if (canWeave)
                    {
                        // Radiant Aegis
                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_RadiantAegis) &&
                            IsOffCooldown(RadiantAegis) && playerHP <= radiantThreshold)
                            return RadiantAegis;

                        // Fester
                        if (HasCharges(Fester) && targetHP <= festerThreshold && !enemyGuarded &&
                            !(phoenixBurst || bahamutBurst)) // Lazy method for correct (?) priority
                            return Fester;
                    }

                    // Phoenix & Bahamut bursts
                    if (phoenixBurst || bahamutBurst)
                    {
                        if (!enemyGuarded && canWeave)
                        {
                            if (IsOffCooldown(EnkindlePhoenix) && phoenixBurst)
                                return EnkindlePhoenix;
                            if (IsOffCooldown(EnkindleBahamut) && bahamutBurst)
                                return EnkindleBahamut;
                            if (HasCharges(Fester) && targetHP <= festerThreshold)
                                return Fester;
                            if (IsOffCooldown(MountainBuster))
                                return MountainBuster;
                        }
                    }

                    // Titan
                    if (IsOffCooldown(MountainBuster) && canWeave && canBind && !enemyGuarded)
                        return MountainBuster;

                    // Ifrit
                    if (!enemyGuarded)
                    {
                        if (OriginalHook(CrimsonCyclone) is CrimsonStrike)
                            return CrimsonStrike;
                        if (IsOffCooldown(CrimsonCyclone) && InMeleeRange())
                            return CrimsonCyclone;
                    }

                    // Garuda
                    if (IsOffCooldown(Slipstream))
                        return Slipstream;
                }

                return actionID;
            }
        }
    }
}
