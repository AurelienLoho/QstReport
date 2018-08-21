/**********************************************************************************************/
/**** Fichier : Siam5/Repository.cs                                                        ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam5
{
    using HtmlAgilityPack;
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
        private readonly SiamHttpSessionHandler _httpSession;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SiamRepository"/>.
        /// </summary>
        /// <param name="hostName">Le nom d'hôte de SIAM.</param>
        /// <param name="userName">Le nom d'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        public Repository(string hostName, string userName, string password)
        {
            _siamHostName = hostName;
            _httpSession = new SiamHttpSessionHandler(hostName);

            Connect(userName, password);
        }

        /// <summary>
        /// Récupère la liste des Avis de Travaux notifiés dans SIAM.
        /// </summary>
        /// <param name="startDate">Le début de la période de recherche.</param>
        /// <param name="endDate">La fin de la période de recherche.</param>
        /// <returns>Une collection d'AVT notifiés.</returns>
        public List<Avt> GetAvts(DateTime startDate, DateTime endDate)
        {
            var avts = new List<Avt>();

            var requestUrl = string.Format("{0}{1}", _siamHostName, Constants.AGENDA_URL);
            var requestData = FormatSearchPostData(startDate, endDate, "PLN");

            using (var httpResponse = _httpSession.SendPostRequest(requestUrl, requestData))
            {
                var result = httpResponse.AsJson<Siam5RequestResult<Siam5AvtData>>();

                var uniqueRefs = result.success.items.DistinctBy(x => x.id_evt)
                                                    .ToList();

                foreach (var t in uniqueRefs)
                {
                    var detailRequest = string.Format("{0}{1}{2}", _siamHostName, Constants.AVT_DETAIL_URL, t.id_occurrence);

                    using (var detailResponse = _httpSession.SendDetailAvtRequest(detailRequest))
                    {
                        var avt = Parser.ParseHtmlAsAvt(detailResponse.AsHtml());

                        if (avt != null)
                        {
                            avt.RefSiam = t.id;

                            avts.Add(avt);
                        }
                    }
                }
            }

            return avts;
        }

        /// <summary>
        /// Récupère la liste des évènements notifiés dans SIAM V5.
        /// </summary>
        /// <param name="startDate">Le début de la période de recherche.</param>
        /// <param name="endDate">La fin de la période de recherche.</param>
        /// <returns>Une collection d'évènements notifiés.</returns>
        public List<TechEvent> GetTechEvents(DateTime startDate, DateTime endDate)
        {
            var events = new List<TechEvent>();

            var requestUrl = string.Format("{0}{1}", _siamHostName, Constants.EVENTS_URL);
            var requestData = FormatSearchPostData(startDate, endDate, "DBK");

            using (var httpResponse = _httpSession.SendPostRequest(requestUrl, requestData))
            {
                var result = httpResponse.AsJson<Siam5RequestResult<Siam5EventData>>();

                foreach (var item in result.success.items)
                {
                    events.Add(ToTechEvent(item));
                }
            }

            return events.Where(x => !x.Modified).ToList();
        }

        /// Connexion à SIAM.
        /// </summary>
        /// <param name="userName">Le nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        private void Connect(string userName, string password)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                                        {
                                            { "form_button", "submitBtn"},
                                            {"source", "ACC"},
                                            {"mode", "search"},
                                            {"uri", "//actuel//"},
                                            {"action", "authenticate"},
                                            {"pseudo", userName },
                                            { "mot_de_passe", password },
                                        };

                using (var response = _httpSession.SendConnectRequest(_siamHostName + Constants.CONNECT_URL, parameters))
                {
                }
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
                var parameters = new Dictionary<string, string>
                                        {
                                            { "form_button", "submitBtn"},
                                            {"source", "ATH"},
                                            {"mode", "search"},
                                            {"action", "doLogout"},
                                        };

                using (var response = _httpSession.SendDisconnectRequest(_siamHostName + Constants.CONNECT_URL, parameters))
                {
                }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se déconnecter du serveur SIAM.");
            }
        }

        private string FormatSearchPostData(DateTime periodStart, DateTime periodEnd, string sourceData)
        {
            var requestFormat = "mode=search&refiners=refiner_window%7Crefiner_toManage%7Crefiner_moduletypes%7Crefiner_sites-1%7Crefiner_chains%7Crefiner_managers-1%7Crefiner_tags%7Crefiner_reference%7Crefiner_keywords%7Crefiner_date%7Crefiner_sites-2%7Crefiner_managers-2%7Crefiner_supervisions%7Crefiner_colors&refiner_window={0}%3B{1}%3B-1%3Bmonths%3B-0%3Bdays&refiner_moduletypes=&refiner_sites-1=&refiner_managers-1=&refiner_tags=&refiner_date=10%2F07%2F2018&refiner_sites-2=&refiner_managers-2=&refiner_supervisions=&refiner_colors=statusCS&search_dateRef=&search_totalItems=&search_itemsPerPage=300&search_order=ASC&source={2}";

            return HtmlEntity.Entitize(string.Format(requestFormat, periodStart.ToString("dd/MM/yyyy"), periodEnd.ToString("dd/MM/yyyy"), sourceData));
        }

        private TechEvent ToTechEvent(Siam5EventData eventData)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(eventData.html);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//td");

            var title = HtmlEntity.DeEntitize(htmlNodes.ElementAt(7).InnerText);
            var refSiam = htmlNodes.ElementAt(1).InnerText;
            var equipment = HtmlEntity.DeEntitize(htmlNodes.ElementAt(5).InnerText);

            var rexAsked = false;
            var incidentTelecom = false;
            var brouillage = false;
            var following = false;

            var options = htmlNodes.ElementAt(7).SelectNodes("//img");
            if(options != null && options.Count > 0)
            {
                foreach(var n in options)
                {
                    var altAttribute = HtmlEntity.DeEntitize(n.GetAttributeValue("alt", string.Empty)).ToLower();

                    if (altAttribute.StartsWith("évè"))
                    {
                        following = true;
                        continue;
                    }
                    if(altAttribute.StartsWith("rex"))
                    {
                        rexAsked = true;
                        continue;
                    }
                    if (altAttribute.StartsWith("bro"))
                    {
                        brouillage = true;
                        continue;
                    }
                    if (altAttribute.StartsWith("inc"))
                    {
                        incidentTelecom = true;
                        continue;
                    }
                }
            }

            var duration = TimeSpan.FromSeconds(eventData.to - eventData.from);
            
            return new TechEvent
            {
                IdSiam = eventData.id_evt,
                ReferenceSiam = refSiam,
                StartDate = UnixEpoch.AddSeconds(eventData.from),
                Title = title,
                Duration = duration,
                CalculatedDuration = ToHumanReadableString(duration),
                Group = equipment,
                RexAsked = rexAsked,
                Telecom = incidentTelecom,
                Jamming = brouillage,
                Modified = following,
            };
        }

        public static string ToHumanReadableString(TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return "sD";
            }
            if (t.TotalMinutes <= 1)
            {
                return string.Format("{0:ss} s", t);
            }
            if (t.TotalHours <= 1)
            {
                return string.Format("{0:mm} m", t);
            }
            if (t.TotalDays <= 1)
            {
                return string.Format("{0:hh} h", t);
            }

            return string.Format("{0:dd} j", t);
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

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
