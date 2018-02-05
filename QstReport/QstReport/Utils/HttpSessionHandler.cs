/**********************************************************************************************/
/**** Fichier : HttpSessionHandler.cs                                                      ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using System.Net;
    using System.Text;

    public sealed class HttpSessionHandler
    {
        /// <summary>
        /// Le conteneur de cookie.
        /// </summary>
        private readonly CookieContainer _cookieJar = new CookieContainer();

        /// <summary>
        /// Le nom de l'hôte.
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="HttpSessionHandler"/>.
        /// </summary>
        /// <param name="hostName">Le nom de l'hôte.</param>
        public HttpSessionHandler(string hostName)
        {
            _hostName = hostName;
        }

        /// <summary>
        /// Crée une requête GET.
        /// </summary>
        /// <param name="url">L'URL de la requête.</param>
        /// <returns>La requête HTTP.</returns>
        public HttpWebRequest GetRequest(string url)
        {
            return CreateBaseRequest(url, "GET");
        }

        /// <summary>
        /// Crée une requête POST.
        /// </summary>
        /// <param name="url">L'URL de la requête.</param>
        /// <param name="data">Les données de la requête.</param>
        /// <returns>La requête HTTP.</returns>
        public HttpWebRequest PostRequest(string url, string data)
        {
            var request = CreateBaseRequest(url, "POST");

            var dataBytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return request;
        }

        /// <summary>
        /// Envoie une requête HTTP 'GET'.
        /// </summary>
        /// <param name="url">L'URL de la requête.</param>
        /// <returns>La réponse du serveur.</returns>
        public HttpWebResponse SendGetRequest(string url)
        {
            var request = CreateBaseRequest(url, "GET");

            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Envoie une requête HTTP 'POST'.
        /// </summary>
        /// <param name="url">L'URL de la requête.</param>
        /// <param name="data">Les données de la requête.</param>
        /// <returns>La réponse du serveur.</returns>
        public HttpWebResponse SendPostRequest(string url, string data)
        {
            var request = CreateBaseRequest(url, "POST");

            var dataBytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Configuration de base d'une requête HTTP.
        /// </summary>
        /// <param name="url">L'URL de la requête.</param>
        /// <param name="method">La méthode HTTP.</param>
        /// <returns></returns>
        private HttpWebRequest CreateBaseRequest(string url, string method)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://" + url);
            request.Method = method;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Host = _hostName;
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.CookieContainer = _cookieJar;
            request.KeepAlive = false;

            return request;
        }
    }
}

