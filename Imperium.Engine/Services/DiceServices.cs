using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.Enums.Dice;

namespace Imperium.Engine.Services
{
    /// <summary>
    /// Central dice rolling service for all game systems.
    /// Handles standard rolls, advantage/disadvantage, skill checks, and contested rolls.
    /// Uses a seeded Random for reproducibility when needed (e.g., replays, save consistency).
    /// </summary>
    public class DiceService
    {
        private Random _rng;

        public DiceService()
        {
            _rng = new Random();
        }

        /// <summary>
        /// Create a seeded dice service for reproducible results.
        /// </summary>
        public DiceService(int seed)
        {
            _rng = new Random(seed);
        }

        /// <summary>
        /// Reseed the RNG. Useful when loading a save or starting a new encounter.
        /// </summary>
        public void Reseed(int seed)
        {
            _rng = new Random(seed);
        }

        // =================================================================
        //  Core Rolls
        // =================================================================

        /// <summary>
        /// Roll a single die of the given type.
        /// </summary>
        public int Roll(DiceType die)
        {
            return _rng.Next(1, (int)die + 1);
        }

        /// <summary>
        /// Roll multiple dice of the given type and return a full DiceResult.
        /// Example: Roll(DiceType.D6, 3, 2) → 3d6+2
        /// </summary>
        public DiceResult Roll(DiceType die, int count, int modifier = 0)
        {
            var result = new DiceResult
            {
                DieType = die,
                DiceCount = count,
                Modifier = modifier
            };

            for (int i = 0; i < count; i++)
            {
                result.Rolls.Add(Roll(die));
            }

            return result;
        }

        /// <summary>
        /// Shorthand: Roll 1dN and return just the number.
        /// </summary>
        public int RollValue(DiceType die) => Roll(die);

        /// <summary>
        /// Shorthand: Roll XdN+M and return just the total.
        /// </summary>
        public int RollTotal(DiceType die, int count = 1, int modifier = 0)
        {
            return Roll(die, count, modifier).Total;
        }

        // =================================================================
        //  Advantage / Disadvantage
        // =================================================================

        /// <summary>
        /// Roll with advantage: roll 2 dice, keep the higher.
        /// </summary>
        public DiceResult RollWithAdvantage(DiceType die, int modifier = 0)
        {
            int roll1 = Roll(die);
            int roll2 = Roll(die);
            int kept = Math.Max(roll1, roll2);

            return new DiceResult
            {
                DieType = die,
                DiceCount = 2,
                Rolls = new List<int> { roll1, roll2 },
                Modifier = modifier
            };
        }

        /// <summary>
        /// Roll with disadvantage: roll 2 dice, keep the lower.
        /// </summary>
        public DiceResult RollWithDisadvantage(DiceType die, int modifier = 0)
        {
            int roll1 = Roll(die);
            int roll2 = Roll(die);

            return new DiceResult
            {
                DieType = die,
                DiceCount = 2,
                Rolls = new List<int> { roll1, roll2 },
                Modifier = modifier
            };
        }

        /// <summary>
        /// From an advantage/disadvantage result, get the kept value (high or low).
        /// </summary>
        public int ResolveAdvantage(DiceResult result) => result.Highest + result.Modifier;
        public int ResolveDisadvantage(DiceResult result) => result.Lowest + result.Modifier;

        // =================================================================
        //  Skill Checks
        // =================================================================

        /// <summary>
        /// Perform a skill check: roll 1dN + modifier against a difficulty class (DC).
        /// Returns true if the roll meets or exceeds the DC.
        /// </summary>
        public (bool success, DiceResult roll) SkillCheck(DiceType die, int modifier, int dc)
        {
            var result = Roll(die, 1, modifier);
            return (result.Total >= dc, result);
        }

        /// <summary>
        /// Contested roll: two entities each roll 1dN + their modifier. Higher total wins.
        /// Returns positive if attacker wins, negative if defender wins, 0 for tie.
        /// </summary>
        public (int margin, DiceResult attackerRoll, DiceResult defenderRoll) ContestedRoll(
            DiceType die, int attackerModifier, int defenderModifier)
        {
            var atkResult = Roll(die, 1, attackerModifier);
            var defResult = Roll(die, 1, defenderModifier);
            int margin = atkResult.Total - defResult.Total;
            return (margin, atkResult, defResult);
        }

        // =================================================================
        //  Utility Rolls
        // =================================================================

        /// <summary>
        /// Coin flip (D2). Returns true for 2 (heads), false for 1 (tails).
        /// </summary>
        public bool CoinFlip() => Roll(DiceType.D2) == 2;

        /// <summary>
        /// Percentage check: roll 1-100, return true if roll is at or below the threshold.
        /// Uses 2d10 to simulate d100 (tens + ones).
        /// </summary>
        public (bool success, int rolled) PercentageCheck(int successThreshold)
        {
            int tens = (Roll(DiceType.D10) - 1) * 10; // 0, 10, 20, ... 90
            int ones = Roll(DiceType.D10);              // 1-10
            int rolled = tens + ones;                    // 1-100
            if (rolled > 100) rolled = 100;
            return (rolled <= successThreshold, rolled);
        }

        /// <summary>
        /// Roll on a table: roll 1dN and return the index (1-based).
        /// Useful for loot tables, event tables, encounter tables.
        /// </summary>
        public int RollOnTable(DiceType die) => Roll(die);

        /// <summary>
        /// Drop lowest: roll N dice, drop the lowest M, return the result.
        /// Common for stat generation (e.g., roll 4d6 drop lowest 1).
        /// </summary>
        public DiceResult RollDropLowest(DiceType die, int rollCount, int dropCount, int modifier = 0)
        {
            var allRolls = new List<int>();
            for (int i = 0; i < rollCount; i++)
            {
                allRolls.Add(Roll(die));
            }

            // Sort and take the top (rollCount - dropCount)
            var kept = allRolls.OrderDescending().Take(rollCount - dropCount).ToList();

            return new DiceResult
            {
                DieType = die,
                DiceCount = rollCount,
                Rolls = kept,
                Modifier = modifier
            };
        }

        /// <summary>
        /// Exploding dice: if a die rolls its max value, roll again and add.
        /// Continues until a non-max is rolled. Capped at maxExplosions to prevent infinite loops.
        /// </summary>
        public DiceResult RollExploding(DiceType die, int modifier = 0, int maxExplosions = 5)
        {
            var rolls = new List<int>();
            int max = (int)die;
            int explosions = 0;

            int current = Roll(die);
            rolls.Add(current);

            while (current == max && explosions < maxExplosions)
            {
                current = Roll(die);
                rolls.Add(current);
                explosions++;
            }

            return new DiceResult
            {
                DieType = die,
                DiceCount = rolls.Count,
                Rolls = rolls,
                Modifier = modifier
            };
        }
    }
}
