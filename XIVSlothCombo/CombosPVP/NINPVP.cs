using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class NINPVP
    {
        public const byte ClassID = 18;
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
                SealedDoton = 3187,
                SeakedForkedRaiju = 3195,
                SealedMeisui = 3198;
        }
    }

    internal class NINBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is NINPVP.SpinningEdge or NINPVP.AeolianEdge or NINPVP.GustSlash)
            {
                //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                //if (globalAction != actionID) return globalAction;

                var threeMudrasCD = GetCooldown(NINPVP.ThreeMudra);
                var fumaCD = GetCooldown(NINPVP.FumaShuriken);
                var bunshinStacks = HasEffect(NINPVP.Buffs.Bunshin) ? GetBuffStacks(NINPVP.Buffs.Bunshin) : 0;
                bool raijuLocked = HasEffect(NINPVP.Debuffs.SeakedForkedRaiju);
                bool meisuiLocked = HasEffect(NINPVP.Debuffs.SealedMeisui);
                bool hyoshoLocked = HasEffect(NINPVP.Debuffs.SealedHyoshoRanryu);
                bool dotonLocked = HasEffect(NINPVP.Debuffs.SealedDoton);
                bool gokaLocked = HasEffect(NINPVP.Debuffs.SealedGokaMekkyaku);
                bool hutonLocked = HasEffect(NINPVP.Debuffs.SealedHuton);
                bool mudraMode = HasEffect(NINPVP.Buffs.ThreeMudra);
                bool canWeave = CanWeave(actionID);

                if (HasEffect(NINPVP.Buffs.Hidden))
                    return OriginalHook(NINPVP.Assassinate);

                if (InMeleeRange() && !GetCooldown(NINPVP.Mug).IsCooldown && canWeave)
                    return OriginalHook(NINPVP.Mug);

                if (!GetCooldown(NINPVP.Bunshin).IsCooldown && canWeave)
                    return OriginalHook(NINPVP.Bunshin);

                if (threeMudrasCD.RemainingCharges > 0 && !mudraMode && canWeave)
                    return OriginalHook(NINPVP.ThreeMudra);

                if (mudraMode && !hyoshoLocked)
                    return OriginalHook(NINPVP.HyoshoRanryu);

                if (mudraMode && !raijuLocked && bunshinStacks > 0)
                    return OriginalHook(NINPVP.ForkedRaiju);

                if (mudraMode && !hutonLocked)
                    return NINPVP.Huton;

                if (fumaCD.RemainingCharges > 0)
                    return OriginalHook(NINPVP.FumaShuriken);

            }

            return actionID;
        }
    }

    internal class NINAoEBurstMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINAoEBurstMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == NINPVP.FumaShuriken)
            {
                //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                //if (globalAction != actionID) return globalAction;

                var threeMudrasCD = GetCooldown(NINPVP.ThreeMudra);
                var fumaCD = GetCooldown(NINPVP.FumaShuriken);
                var bunshinStacks = HasEffect(NINPVP.Buffs.Bunshin) ? GetBuffStacks(NINPVP.Buffs.Bunshin) : 0;
                bool raijuLocked = HasEffect(NINPVP.Debuffs.SeakedForkedRaiju);
                bool meisuiLocked = HasEffect(NINPVP.Debuffs.SealedMeisui);
                bool hyoshoLocked = HasEffect(NINPVP.Debuffs.SealedHyoshoRanryu);
                bool dotonLocked = HasEffect(NINPVP.Debuffs.SealedDoton);
                bool gokaLocked = HasEffect(NINPVP.Debuffs.SealedGokaMekkyaku);
                bool hutonLocked = HasEffect(NINPVP.Debuffs.SealedHuton);
                bool mudraMode = HasEffect(NINPVP.Buffs.ThreeMudra);
                bool canWeave = CanWeave(actionID);

                if (InMeleeRange() && !GetCooldown(NINPVP.Mug).IsCooldown && canWeave)
                    return NINPVP.Mug;

                if (!GetCooldown(NINPVP.Bunshin).IsCooldown && canWeave)
                    return NINPVP.Bunshin;

                if (threeMudrasCD.RemainingCharges > 0 && !mudraMode && canWeave)
                    return OriginalHook(NINPVP.ThreeMudra);

                if (mudraMode && !dotonLocked)
                    return OriginalHook(NINPVP.Doton);

                if (mudraMode && !gokaLocked)
                    return OriginalHook(NINPVP.GokaMekkyaku);

                if (fumaCD.RemainingCharges > 0)
                    return OriginalHook(NINPVP.FumaShuriken);

                if (InMeleeRange())
                {
                    if (lastComboActionID == NINPVP.GustSlash)
                        return OriginalHook(NINPVP.AeolianEdge);

                    if (lastComboActionID == NINPVP.SpinningEdge)
                        return OriginalHook(NINPVP.GustSlash);


                    return OriginalHook(NINPVP.SpinningEdge);
                }
            }

            return actionID;
        }
    }
}
