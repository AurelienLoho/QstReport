/**********************************************************************************************/
/**** Fichier : Entite.cs                                                                  ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.DataModel
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Les entités constituants le service technique.
    /// </summary>
    public enum Entite
    {
        [Display(Name = "Traitement Radar", ShortName = "TR")]
        TraitementRadar,

        [Display(Name = "Traitement Plan de vol", ShortName = "TPV")]
        TraitementPlanDeVol,

        [Display(Name = "Simulation / Supervision", ShortName = "SIMU")]
        SimulationSupervision,

        [Display(Name = "Energie / Climatisation", ShortName = "NRJ")]
        EnergieClimatisation,

        [Display(Name = "Radio / Téléphone", ShortName = "RADIO")]
        RadioTelephone,

        [Display(Name = "Radionavigation", ShortName = "RNAV")]
        Radionavigation,

        [Display(Name = "Installations", ShortName = "Installs")]
        Installations,

        [Display(Name = "Réseaux", ShortName = "RSX")]
        Reseaux,

        [Display(Name = "Radars", ShortName = "RADAR")]
        Radars,

        [Display(Name = "Qualité de service technique", ShortName = "QST")]
        QualiteDeService,

        [Display(Name = "Instruction", ShortName = "INS")]
        Instruction,

        [Display(Name = "Bureautique", ShortName = "CANARI")]
        Bureautique,

        [Display(Name = "Moyens généraux", ShortName = "MG")]
        MoyensGeneraux,
    }
}

