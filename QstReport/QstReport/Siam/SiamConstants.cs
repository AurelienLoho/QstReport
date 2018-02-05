/**********************************************************************************************/
/**** Fichier : SiamConstants.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam
{
    /// <summary>
    /// Ensemble de constantes pour communiquer avec SIAM.
    /// </summary>
    public sealed class SiamConstants
    {
        /// <summary>
        /// L'URL pour se connecter.
        /// </summary>
        public const string CONNECT_URL = "/appli/commun/utilisateur/connecter.php";

        /// <summary>
        /// L'URL pour se déconnecter.
        /// </summary>
        public const string DISCONNECT_URL = "/appli/commun/utilisateur/deconnecter.php";

        /// <summary>
        /// L'URL de la page 'main courante'.
        /// </summary>
        public const string MAIN_COURANTE_URL = "/appli/main_courante/?action=lister";

        /// <summary>
        /// L'URL de la page 'agenda'.
        /// </summary>
        public const string AGENDA_URL = "/appli/agenda/?action=lister";

        /// <summary>
        /// L'URL de la page 'recherche'.
        /// </summary>
        public const string SEARCH_URL = "/appli/commun/recherche/?action=rechercher";

        /// <summary>
        /// L'URL pour une fiche d'évènement.
        /// </summary>
        public const string EVENT_DETAIL_URL = "/appli/fiche/?id=";

        /// <summary>
        /// L'URL pour une fiche REX.
        /// </summary>
        public const string REX_DETAIL_URL = "/appli/esarr2/rex/?action=afficher&id=";

        /// <summary>
        /// Identifiant de l'utilisateur.
        /// </summary>
        public const int USER_ID = 7;
    }
}
