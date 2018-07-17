/**********************************************************************************************/
/**** Fichier : Siam5/SiamHttpSessionHandler.cs                                            ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Siam5
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public sealed class SiamHttpSessionHandler
    {
        /// <summary>
        /// Le conteneur de cookie.
        /// </summary>
        private readonly CookieContainer _cookieJar = new CookieContainer();

        /// <summary>
        /// Le nom de l'hôte.
        /// </summary>
        private readonly string _hostName;

        private readonly bool _useHttps = true;

        private const string formDataDelimeter = "-----------------------------195142331314649";

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="HttpSessionHandler"/>.
        /// </summary>
        /// <param name="hostName">Le nom de l'hôte.</param>
        public SiamHttpSessionHandler(string hostName, bool useHttps = true)
        {
            _hostName = hostName;
            _useHttps = useHttps;
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Host = _hostName;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.AllowAutoRedirect = false;
            request.CookieContainer = _cookieJar;
            request.KeepAlive = true;
            request.Referer = "https://siam5.tech.cana.ri/actuel/appli/planner/";// +url;
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            return request;
        }

        public HttpWebResponse SendConnectRequest(string url, Dictionary<string, string> requestParams)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://" + url);
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Host = _hostName;
            request.ContentType = "multipart/form-data; boundary=---------------------------195142331314649";
            request.AllowAutoRedirect = false;
            request.CookieContainer = _cookieJar;
            request.KeepAlive = true;
            request.Referer = "https://siam5.tech.cana.ri/actuel/";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var requestData = FormatRequestData(requestParams);

            var dataBytes = Encoding.ASCII.GetBytes(requestData);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        public HttpWebResponse SendDisconnectRequest(string url, Dictionary<string, string> requestParams)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://" + url);
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Host = _hostName;
            request.ContentType = "multipart/form-data; boundary=---------------------------195142331314649";
            request.AllowAutoRedirect = false;
            request.CookieContainer = _cookieJar;
            request.KeepAlive = true;
            request.Referer = "https://siam5.tech.cana.ri/actuel/lib/core/auth/?action=logout";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var requestData = FormatRequestData(requestParams);

            var dataBytes = Encoding.ASCII.GetBytes(requestData);
            request.ContentLength = dataBytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        public HttpWebResponse SendDetailAvtRequest(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://" + url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Host = _hostName;
            request.AllowAutoRedirect = false;
            request.CookieContainer = _cookieJar;
            request.KeepAlive = true;

            return (HttpWebResponse)request.GetResponse();
        }

        private string FormatRequestData(Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();

            const string delimeter = formDataDelimeter + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";

            foreach (var entry in parameters)
            {
                sb.AppendFormat(delimeter, entry.Key, entry.Value);
            }

            sb.Append(formDataDelimeter + "--");

            return sb.ToString();
        }
    }
}
