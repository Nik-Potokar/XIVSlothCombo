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
            Play = 17055;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Benefic2 = 26,
                MinorArcana = 50,
                SleeveDraw = 70;
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
                if (gauge.DrawnCard == CardType.NONE)
                    return AST.Draw;
            }

            return actionID;
        }
    }

     internal class AstrologianSleeveDrawFeature : CustomCombo
     {
         protected override CustomComboPreset Preset => CustomComboPreset.AstrologianSleeveDrawFeature;
    
         protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
         {
             if (actionID == AST.MinorArcana)
             {
                 var gauge = GetJobGauge<ASTGauge>().DrawnCard;
                 if (gauge == CardType.NONE && level >= AST.Levels.SleeveDraw)
                     return AST.SleeveDraw;
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
            if (IsEnabled(CustomComboPreset.AstrologianAutoLucidDreaming))
                {
                    var lucidDreamingCd = GetCooldown(AST.LucidDreaming);
                    if (!lucidDreamingCd.IsCooldown && LocalPlayer.CurrentMp < 8000)
                        return AST.LucidDreaming;
                }
            return actionID;
        }
    } 

}
