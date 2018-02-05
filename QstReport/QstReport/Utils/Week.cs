/**********************************************************************************************/
/**** Fichier : Week.cs                                                                    ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Représente une semaine du calendrier.
    /// </summary>
    public sealed class Week
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Week"/> à partir d'une date.
        /// </summary>
        /// <param name="dateInWeek">Une date dans la semaine.</param>
        public Week(DateTime dateInWeek)
        {
            WeekNumber = dateInWeek.GetIso8601WeekNumber();
            Start = dateInWeek.FirstDateOfWeek();
            End = dateInWeek.LastDateOfWeek();
        }

        /// <summary>
        /// Le numéro de la semaine dans l'année (standard ISO 8601).
        /// </summary>
        public int WeekNumber { get; private set; }

        /// <summary>
        /// Une date représentant le début de la semaine (Lundi à 00h00).
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// Une date représentant la fin de la semaine (Dimanche à 23h59)
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
        public bool Contains(DateTime date)
        {
            return (date >= Start) && (date <= End);
        }

        /// <summary>
        /// Retourne la semaine suivante du calendrier.
        /// </summary>
        /// <returns>La semaine suivante.</returns>
        public Week NextWeek()
        {
            return new Week(End.AddDays(1.0));
        }

        /// <summary>
        /// Retourne la semaine précédente du calendrier.
        /// </summary>
        /// <returns>La semaine précédente.</returns>
        public Week PreviousWeek()
        {
            return new Week(Start.AddDays(-1.0));
        }

        /// <summary>
        /// La semaine courante.
        /// </summary>
        public static Week CurrentWeek { get { return new Week(DateTime.Now); } }
    }
}

