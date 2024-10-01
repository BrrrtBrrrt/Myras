namespace Myras.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Flattens a multi-dimensional array into a one-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="array">The multi-dimensional array to be flattened.</param>
        /// <returns>A one-dimensional array containing all elements from the multi-dimensional array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided array is null.</exception>
        public static T[] FlattenArray<T>(this Array array)
        {
            // Get the total number of elements in the array
            int totalLength = array.Length;

            // Create a new one-dimensional float array with the same total length
            T[] flatArray = new T[totalLength];

            // Flatten the multi-dimensional array by iterating through it
            int index = 0;
            foreach (object? element in array)
            {
                // Convert each element to float and add to the flattened array
                flatArray[index++] = (T)element;
            }

            return flatArray;
        }
    }
}
