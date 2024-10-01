namespace Myras.Types.ComputationGraphTypes
{
    /// <summary>
    /// Represents a node in a computation graph that holds a tensor value and its gradient.
    /// </summary>
    public class ValueNode : ComputationGraphNode
    {
        /// <summary>
        /// Gets or sets the tensor value associated with this node.
        /// </summary>
        public Tensor Value { get; set; }

        /// <summary>
        /// Gets or sets the gradient of the tensor value associated with this node.
        /// </summary>
        public Tensor Gradient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueNode"/> class with a specified identifier and tensor value.
        /// The gradient is initialized to zero with the same shape as the value tensor.
        /// </summary>
        /// <param name="id">The unique identifier for the value node.</param>
        /// <param name="value">The tensor value to be held by this node.</param>
        public ValueNode(string id, Tensor value) : base(id)
        {
            Value = value;
            Gradient = new(value.Shape, 0);
        }
    }
}
