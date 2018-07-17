/**********************************************************************************************/
/**** Fichier : SiamRepository.cs                                                          ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam4
{
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public sealed class Repository : IDisposable
    {
        /// <summary>
        /// Le nom d'hôte de SIAM.
        /// </summary>
        private readonly string _siamHostName;

        /// <summary>
        /// Le gestionnaire de session HTTP.
        /// </summary>
        private readonly HttpSessionHandler _httpSession;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Repository"/>.
        /// </summary>
        /// <param name="hostName">Le nom d'hôte de SIAM.</param>
        /// <param name="userName">Le nom d'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        public Repository(string hostName, string userName, string password)
        {
            _siamHostName = hostName;
            _httpSession = new HttpSessionHandler(hostName);

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

        /// <summary>
        /// Récupère les évènements notifiés pendant la période spécifiée.
        /// </summary>
        /// <param name="startDate">Le début de la période de recherche.</param>
        /// <param name="endDate">La fin de la période de recheche.</param>
        /// <returns>Une collection d'évènements notifiés.</returns>
        public List<TechEvent> GetTechEvents(DateTime startDate, DateTime endDate)
        {
            var baseUrl = _siamHostName + Constants.MAIN_COURANTE_URL;

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
                    //    return new List<TechEvent>();
                        continue;
                    }

                    var foundEvents = eventNodes.Select(x => Parser.ParseHtmlAsTechEvent(x, day))
                                     .Where(x => x != null && x.Modified == false);

                    events.AddRange(foundEvents);
                }
            }

            return events;
        }

        /// <summary>
        /// Récupère les données d'un AVT à partir de son identifiant dans l'agenda.
        /// </summary>
        /// <param name="agendaIndex">L'identifiant de l'AVT dans l'agenda.</param>
        /// <returns>Les données concernant l'AVT.</returns>
        private Avt GetAvtData(int agendaIndex)
        {
            var requestUrl = string.Format("{0}/actuel/appli/agenda/?id={1}", _siamHostName, agendaIndex);

            using (var httpResponse = _httpSession.SendGetRequest(requestUrl))
            {
                return Parser.ParseHtmlAsAvt(httpResponse.AsHtml());
            }
        }

        /// <summary>
        /// Récupère une liste d'identifiants correspondants aux AVTs présents dans l'agenda pendant la période spécifiée.
        /// </summary>
        /// <param name="startDate">Le début de la période de recherche.</param>
        /// <param name="endDate">La fin de la période de recherche.</param>
        /// <returns>Une collection d'identifiant unique correspondants aux AVTs recherchés.</returns>
        private List<int> GetAvtIds(DateTime startDate, DateTime endDate)
        {
            var avts = new List<Tuple<int, string>>();

            foreach (var week in startDate.EachWeekTo(endDate))
            {
                var requestUrl = string.Format("{0}{1}&select={2}&mode=semaine", _siamHostName, Constants.AGENDA_URL, week.Start.Date.ToString("dd-MM-yyyy"));

                using (var httpResponse = _httpSession.SendGetRequest(requestUrl))
                {
                    avts.AddRange(Parser.GetAvtIds(httpResponse.AsHtml()));
                }
            }

            return avts.Where(x => x != null)
                       .DistinctBy(x => x.Item2)
                       .Select(x => x.Item1)
                       .ToList();
        }

        /// <summary>
        /// Connexion à SIAM.
        /// </summary>
        /// <param name="userName">Le nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        private void Connect(string userName, string password)
        {
            try
            {
                // Envoi des informations de login
                var credentialInfos = string.Format("requete=%2Factuel%2F&erreurOK=1&pseudo={0}&mot_de_passe={1}&submitBtn=", userName, password);

                using (var response = _httpSession.SendPostRequest(_siamHostName + Constants.CONNECT_URL, credentialInfos))
                { }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se connecter au serveur SIAM.");
            }
        }


        /// <summary>
        /// Déconnexion de SIAM.
        /// </summary>
        private void Disconnect()
        {
            try
            {
                using (var response = _httpSession.SendGetRequest(_siamHostName + Constants.DISCONNECT_URL))
                { }

                // confirmation de la déconnexion
                var disconnectParameters = string.Format("id_utilisateur={0}&confirmer=&deconnexion=1", Constants.USER_ID);

                using (var response = _httpSession.SendPostRequest(_siamHostName + Constants.DISCONNECT_URL, disconnectParameters))
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

