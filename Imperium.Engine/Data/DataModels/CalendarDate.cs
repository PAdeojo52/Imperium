using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Engine.Data.Enums.Calendar;
using Imperium.Engine.Exceptions;

namespace Imperium.Engine.Data.DataModels
{
    public class CalendarDate
    {

        // --- Constants ---
        public const int DaysPerMonth = 30;
        public const int MonthsPerYear = 12;
        public const int DaysPerYear = DaysPerMonth * MonthsPerYear; // 360

        // --- Properties ---
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public CalendarDate() { }

        public CalendarDate(int day, int month, int year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        // --- Derived Properties ---

        /// <summary>
        /// The current season based on month (1-3 Winter, 4-6 Spring, 7-9 Summer, 10-12 Autumn).
        /// </summary>
        public Seasons CurrentSeason()
        {
            if (Month >= 1 && Month <= 3) 
                return Seasons.Winter;
            else if (Month >= 4 && Month <= 6)
                    return Seasons.Spring;
            else if (Month >= 7 && Month <= 9)
                return Seasons.Summer;
            else if (Month >= 10 && Month <= 12)
                return Seasons.Autumn;
            else
                throw new InvalidMonthException();

        }

        /// <summary>
        /// Absolute day count from Year 0, Day 1. Useful for comparisons and math.
        /// </summary>
        public int ToDayNumber() => (Year * DaysPerYear) + ((Month - 1) * DaysPerMonth) + Day;

        /// <summary>
        /// Reconstruct a CalendarDate from an absolute day number.
        /// </summary>
        public static CalendarDate FromDayNumber(int dayNumber)
        {
            int year = dayNumber / DaysPerYear;
            int remainder = dayNumber % DaysPerYear;
            int month = (remainder / DaysPerMonth) + 1;
            int day = remainder % DaysPerMonth;

            // Handle day 0 → last day of previous month
            if (day == 0)
            {
                day = DaysPerMonth;
                month--;
                if (month == 0)
                {
                    month = MonthsPerYear;
                    year--;
                }
            }

            return new CalendarDate(day, month, year);
        }

        /// <summary>
        /// Returns a new CalendarDate advanced by the given number of days.
        /// </summary>
        public CalendarDate AddDays(int days)
        {
            return FromDayNumber(ToDayNumber() + days);
        }

        // --- Display ---

        /// <summary>
        /// Placeholder display until month names are defined in lore.
        /// Example: "Day 15, Month 3, Year 412"
        /// </summary>
        public override string ToString()
        {
            return $"Day {Day}, Month {Month}, Year {Year}";
        }

        // --- Comparison & Equality ---

        public int CompareTo(CalendarDate? other)
        {
            if (other is null) return 1;
            return ToDayNumber().CompareTo(other.ToDayNumber());
        }

        public bool Equals(CalendarDate? other)
        {
            if (other is null) return false;
            return Day == other.Day && Month == other.Month && Year == other.Year;
        }

        public override bool Equals(object? obj) => Equals(obj as CalendarDate);
        public override int GetHashCode() => HashCode.Combine(Day, Month, Year);

        public static bool operator ==(CalendarDate? left, CalendarDate? right)
            => left is null ? right is null : left.Equals(right);
        public static bool operator !=(CalendarDate? left, CalendarDate? right)
            => !(left == right);
        public static bool operator <(CalendarDate left, CalendarDate right)
            => left.CompareTo(right) < 0;
        public static bool operator >(CalendarDate left, CalendarDate right)
            => left.CompareTo(right) > 0;
        public static bool operator <=(CalendarDate left, CalendarDate right)
            => left.CompareTo(right) <= 0;
        public static bool operator >=(CalendarDate left, CalendarDate right)
            => left.CompareTo(right) >= 0;
    }
}
