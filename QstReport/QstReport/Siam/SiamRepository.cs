/**********************************************************************************************/
/**** Fichier : SiamRepository.cs                                                          ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam
{
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Linq;
    using QstReport.DataModel;

    public sealed class SiamRepository : IDisposable
    {
        /// <summary>
        /// L'URL de SIAM
        /// </summary>
        private readonly string _siamUrl;

        /// <summary>
        /// Le gestionnaire de session HTTP.
        /// </summary>
        private readonly HttpSessionHandler _httpSession;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SiamRepository"/>.
        /// </summary>
        /// <param name="url">L'URL de SIAM.</param>
        /// <param name="userName">Le nom d'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        public SiamRepository(string url, string userName, string password)
        {
            _siamUrl = url;
            _httpSession = new HttpSessionHandler("siam.tech.cana.ri"); // TODO : find a way to get host name from url

            Connect(userName, password);
        }

        /// <summary>
        /// Récupère les avis de travaux dans une période donnée.
        /// </summary>
        /// <param name="startDate">La date du début de la période.</param>
        /// <param name="endDate">La date fin de la période.</param>
        /// <returns>Une collection d'avis de travaux.</returns>
        public List<Avt> GetAvts(DateTime startDate, DateTime endDate)
        {
            var avtIds = GetAvtIds(startDate, endDate);

            return avtIds.Select(x => GetAvtData(x)).ToList();
        }

        public List<TechEvent> GetTechEvents(DateTime startDate, DateTime endDate)
        {
            var baseUrl = _siamUrl + SiamConstants.MAIN_COURANTE_URL;

            var events = new List<TechEvent>();

            foreach (var day in startDate.EachDayTo(endDate))
            {
                var requestUrl = string.Format("{0}&select={1}", baseUrl, day.ToString("dd-MM-yyyy"));

                using (var httpResponse = _httpSession.SendGetRequest(requestUrl))
                {
                    var htmlDoc = httpResponse.AsHtml();
                    var eventNodes = htmlDoc.SelectNodes("//table[@class='tblContext']/tbody/tr");

                    if (eventNodes == null)
                    {
                        return new List<TechEvent>();
                    }

                    var foundEvents = eventNodes.Select(x => SiamParser.ParseHtmlAsTechEvent(x, day))
                                     .Where(x => x != null && x.Modified == false);

                    events.AddRange(foundEvents);
                }
            }

            return events;
        }

        private Avt GetAvtData(int agendaIndex)
        {
            var requestUrl = string.Format("{0}/actuel/appli/agenda/?id={1}", _siamUrl, agendaIndex);

            using (var httpResponse = _httpSession.SendGetRequest(requestUrl))
            {
                return SiamParser.ParseHtmlAsAvt(httpResponse.AsHtml());
            }
        }

        private List<int> GetAvtIds(DateTime startDate, DateTime endDate)
        {
            var avts = new List<Tuple<int, string>>();

            foreach (var week in startDate.EachWeekTo(endDate))
            {
                var requestUrl = string.Format("{0}{1}&select={2}&mode=semaine", _siamUrl, SiamConstants.AGENDA_URL, week.Start.Date.ToString("dd-MM-yyyy"));

                using (var httpResponse = _httpSession.SendGetRequest(requestUrl))
                {
                    avts.AddRange(SiamParser.GetAvtIds(httpResponse.AsHtml()));
                }
            }

            return avts.Where(x => x != null)
                       .DistinctBy(x => x.Item2)
                       .Select(x => x.Item1)
                       .ToList();
        }

        private void Connect(string userName, string password)
        {
            try
            {
                // Envoi des informations de login
                var credentialInfos = string.Format("requete=%2Factuel%2F&erreurOK=1&pseudo={0}&mot_de_passe={1}&submitBtn=", userName, password);

                using (var response = _httpSession.SendPostRequest(_siamUrl + SiamConstants.CONNECT_URL, credentialInfos))
                { }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se connecter au serveur SIAM.");
            }
        }

        private void Disconnect()
        {
            try
            {
                using (var response = _httpSession.SendGetRequest(_siamUrl + SiamConstants.DISCONNECT_URL))
                { }

                // confirmation de la déconnexion
                var disconnectParameters = string.Format("id_utilisateur={0}&confirmer=&deconnexion=1", SiamConstants.USER_ID);

                using (var response = _httpSession.SendPostRequest(_siamUrl + SiamConstants.DISCONNECT_URL, disconnectParameters))
                { }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se déconnecter du serveur SIAM.");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).

                    Disconnect();
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~SiamRepository() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}

