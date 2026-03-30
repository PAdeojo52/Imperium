using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.EntityModels.World.Calendar;
using Imperium.Engine.Data.Enums.Calendar;
using Imperium.Engine.Data.Interfaces;

namespace Imperium.Engine.Utilities
{
    /// <summary>
    /// Central hub for interconnecting game systems.
    /// Owns the GameCalendar and orchestrates cross-system triggers
    /// when the player advances turns, completes fights, etc.
    /// </summary>
    public class GameManager
    {
        // --- Dependencies ---
        private readonly ICalendarService _calendarService;
        // TODO: Add other service interfaces as systems are built:
        // private readonly IPlayerService _playerService;
        // private readonly IArenaService _arenaService;
        // private readonly IPatronService _patronService;
        // private readonly IDynastyService _dynastyService;

        // --- State ---
        public GameCalendar Calendar { get; private set; } = new();
        private int _currentSaveId;

        // --- Events (C# events for WPF/MVVM binding) ---

        /// <summary>
        /// Fired after each day advance with the new date. 
        /// ViewModels can subscribe for UI updates.
        /// </summary>
        public event Action<CalendarDate>? OnDayAdvanced;

        /// <summary>
        /// Fired when the calendar crosses into a new month.
        /// Useful for league season checks, patron payments, etc.
        /// </summary>
        public event Action<int, int>? OnNewMonth; // (month, year)

        /// <summary>
        /// Fired when the calendar crosses into a new year.
        /// Triggers age progression, annual reputation decay, dynasty updates.
        /// </summary>
        public event Action<int>? OnNewYear; // (year)

        /// <summary>
        /// Fired when a scheduled calendar event triggers on the current day.
        /// The ViewModel or subsystem handler should react accordingly.
        /// </summary>
        public event Action<CalendarEvent>? OnCalendarEventTriggered;

        // -----------------------------------------------------------------
        //  Constructor
        // -----------------------------------------------------------------

        public GameManager(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        // -----------------------------------------------------------------
        //  Initialization & Save/Load
        // -----------------------------------------------------------------

        /// <summary>
        /// Called when starting a new game. Sets the calendar to Day 1, Month 1, Year 1
        /// (or a configurable start date).
        /// </summary>
        public void InitializeNewGame(int saveId, CalendarDate? startDate = null)
        {
            _currentSaveId = saveId;
            Calendar = new GameCalendar
            {
                CurrentDate = startDate ?? new CalendarDate(1, 1, 1)
            };

            // TODO: Schedule initial world events here
            // ScheduleLeagueSeasons();
            // SchedulePatronFestivals();
            // SchedulePlayerBirthday(playerBirthDate);
        }

        /// <summary>
        /// Load the calendar (and eventually all systems) from a save file.
        /// </summary>
        public void LoadGame(int saveId)
        {
            _currentSaveId = saveId;
            Calendar = _calendarService.LoadCalendar(saveId);

            // TODO: Load other systems
            // _playerService.LoadPlayer(saveId);
            // _arenaService.LoadArenas(saveId);
        }

        /// <summary>
        /// Persist all game state to the database.
        /// </summary>
        public void SaveGame()
        {
            _calendarService.SaveCalendar(_currentSaveId, Calendar);

            // TODO: Save other systems
            // _playerService.SavePlayer(_currentSaveId);
        }

        // -----------------------------------------------------------------
        //  Turn Advancement (Core Game Loop)
        // -----------------------------------------------------------------

        /// <summary>
        /// Called when the player presses "Next" to end their turn.
        /// This is the central orchestration point:
        ///   1. Advance the calendar by one day
        ///   2. Check for and fire calendar events
        ///   3. Check for month/year boundaries → trigger cross-system updates
        /// </summary>
        public void AdvanceTurn()
        {
            // 1. Advance the day and collect triggered events
            List<CalendarEvent> triggeredEvents = Calendar.AdvanceDay();

            // 2. Notify listeners of the new day
            OnDayAdvanced?.Invoke(Calendar.CurrentDate);

            // 3. Process each triggered event
            foreach (var calendarEvent in triggeredEvents)
            {
                ProcessCalendarEvent(calendarEvent);
            }

            // 4. Check for month/year boundaries
            if (Calendar.IsNewMonth())
            {
                HandleNewMonth();
            }

            if (Calendar.IsNewYear())
            {
                HandleNewYear();
            }

            // 5. Auto-save on significant events (optional)
            // if (triggeredEvents.Any() || Calendar.IsNewMonth())
            //     SaveGame();
        }

        // -----------------------------------------------------------------
        //  Calendar Event Scheduling (Public API for other systems)
        // -----------------------------------------------------------------

        /// <summary>
        /// Schedule an arena match on a specific date.
        /// </summary>
        public void ScheduleArenaMatch(string title, CalendarDate date, int arenaId, int priority = 3)
        {
            var matchEvent = new CalendarEvent
            {
                Title = title,
                EventType = CalendarEventType.ArenaMatch,
                ScheduledDate = date,
                RelatedEntityId = arenaId,
                RelatedEntityType = "Arena",
                Priority = priority
            };

            Calendar.AddEvent(matchEvent);
            _calendarService.CreateEvent(_currentSaveId, matchEvent);
        }

        /// <summary>
        /// Schedule a patron summons or festival.
        /// </summary>
        public void SchedulePatronEvent(string title, CalendarDate date, int patronId,
            CalendarEventType type = CalendarEventType.PatronSummons)
        {
            var patronEvent = new CalendarEvent
            {
                Title = title,
                EventType = type,
                ScheduledDate = date,
                RelatedEntityId = patronId,
                RelatedEntityType = "Patron",
                Priority = 2
            };

            Calendar.AddEvent(patronEvent);
            _calendarService.CreateEvent(_currentSaveId, patronEvent);
        }

        /// <summary>
        /// Schedule a recurring annual event (festival, birthday, league season start).
        /// </summary>
        public void ScheduleRecurringEvent(string title, int month, int day,
            CalendarEventType type, string? description = null)
        {
            var recurring = new CalendarEvent
            {
                Title = title,
                Description = description ?? string.Empty,
                EventType = type,
                ScheduledDate = new CalendarDate(day, month, Calendar.CurrentDate.Year),
                IsRecurring = true,
                Priority = 4
            };

            Calendar.AddEvent(recurring);
            _calendarService.CreateEvent(_currentSaveId, recurring);
        }

        /// <summary>
        /// Generic event scheduling for any system.
        /// </summary>
        public void ScheduleEvent(CalendarEvent calendarEvent)
        {
            Calendar.AddEvent(calendarEvent);
            _calendarService.CreateEvent(_currentSaveId, calendarEvent);
        }

        // -----------------------------------------------------------------
        //  Event Processing (Internal Routing)
        // -----------------------------------------------------------------

        /// <summary>
        /// Routes a triggered calendar event to the appropriate subsystem.
        /// This is where the "central hub" pattern pays off — all cross-system 
        /// coordination lives here instead of scattered across services.
        /// </summary>
        private void ProcessCalendarEvent(CalendarEvent calendarEvent)
        {
            // Fire the general event for UI/ViewModel subscribers
            OnCalendarEventTriggered?.Invoke(calendarEvent);

            // Route to specific subsystem handlers based on event type
            switch (calendarEvent.EventType)
            {
                case CalendarEventType.ArenaMatch:
                    HandleArenaMatchEvent(calendarEvent);
                    break;

                case CalendarEventType.PatronSummons:
                case CalendarEventType.PatronFestival:
                    HandlePatronEvent(calendarEvent);
                    break;

                case CalendarEventType.Birthday:
                case CalendarEventType.HeirBorn:
                case CalendarEventType.Wedding:
                    HandleDynastyEvent(calendarEvent);
                    break;

                case CalendarEventType.LeagueSeasonStart:
                case CalendarEventType.LeagueSeasonEnd:
                case CalendarEventType.LeaguePlayoff:
                case CalendarEventType.Championship:
                    HandleLeagueEvent(calendarEvent);
                    break;

                case CalendarEventType.ComingOfAge:
                case CalendarEventType.MidlifeMilestone:
                case CalendarEventType.RetirementDuel:
                    HandleAgingEvent(calendarEvent);
                    break;

                case CalendarEventType.TrainingSession:
                case CalendarEventType.MentorVisit:
                    HandleTrainingEvent(calendarEvent);
                    break;

                case CalendarEventType.RoyalDecree:
                case CalendarEventType.EcclesiasticalEdict:
                case CalendarEventType.WarDeclaration:
                    HandlePoliticalEvent(calendarEvent);
                    break;

                default:
                    // Custom or unhandled — just the notification is enough
                    break;
            }

            // Mark as completed
            Calendar.CompleteEvent(calendarEvent.Id);
            _calendarService.UpdateEvent(calendarEvent);
        }

        // -----------------------------------------------------------------
        //  Subsystem Handlers (Stubs — flesh out as systems are built)
        // -----------------------------------------------------------------

        private void HandleNewMonth()
        {
            int month = Calendar.CurrentDate.Month;
            int year = Calendar.CurrentDate.Year;

            OnNewMonth?.Invoke(month, year);

            // TODO: Monthly triggers
            // - League standings update
            // - Patron favor decay/gain
            // - Arena revenue calculation
            // - Merchant inventory refresh
        }

        private void HandleNewYear()
        {
            int year = Calendar.CurrentDate.Year;

            OnNewYear?.Invoke(year);

            // TODO: Annual triggers
            // - Player age increment → check for aging stat changes (STR/AGI decline after 35)
            // - Reputation annual decay
            // - Dynasty member aging
            // - Generate new league seasons
            // - Ecclesiastical elections (Grand Patriarch)
            // Calendar.CleanupCompletedEvents();
        }

        private void HandleArenaMatchEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger arena match flow
            // - Look up arena via calendarEvent.RelatedEntityId
            // - Set up opponent, match rules, crowd state
            // - Transition to battle panel
        }

        private void HandlePatronEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger patron interaction
            // - Summons: Present patron dialogue and quest
            // - Festival: Attendance check, favor gain/loss
        }

        private void HandleDynastyEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger dynasty updates
            // - Birthday: Age up family member, check milestones
            // - Wedding: Merge dynasties, gain perks
            // - HeirBorn: Add new family member, roll traits
        }

        private void HandleLeagueEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger league phase transitions
            // - Season start: Reset standings
            // - Season end: Calculate final standings
            // - Playoff: Generate brackets
            // - Championship: Award title, prestige, gear
        }

        private void HandleAgingEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger age milestone logic
            // - ComingOfAge (20): Unlock specialization path
            // - MidlifeMilestone (35): New training, stat shifts
            // - RetirementDuel (50+): Farewell fight, legacy mode
        }

        private void HandleTrainingEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger training session
            // - Skill XP gain
            // - Mentor-specific bonuses
        }

        private void HandlePoliticalEvent(CalendarEvent calendarEvent)
        {
            // TODO: Trigger world events
            // - Royal decrees affecting arena rules
            // - Ecclesiastical edicts banning magic types
            // - War declarations changing province access
        }
    }
}
