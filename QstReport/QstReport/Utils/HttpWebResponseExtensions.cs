/**********************************************************************************************/
/**** Fichier : HttpExtensionMethods.cs                                                    ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Fonctions utilitaires pour gérer les réponses HTTP.
    /// </summary>
    public static class HttpExtensionMethods
    {
        /// <summary>
        /// Transforme une réponse HTTP en document JSON.
        /// </summary>
        /// <typeparam name="T">Le type de données contenues dans le document JSON.</typeparam>
        /// <param name="response">Une réponse HTTP.</param>
        /// <returns>La réponse au format JSON.</returns>
        public static T AsJson<T>(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var s = reader.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(s);

                    return data;
                }
            }
        }

        /// <summary>
        /// Transforme une réponse HTTP en document HTML.
        /// </summary>
        /// <param name="response">Une réponse HTTP.</param>
        /// <returns>Un document HTML.</returns>
        public static HtmlNode AsHtml(this HttpWebResponse response)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(response.GetResponseStream());

            return htmlDoc.DocumentNode;
        }

        /// <summary>
        /// Ne fait rien avec la réponse HTTP.
        /// </summary>
        /// <param name="response">Une réponse HTTP.</param>
        public static void Discard(this HttpWebResponse response)
        {
            using (response)
            { }
        }
    }
}
