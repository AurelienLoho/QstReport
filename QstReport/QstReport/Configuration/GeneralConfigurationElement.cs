/**********************************************************************************************/
/**** Fichier : GeneralConfigurationElement.cs                                             ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Configuration
{
    using System.Configuration;

    public class GeneralConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("repertoire", IsRequired = true)]
        public string SaveDirectory
        {
            get { return (string)this["repertoire"]; }
            set { this["repertoire"] = value; }
        }
    }
}
