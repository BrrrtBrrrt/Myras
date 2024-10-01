using Myras.Types;

namespace Myras.Services
{
    /// <summary>
    /// Abstract base class for optimizer services used in training machine learning models.
    /// </summary>
    internal abstract class OptimizerService
    {
        /// <summary>
        /// Initializes the optimizer with the specified parameters.
        /// </summary>
        /// <param name="parameters">A dictionary containing parameters needed for the optimizer.</param>
        public abstract void Initialize(Dictionary<string, dynamic> parameters);

        /// <summary>
        /// Optimizes the given trainable weights using the specified gradients.
        /// </summary>
        /// <param name="trainableWeights">An array of tensors representing the trainable weights of the model.</param>
        /// <param name="gradients">An array of tensors representing the gradients corresponding to the trainable weights.</param>
        public abstract void Optimize(Tensor[] trainableWeights, Tensor[] gradients);
    }
}
