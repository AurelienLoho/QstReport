/**********************************************************************************************/
/**** Fichier : ExcelReportWriter.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report
{
    using QstReport.DataModel;
    using QstReport.Utils;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using xL = Microsoft.Office.Interop.Excel;

    public sealed class ExcelReportWriter : IReportWriter
    {
        /// <summary>
        /// L'application excel.
        /// </summary>
        private xL.Application _xlApp;

        /// <summary>
        /// Le classeur excel.
        /// </summary>
        private xL.Workbook _xlWorkbook;
        
        public void WriteReport(ReportData data, string fileName)
        {
            _xlApp = new xL.Application();
            _xlWorkbook = _xlApp.Workbooks.Add(Missing.Value);

            WriteCurrentWeekAvts(data);

            WritePastWeekAvts(data);
            
            try
            {
                _xlWorkbook.SaveAs(fileName, xL.XlFileFormat.xlOpenXMLWorkbook, Missing.Value, Missing.Value);
                _xlWorkbook.Close(true, Missing.Value, Missing.Value);
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlWorkbook);
                Marshal.ReleaseComObject(_xlApp);
            }
            catch
            { /* silent fail */ }
        }

        private void WriteCurrentWeekAvts(ReportData data)
        {
            var currentWeekAvtSheet = (xL.Worksheet)_xlWorkbook.Worksheets.Add();
            currentWeekAvtSheet.Name = "AVT Semaine courante";
            var currentWeekAvtWriter = new Excel.ByDateAvtReportSheetWriter();
            currentWeekAvtWriter.WriteSheetData(currentWeekAvtSheet, new Week(data.ReportPeriod.End), data.AvtCollection);
        }

        private void WritePastWeekAvts(ReportData data)
        {
            var pastWeekAvtSheet = (xL.Worksheet)_xlWorkbook.Worksheets.Add();
            pastWeekAvtSheet.Name = "AVT Semaine passée";
            var pastWeekAvtWriter = new Excel.ByOwnerAvtReportSheetWriter();
            pastWeekAvtWriter.CreateReport(pastWeekAvtSheet, new Week(data.ReportPeriod.Start), data.AvtCollection);
        }

        private void WritePastWeekEvents(ReportData data)
        {

        }
    }
}
