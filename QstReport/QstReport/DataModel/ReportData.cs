

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

        private List<Avt> _avtCollection = new List<Avt>();
        public List<Avt> AvtCollection
        {
            get { return _avtCollection; }
            set { _avtCollection = value; }
        }

        private List<TechEvent> _techEventCollection = new List<TechEvent>();
        public List<TechEvent> TechEventCollection
        {
            get { return _techEventCollection; }
            set { _techEventCollection = value; }
        }

        private List<ExploitEvent> _exploitEventCollection = new List<ExploitEvent>();
        public List<ExploitEvent> ExploitEventCollection
        {
            get { return _exploitEventCollection; }
            set { _exploitEventCollection = value; }
        }
    }
}
