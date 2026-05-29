using Imperium.Engine.Data.EntityModels.Character;

namespace Imperium.Engine.Data.EntityModels.Combat
{
    /// <summary>
    /// A fighter participating in combat. Wraps a Player or NPC entity
    /// with runtime combat state (current HP, cooldowns, buffs, etc.).
    /// </summary>
    public class Combatant
    {
        /// <summary>Display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>True if this is the human player.</summary>
        public bool IsPlayer { get; set; }

        // --- Base Stats (copied from Player/NPC at combat start) ---
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Endurance { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public int Luck { get; set; }
        public int Level { get; set; }

        // --- Combat Resources ---
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int MaxMP { get; set; }
        public int CurrentMP { get; set; }
        public int MaxStamina { get; set; }
        public int CurrentStamina { get; set; }

        // --- Arena Specific ---

        /// <summary>Is this fighter the crowd favorite? Affects taunt results.</summary>
        public bool IsFanFavorite { get; set; }

        /// <summary>
        /// Crowd favor for this specific fighter (0-100).
        /// Taunts increase or decrease this based on IsFanFavorite.
        /// </summary>
        public int CrowdFavor { get; set; } = 50;

        // --- Whisper State ---

        /// <summary>The move this combatant was told is coming next (via Whisper). Null if none.</summary>
        public DataModels.CombatMove? WhisperedMove { get; set; }

        /// <summary>If true, this combatant will cooperate with the whispered move.</summary>
        public bool WillCooperate { get; set; }

        // --- Cooldown Tracking ---

        /// <summary>Move ID → turns remaining before it can be used again.</summary>
        public Dictionary<int, int> Cooldowns { get; set; } = new();

        // --- Status Effects ---
        public List<string> ActiveBuffs { get; set; } = new();
        public List<string> ActiveDebuffs { get; set; } = new();

        // --- Derived Properties ---

        public bool IsAlive => CurrentHP > 0;
        public bool IsExhausted => CurrentStamina <= 0;
        public float HPPercent => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0;

        /// <summary>
        /// Get the stat value by name for move scaling.
        /// </summary>
        public int GetStat(string statName) => statName switch
        {
            "Strength" => Strength,
            "Agility" => Agility,
            "Endurance" => Endurance,
            "Intelligence" => Intelligence,
            "Wisdom" => Wisdom,
            "Charisma" => Charisma,
            "Luck" => Luck,
            _ => 0
        };

        /// <summary>
        /// Tick cooldowns down by 1 at the start of each turn.
        /// </summary>
        public void TickCooldowns()
        {
            var expired = Cooldowns.Where(kv => kv.Value <= 1).Select(kv => kv.Key).ToList();
            foreach (var id in expired) Cooldowns.Remove(id);
            foreach (var id in Cooldowns.Keys.ToList()) Cooldowns[id]--;
        }

        /// <summary>
        /// Check if a move is on cooldown.
        /// </summary>
        public bool IsOnCooldown(int moveId) => Cooldowns.ContainsKey(moveId) && Cooldowns[moveId] > 0;

        /// <summary>
        /// Build a Combatant from a Player entity.
        /// </summary>
        public static Combatant FromPlayer(Player player)
        {
            return new Combatant
            {
                Name = $"{player.FirstName} {player.LastName}".Trim(),
                IsPlayer = true,
                Strength = player.Strength,
                Agility = player.Agility,
                Endurance = player.Endurance,
                Intelligence = player.Intelligence,
                Wisdom = player.Wisdom,
                Charisma = player.Charisma,
                Luck = player.Luck,
                Level = player.Level,
                MaxHP = player.HP,
                CurrentHP = player.HP,
                MaxMP = player.MP,
                CurrentMP = player.MP,
                MaxStamina = 100 + (player.Endurance / 2),
                CurrentStamina = 100 + (player.Endurance / 2)
            };
        }

        /// <summary>
        /// Generate a basic NPC combatant for testing or random encounters.
        /// </summary>
        public static Combatant CreateNPC(string name, int level, int baseStats = 8)
        {
            return new Combatant
            {
                Name = name,
                IsPlayer = false,
                Level = level,
                Strength = baseStats + level,
                Agility = baseStats + level,
                Endurance = baseStats + level,
                Intelligence = baseStats,
                Wisdom = baseStats,
                Charisma = baseStats,
                Luck = baseStats,
                MaxHP = 80 + (level * 10),
                CurrentHP = 80 + (level * 10),
                MaxMP = 30 + (level * 5),
                CurrentMP = 30 + (level * 5),
                MaxStamina = 100 + (level * 3),
                CurrentStamina = 100 + (level * 3)
            };
        }

        public override string ToString() => $"{Name} (Lv.{Level}) HP:{CurrentHP}/{MaxHP}";
    }
}
