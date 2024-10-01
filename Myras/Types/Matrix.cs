using Myras.Extensions;
using Myras.Utils;
using System.Text;

namespace Myras.Types
{
    /// <summary>
    /// Represents a multidimensional matrix with a specified shape and values.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Gets the shape of the matrix.
        /// </summary>
        public Shape Shape { get; }

        /// <summary>
        /// Gets the list of values contained in the matrix.
        /// </summary>
        public List<float> Values { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with a specified shape and values.
        /// </summary>
        /// <param name="shape">The shape of the matrix.</param>
        /// <param name="values">The initial values for the matrix.</param>
        public Matrix(Shape shape, IList<float> values)
        {
            Shape = shape;
            Values = [.. values];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with a specified shape and a default value for all elements.
        /// </summary>
        /// <param name="shape">The shape of the matrix.</param>
        /// <param name="defaultValue">The default value to fill the matrix.</param>
        public Matrix(Shape shape, float defaultValue = 0)
        {
            Shape = shape;
            Values = Enumerable.Repeat(defaultValue, shape.Dimensions.Aggregate(1, (a, b) => a * b)).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class from a multidimensional array.
        /// </summary>
        /// <param name="values">The multidimensional array of values.</param>
        public Matrix(Array values)
        {
            Shape = new(new int[values.Rank]);

            for (int dimension = 0; dimension < values.Rank; dimension++)
                Shape.Dimensions[dimension] = values.GetLength(dimension);

            Values = [.. values.FlattenArray<float>()];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with a single value.
        /// </summary>
        /// <param name="value">The value to initialize the matrix with.</param>
        public Matrix(float value)
        {
            Shape = new([1]);
            Values = [value];
        }

        /// <summary>
        /// Addition operator for two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of the addition.</returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            return MathM.Addition(a, b);
        }

        /// <summary>
        /// Subtraction operator for two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of the subtraction.</returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            return MathM.Subtraction(a, b);
        }

        /// <summary>
        /// The unary negation operator for a matrix.
        /// </summary>
        /// <param name="value">The matrix to negate.</param>
        /// <returns>The negation of the original.</returns>
        public static Matrix operator -(Matrix value)
        {
            return MathM.Multiplication(value, new(-1));
        }

        /// <summary>
        /// The multiplication operator for two matrices (element-wise).
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            return MathM.Multiplication(a, b);
        }

        /// <summary>
        /// The division operator for two matrices.
        /// </summary>
        /// <param name="a">The numerator matrix.</param>
        /// <param name="b">The denominator matrix.</param>
        /// <returns>The result of the division.</returns>
        public static Matrix operator /(Matrix a, Matrix b)
        {
            return MathM.Division(a, b);
        }

        /// <summary>
        /// Returns a string representation of the matrix.
        /// </summary>
        /// <returns>A string that represents the matrix.</returns>
        public override string? ToString()
        {
            StringBuilder sb = new();
            int[] dimensions = Shape.Dimensions;
            FormatValuesRecursive(sb, 0, dimensions, 0);
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves the value at a specified index in the matrix.
        /// </summary>
        /// <param name="index">The indices specifying the position of the value.</param>
        /// <returns>The value at the specified index.</returns>
        public float GetValue(params int[] index)
        {
            return Values[this.GetFlatIndex(index)];
        }

        /// <summary>
        /// Sets the value at a specified index in the matrix.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="index">The indices specifying the position to set the value.</param>
        public void SetValue(float value, params int[] index)
        {
            Values[this.GetFlatIndex(index)] = value;
        }

        /// <summary>
        /// A recursive method to format the values into a string based on their dimensions.
        /// </summary>
        /// <param name="sb">The StringBuilder to append the formatted values.</param>
        /// <param name="dim">The current dimension being processed.</param>
        /// <param name="dims">The total dimensions of the matrix.</param>
        /// <param name="flatIndex">The current flat index in the value list.</param>
        /// <returns>The updated flat index after formatting.</returns>
        private int FormatValuesRecursive(StringBuilder sb, int dim, int[] dims, int flatIndex)
        {
            if (dim == dims.Length - 1)
            {
                // Base case: We're at the innermost dimension, add the float values
                sb.Append('[');
                for (int i = 0; i < dims[dim]; i++)
                {
                    sb.Append(Values[flatIndex + i]);
                    if (i < dims[dim] - 1)
                        sb.Append(", ");
                }
                sb.Append(']');
                return flatIndex + dims[dim];
            }

            // Recursive case: Add nested arrays for higher dimensions
            sb.Append('[');
            for (int i = 0; i < dims[dim]; i++)
            {
                flatIndex = FormatValuesRecursive(sb, dim + 1, dims, flatIndex);
                if (i < dims[dim] - 1)
                    sb.Append(", ");
            }
            sb.Append(']');
            return flatIndex;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current matrix.
        /// </summary>
        /// <param name="obj">The object to compare with the current matrix.</param>
        /// <returns><c>true</c> if the specified object is equal to the current matrix; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is Matrix matrix &&
                   EqualityComparer<Shape>.Default.Equals(Shape, matrix.Shape) &&
                   Values.SequenceEqual(matrix.Values);
        }

        /// <summary>
        /// Serves as a hash function for the current matrix.
        /// </summary>
        /// <returns>A hash code for the current matrix.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Shape, Values);
        }
    }
}
