/**********************************************************************************************/
/**** Fichier : Pole.cs                                                                    ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Les pôles constituants le service technique.
    /// </summary>
    public enum Pole
    {
        [Display(Name = "Pôle ATM", ShortName = "ATM")]
        ATM,

        [Display(Name = "Pôle CNS", ShortName = "CNS")]
        CNS,

        [Display(Name = "Pôle Transverse", ShortName = "TSV")]
        TSV,

        [Display(Name = "Autres", ShortName = "Autres")]
        Unknown,
    }
}
