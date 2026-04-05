using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.Enums.Dice;

namespace Imperium.Engine.Data.DataModels
{
    /// <summary>
    /// The result of a dice roll. Captures individual rolls, total, and metadata
    /// for combat logs, UI display, and replay.
    /// </summary>
    public class DiceResult
    {
        /// <summary>The type of die rolled.</summary>
        public DiceType DieType { get; set; }

        /// <summary>How many dice were rolled.</summary>
        public int DiceCount { get; set; }

        /// <summary>Each individual die result.</summary>
        public List<int> Rolls { get; set; } = new();

        /// <summary>Flat modifier added after rolling (e.g., STR bonus).</summary>
        public int Modifier { get; set; }

        /// <summary>Sum of all rolls + modifier.</summary>
        public int Total => Rolls.Sum() + Modifier;

        /// <summary>The natural roll total before modifier.</summary>
        public int NaturalTotal => Rolls.Sum();

        /// <summary>True if any die rolled its maximum value.</summary>
        public bool HasCrit => Rolls.Any(r => r == (int)DieType);

        /// <summary>True if any die rolled a 1.</summary>
        public bool HasFumble => Rolls.Any(r => r == 1);

        /// <summary>The highest individual roll.</summary>
        public int Highest => Rolls.Count > 0 ? Rolls.Max() : 0;

        /// <summary>The lowest individual roll.</summary>
        public int Lowest => Rolls.Count > 0 ? Rolls.Min() : 0;

        public override string ToString()
        {
            string rollStr = string.Join(", ", Rolls);
            string mod = Modifier != 0 ? $" + {Modifier}" : "";
            return $"{DiceCount}d{(int)DieType} [{rollStr}]{mod} = {Total}";
        }
    }
}
