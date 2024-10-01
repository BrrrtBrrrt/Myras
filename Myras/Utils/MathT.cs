using Myras.Enums;
using Myras.Extensions;
using Myras.Types;

namespace Myras.Utils
{
    /// <summary>
    /// A utility class providing common mathematical operations for tensors.
    /// </summary>
    public static class MathT
    {
        /// <summary>
        /// Defines a tensor addition operation.
        /// </summary>
        /// <param name="a">First input tensor.</param>
        /// <param name="b">Second input tensor.</param>
        /// <returns>A TensorOperation object representing the addition.</returns>
        public static TensorOperation AdditionOp(Tensor a, Tensor b)
        {
            return new(
                TensorOperationType.ADDITION.ToString(),
                [a, b],
                (operation) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor c = new(a.Values + b.Values);
                    return [c];
                },
                (operation, gradientNode) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor gradientA = new(gradientNode.Gradient.Values.ReduceSum(a.Shape));
                    Tensor gradientB = new(gradientNode.Gradient.Values.ReduceSum(b.Shape));

                    return [gradientA, gradientB];
                }
            );
        }

        /// <summary>
        /// Adds two tensors element-wise.
        /// </summary>
        /// <param name="a">First tensor.</param>
        /// <param name="b">Second tensor.</param>
        /// <param name="tape">Optional gradient tape to record the operation for automatic differentiation.</param>
        /// <returns>The resulting tensor after addition.</returns>
        public static Tensor Addition(Tensor a, Tensor b, GradientTape? tape = null)
        {
            TensorOperation operation = AdditionOp(a, b);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a tensor subtraction operation.
        /// </summary>
        /// <param name="a">First input tensor.</param>
        /// <param name="b">Second input tensor.</param>
        /// <returns>A TensorOperation object representing the subtraction.</returns>
        public static TensorOperation SubtractionOp(Tensor a, Tensor b)
        {
            return new(
                TensorOperationType.SUBTRACTION.ToString(),
                [a, b],
                (operation) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor c = new(a.Values - b.Values);
                    return [c];
                },
                (operation, gradientNode) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor gradientA = new(gradientNode.Gradient.Values.ReduceSum(a.Shape));
                    Tensor gradientB = new(-gradientNode.Gradient.Values.ReduceSum(b.Shape));
                    return [gradientA, gradientB];
                }
            );
        }

        /// <summary>
        /// Subtracts the second tensor from the first tensor element-wise.
        /// </summary>
        /// <param name="a">First tensor.</param>
        /// <param name="b">Second tensor.</param>
        /// <param name="tape">Optional gradient tape to record the operation for automatic differentiation.</param>
        /// <returns>The resulting tensor after subtraction.</returns>
        public static Tensor Subtraction(Tensor a, Tensor b, GradientTape? tape = null)
        {
            TensorOperation operation = SubtractionOp(a, b);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a tensor multiplication operation.
        /// </summary>
        /// <param name="a">First input tensor.</param>
        /// <param name="b">Second input tensor.</param>
        /// <returns>A TensorOperation object representing the multiplication.</returns>
        public static TensorOperation MultiplicationOp(Tensor a, Tensor b)
        {
            return new(
                TensorOperationType.MULTIPLICATION.ToString(),
                [a, b],
                (operation) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor c = new(a.Values * b.Values);
                    return [c];
                },
                (operation, gradientNode) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];

                    int shapeCompareResult = a.Shape.TotalSize.CompareTo(b.Shape.TotalSize);

                    Matrix gradientAValues = b.Values;
                    Matrix gradientBValues = a.Values;

                    if (shapeCompareResult != 0)
                    {
                        gradientAValues = shapeCompareResult < 0 ? b.Values.ReduceSum(a.Shape) : b.Values.Broadcast(a.Shape);
                        gradientBValues = shapeCompareResult < 0 ? a.Values.Broadcast(b.Shape) : a.Values.ReduceSum(b.Shape);
                    }

                    Tensor gradientA = new(gradientAValues * gradientNode.Gradient.Values.ReduceSum(a.Shape));
                    Tensor gradientB = new(gradientBValues * gradientNode.Gradient.Values.ReduceSum(b.Shape));
                    return [gradientA, gradientB];
                }
            );
        }

        /// <summary>
        /// Multiplies two tensors element-wise.
        /// </summary>
        /// <param name="a">First tensor.</param>
        /// <param name="b">Second tensor.</param>
        /// <param name="tape">Optional gradient tape to record the operation for automatic differentiation.</param>
        /// <returns>The resulting tensor after multiplication.</returns>
        public static Tensor Multiplication(Tensor a, Tensor b, GradientTape? tape = null)
        {
            TensorOperation operation = MultiplicationOp(a, b);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a tensor division operation.
        /// </summary>
        /// <param name="a">First input tensor (numerator).</param>
        /// <param name="b">Second input tensor (denominator).</param>
        /// <returns>A TensorOperation object representing the division.</returns>
        public static TensorOperation DivisionOp(Tensor a, Tensor b)
        {
            return new(
                TensorOperationType.DIVISION.ToString(),
                [a, b],
                (operation) => [new(operation.Inputs[0].Values / operation.Inputs[1].Values)],
                (operation, gradientNode) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor gradientA = new(new Matrix(1) / b.Values.ReduceSum(a.Shape) * gradientNode.Gradient.Values.ReduceSum(a.Shape));
                    Tensor gradientB = new(-(a.Values.ReduceSum(b.Shape) / (b.Values * b.Values)) * gradientNode.Gradient.Values.ReduceSum(b.Shape));
                    return [gradientA, gradientB];
                }
            );
        }

        /// <summary>
        /// Divides two tensors element-wise.
        /// </summary>
        /// <param name="a">Numerator tensor.</param>
        /// <param name="b">Denominator tensor.</param>
        /// <param name="tape">Optional gradient tape to record the operation for automatic differentiation.</param>
        /// <returns>The resulting tensor after division.</returns>
        public static Tensor Division(Tensor a, Tensor b, GradientTape? tape = null)
        {
            TensorOperation operation = DivisionOp(a, b);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a square root operation on a tensor.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <returns>A TensorOperation representing the square root operation.</returns>
        public static TensorOperation SqrtOp(Tensor x)
        {
            return new(
                TensorOperationType.SQUARE_ROOT.ToString(),
                [x],
                (operation) => [new(MathM.Sqrt(operation.Inputs[0].Values))],
                (operation, gradientNode) =>
                {
                    Tensor x = operation.Inputs[0];
                    Tensor gradientX = new(new Matrix(1) / (new Matrix(2) * MathM.Sqrt(x.Values)) * gradientNode.Gradient.Values);
                    return [gradientX];
                }
            );
        }

        /// <summary>
        /// Computes the square root of a tensor, optionally recording the operation for gradient calculation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The resulting tensor after applying the square root.</returns>
        public static Tensor Sqrt(Tensor x, GradientTape? tape = null)
        {
            TensorOperation operation = SqrtOp(x);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a transpose operation for a tensor.
        /// </summary>
        /// <param name="x">Input tensor to transpose.</param>
        /// <returns>A TensorOperation representing the transpose operation.</returns>
        public static TensorOperation TransposeOp(Tensor x)
        {
            return new(
                TensorOperationType.TRANSPOSE.ToString(),
                [x],
                (operation) => [new(MathM.Transpose(operation.Inputs[0].Values))],
                (operation, gradientNode) =>
                {
                    Tensor x = operation.Inputs[0];
                    Tensor gradientX = new(MathM.Transpose(gradientNode.Gradient.Values));
                    return [gradientX];
                }
            );
        }

        /// <summary>
        /// Transposes a tensor, optionally recording the operation for gradient calculation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The transposed tensor.</returns>
        public static Tensor Transpose(Tensor x, GradientTape? tape = null)
        {
            TensorOperation operation = TransposeOp(x);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a dot product operation between two tensors.
        /// </summary>
        /// <param name="a">First input tensor.</param>
        /// <param name="b">Second input tensor.</param>
        /// <returns>A TensorOperation representing the dot product.</returns>
        public static TensorOperation DotProductOp(Tensor a, Tensor b)
        {
            return new(
                TensorOperationType.DOT_PRODUCT.ToString(),
                [a, b],
                (operation) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];
                    Tensor c = new(MathM.DotProduct(a.Values, b.Values));

                    return [c];
                },
                (operation, gradientNode) =>
                {
                    Tensor a = operation.Inputs[0];
                    Tensor b = operation.Inputs[1];

                    Matrix gradientAValues = MathM.DotProduct(gradientNode.Gradient.Values, MathM.Transpose(b.Values));
                    Matrix gradientBValues = MathM.DotProduct(MathM.Transpose(a.Values), gradientNode.Gradient.Values);

                    Tensor gradientA = new(gradientAValues);
                    Tensor gradientB = new(gradientBValues);
                    return [gradientA, gradientB];
                }
            );
        }

        /// <summary>
        /// Computes the dot product of two tensors, optionally recording the operation for gradient calculation.
        /// </summary>
        /// <param name="a">First input tensor.</param>
        /// <param name="b">Second input tensor.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The resulting tensor from the dot product.</returns>
        public static Tensor DotProduct(Tensor a, Tensor b, GradientTape? tape = null)
        {
            TensorOperation operation = DotProductOp(a, b);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines the mean squared error (MSE) loss function operation.
        /// </summary>
        /// <param name="predicted">Predicted tensor values.</param>
        /// <param name="target">Target tensor values.</param>
        /// <returns>A TensorOperation representing the MSE loss calculation.</returns>
        public static TensorOperation MseOp(Tensor predicted, Tensor target)
        {
            return new(
                TensorOperationType.LOSS_FUNCTION_MSE.ToString(),
                [predicted, target],
                (operation) =>
                {
                    Tensor predicted = operation.Inputs[0];
                    Tensor target = operation.Inputs[1];

                    float[] squaredErrors = new float[predicted.Shape.TotalSize];

                    Parallel.For(0, predicted.Shape.TotalSize, i =>
                    {
                        float p = predicted.Values.Values[i];
                        float t = target.Values.Values[i];
                        squaredErrors[i] = (p - t) * (p - t);
                    });

                    Tensor result = new(squaredErrors.Average());

                    return [result];
                },
                (operation, gradientNode) =>
                {
                    Tensor predicted = operation.Inputs[0];
                    Tensor target = operation.Inputs[1];

                    float[] gradients = new float[predicted.Shape.TotalSize];

                    Parallel.For(0, predicted.Shape.TotalSize, i =>
                    {
                        float p = predicted.Values.Values[i];
                        float t = target.Values.Values[i];
                        gradients[i] = 2 * (p - t) / predicted.Shape.TotalSize;
                    });

                    Tensor gradientPredicted = new(new Matrix(predicted.Shape, gradients) * gradientNode.Gradient.Values);

                    return [gradientPredicted];
                }
            );
        }

        /// <summary>
        /// Computes the mean squared error (MSE) between the predicted and target tensors, optionally recording the operation.
        /// </summary>
        /// <param name="predicted">Predicted tensor values.</param>
        /// <param name="target">Target tensor values.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The resulting MSE loss.</returns>
        public static Tensor Mse(Tensor predicted, Tensor target, GradientTape? tape = null)
        {
            TensorOperation operation = MseOp(predicted, target);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a ReLU (Rectified Linear Unit) activation function operation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <returns>A TensorOperation representing the ReLU activation.</returns>
        public static TensorOperation ReluOp(Tensor x)
        {
            return new(
                TensorOperationType.ACTIVATION_FUNCTION_RELU.ToString(),
                [x],
                (operation) =>
                {
                    Tensor x = operation.Inputs[0];

                    float[] yValues = new float[x.Shape.TotalSize];

                    Parallel.For(0, x.Shape.TotalSize, i =>
                    {
                        yValues[i] = MathF.Max(0, x.Values.Values[i]);
                    });

                    Tensor y = new(x.Shape, yValues);


                    return [y];
                },
                (operation, gradientNode) =>
                {
                    Tensor x = operation.Inputs[0];

                    float[] gradientXValues = new float[x.Shape.TotalSize];

                    Parallel.For(0, x.Shape.TotalSize, i =>
                    {
                        gradientXValues[i] = x.Values.Values[i] == 0 ? float.NaN : x.Values.Values[i] < 0 ? 0 : 1;
                    });

                    Tensor gradientX = new(new Matrix(x.Shape, gradientXValues) * gradientNode.Gradient.Values);

                    return [gradientX];
                }
            );
        }

        /// <summary>
        /// Applies the ReLU activation function to a tensor, optionally recording the operation for gradient calculation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The resulting tensor after applying ReLU.</returns>
        public static Tensor Relu(Tensor x, GradientTape? tape = null)
        {
            TensorOperation operation = ReluOp(x);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }

        /// <summary>
        /// Defines a linear (identity) activation function operation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <returns>A TensorOperation representing the linear activation function.</returns>
        public static TensorOperation LinearOp(Tensor x)
        {
            return new(
                TensorOperationType.ACTIVATION_FUNCTION_LINEAR.ToString(),
                [x],
                (operation) =>
                {
                    return [new(operation.Inputs[0].Values)];
                },
                (operation, gradientNode) =>
                {
                    return [new(gradientNode.Gradient.Values)];
                }
            );
        }

        /// <summary>
        /// Applies the linear activation function to a tensor, optionally recording the operation for gradient calculation.
        /// </summary>
        /// <param name="x">Input tensor.</param>
        /// <param name="tape">Optional gradient tape for recording the operation.</param>
        /// <returns>The resulting tensor after applying the linear activation function.</returns>
        public static Tensor Linear(Tensor x, GradientTape? tape = null)
        {
            TensorOperation operation = LinearOp(x);
            Tensor result = operation.CallSingleResult();
            tape?.Record(operation);
            return result;
        }
    }
}
