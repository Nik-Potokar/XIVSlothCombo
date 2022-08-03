using Dalamud.Game.ClientState.Conditions;
using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DOL
    {
        public const byte ClassID = 0;
        public const byte JobID = 51;

        public const uint
            //BTN & MIN
            AgelessWords = 215,
            SolidReason = 232,
            //FSH
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

        internal class DOL_Eureka : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DOL_Eureka;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SolidReason && LevelChecked(MinWiseToTheWorld) && HasEffect(Buffs.EurekaMoment)) return MinWiseToTheWorld;
                if (actionID is AgelessWords && LevelChecked(BtnWiseToTheWorld) && HasEffect(Buffs.EurekaMoment)) return BtnWiseToTheWorld;
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
                if (actionID is Cast && IsEnabled(CustomComboPreset.FSH_CastGig) && HasCondition(ConditionFlag.Diving)) return Gig;
                if (actionID is SurfaceSlap && IsEnabled(CustomComboPreset.FSH_SurfaceTrade) && HasCondition(ConditionFlag.Diving)) return VeteranTrade;
                if (actionID is PrizeCatch && IsEnabled(CustomComboPreset.FSH_PrizeBounty) && HasCondition(ConditionFlag.Diving)) return NaturesBounty;
                if (actionID is Snagging && IsEnabled(CustomComboPreset.FSH_SnaggingSalvage) && HasCondition(ConditionFlag.Diving)) return Salvage;
                if (actionID is CastLight && IsEnabled(CustomComboPreset.FSH_CastLight_ElectricCurrent) && HasCondition(ConditionFlag.Diving)) return ElectricCurrent;
                return actionID;
            }
        }
    }
}