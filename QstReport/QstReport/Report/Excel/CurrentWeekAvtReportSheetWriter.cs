/**********************************************************************************************/
/**** Fichier : CurrentWeekAvtReportSheetWriter.cs                                         ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Report.Excel
{
    using QstReport.DataModel;
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using xL = Microsoft.Office.Interop.Excel;

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

        public void WriteSheetData(xL.Worksheet sheet, TimePeriod period, IEnumerable<Avt> avts)
        {
            var rowIndex = 1; // Index de ligne
            var columnHeaders = new string[] { "Ref SIAM", "Ref AVT", "Libellé", "Heure début", "Heure fin", "LBG" };
            var columnCount = columnHeaders.Length;
            var sheetTitle = string.Format("Programme des AVT pour la période du {0:d} au {1:d}", period.Start, period.End);

            var expandedAvts = avts.SelectMany(x =>
                                                    {
                                                        return x.WorkPeriods.SelectMany(t => ExplodeTimePeriod(t))
                                                                            .Select(y => new { Period = y, Avt = x}); 
                                                    })
                                   .Where(z => period.ContainsDate(z.Period.Starts))
                                   .ToList();

            var uniqueAvtCount = expandedAvts.Select(x => x.Avt).UniqueCount(y => y.RefSiam);

            /* Titre de la feuille */
            rowIndex += WritePageTitle(sheet, rowIndex, columnCount, sheetTitle);

            /* Nb AVT de la semaine */
            //rowIndex += WriteNumberOfAvt(sheet, rowIndex, uniqueAvtCount);
            rowIndex += WriteAvtDetailedCount(sheet, rowIndex, columnCount, period, avts);

            /* En-tête du tableau */
            rowIndex += WriteTableHeader(sheet, rowIndex, columnHeaders);

            var groupedByDate = expandedAvts.GroupBy(x => x.Period.Starts.Date);

            var alreadySeenAvt = new HashSet<string>();

            foreach (var group in groupedByDate)
            {
                /* Date du jour */
                rowIndex += WriteTableSubHeader(sheet, rowIndex, columnCount, group.Key.ToString("dddd d MMMM"));

                var dayAvts = group.Where(x => x.Period.Starts.TimeOfDay < nightTime).OrderBy(x => x.Avt.Pole).ThenBy(x => x.Avt.Entite);
                var nightAvts = group.Where(x => x.Period.Starts.TimeOfDay >= nightTime).OrderBy(x => x.Avt.Pole).ThenBy(x => x.Avt.Entite);

                // AVT de la journée
                foreach (var work in dayAvts)
                {
                    bool firstOccurence = alreadySeenAvt.Add(work.Avt.RefSiam);

                    var data = FormatAvtData(work.Period, work.Avt);

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

                        var data = FormatAvtData(work.Period, work.Avt);

                        rowIndex += WriteAvtData(sheet, rowIndex, firstOccurence, work.Avt.IsCancelled, data);
                    }
                }
            }

            rowIndex++; // Ajout d'une ligne entre le dernier AVT du tableau et le pied-de-page

            // pied de page
            rowIndex += WritePageFooter(sheet, rowIndex, columnCount, string.Format("Edité le {0:d} à {0:t}", DateTime.Now));

            /* Mise en page finale */

            sheet.Columns[1].ColumnWidth = 16.5;
            sheet.Columns[2].ColumnWidth = 11.5;
            sheet.Columns[3].WrapText = true;
            sheet.Columns[3].ColumnWidth = 86;
            sheet.Columns[4].ColumnWidth = 8.5;
            sheet.Columns[5].ColumnWidth = 8.5;
            sheet.Columns[6].ColumnWidth = 4;

            sheet.Columns[1].HorizontalAlignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Columns[2].HorizontalAlignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Columns[3].HorizontalAlignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Columns[4].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[5].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[6].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;

            // Force rows height to cells content
            sheet.Range[sheet.Cells[1, 1], sheet.Cells[rowIndex, 1]].Rows.AutoFit();
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

        //private int WriteNumberOfAvt(xL.Worksheet sheet, int rowIndex, long numberOfAvt)
        //{
        //    sheet.Cells[rowIndex, 1].Font.Bold = true;
        //    sheet.Cells[rowIndex, 1].HorizontalALignment = xL.XlHAlign.xlHAlignRight;
        //    sheet.Cells[rowIndex, 1] = "Nb AVT : ";
        //    sheet.Cells[rowIndex, 2].Font.Bold = true;
        //    sheet.Cells[rowIndex, 2] = numberOfAvt;

        //    return 2; // 1 ligne écrite + 1 ligne vide
        //}

        private int WriteAvtDetailedCount(xL.Worksheet sheet, int rowIndex, int numOfCOlumns, TimePeriod week, IEnumerable<Avt> avts)
        {
            var uniqueAvts = avts.Where(y => y.WorkPeriods.Any(t => week.ContainsDate(t.Start))).DistinctBy(x => x.RefSiam).ToList();
            var totalAvt = uniqueAvts.Count();
            var atmCount = uniqueAvts.Count(x => x.Pole == Pole.ATM);
            var cnsCount = uniqueAvts.Count(x => x.Pole == Pole.CNS);

            var resCount = uniqueAvts.Count(x => x.Entite == Entite.Reseaux);
            var tpvCount = uniqueAvts.Count(x => x.Entite == Entite.TraitementPlanDeVol);
            var trCount = uniqueAvts.Count(x => x.Entite == Entite.TraitementRadar);
            var simuCount = uniqueAvts.Count(x => x.Entite == Entite.SimulationSupervision);
            var asmgcsCount = uniqueAvts.Count(x => x.Pole == Pole.ATM && x.Entite == Entite.Radars);

            var rnavCount = uniqueAvts.Count(x => x.Entite == Entite.Radionavigation);
            var nrjCount = uniqueAvts.Count(x => x.Entite == Entite.EnergieClimatisation);
            var rteCount = uniqueAvts.Count(x => x.Entite == Entite.RadioTelephone);
            var radCount = uniqueAvts.Count(x => x.Pole == Pole.CNS && x.Entite == Entite.Radars);

            sheet.Range[sheet.Cells[rowIndex, 3], sheet.Cells[rowIndex, numOfCOlumns]].Merge();
            sheet.Range[sheet.Cells[rowIndex + 1, 3], sheet.Cells[rowIndex + 1, numOfCOlumns]].Merge();

            sheet.Cells[rowIndex, 1].Font.Bold = true;
            sheet.Cells[rowIndex, 1].HorizontalALignment = xL.XlHAlign.xlHAlignRight;
            sheet.Cells[rowIndex, 1] = "Nb AVT : ";

            sheet.Cells[rowIndex, 2].Font.Bold = true;
            sheet.Cells[rowIndex, 2].HorizontalALignment = xL.XlHAlign.xlHAlignRight;
            sheet.Cells[rowIndex, 2] = totalAvt;

            sheet.Cells[rowIndex, 3].Font.Bold = true;
            sheet.Cells[rowIndex, 3].HorizontalALignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Cells[rowIndex, 3] = string.Format("dont Pôle ATM : {0} ( ASMGCS : {1} ; RES : {2} ; SIMU : {3} ; TPV : {4} ; TR : {5} )", atmCount, asmgcsCount, resCount, simuCount, tpvCount, trCount);

            sheet.Cells[rowIndex + 1, 3].Font.Bold = true;
            sheet.Cells[rowIndex + 1, 3].HorizontalALignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Cells[rowIndex + 1, 3] = string.Format("           Pôle CNS : {0} ( NRJ : {1} ; RAD : {2} ; RNAV : {3} ; RTE : {4} )", cnsCount, nrjCount, radCount, rnavCount, rteCount);


            return 3; // 1 ligne écrite + 1 ligne vide
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
        
        private string[] FormatAvtData(WorkPeriod period, Avt avt)
        {
            return new string[] {
                            avt.RefSiam,
                            avt.Id,
                            avt.Title,
                            "'" + (period.StartsBefore ? "<<<" : period.Starts.ToShortTimeString()),
                            "'" + (period.EndsAfter ? ">>>" : period.Ends.ToShortTimeString()),
                            avt.ImpactedAirports.Contains("LFPB") ? "X" : string.Empty
                        };
        }
        
        private IEnumerable<WorkPeriod> ExplodeTimePeriod(TimePeriod period)
        {
            if (period.Start.Date == period.End.Date)
            {
                return new List<WorkPeriod> { new WorkPeriod { Starts = period.Start, Ends = period.End, StartsBefore = false, EndsAfter = false } };
            }

            // TODO : exclure les périodes d'AVT de nuit
            if (period.Start.TimeOfDay > nightTime && period.Duration < TimeSpan.FromDays(1))
            {
                return new List<WorkPeriod> { new WorkPeriod { Starts = period.Start, Ends = period.End, StartsBefore = false, EndsAfter = false } };
            }

            var days = period.Start.EachDayTo(period.End).ToArray();

            var list = new List<WorkPeriod>();

            for (int i = 0; i < days.Length; i++)
            {
                var d = days[i];

                if (i == 0)
                {
                    list.Add(new WorkPeriod { Starts = period.Start, Ends = d.EndOfDay(), StartsBefore = false, EndsAfter = true });
                }
                else if (i == (days.Length - 1))
                {
                    list.Add(new WorkPeriod { Starts =d, Ends = period.End, StartsBefore = true, EndsAfter = false });
                }
                else
                {
                    list.Add(new WorkPeriod { Starts = d, Ends = d.EndOfDay(), StartsBefore = true, EndsAfter = true });
                }
            }

            return list;

        }
    }


    public class WorkPeriod
    {
        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }

        public bool StartsBefore { get; set; }

        public bool EndsAfter { get; set; }
    }
}
