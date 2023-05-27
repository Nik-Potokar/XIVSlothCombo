using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class NINPvP
    {
        public const byte ClassID = 29;
        public const byte JobID = 30;

        internal const uint
            SpinningEdge = 29500,
            GustSlash = 29501,
            AeolianEdge = 29502,
            FumaShuriken = 29505,
            Mug = 29509,
            ThreeMudra = 29507,
            Bunshin = 29511,
            Shukuchi = 29513,
            SeitonTenchu = 29515,
            ForkedRaiju = 29510,
            FleetingRaiju = 29707,
            HyoshoRanryu = 29506,
            GokaMekkyaku = 29504,
            Meisui = 29508,
            Huton = 29512,
            Doton = 29514,
            Assassinate = 29503;

        internal class Buffs
        {
            internal const ushort
                ThreeMudra = 1317,
                Hidden = 1316,
                Bunshin = 2010,
                ShadeShift = 2011;
        }

        internal class Debuffs
        {
            internal const ushort
                SealedHyoshoRanryu = 3194,
                SealedGokaMekkyaku = 3193,
                SealedHuton = 3196,
                SealedDoton = 3197,
                SeakedForkedRaiju = 3195,
                SealedMeisui = 3198;
        }

        internal class Config
        {
            internal const string
                NINPvP_Meisui_ST = "NINPvP_Meisui_ST",
                NINPvP_Meisui_AoE = "NINPvP_Meisui_AoE";
        }

        internal class NINPvP_ST_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINPvP_ST_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is SpinningEdge or GustSlash or AeolianEdge)
                {
                    var threeMudrasCD = GetCooldown(ThreeMudra);
                    var fumaCD = GetCooldown(FumaShuriken);
                    var bunshinStacks = HasEffect(Buffs.Bunshin) ? GetBuffStacks(Buffs.Bunshin) : 0;
                    bool raijuLocked = HasEffect(Debuffs.SeakedForkedRaiju);
                    bool meisuiLocked = HasEffect(Debuffs.SealedMeisui);
                    bool hyoshoLocked = HasEffect(Debuffs.SealedHyoshoRanryu);
                    bool dotonLocked = HasEffect(Debuffs.SealedDoton);
                    bool gokaLocked = HasEffect(Debuffs.SealedGokaMekkyaku);
                    bool hutonLocked = HasEffect(Debuffs.SealedHuton);
                    bool mudraMode = HasEffect(Buffs.ThreeMudra);
                    bool canWeave = CanWeave(SpinningEdge);
                    var jobMaxHp = LocalPlayer.MaxHp;
                    var threshold = GetOptionValue(Config.NINPvP_Meisui_ST);
                    var maxHPThreshold = jobMaxHp - 8000;
                    var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;
                    bool inMeisuiRange = threshold >= (remainingPercentage * 100);


                    if (HasEffect(Buffs.Hidden))
                        return OriginalHook(Assassinate);

                    if (canWeave)
                    {
                        if (InMeleeRange() && !GetCooldown(Mug).IsCooldown)
                            return OriginalHook(Mug);

                        if (!GetCooldown(Bunshin).IsCooldown)
                            return OriginalHook(Bunshin);

                        if (threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                            return OriginalHook(ThreeMudra);
                    }

                    if (mudraMode)
                    {
                        if (IsEnabled(CustomComboPreset.NINPvP_ST_Meisui) && inMeisuiRange && !meisuiLocked)
                            return OriginalHook(Meisui);

                        if (!hyoshoLocked)
                            return OriginalHook(HyoshoRanryu);

                        if (!raijuLocked && bunshinStacks > 0)
                            return OriginalHook(ForkedRaiju);

                        if (!hutonLocked)
                            return OriginalHook(Huton);
                    }

                    if (fumaCD.RemainingCharges > 0)
                        return OriginalHook(FumaShuriken);

                }

                return actionID;
            }
        }

        internal class NINPvP_AoE_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINPvP_AoE_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == FumaShuriken)
                {
                    var threeMudrasCD = GetCooldown(ThreeMudra);
                    var fumaCD = GetCooldown(FumaShuriken);
                    var bunshinStacks = HasEffect(Buffs.Bunshin) ? GetBuffStacks(Buffs.Bunshin) : 0;
                    bool raijuLocked = HasEffect(Debuffs.SeakedForkedRaiju);
                    bool meisuiLocked = HasEffect(Debuffs.SealedMeisui);
                    bool hyoshoLocked = HasEffect(Debuffs.SealedHyoshoRanryu);
                    bool dotonLocked = HasEffect(Debuffs.SealedDoton);
                    bool gokaLocked = HasEffect(Debuffs.SealedGokaMekkyaku);
                    bool hutonLocked = HasEffect(Debuffs.SealedHuton);
                    bool mudraMode = HasEffect(Buffs.ThreeMudra);
                    bool canWeave = CanWeave(SpinningEdge);
                    var jobMaxHp = LocalPlayer.MaxHp;
                    var threshold = GetOptionValue(Config.NINPvP_Meisui_AoE);
                    var maxHPThreshold = jobMaxHp - 8000;
                    var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;
                    bool inMeisuiRange = threshold >= (remainingPercentage * 100);

                    if (HasEffect(Buffs.Hidden))
                        return OriginalHook(Assassinate);

                    if (canWeave)
                    {
                        if (InMeleeRange() && !GetCooldown(Mug).IsCooldown)
                            return OriginalHook(Mug);

                        if (!GetCooldown(Bunshin).IsCooldown)
                            return OriginalHook(Bunshin);

                        if (threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                            return OriginalHook(ThreeMudra);
                    }

                    if (mudraMode)
                    {
                        if (IsEnabled(CustomComboPreset.NINPvP_AoE_Meisui) && inMeisuiRange && !meisuiLocked)
                            return OriginalHook(Meisui);

                        if (!dotonLocked)
                            return OriginalHook(Doton);

                        if (!gokaLocked)
                            return OriginalHook(GokaMekkyaku);
                    }

                    if (fumaCD.RemainingCharges > 0)
                        return OriginalHook(FumaShuriken);

                    if (InMeleeRange())
                    {
                        if (lastComboActionID == GustSlash)
                            return OriginalHook(AeolianEdge);

                        if (lastComboActionID == SpinningEdge)
                            return OriginalHook(GustSlash);

                        return OriginalHook(SpinningEdge);
                    }
                }

                return actionID;
            }
        }
    }
}