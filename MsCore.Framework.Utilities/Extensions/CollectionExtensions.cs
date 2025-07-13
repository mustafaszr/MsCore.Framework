using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Utilities.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Koleksiyonun null veya boş olup olmadığını kontrol eder.
        /// </summary>
        public static bool MsIsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// ICollection tipine birden fazla öğe ekler.
        /// </summary>
        public static void MsAddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Koleksiyon üzerinde döner ve her bir öğeye verilen action'u uygular.
        /// </summary>
        public static void MsForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
