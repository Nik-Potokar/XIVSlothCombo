using Lumina.Excel.GeneratedSheets;
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
            AdventofChocobastion = 39215,
            AeroinGreen = 39192,
            BlizzardinCyan = 34653,
            BlizzardIIinCyan = 34659,
            ClawMotif = 34666,
            CometinBlack = 34663,
            CreatureMotif = 34689,
            FireInRed = 34650,
            FireIIinRed = 34656,
            HammerStamp = 34678,
            HolyInWhite = 34662,
            LandscapeMotif = 34691,
            LivingMuse = 35347,
            MawMotif = 34667,
            MogoftheAges = 34676,
            PomMotif = 34664,
            PomMuse = 34670,
            RainbowDrip = 34688,
            ReleaseSubtractivePalette = 39214,
            RetributionoftheMadeen1 = 34677,
            RetributionoftheMadeen2 = 39783,
            ScenicMuse = 35349,
            Smudge1 = 34684,
            Smudge2 = 39210,
            StarPrism = 34681,
            SteelMuse = 35348,
            SubtractivePalette = 34683,
            ThunderIIinMagenta = 34661,
            ThunderinMagenta1 = 34655,
            ThunderinMagenta2 = 39196,
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
                SubtractiveSpectrum = 3690;
        }

        public static class Debuffs
        {

        }

        public static class Config
        {
            public static UserInt
                CombinedAetherhueChoices = new("CombinedAetherhueChoices");

            public static UserBool
                CombinedMotifsMog = new("CombinedMotifsMog"),
                CombinedMotifsWeapon = new("CombinedMotifsWeapon");
        }


        internal class PCT_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is FireInRed)
                {
                    TmpPCTGauge gauge = new TmpPCTGauge();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardinCyan)) : CanSpellWeave(OriginalHook(FireInRed));

                    if (IsMoving)
                    {
                        if (gauge.Paint > 0)
                        {
                            if (HasEffect(Buffs.MonochromeTones))
                                return OriginalHook(CometinBlack);

                            return OriginalHook(HolyInWhite);
                        }
                    }

                    if (HasEffect(Buffs.RainbowBright))
                        return OriginalHook(RainbowDrip);

                    if (HasEffect(Buffs.StarryMuse))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                            return OriginalHook(SubtractivePalette);

                        if (MogoftheAges.LevelChecked() && gauge.MooglePortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)))
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

                    if (HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (InCombat())
                    {
                        if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= GetActionCastTime(OriginalHook(LandscapeMotif)))
                            return OriginalHook(LandscapeMotif);

                        if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= GetActionCastTime(OriginalHook(CreatureMotif))))
                            return OriginalHook(CreatureMotif);

                        if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= GetActionCastTime(OriginalHook(WeaponMotif))))
                            return OriginalHook(WeaponMotif);

                        if (gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && gauge.MooglePortraitReady && IsOffCooldown(MogoftheAges) && IsOffCooldown(ScenicMuse) && canWeave)
                            return OriginalHook(ScenicMuse);

                        if (MogoftheAges.LevelChecked() && gauge.MooglePortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldown(LivingMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && (!gauge.MooglePortraitReady || GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);
                    }
                    if (gauge.Paint > 0 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (HasEffect(Buffs.SubtractivePalette))
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
                    TmpPCTGauge gauge = new TmpPCTGauge();
                    bool canWeave = HasEffect(Buffs.SubtractivePalette) ? CanSpellWeave(OriginalHook(BlizzardinCyan)) : CanSpellWeave(OriginalHook(FireInRed));

                    if (IsMoving)
                    {
                        if (gauge.Paint > 0)
                        {
                            if (HasEffect(Buffs.MonochromeTones))
                                return OriginalHook(CometinBlack);

                            return OriginalHook(HolyInWhite);
                        }
                    }

                    if (HasEffect(Buffs.RainbowBright))
                        return OriginalHook(RainbowDrip);

                    if (HasEffect(Buffs.StarryMuse))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) && !HasEffect(Buffs.SubtractivePalette) && canWeave)
                            return OriginalHook(SubtractivePalette);

                        if (MogoftheAges.LevelChecked() && gauge.MooglePortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)))
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

                    if (HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (InCombat())
                    {
                        if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= GetActionCastTime(OriginalHook(LandscapeMotif)))
                            return OriginalHook(LandscapeMotif);

                        if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= GetActionCastTime(OriginalHook(CreatureMotif))))
                            return OriginalHook(CreatureMotif);

                        if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= GetActionCastTime(OriginalHook(WeaponMotif))))
                            return OriginalHook(WeaponMotif);

                        if (gauge.LandscapeMotifDrawn && gauge.WeaponMotifDrawn && gauge.MooglePortraitReady && IsOffCooldown(MogoftheAges) && IsOffCooldown(ScenicMuse) && canWeave)
                            return OriginalHook(ScenicMuse);

                        if (MogoftheAges.LevelChecked() && gauge.MooglePortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)) && (GetCooldown(LivingMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(MogoftheAges);

                        if (!HasEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn && HasCharges(OriginalHook(SteelMuse)) && (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) || !ScenicMuse.LevelChecked()) && canWeave)
                            return OriginalHook(SteelMuse);

                        if (gauge.CreatureMotifDrawn && (!gauge.MooglePortraitReady || GetCooldown(LivingMuse).CooldownRemaining > GetCooldown(ScenicMuse).CooldownRemaining || GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse) || !ScenicMuse.LevelChecked()) && HasCharges(OriginalHook(LivingMuse)) && canWeave)
                            return OriginalHook(LivingMuse);
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
                var gauge = new TmpPCTGauge();

                if (actionID == CreatureMotif)
                {
                    if (Config.CombinedMotifsMog && gauge.MooglePortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)))
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
