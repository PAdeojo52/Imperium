using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.EntityModels.World.Calendar;
using Imperium.Engine.Data.Enums.Calendar;

namespace Imperium.Engine.Data.Interfaces
{
    public interface ICalendarService
    {
        /// <summary>
        /// Load the calendar state from the database for the current save.
        /// </summary>
        GameCalendar LoadCalendar(int saveId);

        /// <summary>
        /// Persist the full calendar state (current date + all events) to the database.
        /// </summary>
        void SaveCalendar(int saveId, GameCalendar calendar);

        /// <summary>
        /// Insert a new event into the database and assign it an ID.
        /// Returns the event with its newly assigned ID.
        /// </summary>
        CalendarEvent CreateEvent(int saveId, CalendarEvent calendarEvent);

        /// <summary>
        /// Update an existing event in the database (e.g., mark completed, change date).
        /// </summary>
        void UpdateEvent(CalendarEvent calendarEvent);

        /// <summary>
        /// Delete an event from the database by ID.
        /// </summary>
        void DeleteEvent(int eventId);

        /// <summary>
        /// Retrieve all events for a given save, optionally filtered by type or date range.
        /// </summary>
        List<CalendarEvent> GetEvents(int saveId, CalendarEventType? filterType = null);

    }
}
