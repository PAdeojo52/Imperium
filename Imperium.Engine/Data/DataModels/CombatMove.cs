using Imperium.Engine.Data.Enums;
using Imperium.Engine.Data.Enums.Combat;
using Imperium.Engine.Data.Enums.Dice;

namespace Imperium.Engine.Data.DataModels
{
    /// <summary>
    /// Defines a single combat move. Shared between arena and battle modes.
    /// Moves have a time cost (arena), damage/effect values, and crowd appeal.
    /// </summary>
    public class CombatMove
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        /// <summary>Category determines UI grouping and availability rules.</summary>
        public MoveCategory Category { get; set; }

        /// <summary>Who this move targets.</summary>
        public MoveTarget Target { get; set; } = MoveTarget.Opponent;

        // --- Dice & Damage ---

        /// <summary>Which die to roll for this move's effect.</summary>
        public DiceType DamageDie { get; set; } = DiceType.D6;

        /// <summary>How many dice to roll.</summary>
        public int DiceCount { get; set; } = 1;

        /// <summary>Flat damage bonus added to the roll.</summary>
        public int DamageBonus { get; set; }

        /// <summary>Which stat modifies this move's damage (e.g., STR for melee, INT for magic).</summary>
        public string ScalingStat { get; set; } = "Strength";

        // --- Costs ---

        /// <summary>
        /// Time cost in seconds. Added to the arena match timer.
        /// Promoter's time limit is checked against cumulative time.
        /// </summary>
        public int TimeCostSeconds { get; set; } = 10;

        /// <summary>Mana cost for magic moves. 0 for physical moves.</summary>
        public int ManaCost { get; set; }

        /// <summary>Stamina cost. All moves drain some stamina; exhausted fighters hit weaker.</summary>
        public int StaminaCost { get; set; } = 5;

        /// <summary>Cooldown in turns before this move can be used again. 0 = no cooldown.</summary>
        public int CooldownTurns { get; set; }

        // --- Arena Specific ---

        /// <summary>
        /// How much this move appeals to the crowd when it lands.
        /// Positive = crowd likes it, negative = crowd boos.
        /// Range: -0.50 to +0.50 (added to star increment calculation).
        /// </summary>
        public float CrowdAppeal { get; set; }

        /// <summary>
        /// If true, this move is only available in arena combat (Taunt, Whisper).
        /// </summary>
        public bool ArenaOnly { get; set; }

        // --- Requirements ---

        /// <summary>Minimum level required to use this move.</summary>
        public int RequiredLevel { get; set; }

        /// <summary>Required weapon type equipped (null = no weapon needed).</summary>
        public string? RequiredWeaponType { get; set; }

        /// <summary>Can this move be whispered (telegraphed to opponent for a cooperative spot)?</summary>
        public bool CanBeWhispered { get; set; } = true;

        public override string ToString() => $"{Name} ({Category}) [{DiceCount}d{(int)DamageDie}+{DamageBonus}]";
    }
}
