using QstReport.DataModel;
using QstReport.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QstReport.Siam5
{
    [DebuggerDisplay("{id} - {id_evt} - {id_occurrence}")]
    public class Siam5AvtData
    {
        public string id { get; set; }

        public string id_evt { get; set; }

        public string id_occurrence { get; set; }

        public string from { get; set; }

        public string to { get; set; }

        public string type { get; set; }

        public int status { get; set; }

        public string Label { get; set; }
    }

    public class Siam5RequestResult<DataType>
    {
        public Siam5DataCollection<DataType> success { get; set; }
    }

    public class Siam5DataCollection<DataType>
    {
        public int total { get; set; }

        public List<DataType> items { get; set; }
    }

    public class Siam5EventData
    {
        public string id { get; set; }

        public int id_evt { get; set; }

        public int id_occurrence { get; set; }

        public long from { get; set; }

        public long to { get; set; }

        public string type { get; set; }

        public int status { get; set; }

        public string label { get; set; }

        public string tooltip { get; set; }

        public string filter { get; set; }

        public string lastUpdate { get; set; }

        public string html { get; set; }
    }

}
