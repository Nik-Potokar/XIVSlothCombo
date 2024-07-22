using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.Logging;
using System;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class PCT
    {
        public const byte JobID = 42;

        public const uint
            BlizzardinCyan = 34653,
            BlizzardIIinCyan = 34659,
            ClawMotif = 34666,
            ClawedMuse = 34672,
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
                Aetherhues = 3675,
                AetherhuesII = 3676,
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
                PCT_ST_Lucid = new("PCT_ST_Lucid", 7500),
                PCT_AoE_Lucid = new("PCT_AoE_Lucid", 7500);

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

        internal class PCT_ST_Adv : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_Adv;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                var gauge = GetJobGauge<PCTGauge>();

                if (actionID is FireInRed)
                {
                    if (IsEnabled(CustomComboPreset.PCT_ST_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.PCT_ST_Subtractive_OP) && gauge.PalleteGauge == 100 && ActionReady(SubtractivePalette))
                    {
                        if (HasEffect(Buffs.MonochromeTones))
                        {
                            return OriginalHook(CometinBlack);
                        }
                        if (CanSpellWeave(actionID))
                        {
                            return SubtractivePalette;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.PCT_ST_Comet_OP) && gauge.Paint == 5 && (LevelChecked(HolyInWhite) || LevelChecked(CometinBlack)))
                    {
                        if (HasEffect(Buffs.MonochromeTones))
                        {
                            return OriginalHook(CometinBlack);
                        }
                        return OriginalHook(HolyInWhite);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_ST_Lucid) && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer.CurrentMp <= Config.PCT_ST_Lucid)
                        return All.LucidDreaming;

                    if (HasEffect(Buffs.SubtractivePalette))
                        return OriginalHook(BlizzardinCyan);
                }

                return actionID;
            }
        }

        internal class PCT_AoE_Adv : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_Adv;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                var gauge = GetJobGauge<PCTGauge>();

                if (actionID is FireIIinRed)
                {
                    if (IsEnabled(CustomComboPreset.PCT_AoE_Lucid) && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 1000)
                        return All.LucidDreaming;

                    if (IsEnabled(CustomComboPreset.PCT_AoE_Subtractive_OP) && gauge.PalleteGauge == 100 && CanSpellWeave(actionID) && ActionReady(SubtractivePalette))
                    {
                        if (HasEffect(Buffs.MonochromeTones))
                        {
                            return OriginalHook(CometinBlack);
                        }
                        return SubtractivePalette;
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_Comet_OP) && gauge.Paint == 5 && (LevelChecked(HolyInWhite) || LevelChecked(CometinBlack)))
                    {
                        if (HasEffect(Buffs.MonochromeTones))
                        {
                            return OriginalHook(CometinBlack);
                        }
                        return OriginalHook(HolyInWhite);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_Lucid) && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer.CurrentMp <= Config.PCT_AoE_Lucid)
                        return All.LucidDreaming;

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

        internal class PCT_SubPaint_OP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_SubPaint_OP;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is SubtractivePalette)
                {
                    if (HasEffect(Buffs.MonochromeTones))
                        return CometinBlack;
                }

                return actionID;
            }
        }
    }
}
