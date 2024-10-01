using Myras.Types.ComputationGraphTypes;

namespace Myras.Types
{
    /// <summary>
    /// Represents an operation that manipulates tensors within a computational graph.
    /// </summary>
    public class TensorOperation
    {
        /// <summary>
        /// Gets the type of the tensor operation (e.g., addition, multiplication).
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the input tensors for the operation.
        /// </summary>
        public Tensor[] Inputs { get; }

        /// <summary>
        /// Gets the gradients of the input tensors.
        /// </summary>
        public Tensor[] InputGradients { get; private set; } = [];

        /// <summary>
        /// Gets the output tensors of the operation.
        /// </summary>
        public Tensor[] Outputs { get; private set; } = [];

        /// <summary>
        /// Gets the unique identifier for the tensor operation.
        /// </summary>
        public string Id { get; }

        // The function that performs the operation, taking in a TensorOperation and returning an array of Tensors.
        private readonly Func<TensorOperation, Tensor[]> _function;

        // The derivative (or gradient) function for the operation, taking in a TensorOperation and a gradient node.
        private readonly Func<TensorOperation, ValueNode, Tensor[]> _functionDerivative;

        /// <summary>
        /// Initializes a new instance of the <see cref="TensorOperation"/> class with the given parameters.
        /// </summary>
        /// <param name="type">The type of the operation (e.g., addition, multiplication).</param>
        /// <param name="inputs">The input tensors for the operation.</param>
        /// <param name="function">The function that computes the operation on the tensors.</param>
        /// <param name="functionDerivative">The derivative function used for backpropagation or computing gradients.</param>
        public TensorOperation(
            string type,
            Tensor[] inputs,
            Func<TensorOperation, Tensor[]> function,
            Func<TensorOperation, ValueNode, Tensor[]> functionDerivative
        )
        {
            Type = type;
            Inputs = inputs;
            InputGradients = inputs.Select<Tensor, Tensor>(x => new(x.Shape, 0)).ToArray();
            Id = Guid.NewGuid().ToString();
            _function = function;
            _functionDerivative = functionDerivative;
        }

        /// <summary>
        /// Calls the operation function and returns a single result from the output tensors.
        /// </summary>
        /// <returns>The first tensor from the operation's outputs.</returns>
        public Tensor CallSingleResult()
        {
            return Call()[0];
        }

        /// <summary>
        /// Calls the operation function, computing and setting the output tensors.
        /// </summary>
        /// <returns>An array of output tensors produced by the operation.</returns>
        public Tensor[] Call()
        {
            Outputs = _function(this);
            return Outputs;
        }

        /// <summary>
        /// Calls the derivative function and returns a single result from the input gradients.
        /// </summary>
        /// <param name="gradientNode">The gradient node for which the derivative is computed.</param>
        /// <returns>The first tensor from the operation's input gradients.</returns>
        public Tensor CallDerivativeSingleResult(ValueNode gradientNode)
        {
            return CallDerivative(gradientNode)[0];
        }

        /// <summary>
        /// Calls the derivative function, computing and setting the input gradients.
        /// This is typically used during backpropagation in machine learning models.
        /// </summary>
        /// <param name="gradientNode">The gradient node for which the derivative is computed.</param>
        /// <returns>An array of input gradient tensors.</returns>
        public Tensor[] CallDerivative(ValueNode gradientNode)
        {
            InputGradients = _functionDerivative(this, gradientNode);
            return InputGradients;
        }

        /// <summary>
        /// Returns a string representation of the operation, which is simply the operation type.
        /// </summary>
        /// <returns>The operation type as a string.</returns>
        public override string? ToString()
        {
            return Type;
        }
    }
}
