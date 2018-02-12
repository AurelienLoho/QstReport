/**********************************************************************************************/
/**** Fichier : ExcelReportWriter.cs                                                           ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report
{
    using System;
    using QstReport.DataModel;
    using xL = Microsoft.Office.Interop.Excel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using QstReport.Utils;

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

            var currentWeekAvtSheet = (xL.Worksheet)_xlWorkbook.Worksheets.Add();
            currentWeekAvtSheet.Name = "AVT Semaine courante";
            var currentWeekAvtWriter = new Excel.CurrentWeekAvtReportSheetWriter();
            currentWeekAvtWriter.WriteSheetData(currentWeekAvtSheet, new Week(data.ReportPeriod.End), data.AvtCollection);
            
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

        }

        private void WritePastWeekAvts(ReportData data)
        {

        }

        private void WritePastWeekEvents(ReportData data)
        {

        }
    }
}
