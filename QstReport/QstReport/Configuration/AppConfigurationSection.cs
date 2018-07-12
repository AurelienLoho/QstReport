/**********************************************************************************************/
/**** Fichier : AppConfigurationSection.cs                                                 ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Configuration
{
    using System.Configuration;

    public class AppConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("siam", IsRequired = true)]
        public RepositoryConnectionConfigurationElement Siam
        {
            get { return (RepositoryConnectionConfigurationElement)this["siam"]; }
            set { this["siam"] = value; }
        }

        [ConfigurationProperty("siamV5", IsRequired = true)]
        public RepositoryConnectionConfigurationElement SiamV5
        {
            get { return (RepositoryConnectionConfigurationElement)this["siamV5"]; }
            set { this["siamV5"] = value; }
        }

        [ConfigurationProperty("epeires", IsRequired=true)]
        public RepositoryConnectionConfigurationElement Epeires
        {
            get { return (RepositoryConnectionConfigurationElement)this["epeires"]; }
            set { this["epeires"] = value; }
        }

        [ConfigurationProperty("modele", IsRequired = true)]
        public ReportModelFileConfigurationElement Model
        {
            get { return (ReportModelFileConfigurationElement)this["modele"]; }
            set { this["modele"] = value; }
        }

        [ConfigurationProperty("enregistrement", IsRequired = true)]
        public GeneralConfigurationElement Save
        {
            get { return (GeneralConfigurationElement)this["enregistrement"]; }
            set { this["enregistrement"] = value; }
        }
    }
}
