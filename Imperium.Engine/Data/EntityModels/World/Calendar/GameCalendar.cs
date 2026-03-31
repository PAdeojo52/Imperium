using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.Enums.Calendar;

namespace Imperium.Engine.Data.EntityModels.World.Calendar
{
    public class GameCalendar
    {
        // --- Current Date State ---
        public CalendarDate CurrentDate { get; set; } = new(1, 1, 1);

        // --- Scheduled Events ---
        private readonly List<CalendarEvent> _events = new();

        /// <summary>
        /// Read-only access to all scheduled events.
        /// </summary>
        public IReadOnlyList<CalendarEvent> Events => _events.AsReadOnly();

        // -----------------------------------------------------------------
        //  Date Advancement (called by GameManager at end of turn)
        // -----------------------------------------------------------------

        /// <summary>
        /// Advances the calendar by one day and returns any events that fall on the new date.
        /// GameManager should call this once per turn, then process the returned events.
        /// </summary>
        public List<CalendarEvent> AdvanceDay()
        {
            CurrentDate = CurrentDate.AddDays(1);
            return GetEventsForDate(CurrentDate);
        }

        /// <summary>
        /// Advances by multiple days at once (e.g., for rest/travel). 
        /// Returns all events encountered across those days, in chronological order.
        /// </summary>
        public List<CalendarEvent> AdvanceDays(int count)
        {
            var triggered = new List<CalendarEvent>();
            for (int i = 0; i < count; i++)
            {
                triggered.AddRange(AdvanceDay());
            }
            return triggered;
        }

        // -----------------------------------------------------------------
        //  Date Queries
        // -----------------------------------------------------------------

        /// <summary>
        /// Did we just cross into a new month compared to yesterday?
        /// </summary>
        public bool IsNewMonth()
        {
            var yesterday = CurrentDate.AddDays(-1);
            return CurrentDate.Month != yesterday.Month;
        }

        /// <summary>
        /// Did we just cross into a new year compared to yesterday?
        /// </summary>
        public bool IsNewYear()
        {
            var yesterday = CurrentDate.AddDays(-1);
            return CurrentDate.Year != yesterday.Year;
        }

        /// <summary>
        /// Current season derived from the month.
        /// </summary>
        //public Seasons CurrentSeason => CurrentDate.CurrentSeason;

        // -----------------------------------------------------------------
        //  Event Management
        // -----------------------------------------------------------------

        /// <summary>
        /// Schedule a new event on the calendar.
        /// </summary>
        public void AddEvent(CalendarEvent calendarEvent)
        {
            _events.Add(calendarEvent);
        }

        /// <summary>
        /// Remove a specific event by its ID.
        /// </summary>
        public bool RemoveEvent(int eventId)
        {
            var target = _events.Find(e => e.Id == eventId);
            if (target != null)
            {
                _events.Remove(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all non-completed events scheduled for a specific date.
        /// Recurring events match on month/day regardless of year.
        /// Returns sorted by priority (lower number = higher priority).
        /// </summary>
        public List<CalendarEvent> GetEventsForDate(CalendarDate date)
        {
            return _events
                .Where(e => !e.IsCompleted && IsEventOnDate(e, date))
                .OrderBy(e => e.Priority)
                .ToList();
        }

        /// <summary>
        /// Get all upcoming events within the next N days from the current date.
        /// Useful for showing the player what's coming up.
        /// </summary>
        public List<CalendarEvent> GetUpcomingEvents(int daysAhead = 30)
        {
            int currentDayNum = CurrentDate.ToDayNumber();
            int endDayNum = currentDayNum + daysAhead;

            return _events
                .Where(e => !e.IsCompleted)
                .Where(e =>
                {
                    int eventDayNum = e.ScheduledDate.ToDayNumber();
                    if (eventDayNum > currentDayNum && eventDayNum <= endDayNum)
                        return true;

                    // Also check recurring events that may land in the window
                    if (e.IsRecurring)
                    {
                        var thisYear = new CalendarDate(e.ScheduledDate.Day, e.ScheduledDate.Month, CurrentDate.Year);
                        var nextYear = new CalendarDate(e.ScheduledDate.Day, e.ScheduledDate.Month, CurrentDate.Year + 1);
                        int thisYearNum = thisYear.ToDayNumber();
                        int nextYearNum = nextYear.ToDayNumber();
                        return (thisYearNum > currentDayNum && thisYearNum <= endDayNum) ||
                               (nextYearNum > currentDayNum && nextYearNum <= endDayNum);
                    }
                    return false;
                })
                .OrderBy(e => e.ScheduledDate.ToDayNumber())
                .ToList();
        }

        /// <summary>
        /// Mark an event as completed so it won't fire again.
        /// For recurring events, this resets it to next year's occurrence instead.
        /// </summary>
        public void CompleteEvent(int eventId)
        {
            var target = _events.Find(e => e.Id == eventId);
            if (target == null) return;

            if (target.IsRecurring)
            {
                // Advance the scheduled date to next year's occurrence
                target.ScheduledDate = new CalendarDate(
                    target.ScheduledDate.Day,
                    target.ScheduledDate.Month,
                    CurrentDate.Year + 1
                );
            }
            else
            {
                target.IsCompleted = true;
            }
        }

        /// <summary>
        /// Purge all completed, non-recurring events. Call during saves or season changes.
        /// </summary>
        public void CleanupCompletedEvents()
        {
            _events.RemoveAll(e => e.IsCompleted && !e.IsRecurring);
        }

        // -----------------------------------------------------------------
        //  Internal Helpers
        // -----------------------------------------------------------------

        private static bool IsEventOnDate(CalendarEvent calendarEvent, CalendarDate date)
        {
            if (calendarEvent.IsRecurring)
            {
                return calendarEvent.ScheduledDate.Day == date.Day
                    && calendarEvent.ScheduledDate.Month == date.Month;
            }
            return calendarEvent.ScheduledDate.Equals(date);
        }
    }
}

