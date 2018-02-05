using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QstReport.Utils
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// La durée d'une semaine.
        /// </summary>
        private static readonly TimeSpan FullWeekDuration = new TimeSpan(6, 23, 59, 59);

        /// Enumère les jours entre deux dates.
        /// </summary>
        /// <param name="start">La date de départ.</param>
        /// <param name="end">La date de fin.</param>
        /// <returns>Un objet <see cref="DateTime"/> pour chaque jour entre <paramref name="start"/> et <paramref name="end"/>.</returns>
        public static IEnumerable<DateTime> EachDayTo(this DateTime start, DateTime end)
        {
            var current = start.Date;
            var endDate = end.Date;

            while (current <= endDate)
            {
                yield return current;
                current = current.AddDays(1.0);
            }
        }

        public static IEnumerable<Week> EachWeekTo(this DateTime start, DateTime end)
        {
            var current = start.Date;
            var endDate = end.Date;

            while (current <= endDate)
            {
                yield return new Week(current);
                current = current.AddDays(7.0);
            }
        }

        /// <summary>
        /// Calcule le numéro de la semaine (selon la norme ISO 8601) pour la date spécifiée.
        /// </summary>
        /// <param name="date">Une date.</param>
        /// <returns>Le numéro de la semaine.</returns>
        public static int GetIso8601WeekNumber(this DateTime date)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Retourne le premier le jour de la semaine contenant la date spécifiée.
        /// </summary>              
        /// <returns>Le premier jour de la semaine contenant la date spécifiée.</returns>
        public static DateTime FirstDateOfWeek(this DateTime date)
        {
            int daysDelta = DayOfWeek.Monday - date.DayOfWeek;

            return date.Date.AddDays(daysDelta > 0 ? daysDelta - 7 : daysDelta);
        }

        /// <summary>
        /// Retourne le dernier le jour de la semaine contenant la date spécifiée.
        /// </summary>
        /// <param name="date">Une date.</param>
        /// <returns>Le dernier jour de la semaine contenant la date spécifiée.</returns>
        public static DateTime LastDateOfWeek(this DateTime date)
        {
            return date.FirstDateOfWeek().Add(FullWeekDuration);
        }
    }
}
