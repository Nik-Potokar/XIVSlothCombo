using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Classes
{
  internal static class AST
  {
    public const byte JobID = 33;
    public static class Actions
    {
      public const ushort
        Repose = 16560,
        Esune = 7568,
        Swiftcast = 7561,
        LucidDreaming = 7562,
        Surecast = 7559,
        Rescue = 7571,
        Malefic = 3596,
        Benefic = 3594,
        Combust = 3599,
        Lightspeed = 3606,
        Helios = 3600,
        Ascend = 3603,
        EssentialDignity = 3614,
        Benefic2 = 3610,
        Draw = 3590,
        Undraw = 9629,
        Play = 17055,
        AspectedBenefic = 3595,
        Redraw = 3593,
        AspectedHelios = 3601,
        Gravity = 3615,
        Combust2 = 3608,
        Synastry = 3612,
        Divination = 16552,
        Astrodyne = 25870,
        Malefic2 = 3598,
        CollectiveUnconscious = 3613,
        CelestialOpposition = 16553,
        EarthlyStar = 7439,
        Malefic3 = 7442,
        MinorArcana = 7443,
        CrownPlay = 25869,
        Combust3 = 16554,
        Malefic4 = 16555,
        CelestialIntersection = 16556,
        Horoscope = 16557,
        NeutralSect = 16559,
        FallMalefic = 25871,
        Gravity2 = 25872,
        Exaltation = 25873,
        Macrocosmos = 25874,
        Microcosmos = 25875,
        LordOfCrowns = 7444,
        LadyOfCrowns = 7445;
    }

    public static class Levels
    {
      public const byte
        Malefic = 1,
        Benefic = 2,
        Combust = 4,
        Lightspeed = 6,
        Repose = 8,
        Helios = 10,
        Esuna = 10,
        Ascend = 12,
        EssentialDignity = 15,
        Swiftcast = 18,
        LucidDreaming = 24,
        Benefic2 = 26,
        Draw = 30,
        Undraw = 30,
        Play = 30,
        AspectedBenefic = 34,
        Redraw = 40,
        AspectedHelios = 42,
        Surecast = 44,
        Gravity = 45,
        Combust2 = 46,
        Rescue = 48,
        Synastry = 50,
        Divination = 50,
        Astrodyne = 50,
        Malefic2 = 54,
        CollectiveUnconscious = 58,
        CelestialOpposition = 60,
        EarthlyStar = 62,
        Malefic3 = 64,
        MinorArcana = 70,
        CrownPlay = 70,
        Combust3 = 72,
        Malefic4 = 72,
        CelestialIntersection = 74,
        Horoscope = 76,
        NeutralSect = 80,
        FallMalefic = 82,
        Gravity2 = 82,
        Exaltation = 86,
        Macrocosmos = 90;

      public static Dictionary<ushort, byte> Dictionary =
      new Dictionary<ushort, byte>(){
        { Actions.Repose, Levels.Repose },
        { Actions.Esune, Levels.Esuna },
        { Actions.Swiftcast, Levels.Swiftcast },
        { Actions.LucidDreaming, Levels.LucidDreaming },
        { Actions.Surecast, Levels.Surecast },
        { Actions.Rescue, Levels.Rescue },
        { Actions.Malefic, Levels.Malefic },
        { Actions.Benefic, Levels.Benefic },
        { Actions.Combust, Levels.Combust },
        { Actions.Lightspeed, Levels.Lightspeed },
        { Actions.Helios, Levels.Helios },
        { Actions.Ascend, Levels.Ascend },
        { Actions.EssentialDignity, Levels.EssentialDignity },
        { Actions.Benefic2, Levels.Benefic2 },
        { Actions.Draw, Levels.Draw },
        { Actions.Undraw, Levels.Undraw },
        { Actions.Play, Levels.Play },
        { Actions.AspectedBenefic, Levels.AspectedBenefic },
        { Actions.Redraw, Levels.Redraw },
        { Actions.AspectedHelios, Levels.AspectedHelios },
        { Actions.Gravity, Levels.Gravity },
        { Actions.Combust2, Levels.Combust2 },
        { Actions.Synastry, Levels.Synastry },
        { Actions.Divination, Levels.Divination },
        { Actions.Astrodyne, Levels.Astrodyne },
        { Actions.Malefic2, Levels.Malefic2 },
        { Actions.CollectiveUnconscious, Levels.CollectiveUnconscious },
        { Actions.CelestialOpposition, Levels.CelestialOpposition },
        { Actions.EarthlyStar, Levels.EarthlyStar },
        { Actions.Malefic3, Levels.Malefic3 },
        { Actions.MinorArcana, Levels.MinorArcana },
        { Actions.CrownPlay, Levels.CrownPlay },
        { Actions.Combust3, Levels.Combust3 },
        { Actions.Malefic4, Levels.Malefic4 },
        { Actions.CelestialIntersection, Levels.CelestialIntersection },
        { Actions.Horoscope, Levels.Horoscope },
        { Actions.NeutralSect, Levels.NeutralSect },
        { Actions.FallMalefic, Levels.FallMalefic },
        { Actions.Gravity2, Levels.Gravity2 },
        { Actions.Exaltation, Levels.Exaltation },
        { Actions.Macrocosmos, Levels.Macrocosmos },
        { Actions.Microcosmos, Levels.Macrocosmos },
        { Actions.LordOfCrowns, Levels.CrownPlay },
        { Actions.LadyOfCrowns, Levels.CrownPlay }
      };
    }

    public static class Buffs
    {
      public const ushort
        Divination = 1878,
        Swiftcast = 167,
        LordOfCrownsDrawn = 2054,
        LadyOfCrownsDrawn = 2055,
        AspectedHelios = 836,
        Balance = 913,
        Bole = 914,
        Arrow = 915,
        Spear = 916,
        Ewer = 917,
        Spire = 918;
    }

    public static class Debuffs
    {
      public const ushort
        Combust1 = 838,
        Combust2 = 843,
        Combust3 = 1881;
    }

    public static bool IsUnlocked (ushort actionID, byte level) {
      byte requiredLevel;

      if (!Levels.Dictionary.TryGetValue(actionID, out requiredLevel)) return false;

      return level >= requiredLevel;
    }

    public static ASTGauge JobGauge =>
      Service.ComboCache.GetJobGauge<ASTGauge>();
  }
}
