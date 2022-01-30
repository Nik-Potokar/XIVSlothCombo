using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;


namespace XIVSlothComboPlugin.Combos
{
    internal static class BRDPvP
    {
        public const byte ClassID = 41;
        public const byte JobID = 23;

        public const uint
            BurstShot = 17745,
            ShadowBite = 18931,
            SideWinder = 8841,
            EmpyrealArrow = 8838,
            PitchPerfect = 8842,
            ApexArrow = 17747,
            Concentrate = 18955,
            TheWanderersMinuet = 8843,
            ArmysPeeon = 8844;


        public static class Buffs
        {
            public const ushort
                StraightShotReady = 122,
                BlastArrowReady = 2692,
                ShadowbiteReady = 3002,
                Concentrated = 2186,
                WanderersMinuet = 865,
                MagesBallad = 135,
                ArmysPaeon = 137;
        }

        public static class Debuffs
        {
            public const ushort
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Levels
        {
            public const byte
                StraightShot = 2,
                RagingStrikes = 4,
                VenomousBite = 6,
                Bloodletter = 12,
                Windbite = 30,
                MagesBallad = 30,
                ArmysPaeon = 40,
                RainOfDeath = 45,
                Barrage = 38,
                BattleVoice = 50,
                PitchPerfect = 52,
                EmpyrealArrow = 54,
                IronJaws = 56,
                WanderersMinuet = 52,
                Sidewinder = 60,
                CausticBite = 64,
                StormBite = 64,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                Shadowbite = 72,
                BurstShot = 76,
                ApexArrow = 80,
                Ladonsbite = 82,
                BlastArrow = 86,
                RadiantFinale = 90;
        }

        internal class BurstShotFeaturePVP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BurstShotFeaturePVP;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == BRDPvP.BurstShot)
                {
                    var gauge = GetJobGauge<BRDGauge>();
                    var burstshotCD = GetCooldown(BRDPvP.BurstShot);
                    var sidewinderCD = GetCooldown(BRDPvP.SideWinder);
                    var shadowbiteCD = GetCooldown(BRDPvP.ShadowBite);
                    var empyarrowCD = GetCooldown(BRDPvP.EmpyrealArrow);
                    var concentrateCD = GetCooldown(BRDPvP.Concentrate);
                    if (burstshotCD.IsCooldown)
                    {

                        if (gauge.Repertoire == 3)
                            return BRDPvP.PitchPerfect;
                        if (!shadowbiteCD.IsCooldown)
                            return BRDPvP.ShadowBite;
                        if (empyarrowCD.CooldownRemaining < 15)
                            return BRDPvP.EmpyrealArrow;
                        if (!sidewinderCD.IsCooldown && EnemyHealthPercentage() <= 30)
                            return BRDPvP.SideWinder;
                    }
                    if (gauge.SoulVoice == 100)
                        return BRDPvP.ApexArrow;
                }

                return actionID;
            }
        }
    }
    internal class SongsFeaturePVP : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SongsFeaturePVP;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRDPvP.TheWanderersMinuet)
            {
                var gauge = GetJobGauge<BRDGauge>();
                var minuetCD = GetCooldown(BRDPvP.TheWanderersMinuet);
                var armyspaeCD = GetCooldown(BRDPvP.ArmysPeeon);
                if (gauge.Song == Song.NONE && !minuetCD.IsCooldown)
                    return BRDPvP.TheWanderersMinuet;
                if (gauge.Song == Song.NONE && minuetCD.IsCooldown && !armyspaeCD.IsCooldown)
                    return BRDPvP.ArmysPeeon;
                if (gauge.Song == Song.ARMY && !minuetCD.IsCooldown)
                    return BRDPvP.TheWanderersMinuet;
            }

            return actionID;
        }
    }
}
