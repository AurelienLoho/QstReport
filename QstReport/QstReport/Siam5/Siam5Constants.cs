
namespace QstReport.Siam5
{
    public sealed class Siam5Constants
    {
        /// <summary>
        /// L'URL pour se connecter.
        /// </summary>
        public const string CONNECT_URL = "/actuel/lib/core/auth/";

        /// <summary>
        /// L'URL de la page 'agenda'.
        /// </summary>
        public const string AGENDA_URL = "/actuel/appli/planner/?action=doFilter";

        public const string EVENTS_URL = "/actuel/appli/daybook/?action=doFilter";

        /// <summary>
        /// L'URL pour les détails d'un AVT.
        /// </summary>
        public const string AVT_DETAIL_URL = "/actuel/appli/planner/occurrence/?action=doDisplay&source=PLN&mode=search&popup=1&Occurrence[id]=";
    }
}
