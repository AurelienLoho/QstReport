/**********************************************************************************************/
/**** Fichier : Avt.cs                                                                     ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using QstReport.Utils;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Id}")]
    public class Avt
    {
        /// <summary>
        /// La référence interne à SIAM.
        /// </summary>
        public string RefSiam { get; set; }

        /// <summary>
        /// La référence de l'AVT.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Le titre de l'AVT.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// La description des travaux.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Les conséquences prévues des travaux.
        /// </summary>
        public string Consequences { get; set; }

        /// <summary>
        /// Le pôle rédacteur de l'AVT.
        /// </summary>
        public Pole Pole { get; set; }

        /// <summary>
        /// L'entité rédactrice de l'AVT.
        /// </summary>
        public Entite Entite { get; set; }

        /// <summary>
        /// Le type de d'avis de travaux.
        /// </summary>
        /// <remarks>
        /// Le traitement des avis de travaux est différent pour la QST selon qu'il s'agit d'un AVT simple, d'une MISO ou d'une MICSO.
        /// </remarks>
        public AvtType AvtType { get; set; }

        /// <summary>
        /// La liste des sites impactés par les travaux.
        /// </summary>
        public List<string> ImpactedAirports { get; set; }

        /// <summary>
        /// La liste des équipements impactés par les travaux.
        /// </summary>
        public List<string> ImpactedEquipments { get; set; }

        /// <summary>
        /// La liste des superviseurs concernés par les travaux.
        /// </summary>
        public List<Superviseur> ImpactedSupervisor { get; set; }

        /// <summary>
        /// Indique si l'avis de travaux a été annulé.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Indique si l'avis de travaux a été validé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// L'ensemble des périodes de travail couvertes par cet AVT.
        /// </summary>
        public TimePeriodCollection WorkPeriods { get; set; }
    }
}

