using Myras.Enums;

namespace Myras.Types.LayerTypes
{
    /// <summary>
    /// Abstract base class representing a layer in a neural network.
    /// </summary>
    public abstract class Layer
    {
        /// <summary>
        /// Gets or sets the type of the layer.
        /// </summary>
        public LayerType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// Defaults to an empty string.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the layer is trainable.
        /// Defaults to true.
        /// </summary>
        public bool Trainable { get; set; } = true;

        /// <summary>
        /// Gets or sets the list of weights associated with the layer.
        /// </summary>
        public IList<Tensor> Weights { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of trainable weights associated with the layer.
        /// </summary>
        public IList<Tensor> WeightsTrainable { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of non-trainable weights associated with the layer.
        /// </summary>
        public IList<Tensor> WeightsNonTrainable { get; set; } = [];

        /// <summary>
        /// Gets or sets the input tensor for the layer.
        /// </summary>
        public Tensor Input { get; set; } = new(0);

        /// <summary>
        /// Gets or sets the output tensor for the layer.
        /// </summary>
        public Tensor Output { get; set; } = new(0);

        /// <summary>
        /// Gets or sets the shape of the input tensor.
        /// Defaults to a shape of [1, 1].
        /// </summary>
        public Shape InputShape { get; set; } = new([1, 1]);

        /// <summary>
        /// Gets or sets the shape of the output tensor.
        /// Defaults to a shape of [1, 1].
        /// </summary>
        public Shape OutputShape { get; set; } = new([1, 1]);

        /// <summary>
        /// Gets or sets the batch size for the layer.
        /// Defaults to 1.
        /// </summary>
        public int BatchSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets the shape of the batch input tensor.
        /// </summary>
        public Shape BatchInputShape { get; set; } = new([1, 1]);

        /// <summary>
        /// Gets or sets the shape of the batch output tensor.
        /// </summary>
        public Shape BatchOutputShape { get; set; } = new([1, 1]);

        /// <summary>
        /// Gets or sets the list of previous layers in the network.
        /// </summary>
        public IList<Layer> LayersPrevious { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of next layers in the network.
        /// </summary>
        public IList<Layer> LayersNext { get; set; } = [];

        /// <summary>
        /// Initializes the layer with the specified parameters.
        /// </summary>
        /// <param name="weightsTrainable">The list of trainable weights.</param>
        /// <param name="weightsNonTrainable">The list of non-trainable weights.</param>
        /// <param name="inputShape">The shape of the input tensor.</param>
        /// <param name="outputShape">The shape of the output tensor.</param>
        /// <param name="batchSize">The batch size (optional).</param>
        /// <param name="name">The name of the layer (optional).</param>
        /// <param name="trainable">A value indicating whether the layer is trainable (optional).</param>
        internal void Initialize(
            IList<Tensor> weightsTrainable,
            IList<Tensor> weightsNonTrainable,
            Shape inputShape,
            Shape outputShape,
            int? batchSize = null,
            string? name = null,
            bool? trainable = null
        )
        {
            Name = name ?? Guid.NewGuid().ToString();
            Trainable = trainable ?? true;
            WeightsTrainable = weightsTrainable;
            WeightsNonTrainable = weightsNonTrainable;
            Weights = [.. weightsTrainable, .. weightsNonTrainable];
            InputShape = inputShape;
            OutputShape = outputShape;
            BatchSize = batchSize ?? 1;
            BatchInputShape = new([BatchSize, .. inputShape.Dimensions]);
            BatchOutputShape = new([BatchSize, .. outputShape.Dimensions]);
            int inputValuesCount = 1;
            foreach (int size in BatchInputShape.Dimensions)
                inputValuesCount *= size;
            int outputValuesCount = 1;
            foreach (int size in BatchOutputShape.Dimensions)
                outputValuesCount *= size;
            Input = new(BatchInputShape);
            Output = new(BatchOutputShape);
        }

        /// <summary>
        /// Performs the forward pass of the layer, computing the output based on the input.
        /// </summary>
        /// <param name="tape">An optional gradient tape for automatic differentiation.</param>
        public abstract void ForwardPass(GradientTape? tape = null);
    }
}
