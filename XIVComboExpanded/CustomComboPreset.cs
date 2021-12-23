using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Combo presets.
    /// </summary>
    public enum CustomComboPreset
    {
        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("Draw on Play", "Play turns into Draw when no card is drawn, as well as the usual Play behavior.", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 1,

        [CustomComboInfo("Minor Arcana to Crown Play", "Changes Minor Arcana to Crown Play when a card is not drawn or has Lord Or Lady Buff.", AST.JobID, AST.CrownPlay, AST.LadyOfCrown, AST.LordOfCrowns)]
        AstrologianCrownPlayFeature = 2,

        [CustomComboInfo("Benefic 2 to Benefic Level Sync", "Changes Benefic 2 to Benefic when below level 26 in synced content.", AST.JobID, AST.Benefic2)]
        AstrologianBeneficFeature = 3,

        [ConflictingCombos(DoMSwiftcastFeature)]
        [CustomComboInfo("AST Raise Feature", "Changes Swiftcast to Ascend", AST.JobID, AST.Swiftcast, AST.Ascend)]
        AstrologianAscendFeature = 4,

        [CustomComboInfo("DPS Feature", "Adds Combust to the main malefic combo whenever the debuff is not present or about to expire", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1)]
        AstrologianDpsFeature = 5,

        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the DPS feature when below 8k mana", AST.JobID, AST.FallMalefic, AST.LucidDreaming)]
        AstrologianLucidFeature = 6,

        [CustomComboInfo("Astrodyne Feature", "Adds Astrodyne to the DPS feature when ready", AST.JobID, AST.FallMalefic, AST.LucidDreaming)]
        AstrologianAstrodyneFeature = 7,

        [CustomComboInfo("Aspected Helios Feature", "Replaces Aspected Helios whenever you are under Aspected Helios regen with Helios", AST.JobID, AST.AspectedHelios, AST.Helios)]
        AstrologianHeliosFeature = 8,

        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("Enochian Stance Switcher ++", "Change Scathe to Fire 4 or Blizzard 4 depending on stance. \nScathe becomes all in one rotation. \nIf Thunder Feature is turned on it also adds Thunder3 proces onto all in one combo when DoT is about to expire or dosen't exist \n This REQUIRES other features to be turned on!!!", BLM.JobID, BLM.Scathe)]
        BlackEnochianFeature = 100,

        [CustomComboInfo("Umbral Soul/Transpose Switcher", "Change Transpose into Umbral Soul when Umbral Soul is usable.", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 101,

        [CustomComboInfo("(Between the) Ley Lines", "Change Ley Lines into BTL when Ley Lines is active.", BLM.JobID, BLM.LeyLines)]
        BlackLeyLinesFeature = 102,

        [CustomComboInfo("Blizzard 1/2/3 Feature", "Blizzard 1 becomes Blizzard 3 when out of Umbral Ice. Freeze becomes Blizzard 2 when synced.", BLM.JobID, BLM.Blizzard, BLM.Freeze)]
        BlackBlizzardFeature = 103,

        [CustomComboInfo("Scathe/Xenoglossy Feature", "Scathe becomes Xenoglossy when available.", BLM.JobID, BLM.Scathe)]
        BlackScatheFeature = 104,

        [CustomComboInfo("Fire 1/3", "Fire 1 becomes Fire 3 outside of Astral Fire, OR when Firestarter proc is up.", BLM.JobID, BLM.Fire3, BLM.Fire)]
        BlackFire13Feature = 105,

        [CustomComboInfo("Thunder", "Thunder 1/3 replaces Enochian/Fire 4/Blizzard 4 on Enochian switcher.\n Occurs when Thundercloud is up and either\n- Thundercloud buff on you is about to run out, or\n- Thunder debuff on your CURRENT target is about to run out\nassuming it won't interrupt timer upkeep.\nEnochian Stance Switcher must be active.", BLM.JobID, BLM.Thunder, BLM.Thunder3)]
        BlackThunderFeature = 106,

        [CustomComboInfo("Despair Feature", "Despair replaces Fire 4 when below 2400 MP.\nEnochian Stance Switcher must be active.", BLM.JobID, BLM.Fire4)]
        BlackDespairFeature = 107,

        [CustomComboInfo("AoE Combo Feature", "One Button AoE Feature that adds whole AoE rotation onto HighBlizzard2 (TESTING ONLY!!!)", BLM.JobID, BLM.Flare, BLM.HighBlizzardII, BLM.Freeze, BLM.Thunder4, BLM.HighFireII, BLM.Fire2, BLM.Thunder2)]
        BlackAoEComboFeature = 108,

        [CustomComboInfo("Blizzard Paradox Feature", "Adds Paradox onto ice phase combo", BLM.JobID, BLM.Paradox)]
        BlackBlizzardParadoxFeature = 109,

        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("Wanderer's into Pitch Perfect", "Replaces Wanderer's Minuet with Pitch Perfect while in WM.", BRD.JobID, BRD.WanderersMinuet)]
        BardWanderersPitchPerfectFeature = 200,

        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 201,

        [CustomComboInfo("Iron Jaws Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nAlternates between the two if Iron Jaws isn't available.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsFeature = 202,

        [CustomComboInfo("Burst Shot/Quick Nock into Apex Arrow", "Replaces Burst Shot and Quick Nock with Apex Arrow when gauge is full.", BRD.JobID, BRD.BurstShot, BRD.QuickNock)]
        BardApexFeature = 203,

        #endregion
        // ====================================================================================
        #region DANCER

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing.", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 300,

        [SecretCustomCombo]
        [ConflictingCombos(DancerDanceComboCompatibility)]
        [CustomComboInfo("Dance Step Combo", "Change Standard Step and Technical Step into each dance step while dancing.", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 301,

        [CustomComboInfo("Flourish Proc Saver", "Change Flourish into any available procs before using.", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 302,

        [CustomComboInfo("Single Target Multibutton", "Change Cascade into procs and combos as available.", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 303,

        [CustomComboInfo("AoE Multibutton", "Change Windmill into procs and combos as available.", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 304,

        [ConflictingCombos(DancerDanceStepCombo)]
        [CustomComboInfo(
            "Dance Step Feature",
            "Change actions into dance steps while dancing." +
            "\nThis helps ensure you can still dance with combos on, without using auto dance." +
            "\nYou can change the respective actions by inputting action IDs below for each dance step." +
            "\nThe defaults are Cascade, Flourish, Fan Dance and Fan Dance II. If set to 0, they will reset to these actions." +
            "\nYou can get Action IDs with Garland Tools by searching for the action and clicking the cog.",
            DNC.JobID)]
        DancerDanceComboCompatibility = 305,

        [CustomComboInfo("Devilment Feature", "Change Devilment into Starfall Dance after use.", DNC.JobID, DNC.Devilment)]
        DancerDevilmentFeature = 306,

        #endregion
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("Jump + Mirage Dive", "Replace (High) Jump with Mirage Dive when Dive Ready.", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 400,

        [CustomComboInfo("BOTD Into Stardiver", "Replace Blood of the Dragon with Stardiver when in Life of the Dragon.", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 401,

        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain.", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 402,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain.", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 403,

        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain.", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 404,

        [CustomComboInfo("Wheeling Thrust/Fang and Claw Option", "When you have either Enhanced Fang and Claw or Wheeling Thrust,\nChaos Thrust Combo becomes Wheeling Thrust and Full Thrust Combo becomes Fang and Claw.\nRequires Chaos Thrust Combo and Full Thrust Combo.", DRG.JobID, DRG.FullThrust, DRG.ChaosThrust)]
        DragoonFangThrustFeature = 405,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain.", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 500,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain.", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 501,

        [CustomComboInfo("Delirium Feature", "Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active.", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 502,

        [CustomComboInfo("Dark Knight Gauge Overcap Feature", "Replace AoE combo with gauge spender if you are about to overcap.", DRK.JobID, DRK.StalwartSoul)]
        DRKOvercapFeature = 503,

        [CustomComboInfo("Living Shadow Feature", "Living shadow will now be on main combo if its not on CD and you have gauge for it.", DRK.JobID, DRK.LivingShadow)]
        DRKLivingShadowFeature = 504,

        [CustomComboInfo("EoS Overcap Feature", "Uses EoS if you are above 8k mana and DarkSide is about to expire", DRK.JobID, DRK.EdgeOfShadow)]
        DarkManaOvercapFeature = 505,

        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain.", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 600,

        [CustomComboInfo("Gnashing Fang Combo", "Replace Gnashing Fang with its combo chain.", GNB.JobID, GNB.GnashingFang)]
        GunbreakerGnashingFangCombo = 601,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 602,

        [CustomComboInfo("Fated Circle Feature", "In addition to the Demon Slaughter combo, add Fated Circle when charges are full.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerFatedCircleFeature = 603,

        [CustomComboInfo("Burst Strike to Bloodfest Feature", "Replace Burst Strike with Bloodfest if you have no powder gauge.", GNB.JobID, GNB.BurstStrike)]
        GunbreakerBloodfestOvercapFeature = 604,

        [CustomComboInfo("No Mercy Feature", "Replace No Mercy with Bow Shock, and then Sonic Break, while No Mercy is active.", GNB.JobID, GNB.NoMercy)]
        GunbreakerNoMercyFeature = 605,

        [CustomComboInfo("DangerZoneFeature", "Adds DangerZone on main combo.", GNB.JobID, GNB.DangerZone)]
        GunbreakerDangerZoneFeature = 606,

        [CustomComboInfo("DoubleDownFeature", "Adds DangerZone on main combo when under NoMercy buff", GNB.JobID, GNB.DoubleDown)]
        GunbreakerDoubleDownFeature = 607,

        [CustomComboInfo("BurstStrikeContinuation", "Adds Hypervelocity on Burst Strike Continuation combo", GNB.JobID, GNB.BurstStrike, GNB.Hypervelocity)]
        GunbreakerBurstStrikeConFeature = 608,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain.", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistMainCombo = 700,

        [CustomComboInfo("Spread Shot/Scattergun Heat, +BioBlaster", "Spread Shot turns into Scattergun when lvl 82 or higher, Both turn into Auto Crossbow when overheated\nand Bioblaster is used first whenever it is off cooldown.", MCH.JobID, MCH.AutoCrossbow, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 701,

        [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active.", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 702,

        [SecretCustomCombo]
        [CustomComboInfo("Gauss Round / Ricochet Feature", "Replace Gauss Round and Ricochet with one or the other depending on which has more charges.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistGaussRoundRicochetFeature = 703,

        [CustomComboInfo("Drill / Air Anchor (Hot Shot) Feature", "Replace Drill and Air Anchor (Hot Shot) with one or the other (or Chainsaw) depending on which is on cooldown.", MCH.JobID, MCH.Drill, MCH.HotShot, MCH.AirAnchor)]
        MachinistHotShotDrillChainsawFeature = 704,

        [CustomComboInfo("Drill/Air/Chain Saw Feature On Main Combo", "Air Anchor followed by Drill is added onto main combo if you use Reassemble.\nIf AirAnchor is on cooldown and you use Reassemble Chain Saw will be added to main combo instead.\nRequires (Heated) Shot Combo.", MCH.JobID, MCH.Drill, MCH.AirAnchor, MCH.HotShot, MCH.Reassemble)]
        MachinistDrillAirOnMainCombo = 705,

        [SecretCustomCombo]
        [CustomComboInfo("Single Button Heat Blast", "Switches Heat Blast to Hypercharge, If Wildfire is off cooldown and you have enough heat for Hypercharge then Hypercharge will be replaced with Wildfire.\nAlso weaves Ricochet/Gauss Round on Heat Blast when necessary.", MCH.JobID, MCH.GaussRound, MCH.Ricochet, MCH.HeatBlast, MCH.Wildfire)]
        MachinistHeatblastGaussRicochetFeature = 706,

        [CustomComboInfo("Alternate Drill/Air Feature on Main Combo", "Drill/Air/Hotshot Feature is added onto main combo (Note: It will add them onto main combo ONLY if you are under Reassemble Buff \nOr Reasemble is on CD(Will do nothing if Reassemble is OFF CD)\nRequires (Heated) Shot Combo.", MCH.JobID, MCH.Drill, MCH.AirAnchor, MCH.HotShot, MCH.Reassemble)]
        MachinistAlternateMainCombo = 707,

        [CustomComboInfo("Single Button HeatBlast On Main Combo Option", "Adds Single Button Heatblast onto the main combo when the option is enabled.\n Requires (Heated) Shot Combo.", MCH.JobID, MCH.HeatBlast)]
        MachinistHeatBlastOnMainCombo = 708,

        [CustomComboInfo("Battery Overcap Option", "Overcharge protection for your Battery, If you are at 100 battery charge rook/queen will be added to your (Heated) Shot Combo.\nRequires (Heated) Shot Combo", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverChargeOption = 709,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("Monk AoE Combo", "Replaces Rockbreaker with the AoE combo chain, or Rockbreaker when Perfect Balance is active.", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 800,

        [CustomComboInfo("Monk Bootshine Feature", "Replaces Dragon Kick with Bootshine if both a form and Leaden Fist are up.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineFeature = 801,

        [CustomComboInfo("Monk Basic Rotation", "Basic Monk Combo on one button", MNK.JobID, MNK.Bootshine)]
        MnkBasicCombo = 802,

        [CustomComboInfo("Perfect Balance Feature", "Perfect Balance becomes Masterful Blitz while you have 3 Beast Chakra.", MNK.JobID, MNK.PerfectBalance)]
        MonkPerfectBalanceFeature = 803,

        [CustomComboInfo("Monk Bootshine Balance Feature", "Replaces Dragon Kick with Masterful Blitz if you have 3 Beast Chakra.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineBalanceFeature = 804,

        [CustomComboInfo("Howling Fist / Meditation Feature", "Replaces Howling Fist/Enlightenment with Meditation when the Fifth Chakra is not open.", MNK.JobID, MNK.HowlingFist, MNK.Enlightenment)]
        MonkHowlingFistMeditationFeature = 805,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain.", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 900,

        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain.", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 901,

        [CustomComboInfo("Hakke Mujinsatsu Combo", "Replace Hakke Mujinsatsu with its combo chain.", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 902,

        [CustomComboInfo("Dream to Assassinate", "Replace Dream Within a Dream with Assassinate when Assassinate Ready.", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 903,

        [CustomComboInfo("Kassatsu to Trick", "Replaces Kassatsu with Trick Attack while Suiton or Hidden is up.\nCooldown tracking plugin recommended.", NIN.JobID, NIN.Kassatsu)]
        NinjaKassatsuTrickFeature = 904,

        [CustomComboInfo("Ten Chi Jin to Meisui", "Replaces Ten Chi Jin (the move) with Meisui while Suiton is up.\nCooldown tracking plugin recommended.", NIN.JobID, NIN.TenChiJin)]
        NinjaTCJMeisuiFeature = 905,

        [CustomComboInfo("Kassatsu Chi/Jin Feature", "Replaces Chi with Jin while Kassatsu is up if you have Enhanced Kassatsu.", NIN.JobID, NIN.Chi)]
        NinjaKassatsuChiJinFeature = 906,

        [CustomComboInfo("Hide to Mug", "Replaces Hide with Mug while in combat.", NIN.JobID, NIN.Hide)]
        NinjaHideMugFeature = 907,

        [CustomComboInfo("Aeolian to Ninjutsu Feature", "Replaces Aeolian Edge (combo) with Ninjutsu if any Mudra are used.", NIN.JobID, NIN.AeolianEdge)]
        NinjaNinjutsuFeature = 908,

        [CustomComboInfo("GCDs to Ninjutsu Feature", "Every GCD combo becomes Ninjutsu while Mudras are being used.", NIN.JobID, NIN.AeolianEdge, NIN.ArmorCrush, NIN.HakkeMujinsatsu, NIN.GustSlash, NIN.SpinningEdge)]
        NinjaGCDNinjutsuFeature = 909,

        [CustomComboInfo("Huraijin / Raiju Feature", "Replaces Huraijin with Forked and Fleeting Raiju when available.", NIN.JobID, NIN.Huraijin, NIN.FleetingRaiju)]
        NinjaHuraijinRaijuFeature = 910,

        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain.", PLD.JobID, PLD.GoringBlade, PLD.FastBlade, PLD.RiotBlade)]
        PaladinGoringBladeCombo = 1000,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain.", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinRoyalAuthorityCombo = 1001,

        [CustomComboInfo("Atonement Feature", "Replace Royal Authority with Atonement when under the effect of Sword Oath.", PLD.JobID, PLD.RoyalAuthority, PLD.Confiteor)]
        PaladinAtonementFeature = 1002,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain.", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 1003,

        [CustomComboInfo("Requiescat Feature", "Replace Royal Authority/Goring Blade combo with Holy Spirit and Prominence combo with Holy Circle while Requiescat is active \n And when Fight Or Flight is not Active.\nRequires said combos to be activated to work.", PLD.JobID, PLD.RoyalAuthority, PLD.GoringBlade, PLD.Prominence)]
        PaladinRequiescatFeature = 1004,

        [CustomComboInfo("Confiteor Feature", "Replace Holy Spirit/Circle with Confiteor when Requiescat is up and MP is under 2000 or only one stack remains \nand adds Faith/Truth/Valor Combo after Confiteor.", PLD.JobID, PLD.HolySpirit, PLD.HolyCircle)]
        PaladinConfiteorFeature = 1005,

        [CustomComboInfo("Scornful Spirits Feature", "Replace Spirits Within and Circle of Scorn with whichever is available soonest.", PLD.JobID, PLD.CircleOfScorn, PLD.SpiritsWithin, PLD.Expiacion)]
        PaladinScornfulSpiritsFeature = 1006,

        [SecretCustomCombo]
        [CustomComboInfo("Royal Goring Option", "Insert Goring Blade into the main combo when appropriate.\nRequires Royal Authority Combo", PLD.JobID, PLD.RoyalAuthority, PLD.GoringBlade)]
        PaladinRoyalGoringOption = 1007,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/Verthunder 2 with Impact when Dualcast or Swiftcast are active.", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 1100,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules.", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 1101,

        [SecretCustomCombo]
        [CustomComboInfo("Redoublement Combo Plus", "Replaces Redoublement with Verflare/Verholy after Enchanted Redoublement, whichever is more appropriate.\nRequires Redoublement Combo.", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeComboPlus = 1102,

        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 1103,

        [CustomComboInfo("Verproc into Jolt Plus", "Additionally replaces Verstone/Verfire with Veraero/Verthunder if dualcast/swiftcast are up.\nRequires Verproc into Jolt.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocComboPlus = 1104,

        [CustomComboInfo("Verproc into Jolt Plus Opener Feature", "Turns Verfire into Verthunder when out of combat.\nRequires Verproc into Jolt Plus.", RDM.JobID, RDM.Verfire)]
        RedMageVerprocOpenerFeature = 1105,

        [CustomComboInfo("oGCD Feature", "Replace Contre Strike and Fleche with whichever is available soonest.", RDM.JobID, RDM.ContreSixte, RDM.Fleche)]
        RedMageOgcdCombo = 1106,

        [CustomComboInfo("Resolution Feature", "Adds Resolution finisher to Verthunder/Verareo Combo ", RDM.JobID, RDM.Verstone, RDM.Verfire, RDM.Resolution)]
        RedmageResolutionFinisher = 1107,

        [CustomComboInfo("Resolution Feature Melee", "Adds Resolution finisher to melee combo ", RDM.JobID, RDM.Redoublement, RDM.Resolution)]
        RedmageResolutionFinisherMelee = 1108,
        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain.", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 1200,

        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain.", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 1201,

        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain.", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 1202,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain.", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 1203,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain.", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 1204,

        [CustomComboInfo("Seigan to Third Eye", "Replace Seigan with Third Eye when not procced.", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 1205,

        [CustomComboInfo("Jinpu/Shifu Feature", "Replace Meikyo Shisui with Jinpu or Shifu depending on what is needed.", SAM.JobID, SAM.MeikyoShisui)]
        SamuraiJinpuShifuFeature = 1206,

        [ConflictingCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Iaijutsu", "Replace Tsubame-gaeshi with Iaijutsu when Sen is empty.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiIaijutsuFeature = 1207,

        [ConflictingCombos(SamuraiIaijutsuShohaFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Shoha", "Replace Tsubame-gaeshi with Shoha when meditation is 3.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiShohaFeature = 1208,

        [ConflictingCombos(SamuraiTsubameGaeshiIaijutsuFeature)]
        [CustomComboInfo("Iaijutsu to Tsubame-gaeshi", "Replace Iaijutsu with Tsubame-gaeshi when Sen is not empty.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuTsubameGaeshiFeature = 1209,

        [ConflictingCombos(SamuraiTsubameGaeshiShohaFeature)]
        [CustomComboInfo("Iaijutsu to Shoha", "Replace Iaijutsu with Shoha when meditation is 3.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuShohaFeature = 1210,

        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("Seraph Fey Blessing/Consolation", "Change Fey Blessing into Consolation when Seraph is out.", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 1300,

        [CustomComboInfo("ED Aetherflow", "Change Energy Drain into Aetherflow when you have no more Aetherflow stacks.", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 1301,

        [ConflictingCombos(DoMSwiftcastFeature)]
        [CustomComboInfo("SCH Raise Feature", "Changes Swiftcast to Resurrection.", SCH.JobID, SCH.Swiftcast, SCH.Resurrection)]
        SchRaiseFeature = 1302,

        #endregion
        // ====================================================================================
        #region SUMMONER

        // FESTER
        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when our of Aetherflow stacks.", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 1400,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Siphon when out of Aetherflow stacks.", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 1401,

        [CustomComboInfo("Ruin IV Fester Feature", "Change Fester into Ruin IV when out of Aetherflow stacks, ED/ES is on cooldown, and Ruin IV is up.", SMN.JobID, SMN.Painflare)]
        SummonerFesterPainflareRuinFeature = 1402,

        // SINGLE TARGET
        [CustomComboInfo("Single Target Combo", "Enables changing Single-Target Combo (Ruin III).", SMN.JobID, SMN.Ruin3, SMN.Deathflare)]
        SummonerMainComboFeature = 1403,

        [CustomComboInfo("Single Target Demi Feature", "Replaces Astral Impulse/Fountain of Fire with Enkindle/Deathflare/Rekindle when appropriate. Requires Single Target Combo Feature.", SMN.JobID, SMN.Ruin3)]
        SummonerSingleTargetDemiFeature = 1404,

        [CustomComboInfo("Garuda Unique Feature", "Adds Slipstream on Ruin III. Requires Single Target Combo Feature.", SMN.JobID, SMN.Ruin3)]
        SummonerGarudaUniqueFeature = 1405,

        [CustomComboInfo("Ifrit Unique Feature", "Adds Crimson Cyclone/Crimson Strike on Ruin III. Requires Single Target Combo Feature.", SMN.JobID, SMN.Ruin3)]
        SummonerIfritUniqueFeature = 1406,

        [CustomComboInfo("Titan Unique Feature", "Adds Mountain Buster on Ruin III. Requires Single Target Combo Feature.", SMN.JobID, SMN.Ruin3)]
        SummonerTitanUniqueFeature = 1407,

        // AOE
        [CustomComboInfo("AOE Combo", "Enables changing AOE Combo (Tri-Disaster)", SMN.JobID, SMN.Tridisaster, SMN.Deathflare)]
        SummonerAOEComboFeature = 1408,

        [CustomComboInfo("AOE Demi Feature", "Replaces Astral Flare/Brand of Purgatory with Enkindle/Deathflare/Rekindle when appropriate. Requires AOE Combo Feature.", SMN.JobID, SMN.Ruin3)]
        SummonerAOEDemiFeature = 1409,

        [CustomComboInfo("Egi AOE Combo Feature", "Adds Requires Single Target Combo Feature.", SMN.JobID, SMN.Tridisaster)]
        SummonerEgiAoeComboFeature = 1410,

        // BOTH
        [CustomComboInfo("Egi Attacks Feature", "Replaces Ruin III and Tri-Disaster with Egi attacks. Requires Single Target or AOE Combo feature.", SMN.JobID, SMN.Fester, SMN.EnergyDrain, SMN.Ruin4)]
        SummonerEgiAttacksFeature = 1411,

        // EXTRA

        [CustomComboInfo("Ruin 4 On Ruin3 Combo Feature", "Adds Ruin4 on main Ruin3 combo feature when there are currently no summons being active.", SMN.JobID, SMN.Ruin, SMN.Ruin2, SMN.Ruin3, SMN.Ruin4)]
        SummonerRuin4ToRuin3Feature = 1412,

        [CustomComboInfo("Ruin 4 On Tridisaster Feature", "Adds Ruin4 on main Tridisaster combo feature when there are currently no summons being active.", SMN.JobID, SMN.Tridisaster)]
        SummonerRuin4ToTridisasterFeature = 1413,

        [CustomComboInfo("Earlier Demi Weave Feature", "Adds Enkindle right after summoning Demi. (Looks like Enkindle Bahamut for both Demis)", SMN.JobID, SMN.Ruin3, SMN.Ruin4, SMN.Tridisaster)]
        SummonerEnkindleWeave = 1414,

        [CustomComboInfo("Carbuncle Summon Feature", "Reminds you allways to summon Carbuncle by replacing Ruin (Carbuncle Summon Reminder Feature).", SMN.JobID, SMN.SummonCarbuncle, SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
        SummonerCarbuncleSummonFeature = 1415,

        [CustomComboInfo("Lazy Fester Feature", "Adds Fester during GCDs of most skills (Ruin3/Ruin4/AstralImpulse/FountainOfFire)", SMN.JobID, SMN.Ruin3, SMN.Ruin4, SMN.AstralImpulse, SMN.FountainOfFire)]
        SummonerLazyFesterFeature = 1416,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "All in one main combo feature adds Storm's Eye/Path", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 1500,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 1501,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Overpower with its combo chain", WAR.JobID, WAR.MythrilTempest, WAR.Overpower)]
        WarriorMythrilTempestCombo = 1502,

        [CustomComboInfo("Warrior Gauge Overcap Feature", "Replace Single-target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate beast gauge", WAR.JobID, WAR.MythrilTempest, WAR.StormsEye, WAR.StormsPath)]
        WarriorGaugeOvercapFeature = 1503,

        [CustomComboInfo("Inner Release Feature", "Replace Single-target and AoE combo with Fell Cleave/Decimate during Inner Release", WAR.JobID, WAR.MythrilTempest, WAR.StormsPath)]
        WarriorInnerReleaseFeature = 1504,

        [CustomComboInfo("Nascent Flash Feature", "Replace Nascent Flash with Raw intuition when level synced below 76", WAR.JobID, WAR.NascentFlash)]
        WarriorNascentFlashFeature = 1505,

        [CustomComboInfo("Fellcleave/IB Feature", "Replaces Main Combo With Fellcleave/IB When you are about to overcap ", WAR.JobID, WAR.FellCleave, WAR.InnerBeast)]
        WarriorFellCleaveOvercapFeature = 1506,

        [CustomComboInfo("Upheaval Feature", "Adds Upheaval into maincombo if you have Surging Tempest and if you're synced below 70 while Beserk buff is ON CD", WAR.JobID, WAR.Upheaval)]
        WarriorUpheavalMainComboFeature = 1507,

        [CustomComboInfo("Primal Rend Feature", "Replace Inner Beast and Steel Cyclone with Primal Rend when available (Also added onto Main AoE combo)", WAR.JobID, WAR.PrimalRend, WAR.InnerBeast, WAR.SteelCyclone)]
        WarriorPrimalRendFeature = 1508,

        [CustomComboInfo("Orogeny Feature", "Adds Orogeny onto main AoE combo when you are buffed with Surging Tempest", WAR.JobID, WAR.Orogeny, WAR.MythrilTempest)]
        WarriorOrogenyFeature = 1509,

        [CustomComboInfo("Inner Chaos option", "Adds Inner Chaos to Storms Path Combo if you are buffed with Nascent Chaos and Surging Tempest.\nRequires Storms Path Combo", WAR.JobID, WAR.InnerChaos, WAR.StormsPath)]
        WarriorInnerChaosOption = 1510,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusSolace)]
        WhiteMageSolaceMiseryFeature = 1600,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusRapture)]
        WhiteMageRaptureMiseryFeature = 1601,

        [CustomComboInfo("Cure 2 to Cure Level Sync", "Changes Cure 2 to Cure when below level 30 in synced content.", WHM.JobID, WHM.Cure2)]
        WhiteMageCureFeature = 1602,

        [CustomComboInfo("Afflatus Feature", "Changes Cure 2 into Afflatus Solace, and Medica into Afflatus Rapture, when lilies are up.", WHM.JobID, WHM.Cure2, WHM.Medica)]
        WhiteMageAfflatusFeature = 1603,

        [ConflictingCombos(DoMSwiftcastFeature)]
        [CustomComboInfo("WHM Raise Feature", "Changes Swiftcast to Raise", WHM.JobID, WHM.Swiftcast, WHM.Raise)]
        WHMRaiseFeature = 1604,

        [CustomComboInfo("DoT on Glare1/3 Feature", "Adds DoT on Glare1/3 when DoT is not preset on about to expire and when you are inCombat (You can still prepull Glare)", WHM.JobID, WHM.Glare3, WHM.Dia)]
        WHMDotMainComboFeature = 1605,

        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming onto Glare1/3 Feature combo when you are below 8k mana", WHM.JobID, WHM.LucidDreaming)]
        WHMLucidDreamingFeature = 1606,

        [CustomComboInfo("Medica Feature", "Replaces Medica2 whenever you are under Medica2 regen with Medica1", WHM.JobID, WHM.Medica1, WHM.Medica2)]
        WHMMedicaFeature = 1607,

        #endregion
        // ====================================================================================
        #region REAPER

        [CustomComboInfo("Slice Combo", "Replace Slice with its combo chain.", RPR.JobID, RPR.Slice, RPR.InfernalSlice)]
        ReaperSliceCombo = 1700,

        [CustomComboInfo("Scythe Combo", "Replace Spinning Scythe with its combo chain.", RPR.JobID, RPR.SpinningScythe, RPR.NightmareScythe)]
        ReaperScytheCombo = 1701,

        [CustomComboInfo("Enshroud Communio Feature", "Replace Enshroud with Communio when Enshrouded.", RPR.JobID, RPR.Enshroud)]
        ReaperEnshroudCommunioFeature = 1702,

        [CustomComboInfo("Gibbets and Gallows Feature", "Slice and Shadow of Death are replaced with Gibbet and Gallows while Soul Reaver or Shroud is active.", RPR.JobID, RPR.Slice, RPR.ShadowOfDeath)]
        ReaperGibbetGallowsFeature = 1703,

        [CustomComboInfo("Guillotine Feature", "Spinning Scythe's combo gets replaced with Guillotine while Soul Reaver or Shroud is active.", RPR.JobID, RPR.SpinningScythe)]
        ReaperGuillotineFeature = 1704,

        [CustomComboInfo("GG Gallows Option", "Slice now turns into Gallows when Gallows is Enhanced, and removes it from Shadow of Death.", RPR.JobID, RPR.Slice)]
        ReaperGibbetGallowsOption = 1705,

        [SecretCustomCombo]
        [CustomComboInfo("Combo Communio Feature", "When one stack is left of Shroud, Communio replaces Gibbet/Gallows/Guillotine.", RPR.JobID, RPR.Slice, RPR.InfernalSlice, RPR.Gibbet, RPR.Gallows, RPR.Guillotine, RPR.SpinningScythe, RPR.NightmareScythe)]
        ReaperComboCommunioFeature = 1706,

        [CustomComboInfo("Lemure Feature", "When you have two or more stacks of Void Shroud, Lemure Slice/Scythe replaces Gibbet/Gallows and Guillotine respectively.", RPR.JobID, RPR.Slice, RPR.InfernalSlice, RPR.Gibbet, RPR.Gallows, RPR.Guillotine, RPR.SpinningScythe, RPR.NightmareScythe)]
        ReaperLemureFeature = 1707,

        [CustomComboInfo("Arcane Circle Harvest Feature", "Replace Arcane Circle with Plentiful Harvest when you have stacks of Immortal Sacrifice.", RPR.JobID, RPR.ArcaneCircle)]
        ReaperHarvestFeature = 1708,

        [CustomComboInfo("Regress Feature", "Both Hell's Ingress and Hell's Egress turn into Regress when Threshold is active, instead of just the opposite of the one you used.", RPR.JobID, RPR.HellsIngress, RPR.HellsEgress)]
        ReaperRegressFeature = 1709,

        [CustomComboInfo("Shadow Of Death Feature", "Adds Shadow of Death to Main Combo if the debuff is not present or is about to expire", RPR.JobID, RPR.Slice, RPR.WaxingSlice, RPR.ShadowOfDeath)]
        ReaperShadowOfDeathFeature = 1710,

        [CustomComboInfo("Whorl Of Death Feature", "Adds Whorl of Death to Main AoE Combo if the debuff is not present or is about to expire", RPR.JobID, RPR.SpinningScythe, RPR.NightmareScythe)]
        ReaperWhorlOfDeathFeature = 1711,

        [CustomComboInfo("Blood Stalk / Grim Swathe Feature", "When Gluttony is off-cooldown, Blood Stalk and Grim Swathe will turn into Gluttony.", RPR.JobID, RPR.BloodStalk, RPR.GrimSwathe)]
        ReaperBloodSwatheFeature = 1712,

        #endregion
        // ====================================================================================
        #region SAGE

        [CustomComboInfo("Soteria into Kardia", "Soteria turns into Kardia when not active or Soteria is on-cooldown.", SGE.JobID, SGE.Soteria)]
        SageKardiaFeature = 1800,

        [CustomComboInfo("Phlegma into Dyskrasia", "Phlegma turns into Dyskrasia when you are out of charges.", SGE.JobID, SGE.Phlegma, SGE.Phlegmara, SGE.Phlegmaga)]
        SagePhlegmaFeature = 1801,

        [CustomComboInfo("Dosis Dps Feature", "Adds Eukrasia and Eukrasian dosis on one combo button (Currently only for Dosis1!!!)", SGE.JobID, SGE.Dosis1, SGE.Eukrasia, SGE.EukrasianDosis1)]
        SageDPSFeature = 1802,

        [ConflictingCombos(DoMSwiftcastFeature)]
        [CustomComboInfo("SGE Raise Feature", "Changes Swiftcast to Egeiro", SGE.JobID, SGE.Swiftcast, SGE.Egeiro)]
        SageEgeiroFeature = 1803,

        #endregion
        #region DISCIPLE OF MAGIC
        [ConflictingCombos(SchRaiseFeature, WHMRaiseFeature, AstrologianAscendFeature, SageEgeiroFeature)]
        [CustomComboInfo("Global Raise Feature", "Replaces Swiftcast with Raise/Resurrection/Verraise/Ascend/Egeiro when appropriate.", DoM.JobID, WHM.Raise, SMN.Resurrection, SCH.Resurrection, AST.Ascend, RDM.Verraise, SGE.Egeiro)]
        DoMSwiftcastFeature = 1900,

        #endregion
    }
}
