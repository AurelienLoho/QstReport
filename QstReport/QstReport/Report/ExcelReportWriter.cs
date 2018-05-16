/**********************************************************************************************/
/**** Fichier : ExcelReportWriter.cs                                                       ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report
{
    using QstReport.DataModel;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using xL = Microsoft.Office.Interop.Excel;

    public sealed class ExcelReportWriter : IReportWriter
    {
        private static readonly Color DefaultTabColor = Color.LightGray;

        /// <summary>
        /// L'application excel.
        /// </summary>
        private xL.Application _xlApp;

        /// <summary>
        /// Le classeur excel.
        /// </summary>
        private xL.Workbook _xlWorkbook;
        
        /// <summary>
        /// Crée le rapport au format excel.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        /// <param name="fileName">Le nom de fichier du rapport.</param>
        public void WriteReport(ReportData data, string reportFileName, string reportModelName = null)
        {
            _xlApp = new xL.Application();

            if (string.IsNullOrEmpty(reportModelName))
            {
                _xlWorkbook = _xlApp.Workbooks.Add(Missing.Value);
            }
            else
            {
                _xlWorkbook = _xlApp.Workbooks.Open(reportModelName);
            }

            // Le dernier onglet ajouté sera le premier à l'ouverture du fichier
            WritePastWeekExploitEvents(data);
            WritePastWeekTechEvents(data);
            WritePastWeekAvts(data);
            WriteCurrentWeekAvts(data);

            try
            {
                _xlApp.DisplayAlerts = false;
                _xlWorkbook.SaveAs(reportFileName, xL.XlFileFormat.xlWorkbookNormal, Missing.Value, Missing.Value);                
            }
            catch
            { /* silent fail */ }
            finally
            {
                _xlWorkbook.Close(true, Missing.Value, Missing.Value);
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlWorkbook);
                Marshal.ReleaseComObject(_xlApp);
            }
        }

        /// <summary>
        /// Ajoute le planning des AVTs pour la période à venir.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        private void WriteCurrentWeekAvts(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("AVT Semaine courante", xL.XlPageOrientation.xlPortrait);

            var currentWeekAvtWriter = new Excel.CurrentWeekAvtReportSheetWriter();
            currentWeekAvtWriter.WriteSheetData(workSheet, data.CurrentDataPeriod, data.AvtCollection);
        }

        /// <summary>
        /// Ajoute le bilan des AVTs pour la période passée.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        private void WritePastWeekAvts(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("AVT Semaine passée", xL.XlPageOrientation.xlPortrait);

            var pastWeekAvtWriter = new Excel.LastWeekAvtReportSheetWriter();
            pastWeekAvtWriter.CreateReport(workSheet, data.PastDataPeriod, data.AvtCollection);
        }

        /// <summary>
        /// Ajoute le bilan des évènements techniques pour la période passée.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        private void WritePastWeekTechEvents(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("Evènements SIAM", xL.XlPageOrientation.xlLandscape);

            var pastWeekTechEventWriter = new Excel.LastWeekTechEventsReportSheetWriter();
            pastWeekTechEventWriter.CreateReport(workSheet, data.PastDataPeriod, data.TechEventCollection);
        }

        /// <summary>
        /// Ajoute le bilan des évènements exploitation pour la période passée.
        /// </summary>
        /// <param name="data">Les données du rapport.</param>
        private void WritePastWeekExploitEvents(ReportData data)
        {
            var workSheet = CreateNewWorkSheet("Evènements Epeires", xL.XlPageOrientation.xlPortrait);

            var pastWeekExploitEventWriter = new Excel.LastWeekExploitEventReportSheetWriter();
            pastWeekExploitEventWriter.CreateReport(workSheet, data.PastDataPeriod, data.ExploitEventCollection);
        }

        /// <summary>
        /// Ajoute et configue une nouvelle feuille au fichier excel.
        /// </summary>
        /// <param name="sheetName">Le nom de la nouvelle feuille.</param>
        /// <param name="pageOrientation">L'orientation de la feuille pour l'impression.</param>
        /// <returns>Une nouvelle feuille configurée pour l'impression.</returns>
        private xL.Worksheet CreateNewWorkSheet(string sheetName, xL.XlPageOrientation pageOrientation)
        {
            var firstSheet = _xlWorkbook.Worksheets[1];
            var workSheet = (xL.Worksheet)_xlWorkbook.Worksheets.Add(Missing.Value, firstSheet);
            workSheet.Name = sheetName;
            workSheet.Tab.Color = DefaultTabColor.ToArgb(); 

            workSheet.PageSetup.Orientation = pageOrientation;
            
            // configuration de la mise en page (ajuste toutes les colonnes à une seule page)
            workSheet.PageSetup.Zoom = false;
            workSheet.PageSetup.FitToPagesWide = 1;
            workSheet.PageSetup.FitToPagesTall = false;
            workSheet.PageSetup.CenterHorizontally = true;

            // configuration des marges d'impression (valeurs équivalentes aux marges étroites d'excel)
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
