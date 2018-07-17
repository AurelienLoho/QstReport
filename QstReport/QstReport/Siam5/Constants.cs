/**********************************************************************************************/
/**** Fichier : Siam5/Constants.cs                                                         ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam5
{
    public sealed class Constants
    {
        /// <summary>
        /// L'URL pour se connecter.
        /// </summary>
        public const string CONNECT_URL = "/actuel/lib/core/auth/";

        /// <summary>
        /// L'URL de la page 'agenda'.
        /// </summary>
        public const string AGENDA_URL = "/actuel/appli/planner/?action=doFilter";

        /// <summary>
        /// L'URL pour récupérer les évènements notifiés.
        /// </summary>
        public const string EVENTS_URL = "/actuel/appli/daybook/?action=doFilter";

        /// <summary>
        /// L'URL pour les détails d'un AVT.
        /// </summary>
        public const string AVT_DETAIL_URL = "/actuel/appli/planner/occurrence/?action=doDisplay&source=PLN&mode=search&popup=1&Occurrence[id]=";
    }
}
