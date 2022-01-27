using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Combo presets.
    /// </summary>
    public enum CustomComboPreset
    {
        // ====================================================================================
        #region GLOBAL FEATURES
        [CustomComboInfo("Global Interrupt Feature", "Replaces Stun (LowBlow) with interrupt (Interject) when the target can be interrupted.", All.JobID, All.LowBlow , All.Interject)]
        InterruptFeature = 9000,
        [ConflictingCombos(SchRaiseFeature, WHMRaiseFeature, AstrologianAscendFeature, SageEgeiroFeature)]
        [CustomComboInfo("Global Raise Feature", "Replaces Swiftcast with Raise/Resurrection/Verraise/Ascend/Egeiro when appropriate.", All.JobID, WHM.Raise, SMN.Resurrection, SCH.Resurrection, AST.Ascend, RDM.Verraise, SGE.Egeiro)]
        DoMSwiftcastFeature = 9001,

        #endregion
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

        [ConflictingCombos(AstrologianAlternateDpsFeature)]
        [CustomComboInfo("DPS Feature(On Malefic)", "Adds Combust to the main malefic combo whenever the debuff is not present or about to expire", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1)]
        AstrologianDpsFeature = 5,

        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the DPS feature when below 8k mana", AST.JobID, AST.FallMalefic, AST.LucidDreaming)]
        AstrologianLucidFeature = 6,

        [CustomComboInfo("Astrodyne Feature", "Adds Astrodyne to the DPS feature when ready", AST.JobID, AST.FallMalefic, AST.LucidDreaming)]
        AstrologianAstrodyneFeature = 7,

        [CustomComboInfo("Aspected Helios Feature", "Replaces Aspected Helios whenever you are under Aspected Helios regen with Helios", AST.JobID, AST.AspectedHelios, AST.Helios)]
        AstrologianHeliosFeature = 8,

        [CustomComboInfo("Auto Card Draw", "Adds Auto Card Draw Onto Main DPS Feature", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1)]
        AstrologianAutoDrawFeature = 9,

        [CustomComboInfo("Auto Crown Card Draw", "Adds Auto Crown Card Draw Onto Main DPS Feature ", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1)]
        AstrologianAutoCrownDrawFeature = 10,

        [CustomComboInfo("AoE DPS Feature", "Adds AutoDraws/Astrodyne to the AoE Gravity combo", AST.JobID, AST.Gravity, AST.Gravity2)]
        AstrologianDpsAoEFeature = 11,

        [CustomComboInfo("LazyLordFeature", "Adds LordOfCrowns Onto Main DPS/AoE Feature", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1, AST.Gravity, AST.Gravity2)]
        AstrologianLazyLordFeature = 12,

        [CustomComboInfo("Astrodyne On Play", "Play becomes Astrodyne when you have 3 seals.", AST.JobID, AST.Play)]
        AstrologianAstrodyneOnPlayFeature = 13,

        [ConflictingCombos(AstrologianDpsFeature)]
        [CustomComboInfo("Alternate DPS Feature(On Combust)", "Adds Combust to the main malefic combo whenever the debuff is not present or about to expire", AST.JobID, AST.Combust1, AST.Combust2, AST.Combust3)]
        AstrologianAlternateDpsFeature = 14,

        [ConflictingCombos(AstrologianDpsFeature, AstrologianAlternateDpsFeature)]
        [CustomComboInfo("DPS Feature Custom Values Testing", "Same as DPSFeature(OnMalefic).Allows you to customize target MaxHp & CurrentPercentageHp & CurrentHp checks. Testing Only! ", AST.JobID, AST.FallMalefic, AST.Malefic4, AST.Malefic3, AST.Malefic2, AST.Malefic1)]
        CustomValuesTest = 15,

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
        #region BLUE MAGE

        [CustomComboInfo("Buffed Song of Torment", "Turns Song of Torment into Bristle so SoT is buffed.", BLU.JobID, BLU.SongOfTorment)]
        BluBuffedSoT = 1901,

        [CustomComboInfo("Moon Flute Opener", "Puts the Full Moon Flute Opener on Moon Flute or Whistle. \nSpells required: Whistle, Tingle, Moon Flute, J Kick, Triple Trident, Nightbloom, Rose of Destruction, Feather Rain, BRistle, Glass Dance, Surpanakha, Matra Magic, Shock Strike, Phantom Flurry.", BLU.JobID, BLU.MoonFlute, BLU.Whistle)]
        BluOpener = 1902,

        [CustomComboInfo("Final Sting Combo", "Turns Final Sting into the buff combo of: Moon Flute, Tingle, Whistle, Final Sting. Will use any primals off CD before casting Final Sting. \n Primals used: Feather Rain, Shock Strike, Glass Dance, J Kick, Rose of Destruction.", BLU.JobID, BLU.FinalSting)]
        BluFinalSting = 1903,

        [DependentCombos(BluFinalSting)]
        [CustomComboInfo("Off CD Primal Additions", "Adds any Primals that are off CD to the Final Sting Combo. ", BLU.JobID, BLU.FinalSting)]
        BluPrimals = 1904,

        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("Wanderer's into Pitch Perfect", "Replaces Wanderer's Minuet with Pitch Perfect while in WM.", BRD.JobID, BRD.WanderersMinuet)]
        BardWanderersPitchPerfectFeature = 200,

        [ConflictingCombos(SimpleBardFeature)]
        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 201,

        [DependentCombos(BardStraightShotUpgradeFeature)]
        [CustomComboInfo("Dot Maintenance Option", "Enabling this option will make Heavy Shot into Straight Shot refresh your dots on target when needed if there are any dots present.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardDoTMaintain = 202,

        [ConflictingCombos(BardIronJawsAlternateFeature)]
        [CustomComboInfo("Iron Jaws Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nAlternates between the two if Iron Jaws isn't available.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsFeature = 203,

        [ConflictingCombos(BardIronJawsFeature)]
        [CustomComboInfo("Iron Jaws Alternate Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nIron Jaws will only show up when debuffs are about to expire.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsAlternateFeature = 204,

        [ConflictingCombos(SimpleBardFeature)]
        [CustomComboInfo("Burst Shot/Quick Nock into Apex Arrow", "Replaces Burst Shot and Quick Nock with Apex Arrow when gauge is full and Blast Arrow when you are Blast Arrow ready.", BRD.JobID, BRD.BurstShot, BRD.QuickNock)]
        BardApexFeature = 205,

        [CustomComboInfo("Single Target oGCD Feature", "All oGCD's on Bloodletter (+ Songs rotation) depending on their CD.", BRD.JobID, BRD.BurstShot, BRD.Bloodletter)]
        BardoGCDSingleTargetFeature = 206,

        [CustomComboInfo("AoE oGCD Feature", "All AoE oGCD's on RainOfDeath depending on their CD.", BRD.JobID, BRD.BurstShot, BRD.RainOfDeath)]
        BardoGCDAoEFeature = 207,

        [ConflictingCombos(BardSimpleAoEFeature)]
        [CustomComboInfo("AoE Combo Feature", "Replaces QuickNock/Ladonsbite with Shadowbite when ready", BRD.JobID, BRD.QuickNock, BRD.Ladonsbite)]
        BardAoEComboFeature = 208,

        [CustomComboInfo("SimpleBard", "Adds every single target ability except DoTs to one button,\nIf there are DoTs on target SimpleBard will try to maintain their uptime.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        SimpleBardFeature = 209,

        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("SimpleBard DoT Option", "This option will make SimpleBard apply DoTs if none are present on the target.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        SimpleDoTOption = 210,

        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("SimpleBard Song Option", "This option adds the bards songs to the SimpleBard feature.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        SimpleSongOption = 211,

        [DependentCombos(BardoGCDAoEFeature)]
        [CustomComboInfo("Song Feature", "Adds Songs onto AoE oGCD Feature.", BRD.JobID, BRD.BurstShot, BRD.Bloodletter)]
        BardSongsFeature = 212,

        [CustomComboInfo("Bard Buffs Feature", "Adds RagingStrikes and BattleVoice onto Barrage.", BRD.JobID, BRD.Barrage)]
        BardBuffsFeature = 213,

        [CustomComboInfo("One Button Songs", "Add Mage's Ballade and Army's Paeon to Wanderer's Minuet depending on cooldowns", BRD.JobID, BRD.WanderersMinuet)]
        BardOneButtonSongs = 214,

        [CustomComboInfo("Simple AoE Bard", "Weaves oGCDs onto Quick Nock/Ladonsbite", BRD.JobID, BRD.QuickNock, BRD.Ladonsbite)]
        BardSimpleAoEFeature = 215,

        [DependentCombos(BardSimpleAoEFeature)]
        [CustomComboInfo("Simple AoE Bard Song Option", "Weave songs on the Simple AoE", BRD.JobID, BRD.QuickNock, BRD.Ladonsbite)]
        SimpleAoESongOption = 216,

        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("Simple Buffs Feature", "Adds buffs onto the SimpleBard feature.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardSimpleBuffsFeature = 217,

        [DependentCombos(SimpleBardFeature, BardSimpleBuffsFeature)]
        [CustomComboInfo("Simple Buffs - Radiant", "Adds Radiant Finale to the Simple Buffs feature.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardSimpleBuffsRadiantFeature = 218,

        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("Simple Raid Mode", "Removes enemy health checking on mobs for buffs, dots and songs.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardSimpleRaidMode = 219,

        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("Simple Interrupt", "Uses interrupt during simple bard rotation if applicable", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardSimpleInterrupt = 220,

        [CustomComboInfo("Disable ApexArrow From SimpleBard", "Removes Apex Arrow from SimpleBard and AoE Feature.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardRemoveApexArrowFeature = 221,

        [ConflictingCombos(BardoGCDSingleTargetFeature)]
        [DependentCombos(SimpleBardFeature)]
        [CustomComboInfo("Simple Opener", "BETA TESTING - Adds the optimum opener to simple bard.\nThis conflicts with pretty much everything outside of simple bard options due to the nature of the opener.", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardSimpleOpener = 222,

        #endregion
        // ====================================================================================
        #region DANCER

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing.", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 300,

        [ConflictingCombos(DancerDanceComboCompatibility, DancerDanceStepComboTest)]
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

        [CustomComboInfo("Overcap Feature", "Adds SaberBlade to Cascade/Windmil combo if you are about to overcap on esprit.", DNC.JobID, DNC.Cascade, DNC.Windmill)]
        DancerOvercapFeature = 307,

        [CustomComboInfo("Overcap Feature Option", "Adds SaberBlade to Cascade/Windmill if you have at least 50 esprit.", DNC.JobID, DNC.Cascade, DNC.Windmill)]
        DancerSaberDanceInstantSaberDanceComboFeature = 308,

        [ConflictingCombos(DancerDanceComboCompatibility, DancerDanceStepCombo)]
        [CustomComboInfo("CombinedDanceFeature", "Standard And Technical Dance on one button(StandardStep) Standard>Technical This is combo out into Tilana and StarfallDance.", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepComboTest = 309,

        [CustomComboInfo("FanSaberDanceFeature", "Adds SaberDance onto all FanDance Skills when no feathers are available and you have 50+ gauge", DNC.JobID, DNC.FanDance1, DNC.FanDance2, DNC.FanDance3, DNC.FanDance4)]
        DancerSaberFanDanceFeature = 310,

        [CustomComboInfo("FanDance On Cascade Feature", "Adds FanDance 1/3/4 Onto Cascade When available", DNC.JobID, DNC.Cascade, DNC.FanDance1, DNC.FanDance2, DNC.FanDance3, DNC.FanDance4)]
        DancerFanDanceOnMainComboFeature = 311,

        [CustomComboInfo("FanDance On Windmill Feature", "Adds FanDance 2/3/4 Onto Windmill When available", DNC.JobID, DNC.Windmill)]
        DancerFanDanceOnAoEComboFeature = 312,

        [DependentCombos(DancerDanceStepComboTest)]
        [CustomComboInfo("Devilment On Combined Dance Feature", "Adds Devilment right after Technical finish.", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDevilmentOnCombinedDanceFeature = 313,

        [DependentCombos(DancerDanceStepComboTest)]
        [CustomComboInfo("Flourish On Combined Dance Feature", "Adds Flourish to the CombinedStepCombo.", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerFlourishOnCombinedDanceFeature = 314,



        #endregion
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("Jump + Mirage Dive", "Replace (High) Jump with Mirage Dive when Dive Ready.", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 400,

        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain.", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 401,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain.", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 402,

        [ConflictingCombos(DragoonFullThrustComboPlus)]
        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain.", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 403,

        [ConflictingCombos(DragoonFullThrustCombo)]
        [CustomComboInfo("Full Thrust Combo Plus", "Replace Full Thrust with its combo chain (Disembowel/Chaosthrust/life surge added).", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustComboPlus = 404,

        [CustomComboInfo("Wheeling Thrust/Fang and Claw Option", "When you have either Enhanced Fang and Claw or Wheeling Thrust,\nChaos Thrust Combo becomes Wheeling Thrust and Full Thrust Combo becomes Fang and Claw.\nRequires Chaos Thrust Combo and Full Thrust Combo.", DRG.JobID, DRG.FullThrust, DRG.ChaosThrust)]
        DragoonFangThrustFeature = 405,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain.", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 500,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain.", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 501,

        [ConflictingCombos(DeliriumFeatureOption)]
        [CustomComboInfo("Delirium Feature", "Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active.", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 502,

        [CustomComboInfo("Dark Knight Gauge Overcap Feature", "Replace AoE combo with gauge spender if you are about to overcap.", DRK.JobID, DRK.StalwartSoul)]
        DRKOvercapFeature = 503,

        [CustomComboInfo("Living Shadow Feature", "Living shadow will now be on main combo if its not on CD and you have gauge for it.", DRK.JobID, DRK.LivingShadow)]
        DRKLivingShadowFeature = 504,

        [CustomComboInfo("EoS Overcap Feature", "Uses EoS if you are above 8.5k mana or DarkSide is about to expire(10sec or less)", DRK.JobID, DRK.EdgeOfShadow)]
        DarkManaOvercapFeature = 505,

        [CustomComboInfo("oGCD Feature", "Adds Living Shadow>Salted Earth>CarveAndSpit>SaltAndDarkness to CarveAndSpit and AbysalDrain", DRK.JobID, DRK.CarveAndSpit, DRK.AbyssalDrain)]
        DarkoGCDFeature = 506,

        [DependentCombos(DarkoGCDFeature)]
        [CustomComboInfo("Shadowbringer Feature", "Adds Shadowbringer to oGCD Feature ", DRK.JobID, DRK.CarveAndSpit, DRK.AbyssalDrain)]
        DarkShadowbringeroGCDFeature = 507,

        [ConflictingCombos(DarkPlungeFeatureOption)]
        [CustomComboInfo("Plunge Feature", "Adds Plunge onto main combo whenever its available (Uses all stacks).", DRK.JobID, DRK.Souleater)]
        DarkPlungeFeature = 508,

        [ConflictingCombos(DarkPlungeFeature)]
        [CustomComboInfo("Plunge Option", "Adds Plunge onto main combo whenever its available (Leaves 1 stack).", DRK.JobID, DRK.Souleater)]
        DarkPlungeFeatureOption = 509,

        [ConflictingCombos(DeliriumFeature)]
        [CustomComboInfo("Delirium Feature Option", "Replaces Souleather with Bloodspiller when Delirium has 10sec or less remaining.", DRK.JobID, DRK.Souleater)]
        DeliriumFeatureOption = 510,

        [CustomComboInfo("Unmend Uptime Feature", "Replace Souleater Combo Feature with Unmend when you are out of range.", DRK.JobID, DRK.Souleater)]
        DarkRangedUptimeFeature = 511,

        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain.", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 600,

        [DependentCombos(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Gnashing Fang and Continuation on Main Combo", "Adds Gnashing Fang to the main combo. Gnashing Fang must be started manually and the combo will finish it off.\n Useful for when Gnashing Fang needs to be help due to downtime.", GNB.JobID, GNB.DoubleDown, GNB.SolidBarrel)]
        GunbreakerGnashingFangOnMain = 601,

        [DependentCombos(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Sonic Break/Bow Shock/Blasting Zone on Main Combo", "Adds Sonic Break/Bow Shock/Blasting Zone on main combo when under No Mercy buff", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerCDsOnMainComboFeature = 602,

        [DependentCombos(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Double Down on Main Combo", "Adds Double Down on main combo when under No Mercy buff", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerDDonMain = 603,
        
        [DependentCombos(GunbreakerSolidBarrelCombo)]
        [ConflictingCombos(GunbreakerRoughDivide2StackOption)]
        [CustomComboInfo("Rough Divide Option (Leaves 1 Stack)", "Adds Rough Divide onto main combo whenever it's available (Leaves 1 stack).", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerRoughDivide1StackOption = 604,

        [ConflictingCombos(GunbreakerRoughDivide1StackOption)]
        [CustomComboInfo("Rough Divide Option (Uses all stacks)", "Adds Rough Divide onto main combo whenever its available (Uses all stacks).", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerRoughDivide2StackOption = 605,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 606,

        [CustomComboInfo("Ammo Overcap Feature", "Uses Burst Strike/Fated Circle on the respective ST/AoE combos when ammo is about to overcap.", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerAmmoOvercapFeature = 607,

        [ConflictingCombos(GunbreakerNoMercyRotationFeature)]
        [CustomComboInfo("Gnashing Fang Continuation Combo", "Adds Continuation to Gnashing Fang.", GNB.JobID, GNB.GnashingFang)]
        GunbreakerGnashingFangCombo = 608,

        [DependentCombos(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("No Mercy on Gnashing Fang", "Adds No Mercy to Gnashing Fang when it's ready.", GNB.JobID, GNB.BurstStrike, GNB.Hypervelocity)]
        GunbreakerNoMercyonGF = 613,
        
        [DependentCombos(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("Double Down on Gnashing Fang", "Adds Double Down to Gnashing Fang when No Mercy buff is up.", GNB.JobID, GNB.BurstStrike, GNB.Hypervelocity)]
        GunbreakerDDOnGF = 614,

        [DependentCombos(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("CDs on Gnashing Fang", "Adds Sonic Break/Bow Shock/Blasting Zone on Gnashing Fang, order dependent on No Mercy buff. \nBurst Strike added if there's charges while No Mercy buff is up.", GNB.JobID, GNB.BurstStrike, GNB.Hypervelocity)]
        GunbreakerCDsOnGF = 615,

        [CustomComboInfo("BurstStrikeContinuation", "Adds Hypervelocity on Burst Strike Continuation combo and main combo and Gnashing Fang.", GNB.JobID, GNB.BurstStrike, GNB.Hypervelocity)]
        GunbreakerBurstStrikeConFeature = 609,

        [CustomComboInfo("Burst Strike to Bloodfest Feature", "Replace Burst Strike with Bloodfest if you have no powder gauge.", GNB.JobID, GNB.BurstStrike)]
        GunbreakerBloodfestOvercapFeature = 610,

        [ConflictingCombos(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("No Mercy Rotation Feature", "Turns No Mercy into the the No Mercy Gnashing Fang Rotation when used. \nCurrently coded for the level 90 burst window.", GNB.JobID, GNB.NoMercy)]
        GunbreakerNoMercyRotationFeature = 611,

        [CustomComboInfo("Lightning Shot Uptime", "Replace Solid Barrel Combo Feature with Lightning Shot when you are out of range.", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerRangedUptimeFeature = 612,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain.", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot, MCH.SplitShot, MCH.HeatedSplitShot)]
        MachinistMainCombo = 700,

        [CustomComboInfo("Spread Shot/Scattergun Heat, +BioBlaster", "Spread Shot turns into Scattergun when lvl 82 or higher, Both turn into Auto Crossbow when overheated\nand Bioblaster is used first whenever it is off cooldown.", MCH.JobID, MCH.AutoCrossbow, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 701,

        [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active.", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 702,

        [CustomComboInfo("Gauss Round / Ricochet Feature", "Replace Gauss Round and Ricochet with one or the other depending on which has more charges.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistGaussRoundRicochetFeature = 703,

        [CustomComboInfo("Drill / Air Anchor (Hot Shot) Feature", "Replace Drill and Air Anchor (Hot Shot) with one or the other (or Chainsaw) depending on which is on cooldown.", MCH.JobID, MCH.Drill, MCH.HotShot, MCH.AirAnchor)]
        MachinistHotShotDrillChainsawFeature = 704,

        [DependentCombos(MachinistMainCombo)]
        [ConflictingCombos(MachinistAlternateMainCombo)]
        [CustomComboInfo("Drill/Air/Chain Saw Feature On Main Combo", "Air Anchor followed by Drill is added onto main combo if you use Reassemble.\nIf AirAnchor is on cooldown and you use Reassemble Chain Saw will be added to main combo instead.", MCH.JobID, MCH.Drill, MCH.AirAnchor, MCH.HotShot, MCH.Reassemble)]
        MachinistDrillAirOnMainCombo = 705,

        [CustomComboInfo("Single Button Heat Blast", "Switches Heat Blast to Hypercharge.", MCH.JobID, MCH.GaussRound, MCH.Ricochet, MCH.HeatBlast, MCH.Wildfire)]
        MachinistHeatblastGaussRicochetFeature = 706,

        [DependentCombos(MachinistMainCombo)]
        [ConflictingCombos(MachinistDrillAirOnMainCombo)]
        [CustomComboInfo("Alternate Drill/Air Feature on Main Combo", "Drill/Air/Hotshot Feature is added onto main combo (Note: It will add them onto main combo ONLY if you are under Reassemble Buff \nOr Reasemble is on CD(Will do nothing if Reassemble is OFF CD)", MCH.JobID, MCH.Drill, MCH.AirAnchor, MCH.HotShot, MCH.Reassemble)]
        MachinistAlternateMainCombo = 707,

        [DependentCombos(MachinistMainCombo)]
        [CustomComboInfo("Single Button HeatBlast On Main Combo Option", "Adds Single Button Heatblast onto the main combo when the option is enabled.", MCH.JobID, MCH.HeatBlast)]
        MachinistHeatBlastOnMainCombo = 708,

        [DependentCombos(MachinistMainCombo)]
        [CustomComboInfo("Battery Overcap Option", "Overcharge protection for your Battery, If you are at 100 battery charge rook/queen will be added to your (Heated) Shot Combo.", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverChargeOption = 709,

        [DependentCombos(MachinistSpreadShotFeature)]
        [CustomComboInfo("Battery AOE Overcap Option", "Adds overcharge protection to Spread Shot/Scattergun.", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistAoEOverChargeOption = 710,

        [DependentCombos(MachinistSpreadShotFeature)]
        [CustomComboInfo("Gauss Round Ricochet on AOE Feature", "Adds Gauss Round/Ricochet to the AoE combo during Hypercharge.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistAoEGaussRicochetFeature = 711,

        [DependentCombos(MachinistSpreadShotFeature)]
        [CustomComboInfo("Always Gauss Round/Ricochet on AoE Option", "Adds Gauss Round/Ricochet to the AoE combo outside of Hypercharge windows.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistAoEGaussOption = 712,

        [DependentCombos(MachinistMainCombo)]
        [CustomComboInfo("Ricochet & Gauss Round overcap protection option", "Adds Ricochet and Gauss Round to main combo. Will leave 1 charge of each.", MCH.JobID, MCH.GaussRound, MCH.Ricochet)]
        MachinistRicochetGaussMainCombo = 713,

        [DependentCombos(MachinistMainCombo)]
        [CustomComboInfo("Barrel Stabilizer drift protection feature", "Adds Barrel Stabilizer onto the main combo if heat is between 5-20.", MCH.JobID, MCH.BarrelStabilizer)]
        BarrelStabilizerDrift = 714,

        [DependentCombos(MachinistHeatblastGaussRicochetFeature)]
        [CustomComboInfo("WildFire Feature", "Adds WildFire to the Single Button Heat Blast Feature If Wildfire is off cooldown and you have enough heat for Hypercharge then Hypercharge will be replaced with Wildfire.\nAlso weaves Ricochet/Gauss Round on Heat Blast when necessary.", MCH.JobID, MCH.GaussRound, MCH.Ricochet, MCH.HeatBlast, MCH.Wildfire)]
        MachinistWildfireFeature = 715,

        [DependentCombos(MachinistSpreadShotFeature)]
        [CustomComboInfo("BioBlaster Feature", "Adds Bioblaster to the Spreadshot feature", MCH.JobID, MCH.AutoCrossbow, MCH.SpreadShot)]
        MachinistBioblasterFeature = 716,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("Monk AoE Combo", "Replaces ArmOfTheDestroyer/ShadowOfTheDestroyer with the AoE combo chain.", MNK.JobID, MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer, MNK.FourPointFury)]
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

        [CustomComboInfo("Monk Basic Rotation Plus", "Basic Monk Combo on one button Plus (Only for Testing)", MNK.JobID, MNK.Bootshine)]
        MnkBasicComboPlus = 806,

        [CustomComboInfo("Perfect Balance Feature Plus", "All of the (Optimal?) Blitz combos on Masterfull Bliz when Perfect Balance Is Active", MNK.JobID, MNK.PerfectBalance, MNK.MasterfulBlitz, MNK.ElixirField)]
        MnkPerfectBalancePlus = 807,

        [CustomComboInfo("MasterfullBliz To Main Combo", "Adds all of (Optimal?) Bliz combos and Masterfull Bliz on Main Combo", MNK.JobID, MNK.Bootshine)]
        MonkMasterfullBlizOnMainCombo = 808,

        [CustomComboInfo("MasterfullBliz To AoE Combo", "Adds all of (Optimal?) Bliz combos and Masterfull Bliz on AoE Combo.", MNK.JobID, MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer, MNK.FourPointFury)]
        MonkMasterfullBlizOnAoECombo = 809,

        [CustomComboInfo("Forbidden Chakra Feature", "Adds Forbidden Chakra/Enlightement to the Main/AoE feature combo. Testing Only for now!", MNK.JobID, MNK.Bootshine, MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
        MonkForbiddenChakraFeature = 810,


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

        [DependentCombos(NinjaHuraijinRaijuFeature)]
        [CustomComboInfo("Huraijin / Raiju Feature Option1", "Replaces Huraijin with Fleeting Raiju when available.", NIN.JobID, NIN.Huraijin, NIN.FleetingRaiju)]
        NinjaHuraijinRaijuFeature1 = 911,

        [DependentCombos(NinjaHuraijinRaijuFeature)]
        [CustomComboInfo("Huraijin / Raiju Feature Option2", "Replaces Huraijin with Forked Raiju when available.", NIN.JobID, NIN.Huraijin, NIN.ForkedRaiju)]
        NinjaHuraijinRaijuFeature2 = 912,

        [CustomComboInfo("Armor Crush Main Comb Combo", "Adds Armor Crush onto main combo.", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushOnMainCombo = 913,

        [DependentCombos(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("RaijuToMainComboFeature", "Adds Fleeting Raiju to Aeolian Edge Combo.", NIN.JobID, NIN.AeolianEdge)]
        NinjaFleetingRaijuFeature = 914,

        [DependentCombos(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("HuraijinToMainCombo", "Adds Huraijin to main combo if Huton buff is not present", NIN.JobID, NIN.AeolianEdge)]
        NinjaHuraijinFeature = 915,

        [CustomComboInfo("BunshinOnMainCombo", "Adds Bunshin whenever its off cd and you have gauge for it on main combo.", NIN.JobID, NIN.AeolianEdge)]
        NinjaBunshinFeature = 916,

        [CustomComboInfo("BavacakraOnMainCombo", "Adds Bavacakra you have gauge for it on main combo.", NIN.JobID, NIN.AeolianEdge)]
        NinjaBavacakraFeature = 917,

        [CustomComboInfo("Throwing Dagger Uptime Feature", "Replace Aeolian Edge with Throwing Daggers when targer is our of range.", NIN.JobID, NIN.AeolianEdge)]
        NinjaRangedUptimeFeature = 918,

        [CustomComboInfo("DreamWithinADream Feature", "Adds Assassinate/DwD onto main combo (Not optimal prob).", NIN.JobID, NIN.AeolianEdge)]
        NinjaDreamWithinADream = 919,

        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain.", PLD.JobID, PLD.GoringBlade, PLD.FastBlade, PLD.RiotBlade)]
        PaladinGoringBladeCombo = 1000,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain.", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinRoyalAuthorityCombo = 1001,

        [ConflictingCombos(PaladinAtonementTestFeature)]
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

        [CustomComboInfo("Royal Goring Option", "Insert Goring Blade into the main combo when appropriate.\nRequires Royal Authority Combo", PLD.JobID, PLD.RoyalAuthority, PLD.GoringBlade)]
        PaladinRoyalGoringOption = 1007,

        [CustomComboInfo("Standalone Holy Spirit Feature", "Replaces Holy Spirit with Confiteor and Confiteor combo", PLD.JobID, PLD.HolySpirit)]
        PaladinStandaloneHolySpiritFeature = 1008,

        [CustomComboInfo("Standalone Holy Circle Feature", "Replaces Holy Circle with Confiteor and Confiteor combo", PLD.JobID, PLD.HolyCircle)]
        PaladinStandaloneHolyCircleFeature = 1009,

        [ConflictingCombos(PaladinInterveneFeatureOption)]
        [CustomComboInfo("Intervene Feature", "Adds intervene onto main combo whenever its available (Uses all stacks).", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinInterveneFeature = 1010,

        [ConflictingCombos(PaladinInterveneFeature)]
        [CustomComboInfo("Intervene Option", "Adds intervene onto main combo whenever its available (Leaves 1 stack).", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinInterveneFeatureOption = 1011,

        [ConflictingCombos(PaladinRangedUptimeFeature2)]
        [CustomComboInfo("Shield Lob Uptime Feature", "Replace Royal Authority/Rage of Halone Feature with Shield Lob when out of range.", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinRangedUptimeFeature = 1012,

        [CustomComboInfo("FoF Feature", "Adds FoF onto the main combo (Testing).", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinFightOrFlightMainComboFeature = 1013,

        [CustomComboInfo("Req Feature", "Adds Req onto the main combo (Testing).", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinReqMainComboFeature = 1014,

        [ConflictingCombos(PaladinAtonementFeature)]
        [CustomComboInfo("Atonement Drop Feature", "Drops Atonement to prevent Potency loss (Testing).", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinAtonementTestFeature = 1015,

        [ConflictingCombos(PaladinRangedUptimeFeature)]
        [CustomComboInfo("Holyspirit Uptime Feature", "Replace Royal Authority/Rage of Halone Feature with HolySpirit when out of range.", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone, PLD.Confiteor)]
        PaladinRangedUptimeFeature2 = 1016,



        #endregion
        // ====================================================================================
        #region RED MAGE

        [ConflictingCombos(RedMageSmartcastAoECombo)]
        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/Verthunder 2 with Impact when Dualcast or Swiftcast are active.", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 1100,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules.", RDM.JobID, RDM.Redoublement, RDM.Zwerchhau, RDM.Riposte)]
        RedMageMeleeCombo = 1101,

        [CustomComboInfo("Redoublement Combo Plus", "Replaces Redoublement with Verflare/Verholy after Enchanted Redoublement, whichever is more appropriate.\nRequires Redoublement Combo.", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeComboPlus = 1102,

        [ConflictingCombos(RedMageSmartSingleTargetCombo)]
        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 1103,

        [ConflictingCombos(RedMageSmartSingleTargetCombo)]
        [CustomComboInfo("Verproc into Jolt Plus", "Additionally replaces Verstone/Verfire with Veraero/Verthunder if dualcast/swiftcast are up.\nRequires Verproc into Jolt.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocComboPlus = 1104,

        [ConflictingCombos(RedMageSmartSingleTargetCombo)]
        [CustomComboInfo("Verproc into Jolt Plus Opener Feature", "Turns Verfire into Verthunder when out of combat.\nRequires Verproc into Jolt Plus.", RDM.JobID, RDM.Verfire)]
        RedMageVerprocOpenerFeature = 1105,

        [CustomComboInfo("Resolution Feature", "Adds Resolution finisher to Verthunder/Verareo Combo ", RDM.JobID, RDM.Verstone, RDM.Verfire, RDM.Resolution)]
        RedmageResolutionFinisher = 1106,

        [CustomComboInfo("Resolution Feature Melee", "Adds Resolution finisher to melee combo ", RDM.JobID, RDM.Redoublement, RDM.Resolution)]
        RedmageResolutionFinisherMelee = 1107,

        [ConflictingCombos(RedMageAoECombo)]
        [CustomComboInfo("Smart AoE Feature", "Replaces Verthunder II With Veraero II and impact depending on mana", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageSmartcastAoECombo = 1108,

        [ConflictingCombos(RedMageVerprocComboPlus, RedMageVerprocOpenerFeature, RedMageVerprocCombo)]
        [CustomComboInfo("Smart Single Target Feature", "Smart Single target feature Credit: PrincessRTFM", RDM.JobID, RDM.Veraero, RDM.Verthunder, RDM.Verstone, RDM.Verfire)]
        RedMageSmartSingleTargetCombo = 1109,

        [CustomComboInfo("oGCD Feature", "Replace Contre Strike and Fleche with whichever is available soonest.", RDM.JobID, RDM.ContreSixte, RDM.Fleche)]
        RedMageOgcdCombo = 1110,

        [CustomComboInfo("SmartCast Opener Feature", "Verthunder Opener Feature. Allows you to prepull with verthunder and still let the combo balance the mana for you", RDM.JobID, RDM.Veraero, RDM.Verthunder, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocOpenerSmartCastFeature = 1111,

        [DependentCombos(RedMageSmartcastAoECombo)]
        [CustomComboInfo("Red Mage AoE Finisher", "Adds Finishers onto Moulinet and SmartCast AoE Feature.", RDM.JobID, RDM.EnchantedMoulinet, RDM.Moulinet)]
        RedMageMeleeAoECombo = 1112,

        [CustomComboInfo("Engagement Feature", "Adds Engagement in all melee combos. (Testing Only!)", RDM.JobID, RDM.EnchantedMoulinet, RDM.Moulinet, RDM.Redoublement, RDM.Zwerchhau, RDM.Riposte)]
        RedMageEngagementFeature = 1113,

        [CustomComboInfo("Simple Red Mage Single Target", "Combines Ranged and melee combo into one button(Veraero,Verthunder,Verstone,Verfire).\n (Testing Only!!! Disable Everything else other than SimpleFeatures!)", RDM.JobID, RDM.Veraero, RDM.Verthunder, RDM.Verstone, RDM.Verfire)]
        SimpleRedMage = 1114,

        [CustomComboInfo("Simple Red Mage AoE", "Combines Ranged and melee combo into one button(Veraero2,Verthunder2). \n (Testing Only!!! Disable Everything else other than SimpleFeatures!)", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        SimpleRedMageAoE = 1115,


        #endregion
        // ====================================================================================
        #region SAMURAI

        [ConflictingCombos(SamuraiSimpleSamuraiFeature)]
        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain.", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 1200,

        [ConflictingCombos(SamuraiSimpleSamuraiFeature)]
        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain.", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 1201,

        [ConflictingCombos(SamuraiSimpleSamuraiFeature)]
        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain.", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 1202,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain.", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 1203,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain.", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 1204,

        [CustomComboInfo("Jinpu/Shifu Feature", "Replace Meikyo Shisui with Jinpu or Shifu depending on what is needed.", SAM.JobID, SAM.MeikyoShisui)]
        SamuraiJinpuShifuFeature = 1205,

        [ConflictingCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Iaijutsu", "Replace Tsubame-gaeshi with Iaijutsu when Sen is empty.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiIaijutsuFeature = 1206,

        [ConflictingCombos(SamuraiIaijutsuShohaFeature)]
        [CustomComboInfo("Tsubame-gaeshi to Shoha", "Replace Tsubame-gaeshi with Shoha when meditation is 3.", SAM.JobID, SAM.TsubameGaeshi)]
        SamuraiTsubameGaeshiShohaFeature = 1207,

        [ConflictingCombos(SamuraiTsubameGaeshiIaijutsuFeature)]
        [CustomComboInfo("Iaijutsu to Tsubame-gaeshi", "Replace Iaijutsu with Tsubame-gaeshi when Sen is not empty.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuTsubameGaeshiFeature = 1208,

        [ConflictingCombos(SamuraiTsubameGaeshiShohaFeature)]
        [CustomComboInfo("Iaijutsu to Shoha", "Replace Iaijutsu with Shoha when meditation is 3.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiIaijutsuShohaFeature = 1209,

        [CustomComboInfo("Shinten to Senei", "Replace Hissatsu: Shinten with Senei when its cooldown is up.", SAM.JobID, SAM.Shinten)]
        SamuraiSeneiFeature = 1210,

        [CustomComboInfo("Shinten to Shoha", "Replace Hissatsu: Shinten with Shoha when Meditation is full.", SAM.JobID, SAM.Shinten)]
        SamuraiShohaFeature = 1211,

        [CustomComboInfo("Kyuten to Guren", "Replace Hissatsu: Kyuten with Guren when its cooldown is up.", SAM.JobID, SAM.Kyuten)]
        SamuraiGurenFeature = 1212,

        [CustomComboInfo("Kyuten to Shoha II", "Replace Hissatsu: Kyuten with Shoha II when Meditation is full.", SAM.JobID, SAM.Kyuten)]
        SamuraiShoha2Feature = 1213,

        [CustomComboInfo("Ikishoten Namikiri Feature", "Replace Ikishoten with Ogi Namikiri and then Kaeshi Namikiri when available.\nIf you have full Meditation stacks, Ikishoten becomes Shoha while you have Ogi Namikiri ready.", SAM.JobID, SAM.Ikishoten)]
        SamuraiIkishotenNamikiriFeature = 1214,

        [ConflictingCombos(SamuraiYukikazeCombo, SamuraiGekkoCombo, SamuraiKashaCombo)]
        [CustomComboInfo("SimpleSamuraiSingleTarget", "Every Sticker Combo On One Button (On Hakaze). Big Thanks to Stein121", SAM.JobID, SAM.Yukikaze, SAM.Shifu, SAM.Kasha, SAM.Hakaze)]
        SamuraiSimpleSamuraiFeature = 1215,

        [CustomComboInfo("SimpleSamuraiAoE", "Both AoE Combos on same button (On Oka). Big thanks to Stein121", SAM.JobID, SAM.Mangetsu, SAM.Oka)]
        SamuraiSimpleSamuraiAoECombo = 1216,

        [DependentCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("KaitenFeature Option 1", "Adds Kaiten to Higanbana when it has < 5 seconds remaining.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiKaitenFeature1 = 1218,

        [DependentCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("KaitenFeature Option 2", "Adds Kaiten to Tenka Goken.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiKaitenFeature2 = 1219,

        [DependentCombos(SamuraiIaijutsuTsubameGaeshiFeature)]
        [CustomComboInfo("KaitenFeature Option 3", "Adds Kaiten to Midare Setsugekka.", SAM.JobID, SAM.Iaijutsu)]
        SamuraiKaitenFeature3 = 1220,

        [CustomComboInfo("Gyoten Feature", "Hissatsu: Gyoten becomes Yaten/Gyoten depending on the distance from your target.", SAM.JobID, SAM.Yaten, SAM.Gyoten)]
        SamuraiYatenFeature = 1221,

        [CustomComboInfo("KaitenFeature Option 4", "Adds Kaiten when above 20 gauge to OgiNamikiri and OgiNamikiri is ready.", SAM.JobID, SAM.OgiNamikiri)]
        SamuraiOgiNamikiriFeature = 1222,

        [ConflictingCombos(SamuraiOvercapFeature85)]
        [CustomComboInfo("SimpleSamurai Overcap Feature 1", "Adds Senei>Shinten onto main combo at 75 or more Kenki", SAM.JobID, SAM.Yukikaze, SAM.Shifu, SAM.Kasha, SAM.Hakaze)]
        SamuraiOvercapFeature75 = 1223,

        [ConflictingCombos(SamuraiOvercapFeature75)]
        [CustomComboInfo("SimpleSamurai Overcap Feature 2", "Adds Senei>Shinten onto main combo at 85 or more Kenki", SAM.JobID, SAM.Yukikaze, SAM.Shifu, SAM.Kasha, SAM.Hakaze)]
        SamuraiOvercapFeature85 = 1224,



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

        [CustomComboInfo("SCH Alternate DPS Feature", "Adds Biolysis on Ruin II. Won't work below level 38", SCH.JobID, SCH.Ruin2)]
        SCHDPSAlternateFeature = 1303,

        [CustomComboInfo("Fairy Feature", "Change every action that requires a fairy into Summon Eos if you do not have a fairy summoned.", SCH.JobID, SCH.WhisperingDawn, SCH.FeyIllumination, SCH.FeyBless, SCH.Aetherpact, SCH.Dissipation, SCH.SummonSeraph, SCH.Consolation)]
        ScholarFairyFeature = 1304,

        [CustomComboInfo("DPS Feature", "Adds Bio1/Bio2/Biosys to Broil/Ruin whenever the debuff is not present or about to expire.", SCH.JobID, SCH.JobID, SCH.Broil4, SCH.Broil3, SCH.Broil2, SCH.Broil1, SCH.Ruin1)]
        ScholarDPSFeature = 1305,

        [CustomComboInfo("DPS Feature Buff Option", "Adds Chainstratagem to the DPS Feature.", SCH.JobID, SCH.JobID, SCH.Broil4, SCH.Broil3, SCH.Broil2, SCH.Broil1, SCH.Ruin1)]
        ScholarDPSFeatureBuffOption = 1306,

        [CustomComboInfo("DPS Feature Lucid Dreaming Option", "Adds Lucid dreaming to the DPS feature when below 8k mana.", SCH.JobID, SCH.Broil4, SCH.Broil3, SCH.Broil2, SCH.Broil1, SCH.Ruin1)]
        ScholarLucidDPSFeature = 1307,

        #endregion
        // ====================================================================================
        #region SUMMONER

        [ConflictingCombos(SummonerMainComboFeatureRuin1)]
        [CustomComboInfo("Enable Single Target (RuinIII)", "Enables changing Single-Target Combo (Ruin III).", SMN.JobID, SMN.Ruin3, SMN.Deathflare)]
        SummonerMainComboFeature = 1400,

        [CustomComboInfo("Enable AOE", "Enables changing AOE Combo (Tri-Disaster)", SMN.JobID, SMN.Tridisaster, SMN.Deathflare)]
        SummonerAOEComboFeature = 1401,

        [DependentCombos(SummonerMainComboFeature)]
        [CustomComboInfo("Single Target Demi Feature", "Replaces Astral Impulse/Fountain of Fire with Enkindle/Deathflare/Rekindle when appropriate.", SMN.JobID, SMN.Ruin3)]
        SummonerSingleTargetDemiFeature = 1402,

        [DependentCombos(SummonerAOEComboFeature)]
        [CustomComboInfo("AOE Demi Feature", "Replaces Astral Flare/Brand of Purgatory with Enkindle/Deathflare/Rekindle when appropriate.", SMN.JobID, SMN.Ruin3)]
        SummonerAOEDemiFeature = 1403,
        
        [CustomComboInfo("Egi Attacks Feature", "Replaces RuinI/Ruin III (Depending On Enabeled Combo) and Tri-Disaster with Egi attacks. Will not work without enabling Single Target and/or AOE.", SMN.JobID, SMN.Fester, SMN.EnergyDrain, SMN.Ruin4)]
        SummonerEgiAttacksFeature = 1404,

        [CustomComboInfo("Garuda Slipstream Feature", "Adds Slipstream on RuinI/Ruin III/Tri-disaster.", SMN.JobID, SMN.Ruin3)]
        SummonerGarudaUniqueFeature = 1405,

        [CustomComboInfo("Ifrit Cyclone Feature", "Adds Crimson Cyclone/Crimson Strike on RuinI/Ruin III/Tri-disaster.", SMN.JobID, SMN.Ruin3)]
        SummonerIfritUniqueFeature = 1406,

        [CustomComboInfo("Titan Mountain Buster Feature", "Adds Mountain Buster on RuinI/Ruin III/Tri-disaster.", SMN.JobID, SMN.Ruin3)]
        SummonerTitanUniqueFeature = 1407,

        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when our of Aetherflow stacks.", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 1408,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Siphon when out of Aetherflow stacks.", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 1409,

        // BONUS TWEAKS
        [DependentCombos(SummonerMainComboFeature)]
        [CustomComboInfo("Carbuncle Reminder Feature", "Reminds you always to summon Carbuncle by replacing Ruin (Carbuncle Summon Reminder Feature).", SMN.JobID, SMN.SummonCarbuncle, SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
        SummonerCarbuncleSummonFeature = 1410,

        [CustomComboInfo("Ruin 4 On Ruin3 Combo Feature", "Adds Ruin4 on main RuinI/RuinIII combo feature when there are currently no summons being active.", SMN.JobID, SMN.Ruin, SMN.Ruin2, SMN.Ruin3, SMN.Ruin4)]
        SummonerRuin4ToRuin3Feature = 1411,

        [CustomComboInfo("Ruin 4 On Tri-disaster Feature", "Adds Ruin4 on main Tridisaster combo feature when there are currently no summons being active.", SMN.JobID, SMN.Tridisaster)]
        SummonerRuin4ToTridisasterFeature = 1412,

        [DependentCombos(SummonerEDFesterCombo, SummonerESPainflareCombo)]
        [CustomComboInfo("Ruin IV Fester/PainFlare Feature", "Change Fester/PainFlare into Ruin IV when out of Aetherflow stacks, ED/ES is on cooldown, and Ruin IV is up.", SMN.JobID, SMN.Painflare, SMN.Fester)]
        SummonerFesterPainflareRuinFeature = 1413,

        [CustomComboInfo("Lazy Fester Feature", "Adds Fester during GCDs of most skills (Ruin3/Ruin4/AstralImpulse/FountainOfFire).\nKeep in mind that for optimal fester usage you should only use it when you have Searing Light, and not every time it comes up.", SMN.JobID, SMN.Ruin3, SMN.Ruin4, SMN.AstralImpulse, SMN.FountainOfFire)]
        SummonerLazyFesterFeature = 1414,

        [ConflictingCombos(SimpleSummonerOption2)]
        [CustomComboInfo("One Button Rotation Feature", "Summoner Single Target One Button Rotation (Single Target) on Ruin1/Ruin3.(Titan>Garuda>Ifrit) ", SMN.JobID, SMN.Ruin3, SMN.Deathflare)]
        SimpleSummoner = 1415,

        [CustomComboInfo("One Button AoE Rotation Feature", "Summoner AoE One Button Rotation (AoE) on Tridisaster", SMN.JobID, SMN.Tridisaster, SMN.Deathflare)]
        SimpleAoESummoner = 1416,

        [DependentCombos(SimpleSummoner)] 
        [CustomComboInfo("Searing Light Rotation Option", "Adds Searing Light to Simple Summoner Rotation, Single Target", SMN.JobID, SMN.Ruin3, SMN.SearingLight)]
        BuffOnSimpleSummoner = 1417,

        [DependentCombos(SimpleAoESummoner)]
        [CustomComboInfo("Searing Light  AoE Option", "Adds Searing Light to Simple Summoner Rotation, AoE", SMN.JobID, SMN.Tridisaster, SMN.SearingLight)]
        BuffOnSimpleAoESummoner = 1418,

        [CustomComboInfo("DemiReminderFeature", "Adds Only Demi Summons on RuinIII (So you can still choose your Egis but never forget to summon Demis) ", SMN.JobID, SMN.Ruin3, SMN.Deathflare)]
        SummonerDemiSummonsFeature = 1419,

        [CustomComboInfo("DemiReminderAoEFeature", "Adds Only Demi Summons on TriDisaster (So you can still choose your Egis but never forget to summon Demis) ", SMN.JobID, SMN.Ruin3, SMN.Deathflare)]
        SummonerDemiAoESummonsFeature = 1420,

        [CustomComboInfo("Ruin III mobility", "Allows you to cast Ruin III while Ruin IV is unavailable for mobility reasons. Shows up as Ruin I.\nWill break combos with Ruin I. Might break combos with Ruin IV.", SMN.JobID, SMN.Ruin4)]
        SummonerRuinIVMobilityFeature = 1421,

        [CustomComboInfo("Swiftcast Garuda Option", "Always swiftcasts Slipstream if available.", SMN.JobID, SMN.Ruin3)]
        SummonerSwiftcastFeatureGaruda = 1422,

        [CustomComboInfo("Swiftcast Ifrit Option", "Always swiftcasts 2nd Ruby Rite if available.", SMN.JobID, SMN.Ruin3)]
        SummonerSwiftcastFeatureIfrit = 1423,

        [ConflictingCombos(SimpleSummoner)]
        [CustomComboInfo("One Button Rotation Feature Option2 ", "Same feature as One Button Rotation Feature but Garua>Titan>Ifrit .", SMN.JobID, SMN.Ruin3)]
        SimpleSummonerOption2 = 1424,

        [CustomComboInfo("Prevent Ruin4 Waste Feature", "Puts Ruin4 Above anything if FurtherRuin about to expire and there is no Demi present.", SMN.JobID, SMN.Ruin3)]
        SummonerRuin4WastePrevention = 1425,

        [ConflictingCombos(SummonerMainComboFeature)]
        [CustomComboInfo("Enable Single Target (Ruin1)", "Enables changing Single-Target Combo (Ruin I).", SMN.JobID, SMN.Ruin)]
        SummonerMainComboFeatureRuin1 = 1426,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "All in one main combo feature adds Storm's Eye/Path", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 1500,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 1501,

        [CustomComboInfo("Overpower Combo", "Add combos to Overpower", WAR.JobID, WAR.MythrilTempest, WAR.Overpower)]
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

        [CustomComboInfo("Inner Chaos option", "Adds Inner Chaos to Storms Path Combo and Chaotic Cyclone to Overpower Combo if you are buffed with Nascent Chaos and Surging Tempest.\nRequires Storms Path Combo and Overpower Combo", WAR.JobID, WAR.InnerChaos, WAR.StormsPath)]
        WarriorInnerChaosOption = 1510,

        [CustomComboInfo("Fell Cleave/Decimate Option", "Adds Fell Cleave to main combo when gauge is at 50 or more and adds Decimate to the AoE combo", WAR.JobID, WAR.StormsPath)]
        WarriorSpenderOption = 1511,

        [ConflictingCombos(WarriorOnslaughtFeatureOption, WarriorOnslaughtFeatureOptionTwo)]
        [CustomComboInfo("Onslaught Feature", "Adds Onslaught to Storm's Path feature combo if you are under Surging Tempest Buff (Uses all stacks)", WAR.JobID, WAR.StormsPath)]
        WarriorOnslaughtFeature = 1512,

        [ConflictingCombos(WarriorOnslaughtFeature, WarriorOnslaughtFeatureOptionTwo)]
        [CustomComboInfo("Onslaught Option", "Adds Onslaught to Storm's Path feature combo if you are under Surging Tempest Buff (Leaves 1 stack)", WAR.JobID, WAR.StormsPath)]
        WarriorOnslaughtFeatureOption = 1513,

        [ConflictingCombos(WarriorOnslaughtFeature, WarriorOnslaughtFeatureOption)]
        [CustomComboInfo("Onslaught Option Two", "Adds Onslaught to Storm's Path feature combo if you are under Surging Tempest Buff (Leaves 1/2 stacks depending on lvl)", WAR.JobID, WAR.StormsPath)]
        WarriorOnslaughtFeatureOptionTwo = 1514,

        [CustomComboInfo("Infuriate Feature", "Replaces Infuriate with FellCleave when under InnerRelease buff. Replaces Infuriate with Inner Chaos When under Nascent Chaos buff", WAR.JobID, WAR.Infuriate)]
        WarriorInfuriateFeature = 1515,

        [CustomComboInfo("Tomahawk Uptime Feature", "Replace Storm's Path Combo Feature with Tomahawk when you are out of range.", WAR.JobID, WAR.StormsPath)]
        WARRangedUptimeFeature = 1516,

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

        [CustomComboInfo("DoT on Glare3 Feature", "Adds DoT on Glare3 when DoT is not preset on about to expire and when you are inCombat (You can still prepull Glare)", WHM.JobID, WHM.Glare3, WHM.Dia)]
        WHMDotMainComboFeature = 1605,

        [DependentCombos(WHMDotMainComboFeature)]
        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming onto Glare1/3 Feature combo when you are below 8k mana", WHM.JobID, WHM.LucidDreaming)]
        WHMLucidDreamingFeature = 1606,

        [CustomComboInfo("Medica Feature", "Replaces Medica2 whenever you are under Medica2 regen with Medica1", WHM.JobID, WHM.Medica1, WHM.Medica2)]
        WHMMedicaFeature = 1607,

        [DependentCombos(WHMDotMainComboFeature)]
        [CustomComboInfo("Presence Of Mind Feature", "Adds Presence of mind as oGCD onto main DPS Feature(Glare3)", WHM.JobID, WHM.Glare3)]
        WHMPresenceOfMindFeature = 1608,

        [DependentCombos(WHMDotMainComboFeature)]
        [CustomComboInfo("Assize Feature", "Adds Assize as oGCD onto main DPS Feature(Glare3)", WHM.JobID, WHM.Glare3)]
        WHMAssizeFeature = 1609,

        [DependentCombos(WHMMedicaFeature)]
        [CustomComboInfo("Afflatus Misery On Medica Feature", "Adds Afflatus Misery onto the Medica Feature", WHM.JobID, WHM.Medica1, WHM.Medica2)]
        WhiteMageAfflatusMiseryMedicaFeature = 1610,

        [DependentCombos(WHMMedicaFeature)]
        [CustomComboInfo("Afflatus Rapture On Medica Feature", "Adds Afflatus Rapture onto the Medica Feature", WHM.JobID, WHM.Medica1, WHM.Medica2)]
        WhiteMageAfflatusRaptureMedicaFeature = 1611,

        [CustomComboInfo("Afflatus Misery Feature", "Changes Cure 2 into Afflatus Misery.", WHM.JobID, WHM.Cure2)]
        WhiteMageAfflatusMiseryCure2Feature = 1612,

        [CustomComboInfo("Remove DoT From Glare3 Feature", "Removes DoT from DPS feature", WHM.JobID, WHM.Glare3, WHM.Dia)]
        WHMRemoveDotFromDPSFeature = 1613,

        [DependentCombos(WHMRaiseFeature)]
        [CustomComboInfo("Thin Air Raise Feature", "Adds Thin Air to the WHM Raise Feature", WHM.JobID, WHM.Swiftcast, WHM.Raise)]
        WHMThinAirFeature = 1614,

        #endregion
        // ====================================================================================
        #region REAPER

        [CustomComboInfo("Slice Combo", "Replace Slice with its combo chain.", RPR.JobID, RPR.Slice, RPR.InfernalSlice)]
        ReaperSliceCombo = 1700,

        [CustomComboInfo("Scythe Combo", "Replace Spinning Scythe with its combo chain.", RPR.JobID, RPR.SpinningScythe, RPR.NightmareScythe)]
        ReaperScytheCombo = 1701,

        [CustomComboInfo("Enshroud Communio Feature", "Replace Enshroud with Communio when Enshrouded.", RPR.JobID, RPR.Enshroud)]
        ReaperEnshroudCommunioFeature = 1702,

        [ConflictingCombos(ReaperGibbetGallowsFeatureOption)]
        [CustomComboInfo("Gallows And Gibbet Option", "Slice and Shadow of Death are replaced with Gibbet and Gallows while Soul Reaver or Shroud is active.(Swapped positional skills from ReaperGibbetGallowsFeatureOption)", RPR.JobID, RPR.Slice, RPR.ShadowOfDeath)]
        ReaperGibbetGallowsFeature = 1703,

        [CustomComboInfo("Guillotine Feature", "Spinning Scythe's combo gets replaced with Guillotine while Soul Reaver or Shroud is active.", RPR.JobID, RPR.SpinningScythe)]
        ReaperGuillotineFeature = 1704,

        [CustomComboInfo("GG Gallows Option", "Slice now turns into Gallows when Gallows is Enhanced, and removes it from Shadow of Death.", RPR.JobID, RPR.Slice)]
        ReaperGibbetGallowsOption = 1705,

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

        [ConflictingCombos(ReaperBloodSwatheFeature, ReaperBloodStalkAlternateComboOption)]
        [CustomComboInfo("Blood Stalk Combo Option", "Turns Blood Stalk into Gluttony when off-cooldown and puts Gibbets and Gallows on the same button as Blood Stalk. Adds Enshrouded Combo to button as well", RPR.JobID, RPR.BloodStalk)]
        ReaperBloodSwatheComboFeature = 1713,

        [ConflictingCombos(ReaperBloodSwatheFeature)]
        [CustomComboInfo("Grim Swathe Combo Option", "Turns Grim Swathe into Gluttony when off-cooldown and puts Guillotine on the same button as Grim Swathe. Adds Enshrouded Combo to button as well", RPR.JobID, RPR.GrimSwathe)]
        ReaperGrimSwatheComboOption = 1714,

        [CustomComboInfo("Cross/Void Reaping Feature", "Turns Enshroud into Cross/Void reaping with Lemure Slice as oGCD after Cross Reaping.", RPR.JobID, RPR.Enshroud)]
        ReaperVoidCrossReapingComboOption = 1715,

        [ConflictingCombos(ReaperGibbetGallowsFeature)]
        [CustomComboInfo("Gibbets and Gallows Feature Option", "Slice and Shadow of Death are replaced with Gibbet and Gallows while Soul Reaver or Shroud is active.", RPR.JobID, RPR.Slice, RPR.ShadowOfDeath)]
        ReaperGibbetGallowsFeatureOption = 1716,

        [ConflictingCombos(ReaperBloodSwatheComboFeature)]
        [CustomComboInfo("Blood Stalk Combo Option Alternative", "Turns Blood Stalk into Gluttony when off-cooldown and puts Gibbets and Gallows on the same button as Blood Stalk. Adds Enshrouded Combo to button as well", RPR.JobID, RPR.BloodStalk)]
        ReaperBloodStalkAlternateComboOption = 1717,



        #endregion
        // ====================================================================================
        #region SAGE

        [CustomComboInfo("Soteria into Kardia", "Soteria turns into Kardia when not active or Soteria is on-cooldown.", SGE.JobID, SGE.Soteria)]
        SageKardiaFeature = 1800,

        [CustomComboInfo("Phlegma into Dyskrasia", "Phlegma turns into Dyskrasia when you are out of charges.", SGE.JobID, SGE.Phlegma, SGE.Phlegmara, SGE.Phlegmaga)]
        SagePhlegmaFeature = 1801,

        [ConflictingCombos(SageDPSFeatureTest)]
        [CustomComboInfo("Dosis Dps Feature", "Adds Eukrasia and Eukrasian dosis on one combo button", SGE.JobID, SGE.Dosis1, SGE.Dosis2, SGE.Dosis3, SGE.Eukrasia, SGE.EukrasianDosis1)]
        SageDPSFeature = 1802,

        [ConflictingCombos(DoMSwiftcastFeature)]
        [CustomComboInfo("SGE Raise Feature", "Changes Swiftcast to Egeiro", SGE.JobID, SGE.Swiftcast, SGE.Egeiro)]
        SageEgeiroFeature = 1803,

        [CustomComboInfo("Lucid Dreaming Feature", "Adds LucidDreaming onto Dosis DPS Feature when you have 8k mana or less", SGE.JobID, SGE.Dosis1, SGE.Dosis2, SGE.Dosis3, SGE.Eukrasia, SGE.EukrasianDosis1)]
        SageLucidFeature = 1804,

        [ConflictingCombos(SageDPSFeature)]
        [CustomComboInfo("Dosis DPS Feature Testing", "Same function as Above DPS Feature but you can input some values to your liking ", SGE.JobID, SGE.Dosis1, SGE.Dosis2, SGE.Dosis3, SGE.Eukrasia, SGE.EukrasianDosis1)]
        SageDPSFeatureTest = 1805,

        #endregion
        // ====================================================================================
        #region PvP Combos

        [SecretCustomCombo]
        [CustomComboInfo("BurstShotFeature", "Adds Shadowbite/EmpyArrow/PitchPerfect(3stacks)/SideWinder(When Target is low hp)/ApexArrow when gauge is 100 all on one button combo.", BRDPvP.JobID, BRDPvP.BurstShot)]
        BurstShotFeaturePVP = 3500,

        [SecretCustomCombo]
        [CustomComboInfo("SongsFeature", "Replaces WanderersMinnuet and Peons song all on one button in an optimal order", BRDPvP.JobID, BRDPvP.TheWanderersMinuet)]
        SongsFeaturePVP = 3501,

        [SecretCustomCombo]
        [CustomComboInfo("SouleaterComboFeature", "Adds EoS as oGCD onto main combo and Bloodspiller when at 50 gauge or under delirium buff.", DRKPVP.JobID, DRKPVP.EdgeOfShadow, DRKPVP.SoulEaterCombo, DRKPVP.HardSlash, DRKPVP.SyphonStrike, DRKPVP.SoulEater)]
        SouleaterComboFeature = 3502,

        [SecretCustomCombo]
        [CustomComboInfo("StalwartSoulComboFeature", "Adds FoS as oGCD onto main combo and Quietus when at 50 gauge or under delirium buff.", DRKPVP.JobID, DRKPVP.Unleash, DRKPVP.StalwartSoul)]
        StalwartSoulComboFeature = 3503,

        [SecretCustomCombo]
        [CustomComboInfo("StormsPathComboFeature", "Replaces Storm's Path Combo with FellCleave/IC when at 50 gauge or under IR", WARPVP.JobID, WARPVP.HeavySwing, WARPVP.Maim, WARPVP.StormsPath)]
        StormsPathComboFeature = 3504,

        [SecretCustomCombo]
        [CustomComboInfo("SteelCycloneFeature", "Replaces Steel Cyclone Combo with Decimate/CC when at 50 gauge or under IR", WARPVP.JobID, WARPVP.SteelCyclone, WARPVP.MythrilTempest)]
        SteelCycloneFeature = 3505,

        [SecretCustomCombo]
        [CustomComboInfo("RoyalAuthorityComboFeature", "Adds HolySpirit To the main combo", PLDPVP.JobID, PLDPVP.FastBlade, PLDPVP.RiotBlade, PLDPVP.RoyalAuthority )]
        RoyalAuthorityComboFeature = 3506,

        [SecretCustomCombo]
        [CustomComboInfo("ProminenceComboFeature", "Adds HolyCircle to the main AoE Combo", PLDPVP.JobID, PLDPVP.Prominence, PLDPVP.TotalEclipse)]
        ProminenceComboFeature = 3507,

        [SecretCustomCombo]
        [CustomComboInfo("GnashingFangComboFeature", "Adds BowShock(When target is meleeRange) and Burststrike at 2 ammo gauge to the main combo", GNBPVP.JobID, GNBPVP.KeenEdge, GNBPVP.BrutalShell, GNBPVP.SolidBarrel)]
        SolidBarrelComboFeature = 3508,

        [SecretCustomCombo]
        [CustomComboInfo("DemonSlaughterComboFeature", "Adds BowShock(When target is meleeRange) and Fated Circle at 2 ammo gauge to the main AoE combo", GNBPVP.JobID, GNBPVP.DemonSlaughter, GNBPVP.DemonSlice)]
        DemonSlaughterComboFeature = 3509,

        [SecretCustomCombo]
        [CustomComboInfo("HeatedCleanShotFeature", "Adds Gauss/Rico weave to maincombo", MCHPVP.JobID, MCHPVP.SplitShot, MCHPVP.SlugShot, MCHPVP.CleanShot)]
        HeatedCleanShotFeature = 3510,

        [SecretCustomCombo]
        [CustomComboInfo("WildfireBlankFeature", "Adds Blank To Wildfire if you are in melee Range", MCHPVP.JobID, MCHPVP.Wildfire)]
        WildfireBlankFeature = 3511,

        [SecretCustomCombo]
        [CustomComboInfo("InfernalSliceComboFeature", "Adds Gluttony/BloodStalk/Smite/EnshroudComboRotation on InfernalSliceCombo", RPRPVP.JobID, RPRPVP.Slice, RPRPVP.WaxingSlice, RPRPVP.InfernalSlice)]
        InfernalSliceComboFeature = 3512,

        [SecretCustomCombo]
        [CustomComboInfo("NightmareScytheComboFeature", "Adds Gluttony/GrimSwathe/Smite/EnshroudComboRotation on InfernalScytheCombo", RPRPVP.JobID, RPRPVP.SpinningScythe, RPRPVP.NightmareScythe, RPRPVP.GrimReaping)]
        NightmareScytheComboFeature = 3513,

        [SecretCustomCombo]
        [CustomComboInfo("NinjaAeolianEdgePvpCombo", "Adds Cha/Assassinate/Smite on AeolianEdge combo", NINPVP.JobID, NINPVP.SpinningEdge, NINPVP.GustSlash, NINPVP.AeolianEdge)]
        NinjaAeolianEdgePvpCombo = 3514,

        [SecretCustomCombo]
        [CustomComboInfo("MnkBootshinePvPFeature", "Adds Axekick/Smite/TornadoKick on main combo", MNKPVP.JobID, MNKPVP.Bootshine, MNKPVP.TrueStrike, MNKPVP.SnapPunch)]
        MnkBootshinePvPFeature = 3515,

        [SecretCustomCombo]
        [CustomComboInfo("BlackEnochianPVPFeature", "Enochian Stance Switcher", BLMPVP.JobID, BLMPVP.Enochian)]
        BlackEnochianPVPFeature = 3516,


        #endregion
    }
}
