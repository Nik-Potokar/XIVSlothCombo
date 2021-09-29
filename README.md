# XIVComboPlugin
This plugin condenses combos and mutually exclusive abilities onto a single button. Thanks to Meli for the initial start, and obviously goat for making any of this possible.

## About
XIVCombo is a plugin to allow for "one-button" combo chains, as well as implementing various other mutually-exclusive button consolidation and quality of life replacements. 

For some jobs, this frees a massive amount of hotbar space (looking at you, DRG). For most, it removes a lot of mindless tedium associated with having to press various buttons that have little logical reason to be separate.

## Installation
* Type `/xlplugins` in-game to access the plugin installer and updater. Any releases on this github page have been removed to facilitate proper installation going forward.
## In-game usage
* Type `/pcombo` to pull up a GUI for editing active combo replacements.
* Drag the named ability from your ability list onto your hotbar to use.
  * For example, to use DRK's Souleater combo, first check the box, then place Souleater on your bar. It should automatically turn into Hard Slash.
  * The description associated with each combo should be enough to tell you which ability needs to be placed on the hotbar.
### Examples
![](https://github.com/daemitus/xivcomboplugin/raw/master/res/souleater_combo.gif)
![](https://github.com/daemitus/xivcomboplugin/raw/master/res/hypercharge_heat_blast.gif)
![](https://github.com/daemitus/xivcomboplugin/raw/master/res/eno_swap.gif)

## Known Issues
* None, for now!

## Full list of supported combos
Secret forbidden combos are noted with a ⭐ icon.

| Job | Name | Description |
|-----|------|-------------|
| AST | Draw on Play | Play turns into Draw when no card is drawn, as well as the usual Play behavior. |
| AST | Benefic 2 to Benefic Level Sync | Changes Benefic 2 to Benefic when below level 26 in synced content. |
| BLM | Enochian Stance Switcher | Change Enochian to Fire 4 or Blizzard 4 depending on stance. |
| BLM | Umbral Soul/Transpose Switcher | Change Transpose into Umbral Soul when Umbral Soul is usable. |
| BLM | (Between the) Ley Lines | Change Ley Lines into BTL when Ley Lines is active. |
| BLM | Fire 1/3 Feature | Fire 1 becomes Fire 3 outside of Astral Fire, and when Firestarter proc is up. |
| BLM | Blizzard 1/2/3 Feature | Blizzard 1 becomes Blizzard 3 when out of Umbral Ice. Freeze becomes Blizzard 2 when synced. |
| BRD | Wanderer's into Pitch Perfect | Replaces Wanderer's Minuet with Pitch Perfect while in WM. |
| BRD | Heavy Shot into Straight Shot | Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced. |
| BRD | Iron Jaws Feature | Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up. Alternates between the two if Iron Jaws isn't available. |
| DNC | Fan Dance Combos | Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing. |
| DNC | Dance Step Combo ⭐ | Change Standard Step and Technical Step into each dance step while dancing. |
| DNC | Flourish Proc Saver | Change Flourish into any available procs before using. |
| DNC | Single Target Multibutton | Change Cascade into procs and combos as available. |
| DNC | AoE Multibutton | Change Windmill into procs and combos as available |
| DNC | Dance Step Feature | Change actions into dance steps while dancing. | 
| DRG | Jump + Mirage Dive | Replace (High) Jump with Mirage Dive when Dive Ready. |
| DRG | BOTD Into Stardiver | Replace Blood of the Dragon with Stardiver when in Life of the Dragon. |
| DRG | Coerthan Torment Combo | Replace Coerthan Torment with its combo chain. |
| DRG | Chaos Thrust Combo | Replace Chaos Thrust with its combo chain. |
| DRG | Full Thrust Combo | Replace Full Thrust with its combo chain. |
| DRK | Souleater Combo | Replace Souleater with its combo chain. |
| DRK | Stalwart Soul Combo | Replace Stalwart Soul with its combo chain. |
| DRK | Delirium Feature | Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active. |
| GNB | Solid Barrel Combo | Replace Solid Barrel with its combo chain. |
| GNB | Wicked Talon Combo | Replace Wicked Talon with its combo chain. |
| GNB | Wicked Talon Continuation | In addition to the Wicked Talon combo chain, put Continuation moves on Wicked Talon when appropriate. |
| GNB | Demon Slaughter Combo | Replace Demon Slaughter with its combo chain. |
| GNB | Fated Circle Feature | In addition to the Demon Slaughter combo, add Fated Circle when charges are full. |
| MCH | (Heated) Shot Combo | Replace either form of Clean Shot with its combo chain. |
| MCH | Spread Shot Heat | Replace Spread Shot with Auto Crossbow when overheated. |
| MCH | Hypercharge Feature | Replace Heat Blast and Auto Crossbow with Hypercharge when not overheated. |
| MCH | Overdrive Feature | Replace Rook Autoturret and Automaton Queen with Overdrive while active. |
| MCH | Gauss Round / Ricochet Feature ⭐ | Replace Gauss Round and Ricochet with one or the other depending on which has more charges. |
| MNK | Monk AoE Combo | Replaces Rockbreaker with the AoE combo chain, or Rockbreaker when Perfect Balance is active. |
| NIN | Armor Crush Combo | Replace Armor Crush with its combo chain. |
| NIN | Aeolian Edge Combo | Replace Aeolian Edge with its combo chain. |
| NIN | Hakke Mujinsatsu Combo | Replace Hakke Mujinsatsu with its combo chain. |
| NIN | Dream to Assassinate | Replace Dream Within a Dream with Assassinate when Assassinate Ready. |
| PLD | Goring Blade Combo | Replace Goring Blade with its combo chain. |
| PLD | Royal Authority Combo | Replace Royal Authority/Rage of Halone with its combo chain. |
| PLD | Atonement Feature | Replace Royal Authority with Atonement when under the effect of Sword Oath. |
| PLD | Prominence Combo | Replace Prominence with its combo chain. |
| PLD | Requiescat Confiteor | Replace Requiescat with Confiter while under the effect of Requiescat. |
| PLD | Confiteor Feature ⭐ | Replace Holy Spirit/Circle with Confiteor while MP is under 4000 and Requiescat is up. |
| RDM | Red Mage AoE Combo | Replaces Veraero/Verthunder 2 with Impact when Dualcast or Swiftcast are active. |
| RDM | Redoublement combo | Replaces Redoublement with its combo chain, following enchantment rules. |
| RDM | Redoublement Combo Plus ⭐ | Replaces Redoublement with Verflare/Verholy after Enchanted Redoublement, whichever is more appropriate.\nRequires Redoublement Combo. |
| RDM | Verproc into Jolt | Replaces Verstone/Verfire with Jolt/Scorch when no proc is available. |
| RDM | Verproc into Jolt Plus | Additionally replaces Verstone/Verfire with Veraero/Verthunder if dualcast/swiftcast are up.\nRequires Verproc into Jolt. |
| RDM | Verproc into Jolt Plus Opener Feature | Turns Verfire into Verthunder when out of combat.\nRequires Verproc into Jolt Plus. |
| SAM | Yukikaze Combo | Replace Yukikaze with its combo chain. |
| SAM | Gekko Combo | Replace Gekko with its combo chain. |
| SAM | Kasha Combo | Replace Kasha with its combo chain. |
| SAM | Mangetsu Combo | Replace Mangetsu with its combo chain. |
| SAM | Oka Combo | Replace Oka with its combo chain. |
| SAM | Seigan to Third Eye | Replace Seigan with Third Eye when not procced. |
| SAM | Tsubame-gaeshi to Iaijutsu | Replace Tsubame-gaeshi with Iaijutsu when Sen is empty. |
| SAM | Tsubame-gaeshi to Shoha | Replace Tsubame-gaeshi with Shoha when meditation is 3. |
| SAM | Iaijutsu to Tsubame-gaeshi | Replace Iaijutsu with Tsubame-gaeshi when Sen is not empty. |
| SAM | Iaijutsu to Shoha | Replace Iaijutsu with Shoha when meditation is 3. |
| SCH | Seraph Fey Blessing/Consolation | Change Fey Blessing into Consolation when Seraph is out. |
| SCH | ED Aetherflow | Change Energy Drain into Aetherflow when you have no more Aetherflow stacks. |
| SMN | Demi-summon combiners | Dreadwyrm Trance, Summon Bahamut, and Firebird Trance are now one button.\nDeathflare, Enkindle Bahamut, and Enkindle Phoenix are now one button. |
| SMN | Brand of Purgatory Combo | Replaces Fountain of Fire with Brand of Purgatory when under the affect of Hellish Conduit. |
| SMN | ED Fester | Change Fester into Energy Drain when out of Aetherflow stacks. |
| SMN | ES Painflare | Change Painflare into Energy Syphon when out of Aetherflow stacks. |
| WAR | Storms Path Combo | Replace Storms Path with its combo chain. |
| WAR | Storms Eye Combo | Replace Storms Eye with its combo chain. |
| WAR | Mythril Tempest Combo | Replace Mythril Tempest with its combo chain. |
| WAR | Nascent Flash Feature | Replace Nascent Flash with Raw intuition when level synced below 76. |
| WHM | Solace into Misery | Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used. |
| WHM | Rapture into Misery | Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used. |
| WHM | Cure 2 to Cure Level Sync | Changes Cure 2 to Cure when below level 30 in synced content. |
