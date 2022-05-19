using XIVSlothComboPlugin.Attributes;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Combo presets.
    /// </summary>
    public enum CustomComboPreset
    {
        // ====================================================================================
        #region Misc

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", ADV.JobID)]
        AdvAny = 0,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", AST.JobID)]
        AstAny = AdvAny + AST.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", BLM.JobID)]
        BlmAny = AdvAny + BLM.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", BRD.JobID)]
        BrdAny = AdvAny + BRD.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", DNC.JobID)]
        DncAny = AdvAny + DNC.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", DOH.JobID)]
        DohAny = AdvAny + DOH.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", DoL.JobID)]
        FSH_Cast = AdvAny + DoL.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", DRG.JobID)]
        DrgAny = AdvAny + DRG.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", DRK.JobID)]
        DrkAny = AdvAny + DRK.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", GNB.JobID)]
        GnbAny = AdvAny + GNB.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", MCH.JobID)]
        MchAny = AdvAny + MCH.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", MNK.JobID)]
        MnkAny = AdvAny + MNK.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", NIN.JobID)]
        NinAny = AdvAny + NIN.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", PLD.JobID)]
        PldAny = AdvAny + PLD.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", RDM.JobID)]
        RdmAny = AdvAny + RDM.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", RPR.JobID)]
        RprAny = AdvAny + RPR.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", SAM.JobID)]
        SamAny = AdvAny + SAM.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", SCH.JobID)]
        SchAny = AdvAny + SCH.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", SGE.JobID)]
        SgeAny = AdvAny + SGE.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", SMN.JobID)]
        SmnAny = AdvAny + SMN.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", WAR.JobID)]
        WarAny = AdvAny + WAR.JobID,

        [CustomComboInfo("Any", "This should not be displayed. This always returns true when used with IsEnabled.", WHM.JobID)]
        WhmAny = AdvAny + WHM.JobID,

        [CustomComboInfo("Disabled", "This should not be used.", ADV.JobID)]
        Disabled = 99999,

        #endregion
        // ====================================================================================
        #region GLOBAL FEATURES
        //Tank Features
        [CustomComboInfo("Global Tank Features", "Features and options involving shared role actions for Tanks.\nCollapsing this category does NOT disable the features inside.", ADV.JobID)]
        AllTankFeatures = 100099,

            #region Global Tank Features
            [ReplaceSkill(All.LowBlow, PLD.ShieldBash)]
            [ParentCombo(AllTankFeatures)]
            [CustomComboInfo("Tank: Interrupt Feature", "Replaces Low Blow (Stun) with Interject (Interrupt) when the target can be interrupted.\nPLDs can slot Shield Bash to have the feature to work with Shield Bash.", ADV.JobID)]
            ALL_Tank_Interrupt = 100000,

            [ParentCombo(AllTankFeatures)]
            [CustomComboInfo("Tank: Double Reprisal Protection", "Prevents the use of Reprisal when target already has the effect by replacing it with Stone.", ADV.JobID)]
            ALL_Tank_Reprisal = 100001,
            #endregion

        //Healer Features
        [CustomComboInfo("Global Healer Features", "Features and options involving shared role actions for Healers.\nCollapsing this category does NOT disable the features inside.", ADV.JobID)]
        AllHealerFeatures = 100098,

            #region Global Healer Features
            [ReplaceSkill(AST.Ascend, WHM.Raise, SCH.Resurrection, SGE.Egeiro)]
            [ConflictingCombos(AstrologianAscendFeature, SCH_RaiseFeature, SGE_RaiseFeature, WHMRaiseFeature)]
            [ParentCombo(AllHealerFeatures)]
            [CustomComboInfo("Healer: Raise Feature", "Changes the class' Raise Ability into Swiftcast.", ADV.JobID)]
            ALL_Healer_Raise = 100010,
            #endregion

        //Magical Ranged Features
        [CustomComboInfo("Global Magical Ranged Features", "Features and options involving shared role actions for Magical Ranged DPS.\nCollapsing this category does NOT disable the features inside.", ADV.JobID)]
        AllCasterFeatures = 100097,

            #region Global Magical Ranged Features
            [ParentCombo(AllCasterFeatures)]
            [CustomComboInfo("Magical Ranged DPS: Double Addle Protection", "Prevents the use of Addle when target already has the effect by replacing it with Fell Cleave.", ADV.JobID)]
            ALL_Caster_Addle = 100020,

        //            [ConflictingCombos(SummonerRaiseFeature, RedMageSwiftVerraise)]
            [ConflictingCombos(SummonerRaiseFeature, RDM_Verraise)]
            [ParentCombo(AllCasterFeatures)]
            [CustomComboInfo("Magical Ranged DPS: Raise Feature", "Changes the class' Raise Ability into Swiftcast or Dualcast in the case of RDM.", ADV.JobID)]
            ALL_Caster_Raise = 100021,
            #endregion

        //Melee Features
        [CustomComboInfo("Global Melee DPS Features", "Features and options involving shared role actions for Melee DPS.\nCollapsing this category does NOT disable the features inside.", ADV.JobID)]
        AllMeleeFeatures = 100096,

            #region Global Melee Features
            [ParentCombo(AllMeleeFeatures)]
            [CustomComboInfo("Melee DPS: Double Feint Protection", "Prevents the use of Feint when target already has the effect by replacing it with Fire.", ADV.JobID)]
            ALL_Melee_Feint = 100030,
            #endregion

        //Ranged Physical Features
        [CustomComboInfo("Global Physical Ranged Features", "Features and options involving shared role actions for Physical Ranged DPS.\nCollapsing this category does NOT disable the features inside.", ADV.JobID)]
        AllRangedPhysicalFeatures = 100095,

            #region Global Ranged Physical Features 
            [ParentCombo(AllRangedPhysicalFeatures)]
            [CustomComboInfo("Physical Ranged DPS: Double Mitigation Protection", "Prevents the use of Tactician/Troubadour/Shield Samba when target already has one of those three effects by replacing it with Stardiver.", ADV.JobID)]
            ALL_Ranged_Mitigation = 100040,
            #endregion

        //Non-gameplay Features
        //[CustomComboInfo("Output Combat Log", "Outputs your performed actions to the chat.", ADV.JobID)]
        //AllOutputCombatLog = 100094,


        #endregion
        // ====================================================================================
        #region ASTROLOGIAN

        [ReplaceSkill(AST.Play)]
        [CustomComboInfo("Draw on Play", "Play turns into Draw when no card is drawn, as well as the usual Play behavior.", AST.JobID, 0, "Pot of Greed", "Draw some cards, or something. Idk, you're the one that chose to play AST.")]
        AstrologianCardsOnDrawFeaturelikewhat = 1000,

        [ReplaceSkill(AST.CrownPlay)]
        [CustomComboInfo("Crown Play to Minor Arcana", "Changes Crown Play to Minor Arcana when a card is not drawn or has Lord Or Lady Buff.", AST.JobID, 0, "Bestow Royalty", "This one's for the Lords and Ladies, lemme get a HYEEEAAAAAH!")]
        AstrologianCrownPlayFeature = 1001,

        [ReplaceSkill(AST.Benefic2)]
        [CustomComboInfo("Benefic 2 Downgrade", "Changes Benefic 2 to Benefic when Benefic 2 is not unlocked or available.", AST.JobID, 0, "Sprout's Benedict Cumberbatch", "Changes Big Benedict into Little Benedict when you visit the sprout universe.")]
        AstrologianBeneficFeature = 1002,

        [ReplaceSkill(All.Swiftcast)]
        [ConflictingCombos(ALL_Healer_Raise)]
        [CustomComboInfo("AST Alternative Raise Feature", "Changes Swiftcast to Ascend", AST.JobID, 0, "Rez-bot-3000", "Does your job for you, but faster. You're welcome, little sloth.")]
        AstrologianAscendFeature = 1003,

        [ReplaceSkill(AST.Malefic1, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic)]
        [ConflictingCombos(AstrologianAlternateDpsFeature, CustomValuesTest)]
        [CustomComboInfo("DPS Feature(On Malefic)", "Adds Combust to the main malefic combo whenever the debuff is not present or about to expire", AST.JobID, 0, "Green DPS? Look no further", "Adds fatter deeps to your combo. Just pick another job already...")]
        AstrologianDpsFeature = 1004,

        [ParentCombo(AstrologianDpsFeature)]
        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the DPS feature when below set MP value.", AST.JobID, 0, "Lucid SCREAMING Feature", "I heard once you could control your dreams... But then I woke up.")]
        AstrologianLucidFeature = 1008,

        [CustomComboInfo("Astrodyne Feature", "Adds Astrodyne to the DPS feature when ready", AST.JobID, 0, "Astro-whine Feature", "Astro-whining again? Sorry, everyone's busy looking at the SGE's cool floating sticks.")]
        AstrologianAstrodyneFeature = 1009,

        [ReplaceSkill(AST.AspectedHelios)]
        [CustomComboInfo("Aspected Helios Feature", "Replaces Aspected Helios whenever you are under Aspected Helios regen with Helios", AST.JobID, 0, "HELIOSCOPTER", "HELIOSCOPTER HELIOSCOPTER")]
        AstrologianHeliosFeature = 1010,

        [ConflictingCombos(AstAutoCardTarget)]
        [CustomComboInfo("Auto Card Draw", "Adds Auto Card Draw Onto Main DPS Feature", AST.JobID, 0, "Kaiba Feature", "You just activated my trap card!")]
        AstrologianAutoDrawFeature = 1011,

        [CustomComboInfo("Auto Crown Card Draw", "Adds Auto Crown Card Draw Onto Main DPS Feature ", AST.JobID, 0, "Kaiba 2, Electric Boogaloo", "It's a trap!")]
        AstrologianAutoCrownDrawFeature = 1012,

        [ConflictingCombos(AstAutoCardTarget)]
        [CustomComboInfo("AoE DPS Feature", "Adds AutoDraws/Astrodyne to the AoE Gravity combo", AST.JobID, 0, "A bowlful of cards", "Oops! All AoE!")]
        AstrologianDpsAoEFeature = 1013,

        [CustomComboInfo("Lazy Lord Feature", "Adds Lord Of Crowns Onto Main DPS/AoE Feature", AST.JobID, 0, "Brainless Lord Feature", "You're like that tiny guy from Shrek. - E -")]
        AstrologianLazyLordFeature = 1014,

        [ReplaceSkill(AST.Play)]
        [CustomComboInfo("Astrodyne on Play", "Play becomes Astrodyne when you have 3 seals.", AST.JobID, 0, "Astro-whine on Play", "Seal me up and let me die, baby")]
        AstrologianAstrodyneOnPlayFeature = 1015,

        [ReplaceSkill(AST.Combust, AST.Combust2, AST.Combust3)]
        [ConflictingCombos(AstrologianDpsFeature, CustomValuesTest)]
        [CustomComboInfo("Alternate DPS Feature (On Combust)", "Adds Combust to the main malefic combo whenever the debuff is not present or about to expire", AST.JobID, 0, "Alternate Deeps, buddy", "Now we're really doing your job for you. Damn.")]
        AstrologianAlternateDpsFeature = 1016,

        [ConflictingCombos(AstrologianDpsFeature, AstrologianAlternateDpsFeature, DisableCombustOnDpsFeature)]
        [CustomComboInfo("DPS Feature Custom Values Testing", "Same as DPSFeature (On Malefic).Allows you to customize target MaxHp & CurrentPercentageHp & CurrentHp checks. Testing Only! ", AST.JobID, 0, "Green DPS? But you looked further...", "Same as that other guy, but with NUMBERS.")]
        CustomValuesTest = 1017,

        [ParentCombo(AstrologianDpsFeature)]
        [ConflictingCombos(AstrologianAlternateDpsFeature)]
        [CustomComboInfo("Removes DoT From DPS Feature", "Removed DoT From the DPS Feature, You can still use all other features that are on malefic! ", AST.JobID, 0, "DPS Less", "Oh, look, guys! Look! This one's actually healing!")]
        DisableCombustOnDpsFeature = 1018,

        [CustomComboInfo("Lightspeed Feature", "Adds Lightspeed to the DPS Feature", AST.JobID, 0, "Warp-speed", "GO FASTER! FASTER!")]
        AstrologianLightSpeedFeature = 1020,

        [ParentCombo(AstrologianHeliosFeature)]
        [CustomComboInfo("Celestial Opposition Feature", "Adds Celestial Opposition", AST.JobID, 0)]
        AstrologianCelestialOppositionFeature = 1021,

        [ParentCombo(AstrologianHeliosFeature)]
        [CustomComboInfo("Lazy Lady Feature", "Adds Lady of Crowns, if the card is drawn", AST.JobID, 0)]
        AstrologianLazyLadyFeature = 1022,

        [ReplaceSkill(AST.Benefic2)]
        [CustomComboInfo("Simple Heal", "Single target healing", AST.JobID, 0)]
        AstrologianSimpleSingleTargetHeal = 1023,

        [ParentCombo(AstrologianSimpleSingleTargetHeal)]
        [CustomComboInfo("Essential Dignity Feature", "Essential Dignity will be added when the target is at or below the value set", AST.JobID, 0)]
        AstroEssentialDignity = 1024,

        [ParentCombo(AstrologianSimpleSingleTargetHeal)]
        [CustomComboInfo("Celestial Intersection Feature", "Adds Celestial Intersection.", AST.JobID, 0)]
        CelestialIntersectionFeature = 1025,

        [ParentCombo(AstrologianHeliosFeature)]
        [CustomComboInfo("Horoscope Feature", "Adds Horoscope.", AST.JobID, 0)]
        AstrologianHoroscopeFeature = 1026,

        [ParentCombo(AstrologianSimpleSingleTargetHeal)]
        [CustomComboInfo("Aspected Benefic Feature", "Adds Aspected Benefic & refreshes it if needed.", AST.JobID, 0)]
        AspectedBeneficFeature = 1027,

        [ParentCombo(AstrologianSimpleSingleTargetHeal)]
        [CustomComboInfo("Exaltation Feature", "Adds Exaltation.", AST.JobID, 0)]
        ExaltationFeature = 1028,

        [ConflictingCombos(AstrologianAutoDrawFeature, AstrologianDpsAoEFeature)]
        [ParentCombo(AstrologianCardsOnDrawFeaturelikewhat)]
        [CustomComboInfo("Quick Target Cards", "Grabs a suitable target from the party list when you draw a card and targets them for you.", AST.JobID)]
        AstAutoCardTarget = 1029,

        [ParentCombo(AstAutoCardTarget)]
        [CustomComboInfo("Keep Target Locked", "Keeps your target locked until you play the card", AST.JobID)]
        AstrologianTargetLock = 1030,

        [ParentCombo(AstAutoCardTarget)]
        [CustomComboInfo("Add Tanks/Healers to Auto-Target", "Targets a tank or healer if no DPS remain for quick target selection", AST.JobID)]
        AstrologianTargetExtraFeature = 1031,

        [ParentCombo(AstrologianCardsOnDrawFeaturelikewhat)]
        [CustomComboInfo("Redraw Feature", "Sets Draw to Redraw if you pull a card with a seal you already have and you can use Redraw.", AST.JobID)]
        AstRedrawFeature = 1032,

        [ConflictingCombos(AstReFocusFeature)]
        [ParentCombo(AstrologianCardsOnDrawFeaturelikewhat)]
        [CustomComboInfo("Target Previous Feature", "Once you've played your card, switch back to your previously manually selected target. (May also be who you played the card on)", AST.JobID)]
        AstReTargetFeature = 1033,

        [ConflictingCombos(AstReTargetFeature)]
        [ParentCombo(AstrologianCardsOnDrawFeaturelikewhat)]
        [CustomComboInfo("Target Focus Feature", "Once you've played your card, switch back to your focus target.", AST.JobID)]
        AstReFocusFeature = 1034,


        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [ReplaceSkill(BLM.Scathe)]
        [ConflictingCombos(BlackSimpleFeature)]
        [CustomComboInfo("Scathe Feature", "Replaces Scathe with Fire 4 or Blizzard 4 depending on Astral Fire/Umbral Ice.", BLM.JobID, 2, "BrainLess Mage", "One button, BAYBEE!")]
        BlackEnochianFeature = 2000,

        [ReplaceSkill(BLM.Transpose)]
        [CustomComboInfo("Umbral Soul/Transpose Feature", "Replaces Transpose with Umbral Soul when Umbral Soul is available.", BLM.JobID, 0, "Eh? Huh?", "Just does BLM things. Probably.")]
        BlackManaFeature = 2001,

        [ReplaceSkill(BLM.LeyLines)]
        [CustomComboInfo("Between the Ley Lines Feature", "Replaces Ley Lines with Between the Ley Lines when Ley Lines is active.", BLM.JobID, 0, "BLT Sandwich feature", "Look between, and you shall find")]
        BlackLeyLinesFeature = 2002,

        [ReplaceSkill(BLM.Blizzard, BLM.Freeze)]
        [CustomComboInfo("Blizzard 1/2/3 Feature", "Replaces Blizzard 1 with Blizzard 3 when out of Umbral Ice. Replaces Freeze with Blizzard 2 when synced.", BLM.JobID, 0, "Chilly boi", "Chill out, for real. It's sleepy sloth time")]
        BlackBlizzardFeature = 2003,

        [ReplaceSkill(BLM.Scathe)]
        [ConflictingCombos(BlackEnochianFeature, BlackSimpleFeature)]
        [CustomComboInfo("Xenoglossy Feature", "Replaces Scathe with Xenoglossy when available.", BLM.JobID, 0, "Glossy paint", "So shiny, so glossy...")]
        BlackScatheFeature = 2004,

        [ReplaceSkill(BLM.Fire)]
        [CustomComboInfo("Fire 1/3 Feature", "Replaces Fire 1 with Fire 3 outside of Astral Fire or when Firestarter proc is up.", BLM.JobID, 0, "Burna boi", "It's getting hot in here...")]
        BlackFire13Feature = 2005,

        [ReplaceSkill(BLM.Scathe)]
        [ParentCombo(BlackEnochianFeature)]
        [CustomComboInfo("Thundercloud Option", "Replaces Scathe with Thunder 1/3 when the debuff isn't present or expiring and Thundercloud is available.", BLM.JobID, 0, "Plug Socket Mode", "Forks at the ready!")]
        BlackThunderFeature = 2006,

        [ReplaceSkill(BLM.Fire4)]
        [ParentCombo(BlackEnochianFeature)]
        [CustomComboInfo("Despair Option", "Replaces Fire 4 with Despair when below 2400 MP.", BLM.JobID, 0, "My MP!", "The horror! The despair!")]
        BlackDespairFeature = 2007,

        [ReplaceSkill(BLM.Flare)]
        [CustomComboInfo("Simple AoE Feature", "Replaces Flare with a full one button rotation.", BLM.JobID, -1, "Dungeon Tesla Mode", "Asleep at the wheel? We've got you!")]
        BlackAoEComboFeature = 2008,

        [ReplaceSkill(BLM.Scathe)]
        [ParentCombo(BlackEnochianFeature)]
        [CustomComboInfo("Aspect Swap Option", "Replaces Scathe with Blizzard 3 when at 0 MP in Astral Fire or with Fire 3 when at 10000 MP in Umbral Ice with 3 Umbral Hearts.", BLM.JobID, 0, "", "")]
        BlackAspectSwapFeature = 2010,

        [ReplaceSkill(BLM.Scathe)]
        [ParentCombo(BlackThunderFeature)]
        [CustomComboInfo("Thunder 1/3 Option", "Replaces Scathe with Thunder 1/3 when the debuff isn't present or expiring.", BLM.JobID, 0, "Bzzt", "Shocking!")]
        BlackThunderUptimeFeature = 2011,

        [ReplaceSkill(BLM.Scathe)]
        [ConflictingCombos(BlackEnochianFeature, BlackScatheFeature, BlackSimpleTransposeFeature, BlackSimpleParadoxFeature)]
        [CustomComboInfo("Simple BLM Feature", "Replaces Scathe with a full one button rotation.", BLM.JobID, -3, "", "")]
        BlackSimpleFeature = 2012,

        [ParentCombo(BlackSimpleFeature)]
        [CustomComboInfo("CDs Option", "Adds Manafont, Sharpcast, Amplifier onto the Simple BLM feature.", BLM.JobID, 0, "", "!")]
        BlackSimpleBuffsFeature = 2013,

        [ParentCombo(BlackSimpleFeature)]
        [CustomComboInfo("Ley Lines Option", "Adds Ley Lines onto the Simple BLM feature.", BLM.JobID, 0, "", "!")]
        BlackSimpleBuffsLeylinesFeature = 2014,

        [ParentCombo(BlackSimpleFeature)]
        [CustomComboInfo("Triplecast / Swiftcast Option", "Adds Triplecast/Swiftcast onto the Simple BLM feature.", BLM.JobID, 0, "", "!")]
        BlackSimpleCastsFeature = 2015,

        [ParentCombo(BlackSimpleCastsFeature)]
        [CustomComboInfo("Pool Triplecast / Swiftcast Option", "Keep one triplecast usage and swiftcast for movement in the Simple BLM feature.", BLM.JobID, 0, "", "!")]
        BlackSimpleCastPoolingFeature = 2016,

        [ParentCombo(BlackSimpleFeature)]
        [CustomComboInfo("Pool Xenoglossy Option", "Keep one xenoglossy usage for movement in the Simple BLM feature.", BLM.JobID, 0, "", "!")]
        BlackSimplePoolingFeature = 2017,

        [ParentCombo(BlackSimpleFeature)]
        [CustomComboInfo("Fire 3 Opener", "Adds the Fire 3 Opener to Simple BLM.", BLM.JobID, 0, "", "")]
        BlackSimpleOpenerFeature = 2018,

        [ParentCombo(BlackSimpleOpenerFeature)]
        [CustomComboInfo("Fire 3 Opener - 1 Triplecast", "Modifies the Simple Fire 3 Opener to only use 1 Triplecast.", BLM.JobID, 0, "", "")]
        BlackSimpleAltOpenerFeature = 2019,

        [ParentCombo(BlackAoEComboFeature)]
        [CustomComboInfo("Foul / Manafont Flare Option", "Adds Foul when available during Astral Fire. Weaves Manafont after Foul for additional Flare", BLM.JobID, 0, "", "")]
        BlackAoEFoulOption = 2020,

        [ReplaceSkill(BLM.Scathe)]
        [ConflictingCombos(BlackEnochianFeature, BlackScatheFeature, BlackSimpleFeature, BlackSimpleParadoxFeature)]
        [CustomComboInfo("Advanced BLM Feature", "Replaces Scathe with a full one button rotation that uses Transpose. Requires level 90.", BLM.JobID, -2, "", "")]
        BlackSimpleTransposeFeature = 2021,

        [ParentCombo(BlackSimpleTransposeFeature)]
        [CustomComboInfo("Pool Triplecast Option", "Keep one triplecast usage for movement in the Advanced BLM feature.", BLM.JobID, 0, "", "")]
        BlackSimpleTransposePoolingFeature = 2022,

        [ReplaceSkill(BLM.Scathe)]
        [ConflictingCombos(BlackEnochianFeature, BlackScatheFeature, BlackSimpleFeature, BlackSimpleTransposeFeature)]
        [CustomComboInfo("Paradox BLM Feature", "Replaces Scathe with a full one button rotation that has minimal casts (~9-13%% less damage than Simple BLM). Requires level 90.", BLM.JobID, -2, "", "")]
        BlackSimpleParadoxFeature = 2023,

        [ParentCombo(BlackSimpleTransposeFeature)]
        [CustomComboInfo("Ley Lines Option", "Adds Ley Lines onto the Advanced BLM feature.", BLM.JobID, 0, "", "")]
        BlackSimpleTransposeLeyLinesFeature = 2024,

        [ParentCombo(BlackSimpleParadoxFeature)]
        [CustomComboInfo("Ley Lines Option", "Adds Ley Lines onto the Paradox BLM feature.", BLM.JobID, 0, "", "")]
        BlackSimpleParadoxLeyLinesFeature = 2025,

        #endregion
        // ====================================================================================
        #region BLUE MAGE

        [BlueInactive(BLU.SongOfTorment, BLU.Bristle)]
        [ReplaceSkill(BLU.SongOfTorment)]
        [CustomComboInfo("Buffed Song of Torment", "Turns Song of Torment into Bristle so SoT is buffed. \nSpells Required: Song of Torment.", BLU.JobID)]
        BLU_BuffedSoT = 70000,

        [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident, BLU.Nightbloom, BLU.RoseOfDestruction, BLU.FeatherRain, BLU.Bristle, BLU.GlassDance, BLU.Surpanakha, BLU.MatraMagic, BLU.ShockStrike, BLU.PhantomFlurry)]
        [ReplaceSkill(BLU.MoonFlute)]
        [CustomComboInfo("Moon Flute Opener", "Puts the Full Moon Flute Opener on Moon Flute or Whistle. \nSpells Required: Whistle, Tingle, Moon Flute, J Kick, Triple Trident, Nightbloom, Rose of Destruction, Feather Rain, Bristle, Glass Dance, Surpanakha, Matra Magic, Shock Strike, Phantom Flurry.", BLU.JobID)]
        BLU_Opener = 70001,

        [BlueInactive(BLU.MoonFlute, BLU.Tingle, BLU.ShockStrike, BLU.Whistle, BLU.FinalSting)]
        [ReplaceSkill(BLU.FinalSting)]
        [CustomComboInfo("Final Sting Combo", "Turns Final Sting into the buff combo of: Moon Flute, Tingle, Whistle, Final Sting. Will use any primals off CD before casting Final Sting. \nSpells Required: Moon Flute, Tingle, Whistle, Final Sting", BLU.JobID)]
        BLU_FinalSting = 70002,

        [BlueInactive(BLU.RoseOfDestruction, BLU.FeatherRain, BLU.GlassDance, BLU.JKick)]
        [ParentCombo(BLU_FinalSting)]
        [CustomComboInfo("Off CD Primal Additions", "Adds any Primals that are off CD to the Final Sting Combo. \nPrimals Used: Feather Rain, Shock Strike, Glass Dance, J Kick, Rose of Destruction. ", BLU.JobID)]
        BluPrimals = 70003,

        [BlueInactive(BLU.RamsVoice, BLU.Ultravibration)]
        [ReplaceSkill(BLU.Ultravibration)]
        [CustomComboInfo("Ram's Voice into Ultravibration", "Turns Ultravibration into Ram's Voice if Deep Freeze isn't on the target. Will swiftcast Ultravibration if available. \nSpells Required: Ram's Voice, Ultravibration. ", BLU.JobID)]
        BLU_Ultravibrate = 70005,

        [BlueInactive(BLU.Offguard, BLU.BadBreath, BLU.Devour)]
        [ReplaceSkill(BLU.Devour, BLU.Offguard, BLU.BadBreath)]
        [CustomComboInfo("Tank Debuff Feature", "Puts Devour, Off-Guard, Lucid Dreaming, and Bad Breath into one button when under Tank Mimicry. \nSpells Required: Devour, Off-Guard, Bad Breath.", BLU.JobID)]
        BLU_DebuffCombo = 70006,

        [BlueInactive(BLU.MagicHammer)]
        [ReplaceSkill(BLU.MagicHammer)]
        [CustomComboInfo("Addle/Magic Hammer Debuff Feature", "Turns Magic Hammer into Addle when off CD. \nSpells Required: Magic Hammer.", BLU.JobID)]
        BLU_Addle = 70007,

        [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance, BLU.JKick)]
        [ReplaceSkill(BLU.FeatherRain)]
        [CustomComboInfo("Primal Feature", "Turns Feather Rain into any Primals that are off CD. \nSpells Required: Feather Rain, Shock Strike, The Rose of Destruction, Glass Dance, J Kick. \nWill cause primals to desync from Moon Flute burst phases if used on CD.", BLU.JobID)]
        BLU_PrimalCombo = 70008,

        [BlueInactive(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
        [ReplaceSkill(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
        [CustomComboInfo("Knight's Tour Feature", "Turns Black Knight's Tour or White Knight's Tour into its counterpart when the enemy is under the effect of the spell's debuff. \nSpells Required: White Knight's Tour, Black Knight's Tour", BLU.JobID)]
        BLU_KnightCombo = 70009,

        [BlueInactive(BLU.PeripheralSynthesis, BLU.MustardBomb)]
        [ReplaceSkill(BLU.PeripheralSynthesis)]
        [CustomComboInfo("Peripheral Synthesis into Mustard Bomb", "Turns Peripheral Synthesis into Mustard Bomb when target is under the effect of Lightheaded. \nSpells Required: Peripheral Synthesis, Mustard Bomb.", BLU.JobID)]
        BLU_LightHeadedCombo = 70010,

        [BlueInactive(BLU.BasicInstinct)]
        [ParentCombo(BLU_FinalSting)]
        [CustomComboInfo("Solo Mode", "Uses Basic Instinct if you're in an instance and on your own.", BLU.JobID)]
        BluSoloMode = 70011,

        [BlueInactive(BLU.HydroPull)]
        [ParentCombo(BLU_Ultravibrate)]
        [CustomComboInfo("Hydro Pull Setup", "Uses Hydro Pull before using Ram's Voice.", BLU.JobID)]
        BluHydroPull = 70012,


        #endregion
        // ====================================================================================
        #region BARD

        [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
        [ConflictingCombos(BRD_ST_SimpleMode)]
        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced.", BRD.JobID, 0, "", "")]
        BRD_StraightShotUpgrade = 3001,

        [ConflictingCombos(BRD_ST_SimpleMode)]
        [ParentCombo(BRD_StraightShotUpgrade)]
        [CustomComboInfo("DoT Maintenance Option", "Enabling this option will make Heavy Shot into Straight Shot refresh your DoTs on your current.", BRD.JobID, 0, "", "")]
        BRD_DoTMaintainance = 3002,

        [ReplaceSkill(BRD.IronJaws)]
        [ConflictingCombos(BRD_IronJaws_Alternate)]
        [CustomComboInfo("Iron Jaws Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nAlternates between the two if Iron Jaws isn't available.", BRD.JobID, 0, "", "")]
        BRD_IronJaws = 3003,

        [ReplaceSkill(BRD.IronJaws)]
        [ConflictingCombos(BRD_IronJaws)]
        [CustomComboInfo("Iron Jaws Alternate Feature", "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nIron Jaws will only show up when debuffs are about to expire.", BRD.JobID, 0, "", "")]
        BRD_IronJaws_Alternate = 3004,

        [ReplaceSkill(BRD.BurstShot, BRD.QuickNock)]
        [ConflictingCombos(BRD_ST_SimpleMode)]
        [CustomComboInfo("Burst Shot/Quick Nock into Apex Arrow", "Replaces Burst Shot and Quick Nock with Apex Arrow when gauge is full and Blast Arrow when you are Blast Arrow ready.", BRD.JobID, 0, "", "")]
        BRD_Apex = 3005,

        [ReplaceSkill(BRD.Bloodletter)]
        [ConflictingCombos(BRD_ST_SimpleMode)]
        [CustomComboInfo("Single Target oGCD Feature", "All oGCD's on Bloodletter (+ Songs rotation) depending on their CD.", BRD.JobID, 0, "", "")]
        BRD_ST_oGCD = 3006,

        [ReplaceSkill(BRD.RainOfDeath)]
        [ConflictingCombos(BRD_AoE_Combo)]
        [CustomComboInfo("AoE oGCD Feature", "All AoE oGCD's on Rain of Death depending on their CD.", BRD.JobID, 0, "", "")]
        BRD_AoE_oGCD = 3007,

        [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
        [ConflictingCombos(BRD_AoE_SimpleMode)]
        [CustomComboInfo("AoE Combo Feature", "Replaces Quick Nock/Ladonsbite with Shadowbite when ready", BRD.JobID, 0, "", "")]
        BRD_AoE_Combo = 3008,

        [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
        [ConflictingCombos(BRD_StraightShotUpgrade, BRD_DoTMaintainance, BRD_Apex, BRD_ST_oGCD, BRD_IronJawsApex)]
        [CustomComboInfo("Simple Bard", "Adds every single target ability to one button,\nIf there are DoTs on target Simple Bard will try to maintain their uptime.", BRD.JobID, 0, "", "")]
        BRD_ST_SimpleMode = 3009,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Bard DoTs", "This option will make Simple Bard apply DoTs if none are present on the target.", BRD.JobID, 0, "", "")]
        BRD_Simple_DoT = 3010,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Bard Songs", "This option adds the bards songs to the Simple Bard feature.", BRD.JobID, 0, "", "")]
        BRD_Simple_Song = 3011,

        [ParentCombo(BRD_AoE_oGCD)]
        [CustomComboInfo("Song Feature", "Adds Songs onto AoE oGCD Feature.", BRD.JobID, 0, "", "")]
        BRD_oGCDSongs = 3012,

        [CustomComboInfo("Bard Buffs Feature", "Adds Raging Strikes and Battle Voice onto Barrage.", BRD.JobID, 0, "", "")]
        BRD_Buffs = 3013,

        [CustomComboInfo("One Button Songs", "Add Mage's Ballad and Army's Paeon to Wanderer's Minuet depending on cooldowns", BRD.JobID, 0, "", "")]
        BRD_OneButtonSongs = 3014,

        [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
        [CustomComboInfo("Simple AoE Bard", "Weaves oGCDs onto Quick Nock/Ladonsbite", BRD.JobID, 0, "", "")]
        BRD_AoE_SimpleMode = 3015,

        [ParentCombo(BRD_AoE_SimpleMode)]
        [CustomComboInfo("Simple AoE Bard Song", "Weave songs on the Simple AoE", BRD.JobID, 0, "", "")]
        BRD_AoE_Simple_Songs = 3016,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Buffs", "Adds buffs onto the Simple Bard feature.", BRD.JobID, 0, "", "")]
        BRD_Simple_Buffs = 3017,

        [ParentCombo(BRD_Simple_Buffs)]
        [CustomComboInfo("Simple Buffs - Radiant", "Adds Radiant Finale to the Simple Buffs feature.", BRD.JobID, 0, "", "")]
        BRD_Simple_BuffsRadiant = 3018,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple No Waste Mode", "Adds enemy health checking on mobs for buffs, dots and songs.\nThey will not be reapplied if less than specified.", BRD.JobID, 0, "", "")]
        BRD_Simple_NoWaste = 3019,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Interrupt", "Uses interrupt during simple bard rotation if applicable", BRD.JobID, 0, "", "")]
        BRD_Simple_Interrupt = 3020,

        [CustomComboInfo("Disable Apex Arrow", "Removes Apex Arrow from Simple Bard and AoE Feature.", BRD.JobID, 0, "", "")]
        BRD_RemoveApexArrow = 3021,

        //[ConflictingCombos(BardoGCDSingleTargetFeature)]
        //[ParentCombo(SimpleBardFeature)]
        //[CustomComboInfo("Simple Opener", "Adds the optimum opener to simple bard.\nThis conflicts with pretty much everything outside of simple bard options due to the nature of the opener.", BRD.JobID, 0, "", "")]
        //BardSimpleOpener = 3022,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Pooling", "Pools Bloodletter charges to allow for optimum burst phases.", BRD.JobID, 0, "", "")]
        BRD_Simple_Pooling = 3023,

        [ConflictingCombos(BRD_ST_SimpleMode)]
        [ParentCombo(BRD_IronJaws)]
        [CustomComboInfo("Iron Jaws Apex", "Adds Apex and Blast Arrow to Iron Jaws when available", BRD.JobID, 0, "", "")]
        BRD_IronJawsApex = 3024,

        [ParentCombo(BRD_ST_SimpleMode)]
        [CustomComboInfo("Simple Raging Jaws", "Enable the snapshotting of DoTs, within the remaining time of Raging Strikes below:", BRD.JobID, 0, "", "")]
        BRD_Simple_RagingJaws = 3025,

        [ParentCombo(BRD_Simple_DoT)]
        [CustomComboInfo("Opener Only", "Until the first auto-refresh you can DoT new targets automatically.", BRD.JobID, 0, "", "")]
        BRD_Simple_DoTOpener = 3026,

        [ParentCombo(BRD_AoE_Simple_Songs)]
        [CustomComboInfo("Exclude Wanderer's Minuet", "Dont use Wanderer's Minuet.", BRD.JobID, 0, "", "")]
        BRD_AoE_Simple_SongsExcludeWM = 3027,

        #endregion
        // ====================================================================================
        #region DANCER

        #region Single Target Multibutton
        [ReplaceSkill(DNC.Cascade)]
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("Single Target Multibutton Feature", "Single target combo with Fan Dances and Esprit use.", DNC.JobID, 0, "", "")]
        DNC_ST_MultiButton = 4000,

            [ParentCombo(DNC_ST_MultiButton)]
            [CustomComboInfo("ST Esprit Overcap Option", "Adds Saber Dance above the set Esprit threshold.", DNC.JobID, 0, "", "")]
            DNC_ST_EspritOvercap = 4001,

            [ParentCombo(DNC_ST_MultiButton)]
            [CustomComboInfo("Fan Dance Overcap Protection Option", "Adds Fan Dance 1 when Fourfold Feathers are full.", DNC.JobID, 0, "", "")]
            DNC_ST_FanDanceOvercap = 4003,

            [ParentCombo(DNC_ST_MultiButton)]
            [CustomComboInfo("Fan Dance Option", "Adds Fan Dance 3/4 when available.", DNC.JobID, 0, "", "")]
            DNC_FanDance34_MainCombo = 4004,
            #endregion

        #region AoE Multibutton
        [ReplaceSkill(DNC.Windmill)]
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("AoE Multibutton Feature", "AoE combo with Fan Dances and Esprit use.", DNC.JobID, 0, "", "")]
        DNC_AoE_MultiButton = 4010,

            [ParentCombo(DNC_AoE_MultiButton)]
            [CustomComboInfo("AoE Esprit Overcap Option", "Adds Saber Dance above the set Esprit threshold.", DNC.JobID, 0, "", "")]
            DNC_AoE_EspritOvercap = 4011,

            [ParentCombo(DNC_AoE_MultiButton)]
            [CustomComboInfo("AoE Fan Dance Overcap Protection Option", "Adds Fan Dance 2 when Fourfold Feathers are full.", DNC.JobID, 0, "", "")]
            DNC_AoE_FanDanceOvercap = 4013,

            [ParentCombo(DNC_AoE_MultiButton)]
            [CustomComboInfo("AoE Fan Dance Option", "Adds Fan Dance 3/4 when available.", DNC.JobID, 0, "", "")]
            DancerFanDanceOnAoEComboFeature = 4014,
            #endregion

        #region Dance Features
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("Dance Features", "Features and options involving Standard Step and Technical Step.\nCollapsing this category does NOT disable the features inside.", DNC.JobID, 0, "", "")]
        DNC_Dance_Menu = 4020,

            #region Combined Dance Feature
            [ReplaceSkill(DNC.StandardStep)]
            [ParentCombo(DNC_Dance_Menu)]
            [ConflictingCombos(DNC_DanceStepCombo, DNC_DanceComboReplacer, DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
            [CustomComboInfo("Combined Dance Feature", "Standard And Technical Dance on one button (SS). Standard > Technical. This combos out into Tillana and Starfall Dance.", DNC.JobID, 0, "", "")]
            DNC_CombinedDances = 4022,

                [ParentCombo(DNC_CombinedDances)]
                [CustomComboInfo("Devilment Plus Option", "Adds Devilment right after Technical finish.", DNC.JobID, 0, "", "")]
                DancerDevilmentOnCombinedDanceFeature = 4023,

                [ParentCombo(DNC_CombinedDances)]
                [CustomComboInfo("Flourish Plus Option", "Adds Flourish to the Combined Dance Feature.", DNC.JobID, 0, "", "")]
                DancerFlourishOnCombinedDanceFeature = 4024,
                #endregion

            [ParentCombo(DNC_Dance_Menu)]
            [ConflictingCombos(DNC_DanceStepCombo, DNC_CombinedDances, DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
            [CustomComboInfo("Custom Dance Step Feature",
            "Change custom actions into dance steps while dancing." +
            "\nThis helps ensure you can still dance with combos on, without using auto dance." +
            "\nYou can change the respective actions by inputting action IDs below for each dance step." +
            "\nThe defaults are Cascade, Flourish, Fan Dance and Fan Dance II. If set to 0, they will reset to these actions." +
            "\nYou can get Action IDs with Garland Tools by searching for the action and clicking the cog.", DNC.JobID, 0, "", "")]
            DNC_DanceComboReplacer = 4025,
            #endregion

        #region Flourishing Features
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("Flourishing Features", "Features and options involving Fourfold Feathers and Flourish.\nCollapsing this category does NOT disable the features inside.", DNC.JobID, 0, "", "")]
        DNC_FlourishingFeatures_Menu = 4030,

            [ReplaceSkill(DNC.Flourish)]
            [ParentCombo(DNC_FlourishingFeatures_Menu)]
            [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
            [CustomComboInfo("Flourishing Fan Dance Feature", "Replace Flourish with Fan Dance 3 & 4 during weave-windows, when Flourish is on cooldown.", DNC.JobID, 0, "", "")]
            DNC_FlourishingFanDances = 4032,
            #endregion

        #region Fan Dance Combo Features
        [ParentCombo(DNC_FlourishingFeatures_Menu)]
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("Fan Dance Combo Feature", "Options for Fan Dance combos. Fan Dance 3 takes priority over Fan Dance 4.", DNC.JobID, 0, "", "")]
        DNC_FanDanceCombos = 4033,

            [ReplaceSkill(DNC.FanDance1)]
            [ParentCombo(DNC_FanDanceCombos)]
            [CustomComboInfo("Fan Dance 1 -> 3 Option", "Changes Fan Dance 1 to Fan Dance 3 when available.", DNC.JobID, 0, "", "")]
            DNC_FanDance_1to3_Combo = 4034,

            [ReplaceSkill(DNC.FanDance1)]
            [ParentCombo(DNC_FanDanceCombos)]
            [CustomComboInfo("Fan Dance 1 -> 4 Option", "Changes Fan Dance 1 to Fan Dance 4 when available.", DNC.JobID, 0, "", "")]
            DNC_FanDance_1to4_Combo = 4035,

            [ReplaceSkill(DNC.FanDance2)]
            [ParentCombo(DNC_FanDanceCombos)]
            [CustomComboInfo("Fan Dance 2 -> 3 Option", "Changes Fan Dance 2 to Fan Dance 3 when available.", DNC.JobID, 0, "", "")]
            DNC_FanDance_2to3_Combo = 4036,

            [ReplaceSkill(DNC.FanDance2)]
            [ParentCombo(DNC_FanDanceCombos)]
            [CustomComboInfo("Fan Dance 2 -> 4 Option", "Changes Fan Dance 2 to Fan Dance 4 when available.", DNC.JobID, 0, "", "")]
            DNC_FanDance_2to4_Combo = 4037,
            #endregion

        // Devilment --> Starfall
        [ReplaceSkill(DNC.Devilment)]
        [ConflictingCombos(DNC_ST_SimpleMode, DNC_AoE_SimpleMode)]
        [CustomComboInfo("Devilment to Starfall Feature", "Change Devilment into Starfall Dance after use.", DNC.JobID, 0, "", "")]
        DNC_Starfall_Devilment = 4038,

        [ReplaceSkill(DNC.StandardStep, DNC.TechnicalStep)]
        [ConflictingCombos(DNC_CombinedDances, DNC_DanceComboReplacer)]
        [CustomComboInfo("Dance Step Combo Feature", "Change Standard Step and Technical Step into each dance step while dancing.\nWorks with Simple Dancer and Simple Dancer AoE.", DNC.JobID, 0, "", "")]
        DNC_DanceStepCombo = 4039,

        #region Simple Dancer (Single Target)
        [ReplaceSkill(DNC.Cascade)]
        [ConflictingCombos(DNC_ST_MultiButton, DNC_AoE_MultiButton, DNC_CombinedDances, DNC_DanceComboReplacer, DNC_FlourishingFeatures_Menu, DNC_Starfall_Devilment)]
        [CustomComboInfo("Simple Dancer (Single Target) Feature", "Single button, single target. Includes songs, flourishes and overprotections.\nConflicts with all other non-simple toggles, except 'Dance Step Combo'.", DNC.JobID, 0, "", "")]
        DNC_ST_SimpleMode = 4050,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Interrupt Option", "Includes an interrupt in the rotation (if applicable to your current target).", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_Interrupt = 4051,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Standard Dance Option", "Includes Standard Step (and all steps) in the rotation.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_SS = 4052,

            [ParentCombo(DNC_ST_SimpleMode)]
            [ConflictingCombos(DNC_ST_Simple_TechFill)]
            [CustomComboInfo("Simple Technical Dance Option", "Includes Technical Step, all dance steps and Technical Finish in the rotation.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_TS = 4053,

            [ParentCombo(DNC_ST_SimpleMode)]
            [ConflictingCombos(DNC_ST_Simple_TS)]
            [CustomComboInfo("Simple Tech Fill Option", "Adds ONLY Technical dance steps and Technical Finish to the rotation.\nTechnical Step itself needs to be initiated manually when using this option.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_TechFill = 4054,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Tech Devilment Option", "Includes Devilment in the rotation.\nWill activate only during Technical Finish if you are Lv70 or above.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_Devilment = 4055,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Flourish Option", "Includes Flourish in the rotation.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_Flourish = 4056,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Feathers Option", "Includes Feather usage in the rotation.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_Feathers = 4057,

            [ParentCombo(DNC_ST_Simple_Feathers)]
            [CustomComboInfo("Simple Feather Pooling Option", "Expends a feather in the next available weave window when capped.\nWeaves feathers where possible during Technical Finish.\nWeaves feathers outside of burst when target is below set HP percentage.", DNC.JobID, 0, "")]
            DNC_ST_Simple_FeatherPooling = 4058,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Panic Heals Option", "Includes Curing Waltz and Second Wind in the rotation when available and your HP is below the set percentages.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_PanicHeals = 4059,

            [ParentCombo(DNC_ST_SimpleMode)]
            [CustomComboInfo("Simple Improvisation Option", "Includes Improvisation in the rotation when available.", DNC.JobID, 0, "", "")]
            DNC_ST_Simple_Improvisation = 4060,
            #endregion

        #region Simple Dancer (AoE)
        [ReplaceSkill(DNC.Windmill)]
        [ConflictingCombos(DNC_ST_MultiButton, DNC_AoE_MultiButton, DNC_CombinedDances, DNC_DanceComboReplacer, DNC_FlourishingFeatures_Menu, DNC_Starfall_Devilment)]
        [CustomComboInfo("Simple Dancer (AoE) Feature", "Single button, AoE. Includes songs, flourishes and overprotections.\nConflicts with all other non-simple toggles, except 'Dance Step Combo'.", DNC.JobID, 0, "", "")]
        DNC_AoE_SimpleMode = 4070,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Interrupt Option", "Includes an interrupt in the AoE rotation (if your current target can be interrupted).", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_Interrupt = 4071,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Standard Dance Option", "Includes Standard Step (and all steps) in the AoE rotation.", DNC.JobID, 0, "")]
            DNC_AoE_Simple_SS = 4072,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [ConflictingCombos(DNC_AoE_Simple_TechFill)]
            [CustomComboInfo("Simple AoE Technical Dance Option", "Includes Technical Step, all dance steps and Technical Finish in the AoE rotation.", DNC.JobID, 0, "")]
            DNC_AoE_Simple_TS = 4073,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [ConflictingCombos(DNC_AoE_Simple_TS)]
            [CustomComboInfo("Simple AoE Tech Fill Option", "Adds ONLY Technical dance steps and Technical Finish to the AoE rotation.\nTechnical Step itself needs to be initiated manually when using this option.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_TechFill = 4074,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Tech Devilment Option", "Includes Devilment in the AoE rotation.\nWill activate only during Technical Finish if you Lv70 or above.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_Devilment = 4075,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Flourish Option", "Includes Flourish in the AoE rotation.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_Flourish = 4076,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Feathers Option", "Includes feather usage in the AoE rotation.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_Feathers = 4077,

            [ParentCombo(DNC_AoE_Simple_Feathers)]
            [CustomComboInfo("Simple AoE Feather Pooling Option", "Expends a feather in the next available weave window when capped.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_FeatherPooling = 4078,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Panic Heals Option", "Includes Curing Waltz and Second Wind in the AoE rotation when available and your HP is below the set percentages.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_PanicHeals = 4079,

            [ParentCombo(DNC_AoE_SimpleMode)]
            [CustomComboInfo("Simple AoE Improvisation Option", "Includes Improvisation in the AoE rotation when available.", DNC.JobID, 0, "", "")]
            DNC_AoE_Simple_Improvisation = 4080,
            #endregion

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [ParentCombo(DarkSouleaterCombo)]
        [CustomComboInfo("Buffs on Main Combo", "Collection of Buffs to add to Main Combo", DRK.JobID)]
        DarkMainComboBuffsGroup = 5098,

        [ConflictingCombos(DarkoGCDFeature)]
        [ParentCombo(DarkSouleaterCombo)]
        [CustomComboInfo("CDs on Main Combo", "Collection of CDs to add to Main Combo", DRK.JobID)]
        DarkMainComboCDsGroup = 5099,

        [ReplaceSkill(DRK.Souleater)]
        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain. \nIf all sub options are selected will turn into a full one button rotation (Simple Dark Knight)", DRK.JobID, 0, "Fetch me their souls!", "Heheheheheh")]
        DarkSouleaterCombo = 5000,

        [ReplaceSkill(DRK.StalwartSoul)]
        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain.", DRK.JobID, 0, "", "Ugly name for an ugly job")]
        DarkStalwartSoulCombo = 5001,

        [ReplaceSkill(DRK.Souleater)]
        [ParentCombo(DarkMainComboBuffsGroup)]
        [CustomComboInfo("Delirium Feature", "Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active.", DRK.JobID, 0, "", "Delirium is what you have if you choose to play DRK.\nDoc's words, not mine")]
        DeliriumFeature = 5002,

        [ReplaceSkill(DRK.StalwartSoul)]
        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Dark Knight Gauge Overcap Feature", "Replace AoE combo with gauge spender if you are about to overcap.", DRK.JobID, 0, "", "Hey big spenderrrrr")]
        DRKOvercapFeature = 5003,

        [ParentCombo(DarkMainComboCDsGroup)]
        [CustomComboInfo("Living Shadow Feature", "Living Shadow will now be on main combo if its not on CD and you have gauge for it.", DRK.JobID, 0, "", "Trick everyone into thinking a party member is standing where they shouldn't be!")]
        DRKLivingShadowFeature = 5004,

        [ParentCombo(DarkSouleaterCombo)]
        [CustomComboInfo("EoS Overcap Feature", "Uses EoS if you are above 8.5k mana or Darkside is about to expire (10sec or less)", DRK.JobID, 0, "Something about mana", "You're basically a black mage! Well done!")]
        DarkManaOvercapFeature = 5005,

        [ReplaceSkill(DRK.CarveAndSpit, DRK.AbyssalDrain)]
        [ConflictingCombos(DarkMainComboCDsGroup)]
        [CustomComboInfo("oGCD Feature", "Adds Living Shadow > Salted Earth > Carve And Spit > Salt And Darkness to Carve And Spit and Abysal Drain", DRK.JobID, 0, "", "Just does your whole job for you, really")]
        DarkoGCDFeature = 5006,

        [ParentCombo(DarkoGCDFeature)]
        [CustomComboInfo("Shadowbringer oGCD Feature", "Adds Shadowbringer to oGCD Feature ", DRK.JobID, 0, "", "Wasn't this last expansion?")]
        DarkShadowbringeroGCDFeature = 5007,

        [ParentCombo(DarkMainComboCDsGroup)]
        [CustomComboInfo("Plunge Feature", "Adds Plunge onto main combo whenever its available and Darkside is up.", DRK.JobID, 0, "", "Take the plunge. All the way!")]
        DarkPlungeFeature = 5008,

        [ParentCombo(DeliriumFeature)]
        [CustomComboInfo("Delayed Delirium Feature", "Delays Bloodspiller by 2 GCDs when Delirium is used during even windows, uses it regularly during odd windows. Useful for feeding into raid buffs at level 90.", DRK.JobID, 0)]
        DelayedDeliriumFeatureOption = 5010,

        [ParentCombo(DarkSouleaterCombo)]
        [CustomComboInfo("Unmend Uptime Feature", "Replace Souleater Combo Feature with Unmend when you are out of range.", DRK.JobID, 0, "Ranged DPS job now, duh", "Stubby little arms, huh")]
        DarkRangedUptimeFeature = 5011,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Abyssal Drain Feature", "Adds abyssal drain to the AoE Combo when you fall below 60 percent hp.", DRK.JobID, 0, "", "Even the un-cool kids got heals these days")]
        DRKStalwartabyssalDrainFeature = 5013,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("AoE Shadowbringer Feature", "Adds Shadowbringer to the AoE Combo.", DRK.JobID, 0, "", "Wasn't this last expansion?")]
        DRKStalwartShadowbringerFeature = 5014,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("FoS Overcap Feature", "Uses FoS if you are above 8.5k mana or Darkside is about to expire (10sec or less)", DRK.JobID, 0, "Something about mana", "You're basically a black mage! Well done!")]
        DarkManaOvercapAoEFeature = 5015,

        [ParentCombo(DarkSouleaterCombo)]
        [CustomComboInfo("Blood Gauge Overcap Feature", "Adds Bloodspiller onto main combo when at 80 blood gauge or higher", DRK.JobID, 0, "", "Take the plunge. Or, just dip your toes in. Whatever.")]
        DarkBloodGaugeOvercapFeature = 5016,

        [ParentCombo(DarkMainComboCDsGroup)]
        [CustomComboInfo("Shadowbringer Feature", "Adds Shadowbringer on Main Combo while Darkside is up. Will use all stacks on CD.", DRK.JobID, 0)]
        DarkShBFeature = 5019,

        [ParentCombo(DarkManaOvercapFeature)]
        [CustomComboInfo("EoS Burst Option", "Uses EoS until chosen MP limit is reached during even minute window bursts.", DRK.JobID, 0)]
        DarkEoSPoolOption = 5020,

        [ParentCombo(DarkShBFeature)]
        [CustomComboInfo("Shadowbringer Burst Option", "Pools Shadowbringer to use during even minute window bursts.", DRK.JobID, 0)]
        DarkBurstShBOption = 5021,

        [ParentCombo(DarkMainComboCDsGroup)]
        [CustomComboInfo("Carve and Spit Feature", "Adds Carve and Spit on Main Combo while Darkside is up.", DRK.JobID, 0)]
        DarkCnSFeature = 5022,

        [ParentCombo(DarkPlungeFeature)]
        [CustomComboInfo("Melee Plunge Option", "Uses Plunge when under Darkside and in the target ring (1 yalm).\nWill use as many stacks as selected in the above slider.", DRK.JobID, 0)]
        DarkMeleePlungeOption = 5023,

        [ParentCombo(DarkMainComboCDsGroup)]
        [CustomComboInfo("Salted Earth Feature", "Adds Salted Earth on Main Combo while Darkside is up, will use Salt and Darkness if unlocked.", DRK.JobID, 0)]
        DarkSaltedEarthFeature = 5024,

        [ParentCombo(DeliriumFeature)]
        [CustomComboInfo("Delirium on CD", "Adds Delirium to Main Combo on CD and when Darkside is up. Will also spend 50 blood gauge if Delirium is nearly ready to protect from overcap.", DRK.JobID, 0)]
        DarkDeliriumOnCD = 5025,

        [ParentCombo(DarkMainComboBuffsGroup)]
        [CustomComboInfo("Blood Weapon on CD", "Adds Blood Weapon to Main Combo on CD and when Darkside is up.", DRK.JobID, 0)]
        DarkBloodWeaponOption = 5026,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Blood Weapon Option", "Adds Blood Weapon to AOE Combo on CD and when Darkside is up.", DRK.JobID, 0)]
        DarkBloodWeaponAOEOption = 5027,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Delirium Option", "Adds Deliriun to AOE Combo on CD and when Darkside is up.", DRK.JobID, 0)]
        DarkDeliriumAOEOption = 5028,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Salted Earth Option", "Adds Salted Earth and Salt and Darkness to AOE on CD and when Darkside is up.", DRK.JobID, 0)]
        DarkSaltedEarthAOEOption = 5029,

        [ParentCombo(DarkStalwartSoulCombo)]
        [CustomComboInfo("Living Shadow Option", "Adds Living Shadow to AOE on CD and when Darkside is up.", DRK.JobID, 0)]
        DarkLivingShadowAOEOption = 5030,
        

        #endregion
        // ====================================================================================
        #region DRAGOON

        [ReplaceSkill(DRG.CoerthanTorment)]
        [ConflictingCombos(DragoonSimpleAoE)]
        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain.", DRG.JobID, 1, "", "")]
        DragoonCoerthanTormentCombo = 6100,

        [ReplaceSkill(DRG.ChaosThrust)]
        [ConflictingCombos(DragoonSimple)]
        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain.", DRG.JobID, 2, "", "")]
        DragoonChaosThrustCombo = 6200,

            #region Chaos Thrust Combo
            [ParentCombo(DragoonChaosThrustCombo)]
            [CustomComboInfo("Chaos Piercing Talon Uptime", "Replaces Chaos Thrust Combo with Piercing Talon when you are out of range.", DRG.JobID, 3, "", "")]
            DragoonPiercingTalonChaosFeature = 6201,
            #endregion

        [ReplaceSkill(DRG.FullThrust)]
        [ConflictingCombos(DragoonFullThrustComboPlus, DragoonSimple)]
        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain.", DRG.JobID, 4, "", "")]
        DragoonFullThrustCombo = 6300,

            #region Full Thrust Combo
            [ParentCombo(DragoonFullThrustCombo)]
            [CustomComboInfo("Full Piercing Talon Uptime", "Replaces Full Thrust Combo with Piercing Talon when you are out of range.", DRG.JobID, 5, "", "")]
            DragoonPiercingTalonFullFeature = 6301,
        #endregion

        [ReplaceSkill(DRG.FullThrust)]
        [ConflictingCombos(DragoonFullThrustCombo, DragoonSimple)]
        [CustomComboInfo("Full Thrust Combo Plus", "Replace Full Thrust Plus Combo with its combo chain (Disembowel/Chaosthrust/life surge added).", DRG.JobID, 6, "", "")]
        DragoonFullThrustComboPlus = 6400,

            #region Full Thrust Combo Plus
            [ParentCombo(DragoonFullThrustComboPlus)]
            [CustomComboInfo("High Jump Plus Feature", "Includes High Jump in the rotation.", DRG.JobID, 7, "", "")]
            DragoonHighJumpPlusFeature = 6401,

            [ParentCombo(DragoonHighJumpPlusFeature)]
            [CustomComboInfo("Mirage Plus Feature", "Includes Mirage in the rotation.", DRG.JobID, 8, "", "")]
            DragoonMiragePlusFeature = 6402,

            [ParentCombo(DragoonFullThrustComboPlus)]
            [CustomComboInfo("Life Surge Plus Feature", "Includes Life Surge, while under proper buffs, onto proper GCDs, to the rotation.", DRG.JobID, 9, "", "")]
            DragoonLifeSurgePlusFeature = 6404,

            [ParentCombo(DragoonFullThrustComboPlus)]
            [CustomComboInfo("Plus Piercing Talon Uptime", "Replaces Full Thrust with Piercing Talon when you are out of range.", DRG.JobID, 10, "", "")]
            DragoonPiercingTalonPlusFeature = 6403,
        #endregion

        [ReplaceSkill(DRG.FullThrust)]
        [ConflictingCombos(DragoonFullThrustCombo, DragoonFullThrustComboPlus, DragoonChaosThrustCombo, DragoonFangThrustFeature, DragoonFangAndClawFeature)]
        [CustomComboInfo("Simple Dragoon", "Replaces Full Thrust with the entire DRG combo chain. Conflicts with every non-AoE feature.", DRG.JobID, 11, "", "")]
        DragoonSimple = 6500,

            #region Simple Dragoon
            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Simple Opener", "Level 88+. Use True North on prepull to activate. Adds opener to the Simple Dragoon rotation. Not recommended for use in dungeons. OPTIONAL: USE REACTION OR MOACTION FOR OPTIMAL TARGETING.", DRG.JobID, 12, "", "")]
            DragoonOpenerFeature = 6501,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Wyrmwind Thrust Feature", "Includes Wyrmwind Thrust to the Simple Dragoon rotation.", DRG.JobID, 13, "", "")]
            DragoonWyrmwindFeature = 6502,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Geirskogul and Nastrond Feature", "Includes Geirskogul and Nastrond in the rotation.", DRG.JobID, 18, "", "")]
            DragoonGeirskogulNastrondFeature = 6503,

            [ConflictingCombos(DragoonLitanyDiveFeature, DragoonLanceDiveFeature, DragoonLifeLitanyDiveFeature)]
            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Dives Feature", "Single Weave Friendly, but not optimal: Includes Spineshatter Dive, Dragonfire Dive and Stardiver in the rotation.", DRG.JobID, 14, "", "")]
            DragoonDiveFeature = 6504,

            [ConflictingCombos(DragoonDiveFeature, DragoonLitanyDiveFeature, DragoonLifeLitanyDiveFeature)]
            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Dives under Lance Charge Feature", "Single Weave Friendly: Includes Spineshatter Dive and Dragonfire Dive in the rotation, while under Lance Charge, and Stardiver while under Life of the Dragon.", DRG.JobID, 17, "", "")]
            DragoonLanceDiveFeature = 6505,

            [ConflictingCombos(DragoonDiveFeature, DragoonLanceDiveFeature, DragoonLifeLitanyDiveFeature)]
            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Dives under Litany Feature", "Double Weaves Required: Includes Spineshatter Dive and Dragonfire Dive in the rotation, while under Battle Litany, and Stardiver while under Life of the Dragon.", DRG.JobID, 15, "", "")]
            DragoonLitanyDiveFeature = 6506,

            [ConflictingCombos(DragoonDiveFeature, DragoonLanceDiveFeature, DragoonLitanyDiveFeature)]
            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Dives under Litany and Life of the Dragon Feature", "Double Weaves Required: Includes Spineshatter Dive and Dragonfire Dive in the rotation, while under Battle Litany and Life of the Dragon, and Stardiver while under Life of the Dragon.", DRG.JobID, 16, "", "")]
            DragoonLifeLitanyDiveFeature = 6507,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("High Jump Feature", "Includes High Jump in the rotation.", DRG.JobID, 19, "", "")]
            DragoonHighJumpFeature = 6508,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Mirage Feature", "Includes Mirage in the rotation.", DRG.JobID, 20, "", "")]
            DragoonMirageFeature = 6509,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Lance Charge Feature", "Includes Lance Charge to the rotation.", DRG.JobID, 21, "", "")]
            DragoonLanceFeature = 6510,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Dragon Sight Feature", "Includes Dragon Sight to the rotation. OPTIONAL: USE REACTION OR MOACTION FOR OPTIMAL TARGETING.", DRG.JobID, 22, "", "")]
            DragoonDragonSightFeature = 6511,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Battle Litany Feature", "Includes Battle Litany to the rotation.", DRG.JobID, 23, "", "")]
            DragoonLitanyFeature = 6514,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Life Surge Feature", "Includes Life Surge, while under proper buffs, onto proper GCDs, to the rotation.", DRG.JobID, 24, "", "")]
            DragoonLifeSurgeFeature = 6512,

            [ParentCombo(DragoonSimple)]
            [CustomComboInfo("Ranged Uptime Option", "Replaces Main Combo with Piercing Talon when you are out of melee range.\nNOT OPTIMAL.", DRG.JobID, 25, "", "")]
            DRGSimpleRangedUptimeST = 6513,
        #endregion

        [ReplaceSkill(DRG.CoerthanTorment)]
        [ConflictingCombos(DragoonCoerthanTormentCombo)]
        [CustomComboInfo("Simple Dragoon AoE", "One Button, many enemies hit.", DRG.JobID, 26, "", "")]
        DragoonSimpleAoE = 6600,

            #region Simple Dragoon AoE
            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Wyrmwind Thrust AoE Feature", "Includes Wyrmwind Thrust to the Simple Dragoon AoE rotation.", DRG.JobID, 27, "", "")]
            DragoonAoEWyrmwindFeature = 6601,

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Geirskogul and Nastrond AoE Feature", "Includes Geirskogul and Nastrond in the AoE rotation.", DRG.JobID, 28, "", "")]
            DragoonAoEGeirskogulNastrondFeature = 6602,

            [ConflictingCombos(DragoonAoELitanyDiveFeature, DragoonAoELifeLitanyDiveFeature, DragoonAoELanceDiveFeature)]
            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Dives AoE Feature", "Includes Spineshatter Dive, Dragonfire Dive and Stardiver in the AoE rotation.", DRG.JobID, 29, "", "")]
            DragoonAoEDiveFeature = 6603,

            [ConflictingCombos(DragoonAoEDiveFeature, DragoonAoELitanyDiveFeature, DragoonAoELifeLitanyDiveFeature)]
            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Dives under Lance Charge AoE Feature", "Single Weave Friendly: Includes Spineshatter Dive and Dragonfire Dive in the AoE rotation, while under Lance Charge, and Stardiver while under Life of the Dragon.", DRG.JobID, 30, "", "")]
            DragoonAoELanceDiveFeature = 6604,

            [ConflictingCombos(DragoonAoEDiveFeature, DragoonAoELanceDiveFeature, DragoonAoELifeLitanyDiveFeature)]
            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Dives under Litany AoE Features", "Includes Spineshatter Dive and Dragonfire Dive in the AoE rotation, while under Battle Litany, and Stardiver while under Life of the Dragon.", DRG.JobID, 31, "", "")]
            DragoonAoELitanyDiveFeature = 6605,

            [ConflictingCombos(DragoonAoEDiveFeature, DragoonAoELanceDiveFeature, DragoonAoELitanyDiveFeature)]
            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Dives under Litany and Life of the Dragon AoE Features", "Includes Spineshatter Dive and Dragonfire Dive in the AoE rotation, while under Battle Litany and Life of the Dragon, and Stardiver while under Life of the Dragon.", DRG.JobID, 32, "", "")]
            DragoonAoELifeLitanyDiveFeature = 6606,

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("High Jump AoE Feature", "Includes High Jump in the AoE rotation.", DRG.JobID, 33, "", "")]
            DragoonAoEHighJumpFeature = 6607,

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Mirage AoE Feature", "Includes Mirage in the AoE rotation.", DRG.JobID, 34, "", "")]
            DragoonAoEMirageFeature = 6608,

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Buffs AoE Feature", "Includes Lance Charge and Battle Litany to the AoE rotation.", DRG.JobID, 35, "", "")]
            DragoonAoEBuffsFeature = 6609,

                #region Buffs AoE Feature
                [ParentCombo(DragoonAoEBuffsFeature)]
                [CustomComboInfo("Dragon Sight AoE Feature", "Includes Dragon Sight to the AoE rotation. OPTIONAL: USE REACTION OR MOACTION FOR OPTIMAL TARGETING.", DRG.JobID, 36, "", "")]
                DragoonAoEDragonSightFeature = 6610,
                #endregion

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Life Surge AoE Feature", "Includes Life Surge, while under proper buffs, onto proper GCDs, to the AoE rotation.", DRG.JobID, 37, "", "")]
            DragoonAoELifeSurgeFeature = 6611,

            [ParentCombo(DragoonSimpleAoE)]
            [CustomComboInfo("Ranged Uptime Option", "Replaces Main AoE Combo with Piercing Talon when you are out of melee range.\nNOT OPTIMAL.", DRG.JobID, 40, "", "")]
            DRGSimpleRangedUptimeAoE = 6612,
            #endregion

        [ConflictingCombos(DragoonSimple)]
        [CustomComboInfo("Wheeling Thrust/Fang and Claw Option", "When you have either Enhanced Fang and Claw or Wheeling Thrust, Chaos Thrust Combo becomes Wheeling Thrust and Full Thrust Combo becomes Fang and Claw. Requires Chaos Thrust Combo and Full Thrust Combo.", DRG.JobID, 38, "", "")]
        DragoonFangThrustFeature = 6700,

        [ReplaceSkill(DRG.FangAndClaw)]
        [ConflictingCombos(DragoonSimple)]
        [CustomComboInfo("Wheeling Thrust/Fang and Claw Feature", "Fang And Claw Becomes Wheeling Thrust when under Enhanced Wheeling Thrust Buff.", DRG.JobID, 39, "", "")]
        DragoonFangAndClawFeature = 6701,

        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [ReplaceSkill(GNB.SolidBarrel)]
        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain. \nIf all sub options are selected will turn into a full one button rotation (Simple Gunbreaker)", GNB.JobID, 0, "Floppy Barrel Combo", "Not so solid NOW, are ya?")]
        GunbreakerSolidBarrelCombo = 7000,

        [ParentCombo(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Gnashing Fang and Continuation on Main Combo", "Adds Gnashing Fang to the main combo. Gnashing Fang must be started manually and the combo will finish it off.\n Useful for when Gnashing Fang needs to be help due to downtime.", GNB.JobID, 0, "Fashing Gnang", "Why grandma, what big teeth you have!")]
        GunbreakerGnashingFangOnMain = 7001,

        [ParentCombo(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("CDs on Main Combo", "Adds various CDs to the Main Combo when under No Mercy or when No Mercy is on cooldown", GNB.JobID, 0, "Gee Whiz!", "Mom, I can't manage my oGCDs!")]
        GunbreakerMainComboCDsGroup = 7002,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Double Down on Main Combo", "Adds Double Down on main combo when under No Mercy buff", GNB.JobID, 0, "ALL the deeps", "For when you're both feeling merciless and are stuffed full of powder. BANG!")]
        GunbreakerDDonMain = 7003,

        [ParentCombo(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Rough Divide Option", "Adds Rough Divide onto main combo whenever it's available.", GNB.JobID, 0, "Divide... Roughly", "Ayo pour one out for the homie Squall")]
        GunbreakerRoughDivideFeature = 7004,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Danger Zone/Blasting Zone on Main Combo", "Adds Danger Zone/Blasting Zone to the Main Combo", GNB.JobID, 0)]
        GunbreakerDZOnMainComboFeature = 7005,

        [ReplaceSkill(GNB.DemonSlaughter)]
        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain.", GNB.JobID, 0, "dEmOn SlAuGhTeR", "Demon Slaughter? Really? What is this, RPR?")]
        GunbreakerDemonSlaughterCombo = 7006,

        [ReplaceSkill(GNB.SolidBarrel, GNB.DemonSlaughter)]
        [CustomComboInfo("Ammo Overcap Feature", "Uses Burst Strike/Fated Circle on the respective ST/AoE combos when ammo is about to overcap.", GNB.JobID, 0, "Pew Pew Forever", "The whole nine yards")]
        GunbreakerAmmoOvercapFeature = 7007,

        [ReplaceSkill(GNB.GnashingFang)]
        [CustomComboInfo("Gnashing Fang Continuation Combo", "Adds Continuation to Gnashing Fang.", GNB.JobID, 0, "More Mercy", "More, no wait, less, no wait, MORE Mercy! No, wait...")]
        GunbreakerGnashingFangCombo = 7008,

        [ParentCombo(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("No Mercy on Gnashing Fang", "Adds No Mercy to Gnashing Fang when it's ready.", GNB.JobID, 0, "Blue DPS be like", "This one comes with teeth.")]
        GunbreakerNoMercyonGF = 7009,

        [ParentCombo(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("Double Down on Gnashing Fang", "Adds Double Down to Gnashing Fang when No Mercy buff is up.", GNB.JobID, 0, "My hotbars are a (tiny) mess!", "Thanks plogon!")]
        GunbreakerDDOnGF = 7010,

        [ParentCombo(GunbreakerGnashingFangCombo)]
        [CustomComboInfo("CDs on Gnashing Fang", "Adds Sonic Break/Bow Shock/Blasting Zone on Gnashing Fang, order dependent on No Mercy buff. \nBurst Strike and Hypervelocity added if there's charges while No Mercy buff is up.", GNB.JobID, 0, "More Teeth", "Gnashing fang, but like, if a shark did it. Or something.")]
        GunbreakerCDsOnGF = 7011,

        [ReplaceSkill(GNB.BurstStrike)]
        [CustomComboInfo("Burst Strike Continuation", "Adds Hypervelocity on Burst Strike.", GNB.JobID, 0, "Swish, swoosh", "Now we're cooking with gas! Hyper!")]
        GunbreakerBurstStrikeConFeature = 7012,

        [ReplaceSkill(GNB.BurstStrike)]
        [CustomComboInfo("Burst Strike to Bloodfest Feature", "Replace Burst Strike with Bloodfest if you have no powder gauge.", GNB.JobID, 0, "P4S Vampire man Bloodfest Feature", "Again with the edgelord names?\nTut, tut, Yoshi-P. Do better.")]
        GunbreakerBloodfestOvercapFeature = 7013,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Bloodfest on Main Combo", "Adds Bloodfest to main combo when ammo is 0.", GNB.JobID, 0)]
        GunbreakerBloodfestonST = 7014,

        [ParentCombo(GunbreakerSolidBarrelCombo)]
        [CustomComboInfo("Lightning Shot Uptime", "Replace Solid Barrel Combo Feature with Lightning Shot when you are out of range.", GNB.JobID, 0, "Stubby-armed GNB", "Can't reach?")]
        GunbreakerRangedUptimeFeature = 7015,

        [ParentCombo(GunbreakerDemonSlaughterCombo)]
        [CustomComboInfo("No Mercy AOE Option", "Adds No Mercy to AOE Combo when it's available.", GNB.JobID, 0, "")]
        GunbreakerNoMercyAOEOption = 7016,

        [ParentCombo(GunbreakerDemonSlaughterCombo)]
        [CustomComboInfo("Bow Shock on AoE Feature", "Adds Bow Shock onto the aoe combo when it's off cooldown. Recommended to use with Gnashing Fang features.", GNB.JobID, 0, "AoE cattleprod enabler")]
        GunbreakerBowShockFeature = 7017,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("No Mercy on Main Combo", "Adds No Mercy to main combo when at full ammo.", GNB.JobID, 0)]
        GunbreakerNoMercyonST = 7018,

        [ParentCombo(GunbreakerGnashingFangOnMain)]
        [CustomComboInfo("Gnashing Fang Starter", "Begins Gnashing Fang on main combo.", GNB.JobID, 0)]
        GunbreakerGFStartonMain = 7019,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Bow Shock on Main Combo", "Adds Bow Shock to the Main Combo", GNB.JobID, 0)]
        GunbreakerBSOnMainComboFeature = 7020,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Sonic Break on Main Combo", "Adds Sonic Break to the Main Combo", GNB.JobID, 0)]
        GunbreakerSBOnMainComboFeature = 7021,

        [ReplaceSkill(GNB.NoMercy)]
        [CustomComboInfo("Sonic Break/Bow Shock on NM", "Adds Sonic Break and Bow Shock to No Mercy when NM is on CD", GNB.JobID, 0)]
        GunbreakerCDsonNMFeature = 7022,

        [ParentCombo(GunbreakerMainComboCDsGroup)]
        [CustomComboInfo("Burst Strike on Main Combo", "Adds Burst Strike and Hypervelocity (when available) to Main Combo when under No Mercy and Gnashing Fang is over.", GNB.JobID, 0)]
        GunbreakerBSinNMFeature = 7023,

        [ParentCombo(GunbreakerDemonSlaughterCombo)]
        [CustomComboInfo("Bloodfest AOE Option", "Adds Bloodfest to AOE Combo when it's available. Will dump Ammo through Fated Circle to prepare for Bloodfest.", GNB.JobID, 0, "")]
        GunbreakerBloodfestAOEOption = 7024,

        [ParentCombo(GunbreakerDemonSlaughterCombo)]
        [CustomComboInfo("Double Down AOE Option", "Adds Double Down to AOE Combo when it's available and there is 2 or more ammo.", GNB.JobID, 0, "")]
        GunbreakerDoubleDownAOEOption = 7025,

        [ReplaceSkill(GNB.BurstStrike)]
        [CustomComboInfo("Double Down on Burst Strike Feature", "Adds Double Down to Burst Strike when under No Mercy and ammo is above 2.", GNB.JobID, 0, "")]
        GunbreakerDDonBurstStrikeFeature = 7026,

        [ParentCombo(GunbreakerRoughDivideFeature)]
        [CustomComboInfo("Melee Rough Divide Option", "Uses Rough Divide when under No Mercy, burst CDs on CD, and in the target ring (1 yalm).\nWill use as many stacks as selected in the above slider.", GNB.JobID, 0, "", "")]
        GunbreakerMeleeRoughDivideOption = 7027,

        [CustomComboInfo("Aurora Protection Feature", "Turns Aurora into Nascent Flash if Aurora's effect is on the player.", GNB.JobID, 0, "", "")]
        GunbreakerAuroraProtectionFeature = 7028,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [ReplaceSkill(MCH.CleanShot, MCH.HeatedCleanShot, MCH.SplitShot, MCH.HeatedSplitShot)]
        [ConflictingCombos(MachinistSimpleFeature)]
        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain.", MCH.JobID, 0, "Alright, Hotshot -", "Is there really such a thing as a clean shot? Let's find out.")]
        MachinistMainCombo = 8000,

        [ReplaceSkill(MCH.RookAutoturret, MCH.AutomatonQueen)]
        [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active.", MCH.JobID, 0, "Drive (2011) Feature", "Insert synthwave soundtrack here.")]
        MachinistOverdriveFeature = 8002,

        [ReplaceSkill(MCH.GaussRound, MCH.Ricochet)]
        [CustomComboInfo("Gauss Round / Ricochet Feature", "Replace Gauss Round and Ricochet with one or the other depending on which has more charges.", MCH.JobID, 0, "Gatling feature", "It's just a lot of bullets, really.")]
        MachinistGaussRoundRicochetFeature = 8003,

        [ReplaceSkill(MCH.Drill, MCH.AirAnchor, MCH.HotShot)]
        [CustomComboInfo("Drill / Air Anchor (Hot Shot) Feature", "Replace Drill and Air Anchor (Hot Shot) with one or the other (or Chainsaw) depending on which is on cooldown.", MCH.JobID, 0, "Multi-tool", "Why does MCH have a drill and a chainsaw? What is this, DoH?")]
        MachinistHotShotDrillChainsawFeature = 8004,

        [ParentCombo(MachinistMainCombo)]
        [ConflictingCombos(MachinistAlternateMainCombo)]
        [CustomComboInfo("Drill/Air/Chain Saw Feature On Main Combo", "Air Anchor followed by Drill is added onto main combo if you use Reassemble.\nIf Air Anchor is on cooldown and you use Reassemble, Chainsaw will be added to main combo instead.", MCH.JobID, 0, "A bit of everything feature", "Don't rub your last two brain-cells together! We got you!")]
        MachinistDrillAirOnMainCombo = 8005,

        [ReplaceSkill(MCH.HeatBlast)]
        [ConflictingCombos(MachinistSimpleFeature)]
        [CustomComboInfo("Single Button Heat Blast", "Switches Heat Blast to Hypercharge.", MCH.JobID, 0, "So-called 'Heat Blast'", "Basically a large hair-dryer.")]
        MachinistHeatblastGaussRicochetFeature = 8006,

        [ReplaceSkill(MCH.AutoCrossbow)]
        [CustomComboInfo("Single Button Auto Crossbow", "Switches Auto Crossbow to Hypercharge and weaves gauss/rico.", MCH.JobID, 0, "Laser Crossbow", "It's a crossbow, from the future!")]
        MachinistAutoCrossBowGaussRicochetFeature = 8018,

        [ParentCombo(MachinistMainCombo)]
        [ConflictingCombos(MachinistDrillAirOnMainCombo)]
        [CustomComboInfo("Alternate Drill/Air Feature on Main Combo", "Drill/Air/Hotshot Feature is added onto main combo (Note: It will add them onto main combo ONLY if you are under Reassemble Buff\nOr Reasemble is on CD (Will do nothing if Reassemble is OFF CD)", MCH.JobID, 0, "Gunbreaker feature", "Look at me - I'm the tank now.")]
        MachinistAlternateMainCombo = 8007,

        [ParentCombo(MachinistMainCombo)]
        [CustomComboInfo("Single Button HeatBlast On Main Combo Option", "Adds Single Button Heatblast onto the main combo when the option is enabled.", MCH.JobID, 0, "Scalp burner", "Dry your hair even faster! Even hotter! Owch!")]
        MachinistHeatBlastOnMainCombo = 8008,

        [ParentCombo(MachinistMainCombo)]
        [CustomComboInfo("Battery Overcap Option", "Overcharge protection for your Battery, If you are at 100 battery charge rook/queen will be added to your (Heated) Shot Combo.", MCH.JobID, 0, "Battery", "Isn't battery like, assault? What's wrong with you?")]
        MachinistOverChargeOption = 8009,

        [ParentCombo(MachinistSpreadShotFeature)]
        [CustomComboInfo("Battery AOE Overcap Option", "Adds overcharge protection to Spread Shot/Scattergun.", MCH.JobID, 0, "Battery", "Isn't battery like, assault? What's wrong with you?")]
        MachinistAoEOverChargeOption = 8010,

        [ParentCombo(MachinistSpreadShotFeature)]
        [CustomComboInfo("Gauss Round Ricochet on AOE Feature", "Adds Gauss Round/Ricochet to the AoE combo during Hypercharge.", MCH.JobID, 0, "Ratchet and Clank Feature", "Bouncing off the walls")]
        MachinistAoEGaussRicochetFeature = 8011,

        [ParentCombo(MachinistAoEGaussRicochetFeature)]
        [CustomComboInfo("Always Gauss Round/Ricochet on AoE Option", "Adds Gauss Round/Ricochet to the AoE combo outside of Hypercharge windows.", MCH.JobID, 0, "Ratchet and Clank Feature", "Bouncing off the walls")]
        MachinistAoEGaussOption = 8012,

        [ConflictingCombos(MachinistRicochetGaussMainCombo)]
        [ParentCombo(MachinistMainCombo)]
        [CustomComboInfo("Ricochet & Gauss Round Feature", "Adds Ricochet and Gauss Round to main combo. Will use all charges.", MCH.JobID, 0, "Pew +1", "All the shots, ALL of the time.")]
        MachinistRicochetGaussChargesMainCombo = 8017,

        [ConflictingCombos(MachinistRicochetGaussChargesMainCombo)]
        [ParentCombo(MachinistMainCombo)]
        [CustomComboInfo("Ricochet & Gauss Round overcap protection option", "Adds Ricochet and Gauss Round to main combo. Will leave 1 charge of each.", MCH.JobID, 0, "", "Never run out of bullets again! Or never have too many! Or something!\nBullets!")]
        MachinistRicochetGaussMainCombo = 8013,

        [ParentCombo(MachinistMainCombo)]
        [CustomComboInfo("Barrel Stabilizer drift protection feature", "Adds Barrel Stabilizer onto the main combo if heat is between 5-20.", MCH.JobID, 0, "Third Leg", "Adds Barrel Stabiliser when you're in heat. You heard me.")]
        BarrelStabilizerDrift = 8014,

        [ParentCombo(MachinistHeatblastGaussRicochetFeature)]
        [CustomComboInfo("Wildfire Feature", "Adds Wildfire to the Single Button Heat Blast Feature if Wildfire is off cooldown and you have enough heat for Hypercharge then Hypercharge will be replaced with Wildfire.\nAlso weaves Ricochet/Gauss Round on Heat Blast when necessary.", MCH.JobID, 0, "PEW", "PEW PEW PEW PEW, AND PEW.\nALSO WEAVES PEW AND PEW.")]
        MachinistWildfireFeature = 8015,

        [ParentCombo(MachinistSpreadShotFeature)]
        [CustomComboInfo("BioBlaster Feature", "Adds Bioblaster to the Spreadshot feature", MCH.JobID, 0, "War crime feature", "This is like uh, some sort of anthrax? Jeez bro. Relax.\nBanned by the Geneva Convention.")]
        MachinistBioblasterFeature = 8016,

        [CustomComboInfo("Barrel Feature", "Adds Barrel Stabalizer to Single Button Heat Blast and Single Button Auto Crossbow Features when below 50 heat and is off cooldown", MCH.JobID, 0, "Hot Cross Bow", "Now multi-purpose!")]
        MachinistAutoBarrel = 8019,

        [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
        [ConflictingCombos(MachinistMainCombo, MachinistHeatblastGaussRicochetFeature)]
        [CustomComboInfo("Simple Machinist", "Single button single target machinist, including buffs and overprotections.\nConflicts with other single target toggles!!\nMade to work optimally with a 2.5 GCD.", MCH.JobID, 0, "", "Goodbye, brain!")]
        MachinistSimpleFeature = 8020,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Interrupt", "Uses interrupt during simple machinist rotation, if applicable.", MCH.JobID, 0, "Simple Interr-", "Excuse me, I wa-")]
        MachinistSimpleInterrupt = 8021,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Gadget", "Adds Queen or Rook uses to the feature, based on your current level.\nTry to use Queen at optimal intervals between :55 to :05 windows.", MCH.JobID, 0, "Gadgeteer", "My queen!")]
        MachinistSimpleGadget = 8022,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Assembling", "Pairs reassemble uses with the following skills.\nBefore acquiring Drill it will be used with Clean Shot.", MCH.JobID, 0, "Megablox", "It's like Lego bricks! But worse!")]
        MachinistSimpleAssembling = 8023,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Gauss Ricochet", "Adds Gauss Round and Ricochet uses to the feature.", MCH.JobID, 0, "", "More bullets!")]
        MachinistSimpleGaussRicochet = 8024,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Wildcharge", "Adds Hypercharge and Wildfire uses to the feature.\nIt respects the 8 second rule of Drill, AirAnchor and Chainsaw.", MCH.JobID, 0, "Simply fantastic", "Is it really THAT simple?")]
        MachinistSimpleWildCharge = 8025,

        [ParentCombo(MachinistSimpleFeature)]
        [CustomComboInfo("Simple Stabilizer", "Adds Barrel Stabilizer to the feature.\nWhen heat < 50 and Wildfire is off CD or about to come off CD.", MCH.JobID, 0, "Third Leg", "Reduces recoil when spraying and praying.")]
        MachinistSimpleStabilizer = 8026,

        [ParentCombo(MachinistSpreadShotFeature)]
        [CustomComboInfo("Hypercharge", "Adds hypercharge to the AoE.", MCH.JobID, 0, "Sugar Rush", "I'm gonna slap @augporto for putting in so many damn MCH features.\nHow full of witty one-liners do you think I am?!")]
        MachinistAoEHyperchargeFeature = 8027,

        [ReplaceSkill(MCH.SpreadShot)]
        [CustomComboInfo("Simple Machinist AOE", "Spread Shot turns into Scattergun when lvl 82 or higher, Both turn into Auto Crossbow when overheated\nand Bioblaster is used first whenever it is off cooldown.", MCH.JobID, 0, "Dungeon go zzzz", "AoE, but you're just not here. Go make a coffee.")]
        MachinistSpreadShotFeature = 8028,

        [ParentCombo(MachinistSimpleAssembling)]
        [CustomComboInfo("Drill", "Use Reassemble with Drill when available.", MCH.JobID, 0, "GigaDrillBreaker", "The Drill that will pierce the heavens!")]
        MachinistSimpleAssemblingDrill = 8029,

        [ParentCombo(MachinistSimpleAssembling)]
        [CustomComboInfo("Air Anchor", "Use Reassemble with Air Anchor when available.", MCH.JobID, 0, "Air Guitar", "Play the tunes!")]
        MachinistSimpleAssemblingAirAnchor = 8030,

        [ParentCombo(MachinistSimpleAssembling)]
        [CustomComboInfo("Chain Saw", "Use Reassemble with Chain Saw when available.", MCH.JobID, 0, "Giga Sauce", "The secret to life.")]
        MachinistSimpleAssemblingChainSaw = 8031,

        [ParentCombo(MachinistSimpleAssemblingDrill)]
        [CustomComboInfo("Only use Drill...", "...when you have max charges of reassemble.", MCH.JobID, 0, "GigaDrillBreaker MAX", "Mow pow in the pew!")]
        MachinistSimpleAssemblingDrillMaxCharges = 8032,

        [ParentCombo(MachinistSimpleAssemblingAirAnchor)]
        [CustomComboInfo("Only use Air Anchor...", "...when you have max charges of reassemble.", MCH.JobID, 0, "GigaDrillBreaker MAX", "Mow pow in the pew!")]
        MachinistSimpleAssemblingAirAnchorMaxCharges = 8033,

        [ParentCombo(MachinistSimpleAssemblingChainSaw)]
        [CustomComboInfo("Only use Chain Saw...", "...when you have max charges of reassemble.", MCH.JobID, 0, "GigaDrillBreaker MAX", "Mow pow in the pew!")]
        MachinistSimpleAssemblingChainSawMaxCharges = 8034,

        #endregion
        // ====================================================================================
        #region MONK

        [ReplaceSkill(MNK.ArmOfTheDestroyer)]
        [CustomComboInfo("Arm of the Destroyer Combo", "Replaces Arm Of The Destroyer with its combo chain.", MNK.JobID, 0, "", "Punching, but wide. It's like having extra hands!")]
        MnkArmOfTheDestroyerCombo = 9000,

        [ReplaceSkill(MNK.DragonKick)]
        [CustomComboInfo("Bootshine Feature", "Replaces Dragon Kick with Bootshine if both a form and Leaden Fist are up.", MNK.JobID, 0, "", "Shine ya shoes, guv'na?")]
        MnkBootshineFeature = 9001,

        [ReplaceSkill(MNK.TrueStrike)]
        [CustomComboInfo("Twin Snakes Feature", "Replaces True Strike with Twin Snakes if Disciplined Fist is not applied or is less than 6 seconds from falling off.", MNK.JobID, 0, "", "I've had it with these MF snakes on this MF plane!")]
        MnkTwinSnakesFeature = 9011,

        [ReplaceSkill(MNK.Bootshine)]
        [ConflictingCombos(MnkBootshineCombo)]
        [CustomComboInfo("Basic Rotation", "Basic Monk Combo on one button", MNK.JobID, 0, "", "I presses the buttons, I does the deeps")]
        MnkBasicCombo = 9002,

        [ReplaceSkill(MNK.PerfectBalance)]
        [CustomComboInfo("Perfect Balance Feature", "Perfect Balance becomes Masterful Blitz while you have 3 Beast Chakra.", MNK.JobID, 0, "", "They say life is like walking a tightrope...")]
        MonkPerfectBalanceFeature = 9003,

        [ReplaceSkill(MNK.DragonKick)]
        [CustomComboInfo("Bootshine Balance Feature", "Replaces Dragon Kick with Masterful Blitz if you have 3 Beast Chakra.", MNK.JobID, 0, "The tin", "Does what it says on the tin")]
        MnkBootshineBalanceFeature = 9004,

        [ReplaceSkill(MNK.HowlingFist, MNK.Enlightenment)]
        [CustomComboInfo("Howling Fist/Meditation Feature", "Replaces Howling Fist/Enlightenment with Meditation when the Fifth Chakra is not open.", MNK.JobID, 0, "", "Imagine using your fist to scream at someone. Welcome to MNK!\nEnjoy your stay.")]
        MonkHowlingFistMeditationFeature = 9005,

        [ReplaceSkill(MNK.Bootshine)]
        [ConflictingCombos(MnkBasicCombo)]
        [CustomComboInfo("Bootshine Combo", "Replace Bootshine with its combo chain. \nIf all sub options are selected will turn into a full one button rotation (Simple Monk).  Slider values can be used to control Disciplined Fist + Demolish uptime.", MNK.JobID, -2, "", "They call it 'basic' for a reason, you donkey")]
        MnkBootshineCombo = 9006,

        [ReplaceSkill(MNK.MasterfulBlitz)]
        [CustomComboInfo("Perfect Balance Feature Plus", "All of the (Optimal?) Blitz combos on Masterful Blitz when Perfect Balance Is Active", MNK.JobID, 0, "", "Try not to fall over, eh")]
        MnkPerfectBalancePlus = 9007,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("Masterful Blitz on Main Combo", "Adds Masterful Blitz to the Main Combo", MNK.JobID, 0, "", "It's maths, but for your Single Target combo!")]
        MonkMasterfulBlitzOnMainCombo = 9008,

        [ParentCombo(MnkArmOfTheDestroyerCombo)]
        [CustomComboInfo("Masterful Blitz to AoE Combo", "Adds Masterful Blitz to the AoE Combo.", MNK.JobID, 0, "", "It's maths, but for your AoE combo!")]
        MonkMasterfulBlitzOnAoECombo = 9009,

        [ReplaceSkill(MNK.RiddleOfFire)]
        [CustomComboInfo("Riddle of Fire/Brotherhood Feature", "Replaces Riddle of Fire with Brotherhood when Riddle of Fire is on cooldown.", MNK.JobID, 0, "", "Riddle me this, brotha'")]
        MnkRiddleOfFireBrotherhoodFeature = 9012,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("CDs on Main Combo", "Adds various CDs to the Main Combo when under Riddle of Fire or when Riddle of Fire is on cooldown.", MNK.JobID, 0, "", "Been passed on for dance partner again? Don't worry, you have some buffs of your own.")]
        MnkCDsOnMainComboFeature = 9013,

        [ParentCombo(MnkCDsOnMainComboFeature)]
        [CustomComboInfo("Riddle of Wind on Main Combo", "Adds Riddle of Wind to the Main Combo.", MNK.JobID, 0, "", "Easy, breezy.")]
        MnkRiddleOfWindOnMainComboFeature = 9014,

        [ParentCombo(MnkCDsOnMainComboFeature)]
        [CustomComboInfo("Perfect Balance on Main Combo", "Adds Perfect Balance to the Main Combo.", MNK.JobID, 0, "", "")]
        MnkPerfectBalanceOnMainComboFeature = 9015,

        [ParentCombo(MnkCDsOnMainComboFeature)]
        [CustomComboInfo("Brotherhood on Main Combo", "Adds Brotherhood to the Main Combo.", MNK.JobID, 0, "", "")]
        MnkBrotherhoodOnMainComboFeature = 9016,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("Meditation on Main Combo", "Adds Meditation to the Main Combo.", MNK.JobID, 0, "", "")]
        MnkMeditationOnMainComboFeature = 9017,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("Lunar Solar Opener", "Start with the Lunar Solar Opener on the Main Combo. Requires level 68 for Riddle of Fire. A 1.93/1.94 GCD is highly recommended.", MNK.JobID, 0, "", "")]
        MnkLunarSolarOpenerOnMainComboFeature = 9018,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("Main Combo on Demolish Option", "Replaces Demolish with the Main Combo, except without any oCDs added. Useful for saving burst.", MNK.JobID, -1, "", "")]
        MnkDemolishComboFeature = 9026,

        [ParentCombo(MnkArmOfTheDestroyerCombo)]
        [CustomComboInfo("CDs on AoE Combo", "Adds various CDs to the AoE Combo when under Riddle of Fire or when Riddle of Fire is on cooldown.", MNK.JobID, 0, "", "")]
        MnkCDsOnAoEComboFeature = 9019,

        [ParentCombo(MnkCDsOnAoEComboFeature)]
        [CustomComboInfo("Riddle of Wind on AoE Combo", "Adds Riddle of Wind to the AoE Combo.", MNK.JobID, 0, "", "")]
        MnkRiddleOfWindOnAoEComboFeature = 9020,

        [ParentCombo(MnkCDsOnAoEComboFeature)]
        [CustomComboInfo("Perfect Balance on AoE Combo", "Adds Perfect Balance to the AoE Combo.", MNK.JobID, 0, "", "")]
        MnkPerfectBalanceOnAoEComboFeature = 9021,

        [ParentCombo(MnkCDsOnAoEComboFeature)]
        [CustomComboInfo("Brotherhood on AoE Combo", "Adds Brotherhood to the AoE Combo.", MNK.JobID, 0, "", "")]
        MnkBrotherhoodOnAoEComboFeature = 9022,

        [ParentCombo(MnkArmOfTheDestroyerCombo)]
        [CustomComboInfo("Meditation on AoE Combo", "Adds Meditation to the AoE Combo.", MNK.JobID, 0, "", "")]
        MnkMeditationOnAoEComboFeature = 9023,

        [ParentCombo(MnkArmOfTheDestroyerCombo)]
        [CustomComboInfo("Thunderclap on AoE Combo", "Adds Thunderclap when out of combat to the AoE Combo.", MNK.JobID, 0, "", "")]
        MnkThunderclapOnAoEComboFeature = 9024,

        [ParentCombo(MnkBootshineCombo)]
        [CustomComboInfo("Thunderclap on Main Combo", "Adds Thunderclap when out of combat to the Main Combo.", MNK.JobID, 0, "", "")]
        MnkThunderclapOnMainComboFeature = 9025,

        #endregion
        // ====================================================================================
        #region NINJA

        [ReplaceSkill(NIN.ArmorCrush)]
        [ConflictingCombos(NinSimpleSingleTarget)]
        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain.", NIN.JobID, 3, "One, Two, Three", "It's a Ninja's life for me")]
        NinjaArmorCrushCombo = 10000,

        [ReplaceSkill(NIN.AeolianEdge)]
        [ConflictingCombos(NinSimpleSingleTarget)]
        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain.", NIN.JobID, 2, "Edgy Edge Combo", "Knife go stab")]
        NinjaAeolianEdgeCombo = 10001,

        //[CustomComboInfo("Simple AoE", "Replaces Death Blossom with the AoE rotation.", NIN.JobID)]
        //NinjaHakkeMujinsatsuCombo = 10002,

        //[CustomComboInfo("Dream to Assassinate", "Replace Dream Within a Dream with Assassinate when Assassinate Ready.", NIN.JobID)]
        //NinjaAssassinateFeature = 10003,

        [ReplaceSkill(NIN.Kassatsu)]
        [CustomComboInfo("Kassatsu to Trick", "Replaces Kassatsu with Trick Attack while Suiton or Hidden is up.\nCooldown tracking plugin recommended.", NIN.JobID, 4, "Katsu Curry to Trick", "This is how we eat at a restaurant and don't pay the bill.\nRUN!")]
        NinjaKassatsuTrickFeature = 10004,

        [ReplaceSkill(NIN.TenChiJin)]
        [CustomComboInfo("Ten Chi Jin to Meisui", "Replaces Ten Chi Jin (the move) with Meisui while Suiton is up.\nCooldown tracking plugin recommended.", NIN.JobID, 5, "Ten Chin Scratches to Chop-Suey", "Does something, probably.\nHow do you deal with all these attack names?")]
        NinjaTCJMeisuiFeature = 10005,

        [ReplaceSkill(NIN.Chi)]
        [CustomComboInfo("Kassatsu Chi/Jin Feature", "Replaces Chi with Jin while Kassatsu is up if you have Enhanced Kassatsu.", NIN.JobID, 6, "", "Swaps your Katsu curry with a Chi Chin-scratch.")]
        NinjaKassatsuChiJinFeature = 10006,

        [ReplaceSkill(NIN.Hide)]
        [CustomComboInfo("Hide to Mug", "Replaces Hide with Mug while in combat.", NIN.JobID, 7, "Stand and Deliver", "John Cena is a thief, now?")]
        NinjaHideMugFeature = 10007,

        [ReplaceSkill(NIN.AeolianEdge)]
        [CustomComboInfo("Aeolian to Ninjutsu Feature", "Replaces Aeolian Edge (combo) with Ninjutsu if any Mudra are used.", NIN.JobID, 8, "Hand signs and all that", "Do the Naruto thing, I think.\nIdk I don't watch anime, sorry")]
        NinjaNinjutsuFeature = 10008,

        [ConflictingCombos(NinSimpleSingleTarget)]
        [CustomComboInfo("GCDs to Ninjutsu Feature", "Every GCD combo becomes Ninjutsu while Mudras are being used.", NIN.JobID, 9, "Full-on Sign Language", "NOW you're really communicating with the party.")]
        NinjaGCDNinjutsuFeature = 10009,

        [ReplaceSkill(NIN.Huraijin)]
        [CustomComboInfo("Huraijin / Raiju Feature", "Replaces Huraijin with Forked and Fleeting Raiju when available.", NIN.JobID, 10, "Pikachu / Raichu Feature", "Does something? Maybe? Evolutions? Combos? Probably.")]
        NinjaHuraijinRaijuFeature = 10010,

        [ParentCombo(NinjaHuraijinRaijuFeature)]
        [CustomComboInfo("Huraijin / Raiju Feature Option 1", "Replaces Huraijin with Fleeting Raiju when available.", NIN.JobID, 11, "Pikachu / Raichu Option 1", "Does the same thing probably, who knows.")]
        NinjaHuraijinRaijuFeature1 = 10011,

        [ParentCombo(NinjaHuraijinRaijuFeature)]
        [CustomComboInfo("Huraijin / Raiju Feature Option 2", "Replaces Huraijin with Forked Raiju when available.", NIN.JobID, 12, "Pikachu / Raichu Option 2", "What we DO know, is that all NIN mains just slam their heads on the keyboard to do combos, anyway.")]
        NinjaHuraijinRaijuFeature2 = 10012,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("Armor Crush Feature", "Adds Armor Crush onto main combo.", NIN.JobID, 13, "", "Act like you can crush armor with your kitchen knives or whatever.")]
        NinjaArmorCrushOnMainCombo = 10013,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("Raiju Feature", "Adds Fleeting Raiju to Aeolian Edge Combo.", NIN.JobID, 14, "Raichu Feature", "Thunderbolt!")]
        NinjaFleetingRaijuFeature = 10014,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("HuraijinToMainCombo", "Adds Huraijin to main combo if Huton buff is not present", NIN.JobID, 15, "", "Smells like a hurricane. No idea.")]
        NinjaHuraijinFeature = 10015,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("BunshinOnMainCombo", "Adds Bunshin whenever its off cd and you have gauge for it on main combo.", NIN.JobID, 16, "What do you call a Viera that's been cut off at the knees?", "Bun-shin KEK")]
        NinjaBunshinFeature = 10016,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("BavacakraOnMainCombo", "Adds Bavacakra you have gauge for it on main combo.", NIN.JobID, 17, "BAKLAVA!", "BAKLAVA!")]
        NinjaBhavacakraFeature = 10017,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("Throwing Dagger Uptime Feature", "Replace Aeolian Edge with Throwing Daggers when targer is our of range.", NIN.JobID, 18, "", "Would probably make more sense for NIN to be a Ranged DPS, anyway.")]
        NinjaRangedUptimeFeature = 10018,

        [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
        [CustomComboInfo("Simple Mudras", "Simplify the mudra casting to avoid failing.", NIN.JobID, 19, "Simple Murder", "Murder, made simple. For the everyday user.")]
        NinjaSimpleMudras = 10020,

        [ReplaceSkill(NIN.TenChiJin)]
        [ParentCombo(NinjaTCJMeisuiFeature)]
        [CustomComboInfo("Ten Chi Jin Feature", "Turns Ten Chi Jin (the move) into Ten, Chi, and Jin.", NIN.JobID, 20, "", "Does literally nothing. Ever")]
        NinTCJFeature = 10021,

        [ReplaceSkill(NIN.SpinningEdge)]
        [ConflictingCombos(NinjaArmorCrushCombo, NinjaAeolianEdgeCombo, NinjaGCDNinjutsuFeature)]
        [CustomComboInfo("Simple Ninja Single Target", "Turns Spinning Edge into a one-button full single target rotation.\nUses Ninjitsus, applies Trick Attack and uses Armor Crush to upkeep Huton buff.", NIN.JobID, 0, "", "")]
        NinSimpleSingleTarget = 10022,

        [ReplaceSkill(NIN.DeathBlossom)]
        [CustomComboInfo("Simple Ninja AoE", "Turns Death Blossom into a one-button full AoE rotation.", NIN.JobID, 1, "Dote-on AoE", "Uses /dote on every target.")]
        NinSimpleAoE = 10023,

        [ParentCombo(NinSimpleSingleTarget)]
        [CustomComboInfo("Include Trick Attack", "Add or disable Trick Attack as part of the feature.", NIN.JobID, 1, "Surprise!", "It's like the Uno Reverse card of XIV!")]
        NinSimpleTrickFeature = 10024,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("Assassinate/Dream Within a Dream Feature", "Adds Assassinate and Dream Within a Dream to the combo.", NIN.JobID, 0, "Fever Dream", "Frothinig at the mouth")]
        NinAeolianAssassinateFeature = 10025,

        [ParentCombo(NinjaAeolianEdgeCombo)]
        [CustomComboInfo("Mug Feature", "Adds Mug to the combo.", NIN.JobID, 0, "Petty Theft", "Embarrassing.")]
        NinAeolianMugFeature = 10026,

        [ParentCombo(NinSimpleTrickFeature)]
        [CustomComboInfo("Kassatsu for Suiton Feature", "Allows the use of Kassatsu to set up Suiton. Suiton is prioritised above Hyosho Ranryu under this effect \nand your trick cooldown window has elapsed.", NIN.JobID)]
        NinSimpleTrickKassatsuFeature = 10027,

        [ParentCombo(NinSimpleAoE)]
        [CustomComboInfo("Hellfrog Medium Feature", "Adds Hellfrog Medium to the combo if you have Ninki to spend.", NIN.JobID)]
        NinSimpleHellfrogFeature = 10028,

        [ParentCombo(NinSimpleAoE)]
        [CustomComboInfo("Mudra Feature", "Adds Doton and Katon/Goka Mekkyaku to the combo.", NIN.JobID)]
        NinSimpleAoeMudras = 10029,

        [ParentCombo(NinSimpleAoE)]
        [CustomComboInfo("Bunshin Feature", "Adds Bunshin and Phantom Kamaitachi to the combo.", NIN.JobID)]
        NinSimpleAoeBunshin = 10030,

        [ParentCombo(NinSimpleSingleTarget)]
        [CustomComboInfo("Add Mug", "Adds Mug to this Simple Feature.", NIN.JobID, 2)]
        NinSimpleMug = 10031,

        [ReplaceSkill(NIN.Huraijin)]
        [CustomComboInfo("Huraijin / Armor Crush Combo", "Replace Huraijin with Armor Crush after using Gust Slash", NIN.JobID, 8)]
        NinHuraijinArmorCrush = 10032,

        [ParentCombo(NinSimpleSingleTarget)]
        [CustomComboInfo("Ninki Pooling Feature - Bunshin", "Allows you to have a minimum amount of Ninki saved before spending on Bunshin.", NIN.JobID)]
        NinNinkiBunshinPooling = 10033,

        [ParentCombo(NinSimpleSingleTarget)]
        [CustomComboInfo("Ninki Pooling Feature - Bhavacakra", "Allows you to have a minimum amount of Ninki saved before spending on Bhavacakra.", NIN.JobID)]
        NinNinkiBhavacakraPooling = 10034,

        #endregion
        // ====================================================================================
        #region PALADIN

        [ReplaceSkill(PLD.GoringBlade)]
        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain.", PLD.JobID, 0, "These aren't heals... huh?", "Just take the armour off and don a robe, we all know you're green on the inside.")]
        PaladinGoringBladeCombo = 11000,

        [ReplaceSkill(PLD.RoyalAuthority, PLD.RageOfHalone)]
        [CustomComboInfo("Royal Authority Combo", "All-in-one main combo adds Royal Authority/Rage of Halone.\nToggle all sub-options on to make this a 1 button rotation", PLD.JobID, 0, "", "Lmao, 'Authority'... If you say so, buddy.")]
        PaladinRoyalAuthorityCombo = 11001,

        [ParentCombo(PaladinAtonementFeature)]
        [CustomComboInfo("Atonement drop Feature", "Will drop the last Atonement charge right before FoF comes back off cooldown.\nPlease note that this assumes you use both FoF and Req according to the full FoF opener and standard loop\nRequires a skill speed tier of 2.45-2.40", PLD.JobID, 1, "", "Atonement for what? Picking the weakest Tank?")]
        PaladinAtonementDropFeature = 11002,

        [ReplaceSkill(PLD.Prominence)]
        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain.", PLD.JobID, 0, "Promenade feature", "Long walks on the promenade...")]
        PaladinProminenceCombo = 11003,

        [ParentCombo(PaladinReqMainComboFeature)]
        [CustomComboInfo("Holy Spirit Feature", "Replaces Royal Authority combo with Holy Spirit if you don't have the Fight or Flight buff", PLD.JobID, 0, "Auto-PLD", "Plays the whole job for you.\nJust stand there and take damage, right?")]
        PaladinRequiescatFeature = 11004,

        [ParentCombo(PaladinReqMainComboFeature)]
        [CustomComboInfo("Confiteor Combo Feature", "Replace Holy Spirit/Circle with Confiteor when Requiescat is up and MP is under 2000 or only one stack remains \nand adds Faith/Truth/Valor Combo after Confiteor.", PLD.JobID, 0, "Confetti Feature", "This is gonna be a nightmare to clean up.")]
        PaladinConfiteorFeature = 11005,

        [ReplaceSkill(PLD.SpiritsWithin, PLD.CircleOfScorn)]
        [CustomComboInfo("Scornful Spirits Feature", "Replace Spirits Within and Circle of Scorn with whichever is available soonest.", PLD.JobID, 0, "", "Two for the price of one!")]
        PaladinScornfulSpiritsFeature = 11006,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Goring Blade Feature", "Insert Goring Blade into the main combo when appropriate.", PLD.JobID, 0, "", "")]
        PaladinRoyalGoringOption = 11007,

        [ReplaceSkill(PLD.HolySpirit)]
        [CustomComboInfo("Standalone Holy Spirit Feature", "Replaces Holy Spirit with Confiteor and Confiteor combo", PLD.JobID, 0, "", "It's Christmas already?")]
        PaladinStandaloneHolySpiritFeature = 11008,

        [ReplaceSkill(PLD.HolyCircle)]
        [CustomComboInfo("Standalone Holy Circle Feature", "Replaces Holy Circle with Confiteor and Confiteor combo", PLD.JobID, 0, "", "This is MY circle.")]
        PaladinStandaloneHolyCircleFeature = 11009,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Intervene Feature", "Adds Intervene onto Main Combo whenever it's available.", PLD.JobID, 4, "", "It looks like a gap-closer. It smells like a gap-closer...")]
        PaladinInterveneFeature = 11010,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [ConflictingCombos(PaladinRangedUptimeFeature2)]
        [CustomComboInfo("Shield Lob Uptime Feature", "Replace Main Combo with Shield Lob when out of range.", PLD.JobID, 4, "", "Don't throw your shield, you're not Captain America.\nJust get close!")]
        PaladinRangedUptimeFeature = 11012,

        [ParentCombo(PaladinFightOrFlightMainComboFeature)]
        [ConflictingCombos(PaladinFightOrFlightFeature)]
        [CustomComboInfo("Fight or Flight", "Adds FoF onto the main combo (Testing).", PLD.JobID, 0, "", "What is this, P3S?")]
        PaladinFightOrFlightMainComboFeature = 11013,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Requiescat Feature", "Requiescat gets added onto the main combo when the Fight or Flight buff has 17 seconds remaining or less.", PLD.JobID, 2, "", "Just defend 4hed")]
        PaladinReqMainComboFeature = 11014,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [ConflictingCombos(PaladinRangedUptimeFeature)]
        [CustomComboInfo("Holy Spirit Uptime Feature", "Replace Royal Authority/Rage of Halone Feature with Holy Spirit when out of range.", PLD.JobID, 5, "(Un)Holy Halone", "Who is Halone and why are they so angry?")]
        PaladinRangedUptimeFeature2 = 11016,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Fight or Flight Feature", "Adds FoF onto the main combo with a delayed weave.", PLD.JobID, 2, "", "This feature hurts my brain. Yours too, no doubt")]
        PaladinFightOrFlightFeature = 11017,

        [ParentCombo(PaladinProminenceCombo)]
        [CustomComboInfo("Holy Circle Feature", "Replaces AoE combo with Holy Circle when Requiescat is active.", PLD.JobID, 1, "", "")]
        PaladinHolyCircleFeature = 11020,

        [ParentCombo(PaladinHolyCircleFeature)]
        [CustomComboInfo("AoE Confiteor Feature", "Replaces AoE combo with Confiteor when Requiescat is active and appropiate.", PLD.JobID, 2, "", "")]
        PaladinAoEConfiteorFeature = 11021,

        [ParentCombo(PaladinHolyCircleFeature)]
        [CustomComboInfo("AoE Requiescat Feature", "Replaces AoE combo with Requiescat when it's off cooldown.", PLD.JobID, 0, "", "")]
        PaladinReqAoEComboFeature = 11022,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Expiacion and Circle of Scorn Feature", "Adds Expiacion and Circle of Scorn onto the main combo during weave windows", PLD.JobID, 0, "", "")]
        PaladinExpiacionScornFeature = 11023,

        [ParentCombo(PaladinProminenceCombo)]
        [CustomComboInfo("AOE Expiacion / Circle of Scorn Feature", "Adds Expiacion and Circle of Scorn onto the main AOE combo during weave windows", PLD.JobID, 0, "", "")]
        PaladinAoEExpiacionScornFeature = 11024,

        [ParentCombo(PaladinRoyalAuthorityCombo)]
        [CustomComboInfo("Atonement Feature", "Replace Royal Authority with Atonement when under the effect of Sword Oath.", PLD.JobID, 1, "", "Atonement for what? Picking the weakest Tank?")]
        PaladinAtonementFeature = 11025,

        [ParentCombo(PaladinInterveneFeature)]
        [CustomComboInfo("Melee Intervene Option", "Uses Intervene when under Fight or Flight and in the target ring (1 yalm).\nWill use as many stacks as selected in the above slider.", PLD.JobID, 4, "", "")]
        PaladinMeleeInterveneOption = 11026,

        [ParentCombo(PaladinExpiacionScornFeature)]
        [CustomComboInfo("Expiacion and Circle of Scorn Option", "Uses Circle of Scorn and Expiacion when under Fight or Flight or when Fight or Flight is on cooldown", PLD.JobID, 4, "", "")]
        PaladinExpiacionScornOption = 11027,

        #endregion
        // ====================================================================================
        #region REAPER

        [CustomComboInfo("Positional Preference", "Choose positional order for all positional related features.\nSupports turning Slice/Shadow of Death into all positionals or Slice and Shadow of Death being two separate positionals.", RPR.JobID, 0, "", "")]
        ReaperPositionalConfig = 12000,

        #region Single Target (Slice) Combo Section
        [ReplaceSkill(RPR.Slice)]
        [CustomComboInfo("Slice Combo Feature", "Replace Slice with its combo chain.\nIf all sub options are toggled will turn into a full one button rotation (Simple Reaper)", RPR.JobID, 0, "", "")]
        ReaperSliceCombo = 12001,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Soul Slice Option", "Adds Soul Slice to Slice Combo when Soul Gauge is 50 or less and when current target is afflicted with Death's Design.", RPR.JobID, 0, "", "")]
        ReaperSoulSliceFeature = 12002,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Shadow Of Death Option", "Adds Shadow of Death to Slice Combo if Death's Design is not present on current target, or is about to expire.", RPR.JobID, 0, "", "")]
        ReaperShadowOfDeathFeature = 12003,

        [ParentCombo(ReaperShadowOfDeathFeature)]
        [CustomComboInfo("Double SoD Enshroud Option", "Uses Shadow of Death twice during the first of the two Enshroud Bursts during the 2-minute windows (Double Enshroud Burst).", RPR.JobID, 0, "", "")]
        DoubleSoDOption = 12004,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Stun Option", "Adds Leg Sweep to main combo when target is performing an interruptible cast.", RPR.JobID, 0, "", "")]
        ReaperStunOption = 12005,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the combo at 65%% and 40%% HP, respectively.", RPR.JobID, 0, "", "")]
        ReaperComboHealsOption = 12006,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Ranged Filler Option", "Replaces the combo chain with Harpe (or Harvest Moon, if available) when outside of melee range. Will not override Communio.", RPR.JobID, 0, "", "")]
        ReaperRangedFillerOption = 12007,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Enshroud Option", "Adds Enshroud to the combo when at 50 Shroud or greater and when current target is afflicted with Death's Design.", RPR.JobID, 0, "", "")]
        ReaperEnshroudonSTFeature = 12008,

        [ParentCombo(ReaperEnshroudonSTFeature)]
        [CustomComboInfo("Enshroud Burst (Double Enshroud) Option", "Uses Enshroud at 50 Shroud during Arcane Circle (mimics the 2-minute Double Enshroud window), but will pool Shroud outside of burst windows.\nBelow level 88, will use Enshroud at 50 gauge.", RPR.JobID, 0, "", "")]
        ReaperEnshroudPoolOption = 12009,

        [ParentCombo(GibbetGallowsonSTFeature)]
        [CustomComboInfo("Lemure's Slice Option", "Adds Lemure's Slice to the combo when there are 2 Void Shroud charges.", RPR.JobID, 1, "", "")]
        LemureonSTOption = 12010,

        [ParentCombo(GibbetGallowsonSTFeature)]
        [CustomComboInfo("Communio Finisher Option", "Adds Communio to the combo when there is 1 charge of Lemure Shroud left.", RPR.JobID, 1, "", "")]
        CommunioOnSTOption = 12011,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Arcane Circle Option", "Adds Arcane Circle to the combo when available and when current target is afflicted with Death's Design.", RPR.JobID, 0, "", "")]
        ArcaneCircleonSTFeature = 12012,

        [ParentCombo(ArcaneCircleonSTFeature)]
        [CustomComboInfo("Plentiful Harvest Option", "Adds Plentiful Harvest to the combo when available.", RPR.JobID, 0, "", "")]
        PlentifulHarvestonSTOption = 12013,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Gibbet and Gallows Option", "Adds Gibbet and Gallows to the combo when current target is afflicted with Death's Design.\nWill use Void/Cross Reaping during Enshroud.", RPR.JobID, 0, "", "")]
        GibbetGallowsonSTFeature = 12014,

        [ReplaceSkill(RPR.ShadowOfDeath)]
        [ParentCombo(GibbetGallowsonSTFeature)]
        [CustomComboInfo("Gibbet and Gallows on SoD Option", "Adds Gibbet and Gallows to Shadow of Death as well.", RPR.JobID, 0, "", "")]
        GibbetGallowsonSoD = 12015,

        [ParentCombo(ReaperSliceCombo)]
        [CustomComboInfo("Gluttony and Blood Stalk Option", "Adds Gluttony and Blood Stalk to the combo when target is afflicted with Death's Design, and the skills are off cooldown and < 50 soul.", RPR.JobID, 0, "", "")]
        GluttonyStalkonSTFeature = 12016,
        #endregion

        #region AoE (Scythe) Combo Section
        [ReplaceSkill(RPR.SpinningScythe)]
        [CustomComboInfo("Scythe Combo Feature", "Replace Spinning Scythe with its combo chain.\nIf all sub options are toggled will turn into a full one button rotation (Simple AOE)", RPR.JobID, 0, "", "")]
        ReaperScytheCombo = 12020,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Soul Scythe Option", "Adds Soul Scythe to AoE Combo when Soul Gauge is 50 or less and current target is afflicted with Death's Design.", RPR.JobID, 0, "", "")]
        ReaperSoulScytheFeature = 12021,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Whorl Of Death Option", "Adds Whorl of Death to AoE Combo if Death's Design is not present on current target, or is about to expire.", RPR.JobID, 0, "", "")]
        ReaperWhorlOfDeathFeature = 12022,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Guillotine Option", "Adds Guillotine to AoE combo when under Soul Reaver and when current target is afflicted with Death's Design.\nWill use Grim Reaping during Enshroud.", RPR.JobID, 0, "", "")]
        ReaperGuillotineFeature = 12023,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Arcane Circle Option", "Adds Arcane Circle to AoE combo when off cooldown.", RPR.JobID, 0, "", "")]
        ArcaneCircleonAOEFeature = 12024,

        [ParentCombo(ArcaneCircleonAOEFeature)]
        [CustomComboInfo("Plentiful Harvest Option", "Adds Plentiful Harvest to AoE combo when off cooldown and ready.", RPR.JobID, 0, "", "")]
        PlentifulHarvestonAOEOption = 12025,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Enshroud Option", "Adds Enshroud to the AoE combo when at 50 Shroud and greater and when current target is afflicted with Death's Design.", RPR.JobID, 0, "", "")]
        ReapearEnshroudonAOEFeature = 12026,

        [ParentCombo(ReapearEnshroudonAOEFeature)]
        [CustomComboInfo("Lemure's Slice Option", "Adds Lemure's Slice to the AoE combo when there are 2 Void Shrouds.", RPR.JobID, 0, "", "")]
        ReaperLemureAOEFeature = 12027,

        [ParentCombo(ReapearEnshroudonAOEFeature)]
        [CustomComboInfo("Communio Finisher Option", "Adds Communio to the AoE combo when there is 1 Lemure Shroud left.", RPR.JobID, 0, "", "")]
        ReaperComboCommunioAOEFeature = 12028,

        [ParentCombo(ReaperScytheCombo)]
        [CustomComboInfo("Gluttony and Blood Stalk Option", "Adds Gluttony and Blood Stalk to the AoE combo when current target is afflicted with Death's Design and Soul Gauge < 50.", RPR.JobID, 0, "", "")]
        GluttonyStalkonAOEFeature = 12029,
        #endregion

        #region Blood Stalk/Grim Swathe Combo Section
        [ReplaceSkill(RPR.BloodStalk, RPR.GrimSwathe)]
        [CustomComboInfo("Gluttony on Blood Stalk/Grim Swathe Feature", "Blood Stalk and Grim Swathe will turn into Gluttony when it is available.", RPR.JobID, 0, "", "")]
        ReaperBloodSwatheFeature = 12041,

        [ParentCombo(ReaperBloodSwatheFeature)]
        [CustomComboInfo("Gibbet and Gallows/Guillotine on Blood Stalk/Grim Swathe Feature", "Adds Gibbet and Gallows on Blood Stalk.\nAdds Guillotine on Grim Swathe.", RPR.JobID, 0, "", "")]
        ReaperBloodStalkComboFeature = 12040,

        [ParentCombo(ReaperBloodSwatheFeature)]
        [CustomComboInfo("Enshroud Combo Option", "Adds Enshroud Combo (Void/Cross Reaping, Communio, and Lemure's Slice) on Blood Stalk and Grim Swathe.", RPR.JobID, 0, "", "")]
        ReaperEnshroudonStalkComboFeature = 12042,
        #endregion

        #region Miscellaneous
        [ReplaceSkill(RPR.ArcaneCircle)]
        [CustomComboInfo("Arcane Circle Harvest Feature", "Replaces Arcane Circle with Plentiful Harvest when you have stacks of Immortal Sacrifice.", RPR.JobID, 0, "", "")]
        ReaperHarvestFeature = 12051,

        [ReplaceSkill(RPR.HellsEgress, RPR.HellsIngress)]
        [CustomComboInfo("Regress Feature", "Changes both Hell's Ingress and Hell's Egress turn into Regress when Threshold is active.", RPR.JobID, 0, "", "")]
        ReaperRegressFeature = 12052,

        [ReplaceSkill(RPR.Slice, RPR.SpinningScythe, RPR.ShadowOfDeath, RPR.Harpe, RPR.BloodStalk)]
        [CustomComboInfo("Soulsow Reminder Feature", "Adds Soulsow to Slice, Spinning Scythe, Shadow of Death, Harpe, and Blood Stalk when out of combat.", RPR.JobID, 0, "", "")]
        ReaperSoulSowReminderFeature = 12053,

        [ReplaceSkill(RPR.Harpe)]
        [ParentCombo(ReaperSoulSowReminderFeature)]
        [CustomComboInfo("Harpe Harvest Moon Feature", "Replaces Harpe with Harvest Moon when you are in combat with Soulsow active.", RPR.JobID, 0, "", "")]
        ReaperHarpeHarvestMoonFeature = 12054,

        [ReplaceSkill(RPR.Harpe, RPR.Slice)]
        [ParentCombo(ReaperSoulSowReminderFeature)]
        [CustomComboInfo("Enhanced Harpe Option", "Prevent Harvest Moon replacing Harpe when Enhanced Harpe is active.", RPR.JobID, 0, "", "")]
        ReaperHarpeHarvestMoonEnhancedOption = 12055,

        [ReplaceSkill(RPR.Harpe, RPR.Slice)]
        [ParentCombo(ReaperSoulSowReminderFeature)]
        [CustomComboInfo("Combat Harpe Option", "Prevent Harvest Moon replacing Harpe when you are not in combat.", RPR.JobID, 0, "", "")]
        ReaperHarpeHarvestMoonCombatOption = 12056,

        [ReplaceSkill(RPR.Enshroud)]
        [CustomComboInfo("Enshroud Protection Feature", "Turns Enshroud into Gibbet/Gallows to protect Soul Reaver waste.", RPR.JobID, 0, "", "")]
        ReaperEnshroudProtectionFeature = 12057,

        [ReplaceSkill(RPR.Gibbet,RPR.Gallows,RPR.Guillotine)]
        [CustomComboInfo("Enshroud Combo on Gibbet/Gallows and Guillotine", "Adds Lemure's Slice and Communio to Gibbet/Gallows and Lemure's Scythe and Communio to Guillotine.", RPR.JobID, 0, "", "")]
        ReaperEnshroudComboFeature = 12058,

        [ReplaceSkill(RPR.Enshroud)]
        [CustomComboInfo("Enshroud to Communio Feature", "Turns Enshroud to Communio when available to use.", RPR.JobID, 0, "", "")]
        ReaperEnshroudtoCommunioFeature = 12059,

        #endregion

        #endregion
        // ====================================================================================
        #region RED MAGE

        //RED_MAGE_FEATURE_NUMBERING
        //Numbering Scheme: 13[Section][Feature Number][Sub-Feature]
        //Example: 13110 (Section 1: Openers, Feature Number 1, Sub-feature 0)
        //New features should be added to the appropriate sections.
        //If more than 10 sub features, use the next feature number if available
        //The three digets after RDM.JobID can be used to reorder items in the list

        //SECTION_1_OPENERS
        [ReplaceSkill(RDM.Jolt, RDM.Jolt2)]
        [CustomComboInfo("Balance Opener [Lv.90]", "Replaces Jolt with the Balance opener ending with Resolution\n**Must move into melee range before melee combo**", RDM.JobID, 110)]
        RDM_Balance_Opener = 13110,

        [ParentCombo(RDM_Balance_Opener)]
        [CustomComboInfo("Use Opener at any Mana", "Removes 0/0 Mana reqirement to reset opener\n**All other actions must be off cooldown**", RDM.JobID, 111)]
        RDM_Opener_Any_Mana = 13111,

        //SECTION_2to3_ROTATION
        [ReplaceSkill(RDM.Jolt, RDM.Jolt2)]
        [CustomComboInfo("Verthunder/Veraero", "Replace Jolt with Verthunder and Veraero", RDM.JobID, 210)]
        RDM_VerthunderVeraero = 13210,

        [ParentCombo(RDM_VerthunderVeraero)]
        [CustomComboInfo("Single Target Acceleration", "Add Acceleration when no Verfire/Verstone proc is available", RDM.JobID, 211)]
        RDM_ST_Acceleration = 13211,

        [ParentCombo(RDM_ST_Acceleration)]
        [CustomComboInfo("Include Swiftcast", "Add Swiftcast when all Acceleration charges are used", RDM.JobID, 212)]
        RDM_ST_AccelSwiftCast = 13212,

        [ReplaceSkill(RDM.Jolt, RDM.Jolt2)]
        [CustomComboInfo("Verfire/Verstone", "Replace Jolt with Verfire and Verstone", RDM.JobID,220)]
        RDM_VerfireVerstone = 13220,

        [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Scatter, RDM.Impact, RDM.Fleche, RDM.Riposte, RDM.Moulinet)]
        [CustomComboInfo("Weave OGCD Damage", "Use oGCD actions on specified action(s)", RDM.JobID, 240)]
        RDM_OGCD = 13240,

        [ParentCombo(RDM_OGCD)]
        [CustomComboInfo("Fleche", "Use Fleche on above specified action(s)", RDM.JobID, 241)]
        RDM_Fleche = 13241,

        [ParentCombo(RDM_OGCD)]
        [CustomComboInfo("Contra Sixte", "Use Contre Sixte on above specified action(s)", RDM.JobID, 242)]
        RDM_ContraSixte = 13242,

        [ParentCombo(RDM_OGCD)]
        [CustomComboInfo("Engagement", "Use Engagement on above specified action(s) when in melee range", RDM.JobID, 243)]
        RDM_Engagement = 13243,

        [ParentCombo(RDM_Engagement)]
        [CustomComboInfo("Hold one charge", "Pool one charge of Engagement/Displacement for manual use", RDM.JobID, 246)]
        RDM_PoolEngage = 13246,

        [ParentCombo(RDM_OGCD)]
        [CustomComboInfo("Corps-a-corps", "Use Corps-a-corps on above specified action(s)", RDM.JobID, 244)]
        RDM_Corpsacorps = 13244,

        [ParentCombo(RDM_Corpsacorps)]
        [CustomComboInfo("Only in Melee Range", "Use Corps-a-corps only when in melee range", RDM.JobID, 245)]
        RDM_Corpsacorps_MeleeRange = 13245,

        [ParentCombo(RDM_Corpsacorps)]
        [CustomComboInfo("Hold one charge", "Pool one charge of Corp-a-corps for manual use", RDM.JobID, 247)]
        RDM_PoolCorps = 13247,

        [ReplaceSkill(RDM.Scatter, RDM.Impact)]
        [CustomComboInfo("Verthunder II/Veraero II", "Replace Scatter/Impact with Verthunder II or Veraero II", RDM.JobID, 310)]
        RDM_VerthunderIIVeraeroII = 13310,

        [ReplaceSkill(RDM.Scatter, RDM.Impact)]
        [CustomComboInfo("AoE Acceleration", "Use Acceleration on Scatter/Impact for increased damage", RDM.JobID, 320)]
        RDM_AoE_Acceleration = 13320,

        [ParentCombo(RDM_AoE_Acceleration)]
        [CustomComboInfo("Include Swiftcast", "Add Swiftcast when all Acceleration charges are used or when below level 50", RDM.JobID, 321)]
        RDM_AoE_AccelSwiftCast = 13321,

        [ParentCombo(RDM_AoE_Acceleration)]
        [CustomComboInfo("Weave Acceleration", "Only use acceleration during weave windows", RDM.JobID, 322)]
        RDM_AoE_WeaveAcceleration = 13322,

        //SECTION_4to5_MELEE
        [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Riposte)]
        [CustomComboInfo("Single Target Melee Combo", "Stack Reposte Combo on specified action(s)\n**Must be in melee range or have Gap close with Corps-a-corps enabled**", RDM.JobID, 410)]
        RDM_ST_MeleeCombo = 13410,

        [ParentCombo(RDM_ST_MeleeCombo)]
        [CustomComboInfo("Use Manafication and Embolden", "Add Manafication and Embolden on specified action(s)\n**Must be in melee range or have Gap close with Corps-a-corps enabled**", RDM.JobID, 411)]
        RDM_ST_ManaficationEmbolden = 13411,

        [ParentCombo(RDM_ST_ManaficationEmbolden)]
        [CustomComboInfo("Hold for Double Melee Combo [Lv.90]", "Hold both actions until you can perform a double melee combo", RDM.JobID, 412)]
        RDM_ST_DoubleMeleeCombo = 13412,

        [ReplaceSkill(RDM.Scatter, RDM.Impact)]
        [CustomComboInfo("AoE Melee Combo", "Use Moulinet on Scatter/Impact when over 60/60 mana", RDM.JobID, 420)]
        RDM_AoE_MeleeCombo = 13420,

        [ParentCombo(RDM_AoE_MeleeCombo)]
        [CustomComboInfo("Use Manafication and Embolden", "Add Manafication and Embolden to Scatter/Impact\n**Must be in range of Moulinet**", RDM.JobID, 411)]
        RDM_AoE_ManaficationEmbolden = 13421,

        [ParentCombo(RDM_ST_MeleeCombo)]
        [CustomComboInfo("Gap close with Corps-a-corps", "Use Corp-a-corps when out of melee range and you have enough mana to start the melee combo", RDM.JobID, 430)]
        RDM_ST_CorpsGapClose = 13430,

        [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Scatter, RDM.Impact, RDM.Riposte, RDM.Moulinet, RDM.Veraero, RDM.Veraero2, RDM.Veraero3, RDM.Verthunder, RDM.Verthunder2, RDM.Verthunder3)]
        [CustomComboInfo("Melee Finisher", "Add Verflare/Verholy and other finishing moves to specified action(s)", RDM.JobID, 510)]
        RDM_MeleeFinisher = 13510,

        //SECTION_6to7_QOL
        [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Veraero, RDM.Veraero2, RDM.Veraero3, RDM.Verthunder, RDM.Verthunder2, RDM.Verthunder3, RDM.Scatter, RDM.Impact)]
        [CustomComboInfo("Lucid Dreaming", "Use Lucid Dreaming on Jolt 1/2, Veraero 1/2/3, Verthunder 1/2/3, and Scatter/Impact when below threshold.", RDM.JobID, 610, "Lucid Dreaming the day away", "OOM? Git gud.")]
        RDM_LucidDreaming = 13610,

        [ReplaceSkill(All.Swiftcast)]
        [ConflictingCombos(ALL_Caster_Raise)]
        [CustomComboInfo("Verraise", "Changes Swiftcast to Verraise when under the effect of Swiftcast or Dualcast.", RDM.JobID, 620, "Swifty Verraise", "You're panicing right now, aren't you?")]
        RDM_Verraise = 13620,

        //SECTION_8to9_OTHERS                   
        [ReplaceSkill(RDM.Displacement)]
        [CustomComboInfo("Displacement <> Corps-a-corps", "Replace Displacement with Corps-a-corps when out of range.", RDM.JobID, 810, "I take two steps forward, you take two steps back.", "We come together because opposites attract.")]
        RDM_CorpsDisplacement = 13810,

        [ReplaceSkill(RDM.Embolden)]
        //[ConflictingCombos(RDM_ST_ManaficationEmbolden)]
        [CustomComboInfo("Embolden to Manafication", "Changes Embolden to Manafication when on cooldown.", RDM.JobID, 820, "You're approaching me?", "do do do do do do do do do")]
        RDM_EmboldenManafication = 13820,

        #endregion
        // ====================================================================================
        #region SAGE

        //SAGE_FEATURE_NUMBERING
        //Numbering Scheme: 14[Feature][Option][Sub-Option]
        //Example: 14110 (Feature Number 1, Option 1, no suboption)
        //New features should be added to the appropriate sections.

        #region SAGE DPS

        #region Single Target DPS Feature
        [ReplaceSkill(SGE.Dosis1, SGE.Dosis2, SGE.Dosis3)]
                [CustomComboInfo("Single Target DPS Feature", "Replaces Dosis with options below", SGE.JobID, 100)]
                SGE_ST_DosisFeature = 14100,
                
                    [ParentCombo(SGE_ST_DosisFeature)]
                    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming to Dosis when MP drops below slider value", SGE.JobID, 110)]
                    SGE_ST_Dosis_Lucid = 14110,

                    [ParentCombo(SGE_ST_DosisFeature)]
                    [CustomComboInfo("Eukrasian Dosis Option", "Automatic DoT Uptime", SGE.JobID, 120)]
                    SGE_ST_Dosis_EDosis = 14120,

                        [ParentCombo(SGE_ST_Dosis_EDosis)]
                        [ConflictingCombos(SGE_ST_Dosis_EDosisToT)]
                        [CustomComboInfo("Enemy HP Limiter Options", "Stop using Eukrasian Dosis when Enemy HP values match\nEnable to see input boxes", SGE.JobID, 121)]
                        SGE_ST_Dosis_EDosisHPPer = 14121,

                        [ParentCombo(SGE_ST_Dosis_EDosis)]
                        [ConflictingCombos(SGE_ST_Dosis_EDosisHPPer)]
                        [CustomComboInfo("Target of Target Dosis Option", "Target of Target checking for Dosis", SGE.JobID, 122)]
                        SGE_ST_Dosis_EDosisToT = 14122,

                    [ParentCombo(SGE_ST_DosisFeature)]
                    [CustomComboInfo("Toxikon Movement Option", "Use Toxikon when you have Addersting charges and are moving", SGE.JobID, 130)]
                    SGE_ST_Dosis_Toxikon = 14130,
                #endregion

                #region AoE DPS Feature
                [ReplaceSkill(SGE.Phlegma, SGE.Phlegma2, SGE.Phlegma3)]
                [CustomComboInfo("AoE DPS Feature", "Replaces Phlegma with various options", SGE.JobID, 200, "", "")]
                SGE_AoE_PhlegmaFeature = 14200,

                    [ParentCombo(SGE_AoE_PhlegmaFeature)]
                    [CustomComboInfo("No Phlegma to Toxikon Option", "Use Toxikon when out of Phlegma charges\nTakes priority over Dyskrasia", SGE.JobID, 210, "", "")]
                    SGE_AoE_Phlegma_NoPhlegmaToxikon = 14210,

                    [ParentCombo(SGE_AoE_PhlegmaFeature)]
                    [CustomComboInfo("Toxikon Distance Option", "Use Toxikon when out of Phlemga's Range\nTakes priority over Dyskrasia", SGE.JobID, 220, "", "")]
                    SGE_AoE_Phlegma_OutOfRangeToxikon = 14220,

                    [ParentCombo(SGE_AoE_PhlegmaFeature)]
                    [CustomComboInfo("No Phlegma to Dyskrasia Option", "Use Dyskrasia when out of Phlegma charges", SGE.JobID, 230, "", "Again, Phlegma is the worst skill name in the game. GET RID!")]
                    SGE_AoE_Phlegma_NoPhlegmaDyskrasia = 14230,

                    [ParentCombo(SGE_AoE_PhlegmaFeature)]
                    [CustomComboInfo("Dyskrasia No-Target Option", "Use Dyskrasia when no target is selected", SGE.JobID, 240, "", "")]
                    SGE_AoE_Phlegma_NoTargetDyskrasia = 14240,

                    [ParentCombo(SGE_AoE_PhlegmaFeature)]
                    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming to Phlegma when MP drops below slider value", SGE.JobID, 250)]
                    SGE_AoE_Phlegma_Lucid = 14250,

                #endregion

            #endregion

            #region Diagnosis Simple Single Target Heal
            [ReplaceSkill(SGE.Diagnosis)]
                [ConflictingCombos(SGE_RhizoFeature, SGE_DruoTauroFeature)]
                [CustomComboInfo("Diagnosis Simple Single Target Heal Feature", "Changes Diagnosis. You must target a party member (including yourself) for some features to work.", SGE.JobID, 300)]
                SGE_ST_HealFeature = 14300,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Apply Kardia Option", "Applies Kardia to your target if it's not applied to anyone else.", SGE.JobID, 310)]
                    SGE_ST_Heal_Kardia = 14310,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Eukrasian Diagnosis Option", "Diagnosis becomes Eukrasian Diagnosis if the shield is not applied to the target.", SGE.JobID, 320)]
                    SGE_ST_Heal_Diagnosis = 14320,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Soteria Option", "Applies Soteria when the selected target is at or above the set HP percentage.", SGE.JobID, 330)]
                    SGE_ST_Heal_Soteria = 14330,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Zoe Option", "Applies Zoe when the selected target is at or above the set HP percentage.", SGE.JobID, 340)]
                    SGE_ST_Heal_Zoe = 14340,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Pepsis Option", "Triggers Pepsis if a shield is present and the selected target is at or above the set HP percentage.", SGE.JobID, 350)]
                    SGE_ST_Heal_Pepsis = 14350,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Taurochole Option", "Adds Taurochole when the selected target is at or above the set HP percentage.", SGE.JobID, 360)]
                    SGE_ST_Heal_Taurochole = 14360,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Haima Option", "Adds Haima when the selected target is at or above the set HP percentage.", SGE.JobID, 370)]
                    SGE_ST_Heal_Haima = 14370,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Rhizomata Option", "Adds Rhizomata when Addersgall is 0", SGE.JobID, 380)]
                    SGE_ST_Heal_Rhizomata = 14380,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Krasis Option", "Applies Krasis when the selected target is at or above the set HP percentage.", SGE.JobID, 390)]
                    SGE_ST_Heal_Krasis = 14390,

                    [ParentCombo(SGE_ST_HealFeature)]
                    [CustomComboInfo("Druochole Option", "Adds Druochole when the selected target is at or above the set HP percentage.", SGE.JobID, 400)]
                    SGE_ST_Heal_Druochole = 14400,
            #endregion

            #region Sage Simple AoE Heal
            [ReplaceSkill(SGE.Prognosis)]
            [ConflictingCombos(SGE_RhizoFeature, SGE_DruoTauroFeature)]
            [CustomComboInfo("Sage Simple AoE Heal Feature", "Changes Prognosis. Customize your AoE healing to your liking", SGE.JobID, 500)]
            SGE_AoE_HealFeature = 14500,
            
                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Physis Option", "Adds Physis.", SGE.JobID, 510)]
                SGE_AoE_Heal_Physis = 14510,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Eukrasian Prognosis Option", "Prognosis becomes Eukrasian Prognosis if the shield is not applied.", SGE.JobID, 520)]
                SGE_AoE_Heal_EkPrognosis = 14520,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Holos Option", "Adds Holos.", SGE.JobID, 530)]
                SGE_AoE_Heal_Holos = 14530,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Panhaima Option", "Adds Panhaima.", SGE.JobID, 540)]
                SGE_AoE_Heal_Panhaima = 14540,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Pepsis Option", "Triggers Pepsis if a shield is present.", SGE.JobID, 550)]
                SGE_AoE_Heal_Pepsis = 14550,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Ixochole Option", "Adds Ixochole", SGE.JobID, 560)]
                SGE_AoE_Heal_Ixochole = 14560,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Kerachole Option", "Adds Kerachole", SGE.JobID, 570)]
                SGE_AoE_Heal_Kerachole = 14570,

                [ParentCombo(SGE_AoE_HealFeature)]
                [CustomComboInfo("Rhizomata Option", "Adds Rhizomata when Addersgall is 0", SGE.JobID, 580)]
                SGE_AoE_Heal_Rhizomata = 14580,
            #endregion

            #region Misc Healing
            [ReplaceSkill(SGE.Taurochole, SGE.Druochole, SGE.Ixochole, SGE.Kerachole)]
            [CustomComboInfo("Rhizomata Feature", "Replaces Addersgall skills with Rhizomata when empty.", SGE.JobID, 600)]
            SGE_RhizoFeature = 14600,

            [ReplaceSkill(SGE.Druochole)]
            [CustomComboInfo("Druochole to Taurochole Feature", "Upgrades Druochole to Taurochole when Taurochole is available", SGE.JobID, 700)]
            SGE_DruoTauroFeature = 14700,

            [ReplaceSkill(SGE.Pneuma)]
            [CustomComboInfo("Zoe Buff for Pneuma Feature", "Places Zoe ontop of Pneuma when both actions are on cooldown", SGE.JobID, 701)]//Temporary to keep the order
            SGE_ZoePneumaFeature = 141000,
            #endregion

            #region Utility
            [ReplaceSkill(All.Swiftcast)]
            [ConflictingCombos(ALL_Healer_Raise)]
            [CustomComboInfo("Swiftcast Raise Feature", "Changes Swiftcast to Egeiro while Swiftcast is on cooldown.", SGE.JobID, 800)]
            SGE_RaiseFeature = 14800,

            [ReplaceSkill(SGE.Soteria)]
            [CustomComboInfo("Soteria to Kardia Feature", "Soteria turns into Kardia when not active or Soteria is on-cooldown.", SGE.JobID, 900)]
            SGE_KardiaFeature = 14900,
            #endregion

        #endregion
        // ====================================================================================
        #region SAMURAI

        [ReplaceSkill(SAM.Kasha,SAM.Gekko,SAM.Yukikaze)]
        [CustomComboInfo("Samurai Overcap Feature", "Adds Shinten onto main combo when Kenki is at the selected amount or more", SAM.JobID, 0, "Wink emoji Overcap Feature 1", "Kinky.")]
        SamuraiOvercapFeature = 15001,

        [ReplaceSkill(SAM.Mangetsu,SAM.Oka)]
        [CustomComboInfo("Samurai AoE Overcap Feature", "Adds Kyuten onto main AoE combos when Kenki is at the selected amount or more", SAM.JobID, 0, "Wink emoji Overcap Feature 3", "Kinkier")]
        SamuraiOvercapFeatureAoe = 15002,

        //Main Combo Features
        [ReplaceSkill(SAM.Gekko)]
        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain.\nIf all sub options are selected will turn into a full one button rotation (Simple Samurai)", SAM.JobID, 0, "Geico Combo", "Fifteen minutes could save you 15% or more on car insurance!")]
        SamuraiGekkoCombo = 15003,

            #region Gekko Combo
            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("Enpi Uptime Feature", "Replace Main Combo with Enpi when you are out of range.", SAM.JobID, 0)]
            SamuraiRangedUptimeFeature = 15004,

            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("Yukikaze Combo on Main Combo", "Adds Yukikaze Combo to Main Combo. Will add Yukikaze during Meikyo Shisui as well", SAM.JobID, 0)]
            YukionST = 15005,

            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("Kasha Combo on Main Combo", "Adds Kasha Combo to Main Combo. Will add Kasha during Meikyo Shisui as well.", SAM.JobID, 0)]
            KashaonST = 15006,

            [ConflictingCombos(SamuraiYatenFeature)]
            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("Level 90 Samurai Opener", "Adds the Level 90 Opener to the Main Combo.\nOpener triggered by using Meikyo Shisui before combat. If you have any Sen, Hagakure will be used to clear them.\nWill work at any levels of Kenki, requires 2 charges of Meikyo Shisui and all CDs ready. If conditions aren't met it will skip into the regular rotation. \nIf the Opener is interrupted, it will exit the opener via a Goken and a Kaeshi: Goken at the end or via the last Yukikaze. If the latter, CDs will be used on cooldown regardless of burst options.", SAM.JobID, 0)]
            SamuraiOpenerFeature = 15007,

            [ConflictingCombos(SamuraiYatenFeature)]
            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("Filler Combo Feature", "Adds selected Filler Combos to Main Combo at the appropriate time.\nChoose Skill Speed tier with Fuka buff below.\nWill disable if you die or if you don't activate the opener.", SAM.JobID, 0)]
            SamuraiFillersonMainCombo = 15008,

            [ParentCombo(SamuraiGekkoCombo)]
            [CustomComboInfo("CDs on Main Combo", "Collection of CD features on Main Combo.", SAM.JobID, 0)]
            SamuraiGekkoCDs = 15099,

                #region CDs on Main Combo
                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Ikishoten on Main Combo", "Adds Ikishoten to Gekko and Mangetsu combos when at or below 50 Kenki.\nWill dump Kenki at 10 seconds left to allow Ikishoten to be used.", SAM.JobID, 0, "Gauge pls", "You heard me. Gauge pls")]
                SamuraiIkishotenonmaincombo = 15009,

                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Iaijutsu on Main Combo", "Adds Midare: Setsugekka, Higanbana, and Kaeshi: Setsugekka when ready and when you're not moving to Main Combo.", SAM.JobID, 0)]
                IaijutsuSTFeature = 15010,

                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Ogi Namikiri on Main Combo", "Ogi Namikiri and Kaeshi: Namikiri when ready and when you're not moving to Main Combo.", SAM.JobID, 0)]
                SamuraiOgiNamikiriSTFeature = 15011,

                    #region Ogi Namikiri on Main Combo
                    [ParentCombo(SamuraiOgiNamikiriSTFeature)]
                    [CustomComboInfo("Ogi Namikiri Burst Feature", "Saves Ogi Namikiri for even minute burst windows.\nIf you don't activate the opener or die, Ogi Namikiri will instead be used on CD.", SAM.JobID, 0)]
                    OgiNamikiriinBurstFeature = 15012,
                    #endregion

                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Meikyo Shisui on Main Combo", "Adds Meikyo Shisui to Main Combo when off cooldown.", SAM.JobID, 0)]
                MeikyoShisuionST = 15013,

                    #region Meikyo Shisui on Main Combo
                    [ParentCombo(MeikyoShisuionST)]
                    [CustomComboInfo("Meikyo Shisui Burst Feature", "Saves Meikyo Shisui for burst windows.\nIf you don't activate the opener or die, Meikyo Shisui will instead be used on CD.", SAM.JobID, 0)]
                    MeikyoShisuiBurstFeature = 15014,
                    #endregion

                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Shoha on Main Combo", "Adds Shoha to Main Combo when there are three meditation stacks.", SAM.JobID, 0)]
                SamuraiShohaSTFeature = 15015,

                [ConflictingCombos(SamuraiSeneiFeature)]
                [ParentCombo(SamuraiGekkoCDs)]
                [CustomComboInfo("Senei on Main Combo", "Adds Senei to Main Combo when off cooldown and above 25 Kenki.", SAM.JobID, 0)]
                SeneionST = 15016,
                #endregion

            [ParentCombo(SeneionST)]
            [CustomComboInfo("Senei Burst Feature", "Saves Senei for even minute burst windows.\nIf you don't activate the opener or die, Senei will instead be used on CD.", SAM.JobID, 0)]
            SeneiBurstFeature = 15017,
        #endregion

        [ReplaceSkill(SAM.Yukikaze)]
        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain.", SAM.JobID, 0, "Yakuza Combo", "Gang affiliation? Surely not.")]
        SamuraiYukikazeCombo = 15018,

        [ReplaceSkill(SAM.Kasha)]
        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain.", SAM.JobID, 0, "Cashman Combo", "Dolla dolla bill, y'all")]
        SamuraiKashaCombo = 15019,

        //AOE Combo Features
        [ReplaceSkill(SAM.Mangetsu)]
        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain.\nIf all sub options are toggled will turn into a full one button AOE rotation.", SAM.JobID, 0, "Mangetout Combo", "EAT IT ALL!")]
        SamuraiMangetsuCombo = 15020,

            #region Mangetsu Combo
            [ParentCombo(SamuraiMangetsuCombo)]
            [ConflictingCombos(SamTwoTargetFeature)]
            [CustomComboInfo("Oka to Mangetsu Combo", "Adds Oka combo after Mangetsu combo loop. \n Will add Oka if needed during Meikyo Shisui.", SAM.JobID, 0)]
            SamuraiOkaFeature = 15021,

            [ParentCombo(SamuraiMangetsuCombo)]
            [CustomComboInfo("Iaijutsu on Mangetsu Combo", "Adds Tenka Goken and Midare: Setsugekka and their relevant Kaeshi when ready and when you're not moving to Mangetsu combo.", SAM.JobID, 0)]
            TenkaGokenAOEFeature = 15022,

            [ParentCombo(SamuraiMangetsuCombo)]
            [CustomComboInfo("Ogi Namikiri on Mangetsu Combo", "Adds Ogi Namikiri and Kaeshi: Namikiri when ready and when you're not moving to Mangetsu combo.", SAM.JobID, 0)]
            SamuraiOgiNamikiriAOEFeature = 15023,

            [ParentCombo(SamuraiMangetsuCombo)]
            [CustomComboInfo("Shoha 2 on Mangetsu Combo", "Adds Shoha 2 when you have 3 meditation stacks to Mangetsu combo.", SAM.JobID, 0)]
            SamuraiShoha2AOEFeature = 15024,

            [ConflictingCombos(SamuraiGurenFeature)]
            [ParentCombo(SamuraiMangetsuCombo)]
            [CustomComboInfo("Guren on Mangetsu Combo", "Adds Guren when it's off CD and you have 25 Kenki to Mangetsu combo.", SAM.JobID, 0)]
            SamuraiGurenAOEFeature = 15025,
        #endregion


        [ReplaceSkill(SAM.Oka)]
        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain.", SAM.JobID, 0, "Okeh Combo", "Okeh")]
        SamuraiOkaCombo = 15026,

            #region Oka Combo
            [ParentCombo(SamuraiOkaCombo)]
            [ConflictingCombos(SamuraiOkaFeature)]
            [CustomComboInfo("Oka Two Target Rotation Feature", "Adds the Yukikaze Combo, Mangetsu Combo, Senei, Shinten, and Shoha to Oka Combo.\nUsed for two targets only and when 86 and above.", SAM.JobID, 0)]
            SamTwoTargetFeature = 150261,
        #endregion

        //CD Features
        [ReplaceSkill(SAM.MeikyoShisui)]
        [CustomComboInfo("Jinpu/Shifu Feature", "Replace Meikyo Shisui with Jinpu, Shifu, and Yukikaze depending on what is needed.", SAM.JobID, 0, "Jumpup/Sitdown", "Work those glutes.")]
        SamuraiJinpuShifuFeature = 15027,

        //Iaijutsu Features
        [ReplaceSkill(SAM.Iaijutsu)]
        [CustomComboInfo("Iaijutsu Features", "Collection of Iaijutsu Features.", SAM.JobID, 0, "", "You don't know the difference between this one and that one?")]
        SamuraiIaijutsuFeature = 15028,

            #region Iaijutsu Features
            [ParentCombo(SamuraiIaijutsuFeature)]
            [CustomComboInfo("Iaijutsu to Tsubame-Gaeshi", "Replace Iaijutsu with  Tsubame-gaeshi when Sen is empty.", SAM.JobID, 0, "", "You don't know the difference between this one and that one?")]
            SamuraiIaijutsuTsubameGaeshiFeature = 15029,

            [ParentCombo(SamuraiIaijutsuFeature)]
            [CustomComboInfo("Iaijutsu to Shoha", "Replace Iaijutsu with Shoha when meditation is 3.", SAM.JobID, 0, "", "Don't worry, neither do we.")]
            SamuraiIaijutsuShohaFeature = 15030,

            [ParentCombo(SamuraiIaijutsuFeature)]
            [CustomComboInfo("Iaijutsu to Ogi Namikiri", "Replace Iaijutsu with Ogi Namikiri and Kaeshi: Namikiri when buffed with Ogi Namikiri Ready.", SAM.JobID, 0, "", "Don't worry, neither do we.")]
            SamuraiIaijutsuOgiFeature = 15031,
        #endregion

        //Shinten Features
        [ReplaceSkill(SAM.Shinten)]
        [CustomComboInfo("Shinten to Shoha", "Replace Hissatsu: Shinten with Shoha when Meditation is full.", SAM.JobID, 0, "", "Kicks you in the shins if Shoha is on cooldown")]
        SamuraiShohaFeature = 15032,

            #region Shinten to Shoha
            [ConflictingCombos(SeneionST)]
            [ParentCombo(SamuraiShohaFeature)]
            [CustomComboInfo("Shinten to Senei", "Replace Hissatsu: Shinten with Senei when its cooldown is up.", SAM.JobID, 0, "", "Kicks you in the shins if Senei is on cooldown")]
            SamuraiSeneiFeature = 15033,
        #endregion

        //Kyuten Features
        [ReplaceSkill(SAM.Kyuten)]
        [CustomComboInfo("Kyuten to Shoha II", "Replace Hissatsu: Kyuten with Shoha II when Meditation is full.", SAM.JobID, 0, "", "Hey Kyutie 2, Electric Boogaloo!")]
        SamuraiShoha2Feature = 15034,

            #region Kyuten to Shoha II
            [ConflictingCombos(SamuraiGurenAOEFeature)]
            [ParentCombo(SamuraiShoha2Feature)]
            [CustomComboInfo("Kyuten to Guren", "Replace Hissatsu: Kyuten with Guren when its cooldown is up.", SAM.JobID, 0, "", "Hey Kyutie!")]
            SamuraiGurenFeature = 15035,
            #endregion

        [ConflictingCombos(SamuraiOpenerFeature, SamuraiFillersonMainCombo)]
        [ReplaceSkill(SAM.Gyoten)]
        [CustomComboInfo("Gyoten Feature", "Hissatsu: Gyoten becomes Yaten/Gyoten depending on the distance from your target.", SAM.JobID, 0, "Gyoza Feature", "Mm, tasty.")]
        SamuraiYatenFeature = 15036,

        [ReplaceSkill(SAM.Ikishoten)]
        [CustomComboInfo("Ikishoten Namikiri Feature", "Replace Ikishoten with Ogi Namikiri and then Kaeshi Namikiri when available.\nIf you have full Meditation stacks, Ikishoten becomes Shoha while you have Ogi Namikiri ready.", SAM.JobID, 0, "Sticky-icky-shoten", "Wait, you guys use meditation?")]
        SamuraiIkishotenNamikiriFeature = 15037,

        [ReplaceSkill(SAM.Gekko, SAM.Yukikaze, SAM.Kasha)]
        [CustomComboInfo("True North Feature", "Adds True North on all ST Combos if Meikyo Shisui's buff is on you.", SAM.JobID, 0)]
        SamuraiTrueNorthFeature = 15038,


        #endregion
        // ====================================================================================
        #region SCHOLAR

            //SCHOLAR_FEATURE_NUMBERING
            //Numbering Scheme: 16[Feature][Option][Sub-Option]
            //Example: 16110 (Feature Number 1, Option 1, no suboption)
            //New features should be added to the appropriate sections.

            #region SCHOLAR_DPS

            [ReplaceSkill(SCH.Ruin1, SCH.Broil1, SCH.Broil2, SCH.Broil3, SCH.Broil4)]
            [CustomComboInfo("Single Target DPS Feature", "Replace Ruin I / Broils with options below", SCH.JobID, 100)]
            SCH_ST_BroilFeature = 16100,

                    [ParentCombo(SCH_ST_BroilFeature)]
                    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value:", SCH.JobID, 110)]
                    SCH_ST_Broil_Lucid = 16110,

                    [ParentCombo(SCH_ST_BroilFeature)]
                    [ConflictingCombos(SCH_ST_Broil_BioToT)]
                    [CustomComboInfo("Chain Stratagem Weave Option", "Adds Chain Stratagem on Cooldown with overlap protection", SCH.JobID, 120)]
                    SCH_ST_Broil_ChainStratagem = 16120,

                    [ParentCombo(SCH_ST_BroilFeature)]
                    [CustomComboInfo("Aetherflow Weave Feature", "Use Aetherflow when out of aetherflow stacks", SCH.JobID, 130)]
                    SCH_ST_Broil_Aetherflow = 16130,

                    [ParentCombo(SCH_ST_BroilFeature)]
                    [CustomComboInfo("Ruin II Moving Feature", "Use Ruin 2 when you have to move", SCH.JobID, 140)]
                    SCH_ST_Broil_Ruin2Movement = 16140,

                    [ParentCombo(SCH_ST_BroilFeature)]
                    [CustomComboInfo("Bio / Biolysis Option", "Automatic DoT Uptime", SCH.JobID, 150)]
                    SCH_ST_Broil_Bio = 16150,

                        [ParentCombo(SCH_ST_Broil_Bio)]
                        [ConflictingCombos(SCH_ST_Broil_BioToT)]
                        [CustomComboInfo("Enemy HP Limiter Options", "Stop using Bio when Enemy HP values match below:", SCH.JobID, 151)]
                        SCH_ST_Broil_BioHPPer = 16151,

                        [ParentCombo(SCH_ST_Broil_Bio)]
                        [ConflictingCombos(SCH_ST_Broil_BioHPPer)]
                        [CustomComboInfo("Target of Target Bio Option", "Target of Target checking for Bio", SCH.JobID, 152)]
                        SCH_ST_Broil_BioToT = 16152,
            #endregion

            #region SCHOLAR HEALING

            [ReplaceSkill(SCH.FeyBlessing)]
            [CustomComboInfo("Fey Blessing to Seraph's Consolation Feature", "Change Fey Blessing into Consolation when Seraph is out.", SCH.JobID, 210, "", "Stupid little fairy thing")]
            SCH_ConsolationFeature = 16210,

            #endregion

            #region SCHOLAR UTILITIES
            [ReplaceSkill(SCH.EnergyDrain, SCH.Lustrate, SCH.SacredSoil, SCH.Indomitability, SCH.Excogitation)]
            [CustomComboInfo("Aetherflow Helper Feature", "Change Aetherflow-using skills to Aetherflow, Recitation, or Dissipation as selected", SCH.JobID, 300, "", "Stop trying to pretend you're a SMN. You're not fooling anyone")]
            SCH_AetherflowFeature = 16300,

                    [ParentCombo(SCH_AetherflowFeature)]
                    [CustomComboInfo("Recitation Option", "Prioritizes Recitation usage on Excogitation or Indominability", SCH.JobID, 310)]
                    SCH_Aetherflow_Recite = 16310,

                        [ParentCombo(SCH_Aetherflow_Recite)]
                        [CustomComboInfo("On Excogitation Option", "", SCH.JobID, 311)]
                        SCH_Aetherflow_Recite_Excog = 16311,

                        [ParentCombo(SCH_Aetherflow_Recite)]
                        [CustomComboInfo("On Indominability Option", "", SCH.JobID, 312)]
                        SCH_Aetherflow_Recite_Indom = 16312,

                    [ParentCombo(SCH_AetherflowFeature)]
                    [CustomComboInfo("Dissipation Option", "If Aetherflow itself is on cooldown, show Dissipation instead", SCH.JobID, 320, "", "Oh wow look at that that one...it looks so delicious")]
                    SCH_Aetherflow_Dissipation = 16320,

            [ReplaceSkill(All.Swiftcast)]
            [ConflictingCombos(ALL_Healer_Raise)]
            [CustomComboInfo("Swiftcast Raise Combo Feature", "Changes Swiftcast to Resurrection while Swiftcast is on cooldown", SCH.JobID, 400, "", "BRING OUT YOUR DEAD")]
            SCH_RaiseFeature = 16400,

            [ReplaceSkill(SCH.WhisperingDawn, SCH.FeyBlessing, SCH.FeyBlessing, SCH.Aetherpact, SCH.Dissipation)]
            [CustomComboInfo("Fairy Feature", "Change all fairy actions into Fairy Summons if you do not have a fairy summoned.", SCH.JobID, 500, "", "You're really gonna forget? Really?")]
            SCH_FairyFeature = 16500,
            #endregion

        #endregion
        // ====================================================================================
        #region SUMMONER

        [ReplaceSkill(SMN.Ruin, SMN.Ruin2)]
        [CustomComboInfo("Enable Single Target Combo Features", "Enables features tied to Ruin, or Ruin II.\nIf all sub options are toggled will turn into a full one button rotation (Simple Summoner)\nRuin III is kept untouched for mobility.", SMN.JobID, 0, "Ruin 7 Feature", "Ruination is come... again?")]
        SummonerMainComboFeature = 17000,

        [ReplaceSkill(SMN.Tridisaster)]
        [CustomComboInfo("Enable AOE Combo Features", "Enables features tied to Tridisaster.\nIf all sub options are toggled will turn into a full one button rotation (Simple AOE)", SMN.JobID, 0, "", "Can't deal with dungeons on your own? Fear not.")]
        SummonerAOEComboFeature = 17001,

        [ParentCombo(SummonerDemiSummonsFeature)]
        [CustomComboInfo("Demi Attacks on Main Combo", "Adds Astral Flow to the Main Combo.", SMN.JobID, 0, "Demi Dingus Feature", "Can't tell the difference between a Bahamut and a Phoenix?\nWe know.")]
        SummonerSingleTargetDemiFeature = 17002,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("AOE Demi Attacks on AOE Combo", "Adds Astral Flare/Brand of Purgatory to the AOE Combo.", SMN.JobID, 4, "BRRRR", "Upgrade!")]
        SummonerAOEDemiFeature = 17003,

        [ParentCombo(EgisOnRuinFeature)]
        [CustomComboInfo("Gemshine on Main Combo", "Adds Egi Attacks (Gemshine) to Main Combo.", SMN.JobID, 1, "Eggy-bread", "No idea when you're in burst phase?\nHint: It's all the time, really")]
        SummonerEgiAttacksFeature = 17004,

        [CustomComboInfo("Garuda Slipstream Feature", "Adds Slipstream on RuinI/Ruin II/Tri-disaster.", SMN.JobID, 0, "Slipstream", "2 Fast 2 Furious")]
        SummonerGarudaUniqueFeature = 17005,

        [CustomComboInfo("Ifrit Cyclone Feature", "Adds Crimson Cyclone/Crimson Strike on RuinI/Ruin II/Tri-disaster.", SMN.JobID, 0, "Fists of Fury", "Show MNK how it's done, will ya?")]
        SummonerIfritUniqueFeature = 17006,

        [CustomComboInfo("Titan Mountain Buster Feature", "Adds Mountain Buster on RuinI/Ruin II/Tri-disaster.", SMN.JobID, 0, "Mountain, BUSTA", "Bring the mountain to Mohammed, as they say")]
        SummonerTitanUniqueFeature = 17007,

        [ReplaceSkill(SMN.Fester)]
        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when out of Aetherflow stacks.", SMN.JobID, 0, "Festering", "Festering? Go take a shower, bro")]
        SummonerEDFesterCombo = 17008,

        [ReplaceSkill(SMN.Painflare)]
        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Siphon when out of Aetherflow stacks.", SMN.JobID, 0, "Old age", "I sometimes get a painflare in my middle-back, too.")]
        SummonerESPainflareCombo = 17009,

        // BONUS TWEAKS
        [CustomComboInfo("Carbuncle Reminder Feature", "Reminds you to summon Carbuncle by replacing most actions with Summon Carbuncle.", SMN.JobID, 0)]
        SummonerCarbuncleSummonFeature = 17010,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Ruin 4 on Main Combo", "Adds Ruin4 on Main Combo when there are currently no summons active.", SMN.JobID, 1, "Ruin -> Ruin -> Ruin", "Ruin this, ruin that. Can't you see I'm busy ruining the plugin?!")]
        SummonerRuin4ToRuin3Feature = 17011,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Ruin 4 On Tri-disaster Feature", "Adds Ruin4 on AOE Combo when there are currently no summons active.", SMN.JobID, 0, "", "More Ruin this, more ruin that! Now in sharing size!")]
        SummonerRuin4ToTridisasterFeature = 17012,

        [ParentCombo(SummonerEDFesterCombo)]
        [CustomComboInfo("Ruin IV Fester/PainFlare Feature", "Change Fester/PainFlare into Ruin4 when out of Aetherflow stacks, ED/ES is on cooldown, and Ruin IV is up.", SMN.JobID, 0, "Festering Painflare", "Just take some Advil for that, or see the doc?")]
        SummonerFesterPainflareRuinFeature = 17013,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Energy Drain/Fester on Main Combo", "Adds ED/Fester to Ruin. Will use on cooldown.", SMN.JobID, 1)]
        SummonerEDMainComboFeature = 17014,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Egi Summons combo Features", "Various options for egis.", SMN.JobID, 1)]
        EgisOnRuinFeature = 17015,

        [ParentCombo(EgisOnRuinFeature)]
        [CustomComboInfo("Egi Summons on RuinI/II", "Adds Egi Summons to RuinI/II.\nChoose options below for order of Egis summoned.", SMN.JobID, 1)]
        SummonerEgiSummonsonMainFeature = 17016,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Energy Siphon/Painflare on Main Combo", "Adds Energy Siphon/Painflare to AOE Combo", SMN.JobID, 1, "", "We'll play the game for you. Shush, now")]
        SummonerESAOEFeature = 17017,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Searing Light on Ruin", "Adds Searing Light to the Main Combo and will be used on cooldown.", SMN.JobID, 1, "My eyes!", "I can't see!")]
        SearingLightonRuinFeature = 17018,

        [ParentCombo(SearingLightonRuinFeature)]
        [CustomComboInfo("Searing Light Burst Option", "Casts Searing Light only during Bahamut/Phoenix Phase.\nChoose which phase to burst in under 'Burst Phase Choice' option.\nNot recommended for SpS Builds.", SMN.JobID, 0, "My eyes!", "I can't see!")]
        SummonerSearingLightBurstOption = 170181,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Searing Light AoE Option", "Adds Searing Light to the AOE Combo.", SMN.JobID, 2, "Our Eyes!", "Yay, we're all legally blind!")]
        BuffOnSimpleAoESummoner = 17019,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Demi Summons on Main Combo", "Adds Demi Summons to the Main Combo.", SMN.JobID, 1, "Chad Kroeger Demi Feature", "This is how, you remind me, of what I really am")]
        SummonerDemiSummonsFeature = 17020,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Demi Summons AOE Combo", "Adds Demi Summons to the AOE Combo.", SMN.JobID, 3, "Nickelback Demi Feature", "Oh fuck, the whole band is here! Run!")]
        SummonerDemiAoESummonsFeature = 17021,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Egi Summons on AOE Combo", "Adds Egi Summons to AOE Combo", SMN.JobID, 5, "Nickelback Demi Feature", "Oh fuck, the whole band is here! Run!")]
        EgisOnAOEFeature = 17022,

        [ParentCombo(EgisOnRuinFeature)]
        [CustomComboInfo("Swiftcast Egi Ability Option", "Swiftcasts during the selected Primal Summon.", SMN.JobID, 1, "", "")]
        SummonerSwiftcastEgiFeature = 17023,

        [CustomComboInfo("Astral Flow/Enkindle on Bahamut/Phoenix", "Adds Astral Flow and Enkindle to Bahamut/Phoenix.", SMN.JobID, 1, "", "")]
        SummonerPrimalAbilitiesFeature = 17024,

        [ParentCombo(SummonerEDMainComboFeature)]
        [CustomComboInfo("Pooled Festers Feature", "Pools Festers/Energy Drain to use under Searing Light and in Bahamut/Phoenix Phase.\nChoose which phase to burst in under 'Burst Phase Choice' option.", SMN.JobID, 0)]
        SummonerEDPoolonMainFeature = 17025,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Precious Brilliance on AOE Combo", "Adds Egi attacks (Precious Brilliance) to AOE Combo.", SMN.JobID, 6)]
        SummonerEgiAttacksAOEFeature = 17026,

        [ConflictingCombos(ALL_Caster_Raise)]
        [CustomComboInfo("SMN Alternative Raise Feature", "Changes Swiftcast to Raise when on cooldown", SMN.JobID, 0, "Shittier RezMage", "Just play RDM oh my gawwddddddddddddd")]
        SummonerRaiseFeature = 17027,

        [ParentCombo(SummonerDemiSummonsFeature)]
        [CustomComboInfo("Rekindle on Main Combo option", "Adds Rekindle to the Main Combo.", SMN.JobID, 0, "Phoenix Dingus Feature", "You only need to worry about healing yourself.\nIts okay.")]
        SummonerSingleTargetRekindleOption = 17028,

        [ParentCombo(SummonerAOEComboFeature)]
        [CustomComboInfo("Rekindle on AOE Combo option", "Adds Rekindle to the AOE Combo.", SMN.JobID, 0, "Phoenix Dingus Feature", "You only need to worry about healing yourself.\nIts okay.")]
        SummonerAOETargetRekindleOption = 17029,

        [ReplaceSkill(SMN.Ruin4)]
        [CustomComboInfo("Ruin III Mobility Feature", "Puts Ruin III on Ruin IV when you don't have Further Ruin.", SMN.JobID, 0, "Yo Dawg I Heard You Like Ruin Feature", "Ruin while you Ruin")]
        SummonerSpecialRuinFeature = 17030,

        [ReplaceSkill(SMN.Ruin, SMN.Ruin2)]
        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the Main Combo when below set MP value.", SMN.JobID, 0, "", "")]
        SMNLucidDreamingFeature = 17031,

        [ParentCombo(SummonerMainComboFeature)]
        [CustomComboInfo("Burst Phase Choice", "Chooses which phase to burst in for all relevant burst features. Festers and Searing Lights will only be used during Bahamut/Phoenix windows.", SMN.JobID, 0, "", "")]
        SummonerPrimalBurstChoice = 17032,

        [CustomComboInfo("Egi Abilities on Egi Summons", "Adds Egi Abilities (Astral Flow) to Egi Summons when ready.\nEgi Abilities will appear on their respective Egi Summon Ability, as well as, Titan.", SMN.JobID, 0, "", "")]
        SummonerAstralFlowonSummonsFeature = 17034,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [ReplaceSkill(WAR.StormsEye)]
        [CustomComboInfo("Storms Path Combo", "All in one main combo feature adds Storm's Eye/Path. \nIf all sub options and Fell Cleave/Decimate Options are toggled will turn into a full one button rotation (Simple Warrior)", WAR.JobID, 0, "", "Follow the yellow-brick road.")]
        WarriorStormsPathCombo = 18000,

        [ReplaceSkill(WAR.StormsEye)]
        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, 0, "", "Ow! My fucking eye!")]
        WarriorStormsEyeCombo = 18001,

        [ReplaceSkill(WAR.Overpower)]
        [CustomComboInfo("Overpower Combo", "Add combos to Overpower", WAR.JobID, 0, "Underpower", "Bet you wish you had damage like DRK right now, huh")]
        WarriorMythrilTempestCombo = 18002,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Warrior Gauge Overcap Feature", "Replace Single target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate beast gauge", WAR.JobID, 0, "", "Taming the beast... for now.")]
        WarriorGaugeOvercapFeature = 18003,

        [ReplaceSkill(WAR.NascentFlash)]
        [CustomComboInfo("Nascent Flash Feature", "Replace Nascent Flash with Raw intuition when level synced below 76", WAR.JobID, 0, "Nasty-ass Flash", "Jeez. Keep it to yourself.")]
        WarriorNascentFlashFeature = 18005,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Upheaval Feature", "Adds Upheaval into maincombo if you have Surging Tempest", WAR.JobID, 0, "", "I use this feature when I'm moving house.")]
        WarriorUpheavalMainComboFeature = 18007,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Primal Rend Feature", "Replace Inner Beast and Steel Cyclone with Primal Rend when available (Also added onto Main AoE combo)", WAR.JobID, 0, "", "Going back to our roots. Let's get Primal!")]
        WarriorPrimalRendFeature = 18008,

        [ParentCombo(WarriorMythrilTempestCombo)]
        [CustomComboInfo("Orogeny Feature", "Adds Orogeny onto main AoE combo when you are buffed with Surging Tempest", WAR.JobID, 0, "Orange-y feature", "Orange flavour. Mm.")]
        WarriorOrogenyFeature = 18009,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Fell Cleave/Decimate Option", "Adds Fell Cleave to main combo when gauge is at 50 or more and adds Decimate to the AoE combo .\nWill use Inner Chaos/Chaotic Cyclone if Infuriate is used and Fell Cleave/Steel Cyclone during Inner Release.\nWill begin pooling resources when Inner Release is under 30s", WAR.JobID, 0, "", "MORE CLEAVE!")]
        WarriorSpenderOption = 18011,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Onslaught Feature", "Adds Onslaught to Storm's Path feature combo if you are under Surging Tempest Buff", WAR.JobID, 0, "", "Onslaught! Full Power!")]
        WarriorOnslaughtFeature = 18012,

        [ParentCombo(WarriorMythrilTempestCombo)]
        [CustomComboInfo("Infuriate AOE Feature", "Adds Infuriate to AOE Combo when gauge is below 50 and not under Inner Release.", WAR.JobID, 0)]
        WarriorInfuriateOnAOE = 18013,

        [ParentCombo(WarriorMythrilTempestCombo)]
        [CustomComboInfo("Inner Release AOE Feature", "Adds Inner Release to Storm's Path Combo.", WAR.JobID, 0)]
        WarriorIRonAOE = 18014,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Tomahawk Uptime Feature", "Replace Storm's Path Combo Feature with Tomahawk when you are out of range.", WAR.JobID, 0, "Tomahawk!", "You heard me! Tomahawk! Ka-chow!")]
        WARRangedUptimeFeature = 18016,

        [ReplaceSkill(WAR.FellCleave, WAR.Decimate)]
        [CustomComboInfo("Infuriate on Fell Cleave / Decimate", "Turns Fell Cleave and Decimate into Infuriate if at or under set rage value", WAR.JobID)]
        WarriorInfuriateFellCleave = 18018,

        [ReplaceSkill(WAR.InnerRelease)]
        [CustomComboInfo("Primal Rend Option", "Turns Inner Release into Primal Rend on use.", WAR.JobID)]
        WarriorPrimalRendOnInnerRelease = 18019,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Inner Release on Storm's Path", "Adds Inner Release to Storm's Path Combo.", WAR.JobID)]
        WarriorIRonST = 18020,

        [ParentCombo(WarriorStormsPathCombo)]
        [CustomComboInfo("Infuriate on Storm's Path", "Adds Infuriate to Storm's Path Combo when gauge is below 50 and not under Inner Release.", WAR.JobID)]
        WarriorInfuriateonST = 18021,

        [ParentCombo(WarriorInfuriateFellCleave)]
        [CustomComboInfo("Use Inner Release Stacks First", "Prevents the use of Infuriate while you have Inner Release stacks available.", WAR.JobID, 0, "Don't blow it all in one place.", "Save some for later.")]
        WarriorUseInnerReleaseFirst = 18022,

        [ParentCombo(WarriorPrimalRendFeature)]
        [CustomComboInfo("Primal Rend Melee Feature", "Uses Primal Rend when in the target's target ring (1 yalm) and closer otherwise will use it when buff is less than 10 seconds.", WAR.JobID, 0, "Don't blow it all in one place.", "Save some for later.")]
        WarriorPrimalRendCloseRangeFeature = 18023,

        [ParentCombo(WarriorOnslaughtFeature)]
        [CustomComboInfo("Melee Onslaught Option", "Uses Onslaught when under Surging Tempest and in the target ring (1 yalm).\nWill use as many stacks as selected in the above slider.", WAR.JobID, 0, "", "")]
        WarriorMeleeOnslaughtOption = 18024,
        


        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
        [CustomComboInfo("CDs on Glare/Stone", "Collection of CDs and spell features on Glare/Stone.", WHM.JobID, 0, "Weak", "WHM DPS rotation too much?")]
        WHMCDsonMainComboGroup = 19099,

        [ReplaceSkill(WHM.AfflatusSolace)]
        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, 0, "Misery", "I'd be miserable too if this were one of my DPS options.")]
        WhiteMageSolaceMiseryFeature = 19000,

        [ReplaceSkill(WHM.AfflatusRapture)]
        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, 0, "Misery, but with freinds", "Let's cry together!")]
        WhiteMageRaptureMiseryFeature = 19001,

        [ReplaceSkill(WHM.Cure2)]
        [CustomComboInfo("Cure 2 to Cure Level Sync", "Changes Cure 2 to Cure when below level 30 in synced content.", WHM.JobID, 0, "Weenie Cure", "Bet you forgot Cure 1 existed for a sec, huh")]
        WhiteMageCureFeature = 19002,

        [ReplaceSkill(WHM.Cure2)]
        [CustomComboInfo("Afflatus Feature", "Changes Cure 2 into Afflatus Solace, and Medica into Afflatus Rapture, when lilies are up.", WHM.JobID, 0, "Inflatus Feature", "Pumps you full of air. Boing!")]
        WhiteMageAfflatusFeature = 19003,

        [ReplaceSkill(All.Swiftcast)]
        [ConflictingCombos(ALL_Healer_Raise)]
        [CustomComboInfo("WHM Alternative Raise Feature", "Changes Swiftcast to Raise", WHM.JobID, 0, "What you're really here for", "You're the best at this. You got this.")]
        WHMRaiseFeature = 19004,

        [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the DPS feature when below set MP value.", WHM.JobID, 0, "Dream within a Dream", "Awake, yet wholly asleep")]
        WHMLucidDreamingFeature = 19006,

        [ReplaceSkill(WHM.Medica2)]
        [CustomComboInfo("Medica Feature", "Replaces Medica2 whenever you are under Medica2 regen with Medica1", WHM.JobID, 0, "Big Brain AoE Heals", "God bless us all, eh")]
        WHMMedicaFeature = 19007,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Presence Of Mind Feature", "Adds Presence of mind as oGCD onto main DPS Feature(Glare3)", WHM.JobID, 0, "", "This would imply you're actually paying attention.")]
        WHMPresenceOfMindFeature = 19008,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Assize Feature", "Adds Assize as oGCD onto main DPS Feature(Glare3)", WHM.JobID, 0, "", "Size 'em up, knock 'em down")]
        WHMAssizeFeature = 19009,

        [ParentCombo(WHMMedicaFeature)]
        [CustomComboInfo("Afflatus Misery On Medica Feature", "Adds Afflatus Misery onto the Medica Feature", WHM.JobID, 0, "", "Ah, back to beinig miserable.")]
        WhiteMageAfflatusMiseryMedicaFeature = 19010,

        [ParentCombo(WHMMedicaFeature)]
        [CustomComboInfo("Afflatus Rapture On Medica Feature", "Adds Afflatus Rapture onto the Medica Feature", WHM.JobID, 0, "CRapture", "The final days are upon us!")]
        WhiteMageAfflatusRaptureMedicaFeature = 19011,

        [ReplaceSkill(WHM.Cure2)]
        [CustomComboInfo("Afflatus Misery Feature", "Changes Cure 2 into Afflatus Misery.", WHM.JobID, 0, "", "Cures? Who needs 'em?")]
        WhiteMageAfflatusMiseryCure2Feature = 19012,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Adds DoT to Glare/Stone", "Adds DoT to DPS feature and refreshes it with 3 seconds remaining.", WHM.JobID, 0, "I'm an idiot", "Yes, one serving of less DPS, please.")]
        WHMDotMainComboFeature = 19013,

        [ReplaceSkill(WHM.Raise)]
        [CustomComboInfo("Thin Air Raise Feature", "Adds Thin Air to the WHM Raise Feature/Alternative Feature", WHM.JobID, 0, "", "I can hardly breathe as it is!")]
        WHMThinAirFeature = 19014,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Lily Overcap Protection", "Adds Afflatus Rapture (AoE Heal) to glare when at 3 lilies.", WHM.JobID, 0, "Feed the blood lily!", "Burn out the bad! Burn out the bad!")]
        WHMLilyOvercapFeature = 19016,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("Adds Afflatus Misery to Glare/Stone", "Adds Afflatus Misery to Glare when Blood Lily is in full bloom.", WHM.JobID, 0, "Take this!", "**Throws Blood**")]
        WHMAfflatusMiseryOGCDFeature = 19017,

        [ParentCombo(WhiteMageAfflatusFeature)]
        [CustomComboInfo("oGCD Heals/Shields", "Adds oGCD Healing and Shields to Cure II", WHM.JobID, 0, "To benediction, or to not benediction.", "That is the question. Whether 'tis nobler... NM, you dead.")]
        WHMogcdHealsShieldsFeature = 19018,

        [ParentCombo(WHMogcdHealsShieldsFeature)]
        [CustomComboInfo("Use Tetragrammaton on oGCD.", "Only shows Tetragrammaton during oGCD weave window when HP conditions are met.", WHM.JobID, 0, "Longest word ever.", "Buffalo buffalo buffalo buffalo Buffalo buffalo buffalo.")]
        WHMTetraOnOGCDOption = 19019,

        [ParentCombo(WHMogcdHealsShieldsFeature)]
        [CustomComboInfo("Use Tetragrammaton on GCD.", "Shows Tetragrammaton when HP conditions are met.", WHM.JobID, 0, "Clip it! Clip it good!", "Clip it up! Into shape!")]
        WHMTetraOnGCDOption = 19020,

        [ParentCombo(WHMogcdHealsShieldsFeature)]
        [CustomComboInfo("Use Devine Benison on oGCD", "Only shows Devine Benison during oGCD weave window when target is not already under the effect.", WHM.JobID, 0, "oGCD Shield? Why not?!", "Tsun-tsun")]
        WHMBenisonOGCDOption = 19021,

        [ParentCombo(WHMogcdHealsShieldsFeature)]
        [CustomComboInfo("Use Devine Benison on GCD", "Shows Devine Benison when target is not already under the effect.", WHM.JobID, 0, "It's dangerous to go alone.", "Take this.")]
        WHMBenisonGCDOption = 19022,

        [ParentCombo(WHMCDsonMainComboGroup)]
        [CustomComboInfo("No Swift Opener Option", "Delays all oGCDs until after 3rd Glare 3 cast.\n>> Glare III ONLY <<", WHM.JobID, 0, "Cover me, Porkins.", "Almost there... Almost there...")]
        WHMNoSwiftOpenerOption = 19023,

        [ParentCombo(WHMogcdHealsShieldsFeature)]
        [CustomComboInfo("Prioritize oGCD Heals/Shields on Cure II when available.", "Displays oGCD Heals/Shields over Afflatus.\n(Only applies to GCD options for Tetragrammaton and Divine Benison)", WHM.JobID, 0, "That, not this.", "Shields over flowers.")]
        WHMPrioritizeoGCDHealsShields = 19024,

        [ReplaceSkill(WHM.Holy, WHM.Holy3)]
        [CustomComboInfo("CDs on Holy/Holy3", "Collection of CDs and spell features on Holy/Holy3.", WHM.JobID, 0, "Weak", "WHM DPS rotation too much?")]
        WHM_AoE_DPS_Feature = 19190,

        [ParentCombo(WHM_AoE_DPS_Feature)]
        [CustomComboInfo("Lucid Dreaming Feature", "Adds Lucid dreaming to the AoE DPS feature when below set MP value.", WHM.JobID, 0, "Dream within a Dream", "Awake, yet wholly asleep")]
        WHM_AoE_Lucid = 19191,

        [ParentCombo(WHM_AoE_DPS_Feature)]
        [CustomComboInfo("Assize Feature", "Adds Assize as oGCD to Holy/Holy3", WHM.JobID, 0, "", "Size 'em up, knock 'em down")]
        WHM_AoE_Assize = 19192,

        [ParentCombo(WHM_AoE_DPS_Feature)]
        [CustomComboInfo("Lily Overcap Protection", "Adds Afflatus Rapture (AoE Heal) to Holy/Holy3 when at 3 lilies.", WHM.JobID, 0, "Feed the blood lily!", "Burn out the bad! Burn out the bad!")]
        WHM_AoE_LilyOvercap = 19193,

        [ParentCombo(WHM_AoE_DPS_Feature)]
        [CustomComboInfo("Adds Afflatus Misery to Holy/Holy3", "Adds Afflatus Misery to Holy/Holy3 when Blood Lily is in full bloom.", WHM.JobID, 0, "Take this!", "**Throws Blood**")]
        WHM_AoE_AfflatusMisery = 19194,

        #endregion
        // ====================================================================================
        #region DOH

        // [CustomComboInfo("Placeholder", "Placeholder.", DOH.JobID)]
        // DohPlaceholder = 50001,

        #endregion
        // ====================================================================================
        #region DOL

        [CustomComboInfo("Eureka Feature", "Replace Ageless Words and Solid Reason with Wise to the World when available.", DoL.JobID)]
        DoL_Eureka = 51001,

        [CustomComboInfo("Cast / Hook Feature", "Replace Cast with Hook when fishing.", DoL.JobID)]
        FSH_CastHook = 51002,

        [CustomComboInfo("Cast / Gig Feature", "Replace Cast with Gig when underwater.", DoL.JobID)]
        FSH_CastGig = 51003,

        [CustomComboInfo("Surface Slap / Veteran Trade Feature", "Replace Surface Slap with Veteran Trade when underwater.", DoL.JobID)]
        FSH_SurfaceTrade = 51004,

        [CustomComboInfo("Prize Catch / Nature's Bounty Feature", "Replace Prize Catch with Nature's Bounty when underwater.", DoL.JobID)]
        FSH_PrizeBounty = 51005,

        [CustomComboInfo("Snagging / Salvage Feature", "Replace Snagging with Salvage when underwater.", DoL.JobID)]
        FSH_SnaggingSalvage = 51006,

        [CustomComboInfo("Cast Light / Electric Current Feature", "Replace Cast Light with Electric Current when underwater.", DoL.JobID)]
        FSH_CastLight_ElectricCurrent = 51007,

        #endregion
        // ====================================================================================
        #region PvP Combos

        //[SecretCustomCombo]
        //[CustomComboInfo("BurstShotFeature", "Adds Shadowbite/EmpyArrow/PitchPerfect(3stacks)/SideWinder(When Target is low hp)/ApexArrow when gauge is 100 all on one button combo.", BRDPvP.JobID)]
        //BurstShotFeaturePVP = 80000,

        //[SecretCustomCombo]
        //[CustomComboInfo("SongsFeature", "Replaces WanderersMinnuet and Peons song all on one button in an optimal order", BRDPvP.JobID)]
        //SongsFeaturePVP = 80001,

        //[SecretCustomCombo]
        //[CustomComboInfo("SouleaterComboFeature", "Adds EoS as oGCD onto main combo and Bloodspiller when at 50 gauge or under delirium buff.", DRKPVP.JobID)]
        //SouleaterComboFeature = 80002,

        //[SecretCustomCombo]
        //[CustomComboInfo("StalwartSoulComboFeature", "Adds FoS as oGCD onto main combo and Quietus when at 50 gauge or under delirium buff.", DRKPVP.JobID)]
        //StalwartSoulComboFeature = 80003,

        //[SecretCustomCombo]
        //[CustomComboInfo("StormsPathComboFeature", "Replaces Storm's Path Combo with FellCleave/IC when at 50 gauge or under IR", WARPVP.JobID)]
        //StormsPathComboFeature = 80004,

        //[SecretCustomCombo]
        //[CustomComboInfo("SteelCycloneFeature", "Replaces Steel Cyclone Combo with Decimate/CC when at 50 gauge or under IR", WARPVP.JobID)]
        //SteelCycloneFeature = 80005,

        //[SecretCustomCombo]
        //[CustomComboInfo("RoyalAuthorityComboFeature", "Adds HolySpirit To the main combo", PLDPVP.JobID)]
        //RoyalAuthorityComboFeature = 80006,

        //[SecretCustomCombo]
        //[CustomComboInfo("ProminenceComboFeature", "Adds HolyCircle to the main AoE Combo", PLDPVP.JobID)]
        //ProminenceComboFeature = 80007,

        //[SecretCustomCombo]
        //[CustomComboInfo("GnashingFangComboFeature", "Adds BowShock(When target is meleeRange) and Burststrike at 2 ammo gauge to the main combo", GNBPVP.JobID)]
        //SolidBarrelComboFeature = 80008,

        //[SecretCustomCombo]
        //[CustomComboInfo("DemonSlaughterComboFeature", "Adds BowShock(When target is meleeRange) and Fated Circle at 2 ammo gauge to the main AoE combo", GNBPVP.JobID)]
        //DemonSlaughterComboFeature = 80009,

        //[SecretCustomCombo]
        //[CustomComboInfo("InfernalSliceComboFeature", "Adds Gluttony/BloodStalk/Smite/EnshroudComboRotation on InfernalSliceCombo", RPRPVP.JobID)]
        //InfernalSliceComboFeature = 80012,

        //[SecretCustomCombo]
        //[CustomComboInfo("NightmareScytheComboFeature", "Adds Gluttony/GrimSwathe/Smite/EnshroudComboRotation on InfernalScytheCombo", RPRPVP.JobID)]
        //NightmareScytheComboFeature = 80013,

        //[SecretCustomCombo]
        //[CustomComboInfo("NinjaAeolianEdgePvpCombo", "Adds Cha/Assassinate/Smite on AeolianEdge combo", NINPVP.JobID)]
        //NinjaAeolianEdgePvpCombo = 80014,

        //[SecretCustomCombo]
        //[CustomComboInfo("MnkBootshinePvPFeature", "Adds Axekick/Smite/TornadoKick on main combo", MNKPVP.JobID)]
        //MnkBootshinePvPFeature = 80015,

        //[SecretCustomCombo]
        //[CustomComboInfo("BlackEnochianPVPFeature", "Enochian Stance Switcher", BLMPVP.JobID)]
        //BlackEnochianPVPFeature = 80016,

        // MCH

        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Blast Charge into an all-in-one damage button.", MCHPVP.JobID)]
        MCHBurstMode = 80010,

            #region MCH Burst Mode
            [SecretCustomCombo]
            [ParentCombo(MCHBurstMode)]
            [CustomComboInfo("Alternate Drill Mode", "Saves drill for use after wildfire.", MCHPVP.JobID)]
            MCHAltDrill = 80011,

            [SecretCustomCombo]
            [ParentCombo(MCHBurstMode)]
            [CustomComboInfo("Alternate Analysis Mode", "Uses analysis with Air Anchor instead of Chain Saw.", MCHPVP.JobID)]
            MCHAltAnalysis = 80012,
            #endregion

        // BRD
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Powerful Shot into an all-in-one damage button.", BRDPvP.JobID)]
        BRDBurstMode = 80020,

        // RDM
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Verstone/Verfire into an all-in-one damage button.", RDMPVP.JobID)]
        RDMBurstMode = 80030,

        // WAR
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Heavy Swing into an all-in-one damage button.", WARPVP.JobID)]
        WARBurstMode = 80040,

        [SecretCustomCombo]
        [ParentCombo(WARBurstMode)]
        [CustomComboInfo("Bloodwhetting Option", "Allows usage of bloodwhetting anytime, not just inbetween GCDs.", WARPVP.JobID)]
        WARBurstOption = 80041,

        [SecretCustomCombo]
        [ParentCombo(WARBurstMode)]
        [CustomComboInfo("Blota Option", "Removes blota from main combo if Primal Rend has 5 seconds or less on its cooldown.", WARPVP.JobID)]
        WARBurstBlotaOption = 80042,
        
        // NIN
        [ConflictingCombos(NINAoEBurstMode)]
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Aeolian Edge Combo into an all-in-one damage button.", NINPVP.JobID)]
        NINBurstMode = 80050,

        [ConflictingCombos(NINBurstMode)]
        [SecretCustomCombo]
        [CustomComboInfo("AoE Burst Mode", "Turns Fuma Shuriken into an all-in-one AoE damage button.", NINPVP.JobID)]
        NINAoEBurstMode = 80051,

        // SGE
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Dosis III into an all-in-one damage button.", SGE.JobID)]
        SGEBurstMode = 80060,

        // DNC
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Fountain Combo into an all-in-one damage button.", DNC.JobID)]
        DNCBurstMode = 80070,

        [SecretCustomCombo]
        [ParentCombo(DNCBurstMode)]
        [CustomComboInfo("Honing Dance Option", "Adds Honing Dance to the main combo when in melee range (for pack pushing, respects global offset).\nThis option prevents early use of Honing Ovation!\nKeep Honing Dance bound to another key if you want to end early.", DNC.JobID)]
        DNCHoningDanceOption = 80071,

        [SecretCustomCombo]
        [ParentCombo(DNCBurstMode)]
        [CustomComboInfo("Curing Waltz Option", "Adds Curing Waltz to the main combo when available, and your HP is at or below the set percentage.", DNC.JobID)]
        DNCCuringWaltzOption = 80072,

        // SAM
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Adds Meikyo Shisui, Midare:Setsugekka, Ogi Namikiri, Kaeshi: Namikiri and Soten to Meikyo Shisui.\nWill only cast Midare and Ogi Namikiri when you're not moving.\nWill not use if target is guarding.", SAM.JobID)]
        SAMBurstMode = 80080,

            #region SAM Burst Mode
            [SecretCustomCombo]
            [ParentCombo(SAMBurstMode)]
            [CustomComboInfo("Add Chiten", "Adds Chiten to the Burst Mode when in combat and HP is below 95%.", SAM.JobID)]
            SAMBurstChitenFeature = 80081,

            [SecretCustomCombo]
            [ParentCombo(SAMBurstMode)]
            [CustomComboInfo("Add Mineuchi", "Adds Mineuchi to the Burst Mode.", SAM.JobID)]
            SAMBurstStunFeature = 80082,

            [SecretCustomCombo]
            [ParentCombo(SAMBurstMode)]
            [CustomComboInfo("Burst Mode on Kasha Combo", "Adds Burst Mode to Kasha Combo instead.", SAM.JobID, 1)]
            SamPVPMainComboFeature = 80083,        
            #endregion

        [SecretCustomCombo]
        [CustomComboInfo("PvP Features for Kasha Combo", "Collection of Features for Kasha Combo.", SAM.JobID)]
        SamPvPKashaFeatures = 80084,

            #region PvP Features for Kasha Combo
            [SecretCustomCombo]
            [ParentCombo(SamPvPKashaFeatures)]
            [CustomComboInfo("Soten Gap Closer Option", "Adds Soten when outside melee range to the Kasha Combo.", SAM.JobID)]
            SamGapCloserFeature = 80085,

            [SecretCustomCombo]
            [ParentCombo(SamPvPKashaFeatures)]
            [CustomComboInfo("AOE Melee Protection", "Makes the AOE combos unusable if not in melee range of target.", SAM.JobID)]
            SamAOEMeleeFeature = 80086,
            #endregion
            
        //BLM
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Fire and Blizzard into all-in-one damage buttons.", BLM.JobID)]
        BLMBurstMode = 80090,

            #region BLM Burst Mode
            [ParentCombo(BLMBurstMode)]
            [SecretCustomCombo]
            [CustomComboInfo("Add Night Wing", "Adds Night Wing to the Burst Mode", BLM.JobID)]
            BLMNightWing = 80091,

            [ParentCombo(BLMBurstMode)]
            [SecretCustomCombo]
            [CustomComboInfo("Add Aetherial Manipulation", "Uses Aetherial Manipulation to gap close if Burst is off cooldown.", BLM.JobID)]
            BLMAetherialManip = 80092,
            #endregion

        // RPR
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Slice Combo into an all-in-one damage button.\nAdds Soul Slice to the main combo.", RPR.JobID)]
        RPRBurstMode = 80190,

            #region RPR Burst Mode
            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Grim Swathe Option", "Weaves Grim Swathe onto the main combo when available.", RPR.JobID)]
            RPRPvPGrimSwatheOption = 80191,

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Death Warrant Option", "Adds Death Warrant onto the main combo when Plentiful Harvest is ready to use, or when Plentiful Harvest's cooldown is longer than Death Warrant's.\nRespects Immortal Sacrifice Pooling Option.", RPR.JobID)]
            RPRPvPDeathWarrantOption = 80192,

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Plentiful Harvest Opener Option", "Starts combat with Plentiful Harvest to immediately begin Limit Break generation.", RPR.JobID)]
            RPRPvPPlentifulOpenerOption = 80193,

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Plentiful Harvest + Immortal Sacrifice Pooling Option - BETA", "Pools stacks of Immortal Sacrifice before using Plentiful Harvest.\nAlso holds Plentiful Harvest if Death Warrant is on cooldown.\nSet the value to 3 or below to use Plentiful as soon as it's suitable.", RPR.JobID)]
            RPRPvPImmortalPoolingOption = 80194,

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Enshrouded Burst Option", "Puts Lemure's Slice on the main combo during Enshrouded Burst Phase.\nContains burst options.", RPR.JobID)]
            RPRPvPEnshroudedOption = 80195,

                #region RPR Enshrouded Option
                [SecretCustomCombo]
                [ParentCombo(RPRPvPEnshroudedOption)]
                [CustomComboInfo("Enshrouded Death Warrant Option", "Adds Death Warrant onto the main combo during the Enshroud burst when available.", RPR.JobID)]
                RPRPvPEnshroudedDeathWarrantOption = 80196,

                [SecretCustomCombo]
                [ParentCombo(RPRPvPEnshroudedOption)]
                [CustomComboInfo("Communio Finisher Option", "Adds Communio onto the main combo when you have 1 stack of Enshroud remaining.\nWill not trigger if you are moving.", RPR.JobID)]
                RPRPvPEnshroudedCommunioOption = 80197,
                #endregion

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Ranged Harvest Moon Option", "Puts Harvest Moon onto the main combo when you're out of melee range, the GCD is not rolling and it is available for use.", RPR.JobID)]
            RPRPvPRangedHarvestMoonOption = 80198,

            [SecretCustomCombo]
            [ParentCombo(RPRBurstMode)]
            [CustomComboInfo("Arcane Circle Option", "Adds Arcane Circle to the main combo when under a set HP perecentage.", RPR.JobID)]
            RPRPvPArcaneCircleOption = 80199,
            #endregion

        // MNK
        [SecretCustomCombo]
        [CustomComboInfo("Burst Mode", "Turns Phantom Rush Combo into all-in-one damage button.", MNK.JobID)]
        MNKBurstMode = 80100,

            #region MNK Burst Mode
            [ParentCombo(MNKBurstMode)]
            [SecretCustomCombo]
            [CustomComboInfo("Add Thunderclap", "Adds Thunderclap to jump to Enemy Target when not buffed with Wind Resonance.", MNK.JobID)]
            MNKThunderClapOption = 80101,

            [ParentCombo(MNKBurstMode)]
            [SecretCustomCombo]
            [CustomComboInfo("Add Riddle of Earth", "Adds Riddle of Earth and Earth's Reply to the Burst Mode when in combat.", MNK.JobID)]
            MNKRiddleOfEarthOption = 80102,
            #endregion

        #endregion
        // ====================================================================================
        #region PvPGlobals
        [SecretCustomCombo]
        [CustomComboInfo("Emergency Heals", "Uses Recuperate when your HP is under a certain threshold and you have the MP.", ADV.JobID, 1)]
        PVPEmergencyHeals = 90000,

        [SecretCustomCombo]
        [CustomComboInfo("Emergency Guard", "Uses Guard when your HP is under a certain threshold.", ADV.JobID, 2)]
        PVPEmergencyGuard = 90001,

        [SecretCustomCombo]
        [CustomComboInfo("Quick Purify", "Uses Purify when afflicted with any selected debuff.", ADV.JobID, 4)]
        PVPQuickPurify = 90002,

        [SecretCustomCombo]
        [CustomComboInfo("Prevent Mash Cancelling", "Stops you cancelling your guard if you're mashing buttons", ADV.JobID, 3)]
        PVPMashCancel = 90003,

        #endregion
    }
}
