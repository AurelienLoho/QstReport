/**********************************************************************************************/
/**** Fichier : IReportWriter.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report
{
    using QstReport.DataModel;

    /// <summary>
    /// Interface pour un objet permettant de créer un rapport à partir des données collectées.
    /// </summary>
    public interface IReportWriter
    {
        /// <summary>
        /// Crée le rapport.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        /// <param name="fileName">Le nom de fichier du rapport.</param>
        void WriteReport(ReportData data, string reportFileName, string modelFileName);
    }
}
