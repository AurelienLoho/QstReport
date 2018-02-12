

namespace QstReport.DataModel
{
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class ReportData
    {
        public TimePeriod ReportPeriod { get; set; }

        public List<Avt> AvtCollection { get; set; } = new List<Avt>();
    }
}
