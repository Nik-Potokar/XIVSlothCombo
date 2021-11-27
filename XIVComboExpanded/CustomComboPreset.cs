using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Combo presets.
    /// </summary>
    public enum CustomComboPreset
    {
        // Last enum used: 113 is free

        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("Draw on Play", "Play turns into Draw when no card is drawn, as well as the usual Play behavior.", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 27,

        [CustomComboInfo("Minor Arcana to Sleeve Draw", "Changes Minor Arcana to Sleeve Draw when a card is not drawn.", AST.JobID, AST.MinorArcana)]
        AstrologianSleeveDrawFeature = 75,

        [CustomComboInfo("Benefic 2 to Benefic Level Sync", "Changes Benefic 2 to Benefic when below level 26 in synced content.", AST.JobID, AST.Benefic2)]
        AstrologianBeneficFeature = 73,

        [CustomComboInfo("Swiftcast Feature", "Changes Swiftcast To Ascend", AST.JobID, AST.Swiftcast, AST.Ascend)]
        AstrologianAscendFeature = 110,



        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("Enochian Stance Switcher", "Change Enochian to Fire 4 or Blizzard 4 depending on stance.", BLM.JobID, BLM.Enochian)]
        BlackEnochianFeature = 25,

        [CustomComboInfo("Umbral Soul/Transpose Switcher", "Change Transpose into Umbral Soul when Umbral Soul is usable.", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 26,

        [CustomComboInfo("(Between the) Ley Lines", "Change Ley Lines into BTL when Ley Lines is active.", BLM.JobID, BLM.LeyLines)]
        BlackLeyLinesFeature = 56,

        [CustomComboInfo("Fire 1/3 Feature", "Fire 1 becomes Fire 3 outside of Astral Fire, and when Firestarter proc is up.", BLM.JobID, BLM.Fire)]
        BlackFireFeature = 70,

        [CustomComboInfo("Blizzard 1/2/3 Feature", "Blizzard 1 becomes Blizzard 3 when out of Umbral Ice. Freeze becomes Blizzard 2 when synced.", BLM.JobID, BLM.Blizzard, BLM.Freeze)]
        BlackBlizzardFeature = 71,

        [CustomComboInfo("Scathe/Xenoglossy Feature", "Scathe becomes Xenoglossy when available.", BLM.JobID, BLM.Scathe)]
        BlackScatheFeature = 95,

        [CustomComboInfo("Fire 1/3", "Fire 1 becomes Fire 3 outside of Astral Fire, OR when Firestarter proc is up.", BLM.JobID, BLM.Fire3 , BLM.Fire)]
        BlackFire13Feature = 98,

        [CustomComboInfo("Thunder", "Thunder 1/3 replaces Enochian/Fire 4/Blizzard 4 on Enochian switcher.\n Occurs when Thundercloud is up and either\n- Thundercloud buff on you is about to run out, or\n- Thunder debuff on your CURRENT target is about to run out\nassuming it won't interrupt timer upkeep.\nEnochian Stance Switcher must be active.", BLM.JobID, BLM.Thunder, BLM.Thunder3)]
        BlackThunderFeature = 100,

        [CustomComboInfo("Despair Feature", "Despair replaces Fire 4 when below 2400 MP.\nEnochian Stance Switcher must be active.", BLM.JobID, BLM.Enochian)]
        BlackDespairFeature = 101,

        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("Wanderer's into Pitch Perfect", "Replaces Wanderer's Minuet with Pitch Perfect while in WM.", BRD.JobID, BRD.WanderersMinuet)]
        BardWanderersPitchPerfectFeature = 41,

        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 42,

        [CustomComboInfo("Iron Jaws Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nAlternates between the two if Iron Jaws isn't available.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsFeature = 63,

        [CustomComboInfo("Burst Shot/Quick Nock into Apex Arrow", "Replaces Burst Shot and Quick Nock with Apex Arrow when gauge is full.", BRD.JobID, BRD.BurstShot, BRD.QuickNock)]
        BardApexFeature = 74,

        #endregion
        // ====================================================================================
        #region DANCER

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing.", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 33,

        [SecretCustomCombo]
        [ConflictingCombos(DancerDanceComboCompatibility)]
        [CustomComboInfo("Dance Step Combo", "Change Standard Step and Technical Step into each dance step while dancing.", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 31,

        [CustomComboInfo("Flourish Proc Saver", "Change Flourish into any available procs before using.", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 34,

        [CustomComboInfo("Single Target Multibutton", "Change Cascade into procs and combos as available.", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 43,

        [CustomComboInfo("AoE Multibutton", "Change Windmill into procs and combos as available.", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 50,

        [ConflictingCombos(DancerDanceStepCombo)]
        [CustomComboInfo(
            "Dance Step Feature",
            "Change actions into dance steps while dancing." +
            "\nThis helps ensure you can still dance with combos on, without using auto dance." +
            "\nYou can change the respective actions by inputting action IDs below for each dance step." +
            "\nThe defaults are Cascade, Flourish, Fan Dance and Fan Dance II. If set to 0, they will reset to these actions." +
            "\nYou can get Action IDs with Garland Tools by searching for the action and clicking the cog.",
            DNC.JobID)]
        DancerDanceComboCompatibility = 72,

        #endregion
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("Jump + Mirage Dive", "Replace (High) Jump with Mirage Dive when Dive Ready.", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 44,

        [CustomComboInfo("BOTD Into Stardiver", "Replace Blood of the Dragon with Stardiver when in Life of the Dragon.", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 46,

        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain.", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 0,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain.", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 1,

        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain.", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 2,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain.", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 3,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain.", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 4,

        [CustomComboInfo("Delirium Feature", "Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active.", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 57,

        [CustomComboInfo("Dark Knight Gauge Overcap Feature", "Replace AoE combo with gauge spender if you are about to overcap.", DRK.JobID, DRK.StalwartSoul)]
        DRKOvercapFeature = 85,

        [CustomComboInfo("Living Shadow Feature", "Living shadow will now be on main combo if its not on CD and you have gauge for it.", DRK.JobID, DRK.LivingShadow)]
        DRKLivingShadowFeature = 105,

        [CustomComboInfo("EoS Overcap Feature", "Uses EoS if you are above 8k mana and DarkSide is about to expire", DRK.JobID, DRK.EdgeOfShadow)]
        DarkManaOvercapFeature = 108, 


        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain.", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 20,

        [CustomComboInfo("Wicked Talon Combo", "Replace Wicked Talon with its combo chain.", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCombo = 21,

        [CustomComboInfo("Wicked Talon Continuation", "In addition to the Wicked Talon combo chain, put Continuation moves on Wicked Talon when appropriate.", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCont = 52,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 22,

        [CustomComboInfo("Fated Circle Feature", "In addition to the Demon Slaughter combo, add Fated Circle when charges are full.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerFatedCircleFeature = 30,

        [CustomComboInfo("Burst Strike to Bloodfest Feature", "Replace Burst Strike with Bloodfest if you have no powder gauge.", GNB.JobID, GNB.BurstStrike)]
        GunbreakerBloodfestOvercapFeature = 83,

        [CustomComboInfo("No Mercy Feature", "Replace No Mercy with Bow Shock, and then Sonic Break, while No Mercy is active.", GNB.JobID, GNB.NoMercy)]
        GunbreakerNoMercyFeature = 84,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain.", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistMainCombo = 23,

        [CustomComboInfo("Spread Shot Heat", "Replace Spread Shot with Auto Crossbow when overheated.", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("Hypercharge Feature", "Replace Heat Blast and Auto Crossbow with Hypercharge when not overheated.", MCH.JobID, MCH.HeatBlast, MCH.AutoCrossbow)]
        MachinistOverheatFeature = 47,

        [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active.", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 58,

        [SecretCustomCombo]
        [CustomComboInfo("Gauss Round / Ricochet Feature", "Replace Gauss Round and Ricochet with one or the other depending on which has more charges.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistGaussRoundRicochetFeature = 66,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("Monk AoE Combo", "Replaces Rockbreaker with the AoE combo chain, or Rockbreaker when Perfect Balance is active.", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 54,

        [CustomComboInfo("Monk Bootshine Feature", "Replaces Dragon Kick with Bootshine if both a form and Leaden Fist are up.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineFeature = 82,

        [CustomComboInfo("Monk Basic Rotation", "Basic Monk Combo on one button", MNK.JobID, MNK.Bootshine)]
        MnkBasicCombo = 96,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain.", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 17,

        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain.", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 18,

        [CustomComboInfo("Hakke Mujinsatsu Combo", "Replace Hakke Mujinsatsu with its combo chain.", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 19,

        [CustomComboInfo("Dream to Assassinate", "Replace Dream Within a Dream with Assassinate when Assassinate Ready.", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 45,

        [CustomComboInfo("Kassatsu to Trick", "Replaces Kassatsu with Trick Attack while Suiton or Hidden is up.\nCooldown tracking plugin recommended.", NIN.JobID, NIN.Kassatsu)]
        NinjaKassatsuTrickFeature = 87,

        [CustomComboInfo("Ten Chi Jin to Meisui", "Replaces Ten Chi Jin (the move) with Meisui while Suiton is up.\nCooldown tracking plugin recommended.", NIN.JobID, NIN.TenChiJin)]
        NinjaTCJMeisuiFeature = 88,

        [CustomComboInfo("Kassatsu Chi/Jin Feature", "Replaces Chi with Jin while Kassatsu is up if you have Enhanced Kassatsu.", NIN.JobID, NIN.Chi)]
        NinjaKassatsuChiJinFeature = 89,

        [CustomComboInfo("Hide to Mug", "Replaces Hide with Mug while in combat.", NIN.JobID, NIN.Hide)]
        NinjaHideMugFeature = 90,

        [CustomComboInfo("Aeolian to Ninjutsu Feature", "Replaces Aeolian Edge (combo) with Ninjutsu if any Mudra are used.", NIN.JobID, NIN.AeolianEdge)]
        NinjaNinjutsuFeature = 91,

        [CustomComboInfo("GCDs to Ninjutsu Feature", "Every GCD combo becomes Ninjutsu while Mudras are being used.", NIN.JobID, NIN.AeolianEdge, NIN.ArmorCrush, NIN.HakkeMujinsatsu , NIN.GustSlash , NIN.SpinningEdge)]
        NinjaGCDNinjutsuFeature = 92,



        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain.", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 5,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain.", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("Atonement Feature", "Replace Royal Authority with Atonement when under the effect of Sword Oath.", PLD.JobID, PLD.RoyalAuthority)]
        PaladinAtonementFeature = 59,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain.", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("Requiescat Confiteor", "Replace Requiescat with Confiter while under the effect of Requiescat.", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 55,

        [CustomComboInfo("Requiescat Feature", "Replace Royal Authority/Goring Blade combo with Holy Spirit and Prominence combo with Holy Circle while Requiescat is active.\nRequires said combos to be activated to work.", PLD.JobID, PLD.RoyalAuthority, PLD.GoringBlade, PLD.Prominence)]
        PaladinRequiescatFeature = 97,

        [SecretCustomCombo]
        [CustomComboInfo("Confiteor Feature", "Replace Holy Spirit/Circle with Confiteor while MP is under 4000 and Requiescat is up.", PLD.JobID, PLD.HolySpirit, PLD.HolyCircle)]
        PaladinConfiteorFeature = 69,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/Verthunder 2 with Impact when Dualcast or Swiftcast are active.", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 48,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules.", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 49,

        [SecretCustomCombo]
        [CustomComboInfo("Redoublement Combo Plus", "Replaces Redoublement with Verflare/Verholy after Enchanted Redoublement, whichever is more appropriate.\nRequires Redoublement Combo.", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeComboPlus = 68,

        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 53,

        [CustomComboInfo("Verproc into Jolt Plus", "Additionally replaces Verstone/Verfire with Veraero/Verthunder if dualcast/swiftcast are up.\nRequires Verproc into Jolt.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocComboPlus = 93,

        [CustomComboInfo("Verproc into Jolt Plus Opener Feature", "Turns Verfire into Verthunder when out of combat.\nRequires Verproc into Jolt Plus.", RDM.JobID, RDM.Verfire)]
        RedMageVerprocOpenerFeature = 94,

        [CustomComboInfo("Ogcd's all in one button", "Ogcd Combo", RDM.JobID, RDM.ContreSixte , RDM.Engagement , RDM.Fleche)]
        RedMageOgcdCombo = 99,

        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain.", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 11,

        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain.", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 12,

        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain.", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 13,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain.", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 14,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain.", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 15,

        [CustomComboInfo("Seigan to Third Eye", "Replace Seigan with Third Eye when not procced.", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 51,

        [CustomComboInfo("Jinpu/Shifu Feature", "Replace Meikyo Shisui with Jinpu or Shifu depending on what is needed.", SAM.JobID, SAM.MeikyoShisui)]
        SamuraiJinpuShifuFeature = 81,

        [ConflictingCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Iaijutsu", "Replace Tsubame-gaeshi with Iaijutsu when Sen is empty.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiIaijutsuFeature = 60,

        [ConflictingCombos(SamuraiIaijutsuShohaFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Shoha", "Replace Tsubame-gaeshi with Shoha when meditation is 3.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiShohaFeature = 61,

        [ConflictingCombos(SamuraiTsubameGaeshiIaijutsuFeature)]
        [CustomComboInfo("Iaijutsu to Tsubame-gaeshi", "Replace Iaijutsu with Tsubame-gaeshi when Sen is not empty.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuTsubameGaeshiFeature = 64,

        [ConflictingCombos(SamuraiTsubameGaeshiShohaFeature)]
        [CustomComboInfo("Iaijutsu to Shoha", "Replace Iaijutsu with Shoha when meditation is 3.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuShohaFeature = 65,

        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("Seraph Fey Blessing/Consolation", "Change Fey Blessing into Consolation when Seraph is out.", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 29,

        [CustomComboInfo("ED Aetherflow", "Change Energy Drain into Aetherflow when you have no more Aetherflow stacks.", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 37,

        #endregion
        // ====================================================================================
        #region SUMMONER

        [CustomComboInfo("Demi-summon combiners", "Dreadwyrm Trance, Summon Bahamut, and Firebird Trance are now one button.\nDeathflare, Enkindle Bahamut, and Enkindle Phoenix are now one button", SMN.JobID, SMN.DreadwyrmTrance, SMN.Deathflare)]
        SummonerDemiCombo = 28,

        [CustomComboInfo("Brand of Purgatory Combo", "Replaces Fountain of Fire with Brand of Purgatory when under the affect of Hellish Conduit + Enkindle Phoenix is now on same button!", SMN.JobID, SMN.Ruin1, SMN.Ruin3)]
        SummonerBoPCombo = 38,

        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when out of Aetherflow stacks", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 39,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Syphon when out of Aetherflow stacks", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 40,

        [CustomComboInfo("Demi-Summon Combiners Ultra", "Dreadwyrm Trance, Summon Bahamut, Firebird Trance, Deathflare, Enkindle Bahamut, and Enkindle Phoenix are now one button.\nRequires Demi-Summon Combiners feature.", SMN.JobID, SMN.DreadwyrmTrance)]
        SummonerDemiComboUltra = 80,

        [CustomComboInfo("SummonerDotFeature", "Currently still in testing.. dont enable", SMN.JobID, SMN.Miasma3, SMN.Bio3)]
        SummonerDotFeature = 104,

        [CustomComboInfo("Summoner Easy rotation", "Whole bahamut rotation is now on ruin3 combo + Enkindle Bahamut on same button", SMN.JobID, SMN.Ruin3, SMN.Ruin4)]
        SummonerEasyRotation = 106,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "Replace Storms Path with its combo chain", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Mythril Tempest with its combo chain", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 10,

        [CustomComboInfo("Warrior Gauge Overcap Feature", "Replace Single-target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate beast gauge", WAR.JobID, WAR.MythrilTempest, WAR.StormsEye, WAR.StormsPath)]
        WarriorGaugeOvercapFeature = 78,

        [CustomComboInfo("Inner Release Feature", "Replace Single-target and AoE combo with Fell Cleave/Decimate during Inner Release", WAR.JobID, WAR.MythrilTempest, WAR.StormsPath)]
        WarriorInnerReleaseFeature = 79,

        [CustomComboInfo("Nascent Flash Feature", "Replace Nascent Flash with Raw intuition when level synced below 76", WAR.JobID, WAR.NascentFlash)]
        WarriorNascentFlashFeature = 67,

        [CustomComboInfo("Fellcleave/IB Feature", "Replaces Main Combo With Fellcleave/IB When you are about to overcap ", WAR.JobID, WAR.FellCleave, WAR.InnerBeast)]
        WarriorFellCleaveOvercapFeature = 107,

        [CustomComboInfo("Upheaval Feature", "Adds Upheaval into maincombo if Beserk/IR buff is present or Beserk/IR is ON CD", WAR.JobID, WAR.Upheaval)]
        WarriorUpheavalMainComboFeature = 111,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusSolace)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusRapture)]
        WhiteMageRaptureMiseryFeature = 36,

        [CustomComboInfo("Cure 2 to Cure Level Sync", "Changes Cure 2 to Cure when below level 30 in synced content.", WHM.JobID, WHM.Cure2)]
        WhiteMageCureFeature = 76,

        [CustomComboInfo("Afflatus Feature", "Changes Cure 2 into Afflatus Solace, and Medica into Afflatus Rapture, when lilies are up.", WHM.JobID, WHM.Cure2, WHM.Medica)]
        WhiteMageAfflatusFeature = 77,

        [CustomComboInfo("Swiftcast Into Raise", "Changes Swiftcast into Raise", WHM.JobID, WHM.Raise, WHM.Swiftcast)]
        WHMRaiseFeature = 112,
        #endregion
        // ====================================================================================
        #region DISCIPLE OF MAGIC

        //[CustomComboInfo("SwiftcastToRes", "Replaces Swiftcast with ressurection", DoM.JobID, WHM.Raise, SMN.Resurrection, SCH.Resurrection, AST.Ascend, RDM.Verraise)]
       // DoMSwiftcastFeature = 109,

        #endregion
    }
}
