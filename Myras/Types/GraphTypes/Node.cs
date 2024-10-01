namespace Myras.Types.GraphTypes
{
    /// <summary>
    /// Represents a node in a graph with a unique identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier for the node. Must be non-nullable.</typeparam>
    public class Node<TId>
        where TId : notnull
    {
        /// <summary>
        /// Gets or sets the unique identifier of the node.
        /// </summary>
        public TId Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{TId}"/> class with a specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the node.</param>
        public Node(TId id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns a string representation of the node.
        /// </summary>
        /// <returns>A string that represents the node's identifier.</returns>
        public override string? ToString()
        {
            return $"{Id}";
        }
    }
}
