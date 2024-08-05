using Dalamud.Game.ClientState.Conditions;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DOL
    {
        public const byte ClassID = 0;
        public const byte JobID = 51;

        internal const uint
            //BTN & MIN
            AgelessWords = 215,
            SolidReason = 232,
            MinWiseToTheWorld = 26521,
            BtnWiseToTheWorld = 26522,
            Prospect = 227,
            LayOfTheLand = 228,
            LayOfTheLand2 = 291,
            TruthOfMountains = 238,
            Triangulate = 210,
            ArborCall = 211,
            ArborCall2 = 290,
            TruthOfForests = 221,
            //FSH
            Cast = 289,
            Hook = 296,
            Mooch = 297,
            MoochII = 268,
            CastLight = 2135,
            Snagging = 4100,
            Chum = 4104,
            FishEyes = 4105,
            SurfaceSlap = 4595,
            //FSH Diving
            Gig = 7632,
            SharkEye = 7904,
            SharkEyeII = 7905,
            VeteranTrade = 7906,
            NaturesBounty = 7909,
            Salvage = 7910,
            PrizeCatch = 26806,
            VitalSight = 26870,
            BaitedBreath = 26871,
            ElectricCurrent = 26872;

        internal static class Buffs
        {
            internal const ushort
                TruthOfForests = 221,
                TruthOfMountains = 222,
                Triangulate = 217,
                Prospect = 225,
                EurekaMoment = 2765;
        }

        internal static class Debuffs
        {
            internal const ushort
                Placeholder = 0;
        }

        internal class DOL_Eureka : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DOL_Eureka;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SolidReason && HasEffect(Buffs.EurekaMoment)) return MinWiseToTheWorld;
                if (actionID is AgelessWords && HasEffect(Buffs.EurekaMoment)) return BtnWiseToTheWorld;
                return actionID;
            }
        }

        internal class DOL_NodeSearchingBuffs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DOL_NodeSearchingBuffs;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                //MIN
                if (actionID is DOL.LayOfTheLand && !HasEffect(Buffs.Prospect)) return Prospect;
                if (actionID is DOL.LayOfTheLand2 && LevelChecked(TruthOfMountains) && !HasEffect(Buffs.TruthOfMountains)) return TruthOfMountains;
                //BTN
                if (actionID is DOL.ArborCall && !HasEffect(Buffs.Triangulate)) return Triangulate;
                if (actionID is DOL.ArborCall2 && LevelChecked(TruthOfForests) && !HasEffect(Buffs.TruthOfForests)) return TruthOfForests;
                return actionID;
            }
        }

        internal class FSH_CastHook : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.FSH_CastHook;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                => actionID is Cast && HasCondition(ConditionFlag.Fishing) ? Hook : actionID;
        }

        internal class FSH_Swim : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.FSH_Swim;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (HasCondition(ConditionFlag.Diving))
                {
                    if (actionID is Cast && IsEnabled(CustomComboPreset.FSH_CastGig)) return Gig;
                    if (actionID is SurfaceSlap && IsEnabled(CustomComboPreset.FSH_SurfaceTrade)) return VeteranTrade;
                    if (actionID is PrizeCatch && IsEnabled(CustomComboPreset.FSH_PrizeBounty)) return NaturesBounty;
                    if (actionID is Snagging && IsEnabled(CustomComboPreset.FSH_SnaggingSalvage)) return Salvage;
                    if (actionID is CastLight && IsEnabled(CustomComboPreset.FSH_CastLight_ElectricCurrent)) return ElectricCurrent;
                    if (IsEnabled(CustomComboPreset.FSH_Mooch_SharkEye))
                    {
                        if (actionID is Mooch) return SharkEye;
                        if (actionID is MoochII) return SharkEyeII;
                    }
                    if (actionID is FishEyes && IsEnabled(CustomComboPreset.FSH_FishEyes_VitalSight)) return VitalSight;
                    if (actionID is Chum && IsEnabled(CustomComboPreset.FSH_Chum_BaitedBreath)) return BaitedBreath;
                }

                return actionID;
            }
        }
    }
}