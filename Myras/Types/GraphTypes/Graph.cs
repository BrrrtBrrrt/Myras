namespace Myras.Types.GraphTypes
{
    /// <summary>
    /// Represents a generic graph structure that can contain nodes and edges.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier for nodes and edges. Must be non-nullable.</typeparam>
    /// <typeparam name="TNode">The type of nodes in the graph. Must inherit from <see cref="Node{TId}"/>.</typeparam>
    /// <typeparam name="TEdge">The type of edges in the graph. Must inherit from <see cref="Edge{TId}"/>.</typeparam>
    public class Graph<TId, TNode, TEdge>
        where TId : notnull
        where TNode : Node<TId>
        where TEdge : Edge<TId>
    {
        /// <summary>
        /// Gets or sets the list of nodes in the graph.
        /// </summary>
        public List<TNode> Nodes { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of edges in the graph.
        /// </summary>
        public List<TEdge> Edges { get; set; } = [];
    }
}
