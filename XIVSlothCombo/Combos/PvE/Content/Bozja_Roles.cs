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
}
