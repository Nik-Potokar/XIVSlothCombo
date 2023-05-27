using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal class DRKPvP
    {
        public const uint
            HardSlash = 29085,
            SyphonStrike = 29086,
            Souleater = 29087,
            Quietus = 29737,
            Shadowbringer = 29091,
            Plunge = 29092,
            BlackestNight = 29093,
            SaltedEarth = 29094,
            Bloodspiller = 29088,
            SaltAndDarkness = 29095;

        public class Buffs
        {
            public const ushort
                Blackblood = 3033,
                BlackestNight = 1038,
                SaltedEarthDMG = 3036,
                SaltedEarthDEF = 3037,
                DarkArts = 3034,
                UndeadRedemption = 3039;
        }

        public class Config
        {
            public const string
                ShadowbringerThreshold = nameof(ShadowbringerThreshold);

        }

        internal class DRKPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRKPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is HardSlash or SyphonStrike or Souleater)
                {
                    bool canWeave = CanWeave(HardSlash);
                    int shadowBringerThreshold = GetOptionValue(Config.ShadowbringerThreshold);


                    if (IsEnabled(CustomComboPreset.DRKPvP_Plunge) && HasTarget() && ((!InMeleeRange()) || (InMeleeRange() && IsEnabled(CustomComboPreset.DRKPvP_PlungeMelee))) && ActionReady(Plunge))
                        return OriginalHook(Plunge);

                    if (canWeave)
                    {
                        if (ActionReady(BlackestNight))
                            return OriginalHook(BlackestNight);

                        if (ActionReady(SaltedEarth))
                            return OriginalHook(SaltedEarth);

                        if (HasEffect(Buffs.SaltedEarthDMG) && ActionReady(SaltAndDarkness))
                            return OriginalHook(SaltAndDarkness);

                        if (!HasEffect(Buffs.Blackblood) && (HasEffect(Buffs.DarkArts) || PlayerHealthPercentageHp() >= shadowBringerThreshold))
                            return OriginalHook(Shadowbringer);
                    }

                    if (InMeleeRange())
                    {
                        if (ActionReady(Quietus))
                            return OriginalHook(Quietus);

                        if (comboTime > 1f)
                        {
                            if (lastComboActionID == HardSlash)
                                return OriginalHook(SyphonStrike);

                            if (lastComboActionID == SyphonStrike)
                                return OriginalHook(Souleater);
                        }

                        return OriginalHook(HardSlash);
                    }
                }

                return actionID;
            }
        }

    }
}
