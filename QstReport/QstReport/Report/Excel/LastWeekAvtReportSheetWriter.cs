/**********************************************************************************************/
/**** Fichier : LastWeekAvtReportSheetWriter.cs                                             ****/
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
    using System.Linq;
    using xL = Microsoft.Office.Interop.Excel;

    public sealed class LastWeekAvtReportSheetWriter
    {
        private static readonly Color TableHeaderBackgroundColor = Color.LightGray;
        private static readonly Color SubdivisionHeaderBackgroundColor = Color.DarkGreen;
        private static readonly Color SectionHeaderbackgroundColor = Color.LightGreen;

        private static readonly string MisoStatusList = "Nominal,Ecart,Auc. Info,Hors ST";
        private static readonly string MisoErrorCodeList = "COOR,ECH,ENV. TECH,MTO,REP,RH,SUR,TECH,TPS";
        
        public void CreateReport(xL.Worksheet sheet, Week week, IEnumerable<Avt> avts)
        {
            var rowIndex = 1; // Index de ligne
            var columnsHeader = new string[] { "Ref SIAM", "Ref AVT", "Libellé", "Date début", "Date fin", "MISO", "Bilan", "Code" };
            var columnCount = columnsHeader.Length;

            var reportTitle = string.Format("Bilan des AVT de la semaine n°{0} (du {1:d} au {2:d})", week.WeekNumber, week.Start, week.End);

            var avtForthisWeek = avts.Where(x => week.Contains(x.WorkPeriods.GlobalStart)).ToList(); // TODO : pas la bonne manière de détecter
            var uniqueAvtCount = avtForthisWeek.UniqueCount(x => x.RefSiam);
            
            /* Titre de la feuille */
            rowIndex += WritePageTitle(sheet, rowIndex, columnCount, reportTitle);

            /* Nb AVT de la semaine */
            rowIndex += WriteNumberOfAvt(sheet, rowIndex, uniqueAvtCount);

            /* En-tête du tableau */
            rowIndex += WriteTableHeader(sheet, rowIndex, columnsHeader);

            /* Affichage par subdivision */
            var groupedBy = avtForthisWeek.GroupBy(x => x.Pole).OrderBy(x => x.Key);
            foreach (var group in groupedBy)
            {
                /* Nom de la subdivision */
                rowIndex += WriteTableSubHeader(sheet, rowIndex, columnCount, SubdivisionHeaderBackgroundColor, group.Key.GetDisplayName());

                /* Affichage de chaque AVT */
                var groupedByEntity = group.GroupBy(x => x.Entite).OrderBy(x => x.Key);

                foreach (var g in groupedByEntity)
                {
                    //if (!string.IsNullOrEmpty(g.Key))
                    //{
                        /* Nom de la section */
                        rowIndex += WriteTableSubHeader(sheet, rowIndex, columnCount, SectionHeaderbackgroundColor, g.Key.GetDisplayName());
                    //}

                    foreach (var avt in g)
                    {
                        sheet.Cells[rowIndex, 1] = avt.RefSiam;
                        sheet.Cells[rowIndex, 2] = avt.Id;
                        sheet.Cells[rowIndex, 3] = avt.Title;
                        sheet.Cells[rowIndex, 4] = "'" + avt.WorkPeriods.GlobalStart.ToShortDateString();
                        sheet.Cells[rowIndex, 5] = "'" + avt.WorkPeriods.GlobalEnd.ToShortDateString();
                        sheet.Cells[rowIndex, 6] = avt.AvtType == AvtType.MISO ? "X" : string.Empty;

                        sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].BorderAround(xL.XlLineStyle.xlContinuous);
                        sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Borders.Item(xL.XlBordersIndex.xlInsideVertical).LineStyle = xL.XlLineStyle.xlContinuous;
                        sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].VerticalAlignment = xL.XlVAlign.xlVAlignCenter;

                        var misoStatusCell = (xL.Range)sheet.Cells[rowIndex, 7];
                        misoStatusCell.Validation.Delete();
                        misoStatusCell.Validation.Add(xL.XlDVType.xlValidateList, xL.XlDVAlertStyle.xlValidAlertInformation, xL.XlFormatConditionOperator.xlBetween, MisoStatusList, Type.Missing);
                        misoStatusCell.Validation.InCellDropdown = true;
                        misoStatusCell.Validation.IgnoreBlank = true;

                        var misoErrorCodeCell = (xL.Range)sheet.Cells[rowIndex, 8];
                        misoErrorCodeCell.Validation.Delete();
                        misoErrorCodeCell.Validation.Add(xL.XlDVType.xlValidateList, xL.XlDVAlertStyle.xlValidAlertInformation, xL.XlFormatConditionOperator.xlBetween, MisoErrorCodeList, Type.Missing);
                        misoErrorCodeCell.Validation.InCellDropdown = true;
                        misoErrorCodeCell.Validation.IgnoreBlank = true;

                        if (avt.IsCancelled)
                        {
                            sheet.Range[sheet.Cells[rowIndex, 1], sheet.Cells[rowIndex, columnCount]].Font.StrikeThrough = true;
                        }
                        
                        rowIndex++;
                    }
                }
            }

            rowIndex++; // insère une ligne vide entre la dernière ligne du tableau et le début du footer

            // pied de page
            rowIndex += WritePageFooter(sheet, rowIndex, columnCount, string.Format("Edité le {0:d} à {0:t}", DateTime.Now));

            /* Mise en page finale */
            
            // pour faciliter l'impression la largeur des colonnes est fixée
            sheet.Columns[1].ColumnWidth = 16;
            sheet.Columns[2].ColumnWidth = 11;
            sheet.Columns[3].WrapText = true;
            sheet.Columns[3].ColumnWidth = 56;
            sheet.Columns[4].ColumnWidth = 10.50;
            sheet.Columns[5].ColumnWidth = 10.50;
            sheet.Columns[6].ColumnWidth = 8;
            sheet.Columns[7].ColumnWidth = 10;
            sheet.Columns[8].ColumnWidth = 13;
            
            sheet.Columns[1].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[2].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[3].HorizontalAlignment = xL.XlHAlign.xlHAlignLeft;
            sheet.Columns[4].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[5].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[6].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[7].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;
            sheet.Columns[8].HorizontalAlignment = xL.XlHAlign.xlHAlignCenter;

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

        private int WriteTableSubHeader(xL.Worksheet sheet, int rowIndex, int columnCount, Color backgroundColor, string text)
        {
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
