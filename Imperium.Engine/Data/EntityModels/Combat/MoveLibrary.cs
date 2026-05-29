using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.Enums;
using Imperium.Engine.Data.Enums.Combat;
using Imperium.Engine.Data.Enums.Dice;

namespace Imperium.Engine.Data.EntityModels.Combat
{
    /// <summary>
    /// Static library of all combat moves. Later this can be loaded from DB,
    /// but for now it provides the core move set for testing.
    /// </summary>
    public static class MoveLibrary
    {
        private static int _nextId = 1;

        // =================================================================
        //  Unarmed Moves
        // =================================================================

        public static CombatMove Punch => new()
        {
            Id = 1,
            Name = "Punch",
            Description = "A quick jab at the opponent.",
            Category = MoveCategory.Unarmed,
            DamageDie = DiceType.D4,
            DiceCount = 1,
            ScalingStat = "Strength",
            TimeCostSeconds = 6,
            StaminaCost = 3,
            CrowdAppeal = 0.05f
        };

        public static CombatMove Kick => new()
        {
            Id = 2,
            Name = "Kick",
            Description = "A powerful kick aimed at the body.",
            Category = MoveCategory.Unarmed,
            DamageDie = DiceType.D6,
            DiceCount = 1,
            ScalingStat = "Strength",
            TimeCostSeconds = 8,
            StaminaCost = 5,
            CrowdAppeal = 0.10f
        };

        public static CombatMove Tackle => new()
        {
            Id = 3,
            Name = "Tackle",
            Description = "Charge forward and slam the opponent to the ground.",
            Category = MoveCategory.Unarmed,
            DamageDie = DiceType.D8,
            DiceCount = 1,
            ScalingStat = "Strength",
            TimeCostSeconds = 12,
            StaminaCost = 10,
            CrowdAppeal = 0.20f
        };

        // =================================================================
        //  Weapon Moves
        // =================================================================

        public static CombatMove WeaponSlash => new()
        {
            Id = 10,
            Name = "Slash",
            Description = "A wide slash with your weapon.",
            Category = MoveCategory.Weapon,
            DamageDie = DiceType.D8,
            DiceCount = 1,
            ScalingStat = "Strength",
            TimeCostSeconds = 10,
            StaminaCost = 6,
            RequiredWeaponType = "Sword",
            CrowdAppeal = 0.15f
        };

        public static CombatMove WeaponThrust => new()
        {
            Id = 11,
            Name = "Thrust",
            Description = "A precise forward thrust.",
            Category = MoveCategory.Weapon,
            DamageDie = DiceType.D10,
            DiceCount = 1,
            ScalingStat = "Agility",
            TimeCostSeconds = 8,
            StaminaCost = 7,
            RequiredWeaponType = "Spear",
            CrowdAppeal = 0.15f
        };

        public static CombatMove WeaponSmash => new()
        {
            Id = 12,
            Name = "Smash",
            Description = "An overhead smash with a heavy weapon.",
            Category = MoveCategory.Weapon,
            DamageDie = DiceType.D12,
            DiceCount = 1,
            ScalingStat = "Strength",
            TimeCostSeconds = 15,
            StaminaCost = 12,
            RequiredWeaponType = "Mace",
            CrowdAppeal = 0.25f
        };

        // =================================================================
        //  Magic Moves
        // =================================================================

        public static CombatMove ArcaneBolt => new()
        {
            Id = 20,
            Name = "Arcane Bolt",
            Description = "A quick bolt of magical energy.",
            Category = MoveCategory.Magic,
            DamageDie = DiceType.D6,
            DiceCount = 2,
            ScalingStat = "Intelligence",
            TimeCostSeconds = 8,
            StaminaCost = 3,
            ManaCost = 10,
            CrowdAppeal = 0.15f
        };

        public static CombatMove FlameBlast => new()
        {
            Id = 21,
            Name = "Flame Blast",
            Description = "A roaring burst of fire.",
            Category = MoveCategory.Magic,
            DamageDie = DiceType.D10,
            DiceCount = 1,
            ScalingStat = "Intelligence",
            TimeCostSeconds = 12,
            StaminaCost = 5,
            ManaCost = 20,
            CrowdAppeal = 0.30f,
            CooldownTurns = 2
        };

        public static CombatMove HealingSurge => new()
        {
            Id = 22,
            Name = "Healing Surge",
            Description = "Channel magic to mend your wounds.",
            Category = MoveCategory.Magic,
            Target = MoveTarget.Self,
            DamageDie = DiceType.D8,
            DiceCount = 1,
            ScalingStat = "Wisdom",
            TimeCostSeconds = 10,
            StaminaCost = 4,
            ManaCost = 15,
            CrowdAppeal = -0.10f,
            CooldownTurns = 3
        };

        // =================================================================
        //  Defensive Moves
        // =================================================================

        public static CombatMove Block => new()
        {
            Id = 30,
            Name = "Block",
            Description = "Raise your guard to absorb incoming damage.",
            Category = MoveCategory.Defensive,
            Target = MoveTarget.Self,
            DamageDie = DiceType.D6,
            DiceCount = 1,
            DamageBonus = 0,
            ScalingStat = "Endurance",
            TimeCostSeconds = 5,
            StaminaCost = 4,
            CrowdAppeal = 0.0f
        };

        public static CombatMove Dodge => new()
        {
            Id = 31,
            Name = "Dodge",
            Description = "Attempt to evade the next attack entirely.",
            Category = MoveCategory.Defensive,
            Target = MoveTarget.Self,
            DamageDie = DiceType.D10,
            DiceCount = 1,
            ScalingStat = "Agility",
            TimeCostSeconds = 4,
            StaminaCost = 6,
            CrowdAppeal = 0.10f
        };

        // =================================================================
        //  Arena-Only Moves
        // =================================================================

        public static CombatMove TauntCrowd => new()
        {
            Id = 40,
            Name = "Taunt (Crowd)",
            Description = "Play to the crowd — flex, pose, or shout.",
            Category = MoveCategory.Taunt,
            Target = MoveTarget.Crowd,
            DamageDie = DiceType.D4,
            DiceCount = 0,
            DamageBonus = 0,
            ScalingStat = "Charisma",
            TimeCostSeconds = 8,
            StaminaCost = 2,
            CrowdAppeal = 0.25f,
            ArenaOnly = true
        };

        public static CombatMove TauntOpponent => new()
        {
            Id = 41,
            Name = "Taunt (Opponent)",
            Description = "Mock or provoke your opponent.",
            Category = MoveCategory.Taunt,
            Target = MoveTarget.Opponent,
            DamageDie = DiceType.D4,
            DiceCount = 0,
            DamageBonus = 0,
            ScalingStat = "Charisma",
            TimeCostSeconds = 6,
            StaminaCost = 2,
            CrowdAppeal = 0.15f,
            ArenaOnly = true
        };

        public static CombatMove Whisper => new()
        {
            Id = 50,
            Name = "Whisper",
            Description = "Tell your opponent your next move so they can sell the counter.",
            Category = MoveCategory.Whisper,
            Target = MoveTarget.Opponent,
            DamageDie = DiceType.D4,
            DiceCount = 0,
            DamageBonus = 0,
            ScalingStat = "Charisma",
            TimeCostSeconds = 3,
            StaminaCost = 0,
            CrowdAppeal = 0.0f,
            ArenaOnly = true,
            CanBeWhispered = false
        };

        // =================================================================
        //  Get All Base Moves
        // =================================================================

        /// <summary>
        /// Returns all base moves. Filter by arena/battle mode and equipped weapons.
        /// </summary>
        public static List<CombatMove> GetAllMoves()
        {
            return new List<CombatMove>
            {
                Punch, Kick, Tackle,
                WeaponSlash, WeaponThrust, WeaponSmash,
                ArcaneBolt, FlameBlast, HealingSurge,
                Block, Dodge,
                TauntCrowd, TauntOpponent, Whisper
            };
        }

        /// <summary>
        /// Get moves available for a specific combat mode.
        /// Arena mode gets all moves. Battle mode excludes ArenaOnly moves.
        /// </summary>
        public static List<CombatMove> GetMovesForMode(CombatMode mode)
        {
            return mode == CombatMode.Arena
                ? GetAllMoves()
                : GetAllMoves().Where(m => !m.ArenaOnly).ToList();
        }
    }
}
