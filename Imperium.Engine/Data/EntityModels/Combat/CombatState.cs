using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.EntityModels.Combat;
using Imperium.Engine.Data.Enums.Combat;

namespace Imperium.Engine.Data.EntityModels.Combat
{
    /// <summary>
    /// Complete state of an active combat encounter.
    /// Tracks both fighters, turn order, match timer, star rating, and combat log.
    /// </summary>
    public class CombatState
    {
        // --- Mode ---
        public CombatMode Mode { get; set; }

        // --- Fighters ---
        public Combatant Player { get; set; } = null!;
        public Combatant Opponent { get; set; } = null!;

        // --- Turn Management ---
        /// <summary>Current turn number (1-based).</summary>
        public int CurrentTurn { get; set; } = 1;

        /// <summary>Whose turn it is (0 = player, 1 = opponent).</summary>
        public int ActiveFighterIndex { get; set; }

        /// <summary>Is the combat still ongoing?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>How the fight ended.</summary>
        public CombatOutcome? Outcome { get; set; }

        // --- Arena Specific ---

        /// <summary>
        /// Current star rating of the match (0.00 to 7.00).
        /// Displayed as 0-5 stars publicly, 6-7 are secret ratings for legendary matches.
        /// Increments by 0.25 base per exciting exchange.
        /// </summary>
        public float StarRating { get; set; }

        /// <summary>Maximum visible star rating (5.0). Secret max is 7.0.</summary>
        public const float MaxPublicStars = 5.0f;
        public const float MaxSecretStars = 7.0f;

        /// <summary>
        /// Cumulative match time in seconds. Each move adds its TimeCostSeconds.
        /// </summary>
        public int MatchTimeSeconds { get; set; }

        /// <summary>
        /// Promoter's time limit in seconds. If MatchTimeSeconds exceeds this, 
        /// the promoter calls the match a draw.
        /// 0 = no time limit.
        /// </summary>
        public int TimeLimitSeconds { get; set; }

        /// <summary>Current crowd reaction to the match.</summary>
        public CrowdReaction CrowdMood { get; set; } = CrowdReaction.Neutral;

        /// <summary>
        /// Consecutive exciting actions counter. Streaks boost star gain.
        /// Resets on a boring or repeated move.
        /// </summary>
        public int MomentumStreak { get; set; }

        /// <summary>The last move category used (to detect repetition).</summary>
        public MoveCategory? LastMoveCategory { get; set; }

        // --- Arena Match Info ---
        public string? ArenaName { get; set; }
        public string? PromoterName { get; set; }
        public int? ArenaId { get; set; }

        // --- Combat Log ---
        public List<TurnAction> CombatLog { get; set; } = new();

        // --- Derived ---

        /// <summary>Get the active combatant.</summary>
        public Combatant ActiveFighter => ActiveFighterIndex == 0 ? Player : Opponent;

        /// <summary>Get the defending combatant.</summary>
        public Combatant DefendingFighter => ActiveFighterIndex == 0 ? Opponent : Player;

        /// <summary>Formatted match time as MM:SS.</summary>
        public string FormattedMatchTime
        {
            get
            {
                int mins = MatchTimeSeconds / 60;
                int secs = MatchTimeSeconds % 60;
                return $"{mins:D2}:{secs:D2}";
            }
        }

        /// <summary>Is the match over the time limit?</summary>
        public bool IsOverTime => TimeLimitSeconds > 0 && MatchTimeSeconds >= TimeLimitSeconds;

        /// <summary>Star rating display string (e.g., "★★★½").</summary>
        public string StarDisplay
        {
            get
            {
                float display = Math.Min(StarRating, MaxPublicStars);
                int fullStars = (int)display;
                bool hasHalf = (display - fullStars) >= 0.25f;
                string stars = new string('★', fullStars);
                if (hasHalf) stars += "½";
                int empty = 5 - fullStars - (hasHalf ? 1 : 0);
                if (empty > 0) stars += new string('☆', empty);
                return stars;
            }
        }

        public override string ToString() =>
            Mode == CombatMode.Arena
                ? $"Arena Match: {Player.Name} vs {Opponent.Name} | Turn {CurrentTurn} | {FormattedMatchTime} | {StarDisplay}"
                : $"Battle: {Player.Name} vs {Opponent.Name} | Turn {CurrentTurn}";
    }
}
