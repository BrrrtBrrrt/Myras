using Myras.Types.GraphTypes;

namespace Myras.Types.ComputationGraphTypes
{
    /// <summary>
    /// Represents a node in a computation graph.
    /// This abstract class serves as a base for different types of computation graph nodes,
    /// such as operation nodes or value nodes.
    /// </summary>
    public abstract class ComputationGraphNode : Node<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputationGraphNode"/> class with a specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the computation graph node.</param>
        protected ComputationGraphNode(string id) : base(id)
        {

        }
    }
}
