namespace XIVSlothComboPlugin.Combos
{
    internal static class BRDPvP
    {
        public const byte ClassID = 41;
        public const byte JobID = 23;

        public const uint
            PowerfulShot = 29391,
            ApexArrow = 29393,
            SilentNocturne = 29395,
            EmpyrealArrow = 29398,
            RepellingShot = 29399,
            WardensPaean = 29400,
            PitchPerfect = 29392,
            BlastArrow = 29394,
            FinalFantasia = 29401;

        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 3138,
                FrontlinersForte = 3140,
                Repertoire = 3137,
                BlastArrowReady = 3142;
        }

        internal class BurstShotFeaturePVP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                
                if (actionID == PowerfulShot)
                {
                    var canWeave = CanWeave(actionID, 0.5);
                    //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    if (canWeave)
                    {
                        if (GetCooldown(EmpyrealArrow).RemainingCharges == 3)
                            return OriginalHook(EmpyrealArrow);

                        if (!GetCooldown(SilentNocturne).IsCooldown)
                            return OriginalHook(SilentNocturne);
                    }

                    if (HasEffect(Buffs.BlastArrowReady))
                        return OriginalHook(BlastArrow);

                    if (HasEffect(Buffs.Repertoire))
                        return OriginalHook(PowerfulShot);

                    if (!GetCooldown(ApexArrow).IsCooldown)
                        return OriginalHook(ApexArrow);

                    return OriginalHook(PowerfulShot);
                }
                
                return actionID;
            }
        }
    }
}