using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Benefic = 3594,
            Benefic2 = 3610,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            MinorArcana = 7443,
            SleeveDraw = 7448,
            Malefic4 = 16555,
            LucidDreaming = 7562,
            Ascend = 3603,
            Swiftcast = 7561,
            CrownPlay = 25869,
            Astrodyne = 25870,
            FallMalefic = 25871,
            Malefic1 = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Combust = 3599,
            Play = 17055,
            LordOfCrowns = 7444,
            LadyOfCrown = 7445;

        public static class Buffs
        {
            public const short
            Swiftcast = 167,
            LordOfCrownsDrawn = 2054,
            LadyOfCrownsDrawn = 2055;
        }

        public static class Debuffs
        {
            public const short
            Combust1 = 1881,
            Combust2 = 843,
            Combust3 = 838;

        }

        public static class Levels
        {
            public const byte
                Benefic2 = 26,
                MinorArcana = 50,
                Draw = 30,
                CrownPlay = 70;


        }
    }

    internal class AstrologianCardsOnDrawFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianCardsOnDrawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Play)
            {
                var gauge = GetJobGauge<ASTGauge>();
                if (level >= AST.Levels.Draw && gauge.DrawnCard == CardType.NONE)
                    return AST.Draw;
            }

            return actionID;
        }
    }

    internal class AstrologianCrownPlayFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianCrownPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.CrownPlay)
            {
                var gauge = GetJobGauge<ASTGauge>();
                var ladyofCrown = HasEffect(AST.Buffs.LadyOfCrownsDrawn);
                var lordofCrown = HasEffect(AST.Buffs.LordOfCrownsDrawn);
                var minorArcanaCD = GetCooldown(AST.MinorArcana);
                if (level >= AST.Levels.MinorArcana && gauge.DrawnCrownCard == CardType.NONE)
                    return AST.MinorArcana;
            }
            return actionID;
        }
    }

    internal class AstrologianBeneficFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianBeneficFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Benefic2)
            {
                if (level < AST.Levels.Benefic2)
                    return AST.Benefic;

            }
            return actionID;
        }
        internal class AstrologianAscendFeature : CustomCombo
        {
            protected override CustomComboPreset Preset => CustomComboPreset.AstrologianAscendFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == AST.Swiftcast)
                {
                    if (IsEnabled(CustomComboPreset.AstrologianAscendFeature))
                    {
                        if (HasEffect(AST.Buffs.Swiftcast))
                            return AST.Ascend;
                    }
                    return OriginalHook(AST.Swiftcast);
                }
                return actionID;
            }
        }
    }
}