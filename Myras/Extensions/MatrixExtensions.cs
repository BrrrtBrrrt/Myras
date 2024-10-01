using Myras.Types;

namespace Myras.Extensions
{
    public static class MatrixExtensions
    {
        /// <summary>
        /// Calculates the flat index of a multi-dimensional index in a matrix.
        /// </summary>
        /// <param name="matrix">The matrix for which to calculate the flat index.</param>
        /// <param name="multiDimensionalIndex">An array of indices representing the multi-dimensional index.</param>
        /// <returns>The flat index corresponding to the multi-dimensional index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the length of the multi-dimensional index does not match the matrix shape rank or when an index is out of bounds.</exception>
        public static int GetFlatIndex(this Matrix matrix, params int[] multiDimensionalIndex)
        {
            if (multiDimensionalIndex.Length != matrix.Shape.Rank)
                throw new ArgumentOutOfRangeException(nameof(multiDimensionalIndex), "Index length must be equal to the matrix shape rank");

            int flatIndex = 0;

            for (int dimensionIndex = 0; dimensionIndex < multiDimensionalIndex.Length; dimensionIndex++)
            {
                if (multiDimensionalIndex[dimensionIndex] >= matrix.Shape.Dimensions[dimensionIndex])
                    throw new ArgumentOutOfRangeException(nameof(multiDimensionalIndex), $"Index does not match dimension size. Index: {multiDimensionalIndex[dimensionIndex]} Dimension size: {matrix.Shape.Dimensions[dimensionIndex]}");

                if (multiDimensionalIndex[dimensionIndex] < 0)
                    throw new ArgumentOutOfRangeException(nameof(multiDimensionalIndex), $"Index must not contain negative values.");

                flatIndex += multiDimensionalIndex[dimensionIndex] * matrix.Shape.Dimensions[(dimensionIndex + 1)..].Aggregate(1, (a, b) => a * b);
            }

            return flatIndex;
        }

        /// <summary>
        /// Converts a flat index into a multi-dimensional index in the matrix.
        /// </summary>
        /// <param name="matrix">The matrix for which to convert the flat index.</param>
        /// <param name="flatIndex">The flat index to convert.</param>
        /// <returns>An array representing the multi-dimensional index corresponding to the flat index.</returns>
        /// <exception cref="ArgumentException">Thrown when the matrix shape dimensions are less than or equal to zero.</exception>
        public static int[] GetMultiDimensionalIndex(this Matrix matrix, int flatIndex)
        {
            int[] indices = new int[matrix.Shape.Rank];

            for (int i = matrix.Shape.Rank - 1; i >= 0; i--)
            {
                if (matrix.Shape.Dimensions[i] <= 0)
                    throw new ArgumentException("Shape dimensions must be greater than 0.");

                indices[i] = flatIndex % matrix.Shape.Dimensions[i];
                flatIndex /= matrix.Shape.Dimensions[i];
            }

            return indices;
        }

        /// <summary>
        /// Broadcasts the matrix to a new shape by expanding its dimensions.
        /// </summary>
        /// <param name="matrix">The matrix to broadcast.</param>
        /// <param name="shape">The target shape to broadcast the matrix to.</param>
        /// <returns>A new matrix with the specified shape.</returns>
        /// <exception cref="ArgumentException">Thrown when the matrix cannot be broadcasted to the specified shape.</exception>
        public static Matrix Broadcast(this Matrix matrix, Shape shape)
        {
            Shape extendedShape = new([.. Enumerable.Repeat(1, shape.Rank - matrix.Shape.Rank), .. matrix.Shape.Dimensions]);

            List<float> values = new(matrix.Values);

            for (int dimensionIndex = shape.Rank - 1; dimensionIndex >= 0; dimensionIndex--)
            {
                int dimensionSize = extendedShape.Dimensions[dimensionIndex];
                int targetDimensionSize = shape.Dimensions[dimensionIndex];

                if (dimensionSize == targetDimensionSize)
                    continue;

                if (dimensionSize == 1 && targetDimensionSize > 1)
                {
                    int dimensionsElementCount = shape.Dimensions[(dimensionIndex + 1)..].Aggregate(1, (a, b) => a * b);

                    List<List<float>> dimensionGroups = values.Split(dimensionsElementCount);

                    List<float> newValues = [];

                    foreach (var dimensionGroup in dimensionGroups)
                        for (int i = 0; i < targetDimensionSize; i++)
                            newValues.AddRange(dimensionGroup);

                    values = newValues;
                }
                else
                    throw new ArgumentException($"operand could not be broadcast with shape {matrix.Shape} to {shape}");
            }

            return new(shape, values);
        }

        /// <summary>
        /// Reduces the matrix by summing over the specified dimensions.
        /// </summary>
        /// <param name="matrix">The matrix to reduce.</param>
        /// <param name="shape">The shape specifying the dimensions over which to sum.</param>
        /// <returns>A new matrix with the summed values.</returns>
        /// <exception cref="ArgumentException">Thrown when the matrix cannot be reduced to the specified shape.</exception>
        public static Matrix ReduceSum(this Matrix matrix, Shape shape)
        {
            Shape extendedShape = new([.. Enumerable.Repeat(1, matrix.Shape.Rank - shape.Rank), .. shape.Dimensions]);

            List<float> values = new(matrix.Values);

            for (int dimensionIndex = extendedShape.Rank - 1; dimensionIndex >= 0; dimensionIndex--)
            {
                int dimensionSize = matrix.Shape.Dimensions[dimensionIndex];
                int targetDimensionSize = extendedShape.Dimensions[dimensionIndex];

                if (dimensionSize == targetDimensionSize)
                    continue;

                if (targetDimensionSize == 1 && dimensionSize > 1)
                {
                    int dimensionsElementCount = shape.Dimensions.Length > 1 ? shape.Dimensions[(dimensionIndex + 1)..].Aggregate(1, (a, b) => a * b) : 1;

                    List<List<float>> dimensionGroups = values.Split(dimensionsElementCount);

                    List<float> newValues = [];

                    for (int i = 0; i < dimensionGroups.Count; i += dimensionSize)
                    {
                        List<float> sum = Enumerable.Repeat(0f, dimensionsElementCount).ToList();

                        for (int j = 0; j < dimensionSize; j++)
                        {
                            List<float> dimensionGroup = dimensionGroups[i + j];

                            for (int k = 0; k < dimensionsElementCount; k++)
                                sum[k] += dimensionGroup[k];
                        }

                        newValues.AddRange(sum);
                    }

                    values = newValues;
                }
                else
                    throw new ArgumentException($"operand could not be reduced with shape {matrix.Shape} to {shape}");
            }

            return new(shape, values);
        }

        /// <summary>
        /// Reduces the matrix by checking if the values are the same across the specified dimensions.
        /// </summary>
        /// <param name="matrix">The matrix to reduce.</param>
        /// <param name="shape">The shape specifying the dimensions over which to check for equality.</param>
        /// <returns>A new matrix with values that are the same across the specified dimensions.</returns>
        /// <exception cref="ArgumentException">Thrown when the matrix cannot be reduced to the specified shape due to differing values.</exception>
        public static Matrix Reduce(this Matrix matrix, Shape shape)
        {
            Shape extendedShape = new([.. Enumerable.Repeat(1, matrix.Shape.Rank - shape.Rank), .. shape.Dimensions]);

            List<float> values = new(matrix.Values);

            for (int dimensionIndex = extendedShape.Rank - 1; dimensionIndex >= 0; dimensionIndex--)
            {
                int dimensionSize = matrix.Shape.Dimensions[dimensionIndex];
                int targetDimensionSize = extendedShape.Dimensions[dimensionIndex];

                if (dimensionSize == targetDimensionSize)
                    continue;

                if (targetDimensionSize == 1 && dimensionSize > 1)
                {
                    int dimensionsElementCount = shape.Dimensions.Length > 1 ? shape.Dimensions[(dimensionIndex + 1)..].Aggregate(1, (a, b) => a * b) : 1;

                    List<List<float>> dimensionGroups = values.Split(dimensionsElementCount);

                    List<float> newValues = [];

                    for (int i = 0; i < dimensionGroups.Count; i += dimensionSize)
                    {
                        List<float> baseGroup = dimensionGroups[i];

                        for (int j = 0; j < dimensionSize; j++)
                        {
                            List<float> dimensionGroup = dimensionGroups[i + j];

                            for (int k = 0; k < dimensionsElementCount; k++)
                                if (baseGroup[k] != dimensionGroup[k])
                                    throw new Exception("Matrix cannot be reduced, values are not reducable");
                        }

                        newValues.AddRange(baseGroup);
                    }

                    values = newValues;
                }
                else
                    throw new ArgumentException($"operand could not be reduced with shape {matrix.Shape} to {shape}");
            }

            return new(shape, values);
        }

        /// <summary>
        /// Performs an element-wise operation between two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <param name="operation">The function defining the operation to perform.</param>
        /// <returns>A new matrix resulting from the element-wise operation.</returns>
        public static Matrix ElementWiseOperation(this Matrix a, Matrix b, Func<float, float, float> operation)
        {
            Shape resultShape = Shape.GetBroadcastedShape(a.Shape, b.Shape);

            Matrix aBroadCasted = a.Broadcast(resultShape);
            Matrix bBroadCasted = b.Broadcast(resultShape);

            Matrix result = new(resultShape);

            Parallel.For(0, result.Values.Count, i =>
            {
                result.Values[i] = operation(aBroadCasted.Values[i], bBroadCasted.Values[i]);
            });

            return result;
        }

        /// <summary>
        /// Performs an element-wise operation on a matrix.
        /// </summary>
        /// <param name="x">The matrix on which to perform the operation.</param>
        /// <param name="operation">The function defining the operation to perform.</param>
        /// <returns>A new matrix resulting from the element-wise operation.</returns>
        public static Matrix ElementWiseOperation(this Matrix x, Func<float, float> operation)
        {
            Matrix result = new(x.Shape);

            Parallel.For(0, result.Values.Count, i =>
            {
                result.Values[i] = operation(x.Values[i]);
            });

            return result;
        }
    }
}
