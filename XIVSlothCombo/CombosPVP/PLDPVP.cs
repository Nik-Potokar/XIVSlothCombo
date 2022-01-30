namespace XIVSlothComboPlugin.Combos
{
    internal static class PLDPVP
    {
        public const byte ClassID = 1;
        public const byte JobID = 19;

        public const float CooldownThreshold = 0.5f;

        public const uint
            FastBlade = 8746,
            RiotBlade = 8749,
            RoyalAuthority = 8750,
            TotalEclipse = 18900,
            Prominence = 18901,
            HolySpirit = 8752,
            HolyCircle = 17693;



        public static class Buffs
        {
            public const ushort
                Requiescat = 1369;
        }
    }
    internal class RoyalAuthorityComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RoyalAuthorityComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLDPVP.FastBlade || actionID == PLDPVP.RiotBlade || actionID == PLDPVP.RoyalAuthority)
            {
                if (HasEffect(PLDPVP.Buffs.Requiescat))
                {
                    return PLDPVP.HolySpirit;
                }
            }

            return OriginalHook(actionID);
        }
    }
    internal class ProminenceComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ProminenceComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLDPVP.TotalEclipse || actionID == PLDPVP.Prominence)
            {
                if (HasEffect(PLDPVP.Buffs.Requiescat))
                {
                    return PLDPVP.HolyCircle;
                }
            }

            return OriginalHook(actionID);
        }
    }
}
