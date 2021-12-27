namespace XIVSlothComboPlugin.Combos
{
    internal static class DoM
    {
        public const byte JobID = 99;

        public const uint
            Swiftcast = 7561;

        public static class Buffs
        {
            public const short
                Swiftcast = 167;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Raise = 12;
        }
    }

    internal class DoMSwiftcastFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DoMSwiftcastFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.DoMSwiftcastFeature))
            {
                if (actionID == WHM.Raise || actionID == SMN.Resurrection || actionID == SGE.Egeiro || actionID == AST.Ascend || actionID == RDM.Verraise)
                {
                    var swiftCD = GetCooldown(DoM.Swiftcast);
                    if ((swiftCD.CooldownRemaining == 0 && !HasEffect(RDM.Buffs.Dualcast))
                        || level <= DoM.Levels.Raise
                        || (level <= RDM.Levels.Verraise && actionID == RDM.Verraise))
                        return DoM.Swiftcast;
                }
            }

            return actionID;
        }
    }
}
