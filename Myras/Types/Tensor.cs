namespace Myras.Types
{
    /// <summary>
    /// Represents a multi-dimensional array or tensor that holds data in the form of a matrix.
    /// </summary>
    public class Tensor
    {
        /// <summary>
        /// Gets the matrix values associated with this tensor.
        /// </summary>
        public Matrix Values { get; }

        /// <summary>
        /// Gets the shape of the tensor based on the shape of its matrix values.
        /// </summary>
        public Shape Shape
        {
            get => Values.Shape;
        }

        /// <summary>
        /// Indicates whether this tensor is trainable.
        /// </summary>
        public bool Trainable { get; set; }

        /// <summary>
        /// Gets a unique identifier for this tensor instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the Tensor class with a provided matrix of values.
        /// </summary>
        /// <param name="data">The matrix data to initialize the tensor with.</param>
        /// <param name="trainable">Indicates whether this tensor is trainable (default is true).</param>
        public Tensor(Matrix data, bool trainable = true)
        {
            Values = data;
            Trainable = trainable;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the Tensor class with a specified shape and a list of float values.
        /// </summary>
        /// <param name="shape">The shape of the tensor.</param>
        /// <param name="values">The list of float values used to populate the tensor.</param>
        /// <param name="trainable">Indicates whether this tensor is trainable (default is true).</param>
        public Tensor(Shape shape, IList<float> values, bool trainable = true)
        {
            Values = new(shape, values);
            Trainable = trainable;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the Tensor class with a specified shape and a default value.
        /// All values in the tensor will be initialized to the provided default value.
        /// </summary>
        /// <param name="shape">The shape of the tensor.</param>
        /// <param name="defaultValue">The default value to initialize the tensor with (default is 0).</param>
        /// <param name="trainable">Indicates whether this tensor is trainable (default is true).</param>
        public Tensor(Shape shape, float defaultValue = 0, bool trainable = true)
        {
            Values = new(shape, defaultValue);
            Trainable = trainable;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the Tensor class with an array of values.
        /// </summary>
        /// <param name="values">The array of values used to initialize the tensor.</param>
        /// <param name="trainable">Indicates whether this tensor is trainable (default is true).</param>
        public Tensor(Array values, bool trainable = true)
        {
            Values = new(values);
            Trainable = trainable;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the Tensor class with a single scalar value.
        /// The scalar value will populate a tensor of shape 1x1.
        /// </summary>
        /// <param name="value">The scalar value to initialize the tensor with.</param>
        /// <param name="trainable">Indicates whether this tensor is trainable (default is true).</param>
        public Tensor(float value, bool trainable = true)
        {
            Values = new(value);
            Trainable = trainable;
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns a string representation of the tensor.
        /// </summary>
        /// <returns>A string that represents the tensor's matrix values.</returns>
        public override string? ToString()
        {
            return Values.ToString();
        }
    }
}
