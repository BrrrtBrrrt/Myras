namespace Myras.Types.GraphTypes
{
    /// <summary>
    /// Represents an edge in a graph connecting two nodes.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier for the edge. Must be non-nullable.</typeparam>
    public class Edge<TId>
        where TId : notnull
    {
        /// <summary>
        /// Gets or sets the identifier of the first node connected by the edge.
        /// </summary>
        public TId Id1 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the second node connected by the edge.
        /// </summary>
        public TId Id2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge{TId}"/> class with specified node identifiers.
        /// </summary>
        /// <param name="id1">The identifier of the first node.</param>
        /// <param name="id2">The identifier of the second node.</param>
        public Edge(TId id1, TId id2)
        {
            Id1 = id1;
            Id2 = id2;
        }

        /// <summary>
        /// Returns a string representation of the edge.
        /// </summary>
        /// <returns>A string that represents the edge in the format "Id1 -> Id2".</returns>
        public override string? ToString()
        {
            return $"{Id1} -> {Id2}";
        }
    }
}
