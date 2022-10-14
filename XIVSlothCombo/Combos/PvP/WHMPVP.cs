using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class WHMPVP
    {
        public const byte JobID = 24;

        public const uint
            Glare = 29223,
            Cure = 29224,
            AfflatusMisery = 29226,
            Aquaveil = 29227,
            MiracleOfNature = 29228,
            SeraphStrike = 29229;

        internal class Buffs
        {
            internal const ushort
                Cure3Ready = 3083;
        }

        internal class WHMPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Glare)
                {
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);

                    if (!enemyGuarded)
                    {
                        if (IsEnabled(CustomComboPreset.WHMPvP_Afflatus_Misery) && IsOffCooldown(AfflatusMisery))
                            return AfflatusMisery;

                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.WHMPvP_Mirace_of_Nature) && IsOffCooldown(MiracleOfNature))
                                return MiracleOfNature;

                            if (IsEnabled(CustomComboPreset.WHMPvP_Seraph_Strike) && IsOffCooldown(SeraphStrike))
                                return SeraphStrike;
                        }
                    }
                }
                return actionID;
            }
        }
        internal class WHMPvP_Heal : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMPvP_Heal;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cure)
                {
                    if (IsOffCooldown(Aquaveil))
                        return Aquaveil;
                }
                return actionID;
            }
        }
    }
}