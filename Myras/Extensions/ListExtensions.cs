namespace Myras.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Splits a list into several smaller lists of a specified size.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to split into smaller lists.</param>
        /// <param name="size">The maximum size of each smaller list.</param>
        /// <returns>A list containing smaller lists of the specified size.</returns>
        public static List<List<T>> Split<T>(this List<T> list, int size)
        {
            List<List<T>> result = [];

            for (int i = 0; i < list.Count; i += size)
                result.Add(list[i..(i + size)]);

            return result;
        }

        /// <summary>
        /// Shuffles the elements of a list in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Constants.random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        /// <summary>
        /// Creates a deep copy of a four-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="list">The four-dimensional array to copy.</param>
        /// <returns>A deep copy of the specified four-dimensional array.</returns>
        public static T[][][][] CopyDeep<T>(this T[][][][] list)
        {
            T[][][][] result = new T[list.Length][][][];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = list[i].CopyDeep();
            }

            return result;
        }

        /// <summary>
        /// Creates a deep copy of a three-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="list">The three-dimensional array to copy.</param>
        /// <returns>A deep copy of the specified three-dimensional array.</returns>
        public static T[][][] CopyDeep<T>(this T[][][] list)
        {
            T[][][] result = new T[list.Length][][];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = list[i].CopyDeep();
            }

            return result;
        }

        /// <summary>
        /// Creates a deep copy of a two-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="list">The two-dimensional array to copy.</param>
        /// <returns>A deep copy of the specified two-dimensional array.</returns>
        public static T[][] CopyDeep<T>(this T[][] list)
        {
            T[][] result = new T[list.Length][];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = list[i].CopyDeep();
            }

            return result;
        }

        /// <summary>
        /// Creates a deep copy of a one-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="list">The one-dimensional array to copy.</param>
        /// <returns>A deep copy of the specified one-dimensional array.</returns>
        public static T[] CopyDeep<T>(this T[] list)
        {
            T[] result = new T[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = list[i];
            }

            return result;
        }

        /// <summary>
        /// Sorts a target list and updates related lists according to the specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of elements in the target list.</typeparam>
        /// <param name="targetList">The list to be sorted.</param>
        /// <param name="comparison">A comparison delegate that defines the sort order for the target list.</param>
        /// <param name="relatedLists">A list of related lists that will be sorted alongside the target list.</param>
        /// <exception cref="ArgumentException">Thrown if the lengths of related lists do not match the target list.</exception>
        public static void SortWithRelatedLists<T>(this List<T> targetList, Comparison<T> comparison, IList<IList<T>> relatedLists)
        {
            // If relatedLists is null or empty, just sort the target list and return
            if (relatedLists == null || relatedLists.Count == 0)
            {
                targetList.Sort(comparison);
                return;
            }

            if (relatedLists.Any(list => list.Count != targetList.Count))
            {
                throw new ArgumentException("All related lists must have the same length as the target list.");
            }

            // Create a list of tuples where each tuple contains an item from the target list and corresponding items from related lists
            var combinedList = targetList
                .Select((item, index) => new { Item = item, RelatedItems = relatedLists.Select(list => list[index]).ToList() })
                .ToList();

            // Sort the combined list using the provided comparison for the target list
            combinedList.Sort((x, y) => comparison(x.Item, y.Item));

            // Reconstruct the target list and related lists in the sorted order
            for (int i = 0; i < targetList.Count; i++)
            {
                targetList[i] = combinedList[i].Item;
                for (int j = 0; j < relatedLists.Count; j++)
                {
                    relatedLists[j][i] = combinedList[i].RelatedItems[j];
                }
            }
        }
    }
}
