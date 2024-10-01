namespace Myras.Types.ComputationGraphTypes
{
    /// <summary>
    /// Represents a node in a computation graph that performs a specific tensor operation.
    /// </summary>
    public class OperationNode : ComputationGraphNode
    {
        /// <summary>
        /// Gets or sets the tensor operation associated with this node.
        /// </summary>
        public TensorOperation Operation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationNode"/> class with a specified identifier and tensor operation.
        /// </summary>
        /// <param name="id">The unique identifier for the operation node.</param>
        /// <param name="operation">The tensor operation to be performed by this node.</param>
        public OperationNode(string id, TensorOperation operation) : base(id)
        {
            Operation = operation;
        }
    }
}
