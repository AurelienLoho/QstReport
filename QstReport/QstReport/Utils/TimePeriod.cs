/**********************************************************************************************/
/**** Fichier : TimePeriod.cs                                                              ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Une période de temps.
    /// </summary>
    [DebuggerDisplay("du {Start} au {End}")]
    public class TimePeriod
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="TimePeriod"/>.
        /// </summary>
        /// <param name="start">Le début de la période.</param>
        /// <param name="end">La fin de la période.</param>
        public TimePeriod(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="TimePeriod"/>.
        /// </summary>
        /// <param name="start">Le début de la période.</param>
        /// <param name="duration">La durée de la période.</param>
        public TimePeriod(DateTime start, TimeSpan duration)
        {
            Start = start;
            End = start + duration;
        }

        /// <summary>
        /// La date de début de la période.
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// La date de fin de la période.
        /// </summary>
        public DateTime End { get; private set; }

        /// <summary>
        /// La durée de la période.
        /// </summary>
        public TimeSpan Duration { get { return End - Start; } }

        /// <summary>
        /// Enumère tous les jours de la semaine.
        /// </summary>
        public IEnumerable<DateTime> Days
        {
            get { return Start.EachDayTo(End); }
        }

        /// <summary>
        /// Indique si la date spécifiée est contenue dans la semaine.
        /// </summary>
        /// <param name="date">Une date à vérifier.</param>
        /// <returns>true si la date spcifiée appartient bien à la semaine, false autrement.</returns>
        public bool ContainsDate(DateTime date)
        {
            return (date >= Start) && (date <= End);
        }
    }
}
