using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium.Engine.Data.EntityModels.Character
{
    public class Player
    {
        public int PlayerID { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string MiddleName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public int RaceID { get; init; }
        public int BackgroundID { get; init; }
        public int ClassID { get; init; }
        public int Level { get; init; }
        public int XP { get; init; }
        public int Age { get; init; }
        public int HP { get; init; }
        public int MP { get; init; }
        public int Strength { get; init; }
        public int Agility { get; init; }
        public int Endurance { get; init; }
        public int Intelligence { get; init; }
        public int Wisdom { get; init; }
        public int Charisma { get; init; }
        public int Luck { get; init; }

    }
}
