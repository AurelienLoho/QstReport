/**********************************************************************************************/
/**** Fichier : ReportModelFileConfigurationElement.cs                                     ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Configuration
{
    using System.Configuration;

    public class ReportModelFileConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("rco", IsRequired=true)]
        public string RCO
        {
            get { return (string)this["rco"]; }
            set { this["rco"] = value; }
        }

        [ConfigurationProperty("gsst", IsRequired = true)]
        public string GSST
        {
            get { return (string)this["gsst"]; }
            set { this["gsst"] = value; }
        }
    }
}
