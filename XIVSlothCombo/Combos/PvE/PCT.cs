using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class PCT
    {
        public const byte JobID = 42;

        public const uint
            BlizzardinCyan = 34653,
            StoneinYellow = 34654,
            BlizzardIIinCyan = 34659,
            ClawMotif = 34666,
            ClawedMuse = 34672,
            CometinBlack = 34663,
            CreatureMotif = 34689,
            FireInRed = 34650,
            AeroInGreen = 34651,
            WaterInBlue = 34652,
            FireIIinRed = 34656,
            HammerMotif = 34668,
            WingedMuse = 34671,
            StrikingMuse = 34674,
            StarryMuse = 34675,
            HammerStamp = 34678,
            HammerBrush = 34679,
            PolishingHammer = 34680,
            HolyInWhite = 34662,
            StarrySkyMotif = 34669,
            LandscapeMotif = 34691,
            LivingMuse = 35347,
            MawMotif = 34667,
            MogoftheAges = 34676,
            PomMotif = 34664,
            PomMuse = 34670,
            RainbowDrip = 34688,
            RetributionoftheMadeen = 34677,
            ScenicMuse = 35349,
            Smudge = 34684,
            StarPrism = 34681,
            SteelMuse = 35348,
            SubtractivePalette = 34683,
            ThunderIIinMagenta = 34661,
            ThunderinMagenta = 34655,
            WaterinBlue = 34652,
            WeaponMotif = 34690,
            WingMotif = 34665;

        public static class Buffs
        {
            public const ushort
                SubtractivePalette = 3674,
                RainbowBright = 3679,
                HammerTime = 3680,
                MonochromeTones = 3691,
                StarryMuse = 3685,
                Hyperphantasia = 3688,
                Inspiration = 3689,
                SubtractiveSpectrum = 3690,
                Starstruck = 3681;
        }

        public static class Debuffs
        {

        }

        public static class Config
        {
            public static UserInt
                CombinedAetherhueChoices = new("CombinedAetherhueChoices"),
                PCT_ST_AdvancedMode_LucidOption = new("PCT_ST_AdvancedMode_LucidOption", 6500),
                PCT_AoE_AdvancedMode_LucidOption = new("PCT_AoE_AdvancedMode_LucidOption", 6500);

            public static UserBool
                CombinedMotifsMog = new("CombinedMotifsMog"),
                CombinedMotifsMadeen = new("CombinedMotifsMadeen"),
                CombinedMotifsWeapon = new("CombinedMotifsWeapon");
        }


        internal class PCT_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FireInRed)
                {
                    var gauge = GetJobGauge<PCTGauge>();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardinCyan)) : CanSpellWeave(OriginalHook(FireInRed));

                    if (HasEffect(Buffs.Starstruck))
                        return OriginalHook(StarPrism);

                    if (HasEffect(Buffs.RainbowBright))
                        return OriginalHook(RainbowDrip);

                    if (IsMoving)
                    {
                        if (gauge.Paint > 0)
                        {
                            if (HasEffect(Buffs.MonochromeTones))
                                return OriginalHook(CometinBlack);

                            return OriginalHook(HolyInWhite);
                        }
                    }


                    if (HasEffect(Buffs.StarryMuse))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                            return OriginalHook(SubtractivePalette);

                        if (MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)))
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && GetBuffRemainingTime(Buffs.StarryMuse) >= 15f)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);

                        if (HasEffect(Buffs.HammerTime))
                            return OriginalHook(HammerStamp);

                        if (HasEffect(Buffs.SubtractivePalette))
                            return OriginalHook(BlizzardinCyan);

                        return actionID;
                    }

                    if (gauge.PalleteGauge >= 50 && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                        return OriginalHook(SubtractivePalette);

                    if (HasEffect(Buffs.HammerTime) && !canWeave)
                        return OriginalHook(HammerStamp);

                    if (InCombat())
                    {
                        if (gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(MogoftheAges) && IsOffCooldown(ScenicMuse) && canWeave)
                            return OriginalHook(ScenicMuse);

                        if (MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldown(LivingMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) || GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);

                        if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= GetActionCastTime(OriginalHook(LandscapeMotif)))
                            return OriginalHook(LandscapeMotif);

                        if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= GetActionCastTime(OriginalHook(CreatureMotif))))
                            return OriginalHook(CreatureMotif);

                        if (WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= GetActionCastTime(OriginalHook(WeaponMotif))))
                            return OriginalHook(WeaponMotif);

                    }
                    if (gauge.Paint > 0 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardinCyan);

                }
                return actionID;
            }
        }

        internal class PCT_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_AdvancedMode;
            internal static PCTOpenerLogic PCTOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FireInRed)
                {
                    var gauge = GetJobGauge<PCTGauge>();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardinCyan)) : CanSpellWeave(OriginalHook(FireInRed)) || CanSpellWeave(OriginalHook(HammerStamp)) || CanSpellWeave(CometinBlack) || CanSpellWeave(HolyInWhite);


                    // Prepull logic
                    if (!InCombat() || (InCombat() && CurrentTarget == null))
                    {
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_PrePullMotifs))
                        {
                            if (LevelChecked(OriginalHook(CreatureMotif)) && !gauge.CreatureMotifDrawn)
                                return OriginalHook(CreatureMotif);
                            if (LevelChecked(OriginalHook(WeaponMotif)) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                                return OriginalHook(WeaponMotif);
                            if (LevelChecked(OriginalHook(LandscapeMotif)) && !gauge.LandscapeMotifDrawn && !HasEffect(Buffs.StarryMuse))
                                return OriginalHook(LandscapeMotif);
                        }
                    }

                    // Lvl 100 Opener
                    if (IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers) && LevelChecked(StarPrism))
                    {
                        if (PCTOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    // General Weaves
                    if (canWeave)
                    {
                        //Muses
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_ScenicMuse) && gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && IsOffCooldown(ScenicMuse))
                            return OriginalHook(ScenicMuse);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LivingMuse) && gauge.CreatureMotifDrawn &&
                            (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
                            GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining ||
                            GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) &&
                            HasCharges(OriginalHook(LivingMuse)) && canWeave)
                        {
                            if (GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                                return OriginalHook(LivingMuse);
                        }

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SteelMuse) && !HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()))
                            return OriginalHook(SteelMuse);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MogOfTheAges) && MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldownRemainingTime(StarryMuse) >= 40 || !ScenicMuse.LevelChecked()))
                            return OriginalHook(MogoftheAges);

                        // Swiftcast motifs
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SwitfcastOption) && IsMoving && IsOffCooldown(All.Swiftcast) && (!gauge.CreatureMotifDrawn || !gauge.WeaponMotifDrawn || !gauge.LandscapeMotifDrawn))
                            return All.Swiftcast;

                        // Palette
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SubtractivePalette) && LevelChecked(SubtractivePalette) && !HasEffect(Buffs.SubtractivePalette))
                        {
                            if (HasEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                                return SubtractivePalette;
                        }
                    }

                    if (HasEffect(All.Buffs.Swiftcast))
                    {
                        if (!gauge.CreatureMotifDrawn && LevelChecked(CreatureMotif) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(CreatureMotif);
                        if (!gauge.WeaponMotifDrawn && LevelChecked(WeaponMotif) && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(HammerMotif);
                        if (!gauge.LandscapeMotifDrawn && LevelChecked(LandscapeMotif) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(LandscapeMotif);
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_BlizzardInCyan) && HasEffect(Buffs.SubtractivePalette))
                            return OriginalHook(BlizzardinCyan);
                    }

                    if (IsMoving && InCombat())
                    {
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HammerStampCombo) && HasEffect(Buffs.HammerTime))
                            return OriginalHook(HammerStamp);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) && gauge.Paint >= 1 && !HasEffect(Buffs.MonochromeTones))
                            return OriginalHook(HolyInWhite);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) && gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                            return OriginalHook(CometinBlack);
                    }

                    //Prepare for Burst
                    if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                    {
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && LevelChecked(LandscapeMotif) && !gauge.LandscapeMotifDrawn)
                            return OriginalHook(LandscapeMotif);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && LevelChecked(CreatureMotif) && !gauge.CreatureMotifDrawn)
                            return OriginalHook(CreatureMotif);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && LevelChecked(WeaponMotif) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                            return OriginalHook(WeaponMotif);
                    }

                    // Burst 
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_Phase) && HasEffect(Buffs.StarryMuse))
                    {

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_CometInBlack) && HasEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                            return CometinBlack;

                        // Check for HammerTime 
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_HammerCombo) && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                            return OriginalHook(HammerStamp);

                        // Check for Starstruck
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_StarPrism))
                        {
                            if (HasEffect(Buffs.Starstruck) || HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) <= 3f)
                                return StarPrism;
                        }

                        // Check for RainbowBright
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_RainbowDrip))
                        {
                            if (HasEffect(Buffs.RainbowBright) || HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) <= 3f)
                                return RainbowDrip;
                        }

                    }


                    if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                        return RainbowDrip;

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CometinBlack) && HasEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(Buffs.StarryMuse) > 30f)
                        return OriginalHook(CometinBlack);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_HammerStampCombo) && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);


                    if (!HasEffect(Buffs.StarryMuse))
                    {
                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                            return OriginalHook(LandscapeMotif);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                            return OriginalHook(CreatureMotif);

                        if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime) && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8 && !HasEffect(Buffs.HammerTime)))
                            return OriginalHook(WeaponMotif);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LucidDreaming) && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer.CurrentMp <= Config.PCT_ST_AdvancedMode_LucidOption)
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_BlizzardInCyan) && HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardinCyan);
                }
                return actionID;
            }
        }

        internal class PCT_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FireIIinRed)
                {
                    var gauge = GetJobGauge<PCTGauge>();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardinCyan)) : CanSpellWeave(OriginalHook(FireInRed));

                    if (HasEffect(Buffs.Starstruck))
                        return OriginalHook(StarPrism);

                    if (HasEffect(Buffs.RainbowBright))
                        return OriginalHook(RainbowDrip);

                    if (IsMoving)
                    {
                        if (gauge.Paint > 0)
                        {
                            if (HasEffect(Buffs.MonochromeTones))
                                return OriginalHook(CometinBlack);

                            return OriginalHook(HolyInWhite);
                        }
                    }

                    if (HasEffect(Buffs.StarryMuse))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                            return OriginalHook(SubtractivePalette);

                        if (MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)))
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && GetBuffRemainingTime(Buffs.StarryMuse) >= 15f)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);

                        if (HasEffect(Buffs.HammerTime))
                            return OriginalHook(HammerStamp);

                        if (HasEffect(Buffs.SubtractivePalette))
                            return OriginalHook(BlizzardIIinCyan);

                        return actionID;
                    }

                    if (gauge.PalleteGauge >= 50 && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                        return OriginalHook(SubtractivePalette);

                    if (HasEffect(Buffs.HammerTime) && !canWeave)
                        return OriginalHook(HammerStamp);

                    if (InCombat())
                    {
                        if (gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(MogoftheAges) && IsOffCooldown(ScenicMuse) && canWeave)
                            return OriginalHook(ScenicMuse);

                        if (MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldown(MogoftheAges).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) || GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);

                        if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= GetActionCastTime(OriginalHook(LandscapeMotif)))
                            return OriginalHook(LandscapeMotif);

                        if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= GetActionCastTime(OriginalHook(CreatureMotif))))
                            return OriginalHook(CreatureMotif);

                        if (WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= GetActionCastTime(OriginalHook(WeaponMotif))))
                            return OriginalHook(WeaponMotif);
                    }

                    if (gauge.Paint > 0 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (gauge.Paint > 0)
                        return OriginalHook(HolyInWhite);

                    if (HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardIIinCyan);

                }
                return actionID;
            }
        }

        internal class PCT_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FireIIinRed)
                {
                    var gauge = GetJobGauge<PCTGauge>();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardIIinCyan)) : CanSpellWeave(OriginalHook(FireIIinRed)) || CanSpellWeave(OriginalHook(HammerStamp)) || CanSpellWeave(CometinBlack) || CanSpellWeave(HolyInWhite);


                    // Prepull logic
                    if (!InCombat() || (InCombat() && CurrentTarget == null))
                    {
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_PrePullMotifs))
                        {
                            if (LevelChecked(OriginalHook(CreatureMotif)) && !gauge.CreatureMotifDrawn)
                                return OriginalHook(CreatureMotif);
                            if (LevelChecked(OriginalHook(WeaponMotif)) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                                return OriginalHook(WeaponMotif);
                            if (LevelChecked(OriginalHook(LandscapeMotif)) && !gauge.LandscapeMotifDrawn && !HasEffect(Buffs.StarryMuse))
                                return OriginalHook(LandscapeMotif);
                        }
                    }

                    // General Weaves
                    if (canWeave)
                    {
                        //Muses
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_ScenicMuse) && gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && IsOffCooldown(ScenicMuse))
                            return OriginalHook(ScenicMuse);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LivingMuse) && gauge.CreatureMotifDrawn &&
                            (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
                            GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining ||
                            GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) &&
                            HasCharges(OriginalHook(LivingMuse)) && canWeave)
                        {
                            if (GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                                return OriginalHook(LivingMuse);
                        }

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SteelMuse) && !HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()))
                            return OriginalHook(SteelMuse);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MogOfTheAges) && MogoftheAges.LevelChecked() && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldownRemainingTime(StarryMuse) >= 40 || !ScenicMuse.LevelChecked()))
                            return OriginalHook(MogoftheAges);

                        // Swiftcast motifs
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SwitfcastOption) && IsMoving && IsOffCooldown(All.Swiftcast) && (!gauge.CreatureMotifDrawn || !gauge.WeaponMotifDrawn || !gauge.LandscapeMotifDrawn))
                            return All.Swiftcast;

                        // Palette
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SubtractivePalette) && LevelChecked(SubtractivePalette) && !HasEffect(Buffs.SubtractivePalette))
                        {
                            if (HasEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                                return SubtractivePalette;
                        }
                    }

                    if (HasEffect(All.Buffs.Swiftcast))
                    {
                        if (!gauge.CreatureMotifDrawn && LevelChecked(CreatureMotif) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(CreatureMotif);
                        if (!gauge.WeaponMotifDrawn && LevelChecked(WeaponMotif) && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(HammerMotif);
                        if (!gauge.LandscapeMotifDrawn && LevelChecked(LandscapeMotif) && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(LandscapeMotif);
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_BlizzardInCyan) && HasEffect(Buffs.SubtractivePalette))
                            return OriginalHook(BlizzardIIinCyan);
                    }

                    if (IsMoving && InCombat())
                    {
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo) && HasEffect(Buffs.HammerTime))
                            return OriginalHook(HammerStamp);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HolyInWhite) && gauge.Paint >= 1 && !HasEffect(Buffs.MonochromeTones))
                            return OriginalHook(HolyInWhite);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_CometinBlack) && gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                            return OriginalHook(CometinBlack);
                    }

                    //Prepare for Burst
                    if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                    {
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && LevelChecked(LandscapeMotif) && !gauge.LandscapeMotifDrawn)
                            return OriginalHook(LandscapeMotif);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && LevelChecked(CreatureMotif) && !gauge.CreatureMotifDrawn)
                            return OriginalHook(CreatureMotif);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && LevelChecked(WeaponMotif) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                            return OriginalHook(WeaponMotif);
                    }

                    // Burst 
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_Phase) && HasEffect(Buffs.StarryMuse))
                    {

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_CometInBlack) && HasEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                            return CometinBlack;

                        // Check for HammerTime 
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_HammerCombo) && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                            return OriginalHook(HammerStamp);

                        // Check for Starstruck
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_StarPrism) && HasEffect(Buffs.Starstruck) || HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) < 3)
                            return StarPrism;

                        // Check for RainbowBright
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_RainbowDrip))
                        {
                            if (HasEffect(Buffs.RainbowBright) || HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) < 3)
                                return RainbowDrip;
                        }

                    }


                    if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                        return RainbowDrip;

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CometinBlack) && HasEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 30)
                        return OriginalHook(CometinBlack);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_HammerStampCombo) && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);


                    if (!HasEffect(Buffs.StarryMuse))
                    {
                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                            return OriginalHook(LandscapeMotif);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                            return OriginalHook(CreatureMotif);

                        if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime) && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8 && !HasEffect(Buffs.HammerTime)))
                            return OriginalHook(WeaponMotif);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LucidDreaming) && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer.CurrentMp <= Config.PCT_ST_AdvancedMode_LucidOption)
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_BlizzardInCyan) && HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardIIinCyan);

                }
                return actionID;
            }
        }

        internal class CombinedAetherhues : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedAetherhues;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                int choice = Config.CombinedAetherhueChoices;

                if (actionID == FireInRed && choice is 0 or 1)
                {
                    if (HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardinCyan);
                }

                if (actionID == FireIIinRed && choice is 0 or 2)
                {
                    if (HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardIIinCyan);
                }

                return actionID;
            }
        }

        internal class CombinedMotifs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedMotifs;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                var gauge = GetJobGauge<PCTGauge>();

                if (actionID == CreatureMotif)
                {
                    if ((Config.CombinedMotifsMog && gauge.MooglePortraitReady) || (Config.CombinedMotifsMadeen && gauge.MadeenPortraitReady) && IsOffCooldown(OriginalHook(MogoftheAges)))
                        return OriginalHook(MogoftheAges);

                    if (gauge.CreatureMotifDrawn)
                        return OriginalHook(LivingMuse);
                }

                if (actionID == WeaponMotif)
                {
                    if (Config.CombinedMotifsWeapon && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (gauge.WeaponMotifDrawn)
                        return OriginalHook(SteelMuse);
                }

                if (actionID == LandscapeMotif)
                {
                    if (gauge.LandscapeMotifDrawn)
                        return OriginalHook(ScenicMuse);
                }

                return actionID;
            }
        }

        internal class CombinedPaint : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedPaint;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == HolyInWhite)
                {
                    if (HasEffect(Buffs.MonochromeTones))
                        return CometinBlack;
                }

                return actionID;
            }
        }
    }
}