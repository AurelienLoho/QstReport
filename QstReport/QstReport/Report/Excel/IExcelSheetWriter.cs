using QstReport.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xL = Microsoft.Office.Interop.Excel;

namespace QstReport.Report.Excel
{
    public interface IExcelSheetWriter
    {
        void WriteSheetData(xL.Worksheet sheet, string sheetName, ReportData data);
    }
}
