/**********************************************************************************************/
/**** Fichier : IReportWriter.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report
{
    using QstReport.DataModel;

    public interface IReportWriter
    {
        void WriteReport(ReportData data, string fileName);
    }
}
