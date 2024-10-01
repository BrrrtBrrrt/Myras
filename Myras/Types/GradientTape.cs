using Myras.Types.ComputationGraphTypes;
using Myras.Types.GraphTypes;

namespace Myras.Types
{
    /// <summary>
    /// The <see cref="GradientTape"/> class allows for the automatic differentiation 
    /// of tensor operations, enabling the calculation of gradients for machine learning models.
    /// </summary>
    public class GradientTape : IDisposable
    {
        private readonly ComputationGraph _computationGraph = new();

        // Flag to indicate if the object is disposed
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientTape"/> class.
        /// </summary>
        public GradientTape()
        {

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GradientTape"/> class, releasing unmanaged resources.
        /// </summary>
        ~GradientTape()
        {
            Dispose(false);
        }

        /// <summary>
        /// Computes the gradient of a dependent variable with respect to a single independent variable.
        /// </summary>
        /// <param name="dependentVariable">The tensor for which the gradient is calculated.</param>
        /// <param name="independentVariable">The tensor with respect to which the gradient is calculated.</param>
        /// <returns>A tensor representing the gradient of the dependent variable.</returns>
        /// <exception cref="ArgumentException">Thrown if the dependent variable is not recorded in the computation graph.</exception>
        public Tensor GetGradient(Tensor dependentVariable, Tensor independentVariable)
        {
            return GetGradients(dependentVariable, [independentVariable])[0];
        }

        /// <summary>
        /// Computes the gradients of a dependent variable with respect to multiple independent variables.
        /// </summary>
        /// <param name="dependentVariable">The tensor for which the gradient is calculated.</param>
        /// <param name="independentVariables">An array of tensors with respect to which the gradients are calculated.</param>
        /// <returns>An array of tensors representing the gradients of the dependent variable with respect to each independent variable.</returns>
        /// <exception cref="ArgumentException">Thrown if the dependent variable is not recorded in the computation graph.</exception>
        public Tensor[] GetGradients(Tensor dependentVariable, params Tensor[] independentVariables)
        {
            if (_computationGraph.Nodes.FirstOrDefault(x => x.Id == dependentVariable.Id) is not ValueNode node)
                throw new ArgumentException("Dependant variable not recorded in the computation graph");

            node.Gradient = new(node.Value.Shape, 1);

            OperationNode? operation = _computationGraph.Nodes
                .FirstOrDefault(x => x is OperationNode operation && _computationGraph.Edges.Any(y => y.Id1 == operation.Id && y.Id2 == node.Id)) as OperationNode ??
                throw new ArgumentException("No operation node found in computation graph for dependent variable");

            ValueNode valueNode = _computationGraph.Nodes.FirstOrDefault(x => x.Id == dependentVariable.Id) as ValueNode ??
                throw new ArgumentException("No value node found in computation graph for dependent variable");

            BackpropagateGradients(operation, valueNode, []);

            return _computationGraph.Nodes
                .Where(x => x is ValueNode node && independentVariables.Any(y => y.Id == node.Id))
                .Cast<ValueNode>()
                .Select(x => x.Gradient)
                .ToArray();
        }

        /// <summary>
        /// Performs backpropagation of gradients through the computation graph.
        /// </summary>
        /// <param name="operationNode">The operation node through which to backpropagate.</param>
        /// <param name="valueNode">The value node representing the dependent variable.</param>
        /// <param name="visitedIds">A list to track visited nodes and prevent cycles.</param>
        private void BackpropagateGradients(OperationNode operationNode, ValueNode valueNode, List<string> visitedIds)
        {
            string visitId = $"{operationNode.Id}___{valueNode.Id}";
            if (visitedIds.Any(x => x == visitId))
                return;
            visitedIds.Add(visitId);

            operationNode.Operation.CallDerivative(valueNode);

            for (int i = 0; i < operationNode.Operation.InputGradients.Length; i++)
            {
                Tensor gradient = operationNode.Operation.InputGradients[i];
                Tensor input = operationNode.Operation.Inputs[i];
                ValueNode node = _computationGraph.Nodes.FirstOrDefault(x => x.Id == input.Id) as ValueNode ??
                    throw new Exception("Value node not found in computation graph");
                node.Gradient = gradient;
            }

            foreach (OperationNode predecessorOperation in _computationGraph.Nodes.Where(x => x is OperationNode operation && operation.Operation.Outputs.Any(y => operationNode.Operation.Inputs.Any(z => y.Id == z.Id))).Cast<OperationNode>())
                foreach (ValueNode output in _computationGraph.Nodes.Where(x => predecessorOperation.Operation.Outputs.Any(y => x.Id == y.Id)).Cast<ValueNode>())
                    BackpropagateGradients(predecessorOperation, output, visitedIds);
        }

        /// <summary>
        /// Records a tensor input in the computation graph.
        /// </summary>
        /// <param name="input">The tensor to be recorded.</param>
        public void Record(Tensor input)
        {
            if (_computationGraph.Nodes.Any(x => x.Id == input.Id))
                return;
            _computationGraph.Nodes.Add(new ValueNode(input.Id, input));
        }

        /// <summary>
        /// Records a tensor operation in the computation graph.
        /// </summary>
        /// <param name="operation">The tensor operation to be recorded.</param>
        /// <exception cref="InvalidOperationException">Thrown if the operation is already recorded in the computation graph.</exception>
        public void Record(TensorOperation operation)
        {
            if (_computationGraph.Nodes.Any(x => x.Id == operation.Id))
                throw new InvalidOperationException("Operations cannot be recorded twice");

            foreach (Tensor input in operation.Inputs)
            {
                _computationGraph.Edges.Add(new Edge<string>(input.Id, operation.Id));
                if (_computationGraph.Nodes.Any(x => x.Id == input.Id))
                    continue;
                _computationGraph.Nodes.Add(new ValueNode(input.Id, input));
            }

            _computationGraph.Nodes.Add(new OperationNode(operation.Id, operation));

            foreach (Tensor output in operation.Outputs)
            {
                _computationGraph.Edges.Add(new Edge<string>(operation.Id, output.Id));
                if (_computationGraph.Nodes.Any(x => x.Id == output.Id))
                    continue;
                _computationGraph.Nodes.Add(new ValueNode(output.Id, output));
            }
        }

        /// <summary>
        /// Disposes the <see cref="GradientTape"/> and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent finalizer from running if already disposed
        }

        /// <summary>
        /// Releases the resources used by the <see cref="GradientTape"/> class.
        /// </summary>
        /// <param name="disposing">A value indicating whether the method has been called directly or from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _computationGraph.Nodes.Clear();
                _computationGraph.Edges.Clear();

                _disposed = true; // Set the disposed flag to true
            }
        }
    }
}
