/**********************************************************************************************/
/**** Fichier : RepositoryConnectionConfigurationElement.cs                                ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Configuration
{
    using System.Configuration;

    public class RepositoryConnectionConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("hostname", IsRequired = true)]
        public string HostName
        {
            get { return (string)this["hostname"]; }
            set { this["hostname"] = value; }
        }

        [ConfigurationProperty("user", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["user"]; }
            set { this["user"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }
    }
}
