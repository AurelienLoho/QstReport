/**********************************************************************************************/
/**** Fichier : Superviseur.cs                                                              ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Les différents superviseurs.
    /// </summary>
    public enum Superviseur
    {
        [Display(Name = "Superviseur ATM", ShortName = "SPV ATM")]
        ATM,

        [Display(Name = "Superviseur ASMGCS", ShortName = "SPV ASMGCS")]
        ASMGCS,

        [Display(Name = "Superviseur CNS", ShortName = "SPV CNS")]
        CNS,
    }
}