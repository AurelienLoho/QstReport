/**********************************************************************************************/
/**** Fichier : ExcelReportWriter.cs                                                       ****/
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

            // Le dernier onglet ajouté sera le premier à l'ouverture du fichier

            WritePastWeekExploitEvents(data);
            WritePastWeekTechEvents(data);
            WritePastWeekAvts(data);
            WriteCurrentWeekAvts(data);
            
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
            var workSheet = CreateNewWorkSheet("AVT Semaine courante", xL.XlPageOrientation.xlPortrait);

            var currentWeekAvtWriter = new Excel.CurrentWeekAvtReportSheetWriter();
            currentWeekAvtWriter.WriteSheetData(workSheet, new Week(data.ReportPeriod.End), data.AvtCollection);
        }

        private void WritePastWeekAvts(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("AVT Semaine passée", xL.XlPageOrientation.xlPortrait);

            var pastWeekAvtWriter = new Excel.LastWeekAvtReportSheetWriter();
            pastWeekAvtWriter.CreateReport(workSheet, new Week(data.ReportPeriod.Start), data.AvtCollection);
        }

        private void WritePastWeekTechEvents(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("Evènements SIAM", xL.XlPageOrientation.xlLandscape);

            var pastWeekTechEventWriter = new Excel.LastWeekTechEventsReportSheetWriter();
            pastWeekTechEventWriter.CreateReport(workSheet, new Week(data.ReportPeriod.Start), data.TechEventCollection);
        }

        private void WritePastWeekExploitEvents(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("Evènements Epeires", xL.XlPageOrientation.xlPortrait);

            var pastWeekExploitEventWriter = new Excel.LastWeekExploitEventReportSheetWriter();
            pastWeekExploitEventWriter.CreateReport(workSheet, new Week(data.ReportPeriod.Start), data.ExploitEventCollection);
        }

        private xL.Worksheet CreateNewWorkSheet(string sheetName, xL.XlPageOrientation pageOrientation)
        {
            var workSheet = (xL.Worksheet)_xlWorkbook.Worksheets.Add();
            workSheet.Name = sheetName;
            workSheet.PageSetup.Orientation = pageOrientation;
            workSheet.PageSetup.Zoom = false;
            workSheet.PageSetup.FitToPagesWide = 1;
            workSheet.PageSetup.FitToPagesTall = false;

            workSheet.PageSetup.CenterHorizontally = true;

            var leftRightMargin = workSheet.PageSetup.Application.CentimetersToPoints(0.64);
            var topBottomMargin = workSheet.PageSetup.Application.CentimetersToPoints(1.91);
            var headerFooterSize = workSheet.PageSetup.Application.CentimetersToPoints(0.76);

            workSheet.PageSetup.LeftMargin = leftRightMargin;
            workSheet.PageSetup.RightMargin = leftRightMargin;
            workSheet.PageSetup.TopMargin = topBottomMargin;
            workSheet.PageSetup.BottomMargin = topBottomMargin;
            workSheet.PageSetup.HeaderMargin = headerFooterSize;
            workSheet.PageSetup.FooterMargin = headerFooterSize;

            return workSheet;
        }
    }
}
