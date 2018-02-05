/**********************************************************************************************/
/**** Fichier : TimePeriodCollection.cs                                                    ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimePeriodCollection : List<TimePeriod>
    {
        public DateTime GlobalStart { get { return this.Min(x => x.Start); } }

        public DateTime GlobalEnd { get { return this.Max(x => x.End); } }
    }
}
