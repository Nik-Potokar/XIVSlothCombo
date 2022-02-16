using Dalamud.Game.ClientState.JobGauge.Enums;
using XIVSlothComboPlugin.Classes;

namespace XIVSlothComboPlugin.Combos
{
    internal class AstrologianCardsOnDrawFeaturelikewhat : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Play)
            {
                if (
                    !AST.JobGauge.ContainsSeal(SealType.NONE) &&
                    IsEnabled(CustomComboPreset.AstrologianAstrodyneOnPlayFeature) &&
                    (AST.JobGauge.DrawnCard != CardType.NONE || GetCooldown(AST.Actions.Draw).CooldownRemaining > 30)
                ) return AST.Actions.Astrodyne;

                if (HasEffect(AST.Buffs.Balance) || HasEffect(AST.Buffs.Bole) || HasEffect(AST.Buffs.Arrow) || HasEffect(AST.Buffs.Spear) || HasEffect(AST.Buffs.Ewer) || HasEffect(AST.Buffs.Spire))
                    return OriginalHook(AST.Actions.Play);

                return AST.Actions.Draw;
            }

            return actionID;
        }
    }

    internal class AstrologianCrownPlayFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCrownPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.CrownPlay)
            {
                if (AST.IsUnlocked(AST.Actions.MinorArcana, level) && AST.JobGauge.DrawnCrownCard == CardType.NONE)
                    return AST.Actions.MinorArcana;
            }

            return actionID;
        }
    }

    internal class AstrologianBeneficFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianBeneficFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Benefic2 && !AST.IsUnlocked(AST.Actions.Benefic2, level))
            {
                return AST.Actions.Benefic;
            }

            return actionID;
        }
    }

    internal class AstrologianAscendFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAscendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Swiftcast)
            {
                if (
                    IsEnabled(CustomComboPreset.AstrologianAscendFeature) &&
                    HasEffect(AST.Buffs.Swiftcast)
                )
                {
                    return AST.Actions.Ascend;
                }

                return OriginalHook(AST.Actions.Swiftcast);
            }

            return actionID;
        }
    }
    internal class AstrologianAlternateAscendFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAlternateAscendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Ascend && IsOffCooldown(AST.Actions.Swiftcast))
            {
                return AST.Actions.Swiftcast;
            }

            return actionID;
        }
    }

    internal class AstrologianDpsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.FallMalefic || actionID == AST.Actions.Malefic4 || actionID == AST.Actions.Malefic3 || actionID == AST.Actions.Malefic2 || actionID == AST.Actions.Malefic)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var lucidDreaming = GetCooldown(AST.Actions.LucidDreaming);
                var fallmalefic = GetCooldown(AST.Actions.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.Actions.MinorArcana);
                var drawCD = GetCooldown(AST.Actions.Draw);
                var actionIDCD = GetCooldown(actionID);
                var canWeave = CanWeave(actionID, 0.6);

                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && AST.IsUnlocked(AST.Actions.Lightspeed, level))
                {
                    if (
                        IsOffCooldown(AST.Actions.Lightspeed) &&
                        lastComboMove == OriginalHook(actionID) &&
                        canWeave
                    )
                        return AST.Actions.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && AST.IsUnlocked(AST.Actions.Astrodyne, level))
                {
                    if (!AST.JobGauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6)
                        return AST.Actions.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && AST.IsUnlocked(AST.Actions.Draw, level))
                {
                    if (AST.JobGauge.DrawnCard.Equals(CardType.NONE) && incombat && canWeave && drawCD.CooldownRemaining < 30)
                        return AST.Actions.Draw;
                     
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && AST.IsUnlocked(AST.Actions.MinorArcana, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && canWeave)
                        return AST.Actions.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && AST.IsUnlocked(AST.Actions.LucidDreaming, level))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2)
                        return AST.Actions.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && AST.IsUnlocked(AST.Actions.LordOfCrowns, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.LORD && incombat && canWeave)
                        return AST.Actions.LordOfCrowns;
                }
                if (
                    IsEnabled(CustomComboPreset.AstrologianDpsFeature) &&
                    !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) &&
                    AST.IsUnlocked(AST.Actions.Combust3, level) &&
                    incombat
                )
                {
                    if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust3;
                }

                if (
                    IsEnabled(CustomComboPreset.AstrologianDpsFeature) &&
                    !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) &&
                    !AST.IsUnlocked(AST.Actions.Combust3, level) &&
                    AST.IsUnlocked(AST.Actions.Combust2, level) &&
                    incombat
                )
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust2;
                }

                if (
                    IsEnabled(CustomComboPreset.AstrologianDpsFeature) &&
                    !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) &&
                    !AST.IsUnlocked(AST.Actions.Combust2, level) &&
                    AST.IsUnlocked(AST.Actions.Combust, level) &&
                    incombat
                )
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust;
                }
            }

            return actionID;
        }
    }
   
    internal class AstrologianHeliosFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianHeliosFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.AspectedHelios)
            {
                var heliosBuff = FindEffect(AST.Buffs.AspectedHelios);
                if (HasEffect(AST.Buffs.AspectedHelios) && heliosBuff.RemainingTime > 2)
                    return AST.Actions.Helios;
            }

            return actionID;
        }
    }
    internal class AstrologianDpsAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Gravity || actionID == AST.Actions.Gravity2)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var canWeave = CanWeave(actionID, 0.6);

                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && AST.IsUnlocked(AST.Actions.Lightspeed, level))
                {
                    if (IsOffCooldown(AST.Actions.Lightspeed) && lastComboMove == OriginalHook(actionID) && canWeave)
                        return AST.Actions.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && AST.IsUnlocked(AST.Actions.Astrodyne, level))
                {
                    if (!AST.JobGauge.ContainsSeal(SealType.NONE) && incombat && canWeave)
                        return AST.Actions.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && AST.IsUnlocked(AST.Actions.Draw, level))
                {
                    if (AST.JobGauge.DrawnCard.Equals(CardType.NONE) && incombat && canWeave && GetRemainingCharges(AST.Actions.Draw) > 0)
                        return AST.Actions.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && AST.IsUnlocked(AST.Actions.LordOfCrowns, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.LORD && incombat && canWeave)
                        return AST.Actions.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && AST.IsUnlocked(AST.Actions.MinorArcana, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.NONE && incombat && IsOffCooldown(AST.Actions.MinorArcana) && canWeave)
                        return AST.Actions.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && AST.IsUnlocked(AST.Actions.LucidDreaming, level))
                {
                    if (IsOffCooldown(AST.Actions.LucidDreaming) && LocalPlayer.CurrentMp <= 8000 && CanWeave(actionID, 0.2))
                        return AST.Actions.LucidDreaming;
                }
            }
            return actionID;
        }
    }
    internal class AstrologianAstrodyneOnPlayFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAstrodyneOnPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Play && !IsEnabled(CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat))
            {
                if (!AST.JobGauge.ContainsSeal(SealType.NONE))
                    return AST.Actions.Astrodyne;
            }
            return actionID;

        }
    }
    internal class AstrologianAlternateDpsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAlternateDpsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.Combust || actionID == AST.Actions.Combust2 || actionID == AST.Actions.Combust3)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var lucidDreaming = GetCooldown(AST.Actions.LucidDreaming);
                var fallmalefic = GetCooldown(AST.Actions.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.Actions.MinorArcana);
                var drawCD = GetCooldown(AST.Actions.Draw);
                var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && AST.IsUnlocked(AST.Actions.Lightspeed, level))
                {
                    var lightspeed = GetCooldown(AST.Actions.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Actions.Lightspeed;
                }
                if (!HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                {
                    return OriginalHook(AST.Actions.Malefic);
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && AST.IsUnlocked(AST.Actions.Astrodyne, level))
                {
                    if (!AST.JobGauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6)
                        return AST.Actions.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && AST.IsUnlocked(AST.Actions.Draw, level))
                {
                    if (AST.JobGauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30)
                        return AST.Actions.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && AST.IsUnlocked(AST.Actions.MinorArcana, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Actions.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && AST.IsUnlocked(AST.Actions.LucidDreaming, level))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2)
                        return AST.Actions.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && AST.IsUnlocked(AST.Actions.LordOfCrowns, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Actions.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && AST.IsUnlocked(AST.Actions.Combust3, level) && incombat)
                {
                    if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust3;
                }

                if (
                    IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) &&
                    !AST.IsUnlocked(AST.Actions.Combust3, level) &&
                    AST.IsUnlocked(AST.Actions.Combust2, level) &&
                    incombat
                )
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust2;
                }

                if (
                    IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) &&
                    !AST.IsUnlocked(AST.Actions.Combust2, level) &&
                    AST.IsUnlocked(AST.Actions.Combust, level) &&
                    incombat
                )
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust;
                }
                return OriginalHook(AST.Actions.Malefic);
            }
            return actionID;
        }
    }
    internal class CustomValuesTest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CustomValuesTest;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Actions.FallMalefic || actionID == AST.Actions.Malefic4 || actionID == AST.Actions.Malefic3 || actionID == AST.Actions.Malefic2 || actionID == AST.Actions.Malefic)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var lucidDreaming = GetCooldown(AST.Actions.LucidDreaming);
                var fallmalefic = GetCooldown(AST.Actions.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.Actions.MinorArcana);
                var drawCD = GetCooldown(AST.Actions.Draw);
                var actionIDCD = GetCooldown(actionID);
                var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                var CurrentHpValue = Service.Configuration.EnemyCurrentHp;


                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && AST.IsUnlocked(AST.Actions.Lightspeed, level))
                {
                    var lightspeed = GetCooldown(AST.Actions.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Actions.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && AST.IsUnlocked(AST.Actions.Astrodyne, level))
                {
                    if (!AST.JobGauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6)
                        return AST.Actions.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && AST.IsUnlocked(AST.Actions.Draw, level))
                {
                    if (AST.JobGauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30)
                        return AST.Actions.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && AST.IsUnlocked(AST.Actions.MinorArcana, level))
                {
                    if (AST.JobGauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Actions.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && AST.IsUnlocked(AST.Actions.LucidDreaming, level))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2)
                        return AST.Actions.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && AST.IsUnlocked(AST.Actions.LordOfCrowns, level))
                {
                    var buff = HasEffect(AST.Buffs.Divination);
                    var buffcd = GetCooldown(AST.Actions.Divination);
                    if (AST.JobGauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && buff || AST.JobGauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70 && buffcd.IsCooldown)
                        return AST.Actions.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.CustomValuesTest) && AST.IsUnlocked(AST.Actions.Combust3, level) && incombat)
                {
                    if ((combust3Debuff is null) && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue || (combust3Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue )
                        return AST.Actions.Combust3;
                }

                if (IsEnabled(CustomComboPreset.CustomValuesTest) && !AST.IsUnlocked(AST.Actions.Combust3, level) && AST.IsUnlocked(AST.Actions.Combust2, level) && incombat)
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust2;
                }

                if (IsEnabled(CustomComboPreset.CustomValuesTest) && !AST.IsUnlocked(AST.Actions.Combust2, level) && AST.IsUnlocked(AST.Actions.Combust, level) && incombat)
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Actions.Combust;
                }
            }
            return actionID;
        }

    }
}
