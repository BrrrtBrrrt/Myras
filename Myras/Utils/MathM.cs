using Myras.Extensions;
using Myras.Types;

namespace Myras.Utils
{
    /// <summary>
    /// Provides a set of utility functions for performing mathematical operations on matrices.
    /// </summary>
    public static class MathM
    {
        /// <summary>
        /// Performs element-wise addition of two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>A new matrix that is the result of adding matrix 'a' and matrix 'b'.</returns>
        public static Matrix Addition(Matrix a, Matrix b)
        {
            return a.ElementWiseOperation(b, (x, y) => x + y);
        }

        /// <summary>
        /// Performs element-wise subtraction of two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>A new matrix that is the result of subtracting matrix 'b' from matrix 'a'.</returns>
        public static Matrix Subtraction(Matrix a, Matrix b)
        {
            return a.ElementWiseOperation(b, (x, y) => x - y);
        }

        /// <summary>
        /// Performs element-wise multiplication of two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>A new matrix that is the element-wise multiplication of matrix 'a' and matrix 'b'.</returns>
        public static Matrix Multiplication(Matrix a, Matrix b)
        {
            return a.ElementWiseOperation(b, (x, y) => x * y);
        }

        /// <summary>
        /// Performs element-wise division of two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>A new matrix that is the element-wise division of matrix 'a' by matrix 'b'.</returns>
        public static Matrix Division(Matrix a, Matrix b)
        {
            return a.ElementWiseOperation(b, (x, y) => x / y);
        }

        /// <summary>
        /// Applies the square root operation element-wise to a matrix.
        /// </summary>
        /// <param name="x">The input matrix.</param>
        /// <returns>A new matrix with the square root of each element of the input matrix.</returns>
        public static Matrix Sqrt(Matrix x)
        {
            return x.ElementWiseOperation(MathF.Sqrt);
        }

        /// <summary>
        /// Transposes a matrix according to a specified permutation of axes.
        /// </summary>
        /// <param name="x">The input matrix to transpose.</param>
        /// <param name="permutation">An optional list that specifies the new order of the dimensions. 
        /// If null, the dimensions will be reversed by default.</param>
        /// <returns>A transposed matrix with the dimensions permuted as specified.</returns>
        public static Matrix Transpose(Matrix x, IList<int>? permutation = null)
        {
            if (x.Shape.Rank == 1)
                throw new InvalidOperationException("Cannot transpose a 1D tensor.");

            permutation ??= Enumerable.Range(0, x.Shape.Rank).Reverse().ToList();

            if (permutation.Count != x.Shape.Rank)
                throw new ArgumentException("Permutation length must match the number of tensor dimensions.");

            Shape transposedShape = new(new int[permutation.Count]);
            for (int i = 0; i < permutation.Count; i++)
            {
                transposedShape.Dimensions[i] = x.Shape.Dimensions[permutation[i]];
            }

            Matrix result = new(transposedShape);

            for (int i = 0; i < result.Shape.TotalSize; i++)
            {
                int[] resultIndex = result.GetMultiDimensionalIndex(i);
                int[] originalIndex = new int[resultIndex.Length];

                for (int j = 0; j < resultIndex.Length; j++)
                {
                    originalIndex[permutation[j]] = resultIndex[j];
                }

                int originalFlatIndex = x.GetFlatIndex(originalIndex);
                result.Values[i] = x.Values[originalFlatIndex];
            }

            return result;
        }

        /// <summary>
        /// Computes the dot product of two matrices. The matrices must be either 1D (vectors) or 2D.
        /// </summary>
        /// <param name="a">The first matrix (vector or matrix).</param>
        /// <param name="b">The second matrix (vector or matrix).</param>
        /// <returns>The result of the dot product as a new matrix.</returns>
        public static Matrix DotProduct(Matrix a, Matrix b)
        {
            if (a.Shape.Rank != 1 && a.Shape.Rank != 2)
                throw new ArgumentException("Matrix a must be a 1D or 2D.");

            if (b.Shape.Rank != 1 && b.Shape.Rank != 2)
                throw new ArgumentException("Matrix b must be a 1D or 2D.");

            Matrix c = new(0);

            if (a.Shape.Rank == 1 && b.Shape.Rank == 1)
            {
                Parallel.For(0, a.Shape.TotalSize, i =>
                {
                    c.Values[0] += a.Values[i] * b.Values[i];
                });
            }
            else
            {
                int rowsA = a.Shape.Dimensions[0];
                int colsA = a.Shape.Dimensions[1];
                int rowsB = b.Shape.Dimensions[0];
                int colsB = b.Shape.Dimensions[1];

                float[] cValues = new float[rowsA * colsB];

                Parallel.For(0, rowsA, i =>
                {
                    int rowStartA = i * colsA;
                    for (int j = 0; j < colsB; j++)
                    {
                        float sum = 0;
                        int colStartB = j;
                        for (int k = 0; k < colsA; k++)
                        {
                            sum += a.Values[rowStartA + k] * b.Values[colStartB];
                            colStartB += colsB; // move to the next row in tensor b
                        }
                        cValues[i * colsB + j] = sum;
                    }
                });

                c = new(new Shape([rowsA, colsB]), cValues);
            }

            return c;
        }
    }
}
