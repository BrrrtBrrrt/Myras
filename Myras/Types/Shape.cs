
namespace Myras.Types
{
    /// <summary>
    /// Represents the shape of an matrix or tensor, defined by its dimensions.
    /// </summary>
    public readonly struct Shape
    {
        /// <summary>
        /// Gets the dimensions of the shape.
        /// </summary>
        public int[] Dimensions { get; }

        /// <summary>
        /// Gets the rank (number of dimensions) of the shape.
        /// </summary>
        public readonly int Rank => Dimensions.Length; // Number of dimensions

        /// <summary>
        /// Gets the total size of the shape, calculated as the product of its dimensions.
        /// </summary>
        public readonly int TotalSize => Dimensions.Aggregate(1, (a, b) => a * b);

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> struct with the specified dimensions.
        /// </summary>
        /// <param name="dimensions">A collection of integers representing the dimensions of the shape.</param>
        public Shape(IList<int> dimensions)
        {
            Dimensions = [.. dimensions];
        }

        /// <summary>
        /// Computes the broadcasted shape of two shapes according to broadcasting rules.
        /// </summary>
        /// <param name="shapeA">The first shape.</param>
        /// <param name="shapeB">The second shape.</param>
        /// <returns>A new <see cref="Shape"/> that represents the broadcasted shape.</returns>
        /// <exception cref="ArgumentException">Thrown when the shapes are not broadcastable due to incompatible dimensions.</exception>
        public static Shape GetBroadcastedShape(Shape shapeA, Shape shapeB)
        {
            int rank = Math.Max(shapeA.Rank, shapeB.Rank);

            int[] extendedDimensionsA = [.. shapeA.Dimensions];
            int[] extendedDimensionsB = [.. shapeB.Dimensions];

            int dimensionsSizeCompareResult = extendedDimensionsA.Length.CompareTo(extendedDimensionsB.Length);

            if (dimensionsSizeCompareResult < 0)
                extendedDimensionsA = [.. Enumerable.Repeat(1, extendedDimensionsB.Length - extendedDimensionsA.Length), .. extendedDimensionsA];
            else if (dimensionsSizeCompareResult > 0)
                extendedDimensionsB = [.. Enumerable.Repeat(1, extendedDimensionsA.Length - extendedDimensionsB.Length), .. extendedDimensionsB];

            int[] resultDimensions = new int[rank];

            for (int dimension = 0; dimension < rank; dimension++)
            {
                int dimensionSizeA = extendedDimensionsA[dimension];
                int dimensionSizeB = extendedDimensionsB[dimension];

                if (dimensionSizeA != dimensionSizeB && (dimensionSizeA != 1 && dimensionSizeB != 1))
                    throw new ArgumentException($"Shapes are not broadcastable, dimensions do not match. Shapes {shapeA} {shapeB}");

                resultDimensions[dimension] = Math.Max(dimensionSizeA, dimensionSizeB);
            }

            return new Shape(resultDimensions);
        }

        /// <summary>
        /// Returns a string representation of the shape, displaying its dimensions.
        /// </summary>
        /// <returns>A string that represents the shape.</returns>
        public override readonly string? ToString()
        {
            return $"({string.Join(", ", Dimensions)})";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Shape"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="Shape"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="Shape"/>; otherwise, <c>false</c>.</returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Shape shape &&
                   Dimensions.SequenceEqual(shape.Dimensions);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Shape"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Shape"/>.</returns>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Dimensions);
        }

        /// <summary>
        /// Compares two <see cref="Shape"/> instances for equality.
        /// </summary>
        /// <param name="left">The left <see cref="Shape"/> to compare.</param>
        /// <param name="right">The right <see cref="Shape"/> to compare.</param>
        /// <returns><c>true</c> if both shapes are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Shape left, Shape right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Shape"/> instances for inequality.
        /// </summary>
        /// <param name="left">The left <see cref="Shape"/> to compare.</param>
        /// <param name="right">The right <see cref="Shape"/> to compare.</param>
        /// <returns><c>true</c> if the shapes are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Shape left, Shape right)
        {
            return !(left == right);
        }
    }
}
