using Myras.Enums;
using Myras.Utils;

namespace Myras.Types.LayerTypes
{
    /// <summary>
    /// Represents an input layer in a neural network.
    /// Inherits from the <see cref="Layer"/> class and initializes its type to <see cref="LayerType.INPUT"/>.
    /// </summary>
    public class Input : Layer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class,
        /// setting the layer type to <see cref="LayerType.INPUT"/>.
        /// </summary>
        public Input()
        {
            Type = LayerType.INPUT;
        }

        /// <summary>
        /// Initializes the input layer with optional parameters for name, shape, and batch size.
        /// Sets weights to empty since input layers do not have trainable or non-trainable weights.
        /// </summary>
        /// <param name="name">An optional name for the input layer.</param>
        /// <param name="shape">An optional <see cref="Shape"/> representing the dimensions of the input data.</param>
        /// <param name="batchSize">An optional batch size for the input layer.</param>
        public void Initialize(
            string? name = null,
            Shape? shape = null,
            int? batchSize = null
        )
        {
            Initialize(
                weightsTrainable: [],
                weightsNonTrainable: [],
                inputShape: shape ?? new([1]),
                outputShape: shape ?? new([1]),
                batchSize: batchSize,
                name: name,
                trainable: false
            );
        }

        /// <summary>
        /// Performs the forward pass of the input layer.
        /// This method computes the output of the layer based on the input using a linear transformation.
        /// </summary>
        /// <param name="tape">An optional <see cref="GradientTape"/> for automatic differentiation.</param>
        public override void ForwardPass(GradientTape? tape = null)
        {
            Output = MathT.Linear(Input, tape);
        }
    }
}
