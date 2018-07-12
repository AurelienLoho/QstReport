namespace QstReport.Siam5
{
    using HtmlAgilityPack;
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    public sealed class Repository : IDisposable
    {
        private const string formDataDelimeter = "-----------------------------195142331314649";

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

        public List<Avt> GetAvts(DateTime startDate, DateTime endDate)
        {
            var avts = new List<Avt>();

            var requestUrl = string.Format("{0}{1}", _siamHostName, Siam5Constants.AGENDA_URL);
            var requestData = FormatAvtSearchPostData(startDate, endDate);

            using (var httpResponse = _httpSession.SendPostRequest(requestUrl, requestData))
            {
                var result = httpResponse.AsJson<Siam5RequestResult<Siam5AvtData>>();

                var uniqueRefs = result.success.items.DistinctBy(x => x.id_evt)
                                                    .ToList();

                foreach (var t in uniqueRefs)
                {
                    var detailRequest = string.Format("{0}{1}{2}", _siamHostName, Siam5Constants.AVT_DETAIL_URL, t.id_occurrence);

                    using (var detailResponse = _httpSession.SendDetailAvtRequest(detailRequest))
                    {
                        var avt = Parser.ParseHtmlAsAvt(detailResponse.AsHtml());
                        avt.RefSiam= t.id;

                        avts.Add(avt);
                    }
                }
            }

            return avts;
        }

        public List<TechEvent> GetTechEvents(DateTime startDate, DateTime endDate)
        {
            var events = new List<TechEvent>();

            var requestUrl = string.Format("{0}{1}", _siamHostName, Siam5Constants.EVENTS_URL);
            var requestData = FormatTechEventSearchPostData(startDate, endDate);

            using(var httpResponse = _httpSession.SendPostRequest(requestUrl, requestData))
            {
                var result = httpResponse.AsJson<Siam5RequestResult<Siam5EventData>>();

                foreach(var item in result.success.items)
                {
                    events.Add(ToTechEvent(item));
                }
            }

            return events;
        }

        /// Connexion à SIAM.
        /// </summary>
        /// <param name="userName">Le nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        private void Connect(string userName, string password)
        {
            try
            {
                var requestData = GetConnectRequestPostData(userName, password);
                using (var response = _httpSession.SendConnectRequest(_siamHostName + Siam5Constants.CONNECT_URL, requestData))
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
                var requestData = GetDisconnectRequestPostData();
                using (var response = _httpSession.SendDisconnectRequest(_siamHostName + Siam5Constants.CONNECT_URL, requestData))
                {
                }
            }
            catch
            {
                throw new HttpRequestException("Impossible de se déconnecter du serveur SIAM.");
            }
        }

        public string GetConnectRequestPostData(string userName, string password)
        {
            StringBuilder sb = new StringBuilder();

            const string delimeter = "-----------------------------195142331314649\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";

            sb.AppendFormat(delimeter, "form_button", "submitBtn");
            sb.AppendFormat(delimeter, "source", "ACC");
            sb.AppendFormat(delimeter, "mode", "search");
            sb.AppendFormat(delimeter, "uri", "//actuel//");
            sb.AppendFormat(delimeter, "action", "authenticate");
            sb.AppendFormat(delimeter, "pseudo", userName);
            sb.AppendFormat(delimeter, "mot_de_passe", password);
            sb.AppendFormat(delimeter, "action", "authenticate");

            sb.Append("-----------------------------195142331314649--");

            return sb.ToString();
        }

        public string GetDisconnectRequestPostData()
        {
            StringBuilder sb = new StringBuilder();

            const string delimeter = "-----------------------------195142331314649\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";

            sb.AppendFormat(delimeter, "form_button", "submitBtn");
            sb.AppendFormat(delimeter, "source", "ATH");
            sb.AppendFormat(delimeter, "mode", "search");
            sb.AppendFormat(delimeter, "action", "doLogout");
            sb.AppendFormat(delimeter, "action", "doLogout");

            sb.Append("-----------------------------195142331314649--");

            return sb.ToString();
        }

        public string FormatAvtSearchPostData(DateTime periodStart, DateTime periodEnd)
        {
            //StringBuilder sb = new StringBuilder();

            //sb.Append("&mode=search");
            //sb.Append("&refiners=refiner_window|refiner_toManage|refiner_moduletypes|refiner_sites-1|refiner_chains|refiner_managers-1|refiner_tags|refiner_reference|refiner_keywords|refiner_date|refiner_sites-2|refiner_managers-2|refiner_supervisions|refiner_colors");
            //sb.AppendFormat("&refiner_window={0};{1};-1;months;-0;days", periodStart.ToString("dd/MM/YYYY"), periodEnd.ToString("dd/MM/YYYY"));
            //sb.Append("&refiner_moduletypes=TVX");
            ////sb.Append("&refiner_sites-1=");
            //sb.AppendFormat("&refiner_date={0}", DateTime.Now.ToString("dd/MM/YYYY"));
            //sb.Append("&refiner_colors=statusCS");
            //sb.Append("&search_itemsPerPage=200");
            //sb.Append("&search_order=ASC");
            //sb.Append("&source=PLN");

            //return sb.ToString();

            return "mode=search&refiners=refiner_window%7Crefiner_toManage%7Crefiner_moduletypes%7Crefiner_sites-1%7Crefiner_chains%7Crefiner_managers-1%7Crefiner_tags%7Crefiner_reference%7Crefiner_keywords%7Crefiner_date%7Crefiner_sites-2%7Crefiner_managers-2%7Crefiner_supervisions%7Crefiner_colors&refiner_window=16%2F07%2F2018%3B22%2F07%2F2018%3B-1%3Bmonths%3B-0%3Bdays&refiner_moduletypes=&refiner_sites-1=&refiner_managers-1=&refiner_tags=&refiner_date=10%2F07%2F2018&refiner_sites-2=&refiner_managers-2=&refiner_supervisions=&refiner_colors=statusCS&search_dateRef=&search_totalItems=&search_itemsPerPage=30&search_order=ASC&source=PLN";
        }

        private string FormatTechEventSearchPostData(DateTime periodStart, DateTime periodEnd)
        {
            return "mode=search&refiners=refiner_window%7Crefiner_toManage%7Crefiner_moduletypes%7Crefiner_sites-1%7Crefiner_chains%7Crefiner_managers-1%7Crefiner_tags%7Crefiner_reference%7Crefiner_keywords%7Crefiner_date%7Crefiner_sites-2%7Crefiner_managers-2%7Crefiner_supervisions%7Crefiner_colors&refiner_window=12%2F04%2F2018%3B12%2F07%2F2018%3B-1%3Bmonths%3B-0%3Bdays&refiner_moduletypes=&refiner_sites-1=&refiner_managers-1=&refiner_tags=&refiner_date=10%2F07%2F2018&refiner_sites-2=&refiner_managers-2=&refiner_supervisions=&refiner_colors=statusCS&search_dateRef=&search_totalItems=&search_itemsPerPage=30&search_order=ASC&source=DBK";
        
        }

        private TechEvent ToTechEvent(Siam5EventData eventData)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(eventData.html);
            var title = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.SelectSingleNode("//a").InnerText); 

            return new TechEvent
            {
                IdSiam = eventData.id_evt,
                ReferenceSiam = eventData.id,
                StartDate = UnixEpoch.AddSeconds(eventData.from),
                Title = title,
                Duration = TimeSpan.FromSeconds(eventData.to - eventData.from),
            };
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
