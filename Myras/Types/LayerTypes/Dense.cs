using Myras.Enums;
using Myras.Extensions;
using Myras.Utils;

namespace Myras.Types.LayerTypes
{
    /// <summary>
    /// Represents a dense (fully connected) layer in a neural network.
    /// Inherits from the <see cref="Layer"/> class.
    /// </summary>
    public class Dense : Layer
    {
        /// <summary>
        /// Gets or sets the weight tensor (kernel) for the dense layer.
        /// </summary>
        public Tensor Kernel { get; set; } = new(0f);

        /// <summary>
        /// Gets or sets the bias tensor for the dense layer.
        /// </summary>
        public Tensor Biases { get; set; } = new(0f);

        /// <summary>
        /// Gets or sets a value indicating whether to use biases in the dense layer.
        /// Defaults to true.
        /// </summary>
        public bool UseBiases { get; set; } = true;

        /// <summary>
        /// Gets or sets the activation function type used in the dense layer.
        /// Defaults to <see cref="ActivationFunctionType.LINEAR"/>.
        /// </summary>
        public ActivationFunctionType ActivationFunction { get; set; } = ActivationFunctionType.LINEAR;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dense"/> class,
        /// setting the layer type to <see cref="LayerType.DENSE"/>.
        /// </summary>
        public Dense()
        {
            Type = LayerType.DENSE;
        }

        /// <summary>
        /// Initializes the dense layer with the specified parameters.
        /// This includes the number of units, the number of units in the previous layer,
        /// batch size, and optional parameters for name, bias usage, and activation function.
        /// </summary>
        /// <param name="units">The number of units in the dense layer.</param>
        /// <param name="unitsPreviousLayer">The number of units in the previous layer.</param>
        /// <param name="batchSize">The batch size for the dense layer.</param>
        /// <param name="name">An optional name for the dense layer.</param>
        /// <param name="useBiases">An optional value indicating whether to use biases.</param>
        /// <param name="activation">An optional activation function type for the dense layer.</param>
        public void Initialize(
            int units,
            int unitsPreviousLayer,
            int batchSize,
            string? name = null,
            bool? useBiases = null,
            ActivationFunctionType? activation = null
        )
        {
            if (useBiases.HasValue) UseBiases = useBiases.Value;

            float minWeight = -0.1f;
            float maxWeight = 0.1f;

            IList<float> weights = [];

            for (int i = 0; i < units * unitsPreviousLayer; i++)
            {
                // Xavier initialization
                float weight = Constants.random.NextFloat() * (maxWeight - minWeight) + minWeight;
                weights.Add(weight);
            }

            IList<float> biases = [];

            for (int i = 0; i < units * unitsPreviousLayer; i++)
            {
                biases.Add(0.01f);
            }

            Kernel = new(new([units, unitsPreviousLayer]), weights);
            Biases = new(new([units]), biases);
            if (activation.HasValue) ActivationFunction = activation.Value;

            Initialize(
                weightsTrainable: [Kernel, Biases],
                weightsNonTrainable: [],
                inputShape: new([units]),
                outputShape: new([units]),
                batchSize: batchSize,
                name: name,
                trainable: true
            );
        }

        /// <summary>
        /// Performs the forward pass of the dense layer.
        /// This method computes the output of the layer based on the input,
        /// applying a linear transformation followed by the specified activation function.
        /// </summary>
        /// <param name="tape">An optional <see cref="GradientTape"/> for automatic differentiation.</param>
        public override void ForwardPass(GradientTape? tape = null)
        {
            Input = LayersPrevious[0].Output;

            Tensor preActivationOutput = MathT.Addition(MathT.DotProduct(Input, MathT.Transpose(Kernel, tape), tape), Biases, tape);

            Tensor activation = ActivationFunction switch
            {
                ActivationFunctionType.RE_LU => MathT.Relu(preActivationOutput, tape),
                ActivationFunctionType.LINEAR => MathT.Linear(preActivationOutput, tape),
                _ => throw new NotImplementedException(),
            };

            Output = activation;
        }
    }
}
