using XIVSlothCombo.CustomComboNS;
using static XIVSlothCombo.Combos.PvE.RPR;
using static XIVSlothCombo.Combos.PvE.MNK;
using static XIVSlothCombo.Combos.PvE.SAM;
using static XIVSlothCombo.Combos.PvE.DRG;
using static XIVSlothCombo.Combos.PvE.NIN;
using static XIVSlothCombo.Combos.PvE.DNC;
using static XIVSlothCombo.Combos.PvE.MCH;
using static XIVSlothCombo.Combos.PvE.BRD;
using static XIVSlothCombo.Combos.PvE.DRK;
using static XIVSlothCombo.Combos.PvE.PLD;
using static XIVSlothCombo.Combos.PvE.GNB;
using static XIVSlothCombo.Combos.PvE.WAR;
using static XIVSlothCombo.Combos.PvE.WHM;
using static XIVSlothCombo.Combos.PvE.BLM;
using static XIVSlothCombo.Combos.PvE.RDM;
using static XIVSlothCombo.Combos.PvE.AST;
using static XIVSlothCombo.Combos.PvE.SCH;
using static XIVSlothCombo.Combos.PvE.SGE;
using static XIVSlothCombo.Combos.PvE.SMN;
using Dalamud.Game.ClientState.JobGauge.Enums;
using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;

namespace XIVSlothCombo.Combos.PvE.Content
{
    internal class Bozja_Tank_Mit : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_BozjaAetherShield;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is 7531)
            //Rampart 
            {
                if (IsEnabled(CustomComboPreset.ALL_BozjaAetherShield))
                {

                    if (IsEnabled(Bozja.LostAethershield) && IsOffCooldown(Bozja.LostAethershield))
                        return Bozja.LostAethershield;
                }
            }

            return actionID;
        }

    }

    internal class Bozja_Phys_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_BozjaDPS;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool canuseaction = false;
            bool canusebozjastuffs = false;
            bool isphysranged = false;
            bool istank = false;

            if (actionID is Slice or Bootshine or TrueThrust or Gekko or SpinningEdge)
            {
                //Melees
                canusebozjastuffs = true;
            }

            if (actionID is Souleater or KeenEdge or FastBlade or StormsPath)
            {
                //Tanks
                canusebozjastuffs = true;
                istank = true;
            }

            if (actionID is Cascade or HeavyShot or BurstShot || actionID == CleanShot || actionID == HeatedCleanShot || actionID == SplitShot || actionID == HeatedSplitShot)
            {
                //Phys ranged
                canusebozjastuffs = true;
                isphysranged = true;
            }

            if (canusebozjastuffs)
            {
                // Bozja Stuffs - Riley (Luna)

                if (!HasEffect(DRG.Buffs.SharperFangAndClaw) && !HasEffect(DRG.Buffs.EnhancedWheelingThrust)
                        && !HasEffect(DRG.Buffs.DraconianFire) && !HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(SAM.Buffs.MeikyoShisui)
                        && !HasEffect(NIN.Buffs.Mudra))
                {
                    canuseaction = true;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaOffClassTankSct) &&
                    IsEnabled(Bozja.LostIncense) && IsOffCooldown(Bozja.LostIncense) &&
                    HasBattleTarget())
                {
                    //Congrats your a tank now, good luck!
                    return Bozja.LostIncense;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaCureSelfheal))
                {
                    if (IsEnabled(Bozja.LostCure4) &&
                    PlayerHealthPercentageHp() <= 50 &&
                    CanWeave(actionID))
                        return Bozja.LostCure4;

                    if (IsEnabled(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= 50)
                        return Bozja.LostCure3;

                    if (IsEnabled(Bozja.LostCure2) &&
                    PlayerHealthPercentageHp() <= 50 &&
                    CanWeave(actionID))
                        return Bozja.LostCure2;

                    if (IsEnabled(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= 50)
                        return Bozja.LostCure;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaCure2Phys) &&
                    IsEnabled(Bozja.LostCure2) && IsOffCooldown(Bozja.LostCure2) && HasEffect(Bozja.Buffs.PureFiendhunter) &&
                    isphysranged && !TargetHasEffect(Bozja.Buffs.MPRefresh) && !TargetHasEffect(Bozja.Buffs.MPRefresh2))
                {
                    if (TargetHasEffect(Bozja.Buffs.ProfaneEssence) || TargetHasEffect(Bozja.Buffs.IrregularEssence) || TargetHasEffect(Bozja.Buffs.PureElder))
                    {
                        return Bozja.LostCure2;
                    }
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaRendArmor) &&
                    IsEnabled(Bozja.LostRendArmor) && IsOffCooldown(Bozja.LostRendArmor) &&
                    HasBattleTarget() &&  canuseaction && !isphysranged && !TargetHasEffect(Bozja.Debuffs.LostRendArmor))
                {
                    return Bozja.LostRendArmor;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaPhysDerv) && isphysranged &&
                        IsEnabled(Bozja.LostDervish) && IsOffCooldown(Bozja.LostDervish) && !HasEffect(Bozja.Buffs.LostDervish))
                {
                    return Bozja.LostDervish;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaDPS))
                {

                    if (IsEnabled(Bozja.LostExcellence) && IsOffCooldown(Bozja.LostExcellence))
                        return Bozja.LostExcellence;

                    if (IsEnabled(Bozja.FontOfPower) && IsOffCooldown(Bozja.FontOfPower))
                        return Bozja.FontOfPower;

                    if (IsEnabled(CustomComboPreset.ALL_BozjaAssassinationDPS) &&
                    IsEnabled(Bozja.LostAssassination) && IsOffCooldown(Bozja.LostAssassination) &&
                    HasBattleTarget() && canuseaction)
                    {
                        if (!HasEffect(Bozja.Buffs.FontOfPower) && HasEffect(Bozja.Buffs.BeastEssence))
                            return Bozja.LostAssassination;

                        if (CanWeave(actionID))
                            return Bozja.LostAssassination;
                    }

                    // Checks to see if you have Lost Assassination or Font of Power, and lines up Banners to Font
                    if (IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                    {
                        if (HasEffect(Bozja.Buffs.FontOfPower))
                        {
                            if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                                return Bozja.BannerOfHonoredSacrifice;

                            if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                                return Bozja.BannerOfNobleEnds;
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                    {
                        if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;

                        if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;
                    }
                }
            }
            return actionID;
        }
    }

    internal class Bozja_Phys_AOE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_BozjaPhysAOE;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool canuseaction = false;
            bool canusebozjastuffs = false;

            if (actionID is SpinningScythe or DoomSpike or Oka or DeathBlossom || actionID == ArmOfTheDestroyer || actionID == ShadowOfTheDestroyer)
            {
                //Melees
                canusebozjastuffs = true;
            }

            if (actionID is StalwartSoul or DemonSlice or TotalEclipse or Overpower)
            {
                //Tanks
                canusebozjastuffs = true;
            }

            if (actionID is Windmill or Ladonsbite or QuickNock || actionID == SpreadShot || actionID == Scattergun)
            {
                //Phys ranged
                canusebozjastuffs = true;
            }

            if (!HasEffect(DRG.Buffs.SharperFangAndClaw) && !HasEffect(DRG.Buffs.EnhancedWheelingThrust)
                        && !HasEffect(DRG.Buffs.DraconianFire) && !HasEffect(RPR.Buffs.SoulReaver) && !HasEffect(SAM.Buffs.MeikyoShisui)
                        && !HasEffect(NIN.Buffs.Mudra))
            {
                canuseaction = true;
            }

            if (canusebozjastuffs && IsEnabled(Bozja.LostRampage) && canuseaction)
                return Bozja.LostRampage;

            return actionID;
        }

    }

    internal class Bozja_Magic_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_BozjaMagicDPS;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool canuseaction = false;
            bool canusebozjastuffs = false;
            bool iscastersupport = false;
            bool iswhitemage = false;
            bool isblackmage = false;
            bool thinAirReady = false;

            if (actionID is Glare3 or Glare1)
            {
                //White Mage check
                canusebozjastuffs = true;
                iswhitemage = true;
                thinAirReady = !HasEffect(WHM.Buffs.ThinAir) && LevelChecked(ThinAir) && HasCharges(ThinAir);
            }

            if (actionID is Dosis2 or Malefic4 or Broil3 or SCH.Ruin2)
            {
                //All other healers check
                canusebozjastuffs = true;
            }

            if (actionID is Jolt2 or SMN.Ruin or SMN.Ruin2)
            {
                //Casters
                canusebozjastuffs = true;
                iscastersupport = true;
            }

            if (actionID is Scathe)
            {
                //Black Mage Check
                isblackmage = true;
                canusebozjastuffs = true;
                iscastersupport = true;
            }

            if (canusebozjastuffs)
            {
                if (IsEnabled(CustomComboPreset.ALL_BozjaOffClassTankSct) &&
                    IsEnabled(Bozja.LostIncense) && IsOffCooldown(Bozja.LostIncense) &&
                    HasBattleTarget())
                {
                    //Congrats your a tank now, good luck!
                    return Bozja.LostIncense;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaMagicDPS) && IsEnabled(CustomComboPreset.ALL_BozjaHealerSS) &&
                    IsEnabled(Bozja.LostSeraphStrike) && IsOffCooldown(Bozja.LostSeraphStrike) &&
                    HasBattleTarget())
                {
                    //Thin air added may be jank but yea, it may work? - Riley
                    //return IsEnabled(CustomComboPreset.WHM_ThinAirBozja) && thinAirReady

                    return thinAirReady
                        ? ThinAir
                        : Bozja.LostSeraphStrike;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaCure4Caster) && iscastersupport)
                {
                    if (IsEnabled(Bozja.LostCure4) &&
                    !HasEffect(Bozja.Buffs.LostBravery2) &&
                    HasEffect(Bozja.Buffs.PureElder))
                        return Bozja.LostCure4;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaCureSelfheal) && iscastersupport)
                {
                    if (IsEnabled(Bozja.LostCure4) &&
                    PlayerHealthPercentageHp() <= 50 &&
                    CanWeave(actionID))
                        return Bozja.LostCure4;

                    if (IsEnabled(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= 50)
                        return Bozja.LostCure3;

                    if (IsEnabled(Bozja.LostCure2) &&
                    PlayerHealthPercentageHp() <= 50 &&
                    CanWeave(actionID))
                        return Bozja.LostCure2;

                    if (IsEnabled(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= 50)
                        return Bozja.LostCure;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaMagicDPS))
                {
                    if (IsEnabled(Bozja.LostExcellence) && IsOffCooldown(Bozja.LostExcellence))
                        return Bozja.LostExcellence;

                    if (IsEnabled(Bozja.FontOfMagic) && IsOffCooldown(Bozja.FontOfMagic))
                        return Bozja.FontOfMagic;

                    // Checks to see if you have Font of Magic, and lines up Banners to Font
                    if (IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                    {
                        if (HasEffect(Bozja.Buffs.FontOfMagic))
                        {
                            if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                                return Bozja.BannerOfHonoredSacrifice;

                            if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                                return Bozja.BannerOfNobleEnds;

                            if (IsEnabled(Bozja.LostChainspell) && IsOffCooldown(Bozja.LostChainspell))
                                return Bozja.LostChainspell;

                            //Other devs could we check for chainspell before using swiftcast?
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.ALL_BozjaHoldBannerPhys))
                    {
                        if (IsEnabled(Bozja.BannerOfHonoredSacrifice) && IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;

                        if (IsEnabled(Bozja.BannerOfNobleEnds) && IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;

                        if (IsEnabled(Bozja.LostChainspell) && IsOffCooldown(Bozja.LostChainspell))
                            return Bozja.LostChainspell;
                    }
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaFlareStar))
                {
                    if (IsEnabled(Bozja.LostFlareStar) && !TargetHasEffect(Bozja.Debuffs.LostFlareStar) &&
                        (LocalPlayer.CurrentMp >= 9000))
                        return Bozja.LostFlareStar;
                }
            }

            return actionID;
        }
    }

    internal class Bozja_Magic_AOE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_BozjaMagicAOE;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool canuseaction = false;
            bool canusebozjastuffs = false;
            bool iscastersupport = false;
            bool iswhitemage = false;
            bool isblackmage = false;

            if (actionID is Holy3 or Holy)
            {
                //White Mage check
                canusebozjastuffs = true;
                iswhitemage = true;
            }

            if (actionID is Dyskrasia or Gravity2 or ArtOfWar)
            {
                //All other healers check
                canusebozjastuffs = true;
            }

            if (actionID is Scatter or Impact or Outburst or Tridisaster)
            {
                //Casters
                canusebozjastuffs = true;
                iscastersupport = true;
            }

            if (actionID is Scathe)
            {
                //Black Mage Check
                isblackmage = true;
                canusebozjastuffs = true;
                iscastersupport = true;
            }

            if (canusebozjastuffs)
            {
                if (IsEnabled(CustomComboPreset.ALL_BozjaFlareStar))
                {
                    if (IsEnabled(Bozja.LostFlareStar) && !TargetHasEffect(Bozja.Debuffs.LostFlareStar) &&
                        (LocalPlayer.CurrentMp >= 9000))
                        return Bozja.LostFlareStar;
                }

                if (IsEnabled(CustomComboPreset.ALL_BozjaMagicbanAOE) &&
                IsEnabled(Bozja.LostBanish3) && HasBattleTarget())
                    return Bozja.LostBanish3;

                if (IsEnabled(CustomComboPreset.ALL_BozjaMagicAOE) &&
                IsEnabled(Bozja.LostBurst))
                    return Bozja.LostBurst;

            }

            return actionID;
        }

    }
}
