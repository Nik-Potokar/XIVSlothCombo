using Dalamud.Game.ClientState.Conditions;

namespace XIVSlothComboPlugin.Combos
{
    internal static class DOL
    {
        public const byte ClassID = 0;
        public const byte JobID = 51;

        public const uint
            AgelessWords = 215,
            SolidReason = 232,
            Cast = 289,
            Hook = 296,
            CastLight = 2135,
            Snagging = 4100,
            SurfaceSlap = 4595,
            Gig = 7632,
            VeteranTrade = 7906,
            NaturesBounty = 7909,
            Salvage = 7910,
            MinWiseToTheWorld = 26521,
            BtnWiseToTheWorld = 26522,
            ElectricCurrent = 26872,
            PrizeCatch = 26806;

        public static class Buffs
        {
            public const ushort
                EurekaMoment = 2765;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Cast = 1,
                Hook = 1,
                Snagging = 36,
                Gig = 61,
                Salvage = 67,
                VeteranTrade = 63,
                NaturesBounty = 69,
                SurfaceSlap = 71,
                PrizeCatch = 81,
                WiseToTheWorld = 90;
        }
    

    internal class MinerEurekaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolEurekaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SolidReason)
            {
                if (level >= Levels.WiseToTheWorld && HasEffect(Buffs.EurekaMoment))
                    return MinWiseToTheWorld;
            }

            if (actionID == AgelessWords)
            {
                if (level >= Levels.WiseToTheWorld && HasEffect(Buffs.EurekaMoment))
                    return BtnWiseToTheWorld;
            }

            return actionID;
        }
    }

        internal class FisherCast : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Cast)
                {
                    if (IsEnabled(CustomComboPreset.DolCastHookFeature))
                    {
                        if (HasCondition(ConditionFlag.Fishing))
                            return Hook;
                    }

                    if (IsEnabled(CustomComboPreset.DolCastGigFeature))
                    {
                        if (HasCondition(ConditionFlag.Diving))
                            return Gig;
                    }
                }

                if (actionID == SurfaceSlap)
                {
                    if (IsEnabled(CustomComboPreset.DolSurfaceTradeFeature))
                    {
                        if (HasCondition(ConditionFlag.Diving))
                            return VeteranTrade;
                    }
                }

                if (actionID == PrizeCatch)
                {
                    if (IsEnabled(CustomComboPreset.DolPrizeBountyFeature))
                    {
                        if (HasCondition(ConditionFlag.Diving))
                            return NaturesBounty;
                    }
                }

                if (actionID == Snagging)
                {
                    if (IsEnabled(CustomComboPreset.DolSnaggingSalvageFeature))
                    {
                        if (HasCondition(ConditionFlag.Diving))
                            return Salvage;
                    }
                }

                if (actionID == CastLight)
                {
                    if (IsEnabled(CustomComboPreset.DolCastLightElectricCurrentFeature))
                    {
                        if (HasCondition(ConditionFlag.Diving))
                            return ElectricCurrent;
                    }
                }

                return actionID;
            }
        }
    }
}