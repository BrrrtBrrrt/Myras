using Myras.Enums;
using Myras.Types;
using Myras.Types.LayerTypes;

namespace Myras.Utils
{
    /// <summary>
    /// Utility class to assist in creating and configuring neural network layers.
    /// This class provides helper methods to easily create and initialize common layer types
    /// such as Input and Dense layers, used in deep learning models.
    /// </summary>
    public static class Layers
    {
        /// <summary>
        /// Creates and initializes an Input layer with optional shape and batch size parameters.
        /// </summary>
        /// <param name="shape">Optional shape of the input tensor (number of features, dimensions, etc.).</param>
        /// <param name="batchSize">Optional batch size that defines how many samples are processed in one pass.</param>
        /// <returns>Returns an initialized Input layer object.</returns>
        public static Input Input(Shape? shape = null, int? batchSize = null)
        {
            Input layer = new();
            layer.Initialize(shape: shape, batchSize: batchSize);
            return layer;
        }

        /// <summary>
        /// Creates a function to initialize a Dense (fully connected) layer.
        /// The function returned takes a previous layer and creates a Dense layer connected to it.
        /// </summary>
        /// <param name="units">Number of units/neurons in the dense layer.</param>
        /// <param name="useBiases">Optional parameter to indicate whether biases should be used (default is determined internally).</param>
        /// <param name="activation">Optional parameter specifying the activation function to use (e.g., ReLU, Sigmoid).</param>
        /// <returns>
        /// A function that, when given a previous layer, returns an initialized Dense layer.
        /// The Dense layer is connected to the previous layer.
        /// </returns>
        public static Func<Layer, Dense> Dense(int units, bool? useBiases = null, ActivationFunctionType? activation = null)
        {
            return (layerPrevious) =>
            {
                Dense layer = new();
                layer.Initialize(units: units, unitsPreviousLayer: layerPrevious.OutputShape.Dimensions[^1], useBiases: useBiases, batchSize: layerPrevious.BatchSize, activation: activation);
                layer.LayersPrevious.Add(layerPrevious);
                layerPrevious.LayersNext.Add(layer);
                return layer;
            };
        }
    }
}
