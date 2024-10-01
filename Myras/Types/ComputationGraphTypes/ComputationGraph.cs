using Myras.Types.GraphTypes;
using System.Text;

namespace Myras.Types.ComputationGraphTypes
{
    /// <summary>
    /// Represents a computation graph, which is a directed graph where nodes represent operations or tensors,
    /// and edges represent the data flow between them.
    /// </summary>
    public class ComputationGraph : Graph<string, ComputationGraphNode, Edge<string>>
    {
        /// <summary>
        /// Returns a string representation of the computation graph in the DOT format.
        /// </summary>
        /// <returns>
        /// A string that represents the computation graph, formatted for use with graph visualization tools.
        /// The format includes node labels and shapes based on their types, as well as directed edges 
        /// between the nodes.
        /// </returns>
        public override string? ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"digraph ComputationGraph {{");

            foreach (ComputationGraphNode node in Nodes)
            {
                string label;
                string shape;
                if (node is OperationNode operation)
                {
                    label = operation.Operation.Type;
                    shape = "hexagon";
                }
                else
                {
                    label = "Tensor";
                    shape = "rectangle";
                }
                sb.AppendLine($"  \"{node.Id}\" [label=\"{label}\" shape={shape}]");
            }

            foreach (Edge<string> edge in Edges)
                sb.AppendLine($"  \"{edge.Id1}\" -> \"{edge.Id2}\"");

            sb.Append('}');

            return sb.ToString();
        }
    }
}
