/**********************************************************************************************/
/**** Fichier : EnumerableExtension.cs                                                     ****/
/**** Projet  : QstReport                                                                  ****/
/**** Auteur  : LOHO Aurélien (SNA-RP/CDG/ST/DO-QST-INS)                                   ****/
/**********************************************************************************************/

namespace QstReport.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensionMethods
    {
        public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
        {
            HashSet<K> knownKeys = new HashSet<K>();

            foreach (var item in source)
            {
                if (knownKeys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

        public static long UniqueCount<T, K>(this IEnumerable<T> source, Func<T,K> keySelector)
        {
            return source.DistinctBy(keySelector).Count();
        }
    }
}
