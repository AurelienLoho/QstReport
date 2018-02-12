using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xL = Microsoft.Office.Interop.Excel;
using QstReport.DataModel;
using System.Drawing;
using QstReport.Utils;
using System.Globalization;

namespace QstReport.Report.Excel
{
    public sealed class CurrentWeekAvtReportSheetWriter
    {
        private static readonly Color TableHeaderBackgroundColor = Color.LightGray;
        private static readonly Color TableDateHeaderBackgroundColor = Color.Chocolate;
        private static readonly Color AvtHighlightBackgroundColor = Color.Wheat;
        private static readonly Color AvtDimTextForegroundColor = Color.Gray;

        /// <summary>
        /// L'heure de début de nuit (en UTC).
        /// </summary>
        private static readonly TimeSpan nightTime = new TimeSpan(19, 0, 0);

        public void WriteSheetData(xL.Worksheet sheet, Week week, IEnumerable<Avt> avts)
        {
            var rowIndex = 1; // Index de ligne
            var columnHeaders = new string[] { "Ref SIAM", "Ref AVT", "Libellé", "Heure début", "Heure fin", "LBG" };
            var columnCount = columnHeaders.Length;
            var sheetTitle = string.Format("Programme des AVT - Semaine n°{0} (du {1} au {2})", week.WeekNumber, week.Start, week.End);

            var expandedAvts = avts.SelectMany(x => x.WorkPeriods.Select(y => new { Period = y, Avt = x  }))
                                   .Where(t => week.Contains(t.Period.Start))
                                   .ToList();

            var uniqueAvtCount = expandedAvts.Select(x => x.Avt).UniqueCount(y => y.RefSiam);

            /* Titre de la feuille */
            rowIndex += WritePageTitle(sheet, rowIndex, columnCount, sheetTitle);

            /* Nb AVT de la semaine */
            rowIndex += WriteNumberOfAvt(sheet, rowIndex, uniqueAvtCount);

            /* En-tête du tableau */
            rowIndex += WriteTableHeader(sheet, rowIndex, columnHeaders);

            var groupedByDate = expandedAvts.GroupBy(x => x.Period.Start);

            var alreadySeenAvt = new HashSet<string>();

            // TODO : ....


            foreach (var group in groupedByDate)
            {
                /* Date du jour */
                rowIndex += WriteTableSubHeader(sheet, rowIndex, columnCount, group.Key.ToString("dddd d MMMM"));

                var dayAvts = group.Where(x => x.Period.Start.TimeOfDay < nightTime).OrderBy(x => x.Avt.Pole).ThenBy(x => x.Avt.Entite);
                var nightAvts = group.Where(x => x.Period.Start.TimeOfDay >= nightTime).OrderBy(x => x.Avt.Pole).ThenBy(x => x.Avt.Entite);

                // AVT de la journée
                foreach (var work in dayAvts)
                {
                    bool firstOccurence = alreadySeenAvt.Add(work.Avt.RefSiam);

                    var data = FormatAvtData(group.Key.Date, work.Period, work.Avt);

                    rowIndex += WriteAvtData(sheet, rowIndex, firstOccurence, work.Avt.IsCancelled, data);
                }

                // AVT de la nuit (si il y en a)
                if (nightAvts != null && nightAvts.Count() > 0)
                {
                    /* Date du jour */
                    rowIndex += this.WriteTableSubHeader(sheet, rowIndex, columnCount, string.Format("Nuit de {0} à {1}", CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(group.Key.DayOfWeek), CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(group.Key.AddDays(1).DayOfWeek)));

                    foreach (var work in nightAvts)
                    {
                        bool firstOccurence = alreadySeenAvt.Add(work.Avt.RefSiam);

                        var data = FormatAvtData(group.Key.Date, work.Period, work.Avt);

                        rowIndex += WriteAvtData(sheet, rowIndex, firstOccurence, work.Avt.IsCancelled, data);
                    }
                }
            }

            // pied de page
            rowIndex += WritePageFooter(sheet, rowIndex, columnCount, string.Format("Edité le {0:d} à {0:t}", DateTime.Now));

            /* Mise en page finale */
            sheet.Columns.AutoFit();
            sheet.Columns[4].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[5].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[6].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;

            throw new NotImplementedException();
        }

        private int WritePageTitle(xL.Worksheet sheet, int rowIndex, int columnCount, string title)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Merge();
            sheet.Cells[rowIndex, 1].Font.Size = 14;
            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = title;

            return 2; // 1 ligne écrite + 1 ligne vide
        }

        private int WritePageFooter(xL.Worksheet sheet, int rowIndex, int columnCount, string text)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Merge();
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = text;

            return 1;
        }

        private int WriteNumberOfAvt(xL.Worksheet sheet, int rowIndex, long numberOfAvt)
        {
            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].HorizontalALignment = xL.XlHAlign.xlHAlignRight;
            sheet.Cells[rowIndex, 1] = "Nb AVT : ";
            sheet.Cells[rowIndex, 2].Font.Bold = true;
            sheet.Cells[rowIndex, 2] = numberOfAvt;

            return 2; // 1 ligne écrite + 1 ligne vide
        }

        private int WriteTableHeader(xL.Worksheet sheet, int rowIndex, string[] columnHeaders)
        {
            var columnCount = columnHeaders.Length;

            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Font.Bold = true;
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Interior.Color = ColorTranslator.ToOle(TableHeaderBackgroundColor);
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Borders.Item(xL.XlBordersIndex.xlInsideVertical).LineStyle = xL.XlLineStyle.xlContinuous;

            for (int i = 0; i < columnCount; i++)
            {
                sheet.Cells[rowIndex, i + 1] = columnHeaders[i];
            }

            return 1; // 1 ligne écrite
        }

        private int WriteTableSubHeader(xL.Worksheet sheet, int rowIndex, int columnCount, string text)
        {
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Merge();
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].Interior.Color = ColorTranslator.ToOle(TableDateHeaderBackgroundColor);
            sheet.Cells[rowIndex, 1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Cells[rowIndex, 1] = text;

            return 1; // 1 ligne écrite
        }

        private int WriteAvtData(xL.Worksheet sheet, int rowIndex, bool firstOccurence, bool isCancelled, string[] dataToWrite)
        {
            var columnCount = dataToWrite.Length;

            if (firstOccurence)
            {
                sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Interior.Color = ColorTranslator.ToOle(AvtHighlightBackgroundColor);
            }
            else
            {
                sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Font.Color = ColorTranslator.ToOle(AvtDimTextForegroundColor);
            }

            for (int i = 0; i < columnCount; i++)
            {
                sheet.Cells[rowIndex, i + 1] = dataToWrite[i];
            }

            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Borders.Item(xL.XlBordersIndex.xlInsideVertical).LineStyle = xL.XlLineStyle.xlContinuous;

            if (isCancelled)
            {
                sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Font.StrikeThrough = true;
            }

            return 1; // 1 ligne écrite
        }
      
        private string[] FormatAvtData(DateTime date, TimePeriod period, Avt avt)
        {
            return new string[] {
                            avt.RefSiam,
                            avt.Id,
                            avt.Title,
                            "'" + (period.Start !=  date ? "---" : period.Start.ToShortTimeString()),
                            "'" + ((period.End - date).Days > 1 ? "---" : period.End.ToShortTimeString()),
                            avt.ImpactedAirports.Contains("LBG") ? "X" : string.Empty
                        };
        }
    }
}
