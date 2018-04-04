

namespace QstReport.Report.Excel
{
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using xL = Microsoft.Office.Interop.Excel;

    public sealed class LastWeekTechEventsReportSheetWriter
    {
        private static readonly Color TableHeaderBackgroundColor = Color.LightGray;
        private static readonly Color DayHeaderBackgroundColor = Color.AliceBlue;

        private static readonly string[] ColumnsHeader = { "REX", "Significatif", "Ref SIAM", "Heure", "Durée", "Chaîne", "Description" };

        public void CreateReport(xL.Worksheet sheet, TimePeriod period, IEnumerable<TechEvent> techEvents)
        {
            var rowIndex = 1; // Index de ligne
            var columnCount = ColumnsHeader.Length;

            var reportTitle = string.Format("Bilan des évènements reportés dans SIAM pour la période du {0:d} au {1:d}", period.Start, period.End);


            /* Titre de la feuille */
            rowIndex += WritePageTitle(sheet, rowIndex, reportTitle);

            /* Nb AVT de la semaine */
            rowIndex += WriteNumberOfItems(sheet, rowIndex, techEvents.Count(), techEvents.Where(x => x.RexAsked).Count());

            /* En-tête du tableau */
            rowIndex += WriteTableHeader(sheet, rowIndex);

            /* Affichage par subdivision */
            var groupedBy = techEvents.GroupBy(x => x.StartDate.Date).OrderBy(x => x.Key);
            foreach (var group in groupedBy)
            {
                /* Nom de la subdivision */
                rowIndex += WriteTableSubHeader(sheet, rowIndex, DayHeaderBackgroundColor, group.Key.ToLongDateString());

                foreach (var techEvent in group.OrderBy(x => x.ReferenceSiam))
                {
                    sheet.Cells[rowIndex, 1] = techEvent.RexAsked ? "X" : string.Empty;
                    sheet.Cells[rowIndex, 2] = string.Empty; // TODO : dropdown list pour sélectionner significatif
                    sheet.Cells[rowIndex, 3] = techEvent.ReferenceSiam;
                    sheet.Cells[rowIndex, 4] = "'" + techEvent.StartDate.ToString("hh:mm");
                    sheet.Cells[rowIndex, 5] = techEvent.CalculatedDuration;
                    sheet.Cells[rowIndex, 6] = techEvent.Group;
                    sheet.Cells[rowIndex, 7] = techEvent.Title;

                    sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
                    sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Borders.Item(xL.XlBordersIndex.xlInsideVertical).LineStyle = xL.XlLineStyle.xlContinuous;
                    sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].VerticalAlignment = xL.XlVAlign.xlVAlignCenter;

                    rowIndex++;
                }
            }

            rowIndex++; // insère une ligne vide entre la dernière ligne du tableau et le début du footer

            // pied de page
            rowIndex += WritePageFooter(sheet, rowIndex, string.Format("Edité le {0:d} à {0:t}", DateTime.Now));

            /* Mise en page finale */

            // pour faciliter l'impression la largeur des colonnes est fixée
            sheet.Columns[1].ColumnWidth = 5;
            sheet.Columns[2].ColumnWidth = 11;
            sheet.Columns[3].ColumnWidth = 9;
            sheet.Columns[4].ColumnWidth = 6;
            sheet.Columns[5].ColumnWidth = 9;
            sheet.Columns[6].WrapText = true;
            sheet.Columns[6].ColumnWidth = 30;
            sheet.Columns[7].WrapText = true;
            sheet.Columns[7].ColumnWidth = 90;
            
            sheet.Columns[1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[2].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[3].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[4].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[5].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[6].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[7].HorizontalAlignment = xL.XlHAlign.xlHAlignLeft;

            // Force rows height to cells content
            sheet.Range[sheet.Cells[1, 1], sheet.Cells[rowIndex, 1]].Rows.AutoFit();
        }

        private int WritePageTitle(xL.Worksheet sheet, int rowIndex, string title)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, ColumnsHeader.Length]].Merge();
            sheet.Cells[rowIndex, 1].Font.Size = 14;
            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = title;

            return 2; // 1 ligne écrite + 1 ligne vide
        }

        private int WritePageFooter(xL.Worksheet sheet, int rowIndex, string text)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, ColumnsHeader.Length]].Merge();
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = text;

            return 1;
        }

        private int WriteNumberOfItems(xL.Worksheet sheet, int rowIndex, long numberOfAvt, int numberOfRex)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, ColumnsHeader.Length]].Merge();

            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].HorizontalALignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Cells[rowIndex, 1] = string.Format("Nombre d'évènements : {0}    dont {1} demande(s) de REX", numberOfAvt, numberOfRex);

            return 2; // 1 ligne écrite + 1 ligne vide
        }

        private int WriteTableHeader(xL.Worksheet sheet, int rowIndex)
        {
            var columnCount = ColumnsHeader.Length;

            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Font.Bold = true;
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Interior.Color = ColorTranslator.ToOle(TableHeaderBackgroundColor);
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Borders.Item(xL.XlBordersIndex.xlInsideVertical).LineStyle = xL.XlLineStyle.xlContinuous;

            for (int i = 0; i < columnCount; i++)
            {
                sheet.Cells[rowIndex, i + 1] = ColumnsHeader[i];
            }

            return 1; // 1 ligne écrite
        }

        private int WriteTableSubHeader(xL.Worksheet sheet, int rowIndex, Color backgroundColor, string text)
        {
            var columnCount = ColumnsHeader.Length;

            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Merge();
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].Interior.Color = ColorTranslator.ToOle(backgroundColor);
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = text;

            return 1; // 1 ligne écrite
        }
    }
}
