using Imperium.Engine.Data.Enums.Combat;

namespace Imperium.Engine.Data.DataModels
{
    /// <summary>
    /// A record of a single turn's action in combat.
    /// Used for combat log display, star rating calculation, and replay.
    /// </summary>
    public class TurnAction
    {
        /// <summary>Turn number (1-based).</summary>
        public int TurnNumber { get; set; }

        /// <summary>Who performed this action (fighter index: 0 = player, 1 = opponent).</summary>
        public int ActorIndex { get; set; }

        /// <summary>The move that was used.</summary>
        public CombatMove Move { get; set; } = null!;

        /// <summary>The dice result from the move.</summary>
        public DiceResult? Roll { get; set; }

        /// <summary>Damage dealt (after defense calculation). 0 for non-damage moves.</summary>
        public int DamageDealt { get; set; }

        /// <summary>Was this move whispered to the opponent beforehand?</summary>
        public bool WasWhispered { get; set; }

        /// <summary>Did the defender successfully counter/sell the move?</summary>
        public bool WasCountered { get; set; }

        /// <summary>Star rating change from this action (arena only).</summary>
        public float StarDelta { get; set; }

        /// <summary>Crowd reaction after this action (arena only).</summary>
        public CrowdReaction CrowdState { get; set; }

        /// <summary>Cumulative match time after this action in seconds (arena only).</summary>
        public int MatchTimeSeconds { get; set; }

        /// <summary>Narrative text for the combat log.</summary>
        public string Narration { get; set; } = string.Empty;

        public override string ToString() => $"[Turn {TurnNumber}] {Narration}";
    }
}

