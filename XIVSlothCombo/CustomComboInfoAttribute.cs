using System;

namespace XIVSlothComboPlugin
{
    /// <summary>
    /// Attribute documenting additional information for each combo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class CustomComboInfoAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComboInfoAttribute"/> class.
        /// </summary>
        /// <param name="fancyName">Display name.</param>
        /// <param name="description">Combo description.</param>
        /// <param name="jobID">Associated job ID.</param>
        /// <param name="actionIDs">Associated action IDs.</param>
        internal CustomComboInfoAttribute(string fancyName, string description, byte jobID, params uint[] actionIDs)
        {
            this.FancyName = fancyName;
            this.Description = description;
            this.JobID = jobID;
            this.ActionIDs = actionIDs;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string FancyName { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the job ID.
        /// </summary>
        public byte JobID { get; }

        /// <summary>
        /// Gets the job name.
        /// </summary>
        public string JobName => JobIDToName(this.JobID);

        /// <summary>
        /// Gets the action IDs associated with the combo.
        /// </summary>
        public uint[] ActionIDs { get; }

        private static string JobIDToName(byte key)
        {
            return key switch
            {
                0 => "All",
                1 => "Gladiator",
                2 => "Pugilist",
                3 => "Marauder",
                4 => "Lancer",
                5 => "Archer",
                6 => "Conjurer",
                7 => "Thaumaturge",
                8 => "Carpenter",
                9 => "Blacksmith",
                10 => "Armorer",
                11 => "Goldsmith",
                12 => "Leatherworker",
                13 => "Weaver",
                14 => "Alchemist",
                15 => "Culinarian",
                16 => "Miner",
                17 => "Botanist",
                18 => "Fisher",
                19 => "Paladin",
                20 => "Monk",
                21 => "Warrior",
                22 => "Dragoon",
                23 => "Bard",
                24 => "White Mage",
                25 => "Black Mage",
                26 => "Arcanist",
                27 => "Summoner",
                28 => "Scholar",
                29 => "Rogue",
                30 => "Ninja",
                31 => "Machinist",
                32 => "Dark Knight",
                33 => "Astrologian",
                34 => "Samurai",
                35 => "Red Mage",
                36 => "Blue Mage",
                37 => "Gunbreaker",
                38 => "Dancer",
                39 => "Reaper",
                40 => "Sage",
                99 => "Disciple of Magic",
                _ => "Unknown",
            };
        }
    }
}
