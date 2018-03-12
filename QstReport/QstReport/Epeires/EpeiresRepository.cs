/**********************************************************************************************/
/**** Fichier : EpeiresRepository.cs                                                       ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Epeires
{
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class EpeiresRepository : IDisposable
    {
        /// <summary>
        /// Le gestionnaire de session HTTP.
        /// </summary>
        private readonly HttpSessionHandler _httpSession;

        private readonly string _epeiresUrl;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SiamRepository"/>.
        /// </summary>
        /// <param name="url">L'URL de SIAM.</param>
        /// <param name="userName">Le nom d'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        public EpeiresRepository(string url, string userName, string password)
        {
            _epeiresUrl = url;
            _httpSession = new HttpSessionHandler("epeires.cdg-lb.aviation", false); // TODO : find a way to get host name from url

            Connect(userName, password);
        }

        public List<ExploitEvent> GetExploitEvents(DateTime startDate, DateTime endDate)
        {
            var evts = new List<RawEpeiresEvent>();

            var url = string.Format("{0}/events/geteventsFC?startdate={1}&enddate={2}", _epeiresUrl,
                                                                                        startDate.ToString("yyyy-MM-dd"),
                                                                                        endDate.ToString("yyyy-MM-dd"));

            using (var response = _httpSession.SendGetRequest(url))
            {
                var data = response.AsJson<List<RawEpeiresEvent>>();

                if (data != null)
                {
                    evts.AddRange(data);
                }
            }

            return ToExploitEventCollection(evts);
        }

        private List<ExploitEvent> ToExploitEventCollection(IEnumerable<RawEpeiresEvent> evts)
        {
            return evts.Where(y => y.StatusId != 5) // StatusID = 5 => évènement supprimé
                       .Select(x =>
                                {
                                    var fieldArray = x.Fields.Values.ToArray();

                                    return new ExploitEvent
                                    {
                                        StartDate = x.StartDate,
                                        EndDate = x.EndDate, 
                                        Title = fieldArray.Length >= 1 ? fieldArray[0] : string.Empty,
                                        Description = fieldArray.Length >= 2 ? fieldArray[1] : string.Empty
                                    };
                                })
                       .ToList();
        }

        private void Connect(string userName, string password)
        {
            try
            {
                // Envoi des informations de login
                var credentialInfos = string.Format("identity={0}&credential={1}&redirect=application&submit=", userName, password);

                using (var response = _httpSession.SendPostRequest(_epeiresUrl + "/user/login?redirect=application", credentialInfos))
                { }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se connecter au serveur EPEIRES.");
            }
        }

        private void Disconnect()
        {
            try
            {
                using (var response = _httpSession.SendGetRequest(_epeiresUrl + "/user/logout?redirect=/"))
                { }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se déconnecter du serveur EPEIRES.");
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
