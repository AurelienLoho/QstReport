/**********************************************************************************************/
/**** Fichier : TechEvent.cs                                                               ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Un évènement saisi dans SIAM.
    /// </summary>
    [DebuggerDisplay("{StartDate} : {ReferenceSiam} - {Title}")]
    public sealed class TechEvent
    {/// <summary>
        /// L'identifiant SIAM.
        /// </summary>
        public int IdSiam { get; set; }

        /// <summary>
        /// La référence SIAM.
        /// </summary>
        public string ReferenceSiam { get; set; }

        /// <summary>
        /// Date de l'évènement.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// La durée de l'évènement.
        /// </summary>
        public TimeSpan Duration { get; set; }

        public string CalculatedDuration { get; set; }

        /// <summary>
        /// Le libellé de l'évènement.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// L'évènement concerne LBG.
        /// </summary>
        public bool IsLbg { get; set; }

        /// <summary>
        /// Indique si un REX a été demandé.
        /// </summary>
        public bool RexAsked { get; set; }

        /// <summary>
        /// Indique si l'évènement a été jugé important.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// La chaîne liée à l'évènement.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Indique si l'évènement est une modification d'un évènement passée.
        /// </summary>
        public bool Modified { get; set; }
    }
}
