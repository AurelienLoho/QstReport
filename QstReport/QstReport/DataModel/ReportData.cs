

namespace QstReport.DataModel
{
    using QstReport.Utils;
    using System;
    using System.Collections.Generic;

    public sealed class ReportData
    {
        public ReportData(TimePeriod currentDataPeriod, TimePeriod pastDataPeriod)
        {
            CurrentDataPeriod = currentDataPeriod;
            PastDataPeriod = pastDataPeriod;

            ReportPeriod = TimePeriod.Merge(pastDataPeriod, currentDataPeriod);
        }

        public ReportData(DateTime startPeriod, DateTime endPeriod)
        {
            ReportPeriod = new TimePeriod(startPeriod, endPeriod);
            PastDataPeriod = ReportPeriod; // TODO : Compute
            CurrentDataPeriod = ReportPeriod; // TODO : Compute
        }

        public TimePeriod ReportPeriod { get; private set; }

        public TimePeriod PastDataPeriod { get; private set; }

        public TimePeriod CurrentDataPeriod { get; private set; }

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
