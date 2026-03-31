using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.Enums.Calendar;

namespace Imperium.Engine.Data.DataModels
{
    /// <summary>
    /// Represents a scheduled event on the in-game calendar.
    /// Events are displayed to the player as reminders and can trigger game logic.
    /// </summary>
    public class CalendarEvent
    {
        /// <summary>
        /// Unique identifier for this event (for DB persistence and lookup).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Short title shown to the player. E.g., "Arena Match vs. Iron Fang"
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional longer description or flavor text.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The date this event is scheduled for.
        /// </summary>
        public CalendarDate ScheduledDate { get; set; } = new();

        /// <summary>
        /// Category of the event. Used for filtering, icons, and routing logic in GameManager.
        /// </summary>
        public CalendarEventType EventType { get; set; }

        /// <summary>
        /// If true, the event repeats every year on the same month/day.
        /// Useful for festivals, birthdays, and league season openers.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// If true, this event has already fired its logic and been shown to the player.
        /// Prevents duplicate triggers when loading saves.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Optional: an ID linking to a related entity (patron ID, arena ID, dynasty member ID, etc.).
        /// GameManager uses this to route the event to the correct subsystem.
        /// </summary>
        public int? RelatedEntityId { get; set; }

        /// <summary>
        /// Optional: a tag for the related entity type (e.g., "Patron", "Arena", "DynastyMember").
        /// Used alongside RelatedEntityId for routing.
        /// </summary>
        public string? RelatedEntityType { get; set; }

        /// <summary>
        /// Priority for display ordering when multiple events land on the same day.
        /// Lower number = higher priority.
        /// </summary>
        public int Priority { get; set; } = 5;

        public override string ToString()
        {
            return $"[{EventType}] {Title} on {ScheduledDate}";
        }
    }
}

