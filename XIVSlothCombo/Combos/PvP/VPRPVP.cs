using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class VPRPvP
    {
        public const byte JobID = 41;

        internal const uint
            SteelFangs = 39157,
            HuntersSting = 39159,
            BarbarousSlice = 39161,
            PiercingFangs = 39158,
            SwiftskinsSting = 39160,
            RavenousBite = 39163,
            HuntersSnap = 39166,
            SwiftskinsCoil = 39167,
            UncoiledFury = 39168,
            SerpentsTail = 39183,
            FirstGeneration = 39169,
            SecondGeneration = 39170,
            ThirdGeneration = 39171,
            FourthGeneration = 39172,
            Ouroboros = 39173;

        internal class VPRPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPRPvP_Burst;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Variables
                bool canWeave = CanWeave(OriginalHook(actionID));
                bool isSerpentsTailPrimed = OriginalHook(SerpentsTail) != SerpentsTail;
                bool inGenerationsCombo = OriginalHook(actionID) is FirstGeneration or SecondGeneration or ThirdGeneration or FourthGeneration;
                bool inMeleeRange = GetTargetDistance() <= 5;
                #endregion

                if (actionID is SteelFangs or HuntersSting or BarbarousSlice or PiercingFangs or SwiftskinsSting or RavenousBite)
                {
                    if (HasTarget())
                    {
                        // Serpent's Tail
                        if (isSerpentsTailPrimed && canWeave)
                            return OriginalHook(SerpentsTail);

                        if (inMeleeRange)
                        {
                            // Ouroboros
                            if (OriginalHook(HuntersSnap) == Ouroboros && !inGenerationsCombo)
                                return OriginalHook(HuntersSnap);

                            // Reawakened
                            if (inGenerationsCombo)
                                return OriginalHook(actionID);
                        }

                        // Uncoiled Fury
                        if (IsOffCooldown(UncoiledFury) && (!inMeleeRange || (!HasCharges(HuntersSnap) && OriginalHook(HuntersSnap) != SwiftskinsCoil)))
                            return OriginalHook(UncoiledFury);

                        // Hunter's Snap / Swiftskin's Coil
                        if ((HasCharges(HuntersSnap) && OriginalHook(HuntersSnap) != Ouroboros) || OriginalHook(HuntersSnap) == SwiftskinsCoil)
                            return OriginalHook(HuntersSnap);
                    }
                }

                return actionID;
            }
        }
    }
}