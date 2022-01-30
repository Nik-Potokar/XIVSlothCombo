using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.JobGauge.Enums;

namespace XIVSlothComboPlugin.Combos
{
    internal static class DRKPVP
    {
        public const byte ClassID = 41;
        public const byte JobID = 32;

        public const uint
            SoulEaterCombo = 6,
            EdgeOfShadow = 17701,
            BloodSpiller = 8776,
            HardSlash = 8769,
            SyphonStrike = 8772,
            SoulEater = 8773,
            Unleash = 18906,
            StalwartSoul = 18907,
            FloodOfShadow = 18908,
            Quietus = 17700;
            


        public static class Buffs
        {
            public const short
                Delirium = 1996;
                
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Placeholder = 0;
        }
    }
    internal class SouleaterComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SouleaterComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRKPVP.HardSlash || actionID == DRKPVP.SyphonStrike || actionID == DRKPVP.SoulEater)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var actionCD = GetCooldown(actionID);
                if (actionCD.IsCooldown && LocalPlayer.CurrentMp >= 2500)
                    return DRKPVP.EdgeOfShadow;
                if(HasEffect(DRKPVP.Buffs.Delirium) || gauge.Blood >= 50 )
                {
                    return DRKPVP.BloodSpiller;
                }
            }

            return OriginalHook(actionID);
        }
    }
    internal class StalwartSoulComboFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.StalwartSoulComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRKPVP.StalwartSoul || actionID == DRKPVP.Unleash)
            {
                var gauge = GetJobGauge<DRKGauge>();
                var actionCD = GetCooldown(actionID);
                if (actionCD.IsCooldown && LocalPlayer.CurrentMp >= 2500)
                    return DRKPVP.FloodOfShadow;
                if (HasEffect(DRKPVP.Buffs.Delirium) || gauge.Blood >= 50)
                {
                    return DRKPVP.Quietus;
                }
            }

            return OriginalHook(actionID);
        }
    }

}
