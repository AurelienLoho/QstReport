/**********************************************************************************************/
/**** Fichier : AvtType.cs                                                                 ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Les différents types d'avis de travaux.
    /// </summary>
    public enum AvtType
    {
        /// <summary>
        /// Avis simple.
        /// </summary>
        [Display(Name = "Avis de travaux", ShortName = "AVT")]
        AVT,

        /// <summary>
        /// Intervention sur équipement opérationnel.
        /// </summary>
        [Display(Name = "Méthodologie d'interventions sur système opérationnel", ShortName = "MISO")]
        MISO,

        /// <summary>
        /// Conduite de système opérationnel.
        /// </summary>
        [Display(Name = "Conduite de système opérationnel", ShortName = "MICSO")]
        MICSO,
    }
}
